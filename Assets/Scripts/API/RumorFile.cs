// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Allofich
// Contributors:
//
// Notes:
//

#region Using Statements
using System.IO;
using DaggerfallConnect.Utility;
using System.Collections.Generic;
#endregion

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Connects to a Daggerfall RUMOR.DAT file and extracts rumor records.
    /// </summary>
    public class RumorFile
    {
        #region Class Variables

        /// <summary>
        /// Abstracts rumor file to a managed disk or memory stream.
        /// </summary>
        private readonly FileProxy managedFile = new FileProxy();

        /// <summary>
        /// List of rumors read from file.
        /// </summary>
        public List<DaggerfallRumor> rumors;

        #endregion

        #region Class Structures

        /// <summary>
        /// Rumor types.
        /// </summary>
        public enum RumorTypes
        {
            Plague = 4,
            Famine = 7,
            WitchBurnings = 10,
            CrimeWave = 11,
            NewRuler = 12,
            PersecutedTemple = 18,
            AllianceSignMessage = 26,
            EnemySignMessage = 27,
            WarSignMessage = 28,
            FactionRumor = 100,
        }

        /// <summary>
        /// Represents a rumor.
        /// </summary>
        public struct DaggerfallRumor
        {
            public ushort Faction1;
            public ushort Faction2;
            public uint Type;
            public byte RegionID;
            public byte Flags;
            public byte QuestID;
            public string QuestName;
            public ushort Unknown;
            public uint NPCID;
            public uint TextLength;
            public uint TimeLimit;
            public byte[] RumorText;
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RumorFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to rumor file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public RumorFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        /// <summary>
        /// Load rumor file.
        /// </summary>
        /// <param name="filePath">Absolute path to rumor file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Load file into memory
            if (!managedFile.Load(filePath, usage, readOnly))
                return false;

            // Read file
            if (!Read())
                return false;

            return true;
        }

        #region Readers
        /// <summary>
        /// Read file.
        /// </summary>
        private bool Read()
        {
            // Step through file
            BinaryReader reader = managedFile.GetReader();
            ReadRumors(reader);

            return true;
        }

        /// <summary>
        /// Read rumors.
        /// </summary>
        /// <param name="reader">Reader to stream.</param>
        private void ReadRumors(BinaryReader reader)
        {
            rumors = new List<DaggerfallRumor>();

            while (reader.BaseStream.Position < managedFile.Length)
            {
                DaggerfallRumor rumor = new DaggerfallRumor();
                rumor.Faction1 = reader.ReadUInt16();
                rumor.Faction2 = reader.ReadUInt16();
                rumor.Type = reader.ReadUInt32();
                rumor.RegionID = reader.ReadByte();
                rumor.Flags = reader.ReadByte();
                rumor.QuestID = reader.ReadByte();
                rumor.QuestName = managedFile.ReadCString((int)reader.BaseStream.Position, 9);
                reader.BaseStream.Position += 9;
                rumor.Unknown = reader.ReadUInt16();
                rumor.NPCID = reader.ReadUInt32();
                rumor.TextLength = reader.ReadUInt32();
                rumor.TimeLimit = reader.ReadUInt32();
                rumor.RumorText = reader.ReadBytes((int)rumor.TextLength);

                rumors.Add(rumor);
            }
        }
        #endregion
    }
}
