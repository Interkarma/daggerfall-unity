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

        // This array is used to pick random material values.
        // The array is traversed, subtracting each value from a sum until the sum is less than the next value.
        // Steel through Daedric, or Iron if sum is less than the first value.
        static readonly byte[] materialsByModifier = { 64, 128, 10, 21, 13, 8, 5, 3, 2, 5 };

        // Weight multipliers by material type. Iron through Daedric. Weight is baseWeight * value / 4.
        static readonly short[] weightMultipliersByMaterial = { 4, 5, 4, 4, 3, 4, 4, 2, 4, 5 };

        // Value multipliers by material type. Iron through Daedric. Value is baseValue * ( 3 * value).
        static readonly short[] valueMultipliersByMaterial = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512 };

        // Condition multipliers by material type. Iron through Daedric. MaxCondition is baseMaxCondition * value / 4.
        static readonly short[] conditionMultipliersByMaterial = { 4, 4, 6, 8, 12, 16, 20, 24, 28, 32 };

        // Enchantment point multipliers by material type. Iron through Daedric. Enchantment points is baseEnchanmentPoints * value / 4.
        static readonly short[] enchantmentPointMultipliersByMaterial = { 3, 4, 7, 5, 6, 5, 7, 8, 10, 12 };

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

        #endregion

        #region Public Methods

        public static DyeColors RandomClothingDye()
        {
            return clothingDyes[UnityEngine.Random.Range(0, clothingDyes.Length)];
        }

        /// <summary>
        /// Gets a random material based on player level.
        /// </summary>
        /// <param name="playerLevel">Player level.</param>
        /// <returns>WeaponMaterialTypes.</returns>

        public static WeaponMaterialTypes RandomMaterial(int playerLevel)
        {
            int levelModifier = (playerLevel - 10);

            if (levelModifier >= 0)
                levelModifier *= 2;
            else
                levelModifier *= 4;

            int randomModifier = UnityEngine.Random.Range(0, 256);

            int combinedModifiers = levelModifier + randomModifier;
            combinedModifiers = Mathf.Clamp(combinedModifiers, 0, 256);

            int material = 0; // initialize to iron

            // The higher combinedModifiers is, the higher the material
            while (materialsByModifier[material] < combinedModifiers)
            {
                combinedModifiers -= materialsByModifier[material++];
            }

            return (WeaponMaterialTypes)(material);
        }

        /// <summary>
        /// Gets a random armor material based on player level.
        /// </summary>
        /// <param name="playerLevel">Player level.</param>
        /// <returns>ArmorMaterialTypes.</returns>
        public static ArmorMaterialTypes RandomArmorMaterial(int playerLevel)
        {
            // Random armor material
            int random = UnityEngine.Random.Range(1, 101);

            if (random >= 70)
            {
                if (random >= 90)
                {
                    WeaponMaterialTypes plateMaterial = RandomMaterial(playerLevel);
                    return (ArmorMaterialTypes)(0x0200 + plateMaterial);
                }
                else
                    return ArmorMaterialTypes.Chain;
            }
            else
                return ArmorMaterialTypes.Leather;
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
        public static DaggerfallUnityItem CreateRandomClothing(Genders gender, Races race)
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
            SetRace(newItem, race);

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
            newItem = SetItemPropertiesByMaterial(newItem, material);
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
            // Create random weapon type
            Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ItemGroups.Weapons);
            int groupIndex = UnityEngine.Random.Range(0, enumArray.Length);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.Weapons, groupIndex);

            // Random weapon material
            WeaponMaterialTypes material = RandomMaterial(playerLevel);
            newItem.nativeMaterialValue = (int)material;

            newItem = SetItemPropertiesByMaterial(newItem, material);
            newItem.dyeColor = DaggerfallUnity.Instance.ItemHelper.GetWeaponDyeColor(material);

            // Handle arrows
            if (groupIndex == 18)
            {
                newItem.stackCount = UnityEngine.Random.Range(1, 21);
                newItem.currentCondition = 0; // not sure if this is necessary, but classic does it
            }

            return newItem;
        }

        /// <summary>
        /// Generates armour.
        /// </summary>
        /// <param name="gender">Gender armor is created for.</param>
        /// <param name="race">Race armor is created for.</param>
        /// <param name="armor">Type of armor item to create.</param>
        /// <param name="material">Material of armor.</param>
        /// <param name="variant">Visual variant of armor. If -1, a random variant is chosen.</param>
        /// <returns>DaggerfallUnityItem</returns>
        public static DaggerfallUnityItem CreateArmor(Genders gender, Races race, Armor armor, ArmorMaterialTypes material, int variant = -1)
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

            if (newItem.nativeMaterialValue == (int)ArmorMaterialTypes.Leather)
                newItem.weightInKg /= 2;

            else if (newItem.nativeMaterialValue == (int)ArmorMaterialTypes.Chain)
                newItem.value *= 2;

            else if (newItem.nativeMaterialValue >= (int)ArmorMaterialTypes.Iron)
            {
                int plateMaterial = newItem.nativeMaterialValue - 0x0200;
                newItem = SetItemPropertiesByMaterial(newItem, (WeaponMaterialTypes)plateMaterial);
            }

            newItem.dyeColor = DaggerfallUnity.Instance.ItemHelper.GetArmorDyeColor((ArmorMaterialTypes)newItem.nativeMaterialValue);
            FixLeatherHelm(newItem);

            // Adjust for variant
            if (variant >= 0)
                SetVariant(newItem, variant);
            else
                RandomizeArmorVariant(newItem);

            return newItem;
        }

        /// <summary>
        /// Creates random armor.
        /// </summary>
        /// <param name="playerLevel">Player level for material type.</param>
        /// <param name="gender">Gender armor is created for.</param>
        /// <param name="race">Race armor is created for.</param>
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

            if (newItem.nativeMaterialValue == (int)ArmorMaterialTypes.Leather)
                newItem.weightInKg /= 2;

            else if (newItem.nativeMaterialValue == (int)ArmorMaterialTypes.Chain)
                newItem.value *= 2;

            else if (newItem.nativeMaterialValue >= (int)ArmorMaterialTypes.Iron)
            {
                int plateMaterial = newItem.nativeMaterialValue - 0x0200;
                newItem = SetItemPropertiesByMaterial(newItem, (WeaponMaterialTypes)plateMaterial);
            }

            newItem.dyeColor = DaggerfallUnity.Instance.ItemHelper.GetArmorDyeColor(material);
            FixLeatherHelm(newItem);
            RandomizeArmorVariant(newItem);

            return newItem;
        }

        /// <summary>
        /// Sets properties for a weapon or piece of armor based on its material.
        /// </summary>
        /// <param name="item">Item to have its properties modified.</param>
        /// <param name="material">Material to use to apply properties.</param>
        /// <returns>DaggerfallUnityItem</returns>
        public static DaggerfallUnityItem SetItemPropertiesByMaterial(DaggerfallUnityItem item, WeaponMaterialTypes material)
        {
            item.value *= 3 * valueMultipliersByMaterial[(int)material];
            item.weightInKg = CalculateWeightForMaterial(item, material);
            item.maxCondition *= conditionMultipliersByMaterial[(int)material] / 4;
            item.currentCondition = item.maxCondition;
            item.enchantmentPoints *= enchantmentPointMultipliersByMaterial[(int)material] / 4;

            return item;
        }

        static float CalculateWeightForMaterial(DaggerfallUnityItem item, WeaponMaterialTypes material)
        {
            int quarterKgs = (int)(item.weightInKg * 4);
            float matQuarterKgs = (float)(quarterKgs * weightMultipliersByMaterial[(int)material]) / 4;
            return Mathf.Round(matQuarterKgs) / 4;
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
        /// Sets a random variant of clothing item.
        /// </summary>
        /// <param name="item">Item to randomize variant.</param>
        public static void RandomizeClothingVariant(DaggerfallUnityItem item)
        {
            int totalVariants = item.ItemTemplate.variants;
            SetVariant(item, UnityEngine.Random.Range(0, totalVariants));
        }

        /// <summary>
        /// Sets a random variant of armor item.
        /// </summary>
        /// <param name="item">Item to randomize variant.</param>
        public static void RandomizeArmorVariant(DaggerfallUnityItem item)
        {
            int variant = 0;

            // We only need to pick randomly where there is more than one possible variant. Otherwise we can just pass in 0 to SetVariant and
            // the correct variant will still be chosen.
            if (item.IsOfTemplate(ItemGroups.Armor, (int)Armor.Cuirass) && (item.nativeMaterialValue >= (int)ArmorMaterialTypes.Iron))
            {
                variant = UnityEngine.Random.Range(1, 4);
            }
            else if (item.IsOfTemplate(ItemGroups.Armor, (int)Armor.Greaves))
            {
                if (item.nativeMaterialValue == (int)ArmorMaterialTypes.Leather)
                    variant = UnityEngine.Random.Range(0, 2);
                else if (item.nativeMaterialValue >= (int)ArmorMaterialTypes.Iron)
                    variant = UnityEngine.Random.Range(2, 6);
            }
            else if (item.IsOfTemplate(ItemGroups.Armor, (int)Armor.Left_Pauldron) || item.IsOfTemplate(ItemGroups.Armor, (int)Armor.Right_Pauldron))
            {
                if (item.nativeMaterialValue >= (int)ArmorMaterialTypes.Iron)
                    variant = UnityEngine.Random.Range(1, 4);
            }
            else if (item.IsOfTemplate(ItemGroups.Armor, (int)Armor.Helm))
                variant = UnityEngine.Random.Range(0, item.ItemTemplate.variants);

            SetVariant(item, variant);
        }

        /// <summary>
        /// Set leather helms to use chain dye.
        /// Daggerfall seems to do this also as "leather" helms have the chain tint in-game.
        /// Might need to revisit this later.
        /// </summary>
        public static void FixLeatherHelm(DaggerfallUnityItem item)
        {
            if (item.TemplateIndex == (int)Armor.Helm && (ArmorMaterialTypes)item.nativeMaterialValue == ArmorMaterialTypes.Leather)
                item.dyeColor = DyeColors.Chain;
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
                    variant = 1;
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