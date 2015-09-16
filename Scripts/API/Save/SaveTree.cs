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
    /// Represents a SAVETREE.DAT file.
    /// </summary>
    public class SaveTree
    {
        // Consts
        public const string Filename = "SAVETREE.DAT";

        // Public fields
        public SaveTreeHeader Header;
        public SaveTreeLocationDetail LocationDetail;
        public SaveTreeBaseRecord RootRecord = new SaveTreeBaseRecord();
        public Dictionary<uint, SaveTreeBaseRecord> RecordDictionary = new Dictionary<uint, SaveTreeBaseRecord>();

        // Private fields
        FileProxy saveTreeFile = new FileProxy();
        int duplicateKeysFound = 0;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SaveTree()
        {
        }

        /// <summary>
        /// Open constructor.
        /// </summary>
        /// <param name="saveTreePath">Full path to SAVETREE.DAT file.</param>
        /// <param name="readOnly">Flag to open file in read-only mode.</param>
        public SaveTree(string saveTreePath, bool readOnly = true)
            : base()
        {
            Open(saveTreePath, readOnly);
        }

        /// <summary>
        /// Opens a SAVETREE.DAT file.
        /// Always uses FileUsage.UseDisk so possible to write back to original file.
        /// </summary>
        /// <param name="saveTreePath">Full path to SAVETREE.DAT file.</param>
        /// <param name="readOnly">Flag to open file in read-only mode.</param>
        public bool Open(string saveTreePath, bool readOnly = true)
        {
            // Open file proxy
            if (!saveTreeFile.Load(saveTreePath, FileUsage.UseDisk, readOnly))
                return false;

            // Get reader
            BinaryReader reader = saveTreeFile.GetReader();

            // Read header
            Header = new SaveTreeHeader();
            Header.Read(reader);

            // Read location detail
            LocationDetail = new SaveTreeLocationDetail(reader);

            // Read remaining records
            ReadRecords(reader);

            return true;
        }

        /// <summary>
        /// Saves changes to SAVETREE.DAT file.
        /// Must have been opened with readOnly=false.
        /// </summary>
        public void Save()
        {
            // Must have a file open
            if (saveTreeFile.Length == 0)
                throw new Exception("SaveTree must be opened to invoke Save() method.");

            // File must be read-write
            if (saveTreeFile.ReadOnly)
                throw new Exception("SaveTree must be opened with readOnly=false to invoke Save() method.");

            // Get writer
            BinaryWriter writer = saveTreeFile.GetWriter();

            // Write header
            Header.Save(writer);

            // TODO: Write other records

            writer.Close();
        }

        #region Static Methods

        /// <summary>
        /// Read RecordPosition data from binary stream.
        /// </summary>
        /// <param name="reader">BinaryReader positioned at start of Position data.</param>
        /// <returns>RecordPosition struct.</returns>
        public static RecordPosition ReadPosition(BinaryReader reader)
        {
            RecordPosition position = new RecordPosition();
            position.WorldX = reader.ReadInt32();
            position.YOffset = reader.ReadUInt16();
            position.YBase = reader.ReadUInt16();
            position.WorldZ = reader.ReadInt32();

            return position;
        }

        /// <summary>
        /// Write Position data to a binary stream.
        /// </summary>
        /// <param name="writer">BinaryWriter positioned at start of Position data.</param>
        /// <param name="position">RecordPosition data to write.</param>
        public static void WritePosition(BinaryWriter writer, RecordPosition position)
        {
            writer.Write(position.WorldX);
            writer.Write(position.YOffset);
            writer.Write(position.YBase);
            writer.Write(position.WorldZ);
        }

        /// <summary>
        /// Peeks record type from beginning of RecordRoot without moving stream position.
        /// </summary>
        /// <param name="reader">Reader positioned at start of RecordRoot data.</param>
        /// <returns>Record type.</returns>
        public static RecordTypes PeekRecordType(BinaryReader reader)
        {
            long position = reader.BaseStream.Position;
            byte type = reader.ReadByte();
            reader.BaseStream.Position = position;

            return (RecordTypes)type;
        }

        #endregion

        #region Private Methods

        // Reads all records in SaveTree
        // Reader must be positioned at start of first RecordElement
        void ReadRecords(BinaryReader reader)
        {
            RecordDictionary.Clear();
            RootRecord = new SaveTreeBaseRecord();
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                // Read record length and skip empty records as they have no data
                int length = reader.ReadInt32();
                if (length <= 0)
                    continue;

                // Handle potential stream overflow (e.g. corrupt save, something went wrong)
                if (reader.BaseStream.Position + length >= reader.BaseStream.Length)
                    break;

                // Peek record type from RecordRoot so we can instantiate record class based on type
                RecordTypes type = PeekRecordType(reader);

                // Add record based on type
                SaveTreeBaseRecord record;
                record = new SaveTreeBaseRecord(reader, length);
                //switch (type)
                //{
                //    //case RecordTypes.Item:
                //    //    record = new ItemRecord(reader);
                //    //    break;
                //    //case RecordTypes.Spell:
                //    //    record = new SpellRecord(reader);
                //    //    break;
                //    //case RecordTypes.UnknownTownLink:
                //    //    record = new SaveTreeBaseRecord(reader, length);    // Read then skip these records for now
                //    //    continue;
                //    //case RecordTypes.DungeonData:
                //    //    record = new SaveTreeBaseRecord(reader, length);    // Read then skip these records for now
                //    //    continue;
                //    default:
                //        record = new SaveTreeBaseRecord(reader, length);
                //        break;
                //}
                AddRecord(record);
            }

            LinkChildren();
        }

        // Adds record to dictionary
        bool AddRecord(SaveTreeBaseRecord record)
        {
            // Count duplicate IDs and ignore record
            uint myKey = record.RecordRoot.RecordID;
            if (RecordDictionary.ContainsKey(myKey))
            {
                duplicateKeysFound++;
                return false;
            }

            // Add record to dictionary
            RecordDictionary.Add(myKey, record);

            return true;
        }

        // Populates children of parent records
        void LinkChildren()
        {
            foreach (var kvp in RecordDictionary)
            {
                uint parentKey = kvp.Value.RecordRoot.ParentRecordID;
                if (RecordDictionary.ContainsKey(parentKey))
                {
                    SaveTreeBaseRecord parent = RecordDictionary[parentKey];
                    parent.Children.Add(kvp.Value);
                }
                else
                {
                    RootRecord.Children.Add(kvp.Value);
                }
            }
        }

        #endregion
    }

    #region Structs & Enums

    /// <summary>
    /// Character position record in header.
    /// </summary>
    public struct CharacterPositionRecord
    {
        public Byte RecordType;                 // Must always be 0x01
        public UInt16 Unknown;
        public RecordPosition Position;
    }

    /// <summary>
    /// Position coordinates.
    /// </summary>
    public struct RecordPosition
    {
        public Int32 WorldX;                    // WorldX coordinate
        public UInt16 YOffset;                  // Altitude offset?
        public UInt16 YBase;                    // Altitude base?
        public Int32 WorldZ;                    // WorldZ coordinate
    }

    public struct RecordRoot
    {
        public RecordPosition Position;         // Position of the object in world (if applicable)
        public UInt32 RecordID;                 // Unique ID of this record
        public Byte QuestID;                    // Associated quest ID of this record (0 if no quest)
        public UInt32 ParentRecordID;           // ID of parent record
        public RecordTypes ParentRecordType;    // Type of parent record
    }

    /// <summary>
    /// Types of SaveTree records encountered.
    /// </summary>
    public enum RecordTypes
    {
        Null = 0x00,
        CharacterPosition = 0x01,
        Item = 0x02,
        Character = 0x03,
        CharacterParentUnknown1 = 0x04,
        CharacterParentUnknown2 = 0x05,
        Unknown1 = 0x06,
        DungeonInformation = 0x07,                  // Length MUST be multiplied by 39 (0x27)
        Unknown2 = 0x08,
        Spell = 0x09,
        GuildMembership = 0x0a,
        QBNData = 0x0e,
        QBNDataParent1 = 0x10,
        QBNDataParent2 = 0x12,
        SpellcastingCreatureListHead = 0x16,
        ControlSetting = 0x17,
        LocationName1 = 0x18,                       // Possibly logbook entries
        BankAccount = 0x19,
        PotionMix = 0x1f,
        UnknownTownLink = 0x20,
        UnknownDungeonRecord = 0x21,                // Usually in dungeon saves
        Creature1 = 0x22,
        UnknownItemRecord = 0x24,                   // Possibly items on store shelves
        MysteryRecord1 = 0x27,                      // Referenced but does not exist in file
        LocationName2 = 0x28,                       // Possibly for quests
        LocationName3 = 0x29,                       // Possible for quests
        MysteryRecord2 = 0x2b,                      // Referenced but does not exist in file
        Creature2 = 0x2c,
        NPC = 0x2d,
        GenericNPC = 0x2e,
        DungeonData = 0x33,                         // Huge but mostly zero filled
        Container = 0x34,
        ItemLeftForRepair = 0x36,
        Unknown3 = 0x40,                            // All are part of a 'container'
        Unknown4 = 0x41,                            // Possibly quest information
    }

    #endregion
}