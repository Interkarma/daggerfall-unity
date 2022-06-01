// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// A sampler to produce rolling noise-based terrain.
    /// Does not use Daggerfall map data so usual coastlines and elevations not present.
    /// </summary>
    public class NoiseTerrainSampler : TerrainSampler
    {
        public float Scale = 1;

        public override int Version
        {
            get { return 1; }
        }

        public NoiseTerrainSampler()
        {
            HeightmapDimension = defaultHeightmapDimension;
            MaxTerrainHeight = 200;
            MeanTerrainHeightScale = Scale;
            OceanElevation = -1;
            BeachElevation = -1;
        }

        public override void GenerateSamples(ref MapPixelData mapPixel)
        {
            mapPixel.heightmapSamples = new float[HeightmapDimension, HeightmapDimension];

            // Populate heightmap
            float averageHeight = 0;
            float maxHeight = float.MinValue;
            for (int y = 0; y < HeightmapDimension; y++)
            {
                for (int x = 0; x < HeightmapDimension; x++)
                {
                    // It is important to use a continuous noise function to avoid gaps between tiles
                    int noisex = mapPixel.mapPixelX * (HeightmapDimension - 1) + x;
                    int noisey = (MapsFile.MaxMapPixelY - mapPixel.mapPixelY) * (HeightmapDimension - 1) + y;
                    float height = TerrainHelper.GetNoise(noisex, noisey, 0.01f, 0.5f, 0.1f, 2) * Scale;
                    mapPixel.heightmapSamples[y, x] = height;

                    // Accumulate averages and max height
                    averageHeight += height;
                    if (height > maxHeight)
                        maxHeight = height;
                }
            }

            // Average and max heights are passed back for flattening location areas
            mapPixel.averageHeight = averageHeight /= (float)(HeightmapDimension * HeightmapDimension);
            mapPixel.maxHeight = maxHeight;
        }
    }
}