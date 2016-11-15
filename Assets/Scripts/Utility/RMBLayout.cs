// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Helper for laying out RMB (city block) data in scene.
    /// </summary>
    public static class RMBLayout
    {
        public const int RMBTilesPerBlock = 16;
        public const int RMBTilesPerTerrain = 128;
        public const float RMBSide = 4096f * MeshReader.GlobalScale;
        public const float RMBTileSide = 256f * MeshReader.GlobalScale;

        const float propsOffsetY = -4f;
        const float blockFlatsOffsetY = -6f;
        const float natureFlatsOffsetY = -2f;
        public const uint CityGateOpenModelID = 446;
        public const uint CityGateClosedModelID = 447;

        #region Layout Methods

        /// <summary>
        /// Create base RMB block by name.
        /// </summary>
        /// <param name="blockName">Name of block.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(string blockName, DaggerfallRMBBlock cloneFrom = null)
        {
            DFBlock blockData;
            return CreateBaseGameObject(blockName, out blockData, cloneFrom);
        }

        /// <summary>
        /// Create base RMB block by name and get back DFBlock data.
        /// </summary>
        /// <param name="blockName">Name of block.</param>
        /// <param name="blockDataOut">DFBlock data out.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(string blockName, out DFBlock blockDataOut, DaggerfallRMBBlock cloneFrom = null)
        {
            blockDataOut = new DFBlock();

            // Validate
            if (string.IsNullOrEmpty(blockName))
                return null;
            if (!blockName.EndsWith(".RMB", StringComparison.InvariantCultureIgnoreCase))
                return null;
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            // Get block data
            blockDataOut = dfUnity.ContentReader.BlockFileReader.GetBlock(blockName);

            return CreateBaseGameObject(ref blockDataOut, cloneFrom);
        }

        /// <summary>
        /// Instantiate base RMB block by DFBlock data.
        /// </summary>
        /// <param name="blockData">Block data.</param>
        /// <returns>Block GameObject.</returns>
        public static GameObject CreateBaseGameObject(ref DFBlock blockData, DaggerfallRMBBlock cloneFrom = null)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            // Create gameobject
            GameObject go;
            DaggerfallRMBBlock rmbBlock = null;
            string name = string.Format("DaggerfallBlock [{0}]", blockData.Name);
            if (cloneFrom != null)
            {
                go = GameObjectHelper.InstantiatePrefab(cloneFrom.gameObject, name, null, Vector3.zero);
                rmbBlock = go.GetComponent<DaggerfallRMBBlock>();
            }
            else
            {
                go = new GameObject(name);
                rmbBlock = go.AddComponent<DaggerfallRMBBlock>();
            }

            // Attempt to set block data
            // If using a prefab it must have DaggerfallRMBBlock component
            if (rmbBlock)
            {
                rmbBlock.SetBlockData(blockData);
            }

            // Setup combiner
            ModelCombiner combiner = null;
            if (dfUnity.Option_CombineRMB)
                combiner = new ModelCombiner();

            // Lists to receive any doors found in this block
            List<StaticDoor> modelDoors;
            List<StaticDoor> propDoors;

            // Add models and static props
            GameObject modelsNode = new GameObject("Models");
            modelsNode.transform.parent = go.transform;
            AddModels(dfUnity, ref blockData, out modelDoors, combiner, modelsNode.transform);
            AddProps(dfUnity, ref blockData, out propDoors, combiner, modelsNode.transform);

            // Add doors
            List<StaticDoor> allDoors = new List<StaticDoor>();
            if (modelDoors.Count > 0) allDoors.AddRange(modelDoors);
            if (propDoors.Count > 0) allDoors.AddRange(propDoors);
            if (allDoors.Count > 0)
                AddStaticDoors(allDoors.ToArray(), go);

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
        /// Add nature billboards.
        /// </summary>
        public static void AddNatureFlats(
            ref DFBlock blockData,
            Transform flatsParent,
            DaggerfallBillboardBatch billboardBatch = null,
            ClimateNatureSets climateNature = ClimateNatureSets.TemperateWoodland,
            ClimateSeason climateSeason = ClimateSeason.Summer)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    // Get scenery item - ignore indices -1 (empty) and 0 (marker/waypoint of some kind)
                    DFBlock.RmbGroundScenery scenery = blockData.RmbBlock.FldHeader.GroundData.GroundScenery[x, 15 - y];
                    if (scenery.TextureRecord < 1)
                        continue;

                    // Calculate position
                    Vector3 billboardPosition = new Vector3(
                        x * BlocksFile.TileDimension,
                        natureFlatsOffsetY,
                        y * BlocksFile.TileDimension + BlocksFile.TileDimension) * MeshReader.GlobalScale;

                    // Add billboard to batch or standalone
                    if (billboardBatch != null)
                    {
                        billboardBatch.AddItem(scenery.TextureRecord, billboardPosition);
                    }
                    else
                    {
                        int natureArchive = ClimateSwaps.GetNatureArchive(climateNature, climateSeason);
                        GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(natureArchive, scenery.TextureRecord, flatsParent);
                        go.transform.position = billboardPosition;
                        AlignBillboardToBase(go);
                    }
                }
            }
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
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Do nothing if import option not enabled or missing prefab
            if (!dfUnity.Option_ImportLightPrefabs || dfUnity.Option_CityLightPrefab == null)
                return;

            // Iterate block flats for lights
            foreach (DFBlock.RmbBlockFlatObjectRecord obj in blockData.RmbBlock.MiscFlatObjectRecords)
            {
                // Add point lights
                if (obj.TextureArchive == TextureReader.LightsTextureArchive)
                {
                    // Calculate position
                    Vector3 billboardPosition = new Vector3(
                        obj.XPos,
                        -obj.YPos + blockFlatsOffsetY,
                        obj.ZPos + BlocksFile.RMBDimension) * MeshReader.GlobalScale;

                    // Add billboard to batch or standalone
                    if (billboardBatch != null)
                    {
                        billboardBatch.AddItem(obj.TextureRecord, billboardPosition);
                    }
                    else
                    {
                        GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(obj.TextureArchive, obj.TextureRecord, flatsParent);
                        go.transform.position = billboardPosition;
                        AlignBillboardToBase(go);
                    }

                    // Import light prefab
                    AddLight(dfUnity, obj, lightsParent);
                }
            }
        }

        /// <summary>
        /// Add misc block flats.
        /// Batching is conditionally supported.
        /// </summary>
        public static void AddMiscBlockFlats(
            ref DFBlock blockData,
            Transform flatsParent,
            DaggerfallBillboardBatch animalsBillboardBatch = null,
            TextureAtlasBuilder miscBillboardsAtlas = null,
            DaggerfallBillboardBatch miscBillboardsBatch = null)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

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

                // Use misc billboard atlas where available
                if (miscBillboardsAtlas != null && miscBillboardsBatch != null)
                {
                    TextureAtlasBuilder.AtlasItem item = miscBillboardsAtlas.GetAtlasItem(obj.TextureArchive, obj.TextureRecord);
                    if (item.key != -1)
                    {
                        miscBillboardsBatch.AddItem(item.rect, item.textureItem.size, item.textureItem.scale, billboardPosition);
                        continue;
                    }
                }

                // Add to batch where available
                if (obj.TextureArchive == TextureReader.AnimalsTextureArchive && animalsBillboardBatch != null)
                {
                    animalsBillboardBatch.AddItem(obj.TextureRecord, billboardPosition);
                }
                else
                {
                    // Add standalone billboard gameobject
                    GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(obj.TextureArchive, obj.TextureRecord, flatsParent);
                    go.transform.position = billboardPosition;
                    AlignBillboardToBase(go);
                }
            }
        }

        /// <summary>
        /// Aligns billboard GameObject to centre of base.
        /// This is required for exterior billboard.
        /// </summary>
        /// <param name="go">GameObject with DaggerfallBillboard component.</param>
        public static void AlignBillboardToBase(GameObject go)
        {
            DaggerfallBillboard c = go.GetComponent<DaggerfallBillboard>();
            if (c)
            {
                c.AlignToBase();
            }
        }

        /// <summary>
        /// Add simple ground plane to block layout.
        /// </summary>
        public static GameObject AddGroundPlane(
            ref DFBlock blockData,
            Transform parent = null,
            ClimateBases climateBase = ClimateBases.Temperate,
            ClimateSeason climateSeason = ClimateSeason.Summer)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

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
                go.isStatic = true;

            return go;
        }

        #endregion

        #region Building Methods

        /// <summary>
        /// Information about buildings in this block.
        /// This is a trimmed-down version of DFLocation.BuildingData with some extra information for scene builders.
        /// Notes:
        /// -Daggerfall keeps a base building template in block data specifying generic information like building type.
        /// -This building template is then merged with building data from location data to create unique buildings for each location.
        /// -Which prevents any individual block (reused many hundreds of times across world) from always having same building names.
        /// -The way Daggerfall links location building data with block building data is not 100% known.
        /// -Noted is that special buildings (taverns, shops, temples, etc.) seem to be laid out in same sequential order in blocks and locations.
        /// -So linking could simply be done by sequence, which may explain why Daggerfall can exhibit linking errors (e.g. taverns become residences).
        /// -At this time, building data from location is not merged with with block building data.
        /// -Current implementation is primarily to support exterior automap progress. May be moved to a different setup later.
        /// </summary>
        [Serializable]
        public struct BuildingSummary
        {
            public int NameSeed;                                // Name seed of building - not set at block level
            public int FactionId;                               // Faction ID of building
            public int LocationId;                              // Unique location ID - not set at block level
            public DFLocation.BuildingTypes BuildingType;       // Type of building
            public int Quality;                                 // Quality of building
            public Vector3 Position;                            // Position of building
            public Vector3 Rotation;                            // Rotation of building
            public Matrix4x4 Matrix;                            // Transform matrix of building
            public float Radius;                                // Radius of building
        }

        /// <summary>
        /// Gets BuildingSummary array generated from DFBlock data.
        /// </summary>
        /// <param name="blockData"></param>
        /// <returns></returns>
        public static BuildingSummary[] GetBuildingData(DFBlock blockData)
        {
            // Store building information
            int buildingCount = blockData.RmbBlock.SubRecords.Length;
            BuildingSummary[] buildings = new BuildingSummary[buildingCount];
            for (int i = 0; i < buildingCount; i++)
            {
                // Create building summary
                buildings[i] = new BuildingSummary();

                // Set building data
                DFLocation.BuildingData buildingData = blockData.RmbBlock.FldHeader.BuildingDataList[i];
                buildings[i].NameSeed = buildingData.NameSeed;
                buildings[i].FactionId = buildingData.FactionId;
                buildings[i].LocationId = buildingData.LocationId;
                buildings[i].BuildingType = buildingData.BuildingType;
                buildings[i].Quality = buildingData.Quality;

                // Set building transform info
                DFBlock.RmbSubRecord subRecord = blockData.RmbBlock.SubRecords[i];
                buildings[i].Position = new Vector3(subRecord.XPos, 0, BlocksFile.RMBDimension - subRecord.ZPos) * MeshReader.GlobalScale;
                buildings[i].Rotation = new Vector3(0, -subRecord.YRotation / BlocksFile.RotationDivisor, 0);
                buildings[i].Matrix = Matrix4x4.TRS(buildings[i].Position, Quaternion.Euler(buildings[i].Rotation), Vector3.one);
            }

            return buildings;
        }

        #endregion

        #region Private Methods

        private static void AddModels(
            DaggerfallUnity dfUnity,
            ref DFBlock blockData,
            out List<StaticDoor> doorsOut,
            ModelCombiner combiner = null,
            Transform parent = null)
        {
            doorsOut = new List<StaticDoor>();

            // Iterate through all subrecords
            int recordCount = 0;
            foreach (DFBlock.RmbSubRecord subRecord in blockData.RmbBlock.SubRecords)
            {
                // Get subrecord transform
                Vector3 subRecordPosition = new Vector3(subRecord.XPos, 0, BlocksFile.RMBDimension - subRecord.ZPos) * MeshReader.GlobalScale;
                Vector3 subRecordRotation = new Vector3(0, -subRecord.YRotation / BlocksFile.RotationDivisor, 0);
                Matrix4x4 subRecordMatrix = Matrix4x4.TRS(subRecordPosition, Quaternion.Euler(subRecordRotation), Vector3.one);

                // Iterate through models in this subrecord
                foreach (DFBlock.RmbBlock3dObjectRecord obj in subRecord.Exterior.Block3dObjectRecords)
                {
                    // Get model transform
                    Vector3 modelPosition = new Vector3(obj.XPos, -obj.YPos, obj.ZPos) * MeshReader.GlobalScale;
                    Vector3 modelRotation = new Vector3(0, -obj.YRotation / BlocksFile.RotationDivisor, 0);
                    Matrix4x4 modelMatrix = subRecordMatrix * Matrix4x4.TRS(modelPosition, Quaternion.Euler(modelRotation), Vector3.one);

                    // Get model data
                    ModelData modelData;
                    dfUnity.MeshReader.GetModelData(obj.ModelIdNum, out modelData);

                    // Does this model have doors?
                    if (modelData.Doors != null)
                        doorsOut.AddRange(GameObjectHelper.GetStaticDoors(ref modelData, blockData.Index, recordCount, modelMatrix));

                    // Add or combine
                    if (combiner == null || IsCityGate(obj.ModelIdNum))
                        AddStandaloneModel(dfUnity, ref modelData, modelMatrix, parent);
                    else
                        combiner.Add(ref modelData, modelMatrix);
                }

                // Increment record count
                recordCount++;
            }
        }

        private static void AddProps(
            DaggerfallUnity dfUnity,
            ref DFBlock blockData,
            out List<StaticDoor> doorsOut,
            ModelCombiner combiner = null,
            Transform parent = null)
        {
            doorsOut = new List<StaticDoor>();

            // Iterate through all misc records
            foreach (DFBlock.RmbBlock3dObjectRecord obj in blockData.RmbBlock.Misc3dObjectRecords)
            {
                // Get model transform
                Vector3 modelPosition = new Vector3(obj.XPos, -obj.YPos + propsOffsetY, obj.ZPos + BlocksFile.RMBDimension) * MeshReader.GlobalScale;
                Vector3 modelRotation = new Vector3(0, -obj.YRotation / BlocksFile.RotationDivisor, 0);
                Matrix4x4 modelMatrix = Matrix4x4.TRS(modelPosition, Quaternion.Euler(modelRotation), Vector3.one);

                // Get model data
                ModelData modelData;
                dfUnity.MeshReader.GetModelData(obj.ModelIdNum, out modelData);

                // Does this model have doors?
                if (modelData.Doors != null)
                    doorsOut.AddRange(GameObjectHelper.GetStaticDoors(ref modelData, blockData.Index, 0, modelMatrix));

                // Add or combine
                if (combiner == null)
                    AddStandaloneModel(dfUnity, ref modelData, modelMatrix, parent);
                else
                    combiner.Add(ref modelData, modelMatrix);
            }
        }

        private static GameObject AddStandaloneModel(
            DaggerfallUnity dfUnity,
            ref ModelData modelData,
            Matrix4x4 matrix,
            Transform parent)
        {
            uint modelID = (uint)modelData.DFMesh.ObjectId;

            // Add GameObject
            GameObject go = GameObjectHelper.CreateDaggerfallMeshGameObject(modelID, parent, dfUnity.Option_SetStaticFlags);
            go.transform.position = matrix.GetColumn(3);
            go.transform.rotation = GameObjectHelper.QuaternionFromMatrix(matrix);

            // Is this a city gate?
            if (IsCityGate(modelID))
            {
                DaggerfallCityGate gate = go.AddComponent<DaggerfallCityGate>();
                gate.SetOpen(!dfUnity.Option_CloseCityGates);
            }

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

        private static bool IsCityGate(uint modelID)
        {
            if (modelID == CityGateOpenModelID || modelID == CityGateClosedModelID)
                return true;
            else
                return false;
        }

        #endregion
    }
}