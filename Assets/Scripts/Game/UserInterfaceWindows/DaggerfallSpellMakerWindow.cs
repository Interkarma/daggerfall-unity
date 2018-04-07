// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
//
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallSpellMakerWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        DFSize baseSize = new DFSize(320, 200);
        Vector2 tipLabelPos = new Vector2(5, 22);
        Rect effect1NameRect = new Rect(3, 30, 314, 9);
        Rect effect2NameRect = new Rect(3, 62, 314, 9);
        Rect effect3NameRect = new Rect(3, 94, 314, 9);
        Rect addEffectButtonRect = new Rect(244, 114, 28, 28);

        #endregion

        #region UI Controls

        TextLabel tipLabel;
        TextLabel effect1NameLabel;
        TextLabel effect2NameLabel;
        TextLabel effect3NameLabel;

        #endregion

        #region UI Textures

        Texture2D baseTexture;
        Texture2D effectSetupOverlayTexture;
        Texture2D goldSelectIconsTexture;
        Texture2D colorSelectIconsTexture;

        #endregion

        #region Fields

        const string textDatabase = "SpellmakerUI";

        const string baseTextureFilename = "INFO01I0.IMG";
        const string effectSetupOverlayFilename = "MASK05I0.IMG";
        const string goldSelectIconsFilename = "MASK01I0.IMG";
        const string colorSelectIconsFilename = "MASK04I0.IMG";

        const int alternateAlphaIndex = 12;

        #endregion

        #region Constructors

        public DaggerfallSpellMakerWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load all the textures used by spell maker window
            LoadTextures();

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundTexture = baseTexture;

            // Setup controls
            SetupLabels();
            SetupButtons();
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            // Load source textures
            baseTexture = ImageReader.GetTexture(baseTextureFilename, 0, 0, true, alternateAlphaIndex);
            effectSetupOverlayTexture = ImageReader.GetTexture(effectSetupOverlayFilename);
            goldSelectIconsTexture = ImageReader.GetTexture(goldSelectIconsFilename);
            colorSelectIconsTexture = ImageReader.GetTexture(colorSelectIconsFilename);
        }

        void SetupLabels()
        {
            // Tip label
            tipLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, tipLabelPos, string.Empty, NativePanel);

            // Effect1
            Panel effect1NamePanel = DaggerfallUI.AddPanel(effect1NameRect, NativePanel);
            effect1NameLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.LargeFont, Vector2.zero, string.Empty, effect1NamePanel);
            effect1NameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            effect1NameLabel.ShadowPosition = Vector2.zero;

            // Effect2
            Panel effect2NamePanel = DaggerfallUI.AddPanel(effect2NameRect, NativePanel);
            effect2NameLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.LargeFont, Vector2.zero, string.Empty, effect2NamePanel);
            effect2NameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            effect2NameLabel.ShadowPosition = Vector2.zero;

            // Effect3
            Panel effect3NamePanel = DaggerfallUI.AddPanel(effect3NameRect, NativePanel);
            effect3NameLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.LargeFont, Vector2.zero, string.Empty, effect3NamePanel);
            effect3NameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            effect3NameLabel.ShadowPosition = Vector2.zero;
        }

        void SetupButtons()
        {
            // Add effect
            Button addEffectButton = DaggerfallUI.AddButton(addEffectButtonRect, NativePanel);
            addEffectButton.OnMouseEnter += AddEffectButton_OnMouseEnter;
            addEffectButton.OnMouseLeave += TipButton_OnMouseLeave;
        }

        private void AddEffectButton_OnMouseEnter(BaseScreenComponent sender)
        {
            tipLabel.Text = TextManager.Instance.GetText(textDatabase, "addEffect");
        }

        private void TipButton_OnMouseLeave(BaseScreenComponent sender)
        {
            tipLabel.Text = string.Empty;
        }

        #endregion

        #region Tip Events
        #endregion
    }
}