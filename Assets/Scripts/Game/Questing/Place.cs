// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using DaggerfallWorkshop.Utility;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// A location or site involved in a quest.
    /// A Place can be a random local/remote location or a fixed permanent location.
    /// </summary>
    public class Place : QuestResource
    {
        #region Fields

        string symbol;          // Symbol of place
        PlaceTypes placeType;   // Fixed/remote/local
        string name;            // Source name for data table
        int p1;                 // Parameter 1
        int p2;                 // Parameter 2
        int p3;                 // Parameter 3

        DFLocation location;    // Location data

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
        /// Gets the symbol of this Place.
        /// </summary>
        public string Symbol
        {
            get { return symbol; }
        }

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
        /// True if location has been loaded and ready to use.
        /// </summary>
        public bool IsLocationLoaded
        {
            get { return location.Loaded; }
        }

        /// <summary>
        /// Gets region name of location.
        /// </summary>
        public string RegionName
        {
            get { return location.RegionName; }
        }

        /// <summary>
        /// Gets map name of location.
        /// </summary>
        public string MapName
        {
            get { return location.Name; }
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
            SetPlace(line);
        }

        #endregion

        #region Public Methods

        public void SetPlace(string line)
        {
            // Match string for Place variants
            string matchStr = @"Place (?<symbol>[a-zA-Z0-9_.]+) permanent (?<aPermanentPlace>[a-zA-Z0-9_.]+)|Place (?<symbol>[a-zA-Z0-9_.]+) remote (?<aRemoteSite>[a-zA-Z0-9_.]+)|Place (?<symbol>[a-zA-Z0-9_.]+) local (?<aLocalSite>[a-zA-Z0-9_.]+)";

            // Try to match source line with pattern
            Match match = Regex.Match(line, matchStr);
            if (match.Success)
            {
                // Store symbol for quest system
                symbol = match.Groups["symbol"].Value;

                // Get place type
                if (!string.IsNullOrEmpty(match.Groups["aPermanentPlace"].Value))
                {
                    // This is a permanent/fixed location
                    placeType = PlaceTypes.Fixed;
                    name = match.Groups["aPermanentPlace"].Value;
                }
                else if (!string.IsNullOrEmpty(match.Groups["aRemoteSite"].Value))
                {
                    // This is a remote site/building
                    placeType = PlaceTypes.Remote;
                    name = match.Groups["aRemoteSite"].Value;
                }
                else if (!string.IsNullOrEmpty(match.Groups["aLocalSite"].Value))
                {
                    // This is a local site/building
                    placeType = PlaceTypes.Local;
                    name = match.Groups["aLocalSite"].Value;
                }
                else
                {
                    throw new Exception(string.Format("No Place type match found for source: '{0}'", line));
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

                // Handle fixed location, either exterior or dungeon
                if (placeType == PlaceTypes.Fixed && p1 > 0xc300)
                {
                    SetupFixedLocation();
                }

                // TODO: Handle local and remote locations
            }
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
        /// Setup a fixed location.
        /// </summary>
        void SetupFixedLocation()
        {
            // Dungeon interiors have p2 > 0xfa00, exteriors have p2 = 0x01
            // Need to subtract 1 if inside dungeon for exterior mapid
            int locationId = -1;
            if (p2 > 0xfa00)
                locationId = p1 - 1;
            else
                locationId = p1;

            // Get location
            if (!DaggerfallUnity.Instance.ContentReader.GetQuestLocation(locationId, out location))
            {
                Debug.LogFormat("Could not find locationId: '{0};", locationId);
            }
        }

        #endregion
    }
}