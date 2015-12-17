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

        NativeItemRecord nativeItem;

        #endregion

        #region Structures

        /// <summary>
        /// Stores native item data exactly as read from save file.
        /// </summary>
        struct NativeItemRecord
        {
            public string name;
            public UInt16[] category;
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
            nativeItem = new NativeItemRecord();
            nativeItem.name = FileProxy.ReadCString(reader, 20);

            // Close stream
            reader.Close();
        }

        #endregion
    }
}