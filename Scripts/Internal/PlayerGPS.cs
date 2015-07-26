// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;

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
            }

            // Raise other events
            RaiseEvents();

            // Check if player is inside actual location rect
            PlayerLocationRectCheck();
        }

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
            // This finds the SW origin of this map pixel in world coords
            DFPosition mapPixel = CurrentMapPixel;
            DFPosition worldOrigin = MapsFile.MapPixelToWorldCoord(mapPixel.X, mapPixel.Y);

            // Calculate centre point of this terrain area in world coords
            DFPosition centrePoint = new DFPosition(
                worldOrigin.X + (int)MapsFile.WorldMapTerrainDim / 2,
                worldOrigin.Y + (int)MapsFile.WorldMapTerrainDim / 2);

            // Get width and height of location in world units
            int width = currentLocation.Exterior.ExteriorData.Width * (int)MapsFile.WorldMapRMBDim;
            int height = currentLocation.Exterior.ExteriorData.Height * (int)MapsFile.WorldMapRMBDim;

            // Set true location rect in world coordinates
            locationWorldRectMinX = centrePoint.X - width / 2;
            locationWorldRectMaxX = centrePoint.X + width / 2;
            locationWorldRectMinZ = centrePoint.Y - height / 2;
            locationWorldRectMaxZ = centrePoint.Y + height / 2;
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
            if (!hasCurrentLocation)
            {
                if (isPlayerInLocationRect)
                {
                    RaiseOnExitLocationRectEvent();
                    isPlayerInLocationRect = false;
                }
                return;
            }

            // Check if player is inside current location rect
            if (WorldX >= locationWorldRectMinX && WorldX <= locationWorldRectMaxX &&
                WorldZ >= locationWorldRectMinZ && WorldZ <= locationWorldRectMaxZ)
            {
                //if (!isPlayerInLocationRect) Debug.Log("Player entered location rect of " + CurrentLocation.Name);
                isPlayerInLocationRect = true;
                RaiseOnEnterLocationRectEvent(CurrentLocation);
            }
            else
            {
                //if (isPlayerInLocationRect) Debug.Log("Player left location rect.");
                isPlayerInLocationRect = false;
                RaiseOnExitLocationRectEvent();
            }
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