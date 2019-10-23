// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:
//

using FullSerializer;
using System.Collections.Generic;

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

        static List<int> newLocationVariants = new List<int>();
        static Dictionary<int, string> locationVariants = new Dictionary<int, string>();
//            { { WorldDataReplacement.MakeLocationKey(17, 1331), "_something" } };

        static Dictionary<VariantBlockKey, string> blockVariants = new Dictionary<VariantBlockKey, string>();
//            { { new VariantBlockKey(WorldDataReplacement.MakeLocationKey(17, 1260), "M0000004.RDB"), "_something" } };

        static Dictionary<BlockRecordKey, string> buildingVariants = new Dictionary<BlockRecordKey, string>();
//            { { new BlockRecordKey() {blockIndex=543, recordIndex=10}, "_test" } };

        #region Setters for variants

        /// <summary>
        /// Sets a variant for a location.
        /// </summary>
        /// <param name="regionIndex">Region index</param>
        /// <param name="locationIndex">Location index</param>
        /// <param name="variant">Variant name</param>
        /// <returns>True if overwriting an existing variant set for this location</returns>
        public static bool SetLocationVariant(int regionIndex, int locationIndex, string variant)
        {
            int locationKey = WorldDataReplacement.MakeLocationKey(regionIndex, locationIndex);
            bool overwrite = !locationVariants.ContainsKey(locationKey);
            locationVariants[locationKey] = variant;
            return overwrite;
        }

        /// <summary>
        /// Sets a variant for a new location.
        /// </summary>
        /// <param name="regionIndex">Region index</param>
        /// <param name="locationName">Location name (as index is assigned at runtime)</param>
        /// <param name="variant">Variant name</param>
        /// <returns>True if overwriting an existing variant set for this location</returns>
        public static bool SetNewLocationVariant(int regionIndex, string locationName, string variant)
        {
            int locationIndex = WorldDataReplacement.GetNewDFLocationIndex(regionIndex, locationName);
            if (locationIndex >= 0)
            {
                int locationKey = WorldDataReplacement.MakeLocationKey(regionIndex, locationIndex);
                bool overwrite = !locationVariants.ContainsKey(locationKey);
                locationVariants[locationKey] = variant;
                newLocationVariants.Add(locationKey);
                return overwrite;
            }
            DaggerfallUnity.LogMessage("Failed to set a new location variant.", true);
            return false;
        }

        /// <summary>
        /// Sets a variant for a block.
        /// </summary>
        /// <param name="blockName">Block name</param>
        /// <param name="variant">Variant name</param>
        /// <param name="locationKey">Location key if the variant is only for a specific location</param>
        /// <returns>True if overwriting an existing variant set for this location</returns>
        public static bool SetBlockVariant(string blockName, string variant, int locationKey = AnyLocationKey)
        {
            VariantBlockKey blockKey = new VariantBlockKey(locationKey, blockName);
            bool overwrite = !blockVariants.ContainsKey(blockKey);
            blockVariants[blockKey] = variant;
            return overwrite;
        }

        #endregion

        #region Getters for variants set for the current play session

        public static string GetLocationVariant(int locationKey, out bool newLocation)
        {
            lastLocationKey = locationKey;
            newLocation = false;
            string result = NoVariant;
            if (locationVariants.TryGetValue(locationKey, out result))
                newLocation = newLocationVariants.Contains(locationKey);

            return result;
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

        public static string GetBuildingVariant(ref BlockRecordKey blockRecordKey)
        {
            if (blockRecordKey.variant == NoVariant)
            {
                // TODO: add location filtering like above!
                if (buildingVariants.ContainsKey(blockRecordKey))
                {
                    string variant = buildingVariants[blockRecordKey];
                    blockRecordKey.variant = variant;
                    return variant;
                }
            }
            return NoVariant;
        }


        #endregion

        #region Save, Load & Clear

        public static void Clear()
        {
            newLocationVariants.Clear();
            locationVariants.Clear();
            blockVariants.Clear();
            buildingVariants.Clear();
        }

        public static WorldVariationData_v1 GetWorldVariationSaveData()
        {
            WorldVariationData_v1 data = new WorldVariationData_v1()
            {
                newLocationVariants = newLocationVariants,
                locationVariants = locationVariants,
                blockVariants = blockVariants,
                buildingVariants = buildingVariants
            };
            return data;
        }

        public static void RestoreWorldVariationData(WorldVariationData_v1 worldVariationData)
        {
            if (worldVariationData != null)
            {
                if (worldVariationData.newLocationVariants != null)
                    newLocationVariants = worldVariationData.newLocationVariants;
                if (worldVariationData.locationVariants != null)
                    locationVariants = worldVariationData.locationVariants;
                if (worldVariationData.blockVariants != null)
                    blockVariants = worldVariationData.blockVariants;
                if (worldVariationData.buildingVariants != null)
                    buildingVariants = worldVariationData.buildingVariants;
            }
        }

        [fsObject("v1")]
        public class WorldVariationData_v1
        {
            public List<int> newLocationVariants;

            public Dictionary<int, string> locationVariants;

            public Dictionary<VariantBlockKey, string> blockVariants;

            public Dictionary<BlockRecordKey, string> buildingVariants;

        }

        #endregion
    }
}