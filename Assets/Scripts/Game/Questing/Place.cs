// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// A site involved in a quest.
    /// Can be a random local/remote location or a fixed permanent location.
    /// </summary>
    public class Place : QuestResource
    {
        #region Fields

        Scopes scope;               // Fixed/remote/local
        string name;                // Source name for data table
        int p1;                     // Parameter 1
        int p2;                     // Parameter 2
        int p3;                     // Parameter 3

        SiteDetails siteDetails;    // Site found using inputs

        #endregion

        #region Enums

        public enum Scopes
        {
            None,
            Fixed,
            Remote,
            Local,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the scope of this Place.
        /// </summary>
        public Scopes Scope
        {
            get { return scope; }
        }

        /// <summary>
        /// Gets the Name of this Place used for data table lookup.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets parameter 1 of Place.
        /// </summary>
        public int Param1
        {
            get { return p1; }
        }

        /// <summary>
        /// Gets parameter 2 of Place.
        /// </summary>
        public int Param2
        {
            get { return p2; }
        }

        /// <summary>
        /// Gets parameter 3 of Place.
        /// </summary>
        public int Param3
        {
            get { return p3; }
        }

        /// <summary>
        /// Gets or sets full site details of Place.
        /// </summary>
        public SiteDetails SiteDetails
        {
            get { return siteDetails; }
            set { siteDetails = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="parentQuest">Parent quest.</param>
        public Place(Quest parentQuest)
            : base(parentQuest)
        {
        }

        /// <summary>
        /// Construct a Place resource from QBN input.
        /// </summary>
        /// <param name="parentQuest">Parent quest.</param>
        /// <param name="line">Place definition line from QBN.</param>
        public Place(Quest parentQuest, string line)
            : base(parentQuest)
        {
            SetResource(line);
        }

        #endregion

        #region Overrides

        public override void SetResource(string line)
        {
            base.SetResource(line);

            // Match string for Place variants
            string matchStr = @"(Place|place) (?<symbol>\w+) (?<siteType>local|remote|permanent) (?<siteName>\w+)";

            // Try to match source line with pattern
            Match match = Regex.Match(line, matchStr);
            if (match.Success)
            {
                // Seed random
                UnityEngine.Random.InitState(Time.renderedFrameCount);

                // Store symbol for quest system
                Symbol = new Symbol(match.Groups["symbol"].Value);

                // Get place scope
                string siteType = match.Groups["siteType"].Value;
                if (string.Compare(siteType, "local", true) == 0)
                {
                    // This is a local place
                    scope = Scopes.Local;
                }
                else if (string.Compare(siteType, "remote", true) == 0)
                {
                    // This is a remote place
                    scope = Scopes.Remote;
                }
                else if (string.Compare(siteType, "permanent", true) == 0)
                {
                    // This is a permanent place
                    scope = Scopes.Fixed;
                }
                else
                {
                    throw new Exception(string.Format("Place found no site type match found for source: '{0}'. Must be local|remote|permanent.", line));
                }

                // Get place name for parameter lookup
                name = match.Groups["siteName"].Value;
                if (string.IsNullOrEmpty(name))
                {
                    throw new Exception(string.Format("Place site name empty for source: '{0}'", line));
                }

                // Try to read place variables from data table
                Table placesTable = QuestMachine.Instance.PlacesTable;
                if (placesTable.HasValue(name))
                {
                    // Store values
                    p1 = CustomParseInt(placesTable.GetValue("p1", name));
                    p2 = CustomParseInt(placesTable.GetValue("p2", name));
                    p3 = CustomParseInt(placesTable.GetValue("p3", name));
                }
                else
                {
                    throw new Exception(string.Format("Could not find place name in data table: '{0};", name));
                }

                // Handle place by scope
                if (scope == Scopes.Local)
                {
                    // Get a local site from same town quest was issued
                    SetupLocalSite();
                }
                else if (scope == Scopes.Remote)
                {
                    // Get a remote site in same region quest was issued
                    SetupRemoteSite();
                }
                else if (scope == Scopes.Fixed && p1 > 0xc300)
                {
                    // Get a fixed site, such as a capital city or dungeon
                    SetupFixedLocation();
                }
                else
                {
                    throw new Exception("Invalid placeType in line: " + line);
                }
            }
        }

        /// <summary>
        /// Expand macro for a Place.
        /// </summary>
        /// <param name="macro">Macro type to expand.</param>
        /// <param name="text">Expanded text for this macro type. Empty if macro cannot be expanded.</param>
        /// <returns>True if macro expanded, otherwise false.</returns>
        public override bool ExpandMacro(MacroTypes macro, out string textOut)
        {
            textOut = string.Empty;
            bool result = true;
            switch (macro)
            {
                case MacroTypes.NameMacro1:             // Name of house/business (e.g. Odd Blades)
                    textOut = siteDetails.buildingName;
                    break;

                case MacroTypes.NameMacro2:             // Name of location (e.g. Gothway Garden)
                    textOut = siteDetails.locationName;
                    break;

                case MacroTypes.NameMacro3:             // Name of dungeon (e.g. Privateer's Hold) - Not sure about this one, need to test
                    textOut = siteDetails.locationName;
                    break;

                case MacroTypes.NameMacro4:             // Name of region (e.g. Tigonus)
                    textOut = siteDetails.regionName;
                    break;

                default:                                // Macro not supported
                    result = false;
                    break;
            }

            return result;
        }

        #endregion

        #region Public Methods
        #endregion

        #region Local Site Methods

        /// <summary>
        /// Get a local building in the town player is currently at.
        /// Throws exception if no valid buildings of specified type are found.
        /// Example of how this can happen is issuing a quest to a local palace in a town with no palace.
        /// Use remote palace instead to ensure quest can select from entire region.
        /// </summary>
        void SetupLocalSite()
        {
            // Get player location
            DFLocation location = GameManager.Instance.PlayerGPS.CurrentLocation;
            if (!location.Loaded)
                throw new Exception("Tried to setup a local site but player is not in a location (i.e. player in wilderness).");

            // Local dungeons not supported
            if (p1 == 1)
                throw new Exception("Cannot specify a local dungeon place resource. Only use of remote or fixed supported for dungeons.");

            // Get list of valid sites
            SiteDetails[] foundSites = null;
            if (p2 == -1)
                foundSites = CollectRandomQuestSites(location);
            else
                foundSites = CollectQuestSitesOfBuildingType(location, (DFLocation.BuildingTypes)p2);

            // Must have found at least one site
            if (foundSites == null || foundSites.Length == 0)
                throw new Exception(string.Format("Could not find local site with P2={0} in {1}\\{2}.", p2, location.RegionName, location.Name));

            // Select a random site from available list
            int selectedIndex = UnityEngine.Random.Range(0, foundSites.Length);
            siteDetails = foundSites[selectedIndex];
        }

        #endregion

        #region Remote Site Methods

        /// <summary>
        /// Get a remote site in the same region as player.
        /// </summary>
        void SetupRemoteSite()
        {
            // Select remote dungeon or building of building type
            if (p1 == 1)
                SelectRemoteDungeonSite(p2);
            else
                SelectRemoteTownSite((DFLocation.BuildingTypes)p2);
        }

        /// <summary>
        /// Find a random dungeon site in player region.
        /// dungeonTypeIndex == -1 will select from all dungeons of type 0 through 16
        /// dungeonTypeIndex == 0 through 16 will select from all available dungeons of that specific type
        /// Note: Template only maps dungeon types 0-16 to p2 types dungeon0 through dungeon16.
        /// This is probably because types 17-18 don't seem to contain quest markers.
        /// Warning: Not all dungeon types are available in all regions. http://en.uesp.net/wiki/Daggerfall:Dungeons#Overview_of_Dungeon_Locations
        /// </summary>
        bool SelectRemoteDungeonSite(int dungeonTypeIndex)
        {
            // Get player region
            int regionIndex = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            DFRegion regionData = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(regionIndex);

            // Cannot use a region with no locations
            // This should not happen in normal play
            if (regionData.LocationCount == 0)
                return false;

            //Debug.LogFormat("Selecting for random dungeon of type {0} in {1}", dungeonTypeIndex, regionData.Name);

            // Get indices for all dungeons of this type
            int[] foundIndices = CollectDungeonIndicesOfType(regionData, dungeonTypeIndex);
            if (foundIndices == null || foundIndices.Length == 0)
            {
                Debug.LogFormat("Could not find any random dungeons of type {0} in {1}", dungeonTypeIndex, regionData.Name);
                return false;
            }

            //Debug.LogFormat("Found a total of {0} possible dungeons of type {1} in {2}", foundIndices.Length, dungeonTypeIndex, regionData.Name);

            // Select a random dungeon location index from available list
            int index = UnityEngine.Random.Range(0, foundIndices.Length);

            // Get location data for selected dungeon
            DFLocation location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(regionIndex, foundIndices[index]);
            if (!location.Loaded)
                return false;

            // Check for one or more quest markers inside dungeon
            int totalQuestMarkers, totalQuestItemMarkers;
            if (!EnumerateDungeonQuestMarkers(location, out totalQuestMarkers, out totalQuestItemMarkers))
            {
                Debug.LogFormat("Could not find any quest markers in random dungeon {0}", location.Name);
                return false;
            }

            // Configure new site details
            siteDetails = new SiteDetails();
            siteDetails.questUID = ParentQuest.UID;
            siteDetails.siteType = SiteTypes.Dungeon;
            siteDetails.mapId = location.MapTableData.MapId;
            siteDetails.locationId = location.Exterior.ExteriorData.LocationId;
            siteDetails.regionName = location.RegionName;
            siteDetails.locationName = location.Name;
            siteDetails.totalQuestMarkers = totalQuestMarkers;
            siteDetails.totalQuestItemMarkers = totalQuestItemMarkers;

            //Debug.LogFormat("Found {0} quest markers and {1} quest item markers in random dungeon {2}", totalQuestMarkers, totalQuestItemMarkers, location.Name);
            //Debug.LogFormat("Selected dungeon of type {0} at {1}\\{2}", dungeonTypeIndex, location.RegionName, location.Name);

            return true;
        }

        /// <summary>
        /// Find a town for remote site containing building type.
        /// Daggerfall's locations are so generic that we usually find a match within a few random attempts
        /// compared to indexing several hundred locations and only selecting from known-good candidates.
        /// In short, there are so many possible candidates it's not worth narrowing them down. Throw darts instead.
        /// Basic checks are still done to reject unsuitable locations very quickly.
        /// </summary>
        bool SelectRemoteTownSite(DFLocation.BuildingTypes requiredBuildingType)
        {
            // Get player region
            int regionIndex = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            DFRegion regionData = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(regionIndex);

            // Cannot use a region with no locations
            // This should not happen in normal play
            if (regionData.LocationCount == 0)
                return false;

            // Find random town containing building
            int attempts = 0;
            bool found = false;
            while (!found)
            {
                // Increment attempts
                attempts++;

                // Get a random location index
                int locationIndex = UnityEngine.Random.Range(0, (int)regionData.LocationCount);

                // Discard all dungeon location types
                if (IsDungeonType(regionData.MapTable[locationIndex].LocationType))
                    continue;

                // Get location data for town
                DFLocation location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(regionIndex, locationIndex);
                if (!location.Loaded)
                    continue;

                // Get list of valid sites
                SiteDetails[] foundSites = null;
                if (p2 == -1)
                {
                    // Collect random building sites
                    foundSites = CollectRandomQuestSites(location);
                }
                else
                {
                    // Check if town contains specified building type in MAPS.BSA directory
                    if (!HasBuildingType(location, requiredBuildingType))
                        continue;

                    // Get an array of potential quest sites with specified building type
                    // This ensures building site actually exists inside town, as MAPS.BSA directory can be incorrect
                    foundSites = CollectQuestSitesOfBuildingType(location, (DFLocation.BuildingTypes)p2);
                }

                // Must have found at least one site
                if (foundSites == null || foundSites.Length == 0)
                    continue;

                // Select a random site from available list
                int selectedIndex = UnityEngine.Random.Range(0, foundSites.Length);
                siteDetails = foundSites[selectedIndex];

                // All conditions have been satisfied
                found = true;
            }

            //Debug.LogFormat("Found remote candidate site in {0} attempts", attempts);

            return true;
        }

        #endregion

        #region Fixed Site Methods

        /// <summary>
        /// Setup a fixed location.
        /// TODO: Not sure if need to support last byte of p2 for "sites transferred to by teleport cheat".
        /// I *think* p2 just reference start markers inside dungeon but may have some placement meaning.
        /// </summary>
        void SetupFixedLocation()
        {
            // Attempt to get locationId by p1 - try p1 first then p1-1
            // This should work out dungeon or exterior loctionId as needed
            // No longer certain about p2 meaning of 0xfa
            // Possible p2 only be used for teleport cheat
            SiteTypes siteType;
            DFLocation location;
            if (!DaggerfallUnity.Instance.ContentReader.GetQuestLocation(p1, out location))
            {
                // Could be a dungeon, attempt to get locationId by p1-1
                if (!DaggerfallUnity.Instance.ContentReader.GetQuestLocation(p1 - 1, out location))
                {
                    // p1 is a completely unknown locationId
                    throw new Exception(string.Format("Could not find locationId from p1 using: '{0}' or '{1}'", p1, p1 - 1));
                }
                else
                {
                    // If p1-1 resolves then dungeon is referenced
                    siteType = SiteTypes.Dungeon;
                }
            }
            else
            {
                // If p1 resolves then exterior is referenced
                siteType = SiteTypes.Town;
            }

            // Check for one or more quest markers inside dungeon
            // Towns do not have quest markers
            // Quite often town types are used only to reveal location on travel map
            int totalQuestMarkers = 0, totalQuestItemMarkers = 0;
            if (siteType == SiteTypes.Dungeon)
            {
                // Fixed dungeons should always have markers, throw exception if none found
                if (!EnumerateDungeonQuestMarkers(location, out totalQuestMarkers, out totalQuestItemMarkers))
                    throw new Exception(string.Format("Could not find any quest markers in random dungeon {0}", location.Name));
            }

            // Create a new site for this location
            siteDetails = new SiteDetails();
            siteDetails.questUID = ParentQuest.UID;
            siteDetails.siteType = siteType;
            siteDetails.mapId = location.MapTableData.MapId;
            siteDetails.locationId = location.Exterior.ExteriorData.LocationId;
            siteDetails.regionName = location.RegionName;
            siteDetails.locationName = location.Name;
            siteDetails.totalQuestMarkers = totalQuestMarkers;
            siteDetails.totalQuestItemMarkers = totalQuestItemMarkers;

            Debug.LogFormat("Setup fixed location {0}\\{1} of type {2} with {3}/{4} quest/item markers.", location.RegionName, location.Name, siteType.ToString(), totalQuestMarkers, totalQuestItemMarkers);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Custom parser to handle hex or decimal values from places data table.
        /// </summary>
        int CustomParseInt(string value)
        {
            int result = -1;
            if (value.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                result = int.Parse(value.Replace("0x", ""), NumberStyles.HexNumber);
            }
            else
            {
                result = int.Parse(value);
            }

            return result;
        }

        /// <summary>
        /// Checks if location is one of the dungeon types.
        /// </summary>
        bool IsDungeonType(DFRegion.LocationTypes locationType)
        {
            // Consider 3 major dungeon types and 2 graveyard types as dungeons
            // Will exclude locations with dungeons, such as Daggerfall, Wayrest, Sentinel
            if (locationType == DFRegion.LocationTypes.DungeonKeep ||
                locationType == DFRegion.LocationTypes.DungeonLabyrinth ||
                locationType == DFRegion.LocationTypes.DungeonRuin ||
                locationType == DFRegion.LocationTypes.GraveyardCommon ||
                locationType == DFRegion.LocationTypes.GraveyardForgotten)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets random building type from set of valid types.
        /// </summary>
        DFLocation.BuildingTypes GetRandomBuildingType()
        {
            int[] validBuildingTypes = { 0, 2, 3, 5, 6, 8, 9, 11, 12, 13, 14, 15, 17, 18, 19, 20 };
            int index = UnityEngine.Random.Range(0, validBuildingTypes.Length);

            return (DFLocation.BuildingTypes)validBuildingTypes[index];
        }

        /// <summary>
        /// Generate a list of potential sites based on a random building type.
        /// Will try a few times to find a valid building type before giving up.
        /// Caller must handle this outcome by throwing exception or trying again.
        /// </summary>
        SiteDetails[] CollectRandomQuestSites(DFLocation location)
        {
            const int maxAttempts = 3;

            int attempts = 0;
            while (true)
            {
                // Bail after maximum attempts
                if (attempts++ > maxAttempts)
                    break;

                // Get valid quest sites of a random building type
                SiteDetails[] foundSites = CollectQuestSitesOfBuildingType(location, GetRandomBuildingType());
                if (foundSites == null || foundSites.Length == 0)
                    continue;

                return foundSites;
            }

            return null;
        }

        /// <summary>
        /// Generate a list of potential sites based on building type.
        /// This uses actual map layout and block data rather than the (often inaccurate) list of building in map data.
        /// Likely to need refinement over time to exclude buildings without proper quest markers, etc.
        /// </summary>
        SiteDetails[] CollectQuestSitesOfBuildingType(DFLocation location, DFLocation.BuildingTypes buildingType)
        {
            List<SiteDetails> foundSites = new List<SiteDetails>();

            // Iterate through all blocks
            DFBlock[] blocks;
            RMBLayout.GetLocationBuildingData(location, out blocks);
            int width = location.Exterior.ExteriorData.Width;
            int height = location.Exterior.ExteriorData.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Iterate through all buildings in this block
                    int index = y * width + x;
                    BuildingSummary[] buildingSummary = RMBLayout.GetBuildingData(blocks[index], x, y);
                    for (int i = 0; i < buildingSummary.Length; i++)
                    {
                        // Match building against required type
                        if (buildingSummary[i].BuildingType == buildingType)
                        {
                            // Building must be a quest site
                            // Checking for one or more quest markers inside record interior
                            int totalQuestMarkers, totalQuestItemMarkers;
                            if (!EnumerateBuildingQuestMarkers(blocks[index], i, out totalQuestMarkers, out totalQuestItemMarkers))
                                continue;

                            // Get building name based on type
                            string buildingName;
                            if (RMBLayout.IsResidence(buildingType))
                            {
                                // Generate a random surname for this residence
                                DFRandom.srand(Time.renderedFrameCount);
                                string surname = DaggerfallUnity.Instance.NameHelper.Surname(Utility.NameHelper.BankTypes.Breton);
                                buildingName = HardStrings.theNamedResidence.Replace("%s", surname);
                            }
                            else
                            {
                                // Use fixed name
                                buildingName = BuildingNames.GetName(
                                    buildingSummary[i].NameSeed,
                                    buildingSummary[i].BuildingType,
                                    buildingSummary[i].FactionId,
                                    location.Name,
                                    location.RegionName);
                            }

                            // Configure new site details
                            SiteDetails site = new SiteDetails();
                            site.questUID = ParentQuest.UID;
                            site.siteType = SiteTypes.Building;
                            site.mapId = location.MapTableData.MapId;
                            site.locationId = location.Exterior.ExteriorData.LocationId;
                            site.regionName = location.RegionName;
                            site.locationName = location.Name;
                            site.buildingKey = buildingSummary[i].buildingKey;
                            site.buildingName = buildingName;
                            site.totalQuestMarkers = totalQuestMarkers;
                            site.totalQuestItemMarkers = totalQuestItemMarkers;
                            foundSites.Add(site);
                        }
                    }
                }
            }

            return foundSites.ToArray();
        }

        /// <summary>
        /// Quick check to see if map data lists a specific building type in header.
        /// This can be inaccurate so should always followed up with a full check.
        /// </summary>
        bool HasBuildingType(DFLocation location, DFLocation.BuildingTypes buildingType)
        {
            foreach (var building in location.Exterior.Buildings)
            {
                if (building.BuildingType == buildingType)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets location indices for all dungeons of type.
        /// dungeonTypeIndex == -1 will select all available dungeon
        /// </summary>
        int[] CollectDungeonIndicesOfType(DFRegion regionData, int dungeonTypeIndex)
        {
            // Collect all dungeon types
            List<int> foundLocationIndices = new List<int>();
            for (int i = 0; i < regionData.LocationCount; i++)
            {
                // Discard all non-dungeon location types
                if (!IsDungeonType(regionData.MapTable[i].LocationType))
                    continue;

                //Debug.LogFormat("Checking dungeon type {0} at location {1}", (int)regionData.MapTable[i].DungeonType, regionData.MapNames[i]);

                // Collect dungeons
                if (dungeonTypeIndex == -1)
                {
                    // Limit range to indices 0-16
                    int testIndex = ((int)regionData.MapTable[i].DungeonType >> 8);
                    if (testIndex >= 0 && testIndex <= 16)
                        foundLocationIndices.Add(i);
                }
                else
                {
                    // Otherwise dungeon must be of specifed type
                    if (((int)regionData.MapTable[i].DungeonType >> 8) == dungeonTypeIndex)
                        foundLocationIndices.Add(i);
                }
            }

            return foundLocationIndices.ToArray();
        }

        /// <summary>
        /// Check building interior for total available quest markers.
        /// </summary>
        bool EnumerateBuildingQuestMarkers(DFBlock blockData, int recordIndex, out int totalQuestMarkers, out int totalQuestItemMarkers)
        {
            totalQuestMarkers = 0;
            totalQuestItemMarkers = 0;
            DFBlock.RmbSubRecord recordData = blockData.RmbBlock.SubRecords[recordIndex];
            foreach (DFBlock.RmbBlockFlatObjectRecord obj in recordData.Interior.BlockFlatObjectRecords)
            {
                if (obj.TextureArchive == 199)
                {
                    switch (obj.TextureRecord)
                    {
                        case 11:                        // Quest marker 199.11
                            totalQuestMarkers++;
                            break;
                        case 18:
                            totalQuestItemMarkers++;    // Quest item marker 199.18
                            break;
                    }
                }
            }

            // Return true if at least one quest marker
            if (totalQuestMarkers > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Check dungeon interior for total available quest markers.
        /// </summary>
        bool EnumerateDungeonQuestMarkers(DFLocation location, out int totalQuestMarkers, out int totalQuestItemMarkers)
        {
            totalQuestMarkers = 0;
            totalQuestItemMarkers = 0;

            // Quickly step through dungeon layout to find all blocks with markers
            foreach (var db in location.Dungeon.Blocks)
            {
                // Get block data
                DFBlock blockData = DaggerfallUnity.Instance.ContentReader.BlockFileReader.GetBlock(db.BlockName);

                // Iterate all groups
                foreach (DFBlock.RdbObjectRoot group in blockData.RdbBlock.ObjectRootList)
                {
                    // Skip empty object groups
                    if (null == group.RdbObjects)
                        continue;

                    // Look for flats in this group
                    foreach (DFBlock.RdbObject obj in group.RdbObjects)
                    {
                        // Look for editor flats
                        if (obj.Type == DFBlock.RdbResourceTypes.Flat)
                        {
                            if (obj.Resources.FlatResource.TextureArchive == 199)
                            {
                                switch (obj.Resources.FlatResource.TextureRecord)
                                {
                                    case 11:                        // Quest marker 199.11
                                        totalQuestMarkers++;
                                        break;
                                    case 18:
                                        totalQuestItemMarkers++;    // Quest item marker 199.18
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            // Return true if at least one quest marker
            if (totalQuestMarkers > 0)
                return true;

            return false;
        }

        #endregion
    }
}