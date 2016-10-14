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
using System.Collections.Generic;
using UnityEngine;
using DaggerfallConnect.FallExe;
using DaggerfallConnect.Save;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// Helper class for working with items.
    /// </summary>
    public class ItemHelper
    {
        #region Fields

        const string itemTemplatesFilename = "ItemTemplates";
        const string containerIconsFilename = "INVE16I0.CIF";

        List<ItemTemplate> itemTemplates = new List<ItemTemplate>();
        Dictionary<int, ImageData> itemImages = new Dictionary<int, ImageData>();
        Dictionary<InventoryContainerImages, ImageData> containerImages = new Dictionary<InventoryContainerImages, ImageData>();

        #endregion

        #region Constructors

        public ItemHelper()
        {
            LoadItemTemplates();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets item template data using group and index.
        /// </summary>
        public ItemTemplate GetItemTemplate(ItemGroups itemGroup, int groupIndex)
        {
            Array values = GetEnumArray(itemGroup);
            if (groupIndex < 0 || groupIndex >= values.Length)
            {
                string message = string.Format("Item index out of range: Group={0} Index={1}", itemGroup.ToString(), groupIndex);
                Debug.Log(message);
                return new ItemTemplate();
            }

            int templateIndex = Convert.ToInt32(values.GetValue(groupIndex));

            return itemTemplates[templateIndex];
        }

        /// <summary>
        /// Gets item template from direct template index.
        /// </summary>
        public ItemTemplate GetItemTemplate(int templateIndex)
        {
            if (templateIndex < 0 || templateIndex >= itemTemplates.Count)
            {
                string message = string.Format("Item template index out of range: TemplateIndex={1}", templateIndex);
                Debug.Log(message);
                return new ItemTemplate();
            }

            return itemTemplates[templateIndex];
        }

        /// <summary>
        /// Gets item group index from group and template index.
        /// </summary>
        /// <returns>Item group index, or -1 if not found.</returns>
        public int GetGroupIndex(ItemGroups itemGroup, int templateIndex)
        {
            Array values = GetEnumArray(itemGroup);
            for (int i = 0; i < values.Length; i++)
            {
                int checkTemplateIndex = Convert.ToInt32(values.GetValue(i));
                if (checkTemplateIndex == templateIndex)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Resolves full item name using parameters like %it and material type.
        /// </summary>
        public string ResolveItemName(DaggerfallUnityItem item)
        {
            // Start with base name
            string result = item.shortName;

            // Get item template
            ItemTemplate template = item.ItemTemplate;

            // Resolve %it parameter
            result = result.Replace("%it", template.name);

            // Resolve weapon material
            if (item.ItemGroup == ItemGroups.Weapons)
            {
                WeaponMaterialTypes weaponMaterial = (WeaponMaterialTypes)item.nativeMaterialValue;
                string materialName = DaggerfallUnity.Instance.TextProvider.GetWeaponMaterialName(weaponMaterial);
                result = string.Format("{0} {1}", materialName, result);
            }

            // Resolve armor material
            if (item.ItemGroup == ItemGroups.Armor)
            {
                ArmorMaterialTypes armorMaterial = (ArmorMaterialTypes)item.nativeMaterialValue;
                string materialName = DaggerfallUnity.Instance.TextProvider.GetArmorMaterialName(armorMaterial);
                result = string.Format("{0} {1}", materialName, result);
            }

            return result;
        }

        /// <summary>
        /// Gets inventory/equip image for specified item.
        /// Image will be cached based on material and hand for faster subsequent fetches.
        /// </summary>
        /// <param name="item">Item to fetch image for.</param>
        /// <param name="removeMask">Removes mask index (e.g. around helmets) from final image.</param>
        /// <param name="forPaperDoll">Image is for paper doll.</param>
        /// <returns>ImageData.</returns>
        public ImageData GetItemImage(DaggerfallUnityItem item, bool removeMask = false, bool forPaperDoll = false)
        {
            // Get colour
            int color = (int)item.dyeColor;

            // Get archive and record indices
            int archive = item.InventoryTextureArchive;
            int record = item.InventoryTextureRecord;

            // Paper doll handling
            if (forPaperDoll)
            {
                // 1H Weapons in right hand need record + 1
                if (item.ItemGroup == ItemGroups.Weapons && item.EquipSlot == EquipSlots.RightHand)
                {
                    if (ItemEquipTable.GetItemHands(item) == ItemHands.Either)
                        record += 1;
                }
            }
            else
            {
                // Katanas need +1 for inventory image as they use right-hand image instead of left
                if (item.IsOfTemplate(ItemGroups.Weapons, (int)Weapons.Katana))
                    record += 1;
            }

            // Gold pieces use world texture indices
            if (item.ItemGroup == ItemGroups.Currency && item.TemplateIndex == (int)Currency.Gold_pieces)
            {
                archive = item.ItemTemplate.worldTextureArchive;
                record = item.ItemTemplate.worldTextureRecord;
            }

            // Get unique key
            int key = MakeImageKey(color, archive, record, removeMask);

            // Get existing icon if in cache
            if (itemImages.ContainsKey(key))
                return itemImages[key];

            // Load image data
            string filename = TextureFile.IndexToFileName(archive);
            ImageData data = ImageReader.GetImageData(filename, record, 0, true, false);
            if (data.type == ImageTypes.None)
                throw new Exception("GetItemImage() could not load image data.");

            // Fix items with known incorrect paper doll offsets
            if (archive == 237 && (record == 52 || record == 54))
            {
                // "Short shirt" template index 202 variants 2 and 5 for human female
                data.offset = new DaggerfallConnect.Utility.DFPosition(237, 43);
            }

            // Remove mask if requested
            if (removeMask)
                data.dfBitmap = ImageProcessing.ChangeMask(data.dfBitmap);

            // Change dye or just update texture
            ItemGroups group = item.ItemGroup;
            DyeColors dye = (DyeColors)color;
            if (group == ItemGroups.Weapons || group == ItemGroups.Armor)
                data = ChangeDye(data, dye, DyeTargets.WeaponsAndArmor);
            else if (item.ItemGroup == ItemGroups.MensClothing || item.ItemGroup == ItemGroups.WomensClothing)
                data = ChangeDye(data, dye, DyeTargets.Clothing);
            else
                ImageReader.UpdateTexture(ref data);

            // Add to cache
            itemImages.Add(key, data);

            return data;
        }

        /// <summary>
        /// Gets item image with custom mask colour.
        /// </summary>
        /// <param name="item">Item to fetch image for.</param>
        /// <param name="maskColor">New mask colour.</param>
        /// <param name="forPaperDoll">Image is for paper doll.</param>
        /// <returns>ImageData.</returns>
        public ImageData GetItemImage(DaggerfallUnityItem item, Color maskColor, bool forPaperDoll = false)
        {
            // Get base item with mask intact
            ImageData result = GetItemImage(item, false, forPaperDoll);
            ImageReader.UpdateTexture(ref result, maskColor);

            return result;
        }

        /// <summary>
        /// Gets icon for a container object, such as the wagon.
        /// </summary>
        /// <param name="type">Container type.</param>
        /// <returns>ImageData.</returns>
        public ImageData GetContainerImage(InventoryContainerImages type)
        {
            // Get existing icon if in cache
            if (containerImages.ContainsKey(type))
                return containerImages[type];

            // Load image data
            ImageData data = ImageReader.GetImageData(containerIconsFilename, (int)type, 0, true, true);

            // Add to cache
            containerImages.Add(type, data);

            return data;
        }

        /// <summary>
        /// Helper to get interior parts of cloak.
        /// This is not cached as only seen on paper doll during refresh.
        /// </summary>
        /// <param name="item">Item - must be a formal or casual cloak.</param>
        /// <returns>ImageData.</returns>
        public ImageData GetCloakInteriorImage(DaggerfallUnityItem item)
        {
            // Must be a formal or casual cloak
            if (!IsCloak(item))
                return new ImageData();

            // Get colour
            int color = (int)item.dyeColor;

            // Cloak interior source is combination of player texture archive index and template record index
            int archive = item.InventoryTextureArchive;
            int record = item.ItemTemplate.playerTextureRecord;

            // Load image data
            string filename = TextureFile.IndexToFileName(archive);
            ImageData data = ImageReader.GetImageData(filename, record, 0, true, false);
            if (data.type == ImageTypes.None)
                throw new Exception("GetCloakBackImage() could not load image data.");

            // Change dye
            data = ChangeDye(data, (DyeColors)color, DyeTargets.Clothing);

            return data;
        }

        /// <summary>
        /// Checks if this item is a formal or casual cloak.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <returns>True if formal or casual cloak.</returns>
        public bool IsCloak(DaggerfallUnityItem item)
        {
            // Must be a formal or casual cloak
            switch (item.TemplateIndex)
            {
                case (int)MensClothing.Formal_cloak:
                case (int)MensClothing.Casual_cloak:
                case (int)WomensClothing.Formal_cloak:
                case (int)WomensClothing.Casual_cloak:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Converts Daggerfall weapon to generic API WeaponType.
        /// </summary>
        /// <param name="item">Weapon to convert.</param>
        /// <returns>WeaponTypes.</returns>
        public WeaponTypes ConvertItemToAPIWeaponType(DaggerfallUnityItem item)
        {
            // Must be a weapon
            if (item.ItemGroup != ItemGroups.Weapons)
                return WeaponTypes.None;

            // Find FPS animation set for this weapon type
            // Daggerfall re-uses the same animations for many different weapons
            WeaponTypes result = WeaponTypes.None;
            switch (item.TemplateIndex)
            {
                case (int)Weapons.Dagger:
                    result = WeaponTypes.Dagger;
                    break;
                case (int)Weapons.Staff:
                    result = WeaponTypes.Staff;
                    break;
                case (int)Weapons.Tanto:
                case (int)Weapons.Shortsword:
                case (int)Weapons.Wakazashi:
                case (int)Weapons.Broadsword:
                case (int)Weapons.Saber:
                case (int)Weapons.Longsword:
                case (int)Weapons.Katana:
                case (int)Weapons.Claymore:
                case (int)Weapons.Dai_Katana:
                    result = WeaponTypes.LongBlade;
                    break;
                case (int)Weapons.Mace:
                    result = WeaponTypes.Mace;
                    break;
                case (int)Weapons.Flail:
                    result = WeaponTypes.Flail;
                    break;
                case (int)Weapons.Warhammer:
                    result = WeaponTypes.Warhammer;
                    break;
                case (int)Weapons.Battle_Axe:
                case (int)Weapons.War_Axe:
                    result = WeaponTypes.Battleaxe;
                    break;
                case (int)Weapons.Short_Bow:
                case (int)Weapons.Long_Bow:
                    result = WeaponTypes.Bow;
                    break;
                default:
                    return WeaponTypes.None;
            }

            return result;
        }

        /// <summary>
        /// Converts native Daggerfall weapon and armor material types to generic Daggerfall Unity MetalType.
        /// The old metal type enum may be retired as true materials become better integrated.
        /// </summary>
        /// <param name="item">Item to convert material to metal type.</param>
        /// <returns>MetalTypes.</returns>
        public MetalTypes ConvertItemMaterialToAPIMetalType(DaggerfallUnityItem item)
        {
            // Determine metal type
            if (item.ItemGroup == ItemGroups.Weapons)
            {
                WeaponMaterialTypes weaponMaterial = (WeaponMaterialTypes)item.nativeMaterialValue;
                switch (weaponMaterial)
                {
                    case WeaponMaterialTypes.Iron:
                        return MetalTypes.Iron;
                    case WeaponMaterialTypes.Steel:
                        return MetalTypes.Steel;
                    case WeaponMaterialTypes.Silver:
                        return MetalTypes.Silver;
                    case WeaponMaterialTypes.Elven:
                        return MetalTypes.Elven;
                    case WeaponMaterialTypes.Dwarven:
                        return MetalTypes.Dwarven;
                    case WeaponMaterialTypes.Mithril:
                        return MetalTypes.Mithril;
                    case WeaponMaterialTypes.Adamantium:
                        return MetalTypes.Adamantium;
                    case WeaponMaterialTypes.Ebony:
                        return MetalTypes.Ebony;
                    case WeaponMaterialTypes.Orcish:
                        return MetalTypes.Orcish;
                    case WeaponMaterialTypes.Daedric:
                        return MetalTypes.Daedric;
                    default:
                        return MetalTypes.None;
                }
            }
            else if (item.ItemGroup == ItemGroups.Armor)
            {
                ArmorMaterialTypes armorMaterial = (ArmorMaterialTypes)item.nativeMaterialValue;
                switch (armorMaterial)
                {
                    case ArmorMaterialTypes.Iron:
                        return MetalTypes.Iron;
                    case ArmorMaterialTypes.Steel:
                        return MetalTypes.Steel;
                    case ArmorMaterialTypes.Chain:
                    case ArmorMaterialTypes.Chain2:
                        return MetalTypes.Chain;
                    case ArmorMaterialTypes.Silver:
                        return MetalTypes.Silver;
                    case ArmorMaterialTypes.Elven:
                        return MetalTypes.Elven;
                    case ArmorMaterialTypes.Dwarven:
                        return MetalTypes.Dwarven;
                    case ArmorMaterialTypes.Mithril:
                        return MetalTypes.Mithril;
                    case ArmorMaterialTypes.Adamantium:
                        return MetalTypes.Adamantium;
                    case ArmorMaterialTypes.Ebony:
                        return MetalTypes.Ebony;
                    case ArmorMaterialTypes.Orcish:
                        return MetalTypes.Orcish;
                    case ArmorMaterialTypes.Daedric:
                        return MetalTypes.Daedric;
                    default:
                        return MetalTypes.None;
                }
            }
            else
            {
                return MetalTypes.None;
            }
        }

        /// <summary>
        /// Gets dye color automatically from item data.
        /// </summary>
        /// <param name="item">DaggerfallUnityItem.</param>
        /// <returns>DyeColors.</returns>
        public DyeColors GetDyeColor(DaggerfallUnityItem item)
        {

            if (item.ItemGroup == ItemGroups.Weapons)
                return GetWeaponDyeColor((WeaponMaterialTypes)item.nativeMaterialValue);
            else if (item.ItemGroup == ItemGroups.Armor)
                return GetArmorDyeColor((ArmorMaterialTypes)item.nativeMaterialValue);
            else
                return item.dyeColor;
        }

        /// <summary>
        /// Converts weapon material to appropriate dye colour.
        /// </summary>
        /// <param name="material">WeaponMaterialTypes.</param>
        /// <returns>DyeColors.</returns>
        public DyeColors GetWeaponDyeColor(WeaponMaterialTypes material)
        {
            switch (material)
            {
                case WeaponMaterialTypes.Iron:
                    return DyeColors.Iron;
                case WeaponMaterialTypes.Steel:
                    return DyeColors.Steel;
                case WeaponMaterialTypes.Silver:
                case WeaponMaterialTypes.Elven:
                    return DyeColors.SilverOrElven;
                case WeaponMaterialTypes.Dwarven:
                    return DyeColors.Dwarven;
                case WeaponMaterialTypes.Mithril:
                    return DyeColors.Mithril;
                case WeaponMaterialTypes.Adamantium:
                    return DyeColors.Adamantium;
                case WeaponMaterialTypes.Ebony:
                    return DyeColors.Ebony;
                case WeaponMaterialTypes.Orcish:
                    return DyeColors.Orcish;
                case WeaponMaterialTypes.Daedric:
                    return DyeColors.Daedric;
                default:
                    return DyeColors.Unchanged;

            }
        }

        /// <summary>
        /// Converts armor material to appropriate dye colour.
        /// </summary>
        /// <param name="material">ArmorMaterialTypes.</param>
        /// <returns>DyeColors.</returns>
        public DyeColors GetArmorDyeColor(ArmorMaterialTypes material)
        {
            switch (material)
            {
                case ArmorMaterialTypes.Chain:
                case ArmorMaterialTypes.Chain2:
                    return DyeColors.Chain;
                case ArmorMaterialTypes.Iron:
                    return DyeColors.Iron;
                case ArmorMaterialTypes.Steel:
                    return DyeColors.Steel;
                case ArmorMaterialTypes.Silver:
                case ArmorMaterialTypes.Elven:
                    return DyeColors.SilverOrElven;
                case ArmorMaterialTypes.Dwarven:
                    return DyeColors.Dwarven;
                case ArmorMaterialTypes.Mithril:
                    return DyeColors.Mithril;
                case ArmorMaterialTypes.Adamantium:
                    return DyeColors.Adamantium;
                case ArmorMaterialTypes.Ebony:
                    return DyeColors.Ebony;
                case ArmorMaterialTypes.Orcish:
                    return DyeColors.Orcish;
                case ArmorMaterialTypes.Daedric:
                    return DyeColors.Daedric;
                default:
                    return DyeColors.Unchanged;

            }
        }

        ///// <summary>
        ///// Checks legacy item ID against legacy equip index to determine if item is equipped to a slot.
        ///// Must have previously used SetLegacyEquipTable to EntityItems provided.
        ///// </summary>
        ///// <param name="item">Item to test.</param>
        ///// <param name="entityItems">EntityItems with legacy equip table set.</param>
        ///// <returns>Index of item in equip table or -1 if not equipped.</returns>
        //public int GetLegacyEquipIndex(DaggerfallUnityItem item, EntityItems entityItems)
        //{
        //    // Interim use of classic data
        //    //ItemRecord.ItemRecordData itemRecord = item.ItemRecord.ParsedData;
        //    uint[] legacyEquipTable = (GameManager.Instance.PlayerEntity as DaggerfallEntity).Items.LegacyEquipTable;
        //    if (legacyEquipTable == null || legacyEquipTable.Length == 0)
        //        return -1;

        //    // Try to match item RecordID with equipped item IDs
        //    // Item RecordID must be shifted right 8 bits
        //    for (int i = 0; i < legacyEquipTable.Length; i++)
        //    {
        //        if (legacyEquipTable[i] == (item.ItemRecord.RecordRoot.RecordID >> 8))
        //            return i;
        //    }

        //    return -1;
        //}

        /// <summary>
        /// Helps bridge classic item index pair back to item template index.
        /// </summary>
        /// <param name="group">Group enum to retrieve.</param>
        /// <return>Array of group enum values.</return>
        public Array GetEnumArray(ItemGroups group)
        {
            switch (group)
            {
                case ItemGroups.Drugs:
                    return Enum.GetValues(typeof(Drugs));
                case ItemGroups.UselessItems1:
                    return Enum.GetValues(typeof(UselessItems1));
                case ItemGroups.Armor:
                    return Enum.GetValues(typeof(Armor));
                case ItemGroups.Weapons:
                    return Enum.GetValues(typeof(Weapons));
                case ItemGroups.MagicItems:
                    return Enum.GetValues(typeof(MagicItemSubTypes));
                case ItemGroups.Artifacts:
                    return Enum.GetValues(typeof(ArtifactsSubTypes));
                case ItemGroups.MensClothing:
                    return Enum.GetValues(typeof(MensClothing));
                case ItemGroups.Books:
                    return Enum.GetValues(typeof(Books));
                case ItemGroups.Error:
                    return Enum.GetValues(typeof(ERROR));
                case ItemGroups.UselessItems2:
                    return Enum.GetValues(typeof(UselessItems2));
                case ItemGroups.ReligiousItems:
                    return Enum.GetValues(typeof(ReligiousItems));
                case ItemGroups.Maps:
                    return Enum.GetValues(typeof(Maps));
                case ItemGroups.WomensClothing:
                    return Enum.GetValues(typeof(WomensClothing));
                case ItemGroups.Paintings:
                    return Enum.GetValues(typeof(Paintings));
                case ItemGroups.Gems:
                    return Enum.GetValues(typeof(Gems));
                case ItemGroups.PlantIngredients1:
                    return Enum.GetValues(typeof(PlantIngredients1));
                case ItemGroups.PlantIngredients2:
                    return Enum.GetValues(typeof(PlantIngredients2));
                case ItemGroups.CreatureIngredients1:
                    return Enum.GetValues(typeof(CreatureIngredients1));
                case ItemGroups.CreatureIngredients2:
                    return Enum.GetValues(typeof(CreatureIngredients2));
                case ItemGroups.CreatureIngredients3:
                    return Enum.GetValues(typeof(CreatureIngredients3));
                case ItemGroups.MiscellaneousIngredients1:
                    return Enum.GetValues(typeof(MiscellaneousIngredients1));
                case ItemGroups.MetalIngredients:
                    return Enum.GetValues(typeof(MetalIngredients));
                case ItemGroups.MiscellaneousIngredients2:
                    return Enum.GetValues(typeof(MiscellaneousIngredients2));
                case ItemGroups.Transportation:
                    return Enum.GetValues(typeof(Transportation));
                case ItemGroups.Deeds:
                    return Enum.GetValues(typeof(Deeds));
                case ItemGroups.Jewellery:
                    return Enum.GetValues(typeof(Jewellery));
                case ItemGroups.QuestItems:
                    return Enum.GetValues(typeof(QuestItems));
                case ItemGroups.MiscItems:
                    return Enum.GetValues(typeof(MiscItems));
                case ItemGroups.Currency:
                    return Enum.GetValues(typeof(Currency));
                default:
                    throw new Exception("Error: Item group not found.");
            }
        }

        /// <summary>
        /// Assigns basic starting gear to a new character.
        /// </summary>
        public void AssignStartingGear(PlayerEntity playerEntity)
        {
            // Get references
            ItemCollection items = playerEntity.Items;
            ItemEquipTable equipTable = playerEntity.ItemEquipTable;

            // Starting clothes are gender-specific
            DaggerfallUnityItem shortShirt = null;
            DaggerfallUnityItem casualPants = null;
            if (playerEntity.Gender == Genders.Female)
            {
                shortShirt = ItemBuilder.CreateWomensClothing(WomensClothing.Short_shirt_closed, playerEntity.Race, 1);
                casualPants = ItemBuilder.CreateWomensClothing(WomensClothing.Casual_pants, playerEntity.Race);
            }
            else
            {
                shortShirt = ItemBuilder.CreateMensClothing(MensClothing.Short_shirt_closed_top, playerEntity.Race, 1);
                casualPants = ItemBuilder.CreateMensClothing(MensClothing.Casual_pants, playerEntity.Race);
            }

            // Randomise shirt dye and pants variant
            shortShirt.dyeColor = ItemBuilder.RandomClothingDye();
            ItemBuilder.RandomizeVariant(casualPants);

            // Add a wagon
            // This is required for now as shops not currently implemented
            // Wagon is easy to obtain anyway (150g) and most player can affored right out of Privateer's Hold
            // TODO: Remove this once shops can sell this item to players as normal
            items.AddItem(ItemBuilder.CreateItem(ItemGroups.Transportation, (int)Transportation.Small_cart));

            // Add spellbook, all players start with one
            items.AddItem(ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Spellbook));

            // Add and equip clothing
            items.AddItem(shortShirt);
            items.AddItem(casualPants);
            equipTable.EquipItem(shortShirt, true, false);
            equipTable.EquipItem(casualPants, true, false);

            // Always add ebony dagger until biography implemented
            items.AddItem(ItemBuilder.CreateWeapon(Weapons.Dagger, WeaponMaterialTypes.Ebony));

            // Add a cuirass
            items.AddItem(ItemBuilder.CreateArmor(playerEntity.Gender, playerEntity.Race, Armor.Cuirass, ArmorMaterialTypes.Leather));

            // Add alternate weapons
            items.AddItem(ItemBuilder.CreateWeapon(Weapons.Longsword, WeaponMaterialTypes.Steel));
            items.AddItem(ItemBuilder.CreateWeapon(Weapons.Katana, WeaponMaterialTypes.Iron));
            items.AddItem(ItemBuilder.CreateWeapon(Weapons.Staff, WeaponMaterialTypes.Silver));

            // Add some ingredients
            for (int i = 0; i < 10; i++)
            {
                items.AddItem(ItemBuilder.CreateRandomIngredient());
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads item templates from JSON file.
        /// This data was exported from Daggerfall's FALL.EXE file.
        /// </summary>
        void LoadItemTemplates()
        {
            try
            {
                TextAsset templates = Resources.Load<TextAsset>(itemTemplatesFilename) as TextAsset;
                itemTemplates = SaveLoadManager.Deserialize(typeof(List<ItemTemplate>), templates.text) as List<ItemTemplate>;
            }
            catch
            {
                Debug.Log("Could not load ItemTemplates database from Resources. Check file exists and is in correct format.");
            }
        }

        /// <summary>
        /// Makes a unique image key based on item variables.
        /// </summary>
        int MakeImageKey(int color, int archive, int record, bool removeMask)
        {
            int mask = (removeMask) ? 1 : 0;

            // 5 bits for color
            // 9 bits for archive
            // 7 bits for record
            // 1 bits for mask
            return (color << 27) + (archive << 18) + (record << 11) + (mask << 10);
        }

        /// <summary>
        /// Assigns a new Texture2D based on dye colour.
        /// </summary>
        public ImageData ChangeDye(ImageData imageData, DyeColors dye, DyeTargets target)
        {
            imageData.dfBitmap = ImageProcessing.ChangeDye(imageData.dfBitmap, dye, target);
            ImageReader.UpdateTexture(ref imageData);

            return imageData;
        }

        #endregion
    }
}