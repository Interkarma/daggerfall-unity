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
using System.Collections;
using System.Text.RegularExpressions;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Starts a task by setting it active. Added alternate form "setvar taskname".
    /// </summary>
    public class StartTask : ActionTemplate
    {
        Symbol taskSymbol;

        public override string Pattern
        {
            get { return @"start task (?<taskName>[a-zA-Z0-9_.]+)|setvar (?<taskName>[a-zA-Z0-9_.]+)"; }
        }

        public StartTask(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new start task
            StartTask startTask = new StartTask(parentQuest);
            startTask.taskSymbol = new Symbol(match.Groups["taskName"].Value);

            return startTask;
        }

        public override void Update(Task caller)
        {
            ParentQuest.StartTask(taskSymbol);
            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol taskSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.taskSymbol = taskSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            taskSymbol = data.taskSymbol;
        }

        #endregion
    }
}