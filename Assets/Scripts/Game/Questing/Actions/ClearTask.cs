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
    /// Unsets 1 or more tasks so they can be triggered again
    /// </summary>
    public class ClearTask : ActionTemplate
    {
        public string[] tasknames;

        public override string Pattern
        {
            get { return @"clear (\w+)"; }
        }

        public ClearTask(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction Create(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            ClearTask action = new ClearTask(parentQuest);

            try
            {
                string[] tasks = source.Split();
                action.tasknames = new String[tasks.Length - 1];
                for (int i = 0; i < action.tasknames.Length; i++)
                {
                    action.tasknames[i] = tasks[i+1];
                }
            }
            catch (System.Exception ex)
            {
                DaggerfallUnity.LogMessage("ClearTask.Create() failed with exception: " + ex.Message, true);
                action = null;
            }

            return action;
        }

        public override object GetSaveData()
        {
            return tasknames;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;
            tasknames = (String[])dataIn;
        }

        public override void Update(Task caller)
        {
            foreach (var taskname in this.tasknames)
            {
                var task = ParentQuest.GetTask(taskname);
                if(task != null)
                    task.Unset();
            }

            SetComplete();
        }
    }
}