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

using UnityEngine;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Some effects in Daggerfall add to the state an existing like-kind effect (the incumbent)
    /// rather than become instantiated as a new effect on the host entity.
    /// One example is a drain effect which only adds to the magnitude of incumbent drain for same stat.
    /// Another example is an effect which tops up the duration of same effect in progress.
    /// This class establishes a base for these incumbent effects to coordinate.
    /// NOTES:
    ///  Unflagged incumbent effects (IsIncumbent == false) do not persist beyond AddState() call.
    ///  They will never receive a single MagicRound() call and are never saved/loaded.
    ///  The flagged incumbent (IsIncumbent == true) receives MagicRound() calls and is saved/load as normal.
    /// </summary>
    public abstract class IncumbentEffect : BaseEntityEffect
    {
        bool isIncumbent = false;

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            AttachHost();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            isIncumbent = effectData.isIncumbent;
        }

        public bool IsIncumbent
        {
            get { return isIncumbent; }
        }

        void AttachHost()
        {
            IncumbentEffect incumbent = FindIncumbent();
            if (incumbent == null)
            {
                // First instance of effect on this host becomes flagged incumbent
                isIncumbent = true;
                BecomeIncumbent();

                //Debug.LogFormat("Creating incumbent effect '{0}' on host '{1}'", DisplayName, manager.name);
            }
            else
            {
                // Subsequent instances add to state of flagged incumbent
                AddState(incumbent);

                //Debug.LogFormat("Adding state to incumbent effect '{0}' on host '{1}'", incumbent.DisplayName, incumbent.manager.name);
            }
        }

        IncumbentEffect FindIncumbent()
        {
            // Search for any incumbents on this host matching group
            LiveEffectBundle[] bundles = manager.EffectBundles;
            foreach (LiveEffectBundle bundle in bundles)
            {
                if (bundle.bundleType == ParentBundle.bundleType)
                {
                    foreach (IEntityEffect effect in bundle.liveEffects)
                    {
                        if (effect is IncumbentEffect)
                        {
                            // Effect must be flagged incumbent and agree with like-kind test
                            IncumbentEffect other = effect as IncumbentEffect;
                            if (other.IsIncumbent && other.IsLikeKind(this))
                                return other;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Resign as incumbent effect.
        /// This allows an incumbent to immediately allow for a new incumbent to take over its post.
        /// Useful for when incumbent does not want to receive any further AddState() calls and cannot wait for magic round tick to expire.
        /// </summary>
        protected void ResignAsIncumbent()
        {
            isIncumbent = false;
        }

        protected virtual void BecomeIncumbent()
        {
        }

        protected abstract bool IsLikeKind(IncumbentEffect other);
        protected abstract void AddState(IncumbentEffect incumbent);
    }
}
