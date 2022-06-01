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
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    public class DroppedItemAtPlace : ActionTemplate
    {
        Symbol itemSymbol;
        Symbol placeSymbol;
        int textId;
        bool textShown;

        public override string Pattern
        {
            get { return @"dropped (?<anItem>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+) saying (?<id>\d+)|" +
                         @"dropped (?<anItem>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+)"; }
        }

        public DroppedItemAtPlace(Quest parentQuest)
            : base(parentQuest)
        {
            IsTriggerCondition = true;
            IsAlwaysOnTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            DroppedItemAtPlace action = new DroppedItemAtPlace(parentQuest);
            action.itemSymbol = new Symbol(match.Groups["anItem"].Value);
            action.placeSymbol = new Symbol(match.Groups["aPlace"].Value);
            action.textId = Parser.ParseInt(match.Groups["id"].Value);

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            // Always return true once owning Task is triggered
            // Another action will need to rearm/unset this task if another click is required
            // This seems to fit how classic works based on current observation
            if (caller.IsTriggered)
                return true;

            // Attempt to get Item resource
            Item item = ParentQuest.GetItem(itemSymbol);
            if (item == null)
                return false;

            // Attempt to get Place resource
            Place place = ParentQuest.GetPlace(placeSymbol);
            if (place == null)
                return false;

            // Watch item
            item.ActionWatching = true;

            // Allow drop only when player at correct location
            if (place.IsPlayerHere())
            {
                item.AllowDrop = true;
            }
            else
            {
                item.AllowDrop = false;
                return false;
            }

            // Handle player dropped
            if (item.PlayerDropped)
            {
                // Show text
                if (textId != 0 && !textShown)
                {
                    ParentQuest.ShowMessagePopup(textId);
                    textShown = true;
                }

                SetComplete();
                return true;
            }

            return false;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol itemSymbol;
            public Symbol placeSymbol;
            public int textId;
            public bool textShown;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.itemSymbol = itemSymbol;
            data.placeSymbol = placeSymbol;
            data.textId = textId;
            data.textShown = textShown;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            itemSymbol = data.itemSymbol;
            placeSymbol = data.placeSymbol;
            textId = data.textId;
            textShown = data.textShown;
        }

        #endregion
    }
}