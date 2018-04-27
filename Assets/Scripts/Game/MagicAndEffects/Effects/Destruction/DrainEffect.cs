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
    /// Provides functionality common to all Drain effects which vary only by properties and stat.
    /// This effect uses an incumbent pattern where future applications of same effect type
    /// will only add to total magnitude of first effect of this type.
    /// Incumbent drain effect persists indefinitely until player heals stat enough for magnitude to reach 0.
    /// </summary>
    public abstract class DrainEffect : BaseEntityEffect
    {
        const string textDatabase = "ClassicEffects";

        protected int magnitude = 0;
        protected DFCareer.Stats drainStat = DFCareer.Stats.None;

        int roundsRemaining = 1;
        bool isIncumbent = false;

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);

            AttachHost();
        }

        // Drain effects are permanent until healed so we manage our own lifecycle
        protected override int RemoveRound()
        {
            return roundsRemaining;
        }

        // Always present at least one round remaining so effect system does not remove
        public override int RoundsRemaining
        {
            get { return roundsRemaining; }
        }

        #region Private Methods

        void AttachHost()
        {
            // Get effect bundles from host
            EntityEffectManager.InstancedBundle[] bundles = manager.EffectBundles;

            // The first instance of drain effect for this stat becomes incumbent
            // Otherwise just add to existing effect and expire this one
            int amount = GetMagnitude(caster);
            DrainEffect incumbent = FindIncumbent(bundles);
            if (incumbent == null)
            {
                isIncumbent = true;
                IncreaseMagnitude(amount);
                Debug.LogFormat("Creating incumbent Drain {0} effect with magnitude {1}", drainStat.ToString(), magnitude);
            }
            else
            {
                incumbent.IncreaseMagnitude(amount);
                roundsRemaining = 0;
                Debug.LogFormat("Increasing incumbent Drain {0} effect by amount {1}, total magnitude is now {2}", drainStat.ToString(), amount, incumbent.magnitude);
            }

            // Output "you feel drained."
            DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "youFeelDrained"));
        }

        DrainEffect FindIncumbent(EntityEffectManager.InstancedBundle[] bundles)
        {
            foreach(EntityEffectManager.InstancedBundle bundle in bundles)
            {
                foreach (IEntityEffect effect in bundle.effects)
                {
                    if (effect is DrainEffect)
                    {
                        // Must be same stat type as incoming and flagged as incumbent
                        DrainEffect drainEffect = (DrainEffect)effect;
                        if (drainEffect.drainStat == drainStat && drainEffect.isIncumbent)
                            return drainEffect;
                    }
                }
            }

            return null;
        }

        void IncreaseMagnitude(int amount)
        {
            DaggerfallEntityBehaviour host = GetPeeredEntityBehaviour(manager);

            // Do not allow magnitude to reduce stat below 1
            // Stats class will not allow values below 1 and this prevents drain from going into invisible "healing debt"
            int current = host.Entity.Stats.GetPermanentStatValue(drainStat);
            if (current - (magnitude + amount) < 1)
                magnitude = current - 1;
            else
                magnitude += amount;

            SetStatMod(drainStat, -magnitude);
        }

        #endregion
    }
}