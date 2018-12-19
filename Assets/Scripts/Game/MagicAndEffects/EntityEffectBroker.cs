// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// Enumerates available magic effects and coordinates their instantiation.
    /// Also coordinates "magic rounds" at 5-second intervals for all entity EntityEffectManager components active in scene.
    /// </summary>
    public class EntityEffectBroker : MonoBehaviour
    {
        #region Fields

        const int maxCatchupDays = 2;   // Equal to 2880 game minutes or magic rounds

        public const int CurrentSpellVersion = 1;
        public const int MinimumSupportedSpellVersion = 1;

        public const TargetTypes TargetFlags_None = TargetTypes.None;
        public const TargetTypes TargetFlags_Self = TargetTypes.CasterOnly;
        public const TargetTypes TargetFlags_Other = TargetTypes.ByTouch | TargetTypes.SingleTargetAtRange | TargetTypes.AreaAroundCaster | TargetTypes.AreaAtRange;
        public const TargetTypes TargetFlags_All = TargetTypes.CasterOnly | TargetTypes.ByTouch | TargetTypes.SingleTargetAtRange | TargetTypes.AreaAroundCaster | TargetTypes.AreaAtRange;

        public const ElementTypes ElementFlags_None = ElementTypes.None;
        public const ElementTypes ElementFlags_MagicOnly = ElementTypes.Magic;
        public const ElementTypes ElementFlags_All = ElementTypes.Fire | ElementTypes.Cold | ElementTypes.Poison | ElementTypes.Shock | ElementTypes.Magic;

        public const MagicCraftingStations MagicCraftingFlags_None = MagicCraftingStations.None;
        public const MagicCraftingStations MagicCraftingFlags_All = MagicCraftingStations.SpellMaker | MagicCraftingStations.PotionMaker | MagicCraftingStations.ItemMaker;

        uint lastGameMinute;
        int magicRoundsSinceStartup;

        readonly Dictionary<int, string> classicEffectMapping = new Dictionary<int, string>();
        readonly Dictionary<string, BaseEntityEffect> magicEffectTemplates = new Dictionary<string, BaseEntityEffect>();
        readonly Dictionary<int, BaseEntityEffect> potionEffectTemplates = new Dictionary<int, BaseEntityEffect>();
        readonly Dictionary<int, SpellRecord.SpellRecordData> classicSpells = new Dictionary<int, SpellRecord.SpellRecordData>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of game-minute "magic rounds" since startup.
        /// Reset to current game time when player loads a game or starts a new game.
        /// </summary>
        public int MagicRoundsSinceStartup
        {
            get { return magicRoundsSinceStartup; }
        }

        #endregion

        #region Constructors

        public EntityEffectBroker()
        {
            SaveLoadManager.OnLoad += SaveLoadManager_OnLoad;
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
            StartGameBehaviour.OnStartGame += StartGameBehaviour_OnStartGame;
        }

        #endregion

        #region Unity

        void Start()
        {
            // Create a dictionary of classic spells
            RebuildClassicSpellsDict();

            // Enumerate classes implementing an effect and create an instance to use as factory
            // TODO: Provide an external method for mods to register custom effects without reflections
            magicEffectTemplates.Clear();
            IEnumerable<BaseEntityEffect> effectTemplates = ReflectiveEnumerator.GetEnumerableOfType<BaseEntityEffect>();
            foreach (BaseEntityEffect effect in effectTemplates)
            {
                // Effect must present a key
                if (string.IsNullOrEmpty(effect.Key))
                    continue;

                // Store template
                // TODO: Allow effect overwrite for modded effects
                if (effect.VariantCount > 1)
                {
                    // Store one template per variant for multi-effects
                    for (int i = 0; i < effect.VariantCount; i++)
                    {
                        BaseEntityEffect variantEffect = CloneEffect(effect) as BaseEntityEffect;
                        variantEffect.CurrentVariant = i;
                        magicEffectTemplates.Add(variantEffect.Key, variantEffect);
                        IndexEffectRecipes(variantEffect);
                    }
                }
                else
                {
                    // Just store singleton effect
                    magicEffectTemplates.Add(effect.Key, effect);
                    IndexEffectRecipes(effect);
                }

                // Map classic key when defined - output error in case of classic key conflict
                // NOTE: Mods should also be able to replace classic effect - will need to handle substitutions later
                // NOTE: Not mapping effect keys for non spell effects at this time
                byte groupIndex, subGroupIndex;
                BaseEntityEffect.ClassicEffectFamily family;
                BaseEntityEffect.ReverseClasicKey(effect.Properties.ClassicKey, out groupIndex, out subGroupIndex, out family);
                if (effect.Properties.ClassicKey != 0 && family == BaseEntityEffect.ClassicEffectFamily.Spells)
                {
                    if (classicEffectMapping.ContainsKey(effect.Properties.ClassicKey))
                    {
                        Debug.LogErrorFormat("EntityEffectBroker: Detected duplicate classic effect key for {0} ({1}, {2})", effect.Key, groupIndex, subGroupIndex);
                    }
                    else
                    {
                        classicEffectMapping.Add(effect.Properties.ClassicKey, effect.Key);
                    }
                }
            }
        }

        void Update()
        {
            // Don't tick if lastGameMinute not set (pre-init)
            if (lastGameMinute == 0)
                return;

            // Every game minute passing is another magic round, so work out how many minutes have passed
            // During normal play magic rounds will fire once every game minute like clockwork
            uint gameMinute = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            uint minutesPassed = gameMinute - lastGameMinute;

            // However, fast travel and prison time can step time forwards by potentially millions of minutes
            // Most effects either expire or have constant state and simply don't require that many ticks
            // Setting a 2-day cap (2880 minutes) on catchup rounds limits spell framework from spinning "on empty" and wasting cycles
            // Maximum catchup rounds have been tuned based on the following observations:
            //  - Maximum spell duration of 60+60 per 1 level is < 2000 minutes (most durations are much, much lower)
            //  - Long-running effects such as drains and magic items are constant state and only require a single tick
            //  - Poisons and diseases have their own catch-up mechanisms and only require a single tick
            //  - Vast majority of effects will expire within 120 minutes so 2880 is still a very high limit
            //  - Running empty magic rounds isn't very costly so a small amount of inefficiency is OK
            int catchupRounds = Mathf.Min(maxCatchupDays * DaggerfallDateTime.MinutesPerDay, (int)minutesPassed);

            // Execute magic rounds for each minute passed up to limit
            // This ensure effects continue to operate during rest or fast travel
            if (catchupRounds > 0)
            {
                //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
                //long startTime = stopwatch.ElapsedMilliseconds;

                for (int i = 0; i < catchupRounds; i++)
                {
                    RaiseOnNewMagicRoundEvent();
                    magicRoundsSinceStartup++;
                }
                lastGameMinute = gameMinute;

                //long totalTime = stopwatch.ElapsedMilliseconds - startTime;
                //Debug.LogFormat("Time to run {0} magic rounds: {1}ms", catchupRounds, totalTime);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets number of potion properties assigned to this effect.
        /// Effect must allow the potion maker crafting station and define potion recipes.
        /// Effect 
        /// </summary>
        /// <param name="effect">Input effect.</param>
        /// <returns>Number of recipes for this effect.</returns>
        public int GetEffectPotionRecipeCount(IEntityEffect effect)
        {
            // Effect must be valid and support potion crafting
            if (effect != null && effect.PotionProperties.Recipes != null &&
                (effect.Properties.AllowedCraftingStations & MagicCraftingStations.PotionMaker) == MagicCraftingStations.PotionMaker)
            {
                return effect.PotionProperties.Recipes.Length;
            }

            return 0;
        }

        /// <summary>
        /// Gets PotionRecipe from IEntityEffect.
        /// Effect must allow the potion maker crafting station and define potion recipes.
        /// </summary>
        /// <param name="effect">Input effect.</param>
        /// <param name="variant">Variant index, if more than one exists.</param>
        /// <returns>PotionRecipe if the effect has one, otherwise null.</returns>
        public PotionRecipe GetEffectPotionRecipe(IEntityEffect effect, int variant = 0)
        {
            // Effect must be valid and support potion crafting
            if (effect != null && effect.PotionProperties.Recipes != null &&
                (effect.Properties.AllowedCraftingStations & MagicCraftingStations.PotionMaker) == MagicCraftingStations.PotionMaker)
            {
                // Check variant index does not exceed length of recipes array
                if (effect.PotionProperties.Recipes.Length - 1 < variant)
                    return null;

                // Variant index must have a recipe assigned
                if (!effect.PotionProperties.Recipes[variant].HasRecipe())
                    return null;

                return effect.PotionProperties.Recipes[variant];
            }

            return null;
        }

        /// <summary>
        /// Gets IEntityEffect from PotionRecipe.
        /// </summary>
        /// <param name="recipe">Input recipe.</param>
        /// <returns>IEntityEffect if this recipe is linked to an effect, otherwise null.</returns>
        public IEntityEffect GetPotionRecipeEffect(PotionRecipe recipe)
        {
            if (recipe != null)
            {
                int recipeKey = recipe.GetHashCode();
                if (potionEffectTemplates.ContainsKey(recipeKey))
                    return potionEffectTemplates[recipeKey];
            }

            return null;
        }

        /// <summary>
        /// Gets PotionRecipe from effect that matches the recipeKey provided.
        /// </summary>
        /// <param name="recipeKey">Hashcode of a set of ingredients.</param>
        /// <returns>PotionRecipe if the key matches one from an effect, otherwise null.</returns>
        public PotionRecipe GetPotionRecipe(int recipeKey)
        {
            if (potionEffectTemplates.ContainsKey(recipeKey))
            {
                foreach (PotionRecipe recipe in potionEffectTemplates[recipeKey].PotionProperties.Recipes)
                    if (recipe.GetHashCode() == recipeKey)
                        return recipe;
            }
            return null;
        }

        /// <summary>
        /// Get recipeKeys for all registered potion recipes.
        /// </summary>
        /// <returns>List of int recipeKeys</returns>
        public List<int> GetPotionRecipeKeys()
        {
            return new List<int>(potionEffectTemplates.Keys);
        }

        /// <summary>
        /// Logs a summary of how many recipes ingredients are used in so new recipes can choose to use little used ingredients.
        /// Intended for mod devs, used by invoking 'ingredUsage' console command.
        /// </summary>
        public void LogRecipeIngredientUsage()
        {
            Dictionary<int, int> ingredCounts = new Dictionary<int, int>();
            foreach (int key in potionEffectTemplates.Keys)
            {
                PotionRecipe potionRecipe = GetPotionRecipe(key);
                foreach (PotionRecipe.Ingredient ingred in potionRecipe.Ingredients)
                {
                    if (ingredCounts.ContainsKey(ingred.id))
                        ingredCounts[ingred.id]++;
                    else
                        ingredCounts.Add(ingred.id, 1);
                }
            }
            foreach (int key in ingredCounts.Keys)
            {
                DaggerfallConnect.FallExe.ItemTemplate ingredientTemplate = DaggerfallUnity.Instance.ItemHelper.GetItemTemplate(key);
                Debug.LogFormat("{0} recipes use: {1}", ingredCounts[key], ingredientTemplate.name);
            }
        }

        /// <summary>
        /// Gets group names of registered effects.
        /// </summary>
        /// <param name="sortAlpha">True to sort group names by alpha.</param>
        /// <param name="craftingStations">Filter by allowed magic crafting stations.</param>
        /// <returns>Array of group names.</returns>
        public string[] GetGroupNames(bool sortAlpha = true, MagicCraftingStations craftingStations = MagicCraftingFlags_All)
        {
            List<string> groupNames = new List<string>();

            // Get group list
            foreach (BaseEntityEffect effect in magicEffectTemplates.Values)
            {
                // Skip effects not fitting at least one station requirement
                if ((craftingStations & effect.Properties.AllowedCraftingStations) == 0)
                    continue;

                // Ignore duplicate groups
                if (!groupNames.Contains(effect.Properties.GroupName))
                    groupNames.Add(effect.Properties.GroupName);
            }

            // Sort if required
            if (sortAlpha)
                groupNames.Sort();

            return groupNames.ToArray();
        }

        /// <summary>
        /// Gets subgroup names of registered effects.
        /// </summary>
        /// <param name="groupName">The group name to collect subgroups of.</param>
        /// <param name="sortAlpha">True to sort subgroup names by alpha.</param>
        /// <param name="craftingStations">Filter by allowed magic crafting stations.</param>
        /// <returns>Array of subgroup names.</returns>
        public string[] GetSubGroupNames(string groupName, bool sortAlpha = true, MagicCraftingStations craftingStations = MagicCraftingFlags_All)
        {
            List<string> subGroupNames = new List<string>();

            foreach (BaseEntityEffect effect in magicEffectTemplates.Values.Where(effect => effect.Properties.GroupName == groupName))
            {
                // Skip effects not fitting at least one station requirement
                if ((craftingStations & effect.Properties.AllowedCraftingStations) == 0)
                    continue;

                subGroupNames.Add(effect.Properties.SubGroupName);
            }

            // Sort if required
            if (sortAlpha)
                subGroupNames.Sort();

            return subGroupNames.ToArray();
        }

        /// <summary>
        /// Gets interface to all effect templates belonging to group name.
        /// </summary>
        /// <param name="groupName">The group name to collect effects from.</param>
        /// <param name="craftingStations">Filter by allowed magic crafting stations.</param>
        /// <returns>List of effect templates.</returns>
        public List<IEntityEffect> GetEffectTemplates(string groupName, MagicCraftingStations craftingStations = MagicCraftingFlags_All)
        {
            List<IEntityEffect> effectTemplates = new List<IEntityEffect>();

            foreach (IEntityEffect effectTemplate in magicEffectTemplates.Values.Where(effect => effect.Properties.GroupName == groupName))
            {
                // Skip effects not fitting at least one station requirement
                if ((craftingStations & effectTemplate.Properties.AllowedCraftingStations) == 0)
                    continue;

                effectTemplates.Add(effectTemplate);
            }

            return effectTemplates;
        }

        /// <summary>
        /// Determine if a key exists in the templates dictionary.
        /// </summary>
        /// <param name="key">Key for template.</param>
        /// <returns>True if template exists.</returns>
        public bool HasEffectTemplate(string key)
        {
            return magicEffectTemplates.ContainsKey(key);
        }

        /// <summary>
        /// Determine if a classic key exists in the templates dictionary.
        /// </summary>
        /// <param name="classicKey">Classic key for template.</param>
        /// <returns>True if template exists.</returns>
        public bool HasEffectTemplate(int classicKey)
        {
            return classicEffectMapping.ContainsKey(classicKey);
        }

        /// <summary>
        /// Gets interface to effect template.
        /// Use this to query properties to all effects with this key.
        /// </summary>
        /// <param name="key">Effect key.</param>
        /// <returns>Interface to effect template only (has default effect settings).</returns>
        public IEntityEffect GetEffectTemplate(string key)
        {
            if (!HasEffectTemplate(key))
                return null;

            return magicEffectTemplates[key];
        }

        /// <summary>
        /// Gets interface to effect template from classic key.
        /// </summary>
        /// <param name="classicKey">Classic key.</param>
        /// <returns>Interface to effect template only (has default effect settings).</returns>
        public IEntityEffect GetEffectTemplate(int classicKey)
        {
            if (!HasEffectTemplate(classicKey))
                return null;

            return magicEffectTemplates[classicEffectMapping[classicKey]];
        }

        /// <summary>
        /// Creates a new instance of effect with specified settings.
        /// Use this to create a new effect with unique settings for actual use.
        /// </summary>
        /// <param name="effectEntry">EffectEntry with effect settings.</param>
        /// <returns>Interface to new effect instance.</returns>
        public IEntityEffect InstantiateEffect(EffectEntry effectEntry)
        {
            return InstantiateEffect(effectEntry.Key, effectEntry.Settings);
        }

        /// <summary>
        /// Creates a new instance of effect with specified settings.
        /// Use this to create a new effect with unique settings for actual use.
        /// </summary>
        /// <param name="key">Effect key.</param>
        /// <param name="settings">Effect settings.</param>
        /// <returns>Interface to new effect instance.</returns>
        public IEntityEffect InstantiateEffect(string key, EffectSettings settings)
        {
            if (!HasEffectTemplate(key))
                return null;

            IEntityEffect effectTemplate = magicEffectTemplates[key];
            IEntityEffect effectInstance = Activator.CreateInstance(effectTemplate.GetType()) as IEntityEffect;
            effectInstance.Settings = settings;
            effectInstance.CurrentVariant = effectTemplate.CurrentVariant;

            return effectInstance;
        }

        /// <summary>
        /// Clone an effect and its settings.
        /// </summary>
        /// <param name="effect">Effect to clone.</param>
        /// <returns>Interface to cloned effect.</returns>
        public IEntityEffect CloneEffect(IEntityEffect effect)
        {
            IEntityEffect clone = Activator.CreateInstance(effect.GetType()) as IEntityEffect;
            clone.Settings = effect.Settings;

            return clone;
        }

        /// <summary>
        /// Gets classic spell record data.
        /// </summary>
        /// <param name="id">ID of spell.</param>
        /// <param name="spellOut">Spell record data (if found).</param>
        /// <returns>True if spell found, otherwise false.</returns>
        public bool GetClassicSpellRecord(int id, out SpellRecord.SpellRecordData spellOut)
        {
            if (classicSpells.ContainsKey(id))
            {
                spellOut = classicSpells[id];
                return true;
            }

            spellOut = new SpellRecord.SpellRecordData();
            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Rebuilds dictionary of classic spells by re-reading SPELLS.STD.
        /// </summary>
        void RebuildClassicSpellsDict()
        {
            classicSpells.Clear();

            List<SpellRecord.SpellRecordData> spells = DaggerfallSpellReader.ReadSpellsFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, DaggerfallSpellReader.DEFAULT_FILENAME));
            foreach (SpellRecord.SpellRecordData spell in spells)
            {
                // "Holy Touch" and "Holy Word" have same ID but different properties
                // Not sure of best way to handle - just ignoring duplicate for now
                if (classicSpells.ContainsKey(spell.index))
                {
                    //Debug.LogErrorFormat("RebuildClassicSpellsDict found duplicate key {0} for spell {1}. Existing spell={2}", spell.index, spell.spellName, classicSpells[spell.index].spellName);
                    continue;
                }

                classicSpells.Add(spell.index, spell);
            }
        }

        void IndexEffectRecipes(BaseEntityEffect effect)
        {
            // Must have at least one recipe
            int recipeCount = GetEffectPotionRecipeCount(effect);
            if (recipeCount == 0)
                return;

            Debug.LogFormat("Effect '{0}' has {1} potion recipes:", effect.Key, recipeCount);

            // Index all recipes for this effect
            for (int i = 0; i < recipeCount; i++)
            {
                // Get recipe variant
                PotionRecipe recipe = GetEffectPotionRecipe(effect, i);
                if (recipe != null)
                {
                    // Add potion effect or log error if collision
                    int recipeKey = recipe.GetHashCode();
                    if (!potionEffectTemplates.ContainsKey(recipeKey))
                    {
                        potionEffectTemplates.Add(recipeKey, effect);
                        Debug.LogFormat("'{0}' recipe {1} [key={2}] ingredients: {3}", effect.Key, i, recipeKey, recipe.ToString());
                    }
                    else
                    {
                        Debug.LogErrorFormat("EnityEffectBroker: Already contains potion recipe key {0} for ingredients: {1}", recipeKey, recipe.ToString());
                    }
                }
            }
        }

        // Called when game starts or loaded, after world time has been set/restored
        // Syncs initial magic round timer with game time for counting magic rounds
        void InitMagicRoundTimer()
        {
            lastGameMinute = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            magicRoundsSinceStartup = 0;
            //Debug.LogFormat("Resetting magic round timer to minute {0}", lastGameMinute);
        }

        public void StartGameBehaviour_OnStartGame(object sender, EventArgs e)
        {
            InitMagicRoundTimer();
        }

        void StartGameBehaviour_OnNewGame()
        {
            InitMagicRoundTimer();
        }

        void SaveLoadManager_OnLoad(SaveData_v1 saveData)
        {
            InitMagicRoundTimer();
        }

        #endregion

        #region Classic Spell Record Conversion Helpers

        /// <summary>
        /// Generate EffectBundleSettings from classic SpellRecordData.
        /// </summary>
        /// <param name="spellRecordData">Classic spell record data.</param>
        /// <param name="bundleType">Type of bundle to create.</param>
        /// <param name="effectBundleSettingsOut">Effect bundle created by conversion.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool ClassicSpellRecordDataToEffectBundleSettings(SpellRecord.SpellRecordData spellRecordData, BundleTypes bundleType, out EffectBundleSettings effectBundleSettingsOut)
        {
            // Spell record data must have effect records
            if (spellRecordData.effects == null || spellRecordData.effects.Length == 0)
            {
                effectBundleSettingsOut = new EffectBundleSettings();
                return false;
            }

            // Create bundle
            effectBundleSettingsOut = new EffectBundleSettings()
            {
                Version = EntityEffectBroker.CurrentSpellVersion,
                BundleType = bundleType,
                TargetType = ClassicTargetIndexToTargetType(spellRecordData.rangeType),
                ElementType = ClassicElementIndexToElementType(spellRecordData.element),
                Name = spellRecordData.spellName,
                IconIndex = spellRecordData.icon,
            };

            // Assign effects
            List<EffectEntry> foundEffects = new List<EffectEntry>();
            for (int i = 0; i < spellRecordData.effects.Length; i++)
            {
                // Skip unused effect slots
                if (spellRecordData.effects[i].type == -1)
                    continue;

                // Get entry from effect
                EffectEntry entry;
                if (!ClassicEffectRecordToEffectEntry(spellRecordData.effects[i], out entry))
                    continue;

                // Assign to valid effects
                foundEffects.Add(entry);
            }

            // Must have assigned at least one valid effect
            if (foundEffects.Count == 0)
                return false;

            // Assign effects to bundle
            effectBundleSettingsOut.Effects = foundEffects.ToArray();

            return true;
        }

        /// <summary>
        /// Generate EffectEntry from classic EffectRecordData.
        /// </summary>
        /// <param name="effectRecordData">Classic effect record data.</param>
        /// <returns>EffectEntry.</returns>
        public bool ClassicEffectRecordToEffectEntry(SpellRecord.EffectRecordData effectRecordData, out EffectEntry effectEntryOut)
        {
            // Get template
            IEntityEffect effectTemplate = GetEffectTemplateFromClassicEffectRecordData(effectRecordData);
            if (effectTemplate == null)
            {
                effectEntryOut = new EffectEntry();
                return false;
            }

            // Get settings and create entry
            EffectSettings effectSettings = ClassicEffectRecordToEffectSettings(
                effectRecordData,
                effectTemplate.Properties.SupportDuration,
                effectTemplate.Properties.SupportChance,
                effectTemplate.Properties.SupportMagnitude);
            effectEntryOut = new EffectEntry(effectTemplate.Key, effectSettings);

            return true;
        }

        /// <summary>
        /// Generate EffectSettings from classic EffectRecordData.
        /// </summary>
        /// <param name="effectRecordData">Classic effect record data.</param>
        /// <returns>EffectSettings.</returns>
        public EffectSettings ClassicEffectRecordToEffectSettings(SpellRecord.EffectRecordData effectRecordData, bool supportDuration, bool supportChance, bool supportMagnitude)
        {
            EffectSettings effectSettings = BaseEntityEffect.DefaultEffectSettings();
            if (supportDuration)
            {
                effectSettings.DurationBase = effectRecordData.durationBase;
                effectSettings.DurationPlus = effectRecordData.durationMod;
                effectSettings.DurationPerLevel = effectRecordData.durationPerLevel;
            }

            if (supportChance)
            {
                effectSettings.ChanceBase = effectRecordData.chanceBase;
                effectSettings.ChancePlus = effectRecordData.chanceMod;
                effectSettings.ChancePerLevel = effectRecordData.chancePerLevel;
            }

            if (supportMagnitude)
            {
                effectSettings.MagnitudeBaseMin = effectRecordData.magnitudeBaseLow;
                effectSettings.MagnitudeBaseMax = effectRecordData.magnitudeBaseHigh;
                effectSettings.MagnitudePlusMin = effectRecordData.magnitudeLevelBase;
                effectSettings.MagnitudePlusMax = effectRecordData.magnitudeLevelHigh;
                effectSettings.MagnitudePerLevel = effectRecordData.magnitudePerLevel;
            }

            return effectSettings;
        }

        /// <summary>
        /// Maps classic target index to TargetTypes.
        /// </summary>
        /// <param name="targetIndex">Classic target index.</param>
        /// <returns>TargetTypes.</returns>
        public TargetTypes ClassicTargetIndexToTargetType(int targetIndex)
        {
            switch (targetIndex)
            {
                case 0:
                    return TargetTypes.CasterOnly;
                case 1:
                    return TargetTypes.ByTouch;
                case 2:
                    return TargetTypes.SingleTargetAtRange;
                case 3:
                    return TargetTypes.AreaAroundCaster;
                case 4:
                    return TargetTypes.AreaAtRange;
                default:
                    throw new Exception("ClassicTargetIndexToTargetType() encountered an unknown target index.");
            }
        }

        /// <summary>
        /// Maps classic element index to ElementTypes enum.
        /// </summary>
        /// <param name="elementIndex">Classic element index.</param>
        /// <returns>ElementTypes.</returns>
        public ElementTypes ClassicElementIndexToElementType(int elementIndex)
        {
            switch (elementIndex)
            {
                case 0:
                    return ElementTypes.Fire;
                case 1:
                    return ElementTypes.Cold;
                case 2:
                    return ElementTypes.Poison;
                case 3:
                    return ElementTypes.Shock;
                case 4:
                    return ElementTypes.Magic;
                default:
                    throw new Exception("ClassicElementIndexToElementType() encountered an unknown element index.");
            }
        }

        /// <summary>
        /// Gets effect template from classic effect record data, if one is available.
        /// </summary>
        /// <param name="effectRecordData">Classic effect record data.</param>
        /// <returns>IEntityEffect of template found or null if no matching template found.</returns>
        public IEntityEffect GetEffectTemplateFromClassicEffectRecordData(SpellRecord.EffectRecordData effectRecordData)
        {
            // Ignore unused effect
            if (effectRecordData.type == -1)
                return null;

            // Get effect type/subtype
            int type, subType;
            type = effectRecordData.type;
            subType = (effectRecordData.subType < 0) ? 255 : effectRecordData.subType; // Entity effect keys use 255 instead of -1 for subtype

            // Check if effect template is implemented for this slot - instant fail if effect not implemented
            int classicKey = BaseEntityEffect.MakeClassicKey((byte)type, (byte)subType);

            return GameManager.Instance.EntityEffectBroker.GetEffectTemplate(classicKey);
        }

        /// <summary>
        /// Checks if all classic effects map to an implemented IEntityEffect template.
        /// </summary>
        /// <param name="spellRecordData">Classic spell record data.</param>
        /// <returns>True if all classic effects map to an effect template.</returns>
        public bool AllEffectsImplemented(SpellRecord.SpellRecordData spellRecordData)
        {
            // There are up to 3 effects per spell
            int foundEffects = 0;
            for (int i = 0; i < spellRecordData.effects.Length; i++)
            {
                // Try to get effect template
                IEntityEffect effectTemplate = GetEffectTemplateFromClassicEffectRecordData(spellRecordData.effects[i]);
                if (effectTemplate == null)
                    continue;

                // Otherwise effect is implemented and can be counted
                foundEffects++;
            }

            // Must have at least one effect counted (handles all 3 slots being -1/-1)
            if (foundEffects == 0)
                return false;

            return true;
        }

        #endregion

        #region Events

        // OnNewMagicRound
        public delegate void OnNewMagicRoundEventHandler();
        public static event OnNewMagicRoundEventHandler OnNewMagicRound;
        protected virtual void RaiseOnNewMagicRoundEvent()
        {
            if (OnNewMagicRound != null)
                OnNewMagicRound();
        }

        #endregion
    }

    /// <summary>
    /// Find all subclasses of type and create instance.
    /// </summary>
    public static class ReflectiveEnumerator
    {
        static ReflectiveEnumerator() { }

        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            return objects;
        }
    }
}
