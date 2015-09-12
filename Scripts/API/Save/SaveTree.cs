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
        public SaveTreeHeader Header;               // SaveTree header
        public List<SaveTreeRecord> Records;        // SaveTree records

        // Private fields
        FileProxy saveTreeFile = new FileProxy();

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

            writer.Close();
        }

        #region Static Methods

        /// <summary>
        /// Read Position data from binary stream.
        /// </summary>
        /// <param name="reader">BinaryReader positioned at start of Position data.</param>
        /// <returns>Position struct.</returns>
        public static Position ReadPosition(BinaryReader reader)
        {
            Position position = new Position();
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
        /// <param name="position">Position data to write.</param>
        public static void WritePosition(BinaryWriter writer, Position position)
        {
            writer.Write(position.WorldX);
            writer.Write(position.YOffset);
            writer.Write(position.YBase);
            writer.Write(position.WorldZ);
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
        public Position Position;
    }

    /// <summary>
    /// Position coordinates.
    /// </summary>
    public struct Position
    {
        public Int32 WorldX;                    // WorldX coordinate
        public UInt16 YOffset;                  // Altitude offset?
        public UInt16 YBase;                    // Altitude base?
        public Int32 WorldZ;                    // WorldZ coordinate
    }

    /// <summary>
    /// Types of SaveTree records encountered.
    /// </summary>
    public enum SaveTreeRecordTypes
    {
        CharacterPosition = 0x01,
        Item = 0x02,
        Character = 0x03,
        CharacterParentUnknown1 = 0x04,
        CharacterParentUnknown2 = 0x05,
        Unknown1 = 0x06,
        DungeonInformation = 0x07,                  // Length MUST be multiplied by 39 (0x27)
        Unknown2 = 0x08,
        SpellOrEffect = 0x09,
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