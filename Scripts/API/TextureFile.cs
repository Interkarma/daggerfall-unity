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
using DaggerfallConnect.Utility;
#endregion

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Connects to a TEXTURE.??? file to enumerate and extract image data.
    ///  Each texture file may contain one or more images, including animated records with multiple frames.
    ///  Textures will only be converted from the source binary file when needed. This allows you to extract
    ///  individual records and frames without the overhead of converting unwanted images.
    ///  Combine this with a texture caching scheme when loading large 3D scenes to avoid unnecessary load time.
    /// </summary>
    public class TextureFile : BaseImageFile
    {
        #region Class Variables

        /// <summary>
        /// Width and height of solid images.
        /// </summary>
        private const int solidSize = 32;

        /// <summary>
        /// Type of solid image.
        /// </summary>
        private SolidTypes solidType = SolidTypes.None;

        /// <summary>
        /// Types of spectral image.
        /// </summary>
        private SpectralTypes spectralType = SpectralTypes.None;

        /// <summary>
        /// File header.
        /// </summary>
        private FileHeader header;

        /// <summary>
        /// Record header array
        /// </summary>
        private RecordHeader[] recordHeaders;

        /// <summary>
        /// Record array.
        /// </summary>
        private Record[] records;

        #endregion

        #region Class Structures

        /// <summary>
        /// Solid types enumeration.
        /// </summary>
        private enum SolidTypes
        {
            None,
            SolidColoursA,
            SolidColoursB,
        }

        /// <summary>
        /// Spectral types enumeration.
        /// </summary>
        private enum SpectralTypes
        {
            None,
            Spectral,
        }

        /// <summary>
        /// Row encoding used for the SpecialFrameHeader of RecordRle archives.
        /// </summary>
        private enum RowEncoding
        {
            IsRleEncoded = 0x8000,
            NotRleEncoded = 0,
        }

        /// <summary>
        /// File header.
        /// </summary>
        private struct FileHeader
        {
            public long Position;
            public Int16 RecordCount;
            public String Name;
        }

        /// <summary>
        /// Record header.
        /// </summary>
        private struct RecordHeader
        {
            public long Position;
            public Int16 Type1;
            public Int32 RecordPosition;
            public Int16 Type2;
            public Int32 Unknown1;
            public Int64 NullValue1;
        }

        /// <summary>
        /// Record data.
        /// </summary>
        private struct Record
        {
            public long Position;
            public Int16 OffsetX;
            public Int16 OffsetY;
            public Int16 Width;
            public Int16 Height;
            public CompressionFormats Compression;
            public UInt32 RecordSize;
            public UInt32 DataOffset;
            public Boolean IsNormal;
            public UInt16 FrameCount;
            public Int16 Unknown1;
            public Int16 ScaleX;
            public Int16 ScaleY;
            public DFBitmap[] Frames;
        }

        /// <summary>
        /// Used to decode RecordRle archives.
        /// </summary>
        private struct SpecialRowHeader
        {
            public Int16 RowOffset;
            public RowEncoding RowEncoding;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TextureFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to TEXTURE.* file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public TextureFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        /// <summary>
        /// Load constructor with palette assignment.
        /// </summary>
        /// <param name="filePath">Absolute path to TEXTURE.* file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="palette">Palette to use when building images.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public TextureFile(string filePath, FileUsage usage, DFPalette palette, bool readOnly)
        {
            myPalette = palette;
            Load(filePath, usage, readOnly);
        }

        /// <summary>
        /// Load constructor that also loads a palette.
        /// </summary>
        /// <param name="filePath">Absolute path to TEXTURE.* file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="paletteFilePath">Absolute path to Daggerfall palette file.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public TextureFile(string filePath, FileUsage usage, string paletteFilePath, bool readOnly)
        {
            Load(filePath, usage, readOnly);
            LoadPalette(paletteFilePath);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets description of texture file.
        /// </summary>
        public override string Description
        {
            get {return header.Name;}
        }

        /// <summary>
        /// Gets unsupported filenames. The three texture filenames returned have no image data or are otherwise invalid.
        /// </summary>
        /// <returns>Array of unsupported filenames.</returns>
        public static string[] UnsupportedFilenames
        {
            get
            {
                string[] names = new string[3];
                names[0] = "TEXTURE.215";
                names[1] = "TEXTURE.217";
                names[2] = "TEXTURE.436";
                return names;
            }
        }

        /// <summary>
        /// Gets correct palette name for this file (always ART_PAL.COL for texture files).
        /// </summary>
        public override string PaletteName
        {
            get { return "ART_PAL.COL"; }
        }

        /// <summary>
        /// Number of image records in this file.
        /// </summary>
        public override int RecordCount
        {
            get { return header.RecordCount; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests if a filename is supported.
        /// </summary>
        /// <param name="filename">Name of TEXTURE.* file.</param>
        /// <returns>True if supported, otherwise false.</returns>
        public bool IsFilenameSupported(string filename)
        {
            // Look for filename in list of unsupported filenames
            string[] names = UnsupportedFilenames;
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] == filename)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Loads a texture file.
        /// </summary>
        /// <param name="filePath">Absolute path to TEXTURE.* file</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public override bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Exit if this file already loaded
            if (managedFile.FilePath == filePath)
                return true;

            // Validate filename
            filePath = filePath.ToUpper();
            string fn = Path.GetFileName(filePath);
            if (!fn.StartsWith("TEXTURE."))
                return false;
            
            // Handle unsupported files
            if (!IsFilenameSupported(fn))
            {
                Console.WriteLine(string.Format("{0} is unsupported.", fn));
                return false;
            }

            // Handle solid types
            if (fn == "TEXTURE.000")
                solidType = SolidTypes.SolidColoursA;
            else if (fn == "TEXTURE.001")
                solidType = SolidTypes.SolidColoursB;
            else
                solidType = SolidTypes.None;

            // Handle spectral types
            if (fn == "TEXTURE.273" || fn == "TEXTURE.278")
                spectralType = SpectralTypes.Spectral;
            else
                spectralType = SpectralTypes.None;

            // Load file
            if (!managedFile.Load(filePath, usage, readOnly))
                return false;

            // Read file
            if (!Read())
                return false;

            return true;
        }

        /// <summary>
        /// Gets number of frames in specified record.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>Number of frames.</returns>
        public override int GetFrameCount(int record)
        {
            // Validate
            if (record < 0 || record >= header.RecordCount || records == null)
                return -1;

            // Handle solid types
            if (solidType != SolidTypes.None)
                return 1;

            return records[record].FrameCount;
        }

        /// <summary>
        /// Gets width and height of specified record. All frames of this record are the same dimensions.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>DFSize object.</returns>
        public override DFSize GetSize(int record)
        {
            // Validate
            if (record < 0 || record >= header.RecordCount || records == null)
                return new DFSize(0, 0);

            // Handle solid types
            if (solidType != SolidTypes.None)
                return new DFSize(solidSize, solidSize);

            return new DFSize(records[record].Width, records[record].Height);
        }

        /// <summary>
        /// Get the width and height scale to apply to image in scene. These values are divided by 256
        ///  to obtain a value between -1.0 - 0.0, and presumably 0.0 - 1.0. This is the scale of pixels
        ///  for enlarging or shrinking the image.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>Scale values for X and Y in DFSize object.</returns>
        public DFSize GetScale(int record)
        {
            // Validate
            if (record < 0 || record >= header.RecordCount || records == null)
                return new DFSize(0, 0);

            return new DFSize(records[record].ScaleX, records[record].ScaleX);
        }

        /// <summary>
        /// Gets the offset value of record.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>Offset values for X and Y in DFSize object.</returns>
        public DFSize GetOffset(int record)
        {
            // Validate
            if (record < 0 || record >= header.RecordCount || records == null)
                return new DFSize(0, 0);

            return new DFSize(records[record].OffsetX, records[record].OffsetY);
        }

        /// <summary>
        /// Gets width of this image.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>Width of image in pixels.</returns>
        public int GetWidth(int record)
        {
            // Validate
            if (record < 0 || record >= header.RecordCount || records == null)
                return 0;

            // Handle solid types
            if (solidType != SolidTypes.None)
                return solidSize;

            return records[record].Width;
        }

        /// <summary>
        /// Gets the height of this image.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>Height of image in pixels.</returns>
        public int GetHeight(int record)
        {
            // Validate
            if (record < 0 || record >= header.RecordCount || records == null)
                return 0;

            // Handle solid types
            if (solidType != SolidTypes.None)
                return solidSize;

            return records[record].Height;
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
            if (record < 0 || record >= header.RecordCount || records == null || frame >= GetFrameCount(record))
                return new DFBitmap();

            // Read raw data from file
            if (!ReadImageData(record, frame))
                return new DFBitmap();

            return records[record].Frames[frame];
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Returns a TEXTURE.nnn filename based on index.
        ///  This is needed when loading textures for 3D objects that reference textures by index rather than filename.
        ///  If the index is not valid, the returned filename will also be invalid.
        /// </summary>
        /// <param name="archiveIndex">Index of texture archive.</param>
        /// <returns>Texture filename in the format TEXTURE.nnn.</returns>
        public static string IndexToFileName(int archiveIndex)
        {
            return string.Format("TEXTURE.{0:000}", archiveIndex);
        }

        /// <summary>
        /// Gets size of an unloaded texture quickly with minimum overhead.
        ///  This is useful for mesh loading where the texture dimensions need to be known,
        ///  but you may not need to load the entire texture file at that time.
        /// </summary>
        /// <param name="filePath">Absolute path to TEXTURE.* file</param>
        /// <param name="record">Index of record.</param>
        /// <returns>DFSize.</returns>
        public static DFSize QuickSize(string filePath, int record)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new DFSize(0, 0);
            }

            // Read record count and check range
            BinaryReader reader = new BinaryReader(fs);
            int recordCount = reader.ReadInt16();
            if (record < 0 || record >= recordCount)
                return new DFSize(0, 0);

            // Offset to width and height
            reader.BaseStream.Position = 26 + 20 * record + 2;
            reader.BaseStream.Position = reader.ReadInt32() + 4;

            // Read width and height
            int width = reader.ReadInt16();
            int height = reader.ReadInt16();

            // Close reader and stream
            reader.Close();

            // Return size
            return new DFSize(width, height);
        }

        /// <summary>
        /// Gets scale of an unloaded texture quickly with minimum overhead.
        ///  This is useful for flat loading where the texture scale needs to be known,
        ///  but you may not need to load the entire texture file at that time.
        /// </summary>
        /// <param name="filePath">Absolute path to TEXTURE.* file</param>
        /// <param name="record">Index of record.</param>
        /// <returns>Size.</returns>
        public static DFSize QuickScale(string filePath, int record)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new DFSize(0, 0);
            }

            // Read record count and check range
            BinaryReader reader = new BinaryReader(fs);
            int recordCount = reader.ReadInt16();
            if (record < 0 || record >= recordCount)
                return new DFSize(0, 0);

            // Offset to scale
            reader.BaseStream.Position = 26 + 20 * record + 2;
            reader.BaseStream.Position = reader.ReadInt32() + 24;

            // Read xscale and yscale
            int xscale = reader.ReadInt16();
            int yscale = reader.ReadInt16();

            // Close reader and stream
            reader.Close();

            // Return size
            return new DFSize(xscale, yscale);
        }

        /// <summary>
        /// Gets frame of an unloaded texture quickly with minimum overhead.
        /// This is useful when the number of animation frames needs to be
        /// known before loading texture.
        /// </summary>
        /// <param name="filePath">Absolute path to TEXTURE.* file</param>
        /// <param name="record">Index of record.</param>
        /// <returns>Frame count or 0 on error.</returns>
        public static int QuickFrameCount(string filePath, int record)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }

            // Read record count and check range
            BinaryReader reader = new BinaryReader(fs);
            int recordCount = reader.ReadInt16();
            if (record < 0 || record >= recordCount)
                return 0;

            // Offset to scale
            reader.BaseStream.Position = 26 + 20 * record + 2;
            reader.BaseStream.Position = reader.ReadInt32() + 20;

            // Read frame count
            int frameCount = reader.ReadInt16();

            // Close reader and stream
            reader.Close();

            // Return size
            return frameCount;
        }

        #endregion

        #region Readers

        /// <summary>
        /// Read file.
        /// </summary>
        /// <returns>True if succeeded, otherwise false.</returns>
        private bool Read()
        {
            try
            {
                // Step through file
                BinaryReader reader = managedFile.GetReader();
                ReadHeader(ref reader);
                ReadRecordHeaders(ref reader);
                ReadRecords(ref reader);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads file header.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        private void ReadHeader(ref BinaryReader reader)
        {
            reader.BaseStream.Position = 0;
            header.Position = 0;
            header.RecordCount = reader.ReadInt16();
            header.Name = managedFile.ReadCString(reader, 0).Trim();
        }

        /// <summary>
        /// Reads record headers.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        private void ReadRecordHeaders(ref BinaryReader reader)
        {
            reader.BaseStream.Position = 26;
            recordHeaders = new RecordHeader[header.RecordCount];
            for (int r = 0; r < header.RecordCount; r++)
            {
                recordHeaders[r].Position = reader.BaseStream.Position;
                recordHeaders[r].Type1 = reader.ReadInt16();
                recordHeaders[r].RecordPosition = reader.ReadInt32();
                recordHeaders[r].Type2 = reader.ReadInt16();
                recordHeaders[r].Unknown1 = reader.ReadInt32();
                recordHeaders[r].NullValue1 = reader.ReadInt64();
            }
        }

        /// <summary>
        /// Reads records.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        private void ReadRecords(ref BinaryReader reader)
        {
            records = new Record[header.RecordCount];
            for (int r = 0; r < header.RecordCount; r++)
            {
                reader.BaseStream.Position = recordHeaders[r].RecordPosition;
                records[r].Position = reader.BaseStream.Position;
                records[r].OffsetX = reader.ReadInt16();
                records[r].OffsetY = reader.ReadInt16();
                records[r].Width = reader.ReadInt16();
                records[r].Height = reader.ReadInt16();
                records[r].Compression = (CompressionFormats)reader.ReadInt16();
                records[r].RecordSize = (UInt32)reader.ReadInt32();
                records[r].DataOffset = (UInt32)reader.ReadInt32();
                records[r].IsNormal = Convert.ToBoolean(reader.ReadInt16());
                records[r].FrameCount = (UInt16)reader.ReadInt16();
                records[r].Unknown1 = reader.ReadInt16();
                records[r].ScaleX = reader.ReadInt16();
                records[r].ScaleY = reader.ReadInt16();

                // Create frame array
                if (SolidTypes.None == solidType)
                {
                    records[r].Frames = new DFBitmap[records[r].FrameCount];
                }
                else
                {
                    records[r].Width = solidSize;
                    records[r].Height = solidSize;
                    records[r].Frames = new DFBitmap[1];
                }
            }
        }

        /// <summary>
        /// Reads image data for specified record and frame.
        /// </summary>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <returns>True if succeeded, otherwise false.</returns>
        private bool ReadImageData(int record, int frame)
        {
            // Exit if this image already read
            if (records[record].Frames[frame] != null)
                return true;

            // Handle solid types
            if (solidType != SolidTypes.None)
                return ReadSolid(record);

            // Read based on compression type
            bool result;
            switch (records[record].Compression)
            {
                case CompressionFormats.RecordRle:
                case CompressionFormats.ImageRle:
                    result = ReadRle(record, frame);
                    break;
                case CompressionFormats.Uncompressed:
                default:
                    result = ReadImage(record, frame);
                    break;
            }

            // Handle spectral types
            if (spectralType != SpectralTypes.None)
            {
                SetSpectral(record, frame);
            }

            return result;
        }

        /// <summary>
        /// Create a solid image type.
        /// </summary>
        /// <param name="record">Record index.</param>
        /// <returns>True if succeeded, otherwise false.</returns>
        private bool ReadSolid(int record)
        {
            // Set start colour index
            byte colourIndex;
            if (solidType == SolidTypes.SolidColoursA)
                colourIndex = (byte)record;
            else
                colourIndex = (byte)(128 + record);

            // Create buffer to hold extracted image
            records[record].Frames[0] = new DFBitmap();
            records[record].Frames[0].Width = solidSize;
            records[record].Frames[0].Height = solidSize;
            records[record].Frames[0].Data = new byte[solidSize * solidSize];

            // Write image bytes
            int srcPos = 0;
            byte[] srcData = records[record].Frames[0].Data;
            for (int i = 0; i < solidSize * solidSize; i++)
            {
                srcData[srcPos++] = colourIndex;
            }

            return true;
        }

        /// <summary>
        /// Modify image data for spectral textures.
        /// </summary>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        private void SetSpectral(int record, int frame)
        {
            // Just set spectral enemies to dark gray for now
            for (int i = 0; i < records[record].Frames[frame].Data.Length; i++)
            {
                int index = records[record].Frames[frame].Data[i];
                if (index > 0)
                    records[record].Frames[frame].Data[i] = 93;
            }
        }

        /// <summary>
        /// Read uncompressed record.
        /// </summary>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <returns>True if succeeded, otherwise false.</returns>
        private bool ReadImage(int record, int frame)
        {
            // Create buffer to hold extracted image
            records[record].Frames[frame] = new DFBitmap();
            records[record].Frames[frame].Width = records[record].Width;
            records[record].Frames[frame].Height = records[record].Height;
            records[record].Frames[frame].Stride = records[record].Width;
            records[record].Frames[frame].Format = DFBitmap.Formats.Indexed;
            records[record].Frames[frame].Data = new byte[records[record].Width * records[record].Height];

            if (records[record].FrameCount == 1)
            {
                // Extract image bytes
                long position = records[record].Position + records[record].DataOffset;
                BinaryReader reader = managedFile.GetReader(position);
                BinaryWriter writer = new BinaryWriter(new MemoryStream(records[record].Frames[frame].Data));
                for (int y = 0; y < records[record].Height; y++)
                {
                    writer.Write(reader.ReadBytes(records[record].Width));
                    reader.BaseStream.Position += (256 - records[record].Width);
                }
            }
            else if (records[record].FrameCount > 1)
            {
                // Get frame offset list
                Int32[] offsets = new Int32[records[record].FrameCount];
                long position = records[record].Position + records[record].DataOffset;
                BinaryReader reader = managedFile.GetReader(position);
                for (int offset = 0; offset < records[record].FrameCount; offset++)
                    offsets[offset] = reader.ReadInt32();

                // Offset to desired frame
                reader.BaseStream.Position = position + offsets[frame];
                int cx = reader.ReadInt16();
                int cy = reader.ReadInt16();

                // Extract image bytes
                BinaryWriter writer = new BinaryWriter(new MemoryStream(records[record].Frames[frame].Data));
                for (int y = 0; y < cy; y++)
                {
                    int x = 0;
                    while (x < cx)
                    {
                        // Write transparant bytes
                        byte pixel = reader.ReadByte();
                        int run = x + pixel;
                        for (; x < run; x++)
                        {
                            writer.Write((byte)0);
                        }

                        // Write image bytes
                        pixel = reader.ReadByte();
                        run = x + pixel;
                        for (; x < run; x++)
                        {
                            pixel = reader.ReadByte();
                            writer.Write(pixel);
                        }
                    }
                }
            }
            else
            {
                // No frames
                return false;
            }

            return true;
        }

        /// <summary>
        /// Read a RecordRle record.
        /// </summary>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <returns>True if succeeded, otherwise false.</returns>
        private bool ReadRle(int record, int frame)
        {
            // Create buffer to hold extracted image
            records[record].Frames[frame].Width = records[record].Width;
            records[record].Frames[frame].Height = records[record].Height;
            records[record].Frames[frame].Stride = records[record].Width;
            records[record].Frames[frame].Format = DFBitmap.Formats.Indexed;
            records[record].Frames[frame].Data = new byte[records[record].Width * records[record].Height];

            // Find offset to special row headers for this frame
            long position = records[record].Position + records[record].DataOffset;
            position += (records[record].Height * frame) * 4;
            BinaryReader Reader = managedFile.GetReader(position);

            // Read special row headers for this frame
            SpecialRowHeader[] SpecialRowHeaders = new SpecialRowHeader[records[record].Height];
            for (int i = 0; i < records[record].Height; i++)
            {
                SpecialRowHeaders[i].RowOffset = Reader.ReadInt16();
                SpecialRowHeaders[i].RowEncoding = (RowEncoding)Reader.ReadUInt16();
            }

            // Create row memory writer
            BinaryWriter writer = new BinaryWriter(new MemoryStream(records[record].Frames[frame].Data));

            // Extract all rows of image
            foreach(SpecialRowHeader header in SpecialRowHeaders)
            {
                // Get offset to row relative to record data offset
                position = records[record].Position + header.RowOffset;
                Reader.BaseStream.Position = position;

                // Handle row data based on compression
                if (RowEncoding.IsRleEncoded == header.RowEncoding)
                {
                    // Extract RLE row
                    byte pixel = 0;
                    int probe = 0;
                    int rowPos = 0;
                    int rowWidth = Reader.ReadUInt16();
                    do
                    {
                        probe = Reader.ReadInt16();
                        if (probe < 0)
                        {
                            probe = -probe;
                            pixel = Reader.ReadByte();
                            for (int i = 0; i < probe; i++)
                            {
                                writer.Write(pixel);
                                rowPos++;
                            }
                        }
                        else if (0 < probe)
                        {
                            writer.Write(Reader.ReadBytes(probe));
                            rowPos += probe;
                        }
                    } while (rowPos < rowWidth);
                }
                else
                {
                    // Just copy bytes
                    writer.Write(Reader.ReadBytes(records[record].Width));
                }
            }

            return true;
        }

        #endregion
    }
}
