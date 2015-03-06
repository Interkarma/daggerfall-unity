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
    /// Connects to a SKY??.DAT file to enumerate and extract image data.
    /// </summary>
    public class SkyFile : BaseImageFile
    {
        #region Class Variables

        // Class constants
        private const int frameWidth = 512;
        private const int frameHeight = 220;
        private const int frameDataLength = frameWidth * frameHeight;
        private const int paletteDataLength = 776;
        private const long paletteDataPosition = 0;
        private const long imageDataPosition = 549120;

        /// <summary>
        /// Palette array.
        /// </summary>
        private DFPalette[] palettes = new DFPalette[32];

        /// <summary>
        /// Bitmap array.
        /// </summary>
        private DFBitmap[] bitmaps = new DFBitmap[64];

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SkyFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to SKY??.DAT file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public SkyFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Number of records in this Sky file. Always 2 for a SKY file (one animation each for east and west sky).
        /// </summary>
        public override int RecordCount
        {
            get
            {
                if (managedFile.FilePath == string.Empty)
                    return 0;
                else
                    return 2;
            }

        }

        /// <summary>
        /// SKY files are fully palettized per frame.
        ///  This method always returns string.Empty and is implemented only to satisfy abstract base class DFImage.
        ///  Use GetDFPalette(Frame) instead.
        /// </summary>
        public override string PaletteName
        {
            get { return string.Empty;  }
        }

        /// <summary>
        /// Description of this file (always "SKY File" as the game files contain no text descriptions for this file type).
        /// </summary>
        public override string Description
        {
            get { return "SKY File"; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a SKY file.
        /// </summary>
        /// <param name="filePath">Absolute path to SKY??.DAT file</param>
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
            if (!fn.StartsWith("SKY") && !fn.EndsWith(".DAT"))
                return false;

            // Load file
            if (!managedFile.Load(filePath, usage, readOnly))
                return false;

            return true;
        }

        /// <summary>
        /// Gets palette data for specified record and frame.
        /// </summary>
        /// <param name="frame">Index of frame.</param>
        /// <returns>DFPalette object or null.</returns>
        public DFPalette GetDFPalette(int frame)
        {
            // Validate
            if (frame < 0 || frame >= 32)
                return null;

            // Read palette data
            ReadPalette(frame);

            return palettes[frame];
        }

        /// <summary>
        /// Gets bitmap data as indexed 8-bit byte array.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <param name="frame">Index of frame.</param>
        /// <returns>DFBitmap object. Check DFBitmap.Data for null on failure.</returns>
        public override DFBitmap GetDFBitmap(int record, int frame)
        {
            // Validate
            if (record < 0 || record > 1 || frame < 0 || frame > 31)
                return new DFBitmap();

            // Calculate index
            int Index = record * 32 + frame;

            // Read image data
            ReadImageData(Index);

            return bitmaps[Index];
        }

        /// <summary>
        /// Gets number of frames in specified record. Always 32 for SKY files.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>Number of frames or -1 on error.</returns>
        public override int GetFrameCount(int record)
        {
            // Validate
            if (string.IsNullOrEmpty(managedFile.FileName) || record < 0 || record >= 2)
                return -1;

            return 32;
        }

        /// <summary>
        /// Gets width and height of specified record. All frames of this record are the same dimensions.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>Size object.</returns>
        public override DFSize GetSize(int record)
        {
            // Validate
            if (string.IsNullOrEmpty(managedFile.FileName) || record < 0 || record >= 2)
                return new DFSize(0, 0);

            return new DFSize(frameWidth, frameHeight);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Returns a SKY??.DAT filename based on index.
        /// If the index is not valid, the returned filename will also be invalid.
        /// </summary>
        /// <param name="skyIndex">Index of sky archive.</param>
        /// <returns>Texture filename in the format SKY??.DAT.</returns>
        public static string IndexToFileName(int skyIndex)
        {
            return string.Format("SKY{0:00}.DAT", skyIndex);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Read palette for specified record.
        /// </summary>
        /// <param name="index">Index of palette.</param>
        private void ReadPalette(int index)
        {
            // Read palette data if not already stored
            if (null == palettes[index])
            {
                BinaryReader Reader = managedFile.GetReader(paletteDataPosition + (776 * index) + 8);
                palettes[index] = new DFPalette();
                palettes[index].Read(ref Reader);
            }
        }

        /// <summary>
        /// Reads image data.
        /// </summary>
        /// <param name="index">Index of image.</param>
        private bool ReadImageData(int index)
        {
            // Read image if not already stored
            if (null == bitmaps[index])
            {
                BinaryReader Reader = managedFile.GetReader(imageDataPosition + (frameDataLength * index));
                bitmaps[index] = new DFBitmap();
                bitmaps[index].Width = frameWidth;
                bitmaps[index].Height = frameHeight;
                bitmaps[index].Stride = frameWidth;
                bitmaps[index].Format = DFBitmap.Formats.Indexed;
                bitmaps[index].Data = Reader.ReadBytes(frameDataLength);
            }

            return true;
        }

        #endregion
    }
}
