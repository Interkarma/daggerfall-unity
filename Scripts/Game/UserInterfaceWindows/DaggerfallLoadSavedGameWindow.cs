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
using DaggerfallConnect.Save;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements the Daggerfall Load Saved Game interface.
    /// </summary>
    public class DaggerfallLoadSavedGameWindow : DaggerfallBaseWindow
    {
        const string nativeImgName = "LOAD00I0.IMG";
        const string gameid = "gameid";

        Texture2D nativeTexture;
        int selectedSaveGame = -1;
        Button[] saveImageButtons;
        Button[] saveTextButtons;
        Outline outline;

        SaveGames saveGames = new SaveGames();

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

            OpenSaveGames();
            AddControls();
        }

        public override void ProcessMessages()
        {
            base.ProcessMessages();

            string message = uiManager.GetMessage();
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
        }

        #region Private Methods

        void OpenSaveGames()
        {
            if (DaggerfallUnity.Instance.IsPathValidated)
            {
                string savePath = Path.GetDirectoryName(DaggerfallUnity.Instance.Arena2Path);
                saveGames.OpenSavesPath(savePath);
            }
        }

        void AddControls()
        {
            if (!saveGames.IsPathOpen)
                return;

            saveImageButtons = new Button[saveImageButtonDims.Length];
            saveTextButtons = new Button[saveTextButtonDims.Length];
            for (int i = 0; i < saveImageButtonDims.Length; i++)
            {
                // Open save
                if (!saveGames.OpenSave(i))
                    continue;

                // Get save texture
                Texture2D saveTexture = TextureReader.CreateFromAPIImage(saveGames.SaveImage);
                saveTexture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

                // Setup image button
                saveImageButtons[i] = AddButton(saveImageButtonDims[i]);
                saveImageButtons[i].BackgroundTexture = saveTexture;
                saveImageButtons[i].BackgroundTextureLayout = TextureLayout.ScaleToFit;
                saveImageButtons[i].ClickMessage = string.Format("{0}?{1}={2}", DaggerfallUIMessages.dfuiSelectSaveGame, gameid, i);
                saveImageButtons[i].DoubleClickMessage = DaggerfallUIMessages.dfuiOpenSelectedSaveGame;

                // Setup text button
                saveTextButtons[i] = AddButton(saveTextButtonDims[i]);
                saveTextButtons[i].Label.Text = saveGames.SaveName;
                saveTextButtons[i].ClickMessage = string.Format("{0}?{1}={2}", DaggerfallUIMessages.dfuiSelectSaveGame, gameid, i);
                saveTextButtons[i].DoubleClickMessage = DaggerfallUIMessages.dfuiOpenSelectedSaveGame;

                // Select first valid save game
                if (selectedSaveGame == -1)
                    selectedSaveGame = i;
            }

            // Setup outline
            outline = AddOutline(outlineRects[0], DaggerfallUI.DaggerfallDefaultTextColor);
            if (selectedSaveGame == -1)
                outline.Enabled = false;
            else
                SelectSaveGame(selectedSaveGame);

            // Setup load game and exit buttons
            AddButton(new Vector2(126, 5), new Vector2(68, 11), DaggerfallUIMessages.dfuiOpenSelectedSaveGame);
            AddButton(new Vector2(133, 150), new Vector2(56, 19), WindowMessages.wmCloseWindow);
        }

        void SelectSaveGame(int index)
        {
            selectedSaveGame = index;

            Rect rect = outlineRects[index];
            outline.Position = new Vector2(rect.x, rect.y);
            outline.Size = new Vector2(rect.width, rect.height);
        }

        #endregion
    }
}