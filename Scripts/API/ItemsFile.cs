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
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Utility;

namespace DaggerfallConnect.FallExe
{
    /// <summary>
    /// Stores native item template data exactly as read from FALL.EXE.
    /// </summary>
    [Serializable]
    public struct DFItem
    {
        public long position;                       // Position record was read from FALL.EXE
        public Byte[] name;                         // Display name
        public Int32 baseWeightUnits;               // Base weight in number of 0.25 units
        public Int16 hitPoints;                     // Hit points
        public Int32 Unknown1;                      // Holding weight? - From 20 and up seems to be holding weight of container-like items (e.g. bookcase, chest, cart, ship)
        public Int32 baseCost;                      // Base cost before material, mercantile, etc.
        public Int16 enchantmentPoints;             // Enchantment points
        public Int32 Unknown2Long;                  // Unknown2 read as 32-bits wide
        public Int16 Unknown2;                      // Unknown
        public Byte drawOrder;                      // Ordering of items on paper doll - lower numbered items are drawn first
        public Byte Unknown3;                       // Item type? (known: value & 1 == ingredients) - could be part of a larger bitfield with drawOrder
        public Int16 inventoryTextureBitfield;      // Bitfield for generic texture indices - does not appear to be used for weapons & armor
        public Int16 unknownTextureBitfield;        // Bitfield for unknown texture indices, usually a generic icon - seems to be 0 for icons that do not appear on paper doll
    }

    /// <summary>
    /// Stores item template data in a more friendly format.
    /// Provides some convenience such as splitting out texture bitfields.
    /// Passing through unknown values as-is.
    /// </summary>
    [Serializable]
    public struct ItemTemplate
    {
        public int position;                        // Position in file this item was read from
        public int index;                           // Index of this item in list
        public string name;                         // Display name
        public float baseWeight;                    // Base weight before material, etc.
        public int hitPoints;                       // Hit points
        public Int32 Unknown1;                      // Holding weight? - From 20 and up seems to be holding weight of container-like items (e.g. bookcase, chest, cart, ship)
        public int baseCost;                        // Base cost before material, mercantile, etc.
        public int enchantmentPoints;               // Enchantment points
        public Int32 Unknown2Long;                  // Unknown
        public Int16 Unknown2;                      // Unknown
        public byte drawOrder;                      // Ordering of items on paper doll - lower numbered items are drawn first
        public Byte Unknown3;                       // Item type? (known: value & 1 == ingredients) - could be part of a larger bitfield with drawOrder
        public bool isIngredient;                   // True for ingedient items, derived from Unknown3
        public int inventoryTextureArchive;         // Generic texture archive index
        public int inventoryTextureRecord;          // Generic texture record index
        public int unknownTextureArchive;           // Unknown texture archive index
        public int unknownTextureRecord;            // Unknown texture record index
    }

    /// <summary>
    /// Reads item template data from FALL.EXE and structures for use.
    /// There is no "items file" as such for this data, just keeping
    /// naming conveniention similar to other API classes.
    /// </summary>
    public class ItemsFile
    {
        #region Fields

        const string rubyString = "Ruby";
        const string fallExeFilename = "FALL.EXE";
        const int defaultItemsOffset = 1776954;
        const int nameLength = 24;
        const int recordLength = 48;
        const int totalItems = 288;

        bool isOpen = false;
        int itemsOffset = defaultItemsOffset;
        FileProxy fallExeFile = new FileProxy();
        List<DFItem> items = new List<DFItem>();
        Exception lastException = new Exception();

        #endregion

        #region Properties

        /// <summary>
        /// Gets static FALL.EXE filename.
        /// </summary>
        public static string Filename
        {
            get { return fallExeFilename; }
        }

        /// <summary>
        /// Gets path to FALL.EXE file.
        /// </summary>
        public string FilePath
        {
            get { return fallExeFile.FilePath; }
        }

        /// <summary>
        /// Gets array of native item data.
        /// </summary>
        public DFItem[] ItemsArray
        {
            get { return items.ToArray(); }
        }

        /// <summary>
        /// Gets or sets the items offset.
        /// </summary>
        public int ItemsOffset
        {
            get { return itemsOffset; }
            set { itemsOffset = value; }
        }

        /// <summary>
        /// Gets number of items enumerated.
        /// </summary>
        public int ItemsCount
        {
            get { return items.Count; }
        }

        /// <summary>
        /// Gets file open flag.
        /// </summary>
        public bool IsOpen
        {
            get { return isOpen; }
        }

