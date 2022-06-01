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
    /// Adds Qrc text message to player journal
    /// </summary>
    public class LogMessage : ActionTemplate
    {
        /// Note: Daggerfall groups journal entries together by quest (max 32 active at once)
        /// each quest can have up to 10 journal entries
        /// if message has already been added at stepID index, old message gets replaced

        int messageID;  //Qrc message #
        int stepID;

        public override string Pattern
        {
            get { return @"log (?<id>\d+)( step)? (?<step>\d+)"; }
        }

        public LogMessage(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            LogMessage action   = new LogMessage(parentQuest);

            try
            {
                // Factory new action
                action.messageID    = Parser.ParseInt(match.Groups["id"].Value);
                action.stepID       = Parser.ParseInt(match.Groups["step"].Value);
            }
            catch(System.Exception ex)
            {
                DaggerfallUnity.LogMessage("LogMessage.Create() failed with exception: " + ex.Message, true);
                action = null;
            }

            return action;
        }

        public override void Update(Task caller)
        {
            ParentQuest.AddLogStep(stepID, messageID);
            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int messageID;
            public int stepID;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.messageID = messageID;
            data.stepID = stepID;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            messageID = data.messageID;
            stepID = data.stepID;
        }

        #endregion
    }
}