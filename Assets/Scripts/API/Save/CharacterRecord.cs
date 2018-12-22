// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.IO;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop;

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

            // Strip back classic changes for vampire or lycanthrope as this is handled by effect system in DFU
            // If player is not transformed then this will simply return parsedData.race + 1
            Races classicTransformedRace;
            Races liveRace = StripTransformedRace(out classicTransformedRace);

            doc.raceTemplate = raceDict[(int)liveRace];
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
            doc.skillsRaisedThisLevel1 = parsedData.skillsRaisedThisLevel1;
            doc.skillsRaisedThisLevel2 = parsedData.skillsRaisedThisLevel2;
            doc.startingLevelUpSkillSum = parsedData.startingLevelUpSkillSum;
            doc.minMetalToHit = parsedData.minMetalToHit;
            doc.armorValues = parsedData.armorValues;
            doc.timeToBecomeVampireOrWerebeast = parsedData.timeToBecomeVampireOrWerebeast;
            doc.hasStartedInitialVampireQuest = parsedData.hasStartedInitialVampireQuest;
            doc.lastTimeVampireNeedToKillSatiated = parsedData.lastTimeVampireNeedToKillSatiated;
            doc.lastTimePlayerAteOrDrankAtTavern = parsedData.lastTimePlayerAteOrDrankAtTavern;
            doc.lastTimePlayerBoughtTraining = parsedData.lastTimePlayerBoughtTraining;
            doc.timeForThievesGuildLetter = parsedData.timeForThievesGuildLetter;
            doc.timeForDarkBrotherhoodLetter = parsedData.timeForDarkBrotherhoodLetter;
            doc.vampireClan = parsedData.vampireClan;
            doc.darkBrotherhoodRequirementTally = parsedData.darkBrotherhoodRequirementTally;
            doc.thievesGuildRequirementTally = parsedData.thievesGuildRequirementTally;
            doc.biographyReactionMod = parsedData.biographyReactionMod;
            doc.classicTransformedRace = classicTransformedRace;

            return doc;
        }

        Races StripTransformedRace(out Races classicTransformedRace)
        {
            // Restore original character race if vampire or lycanthrope
            // Racial overrides are handled by the effect system in DFU rather than entirely hardcoded, but still need to handle importing from classic
            Races liveRace = parsedData.race + 1;
            classicTransformedRace = Races.None;
            if (liveRace == Races.Vampire || liveRace == Races.Werewolf || liveRace == Races.Wereboar)
            {
                classicTransformedRace = liveRace;
                liveRace = parsedData.race2 + 1;
            }

            // Remove vampire bonuses to stats and skills
            if (classicTransformedRace == Races.Vampire)
            {
                // Remove +20 bonus to stats
                parsedData.currentStats.SetPermanentStatValue(DFCareer.Stats.Strength, parsedData.currentStats.PermanentStrength - 20);
                parsedData.currentStats.SetPermanentStatValue(DFCareer.Stats.Willpower, parsedData.currentStats.PermanentWillpower - 20);
                parsedData.currentStats.SetPermanentStatValue(DFCareer.Stats.Agility, parsedData.currentStats.PermanentAgility - 20);
                parsedData.currentStats.SetPermanentStatValue(DFCareer.Stats.Endurance, parsedData.currentStats.PermanentEndurance - 20);
                parsedData.currentStats.SetPermanentStatValue(DFCareer.Stats.Personality, parsedData.currentStats.PermanentPersonality - 20);
                parsedData.currentStats.SetPermanentStatValue(DFCareer.Stats.Speed, parsedData.currentStats.PermanentSpeed - 20);
                parsedData.currentStats.SetPermanentStatValue(DFCareer.Stats.Luck, parsedData.currentStats.PermanentLuck - 20);
                if ((VampireClans)parsedData.vampireClan == VampireClans.Anthotis)
                    parsedData.currentStats.SetPermanentStatValue(DFCareer.Stats.Intelligence, parsedData.currentStats.PermanentIntelligence - 20);

                // Remove +30 bonus to vampire skills
                parsedData.skills.SetPermanentSkillValue(DFCareer.Skills.Jumping, (short)(parsedData.skills.GetPermanentSkillValue(DFCareer.Skills.Jumping) - 30));
                parsedData.skills.SetPermanentSkillValue(DFCareer.Skills.Running, (short)(parsedData.skills.GetPermanentSkillValue(DFCareer.Skills.Running) - 30));
                parsedData.skills.SetPermanentSkillValue(DFCareer.Skills.Stealth, (short)(parsedData.skills.GetPermanentSkillValue(DFCareer.Skills.Stealth) - 30));
                parsedData.skills.SetPermanentSkillValue(DFCareer.Skills.CriticalStrike, (short)(parsedData.skills.GetPermanentSkillValue(DFCareer.Skills.CriticalStrike) - 30));
                parsedData.skills.SetPermanentSkillValue(DFCareer.Skills.Climbing, (short)(parsedData.skills.GetPermanentSkillValue(DFCareer.Skills.Climbing) - 30));
                parsedData.skills.SetPermanentSkillValue(DFCareer.Skills.HandToHand, (short)(parsedData.skills.GetPermanentSkillValue(DFCareer.Skills.HandToHand) - 30));

                // Remove vampire advantages/disadvantages
                // NOTES:
                //  * This will also strip similar advantages/disadvantages selected at character creation time
                //  * Need a way to differentiate base vs. transformed specials so they can be restored to pre-transform state only
                //  * DFU does not deliver most of these effects via the career template anyway, rather uses effect system
                //  * Custom effect mods have no way of showing specials on History popup either
                //  * Will need to find a solution to help unify specials popup output with modern effect system while supporting classic
                parsedData.career.DamageFromSunlight = false;
                parsedData.career.DamageFromSunlight = false;
                parsedData.career.DamageFromHolyPlaces = false;
                parsedData.career.Paralysis = DFCareer.Tolerance.Normal;
                parsedData.career.Disease = DFCareer.Tolerance.Normal;
            }

            // TODO: Remove werewolf/wereboar bonuses to stats and skills and instantiate racial override effect

            return liveRace;
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

            reader.BaseStream.Position = 0x50;

            parsedData.skillsRaisedThisLevel1 = reader.ReadUInt32();
            parsedData.skillsRaisedThisLevel2 = reader.ReadUInt32();
            parsedData.startingLevelUpSkillSum = reader.ReadInt32();

            parsedData.baseHealth = reader.ReadInt16();

            reader.BaseStream.Position = 0x60;
            parsedData.lastTimeUrgeToHuntInnocentSatisfied = reader.ReadUInt32();
            parsedData.timeAfterWhichShieldEffectWillEnd = reader.ReadUInt32();
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
            parsedData.magicEffects4 = reader.ReadByte();

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

            reader.BaseStream.Position = 0x1f8;
            parsedData.hasStartedInitialVampireQuest = reader.ReadByte();

            reader.BaseStream.Position = 0x1fd;
            parsedData.lastTimeVampireNeedToKillSatiated = reader.ReadUInt32();

            reader.BaseStream.Position = 0x205;
            parsedData.lastTimePlayerAteOrDrankAtTavern = reader.ReadUInt32();
            parsedData.lastTimePlayerBoughtTraining = reader.ReadUInt32();

            reader.BaseStream.Position = 0x211;
            parsedData.timeForThievesGuildLetter = reader.ReadUInt32();
            parsedData.timeForDarkBrotherhoodLetter = reader.ReadUInt32();
            parsedData.shieldEffectAmount = reader.ReadUInt32();
            parsedData.vampireClan = reader.ReadByte();

            reader.BaseStream.Position = 0x21f;
            parsedData.darkBrotherhoodRequirementTally = reader.ReadByte();

            reader.BaseStream.Position = 0x222;
            parsedData.thievesGuildRequirementTally = reader.ReadByte();

            reader.BaseStream.Position = 0x224;
            parsedData.biographyReactionMod = reader.ReadSByte();
            parsedData.resistanceToFire = reader.ReadByte();
            parsedData.resistanceToFrost = reader.ReadByte();
            parsedData.resistanceToDiseaseAndPoison = reader.ReadByte();
            parsedData.resistanceToShock = reader.ReadByte();
            parsedData.resistanceToMagicka = reader.ReadByte();

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
                skills.SetPermanentSkillValue(i, skillValue);
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
            public UInt32 skillsRaisedThisLevel1; // Flags for skills 0 through 31.
            public UInt32 skillsRaisedThisLevel2; // Flags for skills 32 through 34.
            public Int32 startingLevelUpSkillSum; // The starting total of all the primary skills, the two top major skills and the top minor skill
            public Int16 baseHealth;
            public UInt32 lastTimeUrgeToHuntInnocentSatisfied;
            public UInt32 timeAfterWhichShieldEffectWillEnd;
            public Int16 unknownLycanthropy; // Lycanthropy stage? Set when inflicted with lycanthropy.
            public UInt32 playerHouse; // Building ID of player's house. 0 if player doesn't own a house.
            public UInt32 playerShip; // Probably same type of data as above, for player's ship. 0 if player doesn't own a ship.
            public Int16 currentHealth;
            public Int16 maxHealth;
            public Byte faceIndex;
            public Byte level;
            public PlayerReflexes reflexes;
            public UInt32 physicalGold;
            public Byte magicEffects1; // x1 = paralyzed, x2 = resist fire, x4 = invisible, x8 = levitating, x10 = light, x20 = lock, x40 = open, x80 = regenerating
            public Byte magicEffects2; // x1 = silenced, x2 = spell absorption, x4 = spell reflection, x8 = spell resistance, x10 = chameleon, x20 = shade, x40 = slowfall, x80 = climbing
            public Byte magicEffects3; // x1 = jumping, x2 = free action, x4 = lycanthropy, x8 = water breathing, x10 = water walking, x20 = diminution (not implemented), x40 = shield, x80 = detect
            public Byte magicEffects4; // x1 = darkness, x2 = tongues, x4 = intensify fire (not implemented), x8 = diminish fire (not implemented), x10 = resist frost, x20 = resist disease/poison, x40 = resist shock, 0x80 = resist magicka
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
            public UInt32 timeToBecomeVampireOrWerebeast; // Three days after infection.
            public Byte hasStartedInitialVampireQuest;
            public UInt32 lastTimeVampireNeedToKillSatiated;
            public UInt32 lastTimePlayerCastLycanthropy;
            public UInt32 lastTimePlayerAteOrDrankAtTavern;
            public UInt32 lastTimePlayerBoughtTraining;
            public UInt32 timeForThievesGuildLetter;
            public UInt32 timeForDarkBrotherhoodLetter;
            public UInt32 shieldEffectAmount;
            public Byte vampireClan;
            public Byte effectStrength; // Used for Open effect at least.
            public Byte darkBrotherhoodRequirementTally;
            public Byte thievesGuildRequirementTally;
            public SByte biographyReactionMod;
            public Byte resistanceToFire;
            public Byte resistanceToFrost;
            public Byte resistanceToDiseaseAndPoison;
            public Byte resistanceToShock;
            public Byte resistanceToMagicka;
            public DFCareer career;
        }

        #endregion
    }
}
