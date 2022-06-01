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

using System;
using System.IO;
using DaggerfallConnect.Utility;

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Connects to *.CFA files to extract image data.
    /// CFA is a legacy Arena image file format that is largely unused in Daggerfall.
    /// Most significant usage is first-person horse and cart transport modes.
    /// Notes:
    ///  This implementation is limited in that it only supports 8-bit wide indexed pixels without muxing.
    ///  Thanks to creator of WinArena for releasing source to decode Arena CFA files.
    ///  This made it significantly easier to research Daggerfall's CFA files for DFTFU.
    /// </summary>
    public class CfaFile : BaseImageFile
    {
        #region Fields

        Header header;
        byte[] imageData;

        #endregion

        #region Properties

        /// <summary>
        /// Number of image records in this CFA file.
        /// </summary>
        public override int RecordCount
        {
            get
            {
                if (string.IsNullOrEmpty(managedFile.FilePath))
                    return 0;
                else
                    return 1;
            }
        }

        /// <summary>
        /// Gets palette name for CFA file.
        /// </summary>
        public override string PaletteName
        {
            get { return "ART_PAL.COL"; }
        }

        /// <summary>
        /// Description of this file (always "CFA File" as game data contain no text descriptions for this file type).
        /// </summary>
        public override string Description
        {
            get { return "CFA File"; }
        }

        #endregion

        #region Structures

        struct Header
        {
            public Int16 WidthUncompressed;
            public Int16 Height;
            public Int16 WidthCompressed;
            public Int16 Unknown1;
            public Int16 Unknown2;
            public Byte BitsPerPixel;
            public Byte FrameCount;
            public Int16 HeaderSize;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CfaFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to *.CFA file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public CfaFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a CFA file.
        /// </summary>
        /// <param name="filePath">Absolute path to *.CFA file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public override bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Exit if this file already loaded
            if (managedFile.FilePath == filePath)
                return true;

            // Validate filename
            if (!filePath.EndsWith(".CFA", StringComparison.InvariantCultureIgnoreCase))
                return false;

            // Load file
            if (!managedFile.Load(filePath, usage, readOnly))
                return false;

            // Read file
            if (!Read())
                return false;

            return true;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets number of frames.
        /// </summary>
        /// <param name="record">Record index. Only 1 record in Daggerfall CFA files so value should be 0.</param>
        /// <returns>Number of frames.</returns>
        public override int GetFrameCount(int record)
        {
            // Validate
            if (record < 0 || record >= RecordCount)
                return -1;

            return header.FrameCount;
        }

        /// <summary>
        /// Gets width and height of specified record. All frames of this record are the same dimensions.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>DFSize object.</returns>
        public override DFSize GetSize(int record)
        {
            // Validate
            if (record < 0 || record >= RecordCount)
                return new DFSize(0, 0);

            return new DFSize(header.WidthUncompressed, header.Height);
        }

        /// <summary>
        /// Gets bitmap data as indexed 8-bit byte array for specified record and frame.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <param name="frame">Index of frame.</param>
        /// <returns>DFBitmap object.</returns>
        public override DFBitmap GetDFBitmap(int record, int frame)
        {
            // Validate
            if (record < 0 || record >= RecordCount || frame >= GetFrameCount(record) || imageData == null)
                return new DFBitmap();

            // Create bitmap
            DFBitmap bitmap = new DFBitmap(header.WidthUncompressed, header.Height);
            bitmap.Data = new byte[bitmap.Width * bitmap.Height];
            bitmap.Palette = Palette;

            // Copy rows of image data
            int offset = bitmap.Width * bitmap.Height * frame;
            for (int y = 0; y < bitmap.Height; y++)
            {
                Array.Copy(imageData, offset + y * bitmap.Width, bitmap.Data, y * bitmap.Width, bitmap.Width);
            }

            return bitmap;
        }

        #endregion

        #region Readers

        bool Read()
        {
            try
            {
                // Step through file
                BinaryReader reader = managedFile.GetReader();
                ReadHeader(reader);
                ReadImageData(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        // Read file header
        void ReadHeader(BinaryReader reader)
        {
            reader.BaseStream.Position = 0;
            header = new Header();
            header.WidthUncompressed = reader.ReadInt16();
            header.Height = reader.ReadInt16();
            header.WidthCompressed = reader.ReadInt16();
            header.Unknown1 = reader.ReadInt16();
            header.Unknown2 = reader.ReadInt16();
            header.BitsPerPixel = reader.ReadByte();
            header.FrameCount = reader.ReadByte();
            header.HeaderSize = reader.ReadInt16();
        }

        // Read image data for all frames
        void ReadImageData(BinaryReader reader)
        {
            // Create buffer to hold extracted image data
            imageData = new byte[header.WidthUncompressed * header.Height * header.FrameCount];

            // Extract image data from RLE
            // Image data is a series of sequential frames
            reader.BaseStream.Position = header.HeaderSize;
            BinaryWriter writer = new BinaryWriter(new MemoryStream(imageData));
            ReadRleData(ref reader, header.WidthCompressed * header.Height * header.FrameCount, ref writer);
            writer.Close();
        }

        #endregion
    }
}
