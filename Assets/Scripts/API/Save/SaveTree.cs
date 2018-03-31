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
    /// Represents a SAVETREE.DAT file.
    /// </summary>
    public class SaveTree
    {
        // Consts
        public const string Filename = "SAVETREE.DAT";

        // Public fields
        public SaveTreeHeader Header;
        public SaveTreeBuildingRecords BuildingRecords;
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

            // Read building records
            BuildingRecords = new SaveTreeBuildingRecords(reader);

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

        /// <summary>
        /// Finds first record of type in tree starting from root record.
        /// </summary>
        /// <param name="type">Type of record to search for.</param>
        /// <param name="root">Root record to start searching from. If null, will start from RecordRoot.</param>
        /// <returns>Found item or null if not found.</returns>
        public SaveTreeBaseRecord FindRecord(RecordTypes type, SaveTreeBaseRecord root = null)
        {
            List<SaveTreeBaseRecord> records = FindRecords(type, root);
            if (records.Count == 0)
                return null;

            return records[0];
        }

        /// <summary>
        /// Finds all instances of a specific record type in tree starting from root record.
        /// </summary>
        /// <param name="type">Type of record to search for.</param>
        /// <param name="root">Root record to start searching from. If null, will start from RecordRoot.</param>
        /// <returns>List of records found. May contain 0 records.</returns>
        public List<SaveTreeBaseRecord> FindRecords(RecordTypes type, SaveTreeBaseRecord root = null)
        {
            List<SaveTreeBaseRecord> recordList = new List<SaveTreeBaseRecord>();

            if (root == null)
                root = RootRecord;

            FindRecordsByType(type, root, recordList);

            return recordList;
        }

        /// <summary>
        /// Filters a record list by parent type.
        /// </summary>
        /// <param name="source">Source list.</param>
        /// <param name="parentType">Parent type to filter for.</param>
        /// <returns>New list of items with specific parent type.</returns>
        public List<SaveTreeBaseRecord> FilterRecordsByParentType(List<SaveTreeBaseRecord> source, RecordTypes parentType)
        {
            List<SaveTreeBaseRecord> newList = new List<SaveTreeBaseRecord>();

            foreach(SaveTreeBaseRecord record in source)
            {
                if (record.Parent == null)
                    continue;

                if (record.Parent.RecordType == parentType)
                    newList.Add(record);
            }

            return newList;
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
            position.WorldY = reader.ReadInt32();
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
            writer.Write(position.WorldY);
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
                switch (type)
                {
                    case RecordTypes.Item:
                        record = new ItemRecord(reader, length);
                        break;
                    case RecordTypes.Character:
                        record = new CharacterRecord(reader, length);
                        break;
                    case RecordTypes.Spell:
                        record = new SpellRecord(reader, length);
                        break;
                    case RecordTypes.DiseaseOrPoison:
                        record = new DiseaseOrPoisonRecord(reader, length);
                        break;
                    case RecordTypes.TrappedSoul:
                        record = new TrappedSoulRecord(reader, length);
                        break;
                    case RecordTypes.Container:
                        record = new ContainerRecord(reader, length);
                        break;
                    //case RecordTypes.Door:
                    //    record = new SaveTreeBaseRecord(reader, length);    // Read then skip these records for now
                    //    continue;
                    //case RecordTypes.DungeonData:
                    //    record = new SaveTreeBaseRecord(reader, length);    // Read then skip these records for now
                    //    continue;
                    default:
                        record = new SaveTreeBaseRecord(reader, length);
                        break;
                }
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
                    kvp.Value.Parent = parent;
                    parent.Children.Add(kvp.Value);
                }
                else
                {
                    RootRecord.Children.Add(kvp.Value);
                }
            }
        }

        // Recursively search for record type
        void FindRecordsByType(RecordTypes type, SaveTreeBaseRecord parent, List<SaveTreeBaseRecord> recordList)
        {
            if (parent.RecordType == type)
                recordList.Add(parent);

            for (int i = 0; i < parent.Children.Count; i++)
            {
                FindRecordsByType(type, parent.Children[i], recordList);
            }
        }

        #endregion
    }

    #region Structs & Enums

    /// <summary>
    /// Character position record in header.
    /// </summary>
    public struct HeaderCharacterPositionRecord
    {
        public RecordPosition Position;
    }

    /// <summary>
    /// Position coordinates.
    /// </summary>
    public struct RecordPosition
    {
        public Int32 WorldX;                    // WorldX coordinate
        public Int32 WorldY;                    // WorldY coordinate
        public Int32 WorldZ;                    // WorldZ coordinate
    }

    public struct RecordRoot
    {
        // Some parts are identified based on save-game viewer "chunktcl"

        public Int16 Pitch;                     // Pitch of object
        public Int16 Yaw;                       // Yaw of object
        public Int16 Roll;                      // Roll of object
        public RecordPosition Position;         // Position of the object in world (if applicable)
        public UInt16 SpriteIndex;              // chunktcl's description: 3d view picture. Called "sprite index" by Fixsave.
        public UInt16 Picture2;                 // chunktcl's description: Inventory picture
        public UInt32 RecordID;                 // Unique ID of this record. Called "mapID" and "map identifier" by Fixsave.
        public Byte QuestID;                    // Associated quest ID of this record (0 if no quest)
        public UInt32 ParentRecordID;           // ID of parent record
        public UInt32 ItemObject;               // chunktcl's description: ItemObject. Active spell/spell book/permanent treasure container
        public UInt32 QuestObjectID;            // chunktcl's description: QuestObjectID
        public UInt32 NextObject;               // chunktcl's description: Link to next object in series
        public UInt32 ChildObject;              // chunktcl's description: Link to first child object in series
        public UInt32 SublistHead;              // chunktcl's description: Sublist head. Link to parent object of series
        public RecordTypes ParentRecordType;    // Type of parent record
    }

    /// <summary>
    /// Player environment types.
    /// </summary>
    public enum Environments
    {
        Outside = 1,
        Building = 2,
        Dungeon = 3,
    }

    /// <summary>
    /// Types of SaveTree records encountered.
    /// Save viewer program "chunktcl" used to identify some.
    /// </summary>
    public enum RecordTypes
    {
        Null = 0x00,
        Item = 0x02,
        Character = 0x03,
        CharacterPositionRecord = 0x04,             // This record, not the position record in the header, determines where player is when game loads.
        CharacterCamera = 0x05,                     // Guessing that this is the player camera. Has the same position as CharacterPositionRecord, and changing the roll value with a hex editor will cause the screen to be rotated on game load.
        Interactable3dObject = 0x06,                // Levers, switches, moving platforms, etc.
        DungeonInformation = 0x07,                  // Length MUST be multiplied by 39 (0x27)
        NPCFlat = 0x08,
        Spell = 0x09,
        GuildMembership = 0x0a,
        DiseaseOrPoison = 0x0b,
        QBNData = 0x0e,
        QuestTree = 0x10,                           // Fixsave calls this "quest tree".
        EnemyMobile = 0x12,
        TrappedSoul = 0x14,                                // Souls held in soul traps.
        SpellcastingCreatureListHead = 0x16,
        UserOptions = 0x17,                         // Fixsave calls this "user options"
        LocationName1 = 0x18,                       // Possibly logbook entries. chunktcl calls this "Logbook"
        BankAccount = 0x19,
        PotionMix = 0x1f,
        Door = 0x20,
        Treasure = 0x21,
        Marker = 0x22,                              // Spawn markers, quest markers, teleport markers, etc.
        UnknownItemRecord = 0x24,                   // Possibly items on store shelves
        MysteryRecord1 = 0x27,                      // Referenced but does not exist in file
        LocationName2 = 0x28,                       // Possibly for quests. chunktcl calls this "Quest site"
        LocationName3 = 0x29,                       // Possible for quests. chunktcl calls this "Quest NPC"
        MysteryRecord2 = 0x2b,                      // Referenced but does not exist in file
        Corpse = 0x2c,                              // Dead mobile
        NPC = 0x2d,
        GenericNPC = 0x2e,
        DungeonData = 0x33,                         // Huge but mostly zero filled
        Container = 0x34,                           // Fixsave calls this "item holder"
        NPCMobile = 0x35,                           // Mobile NPCs wandering around outside
        ItemLeftForRepair = 0x36,
        Unknown2 = 0x40,                            // All are part of a 'container'. chunktcl calls this "Visited?"
        QuestNPC = 0x41,                            // Fixsave calls this "quest NPC". chunktcl calls this "Qbn questor"
    }

    #endregion
}