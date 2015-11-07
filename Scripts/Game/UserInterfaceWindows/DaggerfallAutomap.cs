// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Nystul
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
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements character sheet window.
    /// </summary>
    public class DaggerfallAutomapWindow : DaggerfallPopupWindow
    {
        const string nativeImgName = "AMAP00I0.IMG";

        Texture2D nativeTexture;

        public DaggerfallAutomapWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallAutomapWindow: Could not load native texture.");

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Exit button
            Button exitButton = DaggerfallUI.AddButton(new Rect(281, 171, 28, 18), NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
        }

        public override void OnPush()
        {
            if (IsSetup)
            {

            }
        }

        #region Private Methods



        #endregion

        #region Event Handlers

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion
    }
}