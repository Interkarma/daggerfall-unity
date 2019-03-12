// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallStartWindow: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Setup buttons
            DaggerfallUI.AddButton(new Vector2(72, 45), new Vector2(147, 15), DaggerfallUIMessages.dfuiOpenLoadSavedGameWindow, NativePanel);
            DaggerfallUI.AddButton(new Vector2(72, 99), new Vector2(147, 15), DaggerfallUIMessages.dfuiStartNewGame, NativePanel);
            DaggerfallUI.AddButton(new Vector2(125, 145), new Vector2(41, 15), DaggerfallUIMessages.dfuiExitGame, NativePanel);
        }

        public override void Update()
        {
            base.Update();
            Cursor.visible = true;

            // Shortcuts for options
            if (Input.GetKeyDown(KeyCode.L))
                LoadGame();
            else if (Input.GetKeyDown(KeyCode.S))
                StartNewGame();
            else if (Input.GetKeyDown(KeyCode.E))
                ExitGame();
        }

        void LoadGame()
        {
            uiManager.PushWindow(new DaggerfallUnitySaveGameWindow(uiManager, DaggerfallUnitySaveGameWindow.Modes.LoadGame, null, true));
        }

        void StartNewGame()
        {
            uiManager.PushWindow(new StartNewGameWizard(uiManager));
        }

        void ExitGame()
        {
            Application.Quit();
        }

        public override void ProcessMessages()
        {
            base.ProcessMessages();

            switch (uiManager.GetMessage())
            {
                case DaggerfallUIMessages.dfuiOpenLoadSavedGameWindow:
                    LoadGame();
                    break;
                case DaggerfallUIMessages.dfuiStartNewGame:
                    StartNewGame();
                    break;
                case DaggerfallUIMessages.dfuiExitGame:
                    ExitGame();
                    break;
            }
        }
    }
}