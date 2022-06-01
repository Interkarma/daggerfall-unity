// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallConnect.FallExe;

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Simple reader class to extract MAGIC.DEF item data.
    /// </summary>
    public class MagicItemsFile
    {
        #region Fields

        const int nameLength = 32;

        readonly FileProxy magicItemsFile = new FileProxy();
        readonly List<MagicItemTemplate> magicItems = new List<MagicItemTemplate>();

        #endregion

        #region Properties

        public List<MagicItemTemplate> MagicItemsList
        {
            get { return magicItems; }
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

        MagicItemTemplate ReadNextMagicItem(BinaryReader reader)
        {
            MagicItemTemplate magicItem = new MagicItemTemplate();
            magicItem.index = reader.BaseStream.Position;
            magicItem.name = FileProxy.ReadCString(reader, nameLength);
            magicItem.type = (MagicItemTypes)reader.ReadByte();
            magicItem.group = reader.ReadByte();
            magicItem.groupIndex = reader.ReadByte();

            // Read enchantments
            magicItem.enchantments = new DaggerfallEnchantment[10];
            for (int i = 0; i < 10; i++)
            {
                magicItem.enchantments[i].type = (EnchantmentTypes)reader.ReadSByte();
                magicItem.enchantments[i].param = reader.ReadSByte();
            }

            magicItem.uses = reader.ReadInt16();
            magicItem.value = reader.ReadInt32();
            magicItem.material = reader.ReadByte();

            return magicItem;
        }

        #endregion
    }
}
