// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Generates building names in city locations.
    /// </summary>
    public static class BuildingNames
    {
        public static string GetName(int seed, DFLocation.BuildingTypes type, int factionID, string locationName, string regionName)
        {
            const string firstNameTitleVar = "%ef";
            const string cityNameTitleVar = "%cn";
            const string royalTitleVar = "%rt";

            string a = string.Empty, b = string.Empty;
            string result = string.Empty;

            bool singleton = false;
            FactionFile.FactionData factionData;
            DFRandom.srand(seed);
            switch (type)
            {
                case DFLocation.BuildingTypes.HouseForSale:
                    return "House for sale";

                case DFLocation.BuildingTypes.Tavern:
                    b = TavernsB[DFRandom.random_range(0, TavernsB.Length)];
                    a = TavernsA[DFRandom.random_range(0, TavernsA.Length)];
                    break;

                case DFLocation.BuildingTypes.GeneralStore:
                    b = GeneralStoresB[DFRandom.random_range(0, GeneralStoresB.Length)];
                    a = StoresA[DFRandom.random_range(0, StoresA.Length)];
                    break;

                case DFLocation.BuildingTypes.WeaponSmith:
                    b = WeaponStoresB[DFRandom.random_range(0, WeaponStoresB.Length)];
                    a = StoresA[DFRandom.random_range(0, StoresA.Length)];
                    break;

                case DFLocation.BuildingTypes.Armorer:
                    b = ArmorStoresB[DFRandom.random_range(0, ArmorStoresB.Length)];
                    a = StoresA[DFRandom.random_range(0, StoresA.Length)];
                    break;

                case DFLocation.BuildingTypes.Bookseller:
                    b = BookStoresB[DFRandom.random_range(0, BookStoresB.Length)];
                    a = StoresA[DFRandom.random_range(0, StoresA.Length)];
                    break;

                case DFLocation.BuildingTypes.ClothingStore:
                    b = ClothingStoresB[DFRandom.random_range(0, ClothingStoresB.Length)];
                    a = StoresA[DFRandom.random_range(0, StoresA.Length)];
                    break;

                case DFLocation.BuildingTypes.Alchemist:
                    b = AlchemyStoresB[DFRandom.random_range(0, AlchemyStoresB.Length)];
                    a = StoresA[DFRandom.random_range(0, StoresA.Length)];
                    break;

                case DFLocation.BuildingTypes.GemStore:
                    b = GemStoresB[DFRandom.random_range(0, GemStoresB.Length)];
                    a = StoresA[DFRandom.random_range(0, StoresA.Length)];
                    break;

                case DFLocation.BuildingTypes.PawnShop:
                    b = PawnStoresB[DFRandom.random_range(0, PawnStoresB.Length)];
                    a = StoresA[DFRandom.random_range(0, StoresA.Length)];
                    break;

                case DFLocation.BuildingTypes.FurnitureStore:
                    b = FurnitureStoresB[DFRandom.random_range(0, FurnitureStoresB.Length)];
                    a = StoresA[DFRandom.random_range(0, StoresA.Length)];
                    break;

                case DFLocation.BuildingTypes.Library:
                    b = LibraryStoresB[DFRandom.random_range(0, LibraryStoresB.Length)];
                    a = StoresA[DFRandom.random_range(0, StoresA.Length)];
                    break;

                case DFLocation.BuildingTypes.Bank:
                    // Banks always appear to be named "The Bank of RegionName"
                    b = regionName;
                    a = "The Bank of";
                    break;

                case DFLocation.BuildingTypes.GuildHall:
                    // Guild halls get the name from faction data
                    if (DaggerfallUnity.Instance.ContentReader.FactionFileReader.GetFactionData(factionID, out factionData))
                    {
                        a = factionData.name;
                        singleton = true;
                    }
                    break;

                case DFLocation.BuildingTypes.Temple:
                    // Temples get name from faction data - always seem to be first child of factionID
                    if (DaggerfallUnity.Instance.ContentReader.FactionFileReader.GetFactionData(factionID, out factionData))
                    {
                        if (factionData.children.Count > 0)
                        {
                            FactionFile.FactionData firstChild;
                            if (DaggerfallUnity.Instance.ContentReader.FactionFileReader.GetFactionData(factionData.children[0], out firstChild))
                            {
                                a = firstChild.name;
                                singleton = true;
                            }
                        }
                    }
                    break;

                case DFLocation.BuildingTypes.Palace:
                    // Main palace names come from TEXT.RSC (e.g. "Castle Daggerfall")
                    // Other palaces are just named "Palace" (still need to confirm behaviour)
                    int textId = 0;
                    if (locationName == "Daggerfall")
                        textId = 475;
                    else if (locationName == "Wayrest")
                        textId = 476;
                    else if (locationName == "Sentinel")
                        textId = 477;

                    if (textId > 0)
                    {
                        TextFile.Token[] nameTokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(textId);
                        foreach (TextFile.Token token in nameTokens)
                        {
                            if (token.formatting == TextFile.Formatting.Text)
                            {
                                a = token.text;
                                break;
                            }
                        }
                        a = a.TrimEnd('.'); // remove character '.' from castle text record entry if it is last character
                    }
                    else
                    {
                        a = "Palace";
                    }
                    singleton = true;
                    break;

                default:
                    // Do nothing for unknown/unsupported building type
                    // Houses can actually change names based on active quests
                    return string.Empty;
            }

            // Replace %cn
            a = a.Replace(cityNameTitleVar, locationName);

            // Replace %ef
            if (a.Contains(firstNameTitleVar))
            {
                // Need to burn a rand() for %ef roll to be correct
                // What is Daggerfall rolling here?
                DFRandom.rand();

                // Observation finds nameplates only seem to use male Breton namebank
                string firstName = DaggerfallUnity.Instance.NameHelper.FirstName(NameHelper.BankTypes.Breton, Game.Entity.Genders.Male);
                a = a.Replace(firstNameTitleVar, firstName);
            }

            // Replace %rt based on faction ruler
            if (a.Contains(royalTitleVar))
            {
                a = a.Replace(royalTitleVar, MacroHelper.RegentTitle(null));
            }

            // Final text is "{a} {b}" for two-part names or just "{a}" for singleton names
            if (!singleton)
                result = string.Format("{0} {1}", a, b);
            else
                result = a;

            return result;
        }

        #region Stores

        static string[] StoresA = new string[]
        {
            "%ef's",
            "%cn's Best",
            "The Essential",
            "Lord %ef's",
            "The Adventurer's",
            "The Odd",
            "%ef's Finest",
            "Bargain",
            "Vintage",
            "The Emperor's",
            "%cn",
            "%ef's General",
            "The Superior",
            "%ef's Quality",
            "First Class",
            "The %rt's",
            "The Champion",
            "Doctor %ef's",
            "Lady %ef's",
        };

        static string[] GeneralStoresB = new string[]
        {
            "Supplies",
            "Supply Store",
            "Gear",
            "Gear Store",
            "Equipment",
            "Equipment Store",
            "Sundries",
            "Provisions",
            "Merchandise",
            "General Store",
            "Retail Store",
            "Trading Post",
            "Market",
            "Wares",
            "Warehouse",
        };

        static string[] WeaponStoresB = new string[]
        {
            "Weapons",
            "Weaponry",
            "Arms",
            "Armaments",
            "Arsenal",
            "Armsmaker",
            "Blades",
            "Blacksmith",
            "Metalsmith",
            "Weaponsmith",
        };

        static string[] ArmorStoresB = new string[]
        {
            "Armory",
            "Mail",
            "Shielding",
            "Armor",
            "Shields",
            "Aegis",
            "Metalworks",
            "Blacksmith",
            "Metalsmith",
            "Armorer",
            "Smith",
            "Smithy",
        };

        static string[] BookStoresB = new string[]
        {
            "Books",
            "Bookstore",
            "Bookshop",
            "Book Dealer",
            "Book Center",
            "Bookseller",
            "Bookstall",
            "Incunabula",
        };

        static string[] LibraryStoresB = new string[]
        {
            "Library",
            "Bookroom",
            "Athenaeum",
            "Public Library",
            "Historians",
            "Bookroom",
            "Seminary",
            "Lyceum",
        };

        static string[] ClothingStoresB = new string[]
        {
            "Clothing",
            "Clothes",
            "Garments",
            "Apparel",
            "Costumes",
            "Vestments",
            "Attire",
            "Fashion",
            "Tailoring",
            "Outfits",
            "Finery",
        };

        static string[] AlchemyStoresB = new string[]
        {
            "Herbs",
            "Potherbs",
            "Spices",
            "Remedies",
            "Antidotes",
            "Physics",
            "Medicines",
            "Potions",
            "Tinctures",
            "Medicaments",
            "Elixirs",
            "Pharmacy",
            "Apothecary",
            "Unguents",
            "Medicinal Agents",
            "Herb Garden",
            "Pharmaceuticals",
            "Chemistry",
            "Chemicals",
            "Experiental Products",
            "Alchemistry",
            "Alchemical Solutions",
            "Metallurgy",
        };

        static string[] GemStoresB = new string[]
        {
            "Gems",
            "Gemstones",
            "Jewelry",
            "Jewels",
            "Precious Stones",
            "Bijoutry",
            "Jewelers",
            "Jewel Box",
            "Jewelry Shop",
            "Gemcutter",
        };

        static string[] PawnStoresB = new string[]
        {
            "Pawnshop",
            "Pawnbrokers",
            "Used Supplies",
            "Used Gear",
            "Used Equipment",
            "Used Merchandise",
            "Hockshop",
            "Antiquities",
        };

        static string[] FurnitureStoresB = new string[]
        {
            "Furniture",
            "Furnishings",
            "Interior Design",
            "Furniture Shop",
            "Decor",
            "Carpentry",
            "Woodworking",
            "Crafts",
            "Woodwork",
        };

        // Currently unused
        //static string[] LivestockStoresB = new string[]
        //{
        //    "Livestock",
        //    "Breeders",
        //    "Farm Animals",
        //    "Beasts",
        //    "Creatures",
        //    "Game",
        //    "Animalia",
        //    "Fauna",
        //    "Menagerie",
        //};

        #endregion

        #region Taverns

        static string[] TavernsA = new string[]
        {
            "The Queen's",
            "The King's",
            "The Dirty",
            "The Black",
            "The Mole and",
            "The Green",
            "The Red",
            "The Gold",
            "The White",
            "The Silver",
            "The Crimson",
            "The Flying",
            "The Dancing",
            "The Laughing",
            "The Restless",
            "The Thirsty",
            "The Unfortunate",
            "The Lucky",
            "The Devil's",
            "The Rusty",
            "The Howling",
            "The Screaming",
            "The Bat and",
            "The Lion and",
            "The Lynx and",
            "The Dwarf and",
            "The Beaver and",
            "The Fox and",
            "The Mouse and",
            "The Pig and",
            "The Feather and",
            "The Toad and",
            "The Rat and",
            "The Savage",
            "The Knave and",
            "The Dead",
        };

        static string[] TavernsB = new string[]
        {
            "Chasm",
            "Mug",
            "Pit",
            "Cat",
            "Dog",
            "Goblin",
            "Griffin",
            "Dragon",
            "Ogre",
            "Giant",
            "Djinn",
            "Wolf",
            "Huntsman",
            "Dagger",
            "Skull",
            "Sword",
            "Guard",
            "Dungeon",
            "Helm",
            "Castle",
            "Jug",
            "Bird",
            "Gnome",
            "Hedgehog",
            "Muskrat",
            "Woodchuck",
            "Scorpion",
            "Badger",
            "Goat",
            "Porcupine",
            "Priest",
            "Fawn",
            "Stag",
            "Barbarian",
            "Rascal",
            "Fairy",
        };

        #endregion

    }
}