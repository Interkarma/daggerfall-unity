// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements popup window on escape during gameplay.
    /// </summary>
    public class DaggerfallPauseOptionsWindow : DaggerfallPopupWindow
    {
        const string nativeImgName = "OPTN00I0.IMG";

        Texture2D nativeTexture;
        Panel optionsPanel = new Panel();

        public DaggerfallPauseOptionsWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            :base(uiManager, previousWindow)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallOptionsWindow: Could not load native texture.");

            ParentPanel.BackgroundColor = ScreenDimColor;

            optionsPanel.HorizontalAlignment = HorizontalAlignment.Center;
            optionsPanel.Position = new Vector2(0, 40);
            optionsPanel.Size = new Vector2(nativeTexture.width, nativeTexture.height);
            optionsPanel.BackgroundTexture = nativeTexture;
            NativePanel.Components.Add(optionsPanel);
        }
    }
}