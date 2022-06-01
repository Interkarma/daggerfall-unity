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

using System.IO;

namespace DaggerfallConnect.Save
{
    /// <summary>
    /// Trapped Soul record.
    /// SaveTreeRecordTypes = 0x14
    /// </summary>
    public class TrappedSoulRecord : SaveTreeBaseRecord
    {
        #region Fields

        TrappedSoulRecordData parsedData;

        #endregion

        #region Properties

        public TrappedSoulRecordData ParsedData
        {
            get { return parsedData; }
            set { parsedData = value; }
        }

        #endregion

        #region Structures

        /// <summary>
        /// Soul records have a 2 byte monster id at offset 0x1B which is part of the root record so this is not used.
        /// </summary>
        public struct TrappedSoulRecordData
        {
            public byte[] unknown;
        }

        #endregion

        #region Constructors

        public TrappedSoulRecord()
        {
        }

        public TrappedSoulRecord(BinaryReader reader, int length)
            : base(reader, length)
        {
            ReadNativeContainerData();
        }

        #endregion

        #region Private Methods

        void ReadNativeContainerData()
        {
            // Must be a trapped soul type and record must not be failed
            if (recordType != RecordTypes.TrappedSoul || IsFailedRecord)
                return;

            // Prepare stream
            MemoryStream stream = new MemoryStream(RecordData);
            BinaryReader reader = new BinaryReader(stream);

            // Read container data - seems always is a single byte value = 150, use unknown
            parsedData = new TrappedSoulRecordData();
            parsedData.unknown = reader.ReadBytes(RecordLength);

            // Close stream
            reader.Close();
        }

        #endregion
    }
}
