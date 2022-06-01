// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Michael Rauter (Nystul)
// 
// Notes:
//

using System;
using Unity.Jobs;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Interface to TerrainSampler.
    /// </summary>
    public interface ITerrainSampler
    {
        /// <summary>
        /// Version of terrain sampler implementation.
        /// This is serialized with save games to ensure player is being loaded
        /// back into the same world on deserialization.
        /// If you make a change to how heightmaps are generated, then you MUST tick up the version number.
        /// Failed to do this can make player fall through world on load.
        /// </summary>
        int Version { get; }

        // Terrain heightmap dimension (+1 extra point for end vertex)
        // Example settings are 129, 257, 513, 1023, etc.
        // Do not set to a value less than MapsFile.WorldMapTileDim
        int HeightmapDimension { get; set; }

        /// <summary>
        /// Maximum height of terrain. Used for clamping max height and setting Unity's TerrainData.size Y axis.
        /// </summary>
        float MaxTerrainHeight { get; set; }

        /// <summary>
        /// Mean scale factor of terrain height. This should return the mean value of the scale factor for terrain heights
        /// (e.g. multiplication factor of low resolution height map values + multiplication factor of detailed height map values).
        /// This value can/will be used by terrain-related mods.
        /// </summary>
        float MeanTerrainHeightScale { get; set; }        

        /// <summary>
        /// Sea level. Use for clamping min height and texturing with ocean.
        /// </summary>
        float OceanElevation { get; set; }

        /// <summary>
        /// Beach line elevation. How far above sea level the beach line extends.
        /// </summary>
        float BeachElevation { get; set; }

        /// <summary>
        /// Get terrain height scale for given x and y position on the world map
        /// </summary>
        /// <param name="x">world map x position</param>
        /// <param name="y">world map y position</param>
        /// <returns></returns>
        float TerrainHeightScale(int x, int y);

        /// <summary>
        /// Indicates whether this sampler blends / flattens terrain for locations and thus controls whether the 
        /// standard job to blend terrain for locations should be run or not.
        /// </summary>
        /// <returns>true if this sampler blends terrain for locations, false if standard location blending should be performed</returns>
        bool IsLocationTerrainBlended();

        /// <summary>
        /// Populates a MapPixelData struct using custom height sample generator.
        /// </summary>
        /// <param name="mapPixel">MapPixelData struct.</param>
        void GenerateSamples(ref MapPixelData mapPixel);

        /// <summary>
        /// Populates a MapPixelData struct using custom height sample generator implemented using Unity Jobs system.
        /// The Dispose() method should be called after the jobs are completed to free any allocated memory.
        /// NOTE: Backwards compatible with implementations that only implement GenerateSamples, and will call that then
        /// convert the output before returning. (reduces performance a little)
        /// </summary>
        /// <param name="mapPixel">MapPixelData struct.</param>
        /// <returns>JobHandle of the scheduled job.</returns>
        JobHandle ScheduleGenerateSamplesJob(ref MapPixelData mapPixel);
    }

    /// <summary>
    /// Base TerrainSampler for transforming heightmap samples.
    /// This class and interface will be expanded on later.
    /// </summary>
    public abstract class TerrainSampler : ITerrainSampler
    {
        protected int defaultHeightmapDimension = 129;

        public abstract int Version { get; }
        public virtual int HeightmapDimension { get; set; }        
        public virtual float MaxTerrainHeight { get; set; }
        public virtual float MeanTerrainHeightScale { get; set; }
        public virtual float OceanElevation { get; set; }
        public virtual float BeachElevation { get; set; }
        
        //this function may be overriden if terrain sampler implementation creates different height scales for different map pixels
        public virtual float TerrainHeightScale(int x, int y) { return MeanTerrainHeightScale; } // default implementation returns MeanTerrainHeightScale for every world map position

        public virtual bool IsLocationTerrainBlended() { return false; }

        public abstract void GenerateSamples(ref MapPixelData mapPixel);

        // Makes terrain sampler implementations backwards compatible with jobs system terrain data generation.
        public virtual JobHandle ScheduleGenerateSamplesJob(ref MapPixelData mapPixel)
        {
            GenerateSamples(ref mapPixel);

            // Convert generated samples to the flattened native array used by jobs.
            int hDim = HeightmapDimension;
            for (int y = 0; y < hDim; y++)
            {
                for (int x = 0; x < hDim; x++)
                {
                    mapPixel.heightmapData[JobA.Idx(y, x, hDim)] = mapPixel.heightmapSamples[y, x];
                }
            }
            return new JobHandle();
        }
    }
}