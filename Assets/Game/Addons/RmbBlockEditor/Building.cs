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
using UnityEditor;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    [ExecuteInEditMode]
    [SelectionBase]
    [Serializable]
    public class Building : MonoBehaviour
    {
        public DFLocation.BuildingData BuildingDataObj;
        [SerializeField] private DFBlock.RmbBlockHeader InteriorHeader;
        [SerializeField] private DFBlock.RmbBlock3dObjectRecord[] InteriorBlock3dObjectRecords;
        [SerializeField] private DFBlock.RmbBlockFlatObjectRecord[] InteriorBlockFlatObjectRecords;
        [SerializeField] private DFBlock.RmbBlockSection3Record[] InteriorBlockSection3Records;
        [SerializeField] private DFBlock.RmbBlockPeopleRecord[] InteriorBlockPeopleRecords;
        [SerializeField] private DFBlock.RmbBlockDoorRecord[] InteriorBlockDoorRecords;
        [SerializeField] private DFBlock.RmbBlockHeader ExteriorHeader;
        [SerializeField] private DFBlock.RmbBlock3dObjectRecord[] ExteriorBlock3dObjectRecords;
        [SerializeField] private DFBlock.RmbBlockFlatObjectRecord[] ExteriorBlockFlatObjectRecords;
        [SerializeField] private DFBlock.RmbBlockSection3Record[] ExteriorBlockSection3Records;
        [SerializeField] private DFBlock.RmbBlockPeopleRecord[] ExteriorBlockPeopleRecords;
        [SerializeField] private DFBlock.RmbBlockDoorRecord[] ExteriorBlockDoorRecords;


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

        private ClimateBases climate;
        private ClimateSeason season;
        private WindowStyle windowStyle;

        public void CreateObject(DFLocation.BuildingData buildingData, DFBlock.RmbSubRecord subRecord,
            ClimateBases climate, ClimateSeason season, WindowStyle windowStyle)
        {
            BuildingDataObj = buildingData;

            InteriorHeader = subRecord.Interior.Header;
            InteriorBlock3dObjectRecords = subRecord.Interior.Block3dObjectRecords;
            InteriorBlockFlatObjectRecords = subRecord.Interior.BlockFlatObjectRecords;
            InteriorBlockSection3Records = subRecord.Interior.BlockSection3Records;
            InteriorBlockPeopleRecords = subRecord.Interior.BlockPeopleRecords;
            InteriorBlockDoorRecords = subRecord.Interior.BlockDoorRecords;

            ExteriorHeader = subRecord.Exterior.Header;
            ExteriorBlock3dObjectRecords = subRecord.Exterior.Block3dObjectRecords;
            ExteriorBlockFlatObjectRecords = subRecord.Exterior.BlockFlatObjectRecords;
            ExteriorBlockSection3Records = subRecord.Exterior.BlockSection3Records;
            ExteriorBlockPeopleRecords = subRecord.Exterior.BlockPeopleRecords;
            ExteriorBlockDoorRecords = subRecord.Exterior.BlockDoorRecords;

            FactionId = BuildingDataObj.FactionId;
            NameSeed = BuildingDataObj.NameSeed;
            Sector = BuildingDataObj.Sector;
            LocationId = BuildingDataObj.LocationId;
            BuildingType = BuildingDataObj.BuildingType;
            Quality = BuildingDataObj.Quality;

            YRotation = subRecord.YRotation;

            XPos = subRecord.XPos;
            ZPos = subRecord.ZPos;
            ModelsYPos = ExteriorBlock3dObjectRecords[0].YPos;

            this.climate = climate;
            this.season = season;
            this.windowStyle = windowStyle;

            SaveOldRotation();
            SaveOldPosition();

            Add3DObjects();
            AddFlatObjects();
            AddPersonObjects();

            UpdateScenePosition();
            UpdateSceneRotation();
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

        public DFBlock.RmbSubRecord GetSubRecord()
        {
            var record = new DFBlock.RmbSubRecord();
            record.XPos = XPos;
            record.ZPos = ZPos;
            record.YRotation = YRotation;
            record.Exterior = new DFBlock.RmbBlockData();
            record.Exterior.Header = ExteriorHeader;
            record.Exterior.Block3dObjectRecords = ExteriorBlock3dObjectRecords;
            record.Exterior.BlockFlatObjectRecords = ExteriorBlockFlatObjectRecords;
            record.Exterior.BlockSection3Records = ExteriorBlockSection3Records;
            record.Exterior.BlockPeopleRecords = ExteriorBlockPeopleRecords;
            record.Exterior.BlockDoorRecords = ExteriorBlockDoorRecords;

            record.Interior = new DFBlock.RmbBlockData();
            record.Interior.Header = InteriorHeader;
            record.Interior.Block3dObjectRecords = InteriorBlock3dObjectRecords;
            record.Interior.BlockFlatObjectRecords = InteriorBlockFlatObjectRecords;
            record.Interior.BlockSection3Records = InteriorBlockSection3Records;
            record.Interior.BlockPeopleRecords = InteriorBlockPeopleRecords;
            record.Interior.BlockDoorRecords = InteriorBlockDoorRecords;

            return record;
        }

        private void Add3DObjects()
        {
            var obj3D = RmbBlockHelper.AddBuilding3dObjects(ExteriorBlock3dObjectRecords, ModelsYPos, climate, season,
                windowStyle);
            obj3D.transform.parent = transform;
        }

        private void AddFlatObjects()
        {
            var flats = new GameObject("Flats");
            flats.transform.parent = transform;

            foreach (var blockRecord in ExteriorBlockFlatObjectRecords)
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

            foreach (var blockRecord in ExteriorBlockPeopleRecords)
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
            if (ExteriorBlock3dObjectRecords.Length == 0)
            {
                return;
            }

            var main = ExteriorBlock3dObjectRecords[0];

            for (var i = 0; i < ExteriorBlock3dObjectRecords.Length; i++)
            {
                ExteriorBlock3dObjectRecords[i].YPos = ModelsYPos + (ExteriorBlock3dObjectRecords[i].YPos - main.YPos);
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
}