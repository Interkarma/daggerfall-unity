using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureReaderAtlas: MonoBehaviour
{
    public GetTextureResults results;

    public IEnumerator Load(
        TextureReader textureReader,
        GetTextureSettings settings,
        SupportedAlphaTextureFormats alphaTextureFormat = SupportedAlphaTextureFormats.ARGB32)
    {
        results = new GetTextureResults();

        // Individual textures must remain readable to pack into atlas
        bool stayReadable = settings.stayReadable;
        settings.stayReadable = true;

        // Assign texture file
        TextureFile textureFile;
        if (settings.textureFile == null)
        {
            textureFile = new TextureFile(Path.Combine(textureReader.Arena2Path, TextureFile.IndexToFileName(settings.archive)), FileUsage.UseMemory, true);
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
                textureReader.MipMaps = false;

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
                GetTextureResults nextTextureResults = textureReader.GetTexture2D(settings, alphaTextureFormat, textureImport);
                yield return null;

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
        Texture2D atlasAlbedoMap = new Texture2D(settings.atlasMaxSize, settings.atlasMaxSize, textureReader.ParseTextureFormat(alphaTextureFormat), textureReader.MipMaps);
        Rect[] rects = atlasAlbedoMap.PackTextures(albedoTextures.ToArray(), settings.atlasPadding, settings.atlasMaxSize, !stayReadable);

        // Pack normal textures into atlas
        Texture2D atlasNormalMap = null;
        if (hasNormalMaps)
        {
            // Normals must be ARGB32
            atlasNormalMap = new Texture2D(settings.atlasMaxSize, settings.atlasMaxSize, TextureFormat.ARGB32, textureReader.MipMaps);
            atlasNormalMap.PackTextures(normalTextures.ToArray(), settings.atlasPadding, settings.atlasMaxSize, !stayReadable);
        }

        // Pack emission textures into atlas
        // TODO: Change this as packing not consistent
        Texture2D atlasEmissionMap = null;
        if (hasEmissionMaps)
        {
            // Repacking to ensure correct mix of lit and unlit
            atlasEmissionMap = new Texture2D(settings.atlasMaxSize, settings.atlasMaxSize, textureReader.ParseTextureFormat(alphaTextureFormat), textureReader.MipMaps);
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
    }
}
