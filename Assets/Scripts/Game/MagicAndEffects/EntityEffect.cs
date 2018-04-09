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
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// Interface to an entity effect.
    /// </summary>
    public interface IEntityEffect
    {
        /// <summary>
        /// Main group for this effect class in spellmaker (e.g. "Damage") - can be shared with other effects.
        /// </summary>
        string GroupName { get; }

        /// <summary>
        /// Sub-group name for effect class in spellmaker (e.g. "Health") - must be unique within group.
        /// </summary>
        string SubGroupName { get; }

        /// <summary>
        /// Unique string key for this effect, usually equal to group+subgroup (e.g. "DamageHealth").
        /// Effects can use any text as key provided it is unique.
        /// </summary>
        string GroupKey { get; }

        /// <summary>
        /// Key value for legacy classic effect compatibility. Do not set this for non-classic effects.
        /// </summary>
        int ClassicKey { get; }

        /// <summary>
        /// TEXT.RSC ID description for this effect (effect descriptions start at ID 1500).
        /// </summary>
        int TextID { get; }

        /// <summary>
        /// Custom description tokens for spell effect.
        /// Overrides TextID when non-null.
        /// Effect must provide either a valid custom description or ID.
        /// </summary>
        TextFile.Token[] CustomText { get; }

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
    public abstract class BaseEntityEffect : IEntityEffect
    {
        #region Fields

        protected int[] statMods = new int[DaggerfallStats.Count];
        protected int[] skillMods = new int[DaggerfallSkills.Count];

        #endregion

        #region Properties
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseEntityEffect()
        {
        }

        #endregion

        #region IEntityEffect Properties

        public abstract string GroupName { get; }
        public abstract string SubGroupName { get; }
        public abstract string GroupKey { get; }
        public virtual int ClassicKey { get { return 0; } }
        public virtual int TextID { get { return 0; } }

        public virtual TextFile.Token[] CustomText
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

        #region IEntityEffect Public Methods
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