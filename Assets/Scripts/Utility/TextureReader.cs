// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Helper to load Daggerfall TEXTURE.xxx files as Texture2D.
    /// </summary>
    public class TextureReader
    {
        const int spectralEyesReserved = 14;
        const int spectralEyesPatched = 247;
        const int spectralGrayStart = 96;

        bool mipMaps = true;

        // Special texture indices
        public const int EditorFlatsTextureArchive = 199;
        public const int AnimalsTextureArchive = 201;
        public const int LightsTextureArchive = 210;
        public const int FixedTreasureFlatsArchive = 216;
        public const int FireWallsArchive = 356;
        //public int[] MiscFlatsTextureArchives = new int[] { 97, 205, 211, 212, 213, 301 };

        /// <summary>
        /// Gets or sets Arena2 path.
        /// Path must be set before attempting to load textures.
        /// </summary>
        public string Arena2Path { get; set; }

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
        public static GetTextureSettings CreateTextureSettings(
            int archive = 0,
            int record = 0,
            int frame = 0,
            int alphaIndex = 0,
            int borderSize = 0,
            bool dilate = false,
            int maxAtlasSize = 2048)
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
        /// Creates a simple texture from any base API image.
        /// </summary>
        public static Texture2D CreateFromAPIImage(BaseImageFile image, int record = 0, int frame = 0, int alphaIndex = -1, bool createMipMaps = false, bool makeNoLongerReadable = true)
        {
            // Override readable flag when user has set preference in material reader
            if (DaggerfallUnity.Instance.MaterialReader.ReadableTextures)
            {
                makeNoLongerReadable = false;
            }

            // Disable mipmaps in retro mode
            if (DaggerfallUnity.Settings.RetroRenderingMode > 0 && !DaggerfallUnity.Settings.UseMipMapsInRetroMode)
                createMipMaps = false;

            Texture2D texture;
            if (TextureReplacement.TryImportImage(image.FileName, makeNoLongerReadable, out texture))
                return texture;

            if (TextureReplacement.TryImportCifRci(image.FileName, record, frame, makeNoLongerReadable, out texture))
                return texture;

            DFSize sz;
            Color32[] colors = image.GetColor32(record, frame, alphaIndex, 0, out sz);
            texture = new Texture2D(sz.Width, sz.Height, TextureFormat.ARGB32, createMipMaps);
            texture.SetPixels32(colors);
            texture.Apply(createMipMaps, makeNoLongerReadable);
            return texture;
        }

        /// <summary>
        /// Creates a simple solid-colour texture.
        /// </summary>
        public static Texture2D CreateFromSolidColor(int width, int height, Color color, bool createMipMaps = false, bool makeNoLongerReadable = true)
        {
            // Override readable flag when user has set preference in material reader
            if (DaggerfallUnity.Instance.MaterialReader.ReadableTextures)
            {
                makeNoLongerReadable = false;
            }

            // Disable mipmaps in retro mode
            if (DaggerfallUnity.Settings.RetroRenderingMode > 0 && !DaggerfallUnity.Settings.UseMipMapsInRetroMode)
                createMipMaps = false;

            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, createMipMaps);
            Color32[] colors = new Color32[width * height];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = color;
            }
            texture.SetPixels32(colors);
            texture.Apply(createMipMaps, makeNoLongerReadable);

            return texture;
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        static TextureReader()
        {
            FastEmissiveTexturesInit();
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
        public Texture2D GetTexture2D(
            int archive,
            int record,
            int frame = 0,
            int alphaIndex = 0)
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
        /// <param name="alphaTextureFormat">Alpha TextureFormat.</param>
        /// <param name="textureImport">Options for import of custom textures.</param>
        /// <returns>GetTextureResults.</returns>
        public GetTextureResults GetTexture2D(
            GetTextureSettings settings,
            SupportedAlphaTextureFormats alphaTextureFormat = SupportedAlphaTextureFormats.ARGB32,
            TextureImport textureImport = TextureImport.None)
        {
            GetTextureResults results = new GetTextureResults();

            // Check if window, spectral, or auto-emissive
            bool isWindow = ClimateSwaps.IsExteriorWindow(settings.archive, settings.record);
            bool isSpectral = TextureFile.IsSpectralArchive(settings.archive);
            bool isEmissive = (settings.autoEmission) ? IsEmissive(settings.archive, settings.record) : false;

            // Override readable flag when user has set preference in material reader
            if (DaggerfallUnity.Instance.MaterialReader.ReadableTextures)
            {
                settings.stayReadable = true;
            }

            // Disable mipmaps in retro mode
            if (DaggerfallUnity.Settings.RetroRenderingMode > 0 && !DaggerfallUnity.Settings.UseMipMapsInRetroMode)
                mipMaps = false;

            bool hasReferenceTexture;

            // Assign texture file
            TextureFile textureFile;
            if (settings.textureFile == null)
            {
                textureFile = new TextureFile();
                hasReferenceTexture = textureFile.Load(Path.Combine(Arena2Path, TextureFile.IndexToFileName(settings.archive)), FileUsage.UseMemory, true);
            }
            else
            {
                textureFile = settings.textureFile;
                hasReferenceTexture = true;
            }

            // Get starting DFBitmap
            DFSize sz = null;
            DFBitmap srcBitmap = hasReferenceTexture ? textureFile.GetDFBitmap(settings.record, settings.frame) : null;

            // Get albedo Color32 array
            Color32[] albedoColors = null;

            if (hasReferenceTexture)
            {                
                if (isSpectral)
                {
                    // Adjust source bitmap to set spectral grays
                    // 180 = transparency amount (~70% visible)
                    SetSpectral(ref srcBitmap);
                    albedoColors = textureFile.GetColor32(srcBitmap, settings.alphaIndex, settings.borderSize, out sz, spectralEyesPatched, 180);
                }
                else
                {
                    // Read direct from source bitmap
                    albedoColors = textureFile.GetColor32(srcBitmap, settings.alphaIndex, settings.borderSize, out sz);
                }

                // Sharpen source image
                if (settings.sharpen)
                    albedoColors = ImageProcessing.Sharpen(ref albedoColors, sz.Width, sz.Height);

                // Dilate edges
                if (settings.borderSize > 0 && settings.dilate && !settings.copyToOppositeBorder)
                    ImageProcessing.DilateColors(ref albedoColors, sz);

                // Copy to opposite border
                if (settings.borderSize > 0 && settings.copyToOppositeBorder)
                    ImageProcessing.WrapBorder(ref albedoColors, sz, settings.borderSize);
            }

            // Set albedo texture
            Texture2D albedoMap;
            bool albedoImported = TextureReplacement.TryImportTexture(settings.archive, settings.record, settings.frame, TextureMap.Albedo, textureImport, !settings.stayReadable, out albedoMap);
            if (!albedoImported && hasReferenceTexture)
            {
                // Create albedo texture
                albedoMap = new Texture2D(sz.Width, sz.Height, ParseTextureFormat(alphaTextureFormat), MipMaps);
                albedoMap.SetPixels32(albedoColors);
                albedoMap.Apply(true, !settings.stayReadable);
            }

            // Adjust mipmap bias of albedo map when retro mode rendering is enabled
            if (albedoMap && DaggerfallUnity.Settings.RetroRenderingMode > 0)
            {
                albedoMap.mipMapBias = -0.75f;
            }

            // Set normal texture (always import normal if present on disk)
            Texture2D normalMap = null;
            bool normalMapImported = TextureReplacement.TryImportTexture(settings.archive, settings.record, settings.frame, TextureMap.Normal, textureImport, !settings.stayReadable, out normalMap);
            if (!normalMapImported && settings.createNormalMap && hasReferenceTexture && textureFile.SolidType == TextureFile.SolidTypes.None)
            {
                // Create normal texture - must be ARGB32
                // Normal maps are bypassed for solid-colour textures
                Color32[] normalColors;
                normalColors = ImageProcessing.GetBumpMap(ref albedoColors, sz.Width, sz.Height);
                normalColors = ImageProcessing.ConvertBumpToNormals(ref normalColors, sz.Width, sz.Height, settings.normalStrength);
                normalMap = new Texture2D(sz.Width, sz.Height, TextureFormat.ARGB32, MipMaps);
                normalMap.SetPixels32(normalColors);
                normalMap.Apply(true, !settings.stayReadable);
            }

            // Import emission map or create basic emissive texture
            Texture2D emissionMap = null;
            bool resultEmissive = false;
            if (TextureReplacement.TryImportTexture(settings.archive, settings.record, settings.frame, TextureMap.Emission, textureImport, !settings.stayReadable, out emissionMap))
            {
                // Always import emission if present on disk
                resultEmissive = true;
            }
            else
            {
                if (settings.createEmissionMap || (settings.autoEmission && isEmissive) && !isWindow)
                {
                    if (settings.archive != FireWallsArchive)
                    {
                        // Just reuse albedo map for basic colour emission
                        emissionMap = albedoMap;
                        resultEmissive = true;
                    }
                    else if (hasReferenceTexture)
                    {
                        // Mantellan Crux fire walls - lessen stroboscopic effect
                        Color fireColor = new Color(0.706f, 0.271f, 0.086f); // Average of dim frame (0)
                        // Color fireColor = new Color(0.773f, 0.38f, 0.122f); // Average of average frames (1 and 3)
                        // Color fireColor = new Color(0.851f, 0.506f, 0.165f); // Average of bright frame (2)
                        Color32[] firewallEmissionColors = textureFile.GetFireWallColors32(ref albedoColors, sz.Width, sz.Height, fireColor, 0.3f);
                        emissionMap = new Texture2D(sz.Width, sz.Height, ParseTextureFormat(alphaTextureFormat), MipMaps);
                        emissionMap.SetPixels32(firewallEmissionColors);
                        emissionMap.Apply(true, !settings.stayReadable);
                        resultEmissive = true;
                    }                    
                }

                // Windows need special handling as only glass parts are emissive
                if ((settings.createEmissionMap || settings.autoEmissionForWindows) && isWindow && hasReferenceTexture)
                {
                    // Create custom emission texture for glass area of windows
                    Color32[] emissionColors = textureFile.GetWindowColors32(srcBitmap);
                    emissionMap = new Texture2D(sz.Width, sz.Height, ParseTextureFormat(alphaTextureFormat), MipMaps);
                    emissionMap.SetPixels32(emissionColors);
                    emissionMap.Apply(true, !settings.stayReadable);
                    resultEmissive = true;
                }

                // Spectral need special handling as only eye parts are emissive
                if ((settings.createEmissionMap || settings.autoEmission) && isSpectral && hasReferenceTexture)
                {
                    Color eyeEmission = Color.red;
                    Color bodyEmission = Color.black;// new Color(0.1f, 0.1f, 0.1f);
                    Color32[] emissionColors = textureFile.GetSpectralEmissionColors32(srcBitmap, albedoColors, settings.borderSize, spectralEyesPatched, eyeEmission, bodyEmission);
                    emissionMap = new Texture2D(albedoMap.width, albedoMap.height, ParseTextureFormat(alphaTextureFormat), MipMaps);
                    emissionMap.SetPixels32(emissionColors);
                    emissionMap.Apply(true, !settings.stayReadable);
                    resultEmissive = true;
                }

                // Some archives need special handling as they contains a mix of emissive and non-emissive flats
                // This can cause problems with atlas packing due to mismatch between albedo and emissive texture counts
                if ((settings.createEmissionMap || settings.autoEmission) && IsEmissiveArchive(settings.archive))
                {
                    // For the unlit flats we create a null-emissive black texture
                    if (!resultEmissive)
                    {
                        Color32[] emissionColors = new Color32[albedoMap.width * albedoMap.height];
                        emissionMap = new Texture2D(albedoMap.width, albedoMap.height, ParseTextureFormat(alphaTextureFormat), MipMaps);
                        emissionMap.SetPixels32(emissionColors);
                        emissionMap.Apply(true, !settings.stayReadable);
                        resultEmissive = true;
                    }
                }
            }

            // Shrink UV rect to compensate for internal border
            if (hasReferenceTexture)
            {
                float ru = 1f / sz.Width;
                float rv = 1f / sz.Height;
                results.singleRect = new Rect(
                    settings.borderSize * ru,
                    settings.borderSize * rv,
                    (sz.Width - settings.borderSize * 2) * ru,
                    (sz.Height - settings.borderSize * 2) * rv);
            }
            else
            {
                // Use the albedo's sizes as a fallback
                float ru = 1f / albedoMap.width;
                float rv = 1f / albedoMap.height;
                results.singleRect = new Rect(
                    settings.borderSize * ru,
                    settings.borderSize * rv,
                    (albedoMap.width - settings.borderSize * 2) * ru,
                    (albedoMap.height - settings.borderSize * 2) * rv);
            }

            // Store results
            results.albedoMap = albedoMap;
            results.normalMap = normalMap;
            results.emissionMap = emissionMap;
            results.isWindow = isWindow;
            results.isEmissive = resultEmissive;
            results.textureFile = hasReferenceTexture ? textureFile : null;

            return results;
        }

        /// <summary>
        /// Updates spectral color indices to set eyes red and accentuate other features
        /// </summary>
        /// <param name="srcBitmap"></param>
        public void SetSpectral(ref DFBitmap srcBitmap)
        {
            for (int i = 0; i < srcBitmap.Data.Length; i++)
            {
                byte index = srcBitmap.Data[i];

                // Index 14 is reserved for emissive eyes, patching this to 112 (white color index) for best interaction with shader
                // Other colours are reindexed to a grey gradiant area of palette so that bones are brighter than shroud
                if (index == spectralEyesReserved)
                    srcBitmap.Data[i] = spectralEyesPatched;
                else if (index > 0)
                    srcBitmap.Data[i] = (byte)(spectralGrayStart - index);
            }
        }

        /// <summary>
        /// Gets Texture2D atlas from Daggerfall texture archive.
        /// Every record and frame in the archive will be added to atlas.
        /// An array of rects will be returned with sub-texture rect for each record index and frame.
        /// Currently supports one archive per atlas. Super-atlas packing (multiple archives) is in the works.
        /// </summary>
        /// <param name="settings">Get texture settings.</param>
        /// <param name="alphaTextureFormat">Alpha TextureFormat.</param>
        /// <returns>GetTextureResults.</returns>
        public GetTextureResults GetTexture2DAtlas(
            GetTextureSettings settings,
            SupportedAlphaTextureFormats alphaTextureFormat = SupportedAlphaTextureFormats.ARGB32)
        {
            GetTextureResults results = new GetTextureResults();

            // Individual textures must remain readable to pack into atlas
            bool stayReadable = settings.stayReadable;
            settings.stayReadable = true;

            // Assign texture file
            TextureFile textureFile;
            if (settings.textureFile == null)
            {
                textureFile = new TextureFile(Path.Combine(Arena2Path, TextureFile.IndexToFileName(settings.archive)), FileUsage.UseMemory, true);
                settings.textureFile = textureFile;
            }
            else
                textureFile = settings.textureFile;

            // Create lists
            results.atlasSizes = new List<Vector2>(textureFile.RecordCount);
            results.atlasScales = new List<Vector2>(textureFile.RecordCount);
            results.atlasOffsets = new List<Vector2>(textureFile.RecordCount);
            results.atlasFrameCounts = new List<int>(textureFile.RecordCount);

            // Read every texture in archive
            bool hasNormalMaps = false;
            bool hasEmissionMaps = false;
            bool hasAnimation = false;
            var textureImport = settings.atlasMaxSize == 4096 ? TextureImport.AllLocations : TextureImport.None;
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

                // Disable mipmaps in retro mode
                if (DaggerfallUnity.Settings.RetroRenderingMode > 0 && !DaggerfallUnity.Settings.UseMipMapsInRetroMode)
                    mipMaps = false;

                // Get record information
                DFSize size = textureFile.GetSize(record);
                DFSize scale = textureFile.GetScale(record);
                DFPosition offset = textureFile.GetOffset(record);
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
                    GetTextureResults nextTextureResults = GetTexture2D(settings, alphaTextureFormat, textureImport);

                    albedoTextures.Add(nextTextureResults.albedoMap);
                    if (nextTextureResults.normalMap != null)
                    {
                        if (nextTextureResults.normalMap.width != nextTextureResults.albedoMap.width || nextTextureResults.normalMap.height != nextTextureResults.albedoMap.height)
                        {
                            Debug.LogErrorFormat("The size of atlased normal map for {0}-{1} must be equal to the size of main texture.", settings.archive, settings.record);
                            nextTextureResults.normalMap = nextTextureResults.albedoMap;
                        }

                        normalTextures.Add(nextTextureResults.normalMap);
                        hasNormalMaps = true;
                    }
                    if (nextTextureResults.emissionMap != null)
                    {
                        if (nextTextureResults.emissionMap.width != nextTextureResults.albedoMap.width || nextTextureResults.emissionMap.height != nextTextureResults.albedoMap.height)
                        {
                            Debug.LogErrorFormat("The size of atlased emission map for {0}-{1} must be equal to the size of main texture.", settings.archive, settings.record);
                            nextTextureResults.emissionMap = nextTextureResults.albedoMap;
                        }

                        emissionTextures.Add(nextTextureResults.emissionMap);
                        hasEmissionMaps = true;
                    }
                }

                results.atlasSizes.Add(new Vector2(size.Width, size.Height));
                results.atlasScales.Add(new Vector2(scale.Width, scale.Height));
                results.atlasOffsets.Add(new Vector2(offset.X, offset.Y));
                results.atlasFrameCounts.Add(frames);
                results.textureFile = textureFile;
            }

            // Pack albedo textures into atlas and get our rects
            Texture2D atlasAlbedoMap = new Texture2D(settings.atlasMaxSize, settings.atlasMaxSize, ParseTextureFormat(alphaTextureFormat), MipMaps);
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
            // TODO: Change this as packing not consistent
            Texture2D atlasEmissionMap = null;
            if (hasEmissionMaps)
            {
                // Repacking to ensure correct mix of lit and unlit
                atlasEmissionMap = new Texture2D(settings.atlasMaxSize, settings.atlasMaxSize, ParseTextureFormat(alphaTextureFormat), MipMaps);
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

        ///// <summary>
        ///// TEMP: Creates super-atlas populated with all archives in array.
        ///// Currently does not support animated textures, normal map, or emission map.
        ///// TODO: Integrate this feature fully with material system.
        ///// </summary>
        ///// <param name="archives">Archive array.</param>
        ///// <param name="borderSize">Number of pixels border to add around image.</param>
        ///// <param name="dilate">Blend texture into surrounding empty pixels. Requires border.</param>
        ///// <param name="maxAtlasSize">Maximum atlas size.</param>
        ///// <param name="alphaTextureFormat">Alpha TextureFormat.</param>
        ///// <param name="nonAlphaFormat">Non-alpha TextureFormat.</param>
        ///// <returns>TextureAtlasBuilder.</returns>
        //public TextureAtlasBuilder CreateTextureAtlasBuilder(
        //    int[] archives,
        //    int borderSize = 0,
        //    bool dilate = false,
        //    int maxAtlasSize = 2048,
        //    SupportedAlphaTextureFormats alphaTextureFormat = SupportedAlphaTextureFormats.ARGB32,
        //    SupportedNonAlphaTextureFormats nonAlphaFormat = SupportedNonAlphaTextureFormats.RGB24)
        //{
        //    // Iterate archives
        //    TextureFile textureFile = new TextureFile();
        //    TextureAtlasBuilder builder = new TextureAtlasBuilder();
        //    GetTextureSettings settings = TextureReader.CreateTextureSettings(0, 0, 0, 0, borderSize, dilate, maxAtlasSize);
        //    settings.stayReadable = true;
        //    for (int i = 0; i < archives.Length; i++)
        //    {
        //        // Load texture file
        //        settings.archive = archives[i];
        //        textureFile.Load(Path.Combine(Arena2Path, TextureFile.IndexToFileName(settings.archive)), FileUsage.UseMemory, true);

        //        // Add all records for this archive - single frame only
        //        for (int record = 0; record < textureFile.RecordCount; record++)
        //        {
        //            settings.record = record;
        //            GetTextureResults results = GetTexture2D(settings, alphaTextureFormat, nonAlphaFormat);
        //            DFSize size = textureFile.GetSize(record);
        //            DFSize scale = textureFile.GetScale(record);
        //            builder.AddTextureItem(
        //                results.albedoMap,
        //                settings.archive,
        //                settings.record,
        //                0, 1,
        //                new Vector2(size.Width, size.Height),
        //                new Vector2(scale.Width, scale.Height));
        //        }
        //    }

        //    // Apply the builder
        //    builder.Rebuild(borderSize);
            
        //    return builder;
        //}

        /// <summary>
        /// Gets specially packed tileset atlas for terrains.
        /// This needs to be improved to create each mip level manually to further reduce artifacts.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="stayReadable">Texture should stay readable.</param>
        /// <returns></returns>
        public GetTextureResults GetTerrainTilesetTexture(
            int archive,
            bool stayReadable = false)
        {
            const int atlasDim = 2048;
            const int gutterSize = 32;

            GetTextureResults results = new GetTextureResults();

            // Load texture file and check count matches terrain tiles
            TextureFile textureFile = new TextureFile(Path.Combine(Arena2Path, TextureFile.IndexToFileName(archive)), FileUsage.UseMemory, true);
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
                Color32[] albedo = textureFile.GetColor32(record, 0, -1, gutterSize, out sz);

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
            Texture2D albedoAtlas = new Texture2D(atlasDim, atlasDim, TextureFormat.ARGB32, MipMaps);
            albedoAtlas.SetPixels32(atlasColors);
            albedoAtlas.Apply(true, !stayReadable);

            // Change settings for these textures
            albedoAtlas.wrapMode = TextureWrapMode.Clamp;
            albedoAtlas.anisoLevel = 8;

            // Store results
            results.albedoMap = albedoAtlas;

            return results;
        }

        /// <summary>
        /// Gets terrain texture array containing each terrain tile texture in a seperate array slice.        
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="textureMap">The texture type.</param>
        /// <returns>Texture2DArray or null.</returns>
        public Texture2DArray GetTerrainTextureArray(int archive, TextureMap textureMap)
        {
            // Load texture file and check count matches terrain tiles
            TextureFile textureFile = new TextureFile(Path.Combine(Arena2Path, TextureFile.IndexToFileName(archive)), FileUsage.UseMemory, true);
            int numSlices = 0;
            if (textureFile.RecordCount == 56)
                numSlices = textureFile.RecordCount;
            else
                return null;

            // Try to import whole texture array
            Texture2DArray textureArray;
            if (TextureReplacement.TryImportTextureArray(archive, numSlices, textureMap, null, out textureArray))
                return textureArray;

            // Only use Color32 fallback loader for albedo texture
            if (textureMap != TextureMap.Albedo)
                return null;

            // Try to import first replacement texture for tile archive to determine width and height of replacement texture set (must be the same for all replacement textures for Texture2DArray)
            Texture2D texture;
            if (TextureReplacement.TryImportTexture(archive, 0, 0, out texture))
                textureArray = new Texture2DArray(texture.width, texture.height, numSlices, TextureFormat.ARGB32, MipMaps, TextureReplacement.IsLinearTextureMap(textureMap));
            else
                textureArray = new Texture2DArray(textureFile.GetWidth(0), textureFile.GetHeight(0), numSlices, TextureFormat.ARGB32, MipMaps, TextureReplacement.IsLinearTextureMap(textureMap));

            // Rollout tiles into texture array
            for (int record = 0; record < textureFile.RecordCount; record++)
            {
                DFSize sz;
                Color32[] colors;
                if (TextureReplacement.TryImportTexture(archive, record, 0, out texture))
                    colors = texture.GetPixels32();                                 // Import custom texture
                else
                    colors = textureFile.GetColor32(record, 0, -1, 0, out sz);      // Create base image with gutter

                // Insert into texture array
                textureArray.SetPixels32(colors, record, 0);
            }
            textureArray.Apply(true);

            // Change settings for these textures
            textureArray.wrapMode = TextureWrapMode.Clamp;
            textureArray.anisoLevel = 8;

            return textureArray;
        }

        #region Helpers

        // Textures that should receive emission map
        // TODO: Consider setting this from an external list
        static DaggerfallTextureIndex[] emissiveTextures = new DaggerfallTextureIndex[]
        {
            // Fireplace
            new DaggerfallTextureIndex() { archive = 87, record = 0 },

            // Lights (which are on/lit)
            new DaggerfallTextureIndex() { archive = 101, record = 2 },
            new DaggerfallTextureIndex() { archive = 101, record = 3 },
            new DaggerfallTextureIndex() { archive = 101, record = 5 },
            new DaggerfallTextureIndex() { archive = 101, record = 6 },
            new DaggerfallTextureIndex() { archive = 101, record = 7 },
            new DaggerfallTextureIndex() { archive = 101, record = 8 },
            new DaggerfallTextureIndex() { archive = 101, record = 9 },
            // new DaggerfallTextureIndex() { archive = 101, record = 10 }, // is glass globe a light source?
            new DaggerfallTextureIndex() { archive = 101, record = 11 },
            new DaggerfallTextureIndex() { archive = 101, record = 12 },
            new DaggerfallTextureIndex() { archive = 190, record = 3 },
            new DaggerfallTextureIndex() { archive = 190, record = 4 },
            new DaggerfallTextureIndex() { archive = 190, record = 5 },
            new DaggerfallTextureIndex() { archive = 200, record = 7 },
            new DaggerfallTextureIndex() { archive = 200, record = 8 },
            new DaggerfallTextureIndex() { archive = 200, record = 9 },
            new DaggerfallTextureIndex() { archive = 200, record = 10 },

            // Statue
            new DaggerfallTextureIndex() { archive = 202, record = 2 },

            // Brewing potion
            new DaggerfallTextureIndex() { archive = 208, record = 2 },

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

            new DaggerfallTextureIndex() { archive = 253, record = 10 },
            new DaggerfallTextureIndex() { archive = 253, record = 17 },
            new DaggerfallTextureIndex() { archive = 253, record = 18 },
            new DaggerfallTextureIndex() { archive = 253, record = 19 },
            new DaggerfallTextureIndex() { archive = 253, record = 22 },
            new DaggerfallTextureIndex() { archive = 253, record = 41 },
            new DaggerfallTextureIndex() { archive = 253, record = 48 },
            new DaggerfallTextureIndex() { archive = 253, record = 49 },
            new DaggerfallTextureIndex() { archive = 253, record = 50 },
            new DaggerfallTextureIndex() { archive = 253, record = 51 },
            new DaggerfallTextureIndex() { archive = 253, record = 52 },
            new DaggerfallTextureIndex() { archive = 253, record = 75 },
            new DaggerfallTextureIndex() { archive = 253, record = 77 },

            // Ghost
            new DaggerfallTextureIndex() { archive = 273, record = 0 },
            new DaggerfallTextureIndex() { archive = 273, record = 1 },
            new DaggerfallTextureIndex() { archive = 273, record = 2 },
            new DaggerfallTextureIndex() { archive = 273, record = 3 },
            new DaggerfallTextureIndex() { archive = 273, record = 4 },
            new DaggerfallTextureIndex() { archive = 273, record = 5 },
            new DaggerfallTextureIndex() { archive = 273, record = 6 },
            new DaggerfallTextureIndex() { archive = 273, record = 7 },
            new DaggerfallTextureIndex() { archive = 273, record = 8 },
            new DaggerfallTextureIndex() { archive = 273, record = 9 },
            new DaggerfallTextureIndex() { archive = 273, record = 10 },
            new DaggerfallTextureIndex() { archive = 273, record = 11 },
            new DaggerfallTextureIndex() { archive = 273, record = 12 },
            new DaggerfallTextureIndex() { archive = 273, record = 13 },
            new DaggerfallTextureIndex() { archive = 273, record = 14 },

            // Wraith
            new DaggerfallTextureIndex() { archive = 278, record = 0 },
            new DaggerfallTextureIndex() { archive = 278, record = 1 },
            new DaggerfallTextureIndex() { archive = 278, record = 2 },
            new DaggerfallTextureIndex() { archive = 278, record = 3 },
            new DaggerfallTextureIndex() { archive = 278, record = 4 },
            new DaggerfallTextureIndex() { archive = 278, record = 5 },
            new DaggerfallTextureIndex() { archive = 278, record = 6 },
            new DaggerfallTextureIndex() { archive = 278, record = 7 },
            new DaggerfallTextureIndex() { archive = 278, record = 8 },
            new DaggerfallTextureIndex() { archive = 278, record = 9 },
            new DaggerfallTextureIndex() { archive = 278, record = 10 },
            new DaggerfallTextureIndex() { archive = 278, record = 11 },
            new DaggerfallTextureIndex() { archive = 278, record = 12 },
            new DaggerfallTextureIndex() { archive = 278, record = 13 },
            new DaggerfallTextureIndex() { archive = 278, record = 14 },

            // Frost daedra
            new DaggerfallTextureIndex() { archive = 280, record = 0 },
            new DaggerfallTextureIndex() { archive = 280, record = 1 },
            new DaggerfallTextureIndex() { archive = 280, record = 2 },
            new DaggerfallTextureIndex() { archive = 280, record = 3 },
            new DaggerfallTextureIndex() { archive = 280, record = 4 },
            new DaggerfallTextureIndex() { archive = 280, record = 5 },
            new DaggerfallTextureIndex() { archive = 280, record = 6 },
            new DaggerfallTextureIndex() { archive = 280, record = 7 },
            new DaggerfallTextureIndex() { archive = 280, record = 8 },
            new DaggerfallTextureIndex() { archive = 280, record = 9 },
            new DaggerfallTextureIndex() { archive = 280, record = 10 },
            new DaggerfallTextureIndex() { archive = 280, record = 11 },
            new DaggerfallTextureIndex() { archive = 280, record = 12 },
            new DaggerfallTextureIndex() { archive = 280, record = 13 },
            new DaggerfallTextureIndex() { archive = 280, record = 14 },
            new DaggerfallTextureIndex() { archive = 280, record = 15 },
            new DaggerfallTextureIndex() { archive = 280, record = 16 },
            new DaggerfallTextureIndex() { archive = 280, record = 17 },
            new DaggerfallTextureIndex() { archive = 280, record = 18 },
            new DaggerfallTextureIndex() { archive = 280, record = 19 },
            new DaggerfallTextureIndex() { archive = 400, record = 3 }, // corpse

            // Fire daedra
            new DaggerfallTextureIndex() { archive = 281, record = 0 },
            new DaggerfallTextureIndex() { archive = 281, record = 1 },
            new DaggerfallTextureIndex() { archive = 281, record = 2 },
            new DaggerfallTextureIndex() { archive = 281, record = 3 },
            new DaggerfallTextureIndex() { archive = 281, record = 4 },
            new DaggerfallTextureIndex() { archive = 281, record = 5 },
            new DaggerfallTextureIndex() { archive = 281, record = 6 },
            new DaggerfallTextureIndex() { archive = 281, record = 7 },
            new DaggerfallTextureIndex() { archive = 281, record = 8 },
            new DaggerfallTextureIndex() { archive = 281, record = 9 },
            new DaggerfallTextureIndex() { archive = 281, record = 10 },
            new DaggerfallTextureIndex() { archive = 281, record = 11 },
            new DaggerfallTextureIndex() { archive = 281, record = 12 },
            new DaggerfallTextureIndex() { archive = 281, record = 13 },
            new DaggerfallTextureIndex() { archive = 281, record = 14 },
            new DaggerfallTextureIndex() { archive = 281, record = 15 },
            new DaggerfallTextureIndex() { archive = 281, record = 16 },
            new DaggerfallTextureIndex() { archive = 281, record = 17 },
            new DaggerfallTextureIndex() { archive = 281, record = 18 },
            new DaggerfallTextureIndex() { archive = 281, record = 19 },
            new DaggerfallTextureIndex() { archive = 400, record = 2 }, // corpse

            // Fire atronach
            new DaggerfallTextureIndex() { archive = 290, record = 0 },
            new DaggerfallTextureIndex() { archive = 290, record = 1 },
            new DaggerfallTextureIndex() { archive = 290, record = 2 },
            new DaggerfallTextureIndex() { archive = 290, record = 3 },
            new DaggerfallTextureIndex() { archive = 290, record = 4 },
            new DaggerfallTextureIndex() { archive = 290, record = 5 },
            new DaggerfallTextureIndex() { archive = 290, record = 6 },
            new DaggerfallTextureIndex() { archive = 290, record = 7 },
            new DaggerfallTextureIndex() { archive = 290, record = 8 },
            new DaggerfallTextureIndex() { archive = 290, record = 9 },
            new DaggerfallTextureIndex() { archive = 290, record = 10 },
            new DaggerfallTextureIndex() { archive = 290, record = 11 },
            new DaggerfallTextureIndex() { archive = 290, record = 12 },
            new DaggerfallTextureIndex() { archive = 290, record = 13 },
            new DaggerfallTextureIndex() { archive = 290, record = 14 },
            new DaggerfallTextureIndex() { archive = 290, record = 15 },
            new DaggerfallTextureIndex() { archive = 290, record = 16 },
            new DaggerfallTextureIndex() { archive = 290, record = 17 },
            new DaggerfallTextureIndex() { archive = 290, record = 18 },
            new DaggerfallTextureIndex() { archive = 290, record = 19 },
            new DaggerfallTextureIndex() { archive = 405, record = 2 }, // corpse

            // Mantellan Crux fire textures
            new DaggerfallTextureIndex() { archive = 356, record = 0 },
            new DaggerfallTextureIndex() { archive = 356, record = 2 },
            new DaggerfallTextureIndex() { archive = 356, record = 3 },

            // Spell missiles
            new DaggerfallTextureIndex() { archive = 375, record = 0 },
            new DaggerfallTextureIndex() { archive = 375, record = 1 },
            new DaggerfallTextureIndex() { archive = 376, record = 0 },
            new DaggerfallTextureIndex() { archive = 376, record = 1 },
            new DaggerfallTextureIndex() { archive = 377, record = 0 },
            new DaggerfallTextureIndex() { archive = 377, record = 1 },
            new DaggerfallTextureIndex() { archive = 378, record = 0 },
            new DaggerfallTextureIndex() { archive = 378, record = 1 },
            new DaggerfallTextureIndex() { archive = 379, record = 0 },
            new DaggerfallTextureIndex() { archive = 379, record = 1 },

            // Magic decorative effects
            new DaggerfallTextureIndex() { archive = 380, record = 3 },
            // new DaggerfallTextureIndex() { archive = 380, record = 5 }, // UI
            new DaggerfallTextureIndex() { archive = 434, record = 3 },
            // new DaggerfallTextureIndex() { archive = 434, record = 5 }, // UI

            // Lysandus
            new DaggerfallTextureIndex() { archive = 473, record = 0 },
            new DaggerfallTextureIndex() { archive = 473, record = 1 },
            new DaggerfallTextureIndex() { archive = 473, record = 2 },
            new DaggerfallTextureIndex() { archive = 473, record = 3 },
            new DaggerfallTextureIndex() { archive = 473, record = 4 },
            new DaggerfallTextureIndex() { archive = 473, record = 5 },
            new DaggerfallTextureIndex() { archive = 473, record = 6 },
            new DaggerfallTextureIndex() { archive = 473, record = 7 },
            new DaggerfallTextureIndex() { archive = 473, record = 8 },
            new DaggerfallTextureIndex() { archive = 473, record = 9 },
            new DaggerfallTextureIndex() { archive = 473, record = 10 },
            new DaggerfallTextureIndex() { archive = 473, record = 11 },
            new DaggerfallTextureIndex() { archive = 473, record = 12 },
            new DaggerfallTextureIndex() { archive = 473, record = 13 },
            new DaggerfallTextureIndex() { archive = 473, record = 14 },
        };

        static Dictionary<int, List<int>> fastEmissiveTextures = null;

        private static void FastEmissiveTexturesInit()
        {
            fastEmissiveTextures = new Dictionary<int, List<int>>();
            foreach (DaggerfallTextureIndex emissiveTexture in emissiveTextures)
            {
                List<int> textureRecords;
                if (!fastEmissiveTextures.TryGetValue(emissiveTexture.archive, out textureRecords))
                {
                    textureRecords = new List<int>();
                    fastEmissiveTextures.Add(emissiveTexture.archive, textureRecords);
                }
                textureRecords.Add(emissiveTexture.record);
            }
        }

        public bool IsEmissiveArchive(int archive)
        {
            return fastEmissiveTextures.ContainsKey(archive);
        }

        public bool IsEmissive(int archive, int record)
        {
            List<int> textureRecords;
            if (fastEmissiveTextures.TryGetValue(archive, out textureRecords))
            {
                // Check emissive list for this texture
                foreach (int textureRecord in textureRecords)
                {
                    if (textureRecord == record)
                        return true;
                }
            }
            return false;
        }

#if UNITY_EDITOR && !UNITY_WEBPLAYER
        public static void SaveTextureToPNG(Texture2D source, string path)
        {
            byte[] bytes = source.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
        }
#endif

        /// <summary>
        /// Checks if sprite is a child NPC sprite texture.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Texture record.</param>
        /// <returns>True if a child NPC sprite texture.</returns>
        public static bool IsChildNPCTexture(int archive, int record)
        {
            // Archives known to store child NPC textures
            // Records are checked for each individually for a match
            const int templePeople = 181;
            const int mediumCommonPeople = 182;
            const int flatPeople2 = 184;
            const int testBigFlats = 186;
            const int kludgeTown = 197;
            const int daggerfallPeople = 334;
            const int wayrestPeople = 346;
            const int sentinelPeople = 357;

            if (archive == templePeople)
            {
                if (record == 3)
                    return true;
            }

            if (archive == mediumCommonPeople)
            {
                if (record == 4 || record == 5 || record == 6 || record == 18 || record == 36 || record == 37 || record == 38 || record == 42 || record == 43 || record == 52 || record == 53)
                    return true;
            }

            if (archive == flatPeople2)
            {
                if (record == 15)
                    return true;
            }

            if (archive == testBigFlats)
            {
                if (record == 4 || record == 5 || record == 6 || record == 7 || record == 19 || record == 37 || record == 38 || record == 39 || record == 43 || record == 44 || record == 53 || record == 54)
                    return true;
            }

            if (archive == kludgeTown)
            {
                if (record == 3)
                    return true;
            }

            if (archive == daggerfallPeople)
            {
                if (record == 2 || record == 3 || record == 6 || record == 9 || record == 12)
                    return true;
            }

            if (archive == wayrestPeople)
            {
                if (record == 2 || record == 3 || record == 12 || record == 15 || record == 16 || record == 18)
                    return true;
            }

            if (archive == sentinelPeople)
            {
                if (record == 5 || record == 6 || record == 7 || record == 8)
                    return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        private TextureFormat ParseTextureFormat(SupportedAlphaTextureFormats format)
        {
            switch (format)
            {
                default:
                case SupportedAlphaTextureFormats.RGBA32:
                    return TextureFormat.RGBA32;
                case SupportedAlphaTextureFormats.ARGB32:
                    return TextureFormat.ARGB32;
                case SupportedAlphaTextureFormats.ARGB444:
                    return TextureFormat.ARGB4444;
                case SupportedAlphaTextureFormats.RGBA444:
                    return TextureFormat.RGBA4444;
            }
        }

        #endregion
    }
}