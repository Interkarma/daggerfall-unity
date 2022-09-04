// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Michael Rauter (Nystul), Hazelnut, Andrzej Łukasik (andrew.r.lukasik)
// 
// Notes:
//

using DaggerfallConnect.Arena2;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Profiling;

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

        public override int Version => 1;

        #region Profiler Markers

        static readonly ProfilerMarker
            ___m_ScheduleGenerateSamplesJob = new ProfilerMarker(nameof(ScheduleGenerateSamplesJob)),
            ___createLhm = new ProfilerMarker("create lhm array"),
            ___schedule = new ProfilerMarker("schedule");

        #endregion

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
        struct GenerateSamplesJob : IJobParallelForBatch
        {
            [ReadOnly] public NativeArray<byte> Shm;
            [ReadOnly] public NativeArray<byte> Lhm;
            [WriteOnly] public NativeArray<float> HeightmapData;
            public int
                Sdim,
                Ldim,
                NumRows,
                MapPixelX,
                MapPixelY;
            public float
                Div,
                MaxTerrainHeight;
            void IJobParallelForBatch.Execute(int startIndex, int count)
            {
                byte
                    shmx4v0 = Shm[Matrix.Idx(0, 0, Sdim)],
                    shmx4v1 = Shm[Matrix.Idx(1, 0, Sdim)],
                    shmx4v2 = Shm[Matrix.Idx(2, 0, Sdim)],
                    shmx4v3 = Shm[Matrix.Idx(3, 0, Sdim)],

                    shmx3v0 = Shm[Matrix.Idx(0, 1, Sdim)],
                    shmx3v1 = Shm[Matrix.Idx(1, 1, Sdim)],
                    shmx3v2 = Shm[Matrix.Idx(2, 1, Sdim)],
                    shmx3v3 = Shm[Matrix.Idx(3, 1, Sdim)],

                    shmx2v0 = Shm[Matrix.Idx(0, 2, Sdim)],
                    shmx2v1 = Shm[Matrix.Idx(1, 2, Sdim)],
                    shmx2v2 = Shm[Matrix.Idx(2, 2, Sdim)],
                    shmx2v3 = Shm[Matrix.Idx(3, 2, Sdim)],

                    shmx1v0 = Shm[Matrix.Idx(0, 3, Sdim)],
                    shmx1v1 = Shm[Matrix.Idx(1, 3, Sdim)],
                    shmx1v2 = Shm[Matrix.Idx(2, 3, Sdim)],
                    shmx1v3 = Shm[Matrix.Idx(3, 3, Sdim)];

                int max = startIndex + count;
                for (int index = startIndex; index < max; index++)
                {
                    // Use cols=x and rows=y for height data
                    int x = Matrix.Col(index, NumRows);
                    int y = Matrix.Row(index, NumRows);
                    float2 xy = new float2(x, y);

                    float2 r = xy / Div;
                    int2 i = (int2)math.floor(r);
                    float2 sfrac = xy / (float)(NumRows - 1);
                    float2 frac = (xy - (float2)i * Div) / Div;
                    float scaledHeight = 0;

                    // Bicubic sample small height map for base terrain elevation
                    {
                        float
                            x1 = TerrainHelper.CubicInterpolator(shmx1v0, shmx1v1, shmx1v2, shmx1v3, sfrac.x),
                            x2 = TerrainHelper.CubicInterpolator(shmx2v0, shmx2v1, shmx2v2, shmx2v3, sfrac.x),
                            x3 = TerrainHelper.CubicInterpolator(shmx3v0, shmx3v1, shmx3v2, shmx3v3, sfrac.x),
                            x4 = TerrainHelper.CubicInterpolator(shmx4v0, shmx4v1, shmx4v2, shmx4v3, sfrac.x);

                        scaledHeight += TerrainHelper.CubicInterpolator(x1, x2, x3, x4, sfrac.y) * baseHeightScale;
                    }

                    // Bicubic sample large height map for noise mask over terrain features
                    {
                        int2
                            i1 = i + 1,
                            i2 = i + 2,
                            i3 = i + 3;

                        byte
                            x1v0 = Lhm[Matrix.Idx(i.y, i.x, Ldim)],
                            x1v1 = Lhm[Matrix.Idx(i.y, i1.x, Ldim)],
                            x1v2 = Lhm[Matrix.Idx(i.y, i2.x, Ldim)],
                            x1v3 = Lhm[Matrix.Idx(i.y, i3.x, Ldim)],

                            x2v0 = Lhm[Matrix.Idx(i1.y, i.x, Ldim)],
                            x2v1 = Lhm[Matrix.Idx(i1.y, i1.x, Ldim)],
                            x2v2 = Lhm[Matrix.Idx(i1.y, i2.x, Ldim)],
                            x2v3 = Lhm[Matrix.Idx(i1.y, i3.x, Ldim)],

                            x3v0 = Lhm[Matrix.Idx(i2.y, i.x, Ldim)],
                            x3v1 = Lhm[Matrix.Idx(i2.y, i1.x, Ldim)],
                            x3v2 = Lhm[Matrix.Idx(i2.y, i2.x, Ldim)],
                            x3v3 = Lhm[Matrix.Idx(i2.y, i3.x, Ldim)],

                            x4v0 = Lhm[Matrix.Idx(i3.y, i.x, Ldim)],
                            x4v1 = Lhm[Matrix.Idx(i3.y, i1.x, Ldim)],
                            x4v2 = Lhm[Matrix.Idx(i3.y, i2.x, Ldim)],
                            x4v3 = Lhm[Matrix.Idx(i3.y, i3.x, Ldim)];

                        float
                            x1 = TerrainHelper.CubicInterpolator(x1v0, x1v1, x1v2, x1v3, frac.x),
                            x2 = TerrainHelper.CubicInterpolator(x2v0, x2v1, x2v2, x2v3, frac.x),
                            x3 = TerrainHelper.CubicInterpolator(x3v0, x3v1, x3v2, x3v3, frac.x),
                            x4 = TerrainHelper.CubicInterpolator(x4v0, x4v1, x4v2, x4v3, frac.x);

                        scaledHeight += TerrainHelper.CubicInterpolator(x1, x2, x3, x4, frac.y) * noiseMapScale;
                    }

                    // Additional noise mask for small terrain features at ground level
                    int noisex = MapPixelX * (NumRows - 1) + x;
                    int noisey = (MapsFile.MaxMapPixelY - MapPixelY) * (NumRows - 1) + y;
                    float lowFreq = TerrainHelper.GetNoise(noisex, noisey, 0.3f, 0.5f, 0.5f, 1);
                    float highFreq = TerrainHelper.GetNoise(noisex, noisey, 0.9f, 0.5f, 0.5f, 1);
                    scaledHeight += (lowFreq * highFreq) * extraNoiseScale;

                    // Clamp lower values to ocean elevation
                    if (scaledHeight < scaledOceanElevation)
                        scaledHeight = scaledOceanElevation;

                    // Set sample
                    float height = math.saturate(scaledHeight / MaxTerrainHeight);
                    HeightmapData[index] = height;
                }
            }
        }

        public override JobHandle ScheduleGenerateSamplesJob(ref MapPixelData mapPixel)
        {
            ___m_ScheduleGenerateSamplesJob.Begin();

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Divisor ensures continuous 0-1 range of height samples
            float div = (HeightmapDimension - 1) / 3f;

            // Read neighbouring height samples for this map pixel
            int mx = mapPixel.mapPixelX;
            int my = mapPixel.mapPixelY;
            int sDim = 4;

            // Convert & flatten large height samples 2d array into 1d native array.
            ___createLhm.Begin();
            var woodsFileReader = dfUnity.ContentReader.WoodsFileReader;
            byte[,] lhm2 = woodsFileReader.GetLargeHeightMapValuesRange(mx - 1, my, 3);
            var lhm = lhm2.AsNativeArray(out ulong gcHandleLhm);
            int lDim = lhm2.GetLength(0);
            ___createLhm.End();

            // Extract height samples for all chunks
            ___schedule.Begin();
            int hDim = HeightmapDimension;
            var generateSamplesJob = new GenerateSamplesJob
            {
                Shm = woodsFileReader.GetHeightMapValuesRange1Dim(mx - 2, my - 2, sDim).AsNativeArray(out ulong gcHandleShm),
                Lhm = lhm,
                HeightmapData = mapPixel.heightmapData,
                Sdim = sDim,
                Ldim = lDim,
                NumRows = hDim,
                Div = div,
                MapPixelX = mapPixel.mapPixelX,
                MapPixelY = mapPixel.mapPixelY,
                MaxTerrainHeight = MaxTerrainHeight,
            };
            JobHandle generateSamplesHandle = generateSamplesJob.ScheduleBatch(hDim * hDim, JobUtility.OptimalLoopBatchCount(hDim * hDim));
            JobUtility.ReleaseGCObject(gcHandleShm, generateSamplesHandle);
            JobUtility.ReleaseGCObject(gcHandleLhm, generateSamplesHandle);
            ___schedule.End();

            ___m_ScheduleGenerateSamplesJob.End();
            return generateSamplesHandle;
        }
    }
}