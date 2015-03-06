// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.IO;
using System.Collections;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Basic image processing for DFBitmap, Color32, and FntFile formats.
    /// </summary>
    public static class ImageProcessing
    {
        #region Helpers

        // Clamps value to range
        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        // Inserts a Color32 array into XY position of another Color32 array
        public static void InsertColors(ref Color32[] src, ref Color32[] dst, int xPos, int yPos, int srcWidth, int srcHeight, int dstWidth, int dstHeight)
        {
            for (int y = 0; y < srcHeight; y++)
            {
                for (int x = 0; x < srcWidth; x++)
                {
                    Color32 col = src[y * srcWidth + x];
                    dst[(yPos + y) * dstWidth + (xPos + x)] = col;
                }
            }
        }

        /// <summary>
        /// Converts full colour DFBitmap to Color32 array.
        /// </summary>
        /// <param name="bitmap">Source DFBitmap.</param>
        /// <param name="flipY">Flip Y rows so bottom becomes top.</param>
        /// <returns>Color32 array.</returns>
        public static Color32[] ConvertToColor32(DFBitmap bitmap, bool flipY = true)
        {
            // Must not be indexed
            if (bitmap.Format == DFBitmap.Formats.Indexed)
                return null;

            // Create destination array
            Color32[] colors = new Color32[bitmap.Width * bitmap.Height];

            int index = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                int rowOffset = (flipY) ? (bitmap.Height - 1 - y) * bitmap.Stride : y * bitmap.Stride;
                for (int x = 0; x < bitmap.Width; x++)
                {
                    // Get color bytes
                    int pos = rowOffset + x * bitmap.FormatWidth;
                    byte r = bitmap.Data[pos++];
                    byte g = bitmap.Data[pos++];
                    byte b = bitmap.Data[pos++];
                    byte a = bitmap.Data[pos];

                    // Store in array
                    colors[index++] = new Color32(r, g, b, a);
                }
            }

            return colors;
        }

        // Helper to create and apply Texture2D from single call
        public static Texture2D MakeTexture2D(ref Color32[] src, int width, int height, TextureFormat textureFormat, bool mipMaps)
        {
            Texture2D texture = new Texture2D(width, height, textureFormat, mipMaps);
            texture.SetPixels32(src);
            texture.Apply(mipMaps);

            return texture;
        }

        // Helper to save Texture2D to a PNG file
        public static void SaveTextureAsPng(Texture2D texture, string path)
        {
            // Path must end with PNG
            if (!path.EndsWith(".png", System.StringComparison.InvariantCultureIgnoreCase))
                path += ".png";

            // Encode and save file
            byte[] buffer = texture.EncodeToPNG();
            File.WriteAllBytes(path, buffer);
        }

        #endregion

        #region Scaling

        /// <summary>
        /// Resize DFBitmap with a bicubic kernel.
        /// </summary>
        /// <param name="bitmap">DFBitmap source.</param>
        /// <param name="level">MipMap level.</param>
        /// <returns>SnapBitmap mipmap.</returns>
        public static DFBitmap ResizeBicubic(DFBitmap bitmap, int newWidth, int newHeight)
        {
            // Must be a colour format
            if (bitmap.Format == DFBitmap.Formats.Indexed)
                return null;

            // Get start size
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Create new bitmap
            DFBitmap newBitmap = new DFBitmap();
            newBitmap.Width = newWidth;
            newBitmap.Height = newHeight;
            newBitmap.Format = bitmap.Format;
            newBitmap.Stride = newBitmap.Width * bitmap.FormatWidth;
            newBitmap.Data = new byte[newBitmap.Stride * newBitmap.Height];

            // Scale factors
            double xFactor = (double)width / newWidth;
            double yFactor = (double)height / newHeight;

            // Coordinates of source points and coefficients
            double ox, oy, dx, dy, k1, k2;
            int ox1, oy1, ox2, oy2;

            // Destination pixel values
            double r, g, b, a;

            // Width and height decreased by 1
            int ymax = height - 1;
            int xmax = width - 1;

            // Bicubic resize
            for (int y = 0; y < newHeight; y++)
            {
                // Y coordinates
                oy = (double)y * yFactor - 0.5f;
                oy1 = (int)oy;
                dy = oy - (double)oy1;

                for (int x = 0; x < newWidth; x++)
                {
                    // X coordinates
                    ox = (double)x * xFactor - 0.5f;
                    ox1 = (int)ox;
                    dx = ox - (double)ox1;

                    // Initial pixel value
                    r = g = b = a = 0;

                    for (int n = -1; n < 3; n++)
                    {
                        // Get Y coefficient
                        k1 = BiCubicKernel(dy - (double)n);

                        oy2 = oy1 + n;
                        if (oy2 < 0)
                            oy2 = 0;
                        if (oy2 > ymax)
                            oy2 = ymax;

                        for (int m = -1; m < 3; m++)
                        {
                            // Get X coefficient
                            k2 = k1 * BiCubicKernel((double)m - dx);

                            ox2 = ox1 + m;
                            if (ox2 < 0)
                                ox2 = 0;
                            if (ox2 > xmax)
                                ox2 = xmax;

                            // Get pixel of original image
                            int srcPos = (ox2 * bitmap.FormatWidth) + (bitmap.Stride * oy2);
                            r += k2 * bitmap.Data[srcPos++];
                            g += k2 * bitmap.Data[srcPos++];
                            b += k2 * bitmap.Data[srcPos++];
                            a += k2 * bitmap.Data[srcPos];
                        }
                    }

                    // Set destination pixel
                    int dstPos = (x * newBitmap.FormatWidth) + (newBitmap.Stride * y);
                    newBitmap.Data[dstPos++] = (byte)r;
                    newBitmap.Data[dstPos++] = (byte)g;
                    newBitmap.Data[dstPos++] = (byte)b;
                    newBitmap.Data[dstPos] = (byte)a;
                }
            }

            return newBitmap;
        }

        /// <summary>
        /// BiCubic calculations.
        /// </summary>
        /// <param name="x">Value.</param>
        /// <returns>Double.</returns>
        private static double BiCubicKernel(double x)
        {
            if (x > 2.0)
                return 0.0;

            double a, b, c, d;
            double xm1 = x - 1.0;
            double xp1 = x + 1.0;
            double xp2 = x + 2.0;

            a = (xp2 <= 0.0) ? 0.0 : xp2 * xp2 * xp2;
            b = (xp1 <= 0.0) ? 0.0 : xp1 * xp1 * xp1;
            c = (x <= 0.0) ? 0.0 : x * x * x;
            d = (xm1 <= 0.0) ? 0.0 : xm1 * xm1 * xm1;

            return (0.16666666666666666667 * (a - (4.0 * b) + (6.0 * c) - (4.0 * d)));
        }

        #endregion

        #region Grayscale

        /// <summary>
        /// Creates a grayscale version of bitmap based on colour theory.
        /// </summary>
        /// <param name="bitmap">Source DFBitmap.</param>
        /// <returns>Grayscale DFBitmap image.</returns>
        public static DFBitmap MakeGrayscale(DFBitmap bitmap)
        {
            // Must be a colour format
            if (bitmap.Format == DFBitmap.Formats.Indexed)
                return null;

            DFBitmap newBitmap = new DFBitmap();
            newBitmap.Format = bitmap.Format;
            newBitmap.Initialise(bitmap.Width, bitmap.Height);

            // Make each pixel grayscale
            int srcPos = 0, dstPos = 0;
            for (int i = 0; i < bitmap.Width * bitmap.Height; i++)
            {
                // Get source color
                byte r = bitmap.Data[srcPos++];
                byte g = bitmap.Data[srcPos++];
                byte b = bitmap.Data[srcPos++];
                byte a = bitmap.Data[srcPos++];

                // Create grayscale color
                int grayscale = (int)(r * 0.3f + g * 0.59f + b * 0.11f);
                DFBitmap.DFColor dstColor = DFBitmap.DFColor.FromRGBA((byte)grayscale, (byte)grayscale, (byte)grayscale, a);

                // Write destination pixel
                newBitmap.Data[dstPos++] = dstColor.r;
                newBitmap.Data[dstPos++] = dstColor.g;
                newBitmap.Data[dstPos++] = dstColor.b;
                newBitmap.Data[dstPos++] = dstColor.a;
            }

            return newBitmap;
        }

        /// <summary>
        /// Creates average intensity bitmap.
        /// </summary>
        /// <param name="bitmap">Source DFBitmap.</param>
        /// <returns>Average intensity DFBitmap.</returns>
        public static DFBitmap MakeAverageIntensityBitmap(DFBitmap bitmap)
        {
            // Must be a colour format
            if (bitmap.Format == DFBitmap.Formats.Indexed)
                return null;

            DFBitmap newBitmap = new DFBitmap();
            newBitmap.Format = bitmap.Format;
            newBitmap.Initialise(bitmap.Width, bitmap.Height);

            // Make each pixel grayscale based on average intensity
            int srcPos = 0, dstPos = 0;
            for (int i = 0; i < bitmap.Width * bitmap.Height; i++)
            {
                // Get source color
                byte r = bitmap.Data[srcPos++];
                byte g = bitmap.Data[srcPos++];
                byte b = bitmap.Data[srcPos++];
                byte a = bitmap.Data[srcPos++];

                // Get average intensity
                int intensity = (r + g + b) / 3;
                DFBitmap.DFColor dstColor = DFBitmap.DFColor.FromRGBA((byte)intensity, (byte)intensity, (byte)intensity, a);

                // Write destination pixel
                newBitmap.Data[dstPos++] = dstColor.r;
                newBitmap.Data[dstPos++] = dstColor.g;
                newBitmap.Data[dstPos++] = dstColor.b;
                newBitmap.Data[dstPos++] = dstColor.a;
            }

            return newBitmap;
        }

        #endregion

        #region PreMultiply Alpha

        /// <summary>
        /// Pre-multiply bitmap alpha.
        /// </summary>
        /// <param name="bitmap">DFBitmap.</param>
        public static void PreMultiplyAlpha(DFBitmap bitmap)
        {
            // Must be a colour format
            if (bitmap.Format == DFBitmap.Formats.Indexed)
                return;

            // Pre-multiply alpha for each pixel
            int pos;
            float multiplier;
            for (int y = 0; y < bitmap.Height; y++)
            {
                pos = y * bitmap.Stride;
                for (int x = 0; x < bitmap.Width; x++)
                {
                    multiplier = bitmap.Data[pos + 3] / 256f;
                    bitmap.Data[pos] = (byte)(bitmap.Data[pos] * multiplier);
                    bitmap.Data[pos + 1] = (byte)(bitmap.Data[pos + 1] * multiplier);
                    bitmap.Data[pos + 2] = (byte)(bitmap.Data[pos + 2] * multiplier);
                    pos += bitmap.FormatWidth;
                }
            }
        }

        #endregion

        #region Rotate & Flip

        // Rotates a Color32 array 90 degrees counter-clockwise
        public static Color32[] RotateColors(ref Color32[] src, int width, int height)
        {
            Color32[] dst = new Color32[src.Length];

            // Rotate image data
            int srcPos = 0;
            for (int x = width - 1; x >= 0; x--)
            {
                for (int y = 0; y < height; y++)
                {
                    Color32 col = src[srcPos++];
                    dst[y * width + x] = col;
                }
            }

            return dst;
        }

        // Flips a Color32 array horizontally and vertically
        public static Color32[] FlipColors(ref Color32[] src, int width, int height)
        {
            Color32[] dst = new Color32[src.Length];

            // Flip image data
            int srcPos = 0;
            int dstPos = dst.Length - 1;
            for (int i = 0; i < width * height; i++)
            {
                Color32 col = src[srcPos++];
                dst[dstPos--] = col;
            }

            return dst;
        }

        #endregion

        #region Dilate

        /// <summary>
        /// Creates a blended border around transparent textures.
        /// Removes dark edges from billboards.
        /// </summary>
        /// <param name="colors">Source image.</param>
        /// <param name="size">Image size.</param>
        public static void DilateColors(ref Color32[] colors, DFSize size)
        {
            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    Color32 color = ReadColor(ref colors, ref size, x, y);
                    if (color.a != 0)
                    {
                        MixColor(ref colors, ref size, color, x - 1, y - 1);
                        MixColor(ref colors, ref size, color, x, y - 1);
                        MixColor(ref colors, ref size, color, x + 1, y - 1);
                        MixColor(ref colors, ref size, color, x - 1, y);
                        MixColor(ref colors, ref size, color, x + 1, y);
                        MixColor(ref colors, ref size, color, x - 1, y + 1);
                        MixColor(ref colors, ref size, color, x, y + 1);
                        MixColor(ref colors, ref size, color, x + 1, y + 1);
                    }
                }
            }
        }

        #endregion

        #region Wrap & Clamp

        /// <summary>
        /// Wraps texture into emtpty border area on opposite side.
        /// </summary>
        /// <param name="colors">Source image.</param>
        /// <param name="size">Image size.</param>
        /// <param name="border">Border width.</param>
        public static void WrapBorder(ref Color32[] colors, DFSize size, int border, bool leftRight = true, bool topBottom = true)
        {
            // Wrap left-right
            if (leftRight)
            {
                for (int y = border; y < size.Height - border; y++)
                {
                    int ypos = y * size.Width;
                    int il = ypos + border;
                    int ir = ypos + size.Width - border * 2;
                    for (int x = 0; x < border; x++)
                    {
                        colors[ypos + x] = colors[ir + x];
                        colors[ypos + size.Width - border + x] = colors[il + x];
                    }
                }
            }

            // Wrap top-bottom
            if (topBottom)
            {
                for (int y = 0; y < border; y++)
                {
                    int ypos1 = y * size.Width;
                    int ypos2 = (y + size.Height - border) * size.Width;

                    int it = (border + y) * size.Width;
                    int ib = (y + size.Height - border * 2) * size.Width;
                    for (int x = 0; x < size.Width; x++)
                    {
                        colors[ypos1 + x] = colors[ib + x];
                        colors[ypos2 + x] = colors[it + x];
                    }
                }
            }
        }

        /// <summary>
        /// Clamps texture into empty border area.
        /// </summary>
        /// <param name="colors">Source image.</param>
        /// <param name="size">Image size.</param>
        /// <param name="border">Border width.</param>
        public static void ClampBorder(
            ref Color32[] colors,
            DFSize size,
            int border,
            bool leftRight = true,
            bool topBottom = true,
            bool topLeft = true,
            bool topRight = true,
            bool bottomLeft = true,
            bool bottomRight = true)
        {
            if (leftRight)
            {
                // Clamp left-right
                for (int y = border; y < size.Height - border; y++)
                {
                    int ypos = y * size.Width;
                    Color32 leftColor = colors[ypos + border];
                    Color32 rightColor = colors[ypos + size.Width - border - 1];

                    int il = ypos;
                    int ir = ypos + size.Width - border;
                    for (int x = 0; x < border; x++)
                    {
                        colors[il + x] = leftColor;
                        colors[ir + x] = rightColor;
                    }
                }
            }

            if (topBottom)
            {
                // Clamp top-bottom
                for (int x = border; x < size.Width - border; x++)
                {
                    Color32 topColor = colors[((border) * size.Width) + x];
                    Color32 bottomColor = colors[(size.Height - border - 1) * size.Width + x];

                    for (int y = 0; y < border; y++)
                    {
                        int it = y * size.Width + x;
                        int ib = (size.Height - y - 1) * size.Width + x;

                        colors[it] = topColor;
                        colors[ib] = bottomColor;
                    }
                }
            }

            if (topLeft)
            {
                // Clamp top-left
                Color32 topLeftColor = colors[(border + 1) * size.Width + border];
                for (int y = 0; y < border; y++)
                {
                    for (int x = 0; x < border; x++)
                    {
                        colors[y * size.Width + x] = topLeftColor;
                    }
                }
            }

            if (topRight)
            {
                // Clamp top-right
                Color32 topRightColor = colors[(border + 1) * size.Width + size.Width - border - 1];
                for (int y = 0; y < border; y++)
                {
                    for (int x = size.Width - border; x < size.Width; x++)
                    {
                        colors[y * size.Width + x] = topRightColor;
                    }
                }
            }

            if (bottomLeft)
            {
                // Clamp bottom-left
                Color32 bottomLeftColor = colors[(size.Height - border - 1) * size.Width + border];
                for (int y = size.Height - border; y < size.Height; y++)
                {
                    for (int x = 0; x < border; x++)
                    {
                        colors[y * size.Width + x] = bottomLeftColor;
                    }
                }
            }

            if (bottomRight)
            {
                // Clamp bottom-right
                Color32 bottomRightColor = colors[(size.Height - border - 1) * size.Width + size.Width - border - 1];
                for (int y = size.Height - border; y < size.Height; y++)
                {
                    for (int x = size.Width - border; x < size.Width; x++)
                    {
                        colors[y * size.Width + x] = bottomRightColor;
                    }
                }
            }
        }

        #endregion

        #region Weapon Tinting

        /// <summary>
        /// Tint a weapon image based on metal type.
        /// </summary>
        /// <param name="srcBitmap">Source weapon image.</param>
        /// <param name="size">Image size.</param>
        /// <param name="metalType">Metal type for tint.</param>
        public static void TintWeaponImage(DFBitmap srcBitmap, MetalTypes metalType)
        {
            // Must be indexed format
            if (srcBitmap.Format != DFBitmap.Formats.Indexed)
                return;

            byte[] swaps = GetMetalColors(metalType);

            int rowPos;
            for (int y = 0; y < srcBitmap.Height; y++)
            {
                rowPos = y * srcBitmap.Width;
                for (int x = 0; x < srcBitmap.Width; x++)
                {
                    byte index = srcBitmap.Data[rowPos + x];
                    if (index >= 0x70 && index <= 0x7f)
                    {
                        int offset = index - 0x70;
                        srcBitmap.Data[rowPos + x] = swaps[offset];
                    }
                }
            }
        }

        // Gets colour indices based on metal type
        public static byte[] GetMetalColors(MetalTypes metalType)
        {
            byte[] indices;
            switch (metalType)
            {
                case MetalTypes.Iron:
                    indices = new byte[] { 0x77, 0x78, 0x57, 0x79, 0x58, 0x59, 0x7A, 0x5A, 0x7B, 0x5B, 0x7C, 0x5C, 0x7D, 0x5D, 0x5E, 0x5F };
                    break;
                case MetalTypes.Steel:
                    indices = new byte[] { 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F };
                    break;
                case MetalTypes.Silver:
                    indices = new byte[] { 0xE0, 0x70, 0x50, 0x71, 0x51, 0x72, 0x73, 0x52, 0x74, 0x53, 0x75, 0x54, 0x55, 0x56, 0x57, 0x58 };
                    break;
                case MetalTypes.Elven:
                    indices = new byte[] { 0xE0, 0x70, 0x50, 0x71, 0x51, 0x72, 0x73, 0x52, 0x74, 0x53, 0x75, 0x54, 0x55, 0x56, 0x57, 0x58 };
                    break;
                case MetalTypes.Dwarven:
                    indices = new byte[] { 0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0x9B, 0x9C, 0x9D, 0x9E, 0x9F };
                    break;
                case MetalTypes.Mithril:
                    indices = new byte[] { 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE };
                    break;
                case MetalTypes.Adamantium:
                    indices = new byte[] { 0x5A, 0x5B, 0x7C, 0x5C, 0x7D, 0x5D, 0x7E, 0x5E, 0x7F, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE };
                    break;
                case MetalTypes.Ebony:
                    indices = new byte[] { 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE };
                    break;
                case MetalTypes.Orcish:
                    indices = new byte[] { 0xA2, 0xA3, 0xC8, 0xC9, 0xCA, 0xCB, 0xCC, 0xCD, 0xCE, 0xCF, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD };
                    break;
                case MetalTypes.Daedric:
                    indices = new byte[] { 0xEF, 0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE };
                    break;
                default:
                    indices = new byte[] { 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F };
                    break;
            }

            return indices;
        }

        #endregion

        #region Edge Detection

        /*
         * Based on Craig's Utility Library (CUL) by James Craig.
         * http://www.gutgames.com/post/Edge-detection-in-C.aspx
         * MIT License (http://www.opensource.org/licenses/mit-license.php)
         */

        /// <summary>
        /// Gets a new bitmap containing edges detected in source bitmap.
        /// The source bitmap is unchanged.
        /// </summary>
        /// <param name="bitmap">DFBitmap source.</param>
        /// <param name="threshold">Edge detection threshold.</param>
        /// <param name="edgeColor">Edge colour to write.</param>
        /// <returns>DFBitmap containing edges.</returns>
        private static DFBitmap FindEdges(DFBitmap bitmap, float threshold, DFBitmap.DFColor edgeColor)
        {
            // Must be a colour format
            if (bitmap.Format == DFBitmap.Formats.Indexed)
                return null;

            // Clone bitmap settings
            DFBitmap newBitmap = DFBitmap.CloneDFBitmap(bitmap);

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    DFBitmap.DFColor currentColor = DFBitmap.GetPixel(bitmap, x, y);
                    if (y < newBitmap.Height - 1 && x < newBitmap.Width - 1)
                    {
                        DFBitmap.DFColor tempColor = DFBitmap.GetPixel(bitmap, x + 1, y + 1);
                        if (ColorDistance(currentColor, tempColor) > threshold)
                            DFBitmap.SetPixel(newBitmap, x, y, edgeColor);
                    }
                    else if (y < newBitmap.Height - 1)
                    {
                        DFBitmap.DFColor tempColor = DFBitmap.GetPixel(bitmap, x, y + 1);
                        if (ColorDistance(currentColor, tempColor) > threshold)
                            DFBitmap.SetPixel(newBitmap, x, y, edgeColor);
                    }
                    else if (x < newBitmap.Width - 1)
                    {
                        DFBitmap.DFColor tempColor = DFBitmap.GetPixel(bitmap, x + 1, y);
                        if (ColorDistance(currentColor, tempColor) > threshold)
                            DFBitmap.SetPixel(newBitmap, x, y, edgeColor);
                    }
                }
            }

            return newBitmap;
        }

        /// <summary>
        /// Gets distance between two colours using Euclidean distance function.
        /// Distance = SQRT( (R1-R2)2 + (G1-G2)2 + (B1-B2)2 )
        /// </summary>
        /// <param name="color1">First Color.</param>
        /// <param name="color2">Second Color.</param>
        /// <returns>Distance between colours.</returns>
        private static float ColorDistance(DFBitmap.DFColor color1, DFBitmap.DFColor color2)
        {
            float r = color1.r - color2.r;
            float g = color1.g - color2.g;
            float b = color1.b - color2.b;

            return (float)Mathf.Sqrt((r * r) + (g * g) + (b * b));
        }

        #endregion

        #region Matrix Convolution Filter

        /*
         * Based on Craig's Utility Library (CUL) by James Craig.
         * http://www.gutgames.com/post/Matrix-Convolution-Filters-in-C.aspx
         * MIT License (http://www.opensource.org/licenses/mit-license.php)
         */

        /// <summary>
        /// Used when applying convolution filters to an image.
        /// </summary>
        public class Filter
        {
            #region Constructors
            /// <summary>
            /// Constructor
            /// </summary>
            public Filter()
            {
                MyFilter = new int[3, 3];
                Width = 3;
                Height = 3;
                Offset = 0;
                Absolute = false;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="Width">Width</param>
            /// <param name="Height">Height</param>
            public Filter(int Width, int Height)
            {
                MyFilter = new int[Width, Height];
                this.Width = Width;
                this.Height = Height;
                Offset = 0;
                Absolute = false;
            }
            #endregion

            #region Public Properties
            /// <summary>
            /// The actual filter array
            /// </summary>
            public int[,] MyFilter { get; set; }

            /// <summary>
            /// Width of the filter box
            /// </summary>
            public int Width { get; set; }

            /// <summary>
            /// Height of the filter box
            /// </summary>
            public int Height { get; set; }

            /// <summary>
            /// Amount to add to the red, blue, and green values
            /// </summary>
            public int Offset { get; set; }

            /// <summary>
            /// Determines if we should take the absolute value prior to clamping
            /// </summary>
            public bool Absolute { get; set; }
            #endregion

            #region Public Methods
            /// <summary>
            /// Applies the filter to the input image
            /// </summary>
            /// <param name="Input">input image</param>
            /// <returns>Returns a separate image with the filter applied</returns>
            public DFBitmap ApplyFilter(DFBitmap Input)
            {
                // Must be a colour format
                if (Input.Format == DFBitmap.Formats.Indexed)
                    return null;

                DFBitmap NewBitmap = DFBitmap.CloneDFBitmap(Input);
                for (int x = 0; x < Input.Width; ++x)
                {
                    for (int y = 0; y < Input.Height; ++y)
                    {
                        int RValue = 0;
                        int GValue = 0;
                        int BValue = 0;
                        int AValue = 0;
                        int Weight = 0;
                        int XCurrent = -Width / 2;
                        for (int x2 = 0; x2 < Width; ++x2)
                        {
                            if (XCurrent + x < Input.Width && XCurrent + x >= 0)
                            {
                                int YCurrent = -Height / 2;
                                for (int y2 = 0; y2 < Height; ++y2)
                                {
                                    if (YCurrent + y < Input.Height && YCurrent + y >= 0)
                                    {
                                        DFBitmap.DFColor Pixel = DFBitmap.GetPixel(Input, XCurrent + x, YCurrent + y);
                                        RValue += MyFilter[x2, y2] * Pixel.r;
                                        GValue += MyFilter[x2, y2] * Pixel.g;
                                        BValue += MyFilter[x2, y2] * Pixel.b;
                                        AValue = Pixel.a;
                                        Weight += MyFilter[x2, y2];
                                    }
                                    ++YCurrent;
                                }
                            }
                            ++XCurrent;
                        }

                        DFBitmap.DFColor MeanPixel = DFBitmap.GetPixel(Input, x, y);
                        if (Weight == 0)
                            Weight = 1;
                        if (Weight > 0)
                        {
                            if (Absolute)
                            {
                                RValue = System.Math.Abs(RValue);
                                GValue = System.Math.Abs(GValue);
                                BValue = System.Math.Abs(BValue);
                            }

                            RValue = (RValue / Weight) + Offset;
                            RValue = Clamp(RValue, 0, 255);
                            GValue = (GValue / Weight) + Offset;
                            GValue = Clamp(GValue, 0, 255);
                            BValue = (BValue / Weight) + Offset;
                            BValue = Clamp(BValue, 0, 255);
                            MeanPixel = DFBitmap.DFColor.FromRGBA((byte)RValue, (byte)GValue, (byte)BValue, (byte)AValue);
                        }

                        DFBitmap.SetPixel(NewBitmap, x, y, MeanPixel);
                    }
                }

                return NewBitmap;
            }
            #endregion
        }

        #endregion

        #region Sharpening

        /// <summary>
        /// Sharpen a DFBitmap.
        /// </summary>
        /// <param name="bitmap">Source DFBitmap.</param>
        /// <param name="passes">Number of sharpen passes.</param>
        /// <returns>Sharpened DFBitmap.</returns>
        public static DFBitmap Sharpen(DFBitmap bitmap, int passes = 1)
        {
            // Must be a colour format
            if (bitmap.Format == DFBitmap.Formats.Indexed)
                return null;

            DFBitmap newBitmap = DFBitmap.CloneDFBitmap(bitmap);

            for (int i = 0; i < passes; i++)
            {
                // Create horizontal sobel matrix
                Filter sharpenMatrix = new Filter(3, 3);
                sharpenMatrix.MyFilter[0, 0] = -1;
                sharpenMatrix.MyFilter[1, 0] = -1;
                sharpenMatrix.MyFilter[2, 0] = -1;
                sharpenMatrix.MyFilter[0, 1] = 1;
                sharpenMatrix.MyFilter[1, 1] = 12;
                sharpenMatrix.MyFilter[2, 1] = 1;
                sharpenMatrix.MyFilter[0, 2] = -1;
                sharpenMatrix.MyFilter[1, 2] = -1;
                sharpenMatrix.MyFilter[2, 2] = -1;
                newBitmap = sharpenMatrix.ApplyFilter(newBitmap);
            }

            return newBitmap;
        }

        #endregion

        #region Bump Map

        /// <summary>
        /// Gets a bump map from the source DFBitmap.
        /// </summary>
        /// <param name="bitmap">Source colour image.</param>
        /// <returns>DFBitmap bump image.</returns>
        public static DFBitmap GetBumpMap(DFBitmap bitmap)
        {
            // Must be a colour format
            if (bitmap.Format == DFBitmap.Formats.Indexed)
                return null;

            // Create horizontal sobel matrix
            Filter horizontalMatrix = new Filter(3, 3);
            horizontalMatrix.MyFilter[0, 0] = -1;
            horizontalMatrix.MyFilter[1, 0] = 0;
            horizontalMatrix.MyFilter[2, 0] = 1;
            horizontalMatrix.MyFilter[0, 1] = -2;
            horizontalMatrix.MyFilter[1, 1] = 0;
            horizontalMatrix.MyFilter[2, 1] = 2;
            horizontalMatrix.MyFilter[0, 2] = -1;
            horizontalMatrix.MyFilter[1, 2] = 0;
            horizontalMatrix.MyFilter[2, 2] = 1;

            // Create vertical sobel matrix
            Filter verticalMatrix = new Filter(3, 3);
            verticalMatrix.MyFilter[0, 0] = 1;
            verticalMatrix.MyFilter[1, 0] = 2;
            verticalMatrix.MyFilter[2, 0] = 1;
            verticalMatrix.MyFilter[0, 1] = 0;
            verticalMatrix.MyFilter[1, 1] = 0;
            verticalMatrix.MyFilter[2, 1] = 0;
            verticalMatrix.MyFilter[0, 2] = -1;
            verticalMatrix.MyFilter[1, 2] = -2;
            verticalMatrix.MyFilter[2, 2] = -1;

            // Get filtered images
            DFBitmap horz = MakeAverageIntensityBitmap(horizontalMatrix.ApplyFilter(bitmap));
            DFBitmap vert = MakeAverageIntensityBitmap(verticalMatrix.ApplyFilter(bitmap));

            // Create target bitmap
            DFBitmap result = new DFBitmap();
            result.Format = bitmap.Format;
            result.Initialise(horz.Width, horz.Height);

            // Merge
            int pos = 0;
            for (int i = 0; i < bitmap.Width * bitmap.Height; i++)
            {
                // Merge average intensity
                int r = (horz.Data[pos + 0] + vert.Data[pos + 0]) / 2;
                int g = (horz.Data[pos + 1] + vert.Data[pos + 1]) / 2;
                int b = (horz.Data[pos + 2] + vert.Data[pos + 2]) / 2;

                // Write destination pixel
                result.Data[pos + 0] = (byte)((r > 255) ? 255 : r);
                result.Data[pos + 1] = (byte)((g > 255) ? 255 : g);
                result.Data[pos + 2] = (byte)((b > 255) ? 255 : b);
                result.Data[pos + 3] = 255;

                pos += bitmap.FormatWidth;
            }

            return result;
        }

        #endregion

        #region Normal Map

        /// <summary>
        /// Converts a bump map to a normal map.
        /// </summary>
        /// <param name="bitmap">Source bump map image.</param>
        /// <param name="strength">Normal strength.</param>
        /// <returns>DFBitmap normal image.</returns>
        public static DFBitmap ConvertBumpToNormals(DFBitmap bitmap, float strength = 1)
        {
            // Must be a colour format
            if (bitmap.Format == DFBitmap.Formats.Indexed)
                return null;

            DFBitmap newBitmap = new DFBitmap(bitmap.Width, bitmap.Height);
            newBitmap.Format = bitmap.Format;
            newBitmap.Initialise(bitmap.Width, bitmap.Height);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    // Look up the heights to either side of this pixel
                    float left = GetIntensity(bitmap, x - 1, y);
                    float right = GetIntensity(bitmap, x + 1, y);
                    float top = GetIntensity(bitmap, x, y - 1);
                    float bottom = GetIntensity(bitmap, x, y + 1);

                    // Compute gradient vectors, then cross them to get the normal
                    Vector3 dx = new Vector3(1, 0, (right - left) * strength);
                    Vector3 dy = new Vector3(0, 1, (bottom - top) * strength);
                    Vector3 normal = Vector3.Cross(dx, dy);
                    normal.Normalize();

                    // Store result
                    int pos = y * bitmap.Stride + x * bitmap.FormatWidth;
                    newBitmap.Data[pos + 0] = (byte)((normal.x + 1.0f) * 127.5f);
                    newBitmap.Data[pos + 1] = (byte)((normal.y + 1.0f) * 127.5f);
                    newBitmap.Data[pos + 2] = (byte)((normal.z + 1.0f) * 127.5f);
                    newBitmap.Data[pos + 3] = 0xff;
                }
            }

            return newBitmap;
        }

        #endregion

        #region Fonts

        // Gets a glyph from FntFile as Color32 array
        public static Color32[] GetGlyphColors(FntFile fntFile, int index, Color backColor, Color textColor, out Rect sizeOut)
        {
            // Get actual glyph rect
            sizeOut = new Rect(0, 0, fntFile.GetGlyphWidth(index), fntFile.FixedHeight);

            // Get glyph byte data as color array
            byte[] data = fntFile.GetGlyphPixels(index);
            Color32[] colors = new Color32[data.Length];
            for (int y = 0; y < FntFile.GlyphFixedDimension; y++)
            {
                for (int x = 0; x < FntFile.GlyphFixedDimension; x++)
                {
                    int pos = y * FntFile.GlyphFixedDimension + x;
                    if (data[pos] > 0)
                        colors[pos] = textColor;
                    else
                        colors[pos] = backColor;
                }
            }

            return colors;
        }

        // Creates a font atlas from FntFile
        public static void CreateFontAtlas(FntFile fntFile, Color backColor, Color textColor, out Texture2D atlasTextureOut, out Rect[] atlasRectsOut)
        {
            const int atlasDim = 256;

            // Create atlas colors array
            Color32[] atlasColors = new Color32[atlasDim * atlasDim];

            // Add Daggerfall glyphs
            int xpos = 0, ypos = 0;
            Rect[] rects = new Rect[FntFile.MaxGlyphCount];
            for (int i = 0; i < FntFile.MaxGlyphCount; i++)
            {
                // Get glyph colors
                Rect rect;
                Color32[] glyphColors = GetGlyphColors(fntFile, i, backColor, textColor, out rect);

                // Offset pixel rect
                rect.x += xpos;
                rect.y += ypos;

                // Flip pixel rect top-bottom
                float top = rect.yMin;
                float bottom = rect.yMax;
                rect.yMin = bottom;
                rect.yMax = top;

                // Convert to UV coords and store
                rect.xMin /= atlasDim;
                rect.yMin /= atlasDim;
                rect.xMax /= atlasDim;
                rect.yMax /= atlasDim;
                rects[i] = rect;

                // Insert into atlas
                InsertColors(
                    ref glyphColors,
                    ref atlasColors,
                    xpos,
                    ypos,
                    FntFile.GlyphFixedDimension,
                    FntFile.GlyphFixedDimension,
                    atlasDim,
                    atlasDim);

                // Offset position
                xpos += FntFile.GlyphFixedDimension;
                if (xpos >= atlasDim)
                {
                    xpos = 0;
                    ypos += FntFile.GlyphFixedDimension;
                }
            }

            // Create texture from colors array
            atlasTextureOut = MakeTexture2D(ref atlasColors, atlasDim, atlasDim, TextureFormat.ARGB32, false);
            atlasRectsOut = rects;
        }

        #endregion

        #region Private Methods

        private static void MixColor(ref Color32[] colors, ref DFSize size, Color32 src, int x, int y)
        {
            // Handle outside of bounds
            if (x < 0 || y < 0 || x > size.Width - 1 || y > size.Height - 1)
                return;

            // Get destination pixel colour and ensure it has empty alpha
            Color32 dst = ReadColor(ref colors, ref size, x, y);
            if (dst.a != 0)
                return;

            // Get count for averaging
            int count = 1;
            if (dst != Color.clear)
                count = 2;

            // Mix source colour with destination
            Vector3 avg = new Vector3(
                src.r + dst.r,
                src.g + dst.g,
                src.b + dst.b) / count;

            // Assign new colour to destination
            colors[y * size.Width + x] = new Color32((byte)avg.x, (byte)avg.y, (byte)avg.z, 0);
        }

        private static Color32 ReadColor(ref Color32[] colors, ref DFSize size, int x, int y)
        {
            // Handle outside of bounds
            if (x < 0 || y < 0 || x > size.Width - 1 || y > size.Height - 1)
                return Color.clear;

            return colors[y * size.Width + x];
        }

        private static float GetIntensity(DFBitmap bitmap, int x, int y)
        {
            // Clamp X
            if (x < 0)
                x = 0;
            else if (x >= bitmap.Width)
                x = bitmap.Width - 1;

            // Clamp Y
            if (y < 0)
                y = 0;
            else if (y >= bitmap.Height)
                y = bitmap.Height - 1;

            // Get position
            int pos = y * bitmap.Stride + x * bitmap.FormatWidth;

            // Average intensity
            float intensity = ((bitmap.Data[pos + 0] + bitmap.Data[pos + 1] + bitmap.Data[pos + 2]) / 3) / 255f;

            return intensity;
        }

        #endregion
    }
}