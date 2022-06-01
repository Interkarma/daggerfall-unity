// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Numidium
// 
// Notes:
//

using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.FallExe;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// Interface to an entity effect.
    /// </summary>
    public interface IEntityEffect : IMacroContextProvider
    {
        /// <summary>
        /// Gets effect properties.
        /// </summary>
        EffectProperties Properties { get; }

        /// <summary>
        /// Gets or sets current effect settings.
        /// </summary>
        EffectSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets enchantment param for enchantment effects.
        /// If this property is null then effect is not a live enchantment.
        /// </summary>
        EnchantmentParam? EnchantmentParam { get; set; }

        /// <summary>
        /// Gets effect potion properties (if any).
        /// </summary>
        PotionProperties PotionProperties { get; }

        /// <summary>
        /// Gets the caster entity behaviour of this effect (can return null).
        /// </summary>
        DaggerfallEntityBehaviour Caster { get; }

        /// <summary>
        /// Gets key from properties.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Group display name (used by crafting stations).
        /// </summary>
        string GroupName { get; }

        /// <summary>
        /// SubGroup display name (used by crafting stations).
        /// </summary>
        string SubGroupName { get; }

        /// <summary>
        /// Description for spellmaker.
        /// </summary>
        TextFile.Token[] SpellMakerDescription { get; }

        /// <summary>
        /// Description for spellbook.
        /// </summary>
        TextFile.Token[] SpellBookDescription { get; }

        /// <summary>
        /// Gets display name from properties or construct one from Group+SubGroup text in properties.
        /// This allows effects to set a custom display name or just roll with automatic names.
        /// Daggerfall appears to use first token of spellmaker/spellbook description, but we want more control for effect mods.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets or sets number of magic rounds remaining.
        /// </summary>
        int RoundsRemaining { get; set; }

        /// <summary>
        /// Gets flag stating if effect passed a chance check on start.
        /// If always false if effect does not support chance component.
        /// </summary>
        bool ChanceSuccess { get; }

        /// <summary>
        /// Gets array DaggerfallStats.Count items wide.
        /// Array items represent Strength, Intelligence, Willpower, etc.
        /// Effect implementation should set modifier values for stats when part of payload.
        /// For example, a "Damage Strength" effect would set the current modifier for Strength (such as -5 to Strength).
        /// Use (int)DFCareer.State.StatName to get index.
        /// </summary>
        int[] StatMods { get; }

        /// <summary>
        /// Gets array DaggerfallStats.Count items wide.
        /// Array items represent Strength, Intelligence, Willpower, etc.
        /// Allows an effect to temporarily override stat maximum value.
        /// </summary>
        int[] StatMaxMods { get; }

        /// <summary>
        /// Get array DaggerfallSkills.Count items wide.
        /// Array items represent Medical, Etiquette, Streetwise, etc.
        /// Effect implementation should set modifier values for skills when part of payload.
        /// For example, a "Tongues" effect would set the current modifier for all language skills (such as +5 to Dragonish, +5 to Giantish, and so on).
        /// Use (int)DFCareer.Skills.SkillName to get index.
        /// </summary>
        int[] SkillMods { get; }

        /// <summary>
        /// Gets array DaggerfallResistances.Count items wide.
        /// Array items represent Fire, Cold, Poison/Disease, Shock, Magic.
        /// Effect implementation should set modifier values for resistances when part of payload.
        /// For example, a "Resist Fire" effect would set the current modifier for Fire resistance (such as +30 to Fire resistance).
        /// Use (int)DFCareer.Resistances.ResistanceName to get index.
        /// </summary>
        int[] ResistanceMods { get; }

        /// <summary>
        /// Gets or sets parent bundle for this effect.
        /// Will be null for template effects.
        /// </summary>
        LiveEffectBundle ParentBundle { get; set; }

        /// <summary>
        /// True if effect has ended by calling End();
        /// </summary>
        bool HasEnded { get; }

        /// <summary>
        /// Gets total number of variants for multi-effects.
        /// </summary>
        int VariantCount { get; }

        /// <summary>
        /// Sets current variant for multi-effects.
        /// </summary>
        int CurrentVariant { get; set; }

        /// <summary>
        /// True if spell effect always lands regardless of saving throws.
        /// </summary>
        bool BypassSavingThrows { get; }

        /// <summary>
        /// Called by an EntityEffectManager when parent bundle is attached to host entity.
        /// Use this for setup or immediate work performed only once.
        /// </summary>
        void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null);

        /// <summary>
        /// Called by an EntityEffect manage when parent bundle is resumed from save.
        /// </summary>
        void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null);

        /// <summary>
        /// Use this for work performed every frame.
        /// </summary>
        void ConstantEffect();

        /// <summary>
        /// Use this for any work performed every magic round.
        /// </summary>
        void MagicRound();

        /// <summary>
        /// Called when bundle lifetime is at an end.
        /// Use this for any wrap-up work.
        /// </summary>
        void End();

        /// <summary>
        /// Perform a chance roll on this effect based on chance settings.
        /// Can be used by custom chance effects that need to roll chance other than at start.
        /// </summary>
        bool RollChance();

        /// <summary>
        /// Gets array of enchantment settings supported by this effect.
        /// Can return a null or empty array, especially if effect not an enchantment.
        /// </summary>
        EnchantmentSettings[] GetEnchantmentSettings();

        /// <summary>
        /// Helper to get a specific enchantment setting based on param.
        /// Can return null if enchantment with param not found.
        /// </summary>
        EnchantmentSettings? GetEnchantmentSettings(EnchantmentParam param);

        /// <summary>
        /// Helper to check if properties contain the specified item maker flags.
        /// </summary>
        bool HasItemMakerFlags(ItemMakerFlags flags);

        /// <summary>
        /// Helper to check if properties contain the specified enchantment payload flags
        /// </summary>
        bool HasEnchantmentPayloadFlags(EnchantmentPayloadFlags flags);

        /// <summary>
        /// Enchantment payload callback for enchantment to perform custom execution based on context.
        /// These callbacks are performed directly from template, not from a live instance of effect. Do not store state in effect during callbacks.
        /// Not used by EnchantmentPayloadFlags.Held - rather, an effect instance bundle is assigned to entity's effect manager to execute as normal.
        /// </summary>
        PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0);

        /// <summary>
        /// Gets related enchantments that will be forced onto item along with this enchantment.
        /// Only used by Soul Bound in classic gameplay.
        /// </summary>
        /// <returns></returns>
        ForcedEnchantmentSet? GetForcedEnchantments(EnchantmentParam? param = null);

        /// <summary>
        /// Enchantment can flag that it is exclusive to one or more enchantments in array provided.
        /// Used by enchanting window to prevent certain enchantments from being selected together.
        /// </summary>
        bool IsEnchantmentExclusiveTo(EnchantmentSettings[] settingsToTest, EnchantmentParam? comparerParam = null);

        /// <summary>
        /// Get effect state data to serialize.
        /// </summary>
        object GetSaveData();

        /// <summary>
        /// Restore effect state from serialized data.
        /// </summary>
        void RestoreSaveData(object dataIn);
    }

    /// <summary>
    /// Base implementation of an entity effect.
    /// Entity effects are like "actions" for spells, potions, items, advantages, diseases, etc.
    /// They generally perform work against one or more entities (e.g. damage or restore health).
    /// Some effects perform highly custom operations unique to player (e.g. anchor/teleport UI).
    /// Magic effects are scripted in C# so they have full access to engine and UI as required.
    /// Classic magic effects are included in build for cross-platform compatibility.
    /// Custom effects can be added later using mod system (todo:).
    /// </summary>
    public abstract partial class BaseEntityEffect : IEntityEffect
    {
        #region Fields

        protected EffectProperties properties = new EffectProperties();
        protected EffectSettings settings = new EffectSettings();
        protected PotionProperties potionProperties = new PotionProperties();
        protected DaggerfallEntityBehaviour caster = null;
        protected EntityEffectManager manager = null;
        protected int variantCount = 1;
        protected int currentVariant = 0;
        protected bool bypassSavingThrows = false;

        int roundsRemaining;
        bool chanceSuccess = false;
        int[] statMods = new int[DaggerfallStats.Count];
        int[] statMaxMods = new int[DaggerfallStats.Count];
        int[] skillMods = new int[DaggerfallSkills.Count];
        int[] resistanceMods = new int[DaggerfallResistances.Count];
        LiveEffectBundle parentBundle;
        bool effectEnded = false;

        #endregion

        #region Enums

        public enum ClassicEffectFamily
        {
            Spells,
            PowersAndSideEffects,
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected BaseEntityEffect()
        {
            // Set default properties
            properties.ShowSpellIcon = true;
            properties.SupportDuration = false;
            properties.SupportChance = false;
            properties.SupportMagnitude = false;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.None;
            properties.MagicSkill = DFCareer.MagicSkills.None;

            // Set default settings
            settings = DefaultEffectSettings();

            // Allow effect to set own properties
            SetProperties();
            SetPotionProperties();
        }

        #endregion

        #region IEntityEffect Properties

        public virtual EffectProperties Properties
        {
            get { return properties; }
        }

        public EffectSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public EnchantmentParam? EnchantmentParam { get; set; }

        public virtual PotionProperties PotionProperties
        {
            get { return potionProperties; }
        }

        public DaggerfallEntityBehaviour Caster
        {
            get { return caster; }
        }

        public virtual int RoundsRemaining
        {
            get { return roundsRemaining; }
            set { roundsRemaining = value; }
        }

        public virtual bool ChanceSuccess
        {
            get { return chanceSuccess; }
        }

        public int[] StatMods
        {
            get { return statMods; }
        }

        public int[] StatMaxMods
        {
            get { return statMaxMods; }
        }

        public int[] SkillMods
        {
            get { return skillMods; }
        }

        public int[] ResistanceMods
        {
            get { return resistanceMods; }
        }

        public string Key
        {
            get { return Properties.Key; }
        }

        public virtual string GroupName
        {
            get { return string.Empty; }
        }

        public virtual string SubGroupName
        {
            get { return string.Empty; }
        }

        public virtual string DisplayName
        {
            get { return GetDisplayName(); }
        }

        public virtual TextFile.Token[] SpellMakerDescription
        {
            get { return null; }
        }

        public virtual TextFile.Token[] SpellBookDescription
        {
            get { return null; }
        }

        public LiveEffectBundle ParentBundle
        {
            get { return parentBundle; }
            set { parentBundle = value; }
        }

        public bool HasEnded
        {
            get { return effectEnded; }
            protected set { effectEnded = value; }
        }

        public int VariantCount
        {
            get { return variantCount; }
        }

        public int CurrentVariant
        {
            get { return currentVariant; }
            set { currentVariant = Mathf.Clamp(value, 0, variantCount - 1); }
        }

        public bool BypassSavingThrows
        {
            get { return bypassSavingThrows; }
        }

        #endregion

        #region IEntityEffect Virtual Methods

        public abstract void SetProperties();

        public virtual void SetPotionProperties()
        {
        }

        /// <summary>
        /// Gets all enchantment settings supported by this effect.
        /// Effects supporting MagicCraftingStations.ItemMaker must implement this method to become usable in item maker.
        /// The effect must also be able to execute enchantment payload when handed back settings. 
        /// </summary>
        /// <returns>EnchantmentSettings array. Can return null or empty array.</returns>
        public virtual EnchantmentSettings[] GetEnchantmentSettings()
        {
            return null;
        }

        /// <summary>
        /// Helper to get specific enchantment based on param.
        /// </summary>
        /// <param name="param">Param of enchantment to retrieve.</param>
        /// <returns>EnchantmentSettings for specificed param or null if not found.</returns>
        public virtual EnchantmentSettings? GetEnchantmentSettings(EnchantmentParam param)
        {
            // Get all enchantment settings for this effect
            EnchantmentSettings[] allSettings = GetEnchantmentSettings();
            if (allSettings == null || allSettings.Length == 0)
                return null;

            // Locate matching param
            bool usingCustomParam = !string.IsNullOrEmpty(param.CustomParam);
            foreach(EnchantmentSettings settings in allSettings)
            {
                if (usingCustomParam && param.CustomParam == settings.CustomParam)
                    return settings;
                else if (!usingCustomParam && param.ClassicParam == settings.ClassicParam)
                    return settings;
            }

            return null;
        }

        /// <summary>
        /// Helper to check properties carry specified item maker flags.
        /// </summary>
        /// <param name="flags">Flags to check.</param>
        /// <returns>True if flags specified.</returns>
        public virtual bool HasItemMakerFlags(ItemMakerFlags flags)
        {
            return (Properties.ItemMakerFlags & flags) == flags;
        }

        /// <summary>
        /// Helper to check if properties contain the specified enchantment payload flags
        /// </summary>
        /// <param name="flags">Flags to check.</param>
        /// <returns>True if flags specified.</returns>
        public virtual bool HasEnchantmentPayloadFlags(EnchantmentPayloadFlags flags)
        {
            return (Properties.EnchantmentPayloadFlags & flags) == flags;
        }

        /// <summary>
        /// Enchantment payload callback for enchantment to perform custom execution based on context.
        /// These callbacks are performed directly from template, not from a live instance of effect. Do not store state in effect during callbacks.
        /// Not used by EnchantmentPayloadFlags.Held - rather, an effect instance bundle is assigned to entity's effect manager to execute as normal.
        /// </summary>
        public virtual PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            return null;
        }

        /// <summary>
        /// Enchantment can flag that it is exclusive to one or more enchantments in array provided.
        /// Used by enchanting window to prevent certain enchantments from being selected together.
        /// </summary>
        public virtual bool IsEnchantmentExclusiveTo(EnchantmentSettings[] settingsToTest, EnchantmentParam? comparerParam = null)
        {
            return false;
        }

        /// <summary>
        /// Gets related enchantments that will be forced onto item along with this enchantment.
        /// Only used by Soul Bound in classic gameplay.
        /// </summary>
        public virtual ForcedEnchantmentSet? GetForcedEnchantments(EnchantmentParam? param = null)
        {
            return null;
        }

        /// <summary>
        /// Starts effect running when first attached to an entity.
        /// Executes a MagicRound() tick immediately.
        /// Child classes must call base.Start() when overriding.
        /// NOTE: Start() is only called when effect is first instantiated - it not called again on load, see Resume().
        /// </summary>
        public virtual void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            this.manager = manager;
            this.caster = caster;
            SetDuration();
            SetChanceSuccess();
        }

        /// <summary>
        /// Restarts effect running after deserialization. Does not execute a MagicRound() tick.
        /// </summary>
        public virtual void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            this.manager = manager;
            this.caster = caster;
            roundsRemaining = effectData.roundsRemaining;
            chanceSuccess = effectData.chanceSuccess;
            statMods = effectData.statMods;
            statMaxMods = (effectData.statMaxMods != null) ? effectData.statMaxMods : new int[DaggerfallStats.Count];
            skillMods = effectData.skillMods;
            variantCount = effectData.variantCount;
            currentVariant = effectData.currentVariant;
            effectEnded = effectData.effectEnded;
        }

        /// <summary>
        /// Called to perform any cleanup at end of lifetime, or when manually removed from host.
        /// </summary>
        public virtual void End()
        {
            effectEnded = true;
        }

        /// <summary>
        /// Called for effects that need to perform work each frame, such as setting a toggle in entity.
        /// </summary>
        public virtual void ConstantEffect()
        {
        }

        /// <summary>
        /// Called to execute effect payload on host and count down a magic round.
        /// Child classes must call base.MagicRound() when overriding to properly count rounds.
        /// </summary>
        public virtual void MagicRound()
        {
            RemoveRound();
        }

        /// <summary>
        /// Called to remove a magic round.
        /// Child classes should call base.RemoveRound() when overriding to properly count rounds.
        /// Otherwise child class will need to manually count rounds.
        /// </summary>
        /// <returns></returns>
        protected virtual int RemoveRound()
        {
            if (roundsRemaining <= 0)
                return 0;
            else
                return --roundsRemaining;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the attribute modifier of this effect.
        /// </summary>
        /// <param name="stat">Attribute to query.</param>
        /// <returns>Current attribute modifier.</returns>
        public int GetAttributeMod(DFCareer.Stats stat)
        {
            if (stat == DFCareer.Stats.None)
                return 0;

            return statMods[(int)stat];
        }

        /// <summary>
        /// Gets the attribute maximum modifier of this effect.
        /// </summary>
        /// <param name="stat">Attribute to query.</param>
        /// <returns>Current attribute maximum modifier.</returns>
        public int GetAttributeMaximumMod(DFCareer.Stats stat)
        {
            if (stat == DFCareer.Stats.None)
                return 0;

            return statMaxMods[(int)stat];
        }

        /// <summary>
        /// Gets the skill modifier of the effect.
        /// </summary>
        /// <param name="skill">Skill to query.</param>
        /// <returns>Current skill modifier.</returns>
        protected int GetSkillMod(DFCareer.Skills skill)
        {
            if (skill == DFCareer.Skills.None)
                return 0;

            return skillMods[(int)skill];
        }

        /// <summary>
        /// Heal attribute damage by amount.
        /// Does nothing if this effect does not damage attributes.
        /// Attribute will not heal past 0.
        /// </summary>
        /// <param name="stat">Attribute to heal.</param>
        /// <param name="amount">Amount to heal. Must be positive value.</param>
        public virtual void HealAttributeDamage(DFCareer.Stats stat, int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("EntityEffect.HealAttributeDamage() received a negative value for amount - ignoring.");
                return;
            }

            int result = GetAttributeMod(stat) + amount;
            if (result > 0)
                result = 0;

            SetStatMod(stat, result);
            Debug.LogFormat("Healed {0}'s {1} by {2} points", GetPeeredEntityBehaviour(manager).name, stat.ToString(), amount);
        }

        /// <summary>
        /// Cure all attribute damage by this effect.
        /// </summary>
        public virtual void CureAttributeDamage()
        {
            for (int i = 0; i < DaggerfallStats.Count; i++)
            {
                int amount = GetAttributeMod((DFCareer.Stats)i);
                if (amount < 0)
                    HealAttributeDamage((DFCareer.Stats)i, Mathf.Abs(amount));
            }
        }

        /// <summary>
        /// Heal skill damage by amount.
        /// Does nothing if this effect does not damage skills.
        /// Skill will not heal past 0.
        /// </summary>
        /// <param name="skill">Skill to heal.</param>
        /// <param name="amount">Amount to heal. Must be positive value.</param>
        public virtual void HealSkillDamage(DFCareer.Skills skill, int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("EntityEffect.HealSkillDamage() received a negative value for amount - ignoring.");
                return;
            }

            int result = GetSkillMod(skill) + amount;
            if (result > 0)
                result = 0;

            SetSkillMod(skill, result);
            Debug.LogFormat("Healed {0}'s {1} by {2} points", GetPeeredEntityBehaviour(manager).name, skill.ToString(), amount);
        }

        /// <summary>
        /// Cure all skill damage by this effect.
        /// </summary>
        public virtual void CureSkillDamage()
        {
            for (int i = 0; i < DaggerfallSkills.Count; i++)
            {
                int amount = GetSkillMod((DFCareer.Skills)i);
                if (amount < 0)
                    HealSkillDamage((DFCareer.Skills)i, Mathf.Abs(amount));
            }
        }

        /// <summary>
        /// Checks if all damaged attributes are healed back to 0.
        /// </summary>
        /// <returns>True if all attributes have returned to baseline.</returns>
        public bool AllAttributesHealed()
        {
            for (int i = 0; i < StatMods.Length; i++)
            {
                if (StatMods[i] < 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if all damaged skills are healed back to 0.
        /// </summary>
        /// <returns>True if all skills have returned to baseline.</returns>
        public bool AllSkillsHealed()
        {
            for (int i = 0; i < SkillMods.Length; i++)
            {
                if (SkillMods[i] < 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets final chance value based on caster level and effect settings.
        /// </summary>
        /// <returns>Chance value.</returns>
        public int ChanceValue()
        {
            int casterLevel = (caster) ? FormulaHelper.CalculateCasterLevel(caster.Entity, this) : 1;
            //Debug.LogFormat("{5} ChanceValue {0} = base + plus * (level/chancePerLevel) = {1} + {2} * ({3}/{4})", settings.ChanceBase + settings.ChancePlus * (int)Mathf.Floor(casterLevel / settings.ChancePerLevel), settings.ChanceBase, settings.ChancePlus, casterLevel, settings.ChancePerLevel, Key);
            return settings.ChanceBase + settings.ChancePlus * (int)Mathf.Floor(casterLevel / settings.ChancePerLevel);
        }

        /// <summary>
        /// Performs a chance roll for this effect based on chance settings.
        /// </summary>
        /// <returns>True if chance roll succeeded.</returns>
        public virtual bool RollChance()
        {
            if (!Properties.SupportChance)
                return false;

            bool outcome = Dice100.SuccessRoll(ChanceValue());

            //Debug.LogFormat("Effect '{0}' has a {1}% chance of succeeding and rolled {2} for a {3}", Key, ChanceValue(), roll, (outcome) ? "success" : "fail");

            return outcome;
        }

        /// <summary>
        /// Helper to compare the settings of this effect with another effect.
        /// Used to determine if the Duration, Chance, Magnitude settings are equivalent in both effects.
        /// </summary>
        /// <param name="other">Other effect for comparison.</param>
        public virtual bool CompareSettings(IEntityEffect other)
        {
            return this.Settings.Equals(other.Settings);
        }

        #endregion

        #region Protected Helpers

        protected DaggerfallEntityBehaviour GetPeeredEntityBehaviour(EntityEffectManager manager)
        {
            // Return cached entity behaviour or attempt to get component directly
            if (manager && manager.EntityBehaviour)
                return manager.EntityBehaviour;
            else
                return manager.GetComponent<DaggerfallEntityBehaviour>();
        }

        protected int GetMagnitude(DaggerfallEntityBehaviour caster = null)
        {
            if (caster == null)
                Debug.LogWarningFormat("GetMagnitude() for {0} has no caster. Using caster level 1 for magnitude.", Properties.Key);

            if (manager == null)
                Debug.LogWarningFormat("GetMagnitude() for {0} has no parent manager.", Properties.Key);

            int magnitude = 0;
            if (Properties.SupportMagnitude)
            {
                int casterLevel = (caster) ? FormulaHelper.CalculateCasterLevel(caster.Entity, this) : 1;
                int baseMagnitude = UnityEngine.Random.Range(settings.MagnitudeBaseMin, settings.MagnitudeBaseMax + 1);
                int plusMagnitude = UnityEngine.Random.Range(settings.MagnitudePlusMin, settings.MagnitudePlusMax + 1);
                int multiplier = (int)Mathf.Floor(casterLevel / settings.MagnitudePerLevel);
                magnitude = baseMagnitude + plusMagnitude * multiplier;
            }

            int initialMagnitude = magnitude;
            if (ParentBundle.targetType != TargetTypes.CasterOnly)
                magnitude = FormulaHelper.ModifyEffectAmount(this, manager.EntityBehaviour.Entity, magnitude);

            // Output "Save versus spell made." when magnitude is fully reduced to 0 by saving throw
            if (initialMagnitude > 0 && magnitude == 0)
                DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("saveVersusSpellMade"));

            return magnitude;
        }

        protected void PlayerAggro()
        {
            // Caster must be player
            if (caster != GameManager.Instance.PlayerEntityBehaviour)
                return;

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Cause aggro based on attack source
            entityBehaviour.HandleAttackFromSource(caster);
        }

        protected void SetStatMod(DFCareer.Stats stat, int value)
        {
            if (stat == DFCareer.Stats.None)
                return;

            statMods[(int)stat] = value;
        }

        protected void SetStatMaxMod(DFCareer.Stats stat, int value)
        {
            if (stat == DFCareer.Stats.None)
                return;

            statMaxMods[(int)stat] = value;
        }

        protected void ChangeStatMod(DFCareer.Stats stat, int amount)
        {
            if (stat == DFCareer.Stats.None)
                return;

            statMods[(int)stat] += amount;
        }

        protected void ChangeStatMaxMod(DFCareer.Stats stat, int amount)
        {
            if (stat == DFCareer.Stats.None)
                return;

            statMaxMods[(int)stat] += amount;
        }

        protected void SetSkillMod(DFCareer.Skills skill, int value)
        {
            if (skill == DFCareer.Skills.None)
                return;

            skillMods[(int)skill] = value;
        }

        protected void ChangeSkillMod(DFCareer.Skills skill, int amount)
        {
            if (skill == DFCareer.Skills.None)
                return;

            skillMods[(int)skill] += amount;
        }

        protected void SetResistanceMod(DFCareer.Elements resistance, int amount)
        {
            if (resistance == DFCareer.Elements.None)
                return;

            resistanceMods[(int)resistance] = amount;
        }

        protected void ChanceResistanceMod(DFCareer.Elements resistance, int amount)
        {
            if (resistance == DFCareer.Elements.None)
                return;

            resistanceMods[(int)resistance] += amount;
        }

        protected void AssignPotionRecipes(params PotionRecipe[] recipes)
        {
            if (recipes == null || recipes.Length == 0)
                return;

            potionProperties.Recipes = recipes;
        }

        #endregion

        #region Private Methods

        string GetDisplayName()
        {
            // Manufacture a default display name from group names
            // Effects can override DisplayName property to set a custom display name
            string groupName = GroupName;
            string subGroupName = SubGroupName;
            if (!string.IsNullOrEmpty(groupName) && !string.IsNullOrEmpty(subGroupName))
                return string.Format("{0} {1}", groupName, subGroupName);
            else if (!string.IsNullOrEmpty(groupName) && string.IsNullOrEmpty(subGroupName))
                return groupName;
            else
                return TextManager.Instance.GetLocalizedText("noName");
        }

        void SetDuration()
        {
            int casterLevel = (caster) ? FormulaHelper.CalculateCasterLevel(caster.Entity, this) : 1;
            if (Properties.SupportDuration)
            {
                // Multiplier clamped at 1 or player can lose a round depending on spell settings and level
                int durationPerLevelMultiplier = (int)Mathf.Floor(casterLevel / settings.DurationPerLevel);
                if (durationPerLevelMultiplier < 1)
                    durationPerLevelMultiplier = 1;
                roundsRemaining = settings.DurationBase + settings.DurationPlus * durationPerLevelMultiplier;
            }
            else
                roundsRemaining = 0;

            //Debug.LogFormat("Effect '{0}' will run for {1} magic rounds", Key, roundsRemaining);
        }

        void SetChanceSuccess()
        {
            chanceSuccess = RollChance();
        }

        #endregion

        #region Static Methods

        public static EffectSettings DefaultEffectSettings()
        {
            EffectSettings defaultSettings = new EffectSettings();

            // Default duration is 1 + 1 per level
            defaultSettings.DurationBase = 1;
            defaultSettings.DurationPlus = 1;
            defaultSettings.DurationPerLevel = 1;

            // Default chance is 1 + 1 per level
            defaultSettings.ChanceBase = 1;
            defaultSettings.ChancePlus = 1;
            defaultSettings.ChancePerLevel = 1;

            // Default magnitude is 1-1 + 1-1 per level
            defaultSettings.MagnitudeBaseMin = 1;
            defaultSettings.MagnitudeBaseMax = 1;
            defaultSettings.MagnitudePlusMin = 1;
            defaultSettings.MagnitudePlusMax = 1;
            defaultSettings.MagnitudePerLevel = 1;

            return defaultSettings;
        }

        public static EffectSettings SetEffectDuration(EffectSettings settings, int durationBase, int durationPlus, int durationPerLevel)
        {
            settings.DurationBase = durationBase;
            settings.DurationPlus = durationPlus;
            settings.DurationPerLevel = durationPerLevel;

            return settings;
        }

        public static EffectSettings SetEffectChance(EffectSettings settings, int chanceBase, int chancePlus, int chancePerLevel)
        {
            settings.ChanceBase = chanceBase;
            settings.ChancePlus = chancePlus;
            settings.ChancePerLevel = chancePerLevel;

            return settings;
        }

        public static EffectSettings SetEffectMagnitude(EffectSettings settings, int magnitudeBaseMin, int magnitudeBaseMax, int magnitudePlusMin, int magnitudePlusMax, int magnitudePerLevel)
        {
            settings.MagnitudeBaseMin = magnitudeBaseMin;
            settings.MagnitudeBaseMax = magnitudeBaseMax;
            settings.MagnitudePlusMin = magnitudePlusMin;
            settings.MagnitudePlusMax = magnitudePlusMax;
            settings.MagnitudePerLevel = magnitudePerLevel;

            return settings;
        }

        public static int MakeClassicKey(byte groupIndex, byte subgroupIndex, ClassicEffectFamily family = ClassicEffectFamily.Spells)
        {
            return ((int)family << 16) + (groupIndex << 8) + subgroupIndex;
        }

        public static void ReverseClasicKey(int key, out byte groupIndex, out byte subgroupIndex, out ClassicEffectFamily family)
        {
            family = (ClassicEffectFamily)(key >> 16);
            groupIndex = (byte)(key >> 8);
            subgroupIndex = (byte)(key & 0xff);
        }

        public static EffectCosts MakeEffectCosts(float costA, float costB, float offsetGold = 0)
        {
            EffectCosts costs = new EffectCosts();
            costs.OffsetGold = offsetGold;
            costs.CostA = costA;
            costs.CostB = costB;

            return costs;
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Override to provide save data for effect.
        /// Not all effects need to be stateful.
        /// </summary>
        public virtual object GetSaveData()
        {
            return null;
        }

        /// <summary>
        /// Override to restore save data for effect.
        /// </summary>
        public virtual void RestoreSaveData(object dataIn)
        {
        }

        #endregion
    }
}
