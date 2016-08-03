// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Specialized;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// Collection of items.
    /// Items are stored in an ordered dictionary keyed to a unique identifier.
    /// This collection may represent an inventory, a loot pile, harvestable items, etc.
    /// This class is under active development and may change several times before completed.
    /// TODO: Implement enumerator
    /// </summary>
    [Serializable]
    public class ItemCollection
    {
        #region Fields

        OrderedDictionary items = new OrderedDictionary();

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
        /// Adds an item to this collection.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="position">Position in list to insert item.</param>
        public void AddItem(DaggerfallUnityItem item, AddPosition position = AddPosition.Back)
        {
            if (item == null)
                return;

            // Add the item based on stack behaviour
            // TODO: Look at implementing proper stacking with max limits, split, merge, etc.
            DaggerfallUnityItem stack = FindExistingStack(item);
            if (stack != null)
            {
                // Add to stack count
                stack.stackCount++;
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
        /// Reorders item in collection.
        /// </summary>
        /// <param name="item">Item to reorder. Must exist inside this collection.</param>
        /// <param name="position">Position to reorder to.</param>
        public void ReorderItem(DaggerfallUnityItem item, AddPosition position)
        {
            if (!items.Contains(item.UID) || position == AddPosition.DontCare)
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
        public void Transfer(DaggerfallUnityItem item, ItemCollection source)
        {
            if (item == null || source == null)
                return;

            source.RemoveItem(item);
            AddItem(item);
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
            foreach(DaggerfallUnityItem item in items.Values)
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
            for(int i = 0; i < itemArray.Length; i++)
            {
                DaggerfallUnityItem item = new DaggerfallUnityItem(itemArray[i]);
                AddItem(item);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines if item is stackable.
        /// Currently very simple - only ingredients and gold pieces are stackable.
        /// </summary>
        /// <param name="item">Item to check if stackable.</param>
        /// <returns>True if item stackable.</returns>
        bool IsStackable(DaggerfallUnityItem item)
        {
            if (item.IsIngredient)
                return true;
            else if (item.IsOfTemplate(ItemGroups.Currency, (int)Currency.Gold_pieces))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Finds existing stack of item.
        /// </summary>
        /// <param name="item">Item to find existing stack for.</param>
        /// <returns>Existing item stack, or null if no stack or not stackable.</returns>
        DaggerfallUnityItem FindExistingStack(DaggerfallUnityItem item)
        {
            if (!IsStackable(item))
                return null;

            ItemGroups itemGroup = item.ItemGroup;
            int groupIndex = item.GroupIndex;
            foreach (DaggerfallUnityItem checkItem in items.Values)
            {
                if (checkItem.ItemGroup == itemGroup && checkItem.GroupIndex == groupIndex)
                    return checkItem;
            }

            return null;
        }

        #endregion
    }
}