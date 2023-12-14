// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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

        readonly FileProxy factionFile = new FileProxy();
        readonly Dictionary<int, FactionData> factionDict = new Dictionary<int, FactionData>();
        readonly Dictionary<string, int> factionNameToIDDict = new Dictionary<string, int>();

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

        /// <summary>
        /// Faction IDs in Daggerfall's FACTION.TXT file.
        /// </summary>
        public enum FactionIDs
        {
            Clavicus_Vile = 1,
            Mehrunes_Dagon = 2,
            Molag_Bal = 3,
            Hircine = 4,
            Sanguine = 5,
            Peryite = 6,
            Malacath = 7,
            Hermaeus_Mora = 8,
            Sheogorath = 9,
            Boethiah = 10,
            Namira = 11,
            Meridia = 12,
            Vaernima = 13,
            Nocturnal = 14,
            Mephala = 15,
            Azura = 16,
            Oblivion = 17,

            Arkay = 21,
            Zen = 22,
            Mara = 24,
            Ebonarm = 25,
            Akatosh = 26,
            Julianos = 27,
            Dibella = 29,
            Stendarr = 33,
            Kynareth = 35,

            The_Temple_of_Kynareth = 36,
            The_Kynaran_Order = 37,

            The_Mages_Guild = 40,
            The_Fighters_Guild = 41,
            The_Thieves_Guild = 42,

            The_Academics = 60,
            The_Patricians = 61,
            The_Travelers_League = 62,
            The_Mercenary_Mages = 63,
            The_Isolationists = 64,
            The_Utility_Mages = 65,
            The_Cabal = 66,
            The_Order_of_the_Lamp = 67,
            The_Archmagister = 68,
            The_Guildmagister = 69,
            The_Master_of_Academia = 70,
            The_Master_of_Incunabula = 71,
            The_Master_at_Arms = 72,
            The_Palatinus = 73,
            The_Master_of_the_Scry = 74,
            The_Master_of_Initiates = 76,
            The_Guildmaster = 77, // A second "The Master of Initiates" in FACTION.TXT is also 77

            The_Order_of_Arkay = 82,
            The_Knights_of_the_Circle = 83,

            The_Resolution_of_Zen = 84,
            The_Knights_of_Iron = 85,

            The_Benevolence_of_Mara = 88,
            The_Maran_Knights = 89,

            The_Citadel_of_Ebonarm = 90,
            The_Battlelords = 91,

            The_Akatosh_Chantry = 92,
            The_Order_of_the_Hour = 93,

            The_Schools_of_Julianos = 94,
            The_Knights_Mentor = 95,

            The_House_of_Dibella = 98,
            The_Order_of_the_Lily = 99,

            The_Temple_of_Stendarr = 106,
            The_Crusaders = 107,

            The_Dark_Brotherhood = 108,

            The_Blades = 129,

            The_Vraseth = 150,
            The_Haarvenu = 151,
            The_Thrafey = 152,
            The_Lyrezi = 153,
            The_Montalion = 154,
            The_Khulari = 155,
            The_Garlythi = 156,
            The_Anthotis = 157,
            The_Selenu = 158,

            People_of_Glenumbra_Moors = 194,
            Court_of_Glenumbra_Moors = 195,
            Glenumbra_Moors = 196,

            People_of_Balfiera = 198,
            Isle_of_Balfiera = 199,

            Dwynnen = 200,
            Daggerfall = 201,
            Glenpoint = 202,
            Betony = 203,
            Sentinel = 204,
            Anticlere = 205,
            Lainlyn = 206,
            Wayrest = 207,
            Northmoor = 208,
            Menevia = 209,
            Alcaire = 210,
            Koegria = 211,
            Bhoriane = 212,
            Kambria = 213,
            Phrygia = 214,
            Urvaius = 215,
            Ykalon = 216,
            Daenia = 217,
            Shalgora = 218,
            Abibon_Gora = 219,
            Kairou = 220,
            Pothago = 221,
            Myrkwasa = 222,
            Ayasofya = 223,
            Tigonus = 224,
            Kozanset = 225,
            Satakalaam = 226,
            Totambu = 227,
            Mournoth = 228,
            Ephesus = 229,
            Santaki = 230,
            Antiphyllos = 231,
            Bergama = 232,
            Gavaudon = 233,
            Tulune = 234,
            Ilessan_Hills = 235,
            Cybiades = 236,

            Temple_Missionaries = 240,
            Teachers_of_Arkay = 241,

            Random_Noble = 242,

            Teachers_of_Zen = 243,

            Court_of_Balfiera = 244,

            Teachers_of_Mara = 245,

            Court_of_Lainlyn = 246,

            Teachers_of_Akatosh = 247,
            Teachers_of_Julianos = 249,
            Teachers_of_Dibella = 250,
            Teachers_of_Stendarr = 252,
            Teachers_of_Kynareth = 254,

            The_Oracle = 301,
            The_Acolyte = 302,

            Nulfaga = 303,
            Skakmat = 304,

            King_of_Worms = 305,
            The_Necromancers = 306,

            The_Septim_Empire = 350,
            The_Great_Knight = 351,
            Lady_Brisienna = 352,

            The_Underking = 353,
            Agents_of_The_Underking = 354,

            Lord_Harth = 355,

            The_Night_Mother = 356,

            Gortwog = 357,
            Orsinium = 358,

            Lord_Plessington = 359,
            Lord_Kilbar = 360,

            Chulmore_Quill = 361,
            The_Quill_Circus = 362,

            Medora = 363,

            King_Gothryd = 364,
            Queen_Aubki = 365,
            Mynisera = 366,
            Lord_Bridwell = 367,
            The_Knights_of_the_Dragon = 368,

            Popudax = 369,
            Lord_Coulder = 370,
            Mobar = 371,
            The_Royal_Guard = 372,

            The_Crow = 373,
            Thyr_Topfield = 374,
            Lord_Bertram_Spode = 375,
            Baltham_Greyman = 376,
            Lady_Bridwell = 377,
            Sylch_Greenwood = 378,
            Baron_Shrike = 379,

            Queen_Akorithi = 380,
            Prince_Greklith = 381,
            Prince_Lhotun = 382,
            Lord_Vhosek = 383,
            The_Royal_Guards = 384,
            Charvek_si = 385,
            Lord_Kavar = 386,
            Lord_Provlith = 387,
            Thaik = 388,
            Whitka = 389,

            King_Eadwyre = 390,
            Queen_Barenziah = 391,
            Princess_Elysana = 392,
            Prince_Helseth = 393,
            Princess_Morgiah = 394,
            Lord_Castellian = 395,
            Karethys = 396,
            Lord_Darkworth = 397,
            Lord_Woodborne = 398,

            The_Squid = 399,
            Lady_Doryanna_Flyte = 400,
            Lord_Auberon_Flyte = 401,
            Lord_Quistley = 402,
            Farrington = 403,
            Lord_Perwright = 404,
            Baroness_Dhemka = 405,
            Lord_Khane = 406,
            Britsa = 407,

            The_Order_of_the_Candle = 408,
            The_Knights_of_the_Rose = 409,
            The_Knights_of_the_Flame = 410,
            The_Host_of_the_Horn = 411,
            The_Host_of_the_True_Horn = 412,
            The_Knights_of_the_Owl = 413,
            The_Order_of_the_Raven = 414,
            The_Knights_of_the_Wheel = 415,
            The_Order_of_the_Scarab = 416,
            The_Knights_of_the_Hawk = 417,
            The_Order_of_the_Cup = 418,

            The_Glenmoril_Witches = 419,
            The_Dust_Witches = 420,
            The_Witches_of_Devilrock = 421,
            The_Tamarilyn_Witches = 422,
            The_Sisters_of_the_Bluff = 423,
            The_Daughters_of_Wroth = 424,
            The_Skeffington_Witches = 425,
            The_Witches_of_the_Marsh = 426,
            The_Mountain_Witches = 427,
            The_Daggerfall_Witches = 428,
            The_Beldama = 429,
            The_Sisters_of_Kykos = 430,
            The_Tide_Witches = 431,
            The_Witches_of_Alcaire = 432,

            Generic_Temple = 450,

            Apothecaries_of_Arkay = 453,
            Mixers_of_Arkay = 454,
            Binders_of_Arkay = 455,
            Summoners_of_Arkay = 456,

            Apothecaries_of_Zen = 462,
            Mixers_of_Zen = 463,
            Summonists_of_Zen = 464,

            Apothecaries_of_Mara = 468,
            Mixers_of_Mara = 469,
            Summoners_of_Mara = 470,

            Apothecaries_of_Akatosh = 473,
            Mixers_of_Akatosh = 474,
            Summoners_of_Akatosh = 475,

            Crafters_of_Julianos = 480,
            Smiths_of_Julianos = 481,
            Summoners_of_Julianos = 482,

            Apothecaries_of_Dibella = 485,
            Mixers_of_Dibella = 487,
            Summoners_of_Dibella = 488,

            Apothecaries_of_Stendarr = 490,
            Mixers_of_Stendarr = 491,
            Summoners_of_Stendarr = 492,

            Enchanters_of_Kynareth = 496,
            Spellsmiths_of_Kynareth = 497,
            Summoners_of_Kynareth = 498,

            Court_of_Wayrest = 499,

            Cyndassa = 500,
            Lady_Northbridge = 501,

            Wrothgaria = 502,
            Court_of_Wrothgaria = 503,
            People_of_Wrothgaria = 504,

            Dragontail = 505,
            Court_of_Dragontail = 506,
            People_of_Dragontail = 507,

            Alikra = 508,
            Court_of_Alikra = 509,

            The_Merchants = 510,
            The_Bards = 511,
            The_Prostitutes = 512,
            The_Fey = 513,
            Children = 514,
            Dancers = 515,

            Court_of_Dwynnen = 516,
            People_of_Dwynnen = 517,

            People_of_Daggerfall = 518,

            Court_of_Betony = 519,
            People_of_Betony = 520,

            Court_of_Glenpoint = 521,
            People_of_Glenpoint = 522,

            People_of_Sentinel = 523,
            People_of_Anticlere = 524,
            People_of_Lainlyn = 525,
            People_of_Wayrest = 526,

            Court_of_Northmoor = 527,
            People_of_Northmoor = 528,

            Court_of_Menevia = 529,
            People_of_Menevia = 530,

            Court_of_Alcaire = 531,
            People_of_Alcaire = 532,

            Court_of_Koegria = 533,
            People_of_Koegria = 534,

            Court_of_Bhoriane = 535,
            People_of_Bhoriane = 536,

            Court_of_Kambria = 537,
            People_of_Kambria = 538,

            Court_of_Phrygia = 539,
            People_of_Phrygia = 540,

            Court_of_Urvaius = 541,
            People_of_Urvaius = 542,

            Court_of_Ykalon = 543,
            People_of_Ykalon = 544,

            Court_of_Daenia = 545,
            People_of_Daenia = 546,

            Court_of_Shalgora = 547, // Name is duplicate "Court of Koegria" in FACTION.TXT
            People_of_Shalgora = 548, // Name is duplicate "People of Koegria" in FACTION.TXT

            Court_of_Abibon_Gora = 549,
            People_of_Abibon_Gora = 550,

            Court_of_Kairou = 551,
            People_of_Kairou = 552,

            Court_of_Pothago = 553,
            People_of_Pothago = 554,

            Court_of_Myrkwasa = 555,
            People_of_Myrkwasa = 556,

            Court_of_Ayasofya = 557,
            People_of_Ayasofya = 558,

            Court_of_Tigonus = 559,
            People_of_Tigonus = 560,

            Court_of_Kozanset = 561,
            People_of_Kozanset = 562,

            Court_of_Satakalaam = 563,
            People_of_Satakalaam = 564,

            Court_of_Totambu = 565,
            People_of_Totambu = 566,

            Court_of_Mournoth = 567,
            People_of_Mournoth = 568,

            Court_of_Ephesus = 569,
            People_of_Ephesus = 570,

            Court_of_Santaki = 571,
            People_of_Santaki = 572,

            Court_of_Antiphyllos = 573,
            People_of_Antiphyllos = 574,

            Court_of_Bergama = 575,
            People_of_Bergama = 576,

            Court_of_Gavaudon = 577,
            People_of_Gavaudon = 578,

            Court_of_Tulune = 579,
            People_of_Tulune = 580,

            Court_of_Ilessen_Hills = 581,
            People_of_Ilessen_Hills = 582,

            Court_of_Cybiades = 583,
            People_of_Cybiades = 584,

            People_of_Alikra = 590,

            Dakfron = 591,
            Court_of_Dakfron = 592,
            People_of_Dakfron = 593,

            Court_of_Daggerfall = 595,
            Court_of_Sentinel = 596,
            Court_of_Anticlere = 597,

            Court_of_Orsinium = 598,
            People_of_Orsinium = 599,

            The_Odylic_Mages = 801,
            The_Crafters = 802,

            The_Shadow_Trainers = 803,
            The_Shadow_Schemers = 804,
            The_Shadow_Appraisers = 805,
            The_Shadow_Spies = 806,

            Dark_Slayers = 807,

            Temple_Treasurers = 810,
            Temple_Blessers = 811,
            Temple_Healers = 813,

            Dark_Trainers = 839,
            Dark_Mixers = 840,
            Venom_Masters = 841,
            Dark_Plotters = 842,
            Dark_Binders = 843,

            Generic_Knightly_Order = 844,
            Smiths = 845,
            Questers = 846,
            Healers = 847,
            Seneschal = 848,

            Fighter_Trainers = 849,
            Fighter_Equippers = 850,
            Fighter_Questers = 851,

            Random_Ruler = 852,
            Random_Knight = 853,

            Secret_of_Oblivion = 977,
        }

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
        /// Faction race value does not map to usual race ID but maps
        /// to oath selection and is used only for this. Values above 7
        /// are not used in-game but are guessed from FACTION.TXT.
        /// </summary>
        public enum FactionRaces
        {
            None = -1,
            Nord = 0,
            Khajiit = 1,
            Redguard = 2,
            Breton = 3,
            Argonian = 4,
            WoodElf = 5,
            HighElf = 6,
            DarkElf = 7,
            Skakmat = 11,       // Only used on #304 "Skakmat"
            Orc = 17,           // Only used on #358 "Orsinium" in the original file
            Vampire = 18,
            Fey = 19,           // Only used on #513 "The Fey" a.k.a. Le Fay :)
        }

        /// <summary>
        /// Faction flags bitmask values.
        /// </summary>
        public enum Flags
        {
            RulerImmune = 0x10,
            Summoned = 0x40,
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
            public uint rulerNameSeed;
            public int rulerPowerBonus;
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
        /// <param name="filePath">Absolute path to FACTION.TXT file.</param>
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
            foreach (var srcFaction in factions)
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
        /// This method returns static faction data from FACTION.TXT file.
        /// Does not represent current state of faction simulation and is not localized.
        /// Use PlayerEntity.FactionData.GetFactionData() for faction data used during gameplay.
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
        /// Do not use for gameplay purposes.
        /// Use only for development and testing with unmodified FACTION.TXT.
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

        /// <summary>
        /// Check if a faction is another faction ally.
        /// </summary>
        /// <param name="firstFaction">The faction to check the allies of.</param>
        /// <param name="secondFaction">The potential allied faction.</param>
        /// <returns>True if factions are allied, otherwise false.</returns>
        public static bool IsAlly(ref FactionData firstFaction, ref FactionData secondFaction)
        {
            return firstFaction.ally1 == secondFaction.id || firstFaction.ally2 == secondFaction.id ||
                   firstFaction.ally3 == secondFaction.id;
        }

        /// <summary>
        /// Check if a faction is another faction enemy.
        /// </summary>
        /// <param name="firstFaction">The faction to check the enemies of.</param>
        /// <param name="secondFaction">The potential enemy faction.</param>
        /// <returns></returns>
        public static bool IsEnemy(ref FactionData firstFaction, ref FactionData secondFaction)
        {
            return firstFaction.enemy1 == secondFaction.id || firstFaction.enemy2 == secondFaction.id ||
                   firstFaction.enemy3 == secondFaction.id;
        }

        #endregion

        #region Private Methods

        void ParseFactions(string txt)
        {
            // Unmodded faction.txt contains multiples of same id
            // This resolver counter is used to give a faction a unique id if needed
            int resolverId = 980;

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
                    while (parentStack.Count > precedingTabs)
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

                // Calculate ruler name seed and ruler bonus in same manner as classic. These are not read from FACTION.TXT.
                uint random = DFRandom.rand() << 16;
                faction.rulerNameSeed = DFRandom.rand() | random;
                faction.rulerPowerBonus = DFRandom.random_range_inclusive(0, 50) + 20;

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

                        // Convert from 1-based to 0-based. This is also done in classic.
                        if (faction.region != -1)
                            faction.region--;
                        break;
                    case "type":
                        faction.type = ParseInt(value);
                        break;
                    case "power":
                        faction.power = ParseInt(value);
                        break;
                    case "flags":
                        faction.flags |= ParseInt(value);
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
