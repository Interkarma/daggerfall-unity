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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Helper methods for terrain generation.
    /// </summary>
    public static class TerrainHelper
    {
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

        // Set all sample tiles to same base index
        public static void FillTilemapSamples(ref MapPixelData mapPixel, byte record)
        {
            for (int y = 0; y < MapsFile.WorldMapTileDim; y++)
            {
                for (int x = 0; x < MapsFile.WorldMapTileDim; x++)
                {
                    mapPixel.tilemapSamples[x, y].record = record;
                }
            }
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

            // But some 1x1 locations (e.g. Privateer's Hold exterior) are positioned differently
            // Seems to be 1x1 blocks using CUST prefix, but possibly more research needed
            const int custPrefixIndex = 40;
            if (width == 1 && height == 1)
            {
                if (location.Exterior.ExteriorData.BlockIndex[0] == custPrefixIndex)
                {
                    result.X = 72;
                    result.Y = 55;
                }
            }

            return result;
        }

        // Set location tilemap data
        public static void SetLocationTiles(ref MapPixelData mapPixel)
        {
            //const int tileDim = 16;
            //const int chunkDim = 8;

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Get location
            DFLocation location = dfUnity.ContentReader.MapFileReader.GetLocation(mapPixel.mapRegionIndex, mapPixel.mapLocationIndex);

            // Centre location tiles inside terrain area
            //int startX = ((chunkDim * tileDim) - location.Exterior.ExteriorData.Width * tileDim) / 2;
            //int startY = ((chunkDim * tileDim) - location.Exterior.ExteriorData.Height * tileDim) / 2;

            // Position tiles inside terrain area
            //int width = location.Exterior.ExteriorData.Width;
            //int height = location.Exterior.ExteriorData.Height;
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

                            int record = tile.TextureRecord;
                            if (tile.TextureRecord < 56)
                            {
                                // Track interior bounds of location tiled area
                                if (xpos < xmin) xmin = xpos;
                                if (xpos > xmax) xmax = xpos;
                                if (ypos < ymin) ymin = ypos;
                                if (ypos > ymax) ymax = ypos;

                                // Store texture data from block
                                mapPixel.tilemapSamples[xpos, ypos].record = record;
                                mapPixel.tilemapSamples[xpos, ypos].flip = tile.IsFlipped;
                                mapPixel.tilemapSamples[xpos, ypos].rotate = tile.IsRotated;
                                mapPixel.tilemapSamples[xpos, ypos].location = true;
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

        // Set location tilemap data
        public static void SetLocationTilesJobs(ref MapPixelDataJobs mapPixel)
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

                            int record = tile.TextureRecord;
                            if (tile.TextureRecord < 56)
                            {
                                // Track interior bounds of location tiled area
                                if (xpos < xmin) xmin = xpos;
                                if (xpos > xmax) xmax = xpos;
                                if (ypos < ymin) ymin = ypos;
                                if (ypos > ymax) ymax = ypos;

                                // Store texture data from block
                                mapPixel.tilemapSamples[JobA.Idx(xpos, ypos, MapsFile.WorldMapTileDim)] = tile.TileBitfield == 0 ? byte.MaxValue : tile.TileBitfield;
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

        // Flattens location terrain and blends with surrounding terrain
        public static void BlendLocationTerrain(ref MapPixelData mapPixel, float noiseStrength = 4f)
        {
            int heightmapDimension = DaggerfallUnity.Instance.TerrainSampler.HeightmapDimension;

            // Convert from rect in tilemap space to interior corners in 0-1 range
            float xMin = mapPixel.locationRect.xMin / MapsFile.WorldMapTileDim;
            float xMax = mapPixel.locationRect.xMax / MapsFile.WorldMapTileDim;
            float yMin = mapPixel.locationRect.yMin / MapsFile.WorldMapTileDim;
            float yMax = mapPixel.locationRect.yMax / MapsFile.WorldMapTileDim;

            // Scale values for converting blend space into 0-1 range
            float leftScale = 1 / xMin;
            float rightScale = 1 / (1 - xMax);
            float topScale = 1 / yMin;
            float bottomScale = 1 / (1 - yMax);

            // Flatten location area and blend with surrounding heights
            float strength = 0;
            float targetHeight = mapPixel.averageHeight;
            for (int y = 0; y < heightmapDimension; y++)
            {
                float v = (float)y / (float)(heightmapDimension - 1);
                bool insideY = (v >= yMin && v <= yMax);

                for (int x = 0; x < heightmapDimension; x++)
                {
                    float u = (float)x / (float)(heightmapDimension - 1);
                    bool insideX = (u >= xMin && u <= xMax);

                    float height = mapPixel.heightmapSamples[y, x];

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
                        strength = BilinearInterpolator(0, 0, 0, 1, xs, ys);
                    }

                    if (insideX && insideY)
                        height = targetHeight;
                    else
                        height = Mathf.Lerp(height, targetHeight, strength);

                    mapPixel.heightmapSamples[y, x] = height;
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
            const float baseChanceOnDirt = 0.2f;        // 0.2
            const float baseChanceOnGrass = 0.9f;       // 0.4
            const float baseChanceOnStone = 0.05f;      // 0.05

            // Location Rect is expanded slightly to give extra clearance around locations
            const int natureClearance = 4;
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
            float chanceOnDirt = baseChanceOnDirt * elevationScale * climateScale;
            float chanceOnGrass = baseChanceOnGrass * elevationScale * climateScale;
            float chanceOnStone = baseChanceOnStone * elevationScale * climateScale;

            int heightmapDimension = DaggerfallUnity.Instance.TerrainSampler.HeightmapDimension;

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
            MeshReplacement.ClearNatureGameObjects(terrain);

            // Seed random with terrain key
            Random.InitState(MakeTerrainKey(dfTerrain.MapPixelX, dfTerrain.MapPixelY));

            // Just layout some random flats spread evenly across entire map pixel area
            // Flats are aligned with tiles, max 16129 billboards per batch
            Vector2 tilePos = Vector2.zero;
            int dim = MapsFile.WorldMapTileDim;
            float scale = terrainData.heightmapScale.x * (float)heightmapDimension / (float)dim;
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
                    if (rect.Contains(tilePos))
                        continue;

                    // Chance also determined by tile type
                    TilemapSample sample = dfTerrain.MapData.tilemapSamples[x, y];
                    if (sample.record == 1)
                    {
                        // Dirt
                        if (UnityEngine.Random.Range(0f, 1f) > chanceOnDirt)
                            continue;
                    }
                    else if (sample.record == 2)
                    {
                        // Grass
                        if (UnityEngine.Random.Range(0f, 1f) > chanceOnGrass)
                            continue;
                    }
                    else if (sample.record == 3)
                    {
                        // Stone
                        if (UnityEngine.Random.Range(0f, 1f) > chanceOnStone)
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
                    float beachLine = DaggerfallUnity.Instance.TerrainSampler.BeachElevation * terrainScale;
                    if (height < beachLine)
                        continue;

                    // Add to batch
                    int record = UnityEngine.Random.Range(1, 32);
                    if (!MeshReplacement.ImportNatureGameObject(dfBillboardBatch.TextureArchive, record, terrain, x, y))
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

        public static float GetClampedHeight(ref MapPixelData mapPixel, int heightmapDimension, float u, float v)
        {
            int x = (int)Mathf.Clamp(heightmapDimension * u, 0, heightmapDimension - 1);
            int y = (int)Mathf.Clamp(heightmapDimension * v, 0, heightmapDimension - 1);

            return mapPixel.heightmapSamples[y, x];
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