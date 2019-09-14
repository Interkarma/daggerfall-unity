using System;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    // this is a helper class to implement behaviour and easier use of hotkeys and key modifiers (left-shift, right-shift, ...) in conjunction
    public class HotkeySequence
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
            // Virtuals
            Control = 64,
            Shift = 128,
            Alt = 256,
        };

        public const KeyModifiers virtualKeys = KeyModifiers.Control | KeyModifiers.Shift | KeyModifiers.Alt;

        private KeyCode keyCode;
        private KeyModifiers modifiers;

        public static HotkeySequence None = new HotkeySequence(KeyCode.None, KeyModifiers.None);

        public HotkeySequence(KeyCode keyCode, KeyModifiers modifiers)
        {
            this.keyCode = keyCode;
            this.modifiers = modifiers;
            // Add inferred virtuals
            if ((modifiers & (KeyModifiers.LeftControl | KeyModifiers.RightControl)) != 0)
                this.modifiers |= KeyModifiers.Control;
            if ((modifiers & (KeyModifiers.LeftShift | KeyModifiers.RightShift)) != 0)
                this.modifiers |= KeyModifiers.Shift;
            if ((modifiers & (KeyModifiers.LeftAlt | KeyModifiers.RightAlt)) != 0)
                this.modifiers |= KeyModifiers.Alt;
        }

        public static KeyModifiers GetKeyModifiers(bool leftControl, bool rightControl, bool leftShift, bool rightShift, bool leftAlt, bool rightAlt)
        {
            KeyModifiers keyModifiers = KeyModifiers.None;
            if (leftControl)
                keyModifiers = keyModifiers | KeyModifiers.LeftControl | KeyModifiers.Control;
            if (rightControl)
                keyModifiers = keyModifiers | KeyModifiers.RightControl | KeyModifiers.Control;
            if (leftShift)
                keyModifiers = keyModifiers | KeyModifiers.LeftShift | KeyModifiers.Shift;
            if (rightShift)
                keyModifiers = keyModifiers | KeyModifiers.RightShift | KeyModifiers.Shift;
            if (leftAlt)
                keyModifiers = keyModifiers | KeyModifiers.LeftAlt | KeyModifiers.Alt;
            if (rightAlt)
                keyModifiers = keyModifiers | KeyModifiers.RightAlt | KeyModifiers.Alt;
            return keyModifiers;
        }

        public static KeyModifiers GetKeyboardKeyModifiers()
        {
            return GetKeyModifiers(Input.GetKey(KeyCode.LeftControl), Input.GetKey(KeyCode.RightControl), Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.RightShift), Input.GetKey(KeyCode.LeftAlt), Input.GetKey(KeyCode.RightAlt));
        }

        public static bool CheckSetModifiers(HotkeySequence.KeyModifiers pressedModifiers, HotkeySequence.KeyModifiers triggeringModifiers)
        {
            return ((pressedModifiers & triggeringModifiers) == triggeringModifiers) && // all of the modifiers in triggeringModifiers are pressed
                ((pressedModifiers & (virtualKeys & ~triggeringModifiers)) == 0);       // only the virtual modifiers specified in triggeringModifiers are pressed
        }

        public bool IsDownWith(KeyModifiers pressedModifiers)
        {
            return Input.GetKeyDown(keyCode) && CheckSetModifiers(pressedModifiers, modifiers);
        }

        public bool IsPressedWith(KeyModifiers pressedModifiers)
        {
            return Input.GetKey(keyCode) && CheckSetModifiers(pressedModifiers, modifiers);
        }

        // Simple method variants if you don't mind building a temporary KeyModifiers
        public bool IsDown()
        {
            return IsDownWith(GetKeyboardKeyModifiers());
        }

        public bool IsPressed()
        {
            return IsPressedWith(GetKeyboardKeyModifiers());
        }
    }
}
