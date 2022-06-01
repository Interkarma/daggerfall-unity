// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using FullSerializer;
using System.Text.RegularExpressions;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Starts another quest and waits for its termination, then sets task for success or failure.
    /// Sets failure task immediately if target quest is not found.
    /// Will ensure that target quest is also terminated if still running when parent quest ends.  
    /// </summary>
    public class RunQuest : ActionTemplate
    {  
        Quest quest;
        string questName;
        ulong? questUId;
        bool questStarted;
        Symbol successSymbol;
        Symbol failureSymbol;

        public override string Pattern
        {
            get { return @"run quest (?<questName>\w+) then (?<successTask>[a-zA-Z0-9_.]+) or (?<failureTask>[a-zA-Z0-9_.]+)"; }
        }

        public RunQuest(Quest parentQuest)
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
            var action = new RunQuest(parentQuest);
            action.questName = match.Groups["questName"].Value;
            action.successSymbol = new Symbol(match.Groups["successTask"].Value);
            action.failureSymbol = new Symbol(match.Groups["failureTask"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            if (!questStarted)
            {
                // Start quest from name and store UID if found
                quest = GameManager.Instance.QuestListsManager.GetQuest(questName);
                if (quest != null)
                {
                    questUId = quest.UID;
                    QuestMachine.Instance.ScheduleQuest(quest);
                }
                else
                {
                    questUId = null;
                }

                questStarted = true;
            }
            
            if (quest == null)
            {
                // Try to retrieve quest after deserialization
                if (questUId != null)
                    quest = QuestMachine.Instance.GetQuest(questUId.Value);

                // Set 'failure' task if quest is not found
                if (quest == null)
                {
                    ParentQuest.StartTask(failureSymbol);
                    SetComplete();
                    return;
                }
            }

            if (quest.QuestComplete)
            {
                // Set 'success' or 'failure' task
                ParentQuest.StartTask(quest.QuestSuccess ? successSymbol : failureSymbol);
                SetComplete();
                quest = null;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            if (quest != null && !quest.QuestComplete)
            {
                QuestMachine.Instance.TombstoneQuest(quest);
                quest = null;
            }
        }

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public string questName;
            public ulong? questUId;
            public bool questStarted;
            public Symbol successSymbol;
            public Symbol failureSymbol;
        }

        public override object GetSaveData()
        {
            var data = new SaveData_v1();
            data.questName = questName;
            data.questUId = questUId;
            data.questStarted = questStarted;
            data.successSymbol = successSymbol;
            data.failureSymbol = failureSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            var data = (SaveData_v1)dataIn;
            questName = data.questName;
            questUId = data.questUId;
            questStarted = data.questStarted;
            successSymbol = data.successSymbol;
            failureSymbol = data.failureSymbol;
        }
    }
}
