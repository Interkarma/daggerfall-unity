// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
    /// Implements joystick controls window.
    /// </summary>
    public class DaggerfallJoystickControlsWindow : DaggerfallPopupWindow
    {
        Texture2D nativeTexture;
        Panel joystickPanel = new Panel();

        const string nativeImgName = "CNFG04I0.IMG";

        public DaggerfallJoystickControlsWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            : base(uiManager, previousWindow)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallJoystickControlsWindow: Could not load native texture.");

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            //Joystick Controls panel
            joystickPanel.HorizontalAlignment = HorizontalAlignment.Center;
            joystickPanel.Position = new Vector2(0, 30);
            joystickPanel.Size = new Vector2(nativeTexture.width, nativeTexture.height);
            joystickPanel.BackgroundTexture = nativeTexture;
            NativePanel.Components.Add(joystickPanel);

            // Continue
            Button continueButton = DaggerfallUI.AddButton(new Rect(20, 109, 68, 18), joystickPanel);
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