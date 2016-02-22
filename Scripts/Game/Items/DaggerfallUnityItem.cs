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
using DaggerfallConnect.FallExe;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// Parent class for any individual item in Daggerfall Unity.
    /// Will retain some data from classic items to assist with save import.
    /// </summary>
    public class DaggerfallUnityItem
    {
        #region Fields

        // Interim native item data from save game import
        ItemRecord itemRecord = new ItemRecord();
        ItemTemplate itemTemplate = new ItemTemplate();

        // New item data - to be expanded
        string name;
        EquipSlots equipSlot = EquipSlots.None;
        int playerTextureArchive;
        int playerTextureRecord;
        int drawOrder;

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public ItemRecord ItemRecord
        {
            get { return itemRecord; }
            set { SetItemRecord(value); }
        }

        public ItemGroups ItemGroup
        {
            get { return GetItemGroup(); }
        }

        public int ItemIndex
        {
            get { return GetItemIndex(); }
        }

        public ItemTemplate ItemTemplate
        {
            get { return itemTemplate; }
        }

        public int TemplateIndex
        {
            get { return itemTemplate.index; }
        }

        public EquipSlots EquipSlot
        {
            get { return equipSlot; }
            set { equipSlot = value; }
        }

        public int PlayerTextureArchive
        {
            get { return playerTextureArchive; }
            set { playerTextureArchive = value; }
        }

        public int PlayerTextureRecord
        {
            get { return playerTextureRecord; }
            set { playerTextureRecord = value; }
        }

        public int DrawOrder
        {
            get { return drawOrder; }
            set { drawOrder = value; }
        }

        #endregion

        #region Constructors

        public DaggerfallUnityItem()
        {
        }

        public DaggerfallUnityItem(ItemRecord record)
        {
            SetItemRecord(record);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets native save ItemRecord.
        /// This is an interim way to store item data while DaggerfallUnityItem class under development.
        /// </summary>
        /// <param name="SetItemRecord">ItemRecord.</param>
        public void SetItemRecord(ItemRecord itemRecord)
        {
            // Assign interim data
            this.itemRecord = itemRecord;
            itemTemplate = DaggerfallUnity.Instance.ItemHelper.GetItemTemplate(this);

            // Promote leather helms to chain
            // Daggerfall seems to do this also as "leather" helms have the chain tint in-game
            // Is this why Daggerfall intentionally leaves off the material type from helms and shields?
            // Might need to revisit this later
            ItemGroups group = (ItemGroups)itemRecord.ParsedData.category1;
            if (group == ItemGroups.Armor)
            {
                // Check if helm
                if (TemplateIndex == (int)Armor.Helm)
                {
                    // Change material to chain and leave other stats the same
                    ItemRecord.ItemRecordData parsedData = this.itemRecord.ParsedData;
                    parsedData.material = (ushort)ArmorMaterialTypes.Chain;
                    this.itemRecord.ParsedData = parsedData;
                }
            }

            // Get player image
            int bitfield = (int)itemRecord.ParsedData.image1;
            int archive = bitfield >> 7;
            int record = (bitfield & 0x7f);

            // Assign new data
            name = DaggerfallUnity.Instance.ItemHelper.ResolveItemName(this);
            drawOrder = itemTemplate.drawOrderOrEffect;
            playerTextureArchive = archive;
            playerTextureRecord = record;   
        }

        /// <summary>
        /// Creates a new copy of item data.
        /// </summary>
        /// <returns>New DaggerfallUnityItem.</returns>
        public DaggerfallUnityItem Copy()
        {
            // Create new item
            DaggerfallUnityItem newItem = new DaggerfallUnityItem();

            // Copy local data
            newItem.name = this.name;

            // Copy record data
            itemRecord.CopyTo(newItem.itemRecord);

            return newItem;
        }

        /// <summary>
        /// Checks if item matches specified template group and index.
        /// </summary>
        /// <param name="group">Item group.</param>
        /// <param name="templateIndex">Template index.</param>
        /// <returns>True if item matches type.</returns>
        public bool IsOfTemplate(ItemGroups group, int templateIndex)
        {
            if (ItemGroup == group && TemplateIndex == templateIndex)
                return true;
            else
                return false;
        }

        #endregion

        #region Private Methods

        ItemGroups GetItemGroup()
        {
            return (ItemGroups)itemRecord.ParsedData.category1;
        }

        int GetItemIndex()
        {
            return itemRecord.ParsedData.category2;
        }

        int GetTemplateIndex()
        {
            return itemTemplate.index;
        }

        #endregion
    }
}