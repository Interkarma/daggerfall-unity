// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    public struct BlockRecordId
    {
        public int blockIndex;
        public int recordIndex;
    }

    public struct BuildingReplacementData
    {
        public ushort factionId;
        public int buildingType;
        public DFBlock.RmbSubRecord rmbSubRecord;
        // TODO: autoMapData for coloured map?
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
            if (DaggerfallUnity.Settings.MeshAndTextureReplacement)
            {
                BlockRecordId blockRecordId = new BlockRecordId() { blockIndex = blockIndex, recordIndex = recordIndex };
                if (!Buildings.ContainsKey(blockRecordId))
                {
                    if (File.Exists(Path.Combine(worldDataPath, GetBuildingReplacementFilename(blockName, blockIndex, recordIndex))))
                    {
                        string fileName = GetBuildingReplacementFilename(blockName, blockIndex, recordIndex);
                        string buildingReplacementJson = File.ReadAllText(Path.Combine(worldDataPath, fileName));
                        buildingData = (BuildingReplacementData) SaveLoadManager.Deserialize(typeof(BuildingReplacementData), buildingReplacementJson);
                        Buildings.Add(blockRecordId, buildingData);
                        return true;
                    }
                }
                else
                {
                    buildingData = Buildings[blockRecordId];
                    return true;
                }
            }
            buildingData = new BuildingReplacementData();
            return false;
        }

        #endregion

    }
}
