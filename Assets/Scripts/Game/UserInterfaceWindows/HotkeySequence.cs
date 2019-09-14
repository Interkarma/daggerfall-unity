using System;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    // this is a helper class to implement behaviour and easier use of hotkeys and key modifiers (left-shift, right-shift, ...) in conjunction
    // note: currently a combination of key modifiers like shift+alt is not supported. all specified modifiers are comined with an or-relation
    class HotkeySequence
    {
        [Flags]
        public enum KeyModifiers
        {
            None = 0,
            LeftControl = 1,
            RightControl = 2,
            LeftShift = 4,
            RightShift = 8,
            LeftAlt = 16,
            RightAlt = 32,
        };

        public KeyCode keyCode;
        public KeyModifiers modifiers;

        public HotkeySequence(KeyCode keyCode, KeyModifiers modifiers)
        {
            this.keyCode = keyCode;
            this.modifiers = modifiers;
        }

        public static KeyModifiers getKeyModifiers(bool leftControl, bool rightControl, bool leftShift, bool rightShift, bool leftAlt, bool rightAlt)
        {
            KeyModifiers keyModifiers = KeyModifiers.None;
            if (leftControl)
                keyModifiers = keyModifiers | KeyModifiers.LeftControl;
            if (rightControl)
                keyModifiers = keyModifiers | KeyModifiers.RightControl;
            if (leftShift)
                keyModifiers = keyModifiers | KeyModifiers.LeftShift;
            if (rightShift)
                keyModifiers = keyModifiers | KeyModifiers.RightShift;
            if (leftAlt)
                keyModifiers = keyModifiers | KeyModifiers.LeftAlt;
            if (rightAlt)
                keyModifiers = keyModifiers | KeyModifiers.RightAlt;
            return keyModifiers;
        }

        public static bool checkSetModifiers(HotkeySequence.KeyModifiers pressedModifiers, HotkeySequence.KeyModifiers triggeringModifiers)
        {
            if (triggeringModifiers == KeyModifiers.None)
            {
                if (pressedModifiers == KeyModifiers.None)
                    return true;
                else
                    return false;
            }

            return ((pressedModifiers & triggeringModifiers) != 0); // if any of the modifiers in triggeringModifiers is pressed return true                
        }
    }
}
