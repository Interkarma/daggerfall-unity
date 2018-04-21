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

#region Using Statements
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Save;
using DaggerfallWorkshop;
#endregion

namespace DaggerfallConnect.Arena2
{
    /// <summary>
    /// Connects to FACTION.TXT and reads faction data.
    /// </summary>
    public class FactionFile
    {
        #region Fields

        FileProxy factionFile = new FileProxy();
        Dictionary<int, FactionData> factionDict = new Dictionary<int, FactionData>();
        Dictionary<string, int> factionNameToIDDict = new Dictionary<string, int>();

        #endregion

        #region Properties

        public Dictionary<int, FactionData> FactionDict
        {
            get { return factionDict; }
        }

        public Dictionary<string, int> FactionNameToIDDict
        {
            get { return factionNameToIDDict; }
        }

        #endregion

        #region Enums

        public enum FactionTypes
        {
            None = -1,
            Daedra = 0,
            God = 1,
            Group = 2,
            Subgroup = 3,
            Individual = 4,
            Official = 5,
            VampireClan = 6,
            Province = 7,
            WitchesCoven = 8,
            Temple = 9,
            KnightlyGuard = 10,
            MagicUser = 11,
            Generic = 12,
            Thieves = 13,
            Courts = 14,
            People = 15,
        }

        public enum SocialGroups
        {
            None = -1,
            Commoners = 0,
            Merchants = 1,
            Scholars = 2,
            Nobility = 3,
            Underworld = 4,
            SGroup5 = 5,
            SupernaturalBeings = 6,
            GuildMembers = 7,
            SGroup8 = 8,
            SGroup9 = 9,
            SGroup10 = 10,
        }

        public enum GuildGroups
        {
            None = -1,
            GGroup0 = 0,
            GGroup1 = 1,
            Oblivion = 2,
            DarkBrotherHood = 3,
            GeneralPopulace = 4,    // ThievesGuild when socialGroup = Underworld
            Bards = 5,
            TheFey = 6,
            Prostitutes = 7,
            GGroup8 = 8,
            KnightlyOrder = 9,
            MagesGuild = 10,
            FightersGuild = 11,
            GGroup12 = 12,
            GGroup13 = 13,
            Necromancers = 14,
            Region = 15,
            GGroup16 = 16,
            HolyOrder = 17,
            GGroup18 = 18,
            GGroup19 = 19,
            GGroup20 = 20,
            GGroup21 = 21,
            Witches = 22,
            Vampires = 23,
            Orsinium = 24,
        }

        /// <summary>
        /// Faction race value does not map to usual race ID.
        /// Instead it selects from a smaller pool as below.
        /// For example:
        ///  * "Daggerfall", and most others, have a race of 3 (Breton)
        ///  * "Sentinel" has a race of 2 (Redguard)
        ///  * This is likely involved in how Daggerfall assigns the race of wandering NPCs in towns
        ///  * When selecting a random face for escorts, it is assigned by the dominant race of region
        ///  * Not all races found in FACTION.TXT are present here - unsure if these are even used in game
        /// </summary>
        public enum FactionRaces
        {
            None = -1,
            Redguard = 2,
            Breton = 3,
            Nord = 4,           // Guess - not found in faction.txt
            WoodElf = 5,        // Guess - based on #378 "Sylch Greenwood"
            DarkElf = 7,
            Skakmat = 11,       // Only used on #304 "Skakmat"
            Orc = 17,           // Only used on #358 "Orsinium"
            Vampire = 18,
            Fey = 19,           // Only used on #513 "The Fey"
        }

        #endregion

        #region Structures

        [Serializable]
        public struct FactionData
        {
            public int id;
            public int parent;
            public int type;
            public string name;
            public int rep;
            public int summon;
            public int region;
            public int power;
            public int flags;
            public int ruler;
            public int ally1;
            public int ally2;
            public int ally3;
            public int enemy1;
            public int enemy2;
            public int enemy3;
            public int face;
            public int race;
            public int flat1;
            public int flat2;
            public int sgroup;
            public int ggroup;
            public int minf;
            public int maxf;
            public int vam;
            public int rank;
            public int randomValue;
            public int randomPowerBonus;
            public int ptrToNextFactionAtSameHierarchyLevel;
            public int ptrToFirstChildFaction;
            public int ptrToParentFaction;

            public List<int> children;
        }

