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

//using System;
using UnityEngine;
using System.Collections.Generic;

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
        /// </summary>
        public static LootChanceMatrix[] DefaultLootTables = new LootChanceMatrix[]
        {
            new LootChanceMatrix() {key = "-", MinGold = 0, MaxGold = 0, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 0, MI = 0, CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "A", MinGold = 1, MaxGold = 10, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 5, WP = 5, MI = 2, CL = 4, BK = 0, M2 = 2, RL = 0 },
            new LootChanceMatrix() {key = "B", MinGold = 0, MaxGold = 0, P1 = 0, P2 = 10, C1 = 10, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 0, MI = 0, CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "C", MinGold = 2, MaxGold = 20, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 25, AM = 3, WP = 0, MI = 2, CL = 0, BK = 2, M2 = 2, RL = 2 },
            new LootChanceMatrix() {key = "D", MinGold = 1, MaxGold = 4, P1 = 6, P2 = 6, C1 = 6, C2 = 6, C3 = 6, M1 = 6, AM = 0, WP = 0, MI = 0, CL = 0, BK = 0, M2 = 0, RL = 4 },
            new LootChanceMatrix() {key = "E", MinGold = 20, MaxGold = 80, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 10, WP = 10, MI = 5, CL = 4, BK = 2, M2 = 1, RL = 15 },
            new LootChanceMatrix() {key = "F", MinGold = 4, MaxGold = 30, P1 = 2, P2 = 2, C1 = 5, C2 = 5, C3 = 5, M1 = 2, AM = 50, WP = 50, MI = 2, CL = 0, BK = 0, M2 = 3, RL = 0 },
            new LootChanceMatrix() {key = "G", MinGold = 3, MaxGold = 15, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 50, WP = 50, MI = 3, CL = 5, BK = 0, M2 = 3, RL = 0 },
            new LootChanceMatrix() {key = "H", MinGold = 2, MaxGold = 10, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 100, MI = 2, CL = 2, BK = 0, M2 = 0, RL = 0 },
            // Chronicles missing I
            new LootChanceMatrix() {key = "J", MinGold = 0, MaxGold = 0, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 0, MI = 10, CL = 0, BK = 0, M2 = 0, RL = 5 },
            new LootChanceMatrix() {key = "K", MinGold = 50, MaxGold = 150, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 5, WP = 5, MI = 20, CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "L", MinGold = 1, MaxGold = 10, P1 = 3, P2 = 3, C1 = 3, C2 = 3, C3 = 3, M1 = 3, AM = 5, WP = 5, MI = 5, CL = 0, BK = 5, M2 = 2, RL = 100 },
            new LootChanceMatrix() {key = "M", MinGold = 1, MaxGold = 20, P1 = 0, P2 = 0, C1 = 3, C2 = 3, C3 = 3, M1 = 3, AM = 50, WP = 50, MI = 2, CL = 75, BK = 0, M2 = 5, RL = 3 },
            new LootChanceMatrix() {key = "N", MinGold = 1, MaxGold = 15, P1 = 1, P2 = 1, C1 = 1, C2 = 1, C3 = 1, M1 = 2, AM = 10, WP = 10, MI = 1, CL = 15, BK = 2, M2 = 3, RL = 1 },
            new LootChanceMatrix() {key = "O", MinGold = 1, MaxGold = 80, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 5, AM = 5, WP = 5, MI = 2, CL = 20, BK = 5, M2 = 2, RL = 5 },
            new LootChanceMatrix() {key = "P", MinGold = 5, MaxGold = 20, P1 = 1, P2 = 1, C1 = 1, C2 = 1, C3 = 1, M1 = 1, AM = 10, WP = 15, MI = 3, CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "Q", MinGold = 5, MaxGold = 20, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 5, AM = 5, WP = 10, MI = 3, CL = 0, BK = 10, M2 = 5, RL = 0 },
            new LootChanceMatrix() {key = "R", MinGold = 20, MaxGold = 80, P1 = 2, P2 = 2, C1 = 8, C2 = 8, C3 = 8, M1 = 2, AM = 10, WP = 25, MI = 10, CL = 35, BK = 5, M2 = 3, RL = 0 },
            new LootChanceMatrix() {key = "S", MinGold = 5, MaxGold = 20, P1 = 0, P2 = 0, C1 = 3, C2 = 3, C3 = 3, M1 = 5, AM = 5, WP = 15, MI = 5, CL = 0, BK = 0, M2 = 0, RL = 0 },
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

        /// <summary>
        /// Generates an array of items based on loot chance matrix.
        /// </summary>
        /// <param name="matrix">Loot chance matrix.</param>
        /// <param name="playerLevel">Level of player.</param>
        /// <returns>DaggerfallUnityItem array.</returns>
        public static DaggerfallUnityItem[] GenerateRandomLoot(LootChanceMatrix matrix, int playerLevel)
        {
            List<DaggerfallUnityItem> items = new List<DaggerfallUnityItem>();

            // Random gold
            int goldCount = Random.Range(matrix.MinGold, matrix.MaxGold) * playerLevel;
            if (goldCount > 0)
            {
                items.Add(ItemBuilder.CreateGoldPieces(goldCount));
            }

            return items.ToArray();
        }
    }
}