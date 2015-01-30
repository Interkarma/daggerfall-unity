// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Helper methods for terrain generation.
    /// </summary>
    public static class TerrainHelper
    {
        // Terrain setup constants
        public const int terrainTileDim = 128;                          // Terrain tile dimension is 128x128 ground tiles
        public const int terrainSampleDim = terrainTileDim + 1;         // Terrain height sample dimension has 1 extra point for end vertex

        // Maximum terrain height is determined by scaled max input values
        // This can be further increased by global scale on terrain itself
        // Formula is (128 * baseHeightScale) + (128 * noiseMapScale) + extraNoiseScale
        // Do not change these unless necessary, use DaggerfallTerrain.TerrainScale from editor instead
        public const float baseHeightScale = 8f;        // 8 * 128 +
        public const float noiseMapScale = 4f;          // 4 * 128 + 
        public const float extraNoiseScale = 3f;        // 3 =
        public const float maxTerrainHeight = 1539f;    // 1539

        // Elevation of ocean and beach
        public const float scaledOceanElevation = 3.4f * baseHeightScale;
        public const float scaledBeachElevation = 5.0f * baseHeightScale;

        // Ranges and defaults for editor
        // Map pixel ranges are slightly smaller to allow for interpolation of neighbours
        public const int minMapPixelX = 3;
        public const int minMapPixelY = 3;
        public const int maxMapPixelX = 998;
        public const int maxMapPixelY = 498;
        public const int defaultMapPixelX = 207;
        public const int defaultMapPixelY = 213;
        public const float minTerrainScale = 1.0f;
        public const float maxTerrainScale = 6.0f;
        public const float defaultTerrainScale = 1.5f;

        /// <summary>
        /// Gets map pixel data for any location in world.
        /// </summary>
        public static MapPixelData GetMapPixelData(ContentReader contentReader, int mapPixelX, int mapPixelY)
        {
            // Read general data from world maps
            int worldHeight = contentReader.WoodsFileReader.GetHeightMapValue(mapPixelX, mapPixelY);
            int worldClimate = contentReader.MapFileReader.GetClimateIndex(mapPixelX, mapPixelY);
            int worldPolitic = contentReader.MapFileReader.GetPoliticIndex(mapPixelX, mapPixelY);

            // Get location if present
            int id = -1, regionIndex = -1, mapIndex = -1;
            string locationName = string.Empty;
            ContentReader.MapSummary mapSummary = new ContentReader.MapSummary();
            bool hasLocation = contentReader.HasLocation(mapPixelX, mapPixelY, out mapSummary);
            if (hasLocation)
            {
                id = mapSummary.ID;
                regionIndex = mapSummary.RegionIndex;
                mapIndex = mapSummary.MapIndex;
                DFLocation location = contentReader.MapFileReader.GetLocation(regionIndex, mapIndex);
                locationName = location.Name;
            }

            // Create map pixel data
            MapPixelData mapPixel = new MapPixelData()
            {
                inWorld = true,
                mapPixelX = mapPixelX,
                mapPixelY = mapPixelY,
                worldHeight = worldHeight,
                worldClimate = worldClimate,
                worldPolitic = worldPolitic,
                hasLocation = hasLocation,
                mapRegionIndex = regionIndex,
                mapLocationIndex = mapIndex,
                locationID = id,
                locationName = locationName,
            };

            return mapPixel;
        }

        /// <summary>
        /// Generate initial samples from any map pixel coordinates in world range.
        /// Also sets location height in mapPixelData for location positioning.
        /// </summary>
        public static void GenerateSamples(ContentReader contentReader, ref MapPixelData mapPixel)
        {
            // Divisor ensures continuous 0-1 range of tile samples
            float div = (float)terrainTileDim / 3f;

            // Read neighbouring height samples for this map pixel
            int mx = mapPixel.mapPixelX;
            int my = mapPixel.mapPixelY;
            byte[,] shm = contentReader.WoodsFileReader.GetHeightMapValuesRange(mx - 2, my - 2, 4);
            byte[,] lhm = contentReader.WoodsFileReader.GetLargeHeightMapValuesRange(mx - 1, my, 3);

            // Extract height samples for all chunks
            float averageHeight = 0;
            float maxHeight = float.MinValue;
            float baseHeight, noiseHeight;
            float x1, x2, x3, x4;
            int dim = terrainSampleDim;
            mapPixel.samples = new WorldSample[dim * dim];
            for (int y = 0; y < dim; y++)
            {
                for (int x = 0; x < dim; x++)
                {
                    float rx = (float)x / div;
                    float ry = (float)y / div;
                    int ix = Mathf.FloorToInt(rx);
                    int iy = Mathf.FloorToInt(ry);
                    float sfracx = (float)x / (float)(dim - 1);
                    float sfracy = (float)y / (float)(dim - 1);
                    float fracx = (float)(x - ix * div) / div;
                    float fracy = (float)(y - iy * div) / div;
                    float scaledHeight = 0;

                    //// TEST: Point sample small height map for base terrain
                    //baseHeight = shm[2, 2];
                    //scaledHeight += baseHeight * baseHeightScale;

                    // Bicubic sample small height map for base terrain elevation
                    x1 = CubicInterpolator(shm[0, 3], shm[1, 3], shm[2, 3], shm[3, 3], sfracx);
                    x2 = CubicInterpolator(shm[0, 2], shm[1, 2], shm[2, 2], shm[3, 2], sfracx);
                    x3 = CubicInterpolator(shm[0, 1], shm[1, 1], shm[2, 1], shm[3, 1], sfracx);
                    x4 = CubicInterpolator(shm[0, 0], shm[1, 0], shm[2, 0], shm[3, 0], sfracx);
                    baseHeight = CubicInterpolator(x1, x2, x3, x4, sfracy);
                    scaledHeight += baseHeight * baseHeightScale;

                    // Bicubic sample large height map for noise mask over terrain features
                    x1 = CubicInterpolator(lhm[ix, iy + 0], lhm[ix + 1, iy + 0], lhm[ix + 2, iy + 0], lhm[ix + 3, iy + 0], fracx);
                    x2 = CubicInterpolator(lhm[ix, iy + 1], lhm[ix + 1, iy + 1], lhm[ix + 2, iy + 1], lhm[ix + 3, iy + 1], fracx);
                    x3 = CubicInterpolator(lhm[ix, iy + 2], lhm[ix + 1, iy + 2], lhm[ix + 2, iy + 2], lhm[ix + 3, iy + 2], fracx);
                    x4 = CubicInterpolator(lhm[ix, iy + 3], lhm[ix + 1, iy + 3], lhm[ix + 2, iy + 3], lhm[ix + 3, iy + 3], fracx);
                    noiseHeight = CubicInterpolator(x1, x2, x3, x4, fracy);
                    scaledHeight += noiseHeight * noiseMapScale;

                    // Additional noise mask for small terrain features at ground level
                    float latitude = mapPixel.mapPixelX * MapsFile.WorldMapTileDim + x;
                    float longitude = MapsFile.MaxWorldTileCoordZ - mapPixel.mapPixelY * MapsFile.WorldMapTileDim + y;
                    float lowFreq = GetNoise(contentReader, latitude, longitude, 0.1f, 0.5f, 0.5f, 1);
                    float highFreq = GetNoise(contentReader, latitude, longitude, 6f, 0.5f, 0.5f, 1);
                    scaledHeight += (lowFreq * highFreq) * extraNoiseScale;

                    // Clamp lower values to ocean elevation
                    if (scaledHeight < scaledOceanElevation)
                        scaledHeight = scaledOceanElevation;

                    // Accumulate average height
                    averageHeight += scaledHeight;

                    // Get max height
                    if (scaledHeight > maxHeight)
                        maxHeight = scaledHeight;

                    // Set sample
                    mapPixel.samples[y * dim + x] = new WorldSample()
                    {
                        scaledHeight = scaledHeight,
                        record = 2,
                    };
                }
            }

            // Average and max heights are passed back for locations
            mapPixel.averageHeight = averageHeight /= (float)(dim * dim);
            mapPixel.maxHeight = maxHeight;
        }

        // Clear all sample tiles to same base index
        public static void ClearSampleTiles(ref MapPixelData mapPixel, byte record)
        {
            for (int i = 0; i < mapPixel.samples.Length; i++)
            {
                mapPixel.samples[i].record = record;
            }
        }

        // Set texture and height data for city tiles
        public static void SetLocationTiles(ContentReader contentReader, ref MapPixelData mapPixel)
        {
            const int tileDim = 16;
            const int chunkDim = 8;

            // Get location
            DFLocation location = contentReader.MapFileReader.GetLocation(mapPixel.mapRegionIndex, mapPixel.mapLocationIndex);

            // Centre location tiles inside terrain area
            int startX = ((chunkDim * tileDim) - location.Exterior.ExteriorData.Width * tileDim) / 2;
            int startY = ((chunkDim * tileDim) - location.Exterior.ExteriorData.Height * tileDim) / 2;

            // Full 8x8 locations have "terrain blend space" around walls to smooth down random terrain towards flat area.
            // This is indicated by texture index > 55 (ground texture range is 0-55), larger values indicate blend space.
            // We need to know rect of actual city area so we can use blend space outside walls.
            int xmin = terrainSampleDim, ymin = terrainSampleDim;
            int xmax = 0, ymax = 0;

            // Iterate blocks of this location
            for (int blockY = 0; blockY < location.Exterior.ExteriorData.Height; blockY++)
            {
                for (int blockX = 0; blockX < location.Exterior.ExteriorData.Width; blockX++)
                {
                    // Get block data
                    DFBlock block;
                    string blockName = contentReader.MapFileReader.GetRmbBlockName(ref location, blockX, blockY);
                    if (!contentReader.GetBlock(blockName, out block))
                        continue;

                    // Copy ground tile info
                    for (int tileY = 0; tileY < tileDim; tileY++)
                    {
                        for (int tileX = 0; tileX < tileDim; tileX++)
                        {
                            DFBlock.RmbGroundTiles tile = block.RmbBlock.FldHeader.GroundData.GroundTiles[tileX, (tileDim - 1) - tileY];
                            int xpos = startX + blockX * tileDim + tileX;
                            int ypos = startY + blockY * tileDim + tileY;
                            int offset = (ypos * terrainSampleDim) + xpos;

                            int record = tile.TextureRecord;
                            if (tile.TextureRecord < 56)
                            {
                                // Track interior bounds of location tiled area
                                if (xpos < xmin) xmin = xpos;
                                if (xpos > xmax) xmax = xpos;
                                if (ypos < ymin) ymin = ypos;
                                if (ypos > ymax) ymax = ypos;

                                // Store texture data from block
                                mapPixel.samples[offset].record = record;
                                mapPixel.samples[offset].flip = tile.IsFlipped;
                                mapPixel.samples[offset].rotate = tile.IsRotated;
                                mapPixel.samples[offset].location = true;
                            }
                        }
                    }
                }
            }

            // Update location rect with extra clearance
            const int extraClearance = 2;
            Rect locationRect = new Rect();
            locationRect.xMin = xmin - extraClearance;
            locationRect.xMax = xmax + extraClearance;
            locationRect.yMin = ymin - extraClearance;
            locationRect.yMax = ymax + extraClearance;
            mapPixel.locationRect = locationRect;
        }

        // Flattens location terrain and blends flat area with surrounding terrain
        // Not entirely happy with this, need to revisit later
        public static void FlattenLocationTerrain(ContentReader contentReader, ref MapPixelData mapPixel)
        {
            // Get range between bounds of sample data and interior location rect
            // The location rect is always smaller than the sample area
            float leftRange = 1f / (mapPixel.locationRect.xMin);
            float topRange = 1f / (mapPixel.locationRect.yMin);
            float rightRange = 1f / (terrainSampleDim - mapPixel.locationRect.xMax);
            float bottomRange = 1f / (terrainSampleDim - mapPixel.locationRect.yMax);

            float desiredHeight = mapPixel.averageHeight;
            float strength = 0;
            float u, v;
            for (int y = 1; y < terrainSampleDim - 1; y++)
            {
                for (int x = 1; x < terrainSampleDim - 1; x++)
                {
                    // Create a height scale from location to edge of terrain using
                    // linear interpolation on straight edges and bilinear in corners
                    if (x <= mapPixel.locationRect.xMin && y >= mapPixel.locationRect.yMin && y <= mapPixel.locationRect.yMax)
                    {
                        strength = x * leftRange;
                    }
                    else if (x >= mapPixel.locationRect.xMax && y >= mapPixel.locationRect.yMin && y <= mapPixel.locationRect.yMax)
                    {
                        strength = (terrainSampleDim - x) * rightRange;
                    }
                    else if (y <= mapPixel.locationRect.yMin && x >= mapPixel.locationRect.xMin && x <= mapPixel.locationRect.xMax)
                    {
                        strength = y * topRange;
                    }
                    else if (y >= mapPixel.locationRect.yMax && x >= mapPixel.locationRect.xMin && x <= mapPixel.locationRect.xMax)
                    {
                        strength = (terrainSampleDim - y) * bottomRange;
                    }
                    else if (x <= mapPixel.locationRect.xMin && y <= mapPixel.locationRect.yMin)
                    {
                        u = x * leftRange;
                        v = y * topRange;
                        strength = BilinearInterpolator(0, 0, 0, 1, u, v);
                    }
                    else if (x >= mapPixel.locationRect.xMax && y <= mapPixel.locationRect.yMin)
                    {
                        u = (terrainSampleDim - x) * rightRange;
                        v = y * topRange;
                        strength = BilinearInterpolator(0, 0, 0, 1, u, v);
                    }
                    else if (x <= mapPixel.locationRect.xMin && y >= mapPixel.locationRect.yMax)
                    {
                        u = x * leftRange;
                        v = (terrainSampleDim - y) * bottomRange;
                        strength = BilinearInterpolator(0, 0, 0, 1, u, v);
                    }
                    else if (x >= mapPixel.locationRect.xMax && y >= mapPixel.locationRect.yMax)
                    {
                        u = (terrainSampleDim - x) * rightRange;
                        v = (terrainSampleDim - y) * bottomRange;
                        strength = BilinearInterpolator(0, 0, 0, 1, u, v);
                    }

                    // Apply a little noise to gradient so it doesn't look perfectly smooth
                    // Noise strength is the inverse of scalemap strength
                    float extraNoise = GetNoise(contentReader, x, y, 0.1f, 0.5f, 0.5f, 1) * extraNoiseScale * (1f - strength);

                    int offset = y * terrainSampleDim + x;
                    float curHeight = mapPixel.samples[offset].scaledHeight;
                    if (!mapPixel.samples[offset].location)
                    {
                        mapPixel.samples[offset].scaledHeight = (desiredHeight * strength) + (curHeight * (1 - strength)) + extraNoise;
                    }
                    else
                    {
                        mapPixel.samples[offset].scaledHeight = desiredHeight;
                    }
                }
            }
        }

        /// <summary>
        /// Terrain interpolation causes Daggerfall's square coastline to become nicely raised and curvy.
        /// A side effect of this is that underwater climate areas are raised above sea-level.
        /// This function dilates coastal land climate into nearby ocean to hide this issue.
        /// Intended to be called once at startup. Modifies runtime copy of CLIMATE.PAK buffer.
        /// </summary>
        public static void DilateCoastalClimate(ContentReader contentReader, int passes)
        {
            //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            //long startTime = stopwatch.ElapsedMilliseconds;

            for (int pass = 0; pass < passes; pass++)
            {
                // Get clone of in-memory climate array
                byte[] climateArray = contentReader.MapFileReader.ClimateFile.Buffer.Clone() as byte[];

                // Dilate coastal areas
                for (int y = 1; y < WoodsFile.mapHeightValue - 1; y++)
                {
                    for (int x = 1; x < WoodsFile.mapWidthValue - 1; x++)
                    {
                        // Transfer climate of this pixel to any ocean pixel in Moore neighbourhood
                        TransferLandToOcean(contentReader, ref climateArray, x, y, x - 1, y - 1);
                        TransferLandToOcean(contentReader, ref climateArray, x, y, x, y - 1);
                        TransferLandToOcean(contentReader, ref climateArray, x, y, x + 1, y - 1);
                        TransferLandToOcean(contentReader, ref climateArray, x, y, x - 1, y);
                        TransferLandToOcean(contentReader, ref climateArray, x, y, x + 1, y);
                        TransferLandToOcean(contentReader, ref climateArray, x, y, x - 1, y + 1);
                        TransferLandToOcean(contentReader, ref climateArray, x, y, x, y + 1);
                        TransferLandToOcean(contentReader, ref climateArray, x, y, x + 1, y + 1);
                    }
                }

                // Store modified climate array
                contentReader.MapFileReader.ClimateFile.Buffer = climateArray;
            }

            //long totalTime = stopwatch.ElapsedMilliseconds - startTime;
            //DaggerfallUnity.LogMessage(string.Format("Time to dilate coastal climates: {0}ms", totalTime), true);
        }

        // Copies climate data from source to destination if destination is ocean
        // Used to dilate climate data around shorelines as a pre-process
        private static void TransferLandToOcean(ContentReader contentReader, ref byte[] dstClimateArray, int srcX, int srcY, int dstX, int dstY)
        {
            const int oceanClimate = 223;

            // Source must be land
            int srcOffset = srcY * PakFile.pakWidthValue + (srcX + 1);
            int srcClimate = contentReader.MapFileReader.ClimateFile.Buffer[srcOffset];
            if (srcClimate == oceanClimate)
                return;

            // Destination must be ocean
            int dstOffset = dstY * PakFile.pakWidthValue + (dstX + 1);
            int dstClimate = contentReader.MapFileReader.ClimateFile.Buffer[dstOffset];
            if (dstClimate == oceanClimate)
            {
                dstClimateArray[dstOffset] = (byte)srcClimate;
            }
        }

        /// <summary>
        /// If a location map pixel is on a gradient greater than threshold, then
        /// smooth surrounding Moore neighbourhood with location height
        /// </summary>
        public static void SmoothLocationNeighbourhood(ContentReader contentReader, int threshold = 20)
        {
            //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            //long startTime = stopwatch.ElapsedMilliseconds;

            // Get in-memory height array
            byte[] heightArray = contentReader.WoodsFileReader.Buffer;

            // Search for locations
            for (int y = 1; y < WoodsFile.mapHeightValue - 1; y++)
            {
                for (int x = 1; x < WoodsFile.mapWidthValue - 1; x++)
                {
                    ContentReader.MapSummary summary;
                    if (contentReader.HasLocation(x, y, out summary))
                    {
                        // Use Sobel filter for gradient
                        float x0y0 = heightArray[y * WoodsFile.mapWidthValue + x];
                        float x1y0 = heightArray[y * WoodsFile.mapWidthValue + (x + 1)];
                        float x0y1 = heightArray[(y + 1) * WoodsFile.mapWidthValue + x];
                        float gradient = GetGradient(x0y0, x1y0, x0y1);
                        if (gradient > threshold)
                        {
                            AverageHeights(ref heightArray, x, y);
                        }
                    }
                }
            }

            //long totalTime = stopwatch.ElapsedMilliseconds - startTime;
            //DaggerfallUnity.LogMessage(string.Format("Time to smooth location neighbourhoods: {0}ms", totalTime), true);
        }

        // Drops nature flats based on random chance scaled by simple rules
        public static void LayoutNatureBillboards(DaggerfallTerrain dfTerrain, DaggerfallBillboardBatch dfBillboardBatch, float terrainScale)
        {
            const float maxSteepness = 50f;         // 50
            const float chanceOnDirt = 0.2f;        // 0.2
            const float chanceOnGrass = 0.9f;       // 0.4
            const float chanceOnStone = 0.05f;      // 0.05

            // Get terrain
            Terrain terrain = dfTerrain.gameObject.GetComponent<Terrain>();
            if (!terrain)
                return;

            // Get terrain data
            TerrainData terrainData = terrain.terrainData;
            if (!terrainData)
                return;

            // Remove exiting billboards
            dfBillboardBatch.Clear();

            // Seed random with terrain key
            UnityEngine.Random.seed = MakeTerrainKey(dfTerrain.MapPixelX, dfTerrain.MapPixelY);

            // Just layout some random flats spread evenly across entire map pixel area
            // Flats are aligned with tiles, max 127x127 in billboard batch
            Vector2 tilePos = Vector2.zero;
            float scale = terrainData.heightmapScale.x;
            int dim = TerrainHelper.terrainTileDim - 1;
            for (int y = 0; y < dim; y++)
            {
                for (int x = 0; x < dim; x++)
                {
                    // Reject based on steepness
                    float steepness = terrainData.GetSteepness((float)x / dim, (float)y / dim);
                    if (steepness > maxSteepness)
                        continue;

                    // Reject if inside location rect
                    // Rect is expanded slightly to give extra clearance around locations
                    tilePos.x = x;
                    tilePos.y = y;
                    const int natureClearance = 4;
                    Rect rect = dfTerrain.MapData.locationRect;
                    if (rect.x > 0 && rect.y > 0)
                    {
                        rect.xMin -= natureClearance;
                        rect.xMin += natureClearance;
                        rect.yMin -= natureClearance;
                        rect.yMax += natureClearance;
                        if (rect.Contains(tilePos))
                            continue;
                    }

                    // Chance scaled based on map pixel height
                    // This tends to produce sparser lowlands and denser highlands
                    // Adjust or remove clamp range to influence nature generation
                    float elevationScale = (dfTerrain.MapData.worldHeight / 128f);
                    elevationScale = Mathf.Clamp(elevationScale, 0.4f, 1.0f);

                    // Chance scaled by base climate type
                    float climateScale = 1.0f;
                    DFLocation.ClimateSettings climate = MapsFile.GetWorldClimateSettings(dfTerrain.MapData.worldClimate);
                    switch (climate.ClimateType)
                    {
                        case DFLocation.ClimateBaseType.Desert:         // Just lower desert for now
                            climateScale = 0.25f;
                            break;
                    }

                    // Chance also determined by tile type
                    WorldSample sample = TerrainHelper.GetSample(ref dfTerrain.MapData.samples, x, y);
                    if (sample.record == 1)
                    {
                        // Dirt
                        if (UnityEngine.Random.Range(0f, 1f) > chanceOnDirt * elevationScale * climateScale)
                            continue;
                    }
                    else if (sample.record == 2)
                    {
                        // Grass
                        if (UnityEngine.Random.Range(0f, 1f) > chanceOnGrass * elevationScale * climateScale)
                            continue;
                    }
                    else if (sample.record == 3)
                    {
                        // Stone
                        if (UnityEngine.Random.Range(0f, 1f) > chanceOnStone * elevationScale * climateScale)
                            continue;
                    }
                    else
                    {
                        // Anything else
                        continue;
                    }

                    // Sample height and position billboard
                    Vector3 pos = new Vector3(x * scale, 0, y * scale);
                    float height = terrain.SampleHeight(pos + terrain.transform.position);
                    pos.y = height;

                    // Reject if too close to water
                    float beachLine = TerrainHelper.scaledBeachElevation * terrainScale;
                    if (height < beachLine - beachLine * 0.1f)
                        continue;

                    // Add to batch
                    int record = UnityEngine.Random.Range(1, 32);
                    dfBillboardBatch.AddItem(record, pos);
                }
            }

            // Apply new batch
            dfBillboardBatch.Apply();
        }

        // Gets terrain key based on map pixel coordinates
        public static int MakeTerrainKey(int mapPixelX, int mapPixelY)
        {
            return ((short)mapPixelY << 16) + (short)mapPixelX;
        }

        // Reverse terrain key back to map pixel coordinates
        public static void ReverseTerrainKey(int key, out int mapPixelX, out int mapPixelY)
        {
            mapPixelY = key >> 16;
            mapPixelX = key & 0xffff;
        }

        // Gets fast gradient value from heights
        private static float GetGradient(float x0y0, float x1y0, float x0y1)
        {
            float dx = x1y0 - x0y0;
            float dy = x0y1 - x0y0;
            return Mathf.Abs(dx) + Mathf.Abs(dy);           // Faster
            //return Mathf.Sqrt(dx * dx + dy * dy);         // More accurate
        }

        // Average centre coordinate height with surrounding heights
        private static void AverageHeights(ref byte[] heightArray, int cx, int cy)
        {
            // First pass averages
            float average = 0;
            float counter = 0;
            for (int y = cy - 1; y < cy + 2; y++)
            {
                for (int x = cx - 1; x < cx + 2; x++)
                {
                    average += heightArray[y * WoodsFile.mapWidthValue + x];
                    counter++;
                }
            }

            // Next pass applies average
            average /= counter;
            for (int y = cy - 1; y < cy + 2; y++)
            {
                for (int x = cx - 1; x < cx + 2; x++)
                {
                    heightArray[y * WoodsFile.mapWidthValue + x] = (byte)average;
                }
            }
        }

        /// <summary>
        /// Gets sample data at coordinate.
        /// </summary>
        public static WorldSample GetSample(ref WorldSample[] samples, int x, int y)
        {
            return samples[y * terrainSampleDim + x];
        }

        /// <summary>
        /// Get height value at coordinates.
        /// </summary>
        public static float GetHeight(ref WorldSample[] samples, int x, int y)
        {
            return samples[y * terrainSampleDim + x].scaledHeight;
        }

        /// <summary>
        /// Set height value at coordinates.
        /// </summary>
        public static void SetHeight(ref WorldSample[] samples, int x, int y, float height)
        {
            samples[y * terrainSampleDim + x].scaledHeight = height;
        }

        /// <summary>
        /// Get clamped height value at coordinates.
        /// </summary>
        public static float GetClampedHeight(ref WorldSample[] samples, int x, int y)
        {
            x = Mathf.Clamp(x, 0, terrainTileDim - 1);
            y = Mathf.Clamp(y, 0, terrainTileDim - 1);
            return samples[y * terrainSampleDim + x].scaledHeight;
        }

        /// <summary>
        /// Set clamped height value at coordinates.
        /// </summary>
        public static void SetClampedHeight(ref WorldSample[] samples, int x, int y, float height)
        {
            x = Mathf.Clamp(x, 0, terrainTileDim - 1);
            y = Mathf.Clamp(y, 0, terrainTileDim - 1);
            samples[y * terrainSampleDim + x].scaledHeight = height;
        }

        #region Private Methods

        // Bilinear interpolation of values
        private static float BilinearInterpolator(float valx0y0, float valx0y1, float valx1y0, float valx1y1, float u, float v)
        {
            float result =
                        (1 - u) * ((1 - v) * valx0y0 +
                        v * valx0y1) +
                        u * ((1 - v) * valx1y0 +
                        v * valx1y1);

            return result;
        }

        // Cubic interpolation of values
        private static float CubicInterpolator(float v0, float v1, float v2, float v3, float fracy)
        {
            float A = (v3 - v2) - (v0 - v1);
            float B = (v0 - v1) - A;
            float C = v2 - v0;
            float D = v1;

            return A * (fracy * fracy * fracy) + B * (fracy * fracy) + C * fracy + D;           // Faster
            //return A * Mathf.Pow(fracy, 3) + B * Mathf.Pow(fracy, 2) + C * fracy + D;
        }

        // Get noise sample at coordinates
        private static float GetNoise(
            ContentReader reader,
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
                finalValue += reader.Noise.Generate(x * frequency, y * frequency) * amplitude;
                frequency *= 2.0f;
                amplitude *= persistance;
            }

            return Mathf.Clamp(finalValue, -1, 1);
        }

        #endregion
    }
}