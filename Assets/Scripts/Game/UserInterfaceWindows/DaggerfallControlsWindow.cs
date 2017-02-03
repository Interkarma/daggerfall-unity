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
    /// Implements controls window.
    /// </summary>
    public class DaggerfallControlsWindow : DaggerfallPopupWindow
    {
        Texture2D nativeTexture;
        Panel controlsPanel = new Panel();

        const string nativeImgName = "CNFG00I0.IMG";

        public DaggerfallControlsWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            :base(uiManager, previousWindow)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallControlsWindow: Could not load native texture.");

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            //Controls panel
            controlsPanel.HorizontalAlignment = HorizontalAlignment.Center;
            controlsPanel.Size = new Vector2(nativeTexture.width, nativeTexture.height);
            controlsPanel.BackgroundTexture = nativeTexture;
            NativePanel.Components.Add(controlsPanel);

            //Joystick
            Button joystickButton = DaggerfallUI.AddButton(new Rect(0, 190, 80, 10), controlsPanel);
            joystickButton.OnMouseClick += JoystickButton_OnMouseClick;

            //Mouse
            Button mouseButton = DaggerfallUI.AddButton(new Rect(80, 190, 80, 10), controlsPanel);
            mouseButton.OnMouseClick += MouseButton_OnMouseClick;

            //Default
            Button defaultButton = DaggerfallUI.AddButton(new Rect(160, 190, 80, 10), controlsPanel);
            defaultButton.BackgroundColor = new Color(1, 0, 0, 0.5f);
            defaultButton.OnMouseClick += DefaultButton_OnMouseClick;

            // Continue
            Button continueButton = DaggerfallUI.AddButton(new Rect(240, 190, 80, 10), controlsPanel);
            continueButton.OnMouseClick += ContinueButton_OnMouseClick;
        }

        #region Tab Event Handlers

        private void JoystickButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenJoystickControlsWindow);
        }

        private void MouseButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenMouseControlsWindow);
        }

        private void DefaultButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            //Set keybinds back to default
        }

        private void ContinueButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CancelWindow();
        }

        #endregion
    }
}