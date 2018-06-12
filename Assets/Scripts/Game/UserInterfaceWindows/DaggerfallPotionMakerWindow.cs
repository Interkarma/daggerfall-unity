// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallPotionMakerWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        Rect recipesButtonRect = new Rect(169, 26, 36, 16);
        Rect mixButtonRect = new Rect(169, 42, 36, 16);
        Rect exitButtonRect = new Rect(290, 178, 24, 16);

        #endregion

        #region UI Controls

        Panel mainPanel = new Panel();
        Button recipesButton;
        Button mixButton;
        Button exitButton;

        #endregion

        #region UI Textures

        Texture2D baseTexture;
        const string baseTextureName = "MASK00I0.IMG";
        const int alternateAlphaIndex = 12;

        #endregion

        #region Constructors

        public DaggerfallPotionMakerWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        #endregion

        #region Setup Methods

        // guildrank DarkBrotherHood 3
        protected override void Setup()
        {
            // Load textures
            LoadTextures();

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundColor = new Color(0, 0, 0, 0.75f);
            NativePanel.BackgroundTexture = baseTexture;

            // Setup buttons
            SetupButtons();
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName, 0, 0, true, alternateAlphaIndex);
        }

        void SetupButtons()
        {
            // Exit button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
        }

        #endregion

        #region Event Handlers

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void RecipesButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void MixButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion
    }
}