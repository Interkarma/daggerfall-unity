// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
using System;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    public class EndQuest : ActionTemplate
    {
        int textId;

        public override string Pattern
        {
            get { return @"end quest saying (?<id>\d+)|" +
                         @"end quest"; }
        }

        public EndQuest(Quest parentQuest)
            : base(parentQuest)
        {
            allowRearm = false;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            EndQuest action = new EndQuest(parentQuest);
            action.textId = Parser.ParseInt(match.Groups["id"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            // "saying" popup
            if (textId != 0)
            {
                ParentQuest.ShowMessagePopup(textId);
            }

            // Flag quest over so quest machine can remove it
            //Debug.LogFormat("Ending quest {0}", ParentQuest.UID);
            ParentQuest.QuestBreak = true;
            ParentQuest.EndQuest();
            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int textId;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.textId = textId;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            textId = data.textId;
        }

        #endregion
    }
}