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

#region Using Statements
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect.Utility;
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

        #endregion

        #region Properties

        public Dictionary<int, FactionData> FactionDict
        {
            get { return factionDict; }
        }

        #endregion

        #region Enums

        public enum FactionGroups
        {
            Commoner = 0,
            Merchant = 1,
            Scholar = 2,
            Nobility = 3,
            Underworld = 4,
        }

        public enum FactionTypes
        {
            Daedra = 0,
            God = 1,
            Group = 2,
            Subgroup = 3,
            Individual = 4,
            OnePer = 5,
            Vampire = 6,
            Noble = 7,
            Witches = 8,
            Temple = 9,
            GenericG = 12,
            Thieves = 13,
            Courts = 14,
            People = 15,
        }

        #endregion

        #region Structures

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
            public int[] allies;
            public int[] enemies;
            public int face;
            public int race;
            public FactionFlat[] flats;
            public int sgroup;
            public int ggroup;
            public int minf;
            public int maxf;
            public int vam;
            public int rank;
        }

        public struct FactionFlat
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
        }

        #endregion

        #region Private Methods

        void ParseFactions(string txt)
        {
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

                    // All factions blocks start with a # character
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
            FactionData previousFaction = new FactionData();
            for (int i = 0; i < factionBlocks.Count; i++)
            {
                // Start a new faction
                FactionData currentFaction = new FactionData();
                string[] block = factionBlocks[i];

                // If this block starts with a \t then it's a child faction of previous block
                if (block[0].StartsWith("\t"))
                    currentFaction.parent = previousFaction.id;

                // Parse faction block
                ParseFactionData(ref block, ref currentFaction);

                // Store faction just read
                factionDict.Add(currentFaction.id, currentFaction);
                previousFaction = currentFaction;
            }
        }

        void ParseFactionData(ref string[] block, ref FactionData faction)
        {
            string[] parts;
            List<int> allies = new List<int>();
            List<int> enemies = new List<int>();
            List<FactionFlat> flats = new List<FactionFlat>();
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

                // Split string into tag and value using : character
                parts = line.Split(':');
                if (parts.Length != 2)
                    throw new Exception(string.Format("Invalid tag format for data {0} on faction {1}", line, faction.id));

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
                        allies.Add(ParseInt(value));
                        break;
                    case "enemy":
                        enemies.Add(ParseInt(value));
                        break;
                    case "flat":
                        FactionFlat flat;
                        if (ParseFlat(value, out flat))
                        {
                            flats.Add(flat);
                        }
                        break;
                    default:
                        throw new Exception(string.Format("Unexpected tag '{0}' with value '{1}' encountered on faction #{2}", tag, value, faction.id));
                }
            }

            // Store array-based values
            faction.allies = allies.ToArray();
            faction.enemies = enemies.ToArray();
            faction.flats = flats.ToArray();
        }

        bool ParseFlat(string value, out FactionFlat flat)
        {
            flat = new FactionFlat();
            string[] parts = value.Split();
            if (parts.Length == 1)
            {
                return false;
            }
            else if (parts.Length == 2)
            {
                flat.archive = ParseInt(parts[0]);
                flat.record = ParseInt(parts[1]);
            }
            else
            {
                throw new Exception(string.Format("Invalid flat format for value {0}", value));
            }

            return true;
        }

        int ParseInt(string value)
        {
            return int.Parse(value);
        }

        /// <summary>
        /// Creates hash of tag name.
        /// </summary>
        /// <param name="tag">Source tag name.</param>
        /// <returns>Hashed tag name.</returns>
        int HashTagName(string tag)
        {
            int id = 0;
            for (int i = 0; i < tag.Length; i++)
            {
                id = id << 1;
                id += tag[i];
            }

            return id;
        }

        #endregion
    }
}