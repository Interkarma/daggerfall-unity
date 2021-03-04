using System;
using System.Text;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    // this is a helper class to implement behaviour and easier use of hotkeys and key modifiers (left-shift, right-shift, ...) in conjunction
    public class HotkeySequence
    {
        [Flags]
        public enum KeyModifiers
        {
            None = 0,
            LeftCtrl = 1,
            RightCtrl = 2,
            LeftShift = 4,
            RightShift = 8,
            LeftAlt = 16,
            RightAlt = 32,
            // Virtuals
            Ctrl = 64,
            Shift = 128,
            Alt = 256,
        };

        public enum HotkeySequenceProcessStatus
        {
            NotFound,
            Handled,
            Disabled,
        };

        public const KeyModifiers virtualKeys = KeyModifiers.Ctrl | KeyModifiers.Shift | KeyModifiers.Alt;

        private readonly KeyCode keyCode;
        private readonly KeyModifiers modifiers;

        private readonly bool useSecondary;

        public static HotkeySequence None = new HotkeySequence(KeyCode.None, KeyModifiers.None);

        public HotkeySequence(KeyCode keyCode, KeyModifiers modifiers, bool useSecondary = false)
        {
            this.keyCode = keyCode;
            this.modifiers = modifiers;
            this.useSecondary = useSecondary;
            // Add inferred virtuals
            if ((modifiers & (KeyModifiers.LeftCtrl | KeyModifiers.RightCtrl)) != 0)
                this.modifiers |= KeyModifiers.Ctrl;
            if ((modifiers & (KeyModifiers.LeftShift | KeyModifiers.RightShift)) != 0)
                this.modifiers |= KeyModifiers.Shift;
            if ((modifiers & (KeyModifiers.LeftAlt | KeyModifiers.RightAlt)) != 0)
                this.modifiers |= KeyModifiers.Alt;
        }

        // Used to check for conflicts without giving away private data
        public bool IsSameKeyCode(KeyCode otherKeyCode)
        {
            return keyCode == otherKeyCode;
        }

        public HotkeySequence WithKeyCode(KeyCode newKeyCode)
        {
            return new HotkeySequence(newKeyCode, modifiers);
        }

        // Build a HotkeySequence from string
        // the order of modifiers doesn't matter
        public static readonly string expectedSyntax = "[[Left|Right]Ctrl-][[Left|Right]Alt-][[Left|Right]Shift-]<KeyCode>";

        public static HotkeySequence FromString(string hotkeyName)
        {
            if (hotkeyName == "None")
                // Make sure to return the HotkeySequence.None instance
                return None;
            try
            {
                string[] keywords = hotkeyName.Split('-');

                KeyCode keyCode = ParseEnum<KeyCode>(keywords[keywords.Length - 1]);
                KeyModifiers modifiers = KeyModifiers.None;
                for (int i = 0; i < keywords.Length - 1; i++)
                {
                    KeyModifiers keyModifier = ParseEnum<KeyModifiers>(keywords[i]);
                    modifiers |= keyModifier;
                }
                return new HotkeySequence(keyCode, modifiers);
            }
            catch (Exception)
            {
                Debug.Log(string.Format("Failed parsing hotkey {0}, expected {1}", hotkeyName, expectedSyntax));
                return None;
            }
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder(30);
            if ((modifiers & KeyModifiers.Ctrl) != 0)
            {
                if ((modifiers & KeyModifiers.LeftCtrl) != 0 && (modifiers & KeyModifiers.RightCtrl) == 0)
                    result.Append("LeftCtrl-");
                else if ((modifiers & KeyModifiers.RightCtrl) != 0 && (modifiers & KeyModifiers.LeftCtrl) == 0)
                    result.Append("RightCtrl-");
                else
                    result.Append("Ctrl-");
            }
            if ((modifiers & KeyModifiers.Alt) != 0)
            {
                if ((modifiers & KeyModifiers.LeftAlt) != 0 && (modifiers & KeyModifiers.RightAlt) == 0)
                    result.Append("LeftAlt-");
                else if ((modifiers & KeyModifiers.RightAlt) != 0 && (modifiers & KeyModifiers.LeftAlt) == 0)
                    result.Append("RightAlt-");
                else
                    result.Append("Alt-");
            }
            if ((modifiers & KeyModifiers.Shift) != 0)
            {
                if ((modifiers & KeyModifiers.LeftShift) != 0 && (modifiers & KeyModifiers.RightShift) == 0)
                    result.Append("LeftShift-");
                else if ((modifiers & KeyModifiers.RightShift) != 0 && (modifiers & KeyModifiers.LeftShift) == 0)
                    result.Append("RightShift-");
                else
                    result.Append("Shift-");
            }
            result.Append(keyCode.ToString());
            return result.ToString();
        }

        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static KeyModifiers GetKeyModifiers(bool leftControl, bool rightControl, bool leftShift, bool rightShift, bool leftAlt, bool rightAlt)
        {
            KeyModifiers keyModifiers = KeyModifiers.None;
            if (leftControl)
                keyModifiers = keyModifiers | KeyModifiers.LeftCtrl | KeyModifiers.Ctrl;
            if (rightControl)
                keyModifiers = keyModifiers | KeyModifiers.RightCtrl | KeyModifiers.Ctrl;
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
            return InputManager.Instance.GetKeyDown(keyCode, useSecondary) && CheckSetModifiers(pressedModifiers, modifiers);
        }

        public bool IsUpWith(KeyModifiers pressedModifiers)
        {
            return InputManager.Instance.GetKeyUp(keyCode, useSecondary) && CheckSetModifiers(pressedModifiers, modifiers);
        }

        public bool IsPressedWith(KeyModifiers pressedModifiers)
        {
            return InputManager.Instance.GetKey(keyCode, useSecondary) && CheckSetModifiers(pressedModifiers, modifiers);
        }

        // Simple method variants if you don't mind building a temporary KeyModifiers
        public bool IsDown()
        {
            return IsDownWith(GetKeyboardKeyModifiers());
        }

        public bool IsUp()
        {
            return IsUpWith(GetKeyboardKeyModifiers());
        }

        public bool IsPressed()
        {
            return IsPressedWith(GetKeyboardKeyModifiers());
        }
    }
}
