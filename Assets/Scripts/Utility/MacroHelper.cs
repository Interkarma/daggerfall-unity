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
using DaggerfallWorkshop.Game.Utility;

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
            { "%adj", PaintingAdjective }, // Painting adjective
            { "%agi", Agi }, //  Amount of Agility
            { "%an", ArtistName }, //  Artist name (of a painting)
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
            { "%dae", Daedra }, // A daedra
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
            { "%fn", FemaleName },  // Random first name (Female)
            { "%fn2", FemaleFullname }, // Random full name (Female)
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
            { "%hpn", HomeProvinceName }, // Home province name
            { "%hpw", GeographicalFeature }, // Geographical feature of home province
            { "%hrg", null }, // House region
            { "%hs", HeldSoul },  //  Holding Soul type
            { "%htwn", null },// House town
            { "%imp", MaleFullname }, // Emperor's son's name
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
            { "%mn", MaleName },  // Random First name (Male)
            { "%mn2", MaleFullname }, // Random Full name (Male)
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
            { "%pp1", PaintingPrefix1 }, // ?
            { "%pp2", PaintingPrefix2 }, // ?
            { "%pqn", PotentialQuestorName }, // Potential Quest Giver
            { "%pqp", PotentialQuestorLocation }, // Potential Quest Giver's Location
            { "%ptm", null }, // An enemy of the current region (?)
            { "%q1", Q1 },  // q1 to q12 Effects of questions answered in bio.
            { "%q2", Q2 },
            { "%q3", Q3 },
            { "%q4", Q4 },
            { "%q5", Q5 },
            { "%q6", Q6 },
            { "%q7", Q7 },
            { "%q8", Q8 },
            { "%q9", Q9 },
            { "%q10", Q10 },
            { "%q11", Q11 },
            { "%q12", Q12 },
            { "%q1a", Q1a },  // secondary effects of questions answered in bio
            { "%q2a", Q2a },
            { "%q3a", Q3a },
            { "%q4a", Q4a },
            { "%q5a", Q5a },
            { "%q6a", Q6a },
            { "%q7a", Q7a },
            { "%q8a", Q8a },
            { "%q9a", Q9a },
            { "%q10a", Q10a },
            { "%q11a", Q11a },
            { "%q12a", Q12a },
            { "%q1b", Q1b },  // tertiary effects of questions answered in bio
            { "%q2b", Q2b },
            { "%q3b", Q3b },
            { "%q4b", Q4b },
            { "%q5b", Q5b },
            { "%q6b", Q6b },
            { "%q7b", Q7b },
            { "%q8b", Q8b },
            { "%q9b", Q9b },
            { "%q10b", Q10b },
            { "%q11b", Q11b },
            { "%q12b", Q12b },
            { "%qdt", QuestDate }, // Quest date of log entry
            { "%qdat", null },// Quest date of log entry [2]
            { "%qot", null }, // The log comment
            { "%qua", Condition }, // Condition
            { "%r1", CommonersRep },  // Commoners rep
            { "%r2", MerchantsRep },  // Merchants rep
            { "%r3", ScholarsRep },  // Scholers rep
            { "%r4", NobilityRep },  // Nobilitys rep
            { "%r5", UnderworldRep },  // Underworld rep
            { "%ra", PlayerRace },  // Player's race
            { "%reg", RegionInContext }, // Region in context
            { "%rn", null },  // Regent's Name
            { "%rt", RegentTitle },  // Regent's Title
            { "%spc", Magicka }, // Current Spell Points
            { "%ski", Skill }, // Mastered skill name
            { "%spd", Spd }, // Speed
            { "%spt", MagickaMax }, // Max spell points
            { "%str", Str }, // Amount of strength
            { "%sub", PaintingSubject }, // Painting subject
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

        #region Public Utility Functions

        public static void ResetFactionAndRulerIds()
        {
            idFaction1InNews = -1;
            idFaction1Ruler = -1;
        }

        public static string GetFirstname(string name)
        {
            string[] parts = name.Split(' ');
            return (parts != null && parts.Length > 0) ? parts[0] : name;
        }

        public static NameHelper.BankTypes GetRandomNameBank()
        {
            // TODO: How should bank type be randomised? This line results in blank names sometimes, so using race instead.
            //return (NameHelper.BankTypes) DFRandom.random_range_inclusive(0, 8);
            Races race = (Races) DFRandom.random_range_inclusive(0, 8);
            return GetNameBank(race);
        }

        public static string GetLordNameForFaction(int factionId)
        {
            PersistentFactionData factions = GameManager.Instance.PlayerEntity.FactionData;
            FactionFile.FactionData fd;
            factions.GetFactionData(factionId, out fd);

            Genders gender = (Genders) ((fd.ruler + 1) % 2); // even entries are female titles/genders, odd entries are male ones
            Races race = (Races) fd.race;

            return DaggerfallUnity.Instance.NameHelper.FullName(GetNameBank(race), gender);
        }

        public static NameHelper.BankTypes GetNameBank(Races race)
        {
            switch (race)
            {
                case Races.Argonian:
                case Races.Breton:
                case Races.Khajiit:
                default:
                    return NameHelper.BankTypes.Breton;
                case Races.DarkElf:
                    return NameHelper.BankTypes.DarkElf;
                case Races.HighElf:
                    return NameHelper.BankTypes.HighElf;
                case Races.WoodElf:
                    return NameHelper.BankTypes.WoodElf;
                case Races.Nord:
                    return NameHelper.BankTypes.Nord;
                case Races.Redguard:
                    return NameHelper.BankTypes.Redguard;
            }
        }

        #endregion

        #region Macro Expansion Code

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
            // Initialise macro cache - used to ensure macros are only evaluated once per ExpandMacros() call.
            // Important since some macros evaluate differently each time. (e.g. macros with random generated names like %fx1 & %fx2)
            Dictionary<string, string> macroCache = new Dictionary<string, string>();

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

                                if (macro.StartsWith("%"))
                                {
                                    int macroLen;
                                    if (macroCache.ContainsKey(macro))
                                    {
                                        words[wordIdx] = prefix + macroCache[macro];
                                    }
                                    else if ((macroLen = macro.IndexOfAny(PUNCTUATION)) > 0)
                                    {
                                        string symbolStr = macro.Substring(0, macroLen);
                                        string expandedString = GetValue(symbolStr, mcp);
                                        words[wordIdx] = prefix + expandedString + macro.Substring(macroLen);
                                        macroCache[macro] = expandedString;
                                    }
                                    else
                                    {
                                        string expandedString = GetValue(macro, mcp);
                                        words[wordIdx] = prefix + expandedString;
                                        macroCache[macro] = expandedString;
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
                        string try1 = svp.Invoke(mcp);
                        if (try1 != null)
                            return try1;
                        return symbolStr + "[nullMCP]";
                    } catch (NotImplementedException) {
                        try {
                            string try2 = svp.Invoke(mcp2);
                            if (try2 != null)
                                return try2;
                        } catch (NotImplementedException) { }
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

        #endregion


        //
        // Global macro handlers - not context sensitive. (mcp will be null, and should not be used)
        //
        #region Global macro handlers

        public static string CityName(IMacroContextProvider mcp)
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

        public static string RegentTitle(IMacroContextProvider mcp)
        {   // %rt %t
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            FactionFile.FactionData regionFaction;
            GameManager.Instance.PlayerEntity.FactionData.FindFactionByTypeAndRegion(7, gps.CurrentRegionIndex, out regionFaction);

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
            return GetFirstname(GameManager.Instance.PlayerEntity.Name);
        }

        private static string PlayerPronoun(IMacroContextProvider mcp)
        {   // %pg
            return (GameManager.Instance.PlayerEntity.Gender == Genders.Female) ? HardStrings.pronounShe : HardStrings.pronounHe;
        }
        private static string PlayerPronoun1(IMacroContextProvider mcp)
        {   // %pg1
            return (GameManager.Instance.PlayerEntity.Gender == Genders.Female) ? HardStrings.pronounHer : HardStrings.pronounHis;
        }
        private static string PlayerPronoun2(IMacroContextProvider mcp)
        {   // %pg2
            return (GameManager.Instance.PlayerEntity.Gender == Genders.Female) ? HardStrings.pronounHer : HardStrings.pronounHim;
        }
        private static string PlayerPronoun2self(IMacroContextProvider mcp)
        {   // %pg2self
            return (GameManager.Instance.PlayerEntity.Gender == Genders.Female) ? HardStrings.pronounHerself : HardStrings.pronounHimself;
        }
        private static string PlayerPronoun3(IMacroContextProvider mcp)
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
            return GameManager.Instance.PlayerEntity.BirthRaceTemplate.Name;
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
            return GetLordNameForFaction((int)TalkManager.factionsUsedForRulers[id]);
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
            return GetLordNameForFaction((int)TalkManager.factionsUsedForRulers[id]);
        }

        public static string LordOfFaction2(IMacroContextProvider mcp)
        {   // %fl2
            // get random number between 0 and factionsUsedForRulers.Count - 2 now since we might add a + 1 for an id >= idFaction1Ruler later to prevent same faction as for %fl1
            int id = UnityEngine.Random.Range(0, TalkManager.factionsUsedForRulers.Count - 2);
            if (id >= idFaction1Ruler) // make sure to create an id != idFaction1Ruler
                id += 1; // by just adding 1 if id >= idFaction1InNews -> so we will end up with an id in ranges [0, idFaction1Ruler) union (idFaction1InNews, factionsUsedForFactionRulers.Count]
            return GetLordNameForFaction((int)TalkManager.factionsUsedForRulers[id]);
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
        {   // %loc
            if (GameManager.Instance.TalkManager.MarkLocationOnMap)
                GameManager.Instance.TalkManager.MarkKeySubjectLocationOnMap();
            return GameManager.Instance.TalkManager.CurrentKeySubject;
        }

        private static string LocationRevealedByMapItem(IMacroContextProvider mcp)
        {   // %map
            return GameManager.Instance.PlayerGPS.LocationRevealedByMapItem;
        }

        private static string LocationOfRegionalBuilding(IMacroContextProvider mcp)
        {   // %fcn
            return GameManager.Instance.TalkManager.LocationOfRegionalBuilding;
        }

        private static string FemaleName(IMacroContextProvider mcp)
        {   // %fn
            return DaggerfallUnity.Instance.NameHelper.FirstName(GetRandomNameBank(), Genders.Female);
        }
        private static string FemaleFullname(IMacroContextProvider mcp)
        {   // %fn2
            return DaggerfallUnity.Instance.NameHelper.FullName(GetRandomNameBank(), Genders.Female);
        }

        private static string MaleName(IMacroContextProvider mcp)
        {   // %mn
            return DaggerfallUnity.Instance.NameHelper.FirstName(GetRandomNameBank(), Genders.Male);
        }
        private static string MaleFullname(IMacroContextProvider mcp)
        {   // %mn2
            return DaggerfallUnity.Instance.NameHelper.FullName(GetRandomNameBank(), Genders.Male);
        }

        #endregion

        //
        // Contextual macro handlers - delegate to the macro data source provided by macro context provider.
        //
        #region Contextual macro handlers

        private static string GuildTitle(IMacroContextProvider mcp)
        {   // %lev %pct
            if (mcp == null)
                return GameManager.Instance.PlayerEntity.Name;
            return mcp.GetMacroDataSource().GuildTitle();
        }

        private static string FactionOrderName(IMacroContextProvider mcp)
        {   // %fon
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().FactionOrderName();
        }

        public static string Dungeon(IMacroContextProvider mcp)
        {   // %dng
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Dungeon();
        }

        private static string Amount(IMacroContextProvider mcp)
        {   // %a
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Amount();
        }
        private static string ShopName(IMacroContextProvider mcp)
        {   // %cpn
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().ShopName();
        }
        private static string MaxLoan(IMacroContextProvider mcp)
        {   // %ml
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().MaxLoan();
        }

        private static string Str(IMacroContextProvider mcp)
        {   // %str
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Str();
        }
        private static string Int(IMacroContextProvider mcp)
        {   // %int
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Int();
        }
        private static string Wil(IMacroContextProvider mcp)
        {   // %wil
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Wil();
        }
        private static string Agi(IMacroContextProvider mcp)
        {   // %agi
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Agi();
        }
        private static string End(IMacroContextProvider mcp)
        {   // %end
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().End();
        }
        private static string Per(IMacroContextProvider mcp)
        {   // %per
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Per();
        }
        private static string Spd(IMacroContextProvider mcp)
        {   // %spd
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Spd();
        }
        private static string Luck(IMacroContextProvider mcp)
        {   // %luc
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Luck();
        }

        private static string AttributeRating(IMacroContextProvider mcp)
        {   // %ark
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().AttributeRating();
        }

        public static string ItemName(IMacroContextProvider mcp)
        {   // %wep, %arm, %it
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().ItemName();
        }

        public static string Worth(IMacroContextProvider mcp)
        {   // %wth
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Worth();
        }

        public static string Material(IMacroContextProvider mcp)
        {   // %mat
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Material();
        }

        public static string Condition(IMacroContextProvider mcp)
        {   // %qua
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Condition();
        }

        public static string Weight(IMacroContextProvider mcp)
        {   // %kg
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Weight();
        }

        public static string WeaponDamage(IMacroContextProvider mcp)
        {   // %wdm
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().WeaponDamage();
        }

        public static string ArmourMod(IMacroContextProvider mcp)
        {   // %mod
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().ArmourMod();
        }

        public static string BookAuthor(IMacroContextProvider mcp)
        {   // %ba
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().BookAuthor();
        }

        public static string PaintingAdjective(IMacroContextProvider mcp)
        {   // %adj
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().PaintingAdjective();
        }
        public static string ArtistName(IMacroContextProvider mcp)
        {   // %an
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().ArtistName();
        }
        public static string PaintingPrefix1(IMacroContextProvider mcp)
        {   // %pp1
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().PaintingPrefix1();
        }
        public static string PaintingPrefix2(IMacroContextProvider mcp)
        {   // %pp2
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().PaintingPrefix2();
        }
        public static string PaintingSubject(IMacroContextProvider mcp)
        {   // %sub
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().PaintingSubject();
        }

        public static string HeldSoul(IMacroContextProvider mcp)
        {   // %hs
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().HeldSoul();
        }

        public static string Potion(IMacroContextProvider mcp)
        {   // %po
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Potion();
        }

        public static string Pronoun(IMacroContextProvider mcp)
        {   // %g & %g1
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Pronoun();
        }
        public static string Pronoun2(IMacroContextProvider mcp)
        {   // %g2
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Pronoun2();
        }
        public static string Pronoun2self(IMacroContextProvider mcp)
        {   // %g2self
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Pronoun2self();
        }
        public static string Pronoun3(IMacroContextProvider mcp)
        {   // %g3
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Pronoun3();
        }

        public static string QuestDate(IMacroContextProvider mcp)
        {   // %qdt
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().QuestDate();
        }

        public static string Oath(IMacroContextProvider mcp)
        {   // %oth
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Oath();
        }

        public static string HomeRegion(IMacroContextProvider mcp)
        {   // %hrn
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().HomeRegion();
        }

        public static string GodDesc(IMacroContextProvider mcp)
        {   // %gdd
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().GodDesc();
        }
        public static string God(IMacroContextProvider mcp)
        {   // %god
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().God();
        }

        public static string Daedra(IMacroContextProvider mcp)
        {   // %dae
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Daedra();
        }

        public static string LocationDirection(IMacroContextProvider mcp)
        {   // %di
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().LocationDirection();
        }

        public static string DialogHint(IMacroContextProvider mcp)
        {   // %hnt
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().DialogHint();
        }

        public static string DialogHint2(IMacroContextProvider mcp)
        {   // %hnt2
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().DialogHint2();
        }

        public static string RoomHoursLeft(IMacroContextProvider mcp)
        {   // %dwr
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().RoomHoursLeft();
        }

        public static string PotentialQuestorName(IMacroContextProvider mcp)
        {   // %pqn
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().PotentialQuestorName();
        }

        public static string PotentialQuestorLocation(IMacroContextProvider mcp)
        {   // %pqp
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().PotentialQuestorLocation();
        }

        public static string DurationBase(IMacroContextProvider mcp)
        {   // %bdr
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().DurationBase();
        }
        public static string DurationPlus(IMacroContextProvider mcp)
        {   // %adr
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().DurationPlus();
        }
        public static string DurationPerLevel(IMacroContextProvider mcp)
        {   // %cld
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().DurationPerLevel();
        }

        public static string ChanceBase(IMacroContextProvider mcp)
        {   // %bch
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().ChanceBase();
        }
        public static string ChancePlus(IMacroContextProvider mcp)
        {   // %ach
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().ChancePlus();
        }
        public static string ChancePerLevel(IMacroContextProvider mcp)
        {   // %clc
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().ChancePerLevel();
        }

        public static string MagnitudeBaseMin(IMacroContextProvider mcp)
        {   // %1bm
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().MagnitudeBaseMin();
        }
        public static string MagnitudeBaseMax(IMacroContextProvider mcp)
        {   // %2bm
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().MagnitudeBaseMax();
        }
        public static string MagnitudePlusMin(IMacroContextProvider mcp)
        {   // %1am
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().MagnitudePlusMin();
        }
        public static string MagnitudePlusMax(IMacroContextProvider mcp)
        {   // %2am
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().MagnitudePlusMax();
        }
        public static string MagnitudePerLevel(IMacroContextProvider mcp)
        {   // %clm
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().MagnitudePerLevel();
        }

        public static string CommonersRep(IMacroContextProvider mcp)
        {
            // %r1
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().CommonersRep();
        }

        public static string MerchantsRep(IMacroContextProvider mcp)
        {
            // %r2
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().MerchantsRep();
        }

        public static string ScholarsRep(IMacroContextProvider mcp)
        {
            // %r3
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().ScholarsRep();
        }

        public static string NobilityRep(IMacroContextProvider mcp)
        {
            // %r4
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().NobilityRep();
        }

        public static string UnderworldRep(IMacroContextProvider mcp)
        {
            // %r5
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().UnderworldRep();
        }

        private static string HomeProvinceName(IMacroContextProvider mcp)
        {
            // %hpn
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().HomeProvinceName();
        }

        private static string GeographicalFeature(IMacroContextProvider mcp)
        {
            // %hpw
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().GeographicalFeature();
        }

        public static string Q1(IMacroContextProvider mcp)
        {
            // %q1
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q1();
        }

        public static string Q2(IMacroContextProvider mcp)
        {
            // %q2
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q2();
        }

        public static string Q3(IMacroContextProvider mcp)
        {
            // %q3
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q3();
        }

        public static string Q4(IMacroContextProvider mcp)
        {
            // %q4
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q4();
        }

        public static string Q5(IMacroContextProvider mcp)
        {
            // %q5
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q5();
        }

        public static string Q6(IMacroContextProvider mcp)
        {
            // %q6
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q6();
        }

        public static string Q7(IMacroContextProvider mcp)
        {
            // %q7
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q7();
        }

        public static string Q8(IMacroContextProvider mcp)
        {
            // %q8
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q8();
        }

        public static string Q9(IMacroContextProvider mcp)
        {
            // %q9
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q9();
        }

        public static string Q10(IMacroContextProvider mcp)
        {
            // %q10
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q10();
        }

        public static string Q11(IMacroContextProvider mcp)
        {
            // %q11
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q11();
        }

        public static string Q12(IMacroContextProvider mcp)
        {
            // %q12
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q12();
        }

        public static string Q1a(IMacroContextProvider mcp)
        {
            // %q1a
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q1a();
        }

        public static string Q2a(IMacroContextProvider mcp)
        {
            // %q2a
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q2a();
        }

        public static string Q3a(IMacroContextProvider mcp)
        {
            // %q3a
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q3a();
        }

        public static string Q4a(IMacroContextProvider mcp)
        {
            // %q4a
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q4a();
        }

        public static string Q5a(IMacroContextProvider mcp)
        {
            // %q5a
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q5a();
        }

        public static string Q6a(IMacroContextProvider mcp)
        {
            // %q6a
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q6a();
        }

        public static string Q7a(IMacroContextProvider mcp)
        {
            // %q7a
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q7a();
        }

        public static string Q8a(IMacroContextProvider mcp)
        {
            // %q8a
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q8a();
        }

        public static string Q9a(IMacroContextProvider mcp)
        {
            // %q9a
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q9a();
        }

        public static string Q10a(IMacroContextProvider mcp)
        {
            // %q10a
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q10a();
        }

        public static string Q11a(IMacroContextProvider mcp)
        {
            // %q11a
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q11a();
        }

        public static string Q12a(IMacroContextProvider mcp)
        {
            // %q12a
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q12a();
        }

        public static string Q1b(IMacroContextProvider mcp)
        {
            // %q1b
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q1b();
        }

        public static string Q2b(IMacroContextProvider mcp)
        {
            // %q2b
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q2b();
        }

        public static string Q3b(IMacroContextProvider mcp)
        {
            // %q3b
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q3b();
        }

        public static string Q4b(IMacroContextProvider mcp)
        {
            // %q4b
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q4b();
        }

        public static string Q5b(IMacroContextProvider mcp)
        {
            // %q5b
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q5b();
        }

        public static string Q6b(IMacroContextProvider mcp)
        {
            // %q6b
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q6b();
        }

        public static string Q7b(IMacroContextProvider mcp)
        {
            // %q7b
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q7b();
        }

        public static string Q8b(IMacroContextProvider mcp)
        {
            // %q8b
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q8b();
        }

        public static string Q9b(IMacroContextProvider mcp)
        {
            // %q9b
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q9b();
        }

        public static string Q10b(IMacroContextProvider mcp)
        {
            // %q10b
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q10b();
        }

        public static string Q11b(IMacroContextProvider mcp)
        {
            // %q11b
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q11b();
        }

        public static string Q12b(IMacroContextProvider mcp)
        {
            // %q12b
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Q12b();
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
