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

using System;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using FullSerializer;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Disease effect base.
    /// </summary>
    public abstract class Disease : IncumbentEffect
    {
        const string textDatabase = "Diseases";

        int forcedRoundsRemaining = 1;

        // Disease effects are permanent until cured so we manage our own lifecycle
        protected override int RemoveRound()
        {
            return forcedRoundsRemaining;
        }

        // Always present at least one round remaining so effect system does not remove
        public override int RoundsRemaining
        {
            get { return forcedRoundsRemaining; }
        }

        protected override void BecomeIncumbent()
        {
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            if (forcedRoundsRemaining == 0)
                return;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int forcedRoundsRemaining;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.forcedRoundsRemaining = forcedRoundsRemaining;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            SaveData_v1 data = (SaveData_v1)dataIn;
            if (dataIn == null)
                return;

            forcedRoundsRemaining = data.forcedRoundsRemaining;
        }

        #endregion
    }
}