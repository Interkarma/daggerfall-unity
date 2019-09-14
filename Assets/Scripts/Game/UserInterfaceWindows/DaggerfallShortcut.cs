using System;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public static class DaggerfallShortcut
    {
        const string textDatabase = "DialogShortcuts";

        public enum Buttons
        {
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
