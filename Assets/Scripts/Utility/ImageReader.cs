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
using System;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Static helper to read most Daggerfall image files in a simple, consistent manner.
    /// Useful for direct-to-texture features like UI elements.
    /// Highly simplified compared to TextureReader and MaterialReader.
    /// Not intended for generating scene materials.
    /// </summary>
    public static class ImageReader
    {
        #region Public Methods

        /// <summary>
        /// Reads any Daggerfall image to Texture2D.
        /// </summary>
        /// <param name="filename">Name of standalone file as it appears in arena2 folder.</param>
        /// <param name="record">Which image record to read for multi-image files.</param>
        /// <param name="frame">Which frame to read for multi-frame images.</param>
        /// <param name="hasAlpha">Enable this for image cutouts.</param>
        /// <returns>Texture2D.</returns>
        public static Texture2D GetTexture(string filename, int record = 0, int frame = 0, bool hasAlpha = false)
        {
            ImageData data = GetImageData(filename, record, frame, hasAlpha);
            if (data.type == ImageTypes.None)
                return null;

            return data.texture;
        }

        /// <summary>
        /// Gets texture from source Color32 array.
        /// </summary>
        /// <param name="colors">Source Color32 array.</param>
        /// <param name="srcWidth">Source width.</param>
        /// <param name="srcHeight">Source height.</param>
        /// <returns>Texture2D.</returns>
        public static Texture2D GetTexture(Color32[] colors, int srcWidth, int srcHeight)
        {
            Texture2D texture = new Texture2D(srcWidth, srcHeight, TextureFormat.ARGB32, false);
            texture.SetPixels32(colors, 0);
            texture.Apply(false, false);
            texture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            return texture;
        }

        /// <summary>
        /// Gets Color32 array from texture. Generates texture if not present.
        /// Origin 0,0 will always be at Unity texture bottom-left.
        /// </summary>
        /// <param name="imageData">Source ImageData.</param>
        /// <returns>Color32 array.</returns>
        public static Color32[] GetColors(ImageData imageData)
        {
            // Generate texture if not present
            if (imageData.texture == null)
                UpdateTexture(ref imageData);

            return imageData.texture.GetPixels32();
        }

        /// <summary>
        /// Cuts out a sub-texture from source texture.
        /// Useful for slicing up native UI elements into smaller functional units.
        /// </summary>
        /// <param name="texture">Source texture. Must be readable.</param>
        /// <param name="subRect">Rectangle of source image to extract. Origin 0,0 is top-left.</param>
        /// <returns>New Texture2D containing sub-texture.</returns>
        public static Texture2D GetSubTexture(Texture2D texture, Rect subRect)
        {
            // Check ready
            if (texture == null)
                return null;

            // Get Color32 from source texture
            Color32[] colors = texture.GetPixels32();

            return GetSubTexture(colors, subRect, texture.width, texture.height);
        }

        /// <summary>
        /// Cuts out a sub-texture from source ImageData.
        /// Texture
        /// </summary>
        /// <param name="imageData">Source ImageData.</param>
        /// <param name="subRect">Rectangle of source image to extract. Origin 0,0 is top-left.</param>
        /// <returns>New Texture2D containing sub-texture.</returns>
        public static Texture2D GetSubTexture(ImageData imageData, Rect subRect)
        {
            if (imageData.texture != null)
                return GetSubTexture(imageData.texture, subRect);

            Color32[] colors = GetColors(imageData);
            return GetSubTexture(colors, subRect, imageData.dfBitmap.Width, imageData.dfBitmap.Height);
        }

        /// <summary>
        /// Cuts out a sub-texture from source Color32 array.
        /// Useful for slicing up native UI elements into smaller functional units.
        /// </summary>
        /// <param name="colors">Source Color32 array.</param>
        /// <param name="subRect">Rectangle of source image to extract. Origin 0,0 is top-left.</param>
        /// <param name="srcWidth">Source width.</param>
        /// <param name="srcHeight">Source height.</param>
        /// <returns>New Texture2D containing sub-texture.</returns>
        public static Texture2D GetSubTexture(Color32[] colors, Rect subRect, int srcWidth, int srcHeight)
        {
            // Check ready
            if (colors == null)
                return null;

            // Invert Y as Unity textures have origin 0,0 at bottom-left and we use top-left
            subRect.y = srcHeight - subRect.y - subRect.height;

            // Create new texture
            Color32[] newColors = GetSubColors(colors, subRect, srcWidth, srcHeight);
            Texture2D newTexture = new Texture2D((int)subRect.width, (int)subRect.height, TextureFormat.ARGB32, false);
            newTexture.SetPixels32(newColors, 0);
            newTexture.Apply(false, false);
            newTexture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            return newTexture;
        }

        /// <summary>
        /// Gets sub-rect of Color32 array.
        /// </summary>
        /// <param name="colors">Source Color32 array.</param>
        /// <param name="subRect">Rectangle of source image to extract.</param>
        /// <param name="srcWidth">Source width.</param>
        /// <param name="srcHeight">Source height.</param>
        /// <returns>Color32 array containing sub-rect.</returns>
        public static Color32[] GetSubColors(Color32[] colors, Rect subRect, int srcWidth, int srcHeight)
        {
            // Create new Color32 array
            Color32[] newColors = new Color32[(int)subRect.width * (int)subRect.height];
            ImageProcessing.CopyColors(
                ref colors,
                ref newColors,
                new DFSize(srcWidth, srcHeight),
                new DFSize((int)subRect.width, (int)subRect.height),
                new DFPosition((int)subRect.x, (int)subRect.y),
                new DFPosition(0, 0),
                new DFSize((int)subRect.width, (int)subRect.height));

            return newColors;
        }

        /// <summary>
        /// Reads any Daggerfall image file to ImageData package.
        /// </summary>
        /// <param name="filename">Name of standalone file as it appears in arena2 folder.</param>
        /// <param name="record">Which image record to read for multi-image files.</param>
        /// <param name="frame">Which frame to read for multi-frame images.</param>
        /// <param name="hasAlpha">Enable this for image cutouts.</param>
        /// <param name="createTexture">Create a Texture2D.</param>
        /// <returns>ImageData. If result.type == ImageTypes.None then read failed.</returns>
        public static ImageData GetImageData(string filename, int record = 0, int frame = 0, bool hasAlpha = false, bool createTexture = true)
        {
            // Check API ready
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return new ImageData();

            // Parse image file type
            ImageTypes fileType;
            try
            {
                fileType = ParseFileType(filename);
            }
            catch
            {
                return new ImageData();
            }

            // Create base image data
            ImageData imageData = new ImageData();
            imageData.type = fileType;
            imageData.filename = filename;
            imageData.record = record;
            imageData.frame = frame;
            imageData.hasAlpha = hasAlpha;

            // Read supported image files
            DFBitmap dfBitmap = null;
            switch (fileType)
            {
                case ImageTypes.TEXTURE:
                    TextureFile textureFile = new TextureFile(Path.Combine(dfUnity.Arena2Path, filename), FileUsage.UseMemory, true);
                    textureFile.LoadPalette(Path.Combine(dfUnity.Arena2Path, textureFile.PaletteName));
                    dfBitmap = textureFile.GetDFBitmap(record, frame);
                    imageData.offset = textureFile.GetOffset(record);
                    imageData.scale = textureFile.GetScale(record);
                    imageData.size = textureFile.GetSize(record);
                    break;

                case ImageTypes.IMG:
                    ImgFile imgFile = new ImgFile(Path.Combine(dfUnity.Arena2Path, filename), FileUsage.UseMemory, true);
                    imgFile.LoadPalette(Path.Combine(dfUnity.Arena2Path, imgFile.PaletteName));
                    dfBitmap = imgFile.GetDFBitmap();
                    imageData.offset = imgFile.ImageOffset;
                    imageData.scale = new DFSize();
                    imageData.size = imgFile.GetSize(0);

                    // texture pack support
                    if ((AssetInjection.TextureReplacement.CustomImageExist(filename)) && (createTexture))
                    {
                        imageData.texture = AssetInjection.TextureReplacement.LoadCustomImage(filename);
                        createTexture = false;
                    }

                    break;

                case ImageTypes.CIF:
                case ImageTypes.RCI:
                    CifRciFile cifFile = new CifRciFile(Path.Combine(dfUnity.Arena2Path, filename), FileUsage.UseMemory, true);
                    cifFile.LoadPalette(Path.Combine(dfUnity.Arena2Path, cifFile.PaletteName));
                    dfBitmap = cifFile.GetDFBitmap(record, frame);
                    imageData.offset = cifFile.GetOffset(record);
                    imageData.scale = new DFSize();
                    imageData.size = cifFile.GetSize(record);

                    // texture pack support
                    if ((AssetInjection.TextureReplacement.CustomCifExist(filename, record, frame)) && (createTexture))
                    {
                        imageData.texture = AssetInjection.TextureReplacement.LoadCustomCif(filename, record, frame);
                        createTexture = false;
                    }

                    break;

                case ImageTypes.CFA:
                    CfaFile cfaFile = new CfaFile(Path.Combine(dfUnity.Arena2Path, filename), FileUsage.UseMemory, true);
                    cfaFile.LoadPalette(Path.Combine(dfUnity.Arena2Path, cfaFile.PaletteName));

                    dfBitmap = cfaFile.GetDFBitmap(record, frame);
                    imageData.offset = new DFPosition(0, 0);
                    imageData.scale = new DFSize();
                    imageData.size = cfaFile.GetSize(record);

                    // TODO: texture pack support?

                    break;

                default:
                    return new ImageData();
            }

            // Store bitmap
            imageData.dfBitmap = dfBitmap;
            imageData.width = dfBitmap.Width;
            imageData.height = dfBitmap.Height;

            // Create Texture2D
            if (createTexture)
            {
                // Get colors array
                Color32[] colors = GetColors(imageData);
                if (colors == null)
                    return new ImageData();

                // Create new Texture2D
                imageData.texture = GetTexture(colors, imageData.width, imageData.height);
            }

            return imageData;
        }

        /// <summary>
        /// Updates Texture2D from DFBitmap data.
        /// </summary>
        /// <param name="imageData">Source ImageData.</param>
        public static void UpdateTexture(ref ImageData imageData)
        {
            // Get colors array
            Color32[] colors = imageData.dfBitmap.GetColor32((imageData.hasAlpha) ? 0 : -1);
            if (colors == null)
                return;

            // Create new Texture2D
            imageData.texture = GetTexture(colors, imageData.width, imageData.height);
        }

        /// <summary>
        /// Updates Texture2D from DFBitmap data.
        /// </summary>
        /// <param name="imageData">Source ImageData.</param>
        /// <param name="maskColor">Set mask pixels to this colour.</param>
        public static void UpdateTexture(ref ImageData imageData, Color maskColor)
        {
            // Get colors array
            Color32[] colors = imageData.dfBitmap.GetColor32((imageData.hasAlpha) ? 0 : -1, 0xff, maskColor);
            if (colors == null)
                return;

            // Create new Texture2D
            imageData.texture = GetTexture(colors, imageData.width, imageData.height);
        }

        #endregion

        #region Private Methods

        // Parses a case-insensitive filename into known image file types
        static ImageTypes ParseFileType(string filename)
        {
            if (filename.StartsWith("TEXTURE.", StringComparison.InvariantCultureIgnoreCase))
                return ImageTypes.TEXTURE;
            else if (filename.EndsWith(".IMG", StringComparison.InvariantCultureIgnoreCase))
                return ImageTypes.IMG;
            else if (filename.EndsWith(".CIF", StringComparison.InvariantCultureIgnoreCase))
                return ImageTypes.CIF;
            else if (filename.EndsWith(".RCI", StringComparison.InvariantCultureIgnoreCase))
                return ImageTypes.RCI;
            else if (filename.EndsWith(".CFA", StringComparison.InvariantCultureIgnoreCase))
                return ImageTypes.CFA;
            else
                throw new Exception("ParseFileType could not match filename with a supported image type.");
        }

        #endregion
    }
}