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
    /// Connects to CLIMATE.PAK or POLITIC.PAK to extract and read meta-data about the Daggerfall world map.
    /// </summary>
    public class PakFile
    {
        #region Class Variables

        /// <summary>Length of each PAK row.</summary>
        public const int pakWidthValue = 1001;

        /// <summary>Number of PAK rows.</summary>
        public const int pakHeightValue = 500;

        /// <summary>Memory length of extracted PAK file.</summary>
        const int pakBufferLengthValue = pakWidthValue * pakHeightValue;

        /// <summary>Abstracts PAK file to a managed disk or memory stream.</summary>
        private FileProxy managedFile = new FileProxy();

        /// <summary>Extracted PAK file buffer.</summary>
        private Byte[] pakExtractedBuffer = new Byte[pakBufferLengthValue];

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets extracted PAK data.
        /// </summary>
        public Byte[] Buffer
        {
            get { return pakExtractedBuffer; }
            set { pakExtractedBuffer = value; }
        }

        /// <summary>
        /// Number of rows in PAK file (always 500).
        /// </summary>
        public int PakRowCount
        {
            get { return pakHeightValue; }
        }

        /// <summary>
        /// Number of bytes per PAK row (always 1001).
        /// </summary>
        public int PakRowLength
        {
            get { return pakWidthValue; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PakFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to PAK file.</param>
        public PakFile(string filePath)
        {
            Load(filePath);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load PAK file.
        /// </summary>
        /// <param name="filePath">Absolute path to PAK file.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath)
        {
            // Validate filename
            filePath = filePath.ToUpper();
            if (!filePath.EndsWith("CLIMATE.PAK") && !filePath.EndsWith("POLITIC.PAK"))
                return false;

            // Load file
            if (!managedFile.Load(filePath, FileUsage.UseMemory, true))
                return false;

            // Expand each row of PAK file into buffer
            BinaryReader offsetReader = managedFile.GetReader(0);
            BinaryReader rowReader = managedFile.GetReader();
            for (int row = 0; row < pakHeightValue; row++)
            {
                // Get offsets
                UInt32 offset = offsetReader.ReadUInt32();
                int bufferPos = pakWidthValue * row;
                rowReader.BaseStream.Position = offset;

                // Unroll PAK row into buffer
                int rowPos = 0;
                while (rowPos < pakWidthValue)
                {
                    // Get PakRun data
                    UInt16 count = rowReader.ReadUInt16();
                    Byte value = rowReader.ReadByte();

                    // Do PakRun
                    for (int c = 0; c < count; c++)
                    {
                        pakExtractedBuffer[bufferPos + rowPos++] = value;
                    }
                }
            }

            // Managed file is no longer needed
            managedFile.Close();

            return true;
        }

        /// <summary>
        /// Get extracted PAK data as an indexed image.
        /// </summary>
        /// <returns>DFBitmap object.</returns>
        public DFBitmap GetDFBitmap()
        {
            DFBitmap DFBitmap = new DFBitmap();
            DFBitmap.Format = DFBitmap.Formats.Indexed;
            DFBitmap.Width = pakWidthValue;
            DFBitmap.Height = PakRowCount;
            DFBitmap.Stride = pakWidthValue;
            DFBitmap.Data = pakExtractedBuffer;
            return DFBitmap;
        }

        /// <summary>
        /// Gets value for specified position in world map.
        /// </summary>
        /// <param name="x">X position in world map. 0 to PakRowLength-1.</param>
        /// <param name="y">Y position in world map. 0 to PakRowCount-1.</param>
        /// <returns>Value of pak data if valid, -1 if invalid.</returns>
        public int GetValue(int x, int y)
        {
            // Validate
            if (x < 0 || x >= PakRowLength) return -1;
            if (y < 0 || y >= PakRowCount) return -1;

            return Buffer[(y * PakRowLength) + x];
        }

        #endregion
    }
}
