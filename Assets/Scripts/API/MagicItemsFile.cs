// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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
using System.Collections.Generic;
using DaggerfallConnect.Utility;

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Simple reader class to extract MAGIC.DEF item data.
    /// </summary>
    public class MagicItemsFile
    {
        #region Fields

        const int nameLength = 32;

        FileProxy magicItemsFile = new FileProxy();
        List<DFMagicItem> magicItems = new List<DFMagicItem>();

        #endregion

        #region Properties

        public List<DFMagicItem> MagicItemsList
        {
            get { return magicItems; }
        }

        #endregion

        #region Structs & Enums

        public struct DFMagicItem
        {
            public long position;
            public string name;
            public MagicItemTypes type;
            public byte group;
            public byte groupIndex;
            public short[] enchantments;
            public short uses;
            public ushort unknown1;
            public byte material;
            public short unknown2;
        }

        public enum MagicItemTypes
        {
            RegularMagicItem,
            ArtifactClass1,
            ArtifactClass2,
        }

        #endregion

        #region Constructors

        public MagicItemsFile(string filePath, FileUsage usage = FileUsage.UseMemory, bool readOnly = true)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Private Methods

        public bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Validate filename
            if (!filePath.EndsWith("MAGIC.DEF", StringComparison.InvariantCultureIgnoreCase))
                return false;

            // Open file
            if (!magicItemsFile.Load(filePath, usage, readOnly))
                return false;

            // Read file
            ReadMagicItems();

            return true;
        }

        void ReadMagicItems()
        {
            // Get file reader
            BinaryReader reader = magicItemsFile.GetReader();

            // Read all magic items
            int recordCount = reader.ReadInt32();
            for (int i = 0; i < recordCount; i++)
            {
                magicItems.Add(ReadNextMagicItem(reader));
            }
        }

        DFMagicItem ReadNextMagicItem(BinaryReader reader)
        {
            DFMagicItem magicItem = new DFMagicItem();
            magicItem.position = reader.BaseStream.Position;
            magicItem.name = FileProxy.ReadCString(reader, nameLength);
            magicItem.type = (MagicItemTypes)reader.ReadByte();
            magicItem.group = reader.ReadByte();
            magicItem.groupIndex = reader.ReadByte();

            // Read enchantments
            magicItem.enchantments = new short[10];
            for (int i = 0; i < 10; i++)
            {
                magicItem.enchantments[i] = reader.ReadInt16();
            }

            magicItem.uses = reader.ReadInt16();
            magicItem.unknown1 = reader.ReadUInt16();
            magicItem.material = reader.ReadByte();
            magicItem.unknown2 = reader.ReadInt16();

            return magicItem;
        }

        #endregion
    }
}