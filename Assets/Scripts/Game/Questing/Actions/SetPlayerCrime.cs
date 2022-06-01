// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    DFIronman, Hazelnut
// 
// Notes:
// 

using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Entity;
using FullSerializer;
using System;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Sets a player's crime.
    /// </summary>
    public class SetPlayerCrime : ActionTemplate
    {
        public PlayerEntity.Crimes playerCrime;
        
        public override string Pattern
        {
            get { return @"setplayercrime (?<crime>[a-zA-Z_]+)"; }
        }

        public SetPlayerCrime(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            SetPlayerCrime action = new SetPlayerCrime(parentQuest);
            string crime = match.Groups["crime"].Value;
            if (!Enum.IsDefined(typeof(PlayerEntity.Crimes), crime))
            {
                SetComplete();
                throw new Exception(string.Format("SetPlayerCrime: Crime {0} is not a known crime from PlayerEntity.Crimes enum.", crime));
            }
            action.playerCrime = (PlayerEntity.Crimes)Enum.Parse(typeof(PlayerEntity.Crimes), crime);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            GameManager.Instance.PlayerEntity.CrimeCommitted = playerCrime;

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public PlayerEntity.Crimes crime;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.crime = playerCrime;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            playerCrime = data.crime;
        }

        #endregion
    }
}