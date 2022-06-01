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
using System.Text.RegularExpressions;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Starts and stops a Clock resource timer.
    /// </summary>
    public class StartStopTimer : ActionTemplate
    {
        bool isStartTimer;
        Symbol targetSymbol;

        public override string Pattern
        {
            get { return @"(?<start>start) timer (?<symbol>[a-zA-Z0-9_.-]+)|stop timer (?<symbol>[a-zA-Z0-9_.-]+)"; }
        }

        public StartStopTimer(Quest parentQuest)
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
            StartStopTimer action = new StartStopTimer(parentQuest);
            action.targetSymbol = new Symbol(match.Groups["symbol"].Value);
            if (!string.IsNullOrEmpty(match.Groups["start"].Value))
                action.isStartTimer = true;
            else
                action.isStartTimer = false;

            return action;
        }

        public override void Update(Task caller)
        {
            // Must have a target symbol
            if (targetSymbol == null)
                return;

            // Get target clock
            Clock targetClock = ParentQuest.GetClock(targetSymbol);
            if (targetClock == null)
            {
                Debug.LogFormat("start timer was unable to find clock symbol {0}", targetSymbol.Name);
                return;
            }

            // Start or stop clock
            if (isStartTimer)
                targetClock.StartTimer();
            else
                targetClock.StopTimer();

            // Action complete
            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public bool isStartTimer;
            public Symbol targetSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.isStartTimer = isStartTimer;
            data.targetSymbol = targetSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            isStartTimer = data.isStartTimer;
            targetSymbol = data.targetSymbol;
        }

        #endregion
    }
}