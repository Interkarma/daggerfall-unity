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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FullSerializer;

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
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action and get list of symbols
            source = source.Trim();
            List<Symbol> symbols = new List<Symbol>();
            PickOneOf action = new PickOneOf(parentQuest);
            try
            {
                string[] splits = source.Split();
                if (splits == null || splits.Length < 4)
                    return null;

                for (int i = 3; i < splits.Length; i++)
                {
                    if (string.IsNullOrEmpty(splits[i]))
                        continue;

                    symbols.Add(new Symbol(splits[i]));
                }

            }
            catch (System.Exception ex)
            {
                DaggerfallUnity.LogMessage("PickOneOf.Create() failed with exception: " + ex.Message, true);
                return null;
            }

            // Handle no valid symbols
            if (symbols.Count == 0)
                return null;

            // Assign symbols
            action.taskSymbols = symbols.ToArray();

            return action;

        }

        public override void Update(Task caller)
        {
            Symbol selected = new Symbol();
            bool success = false;
            if(ParentQuest != null)
            {
                UnityEngine.Random.InitState(GameManager.Instance.QuestMachine.InternalSeed);
                selected = taskSymbols[UnityEngine.Random.Range(0, taskSymbols.Length)];
                Task task = ParentQuest.GetTask(selected);

                if (task != null)
                {
                    success = true;
                    task.Start();
                }
            }

            if(!success)
            {
                Debug.LogError(string.Format("PickOneOf failed to activate task.  Quest: {0} Task: {1} Selected: {2}", ParentQuest.UID, caller.Symbol.Name, selected.Name));
            }

            SetComplete();
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