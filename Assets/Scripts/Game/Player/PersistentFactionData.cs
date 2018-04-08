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
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Save;

namespace DaggerfallWorkshop.Game.Player
{
    /// <summary>
    /// Persistent runtime faction data is instanstiated from FACTION.TXT at startup.
    /// This data represents player's ongoing relationship with factions as game evolves.
    /// Actions which influence faction standing will modify items in this tree.
    /// This data will be reset with new character or saved/loaded with existing character.
    /// Save/Load is handled by SerializablePlayer and SaveLoadManager.
    /// </summary>
    public class PersistentFactionData
    {
        #region Fields

        const int minReputation = -100;
        const int maxReputation = 100;

        // TEMP: Faction IDs for curated quest givers
        public const int fightersGuildQuestorFactionID = 851;
        public const int magesGuildQuestorFactionID = 63;

        Dictionary<int, FactionFile.FactionData> factionDict = new Dictionary<int, FactionFile.FactionData>();
        Dictionary<string, int> factionNameToIDDict = new Dictionary<string, int>();

        #endregion

        #region Properties

        public Dictionary<int, FactionFile.FactionData> FactionDict
        {
            get { return factionDict; }
            set { factionDict = value; }
        }

        public Dictionary<string, int> FactionNameToIDDict
        {
            get { return factionNameToIDDict; }
            set { factionNameToIDDict = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds any registered custom factions into existing save data or new games
        /// </summary>
        public void AddCustomFactions()
        {
            foreach (int id in FactionFile.CustomFactions.Keys)
            {
                if (!factionDict.ContainsKey(id))
                {
                    factionDict.Add(id, FactionFile.CustomFactions[id]);
                    factionNameToIDDict.Add(FactionFile.CustomFactions[id].name, id);
                }
            }
        }

        /// <summary>
        /// Gets faction data from faction ID.
        /// </summary>
        /// <param name="factionID">Faction ID.</param>
        /// <param name="factionDataOut">Receives faction data.</param>
        /// <returns>True if successful.</returns>
        public bool GetFactionData(int factionID, out FactionFile.FactionData factionDataOut)
        {
            // Reset if no faction data available
            if (factionDict.Count == 0)
                Reset();

            // Try to get requested faction
            factionDataOut = new FactionFile.FactionData();
            if (factionDict.ContainsKey(factionID))
            {
                factionDataOut = factionDict[factionID];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Finds all faction data matching the search parameters.
        /// Specify -1 to ignore a parameter. If all params are -1 then all regions are returned.
        /// </summary>
        /// <param name="type">Type to match.</param>
        /// <param name="socialGroup">Social Group to match.</param>
        /// <param name="guildGroup">Guild group to match.</param>
        /// <param name="oneBasedRegionIndex">Region index to match. Must be ONE-BASED region index used by FACTION.TXT.</param>
        /// <returns>FactionData[] array.</returns>
        public FactionFile.FactionData[] FindFactions(int type = -1, int socialGroup = -1, int guildGroup = -1, int oneBasedRegionIndex = -1)
        {
            List<FactionFile.FactionData> factionDataList = new List<FactionFile.FactionData>();

            // Match faction items
            foreach(FactionFile.FactionData item in factionDict.Values)
            {
                bool match = true;

                // Validate type if specified
                if (type != -1 && type != item.type)
                    match = false;

                // Validate socialGroup if specified
                if (socialGroup != -1 && socialGroup != item.sgroup)
                    match = false;

                // Validate guildGroup if specified
                if (guildGroup != -1 && guildGroup != item.ggroup)
                    match = false;

                // Validate regionIndex if specified
                if (oneBasedRegionIndex != -1 && oneBasedRegionIndex != item.region)
                    match = false;

                // Store if a match found
                if (match)
                    factionDataList.Add(item);
            }

            return factionDataList.ToArray();
        }

        /// <summary>
        /// Finds faction of the given type and for the given region.
        /// If a match for both is not found, the last faction that matched type and had -1 for region is returned.
        /// A function like this exists in classic and is used in a number of places.
        /// One known use case is for finding the region factions. If no specific
        /// faction exists for the region, Random Ruler (region -1) is returned.
        /// </summary>
        /// <param name="type">Type to match.</param>
        /// <param name="oneBasedRegionIndex">Region index to match. Must be ONE-BASED region index used by FACTION.TXT.</param>
        /// <param name="factionDataOut">Receives faction data.</param>
        /// <returns>True if successful.</returns>
        public bool FindFactionByTypeAndRegion(int type, int oneBasedRegionIndex, out FactionFile.FactionData factionDataOut)
        {
            bool foundPartialMatch = false;
            factionDataOut = new FactionFile.FactionData();
            FactionFile.FactionData partialMatch = new FactionFile.FactionData();

            // Match faction items
            foreach (FactionFile.FactionData item in factionDict.Values)
            {
                if (type == item.type && oneBasedRegionIndex == item.region)
                {
                    factionDataOut = item;
                    return true;
                }
                else if (type == item.type && item.region == -1)
                {
                    foundPartialMatch = true;
                    partialMatch = item;
                }
            }

            if (foundPartialMatch)
            {
                factionDataOut = partialMatch;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets faction ID from name. Experimental.
        /// </summary>
        /// <param name="name">Name of faction to get ID of.</param>
        /// <returns>Faction ID if name found, otherwise -1.</returns>
        public int GetFactionID(string name)
        {
            if (factionNameToIDDict.ContainsKey(name))
                return factionNameToIDDict[name];

            return -1;
        }

        /// <summary>
        /// Resets faction state back to starting point from FACTION.TXT.
        /// </summary>
        public void Reset()
        {
            // Get base faction data
            FactionFile factionFile = DaggerfallUnity.Instance.ContentReader.FactionFileReader;
            if (factionFile == null)
                throw new Exception("PersistentFactionData.Reset() unable to load faction file reader.");

            // Get dictionaries
            factionDict = factionFile.FactionDict;
            factionNameToIDDict = factionFile.FactionNameToIDDict;

            // Add any registered custom factions
            AddCustomFactions();

            // Log message to see when faction data reset
            Debug.Log("PersistentFactionData.Reset() loaded fresh faction data.");
        }

        #endregion

        #region Reputation

        public void ImportClassicReputation(SaveVars saveVars)
        {
            // Get faction reader
            FactionFile factionFile = DaggerfallUnity.Instance.ContentReader.FactionFileReader;
            if (factionFile == null)
                throw new Exception("PersistentFactionData.ImportClassicReputation() unable to load faction file reader.");

            // Assign new faction dict
            factionDict = factionFile.Merge(saveVars);
            Debug.Log("Imported faction data from classic save.");
        }

        /// <summary>
        /// Gets reputation value.
        /// </summary>
        public int GetReputation(int factionID)
        {
            if (factionDict.ContainsKey(factionID))
            {
                FactionFile.FactionData factionData = factionDict[factionID];
                return factionData.rep;
            }

            return 0;
        }

        /// <summary>
        /// Set reputation to a specific value.
        /// </summary>
        public bool SetReputation(int factionID, int value)
        {
            if (factionDict.ContainsKey(factionID))
            {
                FactionFile.FactionData factionData = factionDict[factionID];
                factionData.rep = Mathf.Clamp(value, minReputation, maxReputation);
                factionDict[factionID] = factionData;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Change reputation value by amount.
        /// </summary>
        public bool ChangeReputation(int factionID, int amount, bool propagate = false)
        {
            if (factionDict.ContainsKey(factionID))
            {
                FactionFile.FactionData factionData = factionDict[factionID];
                factionData.rep = Mathf.Clamp(factionData.rep + amount, minReputation, maxReputation);
                factionDict[factionID] = factionData;
                return true;
            }

            if (propagate)
            {
                // TODO: Propagate reputation changes to related factions
            }

            return false;
        }

        /// <summary>
        /// Reset all reputations and legal reputations back to 0 (and resets from FACTION.TXT).
        /// </summary>
        public void ZeroAllReputations()
        {
            // Reset faction reputations
            Reset();

            // Reset legal reputations
            Entity.PlayerEntity player = GameManager.Instance.PlayerEntity;
            for (int i = 0; i < player.RegionData.Length; i++)
            {
                player.RegionData[i].LegalRep = 0;
            }
        }

        #endregion
    }
}