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
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// Enumerates available magic effects and coordinates their instantiation.
    /// Also coordinates "magic rounds" at 5-second intervals for all entity EntityEffectManager components active in scene.
    /// </summary>
    public class EntityEffectBroker : MonoBehaviour
    {
        #region Fields

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

        const float roundInterval = 5.0f;

        int magicRoundsSinceStartup = 0;
        float roundTimer = 0f;

        Dictionary<int, string> classicEffectMapping = new Dictionary<int, string>();
        Dictionary<string, BaseEntityEffect> magicEffectTemplates = new Dictionary<string, BaseEntityEffect>();
        Dictionary<int, BaseEntityEffect> potionEffectTemplates = new Dictionary<int, BaseEntityEffect>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of 5-second "magic rounds" since startup.
        /// </summary>
        public int MagicRoundsSinceStartup
        {
            get { return magicRoundsSinceStartup; }
        }

        #endregion

        #region Unity

        void Start()
        {
            // Enumerate classes implementing an effect and create an instance to use as factory
            // TODO: Provide an external method for mods to register custom effects without reflections
            magicEffectTemplates.Clear();
            IEnumerable<BaseEntityEffect> effectTemplates = ReflectiveEnumerator.GetEnumerableOfType<BaseEntityEffect>();
            foreach(BaseEntityEffect effect in effectTemplates)
            {
                // Store template
                magicEffectTemplates.Add(effect.Key, effect);
                IndexEffectRecipes(effect);

                // Map classic key when defined - output error in case of classic key conflict
                // NOTE: Mods should also be able to replace classic effect - will need to handle substitutions later
                if (effect.Properties.ClassicKey != 0)
                {
                    if (classicEffectMapping.ContainsKey(effect.Properties.ClassicKey))
                    {
                        byte groupIndex, subGroupIndex;
                        BaseEntityEffect.ReverseClasicKey(effect.Properties.ClassicKey, out groupIndex, out subGroupIndex);
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
            // Increment magic round timer when not paused
            if (!GameManager.IsGamePaused)
            {
                roundTimer += Time.deltaTime;
                if (roundTimer > roundInterval)
                {
                    RaiseOnNewMagicRoundEvent();
                    magicRoundsSinceStartup++;
                    roundTimer = 0;
                    //Debug.Log("New magic round starting.");
                }
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
                if (!potionEffectTemplates.ContainsKey(recipeKey))
                    return potionEffectTemplates[recipeKey];
            }

            return null;
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
            foreach(BaseEntityEffect effect in magicEffectTemplates.Values)
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

            return effectInstance;
        }

        #endregion

        #region Private Methods

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
