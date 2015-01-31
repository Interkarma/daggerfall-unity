// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

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
        public FilterMode MainFilterMode = FilterMode.Point;
        public FilterMode SkyFilterMode = FilterMode.Point;
        public bool MipMaps = true;
        public string DefaultShaderName = "Diffuse";
        public string DefaultSelfIlluminShaderName = "Self-Illumin/Diffuse";
        public string DefaultBillboardShaderName = "Transparent/Diffuse";
        public string DefaultUnlitBillboardShaderName = "Unlit/Transparent";
        public string DefaultUnlitTextureShaderName = "Unlit/Texture";
        public string DefaultWeaponShaderName = "Unlit/Transparent";
        public const string _DaggerfallTilemapShaderName = "Daggerfall/Tilemap";
        public const string _DaggerfallTerrainTilemapShaderName = "Daggerfall/TerrainTilemap";
        public const string _DaggerfallBillboardBatchShaderName = "Daggerfall/BillboardBatch/TransparentCutoutForceForward";

        // Window settings
        public Color DayWindowColor = new Color32(89, 154, 178, 0xff);
        public Color NightWindowColor = new Color32(255, 182, 56, 0xff);
        public Color FogWindowColor = new Color32(117, 117, 117, 0xff);
        public Color CustomWindowColor = new Color32(200, 0, 200, 0xff);
        public float DayWindowIntensity = 0.3f;
        public float NightWindowIntensity = 0.6f;
        public float FogWindowIntensity = 0.1f;
        public float CustomWindowIntensity = 0.5f;

        // Keys groups use increments of 512 as this the total number of texture file indices.
        // Groups allow the same material to be uniquely cached based on usage.
        // There can be a maximum of 128 unique groups of materials in cache.
        public const int MainKeyGroup = 0;
        public const int AtlasKeyGroup = 512;
        public const int DayWindowKeyGroup = 1024;
        public const int NightWindowKeyGroup = 1536;
        public const int FogWindowKeyGroup = 2048;
        public const int CustomWindowKeyGroup = 2560;
        public const int TileMapKeyGroup = 3072;
        public const int UnusedKeyGroup2 = 3584;
        public const int UnusedKeyGroup3 = 4096;

        DaggerfallUnity dfUnity;
        TextureReader textureReader;
        Dictionary<int, CachedMaterial> materialDict = new Dictionary<int, CachedMaterial>();

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
            get { return textureReader; }
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
        /// <param name="shader">Shader for material. If null, DefaultShaderName will be applied.</param>
        /// <returns>Material or null.</returns>
        public Material GetMaterial(int archive, int record, int frame = 0, int alphaIndex = -1, Shader shader = null)
        {
            Rect rect;
            return GetMaterial(archive, record, frame, alphaIndex, out rect, 0, false, shader);
        }

        /// <summary>
        /// Gets Unity Material from Daggerfall texture with more options.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <param name="rectOut">Receives UV rect for texture inside border.</param>
        /// <param name="border">Number of pixels internal border around each texture.</param>
        /// <param name="dilate">Blend texture into surrounding empty pixels.</param>
        /// <param name="shader">Shader for material. If null, DefaultShaderName will be applied.</param>
        /// <returns>Material or null.</returns>
        public Material GetMaterial(
            int archive,
            int record,
            int frame,
            int alphaIndex,
            out Rect rectOut,
            int border = 0,
            bool dilate = false,
            Shader shader = null)
        {
            // Ready check
            if (!IsReady)
            {
                rectOut = new Rect();
                return null;
            }

            // HACK: Override shader for unlit textures
            // TODO: Find a better way to do this
            if (archive == 356 && (record == 0 || record == 2 || record == 3) ||
                archive == 87 && record == 0)
            {
                // Only override if not specified
                if (shader == null)
                    shader = Shader.Find(dfUnity.MaterialReader.DefaultUnlitTextureShaderName);
            }

            int key = MakeTextureKey((short)archive, (byte)record, (byte)frame);
            if (materialDict.ContainsKey(key))
            {
                CachedMaterial cm = materialDict[key];
                if (cm.filterMode == MainFilterMode)
                {
                    // Properties are the same
                    rectOut = cm.singleRect;
                    return cm.material;
                }
                else
                {
                    // Properties don't match, remove material and reload
                    materialDict.Remove(key);
                }
            }

            if (shader == null)
                shader = Shader.Find(DefaultShaderName);

            Material material = new Material(shader);
            material.name = FormatName(archive, record);
            Texture2D texture = textureReader.GetTexture2D(archive, record, frame, alphaIndex, out rectOut, border, dilate);
            material.mainTexture = texture;
            material.mainTexture.filterMode = MainFilterMode;

            DFSize size = textureReader.TextureFile.GetSize(record);
            DFSize scale = textureReader.TextureFile.GetScale(record);
            DFSize offset = textureReader.TextureFile.GetOffset(record);
            Vector2[] recordSizes = new Vector2[1] { new Vector2(size.Width, size.Height) };
            Vector2[] recordScales = new Vector2[1] { new Vector2(scale.Width, scale.Height) };
            Vector2[] recordOffsets = new Vector2[1] { new Vector2(offset.Width, offset.Height) };
            CachedMaterial newcm = new CachedMaterial()
            {
                key = key,
                keyGroup = 0,
                singleRect = rectOut,
                material = material,
                filterMode = MainFilterMode,
                isWindow = ClimateSwaps.IsExteriorWindow(archive, record),
                recordSizes = recordSizes,
                recordScales = recordScales,
                recordOffsets = recordOffsets,
                recordFrameCount = textureReader.TextureFile.GetFrameCount(record),
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
            Shader shader = null)
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
                    indicesOut = cm.indices;
                    return cm.material;
                }
                else
                {
                    // Properties don't match, remove material and reload
                    materialDict.Remove(key);
                }
            }

            if (shader == null)
                shader = Shader.Find(DefaultShaderName);

            Vector2[] sizesOut, scalesOut, offsetsOut;
            Material material = new Material(shader);
            material.name = string.Format("TEXTURE.{0:000} [Atlas]", archive);
            Texture2D texture = textureReader.GetTexture2DAtlas(
                archive,
                alphaIndex,
                padding,
                maxAtlasSize,
                out rectsOut,
                out indicesOut,
                out sizesOut,
                out scalesOut,
                out offsetsOut,
                border,
                dilate,
                shrinkUVs,
                copyToOppositeBorder);
            material.mainTexture = texture;
            material.mainTexture.filterMode = MainFilterMode;

            CachedMaterial newcm = new CachedMaterial();
            newcm.key = key;
            newcm.keyGroup = AtlasKeyGroup;
            newcm.atlasRects = rectsOut;
            newcm.indices = indicesOut;
            newcm.material = material;
            newcm.filterMode = MainFilterMode;
            newcm.recordSizes = sizesOut;
            newcm.recordScales = scalesOut;
            newcm.recordOffsets = offsetsOut;
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
            Texture2D texture = textureReader.GetTerrainTilesetTexture(archive);
            texture.filterMode = MainFilterMode;

            Shader shader = Shader.Find(_DaggerfallTilemapShaderName);
            Material material = new Material(shader);
            material.name = string.Format("TEXTURE.{0:000} [Tilemap]", archive);
            material.mainTexture = texture;

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
        /// <param name="shader">Shader used if material needs to be loaded.</param>
        /// <returns>True if CachedMaterial found or loaded successfully.</returns>
        public bool GetCachedMaterial(int archive, int record, int frame, out CachedMaterial cachedMaterialOut, int alphaIndex = -1, Shader shader = null)
        {
            int key = MakeTextureKey((short)archive, (byte)record, (byte)frame);
            if (materialDict.ContainsKey(key))
            {
                cachedMaterialOut = materialDict[key];
                return true;
            }
            else
            {
                // Not in cache, try to load material
                if (GetMaterial(archive, record, frame, alphaIndex, shader) == null)
                {
                    cachedMaterialOut = new CachedMaterial();
                    return false;
                }
            }

            cachedMaterialOut = materialDict[key];

            return true;
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
                material = GetWindowMaterial(cm.key, windowStyle);

            return material;
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

        private Material GetWindowMaterial(int key, WindowStyle windowStyle)
        {
            // Reverse key (input must be a MainKeyGroup key)
            int archive, record, frame;
            ReverseTextureKey(key, out archive, out record, out frame);

            // Determine new key group based on style
            int group;
            Color color;
            float intensity;
            switch (windowStyle)
            {
                case WindowStyle.Day:
                    group = DayWindowKeyGroup;
                    color = dfUnity.MaterialReader.DayWindowColor;
                    intensity = dfUnity.MaterialReader.DayWindowIntensity;
                    break;
                case WindowStyle.Night:
                    group = NightWindowKeyGroup;
                    color = dfUnity.MaterialReader.NightWindowColor;
                    intensity = dfUnity.MaterialReader.NightWindowIntensity;
                    break;
                case WindowStyle.Fog:
                    group = FogWindowKeyGroup;
                    color = dfUnity.MaterialReader.FogWindowColor;
                    intensity = dfUnity.MaterialReader.FogWindowIntensity;
                    break;
                case WindowStyle.Custom:
                    group = CustomWindowKeyGroup;
                    color = dfUnity.MaterialReader.CustomWindowColor;
                    intensity = dfUnity.MaterialReader.CustomWindowIntensity;
                    break;
                default:
                    return GetMaterial(archive, record);    // Just get base material with no processing
            }

            // Make new key based on group
            int newkey = MakeTextureKey((short)archive, (byte)record, (byte)0, group);

            // Check if material is already in cache
            CachedMaterial cm;
            if (materialDict.TryGetValue(newkey, out cm))
            {
                // Return same if settings have not changed since last time
                if (cm.windowColor == color &&
                    cm.windowIntensity == intensity &&
                    cm.filterMode == MainFilterMode)
                {
                    // Properties are the same
                    return cm.material;
                }
                else
                {
                    materialDict.Remove(newkey);
                }
            }

            // Load texture file and get colour arrays
            textureReader.TextureFile.Load(Path.Combine(dfUnity.Arena2Path, TextureFile.IndexToFileName(archive)), FileUsage.UseMemory, true);
            Color32 alpha = new Color32(0, 0, 0, (byte)(255f / intensity));
            Color32[] diffuseColors, alphaColors;
            DFSize sz = textureReader.TextureFile.GetWindowColors32(record, color, alpha, out diffuseColors, out alphaColors);

            // Create diffuse texture
            Texture2D diffuse = new Texture2D(sz.Width, sz.Height, TextureFormat.RGBA32, MipMaps);
            diffuse.SetPixels32(diffuseColors);
            diffuse.Apply(true);

            // Create illumin texture
            Texture2D illumin = new Texture2D(sz.Width, sz.Height, TextureFormat.RGBA32, MipMaps);
            illumin.SetPixels32(alphaColors);
            illumin.Apply(true);

            // Create new material with a self-illuminating shader
            Shader shader = Shader.Find(DefaultSelfIlluminShaderName);
            Material material = new Material(shader);
            material.name = FormatName(archive, record);

            // Set material textures
            material.SetTexture("_MainTex", diffuse);
            material.SetTexture("_Illum", illumin);
            material.mainTexture.filterMode = MainFilterMode;

            // Cache this window material
            CachedMaterial newcm = new CachedMaterial()
            {
                key = newkey,
                keyGroup = group,
                singleRect = new Rect(0, 0, sz.Width, sz.Height),
                material = material,
                filterMode = MainFilterMode,
                isWindow = true,
                windowColor = color,
                windowIntensity = intensity,
            };
            materialDict.Add(newkey, newcm);

            return material;
        }

        #endregion
    }
}
