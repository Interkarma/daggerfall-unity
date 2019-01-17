// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Michael Rauter (Nystul)
// 
// Notes:
//

using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using Unity.Jobs;
using Unity.Collections;
using System.Linq;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Default TerrainSampler for StreamingWorld.
    /// </summary>
    public class DefaultTerrainSampler : TerrainSampler
    {
        // Scale factors for this sampler implementation
        const float baseHeightScale = 8f;
        const float noiseMapScale = 4f;
        const float extraNoiseScale = 10f;
        const float scaledOceanElevation = 3.4f * baseHeightScale;
        const float scaledBeachElevation = 5.0f * baseHeightScale;

        // Max terrain height of this sampler implementation
        const float maxTerrainHeight = 1539f;

        public override int Version
        {
            get { return 1; }
        }

        public DefaultTerrainSampler()
        {
            HeightmapDimension = defaultHeightmapDimension;
            MaxTerrainHeight = maxTerrainHeight;
            MeanTerrainHeightScale = baseHeightScale + noiseMapScale;
            OceanElevation = scaledOceanElevation;
            BeachElevation = scaledBeachElevation;
        }

        /// <summary>
        /// helper class to do parallel computing of heights
        /// </summary>
        public class HeightsComputationTask
        {
            public class DataForTask
            {
                public int numTasks;
                public int currentTask;
                public int HeightmapDimension;
                public float MaxTerrainHeight;
                public float div;
                public byte[,] shm;
                public byte[,] lhm;
                public MapPixelData mapPixel;
            }

            private ManualResetEvent _doneEvent;

            public HeightsComputationTask(ManualResetEvent doneEvent)
            {
                _doneEvent = doneEvent;
            }

            public void ProcessTaskThread(System.Object stateInfo)
            {
                DataForTask dataForTask = stateInfo as DataForTask;

                // Extract height samples for all chunks
                float baseHeight, noiseHeight;
                float x1, x2, x3, x4;

                int dim = dataForTask.HeightmapDimension;
                float div = dataForTask.div;
                byte[,] shm = dataForTask.shm;
                byte[,] lhm = dataForTask.lhm;

                // split the work between different tasks running in different threads (thread n computes data elements n, n + numTasks, n + numTasks*2, ...)
                for (int y = dataForTask.currentTask; y < dim; y += dataForTask.numTasks)
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

                        // Bicubic sample small height map for base terrain elevation
                        x1 = TerrainHelper.CubicInterpolator(shm[0, 3], shm[1, 3], shm[2, 3], shm[3, 3], sfracx);
                        x2 = TerrainHelper.CubicInterpolator(shm[0, 2], shm[1, 2], shm[2, 2], shm[3, 2], sfracx);
                        x3 = TerrainHelper.CubicInterpolator(shm[0, 1], shm[1, 1], shm[2, 1], shm[3, 1], sfracx);
                        x4 = TerrainHelper.CubicInterpolator(shm[0, 0], shm[1, 0], shm[2, 0], shm[3, 0], sfracx);
                        baseHeight = TerrainHelper.CubicInterpolator(x1, x2, x3, x4, sfracy);
                        scaledHeight += baseHeight * baseHeightScale;

                        // Bicubic sample large height map for noise mask over terrain features
                        x1 = TerrainHelper.CubicInterpolator(lhm[ix, iy + 0], lhm[ix + 1, iy + 0], lhm[ix + 2, iy + 0], lhm[ix + 3, iy + 0], fracx);
                        x2 = TerrainHelper.CubicInterpolator(lhm[ix, iy + 1], lhm[ix + 1, iy + 1], lhm[ix + 2, iy + 1], lhm[ix + 3, iy + 1], fracx);
                        x3 = TerrainHelper.CubicInterpolator(lhm[ix, iy + 2], lhm[ix + 1, iy + 2], lhm[ix + 2, iy + 2], lhm[ix + 3, iy + 2], fracx);
                        x4 = TerrainHelper.CubicInterpolator(lhm[ix, iy + 3], lhm[ix + 1, iy + 3], lhm[ix + 2, iy + 3], lhm[ix + 3, iy + 3], fracx);
                        noiseHeight = TerrainHelper.CubicInterpolator(x1, x2, x3, x4, fracy);
                        scaledHeight += noiseHeight * noiseMapScale;

                        // Additional noise mask for small terrain features at ground level
                        int noisex = dataForTask.mapPixel.mapPixelX * (dataForTask.HeightmapDimension - 1) + x;
                        int noisey = (MapsFile.MaxMapPixelY - dataForTask.mapPixel.mapPixelY) * (dataForTask.HeightmapDimension - 1) + y;
                        float lowFreq = TerrainHelper.GetNoise(noisex, noisey, 0.3f, 0.5f, 0.5f, 1);
                        float highFreq = TerrainHelper.GetNoise(noisex, noisey, 0.9f, 0.5f, 0.5f, 1);
                        scaledHeight += (lowFreq * highFreq) * extraNoiseScale;

                        // Clamp lower values to ocean elevation
                        if (scaledHeight < scaledOceanElevation)
                            scaledHeight = scaledOceanElevation;

                        // Set sample
                        float height = Mathf.Clamp01(scaledHeight / dataForTask.MaxTerrainHeight);
                        dataForTask.mapPixel.heightmapSamples[y, x] = height;
                    }
                }

                _doneEvent.Set();
            }
        }

        public override void GenerateSamples(ref MapPixelData mapPixel)
        {
            //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            //long startTime = stopwatch.ElapsedMilliseconds;

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Create samples arrays
            mapPixel.tilemapSamples = new TilemapSample[MapsFile.WorldMapTileDim, MapsFile.WorldMapTileDim];
            mapPixel.heightmapSamples = new float[HeightmapDimension, HeightmapDimension];

            // Divisor ensures continuous 0-1 range of height samples
            float div = (float)(HeightmapDimension - 1) / 3f;

            // Read neighbouring height samples for this map pixel
            int mx = mapPixel.mapPixelX;
            int my = mapPixel.mapPixelY;
            byte[,] shm = dfUnity.ContentReader.WoodsFileReader.GetHeightMapValuesRange(mx - 2, my - 2, 4);
            byte[,] lhm = dfUnity.ContentReader.WoodsFileReader.GetLargeHeightMapValuesRange(mx - 1, my, 3);

            // the number of parallel tasks (note Nystul: use logical processor count for now - seems to be a good value)
            int numParallelTasks = Environment.ProcessorCount;

            // events used to synchronize thread computations (wait for them to finish)
            var doneEvents = new ManualResetEvent[numParallelTasks];

            // the array of instances of the height computations helper class
            var heightsComputationTaskArray = new HeightsComputationTask[numParallelTasks];

            // array of the data needed by the different tasks
            var dataForTasks = new HeightsComputationTask.DataForTask[numParallelTasks];

            for (int i = 0; i < numParallelTasks; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                var heightsComputationTask = new HeightsComputationTask(doneEvents[i]);
                heightsComputationTaskArray[i] = heightsComputationTask;
                dataForTasks[i] = new HeightsComputationTask.DataForTask();
                dataForTasks[i].numTasks = numParallelTasks;
                dataForTasks[i].currentTask = i;
                dataForTasks[i].HeightmapDimension = HeightmapDimension;
                dataForTasks[i].MaxTerrainHeight = MaxTerrainHeight;
                dataForTasks[i].div = div;
                dataForTasks[i].shm = shm;
                dataForTasks[i].lhm = lhm;
                dataForTasks[i].mapPixel = mapPixel;
                ThreadPool.QueueUserWorkItem(heightsComputationTask.ProcessTaskThread, dataForTasks[i]);
            }

            // wait for all tasks to finish computation
            WaitHandle.WaitAll(doneEvents);

            // computed average and max height in a second pass (after threaded tasks computed all heights)
            float averageHeight = 0;
            float maxHeight = float.MinValue;

            int dim = HeightmapDimension;
            for (int y = 0; y < dim; y++)
            {
                for (int x = 0; x < dim; x++)
                {
                    // get sample
                    float height = mapPixel.heightmapSamples[y, x];

                    // Accumulate average height
                    averageHeight += height;

                    // Get max height
                    if (height > maxHeight)
                        maxHeight = height;
                }
            }

            // Average and max heights are passed back for locations
            mapPixel.averageHeight = (averageHeight /= (float)(dim * dim));
            mapPixel.maxHeight = maxHeight;

            //long totalTime = stopwatch.ElapsedMilliseconds - startTime;
            //DaggerfallUnity.LogMessage(string.Format("GenerateSamples took: {0}ms", totalTime), true);
        }

        struct GenerateSamplesJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<byte> shm;
            [ReadOnly]
            public NativeArray<byte> lhm;

            public NativeArray<float> heightmapData;

            public byte sd;
            public byte ld;
            public int dim;
            public float div;
            public int mapPixelX;
            public int mapPixelY;
            public float maxTerrainHeight;

            float baseHeight, noiseHeight;
            float x1, x2, x3, x4;

            public void Execute(int index)
            {
                // Use cols=x and rows=y for height data
                int x = JobA.Col(index, dim);
                int y = JobA.Row(index, dim);

                float rx = (float)x / div;
                float ry = (float)y / div;
                int ix = Mathf.FloorToInt(rx);
                int iy = Mathf.FloorToInt(ry);
                float sfracx = (float)x / (float)(dim - 1);
                float sfracy = (float)y / (float)(dim - 1);
                float fracx = (float)(x - ix * div) / div;
                float fracy = (float)(y - iy * div) / div;
                float scaledHeight = 0;

                // Bicubic sample small height map for base terrain elevation
                x1 = TerrainHelper.CubicInterpolator(shm[JobA.Idx(0, 3, sd)], shm[JobA.Idx(1, 3, sd)], shm[JobA.Idx(2, 3, sd)], shm[JobA.Idx(3, 3, sd)], sfracx);
                x2 = TerrainHelper.CubicInterpolator(shm[JobA.Idx(0, 2, sd)], shm[JobA.Idx(1, 2, sd)], shm[JobA.Idx(2, 2, sd)], shm[JobA.Idx(3, 2, sd)], sfracx);
                x3 = TerrainHelper.CubicInterpolator(shm[JobA.Idx(0, 1, sd)], shm[JobA.Idx(1, 1, sd)], shm[JobA.Idx(2, 1, sd)], shm[JobA.Idx(3, 1, sd)], sfracx);
                x4 = TerrainHelper.CubicInterpolator(shm[JobA.Idx(0, 0, sd)], shm[JobA.Idx(1, 0, sd)], shm[JobA.Idx(2, 0, sd)], shm[JobA.Idx(3, 0, sd)], sfracx);
                baseHeight = TerrainHelper.CubicInterpolator(x1, x2, x3, x4, sfracy);
                scaledHeight += baseHeight * baseHeightScale;

                // Bicubic sample large height map for noise mask over terrain features
                x1 = TerrainHelper.CubicInterpolator(lhm[JobA.Idx(ix, iy + 0, ld)], lhm[JobA.Idx(ix + 1, iy + 0, ld)], lhm[JobA.Idx(ix + 2, iy + 0, ld)], lhm[JobA.Idx(ix + 3, iy + 0, ld)], fracx);
                x2 = TerrainHelper.CubicInterpolator(lhm[JobA.Idx(ix, iy + 1, ld)], lhm[JobA.Idx(ix + 1, iy + 1, ld)], lhm[JobA.Idx(ix + 2, iy + 1, ld)], lhm[JobA.Idx(ix + 3, iy + 1, ld)], fracx);
                x3 = TerrainHelper.CubicInterpolator(lhm[JobA.Idx(ix, iy + 2, ld)], lhm[JobA.Idx(ix + 1, iy + 2, ld)], lhm[JobA.Idx(ix + 2, iy + 2, ld)], lhm[JobA.Idx(ix + 3, iy + 2, ld)], fracx);
                x4 = TerrainHelper.CubicInterpolator(lhm[JobA.Idx(ix, iy + 3, ld)], lhm[JobA.Idx(ix + 1, iy + 3, ld)], lhm[JobA.Idx(ix + 2, iy + 3, ld)], lhm[JobA.Idx(ix + 3, iy + 3, ld)], fracx);
                noiseHeight = TerrainHelper.CubicInterpolator(x1, x2, x3, x4, fracy);
                scaledHeight += noiseHeight * noiseMapScale;

                // Additional noise mask for small terrain features at ground level
                int noisex = mapPixelX * (dim - 1) + x;
                int noisey = (MapsFile.MaxMapPixelY - mapPixelY) * (dim - 1) + y;
                float lowFreq = TerrainHelper.GetNoise(noisex, noisey, 0.3f, 0.5f, 0.5f, 1);
                float highFreq = TerrainHelper.GetNoise(noisex, noisey, 0.9f, 0.5f, 0.5f, 1);
                scaledHeight += (lowFreq * highFreq) * extraNoiseScale;

                // Clamp lower values to ocean elevation
                if (scaledHeight < scaledOceanElevation)
                    scaledHeight = scaledOceanElevation;

                // Set sample
                float height = Mathf.Clamp01(scaledHeight / maxTerrainHeight);
                heightmapData[index] = height;
            }
        }

        public override void GenerateSamplesJobs(ref MapPixelData mapPixel)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Divisor ensures continuous 0-1 range of height samples
            float div = (HeightmapDimension - 1) / 3f;

            // Read neighbouring height samples for this map pixel
            int mx = mapPixel.mapPixelX;
            int my = mapPixel.mapPixelY;
            byte sd = 4;
            NativeArray<byte> shm = new NativeArray<byte>(dfUnity.ContentReader.WoodsFileReader.GetHeightMapValuesRangeJobs(mx - 2, my - 2, sd), Allocator.TempJob);
            // TODO - shortcut conversion & flattening.
            byte[,] lhm2 = dfUnity.ContentReader.WoodsFileReader.GetLargeHeightMapValuesRange(mx - 1, my, 3);
            NativeArray<byte> lhm = new NativeArray<byte>(lhm2.Length, Allocator.TempJob);
            byte ld = (byte)lhm2.GetLength(0);
            int i = 0;
            for (int y = 0; y < ld; y++)
                for (int x = 0; x < ld; x++)
                    lhm[i++] = lhm2[x, y];

            // Extract height samples for all chunks
            int dim = HeightmapDimension;
            GenerateSamplesJob generateSamplesJob = new GenerateSamplesJob
            {
                shm = shm,
                lhm = lhm,
                heightmapData = mapPixel.heightmapData,
                sd = sd,
                ld = ld,
                dim = dim,
                div = div,
                mapPixelX = mapPixel.mapPixelX,
                mapPixelY = mapPixel.mapPixelY,
                maxTerrainHeight = MaxTerrainHeight,
            };

            JobHandle generateSamplesHandle = generateSamplesJob.Schedule(dim*dim, 16);     // Batch = 1 breaks it since shm not copied... test again later
            generateSamplesHandle.Complete();

            shm.Dispose();
            lhm.Dispose();

        }

    }
}