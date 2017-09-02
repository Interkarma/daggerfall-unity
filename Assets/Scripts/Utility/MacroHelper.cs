// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using DaggerfallWorkshop.Game;
using DaggerfallConnect.Arena2;
using System.Collections.Generic;
using System;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Context sensitive macros like '%abc' are used in following Daggerfall files:
    /// arena2\text.rsc, fall.exe, arena2\*.qrc, or arena2\bio*.txt
    /// </summary>
    public static class MacroHelper
    {
        public delegate string MacroValueProvider(MacroDataSource mds = null);

        static Dictionary<string, MacroValueProvider> macros = new Dictionary<string, MacroValueProvider>()
        {
            { "%1am", null }, // 1st + Magnitude
            { "%1bm", null }, // 1st base Magnitude
            { "%1com", null },// Greeting (?)
            { "%1hn", null }, // ?
            { "%2am", null }, // 2nd + Magnitude
            { "%2bm", null }, // 2nd Base Magnitude
            { "%2com", null },// ?
            { "%2hn", null }, // ?
            { "%3hn", null }, // ?
            { "%a", null },   // Cost of somthing.
            { "%ach", null }, // + Chance per
            { "%adr", null }, // + Duration per
            { "%agi", null }, //  Amount of Agility
            { "%arm", ItemName }, //  Armour
            { "%ark", null }, // ?
            { "%ba", null },  // Book Author
            { "%bch", null }, // Base chance
            { "%bdr", null }, // Base Duration
            { "%bn", null },  // ?
            { "%bt", null },  // Book title
            { "%cbl", null }, // Cash balance in current region
            { "%clc", null }, // Per level (Chance)
            { "%cld", null }, // Per level (Duration)
            { "%clm", null }, // Per level (Magnitude)
            { "%cn", CityName },  // City name
            { "%cn2", CityName2 }, // City name #2
            { "%cpn", null }, // Current shop name
            { "%cri", null }, // Accused crime
            { "%crn", CurrentRegion }, // Current Region
            { "%dae", null }, // A daedra
            { "%dam", null }, // Damage modifyer
            { "%dat", Date }, // Date
            { "%di", null },  // Direction
            { "%dip", null }, // Days in prison
            { "%dng", null }, // Dungeon
            { "%dts", null }, // Daedra
            { "%dwr", null }, // Days (hours) with room remaining?
            { "%ef", null },  // Local shop name
            { "%enc", null }, // Encumberence
            { "%end", null }, // Amount of Endurance
            { "%fcn", null }, // Another city
            { "%fe", null },  // ?
            { "%fea", null }, // ?
            { "%fl1", null }, // Lord of _fx1
            { "%fl2", null }, // Lord of _fx2
            { "%fn", null },  // Random first(?) name (Female?)
            { "%fn2", null }, // Same as _mn2 (?)
            { "%fnpc", null }, // ?
            { "%fon", null }, // ?
            { "%fpa", null }, // ?
            { "%fpc", null }, // ?
            { "%fx1", null }, // A faction in news
            { "%fx2", null }, // Another faction in news
            { "%g", Pronoun },   // He/She etc...
            { "%g1", Pronoun },  // He/She ???
            { "%g2", Pronoun2 },  // Him/Her etc...
            { "%g2self", Pronoun2self },// Himself/Herself etc...
            { "%g3", Pronoun3 },  // His/Hers/Theirs etc...
            { "%gii", null }, // Amount of gold in hand
            { "%god", God }, // Some god (listed in TEXT.RSC)
            { "%gtp", null }, // Amount of fine
            { "%hea", null }, // HP Modifier
            { "%hmd", null }, // Healing rate modifer
            { "%hnr", null }, // Honorific
            { "%hnt", null }, // Direction of location.
            { "%hnt2", null },// ?
            { "%hol", null }, // Holiday
            { "%hpn", null }, // ?
            { "%hpw", null }, // ?
            { "%hrg", null }, // House region
            { "%hs", null },  //  Holding Soul type
            { "%htwn", null },// House town
            { "%imp", null }, // ?
            { "%int", null }, // Amount of Intelligence
            { "%it", ItemName },  //  Item
            { "%jok", null }, // A joke
            { "%key", null }, // A location (?)
            { "%key2", null },// Another location
            { "%kg", Weight },  //  Weight of items
            { "%kno", null }, // A knightly guild name
            { "%lev", null }, // Rank in guild that you are in.
            { "%ln", null },  //  Random lastname
            { "%loc", null }, // Location marked on map
            { "%lt1", null }, // Title of _fl1
            { "%ltn", LocalReputation }, // In the eyes of the law you are.......
            { "%luc", null }, // Luck
            { "%map", null }, // ?
            { "%mad", null }, // Resistance
            { "%mat", Material }, // Material
            { "%mit", null }, // Item
            { "%mn", null },  // Random First(?) name (Male?)
            { "%mn2", null }, // Same as _mn (?)
            { "%mod", Modification }, // Modification
            { "%mpw", null }, // Magic powers
            { "%n", null },   // A random female first name
            { "%nam", null }, // A random full name
            { "%nrn", null }, // Noble of the current region
            { "%nt", null },  // ?
            { "%ol1", null }, // Old lord of _fx1
            { "%olf", null }, // What happened to _ol1
            { "%on", null },  // ?
            { "%oth", Oath }, // An oath (listed in TEXT.RSC)
            { "%pc", null },  // ?
            { "%pcf", PlayerFirstname }, // Character's first name
            { "%pcn", PlayerName }, // Character's full name
            { "%pct", GuildTitle }, // Player guild title/rank
            { "%pdg", null }, // Days in jail
            { "%pen", null }, // Prison sentence
            { "%per", null }, // Amount of Personality
            { "%plq", null }, // Place of something in log.
            { "%pnq", null }, // Person of something in log
            { "%pp1", null }, // ?
            { "%pp2", null }, // ?
            { "%pqn", null }, // Potential Quest Giver
            { "%pqp", null }, // Potential Quest Giver's Location
            { "%ptm", null }, // An enemy of the current region (?)
            { "%q1", null },  // q1 to q12 Effects of questions answered in bio.
            { "%q2", null },
            { "%q3", null },
            { "%q4", null },
            { "%q5", null },
            { "%q6", null },
            { "%q7", null },
            { "%q8", null },
            { "%q9", null },
            { "%q10", null },
            { "%q11", null },
            { "%q12", null },
            { "%qdt", QuestDate }, // Quest date of log entry
            { "%qdat", null },// Quest date of log entry [2]
            { "%qot", null }, // The log comment
            { "%qua", Condition }, // Condition
            { "%r1", null },  // Commoners rep
            { "%r2", null },  // Merchants rep
            { "%r3", null },  // Scholers rep
            { "%r4", null },  // Nobilitys rep
            { "%r5", null },  // Underworld rep
            { "%ra", PlayerRace },  // Player's race
            { "%reg", Region }, // Region
            { "%rn", null },  // Regent's Name
            { "%rt", null },  // Regent's Title
            { "%spc", null }, // Current Spell Points
            { "%ski", null }, // Skill
            { "%spd", null }, // Speed
            { "%spt", null }, // ?
            { "%str", null }, // Amount of strength
            { "%sub", null }, // ?
            { "%t", null },   // Regent's Title
            { "%tcn", null }, // Travel city name
            { "%thd", null }, // Combat odds
            { "%tim", Time }, // Time
            { "%vam", null }, // PC's vampire clan
            { "%vcn", null }, // Vampire's Clan
            { "%vn", null },  // ?
            { "%wdm", WeaponDamage }, // Weapon damage
            { "%wep", ItemName }, // Weapon
            { "%wil", null }, // ?
            { "%wpn", null }, // Poison (?)
            { "%wth", null }, // Worth
        };

        static char[] PUNCTUATION = { '.', ',', '\'' };

        public static void ExpandMacros(ref TextFile.Token[] tokens, MacroDataSource mds = null)
        {
            // Iterate message tokens
            string tokenText;
            for (int tokenIdx = 0; tokenIdx < tokens.Length; tokenIdx++)
            {
                tokenText = tokens[tokenIdx].text;
                if (tokenText != null && tokenText.IndexOf('%') >= 0)
                {
                    // Split token text into individual words
                    string[] words = tokenText.Split(' ');

                    // Iterate words to find macros
                    for (int wordIdx = 0; wordIdx < words.Length; wordIdx++)
                    {
                        if (words[wordIdx].StartsWith("%"))
                        {
                            int wordLen = words[wordIdx].Length - 1;
                            if (words[wordIdx].IndexOfAny(PUNCTUATION) == wordLen)
                            {
                                string symbolStr = words[wordIdx].Substring(0, wordLen);
                                words[wordIdx] = GetValue(symbolStr, mds) + words[wordIdx].Substring(wordLen);
                            }
                            else
                            {
                                words[wordIdx] = GetValue(words[wordIdx], mds);
                            }
                        }
                    }

                    // Re-assemble.
                    tokenText = string.Empty;
                    for (int wordIdx = 0; wordIdx < words.Length; wordIdx++)
                    {
                        tokenText += words[wordIdx];
                        if (wordIdx != words.Length - 1)
                            tokenText += " ";
                    }
                    tokens[tokenIdx].text = tokenText;
                }
            }
        }

        public static string GetValue(string symbolStr, MacroDataSource mds)
        {
            if (macros.ContainsKey(symbolStr))
            {
                MacroValueProvider svp = macros[symbolStr];
                if (svp != null)
                {
                    try
                    {
                        return svp.Invoke(mds);
                    }
                    catch (NotImplementedException)
                    {
                        return symbolStr + "[srcDataUnknown]";
                    }
                }
                else
                {
                    return symbolStr + "[unhandled]";
                }
            }
            else
            {
                return symbolStr + "[undefined]";
            }
        }

        #region global symbol value providers

        // Global symbol value providers - not context sensitive. (mds will be null, and should not be used)

        private static string CityName(MacroDataSource mds)
        {   // %cn
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            if (gps.HasCurrentLocation)
                return gps.CurrentLocation.Name;
            else
                return gps.CurrentRegion.Name;
        }

        private static string CityName2(MacroDataSource mds)
        {   // %cn2 (only used in msg #200)
            throw new NotImplementedException();
        }

        private static string CurrentRegion(MacroDataSource mds)
        {   // %crn
            return GameManager.Instance.PlayerGPS.CurrentRegion.Name;
        }

        private static string LocalReputation(MacroDataSource mds)
        {   // %ltn
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            PersistentFactionData factionData = Game.GameManager.Instance.PlayerEntity.FactionData;
            int rep = factionData.GetLegalReputation(gps.CurrentRegionIndex).value;
            if (rep > 60)
                return "Esteemed";
            else if (rep > 40)
                return "Honored";
            else if (rep > 20)
                return "Admired";
            else if (rep > 10)
                return "Respected";
            else if (rep > 0)
                return "Dependable";
            else if (rep == 0)
                return "Common Citizen";
            else if (rep < 0)
                return "Undependable";
            else if (rep < -10)
                return "Scoundrel";
            else if (rep < -20)
                return "Criminal";
            else if (rep < -40)
                return "Villain";
            else if (rep < -60)
                return "Pond Scum";
            else if (rep < -80)
                return "Hated";
            return "Unknown";
        }

        private static string Time(MacroDataSource mds)
        {   // %tim
            return DaggerfallUnity.Instance.WorldTime.Now.MinTimeString();
        }

        private static string Date(MacroDataSource mds)
        {   // %dat
            return DaggerfallUnity.Instance.WorldTime.Now.DateString();
        }

        private static string PlayerName(MacroDataSource mds)
        {   // %pcn
            return GameManager.Instance.PlayerEntity.Name;
        }

        private static string PlayerFirstname(MacroDataSource mds)
        {   // %pcf
            string name = GameManager.Instance.PlayerEntity.Name;
            string[] parts = name.Split(' ');
            return (parts != null && parts.Length > 0) ? parts[0] : name;
        }

        private static string PlayerRace(MacroDataSource mds)
        {   // %ra
            return GameManager.Instance.PlayerEntity.RaceTemplate.Name;
        }

        private static string GuildTitle(MacroDataSource mds)
        {   // %pct
            // Just use "Apprentice" for all %pct guild titles for now
            // Guilds are not implemented yet, will need to move into MacroDataSource
            return "Apprentice";
        }

        #endregion

        #region context sensitive symbol value providers
        // Context sensitive symbol value providers - delegate to the passed context symbol data source.

        public static string Material(MacroDataSource mds)
        {   // %mat
            return mds.Material();
        }

        public static string Condition(MacroDataSource mds)
        {   // %qua
            return mds.Condition();
        }

        public static string Weight(MacroDataSource mds)
        {   // %kg
            return mds.Weight();
        }

        public static string WeaponDamage(MacroDataSource mds)
        {   // %wdm
            return mds.WeaponDamage();
        }

        public static string ItemName(MacroDataSource mds)
        {   // %wep, %arm, %it
            return mds.ItemName();
        }

        public static string Modification(MacroDataSource mds)
        {   // %mod
            return mds.Modification();
        }

        public static string Pronoun(MacroDataSource mds)
        {   // %g & %g1
            return mds.Pronoun();
        }
        public static string Pronoun2(MacroDataSource mds)
        {   // %g2
            return mds.Pronoun2();
        }
        public static string Pronoun2self(MacroDataSource mds)
        {   // %g2self
            return mds.Pronoun2self();
        }
        public static string Pronoun3(MacroDataSource mds)
        {   // %g3
            return mds.Pronoun3();
        }

        public static string QuestDate(MacroDataSource mds)
        {   // %qdt
            return mds.QuestDate();
        }

        public static string Oath(MacroDataSource mds)
        {   // %oth
            return mds.Oath();
        }

        public static string Region(MacroDataSource mds)
        {   // %reg
            return mds.Region();
        }

        public static string God(MacroDataSource mds)
        {   // %god
            return mds.God();
        }

        #endregion
    }

}