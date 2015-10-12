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
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallConnect.Save
{
    /// <summary>
    /// Character record.
    /// SaveTreeRecordTypes = 0x03
    /// </summary>
    public class CharacterRecord : SaveTreeBaseRecord
    {
        CharacterRecordData parsedData = new CharacterRecordData();

        /// <summary>
        /// Gets parsed character data as read from SAVETREE.DAT.
        /// </summary>
        public CharacterRecordData ParsedData
        {
            get { return parsedData; }
        }

        public CharacterRecord()
        {
        }

        public CharacterRecord(BinaryReader reader, int length)
            : base(reader, length)
        {
            ReadCharacterData();
        }

        #region Readers

        /// <summary>
        /// Reads known character data.
        /// </summary>
        void ReadCharacterData()
        {
            byte[] recordData = RecordData;
            MemoryStream stream = new MemoryStream(recordData);
            BinaryReader reader = new BinaryReader(stream);

            parsedData = new CharacterRecordData();
            parsedData.characterName = ReadCharacterName(reader);
            parsedData.currentStats = ReadStats(reader);
            parsedData.baseStats = ReadStats(reader);
            parsedData.gender = ReadGender(reader);
            parsedData.transportationFlags = ReadTransportationFlags(reader);
            parsedData.race = ReadRace(reader);
            parsedData.armorValues = reader.ReadBytes(7);

            reader.BaseStream.Position = 0x58;
            parsedData.startSumUnknown = reader.ReadInt32();
            parsedData.opponentUnknown = reader.ReadInt32();

            reader.BaseStream.Position = 0x7c;
            parsedData.currentHealth = reader.ReadInt16();
            parsedData.startingHealth = reader.ReadInt16();
            parsedData.faceIndex = reader.ReadByte();
            parsedData.level = reader.ReadByte();

            reader.BaseStream.Position = 0x83;
            parsedData.reflexes = ReadReflexes(reader);

            reader.BaseStream.Position = 0x85;
            parsedData.physicalGold = reader.ReadUInt32();

            reader.BaseStream.Position = 0x8d;
            parsedData.maxMagicka = reader.ReadInt16();

            // TODO: 0x91 reputation?

            reader.BaseStream.Position = 0x9d;
            parsedData.skills = ReadSkills(reader);

            // TODO: 0x16f equipped items?

            reader.BaseStream.Position = 0x01fd;
            parsedData.timeStamp = reader.ReadUInt32();

            reader.BaseStream.Position = 0x230;
            parsedData.career = ReadCareer(reader);

            reader.Close();
        }

        string ReadCharacterName(BinaryReader reader)
        {
            return FileProxy.ReadCString(reader, 32);
        }

        DaggerfallStats ReadStats(BinaryReader reader)
        {
            DaggerfallStats result = new DaggerfallStats();
            for (int i = 0; i < 8; i++)
            {
                result.SetStatValue(i, reader.ReadInt16());
            }

            return result;
        }

        Genders ReadGender(BinaryReader reader)
        {
            return (Genders)reader.ReadByte();
        }

        UInt16 ReadTransportationFlags(BinaryReader reader)
        {
            return reader.ReadUInt16();
        }

        Races ReadRace(BinaryReader reader)
        {
            return (Races)reader.ReadByte();
        }

        PlayerReflexes ReadReflexes(BinaryReader reader)
        {
            byte value = reader.ReadByte();
            return (PlayerReflexes)value;
        }

        Int16[] ReadReputationUnknown(BinaryReader reader)
        {
            Int16[] values = new Int16[5];
            for (int i = 0; i < 5; i++)
            {
                values[i] = reader.ReadInt16();
            }

            return values;
        }

        DaggerfallSkills ReadSkills(BinaryReader reader)
        {
            DaggerfallSkills skills = new DaggerfallSkills();

            for (int i = 0; i < DaggerfallSkills.Count; i++)
            {
                Int16 shortValue = reader.ReadInt16();
                reader.ReadInt32(); // Skip unknown Int32
                skills.SetSkillValue(i, shortValue);
            }

            return skills;
        }

        DFCareer ReadCareer(BinaryReader reader)
        {
            ClassFile classFile = new ClassFile();
            classFile.Load(reader);

            return classFile.Career;
        }

        #endregion

        #region Structs & Enums

        /// <summary>
        /// Defines character record data as read from SAVETREE.DAT.
        /// </summary>
        public struct CharacterRecordData
        {
            public string characterName;
            public DaggerfallStats currentStats;
            public DaggerfallStats baseStats;
            public Genders gender;
            public UInt16 transportationFlags;
            public Races race;
            public Byte[] armorValues;
            public Int32 startSumUnknown;
            public Int32 opponentUnknown;
            public Int16 currentHealth;
            public Int16 startingHealth;
            public Byte faceIndex;
            public Byte level;
            public PlayerReflexes reflexes;
            public UInt32 physicalGold;
            public Int16 maxMagicka;
            public DaggerfallSkills skills;
            public UInt32 timeStamp;
            public DFCareer career;
        }

        #endregion
    }
}
