// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Allofich
// Contributors:
//
// Notes:
//

using UnityEngine;
using System;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements popup window for when player is in court for a crime.
    /// </summary>
    public class DaggerfallCourtWindow : DaggerfallPopupWindow
    {
        const string nativeImgName = "CORT01I0.img";

        Texture2D nativeTexture;
        Panel courtPanel = new Panel();
        //private float timer = 0f;

        public DaggerfallCourtWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            : base(uiManager, previousWindow)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallCourtWindow: Could not load native texture.");

            // Native court panel
            courtPanel.HorizontalAlignment = HorizontalAlignment.Center;
            courtPanel.Size = TextureReplacement.GetSize(nativeTexture, nativeImgName);
            courtPanel.BackgroundTexture = nativeTexture;
            NativePanel.Components.Add(courtPanel);

            DaggerfallUI.MessageBox("Not implemented yet. Press ESC to exit.");
        }
    }
}
