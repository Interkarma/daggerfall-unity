// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using System.Text;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Helper class for context sensitive macros, like <c>"%abc"</c>, that are used in following Daggerfall files:
    /// arena2\text.rsc, fall.exe, arena2\*.qrc, or arena2\bio*.txt.
    /// See <see href="http://forums.dfworkshop.net/viewtopic.php?f=23%26t=673">this topic</see> for details about adding new macro handlers.
    /// </summary>
    public static class MacroHelper
    {
        public delegate string MacroHandler(IMacroContextProvider mcp = null);

        public delegate TextFile.Token[] MultilineMacroHandler(IMacroContextProvider mcp, TextFile.Formatting format);

        public static System.Random random = new System.Random();

        
        #region macro definitions and handler mappings

        static Dictionary<string, MacroHandler> macroHandlers = new Dictionary<string, MacroHandler>()
        {
            { "%1am", MagnitudePlusMin }, // 1st + Magnitude
            { "%1bm", MagnitudeBaseMin }, // 1st base Magnitude
            { "%1com", GreetingOrFollowUpText },// Greeting (?)
            { "%1hn", null }, // ?
            { "%2am", MagnitudePlusMax }, // 2nd + Magnitude
            { "%2bm", MagnitudeBaseMax }, // 2nd Base Magnitude
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
            { "%bn", Name }, // Random name in biography text
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
            { "%dam", DmgMod }, // Damage modifier
            { "%dat", Date }, // Date
            { "%di", Direction },  // Direction (depending on scope of last referenced place resource)
            { "%dip", DaysInPrison }, // Days in prison
            { "%dng", Dungeon }, // Dungeon
            { "%dts", null }, // Daedra
            { "%dwr", RoomHoursLeft }, // Hours with room remaining.
            { "%ef", null },  // Local shop name
            { "%enc", EncumbranceMax }, // Encumbrance
            { "%end", End }, // Amount of Endurance
            { "%fa", FactionAlly }, // faction which is both PC and NPC ally (used for greetings)
            { "%fae", FactionAllyEnemy }, // faction which is PC ally and NPC enemy (used for greetings)
            { "%fcn", LocationOfRegionalBuilding }, // Location with regional building asked about
            { "%fe", FactionEnemy }, // faction which is both PC and NPC enemy (used for greetings)
            { "%fea", FactionEnemyAlly }, // faction which is PC enemy and NPC ally (used for greetings)
            { "%fl1", LordOfFaction1 }, // Lord of _fx1
            { "%fl2", LordOfFaction2 }, // Lord of _fx2
            { "%fn", FemaleName },  // Random female name
            { "%fn2", FemaleName }, // Random female name, differs from the previous in classic by using a different seed
            { "%fnpc", FactionNPC }, // faction of npc that is dialog partner
            { "%fon", FactionOrderName }, // Faction order name
            { "%fpa", FactionName }, // faction name? of dialog partner - should return "Kynareth" for npc that are members of "Temple of Kynareth"
            { "%fpc", FactionPC }, // PC faction used for guild related greetings
            { "%fx1", AFactionInNews }, // A faction in news
            { "%fx2", AnotherFactionInNews }, // Another faction in news
            { "%g", Pronoun },   // He/She etc...
            { "%g1", Pronoun },  // He/She ???
            { "%g2", Pronoun2 },  // Him/Her etc...
            { "%g2self", Pronoun2self },// Himself/Herself etc...
            { "%g3", Pronoun3 },  // His/Her
            { "%g4", Pronoun4 },  // His/Hers
            { "%gii", GoldCarried }, // Amount of gold in hand
            { "%gdd", GodDesc }, // God description i.e. God of Logic
            { "%god", God }, // God of current region or current temple
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
            { "%imp", ImperialName }, // Emperor's son's name, using a specific name bank
            { "%int", Int }, // Amount of Intelligence
            { "%it", ItemName },  //  Item
            { "%jok", Joke }, // A joke
            { "%key", DialogKeySubject }, // A location (?) (comment Nystul: it is the topic you are asking about (e.g. building, work, etc.) how it seems)
            { "%key2", null },// Another location
            { "%kg", Weight },  //  Weight of items
            { "%kno", FactionOrderName }, // A knightly guild name
            { "%lev", GuildTitle }, // Rank in guild that you are in.
            { "%lp", LocalProvince },  //  Local province
            { "%ln", LastName },  //  Random lastname
            { "%loc", MarkLocationOnMap }, // Location marked on map (comment Nystul: this seems to be context dependent - it is used both in direction dialogs (7333) and map reveal dialogs (7332) - it seems to return the name of the building and reveal the map only if a 7332 dialog was chosen
            { "%lt1", TitleOfLordOfFaction1 }, // Title of _fl1
            { "%ltn", LegalReputation }, // In the eyes of the law you are.......
            { "%luc", Luck }, // Luck
            { "%mad", MagicResist }, // Resistance
            { "%map", LocationRevealedByMapItem }, // Name of location revealed by a map item
            { "%mat", Material }, // Material
            { "%mit", null }, // Item
            { "%ml", MaxLoan },  // Max loan amount
            { "%mn", MaleName },  // Random male name
            { "%mn2", MaleName }, // Random male name, differs from the previous in classic by using a different seed
            { "%mod", ArmourMod }, // Modification
            { "%n", Name },   // A random name (comment Nystul: I think it is just a random name - or maybe this is the reason that in vanilla all male mobile npcs have female names...)
            { "%nam", Name }, // A random full name
            { "%nrn", LordOfCurrentRegion }, // Noble of the current region (used in: O0B00Y01)
            { "%nt", NearbyTavern },  // Nearby Tavern
            { "%ol1", OldLordOfFaction1 }, // Old lord of _fx1
            { "%olf", OldLeaderFate }, // What happened to _ol1
            { "%on", null },  // ?
            { "%oth", Oath }, // An oath (listed in TEXT.RSC)
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
            { "%qdat", QuestDate },// Quest date of log entry [2]
            { "%qot", null }, // The log comment
            { "%qua", Condition }, // Condition
            { "%r1", CommonersRep },  // Commoners rep
            { "%r2", MerchantsRep },  // Merchants rep
            { "%r3", ScholarsRep },  // Scholers rep
            { "%r4", NobilityRep },  // Nobilitys rep
            { "%r5", UnderworldRep },  // Underworld rep
            { "%ra", PlayerRace },  // Player's race
            { "%reg", RegionInContext }, // Region in context
            { "%rn", RegentName },  // Regent's Name
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
            { "%vam", VampireClan }, // PC's vampire clan
            { "%vcn", VampireNpcClan }, // Vampire's Clan
            { "%vn", null },  // ?
            { "%wdm", WeaponDamage }, // Weapon damage
            { "%wep", ItemName }, // Weapon
            { "%wil", Wil }, // ?
            { "%wpn", null }, // Poison (?)
            { "%wth", Worth }, // Worth
        // DF Unity - new macros:
            { "%", Percent }, // Not really a macro, just print %
            { "%pg", PlayerPronoun },   // He/She (player)
            { "%pg1", PlayerPronoun },  // He/She (player)
            { "%pg2", PlayerPronoun2 }, // Him/Her (player)
            { "%pg2self", PlayerPronoun2self },// Himself/Herself (player)
            { "%pg3", PlayerPronoun3 },  // His/Her (player)
            { "%pg4", PlayerPronoun4 },  // His/Hers (player)
            { "%G", PronounCap }, // He/She, first letter capitalized
            { "%G1", PronounCap }, // He/She, first letter capitalized
            { "%G2", Pronoun2Cap }, // Him/Her, first letter capitalized
            { "%G2self", Pronoun2selfCap }, // Himself/Herself, first letter capitalized
            { "%G3", Pronoun3Cap }, // His/Her, first letter capitalized
            { "%G4", Pronoun4Cap }, // His/Hers, first letter capitalized
            { "%hrn", HomeRegion },  // Home region (of person)
            { "%pcl", PlayerLastname }, // Character's last name
            { "%day", DayNum }, // Current day of the month (ex: 1, 2, ..., 30)
            { "%dayn", DayName }, // Name of the current day (ex: Morndas)
            { "%days", DayWithSuffix }, // Current day of the month with suffix (ex: 1st, 2nd, 3rd, ...)
            { "%mon", MonthNum }, // Current month
            { "%monn", MonthName }, // Name of the current month (ex: Hearthfire)
            { "%year", YearNum }, // Current year
            { "%min", TimeMin }, // Current minute
            { "%hour", TimeHour }, // Current hour
            { "%sign", CurrentSign }, // Current sign (ex: The Lady, The Tower, ...). Not TES2 lore, but it's a staple at this point
            { "%sea", CurrentSeason }, // Current season
            { "%cbd", CurrentBuilding }, // Name of the current building, if any
        };

        // Multi-line macro handlers, returns tokens.
        static Dictionary<string, MultilineMacroHandler> multilineMacroHandlers = new Dictionary<string, MultilineMacroHandler>()
        {
            { "%mpw", MagicPowers }, // Magic powers - multi line (token) message
        };

        #endregion

        #region fields (some macros need state (e.g. %fn2 depends on %fn1)

        static int idFaction1 = -1;
        static int idFaction2 = -1;
        static int idRegion = -1;

        #endregion

        #region Public Utility Functions

        public static string LocationTypeName()
        {
            return CityType(null);
        }

        public static void SetFactionIdsAndRegionID(int faction1, int faction2, int region)
        {
            idFaction1 = faction1;
            idFaction2 = faction2;
            idRegion = region;
        }

        public static string GetFirstname(string name)
        {
            string[] parts = name.Split(' ');
            return (parts != null && parts.Length > 0) ? parts[0] : name;
        }

        public static string GetLastname(string name)
        {
            string[] parts = name.Split(' ');
            return (parts != null && parts.Length > 1) ? parts[1] : name;
        }

        public static NameHelper.BankTypes GetRandomNameBank()
        {
            DFRandom.Seed = (uint)random.Next();
            Races race = (Races)DFRandom.random_range_inclusive(1, 8);
            return GetNameBank(race);
        }

        public static string GetLordNameForFaction(int factionId, bool oldRuler = false)
        {
            PersistentFactionData factions = GameManager.Instance.PlayerEntity.FactionData;
            FactionFile.FactionData fd;
            factions.GetFactionData(factionId, out fd);

            // If the first faction child is an individual, she/he is the ruler: return her/his name
            if (fd.children != null && fd.children.Count > 0)
            {
                FactionFile.FactionData firstChild;
                factions.GetFactionData(fd.children[0], out firstChild);
                if (firstChild.type == (int)FactionFile.FactionTypes.Individual)
                    return firstChild.name;
            }

            Genders gender = (Genders)((fd.ruler + 1) % 2); // even entries are female titles/genders, odd entries are male ones
            Races race = RaceTemplate.GetRaceFromFactionRace((FactionFile.FactionRaces)fd.race);
            // Matched to classic: used to retain the same old and new ruler name for each region
            DFRandom.Seed = oldRuler ? fd.rulerNameSeed >> 16 : fd.rulerNameSeed & 0xffff;

            return DaggerfallUnity.Instance.NameHelper.FullName(GetNameBank(race), gender);
        }

        public static string GetRandomFullName()
        {
            // Get appropriate nameBankType for this region and a random gender
            NameHelper.BankTypes nameBankType = NameHelper.BankTypes.Breton;
            if (GameManager.Instance.PlayerGPS.CurrentRegionIndex > -1)
                nameBankType = (NameHelper.BankTypes)MapsFile.RegionRaces[GameManager.Instance.PlayerGPS.CurrentRegionIndex];
            Genders gender = (DFRandom.random_range_inclusive(0, 1) == 1) ? Genders.Female : Genders.Male;

            return DaggerfallUnity.Instance.NameHelper.FullName(nameBankType, gender);
        }

        public static NameHelper.BankTypes GetNameBank(Races race)
        {
            switch (race)
            {
                case Races.Breton:
                default:
                    return NameHelper.BankTypes.Breton;
                case Races.Redguard:
                    return NameHelper.BankTypes.Redguard;
                case Races.Nord:
                    return NameHelper.BankTypes.Nord;
                case Races.DarkElf:
                    return NameHelper.BankTypes.DarkElf;
                case Races.HighElf:
                    return NameHelper.BankTypes.HighElf;
                case Races.WoodElf:
                    return NameHelper.BankTypes.WoodElf;
                case Races.Khajiit:
                    return NameHelper.BankTypes.Khajiit;
                case Races.Argonian:
                    return NameHelper.BankTypes.Imperial;
            }
        }

        private static string GetRulerTitle(int factionRuler)
        {
            switch (factionRuler)
            {
                case 1:
                    return TextManager.Instance.GetLocalizedText("King");
                case 2:
                    return TextManager.Instance.GetLocalizedText("Queen");
                case 3:
                    return TextManager.Instance.GetLocalizedText("Duke");
                case 4:
                    return TextManager.Instance.GetLocalizedText("Duchess");
                case 5:
                    return TextManager.Instance.GetLocalizedText("Marquis");
                case 6:
                    return TextManager.Instance.GetLocalizedText("Marquise");
                case 7:
                    return TextManager.Instance.GetLocalizedText("Count");
                case 8:
                    return TextManager.Instance.GetLocalizedText("Countess");
                case 9:
                    return TextManager.Instance.GetLocalizedText("Baron");
                case 10:
                    return TextManager.Instance.GetLocalizedText("Baroness");
                case 11:
                    return TextManager.Instance.GetLocalizedText("Lord");
                case 12:
                    return TextManager.Instance.GetLocalizedText("Lady");
                default:
                    return TextManager.Instance.GetLocalizedText("Lord");
            }
        }

        private static string CapFirst(string tempString)
        {
            // Capitalizes first letter of a string, e.g. for capitalized pronoun macros
            return tempString.Substring(0, 1).ToUpper() + tempString.Substring(1);
        }

        #endregion

        #region Macro Expansion Code

        // Any non-alpha characters that can be on the end of a macro symbol need adding here.
        static readonly char[] MACRO_TERMINATORS = { ' ', '%', '.', ',', '\'', '?', '!', '/', '(', ')', '{', '}', '[', ']', '\"', ';', ':', '|' };

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
                // Check if the token contains any macro
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
                        StringBuilder builder = new StringBuilder();
                        int currentPos = 0;
                        int macroPos;

                        // Find if we have a macro left in the string
                        while ((macroPos = tokenText.IndexOf('%', currentPos)) >= 0)
                        {
                            // Find where the macro ends
                            int endPos = macroPos + 1;
                            while (endPos < tokenText.Length && (Array.IndexOf(MACRO_TERMINATORS, tokenText[endPos]) < 0))
                                endPos++;

                            // Evaluate macro
                            string macroName = tokenText.Substring(macroPos, endPos - macroPos);
                            if (!macroCache.TryGetValue(macroName, out string macroValue))
                            {
                                macroValue = GetValue(macroName, mcp);
                                macroCache[macroName] = macroValue;
                            }

                            // Add "prefix" and evaluated macro
                            builder.Append(tokenText, currentPos, macroPos - currentPos);
                            builder.Append(macroValue);

                            // Iterate from macro end
                            currentPos = endPos;

                            // "Eating" the | terminator (for postfixes purpose)
                            // Example: '%di|ern' => 'southern'
                            if (currentPos < tokenText.Length && tokenText[currentPos] == '|')
                                currentPos++;                        
                        }

                        // Add the rest of the text
                        builder.Append(tokenText, currentPos, tokenText.Length - currentPos);

                        tokens[tokenIdx].text = builder.ToString();
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
                return gps.CurrentLocalizedLocationName;
            else
                return gps.CurrentLocalizedRegionName;
        }

        private static string CityName2(IMacroContextProvider mcp)
        {   // %cn2 (only used in msg #200 where cn2 is a random? city)
            // Return first city of the region, unless it's current location, not worth doing more as the macro is hardly ever used.
            DFRegion dfRegion = GameManager.Instance.PlayerGPS.CurrentRegion;
            for (int i = 0; i < dfRegion.LocationCount; i++)
            {
                if (GameManager.Instance.PlayerGPS.CurrentLocationIndex != i && dfRegion.MapTable[i].LocationType == DFRegion.LocationTypes.TownCity)
                    return TextManager.Instance.GetLocalizedLocationName(dfRegion.MapTable[i].MapId, dfRegion.MapNames[i]);
            }
            return TextManager.Instance.GetLocalizedText("daggerfall"); // Localizaed fallback in case of error
        }

        private static string CurrentRegion(IMacroContextProvider mcp)
        {   // %crn
            return GameManager.Instance.PlayerGPS.CurrentLocalizedRegionName;
        }

        private static string CityType(IMacroContextProvider mcp)
        {   // %ct
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            switch (gps.CurrentLocationType)
            {
                case DFRegion.LocationTypes.TownCity:
                    return TextManager.Instance.GetLocalizedText("city");
                case DFRegion.LocationTypes.TownVillage:
                    return TextManager.Instance.GetLocalizedText("village");
                case DFRegion.LocationTypes.TownHamlet:
                    return TextManager.Instance.GetLocalizedText("hamlet");
                case DFRegion.LocationTypes.HomeFarms:
                    return TextManager.Instance.GetLocalizedText("farm");
                case DFRegion.LocationTypes.HomePoor:
                    return TextManager.Instance.GetLocalizedText("shack");
                case DFRegion.LocationTypes.HomeWealthy:
                    return TextManager.Instance.GetLocalizedText("manor");
                case DFRegion.LocationTypes.Tavern:
                    return TextManager.Instance.GetLocalizedText("community");
                case DFRegion.LocationTypes.ReligionTemple:
                    return TextManager.Instance.GetLocalizedText("temple");
                case DFRegion.LocationTypes.ReligionCult:
                    return TextManager.Instance.GetLocalizedText("shrine");
                default:
                    return gps.CurrentLocationType.ToString();
            }
        }

        private static string LocalProvince(IMacroContextProvider mcp)
        {   // %lp
            Races race = GameManager.Instance.PlayerGPS.GetRaceOfCurrentRegion();
            if (race == Races.Breton)
                return TextManager.Instance.GetLocalizedText("highRock");
            else
                return TextManager.Instance.GetLocalizedText("hammerfell");
        }

        private static string NearbyTavern(IMacroContextProvider mcp)
        {   // %nt - just gets a random tavern from current location and ignores how near it is.
            BuildingDirectory buildingDirectory = GameManager.Instance.StreamingWorld.GetCurrentBuildingDirectory();
            if (buildingDirectory && buildingDirectory.BuildingCount > 0)
            {
                List<BuildingSummary> taverns = buildingDirectory.GetBuildingsOfType(DFLocation.BuildingTypes.Tavern);
                int i = UnityEngine.Random.Range(0, taverns.Count);
                PlayerGPS.DiscoveredBuilding tavern;
                if (GameManager.Instance.PlayerGPS.GetAnyBuilding(taverns[i].buildingKey, out tavern))
                    return tavern.displayName;
            }
            return TextManager.Instance.GetLocalizedText("tavern");
        }

        public static string RegentTitle(IMacroContextProvider mcp)
        {   // %rt %t
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            FactionFile.FactionData regionFaction;
            GameManager.Instance.PlayerEntity.FactionData.FindFactionByTypeAndRegion((int)FactionFile.FactionTypes.Province, gps.CurrentRegionIndex, out regionFaction);
            return GetRulerTitle(regionFaction.ruler);
        }

        public static string RegentName(IMacroContextProvider mcp)
        {   // %rn
            // Look for a defined ruler for the region.
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            PersistentFactionData factionData = GameManager.Instance.PlayerEntity.FactionData;
            FactionFile.FactionData regionFaction;
            if (factionData.FindFactionByTypeAndRegion((int)FactionFile.FactionTypes.Province, gps.CurrentRegionIndex, out regionFaction))
            {
                FactionFile.FactionData child;
                foreach (int childID in regionFaction.children)
                    if (factionData.GetFactionData(childID, out child) && child.type == (int)FactionFile.FactionTypes.Individual)
                        return child.name;
            }
            // Use a random name if no defined individual ruler.
            return GetRandomFullName();
        }

        private static string Crime(IMacroContextProvider mcp)
        {   // %cri
            switch ((int)GameManager.Instance.PlayerEntity.CrimeCommitted)
            {
                case 1:
                    return TextManager.Instance.GetLocalizedText("Attempted_Breaking_And_Entering");
                case 2:
                    return TextManager.Instance.GetLocalizedText("Trespassing");
                case 3:
                    return TextManager.Instance.GetLocalizedText("Breaking_And_Entering");
                case 4:
                    return TextManager.Instance.GetLocalizedText("Assault");
                case 5:
                    return TextManager.Instance.GetLocalizedText("Murder");
                case 6:
                    return TextManager.Instance.GetLocalizedText("Tax_Evasion");
                case 7:
                    return TextManager.Instance.GetLocalizedText("Criminal_Conspiracy");
                case 8:
                    return TextManager.Instance.GetLocalizedText("Vagrancy");
                case 9:
                    return TextManager.Instance.GetLocalizedText("Smuggling");
                case 10:
                    return TextManager.Instance.GetLocalizedText("Piracy");
                case 11:
                    return TextManager.Instance.GetLocalizedText("High_Treason");
                case 12:
                    return TextManager.Instance.GetLocalizedText("Pickpocketing");
                case 13:
                    return TextManager.Instance.GetLocalizedText("Theft");
                case 14:
                    return TextManager.Instance.GetLocalizedText("Treason");
                case 15:
                    return TextManager.Instance.GetLocalizedText("Loan_Default");
                default:
                    return "None";
            }
        }

        private static string Penalty(IMacroContextProvider mcp)
        {   // %pen
            int punishmentType = DaggerfallUI.Instance.DfCourtWindow.PunishmentType;

            if (punishmentType == 2)
            {
                TextFile.Token[] tokens = { TextFile.CreateTextToken(TextManager.Instance.GetLocalizedText("Regular_Punishment_String")) };
                ExpandMacros(ref tokens);
                return tokens[0].text;
            }
            else if (punishmentType == 1)
                return TextManager.Instance.GetLocalizedText("Execution");
            else return TextManager.Instance.GetLocalizedText("Banishment");

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
                return TextManager.Instance.GetLocalizedText("revered");
            else if (rep > 60)
                return TextManager.Instance.GetLocalizedText("esteemed");
            else if (rep > 40)
                return TextManager.Instance.GetLocalizedText("honored");
            else if (rep > 20)
                return TextManager.Instance.GetLocalizedText("admired");
            else if (rep > 10)
                return TextManager.Instance.GetLocalizedText("respected");
            else if (rep > 0)
                return TextManager.Instance.GetLocalizedText("dependable");
            else if (rep == 0)
                return TextManager.Instance.GetLocalizedText("aCommonCitizen");
            else if (rep < -80)
                return TextManager.Instance.GetLocalizedText("hated");
            else if (rep < -60)
                return TextManager.Instance.GetLocalizedText("pondScum");
            else if (rep < -40)
                return TextManager.Instance.GetLocalizedText("aVillain");
            else if (rep < -20)
                return TextManager.Instance.GetLocalizedText("aCriminal");
            else if (rep < -10)
                return TextManager.Instance.GetLocalizedText("aScoundrel");
            else if (rep < 0)
                return TextManager.Instance.GetLocalizedText("undependable");

            return TextManager.Instance.GetLocalizedText("unknown");
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

        private static string PlayerLastname(IMacroContextProvider mcp)
        {   // %pcl
            return GetLastname(GameManager.Instance.PlayerEntity.Name);
        }

        private static string DayNum(IMacroContextProvider mcp)
        {   // %day
            return DaggerfallUnity.Instance.WorldTime.Now.DayOfMonth.ToString();
        }

        private static string DayName(IMacroContextProvider mcp)
        {   // %dayn
            return DaggerfallUnity.Instance.WorldTime.Now.DayName;
        }

        private static string DayWithSuffix(IMacroContextProvider mcp)
        {   // %days
            return DaggerfallUnity.Instance.WorldTime.Now.DayOfMonthWithSuffix.ToString();
        }

        private static string MonthNum(IMacroContextProvider mcp)
        {   // %mon
            return DaggerfallUnity.Instance.WorldTime.Now.MonthOfYear.ToString();
        }

        private static string MonthName(IMacroContextProvider mcp)
        {   // %monn
            return DaggerfallUnity.Instance.WorldTime.Now.MonthName;
        }

        private static string YearNum(IMacroContextProvider mcp)
        {   // %year
            return DaggerfallUnity.Instance.WorldTime.Now.Year.ToString();
        }

        private static string TimeMin(IMacroContextProvider mcp)
        {   // %min
            return DaggerfallUnity.Instance.WorldTime.Now.Minute.ToString();
        }

        private static string TimeHour(IMacroContextProvider mcp)
        {   // %hour
            return DaggerfallUnity.Instance.WorldTime.Now.Hour.ToString();
        }

        private static string CurrentSign(IMacroContextProvider mcp)
        {   // %sign
            return DaggerfallUnity.Instance.WorldTime.Now.BirthSignName;
        }

        private static string CurrentSeason(IMacroContextProvider mcp)
        {   // %sea
            return DaggerfallUnity.Instance.WorldTime.Now.SeasonName;
        }

        private static string Percent(IMacroContextProvider mcp)
        {   // %
            return "%";
        }
		
        private static string CurrentBuilding(IMacroContextProvider mcp)
        {   // %cbd
            if(!GameManager.Instance.IsPlayerInsideBuilding)
            {
                return "[invalid]";
            }

            PlayerEnterExit enterExit = GameManager.Instance.PlayerEnterExit;
            DaggerfallInterior buildingInterior = enterExit.Interior;
            DFLocation.BuildingData buildingData = buildingInterior.BuildingData;
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            DFLocation location = gps.CurrentLocation;
            return BuildingNames.GetName(
                buildingData.NameSeed,
                buildingData.BuildingType,
                buildingData.FactionId,
                TextManager.Instance.GetLocalizedLocationName(location.MapTableData.MapId, location.Name),
                TextManager.Instance.GetLocalizedRegionName(location.RegionIndex));
        }

        private static string PlayerPronoun(IMacroContextProvider mcp)
        {   // %pg
            return (GameManager.Instance.PlayerEntity.Gender == Genders.Female) ? TextManager.Instance.GetLocalizedText("pronounShe") : TextManager.Instance.GetLocalizedText("pronounHe");
        }
        private static string PlayerPronoun2(IMacroContextProvider mcp)
        {   // %pg2
            return (GameManager.Instance.PlayerEntity.Gender == Genders.Female) ? TextManager.Instance.GetLocalizedText("pronounHer") : TextManager.Instance.GetLocalizedText("pronounHim");
        }
        private static string PlayerPronoun2self(IMacroContextProvider mcp)
        {   // %pg2self
            return (GameManager.Instance.PlayerEntity.Gender == Genders.Female) ? TextManager.Instance.GetLocalizedText("pronounHerself") : TextManager.Instance.GetLocalizedText("pronounHimself");
        }
        private static string PlayerPronoun3(IMacroContextProvider mcp)
        {   // %pg3
            return (GameManager.Instance.PlayerEntity.Gender == Genders.Female) ? TextManager.Instance.GetLocalizedText("pronounHer2") : TextManager.Instance.GetLocalizedText("pronounHis");
        }
        private static string PlayerPronoun4(IMacroContextProvider mcp)
        {   // %pg4
            return (GameManager.Instance.PlayerEntity.Gender == Genders.Female) ? TextManager.Instance.GetLocalizedText("pronounHers") : TextManager.Instance.GetLocalizedText("pronounHis2");
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

        private static string FactionAlly(IMacroContextProvider mcp)
        {   // %fa
            return GameManager.Instance.TalkManager.GetFactionNPCAlly();
        }

        private static string FactionEnemy(IMacroContextProvider mcp)
        {   // %fe
            return GameManager.Instance.TalkManager.GetFactionNPCEnemy();
        }

        private static string FactionAllyEnemy(IMacroContextProvider mcp)
        {   // %fae
            return GameManager.Instance.TalkManager.GetFactionNPCEnemy();
        }

        private static string FactionEnemyAlly(IMacroContextProvider mcp)
        {   // %fea
            return GameManager.Instance.TalkManager.GetFactionNPCAlly();
        }

        private static string FactionPC(IMacroContextProvider mcp)
        {   // %fpc
            return GameManager.Instance.TalkManager.GetFactionPC();
        }

        private static string FactionNPC(IMacroContextProvider mcp)
        {   // %fnpc
            return GameManager.Instance.TalkManager.GetFactionNPC();
        }

        private static string FactionName(IMacroContextProvider mcp)
        {   // %fpa
            return GameManager.Instance.TalkManager.GetFactionName();
        }

        public static string AFactionInNews(IMacroContextProvider mcp)
        {   // %fx1
            PersistentFactionData factions = GameManager.Instance.PlayerEntity.FactionData;
            FactionFile.FactionData fd;
            factions.GetFactionData(idFaction1, out fd);
            return fd.name;
        }

        public static string AnotherFactionInNews(IMacroContextProvider mcp)
        {   // %fx2
            PersistentFactionData factions = GameManager.Instance.PlayerEntity.FactionData;
            FactionFile.FactionData fd;
            factions.GetFactionData(idFaction2, out fd);
            return fd.name;
        }

        public static string OldLeaderFate(IMacroContextProvider mcp)
        {   // %olf
            int index = UnityEngine.Random.Range(0, 5);
            return TalkManager.Instance.GetOldLeaderFateString(index);
        }

        public static string OldLordOfFaction1(IMacroContextProvider mcp)
        {   // %ol1
            return GetLordNameForFaction(idFaction1, true);
        }

        public static string LordOfFaction1(IMacroContextProvider mcp)
        {   // %fl1
            return GetLordNameForFaction(idFaction1);
        }

        public static string LordOfFaction2(IMacroContextProvider mcp)
        {   // %fl2
            return GetLordNameForFaction(idFaction2);
        }

        public static string LordOfCurrentRegion(IMacroContextProvider mcp)
        {   // %nrn
            return GetLordNameForFaction(GameManager.Instance.PlayerGPS.GetCurrentRegionFaction());
        }

        public static string TitleOfLordOfFaction1(IMacroContextProvider mcp)
        {   // %lt1
            PersistentFactionData factions = GameManager.Instance.PlayerEntity.FactionData;
            FactionFile.FactionData fd;
            factions.GetFactionData(idFaction1, out fd);

            return GetRulerTitle(fd.ruler);
        }

        public static string RegionInContext(IMacroContextProvider mcp)
        {   // %reg
            if (idRegion != -1)
            {
                return TextManager.Instance.GetLocalizedRegionName(idRegion);
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

        private static string LastName(IMacroContextProvider mcp)
        {   // %ln
            return DaggerfallUnity.Instance.NameHelper.Surname(GetRandomNameBank());
        }

        private static string FemaleName(IMacroContextProvider mcp)
        {   // %fn %fn2
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().FemaleName();
        }

        private static string MaleName(IMacroContextProvider mcp)
        {   // %mn %mn2
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().MaleName();
        }

        private static string VampireClan(IMacroContextProvider mcp)
        {   // %vam
            RacialOverrideEffect racialEffect = GameManager.Instance.PlayerEffectManager.GetRacialOverrideEffect();
            if (racialEffect is VampirismEffect)
                return (racialEffect as VampirismEffect).GetClanName();
            else
                return "%vam[ERROR: PC not a vampire]";
        }

        #endregion

        //
        // Contextual macro handlers - delegate to the macro data source provided by macro context provider.
        //
        #region Contextual macro handlers

        private static string Name(IMacroContextProvider mcp)
        {   // %n %nam %bn
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Name();
        }

        private static string VampireNpcClan(IMacroContextProvider mcp)
        {   // %vcn
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().VampireNpcClan();
        }

        private static string GuildTitle(IMacroContextProvider mcp)
        {   // %lev %pct
            if (mcp == null)
                return GameManager.Instance.PlayerEntity.Name;
            return mcp.GetMacroDataSource().GuildTitle();
        }

        private static string FactionOrderName(IMacroContextProvider mcp)
        {   // %fon %kno
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
        public static string Pronoun4(IMacroContextProvider mcp)
        {   // %g4
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().Pronoun4();
        }
        public static string PronounCap(IMacroContextProvider mcp)
        {   // %G & %G1
            if (mcp == null) return null;
            return CapFirst(mcp.GetMacroDataSource().Pronoun());
        }
        public static string Pronoun2Cap(IMacroContextProvider mcp)
        {   // %G2
            if (mcp == null) return null;
            return CapFirst(mcp.GetMacroDataSource().Pronoun2());
        }
        public static string Pronoun2selfCap(IMacroContextProvider mcp)
        {   // %G2self
            if (mcp == null) return null;
            return CapFirst(mcp.GetMacroDataSource().Pronoun2self());
        }
        public static string Pronoun3Cap(IMacroContextProvider mcp)
        {   // %G3
            if (mcp == null) return null;
            return CapFirst(mcp.GetMacroDataSource().Pronoun3());
        }
        public static string Pronoun4Cap(IMacroContextProvider mcp)
        {   // %G4
            if (mcp == null) return null;
            return CapFirst(mcp.GetMacroDataSource().Pronoun4());
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

        public static string Direction(IMacroContextProvider mcp)
        {   // %di
            if (mcp == null) return null;

            return mcp.GetMacroDataSource().Direction();
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

        public static string ImperialName(IMacroContextProvider mcp)
        {
            // %imp
            if (mcp == null) return null;
            return mcp.GetMacroDataSource().ImperialName();
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
