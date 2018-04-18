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

using DaggerfallWorkshop.Game.Entity;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// Interface to an entity effect.
    /// </summary>
    public interface IEntityEffect : IMacroContextProvider
    {
        /// <summary>
        /// Unique string key for this effect, usually equal to some combination of group+subgroup (e.g. "ContinuousDamage-Health").
        /// Effects can use any text they wish for key provided it is unique.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Main group for this effect class in spellmaker (e.g. "Damage") - can be shared with other effects.
        /// </summary>
        string GroupName { get; }

        /// <summary>
        /// Sub-group name for effect class in spellmaker (e.g. "Health") - must be unique within group.
        /// </summary>
        string SubGroupName { get; }

        /// <summary>
        /// Display name of effect. Usually GroupName + " " + SubGroupName (e.g. "Continuous Damage Health")
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Group index for legacy classic effect compatibility. Do not set this for non-classic effects.
        /// </summary>
        int ClassicGroup { get; }

        /// <summary>
        /// Subgroup index for legacy classic effect compatibility. Do not set this for non-classic effects.
        /// </summary>
        int ClassicSubGroup { get; }

        /// <summary>
        /// Text tokens for spellmaker UI if required.
        /// </summary>
        TextFile.Token[] SpellMakerDescription { get; }

        /// <summary>
        /// Text tokens for spellbook UI if required.
        /// </summary>
        TextFile.Token[] SpellBookDescription { get; }

        /// <summary>
        /// Effect supports Duration setting.
        /// </summary>
        bool SupportDuration { get; }

        /// <summary>
        /// Effect supports Chance setting.
        /// </summary>
        bool SupportChance { get; }
        
        /// <summary>
        /// Effect supports Magnitude setting.
        /// </summary>
        bool SupportMagnitude { get; }

        /// <summary>
        /// Targets supported by this effect.
        /// </summary>
        TargetTypes AllowedTargets { get; }

        /// <summary>
        /// Elements supported by this effect.
        /// </summary>
        ElementTypes AllowedElements { get; }

        /// <summary>
        /// Crafting stations supported by this effect.
        /// </summary>
        MagicCraftingStations AllowedCraftingStations { get; }

        /// <summary>
        /// Gets or sets current effect settings.
        /// </summary>
        EffectSettings Settings { get; set; }

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
        /// Called by an EntityEffectManager when parent bundle is attached to an entity.
        /// Use this for setup or immediate work performed only once.
        /// </summary>
        void Start();

        /// <summary>
        /// Called when bundle lifetime is at an end.
        /// Use this for any wrap-up work.
        /// </summary>
        void End();

        /// <summary>
        /// Use this for any work performed at the start of a new "magic round".
        /// </summary>
        void MagicRound();
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

        protected int[] statMods = new int[DaggerfallStats.Count];
        protected int[] skillMods = new int[DaggerfallSkills.Count];
        protected EffectSettings settings = new EffectSettings();

        #endregion

        #region Properties
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseEntityEffect()
        {
            settings = GetDefaultSettings();
        }

        #endregion

        #region IEntityEffect Properties

        public abstract string Key { get; }
        public abstract string GroupName { get; }
        public abstract string SubGroupName { get; }
        public virtual int ClassicGroup { get { return -1; } }
        public virtual int ClassicSubGroup { get { return -1; } }
        public virtual bool SupportDuration { get { return true; } }
        public virtual bool SupportChance { get { return true; } }
        public virtual bool SupportMagnitude { get { return true; } }
        public virtual TargetTypes AllowedTargets { get { return EntityEffectBroker.TargetFlags_All; } }
        public virtual ElementTypes AllowedElements { get { return EntityEffectBroker.ElementFlags_MagicOnly; } }
        public virtual MagicCraftingStations AllowedCraftingStations { get { return MagicCraftingStations.SpellMaker; } }

        public virtual string DisplayName
        {
            get { return string.Format("{0} {1}", GroupName, SubGroupName); }
        }

        public virtual EffectSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public virtual TextFile.Token[] SpellMakerDescription
        {
            get { return null; }
        }

        public virtual TextFile.Token[] SpellBookDescription
        {
            get { return null; }
        }

        public int[] StatMods
        {
            get { return statMods; }
        }

        public int[] SkillMods
        {
            get { return skillMods; }
        }

        #endregion

        #region IEntityEffect Virtual Methods

        public virtual void Start()
        {
        }

        public virtual void End()
        {
        }

        public virtual void MagicRound()
        {
        }

        #endregion

        #region Private Methods

        // Applies default settings when not specified
        EffectSettings GetDefaultSettings()
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

        #endregion

        #region Static Methods

        public static int MakeClassicKey(byte groupIndex, byte subgroupIndex)
        {
            return groupIndex << 8 + subgroupIndex;
        }

        public static void ReverseClasicKey(int key, out byte groupIndex, out byte subgroupIndex)
        {
            groupIndex = (byte)(key >> 8);
            subgroupIndex = (byte)(key & 0xff);
        }

        #endregion
    }
}