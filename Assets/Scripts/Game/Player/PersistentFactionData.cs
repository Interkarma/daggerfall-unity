// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Game.Guilds;

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
        const int minPower = 1;
        const int maxPower = 100;

        static readonly HashSet<GuildNpcServices> questorIds = new HashSet<GuildNpcServices>()
            { GuildNpcServices.MG_Quests, GuildNpcServices.FG_Quests, GuildNpcServices.TG_Quests, GuildNpcServices.DB_Quests, GuildNpcServices.T_Quests, GuildNpcServices.KO_Quests };

        Dictionary<int, FactionFile.FactionData> factionDict = new Dictionary<int, FactionFile.FactionData>();
        Dictionary<string, int> factionNameToIDDict = new Dictionary<string, int>();

        // Regions bordering each region. In FALL.EXE at 0x1B5C68. Includes unused region "Bjoulsae River".
        // This data is used in a strange way. The order is different from the faction IDs of regions in the released game
        // (which is the order by which it is accessed), so regions access borders for seemingly unrelated regions.
        // Those regions are treated as permanent faction enemies, if they are already an enemy, until a war between the two factions has finished.
        // Maybe regions were supposed to access their own bordering regions.
        // In classic the access is off by one (due to FACTION.TXT's region IDs being 1-based and the call not taking that into account),
        // so the first entry (Alik'r Desert's border regions) is unused and the last region (Cybiades) uses out-of-range data.

                                                                             // Borders of:
        readonly byte[] borderRegions = { 44, 45, 47, 21, 56, 48, 49,  2, 55, 12, 57, // Alik'r Desert
                                  1, 49, 55, 12, 54, 53, 52, 50, 23, 10,  0, // Dragontail Mountains
                                 22, 40, 39, 17, 38, 37, 10,  0,  0,  0,  0, // Dwynnen
                                  6, 37, 36, 35, 34, 24, 51, 23,  2,  0,  0, // Isle of Balfiera
                                  1, 55,  2,  0,  0,  0,  0,  0,  0,  0,  0, // Dak'fron
                                 24, 53, 58, 51,  0,  0,  0,  0,  0,  0,  0, // Bjoulsae River
                                 39,  6, 38, 36, 35, 34, 27, 24, 58,  0,  0, // Wrothgarian Mountains
                                 20, 59, 19, 43, 61,  0,  0,  0,  0,  0,  0, // Daggerfall
                                 18, 59, 60, 33, 61,  0,  0,  0,  0,  0,  0, //
                                 18, 59,  0,  0,  0,  0,  0,  0,  0,  0,  0, // Betony
                                 47,  1, 56, 48, 62,  0,  0,  0,  0,  0,  0, //
                                 43, 42, 40,  6,  0,  0,  0,  0,  0,  0,  0, // Anticlere
                                  2, 50, 52, 51, 10,  0,  0,  0,  0,  0,  0, //
                                 16, 58, 17, 27, 34, 10, 53, 51,  0,  0,  0, //
                                 17, 34, 24,  0,  0,  0,  0,  0,  0,  0,  0, //
                                 60, 19, 61, 41,  0,  0,  0,  0,  0,  0,  0, //
                                 10, 35, 17, 27, 24,  0,  0,  0,  0,  0,  0, // Menevia
                                 10, 36, 17, 34,  0,  0,  0,  0,  0,  0,  0, // Alcaire
                                 10, 37, 38, 17, 35,  0,  0,  0,  0,  0,  0, // Koegria
                                 10,  6, 38, 36,  0,  0,  0,  0,  0,  0,  0, // Bhoriane
                                 37,  6, 17, 36,  0,  0,  0,  0,  0,  0,  0, // Kambria
                                 41, 40,  6, 17,  0,  0,  0,  0,  0,  0,  0, // Phrygias
                                 22, 42, 41, 39,  6,  0,  0,  0,  0,  0,  0,
                                 33, 61, 42, 40, 39,  0,  0,  0,  0,  0,  0,
                                 22, 43, 61, 41, 40,  0,  0,  0,  0,  0,  0,
                                 18, 61, 42, 22,  0,  0,  0,  0,  0,  0,  0,
                                 45,  1,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                 44, 46, 47,  1,  0,  0,  0,  0,  0,  0,  0,
                                 45, 47,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                 46, 45,  1, 21,  0,  0,  0,  0,  0,  0,  0,
                                 21, 56,  1, 49, 62,  0,  0,  0,  0,  0,  0,
                                 48,  1,  2, 62,  0,  0,  0,  0,  0,  0,  0,
                                  2, 52, 23,  0,  0,  0,  0,  0,  0,  0,  0,
                                 10, 23, 52, 53, 16, 24,  0,  0,  0,  0,  0,
                                 23, 50,  2, 53, 51,  0,  0,  0,  0,  0,  0,
                                 51, 52,  2, 16, 58, 24,  0,  0,  0,  0,  0,
                                  2,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  1, 12,  2,  0,  0,  0,  0,  0,  0,  0,  0,
                                  1, 21, 48,  0,  0,  0,  0,  0,  0,  0,  0,
                                  1,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                 24, 17, 16, 53,  0,  0,  0,  0,  0,  0,  0,
                                 18, 19, 60, 20,  0,  0,  0,  0,  0,  0,  0,
                                 33, 19, 59,  0,  0,  0,  0,  0,  0,  0,  0,
                                 18, 19, 33, 41, 42, 43,  0,  0,  0,  0,  0,
                                 21, 48, 49,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                                  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                               };

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
                factionDataOut.name = TextManager.Instance.GetLocalizedFactionName(factionID, factionDataOut.name);
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
            foreach (FactionFile.FactionData item in factionDict.Values)
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
        /// <param name="regionIndex">Zero-based region index to find in persistent faction data.</param>
        /// <param name="factionDataOut">Receives faction data out.</param>
        /// <returns>True if successful.</returns>
        public bool FindFactionByTypeAndRegion(int type, int regionIndex, out FactionFile.FactionData factionDataOut)
        {
            bool foundPartialMatch = false;
            factionDataOut = new FactionFile.FactionData();
            FactionFile.FactionData partialMatch = new FactionFile.FactionData();

            // Match faction items
            foreach (FactionFile.FactionData item in factionDict.Values)
            {
                if (type == item.type && regionIndex == item.region)
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
        /// Gets the faction data corresponding to the given region index.
        /// </summary>
        /// <param name="regionIndex">The index of the region to get faction data of.</param>
        /// <param name="factionData">Receives faction data.</param>
        /// <param name="duplicateException">Throw exception if duplicate region faction found, otherwise just log warning.</param>
        public void GetRegionFaction(int regionIndex, out FactionFile.FactionData factionData, bool duplicateException = true)
        {
            FactionFile.FactionData[] factions = GameManager.Instance.PlayerEntity.FactionData.FindFactions(
                (int)FactionFile.FactionTypes.Province, -1, -1, regionIndex);

            // Should always find a single region
            if (factions == null || factions.Length != 1)
            {
                if (duplicateException)
                    throw new Exception(string.Format("GetRegionFaction() found more than 1 matching NPC faction for region {0}.", regionIndex));
                else
                    Debug.LogWarningFormat("GetRegionFaction() found more than 1 matching NPC faction for region {0}.", regionIndex);
            }

            factionData = factions[0];
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
        /// Gets faction name from id.
        /// </summary>
        /// <param name="id">ID of faction to get name of.</param>
        /// <returns>Faction name if name found, otherwise an empty string.</returns>
        public string GetFactionName(int id)
        {
            string name = string.Empty;
            if (factionDict.ContainsKey(id))
                name = factionDict[id].name;

            return TextManager.Instance.GetLocalizedFactionName(id, name);
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
        /// Change reputation value by amount. Propagation is matched to classic.
        /// </summary>
        /// <param name="factionID">Faction ID of faction initiate reputation change.</param>
        /// <param name="amount">Amount to change reputation, positive or negative.</param>
        /// <param name="propagate">True if reputation change should propagate to affiliated factions and allies/enemies.</param>
        /// <returns></returns>
        public bool ChangeReputation(int factionID, int amount, bool propagate = false)
        {
            if (factionDict.ContainsKey(factionID))
            {
                FactionFile.FactionData factionData = factionDict[factionID];

                if (!propagate)
                {
                    factionData.rep = Mathf.Clamp(factionData.rep + amount, minReputation, maxReputation);
                    factionDict[factionID] = factionData;
                }
                else
                {
                    // Change ally and enemy faction reputations first (for guild faction or social questgiver npc)
                    int[] allies = { factionData.ally1, factionData.ally2, factionData.ally3 };
                    int[] enemies = { factionData.enemy1, factionData.enemy2, factionData.enemy3 };
                    for (int i = 0; i < 3; ++i)
                    {
                        ChangeReputation(allies[i], amount / 2);
                        ChangeReputation(enemies[i], -amount / 2);
                    }

                    // If a knightly order faction, propagate rep for the generic order only
                    // (this is what classic does - assume due to all affiliated nobles being aloof from such matters..)
                    if (factionData.ggroup == (int)FactionFile.GuildGroups.KnightlyOrder)
                    {
                        ChangeReputation(factionID, amount, false);
                        // Note: classic doesn't propagate to the generic child factions (smiths etc) which is an (assumed) bug so is done here.
                        ChangeReputation((int)FactionFile.FactionIDs.Generic_Knightly_Order, amount, true);
                    }
                    else
                    {
                        // Navigate up to the root faction (treat Dark Brotherhood faction as a root faction)
                        while (factionDict.ContainsKey(factionData.parent) && factionData.id != (int)FactionFile.FactionIDs.The_Dark_Brotherhood)
                            factionData = factionDict[factionData.parent];

                        // Propagate reputation changes for all children of the root, or just the single faction
                        if (factionData.children != null)
                            PropagateReputationChange(factionData, factionID, amount);
                        else
                            ChangeReputation(factionID, amount);

                        // If a temple deity faction, also propagate rep for generic temple faction hierarchy
                        if (factionData.type == (int)FactionFile.FactionTypes.God)
                            ChangeReputation((int)FactionFile.FactionIDs.Generic_Temple, amount, true);
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Recursively propagate reputation changes to affiliated factions using parent/child faction relationships.
        /// </summary>
        /// <param name="factionData">Faction data of parent faction node to change rep for it and children.</param>
        /// <param name="factionID">Faction ID of faction where rep change was initiated.</param>
        /// <param name="amount">Amount to change reputation. (half applied to all but init and questor factions)</param>
        public void PropagateReputationChange(FactionFile.FactionData factionData, int factionID, int amount)
        {
            // Do full reputation change for specific faction, a root parent, and questor npcs. Then half reputation change for all other factions in hierarchy
            ChangeReputation(factionData.id, (factionData.id == factionID || factionData.parent == 0 || questorIds.Contains((GuildNpcServices)factionData.id)) ? amount : amount / 2);

            // Recursively propagate reputation changes to all child factions
            if (factionData.children != null)
                foreach (int id in factionData.children)
                    if (factionDict.ContainsKey(id))
                        PropagateReputationChange(factionDict[id], factionID, amount);
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

        #region Flags

        public bool GetFlag(int factionID, FactionFile.Flags flag)
        {
            if (factionDict.ContainsKey(factionID))
            {
                FactionFile.FactionData factionData = factionDict[factionID];
                return (factionData.flags & (int) flag) > 0;
            }
            return false;
        }

        public bool SetFlag(int factionID, FactionFile.Flags flag)
        {
            if (factionDict.ContainsKey(factionID))
            {
                FactionFile.FactionData factionData = factionDict[factionID];
                factionData.flags |= (int)flag;
                factionDict[factionID] = factionData;
                return true;
            }
            return false;
        }

        #endregion

        #region Power

        /// <summary>
        /// Change power value by amount.
        /// </summary>
        public bool ChangePower(int factionID, int amount)
        {
            if (factionDict.ContainsKey(factionID))
            {
                FactionFile.FactionData factionData = factionDict[factionID];
                factionData.power = Mathf.Clamp(factionData.power + amount, minPower, maxPower);
                factionDict[factionID] = factionData;
                return true;
            }

            return false;
        }

        #endregion

        #region Allies and Enemies

        /// <summary>
        /// Get the number of common allies and enemies between these two functions.
        /// </summary>
        public int GetNumberOfCommonAlliesAndEnemies(int factionID1, int factionID2)
        {
            // Note: This function seems wrong in classic. It looks for how many of
            // faction2's allies are allies of faction1's allies, and adds this with how many of faction2's enemies
            // are enemies of faction1's enemies. The result is used to determine the likelihood of faction1 and faction2 starting
            // or ending an alliance or rivalry but the ally/enemy relationships considered seem contradictory.
            // I'm assuming the intention was more like how many enemies and allies are shared between faction1 and faction2,
            // which is what is done here.

            int count = 0;

            if (factionDict.ContainsKey(factionID1) && factionDict.ContainsKey(factionID2))
            {
                FactionFile.FactionData factionData1 = factionDict[factionID1];
                FactionFile.FactionData factionData2 = factionDict[factionID2];

                int[] alliesOf1 = { factionData1.ally1, factionData1.ally2, factionData1.ally3 };
                int[] alliesOf2 = { factionData2.ally1, factionData2.ally2, factionData2.ally3 };
                int[] enemiesOf1 = { factionData1.enemy1, factionData1.enemy2, factionData1.enemy3 };
                int[] enemiesOf2 = { factionData2.enemy1, factionData2.enemy2, factionData2.enemy3 };

                for (int i = 0; i < 3; ++i)
                {
                    for (int j = 0; j < 3; ++j)
                    {
                        if (alliesOf1[i] == alliesOf2[j])
                            count++;
                        if (enemiesOf1[i] == enemiesOf2[j])
                            count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Check whether faction 2 is in faction 1's ally list.
        /// </summary>
        public bool IsFaction2AnAllyOfFaction1(int factionID1, int factionID2)
        {
            if (factionDict.ContainsKey(factionID1) && factionDict.ContainsKey(factionID2))
            {
                FactionFile.FactionData factionData1 = factionDict[factionID1];

                int[] alliesOf1 = { factionData1.ally1, factionData1.ally2, factionData1.ally3 };

                for (int i = 0; i < 3; ++i)
                {
                    if (factionID2 == alliesOf1[i])
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check whether faction 2 is in faction 1's enemy list.
        /// </summary>
        public bool IsFaction2AnEnemyOfFaction1(int factionID1, int factionID2)
        {
            if (factionDict.ContainsKey(factionID1) && factionDict.ContainsKey(factionID2))
            {
                FactionFile.FactionData factionData1 = factionDict[factionID1];

                int[] enemiesOf1 = { factionData1.enemy1, factionData1.enemy2, factionData1.enemy3 };

                for (int i = 0; i < 3; ++i)
                {
                    if (factionID2 == enemiesOf1[i])
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get faction2 relation to faction1. Returns:
        ///    -1 if factions are unrelated
        ///     0 if factions are the same
        ///     1 if faction2 is the parent of faction1
        ///     2 if faction1 and faction2 share the same parent
        ///     3 if faction2 is a child of faction1
        /// </summary>
        public int GetFaction2RelationToFaction1(int factionID1, int factionID2)
        {
            if (factionDict.ContainsKey(factionID1) && factionDict.ContainsKey(factionID2))
            {
                // Faction1 and faction2 are the same
                if (factionID1 == factionID2)
                    return 0;

                FactionFile.FactionData factionData1 = factionDict[factionID1];
                while (factionData1.parent != 0)
                {
                    // One of faction1 ancestor is faction2
                    if (factionData1.parent == factionID2)
                        return 1;

                    factionData1 = factionDict[factionData1.parent];
                }

                FactionFile.FactionData factionData2 = factionDict[factionID2];
                while (factionData2.parent != 0)
                {
                    // One of faction2 ancestor is faction1
                    if (factionData2.parent == factionID1)
                        return 3;

                    factionData2 = factionDict[factionData2.parent];
                }

                // Faction1 and faction2 share the same ancestor
                if (factionData1.id != factionID1 && factionData2.id != factionID2 && factionData1.id == factionData2.id)
                    return 2;
            }

            return -1;
        }

        /// <summary>
        /// Recursively checks if faction2 is related to faction1 or to its parents.
        /// </summary>
        public bool IsFaction2RelatedToFaction1(int factionID1, int factionID2)
        {
            if (GetFaction2RelationToFaction1(factionID1, factionID2) > -1 ||
               IsFaction2AnAllyOfFaction1(factionID1, factionID2) ||
               IsFaction2AnEnemyOfFaction1(factionID1, factionID2))
                return true;

            FactionFile.FactionData factionData1 = factionDict[factionID1];
            if (factionData1.parent != 0)
                return IsFaction2RelatedToFaction1(factionData1.parent, factionID2);

            return false;
        }

        /// <summary>
        /// Start ally state between two factions.
        /// Faction 2 only adds Faction 1 as an ally if it has room.
        /// </summary>
        public bool StartFactionAllies(int factionID1, int allyNumberForFaction1, int factionID2)
        {
            if (factionDict.ContainsKey(factionID1) && factionDict.ContainsKey(factionID2))
            {
                FactionFile.FactionData factionData1 = factionDict[factionID1];
                FactionFile.FactionData factionData2 = factionDict[factionID2];

                if (allyNumberForFaction1 == 0)
                    factionData1.ally1 = factionID2;
                else if (allyNumberForFaction1 == 1)
                    factionData1.ally2 = factionID2;
                else if (allyNumberForFaction1 == 2)
                    factionData1.ally3 = factionID2;

                if (factionData2.ally1 == 0)
                    factionData2.ally1 = factionID1;
                else if (factionData2.ally2 == 0)
                    factionData2.ally2 = factionID1;
                else if (factionData2.ally3 == 0)
                    factionData2.ally3 = factionID1;

                factionDict[factionID1] = factionData1;
                factionDict[factionID2] = factionData2;

                return true;
            }

            return false;
        }

        /// <summary>
        /// End ally state between two factions.
        /// </summary>
        public bool EndFactionAllies(int factionID1, int factionID2)
        {
            if (factionDict.ContainsKey(factionID1) && factionDict.ContainsKey(factionID2))
            {
                FactionFile.FactionData factionData1 = factionDict[factionID1];
                FactionFile.FactionData factionData2 = factionDict[factionID2];

                if (factionData1.ally1 == factionID2)
                    factionData1.ally1 = 0;
                if (factionData1.ally2 == factionID2)
                    factionData1.ally2 = 0;
                if (factionData1.ally3 == factionID2)
                    factionData1.ally3 = 0;

                if (factionData2.ally1 == factionID1)
                    factionData2.ally1 = 0;
                if (factionData2.ally2 == factionID1)
                    factionData2.ally2 = 0;
                if (factionData2.ally3 == factionID1)
                    factionData2.ally3 = 0;

                factionDict[factionID1] = factionData1;
                factionDict[factionID2] = factionData2;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Start enemy state between two factions.
        /// Faction 2 only adds faction 1 as an enemy if it has room.
        /// </summary>
        public bool StartFactionEnemies(int factionID1, int enemyNumberForFaction1, int factionID2)
        {
            if (factionDict.ContainsKey(factionID1) && factionDict.ContainsKey(factionID2))
            {
                FactionFile.FactionData factionData1 = factionDict[factionID1];
                FactionFile.FactionData factionData2 = factionDict[factionID2];

                if (enemyNumberForFaction1 == 0)
                    factionData1.enemy1 = factionID2;
                else if (enemyNumberForFaction1 == 1)
                    factionData1.enemy2 = factionID2;
                else if (enemyNumberForFaction1 == 2)
                    factionData1.enemy3 = factionID2;

                if (factionData2.enemy1 == 0)
                    factionData2.enemy1 = factionID1;
                else if (factionData2.enemy2 == 0)
                    factionData2.enemy2 = factionID1;
                else if (factionData2.enemy3 == 0)
                    factionData2.enemy3 = factionID1;

                factionDict[factionID1] = factionData1;
                factionDict[factionID2] = factionData2;

                return true;
            }

            return false;
        }

        /// <summary>
        /// End enemy state between two factions.
        /// </summary>
        public bool EndFactionEnemies(int factionID1, int factionID2)
        {
            if (factionDict.ContainsKey(factionID1) && factionDict.ContainsKey(factionID2))
            {
                FactionFile.FactionData factionData1 = factionDict[factionID1];
                FactionFile.FactionData factionData2 = factionDict[factionID2];

                if (factionData1.enemy1 == factionID2)
                    factionData1.enemy1 = 0;
                if (factionData1.enemy2 == factionID2)
                    factionData1.enemy2 = 0;
                if (factionData1.enemy3 == factionID2)
                    factionData1.enemy3 = 0;

                if (factionData2.enemy1 == factionID1)
                    factionData2.enemy1 = 0;
                if (factionData2.enemy2 == factionID1)
                    factionData2.enemy2 = 0;
                if (factionData2.enemy3 == factionID1)
                    factionData2.enemy3 = 0;

                factionDict[factionID1] = factionData1;
                factionDict[factionID2] = factionData2;

                return true;
            }

            return false;
        }

        public bool IsFaction2APotentialWarEnemyOfFaction1(int factionID1, int factionID2)
        {
            if (factionDict.ContainsKey(factionID1) && factionDict.ContainsKey(factionID2))
            {
                FactionFile.FactionData factionData1 = factionDict[factionID1];
                FactionFile.FactionData factionData2 = factionDict[factionID2];

                return factionData1.region != -1 && factionData1.type == (int)FactionFile.FactionTypes.Province
                    && factionData2.region != -1 && factionData2.type == (int)FactionFile.FactionTypes.Province
                    && IsFaction2AnEnemyOfFaction1(factionID1, factionID2)
                    && IsEnemyStatePermanentUntilWarOver(factionData1, factionData2);
            }
            return false;
        }

        public bool IsEnemyStatePermanentUntilWarOver(FactionFile.FactionData faction1, FactionFile.FactionData faction2)
        {
            if (faction1.region != -1 && faction2.region != -1)
            {
                for (int i = 0; i < 11; ++i)
                {
                    if (borderRegions[(11 * faction1.region) + i] == faction2.region)
                        return true;
                }
            }
            return false;
        }

        public bool SetNewRulerData(int factionID)
        {
            if (factionDict.ContainsKey(factionID))
            {
                FactionFile.FactionData faction = factionDict[factionID];
                faction.rulerPowerBonus = DFRandom.random_range_inclusive(0, 50) + 20;
                uint random = DFRandom.rand() << 16;
                faction.rulerNameSeed = DFRandom.rand() | random;
                factionDict[factionID] = faction;

                return true;
            }

            return false;
        }

        #endregion

        #region Parent Group

        /// <summary>
        /// Find the top-level parent group of a given faction. This parent can be a Group, a Province or a Temple.
        /// </summary>
        /// <param name="faction">The faction to get the parent of.</param>
        /// <param name="parentFaction">The parent group faction.</param>
        public void GetParentGroupFaction(FactionFile.FactionData faction, out FactionFile.FactionData parentFaction)
        {
            parentFaction = faction;
            while (parentFaction.parent != 0 &&
                   parentFaction.type != (int)FactionFile.FactionTypes.Group &&
                   parentFaction.type != (int)FactionFile.FactionTypes.Province &&
                   parentFaction.type != (int)FactionFile.FactionTypes.Temple)
            {
                GameManager.Instance.PlayerEntity.FactionData.GetFactionData(parentFaction.parent, out parentFaction);
            }
        }

        #endregion

    }
}
