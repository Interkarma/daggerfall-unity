// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    Allofich
// 
// Notes: Extracted from DaggerfallLoot class on 2 Feb 2018
//

using System.Collections.Generic;

namespace DaggerfallWorkshop
{
    public static class DaggerfallLootDataTables
    {
        // Default texture archive for random treasure pile
        public const int randomTreasureArchive = 216;

        // Other texture archives for dropped loot icons
        public const int clothingArchive = 204;
        public const int boxesNbottlesArchive = 205;
        public const int combatArchive = 207;
        public const int academicArchive = 209;
        public const int miscArchive = 211;

        // Default icon range for random treasure piles in dungeons and when items are dropped by the player
        // Random treasure is generated only when clicked on and icon has no bearing
        // Only a subset of loot icons from TEXTURE.216 are used & These are matched to classic
        public static int[] randomTreasureIconIndices = new int[]
        {
            0, 20, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 37, 43, 44, 45, 46, 47
        };

        // Dropped items icon lists for player selection in inventory window
        public static Dictionary<int, int[]> dropIconIdxs = new Dictionary<int, int[]>
        {
            { clothingArchive,      new int[] { 0,1,2,3,4,5,6,7,8,9 } },
            { boxesNbottlesArchive, new int[] { 1,11,12,13,14,15,16,17,19,20,21,22,23,24,25,26,31,32,33,34,35,36,42,43,44 } },
            { combatArchive,        new int[] { 3,4,5,6,7,8,9,10,11,12,13,14,16 } },
            { academicArchive,      new int[] { 0,1,2,3,5,6,7,8,10 } },
            { miscArchive,          new int[] { 2,49,51,57 } },
            { randomTreasureArchive, new int[] { 0,1,3,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,36,37,38,39,40,43,44,45,46,47 } },
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
    }
}