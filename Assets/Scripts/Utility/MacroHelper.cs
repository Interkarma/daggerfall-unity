// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    Numidium, Gavin Clayton

using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using UnityEngine;

namespace DaggerfallWorkshop.Utility
{
    /**
     * <summary>
     * Helper class for context sensitive macros like '%abc' that're used in following Daggerfall files:
     * arena2\text.rsc, fall.exe, arena2\*.qrc, or arena2\bio*.txt
     * </summary>
     * 
     * See http://forums.dfworkshop.net/viewtopic.php?f=23&t=673 for details about adding new macro handlers.
     * 
     */
    public static class MacroHelper
    {
        public delegate string MacroHandler(IMacroContextProvider mcp = null);

        public delegate TextFile.Token[] MultilineMacroHandler(IMacroContextProvider mcp, TextFile.Formatting format);

        #region macro definitions and handler mappings

        static Dictionary<string, MacroHandler> macroHandlers = new Dictionary<string, MacroHandler>()
        {
            { "%1am", MagnitudePlusMin }, // 1st + Magnitude
            { "%1bm", MagnitudeBaseMin }, // 1st base Magnitude
            { "%1com", GreetingOrFollowUpText },// Greeting (?)
            { "%1hn", null }, // ?
            { "%2am", MagnitudePlusMax }, // 2nd + Magnitude
            { "%2bm", MagnitudeBaseMax }, // 2nd Base Magnitude
            { "%2com", DummyResolve2com },// ? (comment Nystul: it seems to be used in questions about work - it seems to be resolved to an empty string but not sure what else this macro does)
            { "%2hn", null }, // ?
            { "%3hn", null }, // ?
            { "%a", Amount },   // Cost of somthing.
            { "%ach", ChancePlus }, // + Chance plus
            { "%adr", DurationPlus }, // + Duration plus
            { "%agi", Agi }, //  Amount of Agility
            { "%ark", AttributeRating }, // What property attribute is considered
            { "%arm", ItemName }, //  Armour
            { "%ba", BookAuthor },  // Book Author
            { "%bch", ChanceBase }, // Base chance
            { "%bdr", DurationBase }, // Base Duration
            { "%bn", null },  // ?
            { "%bt", ItemName },  // Book title
            { "%cbl", null }, // Cash balance in current region
            { "%clc", ChancePerLevel }, // Per level (Chance)
            { "%cld", DurationPerLevel }, // Per level (Duration)
            { "%clm", MagnitudePerLevel }, // Per level (Magnitude)
            { "%cn", CityName },  // City name
            { "%cn2", CityName2 }, // City name #2
            { "%cpn", ShopName }, // Current shop name
            { "%cri", Crime }, // Accused crime
            { "%crn", CurrentRegion }, // Current Region
            { "%ct", CityType }, // City type? e.g city, town, village?
            { "%dae", null }, // A daedra
            { "%dam", DmgMod }, // Damage modifyer
            { "%dat", Date }, // Date
            { "%di", LocationDirection },  // Direction
            { "%dip", DaysInPrison }, // Days in prison
            { "%dng", Dungeon }, // Dungeon
            { "%dts", null }, // Daedra
            { "%dwr", RoomHoursLeft }, // Hours with room remaining.
            { "%ef", null },  // Local shop name
            { "%enc", EncumbranceMax }, // Encumbrance
            { "%end", End }, // Amount of Endurance
            { "%fcn", LocationOfRegionalBuilding }, // Location with regional building asked about
            { "%fe", null },  // ?
            { "%fea", null }, // ?
            { "%fl1", LordOfFaction1 }, // Lord of _fx1
            { "%fl2", LordOfFaction2 }, // Lord of _fx2
            { "%fn", null },  // Random first(?) name (Female?)
            { "%fn2", null }, // Same as _mn2 (?)
            { "%fnpc", GuildNPC }, // faction of npc that is dialog partner
            { "%fon", FactionOrderName }, // Faction order name
            { "%fpa", FactionName }, // faction name? of dialog partner - should return "Kynareth" for npc that are members of "Temple of Kynareth"
            { "%fpc", FactionPC }, // faction of pc that is from importance to dialog partner (same as his faction)
            { "%fx1", AFactionInNews }, // A faction in news
            { "%fx2", AnotherFactionInNews }, // Another faction in news
            { "%g", Pronoun },   // He/She etc...
            { "%g1", Pronoun },  // He/She ???
            { "%g2", Pronoun2 },  // Him/Her etc...
            { "%g2self", Pronoun2self },// Himself/Herself etc...
            { "%g3", Pronoun3 },  // His/Hers/Theirs etc...
            { "%gii", GoldCarried }, // Amount of gold in hand
            { "%gdd", GodDesc }, // God description i.e. God of Logic
            { "%god", God }, // Some god (listed in TEXT.RSC)
            { "%gtp", GoldToPay }, // Amount of fine
            { "%hea", HpMod }, // HP Modifier
            { "%hmd", HealRateMod }, // Healing rate modifer
            { "%hnr", Honorific }, // Honorific in guild/faction (note Nystul: vanilla resolved this for my male character to "Sir")
            { "%hnt", DialogHint }, // context "Tell Me About": anyInfo message, context place: Direction of location. (comment Nystul: it is either a location direction hint or a map reveal)
            { "%hnt2", DialogHint2 }, // context "Tell Me About": rumors message
            { "%hol", null }, // Holiday
            { "%hpn", null }, // ?
            { "%hpw", null }, // ?
            { "%hrg", null }, // House region
            { "%hs", HeldSoul },  //  Holding Soul type
            { "%htwn", null },// House town
            { "%imp", null }, // ?
            { "%int", Int }, // Amount of Intelligence
            { "%it", ItemName },  //  Item
            { "%jok", Joke }, // A joke
            { "%key", DialogKeySubject }, // A location (?) (comment Nystul: it is the topic you are asking about (e.g. building, work, etc.) how it seems)
            { "%key2", null },// Another location
            { "%kg", Weight },  //  Weight of items
            { "%kno", FactionOrderName }, // A knightly guild name
            { "%lev", GuildTitle }, // Rank in guild that you are in.
            { "%lp", LocalPalace },  //  Local / palace (?) dungeon
            { "%ln", null },  //  Random lastname
            { "%loc", MarkLocationOnMap }, // Location marked on map (comment Nystul: this seems to be context dependent - it is used both in direction dialogs (7333) and map reveal dialogs (7332) - it seems to return the name of the building and reveal the map only if a 7332 dialog was chosen
            { "%lt1", TitleOfLordOfFaction1 }, // Title of _fl1
            { "%ltn", LegalReputation }, // In the eyes of the law you are.......
            { "%luc", Luck }, // Luck
            { "%mad", MagicResist }, // Resistance
            { "%map", LocationRevealedByMapItem }, // Name of location revealed by a map item
            { "%mat", Material }, // Material
            { "%mit", null }, // Item
            { "%ml", MaxLoan },  // Max loan amount
            { "%mn", null },  // Random First(?) name (Male?)
            { "%mn2", null }, // Same as _mn (?)
            { "%mod", ArmourMod }, // Modification
            { "%n", NameDialogPartner },   // A random female first name (comment Nystul: I think it is just a random name - or maybe this is the reason that in vanilla all male mobile npcs have female names...)
            { "%nam", null }, // A random full name
            { "%nrn", null }, // Noble of the current region (used in: O0B00Y01)
            { "%nt", NearbyTavern },  // Nearby Tavern
            { "%ol1", OldLordOfFaction1 }, // Old lord of _fx1
            { "%olf", OldLeaderFate }, // What happened to _ol1
            { "%on", null },  // ?
            { "%oth", Oath }, // An oath (listed in TEXT.RSC)
            { "%pc", null },  // ?
            { "%pcf", PlayerFirstname }, // Character's first name
            { "%pcn", PlayerName }, // Character's full name
            { "%pct", GuildTitle }, // Player guild title/rank
            { "%pdg", null }, // Days in jail
            { "%pen", Penalty }, // Crime penalty
            { "%per", Per }, // Amount of Personality
            { "%plq", null }, // Place of something in log.
            { "%pnq", null }, // Person of something in log
            { "%po", Potion }, //  Potion
            { "%pp1", null }, // ?
            { "%pp2", null }, // ?
            { "%pqn", PotentialQuestorName }, // Potential Quest Giver
            { "%pqp", PotentialQuestorLocation }, // Potential Quest Giver's Location
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
			{ "%reg", RegionInContext }, // Region in context
            { "%rn", null },  // Regent's Name
            { "%rt", RegentTitle },  // Regent's Title
            { "%spc", Magicka }, // Current Spell Points
            { "%ski", Skill }, // Mastered skill name
            { "%spd", Spd }, // Speed
            { "%spt", MagickaMax }, // Max spell points
            { "%str", Str }, // Amount of strength
            { "%sub", null }, // ?
            { "%t", RegentTitle },  // Regent's Title
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
        // DF Unity - new macros:
            { "%pg", PlayerPronoun },   // He/She (player)
            { "%pg1", PlayerPronoun },  // His/Her (player)
            { "%pg2", PlayerPronoun2 }, // Him/Her (player)
            { "%pg2self", PlayerPronoun2self },// Himself/Herself (player)
            { "%pg3", PlayerPronoun3 },  // His/Hers (player)
			{ "%hrn", HomeRegion },  // Home region (of person)
        };

