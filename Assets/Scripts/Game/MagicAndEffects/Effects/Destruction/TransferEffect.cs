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
    /// Transfer effect base.
    /// This is a two-part effect that will drain target attribute by an amount and fortify same caster attribute by same amount.
    /// Note: This has been reworked from the rather useless version of this effect in classic, which does not actually transfer anything.
    /// Classic only seems to drain the target and there's no real sense of anything happening at all.
    /// In this version of effect, caster establishes a vampiric drain on target until the target expires or passes out of scope.
    /// At this time intended to be cast by player only. Current design would render effect permanent if cast onto player.
    /// </summary>
    public abstract class TransferEffect : IncumbentEffect
    {
        const string textDatabase = "ClassicEffects";

        protected DFCareer.Stats transferStat = DFCareer.Stats.None;
        int[] casterStatMods = new int[DaggerfallStats.Count];
        int forcedRoundsRemaining = 1;

        public DFCareer.Stats TransferStat
        {
            get { return transferStat; }
        }

        // Always present at least one round remaining so effect system does not remove
        public override int RoundsRemaining
        {
            get { return forcedRoundsRemaining; }
        }

        // Transfer effects is permanent until dead or healed so we manage our own lifecycle
        protected override int RemoveRound()
        {
            return forcedRoundsRemaining;
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is TransferEffect && (other as TransferEffect).transferStat == transferStat) ? true : false;
        }

        protected override void BecomeIncumbent()
        {
            // At this time intended to be cast by player only
            if (caster != GameManager.Instance.PlayerEntityBehaviour)
            {
                forcedRoundsRemaining = 0;
                ResignAsIncumbent();
                return;
            }

            // Local entity is drained by magnitude, caster is fortified by magnitude
            // Link will end once target expires
            int magnitude = GetMagnitude(caster);
            SetStatMod(transferStat, -magnitude);
            casterStatMods[(int)transferStat] = magnitude;
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Send stat mods to player effect manager
            if (caster)
            {
                EntityEffectManager casterManager = caster.GetComponent<EntityEffectManager>();
                casterManager.MergeDirectStatMods(casterStatMods);
            }
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public DFCareer.Stats transferStat;
            public int[] casterStatMods;
            public int forcedRoundsRemaining;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.transferStat = transferStat;
            data.casterStatMods = casterStatMods;
            data.forcedRoundsRemaining = forcedRoundsRemaining;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            SaveData_v1 data = (SaveData_v1)dataIn;
            if (dataIn == null)
                return;

            transferStat = data.transferStat;
            casterStatMods = data.casterStatMods;
            forcedRoundsRemaining = data.forcedRoundsRemaining;
        }

        #endregion
    }
}