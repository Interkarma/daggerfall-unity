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
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements inventory window.
    /// </summary>
    public class DaggerfallInventoryWindow : DaggerfallPopupWindow
    {
        const string baseTextureName = "INVE00I0.IMG";

        Texture2D baseTexture;
        CharacterPortrait characterPortrait = new CharacterPortrait();

        public DaggerfallInventoryWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Load all the textures used by inventory system
            LoadTextures();

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundTexture = baseTexture;

            // Character portrait
            NativePanel.Components.Add(characterPortrait);
            characterPortrait.Position = new Vector2(49, 13);
            characterPortrait.Refresh();
        }

        #region Private Methods

        void LoadTextures()
        {
            // Load base texture
            baseTexture = DaggerfallUI.GetTextureFromImg(baseTextureName);
            if (!baseTexture)
                throw new Exception(string.Format("DaggerfallInventoryWindow: Could not load {0}.", baseTextureName));
        }

        #endregion

    }
}