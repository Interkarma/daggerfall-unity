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

using UnityEngine;
using System.Text.RegularExpressions;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Give a quest Item to player.
    /// Currently ignoring "get item from" as it appears to behave identically to "get item".
    /// </summary>
    public class GetItem : ActionTemplate
    {
        Symbol itemSymbol;

        public override string Pattern
        {
            get { return @"get item (?<anItem>[a-zA-Z0-9_.]+)"; }
        }

        public GetItem(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            base.CreateNew(source, parentQuest);

            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            GetItem action = new GetItem(parentQuest);
            action.itemSymbol = new Symbol(match.Groups["anItem"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Attempt to get Item resource
            Item item = ParentQuest.GetItem(itemSymbol);
            if (item == null)
            {
                Debug.LogErrorFormat("Could not find Item resource symbol {0}", itemSymbol);
                return;
            }

            // Give quest item to player
            GameManager.Instance.PlayerEntity.Items.AddItem(item.DaggerfallUnityItem, Items.ItemCollection.AddPosition.Front);

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol itemSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.itemSymbol = itemSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            SaveData_v1 data = (SaveData_v1)dataIn;
            if (dataIn == null)
                return;

            itemSymbol = data.itemSymbol;
        }

        #endregion
    }
}