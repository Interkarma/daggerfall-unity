// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: Justin Steele
//
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements mouse controls window.
    /// </summary>
    public class DaggerfallMouseControlsWindow : DaggerfallPopupWindow
    {
        Texture2D nativeTexture;
        Panel mousePanel = new Panel();

        const string nativeImgName = "CNFG01I0.IMG";

        public DaggerfallMouseControlsWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            : base(uiManager, previousWindow)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallMouseControlsWindow: Could not load native texture.");

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            //Joystick Controls panel
            mousePanel.HorizontalAlignment = HorizontalAlignment.Center;
            mousePanel.VerticalAlignment = VerticalAlignment.Middle;
            mousePanel.Size = new Vector2(nativeTexture.width, nativeTexture.height);
            mousePanel.BackgroundTexture = nativeTexture;
            NativePanel.Components.Add(mousePanel);

            // Continue
            Button continueButton = DaggerfallUI.AddButton(new Rect(93, 123, 90, 20), mousePanel);
            continueButton.OnMouseClick += ContinueButton_OnMouseClick;
        }

        #region Event Handlers

        private void ContinueButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CancelWindow();
        }

        #endregion
    }
}