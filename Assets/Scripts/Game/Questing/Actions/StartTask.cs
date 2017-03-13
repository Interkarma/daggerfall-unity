// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Starts a task by setting it active. Added alternate form "setvar taskname".
    /// </summary>
    public class StartTask : ActionTemplate
    {
        string taskName;

        public override string Pattern
        {
            get { return @"start task (?<taskName>[a-zA-Z0-9_.]+)|setvar (?<taskName>[a-zA-Z0-9_.]+)"; }
        }

        public StartTask(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction Create(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new start task
            StartTask startTask = new StartTask(parentQuest);
            startTask.taskName = match.Groups["taskName"].Value;

            return startTask;
        }

        public override void Update(Task caller)
        {
            ParentQuest.SetTask(taskName);
            SetComplete();
        }
    }
}