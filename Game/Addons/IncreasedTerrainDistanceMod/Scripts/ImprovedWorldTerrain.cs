//Increased Terrain Distance Mod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)
//Version: 1.54

//#define CREATE_PERSISTENT_LOCATION_RANGE_MAPS
//#define CREATE_PERSISTENT_TREE_COVERAGE_MAP
#define LOAD_TREE_COVERAGE_MAP

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text; // for Encoding.UTF8
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;

namespace ProjectIncreasedTerrainDistance
{
    /// <summary>    
    /// </summary>
    public static class ImprovedWorldTerrain
    {
        const string filepathInTreeCoverageMap = "./Assets/daggerfall-unity/Game/Addons/IncreasedTerrainDistanceMod/Resources/mapTreeCoverage_in.bin";

        const string filepathOutTreeCoverageMap = "./Assets/daggerfall-unity/Game/Addons/IncreasedTerrainDistanceMod/Resources/mapTreeCoverage_out.bin";

        const string filepathMapLocationRangeX = "./Assets/daggerfall-unity/Game/Addons/IncreasedTerrainDistanceMod/Resources/mapLocationRangeX.bin";
        const string filepathMapLocationRangeY = "./Assets/daggerfall-unity/Game/Addons/IncreasedTerrainDistanceMod/Resources/mapLocationRangeY.bin";

        const float minDistanceFromWaterForExtraExaggeration = 3.0f; // when does exaggeration start in terms of how far does terrain have to be away from water
        const float exaggerationFactorWaterDistance = 0.15f; // how strong is the distance from water incorporated into the multiplier
        const float extraExaggerationFactorLocationDistance = 0.00075f; // how strong is the distance from locations incorporated into the multiplier
        const float maxHeightsExaggerationMultiplier = 25.0f; // this directly affects maxTerrainHeight in TerrainHelper.cs: maxTerrainHeight should be maxHeightsExaggerationMultiplier * baseHeightScale * 128 + noiseMapScale * 128 + extraNoiseScale

        // 2D distance transform image - squared distance to water pixels of the world map
        private static float[] mapDistanceSquaredFromWater = null;

        // 2D distance transform image - squared distance to world map pixels with location
        private static float[] mapDistanceSquaredFromLocations = null;

        // map of multiplier values
        private static float[] mapMultipliers = null;

        // map with location positions
        private static byte[] mapLocations = null;

        // map with tree coverage
        private static byte[] mapTreeCoverage = null;

        private static byte[] mapLocationRangeX = null;
        private static byte[] mapLocationRangeY = null;

        // indicates if improved terrain is initialized (InitImprovedWorldTerrain() function was called)
        private static bool init = false;

        public static bool IsInit { get { return init; } }

        public static void Unload()
        {
            mapDistanceSquaredFromWater = null;
            mapDistanceSquaredFromLocations = null;
            mapMultipliers = null;
            mapLocations = null;
            mapTreeCoverage = null;
            mapLocationRangeX = null;
            mapLocationRangeY = null;

            Resources.UnloadUnusedAssets();

            System.GC.Collect();
        }

        /// <summary>
        /// Gets or sets map with location positions
        /// </summary>
        public static byte[] MapLocations
        {
            get
            {
                return mapLocations;
            }
            set { mapLocations = value; }
        }

        public static byte[] MapTreeCoverage
        {
            get
            {
                return mapTreeCoverage;
            }
            set { mapTreeCoverage = value; }
        }

        public static byte[] MapLocationRangeX
        {
            get
            {
                return mapLocationRangeX;
            }
            set { mapLocationRangeX = value; }
        }

        public static byte[] MapLocationRangeY
        {
            get
            {
                return mapLocationRangeY;
            }
            set { mapLocationRangeY = value; }
        }

        /// <summary>
        /// Gets or sets map with squared distance to water pixels.
        /// </summary>
        public static float[] MapDistanceSquaredFromWater
        {
            get {
                return mapDistanceSquaredFromWater;
            }
            set { mapDistanceSquaredFromWater = value; }
        }

