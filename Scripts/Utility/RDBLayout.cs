// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Helper for laying out RDB (dungeon block) data in scene.
    /// </summary>
    public static class RDBLayout
    {
        public const float RDBSide = 2048f * MeshReader.GlobalScale;

        const int exitDoorModelID = 70300;
        const int redBrickDoorModelID = 72100;
        const int minTapestryID = 42500;
        const int maxTapestryID = 42571;

        //int[] textureTable = null;
        //DFRegion.DungeonTypes dungeonType = DFRegion.DungeonTypes.HumanStronghold;
        //List<GameObject> startMarkers = new List<GameObject>();
        //bool isStartingBlock;

        #region Structs & Enums

        /// <summary>
        /// Supports linked list of action objects.
        /// </summary>
        private struct ActionLink
        {
            public GameObject gameObject;
            public int nextKey;
            public int prevKey;
        }

        #endregion

        #region Layout Methods

        /// <summary>
        /// Create base RDB block by name.
        /// </summary>
        /// <param name="blockName">Name of block.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(string blockName, DaggerfallRDBBlock cloneFrom = null)
        {
            DFBlock blockData;
            return CreateBaseGameObject(blockName, out blockData, cloneFrom);
        }

        /// <summary>
        /// Create base RDB block by name and get back DFBlock data.
        /// </summary>
        /// <param name="blockName">Name of block.</param>
        /// <param name="blockDataOut">DFBlock data out.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(string blockName, out DFBlock blockDataOut, DaggerfallRDBBlock cloneFrom = null)
        {
            blockDataOut = new DFBlock();

            // Validate
            if (string.IsNullOrEmpty(blockName))
                return null;
            if (!blockName.ToUpper().EndsWith(".RDB"))
                return null;
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            // Get block data
            blockDataOut = dfUnity.ContentReader.BlockFileReader.GetBlock(blockName);

            return CreateBaseGameObject(ref blockDataOut, cloneFrom);
        }

        /// <summary>
        /// Instantiate base RDB block by DFBlock data.
        /// </summary>
        /// <param name="blockData">Block data.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(ref DFBlock blockData, DaggerfallRDBBlock cloneFrom = null)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            // Create gameobject
            GameObject go;
            string name = string.Format("DaggerfallBlock [{0}]", blockData.Name);
            if (cloneFrom != null)
            {
                go = GameObjectHelper.InstantiatePrefab(cloneFrom.gameObject, name, null, Vector3.zero);
            }
            else
            {
                go = new GameObject(name);
                go.AddComponent<DaggerfallRDBBlock>();
            }

            // Setup combiner
            ModelCombiner combiner = null;
            if (dfUnity.Option_CombineRDB)
                combiner = new ModelCombiner();

            // Add parent nodes
            GameObject modelsNode = new GameObject("Models");
            GameObject actionModelsNode = new GameObject("Action Models");
            modelsNode.transform.parent = go.transform;
            actionModelsNode.transform.parent = go.transform;
            AddModels(dfUnity, ref blockData, combiner, modelsNode.transform, actionModelsNode.transform);

            // Apply combiner
            if (combiner != null)
            {
                if (combiner.VertexCount > 0)
                {
                    combiner.Apply();
                    GameObjectHelper.CreateCombinedMeshGameObject(
                        combiner,
                        "CombinedModels",
                        modelsNode.transform,
                        dfUnity.Option_SetStaticFlags);
                }
            }

            return go;
        }

        /// <summary>
        /// Add exit doors to block.
        /// Any block can have several exit doors, which are just a quad slapped onto a wall.
        /// Only the starting block should add exit doors.
        /// </summary>
        public static void AddExitDoors(GameObject go, ref DFBlock blockData, out List<StaticDoor> doorsOut)
        {
            doorsOut = new List<StaticDoor>();

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Add parent node
            GameObject exitDoorsNode = new GameObject("Exit Doors");
            exitDoorsNode.transform.parent = go.transform;

            // Iterate all groups
            foreach (DFBlock.RdbObjectRoot group in blockData.RdbBlock.ObjectRootList)
            {
                // Skip empty object groups
                if (null == group.RdbObjects)
                    continue;

                // Look for models in this group
                foreach (DFBlock.RdbObject obj in group.RdbObjects)
                {
                    if (obj.Type == DFBlock.RdbResourceTypes.Model)
                    {
                        // Look for exit doors
                        int modelReference = obj.Resources.ModelResource.ModelIndex;
                        uint modelId = blockData.RdbBlock.ModelReferenceList[modelReference].ModelIdNum;
                        if (modelId == exitDoorModelID)
                        {
                            // Get model data and matrix
                            ModelData modelData;
                            dfUnity.MeshReader.GetModelData(modelId, out modelData);
                            Matrix4x4 modelMatrix = GetModelMatrix(obj);
                            GameObject exitDoorObject = AddStandaloneModel(dfUnity, ref modelData, modelMatrix, exitDoorsNode.transform, false, true);

                            // Add box collider
                            BoxCollider boxCollider = exitDoorObject.AddComponent<BoxCollider>();
                            boxCollider.isTrigger = true;

                            // Add static doors
                            doorsOut.AddRange(GameObjectHelper.GetStaticDoors(ref modelData, blockData.Index, 0, modelMatrix));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add actions doors to block.
        /// </summary>
        public static void AddActionDoors(GameObject go, ref DFBlock blockData)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Add parent node
            GameObject actionDoorsNode = new GameObject("Action Doors");
            actionDoorsNode.transform.parent = go.transform;

            // Iterate all groups
            foreach (DFBlock.RdbObjectRoot group in blockData.RdbBlock.ObjectRootList)
            {
                // Skip empty object groups
                if (null == group.RdbObjects)
                    continue;

                // Look for models in this group
                foreach (DFBlock.RdbObject obj in group.RdbObjects)
                {
                    if (obj.Type == DFBlock.RdbResourceTypes.Model)
                    {
                        // Look for action doors
                        int modelReference = obj.Resources.ModelResource.ModelIndex;
                        uint modelId = blockData.RdbBlock.ModelReferenceList[modelReference].ModelIdNum;
                        if (IsActionDoor(ref blockData, obj, modelReference))
                            AddActionDoor(dfUnity, modelId, obj, actionDoorsNode.transform);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds models and their actions to scene.
        /// </summary>
        private static void AddModels(
            DaggerfallUnity dfUnity,
            ref DFBlock blockData,
            ModelCombiner combiner = null,
            Transform modelsParent = null,
            Transform actionModelsParent = null)
        {
            // Action record linkages
            Dictionary<int, ActionLink> actionLinkDict = new Dictionary<int, ActionLink>();

            // Iterate object groups
            int groupIndex = 0;
            foreach (DFBlock.RdbObjectRoot group in blockData.RdbBlock.ObjectRootList)
            {
                // Skip empty object groups
                if (null == group.RdbObjects)
                {
                    groupIndex++;
                    continue;
                }

                // Iterate objects in this group
                foreach (DFBlock.RdbObject obj in group.RdbObjects)
                {
                    // Add models
                    if (obj.Type == DFBlock.RdbResourceTypes.Model)
                    {
                        // Get model reference index and id
                        int modelReference = obj.Resources.ModelResource.ModelIndex;
                        uint modelId = blockData.RdbBlock.ModelReferenceList[modelReference].ModelIdNum;

                        // Filter exit door models
                        // These must be added by AddExitDoors()
                        if (modelId == exitDoorModelID)
                            continue;

                        // Filter action door models
                        // These must be added by AddActionDoors()
                        if (IsActionDoor(ref blockData, obj, modelReference))
                            continue;

                        // Get matrix
                        Matrix4x4 modelMatrix = GetModelMatrix(obj);

                        // Get model data
                        ModelData modelData;
                        dfUnity.MeshReader.GetModelData(modelId, out modelData);

                        // Check if model has an action record
                        bool hasAction = HasAction(obj);

                        // Special handling for tapestries and banners
                        // Some of these are so far out from wall player can become stuck behind them
                        // Adding model invidually without collider to avoid problem
                        // Not sure if these object ever actions, but bypass this hack if they do
                        if (modelId >= minTapestryID && modelId <= maxTapestryID && !hasAction)
                        {
                            AddStandaloneModel(dfUnity, ref modelData, modelMatrix, modelsParent, hasAction, true);
                            continue;
                        }

                        // Add or combine
                        GameObject standaloneObject = null;
                        Transform parent = (hasAction) ? actionModelsParent : modelsParent;
                        if (combiner == null || hasAction)
                            standaloneObject = AddStandaloneModel(dfUnity, ref modelData, modelMatrix, parent, hasAction);
                        else
                            combiner.Add(ref modelData, modelMatrix);

                        // Add action
                        if (hasAction && standaloneObject != null)
                            AddAction(standaloneObject, blockData, obj, modelReference, groupIndex, actionLinkDict);
                    }
                }

                // Increment group index
                groupIndex++;
            }

            // Link action nodes
            LinkActionNodes(actionLinkDict);
        }

        /// <summary>
        /// Extracts correct matrix from model data.
        /// </summary>
        private static Matrix4x4 GetModelMatrix(DFBlock.RdbObject obj)
        {
            // Get rotation angle for each axis
            float degreesX = -obj.Resources.ModelResource.XRotation / BlocksFile.RotationDivisor;
            float degreesY = -obj.Resources.ModelResource.YRotation / BlocksFile.RotationDivisor;
            float degreesZ = -obj.Resources.ModelResource.ZRotation / BlocksFile.RotationDivisor;

            // Calcuate transform
            Vector3 position = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;

            // Calculate matrix
            Vector3 rx = new Vector3(degreesX, 0, 0);
            Vector3 ry = new Vector3(0, degreesY, 0);
            Vector3 rz = new Vector3(0, 0, degreesZ);
            Matrix4x4 modelMatrix = Matrix4x4.identity;
            modelMatrix *= Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
            modelMatrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rz), Vector3.one);
            modelMatrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rx), Vector3.one);
            modelMatrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(ry), Vector3.one);

            return modelMatrix;
        }

        /// <summary>
        /// Add a standalone model when not combining.
        /// </summary>
        private static GameObject AddStandaloneModel(
            DaggerfallUnity dfUnity,
            ref ModelData modelData,
            Matrix4x4 matrix,
            Transform parent,
            bool overrideStatic = false,
            bool ignoreCollider = false)
        {
            // Determine static flag
            bool isStatic = (dfUnity.Option_SetStaticFlags && !overrideStatic) ? true : false;

            // Add GameObject
            uint modelID = (uint)modelData.DFMesh.ObjectId;
            GameObject go = GameObjectHelper.CreateDaggerfallMeshGameObject(modelID, parent, isStatic, null, ignoreCollider);
            go.transform.position = matrix.GetColumn(3);
            go.transform.rotation = GameObjectHelper.QuaternionFromMatrix(matrix);

            return go;
        }

        /// <summary>
        /// Check if model is a hinged action door.
        /// </summary>
        private static bool IsActionDoor(ref DFBlock blockData, DFBlock.RdbObject obj, int modelReference)
        {
            // Always reject red brick doors, they are not action doors despite having "DOR" attached
            if (blockData.RdbBlock.ModelReferenceList[modelReference].ModelIdNum == redBrickDoorModelID)
                return false;

            // Otherwise Check if this is a door (DOR) or double-door (DDR)
            string description = blockData.RdbBlock.ModelReferenceList[modelReference].Description;
            if (description == "DOR" || description == "DDR")
                return true;

            return false;
        }

        /// <summary>
        /// Check is model has action record.
        /// </summary>
        private static bool HasAction(DFBlock.RdbObject obj)
        {
            DFBlock.RdbActionResource action = obj.Resources.ModelResource.ActionResource;
            if (action.Flags != 0)
                return true;

            return false;
        }

        /// <summary>
        /// Creates action key unique within group.
        /// </summary>
        private static int GetActionKey(int groupIndex, int objIndex)
        {
            // Create action key for this object
            return groupIndex * 1000 + objIndex;
        }

        /// <summary>
        /// Constructs a Vector3 from magnitude and direction in RDB action resource.
        /// </summary>
        private static Vector3 GetActionVector(ref DFBlock.RdbActionResource resource)
        {
            Vector3 vector = Vector3.zero;
            float magnitude = resource.Magnitude;
            switch (resource.Axis)
            {
                case DFBlock.RdbActionAxes.NegativeX:
                    vector.x = -magnitude;
                    break;
                case DFBlock.RdbActionAxes.NegativeY:
                    vector.y = -magnitude;
                    break;
                case DFBlock.RdbActionAxes.NegativeZ:
                    vector.z = -magnitude;
                    break;

                case DFBlock.RdbActionAxes.PositiveX:
                    vector.x = magnitude;
                    break;
                case DFBlock.RdbActionAxes.PositiveY:
                    vector.y = magnitude;
                    break;
                case DFBlock.RdbActionAxes.PositiveZ:
                    vector.z = magnitude;
                    break;

                default:
                    magnitude = 0f;
                    break;
            }

            return vector;
        }

        /// <summary>
        /// Add action to model.
        /// </summary>
        private static void AddAction(
            GameObject go,
            DFBlock blockData,
            DFBlock.RdbObject obj,
            int modelReference,
            int groupIndex,
            Dictionary<int, ActionLink> actionLinkDict)
        {
            // Get model action record and description
            DFBlock.RdbActionResource action = obj.Resources.ModelResource.ActionResource;
            string description = blockData.RdbBlock.ModelReferenceList[modelReference].Description;

            // Check for known action types
            Vector3 actionRotation = Vector3.zero;
            Vector3 actionTranslation = Vector3.zero;
            if ((action.Flags & (int)DFBlock.RdbActionFlags.Rotation) == (int)DFBlock.RdbActionFlags.Rotation)
                actionRotation = (GetActionVector(ref action) / BlocksFile.RotationDivisor);
            if ((action.Flags & (int)DFBlock.RdbActionFlags.Translation) == (int)DFBlock.RdbActionFlags.Translation)
                actionTranslation = GetActionVector(ref action) * MeshReader.GlobalScale;

            // A quick hack to fix special-case rotation issues.
            // Currently unknown if there is data indicating different rotation behaviour or if something else is happening.
            switch (description)
            {
                case "LID":
                    actionRotation = new Vector3(0, 0, -90f);       // Coffin lids (e.g. Scourg barrow)
                    break;
                case "WHE":
                    actionRotation = new Vector3(0, -360f, 0);      // Wheels (e.g. Direnni Tower)
                    break;
            }

            // Create action component
            DaggerfallAction c = go.AddComponent<DaggerfallAction>();
            c.ActionEnabled = true;
            c.ModelDescription = description;
            c.ActionRotation = actionRotation;
            c.ActionTranslation = actionTranslation;
            c.ActionSoundID = obj.Resources.ModelResource.SoundId;

            // Using 1/20 of native value in seconds
            // This seems to match game very closely
            c.ActionDuration = (float)action.Duration / 20f;
            c.ActionFlags = action.Flags;

            // Create action links
            ActionLink link;
            link.gameObject = go;
            link.nextKey = GetActionKey(groupIndex, action.NextObjectIndex);
            link.prevKey = GetActionKey(groupIndex, action.PreviousObjectIndex);
            actionLinkDict.Add(GetActionKey(groupIndex, obj.Index), link);

            // Add sound
            AddActionAudioSource(go, (uint)c.ActionSoundID);

            return;
        }

        /// <summary>
        /// Adds action audio.
        /// </summary>
        private static void AddActionAudioSource(GameObject go, uint id)
        {
            if (id > 0)
            {
                DaggerfallAudioSource c = go.AddComponent<DaggerfallAudioSource>();
                c.SetSound(id);
            }
        }

        /// <summary>
        /// Links action chains together.
        /// </summary>
        private static void LinkActionNodes(Dictionary<int, ActionLink> actionLinkDict)
        {
            // Exit if no actions
            if (actionLinkDict.Count == 0)
                return;

            // Iterate through actions
            foreach (var item in actionLinkDict)
            {
                ActionLink link = item.Value;

                // Link to next node
                if (actionLinkDict.ContainsKey(link.nextKey))
                    link.gameObject.GetComponent<DaggerfallAction>().NextObject = actionLinkDict[link.nextKey].gameObject;

                // Link to previous node
                if (actionLinkDict.ContainsKey(link.prevKey))
                    link.gameObject.GetComponent<DaggerfallAction>().PreviousObject = actionLinkDict[link.prevKey].gameObject;
            }
        }

        /// <summary>
        /// Adds action door to scene.
        /// </summary>
        private static void AddActionDoor(DaggerfallUnity dfUnity, uint modelId, DFBlock.RdbObject obj, Transform parent)
        {
            if (dfUnity.Option_DungeonDoorPrefab == null)
                return;

            // Get model data and matrix
            ModelData modelData;
            dfUnity.MeshReader.GetModelData(modelId, out modelData);
            Matrix4x4 modelMatrix = GetModelMatrix(obj);

            // Instantiate door prefab and add model
            GameObject go = GameObjectHelper.InstantiatePrefab(dfUnity.Option_DungeonDoorPrefab.gameObject, string.Empty, parent, Vector3.zero);
            GameObjectHelper.CreateDaggerfallMeshGameObject(modelId, parent, false, go, true);

            // Resize box collider to new mesh bounds
            BoxCollider boxCollider = go.GetComponent<BoxCollider>();
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            if (boxCollider != null && meshRenderer != null)
            {
                boxCollider.center = meshRenderer.bounds.center;
                boxCollider.size = meshRenderer.bounds.size;
            }

            // Get rotation angle for each axis
            float degreesX = -obj.Resources.ModelResource.XRotation / BlocksFile.RotationDivisor;
            float degreesY = -obj.Resources.ModelResource.YRotation / BlocksFile.RotationDivisor;
            float degreesZ = -obj.Resources.ModelResource.ZRotation / BlocksFile.RotationDivisor;

            // Apply transforms
            go.transform.Rotate(0, degreesY, 0, Space.World);
            go.transform.Rotate(degreesX, 0, 0, Space.World);
            go.transform.Rotate(0, 0, degreesZ, Space.World);
            go.transform.localPosition = modelMatrix.GetColumn(3);
        }

        #endregion

        #region Deprecated Methods

        ///// <summary>
        ///// Creates a new RDB GameObject and performs block layout.
        ///// Can pass information about dungeon for texture swaps and random enemies.
        ///// </summary>
        ///// <param name="dfUnity">DaggerfallUnity singleton. Required for content readers and settings.</param>
        ///// <param name="blockName">Name of RDB block to build.</param>
        ///// <param name="isStartingBlock">True if this is the starting block. Controls exit doors.</param>
        ///// <param name="textureTable">Dungeon texture table.</param>
        ///// <param name="dungeonType">Type of dungeon for random encounter tables.</param>
        ///// <param name="seed">Seed for random encounters.</param>
        ///// <returns>GameObject.</returns>
        //public static GameObject CreateGameObjectDeprecated(
        //    string blockName,
        //    bool isStartingBlock,
        //    int[] textureTable = null,
        //    DFRegion.DungeonTypes dungeonType = DFRegion.DungeonTypes.HumanStronghold,
        //    int seed = 0)
        //{
        //    // Validate
        //    if (string.IsNullOrEmpty(blockName))
        //        return null;
        //    if (!blockName.ToUpper().EndsWith(".RDB"))
        //        return null;
        //    DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
        //    if (!dfUnity.IsReady)
        //        return null;

        //    // Use default texture table if one not specified
        //    if (textureTable == null)
        //        textureTable = StaticTextureTables.DefaultTextureTable;

        //    // Create gameobject
        //    GameObject go = new GameObject(string.Format("DaggerfallBlock [Name={0}]", blockName));
        //    DaggerfallRDBBlock dfBlock = go.AddComponent<DaggerfallRDBBlock>();

        //    // Start new layout
        //    RDBLayout layout = new RDBLayout(blockName);
        //    layout.isStartingBlock = isStartingBlock;
        //    layout.textureTable = textureTable;
        //    layout.dungeonType = dungeonType;
        //    layout.staticModelsNode = new GameObject("Static Models");
        //    layout.actionModelsNode = new GameObject("Action Models");
        //    layout.doorsNode = new GameObject("Doors");
        //    layout.flatsNode = new GameObject("Flats");
        //    layout.lightsNode = new GameObject("Lights");
        //    layout.enemiesNode = new GameObject("Enemies");

        //    // Parent child game objects
        //    layout.staticModelsNode.transform.parent = go.transform;
        //    layout.actionModelsNode.transform.parent = go.transform;
        //    layout.doorsNode.transform.parent = go.transform;
        //    layout.flatsNode.transform.parent = go.transform;
        //    layout.lightsNode.transform.parent = go.transform;
        //    layout.enemiesNode.transform.parent = go.transform;

        //    // List to receive any exit doors found
        //    List<StaticDoor> allDoors = new List<StaticDoor>();

        //    // Seed random generator
        //    UnityEngine.Random.seed = seed;

        //    // Iterate object groups
        //    layout.groupIndex = 0;
        //    DFBlock blockData = dfUnity.ContentReader.BlockFileReader.GetBlock(blockName);
        //    foreach (DFBlock.RdbObjectRoot group in blockData.RdbBlock.ObjectRootList)
        //    {
        //        // Skip empty object groups
        //        if (null == group.RdbObjects)
        //        {
        //            layout.groupIndex++;
        //            continue;
        //        }

        //        // Iterate objects in this group
        //        List<StaticDoor> modelDoors;
        //        foreach (DFBlock.RdbObject obj in group.RdbObjects)
        //        {
        //            // Handle by object type
        //            switch (obj.Type)
        //            {
        //                case DFBlock.RdbResourceTypes.Model:
        //                    layout.AddRDBModel(obj, out modelDoors, layout.staticModelsNode.transform);
        //                    if (modelDoors.Count > 0) allDoors.AddRange(modelDoors);
        //                    break;
        //                case DFBlock.RdbResourceTypes.Flat:
        //                    layout.AddRDBFlat(obj, layout.flatsNode.transform);
        //                    break;
        //                case DFBlock.RdbResourceTypes.Light:
        //                    layout.AddRDBLight(obj, layout.lightsNode.transform);
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }

        //        // Increment group index
        //        layout.groupIndex++;
        //    }

        //    // Link action nodes
        //    layout.LinkActionNodes();

        //    // Combine meshes
        //    if (dfUnity.Option_CombineRDB)
        //    {
        //        layout.combiner.Apply();
        //        GameObject cgo = GameObjectHelper.CreateCombinedMeshGameObject(layout.combiner, "CombinedMeshes", layout.staticModelsNode.transform, dfUnity.Option_SetStaticFlags);
        //        cgo.GetComponent<DaggerfallMesh>().SetDungeonTextures(textureTable);
        //    }

        //    // Fix enemy standing positions for this block
        //    // Some enemies are floating in air or sunk into ground
        //    // Can only adjust this after geometry instantiated
        //    layout.FixEnemyStanding(go);

        //    // Store start markers in block
        //    dfBlock.SetStartMarkers(layout.startMarkers.ToArray());

        //    // Add doors
        //    if (allDoors.Count > 0)
        //        layout.AddDoors(allDoors.ToArray(), go);

        //    return go;
        //}

        //private void AddRDBModel(DFBlock.RdbObject obj, out List<StaticDoor> doorsOut, Transform parent)
        //{
        //    bool overrideCombine = false;
        //    doorsOut = new List<StaticDoor>();

        //    // Get model reference index and id
        //    int modelReference = obj.Resources.ModelResource.ModelIndex;
        //    uint modelId = blockData.RdbBlock.ModelReferenceList[modelReference].ModelIdNum;

        //    // Get rotation angle for each axis
        //    float degreesX = -obj.Resources.ModelResource.XRotation / BlocksFile.RotationDivisor;
        //    float degreesY = -obj.Resources.ModelResource.YRotation / BlocksFile.RotationDivisor;
        //    float degreesZ = -obj.Resources.ModelResource.ZRotation / BlocksFile.RotationDivisor;

        //    // Calcuate transform
        //    Vector3 position = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;

        //    // Calculate matrix
        //    Vector3 rx = new Vector3(degreesX, 0, 0);
        //    Vector3 ry = new Vector3(0, degreesY, 0);
        //    Vector3 rz = new Vector3(0, 0, degreesZ);
        //    Matrix4x4 modelMatrix = Matrix4x4.identity;
        //    modelMatrix *= Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
        //    modelMatrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rz), Vector3.one);
        //    modelMatrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rx), Vector3.one);
        //    modelMatrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(ry), Vector3.one);

        //    // Get model data
        //    ModelData modelData;
        //    dfUnity.MeshReader.GetModelData(modelId, out modelData);

        //    // Discard static exit doors not in starting block
        //    // Exit doors are just a model slapped over wall or doorway
        //    // This allows Daggerfall to toggle exits on and off
        //    if (modelData.DFMesh.ObjectId == 70300)
        //    {
        //        if (!isStartingBlock)
        //            return;
        //    }

        //    // Doors - there are no working static building doors inside dungeons, just the exits and action doors
        //    bool isActionDoor = IsActionDoor(blockData, obj, modelReference);
        //    if (isActionDoor)
        //        parent = doorsNode.transform;
        //    else if (modelData.Doors != null)
        //        doorsOut.AddRange(GameObjectHelper.GetStaticDoors(ref modelData, blockData.Index, 0, modelMatrix));

        //    // Action records
        //    bool hasAction = HasAction(blockData, obj, modelReference);
        //    if (hasAction)
        //        parent = actionModelsNode.transform;

        //    // Flags
        //    bool isStatic = dfUnity.Option_SetStaticFlags;
        //    if (isActionDoor || hasAction)
        //    {
        //        // Moving objects are never static or combined
        //        isStatic = false;
        //        overrideCombine = true;
        //    }

        //    // Add to scene
        //    if (dfUnity.Option_CombineRDB && !overrideCombine)
        //    {
        //        combiner.Add(ref modelData, modelMatrix);
        //    }
        //    else
        //    {
        //        // Spawn mesh gameobject
        //        GameObject go = GameObjectHelper.CreateDaggerfallMeshGameObject(modelId, parent, isStatic);
        //        go.GetComponent<DaggerfallMesh>().SetDungeonTextures(textureTable);

        //        // Apply transforms
        //        go.transform.Rotate(0, degreesY, 0, Space.World);
        //        go.transform.Rotate(degreesX, 0, 0, Space.World);
        //        go.transform.Rotate(0, 0, degreesZ, Space.World);
        //        go.transform.localPosition = position;

        //        // Add action door
        //        if (isActionDoor)
        //        { 
        //            DaggerfallActionDoor dfActionDoor = go.AddComponent<DaggerfallActionDoor>();

        //            // Add action door audio
        //            if (dfUnity.Option_DefaultSounds)
        //            {
        //                AddActionDoorAudioSource(go);
        //                dfActionDoor.SetDungeonDoorSounds();
        //            }
        //        }

        //        // Add action component
        //        if (hasAction && !isActionDoor)
        //            AddAction(go, blockData, obj, modelReference);
        //    }
        //}

        //private void AddRDBFlat(DFBlock.RdbObject obj, Transform parent)
        //{
        //    int archive = obj.Resources.FlatResource.TextureArchive;
        //    int record = obj.Resources.FlatResource.TextureRecord;

        //    // Spawn billboard gameobject
        //    GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(archive, record, parent, true);
        //    Vector3 billboardPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;

        //    // Add RDB data to billboard
        //    DaggerfallBillboard dfBillboard = go.GetComponent<DaggerfallBillboard>();
        //    dfBillboard.SetResourceData(obj.Resources.FlatResource);

        //    // Set transform
        //    go.transform.position = billboardPosition;

        //    // Handle supported editor flats
        //    if (archive == 199)
        //    {
        //        switch (record)
        //        {
        //            case 10:                        // Start marker
        //                startMarkers.Add(go);
        //                break;
        //            case 15:                        // Random enemy
        //                if (dfUnity.Option_ImportEnemies)
        //                {
        //                    AddRandomRDBEnemy(obj);
        //                    go.SetActive(false);
        //                }
        //                break;
        //            case 16:                        // Fixed enemy
        //                if (dfUnity.Option_ImportEnemies)
        //                {
        //                    AddFixedRDBEnemy(obj);
        //                    go.SetActive(false);
        //                }
        //                break;
        //        }
        //    }

        //    // Add torch burning sound
        //    if (dfUnity.Option_DefaultSounds && archive == 210)
        //    {
        //        switch (record)
        //        {
        //            case 0:
        //            case 1:
        //            case 6:
        //            case 16:
        //            case 17:
        //            case 18:
        //            case 19:
        //            case 20:
        //                AddTorchAudioSource(go);
        //                break;
        //        }
        //    }
        //}

        //private void AddRDBLight(DFBlock.RdbObject obj, Transform parent)
        //{
        //    //// Do nothing if import option not enabled
        //    //if (!dfUnity.Option_ImportPointLights)
        //    //    return;

        //    //// Spawn light gameobject
        //    //float radius = obj.Resources.LightResource.Radius * MeshReader.GlobalScale;
        //    //GameObject go = GameObjectHelper.CreateDaggerfallRDBPointLight(radius, parent);
        //    //Vector3 lightPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;

        //    //// Add component
        //    //DaggerfallLight c = go.AddComponent<DaggerfallLight>();
        //    //if (dfUnity.Option_AnimatedPointLights)
        //    //    c.Animate = true;

        //    //// Set transform
        //    //go.transform.position = lightPosition;
        //}

        //private void AddFixedRDBEnemy(DFBlock.RdbObject obj)
        //{
        //    // Get type value and ignore known invalid types
        //    int typeValue = (int)(obj.Resources.FlatResource.FactionMobileId & 0xff);
        //    if (typeValue == 99)
        //        return;

        //    // Cast to enum
        //    MobileTypes type = (MobileTypes)(obj.Resources.FlatResource.FactionMobileId & 0xff);

        //    AddEnemy(obj, type);
        //}

        //private void AddRandomRDBEnemy(DFBlock.RdbObject obj)
        //{
        //    // Get dungeon type index
        //    int index = (int)dungeonType >> 8;
        //    if (index < RandomEncounters.EncounterTables.Length)
        //    {
        //        // Get encounter table
        //        RandomEncounterTable table = RandomEncounters.EncounterTables[index];

        //        // Get random monster from table
        //        // Normally this would be weighted by player level
        //        MobileTypes type = table.Enemies[UnityEngine.Random.Range(0, table.Enemies.Length)];

        //        // Add enemy
        //        AddEnemy(obj, type);
        //    }
        //    else
        //    {
        //        DaggerfallUnity.LogMessage(string.Format("RDBLayout: Dungeon type {0} is out of range or unknown.", dungeonType), true);
        //    }
        //}

        //private void AddEnemy(DFBlock.RdbObject obj, MobileTypes type)
        //{
        //    // Get default reaction
        //    MobileReactions reaction = MobileReactions.Hostile;
        //    if (obj.Resources.FlatResource.FlatData.Reaction == (int)DFBlock.EnemyReactionTypes.Passive)
        //        reaction = MobileReactions.Passive;

        //    // Spawn enemy gameobject
        //    GameObject go = GameObjectHelper.CreateDaggerfallEnemyGameObject(type, enemiesNode.transform, reaction);
        //    if (go == null)
        //        return;

        //    // Set transform
        //    Vector3 enemyPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
        //    go.transform.position = enemyPosition;
        //}

        //private void FixEnemyStanding(GameObject go)
        //{
        //    Component[] mobiles = go.GetComponentsInChildren(typeof(DaggerfallMobileUnit));
        //    if (mobiles == null)
        //        return;

        //    foreach (DaggerfallMobileUnit enemy in mobiles)
        //    {
        //        // Don't change for flying enemies
        //        if (enemy.Summary.Enemy.Behaviour == MobileBehaviour.Flying)
        //            continue;

        //        // Align to ground
        //        Vector2 size = enemy.Summary.RecordSizes[0];
        //        GameObjectHelper.AlignBillboardToGround(enemy.transform.parent.gameObject, size);
        //    }
        //}

        //private void AddTorchAudioSource(GameObject go)
        //{
        //    // Apply looping burning sound to flaming torches and fires
        //    // Set to linear rolloff or the burning sound is audible almost everywhere
        //    DaggerfallAudioSource c = go.AddComponent<DaggerfallAudioSource>();
        //    c.AudioSource.dopplerLevel = 0;
        //    c.AudioSource.rolloffMode = AudioRolloffMode.Linear;
        //    c.AudioSource.maxDistance = 4f;
        //    c.AudioSource.volume = 0.6f;
        //    c.SetSound(SoundClips.Burning, AudioPresets.LoopIfPlayerNear);
        //}

        //private void AddDoors(StaticDoor[] doors, GameObject target)
        //{
        //    if (doors != null && target != null)
        //    {
        //        DaggerfallStaticDoors c = target.AddComponent<DaggerfallStaticDoors>();
        //        c.Doors = doors;
        //    }
        //}

        #endregion
    }
}