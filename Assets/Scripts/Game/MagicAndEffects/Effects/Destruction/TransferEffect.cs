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
    /// This is a two-part effect that will drain target attribute by an amount and fortify caster by same amount.
    /// Note: This has been reworked from the rather useless version of this effect in classic, which does not actually
    /// transfer anything. It only seems to drain the target and there's no real sense of anything happening at all.
    /// In this version of effect, both Duration and Magnitude are supported.
    /// The effect will not stack magnitude, only duration will be added to incumbent.
    /// </summary>
    public abstract class TransferEffect : IncumbentEffect
    {
        const string textDatabase = "ClassicEffects";

        protected DFCareer.Stats transferStat = DFCareer.Stats.None;

        bool isCasterCopy = false;

        public DFCareer.Stats TransferStat
        {
            get { return transferStat; }
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is TransferEffect && (other as TransferEffect).transferStat == transferStat) ? true : false;
        }

        protected override void BecomeIncumbent()
        {
            // Roll for random magnitude
            int magnitude = GetMagnitude(caster);

            // TODO: Assign debuff to target

            // TODO: Reflect caster-version of this effect back to caster
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public DFCareer.Stats transferStat;
            public bool isCasterCopy;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.transferStat = transferStat;
            data.isCasterCopy = isCasterCopy;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            SaveData_v1 data = (SaveData_v1)dataIn;
            if (dataIn == null)
                return;

            transferStat = data.transferStat;
            isCasterCopy = data.isCasterCopy;
        }

        #endregion
    }
}