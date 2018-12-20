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
using FullSerializer;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Fortify Attribute effect base.
    /// Provides functionality common to all Fortify Attribute effects which vary only by properties and stat.
    /// </summary>
    public abstract class FortifyEffect : IncumbentEffect
    {
        protected DFCareer.Stats fortifyStat = DFCareer.Stats.None;

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is FortifyEffect && (other as FortifyEffect).fortifyStat == fortifyStat) ? true : false;
        }

        protected override void BecomeIncumbent()
        {
            // Incumbent changes fortify magnitude
            ChangeStatMod(fortifyStat, GetMagnitude(caster));
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
            public DFCareer.Stats fortifyStat;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.fortifyStat = fortifyStat;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            fortifyStat = data.fortifyStat;
        }

        #endregion
    }
}
