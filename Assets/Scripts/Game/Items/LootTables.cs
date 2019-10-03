// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:
//
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.Entity;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// Built-in loot tables.
    /// Currently just for testing during early implementation.
    /// These approximate the loot tables on page 156 of Daggerfall Chronicles but are
    /// different in several ways (e.g. Chronicles uses "WP" for both "Warm Plant" and "Weapon").
    /// May diverge substantially over time during testing and future implementation.
    /// </summary>
    public static class LootTables
    {
        /// <summary>
        /// Default loot table chance matrices.
        /// Note: Temporary implementation. Will eventually be moved to an external file and loaded as keyed dict.
        /// Note: Many loot tables are defined with a lower chance for magic items in FALL.EXE's tables than is
        /// shown in Daggerfall Chronicles.
        /// These are shown below in the order "Key", "Chronicles chance", "FALL.EXE chance".
        /// E, 5, 3
        /// F, 2, 1
        /// G, 3, 1
        /// H, 2, 1
        /// I, 10, 2
        /// J, 20, 3
        /// K, 5, 3
        /// L, 2, 1
        /// N, 2, 1
        /// O, 3, 2
        /// P, 3, 2
        /// Q, 10, 3
        /// R, 5, 2
        /// S, 20, 3
        /// T, 3, 1
        /// U, 3, 2
        /// </summary>

        private readonly static int[] tiers = { 1, 4, 6, 10, 20, 40 };


        public static LootChanceMatrix[] DefaultLootTables = {
            new LootChanceMatrix() {key = "-", MinGold = 0, MaxGold = 0, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 0, MI = tiers[0], CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "A", MinGold = 1, MaxGold = 10, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 5, WP = 5, MI = tiers[2], CL = 4, BK = 0, M2 = 2, RL = 0 },
            // Chronicles says B has 10 for Warm Plant and Misc. Monster, but in FALL.EXE it is Temperate Plant and Warm Plant.
            new LootChanceMatrix() {key = "B", MinGold = 0, MaxGold = 0, P1 = 10, P2 = 10, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 0, MI = tiers[0], CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "C", MinGold = 2, MaxGold = 20, P1 = 10, P2 = 10, C1 = 5, C2 = 5, C3 = 5, M1 = 5, AM = 5, WP = 25, MI = tiers[3], CL = 0, BK = 2, M2 = 2, RL = 2 },
            new LootChanceMatrix() {key = "D", MinGold = 1, MaxGold = 4, P1 = 6, P2 = 6, C1 = 6, C2 = 6, C3 = 6, M1 = 6, AM = 0, WP = 0, MI = tiers[0], CL = 0, BK = 0, M2 = 0, RL = 4 },
            new LootChanceMatrix() {key = "E", MinGold = 20, MaxGold = 80, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 10, WP = 10, MI = tiers[3], CL = 4, BK = 2, M2 = 1, RL = 15 },
            new LootChanceMatrix() {key = "F", MinGold = 4, MaxGold = 30, P1 = 2, P2 = 2, C1 = 5, C2 = 5, C3 = 5, M1 = 2, AM = 50, WP = 50, MI = tiers[1], CL = 0, BK = 0, M2 = 3, RL = 0 },
            new LootChanceMatrix() {key = "G", MinGold = 3, MaxGold = 15, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 50, WP = 50, MI = tiers[2], CL = 5, BK = 0, M2 = 3, RL = 0 },
            new LootChanceMatrix() {key = "H", MinGold = 2, MaxGold = 10, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 100, MI = tiers[1], CL = 2, BK = 0, M2 = 0, RL = 0 },
            // Chronicles is missing "I" but lists its data in table "J." All the tables from here are off by one compared to Chronicles.
            new LootChanceMatrix() {key = "I", MinGold = 0, MaxGold = 0, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 0, MI = tiers[4], CL = 0, BK = 0, M2 = 0, RL = 5 },
            new LootChanceMatrix() {key = "J", MinGold = 50, MaxGold = 150, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 5, WP = 5, MI = tiers[5], CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "K", MinGold = 1, MaxGold = 10, P1 = 3, P2 = 3, C1 = 3, C2 = 3, C3 = 3, M1 = 3, AM = 5, WP = 5, MI = tiers[3], CL = 0, BK = 5, M2 = 2, RL = 100 },
            new LootChanceMatrix() {key = "L", MinGold = 1, MaxGold = 20, P1 = 0, P2 = 0, C1 = 3, C2 = 3, C3 = 3, M1 = 3, AM = 50, WP = 50, MI = tiers[2], CL = 75, BK = 0, M2 = 5, RL = 3 },
            new LootChanceMatrix() {key = "M", MinGold = 1, MaxGold = 15, P1 = 1, P2 = 1, C1 = 1, C2 = 1, C3 = 1, M1 = 2, AM = 10, WP = 10, MI = tiers[1], CL = 15, BK = 2, M2 = 3, RL = 1 },
            new LootChanceMatrix() {key = "N", MinGold = 1, MaxGold = 80, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 5, AM = 5, WP = 5, MI = tiers[2], CL = 20, BK = 5, M2 = 2, RL = 5 },
            new LootChanceMatrix() {key = "O", MinGold = 5, MaxGold = 20, P1 = 1, P2 = 1, C1 = 1, C2 = 1, C3 = 1, M1 = 1, AM = 10, WP = 15, MI = tiers[2], CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "P", MinGold = 5, MaxGold = 20, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 5, AM = 5, WP = 10, MI = tiers[3], CL = 0, BK = 10, M2 = 5, RL = 0 },
            new LootChanceMatrix() {key = "Q", MinGold = 20, MaxGold = 80, P1 = 2, P2 = 2, C1 = 8, C2 = 8, C3 = 8, M1 = 2, AM = 10, WP = 25, MI = tiers[4], CL = 35, BK = 5, M2 = 3, RL = 0 },
            new LootChanceMatrix() {key = "R", MinGold = 5, MaxGold = 20, P1 = 0, P2 = 0, C1 = 3, C2 = 3, C3 = 3, M1 = 5, AM = 5, WP = 15, MI = tiers[4], CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "S", MinGold = 50, MaxGold = 125, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 15, AM = 10, WP = 10, MI = tiers[5], CL = 0, BK = 5, M2 = 5, RL = 0 },
            new LootChanceMatrix() {key = "T", MinGold = 20, MaxGold = 80, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 100, WP = 100, MI = tiers[2], CL = 0, BK = 0, M2 = 0, RL = 0},
            new LootChanceMatrix() {key = "U", MinGold = 7, MaxGold = 30, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 10, AM = 10, WP = 10, MI = tiers[2], CL = 0, BK = 2, M2 = 2, RL = 10 },
        };

        /// <summary>
        /// Gets loot matrix by key.
        /// Note: Temporary implementation. Will eventually be moved to an external file and loaded as keyed dict.
        /// </summary>
        /// <param name="key">Key of matrix to get.</param>
        /// <returns>LootChanceMatrix.</returns>
        public static LootChanceMatrix GetMatrix(string key)
        {
            for (int i = 0; i < DefaultLootTables.Length; i++)
            {
                if (DefaultLootTables[i].key == key)
                    return DefaultLootTables[i];
            }

            return DefaultLootTables[0];
        }

        public static bool GenerateLoot(DaggerfallLoot loot, int locationIndex)
        {
            string[] lootTableKeys = {
            "K", // Crypt
            "N", // Orc Stronghold
            "N", // Human Stronghold
            "N", // Prison
            "K", // Desecrated Temple
            "M", // Mine
            "M", // Natural Cave
            "Q", // Coven
            "K", // Vampire Haunt
            "U", // Laboratory
            "D", // Harpy Nest
            "N", // Ruined Castle
            "L", // Spider Nest
            "F", // Giant Stronghold
            "S", // Dragon's Den
            "N", // Barbarian Stronghold
            "M", // Volcanic Caves
            "L", // Scorpion Nest
            "N", // Cemetery
            };

            // Get loot table key
            if (locationIndex < lootTableKeys.Length)
            {
                DaggerfallLoot.GenerateItems(lootTableKeys[locationIndex], loot.Items);

                // Randomly add map
                char key = lootTableKeys[locationIndex][0];
                int alphabetIndex = key - 64;

                if (alphabetIndex >= 10 && alphabetIndex <= 15) // between keys J and O
                {
                    int[] mapChances = { 2, 1, 1, 2, 2, 15 };
                    int mapChance = mapChances[alphabetIndex - 10];
                    DaggerfallLoot.RandomlyAddMap(mapChance, loot.Items);
                    DaggerfallLoot.RandomlyAddPotion(4, loot.Items);
                    DaggerfallLoot.RandomlyAddPotionRecipe(2, loot.Items);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Generates an array of items based on loot chance matrix.
        /// </summary>
        /// <param name="matrix">Loot chance matrix.</param>
        /// <param name="playerEntity">Player entity.</param>
        /// <returns>DaggerfallUnityItem array.</returns>
        public static DaggerfallUnityItem[] GenerateRandomLoot(LootChanceMatrix matrix, PlayerEntity playerEntity)
        {
            // Notes: The first part of the DF Chronicles explanation of how loot is generated does not match the released game.
            // It says the chance for each item category is the matrix amount * the level of the NPC. Actual behavior in the
            // released game is (matrix amount * PC level) for the first 4 item categories (temperate plants, warm plants,
            // miscellaneous monster, warm monster), and just matrix amount for the categories after that.
            // The second part of the DF Chronicles explanation (roll repeatedly for items from a category, each time at halved
            // chance), matches the game.
            // In classic, since a 0-99 roll is compared to whether it is less or greater than item chance,
            // even a 0% chance category has a 1/100 chance to appear, and the chance values are effectively
            // 1 higher than what the loot tables show.
            float chance;
            List<DaggerfallUnityItem> items = new List<DaggerfallUnityItem>();

            // Reseed random
            Random.InitState(items.GetHashCode());

            // Random gold
            int goldCount = Random.Range(matrix.MinGold, matrix.MaxGold + 1) * playerEntity.Level;
            if (goldCount > 0)
            {
                items.Add(ItemBuilder.CreateGoldPieces(goldCount));
            }

            // Random weapon
            chance = matrix.WP;
            while (Dice100.SuccessRoll((int)chance))
            {
                int bonusRoll = RollForBonus(playerEntity.Level, (int)chance);
                if (bonusRoll > 0)
                    items.Add(ItemBuilder.CreateRandomWeapon(bonusRoll));
                else
                    items.Add(ItemBuilder.CreateRandomWeapon(playerEntity.Level));
                chance *= 0.5f;
            }

            // Random armor
            chance = matrix.AM;
            while (Dice100.SuccessRoll((int)chance))
            {
                int bonusRoll = RollForBonus(playerEntity.Level, (int)chance);
                if (bonusRoll > 0)
                    items.Add(ItemBuilder.CreateRandomArmor(bonusRoll, playerEntity.Gender, playerEntity.Race));
                else
                    items.Add(ItemBuilder.CreateRandomArmor(playerEntity.Level, playerEntity.Gender, playerEntity.Race));
                chance *= 0.5f;
            }

            // Random ingredients
            RandomIngredient(matrix.C1 * playerEntity.Level, ItemGroups.CreatureIngredients1, items);
            RandomIngredient(matrix.C2 * playerEntity.Level, ItemGroups.CreatureIngredients2, items);
            RandomIngredient(matrix.C3, ItemGroups.CreatureIngredients3, items);
            RandomIngredient(matrix.P1 * playerEntity.Level, ItemGroups.PlantIngredients1, items);
            RandomIngredient(matrix.P2 * playerEntity.Level, ItemGroups.PlantIngredients2, items);
            RandomIngredient(matrix.M1, ItemGroups.MiscellaneousIngredients1, items);
            RandomIngredient(matrix.M2, ItemGroups.MiscellaneousIngredients2, items);

            // Random magic item
            chance = matrix.MI;
            while (Dice100.SuccessRoll((int)chance))
            {
                int bonusRoll = RollForBonus(playerEntity.Level, (int)chance);
                Debug.Log("checking bonusRoll = " + bonusRoll);
                if (bonusRoll > 0)
                    items.Add(ItemBuilder.CreateRandomMagicItem(bonusRoll, playerEntity.Gender, playerEntity.Race));
                else
                    items.Add(ItemBuilder.CreateRandomMagicItem(playerEntity.Level, playerEntity.Gender, playerEntity.Race));
                chance *= 0.5f;
            }

            // Random clothes
            chance = matrix.CL;
            while (Dice100.SuccessRoll((int)chance))
            {
                items.Add(ItemBuilder.CreateRandomClothing(playerEntity.Gender, playerEntity.Race));
                chance *= 0.5f;
            }

            // Random books
            chance = matrix.BK;
            while (Dice100.SuccessRoll((int)chance))
            {
                items.Add(ItemBuilder.CreateRandomBook());
                chance *= 0.5f;
            }

            // Random religious item
            chance = matrix.RL;
            while (Dice100.SuccessRoll((int)chance))
            {
                items.Add(ItemBuilder.CreateRandomReligiousItem());
                chance *= 0.5f;
            }

            return items.ToArray();
        }

        #region Private Methods

        static void RandomIngredient(float chance, ItemGroups ingredientGroup, List<DaggerfallUnityItem> targetItems)
        {
            while (Dice100.SuccessRoll((int)chance))
            {
                targetItems.Add(ItemBuilder.CreateRandomIngredient(ingredientGroup));
                chance *= 0.5f;
            }
        }
        static int RollForBonus(int playerlevel, int chanceSuccess)        {
            float tier = 0;
            int chanceMultiplier = Random.Range(1, 11) - 7;
            if (chanceMultiplier < 1)
                chanceMultiplier = 1;

            chanceSuccess *= chanceMultiplier;
            if (Dice100.SuccessRoll(chanceSuccess))
            {
                tier = (float)Random.Range(1, 101);
                if (tier < 100)
                    tier = tier % 4 + 1;
            }
            else
                tier = 0f;

            tier = Mathf.Round(tier + (float)playerlevel);
            if (tier > 100f)
                tier = 100f;
            return (int)tier;
        }


        #endregion
    }
}
