// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:
//
// Notes:
//

using System;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// These strings are hard-coded into FALL.EXE.
    /// They are replicated here temporarily during development.
    /// This will eventually be moved to an improved text handler so strings are never hard-coded.
    /// </summary>
    public class HardStrings
    {
        public const string pleaseSelectYourHomeProvince = "Please select your home province...";
        public const string saveGame = "Save Game";
        public const string loadGame = "Load Game";
        public const string thingJustDied = "%s just died.";
        public const string gameSaved = "Game saved.";
        public const string gameLoaded = "Game loaded.";
        public const string thisHouseHasNothingOfValue = "This house has nothing of value.";
        public const string findLocationPrompt = "Enter name of place : ";
        public const string rightHandEquipped = "Right hand equipped.";
        public const string leftHandEquipped = "Left hand equipped.";
        public const string usingRightHand = "Using weapon in right hand.";
        public const string usingLeftHand = "Using weapon in left hand.";
        public const string itemHasBroken = "%s has broken.";
        public const string itemHasBrokenPlural = "%s have broken.";
        public const string cannotCarryGold = "You cannot carry that much gold.";
        public const string cannotFloat = "You are carrying too much to stay afloat.";
        public const string youHaveNoArrows = "You have no arrows.";

        public const string enterSaveName = "Enter save name";
        public const string selectSaveName = "Select a save";
        public const string confirmDeleteSave = "Are you sure you want to delete save?";
        public const string confirmOverwriteSave = "Overwrite this save?";
        public const string youMustEnterASaveName = "You must enter a save name.";
        public const string youMustSelectASaveName = "You must select a save name.";
        public const string noSavesFound = "No saves found. Load a Classic save?";
        public const string oldSaveNoTrade = "Old indoor save loaded, trade window will not work please exit and re-enter building.";

        public const string theBodyHasNoTreasure = "The body has no treasure.";
        public const string youAreTooFarAway = "You are too far away...";
        public const string youSeeAn = "You see an %s.";
        public const string youSeeA = "You see a %s.";
        public const string youSeeADeadPerson = "You see a dead person.";
        public const string youSeeADead = "You see a dead %s.";
        public const string youCollectArrows = "You pluck your arrows out of the corpse.";

        public const string loiterHowManyHours = "Loiter how many hours : ";
        public const string restHowManyHours = "Rest how many hours : ";

        public const string potionRecipeFor = "Recipe for Potion of %po";
        public const string potionOf = "Potion of %po";
        public const string letterPrefix = "Letter: ";
        public const string multipleAssignments = "You have multiple assignments...";

        public const string youAreEntering = "You are entering %s";

        public const string storeClosed = "Store is closed. Open from %d1:00 to %d2:00.";

        public const string any = "Any %s";
        public static readonly string[] buildingNames = { "Temple of Akatosh", "Temple of Arkay", "Temple of Dibella",
                                                "Temple of Julianos", "Temple of Kynareth", "Temple of Mara",
                                                "Temple of Stendarr", "Temple of Zen", "Order of the Raven",
                                                "Knights of the Dragon", "Knights of the Owl", "Order of the Candle",
                                                "Knights of the Flame", "Host of the Horn", "Knights of the Rose",
                                                "Knights of the Wheel", "Order of the Scarab", "Knights of the Hawk",
                                                "Mages Guild", "Fighters Guild", "Tavern", "Library", "Weapon Smith",
                                                "Armorer", "Alchemist", "Bank", "Bookstore", "Clothing store", "Gem store" };
        // TODO: These temple names are not consistent with 4055-4062!

        public const string arkayDesc = "God of Birth and Death";
        public const string zenDesc = "God of Work and Commerce";
        public const string maraDesc = "Goddess of Love";
        public const string akatoshDesc = "God of Time";
        public const string julianosDesc = "God of Logic";
        public const string dibellaDesc = "Goddess of Beauty";
        public const string stendarDesc = "God of Mercy";
        public const string kynarethDesc = "Goddess of Air";

        public const string interactionIsNowInMode = "Interaction is now in %s mode.";
        public const string steal = "steal";
        public const string grab = "grab";
        public const string info = "info";
        public const string dialogue = "dialogue";
        public const string residence = "Residence";
        public const string youSee = "You see %s.";
        public const string theNamedResidence = "The %s Residence";
        public const string playerResidence = "%s's house";

        public const string materialIneffective = "The material of the weapon you are using is ineffective.";
        public const string successfulBackstab = "Successful backstab!";

        public const string languagePacified = "Pacified %e using %s skill."; // Not in classic.

        public const string lockpickingSuccess = "The lock clicks open.";
        public const string lockpickingFailure = "It does not unlock.";
        public const string magicLock = "This is a magically held lock...";
        public const string lockpickChance1 = "This lock has nothing to fear from you...";
        public const string lockpickChance2 = "It'd be a miracle if you picked this lock...";
        public const string lockpickChance3 = "This lock looks to be beyond your skills...";
        public static readonly string[] lockpickChance =  {"You doubt your ability to open this lock...",
                                                    "This lock looks difficult...",
                                                    "You would be challenged by this lock...",
                                                    "This lock would prove a good challenge...",
                                                    "You think you should be able to pick this lock...",
                                                    "This lock seems relatively easy...",
                                                    "You are amused by this lock...",
                                                    "You laugh at the amateur quality of this lock...",
                                                    "You see a pathetic excuse for a lock...",
                                                    "This lock is an insult to your abilities..."};

        public const string youPinchedGoldPiece = "You pinched 1 gold piece."; // Not in classic.
        public const string youPinchedGoldPieces = "You pinched %d gold pieces.";
        public const string youAreNotSuccessful = "You are not successful...";

        public const string daysUntilFreedom = "%d days until freedom.";

        public const string skillImprove = "Your %s skill has improved.";
        public const string mustDistributeBonusPoints = "You must distribute all bonus points.";
        public const string enterNewName = "Enter new name : ";
        public const string affiliation = "Affiliation";
        public const string rank = "Rank";

        public const string serviceQuests = "Get Quest";
        public const string serviceSell = "Sell";
        public const string serviceBanking = "Banking";
        public const string serviceIdentify = "Identify";
        public const string serviceBuySpells = "Buy Spells";
        public const string serviceTraining = "Training";
        public const string serviceTeleport = "Teleportation";
        public const string serviceMakeSpells = "Make Spells";
        public const string serviceBuyMagicItems = "Buy Magic Items";
        public const string serviceSellMagicItems = "Sell Magic Items";
        public const string serviceDaedraSummon = "Daedra Summoning";
        public const string serviceMakeMagicItems = "Make Magic Items";
        public const string serviceRepairs = "Repairs";
        public const string serviceDonate = "Make Donation";
        public const string serviceCure = "Cure Disease";
        public const string serviceSpymaster = "Spymaster";
        public const string serviceMakePotions = "Make Potions";
        public const string serviceBuyPotions = "Buy Potions";
        public const string serviceBuySoulgems = "Buy Soulgems";
        public const string serviceReceiveArmor = "Receive Armor";
        public const string serviceReceiveHouse = "Receive House";
        public const string serviceReceiveHouseAlready = "You have already received your house.";
        public const string serviceMembersOnly = "My services are reserved for members only.";
        public const string accessMembersOnly = "You need to be a member of sufficient rank to access this.";
        public const string serviceDonateHowMuch = "Donate how much money : ";
        public const string serviceCured = "You are cured.";
        public const string serviceSummonCost1 = "%pcn, the cost for me to attempt a";
        public const string serviceSummonCost2 = "summoning of %dae is ";
        public const string serviceSummonCost3 = " gold.";

        public const string roomFreeForKnightSuchAsYou = "The room is free for a knight such as you.";
        public const string roomFreeDueToHeartsDay = "Room is free due to Heart's Day.";
        public const string youAreNotHungry = "You are not hungry.";

        public const string exhaustedInWater = "Fatigue overcomes you and sends you to a watery grave...."; // Not in classic. Borrowed from Arena.
        public const string youFeelSomewhatBad = "You feel somewhat bad.";
        public const string avoidDeath = "By the mercy of Stendarr, you survive certain death!";

        // Words used by macro handlers:

        public const string pronounHe = "he";
        public const string pronounShe = "she";
        public const string pronounHim = "him";
        public const string pronounHer = "her";
        public const string pronounHis = "his";
        public const string pronounHers = "hers";
        public const string pronounHimself = "himself";
        public const string pronounHerself = "herself";

        public const string city = "city";
        public const string village = "village";
        public const string hamlet = "hamlet";
        public const string farm = "farm";
        public const string shack = "shack";
        public const string manor = "manor";
        public const string community = "community";
        public const string shrine = "shrine";

        public const string King = "King";
        public const string Queen = "Queen";
        public const string Duke = "Duke";
        public const string Duchess = "Duchess";
        public const string Marquis = "Marquis";
        public const string Marquise = "Marquise";
        public const string Count = "Count";
        public const string Countess = "Countess";
        public const string Baron = "Baron";
        public const string Baroness = "Baroness";
        public const string Lord = "Lord";
        public const string Lady = "Lady";

        public const string revered = "revered";
        public const string esteemed = "esteemed";
        public const string honored = "honored";
        public const string admired = "admired";
        public const string respected = "respected";
        public const string dependable = "dependable";
        public const string aCommonCitizen = "a common citizen";
        public const string hated = "hated";
        public const string pondScum = "pond scum";
        public const string aVillain = "a villain";
        public const string aCriminal = "a criminal";
        public const string aScoundrel = "a scoundrel";
        public const string undependable = "undependable";
        public const string unknown = "unknown";

        public const string Broken = "Broken";
        public const string Useless = "Useless";
        public const string Battered = "Battered";
        public const string Worn = "Worn";
        public const string Used = "Used";
        public const string SlightlyUsed = "Slightly Used";
        public const string AlmostNew = "Almost New";
        public const string New = "New";
        public const string Nothing = "Nothing";

        public const string local = "local";
        public const string tavern = "tavern";

        public const string Attempted_Breaking_And_Entering = "Attempted Breaking and Entering";
        public const string Trespassing = "Trespassing";
        public const string Breaking_And_Entering = "Breaking and Entering";
        public const string Assault = "Assault";
        public const string Murder = "Murder";
        public const string Tax_Evasion = "Tax Evasion";
        public const string Criminal_Conspiracy = "Criminal Conspiracy";
        public const string Vagrancy = "Vagrancy";
        public const string Smuggling = "Smuggling";
        public const string Piracy = "Piracy";
        public const string High_Treason = "High Treason";
        public const string Pickpocketing = "Pickpocketing";
        public const string Theft = "Theft";
        public const string Treason = "Treason";

        public const string Regular_Punishment_String = "%gtp gold pieces in fines and %dip days in prison";
        public const string Banishment = "Banishment";
        public const string Execution = "Execution";

        public const string cannotChangeTransportationIndoors = "You cannot change transportation indoors.";
        public const string cannotTravelWithEnemiesNearby = "You cannot travel with enemies nearby.";
        public const string cannotTravelIndoors = "You cannot travel while indoors.";

        public const string powersUnknown = "Powers unknown.";

        public static readonly string[] itemPowers = { "Cast when used:",
                                                    "Cast when held:",
                                                    "Cast when strikes:",
                                                    "Extra spell pts",
                                                    "Potent vs",
                                                    "Regens health",
                                                    "Vampiric effect",
                                                    "Increased weight allowance",
                                                    "Repairs objects",
                                                    "Absorbs spells",
                                                    "Enhances skill",
                                                    "Feather weight",
                                                    "Strengthens armor",
                                                    "Improves talents",
                                                    "Good rep with",
                                                    "Soul bound",
                                                    "Item deteriorates",
                                                    "User takes damage",
                                                    "Vision problems",
                                                    "Walking problems",
                                                    "Low damage vs",
                                                    "Health leech",
                                                    "Bad reactions from",
                                                    "Extra weight",
                                                    "Weakens armor",
                                                    "Bad rep with" };

        public static readonly string[] enemyNames =  { "Rat", "Imp", "Spriggan",
                                                       "Giant bat", "Grizzly bear",
                                                       "Sabretooth tiger", "Spider",
                                                       "Orc", "Centaur", "Werewolf",
                                                       "Nymph", "Slaughterfish",
                                                       "Orc sergeant", "Harpy", "Wereboar",
                                                       "Skeletal warrior", "Giant",
                                                       "Zombie", "Ghost", "Mummy",
                                                       "Giant scorpion", "Orc shaman",
                                                       "Gargoyle", "Wraith",
                                                       "Orc warlord", "Frost daedra",
                                                       "Fire daedra", "Daedroth",
                                                       "Vampire", "Daedra seducer",
                                                       "Ancient vampire", "Daedra lord",
                                                       "Lich", "Ancient lich", "Dragonling",
                                                       "Fire Atronach", "Iron Atronach",
                                                       "Flesh Atronach", "Ice Atronach",
                                                       "Horse", "Dragonling", "Dreugh",
                                                       "Lamia" };

        public static readonly string[] extraSpellPtsTimes = { "during Winter", "during Spring", "during Summer", "during Fall", "during Full Moon", "during Half Moon", "during New Moon", "near undead", "near daedra", "near humanoids", "near animals" };
        public static readonly string[] regensHealthTimes = { "all the time", "in sunlight", "in darkness" };
        public static readonly string[] vampiricEffectRanges = { "at range", "when strikes" };
        public static readonly string[] increasedWeightAllowances = { "25% additional", "50% additional" };
        public static readonly string[] improvedTalents = { "hearing", "athleticism", "adrenaline rush" };
        public static readonly string[] itemDeteriorateLocations = { "all the time", "in sunlight", "in holy places" };
        public static readonly string[] userTakesDamageLocations = { "in sunlight", "in holy places" };
        public static readonly string[] enemyGroupNames =  { "undead", "Daedra", "humanoids", "animals" };
        public static readonly string[] healthLeechStopConditions = { "whenever used", "unless used daily", "unless used weekly" };
        public static readonly string[] badReactionFromEnemyGroups = { "humanoids", "animals", "Daedra" };
        public static readonly string[] repWithGroups = { "Commoners", "Merchants", "Scholars", "Nobility", "Underworld", "All" };

        public const string youReceiveGoldPieces = "You receive %s gold pieces.";     // Custom message for "get item" gold assignment

        public const string bankPurchasePrice = "Price : %s gold";

        public const string youHaveRentedRoom = "You have a room at %s for %d hours.";
        public const string haveNotRentedRoom = "You have not rented a room here.";
        public const string expiredRentedRoom = "Your time for this room has expired.";
        public const string tavernAle = "Ale (1 gold)";
        public const string tavernBeer = "Beer (1 gold)";
        public const string tavernMead = "Mead (2 gold)";
        public const string tavernWine = "Wine (3 gold)";
        public const string tavernBread = "Bread (1 gold)";
        public const string tavernBroth = "Broth (1 gold)";
        public const string tavernCheese = "Cheese (2 gold)";
        public const string tavernFowl = "Fowl (3 gold)";
        public const string tavernGruel = "Gruel (2 gold)";
        public const string tavernPie = "Pie (2 gold)";
        public const string tavernStew = "Stew (3 gold)";


        public const string pressButtonToFireSpell = "Press button to fire spell.";

        public const string climbingMode = "Climbing mode.";
        public const string rappelMode = "Rappel mode.";
        public const string hangingMode = "Hanging Mode";

        public const string acuteHearing = "Acute Hearing";
        public const string adrenalineRush = "Adrenaline Rush";
        public const string athleticism = "Athleticism";
        public const string bonusToHit = "Bonus to hit";
        public const string expertiseIn = "Expertise in";
        public const string immunity = "Immunity";
        public const string increasedMagery = "Increased Magery";
        public const string rapidHealing = "Rapid Healing";
        public const string regenerateHealth = "Regenerate Health";
        public const string resistance = "Resistance";
        public const string spellAbsorption = "Spell Absorption";
        public const string animals = "Animals";
        public const string daedra = "Daedra";
        public const string humanoid = "Humanoid";
        public const string undead = "Undead";
        public const string axe = "Axe";
        public const string bluntWeapon = "Blunt Weapon";
        public const string handToHand = "Hand_To_Hand";
        public const string longBlade = "Long Blade";
        public const string missileWeapon = "Missile Weapon";
        public const string shortBlade = "Short Blade";
        public const string toDisease = "To Disease";
        public const string toFire = "To Fire";
        public const string toFrost = "To Frost";
        public const string toMagic = "To Magic";
        public const string toParalysis = "To Paralysis";
        public const string toPoison = "To Poison";
        public const string toShock = "To Shock";
        public const string intInSpellPoints15 = "1.5X INT In Spell Points";
        public const string intInSpellPoints175 = "1.75X INT In Spell Points";
        public const string intInSpellPoints2 = "2X INT In Spell Points";
        public const string intInSpellPoints3 = "3X INT In Spell Points";
        public const string intInSpellPoints = "INT In Spell Points";
        public const string general = "General";
        public const string inDarkness = "In Darkness";
        public const string inLight = "In Light";
        public const string whileImmersed = "While Immersed In Water";

        public const string criticalWeakness = "Critical Weakness";
        public const string damage = "Damage";
        public const string darknessPoweredMagery = "Darkness_Powered Magery";
        public const string forbiddenArmorType = "Forbidden Armor Type";
        public const string forbiddenMaterial = "Forbidden Material";
        public const string forbiddenShieldTypes = "Forbidden Shield Types";
        public const string forbiddenWeaponry = "Forbidden Weaponry";
        public const string inabilityToRegen = "Inability To Regen Spell Points";
        public const string lightPoweredMagery = "Light_Powered Magery";
        public const string lowTolerance = "Low Tolerance";
        public const string phobia = "Phobia";
        public const string fromHolyPlaces = "From Holy Places";
        public const string fromSunlight = "From Sunlight";
        public const string lowerMagicAbilityDaylight = "Lower Magic Ability In Daylight";
        public const string unableToUseMagicInDaylight = "Unable To Use Magic In Daylight";
        public const string chain = "Chain";
        public const string leather = "Leather";
        public const string plate = "Plate";
        public const string adamantium = "Adamantium";
        public const string daedric = "Daedric";
        public const string dwarven = "Dwarven";
        public const string ebony = "Ebony";
        public const string elven = "Elven";
        public const string iron = "Iron";
        public const string mithril = "Mithril";
        public const string orcish = "Orcish";
        public const string silver = "Silver";
        public const string steel = "Steel";
        public const string buckler = "Buckler";
        public const string kiteShield = "Kite Shield";
        public const string roundShield = "Round Shield";
        public const string towerShield = "Tower Shield";
        public const string lowerMagicAbilityDarkness = "Lower Magic Ability In Darkness";
        public const string unableToUseMagicInDarkness = "Unable To Use Magic In Darkness";
        public const string helpAttributes = "Attributes";
        public const string helpClassName = "Class Name";
        public const string helpGeneral = "General";
        public const string helpReputations = "Reputations";
        public const string helpSkillAdvancement = "Skill Advancement";
        public const string helpSkills = "Skills";
        public const string helpSpecialAdvantages = "Special Advantages";
        public const string helpSpecialDisadvantages = "Special Disadvantages";
        public const string lower = "Lower";
        public const string higher = "Higher";
        public const string unchanged = "Unchanged";
        public const string elsweyr = "Elsweyr";
        public const string blackMarsh = "Black Marsh";
        public const string hammerfell = "Hammerfell";
        public const string highRock = "High Rock";
        public const string morrowind = "Morrowind";
        public const string skyrim = "Skyrim";
        public const string sumurset = "Sumurset";
        public const string valenwood = "Valenwood";
        public const string rollingHills = "rolling hills";
        public const string desertLand = "desertland";
        public const string mountains = "mountains";
        public const string swamps = "swamps";
        public const string forests = "forests";
        public const string shores = "shores";
    }
}
