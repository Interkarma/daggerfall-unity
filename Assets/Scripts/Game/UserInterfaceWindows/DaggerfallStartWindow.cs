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

        private Button loadGameButton;
        private Button newGameButton;
        private Button exitButton;

        bool isLoadGameDeferred = false;
        bool isNewGameDeferred = false;
        bool isExitGameDeferred = false;

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
            loadGameButton = DaggerfallUI.AddButton(new Vector2(72, 45), new Vector2(147, 15), DaggerfallUIMessages.dfuiOpenLoadSavedGameWindow, NativePanel);
            loadGameButton.OnMouseClick += LoadGameButton_OnMouseClick;
            loadGameButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.MainMenuLoad);
            loadGameButton.OnKeyboardEvent += LoadGameButton_OnKeyboardEvent;

            newGameButton = DaggerfallUI.AddButton(new Vector2(72, 99), new Vector2(147, 15), DaggerfallUIMessages.dfuiStartNewGame, NativePanel);
            newGameButton.OnMouseClick += NewGameButton_OnMouseClick;
            newGameButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.MainMenuStart);
            newGameButton.OnKeyboardEvent += NewGameButton_OnKeyboardEvent;

            exitButton = DaggerfallUI.AddButton(new Vector2(125, 145), new Vector2(41, 15), DaggerfallUIMessages.dfuiExitGame, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.MainMenuExit);
            exitButton.OnKeyboardEvent += ExitButton_OnKeyboardEvent;
        }

        public override void Update()
        {
            base.Update();
            InputManager.Instance.CursorVisible = true;
        }

        float timer = 0;
        bool oneTime = false;
        public override void Draw()
        {
            base.Draw();

            // 100ms is enough time to ensure window drawn and event raised before user can click
            if (!oneTime && (timer += Time.realtimeSinceStartup) > 100)
            {
                RaiseOnStartFirstVisibleEvent();
                oneTime = true;
            }
        }

        void LoadGame()
        {
            uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.UnitySaveGame, new object[] { uiManager, DaggerfallUnitySaveGameWindow.Modes.LoadGame, null, true }));
        }

        void StartNewGame()
        {
            uiManager.PushWindow(UIWindowFactory.GetInstance(UIWindowType.StartNewGameWizard, uiManager));
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

        void LoadGameButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            LoadGame();
        }

        void LoadGameButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
                isLoadGameDeferred = true;
            else if (keyboardEvent.type == EventType.KeyUp && isLoadGameDeferred)
            {
                isLoadGameDeferred = false;
                LoadGame();
            }
        }

        void NewGameButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            StartNewGame();
        }

        void NewGameButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
                isNewGameDeferred = true;
            else if (keyboardEvent.type == EventType.KeyUp && isNewGameDeferred)
            {
                isNewGameDeferred = false;
                StartNewGame();
            }
        }

        void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ExitGame();
        }

        void ExitButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
                isExitGameDeferred = true;
            else if (keyboardEvent.type == EventType.KeyUp && isExitGameDeferred)
            {
                isExitGameDeferred = false;
                ExitGame();
            }
        }

        public delegate void OnStartFirstVisibleEventHandler();
        public static event OnStartFirstVisibleEventHandler OnStartFirstVisible;
        void RaiseOnStartFirstVisibleEvent()
        {
            if (OnStartFirstVisible != null)
                OnStartFirstVisible();
        }
    }
}