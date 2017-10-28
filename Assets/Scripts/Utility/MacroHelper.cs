// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

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
            { "%1am", null }, // 1st + Magnitude
            { "%1bm", null }, // 1st base Magnitude
            { "%1com", GreetingOrFollowUpText },// Greeting (?)
            { "%1hn", null }, // ?
            { "%2am", null }, // 2nd + Magnitude
            { "%2bm", null }, // 2nd Base Magnitude
            { "%2com", DummyResolve2com },// ? (comment Nystul: it seems to be used in questions about work - it seems to be resolved to an empty string but not sure what else this macro does)
            { "%2hn", null }, // ?
            { "%3hn", null }, // ?
            { "%a", Amount },   // Cost of somthing.
            { "%ach", null }, // + Chance per
            { "%adr", null }, // + Duration per
            { "%agi", Agi }, //  Amount of Agility
            { "%arm", ItemName }, //  Armour
            { "%ark", AttributeRating }, // What property attribute is considered
            { "%ba", BookAuthor },  // Book Author
            { "%bch", null }, // Base chance
            { "%bdr", null }, // Base Duration
            { "%bn", null },  // ?
            { "%bt", ItemName },  // Book title
            { "%cbl", null }, // Cash balance in current region
            { "%clc", null }, // Per level (Chance)
            { "%cld", null }, // Per level (Duration)
            { "%clm", null }, // Per level (Magnitude)
            { "%cn", CityName },  // City name
            { "%cn2", CityName2 }, // City name #2
            { "%cpn", ShopName }, // Current shop name
            { "%cri", null }, // Accused crime
            { "%crn", CurrentRegion }, // Current Region
            { "%ct", CityType }, // City type? e.g city, town, village?
            { "%dae", null }, // A daedra
            { "%dam", DmgMod }, // Damage modifyer
            { "%dat", Date }, // Date
            { "%di", LocationDirection },  // Direction
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
            { "%gii", GoldCarried }, // Amount of gold in hand
            { "%god", God }, // Some god (listed in TEXT.RSC)
            { "%gtp", null }, // Amount of fine
            { "%hea", HpMod }, // HP Modifier
            { "%hmd", HealRateMod }, // Healing rate modifer
            { "%hnr", null }, // Honorific
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
            { "%jok", null }, // A joke
            { "%key", DialogKeySubject }, // A location (?) (comment Nystul: it is the topic you are asking about (e.g. building, work, etc.) how it seems)
            { "%key2", null },// Another location
            { "%kg", Weight },  //  Weight of items
            { "%kno", null }, // A knightly guild name
            { "%lev", null }, // Rank in guild that you are in.
            { "%ln", null },  //  Random lastname
            { "%loc", MarkLocationOnMap }, // Location marked on map (comment Nystul: this seems to be context dependent - it is used both in direction dialogs (7333) and map reveal dialogs (7332) - it seems to return the name of the building and reveal the map only if a 7332 dialog was chosen
            { "%lt1", null }, // Title of _fl1
            { "%ltn", LocalReputation }, // In the eyes of the law you are.......
            { "%luc", Luck }, // Luck
            { "%map", null }, // ?
            { "%mad", MagicResist }, // Resistance
            { "%mat", Material }, // Material
            { "%mit", null }, // Item
            { "%ml", MaxLoan },  // Max loan amount
            { "%mn", null },  // Random First(?) name (Male?)
            { "%mn2", null }, // Same as _mn (?)
            { "%mod", ArmourMod }, // Modification
            { "%n", NameDialogPartner },   // A random female first name (comment Nystul: I think it is just a random name - or maybe this is the reason that in vanilla all male mobile npcs have female names...)
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
            { "%po", Potion }, //  Potion
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
			{ "%reg", CurrentRegion }, // Region
            { "%rn", null },  // Regent's Name
            { "%rt", null },  // Regent's Title
            { "%spc", Magicka }, // Current Spell Points
            { "%ski", null }, // Skill
            { "%spd", Spd }, // Speed
            { "%spt", MagickaMax }, // Max spell points
            { "%str", Str }, // Amount of strength
            { "%sub", null }, // ?
            { "%t", Title },  // Regent's Title
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

        // Any punctuation characters that can be on the end of a macro symbol need adding here.
        static char[] PUNCTUATION = { '.', ',', '\'', '?' };

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
                                    if ((macroLen = macro.IndexOfAny(PUNCTUATION)) > 0)
                                    {
                                        string symbolStr = macro.Substring(0, macroLen);
                                        words[wordIdx] = prefix + GetValue(symbolStr, mcp) + macro.Substring(macroLen);
                                    }
                                    else
                                    {
                                        words[wordIdx] = prefix + GetValue(macro, mcp);
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
        {   // %cn2 (only used in msg #200)
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
                    return "city";
                case DFRegion.LocationTypes.TownVillage:
                    return "village";
                case DFRegion.LocationTypes.TownHamlet:
                    return "hamlet";
                case DFRegion.LocationTypes.HomeFarms:
                    return "farm";
                case DFRegion.LocationTypes.HomePoor:
                    return "shack";
                case DFRegion.LocationTypes.HomeWealthy:
                    return "manor";
                case DFRegion.LocationTypes.Tavern:
                    return "community";
                case DFRegion.LocationTypes.ReligionTemple:
                    return "shrine";
                default:
                    return gps.CurrentLocationType.ToString();
            }
        }

        private static string Title(IMacroContextProvider mcp)
        {   // %t
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            FactionFile.FactionData regionFaction;
            GameManager.Instance.PlayerEntity.FactionData.FindFactionByTypeAndRegion(7, gps.CurrentRegionIndex + 1, out regionFaction);

            switch (regionFaction.ruler)
            {
                case 1:
                    return "King";
                case 2:
                    return "Queen";
                case 3:
                    return "Duke";
                case 4:
                    return "Duchess";
                case 5:
                    return "Marquis";
                case 6:
                    return "Marquise";
                case 7:
                    return "Count";
                case 8:
                    return "Countess";
                case 9:
                    return "Baron";
                case 10:
                    return "Baroness";
                case 11:
                    return "Lord";
                case 12:
                    return "Lady";
                default:
                    return "Lord";
            }
        }

        private static string LocalReputation(IMacroContextProvider mcp)
        {   // %ltn
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            int rep = GameManager.Instance.PlayerEntity.RegionData[gps.CurrentRegionIndex].LegalRep;

            if (rep > 80)
                return "revered";
            else if (rep > 60)
                return "esteemed";
            else if (rep > 40)
                return "honored";
            else if (rep > 20)
                return "admired";
            else if (rep > 10)
                return "respected";
            else if (rep > 0)
                return "dependable";
            else if (rep == 0)
                return "a common citizen";
            else if (rep < -80)
                return "hated";
            else if (rep < -60)
                return "pond scum";
            else if (rep < -40)
                return "a villain";
            else if (rep < -20)
                return "a criminal";
            else if (rep < -10)
                return "a scoundrel";
            else if (rep < 0)
                return "undependable";

            return "unknown";
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

        private static string DmgMod(IMacroContextProvider mcp)
        {   // %dam
            return GameManager.Instance.PlayerEntity.DamageModifier.ToString("+0;-0;0");
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

        private static string GuildTitle(IMacroContextProvider mcp)
        {   // %pct
            // Just use "Apprentice" for all %pct guild titles for now
            // Guilds are not implemented yet, will need to move into a MacroDataSource
            return "Apprentice";
        }

        private static string GoldCarried(IMacroContextProvider mcp)
        {   // %gii
            return GameManager.Instance.PlayerEntity.GoldPieces.ToString();
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
                    return GameManager.Instance.TalkManager.CurrentKeySubject;
            }
        }

        //private static string DialogLocationDirection(IMacroContextProvider mcp)
        //{
        //    // %di
        //    return GameManager.Instance.TalkManager.GetKeySubjectLocationDirection();
        //}

        //private static string DialogHint(IMacroContextProvider mcp)
        //{
        //    // %hnt
        //    return GameManager.Instance.TalkManager.GetKeySubjectLocationHint();
        //}

        private static string MarkLocationOnMap(IMacroContextProvider mcp)
        {
            // %loc
            if (GameManager.Instance.TalkManager.MarkLocationOnMap)
                GameManager.Instance.TalkManager.MarkKeySubjectLocationOnMap();
            return GameManager.Instance.TalkManager.CurrentKeySubject;
        }

        #endregion

        //
        // Contextual macro handlers - delegate to the macro data source provided by macro context provider.
        //
        #region contextual macro handlers

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

        public static string God(IMacroContextProvider mcp)
        {   // %god
            return mcp.GetMacroDataSource().God();
        }

        public static string LocationDirection(IMacroContextProvider mcp)
        {
            // %di
            return mcp.GetMacroDataSource().LocationDirection();
        }

        public static string DialogHint(IMacroContextProvider mcp)
        {
            // %hnt
            return mcp.GetMacroDataSource().DialogHint();
        }

        public static string DialogHint2(IMacroContextProvider mcp)
        {
            // %hnt2
            return mcp.GetMacroDataSource().DialogHint2();
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