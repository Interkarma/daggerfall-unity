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

        // Private item fields
        int playerTextureArchive;
        int playerTextureRecord;
        ItemGroups itemGroup;
        int groupIndex;
        int currentVariant = 0;

        // Item template is cached for faster checks
        // Does not need to be serialized
        ItemTemplate cachedItemTemplate;
        ItemGroups cachedItemGroup = ItemGroups.None;
        int cachedGroupIndex = -1;

        // References current slot if equipped
        EquipSlots equipSlot = EquipSlots.None;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets texture archive.
        /// Should be set by template and body morphology.
        /// Or directly as imported from classic save.
        /// </summary>
        public int PlayerTextureArchive
        {
            get { return playerTextureArchive; }
            set { playerTextureArchive = value; }
        }

        /// <summary>
        /// Gets texture record based on variant.
        /// </summary>
        public int PlayerTextureRecord
        {
            get { return GetPlayerTextureRecord(); }
        }

        /// <summary>
        /// Gets or sets item group.
        /// Setting will reset item data from new template.
        /// </summary>
        public ItemGroups ItemGroup
        {
            get { return itemGroup; }
            set { SetItem(value, groupIndex); }
        }

        /// <summary>
        /// Gets or sets item group index.
        /// Setting will reset item data from new template.
        /// </summary>
        public int GroupIndex
        {
            get { return groupIndex; }
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

        /// <summary>
        /// Gets total variants of this item.
        /// </summary>
        public int TotalVariants
        {
            get { return ItemTemplate.variants; }
        }

        /// <summary>
        /// Gets current variant of this item.
        /// </summary>
        public int CurrentVariant
        {
            get { return currentVariant; }
            set { SetCurrentVariant(value); }
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
        public DaggerfallUnityItem(ItemGroups itemGroup, int groupIndex)
        {
            SetItem(itemGroup, groupIndex);
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
        /// <param name="itemGroup">Item group.</param>
        /// <param name="groupIndex">Item group index.</param>
        public void SetItem(ItemGroups itemGroup, int groupIndex)
        {
            // Get template data
            ItemTemplate itemTemplate = DaggerfallUnity.Instance.ItemHelper.GetItemTemplate(itemGroup, groupIndex);

            // Assign new data
            shortName = itemTemplate.name;
            this.itemGroup = itemGroup;
            this.groupIndex = groupIndex;
            playerTextureArchive = itemTemplate.playerTextureArchive;
            playerTextureRecord = itemTemplate.playerTextureRecord;
            nativeMaterialValue = 0;
            dyeColor = DyeColors.Unchanged;
            weightInKg = itemTemplate.baseWeight;
            drawOrder = itemTemplate.drawOrderOrEffect;
            currentVariant = 0;

            // Fix leather helms
            ItemBuilder.FixLeatherHelm(this);
        }

        public bool IsOfTemplate(ItemGroups itemGroup, int templateIndex)
        {
            if (ItemGroup == itemGroup && TemplateIndex == templateIndex)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Cycles through variants.
        /// </summary>
        public void NextVariant()
        {
            // Cycle through variants
            int variant = CurrentVariant + 1;
            if (variant >= TotalVariants)
                variant = 0;

            // Update image
            playerTextureRecord = ItemTemplate.playerTextureRecord + variant;
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
            groupIndex = other.groupIndex;
            playerTextureArchive = other.playerTextureArchive;
            playerTextureRecord = other.playerTextureRecord;
            nativeMaterialValue = other.nativeMaterialValue;
            dyeColor = other.dyeColor;
            weightInKg = other.weightInKg;
            drawOrder = other.drawOrder;
            currentVariant = other.currentVariant;

            // Fix leather helms
            ItemBuilder.FixLeatherHelm(this);
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
            groupIndex = index;
            playerTextureArchive = archive;
            playerTextureRecord = record;
            nativeMaterialValue = itemRecord.ParsedData.material;
            dyeColor = (DyeColors)itemRecord.ParsedData.color;
            weightInKg = (float)itemRecord.ParsedData.weight * 0.25f;
            drawOrder = itemTemplate.drawOrderOrEffect;
            currentVariant = record - itemTemplate.playerTextureRecord;

            // Fix leather helms
            ItemBuilder.FixLeatherHelm(this);
        }

        /// <summary>
        /// Caches item template.
        /// </summary>
        ItemTemplate GetCachedItemTemplate()
        {
            if (itemGroup != cachedItemGroup || groupIndex != cachedGroupIndex)
            {
                cachedItemTemplate = DaggerfallUnity.Instance.ItemHelper.GetItemTemplate(itemGroup, groupIndex);
                cachedItemGroup = itemGroup;
                cachedGroupIndex = groupIndex;
            }

            return cachedItemTemplate;
        }

        /// <summary>
        /// Gets player texture record based on variant and other properties.
        /// </summary>
        int GetPlayerTextureRecord()
        {
            // Get starting record from template
            int start = ItemTemplate.playerTextureRecord;

            // Cloaks have a special interior image, need to increment for first actual cloak image
            if (IsCloak())
                start += 1;

            return start + currentVariant;
        }

        bool IsCloak()
        {
            // Mens cloaks
            if (ItemGroup == ItemGroups.MensClothing)
            {
                if (TemplateIndex == (int)MensClothing.Casual_cloak ||
                    TemplateIndex == (int)MensClothing.Formal_cloak)
                {
                    return true;
                }
            }

            // Womens cloaks
            if (ItemGroup == ItemGroups.WomensClothing)
            {
                if (TemplateIndex == (int)WomensClothing.Casual_cloak ||
                    TemplateIndex == (int)WomensClothing.Formal_cloak)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sets current variant and clamps within valid range.
        /// </summary>
        void SetCurrentVariant(int variant)
        {
            if (variant < 0)
                currentVariant = 0;
            else if (variant >= TotalVariants)
                currentVariant = TotalVariants - 1;
            else
                currentVariant = variant;
        }

        #endregion
    }
}