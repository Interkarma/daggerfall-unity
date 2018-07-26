// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements special advantage/disadvantage window.
    /// </summary>
    public class CreateCharSpecialAdvantageWindow : DaggerfallPopupWindow
    {
        const string nativeImgName = "CUST01I0.IMG";
        const string nativeImgOverlayName = "CUST02I0.IMG"; // Overlaying this image makes it Special Advantages instead of Disadvantages

        Texture2D nativeTexture;
        Texture2D nativeOverlayTexture;
        DaggerfallFont font;
        Panel popupPanel = new Panel();
        Panel overlayPanel = new Panel();
        bool isDisadvantages;

        #region UI Rects

        Rect exitButtonRect = new Rect(6, 179, 155, 13);

        #endregion

        #region Buttons

        Button exitButton;

        #endregion

        #region Text Labels

        #endregion

        public CreateCharSpecialAdvantageWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previous = null, bool isDisadvantages = false)
            : base(uiManager, previous)
        {
            this.isDisadvantages = isDisadvantages;
        }

        #region Setup Methods

        protected override void Setup()
        {
            if (IsSetup)
                return;

            base.Setup();

            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            nativeOverlayTexture = DaggerfallUI.GetTextureFromImg(nativeImgOverlayName);
            if (!nativeTexture || !nativeOverlayTexture)
                throw new Exception("CreateCharSpecialAdvantage: Could not load native texture.");

            // Create panel for window
            popupPanel.Size = TextureReplacement.GetSize(nativeTexture, nativeImgName);
            popupPanel.HorizontalAlignment = HorizontalAlignment.Left;
            popupPanel.VerticalAlignment = VerticalAlignment.Top;
            popupPanel.BackgroundTexture = nativeTexture;
            popupPanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
            NativePanel.Components.Add(popupPanel);

            // Add components for adding advantages
            if (!isDisadvantages)
            {
                overlayPanel.Size = TextureReplacement.GetSize(nativeOverlayTexture, nativeImgOverlayName);
                overlayPanel.HorizontalAlignment = HorizontalAlignment.Left;
                overlayPanel.VerticalAlignment = VerticalAlignment.Top;
                overlayPanel.BackgroundTexture = nativeOverlayTexture;
                overlayPanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
                popupPanel.Components.Add(overlayPanel);
            }

            // Setup buttons
            exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            IsSetup = true;
        }

        #endregion

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }

        #region Event Handlers

        void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            CloseWindow();
        }

        #endregion
    }    
}