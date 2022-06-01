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
using DaggerfallWorkshop.Game.Entity;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    public class GiveItem : ActionTemplate
    {
        Symbol itemSymbol;
        Symbol targetSymbol;

        public override string Pattern
        {
            get { return @"give item (?<anItem>[a-zA-Z0-9_.]+) to (?<aResource>[a-zA-Z0-9_.]+)"; }
        }

        public GiveItem(Quest parentQuest)
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
            GiveItem action = new GiveItem(parentQuest);
            action.itemSymbol = new Symbol(match.Groups["anItem"].Value);
            action.targetSymbol = new Symbol(match.Groups["aResource"].Value);

            return action;
        }

        public override void Update(Task caller)
        {
            base.Update(caller);

            // Attempt to get Item resource
            Item item = ParentQuest.GetItem(itemSymbol);
            if (item == null)
            {
                SetComplete();
                throw new Exception(string.Format("Could not find Item resource symbol {0}", itemSymbol));
            }

            // Attempt to get target resource
            QuestResource target = ParentQuest.GetResource(targetSymbol);
            if (target == null)
            {
                SetComplete();
                throw new Exception(string.Format("Could not find target resource symbol {0}", targetSymbol));
            }

            // Add item to Foe resource or generic entity
            // In practice GiveItem will always be to a Foe resource - NPCs are handled by "get item"
            // Keeping generic GiveItem behaviour as entity catch-all and reference
            if (target is Foe)
            {
                // Add to Foe item queue
                (target as Foe).QueueItem(item.DaggerfallUnityItem);

                // Dequeue items on entity immediately if target already exists in the world
                // Will also handle placing items to dead enemy loot container
                if (target.QuestResourceBehaviour)
                {
                    DaggerfallEntityBehaviour entityBehaviour = target.QuestResourceBehaviour.GetComponent<DaggerfallEntityBehaviour>();
                    if (entityBehaviour)
                        target.QuestResourceBehaviour.AddItemQueue(target as Foe, entityBehaviour);
                }
            }
            else
            {
                // Target must exist in the world
                if (!target.QuestResourceBehaviour)
                    return;

                // Must have an Entity to receive item
                DaggerfallEntityBehaviour entityBehaviour = target.QuestResourceBehaviour.GetComponent<DaggerfallEntityBehaviour>();
                if (!entityBehaviour)
                {
                    SetComplete();
                    throw new Exception(string.Format("GiveItem target {0} is not an Entity with DaggerfallEntityBehaviour", targetSymbol));
                }

                // Assign item for player to find
                //  * Some quests assign item to Foe at create time, others on injured event
                //  * It's possible for target enemy to be one-shot or to be killed by other means (such as "killall")
                //  * This assignment will direct quest loot item either to live enemy or corpse loot container
                if (entityBehaviour.CorpseLootContainer)
                {
                    // If enemy is already dead then place item in corpse loot container
                    entityBehaviour.CorpseLootContainer.Items.AddItem(item.DaggerfallUnityItem);
                }
                else
                {
                    // Otherwise add quest Item to Entity item collection
                    // It will be transferred to corpse marker loot container when dropped
                    entityBehaviour.Entity.Items.AddItem(item.DaggerfallUnityItem);
                }
            }

            // Remove item from player inventory if they are holding it
            if (GameManager.Instance.PlayerEntity.Items.Contains(item.DaggerfallUnityItem))
            {
                GameManager.Instance.PlayerEntity.Items.RemoveItem(item.DaggerfallUnityItem);
            }

            SetComplete();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public Symbol itemSymbol;
            public Symbol targetSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.itemSymbol = itemSymbol;
            data.targetSymbol = targetSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            itemSymbol = data.itemSymbol;
            targetSymbol = data.targetSymbol;
        }

        #endregion
    }
}