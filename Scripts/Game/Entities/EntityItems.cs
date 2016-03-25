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
    /// This may be an inventory, loot, equipped items, harvestable items, etc.
    /// This class is under active development and may change several times before completed.
    /// </summary>
    [Serializable]
    public class EntityItems : IEnumerable
    {
        #region Fields

        List<DaggerfallUnityItem> items = new List<DaggerfallUnityItem>();
        uint[] legacyEquipTable;

        #endregion

        #region Properties

        /// <summary>
        /// Gets legacy equip table previous set by SetLegacyEquipTable().
        /// </summary>
        public uint[] LegacyEquipTable
        {
            get { return legacyEquipTable; }
        }

        #endregion

        #region Structures
        #endregion

        #region Enums
        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a new item to this collection.
        /// </summary>
        /// <param name="">DaggerfallUnityItem to add.</param>
        public void AddItem(DaggerfallUnityItem item)
        {
            items.Add(item);
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
        /// Replaces all items in this collection with items from another collection.
        /// Items in this collection will be destroyed. Source items will not be changed.
        /// </summary>
        /// <param name="other">Source of items to copy from.</param>
        public void ReplaceAll(EntityItems other)
        {
            RemoveAll();
            CopyAll(other);
        }

        /// <summary>
        /// Copies all items from another collection.
        /// Source items will not be changed.
        /// </summary>
        /// <param name="other">Source of items to copy from.</param>
        public void CopyAll(EntityItems other)
        {
            for (int i = 0; i < other.items.Count; i++)
            {
                items.Add(other.items[i].Clone());
            }
        }

        /// <summary>
        /// Transfers all items from another collection.
        /// Source items will be removed from other collection and placed in this collection.
        /// </summary>
        /// <param name="other">Source of items to transfer from.</param>
        public void TransferAll(EntityItems other)
        {
            CopyAll(other);
            other.RemoveAll();
        }

        /// <summary>
        /// Gets a copy of legacy equip table from a CharacterRecord.
        /// </summary>
        /// <param name="characterRecord"></param>
        public void SetLegacyEquipTable(CharacterRecord characterRecord)
        {
            legacyEquipTable = (uint[])characterRecord.ParsedData.equippedItems.Clone();
        }

        #endregion

        #region Private Methods
        #endregion

        #region IEnumerable

        public IEnumerator GetEnumerator()
        {
            foreach (object o in items)
            {
                yield return o;
            }
        }

        #endregion
    }
}