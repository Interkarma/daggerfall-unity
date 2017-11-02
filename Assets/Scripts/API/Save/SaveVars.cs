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
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallConnect.Save
{
    /// <summary>
    /// Represents a SAVEVARS.DAT file (must be a .175 or later save).
    /// </summary>
    public class SaveVars
    {
        #region Fields

        const string filename = "SAVEVARS.DAT";
        const int biographyModifiersOffset = 0x30;
        const int emperorSonNameOffset = 0x7C;
        const int travelFlagsOffset = 0xF5;
        const int maceOfMolagBalVarsOffset = 0x33F;

        const int globalVarsCount = 64;

        const int isDayOffset = 0x391;
        const int typeOfCrimeCommittedOffset = 0x3A3;
        const int inDungeonWaterOffset = 0x3A6;
        const int weatherTypesOffset = 0x3B7;
        const int weaponDrawnOffset = 0x3BF;
        const int gameTimeOffset = 0x3C9;
        const int maleOrFemaleClothingOffset = 0x3D1; // 6 for male clothing or 12 for female clothing, matching player gender. Not bothering to read right now.
        const int usingLeftHandWeaponOffset = 0x3D9;
        const int currentRegionIdOffset = 0x173A; // Not bothering to read right now.
        const int cheatFlagsOffset = 0x173B;
        const int lastSkillCheckTimeOffset = 0x179A;
        const int dungeonWaterLevelOffset = 0x17A8; // Not bothering to read right now.

        const int regionDataOffset = 0x3DA;
        const int regionDataLength = 80;
        const int regionCount = 62;

        const int factionDataOffset = 0x17D0;
        const int factionDataLength = 92;

        // Private fields

        FileProxy saveVarsFile = new FileProxy();

        int biographyResistDiseaseMod = 0;
        int biographyResistMagicMod = 0;
        int biographyAvoidHitMod = 0;
        int biographyResistPoisonMod = 0;
        int biographyFatigueMod = 0;

        string emperorSonName = ""; // Randomly chosen and can be used in character history, where it fills in %imp.

        bool cautiousTravel = false;
        bool innsTravel = false;
        bool footOrHorseTravel = false;

        int maceOfMolagBalSpellPointBonus = 0;
        int maceOfMolagBalStrengthBonus = 0;
        uint maceOfMolagBalSpellPointBonusTimeLimit = 0;
        uint maceOfMolagBalStrengthBonusTimeLimit = 0;

        byte[] globalVars = new byte[globalVarsCount];

        short lastSpellCost = 0; // The cost of the last spell that was readied. If the spell is aborted, these spell points are returned.

        bool isDay = false;
        byte typeOfCrimeCommitted = 0;
        bool inDungeonWater = false;
        byte[] weatherTypes; // Probably the possible weather types for the player's location
        bool weaponDrawn = false;
        uint gameTime = 0;
        bool usingLeftHandWeapon = false;

        bool allMapLocationsRevealedMode = false;
        bool godMode = false;

        uint lastSkillCheckTime = 0;

        List<PlayerEntity.RegionDataRecord> regionDataList = new List<PlayerEntity.RegionDataRecord>();
        List<FactionFile.FactionData> factions = new List<FactionFile.FactionData>();

        #endregion

        #region Structures and Enumerations

        /// <summary>
        /// Emperor's son's name.
        /// </summary>
        string[] emperorSonNames = { "Pelagius", "Cephorus", "Uriel", "Cassynder", "Voragiel", "Trabbatus" };

        /// <summary>
        /// Travel flags.
        /// </summary>
        [Flags]
        public enum TravelFlags
        {
            Cautiously = 0x01,
            Recklessly = 0x02,
            FootOrHorse = 0x04,
            Ship = 0x08,
            Inns = 0x10,
            CampOut = 0x20,
        }

        /// <summary>
        /// Cheat flags.
        /// </summary>
        [Flags]
        public enum CheatFlags
        {
            AllMapLocationsRevealedMode = 0x08,
            NoCollision = 0x20, // If in dungeon, player falls down through the map geometry. Outside, the ground is still solid and you can move around through 3d objects.
            GodMode = 0x40,
            EnemiesCantCastSpells = 0x80,
        }

        #endregion

        #region Properties

        public static string Filename
        {
            get { return filename; }
        }

        /// <summary>
        /// Gets biography resist modifier from savevars.
        /// </summary>
        public int BiographyResistDiseaseMod
        {
            get { return biographyResistDiseaseMod; }
        }

        /// <summary>
        /// Gets biography magic modifier from savevars.
        /// </summary>
        public int BiographyResistMagicMod
        {
            get { return biographyResistMagicMod; }
        }

        /// <summary>
        /// Gets biography hit avoidance modifier from savevars.
        /// </summary>
        public int BiographyAvoidHitMod
        {
            get { return biographyAvoidHitMod; }
        }

        /// <summary>
        /// Gets biography resist poison modifier from savevars.
        /// </summary>
        public int BiographyResistPoisonMod
        {
            get { return biographyResistPoisonMod; }
        }

        /// <summary>
        /// Gets biography fatigue modifier from savevars.
        /// </summary>
        public int BiographyFatigueMod
        {
            get { return biographyFatigueMod; }
        }

        /// <summary>
        /// Gets Emperor's son's name from savevars.
        /// </summary>
        public string EmperorSonName
        {
            get { return emperorSonName; }
        }

        /// <summary>
        /// Gets whether cautious travel is set from savevars.
        /// </summary>
        public bool CautiousTravel
        {
            get { return cautiousTravel; }
        }

        /// <summary>
        /// Gets whether inns travel is set from savevars.
        /// </summary>
        public bool InnsTravel
        {
            get { return innsTravel; }
        }

        /// <summary>
        /// Gets whether foot/horse travel is set from savevars.
        /// </summary>
        public bool FootOrHorseTravel
        {
            get { return footOrHorseTravel; }
        }

        /// <summary>
        /// Gets Mace of Molag Bal spell point bonus, read from savevars.
        /// </summary>
        public int MaceOfMolagBalSpellPointBonus
        {
            get { return maceOfMolagBalSpellPointBonus; }
        }

        /// <summary>
        /// Gets Mace of Molag Bal strength bonus, read from savevars.
        /// </summary>
        public int MaceOfMolagBalStrengthBonus
        {
            get { return maceOfMolagBalStrengthBonus; }
        }

        /// <summary>
        /// Gets time Mace of Molag Bal spell point bonus ends, read from savevars.
        /// </summary>
        public uint MaceOfMolagBalSpellPointBonusTimeLimit
        {
            get { return maceOfMolagBalSpellPointBonusTimeLimit; }
        }

        /// <summary>
        /// Gets time Mace of Molag Bal strength bonus ends, read from savevars.
        /// </summary>
        public uint MaceOfMolagBalStrengthBonusTimeLimit
        {
            get { return maceOfMolagBalStrengthBonusTimeLimit; }
        }

        /// <summary>
        /// Gets whether it is daytime from savevars.
        /// </summary>
        public bool IsDay
        {
            get { return isDay; }
        }

        /// <summary>
        /// Gets type of crime committed by player from savevars.
        /// </summary>
        public byte TypeOfCrimeCommitted
        {
            get { return typeOfCrimeCommitted; }
        }

        /// <summary>
        /// Gets whether character is in water in a dungeon from savevars.
        /// </summary>
        public bool InDungeonWater
        {
            get { return inDungeonWater; }
        }

        /// <summary>
        /// Gets current possible weather types?
        /// </summary>
        public byte[] WeatherTypes
        {
            get { return weatherTypes; }
        }

        /// <summary>
        /// Gets whether weapon is drawn from savevars.
        /// </summary>
        public bool WeaponDrawn
        {
            get { return weaponDrawn; }
        }

        /// <summary>
        /// Gets the cost of the last readied spell from savevars.
        /// </summary>
        public short LastSpellCost
        {
            get { return lastSpellCost; }
        }

        /// <summary>
        /// Gets game time read from savevars.
        /// </summary>
        public uint GameTime
        {
            get { return gameTime; }
        }

        /// <summary>
        /// Gets whether left-hand weapon is being used from savevars.
        /// </summary>
        public bool UsingLeftHandWeapon
        {
            get { return usingLeftHandWeapon; }
        }

        /// <summary>
        /// Gets whether invulnerability cheat is on from savevars.
        /// </summary>
        public bool GodMode
        {
            get { return godMode; }
        }

        /// <summary>
        /// Gets whether cheat to reveal all map locations is on from savevars.
        /// </summary>
        public bool AllMapLocationsRevealedMode
        {
            get { return allMapLocationsRevealedMode; }
        }

        /// <summary>
        /// Gets time of last check for raising skills, read from savevars.
        /// </summary>
        public uint LastSkillCheckTime
        {
            get { return lastSkillCheckTime; }
        }

        /// <summary>
        /// Gets array of regionData read from savevars.
        /// </summary>
        public PlayerEntity.RegionDataRecord[] RegionData
        {
            get { return regionDataList.ToArray(); }
        }

        /// <summary>
        /// Gets array of factions read from savevars.
        /// </summary>
        public FactionFile.FactionData[] Factions
        {
            get { return factions.ToArray(); }
        }

        /// <summary>
        /// Gets array of global variables.
        /// </summary>
        public byte[] GlobalVars
        {
            get { return globalVars; }
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
            ReadBiographyModifiers(reader);
            ReadEmperorSonName(reader);
            ReadTravelFlags(reader);
            ReadMaceOfMolagBalVars(reader);
            ReadGlobalVars(reader);
            ReadLastSpellCost(reader);
            ReadIsDay(reader);
            ReadTypeOfCrimeCommitted(reader);
            ReadInDungeonWater(reader);
            ReadWeatherTypes(reader);
            ReadWeaponDrawn(reader);
            ReadGameTime(reader);
            ReadUsingLeftHandWeapon(reader);
            ReadCheatFlags(reader);
            ReadLastSkillCheckTime(reader);
            ReadRegionData(reader);
            ReadFactionData(reader);

            return true;
        }

        #endregion

        #region Private Methods

        void ReadBiographyModifiers(BinaryReader reader)
        {
            reader.BaseStream.Position = biographyModifiersOffset;
            biographyResistDiseaseMod = reader.ReadInt32();
            biographyResistMagicMod = reader.ReadInt32();
            biographyAvoidHitMod = reader.ReadInt32();
            biographyResistPoisonMod = reader.ReadInt32();
            biographyFatigueMod = reader.ReadInt32();
        }

        void ReadEmperorSonName(BinaryReader reader)
        {
            reader.BaseStream.Position = emperorSonNameOffset;
            emperorSonName = emperorSonNames[reader.ReadByte()];
        }

        void ReadTravelFlags(BinaryReader reader)
        {
            reader.BaseStream.Position = travelFlagsOffset;
            short flags = reader.ReadInt16();
            if ((flags & (byte)TravelFlags.Cautiously) != 0)
                cautiousTravel = true;
            if ((flags & (byte)TravelFlags.FootOrHorse) != 0)
                footOrHorseTravel = true;
            if ((flags & (byte)TravelFlags.Inns) != 0)
                innsTravel = true;
        }

        void ReadMaceOfMolagBalVars(BinaryReader reader)
        {
            reader.BaseStream.Position = maceOfMolagBalVarsOffset;
            maceOfMolagBalSpellPointBonus = reader.ReadInt32();
            maceOfMolagBalStrengthBonus = reader.ReadInt32();
            maceOfMolagBalSpellPointBonusTimeLimit = reader.ReadUInt32();
            maceOfMolagBalStrengthBonusTimeLimit = reader.ReadUInt32();
        }

        void ReadGlobalVars(BinaryReader reader)
        {
            // Offset 0x34F
            globalVars = reader.ReadBytes(globalVarsCount);
        }

        void ReadLastSpellCost(BinaryReader reader)
        {
            // Offset 0x38F
            lastSpellCost = reader.ReadInt16();
        }

        void ReadIsDay(BinaryReader reader)
        {
            reader.BaseStream.Position = isDayOffset;
            if (reader.ReadByte() == 1)
                isDay = true;
        }

        void ReadTypeOfCrimeCommitted(BinaryReader reader)
        {
            reader.BaseStream.Position = typeOfCrimeCommittedOffset;
            typeOfCrimeCommitted = reader.ReadByte();
        }

        void ReadInDungeonWater(BinaryReader reader)
        {
            reader.BaseStream.Position = inDungeonWaterOffset;
            if (reader.ReadByte() == 1)
                inDungeonWater = true;
        }

        void ReadWeatherTypes(BinaryReader reader)
        {
            reader.BaseStream.Position = weatherTypesOffset;
            weatherTypes = reader.ReadBytes(6);
        }

        void ReadWeaponDrawn(BinaryReader reader)
        {
            reader.BaseStream.Position = weaponDrawnOffset;
            byte flags = reader.ReadByte();
            // This byte's various flags are set and cleared when in-game UIs are open and closed.
            // It might be related to layering images onto the screen.
            // The only value that seems to be relevant to loading a saved game is 0x40, which is set when
            // the player's weapon is drawn.
            if ((flags & 0x40) != 0)
                weaponDrawn = true;
        }

        void ReadGameTime(BinaryReader reader)
        {
            reader.BaseStream.Position = gameTimeOffset;
            gameTime = reader.ReadUInt32();
        }

        void ReadUsingLeftHandWeapon(BinaryReader reader)
        {
            reader.BaseStream.Position = usingLeftHandWeaponOffset;
            if (reader.ReadByte() == 1)
                usingLeftHandWeapon = true;
        }

        void ReadCheatFlags(BinaryReader reader)
        {
            reader.BaseStream.Position = cheatFlagsOffset;
            byte flags = reader.ReadByte();
            if ((flags & (byte)CheatFlags.AllMapLocationsRevealedMode) != 0)
                allMapLocationsRevealedMode = true;
            if ((flags & (byte)CheatFlags.GodMode) != 0)
                godMode = true;
        }

        void ReadLastSkillCheckTime(BinaryReader reader)
        {
            reader.BaseStream.Position = lastSkillCheckTimeOffset;
            lastSkillCheckTime = reader.ReadUInt32();
        }

        void ReadRegionData(BinaryReader reader)
        {
            // Step through region data
            regionDataList.Clear();
            for (int i = 0; i < regionCount; i++)
            {
                PlayerEntity.RegionDataRecord regionData = new PlayerEntity.RegionDataRecord();
                reader.BaseStream.Position = regionDataOffset + (i * regionDataLength);
                regionData.Values = new byte[29];

                for (int j = 0; j < 29; j++)
                    regionData.Values[j] = reader.ReadByte();

                regionData.Flags = new bool[29];
                for (int j = 0; j < 29; j++)
                    regionData.Flags[j] = reader.ReadBoolean();

                regionData.Flags2 = new bool[14];
                for (int j = 0; j < 14; j++)
                    regionData.Flags2[j] = reader.ReadBoolean();

                reader.BaseStream.Position += 2; // unknown

                regionData.LegalRep = reader.ReadInt16();
                regionData.Unknown = reader.ReadUInt16();
                regionData.PriceAdjustment = reader.ReadUInt16();

                regionDataList.Add(regionData);
            }
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