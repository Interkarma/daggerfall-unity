// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using DaggerfallConnect;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class RmbBlockObject : MonoBehaviour
    {
        public long Position;
        public int Index;
        public string Name;
        public DFBlock.BlockTypes Type;
        public DFBlock.RmbBlockDesc RmbBlock;

        public Automap Automap;

        private Action onDestroy;
        private GameObject buildings;
        private GameObject misc3DObjects;
        private GameObject miscFlatObjects;


        public void CreateObject(DFBlock data, Action onDestroy)
        {
            Position = data.Position;
            Index = data.Index;
            Name = data.Name;
            Type = data.Type;
            RmbBlock = data.RmbBlock;
            this.onDestroy = onDestroy;

            // Load ground
            var ground = new GameObject("Ground");
            var groundComponent = ground.AddComponent<Ground>();
            groundComponent.CreateObject(data.RmbBlock.FldHeader.GroundData.GroundTiles);
            ground.transform.parent = transform;

            // Load ground scenery
            var groundScenery = new GameObject("Ground Scenery");
            var groundSceneryComponent = groundScenery.AddComponent<Scenery>();
            groundSceneryComponent.CreateObject(data.RmbBlock.FldHeader.GroundData.GroundScenery);
            groundScenery.transform.parent = transform;

            // Load Automap
            var automap = new GameObject("Automap");
            Automap = automap.AddComponent<Automap>();
            Automap.CreateObject(data.RmbBlock.FldHeader.AutoMapData);
            automap.transform.parent = transform;
            automap.SetActive(false);

            // Create Game Objects for buildings, models and flats
            buildings = new GameObject("Buildings");
            buildings.transform.parent = transform;

            misc3DObjects = new GameObject("Misc 3D Objects");
            misc3DObjects.transform.parent = transform;

            miscFlatObjects = new GameObject("Misc Flat Objects");
            miscFlatObjects.transform.parent = transform;

            // Load buildings, models and flats in the created Game Objects
            AddObjects(data.RmbBlock);
        }

        public void AddObjects(DFBlock.RmbBlockDesc data)
        {
            // Load all buildings
            if (data.FldHeader.BuildingDataList.Length != 0)
            {
                for (var i = 0; i < data.SubRecords.Length; i++)
                {
                    var building = new GameObject("Building-" + i);
                    var buildingComponent = building.AddComponent<Building>();
                    buildingComponent.CreateObject(data.FldHeader.BuildingDataList[i], data.SubRecords[i]);
                    building.transform.parent = buildings.transform;
                }
            }

            // Load all misc 3D objects
            if (data.Misc3dObjectRecords.Length != 0)
            {
                for (var i = 0; i < data.Misc3dObjectRecords.Length; i++)
                {
                    var miscObjectData = data.Misc3dObjectRecords[i];
                    var misc3DObject = RmbBlockHelper.Add3dObject(miscObjectData);
                    var misc3DObjectComponent = misc3DObject.AddComponent<Misc3d>();
                    misc3DObjectComponent.CreateObject(miscObjectData);
                    misc3DObject.transform.parent = misc3DObjects.transform;
                }
            }

            // Load all misc flat objects
            if (data.MiscFlatObjectRecords.Length != 0)
            {
                for (var i = 0; i < data.MiscFlatObjectRecords.Length; i++)
                {
                    var subRecordRotation = Quaternion.AngleAxis(0, Vector3.up);
                    var miscObjectData = data.MiscFlatObjectRecords[i];
                    var miscFlatObject = RmbBlockHelper.AddFlatObject(miscObjectData, subRecordRotation);
                    var miscFlatObjectComponent = miscFlatObject.AddComponent<MiscFlat>();
                    miscFlatObjectComponent.CreateObject(miscObjectData);
                    miscFlatObject.transform.parent = miscFlatObjects.transform;
                }
            }
        }

        private void OnDestroy()
        {
            onDestroy();
        }
    }
#endif
}