        /// <summary>
        /// Gets last exception.
        /// </summary>
        public Exception LastException
        {
            get { return lastException; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ItemsFile()
        {
        }

        /// <summary>
        /// Path constructor.
        /// </summary>
        /// <param name="fallExePath">Path to FALL.EXE.</param>
        public ItemsFile(string fallExePath)
            : base()
        {
            OpenFallExeFile(fallExePath);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens FALL.EXE file.
        /// </summary>
        /// <param name="fallExePath">Path to FALL.EXE file.</param>
        /// <param name="fileUsage">How to load file</param>
        /// <param name="readOnly">Open file read-only</param>
        /// <returns>True if successful.</returns>
        public bool OpenFallExeFile(string fallExePath, FileUsage fileUsage = FileUsage.UseMemory, bool readOnly = true)
        {
            isOpen = false;
            items.Clear();

            // Must be FALL.EXE
            if (string.Compare(Path.GetFileName(fallExePath), Filename, true) != 0)
                return false;

            // Open file
            if (!fallExeFile.Load(fallExePath, fileUsage, readOnly))
                return false;

            // Read file
            if (!ReadFallExeFile())
                return false;

            isOpen = true;

            return true;
        }

        /// <summary>
        /// Gets native item data as read from FALL.EXE.
        /// </summary>
        /// <param name="index">index of item.</param>
        /// <returns>DFItem</returns>
        public DFItem GetItem(int index)
        {
            DFItem item = new DFItem();
            if (items.Count > 0 && index >= 0 && index < items.Count)
            {
                item = items[index];
            }

            return item;
        }

        /// <summary>
        /// Gets a item description pre-formatted into friendly data.
        /// </summary>
        /// <param name="index">Index of item.</param>
        /// <returns>ItemTemplate</returns>
        public ItemTemplate GetItemDescription(int index)
        {
            ItemTemplate desc = new ItemTemplate();
            if (items.Count > 0 && index >= 0 && index < items.Count)
            {
                DFItem item = items[index];
                desc.position = (int)item.position;
                desc.index = index;
                desc.name = Encoding.UTF8.GetString(item.name).TrimEnd('\0');
                desc.baseWeight = (float)item.baseWeightUnits * 0.25f;
                desc.hitPoints = item.hitPoints;
                desc.Unknown1 = item.Unknown1;
                desc.enchantmentPoints = item.enchantmentPoints;
                desc.Unknown2Long = item.Unknown2Long;
                desc.Unknown2 = item.Unknown2;
                desc.drawOrder = item.drawOrder;
                desc.Unknown3 = item.Unknown3;
                desc.isIngredient = ((item.Unknown3 & 1) == 1) ? true : false;
                desc.inventoryTextureArchive = item.inventoryTextureBitfield >> 7;
                desc.inventoryTextureRecord = item.inventoryTextureBitfield & 0x7f;
                desc.unknownTextureArchive = item.unknownTextureBitfield >> 7;
                desc.unknownTextureRecord = item.unknownTextureBitfield & 0x7f;
            }
            
            return desc;
        }

        #endregion

        #region Research

        /// <summary>
        /// Rewrites item data back into disk file.
        /// Must have been opened with FileUsage.UseDisk and readOnly flag false.
        /// </summary>
        /// <param name="item">Item to rewrite.</param>
        public void RewriteItem(DFItem item)
        {
            if (isOpen && fallExeFile.Usage == FileUsage.UseDisk && fallExeFile.ReadOnly == false)
            {
                BinaryWriter writer = fallExeFile.GetWriter();
                writer.BaseStream.Position = item.position;
                writer.Write(item.name);
                writer.Write(item.baseWeightUnits);
                writer.Write(item.hitPoints);
                writer.Write(item.Unknown1);
                writer.Write(item.baseCost);
                writer.Write(item.enchantmentPoints);
                writer.Write(item.Unknown2);
                writer.Write(item.drawOrder);
                writer.Write(item.Unknown3);
                writer.Write(item.inventoryTextureBitfield);
                writer.Write(item.unknownTextureBitfield);
                writer.Close();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads item data from FALL.EXE.
        /// </summary>
        private bool ReadFallExeFile()
        {
            // Get items offset
            itemsOffset = FindItemsOffset();
            if (itemsOffset == -1)
                return false;

            // Get file reader
            BinaryReader reader = fallExeFile.GetReader(itemsOffset);

            // Try reading items
            try
            {
                for (int i = 0; i < totalItems; i++)
                {
                    DFItem item = ReadNextItem(reader);
                    items.Add(item);
                }
            }
            catch (Exception ex)
            {
                lastException = ex;
                items.Clear();
                isOpen = false;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the item data can be found in file provided.
        /// </summary>
        /// <returns>Offset to start of items data or -1 if offset not found.</returns>
        private int FindItemsOffset()
        {
            // The first item in database should be "Ruby"
            if (fallExeFile.ReadCString(itemsOffset, rubyString.Length) == rubyString)
                return itemsOffset;

            // Otherwise search for offset to "Ruby" in file data
            return fallExeFile.FindString(rubyString);
        }

        private DFItem ReadNextItem(BinaryReader reader)
        {
            DFItem item = new DFItem();
            item.position = reader.BaseStream.Position;
            item.name = reader.ReadBytes(nameLength);
            item.baseWeightUnits = reader.ReadInt32();
            item.hitPoints = reader.ReadInt16();
            item.Unknown1 = reader.ReadInt32();
            item.baseCost = reader.ReadInt32();
            item.enchantmentPoints = reader.ReadInt16();

            long pos = reader.BaseStream.Position;
            item.Unknown2Long = reader.ReadInt32();
            reader.BaseStream.Position = pos;

            item.Unknown2 = reader.ReadInt16();
            item.drawOrder = reader.ReadByte();
            item.Unknown3 = reader.ReadByte();
            item.inventoryTextureBitfield = reader.ReadInt16();
            item.unknownTextureBitfield = reader.ReadInt16();

            return item;
        }

        #endregion
    }
}