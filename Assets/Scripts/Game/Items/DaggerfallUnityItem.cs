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
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Save;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Formulas;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// Parent class for any individual item in Daggerfall Unity.
    /// </summary>
    public partial class DaggerfallUnityItem
    {
        #region Fields

        // Public item values
        public string shortName;
        public int nativeMaterialValue;
        public DyeColors dyeColor;
        public float weightInKg;
        public int drawOrder;
        public int value;
        public ushort unknown;
        public ushort flags;
        public int currentCondition;
        public int maxCondition;
        public byte unknown2;
        public byte typeDependentData;
        public int enchantmentPoints;
        public int message;
        public DaggerfallEnchantment[] legacyMagic = null;
        public CustomEnchantment[] customMagic = null;
        public int stackCount = 1;
        public Poisons poisonType = Poisons.None;
        public uint timeHealthLeechLastUsed;
        public uint timeEffectsLastRerolled;

        // Private item fields
        int playerTextureArchive;
        int playerTextureRecord;
        int worldTextureArchive;
        int worldTextureRecord;
        ItemGroups itemGroup;
        int groupIndex;
        int currentVariant = 0;
        ulong uid;

        // Soul trapped
        MobileTypes trappedSoulType = MobileTypes.None;
        // Potion recipe
        int potionRecipeKey;

        // Time for magically-created item to disappear
        uint timeForItemToDisappear = 0;

        // Quest-related fields
        bool isQuestItem = false;
        ulong questUID = 0;
        Symbol questItemSymbol = null;

        // Item template is cached for faster checks
        // Does not need to be serialized
        ItemTemplate cachedItemTemplate;
        ItemGroups cachedItemGroup = ItemGroups.None;
        int cachedGroupIndex = -1;

        // References current slot if equipped
        EquipSlots equipSlot = EquipSlots.None;

        // Repair data, used when left for repair.
        ItemRepairData repairData = new ItemRepairData();

        #endregion

        #region Item flag bitmasks

        const ushort identifiedMask = 0x20;
        const ushort artifactMask = 0x800;

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
        /// Gets or sets world texture archive.
        /// This is the generic texture shown when item is used in world by itself, typically for quests.
        /// </summary>
        public int WorldTextureArchive
        {
            get { return worldTextureArchive; }
            set { worldTextureArchive = value; }
        }

        /// <summary>
        /// Gets or sets world texture record.
        /// This is the generic texture shown when item is used in world by itself, typically for quests.
        /// </summary>
        public int WorldTextureRecord
        {
            get { return worldTextureRecord; }
            set { worldTextureRecord = value; }
        }

        /// <summary>
        /// Gets inventory texture archive.
        /// </summary>
        public virtual int InventoryTextureArchive
        {
            get { return GetInventoryTextureArchive(); }
        }

        /// <summary>
        /// Gets inventory texture record.
        /// </summary>
        public virtual int InventoryTextureRecord
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
        public virtual int GroupIndex
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
        /// Resolve this item's name.
        /// </summary>
        public virtual string ItemName
        {
            get { return DaggerfallUnity.Instance.ItemHelper.ResolveItemName(this); }
        }

        /// <summary>
        /// Resolve this item's full name (mainly for tooltips).
        /// </summary>
        public virtual string LongName
        {
            get { return DaggerfallUnity.Instance.ItemHelper.ResolveItemLongName(this); }
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
        public virtual int CurrentVariant
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
            get { return equipSlot != EquipSlots.None; }
        }

        /// <summary>
        /// Checks if this item has magical properties.
        /// </summary>
        public virtual bool IsEnchanted
        {
            get { return HasLegacyEnchantments || HasCustomEnchantments; }
        }

        /// <summary>
        /// Gets legacy enchantments on this item. Can be null or empty.
        /// Legacy enchantments on items are stored and generated using the classic type/param enchantment format.
        /// </summary>
        public DaggerfallEnchantment[] LegacyEnchantments
        {
            get { return legacyMagic; }
        }

        /// <summary>
        /// True if item has any legacy enchantments.
        /// </summary>
        public bool HasLegacyEnchantments
        {
            get { return GetHasLegacyEnchantment(); }
        }

        /// <summary>
        /// Gets custom enchantments on this item. Can be null or empty.
        /// Custom enchantments on items are stored and generated using an effectKey/customParam pair.
        /// </summary>
        public CustomEnchantment[] CustomEnchantments
        {
            get { return customMagic; }
        }

        /// <summary>
        /// True is item has any custom enchantments.
        /// </summary>
        public bool HasCustomEnchantments
        {
            get { return customMagic != null && customMagic.Length > 0; }
        }

        /// <summary>
        /// Checks if this item is an artifact.
        /// </summary>
        public bool IsArtifact
        {
            get { return (flags & artifactMask) > 0; }
        }

        /// <summary>
        /// Checks if this item is light source.
        /// </summary>
        public bool IsLightSource
        {   // Torch, Lantern, Candle, Holy Candle.
            get { return (itemGroup == ItemGroups.UselessItems2 && 
                          (TemplateIndex == (int)UselessItems2.Torch ||
                           TemplateIndex == (int)UselessItems2.Lantern ||
                           TemplateIndex == (int)UselessItems2.Candle)) ||
                          IsOfTemplate(ItemGroups.ReligiousItems, (int)ReligiousItems.Holy_candle); }
        }

        /// <summary>
        /// Checks if this item is of any shield type.
        /// </summary>
        public bool IsShield
        {
            get { return GetIsShield(); }
        }

        /// <summary>
        /// Checks if this item is identified.
        /// </summary>
        public bool IsIdentified
        {
            get { return GetIsIdentified(); }
        }

        /// <summary>
        /// Checks if this item is a potion recipe.
        /// </summary>
        public bool IsPotionRecipe
        {
            get { return ItemGroup == ItemGroups.MiscItems && TemplateIndex == (int)MiscItems.Potion_recipe; }
        }

        /// <summary>
        /// Checks if this item is a potion.
        /// </summary>
        public bool IsPotion
        {
            get { return ItemGroup == ItemGroups.UselessItems1 && TemplateIndex == (int)UselessItems1.Glass_Bottle; }
        }

        /// <summary>
        /// Checks if this item is a parchment.
        /// </summary>
        public bool IsParchment
        {
            get { return ItemGroup == ItemGroups.UselessItems2 && TemplateIndex == (int)UselessItems2.Parchment; }
        }

        /// <summary>
        /// Checks if this item is clothing.
        /// </summary>
        public bool IsClothing
        {
            get { return ItemGroup == ItemGroups.MensClothing || ItemGroup == ItemGroups.WomensClothing; }
        }

        /// <summary>
        /// Gets/sets the soul trapped in a soul trap.
        /// </summary>
        public MobileTypes TrappedSoulType
        {
            get { return trappedSoulType; }
            set { trappedSoulType = value; }
        }

        /// <summary>
        /// Gets/sets the key of the potion recipe allocated to this item.
        /// Has a side effect (ugh, sorry) of populating the item value from the recipe price.
        /// (due to value not being encapsulated) Also populates texture record for potions.
        /// </summary>
        public int PotionRecipeKey
        {
            get { return potionRecipeKey; }
            set {
                PotionRecipe potionRecipe = GameManager.Instance.EntityEffectBroker.GetPotionRecipe(value);
                if (potionRecipe != null)
                {
                    potionRecipeKey = value;
                    this.value = potionRecipe.Price;
                    if (IsPotion)
                        worldTextureRecord = potionRecipe.TextureRecord;
                }
            }
        }

        /// <summary>
        /// Gets/sets the time for this item to disappear.
        /// </summary>
        public uint TimeForItemToDisappear
        {
            get { return timeForItemToDisappear; }
            set { timeForItemToDisappear = value; }
        }

        /// <summary>
        /// Get flag checking if this is a summoned item.
        /// Summoned items have a specific future game time they expire.
        /// Non-summoned items have 0 in this field.
        /// </summary>
        public bool IsSummoned
        {
            get { return timeForItemToDisappear != 0; }
        }

        /// <summary>
        /// Check if this is a quest item.
        /// </summary>
        public bool IsQuestItem
        {
            get { return isQuestItem; }
        }

        /// <summary>
        /// Gets quest UID of quest owning this item.
        /// </summary>
        public ulong QuestUID
        {
            get { return questUID; }
        }

        /// <summary>
        /// Gets symbol of quest item inside quest.
        /// </summary>
        public Symbol QuestItemSymbol
        {
            get { return questItemSymbol; }
        }

        /// <summary>
        /// Gets native material value.
        /// </summary>
        public virtual int NativeMaterialValue
        {
            get { return nativeMaterialValue; }
        }

        /// <summary>Gets the repair data for this item.</summary>
        public ItemRepairData RepairData
        {
            get { return repairData; }
        }

        /// <summary>Gets current item condition as a percentage of max.</summary>
        public int ConditionPercentage
        {
            get { return maxCondition > 0 ? 100 * currentCondition / maxCondition : 100; }
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
        /// <returns>Cloned item.</returns>
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
            // Hand off for artifacts
            if (itemGroup == ItemGroups.Artifacts)
            {
                SetArtifact(itemGroup, groupIndex);
                return;
            }

            // Get template data
            ItemTemplate itemTemplate = DaggerfallUnity.Instance.ItemHelper.GetItemTemplate(itemGroup, groupIndex);

            // Assign new data
            shortName = itemTemplate.name; // LOCALIZATION_TODO: Lookup item template name from localization
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
            value = itemTemplate.basePrice;
            unknown = 0;
            flags = 0;
            currentCondition = itemTemplate.hitPoints;
            maxCondition = itemTemplate.hitPoints;
            unknown2 = 0;
            typeDependentData = 0;
            enchantmentPoints = itemTemplate.enchantmentPoints;
            message = (itemGroup == ItemGroups.Paintings) ? UnityEngine.Random.Range(0, 65536) : 0;
            stackCount = 1;
        }

        /// <summary>
        /// Sets item by merging item template and artifact template data.
        /// Result is a normal DaggerfallUnityItem with properties of both base template and magic item template.
        /// </summary>
        /// <param name="groupIndex">Artifact group index.</param>
        public void SetArtifact(ItemGroups itemGroup, int groupIndex)
        {
            // Must be an artifact type
            if (itemGroup != ItemGroups.Artifacts)
                throw new Exception("An attempt was made to SetArtifact() with non-artifact ItemGroups value.");

            // Get artifact template
            MagicItemTemplate magicItemTemplate = DaggerfallUnity.Instance.ItemHelper.GetArtifactTemplate(groupIndex);

            // Get base item template data, this is the fundamental item type being expanded
            ItemTemplate itemTemplate = DaggerfallUnity.Instance.ItemHelper.GetItemTemplate((ItemGroups)magicItemTemplate.group, magicItemTemplate.groupIndex);

            // Get artifact texture indices
            int archive, record;
            DaggerfallUnity.Instance.ItemHelper.GetArtifactTextureIndices((ArtifactsSubTypes)groupIndex, out archive, out record);

            // Correct material value for armor artifacts
            int materialValue = magicItemTemplate.material;
            if (magicItemTemplate.group == (int)ItemGroups.Armor)
                materialValue = 0x200 + materialValue;

            // Assign new data
            shortName = magicItemTemplate.name; // LOCALIZATION_TODO: Lookup magic item template name from localization
            this.itemGroup = (ItemGroups)magicItemTemplate.group;
            this.groupIndex = magicItemTemplate.groupIndex;
            playerTextureArchive = archive;
            playerTextureRecord = record;
            worldTextureArchive = archive;                  // Not sure about artifact world textures, just using player texture for now
            worldTextureRecord = record;
            nativeMaterialValue = materialValue;
            dyeColor = DyeColors.Unchanged;
            weightInKg = itemTemplate.baseWeight;
            drawOrder = itemTemplate.drawOrderOrEffect;
            currentVariant = 0;
            value = magicItemTemplate.value;
            unknown = 0;
            flags = artifactMask | identifiedMask;      // Set as artifact & identified.
            currentCondition = magicItemTemplate.uses;
            maxCondition = magicItemTemplate.uses;
            unknown2 = 0;
            typeDependentData = 0;
            enchantmentPoints = 0;
            message = 0;
            stackCount = 1;

            // All artifacts have magical effects
            bool foundEnchantment = false;
            legacyMagic = new DaggerfallEnchantment[magicItemTemplate.enchantments.Length];
            for (int i = 0; i < magicItemTemplate.enchantments.Length; i++)
            {
                legacyMagic[i] = magicItemTemplate.enchantments[i];
                if (legacyMagic[i].type != EnchantmentTypes.None)
                    foundEnchantment = true;
            }

            // Discard list if no enchantment found
            if (!foundEnchantment)
                legacyMagic = null;
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

        // Horses, carts, arrows and maps are not counted against encumbrance.
        public float EffectiveUnitWeightInKg()
        {
            if (ItemGroup == ItemGroups.Transportation || TemplateIndex == (int)Weapons.Arrow ||
                IsOfTemplate(ItemGroups.MiscItems, (int)MiscItems.Map))
                return 0f;
            return weightInKg;
        }

        /// <summary>
        /// Determines if item is stackable.
        /// Only ingredients, potions, gold pieces, oil and arrows are stackable,
        /// but equipped items, enchanted ingredients and quest items are never stackable.
        /// </summary>
        /// <returns>True if item stackable.</returns>
        public virtual bool IsStackable()
        {
            if (IsSummoned)
            {
                // Only allowing summoned arrows to stack at this time
                // But they should only stack with other summoned arrows
                if (!IsOfTemplate(ItemGroups.Weapons, (int)Weapons.Arrow))
                    return false;
            }
            // Equipped, quest and enchanted items cannot stack.
            if (IsEquipped || IsQuestItem || IsEnchanted)
                return false;

            return FormulaHelper.IsItemStackable(this);
        }

        /// <summary>
        /// Determines if item is a stack.
        /// </summary>
        /// <returns><c>true</c> if item is a stack, <c>false</c> otherwise.</returns>
        public bool IsAStack() 
        {
            return stackCount > 1;
        }

        /// <summary>
        /// Allow use of item to be implemented by item object and overridden
        /// </summary>
        /// <returns><c>true</c>, if item use was handled, <c>false</c> otherwise.</returns>
        public virtual bool UseItem(ItemCollection collection)
        {
            return false;
        }

        /// <summary>
        /// Cycles through variants.
        /// </summary>
        public void NextVariant()
        {
            // Only certain items support user-initiated variants
            bool canChangeVariant = false;
            switch (TemplateIndex)
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
        public virtual ItemData_v1 GetSaveData()
        {
            ItemData_v1 data = new ItemData_v1();
            data.uid = uid;
            data.shortName = shortName;
            data.nativeMaterialValue = nativeMaterialValue;
            data.dyeColor = dyeColor;
            data.weightInKg = weightInKg;
            data.drawOrder = drawOrder;
            data.value1 = value;
            data.value2 = (unknown & 0xffff) | (flags << 16);
            data.hits1 = currentCondition;
            data.hits2 = maxCondition;
            data.hits3 = (unknown2 & 0xff) | (typeDependentData << 8);
            data.enchantmentPoints = enchantmentPoints;
            data.message = message;
            if (legacyMagic != null)
            {
                data.legacyMagic = new int[legacyMagic.Length * 2];
                int j = 0;
                for (int i = 0; i < legacyMagic.Length; i++)
                {
                    data.legacyMagic[j] = (short)(legacyMagic[i].type);
                    j++;
                    data.legacyMagic[j] = legacyMagic[i].param;
                    j++;
                }
            }
            data.customMagic = customMagic;
            data.playerTextureArchive = playerTextureArchive;
            data.playerTextureRecord = playerTextureRecord;
            data.worldTextureArchive = worldTextureArchive;
            data.worldTextureRecord = worldTextureRecord;
            data.itemGroup = itemGroup;
            data.groupIndex = groupIndex;
            data.currentVariant = currentVariant;
            data.stackCount = stackCount;
            data.isQuestItem = isQuestItem;
            data.questUID = questUID;
            data.questItemSymbol = questItemSymbol;
            data.trappedSoulType = trappedSoulType;
            data.poisonType = poisonType;
            if ((int)poisonType < MagicAndEffects.MagicEffects.PoisonEffect.startValue)
                data.poisonType = Poisons.None;
            data.potionRecipe = potionRecipeKey;
            data.repairData = repairData.GetSaveData();
            data.timeForItemToDisappear = timeForItemToDisappear;
            data.timeHealthLeechLastUsed = timeHealthLeechLastUsed;

            return data;
        }

        public virtual SoundClips GetEquipSound()
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

        public virtual SoundClips GetSwingSound()
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
                case (int)Weapons.Mace:
                    return SoundClips.SwingMediumPitch;
                case (int)Weapons.Dagger:
                case (int)Weapons.Tanto:
                case (int)Weapons.Shortsword:
                    return SoundClips.SwingHighPitch;
                case (int)Weapons.Short_Bow:
                case (int)Weapons.Long_Bow:
                    return SoundClips.ArrowShoot;

                default:
                    return SoundClips.None;
            }
        }

        public virtual int GetWeaponSkillUsed()
        {
            switch (TemplateIndex)
            {
                case (int)Weapons.Dagger:
                case (int)Weapons.Tanto:
                case (int)Weapons.Wakazashi:
                case (int)Weapons.Shortsword:
                    return (int)DaggerfallConnect.DFCareer.ProficiencyFlags.ShortBlades;
                case (int)Weapons.Broadsword:
                case (int)Weapons.Longsword:
                case (int)Weapons.Saber:
                case (int)Weapons.Katana:
                case (int)Weapons.Claymore:
                case (int)Weapons.Dai_Katana:
                    return (int)DaggerfallConnect.DFCareer.ProficiencyFlags.LongBlades;
                case (int)Weapons.Battle_Axe:
                case (int)Weapons.War_Axe:
                    return (int)DaggerfallConnect.DFCareer.ProficiencyFlags.Axes;
                case (int)Weapons.Flail:
                case (int)Weapons.Mace:
                case (int)Weapons.Warhammer:
                case (int)Weapons.Staff:
                    return (int)DaggerfallConnect.DFCareer.ProficiencyFlags.BluntWeapons;
                case (int)Weapons.Short_Bow:
                case (int)Weapons.Long_Bow:
                    return (int)DaggerfallConnect.DFCareer.ProficiencyFlags.MissileWeapons;

                default:
                    return (int)Skills.None;
            }
        }

        public virtual short GetWeaponSkillIDAsShort()
        {
            int skill = GetWeaponSkillUsed();
            switch (skill)
            {
                case (int)DFCareer.ProficiencyFlags.ShortBlades:
                    return (int)Skills.ShortBlade;
                case (int)DFCareer.ProficiencyFlags.LongBlades:
                    return (int)Skills.LongBlade;
                case (int)DFCareer.ProficiencyFlags.Axes:
                    return (int)Skills.Axe;
                case (int)DFCareer.ProficiencyFlags.BluntWeapons:
                    return (int)Skills.BluntWeapon;
                case (int)DFCareer.ProficiencyFlags.MissileWeapons:
                    return (int)Skills.Archery;

                default:
                    return (int)Skills.None;
            }
        }

        public DFCareer.Skills GetWeaponSkillID()
        {
            return (DFCareer.Skills)GetWeaponSkillIDAsShort();
        }

        public virtual int GetBaseDamageMin()
        {
            return FormulaHelper.CalculateWeaponMinDamage((Weapons)TemplateIndex);
        }

        public virtual int GetBaseDamageMax()
        {
            return FormulaHelper.CalculateWeaponMaxDamage((Weapons)TemplateIndex);
        }

        public int GetWeaponMaterialModifier()
        {
            switch (nativeMaterialValue)
            {
                case (int)WeaponMaterialTypes.Iron:
                    return -1;
                case (int)WeaponMaterialTypes.Steel:
                case (int)WeaponMaterialTypes.Silver:
                    return 0;
                case (int)WeaponMaterialTypes.Elven:
                    return 1;
                case (int)WeaponMaterialTypes.Dwarven:
                    return 2;
                case (int)WeaponMaterialTypes.Mithril:
                case (int)WeaponMaterialTypes.Adamantium:
                    return 3;
                case (int)WeaponMaterialTypes.Ebony:
                    return 4;
                case (int)WeaponMaterialTypes.Orcish:
                    return 5;
                case (int)WeaponMaterialTypes.Daedric:
                    return 6;

                default:
                    return 0;
            }
        }

        public virtual int GetMaterialArmorValue()
        {
            int result = 0;
            if (!IsShield)
            {
                switch (nativeMaterialValue)
                {
                    case (int)ArmorMaterialTypes.Leather:
                        result = 3;
                        break;
                    case (int)ArmorMaterialTypes.Chain:
                    case (int)ArmorMaterialTypes.Chain2:
                        result = 6;
                        break;
                    case (int)ArmorMaterialTypes.Iron:
                        result = 7;
                        break;
                    case (int)ArmorMaterialTypes.Steel:
                    case (int)ArmorMaterialTypes.Silver:
                        result = 9;
                        break;
                    case (int)ArmorMaterialTypes.Elven:
                        result = 11;
                        break;
                    case (int)ArmorMaterialTypes.Dwarven:
                        result = 13;
                        break;
                    case (int)ArmorMaterialTypes.Mithril:
                    case (int)ArmorMaterialTypes.Adamantium:
                        result = 15;
                        break;
                    case (int)ArmorMaterialTypes.Ebony:
                        result = 17;
                        break;
                    case (int)ArmorMaterialTypes.Orcish:
                        result = 19;
                        break;
                    case (int)ArmorMaterialTypes.Daedric:
                        result = 21;
                        break;
                }
            }
            else
            {
                return GetShieldArmorValue();
            }

            // Armor artifact appear to use armor rating divided by 2 rounded down
            if (IsArtifact && ItemGroup == ItemGroups.Armor)
                result /= 2;

            return result;
        }

        public virtual int GetShieldArmorValue()
        {
            switch (TemplateIndex)
            {
                case (int)Armor.Buckler:
                    return 1;
                case (int)Armor.Round_Shield:
                    return 2;
                case (int)Armor.Kite_Shield:
                    return 3;
                case (int)Armor.Tower_Shield:
                    return 4;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Get body parts protected by a shield.
        /// </summary>
        public virtual BodyParts[] GetShieldProtectedBodyParts()
        {
            switch (TemplateIndex)
            {
                case (int)Armor.Buckler:
                    return new BodyParts[] { BodyParts.LeftArm, BodyParts.Hands };
                case (int)Armor.Round_Shield:
                case (int)Armor.Kite_Shield:
                    return new BodyParts[] { BodyParts.LeftArm, BodyParts.Hands, BodyParts.Legs };
                case (int)Armor.Tower_Shield:
                    return new BodyParts[] { BodyParts.Head, BodyParts.LeftArm, BodyParts.Hands, BodyParts.Legs };

                default:
                    return new BodyParts[] { };
            }
        }

        /// <summary>
        /// Get the equip slot that matches to a body part.
        /// Used in armor calculations.
        /// </summary>
        public static EquipSlots GetEquipSlotForBodyPart(BodyParts bodyPart)
        {
            switch (bodyPart)
            {
                case BodyParts.Head:
                    return EquipSlots.Head;
                case BodyParts.RightArm:
                    return EquipSlots.RightArm;
                case BodyParts.LeftArm:
                    return EquipSlots.LeftArm;
                case BodyParts.Chest:
                    return EquipSlots.ChestArmor;
                case BodyParts.Hands:
                    return EquipSlots.Gloves;
                case BodyParts.Legs:
                    return EquipSlots.LegsArmor;
                case BodyParts.Feet:
                    return EquipSlots.Feet;

                default:
                    return EquipSlots.None;
            }
        }

        /// <summary>
        /// Get the body part that matches to an equip slot.
        /// Used in armor calculations.
        /// </summary>
        public static BodyParts GetBodyPartForEquipSlot(EquipSlots equipSlot)
        {
            switch (equipSlot)
            {
                case EquipSlots.Head:
                    return BodyParts.Head;
                case EquipSlots.RightArm:
                    return BodyParts.RightArm;
                case EquipSlots.LeftArm:
                    return BodyParts.LeftArm;
                case EquipSlots.ChestArmor:
                    return BodyParts.Chest;
                case EquipSlots.Gloves:
                    return BodyParts.Hands;
                case EquipSlots.LegsArmor:
                    return BodyParts.Legs;
                case EquipSlots.Feet:
                    return BodyParts.Feet;

                default:
                    return BodyParts.None;
            }
        }

        public virtual EquipSlots GetEquipSlot()
        {
            return EquipSlots.None;
        }

        public virtual ItemHands GetItemHands()
        {
            return ItemHands.None;
        }

        public virtual WeaponTypes GetWeaponType()
        {
            return WeaponTypes.None;
        }

        public void LowerCondition(int amount, DaggerfallEntity unequipFromOwner = null, ItemCollection removeFromCollectionWhenBreaks = null)
        {
            currentCondition -= amount;
            if (currentCondition <= 0)
            {
                // Handle breaking - if AllowMagicRepairs enabled then item will not disappear
                currentCondition = 0;
                ItemBreaks(unequipFromOwner);
                if (removeFromCollectionWhenBreaks != null && !DaggerfallUnity.Settings.AllowMagicRepairs)
                    removeFromCollectionWhenBreaks.RemoveItem(this);
            }
        }

        public void UnequipItem(DaggerfallEntity owner)
        {
            if (owner == null)
                return;

            foreach (EquipSlots slot in Enum.GetValues(typeof(EquipSlots)))
            {
                if (owner.ItemEquipTable.GetItem(slot) == this)
                {
                    owner.ItemEquipTable.UnequipItem(slot);
                    owner.UpdateEquippedArmorValues(this, false);
                }
            }
        }

        protected void ItemBreaks(DaggerfallEntity owner)
        {
            // Classic does not have the plural version of this string, and uses the short name rather than the long one.
            // Also the classic string says "is" instead of "has"
            string itemBroke = "";
            if (TemplateIndex == (int)Armor.Boots || TemplateIndex == (int)Armor.Gauntlets || TemplateIndex == (int)Armor.Greaves)
                itemBroke = TextManager.Instance.GetLocalizedText("itemHasBrokenPlural");
            else
                itemBroke = TextManager.Instance.GetLocalizedText("itemHasBroken");
            itemBroke = itemBroke.Replace("%s", LongName);
            DaggerfallUI.Instance.PopupMessage(itemBroke);

            // Unequip item if owner specified
            if (owner != null)
                UnequipItem(owner);
            else
                return;

            // Breaks payload callback on owner effect manager
            if (owner.EntityBehaviour)
            {
                EntityEffectManager ownerEffectManager = owner.EntityBehaviour.GetComponent<EntityEffectManager>();
                if (ownerEffectManager)
                    ownerEffectManager.DoItemEnchantmentPayloads(EnchantmentPayloadFlags.Breaks, this, owner.Items, owner.EntityBehaviour);
            }
        }

        /// <summary>
        /// Link this DaggerfallUnityItem to a quest Item resource.
        /// </summary>
        /// <param name="questUID">UID of quest owning Item resource.</param>
        /// <param name="questItemSymbol">Symbol to locate Item resource in quest.</param>
        public void LinkQuestItem(ulong questUID, Symbol questItemSymbol)
        {
            this.isQuestItem = true;
            this.questUID = questUID;
            this.questItemSymbol = questItemSymbol;
        }

        /// <summary>
        /// Remove status as quest item so it becomes permanent and will not be removed at end of quest.
        /// </summary>
        public void MakePermanent()
        {
            if (isQuestItem)
            {
                questUID = 0;
                questItemSymbol = null;
                isQuestItem = false;
            }
        }

        /// <summary>
        /// Identifies the item.
        /// </summary>
        public void IdentifyItem()
        {
            flags = (ushort)(flags | identifiedMask);
        }

        /// <summary>
        /// Gets the enchantment points of this item.
        /// </summary>
        public virtual int GetEnchantmentPower()
        {
            return FormulaHelper.GetItemEnchantmentPower(this);
        }

        /// <summary>
        /// Set enchantments on this item. Any existing enchantments will be overwritten.
        /// </summary>
        /// <param name="enchantments">Array of enchantment settings. Maximum of 10 enchantments are applied.</param>
        /// <param name="owner">Owner of this item for unequip test.</param>
        public void SetEnchantments(EnchantmentSettings[] enchantments, DaggerfallEntity owner = null)
        {
            const int maxEnchantments = 10;

            // Validate list
            if (enchantments == null || enchantments.Length == 0)
                throw new Exception("SetEnchantments() enchantments cannot be null or empty.");

            // Build enchantment lists
            int count = 0;
            List<DaggerfallEnchantment> legacyEnchantments = new List<DaggerfallEnchantment>();
            List<CustomEnchantment> customEnchantments = new List<CustomEnchantment>();
            foreach (EnchantmentSettings settings in enchantments)
            {
                // Enchantment must have an effect key
                if (string.IsNullOrEmpty(settings.EffectKey))
                    throw new Exception(string.Format("SetEnchantments() effect key is null or empty at index {0}", count));

                // Created payload callback
                IEntityEffect effectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(settings.EffectKey);
                if (effectTemplate != null)
                {
                    EnchantmentParam param = new EnchantmentParam()
                    {
                        ClassicParam = settings.ClassicParam,
                        CustomParam = settings.CustomParam,
                    };
                    if (effectTemplate.HasEnchantmentPayloadFlags(EnchantmentPayloadFlags.Enchanted))
                        effectTemplate.EnchantmentPayloadCallback(EnchantmentPayloadFlags.Enchanted, param, null, null, this);
                }

                // Add custom or legacy enchantment
                if (!string.IsNullOrEmpty(settings.CustomParam))
                {
                    CustomEnchantment customEnchantment = new CustomEnchantment()
                    {
                        EffectKey = settings.EffectKey,
                        CustomParam = settings.CustomParam,
                    };
                    customEnchantments.Add(customEnchantment);
                }
                else
                {
                    if (settings.ClassicType == EnchantmentTypes.None)
                        throw new Exception(string.Format("SetEnchantments() not a valid enchantment type at index {0}", count));

                    DaggerfallEnchantment legacyEnchantment = new DaggerfallEnchantment()
                    {
                        type = settings.ClassicType,
                        param = settings.ClassicParam,
                    };
                    legacyEnchantments.Add(legacyEnchantment);
                }

                // Cap enchanting at limit
                if (++count > maxEnchantments)
                    break;
            }

            // Do nothing if no enchantments found
            if (customEnchantments.Count == 0 && legacyEnchantments.Count == 0)
                throw new Exception("SetEnchantments() no enchantments provided");

            // Unequip item - entity must equip again
            // This ensures "on equip" effect payloads execute correctly
            UnequipItem(owner);

            // Set new enchantments and identified flag
            legacyMagic = legacyEnchantments.ToArray();
            customMagic = customEnchantments.ToArray();
            IdentifyItem();
        }

        /// <summary>
        /// Rename item.
        /// </summary>
        /// <param name="name">New name of item. Cannot be null or empty.</param>
        public void RenameItem(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                shortName = name;
            }
        }

        /// <summary>
        /// Check if item contains a specific legacy enchantment.
        /// </summary>
        /// <param name="type">Legacy type.</param>
        /// <param name="param">Legacy param.</param>
        /// <returns>True if item contains enchantment.</returns>
        public bool ContainsEnchantment(EnchantmentTypes type, short param)
        {
            if (legacyMagic == null || legacyMagic.Length == 0)
                return false;

            foreach (DaggerfallEnchantment enchantment in LegacyEnchantments)
            {
                if (enchantment.type == type && enchantment.param == param)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check if item contains a specific custom enchantment.
        /// </summary>
        /// <param name="key">Effect key.</param>
        /// <param name="param">Effect param.</param>
        /// <returns>True if item contains enchantment.</returns>
        public bool ContainsEnchantment(string key, string param)
        {
            if (customMagic == null || customMagic.Length == 0)
                return false;

            foreach (CustomEnchantment enchantment in CustomEnchantments)
            {
                if (enchantment.EffectKey == key && enchantment.CustomParam == param)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Combines all legacy and custom enchantments into a single array.
        /// Using EnchantmentSettings to store relavent combined enchantment information.
        /// Not all properties of EnchantmentSettings are set here, just what is needed to identify enchantment.
        /// </summary>
        /// <returns>Array of enchantment settings, can be null or empty.</returns>
        public EnchantmentSettings[] GetCombinedEnchantmentSettings()
        {
            List<EnchantmentSettings> combinedEnchantments = new List<EnchantmentSettings>();

            // Item must have enchancements
            if (!IsEnchanted)
                return null;

            // Legacy enchantments
            if (legacyMagic != null && legacyMagic.Length > 0)
            {
                foreach (DaggerfallEnchantment enchantment in legacyMagic)
                {
                    // Ignore empty enchantment slots
                    if (enchantment.type == EnchantmentTypes.None)
                        continue;

                    // Get classic effect key - enchantments use EnchantmentTypes string as key, artifacts use ArtifactsSubTypes string
                    string effectKey;
                    if (enchantment.type == EnchantmentTypes.SpecialArtifactEffect)
                        effectKey = ((ArtifactsSubTypes)enchantment.param).ToString();
                    else
                        effectKey = enchantment.type.ToString();

                    // Add enchantment settings
                    combinedEnchantments.Add(new EnchantmentSettings()
                    {
                        EffectKey = effectKey,
                        ClassicType = enchantment.type,
                        ClassicParam = enchantment.param,
                    });
                }
            }

            // Custom enchantments
            if (customMagic != null && customMagic.Length > 0)
            {
                foreach (CustomEnchantment enchantment in customMagic)
                {
                    // Ignore enchantment with null or empty key
                    if (string.IsNullOrEmpty(enchantment.EffectKey))
                        continue;

                    // Add enchantment settings
                    combinedEnchantments.Add(new EnchantmentSettings()
                    {
                        EffectKey = enchantment.EffectKey,
                        CustomParam = enchantment.CustomParam,
                        ClassicType = EnchantmentTypes.None,
                    });
                }
            }

            return combinedEnchantments.ToArray();
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
            value = other.value;
            unknown = other.unknown;
            flags = other.flags;
            currentCondition = other.currentCondition;
            maxCondition = other.maxCondition;
            unknown2 = other.unknown2;
            stackCount = other.stackCount;
            enchantmentPoints = other.enchantmentPoints;
            message = other.message;
            potionRecipeKey = other.potionRecipeKey;
            timeHealthLeechLastUsed = other.timeHealthLeechLastUsed;

            isQuestItem = other.isQuestItem;
            questUID = other.questUID;
            questItemSymbol = other.questItemSymbol;

            if (other.legacyMagic != null)
                legacyMagic = (DaggerfallEnchantment[])other.legacyMagic.Clone();

            if (other.customMagic != null)
                customMagic = (CustomEnchantment[])other.customMagic.Clone();
        }

        /// <summary>
        /// Create from native save ItemRecord data.
        /// </summary>
        void FromItemRecord(ItemRecord itemRecord)
        {
            // Get template data
            ItemGroups group = (ItemGroups)itemRecord.ParsedData.group;
            int index = itemRecord.ParsedData.index;
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
            shortName = itemRecord.ParsedData.name; // LOCALIZATION_TODO: Lookup item template name from localizations
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
            value = (int)itemRecord.ParsedData.value;
            unknown = itemRecord.ParsedData.unknown;
            flags = itemRecord.ParsedData.flags;
            currentCondition = itemRecord.ParsedData.currentCondition;
            maxCondition = itemRecord.ParsedData.maxCondition;
            unknown2 = itemRecord.ParsedData.unknown2;
            typeDependentData = itemRecord.ParsedData.typeDependentData;
            // If item is an arrow, typeDependentData is the stack count
            if ((itemGroup == ItemGroups.Weapons) && (groupIndex == 18)) // index 18 is for arrows
            {
                stackCount = itemRecord.ParsedData.typeDependentData;
            }
            else
            {
                stackCount = 1;
            }
            // Convert classic recipes to DFU recipe key.
            if ((IsPotion || IsPotionRecipe) && typeDependentData < MagicAndEffects.PotionRecipe.classicRecipeKeys.Length)
                potionRecipeKey = MagicAndEffects.PotionRecipe.classicRecipeKeys[typeDependentData];

            currentVariant = 0;
            enchantmentPoints = itemRecord.ParsedData.enchantmentPoints;
            message = (int)itemRecord.ParsedData.message;

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
                legacyMagic = new DaggerfallEnchantment[itemRecord.ParsedData.magic.Length];
                for (int i = 0; i < itemRecord.ParsedData.magic.Length; i++)
                {
                    legacyMagic[i] = itemRecord.ParsedData.magic[i];
                    if (legacyMagic[i].type != EnchantmentTypes.None)
                        foundEnchantment = true;
                }
            }

            // Discard list if no enchantment found
            if (!foundEnchantment)
                legacyMagic = null;

            // TEST: Force dye color to match material of imported weapons & armor
            // This is to fix cases where dye colour may be set incorrectly on imported item
            dyeColor = DaggerfallUnity.Instance.ItemHelper.GetDyeColor(this);
        }

        /// <summary>
        /// Creates from a serialized item.
        /// </summary>
        internal void FromItemData(ItemData_v1 data)
        {
            uid = data.uid;
            shortName = data.shortName;
            nativeMaterialValue = data.nativeMaterialValue;
            dyeColor = data.dyeColor;
            weightInKg = data.weightInKg;
            drawOrder = data.drawOrder;
            value = data.value1;
            // These are being saved in DF Unity saves as one int32 value but are two 16-bit values in classic
            unknown = (ushort)(data.value2 & 0xffff);
            flags = (ushort)(data.value2 >> 16);
            currentCondition = data.hits1;
            maxCondition = data.hits2;
            // These are being saved in DF Unity saves as one int32 value but are two 8-bit values in classic
            unknown2 = (byte)(data.hits3 & 0xff);
            typeDependentData = (byte)(data.hits3 >> 8);
            enchantmentPoints = data.enchantmentPoints;
            message = data.message;
            // Convert magic data that was saved as int array
            if (data.legacyMagic != null)
            {
                legacyMagic = new DaggerfallEnchantment[data.legacyMagic.Length / 2];
                int j = 0;
                for (int i = 0; i < legacyMagic.Length; i++)
                {
                    legacyMagic[i].type = (EnchantmentTypes)(data.legacyMagic[j]);
                    j++;
                    legacyMagic[i].param = (short)(data.legacyMagic[j]);
                    j++;
                }
            }
            customMagic = data.customMagic;
            playerTextureArchive = data.playerTextureArchive;
            playerTextureRecord = data.playerTextureRecord;
            worldTextureArchive = data.worldTextureArchive;
            worldTextureRecord = data.worldTextureRecord;
            itemGroup = data.itemGroup;
            groupIndex = data.groupIndex;
            currentVariant = data.currentVariant;
            stackCount = data.stackCount;
            isQuestItem = data.isQuestItem;
            questUID = data.questUID;
            questItemSymbol = data.questItemSymbol;
            trappedSoulType = data.trappedSoulType;
            poisonType = data.poisonType;
            if ((int)data.poisonType < MagicAndEffects.MagicEffects.PoisonEffect.startValue)
                poisonType = Poisons.None;
            potionRecipeKey = data.potionRecipe;
            timeHealthLeechLastUsed = data.timeHealthLeechLastUsed;
            // Convert any old classic recipe items in saves to DFU recipe key.
            if (potionRecipeKey == 0 && (IsPotion || IsPotionRecipe) && typeDependentData < MagicAndEffects.PotionRecipe.classicRecipeKeys.Length)
                potionRecipeKey = MagicAndEffects.PotionRecipe.classicRecipeKeys[typeDependentData];

            repairData.RestoreRepairData(data.repairData);

            timeForItemToDisappear = data.timeForItemToDisappear;
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

            // Use texture record retrieved from MAGIC.DEF for artifacts. Otherwise the below code will give the Oghma Infinium record 2, from the "Book" template.
            if (IsArtifact)
                return playerTextureRecord;

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
            // Men's cloaks
            if (ItemGroup == ItemGroups.MensClothing &&
               (TemplateIndex == (int)MensClothing.Casual_cloak ||
                TemplateIndex == (int)MensClothing.Formal_cloak))
            {
                return true;
            }

            // Women's cloaks
            if (ItemGroup == ItemGroups.WomensClothing &&
               (TemplateIndex == (int)WomensClothing.Casual_cloak ||
                TemplateIndex == (int)WomensClothing.Formal_cloak))
            {
                return true;
            }

            return false;
        }

        // Check if item has any valid legacy enchantments
        bool GetHasLegacyEnchantment()
        {
            if (legacyMagic == null || legacyMagic.Length == 0)
                return false;

            // Check for legacy magic effects
            for (int i = 0; i < legacyMagic.Length; i++)
            {
                if (legacyMagic[i].type != EnchantmentTypes.None)
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

        // Check if this item is identified. (only relevant if item has some enchantments)
        bool GetIsIdentified()
        {
            if (!IsEnchanted)
                return true;
            return (flags & identifiedMask) > 0;
        }

        // Determines if world texture should be displayed in place of player texture
        // Many inventory item types have this setup and more may need to be added
        bool UseWorldTexture()
        {
            // Handle uselessitems1
            if (itemGroup == ItemGroups.UselessItems1)
                return true;

            // Handle ingredients
            if (IsIngredient)
                return true;

            // Handle arrows
            if (IsOfTemplate(ItemGroups.Weapons, (int)Weapons.Arrow))
            {
                return true;
            }

            // Handle religious items
            if (itemGroup == ItemGroups.ReligiousItems)
                return true;

            // Handle misc items
            if (itemGroup == ItemGroups.MiscItems)
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

        #region Events

        // OnWeaponStrike
        public delegate void OnWeaponStrikeEventHandler(DaggerfallUnityItem item, DaggerfallEntityBehaviour receiver, int damage);
        public event OnWeaponStrikeEventHandler OnWeaponStrike;
        public void RaiseOnWeaponStrikeEvent(DaggerfallEntityBehaviour receiver, int damage)
        {
            if (OnWeaponStrike != null)
                OnWeaponStrike(this, receiver, damage);
        }

        #endregion
    }
}
