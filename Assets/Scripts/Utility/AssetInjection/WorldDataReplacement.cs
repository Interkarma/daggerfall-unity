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
    public struct BlockRecordId
    {
        public int blockIndex;
        public int recordIndex;
    }

    public struct BuildingReplacementData
    {
        public ushort FactionId;
        public int BuildingType;
        public byte Quality;
        public DFBlock.RmbSubRecord RmbSubRecord;
        public byte[] AutoMapData;      // for coloured map (optional)
    }

    /// <summary>
    /// Handles import and injection of custom block data with the purpose of providing modding support.
    /// Block data is imported from mod bundles with load order or loaded directly from disk.
    /// </summary>
    public class WorldDataReplacement
    {
        #region Fields & Constants

        const int noReplacementIndicator = -1;
        const string worldData = "WorldData";
        static readonly string worldDataPath = Path.Combine(Application.streamingAssetsPath, worldData);

        // No replacement found indicator structures.
        static readonly DFRegion noReplacementRegion = new DFRegion() { LocationCount = 0 };    // Use 0 as it's a uint
        static readonly DFLocation noReplacementLocation = new DFLocation() { LocationIndex = noReplacementIndicator };
        static readonly DFBlock noReplacementBlock = new DFBlock() { Index = noReplacementIndicator };
        static readonly BuildingReplacementData noReplacementBuilding = new BuildingReplacementData() { BuildingType = noReplacementIndicator };

        // Replacement world data caches.
        private static Dictionary<int, DFRegion> regions = new Dictionary<int, DFRegion>();
        private static Dictionary<int, DFLocation> locations = new Dictionary<int, DFLocation>();
        private static Dictionary<string, DFBlock> blocks = new Dictionary<string, DFBlock>();
        private static Dictionary<BlockRecordId, BuildingReplacementData> buildings = new Dictionary<BlockRecordId, BuildingReplacementData>();

        // New block index/name mappings.
        private static int nextBlockIndex;
        private static Dictionary<int, string> newBlockNames = new Dictionary<int, string>();
        private static Dictionary<string, int> newBlockIndices = new Dictionary<string, int>();

        /// <summary>
        /// Path to custom world data on disk.
        /// </summary>
        public static string WorldDataPath { get { return worldDataPath; } }

        #endregion

        #region Public Filename Methods

        public static string GetDFRegionReplacementFilename(int regionIndex)
        {
            return string.Format("region-{0}.json", regionIndex);
        }

        public static string GetDFLocationReplacementFilename(int regionIndex, int locationIndex)
        {
            return string.Format("location-{0}-{1}.json", regionIndex, locationIndex);
        }

        public static string GetDFBlockReplacementFilename(string blockName)
        {
            return string.Format("{0}.json", blockName);
        }

        public static string GetBuildingReplacementFilename(string blockName, int blockIndex, int recordIndex)
        {
            return string.Format("{0}-{1}-building{2}.json", blockName, blockIndex, recordIndex);
        }

        #endregion

        #region Public WorldData Replacement Methods

        /// <summary>
        /// Checks a region for added location data. (i.e. new locations)
        /// </summary>
        /// <param name="regionIndex">Region index</param>
        /// <param name="locationIndex">Location index</param>
        /// <param name="dfRegion">DFRegion data output updated with any added locations found</param>
        /// <returns>True if added blocks had indices assigned, false otherwise</returns>
        public static bool GetDFRegionAdditionalLocationData(int regionIndex, ref DFRegion dfRegion)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                // If found, return a previously cached DFRegion
                if (regions.ContainsKey(regionIndex))
                {
                    if (regions[regionIndex].LocationCount != noReplacementRegion.LocationCount)
                    {
                        dfRegion = regions[regionIndex];
                        return true;
                    }
                    return false;
                }
                // Setup local lists for the region arrays and record the location count from data
                uint dataLocationCount = dfRegion.LocationCount;
                List<string> mapNames = new List<string>(dfRegion.MapNames);
                List<DFRegion.RegionMapTable> mapTable = new List<DFRegion.RegionMapTable>(dfRegion.MapTable);
                bool newBlocksAssigned = false;

                // Seek from loose files
                string locationPattern = string.Format("locationnew-*-{0}.json", regionIndex);
                string[] fileNames = Directory.GetFiles(worldDataPath, locationPattern);
                foreach (string fileName in fileNames)
                {
                    string locationReplacementJson = File.ReadAllText(Path.Combine(worldDataPath, fileName));
                    DFLocation dfLocation = (DFLocation)SaveLoadManager.Deserialize(typeof(DFLocation), locationReplacementJson);
                    newBlocksAssigned = AddLocationToRegion(regionIndex, ref dfRegion, ref mapNames, ref mapTable, dfLocation);
                }
                // Seek from mods
                if (ModManager.Instance != null)
                {
                    string locationExtension = string.Format("-{0}.json", regionIndex);
                    List<TextAsset> assets = ModManager.Instance.FindAssets<TextAsset>(worldData, locationExtension);
                    if (assets != null)
                    {
                        foreach (TextAsset locationReplacementJsonAsset in assets)
                        {
                            DFLocation dfLocation = (DFLocation)SaveLoadManager.Deserialize(typeof(DFLocation), locationReplacementJsonAsset.text);
                            newBlocksAssigned &= AddLocationToRegion(regionIndex, ref dfRegion, ref mapNames, ref mapTable, dfLocation);
                        }
                    }
                }
                // If found any new locations for this region,
                if (dfRegion.LocationCount > dataLocationCount)
                {
                    // Update the region arrays from local lists
                    dfRegion.MapNames = mapNames.ToArray();
                    dfRegion.MapTable = mapTable.ToArray();

#if !UNITY_EDITOR  // Cache region data for added locations if new blocks have been assigned indices (unless running in editor)
                    if (newBlocksAssigned)
                        regions.Add(regionIndex, dfRegion);
#endif
                    Debug.LogFormat("Added {0} new DFLocation's to region {1}, indexes: {2} - {3}",
                        dfRegion.LocationCount - dataLocationCount, regionIndex, dataLocationCount, dfRegion.LocationCount-1);
                    return true;
                }

#if !UNITY_EDITOR // Cache that there's no replacement region data, so only look for added locations once per region (unless running in editor)
                regions.Add(regionIndex, noReplacementRegion);
#endif
            }
            return false;
        }

        /// <summary>
        /// Checks for replacement location data.
        /// </summary>
        /// <param name="regionIndex">Region index</param>
        /// <param name="locationIndex">Location index</param>
        /// <param name="dfLocation">DFLocation data output</param>
        /// <returns>True if replacement data found, false otherwise</returns>
        public static bool GetDFLocationReplacementData(int regionIndex, int locationIndex, out DFLocation dfLocation)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                // If found, return a previously cached DFLocation
                int locationKey = MakeLocationKey(regionIndex, locationIndex);
                if (locations.ContainsKey(locationKey))
                {
                    dfLocation = locations[locationKey];
                    return dfLocation.LocationIndex != noReplacementLocation.LocationIndex;
                }

                string fileName = GetDFLocationReplacementFilename(regionIndex, locationIndex);
                TextAsset locationReplacementJsonAsset;

                // Seek from loose files
                if (File.Exists(Path.Combine(worldDataPath, fileName)))
                {
                    string locationReplacementJson = File.ReadAllText(Path.Combine(worldDataPath, fileName));
                    dfLocation = (DFLocation)SaveLoadManager.Deserialize(typeof(DFLocation), locationReplacementJson);
                }
                // Seek from mods
                else if (ModManager.Instance != null && ModManager.Instance.TryGetAsset(fileName, false, out locationReplacementJsonAsset))
                {
                    dfLocation = (DFLocation)SaveLoadManager.Deserialize(typeof(DFLocation), locationReplacementJsonAsset.text);
                }
                else
                {
#if !UNITY_EDITOR // Cache that there's no replacement location data, so only look for replaced locations once (unless running in editor)
                    locations[locationKey] = noReplacementLocation;
#endif
                    dfLocation = noReplacementLocation;
                    return false;
                }
                // Assign any new blocks in this location a block index if they haven't already been assigned
                if (AssignBlockIndices(dfLocation))
                {
#if !UNITY_EDITOR   // Cache location data for replaced locations if new blocks have been assigned indices (unless running in editor)
                    locations[locationKey] = dfLocation;
#endif
                }
                Debug.LogFormat("Found DFLocation override, region:{0}, index:{1}", regionIndex, locationIndex);
                return true;
            }
            dfLocation = noReplacementLocation;
            return false;
        }

        /// <summary>
        /// Gets the index for a new added block
        /// </summary>
        /// <param name="blockName">Block name</param>
        /// <returns>Block index, or -1 if not found</returns>
        public static int GetNewDFBlockIndex(string blockName)
        {
            if (newBlockIndices.ContainsKey(blockName))
                return newBlockIndices[blockName];
            else
                return -1;
        }

        /// <summary>
        /// Gets the name for a new added block
        /// </summary>
        /// <param name="block">Block index</param>
        /// <returns>Block name, or null if not found</returns>
        public static string GetNewDFBlockName(int block)
        {
            if (newBlockNames.ContainsKey(block))
                return newBlockNames[block];
            else
                return null;
        }

        /// <summary>
        /// Checks for replacement block data. Only RDB block replacement is currently possible.
        /// </summary>
        /// Replacing entire RMB blocks is not currently possible as RmbFldGroundData contains
        /// groundData as a 2D array and FullSerializer can't do 2D arrays out of the box.
        /// <param name="block">Block index</param>
        /// <param name="blockName">Block name</param>
        /// <param name="dfBlock">DFBlock data output</param>
        /// <returns>True if replacement data found, false otherwise</returns>
        public static bool GetDFBlockReplacementData(int block, string blockName, out DFBlock dfBlock)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                // Check the block cache
                if (blocks.ContainsKey(blockName))
                {
                    dfBlock = blocks[blockName];
                    return dfBlock.Index != noReplacementBlock.Index;
                }

                string fileName = GetDFBlockReplacementFilename(blockName);
                TextAsset blockReplacementJsonAsset;

                // Seek from loose files
                if (File.Exists(Path.Combine(worldDataPath, fileName)))
                {
                    string blockReplacementJson = File.ReadAllText(Path.Combine(worldDataPath, fileName));
                    dfBlock = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), blockReplacementJson);
                }
                // Seek from mods
                else if (ModManager.Instance != null && ModManager.Instance.TryGetAsset(fileName, false, out blockReplacementJsonAsset))
                {
                    dfBlock = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), blockReplacementJsonAsset.text);
                }
                else
                {
#if !UNITY_EDITOR // Cache that there's no replacement block data, so only look for replaced blocks once (unless running in editor)
                    blocks[blockName] = noReplacementBlock;
#endif
                    dfBlock = noReplacementBlock;
                    return false;
                }
                dfBlock.Index = block;
