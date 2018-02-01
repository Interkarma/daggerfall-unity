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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Enables a world object to be lootable by player.
    /// </summary>
    public class DaggerfallLoot : MonoBehaviour
    {
        // Default texture archive for random treasure pile
        public const int randomTreasureArchive = 216;

        // Dimension of random treasure marker in Daggerfall Units
        // Used to align random icon to surface marker is placed on
        public const int randomTreasureMarkerDim = 40;

        // Default icon range for random treasure piles in dungeons and when items are dropped by the player
        // Random treasure is generated only when clicked on and icon has no bearing
        // Only a subset of loot icons from TEXTURE.216 are used
        // These are matched to classic
        public static int[] randomTreasureIconIndices = new int[]
        {
            0, 20, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 37, 43, 44, 45, 46, 47
        };

        public static byte[] itemGroupsAlchemist = new byte[] { 0x0E, 0x1E, 0x0F, 0x32, 0x10, 0x32, 0x11, 0x1E, 0x12, 0x14, 0x13, 0x14, 0x14, 0x3C, 0x15, 0x28, 0x16, 0x1E };
        public static byte[] itemGroupsHouseForSale = new byte[] { 0x11, 0x1E, 0x12, 0x14, 0x13, 0x14 };
        public static byte[] itemGroupsArmorer = new byte[] { 0x02, 0x50, 0x03, 0x14 };
        public static byte[] itemGroupsBank = new byte[] { 0x07, 0x32 };
        public static byte[] itemGroupsTown4 = new byte[] { 0x00, 0x50, 0x04, 0x1E, 0x0D, 0x14, 0x0E, 0x0A, 0x19, 0x32 };
        public static byte[] itemGroupsBookseller = new byte[] { 0x07, 0x28, 0x0B, 0x05 };
        public static byte[] itemGroupsClothingStore = new byte[] { 0x06, 0x32, 0x0C, 0x32 };
        public static byte[] itemGroupsFurnitureStore = new byte[] { 0x0D, 0x14 };
        public static byte[] itemGroupsGemStore = new byte[] { 0x0E, 0x28, 0x19, 0x32 };
        public static byte[] itemGroupsGeneralStore = new byte[] { 0x03, 0x14, 0x06, 0x0A, 0x07, 0x0A, 0x09, 0x32, 0x17, 0x00, 0x0C, 0x0A, 0x04, 0x00 };
        public static byte[] itemGroupsLibrary = new byte[] { 0x07, 0x32 };
        public static byte[] itemGroupsGuildHall = new byte[] { 0x04, 0x1E, 0x07, 0x1E, 0x0F, 0x32, 0x10, 0x32, 0x11, 0x1E, 0x12, 0x14, 0x13, 0x14, 0x14, 0x3C, 0x15, 0x28, 0x16, 0x1E };
        public static byte[] itemGroupsPawnShop = new byte[] { 0x02, 0x0A, 0x03, 0x0A, 0x04, 0x0A, 0x07, 0x0A, 0x09, 0x14, 0x0D, 0x05, 0x0E, 0x0A, 0x19, 0x0A, 0x0A, 0x0A };
        public static byte[] itemGroupsWeaponSmith = new byte[] { 0x02, 0x1E, 0x03, 0x46 };
        public static byte[] itemGroupsTemple = new byte[] { 0x0C, 0xFF, 0x02, 0xFF };

        public static byte[][] privatePropertyItemsModels0to1 =
        {
            new byte[] { 0x06, 0x0C },
            new byte[] { 0x06, 0x0C },
            new byte[] { 0x02, 0x06, 0x0C },
            new byte[] { 0x06, 0x0C },
            new byte[] { 0x02, 0x06, 0x0C },
            new byte[] { 0x06, 0x0C },
            new byte[] { 0x06, 0x0C },
            new byte[] { 0x06, 0x0C },
            new byte[] { 0x06, 0x0C },
            new byte[] { 0x06, 0x0C },
            new byte[] { 0x06, 0x0C },
            new byte[] { 0x02, 0x06, 0x0C },
            new byte[] { 0x02, 0x06, 0x0C },
            new byte[] { 0x02, 0x06, 0x0C },
            new byte[] { 0x06, 0x0C },
            new byte[] { 0x06, 0x0C },
            new byte[] { 0x02, 0x06, 0x0C },
            new byte[] { 0x02, 0x06, 0x0C },
            new byte[] { 0x06, 0x0C },
            new byte[] { 0x06, 0x0C },
            new byte[] { 0x06, 0x0C },
        };

        public static byte[][] privatePropertyItemsModels2to3 =
        {
            new byte[] { 0x06, 0x09, 0x0C },
            new byte[] { 0x06, 0x09, 0x0C },
            new byte[] { 0x03, 0x06, 0x09, 0x0C },
            new byte[] { 0x07, 0x09, 0x0B },
            new byte[] { 0x09, 0x0A, 0x0B, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x19 },
            new byte[] { 0x07, 0x09, 0x0B },
            new byte[] { 0x06, 0x09, 0x0C },
            new byte[] { 0x06, 0x09, 0x0C },
            new byte[] { 0x09, 0x0B, 0x0E, 0x19 },
            new byte[] { 0x09, 0x0A },
            new byte[] { 0x09, 0x0B },
            new byte[] { 0x03, 0x09, 0x0A, 0x0B },
            new byte[] { 0x03, 0x09, 0x0B },
            new byte[] { 0x03, 0x09 },
            new byte[] { 0x07, 0x09, 0x0A, 0x0B, 0x0E },
            new byte[] { 0x03, 0x09, 0x0A },
            new byte[] { 0x03, 0x09, 0x0A, 0x0B, 0x19 },
            new byte[] { 0x03, 0x09, 0x0A, 0x0B },
            new byte[] { 0x06, 0x09, 0x0C },
            new byte[] { 0x06, 0x09, 0x0C },
            new byte[] { 0x06, 0x09, 0x0C }
        };

        public static byte[][] privatePropertyItemsModels4to10 =
        {
            new byte[] { 0x07, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x15 },
            new byte[] { 0x09, 0x15 },
            new byte[] { 0x09, 0x15 },
            new byte[] { 0x07, 0x19 },
            new byte[] { 0x09, 0x0A, 0x0B, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14 },
            new byte[] { 0x07, 0x09, 0x0A, 0x0B },
            new byte[] { 0x06, 0x09, 0x0A, 0x0C, 0x0F, 0x10 },
            new byte[] { 0x09 },
            new byte[] { 0x07, 0x09, 0x0A, 0x15 },
            new byte[] { 0x09 },
            new byte[] { 0x09, 0x0D },
            new byte[] { 0x09, 0x0A, 0x0D },
            new byte[] { 0x09, 0x0A },
            new byte[] { 0x09, 0x0A, 0x15 },
            new byte[] { 0x04, 0x09, 0x0A, 0x14 },
            new byte[] { 0x09, 0x0F, 0x10 },
            new byte[] { 0x07, 0x09, 0x0D, 0x14 },
            new byte[] { 0x07, 0x09, 0x0D, 0x14 },
            new byte[] { 0x09, 0x0A },
            new byte[] { 0x09, 0x0A },
            new byte[] { 0x09, 0x0A }
        };

        public static byte[][] privatePropertyItemsModels11to14 =
        {
            new byte[] { 0x07, 0x09, 0x14, },
            new byte[] { 0x07, 0x09, 0x14, },
            new byte[] { 0x03, 0x06, 0x09, 0x0C },
            new byte[] { 0x09, 0x0D, 0x0E },
            new byte[] { 0x02, 0x03, 0x09 },
            new byte[] { 0x07, 0x09 },
            new byte[] { 0x06, 0x09, 0x0C },
            new byte[] { 0x09 },
            new byte[] { 0x03, 0x09, 0x15, 0x0E },
            new byte[] { 0x09, 0x0A },
            new byte[] { 0x09 },
            new byte[] { 0x09, 0x03, 0x0E },
            new byte[] { 0x09, 0x0D, 0x19 },
            new byte[] { 0x03, 0x09, 0x0A, 0x15 },
            new byte[] { 0x07, 0x09, 0x0A },
            new byte[] { 0x06, 0x09, 0x0C },
            new byte[] { 0x04, 0x07, 0x09, 0x0D, 0x19 },
            new byte[] { 0x07, 0x09, 0x0D, 0x19 },
            new byte[] { 0x03, 0x09 },
            new byte[] { 0x03, 0x09 },
            new byte[] { 0x03, 0x09 }
        };

        public static byte[][] privatePropertyItemsModels15AndUp =
        {
            new byte[] { 0x0F, 0x10, 0x11, 0x12, 0x13, 0x15 },
            new byte[] { 0x02, 0x15 },
            new byte[] { 0x02, 0x15 },
            new byte[] { 0x0D, 0x19 },
            new byte[] { 0x02, 0x03, 0x04, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x15 },
            new byte[] { 0x07, 0x0D },
            new byte[] { 0x06, 0x09, 0x0C, 0x0F, 0x10 },
            new byte[] { 0x09, 0x0D },
            new byte[] { 0x09, 0x15 },
            new byte[] { 0x09 },
            new byte[] { 0x09, 0x0D },
            new byte[] { 0x02, 0x09, 0x0D },
            new byte[] { 0x02, 0x03, 0x09 },
            new byte[] { 0x03, 0x09, 0x15 },
            new byte[] { 0x09, 0x0A, 0x0D },
            new byte[] { 0x09, 0x0F, 0x10 },
            new byte[] { 0x09, 0x0D, 0x19 },
            new byte[] { 0x09, 0x0D, 0x19 },
            new byte[] { 0x06, 0x09, 0x0C },
            new byte[] { 0x06, 0x09, 0x0C },
            new byte[] { 0x06, 0x09, 0x0C }
        };

        public LootContainerTypes ContainerType = LootContainerTypes.Nothing;
        public InventoryContainerImages ContainerImage = InventoryContainerImages.Chest;
        public string entityName = string.Empty;
        public int TextureArchive = 0;
        public int TextureRecord = 0;
        public bool playerOwned = false;
        public bool customDrop = false;         // Custom drop loot is not part of base scene and must be respawned on deserialization
        public bool isEnemyClass = false;

        ulong loadID = 0;
        ItemCollection items = new ItemCollection();

        public ulong LoadID
        {
            get { return loadID; }
            set { loadID = value; }
        }

        public ItemCollection Items
        {
            get { return items; }
        }

        public static void StockShopShelf(PlayerGPS.DiscoveredBuilding buildingData, ItemCollection items)
        {
            DFLocation.BuildingTypes buildingType = buildingData.buildingType;
            int shopQuality = buildingData.quality;
            Game.Entity.PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            byte[] itemGroups = { 0 };

            switch (buildingType)
            {
                case DFLocation.BuildingTypes.Alchemist:
                    itemGroups = itemGroupsAlchemist;
                    RandomlyAddPotionRecipe(25, items);
                    break;
                case DFLocation.BuildingTypes.Armorer:
                    itemGroups = itemGroupsArmorer;
                    break;
                case DFLocation.BuildingTypes.Bookseller:
                    itemGroups = itemGroupsBookseller;
                    break;
                case DFLocation.BuildingTypes.ClothingStore:
                    itemGroups = itemGroupsClothingStore;
                    break;
                case DFLocation.BuildingTypes.GemStore:
                    itemGroups = itemGroupsGemStore;
                    break;
                case DFLocation.BuildingTypes.GeneralStore:
                    itemGroups = itemGroupsGeneralStore;
                    items.AddItem(ItemBuilder.CreateItem(ItemGroups.Transportation, (int)Transportation.Horse));
                    items.AddItem(ItemBuilder.CreateItem(ItemGroups.Transportation, (int)Transportation.Small_cart));
                    break;
                case DFLocation.BuildingTypes.PawnShop:
                    itemGroups = itemGroupsPawnShop;
                    break;
                case DFLocation.BuildingTypes.WeaponSmith:
                    itemGroups = itemGroupsWeaponSmith;
                    break;
            }

            for (int i = 0; i < itemGroups.Length; i += 2)
            {
                ItemGroups itemGroup = (ItemGroups)itemGroups[i];
                int chanceMod = itemGroups[i + 1];
                if (itemGroup == ItemGroups.MensClothing && playerEntity.Gender == Game.Entity.Genders.Female)
                    itemGroup = ItemGroups.WomensClothing;
                if (itemGroup == ItemGroups.WomensClothing && playerEntity.Gender == Game.Entity.Genders.Male)
                    itemGroup = ItemGroups.MensClothing;

                if (itemGroup != ItemGroups.Furniture && itemGroup != ItemGroups.UselessItems1)
                {
                    if (itemGroup == ItemGroups.Books)
                    {
                        int qualityMod = (shopQuality + 3) / 5;
                        if (qualityMod >= 4)
                            --qualityMod;
                        qualityMod++;
                        for (int j = 0; j <= qualityMod; ++j)
                        {
                            items.AddItem(ItemBuilder.CreateRandomBook());
                        }
                    }
                    else
                    {
                        System.Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(itemGroup);
                        for (int j = 0; j < enumArray.Length; ++j)
                        {
                            DaggerfallConnect.FallExe.ItemTemplate itemTemplate = DaggerfallUnity.Instance.ItemHelper.GetItemTemplate(itemGroup, j);
                            if (itemTemplate.rarity <= shopQuality)
                            {
                                int stockChance = chanceMod * 5 * (21 - itemTemplate.rarity) / 100;
                                if (Random.Range(1, 101) <= stockChance)
                                {
                                    DaggerfallUnityItem item = null;
                                    if (itemGroup == ItemGroups.Weapons)
                                        item = ItemBuilder.CreateWeapon(j + Weapons.Dagger, ItemBuilder.RandomMaterial(playerEntity.Level));
                                    else if (itemGroup == ItemGroups.Armor)
                                        item = ItemBuilder.CreateArmor(playerEntity.Gender, playerEntity.Race, j + Armor.Cuirass, ItemBuilder.RandomArmorMaterial(playerEntity.Level));
                                    else if (itemGroup == ItemGroups.MensClothing)
                                    {
                                        item = ItemBuilder.CreateMensClothing(j + MensClothing.Straps, playerEntity.Race);
                                        item.dyeColor = ItemBuilder.RandomClothingDye();
                                    }
                                    else if (itemGroup == ItemGroups.WomensClothing)
                                    {
                                        item = ItemBuilder.CreateWomensClothing(j + WomensClothing.Brassier, playerEntity.Race);
                                        item.dyeColor = ItemBuilder.RandomClothingDye();
                                    }
                                    else
                                        item = new DaggerfallUnityItem(itemGroup, j);

                                    items.AddItem(item);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void StockHouseContainer(PlayerGPS.DiscoveredBuilding buildingData, ItemCollection items, uint modelIndex)
        {
            DFLocation.BuildingTypes buildingType = buildingData.buildingType;
            int buildingQuality = buildingData.quality;
            byte[] privatePropertyList = null;
            DaggerfallUnityItem item = null;
            Game.Entity.PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

            if (buildingType < DFLocation.BuildingTypes.House5)
            {
                if (modelIndex >= 2)
                {
                    if (modelIndex >= 4)
                    {
                        if (modelIndex >= 11)
                        {
                            if (modelIndex >= 15)
                            {
                                privatePropertyList = privatePropertyItemsModels15AndUp[(int)buildingType];
                            }
                            else
                            {
                                privatePropertyList = privatePropertyItemsModels11to14[(int)buildingType];
                            }
                        }
                        else
                        {
                            privatePropertyList = privatePropertyItemsModels4to10[(int)buildingType];
                        }
                    }
                    else
                    {
                        privatePropertyList = privatePropertyItemsModels2to3[(int)buildingType];
                    }
                }
                else
                {
                    privatePropertyList = privatePropertyItemsModels0to1[(int)buildingType];
                }
                if (privatePropertyList == null)
                    return;
                int randomChoice = Random.Range(0, privatePropertyList.Length);
                ItemGroups itemGroup = (ItemGroups)privatePropertyList[randomChoice];
                int continueChance = 100;
                bool keepGoing = true;
                while (keepGoing)
                {
                    if (itemGroup != ItemGroups.MensClothing && itemGroup != ItemGroups.WomensClothing)
                    {
                        if (itemGroup == ItemGroups.MagicItems)
                        {
                            //TODO: Make magic item
                        }
                        else if (itemGroup == ItemGroups.Books)
                        {
                            int groupIndex = (buildingQuality + 3) / 5;
                            if (groupIndex == (int)ItemGroups.Books)
                                items.AddItem(ItemBuilder.CreateRandomBook());
                            else
                                item = new DaggerfallUnityItem(itemGroup, groupIndex);
                        }
                        else
                        {
                            if (itemGroup == ItemGroups.Weapons)
                                item = ItemBuilder.CreateRandomWeapon(playerEntity.Level);
                            else if (itemGroup == ItemGroups.Armor)
                                item = ItemBuilder.CreateRandomArmor(playerEntity.Level, playerEntity.Gender, playerEntity.Race);
                            else
                            {
                                System.Array enumArray = DaggerfallUnity.Instance.ItemHelper.GetEnumArray(itemGroup);
                                item = new DaggerfallUnityItem(itemGroup, Random.Range(0,enumArray.Length));
                            }
                        }
                    }
                    else
                    {
                        item = ItemBuilder.CreateRandomClothing(playerEntity.Gender,playerEntity.Race);
                    }
                    continueChance >>= 1;
                    if (DFRandom.rand() % 100 > continueChance)
                        keepGoing = false;
                    items.AddItem(item);
                }
            }
        }

        /// <summary>
        /// Generates items in the given item collection based on loot table key.
        /// Any existing items will be destroyed.
        /// </summary>
        public static void GenerateItems(string LootTableKey, ItemCollection collection)
        {
            LootChanceMatrix matrix = LootTables.GetMatrix(LootTableKey);
            DaggerfallUnityItem[] newitems = LootTables.GenerateRandomLoot(LootTableKey, matrix, GameManager.Instance.PlayerEntity);

            collection.Import(newitems);
        }

        /// <summary>
        /// Randomly add a map
        /// </summary>
        public static void RandomlyAddMap(int chance, ItemCollection collection)
        {
            if (Random.Range(1, 101) <= chance)
            {
                DaggerfallUnityItem map = new DaggerfallUnityItem(ItemGroups.MiscItems, 8);
                collection.AddItem(map);
            }
        }

        /// <summary>
        /// Randomly add a potion
        /// </summary>
        public static void RandomlyAddPotion(int chance, ItemCollection collection)
        {
            ushort[] potionValues = { 25, 50, 50, 50, 75, 75, 75, 75, 100, 100, 100, 100, 125, 125, 125, 200, 200, 200, 250, 500 };
            if (Random.Range(1, 101) < chance)
            {
                DaggerfallUnityItem potion = new DaggerfallUnityItem(ItemGroups.UselessItems1, 1);
                byte recipe = (byte)Random.Range(0, 20);
                potion.typeDependentData = recipe;
                potion.value = potionValues[recipe];
                collection.AddItem(potion);
            }
        }

        /// <summary>
        /// Randomly add a potion recipe
        /// </summary>
        public static void RandomlyAddPotionRecipe(int chance, ItemCollection collection)
        {
            if (Random.Range(1, 101) < chance)
            {
                DaggerfallUnityItem potionRecipe = new DaggerfallUnityItem(ItemGroups.MiscItems, 4);
                byte recipe = (byte)Random.Range(0, 20);
                potionRecipe.typeDependentData = recipe;
                collection.AddItem(potionRecipe);
            }
        }

        /// <summary>
        /// Called when this loot collection is opened by inventory window
        /// </summary>
        public void OnInventoryOpen()
        {
            //Debug.Log("Loot container opened.");
        }

        /// <summary>
        /// Called when this loot collection is closed by inventory window
        /// </summary>
        public void OnInventoryClose()
        {
            //Debug.Log("Loot container closed.");
        }
    }
}