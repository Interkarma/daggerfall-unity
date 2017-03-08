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

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// A location or site involved in a quest.
    /// A Place can be a random local/remote location or a fixed permanent location.
    /// </summary>
    public class Place
    {
        #region Fields

        string symbol;          // Symbol of place
        PlaceTypes placeType;   // Fixed/remote/local
        string name;            // Source name for data table
        int u1;                 // Unknown parameter 1
        int u2;                 // Unknown parameter 2
        int u3;                 // Unknown parameter 3

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
            get { return u1; }
        }

        /// <summary>
        /// Gets parameter 2 of Place.
        /// </summary>
        public int Param2
        {
            get { return u2; }
        }

        /// <summary>
        /// Gets parameter 3 of Place.
        /// </summary>
        public int Param3
        {
            get { return u3; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Place()
        {
        }

        /// <summary>
        /// Construct a Place resource from QBN input.
        /// </summary>
        /// <param name="line">Place definition line from QBN.</param>
        public Place(string line)
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
                    u1 = CustomParseInt(placesTable.GetValue("u1", name));
                    u2 = CustomParseInt(placesTable.GetValue("u2", name));
                    u3 = CustomParseInt(placesTable.GetValue("u3", name));
                }
                else
                {
                    throw new Exception(string.Format("Could not find place name in data table: '{0};", name));
                }

                // TODO: Now that symbol, type, name, and params have been resolved we can create a mapping to fixed location or generate mapping to random site.
                // This mapping will be used as an engine-specific place marker for Daggerfall Unity.
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

        #endregion
    }
}