#if !UNITY_EDITOR   // Cache block data for added/replaced blocks (unless running in editor)
                blocks[blockName] = dfBlock;
#endif
                Debug.LogFormat("Found DFBlock override: {0} (index: {1})", blockName, block);
                return true;
            }
            dfBlock = noReplacementBlock;
            return false;
        }

        /// <summary>
        /// Checks for replacement building data within a block.
        /// </summary>
        /// <param name="blockName">Block name</param>
        /// <param name="blockIndex">Block index</param>
        /// <param name="recordIndex">Record index of building within block</param>
        /// <param name="buildingData">BuildingReplacementData output</param>
        /// <returns>True if replacement data found, false otherwise</returns>
        public static bool GetBuildingReplacementData(string blockName, int blockIndex, int recordIndex, out BuildingReplacementData buildingData)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                BlockRecordId blockRecordId = new BlockRecordId() { blockIndex = blockIndex, recordIndex = recordIndex };
                if (buildings.ContainsKey(blockRecordId))
                {
                    buildingData = buildings[blockRecordId];
                    return (buildingData.BuildingType != noReplacementIndicator);
                }
                else
                {
                    string fileName = GetBuildingReplacementFilename(blockName, blockIndex, recordIndex);

                    // Seek from loose files
                    if (File.Exists(Path.Combine(worldDataPath, fileName)))
                    {
                        string buildingReplacementJson = File.ReadAllText(Path.Combine(worldDataPath, fileName));
                        buildingData = (BuildingReplacementData)SaveLoadManager.Deserialize(typeof(BuildingReplacementData), buildingReplacementJson);
#if !UNITY_EDITOR       // Cache building replacement data, unless running in editor
                        buildings.Add(blockRecordId, buildingData);
#endif
                        return true;
                    }
                    // Seek from mods
                    TextAsset buildingReplacementJsonAsset;
                    if (ModManager.Instance != null && ModManager.Instance.TryGetAsset(fileName, false, out buildingReplacementJsonAsset))
                    {
                        buildingData = (BuildingReplacementData)SaveLoadManager.Deserialize(typeof(BuildingReplacementData), buildingReplacementJsonAsset.text);
#if !UNITY_EDITOR       // Cache building replacement data, unless running in editor
                        buildings.Add(blockRecordId, buildingData);
#endif
                        return true;
                    }
#if !UNITY_EDITOR   // Only look for replacement data once, unless running in editor
                    buildings.Add(blockRecordId, noReplacementBuilding);
#endif
                }
            }
            buildingData = noReplacementBuilding;
            return false;
        }

        #endregion

        #region Private Methods

        static int MakeLocationKey(int regionIndex, int locationIndex)
        {
            return (locationIndex * 100) + regionIndex;
        }

        private static bool AddLocationToRegion(int regionIndex, ref DFRegion dfRegion, ref List<string> mapNames, ref List<DFRegion.RegionMapTable> mapTable, DFLocation dfLocation)
        {
            // Copy the location id for ReadLocationIdFast() to use instead of peeking the classic data files
            dfLocation.MapTableData.LocationId = dfLocation.Exterior.RecordElement.Header.LocationId;

            // Add location to region using next index value
            int locationIndex = (int)dfRegion.LocationCount++;
            mapNames.Add(dfLocation.Name);
            dfLocation.LocationIndex = locationIndex;
            dfLocation.Exterior.RecordElement.Header.Unknown2 = (uint)locationIndex;
            mapTable.Add(dfLocation.MapTableData);
            dfRegion.MapIdLookup.Add(dfLocation.MapTableData.MapId, locationIndex);
            dfRegion.MapNameLookup.Add(dfLocation.Name, locationIndex);

            // Store location replacement/addition
            locations[MakeLocationKey(regionIndex, locationIndex)] = dfLocation;

            // Assign any new blocks in this location a block index if they haven't already been assigned
            return AssignBlockIndices(dfLocation);
        }

        private static bool AssignBlockIndices(DFLocation dfLocation)
        {
            ContentReader reader = DaggerfallUnity.Instance.ContentReader;
            if (reader != null)
            {
                BlocksFile blocksFile = reader.BlockFileReader;
                if (blocksFile != null)
                {
                    if (nextBlockIndex == 0)
                        nextBlockIndex = blocksFile.BsaFile.Count;

                    // RMB blocks
                    foreach (string blockName in dfLocation.Exterior.ExteriorData.BlockNames)
                        if (blocksFile.GetBlockIndex(blockName) == -1)
                            AssignNextIndex(blockName);

                    // RDB blocks
                    foreach (DFLocation.DungeonBlock dungeonBlock in dfLocation.Dungeon.Blocks)
                    {
                        string blockName = dungeonBlock.BlockName;
                        if (blocksFile.GetBlockIndex(blockName) == -1)
                            AssignNextIndex(blockName);
                    }
                    return true;
                }
            }
            return false;
        }

        private static void AssignNextIndex(string blockName)
        {
            newBlockNames[nextBlockIndex] = blockName;
            newBlockIndices[blockName] = nextBlockIndex;
            Debug.LogFormat("Found a new DFBlock: {0}, (assigned index: {1})", blockName, nextBlockIndex);
            nextBlockIndex++;
        }

        #endregion
    }
}
