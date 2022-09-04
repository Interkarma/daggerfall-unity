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

using UnityEngine;
using DaggerfallConnect.Arena2;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

using Random = Unity.Mathematics.Random;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Terrain texturing interface.
    /// </summary>
    public interface ITerrainTexturing
    {
        // Does the conversion of tilemapData require water tiles converting from 0xFF (the default)
        bool ConvertWaterTiles();

        // Schedule the terrain tile generation and assignment jobs
        JobHandle ScheduleAssignTilesJob(ITerrainSampler terrainSampler, ref MapPixelData mapData, JobHandle dependencies, bool march = true);
    }

    /// <summary>
    /// Generates texture tiles for terrains and uses marching squares for tile transitions.
    /// These features are very much in early stages of development.
    /// </summary>
    public class DefaultTerrainTexturing : ITerrainTexturing
    {
        // Use same seed to ensure continuous tiles
        const int seed = 417028;

        const byte water = 0;
        const byte dirt = 1;
        const byte grass = 2;
        const byte stone = 3;

        protected static readonly int tileDataDim = MapsFile.WorldMapTileDim + 1;

        protected static readonly int assignTilesDim = MapsFile.WorldMapTileDim;

        protected byte[] lookupTable;

        public DefaultTerrainTexturing() => CreateLookupTable();

        public virtual bool ConvertWaterTiles() => true;

        public virtual JobHandle ScheduleAssignTilesJob(ITerrainSampler terrainSampler, ref MapPixelData mapData, JobHandle dependencies, bool march = true)
        {
            // Cache tile data to minimise noise sampling during march.
            var tileData = new NativeArray<byte>(tileDataDim * tileDataDim, Allocator.TempJob);
            var tileDataJob = new GenerateTileDataJob
            {
                HeightmapData = mapData.heightmapData,
                TileData = tileData,
                tdDim = tileDataDim,
                hDim = terrainSampler.HeightmapDimension,
                maxTerrainHeight = terrainSampler.MaxTerrainHeight,
                oceanElevation = terrainSampler.OceanElevation,
                beachElevation = terrainSampler.BeachElevation,
                mapPixelX = mapData.mapPixelX,
                mapPixelY = mapData.mapPixelY
            };
            JobHandle tileDataHandle = tileDataJob.Schedule(tileDataDim * tileDataDim, 64, dependencies);

            // Assign tile data to terrain
            var assignTilesJob = new AssignTilesJob
            {
                LookupTable = lookupTable.AsNativeArray(out ulong gcHandleLookup),
                TileData = tileData,
                TilemapData = mapData.tilemapData,
                tdDim = tileDataDim,
                tDim = assignTilesDim,
                march = march,
                locationRect = mapData.locationRect,
            };
            JobHandle assignTilesHandle = assignTilesJob.Schedule(assignTilesDim * assignTilesDim, 64, tileDataHandle);
            JobUtility.ReleaseGCObject(gcHandleLookup, assignTilesHandle);

            // schedule disposal
            tileData.Dispose(assignTilesHandle);

            return assignTilesHandle;
        }

        #region Marching Squares - WIP

        // Very basic marching squares for water > dirt > grass > stone transitions.
        // Cannot handle water > grass or water > stone, etc.
        // Will improve this at later date to use a wider range of transitions.
        [Unity.Burst.BurstCompile]
        protected struct AssignTilesJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<byte> TileData;
            [ReadOnly] public NativeArray<byte> LookupTable;
            public NativeArray<byte> TilemapData;
            public int tdDim, tDim;
            public bool march;
            public Rect locationRect;
            void IJobParallelFor.Execute(int index)
            {
                int x = Matrix.Row(index, tDim);
                int y = Matrix.Col(index, tDim);

                // Do nothing if in location rect as texture already set, to 0xFF if zero
                if (TilemapData[index] != 0)
                    return;

                // Assign tile texture
                if (march)
                {
                    // Get sample points
                    int tdIdx = Matrix.Idx(x, y, tdDim);
                    int b0 = TileData[tdIdx];               // tileData[x, y]
                    int b1 = TileData[tdIdx + 1];           // tileData[x + 1, y]
                    int b2 = TileData[tdIdx + tdDim];       // tileData[x, y + 1]
                    int b3 = TileData[tdIdx + tdDim + 1];   // tileData[x + 1, y + 1]

                    int shape = (b0 & 1) | (b1 & 1) << 1 | (b2 & 1) << 2 | (b3 & 1) << 3;
                    int ring = (b0 + b1 + b2 + b3) >> 2;
                    int tileID = shape | ring << 4;

                    TilemapData[index] = LookupTable[tileID];
                }
                else
                {
                    TilemapData[index] = TileData[Matrix.Idx(x, y, tdDim)];
                }
            }
        }

        [Unity.Burst.BurstCompile]
        protected struct GenerateTileDataJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<float> HeightmapData;
            public NativeArray<byte> TileData;
            public int hDim, tdDim, mapPixelX, mapPixelY;
            public float maxTerrainHeight, oceanElevation, beachElevation;
            void IJobParallelFor.Execute(int index)
            {
                int x = Matrix.Row(index, tdDim);
                int y = Matrix.Col(index, tdDim);

                // Height sample for ocean and beach tiles
                int2 h = (int2)math.clamp(new float2(hDim) * new float2(x, y) / new float2(tdDim), 0, hDim - 1);
                float height = HeightmapData[Matrix.Idx(h.y, h.x, hDim)] * maxTerrainHeight;// x & y swapped in heightmap for TerrainData.SetHeights()
                
                // Ocean texture
                if (height <= oceanElevation)
                {
                    TileData[index] = water;
                    return;
                }

                // Beach texture
                // Adds a little +/- randomness to threshold so beach line isn't too regular
                if (height <= beachElevation + (new Random(seed ^ (uint)index).NextFloat(-1.5f, 1.5f)))
                {
                    TileData[index] = dirt;
                    return;
                }

                // Get latitude and longitude of this tile
                int latitude = (int)(mapPixelX * MapsFile.WorldMapTileDim + x);
                int longitude = (int)(MapsFile.MaxWorldTileCoordZ - mapPixelY * MapsFile.WorldMapTileDim + y);

                // Set texture tile using weighted noise
                float weight = TerrainHelper.GetNoise(latitude, longitude, 0.05f, 0.9f, 0.4f, 3, seed);
                // TODO: Add other weights to influence texture tile generation
                // Sets texture by range
                const float lowerGrassSpread = 0.5f;
                const float upperGrassSpread = 0.95f;
                if (weight < lowerGrassSpread) TileData[index] = dirt;
                else if (weight > upperGrassSpread) TileData[index] = stone;
                else TileData[index] = grass;
            }
        }

        // Creates lookup table
        void CreateLookupTable()
        {
            lookupTable = new byte[64];
            AddLookupRange(0, 1, 5, 48, false, 0);
            AddLookupRange(2, 1, 10, 51, true, 16);
            AddLookupRange(2, 3, 15, 53, false, 32);
            AddLookupRange(3, 3, 15, 53, true, 48);
        }

        // Adds range of 16 values to lookup table
        void AddLookupRange(int baseStart, int baseEnd, int shapeStart, int saddleIndex, bool reverse, int offset)
        {
            if (reverse)
            {
                // high > low
                lookupTable[offset] = MakeLookup(baseStart, false, false);
                lookupTable[offset + 1] = MakeLookup(shapeStart + 2, true, true);
                lookupTable[offset + 2] = MakeLookup(shapeStart + 2, false, false);
                lookupTable[offset + 3] = MakeLookup(shapeStart + 1, true, true);
                lookupTable[offset + 4] = MakeLookup(shapeStart + 2, false, true);
                lookupTable[offset + 5] = MakeLookup(shapeStart + 1, false, true);
                lookupTable[offset + 6] = MakeLookup(saddleIndex, true, false); //d
                lookupTable[offset + 7] = MakeLookup(shapeStart, true, true);
                lookupTable[offset + 8] = MakeLookup(shapeStart + 2, true, false);
                lookupTable[offset + 9] = MakeLookup(saddleIndex, false, false); //d
                lookupTable[offset + 10] = MakeLookup(shapeStart + 1, false, false);
                lookupTable[offset + 11] = MakeLookup(shapeStart, false, false);
                lookupTable[offset + 12] = MakeLookup(shapeStart + 1, true, false);
                lookupTable[offset + 13] = MakeLookup(shapeStart, false, true);
                lookupTable[offset + 14] = MakeLookup(shapeStart, true, false);
                lookupTable[offset + 15] = MakeLookup(baseEnd, false, false);
            }
            else
            {
                // low > high
                lookupTable[offset] = MakeLookup(baseStart, false, false);
                lookupTable[offset + 1] = MakeLookup(shapeStart, true, false);
                lookupTable[offset + 2] = MakeLookup(shapeStart, false, true);
                lookupTable[offset + 3] = MakeLookup(shapeStart + 1, true, false);
                lookupTable[offset + 4] = MakeLookup(shapeStart, false, false);
                lookupTable[offset + 5] = MakeLookup(shapeStart + 1, false, false);
                lookupTable[offset + 6] = MakeLookup(saddleIndex, false, false); //d
                lookupTable[offset + 7] = MakeLookup(shapeStart + 2, true, false);
                lookupTable[offset + 8] = MakeLookup(shapeStart, true, true);
                lookupTable[offset + 9] = MakeLookup(saddleIndex, true, false); //d
                lookupTable[offset + 10] = MakeLookup(shapeStart + 1, false, true);
                lookupTable[offset + 11] = MakeLookup(shapeStart + 2, false, true);
                lookupTable[offset + 12] = MakeLookup(shapeStart + 1, true, true);
                lookupTable[offset + 13] = MakeLookup(shapeStart + 2, false, false);
                lookupTable[offset + 14] = MakeLookup(shapeStart + 2, true, true);
                lookupTable[offset + 15] = MakeLookup(baseEnd, false, false);
            }
        }

        // Encodes a byte with Daggerfall tile lookup
        byte MakeLookup(int index, bool rotate, bool flip)
        {
            if (index > 55)
                throw new System.IndexOutOfRangeException("Index out of range. Valid range 0-55");
            if (rotate) index += 64;
            if (flip) index += 128;

            return (byte)index;
        }

        #endregion
    }
}