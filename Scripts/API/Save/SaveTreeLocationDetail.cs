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
    /// Location detail data following header.
    /// </summary>
    public class SaveTreeLocationDetail
    {
        long position;
        int recordCount;
        SaveTreeLocationDetailRecord[] records;

        public long Position
        {
            get { return position; }
        }

        public int RecordCount
        {
            get { return recordCount; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader">Reader positioned at start of binary data.</param>
        public SaveTreeLocationDetail(BinaryReader reader)
        {
            position = reader.BaseStream.Position;
            Open(reader);
        }

        void Open(BinaryReader reader)
        {
            recordCount = reader.ReadByte();
            records = new SaveTreeLocationDetailRecord[recordCount];
            for (int i = 0; i < recordCount; i++)
            {
                records[i] = new SaveTreeLocationDetailRecord(reader);
            }
        }
    }
}