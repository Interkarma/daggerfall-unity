// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Removes Qrc text message from player journal
    /// </summary>
    public class RemoveLogMessage : ActionTemplate
    {
        int stepID; // log entry to remove

        public override string Pattern
        {
            get { return @"remove log step (?<step>\d+)"; }
        }

        public RemoveLogMessage(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            RemoveLogMessage action = new RemoveLogMessage(parentQuest);

            try
            {
                action.stepID = Parser.ParseInt(match.Groups["step"].Value);
            }
            catch (System.Exception ex)
            {
                DaggerfallUnity.LogMessage("RemoveLogMessage.Create() failed with exception: " + ex.Message, true);
                action = null;
            }

            return action;
        }

        public override void Update(Task caller)
        {
            ParentQuest.RemoveLogStep(stepID);
            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int stepID;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.stepID = stepID;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            stepID = data.stepID;
        }

        #endregion
    }
}
