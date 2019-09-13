using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public static class DaggerfallShortcut
    {
        const string textDatabase = "DialogShortcuts";

        public static Dictionary<DaggerfallMessageBox.MessageBoxButtons, KeyCode> keys = null;

        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }


        public static Dictionary<DaggerfallMessageBox.MessageBoxButtons, KeyCode> Keys
        {
            get
            {
                if (keys == null)
                {
                    keys = new Dictionary<DaggerfallMessageBox.MessageBoxButtons, KeyCode>();
                    foreach (DaggerfallMessageBox.MessageBoxButtons button in Enum.GetValues(typeof(DaggerfallMessageBox.MessageBoxButtons)))
                    {
                        string buttonName = Enum.GetName(typeof(DaggerfallMessageBox.MessageBoxButtons), button);
                        if (TextManager.Instance.HasText(textDatabase, buttonName))
                            keys[button] = ParseEnum<KeyCode>(TextManager.Instance.GetText(textDatabase, buttonName));
                    }
                }
                return keys;
            }
        }
    }
}
