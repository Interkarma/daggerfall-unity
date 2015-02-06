// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace DaggerfallConnect
{
    /// <summary>
    /// Stores information about locations, such as cities and dungeons.
    /// </summary>
    public struct DFLocation
    {
        #region Structure Variables

        /// <summary>
        /// Set to true if loaded.
        /// </summary>
        public bool Loaded;

        /// <summary>
        /// Name of this location.
        /// </summary>
        public string Name;

        /// <summary>
        /// Name of the parent region.
        /// </summary>
        public string RegionName;

        /// <summary>
        /// True if location has a dungeon, otherwise false.
        /// </summary>
        public bool HasDungeon;

        /// <summary>
        /// RegionMapTable data for this location.
        /// </summary>
        public DFRegion.RegionMapTable MapTableData;

        /// <summary>
        /// Exterior location.
        /// </summary>
        public LocationExterior Exterior;

        /// <summary>
        /// Dungeon attached to location (if HasDungeon = true).
        /// </summary>
        public LocationDungeon Dungeon;

        /// <summary>
        /// Climate settings for this location.
        /// </summary>
        public ClimateSettings Climate;

        /// <summary>
        /// Political alignment of this location (equal to region index + 128).
        /// </summary>
        public int Politic;

        /// <summary>
        /// Region index.
        /// </summary>
        public int RegionIndex;

        /// <summary>
        /// Location index.
        /// </summary>
        public int LocationIndex;

        #endregion

        #region Building Enumerations

        /// <summary>
        /// Building types within cities.
        /// </summary>
        public enum BuildingTypes
        {
            Alchemist,
            HouseForSale,
            Armorer,
            Bank,
            Town4,
            Bookseller,
            ClothingStore,
            FurnitureStore,
            GemStore,
            GeneralStore,
            Library,
            GuildHall,
            PawnShop,
            WeaponSmith,
            Temple,
            Tavern,
            Palace,
            House1,                 // Always has locked entry doors
            House2,                 // Has unlocked doors from 0600-1800
            House3,
            House4,
            House5,
            House6,
            Town23,
            Ship,                   // Never displayed on automap
            Special1 = 0x74,        // Special1-4 never displayed on automap
            Special2 = 0xdf,
            Special3 = 0xf9,
            Special4 = 0xfa,
        }

        #endregion

        #region Climate Enumerations

        /// <summary>
        /// Climate base type enumeration for climate-swapping textures.
        /// </summary>
        public enum ClimateBaseType
        {
            None = -1,
            Desert = 0,
            Mountain = 100,
            Temperate = 300,
            Swamp = 400,
        }

        /// <summary>
        /// Climate texture groups for identifying how texture is used.
        /// </summary>
        public enum ClimateTextureGroup
        {
            None,
            Nature,
            Terrain,
            Exterior,
            Interior,
        }

        /// <summary>
        /// Climate texture sets that can be cast to texture index for any climate.
        /// Includes enumerations for detail/misc textures with no climate swap.
        /// </summary>
        public enum ClimateTextureSet
        {
            // General sets
            None = -1,
            Misc = 46,

            // Terrain sets
            Exterior_Terrain = 2,

            // Exterior sets
            Exterior_Ruins = 7,
            Exterior_Ruins_2 = 8,
            Exterior_Castle = 9,
            Exterior_Castle_Snow = 10,
            Exterior_CityA = 12,
            Exterior_CityA_Snow = 13,
            Exterior_CityB = 14,
            Exterior_CityB_Snow = 15,
            Exterior_CityWalls = 17,
            Exterior_CityWalls_Snow = 18,
            Exterior_Farm = 26,
            Exterior_Farm_Snow = 27,
            Exterior_Fences = 29,
            Exterior_Fences_Snow = 30,
            Exterior_MagesGuild = 35,
            Exterior_MagesGuild_Snow = 36,
            Exterior_Manor = 38,
            Exterior_Manor_Snow = 39,
            Exterior_MerchantHomes = 42,
            Exterior_MerchantHomes_Snow = 43,
            Exterior_TavernExteriors = 58,
            Exterior_TavernExteriors_Snow = 59,
            Exterior_TempleExteriors = 61,
            Exterior_TempleExteriors_Snow = 62,
            Exterior_Village = 64,
            Exterior_Village_Snow = 65,
            Exterior_Roofs = 69,
            Exterior_Roofs_Snow = 70,
            Exterior_CitySpec = 79,
            Exterior_CitySpec_Snow = 80,
            Exterior_CitySpecB = 82,
            Exterior_CitySpecB_Snow = 83,

            // Exterior misc (no climate variant)
            Exterior_Hedges = 33,
            Exterior_MiscWoodPlanks = 67,
            Exterior_Doors = 74,
            Exterior_BaseTextures = 85,
            Exterior_Wagon = 88,
            Exterior_BarnInterior = 171,     // Used on underside of city roofs
            Exterior_Barn = 171,
            Exterior_Barn_Snow = 172,

            // Interior sets
            Interior_PalaceInt = 11,
            Interior_CityInt = 16,
            Interior_CryptA = 19,
            Interior_CryptB = 20,
            Interior_DungeonsA = 22,
            Interior_DungeonsB = 23,
            Interior_DungeonsC = 24,
            Interior_DungeonsNEWCs = 25,
            Interior_FarmInt = 28,
            Interior_MagesGuildInt = 37,
            Interior_ManorInt = 40,
            Interior_MarbleFloors = 41,
            Interior_MerchantHomesInt = 44,
            Interior_Mines = 45,
            Interior_Caves = 47,
            Interior_Paintings = 48,
            Interior_TavernInt = 60,
            Interior_TempleInt = 63,
            Interior_VillageInt = 66,
            Interior_Sewer = 68,
            Interior_MiscFurniture = 90,

            // Nature sets
            Nature_RainForest = 500,
            Nature_SubTropical = 501,
            Nature_Swamp = 502,
            Nature_Desert = 503,
            Nature_TemperateWoodland = 504,
            Nature_TemperateWoodland_Snow = 505,
            Nature_WoodlandHills = 506,
            Nature_WoodlandHills_Snow = 507,
            Nature_HauntedWoodlands = 508,
            Nature_HauntedWoodlands_Snow = 509,
            Nature_Mountains = 510,
            Nature_Mountains_Snow = 511,
        }

        /// <summary>
        /// Weather variations of climate sets.
        /// </summary>
        public enum ClimateWeather
        {
            /// <summary>Dry summer weather. Use with any climate set.</summary>
            Normal = 0,
            /// <summary>Buildings and ground in winter (only valid with exterior climate sets).</summary>
            Winter = 1,
            /// <summary>Ground wet from rain (only valid with ClimateSet.Terrain).</summary>
            Rain = 2,
        }

        #endregion

        #region Climate Structures

        /// <summary>
        /// Contains settings for various climate data.
        /// </summary>
        public struct ClimateSettings
        {
            /// <summary>World climate value from CLIMATE.PAK. Ranges from 223-232.</summary>
            public int WorldClimate;

            /// <summary>Base climate type.</summary>
            public ClimateBaseType ClimateType;

            /// <summary>Texture set for nature textures.</summary>
            public ClimateTextureSet NatureSet;

            /// <summary>Texture archive index for ground plane.</summary>
            public int GroundArchive;

            /// <summary>Texture archive index for ground scenery (rocks, trees, etc.).</summary>
            public int NatureArchive;

            /// <summary>Texture archive index for base sky set.</summary>
            public int SkyBase;
        }

        #endregion

        #region Shared Child Structures

        /// <summary>
        /// Initial data common to both dungeons and exterior locations.
        /// </summary>
        public struct LocationRecordElement
        {
            /// <summary>Number of doors in this location.</summary>
            public UInt32 DoorCount;

            /// <summary>Door data array with DoorCount members.</summary>
            public LocationDoorElement[] Doors;

            /// <summary>Header for location data.</summary>
            public LocationRecordElementHeader Header;
        }

        /// <summary>
        /// Describes a door element.
        /// </summary>
        public struct LocationDoorElement
        {
            /// <summary>Index to element within LocationExterior.BuildingData array.
            ///  Or 0xffff when door is not associated with BuildingData array.</summary>
            public UInt16 BuildingDataIndex;

            /// <summary>Always 0.</summary>
            public Byte NullValue;

            /// <summary>Unknown mask;</summary>
            public Byte Mask;

            /// <summary>Unknown.</summary>
            public Byte Unknown1;

            /// <summary>Unknown.</summary>
            public Byte Unknown2;
        }

        /// <summary>
        /// Header used for each location record element.
        /// </summary>
        public struct LocationRecordElementHeader
        {
            /// <summary>Always 1.</summary>
            public UInt32 AlwaysOne1;

            /// <summary>Always 0.</summary>
            public UInt16 NullValue1;

            /// <summary>Always 0.</summary>
            public Byte NullValue2;

            /// <summary>X coordinate for location in world units.</summary>
            public Int32 X;

            /// <summary>Always 0.</summary>
            public UInt32 NullValue3;

            /// <summary>X coordinate for location in world units.</summary>
            public Int32 Y;

            /// <summary>Set to 0x0000 for dungeon interior data, or 0x8000 for location exterior data.</summary>
            public UInt16 IsExterior;

            /// <summary>Always 0.</summary>
            public UInt16 NullValue4;

            /// <summary>Unknown.</summary>
            public UInt32 Unknown1;

            /// <summary>Unknown.</summary>
            public UInt32 Unknown2;

            /// <summary>Always 1.</summary>
            public UInt16 AlwaysOne2;

            /// <summary>LocationID used by the quest subsystem.</summary>
            public UInt16 LocationId;

            /// <summary>Always 0.</summary>
            public UInt32 NullValue5;

            /// <summary>Set to 0x0000 for exterior data, or 0x0001 for interior data.</summary>
            public UInt16 IsInterior;

            /// <summary>Set to 0x0000 when no exterior data present, or ID of exterior location</summary>
            public UInt32 ExteriorLocationId;

            /// <summary>Array of 26 0-value bytes.</summary>
            public Byte[] NullValue6;

            /// <summary>Name of location used when entering it.</summary>
            public String LocationName;

            /// <summary>Array of 9 unknown bytes.</summary>
            public Byte[] Unknown3;
        }

        #endregion

        #region Exterior Child Structures

        /// <summary>
        /// Describes exterior location.
        /// </summary>
        public struct LocationExterior
        {
            /// <summary>Each location is described by a LocationRecordElement.</summary>
            public LocationRecordElement RecordElement;

            /// <summary>The number of building structures present.</summary>
            public UInt16 BuildingCount;

            /// <summary>Unknown.</summary>
            public Byte[] Unknown1;

            /// <summary>Data associated with each building.</summary>
            public BuildingData[] Buildings;

            /// <summary>Exterior map data including layout information.</summary>
            public ExteriorData ExteriorData;
        }

        /// <summary>
        /// Data relating to a building.
        /// </summary>
        public struct BuildingData
        {
            /// <summary>Used to generate building name.</summary>
            public UInt16 NameSeed;

            /// <summary>Always 0.</summary>
            public UInt64 NullValue1;

            /// <summary>Always 0.</summary>
            public UInt64 NullValue2;

            /// <summary>FactionId associated with building, or 0 if no faction.</summary>
            public UInt16 FactionId;

            /// <summary>Generally increases with each building. Otherwise unknown.</summary>
            public Int16 Sector;

            /// <summary>Should always be the same as LocationRecordElementHeader.LocationId.</summary>
            public UInt16 LocationId;

            /// <summary>Type of building.</summary>
            public BuildingTypes BuildingType;

            /// <summary>Specifies quality of building from 1-20.</summary>
            public Byte Quality;
        }

        /// <summary>
        /// Layout data for exterior location.
        /// </summary>
        public struct ExteriorData
        {
            /// <summary>Another name for this location. Changing seems to have no effect in game.</summary>
            public String AnotherName;

            /// <summary>This (value and 0x000fffff) matches (MapTable.MapId and 0x000fffff).</summary>
            public Int32 MapId;

            /// <summary>Location ID.</summary>
            public UInt32 LocationId;

            /// <summary>Width of exterior map grid from 1-8.</summary>
            public Byte Width;

            /// <summary>Height of exterior map grid from 1-8.</summary>
            public Byte Height;

            /// <summary>Unknown.</summary>
            public Byte[] Unknown2;

            /// <summary>Only first Width*Height elements will have any meaning.</summary>
            public Byte[] BlockIndex;

            /// <summary>Only first Width*Height elements will have any meaning.</summary>
            public Byte[] BlockNumber;

            /// <summary>Only first Width*Height elements will have any meaning.</summary>
            public Byte[] BlockCharacter;
            
            /// <summary>Unknown.</summary>
            public Byte[] Unknown3;

            /// <summary>Always 0.</summary>
            public UInt64 NullValue1;

            /// <summary>Always 0.</summary>
            public Byte NullValue2;

            /// <summary>Unknown.</summary>
            public UInt32[] Unknown4;

            /// <summary>Always 0.</summary>
            public Byte[] NullValue3;

            /// <summary>Unknown.</summary>
            public UInt32 Unknown5;
        }

        #endregion

        #region Dungeon Child Structures

        /// <summary>
        /// Describes a dungeon map.
        /// </summary>
        public struct LocationDungeon
        {
            /// <summary>Each dungeon is described by a LocationRecordElement.</summary>
            public LocationRecordElement RecordElement;

            /// <summary>Header for dungeon layout.</summary>
            public DungeonHeader Header;

            /// <summary>Layout of dungeon.</summary>
            public DungeonBlock[] Blocks;
        }

        /// <summary>
        /// Header preceding dungeon layout elements.
        /// </summary>
        public struct DungeonHeader
        {
            /// <summary>Always 0.</summary>
            public UInt16 NullValue1;

            /// <summary>Unknown.</summary>
            public UInt32 Unknown1;

            /// <summary>Unknown.</summary>
            public UInt32 Unknown2;

            /// <summary>Count of DungeonBlock elements.</summary>
            public UInt16 BlockCount;

            /// <summary>Unknown.</summary>
            public Byte[] Unknown3;
        }

        /// <summary>
        /// Describes a dungeon block element in a dungeon map layout.
        /// </summary>
        public struct DungeonBlock
        {
            /// <summary>X position of block.</summary>
            public SByte X;
            
            /// <summary>Y position of block.</summary>
            public SByte Z;

            /// <summary>Bitfield containing BlockNumber, start bit, and BlockIndex.</summary>
            public UInt16 BlockNumberStartIndexBitfield;

            /// <summary>BlockNumber read from bitfield.</summary>
            public UInt16 BlockNumber;

            /// <summary>IsStartingBlock read from bitfield.</summary>
            public Boolean IsStartingBlock;

            /// <summary>BlockIndex read from bitfield.</summary>
            public Byte BlockIndex;

            /// <summary>Name of RDB block.</summary>
            public String BlockName;
        }

        #endregion
    }
}
