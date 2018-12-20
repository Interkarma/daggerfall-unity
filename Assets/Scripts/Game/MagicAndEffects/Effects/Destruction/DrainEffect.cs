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

using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using FullSerializer;

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
        protected int magnitude = 0;
        protected DFCareer.Stats drainStat = DFCareer.Stats.None;
        protected int lastMagnitudeIncreaseAmount = 0;
        int forcedRoundsRemaining = 1;

        public int Magnitude
        {
            get { return magnitude; }
        }

        public DFCareer.Stats DrainStat
        {
            get { return drainStat; }
        }

        // Drain effects are permanent until healed so we manage our own lifecycle
        protected override int RemoveRound()
        {
            return forcedRoundsRemaining;
        }

        // Always present at least one round remaining so effect system does not remove
        public override int RoundsRemaining
        {
            get { return forcedRoundsRemaining; }
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            PlayerAggro();
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is DrainEffect && (other as DrainEffect).drainStat == drainStat);
        }

        protected override void BecomeIncumbent()
        {
            lastMagnitudeIncreaseAmount = GetMagnitude(caster);
            IncreaseMagnitude(lastMagnitudeIncreaseAmount);
            ShowPlayerDrained();
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            if (forcedRoundsRemaining == 0)
                return;

            lastMagnitudeIncreaseAmount = GetMagnitude(caster);
            (incumbent as DrainEffect).IncreaseMagnitude(lastMagnitudeIncreaseAmount);
            ShowPlayerDrained();
        }

        public override void HealAttributeDamage(DFCareer.Stats stat, int amount)
        {
            // Can only heal incumbent matching drain
            if (!IsIncumbent || stat != drainStat)
                return;

            // Heal attribute
            base.HealAttributeDamage(stat, amount);

            // Reduce magnitude and cancel effect once reduced to 0
            if (DecreaseMagnitude(amount) == 0)
                forcedRoundsRemaining = 0;
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

        int DecreaseMagnitude(int amount)
        {
            magnitude -= amount;
            if (magnitude < 0)
                magnitude = 0;

            return magnitude;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int magnitude;
            public DFCareer.Stats drainStat;
            public int forcedRoundsRemaining;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.magnitude = magnitude;
            data.drainStat = drainStat;
            data.forcedRoundsRemaining = forcedRoundsRemaining;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            magnitude = data.magnitude;
            drainStat = data.drainStat;
            forcedRoundsRemaining = data.forcedRoundsRemaining;
        }

        #endregion
    }
}
