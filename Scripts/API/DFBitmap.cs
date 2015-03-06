// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Text;
#endregion

namespace DaggerfallConnect
{
    /// <summary>
    /// Stores raw bitmap data in indexed or colour byte formats.
    /// </summary>
    public class DFBitmap
    {
        #region Fields

        /// <summary>Format byte width of indexed formats.</summary>
        const int indexedFormatWidth = 1;

        /// <summary>Format byte width of full colour formats.</summary>
        const int colorFormatWidth = 4;

        /// <summary>Format of the image.</summary>
        public Formats Format;

        /// <summary>Width of the image in pixels.</summary>
        public int Width;

        /// <summary>Height of the image in pixels.</summary>
        public int Height;

        /// <summary>Stride (bytes per horizontal row) of the image.</summary>
        public int Stride;

        /// <summary>Image byte array in specified format.</summary>
        public byte[] Data;

        #endregion

        #region Structures

        /// <summary>
        /// Bitmap formats enumeration.
        /// </summary>
        public enum Formats
        {
            /// <summary>Indexed image. 1 byte per pixel. Each byte is an index into the palette colour data.</summary>
            Indexed,
            /// <summary>Colour channels in order alpha, red, green, blue. 4 bytes per pixel.</summary>
            ARGB,
            /// <summary>Colour channels in order red, green, blue, alpha. 4 bytes per pixel.</summary>
            RGBA,
        }

        /// <summary>
        /// Internal colour struct.
        /// </summary>
        public struct DFColor
        {
            public byte r;
            public byte g;
            public byte b;
            public byte a;

            public static DFColor FromRGBA(byte r, byte g, byte b, byte a = 255)
            {
                DFColor color = new DFColor()
                {
                    r = r,
                    g = g,
                    b = b,
                    a = a,
                };
                return color;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets byte width of a single pixel (Indexed=1, other formats=4).
        /// </summary>
        public int FormatWidth
        {
            get
            {
                if (this.Format == Formats.Indexed)
                    return indexedFormatWidth;
                else
                    return colorFormatWidth;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// Creates an empty DFBitmap with null data.
        /// </summary>
        public DFBitmap()
        {
        }

        /// <summary>
        /// Constructor.
        /// Creates a new DFBitmap with sized and zeroed data array.
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        public DFBitmap(int width, int height)
        {
            Initialise(width, height);
        }

        /// <summary>
        /// Constructor.
        /// Creates a new DFBitmap with sized data array filled to specified colour.
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="color">Fill colour.</param>
        public DFBitmap(int width, int height, DFColor color)
            : this(width, height)
        {
            Fill(this, color);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise DFBitmap data to specified size.
        /// Any existing data will be discarded.
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        public void Initialise(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Stride = width * FormatWidth;
            this.Data = new byte[this.Height * this.Stride];
        }

        #endregion

        #region Static Public Methods

        /// <summary>
        /// Fills DFBitmap with specified values.
        /// </summary>
        /// <param name="bitmap">DFBitmap to fill.</param>
        /// <param name="r">Red value.</param>
        /// <param name="g">Green value.</param>
        /// <param name="b">Blue value.</param>
        /// <param name="a">Alpha value.</param>
        static public void Fill(DFBitmap bitmap, byte r, byte g, byte b, byte a)
        {
            // Fill with value and alpha
            for (int pos = 0; pos < bitmap.Data.Length; pos += bitmap.FormatWidth)
            {
                bitmap.Data[pos] = r;
                bitmap.Data[pos + 1] = g;
                bitmap.Data[pos + 2] = b;
                bitmap.Data[pos + 3] = a;
            }
        }

        /// <summary>
        /// Fills DFBitmap with specified colour.
        /// </summary>
        /// <param name="bitmap">DFBitmap to fill.</param>
        /// <param name="color">Source colour.</param>
        static public void Fill(DFBitmap bitmap, DFColor color)
        {
            Fill(bitmap, color.r, color.g, color.b, color.a);
        }

        /// <summary>
        /// Sets pixel in DFBitmap. Colour formats only.
        /// </summary>
        /// <param name="bitmap">DFBitmap.</param>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <param name="colour">Fill colour.</param>
        static public void SetPixel(DFBitmap bitmap, int x, int y, DFColor color)
        {
            int pos = y * bitmap.Stride + x * bitmap.FormatWidth;
            if (bitmap.Format == Formats.RGBA)
            {
                bitmap.Data[pos++] = color.r;
                bitmap.Data[pos++] = color.g;
                bitmap.Data[pos++] = color.b;
                bitmap.Data[pos] = color.a;
            }
            else if (bitmap.Format == Formats.ARGB)
            {
                bitmap.Data[pos++] = color.a;
                bitmap.Data[pos++] = color.r;
                bitmap.Data[pos++] = color.g;
                bitmap.Data[pos] = color.b;
            }
        }

        /// <summary>
        /// Sets pixel in DFBitmap. Colour formats only.
        /// </summary>
        /// <param name="bitmap">DFBitmap.</param>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <param name="r">Red value.</param>
        /// <param name="g">Green value.</param>
        /// <param name="b">Blue value.</param>
        /// <param name="a">Alpha value.</param>
        static public void SetPixel(DFBitmap bitmap, int x, int y, byte r, byte g, byte b, byte a)
        {
            SetPixel(bitmap, x, y, DFColor.FromRGBA(r, g, b, a));
        }

        /// <summary>
        /// Gets pixel DFColor from DFBitmap.
        /// </summary>
        /// <param name="bitmap">DFBitmap.</param>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <returns>DFColor.</returns>
        static public DFColor GetPixel(DFBitmap bitmap, int x, int y)
        {
            DFColor color = new DFColor();
            int srcPos = y * bitmap.Stride + x * bitmap.FormatWidth;
            color.r = bitmap.Data[srcPos++];
            color.g = bitmap.Data[srcPos++];
            color.b = bitmap.Data[srcPos++];
            color.a = bitmap.Data[srcPos++];

            return color;
        }

        /// <summary>
        /// Creates a clone of a DFBitmap.
        /// </summary>
        /// <param name="bitmap">DFBitmap source.</param>
        /// <returns>Cloned DFBitmap.</returns>
        static public DFBitmap CloneDFBitmap(DFBitmap bitmap)
        {
            // Create destination bitmap to receive normal image
            DFBitmap newBitmap = new DFBitmap();
            newBitmap.Width = bitmap.Width;
            newBitmap.Height = bitmap.Height;
            newBitmap.Stride = bitmap.Stride;
            newBitmap.Format = bitmap.Format;
            newBitmap.Data = new byte[bitmap.Data.Length];
            bitmap.Data.CopyTo(newBitmap.Data, 0);

            return newBitmap;
        }

        #endregion
    }
}
