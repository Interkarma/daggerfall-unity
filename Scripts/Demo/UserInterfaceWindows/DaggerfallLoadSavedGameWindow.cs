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
using System.Collections.Generic;
using DaggerfallWorkshop.Demo.UserInterface;

namespace DaggerfallWorkshop.Demo.UserInterfaceWindows
{
    /// <summary>
    /// Implements the Daggerfall Load Saved Game interface.
    /// </summary>
    public class DaggerfallLoadSavedGameWindow : DaggerfallBaseWindow
    {
        const string nativeImgName = "LOAD00I0.IMG";
        const string gameid = "gameid";

        Texture2D nativeTexture;
        int selectedSaveGame = 0;
        Button[] saveImageButtons;
        Button[] saveTextButtons;
        Outline outline;

        Rect[] saveImageButtonDims = new Rect[]
        {
            new Rect(40, 4, 80, 50),
            new Rect(40, 69, 80, 50),
            new Rect(40, 134, 80, 50),
            new Rect(200, 4, 80, 50),
            new Rect(200, 69, 80, 50),
            new Rect(200, 134, 80, 50),
        };

        Rect[] saveTextButtonDims = new Rect[]
        {
            new Rect(1, 56, 158, 9),
            new Rect(1, 121, 158, 9),
            new Rect(1, 186, 158, 9),
            new Rect(162, 56, 158, 9),
            new Rect(162, 121, 158, 9),
            new Rect(162, 186, 158, 9),
        };

        Rect[] outlineRects = new Rect[]
        {
            new Rect(39, 3, 81, 51),
            new Rect(39, 68, 81, 51),
            new Rect(39, 133, 81, 51),
            new Rect(199, 3, 81, 51),
            new Rect(199, 68, 81, 51),
            new Rect(199, 133, 81, 51),
        };

        public int SelectedSaveGame
        {
            get { return selectedSaveGame; }
        }

        public DaggerfallLoadSavedGameWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallLoadSavedGameWindow: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Setup game image buttons
            saveImageButtons = new Button[saveImageButtonDims.Length];
            for (int i = 0; i < saveImageButtonDims.Length; i++)
            {
                saveImageButtons[i] = AddButton(saveImageButtonDims[i]);
                saveImageButtons[i].ClickMessage = string.Format("{0}?{1}={2}", DaggerfallUIMessages.dfuiSelectSaveGame, gameid, i);
                saveImageButtons[i].DoubleClickMessage = DaggerfallUIMessages.dfuiOpenSelectedSaveGame;
            }

            // Setup game text buttons
            saveTextButtons = new Button[saveTextButtonDims.Length];
            for (int i = 0; i < saveTextButtonDims.Length; i++)
            {
                saveTextButtons[i] = AddButton(saveTextButtonDims[i]);
                saveTextButtons[i].ClickMessage = string.Format("{0}?{1}={2}", DaggerfallUIMessages.dfuiSelectSaveGame, gameid, i);
                saveTextButtons[i].DoubleClickMessage = DaggerfallUIMessages.dfuiOpenSelectedSaveGame;
                saveTextButtons[i].Label.Text = string.Format("Save {0}", i);
            }

            // Setup outline
            outline = AddOutline(outlineRects[selectedSaveGame], DaggerfallUI.DaggerfallDefaultTextColor);

            // Setup load game and exit buttons
            AddButton(new Vector2(126, 5), new Vector2(68, 11), DaggerfallUIMessages.dfuiOpenSelectedSaveGame);
            AddButton(new Vector2(133, 150), new Vector2(56, 19), WindowMessages.wmCloseWindow);

            IsSetup = true;
        }

        protected override void ProcessMessageQueue()
        {
            string message = uiManager.PeekMessage();
            Dictionary<string, string> paramDict = UserInterfaceManager.BuildParamDict(message);
            
            // Select save game
            if (message.Contains(DaggerfallUIMessages.dfuiSelectSaveGame))
                SelectSaveGame(int.Parse(paramDict[gameid]));

            // Other messages
            switch (message)
            {
                case DaggerfallUIMessages.dfuiOpenSelectedSaveGame:
                    // TODO: Open selected save game
                    break;
                default:
                    return;
            }

            // Message was handled, pop from stack
            uiManager.PopMessage();
        }

        void SelectSaveGame(int index)
        {
            selectedSaveGame = index;

            Rect rect = outlineRects[index];
            outline.Position = new Vector2(rect.x, rect.y);
            outline.Size = new Vector2(rect.width, rect.height);
        }
    }
}