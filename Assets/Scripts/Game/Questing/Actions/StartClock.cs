// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// This action (re)starts the specified timer.
    /// </summary>
    public class StartClock : ActionTemplate
    {
        public string clockName;

        public override string Pattern
        {
            get { return @"start timer (?<clockName>[a-zA-Z0-9_.]+)"; }
        }

        public StartClock(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction Create(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            StartClock action = new StartClock(parentQuest);
            action.clockName = match.Groups["clockName"].Value.Replace("_", "");

            return action;
        }

        public override object GetSaveData()
        {
            return clockName;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;
            else
                this.clockName = (string)dataIn;
        }

        public override void Update(Task caller)
        {
            var clock = ParentQuest.GetClock(clockName);

            if (clock != null)
            {
                clock.StartClock();
                SetComplete();
            }
            else
            {
                Debug.LogWarning(string.Format("StartClock failed to locate clock: {0} in task {1} for quest: {2}", clockName, caller.Symbol.Name, ParentQuest.UID));
            }
        }
    }
}