// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallConnect.Arena2;
using System.Collections.Generic;
using System;
using DaggerfallWorkshop.Game.Player;


namespace DaggerfallWorkshop.Utility
{
    /**
     * <summary>
     * Helper class for context sensitive macros like '%abc' that're used in following Daggerfall files:
     * arena2\text.rsc, fall.exe, arena2\*.qrc, or arena2\bio*.txt
     * </summary>
     * 
     * If any messages displayed in game contain the following markup, this is what needs adding:
     * %abc[undefined]      -> macro needs adding to <c>macroHandlers</c> list
     * %abc[unhandled]      -> macro requires handler method name in <c>macroHandlers</c> list
     * %abc[srcDataUnknown] -> macro context provider object needs handler method adding
     * 
     * Adding new macro handlers:
     * 
     * Open the file <c>DaggerfallWorkshop.Utility.MacroHelper</c>.
     * Does the macro need an object instance to provide context? (e.g. item, quest)
     * 
     * If no context required:
     * 1) Find the macro in <c>macroHandlers</c>, if the macro isn't in the list then add it at the bottom. e.g. '%ra'
     * 2) Define the handler method name, replacing 'null' with it. e.g. 'PlayerRace'
     * 3) Add a suitable handler method to the region <c>global macro handlers</c>. (mcp will be null so don't use it) e.g.
     * <code>
     * private static string PlayerRace(IMacroContextProvider mcp)
     * {   // %ra
     *     return GameManager.Instance.PlayerEntity.RaceTemplate.Name;
     * }
     * </code>
     *      
     * If context required:
     * 1) Find the macro in <c>macroHandlers</c>, if the macro isn't in the list then add it at the bottom. e.g. '%ra'
     * 2) Define the handler method name, replacing 'null' with it. e.g. 'PlayerRace'
     * 3) Add a suitable handler method that delegates to the an <c>MacroDataSource</c> to the region <c>contextual macro handlers</c>. e.g.
     * <code>
     * public static string Region(IMacroContextProvider mcp)
     * {   // %reg
     *     return mcp.GetMacroDataSource().Region();
     * }
     * </code>
     * 4) Add an unimplemented implementation to the <c>MacroDataSource</c> base class. e.g.
     * <code>
     * public virtual string Region()
     * {   // %reg
     *     throw new NotImplementedException();
     * }
     * </code>
     * 5) Find the class that will provide the context (named using suffix 'MCP') and override the handler method. (<c>parent</c> refers to the context providing class) e.g.
     * <code>
     * public override string Region()
     * {
     *     return (parent.LastPersonReferenced != null) ? parent.LastPersonReferenced.HomeRegionName : "";
     * }
     * 
     */
    public static class MacroHelper
    {
        public delegate string MacroHandler(IMacroContextProvider mcp = null);

        #region macro definitions and handler mapping

        static Dictionary<string, MacroHandler> macroHandlers = new Dictionary<string, MacroHandler>()
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
            { "%agi", Agi }, //  Amount of Agility
            { "%arm", ItemName }, //  Armour
            { "%ark", AttributeRating }, // What property attribute is considered
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
            { "%dam", DmgMod }, // Damage modifyer
            { "%dat", Date }, // Date
            { "%di", null },  // Direction
            { "%dip", null }, // Days in prison
            { "%dng", null }, // Dungeon
            { "%dts", null }, // Daedra
            { "%dwr", null }, // Days (hours) with room remaining?
            { "%ef", null },  // Local shop name
            { "%enc", EncumbranceMax }, // Encumbrance
            { "%end", End }, // Amount of Endurance
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
            { "%hea", HpMod }, // HP Modifier
            { "%hmd", HealRateMod }, // Healing rate modifer
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
            { "%int", Int }, // Amount of Intelligence
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
            { "%luc", Luck }, // Luck
            { "%map", null }, // ?
            { "%mad", MagicResist }, // Resistance
            { "%mat", Material }, // Material
            { "%mit", null }, // Item
            { "%mn", null },  // Random First(?) name (Male?)
            { "%mn2", null }, // Same as _mn (?)
            { "%mod", ArmourMod }, // Modification
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
            { "%per", Per }, // Amount of Personality
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
            { "%spc", Magicka }, // Current Spell Points
            { "%ski", null }, // Skill
            { "%spd", Spd }, // Speed
            { "%spt", MagickaMax }, // Max spell points
            { "%str", Str }, // Amount of strength
            { "%sub", null }, // ?
            { "%t", null },   // Regent's Title
            { "%tcn", null }, // Travel city name
            { "%thd", ToHitMod }, // Combat odds
            { "%tim", Time }, // Time
            { "%vam", null }, // PC's vampire clan
            { "%vcn", null }, // Vampire's Clan
            { "%vn", null },  // ?
            { "%wdm", WeaponDamage }, // Weapon damage
            { "%wep", ItemName }, // Weapon
            { "%wil", Wil }, // ?
            { "%wpn", null }, // Poison (?)
            { "%wth", Worth }, // Worth
        };
        #endregion

