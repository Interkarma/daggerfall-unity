using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public static class DaggerfallShortcut
    {
        const string textDatabase = "DialogShortcuts";

        public static Dictionary<DaggerfallMessageBox.MessageBoxButtons, HotkeySequence> keys = null;

        public static Dictionary<DaggerfallMessageBox.MessageBoxButtons, HotkeySequence> Keys
        {
            get
            {
                if (keys == null)
                {
                    keys = new Dictionary<DaggerfallMessageBox.MessageBoxButtons, HotkeySequence>();
                    foreach (DaggerfallMessageBox.MessageBoxButtons button in Enum.GetValues(typeof(DaggerfallMessageBox.MessageBoxButtons)))
                    {
                        string buttonName = Enum.GetName(typeof(DaggerfallMessageBox.MessageBoxButtons), button);
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
