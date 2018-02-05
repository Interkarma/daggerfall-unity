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
    /// Fake spell class used to bootstrap effect system.
    /// This will be replaced/reworked later for actual spell integration.
    /// Initial work to focus mainly on effect script execution.
    /// </summary>
    public abstract class FakeSpell
    {
        /// <summary>
        /// Settings for a single spell effect.
        /// Each spell has one or more effects with unique settings.
        /// For example, the player and a lich might cast a fireball at each other at the same time.
        /// This results in two unique invocations of the same "DamageHealth" effect with different values.
        /// Certain properties are inherited from spell itself (e.g. touch/area or fire/cold).
        /// The effect may not consume all settings depending on scripted behaviour of effect iself.
        /// </summary>
        public struct SpellEffectSettings
        {
            public string GroupKey;

            public int DurationBase;
            public int DurationBonus;
            public int DurationBonusPerLevel;

            public int ChanceBase;
            public int ChanceBonus;
            public int ChanceBonusPerLevel;
            public int ChanceCostResult;

            public int MagnitudeBaseMin;
            public int MagnitudeBaseMax;
            public int MagnitudeBonusMin;
            public int MagnitudeBonusMax;
            public int MagnitudeBonusPerLevel;
        }

        protected string displayName;                           // Display name of spell
        protected TargetTypes selectedTargetType;               // How this spell targets entities in world space
        protected EffectTypes selectedEffectType;               // How effect manifests for cast animations, billboard effects, resist checks, etc.
        protected int spellCost;                                // Cost of this spell to cast - not used for now

        protected List<SpellEffectSettings> effects = new List<SpellEffectSettings>();

        public FakeSpell()
        {
        }
    }
}