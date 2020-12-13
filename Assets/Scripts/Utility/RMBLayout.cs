// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game;

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
        public const uint BulletinBoardModelID = 41739;

        // Animal sounds range. Matched to classic.
        const float animalSoundMaxDistance = 768 * MeshReader.GlobalScale;

        #region Structures

        class BuildingPoolItem
        {
            public DFLocation.BuildingData buildingData;
            public bool used;
        }

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

                    // Get Archive
                    int natureArchive = ClimateSwaps.GetNatureArchive(climateNature, climateSeason);

                    // Import custom 3d gameobject instead of flat
                    if (MeshReplacement.ImportCustomFlatGameobject(natureArchive, scenery.TextureRecord, billboardPosition, flatsParent) != null)
                        continue;

                    // Add billboard to batch or standalone
                    if (billboardBatch != null)
                    {
                        billboardBatch.AddItem(scenery.TextureRecord, billboardPosition);
                    }
                    else
                    {
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

                    // Import custom 3d gameobject instead of flat
                    if (MeshReplacement.ImportCustomFlatGameobject(obj.TextureArchive, obj.TextureRecord, billboardPosition, flatsParent) != null)
                        continue;

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

                // Import custom 3d gameobject instead of flat
                if (MeshReplacement.ImportCustomFlatGameobject(obj.TextureArchive, obj.TextureRecord, billboardPosition, flatsParent) != null)
                    continue;

                //// Use misc billboard atlas where available
                //if (miscBillboardsAtlas != null && miscBillboardsBatch != null)
                //{
                //    TextureAtlasBuilder.AtlasItem item = miscBillboardsAtlas.GetAtlasItem(obj.TextureArchive, obj.TextureRecord);
                //    if (item.key != -1)
                //    {
                //        miscBillboardsBatch.AddItem(item.rect, item.textureItem.size, item.textureItem.scale, billboardPosition);
                //        continue;
                //    }
                //}

                // Add to batch where available
                //if (obj.TextureArchive == TextureReader.AnimalsTextureArchive && animalsBillboardBatch != null)
                //{
                //    animalsBillboardBatch.AddItem(obj.TextureRecord, billboardPosition);
                //    continue;
                //}

                // Add standalone billboard gameobject
                GameObject go = GameObjectHelper.CreateDaggerfallBillboardGameObject(obj.TextureArchive, obj.TextureRecord, flatsParent);
                go.transform.position = billboardPosition;
                AlignBillboardToBase(go);

                // Add animal sound
                if (obj.TextureArchive == TextureReader.AnimalsTextureArchive)
                    AddAnimalAudioSource(go);

                // If flat record has a non-zero faction id, then it's an exterior NPC
                if (obj.FactionID != 0)
                {
                    // Add RMB data to billboard
                    DaggerfallBillboard dfBillboard = go.GetComponent<DaggerfallBillboard>();
                    dfBillboard.SetRMBPeopleData(obj.FactionID, obj.Flags, obj.Position);

                    // Add StaticNPC behaviour
                    StaticNPC npc = go.AddComponent<StaticNPC>();
                    npc.SetLayoutData(obj);
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
                GameObjectHelper.TagStaticGeometry(go);

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
        public static BuildingSummary[] GetBuildingData(DFBlock blockData, int layoutX = -1, int layoutY = -1)
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
                buildings[i].buildingKey = BuildingDirectory.MakeBuildingKey((byte)layoutX, (byte)layoutY, (byte)i);
                buildings[i].NameSeed = buildingData.NameSeed;
                buildings[i].FactionId = buildingData.FactionId;
                buildings[i].BuildingType = buildingData.BuildingType;
                buildings[i].Quality = buildingData.Quality;

                // Set building transform info
                DFBlock.RmbSubRecord subRecord = blockData.RmbBlock.SubRecords[i];
                buildings[i].Position = new Vector3(subRecord.XPos, 0, BlocksFile.RMBDimension - subRecord.ZPos) * MeshReader.GlobalScale;
                buildings[i].Rotation = new Vector3(0, -subRecord.YRotation / BlocksFile.RotationDivisor, 0);
                buildings[i].Matrix = Matrix4x4.TRS(buildings[i].Position, Quaternion.Euler(buildings[i].Rotation), Vector3.one);

                // First model of record is building model
                if (subRecord.Exterior.Block3dObjectRecords.Length > 0)
                {
                    buildings[i].ModelID = subRecord.Exterior.Block3dObjectRecords[0].ModelIdNum;
                }
            }

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
        /// <param name="blocksOut">Array of blocks populated with data from MAPS.BSA.</param>
        public static void GetLocationBuildingData(DFLocation location, out DFBlock[] blocksOut)
        {
            List<BuildingPoolItem> namedBuildingPool = new List<BuildingPoolItem>();
            List<DFBlock> blocks = new List<DFBlock>();

            // Get content reader
            ContentReader contentReader = DaggerfallUnity.Instance.ContentReader;
            if (contentReader == null)
                throw new Exception("GetCompleteBuildingData() could not find ContentReader.");

            // Store named buildings in pool for distribution
            for (int i = 0; i < location.Exterior.BuildingCount; i++)
            {
                DFLocation.BuildingData building = location.Exterior.Buildings[i];
                if (IsNamedBuilding(building.BuildingType))
                {
                    BuildingPoolItem bpi = new BuildingPoolItem();
                    bpi.buildingData = building;
                    bpi.used = false;
                    namedBuildingPool.Add(bpi);
                }
            }

            // Get building summary of all blocks in this location
            int width = location.Exterior.ExteriorData.Width;
            int height = location.Exterior.ExteriorData.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get block name
                    string blockName = contentReader.BlockFileReader.CheckName(contentReader.MapFileReader.GetRmbBlockName(ref location, x, y));

                    // Get block data
                    DFBlock block;
                    if (!contentReader.GetBlock(blockName, out block))
                        throw new Exception("GetCompleteBuildingData() could not read block " + blockName);

                    // Make a copy of the building data array for our block copy since we're modifying it
                    DFLocation.BuildingData[] buildingArray = new DFLocation.BuildingData[block.RmbBlock.FldHeader.BuildingDataList.Length];
                    Array.Copy(block.RmbBlock.FldHeader.BuildingDataList, buildingArray, block.RmbBlock.FldHeader.BuildingDataList.Length);
                    block.RmbBlock.FldHeader.BuildingDataList = buildingArray;

                    // Assign building data for this block
                    BuildingReplacementData buildingReplacementData;
                    for (int i = 0; i < block.RmbBlock.SubRecords.Length; i++)
                    {
                        DFLocation.BuildingData building = block.RmbBlock.FldHeader.BuildingDataList[i];
                        if (IsNamedBuilding(building.BuildingType))
                        {
                            // Try to find next building and merge data
                            BuildingPoolItem item;
                            if (!GetNextBuildingFromPool(namedBuildingPool, building.BuildingType, out item))
                            {
                                Debug.LogFormat("End of city building list reached without finding building type {0} in location {1}.{2}", building.BuildingType, location.RegionName, location.Name);
                            }

                            // Copy found city building data to block level
                            building.NameSeed = item.buildingData.NameSeed;
                            building.FactionId = item.buildingData.FactionId;
                            building.Sector = item.buildingData.Sector;
                            building.LocationId = item.buildingData.LocationId;
                            building.Quality = item.buildingData.Quality;

                            // Check for replacement building data and use it if found
                            if (WorldDataReplacement.GetBuildingReplacementData(blockName, block.Index, i, out buildingReplacementData))
                            {
                                // Use custom building values from replacement data, but only use up pool item if factionId is zero
                                if (buildingReplacementData.FactionId != 0)
                                {
                                    // Don't use up pool item and set factionId from replacement data
                                    item.used = false;
                                    building.FactionId = buildingReplacementData.FactionId;
                                }
                                // Always override type and quality
                                building.BuildingType = (DFLocation.BuildingTypes)buildingReplacementData.BuildingType;
                                building.Quality = buildingReplacementData.Quality;
                            }

                            // Matched to classic: special handling for some Order of the Raven buildings
                            if (block.RmbBlock.FldHeader.OtherNames != null &&
                                block.RmbBlock.FldHeader.OtherNames[i] == "KRAVE01.HS2")
                            {
                                building.BuildingType = DFLocation.BuildingTypes.GuildHall;
                                building.FactionId = 414;
                            }

                            // Set whatever building data we could find
                            block.RmbBlock.FldHeader.BuildingDataList[i] = building;
                        }
                    }

                    // Save block data
                    blocks.Add(block);
                }
            }

            // Send blocks array back to caller
            blocksOut = blocks.ToArray();
        }

        /// <summary>
        /// Draws and consume building from pool.
        /// Checking if buildings are ordered by special type.
        /// </summary>
        static bool GetNextBuildingFromPool(List<BuildingPoolItem> namedBuildingPool, DFLocation.BuildingTypes buildingType, out BuildingPoolItem itemOut)
        {
            itemOut = new BuildingPoolItem();
            for (int i = 0; i < namedBuildingPool.Count; i++)
            {
                if (!namedBuildingPool[i].used && namedBuildingPool[i].buildingData.BuildingType == buildingType)
                {
                    itemOut = namedBuildingPool[i];
                    itemOut.used = true;
                    return true;
                }
            }
            return false;
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
                    {
                        staticDoors = GameObjectHelper.GetStaticDoors(ref modelData, blockData.Index, recordCount, modelMatrix);
                        doorsOut.AddRange(staticDoors);
                    }

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

                    // Import custom GameObject
                    GameObject go;
                    if (go = MeshReplacement.ImportCustomGameobject(obj.ModelIdNum, parent, modelMatrix))
                    {
                        // Find doors
                        if (staticDoors != null && staticDoors.Length > 0)
                            CustomDoor.InitDoors(go, staticDoors, buildingKey);

                        continue;
                    }

                    // Use Daggerfall Model
                    // Add or combine
                    if (combiner == null || IsCityGate(obj.ModelIdNum) || IsBulletinBoard(obj.ModelIdNum) || PlayerActivate.HasCustomActivation(obj.ModelIdNum))
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

        private static void AddAnimalAudioSource(GameObject go)
        {
            DaggerfallAudioSource source = go.AddComponent<DaggerfallAudioSource>();
            source.AudioSource.maxDistance = animalSoundMaxDistance;

            DaggerfallBillboard dfBillboard = go.GetComponent<DaggerfallBillboard>();
            SoundClips sound = SoundClips.None;
            switch (dfBillboard.Summary.Record)
            {
                case 0:
                case 1:
                    sound = SoundClips.AnimalHorse;
                    break;
                case 3:
                case 4:
                    sound = SoundClips.AnimalCow;
                    break;
                case 5:
                case 6:
                    sound = SoundClips.AnimalPig;
                    break;
                case 7:
                case 8:
                    sound = SoundClips.AnimalCat;
                    break;
                case 9:
                case 10:
                    sound = SoundClips.AnimalDog;
                    break;
                default:
                    sound = SoundClips.None;
                    break;
            }

            source.SetSound(sound, AudioPresets.PlayRandomlyIfPlayerNear);
        }

        #endregion
    }
}
