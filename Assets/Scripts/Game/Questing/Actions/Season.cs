// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: kaboissonneault (kaboissonneault@gmail.com)
// Contributors:    
// 
// Notes:
//

using System;
using System.Text.RegularExpressions;
using FullSerializer;

using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// DFU extension action. Triggers when the calendar is at the right season
    /// </summary>
    public class Season : ActionTemplate
    {
        DaggerfallDateTime.Seasons season;

        public override string Pattern
        {
            get { return @"season (?<season>fall|summer|spring|winter)"; }
        }

        public Season(Quest parentQuest)
                    : base(parentQuest)
        {
            IsTriggerCondition = true;
            IsAlwaysOnTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            Season action = new Season(parentQuest);
            if (!Enum.TryParse(match.Groups["season"].Value, true /*ignore case*/, out action.season))
            {
                throw new Exception("Season: Syntax is 'season fall|summer|spring|winter'");
            }

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            return DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == season;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public DaggerfallDateTime.Seasons season;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.season = season;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            season = data.season;
        }
        #endregion
    }
}
