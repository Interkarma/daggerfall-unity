// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Items;

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
        /// <param name="serialize">Allow for serialization of supported sub-objects.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(
            string blockName,
            Dictionary<int, ActionLink> actionLinkDict,
            int[] textureTable = null,
            bool allowExitDoors = true,
            DaggerfallRDBBlock cloneFrom = null,
            bool serialize = true)
        {
            DFBlock blockData;
            return CreateBaseGameObject(blockName, actionLinkDict, out blockData, textureTable, allowExitDoors, cloneFrom, serialize);
        }

        /// <summary>
        /// Create base RDB block by name and get back DFBlock data.
        /// </summary>
        /// <param name="blockName">Name of block.</param>
        /// <param name="blockDataOut">DFBlock data out.</param>
        /// <param name="textureTable">Optional texture table for dungeon.</param>
        /// <param name="allowExitDoors">Add exit doors to block.</param>
        /// <param name="cloneFrom">Clone and build on a prefab object template.</param>
        /// <param name="serialize">Allow for serialization of supported sub-objects.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(
            string blockName,
            Dictionary<int, ActionLink> actionLinkDict,
            out DFBlock blockDataOut,
            int[] textureTable = null,
            bool allowExitDoors = true,
            DaggerfallRDBBlock cloneFrom = null,
            bool serialize = true,
            bool isAutomapRun = false)
        {
            blockDataOut = new DFBlock();

            // Validate
            if (string.IsNullOrEmpty(blockName))
                return null;
            if (!blockName.EndsWith(".RDB", StringComparison.InvariantCultureIgnoreCase))
                return null;
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            // Get block data
            blockDataOut = dfUnity.ContentReader.BlockFileReader.GetBlock(blockName);

            return CreateBaseGameObject(ref blockDataOut, actionLinkDict, textureTable, allowExitDoors, cloneFrom, serialize, isAutomapRun);
        }

        /// <summary>
        /// Instantiate base RDB block by DFBlock data.
        /// </summary>
        /// <param name="blockData">Block data.</param>
        /// <param name="textureTable">Optional texture table for dungeon.</param>
        /// <param name="allowExitDoors">Add exit doors to block.</param>
        /// <param name="cloneFrom">Clone and build on a prefab object template.</param>
        /// <param name="serialize">Allow for serialization of supported sub-objects.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(
            ref DFBlock blockData,
            Dictionary<int, ActionLink> actionLinkDict,
            int[] textureTable = null,
            bool allowExitDoors = true,
            DaggerfallRDBBlock cloneFrom = null,
            bool serialize = true,
            bool isAutomapRun = false)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            // Use default texture table if one not specified
            if (textureTable == null)
                textureTable = DungeonTextureTables.DefaultTextureTable;

            // Create gameobject
            GameObject daggerfallBlockGO;
            string name = string.Format("DaggerfallBlock [{0}]", blockData.Name);
            if (cloneFrom != null)
            {
                daggerfallBlockGO = GameObjectHelper.InstantiatePrefab(cloneFrom.gameObject, name, null, Vector3.zero);
            }
            else
            {
                daggerfallBlockGO = new GameObject(name);
                daggerfallBlockGO.AddComponent<DaggerfallRDBBlock>();
            }

            // Setup combiner
            ModelCombiner combiner = null;
            if (dfUnity.Option_CombineRDB)
                combiner = new ModelCombiner();

            // Add parent node
            GameObject modelsNode = new GameObject("Models");
            GameObject actionModelsNode = new GameObject("Action Models");
            modelsNode.transform.parent = daggerfallBlockGO.transform;
            actionModelsNode.transform.parent = daggerfallBlockGO.transform;

            // Add models
            List<StaticDoor> exitDoors;
            AddModels(
                dfUnity,
                ref blockData,
                actionLinkDict,
                textureTable,
                allowExitDoors,
                out exitDoors,
                serialize,
                combiner,
                modelsNode.transform,
                actionModelsNode.transform,
                isAutomapRun);

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
                DaggerfallStaticDoors c = daggerfallBlockGO.AddComponent<DaggerfallStaticDoors>();
                c.Doors = exitDoors.ToArray();
            }

            return daggerfallBlockGO;
        }

        /// <summary>
        /// Add actions doors to block.
        /// </summary>
        public static void AddActionDoors(GameObject go, Dictionary<int, ActionLink> actionLinkDict, ref DFBlock blockData, int[] textureTable, bool serialize = true)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Use default texture table if one not specified
            if (textureTable == null)
                textureTable = DungeonTextureTables.DefaultTextureTable;

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
                        ulong loadID = 0;
                        if (serialize)
                            loadID = (ulong)(blockData.Position + obj.Position);

                        // Look for action doors
                        int modelReference = obj.Resources.ModelResource.ModelIndex;
                        uint modelId = blockData.RdbBlock.ModelReferenceList[modelReference].ModelIdNum;
                        if (IsActionDoor(ref blockData, obj, modelReference))
                        {
                            GameObject cgo = AddActionDoor(dfUnity, modelId, obj, actionDoorsNode.transform, loadID);

                            var dfMesh = cgo.GetComponent<DaggerfallMesh>();
                            if (dfMesh)
                                dfMesh.SetDungeonTextures(textureTable);

                            // Add action component to door if it also has an action
                            if (HasAction(obj))
                            {
                                AddActionModelHelper(cgo, actionLinkDict, obj, ref blockData, serialize);
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
            Dictionary<int, ActionLink> actionLinkDict,
            ref DFBlock blockData,
            out DFBlock.RdbObject[] editorObjectsOut,
            out GameObject[] startMarkersOut,
            out GameObject[] enterMarkersOut,
            DFRegion.DungeonTypes dungeonType)
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
                        
                        // Add animal sound
                        // This was specifically added to accommodate the cat in
                        // Direnni Tower. This should also enable animals added
                        // to dungeons by mods to have proper sounds too.
                        if (archive == TextureReader.AnimalsTextureArchive)
                            GameObjectHelper.AddAnimalAudioSource(flatObject, record);

                        if (archive == TextureReader.EditorFlatsTextureArchive)
                        {
                            editorObjects.Add(obj);
                            if (record == 10)
                                startMarkers.Add(flatObject);
                            else if (record == 8)
                                enterMarkers.Add(flatObject);

                            // Add special marker component for quest and item markers
                            if (record == 11 || record == 18)
                            {
                                ulong markerID = (ulong)(blockData.Position + obj.Position);
                                DaggerfallMarker marker = flatObject.AddComponent<DaggerfallMarker>();
                                marker.MarkerID = markerID;
                            }

                            //add editor flats to actionLinkDict
                            if (!actionLinkDict.ContainsKey(obj.Position))
                            {
                                ActionLink link;
                                link.gameObject = flatObject;
                                link.nextKey = obj.Resources.FlatResource.NextObjectOffset;
                                link.prevKey = -1;
                                actionLinkDict.Add(obj.Position, link);
                            }
                        }

                        // Disable MeshRenderer is this is a fixed treasure flat
                        // Will be restored in-place by AddFixedTreasure()
                        if (archive == TextureReader.FixedTreasureFlatsArchive)
                        {
                            // Disable original marker appearance
                            MeshRenderer meshRenderer = flatObject.GetComponent<MeshRenderer>();
                            if (meshRenderer)
                                meshRenderer.enabled = false;

                            // Assign fixed treaure to this marker
                            AssignFixedTreasure(flatObject, obj, ref blockData, dungeonType);
                        }

                        // Parent random treasure to marker to use actions on parent
                        const int randomTreasureFlatIndex = 19;
                        if (archive == TextureReader.EditorFlatsTextureArchive &&
                            record == randomTreasureFlatIndex &&
                            dfUnity.Option_ImportRandomTreasure &&
                            dfUnity.Option_LootContainerPrefab)
                        {
                            AddRandomTreasure(obj, flatObject.transform, ref blockData, dungeonType, true);
                        }

                        //add action component to flat if it has an action
                        if (obj.Resources.FlatResource.Action > 0)
                        {
                            AddActionFlatHelper(flatObject, actionLinkDict, ref blockData, obj);
                        }
                    }
                }
            }

            // Output editor objects
            editorObjectsOut = editorObjects.ToArray();
            startMarkersOut = startMarkers.ToArray();
            enterMarkersOut = enterMarkers.ToArray();
        }

        public static void AssignFixedTreasure(
            GameObject parent,
            DFBlock.RdbObject obj,
            ref DFBlock blockData,
            DFRegion.DungeonTypes dungeonType,
            bool serialize = true)
        {
            // Add fixed treasure flat with same archive & record and use exact position
            int archive = obj.Resources.FlatResource.TextureArchive;
            int record = obj.Resources.FlatResource.TextureRecord;
            AddRandomTreasure(obj, parent.transform, ref blockData, dungeonType, serialize, archive, record, false);
        }

        /// <summary>
        /// Add fixed enemies.
        /// </summary>
        /// <param name="go">GameObject to add monsters to.</param>
        /// <param name="editorObjects">Editor objects containing flats.</param>
        /// <param name="serialize">Allow for serialization when available.</param>
        public static void AddFixedEnemies(
            GameObject go,
            DFBlock.RdbObject[] editorObjects,
            ref DFBlock blockData,
            GameObject[] startMarkers,
            bool serialize = true)
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
                    AddFixedRDBEnemy(editorObjects[i], fixedEnemiesNode.transform, ref blockData, startMarkers, serialize);
            }
        }

        /// <summary>
        /// Add random enemies from encounter tables based on dungeon type, monster power, and seed.
        /// </summary>
        /// <param name="go">GameObject to add monsters to.</param>
        /// <param name="editorObjects">Editor objects containing flats.</param>
        /// <param name="dungeonType">Dungeon type selects the encounter table.</param>
        /// <param name="monsterPower">Value between 0-1 for lowest monster power to highest.</param>
        /// <param name="monsterVariance">Adjust final index +/- this value in encounter table.</param>
        /// <param name="seed">Random seed for encounters.</param>
        /// <param name="serialize">Allow for serialization when available.</param>
        public static void AddRandomEnemies(
            GameObject go,
            DFBlock.RdbObject[] editorObjects,
            DFRegion.DungeonTypes dungeonType,
            float monsterPower,
            ref DFBlock blockData,
            GameObject[] startMarkers,
            int monsterVariance = 4,
            int seed = 0,
            bool serialize = true)
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
            UnityEngine.Random.InitState(seed);

            bool alternateRandomEnemySelection = DaggerfallUnity.Settings.AlternateRandomEnemySelection;

            if (!alternateRandomEnemySelection) // Classic enemy selection
            {
                // Set up enemy lists used by classic
                DFRandom.srand(GameManager.Instance.PlayerGPS.CurrentLocation.Dungeon.RecordElement.Header.LocationId);
                MobileTypes[] DungeonWaterEnemiesToPlace = new MobileTypes[256];
                MobileTypes[] DungeonNonWaterEnemiesToPlace = new MobileTypes[256];

                for (int i = 0; i < 256; ++i)
                    DungeonNonWaterEnemiesToPlace[i] = ChooseRandomEnemyType(RandomEncounters.EncounterTables[(int)dungeonType]);
                for (int i = 0; i < 256; ++i)
                    DungeonWaterEnemiesToPlace[i] = ChooseRandomEnemyType(RandomEncounters.EncounterTables[19]);

                // Iterate editor flats for enemies
                for (int i = 0; i < editorObjects.Length; i++)
                {
                    // Add random enemy objects
                    if (editorObjects[i].Resources.FlatResource.TextureRecord == randomMonsterFlatIndex)
                        AddRandomRDBEnemyClassic(editorObjects[i], dungeonType, monsterPower, monsterVariance, randomEnemiesNode.transform, ref blockData, startMarkers, serialize, DungeonWaterEnemiesToPlace, DungeonNonWaterEnemiesToPlace);
                }
            }
            else // Alternate enemy selection (more randomized)
            {
                // Iterate editor flats for enemies
                for (int i = 0; i < editorObjects.Length; i++)
                {
                    // Add random enemy objects
                    if (editorObjects[i].Resources.FlatResource.TextureRecord == randomMonsterFlatIndex)
                        AddRandomRDBEnemy(editorObjects[i], dungeonType, monsterPower, monsterVariance, randomEnemiesNode.transform, ref blockData, startMarkers, serialize);
                }
            }
        }

        /// <summary>
        /// Add dungeon water plane prefab to RDB block post layout.
        /// This is called from DaggerfallDungeon layout as water level is available from map data, not RDB data.
        /// </summary>
        /// <param name="go">Parent block GameObject.</param>
        /// <param name="nativeBlockWaterLevel">Native water level from map data.</param>
        public static void AddWater(
            GameObject parent,
            Vector3 position,
            short nativeBlockWaterLevel)
        {
            // Exit if no water present or prefab not set
            if (nativeBlockWaterLevel == 10000 || DaggerfallUnity.Instance.Option_DungeonWaterPrefab.gameObject == null)
                return;

            // Instantiate water prefab to block parent
            GameObject go = GameObjectHelper.InstantiatePrefab(DaggerfallUnity.Instance.Option_DungeonWaterPrefab.gameObject, "DungeonWater", parent.transform, position);

            // Scale water plane to RDB dimensions
            Vector3 prefabScale = DaggerfallUnity.Instance.Option_DungeonWaterPlaneSize;
            go.transform.localScale = new Vector3(
                1f / prefabScale.x * RDBSide,
                1f,
                1f / prefabScale.z * RDBSide);

            // Align water plane to RDB origin
            Vector3 prefabOffset = DaggerfallUnity.Instance.Option_DungeonWaterPlaneOffset;
            go.transform.localPosition += new Vector3(
                prefabOffset.x * (1f / prefabScale.x) * RDBSide,
                nativeBlockWaterLevel * -1 * MeshReader.GlobalScale,
                prefabOffset.z * (1f / prefabScale.x) * RDBSide);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds models and their actions to scene.
        /// </summary>
        private static void AddModels(
            DaggerfallUnity dfUnity,
            ref DFBlock blockData,
            Dictionary<int, ActionLink> actionLinkDict,
            int[] textureTable,
            bool allowExitDoors,
            out List<StaticDoor> exitDoorsOut,
            bool serialize,
            ModelCombiner combiner = null,
            Transform modelsParent = null,
            Transform actionModelsParent = null,
            bool isAutomapRun = false)
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
                        if (dfUnity.MeshReader.GetModelData(modelId, out modelData))
                        {
                            // Add to static doors
                            exitDoorsOut.AddRange(GameObjectHelper.GetStaticDoors(ref modelData, blockData.Index, 0, modelMatrix));
                        }

                        // Check if model has an action record
                        bool hasAction = HasAction(obj);

                        // Get GameObject
                        Transform parent = (hasAction) ? actionModelsParent : modelsParent;
                        GameObject standaloneObject = MeshReplacement.ImportCustomGameobject(modelId, parent, modelMatrix, isAutomapRun);
                        if (standaloneObject == null)
                        {
                            // Special handling for dungeon exits - collider handled as a special case in DaggerfallStaticDoors startup
                            if (modelId == exitDoorModelID)
                            {
                                AddStandaloneModel(dfUnity, ref modelData, modelMatrix, modelsParent, isAutomapRun, hasAction, true);
                                continue;
                            }

                            // Special handling for tapestries and banners
                            // Some of these are so far out from wall player can become stuck behind them
                            // Adding model invidually without collider to avoid problem
                            // Not sure if these object ever actions, but bypass this hack if they do
                            if (modelId >= minTapestryID && modelId <= maxTapestryID && !hasAction)
                            {
                                AddStandaloneModel(dfUnity, ref modelData, modelMatrix, modelsParent, isAutomapRun, hasAction, true);
                                continue;
                            }

                            // Add or combine
                            if (combiner == null || hasAction || PlayerActivate.HasCustomActivation(modelId))
                            {
                                standaloneObject = AddStandaloneModel(dfUnity, ref modelData, modelMatrix, parent, isAutomapRun, hasAction);
                                standaloneObject.GetComponent<DaggerfallMesh>().SetDungeonTextures(textureTable);
                            }
                            else
                            {
                                combiner.Add(ref modelData, modelMatrix);
                            }
                        }
                        else if (blockData.RdbBlock.ModelReferenceList[modelReference].Description == "COL")
                        {
                            // Add colliders to custom dungeon models added using "COL" in the World Data Editor.
                            MeshCollider collider = standaloneObject.GetComponent<MeshCollider>();
                            if (collider == null) collider = standaloneObject.AddComponent<MeshCollider>();
                            MeshFilter meshFilter = standaloneObject.GetComponent<MeshFilter>();
                            collider.sharedMesh = meshFilter.sharedMesh;
                        }

                        // Add action
                        if (hasAction && standaloneObject != null)
                            AddActionModelHelper(standaloneObject, actionLinkDict, obj, ref blockData, serialize);
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
            bool isAutomapRun = false,
            bool overrideStatic = false,
            bool ignoreCollider = false,
            bool convexCollider = false)
        {
            // Determine static flag
            bool makeStatic = (dfUnity.Option_SetStaticFlags && !overrideStatic) ? true : false;

            // Add GameObject
            uint modelID = (uint)modelData.DFMesh.ObjectId;
            GameObject modelGO = GameObjectHelper.CreateDaggerfallMeshGameObject(modelID, parent, makeStatic, null, ignoreCollider, convexCollider);
            modelGO.transform.position = matrix.GetColumn(3);
            modelGO.transform.rotation = matrix.rotation;
            modelGO.transform.localScale = matrix.lossyScale;

            if (isAutomapRun)
                modelGO.AddComponent<AutomapModel>();

            return modelGO;
        }

        /// <summary>
        /// Check if model is a hinged action door.
        /// </summary>
        private static bool IsActionDoor(ref DFBlock blockData, DFBlock.RdbObject obj, int modelReference)
        {
            // Always reject red brick doors, they are not action doors despite having "DOR" attached
            if (blockData.RdbBlock.ModelReferenceList[modelReference].ModelIdNum == redBrickDoorModelID)
                return false;

            // Otherwise Check if this is a door (DOR) or double-door (DDR) or has a NEW tag
            //models 55007 550245 5018 have the NEW tag and always seem to be doors
            // CAV appears to be cave-wall doors, like 55033 in S0000204.RDB.
            string description = blockData.RdbBlock.ModelReferenceList[modelReference].Description;
            if (description == "DOR" || description == "DDR" || description == "NEW" || description == "CAV")
                return true;

            return false;
        }

        /// <summary>
        /// Check is model has action record.
        /// </summary>
        private static bool HasAction(DFBlock.RdbObject obj)
        {
            DFBlock.RdbActionResource action = obj.Resources.ModelResource.ActionResource;
            return (action.Flags != 0);
        }

        /// <summary>
        /// Constructs a Vector3 from magnitude and direction in RDB action resource.
        /// </summary>
        private static void GetRotationActionVector(ref DaggerfallAction action, DFBlock.RdbActionAxes axis)
        {
            // HACK: Workaround for a specific rotation case where TRP object has a raw axis value > 6
            // If more examples with a raw axis value > 6 can be found, there's possibly some global bitwise op needed here instead
            if (action.ActionAxisRawValue == 13 && action.ModelDescription == "TRP")
            {
                axis = DFBlock.RdbActionAxes.NegativeX;
                action.Magnitude = 400; // Classic magnitude is 392 but player is able to stick to that angle so increasing to 400
            }

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
                    break;
            }
            action.ActionTranslation = vector * MeshReader.GlobalScale;
        }

        private static void AddActionModelHelper(
            GameObject go,
            Dictionary<int, ActionLink> actionLinkDict,
            DFBlock.RdbObject rdbObj,
            ref DFBlock blockData,
            bool serialize)
        {

            DFBlock.RdbModelResource obj = rdbObj.Resources.ModelResource;
            string description = blockData.RdbBlock.ModelReferenceList[obj.ModelIndex].Description;
            int soundID_Index = obj.SoundIndex;
            float duration = obj.ActionResource.Duration;
            float magnitude = obj.ActionResource.Magnitude;
            int axis = obj.ActionResource.Axis;
            DFBlock.RdbTriggerFlags triggerFlag = DFBlock.RdbTriggerFlags.None;
            DFBlock.RdbActionFlags actionFlag = DFBlock.RdbActionFlags.None;

            if (actionLinkDict != null)
            {
                // Set action flag if valid / known
                if (Enum.IsDefined(typeof(DFBlock.RdbActionFlags), (DFBlock.RdbActionFlags)obj.ActionResource.Flags))
                    actionFlag = (DFBlock.RdbActionFlags)obj.ActionResource.Flags;

                // Set trigger flag if valid / known
                if (Enum.IsDefined(typeof(DFBlock.RdbTriggerFlags), (DFBlock.RdbTriggerFlags)obj.TriggerFlag_StartingLock))
                    triggerFlag = (DFBlock.RdbTriggerFlags)obj.TriggerFlag_StartingLock;

                // Add action node to actionLink dictionary
                if (!actionLinkDict.ContainsKey(rdbObj.Position))
                {
                    ActionLink link;
                    link.nextKey = obj.ActionResource.NextObjectOffset;
                    link.prevKey = obj.ActionResource.PreviousObjectOffset;
                    link.gameObject = go;
                    actionLinkDict.Add(rdbObj.Position, link);
                }
            }

            // Create unique LoadID for save sytem
            ulong loadID = 0;
            if (serialize)
                loadID = (ulong)(blockData.Position + rdbObj.Position);

            AddAction(go, description, soundID_Index, duration, magnitude, axis, triggerFlag, actionFlag, loadID);

            // Add special action door component if this is an "open"/"close" action but model is not a regular action door
            if (actionFlag == DFBlock.RdbActionFlags.OpenDoor || actionFlag == DFBlock.RdbActionFlags.CloseDoor && !IsActionDoor(ref blockData, rdbObj, rdbObj.Resources.ModelResource.ModelIndex))
                go.AddComponent<DaggerfallActionDoorSpecial>();
        }

        private static void AddActionFlatHelper(
            GameObject go,
            Dictionary<int, ActionLink> actionLinkDict,
            ref DFBlock blockData,
            DFBlock.RdbObject rdbObj,
            bool serialize = true)
        {

            DFBlock.RdbFlatResource obj = rdbObj.Resources.FlatResource;
            string description = "FLT";
            int soundID_Index = obj.SoundIndex;
            float duration = 0.0f;
            float magnitude = obj.Magnitude;
            int axis = obj.Magnitude;
            DFBlock.RdbTriggerFlags triggerFlag = DFBlock.RdbTriggerFlags.None;
            DFBlock.RdbActionFlags actionFlag = DFBlock.RdbActionFlags.None;

            //set action flag if valid / known
            if (Enum.IsDefined(typeof(DFBlock.RdbActionFlags), (DFBlock.RdbActionFlags)obj.Action))
                actionFlag = (DFBlock.RdbActionFlags)obj.Action;

            //set trigger flag if valid / known
            if (Enum.IsDefined(typeof(DFBlock.RdbTriggerFlags), (DFBlock.RdbTriggerFlags)obj.Flags))
                triggerFlag = (DFBlock.RdbTriggerFlags)obj.Flags;

            //add action node to actionLink dictionary
            if (!actionLinkDict.ContainsKey(rdbObj.Position))
            {
                ActionLink link;
                link.nextKey = obj.NextObjectOffset;
                link.prevKey = -1;
                link.gameObject = go;
                actionLinkDict.Add(rdbObj.Position, link);
            }

            // Create unique LoadID for save sytem
            ulong loadID = 0;
            if (serialize)
                loadID = (ulong)(blockData.Position + rdbObj.Position);

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
            ulong loadID = 0
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
            action.ActionAxisRawValue = axis_raw;

            // If SaveLoadManager present in game then attach SerializableActionObject
            if (SaveLoadManager.Instance != null)
            {
                go.AddComponent<SerializableActionObject>();
            }

            if (description == "FLT")
            {
                action.IsFlat = true;

                // Add box collider to flats with actions for raycasting - only flats that can be activated directly need this, so this can possibly be restricted in future
                // Skip this for flats that already have a collider assigned from elsewhere (e.g. NPC flats)
                if (!go.GetComponent<Collider>())
                {
                    Collider col = go.AddComponent<BoxCollider>();
                    col.isTrigger = true;
                }
            }
            else
                action.IsFlat = false;

            //if a collision type action or action flat, add DaggerFallActionCollision component
            if (action.TriggerFlag == DFBlock.RdbTriggerFlags.Collision01 || action.TriggerFlag == DFBlock.RdbTriggerFlags.Collision03 ||
                action.TriggerFlag == DFBlock.RdbTriggerFlags.MultiTrigger || action.TriggerFlag == DFBlock.RdbTriggerFlags.Collision09)
            {
                go.AddComponent<DaggerfallActionCollision>();
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
                DaggerfallAction dfAction = link.gameObject.GetComponent<DaggerfallAction>();

                if (dfAction == null)
                    continue;

                try
                {
                    // Link to next node
                    if (actionLinkDict.ContainsKey(link.nextKey))
                        dfAction.NextObject = actionLinkDict[link.nextKey].gameObject;

                    // Link to previous node
                    if (actionLinkDict.ContainsKey(link.prevKey))
                        dfAction.PreviousObject = actionLinkDict[link.prevKey].gameObject;
                }
                catch (Exception ex)
                {
                    DaggerfallUnity.LogMessage(ex.Message, true);
                    DaggerfallUnity.LogMessage(string.Format("Error in LinkActionNodes; {0} : {1} : {2} : {3}", link.gameObject.name, link.nextKey, link.prevKey, dfAction), true);
                }
            }

        }

        /// <summary>
        /// Adds action door to scene.
        /// </summary>
        private static GameObject AddActionDoor(DaggerfallUnity dfUnity, uint modelId, DFBlock.RdbObject obj, Transform parent, ulong loadID = 0)
        {
            if (dfUnity.Option_DungeonDoorPrefab == null)
                return null;

            // Get model data and matrix
            ModelData modelData;
            dfUnity.MeshReader.GetModelData(modelId, out modelData);
            Matrix4x4 modelMatrix = GetModelMatrix(obj);

            // Instantiate door prefab and add model
            // A custom prefab can be provided by mods and must include DaggerfallActionDoor component with all requirements.
            GameObject go = MeshReplacement.ImportCustomGameobject(modelId, parent, Matrix4x4.identity);
            if (!go)
            {
                go = GameObjectHelper.InstantiatePrefab(dfUnity.Option_DungeonDoorPrefab.gameObject, string.Empty, parent, Vector3.zero);
                GameObjectHelper.CreateDaggerfallMeshGameObject(modelId, parent, false, go, true);

                // Resize box collider to new mesh bounds
                BoxCollider boxCollider = go.GetComponent<BoxCollider>();
                MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                if (boxCollider != null && meshRenderer != null)
                {
                    boxCollider.center = meshRenderer.bounds.center;
                    boxCollider.size = meshRenderer.bounds.size;
                }
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
            if (actionDoor)
            {
                // Set starting lock value
                byte[] lockValues = { 0x00, 0x02, 0x04, 0x06, 0x08, 0x0A, 0x0C, 0x0E, 0x10, 0x12, 0x14, 0x19, 0x1E, 0x32, 0x80, 0xFF };
                actionDoor.StartingLockValue = lockValues[obj.Resources.ModelResource.TriggerFlag_StartingLock >> 4];

                // Set LoadID
                actionDoor.LoadID = loadID;
            }
            else
            {
                Debug.LogError($"Failed to get DaggerfallActionDoor on {modelId}. Make sure is added to door prefab.");
            }

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

        private static GameObject AddFlat(DFBlock.RdbObject obj, Transform parent, int archive = -1, int record = -1)
        {
            // Use default archive index if not specified
            if (archive == -1)
                archive = obj.Resources.FlatResource.TextureArchive;

            // Use default record index if not specified
            if (record == -1)
                record = obj.Resources.FlatResource.TextureRecord;

            // Add GameObject to scene
            Vector3 targetPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
            GameObject go = MeshReplacement.ImportCustomFlatGameobject(archive, record, targetPosition, parent, true);
            if (!go)
            {
                // Setup standard billboard and assign RDB data
                go = GameObjectHelper.CreateDaggerfallBillboardGameObject(archive, record, parent);
                go.transform.position = targetPosition;
            }
            Billboard dfBillboard = go.GetComponent<Billboard>();
            if (dfBillboard)
                dfBillboard.SetRDBResourceData(obj.Resources.FlatResource);

            // Add StaticNPC behaviour - required for quest system
            if (IsNPCFlat(archive))
            {
                StaticNPC npc = go.AddComponent<StaticNPC>();
                npc.SetLayoutData(obj);
            }

            // Special handling for individual NPCs found in layout data
            // This NPC may be used in 0 or more active quests at home
            int factionID = obj.Resources.FlatResource.FactionOrMobileId;
            QuestMachine.Instance.SetupIndividualStaticNPC(go, factionID);

            // Disable enemy editor flats
            if (archive == TextureReader.EditorFlatsTextureArchive && (record == 15 || record == 16))
                go.SetActive(false);

            // Add torch burning sound
            if(IsTorchFlat(archive, record))
                AddTorchAudioSource(go);

            return go;
        }

        public static List<int> NPCFlatArchives = new List<int>{334, 346, 357, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184};
        public static bool IsNPCFlat(int archive)
        {
            return NPCFlatArchives.Contains(archive);
        }

        public static bool IsTorchFlat(int archive, int record)
        {
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
                        return true;
                }
            }

            return false;
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
            Transform parent,
            ref DFBlock blockData,
            GameObject[] startMarkers,
            bool serialize)
        {
            // Must have a dungeon type
            if (dungeonType == DFRegion.DungeonTypes.NoDungeon)
                return;

            // Get dungeon type index
            int dungeonIndex = (int)dungeonType;
            if (dungeonIndex < RandomEncounters.EncounterTables.Length)
            {
                // Get water level from start marker if it exists
                Billboard dfBillboard;
                if (startMarkers.Length > 0)
                    dfBillboard = startMarkers[0].GetComponent<Billboard>();
                else
                    dfBillboard = null;

                int waterLevel = 10000;
                if (dfBillboard != null)
                    waterLevel = dfBillboard.Summary.WaterLevel;

                // Get encounter table
                // Use water encounter table if the marker is under the water level
                // These are classic values, so a greater y value means elevation is lower
                RandomEncounterTable table;
                if (waterLevel >= obj.YPos)
                {
                    table = RandomEncounters.EncounterTables[dungeonIndex];
                }
                else
                {
                    table = RandomEncounters.EncounterTables[19]; // underwater encounter table
                }

                // Get base monster index into table
                int baseMonsterIndex = (int)(table.Enemies.Length * monsterPower);

                // Set min index
                int minMonsterIndex = baseMonsterIndex - monsterVariance;
                if (minMonsterIndex < 0)
                    minMonsterIndex = 0;

                // Set max index
                int maxMonsterIndex = baseMonsterIndex + monsterVariance;
                if (maxMonsterIndex >= table.Enemies.Length)
                    maxMonsterIndex = table.Enemies.Length - 1;

                // Get random monster from table
                MobileTypes type = table.Enemies[UnityEngine.Random.Range(minMonsterIndex, maxMonsterIndex + 1)];

                // Create unique LoadID for save sytem
                ulong loadID = 0;
                if (serialize)
                    loadID = (ulong)(blockData.Position + obj.Position);

                byte classicSpawnDistanceType = obj.Resources.FlatResource.SoundIndex;

                // Add enemy
                AddEnemy(obj, type, parent, loadID, classicSpawnDistanceType, false, waterLevel);
            }
            else
            {
                DaggerfallUnity.LogMessage(string.Format("RDBLayout: Dungeon type {0} is out of range or unknown.", dungeonType), true);
            }
        }

        private static void AddRandomRDBEnemyClassic(
            DFBlock.RdbObject obj,
            DFRegion.DungeonTypes dungeonType,
            float monsterPower,
            int monsterVariance,
            Transform parent,
            ref DFBlock blockData,
            GameObject[] startMarkers,
            bool serialize,
            MobileTypes[] DungeonWaterEnemiesToPlace,
            MobileTypes[] DungeonNonWaterEnemiesToPlace)
        {
            // Get dungeon type index
            int dungeonIndex = (int)dungeonType;
            if (dungeonIndex < RandomEncounters.EncounterTables.Length)
            {
                // Get water level from start marker if it exists
                Billboard dfBillboard;
                if (startMarkers.Length > 0)
                    dfBillboard = startMarkers[0].GetComponent<Billboard>();
                else
                    dfBillboard = null;

                int waterLevel = 10000;
                if (dfBillboard != null)
                    waterLevel = dfBillboard.Summary.WaterLevel;

                // Get encounter table
                // Use water encounter table if the marker is under the water level
                // These are classic values, so a greater y value means elevation is lower
                bool usingWaterEnemies = false;
                if (waterLevel < obj.YPos)
                    usingWaterEnemies = true;

                // Create unique LoadID for save sytem
                ulong loadID = 0;
                if (serialize)
                    loadID = (ulong)(blockData.Position + obj.Position);

                int slot = obj.Resources.FlatResource.Flags;
                if (slot == 0)
                    slot = UnityEngine.Random.Range(1, 7);

                MobileTypes type;
                if (usingWaterEnemies)
                    type = DungeonWaterEnemiesToPlace[slot];
                else
                    type = DungeonNonWaterEnemiesToPlace[slot];

                byte classicSpawnDistanceType = obj.Resources.FlatResource.SoundIndex;

                // Add enemy
                AddEnemy(obj, type, parent, loadID, classicSpawnDistanceType, false, waterLevel);
            }
            else
            {
                DaggerfallUnity.LogMessage(string.Format("RDBLayout: Dungeon type {0} is out of range or unknown.", dungeonType), true);
            }
        }

        // Recreation of how classic chooses an enemy type from the random encounter tables
        private static MobileTypes ChooseRandomEnemyType(RandomEncounterTable table)
        {
            int playerLevel = GameManager.Instance.PlayerEntity.Level;
            int minTableIndex = 0;
            int maxTableIndex = table.Enemies.Length;

            int random = DFRandom.random_range_inclusive(1, 100);
            if (random > 95 && playerLevel <= 5)
            {
                maxTableIndex = playerLevel + 2;
            }
            else if (random > 80)
            {
                maxTableIndex = playerLevel + 1;
            }
            else
            {
                minTableIndex = playerLevel - 3;
                maxTableIndex = playerLevel + 3;
            }
            if (minTableIndex < 0)
            {
                minTableIndex = 0;
                maxTableIndex = 5;
            }
            else if (maxTableIndex > 19)
            {
                minTableIndex = 14;
                maxTableIndex = 19;
            }

            return table.Enemies[DFRandom.random_range_inclusive(minTableIndex, maxTableIndex)];
        }

        private static void AddFixedRDBEnemy(DFBlock.RdbObject obj, Transform parent, ref DFBlock blockData, GameObject[] startMarkers, bool serialize)
        {
            bool isCustomMarker = obj.Resources.FlatResource.IsCustomData;

            if (!isCustomMarker)
            {
                // Get type value and ignore known invalid types
                int typeValue = (int)(obj.Resources.FlatResource.FactionOrMobileId & 0xff);
                if (typeValue == 99)
                    return;
            }

            // Create unique LoadID for save sytem
            ulong loadID = 0;
            if (serialize)
                loadID = (ulong)(blockData.Position + obj.Position);

            // Cast to enum
            // DF "Fixed Enemy" markers have garbage data in the 8 MSBs.
            // For custom marker, we take all 16 bits as a MobileId, in case of custom enemies
            MobileTypes type;
            if (!isCustomMarker)
                type = (MobileTypes)(obj.Resources.FlatResource.FactionOrMobileId & 0xff);
            else
                type = (MobileTypes)obj.Resources.FlatResource.FactionOrMobileId;

            byte classicSpawnDistanceType = obj.Resources.FlatResource.SoundIndex;

            // Get water level from start marker if it exists
            Billboard dfBillboard;
            if (startMarkers.Length > 0)
                dfBillboard = startMarkers[0].GetComponent<Billboard>();
            else
                dfBillboard = null;

            int waterLevel = 10000;
            if (dfBillboard != null)
                waterLevel = dfBillboard.Summary.WaterLevel;

            AddEnemy(obj, type, parent, loadID, classicSpawnDistanceType, true, waterLevel);
        }

        private static void AddEnemy(
            DFBlock.RdbObject obj,
            MobileTypes type,
            Transform parent = null,
            ulong loadID = 0,
            byte classicSpawnDistanceType = 0,
            bool useGenderFlag = true,
            int waterLevel = 10000)
        {
            // Water level check. This is done by classic and is needed in at least one case, where otherwise
            // a slaughterfish will be placed outside of water.
            // These are classic values, so a greater y value means elevation is lower
            if ((type == MobileTypes.Slaughterfish || type == MobileTypes.Dreugh || type == MobileTypes.Lamia)
                && (waterLevel == 10000 || waterLevel - 20 > obj.YPos))
            {
                return;
            }

            // Get default reaction
            MobileReactions reaction = MobileReactions.Hostile;
            if (obj.Resources.FlatResource.Action == (int)DFBlock.EnemyReactionTypes.Passive)
                reaction = MobileReactions.Passive;

            MobileGender gender = MobileGender.Unspecified;
            if ((int)type > 43 && useGenderFlag)
            {
                int femaleFlag = (int)DFBlock.EnemyGenders.Female;
                int maleFlag = (int)DFBlock.EnemyGenders.Male;
                if ((obj.Resources.FlatResource.Flags & femaleFlag) == femaleFlag)
                    gender = MobileGender.Female;
                if ((obj.Resources.FlatResource.Flags & maleFlag) == maleFlag)
                    gender = MobileGender.Male;
            }

            // Just setup demo enemies at this time
            string name = string.Format("DaggerfallEnemy [{0}]", type.ToString());
            Vector3 position = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
            GameObject go = GameObjectHelper.InstantiatePrefab(DaggerfallUnity.Instance.Option_EnemyPrefab.gameObject, name, parent, position);
            SetupDemoEnemy setupEnemy = go.GetComponent<SetupDemoEnemy>();
            if (setupEnemy != null)
            {
                // Configure enemy
                setupEnemy.ApplyEnemySettings(type, reaction, gender, classicSpawnDistanceType);

                // Align non-flying units with ground
                MobileUnit mobileUnit = setupEnemy.GetMobileBillboardChild();
                if (mobileUnit.Enemy.Behaviour != MobileBehaviour.Flying)
                    GameObjectHelper.AlignControllerToGround(go.GetComponent<CharacterController>());
            }

            DaggerfallEnemy enemy = go.GetComponent<DaggerfallEnemy>();
            if (enemy)
            {
                enemy.LoadID = loadID;
            }

            GameManager.Instance?.RaiseOnEnemySpawnEvent(go);

        }

        private static DaggerfallLoot AddRandomTreasure(
            DFBlock.RdbObject obj,
            Transform parent,
            ref DFBlock blockData,
            DFRegion.DungeonTypes dungeonType,
            bool serialize,
            int archive = 0,
            int record = 0,
            bool adjustPosition = true)
        {
            // Create unique LoadID for save sytem
            ulong loadID = 0;
            if (serialize)
                loadID = (ulong)(blockData.Position + obj.Position);

            // Randomise container texture
            int iconIndex = UnityEngine.Random.Range(0, DaggerfallLootDataTables.randomTreasureIconIndices.Length);
            int iconRecord = DaggerfallLootDataTables.randomTreasureIconIndices[iconIndex];

            // Find bottom of marker in world space
            // Marker is aligned to surface and has a constant size (40x40)
            Vector3 position = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
            if (adjustPosition)
                position.y += (-DaggerfallLoot.randomTreasureMarkerDim / 2 * MeshReader.GlobalScale);

            // Create random loot container
            DaggerfallLoot loot = null;
            if (archive == 0)
            {
                loot = GameObjectHelper.CreateLootContainer(
                    LootContainerTypes.RandomTreasure,
                    InventoryContainerImages.Chest,
                    position,
                    parent,
                    DaggerfallLootDataTables.randomTreasureArchive,
                    iconRecord,
                    loadID,
                    null,
                    adjustPosition);
            }
            else
            {
                loot = GameObjectHelper.CreateLootContainer(
                    LootContainerTypes.RandomTreasure,
                    InventoryContainerImages.Chest,
                    position,
                    parent,
                    archive,
                    record,
                    loadID,
                    null,
                    adjustPosition);
            }

            // Also Adjust random treasure to surface below so it doesn't end up floating if marker is floating
            if (adjustPosition && loot)
            {
                Billboard dfBillboard = loot.gameObject.GetComponent<Billboard>();
                if (dfBillboard)
                    GameObjectHelper.AlignBillboardToGround(dfBillboard.gameObject, dfBillboard.Summary.Size, 4);
            }

            // Get dungeon type index
            int dungeonIndex = (int)dungeonType;

            if (!LootTables.GenerateLoot(loot, dungeonIndex))
                DaggerfallUnity.LogMessage(string.Format("RDBLayout: Dungeon type {0} is out of range or unknown.", dungeonType), true);

            return loot;
        }

        #endregion
    }
}