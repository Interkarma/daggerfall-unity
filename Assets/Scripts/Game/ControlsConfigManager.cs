// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: jefetienne
// Contributors:    Gavin Clayton (interkarma@dfworkshop.net)
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// ControlsConfigManager singleton class for controls settings and configuration. Related to the controls windows.
    /// </summary>
    public class ControlsConfigManager : MonoBehaviour
    {
        #region Enums

        public enum UnaryBindings
        {
            Primary,
            Secondary,
            Current
        }

        #endregion

        #region Fields

        private readonly Color crossDupeColor = new Color(0, 0.58f, 1);
        private readonly Color internalDupeColor = new Color(1, 0, 0);

        private readonly Dictionary<InputManager.Actions, string> PrimaryUnsavedKeybindDict 
            = new Dictionary<InputManager.Actions, string>();
        private readonly Dictionary<InputManager.Actions, string> SecondaryUnsavedKeybindDict
            = new Dictionary<InputManager.Actions, string>();

        #endregion

        #region Public Properties

        public static string ElongatedButtonText { get => "..."; }

        public bool UsingPrimary { get; set; } = true;

        #endregion

        #region Private Properties

        private int maxButtonTextLength { get => DaggerfallUnity.Settings.SDFFontRendering ? 16 : 10; }

        private Dictionary<InputManager.Actions, string> CurrentUnsavedKeybindDict
        {
            get { return UsingPrimary ? PrimaryUnsavedKeybindDict : SecondaryUnsavedKeybindDict; }
        }

        #endregion

        #region Singleton

        static ControlsConfigManager instance = null;
        public static ControlsConfigManager Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindSingleton(out instance))
                    {
                        GameObject go = new GameObject();
                        go.name = "ControlsConfigManager";
                        instance = go.AddComponent<ControlsConfigManager>();
                    }
                }
                return instance;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return (instance != null);
            }
        }

        #endregion

        #region Public Static Methods

        public static bool FindSingleton(out ControlsConfigManager singletonOut)
        {
            singletonOut = GameObject.FindObjectOfType<ControlsConfigManager>();
            if (singletonOut == null)
            {
                DaggerfallUnity.LogMessage("Could not locate ControlsConfigManager GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        #endregion

        #region Public Methods

        public string GetUnsavedBinding(InputManager.Actions action, UnaryBindings binding = UnaryBindings.Current)
        {
            var dict = GetUnsavedBindingDictionary(binding);
            string ret;

            if (dict.TryGetValue(action, out ret))
                return ret;
            
            return null;
        }

        public KeyCode GetUnsavedBindingKeyCode(InputManager.Actions action, UnaryBindings binding = UnaryBindings.Current)
        {
            var str = GetUnsavedBinding(action, binding);
            if (!string.IsNullOrEmpty(str))
                return InputManager.Instance.ParseKeyCodeString(str);

            return KeyCode.None;
        }

        public void SetUnsavedBinding(InputManager.Actions action, string keyCodeString, UnaryBindings binding = UnaryBindings.Current)
        {
            GetUnsavedBindingDictionary(binding)[action] = keyCodeString;
        }

        public HashSet<String> GetDuplicates(IEnumerable<String> texts)
        {
            HashSet<String> recorded = new HashSet<String>();
            HashSet<String> dupes = new HashSet<String>();
            Dictionary<String, HashSet<String>> modifiers = new Dictionary<string, HashSet<string>>();
            String none = KeyCode.None.ToString();

            // Sort by combos first to record their modifiers
            var list = texts.OrderByDescending(str =>
            {
                if (str.Contains('+'))
                {
                    var c = InputManager.Instance.GetCombo(InputManager.Instance.GetComboCode(str));
                    // Only record the modifier (i.e. the button that gets held down first)
                    var kc1 = InputManager.Instance.GetKeyString(c.Item1);

                    // Add modifier to 'recorded' so it cannot be used as an independent keybind
                    if (!recorded.Contains(kc1))
                        recorded.Add(kc1);

                    HashSet<String> mods;
                    if (!modifiers.TryGetValue(kc1, out mods))
                        mods = (modifiers[kc1] = new HashSet<String>());

                    // Add combo to mod list
                    mods.Add(str);

                    return true;
                }

                return false;
            });

            // Simple check for duplicates in the list
            // Example (both marked as dupes):
            // "Action1: T"
            // "Action2: T"
            foreach (String str in list)
            {
                if (!recorded.Contains(str))
                    recorded.Add(str);
                else if (str != none)
                    dupes.Add(str);
            }

            // Mark combos as dupes too if a modifier has been used as an independent keybind
            // Example (both marked as dupes):
            // "Action1: LeftShift + T"
            // "Action2: LeftShift"
            foreach (var kv in modifiers)
            {
                if (dupes.Contains(kv.Key))
                    dupes.UnionWith(kv.Value);
            }

            // Mark combos as dupes if the combo'd key is also used as a modifier
            // Example (both marked as dupes):
            // "Action1: LeftShift + T"
            // "Action2: Z + LeftShift"
            foreach (String str in list)
            {
                if (str.Contains('+'))
                {
                    var c = InputManager.Instance.GetCombo(InputManager.Instance.GetComboCode(str));
                    var kc2 = InputManager.Instance.GetKeyString(c.Item2);
                    HashSet<string> mods;

                    if (modifiers.TryGetValue(kc2, out mods))
                    {
                        dupes.UnionWith(mods);
                        dupes.Add(str);
                    }
                }
            }
            return dupes;
        }

        public bool InternalDuplicateKeyCodesExist(UnaryBindings binding)
        {
            var dict = GetUnsavedBindingDictionary(binding);

            return GetDuplicates(dict.Values).Count > 0;
        }

        public bool CheckDuplicateKeyCodes(IEnumerable<Button> totalButtons)
        {
            IEnumerable<String> pkeyList = PrimaryUnsavedKeybindDict.Values;
            IEnumerable<String> skeyList = SecondaryUnsavedKeybindDict.Values;

            var dupes = GetDuplicates(UsingPrimary ? pkeyList : skeyList);

            bool noRedDupes = dupes.Count == 0;

            foreach (Button keybindButton in totalButtons)
            {
                // Assumption: button.Name is the action.ToString()
                var action = (InputManager.Actions)Enum.Parse(typeof(InputManager.Actions), keybindButton.Name);
                var binding = CurrentUnsavedKeybindDict[action];

                if (dupes.Contains(binding))
                    keybindButton.Label.TextColor = internalDupeColor;
                else
                    keybindButton.Label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            }

            // Concat both lists together
            // Remove any duplicates from inside each list, to find the duplicates between the two lists
            var list = new HashSet<string>(pkeyList).Concat(new HashSet<String>(skeyList));

            // Get duplicates between primary and secondary key lists
            dupes = GetDuplicates(list);

            foreach (Button keybindButton in totalButtons)
            {
                // Assumption: button.Name is the action.ToString()
                var action = (InputManager.Actions)Enum.Parse(typeof(InputManager.Actions), keybindButton.Name);
                var binding = CurrentUnsavedKeybindDict[action];

                if (dupes.Contains(binding) && keybindButton.Label.TextColor != internalDupeColor)
                    keybindButton.Label.TextColor = crossDupeColor;
            }

            return noRedDupes && dupes.Count == 0;
        }

        public void ResetUnsavedKeybinds()
        {
            foreach (InputManager.Actions a in Enum.GetValues(typeof(InputManager.Actions)))
            {
                PrimaryUnsavedKeybindDict[a] = InputManager.Instance.GetKeyString(InputManager.Instance.GetBinding(a, true));
            }

            foreach (InputManager.Actions a in Enum.GetValues(typeof(InputManager.Actions)))
            {
                SecondaryUnsavedKeybindDict[a] = InputManager.Instance.GetKeyString(InputManager.Instance.GetBinding(a, false));
            }
        }

        public void SetAllKeyBindValues()
        {
            SetKeyBindValues(true);
            SetKeyBindValues(false);
        }

        public void PromptRemoveKeybindMessage(Button button, Action checkDuplicates)
        {
            if (button.Label.Text == KeyCode.None.ToString())
                return;

            DaggerfallMessageBox removeAssignmentBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow, true);
            removeAssignmentBox.PauseWhileOpen = true;

            string prompt = TextManager.Instance.GetLocalizedText("removeKeybind");
            var action = (InputManager.Actions)Enum.Parse(typeof(InputManager.Actions), button.Name);
            string text = GetButtonText(GetUnsavedBindingKeyCode(action), true);

            removeAssignmentBox.SetText(string.Format(prompt, Regex.Replace(button.Name, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled).Trim(), text));

            removeAssignmentBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            removeAssignmentBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No, true);

            removeAssignmentBox.OnButtonClick += ((s, messageBoxButton) =>
            {
                if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
                {
                    button.Label.Text = KeyCode.None.ToString();
                    SetUnsavedBinding(action, button.Label.Text);

                    checkDuplicates();
                }
                s.CloseWindow();
            });

            removeAssignmentBox.Show();
        }

        public string GetButtonText(KeyCode key, bool fullString = false)
        {

            //Daggerfall DOS-inspired, to an extent
            if (!DaggerfallUnity.Settings.SDFFontRendering)
            {
                switch(key)
                {
                    case KeyCode.LeftAlt:
                        return "LALT";
                    case KeyCode.RightAlt:
                        return "RALT";
                    case KeyCode.LeftControl:
                        return "LCTRL";
                    case KeyCode.RightControl:
                        return "RCTRL";
                    case KeyCode.LeftShift:
                        return "LSHIFT";
                    case KeyCode.RightShift:
                        return "RSHIFT";
                    case KeyCode.PageUp:
                        return "PG UP";
                    case KeyCode.PageDown:
                        return "PG DN";
                    case KeyCode.Insert:
                        return "INS";
                    case KeyCode.Delete:
                        return "DEL";
                    case KeyCode.Backspace:
                        return "BCKSPC";
                    case KeyCode.CapsLock:
                        return "CAPS";

                    case KeyCode.Alpha0:
                        return "A0";
                    case KeyCode.Alpha1:
                        return "A1";
                    case KeyCode.Alpha2:
                        return "A2";
                    case KeyCode.Alpha3:
                        return "A3";
                    case KeyCode.Alpha4:
                        return "A4";
                    case KeyCode.Alpha5:
                        return "A5";
                    case KeyCode.Alpha6:
                        return "A6";
                    case KeyCode.Alpha7:
                        return "A7";
                    case KeyCode.Alpha8:
                        return "A8";
                    case KeyCode.Alpha9:
                        return "A9";

                    case KeyCode.Keypad0:
                        return "KPAD0";
                    case KeyCode.Keypad1:
                        return "KPAD1";
                    case KeyCode.Keypad2:
                        return "KPAD2";
                    case KeyCode.Keypad3:
                        return "KPAD3";
                    case KeyCode.Keypad4:
                        return "KPAD4";
                    case KeyCode.Keypad5:
                        return "KPAD5";
                    case KeyCode.Keypad6:
                        return "KPAD6";
                    case KeyCode.Keypad7:
                        return "KPAD7";
                    case KeyCode.Keypad8:
                        return "KPAD8";
                    case KeyCode.Keypad9:
                        return "KPAD9";
                    case KeyCode.KeypadPeriod:
                        return "KPAD.";
                    case KeyCode.KeypadDivide:
                        return "KPAD/";
                    case KeyCode.KeypadMultiply:
                        return "KPAD*";
                    case KeyCode.KeypadMinus:
                        return "KPAD-";
                    case KeyCode.KeypadPlus:
                        return "KPAD+";
                    case KeyCode.KeypadEquals:
                        return "KPAD=";
                    case KeyCode.KeypadEnter:
                        return "KPD ENTR";
                }
            }

            string text = null;

            switch(key)
            {
                case KeyCode.BackQuote:
                    text = "`";
                    break;
                case KeyCode.Minus:
                    text = "-";
                    break;
                case KeyCode.Equals:
                    text = "=";
                    break;
                case KeyCode.Backslash:
                    text = "\\";
                    break;
                case KeyCode.LeftBracket:
                    text = "[";
                    break;
                case KeyCode.RightBracket:
                    text = "]";
                    break;
                case KeyCode.Semicolon:
                    text = ";";
                    break;
                case KeyCode.Quote:
                    text = "'";
                    break;
                case KeyCode.Comma:
                    text = ",";
                    break;
                case KeyCode.Period:
                    text = ".";
                    break;
                case KeyCode.Slash:
                    text = "/";
                    break;
                case KeyCode.KeypadPeriod:
                    text = "Keypad.";
                    break;
                case KeyCode.KeypadDivide:
                    text = "Keypad/";
                    break;
                case KeyCode.KeypadMultiply:
                    text = "Keypad*";
                    break;
                case KeyCode.KeypadMinus:
                    text = "Keypad-";
                    break;
                case KeyCode.KeypadPlus:
                    text = "Keypad+";
                    break;
                case KeyCode.KeypadEquals:
                    text = "Keypad=";
                    break;
                case KeyCode.UpArrow:
                    text = "Up";
                    break;
                case KeyCode.DownArrow:
                    text = "Down";
                    break;
                case KeyCode.LeftArrow:
                    text = "Left";
                    break;
                case KeyCode.RightArrow:
                    text = "Right";
                    break;
                case KeyCode.LeftAlt:
                    text = "L Alt";
                    break;
                case KeyCode.RightAlt:
                    text = "R Alt";
                    break;
                case KeyCode.LeftControl:
                    text = "L Ctrl";
                    break;
                case KeyCode.RightControl:
                    text = "R Ctrl";
                    break;
                case KeyCode.LeftShift:
                    text = "L Shift";
                    break;
                case KeyCode.RightShift:
                    text = "R Shift";
                    break;
                case KeyCode.Return:
                    text = "Enter";
                    break;
            }

            if ((int)key >= (int)KeyCode.JoystickButton0 && (int)key <= (int)KeyCode.JoystickButton19)
            {
                text = "Joy B" + (((int)key - 10) % 20);
            }
            else if ((int)key >= InputManager.startingAxisKeyCode && (int)key < InputManager.startingComboKeyCode)
            {
                text = "Joy" + (((int)key % InputManager.startingAxisKeyCode) / 2 + 1) + " B" + ((int)key % 2);
            }
            else if ((int)key >= InputManager.startingComboKeyCode)
            {
                var combo = InputManager.Instance.GetCombo(key);
                text = FormatButtonText(GetButtonText(combo.Item1) + " + " + GetButtonText(combo.Item2), fullString);
            }
            else if (string.IsNullOrEmpty(text))
            {
                text = FormatButtonText(InputManager.Instance.GetKeyString((KeyCode)key), fullString);
            }

            return DaggerfallUnity.Settings.SDFFontRendering ? text : text.ToUpper();
        }

        #endregion

        #region Private Methods

        private Dictionary<InputManager.Actions, string> GetUnsavedBindingDictionary(UnaryBindings ub)
        {
            switch(ub)
            {
                case UnaryBindings.Primary:
                    return PrimaryUnsavedKeybindDict;
                case UnaryBindings.Secondary:
                    return SecondaryUnsavedKeybindDict;
                default:
                    return CurrentUnsavedKeybindDict;
            }
        }

        private void SetKeyBindValues(bool primary)
        {
            var dict = primary ? PrimaryUnsavedKeybindDict : SecondaryUnsavedKeybindDict;
            foreach (var action in dict.Keys)
            {
                KeyCode code = InputManager.Instance.ParseKeyCodeString(dict[action]);

                // Rebind only if new code is different
                KeyCode curCode = InputManager.Instance.GetBinding(action, primary);
                if (curCode != code)
                {
                    if (primary && code == KeyCode.None)
                        InputManager.Instance.AddRemovedPrimaryAction(action);

                    InputManager.Instance.SetBinding(code, action, primary);
                    Debug.LogFormat("({0}) Bound Action {1} with Code {2} ({3})", primary ? "Primary" : "Secondary", action, code.ToString(), (int)code);
                }
            }
        }

        private string FormatButtonText(string str, bool fullString)
        {
            if (str.Length <= maxButtonTextLength || fullString)
                //Split camel/pascal case by spaces
                return Regex.Replace(str, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled).Trim();

            return ElongatedButtonText;
        }

        #endregion
    }
}
