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

namespace DaggerfallWorkshop
{
    public static class TextureTables
    {
        // Default texture table at linear offset 0x28617C
        public static int[] DefaultTextureTable = new int[] { 119, 120, 122, 123, 124, 168 };

        // Helper to generate valid texture table from random seed
        public static int[] RandomTextureTable(int seed, DFLocation.ClimateBaseType climate)
        {
            byte[] climateTextureArchiveIndices = { 0, 0, 1, 4, 4, 0, 3, 3, 3, 0 };
            short[] climateTextureArchives = { 19, 119, 319, 419, 119 };

            int terrain = Game.GameManager.Instance.PlayerGPS.CurrentClimateIndex;
            int climateTextureArchiveIndicesIndex = Game.Utility.TravelTimeCalculator.climateIndices[terrain - (int)Game.Utility.TravelTimeCalculator.TerrainTypes.Ocean];
            int climateTextureArchiveIndex = climateTextureArchiveIndices[climateTextureArchiveIndicesIndex];

            int textureRecordOffset;
            DFRandom.srand(seed);

            int[] textureTable = (int[])DefaultTextureTable.Clone();

            if (climateTextureArchiveIndex != 1)
            {
                for (int i = 0; i < 5; ++i)
                {
                    textureRecordOffset = DFRandom.random_range_inclusive(0, 4);
                    if (textureRecordOffset == 2) // invalid
                        textureRecordOffset = 4;
                    textureTable[i] = climateTextureArchives[climateTextureArchiveIndex] + textureRecordOffset;
                }
            }

            textureTable[5] = (int)DFLocation.ClimateTextureSet.Interior_Sewer + (int)climate;
            return textureTable;
        }
    }
}