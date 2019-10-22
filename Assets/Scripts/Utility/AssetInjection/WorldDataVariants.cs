// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:
//

using System;
using System.IO;
using FullSerializer;
using UnityEngine;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Serialization;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Utility.AssetInjection
{

    public struct VariantBlockKey
    {
        public int locationKey;
        public string blockName;

        public VariantBlockKey(int locationKey, string blockName)
        {
            this.locationKey = locationKey;
            this.blockName = blockName;
        }
    }

    /// <summary>
    /// Handles variants of world data overrides for a more dynamic world.
    /// </summary>
    public static class WorldDataVariants
    {
        public const string NoVariant = "";
        public const int AnyLocationKey = -8;   // Cos 8 looks like infinity and also on keyboards has * symbol 

        static int lastLocationKey;

        static Dictionary<int, string> locationVariants = new Dictionary<int, string>();

        static Dictionary<VariantBlockKey, string> blockVariants = new Dictionary<VariantBlockKey, string>();
//            { { new VariantBlockKey(WorldDataReplacement.MakeLocationKey(17, 1260), "M0000004.RDB"), "_something" } };

        public static string GetLocationVariant(int regionIndex, int locationIndex)
        {
            return GetLocationVariant(WorldDataReplacement.MakeLocationKey(regionIndex, locationIndex));
        }

        public static string GetLocationVariant(int locationKey)
        {
            lastLocationKey = locationKey;
            return locationVariants.ContainsKey(locationKey) ? locationVariants[locationKey] : NoVariant;
        }

        public static string GetBlockVariant(string blockName)
        {
            if (lastLocationKey >= 0)
            {
                VariantBlockKey blockKey = new VariantBlockKey(lastLocationKey, blockName);
                if (blockVariants.ContainsKey(blockKey))
                    return blockVariants[blockKey];
                else
                {
                    blockKey.locationKey = AnyLocationKey;
                    if (blockVariants.ContainsKey(blockKey))
                        return blockVariants[blockKey];
                }
            }
            return NoVariant;
        }
    }
}