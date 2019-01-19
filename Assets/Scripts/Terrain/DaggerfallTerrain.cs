// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using Unity.Collections;
using Unity.Jobs;
using DaggerfallConnect.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Partners with a Unity Terrain for use by StreamingWorld.
    /// Each terrain is "self-assembling" based on position in world (1000x500 map pixels).
    /// Also serializes additional information about neighbour terrains.
    /// </summary>
    [RequireComponent(typeof(Terrain))]
    [RequireComponent(typeof(TerrainCollider))]
    public class DaggerfallTerrain : MonoBehaviour
    {
        // Settings are tuned for Daggerfall and fast procedural layout
        const int tilemapDimension = MapsFile.WorldMapTileDim;
        const int resolutionPerPatch = 16;

        const byte avgHeightIdx = 0;
        const byte maxHeightIdx = 1;
        private const int rotBit = 0x40;
        private const int flipBit = 0x80;

        // This controls which map pixel the terrain will represent
        [Range(TerrainHelper.minMapPixelX, TerrainHelper.maxMapPixelX)]
        public int MapPixelX = TerrainHelper.defaultMapPixelX;
        [Range(TerrainHelper.minMapPixelY, TerrainHelper.maxMapPixelY)]
        public int MapPixelY = TerrainHelper.defaultMapPixelY;

        // Increasing scale will amplify terrain height
        // Must be set per terrain for correct tiling
        [Range(TerrainHelper.minTerrainScale, TerrainHelper.maxTerrainScale)]
        public float TerrainScale = TerrainHelper.defaultTerrainScale;

        // Data for this terrain
        public MapPixelData MapData;

        // Neighbours of this terrain
        public Terrain LeftNeighbour;
        public Terrain TopNeighbour;
        public Terrain RightNeighbour;
        public Terrain BottomNeighbour;

        // The tile map
        [NonSerialized]
        public Color32[] TileMap;
        [NonSerialized]
        NativeArray<Color32> tileMap;

        // Required for material properties
        [SerializeField, HideInInspector]
        Texture2D tileMapTexture;
        [SerializeField, HideInInspector]
        Material terrainMaterial;

        public Material TerrainMaterial { get { return terrainMaterial; } set { terrainMaterial = value; } }

        DaggerfallUnity dfUnity;
        int currentWorldClimate = -1;
        DaggerfallDateTime.Seasons season = DaggerfallDateTime.Seasons.Summer;
        bool ready;

        private float heightMapPixelError = 5; // just a default value in case ini value reading fails (so value will be overwritten by ini file value)

        public float HeightMapPixelError
        {
            get { return heightMapPixelError; }
            set { heightMapPixelError = value; }
        }


        void Awake()
        {
            HeightMapPixelError = DaggerfallUnity.Settings.TerrainHeightmapPixelError;
        }

        void Start()
        {
            UpdateNeighbours();
            ready = false;
        }

        // Update data for terrain using jobs system.
        public void UpdateTerrainData(bool init, TerrainTexturingJobs terrainTexturingJobs)
        {
            // Get basic terrain data.
            MapData = TerrainHelper.GetMapPixelData(dfUnity.ContentReader, MapPixelX, MapPixelY);

            // Create data array for heightmap.
            int hDim = dfUnity.TerrainSampler.HeightmapDimension;
            MapData.heightmapData = new NativeArray<float>(hDim * hDim, Allocator.Persistent);
            // Create data array for tilemap.
            int tDim = MapsFile.WorldMapTileDim;
            MapData.tilemapData = new NativeArray<byte>(tDim * tDim, Allocator.Persistent);
            // Create data array for average & max heights.
            MapData.avgMaxHeight = new NativeArray<float>(new float[] { 0, float.MinValue }, Allocator.TempJob);
            // Create data array for shader tile map data.
            tileMap = new NativeArray<Color32>(tilemapDimension * tilemapDimension, Allocator.Persistent);

            // Generate heightmap samples. (returns when complete)
            dfUnity.TerrainSampler.GenerateSamplesJobs(ref MapData);

            // Handle location if one is present on terrain.
            JobHandle blendLocationTerrainJobHandle = new JobHandle();
            if (MapData.hasLocation)
            {
                // Schedule job to calc average & max heights.
                JobHandle calcAvgMaxHeightJobHandle = ScheduleCalcAvgMaxHeightJob();
                JobHandle.ScheduleBatchedJobs();

                // Set location tiles.
                TerrainHelper.SetLocationTilesJobs(ref MapData);

                // Schedule job to blend and flatten location heights. (depends on SetLocationTiles being done first)
                blendLocationTerrainJobHandle = ScheduleBlendLocationTerrainJob(calcAvgMaxHeightJobHandle);
            }

            // Assign tiles for terrain texturing.
            JobHandle assignTilesJobHandle = new JobHandle();
            if (terrainTexturingJobs != null)
            {
                assignTilesJobHandle = terrainTexturingJobs.ScheduleAssignTilesJob(dfUnity.TerrainSampler, ref MapData, blendLocationTerrainJobHandle);
                assignTilesJobHandle.Complete();
            }

            // Update tile map for shader
            UpdateTileMapDataJobs();


            // Convert back to standard managed 2d arrays
            MapData.heightmapSamples = new float[hDim, hDim];
            for (int i = 0; i < MapData.heightmapData.Length; i++)
                MapData.heightmapSamples[JobA.Row(i, hDim), JobA.Col(i, hDim)] = MapData.heightmapData[i];

            MapData.tilemapSamples = new TilemapSample[tDim, tDim];
            for (int i = 0; i < MapData.tilemapData.Length; i++)
            {
                byte tile = MapData.tilemapData[i];
                if (tile == byte.MaxValue)
                    tile = 0;
                MapData.tilemapSamples[JobA.Row(i, tDim), JobA.Col(i, tDim)] = new TilemapSample()
                {
                    record = tile & 0x3f,
                    rotate = (tile & rotBit) != 0,
                    flip = (tile & flipBit) != 0,
                };
            }
            // TODO: Are these needed? Seem to not be used anywhere
            MapData.averageHeight = MapData.avgMaxHeight[avgHeightIdx];
            MapData.maxHeight = MapData.avgMaxHeight[maxHeightIdx];

            // Dispose native array memory allocationsnow data has been extracted.
            MapData.heightmapData.Dispose();
            MapData.tilemapData.Dispose();
            MapData.avgMaxHeight.Dispose();
            if (terrainTexturingJobs != null)
                terrainTexturingJobs.Dispose();

            //UpdateTileMapData();
        }

        JobHandle ScheduleCalcAvgMaxHeightJob()
        {
            CalcAvgMaxHeightJob calcAvgMaxHeightJob = new CalcAvgMaxHeightJob()
            {
                heightmapData = MapData.heightmapData,
                avgMaxHeight = MapData.avgMaxHeight,
            };
            return calcAvgMaxHeightJob.Schedule();
        }

        JobHandle ScheduleBlendLocationTerrainJob(JobHandle dependencies)
        {
            BlendLocationTerrainJob blendLocationTerrainJob = new BlendLocationTerrainJob()
            {
                heightmapData = MapData.heightmapData,
                avgMaxHeight = MapData.avgMaxHeight,
                hDim = DaggerfallUnity.Instance.TerrainSampler.HeightmapDimension,
                locationRect = MapData.locationRect,
            };
            return blendLocationTerrainJob.Schedule(dependencies);
        }

        /// <summary>
        /// Update tile map based on current samples.
        /// </summary>
        public void UpdateTileMapDataJobs()
        {
            int tDim = tilemapDimension;
            // Assign tile data to tilemap
            Color32 tileColor = new Color32(0, 0, 0, 0);
            for (int y = 0; y < tilemapDimension; y++)
            {
                for (int x = 0; x < tilemapDimension; x++)
                {
                    // Get sample tile data
                    byte tile = MapData.tilemapData[JobA.Idx(x, y, tDim)];

                    // Convert from [flip,rotate,6bit-record] => [6bit-record,flip,rotate]
                    // TODO: test speed difference with cast here:
                    int record = tile * 4;
                    if ((tile & rotBit) != 0) record += 1;
                    if ((tile & flipBit) != 0) record += 2;

                    // Assign to tileMap
                    tileColor.r = (byte) record;
                    tileMap[y * tilemapDimension + x] = tileColor;
                }
            }
            // Create tileMap array or resize if needed and copy native array
            if (TileMap == null || TileMap.Length != tileMap.Length)
                TileMap = new Color32[tileMap.Length];

            tileMap.CopyTo(TileMap);
        }


        /* Cannot jobify because data structs used in jobs cannot contain ref types like bool or string!

                        SetLocationTilesJob InitSetLocationTilesJob()
                        {
                            // Get location
                            DFLocation location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(MapData.mapRegionIndex, MapData.mapLocationIndex);
                            // Position tiles inside terrain area
                            DFPosition tilePos = TerrainHelper.GetLocationTerrainTileOrigin(location);

                            return new SetLocationTilesJob()
                            {
                                tilemapData = MapData.tilemapData,
                                locationRectData = MapData.locationRectData,
                                location = location,
                                tilePos = tilePos,
                            };
                        }

                        struct SetLocationTilesJob : IJob
                        {
                            [WriteOnly]
                            public NativeArray<byte> tilemapData;
                            [WriteOnly]
                            public NativeArray<Rect> locationRectData;

                            public DFLocation location;
                            public DFPosition tilePos;

                            public void Execute()
                            {
                                // Full 8x8 locations have "terrain blend space" around walls to smooth down random terrain towards flat area.
                                // This is indicated by texture index > 55 (ground texture range is 0-55), larger values indicate blend space.
                                // We need to know rect of actual city area so we can use blend space outside walls.
                                int xmin = int.MaxValue, ymin = int.MaxValue;
                                int xmax = 0, ymax = 0;

                                // Iterate blocks of this location
                                for (int blockY = 0; blockY < location.Exterior.ExteriorData.Height; blockY++)
                                {
                                    for (int blockX = 0; blockX < location.Exterior.ExteriorData.Width; blockX++)
                                    {
                                        // Get block data
                                        DFBlock block;
                                        string blockName = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRmbBlockName(ref location, blockX, blockY);
                                        if (!DaggerfallUnity.Instance.ContentReader.GetBlock(blockName, out block))
                                            continue;

                                        // Copy ground tile info
                                        for (int tileY = 0; tileY < RMBLayout.RMBTilesPerBlock; tileY++)
                                        {
                                            for (int tileX = 0; tileX < RMBLayout.RMBTilesPerBlock; tileX++)
                                            {
                                                DFBlock.RmbGroundTiles tile = block.RmbBlock.FldHeader.GroundData.GroundTiles[tileX, (RMBLayout.RMBTilesPerBlock - 1) - tileY];
                                                int xpos = tilePos.X + blockX * RMBLayout.RMBTilesPerBlock + tileX;
                                                int ypos = tilePos.Y + blockY * RMBLayout.RMBTilesPerBlock + tileY;

                                                int record = tile.TextureRecord;
                                                if (tile.TextureRecord < 56)
                                                {
                                                    // Track interior bounds of location tiled area
                                                    if (xpos < xmin) xmin = xpos;
                                                    if (xpos > xmax) xmax = xpos;
                                                    if (ypos < ymin) ymin = ypos;
                                                    if (ypos > ymax) ymax = ypos;

                                                    // Store texture data from block
                                                    tilemapData[JobA.Idx(xpos, ypos, MapsFile.WorldMapTileDim)] = tile.TileBitfield == 0 ? byte.MaxValue : tile.TileBitfield;
                                                }
                                            }
                                        }
                                    }
                                }
                                // Update location rect with extra clearance
                                const int extraClearance = 2;
                                Rect locRect = new Rect();
                                locRect.xMin = xmin - extraClearance;
                                locRect.xMax = xmax + extraClearance;
                                locRect.yMin = ymin - extraClearance;
                                locRect.yMax = ymax + extraClearance;
                                locationRectData[0] = locRect;
                            }
                        }
                        */

        #region Terrain Jobs

        struct CalcAvgMaxHeightJob : IJob
        {
            [ReadOnly]
            public NativeArray<float> heightmapData;

            public NativeArray<float> avgMaxHeight;

            public void Execute()
            {
                for (int i = 0; i < heightmapData.Length; i++)
                {
                    float height = heightmapData[i];
                    // Accumulate average height
                    avgMaxHeight[avgHeightIdx] += height;
                    // Update max height
                    if (height > avgMaxHeight[maxHeightIdx])
                        avgMaxHeight[maxHeightIdx] = height;
                }
                avgMaxHeight[avgHeightIdx] = avgMaxHeight[avgHeightIdx] / heightmapData.Length;
            }
        }

        // Flattens location terrain and blends with surrounding terrain
        struct BlendLocationTerrainJob : IJob
        {
            public NativeArray<float> heightmapData;
            [ReadOnly]
            public NativeArray<float> avgMaxHeight;

            public int hDim;
            public Rect locationRect;

            public void Execute()
            {
                // Convert from rect in tilemap space to interior corners in 0-1 range
                float xMin = locationRect.xMin / MapsFile.WorldMapTileDim;
                float xMax = locationRect.xMax / MapsFile.WorldMapTileDim;
                float yMin = locationRect.yMin / MapsFile.WorldMapTileDim;
                float yMax = locationRect.yMax / MapsFile.WorldMapTileDim;

                // Scale values for converting blend space into 0-1 range
                float leftScale = 1 / xMin;
                float rightScale = 1 / (1 - xMax);
                float topScale = 1 / yMin;
                float bottomScale = 1 / (1 - yMax);

                // Flatten location area and blend with surrounding heights
                float strength = 0;
                float targetHeight = avgMaxHeight[avgHeightIdx];
                for (int y = 0; y < hDim; y++)
                {
                    float v = (float)y / (float)(hDim - 1);
                    bool insideY = (v >= yMin && v <= yMax);

                    for (int x = 0; x < hDim; x++)
                    {
                        float u = (float)x / (float)(hDim - 1);
                        bool insideX = (u >= xMin && u <= xMax);


                        if (insideX || insideY)
                        {
                            if (insideY && u <= xMin)
                                strength = u * leftScale;
                            else if (insideY && u >= xMax)
                                strength = (1 - u) * rightScale;
                            else if (insideX && v <= yMin)
                                strength = v * topScale;
                            else if (insideX && v >= yMax)
                                strength = (1 - v) * bottomScale;
                        }
                        else
                        {
                            float xs = 0, ys = 0;
                            if (u <= xMin) xs = u * leftScale; else if (u >= xMax) xs = (1 - u) * rightScale;
                            if (v <= yMin) ys = v * topScale; else if (v >= yMax) ys = (1 - v) * bottomScale;
                            strength = TerrainHelper.BilinearInterpolator(0, 0, 0, 1, xs, ys);
                        }

                        int idx = JobA.Idx(x, y, hDim);
                        float height = heightmapData[idx];

                        if (insideX && insideY)
                            height = targetHeight;
                        else
                            height = Mathf.Lerp(height, targetHeight, strength);

                        heightmapData[idx] = height;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// This must be called when first creating terrain or before updating terrain.
        /// Safe to call multiple times. Recreates expired volatile objects on subsequent calls.
        /// </summary>
        public void InstantiateTerrain()
        {
            if (!ReadyCheck())
                return;

            // Create tileMap texture
            if (tileMapTexture == null)
            {
                tileMapTexture = new Texture2D(tilemapDimension, tilemapDimension, TextureFormat.ARGB32, false);
                tileMapTexture.filterMode = FilterMode.Point;
                tileMapTexture.wrapMode = TextureWrapMode.Clamp;
            }

            // Create terrain material
            if (terrainMaterial == null)
            {
                if ((SystemInfo.supports2DArrayTextures) && DaggerfallUnity.Settings.EnableTextureArrays)
                {
                    terrainMaterial = new Material(Shader.Find(MaterialReader._DaggerfallTilemapTextureArrayShaderName));
                }
                else
                {
                    terrainMaterial = new Material(Shader.Find(MaterialReader._DaggerfallTilemapShaderName));
                }
                UpdateClimateMaterial();
            }

            // Raise event
            RaiseOnInstantiateTerrainEvent();
        }

        /// <summary>
        /// Updates climate material based on current map pixel data.
        /// </summary>
        public void UpdateClimateMaterial(bool init = false)
        {
            // Update atlas texture if world climate changed
            if (currentWorldClimate != MapData.worldClimate || dfUnity.WorldTime.Now.SeasonValue != season || init)
            {
                // Get current climate and ground archive
                DFLocation.ClimateSettings climate = MapsFile.GetWorldClimateSettings(MapData.worldClimate);
                int groundArchive = climate.GroundArchive;
                if (climate.ClimateType != DFLocation.ClimateBaseType.Desert &&
                    dfUnity.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
                {
                    // Offset to snow textures
                    groundArchive++;
                }

                if ((SystemInfo.supports2DArrayTextures) && DaggerfallUnity.Settings.EnableTextureArrays)
                {
                    Material tileMaterial = dfUnity.MaterialReader.GetTerrainTextureArrayMaterial(groundArchive);
                    currentWorldClimate = MapData.worldClimate;

                    // Assign textures (propagate material settings from tileMaterial to terrainMaterial)
                    terrainMaterial.SetTexture(TileTexArrUniforms.TileTexArr, tileMaterial.GetTexture(TileTexArrUniforms.TileTexArr));
                    terrainMaterial.SetTexture(TileTexArrUniforms.TileNormalMapTexArr, tileMaterial.GetTexture(TileTexArrUniforms.TileNormalMapTexArr));
                    if (tileMaterial.IsKeywordEnabled(KeyWords.NormalMap))
                        terrainMaterial.EnableKeyword(KeyWords.NormalMap);
                    else
                        terrainMaterial.DisableKeyword(KeyWords.NormalMap);
                    terrainMaterial.SetTexture(TileTexArrUniforms.TileMetallicGlossMapTexArr, tileMaterial.GetTexture(TileTexArrUniforms.TileMetallicGlossMapTexArr));
                    terrainMaterial.SetTexture(TileTexArrUniforms.TilemapTex, tileMapTexture);
                }
                else
                {
                    // Get tileset material to "steal" atlas texture for our shader
                    // TODO: Improve material system to handle custom shaders
                    Material tileSetMaterial = dfUnity.MaterialReader.GetTerrainTilesetMaterial(groundArchive);
                    currentWorldClimate = MapData.worldClimate;

                    // Assign textures
                    terrainMaterial.SetTexture(TileUniforms.TileAtlasTex, tileSetMaterial.GetTexture(TileUniforms.TileAtlasTex));
                    terrainMaterial.SetTexture(TileUniforms.TilemapTex, tileMapTexture);
                    terrainMaterial.SetInt(TileUniforms.TilemapDim, tilemapDimension);
                }
            }
        }

        /// <summary>
        /// Updates map pixel data based on current coordinates.
        /// Must be called before other data update methods.
        /// </summary>
        public void UpdateMapPixelData(TerrainTexturing terrainTexturing = null)
        {
            if (!ReadyCheck())
                return;

            //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Get basic terrain data
            MapData = TerrainHelper.GetMapPixelData(dfUnity.ContentReader, MapPixelX, MapPixelY);
            dfUnity.TerrainSampler.GenerateSamples(ref MapData);

            // Handle terrain with location
            if (MapData.hasLocation)
            {
                TerrainHelper.SetLocationTiles(ref MapData);
                TerrainHelper.BlendLocationTerrain(ref MapData);
            }

            // Set textures
            if (terrainTexturing != null)
            {
                terrainTexturing.AssignTiles(dfUnity.TerrainSampler, ref MapData);
            }

            //stopwatch.Stop();
            //DaggerfallUnity.LogMessage(string.Format("Time to update map pixel data: {0}ms", stopwatch.ElapsedMilliseconds), true);
        }

        /// <summary>
        /// Update tile map based on current samples.
        /// </summary>
        public void UpdateTileMapData()
        {
            // Create tileMap array if not present
            if (TileMap == null)
                TileMap = new Color32[tilemapDimension * tilemapDimension];

            // Also recreate if not sized appropriately
            if (TileMap.Length != tilemapDimension * tilemapDimension)
                TileMap = new Color32[tilemapDimension * tilemapDimension];

            // Assign tile data to tilemap
            Color32 tileColor = new Color32(0, 0, 0, 0);
            for (int y = 0; y < tilemapDimension; y++)
            {
                for (int x = 0; x < tilemapDimension; x++)
                {
                    // Get sample tile data
                    TilemapSample sample = MapData.tilemapSamples[x, y];

                    // Calculate tile index
                    byte record = (byte)(sample.record * 4);
                    if (sample.rotate && !sample.flip)
                        record += 1;
                    if (!sample.rotate && sample.flip)
                        record += 2;
                    if (sample.rotate && sample.flip)
                        record += 3;

                    // Assign to tileMap
                    tileColor.r = record;
                    TileMap[y * tilemapDimension + x] = tileColor;
                }
            }
        }

        /// <summary>
        /// Promote data to live terrain.
        /// This must be called after other processing complete.
        /// </summary>
        public void PromoteTerrainData()
        {
            // Basemap not used and is just pushed far away
            const float basemapDistance = 10000f;
            int heightmapDimension = dfUnity.TerrainSampler.HeightmapDimension;
            int detailResolution = heightmapDimension;

            // Ensure TerrainData is created
            Terrain terrain = GetComponent<Terrain>();
            if (terrain.terrainData == null)
            {
                // Calculate width and length of terrain in world units
                float terrainSize = (MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale);

                // Setup terrain data
                // Must set terrainData.heightmapResolution before size (thanks Nystul!)
                TerrainData terrainData = new TerrainData();
                terrainData.name = "TerrainData";
                terrainData.heightmapResolution = heightmapDimension;
                terrainData.size = new Vector3(terrainSize, dfUnity.TerrainSampler.MaxTerrainHeight, terrainSize);
                terrainData.SetDetailResolution(detailResolution, resolutionPerPatch);
                terrainData.alphamapResolution = detailResolution;
                terrainData.baseMapResolution = detailResolution;

                // Apply terrain data
                terrain.terrainData = terrainData;
                terrain.GetComponent<TerrainCollider>().terrainData = terrainData;
                terrain.basemapDistance = basemapDistance;
                terrain.heightmapPixelError = heightMapPixelError;
            }

            // Promote tileMap
            tileMapTexture.SetPixels32(TileMap);
            tileMapTexture.Apply(false);

            // Promote material
            terrain.materialTemplate = terrainMaterial;
            terrain.materialType = Terrain.MaterialType.Custom;

            // Promote heights
            Vector3 size = terrain.terrainData.size;
            terrain.terrainData.size = new Vector3(size.x, dfUnity.TerrainSampler.MaxTerrainHeight * TerrainScale, size.z);
            terrain.terrainData.SetHeights(0, 0, MapData.heightmapSamples);

            // Raise event
            RaiseOnPromoteTerrainDataEvent(terrain.terrainData);
        }

        /// <summary>
        /// Updates neighbour terrains.
        /// </summary>
        public void UpdateNeighbours()
        {
            Terrain terrain = GetComponent<Terrain>();
            terrain.SetNeighbors(LeftNeighbour, TopNeighbour, RightNeighbour, BottomNeighbour);
        }

        #region Editor Support

//#if UNITY_EDITOR
//        /// <summary>
//        /// Allows editor to set terrain independently of StreamingWorld. 
//        /// Mainly for testing purposes, but could be used for static scenes.
//        /// Also shows full terrain setup procedure for reference.
//        /// </summary>
//        public void __EditorUpdateTerrain()
//        {
//            // Setup terrain
//            InstantiateTerrain();

//            // Update data for terrain
//            UpdateMapPixelData();
//            UpdateTileMapData();
//            //UpdateHeightData();

//            // Promote data to live terrain
//            UpdateClimateMaterial();
//            PromoteTerrainData();

//            // Set neighbours
//            UpdateNeighbours();
//        }
//#endif

        #endregion

        #region Private Methods

        private bool ReadyCheck()
        {
            if (ready)
                return true;

            // Ensure we have a DaggerfallUnity reference
            if (dfUnity == null)
            {
                dfUnity = DaggerfallUnity.Instance;
            }

            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady)
            {
                DaggerfallUnity.LogMessage("DaggerfallTerrain: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            // Raise ready flag
            ready = true;

            return true;
        }

        #endregion

        #region Event Handlers

        // OnInstantiateTerrain
        public delegate void OnInstantiateTerrainEventHandler(DaggerfallTerrain sender);
        public static event OnInstantiateTerrainEventHandler OnInstantiateTerrain;
        protected virtual void RaiseOnInstantiateTerrainEvent()
        {
            if (OnInstantiateTerrain != null)
                OnInstantiateTerrain(this);
        }

        // OnPromoteTerrainData
        public delegate void OnPromoteTerrainDataEventHandler(DaggerfallTerrain sender, TerrainData terrainData);
        public static event OnPromoteTerrainDataEventHandler OnPromoteTerrainData;
        protected virtual void RaiseOnPromoteTerrainDataEvent(TerrainData terrainData)
        {
            if (OnPromoteTerrainData != null)
                OnPromoteTerrainData(this, terrainData);
        }

        #endregion
    }
}