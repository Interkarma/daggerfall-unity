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

using DaggerfallConnect.Save;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Serialization;

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
        public int value1;
        public int value2;
        public int hits1;
        public int hits2;
        public int hits3;
        public int stackCount = 1;
        public int enchantmentPoints;
        public int message;
        public int[] legacyMagic = null;

        // Private item fields
        int playerTextureArchive;
        int playerTextureRecord;
        int worldTextureArchive;
        int worldTextureRecord;
        ItemGroups itemGroup;
        int groupIndex;
        int currentVariant = 0;
        ulong uid;

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
        /// Gets generated unique identifier.
        /// </summary>
        public ulong UID
        {
            get { return uid; }
        }

        /// <summary>
        /// Gets or sets player texture archive.
        /// Used during item creation to set correct body morphology.
        /// </summary>
        public int PlayerTextureArchive
        {
            get { return playerTextureArchive; }
            set { playerTextureArchive = value; }
        }

        /// <summary>
        /// Gets inventory texture archive.
        /// </summary>
        public int InventoryTextureArchive
        {
            get { return GetInventoryTextureArchive(); }
        }

        /// <summary>
        /// Gets inventory texture record.
        /// </summary>
        public int InventoryTextureRecord
        {
            get { return GetInventoryTextureRecord(); }
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

        /// <summary>
        /// Checks if this is an ingredient.
        /// </summary>
        public bool IsIngredient
        {
            get { return ItemTemplate.isIngredient; }
        }

        /// <summary>
        /// Checks if this item is equipped.
        /// </summary>
        public bool IsEquipped
        {
            get { return !(equipSlot == EquipSlots.None); }
        }

        /// <summary>
        /// Checks if this item has magical properties.
        /// </summary>
        public bool IsEnchanted
        {
            get { return GetIsEnchanted(); }
        }

        /// <summary>
        /// Checks if this item is of any shield type.
        /// </summary>
        public bool IsShield
        {
            get { return GetIsShield(); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// Generates new UID.
        /// </summary>
        public DaggerfallUnityItem()
        {
            uid = DaggerfallUnity.NextUID;
        }

        /// <summary>
        /// Construct from item group and index.
        /// Generates new UID.
        /// </summary>
        public DaggerfallUnityItem(ItemGroups itemGroup, int groupIndex)
        {
            uid = DaggerfallUnity.NextUID;
            SetItem(itemGroup, groupIndex);
        }

        /// <summary>
        /// Construct from another item.
        /// Generates new UID.
        /// </summary>
        public DaggerfallUnityItem(DaggerfallUnityItem item)
        {
            uid = DaggerfallUnity.NextUID;
            FromItem(item);
        }

        /// <summary>
        /// Construct from legacy ItemRecord data.
        /// Generates new UID.
        /// </summary>
        public DaggerfallUnityItem(ItemRecord record)
        {
            uid = DaggerfallUnity.NextUID;
            FromItemRecord(record);
        }

        /// <summary>
        /// Construct item from serialized data.
        /// Used serialized UID.
        /// </summary>
        /// <param name="itemData">Item data to restore.</param>
        public DaggerfallUnityItem(ItemData_v1 itemData)
        {
            FromItemData(itemData);
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
        /// Retains existing UID.
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
            worldTextureArchive = itemTemplate.worldTextureArchive;
            worldTextureRecord = itemTemplate.worldTextureRecord;
            nativeMaterialValue = 0;
            dyeColor = DyeColors.Unchanged;
            weightInKg = itemTemplate.baseWeight;
            drawOrder = itemTemplate.drawOrderOrEffect;
            currentVariant = 0;
            value1 = itemTemplate.basePrice;
            value2 = itemTemplate.basePrice;
            hits1 = itemTemplate.hitPoints;
            hits2 = itemTemplate.hitPoints;
            hits3 = itemTemplate.hitPoints;
            enchantmentPoints = itemTemplate.enchantmentPoints;
            message = 0;
            stackCount = 1;

            // Fix leather helms
            ItemBuilder.FixLeatherHelm(this);
        }

        /// <summary>
        /// Checks if item is of both group and template index.
        /// </summary>
        /// <param name="itemGroup">Item group to check.</param>
        /// <param name="templateIndex">Template index to check.</param>
        /// <returns>True if item matches both group and template index.</returns>
        public bool IsOfTemplate(ItemGroups itemGroup, int templateIndex)
        {
            if (ItemGroup == itemGroup && TemplateIndex == templateIndex)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if item is of template index.
        /// </summary>
        /// <param name="templateIndex">Template index to check.</param>
        /// <returns>True if item matches template index.</returns>
        public bool IsOfTemplate(int templateIndex)
        {
            return (TemplateIndex == templateIndex);
        }

        /// <summary>
        /// Cycles through variants.
        /// </summary>
        public void NextVariant()
        {
            // Only certain items support user-initiated variants
            bool canChangeVariant = false;
            switch(TemplateIndex)
            {        
                case (int)MensClothing.Casual_cloak:
                case (int)MensClothing.Formal_cloak:
                case (int)MensClothing.Reversible_tunic:
                case (int)MensClothing.Plain_robes:
                case (int)MensClothing.Short_shirt:
                case (int)MensClothing.Short_shirt_with_belt:
                case (int)MensClothing.Long_shirt:
                case (int)MensClothing.Long_shirt_with_belt:
                case (int)MensClothing.Short_shirt_closed_top:
                case (int)MensClothing.Short_shirt_closed_top2:
                case (int)MensClothing.Long_shirt_closed_top:
                case (int)MensClothing.Long_shirt_closed_top2:
                case (int)WomensClothing.Casual_cloak:
                case (int)WomensClothing.Formal_cloak:
                case (int)WomensClothing.Strapless_dress:
                case (int)WomensClothing.Plain_robes:
                case (int)WomensClothing.Short_shirt:
                case (int)WomensClothing.Short_shirt_belt:
                case (int)WomensClothing.Long_shirt:
                case (int)WomensClothing.Long_shirt_belt:
                case (int)WomensClothing.Short_shirt_closed:
                case (int)WomensClothing.Short_shirt_closed_belt:
                case (int)WomensClothing.Long_shirt_closed:
                case (int)WomensClothing.Long_shirt_closed_belt:               
                    canChangeVariant = true;
                    break;
            }

            // Cycle through variants
            if (canChangeVariant)
            {
                int variant = CurrentVariant + 1;
                if (variant >= TotalVariants)
                    variant = 0;

                currentVariant = variant;
            }
        }

        /// <summary>
        /// Gets item data for serialization.
        /// </summary>
        /// <returns>ItemData_v1.</returns>
        public ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = new ItemData_v1();
            data.uid = uid;
            data.shortName = shortName;
            data.nativeMaterialValue = nativeMaterialValue;
            data.dyeColor = dyeColor;
            data.weightInKg = weightInKg;
            data.drawOrder = drawOrder;
            data.value1 = value1;
            data.value2 = value2;
            data.hits1 = hits1;
            data.hits2 = hits2;
            data.hits3 = hits3;
            data.stackCount = stackCount;
            data.enchantmentPoints = enchantmentPoints;
            data.message = message;
            data.legacyMagic = legacyMagic;
            data.playerTextureArchive = playerTextureArchive;
            data.playerTextureRecord = playerTextureRecord;
            data.worldTextureArchive = worldTextureArchive;
            data.worldTextureRecord = worldTextureRecord;
            data.itemGroup = itemGroup;
            data.groupIndex = groupIndex;
            data.currentVariant = currentVariant;

            return data;
        }

        public SoundClips GetEquipSound()
        {
            switch (itemGroup)
            {
                case ItemGroups.MensClothing:
                case ItemGroups.WomensClothing:
                    return SoundClips.EquipClothing;
                case ItemGroups.Jewellery:
                case ItemGroups.Gems:
                    return SoundClips.EquipJewellery;
                case ItemGroups.Armor:
                    if (GetIsShield() || TemplateIndex == (int)Armor.Helm)
                        return SoundClips.EquipPlate;
                    else if (nativeMaterialValue == (int)ArmorMaterialTypes.Leather)
                        return SoundClips.EquipLeather;
                    else if (nativeMaterialValue == (int)ArmorMaterialTypes.Chain)
                        return SoundClips.EquipChain;
                    else
                        return SoundClips.EquipPlate;
                case ItemGroups.Weapons:
                    switch (TemplateIndex)
                    {
                        case (int)Weapons.Battle_Axe:
                        case (int)Weapons.War_Axe:
                            return SoundClips.EquipAxe;
                        case (int)Weapons.Broadsword:
                        case (int)Weapons.Longsword:
                        case (int)Weapons.Saber:
                        case (int)Weapons.Katana:
                            return SoundClips.EquipLongBlade;
                        case (int)Weapons.Claymore:
                        case (int)Weapons.Dai_Katana:
                            return SoundClips.EquipTwoHandedBlade;
                        case (int)Weapons.Dagger:
                        case (int)Weapons.Tanto:
                        case (int)Weapons.Wakazashi:
                        case (int)Weapons.Shortsword:
                            return SoundClips.EquipShortBlade;
                        case (int)Weapons.Flail:
                            return SoundClips.EquipFlail;
                        case (int)Weapons.Mace:
                        case (int)Weapons.Warhammer:
                            return SoundClips.EquipMaceOrHammer;
                        case (int)Weapons.Staff:
                            return SoundClips.EquipStaff;
                        case (int)Weapons.Short_Bow:
                        case (int)Weapons.Long_Bow:
                            return SoundClips.EquipBow;

                        default:
                            return SoundClips.None;
                    }

                default:
                    return SoundClips.None;
            }
        }

        public SoundClips GetSwingSound()
        {
            switch (TemplateIndex)
            {
                case (int)Weapons.Warhammer:
                case (int)Weapons.Battle_Axe:
                case (int)Weapons.Katana:
                case (int)Weapons.Claymore:
                case (int)Weapons.Dai_Katana:
                case (int)Weapons.Flail:
                    return SoundClips.SwingLowPitch;
                case (int)Weapons.Broadsword:
                case (int)Weapons.Longsword:
                case (int)Weapons.Saber:
                case (int)Weapons.Wakazashi:
                case (int)Weapons.War_Axe:
                case (int)Weapons.Staff:
                    return SoundClips.SwingMediumPitch;
                case (int)Weapons.Dagger:
                case (int)Weapons.Tanto:                
                case (int)Weapons.Shortsword:
                    return SoundClips.SwingHighPitch;

                default:
                    return SoundClips.None;
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Compares two items for equal UID.
        /// One or both items can be null.
        /// </summary>
        /// <param name="item1">First item.</param>
        /// <param name="item2">Second item.</param>
        /// <returns>True if items have same UID.</returns>
        public static bool CompareItems(DaggerfallUnityItem item1, DaggerfallUnityItem item2)
        {
            // Exclude null cases
            if (item1 == null && item2 == null)
                return true;
            else if (item1 == null || item2 == null)
                return false;

            // Compare UIDs
            if (item1.UID == item2.UID)
                return true;
            else
                return false;
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
            worldTextureArchive = other.worldTextureArchive;
            worldTextureRecord = other.worldTextureRecord;
            nativeMaterialValue = other.nativeMaterialValue;
            dyeColor = other.dyeColor;
            weightInKg = other.weightInKg;
            drawOrder = other.drawOrder;
            currentVariant = other.currentVariant;
            value1 = other.value1;
            value2 = other.value2;
            hits1 = other.hits1;
            hits2 = other.hits2;
            hits3 = other.hits3;
            enchantmentPoints = other.enchantmentPoints;
            message = other.message;
            stackCount = other.stackCount;

            if (other.legacyMagic != null)
                legacyMagic = (int[])other.legacyMagic.Clone();
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
            int playerBitfield = (int)itemRecord.ParsedData.image1;
            int playerArchive = playerBitfield >> 7;
            int playerRecord = (playerBitfield & 0x7f);

            // Get world image
            int worldBitfield = (int)itemRecord.ParsedData.image2;
            int worldArchive = worldBitfield >> 7;
            int worldRecord = (worldBitfield & 0x7f);

            // Assign new data
            shortName = itemRecord.ParsedData.name;
            itemGroup = group;
            groupIndex = index;
            playerTextureArchive = playerArchive;
            playerTextureRecord = playerRecord;
            worldTextureArchive = worldArchive;
            worldTextureRecord = worldRecord;
            nativeMaterialValue = itemRecord.ParsedData.material;
            dyeColor = (DyeColors)itemRecord.ParsedData.color;
            weightInKg = (float)itemRecord.ParsedData.weight * 0.25f;
            drawOrder = itemTemplate.drawOrderOrEffect;
            value1 = (int)itemRecord.ParsedData.value1;
            value2 = (int)itemRecord.ParsedData.value2;
            hits1 = itemRecord.ParsedData.hits1;
            hits2 = itemRecord.ParsedData.hits2;
            hits3 = itemRecord.ParsedData.hits3;
            currentVariant = 0;
            enchantmentPoints = itemRecord.ParsedData.enchantmentPoints;
            message = (int)itemRecord.ParsedData.message;
            stackCount = 1;

            // Assign current variant
            if (itemTemplate.variants > 0)
            {
                if (IsCloak())
                    currentVariant = playerRecord - (itemTemplate.playerTextureRecord + 1);
                else
                    currentVariant = playerRecord - itemTemplate.playerTextureRecord;
            }

            // Assign legacy magic effects array
            bool foundEnchantment = false;
            if (itemRecord.ParsedData.magic != null)
            {
                legacyMagic = new int[itemRecord.ParsedData.magic.Length];
                for (int i = 0; i < itemRecord.ParsedData.magic.Length; i++)
                {
                    legacyMagic[i] = itemRecord.ParsedData.magic[i];
                    if (legacyMagic[i] != 0xffff)
                        foundEnchantment = true;
                }
            }

            // Discard list if no enchantment found
            if (!foundEnchantment)
                legacyMagic = null;

            // Fix leather helms
            ItemBuilder.FixLeatherHelm(this);

            // TEST: Force dye color to match material of imported weapons & armor
            // This is to fix cases where dye colour may be set incorrectly on imported item
            dyeColor = DaggerfallUnity.Instance.ItemHelper.GetDyeColor(this);
        }

        /// <summary>
        /// Creates from a serialized item.
        /// </summary>
        void FromItemData(ItemData_v1 data)
        {
            uid = data.uid;
            shortName = data.shortName;
            nativeMaterialValue = data.nativeMaterialValue;
            dyeColor = data.dyeColor;
            weightInKg = data.weightInKg;
            drawOrder = data.drawOrder;
            value1 = data.value1;
            value2 = data.value2;
            hits1 = data.hits1;
            hits2 = data.hits2;
            hits3 = data.hits3;
            stackCount = data.stackCount;
            enchantmentPoints = data.enchantmentPoints;
            message = data.message;
            legacyMagic = data.legacyMagic;
            playerTextureArchive = data.playerTextureArchive;
            playerTextureRecord = data.playerTextureRecord;
            worldTextureArchive = data.worldTextureArchive;
            worldTextureRecord = data.worldTextureRecord;
            itemGroup = data.itemGroup;
            groupIndex = data.groupIndex;
            currentVariant = data.currentVariant;
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
        /// Gets inventory texture archive based on certain properties.
        /// </summary>
        /// <returns></returns>
        int GetInventoryTextureArchive()
        {
            // Handle world texture
            if (UseWorldTexture())
                return worldTextureArchive;

            return playerTextureArchive;
        }

        /// <summary>
        /// Gets inventory texture record based on variant and other properties.
        /// </summary>
        int GetInventoryTextureRecord()
        {
            // Handle world texture
            if (UseWorldTexture())
                return worldTextureRecord;

            // Handle items with variants
            if (ItemTemplate.variants > 0)
            {
                // Get starting record from template
                int start = ItemTemplate.playerTextureRecord;

                // Cloaks have a special interior image, need to increment for first actual cloak image
                if (IsCloak())
                    start += 1;

                return start + currentVariant;
            }

            return playerTextureRecord;
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

        // Basic check for magic items
        // Currently uses legacyMagic data for imported items
        // New items cannot currently have magical properties
        bool GetIsEnchanted()
        {
            // Spellbook considered enchanted
            if (IsOfTemplate((int)MiscItems.Spellbook))
                return true;

            if (legacyMagic == null || legacyMagic.Length == 0)
                return false;

            // Check for legacy magic effects
            // A value of 0xffff means no enchantment in that slot
            // http://www.uesp.net/wiki/Daggerfall:MAGIC.DEF_indices
            for (int i = 0;  i < legacyMagic.Length; i++)
            {
                if (legacyMagic[i] != 0xffff)
                    return true;
            }

            return false;
        }

        // Check if this is a shield
        bool GetIsShield()
        {
            if (ItemGroup == ItemGroups.Armor)
            {
                if (TemplateIndex == (int)Armor.Kite_Shield ||
                    TemplateIndex == (int)Armor.Round_Shield ||
                    TemplateIndex == (int)Armor.Tower_Shield ||
                    TemplateIndex == (int)Armor.Buckler)
                {
                    return true;
                }
            }

            return false;
        }

        // Determines if world texture should be displayed in place of player texture
        // Many inventory item types have this setup and more may need to be added
        bool UseWorldTexture()
        {
            // Handle ingredients
            if (IsIngredient)
                return true;

            // Handle unique items
            if (IsOfTemplate(ItemGroups.MiscItems, (int)MiscItems.Spellbook) ||
                IsOfTemplate(ItemGroups.Weapons, (int)Weapons.Arrow))
            {
                return true;
            }

            // Handle religious items
            if (itemGroup == ItemGroups.ReligiousItems)
                return true;

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