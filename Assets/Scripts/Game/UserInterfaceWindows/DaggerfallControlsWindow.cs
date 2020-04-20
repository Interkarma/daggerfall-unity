// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: Justin Steele
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
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;


namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements controls window.
    /// </summary>
    public class DaggerfallControlsWindow : DaggerfallPopupWindow
    {
        #region Fields

        Texture2D nativeTexture;
        Texture2D mLookAltTexture;
        Panel controlsPanel = new Panel();
        Panel mLookAltPanel = new Panel();
        List<Button> moveKeysOne = new List<Button>();
        List<Button> moveKeysTwo = new List<Button>();
        List<Button> modeKeys = new List<Button>();
        List<Button> magicKeys = new List<Button>();
        List<Button> weaponKeys = new List<Button>();
        List<Button> statusKeys = new List<Button>();
        List<Button> activateKeys = new List<Button>();
        List<Button> lookKeys = new List<Button>();
        List<Button> uiKeys = new List<Button>();
        List<List<Button>> allKeys = new List<List<Button>>();

        string[] actions = Enum.GetNames(typeof(InputManager.Actions));
        const string nativeTextureName = "CNFG00I0.IMG";
        const string mLookAltTextureName = "CNFG00I1.IMG";
        const string confirmDefaults = "Are you sure you want to set default controls?";
        bool waitingForInput = false;
        bool doUpdateKeybinds = true;

        #endregion

        #region Public Static Fields

        public static Dictionary<InputManager.Actions, string> UnsavedKeybindDict = new Dictionary<InputManager.Actions, string>();

        #endregion

        #region Constructors

        public DaggerfallControlsWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            :base(uiManager, previousWindow)
        {
        }

        #endregion

        #region Unity

        public override void Update()
        {
            base.Update();

            if (!AllowCancel && !waitingForInput && Input.GetKeyDown(KeyCode.Escape))
            {
                ShowMultipleAssignmentsMessage();
            }
        }

        #endregion

        #region Setup

        protected override void Setup()
        {
            // Load textures
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeTextureName);
            if (!nativeTexture)
                throw new Exception("DaggerfallControlsWindow: Could not load native texture.");
            mLookAltTexture = DaggerfallUI.GetTextureFromImg(mLookAltTextureName);

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Controls panel
            controlsPanel.HorizontalAlignment = HorizontalAlignment.Center;
            controlsPanel.Size = NativePanel.Size;
            controlsPanel.BackgroundTexture = nativeTexture;
            NativePanel.Components.Add(controlsPanel);

            // Mouse Look Alternative Controls Panel
            mLookAltPanel.Position = new Vector2(152, 100);
            mLookAltPanel.Size = new Vector2(168, 45);
            mLookAltPanel.BackgroundTexture = mLookAltTexture;
            controlsPanel.Components.Add(mLookAltPanel);

            #region Tab Buttons

            // Joystick
            Button joystickButton = DaggerfallUI.AddButton(new Rect(0, 190, 80, 10), controlsPanel);
            joystickButton.BackgroundColor = new Color(1, 0, 0, 0.5f);
            joystickButton.OnMouseClick += JoystickButton_OnMouseClick;

            // Mouse
            Button mouseButton = DaggerfallUI.AddButton(new Rect(80, 190, 80, 10), controlsPanel);
            mouseButton.BackgroundColor = new Color(0.3f, 0.42f, 0.16f, 1f);
            mouseButton.Label.Text = "ADVANCED";
            mouseButton.OnMouseClick += MouseButton_OnMouseClick;

            // Default
            Button defaultButton = DaggerfallUI.AddButton(new Rect(160, 190, 80, 10), controlsPanel);
            defaultButton.OnMouseClick += DefaultButton_OnMouseClick;

            // Continue
            Button continueButton = DaggerfallUI.AddButton(new Rect(240, 190, 80, 10), controlsPanel);
            continueButton.OnMouseClick += ContinueButton_OnMouseClick;

            #endregion

            #region Keybind Buttons

            ResetUnsavedDictionary();

            SetupKeybindButtons(moveKeysOne, 2, 8, 57, 13, true);
            SetupKeybindButtons(moveKeysTwo, 8, 14, 164, 13, true);
            SetupKeybindButtons(modeKeys, 14, 20, 270, 13, true);
            SetupKeybindButtons(magicKeys, 20, 24, 102, 80, true);
            SetupKeybindButtons(weaponKeys, 24, 27, 102, 125, true);
            SetupKeybindButtons(statusKeys, 27, 30, 102, 159, true);
            SetupKeybindButtons(activateKeys, 30, 32, 270, 80, true);
            SetupKeybindButtons(lookKeys, 32, 36, 270, 103, true);
            SetupKeybindButtons(uiKeys, 36, 40, 270, 148, true);

            #endregion
        }

        #endregion

        #region Overrides

        public override void OnPop()
        {
            // Update keybinds only when exiting from a valid configuration
            SaveAllKeyBindValues();
            InputManager.Instance.SaveKeyBinds();
            ResetUnsavedDictionary();
        }

        public override void OnReturn()
        {
            if (doUpdateKeybinds)
                UpdateKeybindButtons();
            doUpdateKeybinds = true;
        }

        #endregion

        #region Public Static Methods

        public static HashSet<String> GetDuplicates(IEnumerable<String> texts)
        {
            HashSet<String> recorded = new HashSet<String>();
            HashSet<String> dupes = new HashSet<String>();
            foreach (String str in texts)
            {
                if(!recorded.Contains(str))
                    recorded.Add(str);
                else
                    dupes.Add(str);
            }
            return dupes;
        }

        #endregion

        #region Private Methods

        private void ResetUnsavedDictionary()
        {
            foreach (InputManager.Actions a in Enum.GetValues(typeof(InputManager.Actions)))
                UnsavedKeybindDict[a] = InputManager.Instance.GetBinding(a).ToString();
        }

        private void SaveAllKeyBindValues()
        {
            foreach(var action in UnsavedKeybindDict.Keys)
            {
                KeyCode code = (KeyCode)Enum.Parse(typeof(KeyCode), UnsavedKeybindDict[action]);

                // Rebind only if new code is different
                KeyCode curCode = InputManager.Instance.GetBinding(action);
                if (curCode != code)
                {
                    InputManager.Instance.SetBinding(code, action);
                    Debug.LogFormat("Bound Action {0} with Code {1}", action, code.ToString());
                }
            }
        }

        private void SetupKeybindButtons(List<Button> buttonGroup, int startPoint, int endPoint, int leftOffset, int topOffset, bool firstSetup)
        {
            for (int i = startPoint; i < endPoint; i++)
            {
                InputManager.Actions key = (InputManager.Actions)Enum.Parse(typeof(InputManager.Actions), actions[i]);
                int j = i - startPoint;

                if (firstSetup)
                {
                    buttonGroup.Add(new Button());
                    buttonGroup[j].Label.ShadowPosition = Vector2.zero;
                    buttonGroup[j].Size = new Vector2(47, 7);

                    if (j == 0)
                        buttonGroup[j].Position = new Vector2(leftOffset, topOffset);
                    else
                        buttonGroup[j].Position = new Vector2(leftOffset, buttonGroup[j - 1].Position.y + 11);

                    controlsPanel.Components.Add(buttonGroup[j]);
                    buttonGroup[j].Name = actions[i];
                    buttonGroup[j].OnMouseClick += KeybindButton_OnMouseClick;
                    if (i == endPoint - 1)
                    {
                        allKeys.Add(buttonGroup);
                    }
                }

                buttonGroup[j].Label.Text = UnsavedKeybindDict[key].ToString();
                buttonGroup[j].Label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            }
        }

        private void CheckDuplicates()
        {
            IEnumerable<String> keyList = UnsavedKeybindDict.Select(kv => kv.Value);

            HashSet<String> dupes = GetDuplicates(keyList);

            AllowCancel = dupes.Count == 0;

            IEnumerable<Button> totalButtons = allKeys.SelectMany(l => l);
            foreach (Button keybindButton in totalButtons)
            {
                if (dupes.Contains(keybindButton.Label.Text))
                    keybindButton.Label.TextColor = new Color(1, 0, 0);
                else
                    keybindButton.Label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            }
        }

        private void SetDefaults()
        {
            InputManager.Instance.ResetDefaults();
            ResetUnsavedDictionary();
            UpdateKeybindButtons();
            AllowCancel = true;
        }

        private void UpdateKeybindButtons()
        {
            SetupKeybindButtons(moveKeysOne, 2, 8, 56, 12, false);
            SetupKeybindButtons(moveKeysTwo, 8, 14, 163, 12, false);
            SetupKeybindButtons(modeKeys, 14, 20, 269, 12, false);
            SetupKeybindButtons(magicKeys, 20, 24, 101, 79, false);
            SetupKeybindButtons(weaponKeys, 24, 27, 101, 124, false);
            SetupKeybindButtons(statusKeys, 27, 30, 101, 158, false);
            SetupKeybindButtons(activateKeys, 30, 32, 269, 79, false);
            SetupKeybindButtons(lookKeys, 32, 36, 269, 102, false);
            SetupKeybindButtons(uiKeys, 36, 40, 269, 147, false);

            CheckDuplicates();
        }

        private void ShowMultipleAssignmentsMessage()
        {
            doUpdateKeybinds = false;

            DaggerfallMessageBox multipleAssignmentsBox = new DaggerfallMessageBox(uiManager, this);
            multipleAssignmentsBox.SetText(HardStrings.multipleAssignments);
            multipleAssignmentsBox.ClickAnywhereToClose = true;
            multipleAssignmentsBox.Show();
        }

        //a workaround solution to setting the 'waitingForInput' instance variable in a
        //static IEnumerator method. IEnumerator methods can't accept out/ref parameters
        private void SetWaitingForInput(bool b)
        {
            waitingForInput = b;
            AllowCancel = !b;
        }

        #endregion

        #region Tab Button Event Handlers

        private void JoystickButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (waitingForInput)
                return;

            // uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenJoystickControlsWindow);
        }

        private void MouseButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (waitingForInput)
                return;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenMouseControlsWindow);
        }

        private void DefaultButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (waitingForInput)
                return;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaggerfallMessageBox confirmDefaultsBox = new DaggerfallMessageBox(uiManager, DaggerfallMessageBox.CommonMessageBoxButtons.YesNo, confirmDefaults, this);
            confirmDefaultsBox.OnButtonClick += ConfirmDefaultsBox_OnButtonClick;
            confirmDefaultsBox.Show();
        }

        private void ConfirmDefaultsBox_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                SetDefaults();
                InputManager.Instance.SaveKeyBinds();
            }
        }

        private void ContinueButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (waitingForInput)
                return;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);

            if (!AllowCancel)
            {
                ShowMultipleAssignmentsMessage();
            }
            else
            {
                CancelWindow();
            }
        }

        #endregion

        #region Keybind Event Handlers

        private void KeybindButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (waitingForInput)
                return;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            Button thisKeybindButton = (Button)sender;

            InputManager.Instance.StartCoroutine(WaitForKeyPress(thisKeybindButton, CheckDuplicates, SetWaitingForInput));
        }

        public static IEnumerator WaitForKeyPress(Button button, System.Action checkDuplicates, System.Action<bool> setWaitingForInput)
        {
            string currentLabel = button.Label.Text;

            button.Label.Text = "";
            yield return new WaitForSecondsRealtime(0.05f);

            while(!Input.anyKeyDown)
            {
                setWaitingForInput(true);
                yield return null;
            }
            setWaitingForInput(false);

            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(code))
                {
                    if (InputManager.Instance.ReservedKeys.FirstOrDefault(x => x == code) == KeyCode.None)
                    {
                        button.Label.Text = code.ToString();

                        var action = (InputManager.Actions)Enum.Parse(typeof(InputManager.Actions), button.Name);

                        UnsavedKeybindDict[action] = button.Label.Text;
                        checkDuplicates();
                    }
                    else
                    {
                        button.Label.Text = currentLabel;
                    }
                }
            }
        }

        #endregion
    }
}
