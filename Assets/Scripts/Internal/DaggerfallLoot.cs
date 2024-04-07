// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Allofich, Hazelnut
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Questing;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Enables a world object to be lootable by player.
    /// </summary>
    public class DaggerfallLoot : MonoBehaviour
    {
        // Dimension of random treasure marker in Daggerfall Units
        // Used to align random icon to surface marker is placed on
        public const int randomTreasureMarkerDim = 40;

        public WorldContext WorldContext = WorldContext.Nothing;
        public LootContainerTypes ContainerType = LootContainerTypes.Nothing;
        public InventoryContainerImages ContainerImage = InventoryContainerImages.Chest;
        public string entityName = string.Empty;
        public int TextureArchive = 0;
        public int TextureRecord = 0;
        public bool playerOwned = false;
        public bool houseOwned = false;
        public bool customDrop = false;         // Custom drop loot is not part of base scene and must be respawned on deserialization
        public bool isEnemyClass = false;
        public int stockedDate = 0;
        public ulong corpseQuestUID = 0;

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

        public void Awake()
        {
            // Register as Loot object
            ActiveGameObjectDatabase.RegisterLoot(gameObject);
        }

        public static int CreateStockedDate(DaggerfallDateTime date)
        {
            return (date.Year * 1000) + date.DayOfYear;
        }

        /// <summary>
        /// Generates items in the given item collection based on loot table key.
        /// Any existing items will be destroyed.
        /// </summary>
        public static void GenerateItems(string LootTableKey, ItemCollection collection)
        {
            LootChanceMatrix matrix = LootTables.GetMatrix(LootTableKey);
            DaggerfallUnityItem[] newitems = LootTables.GenerateRandomLoot(matrix, GameManager.Instance.PlayerEntity);

            collection.Import(newitems);
        }

        /// <summary>
        /// Randomly add a map
        /// </summary>
        public static void RandomlyAddMap(int chance, ItemCollection collection)
        {
            if (Dice100.SuccessRoll(chance))
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
            if (Dice100.SuccessRoll(chance))
                collection.AddItem(ItemBuilder.CreateRandomPotion());
        }

        /// <summary>
        /// Randomly add a potion recipe
        /// </summary>
        public static void RandomlyAddPotionRecipe(int chance, ItemCollection collection)
        {
            if (Dice100.SuccessRoll(chance))
            {
                int recipeIdx = Random.Range(0, PotionRecipe.classicRecipeKeys.Length);
                int recipeKey = PotionRecipe.classicRecipeKeys[recipeIdx];
                DaggerfallUnityItem potionRecipe = new DaggerfallUnityItem(ItemGroups.MiscItems, 4) { PotionRecipeKey = recipeKey };
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

        private void Update()
        {
            // If this a quest corpse marker then disable and destroy self when quest complete
            if (ContainerType == LootContainerTypes.CorpseMarker && corpseQuestUID != 0)
            {
                Quest quest = QuestMachine.Instance.GetQuest(corpseQuestUID);
                if ((quest == null || quest.QuestTombstoned) && gameObject.activeSelf)
                {
                    gameObject.SetActive(false);
                    GameObject.Destroy(gameObject);
                }
            }
        }

        public void StockShopShelf(PlayerGPS.DiscoveredBuilding buildingData)
        {
            stockedDate = CreateStockedDate(DaggerfallUnity.Instance.WorldTime.Now);
            items.Clear();

            DFLocation.BuildingTypes buildingType = buildingData.buildingType;
            int shopQuality = buildingData.quality;
            Game.Entity.PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            ItemHelper itemHelper = DaggerfallUnity.Instance.ItemHelper;
            byte[] itemGroups = { 0 };

            switch (buildingType)
            {
                case DFLocation.BuildingTypes.Alchemist:
                    itemGroups = DaggerfallLootDataTables.itemGroupsAlchemist;
                    RandomlyAddPotionRecipe(25, items);
                    break;
                case DFLocation.BuildingTypes.Armorer:
                    itemGroups = DaggerfallLootDataTables.itemGroupsArmorer;
                    break;
                case DFLocation.BuildingTypes.Bookseller:
                    itemGroups = DaggerfallLootDataTables.itemGroupsBookseller;
                    break;
                case DFLocation.BuildingTypes.ClothingStore:
                    itemGroups = DaggerfallLootDataTables.itemGroupsClothingStore;
                    break;
                case DFLocation.BuildingTypes.GemStore:
                    itemGroups = DaggerfallLootDataTables.itemGroupsGemStore;
                    break;
                case DFLocation.BuildingTypes.GeneralStore:
                    itemGroups = DaggerfallLootDataTables.itemGroupsGeneralStore;
                    items.AddItem(ItemBuilder.CreateItem(ItemGroups.Transportation, (int)Transportation.Horse));
                    items.AddItem(ItemBuilder.CreateItem(ItemGroups.Transportation, (int)Transportation.Small_cart));
                    break;
                case DFLocation.BuildingTypes.PawnShop:
                    itemGroups = DaggerfallLootDataTables.itemGroupsPawnShop;
                    break;
                case DFLocation.BuildingTypes.WeaponSmith:
                    itemGroups = DaggerfallLootDataTables.itemGroupsWeaponSmith;
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
                        System.Array enumArray = itemHelper.GetEnumArray(itemGroup);
                        for (int j = 0; j < enumArray.Length; ++j)
                        {
                            ItemTemplate itemTemplate = itemHelper.GetItemTemplate(itemGroup, j);
                            if (itemTemplate.rarity <= shopQuality)
                            {
                                int stockChance = chanceMod * 5 * (21 - itemTemplate.rarity) / 100;
                                if (Dice100.SuccessRoll(stockChance))
                                {
                                    DaggerfallUnityItem item = null;
                                    if (itemGroup == ItemGroups.Weapons)
                                        item = ItemBuilder.CreateWeapon(j + Weapons.Dagger, FormulaHelper.RandomMaterial(playerEntity.Level));
                                    else if (itemGroup == ItemGroups.Armor)
                                        item = ItemBuilder.CreateArmor(playerEntity.Gender, playerEntity.Race, j + Armor.Cuirass, FormulaHelper.RandomArmorMaterial(playerEntity.Level));
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
                                    else if (itemGroup == ItemGroups.MagicItems)
                                    {
                                        item = ItemBuilder.CreateRandomMagicItem(playerEntity.Level, playerEntity.Gender, playerEntity.Race);
                                    }
                                    else
                                    {
                                        item = new DaggerfallUnityItem(itemGroup, j);
                                        if (DaggerfallUnity.Settings.PlayerTorchFromItems && item.IsOfTemplate(ItemGroups.UselessItems2, (int)UselessItems2.Oil))
                                            item.stackCount = Random.Range(5, 20 + 1);  // Shops stock 5-20 bottles
                                    }
                                    items.AddItem(item);
                                }
                            }
                        }
                        // Add any modded items registered in applicable groups
                        int[] customItemTemplates = itemHelper.GetCustomItemsForGroup(itemGroup);
                        // Ensure there's a (21-rarity)% chance for a custom transportation item to be generated
                        if (chanceMod == 0 && itemGroup == ItemGroups.Transportation)
                            chanceMod = 20;

                        for (int j = 0; j < customItemTemplates.Length; j++)
                        {
                            ItemTemplate itemTemplate = itemHelper.GetItemTemplate(itemGroup, customItemTemplates[j]);
                            if (itemTemplate.rarity <= shopQuality)
                            {
                                int stockChance = chanceMod * 5 * (21 - itemTemplate.rarity) / 100;
                                if (Dice100.SuccessRoll(stockChance))
                                {
                                    DaggerfallUnityItem item = ItemBuilder.CreateItem(itemGroup, customItemTemplates[j]);

                                    // Setup specific group stats
                                    if (itemGroup == ItemGroups.Weapons)
                                    {
                                        WeaponMaterialTypes material = FormulaHelper.RandomMaterial(playerEntity.Level);
                                        ItemBuilder.ApplyWeaponMaterial(item, material);
                                    }
                                    else if (itemGroup == ItemGroups.Armor)
                                    {
                                        ArmorMaterialTypes material = FormulaHelper.RandomArmorMaterial(playerEntity.Level);
                                        ItemBuilder.ApplyArmorSettings(item, playerEntity.Gender, playerEntity.Race, material);
                                    }

                                    items.AddItem(item);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void StockHouseContainer(PlayerGPS.DiscoveredBuilding buildingData)
        {
            stockedDate = CreateStockedDate(DaggerfallUnity.Instance.WorldTime.Now);
            items.Clear();

            DFLocation.BuildingTypes buildingType = buildingData.buildingType;
            uint modelIndex = (uint) TextureRecord;
            //int buildingQuality = buildingData.quality;
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
                                privatePropertyList = DaggerfallLootDataTables.privatePropertyItemsModels15AndUp[(int)buildingType];
                            }
                            else
                            {
                                privatePropertyList = DaggerfallLootDataTables.privatePropertyItemsModels11to14[(int)buildingType];
                            }
                        }
                        else
                        {
                            privatePropertyList = DaggerfallLootDataTables.privatePropertyItemsModels4to10[(int)buildingType];
                        }
                    }
                    else
                    {
                        privatePropertyList = DaggerfallLootDataTables.privatePropertyItemsModels2to3[(int)buildingType];
                    }
                }
                else
                {
                    privatePropertyList = DaggerfallLootDataTables.privatePropertyItemsModels0to1[(int)buildingType];
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
                            item = ItemBuilder.CreateRandomMagicItem(playerEntity.Level, playerEntity.Gender, playerEntity.Race);
                        }
                        else if (itemGroup == ItemGroups.Books)
                        {
                            item = ItemBuilder.CreateRandomBook();
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
                                item = new DaggerfallUnityItem(itemGroup, Random.Range(0, enumArray.Length));
                            }
                        }
                    }
                    else
                    {
                        item = ItemBuilder.CreateRandomClothing(playerEntity.Gender, playerEntity.Race);
                    }
                    continueChance >>= 1;
                    if (DFRandom.rand() % 100 > continueChance)
                        keepGoing = false;
                    items.AddItem(item);
                }
            }
        }
    }
}