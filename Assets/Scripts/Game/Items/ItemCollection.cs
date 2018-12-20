// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Questing;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// Collection of items.
    /// Items are stored in an ordered dictionary keyed to a unique identifier.
    /// This collection may represent an inventory, a loot pile, harvestable items, etc.
    /// </summary>
    [Serializable]
    public class ItemCollection
    {
        #region Fields

        OrderedDictionary items = new OrderedDictionary();

        #endregion

        #region Custom item registry.

        private static Dictionary<string, Type> customItems = new Dictionary<string, Type>();

        public static bool RegisterCustomItem(string className, Type guildType)
        {
            DaggerfallUnity.LogMessage("RegisterCustomItem: " + className, true);
            if (!customItems.ContainsKey(className))
            {
                customItems.Add(className, guildType);
                return true;
            }
            return false;
        }

        #endregion

        #region Enums

        /// <summary>
        /// Where to position items added to collection.
        /// </summary>
        public enum AddPosition
        {
            DontCare,
            Front,
            Back,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets count of items in collection.
        /// </summary>
        public int Count
        {
            get { return items.Count; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets number of items in collection, including item stack sizes.
        /// </summary>
        public int GetNumItems()
        {
            int num = 0;
            foreach (DaggerfallUnityItem item in items.Values)
                num += item.stackCount;
            return num;
        }

        /// <summary>
        /// Gets the combined weight of all the items in this collection. (ignoring arrows like classic)
        /// </summary>
        /// <returns>Weight in kg</returns>
        public float GetWeight()
        {
            float weight = 0;
            foreach (DaggerfallUnityItem item in items.Values)
            {
                weight += item.stackCount * item.EffectiveUnitWeightInKg();
            }
            return weight;
        }

        /// <summary>
        /// Gets the value of all the credit letters in this collection.
        /// </summary>
        /// <returns>Amount in gp</returns>
        public int GetCreditAmount()
        {
            int locGroupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.MiscItems, (int)MiscItems.Letter_of_credit);
            int amount = 0;
            foreach (DaggerfallUnityItem item in items.Values)
            {
                if (item.ItemGroup == ItemGroups.MiscItems && item.GroupIndex == locGroupIndex)
                    amount += item.value;
            }
            return amount;
        }

        /// <summary>
        /// Removes items that have expired. Used for magically-created items. Only for the player.
        /// Note: Reverse-engineering suggests this was intended behavior in classic, but classic
        /// does not correctly set the item flags so magically-created items never disappear.
        /// </summary>
        public void RemoveExpiredItems()
        {
            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            List<DaggerfallUnityItem> itemsToRemove = new List<DaggerfallUnityItem>();

            foreach (DaggerfallUnityItem item in items.Values)
            {
                if (item.TimeForItemToDisappear != 0 && item.TimeForItemToDisappear < gameMinutes)
                {
                    Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
                    foreach (EquipSlots slotToCheck in Enum.GetValues(typeof(EquipSlots)))
                    {
                        if (player.ItemEquipTable.GetItem(slotToCheck) == item)
                            player.ItemEquipTable.UnequipItem(slotToCheck);
                    }
                    itemsToRemove.Add(item);
                }
            }
            foreach (DaggerfallUnityItem item in itemsToRemove)
            {
                RemoveItem(item);
            }
        }

        /// <summary>
        /// Check if item exists in this collection.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>True if item exists in this collection.</returns>
        public bool Contains(DaggerfallUnityItem item)
        {
            if (item == null)
                return false;

            return items.Contains(item.UID);
        }

        /// <summary>
        /// Check if item UID exists in this collection.
        /// </summary>
        /// <param name="uid">UID to check.</param>
        /// <returns>True if item with UID exists in this collection.</returns>
        public bool Contains(ulong uid)
        {
            return items.Contains(uid);
        }

        /// <summary>
        /// Check if quest item held by player.
        /// </summary>
        /// <param name="questItem">Quest Item resource.</param>
        /// <returns>True if player holding this quest item.</returns>
        public bool Contains(Item questItem)
        {
            foreach (DaggerfallUnityItem item in items.Values)
            {
                if (item.IsQuestItem &&
                    item.QuestUID == questItem.ParentQuest.UID &&
                    item.QuestItemSymbol.Equals(questItem.Symbol))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if item type exists in this collection.
        /// </summary>
        /// <param name="itemGroup">Item group.</param>
        /// <param name="itemIndex">Template index.</param>
        /// <returns>True if collection contains an item of this type.</returns>
        public bool Contains(ItemGroups itemGroup, int itemIndex)
        {
            int groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(itemGroup, itemIndex);
            foreach (DaggerfallUnityItem item in items.Values)
            {
                if (item.ItemGroup == itemGroup && item.GroupIndex == groupIndex)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Adds an item to this collection.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="position">Position in list to insert item.</param>
        public void AddItem(DaggerfallUnityItem item, AddPosition position = AddPosition.Back, bool noStack = false)
        {
            if (item == null)
                return;

            // Add the item based on stack behaviour
            // TODO: Look at implementing proper stacking with max limits, split, merge, etc.
            DaggerfallUnityItem stack = FindExistingStack(item);
            if (stack != null && !noStack)
            {
                // Add to stack count
                stack.stackCount += item.stackCount;
            }
            else
            {
                // Check duplicate key
                if (items.Contains(item.UID))
                    throw new Exception("AddItem() encountered a duplicate item UID for " + item.LongName);

                // Add the item
                switch (position)
                {
                    case AddPosition.DontCare:
                        items.Add(item.UID, item);
                        break;
                    case AddPosition.Front:
                        items.Insert(0, item.UID, item);
                        break;
                    case AddPosition.Back:
                        items.Insert(Count, item.UID, item);
                        break;
                }
            }
        }

        /// <summary>
        /// Remove some number of items from a stack, return the removed items or null if not possible.
        /// </summary>
        /// <returns>The items picked from stack.</returns>
        /// <param name="stack">Source stack of items</param>
        /// <param name="numberToPick">Number of items to pick</param>
        public DaggerfallUnityItem SplitStack(DaggerfallUnityItem stack, int numberToPick) 
        {
            if (!stack.IsAStack() || numberToPick < 1 || numberToPick > stack.stackCount || !Contains(stack))
                return null;
            if (numberToPick == stack.stackCount)
                return stack;
            DaggerfallUnityItem pickedItems = new DaggerfallUnityItem(stack);
            pickedItems.stackCount = numberToPick;
            AddItem(pickedItems, noStack: true);
            stack.stackCount -= numberToPick;
            return pickedItems;
        }

        /// <summary>
        /// Removes an item from this collection.
        /// </summary>
        /// <param name="item">Item to remove. Must exist inside this collection.</param>
        public void RemoveItem(DaggerfallUnityItem item)
        {
            if (item == null)
                return;

            if (items.Contains(item.UID))
            {
                items.Remove(item.UID);
            }
        }

        /// <summary>
        /// Removes one item from this collection, decrementing stack count.
        /// </summary>
        /// <param name="item">Item to remove. Must exist inside this collection.</param>
        public void RemoveOne(DaggerfallUnityItem item)
        {
            if (item == null)
                return;

            if (items.Contains(item.UID))
            {
                item.stackCount--;
                if (item.stackCount <= 0)
                    RemoveItem(item);
            }
        }

        /// <summary>
        /// Reorders item in collection.
        /// </summary>
        /// <param name="item">Item to reorder. Must exist inside this collection.</param>
        /// <param name="position">Position to reorder to.</param>
        public void ReorderItem(DaggerfallUnityItem item, AddPosition position)
        {
            if (!items.Contains(item.UID))
                return;
            bool couldBeStacked = FindExistingStack(item) != null;
            if (position == AddPosition.DontCare && !couldBeStacked)
                return;

            RemoveItem(item);
            AddItem(item, position);
        }

        /// <summary>
        /// Gets item from UID.
        /// </summary>
        /// <param name="key">UID of item.</param>
        /// <returns>DaggerfallUnityItem.</returns>
        public DaggerfallUnityItem GetItem(ulong key)
        {
            if (items.Contains(key))
                return (DaggerfallUnityItem)items[key];
            else
                return null;
        }

        /// <summary>
        /// Gets item from index.
        /// </summary>
        /// <param name="index">Index of item between 0 and Count.</param>
        /// <returns>DaggerfallUnityItem.</returns>
        public DaggerfallUnityItem GetItem(int index)
        {
            if (index < 0 || index >= items.Count)
                return null;
            else
                return (DaggerfallUnityItem)items[index];
        }

        /// <summary>
        /// Get the first of an item type from this collection.
        /// </summary>
        /// <param name="itemGroup">Item group.</param>
        /// <param name="itemIndex">Template index.</param>
        /// <returns>An item of this type, or null if none found.</returns>
        public DaggerfallUnityItem GetItem(ItemGroups itemGroup, int itemIndex)
        {
            int groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(itemGroup, itemIndex);
            foreach (DaggerfallUnityItem item in items.Values)
            {
                if (item.ItemGroup == itemGroup && item.GroupIndex == groupIndex)
                    return item;
            }
            return null;
        }

        /// <summary>
        /// Clears all items from this collection.
        /// Items in this collection will be destroyed.
        /// </summary>
        public void Clear()
        {
            items.Clear();
        }

        /// <summary>
        /// Replaces all items in this collection with clones of items from source collection.
        /// Items in this collection will be destroyed. Source items will not be changed.
        /// New UIDs will be allocated to cloned items.
        /// </summary>
        /// <param name="source">Source collection to copy from.</param>
        public void ReplaceAll(ItemCollection source)
        {
            if (source == null)
                return;

            Clear();
            CopyAll(source);
        }

        /// <summary>
        /// Clones all items from source collection into this collection.
        /// Source items will not be changed.
        /// New UIDs will be allocated to cloned items.
        /// </summary>
        /// <param name="source">Source collection to copy from.</param>
        public void CopyAll(ItemCollection source)
        {
            if (source == null)
                return;

            foreach (DaggerfallUnityItem item in source.items.Values)
            {
                AddItem(item.Clone());
            }
        }

        /// <summary>
        /// Transfers all items from another collection.
        /// Items will be removed from source collection and placed in this collection.
        /// UIDs will be retained.
        /// </summary>
        /// <param name="source">Source collection to transfer from.</param>
        public void TransferAll(ItemCollection source)
        {
            if (source == null)
                return;

            foreach (DaggerfallUnityItem item in source.items.Values)
            {
                AddItem(item);
            }

            source.Clear();
        }

        /// <summary>
        /// Transfers a single item from another collection to this collection.
        /// Item will be removed from source collection and placed in this collection.
        /// UID will be retained.
        /// </summary>
        /// <param name="item">Item to transfer.</param>
        /// <param name="source">Source collection to transfer from.</param>
        /// <param name="position">Position in list to transfer item.</param>
        public void Transfer(DaggerfallUnityItem item, ItemCollection source, AddPosition position = AddPosition.DontCare)
        {
            if (item == null || source == null)
                return;

            source.RemoveItem(item);
            AddItem(item, position);
        }

        /// <summary>
        /// Imports items to this collection from an array of items.
        /// Items in this collection will be destroyed. Source items will not be changed.
        /// UIDs will be retained.
        /// </summary>
        /// <param name="items">Items array.</param>
        public void Import(DaggerfallUnityItem[] items)
        {
            if (items == null || items.Length == 0)
                return;

            Clear();
            for (int i = 0; i < items.Length; i++)
            {
                AddItem(items[i]);
            }
        }

        /// <summary>
        /// Exports items from this collection to an array of items.
        /// Items in this collection will be destroyed.
        /// UIDs will be retained.
        /// </summary>
        /// <returns>Items array.</returns>
        public DaggerfallUnityItem[] Export()
        {
            int index = 0;
            DaggerfallUnityItem[] itemArray = new DaggerfallUnityItem[items.Count];
            foreach (DaggerfallUnityItem item in items.Values)
            {
                itemArray[index++] = item;
            }

            Clear();

            return itemArray;
        }

        /// <summary>
        /// Serialize items from this collection.
        /// </summary>
        /// <returns>ItemData_v1 array.</returns>
        public ItemData_v1[] SerializeItems()
        {
            ItemData_v1[] itemArray = new ItemData_v1[Count];

            int index = 0;
            foreach (DaggerfallUnityItem item in items.Values)
            {
                itemArray[index++] = item.GetSaveData();
            }

            return itemArray;
        }

        /// <summary>
        /// Deserialize items into this collection.
        /// Existing items will be destroyed.
        /// </summary>
        /// <param name="itemArray">ItemData_v1 array.</param>
        public void DeserializeItems(ItemData_v1[] itemArray)
        {
            // Clear existing items
            Clear();

            // Nothing more to do if no items in source array
            if (itemArray == null || itemArray.Length == 0)
                return;

            // Add items to this collection
            for (int i = 0; i < itemArray.Length; i++)
            {
                if (itemArray[i].className != null)
                {
                    Type itemClassType;
                    customItems.TryGetValue(itemArray[i].className, out itemClassType);
                    if (itemClassType != null)
                    {
                        DaggerfallUnityItem modItem = (DaggerfallUnityItem)Activator.CreateInstance(itemClassType);
                        modItem.FromItemData(itemArray[i]);
                        AddItem(modItem, AddPosition.DontCare, true);
                        continue;
                    }
                }
                DaggerfallUnityItem item = new DaggerfallUnityItem(itemArray[i]);
                AddItem(item);
            }
        }

        /// <summary>
        /// Linear search that returns all items of particular group and template from this collection.
        /// Does not change items in this collection.
        /// For speed purposes returns actual item reference not a clone.
        /// </summary>
        /// <param name="itemGroup">Item group.</param>
        /// <param name="templateIndex">Item template index. Use -1 to match group only.</param>
        /// <returns>List of item references.</returns>
        public List<DaggerfallUnityItem> SearchItems(ItemGroups itemGroup, int templateIndex = -1)
        {
            List<DaggerfallUnityItem> results = new List<DaggerfallUnityItem>();
            foreach (DaggerfallUnityItem item in items.Values)
            {
                if (templateIndex == -1)
                {
                    if (item.GroupIndex == (int)itemGroup)
                    {
                        results.Add(item);
                    }
                }
                else
                {
                    if (item.IsOfTemplate(itemGroup, templateIndex))
                    {
                        results.Add(item);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Removes any orphaned items from this collection.
        /// </summary>
        public int RemoveOrphanedItems()
        {
            // Schedule removal if item relates to a null or tombstoned quest, or has an invalid template
            List<DaggerfallUnityItem> itemsToRemove = new List<DaggerfallUnityItem>();
            foreach (DaggerfallUnityItem item in items.Values)
            {
                if (item.IsQuestItem)
                {
                    Quest quest = GameManager.Instance.QuestMachine.GetQuest(item.QuestUID);
                    if (quest == null)
                        itemsToRemove.Add(item);
                    else if (quest.QuestTombstoned)
                        itemsToRemove.Add(item);
                }
                else if (string.IsNullOrEmpty(item.shortName))
                {
                    itemsToRemove.Add(item);
                }
            }

            // Remove scheduled items
            foreach (DaggerfallUnityItem item in itemsToRemove)
            {
                RemoveItem(item);
            }

            return itemsToRemove.Count;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds existing stack of item.
        /// </summary>
        /// <param name="item">Item to find existing stack for.</param>
        /// <returns>Existing item stack, or null if no stack or not stackable.</returns>
        DaggerfallUnityItem FindExistingStack(DaggerfallUnityItem item)
        {
            if (!item.IsStackable())
                return null;

            ItemGroups itemGroup = item.ItemGroup;
            int groupIndex = item.GroupIndex;
            foreach (DaggerfallUnityItem checkItem in items.Values)
            {
                if (checkItem != item && 
                    checkItem.ItemGroup == itemGroup && checkItem.GroupIndex == groupIndex &&
                    checkItem.PotionRecipeKey == item.PotionRecipeKey &&
                    checkItem.IsStackable())
                    return checkItem;
            }

            return null;
        }

        #endregion
    }
}
