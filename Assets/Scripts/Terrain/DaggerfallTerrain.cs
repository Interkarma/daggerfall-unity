// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Hazelnut
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
using System.Collections.Generic;

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
        const int tilemapDim = MapsFile.WorldMapTileDim;
        const int resolutionPerPatch = 16;

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

        // Required for material properties
        [SerializeField, HideInInspector]
        Texture2D tileMapTexture;
        [SerializeField, HideInInspector]
        Material terrainMaterial;

        public Material TerrainMaterial { get { return terrainMaterial; } set { terrainMaterial = value; } }

        DaggerfallUnity dfUnity;
        int heightmapDim;
        int currentWorldClimate = -1;
        DaggerfallDateTime.Seasons season = DaggerfallDateTime.Seasons.Summer;
        bool ready;

        private float heightMapPixelError = 5; // just a default value in case ini value reading fails (so value will be overwritten by ini file value)

        public float HeightMapPixelError
        {
            get { return heightMapPixelError; }
            set { heightMapPixelError = value; }
        }

        // Dispose any native memory if class is destroyed.
        ~DaggerfallTerrain()
        {
            DisposeNativeMemory();
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
                tileMapTexture = new Texture2D(tilemapDim, tilemapDim, TextureFormat.ARGB32, false, true);
                tileMapTexture.filterMode = FilterMode.Point;
                tileMapTexture.wrapMode = TextureWrapMode.Clamp;
            }

            // Create terrain material
            if (terrainMaterial == null)
            {
                terrainMaterial = dfUnity.TerrainMaterialProvider.CreateMaterial();
                UpdateClimateMaterial();
            }

            // Raise event
            RaiseOnInstantiateTerrainEvent();
        }

        /// <summary>
        /// Updates climate material based on current map pixel data.
        /// Use <see cref="PromoteTerrainData()"/> to apply changes.
        /// </summary>
        public void UpdateClimateMaterial(bool init = false)
        {
            // Update atlas texture if world climate changed
            if (currentWorldClimate != MapData.worldClimate || dfUnity.WorldTime.Now.SeasonValue != season || init)
            {
                currentWorldClimate = MapData.worldClimate;
            }
        }

        /// <summary>
        /// Update map pixel data based on current coordinates. (first of a two stage process)
        /// 
        /// 1) BeginMapPixelDataUpdate - Schedules terrain data update using jobs system.
        /// 2) CompleteMapPixelDataUpdate - Completes terrain data update using jobs system.
        /// </summary>
        /// <param name="terrainTexturing">Instance of ITerrainTexturing implementation class to use.</param>
        /// <returns>JobHandle of the scheduled jobs</returns>
        public JobHandle BeginMapPixelDataUpdate(ITerrainTexturing terrainTexturing = null)
        {
            // Get basic terrain data.
            MapData = TerrainHelper.GetMapPixelData(dfUnity.ContentReader, MapPixelX, MapPixelY);

            // Create data array for heightmap.
            MapData.heightmapData = new NativeArray<float>(heightmapDim * heightmapDim, Allocator.TempJob);

            // Create data array for tilemap data.
            MapData.tilemapData = new NativeArray<byte>(tilemapDim * tilemapDim, Allocator.TempJob);

            // Create data array for shader tile map data.
            MapData.tileMap = new NativeArray<Color32>(tilemapDim * tilemapDim, Allocator.TempJob);

            // Create data array for average & max heights.
            MapData.avgMaxHeight = new NativeArray<float>(new float[] { 0, float.MinValue }, Allocator.TempJob);

            // Create list for recording native arrays that need disposal after jobs complete.
            MapData.nativeArrayList = new List<IDisposable>();

            // Generate heightmap samples. (returns when complete)
            JobHandle generateHeightmapSamplesJobHandle = dfUnity.TerrainSampler.ScheduleGenerateSamplesJob(ref MapData);

            // Handle location if one is present on terrain.
            JobHandle blendLocationTerrainJobHandle;
            if (MapData.hasLocation)
            {
                // Schedule job to calc average & max heights.
                JobHandle calcAvgMaxHeightJobHandle = TerrainHelper.ScheduleCalcAvgMaxHeightJob(ref MapData, generateHeightmapSamplesJobHandle);
                JobHandle.ScheduleBatchedJobs();

                // Set location tiles.
                TerrainHelper.SetLocationTiles(ref MapData);

                if (!dfUnity.TerrainSampler.IsLocationTerrainBlended())
                {
                    // Schedule job to blend and flatten location heights. (depends on SetLocationTiles being done first)
                    blendLocationTerrainJobHandle = TerrainHelper.ScheduleBlendLocationTerrainJob(ref MapData, calcAvgMaxHeightJobHandle);
                }
                else
                    blendLocationTerrainJobHandle = calcAvgMaxHeightJobHandle;
            }
            else
                blendLocationTerrainJobHandle = generateHeightmapSamplesJobHandle;

            // Assign tiles for terrain texturing.
            JobHandle assignTilesJobHandle = (terrainTexturing == null) ? blendLocationTerrainJobHandle :
                terrainTexturing.ScheduleAssignTilesJob(dfUnity.TerrainSampler, ref MapData, blendLocationTerrainJobHandle);

            // Update tile map for shader.
            JobHandle updateTileMapJobHandle = TerrainHelper.ScheduleUpdateTileMapDataJob(ref MapData, assignTilesJobHandle);
            JobHandle.ScheduleBatchedJobs();
            return updateTileMapJobHandle;
        }

        /// <summary>
        /// Complete terrain data update using jobs system. (second of a two stage process)
        /// </summary>
        /// <param name="terrainTexturing">Instance of ITerrainTexturing implementation class to use.</param>
        public void CompleteMapPixelDataUpdate(ITerrainTexturing terrainTexturing = null)
        {
            // Convert heightmap data back to standard managed 2d array.
            MapData.heightmapSamples = new float[heightmapDim, heightmapDim];
            for (int i = 0; i < MapData.heightmapData.Length; i++)
                MapData.heightmapSamples[JobA.Row(i, heightmapDim), JobA.Col(i, heightmapDim)] = MapData.heightmapData[i];

            // Convert tilemap data back to standard managed 2d array.
            // (Still needed for nature layout so it can be called again without requiring terrain data generation)
            MapData.tilemapSamples = new byte[tilemapDim, tilemapDim];
            for (int i = 0; i < MapData.tilemapData.Length; i++)
            {
                byte tile = MapData.tilemapData[i];
                if (tile == byte.MaxValue)
                    tile = 0;
                MapData.tilemapSamples[JobA.Row(i, tilemapDim), JobA.Col(i, tilemapDim)] = tile;
            }

            // Create tileMap array or resize if needed and copy native array.
            if (TileMap == null || TileMap.Length != MapData.tileMap.Length)
                TileMap = new Color32[MapData.tileMap.Length];
            MapData.tileMap.CopyTo(TileMap);

            // Copy max and avg heights. (TODO: Are these needed? Seem to not be used anywhere)
            MapData.averageHeight = MapData.avgMaxHeight[TerrainHelper.avgHeightIdx];
            MapData.maxHeight = MapData.avgMaxHeight[TerrainHelper.maxHeightIdx];

            DisposeNativeMemory();
        }

        /// <summary>
        /// Disposes the native arrays used in jobs system terrain data update.
        /// </summary>
        private void DisposeNativeMemory()
        {
            if (MapData.nativeArrayList != null)
            {
                // Dispose any temp working native array memory.
                foreach (IDisposable nativeArray in MapData.nativeArrayList)
                    nativeArray.Dispose();
                MapData.nativeArrayList = null;
            }

            // Dispose native array memory allocations now data has been extracted.
            if (MapData.heightmapData.IsCreated)
                MapData.heightmapData.Dispose();
            if (MapData.tilemapData.IsCreated)
                MapData.tilemapData.Dispose();
            if (MapData.avgMaxHeight.IsCreated)
                MapData.avgMaxHeight.Dispose();
            if (MapData.tileMap.IsCreated)
                MapData.tileMap.Dispose();
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
            var terrainMaterialData = new TerrainMaterialData(terrainMaterial, terrain.terrainData, tileMapTexture, currentWorldClimate);
            dfUnity.TerrainMaterialProvider.PromoteMaterial(this, terrainMaterialData);
            terrain.materialTemplate = terrainMaterial;

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
                heightmapDim = dfUnity.TerrainSampler.HeightmapDimension;
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