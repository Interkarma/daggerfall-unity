// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Defines static texture tables for key dungeons.
    /// Currently missing key table information required to load correct texture tables at runtime.
    /// </summary>
    public static class StaticTextureTables
    {
        public const int TableLength = 6;

        /// The following tables were dumped at runtime from linear offset 0x286186 using dosbox debugger
        public static int[] PrivateersHold = new int[] { 23, 22, 19, 22, 20, 323 };
        public static int[] Wayrest = new int[] { 23, 19, 19, 20, 20, 168 };
        public static int[] Daggerfall = new int[] { 19, 20, 22, 23, 20, 168 };
        public static int[] Sentinel = new int[] { 120, 123, 122, 122, 119, 168 };
        public static int[] Orsinium = new int[] { 23, 23, 19, 20, 20, 168 };
        public static int[] Shedungent = new int[] { 23, 22, 23, 22, 22, 168 };
        public static int[] ScourgBarrow = new int[] { 20, 20, 23, 23, 20, 168 };
        public static int[] WoodborneHall = new int[] { 23, 23, 22, 22, 23, 168 };
        public static int[] MantellanCrux = new int[] { 123, 123, 123, 120, 120, 168 };
        public static int[] LysandusTomb = new int[] { 23, 23, 23, 23, 23, 168 };
        public static int[] DirenniTower = new int[] { 19, 20, 23, 19, 20, 168 };

        // Default texture table at linear offset 0x28617C
        public static int[] DefaultTextureTable = new int[] { 119, 120, 122, 123, 124, 168 };

        // Helper to generate valid texture table from random seed
        public static int[] RandomTextureTable(int seed)
        {
            // Valid dungeon textures table indices
            int[] valids = new int[]
            {
                019, 020, 022, 023, 024, 068,
                119, 120, 122, 123, 124, 168,
                319, 320, 322, 323, 324, 368,
                419, 420, 422, 423, 424, 468,
            };

            Random.seed = seed;
            int[] textureTable = new int[TableLength];
            for (int i = 0; i < TableLength; i++)
            {
                textureTable[i] = valids[Random.Range(0, valids.Length)];
            }

            return textureTable;
        }
    }
}