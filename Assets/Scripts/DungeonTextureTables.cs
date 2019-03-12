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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    public static class DungeonTextureTables
    {
        public const int TableLength = 6;
        // Default dungeon texture table at linear offset 0x28617C
        public static int[] DefaultTextureTable = new int[] { 119, 120, 122, 123, 124, 168 };

        public static int[] RandomTextureTableClassic(int seed, int randomDungeonTextures = 0)
        {
            byte[] climateTextureArchiveIndices = { 0, 0, 1, 4, 4, 0, 3, 3, 3, 0 };
            short[] climateTextureArchives;
            if (randomDungeonTextures == 0)
                climateTextureArchives = new short[] { 19, 119, 319, 419, 119 }; // Values from classic, used in classic algorithm
            else
                climateTextureArchives = new short[] { 19, 119, 119, 319, 419 }; // Values for climate-based algorithm. Index 2 (unused) is a dummy value.

            int climate = Game.GameManager.Instance.PlayerGPS.CurrentClimateIndex;
            if (climate == (int)MapsFile.Climates.Ocean)
                climate = (int)MapsFile.Climates.Swamp;

            int classicIndexValue = Game.Utility.TravelTimeCalculator.climateIndices[climate - (int)MapsFile.Climates.Ocean];
            int climateBasedIndexValue = climate - (int)MapsFile.Climates.Desert;

            int climateTextureArchiveIndex;
            if (randomDungeonTextures == 0) // classic algorithm
                climateTextureArchiveIndex = climateTextureArchiveIndices[classicIndexValue];
            else // climate-based algorithm
                climateTextureArchiveIndex = climateTextureArchiveIndices[climateBasedIndexValue];

            int textureArchiveOffset;
            DFRandom.srand(seed);
            int[] textureTable = new int[TableLength];

            // In classic, if climateTextureArchiveIndex is 1 here (only happens with rainforest climate), the following loop is skipped and
            // a dungeon in that climate will have the texture table of the last visited dungeon or, if no dungeons
            // had been visited since starting the program, the default dungeon textures. This seems like it must just be a bug, so the
            // recreation of the classic algorithm here assigns textures even if climateTextureArchiveIndex is 1.
            for (int i = 0; i < 5; ++i)
            {
                textureArchiveOffset = DFRandom.random_range_inclusive(0, 4);
                if (textureArchiveOffset == 2) // invalid
                    textureArchiveOffset = 4;
                textureTable[i] = climateTextureArchives[climateTextureArchiveIndex] + textureArchiveOffset;
            }

            textureTable[5] = (int)DFLocation.ClimateTextureSet.Interior_Sewer + 100 * climateTextureArchiveIndices[climateBasedIndexValue];
            return textureTable;
        }

        // Randomly pick from all dungeon texture tables. This is a DF Unity implementation different from classic.
        public static int[] RandomTextureTableAlternate(int seed)
        {
            // Valid dungeon textures table indices
            int[] valids = new int[]
            {
                019, 020, 022, 023, 024, 068,
                119, 120, 122, 123, 124, 168,
                319, 320, 322, 323, 324, 368,
                419, 420, 422, 423, 424, 468,
            };

            Random.InitState(seed);
            int[] textureTable = new int[TableLength];
            for (int i = 0; i < TableLength; i++)
            {
                textureTable[i] = valids[Random.Range(0, valids.Length)];
            }

            // Make sure sewer textures are used for last slot, or errors can occur because sewer archives have more records than the others
            int[] validSewerArchives = { 68, 168, 368, 468 };
            if ((textureTable[5] % 100) != 68)
                textureTable[5] = validSewerArchives[Random.Range(0, validSewerArchives.Length)];

            return textureTable;
        }
    }
}