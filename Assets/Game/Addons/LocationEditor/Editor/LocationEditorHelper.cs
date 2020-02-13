using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game.Utility.LocationEditor
{
    public static class LocationEditorHelper {
        public const string locationFolder = "/StreamingAssets/WorldData/";

        public const byte InteriorHousePart = 13;
        public const byte ExteriorBuilding = 4;
        public const byte InteractiveObject = 3;

        public enum DataType { Object3D, Flat, Person, Door };

        public const int ladder = 41409;
        public const uint houseContainerObjectGroup = 418;
        public const uint containerObjectGroupOffset = 41000;
        public static List<uint> shopShelvesObjectGroupIndices = new List<uint> { 5, 6, 11, 12, 13, 14, 15, 16, 17, 18, 19, 26, 28, 29, 31, 35, 36, 37, 40, 41, 42, 44, 46, 47, 48, 49, 808 };
        public static List<uint> houseContainerObjectGroupIndices = new List<uint> { 3, 4, 7, 8, 27, 32, 33, 34, 35, 37, 38, 50, 51 };

        public static string[] paintings = { "51115", "51116", "51117", "51118", "51119", "51120" };
        public static string[] carpets = { "74800", "74801", "74802", "74803", "74804", "74805", "74806", "74807", "74808" };
        public static string[] crates = { "41815", "41816", "41817", "41818", "41820", "41821", "41823", "41824", "41825", "41826", "41827", "41828", "41829", "41830", "41831", "41832"};
        public static string[] tables = { "41108", "41109", "41110", "41111", "41112"};
        public static string[] woodenTreeLog = { "41734", "41735", "41736", "41737", "41738" };
        public static string[] chests = { "41811", "41812", "41813" };
        public static string[] fountains = { "41220", "41221", "41222" };
        public static string[] ships = { "909", "910", "41502", "41504", "41501"};
        public static string[] beds = { "41000", "41001", "41002" };
        public static string[] chair = { "41100", "41101", "41102", "41103", "41104" };
        public static string[] bench = { "41105", "41106", "41107"};
        public static string[] sword = { "74095", "74224", "74227" };
        public static string[] brownStoneWallPiece = { "21104", "21105", "21106", "21107" };

        public static string[] bottles = { "205.11", "205.12", "205.13", "205.14", "205.15", "205.16"};
        public static string[] pottedPlants = { "213.2", "213.3", "213.4", "213.5", "213.6"};
        public static string[] pots = { "218.0", "218.1", "218.2", "218.3", "211.2" };
        public static string[] grainBags = { "205.17", "205.18", "205.19", "205.20" };
        public static string[] flowers = { "254.26", "254.27", "254.28", "254.29" , "432.19"};
        public static string[] chalice = { "200.0", "200.1" };
        public static string[] hangingPlant = { "213.13", "213.14" };
        public static string[] woodPillar = { "100.14", "100.15", "100.16" };

        //List of objects
        public static Dictionary<string, string> models_civil = new Dictionary<string, string>()
        {
                {"909", "Ships / Boats [5]"},
                {"6800", "Half Wall"},
                {"21103", "Broken Wooden Fence"},
                {"21104", "Brown Stone Wall Piece [4]"},
                {"22235", "Crystal" },
                {"41000", "Beds [3]" },
                {"41006", "Double Alchemy Shelf"},
                {"41009", "Spinning Wheel" },
                {"41030", "Empty Shelf" },
                {"41034", "Small Dresser" },
                {"41039", "Food Shelf"},
                {"41100", "Wooden Chairs [5]"},
                {"41012", "Shelf Clothes" },
                {"41105", "Benches [3]" },
                {"41108", "Tables [5]"},
                {"41113", "Stol"},
                {"41114", "Stol"},
                {"41116", "Fireplace"},
                {"41117", "Fireplace side"},
                {"41120", "Organ"},
                {"41122", "Wooden Throne"},
                {"41123", "Wooden Throne 2"},
                {"41124", "Bar Shelf 2"},
                {"41127", "Wooden Plank"},
                {"41130", "Short Table"},
                {"41206", "Wooden Fence Endpiece 2"},
                {"41208", "Wooden Fence Endpiece"},
                {"41209", "Water Tank"},
                {"41210", "Water Tank Empty"},
                {"41214", "Wooden Cart"},
                {"41220", "Fountain [3]"},
                {"41227", "Tower Hex"},
                {"41228", "Tower Hex 2"},
                {"41233", "Tower Star 1"},
                {"41234", "Tower Star 2"},
                {"41238", "Plant Wall"},
                {"41400", "Wooden Pole"},
                {"41403", "Construction Plattform 1"},
                {"41404", "Construction Plattform 2"},
                {"41407", "Catapult"},
                {"41409", "Ladder"},
                {"41700", "Medieval Stock 1"},
                {"41701", "Medieval Stock 2"},
                {"41702", "Wood Portal"},
                {"41734", "Wooden Tree Log [5]"},
                {"41739", "Sign"},
                {"41800", "Small Dresser"},
                {"41801", "Dresser"},
                {"41807", "Alchemy Shelf Dresser"},
                {"41811", "Chest [3]"},
                {"41815", "Wooden Crates [16]"},
                {"42548", "Hanging Banner Daggerfall Crest (Dragons)"},
                {"42549", "Hanging Banner Sentinel Crest (Moon)"},
                {"42550", "Hanging Banner Sentinel Crest (Waycrest)"},
                {"42551", "Hanging Banner Grey yellow balls"},
                {"42552", "Hanging Banner Bull, red, orange"},
                {"42553", "Hanging Banner Crow blue/green"},
                {"42554", "Hanging Banner Purple lamp"},
                {"42555", "Hanging Banner Yellow Fire)"},
                {"42556", "Hanging Banner Red"},
                {"42557", "Hanging Banner Green"},
                {"42558", "Hanging Banner Blue"},
                {"42559", "Hanging Banner Yellow)"},
                {"43001", "Standing Wooden Boards"},
                {"43307", "Wooden Bench"},            
                {"51115", "Painting [6]" },
                {"62318", "Wooden Piece"},
                {"62319", "Wooden Board"},
                {"74096", "Swords / Blades [3]"},
                {"74219", "Sickle"},
                {"74221", "Crossbow"},
                {"74225", "Axe"},
                {"74226", "Armor"},
                {"74228", "Crossbow"},
                {"74800", "Carpets [9]"},
                {"99800", "Arrow"}
            };
        public static Dictionary<string, string> models_nature = new Dictionary<string, string>() {
                {"60610", "Black Rock"},
                {"60711", "Rock 2"},
                {"60712", "Rock 3"},
                {"60713", "Rock 4"},
                {"60714", "Rock 5"},
                {"60715", "Rock 6"},
                {"60716", "Rock 7"},
                {"60717", "Rock 8"},
                {"60718", "Rock 9"},
                {"60719", "Rock 10"},
                {"60720", "Rock 11"}
        };
        public static Dictionary<string, string> models_dungeon = new Dictionary<string, string>() {
                { "41313", "Cage"},
                {"41325", "Wooden Casket w. Zombie"},
                {"43003", "Stone Gate Dark"},
                {"43004", "Stone Gate Red"},
                {"43005", "Stone Gate Gray"},
                {"43006", "Stone Gate White"},
                {"43007", "Stone Gate Dark"},
                {"43008", "Stone Gate Red"},
                {"43009", "Stone Gate Gray"},
                {"43010", "Stone Gate White"},
                {"43011", "Gravestone Dark"},
                {"43012", "Gravestone Red"},
                {"43013", "Gravestone Gray"},
                {"43014", "Gravestone White"},
                {"43015", "Gravestone Dark"},
                {"43016", "Gravestone Red"},
                {"43017", "Gravestone Gray"},
                {"43018", "Gravestone White"},
                {"43019", "Gravestone Dark"},
                {"43020", "Gravestone Red"},
                {"43021", "Gravestone Gray"},
                {"43022", "Gravestone White"},
                {"43023", "Gravestone Dark"},
                {"43024", "Gravestone Red"},
                {"43025", "Gravestone Gray"},
                {"43026", "Gravestone White"},
                {"43027", "Gravestone Ground Dark"},
                {"43028", "Gravestone Ground Red"},
                {"43029", "Gravestone Ground Gray"},
                {"43030", "Gravestone Ground White"},
                {"43031", "Gravestone Ground Dark"},
                {"43032", "Gravestone Ground Red"},
                {"43033", "Gravestone Ground Gray"},
                {"43034", "Gravestone Ground White"},
                {"43035", "Gravestone Ground Dark"},
                {"43036", "Gravestone Ground Red"},
                {"43037", "Gravestone Ground Gray"},
                {"43038", "Gravestone Ground White"},
                {"43051", "Gravestone Dark"},
                {"43052", "Gravestone Red"},
                {"43053", "Gravestone Gray"},
                {"43054", "Gravestone White"},
                {"43055", "Gravestone Dark"},
                {"43056", "Gravestone Red"},
                {"43057", "Gravestone Gray"},
                {"43058", "Gravestone White"},
                {"43059", "Gravestone Dark"},
                {"43060", "Gravestone Red"},
                {"43061", "Gravestone Gray"},
                {"43062", "Gravestone White"},
                {"43063", "Gravestone Dark"},
                {"43064", "Gravestone Red"},
                {"43065", "Gravestone Gray"},
                {"43066", "Gravestone White"},
                {"43071", "Gravestone Tall Dark"},
                {"43072", "Gravestone Tall Red"},
                {"43073", "Gravestone Tall Gray"},
                {"43074", "Gravestone Tall White"},
                {"43075", "Stone Casket Dark"},
                {"43076", "Stone Casket Red"},
                {"43077", "Stone Casket Gray"},
                {"43078", "Stone Casket White"},
                {"43079", "Stone Tomb Dark"},
                {"43080", "Stone Tomb Red"},
                {"43081", "Stone Tomb Gray"},
                {"43082", "Stone Tomb White"},
                {"43083", "Stone Ankh Dark"},
                {"43084", "Stone Ankh Red"},
                {"43085", "Stone Ankh Gray"},
                {"43086", "Stone Ankh White"},
                {"43101", "Gravestone Tall Dark"},
                {"43102", "Gravestone Tall Red"},
                {"43103", "Gravestone Tall Gray"},
                {"43104", "Gravestone Tall White"},
                {"43105", "Small Obelisk Dark"},
                {"43106", "Small Obelisk Red"},
                {"43107", "Small Obelisk Gray"},
                {"43108", "Small Obelisk White"},
                {"43109", "Cemetery Gazebo Dark"},
                {"43110", "Cemetery Gazebo Red"},
                {"43111", "Cemetery Gazebo Gray"},
                {"43112", "Cemetery Gazebo White"},
                {"43113", "Gravestone Wall Dark"},
                {"43114", "Gravestone Wall Red"},
                {"43115", "Gravestone Wall Gray"},
                {"43116", "Gravestone Wall White"},
                {"43117", "Gravestone Dark"},
                {"43118", "Gravestone Red"},
                {"43119", "Gravestone Gray"},
                {"43120", "Gravestone White"},
                {"43121", "Stone Ankh Dark"},
                {"43122", "Stone Ankh Red"},
                {"43123", "Stone Ankh Gray"},
                {"43124", "Stone Ankh White"},
                {"43125", "Gravestone Tall Dark"},
                {"43126", "Gravestone Tall Red"},
                {"43127", "Gravestone Tall Gray"},
                {"43128", "Gravestone Tall White"},
                {"43129", "Gravestone Broken Dark"},
                {"43130", "Gravestone Broken Red"},
                {"43131", "Gravestone Broken Gray"},
                {"43132", "Gravestone Broken White"},
                {"43133", "Gravestone Dark"},
                {"43134", "Gravestone Red"},
                {"43135", "Gravestone Gray"},
                {"43136", "Gravestone White"},
                {"43137", "Mausoleum Gray"},
                {"43138", "Mausoleum Dark"},
                {"43139", "Mausoleum Red"},
                {"43140", "Mausoleum Gray"},
                {"43141", "Mausoleum White"},
                {"43142", "Gravestone Dark"},
                {"43143", "Gravestone Red"},
                {"43144", "Gravestone Gray"},
                {"43145", "Gravestone White"},
                {"43201", "Gravestone"},
                {"43202", "Broken Fancy Gravestone"},
                {"43204", "Mausoleum Entrance"},
                {"43205", "Broken Gravestone"},
                {"43206", "Broken Gravestone"},
                {"43300", "Gravestone"},
                {"43301", "Gravestone"},
                {"43302", "Gravestone"},
                {"43303", "Tall Gravestone"},
                {"43304", "Stone Casket"},
                {"43305", "Stone Casket 2"},
                {"43306", "Stone Casket 3"},
                {"54001", "Trap Door" },
                {"61228", "Unlit Torch"},
                {"61227", "Unlit Torch"},
                {"62031", "Rope Bridge"},
                {"62310", "Stone Arch Round"},
                {"62313", "Stone Arch Square"},
                {"62314", "Stone Obelisk"},
                {"62315", "Marble Pillar"},
                {"62322", "Marble Slab"},
                {"62324", "Stone Statue LightGrey"},
                {"62325", "Stone Statue LightGrey 2"},
                {"62328", "Stone Statue Dark"},
                {"62330", "Stone Statue Dark 2"},
                {"62317", "Marble Arch"},
                {"74009", "Marble Pillar 2"},
                {"74082", "Wood Pedestal"},
                {"74086", "Marble Pedestal"},
                {"74091", "Stone Pedestal"},
                {"74094", "Open Pillar"}
        };
        public static Dictionary<string, string> models_signs = new Dictionary<string, string>()
        {            
                {"43700", "Arkay Temple Suspended Sign"},
                {"43701", "Julianos Temple Suspended Sign"},
                {"43702", "Alchemist Shop Suspended Sign"},
                {"43703", "Clothes Shop Suspended Sign"},
                {"43704", "Mages Guild Suspended Sign"},
                {"43705", "Zenithar Temple Suspended Sign"},
                {"43706", "Dibella Temple Suspended Sign"},
                {"43707", "Jewelry Shop Suspended Sign"},
                {"43708", "Tavern Suspended Sign"},
                {"43709", "Mara Temple Suspended Sign"},
                {"43710", "Stendarr Temple Suspended Sign"},
                {"43711", "Bank Suspended Sign"},
                {"43712", "General Store Suspended Sign"},
                {"43713", "Pawn Shop Suspended Sign"},
                {"43714", "Akatosh Temple Suspended Sign"},
                {"43715", "Kynareth Temple Suspended Sign"},
                {"43716", "Library Suspended Sign"},
                {"43717", "Weapon Shop Suspended Sign"},
                {"43718", "Arkay Temple Hanging Sign"},
                {"43719", "Julianos Temple Hanging Sign"},
                {"43720", "Alchemist Shop Hanging Sign"},
                {"43721", "Clothes Shop Hanging Sign"},
                {"43722", "Mages Guild Hanging Sign"},
                {"43723", "Zenithar Temple Hanging Sign"},
                {"43724", "Dibella Temple Hanging Sign"},
                {"43725", "Armor Shop Hanging Sign"},
                {"43726", "Jewelry Shop Hanging Sign"},
                {"43727", "Tavern Hanging Sign"},
                {"43728", "Mara Temple Hanging Sign"},
                {"43729", "Stendarr Temple Hanging Sign"},
                {"43730", "Bank Hanging Sign"},
                {"43731", "General Store Hanging Sign"},
                {"43732", "Pawn Shop Hanging Sign"},
                {"43733", "Akatosh Temple Hanging Sign"},
                {"43734", "Kynareth Temple Hanging Sign"},
                {"43735", "Library Hanging Sign"},
                {"43736", "Weapon Shop Hanging Sign"},
                {"43737", "Arkay Temple Standing Sign"},
                {"43738", "Julianos Temple Standing Sign"},
                {"43739", "Alchemist Shop Standing Sign"},
                {"43740", "Clothes Shop Standing Sign"},
                {"43741", "Mages Guild Standing Sign"},
                {"43742", "Zenithar Temple Standing Sign"},
                {"43743", "Dibella Temple Standing Sign"},
                {"43744", "Armor Shop Standing Sign"},
                {"43745", "Jewelry Shop Standing Sign"},
                {"43746", "Tavern Standing Sign"},
                {"43747", "Mara Temple Standing Sign"},
                {"43748", "Stendarr Temple Standing Sign"},
                {"43749", "Bank Standing Sign"},
                {"43750", "General Store Standing Sign"},
                {"43751", "Pawn Shop Standing Sign"},
                {"43752", "Akatosh Temple Standing Sign"},
                {"43753", "Kynareth Temple Standing Sign"},
                {"43754", "Library Standing Sign"},
                {"43755", "Weapon Shop Standing Sign"},           
            };
        public static Dictionary<string, string> NPCs = new Dictionary<string, string>() {
            {"175.0", "Daedra Lord #0"},
            {"175.1", "Daedra Lord #1"},
            {"175.2", "Daedra Lord #2"},
            {"175.3", "Daedra Lord #3"},
            {"175.4", "Daedra Lord #4"},
            {"175.5", "Daedra Lord #5"},
            {"175.6", "Daedra Lord #6"},
            {"175.7", "Daedra Lord #7"},
            {"175.8", "Daedra Lord #8"},
            {"175.9", "Daedra Lord #9"},
            {"175.10","Daedra Lord #10"},
            {"175.11","Daedra Lord #11"},
            {"175.12","Daedra Lord #12"},
            {"175.13","Daedra Lord #13"},
            {"175.14","Daedra Lord #14"},
            {"175.15","Daedra Lord #15"},
            {"176.0","Dark BrotherHood #0"},
            {"176.1","Dark BrotherHood #1"},
            {"176.2","Dark BrotherHood #2"},
            {"176.3","Dark BrotherHood #3"},
            {"176.4","Dark BrotherHood #4"},
            {"176.5","Dark BrotherHood #5"},
            {"176.6","Dark BrotherHood #6"},
            {"177.0","Mage #0"},
            {"177.1","Mage #1"},
            {"177.2","Mage #2"},
            {"177.3","Mage #3"},
            {"177.4","Mage #4"},
            {"177.5","Smith"},
            {"178.0","Necromancers #0"},
            {"178.1","Necromancers #1"},
            {"178.2","Necromancers #2"},
            {"178.3","Necromancers #3"},
            {"178.4","Necromancers #4"},
            {"178.5","Necromancers #5"},
            {"178.6","Necromancers #6"},
            {"179.0","Witches Coven #0"},
            {"179.1","Witches Coven #1"},
            {"179.2","Witches Coven #2"},
            {"179.3","Witches Coven #3"},
            {"179.4","Witches Coven #4"},
            {"180.0","Courtiers #0"},
            {"180.1","Courtiers #1"},
            {"180.2","Courtiers #2"},
            {"180.3","Courtiers #3"},
            {"181.0","Temple #0"},
            {"181.1","Temple #1"},
            {"181.2","Temple #2"},
            {"181.3","Temple #3"},
            {"181.4","Temple #4"},
            {"181.5","Temple #5"},
            {"181.6","Temple #6"},
            {"181.7","Temple #7"},
            {"182.0","Shop Keeper"},
            {"182.1","Tavern owner 1"},
            {"182.2","Tavern owner 2"},
            {"182.3","Tavern owner 3"},
            {"182.4","Common #4"},
            {"182.5","Jester"},
            {"182.6","Jester 2"},
            {"182.7","Common #7"},
            {"182.8","Common #8"},
            {"182.9","Common #9"},
            {"182.10","Common #10"},
            {"182.11","Common #11"},
            {"182.12","Common #12"},
            {"182.13","Common #13"},
            {"182.14","Common #14"},
            {"182.15","Common #15"},
            {"182.16","Common #16"},
            {"182.17","Common #17"},
            {"182.18","Common #18"},
            {"182.19","Common #19"},
            {"182.20","Common #20"},
            {"182.21","Common #21"},
            {"182.22","Common #22"},
            {"182.23","Common #23"},
            {"182.24","Common #24"},
            {"182.25","Common #25"},
            {"182.26","Common #26"},
            {"182.27","Common #27"},
            {"182.28","Common #28"},
            {"182.29","Common #29"},
            {"182.30","Common #30"},
            {"182.31","Common #31"},
            {"182.32","Common #32"},
            {"182.33","Common #33"},
            {"182.34","Common #34"},
            {"182.35","Common #35"},
            {"182.36","Common #36"},
            {"182.37","Common #37"},
            {"182.38","Common #38"},
            {"182.39","Common #39"},
            {"182.40","Common #40"},
            {"182.41","Common #41"},
            {"182.42","Common #42"},
            {"182.43","Common #43"},
            {"182.44","Common #44"},
            {"182.45","Common #45"},
            {"182.46","Common #46"},
            {"182.47","Common #47"},
            {"182.48","Common #48"},
            {"182.49","Common #49"},
            {"182.50","Common #50"},
            {"182.51","Common #51"},
            {"182.52","Common #52"},
            {"182.53","Common #53"},
            {"182.54","Common #54"},
            {"182.55","Common #55"},
            {"182.56","Common #56"},
            {"182.57","Common #57"},
            {"182.58","Common #58"},
            {"182.59","Common #59"},
            {"183.0","Noble #0"},
            {"183.1","Noble #1"},
            {"183.2","Noble #2"},
            {"183.3","Noble #3"},
            {"183.4","Noble #4"},
            {"183.5","Noble #5"},
            {"183.6","Noble #6"},
            {"183.7","Noble #7"},
            {"183.8","Noble #8"},
            {"183.9","Noble #9"},
            {"183.10","Noble #10"},
            {"183.11","Noble #11"},
            {"183.12","Noble #12"},
            {"183.13","Noble #13"},
            {"183.14","Noble #14"},
            {"183.15","Noble #15"},
            {"183.16","Noble #16"},
            {"183.17","Noble #17"},
            {"183.18","Noble #18"},
            {"183.19","Noble #19"},
            {"183.20","Noble #20"},
            {"183.21","Noble #21"},
            {"184.0", "People #0"},
            {"184.1", "People #1"},
            {"184.2", "People #2"},
            {"184.3", "People #3"},
            {"184.4", "People #4"},
            {"184.5", "People #5"},
            {"184.6", "People #6"},
            {"184.7", "People #7"},
            {"184.8", "People #8"},
            {"184.9", "People #9"},
            {"184.10", "People #10"},
            {"184.11", "People #11"},
            {"184.12", "People #12"},
            {"184.13", "People #13"},
            {"184.14", "People #14"},
            {"184.15", "People #15"},
            {"184.16", "People #16"},
            {"184.17", "People #17"},
            {"184.18", "People #18"},
            {"184.19", "People #19"},
            {"184.20", "People #20"},
            {"184.21", "People #21"},
            {"184.22", "People #22"},
            {"184.23", "People #23"},
            {"184.24", "People #24"},
            {"184.25", "People #25"},
            {"184.26", "People #26"},
            {"184.27", "People #27"},
            {"184.28", "People #28"},
            {"184.29", "People #29"},
            {"184.30", "People #30"},
            {"184.31", "People #31"},
            {"184.32", "People #32"},
            {"184.33", "People #33"},
            {"184.34", "People #34"},
        };
        public static Dictionary<string, string> billboards_interior = new Dictionary<string, string>()
             {
                {"097.1", "Statue of Zenithar"},
                {"097.3", "Statue of Stendarr"},
                {"097.12", "Statue of Dibella"},
                {"097.13", "Statue of Kynareth"},
                {"100.0", "Blood"},
                {"100.1", "Blood 2"},
                {"100.2", "Skeleton Hand"},
                {"100.3", "Hand"},
                {"100.4", "Cage from Ceiling"},
                {"100.5", "Short Chain from Ceiling"},
                {"100.6", "Dual Chains from Ceiling"},
                {"100.7", "Dual Chains from Celing"},
                {"100.8", "Long Chain from Ceiling"},
                {"100.9", "Horned Skull"},
                {"100.10", "Decapitated Heads"},
                {"100.11", "Head on a Pole"},
                {"100.12", "Head on a Pole 2"},
                {"100.13", "Bloody Stick"},
                {"100.14", "Wood Pillar"},
                {"100.17", "Pile pf skulls"},
                {"100.18", "Ribcage"},
                {"100.19", "Single Vine"},
                {"100.20", "Dual Vines"},
                {"100.21", "Dual Vines 2"},
                {"100.22", "Dual Vines 3"},
                {"100.23", "White Skull"},
                {"100.24", "Skull on a Stick"},
                {"100.25", "Broken Skull"},
                {"100.26", "Beast Skull"},
                {"100.27", "Gray Skull"},
                {"100.28", "Impaled Body"},
                {"100.29", "Impaled Body 2"},
                {"101.1", "Chandelier"},
                {"101.2", "Chandelier Blue"},
                {"101.3", "Chandelier 2"},
                {"101.4", "Chandelier 3"},
                {"101.6", "Hanging Light"},
                {"101.7", "Hanging Light 2"},
                {"101.8", "Hanging Light 3"},
                {"101.9", "Hanging Light 4"},
                {"101.10", "Hanging Sphere Light"},
                {"101.11", "White Skull w. Candle"},
                {"101.12", "Burning Skull on Stick"},
                {"200.00", "Chalice / Drinking Cup [2]"},
                {"200.11", "White Pillow"},
                {"200.13", "Pink Pillow"},
                {"200.14", "Chair"},
                {"200.15", "Cloth Roll"},
                {"200.16", "Cloth Roll empty"},
                {"200.17", "Pile of Coal"},
                {"200.18", "Crib"},
                {"201.00", "Horse Brown"},
                {"201.01", "Horse Gray"},
                {"201.02", "Camel"},
                {"201.03", "Cow 1"},
                {"201.04", "Cow 2"},
                {"201.05", "Pig 1"},
                {"201.06", "Pig 2"},
                {"201.07", "Cat 1"},
                {"201.08", "Cat 2"},
                {"201.09", "Dog"},
                {"201.10", "Dog 2"},
                {"201.11", "Seagull"},
                {"204.0", "Pile of Clothes"},
                {"204.1", "Blue Shoes"},
                {"204.2", "Brown Shoes"},
                {"204.3", "Hat w.feather"},
                {"204.4", "Hat"},
                {"204.5", "Pair of Hats"},
                {"204.6", "Pink Fabric"},
                {"204.7", "Yellow Fabric"},
                {"204.8", "Blue Fabric"},
                {"205.0", "Barrel"},
                {"205.1", "Bottle w. Yellow liquid"},
                {"205.2", "Bottle Big w. content"},
                {"205.3", "Bottle Big Empty"},
                {"205.4", "Bottle Big w. Brain"},
                {"205.5", "Bottle Big w. Orange liquid"},
                {"205.6", "Bottle w. Orange liquid"},
                {"205.7", "Bottle Big w. Purple liquid"},
                {"205.8", "Small Basket"},
                {"205.9", "Large Basket"},
                {"205.10", "Large Basket w. Fish"},
                {"205.11", "Wine Bottle [6]"},
                {"205.17", "Grain/Flour Bag/Sack [4]"},
                {"205.21", "Chest"},
                {"205.29", "Bucket"},
                {"208.0", "Globe"},
                {"208.1", "Maginfying Glass"},
                {"208.2", "Potion Fire"},
                {"208.3", "Scale"},
                {"208.4", "Telescope"},
                {"208.5", "Hand Mirror"},
                {"209.0", "Books x3"},
                {"209.1", "Books x2"},
                {"209.2", "Brown Book"},
                {"209.3", "Green Book"},
                {"209.4", "Large Book"},
                {"209.5", "Scroll"},
                {"209.6", "Scrolls x3"},
                {"209.7", "Paper"},
                {"209.8", "Paper"},
                {"209.9", "Small Paper"},
                {"209.10", "Papers x3"},
                {"211.0", "Bandages"},
                {"211.1", "Feather Pen"},
                {"211.10", "Fish"},
                {"211.11", "Hanging Fish"},
                {"211.21", "Rocking Horse"},
                {"211.22", "Nuse"},
                {"211.24", "Pipe"},
                {"211.40", "Meat"},
                {"211.43", "Cloak"},
                {"212.0", "Well"},
                {"212.5", "Blank Sign"},
                {"212.11", "Wood Pile"},
                {"213.0", "Orange"},
                {"213.1", "Tomato"},
                {"213.2", "Potted Plants [5]"},
                {"213.13", "Hanging Plant [2]"},
                {"214.1", "Shovel"},
                {"214.2", "Hammer"},
                {"214.3", "Hammer"},
                {"214.5", "Butter Churner"},
                {"214.6", "Pick Axe"},
                {"214.7", "Hoe"},
                {"214.8", "Rope"},
                {"214.10", "Broom"},
                {"218.0", "Pots [5]"},
                {"218.4", "Frying Pan"},
                {"218.6", "Hanging Spoon"},
                {"254.26", "Flower [5]"},
            };
        public static Dictionary<string, string> billboards_nature = new Dictionary<string, string>()
             {
                {"500.01", "Rain Forest Bush"},
                {"500.02", "Rain Forest Bush"},
                {"500.03", "Rain Forest Bush"},
                {"500.04", "Rain Forest Rock"},
                {"500.05", "Rain Forest Plant"},
                {"500.06", "Rain Forest Plant"},
                {"500.07", "Rain Forest Plant"},
                {"500.08", "Rain Forest Plant"},
                {"500.09", "Rain Forest Fern"},
                {"500.10", "Rain Forest Fern"},
                {"500.11", "Rain Forest Plant"},
                {"500.12", "Rain Forest Tree"},
                {"500.13", "Rain Forest Tree"},
                {"500.14", "Rain Forest Tree"},
                {"500.15", "Rain Forest Tree"},
                {"500.16", "Rain Forest Tree"},
                {"500.17", "Rain Forest Rock"},
                {"500.18", "Rain Forest Tree"},
                {"500.19", "Rain Forest Plant"},
                {"500.20", "Rain Forest Plant"},
                {"500.21", "Rain Forest Plant"},
                {"500.22", "Rain Forest Plant"},
                {"500.23", "Rain Forest Plant"},
                {"500.24", "Rain Forest Plant"},
                {"500.25", "Rain Forest Plant"},
                {"500.26", "Rain Forest Plant"},
                {"500.27", "Rain Forest Plant"},
                {"500.28", "Rain Forest Plant"},
                {"500.29", "Rain Forest Plant"},
                {"500.30", "Rain Forest Tree"},
                {"500.31", "Rain Forest Plant"},
                {"501.01", "Sub_Tropical Plant"},
                {"501.02", "Sub_Tropical Plant"},
                {"501.03", "Sub_Tropical Rock"},
                {"501.04", "Sub_Tropical Rock"},
                {"501.05", "Sub_Tropical Rock"},
                {"501.06", "Sub_Tropical Rock"},
                {"501.07", "Sub_Tropical Plant"},
                {"501.08", "Sub_Tropical Plant"},
                {"501.09", "Sub_Tropical Plant"},
                {"501.10", "Sub_Tropical Plant"},
                {"501.11", "Sub_Tropical Tree"},
                {"501.12", "Sub_Tropical Tree"},
                {"501.13", "Sub_Tropical Tree"},
                {"501.14", "Sub_Tropical Plant"},
                {"501.15", "Sub_Tropical Plant"},
                {"501.16", "Sub_Tropical Tree"},
                {"501.17", "Sub_Tropical Plant"},
                {"501.18", "Sub_Tropical Plant"},
                {"501.19", "Sub_Tropical Plant"},
                {"501.20", "Sub_Tropical Plant"},
                {"501.21", "Sub_Tropical Plant"},
                {"501.22", "Sub_Tropical Plant"},
                {"501.23", "Sub_Tropical Rock"},
                {"501.24", "Sub_Tropical Plant"},
                {"501.25", "Sub_Tropical Plant"},
                {"501.26", "Sub_Tropical Plant"},
                {"501.27", "Sub_Tropical Plant"},
                {"501.28", "Sub_Tropical Plant"},
                {"501.29", "Sub_Tropical Plant"},
                {"501.30", "Sub_Tropical Tree"},
                {"501.31", "Sub_Tropical Plant"},
                {"502.01", "Swamp Plant"},
                {"502.02", "Swamp Plant"},
                {"502.03", "Swamp Rock"},
                {"502.04", "Swamp Rock"},
                {"502.05", "Swamp Rock"},
                {"502.06", "Swamp Rock"},
                {"502.07", "Swamp Plant"},
                {"502.08", "Swamp Plant"},
                {"502.09", "Swamp Plant"},
                {"502.10", "Swamp Rock"},
                {"502.11", "Swamp Plant"},
                {"502.12", "Swamp Tree"},
                {"502.13", "Swamp Tree"},
                {"502.14", "Swamp Plant"},
                {"502.15", "Swamp Tree"},
                {"502.16", "Swamp Tree"},
                {"502.17", "Swamp Tree"},
                {"502.18", "Swamp Tree"},
                {"502.19", "Swamp Plant"},
                {"502.20", "Swamp Plant"},
                {"502.21", "Swamp Plant"},
                {"502.22", "Swamp Plant"},
                {"502.23", "Swamp Plant"},
                {"502.24", "Swamp Plant"},
                {"502.25", "Swamp Plant"},
                {"502.26", "Swamp Plant"},
                {"502.27", "Swamp Plant"},
                {"502.28", "Swamp Plant"},
                {"502.29", "Swamp Plant"},
                {"502.30", "Swamp Tree"},
                {"502.31", "Swamp Plant"},
                {"503.01", "Desert Plant"},
                {"503.02", "Desert Rock"},
                {"503.03", "Desert Rock"},
                {"503.04", "Desert Rick"},
                {"503.05", "Desert Tree"},
                {"503.06", "Desert Plant"},
                {"503.07", "Desert Plant"},
                {"503.08", "Desert Plant"},
                {"503.09", "Desert Plant"},
                {"503.10", "Desert Plant"},
                {"503.11", "Desert Tree"},
                {"503.12", "Desert Tree"},
                {"503.13", "Desert Tree"},
                {"503.14", "Desert Cactus"},
                {"503.15", "Desert Cactus"},
                {"503.16", "Desert Cactus"},
                {"503.17", "Desert Plant"},
                {"503.18", "Desert Rock"},
                {"503.19", "Desert Rock"},
                {"503.20", "Desert Rock"},
                {"503.21", "Desert Rock"},
                {"503.22", "Desert Rock"},
                {"503.23", "Desert Plant"},
                {"503.24", "Desert Plant"},
                {"503.25", "Desert Plant"},
                {"503.26", "Desert Plant"},
                {"503.27", "Desert Plant"},
                {"503.28", "Desert Tree"},
                {"503.29", "Desert Plant"},
                {"503.30", "Desert Tree"},
                {"504.01", "Woodland Bush"},
                {"504.02", "Woodland Bush"},
                {"504.03", "Woodland Rock"},
                {"504.04", "Woodland Rock"},
                {"504.05", "Woodland Rock"},
                {"504.06", "Woodland Rock"},
                {"504.07", "Woodland Bush"},
                {"504.08", "Woodland Bush"},
                {"504.09", "Woodland Bush"},
                {"504.10", "Woodland Bush"},
                {"504.11", "Woodland Bush"},
                {"504.12", "Woodland Tree"},
                {"504.13", "Woodland Tree"},
                {"504.14", "Woodland Tree"},
                {"504.15", "Woodland Tree"},
                {"504.16", "Woodland Tree"},
                {"504.17", "Woodland Tree"},
                {"504.18", "Woodland Tree"},
                {"504.19", "Woodland Trunk"},
                {"504.20", "Woodland Trunk"},
                {"504.21", "Woodland Flower"},
                {"504.22", "Woodland Flower"},
                {"504.23", "Woodland Mushroom"},
                {"504.24", "Woodland Bush"},
                {"504.25", "Woodland Tree"},
                {"504.26", "Woodland Fern"},
                {"504.27", "Woodland Bush"},
                {"504.28", "Woodland Bush"},
                {"504.29", "Woodland Bush"},
                {"504.30", "Woodland Tree"},
                {"504.31", "Woodland Logs"},
                {"506.01", "Woodland Hills Rock"},
                {"506.02", "Woodland Hills Grass"},
                {"506.03", "Woodland Hills Rock"},
                {"506.04", "Woodland Hills Rock"},
                {"506.05", "Woodland Hills Tree"},
                {"506.06", "Woodland Hills Rock"},
                {"506.07", "Woodland Hills Grass"},
                {"506.08", "Woodland Hills Grass"},
                {"506.09", "Woodland Hills Bush"},
                {"506.10", "Woodland Hills Plant"},
                {"506.11", "Woodland Hills Tree"},
                {"506.12", "Woodland Hills Tree"},
                {"506.13", "Woodland Hills Tree"},
                {"506.14", "Woodland Hills Tree"},
                {"506.15", "Woodland Hills Tree"},
                {"506.16", "Woodland Hills Tree"},
                {"506.17", "Woodland Hills Rock"},
                {"506.18", "Woodland Hills Rock"},
                {"506.19", "Woodland Hills Tree Trunk"},
                {"506.20", "Woodland Hills Tree Trunk"},
                {"506.21", "Woodland Hills Flower"},
                {"506.22", "Woodland Hills Flower"},
                {"506.23", "Woodland Hills Plant"},
                {"506.24", "Woodland Hills Tree"},
                {"506.25", "Woodland Hills Tree"},
                {"506.26", "Woodland Hills Plant"},
                {"506.27", "Woodland Hills Plant"},
                {"506.28", "Woodland Hills Rock"},
                {"506.29", "Woodland Hills Grass"},
                {"506.30", "Woodland Hills Tree"},
                {"506.31", "Woodland Hills Fern"},
                {"508.01", "Haunted Woodland Rock"},
                {"508.02", "Haunted Woodland Flower"},
                {"508.03", "Haunted Woodland Rock"},
                {"508.04", "Haunted Woodland Rock"},
                {"508.05", "Haunted Woodland Rock"},
                {"508.06", "Haunted Woodland Rock"},
                {"508.07", "Haunted Woodland Plant"},
                {"508.08", "Haunted Woodland Plant"},
                {"508.09", "Haunted Woodland Plant"},
                {"508.10", "Haunted Woodland Plant"},
                {"508.11", "Haunted Woodland Ribcage"},
                {"508.12", "Haunted Woodland Rock"},
                {"508.13", "Haunted Woodland Tree"},
                {"508.14", "Haunted Woodland Grass"},
                {"508.15", "Haunted Woodland Tree"},
                {"508.16", "Haunted Woodland Tree"},
                {"508.17", "Haunted Woodland Rock"},
                {"508.18", "Haunted Woodland Tree"},
                {"508.19", "Haunted Woodland Tree Trunk"},
                {"508.20", "Haunted Woodland Tree Trunk"},
                {"508.21", "Haunted Woodland Flower"},
                {"508.22", "Haunted Woodland Mushroom"},
                {"508.23", "Haunted Woodland Mushroom"},
                {"508.24", "Haunted Woodland Tree"},
                {"508.25", "Haunted Woodland Tree"},
                {"508.26", "Haunted Woodland Fern"},
                {"508.27", "Haunted Woodland Bush"},
                {"508.28", "Haunted Woodland Bush"},
                {"508.29", "Haunted Woodland Grass"},
                {"508.30", "Haunted Woodland Tree"},
                {"508.31", "Haunted Woodland Logs"},
                {"510.01", "Mountain Rock"},
                {"510.02", "Mountain Grass"},
                {"510.03", "Mountain Rock"},
                {"510.04", "Mountain Rock"},
                {"510.05", "Mountain Tree"},
                {"510.06", "Mountain Rock"},
                {"510.07", "Mountain Grass"},
                {"510.08", "Mountain Plant"},
                {"510.09", "Mountain Flower"},
                {"510.10", "Mountain Plant"},
                {"510.11", "Mountain Tree"},
                {"510.12", "Mountain Tree"},
                {"510.13", "Mountain Tree"},
                {"510.14", "Mountain Rock"},
                {"510.15", "Mountain Tree"},
                {"510.16", "Mountain Tree"},
                {"510.17", "Mountain Rock"},
                {"510.18", "Mountain Rock"},
                {"510.19", "Mountain Tree Trunk"},
                {"510.20", "Mountain Tree Trunk"},
                {"510.21", "Mountain Bush"},
                {"510.22", "Mountain Flower"},
                {"510.23", "Mountain Flower"},
                {"510.24", "Mountain Tree"},
                {"510.25", "Mountain Tree"},
                {"510.26", "Mountain Plant"},
                {"510.27", "Mountain Rock"},
                {"510.28", "Mountain Rock"},
                {"510.29", "Mountain Grass"},
                {"510.30", "Mountain Tree"},
                {"510.31", "Mountain Plant"},
            };
        public static Dictionary<string, string> billboards_lights = new Dictionary<string, string>()
{
                {"210.0", "Bowl of Fire"},
                {"210.1", "Camp Fire"},
                {"210.2", "Skull Candle"},
                {"210.3", "Candle"},
                {"210.4", "Candle w. Base"},
                {"210.5", "Candleholder with 3 candles"},
                {"210.6", "Skull torch"},
                {"210.7", "Wooden Chandelier w. Extinguished Candles"},
                {"210.8", "Turkis Lamp"},
                {"210.9", "Metallic Chandelier w. Burning Candles"},
                {"210.10", "Metallic Chandelier w. Extinguished Candles"},
                {"210.11", "Candle in Lamp"},
                {"210.12", "Extinguished Lamp"},
                {"210.13", "Round Lamp"},
                {"210.14", "Standing Lantern"},
                {"210.15", "Standing Lantern Round"},
                {"210.16", "Mounted Torch w. Thin Holder"},
                {"210.17", "Mounted Torch 1"},
                {"210.18", "Mounted Torch 2"},
                {"210.19", "Pillar w. Firebowl"},
                {"210.20", "Brazier Torch"},
                {"210.21", "Standing Candle"},
                {"210.22", "Round Lantern w. Medium Chain"},
                {"210.23", "Wooden Chandelier w. Burning Candles"},
                {"210.24", "Lantern w. Long Chain"},
                {"210.25", "Lantern w. Medium Chain"},
                {"210.26", "Lantern w. Short Chain"},
                {"210.27", "Lantern w, No Chain"},
                {"210.28", "Street Lantern 1"},
                {"210.29", "Street Lantern 2"},
        };
        public static Dictionary<string, string> billboards_treasure = new Dictionary<string, string>() {

               {"216.0", "Goldpile 1"},
                {"216.1", "Goldpile 2"},
                {"216.2", "Goldpile 3"},
                {"216.3", "Gold Casket"},
                {"216.4", "Gold Coin"},
                {"216.5", "Silver Coin"},
                {"216.6", "Gold Crown 1"},
                {"216.7", "Silver Crown 1"},
                {"216.8", "Silver Crown 2"},
                {"216.9", "Gold Crown 2"},
                {"216.10", "Silver plate"},
                {"216.11", "Treasure 1"},
                {"216.12", "Treasure 2"},
                {"216.13", "Treasure 3"},
                {"216.14", "Treasure 4"},
                {"216.15", "Treasure 5"},
                {"216.16", "Treasure 6"},
                {"216.17", "Treasure 7"},
                {"216.18", "Treasure 8"},
                {"216.19", "Treasure 9"},
                {"216.20", "Treasure 10"},
                {"216.21", "Treasure 11"},
                {"216.22", "Treasure 12"},
                {"216.23", "Treasure 13"},
                {"216.24", "Treasure 14"},
                {"216.25", "Treasure 15"},
                {"216.26", "Treasure 16"},
                {"216.27", "Treasure 17"},
                {"216.28", "Treasure 18"},
                {"216.30", "Treasure 19"},
                {"216.31", "Treasure 20"},
                {"216.32", "Treasure 21"},
                {"216.33", "Treasure 22"},
                {"216.34", "Treasure 23"},
                {"216.35", "Treasure 24"},
                {"216.36", "Treasure 25"},
                {"216.37", "Treasure 26"},
                {"216.38", "Treasure 27"},
                {"216.39", "Treasure 28"},
                {"216.40", "Treasure 29"},
                {"216.41", "Treasure 30"},
                {"216.42", "Treasure 31"},
                {"216.43", "Treasure 32"},
                {"216.44", "Treasure 33"},
                {"216.45", "Treasure 34"},
                {"216.46", "Treasure 35"},
                {"216.47", "Treasure 36"},
        };
        public static Dictionary<string, string> billboards_markers = new Dictionary<string, string>() {
             {"199.2", "Marker : Daedra"},
             {"199.3", "Marker : Summon"},
             {"199.4", "Marker : Inn Bed"},
             {"199.8", "Marker : Enter"},
             {"199.11", "Marker : Quest"},
             {"199.12", "Marker : Create"},
             {"199.13", "Marker : Prison"},
             {"199.14", "Marker : Prison Exit"},
             {"199.15", "Marker : Random Monster"},
             {"199.18","Marker : Quest Item"},
             {"199.19","Marker : Random Treasure"},
             {"199.20", "Marker : Random Flat"},
             {"199.21", "Marker : Ladder Bottom"},
             {"199.22", "Marker : Ladder Top"},
        };
        public static Dictionary<string, string> houseParts = new Dictionary<string, string>() {
            {"1000" , "Single Floor" },
            {"2000" , "Single Ceiling" },
            {"3000" , "Single Doorway" },
            {"3002" , "Single Exit" },
            {"3004", "Single Wall"},
            {"4004" , "Single Pillar" },
            {"5000", "1x1 - Stairwell" },
            {"5001", "1x1 - Stairwell" },
            {"5002", "1x1 - Stairwell" },
            {"5003", "1x1 - Stairwell" },
            {"5004", "1x1 - Stairwell" },
            {"5005", "1x1 - Stairwell" },
            {"5006", "1x1 - Stairwell" },
            {"5007", "1x1 - Stairwell" },
            {"8000", "1x1 - Door, Wall, Wall, Wall" },
            {"8001", "1x2 - Door x1" },
            {"8002", "1x3 - Door x1" },
            {"8003", "1x4 - Door x1" },
            {"8004", "1x5 - Door x1" },
            {"8005", "1x6 - Door x1" },
            {"8006", "2x1 - Door x1" },
            {"8007", "2x2 - Door x1" },
            {"8008", "2x3 - Door x1" },
            {"8009", "2x4 - Door x1" },
            {"8010", "2x5 - Door x1" },
            {"8011", "2x6 - Door x1" },
            {"8012", "3x1 - Door x1" },
            {"8013", "3x2 - Door x1" },
            {"8014", "3x3 - Door x1" },
            {"8015", "3x4 - Door x1" },
            {"8016", "3x5 - Door x1" },
            {"8017", "3x6 - Door x1" },
            {"8018", "4x1 - Door x1" },
            {"8019", "4x2 - Door x1" },
            {"8020", "4x3 - Door x1" },
            {"8021", "4x4 - Door x1" },
            {"8022", "4x5 - Door x1" },
            {"8023", "4x6 - Door x1" },
            {"8024", "5x1 - Door x1" },
            {"8025", "5x2 - Door x1" },
            {"8026", "5x3 - Door x1" },
            {"8027", "5x4 - Door x1" },
            {"8028", "5x5 - Door x1" },
            {"8029", "5x6 - Door x1" },
            {"8030", "6x1 - Door x1" },
            {"8031", "6x2 - Door x1" },
            {"8032", "6x3 - Door x1" },
            {"8033", "6x4 - Door x1" },
            {"8034", "6x5 - Door x1" },
            {"8035", "6x6 - Door x1" },
            {"10001", "1x2 - Door x1 (Height 2)" },
            {"10002", "1x3 - Door x1 (Height 2)" },
            {"10003", "1x4 - Door x1 (Height 2)" },
            {"10004", "1x5 - Door x1 (Height 2)" },
            {"10005", "1x6 - Door x1 (Height 2)" },
            {"10006", "2x1 - Door x1 (Height 2)" },
            {"10007", "2x2 - Door x1 (Height 2)" },
            {"10008", "2x3 - Door x1 (Height 2)" },
            {"10009", "2x4 - Door x1 (Height 2)" },
            {"10010", "2x5 - Door x1 (Height 2)" },
            {"10011", "2x6 - Door x1 (Height 2)" },
            {"10012", "2x6 - Door x1 (Height 2)" },
            {"31000", "1x1 - Hole, Wall, Hole, Wall" },
            {"31003", "1x1 - Hole, Wall, Wall, Door" },
            {"31004", "1x1 - Hole, Door, Wall, Wall" },
            {"31005", "1x1 - Hole, Wall, Door, Wall" },
            {"31006", "1x1 - Hole, Wall, Wall, Hole" },
            {"31007", "1x1 - Hole, Door, Hole, Wall" },
            {"31008", "1x1 - Door, Door, Hole, Wall" },
            {"31009", "1x1 - Door, Wall, Hole, Door" },
            {"31010", "1x1 - Wall, Door, Hole, Door" },
            {"31011", "1x1 - Wall, Door, Wall, Door" },
            {"31014", "1x1 - Exit, Door, Wall, Door" },
            {"31016", "1x1 - Hole, Door, Hole, Door" },
            {"31017", "1x1 - Door, Door, Hole, Door" },
            {"31018", "1x1 - Hole, Wall, Wall, Wall" },
            {"31019", "1x1 - Exit, Wall, Hole, Wall" },
            {"31020", "1x1 - Door, Door, Hole, Exit" },
            {"31021", "1x1 - Door, Exit, Hole, Door" },
            {"31022", "1x1 - Stairwell" },
            {"31023", "1x1 - Stairs" },
            {"31024", "1x1 - Hole. Wall, Hole, Hole" },
            {"31025", "1x1 - Hole, Hole, Hole, Hole" },
            {"31026", "1x1 - Hole, Wall, Door, Hole" },
            {"31027", "1x1 - Hole, Hole, Door, Wall" },
            {"31028", "1x1 - Hole, Hole, Door, Door" },
            {"31029", "1x1 - Exit, Hole, Wall, hole" },
            {"31030", "1x1 - Exit, Hole, Hole, Hole" },
            {"31031", "1x1 - Hole, diagWall, diagWall, Hole" },
            {"32000", "3x3 - 1 door, *special" },
            {"33001", "4x4 - 1 door, special" },
            {"33002", "5x5 - 1 door, special" },
            {"33003", "2x3 - 1 door, height 2" },
            {"33004", " 3x3 - 1 door, diag roof, height 2" },
            {"33005", "4x3 - 1 door, diag roof, height 2" },
            {"33006", "5x3 - 1 door, diag roof, height 2" },
            {"33007", "6x3 - 1 door, diag roof, height 2" },
            {"34000", "2x2 - 2 doors, w. Windows" },
            {"34001", "2x4 - 2 doors, w Windows" },
            {"34002", "2x3 2 doors, w Windows" },
            {"34003", "2x4 2 doors, w Windows" },
            {"34004", "2x2, 2 doors" },
            {"34005", "2x4  2 doors" },
            {"34006", "3x2 2 door" },
            {"34007", "4x4 2 door" },
            {"34008", "2x2 3 doors" },
            {"34009", "2x3 3 doors" },
            {"34010", "2x3 2 door" },
            {"34011", "4x4 4 doors" },
            {"35000", "2x2 (2 height)" },
            {"35001", "2z2 4 doors" },
            {"35002", "4x4 4 doors" },
            {"35003", "2x4 3 doors" },
            {"35004", "2x3 3 doors (3 height)" },
            {"35005", "2x3 1 door *special" },
            {"35006", "2x3 2 door window" },
            {"35007", "2x3 3 door window" },
            {"35008", "4x4 2 door window" },
            {"35009", "3x3 (4 doors) window" },
            {"35010", "3x3 (3 doors) window" },
            {"35011", "3x3 3 doors (heigt 2)" },
            {"35012", "3x3 2 doors w. closet" },
        };
       

        public static bool LoadInterior(string path, out BuildingReplacementData buildingReplacement) {

            buildingReplacement = new BuildingReplacementData();

            if (string.IsNullOrEmpty(path)) {
                Debug.LogError("Error: Trying to open file, but path is null or empty");
                return false;
            }

            if (!File.Exists(path)) {
                Debug.LogError("Error: Trying to open file at invalid path: " + path);
                return false;
            }

            StreamReader reader = new StreamReader(path, false);
            buildingReplacement = (BuildingReplacementData)SaveLoadManager.Deserialize(typeof(BuildingReplacementData), reader.ReadToEnd());

            reader.Close();

            return true;
        }
        public static void SaveInterior(BuildingReplacementData interiorData, string path) {

            if (string.IsNullOrEmpty(path)) {
                return;
            }

            StreamWriter writer = new StreamWriter(path, false);
            string saveData = SaveLoadManager.Serialize(interiorData.GetType(), interiorData);
            writer.Write(saveData);
            writer.Close();
        }

        public static GameObject Add3dObject(DaggerfallConnect.DFBlock.RmbBlock3dObjectRecord rmbBlock) {

            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Get model data
            ModelData modelData;
            dfUnity.MeshReader.GetModelData(rmbBlock.ModelIdNum, out modelData);

            // Get model position by type (3 seems to indicate props/clutter)
            // Also stop these from being combined as some may carry a loot container
            Vector3 modelPosition;
            if (rmbBlock.ObjectType == (int)InteractiveObject) {
                // Props axis needs to be transformed to lowest Y point
                Vector3 bottom = modelData.Vertices[0];
                for (int i = 0; i < modelData.Vertices.Length; i++) {
                    if (modelData.Vertices[i].y < bottom.y)
                        bottom = modelData.Vertices[i];
                }
                modelPosition = new Vector3(rmbBlock.XPos, rmbBlock.YPos, rmbBlock.ZPos) * MeshReader.GlobalScale;
                modelPosition += new Vector3(0, -bottom.y, 0);
            }
            else {
                modelPosition = new Vector3(rmbBlock.XPos, -rmbBlock.YPos, rmbBlock.ZPos) * MeshReader.GlobalScale;
            }

            // Get model transform
            Vector3 modelRotation = new Vector3(-rmbBlock.XRotation / DaggerfallConnect.Arena2.BlocksFile.RotationDivisor, -rmbBlock.YRotation / DaggerfallConnect.Arena2.BlocksFile.RotationDivisor, -rmbBlock.ZRotation / DaggerfallConnect.Arena2.BlocksFile.RotationDivisor);                    
            Vector3 modelScale = new Vector3(rmbBlock.XScale, rmbBlock.YScale, rmbBlock.ZScale);
            Matrix4x4 modelMatrix = Matrix4x4.TRS(modelPosition, Quaternion.Euler(modelRotation), modelScale);

            // Inject custom GameObject if available
            GameObject go = MeshReplacement.ImportCustomGameobject(rmbBlock.ModelIdNum, null, modelMatrix);

            // Otherwise use Daggerfall mesh - combine or add
            if (!go) {
                // Add individual GameObject
                go = DaggerfallWorkshop.Utility.GameObjectHelper.CreateDaggerfallMeshGameObject(rmbBlock.ModelIdNum, null);
                go.transform.position = modelMatrix.GetColumn(3);
                go.transform.localScale = modelMatrix.lossyScale;
                go.transform.rotation = modelMatrix.rotation;
            }

            return go;
        }
        public static GameObject AddDoorObject(DaggerfallConnect.DFBlock.RmbBlockDoorRecord rmbBlock) {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;

            // Get model data
            ModelData modelData;
            dfUnity.MeshReader.GetModelData(9000, out modelData);

            Vector3 modelRotation = new Vector3(0, -rmbBlock.YRotation / DaggerfallConnect.Arena2.BlocksFile.RotationDivisor, 0);
            Vector3 modelPosition = new Vector3(rmbBlock.XPos, -rmbBlock.YPos, rmbBlock.ZPos) * MeshReader.GlobalScale;

            //Matrix4x4 modelMatrix = Matrix4x4.TRS(modelPosition, Quaternion.Euler(modelRotation), Vector3.one);

            GameObject go = DaggerfallWorkshop.Utility.GameObjectHelper.CreateDaggerfallMeshGameObject(9000, null);
            go.transform.rotation = Quaternion.Euler(modelRotation);
            go.transform.position = modelPosition;
            return go;
        }
        public static GameObject AddFlatObject(DaggerfallConnect.DFBlock.RmbBlockFlatObjectRecord rmbBlock) {

            Vector3 billboardPosition = new Vector3(rmbBlock.XPos, -rmbBlock.YPos, rmbBlock.ZPos) * MeshReader.GlobalScale;

            // Spawn billboard gameobject
            GameObject go = DaggerfallWorkshop.Utility.GameObjectHelper.CreateDaggerfallBillboardGameObject(rmbBlock.TextureArchive, rmbBlock.TextureRecord, null);

            // Set position
            DaggerfallBillboard dfBillboard = go.GetComponent<DaggerfallBillboard>();
            go.transform.position = billboardPosition;
            go.transform.position += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);

            return go;
        }
        public static GameObject AddPersonObject(DaggerfallConnect.DFBlock.RmbBlockPeopleRecord rmbBlock) {

            Vector3 billboardPosition = new Vector3(rmbBlock.XPos, -rmbBlock.YPos, rmbBlock.ZPos) * MeshReader.GlobalScale;

            // Spawn billboard gameobject
            GameObject go = DaggerfallWorkshop.Utility.GameObjectHelper.CreateDaggerfallBillboardGameObject(rmbBlock.TextureArchive, rmbBlock.TextureRecord, null);

            // Set position
            DaggerfallBillboard dfBillboard = go.GetComponent<DaggerfallBillboard>();
            go.transform.position = billboardPosition;
            go.transform.position += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);

            // Add RMB data to billboard
            dfBillboard.SetRMBPeopleData(rmbBlock);

            return go;
        }
        public static Texture2D CreateColorTexture(int width, int height, Color color) {
            Texture2D colorTexture = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    colorTexture.SetPixel(x, y, color);

            colorTexture.Apply();
            return colorTexture;
        }

        public static void AddLight(Transform parent, DaggerfallConnect.DFBlock.RmbBlockFlatObjectRecord obj)
        {
            if (DaggerfallUnity.Instance.Option_InteriorLightPrefab == null)
                return;

            // Create gameobject
            GameObject go = DaggerfallWorkshop.Utility.GameObjectHelper.InstantiatePrefab(DaggerfallUnity.Instance.Option_InteriorLightPrefab.gameObject, string.Empty, parent, Vector3.zero);

            // Set local position to billboard origin, otherwise light transform is at base of billboard
            go.transform.localPosition = Vector3.zero;

            go.hideFlags = HideFlags.HideInHierarchy;

            // Adjust position of light for standing lights as their source comes more from top than middle
            Vector2 size = DaggerfallUnity.Instance.MeshReader.GetScaledBillboardSize(210, obj.TextureRecord) * MeshReader.GlobalScale;
            switch (obj.TextureRecord)
            {
                case 0:         // Bowl with fire
                    go.transform.localPosition += new Vector3(0, -0.1f, 0);
                    break;
                case 1:         // Campfire
                                // todo
                    break;
                case 2:         // Skull candle
                    go.transform.localPosition += new Vector3(0, 0.1f, 0);
                    break;
                case 3:         // Candle
                    go.transform.localPosition += new Vector3(0, 0.1f, 0);
                    break;
                case 4:         // Candle in bowl
                                // todo
                    break;
                case 5:         // Candleholder with 3 candles
                    go.transform.localPosition += new Vector3(0, 0.15f, 0);
                    break;
                case 6:         // Skull torch
                    go.transform.localPosition += new Vector3(0, 0.6f, 0);
                    break;
                case 7:         // Wooden chandelier with extinguished candles
                                // todo
                    break;
                case 8:         // Turkis lamp
                                // do nothing
                    break;
                case 9:        // Metallic chandelier with burning candles
                    go.transform.localPosition += new Vector3(0, 0.4f, 0);
                    break;
                case 10:         // Metallic chandelier with extinguished candles
                                 // todo
                    break;
                case 11:        // Candle in lamp
                    go.transform.localPosition += new Vector3(0, -0.4f, 0);
                    break;
                case 12:         // Extinguished lamp
                                 // todo
                    break;
                case 13:        // Round lamp (e.g. main lamp in mages guild)
                    go.transform.localPosition += new Vector3(0, -0.35f, 0);
                    break;
                case 14:        // Standing lantern
                    go.transform.localPosition += new Vector3(0, size.y / 2, 0);
                    break;
                case 15:        // Standing lantern round
                    go.transform.localPosition += new Vector3(0, size.y / 2, 0);
                    break;
                case 16:         // Mounted Torch with thin holder
                                 // todo
                    break;
                case 17:        // Mounted torch 1
                    go.transform.localPosition += new Vector3(0, 0.2f, 0);
                    break;
                case 18:         // Mounted Torch 2
                                 // todo
                    break;
                case 19:         // Pillar with firebowl
                                 // todo
                    break;
                case 20:        // Brazier torch
                    go.transform.localPosition += new Vector3(0, 0.6f, 0);
                    break;
                case 21:        // Standing candle
                    go.transform.localPosition += new Vector3(0, size.y / 2.4f, 0);
                    break;
                case 22:         // Round lantern with medium chain
                    go.transform.localPosition += new Vector3(0, -0.5f, 0);
                    break;
                case 23:         // Wooden chandelier with burning candles
                                 // todo
                    break;
                case 24:        // Lantern with long chain
                    go.transform.localPosition += new Vector3(0, -1.85f, 0);
                    break;
                case 25:        // Lantern with medium chain
                    go.transform.localPosition += new Vector3(0, -1.0f, 0);
                    break;
                case 26:        // Lantern with short chain
                                // todo
                    break;
                case 27:        // Lantern with no chain
                    go.transform.localPosition += new Vector3(0, -0.02f, 0);
                    break;
                case 28:        // Street Lantern 1
                                // todo
                    break;
                case 29:        // Street Lantern 2
                                // todo
                    break;
            }

            // adjust properties of light sources (e.g. Shrink light radius of candles)
            Light light = go.GetComponent<Light>();
            switch (obj.TextureRecord)
            {
                case 0:         // Bowl with fire
                    light.range = 20.0f;
                    light.intensity = 1.1f;
                    light.color = new Color(0.95f, 0.91f, 0.63f);
                    break;
                case 1:         // Campfire
                                // todo
                    break;
                case 2:         // Skull candle
                    light.range /= 3f;
                    light.intensity = 0.6f;
                    light.color = new Color(1.0f, 0.99f, 0.82f);
                    break;
                case 3:         // Candle
                    light.range /= 3f;
                    break;
                case 4:         // Candle with base
                    light.range /= 3f;
                    break;
                case 5:         // Candleholder with 3 candles
                    light.range = 7.5f;
                    light.intensity = 0.33f;
                    light.color = new Color(1.0f, 0.89f, 0.61f);
                    break;
                case 6:         // Skull torch
                    light.range = 15.0f;
                    light.intensity = 0.75f;
                    light.color = new Color(1.0f, 0.93f, 0.62f);
                    break;
                case 7:         // Wooden chandelier with extinguished candles
                                // todo
                    break;
                case 8:         // Turkis lamp
                    light.color = new Color(0.68f, 1.0f, 0.94f);
                    break;
                case 9:        // metallic chandelier with burning candles
                    light.range = 15.0f;
                    light.intensity = 0.65f;
                    light.color = new Color(1.0f, 0.92f, 0.6f);
                    break;
                case 10:         // Metallic chandelier with extinguished candles
                                 // todo
                    break;
                case 11:        // Candle in lamp
                    light.range = 5.0f;
                    light.intensity = 0.5f;
                    break;
                case 12:         // Extinguished lamp
                                 // todo
                    break;
                case 13:        // Round lamp (e.g. main lamp in mages guild)
                    light.range *= 1.2f;
                    light.intensity = 1.1f;
                    light.color = new Color(0.93f, 0.84f, 0.49f);
                    break;
                case 14:        // Standing lantern
                                // todo
                    break;
                case 15:        // Standing lantern round
                                // todo
                    break;
                case 16:         // Mounted Torch with thin holder
                                 // todo
                    break;
                case 17:        // Mounted torch 1
                    light.intensity = 0.8f;
                    light.color = new Color(1.0f, 0.97f, 0.87f);
                    break;
                case 18:         // Mounted Torch 2
                                 // todo
                    break;
                case 19:         // Pillar with firebowl
                                 // todo
                    break;
                case 20:        // Brazier torch
                    light.range = 12.0f;
                    light.intensity = 0.75f;
                    light.color = new Color(1.0f, 0.92f, 0.72f);
                    break;
                case 21:        // Standing candle
                    light.range /= 3f;
                    light.intensity = 0.5f;
                    light.color = new Color(1.0f, 0.95f, 0.67f);
                    break;
                case 22:         // Round lantern with medium chain
                    light.intensity = 1.5f;
                    light.color = new Color(1.0f, 0.95f, 0.78f);
                    break;
                case 23:         // Wooden chandelier with burning candles
                                 // todo
                    break;
                case 24:        // Lantern with long chain
                    light.intensity = 1.4f;
                    light.color = new Color(1.0f, 0.98f, 0.64f);
                    break;
                case 25:        // Lantern with medium chain
                    light.intensity = 1.4f;
                    light.color = new Color(1.0f, 0.98f, 0.64f);
                    break;
                case 26:        // Lantern with short chain
                    light.intensity = 1.4f;
                    light.color = new Color(1.0f, 0.98f, 0.64f);
                    break;
                case 27:        // Lantern with no chain
                    light.intensity = 1.4f;
                    light.color = new Color(1.0f, 0.98f, 0.64f);
                    break;
                case 28:        // Street Lantern 1
                                // todo
                    break;
                case 29:        // Street Lantern 2
                                // todo
                    break;
            }

            // TODO: Could also adjust light colour and intensity, or change prefab entirely above for any obj.TextureRecord
        }

    }

}