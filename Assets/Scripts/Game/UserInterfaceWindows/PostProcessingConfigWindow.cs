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
    public class PostProcessingConfigWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        protected Vector2 mainPanelSize = new Vector2(200, 141);
        protected Rect effectListPanelSize = new Rect(2, 2, 60, 137);
        protected Rect effectPanelSize = new Rect(63, 2, 135, 137);

        #endregion

        #region UI Controls

        protected Panel mainPanel = new Panel();
        protected ListBox effectList = new ListBox();

        protected Color mainPanelBackgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.2f);
        protected Color effectListBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);
        protected Color effectListTextColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        protected Color effectPanelBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);

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

        public PostProcessingConfigWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
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
            effectList.Position = effectListPanelSize.position;
            effectList.Size = effectListPanelSize.size;
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
            AddEffectPanel(ambientOcclusionKey, null);
            AddEffectPanel(bloomKey, null);
            AddEffectPanel(motionBlurKey, null);
            AddEffectPanel(vignetteKey, null);
            AddEffectPanel(depthOfFieldKey, null);

            effectList.SelectedIndex = 0;
        }

        void AddEffectPanel(string key, Action settingsMethod)
        {
            // Add panel to home effect settings
            Panel panel = new Panel();
            panel.Position = effectPanelSize.position;
            panel.Size = effectPanelSize.size;
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

        #endregion

        #region Antialising Settings

        protected void AntialiasingSettings()
        {
        }

        #endregion

        #region Ambient Occlusion Settings
        #endregion

        #region Bloom Settings
        #endregion

        #region Motion Blur Settings
        #endregion

        #region Vignette Settings
        #endregion

        #region Depth of Field Settings
        #endregion
    }
}