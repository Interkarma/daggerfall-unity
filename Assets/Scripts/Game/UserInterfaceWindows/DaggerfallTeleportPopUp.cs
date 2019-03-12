// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut

using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallConnect.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallTeleportPopUp : DaggerfallPopupWindow
    {
        #region UI Rects

        Rect destinationPanelRect = new Rect(5, 15, 161, 8);
        Rect yesButtonRect = new Rect(4, 38, 52, 15);
        Rect noButtonRect = new Rect(115, 38, 52, 15);

        #endregion

        #region UI Controls

        Panel mainPanel = new Panel();
        Panel destinationPanel = new Panel();
        TextLabel destinationLabel;
        Button yesButton;
        Button noButton;

        #endregion

        #region UI Textures

        Texture2D baseTexture;
        const string baseTextureName = "TELE00I0.IMG";

        #endregion

        #region Properties

        DaggerfallTravelMapWindow travelWindow = null;
        DFPosition destinationPos;
        string destinationName;

        public DFPosition DestinationPos { get { return destinationPos; } set { destinationPos = value; } }
        public string DestinationName { get { return destinationName; } set { destinationName = value; } }

        #endregion

        #region Constructors

        public DaggerfallTeleportPopUp(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null, DaggerfallTravelMapWindow travelWindow = null)
            : base(uiManager, previousWindow)
        {
            this.travelWindow = travelWindow;
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load textures
            LoadTextures();

            // Create interface panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.BackgroundTexture = baseTexture;
            mainPanel.Position = new Vector2(0, 50);
            mainPanel.Size = new Vector2(baseTexture.width, baseTexture.height);

            destinationPanel = DaggerfallUI.AddPanel(destinationPanelRect, mainPanel);
            destinationLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(1, 1), destinationPanel);
            destinationLabel.ShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
            destinationLabel.HorizontalAlignment = HorizontalAlignment.Center;
            destinationLabel.Text = destinationName;

            // Yes button
            yesButton = DaggerfallUI.AddButton(yesButtonRect, mainPanel);
            yesButton.OnMouseClick += YesButton_OnMouseClick;

            // No button
            noButton = DaggerfallUI.AddButton(noButtonRect, mainPanel);
            noButton.OnMouseClick += NoButton_OnMouseClick;

            NativePanel.Components.Add(mainPanel);
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName);
        }

        #endregion

        #region Event Handlers

        private void NoButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void YesButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.FadeBehaviour.SmashHUDToBlack();

            // Teleport to destination.
            PlayerEnterExit playerEnterExit = GameManager.Instance.PlayerEnterExit;
            if (playerEnterExit.IsPlayerInside)
                playerEnterExit.TransitionExterior();
            GameManager.Instance.StreamingWorld.TeleportToCoordinates((int)destinationPos.X, (int)destinationPos.Y, StreamingWorld.RepositionMethods.RandomStartMarker);

            // Close windows.
            DaggerfallUI.Instance.UserInterfaceManager.PopWindow();
            travelWindow.CloseTravelWindows();
            CloseWindow();

            DaggerfallUI.Instance.FadeBehaviour.FadeHUDFromBlack();
        }

        #endregion
    }
}