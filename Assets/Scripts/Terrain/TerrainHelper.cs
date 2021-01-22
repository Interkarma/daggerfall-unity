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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using Unity.Jobs;
using Unity.Collections;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Helper methods for terrain generation.
    /// </summary>
    public static class TerrainHelper
    {
        public const byte avgHeightIdx = 0;
        public const byte maxHeightIdx = 1;
        public const int rotBit = 0x40;
        public const int flipBit = 0x80;

        // Ranges and defaults for editor
        // Map pixel ranges are slightly smaller to allow for interpolation of neighbours
        public const int minMapPixelX = 3;
        public const int minMapPixelY = 3;
        public const int maxMapPixelX = 998;
        public const int maxMapPixelY = 498;
        public const int defaultMapPixelX = 207;
        public const int defaultMapPixelY = 213;
        public const float minTerrainScale = 1.0f;
        public const float maxTerrainScale = 10.0f;
        public const float defaultTerrainScale = 1.5f;

        /// <summary>
        /// Gets the Terrain name for a given map pixel
        /// </summary>
        public static string GetTerrainName(int mapPixelX, int mapPixelY)
        {
            return string.Format("DaggerfallTerrain [{0},{1}]", mapPixelX, mapPixelY);
        }

        // Allow mods to set extra blend space around locations for placing content, specifed in # of tiles
        public delegate int AdditionalLocationBlendSpace(DFRegion.LocationTypes locationType);
        public static AdditionalLocationBlendSpace ExtraBlendSpace = (locationType) => { return 0; };

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
                LocationType = mapSummary.LocationType
            };

            return mapPixel;
        }

        // Determines tile origin of location inside terrain area.
        // This is not always centred precisely but rather seems to follow some other
        // logic/formula for locations of certain RMB dimensions (e.g. 1x1).
        // Unknown if there are more exceptions or if a specific formula is needed.
        // This method will be used in the interim pending further research.
        public static DFPosition GetLocationTerrainTileOrigin(DFLocation location)
        {
            // Get map width and height
            int width = location.Exterior.ExteriorData.Width;
            int height = location.Exterior.ExteriorData.Height;

            // Centring works nearly all the time
            DFPosition result = new DFPosition();
            result.X = (RMBLayout.RMBTilesPerTerrain - width * RMBLayout.RMBTilesPerBlock) / 2;
            result.Y = (RMBLayout.RMBTilesPerTerrain - height * RMBLayout.RMBTilesPerBlock) / 2;

            // Handle custom 1x1 location position
            if (location.HasCustomLocationPosition())
            {
                result.X = 72;
                result.Y = 55;
            }
            return result;
        }

        // Set location tilemap data
        public static void SetLocationTiles(ref MapPixelData mapPixel)
        {
            // Get location
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            DFLocation location = dfUnity.ContentReader.MapFileReader.GetLocation(mapPixel.mapRegionIndex, mapPixel.mapLocationIndex);

            // Position tiles inside terrain area
            DFPosition tilePos = TerrainHelper.GetLocationTerrainTileOrigin(location);

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
                    string blockName = dfUnity.ContentReader.MapFileReader.GetRmbBlockName(ref location, blockX, blockY);
                    if (!dfUnity.ContentReader.GetBlock(blockName, out block))
                        continue;

                    // Copy ground tile info
                    for (int tileY = 0; tileY < RMBLayout.RMBTilesPerBlock; tileY++)
                    {
                        for (int tileX = 0; tileX < RMBLayout.RMBTilesPerBlock; tileX++)
                        {
                            DFBlock.RmbGroundTiles tile = block.RmbBlock.FldHeader.GroundData.GroundTiles[tileX, (RMBLayout.RMBTilesPerBlock - 1) - tileY];
                            int xpos = tilePos.X + blockX * RMBLayout.RMBTilesPerBlock + tileX;
                            int ypos = tilePos.Y + blockY * RMBLayout.RMBTilesPerBlock + tileY;

                            if (tile.TextureRecord < 56)
                            {
                                // Track interior bounds of location tiled area
                                if (xpos < xmin) xmin = xpos;
                                if (xpos > xmax) xmax = xpos;
                                if (ypos < ymin) ymin = ypos;
                                if (ypos > ymax) ymax = ypos;

                                // Store texture data from block
                                mapPixel.tilemapData[JobA.Idx(xpos, ypos, MapsFile.WorldMapTileDim)] = tile.TileBitfield == 0 ? byte.MaxValue : tile.TileBitfield;
                            }
                        }
                    }
                }
            }

            // Update location rect with extra clearance
            int extraClearance = location.MapTableData.LocationType == DFRegion.LocationTypes.TownCity ? 3 : 2;
            Rect locationRect = new Rect();
            locationRect.xMin = xmin - extraClearance;
            locationRect.xMax = xmax + extraClearance;
            locationRect.yMin = ymin - extraClearance;
            locationRect.yMax = ymax + extraClearance;
            mapPixel.locationRect = locationRect;
        }

        #region Terrain Jobs - Schedulers

        public static JobHandle ScheduleCalcAvgMaxHeightJob(ref MapPixelData mapPixel, JobHandle dependencies)
        {
            CalcAvgMaxHeightJob calcAvgMaxHeightJob = new CalcAvgMaxHeightJob()
            {
                heightmapData = mapPixel.heightmapData,
                avgMaxHeight = mapPixel.avgMaxHeight,
            };
            return calcAvgMaxHeightJob.Schedule(dependencies);
        }

        public static JobHandle ScheduleBlendLocationTerrainJob(ref MapPixelData mapPixel, JobHandle dependencies)
        {
            BlendLocationTerrainJob blendLocationTerrainJob = new BlendLocationTerrainJob()
            {
                heightmapData = mapPixel.heightmapData,
                avgMaxHeight = mapPixel.avgMaxHeight,
                hDim = DaggerfallUnity.Instance.TerrainSampler.HeightmapDimension,
                locationRect = mapPixel.locationRect,
            };
            int extraBlendSpace = ExtraBlendSpace(mapPixel.LocationType);
            if (extraBlendSpace > 0)
            {
                blendLocationTerrainJob.locationRect.xMin -= extraBlendSpace;
                blendLocationTerrainJob.locationRect.xMax += extraBlendSpace;
                blendLocationTerrainJob.locationRect.yMin -= extraBlendSpace;
                blendLocationTerrainJob.locationRect.yMax += extraBlendSpace;
            }
            return blendLocationTerrainJob.Schedule(dependencies);
        }

        public static JobHandle ScheduleUpdateTileMapDataJob(ref MapPixelData mapPixel, JobHandle dependencies)
        {
            int tilemapDim = MapsFile.WorldMapTileDim;
            UpdateTileMapDataJob updateTileMapDataJob = new UpdateTileMapDataJob()
            {
                tilemapData = mapPixel.tilemapData,
                tileMap = mapPixel.tileMap,
                tDim = tilemapDim,
            };
            return updateTileMapDataJob.Schedule(tilemapDim * tilemapDim, 64, dependencies);
        }

        #endregion

        #region Terrain Jobs

        // Calculates average and maximum heights of terrain data
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

                        int idx = JobA.Idx(y, x, hDim);
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

        // Converts tileMap data to color array for use by shader
        struct UpdateTileMapDataJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<byte> tilemapData;
            [WriteOnly]
            public NativeArray<Color32> tileMap;

            public int tDim;

            public void Execute(int index)
            {
                int x = JobA.Row(index, tDim);
                int y = JobA.Col(index, tDim);

                // Assign tile data to tilemap
                Color32 tileColor = new Color32(0, 0, 0, 0);

                // Get sample tile data
                byte tile = tilemapData[JobA.Idx(x, y, tDim)];

                // Convert from [flip,rotate,6bit-record] => [6bit-record,flip,rotate]
                int record;
                if (tile == byte.MaxValue)
                {   // Zeros are converted to FF so assign tiles doesn't overwrite location tiles, convert back.
                    record = 0;
                }
                else
                {
                    record = tile * 4;
                    if ((tile & rotBit) != 0) record += 1;
                    if ((tile & flipBit) != 0) record += 2;
                }

                // Assign to tileMap
                tileColor.r = tileColor.a = (byte)record;
                tileMap[y * tDim + x] = tileColor;
            }
        }

        #endregion

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

        #region Helper Methods

        // Bilinear interpolation of values
        public static float BilinearInterpolator(float valx0y0, float valx0y1, float valx1y0, float valx1y1, float u, float v)
        {
            float result =
                        (1 - u) * ((1 - v) * valx0y0 +
                        v * valx0y1) +
                        u * ((1 - v) * valx1y0 +
                        v * valx1y1);

            return result;
        }

        // Cubic interpolation of values
        public static float CubicInterpolator(float v0, float v1, float v2, float v3, float fracy)
        {
            float A = (v3 - v2) - (v0 - v1);
            float B = (v0 - v1) - A;
            float C = v2 - v0;
            float D = v1;

            return A * (fracy * fracy * fracy) + B * (fracy * fracy) + C * fracy + D;           // Faster
            //return A * Mathf.Pow(fracy, 3) + B * Mathf.Pow(fracy, 2) + C * fracy + D;
        }

        // Get noise sample at coordinates
        public static float GetNoise(
            int x,
            int y,
            float frequency,
            float amplitude,
            float persistance,
            int octaves,
            int seed = 0)
        {
            float finalValue = 0f;
            for (int i = 0; i < octaves; ++i)
            {
                finalValue += Mathf.PerlinNoise(seed + x * frequency, seed + y * frequency) * amplitude;
                frequency *= 2.0f;
                amplitude *= persistance;
            }

            return Mathf.Clamp01(finalValue);
        }

        #endregion
    }
}