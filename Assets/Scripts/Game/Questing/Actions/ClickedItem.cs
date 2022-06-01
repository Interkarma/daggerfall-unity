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
using DaggerfallWorkshop.Utility;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    public class ClickedItem : ActionTemplate
    {
        Symbol itemSymbol;
        int id;

        public override string Pattern
        {
            get { return @"clicked item (?<anItem>[a-zA-Z0-9_.-]+) say (?<id>\d+)|" +
                         @"clicked item (?<anItem>[a-zA-Z0-9_.-]+) say (?<idName>\w+)|" +
                         @"clicked item (?<anItem>[a-zA-Z0-9_.-]+)"; }
        }

        public ClickedItem(Quest parentQuest)
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
            ClickedItem action = new ClickedItem(parentQuest);
            action.itemSymbol = new Symbol(match.Groups["anItem"].Value);
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

            // Get related Item resource
            Item item = ParentQuest.GetItem(itemSymbol);
            if (item == null)
                return false;

            // Check item clicked flag
            if (item.HasPlayerClicked)
            {
                //item.RearmPlayerClick();
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
            public Symbol itemSymbol;
            public int id;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.itemSymbol = itemSymbol;
            data.id = id;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            itemSymbol = data.itemSymbol;
            id = data.id;
        }

        #endregion
    }
}