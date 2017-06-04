// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;

namespace DaggerfallConnect.Save
{
    /// <summary>
    /// Represents a SAVEVARS.DAT file (must be a .175 or later save).
    /// </summary>
    public class SaveVars
    {
        #region Fields

        const string filename = "SAVEVARS.DAT";
        const int weaponDrawnOffset = 0x3BF;
        const int gameTimeOffset = 0x3C9;
        const int godModeOffset = 0x173B;
        const int lastSkillCheckTimeOffset = 0x179A;
        const int factionDataOffset = 0x17D0;
        const int factionDataLength = 92;

        bool weaponDrawn = false;
        uint gameTime = 0;
        bool godMode = false;
        uint lastSkillCheckTime = 0;

        // Private fields
        FileProxy saveVarsFile = new FileProxy();
        List<FactionFile.FactionData> factions = new List<FactionFile.FactionData>();

        #endregion

        #region Properties

        public static string Filename
        {
            get { return filename; }
        }

        /// <summary>
        /// Gets whether weapon is drawn from savevars.
        /// </summary>
        public bool WeaponDrawn
        {
            get { return weaponDrawn; }
        }

        /// <summary>
        /// Gets game time read from savevars.
        /// </summary>
        public uint GameTime
        {
            get { return gameTime; }
        }

        /// <summary>
        /// Gets whether GodMode is on from savevars.
        /// </summary>
        public bool GodMode
        {
            get { return godMode; }
        }

        /// <summary>
        /// Gets time of last check for raising skills, read from savevars.
        /// </summary>
        public uint LastSkillCheckTime
        {
            get { return lastSkillCheckTime; }
        }

        /// <summary>
        /// Gets array of factions read from savevars.
        /// </summary>
        public FactionFile.FactionData[] Factions
        {
            get { return factions.ToArray(); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SaveVars()
        {
        }

        /// <summary>
        /// Open constructor.
        /// </summary>
        /// <param name="saveVarsPath">Full path to SAVEVARS.DAT file.</param>
        /// <param name="readOnly">Flag to open file in read-only mode.</param>
        public SaveVars(string saveVarsPath, bool readOnly = true)
            : base()
        {
            Open(saveVarsPath, readOnly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens a SAVEVARS.DAT file.
        /// Always uses FileUsage.UseDisk so possible to write back to original file.
        /// </summary>
        /// <param name="saveVarsPath">Full path to SAVEVARS.DAT file.</param>
        /// <param name="readOnly">Flag to open file in read-only mode.</param>
        public bool Open(string saveVarsPath, bool readOnly = true)
        {
            // Open file proxy
            if (!saveVarsFile.Load(saveVarsPath, FileUsage.UseDisk, readOnly))
                return false;

            // Get reader
            BinaryReader reader = saveVarsFile.GetReader();

            // Read data
            ReadWeaponDrawn(reader);
            ReadGameTime(reader);
            ReadGodMode(reader);
            ReadLastSkillCheckTime(reader);
            ReadFactionData(reader);

            return true;
        }

        #endregion

        #region Private Methods

        void ReadWeaponDrawn(BinaryReader reader)
        {
            reader.BaseStream.Position = weaponDrawnOffset;
            if (reader.ReadByte() == 0x40)
                weaponDrawn = true;
        }

        void ReadGameTime(BinaryReader reader)
        {
            reader.BaseStream.Position = gameTimeOffset;
            gameTime = reader.ReadUInt32();
        }

        void ReadGodMode(BinaryReader reader)
        {
            reader.BaseStream.Position = godModeOffset;
            if (reader.ReadByte() == 0x40)
                godMode = true;
        }

        void ReadLastSkillCheckTime(BinaryReader reader)
        {
            reader.BaseStream.Position = lastSkillCheckTimeOffset;
            lastSkillCheckTime = reader.ReadUInt32();
        }

        void ReadFactionData(BinaryReader reader)
        {
            // Step through factions
            factions.Clear();
            int factionCount = (int)(reader.BaseStream.Length - factionDataOffset) / factionDataLength;
            for (int i = 0; i < factionCount; i++)
            {
                FactionFile.FactionData faction = new FactionFile.FactionData();
                reader.BaseStream.Position = factionDataOffset + (i * factionDataLength);

                faction.type = reader.ReadByte();
                faction.region = reader.ReadSByte();
                faction.ruler = reader.ReadSByte();
                faction.name = FileProxy.ReadCString(reader, 26);

                faction.rep = reader.ReadInt16();
                faction.power = reader.ReadInt16();
                faction.id = reader.ReadInt16();
                faction.vam = reader.ReadInt16();
                faction.flags = reader.ReadInt16();

                reader.BaseStream.Position += 8;            // Skip 8 unknown bytes

                faction.flat1 = reader.ReadInt16();
                faction.flat2 = reader.ReadInt16();

                faction.face = reader.ReadSByte();
                reader.BaseStream.Position += 1;            // Second face index is always -1

                faction.race = reader.ReadSByte();
                faction.sgroup = reader.ReadSByte();
                faction.ggroup = reader.ReadSByte();

                faction.ally1 = reader.ReadInt32();
                faction.ally2 = reader.ReadInt32();
                faction.ally3 = reader.ReadInt32();

                faction.enemy1 = reader.ReadInt32();
                faction.enemy2 = reader.ReadInt32();
                faction.enemy3 = reader.ReadInt32();

                reader.BaseStream.Position += 12;           // Skip 12 unknown bytes

                factions.Add(faction);
            }
        }

        #endregion
    }
}