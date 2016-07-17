// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Nystul   
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    public class DaggerfallInterior : MonoBehaviour
    {
        const int doorModelId = 9800;

        DaggerfallUnity dfUnity;
        DFBlock blockData;
        DFBlock.RmbSubRecord recordData;
        ModelCombiner combiner = new ModelCombiner();
        ClimateBases climateBase = ClimateBases.Temperate;
        ClimateSeason climateSeason = ClimateSeason.Summer;
        List<GameObject> markers = new List<GameObject>();
        StaticDoor entryDoor;
        Transform doorOwner;

        /// <summary>
        /// Gets transform owning door array.
        /// </summary>
        public Transform DoorOwner
        {
            get { return doorOwner; }
        }

        public DFLocation.BuildingData BuildingData
        {
            get { return blockData.RmbBlock.FldHeader.BuildingDataList[entryDoor.recordIndex]; }
        }

        /// <summary>
        /// Gets door array from owner.
        /// </summary>
        public DaggerfallStaticDoors ExteriorDoors
        {
            get { return (doorOwner) ? doorOwner.GetComponent<DaggerfallStaticDoors>() : null; }
        }

        /// <summary>
        /// Gets the door player clicked on to enter building.
        /// </summary>
        public StaticDoor EntryDoor
        {
            get { return entryDoor; }
        }

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
        }

        /// <summary>
        /// Layout interior based on data in exterior door and optional location for climate settings.
        /// </summary>
        /// <param name="doorOwner">Parent transform owning door array.</param>
        /// <param name="door">Exterior door player clicked on.</param>
        /// <returns>True if successful.</returns>
        public bool DoLayout(Transform doorOwner, StaticDoor door, ClimateBases climateBase)
        {
            if (dfUnity == null)
                dfUnity = DaggerfallUnity.Instance;

            // Use specified climate
            this.climateBase = climateBase;

            // Save exterior information
            this.entryDoor = door;
            this.doorOwner = doorOwner;

            // Get block data
            blockData = dfUnity.ContentReader.BlockFileReader.GetBlock(door.blockIndex);
            if (blockData.Type != DFBlock.BlockTypes.Rmb)
                throw new Exception(string.Format("Could not load RMB block index {0}", door.blockIndex), null);

            // Get record data
            recordData = blockData.RmbBlock.SubRecords[door.recordIndex];
            if (recordData.Interior.Header.Num3dObjectRecords == 0)
                throw new Exception(string.Format("No interior 3D models found for record index {0}", door.recordIndex), null);

            // Layout interior data
            AddModels();
            AddFlats();
            AddPeople();
            AddActionDoors();

            return true;
        }

        /// <summary>
        /// Layout interior for automap based on data in exterior door and optional location for climate settings.
        /// </summary>
        /// <param name="doorOwner">Parent transform owning door array.</param>
        /// <param name="door">Exterior door player clicked on.</param>
        /// <returns>True if successful.</returns>
        public bool DoLayoutAutomap(Transform doorOwner, StaticDoor door, ClimateBases climateBase)
        {
            if (dfUnity == null)
                dfUnity = DaggerfallUnity.Instance;

            // Use specified climate
            this.climateBase = climateBase;

            // Save exterior information
            this.entryDoor = door;
            this.doorOwner = doorOwner;

            // Get block data
            blockData = dfUnity.ContentReader.BlockFileReader.GetBlock(door.blockIndex);
            if (blockData.Type != DFBlock.BlockTypes.Rmb)
                throw new Exception(string.Format("Could not load RMB block index {0}", door.blockIndex), null);

            // Get record data
            recordData = blockData.RmbBlock.SubRecords[door.recordIndex];
            if (recordData.Interior.Header.Num3dObjectRecords == 0)
                throw new Exception(string.Format("No interior 3D models found for record index {0}", door.recordIndex), null);

            // Layout interior data
            AddModels();
            //AddFlats();
            //AddPeople();
            //AddActionDoors();

            return true;
        }

        /// <summary>
        /// Finds closest entrance marker to door position.
        /// </summary>
        /// <param name="playerPos">Player position in world space.</param>
        /// <param name="closestMarkerOut">Closest enter marker to door position.</param>
        /// <returns>True if successful.</returns>
        public bool FindClosestEnterMarker(Vector3 playerPos, out Vector3 closestMarkerOut)
        {
            if (markers.Count == 0)
            {
                closestMarkerOut = Vector3.zero;
                return false;
            }

            float minDistance = float.MaxValue;
            closestMarkerOut = markers[0].transform.position;
            for (int i = 0; i < markers.Count; i++)
            {
                float distance = Vector3.Distance(playerPos, markers[i].transform.position);
                if (distance < minDistance)
                {
                    closestMarkerOut = markers[i].transform.position;
                    minDistance = distance;
                }
            }

            return true;
        }

        #region Private Methods

        /// <summary>
        /// Add interior models.
        /// </summary>
        private void AddModels()
        {
            List<StaticDoor> doors = new List<StaticDoor>();
            GameObject node = new GameObject("Models");
            GameObject doorsNode = new GameObject("Doors");
            node.transform.parent = this.transform;
            doorsNode.transform.parent = this.transform;

            // Iterate through models in this subrecord
            combiner.NewCombiner();
            foreach (DFBlock.RmbBlock3dObjectRecord obj in recordData.Interior.Block3dObjectRecords)
            {
                // Get model data
                ModelData modelData;
                dfUnity.MeshReader.GetModelData(obj.ModelIdNum, out modelData);

                // Get model position by type (3 seems to indicate props/clutter)
                Vector3 modelPosition;
                if (obj.ObjectType == 3)
                {
                    // Props axis needs to be transformed to lowest Y point
                    Vector3 bottom = modelData.Vertices[0];
                    for (int i = 0; i < modelData.Vertices.Length; i++)
                    {
                        if (modelData.Vertices[i].y < bottom.y)
                            bottom = modelData.Vertices[i];
                    }
                    modelPosition = new Vector3(obj.XPos, obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
                    modelPosition += new Vector3(0, -bottom.y, 0);
                }
                else
                {
                    modelPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
                }

                // Get model transform
                Vector3 modelRotation = new Vector3(0, -obj.YRotation / BlocksFile.RotationDivisor, 0);
                Matrix4x4 modelMatrix = Matrix4x4.TRS(modelPosition, Quaternion.Euler(modelRotation), Vector3.one);

                // Does this model have doors?
                if (modelData.Doors != null)
                    doors.AddRange(GameObjectHelper.GetStaticDoors(ref modelData, entryDoor.blockIndex, entryDoor.recordIndex, modelMatrix));

                // Combine or add
                if (dfUnity.Option_CombineRMB)
                {
                    combiner.Add(ref modelData, modelMatrix);
                }
                else
                {
                    // Add GameObject
                    GameObject go = GameObjectHelper.CreateDaggerfallMeshGameObject(obj.ModelIdNum, node.transform, dfUnity.Option_SetStaticFlags);
                    go.transform.position = modelMatrix.GetColumn(3);
                    go.transform.rotation = GameObjectHelper.QuaternionFromMatrix(modelMatrix);

                    // Update climate
                    DaggerfallMesh dfMesh = go.GetComponent<DaggerfallMesh>();
                    dfMesh.SetClimate(climateBase, climateSeason, WindowStyle.Disabled);
                }
            }

            // Add combined GameObject
            if (dfUnity.Option_CombineRMB)
            {
                if (combiner.VertexCount > 0)
                {
                    combiner.Apply();
                    GameObject go = GameObjectHelper.CreateCombinedMeshGameObject(combiner, "CombinedModels", node.transform, dfUnity.Option_SetStaticFlags);

                    // Update climate
                    DaggerfallMesh dfMesh = go.GetComponent<DaggerfallMesh>();
                    dfMesh.SetClimate(climateBase, climateSeason, WindowStyle.Disabled);
                }
            }

            // Add static doors component
            DaggerfallStaticDoors c = this.gameObject.AddComponent<DaggerfallStaticDoors>();
            c.Doors = doors.ToArray();
        }

        /// <summary>
        /// Add interior flats.
        /// </summary>
        private void AddFlats()
        {
            GameObject node = new GameObject("Interior Flats");
            node.transform.parent = this.transform;

            // Add block flats
            markers.Clear();
            foreach (DFBlock.RmbBlockFlatObjectRecord obj in recordData.Interior.BlockFlatObjectRecords)
            {
                // Spawn billboard gameobject
                GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(obj.TextureArchive, obj.TextureRecord, node.transform);

                // Set position
                DaggerfallBillboard dfBillboard = go.GetComponent<DaggerfallBillboard>();
                go.transform.position = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
                go.transform.position += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);

                // Add to enter marker list, which is TEXTURE.199, index 8.
                // Sometimes marker 199.4 is used where the 199.8 enter marker should be
                // Being a little forgiving and also accepting 199.4 as enter marker
                // Will add more of these cases if I find them
                if (obj.TextureArchive == TextureReader.EditorFlatsTextureArchive && (obj.TextureRecord == 8 || obj.TextureRecord == 4))
                    markers.Add(go);
            }
        }

        /// <summary>
        /// Add interior people flats.
        /// </summary>
        private void AddPeople()
        {
            GameObject node = new GameObject("People Flats");
            node.transform.parent = this.transform;

            // Add block flats
            foreach (DFBlock.RmbBlockPeopleRecord obj in recordData.Interior.BlockPeopleRecords)
            {
                // Spawn billboard gameobject
                GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(obj.TextureArchive, obj.TextureRecord, node.transform);

                // Set position
                DaggerfallBillboard dfBillboard = go.GetComponent<DaggerfallBillboard>();
                go.transform.position = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
                go.transform.position += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);
            }
        }

        /// <summary>
        /// Add action doors to parent transform.
        /// </summary>
        private void AddActionDoors()
        {
            GameObject actionDoorsNode = new GameObject("Action Doors");
            actionDoorsNode.transform.parent = this.transform;

            foreach (DFBlock.RmbBlockDoorRecord obj in recordData.Interior.BlockDoorRecords)
            {
                // Create unique LoadID for save sytem
                ulong loadID = (ulong)(blockData.Position + obj.This);

                // Get model transform
                Vector3 modelRotation = new Vector3(0, -obj.YRotation / BlocksFile.RotationDivisor, 0);
                Vector3 modelPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;

                // Instantiate door prefab and add model
                GameObject go = GameObjectHelper.InstantiatePrefab(dfUnity.Option_InteriorDoorPrefab.gameObject, string.Empty, actionDoorsNode.transform, Vector3.zero);
                GameObjectHelper.CreateDaggerfallMeshGameObject(doorModelId, actionDoorsNode.transform, false, go, true);

                // Resize box collider to new mesh bounds
                BoxCollider boxCollider = go.GetComponent<BoxCollider>();
                MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                if (boxCollider != null && meshRenderer != null)
                {
                    boxCollider.center = meshRenderer.bounds.center;
                    boxCollider.size = meshRenderer.bounds.size;
                }

                // Apply transforms
                go.transform.rotation = Quaternion.Euler(modelRotation);
                go.transform.position = modelPosition;

                // Get action door script
                DaggerfallActionDoor actionDoor = go.GetComponent<DaggerfallActionDoor>();

                // Assign loadID
                if (actionDoor)
                    actionDoor.LoadID = loadID;
            }
        }

        #endregion
    }
}