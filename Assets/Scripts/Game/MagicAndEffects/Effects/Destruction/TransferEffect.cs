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

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Base class for Transfer stat effect classes.
    /// Essentially a DrainEffect on target with a HealEffect step for caster.
    /// </summary>
    public abstract class TransferEffect : DrainEffect
    {
        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is TransferEffect && (other as TransferEffect).drainStat == drainStat);
        }

        protected override void BecomeIncumbent()
        {
            base.BecomeIncumbent();
            HealCaster();
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            base.AddState(incumbent);
            HealCaster();
        }

        void HealCaster()
        {
            if (caster)
            {
                caster.GetComponent<EntityEffectManager>().HealAttribute(drainStat, lastMagnitudeIncreaseAmount);
            }
        }
    }
}
