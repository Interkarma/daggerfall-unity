// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.Entity
{
    /// <summary>
    /// Collection of items held by an Entity.
    /// Items are stored in an ordered dictionary keyed to a unique identifier.
    /// This collection may represent an inventory, a loot pile, harvestable items, etc.
    /// This class is under active development and may change several times before completed.
    /// </summary>
    [Serializable]
    public class EntityItems
    {
        #region Fields

        Dictionary<ulong, DaggerfallUnityItem> items = new Dictionary<ulong, DaggerfallUnityItem>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets count of items in collection.
        /// </summary>
        public int Count
        {
            get { return items.Count; }
        }

        /// <summary>
        /// Gets the live items collection.
        /// </summary>
        public Dictionary<ulong, DaggerfallUnityItem> Items
        {
            get { return items; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a new item to this collection.
        /// </summary>
        /// <param name="">DaggerfallUnityItem to add.</param>
        public void AddItem(DaggerfallUnityItem item)
        {
            if (item != null)
            {
                // Add the item based on stack behaviour
                // Stacking currently only works for ingredients
                // TODO: Look at implementing proper stacking with max limits, etc.
                DaggerfallUnityItem ingredient = FindIngredient(item);
                if (ingredient != null)
                {
                    // Add to stack count
                    ingredient.stackCount++;
                }
                else
                {
                    // Check duplicate key
                    if (items.ContainsKey(item.UID))
                        throw new Exception("AddItem() encountered a duplicate item UID for " + item.LongName);

                    // Add the item
                    items.Add(item.UID, item);
                }
            }
        }

        /// <summary>
        /// Clears all items from this collection.
        /// Items in this collection will be destroyed.
        /// </summary>
        public void RemoveAll()
        {
            items.Clear();
        }

        /// <summary>
        /// Replaces all items in this collection with clones of items from another collection.
        /// Items in this collection will be destroyed. Source items will not be changed.
        /// New UIDs will be allocated to new items.
        /// </summary>
        /// <param name="other">Source of items to copy from.</param>
        public void ReplaceAll(EntityItems other)
        {
            RemoveAll();
            CopyAll(other);
        }

        /// <summary>
        /// Add to this collection with clones of items from another collection.
        /// Source items will not be changed.
        /// New UIDs will be allocated to new items.
        /// </summary>
        /// <param name="other">Source of items to copy from.</param>
        public void CopyAll(EntityItems other)
        {
            foreach(var item in other.items)
            {
                AddItem(item.Value.Clone());
            }
        }

        /// <summary>
        /// Transfers all items from another collection.
        /// Source items will be removed from other collection and placed in this collection.
        /// UIDs will be retained.
        /// </summary>
        /// <param name="other">Source of items to transfer from.</param>
        public void TransferAll(EntityItems other)
        {
            foreach (var item in other.items)
            {
                AddItem(item.Value);
            }

            other.RemoveAll();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds a specific ingredient in list of existing items.
        /// Used to stack ingredients when added.
        /// </summary>
        DaggerfallUnityItem FindIngredient(DaggerfallUnityItem ingredient)
        {
            if (!ingredient.IsIngredient)
                return null;

            ItemGroups itemGroup = ingredient.ItemGroup;
            int groupIndex = ingredient.GroupIndex;
            foreach (var item in items)
            {
                if (item.Value.ItemGroup == itemGroup && item.Value.GroupIndex == groupIndex)
                    return item.Value;
            }

            return null;
        }

        #endregion
    }
}