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
    /// A minimal terrain sampler to create perfectly flat terrain.
    /// Does not create any ocean or terrain features.
    /// </summary>
    public class SimpleTerrainSampler : TerrainSampler
    {
        public override int Version
        {
            get { return 1; }
        }

        public SimpleTerrainSampler()
        {
            HeightmapDimension = defaultHeightmapDimension;
            MaxTerrainHeight = 1;
            MeanTerrainHeightScale = 1;
            OceanElevation = -1;
            BeachElevation = -1;
        }

        public override void GenerateSamples(ref MapPixelData mapPixel)
        {
            mapPixel.heightmapSamples = new float[HeightmapDimension, HeightmapDimension];
        }
    }
}