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

        public static void StockItems(PlayerGPS.DiscoveredBuilding buildingData, ItemCollection items)
        {
            // TODO: Allofich to replace with shelf stocking code... using supplied building type and quality
            DFLocation.BuildingTypes buildingType = buildingData.buildingType;
            int shopQuality = buildingData.quality;

            // Temp test code...
            Game.Entity.PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            items.AddItem(ItemBuilder.CreateRandomArmor(playerEntity.Level, playerEntity.Gender, playerEntity.Race));
            items.AddItem(ItemBuilder.CreateRandomArmor(playerEntity.Level, playerEntity.Gender, playerEntity.Race));
            items.AddItem(ItemBuilder.CreateRandomArmor(playerEntity.Level, playerEntity.Gender, playerEntity.Race));
            DaggerfallUnityItem magic = ItemBuilder.CreateRandomArmor(playerEntity.Level, playerEntity.Gender, playerEntity.Race);
            magic.legacyMagic = new int[] { 1, 87, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535 };
            items.AddItem(magic);
            items.AddItem(ItemBuilder.CreateRandomBook());
            items.AddItem(ItemBuilder.CreateRandomClothing(playerEntity.Gender, playerEntity.Race));
            items.AddItem(ItemBuilder.CreateRandomClothing(playerEntity.Gender, playerEntity.Race));
            items.AddItem(ItemBuilder.CreateRandomClothing(playerEntity.Gender, playerEntity.Race));
            items.AddItem(ItemBuilder.CreateRandomClothing(playerEntity.Gender, playerEntity.Race));
            items.AddItem(ItemBuilder.CreateRandomIngredient());
            items.AddItem(ItemBuilder.CreateItem(ItemGroups.MiscItems, 274));
            items.AddItem(ItemBuilder.CreateRandomReligiousItem());
            items.AddItem(ItemBuilder.CreateRandomWeapon(playerEntity.Level));
            items.AddItem(ItemBuilder.CreateRandomWeapon(playerEntity.Level));
            items.AddItem(ItemBuilder.CreateRandomWeapon(playerEntity.Level));
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