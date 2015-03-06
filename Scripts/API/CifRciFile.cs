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
    /// Connects to a *.CIF or *.RCI file to enumerate and extract image data.
    ///  Each CIF file may contain one or more images, including animated records with multiple frames.
    ///  Each RCI file may contain one or more images, but never animated records.
    /// </summary>
    public class CifRciFile : BaseImageFile
    {
        #region Class Variables

        /// <summary>
        /// Record array. This is pre-sized to 64 objects as CIF files do not contain a master header and
        ///  it's necessary to read through the file to count the number of image records.
        /// </summary>
        private Record[] records = new Record[64];

        /// <summary>
        /// Total number of records in this file. Any records past this count in Records array are not valid.
        /// </summary>
        private int totalRecords = 0;

        #endregion

        #region Class Structures

        /// <summary>
        /// Types of records found in this file.
        /// </summary>
        private enum RecordType
        {
            MultiImage,
            WeaponAnim,
        }

        /// <summary>
        /// Record data.
        /// </summary>
        private struct Record
        {
            public ImgFileHeader Header;
            public AnimationHeader AnimHeader;
            public RecordType FileType;
            public long AnimPixelDataPosition;
            public DFBitmap[] Frames;
        }

        /// <summary>
        /// Animation header for weapon files.
        /// </summary>
        private struct AnimationHeader
        {
            public long Position;
            public UInt16 Width;
            public UInt16 Height;
            public UInt16 LastFrameWidth;
            public Int16 XOffset;
            public Int16 LastFrameYOffset;
            public Int16 DataLength;
            public UInt16[] FrameDataOffsetList;
            public UInt16 TotalSize;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CifRciFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to *.CIF or *.RCI file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public CifRciFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        /// <summary>
        /// Load constructor with palette assignment.
        /// </summary>
        /// <param name="filePath">Absolute path to *.CIF or *.RCI file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="palette">Palette to use when building images.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public CifRciFile(string filePath, FileUsage usage, DFPalette palette, bool readOnly)
        {
            myPalette = palette;
            Load(filePath, usage, readOnly);
        }

        /// <summary>
        /// Load constructor that also loads a palette.
        /// </summary>
        /// <param name="filePath">Absolute path to *.CIF or *.RCI file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="paletteFilePath">Absolute path to Daggerfall palette file.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public CifRciFile(string filePath, FileUsage usage, string paletteFilePath, bool readOnly)
        {
            LoadPalette(paletteFilePath);
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Return correct palette name for this file (always ART_PAL.COL for CIF and RCI files).
        /// </summary>
        public override string PaletteName
        {
            get { return "ART_PAL.COL"; }
        }

        /// <summary>
        /// Number of image records in this Cif or Rci file.
        /// </summary>
        public override int RecordCount
        {
            get { return totalRecords; }
        }

        /// <summary>
        /// Description of this file.
        /// </summary>
        public override string Description
        {
            get
            {
                if (FilePath.EndsWith(".CIF"))
                    return "CIF File";
                else if (FilePath.EndsWith(".RCI"))
                    return "RCI File";
                else
                    return "Unknown";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a CIF or RCI file.
        /// </summary>
        /// <param name="filePath">Absolute path to *.CIF or *.RCI file</param>
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
            if (!filePath.EndsWith(".CIF") && !filePath.EndsWith(".RCI"))
                return false;

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
            if (record < 0 || record >= RecordCount)
                return 0;

            return records[record].Header.FrameCount;
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

            if (records[record].Header.FrameCount > 1)
                return new DFSize(records[record].AnimHeader.Width, records[record].AnimHeader.Height);
            else
                return new DFSize(records[record].Header.Width, records[record].Header.Height);
        }

        /// <summary>
        /// Gets bitmap data as indexed 8-bit byte array for specified record and frame.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <param name="frame">Index of frame.</param>
        /// <returns>DFBitmap object.</returns>
        public override DFBitmap GetDFBitmap(int fecord, int frame)
        {
            // Validate
            if (fecord < 0 || fecord >= RecordCount || frame >= GetFrameCount(fecord))
                return new DFBitmap();

            // Read raw data from file
            if (!ReadImageData(fecord, frame))
                return new DFBitmap();

            return records[fecord].Frames[frame];
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
        /// Reads records and formats Records array.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        private void ReadRecords(ref BinaryReader reader)
        {
            // Go to start of file
            reader.BaseStream.Position = 0;

            // Handle RCI files (faces.cif is actually an rci file)
            string fn = Path.GetFileName(managedFile.FilePath);
            if (fn.EndsWith(".RCI") || fn == "FACES.CIF")
            {
                ReadRci(ref reader, fn);
                return;
            }

            // Handle WEAPON files
            if (fn.Contains("WEAPO"))
            {
                ReadWeaponCif(ref reader, fn);
                return;
            }

            // Count number of single-frame records. This is just a contiguous array of single-frame IMG files
            int count = 0;
            do
            {
                // Read header data
                ReadImgFileHeader(ref reader, ref records[count].Header);

                // Set file type
                records[count].FileType = RecordType.MultiImage;

                // Create empty frame object
                records[count].Frames = new DFBitmap[1];

                // Increment past image data for now
                reader.BaseStream.Position += records[count].Header.PixelDataLength;

                // Increment count
                count++;
            } while (reader.BaseStream.Position < reader.BaseStream.Length);

            // Store count
            totalRecords = count;
        }

        /// <summary>
        /// Special handling for RCI files.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        /// <param name="filename">Name of this file without path.</param>
        private void ReadRci(ref BinaryReader reader, string filename)
        {
            // Set dimensions based on filename
            DFSize sz;
            switch (filename)
            {
                case "FACES.CIF":
                case "CHLD00I0.RCI":
                    sz = new DFSize(64, 64);
                    break;
                case "TFAC00I0.RCI":
                    records = new Record[503];  // Extend array as this file has hundreds of images
                    sz = new DFSize(64, 64);
                    break;
                case "BUTTONS.RCI":
                    sz = new DFSize(32, 16);
                    break;
                case "MPOP.RCI":
                    sz = new DFSize(17, 17);
                    break;
                case "NOTE.RCI":
                    sz = new DFSize(44, 9);
                    break;
                case "SPOP.RCI":
                    sz = new DFSize(22, 22);
                    break;
                default:
                    return;
            }

            // Get image count
            int count = managedFile.Length / (sz.Width * sz.Height);

            // Read count image records
            for (int i = 0; i < count; i++)
            {
                // Create empty frame object
                records[i].Header.Position = reader.BaseStream.Position;
                records[i].Header.XOffset = 0;
                records[i].Header.YOffset = 0;
                records[i].Header.Width = (Int16)sz.Width;
                records[i].Header.Height = (Int16)sz.Height;
                records[i].Header.Compression = CompressionFormats.Uncompressed;
                records[i].Header.PixelDataLength = (UInt16)(sz.Width * sz.Height);
                records[i].Header.FrameCount = 1;
                records[i].Header.DataPosition = reader.BaseStream.Position;

                // Set record type
                records[i].FileType = RecordType.MultiImage;

                // Increment past image data for now
                reader.BaseStream.Position += records[i].Header.PixelDataLength;

                // Create empty frame object
                records[i].Frames = new DFBitmap[1];
            }

            // Store count
            totalRecords = count;
        }

        /// <summary>
        /// Special handling for WEAPON CIF files.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        /// <param name="filename">Name of this file without path.</param>
        private void ReadWeaponCif(ref BinaryReader reader, string filename)
        {
            // Read "wielding" image, except for WEAPO09.CIF (the bow) which has no such image
            int count = 0;
            if (filename != "WEAPON09.CIF")
            {
                // Read header data
                ReadImgFileHeader(ref reader, ref records[count].Header);

                // Set record type (first image is just a standard IMG image)
                records[count].FileType = RecordType.MultiImage;

                // Create empty frame object
                records[count].Frames = new DFBitmap[1];

                // Increment past image data for now
                reader.BaseStream.Position += records[count].Header.PixelDataLength;

                // Increment count
                count++;
            }

            do
            {
                // Read animation header
                records[count].AnimHeader.Position = reader.BaseStream.Position;
                records[count].AnimHeader.Width = reader.ReadUInt16();
                records[count].AnimHeader.Height = reader.ReadUInt16();
                records[count].AnimHeader.LastFrameWidth = reader.ReadUInt16();
                records[count].AnimHeader.XOffset = reader.ReadInt16();
                records[count].AnimHeader.LastFrameYOffset = reader.ReadInt16();
                records[count].AnimHeader.DataLength = reader.ReadInt16();

                // Set record type
                records[count].FileType = RecordType.WeaponAnim;

                // Read frame data offset list
                int FrameCount = 0;
                records[count].AnimHeader.FrameDataOffsetList = new UInt16[31];
                for (int i = 0; i < 31; i++)
                {
                    records[count].AnimHeader.FrameDataOffsetList[i] = reader.ReadUInt16();
                    if (records[count].AnimHeader.FrameDataOffsetList[i] != 0)
                        FrameCount++;
                }

                // Create empty frame objects
                records[count].Header.FrameCount = FrameCount;
                records[count].Frames = new DFBitmap[FrameCount];

                // Read total size
                records[count].AnimHeader.TotalSize = reader.ReadUInt16();

                // Get position of pixel data
                records[count].AnimPixelDataPosition = reader.BaseStream.Position;

                // Skip over pixel data for now
                reader.BaseStream.Position = records[count].AnimHeader.Position + records[count].AnimHeader.TotalSize;

                // Increment count
                count++;
            } while (reader.BaseStream.Position < reader.BaseStream.Length);

            // Store count
            totalRecords = count;
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

            // Handle weapon-type records
            if (records[record].FileType == RecordType.WeaponAnim)
                return ReadWeaponImage(record, frame);

            // Read based on compression type
            switch (records[record].Header.Compression)
            {
                case CompressionFormats.RleCompressed:
                    return ReadRleImage(record, frame);
                case CompressionFormats.Uncompressed:
                    return ReadImage(record, frame);
                default:
                    return false;
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
            // Setup frame to hold extracted image
            records[record].Frames[frame] = new DFBitmap();
            records[record].Frames[frame].Width = records[record].Header.Width;
            records[record].Frames[frame].Height = records[record].Header.Height;
            records[record].Frames[frame].Stride = records[record].Header.Width;
            records[record].Frames[frame].Format = DFBitmap.Formats.Indexed;
            records[record].Frames[frame].Data = new byte[records[record].Header.PixelDataLength];

            // Read image bytes
            long position = records[record].Header.DataPosition;
            BinaryReader reader = managedFile.GetReader(position);
            BinaryWriter writer = new BinaryWriter(new MemoryStream(records[record].Frames[frame].Data));
            writer.Write(reader.ReadBytes(records[record].Header.PixelDataLength));

            return true;
        }

        /// <summary>
        /// Reads image data for specified weapon-type record and frame.
        /// </summary>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <returns>True if succeeded, otherwise false.</returns>
        private bool ReadWeaponImage(int record, int frame)
        {
            // Setup frame to hold extracted image
            int length = records[record].AnimHeader.Width * records[record].AnimHeader.Height;
            records[record].Frames[frame] = new DFBitmap();
            records[record].Frames[frame].Width = records[record].AnimHeader.Width;
            records[record].Frames[frame].Height = records[record].AnimHeader.Height;
            records[record].Frames[frame].Stride = records[record].AnimHeader.Width;
            records[record].Frames[frame].Format = DFBitmap.Formats.Indexed;
            records[record].Frames[frame].Data = new byte[length];

            // Extract image data from frame RLE
            long position = records[record].AnimHeader.Position + records[record].AnimHeader.FrameDataOffsetList[frame];
            BinaryReader reader = managedFile.GetReader(position);
            BinaryWriter writer = new BinaryWriter(new MemoryStream(records[record].Frames[frame].Data));
            ReadRleData(ref reader, length, ref writer);

            return true;
        }

        /// <summary>
        /// Read a RLE record.
        /// </summary>
        /// <param name="record">Record index.</param>
        /// <param name="frame">Frame index.</param>
        /// <returns>True if succeeded, otherwise false.</returns>
        private bool ReadRleImage(int record, int frame)
        {
            // Setup frame to hold extracted image
            int length = records[record].Header.Width * records[record].Header.Height;
            records[record].Frames[frame].Width = records[record].Header.Width;
            records[record].Frames[frame].Height = records[record].Header.Height;
            records[record].Frames[frame].Stride = records[record].Header.Width;
            records[record].Frames[frame].Format = DFBitmap.Formats.Indexed;
            records[record].Frames[frame].Data = new byte[length];

            // Extract image data from RLE
            long position = records[record].Header.DataPosition;
            BinaryReader reader = managedFile.GetReader(position);
            BinaryWriter writer = new BinaryWriter(new MemoryStream(records[record].Frames[frame].Data));
            ReadRleData(ref reader, length, ref writer);

            return true;
        }

        /// <summary>
        /// Reads RLE compressed data from source reader to destination writer.
        /// </summary>
        /// <param name="reader">Source reader positioned at start of input data.</param>
        /// <param name="length">Length of source data.</param>
        /// <param name="writer">Destination writer positioned at start of output data.</param>
        /// <returns>True if succeeded, otherwise false.</returns>
        private void ReadRleData(ref BinaryReader reader, int length, ref BinaryWriter writer)
        {
            // Read image bytes
            byte pixel = 0;
            byte code = 0;
            int pos = 0;
            do
            {
                code = reader.ReadByte();
                if (code > 127)
                {
                    pixel = reader.ReadByte();
                    for (int i = 0; i < code - 127; i++)
                    {
                        writer.Write(pixel);
                        pos++;
                    }
                }
                else
                {
                    writer.Write(reader.ReadBytes(code + 1));
                    pos += (code + 1);
                }
            } while (pos < length);
        }

        #endregion
    }
}
