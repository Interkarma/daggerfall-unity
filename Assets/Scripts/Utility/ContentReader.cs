// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using System;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// General interface between DaggerfallConnect API reader classes and Unity.
    /// </summary>
    public class ContentReader
    {
        bool isReady = false;
        string arena2Path;
        
        BlocksFile blockFileReader;
        MapsFile mapFileReader;
        MonsterFile monsterFileReader;
        WoodsFile woodsFileReader;
        FactionFile factionFileReader;
        FlatsFile flatsFileReader;
        PaintFile paintFileReader;
        Dictionary<int, MapSummary> mapDict;
        Dictionary<int, int> locationIdToMapIdDict;

        public struct MapSummary
        {
            public int ID;                  // mapTable.MapId & 0x000fffff for dict key and matching with ExteriorData.MapId
            public int MapID;               // Full mapTable.MapId for matching with localization key
            public int RegionIndex;
            public int MapIndex;
            public DFRegion.LocationTypes LocationType;
            public DFRegion.DungeonTypes DungeonType;
            public bool Discovered;
        }

        public bool IsReady
        {
            get { return isReady; }
        }

        public BlocksFile BlockFileReader
        {
            get { return blockFileReader; }
        }

        public MapsFile MapFileReader
        {
            get { return mapFileReader; }
        }

        public MonsterFile MonsterFileReader
        {
            get { return monsterFileReader; }
        }

        public WoodsFile WoodsFileReader
        {
            get { return woodsFileReader; }
        }

        public FactionFile FactionFileReader
        {
            get { return factionFileReader; }
        }

        public FlatsFile FlatsFileReader
        {
            get { return flatsFileReader; }
        }

        public PaintFile PaintFileReader
        {
            get { return paintFileReader; }
        }

        #region Constructors

        public ContentReader(string arena2Path)
        {
            this.arena2Path = arena2Path;
            SetupReaders();
        }

        #endregion

        #region Blocks & Locations

        /// <summary>
        /// Attempts to get a Daggerfall block from BLOCKS.BSA.
        /// </summary>
        /// <param name="name">Name of block.</param>
        /// <param name="blockOut">DFBlock data out.</param>
        /// <returns>True if successful.</returns>
        public bool GetBlock(string name, out DFBlock blockOut)
        {
            blockOut = new DFBlock();

            if (!isReady)
                return false;

            // Get block data
            blockOut = blockFileReader.GetBlock(name);
            if (blockOut.Type == DFBlock.BlockTypes.Unknown)
            {
                DaggerfallUnity.LogMessage(string.Format("Unknown block '{0}'.", name), true);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attempts to get a Daggerfall location from MAPS.BSA.
        /// </summary>
        /// <param name="regionIndex">Index of region.</param>
        /// <param name="locationIndex">Index of location.</param>
        /// <param name="locationOut">DFLocation data out.</param>
        /// <returns>True if successful.</returns>
        public bool GetLocation(int regionIndex, int locationIndex, out DFLocation locationOut)
        {
            locationOut = new DFLocation();

            if (!isReady)
                return false;

            // Get location data
            locationOut = mapFileReader.GetLocation(regionIndex, locationIndex);
            if (!locationOut.Loaded)
            {
                DaggerfallUnity.LogMessage(string.Format("Unknown location RegionIndex='{0}', LocationIndex='{1}'.", regionIndex, locationIndex), true);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attempts to get a Daggerfall location from MAPS.BSA.
        /// </summary>
        /// <param name="regionName">Name of region.</param>
        /// <param name="locationName">Name of location.</param>
        /// <param name="locationOut">DFLocation data out.</param>
        /// <returns>True if successful.</returns>
        public bool GetLocation(string regionName, string locationName, out DFLocation locationOut)
        {
            locationOut = new DFLocation();

            if (!isReady)
                return false;

            // Get location data
            locationOut = mapFileReader.GetLocation(regionName, locationName);
            if (!locationOut.Loaded)
            {
                DaggerfallUnity.LogMessage(string.Format("Unknown location RegionName='{0}', LocationName='{1}'.", regionName, locationName), true);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attempts to get a Daggerfall location from MAPS.BSA using a locationId from quest system.
        /// The locationId is different to the mapId, which is derived from location coordinates in world.
        /// At this time, best known way to determine locationId is from LocationRecordElementHeader data.
        /// This is linked to mapId at in EnumerateMaps().
        /// Note: Not all locations have a locationId, only certain key locations
        /// </summary>
        /// <param name="locationId">LocationId of map from quest system.</param>
        /// <param name="locationOut">DFLocation data out.</param>
        /// <returns>True if successful.</returns>
        public bool GetQuestLocation(int locationId, out DFLocation locationOut)
        {
            locationOut = new DFLocation();

            if (!isReady)
                return false;

            MapDictCheck();

            // Get mapId from locationId
            int mapId = LocationIdToMapId(locationId);
            if (mapDict.ContainsKey(mapId))
            {
                MapSummary summary = mapDict[mapId];
                return GetLocation(summary.RegionIndex, summary.MapIndex, out locationOut);
            }

            return false;
        }

        /// <summary>
        /// Determines if the current WorldCoord has a location.
        /// </summary>
        /// <param name="mapPixelX">Map pixel X.</param>
        /// <param name="mapPixelY">Map pixel Y.</param>
        /// <returns>True if there is a location at this map pixel.</returns>
        public bool HasLocation(int mapPixelX, int mapPixelY, out MapSummary summaryOut)
        {
            if (!isReady)
            {
                summaryOut = new MapSummary();
                return false;
            }
            MapDictCheck();

            int id = MapsFile.GetMapPixelID(mapPixelX, mapPixelY);
            if (mapDict.ContainsKey(id))
            {
                summaryOut = mapDict[id];
                return true;
            }

            summaryOut = new MapSummary();
            return false;
        }

        /// <summary>
        /// Determines if the current WorldCoord has a location.
        /// </summary>
        /// <param name="mapPixelX">Map pixel X.</param>
        /// <param name="mapPixelY">Map pixel Y.</param>
        /// <returns>True if there is a location at this map pixel.</returns>
        public bool HasLocation(int mapPixelX, int mapPixelY)
        {
            if (!isReady)
            {
                return false;
            }
            MapDictCheck();

            int id = MapsFile.GetMapPixelID(mapPixelX, mapPixelY);
            if (mapDict.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Converts LocationId from quest system to a MapId for map lookups.
        /// </summary>
        /// <param name="locationId">LocationId from quest system.</param>
        /// <returns>MapId if present or -1.</returns>
        public int LocationIdToMapId(int locationId)
        {
            if (locationIdToMapIdDict.ContainsKey(locationId))
            {
                return locationIdToMapIdDict[locationId];
            }

            return -1;
        }

        /// <summary>
        /// Gets path to FACTION.TXT file in StreamingAssets/Factions folder.
        /// </summary>
        /// <returns>Full path to file.</returns>
        public string GetFactionFilePath()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Factions");
            return Path.Combine(path, FactionFile.Filename);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Setup API file readers.
        /// </summary>
        private void SetupReaders()
        {
            // Try to setup Arena2-dependent content readers
            if (blockFileReader == null)
                blockFileReader = new BlocksFile(Path.Combine(arena2Path, BlocksFile.Filename), FileUsage.UseMemory, true);
            if (mapFileReader == null)
                mapFileReader = new MapsFile(Path.Combine(arena2Path, MapsFile.Filename), FileUsage.UseMemory, true);
            if (monsterFileReader == null)
                monsterFileReader = new MonsterFile(Path.Combine(arena2Path, MonsterFile.Filename), FileUsage.UseMemory, true);
            if (woodsFileReader == null)
                woodsFileReader = new WoodsFile(Path.Combine(arena2Path, WoodsFile.Filename), FileUsage.UseMemory, true);
            if (factionFileReader == null)
                factionFileReader = new FactionFile(GetFactionFilePath(), FileUsage.UseMemory, true);
            if (flatsFileReader == null)
                flatsFileReader = new FlatsFile(Path.Combine(arena2Path, FlatsFile.Filename), FileUsage.UseMemory, true);
            if (paintFileReader == null)
                paintFileReader = new PaintFile(Path.Combine(arena2Path, PaintFile.Filename), FileUsage.UseMemory, true);

            // Raise ready flag
            isReady = true;
        }

        private void MapDictCheck()
        {
            // Build map lookup dictionary
            if (mapDict == null && mapFileReader != null)
                EnumerateMaps();
        }

        /// <summary>
        /// Build dictionary of locations.
        /// </summary>
        private void EnumerateMaps()
        {
            //System.Diagnostics.Stopwatch s = System.Diagnostics.Stopwatch.StartNew();
            //long startTime = s.ElapsedMilliseconds;

            mapDict = new Dictionary<int, MapSummary>();
            locationIdToMapIdDict = new Dictionary<int, int>();
            for (int region = 0; region < mapFileReader.RegionCount; region++)
            {
                DFRegion dfRegion = mapFileReader.GetRegion(region);
                for (int location = 0; location < dfRegion.LocationCount; location++)
                {
                    MapSummary summary = new MapSummary();
                    try
                    {
                        // Get map summary
                        DFRegion.RegionMapTable mapTable = dfRegion.MapTable[location];
                        summary.ID = mapTable.MapId & 0x000fffff;
                        summary.MapID = mapTable.MapId;
                        summary.RegionIndex = region;
                        summary.MapIndex = location;
                        summary.LocationType = mapTable.LocationType;
                        summary.DungeonType = mapTable.DungeonType;

                        // TODO: This by itself doesn't account for DFRegion.LocationTypes.GraveyardForgotten locations that start the game discovered in classic
                        summary.Discovered = mapTable.Discovered;

                        mapDict.Add(summary.ID, summary);

                        // Link locationId with mapId - adds ~25ms overhead
                        int locationId = mapFileReader.ReadLocationIdFast(region, location);
                        locationIdToMapIdDict.Add(locationId, summary.ID);
                    }
                    catch (ArgumentException)
                    {
                        Debug.LogErrorFormat("Colliding location for MapId:{0} found when enumerating maps! Unable to initialise content reader. ", summary.ID);
                    }
                }
            }

            //long totalTime = s.ElapsedMilliseconds - startTime;
            //Debug.LogFormat("Total time to enum maps: {0}ms", totalTime);
        }

        #endregion
    }
}
