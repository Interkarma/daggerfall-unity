﻿// Project:         Daggerfall Tools For Unity
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
    public class Misc3d : MonoBehaviour
    {
        public DFBlock.RmbBlock3dObjectRecord data;
        public uint ModelId;
        public byte ObjectType;

        public Vector3 pos;
        private Vector3 oldPos;
        public Vector3 rotation;
        public Vector3 oldRotation;

        public float scale;
        private float scaleOld;

        public void CreateObject(DFBlock.RmbBlock3dObjectRecord data)
        {
            this.data = data;
            ModelId = data.ModelIdNum;
            ObjectType = data.ObjectType;
            pos = new Vector3(data.XPos, data.YPos, data.ZPos);
            rotation = new Vector3(data.XRotation, data.YRotation, data.ZRotation);
            scale = data.YScale;

            SaveOldRotation();
            SaveOldPosition();
            UpdateScenePosition();
            UpdateSceneRotation();
        }

        public void Start()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public DFBlock.RmbBlock3dObjectRecord GetRecord()
        {
            var record = new DFBlock.RmbBlock3dObjectRecord();
            record.ModelId = ModelId.ToString();
            record.ModelIdNum = ModelId;
            record.ObjectType = ObjectType;
            record.XPos = (int)pos.x;
            record.YPos = (int)pos.y;
            record.ZPos = (int)pos.z;
            record.XRotation = (short)rotation.x;
            record.YRotation = (short)rotation.y;
            record.ZRotation = (short)rotation.z;
            record.XScale = scale;
            record.YScale = scale;
            record.ZScale = scale;

            return record;
        }

        private void Save()
        {
            data.XPos = (int)pos.x;
            data.YPos = (int)pos.y;
            data.ZPos = (int)pos.z;
            data.XRotation = (short)rotation.x;
            data.YRotation = (short)rotation.y;
            data.ZRotation = (short)rotation.z;
            data.XScale = scale;
            data.YScale = scale;
            data.ZScale = scale;
        }

        private void Update()
        {
            UpdateTransform();
            Save();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            // Only update the values when the transformation is complete (when a mouseup is detected)
            var mouseUp = Event.current.type == EventType.MouseUp;
            if (!mouseUp) return;

            SaveOldRotation();
            var currentRotation = transform.rotation.eulerAngles;
            var rotationX = (short)(-Math.Round(currentRotation.x * BlocksFile.RotationDivisor));
            var rotationY = (short)(-Math.Round(currentRotation.y * BlocksFile.RotationDivisor));
            var rotationZ = (short)(-Math.Round(currentRotation.z * BlocksFile.RotationDivisor));
            rotation = new Vector3(rotationX, rotationY, rotationZ);

            SaveOldScale();
            scale = transform.localScale.x;
            scale = transform.localScale.y;
            scale = transform.localScale.z;

            SaveOldPosition();
            var xPos = (int)Math.Round(transform.position.x / MeshReader.GlobalScale);
            var yPos = -(int)Math.Round(transform.position.y / MeshReader.GlobalScale);
            var zPos = (int)Math.Round(transform.position.z / MeshReader.GlobalScale);
            pos = new Vector3(xPos, yPos, zPos);


            UpdateScenePosition();
            UpdateSceneRotation();
            UpdateSceneScale();
        }

        private void UpdateTransform()
        {
            // Object rotation has been changed in the editor fields
            if (rotation != oldRotation)
            {
                SaveOldRotation();
                UpdateSceneRotation();
                return;
            }

            // Object position has been changed in the editor fields
            if (pos != oldPos)
            {
                SaveOldPosition();
                UpdateScenePosition();
                return;
            }

            // Object scale has been changed in the editor fields
            if ((int)(scale * 10000) != (int)(scaleOld * 10000))
            {
                SaveOldScale();
                UpdateSceneScale();
            }
        }

        private void SaveOldPosition()
        {
            oldPos = pos;
        }

        private void SaveOldRotation()
        {
            oldRotation = rotation;
        }

        private void SaveOldScale()
        {
            scaleOld = scale;
        }

        private void UpdateScenePosition()
        {
            transform.position = new Vector3(pos.x, -pos.y, pos.z) * MeshReader.GlobalScale;
        }

        private void UpdateSceneRotation()
        {
            var rotationX = (float)(-Math.Round(rotation.x / BlocksFile.RotationDivisor, 4));
            var rotationY = (float)(-Math.Round(rotation.y / BlocksFile.RotationDivisor, 4));
            var rotationZ = (float)(-Math.Round(rotation.z / BlocksFile.RotationDivisor, 4));
            var modelRotation = new Vector3(rotationX, rotationY, rotationZ);
            transform.rotation = Quaternion.Euler(modelRotation);
        }

        private void UpdateSceneScale()
        {
            transform.localScale = new Vector3(scale, scale, scale);
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }
}