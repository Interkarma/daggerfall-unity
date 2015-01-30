// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DaggerfallConnect.Utility;
#endregion

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Reads data from WOODS.WLD.
    /// </summary>
    public class WoodsFile
    {
        #region Class Variables

        /// <summary>Width of heightmap in bytes.</summary>
        public const int mapWidthValue = 1000;

        /// <summary>Height of heightmap in bytes.</summary>
        public const int mapHeightValue = 500;

        /// <summary>Memory length of heightmap in bytes.</summary>
        private const int mapBufferLengthValue = mapWidthValue * mapHeightValue;

        /// <summary>
        /// Abstracts WOODS.WLD file to a managed disk or memory stream.
        /// </summary>
        private FileProxy managedFile = new FileProxy();

        /// <summary>
        /// Contains the WOODS.WLD file header data.
        /// </summary>
        private FileHeader header;

        /// <summary>
        /// Offsets to CellData structures. There are 1000*500 offsets that correspond to
        ///  the standard 1000x500 world map structure.
        /// </summary>
        private UInt32[] dataOffsets;

        ///// <summary>
        ///// UNUSED.
        /////  Unknown data.
        ///// </summary>
        //private DataSection1 dataSection1Data;

        /// <summary>
        /// Height map data buffer.
        /// </summary>
        private Byte[] heightMapBuffer = new Byte[mapBufferLengthValue];

        #endregion

        #region Class Structures

        /// <summary>
        /// Represents WOODS.WLD file header.
        /// </summary>
        private struct FileHeader
        {
            public long Position;
            public UInt32 OffsetSize;
            public UInt32 Width;
            public UInt32 Height;
            public UInt32 NullValue1;
            public UInt32 DataSection1Offset;
            public UInt32 Unknown1;
            public UInt32 Unknown2;
            public UInt32 HeightMapOffset;
            public UInt32[] NullValue2;
        }

        ///// <summary>
        ///// UNUSED.
        /////  Represents DataSection1 data.
        /////  The purpose of this data is currently unknown.
        ///// </summary>
        //private struct DataSection1
        //{
        //    public UInt32[] Unknown1;
        //}

        ///// <summary>
        ///// UNUSED.
        /////  Extended information per map pixel.
        ///// </summary>
        //private struct PixelData
        //{
        //    public UInt16 Unknown1;
        //    public UInt32 NullValue1;
        //    public UInt16 FileIndex;
        //    public Byte Climate;
        //    public Byte ClimateNoise;
        //    public UInt32[] NullValue2;
        //    public Byte[][] ElevationNoise;
        //}

        #endregion

        #region Public Properties

        /// <summary>
        /// Width of heightmap data (always 1000).
        /// </summary>
        public static int MapWidth
        {
            get { return mapWidthValue; }
        }

        /// <summary>
        /// Height of heightmap data (always 500).
        /// </summary>
        public static int MapHeight
        {
            get { return mapHeightValue; }
        }

        /// <summary>
        /// Gets or sets extracted heightmap data.
        /// </summary>
        public Byte[] Buffer
        {
            get { return heightMapBuffer; }
            set { heightMapBuffer = value; }
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets default WOODS.WLD filename.
        /// </summary>
        static public string Filename
        {
            get { return "WOODS.WLD"; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WoodsFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to WOODS.WLD.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public WoodsFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load WOODS.WLD file.
        /// </summary>
        /// <param name="filePath">Absolute path to WOODS.WLD file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Validate filename
            filePath = filePath.ToUpper();
            if (!filePath.EndsWith(Filename))
                return false;

            // Load file into memory
            if (!managedFile.Load(filePath, usage, readOnly))
                return false;

            // Read file
            if (!Read())
                return false;

            return true;
        }

        /// <summary>
        /// Get extracted heightmap data as an indexed image.
        /// </summary>
        /// <returns>DFBitmap object.</returns>
        public DFBitmap GetHeightMapDFBitmap()
        {
            DFBitmap DFBitmap = new DFBitmap();
            DFBitmap.Format = DFBitmap.Formats.Indexed;
            DFBitmap.Width = mapWidthValue;
            DFBitmap.Height = mapHeightValue;
            DFBitmap.Stride = mapWidthValue;
            DFBitmap.Data = heightMapBuffer;

            return DFBitmap;
        }

        /// <summary>
        /// Gets value for specified position in heightmap.
        /// Clamps range to ensure not outside array bounds.
        /// </summary>
        /// <param name="mapPixelX">X position in heightmap. 0 to MapWidth-1.</param>
        /// <param name="mapPixelY">Y position in heightmap. 0 to MapHeight-1.</param>
        /// <returns>Value of heightmap data.</returns>
        public Byte GetHeightMapValue(int mapPixelX, int mapPixelY)
        {
            // Clamp X
            if (mapPixelX < 0) mapPixelX = 0;
            if (mapPixelX >= MapWidth - 1) mapPixelX = MapWidth - 1;

            // Clamp Y
            if (mapPixelY < 0) mapPixelY = 0;
            if (mapPixelY >= MapHeight - 1) mapPixelY = MapHeight - 1;

            return Buffer[(mapPixelY * mapWidthValue) + mapPixelX];
        }

        /// <summary>
        /// Gets range of small height map data.
        /// </summary>
        /// <param name="mapPixelX">X position in heightmap. 0 to MapWidth-1.</param>
        /// <param name="mapPixelY">Y position in heightmap. 0 to MapHeight-1.</param>
        /// <param name="dim">Dimension of heightmap samples to read.</param>
        /// <returns>Byte array dim,dim in size.</returns>
        public Byte[,] GetHeightMapValuesRange(int mapPixelX, int mapPixelY, int dim)
        {
            Byte[,] dstData = new Byte[dim, dim];
            for (int y = 0; y < dim; y++)
            {
                for (int x = 0; x < dim; x++)
                {
                    dstData[x, y] = GetHeightMapValue(mapPixelX + x, mapPixelY + y);
                }
            }

            return dstData;
        }

        /// <summary>
        /// Gets 5x5 large map data for given map pixel.
        /// Read exactly as found in files.
        /// </summary>
        /// <param name="mapPixelX">X position in heightmap. 0 to MapWidth-1.</param>
        /// <param name="mapPixelY">Y position in heightmap. 0 to MapHeight-1.</param>
        /// <returns>5x5 grid of map data.</returns>
        public Byte[,] GetLargeMapData(int mapPixelX, int mapPixelY)
        {
            // Clamp X
            if (mapPixelX < 0) mapPixelX = 0;
            if (mapPixelX >= MapWidth - 1) mapPixelX = MapWidth - 1;

            // Clamp Y
            if (mapPixelY < 0) mapPixelY = 0;
            if (mapPixelY >= MapHeight - 1) mapPixelY = MapHeight - 1;

            // Offset directly to this pixel data
            BinaryReader reader = managedFile.GetReader();
            reader.BaseStream.Position = dataOffsets[mapPixelY * mapWidthValue + mapPixelX] + 22;

            // Read 5x5 data
            Byte[,] data = new Byte[5, 5];
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    data[x, y] = reader.ReadByte();
                }
            }

            return data;
        }

        /// <summary>
        /// Gets a range of large height map values with unknown grid stripped away.
        /// Also inverts Y order of height samples.
        /// </summary>
        /// <param name="mapPixelX">Start X position in heightmap. 0 to MapWidth-1.</param>
        /// <param name="mapPixelY">Start Y position in heightmap. 0 to MapHeight-1.</param>
        /// <param name="dim">Dimension of heightmap cells to read.</param>
        /// <returns>Byte array dim*3,dim*3 in size.</returns>
        public Byte[,] GetLargeHeightMapValuesRange(int mapPixelX, int mapPixelY, int dim)
        {
            const int offsetx = 1;
            const int offsety = 1;
            const int len = 3;

            Byte[,] dstData = new Byte[dim * len, dim * len];
            for (int y = 0; y < dim; y++)
            {
                for (int x = 0; x < dim; x++)
                {
                    // Get source 5x5 data
                    Byte[,] srcData = GetLargeMapData(mapPixelX + x, mapPixelY - y);

                    // Write 3x3 heightmap pixels to destination array
                    int startX = x * len;
                    int startY = y * len;
                    for (int iy = offsety; iy < offsety + len; iy++)
                    {
                        for (int ix = offsetx; ix < offsetx + len; ix++)
                        {
                            dstData[startX + ix - offsetx, startY + iy - offsety] = srcData[ix, 4 - iy];
                        }
                    }
                }
            }

            return dstData;
        }

        #endregion

        #region Readers

        /// <summary>
        /// Read file.
        /// </summary>
        private bool Read()
        {
            try
            {
                // Step through file
                BinaryReader Reader = managedFile.GetReader();
                ReadHeader(Reader);
                ReadDataOffsets(Reader);
                //ReadDataSection1(Reader);     // No sense reading this if not used
                ReadHeightMap(Reader);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Read header data.
        /// </summary>
        /// <param name="reader">Reader to stream.</param>
        private void ReadHeader(BinaryReader reader)
        {
            // Read header
            reader.BaseStream.Position = 0;
            header.Position = 0;
            header.OffsetSize = reader.ReadUInt32();
            header.Width = reader.ReadUInt32();
            header.Height = reader.ReadUInt32();
            header.NullValue1 = reader.ReadUInt32();
            header.DataSection1Offset = reader.ReadUInt32();
            header.Unknown1 = reader.ReadUInt32();
            header.Unknown2 = reader.ReadUInt32();
            header.HeightMapOffset = reader.ReadUInt32();
            header.NullValue2 = new UInt32[28];
            for (int i = 0; i < 28; i++)
                header.NullValue2[i] = reader.ReadUInt32();
        }

        /// <summary>
        /// Read data offsets.
        /// </summary>
        /// <param name="reader">Reader to stream, positioned at start of offset data.</param>
        private void ReadDataOffsets(BinaryReader reader)
        {
            // Validate
            if (header.Width * header.Height != mapBufferLengthValue)
                throw new Exception("Invalid WOODS.WLD Width*Height result from Header.");

            // Create offset array
            dataOffsets = new UInt32[mapBufferLengthValue];

            // Read offsets 
            for (int i = 0; i < mapBufferLengthValue; i++)
                dataOffsets[i] = reader.ReadUInt32();
        }

        ///// <summary>
        ///// Read DataSection1 data.
        /////  The purpose of this data is currently unknown.
        ///// </summary>
        ///// <param name="reader">Reader to stream.</param>
        //private void ReadDataSection1(BinaryReader reader)
        //{
        //    // Position reader
        //    reader.BaseStream.Position = header.DataSection1Offset;

        //    // Read data
        //    dataSection1Data.Unknown1 = new UInt32[256];
        //    for (int i = 0; i < 256; i++)
        //        dataSection1Data.Unknown1[i] = reader.ReadUInt32();
        //}

        /// <summary>
        /// Read heightmap data.
        /// </summary>
        /// <param name="reader">Reader to stream.</param>
        private void ReadHeightMap(BinaryReader reader)
        {
            // Read heightmap data
            reader.BaseStream.Position = header.HeightMapOffset;
            heightMapBuffer = reader.ReadBytes(mapBufferLengthValue);
        }

        #endregion
    }
}
