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
            ImageData? data = GetImageData(filename, record, frame, hasAlpha);
            if (data == null)
                return null;

            return data.Value.texture;
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
        /// Cuts out a sub-texture from source Color32 array.
        /// Useful for slicing up native UI elements into smaller functional units.
        /// </summary>
        /// <param name="colors">Source Color32 array.</param>
        /// <param name="subRect">Rectangle of source image to extract. Origin 0,0 is top-left.</param>
        /// <returns>New Texture2D containing sub-texture.</returns>
        public static Texture2D GetSubTexture(Color32[] colors, Rect subRect, int srcWidth, int srcHeight)
        {
            // Check ready
            if (colors == null)
                return null;

            // Invert Y as Unity textures have origin 0,0 at bottom-left and we use top-left
            subRect.y = srcHeight - subRect.y - subRect.height;

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

            // Create new texture
            Texture2D newTexture = new Texture2D((int)subRect.width, (int)subRect.height, TextureFormat.ARGB32, false);
            newTexture.SetPixels32(newColors, 0);
            newTexture.Apply(false, false);
            newTexture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            return newTexture;
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
            ImageData data = new ImageData();
            data.type = fileType;
            data.filename = filename;
            data.record = record;
            data.frame = frame;
            data.hasAlpha = hasAlpha;

            // Read supported image files
            DFBitmap dfBitmap = null;
            switch (fileType)
            {
                case ImageTypes.TEXTURE:
                    TextureFile textureFile = new TextureFile(Path.Combine(dfUnity.Arena2Path, filename), FileUsage.UseMemory, true);
                    textureFile.LoadPalette(Path.Combine(dfUnity.Arena2Path, textureFile.PaletteName));
                    dfBitmap = textureFile.GetDFBitmap(record, frame);
                    data.offset = textureFile.GetOffset(record);
                    data.scale = textureFile.GetScale(record);
                    data.size = textureFile.GetSize(record);
                    break;

                case ImageTypes.IMG:
                    ImgFile imgFile = new ImgFile(Path.Combine(dfUnity.Arena2Path, filename), FileUsage.UseMemory, true);
                    imgFile.LoadPalette(Path.Combine(dfUnity.Arena2Path, imgFile.PaletteName));
                    dfBitmap = imgFile.GetDFBitmap();
                    data.size = imgFile.GetSize(0);
                    break;

                case ImageTypes.CIF:
                case ImageTypes.RCI:
                    CifRciFile cifFile = new CifRciFile(Path.Combine(dfUnity.Arena2Path, filename), FileUsage.UseMemory, true);
                    cifFile.LoadPalette(Path.Combine(dfUnity.Arena2Path, cifFile.PaletteName));
                    dfBitmap = cifFile.GetDFBitmap(record, frame);
                    data.offset = cifFile.GetOffset(record);
                    data.size = cifFile.GetSize(record);
                    break;

                default:
                    return new ImageData();
            }

            // Get colors array
            Color32[] colors = dfBitmap.GetColor32((hasAlpha) ? 0 : -1);
            if (colors == null)
                return new ImageData();

            // Store bitmap
            data.dfBitmap = dfBitmap;

            // Create Texture2D
            if (createTexture)
            {
                Texture2D texture = new Texture2D(data.size.Width, data.size.Height, TextureFormat.ARGB32, false);
                texture.SetPixels32(colors);
                texture.Apply(false, false);
                texture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
                data.texture = texture;
            }

            return data;
        }

        /// <summary>
        /// Updates Texture2D from DFBitmap data.
        /// </summary>
        /// <param name="imageData">ImageData to update texture.</param>
        public static void UpdateTexture(ref ImageData imageData)
        {
            // Get colors array
            Color32[] colors = imageData.dfBitmap.GetColor32((imageData.hasAlpha) ? 0 : -1);
            if (colors == null)
                return;

            // Create new Texture2D
            Texture2D texture = new Texture2D(imageData.dfBitmap.Width, imageData.dfBitmap.Height, TextureFormat.ARGB32, false);
            texture.SetPixels32(colors);
            texture.Apply(false, false);
            texture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            imageData.texture = texture;
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
            else
                throw new Exception("ParseFileType could not match filename with a supported image type.");
        }

        #endregion
    }
}