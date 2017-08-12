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

        // Default icon range for random treasure piles
        // Random treasure is generated only when clicked on and icon has no bearing
        // Only a subset of loot icons from TEXTURE.216 are used
        // May be expanded later
        public static int[] randomTreasureIconIndices = new int[]
        {
            0, 1, 20, 22, 23, 24, 25, 26, 27, 28, 30, 37, 46, 47
        };

        public LootContainerTypes ContainerType = LootContainerTypes.Nothing;
        public InventoryContainerImages ContainerImage = InventoryContainerImages.Chest;
        public string LootTableKey = string.Empty;
        public int TextureArchive = 0;
        public int TextureRecord = 0;
        public bool playerOwned = false;
        public bool customDrop = false;         // Custom drop loot is not part of base scene and must be respawned on deserialization

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

        /// <summary>
        /// Regenerates items in this container based on loot table key.
        /// Existing items will be destroyed.
        /// </summary>
        public void GenerateItems()
        {
            LootChanceMatrix matrix = LootTables.GetMatrix(LootTableKey);
            DaggerfallUnityItem[] newitems = LootTables.GenerateRandomLoot(LootTableKey, matrix, GameManager.Instance.PlayerEntity);

            items.Import(newitems);
        }

        /// <summary>
        /// Randomly add a map
        /// </summary>
        public void RandomlyAddMap(int chance)
        {
            if (Random.Range(1, 101) <= chance)
            {
                DaggerfallUnityItem map = new DaggerfallUnityItem(ItemGroups.MiscItems, 8);
                items.AddItem(map);
            }
        }

        /// <summary>
        /// Randomly add a potion
        /// TODO: Add potion contents. Just a glass bottle for now.
        /// </summary>
        public void RandomlyAddPotion(int chance)
        {
            if (Random.Range(1, 101) < chance)
            {
                DaggerfallUnityItem potion = new DaggerfallUnityItem(ItemGroups.UselessItems1, 1);
                items.AddItem(potion);
            }
        }

        /// <summary>
        /// Randomly add a potion recipe
        /// </summary>
        public void RandomlyAddPotionRecipe(int chance)
        {
            if (Random.Range(1, 101) < chance)
            {
                DaggerfallUnityItem potionRecipe = new DaggerfallUnityItem(ItemGroups.MiscItems, 4);
                byte recipe = (byte)Random.Range(0, 20);
                potionRecipe.typeDependentData = recipe;
                items.AddItem(potionRecipe);
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