// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
using System.Linq;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Tracks player position in virtual world space.
    /// Provides information about world around the player.
    /// </summary>
    public class PlayerGPS : MonoBehaviour
    {
        #region Fields

        const float refreshNearbyObjectsInterval = 0.33f;

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

        string locationRevealedByMapItem;

        float nearbyObjectsUpdateTimer = 0f;
        List<NearbyObject> nearbyObjects = new List<NearbyObject>();

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
            public string oldDisplayName;
            public bool isOverrideName;
            public int factionID;
            public int quality;
            public DFLocation.BuildingTypes buildingType;
        }

        public struct NearbyObject
        {
            public GameObject gameObject;
            public NearbyObjectFlags flags;
            public float distance;
        }

        [Flags]
        public enum NearbyObjectFlags
        {
            None = 0,
            Enemy = 1,
            Treasure = 2,
            Magic = 4,
            Undead = 8,
            Daedra = 16,
            Humanoid = 32,
            Animal = 64,
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

        /// <summary>
        /// The name of the last location revealed by a map item. Used for %map macro.
        /// </summary>
        public string LocationRevealedByMapItem
        {
            get { return locationRevealedByMapItem; } set { locationRevealedByMapItem = value; }
        }

        #endregion

        #region Constructors

        public PlayerGPS()
        {
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            DaggerfallTravelPopUp.OnPostFastTravel += DaggerfallTravelPopUp_OnPostFastTravel;
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

                // Clear non-permanent scenes from cache, unless going to/from owned ship
                DFPosition shipCoords = DaggerfallBankManager.GetShipCoords();
                if (shipCoords == null || (!(pos.X == shipCoords.X && pos.Y == shipCoords.Y) && !(lastMapPixelX == shipCoords.X && lastMapPixelY == shipCoords.Y)))
                    SaveLoadManager.ClearSceneCache(false);

                lastMapPixelX = pos.X;
                lastMapPixelY = pos.Y;
            }

            // Raise other events
            RaiseEvents();

            // Check if player is inside actual location rect
            PlayerLocationRectCheck();

            // Update nearby objects
            nearbyObjectsUpdateTimer += Time.deltaTime;
            if (nearbyObjectsUpdateTimer > refreshNearbyObjectsInterval)
            {
                UpdateNearbyObjects();
                nearbyObjectsUpdateTimer = 0;
            }
        }

        #endregion

        #region Private Methods

        private void StartGameBehaviour_OnNewGame()
        {
            // Reset state when loading a new game
            ResetState();
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            // Reset state when starting a new load process
            ResetState();
        }

        private void DaggerfallTravelPopUp_OnPostFastTravel()
        {
            // Reset state after fast travelling
            ResetState();
        }

        void ResetState()
        {
            isPlayerInLocationRect = false;
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
                CurrentRegionIndex);

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
                (int)FactionFile.FactionTypes.Province, -1, -1, CurrentRegionIndex);

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
                CurrentRegionIndex);

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
                CurrentLocationType == DFRegion.LocationTypes.Tavern ||
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

        /// <summary>
        /// Gets nearby objects matching flags and within maxRange.
        /// Can be used as needed, does not trigger a scene search.
        /// This only searches pre-populated list of nearby objects which is updated at low frequency.
        /// </summary>
        /// <param name="flags">Flags to search for.</param>
        /// <param name="maxRange">Max range for search. Not matched to classic range at this time.</param>
        /// <returns>NearbyObject list. Can be null or empty.</returns>
        public List<NearbyObject> GetNearbyObjects(NearbyObjectFlags flags, float maxRange = 14f)
        {
            if (flags == NearbyObjectFlags.None)
                return null;

            var query =
                from no in nearbyObjects
                where ((no.flags & flags) == flags) && no.distance < maxRange
                select no;

            return query.ToList();
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
            if (currentPoliticIndex >= 128)
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

        public bool ReadyCheck()
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

        #region Nearby Objects

        /// <summary>
        /// Refresh list of nearby objects to service related systems.
        /// </summary>
        void UpdateNearbyObjects()
        {
            nearbyObjects.Clear();

            // Get entities
            DaggerfallEntityBehaviour[] entities = FindObjectsOfType<DaggerfallEntityBehaviour>();
            if (entities != null)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    if (entities[i] == GameManager.Instance.PlayerEntityBehaviour)
                        continue;

                    NearbyObject no = new NearbyObject()
                    {
                        gameObject = entities[i].gameObject,
                        distance = Vector3.Distance(transform.position, entities[i].transform.position),
                    };

                    no.flags = GetEntityFlags(entities[i]);
                    nearbyObjects.Add(no);
                }
            }

            // Get treasure - this assumes loot containers will never carry entity component
            DaggerfallLoot[] lootContainers = FindObjectsOfType<DaggerfallLoot>();
            if (lootContainers != null)
            {
                for (int i = 0; i < lootContainers.Length; i++)
                {
                    NearbyObject no = new NearbyObject()
                    {
                        gameObject = lootContainers[i].gameObject,
                        distance = Vector3.Distance(transform.position, lootContainers[i].transform.position),
                    };

                    no.flags = GetLootFlags(lootContainers[i]);
                    nearbyObjects.Add(no);
                }
            }
        }

        public NearbyObjectFlags GetEntityFlags(DaggerfallEntityBehaviour entity)
        {
            NearbyObjectFlags result = NearbyObjectFlags.None;
            if (!entity)
                return result;

            if (entity.EntityType == EntityTypes.EnemyClass || entity.EntityType == EntityTypes.EnemyMonster)
            {
                result |= NearbyObjectFlags.Enemy;
                EnemyEntity enemyEntity = entity.Entity as EnemyEntity;
                switch (enemyEntity.MobileEnemy.Affinity)
                {
                    case MobileAffinity.Undead:
                        result |= NearbyObjectFlags.Undead;
                        break;
                    case MobileAffinity.Daedra:
                        result |= NearbyObjectFlags.Daedra;
                        break;
                    case MobileAffinity.Human:
                        result |= NearbyObjectFlags.Humanoid;
                        break;
                    case MobileAffinity.Animal:
                        result |= NearbyObjectFlags.Animal;
                        break;
                }
            }
            else if (entity.EntityType == EntityTypes.CivilianNPC)
            {
                result |= NearbyObjectFlags.Humanoid;
            }

            // Set magic flag
            // Not completely sure what conditions should flag entity for "detect magic"
            // Currently just assuming entity has active effects
            EntityEffectManager manager = entity.GetComponent<EntityEffectManager>();
            if (manager && manager.EffectCount > 0)
            {
                result |= NearbyObjectFlags.Magic;
            }

            return result;
        }

        public NearbyObjectFlags GetLootFlags(DaggerfallLoot loot)
        {
            NearbyObjectFlags result = NearbyObjectFlags.None;
            if (!loot)
                return result;

            // Set treasure flag when container not empty
            // Are any other conditions required?
            // Should corspes loot container be filtered out?
            if (loot.Items.Count > 0)
            {
                result |= NearbyObjectFlags.Treasure;
            }

            return result;
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

        public DFLocation DiscoverRandomLocation()
        {
            // Get all undiscovered locations that exist in the current region
            List<int> undiscoveredLocIdxs = new List<int>();
            for (int i = 0; i < currentRegion.LocationCount; i++)
                if (currentRegion.MapTable[i].Discovered == false && !HasDiscoveredLocation(currentRegion.MapTable[i].MapId & 0x000fffff))
                    undiscoveredLocIdxs.Add(i);

            // If there aren't any left, there's nothing to find. Classic will just keep returning a particular location over and over if this happens.
            if (undiscoveredLocIdxs.Count == 0)
                return new DFLocation();

            // Choose a random location and discover it
            int locIdx = UnityEngine.Random.Range(0, undiscoveredLocIdxs.Count);
            DFLocation location = dfUnity.ContentReader.MapFileReader.GetLocation(CurrentRegionIndex, undiscoveredLocIdxs[locIdx]);
            DiscoverLocation(CurrentRegionName, location.Name);
            return location;
        }

        /// <summary>
        /// Discover the specified building in current location.
        /// Does nothing if player not inside a location or building already discovered.
        /// </summary>
        /// <param name="buildingKey">Building key of building to be discovered</param>
        /// <param name="overrideName">If provided, ignore previous discovery and override the name</param>
        public void DiscoverBuilding(int buildingKey, string overrideName = null)
        {
            // Ensure current location also discovered before processing building
            DiscoverCurrentLocation();

            // Must have a location loaded
            if (!CurrentLocation.Loaded)
                return;

            // Do nothing if building already discovered, unless overriding name
            if (overrideName == null && HasDiscoveredBuilding(buildingKey))
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

            // Add the building and store back to discovered location, overriding name if requested
            if (overrideName != null)
            {
                if (!db.isOverrideName)
                    db.oldDisplayName = db.displayName;
                db.displayName = overrideName;
                db.isOverrideName = true;
            }

            if (db.oldDisplayName == db.displayName)
                db.isOverrideName = false;

            dl.discoveredBuildings[db.buildingKey] = db;
            discoveredLocations[mapPixelID] = dl;
        }

        /// <summary>
        /// Undiscover the specified building in current location.
        /// used to undiscover residences when they are a quest resource (named residence) when "add dialog" is done for this quest resource or on quest startup
        /// otherwise previously discovered residences will automatically show up on the automap when used in a quest
        /// </summary>
        /// <param name="buildingKey">Building key of building to be undiscovered</param>
        /// <param name="onlyIfResidence">gets undiscovered only if buildingType is residence</param>
        public void UndiscoverBuilding(int buildingKey, bool onlyIfResidence = false)
        {
            // Must have a location loaded
            if (!CurrentLocation.Loaded)
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

            if (onlyIfResidence && !RMBLayout.IsResidence(db.buildingType))
                return;

            if (dl.discoveredBuildings.ContainsKey(db.buildingKey))
                dl.discoveredBuildings.Remove(db.buildingKey);
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

            // Check if name should be overridden (owned house / quest site)
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            if (DaggerfallBankManager.IsHouseOwned(buildingKey))
                discoveredBuildingOut.displayName = HardStrings.playerResidence.Replace("%s", playerEntity.Name);

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
                buildingName = HardStrings.residence;

                // Link to quest system active sites
                // note Nystul: do this via TalkManager, this might seem odd at first glance but there is a reason to do so:
                //              get info from TalkManager if pc learned about existence of the building (i.e. its name)
                //              either through dialog ("add dialog" or by dialog-link) or quest (quest did not hide location via "dialog link" command)
                bool pcLearnedAboutExistence = false;
                bool receivedDirectionalHints = false;
                bool locationWasMarkedOnMapByNPC = false;
                string overrideBuildingName = string.Empty;
                if (GameManager.Instance.TalkManager.IsBuildingQuestResource(buildingSummary.buildingKey, ref overrideBuildingName, ref pcLearnedAboutExistence, ref receivedDirectionalHints, ref locationWasMarkedOnMapByNPC))
                {
                    if (pcLearnedAboutExistence)
                        buildingName = overrideBuildingName;
                }
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