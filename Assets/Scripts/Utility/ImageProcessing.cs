using UnityEngine;
using System.IO;
using System.Collections;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Basic image processing.
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

        // Copies a subset of Color32 array into XY position of another Color32 array
        public static void CopyColors(ref Color32[] src, ref Color32[] dst, DFSize srcSize, DFSize dstSize, DFPosition srcPos, DFPosition dstPos, DFSize copySize)
        {
            for (int y = 0; y < copySize.Height; y++)
            {
                for (int x = 0; x < copySize.Width; x++)
                {
                    Color32 col = src[(srcPos.Y + y) * srcSize.Width + (srcPos.X + x)];
                    dst[(dstPos.Y + y) * dstSize.Width + (dstPos.X + x)] = col;
                }
            }
        }

        // Helper to create and apply Texture2D from single call
        public static Texture2D MakeTexture2D(ref Color32[] src, int width, int height, TextureFormat textureFormat, bool mipMaps)
        {
            Texture2D texture = new Texture2D(width, height, textureFormat, mipMaps);
            texture.SetPixels32(src);
            texture.Apply(mipMaps);

            return texture;
        }

#if UNITY_EDITOR && !UNITY_WEBPLAYER
        // Helper to save Texture2D to a PNG file
        public static void SaveTextureAsPng(Texture2D texture, string path)
        {
            // Path must end with PNG
            if (!path.EndsWith(".png", System.StringComparison.InvariantCultureIgnoreCase))
                path += ".png";

            // Encode and save file
            byte[] buffer = texture.EncodeToPNG();
            File.WriteAllBytes(path, buffer);

            DaggerfallUnity.LogMessage(string.Format("Saved texture to {0}", path));
        }