        // Multi-line macro handlers, returns tokens.
        static Dictionary<string, MultilineMacroHandler> multilineMacroHandlers = new Dictionary<string, MultilineMacroHandler>()
        {
            { "%mpw", MagicPowers }, // Magic powers - multi line (token) message
        };

        #endregion

        #region fields (some macros need state (e.g. %fn2 depends on %fn1)

        static int idFaction1InNews = -1;
        static int idFaction1Ruler = -1;

        #endregion

        #region Public Functions

        public static void ResetFactionAndRulerIds()
        {
            idFaction1InNews = -1;
            idFaction1Ruler = -1;
        }

        #endregion

        // Any punctuation characters that can be on the end of a macro symbol need adding here.
        static char[] PUNCTUATION = { '.', ',', '\'', '?', '!' };

        /// <summary>
        /// Expands any macros in the textfile tokens.
        /// </summary>
        /// <param name="tokens">a reference to textfile tokens to have macros expanded.</param>
        /// <param name="mcp">an object instance to provide context for macro expansion. (optional)</param>
        public static void ExpandMacros(ref TextFile.Token[] tokens, IMacroContextProvider mcp = null)
        {
            // Iterate message tokens
            string tokenText;
            int multilineIdx = 0;
            TextFile.Token[] multilineTokens = null;

            // this dictionary is used check for previous resolving of a specific macro in the current call to ExpandMacros before expanding macros
            // if macro (macro string used as dict key) has been expanded before use previous expanded string (value of this dict) as result
            // this dict is newly created (and thus empty) for every new call to this function call so macros will be expanded in future calls to ExpandMacros
            Dictionary<string, string> macrosExpandedAlready = new Dictionary<string, string>();

            for (int tokenIdx = 0; tokenIdx < tokens.Length; tokenIdx++)
            {
                tokenText = tokens[tokenIdx].text;
                if (tokenText != null && tokenText.IndexOf('%') >= 0)
                {
                    // Handle multiline macros. (only handles the last one & must be only thing in this token)
                    if (multilineMacroHandlers.ContainsKey(tokenText.Trim()))
                    {
                        multilineTokens = GetMultilineValue(tokenText.Trim(), mcp, tokens[tokenIdx + 1].formatting);
                        multilineIdx = tokenIdx;
                    }
                    else
                    {
                        // Split token text into individual words
                        string[] words = tokenText.Split(' ');

                        // Iterate words to find macros
                        for (int wordIdx = 0; wordIdx < words.Length; wordIdx++)
                        {
                            int pos = words[wordIdx].IndexOf('%');
                            if (pos >= 0)
                            {
                                string prefix = words[wordIdx].Substring(0, pos);
                                string macro = words[wordIdx].Substring(pos);

                                // don't expand macros several times in same expand macro command (when still in one run of this function)
                                // since some macros produce different results when expanded several times (macros with random generated names, e.g. %fx1, %fx2)
                                if (macrosExpandedAlready.ContainsKey(macro))
                                {
                                    words[wordIdx] = prefix + macrosExpandedAlready[macro];
                                    continue;
                                }

                                if (macro.StartsWith("%"))
                                {
                                    int macroLen;
                                    if ((macroLen = macro.IndexOfAny(PUNCTUATION)) > 0)
                                    {
                                        string symbolStr = macro.Substring(0, macroLen);
                                        string expandedString = GetValue(symbolStr, mcp);
                                        words[wordIdx] = prefix + expandedString + macro.Substring(macroLen);
                                        macrosExpandedAlready[macro] = expandedString;
                                    }
                                    else
                                    {
                                        string expandedString = GetValue(macro, mcp);
                                        words[wordIdx] = prefix + expandedString;
                                        macrosExpandedAlready[macro] = expandedString;
                                    }
                                }
                            }
                        }
                        // Re-assemble words and update token.
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
            // Insert multiline tokens if generated.
            if (multilineTokens != null && multilineTokens.Length > 0)
            {
                TextFile.Token[] newTokens = new TextFile.Token[tokens.Length + multilineTokens.Length - 1];
                Array.Copy(tokens, newTokens, multilineIdx);
                Array.Copy(multilineTokens, 0, newTokens, multilineIdx, multilineTokens.Length);
                Array.Copy(tokens, multilineIdx + 1, newTokens, multilineIdx + multilineTokens.Length, tokens.Length - multilineIdx - 1);
                tokens = newTokens;
            }
        }

        /// <summary>
        /// Gets the value for a single macro symbol string.
        /// </summary>
        /// <returns>The expanded macro value.</returns>
        /// <param name="symbolStr">macro symbol string.</param>
        /// <param name="mcp">an object instance providing context for macro expansion.</param>
        /// <param name="mcp2">an object instance providing secondary context for macro expansion.</param>
        public static string GetValue(string symbolStr, IMacroContextProvider mcp, IMacroContextProvider mcp2 = null)
        {
            if (macroHandlers.ContainsKey(symbolStr))
            {
                MacroHandler svp = macroHandlers[symbolStr];
                if (svp != null)
                {
                    try {
                        return svp.Invoke(mcp);
                    } catch (NotImplementedException) {
                        if (mcp2 != null) {
                            try { return svp.Invoke(mcp2); } catch (NotImplementedException) { }
                        }
                        return symbolStr + "[srcDataUnknown]";
                    }
                } else {
                    return symbolStr + "[unhandled]";
                }
            } else {
                return symbolStr + "[undefined]";
            }
        }

        /// <summary>
        /// Gets a multiline value for a single macro symbol string.
        /// </summary>
        /// <returns>The multiline expanded macro value as a Token array.</returns>
        /// <param name="symbolStr">macro symbol string.</param>
        /// <param name="mcp">an object instance providing context for macro expansion.</param>
        /// <param name="format">the format tag to follow each line. (can be null)</param>
        public static TextFile.Token[] GetMultilineValue(string symbolStr, IMacroContextProvider mcp, TextFile.Formatting format)
        {
            string error;
            if (format == TextFile.Formatting.Text)
                format = TextFile.Formatting.NewLine;
            MultilineMacroHandler svp = multilineMacroHandlers[symbolStr];
            if (svp != null) {
                try {
                    return svp.Invoke(mcp, format);
                } catch (NotImplementedException) {
                    error = symbolStr + "[srcDataUnknown]";
                }
            } else {
                error = symbolStr + "[unhandled]";
            }
            TextFile.Token errorToken = new TextFile.Token();
            errorToken.text = error;
            errorToken.formatting = TextFile.Formatting.Text;
            return new TextFile.Token[] { errorToken };
        }


        //
        // Global macro handlers - not context sensitive. (mcp will be null, and should not be used)
        //
        #region Global macro handlers

        private static string CityName(IMacroContextProvider mcp)
        {   // %cn
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            if (gps.HasCurrentLocation)
                return gps.CurrentLocation.Name;
            else
                return gps.CurrentRegion.Name;
        }

        private static string CityName2(IMacroContextProvider mcp)
        {   // %cn2 (only used in msg #200 where cn and cn2 are random? places)
            throw new NotImplementedException();
        }

        private static string CurrentRegion(IMacroContextProvider mcp)
        {   // %crn
            return GameManager.Instance.PlayerGPS.CurrentRegion.Name;
        }

        private static string CityType(IMacroContextProvider mcp)
        {   // %ct
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            switch (gps.CurrentLocationType)
            {
                case DFRegion.LocationTypes.TownCity:
                    return HardStrings.city;
                case DFRegion.LocationTypes.TownVillage:
                    return HardStrings.village;
                case DFRegion.LocationTypes.TownHamlet:
                    return HardStrings.hamlet;
                case DFRegion.LocationTypes.HomeFarms:
                    return HardStrings.farm;
                case DFRegion.LocationTypes.HomePoor:
                    return HardStrings.shack;
                case DFRegion.LocationTypes.HomeWealthy:
                    return HardStrings.manor;
                case DFRegion.LocationTypes.Tavern:
                    return HardStrings.community;
                case DFRegion.LocationTypes.ReligionTemple:
                    return HardStrings.shrine;
                default:
                    return gps.CurrentLocationType.ToString();
            }
        }

        private static string LocalPalace(IMacroContextProvider mcp)
        {   // %lp - kinda guessing for this one
            BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
            if (buildingDirectory && buildingDirectory.BuildingCount > 0)
            {
                List<BuildingSummary> palaces = buildingDirectory.GetBuildingsOfType(DFLocation.BuildingTypes.Palace);
                if (palaces.Count >= 1)
                {
                    Debug.LogFormat("Location {1} has a palace with buildingKey: {0}", palaces[0].buildingKey, GameManager.Instance.PlayerGPS.CurrentLocation.Name);
                    PlayerGPS.DiscoveredBuilding palace;
                    if (GameManager.Instance.PlayerGPS.GetAnyBuilding(palaces[0].buildingKey, out palace))
                        return palace.displayName.TrimEnd('.');
                }
            }
            return HardStrings.local;
        }

        private static string NearbyTavern(IMacroContextProvider mcp)
        {   // %nt - just gets a random tavern from current location and ignores how near it is.
            BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
            if (buildingDirectory && buildingDirectory.BuildingCount > 0)
            {
                List<BuildingSummary> taverns = buildingDirectory.GetBuildingsOfType(DFLocation.BuildingTypes.Tavern);
                int i = UnityEngine.Random.Range(0, taverns.Count - 1);
                PlayerGPS.DiscoveredBuilding tavern;
                if (GameManager.Instance.PlayerGPS.GetAnyBuilding(taverns[i].buildingKey, out tavern))
                    return tavern.displayName;
            }
            return HardStrings.tavern;
        }

        private static string RegentTitle(IMacroContextProvider mcp)
        {   // %rt %t
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            FactionFile.FactionData regionFaction;
            GameManager.Instance.PlayerEntity.FactionData.FindFactionByTypeAndRegion(7, gps.CurrentRegionIndex + 1, out regionFaction);

            switch (regionFaction.ruler)
            {
                case 1:
                    return HardStrings.King;
                case 2:
                    return HardStrings.Queen;
                case 3:
                    return HardStrings.Duke;
                case 4:
                    return HardStrings.Duchess;
                case 5:
                    return HardStrings.Marquis;
                case 6:
                    return HardStrings.Marquise;
                case 7:
                    return HardStrings.Count;
                case 8:
                    return HardStrings.Countess;
                case 9:
                    return HardStrings.Baron;
                case 10:
                    return HardStrings.Baroness;
                case 11:
                    return HardStrings.Lord;
                case 12:
                    return HardStrings.Lady;
                default:
                    return HardStrings.Lord;
            }
        }

        private static string Crime(IMacroContextProvider mcp)
        {   // %cri
            switch ((int)GameManager.Instance.PlayerEntity.CrimeCommitted)
            {
                case 1:
                    return HardStrings.Attempted_Breaking_And_Entering;
                case 2:
                    return HardStrings.Trespassing;
                case 3:
                    return HardStrings.Breaking_And_Entering;
                case 4:
                    return HardStrings.Assault;
                case 5:
                    return HardStrings.Murder;
                case 6:
                    return HardStrings.Tax_Evasion;
                case 7:
                    return HardStrings.Criminal_Conspiracy;
                case 8:
                    return HardStrings.Vagrancy;
                case 9:
                    return HardStrings.Smuggling;
                case 10:
                    return HardStrings.Piracy;
                case 11:
                    return HardStrings.High_Treason;
                case 12:
                    return HardStrings.Pickpocketing;
                case 13:
                    return HardStrings.Theft;
                case 14:
                    return HardStrings.Treason;
                default:
                    return "None";
            }
        }

        private static string Penalty(IMacroContextProvider mcp)
        {   // %pen
            int punishmentType = DaggerfallUI.Instance.DfCourtWindow.PunishmentType;

            if (punishmentType == 2)
            {
                TextFile.Token[] tokens = { TextFile.CreateTextToken(HardStrings.Regular_Punishment_String) };
                ExpandMacros(ref tokens);
                return tokens[0].text;
            }
            else if (punishmentType == 1)
                return HardStrings.Execution;
            else return HardStrings.Banishment;

        }

        private static string GoldToPay(IMacroContextProvider mcp)
        {   // %gtp
            return DaggerfallUI.Instance.DfCourtWindow.Fine.ToString();
        }

        private static string DaysInPrison(IMacroContextProvider mcp)
        {   // %dip
            return DaggerfallUI.Instance.DfCourtWindow.DaysInPrison.ToString();
        }

        private static string LegalReputation(IMacroContextProvider mcp)
        {   // %ltn
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            int rep = GameManager.Instance.PlayerEntity.RegionData[gps.CurrentRegionIndex].LegalRep;

            if (rep > 80)
                return HardStrings.revered;
            else if (rep > 60)
                return HardStrings.esteemed;
            else if (rep > 40)
                return HardStrings.honored;
            else if (rep > 20)
                return HardStrings.admired;
            else if (rep > 10)
                return HardStrings.respected;
            else if (rep > 0)
                return HardStrings.dependable;
            else if (rep == 0)
                return HardStrings.aCommonCitizen;
            else if (rep < -80)
                return HardStrings.hated;
            else if (rep < -60)
                return HardStrings.pondScum;
            else if (rep < -40)
                return HardStrings.aVillain;
            else if (rep < -20)
                return HardStrings.aCriminal;
            else if (rep < -10)
                return HardStrings.aScoundrel;
            else if (rep < 0)
                return HardStrings.undependable;

            return HardStrings.unknown;
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
        public static string PlayerPronoun(IMacroContextProvider mcp)
        {   // %pg
            return (GameManager.Instance.PlayerEntity.Gender == Genders.Female) ? HardStrings.pronounShe : HardStrings.pronounHe;
        }
        public static string PlayerPronoun1(IMacroContextProvider mcp)
        {   // %pg1
            return (GameManager.Instance.PlayerEntity.Gender == Genders.Female) ? HardStrings.pronounHer : HardStrings.pronounHis;
        }
        public static string PlayerPronoun2(IMacroContextProvider mcp)
        {   // %pg2
            return (GameManager.Instance.PlayerEntity.Gender == Genders.Female) ? HardStrings.pronounHer : HardStrings.pronounHim;
        }
        public static string PlayerPronoun2self(IMacroContextProvider mcp)
        {   // %pg2self
            return (GameManager.Instance.PlayerEntity.Gender == Genders.Female) ? HardStrings.pronounHerself : HardStrings.pronounHimself;
        }
        public static string PlayerPronoun3(IMacroContextProvider mcp)
        {   // %pg3
            return (GameManager.Instance.PlayerEntity.Gender == Genders.Female) ? HardStrings.pronounHers : HardStrings.pronounHis;
        }

        private static string Honorific(IMacroContextProvider mcp)
        {   // %hnr
            return GameManager.Instance.TalkManager.GetHonoric();
        }

        private static string DmgMod(IMacroContextProvider mcp)
        {   // %dam
            return GameManager.Instance.PlayerEntity.DamageModifier.ToString("+0;-0;0");
        }
        private static string EncumbranceMax(IMacroContextProvider mcp)
        {   // %enc
            return GameManager.Instance.PlayerEntity.MaxEncumbrance.ToString();
        }

        private static string MagickaMax(IMacroContextProvider mcp)
        {   // %spt
            return GameManager.Instance.PlayerEntity.MaxMagicka.ToString();
            
        }
        private static string Magicka(IMacroContextProvider mcp)
        {   // %spc
            return GameManager.Instance.PlayerEntity.CurrentMagicka.ToString();
        }
        private static string Skill(IMacroContextProvider mcp)
        {   // %ski
            List<DFCareer.Skills> primarySkills = GameManager.Instance.PlayerEntity.GetPrimarySkills();
            foreach (DFCareer.Skills skill in primarySkills)
            {
                if (GameManager.Instance.PlayerEntity.Skills.GetPermanentSkillValue(skill) == 100)
                    return DaggerfallUnity.Instance.TextProvider.GetSkillName(skill);
            }
            return "BLANK";
        }

        private static string MagicResist(IMacroContextProvider mcp)
        {   // %mad
            return GameManager.Instance.PlayerEntity.MagicResist.ToString();
        }
        private static string ToHitMod(IMacroContextProvider mcp)
        {   // %thd
            return GameManager.Instance.PlayerEntity.ToHitModifier.ToString("+0;-0;0");
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

        private static string GoldCarried(IMacroContextProvider mcp)
        {   // %gii
            return GameManager.Instance.PlayerEntity.GoldPieces.ToString();
        }

        private static string Joke(IMacroContextProvider mcp)
        {   // %jok
            return DaggerfallUnity.Instance.TextProvider.GetRandomText(200);
        }

        private static string GreetingOrFollowUpText(IMacroContextProvider mcp)
        {
            // %1com
            return GameManager.Instance.TalkManager.GetPCGreetingOrFollowUpText();
        }

        private static string DummyResolve2com(IMacroContextProvider mcp)
        {
            // %2com
            return ""; // return empty string for now - not known if it does something else in classic
        }

        private static string NameDialogPartner(IMacroContextProvider mcp)
        {
            // %n
            return GameManager.Instance.TalkManager.NameNPC;
        }

        private static string FactionPC(IMacroContextProvider mcp)
        {   // %fpc
            return GameManager.Instance.TalkManager.GetFactionPC();
        }

        private static string GuildNPC(IMacroContextProvider mcp)
        {   // %fnpc
            return GameManager.Instance.TalkManager.GetGuildNPC();
        }

        private static string FactionName(IMacroContextProvider mcp)
        {   // %fpa
            return GameManager.Instance.TalkManager.GetFactionName();
        }

        public static string AFactionInNews(IMacroContextProvider mcp)
        {   // %fx1
            PersistentFactionData factions = GameManager.Instance.PlayerEntity.FactionData;
            int id;
            if (idFaction1Ruler == -1) // no previous %ol1
            {
                id = UnityEngine.Random.Range(0, TalkManager.factionsUsedForFactionInNews.Count - 1);
                idFaction1InNews = id;
            }
            else
            {
                id = idFaction1Ruler;
            }
            FactionFile.FactionData fd;
            factions.GetFactionData((int)TalkManager.factionsUsedForFactionInNews[id], out fd);
            return fd.name;
        }

        public static string AnotherFactionInNews(IMacroContextProvider mcp)
        {   // %fx2
            PersistentFactionData factions = GameManager.Instance.PlayerEntity.FactionData;
            // get random number between 0 and factionsUsedForFactionInNews.Count - 2 now since we might add a + 1 for an id >= idFaction1InNews later to prevent same faction as for %fx1
            int id = UnityEngine.Random.Range(0, TalkManager.factionsUsedForFactionInNews.Count - 1);
            FactionFile.FactionData fd;
            if (id >= idFaction1InNews) // make sure to create an id != idFaction1InNews
                id += 1; // by just adding 1 if id >= idFaction1InNews -> so we will end up with an id in ranges [0, idFaction1InNews) union (idFaction1InNews, factionsUsedForFactionInNews.Count]
            factions.GetFactionData((int)TalkManager.factionsUsedForFactionInNews[id], out fd);
            return fd.name;
        }

        public static string OldLeaderFate(IMacroContextProvider mcp)
        {   // %olf
            int index = UnityEngine.Random.Range(0, 5);
            return TalkManager.Instance.GetOldLeaderFateString(index);
        }

        private static string HelperCreateLordNameForFaction(int factionId)
        {
            PersistentFactionData factions = GameManager.Instance.PlayerEntity.FactionData;
            FactionFile.FactionData fd;
            factions.GetFactionData(factionId, out fd);

            Genders gender = (Genders)((fd.ruler + 1) % 2); // even entries are female titles/genders, odd entries are male ones

            Races race = (Races)fd.race;

            Game.Utility.NameHelper.BankTypes nameBankType;
            switch (race)
            {
                case Races.Argonian:
                case Races.Breton:
                case Races.Khajiit:
                default:
                    nameBankType = Game.Utility.NameHelper.BankTypes.Breton;
                    break;
                case Races.DarkElf:
                    nameBankType = Game.Utility.NameHelper.BankTypes.DarkElf;
                    break;
                case Races.HighElf:
                    nameBankType = Game.Utility.NameHelper.BankTypes.HighElf;
                    break;
                case Races.WoodElf:
                    nameBankType = Game.Utility.NameHelper.BankTypes.WoodElf;
                    break;
                case Races.Nord:
                    nameBankType = Game.Utility.NameHelper.BankTypes.Nord;
                    break;
                case Races.Redguard:
                    nameBankType = Game.Utility.NameHelper.BankTypes.Redguard;
                    break;
            }
            return DaggerfallUnity.Instance.NameHelper.FullName(nameBankType, gender);
        }

        public static string OldLordOfFaction1(IMacroContextProvider mcp)
        {   // %ol1                    
            int id;
            if (idFaction1Ruler == -1)
            {
                id = UnityEngine.Random.Range(0, TalkManager.factionsUsedForRulers.Count - 1);
                idFaction1Ruler = id;
            }
            else
            {
                id = idFaction1Ruler;
            }
            return HelperCreateLordNameForFaction((int)TalkManager.factionsUsedForRulers[id]);
        }

        public static string LordOfFaction1(IMacroContextProvider mcp)
        {   // %fl1
            int id;
            if (idFaction1Ruler == -1)
            {
                id = UnityEngine.Random.Range(0, TalkManager.factionsUsedForRulers.Count - 1);
                idFaction1Ruler = id;
            }
            else
            {
                id = idFaction1Ruler;
            }
            return HelperCreateLordNameForFaction((int)TalkManager.factionsUsedForRulers[id]);
        }

        public static string LordOfFaction2(IMacroContextProvider mcp)
        {   // %fl2
            // get random number between 0 and factionsUsedForRulers.Count - 2 now since we might add a + 1 for an id >= idFaction1Ruler later to prevent same faction as for %fl1
            int id = UnityEngine.Random.Range(0, TalkManager.factionsUsedForRulers.Count - 2);
            if (id >= idFaction1Ruler) // make sure to create an id != idFaction1Ruler
                id += 1; // by just adding 1 if id >= idFaction1InNews -> so we will end up with an id in ranges [0, idFaction1Ruler) union (idFaction1InNews, factionsUsedForFactionRulers.Count]
            return HelperCreateLordNameForFaction((int)TalkManager.factionsUsedForRulers[id]);
        }

        public static string TitleOfLordOfFaction1(IMacroContextProvider mcp)
        {   // %lt1
            int id;
            if (idFaction1Ruler == -1)
            {
                id = UnityEngine.Random.Range(0, TalkManager.factionsUsedForRulers.Count - 1);
                idFaction1Ruler = id;
            }
            else
            {
                id = idFaction1Ruler;
            }

            PersistentFactionData factions = GameManager.Instance.PlayerEntity.FactionData;
            FactionFile.FactionData fd;
            factions.GetFactionData((int)TalkManager.factionsUsedForRulers[id], out fd);

            switch (fd.ruler)
            {
                case 1:
                    return HardStrings.King;
                case 2:
                    return HardStrings.Queen;
                case 3:
                    return HardStrings.Duke;
                case 4:
                    return HardStrings.Duchess;
                case 5:
                    return HardStrings.Marquis;
                case 6:
                    return HardStrings.Marquise;
                case 7:
                    return HardStrings.Count;
                case 8:
                    return HardStrings.Countess;
                case 9:
                    return HardStrings.Baron;
                case 10:
                    return HardStrings.Baroness;
                case 11:
                    return HardStrings.Lord;
                case 12:
                    return HardStrings.Lady;
                default:
                    return HardStrings.Lord;
            }
        }

        public static string RegionInContext(IMacroContextProvider mcp)
        {   // %reg
            if (idFaction1Ruler != -1)
            {
                //return DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegionName((int)TalkManager.factionsUsedForRulers[idFaction1Ruler]); // not mapping to same regions for some reason as FactionFile.FactionIDs enum
                string regionName = Enum.GetName(typeof(FactionFile.FactionIDs), (FactionFile.FactionIDs)TalkManager.factionsUsedForRulers[idFaction1Ruler]);
                regionName = regionName.Replace('_', ' ');
                return regionName;

            }
            else
                return CurrentRegion(mcp);
        }

        private static string DialogKeySubject(IMacroContextProvider mcp)
        {
            // %key
            switch (GameManager.Instance.TalkManager.CurrentKeySubjectType)
            {
                case TalkManager.KeySubjectType.Unset:
                default:
                    return "";
                case TalkManager.KeySubjectType.Building:
                    return GameManager.Instance.TalkManager.CurrentKeySubject;
                case TalkManager.KeySubjectType.Person:
                    return GameManager.Instance.TalkManager.CurrentKeySubject;
                case TalkManager.KeySubjectType.Thing:
                    return GameManager.Instance.TalkManager.CurrentKeySubject;
                case TalkManager.KeySubjectType.Work:
                    return GameManager.Instance.TalkManager.GetWorkString();
                case TalkManager.KeySubjectType.QuestTopic:
                    if (GameManager.Instance.TalkManager.CurrentQuestionListItem != null)
                        return GameManager.Instance.TalkManager.CurrentQuestionListItem.caption;
                    else
                        return GameManager.Instance.TalkManager.CurrentKeySubject;
                case TalkManager.KeySubjectType.Organization:
                    return GameManager.Instance.TalkManager.CurrentKeySubject;
            }
        }

        private static string MarkLocationOnMap(IMacroContextProvider mcp)
        {
            // %loc
            if (GameManager.Instance.TalkManager.MarkLocationOnMap)
                GameManager.Instance.TalkManager.MarkKeySubjectLocationOnMap();
            return GameManager.Instance.TalkManager.CurrentKeySubject;
        }

        private static string LocationRevealedByMapItem(IMacroContextProvider mcp)
        {
            // %map
            return GameManager.Instance.PlayerGPS.LocationRevealedByMapItem;
        }

        private static string LocationOfRegionalBuilding(IMacroContextProvider mcp)
        {
            // %fcn
            return GameManager.Instance.TalkManager.LocationOfRegionalBuilding;
        }

        #endregion

        //
        // Contextual macro handlers - delegate to the macro data source provided by macro context provider.
        //
        #region Contextual macro handlers

        private static string GuildTitle(IMacroContextProvider mcp)
        {   // %lev %pct
            return mcp.GetMacroDataSource().GuildTitle();
        }

        private static string FactionOrderName(IMacroContextProvider mcp)
        {   // %fon
            return mcp.GetMacroDataSource().FactionOrderName();
        }

        public static string Dungeon(IMacroContextProvider mcp)
        {   // %dng
            return mcp.GetMacroDataSource().Dungeon();
        }

        private static string Amount(IMacroContextProvider mcp)
        {   // %a
            return mcp.GetMacroDataSource().Amount();
        }
        private static string ShopName(IMacroContextProvider mcp)
        {   // %cpn
            return mcp.GetMacroDataSource().ShopName();
        }
        private static string MaxLoan(IMacroContextProvider mcp)
        {   // %ml
            return mcp.GetMacroDataSource().MaxLoan();
        }

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

        public static string BookAuthor(IMacroContextProvider mcp)
        {   // %ba
            return mcp.GetMacroDataSource().BookAuthor();
        }

        public static string HeldSoul(IMacroContextProvider mcp)
        {   // %hs
            return mcp.GetMacroDataSource().HeldSoul();
        }

        public static string Potion(IMacroContextProvider mcp)
        {   // %po
            return mcp.GetMacroDataSource().Potion();
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

        public static string HomeRegion(IMacroContextProvider mcp)
        {   // %hrn
            return mcp.GetMacroDataSource().HomeRegion();
        }

        public static string GodDesc(IMacroContextProvider mcp)
        {   // %gdd
            return mcp.GetMacroDataSource().GodDesc();
        }

        public static string God(IMacroContextProvider mcp)
        {   // %god
            return mcp.GetMacroDataSource().God();
        }

        public static string LocationDirection(IMacroContextProvider mcp)
        {   // %di
            return mcp.GetMacroDataSource().LocationDirection();
        }

        public static string DialogHint(IMacroContextProvider mcp)
        {   // %hnt
            return mcp.GetMacroDataSource().DialogHint();
        }

        public static string DialogHint2(IMacroContextProvider mcp)
        {   // %hnt2
            return mcp.GetMacroDataSource().DialogHint2();
        }

        public static string RoomHoursLeft(IMacroContextProvider mcp)
        {   // %dwr
            return mcp.GetMacroDataSource().RoomHoursLeft();
        }

        public static string PotentialQuestorName(IMacroContextProvider mcp)
        {
            // %pqn
            return mcp.GetMacroDataSource().PotentialQuestorName();
        }

        public static string PotentialQuestorLocation(IMacroContextProvider mcp)
        {
            // %pqp
            return mcp.GetMacroDataSource().PotentialQuestorLocation();
        }

        public static string DurationBase(IMacroContextProvider mcp)
        {
            // %bdr
            return mcp.GetMacroDataSource().DurationBase();
        }

        public static string DurationPlus(IMacroContextProvider mcp)
        {
            // %adr
            return mcp.GetMacroDataSource().DurationPlus();
        }

        public static string DurationPerLevel(IMacroContextProvider mcp)
        {
            // %cld
            return mcp.GetMacroDataSource().DurationPerLevel();
        }

        public static string ChanceBase(IMacroContextProvider mcp)
        {
            // %bch
            return mcp.GetMacroDataSource().ChanceBase();
        }

        public static string ChancePlus(IMacroContextProvider mcp)
        {
            // %ach
            return mcp.GetMacroDataSource().ChancePlus();
        }

        public static string ChancePerLevel(IMacroContextProvider mcp)
        {
            // %clc
            return mcp.GetMacroDataSource().ChancePerLevel();
        }

        public static string MagnitudeBaseMin(IMacroContextProvider mcp)
        {
            // %1bm
            return mcp.GetMacroDataSource().MagnitudeBaseMin();
        }

        public static string MagnitudeBaseMax(IMacroContextProvider mcp)
        {
            // %2bm
            return mcp.GetMacroDataSource().MagnitudeBaseMax();
        }

        public static string MagnitudePlusMin(IMacroContextProvider mcp)
        {
            // %1am
            return mcp.GetMacroDataSource().MagnitudePlusMin();
        }

        public static string MagnitudePlusMax(IMacroContextProvider mcp)
        {
            // %2am
            return mcp.GetMacroDataSource().MagnitudePlusMax();
        }

        public static string MagnitudePerLevel(IMacroContextProvider mcp)
        {
            // %clm
            return mcp.GetMacroDataSource().MagnitudePerLevel();
        }

        #endregion

        //
        // Multiline macro handlers - not sure if there are any others.
        //
        #region multiline macro handlers

        public static TextFile.Token[] MagicPowers(IMacroContextProvider mcp, TextFile.Formatting format)
        {   // %mpw
            return mcp.GetMacroDataSource().MagicPowers(format);
        }

        #endregion
    }

}
