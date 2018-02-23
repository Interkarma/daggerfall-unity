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
        public const string equippingWeapon = "Equipping %s";
        public const string rightHandEquipped = "Right hand equipped.";
        public const string leftHandEquipped = "Left hand equipped.";
        public const string usingRightHand = "Using weapon in right hand.";
        public const string usingLeftHand = "Using weapon in left hand.";
        public const string itemHasBroken = "%s has broken.";
        public const string itemHasBrokenPlural = "%s have broken.";
        public const string cannotCarryAnymore = "You cannot carry anymore stuff.";
        public const string cannotHoldAnymore = "Your cart cannot hold anymore stuff.";
        public const string cannotCarryGold = "You cannot carry that much gold.";
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

        public const string loiterHowManyHours = "Loiter how many hours : ";
        public const string restHowManyHours = "Rest how many hours : ";

        public const string potionRecipeFor = "Recipe for Potion of %po";
        public const string potionOf = "Potion of %po";
        public const string cannotUseThis = "You cannot use this.";
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
        public const string languagePacified = "Pacified %e using %s skill.";

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

        public const string skillImprove = "Your %s skill has improved.";
        public const string mustDistributeBonusPoints = "You must distribute all bonus points.";

        public const string serviceQuests = "Get Quest";
        public const string serviceSell = "Sell";
        public const string serviceBanking = "Banking";
        public const string serviceIdentify = "Identify";
        public const string serviceBuySpells = "Buy Spells";
        public const string serviceTraining = "Training";
        public const string serviceTeleport = "Teleportation";
        public const string serviceMakeSpells = "Make Spells";
        public const string serviceBuyMagicItems = "Buy Magic Items";
        public const string serviceSellMagicItems = "Buy Magic Items";
        public const string serviceDaedraSummon = "Daedra Summoning";
        public const string serviceMakeMagicItems = "Make Magic Items";
        public const string serviceRepairs = "Repairs";
        public const string serviceDonate = "Make Donation";
        public const string serviceCure = "Cure Disease";
        public const string serviceSpymaster = "Spymaster";
        public const string serviceMakePotions = "Make Potions";
        public const string serviceBuyPotions = "Buy Potions";
        public const string serviceBuySoulgems = "Buy Soulgems";
        public const string serviceMembersOnly = "My services are reserved for members only.";
        public const string accessMembersOnly = "You need to be a member to access this.";

        public const string roomFreeForKnightSuchAsYou = "The room is free for a knight such as you.";
        public const string roomFreeDueToHeartsDay = "Room is free due to Heart's Day.";
        public const string youAreNotHungry = "You are not hungry.";

        public const string doesntNeedIdentifying = "This does not need to be identified.";


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



        public const string cannotChangeTransportationIndoors = "You cannot change transportation indoors.";
        public const string cannotTravelWithEnemiesNearby = "You cannot travel with enemies nearby.";

        public const string powersUnknown = "Powers unknown.";
        public const string repairDone = "DONE";
        public const string repairDays = "%d days";

        public const string cannotRemoveThisItem = "You cannot remove this item.";

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
    }
}
