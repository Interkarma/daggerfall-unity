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
        #region Singleton

        private static readonly WorldDataReplacement instance = new WorldDataReplacement();

        private WorldDataReplacement() { }

        public static WorldDataReplacement Instance { get { return instance; } }

        #endregion


        #region Fields & Properties

        static readonly DFLocation nullLocation = new DFLocation();
        static readonly DFBlock nullBlock = new DFBlock();

        const int noReplacementBT = -1;
        static readonly BuildingReplacementData noReplacementData = new BuildingReplacementData() { BuildingType = noReplacementBT };

        static readonly string worldDataPath = Path.Combine(Application.streamingAssetsPath, "WorldData");

        private Dictionary<BlockRecordId, BuildingReplacementData> buildings = new Dictionary<BlockRecordId, BuildingReplacementData>();

        private static Dictionary<BlockRecordId, BuildingReplacementData> Buildings { get { return Instance.buildings; } }

        /// <summary>
        /// Path to custom world data on disk.
        /// </summary>
        public static string WorldDataPath { get { return worldDataPath; } }

        #endregion

        #region Public Methods

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

        public static bool GetDFLocationReplacementData(int regionIndex, int locationIndex, out DFLocation dfLocation)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                string fileName = GetDFLocationReplacementFilename(regionIndex, locationIndex);

                // Seek from loose files
                if (File.Exists(Path.Combine(worldDataPath, fileName)))
                {
                    string locationReplacementJson = File.ReadAllText(Path.Combine(worldDataPath, fileName));
                    dfLocation = (DFLocation)SaveLoadManager.Deserialize(typeof(DFLocation), locationReplacementJson);
                    Debug.LogFormat("Found DFLocation override: {0}, {1}", regionIndex, locationIndex);
                    return true;
                }
                // Seek from mods
                TextAsset locationReplacementJsonAsset;
                if (ModManager.Instance != null && ModManager.Instance.TryGetAsset(fileName, false, out locationReplacementJsonAsset))
                {
                    dfLocation = (DFLocation)SaveLoadManager.Deserialize(typeof(DFLocation), locationReplacementJsonAsset.text);
                    return true;
                }
            }
            dfLocation = nullLocation;
            return false;
        }

        // Currently only RDB block replacement is possible.
        // Replacing entire RMB blocks is not currently possible as RmbFldGroundData contains
        // groundData as a 2D array and FullSerializer can't do 2D arrays out of the box.
        // TODO - add cache so that the replacement data check isn't seeking files constantly.
        public static bool GetDFBlockReplacementData(int block, string blockName, out DFBlock dfBlock)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                string fileName = GetDFBlockReplacementFilename(blockName);

                // Seek from loose files
                if (File.Exists(Path.Combine(worldDataPath, fileName)))
                {
                    string blockReplacementJson = File.ReadAllText(Path.Combine(worldDataPath, fileName));
                    dfBlock = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), blockReplacementJson);
                    return true;
                }
                // Seek from mods
                TextAsset blockReplacementJsonAsset;
                if (ModManager.Instance != null && ModManager.Instance.TryGetAsset(fileName, false, out blockReplacementJsonAsset))
                {
                    dfBlock = (DFBlock)SaveLoadManager.Deserialize(typeof(DFBlock), blockReplacementJsonAsset.text);
                    return true;
                }
            }
            dfBlock = nullBlock;
            return false;
        }

        public static bool GetBuildingReplacementData(string blockName, int blockIndex, int recordIndex, out BuildingReplacementData buildingData)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                BlockRecordId blockRecordId = new BlockRecordId() { blockIndex = blockIndex, recordIndex = recordIndex };
                if (Buildings.ContainsKey(blockRecordId))
                {
                    buildingData = Buildings[blockRecordId];
                    return (buildingData.BuildingType != noReplacementBT);
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
                        Buildings.Add(blockRecordId, buildingData);
#endif
                        return true;
                    }
                    // Seek from mods
                    TextAsset buildingReplacementJsonAsset;
                    if (ModManager.Instance != null && ModManager.Instance.TryGetAsset(fileName, false, out buildingReplacementJsonAsset))
                    {
                        buildingData = (BuildingReplacementData)SaveLoadManager.Deserialize(typeof(BuildingReplacementData), buildingReplacementJsonAsset.text);
#if !UNITY_EDITOR       // Cache building replacement data, unless running in editor
                        Buildings.Add(blockRecordId, buildingData);
#endif
                        return true;
                    }
#if !UNITY_EDITOR   // Only look for replacement data once, unless running in editor
                    Buildings.Add(blockRecordId, noReplacementData);
#endif
                }
            }
            buildingData = noReplacementData;
            return false;
        }

        #endregion

    }

    #region FullSerializer custom processors

    public class RdbBlockDescProcessor : fsObjectProcessor
    {
        /*
        public override void OnBeforeSerialize(Type storageType, object instance)
        {
            // Invoked before serialization begins. Update any state inside of instance / etc.
            DFBlock.RdbBlockDesc blockData = (DFBlock.RdbBlockDesc) instance;
            for (int i=0; i < blockData.ModelReferenceList.Length; i++)
            {
                if (blockData.ModelReferenceList[i].ModelIdNum == 0)
                {
                    Array.Resize(ref blockData.ModelReferenceList, i);
                    break;
                }
            }
            instance = blockData;
            Debug.Log("RDB block");
        }*/

        // Invoked after serialization has finished. Update any state inside of instance, modify the output data, etc.
        public override void OnAfterSerialize(Type storageType, object instance, ref fsData data)
        {
            // Truncate ModelReferenceList at the first null entry in array[750].
            List<fsData> modelRefs = data.AsDictionary["ModelReferenceList"].AsList;
            for (int i = 0; i < modelRefs.Count; i++)
            {
                if (modelRefs[i].AsDictionary["ModelIdNum"].AsInt64 == 0)
                {
                    modelRefs.RemoveRange(i, modelRefs.Count - i);
                    break;
                }
            }
        }
    }

    public class RdbObjectProcessor : fsObjectProcessor
    {
        // Invoked after serialization has finished. Update any state inside of instance, modify the output data, etc.
        public override void OnAfterSerialize(Type storageType, object instance, ref fsData data)
        {
            // Only write relevant type resource data for Rdb Objects.
            Dictionary<string, fsData> rdbObject = data.AsDictionary;
            DFBlock.RdbResourceTypes type = (DFBlock.RdbResourceTypes) Enum.Parse(typeof(DFBlock.RdbResourceTypes), rdbObject["Type"].AsString);
            Dictionary<string, fsData> resources = rdbObject["Resources"].AsDictionary;

            if (type == DFBlock.RdbResourceTypes.Flat || type == DFBlock.RdbResourceTypes.Light)
                resources.Remove("ModelResource");
            if (type == DFBlock.RdbResourceTypes.Flat || type == DFBlock.RdbResourceTypes.Model)
                resources.Remove("LightResource");
            if (type == DFBlock.RdbResourceTypes.Model || type == DFBlock.RdbResourceTypes.Light)
                resources.Remove("FlatResource");
        }
    }

    #endregion
}
