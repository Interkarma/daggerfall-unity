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
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Tracks player position in virtual world space.
    /// Provides information about world around the player.
    /// </summary>
    public class PlayerGPS : MonoBehaviour
    {
        #region Fields

        // Default location is outside Privateer's Hold
        [Range(0, 32735232)]
        public int WorldX;                      // Player X coordinate in Daggerfall world units
        [Range(0, 16351232)]
        public int WorldZ;                      // Player Z coordinate in Daggerfall world units

        DaggerfallUnity dfUnity;
        int lastMapPixelX = -1;
        int lastMapPixelY = -1;
        int currentClimateIndex;
        int currentPoliticIndex;
        DFRegion currentRegion;
        DFLocation currentLocation;
        DFLocation.ClimateSettings climateSettings;
        string regionName;
        bool hasCurrentLocation;
        bool isPlayerInLocationRect;
        DFRegion.LocationTypes currentLocationType;

        int locationWorldRectMinX;
        int locationWorldRectMaxX;
        int locationWorldRectMinZ;
        int locationWorldRectMaxZ;

        int lastRegionIndex;
        int lastClimateIndex;
        int lastPoliticIndex;

        Dictionary<int, DiscoveredLocation> discoveredLocations = new Dictionary<int, DiscoveredLocation>();

        #endregion

        #region Structs & Enums

        public struct DiscoveredLocation
        {
            public int mapID;
            public int mapPixelID;
            public string regionName;
            public string locationName;
            public Dictionary<int, DiscoveredBuilding> discoveredBuildings;
        }

        [Serializable]
        public struct DiscoveredBuilding
        {
            public int buildingKey;
            public string displayName;
            public int factionID;
            public int quality;
            public DFLocation.BuildingTypes buildingType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets current player map pixel.
        /// </summary>
        public DFPosition CurrentMapPixel
        {
            get { return MapsFile.WorldCoordToMapPixel(WorldX, WorldZ); }
        }

        /// <summary>
        /// Gets climate index based on player world position.
        /// </summary>
        public int CurrentClimateIndex
        {
            get { return currentClimateIndex; }
        }

        /// <summary>
        /// Gets political index based on player world position.
        /// </summary>
        public int CurrentPoliticIndex
        {
            get { return currentPoliticIndex; }
        }

        /// <summary>
        /// Gets region index based on player world position.
        /// </summary>
        public int CurrentRegionIndex
        {
            get { if (currentPoliticIndex == 64)
                    return 31; // High Rock sea coast
                  else
                    return currentPoliticIndex - 128; }
        }

        /// <summary>
        /// Gets one-based region index for faction data based on player world position.
        /// Equivalent to CurrentRegionIndex + 1.
        /// </summary>
        public int CurrentOneBasedRegionIndex
        {
            get { return currentPoliticIndex - 127; }
        }

        /// <summary>
        /// Gets current location index.
        /// Returns -1 when HasCurrentLocation=false
        /// </summary>
        public int CurrentLocationIndex
        {
            get { return (hasCurrentLocation) ? currentLocation.LocationIndex : -1; }
        }

        /// <summary>
        /// Gets climate properties based on player world position.
        /// </summary>
        public DFLocation.ClimateSettings ClimateSettings
        {
            get { return climateSettings; }
        }

        /// <summary>
        /// Gets region data based on player world position.
        /// </summary>
        public DFRegion CurrentRegion
        {
            get { return currentRegion; }
        }

        /// <summary>
        /// Gets location data based on player world position.
        /// Location may be empty, check for Loaded=true.
        /// </summary>
        public DFLocation CurrentLocation
        {
            get { return currentLocation; }
        }

        /// <summary>
        /// Gets current location type.
        /// Undefined when HasCurrentLocation=false
        /// </summary>
        public DFRegion.LocationTypes CurrentLocationType
        {
            get { return currentLocationType; }
        }

        /// <summary>
        /// Gets current location MapID.
        /// Returns -1 when HasCurrentLocation=false
        /// </summary>
        public int CurrentMapID
        {
            get { return (hasCurrentLocation) ? currentLocation.MapTableData.MapId : -1; }
        }

        /// <summary>
        /// Gets current region name based on world position.
        /// </summary>
        public string CurrentRegionName
        {
            get { return regionName; }
        }

        /// <summary>
        /// True if CurrentLocation is valid.
        /// </summary>
        public bool HasCurrentLocation
        {
            get { return hasCurrentLocation; }
        }

        /// <summary>
        /// True if player inside actual location rect.
        /// </summary>
        public bool IsPlayerInLocationRect
        {
            get { return isPlayerInLocationRect; }
        }

        /// <summary>
        /// Gets current location rect.
        /// Contents not valid when HasCurrentLocation=false
        /// </summary>
        public RectOffset LocationRect
        {
            get { return new RectOffset(locationWorldRectMinX, locationWorldRectMaxX, locationWorldRectMinZ, locationWorldRectMaxZ); }
        }

        #endregion

        #region Unity

        void Awake()
        {
            dfUnity = DaggerfallUnity.Instance;
        }

        void Start()
        {
            // Init change trackers for event system
            lastRegionIndex = CurrentRegionIndex;
            lastClimateIndex = CurrentClimateIndex;
            lastPoliticIndex = CurrentPoliticIndex;
        }

        void Update()
        {
            // Do nothing if not ready
            if (!ReadyCheck())
                return;

            // Update local world information whenever player map pixel changes
            DFPosition pos = CurrentMapPixel;
            if (pos.X != lastMapPixelX || pos.Y != lastMapPixelY)
            {
                RaiseOnMapPixelChangedEvent(pos);
                UpdateWorldInfo(pos.X, pos.Y);
                lastMapPixelX = pos.X;
                lastMapPixelY = pos.Y;
                isPlayerInLocationRect = false;
            }

            // Raise other events
            RaiseEvents();

            // Check if player is inside actual location rect
            PlayerLocationRectCheck();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Force update of world information (climate, politic, etc.) when Update() not running.
        /// </summary>
        public void UpdateWorldInfo()
        {
            DFPosition pos = CurrentMapPixel;
            UpdateWorldInfo(pos.X, pos.Y);
        }

        /// <summary>
        /// Gets NameHelper.BankType in player's current region.
        /// In practice this will always be Redguard/Breton/Nord.
        /// Supporting a few other name banks for possible diversity later.
        /// </summary>
        public NameHelper.BankTypes GetNameBankOfCurrentRegion()
        {
            DFLocation.ClimateSettings settings = MapsFile.GetWorldClimateSettings(climateSettings.WorldClimate);
            NameHelper.BankTypes bankType;
            switch (settings.Names)
            {
                case FactionFile.FactionRaces.Redguard:
                    bankType = NameHelper.BankTypes.Redguard;
                    break;
                case FactionFile.FactionRaces.Nord:
                    bankType = NameHelper.BankTypes.Nord;
                    break;
                case FactionFile.FactionRaces.DarkElf:
                    bankType = NameHelper.BankTypes.DarkElf;
                    break;
                case FactionFile.FactionRaces.WoodElf:
                    bankType = NameHelper.BankTypes.WoodElf;
                    break;
                default:
                case FactionFile.FactionRaces.Breton:
                    bankType = NameHelper.BankTypes.Breton;
                    break;
            }

            return bankType;
        }

        /// <summary>
        /// Gets the dominant race in player's current region.
        /// This seems to be based on subclimate rather than FACTION.TXT.
        /// The faction data has very little diversity and does not match observed race in many regions.
        /// </summary>
        public Races GetRaceOfCurrentRegion()
        {
            // Racial distribution in Daggerfall:
            //  * Desert, Desert2, Rainforest = Redguard
            //  * Mountain, MountainWoods = Nord
            //  * Swamp, Subtropical, Woodlands, Default = Breton

            DFLocation.ClimateSettings settings = MapsFile.GetWorldClimateSettings(climateSettings.WorldClimate);
            switch(settings.People)
            {
                case FactionFile.FactionRaces.Redguard:
                    return Races.Redguard;
                case FactionFile.FactionRaces.Nord:
                    return Races.Nord;
                case FactionFile.FactionRaces.Breton:
                default:
                    return Races.Breton;
            }
        }

        /// <summary>
        /// Gets the factionID for "people of region" in player's current region.
        /// </summary>
        public int GetPeopleOfCurrentRegion()
        {
            // Find people of current region
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(
                (int)FactionFile.FactionTypes.People,
                (int)FactionFile.SocialGroups.Commoners,
                (int)FactionFile.GuildGroups.GeneralPopulace,
                CurrentOneBasedRegionIndex);

            // Should always find a single people of
            if (factions == null || factions.Length != 1)
                throw new Exception("GetPeopleOfCurrentRegion() did not find exactly 1 match.");

            return factions[0].id;
        }

        /// <summary>
        /// Gets the factionID of player's current region.
        /// </summary>
        int GetCurrentRegionFaction()
        {
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(
                (int)FactionFile.FactionTypes.Province, -1, -1, CurrentOneBasedRegionIndex);

            // Should always find a single region
            if (factions == null || factions.Length != 1)
                throw new Exception("GetCurrentRegionFaction() did not find exactly 1 match.");

            return factions[0].id;
        }

        /// <summary>
        /// Gets the factionID of noble court in player's current region 
        /// </summary>
        int GetCourtOfCurrentRegion()
        {
            // Find court in current region
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(
                (int)FactionFile.FactionTypes.Courts,
                (int)FactionFile.SocialGroups.Nobility,
                (int)FactionFile.GuildGroups.Region,
                CurrentOneBasedRegionIndex);

            // Should always find a single court
            if (factions == null || factions.Length != 1)
                throw new Exception("GetCourtOfCurrentRegion() did not find exactly 1 match.");

            return factions[0].id;
        }

        /// <summary>
        /// Checks if player is inside a location world cell, optionally inside location rect, optionally outside
        /// </summary>
        /// <returns>True if player inside a township</returns>
        public bool IsPlayerInTown(bool mustBeInLocationRect = false, bool mustBeOutside = false)
        {
            // Check if player inside a town cell
            if (CurrentLocationType == DFRegion.LocationTypes.TownCity ||
                CurrentLocationType == DFRegion.LocationTypes.TownHamlet ||
                CurrentLocationType == DFRegion.LocationTypes.TownVillage ||
                CurrentLocationType == DFRegion.LocationTypes.HomeFarms ||
                CurrentLocationType == DFRegion.LocationTypes.HomeWealthy ||
                CurrentLocationType == DFRegion.LocationTypes.ReligionTemple)
            {
                // Optionally check if player inside location rect
                if (mustBeInLocationRect && !IsPlayerInLocationRect)
                    return false;

                // Optionally check if player outside
                if (mustBeOutside && GameManager.Instance.IsPlayerInside)
                    return false;

                return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        private void RaiseEvents()
        {
            // Region index changed
            if (CurrentRegionIndex != lastRegionIndex)
            {
                RaiseOnRegionIndexChangedEvent(CurrentRegionIndex);
                lastRegionIndex = CurrentRegionIndex;
            }

            // Climate index changed
            if (CurrentClimateIndex != lastClimateIndex)
            {
                RaiseOnClimateIndexChangedEvent(CurrentClimateIndex);
                lastClimateIndex = CurrentClimateIndex;
            }

            // Politic index changed
            if (CurrentPoliticIndex != lastPoliticIndex)
            {
                RaiseOnPoliticIndexChangedEvent(CurrentPoliticIndex);
                lastPoliticIndex = CurrentPoliticIndex;
            }
        }

        private void UpdateWorldInfo(int x, int y)
        {
            // Requires DaggerfallUnity to be ready
            if (!ReadyCheck())
                return;

            // Requires MAPS.BSA connection
            if (dfUnity.ContentReader.MapFileReader == null)
                return;

            // Get climate and politic data
            currentClimateIndex = dfUnity.ContentReader.MapFileReader.GetClimateIndex(x, y);
            currentPoliticIndex = dfUnity.ContentReader.MapFileReader.GetPoliticIndex(x, y);
            climateSettings = MapsFile.GetWorldClimateSettings(currentClimateIndex);
            if (currentPoliticIndex > 128)
                regionName = dfUnity.ContentReader.MapFileReader.GetRegionName(currentPoliticIndex - 128);
            else if (currentPoliticIndex == 64)
                regionName = "Ocean";
            else
                regionName = "Unknown";

            // Get region data
            currentRegion = dfUnity.ContentReader.MapFileReader.GetRegion(CurrentRegionIndex);

            // Get location data
            ContentReader.MapSummary mapSummary;
            if (dfUnity.ContentReader.HasLocation(x, y, out mapSummary))
            {
                currentLocation = dfUnity.ContentReader.MapFileReader.GetLocation(mapSummary.RegionIndex, mapSummary.MapIndex);
                hasCurrentLocation = true;
                CalculateWorldLocationRect();
            }
            else
            {
                currentLocation = new DFLocation();
                hasCurrentLocation = false;
                ClearWorldLocationRect();
            }

            // Get location type
            if (hasCurrentLocation)
            {
                if (currentRegion.MapTable == null)
                {
                    DaggerfallUnity.LogMessage(string.Format("PlayerGPS: Location {0} in region{1} has a null MapTable.", currentLocation.Name, currentLocation.RegionName));
                }
                else
                {
                    currentLocationType = currentRegion.MapTable[mapSummary.MapIndex].LocationType;
                }
            }
        }

        // Calculate location rect in world units
        private void CalculateWorldLocationRect()
        {
            if (!hasCurrentLocation)
                return;

            // Convert world coords to map pixel coords then back again
            // This finds the absolute SW origin of this map pixel in world coords
            DFPosition mapPixel = CurrentMapPixel;
            DFPosition worldOrigin = MapsFile.MapPixelToWorldCoord(mapPixel.X, mapPixel.Y);

            // Find tile offset point using same logic as terrain helper
            DFPosition tileOrigin = TerrainHelper.GetLocationTerrainTileOrigin(CurrentLocation);

            // Adjust world origin by tileorigin*2 in world units
            worldOrigin.X += (tileOrigin.X * 2) * MapsFile.WorldMapTileDim;
            worldOrigin.Y += (tileOrigin.Y * 2) * MapsFile.WorldMapTileDim;

            // Get width and height of location in world units
            int width = currentLocation.Exterior.ExteriorData.Width * MapsFile.WorldMapRMBDim;
            int height = currentLocation.Exterior.ExteriorData.Height * MapsFile.WorldMapRMBDim;

            // Set location rect in world coordinates
            locationWorldRectMinX = worldOrigin.X;
            locationWorldRectMaxX = worldOrigin.X + width;
            locationWorldRectMinZ = worldOrigin.Y;
            locationWorldRectMaxZ = worldOrigin.Y + height;
        }

        private void ClearWorldLocationRect()
        {
            locationWorldRectMinX = -1;
            locationWorldRectMaxX = -1;
            locationWorldRectMinZ = -1;
            locationWorldRectMaxZ = -1;
        }

        private void PlayerLocationRectCheck()
        {
            // Bail if no current location at this map pixel
            if (!hasCurrentLocation)
            {
                // Raise exit event if player was in location rect
                if (isPlayerInLocationRect)
                {
                    RaiseOnExitLocationRectEvent();
                }

                // Clear flag and exit
                isPlayerInLocationRect = false;
                return;
            }

            // Player can be inside a map pixel with location but not inside location rect
            // So check if player currently inside location rect
            bool check;
            if (WorldX >= locationWorldRectMinX && WorldX <= locationWorldRectMaxX &&
                WorldZ >= locationWorldRectMinZ && WorldZ <= locationWorldRectMaxZ)
            {
                check = true;
            }
            else
            {
                check = false;
            }

            // Call events based on location rect change
            if (check && !isPlayerInLocationRect)
            {
                // Player has entered location rect
                RaiseOnEnterLocationRectEvent(CurrentLocation);

                // Perform location discovery
                DiscoverCurrentLocation();
            }
            else if (!check && isPlayerInLocationRect)
            {
                // Player has left a location rect
                RaiseOnExitLocationRectEvent();
            }

            // Update last known state
            isPlayerInLocationRect = check;
        }

        private bool ReadyCheck()
        {
            // Ensure we have a DaggerfallUnity reference
            if (dfUnity == null)
            {
                dfUnity = DaggerfallUnity.Instance;
            }

            // Do nothing until DaggerfallUnity is ready
            if (!dfUnity.IsReady)
                return false;

            return true;
        }

        #endregion

        #region Location Discovery

        /// <summary>
        /// Discover current location.
        /// Does nothing if player in wilderness or location already dicovered.
        /// This is performed automatically by PlayerGPS when player enters a location rect.
        /// </summary>
        void DiscoverCurrentLocation()
        {
            // Must have a location loaded
            if (!CurrentLocation.Loaded)
                return;

            // Check if already discovered
            int mapPixelID = MapsFile.GetMapPixelIDFromLongitudeLatitude((int)CurrentLocation.MapTableData.Longitude, CurrentLocation.MapTableData.Latitude);
            if (HasDiscoveredLocation(mapPixelID))
                return;

            // Add to discovered locations dict
            DiscoveredLocation dl = new DiscoveredLocation();

            dl.mapID = CurrentLocation.MapTableData.MapId;
            dl.mapPixelID = mapPixelID;
            dl.regionName = CurrentLocation.RegionName;
            dl.locationName = CurrentLocation.Name;
            discoveredLocations.Add(mapPixelID, dl);
        }

        /// <summary>
        /// Discover location with regionName and locationName.
        /// </summary>
        public void DiscoverLocation(string regionName, string locationName)
        {
            DFLocation location;        
            bool found = dfUnity.ContentReader.GetLocation(regionName, locationName, out location);
            if (!found)
                throw new Exception(String.Format("Error finding location {0} : {1}", regionName, locationName));
            // Check if already discovered
            int mapPixelID = MapsFile.GetMapPixelIDFromLongitudeLatitude((int)location.MapTableData.Longitude, location.MapTableData.Latitude);
            if (HasDiscoveredLocation(mapPixelID))
                return;

            // Add to discovered locations dict
            DiscoveredLocation dl = new DiscoveredLocation();
            dl.mapID = location.MapTableData.MapId;
            dl.mapPixelID = mapPixelID;
            dl.regionName = location.RegionName;
            dl.locationName = location.Name;
            discoveredLocations.Add(mapPixelID, dl);
        }

        /// <summary>
        /// Discover the specified building in current location.
        /// Does nothing if player not inside a location or building already discovered.
        /// </summary>
        /// <param name="buildingKey"></param>
        public void DiscoverBuilding(int buildingKey)
        {
            // Must have a location loaded
            if (!CurrentLocation.Loaded)
                return;

            // Do nothing if building already discovered
            if (HasDiscoveredBuilding(buildingKey))
                return;

            // Get building information
            DiscoveredBuilding db;
            if (!GetBuildingDiscoveryData(buildingKey, out db))
                return;

            // Get location discovery
            int mapPixelID = MapsFile.GetMapPixelIDFromLongitudeLatitude((int)CurrentLocation.MapTableData.Longitude, CurrentLocation.MapTableData.Latitude);
            DiscoveredLocation dl = new DiscoveredLocation();
            if (discoveredLocations.ContainsKey(mapPixelID))
            {
                dl = discoveredLocations[mapPixelID];
            }

            // Ensure the building dict is created
            if (dl.discoveredBuildings == null)
                dl.discoveredBuildings = new Dictionary<int, DiscoveredBuilding>();

            // Add the building and store back to discovered location
            dl.discoveredBuildings.Add(db.buildingKey, db);
            discoveredLocations[mapPixelID] = dl;
        }

        /// <summary>
        /// Check if player has discovered location.
        /// MapPixelID is derived from longitude/latitude or MapPixelX, MapPixelY.
        /// See MapsFile.GetMapPixelID() and MapsFile.GetMapPixelIDFromLongitudeLatitude().
        /// </summary>
        /// <param name="mapPixelID">ID of location pixel.</param>
        /// <returns>True if already discovered.</returns>
        public bool HasDiscoveredLocation(int mapPixelID)
        {
            return discoveredLocations.ContainsKey(mapPixelID);
        }

        /// <summary>
        /// Check if player has discovered building in current location.
        /// </summary>
        /// <param name="buildingKey">Building key to check.</param>
        /// <returns>True if building discovered.</returns>
        public bool HasDiscoveredBuilding(int buildingKey)
        {
            // Must have a location loaded
            if (!CurrentLocation.Loaded)
                return false;

            // Must have discovered current location before building
            int mapPixelID = MapsFile.GetMapPixelIDFromLongitudeLatitude((int)CurrentLocation.MapTableData.Longitude, CurrentLocation.MapTableData.Latitude);
            if (!HasDiscoveredLocation(mapPixelID))
                return false;

            // Get the location discovery for this mapID
            DiscoveredLocation dl = discoveredLocations[mapPixelID];
            if (dl.discoveredBuildings == null)
                return false;

            return dl.discoveredBuildings.ContainsKey(buildingKey);
        }

        /// <summary>
        /// Gets discovered building data for current location.
        /// </summary>
        /// <param name="buildingKey">Building key in current location.</param>
        /// <param name="discoveredBuildingOut">Building discovery data out.</param>
        /// <returns>True if building discovered, false if building not discovered.</returns>
        public bool GetDiscoveredBuilding(int buildingKey, out DiscoveredBuilding discoveredBuildingOut)
        {
            discoveredBuildingOut = new DiscoveredBuilding();

            // Must have discovered building
            if (!HasDiscoveredBuilding(buildingKey))
                return false;

            // Get the location discovery for this mapID
            int mapPixelID = MapsFile.GetMapPixelIDFromLongitudeLatitude((int)CurrentLocation.MapTableData.Longitude, CurrentLocation.MapTableData.Latitude);
            DiscoveredLocation dl = discoveredLocations[mapPixelID];
            if (dl.discoveredBuildings == null)
                return false;

            // Get discovery data for building
            discoveredBuildingOut = dl.discoveredBuildings[buildingKey];

            return true;
        }

        /// <summary>
        /// Gets discovery information from any building in current location.
        /// Does not change discovery state, simply returns data as if building is always discovered.
        /// </summary>
        /// <param name="buildingKey">Building key in current location.</param>
        /// <param name="discoveredBuildingOut">Discovered building data.</param>
        /// <returns>True if successful.</returns>
        public bool GetAnyBuilding(int buildingKey, out DiscoveredBuilding discoveredBuildingOut)
        {
            discoveredBuildingOut = new DiscoveredBuilding();

            // Must have a location loaded
            if (!CurrentLocation.Loaded)
                return false;

            // Get building discovery data only
            if (GetBuildingDiscoveryData(buildingKey, out discoveredBuildingOut))
                return true;

            return false;
        }

        /// <summary>
        /// Gets discovery dictionary for save.
        /// </summary>
        public Dictionary<int, DiscoveredLocation> GetDiscoverySaveData()
        {
            return discoveredLocations;
        }

        /// <summary>
        /// Restores discovery dictionary for load.
        /// </summary>
        public void RestoreDiscoveryData(Dictionary<int, DiscoveredLocation> data)
        {
            discoveredLocations = data;

            // Purge any entries with MapPixelID of 0
            // These are from a previous save format keyed to MapID
            List<int> keysToRemove = new List<int>();
            foreach(var kvp in discoveredLocations)
            {
                if (kvp.Value.mapPixelID == 0)
                    keysToRemove.Add(kvp.Key);
            }

            // Remove legacy entries
            foreach(int key in keysToRemove)
            {
                discoveredLocations.Remove(key);
            }
        }

        /// <summary>
        /// Clear discovered locations.
        /// Intended to be used when loading an old save without discovery data.
        /// Otherwise live discovery state from previous session is retained.
        /// </summary>
        public void ClearDiscoveryData()
        {
            discoveredLocations.Clear();
        }

        /// <summary>
        /// Gets building information from current location.
        /// Does not change discovery state for building.
        /// </summary>
        /// <param name="buildingKey">Key of building to query.</param>
        bool GetBuildingDiscoveryData(int buildingKey, out DiscoveredBuilding buildingDiscoveryData)
        {
            buildingDiscoveryData = new DiscoveredBuilding();

            // Get building directory for location
            BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
            if (!buildingDirectory)
                return false;

            // Get detailed building data from directory
            BuildingSummary buildingSummary;
            if (!buildingDirectory.GetBuildingSummary(buildingKey, out buildingSummary))
            {
                int layoutX, layoutY, recordIndex;
                BuildingDirectory.ReverseBuildingKey(buildingKey, out layoutX, out layoutY, out recordIndex);
                Debug.LogFormat("Unable to find expected building key {0} in {1}.{2}", buildingKey, buildingDirectory.LocationData.RegionName, buildingDirectory.LocationData.Name);
                Debug.LogFormat("LayoutX={0}, LayoutY={1}, RecordIndex={2}", layoutX, layoutY, recordIndex);
                return false;
            }

            // Resolve name by building type
            string buildingName;
            if (RMBLayout.IsResidence(buildingSummary.BuildingType))
            {
                // Residence
                // TODO: Link to quest system active sites
                buildingName = HardStrings.residence;
            }
            else
            {
                // Fixed building name
                buildingName = BuildingNames.GetName(
                    buildingSummary.NameSeed,
                    buildingSummary.BuildingType,
                    buildingSummary.FactionId,
                    buildingDirectory.LocationData.Name,
                    buildingDirectory.LocationData.RegionName);
            }

            // Add to data
            buildingDiscoveryData.buildingKey = buildingKey;
            buildingDiscoveryData.displayName = buildingName;
            buildingDiscoveryData.factionID = buildingSummary.FactionId;
            buildingDiscoveryData.quality = buildingSummary.Quality;
            buildingDiscoveryData.buildingType = buildingSummary.BuildingType;

            return true;
        }

        #endregion

        #region Event Handlers

        // OnMapPixelChanged
        public delegate void OnMapPixelChangedEventHandler(DFPosition mapPixel);
        public static event OnMapPixelChangedEventHandler OnMapPixelChanged;
        protected virtual void RaiseOnMapPixelChangedEvent(DFPosition mapPixel)
        {
            if (OnMapPixelChanged != null)
                OnMapPixelChanged(mapPixel);
        }

        // OnRegionIndexChanged
        public delegate void OnRegionIndexChangedEventHandler(int regionIndex);
        public static event OnRegionIndexChangedEventHandler OnRegionIndexChanged;
        protected virtual void RaiseOnRegionIndexChangedEvent(int regionIndex)
        {
            if (OnRegionIndexChanged != null)
                OnRegionIndexChanged(regionIndex);
        }

        // OnClimateIndexChanged
        public delegate void OnClimateIndexChangedEventHandler(int climateIndex);
        public static event OnClimateIndexChangedEventHandler OnClimateIndexChanged;
        protected virtual void RaiseOnClimateIndexChangedEvent(int climateIndex)
        {
            if (OnClimateIndexChanged != null)
                OnClimateIndexChanged(climateIndex);
        }

        // OnPoliticIndexChanged
        public delegate void OnPoliticIndexChangedEventHandler(int politicIndex);
        public static event OnPoliticIndexChangedEventHandler OnPoliticIndexChanged;
        protected virtual void RaiseOnPoliticIndexChangedEvent(int politicIndex)
        {
            if (OnPoliticIndexChanged != null)
                OnPoliticIndexChanged(politicIndex);
        }

        // OnEnterLocationRect
        public delegate void OnEnterLocationRectEventHandler(DFLocation location);
        public static event OnEnterLocationRectEventHandler OnEnterLocationRect;
        protected virtual void RaiseOnEnterLocationRectEvent(DFLocation location)
        {
            if (OnEnterLocationRect != null)
                OnEnterLocationRect(location);
        }

        // OnExitLocationRect
        public delegate void OnExitLocationRectEventHandler();
        public static event OnExitLocationRectEventHandler OnExitLocationRect;
        protected virtual void RaiseOnExitLocationRectEvent()
        {
            if (OnExitLocationRect != null)
                OnExitLocationRect();
        }

        #endregion
    }
}