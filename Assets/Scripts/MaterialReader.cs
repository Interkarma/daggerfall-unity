// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
    /// <summary>
    /// Imports Daggerfall images into Unity.
    /// Should only be attached to DaggerfallUnity singleton (for which it is a required component).
    /// Note: Texture loading methods have been moved to TextureReader class.
    /// </summary>
    [RequireComponent(typeof(DaggerfallUnity))]
    public class MaterialReader : MonoBehaviour
    {
        #region Fields

        // General settings
        public bool AtlasTextures = true;
        public bool CompressSkyTextures = false;
        public bool Sharpen = false;
        public bool GenerateNormals = false;
        public float NormalTextureStrength = 0.2f;
        public FilterMode MainFilterMode = FilterMode.Point;
        public FilterMode SkyFilterMode = FilterMode.Point;
        public bool MipMaps = true;
        public bool ReadableTextures = false;
        public SupportedAlphaTextureFormats AlphaTextureFormat = SupportedAlphaTextureFormats.RGBA32;
        public SupportedNonAlphaTextureFormats NonAlphaTextureFormat = SupportedNonAlphaTextureFormats.RGB24;

        // Window settings
        public Color DayWindowColor = new Color32(89, 154, 178, 0xff);
        public Color NightWindowColor = new Color32(255, 182, 56, 0xff);
        public Color FogWindowColor = new Color32(117, 117, 117, 0xff);
        public Color CustomWindowColor = new Color32(200, 0, 200, 0xff);
        public float DayWindowIntensity = 0.5f;
        public float NightWindowIntensity = 0.8f;
        public float FogWindowIntensity = 0.5f;
        public float CustomWindowIntensity = 1.0f;

        // Keys groups use increments of 512 as this the total number of texture file indices.
        // Groups allow the same material to be uniquely cached based on usage.
        // There can be a maximum of 128 unique groups of materials in cache.
        public const int MainKeyGroup = 0;
        public const int AtlasKeyGroup = 512;
        public const int TileMapKeyGroup = 1024;
        //public const int UnusedKeyGroup1 = 1536;
        //public const int UnusedKeyGroup2 = 2048;
        //public const int UnusedKeyGroup3 = 2560;
        //public const int UnusedKeyGroup4 = 3584;
        //public const int UnusedKeyGroup5 = 4096;

        // Shader names
        public const string _StandardShaderName = "Standard";
        public const string _DaggerfallTilemapShaderName = "Daggerfall/Tilemap";
        public const string _DaggerfallBillboardShaderName = "Daggerfall/Billboard";
        public const string _DaggerfallBillboardBatchShaderName = "Daggerfall/BillboardBatch";

        DaggerfallUnity dfUnity;
        TextureReader textureReader;
        Dictionary<int, CachedMaterial> materialDict = new Dictionary<int, CachedMaterial>();
        TextureAtlasBuilder miscBillboardsAtlas = null;

        #endregion

        #region Enums & Structs

        /// <summary>
        /// Standard shader blend modes.
        /// Using a custom enum as Unity does not expose outside of editor GUI.
        /// </summary>
        public enum CustomBlendMode
        {
            Opaque = 0,
            Cutout = 1,
            Fade = 2,
            Transparent = 3,
        }

        /// <summary>
        /// Standard shader smoothness map channel setting.
        /// Using a custom enum as Unity does not expose outside of editor GUI.
        /// </summary>
        public enum CustomSmoothnessMapChannel
        {
            SpecularMetallicAlpha = 0,
            AlbedoAlpha = 1,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets true if file reading is ready.
        /// </summary>
        public bool IsReady
        {
            get { return ReadyCheck(); }
        }

        /// <summary>
        /// Gets managed texture reader.
        /// </summary>
        public TextureReader TextureReader
        {
            get { return (IsReady) ? textureReader : null; }
        }

        /// <summary>
        /// TEMP: Gets a special misc billboards super-atlas for scene builders.
        /// </summary>
        public TextureAtlasBuilder MiscBillboardAtlas
        {
            get
            {
                // Create new atlas or return existing
                if (miscBillboardsAtlas == null)
                {
                    if (!IsReady)
                        return null;

                    miscBillboardsAtlas = textureReader.CreateTextureAtlasBuilder(
                        textureReader.MiscFlatsTextureArchives,
                        2,
                        true,
                        2048,
                        AlphaTextureFormat,
                        NonAlphaTextureFormat);

                    return miscBillboardsAtlas;
                }
                else
                {
                    return miscBillboardsAtlas;
                }
            }
        }

        #endregion

        #region Material Creation

        /// <summary>
        /// Creates new Standard material with default properties suitable for most Daggerfall textures.
        /// </summary>
        /// <returns></returns>
        public static Material CreateStandardMaterial(
            CustomBlendMode blendMode = CustomBlendMode.Opaque,
            CustomSmoothnessMapChannel smoothnessChannel = CustomSmoothnessMapChannel.AlbedoAlpha,
            float metallic = 0,
            float glossiness = 1)
        {
            // Create material
            Shader shader = Shader.Find(_StandardShaderName);
            Material material = new Material(shader);

            // Set blend mode
            SetBlendMode(material, blendMode, smoothnessChannel, metallic, glossiness);

            return material;
        }

        /// <summary>
        /// Change the blend mode of a Standard material at runtime.
        /// </summary>
        public static void SetBlendMode(
            Material material,
            CustomBlendMode blendMode,
            CustomSmoothnessMapChannel smoothnessChannel,
            float metallic,
            float glossiness)
        {
            // Set properties
            material.SetFloat("_Mode", (int)blendMode);
            material.SetFloat("_SmoothnessTextureChannel", (int)smoothnessChannel);
            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Glossiness", glossiness);

            switch (blendMode)
            {
                case CustomBlendMode.Opaque:
                    material.SetOverrideTag("RenderType", "");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case CustomBlendMode.Cutout:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    break;
                case CustomBlendMode.Fade:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
                case CustomBlendMode.Transparent:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
            }
        }

        #endregion

        #region Material Loading

        /// <summary>
        /// Gets Unity Material from Daggerfall texture.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <returns>Material or null.</returns>
        public Material GetMaterial(int archive, int record, int frame = 0, int alphaIndex = -1)
        {
            Rect rect;
            return GetMaterial(archive, record, frame, alphaIndex, out rect, 0, false);
        }

        /// <summary>
        /// Gets Unity Material from Daggerfall texture with more options.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <param name="rectOut">Receives UV rect for texture inside border.</param>
        /// <param name="borderSize">Number of pixels internal border around each texture.</param>
        /// <param name="dilate">Blend texture into surrounding empty pixels.</param>
        /// <param name="isBillboard">Set true when creating atlas material for simple billboards.</param>
        /// <returns>Material or null.</returns>
        public Material GetMaterial(
            int archive,
            int record,
            int frame,
            int alphaIndex,
            out Rect rectOut,
            int borderSize = 0,
            bool dilate = false,
            bool isBillboard = false)
        {
            // Ready check
            if (!IsReady)
            {
                rectOut = new Rect();
                return null;
            }

            // Try to retrieve from cache
            int key = MakeTextureKey((short)archive, (byte)record, (byte)frame);
            if (materialDict.ContainsKey(key))
            {
                CachedMaterial cm = materialDict[key];
                rectOut = cm.singleRect;
                return cm.material;
            }

            // Create new texture settings
            GetTextureSettings settings = TextureReader.CreateTextureSettings(archive, record, frame, alphaIndex, borderSize, dilate);
            settings.autoEmissionForWindows = true;
            settings.sharpen = Sharpen;
            if (GenerateNormals)
            {
                settings.createNormalMap = true;
                settings.normalStrength = NormalTextureStrength;
            }

            // Set emissive for self-illuminated textures
            if (textureReader.IsEmissive(archive, record))
            {
                settings.createEmissionMap = true;
                settings.emissionIndex = -1;
            }

            // Get texture
            GetTextureResults results = textureReader.GetTexture2D(settings, AlphaTextureFormat, NonAlphaTextureFormat);
            rectOut = results.singleRect;

            // Create material
            Material material;
            if (isBillboard)
                material = CreateStandardMaterial(CustomBlendMode.Cutout);
            else
                material = CreateStandardMaterial();

            // Setup material
            material.name = FormatName(archive, record);
            material.mainTexture = results.albedoMap;
            material.mainTexture.filterMode = MainFilterMode;

            // Setup normal map
            if (GenerateNormals && results.normalMap != null)
            {
                results.normalMap.filterMode = MainFilterMode;
                material.SetTexture("_BumpMap", results.normalMap);
                material.EnableKeyword("_NORMALMAP");
            }

            // Setup emission map
            if (results.isEmissive && !results.isWindow && results.emissionMap != null)
            {
                results.emissionMap.filterMode = MainFilterMode;
                material.SetTexture("_EmissionMap", results.emissionMap);
                material.SetColor("_EmissionColor", Color.white);
                material.EnableKeyword("_EMISSION");
            }
            else if (results.isEmissive && results.isWindow && results.emissionMap != null)
            {
                results.emissionMap.filterMode = MainFilterMode;
                material.SetTexture("_EmissionMap", results.emissionMap);
                material.SetColor("_EmissionColor", DayWindowColor * DayWindowIntensity);
                material.EnableKeyword("_EMISSION");
            }

            // Setup cached material
            DFSize size = results.textureFile.GetSize(record);
            DFSize scale = results.textureFile.GetScale(record);
            DFPosition offset = results.textureFile.GetOffset(record);
            Vector2[] recordSizes = new Vector2[1] { new Vector2(size.Width, size.Height) };
            Vector2[] recordScales = new Vector2[1] { new Vector2(scale.Width, scale.Height) };
            Vector2[] recordOffsets = new Vector2[1] { new Vector2(offset.X, offset.Y) };
            CachedMaterial newcm = new CachedMaterial()
            {
                key = key,
                keyGroup = 0,
                albedoMap = results.albedoMap,
                normalMap = results.normalMap,
                emissionMap = results.emissionMap,
                singleRect = rectOut,
                material = material,
                filterMode = MainFilterMode,
                isWindow = results.isWindow,
                recordSizes = recordSizes,
                recordScales = recordScales,
                recordOffsets = recordOffsets,
                singleFrameCount = results.textureFile.GetFrameCount(record),
            };
            materialDict.Add(key, newcm);

            return material;
        }

        /// <summary>
        /// Gets Unity Material atlas from Daggerfall texture archive.
        /// </summary>
        /// <param name="archive">Archive index to create atlas from.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <param name="rectsOut">Array of rects, one for each record sub-texture and frame.</param>
        /// <param name="padding">Number of pixels each sub-texture.</param>
        /// <param name="maxAtlasSize">Max size of atlas.</param>
        /// <param name="rectsOut">Array of rects, one for each record sub-texture and frame.</param>
        /// <param name="indicesOut">Array of record indices into rect array, accounting for animation frames.</param>
        /// <param name="border">Number of pixels internal border around each texture.</param>
        /// <param name="dilate">Blend texture into surrounding empty pixels.</param>
        /// <param name="shrinkUVs">Number of pixels to shrink UV rect.</param>
        /// <param name="copyToOppositeBorder">Copy texture edges to opposite border. Requires border, will overwrite dilate.</param>
        /// <param name="shader">Shader for material. If null, DefaultShaderName will be applied.</param>
        /// <param name="isBillboard">Set true when creating atlas material for simple billboards.</param>
        /// <returns>Material or null.</returns>
        public Material GetMaterialAtlas(
            int archive,
            int alphaIndex,
            int padding,
            int maxAtlasSize,
            out Rect[] rectsOut,
            out RecordIndex[] indicesOut,
            int border = 0,
            bool dilate = false,
            int shrinkUVs = 0,
            bool copyToOppositeBorder = false,
            bool isBillboard = false)
        {
            // Ready check
            if (!IsReady)
            {
                rectsOut = null;
                indicesOut = null;
                return null;
            }

            int key = MakeTextureKey((short)archive, (byte)0, (byte)0, AtlasKeyGroup);
            if (materialDict.ContainsKey(key))
            {
                CachedMaterial cm = materialDict[key];
                if (cm.filterMode == MainFilterMode)
                {
                    // Properties are the same
                    rectsOut = cm.atlasRects;
                    indicesOut = cm.atlasIndices;
                    return cm.material;
                }
                else
                {
                    // Properties don't match, remove material and reload
                    materialDict.Remove(key);
                }
            }

            // Create material
            Material material;
            if (isBillboard)
                material = CreateStandardMaterial(CustomBlendMode.Cutout);
            else
                material = CreateStandardMaterial();

            // Create settings
            GetTextureSettings settings = TextureReader.CreateTextureSettings(archive, 0, 0, alphaIndex, border, dilate);
            settings.createNormalMap = GenerateNormals;
            settings.autoEmission = true;
            settings.atlasShrinkUVs = shrinkUVs;
            settings.atlasPadding = padding;
            settings.atlasMaxSize = maxAtlasSize;
            settings.copyToOppositeBorder = copyToOppositeBorder;

            // Setup material
            material.name = string.Format("TEXTURE.{0:000} [Atlas]", archive);
            GetTextureResults results = textureReader.GetTexture2DAtlas(settings, AlphaTextureFormat, NonAlphaTextureFormat);
            material.mainTexture = results.albedoMap;
            material.mainTexture.filterMode = MainFilterMode;

            // Setup normal map
            if (GenerateNormals && results.normalMap != null)
            {
                results.normalMap.filterMode = MainFilterMode;
                material.SetTexture("_BumpMap", results.normalMap);
                material.EnableKeyword("_NORMALMAP");
            }

            // Setup emission map
            if (results.isEmissive && results.emissionMap != null)
            {
                results.emissionMap.filterMode = MainFilterMode;
                material.SetTexture("_EmissionMap", results.emissionMap);
                material.SetColor("_EmissionColor", Color.white);
                material.EnableKeyword("_EMISSION");
            }

            // TEMP: Bridging between legacy material out params and GetTextureResults for now
            Vector2[] sizesOut, scalesOut, offsetsOut;
            sizesOut = results.atlasSizes.ToArray();
            scalesOut = results.atlasScales.ToArray();
            offsetsOut = results.atlasOffsets.ToArray();
            rectsOut = results.atlasRects.ToArray();
            indicesOut = results.atlasIndices.ToArray();

            // Setup cached material
            CachedMaterial newcm = new CachedMaterial();
            newcm.key = key;
            newcm.keyGroup = AtlasKeyGroup;
            newcm.atlasRects = rectsOut;
            newcm.atlasIndices = indicesOut;
            newcm.material = material;
            newcm.filterMode = MainFilterMode;
            newcm.recordSizes = sizesOut;
            newcm.recordScales = scalesOut;
            newcm.recordOffsets = offsetsOut;
            newcm.atlasFrameCounts = results.atlasFrameCounts.ToArray();
            materialDict.Add(key, newcm);

            return material;
        }

        /// <summary>
        /// Gets Unity Material from Daggerfall terrain tilemap texture.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <returns>Material or null.</returns>
        public Material GetTerrainTilesetMaterial(int archive)
        {
            // Ready check
            if (!IsReady)
                return null;

            // Return from cache if present
            int key = MakeTextureKey((short)archive, (byte)0, (byte)0, TileMapKeyGroup);
            if (materialDict.ContainsKey(key))
            {
                CachedMaterial cm = materialDict[key];
                if (cm.filterMode == MainFilterMode)
                {
                    // Properties are the same
                    return cm.material;
                }
                else
                {
                    // Properties don't match, remove material and reload
                    materialDict.Remove(key);
                }
            }

            //// TODO: Attempt to load prebuilt atlas asset, otherwise create one in memory
            //Texture2D texture = TerrainAtlasBuilder.LoadTerrainAtlasTextureResource(archive, CreateTextureAssetResourcesPath);
            //if (texture == null)

            // Generate atlas
            // Not currently generating normals as very slow on such a large texture
            // and results are not very noticeable
            GetTextureResults results = textureReader.GetTerrainTilesetTexture(archive);
            results.albedoMap.filterMode = MainFilterMode;

            Shader shader = Shader.Find(_DaggerfallTilemapShaderName);
            Material material = new Material(shader);
            material.name = string.Format("TEXTURE.{0:000} [Tilemap]", archive);
            material.SetTexture("_TileAtlasTex", results.albedoMap);

            CachedMaterial newcm = new CachedMaterial()
            {
                key = key,
                keyGroup = TileMapKeyGroup,
                material = material,
                filterMode = MainFilterMode,
            };
            materialDict.Add(key, newcm);

            return material;
        }

        #endregion

        #region Support

        /// <summary>
        /// Gets CachedMaterial properties.
        /// Material will be loaded into cache if not present already.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <param name="cachedMaterialOut">CachedMaterial out.</param>
        /// <param name="alphaIndex">Alpha index used if material needs to be loaded.</param>
        /// <returns>True if CachedMaterial found or loaded successfully.</returns>
        public bool GetCachedMaterial(int archive, int record, int frame, out CachedMaterial cachedMaterialOut, int alphaIndex = -1)
        {
            int key = MakeTextureKey((short)archive, (byte)record, (byte)frame);
            if (materialDict.ContainsKey(key))
            {
                cachedMaterialOut = materialDict[key];
                return true;
            }
            else
            {
                // Not in cache - try to load material
                if (GetMaterial(archive, record, frame, alphaIndex) == null)
                {
                    cachedMaterialOut = new CachedMaterial();
                    return false;
                }
            }

            cachedMaterialOut = materialDict[key];

            return true;
        }

        /// <summary>
        /// Sets CachedMaterial properties.
        /// existing Material will be updated in cache with cachedMaterialIn.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <param name="cachedMaterialIn">the CachedMaterial used to update the cache.</param>
        /// <returns>True if CachedMaterial was found and updated successfully.</returns>
        public bool SetCachedMaterial(int archive, int record, int frame, CachedMaterial cachedMaterialIn)
        {
            int key = MakeTextureKey((short)archive, (byte)record, (byte)frame);
            if (materialDict.ContainsKey(key))
            {
                materialDict[key] = cachedMaterialIn;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets CachedMaterial properties for an atlased material.
        /// Atlas material will not be loaded automatically if not found in cache.
        /// </summary>
        /// <param name="archive">Atlas archive index.</param>
        /// <param name="cachedMaterialOut">CachedMaterial out</param>
        /// <returns>True if CachedMaterial found.</returns>
        public bool GetCachedMaterialAtlas(int archive, out CachedMaterial cachedMaterialOut)
        {
            int key = MakeTextureKey((short)archive, (byte)0, (byte)0, AtlasKeyGroup);
            if (materialDict.ContainsKey(key))
            {
                cachedMaterialOut = materialDict[key];
                return true;
            }

            cachedMaterialOut = new CachedMaterial();
            return false;
        }

        /// <summary>
        /// Get a new Material based on climate.
        /// </summary>
        /// <param name="key">Material key.</param>
        /// <param name="climate">New climate base.</param>
        /// <param name="season">New season.</param>
        /// <param name="windowStyle">New window style.</param>
        /// <returns>New material.</returns>
        public Material ChangeClimate(int key, ClimateBases climate, ClimateSeason season, WindowStyle windowStyle)
        {
            // Ready check
            if (!IsReady)
                return null;

            // Reverse key and apply climate
            int archive, record, frame;
            ReverseTextureKey(key, out archive, out record, out frame);
            archive = ClimateSwaps.ApplyClimate(archive, record, climate, season);

            // Get new material
            Material material;
            CachedMaterial cm = GetMaterialFromCache(archive, record, out material);

            // Handle windows
            if (cm.isWindow)
            {
                ChangeWindowEmissionColor(material, windowStyle);
            }

            return material;
        }

        /// <summary>
        /// Change emission colour of window materials.
        /// </summary>
        /// <param name="material">Source material to change.</param>
        /// <param name="windowStyle">New window style.</param>
        public void ChangeWindowEmissionColor(Material material, WindowStyle windowStyle)
        {
            switch (windowStyle)
            {
                case WindowStyle.Day:
                    material.SetColor("_EmissionColor", DayWindowColor * DayWindowIntensity);
                    break;
                case WindowStyle.Night:
                    material.SetColor("_EmissionColor", NightWindowColor * NightWindowIntensity);
                    break;
                case WindowStyle.Fog:
                    material.SetColor("_EmissionColor", FogWindowColor * FogWindowIntensity);
                    break;
                case WindowStyle.Custom:
                    material.SetColor("_EmissionColor", CustomWindowColor * CustomWindowIntensity);
                    break;
                case WindowStyle.Disabled:
                    material.SetColor("_EmissionColor", Color.black);
                    break;
            }
        }

        /// <summary>
        /// Clears material cache dictionary, forcing material to reload.
        /// </summary>
        public void ClearCache()
        {
            materialDict.Clear();
        }

        #endregion

        #region Static Methods

        public static int MakeTextureKey(short archive, byte record, byte frame = 0, int group = 0)
        {
            return ((archive + group) << 16) + (record << 8) + frame;
        }

        public static void ReverseTextureKey(int key, out int archiveOut, out int recordOut, out int frameOut, int group = 0)
        {
            archiveOut = (key >> 16) - group;
            recordOut = (key >> 8) & 0xff;
            frameOut = key & 0xff;
        }

        public static FlatTypes GetFlatType(int archive)
        {
            // Single-archive types
            switch (archive)
            {
                case 210:
                    return FlatTypes.Light;
                case 199:
                    return FlatTypes.Editor;
                case 201:
                    return FlatTypes.Animal;
            }

            // Nature set type
            if (archive >= 500 && archive <= 511)
                return FlatTypes.Nature;

            // Everything else is just decoration for now
            return FlatTypes.Decoration;
        }

        public static EditorFlatTypes GetEditorFlatType(int record)
        {
            switch (record)
            {
                case 8:
                    return EditorFlatTypes.Enter;
                case 10:
                    return EditorFlatTypes.Start;
                default:
                    return EditorFlatTypes.Other;
            }
        }

        #endregion

        #region Private Methods

        private CachedMaterial GetMaterialFromCache(int archive, int record, out Material materialOut)
        {
            materialOut = GetMaterial(archive, record);
            int key = MakeTextureKey((short)archive, (byte)record);

            return materialDict[key];
        }

        private bool ReadyCheck()
        {
            // Ensure we have a DaggerfallUnity reference
            if (dfUnity == null)
            {
                dfUnity = DaggerfallUnity.Instance;
            }

            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady)
            {
                DaggerfallUnity.LogMessage("MaterialReader: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            // Ensure texture reader is ready
            if (textureReader == null)
            {
                textureReader = new TextureReader(dfUnity.Arena2Path);
            }

            return true;
        }

        private string FormatName(int archive, int record)
        {
            return string.Format("TEXTURE.{0:000} [Index={1}]", archive, record);
        }

        private string FormatImageName(string filename, int record)
        {
            return string.Format("{0} [Index={1}]", filename, record);
        }

        #endregion
    }
}
