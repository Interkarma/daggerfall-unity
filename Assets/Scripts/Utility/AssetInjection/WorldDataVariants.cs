// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:
//

using FullSerializer;
using System.Collections.Generic;
using UnityEngine;

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

    public struct VariantBuildingKey
    {
        public int locationKey;
        public string blockName;
        public int recordIndex;

        public VariantBuildingKey(int locationKey, string blockName, int recordIndex)
        {
            this.locationKey = locationKey;
            this.blockName = blockName;
            this.recordIndex = recordIndex;
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

        static Dictionary<VariantBlockKey, string> blockVariants = new Dictionary<VariantBlockKey, string>();

        static Dictionary<VariantBuildingKey, string> buildingVariants = new Dictionary<VariantBuildingKey, string>();

        /// <summary>
        /// Sets the last location key to specified location in region. Used before entering buildings to
        /// ensure that the correct location specific variant can be found.
        /// </summary>
        public static void SetLastLocationKeyTo(int regionIndex, int locationIndex)
        {
            lastLocationKey = WorldDataReplacement.MakeLocationKey(regionIndex, locationIndex);
        }

        #region Setters for variants

        /// <summary>
        /// Sets a variant for a location.
        /// </summary>
        /// <param name="regionIndex">Region index</param>
        /// <param name="locationIndex">Location index</param>
        /// <param name="variant">Variant name</param>
        /// <returns>True unless this overwrote an existing variant set for this location</returns>
        public static bool SetLocationVariant(int regionIndex, int locationIndex, string variant)
        {
            int locationKey = WorldDataReplacement.MakeLocationKey(regionIndex, locationIndex);
            bool added = locationVariants.ContainsKey(locationKey);
            if (variant == NoVariant)
                locationVariants.Remove(locationKey);
            else
                locationVariants[locationKey] = variant;

            Debug.LogFormat("Set variant \"{0}\" for the location index \"{1}\" in region {2}", variant, locationIndex, regionIndex);
            RMBLayout.ClearLocationCache();
            return added;
        }

        /// <summary>
        /// Sets a variant for a new location.
        /// </summary>
        /// <param name="regionIndex">Region index</param>
        /// <param name="locationName">Location name (as index is assigned at runtime)</param>
        /// <param name="variant">Variant name</param>
        /// <returns>True unless this overwrote an existing variant set for this location</returns>
        public static bool SetNewLocationVariant(int regionIndex, string locationName, string variant)
        {
            int locationIndex = WorldDataReplacement.GetNewDFLocationIndex(regionIndex, locationName);
            if (locationIndex >= 0)
            {
                int locationKey = WorldDataReplacement.MakeLocationKey(regionIndex, locationIndex);
                bool added = !locationVariants.ContainsKey(locationKey);
                if (variant == NoVariant)
                    locationVariants.Remove(locationKey);
                else
                    locationVariants[locationKey] = variant;
                
                if (!newLocationVariants.Contains(locationKey))
                    newLocationVariants.Add(locationKey);

                Debug.LogFormat("Set variant \"{0}\" for the new location \"{1}\" in region {2}", variant, locationName, regionIndex);
                RMBLayout.ClearLocationCache();
                return added;
            }
            DaggerfallUnity.LogMessage("Failed to set a new location variant.", true);
            return true;
        }

        /// <summary>
        /// Sets a variant for a block.
        /// </summary>
        /// <param name="blockName">Block name</param>
        /// <param name="variant">Variant name</param>
        /// <param name="locationKey">Location key if the variant is only for a specific location</param>
        /// <returns>True unless this overwrote an existing block variant set for this location</returns>
        public static bool SetBlockVariant(string blockName, string variant, int locationKey = AnyLocationKey)
        {
            VariantBlockKey blockKey = new VariantBlockKey(locationKey, blockName);
            bool added = !blockVariants.ContainsKey(blockKey);
            if (variant == NoVariant)
                blockVariants.Remove(blockKey);
            else
                blockVariants[blockKey] = variant;

            Debug.LogFormat("Set variant \"{0}\" for the block {1} at locationKey {2}", variant, blockName, locationKey);
            RMBLayout.ClearLocationCache();
            return added;
        }

        /// <summary>
        /// Sets a variant for a building. (block record)
        /// </summary>
        /// <param name="blockName">Block name</param>
        /// <param name="variant">Variant name</param>
        /// <param name="locationKey">Location key if the variant is only for a specific location</param>
        /// <returns>True unless this overwrote an existing building variant set for this location</returns>
        public static bool SetBuildingVariant(string blockName, int recordIndex, string variant, int locationKey = AnyLocationKey)
        {
            VariantBuildingKey buildingKey = new VariantBuildingKey(locationKey, blockName, recordIndex);
            bool added = !buildingVariants.ContainsKey(buildingKey);
            if (variant == NoVariant)
                buildingVariants.Remove(buildingKey);
            else
                buildingVariants[buildingKey] = variant;

            Debug.LogFormat("Set variant \"{0}\" for building {2} of {1} at locationKey {3}", variant, blockName, recordIndex, locationKey);
            RMBLayout.ClearLocationCache();
            return added;
        }

        #endregion

        #region Getters for variants set for the current play session

        /// <summary>
        /// Gets a variant set for the specified location. Sets the last location checked.
        /// </summary>
        /// <param name="locationKey">Location key if the variant is only for a specific location</param>
        /// <param name="newLocation">True if this variant is for a new location (i.e. not vanilla)</param>
        /// <returns>The variant string if found, else WorldDataVariants.NoVariant (empty string)</returns>
        public static string GetLocationVariant(int locationKey, out bool newLocation)
        {
            lastLocationKey = locationKey;
            newLocation = false;
            string result = NoVariant;
            if (locationVariants.TryGetValue(locationKey, out result))
                newLocation = newLocationVariants.Contains(locationKey);

            return result;
        }

        /// <summary>
        /// Gets a variant set for the specified block, based on the last location variant checked using GetLocationVariant().
        /// </summary>
        /// <param name="blockName">Block name</param>
        /// <returns>The variant string if found, else WorldDataVariants.NoVariant (empty string)</returns>
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

        /// <summary>
        /// Gets a variant set for the specified building, based on last location variant checked using GetLocationVariant().
        /// </summary>
        /// <param name="blockRecordKey">Block record key with the record index to use</param>
        /// <param name="blockName">Block name</param>
        /// <returns>The variant string if found, else WorldDataVariants.NoVariant (empty string)</returns>
        public static string GetBuildingVariant(ref BlockRecordKey blockRecordKey, string blockName)
        {
            if (blockRecordKey.variant == NoVariant)
            {
                VariantBuildingKey buildingKey = new VariantBuildingKey(lastLocationKey, blockName, blockRecordKey.recordIndex);
                if (buildingVariants.ContainsKey(buildingKey))
                {
                    string variant = buildingVariants[buildingKey];
                    blockRecordKey.variant = variant;
                    return variant;
                }
                else
                {
                    buildingKey.locationKey = AnyLocationKey;
                    if (buildingVariants.ContainsKey(buildingKey))
                    {
                        string variant = buildingVariants[buildingKey];
                        blockRecordKey.variant = variant;
                        return variant;
                    }
                }
            }
            return NoVariant;
        }

        /// <summary>
        /// Gets a variant set for the specified block, based on supplied region and location.
        /// </summary>
        /// <param name="regionIndex">e.g. DFLocation.RegionIndex</param>
        /// <param name="locationIndex">e.g. DFLocation.LocationIndex</param>
        /// <param name="blockName">Block name</param>
        /// <returns>The variant string if found, or null</returns>
        public static string GetBlockVariant(int regionIndex, int locationIndex, string blockName)
        {
            int locationKey = WorldDataReplacement.MakeLocationKey(regionIndex, locationIndex);
            VariantBlockKey blockKey = new VariantBlockKey(locationKey, blockName);
            return blockVariants.ContainsKey(blockKey) ? blockVariants[blockKey] : null;
        }

        /// <summary>
        /// Gets a variant set for the specified building, based on supplied region and location.
        /// </summary>
        /// <param name="regionIndex">e.g. DFLocation.RegionIndex</param>
        /// <param name="locationIndex">e.g. DFLocation.LocationIndex</param>
        /// <param name="blockName">Block name</param>
        /// <param name="recordIndex">Building record index</param>
        /// <returns>The variant string if found, or null</returns>
        public static string GetBuildingVariant(int regionIndex, int locationIndex, string blockName, int recordIndex)
        {
            int locationKey = WorldDataReplacement.MakeLocationKey(regionIndex, locationIndex);
            VariantBuildingKey buildingKey = new VariantBuildingKey(locationKey, blockName, recordIndex);
            return buildingVariants.ContainsKey(buildingKey) ? buildingVariants[buildingKey] : null;
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

            public Dictionary<VariantBuildingKey, string> buildingVariants;
        }

        #endregion
    }
}