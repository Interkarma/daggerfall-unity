// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallMerchantRepairPopupWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        Rect repairButtonRect = new Rect(5, 5, 120, 7);
        Rect talkButtonRect = new Rect(5, 14, 120, 7);
        Rect sellButtonRect = new Rect(5, 23, 120, 7);
        Rect exitButtonRect = new Rect(44, 33, 43, 15);

        #endregion

        #region UI Controls

        Panel mainPanel = new Panel();
        Button repairButton = new Button();
        Button talkButton = new Button();
        Button sellButton = new Button();
        Button exitButton = new Button();

        #endregion

        #region UI Textures

        Texture2D baseTexture;

        #endregion

        #region Fields

        const string baseTextureName = "REPR01I0.IMG";      // Repair / Talk / Sell

        DFLocation.BuildingTypes buildingType = DFLocation.BuildingTypes.AllValid;

        #endregion

        #region Properties

        public DFLocation.BuildingTypes BuildingType
        {
            get { return buildingType; }
            set { buildingType = value; }
        }

        #endregion

        #region Constructors

        public DaggerfallMerchantRepairPopupWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
            // Clear background
            ParentPanel.BackgroundColor = Color.clear;
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load all textures
            LoadTextures();

            // Create interface panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.BackgroundTexture = baseTexture;
            mainPanel.Position = new Vector2(0, 50);
            mainPanel.Size = new Vector2(baseTexture.width, baseTexture.height);

            // Repair button
            repairButton = DaggerfallUI.AddButton(repairButtonRect, mainPanel);
            repairButton.BackgroundColor = DaggerfallUI.DaggerfallUnityNotImplementedColor;

            // Talk button
            talkButton = DaggerfallUI.AddButton(talkButtonRect, mainPanel);
            talkButton.BackgroundColor = DaggerfallUI.DaggerfallUnityNotImplementedColor;

            // Sell button
            sellButton = DaggerfallUI.AddButton(sellButtonRect, mainPanel);
            sellButton.OnMouseClick += SellButton_OnMouseClick;

            // Exit button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, mainPanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

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

        private void TalkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void SellButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
            DaggerfallItemActionWindow sellWindow = new DaggerfallItemActionWindow(uiManager, DaggerfallItemActionWindow.WindowModes.Sell, this);
            uiManager.PushWindow(sellWindow);
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion
    }
}