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

        /// <summary>
        /// Converts a CharacterRecord to a prototypical CharacterDocument for character import.
        /// </summary>
        /// <returns>CharacterDocument derived from CharacterRecord data.</returns>
        public CharacterDocument ToCharacterDocument()
        {
            CharacterDocument doc = new CharacterDocument();
            Dictionary<int, RaceTemplate> raceDict = RaceTemplate.GetRaceDictionary();

            doc.raceTemplate = raceDict[(int)parsedData.race + 1];
            doc.gender = parsedData.gender;
            doc.career = parsedData.career;
            doc.name = parsedData.characterName;
            doc.faceIndex = parsedData.faceIndex;
            doc.workingStats = parsedData.currentStats;
            doc.workingSkills = parsedData.skills;
            doc.reflexes = parsedData.reflexes;
            doc.currentHealth = parsedData.currentHealth;
            doc.maxHealth = parsedData.maxHealth;
            doc.currentSpellPoints = parsedData.currentSpellPoints;
            doc.currentFatigue = parsedData.currentFatigue;
            doc.skillUses = parsedData.skillUses;
            doc.startingLevelUpSkillSum = parsedData.startingLevelUpSkillSum;

            return doc;
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
            parsedData.startingLevelUpSkillSum = reader.ReadInt32();
            parsedData.opponentUnknown = reader.ReadInt32();

            reader.BaseStream.Position = 0x5c;
            parsedData.baseHealth = reader.ReadInt16();

            reader.BaseStream.Position = 0x7c;
            parsedData.currentHealth = reader.ReadInt16();
            parsedData.maxHealth = reader.ReadInt16();
            parsedData.faceIndex = reader.ReadByte();
            parsedData.level = reader.ReadByte();

            reader.BaseStream.Position = 0x83;
            parsedData.reflexes = ReadReflexes(reader);

            reader.BaseStream.Position = 0x85;
            parsedData.physicalGold = reader.ReadUInt32();

            reader.BaseStream.Position = 0x8d;
            parsedData.currentSpellPoints = reader.ReadInt16();
            parsedData.maxSpellPoints = reader.ReadInt16();

            parsedData.reputationCommoners = reader.ReadInt16();
            parsedData.reputationMerchants = reader.ReadInt16();
            parsedData.reputationScholars = reader.ReadInt16();
            parsedData.reputationNobility = reader.ReadInt16();
            parsedData.reputationUnderworld = reader.ReadInt16();

            parsedData.currentFatigue = reader.ReadUInt16();

            parsedData.skills = ReadSkills(reader, out parsedData.skillUses);

            reader.BaseStream.Position = 0x16f;
            parsedData.equippedItems = ReadEquippedItems(reader);

            reader.BaseStream.Position = 0x1f2;
            parsedData.race2 = ReadRace(reader);

            reader.BaseStream.Position = 0x1fd;
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
            byte value = reader.ReadByte();

            // Daggerfall uses a wide range of gender values
            // It is currently unknown what these values represent
            // However it seems that first bit always maps to
            // 0 for male and 1 for female
            if ((value & 1) == 1)
                return Genders.Female;
            else
                return Genders.Male;
        }

        UInt16 ReadTransportationFlags(BinaryReader reader)
        {
            // Known values:
            // 1 = Foot
            // 3 = Horse
            // 5 = Cart
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

        DaggerfallSkills ReadSkills(BinaryReader reader, out Int16[] skillUses)
        {
            DaggerfallSkills skills = new DaggerfallSkills();
            skillUses = new Int16[DaggerfallSkills.Count];

            for (int i = 0; i < DaggerfallSkills.Count; i++)
            {
                Int16 skillValue = reader.ReadInt16();
                Int16 skillCounterValue = reader.ReadInt16();
                reader.ReadInt16(); // Seems to always be 00
                skills.SetSkillValue(i, skillValue);
                skillUses[i] = skillCounterValue;
            }

            return skills;
        }

        UInt32[] ReadEquippedItems(BinaryReader reader)
        {
            const int equippedCount = 27;   // May actually be 35 based on data between here and next record. Test and confirm.

            UInt32[] equippedItems = new UInt32[equippedCount];
            for (int i = 0; i < equippedCount; i++)
            {
                equippedItems[i] = reader.ReadUInt32();
            }

            return equippedItems;
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
            public Int32 startingLevelUpSkillSum; // The starting total of all the primary skills, the two top major skills and the top minor skill
            public Int32 opponentUnknown;
            public Int16 baseHealth;
            public Int16 currentHealth;
            public Int16 maxHealth;
            public Byte faceIndex;
            public Byte level;
            public PlayerReflexes reflexes;
            public UInt32 physicalGold;
            public Int16 currentSpellPoints;
            public Int16 maxSpellPoints;
            public Int16 reputationCommoners;
            public Int16 reputationNobility;
            public Int16 reputationScholars;
            public Int16 reputationMerchants;
            public Int16 reputationUnderworld;
            public UInt16 currentFatigue;
            public DaggerfallSkills skills;
            public Int16[] skillUses;
            public UInt32[] equippedItems;
            public Races race2; // Stores character's original race for when returning from being a vampire, werewolf or wereboar
            public UInt32 timeStamp;
            public DFCareer career;
        }

        #endregion
    }
}