        /// <summary>
        /// gets the distance to water for a given world map pixel.
        /// </summary>
        public static float getDistanceFromWater(int mapPixelX, int mapPixelY)
        {
            if (init)
            {
                return ((float)Math.Sqrt(mapDistanceSquaredFromWater[mapPixelY * WoodsFile.mapWidthValue + mapPixelX]));
            }
            else
            {
                DaggerfallUnity.LogMessage("ImprovedWorldTerrain not initialized.", true);
                return (1.0f);
            }
        }

        /// <summary>
        /// computes the height multiplier for a given world map pixel.
        /// </summary>
        public static float computeHeightMultiplier(int mapPixelX, int mapPixelY)
        {
            if (init)
            {
                return (mapMultipliers[mapPixelY * WoodsFile.mapWidthValue + mapPixelX]);
            }
            else
            {
                return (1.0f);
            }
        }

        /// <summary>
        /// initializes resources (mapDistanceSquaredFromWater, mapDistanceSquaredFromLocations, mapMultipliers) and smoothes small height map
        /// </summary>
        public static void InitImprovedWorldTerrain(ContentReader contentReader)
        {
            if (!init)
            {
                #if CREATE_PERSISTENT_LOCATION_RANGE_MAPS
                {
                    int width = WoodsFile.mapWidthValue;
                    int height = WoodsFile.mapHeightValue;

                    mapLocationRangeX = new byte[width * height];
                    mapLocationRangeY = new byte[width * height];

                    //int y = 204;
                    //int x = 718;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            //MapPixelData MapData = TerrainHelper.GetMapPixelData(contentReader, x, y);
                            //if (MapData.hasLocation)
                            //{
                            //    int locationRangeX = (int)MapData.locationRect.xMax - (int)MapData.locationRect.xMin;
                            //    int locationRangeY = (int)MapData.locationRect.yMax - (int)MapData.locationRect.yMin;
                            //}

                            ContentReader.MapSummary mapSummary;
                            int regionIndex = -1, mapIndex = -1;
                            bool hasLocation = contentReader.HasLocation(x, y, out mapSummary);
                            if (hasLocation)
                            {   
                                regionIndex = mapSummary.RegionIndex;
                                mapIndex = mapSummary.MapIndex;
                                DFLocation location = contentReader.MapFileReader.GetLocation(regionIndex, mapIndex);
                                byte locationRangeX = location.Exterior.ExteriorData.Width;
                                byte locationRangeY = location.Exterior.ExteriorData.Height;

                                mapLocationRangeX[y * width + x] = locationRangeX;
                                mapLocationRangeY[y * width + x] = locationRangeY;
                            }
                        }
                    }
              
                    // save to files
                    FileStream ostream;
                    ostream = new FileStream("./Assets/IncreasedTerrainDistance/Resources/mapLocationRangeX.bin", FileMode.Create, FileAccess.Write);
                    BinaryWriter writerMapLocationRangeX = new BinaryWriter(ostream, Encoding.UTF8);
                    writerMapLocationRangeX.Write(mapLocationRangeX, 0, width * height);
                    writerMapLocationRangeX.Close();
                    ostream.Close();

                    ostream = new FileStream("./Assets/IncreasedTerrainDistance/Resources/mapLocationRangeY.bin", FileMode.Create, FileAccess.Write);
                    BinaryWriter writerMapLocationRangeY = new BinaryWriter(ostream, Encoding.UTF8);
                    writerMapLocationRangeY.Write(mapLocationRangeY, 0, width * height);
                    writerMapLocationRangeY.Close();
                    ostream.Close();
                }
#else
                {
                    int width = WoodsFile.mapWidthValue;
                    int height = WoodsFile.mapHeightValue;

                    mapLocationRangeX = new byte[width * height];
                    mapLocationRangeY = new byte[width * height];

                    FileStream istream;
                    istream = new FileStream(filepathMapLocationRangeX, FileMode.Open, FileAccess.Read);
                    BinaryReader readerMapLocationRangeX = new BinaryReader(istream, Encoding.UTF8);
                    readerMapLocationRangeX.Read(mapLocationRangeX, 0, width * height);
                    readerMapLocationRangeX.Close();
                    istream.Close();

                    istream = new FileStream(filepathMapLocationRangeY, FileMode.Open, FileAccess.Read);
                    BinaryReader readerMapLocationRangeY = new BinaryReader(istream, Encoding.UTF8);
                    readerMapLocationRangeY.Read(mapLocationRangeY, 0, width * height);
                    readerMapLocationRangeY.Close();
                    istream.Close();
                }
                #endif

