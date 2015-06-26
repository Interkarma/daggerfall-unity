// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Helper to load Daggerfall TEXTURE.xxx files as Texture2D.
    /// </summary>
    public class TextureReader
    {
        TextureFile textureFile;
        bool mipMaps = true;

        // Special texture indices
        public const int AnimalsTextureArchive = 201;
        public const int LightsTextureArchive = 210;
        public int[] MiscFlatsTextureArchives = new int[] { 97, 205, 211, 212, 213 };

        /// <summary>
        /// Gets or sets Arena2 path.
        /// Path must be set before attempting to load textures.
        /// </summary>
        public string Arena2Path { get; set; }

        /// <summary>
        /// Gets managed API TextureFile.
        /// Used for accessing more properties about texture just loaded
        /// or when manual control over texture loading is required.
        /// </summary>
        public TextureFile TextureFile
        {
            get { return textureFile; }
        }

        /// <summary>
        /// Gets or sets flag to generate mipmaps for textures.
        /// </summary>
        public bool MipMaps
        {
            get { return mipMaps; }
            set { mipMaps = value; }
        }

        /// <summary>
        /// Creates common texture settings.
        /// </summary>
        /// <returns>GetTextureSettings.</returns>
        public static GetTextureSettings CreateTextureSettings(int archive = 0, int record = 0, int frame = 0, int alphaIndex = 0, int borderSize = 0, bool dilate = false, int maxAtlasSize = 2048)
        {
            GetTextureSettings settings = new GetTextureSettings();
            settings.archive = archive;
            settings.record = record;
            settings.frame = frame;
            settings.alphaIndex = alphaIndex;
            settings.borderSize = borderSize;
            settings.dilate = dilate;
            settings.atlasMaxSize = maxAtlasSize;

            return settings;
        }

        /// <summary>
        /// Constructor to set Arena2Path.
        /// </summary>
        /// <param name="arena2Path">Path to Arena2 folder.</param>
        public TextureReader(string arena2Path)
        {
            this.Arena2Path = arena2Path;
        }

        /// <summary>
        /// Gets Unity albedo texture from Daggerfall texture with minimum options.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <returns>Texture2D or null.</returns>
        public Texture2D GetTexture2D(int archive, int record, int frame = 0, int alphaIndex = 0)
        {
            GetTextureSettings settings = new GetTextureSettings();
            settings.archive = archive;
            settings.record = record;
            settings.frame = frame;
            settings.alphaIndex = alphaIndex;
            GetTextureResults results = GetTexture2D(settings);

            return results.albedoMap;
        }

        /// <summary>
        /// Gets Unity textures from Daggerfall texture with all options.
        /// Returns all supported texture maps for Standard shader in one call.
        /// </summary>
        /// <param name="settings">Get texture settings.</param>
        /// <returns>GetTextureResults.</returns>
        public GetTextureResults GetTexture2D(GetTextureSettings settings)
        {
            GetTextureResults results = new GetTextureResults();

            // Ready check
            if (!ReadyCheck())
                return results;

            // Check if window or auto-emissive
            bool isWindow = ClimateSwaps.IsExteriorWindow(settings.archive, settings.record);
            bool isEmissive = (settings.autoEmission) ? IsEmissive(settings.archive, settings.record) : false;

            // Load texture file
            textureFile.Load(Path.Combine(Arena2Path, TextureFile.IndexToFileName(settings.archive)), FileUsage.UseMemory, true);

            // Get starting DFBitmap and albedo Color32 array
            DFSize sz;
            DFBitmap srcBitmap = textureFile.GetDFBitmap(settings.record, settings.frame);
            Color32[] albedoColors = textureFile.GetColors32(srcBitmap, settings.alphaIndex, settings.borderSize, out sz);

            // Sharpen source image
            if (settings.sharpen)
                albedoColors = ImageProcessing.Sharpen(ref albedoColors, sz.Width, sz.Height);

            // Dilate edges
            if (settings.borderSize > 0 && settings.dilate && !settings.copyToOppositeBorder)
                ImageProcessing.DilateColors(ref albedoColors, sz);

            // Copy to opposite border
            if (settings.borderSize > 0 && settings.copyToOppositeBorder)
                ImageProcessing.WrapBorder(ref albedoColors, sz, settings.borderSize);

            // Create albedo texture
            Texture2D albedoMap = null;
            if (settings.alphaIndex < 0)
                albedoMap = new Texture2D(sz.Width, sz.Height, TextureFormat.RGB24, MipMaps);
            else
                albedoMap = new Texture2D(sz.Width, sz.Height, TextureFormat.RGBA32, MipMaps);
            albedoMap.SetPixels32(albedoColors);
            albedoMap.Apply(true, !settings.stayReadable);

            // Create normal texture - must be ARGB32
            Texture2D normalMap = null;
            if (settings.createNormalMap)
            {
                Color32[] normalColors;
                normalColors = ImageProcessing.GetBumpMap(ref albedoColors, sz.Width, sz.Height);
                normalColors = ImageProcessing.ConvertBumpToNormals(ref normalColors, sz.Width, sz.Height, settings.normalStrength);
                normalMap = new Texture2D(sz.Width, sz.Height, TextureFormat.ARGB32, MipMaps);
                normalMap.SetPixels32(normalColors);
                normalMap.Apply(true, !settings.stayReadable);
            }

            // Create basic emissive texture
            Texture2D emissionMap = null;
            bool resultEmissive = false;
            if (settings.createEmissionMap || (settings.autoEmission && isEmissive) && !isWindow)
            {
                // Just reuse albedo map for basic colour emission
                emissionMap = albedoMap;
                resultEmissive = true;
            }

            // Windows need special handling as only glass parts are emissive
            if ((settings.createEmissionMap || settings.autoEmissionForWindows) && isWindow)
            {
                // Create custom emission texture for glass area of windows
                Color32[] emissionColors = textureFile.GetWindowColors32(srcBitmap);
                emissionMap = new Texture2D(sz.Width, sz.Height, TextureFormat.RGBA32, MipMaps);
                emissionMap.SetPixels32(emissionColors);
                emissionMap.Apply(true, !settings.stayReadable);
                resultEmissive = true;
            }

            // Lights need special handling as this archive contains a mix of emissive and non-emissive flats
            // This can cause problems with atlas packing due to mismatch between albedo and emissive texture counts
            if ((settings.createEmissionMap || settings.autoEmission) && settings.archive == LightsTextureArchive)
            {
                // For the unlit flats we create a null-emissive black texture
                if (!isEmissive)
                {
                    Color32[] emissionColors = new Color32[sz.Width * sz.Height];
                    emissionMap = new Texture2D(sz.Width, sz.Height, TextureFormat.RGBA32, MipMaps);
                    emissionMap.SetPixels32(emissionColors);
                    emissionMap.Apply(true, !settings.stayReadable);
                    resultEmissive = true;
                }
            }

            // Shrink UV rect to compensate for internal border
            float ru = 1f / sz.Width;
            float rv = 1f / sz.Height;
            results.singleRect = new Rect(
                settings.borderSize * ru,
                settings.borderSize * rv,
                (sz.Width - settings.borderSize * 2) * ru,
                (sz.Height - settings.borderSize * 2) * rv);

            // Store results
            results.albedoMap = albedoMap;
            results.normalMap = normalMap;
            results.emissionMap = emissionMap;
            results.isWindow = isWindow;
            results.isEmissive = resultEmissive;

            return results;
        }

        /// <summary>
        /// Gets Texture2D atlas from Daggerfall texture archive.
        /// Every record and frame in the archive will be added to atlas.
        /// An array of rects will be returned with sub-texture rect for each record index and frame.
        /// Currently supports one archive per atlas. Super-atlas packing (multiple archives) is in the works.
        /// </summary>
        /// <param name="settings">Get texture settings.</param>
        /// <returns>GetTextureResults.</returns>
        public GetTextureResults GetTexture2DAtlas(GetTextureSettings settings)
        {
            GetTextureResults results = new GetTextureResults();

            // Ready check
            if (!ReadyCheck())
                return results;

            // Individual textures must remain readable to pack into atlas
            bool stayReadable = settings.stayReadable;
            settings.stayReadable = true;

            // Load texture file
            textureFile.Load(Path.Combine(Arena2Path, TextureFile.IndexToFileName(settings.archive)), FileUsage.UseMemory, true);

            // Create lists
            results.atlasSizes = new List<Vector2>(textureFile.RecordCount);
            results.atlasScales = new List<Vector2>(textureFile.RecordCount);
            results.atlasOffsets = new List<Vector2>(textureFile.RecordCount);
            results.atlasFrameCounts = new List<int>(textureFile.RecordCount);

            // Read every texture in archive
            bool hasNormalMaps = false;
            bool hasEmissionMaps = false;
            bool hasAnimation = false;
            List<Texture2D> albedoTextures = new List<Texture2D>();
            List<Texture2D> normalTextures = new List<Texture2D>();
            List<Texture2D> emissionTextures = new List<Texture2D>();
            List<RecordIndex> indices = new List<RecordIndex>();
            for (int record = 0; record < textureFile.RecordCount; record++)
            {
                // Get record index and frame count
                settings.record = record;
                int frames = textureFile.GetFrameCount(record);
                if (frames > 1)
                    hasAnimation = true;

                // Get record information
                DFSize size = textureFile.GetSize(record);
                DFSize scale = textureFile.GetScale(record);
                DFSize offset = textureFile.GetOffset(record);
                RecordIndex ri = new RecordIndex()
                {
                    startIndex = albedoTextures.Count,
                    frameCount = frames,
                    width = size.Width,
                    height = size.Height,
                };
                indices.Add(ri);
                for (int frame = 0; frame < frames; frame++)
                {
                    settings.frame = frame;
                    GetTextureResults nextTextureResults = GetTexture2D(settings);
                    albedoTextures.Add(nextTextureResults.albedoMap);
                    if (nextTextureResults.normalMap != null)
                    {
                        normalTextures.Add(nextTextureResults.normalMap);
                        hasNormalMaps = true;
                    }
                    if (nextTextureResults.emissionMap != null)
                    {
                        emissionTextures.Add(nextTextureResults.emissionMap);
                        hasEmissionMaps = true;
                    }
                }

                results.atlasSizes.Add(new Vector2(size.Width, size.Height));
                results.atlasScales.Add(new Vector2(scale.Width, scale.Height));
                results.atlasOffsets.Add(new Vector2(offset.Width, offset.Height));
                results.atlasFrameCounts.Add(frames);
            }

            // Pack albedo textures into atlas and get our rects
            Texture2D atlasAlbedoMap = new Texture2D(settings.atlasMaxSize, settings.atlasMaxSize, TextureFormat.RGBA32, MipMaps);
            Rect[] rects = atlasAlbedoMap.PackTextures(albedoTextures.ToArray(), settings.atlasPadding, settings.atlasMaxSize, !stayReadable);

            // Pack normal textures into atlas
            Texture2D atlasNormalMap = null;
            if (hasNormalMaps)
            {
                // Normals must be ARGB32
                atlasNormalMap = new Texture2D(settings.atlasMaxSize, settings.atlasMaxSize, TextureFormat.ARGB32, MipMaps);
                atlasNormalMap.PackTextures(normalTextures.ToArray(), settings.atlasPadding, settings.atlasMaxSize, !stayReadable);
            }

            // Pack emission textures into atlas
            Texture2D atlasEmissionMap = null;
            if (hasEmissionMaps)
            {
                // Repacking to ensure correct mix of lit and unlit
                atlasEmissionMap = new Texture2D(settings.atlasMaxSize, settings.atlasMaxSize, TextureFormat.RGBA32, MipMaps);
                atlasEmissionMap.PackTextures(emissionTextures.ToArray(), settings.atlasPadding, settings.atlasMaxSize, !stayReadable);
            }

            // Add to results
            if (results.atlasRects == null) results.atlasRects = new List<Rect>(rects.Length);
            if (results.atlasIndices == null) results.atlasIndices = new List<RecordIndex>(indices.Count);
            results.atlasRects.AddRange(rects);
            results.atlasIndices.AddRange(indices);

            // Shrink UV rect to compensate for internal border
            float ru = 1f / atlasAlbedoMap.width;
            float rv = 1f / atlasAlbedoMap.height;
            int finalBorder = settings.borderSize + settings.atlasShrinkUVs;
            for (int i = 0; i < results.atlasRects.Count; i++)
            {
                Rect rct = results.atlasRects[i];
                rct.xMin += finalBorder * ru;
                rct.xMax -= finalBorder * ru;
                rct.yMin += finalBorder * rv;
                rct.yMax -= finalBorder * rv;
                results.atlasRects[i] = rct;
            }

            // Store results
            results.albedoMap = atlasAlbedoMap;
            results.normalMap = atlasNormalMap;
            results.emissionMap = atlasEmissionMap;
            results.isAtlasAnimated = hasAnimation;
            results.isEmissive = hasEmissionMaps;

            return results;
        }

        /// <summary>
        /// TEMP: Creates super-atlas populated with all archives in array.
        /// Currently does not support animated textures, normal map, or emission map.
        /// TODO: Integrate this feature fully with material system.
        /// </summary>
        /// <param name="archives">Archive array.</param>
        /// <param name="borderSize">Number of pixels border to add around image.</param>
        /// <param name="dilate">Blend texture into surrounding empty pixels. Requires border.</param>
        /// <returns>TextureAtlasBuilder.</returns>
        public TextureAtlasBuilder CreateTextureAtlasBuilder(int[] archives, int borderSize = 0, bool dilate = false, int maxAtlasSize = 2048)
        {
            // Ready check
            if (!ReadyCheck())
                return null;

            // Iterate archives
            TextureAtlasBuilder builder = new TextureAtlasBuilder();
            GetTextureSettings settings = TextureReader.CreateTextureSettings(0, 0, 0, 0, borderSize, dilate, maxAtlasSize);
            settings.stayReadable = true;
            for (int i = 0; i < archives.Length; i++)
            {
                // Load texture file
                settings.archive = archives[i];
                textureFile.Load(Path.Combine(Arena2Path, TextureFile.IndexToFileName(settings.archive)), FileUsage.UseMemory, true);

                // Add all records for this archive - single frame only
                for (int record = 0; record < textureFile.RecordCount; record++)
                {
                    settings.record = record;
                    GetTextureResults results = GetTexture2D(settings);
                    DFSize size = textureFile.GetSize(record);
                    DFSize scale = textureFile.GetScale(record);
                    builder.AddTextureItem(
                        results.albedoMap,
                        settings.archive,
                        settings.record,
                        0, 1,
                        new Vector2(size.Width, size.Height),
                        new Vector2(scale.Width, scale.Height));
                }
            }

            // Apply the builder
            builder.Rebuild(borderSize);
            
            return builder;
        }

        /// <summary>
        /// Gets specially packed tileset atlas for terrains.
        /// This needs to be improved to create each mip level manually to further reduce artifacts.
        /// </summary>
        public GetTextureResults GetTerrainTilesetTexture(int archive, bool stayReadable = false)
        {
            const int atlasDim = 2048;
            const int gutterSize = 32;

            GetTextureResults results = new GetTextureResults();

            // Ready check
            if (!ReadyCheck())
                return results;

            // Load texture file and check count matches terrain tiles
            textureFile.Load(Path.Combine(Arena2Path, TextureFile.IndexToFileName(archive)), FileUsage.UseMemory, true);
            if (textureFile.RecordCount != 56)
                return results;

            // Rollout tiles into atlas.
            // This is somewhat inefficient, but fortunately atlases only
            // need to be generated once and can be prepared offline where
            // startup time is critical.
            int x = 0, y = 0;
            Color32[] atlasColors = new Color32[atlasDim * atlasDim];
            for (int record = 0; record < textureFile.RecordCount; record++)
            {
                // Create base image with gutter
                DFSize sz;
                Color32[] albedo = textureFile.GetColors32(record, 0, -1, gutterSize, out sz);

                // Wrap and clamp textures based on tile
                switch (record)
                {
                    // Textures that do not tile in any direction
                    case 5:
                    case 7:
                    case 10:
                    case 12:
                    case 15:
                    case 17:
                    case 20:
                    case 22:
                    case 25:
                    case 27:
                    case 30:
                    case 32:
                    case 34:
                    case 35:
                    case 36:
                    case 37:
                    case 38:
                    case 39:
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                    case 47:
                    case 48:
                    case 49:
                    case 50:
                    case 51:
                    case 52:
                    case 53:
                    case 55:
                        ImageProcessing.ClampBorder(ref albedo, sz, gutterSize);
                        break;

                    // Textures that clamp horizontally and tile vertically
                    case 6:
                    case 11:
                    case 16:
                    case 21:
                    case 26:
                    case 31:
                        ImageProcessing.WrapBorder(ref albedo, sz, gutterSize, false);
                        ImageProcessing.ClampBorder(ref albedo, sz, gutterSize, true, false);
                        break;

                    // Textures that tile in all directions
                    default:
                        ImageProcessing.WrapBorder(ref albedo, sz, gutterSize);
                        break;
                }

                // Create variants
                Color32[] rotate = ImageProcessing.RotateColors(ref albedo, sz.Width, sz.Height);
                Color32[] flip = ImageProcessing.FlipColors(ref albedo, sz.Width, sz.Height);
                Color32[] rotateFlip = ImageProcessing.RotateColors(ref flip, sz.Width, sz.Height);

                // Insert into atlas
                ImageProcessing.InsertColors(ref albedo, ref atlasColors, x, y, sz.Width, sz.Height, atlasDim, atlasDim);
                ImageProcessing.InsertColors(ref rotate, ref atlasColors, x + sz.Width, y, sz.Width, sz.Height, atlasDim, atlasDim);
                ImageProcessing.InsertColors(ref flip, ref atlasColors, x + sz.Width * 2, y, sz.Width, sz.Height, atlasDim, atlasDim);
                ImageProcessing.InsertColors(ref rotateFlip, ref atlasColors, x + sz.Width * 3, y, sz.Width, sz.Height, atlasDim, atlasDim);

                // Increment position
                x += sz.Width * 4;
                if (x >= atlasDim)
                {
                    y += sz.Height;
                    x = 0;
                }
            }

            // Create Texture2D
            Texture2D albedoAtlas = new Texture2D(atlasDim, atlasDim, TextureFormat.RGB24, MipMaps);
            albedoAtlas.SetPixels32(atlasColors);
            albedoAtlas.Apply(true, !stayReadable);

            // Change settings for these textures
            albedoAtlas.wrapMode = TextureWrapMode.Clamp;
            albedoAtlas.anisoLevel = 8;

            // Store results
            results.albedoMap = albedoAtlas;

            return results;
        }

        #region Helpers

        // Textures that should receive emission map
        // TODO: This should be set by a user-managed list somewhere
        DaggerfallTextureIndex[] emissiveTextures = new DaggerfallTextureIndex[]
        {
            // Mantellan Crux fire textures
            new DaggerfallTextureIndex() { archive = 356, record = 0 },
            new DaggerfallTextureIndex() { archive = 356, record = 2 },
            new DaggerfallTextureIndex() { archive = 356, record = 3 },

            // Fireplace
            new DaggerfallTextureIndex() { archive = 87, record = 0 },

            // Lights (which are on/lit)
            new DaggerfallTextureIndex() { archive = 210, record = 0 },
            new DaggerfallTextureIndex() { archive = 210, record = 1 },
            new DaggerfallTextureIndex() { archive = 210, record = 2 },
            new DaggerfallTextureIndex() { archive = 210, record = 3 },
            new DaggerfallTextureIndex() { archive = 210, record = 4 },
            new DaggerfallTextureIndex() { archive = 210, record = 5 },
            new DaggerfallTextureIndex() { archive = 210, record = 6 },
            new DaggerfallTextureIndex() { archive = 210, record = 8 },
            new DaggerfallTextureIndex() { archive = 210, record = 9 },
            new DaggerfallTextureIndex() { archive = 210, record = 11 },
            new DaggerfallTextureIndex() { archive = 210, record = 13 },
            new DaggerfallTextureIndex() { archive = 210, record = 14 },
            new DaggerfallTextureIndex() { archive = 210, record = 15 },
            new DaggerfallTextureIndex() { archive = 210, record = 16 },
            new DaggerfallTextureIndex() { archive = 210, record = 17 },
            new DaggerfallTextureIndex() { archive = 210, record = 18 },
            new DaggerfallTextureIndex() { archive = 210, record = 19 },
            new DaggerfallTextureIndex() { archive = 210, record = 20 },
            new DaggerfallTextureIndex() { archive = 210, record = 21 },
            new DaggerfallTextureIndex() { archive = 210, record = 22 },
            new DaggerfallTextureIndex() { archive = 210, record = 23 },
            new DaggerfallTextureIndex() { archive = 210, record = 24 },
            new DaggerfallTextureIndex() { archive = 210, record = 25 },
            new DaggerfallTextureIndex() { archive = 210, record = 26 },
            new DaggerfallTextureIndex() { archive = 210, record = 27 },
            new DaggerfallTextureIndex() { archive = 210, record = 28 },
            new DaggerfallTextureIndex() { archive = 210, record = 29 },
        };

        public bool IsEmissive(int archive, int record)
        {
            // Check emissive list for this texture
            // TODO: Replace this with a dictionary/hash lookup
            bool emissiveFound = false;
            for (int i = 0; i < emissiveTextures.Length; i++)
            {
                if (emissiveTextures[i].archive == archive && emissiveTextures[i].record == record)
                {
                    emissiveFound = true;
                    break;
                }
            }

            return emissiveFound;
        }

#if UNITY_EDITOR && !UNITY_WEBPLAYER
        public void SaveTextureToPNG(Texture2D source, string path)
        {
            byte[] bytes = source.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
        }
#endif

        #endregion

        #region Private Methods

        private bool ReadyCheck()
        {
            // Arena2 path must be set
            if (string.IsNullOrEmpty(Arena2Path))
            {
                DaggerfallUnity.LogMessage("TextureReader: Arena2Path not set.", true);
                return false;
            }

            // Ensure texture reader is ready
            if (textureFile == null)
            {
                textureFile = new TextureFile();
                if (!textureFile.Palette.Load(Path.Combine(Arena2Path, textureFile.PaletteName)))
                {
                    DaggerfallUnity.LogMessage("TextureReader: Failed to load palette file, is Arena2Path correct?", true);
                    textureFile = null;
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}