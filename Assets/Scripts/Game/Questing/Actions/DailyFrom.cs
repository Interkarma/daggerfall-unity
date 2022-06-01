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

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Utility;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Raise or lower task state based on time of day.
    /// Time must be in 24-hour time 00:00 to 23:59.
    /// </summary>
    public class DailyFrom : ActionTemplate
    {
        int minDailySeconds;
        int maxDailySeconds;

        public override string Pattern
        {
            get { return @"daily from (?<hours1>\d+):(?<minutes1>\d+) to (?<hours2>\d+):(?<minutes2>\d+)"; }
        }

        public DailyFrom(Quest parentQuest)
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
            DailyFrom action = new DailyFrom(parentQuest);
            int hours1 = Parser.ParseInt(match.Groups["hours1"].Value);
            int minutes1 = Parser.ParseInt(match.Groups["minutes1"].Value);
            int hours2 = Parser.ParseInt(match.Groups["hours2"].Value);
            int minutes2 = Parser.ParseInt(match.Groups["minutes2"].Value);
            action.minDailySeconds = ToDailySeconds(hours1, minutes1);
            action.maxDailySeconds = ToDailySeconds(hours2, minutes2);

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            // Get live world time
            DaggerfallDateTime worldTime = DaggerfallUnity.Instance.WorldTime.Now;
            int currentDailySeconds = ToDailySeconds(worldTime.Hour, worldTime.Minute);

            // Check if inside range
            if (currentDailySeconds >= minDailySeconds && currentDailySeconds <= maxDailySeconds)
                return true;
            else
                return false;
        }

        int ToDailySeconds(int hours, int minutes)
        {
            return hours * 3600 + minutes * 60;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int minDailySeconds;
            public int maxDailySeconds;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.minDailySeconds = minDailySeconds;
            data.maxDailySeconds = maxDailySeconds;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            minDailySeconds = data.minDailySeconds;
            maxDailySeconds = data.maxDailySeconds;
        }

        #endregion
    }
}