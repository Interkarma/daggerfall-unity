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

#region Using Statements
using System;
using System.IO;
using DaggerfallConnect.Utility;
#endregion

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Reads a CLASS*.CFG file or a stream to another CFG file (e.g. ENEMY*.CFG in a MONSTER.BSA record).
    /// </summary>
    public class ClassFile
    {
        #region Fields

        readonly FileProxy file = new FileProxy();
        DFCareer career = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets structured career data.
        /// Will be null unless file loaded.
        /// </summary>
        public DFCareer Career
        {
            get { return career; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ClassFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        public ClassFile(string filePath, FileUsage usage = FileUsage.UseMemory, bool readOnly = true)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load a CFG file from disk.
        /// </summary>
        /// <param name="filePath">Absolute path to CLASS*.CFG file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath, FileUsage usage = FileUsage.UseMemory, bool readOnly = true)
        {
            // Validate filename
            string filename = Path.GetFileName(filePath);
            if (!filename.StartsWith("CLASS", StringComparison.InvariantCultureIgnoreCase) ||
                !filename.EndsWith(".CFG", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            // Load file
            if (!file.Load(filePath, usage, readOnly))
                return false;

            // Read file data
            BinaryReader reader = file.GetReader();
            ReadFile(reader);

            return true;
        }

        /// <summary>
        /// Load a CFG file from stream.
        /// </summary>
        /// <param name="reader">Stream positioned at 74-byte CFG data record.</param>
        /// <returns>True if successful.</returns>
        public bool Load(BinaryReader reader)
        {
            ReadFile(reader);

            return true;
        }

        #endregion

        #region File Readers

        void ReadFile(BinaryReader reader)
        {
            // Read class resist, etc. flags
            DFCareer.CFGData cfg = new DFCareer.CFGData();
            cfg.ResistanceFlags = reader.ReadByte();
            cfg.ImmunityFlags = reader.ReadByte();
            cfg.LowToleranceFlags = reader.ReadByte();
            cfg.CriticalWeaknessFlags = reader.ReadByte();

            // Read class special ability and spell point bitfield
            cfg.AbilityFlagsAndSpellPointsBitfield = reader.ReadUInt16();

            // Read rapid healing flags
            cfg.RapidHealing = reader.ReadByte();

            // Read regeneration flags
            cfg.Regeneration = reader.ReadByte();

            // Unknown value
            cfg.Unknown1 = reader.ReadByte();

            // Spell absorption flags
            cfg.SpellAbsorptionFlags = reader.ReadByte();

            // Attack modifier against major enemy groups
            cfg.AttackModifierFlags = reader.ReadByte();

            // Read forbidden material flags
            cfg.ForbiddenMaterialsFlags = reader.ReadUInt16();

            // Read weapon, armor, shields bitfield
            Byte a = reader.ReadByte();
            Byte b = reader.ReadByte();
            Byte c = reader.ReadByte();
            cfg.WeaponArmorShieldsBitfield = (UInt32)((a << 16) | (c << 8) | b);

            // Read primary skills
            cfg.PrimarySkill1 = reader.ReadByte();
            cfg.PrimarySkill2 = reader.ReadByte();
            cfg.PrimarySkill3 = reader.ReadByte();

            // Read major skills
            cfg.MajorSkill1 = reader.ReadByte();
            cfg.MajorSkill2 = reader.ReadByte();
            cfg.MajorSkill3 = reader.ReadByte();

            // Read minor skills
            cfg.MinorSkill1 = reader.ReadByte();
            cfg.MinorSkill2 = reader.ReadByte();
            cfg.MinorSkill3 = reader.ReadByte();
            cfg.MinorSkill4 = reader.ReadByte();
            cfg.MinorSkill5 = reader.ReadByte();
            cfg.MinorSkill6 = reader.ReadByte();

            // Read class name
            cfg.Name = FileProxy.ReadCStringSkip(reader, 0, 16);

            // Read 8 unknown bytes
            cfg.Unknown2 = reader.ReadBytes(8);

            // Hit points per level
            cfg.HitPointsPerLevel = reader.ReadUInt16();

            // Read advancement multiplier
            cfg.AdvancementMultiplier = reader.ReadUInt32();

            // Read attributes
            cfg.Attributes = new UInt16[8];
            for (int i = 0; i < 8; i++)
            {
                cfg.Attributes[i] = reader.ReadUInt16();
            }

            // Structure data
            career = new DFCareer(cfg);
        }

        #endregion
    }
}
