// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

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
        Dictionary<int, MapSummary> mapDict;
        Noise noise;

        public struct MapSummary
        {
            public int ID;
            public int RegionIndex;
            public int MapIndex;
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

        public Noise Noise
        {
            get { return noise; }
        }

        #region Constructors

        public ContentReader(string arena2Path, DaggerfallUnity dfUnity)
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

            int id = MapsFile.GetMapPixelID(mapPixelX, mapPixelY);
            if (mapDict.ContainsKey(id))
            {
                summaryOut = mapDict[id];
                return true;
            }

            summaryOut = new MapSummary();
            return false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Setup API file readers.
        /// </summary>
        private void SetupReaders()
        {
            // Setup general content readers
            if (blockFileReader == null)
                blockFileReader = new BlocksFile(Path.Combine(arena2Path, BlocksFile.Filename), FileUsage.UseMemory, true);
            if (mapFileReader == null)
                mapFileReader = new MapsFile(Path.Combine(arena2Path, MapsFile.Filename), FileUsage.UseMemory, true);
            if (monsterFileReader == null)
                monsterFileReader = new MonsterFile(Path.Combine(arena2Path, MonsterFile.Filename), FileUsage.UseMemory, true);
            if (woodsFileReader == null)
                woodsFileReader = new WoodsFile(Path.Combine(arena2Path, WoodsFile.Filename), FileUsage.UseMemory, true);
            if (noise == null)
                noise = new Noise();

            // Build map lookup dictionary
            if (mapDict == null)
                EnumerateMaps();

            isReady = true;
        }

        /// <summary>
        /// Build dictionary of locations.
        /// </summary>
        private void EnumerateMaps()
        {
            mapDict = new Dictionary<int, MapSummary>();
            for (int region = 0; region < mapFileReader.RegionCount; region++)
            {
                DFRegion dfRegion = mapFileReader.GetRegion(region);
                for (int location = 0; location < dfRegion.LocationCount; location++)
                {
                    MapSummary summary = new MapSummary();
                    DFRegion.RegionMapTable mapTable = dfRegion.MapTable[location];
                    summary.ID = mapTable.MapId & 0x000fffff;
                    summary.RegionIndex = region;
                    summary.MapIndex = location;
                    mapDict.Add(summary.ID, summary);
                }
            }
        }

        #endregion
    }
}
