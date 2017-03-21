// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Revisiting exterior city layouts to better integrate building names, doors, quest information, etc.
    /// Doing this purely in an experimental class so nothing is broken in city builders.
    /// This class will be deleted once no longer required.
    /// </summary>
    public class _ExperimentalCityLayout
    {
        public void Layout(string multiName)
        {
            // Get location data
            DFLocation location;
            if (!GameObjectHelper.FindMultiNameLocation(multiName, out location))
                throw new Exception("Could not read location " + multiName); ;

            // Get all RMB blocks with building data populated from location
            // Blocks array output follows same block order as location layout
            // This is slightly expensive, only call as needed and cache resulting array
            DFBlock[] blocks;
            RMBLayout.GetLocationBuildingData(location, out blocks);

            // TEST FOLLOWS: Do stuff with this data

            // Create location root object
            GameObject locationGameObject = new GameObject(string.Format("ExperimentalLocation [Region={0}, Name={1}]", location.RegionName, location.Name));

            // Iterate all blocks
            int width = location.Exterior.ExteriorData.Width;
            int height = location.Exterior.ExteriorData.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get building summary for this block
                    // Must send it a DFBlock from GetLocationBuildingData()
                    // Otherwise the building data will not be populated
                    int index = y * width + x;
                    RMBLayout.BuildingSummary[] buildingSummary = RMBLayout.GetBuildingData(blocks[index]);

                    // Add a block parent object
                    GameObject blockGameObject = new GameObject(blocks[index].Name);
                    blockGameObject.transform.parent = locationGameObject.transform;

                    // Add all buildings to block parent
                    for (int i = 0; i < buildingSummary.Length; i++)
                    {
                        // Resolve building name from seed and related information
                        string buildingName = BuildingNames.GetName(
                            buildingSummary[i].NameSeed,
                            buildingSummary[i].BuildingType,
                            buildingSummary[i].FactionId,
                            location.Name,
                            location.RegionName);

                        // Create a descriptive name string for gameobject
                        // Add nameplate for named buildings
                        string goName = string.Format("Building: {0}", buildingSummary[i].BuildingType);
                        if (RMBLayout.IsNamedBuildng(buildingSummary[i].BuildingType))
                            goName += string.Format(" [{0}]", buildingName);

                        // Add building object
                        GameObject buildingGameObject = new GameObject(blocks[index].Name);
                        buildingGameObject.name = goName;
                        buildingGameObject.transform.parent = blockGameObject.transform;
                    }
                }
            }
        }

        #region Retired

        //DaggerfallUnity dfUnity;
        //ContentReader contentReader;

        //List<DFLocation.LocationDoorElement> cityDoors = new List<DFLocation.LocationDoorElement>();
        //List<DFLocation.BuildingData> cityBuildings = new List<DFLocation.BuildingData>();
        //List<string> cityBuildingNames = new List<string>();
        //List<string> blockBuildingNames = new List<string>();

        //List<BuildingPoolItem> specialBuildingPool = new List<BuildingPoolItem>();

        //struct BuildingPoolItem
        //{
        //    public DFLocation.BuildingData buildingData;
        //    public bool used;
        //}

        ///// <summary>
        ///// Rough layout code to drop hierarchy only into scene.
        ///// </summary>
        //public void Layout(string multiName)
        //{
        //    dfUnity = DaggerfallUnity.Instance;
        //    contentReader = dfUnity.ContentReader;

        //    // Get location data
        //    DFLocation location;
        //    if (!GameObjectHelper.FindMultiNameLocation(multiName, out location))
        //        throw new Exception("Could not read location " + multiName); ;

        //    // Create location root object
        //    GameObject locationGameObject = new GameObject(string.Format("ExperimentalLocation [Region={0}, Name={1}]", location.RegionName, location.Name));
        //    locationGameObject.transform.position = Vector3.zero;

        //    // Create building list object
        //    GameObject buildingListGameObject = new GameObject("BuildingList");
        //    buildingListGameObject.transform.position = Vector3.zero;
        //    buildingListGameObject.transform.parent = locationGameObject.transform;

        //    // Get city dimensions
        //    int width = location.Exterior.ExteriorData.Width;
        //    int height = location.Exterior.ExteriorData.Height;

        //    // Import buildings
        //    int totalMapDoors;
        //    int totalMapBuildings = AddBuildings(ref location, buildingListGameObject, out totalMapDoors);

        //    // Import blocks
        //    int totalBlockBuildings = 0;
        //    for (int y = 0; y < height; y++)
        //    {
        //        for (int x = 0; x < width; x++)
        //        {
        //            totalBlockBuildings += AddBlock(ref location, x, y, locationGameObject);
        //        }
        //    }

        //    // Name arrays should be same size
        //    if (cityBuildingNames.Count != blockBuildingNames.Count)
        //        throw new Exception("Name array size mismatch");

        //    Debug.LogFormat("Total map buildings: {0}, Total block buildings: {1}, Total \"doors\": {2}", totalMapBuildings, totalBlockBuildings, totalMapDoors);

        //    // Find where linking failed name should be equal on both sides
        //    for (int i = 0; i < cityBuildingNames.Count; i++)
        //    {
        //        if (cityBuildingNames[i] != blockBuildingNames[i])
        //        {
        //            Debug.LogErrorFormat("Name mismatch from index {0}", i);
        //            break;
        //        }
        //    }
        //}

        ///// <summary>
        ///// One problem I'm still not satisfied with is how to correctly link buildings in map data to buildings in block data.
        ///// </summary>
        //int AddBuildings(ref DFLocation location, GameObject parent, out int doorCountOut)
        //{
        //    int totalMapBuildings = 0;

        //    // Location buildings
        //    int buildingCount = location.Exterior.BuildingCount;
        //    GameObject[] buildingGameObjectArray = new GameObject[buildingCount];
        //    for (int i = 0; i < buildingCount; i++)
        //    {
        //        DFLocation.BuildingData building = location.Exterior.Buildings[i];
        //        cityBuildings.Add(building);

        //        string buildingName = BuildingNames.GetName(building.NameSeed, building.BuildingType, building.FactionId, location.Name, location.RegionName);
        //        if (IsSpecial(building.BuildingType))
        //        {
        //            cityBuildingNames.Add(buildingName);

        //            BuildingPoolItem bp = new BuildingPoolItem();
        //            bp.buildingData = building;
        //            bp.used = false;
        //            specialBuildingPool.Add(bp);
        //        }

        //        // Would love to know what this "Sector" value actually is
        //        buildingGameObjectArray[i] = new GameObject(string.Format("Building: {0} [{1}]", building.BuildingType, buildingName));
        //        buildingGameObjectArray[i].transform.parent = parent.transform;
        //        totalMapBuildings++;
        //    }

        //    // UESP guesses these records are doors. They do map to a building index but do not distribute as doors would.
        //    // A building might have 0-N of these items regardless of how many doors that building actually has.
        //    // What is this data, really?
        //    // Observations: One of the Unknown1 values is often (but not always) +2-3 greater than Sector value on building itself.
        //    // Observation: temples often have a larger number of these records attached than any other building type.
        //    int doorCount = (int)location.Exterior.RecordElement.DoorCount;
        //    for (int i = 0; i < doorCount; i++)
        //    {
        //        DFLocation.LocationDoorElement door = location.Exterior.RecordElement.Doors[i];
        //        if (door.BuildingDataIndex == 0xffff)
        //            continue;

        //        cityDoors.Add(door);
        //        GameObject doorGameObject = new GameObject(string.Format("Door: Mask={0}, Unknown1={1}, Unknown2={2}", door.Mask, door.Unknown1, door.Unknown2));
        //        doorGameObject.transform.parent = buildingGameObjectArray[door.BuildingDataIndex].transform;
        //    }

        //    doorCountOut = doorCount;

        //    return totalMapBuildings;
        //}


        //int AddBlock(ref DFLocation location, int x, int y, GameObject parent)
        //{
        //    int totalBlockBuildings = 0;

        //    // Get block name
        //    string blockName = contentReader.BlockFileReader.CheckName(contentReader.MapFileReader.GetRmbBlockName(ref location, x, y));

        //    // Get block data
        //    DFBlock block;
        //    if (!contentReader.GetBlock(blockName, out block))
        //        throw new Exception("Could not read block " + blockName);

        //    // Add block
        //    GameObject blockGameObject = new GameObject(blockName);
        //    blockGameObject.transform.parent = parent.transform;
        //    blockGameObject.transform.position = new Vector3((x * RMBLayout.RMBSide), 0, (y * RMBLayout.RMBSide));

        //    int buildingCount = block.RmbBlock.SubRecords.Length;
        //    for (int i = 0; i < 32; i++)
        //    {
        //        DFLocation.BuildingData building = block.RmbBlock.FldHeader.BuildingDataList[i];

        //        string name;
        //        if (i < buildingCount)
        //        {
        //            name = string.Format("[{0}] Building: {1}", i, building.BuildingType);
        //            totalBlockBuildings++;

        //            // Link special buildings
        //            if (IsSpecial(building.BuildingType))
        //            {
        //                // Try to find next building
        //                BuildingPoolItem item;
        //                bool found = GetNextBuildingFromPool(building.BuildingType, out item); //FindNextBuildingType(building.BuildingType, buildingIndex);
        //                if (!found)
        //                    throw new Exception(string.Format("End of city building list reached without finding building type {0}", building.BuildingType));

        //                // Copy city building data to block level
        //                building.NameSeed = item.buildingData.NameSeed;
        //                building.FactionId = item.buildingData.FactionId;
        //                building.LocationId = item.buildingData.LocationId;
        //                building.Quality = item.buildingData.Quality;
        //                building.Sector = item.buildingData.Sector;

        //                // Resolve name for block building
        //                string buildingName = BuildingNames.GetName(building.NameSeed, building.BuildingType, building.FactionId, location.Name, location.RegionName);
        //                blockBuildingNames.Add(buildingName);
        //                name += string.Format(" [{0}]", buildingName);
        //            }
        //        }
        //        else
        //        {
        //            continue;
        //            //name = string.Format("[{0}] Unused", i);
        //        }
        //        //DFBlock.RmbBlockHeader header = block.RmbBlock.SubRecords[i].Exterior.Header;
        //        GameObject buildingGameObject = new GameObject(name);
        //        buildingGameObject.transform.parent = blockGameObject.transform;
        //    }

        //    return totalBlockBuildings;
        //}

        ///// <summary>
        ///// Draws and consume building from pool.
        ///// Checking if buildings are ordered by special type.
        ///// </summary>
        //bool GetNextBuildingFromPool(DFLocation.BuildingTypes buildingType, out BuildingPoolItem itemOut)
        //{
        //    itemOut = new BuildingPoolItem();
        //    for (int i = 0; i < specialBuildingPool.Count; i++)
        //    {
        //        if (!specialBuildingPool[i].used && specialBuildingPool[i].buildingData.BuildingType == buildingType)
        //        {
        //            BuildingPoolItem item = specialBuildingPool[i];
        //            item.used = true;
        //            specialBuildingPool[i] = item;
        //            itemOut = item;
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        /////// <summary>
        /////// Finds next building of specified type.
        /////// </summary>
        ////int FindNextBuildingType(DFLocation.BuildingTypes buildingType, int startIndex)
        ////{
        ////    for (int i = startIndex; i < cityBuildings.Count; i++)
        ////    {
        ////        if (cityBuildings[i].BuildingType == buildingType)
        ////            return i;
        ////    }

        ////    return -1;
        ////}

        ///// <summary>
        ///// Checks if building type is a special (i.e. not a home).
        ///// </summary>
        //bool IsSpecial(DFLocation.BuildingTypes buildingType)
        //{
        //    switch (buildingType)
        //    {
        //        case DFLocation.BuildingTypes.Alchemist:
        //        //case DFLocation.BuildingTypes.HouseForSale:
        //        case DFLocation.BuildingTypes.Armorer:
        //        case DFLocation.BuildingTypes.Bank:
        //        case DFLocation.BuildingTypes.Bookseller:
        //        case DFLocation.BuildingTypes.ClothingStore:
        //        case DFLocation.BuildingTypes.FurnitureStore:
        //        case DFLocation.BuildingTypes.GemStore:
        //        case DFLocation.BuildingTypes.GeneralStore:
        //        case DFLocation.BuildingTypes.Library:
        //        case DFLocation.BuildingTypes.GuildHall:
        //        case DFLocation.BuildingTypes.PawnShop:
        //        case DFLocation.BuildingTypes.WeaponSmith:
        //        case DFLocation.BuildingTypes.Temple:
        //        case DFLocation.BuildingTypes.Tavern:
        //        case DFLocation.BuildingTypes.Palace:
        //            return true;
        //        default:
        //            return false;
        //    }
        //}

        #endregion
    }
}