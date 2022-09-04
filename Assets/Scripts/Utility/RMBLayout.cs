// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;
using Unity.Profiling;
using Unity.Collections;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Helper for laying out RMB (city block) data in scene.
    /// </summary>
    public static class RMBLayout
    {

        #region Fields

        public const int RMBTilesPerBlock = 16;
        public const int RMBTilesPerTerrain = 128;
        public const float RMBSide = 4096f * MeshReader.GlobalScale;
        public const float RMBTileSide = 256f * MeshReader.GlobalScale;

        const float propsOffsetY = -4f;
        const float blockFlatsOffsetY = -6f;
        const float natureFlatsOffsetY = -2f;
        public const uint CityGateOpenModelID = 446;
        public const uint CityGateClosedModelID = 447;
        public const uint BulletinBoardModelID = 41739;

#if !UNITY_EDITOR
        private static int maxLocationCacheSize = 12;
#endif
        private static List<KeyValuePair<int, DFBlock[]>> locationCache = new List<KeyValuePair<int, DFBlock[]>>();

        /// <summary>Clear the location cache. Use if block data is changed dynamically.</summary>
        public static void ClearLocationCache()
        {
            locationCache.Clear();
        }

        #endregion

        #region Profiler Markers

        static readonly ProfilerMarker
            ___CreateBaseGameObject = new ProfilerMarker($"{nameof(RMBLayout)}.{nameof(CreateBaseGameObject)}"),
            ___GetBuildingData = new ProfilerMarker($"{nameof(RMBLayout)}.{nameof(GetBuildingData)}"),
            ___GetLocationBuildingData = new ProfilerMarker($"{nameof(RMBLayout)}.{nameof(GetLocationBuildingData)}"),
            ___getNextBuildingFromPool = new ProfilerMarker("get next building from pool"),
            ___storeNamedNuildings = new ProfilerMarker("store named buildings in pool for distribution"),
            ___getBuildingSummaryOfAllBlocks = new ProfilerMarker("get building summary of all blocks in this location"),
            ___getBlockName = new ProfilerMarker("get block name"),
            ___getBlockData = new ProfilerMarker("get block data"),
            ___copyBuildingDataArray = new ProfilerMarker("make a copy of the building data array for our block copy since we're modifying it"),
            ___assignBuildingData = new ProfilerMarker("assign building data for this block"),
            ___AddGroundPlane = new ProfilerMarker($"{nameof(RMBLayout)}.{nameof(AddGroundPlane)}"),
            ___AddModels = new ProfilerMarker($"{nameof(RMBLayout)}.{nameof(AddModels)}"),
            ___AddProps = new ProfilerMarker($"{nameof(RMBLayout)}.{nameof(AddProps)}"),
            ___AddStandaloneModel = new ProfilerMarker($"{nameof(RMBLayout)}.{nameof(AddStandaloneModel)}"),
            ___AddExteriorBlockFlats = new ProfilerMarker($"{nameof(RMBLayout)}.{nameof(AddExteriorBlockFlats)}"),
            ___AddNatureFlats = new ProfilerMarker($"{nameof(RMBLayout)}.{nameof(AddNatureFlats)}"),
            ___AddLights = new ProfilerMarker($"{nameof(RMBLayout)}.{nameof(AddLights)}"),
            ___AddMiscBlockFlats = new ProfilerMarker($"{nameof(RMBLayout)}.{nameof(AddMiscBlockFlats)}");

        #endregion

        #region Layout Methods

        /// <summary>
        /// Gets the model scale vector, converting zeros to 1s if needed.
        /// </summary>
        /// <param name="obj">RmbBlock3dObjectRecord structure</param>
        /// <returns>Vector3 with the scaling factors</returns>
        public static Vector3 GetModelScaleVector(DFBlock.RmbBlock3dObjectRecord obj)
        {
            return new Vector3(obj.XScale == 0 ? 1 : obj.XScale, obj.YScale == 0 ? 1 : obj.YScale, obj.ZScale == 0 ? 1 : obj.YScale);
        }

        /// <summary>
        /// Gets block data with validation.
        /// </summary>
        /// <param name="blockName">Block name.</param>
        /// <param name="blockDataOut">DFBlock data out.</param>
        /// <returns>True if validated and successful.</returns>
        public static bool GetBlockData(string blockName, out DFBlock blockDataOut)
        {
            blockDataOut = new DFBlock();

            // Validate
            if (string.IsNullOrEmpty(blockName))
                return false;
            if (!blockName.EndsWith(".RMB", StringComparison.InvariantCultureIgnoreCase))
                return false;
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return false;

            // Get block data
            blockDataOut = dfUnity.ContentReader.BlockFileReader.GetBlock(blockName);

            return true;
        }

        /// <summary>
        /// Create base RMB block by name.
        /// </summary>
        /// <param name="blockName">Name of block.</param>
        /// <param name="layoutX">X coordinate in parent map layout.</param>
        /// <param name="layoutY">Y coordinate in parent map layout.</param>
        /// <param name="cloneFrom">Prefab to clone from.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(string blockName, int layoutX, int layoutY, DaggerfallRMBBlock cloneFrom = null)
        {
            DFBlock blockData;
            return CreateBaseGameObject(blockName, layoutX, layoutY, out blockData, cloneFrom);
        }

        /// <summary>
        /// Create base RMB block by name and get back DFBlock data.
        /// </summary>
        /// <param name="blockName">Name of block.</param>
        /// <param name="layoutX">X coordinate in parent map layout.</param>
        /// <param name="layoutY">Y coordinate in parent map layout.</param>
        /// <param name="blockDataOut">DFBlock data out.</param>
        /// <param name="cloneFrom">Prefab to clone from.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(string blockName, int layoutX, int layoutY, out DFBlock blockDataOut, DaggerfallRMBBlock cloneFrom = null)
        {
            // Get block data
            if (!GetBlockData(blockName, out blockDataOut))
                return null;

            return CreateBaseGameObject(ref blockDataOut, layoutX, layoutY, cloneFrom);
        }

        /// <summary>
        /// Instantiate base RMB block by DFBlock data.
        /// </summary>
        /// <param name="blockData">Block data.</param>
        /// <param name="layoutX">X coordinate in parent map layout.</param>
        /// /// <param name="layoutY">Y coordinate in parent map layout.</param>
        /// <param name="cloneFrom">Prefab to clone from.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(ref DFBlock blockData, int layoutX, int layoutY, DaggerfallRMBBlock cloneFrom = null)
        {
            ___CreateBaseGameObject.Begin();

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            // Create gameobject
            GameObject go;
            string name = string.Format("DaggerfallBlock [{0}]", blockData.Name);
            if (cloneFrom != null)
                go = GameObjectHelper.InstantiatePrefab(cloneFrom.gameObject, name, null, Vector3.zero);
            else
                go = new GameObject(name);

            // Setup combiner
            ModelCombiner combiner = null;
            if (dfUnity.Option_CombineRMB)
                combiner = new ModelCombiner();

            // Lists to receive any doors found in this block
            List<StaticDoor> modelDoors;
            List<StaticDoor> propDoors;
            List<StaticBuilding> modelBuildings;

            // Add models and static props
            GameObject modelsNode = new GameObject("Models");
            modelsNode.transform.parent = go.transform;
            AddModels(dfUnity, layoutX, layoutY, ref blockData, out modelDoors, out modelBuildings, combiner, modelsNode.transform);
            AddProps(dfUnity, ref blockData, out propDoors, combiner, modelsNode.transform);

            // Combine list of doors found in models and props
            List<StaticDoor> allDoors = new List<StaticDoor>();
            if (modelDoors.Count > 0) allDoors.AddRange(modelDoors);
            if (propDoors.Count > 0) allDoors.AddRange(propDoors);

            // Assign building key to each door
            for (int i = 0; i < allDoors.Count; i++)
            {
                StaticDoor door = allDoors[i];
                door.buildingKey = BuildingDirectory.MakeBuildingKey((byte)layoutX, (byte)layoutY, (byte)door.recordIndex);
                allDoors[i] = door;
            }

            // Assign building key to each building
            for (int i = 0; i < modelBuildings.Count; i++)
            {
                StaticBuilding building = modelBuildings[i];
                building.buildingKey = BuildingDirectory.MakeBuildingKey((byte)layoutX, (byte)layoutY, (byte)building.recordIndex);
                modelBuildings[i] = building;
            }

            // Add static doors component
            if (allDoors.Count > 0)
                AddStaticDoors(allDoors.ToArray(), go);

            // Add static buildings component
            if (modelBuildings.Count > 0)
                AddStaticBuildings(modelBuildings.ToArray(), go);

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

            ___CreateBaseGameObject.End();
            return go;
        }

        /// <summary>
        /// Add nature billboards.
        /// </summary>
        public static void AddNatureFlats(
            ref DFBlock blockData,
            Transform flatsParent,
            DaggerfallBillboardBatch billboardBatch = null,
            ClimateNatureSets climateNature = ClimateNatureSets.TemperateWoodland,
            ClimateSeason climateSeason = ClimateSeason.Summer)
        {
            ___AddNatureFlats.Begin();

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
            {
                ___AddNatureFlats.End();
                return;
            }

            // aliases:
            var groundScenery = blockData.RmbBlock.FldHeader.GroundData.GroundScenery;
            float tileDimension =  BlocksFile.TileDimension;
            float globalScale = MeshReader.GlobalScale;

            // Add billboard to batch or standalone
            const int numX = 16, numY = 16;
            if (billboardBatch != null)
            {
                var billboardItems = new NativeArray<DaggerfallBillboardBatch.ItemToAdd>(numX * numY, Allocator.TempJob);
                int billboardCounter = 0;
                for (int y = 0; y < numY; y++)
                for (int x = 0; x < numX; x++)
                {
                    // Get scenery item - ignore indices -1 (empty) and 0 (marker/waypoint of some kind)
                    ref var refScenery = ref groundScenery[x, 15 - y];
                    if (refScenery.TextureRecord < 1)
                        continue;

                    // Calculate position
                    Vector3 billboardPosition = new Vector3(x * tileDimension, natureFlatsOffsetY, y * tileDimension + tileDimension) * globalScale;

                    // Get Archive
                    int natureArchive = ClimateSwaps.GetNatureArchive(climateNature, climateSeason);

                    // Import custom 3d gameobject instead of flat
                    if (MeshReplacement.ImportCustomFlatGameobject(natureArchive, refScenery.TextureRecord, billboardPosition, flatsParent) != null)
                        continue;
                    
                    billboardItems[billboardCounter++] = new DaggerfallBillboardBatch.ItemToAdd(refScenery.TextureRecord, billboardPosition);
                }
                if( billboardCounter!=0 )
                {
                    var addItemsJobHandle = billboardBatch.AddItemsAsync(billboardItems.GetSubArray(0,billboardCounter));
                    billboardItems.Dispose(addItemsJobHandle);
                }
                else billboardItems.Dispose();
            }
            else
            {
                for (int y = 0; y < numY; y++)
                for (int x = 0; x < numX; x++)
                {
                    // Get scenery item - ignore indices -1 (empty) and 0 (marker/waypoint of some kind)
                    ref var refScenery = ref groundScenery[x, 15 - y];
                    if (refScenery.TextureRecord < 1)
                        continue;

                    // Calculate position
                    Vector3 billboardPosition = new Vector3(x * tileDimension, natureFlatsOffsetY, y * tileDimension + tileDimension) * globalScale;

                    // Get Archive
                    int natureArchive = ClimateSwaps.GetNatureArchive(climateNature, climateSeason);

                    // Import custom 3d gameobject instead of flat
                    if (MeshReplacement.ImportCustomFlatGameobject(natureArchive, refScenery.TextureRecord, billboardPosition, flatsParent) != null)
                        continue;
                    
                    GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(natureArchive, refScenery.TextureRecord, flatsParent);
                    go.transform.position = billboardPosition;
                    AlignBillboardToBase(go);
                }
            }

            ___AddNatureFlats.End();
        }

        /// <summary>
        /// Adds light flats and prefabs.
        /// </summary>
        public static void AddLights(
            ref DFBlock blockData,
            Transform flatsParent,
            Transform lightsParent,
            DaggerfallBillboardBatch billboardBatch = null)
        {
            ___AddLights.Begin();

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
            {
                ___AddLights.End();
                return;
            }
                

            // Do nothing if import option not enabled or missing prefab
            if (!dfUnity.Option_ImportLightPrefabs || dfUnity.Option_CityLightPrefab == null)
            {
                ___AddLights.End();
                return;
            }

            // aliases:
            var flatObjectRecords = blockData.RmbBlock.MiscFlatObjectRecords;
            float globalScale = MeshReader.GlobalScale;

            // Add billboard to batch or standalone
            if (billboardBatch != null)
            {
                // Iterate block flats for lights
                var billboardItems = new NativeArray<DaggerfallBillboardBatch.ItemToAdd>(flatObjectRecords.Length, Allocator.TempJob);
                int billboardCounter = 0;
                // Add point lights
                foreach (var obj in flatObjectRecords)
                if (obj.TextureArchive == TextureReader.LightsTextureArchive)
                {
                    // Calculate position
                    Vector3 billboardPosition = new Vector3( obj.XPos, -obj.YPos + blockFlatsOffsetY, obj.ZPos + BlocksFile.RMBDimension) * globalScale;

                    // Import custom 3d gameobject instead of flat
                    if (MeshReplacement.ImportCustomFlatGameobject(obj.TextureArchive, obj.TextureRecord, billboardPosition, flatsParent) != null)
                        continue;

                    billboardItems[billboardCounter++] = new DaggerfallBillboardBatch.ItemToAdd(obj.TextureRecord, billboardPosition);

                    // Import light prefab
                    AddLight(dfUnity, obj, lightsParent);
                }
                if( billboardCounter!=0 )
                {
                    var addItemsJobHandle = billboardBatch.AddItemsAsync(billboardItems.GetSubArray(0, billboardCounter));
                    billboardItems.Dispose(addItemsJobHandle);
                }
                else billboardItems.Dispose();
            }
            else
            {
                // Add point lights
                foreach (var obj in flatObjectRecords)
                if (obj.TextureArchive == TextureReader.LightsTextureArchive)
                {
                    // Calculate position
                    Vector3 billboardPosition = new Vector3( obj.XPos, -obj.YPos + blockFlatsOffsetY, obj.ZPos + BlocksFile.RMBDimension) * globalScale;

                    // Import custom 3d gameobject instead of flat
                    if (MeshReplacement.ImportCustomFlatGameobject(obj.TextureArchive, obj.TextureRecord, billboardPosition, flatsParent) != null)
                        continue;

                    GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(obj.TextureArchive, obj.TextureRecord, flatsParent);
                    go.transform.position = billboardPosition;
                    AlignBillboardToBase(go);

                    // Import light prefab
                    AddLight(dfUnity, obj, lightsParent);
                }
            }

            ___AddLights.End();
        }

        /// <summary>
        /// Add misc block flats.
        /// Batching is conditionally supported.
        /// </summary>
        public static void AddMiscBlockFlats(
            ref DFBlock blockData,
            Transform flatsParent,
            int mapId,
            int locationIndex,
            DaggerfallBillboardBatch animalsBillboardBatch = null,
            TextureAtlasBuilder miscBillboardsAtlas = null,
            DaggerfallBillboardBatch miscBillboardsBatch = null)
        {
            ___AddMiscBlockFlats.Begin();

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
            {
                ___AddMiscBlockFlats.End();
                return;
            }

            // Add block flats
            foreach (DFBlock.RmbBlockFlatObjectRecord obj in blockData.RmbBlock.MiscFlatObjectRecords)
            {
                // Ignore lights as they are handled by AddLights()
                if (obj.TextureArchive == TextureReader.LightsTextureArchive)
                    continue;

                // Calculate position
                Vector3 billboardPosition = new Vector3(
                    obj.XPos,
                    -obj.YPos + blockFlatsOffsetY,
                    obj.ZPos + BlocksFile.RMBDimension) * MeshReader.GlobalScale;
                
                GameObject go = MeshReplacement.ImportCustomFlatGameobject(obj.TextureArchive, obj.TextureRecord, billboardPosition, flatsParent);
                if (go == null)
                {
                    // Add standalone billboard gameobject
                    go = GameObjectHelper.CreateDaggerfallBillboardGameObject(obj.TextureArchive, obj.TextureRecord, flatsParent);
                    go.transform.position = billboardPosition;
                    AlignBillboardToBase(go);
                }

                // Add animal sound
                if (obj.TextureArchive == TextureReader.AnimalsTextureArchive)
                    GameObjectHelper.AddAnimalAudioSource(go, obj.TextureRecord);

                // If flat record has a non-zero faction id, then it's an exterior NPC
                if (obj.FactionID != 0)
                {
                    // Add RMB data to billboard
                    Billboard dfBillboard = go.GetComponent<Billboard>();
                    if (dfBillboard != null)
                        dfBillboard.SetRMBPeopleData(obj.FactionID, obj.Flags, obj.Position);

                    // Add StaticNPC behaviour
                    StaticNPC npc = go.AddComponent<StaticNPC>();
                    npc.SetLayoutData(obj, mapId, locationIndex);

                    QuestMachine.Instance.SetupIndividualStaticNPC(go, obj.FactionID);
                }
            }

            ___AddMiscBlockFlats.End();
        }

        /// <summary>
        /// Add subrecord (building) exterior block flats.
        /// </summary>
        public static void AddExteriorBlockFlats(
            ref DFBlock blockData,
            Transform flatsParent,
            Transform lightsParent,
            int mapId,
            int locationIndex,
            ClimateNatureSets climateNature = ClimateNatureSets.TemperateWoodland,
            ClimateSeason climateSeason = ClimateSeason.Summer)
        {
            ___AddExteriorBlockFlats.Begin();

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
            {
                ___AddExteriorBlockFlats.End();
                return;
            }

            // Get Nature Archive
            int natureArchive = ClimateSwaps.GetNatureArchive(climateNature, climateSeason);

            foreach (DFBlock.RmbSubRecord subRecord in blockData.RmbBlock.SubRecords)
            {
                Vector3 subRecordPosition = new Vector3(subRecord.XPos, 0, -subRecord.ZPos) * MeshReader.GlobalScale;

                foreach (DFBlock.RmbBlockFlatObjectRecord obj in subRecord.Exterior.BlockFlatObjectRecords)
                {
                    // Don't add building exterior editor flats since they can't be used by any DFU systems
                    int archive = obj.TextureArchive;
                    if (archive == TextureReader.EditorFlatsTextureArchive)
                        continue;

                    // Calculate position
                    Vector3 billboardPosition = new Vector3(
                        obj.XPos,
                        -obj.YPos + blockFlatsOffsetY,
                        obj.ZPos + BlocksFile.RMBDimension) * MeshReader.GlobalScale;

                    billboardPosition += subRecordPosition;

                    // Add natures using correct climate set archive
                    if (archive >= (int)DFLocation.ClimateTextureSet.Nature_RainForest && archive <= (int)DFLocation.ClimateTextureSet.Nature_Mountains_Snow)
                    {
                        archive = natureArchive;
                        billboardPosition.z = natureFlatsOffsetY;
                    }

                    GameObject go = MeshReplacement.ImportCustomFlatGameobject(archive, obj.TextureRecord, billboardPosition, flatsParent);
                    bool isImported = go != null;
                    if (!isImported)
                    {
                        // Add standalone billboard gameobject
                        go = GameObjectHelper.CreateDaggerfallBillboardGameObject(archive, obj.TextureRecord, flatsParent);
                        go.transform.position = billboardPosition;
                        AlignBillboardToBase(go);
                    }

                    // Add animal sound
                    if (archive == TextureReader.AnimalsTextureArchive)
                        GameObjectHelper.AddAnimalAudioSource(go, obj.TextureRecord);

                    // If flat record has a non-zero faction id, then it's an exterior NPC
                    if (obj.FactionID != 0)
                    {
                        // Add RMB data to billboard
                        Billboard dfBillboard = go.GetComponent<Billboard>();
                        if (dfBillboard != null)
                            dfBillboard.SetRMBPeopleData(obj.FactionID, obj.Flags, obj.Position);

                        // Add StaticNPC behaviour
                        StaticNPC npc = go.AddComponent<StaticNPC>();
                        npc.SetLayoutData(obj, mapId, locationIndex);

                        QuestMachine.Instance.SetupIndividualStaticNPC(go, obj.FactionID);
                    }

                    // If this is a light flat, import light prefab
                    if (archive == TextureReader.LightsTextureArchive && !isImported)
                    {
                        if (dfUnity.Option_CityLightPrefab == null)
                        {
                            ___AddExteriorBlockFlats.End();
                            return;
                        }

                        Vector2 size = dfUnity.MeshReader.GetScaledBillboardSize(210, obj.TextureRecord);
                        Vector3 position = new Vector3(
                            obj.XPos,
                            -obj.YPos + size.y,
                            obj.ZPos + BlocksFile.RMBDimension) * MeshReader.GlobalScale;
                        position += subRecordPosition;

                        GameObjectHelper.InstantiatePrefab(dfUnity.Option_CityLightPrefab.gameObject, string.Empty, lightsParent, position);
                    }
                }
            }

            ___AddExteriorBlockFlats.End();
        }

        /// <summary>
        /// Aligns billboard GameObject to centre of base.
        /// This is required for exterior billboard.
        /// </summary>
        /// <param name="go">GameObject with DaggerfallBillboard component.</param>
        public static void AlignBillboardToBase(GameObject go)
            => go.GetComponent<Billboard>()?.AlignToBase();

        /// <summary>
        /// Add simple ground plane to block layout.
        /// </summary>
        public static GameObject AddGroundPlane(
            ref DFBlock blockData,
            Transform parent = null,
            ClimateBases climateBase = ClimateBases.Temperate,
            ClimateSeason climateSeason = ClimateSeason.Summer)
        {
            ___AddGroundPlane.Begin();

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
            {
                ___AddGroundPlane.End();
                return null;
            }

            GameObject go = new GameObject("Ground");
            if (parent != null)
                go.transform.parent = parent;

            // Assign components
            DaggerfallGroundPlane dfGround = go.AddComponent<DaggerfallGroundPlane>();
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();

            // Assign climate and mesh
            Color32[] tileMap;
            Mesh mesh = dfUnity.MeshReader.GetSimpleGroundPlaneMesh(
                ref blockData,
                out tileMap,
                dfUnity.MeshReader.AddMeshTangents,
                dfUnity.MeshReader.AddMeshLightmapUVs);
            if (mesh)
            {
                meshFilter.sharedMesh = mesh;
            }

            // Assign tileMap and climate
            dfGround.tileMap = tileMap;
            dfGround.SetClimate(dfUnity, climateBase, climateSeason);

            // Assign collider
            if (dfUnity.Option_AddMeshColliders)
                go.AddComponent<BoxCollider>();

            // Assign static
            if (dfUnity.Option_SetStaticFlags)
                GameObjectHelper.TagStaticGeometry(go);

            ___AddGroundPlane.End();
            return go;
        }

        #endregion

        #region Building Methods

        /// <summary>
        /// Gets BuildingSummary array generated from DFBlock data.
        /// DFBlock data should be provided from RMBLayout.GetLocationBuildingData() output.
        /// Otherwise not all building data will be present.
        /// </summary>
        /// <param name="blockData">DFBlock data.</param>
        /// <param name="layoutX">X coordindate in map layout used to generate building key.</param>
        /// <param name="layoutY">Y coordindate in map layout used to generate building key.</param>
        /// <returns>BuildingSummary.</returns>
        public static BuildingSummary[] GetBuildingData(in DFBlock blockData, int layoutX = -1, int layoutY = -1)
        {
            ___GetBuildingData.Begin();

            // Store building information
            int buildingCount = blockData.RmbBlock.SubRecords.Length;
            BuildingSummary[] buildings = new BuildingSummary[buildingCount];
            for (int i = 0; i < buildingCount; i++)
            {
                // Create building summary
                var building = new BuildingSummary();

                // Set building data
                ref readonly DFLocation.BuildingData buildingData = ref blockData.RmbBlock.FldHeader.BuildingDataList[i];
                building.buildingKey = BuildingDirectory.MakeBuildingKey((byte)layoutX, (byte)layoutY, (byte)i);
                building.NameSeed = buildingData.NameSeed;
                building.FactionId = buildingData.FactionId;
                building.BuildingType = buildingData.BuildingType;
                building.Quality = buildingData.Quality;

                // Set building transform info
                ref readonly DFBlock.RmbSubRecord subRecord = ref blockData.RmbBlock.SubRecords[i];
                building.Position = new Vector3(subRecord.XPos, 0, BlocksFile.RMBDimension - subRecord.ZPos) * MeshReader.GlobalScale;
                building.Rotation = new Vector3(0, -subRecord.YRotation / BlocksFile.RotationDivisor, 0);
                building.Matrix = Matrix4x4.TRS(building.Position, Quaternion.Euler(building.Rotation), Vector3.one);

                // First model of record is building model
                if (subRecord.Exterior.Block3dObjectRecords.Length > 0)
                    building.ModelID = subRecord.Exterior.Block3dObjectRecords[0].ModelIdNum;

                buildings[i] = building;
            }

            ___GetBuildingData.End();
            return buildings;
        }

        /// <summary>
        /// Gets all RMB blocks from a location populated with building data from MAPS.BSA.
        /// This method is using "best effort" process at this point in time.
        /// However, it does yield very accurate results most of the time.
        /// Please use exception handling when calling this method for now.
        /// It will be progressed over time.
        /// </summary>
        /// <param name="location">Location to use.</param>
        public static DFBlock[] GetLocationBuildingData(in DFLocation location)
        {
            ___GetLocationBuildingData.Begin();

            int mapId = location.MapTableData.MapId;
            DFBlock[] blocksArray = locationCache.FirstOrDefault(l => l.Key == mapId).Value;
            if (blocksArray != null)
            {
                ___GetLocationBuildingData.End();
                return blocksArray;
            }

            var namedBuildingPool = new Dictionary<DFLocation.BuildingTypes, List<DFLocation.BuildingData>>(capacity:System.Enum.GetNames(typeof(DFLocation.BuildingTypes)).Length);
            List<DFBlock> blocks = new List<DFBlock>();

            // Get content reader
            ContentReader contentReader = DaggerfallUnity.Instance.ContentReader;
            if (contentReader == null)
            {
                ___GetLocationBuildingData.End();
                throw new Exception("GetCompleteBuildingData() could not find ContentReader.");
            }

            // Store named buildings in pool for distribution
            ___storeNamedNuildings.Begin();
            {
                int buildingCount = location.Exterior.BuildingCount;
                var buildings = location.Exterior.Buildings;
                for (int i = 0; i < buildingCount; i++)
                {
                    var building = buildings[i];
                    var type = building.BuildingType;
                    if (IsNamedBuilding(type))
                    {
                        if(!namedBuildingPool.ContainsKey(type))
                            namedBuildingPool.Add(type,new List<DFLocation.BuildingData>());
                        
                        namedBuildingPool[type].Add(building);
                    }
                }
            }
            ___storeNamedNuildings.End();

            // Get building summary of all blocks in this location
            ___getBuildingSummaryOfAllBlocks.Begin();
            var exteriorData = location.Exterior.ExteriorData;
            int
                width = exteriorData.Width,
                height = exteriorData.Height;
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                // Get block name
                ___getBlockName.Begin();
                string blockName = contentReader.BlockFileReader.CheckName(contentReader.MapFileReader.GetRmbBlockName(location, x, y));
                ___getBlockName.End();

                // Get block data
                ___getBlockData.Begin();
                if (!contentReader.GetBlock(blockName, out var block))
                {
                    ___getBlockData.End();
                    ___GetLocationBuildingData.End();
                    throw new Exception($"GetCompleteBuildingData() could not read block {blockName}");
                }
                ___getBlockData.End();

                // aliases
                ref var refBlockRmbBlock = ref block.RmbBlock;
                ref var refBlockFldHeader = ref refBlockRmbBlock.FldHeader;
                var blockBuildingDataList = refBlockFldHeader.BuildingDataList;
                string[] blockOtherNames = refBlockFldHeader.OtherNames;
                int numSubRecords = refBlockRmbBlock.SubRecords.Length;

                // Make a copy of the building data array for our block copy since we're modifying it
                ___copyBuildingDataArray.Begin();
                refBlockFldHeader.BuildingDataList = (DFLocation.BuildingData[])refBlockFldHeader.BuildingDataList.Clone();
                ___copyBuildingDataArray.End();

                // Assign building data for this block
                ___assignBuildingData.Begin();
                for (int i = 0; i < numSubRecords; i++)
                {
                    ref var refBuilding = ref blockBuildingDataList[i];
                    ref var refBuildingType = ref refBuilding.BuildingType;
                    if (IsNamedBuilding(refBuildingType))
                    {
                        // Try to find next building and merge data
                        
                        // Draws and consume building from pool. Checking if buildings are ordered by special type.
                        ___getNextBuildingFromPool.Begin();
                        DFLocation.BuildingData item = default;
                        if(namedBuildingPool.TryGetValue(refBuildingType, out var list) && list.Count!=0)
                        {
                            int lastIndex = list.Count-1;
                            item = list[lastIndex];
                            list.RemoveAtSwapBack(lastIndex);
                        }
                        else
                        {
                            Debug.Log($"End of city building list reached without finding building type {refBuildingType} in location {location.RegionName}.{location.Name}");
                            // can we continue if the results item is just zeroed struct ??
                            // won't this produce an undefined behaviour (bug)?
                        }
                        ___getNextBuildingFromPool.End();
                        
                        // Copy found city building data to block level
                        refBuilding.NameSeed = item.NameSeed;
                        refBuilding.FactionId = item.FactionId;
                        refBuilding.Sector = item.Sector;
                        refBuilding.LocationId = item.LocationId;
                        refBuilding.Quality = item.Quality;

                        // Check for replacement building data and use it if found
                        if (WorldDataReplacement.GetBuildingReplacementData(blockName, block.Index, i, out var buildingReplacementData))
                        {
                            // Use custom building values from replacement data, but only use up pool item if factionId is zero
                            if (buildingReplacementData.FactionId != 0)
                            {
                                // Don't use up pool item and set factionId, NameSeed, Quality from replacement data
                                namedBuildingPool[refBuildingType].Add(item);
                                refBuilding.FactionId = buildingReplacementData.FactionId;
                                refBuilding.Quality = buildingReplacementData.Quality;
                                refBuilding.NameSeed = (ushort)(buildingReplacementData.NameSeed + location.LocationIndex);    // Vary name seed by location
                            }
                            // Always override type
                            refBuildingType = (DFLocation.BuildingTypes)buildingReplacementData.BuildingType;
                        }

                        // Matched to classic: special handling for some Order of the Raven buildings
                        if (blockOtherNames?[i] == "KRAVE01.HS2")
                        {
                            refBuildingType = DFLocation.BuildingTypes.GuildHall;
                            refBuilding.FactionId = 414;
                        }
                    }
                }
                ___assignBuildingData.End();

                // Save block data
                blocks.Add(block);
            }
            ___getBuildingSummaryOfAllBlocks.End();

            blocksArray = blocks.ToArray();

#if !UNITY_EDITOR // Cache blocks for this location if not in editor
            if (locationCache.Count == maxLocationCacheSize)
                locationCache.RemoveAt(0);
            locationCache.Add(new KeyValuePair<int, DFBlock[]>(mapId, blocksArray));
#endif

            ___GetLocationBuildingData.End();
            return blocksArray;
        }

        /// <summary>
        /// Checks if building type is a special named building.
        /// </summary>
        public static bool IsNamedBuilding(DFLocation.BuildingTypes buildingType)
        {
            switch (buildingType)
            {
                case DFLocation.BuildingTypes.Alchemist:
                case DFLocation.BuildingTypes.Armorer:
                case DFLocation.BuildingTypes.Bank:
                case DFLocation.BuildingTypes.Bookseller:
                case DFLocation.BuildingTypes.ClothingStore:
                case DFLocation.BuildingTypes.FurnitureStore:
                case DFLocation.BuildingTypes.GemStore:
                case DFLocation.BuildingTypes.GeneralStore:
                case DFLocation.BuildingTypes.Library:
                case DFLocation.BuildingTypes.GuildHall:
                case DFLocation.BuildingTypes.PawnShop:
                case DFLocation.BuildingTypes.WeaponSmith:
                case DFLocation.BuildingTypes.Temple:
                case DFLocation.BuildingTypes.Tavern:
                case DFLocation.BuildingTypes.Palace:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if building is a residence.
        /// </summary>
        public static bool IsResidence(DFLocation.BuildingTypes buildingType)
        {
            // Only House1-House4 seem to ID as "Residence" (to confirm)
            if (buildingType >= DFLocation.BuildingTypes.House1 && buildingType <= DFLocation.BuildingTypes.House4)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if building is a shop with quality text.
        /// </summary>
        public static bool IsShop(DFLocation.BuildingTypes buildingType)
        {
            switch (buildingType)
            {
                case DFLocation.BuildingTypes.Alchemist:
                case DFLocation.BuildingTypes.Armorer:
                case DFLocation.BuildingTypes.Bookseller:
                case DFLocation.BuildingTypes.ClothingStore:
                case DFLocation.BuildingTypes.FurnitureStore:
                case DFLocation.BuildingTypes.GemStore:
                case DFLocation.BuildingTypes.GeneralStore:
                case DFLocation.BuildingTypes.PawnShop:
                case DFLocation.BuildingTypes.WeaponSmith:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if building is a shop which offers repair service.
        /// </summary>
        public static bool IsRepairShop(DFLocation.BuildingTypes buildingType)
        {
            switch (buildingType)
            {
                case DFLocation.BuildingTypes.Armorer:
                case DFLocation.BuildingTypes.GeneralStore:
                case DFLocation.BuildingTypes.WeaponSmith:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if building is a tavern.
        /// </summary>
        public static bool IsTavern(DFLocation.BuildingTypes buildingType) => buildingType == DFLocation.BuildingTypes.Tavern;

        #endregion

        #region Private Methods

        private static void AddModels(
            DaggerfallUnity dfUnity,
            int layoutX,
            int layoutY,
            ref DFBlock blockData,
            out List<StaticDoor> doorsOut,
            out List<StaticBuilding> buildingsOut,
            ModelCombiner combiner = null,
            Transform parent = null)
        {
            ___AddModels.Begin();

            doorsOut = new List<StaticDoor>();
            buildingsOut = new List<StaticBuilding>();

            // Iterate through all subrecords
            int recordCount = 0;
            foreach (DFBlock.RmbSubRecord subRecord in blockData.RmbBlock.SubRecords)
            {
                // Get subrecord transform
                Vector3 subRecordPosition = new Vector3(subRecord.XPos, 0, BlocksFile.RMBDimension - subRecord.ZPos) * MeshReader.GlobalScale;
                Vector3 subRecordRotation = new Vector3(0, -subRecord.YRotation / BlocksFile.RotationDivisor, 0);
                Matrix4x4 subRecordMatrix = Matrix4x4.TRS(subRecordPosition, Quaternion.Euler(subRecordRotation), Vector3.one);

                // Iterate through models in this subrecord
                bool firstModel = true;
                foreach (DFBlock.RmbBlock3dObjectRecord obj in subRecord.Exterior.Block3dObjectRecords)
                {
                    // Get model transform
                    Vector3 modelPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
                    Vector3 modelRotation = new Vector3(-obj.XRotation / BlocksFile.RotationDivisor, -obj.YRotation / BlocksFile.RotationDivisor, -obj.ZRotation / BlocksFile.RotationDivisor);
                    Vector3 modelScale = GetModelScaleVector(obj);
                    Matrix4x4 modelMatrix = subRecordMatrix * Matrix4x4.TRS(modelPosition, Quaternion.Euler(modelRotation), modelScale);

                    // Get model data
                    ModelData modelData;
                    dfUnity.MeshReader.GetModelData(obj.ModelIdNum, out modelData);

                    // Does this model have doors?
                    StaticDoor[] staticDoors = null;
                    if (modelData.Doors != null)
                        staticDoors = GameObjectHelper.GetStaticDoors(ref modelData, blockData.Index, recordCount, modelMatrix);

                    // Store building information for first model of record
                    // First model is main record structure, others are attachments like posts
                    // Only main structure is needed to resolve building after hit-test
                    int buildingKey = 0;
                    if (firstModel)
                    {
                        // Create building key for this record - considered experimental for now
                        buildingKey = BuildingDirectory.MakeBuildingKey((byte)layoutX, (byte)layoutY, (byte)recordCount);

                        StaticBuilding staticBuilding = new StaticBuilding();
                        staticBuilding.modelMatrix = modelMatrix;
                        staticBuilding.recordIndex = recordCount;
                        staticBuilding.centre = new Vector3(modelData.DFMesh.Centre.X, modelData.DFMesh.Centre.Y, modelData.DFMesh.Centre.Z) * MeshReader.GlobalScale;
                        staticBuilding.size = new Vector3(modelData.DFMesh.Size.X, modelData.DFMesh.Size.Y, modelData.DFMesh.Size.Z) * MeshReader.GlobalScale;
                        buildingsOut.Add(staticBuilding);
                        firstModel = false;
                    }

                    bool dontCreateStaticDoors = false;

                    // Import custom GameObject or use Daggerfall Model
                    GameObject go;
                    if (go = MeshReplacement.ImportCustomGameobject(obj.ModelIdNum, parent, modelMatrix))
                    {
                        // Find doors
                        if (staticDoors != null && staticDoors.Length > 0)
                            CustomDoor.InitDoors(go, staticDoors, buildingKey, out dontCreateStaticDoors);
                    }
                    else if (combiner == null || IsCityGate(obj.ModelIdNum) || IsBulletinBoard(obj.ModelIdNum) || PlayerActivate.HasCustomActivation(obj.ModelIdNum))
                        AddStandaloneModel(dfUnity, ref modelData, modelMatrix, parent);
                    else
                        combiner.Add(ref modelData, modelMatrix);

                    if (modelData.Doors != null && !dontCreateStaticDoors)
                        doorsOut.AddRange(staticDoors);
                }

                // Increment record count
                recordCount++;
            }

            ___AddModels.End();
        }

        private static void AddProps(
            DaggerfallUnity dfUnity,
            ref DFBlock blockData,
            out List<StaticDoor> doorsOut,
            ModelCombiner combiner = null,
            Transform parent = null)
        {
            ___AddProps.Begin();

            doorsOut = new List<StaticDoor>();

            // Iterate through all misc records
            foreach (DFBlock.RmbBlock3dObjectRecord obj in blockData.RmbBlock.Misc3dObjectRecords)
            {
                // Get model transform
                Vector3 modelPosition = new Vector3(obj.XPos, -obj.YPos + propsOffsetY, obj.ZPos + BlocksFile.RMBDimension) * MeshReader.GlobalScale;
                Vector3 modelRotation = new Vector3(-obj.XRotation / BlocksFile.RotationDivisor, -obj.YRotation / BlocksFile.RotationDivisor, -obj.ZRotation / BlocksFile.RotationDivisor);
                Vector3 modelScale = GetModelScaleVector(obj);
                Matrix4x4 modelMatrix = Matrix4x4.TRS(modelPosition, Quaternion.Euler(modelRotation), modelScale);

                // Get model data
                ModelData modelData;
                dfUnity.MeshReader.GetModelData(obj.ModelIdNum, out modelData);

                // Does this model have doors?
                if (modelData.Doors != null)
                    doorsOut.AddRange(GameObjectHelper.GetStaticDoors(ref modelData, blockData.Index, 0, modelMatrix));

                // Import custom GameObject
                if (MeshReplacement.ImportCustomGameobject(obj.ModelIdNum, parent, modelMatrix) != null)
                    continue;

                // Use Daggerfall Model
                // Add or combine
                if (combiner == null || IsBulletinBoard(obj.ModelIdNum) || PlayerActivate.HasCustomActivation(obj.ModelIdNum))
                {
                    AddStandaloneModel(dfUnity, ref modelData, modelMatrix, parent);
                }
                else
                {
                    combiner.Add(ref modelData, modelMatrix);
                }
            }

            ___AddProps.End();
        }

        private static GameObject AddStandaloneModel(
            DaggerfallUnity dfUnity,
            ref ModelData modelData,
            Matrix4x4 matrix,
            Transform parent)
        {
            ___AddStandaloneModel.Begin();

            uint modelID = (uint)modelData.DFMesh.ObjectId;

            // Add GameObject
            GameObject go = GameObjectHelper.CreateDaggerfallMeshGameObject(modelID, parent, dfUnity.Option_SetStaticFlags);
            go.transform.position = matrix.GetColumn(3);
            go.transform.rotation = matrix.rotation;
            go.transform.localScale = matrix.lossyScale;

            // Is this a city gate?
            if (IsCityGate(modelID))
            {
                go.AddComponent<DaggerfallCityGate>();
            }

            // Is this a bulletin board?
            if (IsBulletinBoard(modelID))
            {
                go.AddComponent<DaggerfallBulletinBoard>();
            }

            ___AddStandaloneModel.End();
            return go;
        }

        private static void AddLight(DaggerfallUnity dfUnity, DFBlock.RmbBlockFlatObjectRecord obj, Transform parent = null)
        {
            if (dfUnity.Option_CityLightPrefab == null)
                return;

            Vector2 size = dfUnity.MeshReader.GetScaledBillboardSize(210, obj.TextureRecord);
            Vector3 position = new Vector3(
                obj.XPos,
                -obj.YPos + size.y,
                obj.ZPos + BlocksFile.RMBDimension) * MeshReader.GlobalScale;

            GameObjectHelper.InstantiatePrefab(dfUnity.Option_CityLightPrefab.gameObject, string.Empty, parent, position);
        }

        private static void AddStaticDoors(StaticDoor[] doors, GameObject target)
        {
            DaggerfallStaticDoors c = target.GetComponent<DaggerfallStaticDoors>();
            if (c == null)
                c = target.AddComponent<DaggerfallStaticDoors>();
            if (doors != null && target != null)
                c.Doors = doors;
        }

        private static void AddStaticBuildings(StaticBuilding[] buildings, GameObject target)
        {
            DaggerfallStaticBuildings c = target.GetComponent<DaggerfallStaticBuildings>();
            if (c == null)
                c = target.AddComponent<DaggerfallStaticBuildings>();
            if (buildings != null && target != null)
                c.Buildings = buildings;
        }

        private static bool IsCityGate(uint modelID)
        {
            // Two variants of City Gate model known
            return modelID == CityGateOpenModelID || modelID == CityGateClosedModelID;
        }

        private static bool IsBulletinBoard(uint modelID)
        {
            // Only a single variant of Bulletin Board model known
            return modelID == BulletinBoardModelID;
        }


        #endregion
    }
}
