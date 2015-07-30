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
    /// A minimal terrain sampler to create perfectly flat terrain.
    /// Does not create any ocean or terrain features.
    /// </summary>
    public class SimpleTerrainSampler : TerrainSampler
    {
        const float height = 100;

        public SimpleTerrainSampler()
        {
            MaxTerrainHeight = 10;
            OceanElevation = 0;
            BeachElevation = 0;
        }

        public override void GenerateSamples(ref MapPixelData mapPixel)
        {
            // Create samples array
            int dim = TerrainHelper.terrainSampleDim;
            mapPixel.samples = new WorldSample[dim * dim];

            // Populate samples
            for (int y = 0; y < dim; y++)
            {
                for (int x = 0; x < dim; x++)
                {
                    // Set sample 
                    mapPixel.samples[y * dim + x] = new WorldSample()
                    {
                        scaledHeight = height,
                    };
                }
            }

            // Average and max heights are passed back for flattening location areas
            mapPixel.averageHeight = height;
            mapPixel.maxHeight = height;
        }
    }
}