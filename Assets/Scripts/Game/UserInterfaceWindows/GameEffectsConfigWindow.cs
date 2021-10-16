// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2021 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class GameEffectsConfigWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        protected Vector2 mainPanelSize = new Vector2(200, 141);
        protected readonly Vector2 settingsStartPos = new Vector2(2, 10);
        protected Rect effectListPanelRect = new Rect(2, 2, 60, 128);
        protected Rect effectPanelRect = new Rect(63, 2, 135, 137);
        protected Rect resetDefaultsButtonRect = new Rect(2, 131, 60, 8);
        protected Rect aboutPanelRect = new Rect(0, 0, 135, 6);
        protected int yIncrement = 12;

        #endregion

        #region UI Controls

        protected Panel mainPanel = new Panel();
        protected ListBox effectList = new ListBox();
        protected Button resetDefaultsButton = new Button();

        protected Color mainPanelBackgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.2f);
        protected Color effectListBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);
        protected Color effectListTextColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        protected Color effectPanelBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);
        protected Color aboutPanelBackgroundColor = new Color(0.4f, 0.4f, 0.4f, 0.65f);
        protected Color aboutPanelTextColor = Color.cyan;
        protected Color aboutPanelTextShadowColor = Color.cyan / 2;

        #endregion

        #region Fields

        // Keys used for text and list item selection
        const string antialiasingKey = "antialiasing";
        const string ambientOcclusionKey = "ambientOcclusion";
        const string bloomKey = "bloom";
        const string motionBlurKey = "motionBlur";
        const string vignetteKey = "vignette";
        const string depthOfFieldKey = "depthOfField";

        Dictionary<string, Panel> effectPanelDict = new Dictionary<string, Panel>();

        #endregion

        #region Constructors

        public GameEffectsConfigWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Show world while configuring postprocessing settings
            ParentPanel.BackgroundColor = Color.clear;

            // Main panel
            bool largeHUDEnabled = DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.Enabled;
            mainPanel.HorizontalAlignment = HorizontalAlignment.Right;
            mainPanel.VerticalAlignment = (largeHUDEnabled) ? VerticalAlignment.Top : VerticalAlignment.Middle; // Top-align when large HUD enabled to avoid overlap
            mainPanel.Size = mainPanelSize;
            mainPanel.Outline.Enabled = true;
            mainPanel.BackgroundColor = mainPanelBackgroundColor;
            NativePanel.Components.Add(mainPanel);

            // Effect list
            effectList.Position = effectListPanelRect.position;
            effectList.Size = effectListPanelRect.size;
            effectList.TextColor = effectListTextColor;
            effectList.BackgroundColor = effectListBackgroundColor;
            effectList.ShadowPosition = Vector2.zero;
            effectList.RowsDisplayed = 16;
            effectList.OnSelectItem += EffectList_OnSelectItem;
            mainPanel.Components.Add(effectList);
            AddEffects();

            // Reset page defaults button
            resetDefaultsButton.Position = resetDefaultsButtonRect.position;
            resetDefaultsButton.Size = resetDefaultsButtonRect.size;
            resetDefaultsButton.BackgroundColor = Color.gray;
            resetDefaultsButton.Label.TextScale = 0.75f;
            resetDefaultsButton.Label.Text = TextManager.Instance.GetLocalizedText("setPageDefaults");
            resetDefaultsButton.OnMouseClick += ResetDefaultsButton_OnMouseClick;
            mainPanel.Components.Add(resetDefaultsButton);

            IsSetup = true;
            RefreshSettingsPages();
        }

        protected void RefreshSettingsPages()
        {
            AntialiasingReadSettings();
        }

        public override void OnPush()
        {
            base.OnPush();

            if (IsSetup)
                RefreshSettingsPages();
        }

        public override void OnPop()
        {
            base.OnPop();

            DaggerfallUnity.Settings.SaveSettings();
        }

        #endregion

        #region Private Methods

        void AddEffects()
        {
            effectList.ClearItems();
            effectPanelDict.Clear();
            AddEffectPanel(antialiasingKey, AntialiasingSettings);
            AddEffectPanel(ambientOcclusionKey, AmbientOcclusionSettings);
            AddEffectPanel(bloomKey, BloomSettings);
            AddEffectPanel(motionBlurKey, MotionBlurSettings);
            AddEffectPanel(vignetteKey, VignetteSettings);
            AddEffectPanel(depthOfFieldKey, DepthOfFieldSettings);

            effectList.SelectedIndex = 0;
        }

        void AddEffectPanel(string key, Action settingsMethod)
        {
            // Add panel to home effect settings
            Panel panel = new Panel();
            panel.Position = effectPanelRect.position;
            panel.Size = effectPanelRect.size;
            panel.BackgroundColor = effectPanelBackgroundColor;
            panel.Enabled = false;
            panel.Tag = key;
            mainPanel.Components.Add(panel);

            // Add item to select panel from list
            effectPanelDict.Add(key, panel);
            effectList.AddItem(TextManager.Instance.GetLocalizedText(key), -1, key);

            // Configure settings for this effect panel
            if (settingsMethod != null)
                settingsMethod();
        }

        private void EffectList_OnSelectItem()
        {
            // Do nothing if event fires before setup or nothing actually selected
            if (effectPanelDict.Count == 0)
                return;

            // Enable just the panel selected
            string selectedKey = effectList.GetItem(effectList.SelectedIndex).tag as string;
            foreach (var item in effectPanelDict.Values)
            {
                string tag = item.Tag as string;
                item.Enabled = tag == selectedKey;
            }
        }

        private void ResetDefaultsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // TEMP: Going to split each effect page into their own class and call virtual methods instead
            string selectedKey = effectList.GetItem(effectList.SelectedIndex).tag as string;
            if (selectedKey == "antialiasing")
                AntialiasingSetDefaults();
        }

        void AddTipPanel(Panel parent, string text)
        {
            Panel panel = new Panel();
            panel.EnableBorder = true;
            panel.BackgroundColor = aboutPanelBackgroundColor;
            panel.Position = aboutPanelRect.position;
            panel.Size = aboutPanelRect.size;
            panel.VerticalAlignment = VerticalAlignment.Top;
            parent.Components.Add(panel);

            TextLabel label = new TextLabel();
            label.Text = text;
            label.TextColor = aboutPanelTextColor;
            label.ShadowColor = aboutPanelTextShadowColor;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Middle;
            label.TextScale = 0.75f;
            panel.Components.Add(label);
        }

        TextLabel AddLabel(Panel parent, string text, ref Vector2 position)
        {
            TextLabel label = new TextLabel();
            label.Text = text;
            label.Position = position;
            parent.Components.Add(label);

            return label;
        }

        Checkbox AddCheckbox(Panel parent, string label, ref Vector2 position)
        {
            Checkbox check = new Checkbox();
            check.Label.Text = label;
            check.IsChecked = false;
            check.Position = position;
            parent.Components.Add(check);
            position.y += yIncrement;

            return check;
        }

        HorizontalSlider AddSlider(Panel parent, string label, int count, ref Vector2 position)
        {
            const int sliderWidth = 70;
            const int sliderHeight = 6;

            // Slider label
            AddLabel(parent, label, ref position);
            position.y += 6;

            // Slider
            HorizontalSlider slider = new HorizontalSlider();
            slider.Size = new Vector2(sliderWidth, sliderHeight);
            slider.Position = position;
            slider.BackgroundColor = Color.black;
            slider.TotalUnits = count;
            slider.DisplayUnits = count;
            slider.ScrollIndex = 0;
            parent.Components.Add(slider);

            position.y += yIncrement;

            return slider;
        }

        void StyleIndicator(HorizontalSlider slider)
        {
            slider.IndicatorOffset = 2;
            slider.Indicator.TextColor = Color.white;
            slider.Indicator.ShadowPosition = Vector2.zero;
        }

        #endregion

        #region Antialising Settings

        HorizontalSlider antialiasingMethodSlider;
        Checkbox fxaaFastMostCheckbox;
        HorizontalSlider smaaQualitySlider;
        HorizontalSlider taaSharpnessSlider;

        protected void AntialiasingSettings()
        {
            Vector2 pos = settingsStartPos;
            Panel parent = effectPanelDict[antialiasingKey];

            // About this effect
            AddTipPanel(parent, TextManager.Instance.GetLocalizedText("antialiasingTip"));

            // Method slider
            string[] antiAliasingMethods = new string[]
            {
                TextManager.Instance.GetLocalizedText("none"),
                TextManager.Instance.GetLocalizedText("fxaa"),
                TextManager.Instance.GetLocalizedText("smaa"),
                TextManager.Instance.GetLocalizedText("taa")
            };
            antialiasingMethodSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("method"), antiAliasingMethods.Length, ref pos);
            antialiasingMethodSlider.OnScroll += AntialiasingMethodSlider_OnScroll;
            antialiasingMethodSlider.SetIndicator(antiAliasingMethods, DaggerfallUnity.Settings.AntialiasingMethod);
            StyleIndicator(antialiasingMethodSlider);

            // FXAA Fast Mode toggle
            fxaaFastMostCheckbox = AddCheckbox(parent, TextManager.Instance.GetLocalizedText("fxaaFastMode"), ref pos);
            fxaaFastMostCheckbox.OnToggleState += FxaaFastMostCheckbox_OnToggleState;

            // SMAA Quality slider
            string[] smaaQuality = new string[]
            {
                TextManager.Instance.GetLocalizedText("low"),
                TextManager.Instance.GetLocalizedText("medium"),
                TextManager.Instance.GetLocalizedText("high")
            };
            smaaQualitySlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("smaaQuality"), smaaQuality.Length, ref pos);
            smaaQualitySlider.OnScroll += SmaaQualitySlider_OnScroll;
            smaaQualitySlider.SetIndicator(smaaQuality, DaggerfallUnity.Settings.AntialiasingSMAAQuality);
            StyleIndicator(smaaQualitySlider);

            // TAA Sharpness slider
            taaSharpnessSlider = AddSlider(parent, TextManager.Instance.GetLocalizedText("taaSharpness"), 30, ref pos);
            taaSharpnessSlider.OnScroll += TaaSharpnessSlider_OnScroll;
            taaSharpnessSlider.SetIndicator(0.0f, 3.0f, DaggerfallUnity.Settings.AntialiasingTAASharpness);
            StyleIndicator(taaSharpnessSlider);
        }

        void AntialiasingSetDefaults()
        {
            Debug.Log("AA set defaults");
        }

        void AntialiasingReadSettings()
        {
            antialiasingMethodSlider.ScrollIndex = DaggerfallUnity.Settings.AntialiasingMethod;
            fxaaFastMostCheckbox.IsChecked = DaggerfallUnity.Settings.AntialiasingFXAAFastMode;
            smaaQualitySlider.ScrollIndex = DaggerfallUnity.Settings.AntialiasingSMAAQuality;
            taaSharpnessSlider.Value = Mathf.RoundToInt(DaggerfallUnity.Settings.AntialiasingTAASharpness * 10);
        }

        private void AntialiasingMethodSlider_OnScroll()
        {
            DaggerfallUnity.Settings.AntialiasingMethod = antialiasingMethodSlider.ScrollIndex;
            GameManager.Instance.StartGameBehaviour.DeployGameEffectSettings();
        }

        private void FxaaFastMostCheckbox_OnToggleState()
        {
            DaggerfallUnity.Settings.AntialiasingFXAAFastMode = fxaaFastMostCheckbox.IsChecked;
            GameManager.Instance.StartGameBehaviour.DeployGameEffectSettings();
        }

        private void SmaaQualitySlider_OnScroll()
        {
            DaggerfallUnity.Settings.AntialiasingSMAAQuality = smaaQualitySlider.ScrollIndex;
            GameManager.Instance.StartGameBehaviour.DeployGameEffectSettings();
        }

        private void TaaSharpnessSlider_OnScroll()
        {
            DaggerfallUnity.Settings.AntialiasingTAASharpness = taaSharpnessSlider.ScrollIndex / 10f;
            GameManager.Instance.StartGameBehaviour.DeployGameEffectSettings();
        }

        #endregion

        #region Ambient Occlusion Settings

        protected void AmbientOcclusionSettings()
        {
        }

        #endregion

        #region Bloom Settings

        protected void BloomSettings()
        {
        }

        #endregion

        #region Motion Blur Settings

        protected void MotionBlurSettings()
        {
        }

        #endregion

        #region Vignette Settings

        protected void VignetteSettings()
        {
        }

        #endregion

        #region Depth of Field Settings

        protected void DepthOfFieldSettings()
        {
        }

        #endregion
    }
}