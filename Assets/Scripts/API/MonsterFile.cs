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

        /// <summary>
        /// The BsaFile representing MONSTER.BSA.
        /// </summary>
        private readonly BsaFile bsaFile = new BsaFile();

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

        // NOTE: Monster CFG records are identical to CLASS.CFG records
        // and are loaded using ClassFile

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
        /// Gets monster class data.
        /// </summary>
        /// <param name="monster">Monster index.</param>
        /// <returns>DFClass.</returns>
        public DFCareer GetMonsterClass(int monster)
        {
            // Load the record
            DFCareer monsterClass;
            if (!LoadMonster(monster, out monsterClass))
                return null;

            return monsterClass;
        }

        /// <summary>
        /// Load monster record into memory and decompose it for use.
        /// </summary>
        /// <param name="monster">Monster index.</param>
        /// <returns>True if successful.</returns>
        public bool LoadMonster(int monster, out DFCareer monsterClassOut)
        {
            monsterClassOut = new DFCareer();

            // Generate name from index
            string name = string.Format("ENEMY{0:000}.CFG", monster);

            // Attempt to load record
            int index = bsaFile.GetRecordIndex(name);
            if (index == -1)
                return false;

            // Read monster class data
            ClassFile classFile = new ClassFile();
            byte[] data = bsaFile.GetRecordBytes(index);
            MemoryStream stream = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(stream);
            classFile.Load(reader);
            reader.Close();

            // Set output class
            monsterClassOut = classFile.Career;

            return true;
        }

        #endregion
    }
}
