// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net), Nystul, TheLacus
// Contributors:    
// 
// Notes:
//

using System;
using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Data used for promotion of Daggerfall terrain material.
    /// </summary>
    public readonly struct TerrainMaterialData
    {
        /// <summary>
        /// Material assigned to the terrain.
        /// </summary>
        public Material Material { get; }

        /// <summary>
        /// Unity terrain data.
        /// </summary>
        public TerrainData TerrainData { get; }

        /// <summary>
        /// Texture generated from <see cref="DaggerfallTerrain.TileMap"/>, resulting from <see cref="ITerrainTexturing"/> process.
        /// </summary>
        public Texture2D TileMapTexture { get; }

        /// <summary>
        /// Climate used by terrain, corresponding to <see cref="MapPixelData.worldClimate"/>.
        /// </summary>
        public int WorldClimate { get; }

        internal TerrainMaterialData(Material material, TerrainData terrainData, Texture2D tileMapTexture, int worldClimate)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material));

            if (terrainData == null)
                throw new ArgumentNullException(nameof(terrainData));

            if (tileMapTexture == null)
                throw new ArgumentNullException(nameof(tileMapTexture));

            Material = material;
            TerrainData = terrainData;
            TileMapTexture = tileMapTexture;
            WorldClimate = worldClimate;
        }
    }

    /// <summary>
    /// Provides materials for Daggerfall terrains.
    /// </summary>
    public interface ITerrainMaterialProvider
    {
        /// <summary>
        /// Creates an empty instance of material using target shader.
        /// </summary>
        /// <returns>A new instance of a terrain material.</returns>
        Material CreateMaterial();

        /// <summary>
        /// Applies material properties for climate and season. This is called after tilemap has been generated.
        /// </summary>
        /// <param name="daggerfallTerrain">Daggerfall terrain that owns the material.</param>
        /// <param name="terrainMaterialData">Properties for Daggerfall terrain.</param>
        void PromoteMaterial(DaggerfallTerrain daggerfallTerrain, TerrainMaterialData terrainMaterialData);
    }

    /// <summary>
    /// Base class for Daggerfall terrain material providers.
    /// </summary>
    public abstract class TerrainMaterialProvider : ITerrainMaterialProvider
    {
        /// <summary>
        /// Gets default implementation supported on current system.
        /// </summary>
        internal static TerrainMaterialProvider Default
        {
            get
            {
                if (TilemapTextureArrayTerrainMaterialProvider.IsSupported)
                    return new TilemapTextureArrayTerrainMaterialProvider();
                else
                    return new TilemapTerrainMaterialProvider();
            }
        }

        public abstract Material CreateMaterial();

        public abstract void PromoteMaterial(DaggerfallTerrain daggerfallTerrain, TerrainMaterialData terrainMaterialData);

        /// <summary>
        /// Parses climate informations and retrieves ground archive index.
        /// </summary>
        /// <param name="worldClimate">Index of world climate.</param>
        /// <returns>Texture archive index.</returns>
        protected int GetGroundArchive(int worldClimate)
        {
            return GetClimateInfo(worldClimate).GroundArchive;
        }

        /// <summary>
        /// Parses climate informations.
        /// </summary>
        /// <param name="worldClimate">Index of world climate.</param>
        /// <returns>Parsed climate informations.</returns>
        protected virtual (int GroundArchive, DFLocation.ClimateSettings Settings, bool IsWinter) GetClimateInfo(int worldClimate)
        {
            // Get current climate and ground archive
            DFLocation.ClimateSettings climate = MapsFile.GetWorldClimateSettings(worldClimate);
            int groundArchive = climate.GroundArchive;
            bool isWinter = false;
            if (climate.ClimateType != DFLocation.ClimateBaseType.Desert &&
                DaggerfallUnity.Instance.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
            {
                // Offset to snow textures
                groundArchive++;
                isWinter = true;
            }

            return (groundArchive, climate, isWinter);
        }
    }

    /// <summary>
    /// Fallback implementation for terrain materials, supported on most platforms.
    /// </summary>
    public class TilemapTerrainMaterialProvider : TerrainMaterialProvider
    {
        private readonly Shader shader = Shader.Find(MaterialReader._DaggerfallTilemapShaderName);

        public sealed override Material CreateMaterial()
        {
            return new Material(shader);
        }

        public override void PromoteMaterial(DaggerfallTerrain daggerfallTerrain, TerrainMaterialData terrainMaterialData)
        {
            // Get tileset material to "steal" atlas texture for our shader
            Material tileSetMaterial = DaggerfallUnity.Instance.MaterialReader.GetTerrainTilesetMaterial(GetGroundArchive(terrainMaterialData.WorldClimate));

            // Assign textures
            terrainMaterialData.Material.SetTexture(TileUniforms.TileAtlasTex, tileSetMaterial.GetTexture(TileUniforms.TileAtlasTex));
            terrainMaterialData.Material.SetTexture(TileUniforms.TilemapTex, terrainMaterialData.TileMapTexture);
            terrainMaterialData.Material.SetInt(TileUniforms.TilemapDim, MapsFile.WorldMapTileDim);
        }
    }

    /// <summary>
    /// Implements terrain materials using <see cref="Texture2DArray"/>, with support for custom textures.
    /// </summary>
    public class TilemapTextureArrayTerrainMaterialProvider : TerrainMaterialProvider
    {
        private readonly Shader shader = Shader.Find(MaterialReader._DaggerfallTilemapTextureArrayShaderName);

        internal static bool IsSupported
        {
            get { return SystemInfo.supports2DArrayTextures && DaggerfallUnity.Settings.EnableTextureArrays; }
        }

        public sealed override Material CreateMaterial()
        {
            return new Material(shader);
        }

        public override void PromoteMaterial(DaggerfallTerrain daggerfallTerrain, TerrainMaterialData terrainMaterialData)
        {
            Material tileMaterial = DaggerfallUnity.Instance.MaterialReader.GetTerrainTextureArrayMaterial(GetGroundArchive(terrainMaterialData.WorldClimate));

            // Assign textures (propagate material settings from tileMaterial to terrainMaterial)
            terrainMaterialData.Material.SetTexture(TileTexArrUniforms.TileTexArr, tileMaterial.GetTexture(TileTexArrUniforms.TileTexArr));
            terrainMaterialData.Material.SetTexture(TileTexArrUniforms.TileNormalMapTexArr, tileMaterial.GetTexture(TileTexArrUniforms.TileNormalMapTexArr));
            terrainMaterialData.Material.SetTexture(TileTexArrUniforms.TileParallaxMapTexArr, tileMaterial.GetTexture(TileTexArrUniforms.TileParallaxMapTexArr));
            terrainMaterialData.Material.SetTexture(TileTexArrUniforms.TileMetallicGlossMapTexArr, tileMaterial.GetTexture(TileTexArrUniforms.TileMetallicGlossMapTexArr));
            terrainMaterialData.Material.SetTexture(TileTexArrUniforms.TilemapTex, terrainMaterialData.TileMapTexture);

            // Assign keywords (propagate material keywords from tileMaterial to terrainMaterial)
            AssignKeyWord(KeyWords.NormalMap, tileMaterial, terrainMaterialData.Material);
            AssignKeyWord(KeyWords.HeightMap, tileMaterial, terrainMaterialData.Material);
            AssignKeyWord(KeyWords.MetallicGlossMap, tileMaterial, terrainMaterialData.Material);
        }

        void AssignKeyWord(string keyword, Material src, Material dst)
        {
            if (src.IsKeywordEnabled(keyword))
                dst.EnableKeyword(keyword);
            else
                dst.DisableKeyword(keyword);
        }
    }
}
