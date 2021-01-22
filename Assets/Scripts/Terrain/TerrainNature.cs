// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Hazelnut
// 
// Notes:
//

using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Terrain nature layout interface.
    /// </summary>
    public interface ITerrainNature
    {
        bool NatureMeshUsed { get; }

        void LayoutNature(DaggerfallTerrain dfTerrain, DaggerfallBillboardBatch dfBillboardBatch, float terrainScale, int terrainDist);
    }

    /// <summary>
    /// Drops nature flats based on random chance scaled by simple rules.
    /// </summary>
    public class DefaultTerrainNature : ITerrainNature
    {
        protected const float maxSteepness = 50f;             // 50
        protected const float slopeSinkRatio = 70f;           // Sink flats slightly into ground as slope increases to prevent floaty trees.
        protected const float baseChanceOnDirt = 0.2f;        // 0.2
        protected const float baseChanceOnGrass = 0.9f;       // 0.4
        protected const float baseChanceOnStone = 0.05f;      // 0.05
        protected const int natureClearance = 4;

        public bool NatureMeshUsed { get; protected set; }

        public virtual void LayoutNature(DaggerfallTerrain dfTerrain, DaggerfallBillboardBatch dfBillboardBatch, float terrainScale, int terrainDist)
        {
            // Location Rect is expanded slightly to give extra clearance around locations
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
            Random.InitState(TerrainHelper.MakeTerrainKey(dfTerrain.MapPixelX, dfTerrain.MapPixelY));

            // Just layout some random flats spread evenly across entire map pixel area
            // Flats are aligned with tiles, max 16129 billboards per batch
            Vector2 tilePos = Vector2.zero;
            int tDim = MapsFile.WorldMapTileDim;
            int hDim = DaggerfallUnity.Instance.TerrainSampler.HeightmapDimension;
            float scale = terrainData.heightmapScale.x * (float)hDim / (float)tDim;
            float maxTerrainHeight = DaggerfallUnity.Instance.TerrainSampler.MaxTerrainHeight;
            float beachLine = DaggerfallUnity.Instance.TerrainSampler.BeachElevation;

            for (int y = 0; y < tDim; y++)
            {
                for (int x = 0; x < tDim; x++)
                {
                    // Reject based on steepness
                    float steepness = terrainData.GetSteepness((float)x / tDim, (float)y / tDim);
                    if (steepness > maxSteepness)
                        continue;

                    // Reject if inside location rect (expanded slightly to give extra clearance around locations)
                    tilePos.x = x;
                    tilePos.y = y;
                    if (rect.x > 0 && rect.y > 0 && rect.Contains(tilePos))
                        continue;

                    // Chance also determined by tile type
                    int tile = dfTerrain.MapData.tilemapSamples[x, y] & 0x3F;
                    if (tile == 1)
                    {   // Dirt
                        if (Random.Range(0f, 1f) > chanceOnDirt)
                            continue;
                    }
                    else if (tile == 2)
                    {   // Grass
                        if (Random.Range(0f, 1f) > chanceOnGrass)
                            continue;
                    }
                    else if (tile == 3)
                    {   // Stone
                        if (Random.Range(0f, 1f) > chanceOnStone)
                            continue;
                    }
                    else
                    {   // Anything else
                        continue;
                    }

                    int hx = (int)Mathf.Clamp(hDim * ((float)x / (float)tDim), 0, hDim - 1);
                    int hy = (int)Mathf.Clamp(hDim * ((float)y / (float)tDim), 0, hDim - 1);
                    float height = dfTerrain.MapData.heightmapSamples[hy, hx] * maxTerrainHeight;  // x & y swapped in heightmap for TerrainData.SetHeights()

                    // Reject if too close to water
                    if (height < beachLine)
                        continue;

                    // Sample height and position billboard
                    Vector3 pos = new Vector3(x * scale, 0, y * scale);
                    float height2 = terrain.SampleHeight(pos + terrain.transform.position);
                    pos.y = height2 - (steepness / slopeSinkRatio);

                    // Add to batch unless a mesh replacement is found
                    int record = Random.Range(1, 32);
                    if (terrainDist > 1 || !MeshReplacement.ImportNatureGameObject(dfBillboardBatch.TextureArchive, record, terrain, x, y))
                        dfBillboardBatch.AddItem(record, pos);
                    else if (!NatureMeshUsed)
                        NatureMeshUsed = true;  // Signal that nature mesh has been used to initiate extra terrain updates
                }
            }

            // Apply new batch
            dfBillboardBatch.Apply();
        }
    }

}