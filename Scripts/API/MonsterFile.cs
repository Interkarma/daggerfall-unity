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

#region Using Statements
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect.Utility;
#endregion

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Connects to MONSTER.BSA to enumerate and extract monster data.
    /// NOTE: This is a work in progress and is not complete.
    /// 
    /// Thanks to tamentis for starting point:
    /// https://github.com/tamentis/openscrolls/blob/master/list_monsters.py
    /// </summary>
    public class MonsterFile
    {
        #region Fields

        /// <summary>
        /// Spell point multipliers.
        /// </summary>
        public static float[] SpellPointMultipliers = new float[] { 3.0f, 2.0f, 1.75f, 1.5f, 1.0f };

        /// <summary>
        /// The BsaFile representing MONSTER.BSA.
        /// </summary>
        private BsaFile bsaFile = new BsaFile();

        #endregion

        #region Structures & Enums

        /// <summary>
        /// Defines a monster ANC record.
        /// Name is in format ASCR0000.ASC, where 0000 is monster ID.
        /// Likely to be animation settings for monsters.
        /// Format currently unknown and not researched.
        /// </summary>
        public struct MonsterANC
        {
        }

        /// <summary>
        /// Defines a monster CFG record.
        /// Name is in format ENEMY000.CFG, where 000 is monster ID.
        /// These records are always 74 bytes.
        /// </summary>
        public struct MonsterCFG
        {
            // bytes [0-3]
            // Flags controlling how monster tolerates various magic effects.
            public Byte ResistanceFlags;
            public Byte ImmunityFlags;
            public Byte LowToleranceFlags;
            public Byte CriticalWeaknessFlags;

            // byte [4-5]
            // Bitfield controlling special ability flags and spellpoints
            // value & 1 = AcuteHearing
            // value & 2 = Athleticism
            // value & 4 = AdrenalineRush
            // value & 8 = NoRegenSpellPoints
            // value & 16 = SunDamage
            // value & 32 = HolyDamage
            // (value & 0x00C0) >> 8 = SpellPoints in dark
            // (value & 0x0300) >> 10 = SpellPoints in light
            // (value 0x1C00) >> 12 = SpellPointMultiplierIndex
            public Int16 AbilityFlagsAndSpellPointsBitfield;

            // bytes [6-27]
            // Unknown values. Research needed.
            public Byte[] UnknownRange1;

            // bytes [28-43]
            // Name of monster.
            public String Name;

            // bytes [44-73]
            public Byte[] UnknownRange2;
        }

        /// <summary>
        /// Flags for special abilities.
        /// </summary>
        [Flags]
        public enum SpecialAbilityFlags
        {
            AcuteHearing = 1,
            Athleticism = 2,
            AdrenalineRush = 4,
            NoRegenSpellPoints = 8,
            SunDamage = 16,
            HolyDamage = 32,
        }

        /// <summary>
        /// Flags for magic effects.
        /// </summary>
        [Flags]
        public enum MagicEffectFlags
        {
            Paralysis = 1,
            Magic = 2,
            Poison = 4,
            Fire = 8,
            Frost = 16,
            Shock = 64,
            Disease = 128,
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MonsterFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to MONSTER.BSA.</param>
        /// <param name="usage">Determines if the BSA file will read from disk or memory.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public MonsterFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Number of BSA records in MONSTER.BSA.
        /// </summary>
        public int Count
        {
            get { return bsaFile.Count; }
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets default MONSTER.BSA filename.
        /// </summary>
        static public string Filename
        {
            get { return "MONSTER.BSA"; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load MONSTER.BSA file.
        /// </summary>
        /// <param name="filePath">Absolute path to MONSTER.BSA file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Validate filename
            if (!filePath.EndsWith(Filename, StringComparison.InvariantCultureIgnoreCase))
                return false;

            // Load file
            if (!bsaFile.Load(filePath, usage, readOnly))
                return false;

            return true;
        }

        /// <summary>
        /// Gets name of specified record.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>Name of the record.</returns>
        public string GetRecordName(int record)
        {
            return bsaFile.GetRecordName(record);
        }

        /// <summary>
        /// Gets data from specified record.
        /// </summary>
        /// <param name="record">Index of record.</param>
        /// <returns>Byte array of record data.</returns>
        public byte[] GetRecordBytes(int record)
        {
            return bsaFile.GetRecordBytes(record);
        }

        /// <summary>
        /// Gets monster data.
        /// </summary>
        /// <param name="monster">Monster index.</param>
        /// <returns>DFMonster.</returns>
        public DFMonster GetMonster(int monster)
        {
            // Load the record
            DFMonster dfMonster = new DFMonster();
            if (!LoadMonster(monster, out dfMonster))
                return dfMonster;

            return dfMonster;
        }

        /// <summary>
        /// Load monster record into memory and decompose it for use.
        /// </summary>
        /// <param name="monster">Monster index.</param>
        /// <returns>True if successful.</returns>
        public bool LoadMonster(int monster, out DFMonster dfMonster)
        {
            dfMonster = new DFMonster();

            // Generate name from index
            string name = string.Format("ENEMY{0:000}.CFG", monster);

            // Attempt to load record
            int index = bsaFile.GetRecordIndex(name);
            if (index == -1)
                return false;

            // Read monster CFG data
            MonsterCFG cfg = ReadMonsterCFG(index);
            dfMonster.MonsterCFG = cfg;

            // Copy name, resists, etc.
            dfMonster.Name = cfg.Name;
            dfMonster.ResistanceFlags = cfg.ResistanceFlags;
            dfMonster.ImmunityFlags = cfg.ImmunityFlags;
            dfMonster.LowToleranceFlags = cfg.LowToleranceFlags;
            dfMonster.CriticalWeaknessFlags = cfg.CriticalWeaknessFlags;

            // Expand special abilities, spell points, etc.
            int value = cfg.AbilityFlagsAndSpellPointsBitfield;
            dfMonster.AcuteHearing = ((value & 1) == 1);
            dfMonster.Athleticism = ((value & 2) == 2);
            dfMonster.AdrenalineRush = ((value & 4) == 4);
            dfMonster.NoRegenSpellPoints = ((value & 8) == 8);
            dfMonster.SunDamage = ((value & 16) == 16);
            dfMonster.HolyDamage = ((value & 32) == 32);
            dfMonster.SpellPointsInDark = ((value & 0x00c0) >> 8);
            dfMonster.SpellPointsInLight = ((value & 0x0300) >> 10);
            dfMonster.SpellPointMultiplier = ((value & 0x1c00) >> 12);

            return true;
        }

        #endregion

        #region Readers

        /// <summary>
        /// Read monster data.
        /// </summary>
        private MonsterCFG ReadMonsterCFG(int index)
        {
            // Get reader for this monster record
            FileProxy proxy = bsaFile.GetRecordProxy(index);
            BinaryReader reader = proxy.GetReader();

            // Read monster resist, etc. flags
            MonsterCFG cfg = new MonsterCFG();
            cfg.ResistanceFlags = reader.ReadByte();
            cfg.ImmunityFlags = reader.ReadByte();
            cfg.LowToleranceFlags = reader.ReadByte();
            cfg.CriticalWeaknessFlags = reader.ReadByte();

            // Read monster special ability and spell point bitfield
            cfg.AbilityFlagsAndSpellPointsBitfield = reader.ReadInt16();

            // Read 22 unknown bytes for UnknownRange1
            cfg.UnknownRange1 = reader.ReadBytes(22);

            // Read name
            cfg.Name = proxy.ReadCStringSkip(reader, 0, 16);

            // Read 30 unknown bytes for UnknownRange2
            cfg.UnknownRange2 = reader.ReadBytes(30);

            return cfg;
        }

        #endregion
    }
}
