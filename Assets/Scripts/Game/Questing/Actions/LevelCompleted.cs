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

using System.Text.RegularExpressions;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Triggers when player has reached specified level or higher.
    /// </summary>
    public class LevelCompleted : ActionTemplate
    {
        int minLevelValue;

        public override string Pattern
        {
            get { return @"level (?<minLevelValue>\d+) completed"; }
        }

        public LevelCompleted(Quest parentQuest)
            : base(parentQuest)
        {
            IsTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            LevelCompleted action = new LevelCompleted(parentQuest);
            action.minLevelValue = Parser.ParseInt(match.Groups["minLevelValue"].Value);

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            return GameManager.Instance.PlayerEntity.Level >= minLevelValue;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int minLevelValue;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.minLevelValue = minLevelValue;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            minLevelValue = data.minLevelValue;
        }

        #endregion
    }
}