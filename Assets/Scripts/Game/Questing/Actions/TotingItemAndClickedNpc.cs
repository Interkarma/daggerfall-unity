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

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Condition triggers when player clicks on NPC while holding a quest Item in their inventory.
    /// Superficially very similar to ClickedNpc but also requires item check to be true.
    /// </summary>
    public class TotingItemAndClickedNpc : ActionTemplate
    {
        Symbol itemSymbol;
        Symbol npcSymbol;
        int id;

        public override string Pattern
        {
            get { return @"toting (?<anItem>[a-zA-Z0-9_.]+) and (?<anNPC>[a-zA-Z0-9_.-]+) clicked saying (?<id>\d+)|" +
                         @"toting (?<anItem>[a-zA-Z0-9_.]+) and (?<anNPC>[a-zA-Z0-9_.-]+) clicked saying (?<idName>\w+)|" +
                         @"toting (?<anItem>[a-zA-Z0-9_.]+) and (?<anNPC>[a-zA-Z0-9_.-]+) clicked"; }
        }

        public TotingItemAndClickedNpc(Quest parentQuest)
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
            TotingItemAndClickedNpc action = new TotingItemAndClickedNpc(parentQuest);
            action.itemSymbol = new Symbol(match.Groups["anItem"].Value);
            action.npcSymbol = new Symbol(match.Groups["anNPC"].Value);
            action.id = Parser.ParseInt(match.Groups["id"].Value);

            // Resolve static message back to ID
            string idName = match.Groups["idName"].Value;
            if (action.id == 0 && !string.IsNullOrEmpty(idName))
            {
                Table table = QuestMachine.Instance.StaticMessagesTable;
                action.id = Parser.ParseInt(table.GetValue("id", idName));
            }

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

            // Get related Item resource
            Item item = ParentQuest.GetItem(itemSymbol);
            if (item == null)
                return false;

            // Check player clicked flag
            if (person.HasPlayerClicked)
            {
                // Check if player has item
                if (GameManager.Instance.PlayerEntity.Items.Contains(item))
                {
                    // Show message popup, remove item, return true on trigger
                    ParentQuest.ShowMessagePopup(id);
                    GameManager.Instance.PlayerEntity.ReleaseQuestItemForReoffer(ParentQuest.UID, item);
                    return true;
                }
            }

            return false;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol itemSymbol;
            public Symbol npcSymbol;
            public int id;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.itemSymbol = itemSymbol;
            data.npcSymbol = npcSymbol;
            data.id = id;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            itemSymbol = data.itemSymbol;
            npcSymbol = data.npcSymbol;
            id = data.id;
        }

        #endregion
    }
}