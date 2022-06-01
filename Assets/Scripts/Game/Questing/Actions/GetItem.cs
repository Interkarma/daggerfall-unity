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

using UnityEngine;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
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
        int textId;

        public override string Pattern
        {
            get { return @"get item (?<anItem>[a-zA-Z0-9_.]+) saying (?<id>\d+)|" +
                         @"get item (?<anItem>[a-zA-Z0-9_.]+)"; }
        }

        public GetItem(Quest parentQuest)
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
            GetItem action = new GetItem(parentQuest);
            action.itemSymbol = new Symbol(match.Groups["anItem"].Value);
            action.textId = Parser.ParseInt(match.Groups["id"].Value);

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

            // Release item so we can give back to player
            // Sometimes a quest item is both carried by player then handed back to them
            // Example is Sx012 where courier hands back two items of jewellery
            GameManager.Instance.PlayerEntity.ReleaseQuestItemForReoffer(ParentQuest.UID, item);

            // Give quest item to player
            if (item.DaggerfallUnityItem.IsOfTemplate(ItemGroups.Currency, (int)Currency.Gold_pieces))
            {
                // Give player gold equal to stack size and notify
                int amount = item.DaggerfallUnityItem.stackCount;
                GameManager.Instance.PlayerEntity.GoldPieces += amount;
                DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("youReceiveGoldPieces").Replace("%s", amount.ToString()));
            }
            else
            {
                // Give player actual item
                GameManager.Instance.PlayerEntity.Items.AddItem(item.DaggerfallUnityItem, ItemCollection.AddPosition.Front);
            }

            // "saying" popup
            if (textId != 0)
            {
                ParentQuest.ShowMessagePopup(textId);
            }

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol itemSymbol;
            public int textId;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.itemSymbol = itemSymbol;
            data.textId = textId;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            itemSymbol = data.itemSymbol;
            textId = data.textId;
        }

        #endregion
    }
}