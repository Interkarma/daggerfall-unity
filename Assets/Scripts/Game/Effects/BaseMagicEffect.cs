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
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.Effects
{
    /// <summary>
    /// Interface to a magic effect.
    /// </summary>
    public interface IMagicEffect
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
    }

    /// <summary>
    /// Base implementation of a magic effect.
    /// Magic effects are like "actions" for spells, potions, items, advantages, diseases, etc.
    /// They generally perform work against one or more entities (e.g. damage or restore health).
    /// Some effects perform highly custom operations unique to player (e.g. anchor/teleport UI).
    /// Magic effects are scripted in C# so they have full access to engine and UI as required.
    /// Classic magic effects are included in build for cross-platform compatibility.
    /// Custom effects can be added later using mod system (todo:).
    /// </summary>
    public abstract class BaseMagicEffect : IMagicEffect
    {
        public abstract string GroupName { get; }
        public abstract string SubGroupName { get; }
        public abstract string GroupKey { get; }
        public virtual int ClassicKey { get { return 0; } }
        public abstract int TextID { get; }

        public virtual TextFile.Token[] CustomText
        {
            get { return null; }
        }

        public static int MakeClassicKey(byte groupIndex, byte subgroupIndex)
        {
            return groupIndex << 8 + subgroupIndex;
        }

        public static void ReverseClasicKey(int key, out byte groupIndex, out byte subgroupIndex)
        {
            groupIndex = (byte)(key >> 8);
            subgroupIndex = (byte)(key & 0xff);
        }
    }
}