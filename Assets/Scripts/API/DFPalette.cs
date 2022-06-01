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

#region Using Statements
using System;
using System.IO;
using DaggerfallConnect.Utility;
#endregion

namespace DaggerfallConnect
{
    /// <summary>
    /// Describes a 256-colour Daggerfall palette. Supports loading .PAL and .COL files.
    ///  Palette is initialised to all 0xff0000 (red) to make it obvious when palette isn't loaded.
    /// </summary>
    public class DFPalette
    {
        #region Fields

        /// <summary>
        /// Length of header in bytes for supporting .COL files.
        /// </summary>
        private int headerLength = 0;

        /// <summary>
        /// Array of 256x RGB values. Includes 8-byte header for supporting .COL files.
        /// </summary>
        private readonly byte[] paletteBuffer = new byte[776];

        #endregion

        #region Properties

        /// <summary>
        /// Gets palette memory buffer.
        /// </summary>
        public byte[] PaletteBuffer
        {
            get { return paletteBuffer; }
        }

        /// <summary>
        /// Gets palette header length;
        /// </summary>
        public int HeaderLength
        {
            get { return headerLength; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor. Palette is initialised with red.
        /// </summary>
        public DFPalette()
        {
            Fill(0xff, 0, 0);
        }

        /// <summary>
        /// Load constructor (supports both .PAL and .COL files).
        /// </summary>
        /// <param name="FilePath">Absolute path to palette file.</param>
        public DFPalette(string FilePath)
        {
            if (!Load(FilePath))
                Fill(0xff, 0, 0);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">Palette to copy from.</param>
        public DFPalette(DFPalette other)
        {
            if (other != null && other.PaletteBuffer != null && other.paletteBuffer.Length != 0)
            {
                headerLength = other.HeaderLength;
                paletteBuffer = new byte[other.PaletteBuffer.Length];
                Array.Copy(other.PaletteBuffer, paletteBuffer, other.PaletteBuffer.Length);
            }
        }

        /// <summary>
        /// Loads a Daggerfall palette file (supports both .PAL and .COL files).
        /// </summary>
        /// <param name="FilePath">Absolute path to palette file.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string FilePath)
        {
            FileProxy fileProxy = new FileProxy(FilePath, FileUsage.UseMemory, true);
            switch (fileProxy.Length)
            {
                case 768:
                    headerLength = 0;
                    break;
                case 776:
                    headerLength = 8;
                    break;
                default:
                    return false;
            }

            // Read palette
            BinaryReader reader = fileProxy.GetReader();
            if (fileProxy.Length != reader.Read(paletteBuffer, 0, fileProxy.Length))
                return false;

            // Multiply MAP.PAL
            if (Path.GetFileName(FilePath) == "MAP.PAL")
                Multiply(4);

            return true;
        }

        /// <summary>
        /// Read palette information from a binary reader.
        ///  Palette must be a 768-byte PalFile structure (256x 24-bit RGB values).
        /// </summary>
        /// <param name="Reader">Source reader positioned at start of palette data.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Read(ref BinaryReader Reader)
        {
            // Read palette bytes
            if (768 != Reader.Read(paletteBuffer, 8, 768))
                return false;

            // Set header length
            headerLength = 8;

            return true;
        }

        /// <summary>
        /// Fills entire palette with specified RGB value.
        /// </summary>
        /// <param name="R">Red component.</param>
        /// <param name="G">Green component.</param>
        /// <param name="B">Blue component.</param>
        public void Fill(byte R, byte G, byte B)
        {
            int offset = headerLength;
            for (int i = 0; i < 256; i++)
            {
                paletteBuffer[offset++] = R;
                paletteBuffer[offset++] = G;
                paletteBuffer[offset++] = B;
            }
        }

        /// <summary>
        /// Fills entire palette with grayscale values.
        /// </summary>
        public void MakeGrayscale()
        {
            int offset = headerLength;
            for (int i = 0; i < 256; i++)
            {
                paletteBuffer[offset++] = (byte)i;
                paletteBuffer[offset++] = (byte)i;
                paletteBuffer[offset++] = (byte)i;
            }
        }

        /// <summary>
        /// Fills entire palette with random values.
        /// </summary>
        public void MakeRandom()
        {
            Random rnd = new Random();
            int offset = headerLength;
            for (int i = 0; i < 256; i++)
            {
                paletteBuffer[offset++] = (byte)(rnd.Next() % 255);
                paletteBuffer[offset++] = (byte)(rnd.Next() % 255);
                paletteBuffer[offset++] = (byte)(rnd.Next() % 255);
            }
        }

        public void Multiply(int scale)
        {
            int offset = headerLength;
            for (int i = 0; i < 256; i++)
            {
                paletteBuffer[offset] = (byte)(paletteBuffer[offset++] * scale);
                paletteBuffer[offset] = (byte)(paletteBuffer[offset++] * scale);
                paletteBuffer[offset] = (byte)(paletteBuffer[offset++] * scale);
            }
        }

        /// <summary>
        /// Fills palette with AutoMap colours. The AutoMap colour index is equal to BuildingTypes value + 1.
        /// </summary>
        public void MakeAutomap()
        {
            // Fill to most common colour
            Fill(69, 60, 40);

            // Guild halls
            Set(12, 69, 125, 195);

            // Temples
            Set(15, 69, 125, 195);

            // Taverns
            Set(16, 85, 117, 48);

            // Stores
            Set(1, 190, 85, 24);       // Alchemist
            Set(3, 190, 85, 24);       // Amorer
            Set(4, 190, 85, 24);       // Bank
            Set(6, 190, 85, 24);       // Bookseller
            Set(7, 190, 85, 24);       // Clothing Store
            Set(8, 190, 85, 24);       // Furniture Store
            Set(9, 190, 85, 24);       // Gem Store
            Set(10, 190, 85, 24);      // General Store
            Set(11, 190, 85, 24);      // Library
            Set(13, 190, 85, 24);      // Pawn Shop
            Set(14, 190, 85, 24);      // Weapon Smith

            // Special 1-4
            //Set(0x75, 255, 0, 0);
            //Set(0xe0, 255, 0, 0);
            //Set(0xfa, 255, 0, 0);

            // Ground flats
            Set(0xfb, 0, 0, 0);
        }

        /// <summary>
        /// Gets colour at specified index.
        /// </summary>
        /// <param name="Index">Index into colour array.</param>
        /// <returns>DFColor object.</returns>
        public DFColor Get(int Index)
        {
            int offset = headerLength + Index * 3;
            DFColor col = new DFColor(paletteBuffer[offset], paletteBuffer[offset + 1], paletteBuffer[offset + 2]);

            return col;
        }

        /// <summary>
        /// Gets red colour value at index.
        /// </summary>
        /// <param name="Index">Index into colour array.</param>
        /// <returns>Red value byte.</returns>
        public byte GetRed(int Index)
        {
            int offset = headerLength + Index * 3;
            return paletteBuffer[offset];
        }

        /// <summary>
        /// Gets green colour value at index.
        /// </summary>
        /// <param name="Index">Index into colour array.</param>
        /// <returns>Green value byte.</returns>
        public byte GetGreen(int Index)
        {
            int offset = headerLength + Index * 3;
            return paletteBuffer[offset + 1];
        }

        /// <summary>
        /// Gets blue colour value at index.
        /// </summary>
        /// <param name="Index">Index into colour array.</param>
        /// <returns>Blue value byte.</returns>
        public byte GetBlue(int Index)
        {
            int offset = headerLength + Index * 3;
            return paletteBuffer[offset + 2];
        }

        /// <summary>
        /// Sets index to specified RGB values.
        /// </summary>
        /// <param name="Index">Index into colour array.</param>
        /// <param name="R">Red component.</param>
        /// <param name="G">Green component.</param>
        /// <param name="B">Blue component.</param>
        public void Set(int Index, byte R, byte G, byte B)
        {
            int offset = headerLength + Index * 3;
            paletteBuffer[offset] = R;
            paletteBuffer[offset + 1] = G;
            paletteBuffer[offset + 2] = B;
        }

        /// <summary>
        /// Sets 768-byte palette buffer directly.
        /// </summary>
        /// <param name="data">Data to set.</param>
        public void Set(byte[] data)
        {
            if (data.Length != 768)
                throw new Exception("DFPalette: Invalid buffer length. Must be 768 bytes for direct set.");

            headerLength = 0;
            Array.Copy(data, paletteBuffer, 768);
        }

        /// <summary>
        /// Finds index with specified RGB values.
        /// </summary>
        /// <param name="R">Red component.</param>
        /// <param name="G">Green component.</param>
        /// <param name="B">Blue component.</param>
        /// <returns>Index of found RGB value.</returns>
        public int Find(byte R, byte G, byte B)
        {
            int offset = headerLength;
            for (int i = 0; i < 256; i++)
            {
                // Check for match
                if (paletteBuffer[offset] == R && paletteBuffer[offset + 1] == G && paletteBuffer[offset + 2] == B)
                    return i;

                // Increment offset
                offset += 3;
            }

            return -1;
        }

        #endregion
    }
}
