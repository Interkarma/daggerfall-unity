// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Michael Rauter (Nystul), Hazelnut
// 
// Notes:
//

using UnityEngine;
using DaggerfallConnect.Arena2;
using Unity.Jobs;
using Unity.Collections;

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

        public override void GenerateSamples(ref MapPixelData mapPixel)
        {
            // Should never get called since class has been updated to schedule work using jobs system.
            throw new System.NotImplementedException();
        }

        [Unity.Burst.BurstCompile]
        struct GenerateSamplesJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<byte> shm;
            [ReadOnly]
            public NativeArray<byte> lhm;

            public NativeArray<float> heightmapData;

            public byte sd;
            public byte ld;
            public int hDim;
            public float div;
            public int mapPixelX;
            public int mapPixelY;
            public float maxTerrainHeight;

            float baseHeight, noiseHeight;
            float x1, x2, x3, x4;

            public void Execute(int index)
            {
                // Use cols=x and rows=y for height data
                int x = JobA.Col(index, hDim);
                int y = JobA.Row(index, hDim);

                float rx = (float)x / div;
                float ry = (float)y / div;
                int ix = Mathf.FloorToInt(rx);
                int iy = Mathf.FloorToInt(ry);
                float sfracx = (float)x / (float)(hDim - 1);
                float sfracy = (float)y / (float)(hDim - 1);
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
                int noisex = mapPixelX * (hDim - 1) + x;
                int noisey = (MapsFile.MaxMapPixelY - mapPixelY) * (hDim - 1) + y;
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

        public override JobHandle ScheduleGenerateSamplesJob(ref MapPixelData mapPixel)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Divisor ensures continuous 0-1 range of height samples
            float div = (HeightmapDimension - 1) / 3f;

            // Read neighbouring height samples for this map pixel
            int mx = mapPixel.mapPixelX;
            int my = mapPixel.mapPixelY;
            byte sDim = 4;
            NativeArray<byte> shm = new NativeArray<byte>(dfUnity.ContentReader.WoodsFileReader.GetHeightMapValuesRange1Dim(mx - 2, my - 2, sDim), Allocator.TempJob);

            // Convert & flatten large height samples 2d array into 1d native array.
            byte[,] lhm2 = dfUnity.ContentReader.WoodsFileReader.GetLargeHeightMapValuesRange(mx - 1, my, 3);
            NativeArray<byte> lhm = new NativeArray<byte>(lhm2.Length, Allocator.TempJob);
            byte lDim = (byte)lhm2.GetLength(0);
            int i = 0;
            for (int y = 0; y < lDim; y++)
                for (int x = 0; x < lDim; x++)
                    lhm[i++] = lhm2[x, y];

            // Add both working native arrays to disposal list.
            mapPixel.nativeArrayList.Add(shm);
            mapPixel.nativeArrayList.Add(lhm);

            // Extract height samples for all chunks
            int hDim = HeightmapDimension;
            GenerateSamplesJob generateSamplesJob = new GenerateSamplesJob
            {
                shm = shm,
                lhm = lhm,
                heightmapData = mapPixel.heightmapData,
                sd = sDim,
                ld = lDim,
                hDim = hDim,
                div = div,
                mapPixelX = mapPixel.mapPixelX,
                mapPixelY = mapPixel.mapPixelY,
                maxTerrainHeight = MaxTerrainHeight,
            };

            JobHandle generateSamplesHandle = generateSamplesJob.Schedule(hDim * hDim, 64);     // Batch = 1 breaks it since shm not copied... test again later
            return generateSamplesHandle;
        }
    }
}