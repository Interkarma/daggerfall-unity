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

using UnityEngine;
using System.Collections;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// A minimum sampler to produce rolling noise-based terrain.
    /// Does not use Daggerfall map data so usual coastlines and elevations not present.
    /// Creates lots of small hills and lakes. Locations may appear in water.
    /// </summary>
    public class NoiseTerrainSampler : TerrainSampler
    {
        const float baseHeightScale = 100f;

        public NoiseTerrainSampler()
        {
            MaxTerrainHeight = 2000;
            OceanElevation = 1;
            BeachElevation = 5;
        }

        public override void GenerateSamples(ref MapPixelData mapPixel)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Create samples array
            int dim = TerrainHelper.terrainSampleDim;
            mapPixel.samples = new WorldSample[dim * dim];

            // Populate samples
            float averageHeight = 0;
            float maxHeight = float.MinValue;
            for (int y = 0; y < dim; y++)
            {
                for (int x = 0; x < dim; x++)
                {
                    // It is important to use a continous noise function to avoid gaps between tiles
                    float latitude = mapPixel.mapPixelX * MapsFile.WorldMapTileDim + x;
                    float longitude = MapsFile.MaxWorldTileCoordZ - mapPixel.mapPixelY * MapsFile.WorldMapTileDim + y;
                    float height = TerrainHelper.GetNoise(dfUnity.ContentReader.Noise, latitude, longitude, 0.0025f, 0.9f, 0.7f, 2);
                    height *= baseHeightScale;

                    // Set sample 
                    mapPixel.samples[y * dim + x] = new WorldSample()
                    {
                        scaledHeight = height,
                    };

                    // Accumulate averages and max height
                    averageHeight += height;
                    if (height > maxHeight)
                        maxHeight = height;
                }
            }

            // Average and max heights are passed back for flattening location areas
            mapPixel.averageHeight = averageHeight /= (float)(dim * dim);
            mapPixel.maxHeight = maxHeight;
        }
    }
}