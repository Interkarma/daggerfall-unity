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
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Formulas;

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

        protected const string textDatabase = "ClassicEffects";

        protected EffectProperties properties = new EffectProperties();
        protected EffectSettings settings = new EffectSettings();
        protected PotionProperties potionProperties = new PotionProperties();
        protected DaggerfallEntityBehaviour caster = null;
        protected EntityEffectManager manager = null;
        protected int variantCount = 1;
        protected int currentVariant = 0;

        int roundsRemaining;
        bool chanceSuccess = false;
        int[] statMods = new int[DaggerfallStats.Count];
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
            properties.GroupName = string.Empty;
            properties.SubGroupName = string.Empty;
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

        public string DisplayName
        {
            get { return GetDisplayName(); }
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

        #endregion

        #region IEntityEffect Virtual Methods

        public abstract void SetProperties();

        public virtual void SetPotionProperties()
        {
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

            return false;
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
                Debug.LogWarningFormat("GetMagnitude() for {0} has no caster.", Properties.Key);

            if (manager == null)
                Debug.LogWarningFormat("GetMagnitude() for {0} has no parent manager.", Properties.Key);

            int magnitude = 0;
            if (Properties.SupportMagnitude)
            {
                int casterLevel = (caster) ? caster.Entity.Level : 1;
                int baseMagnitude = UnityEngine.Random.Range(settings.MagnitudeBaseMin, settings.MagnitudeBaseMax + 1);
                int plusMagnitude = UnityEngine.Random.Range(settings.MagnitudePlusMin, settings.MagnitudePlusMax + 1);
                int multiplier = (int)Mathf.Floor(casterLevel / settings.MagnitudePerLevel);
                magnitude = baseMagnitude + plusMagnitude * multiplier;
            }

            if (ParentBundle.targetType != TargetTypes.CasterOnly)
                magnitude = FormulaHelper.ModifyEffectAmount(this, manager.EntityBehaviour.Entity, magnitude);

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

        protected void ChangeStatMod(DFCareer.Stats stat, int amount)
        {
            if (stat == DFCareer.Stats.None)
                return;

            statMods[(int)stat] += amount;
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

        protected int ChanceValue()
        {
            int casterLevel = (caster) ? caster.Entity.Level : 1;
            //Debug.LogFormat("{5} ChanceValue {0} = base + plus * (level/chancePerLevel) = {1} + {2} * ({3}/{4})", settings.ChanceBase + settings.ChancePlus * (int)Mathf.Floor(casterLevel / settings.ChancePerLevel), settings.ChanceBase, settings.ChancePlus, casterLevel, settings.ChancePerLevel, Key);
            return settings.ChanceBase + settings.ChancePlus * (int)Mathf.Floor(casterLevel / settings.ChancePerLevel);
        }

        protected bool RollChance()
        {
            if (!Properties.SupportChance)
                return false;

            int roll = UnityEngine.Random.Range(1, 100);
            bool outcome = (roll <= ChanceValue());

            //Debug.LogFormat("Effect '{0}' has a {1}% chance of succeeding and rolled {2} for a {3}", Key, ChanceValue(), roll, (outcome) ? "success" : "fail");

            return outcome;
        }

        #endregion

        #region Private Methods

        string GetDisplayName()
        {
            // Get display name or manufacture a default from group names
            if (!string.IsNullOrEmpty(Properties.DisplayName))
            {
                return Properties.DisplayName;
            }
            else
            {
                if (!string.IsNullOrEmpty(Properties.GroupName) && !string.IsNullOrEmpty(Properties.SubGroupName))
                    return properties.DisplayName = string.Format("{0} {1}", Properties.GroupName, Properties.SubGroupName);
                else if (!string.IsNullOrEmpty(Properties.GroupName) && string.IsNullOrEmpty(Properties.SubGroupName))
                    return properties.DisplayName = Properties.GroupName;
                else
                    return properties.DisplayName = TextManager.Instance.GetText("ClassicEffect", "noName");
            }
        }

        void SetDuration()
        {
            int casterLevel = (caster) ? caster.Entity.Level : 1;
            if (Properties.SupportDuration)
                roundsRemaining = settings.DurationBase + settings.DurationPlus * (int)Mathf.Floor(casterLevel / settings.DurationPerLevel);
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
