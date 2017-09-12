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

        const int legalReputationCount = 62;
        const int minReputation = -100;
        const int maxReputation = 100;

        // TEMP: Faction IDs for curated quest givers
        public const int fightersGuildQuestorFactionID = 851;
        public const int magesGuildQuestorFactionID = 63;

        Dictionary<int, FactionFile.FactionData> factionDict = new Dictionary<int, FactionFile.FactionData>();
        Dictionary<string, int> factionNameToIDDict = new Dictionary<string, int>();
        List<LegalReputation> legalReputationList = new List<LegalReputation>();

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

        #region Structs & Enums

        /// <summary>
        /// Legal reputation for regions.
        /// </summary>
        public struct LegalReputation
        {
            public int index;
            public int value;
            public string name;
        }

        #endregion

        #region Public Methods

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
        /// Gets legal reputation by region index.
        /// </summary>
        /// <param name="regionIndex">Region index 0 to 61.</param>
        /// <returns>Legal reputation for that region.</returns>
        public LegalReputation GetLegalReputation(int regionIndex)
        {
            if (regionIndex < 0 || regionIndex >= legalReputationCount)
                throw new IndexOutOfRangeException("GetLegalReputation regionIndex out of range");

            // Reset if no legal reputation data available
            if (legalReputationList.Count != legalReputationCount)
                ResetLegalReputation();

            return legalReputationList[regionIndex];
        }

        /// <summary>
        /// Change legal reputation value by amount.
        /// </summary>
        public int ChangeLegalReputation(int regionIndex, int amount)
        {
            if (regionIndex < 0 || regionIndex >= legalReputationCount)
                throw new IndexOutOfRangeException("ChangeLegalReputation regionIndex out of range");

            // Reset if no legal reputation data available
            if (legalReputationList.Count != legalReputationCount)
                ResetLegalReputation();

            LegalReputation lr = legalReputationList[regionIndex];
            lr.value = Mathf.Clamp(lr.value + amount, minReputation, maxReputation);
            legalReputationList[regionIndex] = lr;

            return lr.value;
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

            // Log message to see when faction data reset
            Debug.Log("PersistentFactionData.Reset() loaded fresh faction data.");
        }

        /// <summary>
        /// Resets legal reputation for all regions back to 0.
        /// Not fully implemented yet, still needs to be imported from SAVEVARS.DAT.
        /// Might move to own class later, but here is a good spot for now.
        /// </summary>
        public void ResetLegalReputation()
        {
            // Clear legal reputation across all regions
            legalReputationList.Clear();
            for (int i = 0; i < legalReputationCount; i++)
            {
                LegalReputation lr = new LegalReputation();
                lr.index = i;
                lr.value = 0;
                lr.name = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegionName(i);
                legalReputationList.Add(lr);
            }
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
            Reset();
            ResetLegalReputation();
        }

        #endregion
    }
}