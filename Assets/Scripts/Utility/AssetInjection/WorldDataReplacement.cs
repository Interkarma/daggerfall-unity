// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:
//

using System.IO;
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

        public static string GetBuildingReplacementFilename(string blockName, int blockIndex, int recordIndex)
        {
            return string.Format("{0}-{1}-building{2}.json", blockName, blockIndex, recordIndex);
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
}
