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
    /// Opens FONT000*.FNT files and reads glyph data.
    /// </summary>
    public class FntFile
    {
        #region Fields

        public const int MaxGlyphCount = 240;           // Fixed number of glyphs in every FNT file
        public const int GlyphDataLength = 32;          // Fixed data length of each glyph
        public const int GlyphFixedDimension = 16;      // Fixed pixel dimension of each glyph (i.e. 16x16 pixels)

        FileProxy managedFile = new FileProxy();
        FntFileData fileData = new FntFileData();

        #endregion

        #region Structures

        //
        // .FNT Course File Format
        // -Header          (4  bytes)
        // -Glyph table     (4  bytes * 240)
        // -Glyph data      (32 bytes * 240)
        //

        /// <summary>
        /// Structured FNT file data.
        /// </summary>
        public struct FntFileData
        {
            public FileHeader Header;
            public GlyphTableEntry[] Glyphs;
        }

        /// <summary>
        /// FNT header data.
        /// </summary>
        public struct FileHeader
        {
            public UInt16 FixedWidth;           // Fixed width of each glyph
            public UInt16 FixedHeight;          // Fixed height of each glyph
        }

        /// <summary>
        /// Glyph table entry.
        /// Each glyph has 32-bytes of data in 16-bit bitfield format.
        /// </summary>
        public struct GlyphTableEntry
        {
            public UInt16 GlyphDataOffset;      // Offset to data for this glyph
            public UInt16 GlyphWidth;           // Actual pixel width of this glyph
            public Byte[] GlyphData;            // Glyph data at offset
        }

        #endregion

        #region Properties

        /// <summary>
        /// True if a file is loaded.
        /// </summary>
        public bool IsLoaded
        {
            get { return (managedFile.Length > 0); }
        }

        /// <summary>
        /// Gets FNT file data.
        /// </summary>
        public FntFileData FileData
        {
            get { return fileData; }
        }

        /// <summary>
        /// Gets fixed glyph width.
        /// </summary>
        public int FixedWidth
        {
            get { return fileData.Header.FixedWidth; }
        }

        /// <summary>
        /// Gets fixed glyph height.
        /// </summary>
        public int FixedHeight
        {
            get { return fileData.Header.FixedHeight; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FntFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to FONT000*.FNT file.</param>
        /// <param name="usage">Determines if the FNT file will read from disk or memory.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public FntFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a FONT000*.FNT file.
        /// </summary>
        /// <param name="filePath">Absolute path to FONT000*.FNT file.</param>
        /// <param name="usage">Determines if the FNT file will read from disk or memory.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            const string prefix = "FONT000";
            const string suffix = ".FNT";

            // Validate filename
            string fileName = Path.GetFileName(filePath).ToUpper();
            if (!fileName.StartsWith(prefix) || !fileName.EndsWith(suffix))
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
        /// Gets actual pixel width of glyph.
        /// </summary>
        /// <param name="index">Index of glyph.</param>
        /// <returns>Pixel width of glyph or -1 on error.</returns>
        public int GetGlyphWidth(int index)
        {
            if (!IsLoaded)
                return -1;
            if (index < 0 || index >= MaxGlyphCount)
                return -1;

            return fileData.Glyphs[index].GlyphWidth;
        }

        /// <summary>
        /// Gets raw glyph data bytes.
        /// </summary>
        /// <param name="index">Index of glyph.</param>
        /// <returns>Buffer of GlyphDataLength or null on error.</returns>
        public byte[] GetGlyphBytes(int index)
        {
            if (!IsLoaded)
                return null;
            if (index < 0 || index >= MaxGlyphCount)
                return null;

            return fileData.Glyphs[index].GlyphData;
        }

        /// <summary>
        /// Gets fixed glyph pixel 2D array where 0 is "off" and colorIndex (or just non-zero) is "on".
        /// Always GlyphFixedDimension*GlyphFixedDimension bytes (i.e. 16x16 bytes).
        /// Glyph data aligned to top-left.
        /// </summary>
        /// <param name="index">Index of glyph.</param>
        /// <param name="colorIndex">Palette colour index for "on" pixels.</param>
        /// <returns>Glyph pixel array or null on error.</returns>
        public byte[] GetGlyphPixels(int index, int colorIndex = 127)
        {
            if (!IsLoaded)
                return null;
            if (index < 0 || index >= MaxGlyphCount)
                return null;

            byte[] bufferIn = fileData.Glyphs[index].GlyphData;
            byte[] bufferOut = new byte[GlyphFixedDimension * GlyphFixedDimension];
            for (int y = 0; y < GlyphFixedDimension; y++)
            {
                byte[] rowL = GetPixels(bufferIn[y * 2 + 1], (byte)colorIndex);
                byte[] rowR = GetPixels(bufferIn[y * 2], (byte)colorIndex);

                Array.Copy(rowL, 0, bufferOut, y * GlyphFixedDimension, rowL.Length);
                Array.Copy(rowR, 0, bufferOut, y * GlyphFixedDimension + rowL.Length, rowR.Length);
            }

            return bufferOut;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Read file.
        /// </summary>
        /// <returns>True if succeeded, otherwise false.</returns>
        private bool Read()
        {
            fileData = new FntFileData();

            try
            {
                // Step through file
                BinaryReader reader = managedFile.GetReader();
                ReadHeader(reader);
                ReadGlyphs(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Read ARCH3D.BSA file header.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        private void ReadHeader(BinaryReader reader)
        {
            reader.BaseStream.Position = 0;
            fileData.Header.FixedWidth = reader.ReadUInt16();
            fileData.Header.FixedHeight = reader.ReadUInt16();
        }

        /// <summary>
        /// Read glyphs.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        private void ReadGlyphs(BinaryReader reader)
        {
            fileData.Glyphs = new GlyphTableEntry[MaxGlyphCount];
            for (int i = 0; i < MaxGlyphCount; i++)
            {
                fileData.Glyphs[i].GlyphDataOffset = reader.ReadUInt16();
                fileData.Glyphs[i].GlyphWidth = reader.ReadUInt16();
                fileData.Glyphs[i].GlyphData = ReadGlyphData(reader, fileData.Glyphs[i].GlyphDataOffset);
            }
        }

        /// <summary>
        /// Reads glyph data. This does not change reader position.
        /// </summary>
        /// <param name="reader">A binary reader to file.</param>
        /// <param name="offset">Offset to glyph data.</param>
        private byte[] ReadGlyphData(BinaryReader reader, int offset)
        {
            long savedPosition = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            byte[] data = reader.ReadBytes(GlyphDataLength);
            reader.BaseStream.Position = savedPosition;

            return data;
        }

        /// <summary>
        /// Gets 8 bytes of pixel data from byte bitfield.
        /// </summary>
        /// <param name="data">Source data byte for bitfield.</param>
        /// <returns>8 bytes of pixel data.</returns>
        private byte[] GetPixels(byte data, byte colorIndex)
        {
            int bit = 1;
            byte[] row = new byte[8];
            for (int i = 7; i >= 0; i--)
            {
                if ((data & bit) == bit)
                    row[i] = colorIndex;
                else
                    row[i] = 0;

                bit *= 2;
            }

            return row;
        }

        #endregion
    }
}