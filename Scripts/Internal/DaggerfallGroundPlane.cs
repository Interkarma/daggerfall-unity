// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class DaggerfallGroundPlane : MonoBehaviour
    {
        public static float GroundOffset = -1f;
        public static float TileSize = 256f;

        public Color32[] tileMap;
        public int tileMapDim = 16;

        [SerializeField]
        private GroundSummary summary = new GroundSummary();
        public GroundSummary Summary
        {
            get { return summary; }
        }

        [Serializable]
        public struct GroundSummary
        {
            public ClimateBases climate;
            public ClimateSeason season;
            public int archive;
        }

        /// <summary>
        /// Set ground climate by texture archive index.
        /// </summary>
        /// <param name="dfUnity">DaggerfallUnity singleton. Required for content readers and settings.</param>
        /// <param name="archive">Texture archive index.</param>
        /// <param name="season">Season to set.</param>
        public void SetClimate(DaggerfallUnity dfUnity, int archive, ClimateSeason season)
        {
            // Create tileMap texture
            Texture2D tileMapTexture = new Texture2D(tileMapDim, tileMapDim, TextureFormat.RGB24, false);
            tileMapTexture.SetPixels32(tileMap);
            tileMapTexture.Apply(false, true);
            tileMapTexture.filterMode = FilterMode.Point;
            tileMapTexture.wrapMode = TextureWrapMode.Clamp;

            // Get tileMap material
            Material material = Instantiate(dfUnity.MaterialReader.GetTerrainTilesetMaterial(archive)) as Material;
            material.SetTexture("_TilemapTex", tileMapTexture);
            material.SetInt("_TilemapDim", tileMapDim);

            // Assign new season
            summary.archive = archive;
            summary.season = season;
            renderer.material = material;
        }

        /// <summary>
        /// Set ground climate.
        /// </summary>
        /// <param name="dfUnity">DaggerfallUnity singleton. Required for content readers and settings.</param>
        /// <param name="climate">Climate to set.</param>
        /// <param name="season">Season to set.</param>
        public void SetClimate(DaggerfallUnity dfUnity, ClimateBases climate, ClimateSeason season)
        {
            int archive = ClimateSwaps.GetGroundArchive(climate, season);
            SetClimate(dfUnity, archive, season);
            summary.climate = climate;
        }
    }
}