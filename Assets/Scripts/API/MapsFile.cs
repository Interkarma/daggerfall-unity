// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
using System.IO;
using System.Collections.Generic;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Utility.AssetInjection;
#endregion

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Connects to MAPS.BSA to enumerate locations within a specific region and extract their layouts.
    /// </summary>
    public class MapsFile
    {
        #region Class Variables

        public const int WorldMapTerrainDim = 32768;
        public const int WorldMapTileDim = 128;
        public const int WorldMapRMBDim = 4096;
        public const int MinWorldCoordX = 0;
        public const int MinWorldCoordZ = 0;
        public const int MaxWorldCoordX = 32768000;
        public const int MaxWorldCoordZ = 16384000;
        public const int MinWorldTileCoordX = 0;
        public const int MaxWorldTileCoordX = 128000;
        public const int MinWorldTileCoordZ = 0;
        public const int MaxWorldTileCoordZ = 64000;
        public const int MinMapPixelX = 0;
        public const int MinMapPixelY = 0;
        public const int MaxMapPixelX = 1000;
        public const int MaxMapPixelY = 500;

        /// <summary>
        /// All region names.
        /// </summary>
        private static readonly string[] regionNames = {
            "Alik'r Desert", "Dragontail Mountains", "Glenpoint Foothills", "Daggerfall Bluffs",
            "Yeorth Burrowland", "Dwynnen", "Ravennian Forest", "Devilrock",
            "Malekna Forest", "Isle of Balfiera", "Bantha", "Dak'fron",
            "Islands in the Western Iliac Bay", "Tamarilyn Point", "Lainlyn Cliffs", "Bjoulsae River",
            "Wrothgarian Mountains", "Daggerfall", "Glenpoint", "Betony", "Sentinel", "Anticlere", "Lainlyn", "Wayrest",
            "Gen Tem High Rock village", "Gen Rai Hammerfell village", "Orsinium Area", "Skeffington Wood",
            "Hammerfell bay coast", "Hammerfell sea coast", "High Rock bay coast", "High Rock sea coast",
            "Northmoor", "Menevia", "Alcaire", "Koegria", "Bhoriane", "Kambria", "Phrygias", "Urvaius",
            "Ykalon", "Daenia", "Shalgora", "Abibon-Gora", "Kairou", "Pothago", "Myrkwasa", "Ayasofya",
            "Tigonus", "Kozanset", "Satakalaam", "Totambu", "Mournoth", "Ephesus", "Santaki", "Antiphyllos",
            "Bergama", "Gavaudon", "Tulune", "Glenumbra Moors", "Ilessan Hills", "Cybiades"
        };

        /// <summary>
        /// All region races, primarily used to generate townsfolk names. In the array extracted from FALL.EXE:
        /// 0 = Breton, 1 = Redguard.
        /// </summary>
        private static readonly byte[] regionRaces = {
            1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0, 1,
            0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0,
            0, 1
        };

        /// <summary>
        /// Region temple faction IDs, extracted from FALL.EXE.
        /// </summary>
        private static readonly int[] regionTemples = {
            106,  82,   0,   0,   0,  98,   0,  0,   0,  92,
              0, 106,   0,   0,   0,  84,  36,  36,  84,  88,
             82,  88,  98,  92,   0,   0,  82,  0,   0,   0,
              0,   0,  88,  94,  36,  94, 106, 84, 106, 106,
             88,  98,  82,  98,  84,  94,  36, 88,  94,  36,
             98,  84, 106,  88, 106,  88,  92, 84,  98,  88,
             82,  94
        };

        /// <summary>
        /// Block file prefixes.
        /// </summary>
        private readonly string[] rmbBlockPrefixes = {
            "TVRN", "GENR", "RESI", "WEAP", "ARMR", "ALCH", "BANK", "BOOK",
            "CLOT", "FURN", "GEMS", "LIBR", "PAWN", "TEMP", "TEMP", "PALA",
            "FARM", "DUNG", "CAST", "MANR", "SHRI", "RUIN", "SHCK", "GRVE",
            "FILL", "KRAV", "KDRA", "KOWL", "KMOO", "KCAN", "KFLA", "KHOR",
            "KROS", "KWHE", "KSCA", "KHAW", "MAGE", "THIE", "DARK", "FIGH",
            "CUST", "WALL", "MARK", "SHIP", "WITC"
        };

        /// <summary>
        /// Letters that form the second part of an RMB block name.
        /// </summary>
        readonly string[] letter2Array = { "A", "L", "M", "S" };

        /// <summary>
        /// RDB block letters array.
        /// </summary>
        private readonly string[] rdbBlockLetters = { "N", "W", "L", "S", "B", "M" };

        /// <summary>
        /// Auto-discard behaviour enabled or disabled.
        /// </summary>
        private bool autoDiscardValue = true;

        /// <summary>
        /// The last region opened. Used by auto-discard logic.
        /// </summary>
        private int lastRegion = -1;

        /// <summary>
        /// The BsaFile representing MAPS.BSA.
        /// </summary>
        private readonly BsaFile bsaFile = new BsaFile();

        /// <summary>
        /// Array of decomposed region records.
        /// </summary>
        private RegionRecord[] regions;

        /// <summary>
        /// Climate PAK file.
        /// </summary>
        private PakFile climatePak;

        /// <summary>
        /// Politic PAK file.
        /// </summary>
        private PakFile politicPak;

        /// <summary>
        /// Flag set when class is loaded and ready.
        /// </summary>
        private bool isReady = false;

        #endregion

        #region Class Structures

        /// <summary>
        /// Represents a single region record.
        /// </summary>
        private struct RegionRecord
        {
            public string Name;
            public FileProxy MapNames;
            public FileProxy MapTable;
            public FileProxy MapPItem;
            public FileProxy MapDItem;
            public DFRegion DFRegion;
        }

        /// <summary>
        /// Offsets to dungeon records.
        /// </summary>
        private struct DungeonOffset
        {
            /// <summary>Offset in bytes relative to end of offset list.</summary>
            public UInt32 Offset;

            /// <summary>Is TRUE (0x0001) for all elements.</summary>
            public UInt16 IsDungeon;

            /// <summary>The exterior location this dungeon is paired with.</summary>
            public UInt16 ExteriorLocationId;
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets default MAPS.BSA filename.
        /// </summary>
        public static string Filename
        {
            get { return "MAPS.BSA"; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MapsFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to MAPS.BSA.</param>
        /// <param name="usage">Determines if the BSA file will read from disk or memory.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public MapsFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// If true then decomposed regions will be destroyed every time a different region is fetched.
        ///  If false then decomposed regions will be maintained until DiscardRegion() or DiscardAllRegions() is called.
        ///  Turning off auto-discard will speed up region retrieval times at the expense of RAM. For best results, disable
        ///  auto-discard and impose your own caching scheme using LoadRecord() and DiscardRecord() based on your application
        ///  needs.
        /// </summary>
        public bool AutoDiscard
        {
            get { return autoDiscardValue; }
            set { autoDiscardValue = value; }
        }

        /// <summary>
        /// Number of regions in MAPS.BSA.
        /// </summary>
        public int RegionCount
        {
            get { return bsaFile.Count / 4; }
        }

        /// <summary>
        /// Gets all region names as string array.
        /// </summary>
        public static string[] RegionNames
        {
            get { return regionNames; }
        }

        /// <summary>
        /// Gets all region races as byte array.
        /// </summary>
        public static byte[] RegionRaces
        {
            get { return regionRaces; }
        }

        /// <summary>
        /// Gets region temple faction IDs.
        /// </summary>
        public static int[] RegionTemples {
            get { return regionTemples; }
        }

        /// <summary>
        /// Gets raw MAPS.BSA file.
        /// </summary>
        public BsaFile BsaFile
        {
            get { return bsaFile; }
        }

        /// <summary>
        /// True when ready to load regions and locations, otherwise false.
        /// </summary>
        public bool Ready
        {
            get { return isReady; }
        }

        /// <summary>
        /// Gets internal climate data.
        /// </summary>
        public PakFile ClimateFile
        {
            get { return climatePak; }
        }

        #endregion

        #region Static Properties

        public enum Climates
        {
            Ocean = 223,
            Desert = 224,
            Desert2 = 225, // seen in dak'fron
            Mountain = 226,
            Rainforest = 227,
            Swamp = 228,
            Subtropical = 229,
            MountainWoods = 230,
            Woodlands = 231,
            HauntedWoodlands = 232 // not sure where this is?
        }

        /// <summary>
        /// Gets default climate index.
        /// </summary>
        public static int DefaultClimate
        {
            get { return 231; }
        }

        /// <summary>
        /// Gets default climate settings.
        /// </summary>
        public static DFLocation.ClimateSettings DefaultClimateSettings
        {
            get { return GetWorldClimateSettings(DefaultClimate); }
        }

        #endregion

        #region Static Public Methods

        /// <summary>
        /// Converts longitude and latitude to map pixel coordinates.
        /// The world is 1000x500 map pixels.
        /// </summary>
        /// <param name="longitude">Longitude position.</param>
        /// <param name="latitude">Latitude position.</param>
        /// <returns>Map pixel position.</returns>
        public static DFPosition LongitudeLatitudeToMapPixel(int longitude, int latitude)
        {
            DFPosition pos = new DFPosition();
            pos.X = longitude / 128;
            pos.Y = 499 - (latitude / 128);

            return pos;
        }

        /// <summary>
        /// Converts map pixel coord to longitude and latitude.
        /// </summary>
        /// <param name="mapPixelX">Map pixel X.</param>
        /// <param name="mapPixelY">Map pixel Y.</param>
        /// <returns></returns>
        public static DFPosition MapPixelToLongitudeLatitude(int mapPixelX, int mapPixelY)
        {
            DFPosition pos = new DFPosition();
            pos.X = mapPixelX * 128;
            pos.Y = (499 - mapPixelY) * 128;

            return pos;
        }

        /// <summary>
        /// Gets ID of map pixel.
        /// This can be mapped to location IDs and quest IDs.
        /// MapTableData.MapId &amp; 0x000fffff = WorldPixelID.
        /// </summary>
        /// <param name="mapPixelX">Map pixel X.</param>
        /// <param name="mapPixelY">Map pixel Y.</param>
        /// <returns>Map pixel ID.</returns>
        public static int GetMapPixelID(int mapPixelX, int mapPixelY)
        {
            return mapPixelY * 1000 + mapPixelX;
        }

        public static DFPosition GetPixelFromPixelID(int pixelID)
        {
            int x = pixelID % 1000;
            int y = (pixelID - x) / 1000;
            return new DFPosition(x, y);
        }

        /// <summary>
        /// Gets ID of map pixel using latitude and longitude.
        /// This can be mapped to location IDs and quest IDs.
        /// MapTableData.MapId &amp; 0x000fffff = WorldPixelID.
        /// </summary>
        /// <param name="longitude">Longitude position.</param>
        /// <param name="latitude">Latitude position.</param>
        /// <returns>Map pixel ID.</returns>
        public static int GetMapPixelIDFromLongitudeLatitude(int longitude, int latitude)
        {
            DFPosition pos = LongitudeLatitudeToMapPixel(longitude, latitude);

            return pos.Y * 1000 + pos.X;
        }

        /// <summary>
        /// Converts map pixel to world coord.
        /// </summary>
        /// <param name="mapPixelX">Map pixel X.</param>
        /// <param name="mapPixelY">Map pixel Y.</param>
        /// <returns>World position.</returns>
        public static DFPosition MapPixelToWorldCoord(int mapPixelX, int mapPixelY)
        {
            DFPosition pos = new DFPosition();
            pos.X = mapPixelX * 32768;
            pos.Y = (499 - mapPixelY) * 32768;

            return pos;
        }

        /// <summary>
        /// Converts world coord to nearest map pixel.
        /// </summary>
        /// <param name="worldX">World X position in native Daggerfall units.</param>
        /// <param name="worldZ">World Z position in native Daggerfall units.</param>
        /// <returns>Map pixel position.</returns>
        public static DFPosition WorldCoordToMapPixel(int worldX, int worldZ)
        {
            DFPosition pos = new DFPosition();
            pos.X = worldX / 32768;
            pos.Y = 499 - (worldZ / 32768);

            return pos;
        }

        /// <summary>
        /// Converts world coord back to longitude and latitude.
        /// </summary>
        /// <param name="worldX">World X position in native Daggerfall units.</param>
        /// <param name="worldZ">World Z position in native Daggerfall units.</param>
        /// <returns>Longitude and latitude.</returns>
        public static DFPosition WorldCoordToLongitudeLatitude(int worldX, int worldZ)
        {
            DFPosition mapPixel = WorldCoordToMapPixel(worldX, worldZ);
            DFPosition pos = new DFPosition();
            pos.X = mapPixel.X * 128;
            pos.Y = (499 - mapPixel.Y) * 128;

            return pos;
        }

        /// <summary>
        /// Gets settings for specified map climate.
        /// </summary>
        /// <param name="worldClimate">Climate value from CLIMATE.PAK. Valid range is 223-232.</param>
        /// <returns>Climate settings for specified world climate value.</returns>
        public static DFLocation.ClimateSettings GetWorldClimateSettings(int worldClimate)
        {
            // Create settings struct
            DFLocation.ClimateSettings settings = new DFLocation.ClimateSettings();
            settings.WorldClimate = worldClimate;

            // Set based on world climate
            switch (worldClimate)
            {
                case (int)Climates.Ocean:   // Ocean
                    settings.ClimateType = DFLocation.ClimateBaseType.Swamp;
                    settings.GroundArchive = 402;
                    settings.NatureArchive = (int)DFLocation.ClimateTextureSet.Nature_TemperateWoodland;
                    settings.SkyBase = 24;
                    settings.People = FactionFile.FactionRaces.Breton;
                    break;
                case (int)Climates.Desert:
                case (int)Climates.Desert2:
                    settings.ClimateType = DFLocation.ClimateBaseType.Desert;
                    settings.GroundArchive = 2;
                    settings.NatureArchive = (int)DFLocation.ClimateTextureSet.Nature_Desert;
                    settings.SkyBase = 8;
                    settings.People = FactionFile.FactionRaces.Redguard;
                    break;
                case (int)Climates.Mountain:
                    settings.ClimateType = DFLocation.ClimateBaseType.Mountain;
                    settings.GroundArchive = 102;
                    settings.NatureArchive = (int)DFLocation.ClimateTextureSet.Nature_Mountains;
                    settings.SkyBase = 0;
                    settings.People = FactionFile.FactionRaces.Nord;
                    break;
                case (int)Climates.Rainforest:
                    settings.ClimateType = DFLocation.ClimateBaseType.Swamp;
                    settings.GroundArchive = 402;
                    settings.NatureArchive = (int)DFLocation.ClimateTextureSet.Nature_RainForest;
                    settings.SkyBase = 24;
                    settings.People = FactionFile.FactionRaces.Redguard;
                    break;
                case (int)Climates.Swamp:
                    settings.ClimateType = DFLocation.ClimateBaseType.Swamp;
                    settings.GroundArchive = 402;
                    settings.NatureArchive = (int)DFLocation.ClimateTextureSet.Nature_Swamp;
                    settings.SkyBase = 24;
                    settings.People = FactionFile.FactionRaces.Breton;
                    break;
                case (int)Climates.Subtropical:
                    settings.ClimateType = DFLocation.ClimateBaseType.Desert;
                    settings.GroundArchive = 2;
                    settings.NatureArchive = (int)DFLocation.ClimateTextureSet.Nature_SubTropical;
                    settings.SkyBase = 24;
                    settings.People = FactionFile.FactionRaces.Breton;
                    break;
                case (int)Climates.MountainWoods:
                    settings.ClimateType = DFLocation.ClimateBaseType.Temperate;
                    settings.GroundArchive = 102;
                    settings.NatureArchive = (int)DFLocation.ClimateTextureSet.Nature_TemperateWoodland;
                    settings.SkyBase = 16;
                    settings.People = FactionFile.FactionRaces.Breton;
                    break;
                case (int)Climates.Woodlands:
                    settings.ClimateType = DFLocation.ClimateBaseType.Temperate;
                    settings.GroundArchive = 302;
                    settings.NatureArchive = (int)DFLocation.ClimateTextureSet.Nature_TemperateWoodland;
                    settings.SkyBase = 16;
                    settings.People = FactionFile.FactionRaces.Breton;
                    break;
                case (int)Climates.HauntedWoodlands:
                    settings.ClimateType = DFLocation.ClimateBaseType.Temperate;
                    settings.GroundArchive = 302;
                    settings.NatureArchive = (int)DFLocation.ClimateTextureSet.Nature_HauntedWoodlands;
                    settings.SkyBase = 16;
                    settings.People = FactionFile.FactionRaces.Breton;
                    break;
                default:
                    settings.ClimateType = DFLocation.ClimateBaseType.Temperate;
                    settings.GroundArchive = 302;
                    settings.NatureArchive = (int)DFLocation.ClimateTextureSet.Nature_TemperateWoodland;
                    settings.SkyBase = 16;
                    settings.People = FactionFile.FactionRaces.Breton;
                    break;
            }

            // Set nature set from nature archive index
            settings.NatureSet = (DFLocation.ClimateTextureSet)settings.NatureArchive;

            return settings;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load MAPS.BSA file.
        /// </summary>
        /// <param name="filePath">Absolute path to MAPS.BSA file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Validate filename
            if (!filePath.EndsWith("MAPS.BSA", StringComparison.InvariantCultureIgnoreCase))
                return false;

            // Load PAK files
            string arena2Path = Path.GetDirectoryName(filePath);
            climatePak = new PakFile(Path.Combine(arena2Path, "CLIMATE.PAK"));
            politicPak = new PakFile(Path.Combine(arena2Path, "POLITIC.PAK"));

            // Load file
            isReady = false;
            if (!bsaFile.Load(filePath, usage, readOnly))
                return false;

            // Create records array
            regions = new RegionRecord[RegionCount];

            // Set ready flag
            isReady = true;

            return true;
        }

        /// <summary>
        /// Gets the name of specified region. Does not change the currently loaded region.
        /// </summary>
        /// <param name="region">Index of region.</param>
        /// <returns>Name of the region.</returns>
        public string GetRegionName(int region)
        {
            if (region < 0 || region >= RegionCount)
                return string.Empty;
            else
                return regionNames[region];
        }

        /// <summary>
        /// Gets index of region with specified name. Does not change the currently loaded region.
        /// </summary>
        /// <param name="name">Name of region.</param>
        /// <returns>Index of found region, or -1 if not found.</returns>
        public int GetRegionIndex(string name)
        {
            // Search for region name
            for (int i = 0; i < RegionCount; i++)
            {
                if (regionNames[i] == name)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Load a region into memory by name and decompose it for use.
        /// </summary>
        /// <param name="name">Name of region.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool LoadRegion(string name)
        {
            return LoadRegion(GetRegionIndex(name));
        }

        /// <summary>
        /// Load a region into memory by index and decompose it for use.
        /// </summary>
        /// <param name="region">Index of region to load.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool LoadRegion(int region)
        {
            // Validate
            if (region < 0 || region >= RegionCount)
                return false;

            // Auto discard previous record
            if (autoDiscardValue && lastRegion != -1)
                DiscardRegion(lastRegion);

            // Load record data
            int record = region * 4;
            regions[region].MapPItem = bsaFile.GetRecordProxy(record++);
            regions[region].MapDItem = bsaFile.GetRecordProxy(record++);
            regions[region].MapTable = bsaFile.GetRecordProxy(record++);
            regions[region].MapNames = bsaFile.GetRecordProxy(record);
            if (regions[region].MapNames.Length == 0 ||
                regions[region].MapTable.Length == 0 ||
                regions[region].MapPItem.Length == 0 ||
                regions[region].MapDItem.Length == 0)
                return false;

            // Set region name
            regions[region].Name = regionNames[region];

            // Read region
            if (!ReadRegion(region))
            {
                DiscardRegion(region);
                return false;
            }

            // Set previous record
            lastRegion = region;

            return true;
        }

        /// <summary>
        /// Discard a region from memory.
        /// </summary>
        /// <param name="region">Index of region to discard.</param>
        public void DiscardRegion(int region)
        {
            // Validate
            if (region >= RegionCount)
                return;

            // Discard memory files and other data
            regions[region].Name = string.Empty;
            regions[region].MapNames = null;
            regions[region].MapTable = null;
            regions[region].MapPItem = null;
            regions[region].MapDItem = null;
            regions[region].DFRegion = new DFRegion();
        }

        /// <summary>
        /// Discard all regions.
        /// </summary>
        public void DiscardAllRegions()
        {
            for (int index = 0; index < RegionCount; index++)
            {
                DiscardRegion(index);
            }
        }

        /// <summary>
        /// Gets a DFRegion by index.
        /// </summary>
        /// <param name="region">Index of region.</param>
        /// <returns>DFRegion.</returns>
        public DFRegion GetRegion(int region)
        {
            // Load the region
            if (!LoadRegion(region))
                return new DFRegion();

            return regions[region].DFRegion;
        }

        /// <summary>
        /// Gets a DFRegion by name.
        /// </summary>
        /// <param name="name">Name of region.</param>
        /// <returns>DFRegion.</returns>
        public DFRegion GetRegion(string name)
        {
            int Region = GetRegionIndex(name);
            if (-1 == Region)
                return new DFRegion();

            return GetRegion(Region);
        }

        /// <summary>
        /// Gets a DFLocation representation of a location.
        /// </summary>
        /// <param name="region">Index of region.</param>
        /// <param name="location">Index of location.</param>
        /// <returns>DFLocation.</returns>
        public DFLocation GetLocation(int region, int location)
        {
            // Load the region
            if (!LoadRegion(region))
                return new DFLocation();

            // Read location
            DFLocation dfLocation = new DFLocation();
            if (!ReadLocation(region, location, ref dfLocation))
                return new DFLocation();

            // Store indices
            dfLocation.RegionIndex = region;
            dfLocation.LocationIndex = location;

            // Generate smaller dungeon when possible
            if (UseSmallerDungeon(ref dfLocation))
                GenerateSmallerDungeon(ref dfLocation);

            return dfLocation;
        }

        private bool UseSmallerDungeon(ref DFLocation dfLocation)
        {
            // Do nothing if location has no dungeon or if a main story dungeon - these are never made smaller
            if (!dfLocation.HasDungeon || DaggerfallDungeon.IsMainStoryDungeon(dfLocation.MapTableData.MapId))
                return false;

            // Collect any SiteLinks associated with this dungeon - there should only be one quest assignment per dungeon
            SiteLink[] siteLinks = QuestMachine.Instance.GetSiteLinks(SiteTypes.Dungeon, dfLocation.MapTableData.MapId);
            if (siteLinks != null && siteLinks.Length > 0)
            {
                // If a SiteLink and related Quest is found then use whatever SmallerDungeons setting as configured at time quest started
                // Marker assignments are not relocated when user changes SmallerDungeons state so we always use whatever quest compiled with
                Quest quest = QuestMachine.Instance.GetQuest(siteLinks[0].questUID);
                if (quest != null && quest.SmallerDungeonsState == QuestSmallerDungeonsState.Enabled)
                    return true;
                else if (quest != null && quest.SmallerDungeonsState == QuestSmallerDungeonsState.Disabled)
                    return false;
            }

            // If not a main story dungeon and no quest found in dungeon then just use setting as configured
            return DaggerfallUnity.Settings.SmallerDungeons;
        }

        /// <summary>
        /// Gets DFLocation representation of a location.
        /// </summary>
        /// <param name="regionName">Name of region.</param>
        /// <param name="locationName">Name of location.</param>
        /// <returns>DFLocation.</returns>
        public DFLocation GetLocation(string regionName, string locationName)
        {
            // Load region
            int Region = GetRegionIndex(regionName);
            if (!LoadRegion(Region))
                return new DFLocation();

            // Check location exists
            if (!regions[Region].DFRegion.MapNameLookup.ContainsKey(locationName))
                return new DFLocation();

            // Get location index
            int Location = regions[Region].DFRegion.MapNameLookup[locationName];

            return GetLocation(Region, Location);
        }

        /// <summary>
        /// Lookup block name for exterior block from location data provided.
        /// </summary>
        /// <param name="dfLocation">DFLocation to read block name.</param>
        /// <param name="x">Block X coordinate.</param>
        /// <param name="y">Block Y coordinate.</param>
        /// <returns>Block name.</returns>
        public string GetRmbBlockName(ref DFLocation dfLocation, int x, int y)
        {
            int index = y * dfLocation.Exterior.ExteriorData.Width + x;
            return dfLocation.Exterior.ExteriorData.BlockNames[index];
        }

        /// <summary>
        /// Resolve block name for exterior block from X, Y coordinates.
        /// </summary>
        /// <param name="dfLocation">DFLocation to resolve block name.</param>
        /// <param name="x">Block X coordinate.</param>
        /// <param name="y">Block Y coordinate.</param>
        /// <returns>Block name.</returns>
        public string ResolveRmbBlockName(ref DFLocation dfLocation, int x, int y)
        {
            // Get indices
            int offset = y * dfLocation.Exterior.ExteriorData.Width + x;
            byte blockIndex = dfLocation.Exterior.ExteriorData.BlockIndex[offset];
            byte blockNumber = dfLocation.Exterior.ExteriorData.BlockNumber[offset];
            byte blockCharacter = dfLocation.Exterior.ExteriorData.BlockCharacter[offset];

            return ResolveRmbBlockName(ref dfLocation, blockIndex, blockNumber, blockCharacter);
        }

        /// <summary>
        /// Resolve block name from raw components.
        /// </summary>
        /// <param name="dfLocation">DFLocation to resolve block name.</param>
        /// <param name="blockIndex">Block index.</param>
        /// <param name="blockNumber">Block number.</param>
        /// <param name="blockCharacter">Block character.</param>
        /// <returns>Block name.</returns>
        public string ResolveRmbBlockName(ref DFLocation dfLocation, byte blockIndex, byte blockNumber, byte blockCharacter)
        {
            string letter1 = string.Empty;
            string letter2 = string.Empty;
            string numbers = string.Empty;

            // Get prefix
            string prefix = rmbBlockPrefixes[blockIndex];

            // Get letter 1
            if ((blockCharacter & 0x10) != 0)
            {
                int asciiValue = dfLocation.Exterior.ExteriorData.Letter1ForRMBName;
                letter1 = char.ConvertFromUtf32(asciiValue);
            }
            else
                letter1 = "A";

            // Get letter 2
            letter2 = letter2Array[(byte)(2 * blockCharacter) >> 6];

            // Get block number as a string
            string blockNumberString = blockNumber.ToString();

            if (blockIndex == 13 || blockIndex == 14)
            {
                // Handle temple numbers
                int asciivalue = (blockCharacter & 0xF) + 65;
                numbers = char.ConvertFromUtf32(asciivalue) + blockNumberString;
            }
            else
            {
                // Numbers are uniform in non-temple blocks
                numbers = string.Format("{0:00}", blockNumber);
            }

            return prefix + letter1 + letter2 + numbers + ".RMB";
        }

        /// <summary>
        /// Reads climate index from CLIMATE.PAK based on world pixel.
        /// </summary>
        /// <param name="mapPixelX">Map pixel X position.</param>
        /// <param name="mapPixelY">Map pixel Y position.</param>
        public int GetClimateIndex(int mapPixelX, int mapPixelY)
        {
            // Add +1 to X coordinate to line up with height map
            mapPixelX += 1;

            return climatePak.GetValue(mapPixelX, mapPixelY);
        }

        /// <summary>
        /// Reads politic index from POLITIC.PAK based on world pixel.
        /// </summary>
        /// <param name="mapPixelX">Map pixel X position.</param>
        /// <param name="mapPixelY">Map pixel Y position.</param>
        public int GetPoliticIndex(int mapPixelX, int mapPixelY)
        {
            mapPixelX += 1;
            return politicPak.GetValue(mapPixelX, mapPixelY);
        }

        /// <summary>
        /// Sets climate index from CLIMATE.PAK based on world pixel.
        /// Allows loaded climate data from Pak file to be modified by mods.
        /// </summary>
        /// <param name="mapPixelX">Map pixel X position.</param>
        /// <param name="mapPixelY">Map pixel Y position.</param>
        /// <param name="value">The climate to set for the specified map pixel.</param>
        /// <returns>True if climate index was set, false otherwise.</returns>
        public bool SetClimateIndex(int mapPixelX, int mapPixelY, Climates value)
        {
            mapPixelX += 1;
            return climatePak.SetValue(mapPixelX, mapPixelY, (byte)value);
        }

        /// <summary>
        /// Reads politic index from POLITIC.PAK based on world pixel.
        /// Allows loaded region data from Pak file to be modified by mods.
        /// </summary>
        /// <param name="mapPixelX">Map pixel X position.</param>
        /// <param name="mapPixelY">Map pixel Y position.</param>
        /// <param name="value">The politic index to set for the specified map pixel.</param>
        /// <returns>True if politic index was set, false otherwise.</returns>
        public bool SetPoliticIndex(int mapPixelX, int mapPixelY, byte value)
        {
            mapPixelX += 1;
            return politicPak.SetValue(mapPixelX, mapPixelY, value);
        }

        #endregion

        #region Readers

        /// <summary>
        /// Read a region.
        /// </summary>
        /// <param name="region">The region index to read.</param>
        /// <returns>True if successful, otherwise false.</returns>
        private bool ReadRegion(int region)
        {
            try
            {
                // Store region name
                regions[region].DFRegion.Name = regionNames[region];

                // Read map names
                BinaryReader reader = regions[region].MapNames.GetReader();
                ReadMapNames(ref reader, region);

                // Read map table
                reader = regions[region].MapTable.GetReader();
                ReadMapTable(ref reader, region);
            }
            catch (Exception e)
            {
                DiscardRegion(region);
                Console.WriteLine(e.Message);
                return false;
            }

            // Add any additional replacement location data to the region, assigning locationIndex
            WorldDataReplacement.GetDFRegionAdditionalLocationData(region, ref regions[region].DFRegion);

            return true;
        }

        /// <summary>
        /// Read a location from the currently loaded region.
        /// </summary>
        /// <param name="region">Region index.</param>
        /// <param name="location">Location index.</param>
        /// <param name="dfLocation">DFLocation object to receive data.</param>
        /// <returns>True if successful, otherwise false.</returns>
        private bool ReadLocation(int region, int location, ref DFLocation dfLocation)
        {
            // Check for replacement location data and use it if found
            if (WorldDataReplacement.GetDFLocationReplacementData(region, location, out dfLocation))
                return true;

            try
            {
                // Store parent region name
                dfLocation.RegionName = RegionNames[region];

                // Read MapPItem for this location
                BinaryReader reader = regions[region].MapPItem.GetReader();
                ReadMapPItem(ref reader, region, location, ref dfLocation);

                // Read MapDItem for this location
                reader = regions[region].MapDItem.GetReader();
                ReadMapDItem(ref reader, region, ref dfLocation);

                // Copy RegionMapTable data to this location
                dfLocation.MapTableData = regions[region].DFRegion.MapTable[location];

                // Read climate and politic data
                ReadClimatePoliticData(ref dfLocation);

                // Set loaded flag
                dfLocation.Loaded = true;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Reads information from CLIMATE.PAK and POLITIC.PAK.
        /// </summary>
        /// <param name="dfLocation">DFLocation.</param>
        private void ReadClimatePoliticData(ref DFLocation dfLocation)
        {
            DFPosition pos = LongitudeLatitudeToMapPixel(dfLocation.MapTableData.Longitude, dfLocation.MapTableData.Latitude);

            // Read politic data. This should always equal region index + 128.
            dfLocation.Politic = politicPak.GetValue(pos.X, pos.Y);

            // Read climate data
            int worldClimate = climatePak.GetValue(pos.X, pos.Y);
            dfLocation.Climate = MapsFile.GetWorldClimateSettings(worldClimate);
        }

        /// <summary>
        /// Read map names.
        /// </summary>
        /// <param name="reader">A binary reader to data.</param>
        /// <param name="region">Destination region index.</param>
        private void ReadMapNames(ref BinaryReader reader, int region)
        {
            // Location count
            reader.BaseStream.Position = 0;
            regions[region].DFRegion.LocationCount = reader.ReadUInt32();

            // Read names
            regions[region].DFRegion.MapNames = new String[regions[region].DFRegion.LocationCount];
            regions[region].DFRegion.MapNameLookup = new System.Collections.Generic.Dictionary<string, int>();
            for (int i = 0; i < regions[region].DFRegion.LocationCount; i++)
            {
                // Read map name data
                regions[region].DFRegion.MapNames[i] = FileProxy.ReadCStringSkip(reader, 0, 32);

                // Add to dictionary
                if (!regions[region].DFRegion.MapNameLookup.ContainsKey(regions[region].DFRegion.MapNames[i]))
                    regions[region].DFRegion.MapNameLookup.Add(regions[region].DFRegion.MapNames[i], i);
            }
        }

        /// <summary>
        /// Read map table.
        /// </summary>
        /// <param name="reader">A binary reader to data.</param>
        /// <param name="region">Destination region index.</param>
        private void ReadMapTable(ref BinaryReader reader, int region)
        {
            // Read map table for each location
            UInt32 bitfield;
            reader.BaseStream.Position = 0;
            regions[region].DFRegion.MapTable = new DFRegion.RegionMapTable[regions[region].DFRegion.LocationCount];
            regions[region].DFRegion.MapIdLookup = new System.Collections.Generic.Dictionary<int, int>();
            for (int i = 0; i < regions[region].DFRegion.LocationCount; i++)
            {
                // Read map table data
                regions[region].DFRegion.MapTable[i].MapId = reader.ReadInt32();
                bitfield = reader.ReadUInt32();
                regions[region].DFRegion.MapTable[i].Longitude = (int)(bitfield & 0x1FFFFFF) >> 8;
                regions[region].DFRegion.MapTable[i].LocationType = (DFRegion.LocationTypes)((4 * bitfield) >> 27);
                regions[region].DFRegion.MapTable[i].Discovered = ((bitfield >> 24) & 0x40) != 0;
                regions[region].DFRegion.MapTable[i].Latitude = (reader.ReadInt32() & 0xFFFFFF) >> 8;
                regions[region].DFRegion.MapTable[i].DungeonType = (DFRegion.DungeonTypes)reader.ReadByte();
                regions[region].DFRegion.MapTable[i].Key = reader.ReadUInt32();

                // Add to dictionary
                if (!regions[region].DFRegion.MapIdLookup.ContainsKey(regions[region].DFRegion.MapTable[i].MapId))
                    regions[region].DFRegion.MapIdLookup.Add(regions[region].DFRegion.MapTable[i].MapId, i);
            }
        }

        /// <summary>Read location count from maps file, used to get count in
        /// classic datafiles only, excluding added locations. </summary>
        private uint ReadLocationCount(int region)
        {
            BinaryReader reader = regions[region].MapNames.GetReader();
            reader.BaseStream.Position = 0;
            return reader.ReadUInt32();
        }

        /// <summary>
        /// Quickly reads the LocationId with minimal overhead.
        /// Region must be loaded before calling this method.
        /// </summary>
        /// <param name="region">Region index.</param>
        /// <param name="location">Location index.</param>
        /// <returns>LocationId.</returns>
        public int ReadLocationIdFast(int region, int location)
        {
            // Added new locations will put the LocationId in regions map table, since it doesn't exist in classic data
            if (regions[region].DFRegion.MapTable[location].LocationId != 0)
                return regions[region].DFRegion.MapTable[location].LocationId;

            // Get datafile location count (excluding added locations)
            uint locationCount = ReadLocationCount(region);

            // Get reader
            BinaryReader reader = regions[region].MapPItem.GetReader();

            // Position reader at location record by reading offset and adding to end of offset table
            reader.BaseStream.Position = location * 4;
            reader.BaseStream.Position = (locationCount * 4) + reader.ReadUInt32();

            // Skip doors (+6 bytes per door)
            UInt32 doorCount = reader.ReadUInt32();
            reader.BaseStream.Position += doorCount * 6;

            // Skip to LocationId (+33 bytes)
            reader.BaseStream.Position += 33;

            // Read the LocationId
            int locationId = reader.ReadUInt16();

            return locationId;
        }

        /// <summary>
        /// Reads MapPItem data.
        /// </summary>
        /// <param name="reader">A binary reader to data.</param>
        /// <param name="region">Region index.</param>
        /// <param name="location">Location Index.</param>
        /// <param name="dfLocation">Destination DFLocation.</param>
        private void ReadMapPItem(ref BinaryReader reader, int region, int location, ref DFLocation dfLocation)
        {
            // Get datafile location count (excluding added locations)
            uint locationCount = ReadLocationCount(region);

            // Position reader at location record by reading offset and adding to end of offset table
            reader.BaseStream.Position = location * 4;
            reader.BaseStream.Position = (locationCount * 4) + reader.ReadUInt32();

            // Store name
            dfLocation.Name = regions[region].DFRegion.MapNames[location];

            // Read LocationRecordElement
            ReadLocationRecordElement(ref reader, region, ref dfLocation.Exterior.RecordElement);

            // Read BuildingListHeader
            dfLocation.Exterior.BuildingCount = reader.ReadUInt16();
            dfLocation.Exterior.Unknown1 = new Byte[5];
            for (int i = 0; i < 5; i++) dfLocation.Exterior.Unknown1[i] = reader.ReadByte();

            // Read BuildingData
            dfLocation.Exterior.Buildings = new DFLocation.BuildingData[dfLocation.Exterior.BuildingCount];
            for (int building = 0; building < dfLocation.Exterior.BuildingCount; building++)
            {
                dfLocation.Exterior.Buildings[building].NameSeed = reader.ReadUInt16();
                dfLocation.Exterior.Buildings[building].ServiceTimeLimit = reader.ReadUInt32();
                dfLocation.Exterior.Buildings[building].Unknown = reader.ReadUInt16();
                dfLocation.Exterior.Buildings[building].Unknown2 = reader.ReadUInt16();
                dfLocation.Exterior.Buildings[building].Unknown3 = reader.ReadUInt32();
                dfLocation.Exterior.Buildings[building].Unknown4 = reader.ReadUInt32();
                dfLocation.Exterior.Buildings[building].FactionId = reader.ReadUInt16();
                dfLocation.Exterior.Buildings[building].Sector = reader.ReadInt16();
                dfLocation.Exterior.Buildings[building].LocationId = reader.ReadUInt16();
                dfLocation.Exterior.Buildings[building].BuildingType = (DFLocation.BuildingTypes)reader.ReadByte();
                dfLocation.Exterior.Buildings[building].Quality = reader.ReadByte();
            }

            // Read ExteriorData
            dfLocation.Exterior.ExteriorData.AnotherName = FileProxy.ReadCStringSkip(reader, 0, 32);
            dfLocation.Exterior.ExteriorData.MapId = reader.ReadInt32();
            dfLocation.Exterior.ExteriorData.LocationId = reader.ReadUInt32();
            dfLocation.Exterior.ExteriorData.Width = reader.ReadByte();
            dfLocation.Exterior.ExteriorData.Height = reader.ReadByte();
            dfLocation.Exterior.ExteriorData.Unknown2 = reader.ReadBytes(4);
            dfLocation.Exterior.ExteriorData.Letter1ForRMBName = reader.ReadByte();
            dfLocation.Exterior.ExteriorData.PortTownAndUnknown = reader.ReadByte();
            dfLocation.Exterior.ExteriorData.Unknown3 = reader.ReadByte();
            dfLocation.Exterior.ExteriorData.BlockIndex = reader.ReadBytes(64);
            dfLocation.Exterior.ExteriorData.BlockNumber = reader.ReadBytes(64);
            dfLocation.Exterior.ExteriorData.BlockCharacter = reader.ReadBytes(64);
            dfLocation.Exterior.ExteriorData.Unknown4 = reader.ReadBytes(34);
            dfLocation.Exterior.ExteriorData.NullValue1 = reader.ReadUInt64();
            dfLocation.Exterior.ExteriorData.NullValue2 = reader.ReadByte();
            dfLocation.Exterior.ExteriorData.Unknown5 = new UInt32[22];
            for (int i = 0; i < 22; i++) dfLocation.Exterior.ExteriorData.Unknown5[i] = reader.ReadUInt32();
            dfLocation.Exterior.ExteriorData.NullValue3 = reader.ReadBytes(40);
            dfLocation.Exterior.ExteriorData.Unknown6 = reader.ReadUInt32();

            // Get block names
            int totalBlocks = dfLocation.Exterior.ExteriorData.Width * dfLocation.Exterior.ExteriorData.Height;
            dfLocation.Exterior.ExteriorData.BlockNames = new string[totalBlocks];
            for (int i = 0; i < totalBlocks; i++)
            {
                // Construct block name
                dfLocation.Exterior.ExteriorData.BlockNames[i] = ResolveRmbBlockName(
                    ref dfLocation,
                    dfLocation.Exterior.ExteriorData.BlockIndex[i],
                    dfLocation.Exterior.ExteriorData.BlockNumber[i],
                    dfLocation.Exterior.ExteriorData.BlockCharacter[i]);
            }
        }

        /// <summary>
        /// Read LocationRecordElementData common to both MapPItem and MapDItem
        /// </summary>
        /// <param name="reader">A binary reader to data.</param>
        /// <param name="region">Region index.</param>
        /// <param name="recordElement">Destination DFLocation.LocationRecordElement.</param>
        private void ReadLocationRecordElement(ref BinaryReader reader, int region, ref DFLocation.LocationRecordElement recordElement)
        {
            // Read LocationDoorElement
            UInt32 doorCount = reader.ReadUInt32();
            recordElement.DoorCount = doorCount;
            recordElement.Doors = new DFLocation.LocationDoorElement[doorCount];
            for (int door = 0; door < doorCount; door++)
            {
                recordElement.Doors[door].BuildingDataIndex = reader.ReadUInt16();
                recordElement.Doors[door].NullValue = reader.ReadByte();
                recordElement.Doors[door].Mask = reader.ReadByte();
                recordElement.Doors[door].Unknown1 = reader.ReadByte();
                recordElement.Doors[door].Unknown2 = reader.ReadByte();
            }

            // Read LocationRecordElementHeader
            recordElement.Header.AlwaysOne1 = reader.ReadUInt32();
            recordElement.Header.NullValue1 = reader.ReadUInt16();
            recordElement.Header.NullValue2 = reader.ReadByte();
            recordElement.Header.X = reader.ReadInt32();
            recordElement.Header.NullValue3 = reader.ReadUInt32();
            recordElement.Header.Y = reader.ReadInt32();
            recordElement.Header.IsExterior = reader.ReadUInt16();
            recordElement.Header.NullValue4 = reader.ReadUInt16();
            recordElement.Header.Unknown1 = reader.ReadUInt32();
            recordElement.Header.Unknown2 = reader.ReadUInt32();
            recordElement.Header.AlwaysOne2 = reader.ReadUInt16();
            recordElement.Header.LocationId = reader.ReadUInt16();
            recordElement.Header.NullValue5 = reader.ReadUInt32();
            recordElement.Header.IsInterior = reader.ReadUInt16();
            recordElement.Header.ExteriorLocationId = reader.ReadUInt32();
            recordElement.Header.NullValue6 = reader.ReadBytes(26);
            recordElement.Header.LocationName = FileProxy.ReadCStringSkip(reader, 0, 32);
            recordElement.Header.Unknown3 = reader.ReadBytes(9);
        }

        /// <summary>
        /// Reads MapDItem data.
        /// </summary>
        /// <param name="reader">A binary reader to data.</param>
        /// <param name="region">Region index.</param>
        /// <param name="dfLocation">Destination DFLocation.</param>
        private void ReadMapDItem(ref BinaryReader reader, int region, ref DFLocation dfLocation)
        {
            // Exit if no data
            dfLocation.HasDungeon = false;
            if (reader.BaseStream.Length == 0)
                return;

            // Find dungeon offset
            bool found = false;
            UInt32 locationId = dfLocation.Exterior.RecordElement.Header.LocationId;
            UInt32 dungeonCount = reader.ReadUInt32();
            DungeonOffset dungeonOffset = new DungeonOffset();
            for (int i = 0; i < dungeonCount; i++)
            {
                // Search for dungeon offset matching location id
                dungeonOffset.Offset = reader.ReadUInt32();
                dungeonOffset.IsDungeon = reader.ReadUInt16();
                dungeonOffset.ExteriorLocationId = reader.ReadUInt16();
                if (dungeonOffset.ExteriorLocationId == locationId)
                {
                    found = true;
                    break;
                }
            }

            // Exit if correct offset not found
            if (!found)
                return;

            // Position reader at dungeon record by reading offset and adding to end of offset table
            reader.BaseStream.Position = 4 + dungeonCount * 8 + dungeonOffset.Offset;

            // Read LocationRecordElement
            ReadLocationRecordElement(ref reader, region, ref dfLocation.Dungeon.RecordElement);

            // Read DungeonHeader
            dfLocation.Dungeon.Header.NullValue1 = reader.ReadUInt16();
            dfLocation.Dungeon.Header.Unknown1 = reader.ReadUInt32();
            dfLocation.Dungeon.Header.Unknown2 = reader.ReadUInt32();
            dfLocation.Dungeon.Header.BlockCount = reader.ReadUInt16();
            dfLocation.Dungeon.Header.Unknown3 = reader.ReadBytes(5);

            // Read DungeonBlock elements
            dfLocation.Dungeon.Blocks = new DFLocation.DungeonBlock[dfLocation.Dungeon.Header.BlockCount];
            for (int i = 0; i < dfLocation.Dungeon.Header.BlockCount; i++)
            {
                // Read data
                dfLocation.Dungeon.Blocks[i].X = reader.ReadSByte();
                dfLocation.Dungeon.Blocks[i].Z = reader.ReadSByte();
                dfLocation.Dungeon.Blocks[i].BlockNumberStartIndexBitfield = reader.ReadUInt16();

                // Decompose bitfield
                UInt16 bitfield = dfLocation.Dungeon.Blocks[i].BlockNumberStartIndexBitfield;
                dfLocation.Dungeon.Blocks[i].BlockNumber = (UInt16)(bitfield & 0x3ff);
                dfLocation.Dungeon.Blocks[i].IsStartingBlock = ((bitfield & 0x400) == 0x400);
                dfLocation.Dungeon.Blocks[i].BlockIndex = (Byte)(bitfield >> 11);

                // Compose block name
                dfLocation.Dungeon.Blocks[i].BlockName = String.Format("{0}{1:0000000}.RDB", rdbBlockLetters[dfLocation.Dungeon.Blocks[i].BlockIndex], dfLocation.Dungeon.Blocks[i].BlockNumber);
            }

            // Set dungeon flag
            dfLocation.HasDungeon = true;
        }

        #endregion

        #region ExperimentalSmallerDungeons

        // Generates a smaller dungeon by overwriting the block layout
        // Creates a single interior block surrounded by 4 border blocks (smallest viable dungeon)
        // Should not be called for main story dungeons
        // Will filter out dungeons that are already below a threshold size
        void GenerateSmallerDungeon(ref DFLocation dfLocation)
        {
            // Smallest viable dungeon block count, comprised of 1x interior block and 4x border blocks
            const int threshold = 5;

            // Must not be called for main story dungeons
            if (DaggerfallWorkshop.DaggerfallDungeon.IsMainStoryDungeon(dfLocation.MapTableData.MapId))
                throw new Exception("GenerateSmallerDungeon() must not be called on a main story dungeon.");

            // Ignore small dungeons under threshold - this will exclude already small crypts and the like
            if (dfLocation.Dungeon.Blocks == null || dfLocation.Dungeon.Blocks.Length <= threshold)
                return;

            // Dungeon layout might be looked up multiple times in a row by different systems
            // It is expected to see this output more than once in log when generating quests, etc.
            // Disabling for now just to reduce spam to logs
            //UnityEngine.Debug.LogFormat("Generating smaller dungeon for {0}/{1}", dfLocation.RegionName, dfLocation.Name);

            // TODO: Some potential issues for later:
            //  * Quests assigned to a dungeon will probably break/crash as marker layout different in smaller dungeon, must handle this
            //  * Might need to ensure automap cache is cleared when switching smaller dungeons setting on/off

            // Seed random generation with map ID so we get the same layout each time map is looked up
            DaggerfallWorkshop.DFRandom.Seed = (uint)dfLocation.MapTableData.MapId;

            // Generate new dungeon layout with smallest viable dungeon (1x normal block surrounded by 4x border blocks)
            DFLocation.DungeonBlock[] layout = new DFLocation.DungeonBlock[5];
            layout[0] = GenerateRDBBlock(0, 0, false, true, ref dfLocation);           // Central starting block
            layout[1] = GenerateRDBBlock(0, -1, true, false, ref dfLocation);          // North border block
            layout[2] = GenerateRDBBlock(-1, 0, true, false, ref dfLocation);          // West border block
            layout[3] = GenerateRDBBlock(1, 0, true, false, ref dfLocation);           // East border block
            layout[4] = GenerateRDBBlock(0, 1, true, false, ref dfLocation);           // South border block

            // Inject new block array into location
            dfLocation.Dungeon.Blocks = layout;
        }

        /// <summary>
        /// Generates a new dungeon block by selecting one at random from a reference dungeon layout.
        /// </summary>
        /// <param name="x">X block tile position.</param>
        /// <param name="z">Z block tile position.</param>
        /// <param name="borderBlock">True to select a border block, false to select an interior block.</param>
        /// <param name="startingBlock">True to make this a starting block (must only be one).</param>
        /// <param name="dfLocation">Reference location to select a random block from.</param>
        /// <returns>DFLocation.DungeonBlock</returns>
        DFLocation.DungeonBlock GenerateRDBBlock(sbyte x, sbyte z, bool borderBlock, bool startingBlock, ref DFLocation dfLocation)
        {
            // Get random block from reference location and overwrite some properties
            DFLocation.DungeonBlock block = GetRandomBlock(borderBlock, ref dfLocation);
            block.X = x;
            block.Z = z;
            block.IsStartingBlock = startingBlock;

            return block;
        }

        /// <summary>
        /// Gets a random block from reference location.
        /// </summary>
        /// <param name="borderBlock">True to select a border block, false to select an interior block.</param>
        /// <param name="dfLocation">Reference location to select a random block from.</param>
        /// <returns>DFLocation.DungeonBlock</returns>
        DFLocation.DungeonBlock GetRandomBlock(bool borderBlock, ref DFLocation dfLocation)
        {
            List<DFLocation.DungeonBlock> filteredBlocks = new List<DFLocation.DungeonBlock>();
            foreach (DFLocation.DungeonBlock block in dfLocation.Dungeon.Blocks)
            {
                // Is this a border block?
                bool isBorderBlock = block.BlockName.StartsWith("B", StringComparison.InvariantCultureIgnoreCase);

                // Collect blocks based on params
                if (borderBlock && isBorderBlock)
                    filteredBlocks.Add(block);
                else if (!borderBlock && !isBorderBlock)
                    filteredBlocks.Add(block);
            }

            // Should have found at least one block
            if (filteredBlocks.Count == 0)
                throw new Exception(string.Format("GetRandomBlock() failed to find a suitable block. borderBlock={0}, region={1}, location={2}", borderBlock.ToString(), dfLocation.RegionName, dfLocation.Name));

            // Select a random index from pool and return this block
            return filteredBlocks[DaggerfallWorkshop.DFRandom.random_range(filteredBlocks.Count)];
        }

        #endregion
    }
}