                if (mapDistanceSquaredFromWater == null)
                {
                    byte[] heightMapArray = contentReader.WoodsFileReader.Buffer.Clone() as byte[];
                    int width = WoodsFile.mapWidthValue;
                    int height = WoodsFile.mapHeightValue;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (heightMapArray[y * width + x] <= 2)
                                heightMapArray[y * width + x] = 1;
                            else
                                heightMapArray[y * width + x] = 0;
                        }
                    }
                    //now set image borders to "water" (this is a workaround to prevent mountains to become too high in north-east and south-east edge of map)
                    for (int y = 0; y < height; y++)
                    {
                        heightMapArray[y * width + 0] = 1;
                        heightMapArray[y * width + width - 1] = 1;
                    }
                    for (int x = 0; x < width; x++)
                    {
                        heightMapArray[0 * width + x] = 1;
                        heightMapArray[(height - 1) * width + x] = 1;
                    }

                    mapDistanceSquaredFromWater = imageDistanceTransform(heightMapArray, width, height, 1);

                    heightMapArray = null;
                }

                if (mapDistanceSquaredFromLocations == null)
                {
                    int width = WoodsFile.mapWidthValue;
                    int height = WoodsFile.mapHeightValue;
                    mapLocations = new byte[width * height];

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            ContentReader.MapSummary summary;
                            if (contentReader.HasLocation(x + 1, height - 1 - y, out summary))
                                mapLocations[y * width + x] = 1;
                            else
                                mapLocations[y * width + x] = 0;
                        }
                    }
                    mapDistanceSquaredFromLocations = imageDistanceTransform(mapLocations, width, height, 1);
                }



                if (mapMultipliers == null)
                {
                    int width = WoodsFile.mapWidthValue;
                    int height = WoodsFile.mapHeightValue;
                    mapMultipliers = new float[width * height];

                    // compute the multiplier and store it in mapMultipliers
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            float distanceFromWater = (float)Math.Sqrt(mapDistanceSquaredFromWater[y * WoodsFile.mapWidthValue + x]);
                            float distanceFromLocation = (float)Math.Sqrt(mapDistanceSquaredFromLocations[y * WoodsFile.mapWidthValue + x]);
                            float multiplierLocation = (distanceFromLocation * extraExaggerationFactorLocationDistance + 1.0f); // terrain distant from location gets extra exaggeration
                            if (distanceFromWater < minDistanceFromWaterForExtraExaggeration) // except if it is near water
                                multiplierLocation = 1.0f;
                            mapMultipliers[y * width + x] = (Math.Min(maxHeightsExaggerationMultiplier, multiplierLocation * Math.Max(1.0f, distanceFromWater * exaggerationFactorWaterDistance)));
                        }
                    }

                    // multipliedMap gets smoothed
                    float[] newmapMultipliers = mapMultipliers.Clone() as float[];
                    float[,] weights = { { 0.0625f, 0.125f, 0.0625f }, { 0.125f, 0.25f, 0.125f }, { 0.0625f, 0.125f, 0.0625f } };
                    for (int y = 1; y < height - 1; y++)
                    {
                        for (int x = 1; x < width - 1; x++)
                        {
                            if (mapDistanceSquaredFromLocations[y * width + x] <= 2) // at and around locations ( <= 2 ... only map pixels in 8-connected neighborhood (distanceFromLocationMaps stores squared distances...))
                            {
                                newmapMultipliers[y * width + x] =
                                    weights[0, 0] * mapMultipliers[(y - 1) * width + (x - 1)] + weights[0, 1] * mapMultipliers[(y - 1) * width + (x)] + weights[0, 2] * mapMultipliers[(y - 1) * width + (x + 1)] +
                                    weights[1, 0] * mapMultipliers[(y - 0) * width + (x - 1)] + weights[1, 1] * mapMultipliers[(y - 0) * width + (x)] + weights[1, 2] * mapMultipliers[(y - 0) * width + (x + 1)] +
                                    weights[2, 0] * mapMultipliers[(y + 1) * width + (x - 1)] + weights[2, 1] * mapMultipliers[(y + 1) * width + (x)] + weights[2, 2] * mapMultipliers[(y + 1) * width + (x + 1)];
                            }
                        }
                    }
                    mapMultipliers = newmapMultipliers;

                    newmapMultipliers = null;
                    weights = null;
                }

                //the height map gets smoothed as well
                {
                    int width = WoodsFile.mapWidthValue;
                    int height = WoodsFile.mapHeightValue;
                    byte[] heightMapBuffer = contentReader.WoodsFileReader.Buffer.Clone() as byte[];
                    int[,] intWeights = { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };
                    for (int y = 1; y < height - 1; y++)
                    {
                        for (int x = 1; x < width - 1; x++)
                        {
                            if (mapDistanceSquaredFromWater[y * width + x] > 0) // check if squared distance from water is greater than zero -> if it is no water pixel
                            {
                                int value =
                                    intWeights[0, 0] * (int)heightMapBuffer[(y - 1) * width + (x - 1)] + intWeights[0, 1] * (int)heightMapBuffer[(y - 1) * width + (x)] + intWeights[0, 2] * (int)heightMapBuffer[(y - 1) * width + (x + 1)] +
                                    intWeights[1, 0] * (int)heightMapBuffer[(y - 0) * width + (x - 1)] + intWeights[1, 1] * (int)heightMapBuffer[(y - 0) * width + (x)] + intWeights[1, 2] * (int)heightMapBuffer[(y - 0) * width + (x + 1)] +
                                    intWeights[2, 0] * (int)heightMapBuffer[(y + 1) * width + (x - 1)] + intWeights[2, 1] * (int)heightMapBuffer[(y + 1) * width + (x)] + intWeights[2, 2] * (int)heightMapBuffer[(y + 1) * width + (x + 1)];

                                heightMapBuffer[y * width + x] = (byte)(value / 16);
                            }
                        }
                    }
                    contentReader.WoodsFileReader.Buffer = heightMapBuffer;

                    heightMapBuffer = null;
                    intWeights = null;
                }

                // build tree coverage map
                if (mapTreeCoverage == null)
                {
                    int width = WoodsFile.mapWidthValue;
                    int height = WoodsFile.mapHeightValue;
                    mapTreeCoverage = new byte[width * height];

                    #if LOAD_TREE_COVERAGE_MAP
                    {
                        float startTreeCoverageAtElevation = ImprovedTerrainSampler.baseHeightScale * 2.0f; // ImprovedTerrainSampler.scaledBeachElevation;
                        float minTreeCoverageSaturated = ImprovedTerrainSampler.baseHeightScale * 6.0f;
                        float maxTreeCoverageSaturated = ImprovedTerrainSampler.baseHeightScale * 60.0f;
                        float endTreeCoverageAtElevation = ImprovedTerrainSampler.baseHeightScale * 80.0f;
                        //float maxElevation = 0.0f;
                        for (int y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                int readIndex = (height - 1 - y) * width + x;
                                float w = 0.0f;

                                //float elevation = ((float)contentReader.WoodsFileReader.Buffer[(height - 1 - y) * width + x]) / 255.0f; // *mapMultipliers[index];
                                float elevation = ((float)contentReader.WoodsFileReader.Buffer[readIndex]) * mapMultipliers[readIndex];

                                //maxElevation = Math.Max(maxElevation, elevation);
                                if ((elevation > minTreeCoverageSaturated) && (elevation < maxTreeCoverageSaturated))
                                {
                                    w = 1.0f;
                                }
                                else if ((elevation >= startTreeCoverageAtElevation) && (elevation <= minTreeCoverageSaturated))
                                {
                                    w = (elevation - startTreeCoverageAtElevation) / (minTreeCoverageSaturated - startTreeCoverageAtElevation);
                                }
                                else if ((elevation >= maxTreeCoverageSaturated) && (elevation <= endTreeCoverageAtElevation))
                                {
                                    w = 1.0f - ((elevation - maxTreeCoverageSaturated) / (endTreeCoverageAtElevation - maxTreeCoverageSaturated));
                                }

                                //w = 0.65f * w + 0.35f * Math.Min(6.0f, (float)Math.Sqrt(mapDistanceSquaredFromLocations[y * width + x])) / 6.0f;

                                mapTreeCoverage[(y) * width + x] = Convert.ToByte(w * 255.0f);

                                //if (elevation>0.05f)
                                //    mapTreeCoverage[index] = Convert.ToByte(250); //w * 255.0f);
                                //else mapTreeCoverage[index] = Convert.ToByte(0);

                                //if (elevation >= startTreeCoverageAtElevation)
                                //{
                                //    mapTreeCoverage[(y) * width + x] = Convert.ToByte(255.0f);
                                //} else{
                                //    mapTreeCoverage[(y) * width + x] = Convert.ToByte(0.0f);
                                //}
                            }
                        }
                    }
                    #else
                    {
                        FileStream istream;
                        istream = new FileStream(filepathInTreeCoverageMap, FileMode.Open, FileAccess.Read);
                        BinaryReader readerMapTreeCoverage = new BinaryReader(istream, Encoding.UTF8);
                        readerMapTreeCoverage.Read(mapTreeCoverage, 0, width * height);
                        readerMapTreeCoverage.Close();
                        istream.Close();
                    }
                    #endif

                    #if CREATE_PERSISTENT_TREE_COVERAGE_MAP
                    {
                        FileStream ostream = new FileStream(filepathOutTreeCoverageMap, FileMode.Create, FileAccess.Write);
                        BinaryWriter writerMapTreeCoverage = new BinaryWriter(ostream, Encoding.UTF8);
                        writerMapTreeCoverage.Write(mapTreeCoverage, 0, width * height);
                        writerMapTreeCoverage.Close();
                        ostream.Close();
                    }
                    #endif
                    //Debug.Log(string.Format("max elevation: {0}", maxElevation));
                }
                
                init = true;
            }
        }

        /* distance transform of image (will get binarized) using squared distance */
        public static float[] imageDistanceTransform(byte[] imgIn, int width, int height, byte maskValue)
        {
            const float INF = 1E20f;
            // allocate image and initialize
            float[] imgOut = new float[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (imgIn[y * width + x] == maskValue)
                        imgOut[y * width + x] = 0; // pixels with maskValue -> distance 0
                    else
                        imgOut[y * width + x] = INF; // set to infinite
                }
            }

            distanceTransform2D(ref imgOut, width, height);
            return imgOut;
        }

        /* euklidean distance transform (based on an implementation of Pedro Felzenszwalb (from paper Fast distance transform in C++ by Felzenszwalb and Huttenlocher))*/

        /* distance transform of 1d function using squared distance */
        private static float[] distanceTransform1D(ref float[] f, int n)
        {
            const float INF = 1E20f;

            float[] d = new float[n];
            int[] v = new int[n];
            float[] z = new float[n + 1];
            int k = 0;
            v[0] = 0;
            z[0] = -INF;
            z[1] = +INF;
            for (int q = 1; q <= n - 1; q++)
            {
                float s = ((f[q] + (q * q)) - (f[v[k]] + (v[k] * v[k]))) / (2 * q - 2 * v[k]);
                while (s <= z[k])
                {
                    k--;
                    s = ((f[q] + (q * q)) - (f[v[k]] + (v[k] * v[k]))) / (2 * q - 2 * v[k]);
                }
                k++;
                v[k] = q;
                z[k] = s;
                z[k + 1] = +INF;
            }

            k = 0;
            for (int q = 0; q <= n - 1; q++)
            {
                while (z[k + 1] < q)
                    k++;
                d[q] = (q - v[k]) * (q - v[k]) + f[v[k]];
            }

            return d;
        }
        /* in-place 2D distance transform on float array using squared distance, float array must be initialized with 0 for maskValue-pixels and infinite otherwise*/
        private static void distanceTransform2D(ref float[] img, int width, int height)
        {
            float[] f = new float[Math.Max(width, height)];

            // transform along columns
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    f[y] = img[y * width + x];
                }
                float[] d = distanceTransform1D(ref f, height);
                for (int y = 0; y < height; y++)
                {
                    img[y * width + x] = d[y];
                }
            }

            // transform along rows
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    f[x] = img[y * width + x];
                }
                float[] d = distanceTransform1D(ref f, width);
                for (int x = 0; x < width; x++)
                {
                    img[y * width + x] = d[x];
                }
            }
        }
    }
}