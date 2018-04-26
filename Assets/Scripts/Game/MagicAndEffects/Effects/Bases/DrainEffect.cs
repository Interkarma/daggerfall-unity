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

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Drain effect base.
    /// Provides functionality common to all Drain effects.
    /// </summary>
    public abstract class DrainEffect : BaseEntityEffect
    {
        protected int amount = 0;
        protected DFCareer.Stats stat = DFCareer.Stats.None;

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            AttachHost();
        }

        #region Private Methods

        void AttachHost()
        {
        }

        #endregion
    }
}