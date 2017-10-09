// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Utility;

namespace DaggerfallConnect.Save
{
    /// <summary>
    /// Container record.
    /// SaveTreeRecordTypes = 0x34
    /// </summary>
    public class ContainerRecord : SaveTreeBaseRecord
    {
        #region Fields

        ContainerRecordData parsedData;

        #endregion

        #region Properties

        public ContainerRecordData ParsedData
        {
            get { return parsedData; }
            set { parsedData = value; }
        }

        /// <summary>
        /// Returns true if this is a wagon.
        /// </summary>
        public bool IsWagon
        {
            get { return WagonCheck(); }
        }

        #endregion

        #region Structures

        /// <summary>
        /// Container records have a variable number of unknown bytes.
        /// Can often be just a single byte, so does not seem to be reliably useful.
        /// First byte seems to indicate wagon container if value is 150.
        /// </summary>
        public struct ContainerRecordData
        {
            public byte[] unknown;
        }

        #endregion

        #region Constructors

        public ContainerRecord()
        {
        }

        public ContainerRecord(BinaryReader reader, int length)
            : base(reader, length)
        {
            ReadNativeContainerData();
        }

        #endregion

        #region Private Methods

        void ReadNativeContainerData()
        {
            // Must be a container type
            if (recordType != RecordTypes.Container)
                return;

            // Prepare stream
            MemoryStream stream = new MemoryStream(RecordData);
            BinaryReader reader = new BinaryReader(stream);

            // Read container data
            parsedData = new ContainerRecordData();
            parsedData.unknown = reader.ReadBytes(RecordLength);

            // Close stream
            reader.Close();
        }

        bool WagonCheck()
        {
            // Must have data
            if (parsedData.unknown == null || parsedData.unknown.Length == 0)
                return false;

            if (recordRoot.SpriteIndex == 4)
                return true;
            else
                return false;
        }

        #endregion
    }
}