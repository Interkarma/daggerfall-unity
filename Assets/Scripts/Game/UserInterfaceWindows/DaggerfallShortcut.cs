using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public static class DaggerfallShortcut
    {
        const string textDatabase = "DialogShortcuts";

        private const string ControlPrefix = "Control-";
        private const string ShiftPrefix = "Shift-";
        private const string AltPrefix = "Alt-";

        public static Dictionary<DaggerfallMessageBox.MessageBoxButtons, HotkeySequence> keys = null;

        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }


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
                            string hotkeyName = TextManager.Instance.GetText(textDatabase, buttonName);
                            int startKeyName = 0;
                            HotkeySequence.KeyModifiers modifiers = default(HotkeySequence.KeyModifiers);
                            while (true)
                            {
                                if (hotkeyName.Substring(startKeyName).StartsWith(ControlPrefix, StringComparison.InvariantCulture))
                                {
                                    modifiers |= HotkeySequence.KeyModifiers.Control;
                                    startKeyName += ControlPrefix.Length;
                                }
                                else if (hotkeyName.Substring(startKeyName).StartsWith(ShiftPrefix, StringComparison.InvariantCulture))
                                {
                                    modifiers |= HotkeySequence.KeyModifiers.Shift;
                                    startKeyName += ShiftPrefix.Length;
                                }
                                else if (hotkeyName.Substring(startKeyName).StartsWith(AltPrefix, StringComparison.InvariantCulture))
                                {
                                    modifiers |= HotkeySequence.KeyModifiers.Alt;
                                    startKeyName += AltPrefix.Length;
                                }
                                else
                                    break;
                            }
                            keys[button] = new HotkeySequence(ParseEnum<KeyCode>(hotkeyName.Substring(startKeyName)), modifiers);
                        }
                    }
                }
                return keys;
            }
        }
    }
}
