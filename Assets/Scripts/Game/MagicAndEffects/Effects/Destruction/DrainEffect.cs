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
using System;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Drain effect base.
    /// Provides functionality common to all Drain effects which vary only by properties and stat.
    /// This effect uses an incumbent pattern where future applications of same effect type
    /// will only add to total magnitude of first effect of this type.
    /// Incumbent drain effect persists indefinitely until player heals stat enough for magnitude to reach 0.
    /// </summary>
    public abstract class DrainEffect : IncumbentEffect
    {
        const string textDatabase = "ClassicEffects";

        protected int magnitude = 0;
        protected DFCareer.Stats drainStat = DFCareer.Stats.None;

        int roundsRemaining = 1;
        bool isIncumbent = false;

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

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is DrainEffect && (other as DrainEffect).drainStat == drainStat) ? true : false;
        }

        protected override void BecomeIncumbent()
        {
            IncreaseMagnitude(GetMagnitude(caster));
            ShowPlayerDrained();
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            (incumbent as DrainEffect).IncreaseMagnitude(GetMagnitude(caster));
            ShowPlayerDrained();
        }

        void ShowPlayerDrained()
        {
            // Output "you feel drained." if the host manager is player
            if (manager.EntityBehaviour == GameManager.Instance.PlayerEntityBehaviour)
                DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "youFeelDrained"));
        }

        void IncreaseMagnitude(int amount)
        {
            DaggerfallEntityBehaviour host = GetPeeredEntityBehaviour(manager);

            // Do not allow magnitude to reduce stat below 1 relative to permanent value
            // Stats are clamped 1-100 and this prevents drain magnitude from going into invisible "healing debt"
            int permanentValue = host.Entity.Stats.GetPermanentStatValue(drainStat);
            if (permanentValue - (magnitude + amount) < 1)
                magnitude = permanentValue - 1;
            else
                magnitude += amount;

            SetStatMod(drainStat, -magnitude);
        }
    }
}