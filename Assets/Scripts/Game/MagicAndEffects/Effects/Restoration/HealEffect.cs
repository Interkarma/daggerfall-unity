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
    /// Heal drain effect base.
    /// Looks for an incumbent DrainEffect to heal.
    /// </summary>
    public abstract class HealEffect : BaseEntityEffect
    {
        protected DFCareer.Stats healStat = DFCareer.Stats.None;

        public override void MagicRound()
        {
            base.MagicRound();

            DrainEffect incumbentDrain = FindDrainStatIncumbent();
            if (incumbentDrain != null)
            {
                int magnitude = GetMagnitude(caster);
                incumbentDrain.Heal(magnitude);
                Debug.LogFormat("Healed {0} Drain {1} by {2} points", GetPeeredEntityBehaviour(manager).name, incumbentDrain.DrainStat.ToString(), magnitude);
            }
            else
            {
                Debug.LogFormat("Could not find incumbent Drain {0} on target", healStat.ToString());
            }
        }

        DrainEffect FindDrainStatIncumbent()
        {
            // Search for any matching incumbents on this host
            EntityEffectManager.InstancedBundle[] bundles = manager.EffectBundles;
            foreach (EntityEffectManager.InstancedBundle bundle in bundles)
            {
                foreach (IEntityEffect effect in bundle.liveEffects)
                {
                    if (effect is DrainEffect)
                    {
                        // Heal stat must match drain stat
                        DrainEffect drainEffect = effect as DrainEffect;
                        if (drainEffect.IsIncumbent && drainEffect.DrainStat == healStat)
                            return drainEffect;
                    }
                }
            }

            return null;
        }
    }
}