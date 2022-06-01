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
    /// Connects to *.BSS files to extract image data.
    /// </summary>
    public class BssFile : BaseImageFile
    {
        #region Fields

        Header header;
        public DFBitmap[] frames;

        #endregion

        #region Properties

        /// <summary>
        /// Number of image records in this BSS file.
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
        /// Gets palette name for BSS file.
        /// </summary>
        public override string PaletteName
        {
            get { return "ART_PAL.COL"; }
        }

        /// <summary>
        /// Description of this file (always "BSS File" as game data contain no text descriptions for this file type).
        /// </summary>
        public override string Description
        {
            get { return "BSS File"; }
        }

        #endregion

        #region Structures

        struct Header
        {
            public Int16 XPos;
            public Int16 YPos;
            public Int16 Width;
            public Int16 Height;
            public Int16 FrameCount;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BssFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to *.BSS file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public BssFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a BSS file.
        /// </summary>
        /// <param name="filePath">Absolute path to *.BSS file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public override bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Exit if this file already loaded
            if (managedFile.FilePath == filePath)
                return true;

            // Validate filename
            if (!filePath.EndsWith(".BSS", StringComparison.InvariantCultureIgnoreCase))
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
        /// <param name="record">Record index. Only 1 record in Daggerfall BSS files so value should be 0.</param>
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
            if (record < 0 || record >= RecordCount || frame >= GetFrameCount(record) || frames == null || frames.Length == 0)
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
            header.XPos = reader.ReadInt16();
            header.YPos = reader.ReadInt16();
            header.Width = reader.ReadInt16();
            header.Height = reader.ReadInt16();
            header.FrameCount = reader.ReadInt16();
        }

        // Read image data for all frames
        void ReadImageData(BinaryReader reader)
        {
            frames = new DFBitmap[header.FrameCount];
            for (int i = 0; i < header.FrameCount; i++)
            {
                DFBitmap bitmap = new DFBitmap(header.Width, header.Height);
                bitmap.Data = reader.ReadBytes(header.Width * header.Height);
                bitmap.Palette = Palette;
                frames[i] = bitmap;
            }
        }

        #endregion
    }
}
