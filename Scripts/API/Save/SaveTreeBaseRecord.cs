// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
    /// Base record of SAVETREE.DAT.
    /// </summary>
    public class SaveTreeBaseRecord
    {
        protected long position;
        protected int recordLength;
        protected SaveTreeRecordTypes recordType;
        protected byte[] recordData;

        public long Position
        {
            get { return position; }
        }

        public int RecordLength
        {
            get { return recordLength; }
        }

        public byte[] RecordData
        {
            get { return recordData; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader">Reader positioned at start of binary data.</param>
        public SaveTreeBaseRecord(BinaryReader reader)
        {
            Open(reader);
        }

        protected virtual void Open(BinaryReader reader)
        {
            recordLength = reader.ReadInt32();
            recordType = PeekRecordType(reader);
            if (recordType == SaveTreeRecordTypes.DungeonData)
                recordLength *= 39;

            recordData = reader.ReadBytes(recordLength);
        }

        protected SaveTreeRecordTypes PeekRecordType(BinaryReader reader)
        {
            long position = reader.BaseStream.Position;
            byte result = reader.ReadByte();
            reader.BaseStream.Position = position;

            return (SaveTreeRecordTypes)result;
        }
    }
}