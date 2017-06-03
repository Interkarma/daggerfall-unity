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
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// A location or site involved in a quest.
    /// A Place can be a random local/remote location or a fixed permanent location.
    /// </summary>
    public class Place : QuestResource
    {
        #region Fields

        PlaceTypes placeType;   // Fixed/remote/local
        string name;            // Source name for data table
        int p1;                 // Parameter 1
        int p2;                 // Parameter 2
        int p3;                 // Parameter 3

        SiteDetails siteDetails;

        #endregion

        #region Enums

        public enum PlaceTypes
        {
            None,
            Fixed,
            Remote,
            Local,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the PlaceType of this Place.
        /// </summary>
        public PlaceTypes PlaceType
        {
            get { return placeType; }
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

                // Get place type
                string siteType = match.Groups["siteType"].Value;
                if (string.Compare(siteType, "local", true) == 0)
                {
                    // This is a local place
                    placeType = PlaceTypes.Local;
                }
                else if (string.Compare(siteType, "remote", true) == 0)
                {
                    // This is a remote place
                    placeType = PlaceTypes.Remote;
                }
                else if (string.Compare(siteType, "permanent", true) == 0)
                {
                    placeType = PlaceTypes.Fixed;
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

                // Handle place by type
                if (placeType == PlaceTypes.Local)
                {
                    // Get a local site from same town quest was issued
                    SetupLocalSite();
                }
                else if (placeType == PlaceTypes.Remote)
                {
                    // Get a remote site in same region quest was issued
                    SetupRemoteSite();
                }
                else if (placeType == PlaceTypes.Fixed && p1 > 0xc300)
                {
                    // TODO: Get a fixed site, such as a key city or dungeon
                    //SetupFixedLocation();
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
                // TODO: Just stubbing out for testing right now as Place class not complete enough to return real values

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

        #region Local Place Methods

        /// <summary>
        /// Get a local building in the town player is currently at.
        /// What happens if quest requests a local building type not available at current location?
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

            // Get building type
            DFLocation.BuildingTypes buildingType = (DFLocation.BuildingTypes)p2;

            // TODO: Support random site
            if (p2 == -1)
                throw new Exception("Random local site not implemented at this time.");

            // Get an array of potential quest sites with specified building type
            SiteDetails[] foundSites = CollectQuestSitesOfBuildingType(location, buildingType);
            if (foundSites == null || foundSites.Length == 0)
                throw new Exception(string.Format("Could not find local site of type {0} in location {1}.{2}.", buildingType, location.RegionName, location.Name));

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
            // TODO: Support dungeons
            if (p1 == 1)
                throw new Exception("Remote dungeon site not implemented at this time.");

            // TODO: Support random site
            if (p2 == -1)
                throw new Exception("Random remote site not implemented at this time.");

            // Get town candidate holding building type
            GetRandomTownSite((DFLocation.BuildingTypes)p2);
        }

        #endregion

        #region Fixed Site Methods

        /// <summary>
        /// Setup a fixed location.
        /// </summary>
        void SetupFixedLocation()
        {
            // Dungeon interiors have p2 > 0xfa00, exteriors have p2 = 0x01 || p2 = 0x02
            // Need to p1 - 1 if inside dungeon for exterior location id
            int locationId = -1;
            if (p2 > 0xfa00)
                locationId = p1 - 1;
            else
                locationId = p1;

            // Get location
            DFLocation location;
            if (!DaggerfallUnity.Instance.ContentReader.GetQuestLocation(locationId, out location))
                throw new Exception(string.Format("Could not find locationId: '{0};", locationId));

            // TODO: Create a site for this location
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
                            // Checking for quest flat marker inside record interior
                            if (!InteriorHasQuestMarker(blocks[index], i))
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
                                buildingName = BuildingNames.GetName(
                                    buildingSummary[i].NameSeed,
                                    buildingSummary[i].BuildingType,
                                    buildingSummary[i].FactionId,
                                    location.Name,
                                    location.RegionName);
                            }

                            // Configure new site details
                            siteDetails = new SiteDetails();
                            siteDetails.mapId = location.MapTableData.MapId;
                            siteDetails.locationId = location.Exterior.ExteriorData.LocationId;
                            siteDetails.regionName = location.RegionName;
                            siteDetails.locationName = location.Name;
                            siteDetails.isBuilding = true;
                            siteDetails.buildingKey = buildingSummary[i].buildingKey;
                            siteDetails.buildingName = buildingName;
                            foundSites.Add(siteDetails);
                        }
                    }
                }
            }

            return foundSites.ToArray();
        }

        /// <summary>
        /// Gets a town for remote site containing building type.
        /// Daggerfall's locations are so generic that we usually find a match within a few random attempts
        /// compared to indexing several hundred locations and only selecting from known-good candidates.
        /// In short, there are so many possible candidates it's not worth narrowing them down. Throw darts instead.
        /// Basic checks are still done to reject unsuitable locations very quickly.
        /// </summary>
        bool GetRandomTownSite(DFLocation.BuildingTypes requiredBuildingType)
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

                // Discard all known dungeon types
                if ((int)regionData.MapTable[locationIndex].DungeonType <= (int)DFRegion.DungeonTypes.Cemetery)
                    continue;

                // Get location data for town
                DFLocation locationData = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(regionIndex, locationIndex);
                if (!locationData.Loaded)
                    continue;

                // Check if town contains building type in MAPS.BSA directory
                if (!HasBuildingType(locationData, requiredBuildingType))
                    continue;

                // Get an array of potential quest sites with specified building type
                // This ensures building site actually exists inside town, as MAPS.BSA directory can be incorrect
                SiteDetails[] foundSites = CollectQuestSitesOfBuildingType(locationData, requiredBuildingType);
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
        /// Check interior for quest marker (199.11) indicating this can be a quest site.
        /// </summary>
        bool InteriorHasQuestMarker(DFBlock blockData, int recordIndex)
        {
            DFBlock.RmbSubRecord recordData = blockData.RmbBlock.SubRecords[recordIndex];
            foreach (DFBlock.RmbBlockFlatObjectRecord obj in recordData.Interior.BlockFlatObjectRecords)
            {
                if (obj.TextureArchive == 199 && obj.TextureRecord == 11)
                    return true;
            }

            return false;
        }

        #endregion
    }
}