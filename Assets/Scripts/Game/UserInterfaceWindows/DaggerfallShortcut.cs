using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public static class DaggerfallShortcut
    {
        const string textDatabase = "DialogShortcuts";

        public enum Buttons
        {
            Accept = 0,
            Reject = 1,
            Cancel = 2,
            Yes = 3,
            No = 4,
            OK = 5,
            Male = 6,
            Female = 7,
            Add = 8,
            Delete = 9,
            Edit = 10,
            Counter = 11,
            _12MON = 12,
            _36MON = 13,
            Copy = 14,
            Guilty = 15,
            NotGuilty = 16,
            Debate = 17,
            Lie = 18,
            Anchor = 19,
            Teleport = 20,

            // Main menu
            MainMenuLoad,
            MainMenuStart,
            MainMenuExit,

            // Options menu
            OptionsExit,
            OptionsContinue,
            OptionsSave,
            OptionsLoad,
            OptionsControls,
            OptionsFullScreen,
            OptionsHeadBobbing,

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

            // TravelMap
            TravelMapFind,
            TravelMapList,

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

            // Automap
            AutomapSwitchAutomapGridMode,
        }

        public static Dictionary<Buttons, HotkeySequence> keys = null;

        public static Dictionary<Buttons, HotkeySequence> Keys
        {
            get
            {
                if (keys == null)
                {
                    keys = new Dictionary<Buttons, HotkeySequence>();
                    foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
                    {
                        string buttonName = Enum.GetName(typeof(Buttons), button);
                        if (TextManager.Instance.HasText(textDatabase, buttonName))
                        {
                            keys[button] = HotkeySequence.FromString(TextManager.Instance.GetText(textDatabase, buttonName));
                        }
                    }
                }
                return keys;
            }
        }
    }
}
