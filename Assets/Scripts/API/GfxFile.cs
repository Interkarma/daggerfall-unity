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
    /// Connects to *.GFX files to extract image data.
    /// The only GFX files in Daggerfall are SCRL00I0.GFX and SCRL01I0.GFX.
    /// Combined thse make a total of 16 frames of scrolling parchment used exclusively by class questions UI.
    /// </summary>
    public class GfxFile : BaseImageFile
    {
        #region Fields

        const int headerSize = 14;

        Header header;
        public DFBitmap[] frames;

        #endregion

        #region Properties

        /// <summary>
        /// Number of image records in this GFX file.
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
        /// Gets palette name for GFX file.
        /// </summary>
        public override string PaletteName
        {
            get { return "ART_PAL.COL"; }
        }

        /// <summary>
        /// Description of this file (always "GFX File" as game data contain no text descriptions for this file type).
        /// </summary>
        public override string Description
        {
            get { return "GFX File"; }
        }

        #endregion

        #region Structures

        struct Header
        {
            public Int16 FrameCount;            // Frame count, always 8
            public Int16 Width;                 // Width of image, always 320
            public Int16 Height;                // Height of image, always 80
            public Int16 PixelDataLength;       // Always 25600 or 320*80
            public Int16 Unknown2;              // Always 1
            public Byte[] Unknown3;             // Always 4x zeros
        }

        struct Row
        {
            public UInt16 RowOffset;            // Offset to data for this row from start of file
            public RowEncoding RowEncoding;     // Type of encoding used for this row
        }

        // Same row encoding value and RLE method used by TEXTURE files
        enum RowEncoding
        {
            IsRleEncoded = 0x8000,
            NotRleEncoded = 0,
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GfxFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to *.GFX file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public GfxFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a GFX file.
        /// </summary>
        /// <param name="filePath">Absolute path to *.GFX file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public override bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Exit if this file already loaded
            if (managedFile.FilePath == filePath)
                return true;

            // Validate filename
            if (!filePath.EndsWith(".GFX", StringComparison.InvariantCultureIgnoreCase))
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
        /// <param name="record">Record index. Only 1 record in Daggerfall GFX files so value should be 0.</param>
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

            return new DFSize(header.Width, header.Height);
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
            if (record < 0 || record >= RecordCount || frame >= GetFrameCount(record))
                return new DFBitmap();

            return frames[frame];
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
            header.FrameCount = reader.ReadInt16();
            header.Width = reader.ReadInt16();
            header.Height = reader.ReadInt16();
            header.PixelDataLength = reader.ReadInt16();
            header.Unknown2 = reader.ReadInt16();
            header.Unknown3 = reader.ReadBytes(4);
        }

        void ReadImageData(BinaryReader reader)
        {
            // Read image data for all rows after header
            Row[] rows = new Row[header.Height * header.FrameCount];
            for (int i = 0; i < header.Height * header.FrameCount; i++)
            {
                rows[i] = new Row()
                {
                    RowOffset = reader.ReadUInt16(),
                    RowEncoding = (RowEncoding)reader.ReadUInt16(),
                };
            }

            // Read all frames
            frames = new DFBitmap[header.FrameCount];
            for (int frame = 0; frame < header.FrameCount; frame++)
            {
                ReadRleImage(reader, rows, frame);
            }
        }

        private bool ReadRleImage(BinaryReader reader, Row[] rows, int frame)
        {
            // Create buffer to hold extracted image
            byte[] data = new byte[header.PixelDataLength];
            frames[frame] = new DFBitmap();
            frames[frame].Width = header.Width;
            frames[frame].Height = header.Height;
            frames[frame].Palette = Palette;

            // Extract all rows of frame image
            int dstPos = 0;
            int rowStart = header.Height * frame;
            for (int rowIndex = 0; rowIndex < header.Height; rowIndex++)
            {
                // Handle row data based on compression
                Row row = rows[rowStart + rowIndex];
                reader.BaseStream.Position = row.RowOffset;
                if (row.RowEncoding == RowEncoding.IsRleEncoded)
                {
                    // Extract RLE row
                    byte pixel = 0;
                    int probe = 0;
                    int rowPos = 0;
                    int rowWidth = reader.ReadUInt16();
                    do
                    {
                        probe = reader.ReadInt16();
                        if (probe < 0)
                        {
                            probe = -probe;
                            pixel = reader.ReadByte();
                            for (int i = 0; i < probe; i++)
                            {
                                data[dstPos++] = pixel;
                                rowPos++;
                            }
                        }
                        else if (0 < probe)
                        {
                            byte[] nextBytes = reader.ReadBytes(probe);
                            Array.Copy(nextBytes, 0, data, dstPos, nextBytes.Length);
                            dstPos += nextBytes.Length;
                            rowPos += probe;
                        }
                    } while (rowPos < rowWidth);
                }
                else
                {
                    // Just copy bytes
                    byte[] nextBytes = reader.ReadBytes(header.Width);
                    Array.Copy(nextBytes, 0, data, dstPos, nextBytes.Length);
                    dstPos += nextBytes.Length;
                }
            }

            // Assign complete data to image
            frames[frame].Data = data;

            return true;
        }

        #endregion
    }
}