#endif

        #endregion

        #region Grayscale

        /// <summary>
        /// Creates average intensity Color32 array.
        /// </summary>
        /// <param name="colors">Source Color32 array.</param>
        /// <returns>Average intensity Color32 array.</returns>
        public static Color32[] MakeAverageIntensity(Color32[] colors)
        {
            Color32[] newColors = (Color32[])colors.Clone();

            // Make each pixel grayscale based on average intensity
            for (int i = 0; i < colors.Length; i++)
            {
                // Get average intensity
                Color32 srcColor = colors[i];
                int intensity = (srcColor.r + srcColor.g + srcColor.b) / 3;
                Color32 dstColor = new Color32((byte)intensity, (byte)intensity, (byte)intensity, srcColor.a);
                newColors[i] = dstColor;
            }

            return newColors;
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

        // Flips a Color32 array horizontally
        public static Color32[] FlipHorizontallyColors(ref Color32[] src, int width, int height)
        {
            Color32[] dst = new Color32[src.Length];

            // Flip image data horizontally
            int srcPos = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = width - 1; x >= 0; x--)
                {
                    Color32 col = src[srcPos++];
                    dst[y * width + x] = col;
                }
            }

            return dst;
        }

        // Flips a Color32 array vertically
        public static Color32[] FlipVerticallyColors(ref Color32[] src, int width, int height)
        {
            Color32[] dst = new Color32[src.Length];

            // Flip image data vertically
            int srcPos = 0;
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
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

        #region Negative

        /// <summary>
        /// Creates a negative of image.
        /// </summary>
        /// <param name="colors">Source Color32 array.</param>
        public static void Negative(ref Color32[] colors)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                Color32 pixel = colors[i];
                colors[i] = new Color32((byte)(255 - pixel.r), (byte)(255 - pixel.g), (byte)(255 - pixel.b), pixel.a);
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

        #region Dyes & Masks

        /// <summary>
        /// Changes mask index to another index.
        /// </summary>
        /// <param name="srcBitmap">Source image.</param>
        /// <param name="replaceWith">Index to substitute for mask index.</param>
        /// <returns>New DFBitmap with modified mask.</returns>
        public static DFBitmap ChangeMask(DFBitmap srcBitmap, byte replaceWith = 0)
        {
            const int mask = 0xff;

            // Handle null bitmap or data input
            if (srcBitmap == null || srcBitmap.Data == null)
                return new DFBitmap();

            // Clone bitmap
            DFBitmap dstBitmap = DFBitmap.CloneDFBitmap(srcBitmap, false);

            // Remove all instances of mask index
            for (int i = 0; i < srcBitmap.Data.Length; i++)
            {
                byte index = srcBitmap.Data[i];
                if (index == mask)
                    dstBitmap.Data[i] = replaceWith;
                else
                    dstBitmap.Data[i] = index;
            }

            return dstBitmap;
        }

        /// <summary>
        /// Dye supported bitmap image based on dye index and type.
        /// </summary>
        /// <param name="srcBitmap">Source image.</param>
        /// <param name="dye">Dye index.</param>
        /// <param name="target">Dye target.</param>
        /// <returns>New DFBitmap dyed to specified colour.</returns>
        public static DFBitmap ChangeDye(DFBitmap srcBitmap, DyeColors dye, DyeTargets target)
        {
            const int clothingStart = 0x60;
            const int weaponsAndArmorStart = 0x70;

            // Clone bitmap and get colour table for swaps
            DFBitmap dstBitmap = DFBitmap.CloneDFBitmap(srcBitmap, false);
            byte[] swaps = GetDyeColorTable(dye, target);

            // Swaps range is based on target type
            int start;
            switch(target)
            {
                case DyeTargets.Clothing:
                    start = clothingStart;
                    break;
                case DyeTargets.WeaponsAndArmor:
                    start = weaponsAndArmorStart;
                    break;
                default:
                    return dstBitmap;
            }

            // Swap indices start through start + 15 with colour table
            int rowPos;
            for (int y = 0; y < srcBitmap.Height; y++)
            {
                rowPos = y * srcBitmap.Width;
                for (int x = 0; x < srcBitmap.Width; x++)
                {
                    int srcOffset = rowPos + x;
                    byte index = srcBitmap.Data[srcOffset];

                    if (index >= start && index <= start + 0x0f)
                    {
                        int tintOffset = index - start;
                        dstBitmap.Data[srcOffset] = swaps[tintOffset];
                    }
                    else
                    {
                        dstBitmap.Data[srcOffset] = index;
                    }
                }
            }

            return dstBitmap;
        }

        /// <summary>
        /// Gets colour table for all supported dyes.
        /// </summary>
        public static byte[] GetDyeColorTable(DyeColors dye, DyeTargets target)
        {
            // Dye targets require slightly different handling
            if (target == DyeTargets.Clothing)
            {
                // Clothing
                int start = 0x60;
                byte[] swaps = new byte[16];
                switch (dye)
                {
                    case DyeColors.Blue:
                        start = 0x60;
                        break;
                    case DyeColors.Grey:
                        start = 0x50;
                        break;
                    case DyeColors.Red:
                        start = 0xEF;
                        break;
                    case DyeColors.DarkBrown:
                        start = 0x20;
                        break;
                    case DyeColors.Purple:
                        start = 0x30;
                        break;
                    case DyeColors.LightBrown:
                        start = 0x40;
                        break;
                    case DyeColors.White:
                        start = 0x70;
                        break;
                    case DyeColors.Aquamarine:
                        start = 0x80;
                        break;
                    case DyeColors.Yellow:
                        start = 0x90;
                        break;
                    case DyeColors.Green:
                        start = 0xA0;
                        break;
                    default:
                        start = 0x60;
                        break;
                }

                // Geneate index swaps
                for (int i = 0; i < 16; i++)
                {
                    swaps[i] = (byte)(start + i);
                }

                return swaps;
            }
            else if (target == DyeTargets.WeaponsAndArmor)
            {
                // Metals
                switch (dye)
                {
                    case DyeColors.Iron:
                        return GetMetalColorTable(MetalTypes.Iron);
                    case DyeColors.Steel:
                        return GetMetalColorTable(MetalTypes.Steel);
                    case DyeColors.Silver:
                        return GetMetalColorTable(MetalTypes.Silver);
                    case DyeColors.Elven:
                        return GetMetalColorTable(MetalTypes.Elven);
                    case DyeColors.Dwarven:
                        return GetMetalColorTable(MetalTypes.Dwarven);
                    case DyeColors.Mithril:
                        return GetMetalColorTable(MetalTypes.Mithril);
                    case DyeColors.Adamantium:
                        return GetMetalColorTable(MetalTypes.Adamantium);
                    case DyeColors.Ebony:
                        return GetMetalColorTable(MetalTypes.Ebony);
                    case DyeColors.Orcish:
                        return GetMetalColorTable(MetalTypes.Orcish);
                    case DyeColors.Daedric:
                        return GetMetalColorTable(MetalTypes.Daedric);
                    default:
                        return GetMetalColorTable(MetalTypes.None);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets colour table for each metal type.
        /// </summary>
        public static byte[] GetMetalColorTable(MetalTypes metalType)
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
                    indices = new byte[] { 0xE0, 0x70, 0x50, 0x71, 0x51, 0x72, 0x73, 0x52, 0x74, 0x53, 0x75, 0x54, 0x76, 0x56, 0x77, 0x78 };
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
                    indices = new byte[] { 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xCB, 0xCC, 0xCD, 0xCE, 0xCF, 0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD };
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

        /// <summary>
        /// Converts a DaggerfallUnity metal type to dye colour.
        /// </summary>
        public static DyeColors GetMetalDyeColor(MetalTypes metalType)
        {
            switch(metalType)
            {
                case MetalTypes.Iron:
                    return DyeColors.Iron;
                case MetalTypes.Steel:
                    return DyeColors.Steel;
                case MetalTypes.Silver:
                    return DyeColors.Silver;
                case MetalTypes.Elven:
                    return DyeColors.Elven;
                case MetalTypes.Dwarven:
                    return DyeColors.Dwarven;
                case MetalTypes.Mithril:
                    return DyeColors.Mithril;
                case MetalTypes.Adamantium:
                    return DyeColors.Adamantium;
                case MetalTypes.Ebony:
                    return DyeColors.Ebony;
                case MetalTypes.Orcish:
                    return DyeColors.Orcish;
                case MetalTypes.Daedric:
                    return DyeColors.Daedric;
                default:
                    return DyeColors.Unchanged;
            }
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
                FilterWidth = 3;
                FilterHeight = 3;
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
                FilterWidth = Width;
                FilterHeight = Height;
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
            public int FilterWidth { get; set; }

            /// <summary>
            /// Height of the filter box
            /// </summary>
            public int FilterHeight { get; set; }

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
            /// Applies filter to input Color32 array.
            /// </summary>
            /// <param name="colors">Source Color32 array.</param>
            /// <param name="width">Image width.</param>
            /// <param name="height">Image height.</param>
            /// <returns>New Color32 array with filter applied.</returns>
            public Color32[] ApplyFilter(Color32[] colors, int width, int height)
            {
                Color32[] newColors = (Color32[])colors.Clone();
                for (int x = 0; x < width; ++x)
                {
                    for (int y = 0; y < height; ++y)
                    {
                        int weight = 0;
                        int r = 0, g = 0, b = 0, a = 0;

                        int curX = -FilterWidth / 2;
                        for (int x2 = 0; x2 < FilterWidth; ++x2)
                        {
                            int sampleX = curX + x;                     // Sample point is wrapped to avoid seams in edge-finding filters
                            if (sampleX < 0) sampleX = width - 1;
                            if (sampleX >= width) sampleX = 0;

                            int curY = -FilterHeight / 2;
                            for (int y2 = 0; y2 < FilterHeight; ++y2)
                            {
                                int sampleY = curY + y;                 // Sample point is wrapped to avoid seams in edge-finding filters
                                if (sampleY < 0) sampleY = height - 1;
                                if (sampleY >= height) sampleY = 0;

                                Color32 pixel = colors[sampleY * width + sampleX];
                                r += MyFilter[x2, y2] * pixel.r;
                                g += MyFilter[x2, y2] * pixel.g;
                                b += MyFilter[x2, y2] * pixel.b;
                                a = pixel.a;
                                weight += MyFilter[x2, y2];

                                ++curY;
                            }

                            ++curX;
                        }

                        Color32 meanPixel = colors[y * width + x];
                        if (weight == 0)
                            weight = 1;
                        if (weight > 0)
                        {
                            if (Absolute)
                            {
                                r = System.Math.Abs(r);
                                g = System.Math.Abs(g);
                                b = System.Math.Abs(b);
                            }

                            r = (r / weight) + Offset;
                            r = Clamp(r, 0, 255);
                            g = (g / weight) + Offset;
                            g = Clamp(g, 0, 255);
                            b = (b / weight) + Offset;
                            b = Clamp(b, 0, 255);
                            meanPixel = new Color32((byte)r, (byte)g, (byte)b, (byte)a);
                        }

                        newColors[y * width + x] = meanPixel;
                    }
                }

                return newColors;
            }
            #endregion
        }

        #endregion

        #region Sharpening

        /// <summary>
        /// Sharpen a DFBitmap.
        /// </summary>
        /// <param name="colors">Source Color32 array.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <returns>Sharpened image.</returns>
        public static Color32[] Sharpen(ref Color32[] colors, int width, int height)
        {
            // Create sharpen matrix
            Filter sharpenMatrix = new Filter(3, 3);
            sharpenMatrix.MyFilter[0, 0] = -1;
            sharpenMatrix.MyFilter[0, 1] = -1;
            sharpenMatrix.MyFilter[0, 2] = -1;
            sharpenMatrix.MyFilter[1, 0] = 1;
            sharpenMatrix.MyFilter[1, 1] = 12;
            sharpenMatrix.MyFilter[1, 2] = 1;
            sharpenMatrix.MyFilter[2, 0] = -1;
            sharpenMatrix.MyFilter[2, 1] = -1;
            sharpenMatrix.MyFilter[2, 2] = -1;

            return sharpenMatrix.ApplyFilter(colors, width, height);
        }

        #endregion

        #region Bump Map

        /// <summary>
        /// Gets a bump map from source Color32 array.
        /// </summary>
        /// <param name="colors">Source Color32 array.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <returns>DFBitmap bump image.</returns>
        public static Color32[] GetBumpMap(ref Color32[] colors, int width, int height)
        {
            // Convert to average intensity
            Color32[] newColors = MakeAverageIntensity(colors);

            // Create horizontal sobel matrix
            Filter horizontalMatrix = new Filter(3, 3);
            horizontalMatrix.MyFilter[0, 0] = -1;
            horizontalMatrix.MyFilter[0, 1] = 0;
            horizontalMatrix.MyFilter[0, 2] = 1;
            horizontalMatrix.MyFilter[1, 0] = -2;
            horizontalMatrix.MyFilter[1, 1] = 0;
            horizontalMatrix.MyFilter[1, 2] = 2;
            horizontalMatrix.MyFilter[2, 0] = -1;
            horizontalMatrix.MyFilter[2, 1] = 0;
            horizontalMatrix.MyFilter[2, 2] = 1;

            // Create vertical sobel matrix
            Filter verticalMatrix = new Filter(3, 3);
            verticalMatrix.MyFilter[0, 0] = 1;
            verticalMatrix.MyFilter[0, 1] = 2;
            verticalMatrix.MyFilter[0, 2] = 1;
            verticalMatrix.MyFilter[1, 0] = 0;
            verticalMatrix.MyFilter[1, 1] = 0;
            verticalMatrix.MyFilter[1, 2] = 0;
            verticalMatrix.MyFilter[2, 0] = -1;
            verticalMatrix.MyFilter[2, 1] = -2;
            verticalMatrix.MyFilter[2, 2] = -1;

            // Apply filters
            Color32[] horz = horizontalMatrix.ApplyFilter(newColors, width, height);
            Color32[] vert = verticalMatrix.ApplyFilter(newColors, width, height);

            // Merge
            Color32[] result = new Color32[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                int r = Clamp((horz[i].r + vert[i].r), 0, 255);
                int g = Clamp((horz[i].g + vert[i].g), 0, 255);
                int b = Clamp((horz[i].b + vert[i].b), 0, 255);
                result[i] = new Color32((byte)r, (byte)g, (byte)b, 255);
            }

            return result;
        }

        #endregion

        #region Normal Map

        /// <summary>
        /// Converts bump map to normal map.
        /// </summary>
        /// <param name="colors">Source Color32 array.</param>
        /// <param name="strength">Normal strength.</param>
        /// <returns>Color32[] normal map.</returns>
        public static Color32[] ConvertBumpToNormals(ref Color32[] colors, int width, int height, float strength = 1)
        {
            Color32[] newColors = (Color32[])colors.Clone();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Look up the heights to either side of this pixel
                    float left = GetIntensity(ref colors, x - 1, y, width, height);
                    float right = GetIntensity(ref colors, x + 1, y, width, height);
                    float top = GetIntensity(ref colors, x, y - 1, width, height);
                    float bottom = GetIntensity(ref colors, x, y + 1, width, height);

                    // Compute gradient vectors, then cross them to get the normal
                    Vector3 dx = new Vector3(1, 0, (right - left) * strength);
                    Vector3 dy = new Vector3(0, 1, (bottom - top) * strength);
                    Vector3 normal = Vector3.Cross(dx, dy);
                    normal.Normalize();

                    // Store result packed for Unity
                    byte r = (byte)((normal.x + 1.0f) * 127.5f);
                    byte g = (byte)(255 - ((normal.y + 1.0f) * 127.5f));
                    newColors[y * width + x] = new Color32(g, g, g, r);

                    //// This is a standard normal texture without Unity packing
                    //newColors[y * width + x] = new Color32(
                    //    (byte)((normal.x + 1.0f) * 127.5f),
                    //    (byte)(255 - ((normal.y + 1.0f) * 127.5f)),
                    //    (byte)((normal.z + 1.0f) * 127.5f),
                    //    0xff);
                }
            }

            return newColors;
        }

        #endregion

        #region Fonts

        // Gets fixed-width glyph data from FntFile as Color32 array
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
                    colors[pos] = (data[pos] > 0) ? textColor : backColor;
                }
            }

            return colors;
        }

        // Gets proportial-width glyph data from FntFile as Color32 array
        public static Color32[] GetProportionalGlyphColors(FntFile fntFile, int index, Color backColor, Color textColor, bool invertY = false)
        {
            // Get actual glyph dimensions
            int width = fntFile.GetGlyphWidth(index);
            int height = fntFile.FixedHeight;
            Color32[] colors = new Color32[width * height];

            // Get glyph byte data as color array
            byte[] data = fntFile.GetGlyphPixels(index);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcPos = y * FntFile.GlyphFixedDimension + x;
                    int dstPos = (invertY) ? (height - 1 - y) * width + x : y * width + x;
                    colors[dstPos] = (data[srcPos] > 0) ? textColor : backColor;
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

        private static float GetIntensity(ref Color32[] colors, int x, int y, int width, int height)
        {
            // Clamp X
            if (x < 0)
                x = 0;
            else if (x >= width)
                x = width - 1;

            // Clamp Y
            if (y < 0)
                y = 0;
            else if (y >= height)
                y = height - 1;

            // Get pixel
            Color32 pixel = colors[y * width + x];

            // Average intensity
            float intensity = ((pixel.r + pixel.g + pixel.b) / 3) / 255f;

            return intensity;
        }

        #endregion
    }
}