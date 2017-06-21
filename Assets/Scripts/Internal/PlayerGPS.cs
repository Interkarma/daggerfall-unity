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

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Tracks player position in world space.
    /// </summary>
    public class PlayerGPS : MonoBehaviour
    {
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
            get { return currentPoliticIndex - 128; }
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
        /// Due to limited use of races in FACTION.TXT this is either Redguard or Breton.
        /// </summary>
        public NameHelper.BankTypes GetNameBankOfCurrentRegion()
        {
            NameHelper.BankTypes bankType;
            switch (GameManager.Instance.PlayerGPS.GetRaceOfCurrentRegion())
            {
                case Races.Redguard:
                    bankType = NameHelper.BankTypes.Redguard;
                    break;

                default:
                case Races.Breton:
                    bankType = NameHelper.BankTypes.Breton;
                    break;
            }

            return bankType;
        }

        /// <summary>
        /// Gets the dominant race in player's current region.
        /// Due to limited use of races in FACTION.TXT this is either Redguard or Breton.
        /// </summary>
        public Races GetRaceOfCurrentRegion()
        {
            // Get faction of current region
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(
                (int)FactionFile.FactionTypes.Province,
                -1,
                -1,
                CurrentOneBasedRegionIndex);

            // Should always find a single province faction
            if (factions == null || factions.Length != 1)
                throw new Exception("GetRaceOfCurrentRegion() did not find exactly 1 match.");

            // Convert faction race to a race template ID
            switch ((FactionFile.FactionRaces)factions[0].race)
            {
                case FactionFile.FactionRaces.Redguard:
                    return Races.Redguard;

                // All other factions are Breton for now
                default:
                case FactionFile.FactionRaces.Breton:
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