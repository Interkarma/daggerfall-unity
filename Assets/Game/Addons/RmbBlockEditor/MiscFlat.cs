// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using DaggerfallConnect;
using UnityEditor;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    [SelectionBase]
    public class MiscFlat : MonoBehaviour
    {
        public long Position;
        public int TextureArchive;
        public int TextureRecord;
        public short FactionID;
        public byte Flags;

        public Vector3 WorldPosition;
        public Vector3 OldWorldPosition;

        private Billboard dfBillboard;
        private Vector3 flatOffset;

        public void CreateObject(DFBlock.RmbBlockFlatObjectRecord data)
        {
            dfBillboard = GetComponent<Billboard>();
            flatOffset = new Vector3(0, (dfBillboard.Summary.Size.y / 2) - 0.1f, 0);

            Position = data.Position;
            TextureArchive = data.TextureArchive;
            TextureRecord = data.TextureRecord;
            FactionID = data.FactionID;
            Flags = data.Flags;
            WorldPosition = new Vector3(data.XPos, data.YPos, data.ZPos);
            OldWorldPosition = WorldPosition;

            SaveOldPosition();
            UpdateScenePosition();
        }

        public void Start()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        protected void OnSceneGUI(SceneView sceneView)
        {
            var cameraPos = SceneView.lastActiveSceneView.camera.transform.position;
            var targetPos = new Vector3(cameraPos.x, transform.position.y, cameraPos.z);
            transform.LookAt(targetPos);
        }

        public DFBlock.RmbBlockFlatObjectRecord GetRecord()
        {
            var record = new DFBlock.RmbBlockFlatObjectRecord();
            record.Position = Position;
            record.TextureArchive = TextureArchive;
            record.TextureRecord = TextureRecord;
            record.FactionID = FactionID;
            record.Flags = Flags;
            record.XPos = (int)WorldPosition.x;
            record.YPos = (int)WorldPosition.y;
            record.ZPos = (int)WorldPosition.z;

            return record;
        }

        public void ChangeId(string flatId)
        {
            var record = GetRecord();

            var dot = Char.Parse(".");
            var splitId = flatId.Split(dot);

            record.TextureArchive = int.Parse(splitId[0]);
            record.TextureRecord = int.Parse(splitId[1]);

            try
            {
                var newGo = RmbBlockHelper.AddFlatObject(flatId);
                newGo.transform.parent = gameObject.transform.parent;
                newGo.AddComponent<MiscFlat>().CreateObject(record);

                DestroyImmediate(gameObject);
                Selection.SetActiveObjectWithContext(newGo, null);
            }
            catch (Exception error)
            {
                Debug.LogError(error);
            }
        }

        private void Update()
        {
            // Object position has been changed in the editor fields
            if (OldWorldPosition != WorldPosition)
            {
                SaveOldPosition();
                UpdateScenePosition();
                return;
            }

            if (!transform.hasChanged)
            {
                return;
            }

            SaveOldPosition();
            var xPos = (int)Math.Round(transform.position.x / MeshReader.GlobalScale);
            var yPos = -(int)Math.Round((transform.position.y - flatOffset.y) / MeshReader.GlobalScale);
            var zPos = (int)Math.Round(transform.position.z / MeshReader.GlobalScale);

            WorldPosition = new Vector3(xPos, yPos, zPos);

            UpdateScenePosition();
        }

        private void SaveOldPosition()
        {
            OldWorldPosition = WorldPosition;
        }

        private void UpdateScenePosition()
        {
            var vect = new Vector3(WorldPosition.x, -WorldPosition.y, WorldPosition.z) * MeshReader.GlobalScale;
            transform.position = vect + flatOffset;
        }

        protected void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }
    #endif
}