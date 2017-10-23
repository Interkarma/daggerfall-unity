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

using DaggerfallConnect;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    public static class DungeonTextureTables
    {
        // Default dungeon texture table at linear offset 0x28617C
        public static int[] DefaultTextureTable = new int[] { 119, 120, 122, 123, 124, 168 };

        // Generates dungeon texture table from random seed
        public static int[] RandomTextureTable(int seed)
        {
            byte[] climateTextureArchiveIndices = { 0, 0, 1, 4, 4, 0, 3, 3, 3, 0 };
            short[] climateTextureArchives = { 19, 119, 319, 419, 119 };

            int climate = Game.GameManager.Instance.PlayerGPS.CurrentClimateIndex;
            if (climate == (int)MapsFile.Climates.Ocean)
                climate = (int)MapsFile.Climates.Swamp;

            int climateTextureArchiveIndicesIndex = Game.Utility.TravelTimeCalculator.climateIndices[climate - (int)MapsFile.Climates.Ocean];
            int climateTextureArchiveIndex = climateTextureArchiveIndices[climateTextureArchiveIndicesIndex];

            int textureArchiveOffset;
            DFRandom.srand(seed);

            int[] textureTable = (int[])DefaultTextureTable.Clone();

            if (climateTextureArchiveIndex != 1)
            {
                for (int i = 0; i < 5; ++i)
                {
                    textureArchiveOffset = DFRandom.random_range_inclusive(0, 4);
                    if (textureArchiveOffset == 2) // invalid
                        textureArchiveOffset = 4;
                    textureTable[i] = climateTextureArchives[climateTextureArchiveIndex] + textureArchiveOffset;
                }
            }

            int sewerTextureArchiveIndex = climate - (int)MapsFile.Climates.Desert;
            textureTable[5] = (int)DFLocation.ClimateTextureSet.Interior_Sewer + 100 * climateTextureArchiveIndices[sewerTextureArchiveIndex];
            return textureTable;
        }
    }
}