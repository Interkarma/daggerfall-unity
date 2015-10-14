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
using System.Text;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Serialization;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// GameManager singleton class.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Fields

        bool gamePaused = false;
        float savedTimeScale;
        Texture2D pauseScreenshot;

        #endregion

        #region Properties

        public static bool GamePaused
        {
            get { return Instance.gamePaused; }
        }

        #endregion

        #region Singleton

        static GameManager instance = null;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindSingleton(out instance))
                    {
                        GameObject go = new GameObject();
                        go.name = "GameManager";
                        instance = go.AddComponent<GameManager>();
                    }
                }
                return instance;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return (instance != null);
            }
        }

        #endregion

        #region Unity

        void Start()
        {
            SetupSingleton();
        }

        void Update()
        {
            if (!IsPlayingGame())
                return;

            // Post message to open options dialog on escape during gameplay
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenPauseOptionsDialog);
                StartCoroutine(TakeScreenshot());
            }

            // Handle in-game windows
            if (InputManager.Instance.ActionStarted(InputManager.Actions.CharacterSheet))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenCharacterSheetDialog);
            }

            // Handle quick save and load
            if (InputManager.Instance.ActionStarted(InputManager.Actions.QuickSave))
            {
                SaveLoadManager.Instance.QuickSave();
            }
            else if (InputManager.Instance.ActionStarted(InputManager.Actions.QuickLoad))
            {
                SaveLoadManager.Instance.QuickLoad();
            }
        }

        #endregion

        #region Public Methods

        public void PauseGame(bool pause)
        {
            if (pause && !gamePaused)
            {
                savedTimeScale = Time.timeScale;
                Time.timeScale = 0;
                InputManager.Instance.IsPaused = true;
                gamePaused = true;
            }
            else if (!pause && gamePaused)
            {
                Time.timeScale = savedTimeScale;
                InputManager.Instance.IsPaused = false;
                gamePaused = false;
            }
        }

        #endregion

        #region Public Static Methods

        public static bool FindSingleton(out GameManager singletonOut)
        {
            singletonOut = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
            if (singletonOut == null)
            {
                DaggerfallUnity.LogMessage("Could not locate GameManager GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        private void SetupSingleton()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
            {
                if (Application.isPlaying)
                {
                    DaggerfallUnity.LogMessage("Multiple GameManager instances detected in scene!", true);
                    Destroy(gameObject);
                }
            }
        }

        // Returns true when gameplay is active
        bool IsPlayingGame()
        {
            // Game not active when paused
            if (gamePaused)
                return false;

            // Game not active when SaveLoadManager not present
            if (SaveLoadManager.Instance == null)
                return false;

            // Game not active when top window is neither null or HUD
            IUserInterfaceWindow topWindow = DaggerfallUI.UIManager.TopWindow;
            if (topWindow != null && !(topWindow is DaggerfallHUD))
                return false;

            return true;
        }

        // Takes a screenshot at end of current frame
        IEnumerator TakeScreenshot()
        {
            yield return new WaitForEndOfFrame();

            pauseScreenshot = new Texture2D(Screen.width, Screen.height);
            pauseScreenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            pauseScreenshot.Apply();
        }

        #endregion
    }
}
