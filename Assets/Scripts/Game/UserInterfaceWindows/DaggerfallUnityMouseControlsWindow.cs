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

        Color mainPanelBackgroundColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        Color keybindButtonBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        Color continueButtonBackgroundColor = new Color(0.5f, 0.0f, 0.0f, 1.0f);

        Panel mainPanel;
        TextLabel titleLabel;
        Button escapeKeybindButton = new Button();
        Button consoleKeybindButton = new Button();
        Button screenshotKeybindButton = new Button();
        Button quickSaveKeybindButton = new Button();
        Button quickLoadKeybindButton = new Button();
        Button autoRunKeybindButton = new Button();
        HorizontalSlider mouseSensitivitySlider;
        HorizontalSlider weaponSensitivitySlider;
        Checkbox moveSpeedCheckbox;
        Checkbox invertMouseVerticalCheckbox;
        Checkbox mouseSmoothingCheckbox;
        Checkbox clickToAttackCheckbox;
        Checkbox bowDrawbackCheckbox;
        Checkbox toggleSneakCheckbox;
        TextBox weaponAttackThresholdTextbox;

        List<Button> buttonGroup = new List<Button>();

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
            Button continueButton = new Button();
            continueButton.Label.Text = "CONTINUE";
            continueButton.Size = new Vector2(80, 10);
            continueButton.HorizontalAlignment = HorizontalAlignment.Right;
            continueButton.VerticalAlignment = VerticalAlignment.Bottom;
            SetBackground(continueButton, continueButtonBackgroundColor, "advancedControlsContinueButtonBackgroundColor");
            mainPanel.Components.Add(continueButton);

            // keybind buttons
            SetupKeybindButton(escapeKeybindButton, InputManager.Actions.Escape, 20, 20);
            SetupKeybindButton(autoRunKeybindButton, InputManager.Actions.AutoRun, 20, 40);
            SetupKeybindButton(consoleKeybindButton, InputManager.Actions.ToggleConsole, 115, 20);
            SetupKeybindButton(screenshotKeybindButton, InputManager.Actions.PrintScreen, 115, 40);
            SetupKeybindButton(quickSaveKeybindButton, InputManager.Actions.QuickSave, 210, 20);
            SetupKeybindButton(quickLoadKeybindButton, InputManager.Actions.QuickLoad, 210, 40);

            mouseSensitivitySlider = CreateSlider("Mouse Look Sensitivity", 15, 80, 0.1f, 8.0f, DaggerfallUnity.Settings.MouseLookSensitivity);
            invertMouseVerticalCheckbox = AddOption(20, 100, "Invert Look-Y", DaggerfallUnity.Settings.InvertMouseVertical);
            mouseSmoothingCheckbox = AddOption(20, 110, "Mouse Smoothing", DaggerfallUnity.Settings.MouseLookSmoothing);
            moveSpeedCheckbox = AddOption(20, 120, "Movement Acceleration", DaggerfallUnity.Settings.MovementAcceleration);

            weaponSensitivitySlider = CreateSlider("Mouse Weapon Sensitivity", 115, 80, 0.1f, 10.0f, DaggerfallUnity.Settings.WeaponSensitivity);
            clickToAttackCheckbox = AddOption(115, 100, "Click to Attack", DaggerfallUnity.Settings.ClickToAttack);
            bowDrawbackCheckbox = AddOption(115, 110, "Bows - draw and release", DaggerfallUnity.Settings.BowDrawback);
            toggleSneakCheckbox = AddOption(115, 120, "Toggle Sneak", DaggerfallUnity.Settings.ToggleSneak);

            weaponAttackThresholdTextbox = AddTextbox("Mouse Weapon Attack Threshold", 215, 80, DaggerfallUnity.Settings.WeaponAttackThreshold.ToString());


            continueButton.OnMouseClick += ContinueButton_OnMouseClick;
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

        //for "reset defaults" overload
        //**might delete this, since reset defaults is in the main controls window
        private void SetupKeybindButton(Button button, InputManager.Actions action)
        {
            button.Label.Text = DaggerfallControlsWindow.UnsavedKeybindDict[action];
            button.Label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
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
            label.Text = action == InputManager.Actions.Escape ? "Pause"
                                   : action == InputManager.Actions.ToggleConsole ? "Console"
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

            buttonGroup.Add(button);

            labelPanel.Components.Add(label);
            panel.Components.Add(labelPanel);
            panel.Components.Add(button);
            mainPanel.Components.Add(panel);

            SetupKeybindButton(button, action);
        }

        private void UpdateKeybindButtons()
        {
            SetupKeybindButton(escapeKeybindButton, InputManager.Actions.Escape);
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
            IEnumerable<String> keyList = DaggerfallControlsWindow.UnsavedKeybindDict.Select(kv => kv.Value);

            var dupes = DaggerfallControlsWindow.GetDuplicates(keyList);

            foreach (Button keybindButton in buttonGroup)
            {
                if (dupes.Contains(keybindButton.Label.Text))
                    keybindButton.Label.TextColor = new Color(1, 0, 0);
                else
                    keybindButton.Label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
            }
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
            DaggerfallUnity.Settings.WeaponSensitivity = weaponSensitivitySlider.GetValue();
            DaggerfallUnity.Settings.MovementAcceleration = moveSpeedCheckbox.IsChecked;
            DaggerfallUnity.Settings.InvertMouseVertical = invertMouseVerticalCheckbox.IsChecked;
            DaggerfallUnity.Settings.MouseLookSmoothing = mouseSmoothingCheckbox.IsChecked;
            DaggerfallUnity.Settings.ClickToAttack = clickToAttackCheckbox.IsChecked;
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

        IEnumerator WaitForKeyPress(Button button)
        {
            yield return DaggerfallControlsWindow.WaitForKeyPress(button, CheckDuplicates, SetWaitingForInput);
        }

        #endregion

    }
}