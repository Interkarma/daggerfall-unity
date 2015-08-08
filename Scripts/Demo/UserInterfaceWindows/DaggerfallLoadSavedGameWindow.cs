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

        Texture2D nativeTexture;
        int selectedSaveGame = 0;

        Vector4[] saveButtons = new Vector4[]
        {
            new Vector4(40, 4, 80, 50),
            new Vector4(40, 69, 80, 50),
            new Vector4(40, 134, 80, 50),
            new Vector4(200, 4, 80, 50),
            new Vector4(200, 69, 80, 50),
            new Vector4(200, 134, 80, 50),
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

            // Setup game select buttons
            for (int i = 0; i < saveButtons.Length; i++)
            {
                Button button = AddButton(saveButtons[i]);
                button.ClickMessage = string.Format("{0}?i={1}", DaggerfallUIMessages.dfuiSelectSaveGame, i);
                button.DoubleClickMessage = DaggerfallUIMessages.dfuiOpenSelectedSaveGame;
            }

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
            if (message.StartsWith(DaggerfallUIMessages.dfuiSelectSaveGame))
            {
                selectedSaveGame = int.Parse(paramDict["i"]);
            }

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
    }
}