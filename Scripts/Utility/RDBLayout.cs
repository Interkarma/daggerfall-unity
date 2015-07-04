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
using DaggerfallWorkshop.Demo;

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
        /// <param name="textureTable">Optional texture table for dungeon.</param>
        /// <param name="allowExitDoors">Add exit doors to block.</param>
        /// <param name="cloneFrom">Clone and build on a prefab object template.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(
            string blockName,
            int[] textureTable = null,
            bool allowExitDoors = true,
            DaggerfallRDBBlock cloneFrom = null)
        {
            DFBlock blockData;
            return CreateBaseGameObject(blockName, out blockData, textureTable, allowExitDoors, cloneFrom);
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

            return CreateBaseGameObject(ref blockDataOut, textureTable, allowExitDoors, cloneFrom);
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
        public static void AddActionDoors(GameObject go, ref DFBlock blockData, int[] textureTable)
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
                        // Look for action doors
                        int modelReference = obj.Resources.ModelResource.ModelIndex;
                        uint modelId = blockData.RdbBlock.ModelReferenceList[modelReference].ModelIdNum;
                        if (IsActionDoor(ref blockData, obj, modelReference))
                        {
                            GameObject cgo = AddActionDoor(dfUnity, modelId, obj, actionDoorsNode.transform);
                            cgo.GetComponent<DaggerfallMesh>().SetDungeonTextures(textureTable);
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
            ref DFBlock blockData,
            out DFBlock.RdbObject[] editorObjectsOut,
            out GameObject[] startMarkersOut)
        {
            List<DFBlock.RdbObject> editorObjects = new List<DFBlock.RdbObject>();
            List<GameObject> startMarkers = new List<GameObject>();
            editorObjectsOut = null;
            startMarkersOut = null;

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

                // Look for flat in this group
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
                        }
                    }
                }
            }

            // Output editor objects
            editorObjectsOut = editorObjects.ToArray();
            startMarkersOut = startMarkers.ToArray();
        }

        /// <summary>
        /// Add all enemies.
        /// Dungeon type can be provided to simulate random encounters.
        /// If dungeon type not given then random enemies will be omitted.
        /// TODO: Split out fixed and random enemies so developer has more control over spawns.
        /// </summary>
        public static void AddEnemies(
            GameObject go,
            DFBlock.RdbObject[] editorObjects,
            DFRegion.DungeonTypes dungeonType = DFRegion.DungeonTypes.HumanStronghold)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Must have import enabled and prefab set
            if (!dfUnity.Option_ImportEnemyPrefabs || dfUnity.Option_EnemyPrefab == null)
                return;

            // Editor objects array must be populated
            if (editorObjects == null)
                return;
            if (editorObjects.Length == 0)
                return;

            // Add parent node
            GameObject enemiesNode = new GameObject("Enemies");
            enemiesNode.transform.parent = go.transform;

            // Iterate editor flats for enemies
            for (int i = 0; i < editorObjects.Length; i++)
            {
                // Add enemy objects
                int record = editorObjects[i].Resources.FlatResource.TextureRecord;
                switch (record)
                {
                    case 15:                        // Random enemy
                        AddRandomRDBEnemy(editorObjects[i], dungeonType, enemiesNode.transform);
                        break;
                    case 16:                        // Fixed enemy
                        AddFixedRDBEnemy(editorObjects[i], enemiesNode.transform);
                        break;
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
            int[] textureTable,
            bool allowExitDoors,
            out List<StaticDoor> exitDoorsOut,
            ModelCombiner combiner = null,
            Transform modelsParent = null,
            Transform actionModelsParent = null)
        {
            exitDoorsOut = new List<StaticDoor>();

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
        private static GameObject AddActionDoor(DaggerfallUnity dfUnity, uint modelId, DFBlock.RdbObject obj, Transform parent)
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
            Transform parent)
        {
            // Must have a dungeon type
            if (dungeonType == DFRegion.DungeonTypes.NoDungeon)
                return;

            // Get dungeon type index
            int index = (int)dungeonType >> 8;
            if (index < RandomEncounters.EncounterTables.Length)
            {
                // Get encounter table
                RandomEncounterTable table = RandomEncounters.EncounterTables[index];

                // Get random monster from table
                // Normally this would be weighted by player level
                MobileTypes type = table.Enemies[UnityEngine.Random.Range(0, table.Enemies.Length)];

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
            if (obj.Resources.FlatResource.FlatData.Reaction == (int)DFBlock.EnemyReactionTypes.Passive)
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