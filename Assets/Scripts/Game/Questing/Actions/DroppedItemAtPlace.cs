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
using System;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    public class DroppedItemAtPlace : ActionTemplate
    {
        Symbol itemSymbol;
        Symbol placeSymbol;

        public override string Pattern
        {
            get { return @"dropped (?<anItem>[a-zA-Z0-9_.-]+) at (?<aPlace>[a-zA-Z0-9_.-]+)"; }
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

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
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
                item.DaggerfallUnityItem.AllowQuestItemRemoval = true;
            }
            else
            {
                item.DaggerfallUnityItem.AllowQuestItemRemoval = false;
                return false;
            }

            // Handle player dropped
            if (item.PlayerDropped)
                return true;

            return false;
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol itemSymbol;
            public Symbol placeSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.itemSymbol = itemSymbol;
            data.placeSymbol = placeSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            SaveData_v1 data = (SaveData_v1)dataIn;
            if (dataIn == null)
                return;

            itemSymbol = data.itemSymbol;
            placeSymbol = data.placeSymbol;
        }

        #endregion
    }
}