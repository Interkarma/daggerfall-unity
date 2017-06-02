// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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

        public const string enterSaveName = "Enter save name";
        public const string selectSaveName = "Select a save";
        public const string confirmDeleteSave = "Are you sure you want to delete save?";
        public const string confirmOverwriteSave = "Overwrite this save?";
        public const string youMustEnterASaveName = "You must enter a save name.";
        public const string youMustSelectASaveName = "You must select a save name.";
        public const string noSavesFound = "No saves found. Load a Classic save?";

        public const string theBodyHasNoTreasure = "The body has no treasure.";

        public const string loiterHowManyHours = "Loiter how many hours : ";
        public const string restHowManyHours = "Rest how many hours : ";

        public const string potionRecipeFor = "Recipe for %po";
        public const string cannotUseThis = "You cannot use this.";
        public const string multipleAssignments = "You have multiple assignments...";

        public const string youAreEntering = "You are entering %s";

        public const string interactionIsNowInMode = "Interaction is now in %s mode.";
        public const string steal = "steal";
        public const string grab = "grab";
        public const string info = "info";
        public const string dialogue = "dialogue";
        public const string residence = "Residence";
        public const string youSee = "You see %s.";
        public const string theNamedResidence = "The %s Residence";

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

        public const string skillImprove = "Your %s skill has improved.";
        public const string mustDistributeBonusPoints = "You must distribute all bonus points.";
    }
}