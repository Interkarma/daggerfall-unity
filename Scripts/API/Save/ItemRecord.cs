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
    /// Item record.
    /// SaveTreeRecordTypes = 0x02
    /// </summary>
    public class ItemRecord : SaveTreeBaseRecord
    {
        #region Fields

        ItemRecordData parsedData;

        #endregion

        #region Properties

        public ItemRecordData ParsedData
        {
            get { return parsedData; }
        }

        #endregion

        #region Structures

        /// <summary>
        /// Stores native item data exactly as read from save file.
        /// </summary>
        public struct ItemRecordData
        {
            public string name;
            public UInt16 category1;
            public UInt16 category2;
            public UInt32[] value;
            public UInt16[] hits;
            public UInt32[] picture;
            public Byte material;
            public Byte construction;
            public Byte color;
            public UInt32 weight;
            public UInt16 enchantmentPoints;
            public UInt16 message;
            public UInt16[] magic;
        }

        #endregion

        #region Constructors

        public ItemRecord()
        {
        }

        public ItemRecord(BinaryReader reader, int length)
            : base(reader, length)
        {
            ReadNativeItemData();
        }

        #endregion

        #region Private Methods

        void ReadNativeItemData()
        {
            // Must be an item type
            if (recordType != RecordTypes.Item)
                return;

            // Prepare stream
            MemoryStream stream = new MemoryStream(RecordData);
            BinaryReader reader = new BinaryReader(stream);

            // Read native item data
            parsedData = new ItemRecordData();
            parsedData.name = FileProxy.ReadCString(reader, 32);
            parsedData.category1 = reader.ReadUInt16();
            parsedData.category2 = reader.ReadUInt16();

            // Close stream
            reader.Close();
        }

        #endregion
    }
}