// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Utility.ModSupport;

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

        Vector3 lastFramePosition;

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
            public int lastLockpickAttempt;
            public string customUserDisplayName;
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

        /// <summary>
        /// Data used when performing building name refresh.
        /// </summary>
        struct BuildingRenameOperation
        {
            public int buildingKey;
            public string displayName;
        };

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
            get {
                // Determine region from current politic index
                int result = 0;
                if (currentPoliticIndex == 64)
                    result = 31; // High Rock sea coast
                  else
                    result = currentPoliticIndex - 128;

                // Patch known bad value to Wrothgarian Mountains
                if (result == 105)
                    result = 16;

                // Clamp any out of range results to 0
                if (result < 0 || result >= 62)
                    result = 0;

                return result;
            }
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
        /// Gets non-localized current region name based on world position.
        /// IMPORTANT: This is used when matching regions for NPC knowledge in TalkManager and should not be localized.
        /// </summary>
        public string CurrentRegionName
        {
            get { return regionName; }
        }

        /// <summary>
        /// Gets localized current region name based on world position.
        /// This should only be used for display strings in UI.
        /// </summary>
        public string CurrentLocalizedRegionName
        {
            get { return TextManager.Instance.GetLocalizedRegionName(CurrentRegionIndex); }
        }

        public string CurrentLocalizedLocationName
        {
            get { return TextManager.Instance.GetLocalizedLocationName(currentLocation.MapTableData.MapId, currentLocation.Name); }
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
                UpdateWorldInfo(pos.X, pos.Y);
                RaiseOnMapPixelChangedEvent(pos);

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

        private void LateUpdate()
        {
            // Snap back to physical world boundary to prevent player running off edge of world
            // Setting to approx. 10000 inches (254 metres) in from edge so end of world not so visible
            if (WorldX < 10000 ||       // West
                WorldZ > 16370000 ||    // North
                WorldZ < 10000 ||       // South
                WorldX > 32750000)      // East
            {
                gameObject.transform.position = lastFramePosition;
            }

            // Record player's last frame position
            lastFramePosition = gameObject.transform.position;
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
        /// In practice this will always be Redguard/Breton.
        /// Supporting other name banks for possible diversity later.
        /// </summary>
        public NameHelper.BankTypes GetNameBankOfCurrentRegion()
        {
            if (GameManager.Instance.PlayerGPS.CurrentRegionIndex > -1)
                return (NameHelper.BankTypes) MapsFile.RegionRaces[GameManager.Instance.PlayerGPS.CurrentRegionIndex];

            return NameHelper.BankTypes.Breton;
        }

        /// <summary>
        /// Gets the dominant race in player's current region.
        /// </summary>
        public Races GetRaceOfCurrentRegion()
        {
            return (Races) MapsFile.RegionRaces[GameManager.Instance.PlayerGPS.CurrentRegionIndex] + 1;
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
        public int GetCurrentRegionFaction()
        {
            FactionFile.FactionData factionData;
            GameManager.Instance.PlayerEntity.FactionData.GetRegionFaction(CurrentRegionIndex, out factionData);
            return factionData.id;
        }

        /// <summary>
        /// Gets the factionID of noble court in player's current region 
        /// </summary>
        public int GetCourtOfCurrentRegion()
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

        public int GetCurrentRegionVampireClan()
        {
            FactionFile.FactionData factionData;
            GameManager.Instance.PlayerEntity.FactionData.GetRegionFaction(CurrentRegionIndex, out factionData);
            return factionData.vam;
        }

        /// <summary>
        /// Gets the dominant temple in player's current region.
        /// </summary>
        public int GetTempleOfCurrentRegion()
        {
            return MapsFile.RegionTemples[GameManager.Instance.PlayerGPS.CurrentRegionIndex];
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
        /// <param name="activeInHierarchy">Flag to get active or inactive objects.</param>
        /// <returns>NearbyObject list. Can be null or empty.</returns>
        public List<NearbyObject> GetNearbyObjects(NearbyObjectFlags flags, float maxRange = 14f, bool activeInHierarchy = true)
        {
            if (flags == NearbyObjectFlags.None)
                return null;

            var query =
                from no in nearbyObjects
                where ((no.flags & flags) == flags) && no.distance < maxRange && no.gameObject != null && no.gameObject.activeInHierarchy == activeInHierarchy
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
                regionName = TextManager.Instance.GetLocalizedText("ocean");
            else
                regionName = TextManager.Instance.GetLocalizedText("unknownUpper");

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
            int extraRect = 4096;

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
            // Virtual location rect check is extended by 4096 units (size of a full city block) around physical town border to better match classic
            bool check;
            if (WorldX >= locationWorldRectMinX - extraRect && WorldX <= locationWorldRectMaxX + extraRect &&
                WorldZ >= locationWorldRectMinZ - extraRect && WorldZ <= locationWorldRectMaxZ + extraRect)
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
                // Perform location discovery
                DiscoverCurrentLocation();

                // Player has entered location rect
                isPlayerInLocationRect = check;
                RaiseOnEnterLocationRectEvent(CurrentLocation);
            }
            else if (!check && isPlayerInLocationRect)
            {
                // Player has left a location rect
                isPlayerInLocationRect = check;
                RaiseOnExitLocationRectEvent();
            }
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

            // When running the game, wait for the mod manager to be initialized
            // Updating the player location too early causes the region loading to read WorldData
            // locations from disabled mods
            if (ModManager.Instance != null && !ModManager.Instance.Initialized)
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
                DFCareer.EnemyGroups enemyGroup = (entity.Entity as EnemyEntity).GetEnemyGroup();
                switch (enemyGroup)
                {
                    case DFCareer.EnemyGroups.Undead:
                        result |= NearbyObjectFlags.Undead;
                        break;
                    case DFCareer.EnemyGroups.Daedra:
                        result |= NearbyObjectFlags.Daedra;
                        break;
                    case DFCareer.EnemyGroups.Humanoid:
                        result |= NearbyObjectFlags.Humanoid;
                        break;
                    case DFCareer.EnemyGroups.Animals:
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
            if (!GetBaseBuildingDiscoveryData(buildingKey, out db))
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

            // check if building is used in quest (but only if no override name was provided (which will have priority))
            if (overrideName == null)
            {
                bool pcLearnedAboutExistence = false;
                bool receivedDirectionalHints = false;
                bool locationWasMarkedOnMapByNPC = false;
                string overrideBuildingName = string.Empty;
                if (GameManager.Instance.TalkManager.IsBuildingQuestResource(CurrentMapID, buildingKey, ref overrideBuildingName, ref pcLearnedAboutExistence, ref receivedDirectionalHints, ref locationWasMarkedOnMapByNPC))
                {
                    // if pc learned about building existance (was told the name) and quest building has (override) building name different than current building display name
                    if (pcLearnedAboutExistence && overrideBuildingName != db.displayName)
                        overrideName = overrideBuildingName; // set override name for use
                }
            }

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
        /// used to undiscover residences when they are a quest resource (named residence) when "add dialog" is done for this quest resource or on quest startup or on quest tombstone
        /// otherwise previously discovered residences will automatically show up on the automap when used in a quest
        /// </summary>
        /// <param name="buildingKey">Building key of building to be undiscovered</param>
        /// <param name="onlyIfResidence">gets undiscovered only if buildingType is residence</param>
        /// <param name="matchName">use a name for matching (only undiscover if building name matches matchName) - this is used if two quests "occupy" the same residence with different names, and one tries to hide residence on map but other quest's residence name was used and is still running</param>
        public void UndiscoverBuilding(int buildingKey, bool onlyIfResidence = false, string matchName = null)
        {
            // Must have a location loaded
            if (!CurrentLocation.Loaded)
                return;

            // Get location discovery
            int mapPixelID = MapsFile.GetMapPixelIDFromLongitudeLatitude((int)CurrentLocation.MapTableData.Longitude, CurrentLocation.MapTableData.Latitude);
            DiscoveredLocation dl = new DiscoveredLocation();
            if (discoveredLocations.ContainsKey(mapPixelID))
            {
                dl = discoveredLocations[mapPixelID];
            }

            if (dl.discoveredBuildings == null || !dl.discoveredBuildings.ContainsKey(buildingKey))
                return;

            DiscoveredBuilding db = dl.discoveredBuildings[buildingKey];

            // do nothing if only residences should be undiscovered but building is no residence
            if (onlyIfResidence && !RMBLayout.IsResidence(db.buildingType))
                return;

            // Do not undiscover residence if it's a Thieves Guild or Dark Brotherhood hideout
            if (db.factionID == (int)FactionFile.FactionIDs.The_Thieves_Guild ||
                db.factionID == (int)FactionFile.FactionIDs.The_Dark_Brotherhood)
                return;

            // do nothing if matchName was provided but matchName does not match displayName of building
            if (matchName != null && matchName != db.displayName)
                return;

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
        /// Does not change discovery state (NOTE: it kind of does for now with player house override but this will eventually be fixed)
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
        /// Sets custom name field for discovered building in current location.
        /// </summary>
        /// <param name="buildingKey">Building key in current location.</param>
        /// <param name="customName">Custom name of building. Set null or empty to remove custom name.</param>
        public void SetDiscoveredBuildingCustomName(int buildingKey, string customName)
        {
            DiscoveredBuilding discoveredBuilding;
            if (GetDiscoveredBuilding(buildingKey, out discoveredBuilding))
            {
                discoveredBuilding.customUserDisplayName = customName;
                UpdateDiscoveredBuilding(discoveredBuilding);
            }
        }

        /// <summary>
        /// Gets skill value of last lockpick attempt on this building in current location.
        /// </summary>
        /// <param name="buildingKey">Building key in current location.</param>
        public int GetLastLockpickAttempt(int buildingKey)
        {
            DiscoveredBuilding discoveredBuilding;
            if (!GetDiscoveredBuilding(buildingKey, out discoveredBuilding))
                return 0;

            return discoveredBuilding.lastLockpickAttempt;
        }

        /// <summary>
        /// Sets skill value at time of last lockpicking attempt after failure in current location.
        /// Player must increase skill past this value before they can try again.
        /// </summary>
        /// <param name="buildingKey">Building key in current location.</param>
        /// <param name="skillValue">Skill value at time attempt failed.</param>
        public void SetLastLockpickAttempt(int buildingKey, int skillValue)
        {
            DiscoveredBuilding discoveredBuilding;
            if (!GetDiscoveredBuilding(buildingKey, out discoveredBuilding))
                return;

            discoveredBuilding.lastLockpickAttempt = skillValue;
            UpdateDiscoveredBuilding(discoveredBuilding);
        }

        /// <summary>
        /// Refresh any changed non-residence building names in current location.
        /// Allows localized or otherwise changed building names to replace previously discovered building names.
        /// Will change names on buildings with a random NPC name.
        /// Does not affect player custom names or other properties of discovery data other than display name.
        /// </summary>
        public void RefreshBuildingNamesInCurrentLocation()
        {
            // Must have a location loaded
            if (!CurrentLocation.Loaded)
                return;

            // Get building directory for location
            BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
            if (!buildingDirectory)
                return;

            // Get discovered location
            int mapPixelID = MapsFile.GetMapPixelIDFromLongitudeLatitude((int)CurrentLocation.MapTableData.Longitude, CurrentLocation.MapTableData.Latitude);
            DiscoveredLocation dl = discoveredLocations[mapPixelID];
            if (dl.discoveredBuildings == null || dl.discoveredBuildings.Count == 0)
                return;

            // Enumerate changed building names
            List<BuildingRenameOperation> ops = new List<BuildingRenameOperation>();            
            foreach (DiscoveredBuilding db in dl.discoveredBuildings.Values)
            {
                // Get detailed building data from directory
                BuildingSummary buildingSummary;
                if (!buildingDirectory.GetBuildingSummary(db.buildingKey, out buildingSummary))
                    continue;

                // Ignore residences
                if (RMBLayout.IsResidence(buildingSummary.BuildingType))
                    continue;

                // Expand building name
                string displayName = BuildingNames.GetName(
                    buildingSummary.NameSeed,
                    buildingSummary.BuildingType,
                    buildingSummary.FactionId,
                    TextManager.Instance.GetLocalizedLocationName(buildingDirectory.LocationData.MapTableData.MapId, buildingDirectory.LocationData.Name),
                    TextManager.Instance.GetLocalizedRegionName(buildingDirectory.LocationData.RegionIndex));

                // Schedule name change
                if (!string.Equals(displayName, db.displayName))
                    ops.Add(new BuildingRenameOperation() { buildingKey = buildingSummary.buildingKey, displayName = displayName });
            }

            // Update building names
            foreach (BuildingRenameOperation op in ops)
            {
                DiscoveredBuilding discoveredBuilding;
                if (!GetDiscoveredBuilding(op.buildingKey, out discoveredBuilding))
                    return;

                Debug.LogFormat("Renaming '{0}' to '{1}'", discoveredBuilding.displayName, op.displayName);
                discoveredBuilding.displayName = op.displayName;
                UpdateDiscoveredBuilding(discoveredBuilding);
            }
        }

        /// <summary>
        /// Updates discovered building data in current location.
        /// </summary>
        /// <param name="discoveredBuilding">Updated data to write back to live discovery database.</param>
        void UpdateDiscoveredBuilding(DiscoveredBuilding discoveredBuildingIn)
        {
            // Must have discovered building
            if (!HasDiscoveredBuilding(discoveredBuildingIn.buildingKey))
                return;

            // Get the location discovery for this mapID
            int mapPixelID = MapsFile.GetMapPixelIDFromLongitudeLatitude((int)CurrentLocation.MapTableData.Longitude, CurrentLocation.MapTableData.Latitude);
            DiscoveredLocation dl = discoveredLocations[mapPixelID];
            if (dl.discoveredBuildings == null)
                return;

            // Replace discovery data for building
            dl.discoveredBuildings.Remove(discoveredBuildingIn.buildingKey);
            dl.discoveredBuildings.Add(discoveredBuildingIn.buildingKey, discoveredBuildingIn);
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

            // if found in discovered building data, return discovery information
            if (GetDiscoveredBuilding(buildingKey, out discoveredBuildingOut))
                return true;

            // if not try to get it from GetBaseBuildingDiscoveryData() function
            if (GetBaseBuildingDiscoveryData(buildingKey, out discoveredBuildingOut))
                return true;

            return false;
        }

        /// <summary>
        /// Gets discovery dictionary for save.
        /// </summary>
        public Dictionary<int, DiscoveredLocation> GetDiscoverySaveData()
        {
            RemoveUnnamedResidencesFromDiscoveryData();

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
            foreach (int key in keysToRemove)
            {
                discoveredLocations.Remove(key);
            }

            RemoveUnnamedResidencesFromDiscoveryData();
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
        /// Gets base building information from current location (no building name expansion). This is used as intermediate result by other functions like DiscoverBuilding and GetAnyBuilding
        /// Does not change discovery state for building.
        /// </summary>
        /// <param name="buildingKey">Key of building to query.</param>
        /// <param name="buildingDiscoveryData">[out] building discovery data of queried building</param>
        /// <returns>True if building information was found.</returns>
        bool GetBaseBuildingDiscoveryData(int buildingKey, out DiscoveredBuilding buildingDiscoveryData)
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

            // Add to data
            buildingDiscoveryData.buildingKey = buildingKey;
            if (RMBLayout.IsResidence(buildingSummary.BuildingType))
            {
                // Residence                
                buildingDiscoveryData.displayName = TextManager.Instance.GetLocalizedText("residence");
            }
            else
            {
                // Fixed building name
                buildingDiscoveryData.displayName = BuildingNames.GetName(
                    buildingSummary.NameSeed,
                    buildingSummary.BuildingType,
                    buildingSummary.FactionId,
                    buildingDirectory.LocationData.Name,
                    TextManager.Instance.GetLocalizedRegionName(buildingDirectory.LocationData.RegionIndex));
            }
            buildingDiscoveryData.factionID = buildingSummary.FactionId;
            buildingDiscoveryData.quality = buildingSummary.Quality;
            buildingDiscoveryData.buildingType = buildingSummary.BuildingType;

            return true;
        }

        /// <summary>
        /// this function uses two purposes:
        /// keep save data small and 
        /// get rid of discovered and stored residences in the past (old save games will have them) preventing named residences to show up correctly after code rework
        /// </summary>
        void RemoveUnnamedResidencesFromDiscoveryData()
        {
            foreach (var discoveredLocation in discoveredLocations)
            {
                List<int> keysToRemove = new List<int>();

                if (discoveredLocation.Value.discoveredBuildings != null)
                {
                    foreach (var discoveredBuilding in discoveredLocation.Value.discoveredBuildings)
                        if (discoveredBuilding.Value.displayName == TextManager.Instance.GetLocalizedText("residence"))
                            keysToRemove.Add(discoveredBuilding.Key);

                    foreach (int key in keysToRemove)
                    {
                        discoveredLocation.Value.discoveredBuildings.Remove(key);
                    }
                }
            }
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