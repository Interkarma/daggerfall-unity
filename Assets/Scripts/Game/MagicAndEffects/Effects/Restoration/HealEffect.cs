// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Heal drain effect base.
    /// Looks for an incumbent DrainEffect to heal.
    /// NOTE: Does not currently heal attribute loss from disease. Need to confirm if this is allowed in classic.
    /// </summary>
    public abstract class HealEffect : BaseEntityEffect
    {
        protected DFCareer.Stats healStat = DFCareer.Stats.None;

        public override void MagicRound()
        {
            base.MagicRound();

            int magnitude = GetMagnitude(caster);
            manager.HealAttribute(healStat, magnitude);
        }
    }
}
