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
            doc.reputationCommoners = parsedData.reputationCommoners;
            doc.reputationMerchants = parsedData.reputationMerchants;
            doc.reputationNobility = parsedData.reputationNobility;
            doc.reputationScholars = parsedData.reputationScholars;
            doc.reputationUnderworld = parsedData.reputationUnderworld;
            doc.currentFatigue = parsedData.currentFatigue;
            doc.skillUses = parsedData.skillUses;
            doc.startingLevelUpSkillSum = parsedData.startingLevelUpSkillSum;
            doc.minMetalToHit = parsedData.minMetalToHit;
            doc.armorValues = parsedData.armorValues;
            doc.lastTimePlayerBoughtTraining = parsedData.lastTimePlayerBoughtTraining;
            doc.timeForThievesGuildLetter = parsedData.timeForThievesGuildLetter;
            doc.timeForDarkBrotherhoodLetter = parsedData.timeForDarkBrotherhoodLetter;
            doc.darkBrotherhoodRequirementTally = parsedData.darkBrotherhoodRequirementTally;
            doc.thievesGuildRequirementTally = parsedData.thievesGuildRequirementTally;
            doc.biographyReactionMod = parsedData.biographyReactionMod;

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
            parsedData.minMetalToHit = reader.ReadByte();
            parsedData.race = ReadRace(reader);

            sbyte[] armorValues = new sbyte[7];
            for (int i = 0; i < 7; i++)
            {
                armorValues[i] = reader.ReadSByte();
            }
            parsedData.armorValues = armorValues;

            parsedData.skillUnknown1 = reader.ReadInt32();
            parsedData.skillUnknown2 = reader.ReadInt32();
            parsedData.startingLevelUpSkillSum = reader.ReadInt32();

            parsedData.baseHealth = reader.ReadInt16();

            reader.BaseStream.Position = 0x60;
            parsedData.timePlayerBecameWerebeast = reader.ReadUInt32();

            reader.BaseStream.Position = 0x6c;
            parsedData.unknownLycanthropy = reader.ReadInt16();

            reader.BaseStream.Position = 0x74;
            parsedData.playerHouse = reader.ReadUInt32();
            parsedData.playerShip = reader.ReadUInt32();

            parsedData.currentHealth = reader.ReadInt16();
            parsedData.maxHealth = reader.ReadInt16();

            parsedData.faceIndex = reader.ReadByte();
            parsedData.level = reader.ReadByte();

            reader.BaseStream.Position = 0x83;
            parsedData.reflexes = ReadReflexes(reader);

            reader.BaseStream.Position = 0x85;
            parsedData.physicalGold = reader.ReadUInt32();

            parsedData.magicEffects1 = reader.ReadByte();
            parsedData.magicEffects2 = reader.ReadByte();
            parsedData.magicEffects3 = reader.ReadByte();

            reader.BaseStream.Position = 0x8D;
            parsedData.currentSpellPoints = reader.ReadInt16();
            parsedData.maxSpellPoints = reader.ReadInt16();

            parsedData.reputationCommoners = reader.ReadInt16();
            parsedData.reputationMerchants = reader.ReadInt16();
            parsedData.reputationScholars = reader.ReadInt16();
            parsedData.reputationNobility = reader.ReadInt16();
            parsedData.reputationUnderworld = reader.ReadInt16();

            parsedData.currentFatigue = reader.ReadUInt16();

            parsedData.skills = ReadSkills(reader, out parsedData.skillUses);

            parsedData.equippedItems = ReadEquippedItems(reader);

            reader.BaseStream.Position = 0x1f2;
            parsedData.race2 = ReadRace(reader);
            parsedData.timeToBecomeVampireOrWerebeast = reader.ReadUInt32();

            reader.BaseStream.Position = 0x1fd;
            parsedData.timeStamp = reader.ReadUInt32();

            reader.BaseStream.Position = 0x209;
            parsedData.lastTimePlayerBoughtTraining = reader.ReadUInt32();

            reader.BaseStream.Position = 0x211;
            parsedData.timeForThievesGuildLetter = reader.ReadUInt32();
            parsedData.timeForDarkBrotherhoodLetter = reader.ReadUInt32();

            reader.BaseStream.Position = 0x21f;
            parsedData.darkBrotherhoodRequirementTally = reader.ReadByte();

            reader.BaseStream.Position = 0x222;
            parsedData.thievesGuildRequirementTally = reader.ReadByte();

            reader.BaseStream.Position = 0x224;
            parsedData.biographyReactionMod = reader.ReadSByte();

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
                result.SetPermanentStatValue(i, reader.ReadInt16());
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

        byte ReadTransportationFlags(BinaryReader reader)
        {
            // Known values:
            // x1 = Foot
            // x2 = Horse
            // x4 = Cart
            return reader.ReadByte();
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
            const int equippedCount = 27;

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
            public byte transportationFlags;
            public byte minMetalToHit;
            public Races race;
            public sbyte[] armorValues;
            public Int32 skillUnknown1; // Seems to be related to skills and leveling
            public Int32 skillUnknown2; // Seems to be related to skills and leveling
            public Int32 startingLevelUpSkillSum; // The starting total of all the primary skills, the two top major skills and the top minor skill
            public Int16 baseHealth;
            public UInt32 timePlayerBecameWerebeast; // Needs confirming
            public Int16 unknownLycanthropy; // Lycanthropy stage? Set when inflicted with lycanthropy.
            public UInt32 playerHouse; // Building ID of player's house. 0 if player doesn't own a house.
            public UInt32 playerShip; // Probably same type of data as above, for player's ship. 0 if player doesn't own a ship.
            public Int16 currentHealth;
            public Int16 maxHealth;
            public Byte faceIndex;
            public Byte level;
            public PlayerReflexes reflexes;
            public UInt32 physicalGold;
            public Byte magicEffects1; // x1 = paralyzed, x4 = invisible, x8 = levitating, x20 = lock, x40 = open
            public Byte magicEffects2; // x1 = silenced, x10 = chameleon, x20 = shade, x40 = slowfall
            public Byte magicEffects3; // x1 = jump, x8 = waterbreathing,, x10 = waterwalking
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
            // For monsters these are for attack damage, but player uses hand-to-hand skill for unarmed attacks.
            // Might also be used by the player when in werebeast form.
            // Up to 5 attacks appear to be supported, but no monster in the game has more than 3.
            /*public Int16 attackDamageMin1; // Set to 1 for new player character.
            public Int16 attackDamageMax1; // Set to 2 for new player character.
            public Int16 attackDamageMin2;
            public Int16 attackDamageMax2;
            public Int16 attackDamageMin3;
            public Int16 attackDamageMax3;
            public Int16 attackDamageMin4;
            public Int16 attackDamageMax4;
            public Int16 attackDamageMin5;
            public Int16 attackDamageMax5;*/
            public Races race2; // Stores character's original race for when returning from being a vampire, werewolf or wereboar
            public UInt32 timeToBecomeVampireOrWerebeast; // Should equal three days after infection.
            public UInt32 timeStamp; // Time of last kill by vampires and werewolves?
            public UInt32 lastTimePlayerCastLycanthropy;
            public UInt32 lastTimePlayerAteOrDrankAtTavern;
            public UInt32 lastTimePlayerBoughtTraining;
            public UInt32 timeForThievesGuildLetter;
            public UInt32 timeForDarkBrotherhoodLetter;
            public Byte vampireClan;
            public Byte effectStrength; // Used for Open and Shade effects at least.
            public Byte darkBrotherhoodRequirementTally;
            public Byte thievesGuildRequirementTally;
            public SByte biographyReactionMod;
            public DFCareer career;
        }

        #endregion
    }
}
