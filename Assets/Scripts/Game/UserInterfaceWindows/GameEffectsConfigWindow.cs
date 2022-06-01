// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class GameEffectsConfigWindow : DaggerfallPopupWindow
    {
        protected Vector2 mainPanelSize = new Vector2(210, 150);
        protected Rect effectListPanelRect = new Rect(2, 2, 60, 137);
        protected Rect effectPanelRect = new Rect(63, 2, 145, 138);
        protected Rect buttonPanelRect = new Rect(63, 140, 145, 8);
        protected Rect resetDefaultsButtonRect = new Rect(2, 140, 60, 8);
        protected Rect doneButtonRect = new Rect(115, 0, 30, 8);

        protected Panel mainPanel = new Panel();
        protected Panel buttonPanel = new Panel();
        protected ListBox effectList = new ListBox();
        protected Button resetDefaultsButton = new Button();
        protected Button doneButton = new Button();

        protected Color mainPanelBackgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.2f);
        protected Color effectListBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);
        protected Color effectListTextColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        protected Color effectPanelBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);

        Dictionary<string, ConfigPageEntry> effectPagesDict = new Dictionary<string, ConfigPageEntry>();

        protected struct ConfigPageEntry
        {
            public Panel panel;
            public IGameEffectConfigPage page;
        }

        public GameEffectsConfigWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        protected override void Setup()
        {
            // Show world while configuring postprocessing settings
            ParentPanel.BackgroundColor = Color.clear;

            // Main panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Right;
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

            // Button panel (only for doneButton)
            buttonPanel.Position = buttonPanelRect.position;
            buttonPanel.Size = buttonPanelRect.size;
            buttonPanel.BackgroundColor = effectPanelBackgroundColor;
            mainPanel.Components.Add(buttonPanel);

            // Reset page defaults button
            resetDefaultsButton.Position = resetDefaultsButtonRect.position;
            resetDefaultsButton.Size = resetDefaultsButtonRect.size;
            resetDefaultsButton.BackgroundColor = Color.gray;
            resetDefaultsButton.Label.TextScale = 0.75f;
            resetDefaultsButton.Label.Text = TextManager.Instance.GetLocalizedText("setPageDefaults");
            resetDefaultsButton.OnMouseClick += ResetDefaultsButton_OnMouseClick;
            mainPanel.Components.Add(resetDefaultsButton);

            // Done button
            doneButton.Position = doneButtonRect.position;
            doneButton.Size = doneButtonRect.size;
            doneButton.BackgroundColor = Color.gray;
            doneButton.Label.TextScale = 0.75f;
            doneButton.Label.Text = TextManager.Instance.GetLocalizedText("doneButton");
            doneButton.OnMouseClick += DoneButton_OnMouseClick;
            buttonPanel.Components.Add(doneButton);

            IsSetup = true;

            AddCorePages();
            RaiseOnRegisterCustomPages();
            RefreshPageSettings();
        }

        protected void AddCorePages()
        {
            AddConfigPage(new RetroModeConfigPage());
            AddConfigPage(new ColorBoostConfigPage());
            AddConfigPage(new AntialiasingConfigPage());
            AddConfigPage(new AmbientOcclusionConfigPage());
            AddConfigPage(new BloomConfigPage());
            AddConfigPage(new MotionBlurConfigPage());
            AddConfigPage(new VignetteConfigPage());
            AddConfigPage(new DepthOfFieldConfigPage());
            AddConfigPage(new DitherConfigPage());
            
            effectList.SelectedIndex = 0;
        }

        public void AddConfigPage(IGameEffectConfigPage page)
        {
            // Create panel to home config page
            Panel panel = new Panel();
            panel.Position = effectPanelRect.position;
            panel.Size = effectPanelRect.size;
            panel.BackgroundColor = effectPanelBackgroundColor;
            panel.Enabled = false;
            panel.Tag = page.Key;
            mainPanel.Components.Add(panel);

            // Setup config page
            page.Setup(panel);

            // Add page to select from list
            effectPagesDict.Add(page.Key, new ConfigPageEntry() { panel = panel, page = page });
            effectList.AddItem(page.Title, -1, page.Key);
        }

        public override void OnPush()
        {
            base.OnPush();

            bool largeHUDEnabled = DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.Enabled;
            mainPanel.VerticalAlignment = (largeHUDEnabled) ? VerticalAlignment.Top : VerticalAlignment.Middle; // Top-align when large HUD enabled to avoid overlap

            RefreshPageSettings();
        }

        public override void OnPop()
        {
            base.OnPop();

            DaggerfallUnity.Settings.SaveSettings();
        }

        /// <summary>
        /// Event raised when when window is ready for custom pages to be added.
        /// </summary>
        public delegate void OnRegisterCustomPagesHandler();
        public event OnRegisterCustomPagesHandler OnRegisterCustomPages;
        protected virtual void RaiseOnRegisterCustomPages()
        {
            OnRegisterCustomPages?.Invoke();
        }

        #region Private Methods

        protected void RefreshPageSettings()
        {
            if (IsSetup)
            {
                foreach (var entry in effectPagesDict.Values)
                {
                    entry.page.ReadSettings();
                }
            }
        }

        private void EffectList_OnSelectItem()
        {
            if (IsSetup)
            {
                string selectedKey = effectList.GetItem(effectList.SelectedIndex).tag as string;
                foreach (var entry in effectPagesDict.Values)
                {
                    entry.panel.Enabled = selectedKey == entry.page.Key;
                }
            }
        }

        private void ResetDefaultsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (IsSetup)
            {
                string selectedKey = effectList.GetItem(effectList.SelectedIndex).tag as string;
                ConfigPageEntry configPageEntry = effectPagesDict[selectedKey];
                configPageEntry.page.SetDefaults();
                configPageEntry.page.ReadSettings();
            }
        }

        private void DoneButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion
    }
}