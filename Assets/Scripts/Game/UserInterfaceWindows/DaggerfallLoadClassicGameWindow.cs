// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements the classic Daggerfall Load Saved Game interface.
    /// </summary>
    public class DaggerfallLoadClassicGameWindow : DaggerfallPopupWindow
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

        public DaggerfallLoadClassicGameWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            : base(uiManager, previousWindow)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallLoadSavedGameWindow: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Always dim background
            //ParentPanel.BackgroundColor = ScreenDimColor;

            OpenSaveGames();
            AddControls();
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
            saveImageButtons = new Button[saveImageButtonDims.Length];
            saveTextButtons = new Button[saveTextButtonDims.Length];
            for (int i = 0; i < saveImageButtonDims.Length; i++)
            {
                // Open save
                if (!saveGames.LazyOpenSave(i))
                {
                    DaggerfallUnity.LogMessage(string.Format("Could not lazy open save index {0}.", i), true);
                    continue;
                }

                // Get save texture
                Texture2D saveTexture = TextureReader.CreateFromAPIImage(saveGames.SaveImage);
                saveTexture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

                // Setup image button
                saveImageButtons[i] = DaggerfallUI.AddButton(saveImageButtonDims[i], NativePanel);
                saveImageButtons[i].BackgroundTexture = saveTexture;
                saveImageButtons[i].BackgroundTextureLayout = BackgroundLayout.ScaleToFit;
                saveImageButtons[i].Tag = i;
                saveImageButtons[i].OnMouseClick += SaveGame_OnMouseClick;
                saveImageButtons[i].OnMouseDoubleClick += SaveGame_OnMouseDoubleClick;

                // Setup text button
                saveTextButtons[i] = DaggerfallUI.AddButton(saveTextButtonDims[i], NativePanel);
                saveTextButtons[i].Label.Text = saveGames.SaveName;
                saveTextButtons[i].Tag = i;
                saveTextButtons[i].OnMouseClick += SaveGame_OnMouseClick;
                saveTextButtons[i].OnMouseDoubleClick += SaveGame_OnMouseDoubleClick;

                // Select first valid save game
                if (selectedSaveGame == -1)
                    selectedSaveGame = i;
            }

            // Setup outline
            outline = DaggerfallUI.AddOutline(outlineRects[0], DaggerfallUI.DaggerfallDefaultTextColor, NativePanel);
            if (selectedSaveGame == -1)
                outline.Enabled = false;
            else
                SelectSaveGame(selectedSaveGame);

            // Setup load game button
            if (selectedSaveGame >= 0)
            {
                Button loadGameButton = DaggerfallUI.AddButton(new Vector2(126, 5), new Vector2(68, 11), NativePanel);
                loadGameButton.OnMouseClick += LoadGameButton_OnMouseClick;
            }

            // Setup exit button
            DaggerfallUI.AddButton(new Vector2(133, 150), new Vector2(56, 19), WindowMessages.wmCloseWindow, NativePanel);

            //// TEMP: Look for quick save and add temp button
            //if (SaveLoadManager.Instance.HasQuickSave())
            //{
            //    Button quickLoadButton = new Button();
            //    quickLoadButton.HorizontalAlignment = HorizontalAlignment.Center;
            //    quickLoadButton.VerticalAlignment = VerticalAlignment.Middle;
            //    quickLoadButton.BackgroundColor = Color.gray;
            //    quickLoadButton.Label.Text = "Quick Load";
            //    quickLoadButton.Label.BackgroundColor = Color.gray;
            //    quickLoadButton.OnMouseClick += QuickLoadButton_OnMouseClick;
            //    quickLoadButton.Size = new Vector2(52, 10);
            //    NativePanel.Components.Add(quickLoadButton);
            //}
        }

        void SelectSaveGame(int index)
        {
            selectedSaveGame = index;

            Rect rect = outlineRects[index];
            outline.Position = new Vector2(rect.x, rect.y);
            outline.Size = new Vector2(rect.width, rect.height);
        }

        void OpenSelectedSaveGame()
        {
            Cursor.lockState = CursorLockMode.Locked;
            InputManager.Instance.CursorVisible = false;

            // Setup start behaviour
            StartGameBehaviour startGameBehaviour = FindStartGameBehaviour();
            startGameBehaviour.ClassicSaveIndex = selectedSaveGame;
            startGameBehaviour.StartMethod = StartGameBehaviour.StartMethods.LoadClassicSave;
        }

        StartGameBehaviour FindStartGameBehaviour()
        {
            // Get StartGameBehaviour
            StartGameBehaviour startGameBehaviour = GameObject.FindObjectOfType<StartGameBehaviour>();
            if (!startGameBehaviour)
                throw new Exception("Could not find StartGameBehaviour in scene.");

            return startGameBehaviour;
        }

        #endregion

        #region Event Handlers

        private void LoadGameButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            OpenSelectedSaveGame();
        }

        private void SaveGame_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            SelectSaveGame((int)sender.Tag);
        }

        private void SaveGame_OnMouseDoubleClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            SelectSaveGame((int)sender.Tag);
            OpenSelectedSaveGame();
        }

        //private void QuickLoadButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;

        //    QuickLoad();
        //}

        #endregion
    }
}
