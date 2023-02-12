// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor.BuildingPresets
{
    public class BuildingPresetData
    {
        public static Dictionary<string, string[]> buildingGroups = new Dictionary<string, string[]>() {
            { "house1A", new string[] {"013", "014", "0117", "0118", "0119", "0121"}},
            { "house1C", new string[] {"015","0110", "0114"}},
            { "house1D", new string[] {"011", "017", "018"}},
            { "house1E", new string[] {"012", "016", "0111", "0112", "0115", "0120"}},
            { "house1F", new string[] {"0113", "0116"}},
            { "house1Other", new string[] {"019"}},

            { "house2A", new string[] {"025", "027", "0210", "0212", "0213", "0217", "0218", "0222", "0223", "0224", "0226", "0231", "0233", "0235", "0237", "0238", "0239", "0244", "0245", "0251", "0255", "0258", "0259", "0265", "0270", "0274", "0276", "0280", "0281", "0282", "0284", "0285", "0287", "0289", "02101", "02107", "02112", "02116", "02120", "02127", "02128", "02129", "02131", "02135", "02137", "02138", "02139", "02140", "02143", "02146", "02147", "02149", "02150", "02152", "02153", "02156", "02161", "02162", "02163", "02166", "02167", "02171", "02174"}},
            { "house2B", new string[] {"0211", "0225", "0243", "0262", "0267", "0271", "0283", "0293", "02102", "02114", "02133", "02165", "02173"}},
            { "house2C", new string[] {"028", "029", "0215", "0216", "0229", "0230", "0236", "0240", "0246", "0247", "0254", "0256", "0257", "0264", "0268", "0277", "0286", "0288", "0298", "02103", "02108", "02109", "02110", "02124", "02141", "02144", "02148", "02151", "02154", "02155", "02170", "02172"}},
            { "house2D", new string[] {"021", "022", "0241", "0242", "0263", "0266", "0272", "0290", "0291", "0296", "0297", "02104", "02111", "02122", "02123", "02126", "02132", "02136", "02157", "02168"}},
            { "house2E", new string[] {"0219", "0220", "0221", "0248", "0253", "0260", "0295", "0299", "02113", "02115", "02118", "02121", "02125", "02134", "02158"}},
            { "house2F", new string[] {"023", "024", "026", "0214", "0227", "0228", "0232", "0234", "0249", "0250", "0252", "0261", "0269", "0275", "0279", "0292", "0294", "02100", "02105", "02119", "02130", "02145", "02159", "02164"}},
            { "house2Other", new string[] {"0273", "0278", "02106", "02117", "02142", "02160", "02169"}},

            { "house3A", new string[] {"031", "032", "033", "035", "037", "038", "039", "0311", "0312", "0313", "0316", "0317", "0318", "0319"}},
            { "house3C", new string[] {"034", "0310"}},
            { "house3D", new string[] {"036", "0314"}},
            { "house3F", new string[] {"0315"}},

            { "house4A", new string[] {"041", "047", "049", "0412", "0413", "0414", "0415", "0416", "0417", "0418", "0420", "0421", "0423", "0426", "0427", "0428", "0429", "0430", "0431", "0432", "0433", "0434", "0435"}},
            { "house4B", new string[] {"0437"}},
            { "house4C", new string[] {"042", "043", "045", "046", "0410", "0411", "0419", "0424", "0425"}},
            { "house4D", new string[] {"044", "0422"}},
            { "house4E", new string[] {"048"}},

            { "house5", new string[] { "051", "052", "053", "054", "055", "056", "057", "058", "059", "0510", "0511", "0512", "0513", "0514", "0515", "0516", "0517", "0518", "0519", "0520", "0521", "0522" } },
            { "house6", new string[] { "061", "062", "063", "064", "065", "066", "067", "068", "069", "0610", "0611", "0612", "0613", "0614", "0615", "0616", "0617", "0618", "0619", "0620", "0621", "0622", "0623", "0624", "0625", "0626", "0627", "0628", "0629", "0630", "0631", "0632", "0633", "0634", "0635", "0636", "0637", "0638", "0639", "0640", "0641", "0642", "0643", "0644", "0645", "0646", "0647", "0648", "0649", "0650", "0651", "0652", "0653", "0654", "0655", "0656", "0657", "0658" } },

            { "houseForSaleA", new string[] {"072", "076", "0712", "0713", "0714", "0717"}},
            { "houseForSaleC", new string[] {"071", "075", "078", "0711"}},
            { "houseForSaleD", new string[] {"073", "074", "079", "0710", "0715", "0716"}},
            { "houseForSaleE", new string[] {"077", "0718", "0720"}},
            { "houseForSaleF", new string[] {"0719", "0721", "0722", "0723"}},

            { "tavern", new string[] { "081", "082", "083", "084", "085", "086", "087", "088", "089", "0810", "0811", "0812", "0813", "0814", "0815", "0816", "0817", "0818" } },
            { "guildHall", new string[] { "091", "092", "093", "094", "095", "096", "097", "098", "099", "0910", "0911", "0912", "0913", "0914", "0915", "0916", "0917", "0918", "0919", "0920" } },
            { "temple", new string[] { "101", "102", "103", "104", "105", "106", "107", "108", "109", "1010", "1011" } },
            { "furnitureStore", new string[] { "111", "112" } },
            { "bank", new string[] { "121", "122", "123" } },
            { "generalStore", new string[] { "131", "132", "133", "134", "135", "136", "137", "138", "139", "1310", "1311" } },
            { "pawnShop", new string[] { "141", "142" } },
            { "armorer", new string[] { "151", "152", "153", "154", "155" } },
            { "weaponSmith", new string[] { "161", "162", "163", "164", "165", "166", "167" } },
            { "clothingStore", new string[] { "171", "172", "173", "174", "175", "176", "177", "178" } },
            { "alchemist", new string[] { "181", "182", "183", "184", "185", "186", "187", "188", "189", "1810" } },
            { "gemStore", new string[] { "191", "192", "193" } },
            { "bookseller", new string[] { "201", "202", "203", "204", "205", "206" } },
            { "library", new string[] { "211", "212", "213" } },
            { "palace", new string[] { "221", "222", "223", "224", "225", "226", "227", "228", "229", "2210", "2211", "2212", "2213", "2214", "2215", "2216", "2217" } },
            { "town23", new string[] { "231", "232", "233", "234" } },
            { "ship", new string[] { "241", "242" } },
        };

        public static Dictionary<string, string> houses = new Dictionary<string, string>() {
            {"013", "House1 type A [6]"},
            {"015", "House1 type C [3]"},
            {"011", "House1 type D [3]"},
            {"012", "House1 type E [6]"},
            {"0113", "House1 type F [2]"},
            {"019", "House1 Other [1]"},
            {"025", "House2 type A [63]"},
            {"0211", "House2 type B [13]"},
            {"028", "House2 type C [32]"},
            {"021", "House2 type D [20]"},
            {"0219", "House2 type E [15]"},
            {"023", "House2 type F [24]"},
            {"0273", "House2 Other [7]"},
            {"031", "House3 type A [14]"},
            {"034", "House3 type C [2]"},
            {"036", "House3 type D [2]"},
            {"0315", "House3 type F [1]"},
            {"041", "House4 type A [23]"},
            {"0437", "House4 type B [1]"},
            {"042", "House4 type C [9]"},
            {"044", "House4 type D [2]"},
            {"048", "House4 type E [1]"},
            {"051", "House5 [22]"},
            {"061", "House6 [58]"},
            {"072", "House For Sale type A [6]"},
            {"071", "House For Sale type C [4]"},
            {"073", "House For Sale type D [6]"},
            {"077", "House For Sale type E [3]"},
            {"0719", "House For Sale type F [4]"}
        };
        public static Dictionary<string, string> shops = new Dictionary<string, string>() {
            {"131", "General Store [11]"},
            {"141", "Pawn Shop [2]"},
            {"151", "Armorer [5]"},
            {"161", "Weapon Smith [7]"},
            {"171", "Clothing Store [8]"},
            {"181", "Alchemist [10]"},
            {"191", "Gem Store [3]"},
            {"201", "Bookseller [6]"},
        };
        public static Dictionary<string, string> services = new Dictionary<string, string>() {
            {"081", "Tavern [18]"},
            {"121", "Bank [3]"},
            {"211", "Library [3]"}
        };
        public static Dictionary<string, string> guilds = new Dictionary<string, string>() {
            {"091", "Guild Hall [20]"},
            {"101", "Temple [11]"},
            {"111", "Furniture Store (Thieves Guild) [2]"},
        };
        public static Dictionary<string, string> others = new Dictionary<string, string>() {
            {"221", "Palace [17]"},
            {"231", "City Walls [4]"},
            {"241", "Ship [2]"},
        };
    }
}