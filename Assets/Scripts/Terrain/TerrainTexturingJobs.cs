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
using DaggerfallConnect.Arena2;
using Unity.Jobs;
using Unity.Collections;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Generates texture tiles for terrains and uses marching squares for tile transitions.
    /// These features are very much in early stages of development.
    /// </summary>
    public class TerrainTexturingJobs
    {
        // Use same seed to ensure continuous tiles
        const int seed = 417028;

        const byte water = 0;
        const byte dirt = 1;
        const byte grass = 2;
        const byte stone = 3;

        static int tileDataDim = MapsFile.WorldMapTileDim + 1;
        static int tileDataDim2 = tileDataDim * tileDataDim;

        static int assignTilesDim = MapsFile.WorldMapTileDim;
        static int assignTilesDim2 = assignTilesDim * assignTilesDim;

        //byte[] lookupTable;
        //int[,] tileData;
        NativeArray<byte> lookupTable;
        NativeArray<byte> tileData;

        public TerrainTexturingJobs()
        {
            CreateLookupTable();
            // Keep memory allocation for tileData for re-use.
            tileData = new NativeArray<byte>(tileDataDim2, Allocator.Persistent);
        }

        ~TerrainTexturingJobs()
        {
            lookupTable.Dispose();
            tileData.Dispose();
        }

        struct GenerateTileDataJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<byte> lookupTable;
            [ReadOnly]
            public NativeArray<float> heightmapSamples;

            public NativeArray<byte> tileData;

            public int tDim;
            public int hDim;
            public float maxTerrainHeight;
            public float oceanElevation;
            public float beachElevation;
            public int mapPixelX;
            public int mapPixelY;

            // Gets noise value
            private float NoiseWeight(float worldX, float worldY)
            {
                return GetNoise(worldX, worldY, 0.05f, 0.9f, 0.4f, 3, seed);
            }

            // Sets texture by range
            private byte GetWeightedRecord(float weight, float lowerGrassSpread = 0.5f, float upperGrassSpread = 0.95f)
            {
                if (weight < lowerGrassSpread)
                    return dirt;
                else if (weight > upperGrassSpread)
                    return stone;
                else
                    return grass;
            }

            // Noise function
            private float GetNoise(
                float x,
                float y,
                float frequency,
                float amplitude,
                float persistance,
                int octaves,
                int seed = 0)
            {
                float finalValue = 0f;
                for (int i = 0; i < octaves; ++i)
                {
                    finalValue += Mathf.PerlinNoise(seed + (x * frequency), seed + (y * frequency)) * amplitude;
                    frequency *= 2.0f;
                    amplitude *= persistance;
                }

                return Mathf.Clamp(finalValue, -1, 1);
            }

            public void Execute(int index)
            {
                int x = JobA.Row(index, tDim);
                int y = JobA.Col(index, tDim);

                // Height sample for ocean and beach tiles
                int hx = (int)Mathf.Clamp(hDim * ((float)x / (float)tDim), 0, hDim - 1);
                int hy = (int)Mathf.Clamp(hDim * ((float)y / (float)tDim), 0, hDim - 1);
                float height = heightmapSamples[JobA.Idx(hy, hx, hDim)] * maxTerrainHeight;  // x & y swapped in heightmap for TerrainData.SetHeights()
                // Ocean texture
                if (height <= oceanElevation)
                {
                    tileData[index] = water;
                    return;
                }
                // Beach texture
                // Adds a little +/- randomness to threshold so beach line isn't too regular
                if (height <= beachElevation) // + UnityEngine.Random.Range(-1.5f, 1.5f))
                {
                    tileData[index] = dirt;
                    return;
                }

                // Get latitude and longitude of this tile
                int latitude = (int)(mapPixelX * MapsFile.WorldMapTileDim + x);
                int longitude = (int)(MapsFile.MaxWorldTileCoordZ - mapPixelY * MapsFile.WorldMapTileDim + y);

                // Set texture tile using weighted noise
                float weight = 0;
                weight += NoiseWeight(latitude, longitude);
                // TODO: Add other weights to influence texture tile generation
                tileData[index] = GetWeightedRecord(weight);
            }
        }

        #region Marching Squares - WIP

        // Very basic marching squares for water > dirt > grass > stone transitions.
        // Cannot handle water > grass or water > stone, etc.
        // Will improve this at later date to use a wider range of transitions.

        struct AssignTilesJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<byte> tileData;
            [ReadOnly]
            public NativeArray<byte> lookupTable;

            public NativeArray<byte> tilemapSamples;

            public int tDim;
            public int dim;
            public bool march;

            public void Execute(int index)
            {
                int x = JobA.Row(index, dim);
                int y = JobA.Col(index, dim);

                // Do nothing if location tile as texture already set
//                if (tilemapSamples[index].location)
//                    return;

                // Assign tile texture
                if (march)
                {
                    // Get sample points
                    int tdIdx = JobA.Idx(x, y, tDim);
                    int b0 = tileData[tdIdx];               // tileData[x, y]
                    int b1 = tileData[tdIdx + 1];           // tileData[x + 1, y]
                    int b2 = tileData[tdIdx + tDim];        // tileData[x, y + 1]
                    int b3 = tileData[tdIdx + tDim + 1];    // tileData[x + 1, y + 1]

                    int shape = (b0 & 1) | (b1 & 1) << 1 | (b2 & 1) << 2 | (b3 & 1) << 3;
                    int ring = (b0 + b1 + b2 + b3) >> 2;
                    int tileID = shape | ring << 4;

                    tilemapSamples[index] = lookupTable[tileID];
                }
                else
                {
                    tilemapSamples[index] = tileData[JobA.Idx(x, y, tDim)];
                }
            }
        }

        public void AssignTiles(ITerrainSampler terrainSampler, ref MapPixelDataJobs mapData, bool march = true)
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Cache tile data to minimise noise sampling during march.
            GenerateTileDataJob tileDataJob = new GenerateTileDataJob
            {
                lookupTable = lookupTable,
                heightmapSamples = mapData.heightmapSamples,
                tileData = tileData,
                tDim = tileDataDim,
                hDim = terrainSampler.HeightmapDimension,
                maxTerrainHeight = terrainSampler.MaxTerrainHeight,
                oceanElevation = terrainSampler.OceanElevation,
                beachElevation = terrainSampler.BeachElevation,
                mapPixelX = mapData.mapPixelX,
                mapPixelY = mapData.mapPixelY,
            };

            //tileDataJob.Run(tileDataDim2);
            JobHandle tileDataHandle = tileDataJob.Schedule(tileDataDim2, 64);
            //tileDataHandle.Complete();

            // Assign tile data to terrain
            AssignTilesJob assignTilesJob = new AssignTilesJob
            {
                tileData = tileData,
                lookupTable = lookupTable,
                tilemapSamples = mapData.tilemapSamples,
                tDim = tileDataDim,
                dim = assignTilesDim,
                march = march,
            };

            //assignTilesJob.Run(assignTilesDim2);
            JobHandle assignTilesHandle = assignTilesJob.Schedule(assignTilesDim2, 64, tileDataHandle);
            assignTilesHandle.Complete();

            stopwatch.Stop();
            DaggerfallUnity.LogMessage(string.Format("Time to assignTiles for ({0},{1}): {2}ms", mapData.mapPixelX, mapData.mapPixelY, stopwatch.ElapsedMilliseconds), true);
        }

        // Creates lookup table
        void CreateLookupTable()
        {
            lookupTable = new NativeArray<byte>(64, Allocator.Persistent);
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
                throw new IndexOutOfRangeException("Index out of range. Valid range 0-55");
            if (rotate) index += 64;
            if (flip) index += 128;

            return (byte)index;
        }

        #endregion
    }
}