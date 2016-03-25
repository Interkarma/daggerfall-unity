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
    /// </summary>
    public class DaggerfallUnityItem
    {
        #region Fields

        // Public item values
        public string shortName;
        public int playerTextureArchive;
        public int playerTextureRecord;
        public int nativeMaterialValue;
        public DyeColors dyeColor;
        public float weightInKg;
        public int drawOrder;
        //public UInt32 value1;             // TODO: Not currently imported from template / native record
        //public UInt32 value2;
        //public UInt16 hits1;
        //public UInt16 hits2;
        //public UInt16 hits3;
        //int enchantmentPoints;
        //int message;
        //int[] magic;

        // Private item values
        ItemGroups itemGroup;
        int itemIndex;

        // Item template is cached for faster checks
        // Does not need to be serialized
        [NonSerialized] ItemTemplate cachedItemTemplate;
        [NonSerialized] ItemGroups cachedItemGroup = ItemGroups.None;
        [NonSerialized] int cachedItemIndex = -1;

        // Temp
        EquipSlots equipSlot = EquipSlots.None;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets item group.
        /// Setting will reset item data from new template.
        /// </summary>
        public ItemGroups ItemGroup
        {
            get { return itemGroup; }
            set { SetItem(value, itemIndex); }
        }

        /// <summary>
        /// Gets or sets item index.
        /// Setting will reset item data from new template.
        /// </summary>
        public int ItemIndex
        {
            get { return itemIndex; }
            set { SetItem(itemGroup, value); }
        }

        /// <summary>
        /// Get this item's template data.
        /// </summary>
        public ItemTemplate ItemTemplate
        {
            get { return GetCachedItemTemplate(); }
        }

        /// <summary>
        /// Get this item's template index.
        /// </summary>
        public int TemplateIndex
        {
            get { return ItemTemplate.index; }
        }

        /// <summary>
        /// Resolve this item's full name.
        /// </summary>
        public string LongName
        {
            get { return DaggerfallUnity.Instance.ItemHelper.ResolveItemName(this); }
        }

        /// <summary>
        /// Gets temp equip slot.
        /// </summary>
        public EquipSlots EquipSlot
        {
            get { return equipSlot; }
            set { equipSlot = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DaggerfallUnityItem()
        {
        }

        /// <summary>
        /// Construct from item group and index.
        /// </summary>
        public DaggerfallUnityItem(ItemGroups itemGroup, int itemIndex)
        {
            SetItem(itemGroup, itemIndex);
        }

        /// <summary>
        /// Construct from another item.
        /// </summary>
        public DaggerfallUnityItem(DaggerfallUnityItem item)
        {
            FromItem(item);
        }

        /// <summary>
        /// Construct from legacy ItemRecord data.
        /// </summary>
        public DaggerfallUnityItem(ItemRecord record)
        {
            FromItemRecord(record);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if item matches specified template group and index.
        /// </summary>
        /// <param name="group">Item group.</param>
        /// <param name="templateIndex">Template index.</param>
        /// <returns>True if item matches type.</returns>
        public bool IsOfTemplate(ItemGroups group, int templateIndex)
        {
            if (itemGroup == group && TemplateIndex == templateIndex)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Creates a new copy of this item.
        /// </summary>
        /// <returns></returns>
        public DaggerfallUnityItem Clone()
        {
            return new DaggerfallUnityItem(this);
        }

        /// <summary>
        /// Sets item from group and index.
        /// Resets item data from new template.
        /// </summary>
        /// <param name="group">Item group.</param>
        /// <param name="index">Item index.</param>
        public void SetItem(ItemGroups group, int index)
        {
            // Get template data
            ItemTemplate itemTemplate = DaggerfallUnity.Instance.ItemHelper.GetItemTemplate(group, index);

            // Assign new data
            shortName = itemTemplate.name;
            itemGroup = group;
            itemIndex = index;
            playerTextureArchive = itemTemplate.playerTextureArchive;
            playerTextureRecord = itemTemplate.playerTextureRecord;
            nativeMaterialValue = 0;
            dyeColor = DyeColors.Unchanged;
            weightInKg = itemTemplate.baseWeight;
            drawOrder = itemTemplate.drawOrderOrEffect;

            // Fix leather helms
            FixHelm();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates from another item instance.
        /// </summary>
        void FromItem(DaggerfallUnityItem other)
        {
            shortName = other.shortName;
            itemGroup = other.itemGroup;
            itemIndex = other.itemIndex;
            playerTextureArchive = other.playerTextureArchive;
            playerTextureRecord = other.playerTextureRecord;
            nativeMaterialValue = other.nativeMaterialValue;
            dyeColor = other.dyeColor;
            weightInKg = other.weightInKg;
            drawOrder = other.drawOrder;

            // Fix leather helms
            FixHelm();
        }

        /// <summary>
        /// Create from native save ItemRecord data.
        /// </summary>
        void FromItemRecord(ItemRecord itemRecord)
        {
            // Get template data
            ItemGroups group = (ItemGroups)itemRecord.ParsedData.category1;
            int index = itemRecord.ParsedData.category2;
            ItemTemplate itemTemplate = DaggerfallUnity.Instance.ItemHelper.GetItemTemplate(group, index);

            // Get player image
            int bitfield = (int)itemRecord.ParsedData.image1;
            int archive = bitfield >> 7;
            int record = (bitfield & 0x7f);

            // Assign new data
            shortName = itemRecord.ParsedData.name;
            itemGroup = group;
            itemIndex = index;
            playerTextureArchive = archive;
            playerTextureRecord = record;
            nativeMaterialValue = itemRecord.ParsedData.material;
            dyeColor = (DyeColors)itemRecord.ParsedData.color;
            weightInKg = (float)itemRecord.ParsedData.weight * 0.25f;
            drawOrder = itemTemplate.drawOrderOrEffect;

            // Fix leather helms
            FixHelm();
        }

        ItemTemplate GetCachedItemTemplate()
        {
            if (itemGroup != cachedItemGroup || itemIndex != cachedItemIndex)
            {
                cachedItemTemplate = DaggerfallUnity.Instance.ItemHelper.GetItemTemplate(itemGroup, itemIndex);
                cachedItemGroup = itemGroup;
                cachedItemIndex = itemIndex;
            }

            return cachedItemTemplate;
        }

        void FixHelm()
        {
            // Promote leather helms to chain
            // Daggerfall seems to do this also as "leather" helms have the chain tint in-game
            // Is this why Daggerfall intentionally leaves off the material type from helms and shields?
            // Might need to revisit this later
            if (IsOfTemplate(ItemGroups.Armor, (int)Armor.Helm))
            {
                // Check if leather
                if ((ArmorMaterialTypes)nativeMaterialValue == ArmorMaterialTypes.Leather)
                {
                    // Change material to chain and leave other stats the same
                    nativeMaterialValue = (int)ArmorMaterialTypes.Chain;

                    // Change dye to chain if not set
                    if (dyeColor == DyeColors.Unchanged)
                        dyeColor = DyeColors.Chain;
                }
            }
        }

        #endregion
    }
}