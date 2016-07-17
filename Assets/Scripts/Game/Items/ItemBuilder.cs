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
        /// Generates a random armor material.
        /// </summary>
        /// <param name="minMaterial">Lowest quality of material.</param>
        /// <param name="maxMaterial">Highest quality of material.</param>
        /// <returns>WeaponMaterialTypes</returns>
        public static WeaponMaterialTypes RandomWeaponMaterial(
            WeaponMaterialTypes minMaterial = WeaponMaterialTypes.Iron,
            WeaponMaterialTypes maxMaterial = WeaponMaterialTypes.Daedric)
        {
            Array materialValues = Enum.GetValues(typeof(WeaponMaterialTypes));
            int index = UnityEngine.Random.Range((int)minMaterial, (int)maxMaterial);
            return (WeaponMaterialTypes)materialValues.GetValue(index);
        }

        /// <summary>
        /// Generates a random weapon material.
        /// </summary>
        /// <param name="minMaterial">Lowest quality of material.</param>
        /// <param name="maxMaterial">Highest quality of material.</param>
        /// <returns>ArmorMaterialTypes</returns>
        public static ArmorMaterialTypes RandomArmorMaterial(
            ArmorMaterialTypes minMaterial = ArmorMaterialTypes.Leather,
            ArmorMaterialTypes maxMaterial = ArmorMaterialTypes.Daedric)
        {
            Array materialValues = Enum.GetValues(typeof(ArmorMaterialTypes));
            int index = UnityEngine.Random.Range((int)minMaterial, (int)maxMaterial);
            return (ArmorMaterialTypes)materialValues.GetValue(index);
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
        /// Generates armour.
        /// </summary>
        /// <param name="armor">Armor item.</param>
        /// <param name="material">Material.</param>
        /// <returns>DaggerfallUnityItem</returns>
        public static DaggerfallUnityItem CreateArmor(Genders gender, Races race, Armor armor, ArmorMaterialTypes material, int variant = 0)
        {
            const int firstFemaleArchive = 245;
            const int firstMaleArchive = 249;

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

            // Validate cuirass variant
            if (item.IsOfTemplate(ItemGroups.Armor, (int)Armor.Cuirass))
            {
                if (item.nativeMaterialValue == (int)ArmorMaterialTypes.Leather)
                {
                    // Leather can only be variant 0
                    variant = 0;
                }
                else if (item.nativeMaterialValue == (int)ArmorMaterialTypes.Chain || item.nativeMaterialValue == (int)ArmorMaterialTypes.Chain2)
                {
                    // Chain can only be variant 4
                    variant = 4;
                }
                else
                {
                    // Plate must be between 1-3
                    variant = Mathf.Clamp(variant, 1, 3);
                }
            }

            // TODO: Validate greaves variant

            // TODO: Validate pauldron variant

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
                    throw new Exception("GetBodyMorphology() encountered unsupprted race value.");
            }
        }

        #endregion
    }
}