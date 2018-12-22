// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: InconsolableCellist
//
// Notes:
//

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallConnect.FallExe;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;

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

        // Enchantment point/gold value data for item powers
        static readonly int[] extraSpellPtsEnchantPts = { 0x1F4, 0x1F4, 0x1F4, 0x1F4, 0xC8, 0xC8, 0xC8, 0x2BC, 0x320, 0x384, 0x3E8 };
        static readonly int[] potentVsEnchantPts = { 0x320, 0x384, 0x3E8, 0x4B0 };
        static readonly int[] regensHealthEnchantPts = { 0x0FA0, 0x0BB8, 0x0BB8 };
        static readonly int[] vampiricEffectEnchantPts = { 0x7D0, 0x3E8 };
        static readonly int[] increasedWeightAllowanceEnchantPts = { 0x190, 0x258 };
        static readonly int[] improvesTalentsEnchantPts = { 0x1F4, 0x258, 0x258 };
        static readonly int[] goodRepWithEnchantPts = { 0x3E8, 0x3E8, 0x3E8, 0x3E8, 0x3E8, 0x1388 };
        static readonly int[][] enchantmentPtsForItemPowerArrays = { null, null, null, extraSpellPtsEnchantPts, potentVsEnchantPts, regensHealthEnchantPts,
                                                                    vampiricEffectEnchantPts, increasedWeightAllowanceEnchantPts, null, null, null, null, null,
                                                                    improvesTalentsEnchantPts, goodRepWithEnchantPts};
        static readonly ushort[] enchantmentPointCostsForNonParamTypes = { 0, 0x0F448, 0x0F63C, 0x0FF9C, 0x0FD44, 0, 0, 0, 0x384, 0x5DC, 0x384, 0x64, 0x2BC };

        private enum BodyMorphology
        {
            Argonian = 0,
            Elf = 1,
            Human = 2,
            Khajiit = 3,
        }

        static DyeColors[] clothingDyes = {
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
        /// <param name="templateIndex">Template index.</param>
        /// <returns>DaggerfallUnityItem.</returns>
        public static DaggerfallUnityItem CreateItem(ItemGroups itemGroup, int templateIndex)
        {
            // Create item
            int groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(itemGroup, templateIndex);
            if (groupIndex == -1)
            {
                Debug.LogErrorFormat("ItemBuilder.CreateItem() encountered an item with an invalid GroupIndex. Check you're passing 'template index' matching a value in ItemEnums - e.g. (int)Weapons.Dagger NOT a 'group index' (e.g. 0).");
                return null;
            }
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(itemGroup, groupIndex);

            return newItem;
        }

        /// <summary>
        /// Generates men's clothing.
        /// </summary>
        /// <param name="item">Item type to generate.</param>
        /// <param name="race">Race of player.</param>
        /// <param name="variant">Variant to use. If not set, a random variant will be selected.</param>
        /// <param name="dye">Dye to use</param>
        /// <returns>DaggerfallUnityItem.</returns>
        public static DaggerfallUnityItem CreateMensClothing(MensClothing item, Races race, int variant = -1, DyeColors dye = DyeColors.Blue)
        {
            // Create item
            int groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.MensClothing, (int)item);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.MensClothing, groupIndex);

            // Random variant
            if (variant < 0)
                variant = UnityEngine.Random.Range(0, newItem.ItemTemplate.variants);

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
        /// <param name="variant">Variant to use. If not set, a random variant will be selected.</param>
        /// <param name="dye">Dye to use</param>
        /// <returns>DaggerfallUnityItem.</returns>
        public static DaggerfallUnityItem CreateWomensClothing(WomensClothing item, Races race, int variant = -1, DyeColors dye = DyeColors.Blue)
        {
            // Create item
            int groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.WomensClothing, (int)item);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.WomensClothing, groupIndex);

            // Random variant
            if (variant < 0)
                variant = UnityEngine.Random.Range(0, newItem.ItemTemplate.variants);

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
            DaggerfallUnityItem book = new DaggerfallUnityItem(ItemGroups.Books, Array.IndexOf(enumArray, Books.Book0));

            // TODO: Change DaggerfallUnityItem.message from int to ushort
            book.message = DaggerfallUnity.Instance.ItemHelper.getRandomBookID();
            book.CurrentVariant = UnityEngine.Random.Range(0, book.TotalVariants);
            // Update item value for this book.
            BookFile bookFile = new BookFile();
            string name = BookFile.messageToBookFilename(book.message);
            if (!BookReplacement.TryImportBook(name, bookFile))
                bookFile.OpenBook(DaggerfallUnity.Instance.Arena2Path, name);
            book.value = bookFile.Price;
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
        /// Creates a new random gem.
        /// </summary>
        /// <returns>DaggerfallUnityItem.</returns>
        public static DaggerfallUnityItem CreateRandomGem()
        {
            Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ItemGroups.Gems);
            int groupIndex = UnityEngine.Random.Range(0, enumArray.Length);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.Gems, groupIndex);

            return newItem;
        }

        /// <summary>
        /// Creates a new random jewellery.
        /// </summary>
        /// <returns>DaggerfallUnityItem.</returns>
        public static DaggerfallUnityItem CreateRandomJewellery()
        {
            Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ItemGroups.Jewellery);
            int groupIndex = UnityEngine.Random.Range(0, enumArray.Length);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.Jewellery, groupIndex);

            return newItem;
        }

        /// <summary>
        /// Creates a new random drug.
        /// </summary>
        /// <returns>DaggerfallUnityItem.</returns>
        public static DaggerfallUnityItem CreateRandomDrug()
        {
            Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(ItemGroups.Drugs);
            int groupIndex = UnityEngine.Random.Range(0, enumArray.Length);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.Drugs, groupIndex);

            return newItem;
        }

        /// <summary>
        /// Generates a weapon.
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="material">Ignored for arrows</param>
        /// <returns></returns>
        public static DaggerfallUnityItem CreateWeapon(Weapons weapon, WeaponMaterialTypes material)
        {
            // Create item
            int groupIndex = DaggerfallUnity.Instance.ItemHelper.GetGroupIndex(ItemGroups.Weapons, (int)weapon);
            DaggerfallUnityItem newItem = new DaggerfallUnityItem(ItemGroups.Weapons, groupIndex);

            if (weapon == Weapons.Arrow)
            {   // Handle arrows
                newItem.stackCount = UnityEngine.Random.Range(1, 21);
                newItem.currentCondition = 0; // not sure if this is necessary, but classic does it
            }
            else
            {   // Adjust material
                newItem.nativeMaterialValue = (int)material;
                newItem = SetItemPropertiesByMaterial(newItem, material);
                newItem.dyeColor = DaggerfallUnity.Instance.ItemHelper.GetWeaponDyeColor(material);
            }
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
        /// Creates random magic item in same manner as classic.
        /// </summary>
        /// <returns>DaggerfallUnityItem</returns>
        public static DaggerfallUnityItem CreateRandomMagicItem(int playerLevel, Genders gender, Races race)
        {
            byte[] itemGroups0 = { 2, 3, 6, 10, 12, 14, 25 };
            byte[] itemGroups1 = { 2, 3, 6, 12, 25 };

            DaggerfallUnityItem newItem = null;

            // Get the list of magic item templates read from MAGIC.DEF
            MagicItemsFile magicItemsFile = new MagicItemsFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, "MAGIC.DEF"));
            List<MagicItemTemplate> magicItems = magicItemsFile.MagicItemsList;

            // Get the number of non-artifact magic item templates in MAGIC.DEF
            int numberOfRegularMagicItems = 0;
            foreach (MagicItemTemplate magicItem in magicItems)
            {
                if (magicItem.type == MagicItemTypes.RegularMagicItem)
                    numberOfRegularMagicItems++;
            }

            // Choose a random one of the non-artifact magic item templates
            int chosenItem = UnityEngine.Random.Range(0, numberOfRegularMagicItems);

            // Get the chosen template
            foreach (MagicItemTemplate magicItem in magicItems)
            {
                if (magicItem.type == MagicItemTypes.RegularMagicItem)
                {
                    // Proceed when the template is found
                    if (chosenItem == 0)
                    {
                        // Get the item group. The possible groups are determined by the 33rd byte (magicItem.group) of the MAGIC.DEF template being used.
                        ItemGroups group = 0;
                        if (magicItem.group == 0)
                            group = (ItemGroups)itemGroups0[UnityEngine.Random.Range(0, 7)];
                        else if (magicItem.group == 1)
                            group = (ItemGroups)itemGroups1[UnityEngine.Random.Range(0, 5)];
                        else if (magicItem.group == 2)
                            group = ItemGroups.Weapons;

                        // Create the base item
                        if (group == ItemGroups.Weapons)
                        {
                            newItem = CreateRandomWeapon(playerLevel);

                            // No arrows as enchanted items
                            while (newItem.GroupIndex == 18)
                                newItem = CreateRandomWeapon(playerLevel);
                        }
                        else if (group == ItemGroups.Armor)
                            newItem = CreateRandomArmor(playerLevel, gender, race);
                        else if (group == ItemGroups.MensClothing || group == ItemGroups.WomensClothing)
                            newItem = CreateRandomClothing(gender, race);
                        else if (group == ItemGroups.ReligiousItems)
                            newItem = CreateRandomReligiousItem();
                        else if (group == ItemGroups.Gems)
                            newItem = CreateRandomGem();
                        else // Only other possibility is jewellery
                            newItem = CreateRandomJewellery();

                        // Replace the regular item name with the magic item name
                        newItem.shortName = magicItem.name;

                        // Add the enchantments
                        newItem.legacyMagic = new DaggerfallEnchantment[magicItem.enchantments.Length];
                        for (int i = 0; i < magicItem.enchantments.Length; ++i)
                            newItem.legacyMagic[i] = magicItem.enchantments[i];

                        // Set the condition/magic uses
                        newItem.maxCondition = magicItem.uses;
                        newItem.currentCondition = magicItem.uses;

                        // Set the value of the item. This is determined by the enchantment point cost/spell-casting cost
                        // of the enchantments on the item.
                        int value = 0;
                        for (int i = 0; i < magicItem.enchantments.Length; ++i)
                        {
                            if (magicItem.enchantments[i].type != EnchantmentTypes.None
                                && magicItem.enchantments[i].type < EnchantmentTypes.ItemDeteriorates)
                            {
                                switch (magicItem.enchantments[i].type)
                                {
                                    case EnchantmentTypes.CastWhenUsed:
                                    case EnchantmentTypes.CastWhenHeld:
                                    case EnchantmentTypes.CastWhenStrikes:
                                        // Enchantments that cast a spell. The parameter is the spell index in SPELLS.STD.
                                        value += Formulas.FormulaHelper.GetSpellEnchantPtCost(magicItem.enchantments[i].param);
                                        break;
                                    case EnchantmentTypes.RepairsObjects:
                                    case EnchantmentTypes.AbsorbsSpells:
                                    case EnchantmentTypes.EnhancesSkill:
                                    case EnchantmentTypes.FeatherWeight:
                                    case EnchantmentTypes.StrengthensArmor:
                                        // Enchantments that provide an effect that has no parameters
                                        value += enchantmentPointCostsForNonParamTypes[(int)magicItem.enchantments[i].type];
                                        break;
                                    case EnchantmentTypes.SoulBound:
                                        // Bound soul
                                        MobileEnemy mobileEnemy = GameObjectHelper.EnemyDict[magicItem.enchantments[i].param];
                                        value += mobileEnemy.SoulPts; // TODO: Not sure about this. Should be negative? Needs to be tested.
                                        break;
                                    default:
                                        // Enchantments that provide a non-spell effect with a parameter (parameter = when effect applies, what enemies are affected, etc.)
                                        value += enchantmentPtsForItemPowerArrays[(int)magicItem.enchantments[i].type][magicItem.enchantments[i].param];
                                        break;
                                }
                            }
                        }

                        newItem.value = value;

                        break;
                    }

                    chosenItem--;
                }
            }

            if (newItem == null)
                throw new Exception("CreateRandomMagicItem() failed to create an item.");

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
        /// <param name="ingredientGroup">Ingredient group.</param>
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
            ItemGroups itemGroup;
            int group = UnityEngine.Random.Range(0, 8);
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
        /// Creates a potion.
        /// </summary>
        /// <param name="recipe">Recipe index for the potion</param>
        /// <returns>Potion DaggerfallUnityItem</returns>
        public static DaggerfallUnityItem CreatePotion(int recipeKey, int stackSize = 1)
        {
            return new DaggerfallUnityItem(ItemGroups.UselessItems1, 1) { PotionRecipeKey = recipeKey, stackCount = stackSize };
        }

        /// <summary>
        /// Creates a random potion from all registered recipes.
        /// </summary>
        /// <returns>Potion DaggerfallUnityItem</returns>
        public static DaggerfallUnityItem CreateRandomPotion(int stackSize = 1)
        {
            List<int> recipeKeys = GameManager.Instance.EntityEffectBroker.GetPotionRecipeKeys();
            int recipeIdx = UnityEngine.Random.Range(0, recipeKeys.Count);
            return CreatePotion(recipeKeys[recipeIdx]);
        }

        /// <summary>
        /// Creates a random (classic) potion
        /// </summary>
        /// <returns>Potion DaggerfallUnityItem</returns>
        public static DaggerfallUnityItem CreateRandomClassicPotion()
        {
            int recipeIdx = UnityEngine.Random.Range(0, MagicAndEffects.PotionRecipe.classicRecipeKeys.Length);
            return CreatePotion(MagicAndEffects.PotionRecipe.classicRecipeKeys[recipeIdx]);
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
