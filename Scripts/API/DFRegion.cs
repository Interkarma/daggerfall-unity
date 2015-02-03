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
#endregion

namespace DaggerfallConnect
{
    /// <summary>
    /// Stores information about locations in a region.
    /// </summary>
    public struct DFRegion
    {
        #region Structure Variables

        /// <summary>
        /// Name of this region.
        /// </summary>
        public String Name;

        /// <summary>
        /// Number of locations in this region. Not all regions have locations.
        ///  Always check LocationCount before working with MapNames array.
        /// </summary>
        public UInt32 LocationCount;

        /// <summary>
        /// Contains the names of all locations for this region.
        /// </summary>
        public string[] MapNames;

        /// <summary>
        /// Contains extended data about each location in this region.
        /// </summary>
        public RegionMapTable[] MapTable;

        /// <summary>
        /// Dictionary to find map index from MapID.
        /// </summary>
        public Dictionary<int, int> MapIdLookup;

        /// <summary>
        /// Dictionary to find a map index from map name. Note that some map names
        ///  are duplicates. Only the first instance will be stored and subsequent
        ///  duplicates discarded. These locations can still be referenced by MapID or index.
        /// </summary>
        public Dictionary<string, int> MapNameLookup;

        #endregion

        #region Child Structures

        /// <summary>
        /// Describes location types that appear on the automap.
        /// </summary>
        public enum LocationTypes
        {
            /// <summary>Large dungeon.</summary>
            DungeonLabyrinth = 132,
            /// <summary>Medium dungeon.</summary>
            DungeonKeep = 135,
            /// <summary>Small dungeon.</summary>
            DungeonRuin = 138,

            /// <summary>Graveyard visible.</summary>
            GraveyardCommon = 172,
            /// <summary>Graveyard hidden.</summary>
            GraveyardForgotten = 140,

            /// <summary>Farmhouse.</summary>
            HomeFarms = 163,
            /// <summary>Wealthy home.</summary>
            HomeWealthy = 168,
            /// <summary>Poor home.</summary>
            HomePoor = 171,
            /// <summary>Player ship.</summary>
            HomeYourShips = 174,

            /// <summary>Tavern</summary>
            Tavern = 166,

            /// <summary>Temple.</summary>
            ReligionTemple = 165,
            /// <summary>Cult.</summary>
            ReligionCult = 169,
            /// <summary>Coven.</summary>
            ReligionCoven = 141,

            /// <summary>Large settlement.</summary>
            TownCity = 160,
            /// <summary>Medium settlement.</summary>
            TownHamlet = 161,
            /// <summary>Small settlement.</summary>
            TownVillage = 162,
        }

        /// <summary>
        /// These are the building types used for the automap (as value+1), quest subsystem, and
        ///  location building database.
        /// </summary>
        public enum BuildingTypes
        {
            /// <summary>Alchemist</summary>
            Alchemist = 0x00,
            /// <summary>HouseForSale</summary>
            HouseForSale = 0x01,
            /// <summary>Amorer</summary>
            Amorer = 0x02,
            /// <summary>Bank</summary>
            Bank = 0x03,
            /// <summary>Town4</summary>
            Town4 = 0x04,
            /// <summary>Bookseller</summary>
            Bookseller = 0x05,
            /// <summary>ClothingStore</summary>
            ClothingStore = 0x06,
            /// <summary>FurnitureStore</summary>
            FurnitureStore = 0x07,
            /// <summary>GemStore</summary>
            GemStore = 0x08,
            /// <summary>GeneralStore</summary>
            GeneralStore = 0x09,
            /// <summary>Library</summary>
            Library = 0x0a,
            /// <summary>Guildhall</summary>
            Guildhall = 0x0b,
            /// <summary>PawnShop</summary>
            PawnShop = 0x0c,
            /// <summary>WeaponSmith</summary>
            WeaponSmith = 0x0d,
            /// <summary>Temple</summary>
            Temple = 0x0e,
            /// <summary>Tavern</summary>
            Tavern = 0x0f,
            /// <summary>Palace</summary>
            Palace = 0x10,
            /// <summary>House1</summary>
            House1 = 0x11,
            /// <summary>House2</summary>
            House2 = 0x12,
            /// <summary>House3</summary>
            House3 = 0x13,
            /// <summary>House4</summary>
            House4 = 0x14,
            /// <summary>House5</summary>
            House5 = 0x15,
            /// <summary>House6</summary>
            House6 = 0x16,
            /// <summary>Town23</summary>
            Town23 = 0x17,
            /// <summary>Ship</summary>
            Ship = 0x18,
            /// <summary>Special1</summary>
            Special1 = 0x74,
            /// <summary>Special2</summary>
            Special2 = 0xdf,
            /// <summary>Special3</summary>
            Special3 = 0xf9,
            /// <summary>Special4</summary>
            Special4 = 0xfa,
        }

        /// <summary>
        /// Describes dungeon types based on Daggerfall Chronicles.
        /// (value >> 8) = index of type.
        /// </summary>
        public enum DungeonTypes
        {
            Crypt = 51,
            OrcStronghold = 307,
            HumanStronghold = 563,
            Palace = 648,               // Palace also >> 8 to index 2 (HumanStronghold)
            Prison = 819,
            DesecratedTemple = 1075,
            Mine = 1331,
            NaturalCave = 1587,
            Coven = 1843,
            VampireHaunt = 2099,
            Laboratory = 2355,
            HarpyNest = 2611,
            RuinedCastle = 2867,
            SpiderNest = 3123,
            GiantStronghold = 3379,
            DragonsDen = 3635,
            BarbarianStronghold = 3891,
            VolcanicCaves = 4147,
            ScorpionNest = 4403,
            Cemetery = 4659,
            NoDungeon = 65399,
        }

        /// <summary>
        /// String array of dungeon types for display.
        /// </summary>
        public static string[] DungeonTypeNames = new string[]
        {
            "Crypt",
            "Orc Stronghold",
            "Human Stronghold",
            "Prison",
            "Desecrated Temple",
            "Mine",
            "Natural Cave",
            "Coven",
            "Vampire Haunt",
            "Laboratory",
            "Harpy Nest",
            "Ruined Castle",
            "Spider Nest",
            "Giant Stronghold",
            "Dragon's Den",
            "Barbarian Stronghold",
            "Volcanic Caves",
            "Scorpion Nest",
            "Cemetery",
        };

        /// <summary>
        /// Describes a single location.
        /// </summary>
        public struct RegionMapTable
        {
            /// <summary>Numeric ID of this location.</summary>
            public Int32 MapId;

            /// <summary>Unknown.</summary>
            public Byte Unknown1;

            /// <summary>Longitude and Type compressed into a bitfield.</summary>
            public UInt32 LongitudeTypeBitfield;

            /// <summary>Longitude value read from bitfield.</summary>
            public UInt32 Longitude;

            /// <summary>Type value read from bitfield.</summary>
            public LocationTypes Type;

            /// <summary>Latitude of this location.</summary>
            public UInt16 Latitude;

            /// <summary>Dungeon type (Crypt, Orc Stronghold, Human Stronghold, etc.).</summary>
            public DungeonTypes DungeonType;

            /// <summary>Unknown.</summary>
            public UInt32 Unknown3;
        }

        #endregion
    }
}
