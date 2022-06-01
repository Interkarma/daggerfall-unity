// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 
// Notes:
//

using System;
using DaggerfallConnect.Utility;

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Reads data from PAINT.DAT.
    /// </summary>
    public class PaintFile
    {
        #region Class Variables

        /// <summary>Length of each record in bytes.</summary>
        public const int recordLength = 40;

        /// <summary>
        /// Abstracts PAINT.DAT file to a managed disk or memory stream.
        /// </summary>
        private readonly FileProxy managedFile = new FileProxy();

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets default PAINT.DAT filename.
        /// </summary>
        static public string Filename
        {
            get { return "PAINT.DAT"; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PaintFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to PAINT.DAT.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public PaintFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load PAINT.DAT file.
        /// </summary>
        /// <param name="filePath">Absolute path to PAINT.DAT file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Validate filename
            if (!filePath.EndsWith(Filename, StringComparison.InvariantCultureIgnoreCase))
                return false;

            // Load file into memory
            if (!managedFile.Load(filePath, usage, readOnly))
                return false;

            return true;
        }

        #endregion

        #region Readers

        /// <summary>
        /// Read file.
        /// </summary>
        public byte[] Read(uint recordIndex)
        {
            return managedFile.GetBytes(recordIndex * recordLength, recordLength);
        }

        #endregion
    }
}
