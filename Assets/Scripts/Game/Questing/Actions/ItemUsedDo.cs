// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System.Text.RegularExpressions;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Condition that fires when player equips an Item.
    /// Seen in Sx006 when player equips the robe.
    /// Outcome is distinct from "use" in inventory window.
    /// </summary>
    public class ItemUsedDo : ActionTemplate
    {
        Symbol itemSymbol;
        Symbol taskSymbol;
        int textID;

        public override string Pattern
        {
            get { return @"(?<anItem>[a-zA-Z0-9_.-]+) used do (?<aTask>[a-zA-Z0-9_.-]+)|" +
                         @"(?<anItem>[a-zA-Z0-9_.-]+) used saying (?<textID>\d+) do (?<aTask>[a-zA-Z0-9_.-]+)"; }
        }

        public ItemUsedDo(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            ItemUsedDo action = new ItemUsedDo(parentQuest);
            action.itemSymbol = new Symbol(match.Groups["anItem"].Value);
            action.taskSymbol = new Symbol(match.Groups["aTask"].Value);
            action.textID = Parser.ParseInt(match.Groups["textID"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            // Attempt to get Item resource
            Item item = ParentQuest.GetItem(itemSymbol);
            if (item == null)
                return;

            // Check if player is wearing item
            if (!GameManager.Instance.PlayerEntity.ItemEquipTable.IsEquipped(item.DaggerfallUnityItem))
                return;

            // Say message
            if (textID != 0)
                ParentQuest.ShowMessagePopup(textID);

            // Trigger target task
            ParentQuest.StartTask(taskSymbol);

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol itemSymbol;
            public Symbol taskSymbol;
            public int textID;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.itemSymbol = itemSymbol;
            data.taskSymbol = taskSymbol;
            data.textID = textID;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            SaveData_v1 data = (SaveData_v1)dataIn;
            if (dataIn == null)
                return;

            itemSymbol = data.itemSymbol;
            taskSymbol = data.taskSymbol;
            textID = data.textID;
        }

        #endregion
    }
}