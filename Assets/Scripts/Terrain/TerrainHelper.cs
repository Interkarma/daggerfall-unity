// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Andrzej Łukasik (andrew.r.lukasik)
// 
// Notes:
//

using System.Linq;
using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Profiling;

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
        const int oceanClimate = 223;

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

        static readonly ProfilerMarker
            ___SmoothLocationNeighbourhood = new ProfilerMarker($"{nameof(TerrainHelper)}.{nameof(SmoothLocationNeighbourhood)}"),
            ___SmoothLocationNeighbourhoodAsync = new ProfilerMarker($"{nameof(TerrainHelper)}.{nameof(SmoothLocationNeighbourhoodAsync)}"),
            ___DilateCoastalClimate = new ProfilerMarker($"{nameof(TerrainHelper)}.{nameof(DilateCoastalClimate)}"),
            ___DilateCoastalClimateAsync = new ProfilerMarker($"{nameof(TerrainHelper)}.{nameof(DilateCoastalClimateAsync)}"),
            ___SetLocationTiles = new ProfilerMarker($"{nameof(TerrainHelper)}.{nameof(SetLocationTiles)}"),
            ___iterate_location_blocks = new ProfilerMarker("iterate location blocks"),
            ___CalcAvgMaxHeightAsync = new ProfilerMarker($"{nameof(TerrainHelper)}.{nameof(CalcAvgMaxHeightAsync)}");

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
            ___SetLocationTiles.Begin();

            // Get location
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            var mapFileReader = dfUnity.ContentReader.MapFileReader;
            var contentReader = dfUnity.ContentReader;
            DFLocation location = mapFileReader.GetLocation(mapPixel.mapRegionIndex, mapPixel.mapLocationIndex);

            // Position tiles inside terrain area
            int2 tilePos;
            {
                DFPosition tilePosObj = TerrainHelper.GetLocationTerrainTileOrigin(location);
                tilePos = new int2(tilePosObj.X, tilePosObj.Y);
            }

            // Full 8x8 locations have "terrain blend space" around walls to smooth down random terrain towards flat area.
            // This is indicated by texture index > 55 (ground texture range is 0-55), larger values indicate blend space.
            // We need to know rect of actual city area so we can use blend space outside walls.
            int2 min = int.MaxValue;
            int2 max = 0;

            // Iterate blocks of this location
            ___iterate_location_blocks.Begin();
            int
                numBlockX = location.Exterior.ExteriorData.Width,
                numBlockY = location.Exterior.ExteriorData.Height,
                numTile = RMBLayout.RMBTilesPerBlock;
            for (int blockY = 0; blockY < numBlockY; blockY++)
            for (int blockX = 0; blockX < numBlockX; blockX++)
            {
                // Get block data
                string blockName = mapFileReader.GetRmbBlockName(location, blockX, blockY);
                if (!contentReader.GetBlock(blockName, out var block))
                    continue;

                // Copy ground tile info
                var groundTiles = block.RmbBlock.FldHeader.GroundData.GroundTiles;
                for (int row = 0; row < numTile; row++)
                for (int col = 0; col < numTile; col++)
                {
                    var tile = groundTiles[row, (numTile - 1) - col];// [row,col]

                    int2 pos = tilePos + new int2(blockX, blockY) * numTile + new int2(row, col);

                    if (tile.TextureRecord < 56)
                    {
                        // Track interior bounds of location tiled area
                        min = math.min(min, pos);
                        max = math.max(max, pos);

                        // Store texture data from block
                        mapPixel.tilemapData[Matrix.Idx(pos.x, pos.y, MapsFile.WorldMapTileDim)] = tile.TileBitfield == 0 ? byte.MaxValue : tile.TileBitfield;
                    }
                }
            }
            ___iterate_location_blocks.End();

            // Update location rect with extra clearance
            int extraClearance = location.MapTableData.LocationType == DFRegion.LocationTypes.TownCity ? 3 : 2;
            Rect locationRect = new Rect();
            locationRect.xMin = min.x - extraClearance;
            locationRect.xMax = max.x + extraClearance;
            locationRect.yMin = min.y - extraClearance;
            locationRect.yMax = max.y + extraClearance;
            mapPixel.locationRect = locationRect;

            ___SetLocationTiles.End();
        }

        #region Terrain Jobs - Schedulers

        public static JobHandle CalcAvgMaxHeightAsync(MapPixelData mapPixel, JobHandle dependency = default)
        {
            ___CalcAvgMaxHeightAsync.Begin();

            CalcAvgMaxHeightJob calcAvgMaxHeightJob = new CalcAvgMaxHeightJob
            {
                heightmapData = mapPixel.heightmapData,
                avgMaxHeight = mapPixel.avgMaxHeight,
            };
            var jobHandle = calcAvgMaxHeightJob.Schedule(dependency);

            ___CalcAvgMaxHeightAsync.End();
            return jobHandle;
        }
        [System.Obsolete("use "+nameof(CalcAvgMaxHeightAsync)+" instead")]
        public static JobHandle ScheduleCalcAvgMaxHeightJob(ref MapPixelData mapPixel, JobHandle dependencies)
            => CalcAvgMaxHeightAsync(mapPixel, dependencies);

        public static JobHandle BlendLocationTerrainAsync(MapPixelData mapPixel, JobHandle dependency = default)
        {
            var blendLocationTerrainJob = new BlendLocationTerrainJob
            {
                HeightmapData = mapPixel.heightmapData,
                AvgMaxHeight = mapPixel.avgMaxHeight,
                HDim = DaggerfallUnity.Instance.TerrainSampler.HeightmapDimension,
                LocationRect = mapPixel.locationRect,
            };
            int extraBlendSpace = ExtraBlendSpace(mapPixel.LocationType);
            if (extraBlendSpace > 0)
            {
                blendLocationTerrainJob.LocationRect.xMin -= extraBlendSpace;
                blendLocationTerrainJob.LocationRect.xMax += extraBlendSpace;
                blendLocationTerrainJob.LocationRect.yMin -= extraBlendSpace;
                blendLocationTerrainJob.LocationRect.yMax += extraBlendSpace;
            }
            return blendLocationTerrainJob.Schedule(dependency);
        }
        [System.Obsolete("use "+nameof(BlendLocationTerrainAsync)+" instead")]
        public static JobHandle ScheduleBlendLocationTerrainJob(ref MapPixelData mapPixel, JobHandle dependencies)
            => BlendLocationTerrainAsync(mapPixel, dependencies);

        public static JobHandle UpdateTileMapDataAsync(MapPixelData mapPixel, JobHandle dependency = default)
        {
            int tilemapDim = MapsFile.WorldMapTileDim;
            bool convertWater = DaggerfallUnity.Instance.TerrainTexturing.ConvertWaterTiles();
            var updateTileMapDataJob = new UpdateTileMapDataJob
            {
                tilemapData = mapPixel.tilemapData,
                tileMap = mapPixel.tileMap,
                tDim = tilemapDim,
                convertWater = convertWater,
            };
            return updateTileMapDataJob.Schedule(tilemapDim * tilemapDim, 64, dependency);
        }
        [System.Obsolete("use "+nameof(BlendLocationTerrainAsync)+" instead")]
        public static JobHandle ScheduleUpdateTileMapDataJob(ref MapPixelData mapPixel, JobHandle dependencies)
            => UpdateTileMapDataAsync(mapPixel, dependencies);

        #endregion

        #region Terrain Jobs

        // Calculates average and maximum heights of terrain data
        [Unity.Burst.BurstCompile]
        struct CalcAvgMaxHeightJob : IJob
        {
            [ReadOnly] public NativeArray<float> heightmapData;
            public NativeArray<float> avgMaxHeight;
            void IJob.Execute()
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
        [Unity.Burst.BurstCompile]
        struct BlendLocationTerrainJob : IJob
        {
            public NativeArray<float> HeightmapData;
            [ReadOnly] public NativeArray<float> AvgMaxHeight;
            public int HDim;
            public Rect LocationRect;
            void IJob.Execute()
            {
                // Convert from rect in tilemap space to interior corners in 0-1 range
                float4 minmax = new float4(LocationRect.xMin, LocationRect.xMax, LocationRect.yMin, LocationRect.yMax) / MapsFile.WorldMapTileDim;
                float xMin = minmax.x;
                float xMax = minmax.y;
                float yMin = minmax.z;
                float yMax = minmax.w;

                // Scale values for converting blend space into 0-1 range
                float4 scale = 1f / new float4(xMin, (1 - xMax), yMin, (1 - yMax));
                float leftScale = scale.x;
                float rightScale = scale.y;
                float topScale = scale.z;
                float bottomScale = scale.w;

                // Flatten location area and blend with surrounding heights
                float targetHeight = AvgMaxHeight[avgHeightIdx];
                for (int y = 0; y < HDim; y++)
                {
                    float v = (float)y / (float)(HDim - 1);
                    bool insideY = (v >= yMin && v <= yMax);

                    for (int x = 0; x < HDim; x++)
                    {
                        float u = (float)x / (float)(HDim - 1);
                        bool insideX = (u >= xMin && u <= xMax);
                        float strength = 0;

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

                        int idx = Matrix.Idx(y, x, HDim);
                        float height = HeightmapData[idx];

                        if (insideX && insideY)
                            height = targetHeight;
                        else
                            height = Mathf.Lerp(height, targetHeight, strength);

                        HeightmapData[idx] = height;
                    }
                }
            }
        }

        // Converts tileMap data to color array for use by shader
        [Unity.Burst.BurstCompile]
        struct UpdateTileMapDataJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<byte> tilemapData;
            [WriteOnly] public NativeArray<Color32> tileMap;
            public int tDim;
            public bool convertWater;
            void IJobParallelFor.Execute(int index)
            {
                int x = Matrix.Row(index, tDim);
                int y = Matrix.Col(index, tDim);

                // Assign tile data to tilemap
                Color32 tileColor = new Color32(0, 0, 0, 0);

                // Get sample tile data
                byte tile = tilemapData[Matrix.Idx(x, y, tDim)];

                // Convert from [flip,rotate,6bit-record] => [6bit-record,flip,rotate]
                int record;
                if (convertWater && tile == byte.MaxValue)
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

        // Copies climate data from source to destination if destination is ocean
        // Used to dilate climate data around shorelines as a pre-process
        [Unity.Burst.BurstCompile]
        struct DilateCoastalClimateJob : IJob
        {
            public int Passes, XMax, YMax, PakWidth;
            public NativeArray<byte> Climate;
            void IJob.Execute()
            {
                var previousPass = new NativeArray<byte>(Climate, Allocator.Temp);
                for (int pass = 0; pass < Passes; pass++)
                {
                    // Dilate coastal areas
                    for (int y = 1; y < YMax; y++)
                    for (int x = 1; x < XMax; x++)
                    {
                        // Source must be land
                        int srcOffset = y * PakWidth + (x + 1);
                        byte srcClimate = previousPass[srcOffset];
                        if (srcClimate == oceanClimate)
                            continue;
                        
                        // Transfer climate of this pixel to any ocean pixel in Moore neighbourhood
                        TransferLandToOcean(previousPass, Climate, srcClimate, x - 1, y - 1,PakWidth);
                        TransferLandToOcean(previousPass, Climate, srcClimate, x, y - 1,PakWidth);
                        TransferLandToOcean(previousPass, Climate, srcClimate, x + 1, y - 1,PakWidth);
                        TransferLandToOcean(previousPass, Climate, srcClimate, x - 1, y,PakWidth);
                        TransferLandToOcean(previousPass, Climate, srcClimate, x + 1, y,PakWidth);
                        TransferLandToOcean(previousPass, Climate, srcClimate, x - 1, y + 1,PakWidth);
                        TransferLandToOcean(previousPass, Climate, srcClimate, x, y + 1,PakWidth);
                        TransferLandToOcean(previousPass, Climate, srcClimate, x + 1, y + 1,PakWidth);
                    }
                }
            }
            void TransferLandToOcean(NativeArray<byte> previousPass, NativeArray<byte> currentPass, byte srcClimate, int dstX, int dstY, int pakWidth)
            {
                // Destination must be ocean
                int dstOffset = dstY * pakWidth + (dstX + 1);
                byte dstClimate = previousPass[dstOffset];
                if (dstClimate == oceanClimate)
                    currentPass[dstOffset] = srcClimate;
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
            ___DilateCoastalClimate.Begin();

            for (int pass = 0; pass < passes; pass++)
            {
                // Get clone of in-memory climate array
                byte[] climateArray = contentReader.MapFileReader.ClimateFile.Buffer.Clone() as byte[];

                // Dilate coastal areas
                int
                    mapHeight = WoodsFile.mapHeightValue,
                    mapWidth = WoodsFile.mapWidthValue,
                    xmax = mapWidth - 1,
                    ymax = mapHeight - 1;
                for (int y = 1; y < ymax; y++)
                for (int x = 1; x < xmax; x++)
                {
                    // Transfer climate of this pixel to any ocean pixel in Moore neighbourhood
                    transferLandToOcean(contentReader, climateArray, x, y, x - 1, y - 1);
                    transferLandToOcean(contentReader, climateArray, x, y, x, y - 1);
                    transferLandToOcean(contentReader, climateArray, x, y, x + 1, y - 1);
                    transferLandToOcean(contentReader, climateArray, x, y, x - 1, y);
                    transferLandToOcean(contentReader, climateArray, x, y, x + 1, y);
                    transferLandToOcean(contentReader, climateArray, x, y, x - 1, y + 1);
                    transferLandToOcean(contentReader, climateArray, x, y, x, y + 1);
                    transferLandToOcean(contentReader, climateArray, x, y, x + 1, y + 1);
                }

                // Store modified climate array
                contentReader.MapFileReader.ClimateFile.Buffer = climateArray;
            }

            // Copies climate data from source to destination if destination is ocean
            // Used to dilate climate data around shorelines as a pre-process
            void transferLandToOcean(ContentReader cr, byte[] dstClimateArray, int srcX, int srcY, int dstX, int dstY)
            {
                // Source must be land
                int srcOffset = srcY * PakFile.pakWidthValue + (srcX + 1);
                int srcClimate = cr.MapFileReader.ClimateFile.Buffer[srcOffset];
                if (srcClimate == oceanClimate)
                    return;

                // Destination must be ocean
                int dstOffset = dstY * PakFile.pakWidthValue + (dstX + 1);
                int dstClimate = cr.MapFileReader.ClimateFile.Buffer[dstOffset];
                if (dstClimate == oceanClimate)
                    dstClimateArray[dstOffset] = (byte)srcClimate;
            }

            ___DilateCoastalClimate.End();
        }
        public static JobHandle DilateCoastalClimateAsync( ContentReader contentReader, int passes, JobHandle dependency = default )
        {
            ___DilateCoastalClimateAsync.Begin();

            NativeArray<byte> climateFileBuffer = contentReader.MapFileReader.ClimateFile.Buffer.AsNativeArray(out ulong gcHandle);
            var job = new DilateCoastalClimateJob
            {
                Passes = passes,
                XMax = WoodsFile.mapWidthValue - 1,
                YMax = WoodsFile.mapHeightValue - 1,
                PakWidth = PakFile.pakWidthValue,
                Climate = climateFileBuffer
            };
            var jobHandle = job.Schedule(dependency);
            JobUtility.ReleaseGCObject(gcHandle, jobHandle);

            ___DilateCoastalClimateAsync.End();
            return jobHandle;
        }

        /// <summary>
        /// If a location map pixel is on a gradient greater than threshold, then
        /// smooth surrounding Moore neighbourhood with location height
        /// </summary>
        public static void SmoothLocationNeighbourhood(ContentReader contentReader, int threshold = 20)
        {
            ___SmoothLocationNeighbourhood.Begin();

            // Get in-memory height array
            byte[] heightArray = contentReader.WoodsFileReader.Buffer;

            // Search for locations
            int
                mapHeight = WoodsFile.mapHeightValue,
                mapWidth = WoodsFile.mapWidthValue,
                xmax = mapWidth - 1,
                ymax = mapHeight - 1;
            for (int y = 1; y < ymax; y++)
            for (int x = 1; x < xmax; x++)
            if (contentReader.HasLocation(x, y, out var summary))
            {
                // Use Sobel filter for gradient
                float x0y0 = heightArray[y * mapWidth + x];
                float x1y0 = heightArray[y * mapWidth + (x + 1)];
                float x0y1 = heightArray[(y + 1) * mapWidth + x];
                float gradient = GetGradient(x0y0, x1y0, x0y1);
                if (gradient > threshold)
                    AverageHeights(heightArray, x, y, mapWidth);
            }

            ___SmoothLocationNeighbourhood.End();
        }
        public static JobHandle SmoothLocationNeighbourhoodAsync(ContentReader contentReader, int threshold = 20, JobHandle dependency = default)
        {
            ___SmoothLocationNeighbourhoodAsync.Begin();
            
            // get locations hashset
            NativeArray<int> locations = contentReader.MapSummaries.Keys.ToArray().AsNativeArray(out ulong gcHandleLoc);

            // Get in-memory height array
            NativeArray<byte> woodsFileReaderBuffer = contentReader.WoodsFileReader.Buffer.AsNativeArray(out ulong gcHandleBuff);
            var job = new SmoothLocationNeighbourhoodJob
            {
                Locations = locations,
                HeightArray = woodsFileReaderBuffer,
                MapWidth = WoodsFile.mapWidthValue,
                MapHeight = WoodsFile.mapHeightValue,
                Threshold = threshold,
            };
            var jobHandle = job.Schedule(dependency);
            JobUtility.ReleaseGCObject(gcHandleLoc, jobHandle);
            JobUtility.ReleaseGCObject(gcHandleBuff, jobHandle);

            ___SmoothLocationNeighbourhoodAsync.End();
            return jobHandle;
        }

        [Unity.Burst.BurstCompile]
        struct SmoothLocationNeighbourhoodJob : IJob
        {
            [ReadOnly] public NativeArray<int> Locations;
            public NativeArray<byte> HeightArray;
            public int MapWidth, MapHeight;
            public float Threshold;
            void IJob.Execute()
            {
                // @TODO: change to NativeHashSet<int> once collections package becomes >= v0.11 (https://docs.unity3d.com/Packages/com.unity.collections@0.11/api/Unity.Collections.NativeHashSet-1.html)
                var locationsHashset = new NativeHashMap<int, byte>(Locations.Length, Allocator.Temp);
                foreach( int id in Locations )
                    locationsHashset.TryAdd(id, default);

                // Search for locations
                for (int y = 1; y < MapHeight - 1; y++)
                for (int x = 1; x < MapWidth - 1; x++)
                if (locationsHashset.ContainsKey(MapsFile.GetMapPixelID(x, y)))
                {
                    // Use Sobel filter for gradient
                    float x0y0 = HeightArray[y * MapWidth + x];
                    float x1y0 = HeightArray[y * MapWidth + (x + 1)];
                    float x0y1 = HeightArray[(y + 1) * MapWidth + x];
                    float gradient = GetGradient(x0y0, x1y0, x0y1);
                    if (gradient > Threshold)
                        AverageHeights(HeightArray, x, y, MapWidth);
                }
            }
        }

        // Gets terrain key based on map pixel coordinates
        public static int MakeTerrainKey(int mapPixelX, int mapPixelY)
            => ((short)mapPixelY << 16) + (short)mapPixelX;

        // Reverse terrain key back to map pixel coordinates
        public static void ReverseTerrainKey(int key, out int mapPixelX, out int mapPixelY)
        {
            mapPixelY = key >> 16;
            mapPixelX = key & 0xffff;
        }

        // Gets fast gradient value from heights
        public static float GetGradient(float x0y0, float x1y0, float x0y1)
        {
            float2 d = new float2(x1y0, x0y1) - x0y0;
            return math.csum(math.abs(d));// Faster
            // return math.sqrt(math.csum(d*d));// More accurate
        }
        public static float GetGradient_old(float x0y0, float x1y0, float x0y1)
        {
            float dx = x1y0 - x0y0;
            float dy = x0y1 - x0y0;
            return Mathf.Abs(dx) + Mathf.Abs(dy);           // Faster
            //return Mathf.Sqrt(dx * dx + dy * dy);         // More accurate
        }

        // Average centre coordinate height with surrounding heights
        public static void AverageHeights(byte[] heightArray, int cx, int cy, int mapWidth)
        {
            // First pass averages
            float average = 0;
            float counter = 0;
            {
                for (int y = cy - 1; y < cy + 2; y++)
                for (int x = cx - 1; x < cx + 2; x++)
                {
                    average += heightArray[y * mapWidth + x];
                    counter++;
                }
            }

            // Next pass applies average
            average /= counter;
            {
                byte value = (byte)average;
                for (int y = cy - 1; y < cy + 2; y++)
                for (int x = cx - 1; x < cx + 2; x++)
                    heightArray[y * mapWidth + x] = value;
            }
        }
        public static void AverageHeights(NativeArray<byte> heightArray, int cx, int cy, int mapWidth)
        {
            // First pass averages
            float average = 0;
            float counter = 0;
            {
                for (int y = cy - 1; y < cy + 2; y++)
                for (int x = cx - 1; x < cx + 2; x++)
                {
                    average += heightArray[y * mapWidth + x];
                    counter++;
                }
            }

            // Next pass applies average
            average /= counter;
            {
                byte value = (byte)average;
                for (int y = cy - 1; y < cy + 2; y++)
                for (int x = cx - 1; x < cx + 2; x++)
                    heightArray[y * mapWidth + x] = value;
            }
        }

        #region Helper Methods

        public static float BilinearInterpolator_old(float valx0y0, float valx0y1, float valx1y0, float valx1y1, float u, float v)
        {
            float result =
                        (1 - u) * ((1 - v) * valx0y0 +
                        v * valx0y1) +
                        u * ((1 - v) * valx1y0 +
                        v * valx1y1);

            return result;
        }
        public static float BilinearInterpolator(float valx0y0, float valx0y1, float valx1y0, float valx1y1, float u, float v)
        {
            // return
            //     (1 - u) * ((1 - v) * valx0y0 + v * valx0y1) +
            //     u * ((1 - v) * valx1y0 + v * valx1y1);

            float2 one_sub_uv = new float2(1) - new float2(u, v);
            // return
            //     one_sub_uv.x * (one_sub_uv.y * valx0y0 + v * valx0y1) +
            //     u * (one_sub_uv.y * valx1y0 + v * valx1y1);

            float4 mul1 = new float4(one_sub_uv.y, v, one_sub_uv.y, v) * new float4(valx0y0, valx0y1, valx1y0, valx1y1);
            float2 add = new float2(mul1.x, mul1.z) + new float2(mul1.y, mul1.w);
            // return
            //     one_sub_uv.x * add.x +
            //     u * add.y;

            float2 mul2 = new float2(one_sub_uv.x, u) * new float2(add.x, add.y);
            return math.csum(mul2);
        }

        public static float CubicInterpolator_old(float v0, float v1, float v2, float v3, float fracy)
        {
            float A = (v3 - v2) - (v0 - v1);
            float B = (v0 - v1) - A;
            float C = v2 - v0;
            float D = v1;

            return A * (fracy * fracy * fracy) + B * (fracy * fracy) + C * fracy + D;           // Faster
            //return A * Mathf.Pow(fracy, 3) + B * Mathf.Pow(fracy, 2) + C * fracy + D;
        }
        public static float CubicInterpolator(float v0, float v1, float v2, float v3, float fracy)
        {
            // float a = (v3 - v2) - (v0 - v1);
            // float b = (v0 - v1) - a;
            // float c = v2 - v0;
            // float d = v1;

            float3 sub = new float3(v3, v0, v2) - new float3(v2, v1, v0);
            // float a = sub.x - sub.y;
            // float b = sub.y - a;
            // float c = sub.z;

            float a = sub.x - sub.y;
            float b = sub.y - a;
            float c = sub.z;
            float d = v1;

            // return a * (fracy * fracy * fracy) + b * (fracy * fracy) + c * fracy + d;
            return math.csum(new float4(a, b, c, d) * new float4(fracy, fracy, fracy, 1) * new float4(fracy * fracy, fracy, 1, 1));
        }

        // Get noise sample at coordinates
        public static float GetNoise_old(int x, int y, float frequency, float amplitude, float persistance, int octaves, int seed = 0)
        {
            float finalValue = 0f;
            for (int i = 0; i < octaves; ++i)
            {
                finalValue += Mathf.PerlinNoise(seed + x * frequency, seed + y * frequency) * amplitude;
                frequency *= 2.0f;
                amplitude *= persistance;
            }
            return math.saturate(finalValue);
        }
        public static float GetNoise(int x, int y, float frequency, float amplitude, float persistance, int octaves, int seed = 0)
        {
            float finalValue = 0f;
            for (int i = 0; i < octaves; ++i)
            {
                finalValue += PerlinNoise(seed + x * frequency, seed + y * frequency) * amplitude;
                frequency *= 2.0f;
                amplitude *= persistance;
            }
            return math.saturate(finalValue);
        }

        public static float PerlinNoise(float x, float y)
            => (noise.cnoise(new float2(x, y)) + 1f) * 0.5f;

        #endregion
    }
}