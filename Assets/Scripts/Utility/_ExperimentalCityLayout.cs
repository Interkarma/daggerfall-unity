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
        DaggerfallUnity dfUnity;
        ContentReader contentReader;

        List<DFLocation.LocationDoorElement> cityDoors = new List<DFLocation.LocationDoorElement>();
        List<DFLocation.BuildingData> cityBuildings = new List<DFLocation.BuildingData>();

        /// <summary>
        /// Rough layout code to drop hierarchy only into scene.
        /// </summary>
        public void Layout(string multiName)
        {
            dfUnity = DaggerfallUnity.Instance;
            contentReader = dfUnity.ContentReader;

            // Get location data
            DFLocation location;
            if (!GameObjectHelper.FindMultiNameLocation(multiName, out location))
                throw new Exception("Could not read location " + multiName); ;

            // Create location root object
            GameObject locationGameObject = new GameObject(string.Format("ExperimentalLocation [Region={0}, Name={1}]", location.RegionName, location.Name));
            locationGameObject.transform.position = Vector3.zero;

            // Get city dimensions
            int width = location.Exterior.ExteriorData.Width;
            int height = location.Exterior.ExteriorData.Height;

            // Import buildings
            int totalMapBuildings = AddBuildings(ref location, locationGameObject);

            // Import blocks
            int totalBlockBuildings = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    totalBlockBuildings += AddBlock(ref location, x, y, locationGameObject);
                }
            }

            Debug.LogFormat("Total map buildings: {0}, Total block buildings: {1}", totalMapBuildings, totalBlockBuildings);
        }

        /// <summary>
        /// One problem I'm still not satisfied with is how to correctly link buildings in map data to buildings in block data.
        /// </summary>
        int AddBuildings(ref DFLocation location, GameObject parent)
        {
            int totalMapBuildings = 0;

            // Location buildings
            int buildingCount = location.Exterior.BuildingCount;
            GameObject[] buildingGameObjectArray = new GameObject[buildingCount];
            for (int i = 0; i < buildingCount; i++)
            {
                DFLocation.BuildingData building = location.Exterior.Buildings[i];
                cityBuildings.Add(building);

                string buildingName = BuildingNames.GetName(building.NameSeed, building.BuildingType, building.FactionId, location.Name, location.RegionName);

                // Would love to know what this "Sector" value actually is
                buildingGameObjectArray[i] = new GameObject(string.Format("Building: {0} [{1}]", building.BuildingType, building.Sector));
                buildingGameObjectArray[i].transform.parent = parent.transform;
                totalMapBuildings++;
            }

            // UESP guesses these records are doors. They do map to a building index but do not distribute as doors would.
            // A building might have 0-N of these items regardless of how many doors that building actually has.
            // What is this data, really?
            // Observation: the Unknown1 value is often (but not always) +2-3 greater than Sector value on building itself.
            // Observation: templates often have a larger number of these records attached than any other building type.
            int doorCount = (int)location.Exterior.RecordElement.DoorCount;
            for (int i = 0; i < doorCount; i++)
            {
                DFLocation.LocationDoorElement door = location.Exterior.RecordElement.Doors[i];
                if (door.BuildingDataIndex == 0xffff)
                    continue;

                cityDoors.Add(door);
                GameObject doorGameObject = new GameObject(string.Format("Door: Mask={0}, Unknown1={1}, Unknown2={2}", door.Mask, door.Unknown1, door.Unknown2));
                doorGameObject.transform.parent = buildingGameObjectArray[door.BuildingDataIndex].transform;
            }

            return totalMapBuildings;
        }

        
        int AddBlock(ref DFLocation location, int x, int y, GameObject parent)
        {
            int totalBlockBuildings = 0;

            // Get block name
            string blockName = contentReader.BlockFileReader.CheckName(contentReader.MapFileReader.GetRmbBlockName(ref location, x, y));

            // Get block data
            DFBlock block;
            if (!contentReader.GetBlock(blockName, out block))
                throw new Exception("Could not read block " + blockName);

            // Add block
            GameObject blockGameObject = new GameObject(blockName);
            blockGameObject.transform.parent = parent.transform;
            blockGameObject.transform.position = new Vector3((x * RMBLayout.RMBSide), 0, (y * RMBLayout.RMBSide));

            int buildingCount = block.RmbBlock.SubRecords.Length;
            for (int i = 0; i < 32; i++)
            {
                DFLocation.BuildingData building = block.RmbBlock.FldHeader.BuildingDataList[i];

                string buildingName;
                if (i < buildingCount)
                {
                    buildingName = string.Format("[{0}] Building: {1}", i, building.BuildingType);
                    totalBlockBuildings++;
                }
                else
                {
                    buildingName = string.Format("[{0}] Unused", i);
                }
                //DFBlock.RmbBlockHeader header = block.RmbBlock.SubRecords[i].Exterior.Header;
                GameObject buildingGameObject = new GameObject(buildingName);
                buildingGameObject.transform.parent = blockGameObject.transform;
            }

            return totalBlockBuildings;
        }
    }
}