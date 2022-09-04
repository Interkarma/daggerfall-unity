// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Hazelnut, Andrzej Łukasik (andrew.r.lukasik)
// 
// Notes:
//

using System.Collections.Generic;
using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility.AssetInjection;
using Unity.Profiling;
using Unity.Mathematics;
using Unity.Collections;

using Random = UnityEngine.Random;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Terrain nature layout interface.
    /// </summary>
    public interface ITerrainNature
    {
        /// <summary>
        /// This should return true if any nature flat is replaced by a 3d model.
        /// 
        /// It's used by DFU streaming world to trigger nature layout for map pixels moving between distance 1 and 2 when
        /// 3d models are used. Distance 1 use models, but map pixels at distance 2+ still use billboards for performance.
        /// </summary>
        bool NatureMeshUsed { get; }

        /// <summary>
        /// Layout nature flats for a map pixel.
        /// </summary>
        /// <param name="dfTerrain">The terrain object for the map pixel</param>
        /// <param name="dfBillboardBatch">Daggerfall billboard batcher object to add flats to</param>
        /// <param name="terrainScale">Terrain scale being used by streaming world</param>
        /// <param name="terrainDist">Distance of this map pixel from the one the player is currently in</param>
        void LayoutNature(DaggerfallTerrain dfTerrain, DaggerfallBillboardBatch dfBillboardBatch, float terrainScale, int terrainDist);
    }

    /// <summary>
    /// Drops nature flats based on random chance scaled by simple rules.
    /// </summary>
    public class DefaultTerrainNature : ITerrainNature
    {
        protected const float maxSteepness = 50f;             // 50
        protected const float slopeSinkRatio = 70f;           // Sink flats slightly into ground as slope increases to prevent floaty trees.
        protected const float baseChanceOnDirt = 0.2f;        // 0.2
        protected const float baseChanceOnGrass = 0.9f;       // 0.4
        protected const float baseChanceOnStone = 0.05f;      // 0.05
        protected const int natureClearance = 4;

        public bool NatureMeshUsed { get; protected set; }

        #region Profiler Markers
        
        static readonly ProfilerMarker
            ___getClimateSettings = new ProfilerMarker("get climate settings"),
            ___removeExitingBillboards = new ProfilerMarker("remove existing billboards"),
            ___layoutlayoutRandomFlats = new ProfilerMarker("layout random flats"),
            ___addNewBillboards = new ProfilerMarker("add new billboards"),
            ___apply = new ProfilerMarker("apply"),
            ___LayoutNature = new ProfilerMarker($"{nameof(DefaultTerrainNature)}.{nameof(LayoutNature)}");
        
        #endregion

        public virtual void LayoutNature(DaggerfallTerrain dfTerrain, DaggerfallBillboardBatch dfBillboardBatch, float terrainScale, int terrainDist)
        {
            ___LayoutNature.Begin();

            // Location Rect is expanded slightly to give extra clearance around locations
            Rect rect = dfTerrain.MapData.locationRect;
            if (rect.x > 0 && rect.y > 0)
            {
                rect.xMin -= natureClearance;
                rect.xMax += natureClearance;
                rect.yMin -= natureClearance;
                rect.yMax += natureClearance;
            }
            // Chance scaled based on map pixel height
            // This tends to produce sparser lowlands and denser highlands
            // Adjust or remove clamp range to influence nature generation
            float elevationScale = (dfTerrain.MapData.worldHeight / 128f);
            elevationScale = math.clamp(elevationScale, 0.4f, 1.0f);

            // Chance scaled by base climate type
            ___getClimateSettings.Begin();
            float climateScale = 1.0f;
            DFLocation.ClimateSettings climate = MapsFile.GetWorldClimateSettings(dfTerrain.MapData.worldClimate);
            switch (climate.ClimateType)
            {
                case DFLocation.ClimateBaseType.Desert:         // Just lower desert for now
                    climateScale = 0.25f;
                    break;
            }
            float chanceOnDirt = baseChanceOnDirt * elevationScale * climateScale;
            float chanceOnGrass = baseChanceOnGrass * elevationScale * climateScale;
            float chanceOnStone = baseChanceOnStone * elevationScale * climateScale;
            ___getClimateSettings.End();

            // Get terrain
            Terrain terrain = dfTerrain.gameObject.GetComponent<Terrain>();
            if (!terrain)
            {
                ___LayoutNature.End();
                return;
            }

            // Get terrain data
            TerrainData terrainData = terrain.terrainData;
            if (!terrainData)
            {
                ___LayoutNature.End();
                return;
            }

            // Remove exiting billboards
            ___removeExitingBillboards.Begin();
            dfBillboardBatch.Clear();
            MeshReplacement.ClearNatureGameObjects(terrain);
            ___removeExitingBillboards.End();

            // Seed random with terrain key
            Random.InitState(TerrainHelper.MakeTerrainKey(dfTerrain.MapPixelX, dfTerrain.MapPixelY));

            // Just layout some random flats spread evenly across entire map pixel area
            // Flats are aligned with tiles, max 16129 billboards per batch
            ___layoutlayoutRandomFlats.Begin();
            var terrainSampler = DaggerfallUnity.Instance.TerrainSampler;
            int tDim = MapsFile.WorldMapTileDim;
            int hDim = terrainSampler.HeightmapDimension;
            float scale = terrainData.heightmapScale.x * (float)hDim / (float)tDim;
            float maxTerrainHeight = terrainSampler.MaxTerrainHeight;
            float beachLine = terrainSampler.BeachElevation;
            byte[,] tilemapSamples = dfTerrain.MapData.tilemapSamples;
            float[,] heightmapSamples = dfTerrain.MapData.heightmapSamples;
            float3 terrainPosition = terrain.transform.position;
            var itemsBuffer = new NativeArray<DaggerfallBillboardBatch.ItemToAdd>(math.min(tDim * tDim, DaggerfallBillboardBatch.maxBillboardCount), Allocator.TempJob);
            int itemsBufferCounter = 0;
            for (int y = 0; y < tDim; y++)
            {
                for (int x = 0; x < tDim; x++)
                {
                    // Reject if inside location rect (expanded slightly to give extra clearance around locations)
                    if (rect.x > 0 && rect.y > 0 && rect.Contains(new Vector2(x,y)))// tilePos = new Vector2(x,y);
                        continue;

                    // Chance also determined by tile type
                    int tile = tilemapSamples[x, y] & 0x3F;
                    if (tile == 1)
                    {   // Dirt
                        if (Random.Range(0f, 1f) > chanceOnDirt)
                            continue;
                    }
                    else if (tile == 2)
                    {   // Grass
                        if (Random.Range(0f, 1f) > chanceOnGrass)
                            continue;
                    }
                    else if (tile == 3)
                    {   // Stone
                        if (Random.Range(0f, 1f) > chanceOnStone)
                            continue;
                    }
                    else
                    {   // Anything else
                        continue;
                    }

                    int2 h = (int2)math.clamp(new float2(hDim) * (new float2(x, y) / new float2(tDim)), float2.zero, new float2(hDim) - new float2(1));
                    float height = heightmapSamples[h.y, h.x] * maxTerrainHeight;// x & y swapped in heightmap for TerrainData.SetHeights()

                    // Reject if too close to water
                    if (height < beachLine)
                        continue;

                    // Reject based on steepness
                    float steepness = terrainData.GetSteepness((float)x / tDim, (float)y / tDim);
                    if (steepness > maxSteepness)
                        continue;

                    // Add to batch unless a mesh replacement is found
                    int record = Random.Range(1, 32);
                    if (terrainDist > 1 || !MeshReplacement.ImportNatureGameObject(dfBillboardBatch.TextureArchive, record, terrain, x, y))
                    {
                        // Sample height and position billboard
                        float3 pos = new float3(x, 0, y) * new float3(scale);
                        float height2 = terrain.SampleHeight(pos + terrainPosition);
                        pos.y = height2 - (steepness / slopeSinkRatio);

                        itemsBuffer[itemsBufferCounter++] = new DaggerfallBillboardBatch.ItemToAdd(record, pos);
                        if (itemsBufferCounter == itemsBuffer.Length) goto loopEnd;// stop when full
                    }
                    else NatureMeshUsed = true;  // Signal that nature mesh has been used to initiate extra terrain updates
                }
            }
        loopEnd:
            ___layoutlayoutRandomFlats.End();

            ___addNewBillboards.Begin();
            var addItemsJobHandle = dfBillboardBatch.AddItemsAsync(itemsBuffer.GetSubArray(0, itemsBufferCounter));
            itemsBuffer.Dispose(addItemsJobHandle);
            ___addNewBillboards.End();

            ___apply.Begin();
            dfBillboardBatch.Apply();
            ___apply.End();

            ___LayoutNature.End();
        }
    }

}