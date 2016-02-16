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
        Dictionary<ContainerTypes, ImageData> containerImages = new Dictionary<ContainerTypes, ImageData>();

        #endregion

        #region Constructors

        public ItemHelper()
        {
            LoadItemTemplates();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Quickly get template index.
        /// </summary>
        public int GetTemplateIndex(DaggerfallUnityItem item)
        {
            // Interim use of classic data
            ItemRecord.ItemRecordData itemRecord = item.ItemRecord.ParsedData;

            Array values = DaggerfallUnity.Instance.ItemHelper.GetEnumArray((ItemGroups)itemRecord.category1);
            return Convert.ToInt32(values.GetValue(itemRecord.category2));
        }

        /// <summary>
        /// Gets how item is held in the hands.
        /// </summary>
        public ItemHands GetItemHands(DaggerfallUnityItem item)
        {
            // Interim use of classic data
            ItemRecord.ItemRecordData itemRecord = item.ItemRecord.ParsedData;

            // Check if weapon or armor (for shields)
            if ((ItemGroups)itemRecord.category1 != ItemGroups.Weapons ||
                (ItemGroups)itemRecord.category1 != ItemGroups.Armor)
            {
                return ItemHands.None;
            }

            // Get template index
            int templateIndex = GetTemplateIndex(item);

            // Compare against supported weapon types
            switch((Weapons)templateIndex)
            {
                // These weapons can be used in either hand
                case Weapons.Dagger:
                case Weapons.Tanto:
                case Weapons.Shortsword:
                case Weapons.Wakazashi:
                case Weapons.Broadsword:
                case Weapons.Saber:
                case Weapons.Longsword:
                case Weapons.Katana:
                case Weapons.Battle_Axe:
                case Weapons.Mace:
                    return ItemHands.Either;

                // Two-handed weapons
                case Weapons.Claymore:
                case Weapons.Dai_Katana:
                case Weapons.War_Axe:
                case Weapons.Staff:
                case Weapons.Flail:
                case Weapons.Warhammer:
                case Weapons.Short_Bow:
                case Weapons.Long_Bow:
                    return ItemHands.Both;
            }

            // Compare against supported armor types
            switch ((Armor)templateIndex)
            {
                case Armor.Buckler:
                case Armor.Round_Shield:
                case Armor.Kite_Shield:
                case Armor.Tower_Shield:
                    return ItemHands.LeftOnly;
            }

            // Nothing found
            return ItemHands.None;
        }

        public ItemTemplate GetItemTemplate(DaggerfallUnityItem item)
        {
            return GetItemTemplate((ItemGroups)item.ItemRecord.ParsedData.category1, item.ItemRecord.ParsedData.category2);
        }

        /// <summary>
        /// Resolves full item name using parameters like %it and material type.
        /// </summary>
        public string ResolveItemName(DaggerfallUnityItem item)
        {
            // Interim use of classic data
            ItemRecord.ItemRecordData itemRecord = item.ItemRecord.ParsedData;

            // Start with base name
            string result = itemRecord.name;

            // Get item template
            ItemGroups group = (ItemGroups)itemRecord.category1;
            ItemTemplate template = GetItemTemplate(group, itemRecord.category2);

            // Resolve %it parameter
            result = result.Replace("%it", template.name);

            // Resolve weapon material
            if (group == ItemGroups.Weapons)
            {
                WeaponMaterialTypes weaponMaterial = (WeaponMaterialTypes)itemRecord.material;
                string materialName = DaggerfallUnity.Instance.TextProvider.GetWeaponMaterialName(weaponMaterial);
                result = string.Format("{0} {1}", materialName, result);
            }

            // Resolve armor material
            if (group == ItemGroups.Armor)
            {
                ArmorMaterialTypes armorMaterial = (ArmorMaterialTypes)itemRecord.material;
                string materialName = DaggerfallUnity.Instance.TextProvider.GetArmorMaterialName(armorMaterial);
                result = string.Format("{0} {1}", materialName, result);
            }

            return result;
        }

        /// <summary>
        /// Gets inventory/equip image for specified item.
        /// Image will be cached based on material and hand for faster subsequent fetches.
        /// </summary>
        /// <param name="item">Item fetch image for.</param>
        /// <param name="variant">Variant of image (e.g. hood up/down or hand left/right).</param>
        /// <param name="ignoreMask">Remove background mask.</param>
        /// <returns>ImageData.</returns>
        public ImageData GetItemImage(DaggerfallUnityItem item, int variant = 0, bool ignoreMask = false)
        {
            // Get colour
            int color = item.ItemRecord.ParsedData.color;

            // Get archive and record indices
            int bitfield = (int)item.ItemRecord.ParsedData.image1;
            int archive = bitfield >> 7;
            int record = (bitfield & 0x7f) + variant;

            // Get unique key
            int key = MakeImageKey(color, variant, archive, record, ignoreMask);

            // Get existing icon if in cache
            if (itemImages.ContainsKey(key))
                return itemImages[key];

            // Load image data
            string filename = TextureFile.IndexToFileName(archive);
            ImageData data = ImageReader.GetImageData(filename, record, 0, true, false);
            if (data.type == ImageTypes.None)
                throw new Exception("GetItemImage() could not load image data.");

            // Change dye or just update texture
            ItemGroups group = item.ItemGroup;
            DyeColors dye = (DyeColors)color;
            if (group == ItemGroups.Weapons || group == ItemGroups.Armor)
                data = ChangeDye(data, dye, DyeTargets.WeaponsAndArmor, ignoreMask);
            else if (item.ItemGroup == ItemGroups.MensClothing || item.ItemGroup == ItemGroups.WomensClothing)
                data = ChangeDye(data, dye, DyeTargets.Clothing, ignoreMask);
            else
                ImageReader.UpdateTexture(ref data);

            // Add to cache
            itemImages.Add(key, data);

            return data;
        }

        /// <summary>
        /// Gets icon for a container object, such as the wagon.
        /// </summary>
        /// <param name="type">Container type.</param>
        /// <returns>ImageData.</returns>
        public ImageData GetContainerImage(ContainerTypes type)
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
        /// Converts native Daggerfall weapon and armor material types to generic Daggerfall Unity MetalType.
        /// </summary>
        /// <param name="item">Item to convert material to metal type.</param>
        /// <returns>MetalType.</returns>
        public MetalTypes ConvertItemMaterialToAPIMetalType(DaggerfallUnityItem item)
        {
            // Get item group
            ItemGroups group = (ItemGroups)item.ItemRecord.ParsedData.category1;

            // Determine metal type
            if (group == ItemGroups.Weapons)
            {
                WeaponMaterialTypes weaponMaterial = (WeaponMaterialTypes)item.ItemRecord.ParsedData.material;
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
            else if (group == ItemGroups.Armor)
            {
                ArmorMaterialTypes armorMaterial = (ArmorMaterialTypes)item.ItemRecord.ParsedData.material;
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
        /// Checks legacy item ID against legacy equip index to determine if item is equipped to a slot.
        /// Must have previously used SetLegacyEquipTable to EntityItems provided.
        /// </summary>
        /// <param name="item">Item to test.</param>
        /// <param name="entityItems">EntityItems with legacy equip table set.</param>
        /// <returns>Index of item in equip table or -1 if not equipped.</returns>
        public int GetLegacyEquipIndex(DaggerfallUnityItem item, EntityItems entityItems)
        {
            // Interim use of classic data
            ItemRecord.ItemRecordData itemRecord = item.ItemRecord.ParsedData;
            uint[] legacyEquipTable = (GameManager.Instance.PlayerEntity as DaggerfallEntity).Items.LegacyEquipTable;
            if (legacyEquipTable == null || legacyEquipTable.Length == 0)
                return -1;

            // Try to match item RecordID with equipped item IDs
            // Item RecordID must be shifted right 8 bits
            for (int i = 0; i < legacyEquipTable.Length; i++)
            {
                if (legacyEquipTable[i] == (item.ItemRecord.RecordRoot.RecordID >> 8))
                    return i;
            }

            return -1;
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
        /// Helps bridge classic item index pair back to item template index.
        /// </summary>
        /// <param name="group">Group enum to retrieve.</param>
        /// <return>Array of group enum values.</return>
        Array GetEnumArray(ItemGroups group)
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
        /// Gets classic item template data using group and index.
        /// </summary>
        ItemTemplate GetItemTemplate(ItemGroups group, int index)
        {
            Array values = GetEnumArray(group);
            if (index < 0 || index >= values.Length)
            {
                string message = string.Format("Item index out of range: Group={0} Index={1}", group.ToString(), index);
                Debug.Log(message);
                return new ItemTemplate();
            }

            int templateIndex = Convert.ToInt32(values.GetValue(index));

            return itemTemplates[templateIndex];
        }

        /// <summary>
        /// Makes a unique image key based on item variables.
        /// </summary>
        int MakeImageKey(int color, int variant, int archive, int record, bool ignoreMask)
        {
            int mask = (ignoreMask) ? 1 : 0;

            // 5 bits for color
            // 3 bits for variant
            // 9 bits for archive
            // 7 bits for record
            // 1 bits for mask
            return (color << 27) + (variant << 24) + (archive << 15) + (record << 8) + (mask << 7);
        }

        /// <summary>
        /// Assigns a new Texture2D based on dye colour.
        /// </summary>
        ImageData ChangeDye(ImageData imageData, DyeColors dye, DyeTargets target, bool ignoreMask = false)
        {
            imageData.dfBitmap = ImageProcessing.ChangeDye(imageData.dfBitmap, dye, target, ignoreMask);
            ImageReader.UpdateTexture(ref imageData);

            return imageData;
        }

        #endregion
    }
}