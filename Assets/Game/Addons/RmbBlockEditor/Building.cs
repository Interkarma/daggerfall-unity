// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using DaggerfallConnect;
using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility.AssetInjection;
using UnityEditor;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    [SelectionBase]
    [Serializable]
    public class Building : MonoBehaviour
    {

        public ushort NameSeed;
        public ushort FactionId;
        public short Sector;
        public ushort LocationId;
        public DFLocation.BuildingTypes BuildingType;
        public byte Quality;

        public int YRotation;

        public int XPos;
        public int ZPos;
        public int ModelsYPos;

        private int yRotationOld;

        private int xPosOld;
        private int zPosOld;
        private int yPosOld;

        private DFBlock.RmbSubRecord SubRecord;

        public void CreateObject(DFLocation.BuildingData buildingData, DFBlock.RmbSubRecord subRecord)
        {
            SubRecord = subRecord;

            FactionId = buildingData.FactionId;
            NameSeed = buildingData.NameSeed;
            Sector = buildingData.Sector;
            LocationId = buildingData.LocationId;
            BuildingType = buildingData.BuildingType;
            Quality = buildingData.Quality;

            YRotation = subRecord.YRotation;

            XPos = subRecord.XPos;
            ZPos = subRecord.ZPos;
            ModelsYPos = subRecord.Exterior.Block3dObjectRecords[0].YPos;

            SaveOldRotation();
            SaveOldPosition();

            Add3DObjects();
            AddFlatObjects();
            AddPersonObjects();

            UpdateScenePosition();
            UpdateSceneRotation();
        }

        public DFBlock.RmbSubRecord GetSubRecord()
        {
            var record = new DFBlock.RmbSubRecord();
            record.XPos = XPos;
            record.ZPos = ZPos;
            record.YRotation = YRotation;
            record.Exterior = SubRecord.Exterior;
            record.Interior = SubRecord.Interior;

            return record;
        }

        public void SetSubRecord(DFBlock.RmbSubRecord subRecord)
        {
            SubRecord.Exterior = subRecord.Exterior;
            SubRecord.Interior = subRecord.Interior;

            // A new GameObject needs to be added to the scene with the new Exterior
            var newGo = new GameObject(gameObject.name);
            var newBuilding = newGo.AddComponent<Building>();
            newBuilding.CreateObject(GetBuildingData(), GetSubRecord());
            newGo.transform.parent = gameObject.transform.parent;
            try
            {
                Selection.SetActiveObjectWithContext(newGo, null);
                DestroyImmediate(gameObject);
            }
            catch (Exception error)
            {
                Debug.LogError(error);
            }
        }

        public DFLocation.BuildingData GetBuildingData()
        {
            var record = new DFLocation.BuildingData();
            record.NameSeed = NameSeed;
            record.FactionId = FactionId;
            record.Sector = Sector;
            record.LocationId = LocationId;
            record.BuildingType = BuildingType;
            record.Quality = Quality;

            return record;
        }

        public void Start()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            // If rotation has been done on x or z, ignore it and go back to the old rotation
            if (0 != transform.rotation.x || 0 != transform.rotation.z)
            {
                UpdateSceneRotation();
                UpdateScenePosition();
                return;
            }

            // Only update the values when the transformation is complete (when a mouseup is detected)
            var mouseUp = Event.current.type == EventType.MouseUp;
            if (!mouseUp) return;

            // Calculate the new values, based on the scene rotation and position
            var rotation = transform.rotation.eulerAngles;
            YRotation = -(int)(rotation.y * BlocksFile.RotationDivisor);
            XPos = (int)Math.Round(transform.position.x / MeshReader.GlobalScale);
            ZPos = -(int)Math.Round(transform.position.z / MeshReader.GlobalScale);
            ModelsYPos = -(int)Math.Round(transform.position.y / MeshReader.GlobalScale);

            UpdateExteriorModelsYPos();
            UpdateSceneRotation();
            UpdateScenePosition();
        }

        public void Update()
        {
            // Object rotation has been changed in the editor fields
            if (YRotation != yRotationOld)
            {
                SaveOldRotation();
                UpdateSceneRotation();
                return;
            }

            // Object position has been changed in the editor fields
            if (XPos != xPosOld || ZPos != zPosOld || ModelsYPos != yPosOld)
            {
                SaveOldPosition();
                UpdateExteriorModelsYPos();
                UpdateScenePosition();
            }
        }

        public void Export(string fileName)
        {
            BuildingReplacementData data = new BuildingReplacementData()
            {
                FactionId = FactionId,
                BuildingType = (int)BuildingType,
                Quality = Quality,
                NameSeed = NameSeed,
                RmbSubRecord = GetSubRecord()
            };
            RmbBlockHelper.SaveBuildingFile(data, fileName);
        }

        private void Add3DObjects()
        {
            var obj3D = RmbBlockHelper.AddBuilding3dObjects(SubRecord.Exterior.Block3dObjectRecords, ModelsYPos);
            obj3D.transform.parent = transform;
        }

        private void AddFlatObjects()
        {
            var flats = new GameObject("Flats");
            flats.transform.parent = transform;

            foreach (var blockRecord in SubRecord.Exterior.BlockFlatObjectRecords)
            {
                var subRecordRotation =
                    Quaternion.AngleAxis(YRotation / BlocksFile.RotationDivisor, Vector3.up);
                var go = RmbBlockHelper.AddFlatObject(blockRecord, subRecordRotation);
                go.transform.parent = flats.transform;
            }
        }

        private void AddPersonObjects()
        {
            var people = new GameObject("People");
            people.transform.parent = transform;

            foreach (var blockRecord in SubRecord.Exterior.BlockPeopleRecords)
            {
                var go = RmbBlockHelper.AddPersonObject(blockRecord);
                go.transform.parent = people.transform;
            }
        }

        private void SaveOldPosition()
        {
            xPosOld = XPos;
            zPosOld = ZPos;
            yPosOld = ModelsYPos;
        }

        private void SaveOldRotation()
        {
            yRotationOld = YRotation;
        }

        private void UpdateExteriorModelsYPos()
        {
            if (SubRecord.Exterior.Block3dObjectRecords.Length == 0)
            {
                return;
            }

            var main = SubRecord.Exterior.Block3dObjectRecords[0];

            for (var i = 0; i < SubRecord.Exterior.Block3dObjectRecords.Length; i++)
            {
                SubRecord.Exterior.Block3dObjectRecords[i].YPos = ModelsYPos + (SubRecord.Exterior.Block3dObjectRecords[i].YPos - main.YPos);
            }
        }

        private void UpdateScenePosition()
        {
            var vect = new Vector3(XPos, -ModelsYPos, -ZPos) * MeshReader.GlobalScale;
            transform.position = vect;
        }

        private void UpdateSceneRotation()
        {
            var buildingRotation = new Vector3(0, -YRotation / BlocksFile.RotationDivisor, 0);
            transform.rotation = Quaternion.Euler(buildingRotation);
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }
    #endif
}