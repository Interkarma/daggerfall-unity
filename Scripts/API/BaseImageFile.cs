// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Text;
using System.IO;
using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
#endregion

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Provides base image handling for all Daggerfall image files.
    ///  This class is inherited from and extended by Arena2.TextureFile, Arena2.ImgFile, etc.
    /// </summary>
    public abstract class BaseImageFile
    {
        #region Class Variables

        /// <summary>
        /// Index of window bytes when reading textures.
        /// </summary>
        protected const int windowIndex = 0xff;

        /// <summary>
        /// Palette for building image data
        /// </summary>
        protected DFPalette myPalette = new DFPalette();

        /// <summary>
        /// Managed file.
        /// </summary>
        internal FileProxy managedFile = new FileProxy();

        #endregion

        #region Class Structures

        /// <summary>
        /// Compression formats enumeration. This is shared as internal as most image formats use some kind of compression.
        /// </summary>
        internal enum CompressionFormats
        {
            Uncompressed = 0x0000,
            RleCompressed = 0x0002,
            ImageRle = 0x0108,
            RecordRle = 0x1108,
        }

        /// <summary>
        /// IMG File header. This is shared as internal as the IMG structure is also used in CIF files.
        ///  The CifFile class will use this file header while reading most records.
        /// </summary>
        internal struct ImgFileHeader
        {
            public long Position;
            public Int16 XOffset;
            public Int16 YOffset;
            public Int16 Width;
            public Int16 Height;
            public CompressionFormats Compression;
            public UInt16 PixelDataLength;
            public int FrameCount;
            public long DataPosition;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseImageFile()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets full path of managed file.
        /// </summary>
        public string FilePath
        {
            get { return managedFile.FilePath; }
        }

        /// <summary>
        /// Gets file name only of managed file.
        /// </summary>
        public string FileName
        {
            get { return Path.GetFileName(FilePath); }
        }

        /// <summary>
        /// Gets or sets palette for building images.
        /// </summary>
        public DFPalette Palette
        {
            get { return myPalette; }
            set { myPalette = value; }
        }

        /// <summary>
        /// Gets total number of records in this file.
        /// </summary>
        public abstract int RecordCount
        {
            get;
        }

        /// <summary>
        /// Gets description of this file.
        /// </summary>
        public abstract string Description
        {
            get;
        }

        /// <summary>
        /// Gets correct palette name for this file.
        /// </summary>
        public abstract string PaletteName
        {
            get;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads an image file.
        /// </summary>
        /// <param name="filePath">Absolute path to file</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public abstract bool Load(string filePath, FileUsage usage, bool readOnly);

        /// <summary>
        /// Gets number of frames in specified record.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>Number of frames.</returns>
        public abstract int GetFrameCount(int record);

        /// <summary>
        /// Gets width and height of specified record. All frames of this record are the same dimensions.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>DFSize object.</returns>
        public abstract DFSize GetSize(int record);

        /// <summary>
        /// Gets bitmap data as indexed 8-bit byte array for specified record and frame.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <param name="frame">Index of frame.</param>
        /// <returns>DFBitmap object.</returns>
        public abstract DFBitmap GetDFBitmap(int record, int frame);

        /// <summary>
        /// Loads a Daggerfall palette that will be used for building images.
        /// </summary>
        /// <param name="filePath">Absolute path to Daggerfall palette.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool LoadPalette(string filePath)
        {
            return myPalette.Load(filePath);
        }

        /// <summary>
        /// Gets a Color32 array for engine with minimum options.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <param name="frame">Index of frame.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <returns>Color32 array.</returns>
        public Color32[] GetColors32(int record, int frame, int alphaIndex = -1)
        {
            DFBitmap srcBitmap = GetDFBitmap(record, frame);

            DFSize sz;
            return GetColors32(srcBitmap, alphaIndex, 0, out sz);
        }

        /// <summary>
        /// Gets a Color32 array for engine with minimum options.
        /// </summary>
        /// <param name="srcBitmap">Source DFBitmap.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <returns>Color32 array.</returns>
        public Color32[] GetColors32(DFBitmap srcBitmap, int alphaIndex = -1)
        {
            DFSize sz;
            return GetColors32(srcBitmap, alphaIndex, 0, out sz);
        }

        /// <summary>
        /// Gets a Color32 array for engine with a border.
        /// </summary>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <param name="border">Number of pixels border to add around image.</param>
        /// <param name="sizeOut">Receives image dimensions with borders included.</param>
        /// <returns>Color32 array.</returns>
        public Color32[] GetColors32(int record, int frame, int alphaIndex, int border, out DFSize sizeOut)
        {
            // Get source bitmap
            DFBitmap srcBitmap = GetDFBitmap(record, frame);

            return GetColors32(srcBitmap, alphaIndex, border, out sizeOut);
        }

        /// <summary>
        /// Gets a Color32 array for engine with a border.
        /// </summary>
        /// <param name="srcBitmap">Source DFBitmap.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <param name="border">Number of pixels border to add around image.</param>
        /// <param name="sizeOut">Receives image dimensions with borders included.</param>
        /// <returns>Color32 array.</returns>
        public Color32[] GetColors32(DFBitmap srcBitmap, int alphaIndex, int border, out DFSize sizeOut)
        {
            // Calculate dimensions
            int srcWidth = srcBitmap.Width;
            int srcHeight = srcBitmap.Height;
            int dstWidth = srcWidth + border * 2;
            int dstHeight = srcHeight + border * 2;

            // Create target array
            Color32[] colors = new Color32[dstWidth * dstHeight];

            DFColor c;
            byte a;
            int index, srcRow, dstRow;
            for (int y = 0; y < srcHeight; y++)
            {
                // Get row position
                srcRow = y * srcWidth;
                dstRow = (dstHeight - 1 - border - y) * dstWidth;

                // Write data for this row
                for (int x = 0; x < srcWidth; x++)
                {
                    index = srcBitmap.Data[srcRow + x];
                    c = myPalette.Get(index);
                    if (alphaIndex == index) a = 0x00; else a = 0xff;
                    
                    colors[dstRow + border + x] = new Color32(c.R, c.G, c.B, a);
                }
            }

            sizeOut = new DFSize(dstWidth, dstHeight);

            return colors;
        }

        /// <summary>
        /// Gets a Color32 array with window indices tinted a custom colour.
        /// </summary>
        /// <param name="record">Record index.</param>
        /// <param name="color">New colour for window parts.</param>
        /// <param name="alpha">New alpha for window parts.</param>
        /// <param name="diffuseColors">Receives new window texture with custom colour.</param>
        /// <param name="alphaColors">Receives alpha of window parts.</param>
        public DFSize GetWindowColors32(int record, Color32 color, Color32 alpha, out Color32[] diffuseColors, out Color32[] alphaColors)
        {
            // Get source bitmap
            DFBitmap srcBitmap = GetDFBitmap(record, 0);

            // Create target array
            DFSize sz = new DFSize(srcBitmap.Width, srcBitmap.Height);
            diffuseColors = new Color32[sz.Width * sz.Height];
            alphaColors = new Color32[sz.Width * sz.Height];

            byte r, g, b;
            int index, srcRow, dstRow;
            for (int y = 0; y < sz.Height; y++)
            {
                // Get row position
                srcRow = y * sz.Width;
                dstRow = (sz.Height - 1 - y) * sz.Width; ;

                // Write data for this row
                for (int x = 0; x < sz.Width; x++)
                {
                    index = srcBitmap.Data[srcRow + x];
                    if (index == windowIndex)
                    {
                        // Set window parts
                        diffuseColors[dstRow + x] = color;
                        alphaColors[dstRow + x] = alpha;
                    }
                    else
                    {
                        // Set everthing else
                        r = myPalette.GetRed(index);
                        g = myPalette.GetGreen(index);
                        b = myPalette.GetBlue(index);
                        diffuseColors[dstRow + x] = new Color32(r, g, b, 0xff);
                        alphaColors[dstRow + x] = new Color32(0, 0, 0, 0);
                    }
                }
            }

            return sz;
        }

        /// <summary>
        /// Get raw bytes for specified record and frame using a custom pixel format.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <param name="frame">Index of frame.</param>
        /// <param name="alphaIndex">Index of alpha colour.</param>
        /// <param name="format">Specified pixel format to use.</param>
        /// <returns>DFBitmap object.</returns>
        public DFBitmap GetBitmapFormat(int record, int frame, byte alphaIndex, DFBitmap.Formats format)
        {
            // Get as indexed image
            if (format == DFBitmap.Formats.Indexed)
                return GetDFBitmap(record, frame);

            // Create new bitmap
            const int formatWidth = 4;
            DFBitmap srcBitmap = GetDFBitmap(record, frame);
            DFBitmap dstBitmap = new DFBitmap();
            dstBitmap.Format = format;
            dstBitmap.Width = srcBitmap.Width;
            dstBitmap.Height = srcBitmap.Height;
            dstBitmap.Stride = dstBitmap.Width * formatWidth;
            dstBitmap.Data = new byte[dstBitmap.Stride * dstBitmap.Height];

            // Write pixel data to array
            byte a, r, g, b;
            int srcPos = 0, dstPos = 0;
            for (int i = 0; i < dstBitmap.Width * dstBitmap.Height; i++)
            {
                // Write colour values
                byte index = srcBitmap.Data[srcPos++];
                if (index != alphaIndex)
                {
                    // Get colour values
                    a = 0xff;
                    r = myPalette.GetRed(index);
                    g = myPalette.GetGreen(index);
                    b = myPalette.GetBlue(index);

                    // Write colour values
                    switch (format)
                    {
                        case DFBitmap.Formats.RGBA:
                            dstBitmap.Data[dstPos++] = r;
                            dstBitmap.Data[dstPos++] = g;
                            dstBitmap.Data[dstPos++] = b;
                            dstBitmap.Data[dstPos++] = a;
                            break;
                        case DFBitmap.Formats.ARGB:
                            dstBitmap.Data[dstPos++] = a;
                            dstBitmap.Data[dstPos++] = r;
                            dstBitmap.Data[dstPos++] = g;
                            dstBitmap.Data[dstPos++] = b;
                            break;
                        default:
                            throw new Exception("Unknown output format.");
                    }
                }
                else
                {
                    // Step over alpha pixels
                    dstPos += formatWidth;
                }
            }

            return dstBitmap;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Reads a standard IMG file header from the source stream into the desination header struct.
        ///  This header is found in multiple image files which is why it's implemented here in the base.
        /// </summary>
        /// <param name="reader">Source reader positioned at start of header data.</param>
        /// <param name="header">Destination header structure.</param>
        internal void ReadImgFileHeader(ref BinaryReader reader, ref ImgFileHeader header)
        {
            // Read IMG header data
            header.Position = reader.BaseStream.Position;
            header.XOffset = reader.ReadInt16();
            header.YOffset = reader.ReadInt16();
            header.Width = reader.ReadInt16();
            header.Height = reader.ReadInt16();
            header.Compression = (CompressionFormats)reader.ReadUInt16();
            header.PixelDataLength = reader.ReadUInt16();
            header.FrameCount = 1;
            header.DataPosition = reader.BaseStream.Position;
        }

        #endregion 
    }
}