        public struct FlatData
        {
            public int archive;
            public int record;
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets default FACTION.TXT filename.
        /// </summary>
        static public string Filename
        {
            get { return "FACTION.TXT"; }
        }

        public static Dictionary<int, FactionData> CustomFactions { get { return customFactions; } }

        #endregion

        #region Custom faction registration

        // Registry for custom factions.
        private static Dictionary<int, FactionData> customFactions = new Dictionary<int, FactionData>();

        /// <summary>
        /// Register a custom faction not in the DF FACTION.TXT file
        /// </summary>
        /// <param name="factionId">faction id for the new faction</param>
        /// <param name="factionData">faction data struct for new faction, including child list</param>
        /// <returns>true if faction was registered, false if the faction id is already in use</returns>
        public static bool RegisterCustomFaction(int factionId, FactionData factionData)
        {
            DaggerfallUnity.LogMessage("RegisterCustomFaction: " + factionId, true);
            if (!customFactions.ContainsKey(factionId))
            {
                customFactions.Add(factionId, factionData);
                return true;
            }
            return false;
        }

        #endregion

        #region Constructors

        public FactionFile()
        {
        }

        public FactionFile(string filePath, FileUsage usage, bool readOnly)
        {
            Load(filePath, usage, readOnly);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load from FACTION.TXT file.
        /// </summary>
        /// <param name="path">Absolute path to FACTION.TXT file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public void Load(string filePath, FileUsage usage, bool readOnly)
        {
            // Validate filename
            if (!filePath.EndsWith(Filename, StringComparison.InvariantCultureIgnoreCase))
                return;

            // Load file
            if (!factionFile.Load(filePath, usage, readOnly))
                return;

            // Parse faction file
            byte[] buffer = factionFile.Buffer;
            string txt = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            ParseFactions(txt);
            RelinkChildren(factionDict);
        }

        /// <summary>
        /// Merges faction data from savevars into a new faction dictionary.
        /// Does not affect factions as read from FACTIONS.TXT.
        /// This resultant set of factions is the character's live faction setup.
        /// This is only used when importing a classic save.
        /// Daggerfall Unity uses a different method of storing faction data with saves.
        /// </summary>
        /// <param name="saveVars"></param>
        public Dictionary<int, FactionData> Merge(SaveVars saveVars)
        {
            // Create clone of base faction dictionary
            Dictionary<int, FactionData> dict = new Dictionary<int, FactionData>();
            foreach (var kvp in factionDict)
            {
                dict.Add(kvp.Key, kvp.Value);
            }

            // Merge save faction data from savevars
            FactionData[] factions = saveVars.Factions;
            foreach(var srcFaction in factions)
            {
                if (dict.ContainsKey(srcFaction.id))
                {
                    FactionData dstFaction = dict[srcFaction.id];

                    // First a quick sanity check to ensure IDs are the same
                    if (dstFaction.id != srcFaction.id)
                        throw new Exception(string.Format("ID mismatch while merging faction data. SrcFaction=#{0}, DstFaction=#{1}", srcFaction.id, dstFaction.id));

                    // Copy live reputation value from SAVEVARS.DAT
                    // Other values remain as pre-generated from faction.txt
                    // This is to prevent bad save data polluting faction structure
                    dstFaction.rep = srcFaction.rep;

                    // May migrate other values later
                    //dstFaction.type = srcFaction.type;
                    //dstFaction.name = srcFaction.name;
                    //dstFaction.region = srcFaction.region;
                    //dstFaction.power = srcFaction.power;
                    //dstFaction.flags = srcFaction.flags;
                    //dstFaction.ruler = srcFaction.ruler;
                    //dstFaction.ally1 = srcFaction.ally1;
                    //dstFaction.ally2 = srcFaction.ally2;
                    //dstFaction.ally3 = srcFaction.ally3;
                    //dstFaction.enemy1 = srcFaction.enemy1;
                    //dstFaction.enemy2 = srcFaction.enemy2;
                    //dstFaction.enemy3 = srcFaction.enemy3;
                    //dstFaction.face = srcFaction.face;
                    //dstFaction.race = srcFaction.race;
                    //dstFaction.flat1 = srcFaction.flat1;
                    //dstFaction.flat2 = srcFaction.flat2;
                    //dstFaction.sgroup = srcFaction.sgroup;
                    //dstFaction.ggroup = srcFaction.ggroup;
                    //dstFaction.vam = srcFaction.vam;

                    // Store merged data back in new dictionary
                    dict[srcFaction.id] = dstFaction;
                }
            }

            RelinkChildren(dict);

            return dict;
        }

        /// <summary>
        /// Gets faction data from faction ID.
        /// </summary>
        /// <param name="factionID">Faction ID.</param>
        /// <param name="factionDataOut">Receives faction data.</param>
        /// <returns>True if successful.</returns>
        public bool GetFactionData(int factionID, out FactionData factionDataOut)
        {
            factionDataOut = new FactionData();
            if (factionDict.ContainsKey(factionID))
            {
                factionDataOut = factionDict[factionID];
                return true;
            }

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
        /// Turns a flat int back into archive/record format
        /// </summary>
        /// <param name="flat"></param>
        /// <returns></returns>
        public static FlatData GetFlatData(int flat)
        {
            FlatData flatData = new FlatData();
            flatData.archive = flat >> 7;
            flatData.record = flat & 0x7f;

            return flatData;
        }

        /// <summary>
        /// Relink parent factions to their child faction.
        /// </summary>
        public static void RelinkChildren(Dictionary<int, FactionData> dict)
        {
            List<FactionData> factionValues = new List<FactionData>(dict.Values);
            foreach (FactionData faction in factionValues)
            {
                if (dict.ContainsKey(faction.parent))
                {
                    FactionData parent = dict[faction.parent];
                    if (parent.children == null)
                        parent.children = new List<int>();
                    parent.children.Add(faction.id);
                    dict[faction.parent] = parent;
                }
            }
        }

        #endregion

        #region Private Methods

        void ParseFactions(string txt)
        {
            // Unmodded faction.txt contains multiples of same id
            // This resolver counter is used to give a faction a unique id if needed
            int resolverId = 1000;

            // Clear existing dictionary
            factionDict.Clear();

            // First pass reads each faction text block in order
            List<string[]> factionBlocks = new List<string[]>();
            using (StringReader reader = new StringReader(txt))
            {
                List<string> currentblock = new List<string>();
                while (true)
                {
                    // Handle end of file
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        // Store final block
                        if (currentblock.Count > 0)
                            factionBlocks.Add(currentblock.ToArray());

                        break;
                    }

                    // Ignore comment lines and empty lines
                    if (line.StartsWith(";") || string.IsNullOrEmpty(line))
                        continue;

                    // All factions blocks start with a '#' character
                    if (line.Contains("#"))
                    {
                        // Store current block
                        if (currentblock.Count > 0)
                            factionBlocks.Add(currentblock.ToArray());

                        // Start new block
                        currentblock.Clear();
                    }

                    // Add line to current faction block
                    currentblock.Add(line);
                }
            }

            // Second pass parses the text block into FactionData
            int lastPrecedingTabs = 0;
            FactionData previousFaction = new FactionData();
            Stack<int> parentStack = new Stack<int>();
            for (int i = 0; i < factionBlocks.Count; i++)
            {
                // Start a new faction
                FactionData faction = new FactionData();
                string[] block = factionBlocks[i];

                // Parent child relationship determined by preceding tabs
                int precedingTabs = CountPrecedingTabs(block[0]);
                if (precedingTabs > lastPrecedingTabs)
                {
                    parentStack.Push(previousFaction.id);
                }
                else if (precedingTabs < lastPrecedingTabs)
                {
                    while(parentStack.Count > precedingTabs)
                        parentStack.Pop();
                }
                lastPrecedingTabs = precedingTabs;

                // Set parent from top of stack
                if (parentStack.Count > 0)
                    faction.parent = parentStack.Peek();

                // Parse faction block
                ParseFactionData(ref block, ref faction);

                // Store faction just read
                if (!factionDict.ContainsKey(faction.id))
                {
                    factionDict.Add(faction.id, faction);
                }
                else
                {
                    // Duplicate id detected
                    faction.id = resolverId++;
                    factionDict.Add(faction.id, faction);
                }

                // Key faction name to faction id
                if (!factionNameToIDDict.ContainsKey(faction.name))
                {
                    factionNameToIDDict.Add(faction.name, faction.id);
                }
                else
                {
                    // Just ignoring duplicates for now
                    // Currently only using name to id lookup for to find region faction quickly
                    //UnityEngine.Debug.LogWarningFormat("Duplicate name detected " + faction.name);
                }

                previousFaction = faction;
            }
        }

        void ParseFactionData(ref string[] block, ref FactionData faction)
        {
            string[] parts;
            int allyCount = 0;
            int enemyCount = 0;
            int flatCount = 0;
            for (int i = 0; i < block.Length; i++)
            {
                string line = block[i];

                // Get faction id
                if (line.Contains("#"))
                {
                    parts = line.Split('#');
                    faction.id = ParseInt(parts[1].Trim());
                    continue;
                }

                // Handle empty lines
                if (string.IsNullOrEmpty(line.Trim()))
                    continue;

                // Split string into tag and value using ':' character
                parts = line.Split(':');
                if (parts.Length != 2)
                {
                    // Attempt to split by space
                    // Original faction.txt has malformed tag missing colon
                    // If this still fails throw an exception
                    parts = line.Split(' ');
                    if (parts.Length != 2)
                        throw new Exception(string.Format("Invalid tag format for data {0} on faction {1}", line, faction.id));
                }

                // Get tag and value strings
                string tag = parts[0].Trim().ToLower();
                string value = parts[1].Trim();

                // Set tag value in faction
                switch (tag)
                {
                    case "name":
                        faction.name = value;
                        break;
                    case "rep":
                        faction.rep = ParseInt(value);
                        break;
                    case "summon":
                        faction.summon = ParseInt(value);
                        break;
                    case "region":
                        faction.region = ParseInt(value);
                        break;
                    case "type":
                        faction.type = ParseInt(value);
                        break;
                    case "power":
                        faction.power = ParseInt(value);
                        break;
                    case "flags":
                        faction.flags = ParseInt(value);
                        break;
                    case "ruler":
                        faction.ruler = ParseInt(value);
                        break;
                    case "face":
                        faction.face = ParseInt(value);
                        break;
                    case "race":
                        faction.race = ParseInt(value);
                        break;
                    case "sgroup":
                        faction.sgroup = ParseInt(value);
                        break;
                    case "ggroup":
                        faction.ggroup = ParseInt(value);
                        break;
                    case "minf":
                        faction.minf = ParseInt(value);
                        break;
                    case "maxf":
                        faction.maxf = ParseInt(value);
                        break;
                    case "vam":
                        faction.vam = ParseInt(value);
                        break;
                    case "rank":
                        faction.rank = ParseInt(value);
                        break;
                    case "ally":
                        int ally = ParseInt(value);
                        if (allyCount == 0)
                            faction.ally1 = ally;
                        else if (allyCount == 1)
                            faction.ally2 = ally;
                        else if (allyCount == 2)
                            faction.ally3 = ally;
                        else
                            throw new Exception(string.Format("Too many allies for faction #{0}", faction.id));
                        allyCount++;
                        break;
                    case "enemy":
                        int enemy = ParseInt(value);
                        if (enemyCount == 0)
                            faction.enemy1 = enemy;
                        else if (enemyCount == 1)
                            faction.enemy2 = enemy;
                        else if (enemyCount == 2)
                            faction.enemy3 = enemy;
                        else
                            throw new Exception(string.Format("Too many enemies for faction #{0}", faction.id));
                        enemyCount++;
                        break;
                    case "flat":
                        int flat = ParseFlat(value);
                        if (flat > 0)
                        {
                            if (flatCount == 0)
                            {
                                // When a single flat is specified Daggerfall stores for both male and female
                                faction.flat1 = flat;
                                faction.flat2 = flat;
                            }
                            else if (flatCount == 1)
                            {
                                // The second flat found is a female flat
                                faction.flat2 = flat;
                            }
                            else
                            {
                                throw new Exception(string.Format("Too many flats for faction #{0}", faction.id));
                            }
                            flatCount++;
                        }
                        break;
                    default:
                        throw new Exception(string.Format("Unexpected tag '{0}' with value '{1}' encountered on faction #{2}", tag, value, faction.id));
                }
            }
        }

        int CountPrecedingTabs(string line)
        {
            int count = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\t')
                    count++;
                else
                    break;
            }

            return count;
        }

        int ParseFlat(string value)
        {
            int result = 0;
            string[] parts = value.Split();
            if (parts.Length == 1)
            {
                return 0;
            }
            else if (parts.Length == 2)
            {
                result = ParseInt(parts[0]) << 7;
                result += ParseInt(parts[1]);
            }
            else
            {
                throw new Exception(string.Format("Invalid flat format for value {0}", value));
            }

            return result;
        }

        int ParseInt(string value)
        {
            return int.Parse(value);
        }

        #endregion
    }
}