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

using System.Text.RegularExpressions;
using System;
using DaggerfallConnect.Arena2;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Starts another quest. Classic 'start quest' can only start Sx000 value quests.
    /// Likely to create a Daggerfall Unity specific start quest variant that can start any named quest.
    /// In canonical quests always lists same quest index twice, e.g. "start quest 1 1".
    /// This implementation only uses first index, purpose of second index currently unknown.
    /// </summary>
    public class StartQuest : ActionTemplate
    {
        int questIndex1;
        int questIndex2;
        string questName;

        public override string Pattern
        {
            get { return @"start quest (?<questIndex1>\d+) (?<questIndex2>\d+)|start quest (?<questName>\w+)"; }
        }

        public StartQuest(Quest parentQuest)
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
            StartQuest action = new StartQuest(parentQuest);
            action.questIndex1 = Parser.ParseInt(match.Groups["questIndex1"].Value);
            action.questIndex2 = Parser.ParseInt(match.Groups["questIndex2"].Value);
            action.questName = match.Groups["questName"].Value;

            return action;
        }

        public override void Update(Task caller)
        {
            // Convert quest index to name
            if (string.IsNullOrEmpty(questName) && questIndex1 > 0)
                questName = string.Format("S{0:0000000}", questIndex1);

            // Attempt to parse quest
            Quest quest = GameManager.Instance.QuestListsManager.GetQuest(questName);
            if (quest != null)
            {
                QuestMachine.Instance.ScheduleQuest(quest);
            }

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int questIndex1;
            public int questIndex2;
            public string questName;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.questIndex1 = questIndex1;
            data.questIndex2 = questIndex2;
            data.questName = questName;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            questIndex1 = data.questIndex1;
            questIndex2 = data.questIndex2;
            questName = data.questName;
        }

        #endregion
    }
}