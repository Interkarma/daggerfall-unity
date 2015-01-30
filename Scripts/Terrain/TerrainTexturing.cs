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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Generates texture tiles for terrains and uses marching squares for tile transitions.
    /// These features are very much in early stages of development.
    /// </summary>
    public class TerrainTexturing
    {
        // Use same seed to ensure continuous tiles
        const int seed = 417028;

        const byte water = 0;
        const byte dirt = 1;
        const byte grass = 2;
        const byte stone = 3;

        Noise noise = new Noise();
        byte[] lookupTable;
        int[,] tileData;

        public TerrainTexturing()
        {
            CreateLookupTable();
            noise.Seed(seed);
        }

        #region Marching Squares - WIP

        // Very basic marching squares for water > dirt > grass > stone transitions.
        // Cannot handle water > grass or water > stone, etc.
        // Will improve this at later date to use a wider range of transitions.
        public void AssignTiles(ref MapPixelData mapData, bool march = true)
        {
            // Cache tile data to minimise noise sampling
            CacheTileData(ref mapData);

            // Assign tile data to terrain
            int dim = TerrainHelper.terrainSampleDim;
            for (int y = 0; y < dim; y++)
            {
                for (int x = 0; x < dim; x++)
                {
                    int offset = y * dim + x;

                    // Do nothing if location tile as texture already set
                    if (mapData.samples[offset].location)
                        continue;

                    // Assign tile texture
                    if (march)
                    {
                        // Get sample points
                        int b0 = tileData[x, y];
                        int b1 = tileData[x + 1, y];
                        int b2 = tileData[x, y + 1];
                        int b3 = tileData[x + 1, y + 1];

                        int shape = (b0 & 1) | (b1 & 1) << 1 | (b2 & 1) << 2 | (b3 & 1) << 3;
                        int ring = (b0 + b1 + b2 + b3) >> 2;
                        int tileID = shape | ring << 4;

                        byte val = lookupTable[tileID];
                        mapData.samples[offset].record = val & 63;
                        mapData.samples[offset].rotate = ((val & 64) == 64);
                        mapData.samples[offset].flip = ((val & 128) == 128);
                    }
                    else
                    {
                        mapData.samples[offset].record = tileData[x, y];
                    }
                }
            }
        }

        void CacheTileData(ref MapPixelData mapData)
        {
            // Create array if required
            int dim = TerrainHelper.terrainSampleDim + 1;
            if (tileData == null)
                tileData = new int[dim, dim];

            // Populate array with tile metadata
            for (int y = 0; y < dim; y++)
            {
                for (int x = 0; x < dim; x++)
                {
                    // Height sample for ocean and beach tiles
                    float height = TerrainHelper.GetClampedHeight(ref mapData.samples, x, y);

                    // Ocean texture
                    if (height <= TerrainHelper.scaledOceanElevation)
                    {
                        tileData[x, y] = water;
                        continue;
                    }

                    // Beach texture
                    // Adds a little +/- randomness to threshold so beach line isn't too regular
                    // Might look better with perlin noise instead
                    if (height <= TerrainHelper.scaledBeachElevation + UnityEngine.Random.Range(-1.5f, 1.5f))
                    {
                        tileData[x, y] = dirt;
                        continue;
                    }

                    // Get latitude and longitude of this tile
                    float latitude = mapData.mapPixelX * MapsFile.WorldMapTileDim + x;
                    float longitude = MapsFile.MaxWorldTileCoordZ - mapData.mapPixelY * MapsFile.WorldMapTileDim + y;

                    // Set texture tile using weighted noise
                    float weight = 0;
                    weight += NoiseWeight(latitude, longitude);
                    // TODO: Add other weights to influence texture tile generation
                    tileData[x, y] = GetWeightedRecord(weight);
                }
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
                throw new IndexOutOfRangeException("Index out of range. Valid range 0-55");
            if (rotate) index += 64;
            if (flip) index += 128;

            return (byte)index;
        }

        // Gets noise value
        private float NoiseWeight(float worldX, float worldY)
        {
            return GetNoise(worldX, worldY, 0.05f, 0.9f, 0.5f, 3);
        }

        // Sets texture by range
        private int GetWeightedRecord(float weight, float lowerGrassSpread = -0.2f, float upperGrassSpread = 0.9f)
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
            int octaves)
        {
            float finalValue = 0f;
            for (int i = 0; i < octaves; ++i)
            {
                finalValue += noise.Generate(x * frequency, y * frequency) * amplitude;
                frequency *= 2.0f;
                amplitude *= persistance;
            }

            return Mathf.Clamp(finalValue, -1, 1);
        }

        #endregion
    }
}