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

using System;
using UnityEngine;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Give a quest Item to player. This has three formats:
    ///  * "give pc anItem" - Displays QuestComplete success message and opens loot window with reward. Could probably be called "give quest reward anItem".
    ///  * "give pc nothing" - Also displays QuestComplete success message but does not open loot window as no reward.
    ///  * "give pc anItem notify nnnn" - Places item directly into player's inventory and says message ID nnnn.
    /// </summary>
    public class GivePc : ActionTemplate
    {
        Symbol itemSymbol;
        int textId;
        bool isNothing;
        bool silently;

        bool offerImmediately = false;
        bool waitingForTown = false;
        int ticksUntilFire = 0;

        DaggerfallLoot rewardLoot = null;

        public override string Pattern
        {
            get { return @"give pc (?<nothing>nothing)|give pc (?<anItem>[a-zA-Z0-9_.]+) notify (?<id>\d+)|give pc (?<anItem>[a-zA-Z0-9_.]+) (?<silently>silently)|give pc (?<anItem>[a-zA-Z0-9_.]+)"; }
        }

        public GivePc(Quest parentQuest)
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
            GivePc action = new GivePc(parentQuest);
            action.itemSymbol = new Symbol(match.Groups["anItem"].Value);
            action.textId = Parser.ParseInt(match.Groups["id"].Value);
            action.isNothing = !string.IsNullOrEmpty(match.Groups["nothing"].Value);
            action.silently = !string.IsNullOrEmpty(match.Groups["silently"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            const int minHour = 7;
            const int maxHour = 18;
            const int minDelay = 40;
            const int maxDelay = 500;

            base.Update(caller);

            // "give pc anItem notify 1234" and "give pc anItem silently" require player
            // to be within a town, outdoors, and during set hours for action to fire
            if ((textId != 0 || silently) && !offerImmediately)
            {
                // Fail if conditions not met, but take note we are waiting
                DaggerfallDateTime now = DaggerfallUnity.Instance.WorldTime.Now;
                if (!GameManager.Instance.PlayerGPS.IsPlayerInTown(true, true) || now.Hour < minHour || now.Hour > maxHour)
                {
                    waitingForTown = true;
                    ticksUntilFire = 0;
                    return;
                }

                // If we were waiting then add a small random delay so messages don't all arrive at once
                if (waitingForTown)
                {
                    ticksUntilFire = UnityEngine.Random.Range(minDelay, maxDelay + 1);
                    waitingForTown = false;
                    RaiseOnOfferPendingEvent(this);
                }
            }

            // Reduce random delay until nothing is left
            // This is in questmachine ticks which is currently 10 ticks per second while game is running
            // Does not tick while game paused or resting - player must be actively in town walking around
            if (ticksUntilFire > 0)
            {
                ticksUntilFire--;
                return;
            }

            // Handle giving player nothing
            // This also shows QuestComplete message but does not give player loot
            if (isNothing)
            {
                OfferToPlayerWithQuestComplete(null);
                SetComplete();
                return;
            }

            // Attempt to get Item resource
            Item item = ParentQuest.GetItem(itemSymbol);
            if (item == null)
            {
                Debug.LogErrorFormat("Could not find Item resource symbol {0}", itemSymbol);
                return;
            }

            // Give quest item to player based on command format
            if (textId != 0)
            {
                DirectToPlayerWithNotify(item);
            }
            else
            {
                if (silently)
                    DirectToPlayerWithoutNotify(item);
                else
                    OfferToPlayerWithQuestComplete(item);
            }

            offerImmediately = false;
            SetComplete();
        }

        public void OfferImmediately()
        {
            waitingForTown = false;
            ticksUntilFire = 0;
            offerImmediately = true;
        }

        void OfferToPlayerWithQuestComplete(Item item)
        {
            // Quest successful
            ParentQuest.QuestSuccess = true;

            // Show quest complete message
            DaggerfallMessageBox messageBox = ParentQuest.ShowMessagePopup((int)QuestMachine.QuestMessages.QuestComplete);

            // If no item for reward then we are done
            if (item == null)
                return;

            // Release item so we can offer back to player
            // Sometimes a quest item is both carried by player then offered back to them
            // Example is Sx010 where "curse" is removed and player can keep item
            GameManager.Instance.PlayerEntity.ReleaseQuestItemForReoffer(ParentQuest.UID, item, true);

            // Create a dropped loot container window for player to loot their reward
            rewardLoot = GameObjectHelper.CreateDroppedLootContainer(GameManager.Instance.PlayerObject, DaggerfallUnity.NextUID);
            rewardLoot.ContainerImage = InventoryContainerImages.Merchant;
            rewardLoot.Items.AddItem(item.DaggerfallUnityItem);

            // Schedule loot window to open when player dismisses message
            messageBox.OnClose += QuestCompleteMessage_OnClose;
        }

        void DirectToPlayerWithNotify(Item item)
        {
            // Give player item and show notify message
            GameManager.Instance.PlayerEntity.Items.AddItem(item.DaggerfallUnityItem, Items.ItemCollection.AddPosition.Front);
            ParentQuest.ShowMessagePopup(textId);
        }

        void DirectToPlayerWithoutNotify(Item item)
        {
            // Just give player item
            GameManager.Instance.PlayerEntity.Items.AddItem(item.DaggerfallUnityItem, Items.ItemCollection.AddPosition.Front);
        }

        private void QuestCompleteMessage_OnClose()
        {
            // Open loot reward container once QuestComplete dismissed
            if (rewardLoot != null)
            {
                DaggerfallUI.Instance.InventoryWindow.LootTarget = rewardLoot;
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenInventoryWindow);
            }
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol itemSymbol;
            public int textId;
            public bool isNothing;
            public bool silently;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.itemSymbol = itemSymbol;
            data.textId = textId;
            data.isNothing = isNothing;
            data.silently = silently;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            itemSymbol = data.itemSymbol;
            textId = data.textId;
            isNothing = data.isNothing;
            silently = data.silently;
        }

        #endregion

        #region Event Handlers

        // OnOfferPending
        public delegate void OnOfferPendingHandler(GivePc sender);
        public static event OnOfferPendingHandler OnOfferPending;
        protected virtual void RaiseOnOfferPendingEvent(GivePc sender)
        {
            if (OnOfferPending != null)
                OnOfferPending(sender);
        }

        #endregion
    }
}