// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Unsets 1 or more tasks so they can be triggered again
    /// </summary>
    public class ClearTask : ActionTemplate
    {
        public Symbol[] taskSymbols;

        public override string Pattern
        {
            get { return @"clear [a-zA-Z0-9_.]+"; }
        }

        public ClearTask(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Trim source end or trailing white space will be split to an empty symbol at end of array
            source = source.TrimEnd();

            ClearTask action = new ClearTask(parentQuest);

            try
            {
                string[] tasks = source.Split();
                action.taskSymbols = new Symbol[tasks.Length - 1];
                for (int i = 0; i < action.taskSymbols.Length; i++)
                {
                    action.taskSymbols[i] = new Symbol(tasks[i+1]);
                }
            }
            catch (System.Exception ex)
            {
                DaggerfallUnity.LogMessage("ClearTask.CreateNew() failed with exception: " + ex.Message, true);
                action = null;
            }

            return action;
        }

        public override void Update(Task caller)
        {
            // Set complete right away as we could be clearing our own task
            // If we set complete at end it just overwrites the rearm
            SetComplete();

            foreach (Symbol taskSymbol in taskSymbols)
            {
                Task task = ParentQuest.GetTask(taskSymbol);
                if (task != null)
                    task.Clear();
            }
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol[] taskSymbols;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.taskSymbols = taskSymbols;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            taskSymbols = data.taskSymbols;
        }

        #endregion
    }
}