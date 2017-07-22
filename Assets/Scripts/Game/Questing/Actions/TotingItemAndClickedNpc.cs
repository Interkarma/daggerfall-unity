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

        public override string Pattern
        {
            get { return @"toting (?<anItem>[a-zA-Z0-9_.]+) and (?<anNPC>[a-zA-Z0-9_.-]+) clicked"; }
        }

        public TotingItemAndClickedNpc(Quest parentQuest)
                : base(parentQuest)
        {
            IsTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            base.CreateNew(source, parentQuest);

            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            TotingItemAndClickedNpc action = new TotingItemAndClickedNpc(parentQuest);
            action.itemSymbol = new Symbol(match.Groups["anItem"].Value);
            action.npcSymbol = new Symbol(match.Groups["anNPC"].Value);

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            // Always return true once owning Task is triggered
            // Another action will need to rearm/unset this task if another click is required
            // This seems to fit how classic works based on current observation
            if (caller.IsSet)
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
                person.RearmPlayerClick();
                if (GameManager.Instance.PlayerEntity.Items.Contains(item))
                    return true;
            }

            return false;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol itemSymbol;
            public Symbol npcSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.itemSymbol = itemSymbol;
            data.npcSymbol = npcSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            SaveData_v1 data = (SaveData_v1)dataIn;
            if (dataIn == null)
                return;

            itemSymbol = data.itemSymbol;
            npcSymbol = data.npcSymbol;
        }

        #endregion
    }
}