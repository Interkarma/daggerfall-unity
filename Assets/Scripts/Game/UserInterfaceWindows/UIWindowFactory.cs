// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 
// Notes:
//

using DaggerfallWorkshop.Game.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public enum UIWindowType
    {
        Automap,
        Banking,
        BankPurchasePopup,
        BookReader,
        CharacterSheet,
        Controls,
        Court,
        DaedraSummoned,
        EffectSettingsEditor,
        ExteriorAutomap,
        GuildServicePopup,
        GuildServiceCureDisease = 39,   // Ensure existing values remain 
        GuildServiceDonation = 40,      //  unchanged so as not to break 
        GuildServiceTraining = 41,      //  existing pre-compiled mods.
        Inventory = 11,
        ItemMaker,
        JoystickControls,
        LoadClassicGame,
        MerchantRepairPopup,
        MerchantServicePopup,
        PauseOptions,
        PlayerHistory,
        PotionMaker,
        QuestJournal,
        QuestOffer,
        Rest,
        SpellBook,
        SpellIconPicker = 42, // Ensure existing values remain unchanged so as not to break existing pre-compiled mods.
        SpellMaker = 24,
        StartNewGameWizard,
        Start,
        Talk,
        Tavern,
        TeleportPopUp,
        Trade,
        Transport,
        TravelMap,
        TravelPopUp,
        UnityMouseControls,
        UnitySaveGame,
        UseMagicItem,
        VidPlayer,
        WitchesCovenPopup,
    }

    public static class UIWindowFactory
    {
        static Dictionary<UIWindowType, Type> uiWindowImplementations = new Dictionary<UIWindowType, Type>()
        {
            { UIWindowType.Automap, typeof(DaggerfallAutomapWindow) },
            { UIWindowType.Banking, typeof(DaggerfallBankingWindow) },
            { UIWindowType.BankPurchasePopup, typeof(DaggerfallBankPurchasePopUp) },
            { UIWindowType.BookReader, typeof(DaggerfallBookReaderWindow) },
            { UIWindowType.CharacterSheet, typeof(DaggerfallCharacterSheetWindow) },
            { UIWindowType.Controls, typeof(DaggerfallControlsWindow) },
            { UIWindowType.Court, typeof(DaggerfallCourtWindow) },
            { UIWindowType.DaedraSummoned, typeof(DaggerfallDaedraSummonedWindow) },
            { UIWindowType.EffectSettingsEditor, typeof(DaggerfallEffectSettingsEditorWindow) },
            { UIWindowType.ExteriorAutomap, typeof(DaggerfallExteriorAutomapWindow) },
            { UIWindowType.GuildServicePopup, typeof(DaggerfallGuildServicePopupWindow) },
            { UIWindowType.GuildServiceCureDisease, typeof(DaggerfallGuildServiceCureDisease) },
            { UIWindowType.GuildServiceDonation, typeof(DaggerfallGuildServiceDonation) },
            { UIWindowType.GuildServiceTraining, typeof(DaggerfallGuildServiceTraining) },
            { UIWindowType.Inventory, typeof(DaggerfallInventoryWindow) },
            { UIWindowType.ItemMaker, typeof(DaggerfallItemMakerWindow) },
            { UIWindowType.JoystickControls, typeof(DaggerfallJoystickControlsWindow) },
            { UIWindowType.LoadClassicGame, typeof(DaggerfallLoadClassicGameWindow) },
            { UIWindowType.MerchantRepairPopup, typeof(DaggerfallMerchantRepairPopupWindow) },
            { UIWindowType.MerchantServicePopup, typeof(DaggerfallMerchantServicePopupWindow) },
            { UIWindowType.PauseOptions, typeof(DaggerfallPauseOptionsWindow) },
            { UIWindowType.PlayerHistory, typeof(DaggerfallPlayerHistoryWindow) },
            { UIWindowType.PotionMaker, typeof(DaggerfallPotionMakerWindow) },
            { UIWindowType.QuestJournal, typeof(DaggerfallQuestJournalWindow) },
            { UIWindowType.QuestOffer, typeof(DaggerfallQuestOfferWindow) },
            { UIWindowType.Rest, typeof(DaggerfallRestWindow) },
            { UIWindowType.SpellBook, typeof(DaggerfallSpellBookWindow) },
            { UIWindowType.SpellIconPicker, typeof(SpellIconPickerWindow) },
            { UIWindowType.SpellMaker, typeof(DaggerfallSpellMakerWindow) },
            { UIWindowType.StartNewGameWizard, typeof(StartNewGameWizard) },
            { UIWindowType.Start, typeof(DaggerfallStartWindow) },
            { UIWindowType.Talk, typeof(DaggerfallTalkWindow) },
            { UIWindowType.Tavern, typeof(DaggerfallTavernWindow) },
            { UIWindowType.TeleportPopUp, typeof(DaggerfallTeleportPopUp) },
            { UIWindowType.Trade, typeof(DaggerfallTradeWindow) },
            { UIWindowType.Transport, typeof(DaggerfallTransportWindow) },
            { UIWindowType.TravelMap, typeof(DaggerfallTravelMapWindow) },
            { UIWindowType.TravelPopUp, typeof(DaggerfallTravelPopUp) },
            { UIWindowType.UnityMouseControls, typeof(DaggerfallUnityMouseControlsWindow) },
            { UIWindowType.UnitySaveGame, typeof(DaggerfallUnitySaveGameWindow) },
            { UIWindowType.UseMagicItem, typeof(DaggerfallUseMagicItemWindow) },
            { UIWindowType.VidPlayer, typeof(DaggerfallVidPlayerWindow) },
            { UIWindowType.WitchesCovenPopup, typeof(DaggerfallWitchesCovenPopupWindow) },
        };

        /// <summary>
        /// Register a custom UI Window implementation class. Overwrites the previous class type.
        /// </summary>
        /// <param name="windowType">The type of ui window to be replaced</param>
        /// <param name="windowClassType">The c# class Type of the implementation class to replace with</param>
        public static void RegisterCustomUIWindow(UIWindowType windowType, Type windowClassType)
        {
            DaggerfallUnity.LogMessage("RegisterCustomUIWindow: " + windowType, true);
            uiWindowImplementations[windowType] = windowClassType;
            DaggerfallUI.Instance.ReinstantiatePersistentWindowInstances();
        }

        public static IUserInterfaceWindow GetInstance(UIWindowType windowType, IUserInterfaceManager uiManager)
        {
            object[] args = new object[] { uiManager };
            return GetInstance(windowType, args);
        }

        public static IUserInterfaceWindow GetInstance(UIWindowType windowType, IUserInterfaceManager uiManager, DaggerfallBaseWindow previous)
        {
            object[] args = new object[] { uiManager, previous };
            return GetInstance(windowType, args);
        }

        public static IUserInterfaceWindow GetInstanceWithArgs(UIWindowType windowType, object[] args)
        {

            return GetInstance(windowType, args);
        }

        public static UIWindowType? GetWindowType(Type type)
        {
            var pair = uiWindowImplementations.FirstOrDefault(x => x.Value == type);
            if (pair.Value != null)
                return pair.Key;

            return null;
        }

        private static IUserInterfaceWindow GetInstance(UIWindowType windowType, object[] args)
        {
            Type windowClassType;
            if (uiWindowImplementations.TryGetValue(windowType, out windowClassType))
            {
                return (IUserInterfaceWindow)Activator.CreateInstance(windowClassType, args);
            }
            return null;
        }
    }
}

