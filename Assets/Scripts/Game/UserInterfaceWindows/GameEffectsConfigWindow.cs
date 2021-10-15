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
        protected readonly Vector2 settingsStartPos = new Vector2(2, 18);
        protected Rect effectListPanelRect = new Rect(2, 2, 60, 137);
        protected Rect effectPanelRect = new Rect(63, 2, 135, 137);
        protected Rect aboutPanelRect = new Rect(0, 0, 135, 6);
        protected int yIncrement = 12;

        #endregion

        #region UI Controls

        protected Panel mainPanel = new Panel();
        protected ListBox effectList = new ListBox();

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
            effectList.RowsDisplayed = 17;
            effectList.OnSelectItem += EffectList_OnSelectItem;
            mainPanel.Components.Add(effectList);
            AddEffects();
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
            foreach(var item in effectPanelDict.Values)
            {
                string tag = item.Tag as string;
                item.Enabled = tag == selectedKey;
            }
        }

        void AddTitle(Panel parent, string text)
        {
            TextLabel label = new TextLabel();
            label.Text = text;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Top;
            label.Font = DaggerfallUI.TitleFont;
            label.TextScale = 0.75f;
            parent.Components.Add(label);
        }

        void AddAboutPanel(Panel parent, string text)
        {
            Panel panel = new Panel();
            panel.EnableBorder = true;
            panel.BackgroundColor = aboutPanelBackgroundColor;
            panel.Position = aboutPanelRect.position;
            panel.Size = aboutPanelRect.size;
            panel.VerticalAlignment = VerticalAlignment.Bottom;
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
            const int indicatorOffset = 2;

            slider.IndicatorOffset = indicatorOffset;
            slider.Indicator.TextColor = Color.white;
            slider.Indicator.ShadowPosition = Vector2.zero;
        }

        #endregion

        #region Antialising Settings

        Checkbox antialiasingEnabledCheckbox;
        HorizontalSlider antialiasingMethodSlider;
        Checkbox fxaaFastMostCheckbox;
        HorizontalSlider smaaQualitySlider;
        HorizontalSlider taaSharpnessSlider;

        protected void AntialiasingSettings()
        {
            Vector2 pos = settingsStartPos;
            Panel parent = effectPanelDict[antialiasingKey];

            // About this effect
            AddTitle(parent, TextManager.Instance.GetLocalizedText("antialiasing"));
            AddAboutPanel(parent, TextManager.Instance.GetLocalizedText("antialiasingTip"));

            // Enable toggle
            antialiasingEnabledCheckbox = AddCheckbox(parent, "Enable", ref pos);
            antialiasingEnabledCheckbox.OnToggleState += antialiasingEnabledCheckbox_Toggle;

            // Method slider
            string[] antiAliasingMethods = new string[] { "FXAA", "SMAA", "TAA" };
            antialiasingMethodSlider = AddSlider(parent, "Method", antiAliasingMethods.Length, ref pos);
            antialiasingMethodSlider.SetIndicator(antiAliasingMethods, 0);
            StyleIndicator(antialiasingMethodSlider);

            // FXAA Fast Mode toggle
            fxaaFastMostCheckbox = AddCheckbox(parent, "FXAA Fast Mode", ref pos);
            fxaaFastMostCheckbox.OnToggleState += FxaaFastMostCheckbox_OnToggleState;

            // SMAA Quality slider
            string[] smaaQuality = new string[] { "Low", "Medium", "High" };
            smaaQualitySlider = AddSlider(parent, "SMAA Quality", smaaQuality.Length, ref pos);
            smaaQualitySlider.SetIndicator(smaaQuality, 0);
            StyleIndicator(smaaQualitySlider);

            // TAA Sharpness slider
            taaSharpnessSlider = AddSlider(parent, "TAA Sharpness", 30, ref pos);
            taaSharpnessSlider.SetIndicator(0.0f, 3.0f, 0.0f);
            StyleIndicator(taaSharpnessSlider);
        }

        protected void antialiasingEnabledCheckbox_Toggle()
        {
            DaggerfallUnity.Settings.AntialiasingEnabled = antialiasingEnabledCheckbox.IsChecked;
            if (antialiasingEnabledCheckbox.IsChecked)
                Debug.LogFormat("Toggle AA enable");
            else
                Debug.LogFormat("Toggle AA disable");
        }

        private void FxaaFastMostCheckbox_OnToggleState()
        {
            
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