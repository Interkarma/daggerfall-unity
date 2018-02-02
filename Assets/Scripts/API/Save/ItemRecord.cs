// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
            set { parsedData = value; }
        }

        #endregion

        #region Structures and Enumerations

        /// <summary>
        /// Stores native item data exactly as read from save file.
        /// </summary>
        public struct ItemRecordData
        {
            public string name;
            public UInt16 group;
            public UInt16 index;
            public UInt32 value;
            public UInt16 unknown;
            public UInt16 flags;
            public UInt16 currentCondition;
            public UInt16 maxCondition;
            public Byte unknown2;
            public Byte typeDependentData;          // Stack count for arrows. Recipe ID for potion recipes and potions.
            public UInt16 image1;                   // Inventory list and equip image
            public UInt16 image2;                   // 3D world image. These were used in the Daggerfall demo.
            public UInt16 material;
            public Byte color;
            public UInt32 weight;
            public UInt16 enchantmentPoints;
            public UInt32 message;
            public UInt16[] magic;
        }

        /// <summary>
        /// Item flags.
        /// </summary>
        [Flags]
        public enum ItemFlags
        {
            None = 0x00,
            IngredientRegular = 0x01,
            EdgedWeapon = 0x04,
            IngredientLiquid = 0x09,
            BluntWeapon = 0x10,
            Enchanted = 0x20,
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

        #region Public Methods

        public void CopyTo(ItemRecord other)
        {
            // Copy base record data
            base.CopyTo(other);

            // Copy item data
            other.parsedData = this.parsedData;
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
            parsedData.group = reader.ReadUInt16(); // 32
            parsedData.index = reader.ReadUInt16(); // 34
            parsedData.value = reader.ReadUInt32(); // 36
            parsedData.unknown = reader.ReadUInt16(); // 40
            parsedData.flags = reader.ReadUInt16(); // 42
            parsedData.currentCondition = reader.ReadUInt16(); // 44
            parsedData.maxCondition = reader.ReadUInt16(); // 46
            parsedData.unknown2 = reader.ReadByte(); // 48
            parsedData.typeDependentData = reader.ReadByte(); // 49
            parsedData.image1 = reader.ReadUInt16(); // 50
            parsedData.image2 = reader.ReadUInt16(); // 52
            parsedData.material = reader.ReadUInt16(); // 54
            parsedData.color = reader.ReadByte(); //56
            parsedData.weight = reader.ReadUInt32();
            parsedData.enchantmentPoints = reader.ReadUInt16();
            parsedData.message = reader.ReadUInt32();

            // Read magic effect array
            const int effectCount = 10;
            parsedData.magic = new ushort[effectCount];
            for (int i = 0; i < effectCount; i++)
            {
                parsedData.magic[i] = reader.ReadUInt16();
            }

            // Close stream
            reader.Close();
        }

        #endregion
    }
}