using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public static class DaggerfallShortcut
    {
        const string textDatabase = "DialogShortcuts";

        public enum Buttons
        {
            None,

            Accept,
            Reject,
            Cancel,
            Yes,
            No,
            OK,
            Male,
            Female,
            Add,
            Delete,
            Edit,
            Copy,
            Guilty,
            NotGuilty,
            Debate,
            Lie,
            Anchor,
            Teleport,

            // Game Setup menu
            GameSetupPlay,
            GameSetupAdvancedSettings,
            GameSetupMods,
            GameSetupClose,
            GameSetupBackToOptions,
            GameSetupRestart,
            GameSetupRefresh,
            GameSetupSaveAndClose,
            GameSetupExit,

            // Main menu
            MainMenuLoad,
            MainMenuStart,
            MainMenuExit,

            // Class Creation Menu
            ResetBonusPool,

            // Options menu
            OptionsExit,
            OptionsContinue,
            OptionsSave,
            OptionsLoad,
            OptionsControls,
            OptionsFullScreen,
            OptionsHeadBobbing,
            OptionsDropdown,

            // General
            Pause,
            LargeHUDToggle,
            HUDToggle,
            ToggleRetroPP,

            // Debugger
            DebuggerToggle,
            DebuggerPrevQuest,
            DebuggerNextQuest,
            DebuggerPrevMarker,
            DebuggerNextMarker,

            // Rest menu
            RestForAWhile,
            RestUntilHealed,
            RestLoiter,
            RestStop,

            // Transport menu
            TransportFoot,
            TransportHorse,
            TransportCart,
            TransportShip,
            TransportExit,

            // Travel Map screen
            TravelMapFind,
            TravelMapList,

            // Talk screen
            TalkTellMeAbout,
            TalkWhereIs,
            TalkCategoryLocation,
            TalkCategoryPeople,
            TalkCategoryThings,
            TalkCategoryWork,
            TalkAsk,
            TalkExit,
            TalkCopy,
            TalkTonePolite,
            TalkToneNormal,
            TalkToneBlunt,

            // Spellbook screen
            SpellbookDelete,
            SpellbookUp,
            SpellbookSort,
            SpellbookDown,
            SpellbookBuy,
            SpellbookExit,

            // Travel menu
            TravelBegin,
            TravelExit,
            TravelSpeedToggle,
            TravelTransportModeToggle,
            TravelInnCampOutToggle,

            // Charactersheet Screen
            CharacterSheetName,
            CharacterSheetLevel,
            CharacterSheetGold,
            CharacterSheetHealth,
            CharacterSheetAffiliations,
            CharacterSheetPrimarySkills,
            CharacterSheetMajorSkills,
            CharacterSheetMinorSkills,
            CharacterSheetMiscSkills,
            CharacterSheetInventory,
            CharacterSheetSpellbook,
            CharacterSheetLogbook,
            CharacterSheetHistory,
            CharacterSheetExit,

            // Player History screen
            HistoryNextPage,
            HistoryPreviousPage,
            HistoryExit,

            // Quest Journal Screen
            JournalNextCategory,
            JournalNextPage,
            JournalPreviousPage,
            JournalExit,

            // Inventory screen
            InventoryWeapons,
            InventoryMagic,
            InventoryClothing,
            InventoryIngredients,
            InventoryWagon,
            InventoryInfo,
            InventoryEquip,
            InventoryRemove,
            InventoryUse,
            InventoryGold,
            InventoryExit,

            // Merchant menu
            MerchantRepair,
            MerchantTalk,
            MerchantSell,
            MerchantExit,

            // Trade screen
            TradeWagon,
            TradeInfo,
            TradeSelect,
            TradeSteal,
            TradeBuy,
            TradeIdentify,
            TradeRepair,
            TradeSell,
            TradeClear,
            TradeExit,

            // Taverns menu
            TavernRoom,
            TavernTalk,
            TavernFood,
            TavernExit,

            // Guilds
            GuildsJoin,
            GuildsTalk,
            GuildsExit,
            GuildsTraining,
            GuildsGetQuest,
            GuildsRepair,
            GuildsIdentify,
            GuildsDonate,
            GuildsCure,
            GuildsBuyPotions,
            GuildsMakePotions,
            GuildsBuySpells,
            GuildsMakeSpells,
            GuildsBuyMagicItems,
            GuildsMakeMagicItems,
            GuildsSellMagicItems,
            GuildsTeleport,
            GuildsDaedraSummon,
            GuildsSpymaster,
            GuildsBuySoulgems,
            GuildsReceiveArmor,
            GuildsReceiveHouse,

            // Witches Covens
            WitchesTalk,
            WitchesDaedraSummon,
            WitchesQuest,
            WitchesExit,

            // Spellmaker screen
            SpellMakerAddEffect,
            SpellMakerBuySpell,
            SpellMakerNewSpell,
            SpellMakerExit,
            SpellMakerNameSpell,
            SpellMakerTargetCaster,
            SpellMakerTargetTouch,
            SpellMakerTargetSingleAtRange,
            SpellMakerTargetAroundCaster,
            SpellMakerTargetAreaAtRange,
            SpellMakerElementFire,
            SpellMakerElementCold,
            SpellMakerElementPoison,
            SpellMakerElementShock,
            SpellMakerElementMagic,
            SpellMakerNextIcon,
            SpellMakerPrevIcon,
            SpellMakerSelectIcon,

            // Automap screen
            AutomapSwitchAutomapGridMode,
            AutomapResetView,
            AutomapResetRotationPivotAxisView,
            AutomapSwitchFocusToNextBeaconObject,
            AutomapSwitchToNextAutomapRenderMode,
            AutomapSwitchToAutomapRenderModeCutout,
            AutomapSwitchToAutomapRenderModeWireframe,
            AutomapSwitchToAutomapRenderModeTransparent,
            AutomapSwitchToAutomapBackgroundOriginal,
            AutomapSwitchToAutomapBackgroundAlternative1,
            AutomapSwitchToAutomapBackgroundAlternative2,
            AutomapSwitchToAutomapBackgroundAlternative3,
            AutomapMoveLeft,
            AutomapMoveRight,
            AutomapMoveForward,
            AutomapMoveBackward,
            AutomapMoveRotationPivotAxisLeft,
            AutomapMoveRotationPivotAxisRight,
            AutomapMoveRotationPivotAxisForward,
            AutomapMoveRotationPivotAxisBackward,
            AutomapRotateLeft,
            AutomapRotateRight,
            AutomapRotateCameraLeft,
            AutomapRotateCameraRight,
            AutomapRotateCameraOnCameraYZplaneAroundObjectUp,
            AutomapRotateCameraOnCameraYZplaneAroundObjectDown,
            AutomapUpstairs,
            AutomapDownstairs,
            AutomapIncreaseSliceLevel,
            AutomapDecreaseSliceLevel,
            AutomapZoomIn,
            AutomapZoomOut,
            AutomapIncreaseCameraFieldOfFiew,
            AutomapDecreaseCameraFieldOfFiew,

            // Exterior automap screen
            ExtAutomapFocusPlayerPosition,
            ExtAutomapResetView,
            ExtAutomapSwitchToNextExteriorAutomapViewMode,
            ExtAutomapSwitchToExteriorAutomapViewModeOriginal,
            ExtAutomapSwitchToExteriorAutomapViewModeExtra,
            ExtAutomapSwitchToExteriorAutomapViewModeAll,
            ExtAutomapSwitchToExteriorAutomapBackgroundOriginal,
            ExtAutomapSwitchToExteriorAutomapBackgroundAlternative1,
            ExtAutomapSwitchToExteriorAutomapBackgroundAlternative2,
            ExtAutomapSwitchToExteriorAutomapBackgroundAlternative3,
            ExtAutomapMoveLeft,
            ExtAutomapMoveRight,
            ExtAutomapMoveForward,
            ExtAutomapMoveBackward,
            ExtAutomapMoveToWestLocationBorder,
            ExtAutomapMoveToEastLocationBorder,
            ExtAutomapMoveToNorthLocationBorder,
            ExtAutomapMoveToSouthLocationBorder,
            ExtAutomapRotateLeft,
            ExtAutomapRotateRight,
            ExtAutomapRotateAroundPlayerPosLeft,
            ExtAutomapRotateAroundPlayerPosRight,
            ExtAutomapUpstairs,
            ExtAutomapDownstairs,
            ExtAutomapZoomIn,
            ExtAutomapZoomOut,
            ExtAutomapMaxZoom1,
            ExtAutomapMinZoom1,
            ExtAutomapMinZoom2,
            ExtAutomapMaxZoom2,
        }

        public static Dictionary<Buttons, HotkeySequence> keys = null;

        private static void CheckLoaded()
        {
            if (keys == null)
            {
                keys = new Dictionary<Buttons, HotkeySequence>();
                foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
                {
                    if (button == Buttons.None)
                        continue;

                    string buttonName = Enum.GetName(typeof(Buttons), button);
                    if (TextManager.Instance.HasText(textDatabase, buttonName))
                    {
                        keys[button] = HotkeySequence.FromString(TextManager.Instance.GetText(textDatabase, buttonName));
                    }
                    else
                        Debug.Log(string.Format("{0}: no {1} entry", textDatabase, buttonName));
                }
            }
        }

        public static HotkeySequence GetBinding(Buttons button)
        {
            CheckLoaded();
            HotkeySequence key;
            if (keys.TryGetValue(button, out key))
                return key;
            return HotkeySequence.None;
        }
    }
}
