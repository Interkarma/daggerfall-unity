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
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// Fake spell class used to bootstrap effect system.
    /// This will be replaced/reworked later for actual spell integration.
    /// Initial work to focus mainly on effect script execution.
    /// </summary>
    public class FakeSpell
    {
        #region Fields

        protected string displayName;                           // Display name of spell
        protected TargetTypes selectedTargetType;               // How this spell targets entities in world space
        protected SpellTypes selectedSpellType;                 // How this spell manifests for cast animations, billboard effects, resist checks, etc.
        protected int spellCost;                                // Cost of this spell to cast - not used for now

        //protected List<SpellEffectSettings> effects = new List<SpellEffectSettings>();

        protected DaggerfallEntityBehaviour casterEntityBehaviour;

        #endregion

        /// <summary>
        /// Default construtor.
        /// </summary>
        public FakeSpell()
        {
        }
    }
}