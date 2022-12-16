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
    /// Implements mouse controls window.
    /// </summary>
    public class DaggerfallUnityMouseControlsWindow : DaggerfallPopupWindow
    {

        #region Fields

        protected Color mainPanelBackgroundColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        protected Color keybindButtonBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        protected Color continueButtonBackgroundColor = new Color(0.5f, 0.0f, 0.0f, 1.0f);

        protected Panel mainPanel;
        protected TextLabel titleLabel;
        protected Button pauseKeybindButton = new Button();
        protected Button consoleKeybindButton = new Button();
        protected Button screenshotKeybindButton = new Button();
        protected Button quickSaveKeybindButton = new Button();
        protected Button quickLoadKeybindButton = new Button();
        protected Button autoRunKeybindButton = new Button();
        protected HorizontalSlider mouseSensitivitySlider;
        //protected HorizontalSlider weaponSensitivitySlider;
        protected Checkbox moveSpeedCheckbox;
        protected Checkbox invertMouseVerticalCheckbox;
        protected HorizontalSlider mouseSmoothingSlider;
        protected HorizontalSlider weaponSwingModeSlider;
        protected Checkbox bowDrawbackCheckbox;
        protected Checkbox toggleSneakCheckbox;
        protected TextBox weaponAttackThresholdTextbox;
        protected Button continueButton;

        protected List<Button> buttonGroup = new List<Button>();

        bool waitingForInput = false;

        #endregion

        #region Constructors

        public DaggerfallUnityMouseControlsWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        #endregion

        #region Unity

        public override void Update()
        {
            base.Update();
        }

        #endregion

        #region Setup

        protected override void Setup()
        {
            DaggerfallWorkshop.Game.InputManager.OnSavedKeyBinds += OnUpdateValues;

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Main panel
            Vector2 mainPanelSize = new Vector2(318, 170);
            mainPanel = new Panel();
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.Size = mainPanelSize;
            mainPanel.Outline.Enabled = true;
            SetBackground(mainPanel, mainPanelBackgroundColor, "advancedControlsMainPanelBackgroundColor");
            NativePanel.Components.Add(mainPanel);

            // Title label
            titleLabel = new TextLabel();
            titleLabel.ShadowPosition = Vector2.zero;
            titleLabel.Position = new Vector2(4, 4);
            titleLabel.Text = "Configure Advanced Controls";
            titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.Components.Add(titleLabel);

            // Continue button
            continueButton = new Button();
            continueButton.Label.Text = "CONTINUE";
            continueButton.Size = new Vector2(80, 10);
            continueButton.HorizontalAlignment = HorizontalAlignment.Right;
            continueButton.VerticalAlignment = VerticalAlignment.Bottom;
            SetBackground(continueButton, continueButtonBackgroundColor, "advancedControlsContinueButtonBackgroundColor");
            mainPanel.Components.Add(continueButton);

            // keybind buttons
            SetupKeybindButton(pauseKeybindButton, InputManager.Actions.Escape, 20, 20);
            SetupKeybindButton(autoRunKeybindButton, InputManager.Actions.AutoRun, 20, 40);
            SetupKeybindButton(consoleKeybindButton, InputManager.Actions.ToggleConsole, 115, 20);
            SetupKeybindButton(screenshotKeybindButton, InputManager.Actions.PrintScreen, 115, 40);
            SetupKeybindButton(quickSaveKeybindButton, InputManager.Actions.QuickSave, 210, 20);
            SetupKeybindButton(quickLoadKeybindButton, InputManager.Actions.QuickLoad, 210, 40);

            mouseSmoothingSlider = CreateSlider("Mouse Look Smoothing", 120, 70, SettingsManager.GetMouseLookSmoothingStrength(DaggerfallUnity.Settings.MouseLookSmoothingFactor), SettingsManager.GetMouseLookSmoothingStrengths());
            mouseSensitivitySlider = CreateSlider("Mouse Look Sensitivity", 20, 70, 0.1f, 16.0f, DaggerfallUnity.Settings.MouseLookSensitivity);
            invertMouseVerticalCheckbox = AddOption(20, 120, "Invert Look-Y", DaggerfallUnity.Settings.InvertMouseVertical);
            moveSpeedCheckbox = AddOption(20, 130, "Movement Acceleration", DaggerfallUnity.Settings.MovementAcceleration);

            //weaponSensitivitySlider = CreateSlider("Mouse Weapon Sensitivity", 115, 80, 0.1f, 10.0f, DaggerfallUnity.Settings.WeaponSensitivity);
            weaponSwingModeSlider = CreateSlider("Weapon swing mode", 120, 90, DaggerfallUnity.Settings.WeaponSwingMode, "Vanilla", "Click", "Hold");
            bowDrawbackCheckbox = AddOption(115, 120, "Bows - draw and release", DaggerfallUnity.Settings.BowDrawback);
            toggleSneakCheckbox = AddOption(115, 130, "Toggle Sneak", DaggerfallUnity.Settings.ToggleSneak);

            weaponAttackThresholdTextbox = AddTextbox("Mouse Weapon Attack Threshold", 20, 90, DaggerfallUnity.Settings.WeaponAttackThreshold.ToString());

            continueButton.OnMouseClick += ContinueButton_OnMouseClick;

            CheckDuplicates();
        }

        #endregion

        #region Overrides

        public override void OnPush()
        {
            OnReturn();
        }

        public override void OnReturn()
        {
            UpdateKeybindButtons();
            CheckDuplicates();
        }

        #endregion

        #region Private methods

        private void SetupKeybindButton(Button button, InputManager.Actions action)
        {
            var code = ControlsConfigManager.Instance.GetUnsavedBindingKeyCode(action);
            button.Label.Text = ControlsConfigManager.Instance.GetButtonText(code);
            button.Label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;

            button.ToolTip = defaultToolTip;
            button.SuppressToolTip = button.Label.Text != ControlsConfigManager.ElongatedButtonText;
            button.ToolTipText = ControlsConfigManager.Instance.GetButtonText(code, true);
        }

        //for initialization
        private void SetupKeybindButton(Button button, InputManager.Actions action, int x, int y)
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

            //"ToggleConsole" is too long as a word when looking in non-SDF font view
            //"Screenshot" is a better word and is one letter less than "PrintScreen"
            label.Text = action == InputManager.Actions.ToggleConsole ? "Console"
                                   : action == InputManager.Actions.PrintScreen ? "Screenshot"
                                   : action.ToString();

            label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;

            button.Name = action.ToString();
            button.Label.ShadowPosition = Vector2.zero;
            button.Label.TextScale = 0.9f;
            button.Size = new Vector2(43, 10);
            button.Position = new Vector2(43, 0);
            button.HorizontalAlignment = HorizontalAlignment.Right;
            button.VerticalAlignment = VerticalAlignment.Middle;

            SetBackground(button, keybindButtonBackgroundColor, "advancedControlsKeybindBackgroundColor");
            button.OnMouseClick += KeybindButton_OnMouseClick;
            button.OnRightMouseClick += KeybindButton_OnMouseRightClick;

            buttonGroup.Add(button);

            labelPanel.Components.Add(label);
            panel.Components.Add(labelPanel);
            panel.Components.Add(button);
            mainPanel.Components.Add(panel);

            SetupKeybindButton(button, action);
        }

        private void UpdateKeybindButtons()
        {
            SetupKeybindButton(pauseKeybindButton, InputManager.Actions.Escape);
            SetupKeybindButton(consoleKeybindButton, InputManager.Actions.ToggleConsole);
            SetupKeybindButton(screenshotKeybindButton, InputManager.Actions.PrintScreen);
            SetupKeybindButton(quickSaveKeybindButton, InputManager.Actions.QuickSave);
            SetupKeybindButton(quickLoadKeybindButton, InputManager.Actions.QuickLoad);
            SetupKeybindButton(autoRunKeybindButton, InputManager.Actions.AutoRun);
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
        
        private HorizontalSlider CreateSlider(string text, int x, int y, int selected, params string[] choices)
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

            Action<HorizontalSlider> setIndicator = (s) => s.SetIndicator(choices, selected);
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

        private TextBox AddTextbox(string title, int x, int y, string text)
        {
            Panel panel = new Panel();
            panel.Position = new Vector2(x, y);
            panel.Size = new Vector2(100.0f, 20);

            TextLabel label = new TextLabel();
            label.Position = new Vector2(0, 0);
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Top;
            label.ShadowPosition = Vector2.zero;
            label.Text = title;

            panel.Components.Add(label);

            TextBox textBox = new TextBox();
            textBox.Position = new Vector2(0, 10);
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            //textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.FixedSize = true;
            textBox.Size = new Vector2(30, 6);
            textBox.MaxCharacters = 5;
            textBox.Cursor.Enabled = false;
            textBox.DefaultText = text;
            textBox.DefaultTextColor = Color.white;
            textBox.HasFocusOutlineColor = Color.white;
            textBox.LostFocusOutlineColor = Color.white;
            textBox.UseFocus = true;

            panel.Components.Add(textBox);

            mainPanel.Components.Add(panel);

            return textBox;
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

        private void CheckDuplicates()
        {
            ControlsConfigManager.Instance.CheckDuplicateKeyCodes(buttonGroup);
        }

        //a workaround solution to setting the 'waitingForInput' instance variable in a
        //static IEnumerator method. IEnumerator methods can't accept out/ref parameters
        private void SetWaitingForInput(bool b)
        {
            waitingForInput = b;
            AllowCancel = !b;
        }

        #endregion

        #region Event Handlers

        private void OnUpdateValues()
        {
            DaggerfallUnity.Settings.MouseLookSensitivity = mouseSensitivitySlider.GetValue();
            DaggerfallUnity.Settings.MouseLookSmoothingFactor = SettingsManager.GetMouseLookSmoothingFactor(mouseSmoothingSlider.ScrollIndex);
            //DaggerfallUnity.Settings.WeaponSensitivity = weaponSensitivitySlider.GetValue();
            DaggerfallUnity.Settings.MovementAcceleration = moveSpeedCheckbox.IsChecked;
            DaggerfallUnity.Settings.InvertMouseVertical = invertMouseVerticalCheckbox.IsChecked;
            DaggerfallUnity.Settings.WeaponSwingMode = weaponSwingModeSlider.ScrollIndex;
            DaggerfallUnity.Settings.BowDrawback = bowDrawbackCheckbox.IsChecked;
            DaggerfallUnity.Settings.ToggleSneak = toggleSneakCheckbox.IsChecked;

            float weaponAttackThresholdValue;
            if (float.TryParse(weaponAttackThresholdTextbox.Text, out weaponAttackThresholdValue))
                DaggerfallUnity.Settings.WeaponAttackThreshold = Mathf.Clamp(weaponAttackThresholdValue, 0.001f, 1.0f);

            DaggerfallUnity.Settings.SaveSettings();

            GameManager.Instance.StartGameBehaviour.ApplyStartSettings();
        }

        private void ContinueButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (waitingForInput)
                return;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CancelWindow();
        }

        private void KeybindButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (waitingForInput)
                return;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            Button thisKeybindButton = (Button)sender;

            InputManager.Instance.StartCoroutine(WaitForKeyPress(thisKeybindButton));
        }

        private void KeybindButton_OnMouseRightClick(BaseScreenComponent sender, Vector2 position)
        {
            if (waitingForInput || ((Button)sender).Label.Text == KeyCode.None.ToString())
                return;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);

            ControlsConfigManager.Instance.PromptRemoveKeybindMessage((Button)sender, CheckDuplicates);
        }

        IEnumerator WaitForKeyPress(Button button)
        {
            yield return DaggerfallControlsWindow.WaitForKeyPress(button, CheckDuplicates, SetWaitingForInput);
        }

        #endregion

    }
}
