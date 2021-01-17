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

using System.Text.RegularExpressions;
using DaggerfallWorkshop.Utility;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Handles player clicking on NPC.
    /// </summary>
    public class ClickedNpc : ActionTemplate
    {
        Symbol npcSymbol;
        int id;
        int goldAmount;
        Symbol taskSymbol;

        public override string Pattern
        {
            get { return @"clicked (?<anNPC>[a-zA-Z0-9_.-]+) and at least (?<goldAmount>\d+) gold otherwise do (?<taskName>[a-zA-Z0-9_.]+)|" +
                         @"clicked npc (?<anNPC>[a-zA-Z0-9_.-]+) say (?<id>\d+)|" +
                         @"clicked npc (?<anNPC>[a-zA-Z0-9_.-]+) say (?<idName>\w+)|" +
                         @"clicked npc (?<anNPC>[a-zA-Z0-9_.-]+)"; }
        }

        public ClickedNpc(Quest parentQuest)
            : base(parentQuest)
        {
            IsTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            ClickedNpc action = new ClickedNpc(parentQuest);
            action.npcSymbol = new Symbol(match.Groups["anNPC"].Value);
            action.id = Parser.ParseInt(match.Groups["id"].Value);

            // Resolve static message back to ID
            string idName = match.Groups["idName"].Value;
            if (action.id == 0 && !string.IsNullOrEmpty(idName))
            {
                Table table = QuestMachine.Instance.StaticMessagesTable;
                action.id = Parser.ParseInt(table.GetValue("id", idName));
            }

            // Read gold amount and task name for "clicked anNPC and at least goldAmount gold otherwise do taskName"
            action.goldAmount = Parser.ParseInt(match.Groups["goldAmount"].Value);
            action.taskSymbol = new Symbol(match.Groups["taskName"].Value);

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            // Always return true once owning Task is triggered
            // Another action will need to rearm/unset this task if another click is required
            // This seems to fit how classic works based on current observation
            if (caller.IsTriggered)
                return true;

            // Get related Person resource
            Person person = ParentQuest.GetPerson(npcSymbol);
            if (person == null)
                return false;

            // Check player clicked flag
            if (person.HasPlayerClicked)
            {
                // When a gold amount and task is specified, the player must have that amount of gold or another task is called
                if (goldAmount > 0 && taskSymbol != null && !string.IsNullOrEmpty(taskSymbol.Name))
                {
                    // Does player have enough gold?
                    if (GameManager.Instance.PlayerEntity.GoldPieces >= goldAmount)
                    {
                        // Then deduct gold and fire trigger
                        GameManager.Instance.PlayerEntity.GoldPieces -= goldAmount;
                    }
                    else
                    {
                        // Otherwise trigger secondary task and exit without firing trigger
                        ParentQuest.StartTask(taskSymbol);
                        return false;
                    }
                }

                if (id != 0)
                    ParentQuest.ShowMessagePopup(id);

                return true;
            }

            return false;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol npcSymbol;
            public int id;
            public int goldAmount;
            public Symbol taskSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.npcSymbol = npcSymbol;
            data.id = id;
            data.goldAmount = goldAmount;
            data.taskSymbol = taskSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            npcSymbol = data.npcSymbol;
            id = data.id;
            goldAmount = data.goldAmount;
            taskSymbol = data.taskSymbol;
        }

        #endregion
    }
}