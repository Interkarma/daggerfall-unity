// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: InconsolableCellist
//
// Notes:
//

using System;
using UnityEngine;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallConnect.FallExe;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// Generates new items for various game systems.
    /// This helper still under development.
    /// </summary>
    public static class ItemBuilder
    {
        #region Data

        const int firstFemaleArchive = 245;
        const int firstMaleArchive = 249;

        static ArmorMaterialTypes[] armorMaterialList = new ArmorMaterialTypes[]
        {
            ArmorMaterialTypes.Leather,
            ArmorMaterialTypes.Chain,
            ArmorMaterialTypes.Iron,
            ArmorMaterialTypes.Steel,
            ArmorMaterialTypes.Silver,
            ArmorMaterialTypes.Elven,
            ArmorMaterialTypes.Dwarven,
            ArmorMaterialTypes.Mithril,
            ArmorMaterialTypes.Adamantium,
            ArmorMaterialTypes.Ebony,
            ArmorMaterialTypes.Orcish,
            ArmorMaterialTypes.Daedric,
        };

        static WeaponMaterialTypes[] weaponMaterialList = new WeaponMaterialTypes[]
        {
            WeaponMaterialTypes.Iron,
            WeaponMaterialTypes.Steel,
            WeaponMaterialTypes.Silver,
            WeaponMaterialTypes.Elven,
            WeaponMaterialTypes.Dwarven,
            WeaponMaterialTypes.Mithril,
            WeaponMaterialTypes.Adamantium,
            WeaponMaterialTypes.Ebony,
            WeaponMaterialTypes.Orcish,
            WeaponMaterialTypes.Daedric,
        };

        private enum BodyMorphology
        {
            Argonian = 0,
            Elf = 1,
            Human = 2,
            Khajiit = 3,
        }

        static DyeColors[] clothingDyes = new DyeColors[]
        {
            DyeColors.Blue,
            DyeColors.Grey,
            DyeColors.Red,
            DyeColors.DarkBrown,
            DyeColors.Purple,
            DyeColors.LightBrown,
            DyeColors.White,
            DyeColors.Aquamarine,
            DyeColors.Yellow,
            DyeColors.Green,
        };

        static DyeColors[] metalDyes = new DyeColors[]
        {
            DyeColors.Iron,
            DyeColors.Steel,
            DyeColors.Chain,
            DyeColors.Unchanged,
            DyeColors.SilverOrElven,
            DyeColors.Dwarven,
            DyeColors.Mithril,
            DyeColors.Adamantium,
            DyeColors.Ebony,
            DyeColors.Orcish,
            DyeColors.Daedric,
        };

        #endregion

        #region Public Methods

        public static DyeColors RandomClothingDye()
        {
            return clothingDyes[UnityEngine.Random.Range(0, clothingDyes.Length)];
        }

        public static DyeColors RandomMetalDye()
        {
            return metalDyes[UnityEngine.Random.Range(0, metalDyes.Length)];
        }

        /// <summary>
        /// Gets a random quality index within range.
        /// This is a kludge way of distributing material types for now.
        /// </summary>
        /// <param name="playerLevel">Player level.</param>
        /// <param name="peakLevel">Level at which weight is removed.</param>
        /// <param name="weight">Negative index weight per level below peak.</param>
        /// <returns>Weighted index.</returns>
        public static int RandomQualityIndex(int playerLevel, int min, int max, int peakLevel = 10, float weight = 0.7f)
        {
            // Start with equal chance for any index
            int index = UnityEngine.Random.Range(min, max);

            // Weight pulls quality down below peak level
            int bias = 0;
            if (playerLevel < peakLevel)
                bias = (int)((peakLevel - playerLevel) * weight);

            // Final material index
            index = Mathf.Clamp(index - bias, min, max);

            return index;
        }

        /// <summary>
        /// Gets a random weapon material.
        /// This is a kludge way of distributing material types for now.
        /// </summary>
        /// <param name="playerLevel">Player level.</param>
        /// <returns>WeaponMaterialTypes.</returns>
        public static WeaponMaterialTypes RandomWeaponMaterial(int playerLevel)
        {
            return weaponMaterialList[RandomQualityIndex(playerLevel, 0, weaponMaterialList.Length)];
        }

        /// <summary>
        /// Gets a random armor material.
        /// This is a kludge way of distributing material types for now.
        /// </summary>
        /// <param name="playerLevel">Player level.</param>
        /// <returns>ArmorMaterialTypes.</returns>
        public static ArmorMaterialTypes RandomArmorMaterial(int playerLevel)
        {
            return armorMaterialList[RandomQualityIndex(playerLevel, 0, armorMaterialList.Length)];
        }

        /// <summary>
        /// Creates a generic item from group and template index.
        /// </summary>
        /// <param name="itemGroup">Item group.</param>
        /// <param name="itemIndex">Template index.</param>
        /// <returns>DaggerfallUnityItem.</returns>
        public static DaggerfallUnityItem CreateItem(ItemGroups itemGroup, int templateIndex)
        {
            // Create item
            int groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(itemGroup, templateIndex);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(itemGroup, groupIndex);

            return newItem;
        }

        /// <summary>
        /// Generates men's clothing.
        /// </summary>
        /// <param name="item">Item type to generate.</param>
        /// <param name="race">Race of player.</param>
        /// <param name="variant">Variant to use.</param>
        /// <param name="dye">Dye to use</param>
        /// <returns>DaggerfallUnityItem.</returns>
        public static DaggerfallUnityItem CreateMensClothing(MensClothing item, Races race, int variant = 0, DyeColors dye = DyeColors.Blue)
        {
            // Create item
            int groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.MensClothing, (int)item);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.MensClothing, groupIndex);

            // Set race, variant, dye
            SetRace(newItem, race);
            SetVariant(newItem, variant);
            newItem.dyeColor = dye;

            return newItem;
        }

        /// <summary>
        /// Generates women's clothing.
        /// </summary>
        /// <param name="item">Item type to generate.</param>
        /// <param name="race">Race of player.</param>
        /// <param name="variant">Variant to use.</param>
        /// <param name="dye">Dye to use</param>
        /// <returns>DaggerfallUnityItem.</returns>
        public static DaggerfallUnityItem CreateWomensClothing(WomensClothing item, Races race, int variant = 0, DyeColors dye = DyeColors.Blue)
        {
            // Create item
            int groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.WomensClothing, (int)item);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.WomensClothing, groupIndex);

            // Set race, variant, dye
            SetRace(newItem, race);
            SetVariant(newItem, variant);
            newItem.dyeColor = dye;

            return newItem;
        }

        /// <summary>
        /// Creates a new item of random clothing.
        /// </summary>
        /// <param name="gender">Gender of player</param>
        /// <returns>DaggerfallUnityItem.</returns>
        public static DaggerfallUnityItem CreateRandomClothing(Genders gender)
        {
            // Create random clothing by gender
            DaggerfallUnityItem newItem;
            if (gender == Genders.Male)
            {
                Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ItemGroups.MensClothing);
                int groupIndex = UnityEngine.Random.Range(0, enumArray.Length);
                newItem = new DaggerfallUnityItem(ItemGroups.MensClothing, groupIndex);
            }
            else
            {
                Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ItemGroups.WomensClothing);
                int groupIndex = UnityEngine.Random.Range(0, enumArray.Length);
                newItem = new DaggerfallUnityItem(ItemGroups.WomensClothing, groupIndex);
            }

            // Random dye colour
            newItem.dyeColor = RandomClothingDye();

            // Random variant
            SetVariant(newItem, UnityEngine.Random.Range(0, newItem.TotalVariants));

            return newItem;
        }

        /// <summary>
        /// Creates a new random book
        /// </summary>
        /// <returns>DaggerfallUnityItem.</returns>
        public static DaggerfallUnityItem CreateRandomBook()
        {
            Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ItemGroups.Books);
            DaggerfallUnityItem book = new DaggerfallUnityItem(ItemGroups.Books, Array.IndexOf(enumArray, Books.Book));

            // TODO: FIXME: I don't know what the higher order bits are for the message field. I just know that message & 0xFF encodes
            // the ID of the book. Thus the books that are randomly generated are missing information that the actual game provides.
            // -IC112016
            book.message = DaggerfallUnity.Instance.ItemHelper.getRandomBookID();
            return book;
        }



        /// <summary>
        /// Creates a new random religious item.
        /// </summary>
        /// <returns>DaggerfallUnityItem.</returns>
        public static DaggerfallUnityItem CreateRandomReligiousItem()
        {
            Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ItemGroups.ReligiousItems);
            int groupIndex = UnityEngine.Random.Range(0, enumArray.Length);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.ReligiousItems, groupIndex);

            return newItem;
        }

        /// <summary>
        /// Generates a weapon.
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        public static DaggerfallUnityItem CreateWeapon(Weapons weapon, WeaponMaterialTypes material)
        {
            // Create item
            int groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.Weapons, (int)weapon);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.Weapons, groupIndex);

            // Adjust material
            newItem.nativeMaterialValue = (int)material;
            newItem.dyeColor = DaggerfallUnity.Instance.ItemHelper.GetWeaponDyeColor(material);

            return newItem;
        }

        /// <summary>
        /// Creates random weapon.
        /// </summary>
        /// <param name="playerLevel">Player level for material type.</param>
        /// <returns>DaggerfallUnityItem</returns>
        public static DaggerfallUnityItem CreateRandomWeapon(int playerLevel)
        {
            // Create random weapon type (excluding bows and arrows for now)
            Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ItemGroups.Weapons);
            int groupIndex = UnityEngine.Random.Range(0, enumArray.Length - 3);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.Weapons, groupIndex);

            // Random weapon material
            WeaponMaterialTypes material = RandomWeaponMaterial(playerLevel);
            newItem.nativeMaterialValue = (int)material;
            newItem.dyeColor = DaggerfallUnity.Instance.ItemHelper.GetWeaponDyeColor(material);

            return newItem;
        }

        /// <summary>
        /// Generates armour.
        /// </summary>
        /// <param name="armor">Armor item.</param>
        /// <param name="material">Material.</param>
        /// <returns>DaggerfallUnityItem</returns>
        public static DaggerfallUnityItem CreateArmor(Genders gender, Races race, Armor armor, ArmorMaterialTypes material, int variant = 0)
        {
            // Create item
            int groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.Armor, (int)armor);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.Armor, groupIndex);

            // Adjust for gender
            if (gender == Genders.Female)
                newItem.PlayerTextureArchive = firstFemaleArchive;
            else
                newItem.PlayerTextureArchive = firstMaleArchive;

            // Adjust for body morphology
            SetRace(newItem, race);

            // Adjust material
            newItem.nativeMaterialValue = (int)material;
            newItem.dyeColor = DaggerfallUnity.Instance.ItemHelper.GetArmorDyeColor(material);

            // Adjust for variant
            SetVariant(newItem, variant);

            return newItem;
        }

        /// <summary>
        /// Creates random armor.
        /// </summary>
        /// <param name="playerLevel">Player level for material type.</param>
        /// <returns>DaggerfallUnityItem</returns>
        public static DaggerfallUnityItem CreateRandomArmor(int playerLevel, Genders gender, Races race)
        {
            // Create random armor type
            Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ItemGroups.Armor);
            int groupIndex = UnityEngine.Random.Range(0, enumArray.Length);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.Armor, groupIndex);

            // Adjust for gender
            if (gender == Genders.Female)
                newItem.PlayerTextureArchive = firstFemaleArchive;
            else
                newItem.PlayerTextureArchive = firstMaleArchive;

            // Adjust for body morphology
            SetRace(newItem, race);

            // Random armor material
            ArmorMaterialTypes material = RandomArmorMaterial(playerLevel);
            newItem.nativeMaterialValue = (int)material;
            newItem.dyeColor = DaggerfallUnity.Instance.ItemHelper.GetArmorDyeColor(material);

            // Random variant
            SetVariant(newItem, UnityEngine.Random.Range(0, newItem.TotalVariants));

            return newItem;
        }

        /// <summary>
        /// Creates a random ingredient from any of the ingredient groups.
        /// Passing a non-ingredient group will return null.
        /// </summary>
        /// <param name="group">Ingedient group.</param>
        /// <returns>DaggerfallUnityItem</returns>
        public static DaggerfallUnityItem CreateRandomIngredient(ItemGroups ingredientGroup)
        {
            int groupIndex;
            Array enumArray;
            switch (ingredientGroup)
            {
                case ItemGroups.CreatureIngredients1:
                case ItemGroups.CreatureIngredients2:
                case ItemGroups.CreatureIngredients3:
                case ItemGroups.MetalIngredients:
                case ItemGroups.MiscellaneousIngredients1:
                case ItemGroups.MiscellaneousIngredients2:
                case ItemGroups.PlantIngredients1:
                case ItemGroups.PlantIngredients2:
                    enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ingredientGroup);
                    groupIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    break;
                default:
                    return null;
            }

            // Create item
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ingredientGroup, groupIndex);

            return newItem;
        }

        /// <summary>
        /// Creates a random ingredient from a random ingredient group.
        /// </summary>
        /// <returns>DaggerfallUnityItem</returns>
        public static DaggerfallUnityItem CreateRandomIngredient()
        {
            // Randomise ingredient group
            ItemGroups itemGroup = ItemGroups.None;
            int group = UnityEngine.Random.Range(0, 7);
            Array enumArray;
            switch (group)
            {
                case 0:
                    itemGroup = ItemGroups.CreatureIngredients1;
                    break;
                case 1:
                    itemGroup = ItemGroups.CreatureIngredients2;
                    break;
                case 2:
                    itemGroup = ItemGroups.CreatureIngredients3;
                    break;
                case 3:
                    itemGroup = ItemGroups.MetalIngredients;
                    break;
                case 4:
                    itemGroup = ItemGroups.MiscellaneousIngredients1;
                    break;
                case 5:
                    itemGroup = ItemGroups.MiscellaneousIngredients2;
                    break;
                case 6:
                    itemGroup = ItemGroups.PlantIngredients1;
                    break;
                case 7:
                    itemGroup = ItemGroups.PlantIngredients2;
                    break;
                default:
                    return null;
            }

            // Randomise ingredient within group
            enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(itemGroup);
            int groupIndex = UnityEngine.Random.Range(0, enumArray.Length);

            // Create item
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(itemGroup, groupIndex);

            return newItem;
        }

        /// <summary>
        /// Generates gold pieces.
        /// </summary>
        /// <param name="amount">Total number of gold pieces in stack.</param>
        /// <returns></returns>
        public static DaggerfallUnityItem CreateGoldPieces(int amount)
        {
            DaggerfallUnityItem newItem = CreateItem(ItemGroups.Currency, (int)Currency.Gold_pieces);
            newItem.stackCount = amount;

            return newItem;
        }

        /// <summary>
        /// Sets a random variant of item.
        /// </summary>
        /// <param name="item">Item to randomize variant.</param>
        public static void RandomizeVariant(DaggerfallUnityItem item)
        {
            int totalVariants = item.ItemTemplate.variants;
            SetVariant(item, UnityEngine.Random.Range(0, totalVariants));
        }

        /// <summary>
        /// Promote leather helms to chain.
        /// Daggerfall seems to do this also as "leather" helms have the chain tint in-game.
        /// Is this why Daggerfall intentionally leaves off the material type from helms and shields?
        /// Might need to revisit this later.
        /// </summary>
        public static void FixLeatherHelm(DaggerfallUnityItem item)
        {
            if (item.TemplateIndex == (int)Armor.Helm && (ArmorMaterialTypes)item.nativeMaterialValue == ArmorMaterialTypes.Leather)
            {
                // Change material to chain and leave other stats the same
                item.nativeMaterialValue = (int)ArmorMaterialTypes.Chain;

                // Change dye to chain if not set
                if (item.dyeColor == DyeColors.Unchanged)
                    item.dyeColor = DyeColors.Chain;
            }
        }

        #endregion

        #region Private Methods

        static void SetRace(DaggerfallUnityItem item, Races race)
        {
            int offset = (int)GetBodyMorphology(race);
            item.PlayerTextureArchive += offset;
        }

        static void SetVariant(DaggerfallUnityItem item, int variant)
        {
            // Range check
            int totalVariants = item.ItemTemplate.variants;
            if (variant < 0 || variant >= totalVariants)
                return;

            // Clamp to appropriate variant based on material family
            if (item.IsOfTemplate(ItemGroups.Armor, (int)Armor.Cuirass))
            {
                if (item.nativeMaterialValue == (int)ArmorMaterialTypes.Leather)
                    variant = 0;
                else if (item.nativeMaterialValue == (int)ArmorMaterialTypes.Chain || item.nativeMaterialValue == (int)ArmorMaterialTypes.Chain2)
                    variant = 4;
                else
                    variant = Mathf.Clamp(variant, 1, 3);
            }
            else if (item.IsOfTemplate(ItemGroups.Armor, (int)Armor.Greaves))
            {
                if (item.nativeMaterialValue == (int)ArmorMaterialTypes.Leather)
                    variant = Mathf.Clamp(variant, 0, 1);
                else if (item.nativeMaterialValue == (int)ArmorMaterialTypes.Chain || item.nativeMaterialValue == (int)ArmorMaterialTypes.Chain2)
                    variant = 6;
                else
                    variant = Mathf.Clamp(variant, 2, 5);
            }
            else if (item.IsOfTemplate(ItemGroups.Armor, (int)Armor.Left_Pauldron) || item.IsOfTemplate(ItemGroups.Armor, (int)Armor.Right_Pauldron))
            {
                if (item.nativeMaterialValue == (int)ArmorMaterialTypes.Leather)
                    variant = 0;
                else if (item.nativeMaterialValue == (int)ArmorMaterialTypes.Chain || item.nativeMaterialValue == (int)ArmorMaterialTypes.Chain2)
                    variant = 4;
                else
                    variant = Mathf.Clamp(variant, 1, 3);
            }
            else if (item.IsOfTemplate(ItemGroups.Armor, (int)Armor.Gauntlets))
            {
                if (item.nativeMaterialValue == (int)ArmorMaterialTypes.Leather)
                    variant = 0;
                else
                    variant = 1;
            }
            else if (item.IsOfTemplate(ItemGroups.Armor, (int)Armor.Boots))
            {
                if (item.nativeMaterialValue == (int)ArmorMaterialTypes.Leather)
                    variant = 0;
                else
                    variant = Mathf.Clamp(variant, 1, 2);
            }

            // Store variant
            item.CurrentVariant = variant;
        }

        static BodyMorphology GetBodyMorphology(Races race)
        {
            switch (race)
            {
                case Races.Argonian:
                    return BodyMorphology.Argonian;

                case Races.DarkElf:
                case Races.HighElf:
                case Races.WoodElf:
                    return BodyMorphology.Elf;

                case Races.Breton:
                case Races.Nord:
                case Races.Redguard:
                    return BodyMorphology.Human;

                case Races.Khajiit:
                    return BodyMorphology.Khajiit;

                default:
                    throw new Exception("GetBodyMorphology() encountered unsupported race value.");
            }
        }

        #endregion
    }
}