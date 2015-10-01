// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Lypyl (lypyl@dfworkshop.net)
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
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Helper for laying out RDB (dungeon block) data in scene.
    /// </summary>
    public static class RDBLayout
    {
        public const float RDBSide = 2048f * MeshReader.GlobalScale;

        // Special model IDs
        const int exitDoorModelID = 70300;
        const int redBrickDoorModelID = 72100;
        const int minTapestryID = 42500;
        const int maxTapestryID = 42571;

        // Torch audio range and volume
        const float torchMaxDistance = 5f;
        const float torchVolume = 0.7f;

        #region Structs & Enums

        /// <summary>
        /// Supports linked list of action objects.
        /// </summary>
        public struct ActionLink
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
        /// <param name="textureTable">Optional texture table for dungeon.</param>
        /// <param name="allowExitDoors">Add exit doors to block.</param>
        /// <param name="cloneFrom">Clone and build on a prefab object template.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(
            string blockName,
            ref Dictionary<int, ActionLink> actionLinkDict,
            int[] textureTable = null,
            bool allowExitDoors = true,
            DaggerfallRDBBlock cloneFrom = null)
        {
            DFBlock blockData;
            return CreateBaseGameObject(blockName, ref actionLinkDict, out blockData, textureTable, allowExitDoors, cloneFrom);
        }

        /// <summary>
        /// Create base RDB block by name and get back DFBlock data.
        /// </summary>
        /// <param name="blockName">Name of block.</param>
        /// <param name="blockDataOut">DFBlock data out.</param>
        /// <param name="textureTable">Optional texture table for dungeon.</param>
        /// <param name="allowExitDoors">Add exit doors to block.</param>
        /// <param name="cloneFrom">Clone and build on a prefab object template.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(
            string blockName,
            ref Dictionary<int, ActionLink> actionLinkDict,
            out DFBlock blockDataOut,
            int[] textureTable = null,
            bool allowExitDoors = true,
            DaggerfallRDBBlock cloneFrom = null)
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

            return CreateBaseGameObject(ref blockDataOut, ref actionLinkDict, textureTable, allowExitDoors, cloneFrom);
        }

        /// <summary>
        /// Instantiate base RDB block by DFBlock data.
        /// </summary>
        /// <param name="blockData">Block data.</param>
        /// <param name="textureTable">Optional texture table for dungeon.</param>
        /// <param name="allowExitDoors">Add exit doors to block.</param>
        /// <param name="cloneFrom">Clone and build on a prefab object template.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(
            ref DFBlock blockData,
            ref Dictionary<int, ActionLink> actionLinkDict,
            int[] textureTable = null,
            bool allowExitDoors = true,
            DaggerfallRDBBlock cloneFrom = null)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            // Use default texture table if one not specified
            if (textureTable == null)
                textureTable = StaticTextureTables.DefaultTextureTable;

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

            // Add parent node
            GameObject modelsNode = new GameObject("Models");
            GameObject actionModelsNode = new GameObject("Action Models");
            modelsNode.transform.parent = go.transform;
            actionModelsNode.transform.parent = go.transform;

            // Add models
            List<StaticDoor> exitDoors;
            AddModels(
                dfUnity,
                ref blockData,
                ref actionLinkDict,
                textureTable,
                allowExitDoors,
                out exitDoors,
                combiner,
                modelsNode.transform,
                actionModelsNode.transform);

            // Apply combiner
            if (combiner != null)
            {
                if (combiner.VertexCount > 0)
                {
                    combiner.Apply();
                    GameObject cgo = GameObjectHelper.CreateCombinedMeshGameObject(
                        combiner,
                        "CombinedModels",
                        modelsNode.transform,
                        dfUnity.Option_SetStaticFlags);
                    cgo.GetComponent<DaggerfallMesh>().SetDungeonTextures(textureTable);
                }
            }

            // Add exit doors
            if (exitDoors.Count > 0)
            {
                DaggerfallStaticDoors c = go.AddComponent<DaggerfallStaticDoors>();
                c.Doors = exitDoors.ToArray();
            }

            return go;
        }

        /// <summary>
        /// Add actions doors to block.
        /// </summary>
        public static void AddActionDoors(GameObject go, ref Dictionary<int, ActionLink> actionLinkDict, ref DFBlock blockData, int[] textureTable)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Use default texture table if one not specified
            if (textureTable == null)
                textureTable = StaticTextureTables.DefaultTextureTable;

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
                        // Create unique LoadID for save sytem
                        long loadID = (blockData.Index << 24) + obj.This;

                        // Look for action doors
                        int modelReference = obj.Resources.ModelResource.ModelIndex;
                        uint modelId = blockData.RdbBlock.ModelReferenceList[modelReference].ModelIdNum;
                        if (IsActionDoor(ref blockData, obj, modelReference))
                        {
                            GameObject cgo = AddActionDoor(dfUnity, modelId, obj, actionDoorsNode.transform, loadID);
                            cgo.GetComponent<DaggerfallMesh>().SetDungeonTextures(textureTable);

                            // Add action component to door if it also has an action
                            if (HasAction(obj))
                            {
                                AddActionModelHelper(cgo, ref actionLinkDict, obj, blockData);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add light prefabs.
        /// </summary>
        public static void AddLights(GameObject go, ref DFBlock blockData)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Do nothing if import option not enabled or missing prefab
            if (!dfUnity.Option_ImportLightPrefabs || dfUnity.Option_DungeonLightPrefab == null)
                return;

            // Add parent node
            GameObject lightsNode = new GameObject("Lights");
            lightsNode.transform.parent = go.transform;

            // Iterate all groups
            foreach (DFBlock.RdbObjectRoot group in blockData.RdbBlock.ObjectRootList)
            {
                // Skip empty object groups
                if (null == group.RdbObjects)
                    continue;

                // Look for lights in this group
                foreach (DFBlock.RdbObject obj in group.RdbObjects)
                {
                    if (obj.Type == DFBlock.RdbResourceTypes.Light)
                        AddLight(dfUnity, obj, lightsNode.transform);
                }
            }
        }

        /// <summary>
        /// Add all block flats.
        /// </summary>
        public static void AddFlats(
            GameObject go,
            ref Dictionary<int, ActionLink> actionLinkDict,
            ref DFBlock blockData,
            out DFBlock.RdbObject[] editorObjectsOut,
            out GameObject[] startMarkersOut,
            out GameObject[] enterMarkersOut)
        {
            List<DFBlock.RdbObject> editorObjects = new List<DFBlock.RdbObject>();
            List<GameObject> startMarkers = new List<GameObject>();
            List<GameObject> enterMarkers = new List<GameObject>();
            editorObjectsOut = null;
            startMarkersOut = null;
            enterMarkersOut = null;

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Add parent node
            GameObject flatsNode = new GameObject("Flats");
            flatsNode.transform.parent = go.transform;

            // Iterate all groups
            foreach (DFBlock.RdbObjectRoot group in blockData.RdbBlock.ObjectRootList)
            {
                // Skip empty object groups
                if (null == group.RdbObjects)
                    continue;

                // Look for flats in this group
                foreach (DFBlock.RdbObject obj in group.RdbObjects)
                {
                    if (obj.Type == DFBlock.RdbResourceTypes.Flat)
                    {
                        // Add flat
                        GameObject flatObject = AddFlat(obj, flatsNode.transform);

                        // Store editor objects and start markers
                        int archive = obj.Resources.FlatResource.TextureArchive;
                        int record = obj.Resources.FlatResource.TextureRecord;
                        if (archive == TextureReader.EditorFlatsTextureArchive)
                        {
                            editorObjects.Add(obj);
                            if (record == 10)
                                startMarkers.Add(flatObject);
                            else if (record == 8)
                                enterMarkers.Add(flatObject);

                            //add editor flats to actionLinkDict
                            if (!actionLinkDict.ContainsKey(obj.This))
                            {
                                ActionLink link;
                                link.gameObject = flatObject;
                                link.nextKey = obj.Resources.FlatResource.NextObjectOffset;
                                link.prevKey = -1;
                                actionLinkDict.Add(obj.This, link);
                            }

                        }

                        //add action component to flat if it has an action
                        if (obj.Resources.FlatResource.Action > 0)
                        {
                            AddActionFlatHelper(flatObject, ref actionLinkDict, blockData, obj);
                        }

                    }
                }
            }

            // Output editor objects
            editorObjectsOut = editorObjects.ToArray();
            startMarkersOut = startMarkers.ToArray();
            enterMarkersOut = enterMarkers.ToArray();
        }

        /// <summary>
        /// Add fixed enemies.
        /// </summary>
        /// <param name="go">GameObject to add monsters to.</param>
        /// <param name="editorObjects">Editor objects containing flats.</param>
        public static void AddFixedEnemies(
            GameObject go,
            DFBlock.RdbObject[] editorObjects)
        {
            const int fixedMonsterFlatIndex = 16;

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Must have import enabled and prefab set
            if (!dfUnity.Option_ImportEnemyPrefabs || dfUnity.Option_EnemyPrefab == null)
                return;

            // Editor objects array must be populated
            if (editorObjects == null || editorObjects.Length == 0)
                return;

            // Add parent node
            GameObject fixedEnemiesNode = new GameObject("Fixed Enemies");
            fixedEnemiesNode.transform.parent = go.transform;

            // Iterate editor flats for enemies
            for (int i = 0; i < editorObjects.Length; i++)
            {
                // Add fixed enemy objects
                if (editorObjects[i].Resources.FlatResource.TextureRecord == fixedMonsterFlatIndex)
                    AddFixedRDBEnemy(editorObjects[i], fixedEnemiesNode.transform);
            }
        }

        /// <summary>
        /// Add random enemies from encounter tables based on dungeon type, monster power, and seed.
        /// </summary>
        /// <param name="go">GameObject to add monsters to.</param>
        /// <param name="editorObjects">Editor objects containing flats.</param>
        /// <param name="dungeonType">Dungeon type selects the encounter table.</param>
        /// <param name="monsterPower">Value between 0-1 for lowest monster power to highest.</param>
        /// ?<param name="monsterVariance">Adjust final index +/- this value in encounter table.</param>
        /// <param name="seed">Random seed for encounters.</param>
        public static void AddRandomEnemies(
            GameObject go,
            DFBlock.RdbObject[] editorObjects,
            DFRegion.DungeonTypes dungeonType,
            float monsterPower,
            int monsterVariance = 4,
            int seed = 0)
        {
            const int randomMonsterFlatIndex = 15;

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Must have import enabled and prefab set
            if (!dfUnity.Option_ImportEnemyPrefabs || dfUnity.Option_EnemyPrefab == null)
                return;

            // Editor objects array must be populated
            if (editorObjects == null || editorObjects.Length == 0)
                return;

            // Add parent node
            GameObject randomEnemiesNode = new GameObject("Random Enemies");
            randomEnemiesNode.transform.parent = go.transform;

            // Seed random generator
            UnityEngine.Random.seed = seed;

            // Iterate editor flats for enemies
            for (int i = 0; i < editorObjects.Length; i++)
            {
                // Add random enemy objects
                if (editorObjects[i].Resources.FlatResource.TextureRecord == randomMonsterFlatIndex)
                    AddRandomRDBEnemy(editorObjects[i], dungeonType, monsterPower, monsterVariance, randomEnemiesNode.transform);
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
            ref Dictionary<int, ActionLink> actionLinkDict,
            int[] textureTable,
            bool allowExitDoors,
            out List<StaticDoor> exitDoorsOut,
            ModelCombiner combiner = null,
            Transform modelsParent = null,
            Transform actionModelsParent = null)
        {
            exitDoorsOut = new List<StaticDoor>();

            // Iterate object groups
            foreach (DFBlock.RdbObjectRoot group in blockData.RdbBlock.ObjectRootList)
            {
                // Skip empty object groups
                if (null == group.RdbObjects)
                {
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

                        // Filter exit door models where flag not set
                        if (modelId == exitDoorModelID && !allowExitDoors)
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

                        // Add to static doors
                        exitDoorsOut.AddRange(GameObjectHelper.GetStaticDoors(ref modelData, blockData.Index, 0, modelMatrix));

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
                        {
                            standaloneObject = AddStandaloneModel(dfUnity, ref modelData, modelMatrix, parent, hasAction);
                            standaloneObject.GetComponent<DaggerfallMesh>().SetDungeonTextures(textureTable);
                        }
                        else
                        {
                            combiner.Add(ref modelData, modelMatrix);
                        }

                        // Add action
                        if (hasAction && standaloneObject != null)
                            AddActionModelHelper(standaloneObject, ref actionLinkDict, obj, blockData);
                    }
                }
            }

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
        /// Constructs a Vector3 from magnitude and direction in RDB action resource.
        /// </summary>
        private static void GetRotationActionVector(ref DaggerfallAction action, DFBlock.RdbActionAxes axis)
        {
            Vector3 vector = Vector3.zero;
            float magnitude = action.Magnitude;
            switch (axis)
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

            action.ActionRotation = vector / BlocksFile.RotationDivisor;
        }

        /// <summary>
        /// Constructs a Vector3 from magnitude and direction in RDB action resource.
        /// </summary>
        private static void GetTranslationActionVector(ref DaggerfallAction action, DFBlock.RdbActionAxes axis)
        {
            Vector3 vector = Vector3.zero;
            float magnitude = action.Magnitude;
            switch (axis)
            {
                case DFBlock.RdbActionAxes.NegativeX:
                    vector.x = magnitude;
                    break;
                case DFBlock.RdbActionAxes.NegativeY:
                    vector.y = -magnitude;
                    break;
                case DFBlock.RdbActionAxes.NegativeZ:
                    vector.z = magnitude;
                    break;

                case DFBlock.RdbActionAxes.PositiveX:
                    vector.x = -magnitude;
                    break;
                case DFBlock.RdbActionAxes.PositiveY:
                    vector.y = magnitude;
                    break;
                case DFBlock.RdbActionAxes.PositiveZ:
                    vector.z = -magnitude;
                    break;
                default:
                    magnitude = 0f;
                    break;
            }
            action.ActionTranslation = vector * MeshReader.GlobalScale;
        }



        private static void AddActionModelHelper(
            GameObject go,
            ref Dictionary<int, ActionLink> actionLinkDict,
            DFBlock.RdbObject rdbObj,
            DFBlock blockData)
        {

            DFBlock.RdbModelResource obj = rdbObj.Resources.ModelResource;
            string description = blockData.RdbBlock.ModelReferenceList[obj.ModelIndex].Description;
            int soundID_Index = obj.SoundIndex;
            float duration = obj.ActionResource.Duration;
            float magnitude = obj.ActionResource.Magnitude;
            int axis = obj.ActionResource.Axis;
            DFBlock.RdbTriggerFlags triggerFlag = DFBlock.RdbTriggerFlags.None;
            DFBlock.RdbActionFlags actionFlag = DFBlock.RdbActionFlags.None;

            //set action flag if valid / known
            if (Enum.IsDefined(typeof(DFBlock.RdbActionFlags), (DFBlock.RdbActionFlags)obj.ActionResource.Flags))
                actionFlag = (DFBlock.RdbActionFlags)obj.ActionResource.Flags;

            //set trigger flag if valid / known
            if (Enum.IsDefined(typeof(DFBlock.RdbTriggerFlags), (DFBlock.RdbTriggerFlags)obj.TriggerFlag_StartingLock))
                triggerFlag = (DFBlock.RdbTriggerFlags)obj.TriggerFlag_StartingLock;

            //add action node to actionLink dictionary
            if (!actionLinkDict.ContainsKey(rdbObj.This))
            {
                ActionLink link;
                link.nextKey = obj.ActionResource.NextObjectOffset;
                link.prevKey = obj.ActionResource.PreviousObjectOffset;
                link.gameObject = go;
                actionLinkDict.Add(rdbObj.This, link);
            }

            // Create unique LoadID for save sytem
            long loadID = (blockData.Index << 24) + rdbObj.This;

            AddAction(go, description, soundID_Index, duration, magnitude, axis, triggerFlag, actionFlag, loadID);
        }

        private static void AddActionFlatHelper(
            GameObject go,
            ref Dictionary<int, ActionLink> actionLinkDict,
            DFBlock blockData,
            DFBlock.RdbObject rdbObj)
        {

            DFBlock.RdbFlatResource obj = rdbObj.Resources.FlatResource;
            string description = "FLT";
            int soundID_Index = obj.Sound_index;
            float duration = 0.0f;
            float magnitude = obj.Magnitude;
            int axis = obj.Magnitude;
            DFBlock.RdbTriggerFlags triggerFlag = DFBlock.RdbTriggerFlags.None;
            DFBlock.RdbActionFlags actionFlag = DFBlock.RdbActionFlags.None;

            //set action flag if valid / known
            if (Enum.IsDefined(typeof(DFBlock.RdbActionFlags), (DFBlock.RdbActionFlags)obj.Action))
                actionFlag = (DFBlock.RdbActionFlags)obj.Action;

            //set trigger flag if valid / known
            if (Enum.IsDefined(typeof(DFBlock.RdbTriggerFlags), (DFBlock.RdbTriggerFlags)obj.TriggerFlag))
                triggerFlag = (DFBlock.RdbTriggerFlags)obj.TriggerFlag;

            //add action node to actionLink dictionary
            if (!actionLinkDict.ContainsKey(rdbObj.This))
            {
                ActionLink link;
                link.nextKey = obj.NextObjectOffset;
                link.prevKey = -1;
                link.gameObject = go;
                actionLinkDict.Add(rdbObj.This, link);
            }

            // Create unique LoadID for save sytem
            long loadID = (blockData.Index << 24) + rdbObj.This;

            AddAction(go, description, soundID_Index, duration, magnitude, axis, triggerFlag, actionFlag, loadID);
        }


        private static void AddAction(
            GameObject go,
            string description,
            int soundID_and_index,
            float duration,
            float magnitude,
            int axis_raw,
            DFBlock.RdbTriggerFlags triggerFlag,
            DFBlock.RdbActionFlags actionFlag,
            long loadID = 0
            )
        {
            DaggerfallAction action = go.AddComponent<DaggerfallAction>();
            action.ModelDescription = description;
            action.ActionDuration = duration;
            action.Magnitude = magnitude;
            action.Index = soundID_and_index;
            action.TriggerFlag = triggerFlag;
            action.ActionFlag = actionFlag;
            action.LoadID = loadID;

            // If SaveLoadManager present in game then attach SerializableActionObject
            if (SaveLoadManager.Instance != null)
            {
                go.AddComponent<SerializableActionObject>();
            }

            //if a collision type action or action flat, add DaggerFallActionCollision component
            if (action.TriggerFlag == DFBlock.RdbTriggerFlags.Collision01 || action.TriggerFlag == DFBlock.RdbTriggerFlags.Collision03 ||
                action.TriggerFlag == DFBlock.RdbTriggerFlags.DualTrigger || action.TriggerFlag == DFBlock.RdbTriggerFlags.Collision09)
            {
                DaggerfallActionCollision collision = go.AddComponent<DaggerfallActionCollision>();
                collision.isFlat = false;
            }
            else if (description == "FLT")
            {
                DaggerfallActionCollision collision = go.AddComponent<DaggerfallActionCollision>();
                collision.isFlat = true;
            }


            switch (action.ActionFlag)
            {
                case DFBlock.RdbActionFlags.Translation:
                    {
                        action.Magnitude = magnitude;
                        GetTranslationActionVector(ref action, (DFBlock.RdbActionAxes)axis_raw);
                    }
                    break;

                case DFBlock.RdbActionFlags.Rotation:
                    {
                        action.Magnitude = magnitude;
                        GetRotationActionVector(ref action, (DFBlock.RdbActionAxes)axis_raw);
                    }
                    break;
                case DFBlock.RdbActionFlags.PositiveX:
                    {
                        action.ActionDuration = 50;
                        action.Magnitude = axis_raw * 8;
                        GetTranslationActionVector(ref action, DFBlock.RdbActionAxes.PositiveX);
                    }
                    break;
                case DFBlock.RdbActionFlags.NegativeX:
                    {
                        action.ActionDuration = 50;
                        action.Magnitude = axis_raw * 8;
                        GetTranslationActionVector(ref action, DFBlock.RdbActionAxes.NegativeX);
                    }
                    break;
                case DFBlock.RdbActionFlags.PositiveY:
                    {
                        action.ActionDuration = 50;
                        action.Magnitude = axis_raw * 8;
                        GetTranslationActionVector(ref action, DFBlock.RdbActionAxes.PositiveY);
                    }
                    break;
                case DFBlock.RdbActionFlags.NegativeY:
                    {
                        action.ActionDuration = 50;
                        action.Magnitude = axis_raw * 8;
                        GetTranslationActionVector(ref action, DFBlock.RdbActionAxes.NegativeY);

                    }
                    break;
                case DFBlock.RdbActionFlags.PositiveZ:
                    {
                        action.ActionDuration = 50;
                        action.Magnitude = axis_raw * 8;
                        GetTranslationActionVector(ref action, DFBlock.RdbActionAxes.PositiveZ);
                    }
                    break;
                case DFBlock.RdbActionFlags.NegativeZ:
                    {
                        action.ActionDuration = 50;
                        action.Magnitude = axis_raw * 8;
                        GetTranslationActionVector(ref action, DFBlock.RdbActionAxes.NegativeZ);
                    }
                    break;
                default:
                    {
                        //Dmg actions use axis value to modifiy tot. dmg.
                        action.Magnitude = axis_raw;
                    }
                    break;
            }

            // A quick hack to fix special-case rotation issues.
            // Currently unknown if there is data indicating different rotation behaviour or if something else is happening.
            switch (description)
            {
                case "LID":
                    action.ActionRotation = new Vector3(0, 0, -90f);       // Coffin lids (e.g. Scourg barrow)
                    break;
                case "WHE":
                    action.ActionRotation = new Vector3(0, -360f, 0);      // Wheels (e.g. Direnni Tower)
                    break;
            }

            //Add audio
            AddActionAudioSource(go, (uint)action.Index);
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
        public static void LinkActionNodes(Dictionary<int, ActionLink> actionLinkDict)
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
        private static GameObject AddActionDoor(DaggerfallUnity dfUnity, uint modelId, DFBlock.RdbObject obj, Transform parent, long loadID = 0)
        {
            if (dfUnity.Option_DungeonDoorPrefab == null)
                return null;

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

            // Get action door script
            DaggerfallActionDoor actionDoor = go.GetComponent<DaggerfallActionDoor>();

            // Set starting lock value
            if (obj.Resources.ModelResource.TriggerFlag_StartingLock >= 16)
            {
                actionDoor.StartingLockValue = (int)obj.Resources.ModelResource.TriggerFlag_StartingLock;
            }

            // Set LoadID
            actionDoor.LoadID = loadID;

            return go;
        }

        private static GameObject AddLight(DaggerfallUnity dfUnity, DFBlock.RdbObject obj, Transform parent)
        {
            // Spawn light gameobject
            float range = obj.Resources.LightResource.Radius * MeshReader.GlobalScale;
            Vector3 position = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
            GameObject go = GameObjectHelper.InstantiatePrefab(dfUnity.Option_DungeonLightPrefab.gameObject, string.Empty, parent, position);
            Light light = go.GetComponent<Light>();
            if (light != null)
            {
                light.range = range * 3;
            }

            return go;
        }

        private static GameObject AddFlat(DFBlock.RdbObject obj, Transform parent)
        {
            int archive = obj.Resources.FlatResource.TextureArchive;
            int record = obj.Resources.FlatResource.TextureRecord;

            // Spawn billboard gameobject
            GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(archive, record, parent, true);
            Vector3 billboardPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;

            // Add RDB data to billboard
            DaggerfallBillboard dfBillboard = go.GetComponent<DaggerfallBillboard>();
            dfBillboard.SetResourceData(obj.Resources.FlatResource);

            // Set transform
            go.transform.position = billboardPosition;

            // Disable enemy flats
            if (archive == TextureReader.EditorFlatsTextureArchive && (record == 15 || record == 16))
                go.SetActive(false);

            // Add torch burning sound
            if (archive == TextureReader.LightsTextureArchive)
            {
                switch (record)
                {
                    case 0:
                    case 1:
                    case 6:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                        AddTorchAudioSource(go);
                        break;
                }
            }

            return go;
        }

        private static void AddTorchAudioSource(GameObject go)
        {
            // Apply looping burning sound to flaming torches and fires
            // Set to linear rolloff or the burning sound is audible almost everywhere
            DaggerfallAudioSource c = go.AddComponent<DaggerfallAudioSource>();
            c.AudioSource.dopplerLevel = 0;
            c.AudioSource.rolloffMode = AudioRolloffMode.Linear;
            c.AudioSource.maxDistance = torchMaxDistance;
            c.AudioSource.volume = torchVolume;
            c.SetSound(SoundClips.Burning, AudioPresets.LoopIfPlayerNear);
        }

        private static void AddRandomRDBEnemy(
            DFBlock.RdbObject obj,
            DFRegion.DungeonTypes dungeonType,
            float monsterPower,
            int monsterVariance,
            Transform parent)
        {
            // Must have a dungeon type
            if (dungeonType == DFRegion.DungeonTypes.NoDungeon)
                return;

            // Get dungeon type index
            int dungeonIndex = (int)dungeonType >> 8;
            if (dungeonIndex < RandomEncounters.EncounterTables.Length)
            {
                // Get encounter table
                RandomEncounterTable table = RandomEncounters.EncounterTables[dungeonIndex];

                // Get base monster index into table
                int baseMonsterIndex = (int)((float)table.Enemies.Length * monsterPower);

                // Set min index
                int minMonsterIndex = baseMonsterIndex - monsterVariance;
                if (minMonsterIndex < 0)
                    minMonsterIndex = 0;

                // Set max index
                int maxMonsterIndex = baseMonsterIndex + monsterVariance;
                if (maxMonsterIndex >= table.Enemies.Length)
                    maxMonsterIndex = table.Enemies.Length;

                // Get random monster from table
                MobileTypes type = table.Enemies[UnityEngine.Random.Range(minMonsterIndex, maxMonsterIndex)];

                // Add enemy
                AddEnemy(obj, type, parent);
            }
            else
            {
                DaggerfallUnity.LogMessage(string.Format("RDBLayout: Dungeon type {0} is out of range or unknown.", dungeonType), true);
            }
        }

        private static void AddFixedRDBEnemy(DFBlock.RdbObject obj, Transform parent)
        {
            // Get type value and ignore known invalid types
            int typeValue = (int)(obj.Resources.FlatResource.FactionMobileId & 0xff);
            if (typeValue == 99)
                return;

            // Cast to enum
            MobileTypes type = (MobileTypes)(obj.Resources.FlatResource.FactionMobileId & 0xff);

            AddEnemy(obj, type, parent);
        }

        private static void AddEnemy(
            DFBlock.RdbObject obj,
            MobileTypes type,
            Transform parent = null,
            DFRegion.DungeonTypes dungeonType = DFRegion.DungeonTypes.HumanStronghold)
        {
            // Get default reaction
            MobileReactions reaction = MobileReactions.Hostile;
            if (obj.Resources.FlatResource.Action == (int)DFBlock.EnemyReactionTypes.Passive)
                reaction = MobileReactions.Passive;

            // Just setup demo enemies at this time
            string name = string.Format("DaggerfallEnemy [{0}]", type.ToString());
            Vector3 position = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
            GameObject go = GameObjectHelper.InstantiatePrefab(DaggerfallUnity.Instance.Option_EnemyPrefab.gameObject, name, parent, position);
            SetupDemoEnemy setupEnemy = go.GetComponent<SetupDemoEnemy>();
            if (setupEnemy != null)
            {
                // Configure enemy
                setupEnemy.ApplyEnemySettings(type, reaction);

                // Align non-flying units with ground
                DaggerfallMobileUnit mobileUnit = setupEnemy.GetMobileBillboardChild();
                if (mobileUnit.Summary.Enemy.Behaviour != MobileBehaviour.Flying)
                    GameObjectHelper.AlignControllerToGround(go.GetComponent<CharacterController>());
            }
        }

        #endregion
    }
}