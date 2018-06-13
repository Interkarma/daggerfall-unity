﻿// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
            /// <summary>Large settlement.</summary>
            TownCity = 0,
            /// <summary>Medium settlement.</summary>
            TownHamlet = 1,
            /// <summary>Small settlement.</summary>
            TownVillage = 2,
            /// <summary>Farmhouse.</summary>
            HomeFarms = 3,
            /// <summary>Large dungeon.</summary>
            DungeonLabyrinth = 4,
            /// <summary>Temple.</summary>
            ReligionTemple = 5,
            /// <summary>Tavern</summary>
            Tavern = 6,
            /// <summary>Medium dungeon.</summary>
            DungeonKeep = 7,
            /// <summary>Wealthy home.</summary>
            HomeWealthy = 8,
            /// <summary>Cult.</summary>
            ReligionCult = 9,
            /// <summary>Small dungeon.</summary>
            DungeonRuin = 10,
            /// <summary>Poor home.</summary>
            HomePoor = 11,
            /// <summary>Graveyard.</summary>
            Graveyard = 12,
            /// <summary>Coven.</summary>
            Coven = 13,
            /// <summary>Player ship.</summary>
            HomeYourShips = 14,
            /// <summary>No location type.</summary>
            None = 0xffff,
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
            Armorer = 0x02,
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
        /// </summary>
        public enum DungeonTypes
        {
            Crypt = 0,
            OrcStronghold = 1,
            HumanStronghold = 2,
            Prison = 3,
            DesecratedTemple = 4,
            Mine = 5,
            NaturalCave = 6,
            Coven = 7,
            VampireHaunt = 8,
            Laboratory = 9,
            HarpyNest = 10,
            RuinedCastle = 11,
            SpiderNest = 12,
            GiantStronghold = 13,
            DragonsDen = 14,
            BarbarianStronghold = 15,
            VolcanicCaves = 16,
            ScorpionNest = 17,
            Cemetery = 18,
            NoDungeon = 255,
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

            /// <summary>Latitude of this location.</summary>
            public Int32 Latitude;

            /// <summary>Longitude value read from bitfield.</summary>
            public Int32 Longitude;

            /// <summary>Location type value read from bitfield.</summary>
            public LocationTypes LocationType;

            /// <summary>Dungeon type (Crypt, Orc Stronghold, Human Stronghold, etc.).</summary>
            public DungeonTypes DungeonType;

            /// <summary>Whether or not this location is discovered.</summary>
            public Boolean Discovered;

            /// <summary>A key for the contents of the location.</summary>
            public UInt32 Key;
        }

        #endregion
    }
}