        // Any punctuation characters that can be on the end of a macro symbol need adding here.
        static char[] PUNCTUATION = { '.', ',', '\'' };

        /// <summary>
        /// Expands any macros in the textfile tokens.
        /// </summary>
        /// <param name="tokens">a reference to textfile tokens to have macros expanded.</param>
        /// <param name="mcp">an object instance to provide context for macro expansion. (optional)</param>
        public static void ExpandMacros(ref TextFile.Token[] tokens, IMacroContextProvider mcp = null)
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
                                words[wordIdx] = GetValue(symbolStr, mcp) + words[wordIdx].Substring(wordLen);
                            }
                            else
                            {
                                words[wordIdx] = GetValue(words[wordIdx], mcp);
                            }
                        }
                        else if (words[wordIdx].StartsWith("+%"))
                        {   // Support willpower message which erroneously has the + just before the %. 
                            words[wordIdx] = '+' + GetValue(words[wordIdx].Substring(1), mcp);
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

        /// <summary>
        /// Gets the value for a single macro symbol string.
        /// </summary>
        /// <returns>The expanded macro value.</returns>
        /// <param name="symbolStr">macro symbol string.</param>
        /// <param name="mcp">an object instance providing context for macro expansion. (optional)</param>
        public static string GetValue(string symbolStr, IMacroContextProvider mcp)
        {
            if (macroHandlers.ContainsKey(symbolStr))
            {
                MacroHandler svp = macroHandlers[symbolStr];
                if (svp != null)
                {
                    try
                    {
                        return svp.Invoke(mcp);
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

        //
        // Global macro handlers - not context sensitive. (mcp will be null, and should not be used)
        //
        #region global macro handlers

        private static string CityName(IMacroContextProvider mcp)
        {   // %cn
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            if (gps.HasCurrentLocation)
                return gps.CurrentLocation.Name;
            else
                return gps.CurrentRegion.Name;
        }

        private static string CityName2(IMacroContextProvider mcp)
        {   // %cn2 (only used in msg #200)
            throw new NotImplementedException();
        }

        private static string CurrentRegion(IMacroContextProvider mcp)
        {   // %crn
            return GameManager.Instance.PlayerGPS.CurrentRegion.Name;
        }

        private static string LocalReputation(IMacroContextProvider mcp)
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

        private static string Time(IMacroContextProvider mcp)
        {   // %tim
            return DaggerfallUnity.Instance.WorldTime.Now.MinTimeString();
        }

        private static string Date(IMacroContextProvider mcp)
        {   // %dat
            return DaggerfallUnity.Instance.WorldTime.Now.DateString();
        }

        private static string PlayerName(IMacroContextProvider mcp)
        {   // %pcn
            return GameManager.Instance.PlayerEntity.Name;
        }

        private static string PlayerFirstname(IMacroContextProvider mcp)
        {   // %pcf
            string name = GameManager.Instance.PlayerEntity.Name;
            string[] parts = name.Split(' ');
            return (parts != null && parts.Length > 0) ? parts[0] : name;
        }

        private static string DmgMod(IMacroContextProvider mcp)
        {   // %dam
            return GameManager.Instance.PlayerEntity.DamageModifier.ToString();
        }
        private static string EncumbranceMax(IMacroContextProvider mcp)
        {   // %enc
            return GameManager.Instance.PlayerEntity.MaxEncumbrance.ToString();
        }

        private static string MagickaMax(IMacroContextProvider mcp)
        {   // %spc
            return GameManager.Instance.PlayerEntity.CurrentMagicka.ToString();
        }
        private static string Magicka(IMacroContextProvider mcp)
        {   // %spt
            return GameManager.Instance.PlayerEntity.MaxMagicka.ToString();
        }

        private static string MagicResist(IMacroContextProvider mcp)
        {   // %mad
            return GameManager.Instance.PlayerEntity.MagicResist.ToString();
        }
        private static string ToHitMod(IMacroContextProvider mcp)
        {   // %thd
            return GameManager.Instance.PlayerEntity.ToHitModifier.ToString();
        }
        private static string HpMod(IMacroContextProvider mcp)
        {   // %hea
            return GameManager.Instance.PlayerEntity.HitPointsModifier.ToString("+0;-0;0");
        }
        private static string HealRateMod(IMacroContextProvider mcp)
        {   // %hmd
            return GameManager.Instance.PlayerEntity.HealingRateModifier.ToString("+0;-0;0");
        }


        private static string PlayerRace(IMacroContextProvider mcp)
        {   // %ra
            return GameManager.Instance.PlayerEntity.RaceTemplate.Name;
        }

        private static string GuildTitle(IMacroContextProvider mcp)
        {   // %pct
            // Just use "Apprentice" for all %pct guild titles for now
            // Guilds are not implemented yet, will need to move into MacroDataSource
            return "Apprentice";
        }

        #endregion

        //
        // Contextual macro handlers - delegate to the macro data source provided by macro context provider.
        //
        #region contextual macro handlers

        private static string Str(IMacroContextProvider mcp)
        {   // %str
            return mcp.GetMacroDataSource().Str();
        }
        private static string Int(IMacroContextProvider mcp)
        {   // %int
            return mcp.GetMacroDataSource().Int();
        }
        private static string Wil(IMacroContextProvider mcp)
        {   // %wil
            return mcp.GetMacroDataSource().Wil();
        }
        private static string Agi(IMacroContextProvider mcp)
        {   // %agi
            return mcp.GetMacroDataSource().Agi();
        }
        private static string End(IMacroContextProvider mcp)
        {   // %end
            return mcp.GetMacroDataSource().End();
        }
        private static string Per(IMacroContextProvider mcp)
        {   // %per
            return mcp.GetMacroDataSource().Per();
        }
        private static string Spd(IMacroContextProvider mcp)
        {   // %spd
            return mcp.GetMacroDataSource().Spd();
        }
        private static string Luck(IMacroContextProvider mcp)
        {   // %luc
            return mcp.GetMacroDataSource().Luck();
        }

        private static string AttributeRating(IMacroContextProvider mcp)
        {   // %ark
            return mcp.GetMacroDataSource().AttributeRating();
        }

        public static string ItemName(IMacroContextProvider mcp)
        {   // %wep, %arm, %it
            return mcp.GetMacroDataSource().ItemName();
        }

        public static string Worth(IMacroContextProvider mcp)
        {   // %wth
            return mcp.GetMacroDataSource().Worth();
        }

        public static string Material(IMacroContextProvider mcp)
        {   // %mat
            return mcp.GetMacroDataSource().Material();
        }

        public static string Condition(IMacroContextProvider mcp)
        {   // %qua
            return mcp.GetMacroDataSource().Condition();
        }

        public static string Weight(IMacroContextProvider mcp)
        {   // %kg
            return mcp.GetMacroDataSource().Weight();
        }

        public static string WeaponDamage(IMacroContextProvider mcp)
        {   // %wdm
            return mcp.GetMacroDataSource().WeaponDamage();
        }

        public static string ArmourMod(IMacroContextProvider mcp)
        {   // %mod
            return mcp.GetMacroDataSource().ArmourMod();
        }

        public static string Pronoun(IMacroContextProvider mcp)
        {   // %g & %g1
            return mcp.GetMacroDataSource().Pronoun();
        }
        public static string Pronoun2(IMacroContextProvider mcp)
        {   // %g2
            return mcp.GetMacroDataSource().Pronoun2();
        }
        public static string Pronoun2self(IMacroContextProvider mcp)
        {   // %g2self
            return mcp.GetMacroDataSource().Pronoun2self();
        }
        public static string Pronoun3(IMacroContextProvider mcp)
        {   // %g3
            return mcp.GetMacroDataSource().Pronoun3();
        }

        public static string QuestDate(IMacroContextProvider mcp)
        {   // %qdt
            return mcp.GetMacroDataSource().QuestDate();
        }

        public static string Oath(IMacroContextProvider mcp)
        {   // %oth
            return mcp.GetMacroDataSource().Oath();
        }

        public static string Region(IMacroContextProvider mcp)
        {   // %reg
            return mcp.GetMacroDataSource().Region();
        }

        public static string God(IMacroContextProvider mcp)
        {   // %god
            return mcp.GetMacroDataSource().God();
        }

        #endregion
    }

}