// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com)
// Contributors:    Gavin Clayton (interkarma@dfworkshop.net)
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
    /// This action triggers a random task
    /// </summary>
    public class PickOneOf : ActionTemplate
    {
        public Symbol[] taskSymbols;

        public override string Pattern
        {
            get { return @"pick one of [a-zA-Z0-9_.]+"; }
        }

        public PickOneOf(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            base.CreateNew(source, parentQuest);

            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            PickOneOf action = new PickOneOf(parentQuest);
            try
            {
                var splits = source.Split();
                if (splits == null || splits.Length < 4)
                    return null;
                else
                    action.taskSymbols = new Symbol[splits.Length - 3];

                for (int i = 0; i < action.taskSymbols.Length; i++)
                {
                    action.taskSymbols[i] = new Symbol(splits[i + 3]);
                }

            }
            catch (System.Exception ex)
            {
                DaggerfallUnity.LogMessage("PickRandomTask.Create() failed with exception: " + ex.Message, true);
                action = null;
            }

            return action;

        }

        public override object GetSaveData()
        {
            return taskSymbols;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;
            taskSymbols = (Symbol[])dataIn;
        }

        public override void Update(Task caller)
        {
            bool success = false;
            if(ParentQuest != null)
            {
                //UnityEngine.Random.InitState(System.Environment.TickCount);
                Symbol selected = taskSymbols[UnityEngine.Random.Range(0, taskSymbols.Length)];
                Task task = ParentQuest.GetTask(selected);

                if (task != null)
                {
                    success = true;
                    task.Set();
                }
            }

            if(!success)
            {
                Debug.LogError(string.Format("PickOneOf failed to activate task.  Quest: {0} Task: {1}", ParentQuest.UID, caller.Symbol.Name));
            }

            SetComplete();
        }
    }
}