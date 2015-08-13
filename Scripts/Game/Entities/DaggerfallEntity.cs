// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Effects;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Composition of a base game entity.
    /// </summary>
    public class DaggerfallEntity : MonoBehaviour
    {
        #region Fields

        public int Level;

        public DaggerfallStats Stats;

        public int Health;
        public int Fatigue;

        public EffectFlags ResistanceFlags;
        public EffectFlags ImmunityFlags;
        public EffectFlags LowToleranceFlags;
        public EffectFlags CriticalWeaknessFlags;
        public SpecialAbilityFlags SpecialAbilities;

        public int SpellPointsInDark;
        public int SpellPointsInLight;
        public float SpellPointMultiplier;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DaggerfallEntity()
        {
            Level = 1;
            Stats.SetDefaults();

            ResistanceFlags = EffectFlags.None;
            ImmunityFlags = EffectFlags.None;
            LowToleranceFlags = EffectFlags.None;
            CriticalWeaknessFlags = EffectFlags.None;
            SpecialAbilities = SpecialAbilityFlags.None;

            SpellPointsInDark = 0;
            SpellPointsInLight = 0;
            SpellPointMultiplier = 1.0f;
        }

        /// <summary>
        /// Construct from stats.
        /// </summary>
        /// <param name="stats"></param>
        public DaggerfallEntity(DaggerfallStats stats)
            : base()
        {
            Stats = stats;
        }

        #endregion

        #region Public Methods
        #endregion

        #region Flag Tests

        public bool HasResistance(EffectFlags flags)
        {
            return ((ResistanceFlags & flags) == flags) ? true : false;
        }

        public bool HasImmunity(EffectFlags flags)
        {
            return ((ImmunityFlags & flags) == flags) ? true : false;
        }

        public bool HasLowTolerance(EffectFlags flags)
        {
            return ((LowToleranceFlags & flags) == flags) ? true : false;
        }

        public bool HasCriticalWeakness(EffectFlags flags)
        {
            return ((CriticalWeaknessFlags & flags) == flags) ? true : false;
        }

        public bool HasSpecialAbility(SpecialAbilityFlags flags)
        {
            return ((SpecialAbilities & flags) == flags) ? true : false;
        }

        #endregion
    }
}