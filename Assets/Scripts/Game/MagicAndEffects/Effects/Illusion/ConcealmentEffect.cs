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

using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Magical concealment effect base.
    /// Provides functionality common to all magical concealment effects (Chameleon/Invisibility/Shadow).
    /// </summary>
    public abstract class ConcealmentEffect : IncumbentEffect
    {
        protected MagicalConcealmentFlags concealmentFlag = MagicalConcealmentFlags.None;
        protected string startConcealmentMessageKey = string.Empty;
        bool awakeAlert = true;

        public override void ConstantEffect()
        {
            base.ConstantEffect();
            StartConcealment();
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            StartConcealment();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            StartConcealment();
        }

        public override void End()
        {
            base.End();
            StopConcealment();
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        protected virtual void StartConcealment()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            entityBehaviour.Entity.MagicalConcealmentFlags |= concealmentFlag;

            if (!string.IsNullOrEmpty(startConcealmentMessageKey))
            {
                // Output start of concealment message if the host manager is player (e.g. "You are invisible.")
                if (IsIncumbent && awakeAlert && entityBehaviour == GameManager.Instance.PlayerEntityBehaviour)
                {
                    DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, startConcealmentMessageKey), 1.5f);
                    awakeAlert = false;
                }
            }
        }

        protected virtual void StopConcealment()
        {
            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            entityBehaviour.Entity.MagicalConcealmentFlags &= ~concealmentFlag;
        }
    }
}
