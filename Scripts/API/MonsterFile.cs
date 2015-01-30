// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

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
    /// </summary>
    public class MonsterFile
    {
        #region Fields

        // <summary>
        /// The BsaFile representing MONSTER.BSA.
        /// </summary>
        private BsaFile bsaFile = new BsaFile();

        ///// <summary>
        ///// Effect multipliers.
        ///// </summary>
        //float[] multipliers = new float[] { 3.0f, 2.0f, 1.75f, 1.5f, 1.0f };

        #endregion

        #region Structures & Enums

        //public struct Monster
        //{
        //    public byte ResistanceFlags;
        //    public byte ImmunityFlags;
        //    public byte LowToleranceFlags;
        //    public byte CriticalWeaknessFlags;
        //    public bool AcuteHearing;
        //    public bool Athleticism;
        //    public bool AdrenalineRush;
        //    public bool NoRegenSpellPoints;
        //    public bool SunDamage;
        //    public bool HolyDamage;
        //}

        //public enum EffectFlags
        //{
        //    Paralysis = 1,
        //    Magic = 2,
        //    Poison = 4,
        //    Fire = 8,
        //    Frost = 16,
        //    Shock = 64,
        //    Disease = 128,
        //}

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
            filePath = filePath.ToUpper();
            if (!filePath.EndsWith(Filename))
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

            // Attempt to load file
            int index = bsaFile.GetRecordIndex(name);
            if (index == -1)
                return false;

            // Setup reader, go right to name as that is all we are reading for now
            FileProxy memoryFile = bsaFile.GetRecordProxy(index);
            dfMonster.Name = memoryFile.ReadCString(28, 16);

            return true;
        }

        #endregion
    }
}
