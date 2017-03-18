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
            AddBuildings(ref location, locationGameObject);

            // Import blocks
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //AddBlock(ref location, x, y, locationGameObject);
                }
            }
        }

        /// <summary>
        /// One problem I'm still not satisfied with is how to correctly link buildings in map data to buildings in block data.
        /// </summary>
        void AddBuildings(ref DFLocation location, GameObject parent)
        {
            // Location buildings
            int buildingCount = location.Exterior.BuildingCount;
            GameObject[] buildingGameObjectArray = new GameObject[buildingCount];
            for (int i = 0; i < buildingCount; i++)
            {
                DFLocation.BuildingData building = location.Exterior.Buildings[i];
                cityBuildings.Add(building);

                string buildingName = BuildingNames.GetName(building.NameSeed, building.BuildingType, building.FactionId, location.Name, location.RegionName);

                buildingGameObjectArray[i] = new GameObject(string.Format("Building: {0} [{1}]", building.BuildingType, buildingName));
                buildingGameObjectArray[i].transform.parent = parent.transform;
            }

            //// Location doors
            //int doorCount = (int)location.Exterior.RecordElement.DoorCount;
            //for (int i = 0; i < doorCount; i++)
            //{
            //    DFLocation.LocationDoorElement door = location.Exterior.RecordElement.Doors[i];
            //    if (door.BuildingDataIndex == 0xffff)
            //        continue;

            //    cityDoors.Add(door);

            //    GameObject doorGameObject = new GameObject(string.Format("Door: {0}", door.Unknown1));
            //    doorGameObject.transform.parent = buildingGameObjectArray[door.BuildingDataIndex].transform;
            //}
        }

        
        void AddBlock(ref DFLocation location, int x, int y, GameObject parent)
        {
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
            for (int i = 0; i < buildingCount; i++)
            {
                //DFBlock.RmbBlockHeader header = block.RmbBlock.SubRecords[i].Exterior.Header;
                DFLocation.BuildingData building = block.RmbBlock.FldHeader.BuildingDataList[i];
                GameObject buildingGameObject = new GameObject(string.Format("Building: {0}", building.BuildingType));
                buildingGameObject.transform.parent = blockGameObject.transform;
            }
        }
    }
}