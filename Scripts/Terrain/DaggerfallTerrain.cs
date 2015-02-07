// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;

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
        // May change at a later date
        const int tileMapDim = TerrainHelper.terrainTileDim;
        const int heightmapResolution = TerrainHelper.terrainSampleDim;
        const int detailResolution = TerrainHelper.terrainTileDim;
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

        // Required for material properties
        [SerializeField, HideInInspector]
        Texture2D tileMapTexture;
        [SerializeField, HideInInspector]
        Material terrainMaterial;

        DaggerfallUnity dfUnity;
        float[,] heights;
        Color32[] tileMap;
        int currentWorldClimate = -1;
        WorldTime.Seasons season = WorldTime.Seasons.Summer;
        bool ready;
        
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
                tileMapTexture = new Texture2D(tileMapDim, tileMapDim, TextureFormat.RGB24, false);
                tileMapTexture.filterMode = FilterMode.Point;
                tileMapTexture.wrapMode = TextureWrapMode.Clamp;
            }

            // Create terrain material
            if (terrainMaterial == null)
            {
                terrainMaterial = new Material(Shader.Find(MaterialReader._DaggerfallTerrainTilemapShaderName));
                UpdateClimateMaterial();
            }
        }

        /// <summary>
        /// Updates climate material based on current map pixel data.
        /// </summary>
        public void UpdateClimateMaterial()
        {
            // Update atlas texture if world climate changed
            if (currentWorldClimate != MapData.worldClimate || dfUnity.WorldTime.SeasonValue != season)
            {
                // Get current climate and ground archive
                DFLocation.ClimateSettings climate = MapsFile.GetWorldClimateSettings(MapData.worldClimate);
                int groundArchive = climate.GroundArchive;
                if (climate.ClimateType != DFLocation.ClimateBaseType.Desert &&
                    dfUnity.WorldTime.SeasonValue == WorldTime.Seasons.Winter)
                {
                    // Offset to snow textures
                    groundArchive++;
                }

                // Get tileset material to "steal" atlas texture for our shader
                // TODO: Improve material system to handle custom shaders
                Material tileSetMaterial = dfUnity.MaterialReader.GetTerrainTilesetMaterial(groundArchive);
                currentWorldClimate = MapData.worldClimate;

                // Assign textures
                terrainMaterial.SetTexture("_TileAtlasTex", tileSetMaterial.mainTexture);
                terrainMaterial.SetTexture("_TilemapTex", tileMapTexture);
                terrainMaterial.SetInt("_TilemapDim", tileMapDim);
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

            // Get basic terrain data
            MapData = TerrainHelper.GetMapPixelData(dfUnity.ContentReader, MapPixelX, MapPixelY);
            TerrainHelper.GenerateSamples(dfUnity.ContentReader, ref MapData);

            // Handle terrain with location
            if (MapData.hasLocation)
            {
                TerrainHelper.SetLocationTiles(dfUnity.ContentReader, ref MapData);
                TerrainHelper.FlattenLocationTerrain(dfUnity.ContentReader, ref MapData);
            }

            // Set textures
            if (terrainTexturing != null)
            {
                terrainTexturing.AssignTiles(ref MapData);
            }
        }

        /// <summary>
        /// Update tile map data based on current map pixel data.
        /// </summary>
        public void UpdateTileMapData()
        {
            // Create tileMap array if not present
            if (tileMap == null)
                tileMap = new Color32[tileMapDim * tileMapDim];

            // Also recreate if not sized appropriately
            if (tileMap.Length != tileMapDim * tileMapDim)
                tileMap = new Color32[tileMapDim * tileMapDim];

            // Assign tile data to tilemap
            Color32 tileColor = new Color32(0, 0, 0, 0);
            for (int y = 0; y < tileMapDim; y++)
            {
                for (int x = 0; x < tileMapDim; x++)
                {
                    // Get sample tile data
                    WorldSample sample = MapData.samples[y * TerrainHelper.terrainSampleDim + x];

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
                    tileMap[y * tileMapDim + x] = tileColor;
                }
            }
        }

        /// <summary>
        /// Update heights from current map pixel data.
        /// </summary>
        public void UpdateHeightData()
        {
            // Create target height array if not present
            if (heights == null)
                heights = new float[heightmapResolution, heightmapResolution];

            // Set new height data from world samples
            for (int y = 0; y < heightmapResolution; y++)
            {
                for (int x = 0; x < heightmapResolution; x++)
                {
                    float sampleHeight = MapData.samples[y * TerrainHelper.terrainSampleDim + x].scaledHeight;
                    heights[y, x] = Mathf.Clamp01(sampleHeight / TerrainHelper.maxTerrainHeight);
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
                terrainData.heightmapResolution = heightmapResolution;
                terrainData.size = new Vector3(terrainSize, TerrainHelper.maxTerrainHeight, terrainSize);
                terrainData.SetDetailResolution(detailResolution, resolutionPerPatch);
                terrainData.alphamapResolution = detailResolution;
                terrainData.baseMapResolution = detailResolution;

                // Apply terrain data
                terrain.terrainData = terrainData;
                (terrain.collider as TerrainCollider).terrainData = terrainData;
                terrain.basemapDistance = basemapDistance;
            }

            // Promote tileMap
            tileMapTexture.SetPixels32(tileMap);
            tileMapTexture.Apply(false);

            // Promote material
            terrain.materialTemplate = terrainMaterial;

            // Promote heights
            Vector3 size = terrain.terrainData.size;
            terrain.terrainData.size = new Vector3(size.x, TerrainHelper.maxTerrainHeight * TerrainScale, size.z);
            terrain.terrainData.SetHeights(0, 0, heights);
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

#if UNITY_EDITOR
        /// <summary>
        /// Allows editor to set terrain independently of StreamingWorld. 
        /// Mainly for testing purposes, but could be used for static scenes.
        /// Also shows full terrain setup procedure for reference.
        /// </summary>
        public void __EditorUpdateTerrain()
        {
            // Setup terrain
            InstantiateTerrain();

            // Update data for terrain
            UpdateMapPixelData();
            UpdateTileMapData();
            UpdateHeightData();

            // Promote data to live terrain
            UpdateClimateMaterial();
            PromoteTerrainData();

            // Set neighbours
            UpdateNeighbours();
        }
#endif

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
    }
}