// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

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
        int currentClimate;
        int currentPolitic;
        DFLocation currentLocation;
        DFLocation.ClimateSettings climateSettings;
        string regionName;

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
        public int CurrentClimate
        {
            get { return currentClimate; }
        }

        /// <summary>
        /// Gets political index based on player world position.
        /// </summary>
        public int CurrentPolitic
        {
            get { return currentPolitic; }
        }

        /// <summary>
        /// Gets region index based on player world position.
        /// </summary>
        public int CurrentRegion
        {
            get { return currentPolitic - 128; }
        }

        /// <summary>
        /// Gets climate properties based on player world position.
        /// </summary>
        public DFLocation.ClimateSettings ClimateSettings
        {
            get { return climateSettings; }
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
        /// Gets current region name based on world position.
        /// </summary>
        public string CurrentRegionName
        {
            get { return regionName; }
        }

        void Start()
        {
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
                lastMapPixelX = pos.X;
                lastMapPixelY = pos.Y;
            }
        }

        #region Private Methods

        private void UpdateWorldInfo(int x, int y)
        {
            // Get climate and politic data
            currentClimate = dfUnity.ContentReader.MapFileReader.GetClimateIndex(x, y);
            currentPolitic = dfUnity.ContentReader.MapFileReader.GetPoliticIndex(x, y);
            climateSettings = MapsFile.GetWorldClimateSettings(currentClimate);
            if (currentPolitic > 128)
                regionName = dfUnity.ContentReader.MapFileReader.GetRegionName(currentPolitic - 128);
            else if (currentPolitic == 64)
                regionName = "Ocean";
            else
                regionName = "Unknown";

            // Get location data
            ContentReader.MapSummary mapSummary;
            if (dfUnity.ContentReader.HasLocation(x, y, out mapSummary))
            {
                currentLocation = dfUnity.ContentReader.MapFileReader.GetLocation(mapSummary.RegionIndex, mapSummary.MapIndex);
            }
            else
            {
                currentLocation = new DFLocation();
            }
        }

        private bool ReadyCheck()
        {
            // Ensure we have a DaggerfallUnity reference
            if (dfUnity == null)
            {
                dfUnity = DaggerfallUnity.Instance;
            }

            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady)
            {
                DaggerfallUnity.LogMessage("PlayerGPS: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            return true;
        }

        #endregion
    }
}