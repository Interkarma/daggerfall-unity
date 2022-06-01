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

using System.Collections.Generic;
using UnityEngine;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Attached to same GameObject as DaggerfallLocation by scene builders.
    /// This behaviour manages a building dictionary for every building in an exterior location.
    /// Provides helpers to query buildings and link between related systems (automap, quest sites, info text, etc.).
    /// </summary>
    public class BuildingDirectory : MonoBehaviour
    {
        #region Fields

        // Hack fix to provide a non-zero building key when packed building indices result in 0
        // Fixes an oversight where a building key of 0 can be disregarded by multiple systems
        // This value is 1 higher than any other possible building key
        // MakeBuildingKey() will return this value when packed building key is 0
        // ReverseBuildingKey() will unpack indices to 0 when reversing this value
        public const int buildingKey0 = 1 << 24;

        uint locationId;
        int mapId;
        DFLocation locationData;
        Dictionary<int, BuildingSummary> buildingDict = new Dictionary<int, BuildingSummary>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets total number of buildings in this directory (can be 0).
        /// </summary>
        public int BuildingCount
        {
            get { return buildingDict.Count; }
        }

        /// <summary>
        /// Gets LocationID of set location.
        /// </summary>
        public uint LocationID
        {
            get { return locationId; }
        }

        /// <summary>
        /// Gets MapID of set location.
        /// </summary>
        public int MapID
        {
            get { return mapId; }
        }

        /// <summary>
        /// Gets location data of set location.
        /// </summary>
        public DFLocation LocationData
        {
            get { return locationData; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Setup building directory from specified location.
        /// </summary>
        /// <param name="location">Source location data.</param>
        /// <param name="blocks">Exterior blocks.</param>
        public void SetLocation(in DFLocation location, out DFBlock[] blocks)
        {
            // Clear existing buildings
            buildingDict.Clear();

            // Get block data pre-populated with map building data.
            blocks = RMBLayout.GetLocationBuildingData(location);

            // Construct building directory
            int width = location.Exterior.ExteriorData.Width;
            int height = location.Exterior.ExteriorData.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get all buildings for this block
                    // Some blocks have zero buildings
                    int index = y * width + x;
                    BuildingSummary[] buildings = RMBLayout.GetBuildingData(blocks[index], x, y);
                    if (buildings == null || buildings.Length == 0)
                        continue;

                    // Add all buildings to directory
                    for (int i = 0; i < buildings.Length; i++)
                    {
                        buildingDict.Add(buildings[i].buildingKey, buildings[i]);
                    }
                }
            }

            // Store location info
            locationId = location.Exterior.ExteriorData.LocationId;
            mapId = location.MapTableData.MapId;
            locationData = location;
        }

        /// <summary>
        /// Gets building summary.
        /// </summary>
        /// <param name="key">Building key.</param>
        /// <param name="buildingSummaryOut">Building summary out.</param>
        /// <returns>True if lookup successful.</returns>
        public bool GetBuildingSummary(int key, out BuildingSummary buildingSummaryOut)
        {
            if (!buildingDict.ContainsKey(key))
            {
                buildingSummaryOut = new BuildingSummary();
                return false;
            }

            buildingSummaryOut = buildingDict[key];

            return true;
        }

        public List<BuildingSummary> GetBuildingsOfType(DFLocation.BuildingTypes buildingType)
        {
            List<BuildingSummary> buildings = new List<BuildingSummary>(buildingDict.Values);
            buildings.RemoveAll(b => b.BuildingType != buildingType);
            return buildings;
        }

        public List<BuildingSummary> GetBuildingsOfFaction(int factionId)
        {
            List<BuildingSummary> faction = new List<BuildingSummary>();
            foreach (BuildingSummary building in buildingDict.Values)
                if (building.FactionId == factionId)
                    faction.Add(building);
            return faction;
        }

        public List<BuildingSummary> GetHousesForSale()
        {
            int maxForSale = Mathf.Min(buildingDict.Count / 10, 20);
            List<BuildingSummary> forSale = new List<BuildingSummary>();
            List<BuildingSummary> candidates = new List<BuildingSummary>();
            foreach (BuildingSummary building in buildingDict.Values)
            {
                if (building.BuildingType == DFLocation.BuildingTypes.HouseForSale)
                {
                    forSale.Add(building);
                }
                else if (building.BuildingType >= DFLocation.BuildingTypes.House1 &&
                         building.BuildingType <= DFLocation.BuildingTypes.House4 &&
                         !GameManager.Instance.PlayerActivate.IsActiveQuestBuilding(building, true))
                {
                    candidates.Add(building);
                }
            }

            // Add other random houses.
            Random.InitState(buildingDict.GetHashCode() + DaggerfallUnity.Instance.WorldTime.Now.Month);
            for (int c = (maxForSale - forSale.Count); c > 0 && candidates.Count > 0; c--)
            {
                int idx = Random.Range(0, candidates.Count);
                forSale.Add(candidates[idx]);
                candidates.RemoveAt(idx);
            }
            return forSale;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Create a building key unique within a single location only.
        /// </summary>
        /// <param name="layoutX">X position of parent block in map layout.</param>
        /// <param name="layoutY">Y position of parent block in map layout.</param>
        /// <param name="recordIndex">Record index of building inside parent block.</param>
        /// <returns>Building key.</returns>
        public static int MakeBuildingKey(byte layoutX, byte layoutY, byte recordIndex)
        {
            int buildingKey = (layoutX << 16) + (layoutY << 8) + recordIndex;
            return (buildingKey == 0) ? buildingKey0 : buildingKey;
        }

        /// <summary>
        /// Reverse a building key back to block layout coordinates and record index.
        /// </summary>
        /// <param name="key">Building key.</param>
        /// <param name="layoutXOut">X position of parent block in map layout.</param>
        /// <param name="layoutYOut">Y position of parent block in map layout.</param>
        /// <param name="recordIndexOut">Record index of building inside parent block.</param>
        public static void ReverseBuildingKey(int key, out int layoutXOut, out int layoutYOut, out int recordIndexOut)
        {
            if (key == buildingKey0)
            {
                layoutXOut = 0;
                layoutYOut = 0;
                recordIndexOut = 0;
            }
            else
            {
                layoutXOut = key >> 16;
                layoutYOut = (key >> 8) & 0xff;
                recordIndexOut = key & 0xff;
            }
        }

        #endregion
    }
}