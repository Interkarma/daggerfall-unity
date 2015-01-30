// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

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
        /// Constructor to set Arena2Path.
        /// </summary>
        /// <param name="arena2Path">Path to Arena2 folder.</param>
        public TextureReader(string arena2Path)
        {
            this.Arena2Path = arena2Path;
        }

        /// <summary>
        /// Gets Unity Texture2D from Daggerfall texture.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <returns>Texture2D or null.</returns>
        public Texture2D GetTexture2D(int archive, int record, int frame = 0, int alphaIndex = -1)
        {
            Rect rect;
            return GetTexture2D(archive, record, frame, alphaIndex, out rect);
        }

        /// <summary>
        /// Gets Unity Texture2D from Daggerfall texture with more options.
        /// </summary>
        /// <param name="archive">Archive index.</param>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <param name="rectOut">Receives UV rect for texture inside border.</param>
        /// <param name="border">Number of pixels border to add around image.</param>
        /// <param name="dilate">Blend texture into surrounding empty pixels. Requires border.</param>
        /// <param name="copyToOppositeBorder">Copy texture edges to opposite border. Requires border, will overwrite dilate.</param>
        /// <returns>Texture2D or null.</returns>
        public Texture2D GetTexture2D(
            int archive,
            int record,
            int frame,
            int alphaIndex,
            out Rect rectOut,
            int border = 0,
            bool dilate = false,
            bool copyToOppositeBorder = false,
            bool makeNoLongerReadable = true)
        {
            // Ready check
            if (!ReadyCheck())
            {
                rectOut = new Rect();
                return null;
            }

            // Load texture file
            textureFile.Load(Path.Combine(Arena2Path, TextureFile.IndexToFileName(archive)), FileUsage.UseMemory, true);

            // Get Color32 array
            DFSize sz;
            Color32[] colors = textureFile.GetColors32(record, frame, alphaIndex, border, out sz);

            // Dilate edges
            if (border > 0 && dilate && !copyToOppositeBorder)
                ImageProcessing.DilateColors(ref colors, sz);

            // Copy to opposite border
            if (border > 0 && copyToOppositeBorder)
                ImageProcessing.WrapBorder(ref colors, sz, border);

            // Create Texture2D
            Texture2D texture;
            if (alphaIndex < 0)
                texture = new Texture2D(sz.Width, sz.Height, TextureFormat.RGB24, MipMaps);
            else
                texture = new Texture2D(sz.Width, sz.Height, TextureFormat.RGBA32, MipMaps);
            texture.SetPixels32(colors);
            texture.Apply(true, makeNoLongerReadable);

            // Shrink UV rect to compensate for internal border
            float ru = 1f / sz.Width;
            float rv = 1f / sz.Height;
            rectOut = new Rect(border * ru, border * rv, (sz.Width - border * 2) * ru, (sz.Height - border * 2) * rv);

            return texture;
        }

        /// <summary>
        /// Gets Texture2D atlas from Daggerfall texture archive.
        /// Every record and frame in the archive will be added to atlas.
        /// An array of rects will be returned with sub-texture rect for each record index and frame.
        /// Used for non-tiling textures like flats and ground tiles.
        /// </summary>
        /// <param name="archive">Archive index to create atlas from.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <param name="padding">Number of pixels padding around each sub-texture.</param>
        /// <param name="maxAtlasSize">Max size of atlas.</param>
        /// <param name="rectsOut">Array of rects, one for each record sub-texture and frame.</param>
        /// <param name="indicesOut">Array of record indices into rect array, accounting for animation frames.</param>
        /// <param name="border">Number of pixels internal border around each texture.</param>
        /// <param name="dilate">Blend texture into surrounding empty pixels.</param>
        /// <param name="shrinkUVs">Number of extra pixels to shrink UV rect.</param>
        /// <param name="copyToOppositeBorder">Copy texture edges to opposite border. Requires border, will overwrite dilate.</param>
        /// <returns>Texture2D atlas or null.</returns>
        public Texture2D GetTexture2DAtlas(
            int archive,
            int alphaIndex,
            int padding,
            int maxAtlasSize,
            out Rect[] rectsOut,
            out RecordIndex[] indicesOut,
            out Vector2[] sizesOut,
            out Vector2[] scalesOut,
            out Vector2[] offsetsOut,
            int border,
            bool dilate,
            int shrinkUVs = 0,
            bool copyToOppositeBorder = false,
            bool makeNoLongerReadable = true)
        {
            // Ready check
            if (!ReadyCheck())
            {
                rectsOut = null;
                indicesOut = null;
                sizesOut = null;
                scalesOut = null;
                offsetsOut = null;
                return null;
            }

            // Load texture file
            textureFile.Load(Path.Combine(Arena2Path, TextureFile.IndexToFileName(archive)), FileUsage.UseMemory, true);

            // Read every texture in archive
            Rect rect;
            List<Texture2D> textures = new List<Texture2D>();
            List<RecordIndex> indices = new List<RecordIndex>();
            sizesOut = new Vector2[textureFile.RecordCount];
            scalesOut = new Vector2[textureFile.RecordCount];
            offsetsOut = new Vector2[textureFile.RecordCount];
            for (int record = 0; record < textureFile.RecordCount; record++)
            {
                int frames = textureFile.GetFrameCount(record);
                DFSize size = textureFile.GetSize(record);
                DFSize scale = textureFile.GetScale(record);
                DFSize offset = textureFile.GetOffset(record);
                RecordIndex ri = new RecordIndex()
                {
                    startIndex = textures.Count,
                    frameCount = frames,
                    width = size.Width,
                    height = size.Height,
                };
                indices.Add(ri);
                for (int frame = 0; frame < frames; frame++)
                {
                    textures.Add(GetTexture2D(archive, record, frame, alphaIndex, out rect, border, dilate, copyToOppositeBorder, false));
                }

                sizesOut[record] = new Vector2(size.Width, size.Height);
                scalesOut[record] = new Vector2(scale.Width, scale.Height);
                offsetsOut[record] = new Vector2(offset.Width, offset.Height);
            }

            // Pack textures into atlas
            Texture2D atlas = new Texture2D(maxAtlasSize, maxAtlasSize, TextureFormat.RGBA32, MipMaps);
            rectsOut = atlas.PackTextures(textures.ToArray(), padding, maxAtlasSize, makeNoLongerReadable);
            indicesOut = indices.ToArray();

            // Shrink UV rect to compensate for internal border
            float ru = 1f / atlas.width;
            float rv = 1f / atlas.height;
            border += shrinkUVs;
            for (int i = 0; i < rectsOut.Length; i++)
            {
                Rect rct = rectsOut[i];
                rct.xMin += border * ru;
                rct.xMax -= border * ru;
                rct.yMin += border * rv;
                rct.yMax -= border * rv;
                rectsOut[i] = rct;
            }

            return atlas;
        }

        /// <summary>
        /// Gets specially packed tileset atlas for terrains.
        /// This needs to be improved to create each mip level manually to further reduce artifacts.
        /// </summary>
        /// <param name="archive">Archive index of terrain tiles (e.g. 302).</param>
        /// <param name="makeNoLongerReadable"></param>
        /// <returns>Texture2D.</returns>
        public Texture2D GetTerrainTilesetTexture(int archive, bool makeNoLongerReadable = true)
        {
            const int atlasDim = 2048;
            const int gutterSize = 32;

            // Ready check
            if (!ReadyCheck())
                return null;

            // Load texture file and check count matches terrain tiles
            textureFile.Load(Path.Combine(Arena2Path, TextureFile.IndexToFileName(archive)), FileUsage.UseMemory, true);
            if (textureFile.RecordCount != 56)
                return null;

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
                Color32[] normal = textureFile.GetColors32(record, 0, -1, gutterSize, out sz);

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
                        ImageProcessing.ClampBorder(ref normal, sz, gutterSize);
                        break;

                    // Textures that clamp horizontally and tile vertically
                    case 6:
                    case 11:
                    case 16:
                    case 21:
                    case 26:
                    case 31:
                        ImageProcessing.WrapBorder(ref normal, sz, gutterSize, false);
                        ImageProcessing.ClampBorder(ref normal, sz, gutterSize, true, false);
                        break;

                    // Textures that tile in all directions
                    default:
                        ImageProcessing.WrapBorder(ref normal, sz, gutterSize);
                        break;
                }

                // Create variants
                Color32[] rotate = ImageProcessing.RotateColors(ref normal, sz.Width, sz.Height);
                Color32[] flip = ImageProcessing.FlipColors(ref normal, sz.Width, sz.Height);
                Color32[] rotateFlip = ImageProcessing.RotateColors(ref flip, sz.Width, sz.Height);

                // Insert into atlas
                ImageProcessing.InsertColors(ref normal, ref atlasColors, x, y, sz.Width, sz.Height, atlasDim, atlasDim);
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
            Texture2D atlas = new Texture2D(atlasDim, atlasDim, TextureFormat.RGB24, MipMaps);
            atlas.SetPixels32(atlasColors);
            atlas.Apply(true, makeNoLongerReadable);

            // Change settings for this texture
            atlas.wrapMode = TextureWrapMode.Clamp;
            atlas.anisoLevel = 8;

            return atlas;
        }

#if UNITY_EDITOR && !UNITY_WEBPLAYER
        public void SaveTextureToPNG(Texture2D source, string path)
        {
            byte[] bytes = source.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
        }
#endif

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