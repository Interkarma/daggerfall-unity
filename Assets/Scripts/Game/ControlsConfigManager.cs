// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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

        public bool UsingPrimary { get; set; } = true;

        #endregion

        #region Private Properties

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

        public void SetUnsavedBinding(InputManager.Actions action, string keyCodeString, UnaryBindings binding = UnaryBindings.Current)
        {
            GetUnsavedBindingDictionary(binding)[action] = keyCodeString;
        }

        public HashSet<String> GetDuplicates(IEnumerable<String> texts)
        {
            HashSet<String> recorded = new HashSet<String>();
            HashSet<String> dupes = new HashSet<String>();
            String none = KeyCode.None.ToString();

            foreach (String str in texts)
            {
                if (!recorded.Contains(str))
                    recorded.Add(str);
                else if (str != none)
                    dupes.Add(str);
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
                if (dupes.Contains(keybindButton.Label.Text))
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
                if (dupes.Contains(keybindButton.Label.Text) && keybindButton.Label.TextColor != internalDupeColor)
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

            DaggerfallMessageBox removeAssignmentBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            removeAssignmentBox.PauseWhileOpen = true;

            string prompt = TextManager.Instance.GetLocalizedText("removeKeybind");
            removeAssignmentBox.SetText(string.Format(prompt, button.Name, button.Label.Text));
            removeAssignmentBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
            removeAssignmentBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No, true);

            removeAssignmentBox.OnButtonClick += ((s, messageBoxButton) =>
            {
                if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
                {
                    button.Label.Text = KeyCode.None.ToString();
                    var action = (InputManager.Actions)Enum.Parse(typeof(InputManager.Actions), button.Name);
                    SetUnsavedBinding(action, button.Label.Text);
                    checkDuplicates();
                }
                s.CloseWindow();
            });

            removeAssignmentBox.Show();
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
                    InputManager.Instance.SetBinding(code, action, primary);
                    Debug.LogFormat("({0}) Bound Action {1} with Code {2} ({3})", primary ? "Primary" : "Secondary", action, code.ToString(), (int)code);
                }
            }
        }

        #endregion
    }
}
