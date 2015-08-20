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
using System.Collections;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements the Daggerfall Start user interface to load, save, exit.
    /// </summary>
    public class DaggerfallStartWindow : DaggerfallBaseWindow
    {
        const string nativeImgName = "PICK03I0.IMG";

        Texture2D nativeTexture;

        public DaggerfallStartWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallStartWindow: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Setup buttons
            AddButton(new Vector2(72, 45), new Vector2(147, 15), DaggerfallUIMessages.dfuiOpenLoadSavedGameWindow);
            AddButton(new Vector2(72, 99), new Vector2(147, 15), DaggerfallUIMessages.dfuiStartNewGame);
            AddButton(new Vector2(125, 145), new Vector2(41, 15), DaggerfallUIMessages.dfuiExitGame);
        }

        public override void ProcessMessages()
        {
            base.ProcessMessages();

            switch (uiManager.GetMessage())
            {
                case DaggerfallUIMessages.dfuiOpenLoadSavedGameWindow:
                    uiManager.PushWindow(new DaggerfallLoadSavedGameWindow(uiManager));
                    break;
                case DaggerfallUIMessages.dfuiStartNewGame:
                    uiManager.PushWindow(new StartNewGameWizard(uiManager));
                    break;
                case DaggerfallUIMessages.dfuiExitGame:
                    Application.Quit();
                    break;
            }
        }
    }
}