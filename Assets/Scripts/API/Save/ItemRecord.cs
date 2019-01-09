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
using System.IO;
using DaggerfallConnect.Utility;
using DaggerfallConnect.FallExe;

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
            public DaggerfallEnchantment[] magic;
        }

        /// <summary>
        /// Item flags.
        /// </summary>
        [Flags]
        public enum ItemFlags
        {
            None = 0x00,
            IngredientRegular = 0x01,
            OneHandedWeapon = 0x04,
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

            // Item names should only be read until the null terminator.
            long pos = reader.BaseStream.Position;
            parsedData.name = FileProxy.ReadCString(reader, 0);
            reader.BaseStream.Position = pos + 32;

            parsedData.group = reader.ReadUInt16();
            parsedData.index = reader.ReadUInt16();
            parsedData.value = reader.ReadUInt32();
            parsedData.unknown = reader.ReadUInt16();
            parsedData.flags = reader.ReadUInt16();
            parsedData.currentCondition = reader.ReadUInt16();
            parsedData.maxCondition = reader.ReadUInt16();
            parsedData.unknown2 = reader.ReadByte();
            parsedData.typeDependentData = reader.ReadByte();
            parsedData.image1 = reader.ReadUInt16();
            parsedData.image2 = reader.ReadUInt16();
            parsedData.material = reader.ReadUInt16();
            parsedData.color = reader.ReadByte();
            parsedData.weight = reader.ReadUInt32();
            parsedData.enchantmentPoints = reader.ReadUInt16();
            parsedData.message = reader.ReadUInt32();

            // Read magic effect array
            const int effectCount = 10;
            parsedData.magic = new DaggerfallEnchantment[effectCount];
            for (int i = 0; i < effectCount; i++)
            {
                parsedData.magic[i].type = (EnchantmentTypes)reader.ReadInt16();
                parsedData.magic[i].param = reader.ReadInt16();
            }

            // Close stream
            reader.Close();
        }

        #endregion
    }
}
