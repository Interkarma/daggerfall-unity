// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: jefetienne
// Contributors:
//
// Notes:
//

using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements joystick controls window.
    /// </summary>
    public class DaggerfallJoystickControlsWindow : DaggerfallPopupWindow
    {
        #region Fields

        const string rightClickString = "Right-Click";
        const string middleClickString = "Middle-Click";
        const string leftClickString = "Left-Click";
        const string backString = "Back";

        Color mainPanelBackgroundColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        Color keybindButtonBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        Color continueButtonBackgroundColor = new Color(0.5f, 0.0f, 0.0f, 1.0f);

        protected Panel mainPanel;
        protected TextLabel titleLabel;

        protected Button leftClickKeybindButton = new Button();
        protected Button middleClickKeybindButton = new Button();
        protected Button rightClickKeybindButton = new Button();
        protected Button backKeybindButton = new Button();
        protected Button movementHorizontalAxisKeybindButton = new Button();
        protected Button movementVerticalAxisKeybindButton = new Button();
        protected Button lookHorizontalAxisKeybindButton = new Button();
        protected Button lookVerticalAxisKeybindButton = new Button();
        protected Button continueButton = new Button();

        protected HorizontalSlider joystickCameraSensitivitySlider = new HorizontalSlider();
        protected HorizontalSlider joystickUIMouseSensitivitySlider = new HorizontalSlider();
        protected HorizontalSlider joystickMovementThresholdSlider = new HorizontalSlider();
        protected HorizontalSlider joystickDeadzoneSlider = new HorizontalSlider();

        protected Checkbox enableControllerCheckbox = new Checkbox();
        protected Checkbox invertMovementHorizontalCheckbox = new Checkbox();
        protected Checkbox invertMovementVerticalCheckbox = new Checkbox();
        protected Checkbox invertLookHorizontalCheckbox = new Checkbox();
        protected Checkbox invertLookVerticalCheckbox = new Checkbox();

        List<Button> buttonGroup = new List<Button>();
        bool waitingForInput = false;

        //holds both left/right click and axisActions
        private static Dictionary<string, string> UnsavedKeybindDict = new Dictionary<string, string>();
        
        //holds other unsaved settings
        private static Dictionary<String, object> UnsavedSettingsDict = new Dictionary<String, object>();

        #endregion

        #region Constructors

        public DaggerfallJoystickControlsWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            : base(uiManager, previousWindow)
        {
        }

        #endregion

        #region Unity
        
        public override void Update()
        {
            base.Update();

            if (!AllowCancel && !waitingForInput && InputManager.Instance.GetBackButtonDown())
            {
                ShowMultipleAssignmentsMessage();
            }

            // Sync controller enabling to current setting
            // This is a special realtime setting as controller enabling can change at any time, even on the window itself
            if (enableControllerCheckbox != null)
            {
                enableControllerCheckbox.IsChecked = DaggerfallUnity.Settings.EnableController;
                InputManager.Instance.EnableController = DaggerfallUnity.Settings.EnableController;
            }
        }

        #endregion

        #region Setup

        protected override void Setup()
        {
            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Main panel
            Vector2 mainPanelSize = new Vector2(318, 170);
            mainPanel = new Panel();
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.Size = mainPanelSize;
            mainPanel.Outline.Enabled = true;
            SetBackground(mainPanel, mainPanelBackgroundColor, "joystickControlsMainPanelBackgroundColor");
            NativePanel.Components.Add(mainPanel);

            // Title label
            titleLabel = new TextLabel();
            titleLabel.ShadowPosition = Vector2.zero;
            titleLabel.Position = new Vector2(4, 4);
            titleLabel.Text = "Configure Joystick Controls";
            titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.Components.Add(titleLabel);

            // Continue button
            continueButton.Label.Text = "CONTINUE";
            continueButton.Size = new Vector2(80, 10);
            continueButton.HorizontalAlignment = HorizontalAlignment.Right;
            continueButton.VerticalAlignment = VerticalAlignment.Bottom;
            SetBackground(continueButton, continueButtonBackgroundColor, "joystickControlsContinueButtonBackgroundColor");
            mainPanel.Components.Add(continueButton);

            enableControllerCheckbox = AddOption(20, 20, "Enable Controller", DaggerfallUnity.Settings.EnableController);
            enableControllerCheckbox.OnToggleState += EnableControllerCheckbox_OnToggleState;

            // keybind buttons
            SetupAxisKeybindButton(movementHorizontalAxisKeybindButton, InputManager.AxisActions.MovementHorizontal, 20, 40);
            invertMovementHorizontalCheckbox =  AddOption(63, 60, "Invert", InputManager.Instance.GetAxisActionInversion(InputManager.AxisActions.MovementHorizontal));
            
            SetupAxisKeybindButton(movementVerticalAxisKeybindButton,   InputManager.AxisActions.MovementVertical, 20, 80);
            invertMovementVerticalCheckbox =    AddOption(63, 100, "Invert", InputManager.Instance.GetAxisActionInversion(InputManager.AxisActions.MovementVertical));
            
            SetupAxisKeybindButton(lookHorizontalAxisKeybindButton,     InputManager.AxisActions.CameraHorizontal, 115, 40);
            invertLookHorizontalCheckbox =      AddOption(158, 60, "Invert", InputManager.Instance.GetAxisActionInversion(InputManager.AxisActions.CameraHorizontal));

            SetupAxisKeybindButton(lookVerticalAxisKeybindButton,       InputManager.AxisActions.CameraVertical, 115, 80);
            invertLookVerticalCheckbox =        AddOption(158, 100, "Invert", InputManager.Instance.GetAxisActionInversion(InputManager.AxisActions.CameraVertical));

            SetupUIKeybindButton(leftClickKeybindButton, leftClickString, 210, 40);
            SetupUIKeybindButton(middleClickKeybindButton, middleClickString, 210, 60);
            SetupUIKeybindButton(rightClickKeybindButton, rightClickString, 210, 80);
            SetupUIKeybindButton(backKeybindButton, backString, 210, 100);

            joystickCameraSensitivitySlider = CreateSlider("Look Sensitivity", 15, 120, 0.1f, 4.0f, DaggerfallUnity.Settings.JoystickLookSensitivity);

            joystickUIMouseSensitivitySlider = CreateSlider("UI Mouse Sensitivity", 115, 120, 0.1f, 5.0f, DaggerfallUnity.Settings.JoystickCursorSensitivity);

            joystickMovementThresholdSlider = CreateSlider("Maximum Movement Threshold", 215, 120, 0.0f, 1.0f, DaggerfallUnity.Settings.JoystickMovementThreshold);

            joystickDeadzoneSlider = CreateSlider("Deadzone", 15, 140, 0.0f, 0.9f, DaggerfallUnity.Settings.JoystickDeadzone);

            continueButton.OnMouseClick += ContinueButton_OnMouseClick;
        }

        #endregion

        #region Overrides

        public override void OnPop()
        {
            UpdateUnsavedSettingsToControls();
        }

        public override void OnPush()
        {
            OnReturn();
        }

        public override void OnReturn()
        {
            UpdateControlsToUnsavedSettings();
            CheckDuplicates();
        }

        #endregion

        #region Public Static Methods

        //ResetUnsavedSettings and SaveSettings are essentially inverses of each other

        public static void ResetUnsavedSettings()
        {
            UnsavedKeybindDict[leftClickString] = InputManager.Instance.GetKeyString(InputManager.Instance.GetJoystickUIBinding(InputManager.JoystickUIActions.LeftClick));
            UnsavedKeybindDict[middleClickString] = InputManager.Instance.GetKeyString(InputManager.Instance.GetJoystickUIBinding(InputManager.JoystickUIActions.MiddleClick));
            UnsavedKeybindDict[rightClickString] = InputManager.Instance.GetKeyString(InputManager.Instance.GetJoystickUIBinding(InputManager.JoystickUIActions.RightClick));
            UnsavedKeybindDict[backString] = InputManager.Instance.GetKeyString(InputManager.Instance.GetJoystickUIBinding(InputManager.JoystickUIActions.Back));

            foreach (InputManager.AxisActions a in Enum.GetValues(typeof(InputManager.AxisActions)))
                UnsavedKeybindDict[a.ToString()] = InputManager.Instance.GetAxisBinding(a);

            UnsavedSettingsDict["EnableController"] = DaggerfallUnity.Settings.EnableController;

            UnsavedSettingsDict["JoystickLookSensitivity"] = DaggerfallUnity.Settings.JoystickLookSensitivity;
            UnsavedSettingsDict["JoystickCursorSensitivity"] = DaggerfallUnity.Settings.JoystickCursorSensitivity;
            UnsavedSettingsDict["JoystickMovementThreshold"] = DaggerfallUnity.Settings.JoystickMovementThreshold;
            UnsavedSettingsDict["JoystickDeadzone"] = DaggerfallUnity.Settings.JoystickDeadzone;

            UnsavedSettingsDict["InvertMovementHorizontal"] = InputManager.Instance.GetAxisActionInversion(InputManager.AxisActions.MovementHorizontal);
            UnsavedSettingsDict["InvertMovementVertical"] = InputManager.Instance.GetAxisActionInversion(InputManager.AxisActions.MovementVertical);
            UnsavedSettingsDict["InvertCameraHorizontal"] = InputManager.Instance.GetAxisActionInversion(InputManager.AxisActions.CameraHorizontal);
            UnsavedSettingsDict["InvertCameraVertical"] = InputManager.Instance.GetAxisActionInversion(InputManager.AxisActions.CameraVertical);
        }

        public static void SaveSettings()
        {
            DaggerfallUnity.Settings.EnableController = (bool)UnsavedSettingsDict["EnableController"];

            DaggerfallUnity.Settings.JoystickLookSensitivity = (float)UnsavedSettingsDict["JoystickLookSensitivity"];
            DaggerfallUnity.Settings.JoystickCursorSensitivity = (float)UnsavedSettingsDict["JoystickCursorSensitivity"];
            DaggerfallUnity.Settings.JoystickMovementThreshold = (float)UnsavedSettingsDict["JoystickMovementThreshold"];
            DaggerfallUnity.Settings.JoystickDeadzone = (float)UnsavedSettingsDict["JoystickDeadzone"];

            InputManager.Instance.SetAxisActionInversion(InputManager.AxisActions.MovementHorizontal, (bool)UnsavedSettingsDict["InvertMovementHorizontal"]);
            InputManager.Instance.SetAxisActionInversion(InputManager.AxisActions.MovementVertical, (bool)UnsavedSettingsDict["InvertMovementVertical"]);
            InputManager.Instance.SetAxisActionInversion(InputManager.AxisActions.CameraHorizontal, (bool)UnsavedSettingsDict["InvertCameraHorizontal"]);
            InputManager.Instance.SetAxisActionInversion(InputManager.AxisActions.CameraVertical, (bool)UnsavedSettingsDict["InvertCameraVertical"]);

            DaggerfallUnity.Settings.SaveSettings();
            GameManager.Instance.StartGameBehaviour.ApplyStartSettings();

            SaveAllKeybindValues();
            InputManager.Instance.SaveKeyBinds();
            ResetUnsavedSettings();
        }

        #endregion

        #region Private Methods

        //for "reset defaults" overload
        private void SetupKeybindButton(Button button, string action)
        {
            if (action == leftClickString || action == middleClickString || action == rightClickString || action == backString)
            {
                var code = InputManager.Instance.ParseKeyCodeString(UnsavedKeybindDict[action]);
                button.Label.Text = ControlsConfigManager.Instance.GetButtonText(code);

                button.ToolTip = defaultToolTip;
                button.SuppressToolTip = button.Label.Text != ControlsConfigManager.ElongatedButtonText;
                button.ToolTipText = ControlsConfigManager.Instance.GetButtonText(code, true);
            }
            else
                button.Label.Text = UnsavedKeybindDict[action];

            button.Label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
        }

        private void SetupUIKeybindButton(Button button, string text, int x, int y)
        {
            SetupKeybindButton(button, text, x, y);
            button.OnMouseClick += UIKeybindButton_OnMouseClick;
        }

        private void SetupAxisKeybindButton(Button button, InputManager.AxisActions action, int x, int y)
        {
            SetupKeybindButton(button, action.ToString(), x, y);
            button.OnMouseClick += AxisKeybindButton_OnMouseClick;
        }

        //for initialization
        private void SetupKeybindButton(Button button, string action, int x, int y)
        {
            Panel panel = new Panel();
            panel.Position = new Vector2(x, y);
            panel.Size = new Vector2(85, 15);

            Panel labelPanel = new Panel();
            labelPanel.Size = new Vector2(40, 10);
            labelPanel.Position = new Vector2(0, 0);
            labelPanel.HorizontalAlignment = HorizontalAlignment.Left;
            labelPanel.VerticalAlignment = VerticalAlignment.Middle;

            TextLabel label = new TextLabel();
            label.Position = new Vector2(0, 0);
            label.HorizontalAlignment = HorizontalAlignment.Right;
            label.VerticalAlignment = VerticalAlignment.Middle;
            label.ShadowPosition = Vector2.zero;

            label.Text =    action ==  InputManager.AxisActions.MovementHorizontal.ToString() ? "Movement H."
                            : action == InputManager.AxisActions.MovementVertical.ToString() ? "Movement V."
                            : action == InputManager.AxisActions.CameraHorizontal.ToString() ? "Camera H."
                            : action == InputManager.AxisActions.CameraVertical.ToString() ? "Camera V."
                            : action;

            label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;

            button.Name = action.ToString();
            button.Label.ShadowPosition = Vector2.zero;
            button.Label.TextScale = 0.9f;
            button.Size = new Vector2(43, 10);
            button.Position = new Vector2(43, 0);
            button.HorizontalAlignment = HorizontalAlignment.Right;
            button.VerticalAlignment = VerticalAlignment.Middle;

            SetBackground(button, keybindButtonBackgroundColor, "joystickControlsKeybindBackgroundColor");

            buttonGroup.Add(button);

            labelPanel.Components.Add(label);
            panel.Components.Add(labelPanel);
            panel.Components.Add(button);
            mainPanel.Components.Add(panel);

            SetupKeybindButton(button, action);
        }

        /// <summary>
        /// Add a slider with a numerical indicator.
        /// </summary>
        private HorizontalSlider CreateSlider(string text, int x, int y, float minValue, float maxValue, float startValue)
        {
            Panel sliderPanel = new Panel();
            sliderPanel.Position = new Vector2(x, y);
            sliderPanel.Size = new Vector2(70.0f, 45);

            TextLabel label = new TextLabel();
            label.Position = new Vector2(0, 0);
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Top;
            label.ShadowPosition = Vector2.zero;
            label.Text = text;

            sliderPanel.Components.Add(label);

            Action<HorizontalSlider> setIndicator = ((s) => s.SetIndicator(minValue, maxValue, startValue));
            HorizontalSlider slider = DaggerfallUI.AddSlider(new Vector2(0, 6), 70.0f, setIndicator, 0.9f, sliderPanel);

            mainPanel.Components.Add(sliderPanel);

            return slider;
        }

        Checkbox AddOption(float x, float y, string str, bool isChecked)
        {
            Checkbox checkbox = new Checkbox();
            checkbox.Label.Text = str;
            checkbox.IsChecked = isChecked;
            checkbox.Position = new Vector2(x, y);
            mainPanel.Components.Add(checkbox);

            return checkbox;
        }

        //from DaggerfallUnitySaveGameWindow
        void SetBackground(BaseScreenComponent panel, Color color, string textureName)
        {
            Texture2D tex;
            if (TextureReplacement.TryImportTexture(textureName, true, out tex))
            {
                panel.BackgroundTexture = tex;
                TextureReplacement.LogLegacyUICustomizationMessage(textureName);
            }
            else
                panel.BackgroundColor = color;
        }

        //entering the window
        private void UpdateControlsToUnsavedSettings()
        {
            SetupKeybindButton(leftClickKeybindButton, leftClickString);
            SetupKeybindButton(middleClickKeybindButton, middleClickString);
            SetupKeybindButton(rightClickKeybindButton, rightClickString);
            SetupKeybindButton(backKeybindButton, backString);
            SetupKeybindButton(movementHorizontalAxisKeybindButton, InputManager.AxisActions.MovementHorizontal.ToString());
            SetupKeybindButton(movementVerticalAxisKeybindButton, InputManager.AxisActions.MovementVertical.ToString());
            SetupKeybindButton(lookHorizontalAxisKeybindButton, InputManager.AxisActions.CameraHorizontal.ToString());
            SetupKeybindButton(lookVerticalAxisKeybindButton, InputManager.AxisActions.CameraVertical.ToString());

            joystickCameraSensitivitySlider.SetValue((float)UnsavedSettingsDict["JoystickLookSensitivity"]);
            joystickUIMouseSensitivitySlider.SetValue((float)UnsavedSettingsDict["JoystickCursorSensitivity"]);
            joystickMovementThresholdSlider.SetValue((float)UnsavedSettingsDict["JoystickMovementThreshold"]);
            joystickDeadzoneSlider.SetValue((float)UnsavedSettingsDict["JoystickDeadzone"]);

            enableControllerCheckbox.IsChecked = (bool)UnsavedSettingsDict["EnableController"];
            invertMovementHorizontalCheckbox.IsChecked = (bool)UnsavedSettingsDict["InvertMovementHorizontal"];
            invertMovementVerticalCheckbox.IsChecked = (bool)UnsavedSettingsDict["InvertMovementVertical"];
            invertLookHorizontalCheckbox.IsChecked = (bool)UnsavedSettingsDict["InvertCameraHorizontal"];
            invertLookVerticalCheckbox.IsChecked = (bool)UnsavedSettingsDict["InvertCameraVertical"];
        }

        //leaving the window
        private void UpdateUnsavedSettingsToControls()
        {
            //no need to do UnsavedKeybindDict here.. it's already set in the WaitForKeyPress method

            UnsavedSettingsDict["JoystickLookSensitivity"] = joystickCameraSensitivitySlider.GetValue();
            UnsavedSettingsDict["JoystickCursorSensitivity"] = joystickUIMouseSensitivitySlider.GetValue();
            UnsavedSettingsDict["JoystickMovementThreshold"] = joystickMovementThresholdSlider.GetValue();
            UnsavedSettingsDict["JoystickDeadzone"] = joystickDeadzoneSlider.GetValue();

            UnsavedSettingsDict["EnableController"] = enableControllerCheckbox.IsChecked;
            UnsavedSettingsDict["InvertMovementHorizontal"] = invertMovementHorizontalCheckbox.IsChecked;
            UnsavedSettingsDict["InvertMovementVertical"] = invertMovementVerticalCheckbox.IsChecked;
            UnsavedSettingsDict["InvertCameraHorizontal"] = invertLookHorizontalCheckbox.IsChecked;
            UnsavedSettingsDict["InvertCameraVertical"] = invertLookVerticalCheckbox.IsChecked;
        }

        private void CheckDuplicates()
        {
            IEnumerable<String> keyList = UnsavedKeybindDict.Select(kv => kv.Value);

            var dupes = ControlsConfigManager.Instance.GetDuplicates(keyList);

            AllowCancel = dupes.Count == 0;

            foreach (Button keybindButton in buttonGroup)
            {
                string text;
                //The left/right click buttons have tooltips, axis buttons do not
                if (!string.IsNullOrEmpty(keybindButton.ToolTipText))
                    text = UnsavedKeybindDict[keybindButton.Name];
                else
                    text = keybindButton.Label.Text;

                if (dupes.Contains(text))
                    keybindButton.Label.TextColor = new Color(1, 0, 0);
                else
                    keybindButton.Label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            }
        }

        private void SetWaitingForInput(bool b)
        {
            waitingForInput = b;
            AllowCancel = !b;
        }

        private void ShowMultipleAssignmentsMessage()
        {
            DaggerfallMessageBox multipleAssignmentsBox = new DaggerfallMessageBox(uiManager, this);
            multipleAssignmentsBox.SetText(TextManager.Instance.GetLocalizedText("multipleAssignments"));
            multipleAssignmentsBox.ClickAnywhereToClose = true;
            multipleAssignmentsBox.Show();
        }

        #endregion

        #region Private Static Methods

        private static void SaveAllKeybindValues()
        {
            foreach(var action in UnsavedKeybindDict.Keys)
            {
                if (action == leftClickString || action == middleClickString || action == rightClickString || action == backString)
                {
                    KeyCode code = InputManager.Instance.ParseKeyCodeString(UnsavedKeybindDict[action]);
                    InputManager.JoystickUIActions uiAction = InputManager.JoystickUIActions.LeftClick;

                    if (action == middleClickString)
                        uiAction = InputManager.JoystickUIActions.MiddleClick;
                    else if (action == rightClickString)
                        uiAction = InputManager.JoystickUIActions.RightClick;
                    else if (action == backString)
                        uiAction = InputManager.JoystickUIActions.Back;

                    KeyCode curCode = InputManager.Instance.GetJoystickUIBinding(uiAction);

                    if (curCode != code)
                    {
                        InputManager.Instance.SetJoystickUIBinding(code, uiAction);
                        Debug.LogFormat("Bound joystick {0} with Code {1}", action, code.ToString());
                    }
                }
                else
                {
                    InputManager.AxisActions axisAction = (InputManager.AxisActions)Enum.Parse(typeof(InputManager.AxisActions), action);
                    string code = UnsavedKeybindDict[action];

                    // Rebind only if new code is different
                    string curCode = InputManager.Instance.GetAxisBinding(axisAction);
                    if (curCode != code)
                    {
                        InputManager.Instance.SetAxisBinding(code, axisAction);
                        Debug.LogFormat("Bound AxisAction {0} with Code {1}", axisAction, code.ToString());
                    }
                }
            }
        }

        #endregion

        #region Event Handlers

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

        private void EnableControllerCheckbox_OnToggleState()
        {
            // Immediately toggle controller
            DaggerfallUnity.Settings.EnableController = enableControllerCheckbox.IsChecked;
        }

        private void UIKeybindButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            KeybindButton_OnMouseClick(sender, position, false);
        }

        private void AxisKeybindButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            KeybindButton_OnMouseClick(sender, position, true);
        }

        private void KeybindButton_OnMouseClick(BaseScreenComponent sender, Vector2 position, bool axisAction)
        {
            if (waitingForInput)
                return;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            Button thisKeybindButton = (Button)sender;

            InputManager.Instance.StartCoroutine(WaitForKeyPress(thisKeybindButton, axisAction));
        }

        IEnumerator WaitForKeyPress(Button button, bool isAxisAction)
        {
            string currentLabel = button.Label.Text;
            KeyCode code = KeyCode.None;

            button.Label.Text = "";
            yield return new WaitForSecondsRealtime(0.05f);

            while ((!isAxisAction && (code = InputManager.Instance.GetAnyKeyDownIgnoreAxisBinds(true)) == KeyCode.None)
                || (isAxisAction && (code = InputManager.Instance.GetAnyKeyDown(true)) == KeyCode.None))
            {
                SetWaitingForInput(true);
                yield return null;
            }

            SetWaitingForInput(false);

            if (code != KeyCode.None)
            {
                if (InputManager.Instance.ReservedKeys.FirstOrDefault(x => x == code) == KeyCode.None)
                {
                    if (isAxisAction)
                    {
                        var text = InputManager.Instance.AxisKeyCodeToInputAxis((int)code);
                        if (!string.IsNullOrEmpty(text))
                        {
                            button.Label.Text = text;

                            string actionKey = button.Name;
                            UnsavedKeybindDict[actionKey] = text;
                        }
                        else
                        {
                            button.Label.Text = currentLabel;
                        }
                    }
                    else
                    {
                        button.Label.Text = ControlsConfigManager.Instance.GetButtonText(code);
                        button.SuppressToolTip = button.Label.Text != ControlsConfigManager.ElongatedButtonText;
                        button.ToolTipText = ControlsConfigManager.Instance.GetButtonText(code, true);

                        string actionKey = button.Name;
                        UnsavedKeybindDict[actionKey] = InputManager.Instance.GetKeyString(code);
                    }

                    CheckDuplicates();
                }
                else
                {
                    button.Label.Text = currentLabel;
                }
            }
        }

        #endregion
    }
}