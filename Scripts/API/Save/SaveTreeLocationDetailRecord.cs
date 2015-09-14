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
    /// Location detail records do not follow the usual RecordBase structure.
    /// </summary>
    public class SaveTreeLocationDetailRecord
    {
        long position;
        int recordLength;
        byte[] recordData;

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
        public SaveTreeLocationDetailRecord(BinaryReader reader)
        {
            position = reader.BaseStream.Position;
            Open(reader);
        }

        void Open(BinaryReader reader)
        {
            recordLength = reader.ReadInt32();
            if (recordLength > 0)
                recordData = reader.ReadBytes(recordLength);
        }
    }
}