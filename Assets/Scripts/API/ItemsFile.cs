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
using System.Text;
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
        public Int32 baseWeightUnits;               // Base weight in 0.25kg units
        public UInt16 hitPoints;                    // Hit points
        public Int32 capacityOrTarget;              // Capacity of container or target of effect
        public Int32 basePrice;                     // Base price before material, mercantile, etc. modify value
        public Int16 enchantmentPoints;             // Base enchantment points before material
        public Byte rarity;                         // Rarity of item appearing in buildings. Building quality must be at least equal this for item to appear.
        public Byte variants;                       // Number of variants for wearable items, unknown for non-wearable items
        public Byte drawOrderOrEffect;              // Ordering of items on paper doll (sort lowest to highest) or effect for ingredients
        public Byte propertiesBitfield;             // Bitfield with some other item properties
        public Int16 worldTextureBitfield;          // Bitfield for unknown texture indices, seems to be world icon (e.g. when dropped)
        public Int16 playerTextureBitfield;         // Bitfield for player inventory texture indices
    }

    /// <summary>
    /// Stores item template data in a more friendly format.
    /// Provides some convenience such as splitting out texture bitfields.
    /// </summary>
    [Serializable]
    public struct ItemTemplate
    {
        public int index;                           // Index of this item in list
        public string name;                         // Display name
        public float baseWeight;                    // Base weight in kilograms before material, etc.
        public int hitPoints;                       // Hit points
        public int capacityOrTarget;                // Capacity of container or target of effect
        public int basePrice;                       // Base price before material, mercantile, etc. modify value
        public int enchantmentPoints;               // Base enchantment points before material
        public byte rarity;                         // Rarity of item appearing in buildings. Building quality must be at least equal this for item to appear.
        public byte variants;                       // Number of variants for wearable items, unknown for non-wearable items
        public byte drawOrderOrEffect;              // Ordering of items on paper doll (sort lowest to highest) or effect for ingredients
        public bool isBluntWeapon;                  // True for blunt weapons
        public bool isLiquid;                       // True for liquids
        public bool isOneHanded;                    // True for one-handed item/weapons
        public bool isIngredient;                   // True for ingedient items
        public int worldTextureArchive;             // World texture archive index
        public int worldTextureRecord;              // World texture record index
        public int playerTextureArchive;            // Player inventory texture archive index
        public int playerTextureRecord;             // Player inventory texture record index

        // DFU extension fields
        public bool isNotRepairable;                // Defaults to false if not specified
    }

    /// <summary>
    /// Stores template data for magic items and artifacts.
    /// </summary>
    [Serializable]
    public struct MagicItemTemplate
    {
        public long index;                          // Index of this item in lit
        public string name;                         // Display name
        public MagicItemTypes type;                 // Type of magic item
        public byte group;                          // Group in item templates
        public byte groupIndex;                     // Group index (subgroup) in item templates
        public DaggerfallEnchantment[] enchantments;// Array of legacy enchantments on this item
        public short uses;                          // Number of uses/Item condition
        public int value;                           // Only used for artifacts
        public byte material;                       // Material
    }

    /// <summary>
    /// Daggerfall enchantment data.
    /// </summary>
    public struct DaggerfallEnchantment
    {
        public EnchantmentTypes type;
        public short param;                           // A SPELLS.STD spell ID, an identifier for a unique artifact effect, enemy group affected by bonus to hit, social group affected by reputation modifier
    }

    /// <summary>
    /// Magic item types.
    /// </summary>
    public enum MagicItemTypes
    {
        RegularMagicItem,
        ArtifactClass1,
        ArtifactClass2,
    }

    /// <summary>
    /// Enchantment types
    /// </summary>
    public enum EnchantmentTypes
    {
        None = -1,
        CastWhenUsed = 0,
        CastWhenHeld = 1,
        CastWhenStrikes = 2,
        ExtraSpellPts = 3,
        PotentVs = 4,
        RegensHealth = 5,
        VampiricEffect = 6,
        IncreasedWeightAllowance = 7,
        RepairsObjects = 8,
        AbsorbsSpells = 9,
        EnhancesSkill = 10,
        FeatherWeight = 11,
        StrengthensArmor = 12,
        ImprovesTalents = 13,
        GoodRepWith = 14,
        SoulBound = 15,
        ItemDeteriorates = 16,
        UserTakesDamage = 17,
        VisionProblems = 18,
        WalkingProblems = 19,
        LowDamageVs = 20,
        HealthLeech = 21,
        BadReactionsFrom = 22,
        ExtraWeight = 23,
        WeakensArmor = 24,
        BadRepWith = 25,
        SpecialArtifactEffect = 26,
    }

    public struct BookMappingTemplate
    {
        public int id;
        public string title;
    }

    public struct RecipeMapping
    {
        public string name;
        public Recipe[] recipes;
    }

    public struct Recipe
    {
        public Ingredient[] ingredients;
    }

    public struct Ingredient
    {
        public string name;
        public int id;
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
        //const int recordLength = 48;
        const int totalItems = 288;

        bool isOpen = false;
        int itemsOffset = defaultItemsOffset;
        readonly FileProxy fallExeFile = new FileProxy();
        readonly List<DFItem> items = new List<DFItem>();
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
                desc.index = index;
                desc.name = Encoding.UTF8.GetString(item.name).TrimEnd('\0');
                desc.baseWeight = (float)item.baseWeightUnits * 0.25f;
                desc.hitPoints = item.hitPoints;
                desc.capacityOrTarget = item.capacityOrTarget;
                desc.basePrice = item.basePrice;
                desc.enchantmentPoints = item.enchantmentPoints;
                desc.rarity = item.rarity;
                desc.variants = item.variants;
                desc.drawOrderOrEffect = item.drawOrderOrEffect;
                desc.isBluntWeapon = (((item.propertiesBitfield >> 4) & 1) == 1);
                desc.isLiquid = (((item.propertiesBitfield >> 3) & 1) == 1);
                desc.isOneHanded = (((item.propertiesBitfield >> 2) & 1) == 1);
                desc.isIngredient = ((item.propertiesBitfield & 1) == 1);
                desc.worldTextureArchive = item.worldTextureBitfield >> 7;
                desc.worldTextureRecord = item.worldTextureBitfield & 0x7f;
                desc.playerTextureArchive = item.playerTextureBitfield >> 7;
                desc.playerTextureRecord = item.playerTextureBitfield & 0x7f;
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
            if (isOpen && fallExeFile.Usage == FileUsage.UseDisk && !fallExeFile.ReadOnly)
            {
                BinaryWriter writer = fallExeFile.GetWriter();
                writer.BaseStream.Position = item.position;
                writer.Write(item.name);
                writer.Write(item.baseWeightUnits);
                writer.Write(item.hitPoints);
                writer.Write(item.capacityOrTarget);
                writer.Write(item.basePrice);
                writer.Write(item.enchantmentPoints);
                writer.Write(item.rarity);
                writer.Write(item.variants);
                writer.Write(item.drawOrderOrEffect);
                writer.Write(item.propertiesBitfield);
                writer.Write(item.worldTextureBitfield);
                writer.Write(item.playerTextureBitfield);
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
            item.hitPoints = reader.ReadUInt16();
            item.capacityOrTarget = reader.ReadInt32();
            item.basePrice = reader.ReadInt32();
            item.enchantmentPoints = reader.ReadInt16();
            item.rarity = reader.ReadByte();
            item.variants = reader.ReadByte();
            item.drawOrderOrEffect = reader.ReadByte();
            item.propertiesBitfield = reader.ReadByte();
            item.worldTextureBitfield = reader.ReadInt16();
            item.playerTextureBitfield = reader.ReadInt16();

            return item;
        }

        #endregion
    }
}
