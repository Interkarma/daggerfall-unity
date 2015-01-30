// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using DaggerfallConnect;
using DaggerfallConnect.Utility;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Basic image processing for textures.
    /// </summary>
    public static class ImageProcessing
    {
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

        // Helper to create and apply Texture2D from single call
        public static Texture2D MakeTexture2D(ref Color32[] src, int width, int height, TextureFormat textureFormat, bool mipMaps)
        {
            Texture2D texture = new Texture2D(width, height, textureFormat, mipMaps);
            texture.SetPixels32(src);
            texture.Apply(true);

            return texture;
        }

        /// <summary>
        /// Tint a weapon image based on metal type.
        /// </summary>
        /// <param name="srcBitmap">Source weapon image.</param>
        /// <param name="size">Image size.</param>
        /// <param name="metalType">Metal type for tint.</param>
        public static void TintWeaponImage(ref DFBitmap srcBitmap, MetalTypes metalType)
        {
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

        #endregion
    }
}