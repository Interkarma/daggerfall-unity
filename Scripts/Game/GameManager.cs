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
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// GameManager singleton class.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Fields

        bool isGamePaused = false;
        float savedTimeScale;
        Texture2D pauseScreenshot;

        GameObject playerObject = null;
        Camera mainCamera = null;
        StartGameBehaviour startGameBehaviour = null;
        PlayerEntity playerEntity = null;
        PlayerDeath playerDeath = null;
        WeatherManager weatherManager = null;

        #endregion

        #region Properties

        public static bool IsGamePaused
        {
            get { return Instance.isGamePaused; }
        }

        public Camera MainCamera
        {
            get { return (mainCamera) ? mainCamera : FindMainCamera(); }
        }

        public GameObject PlayerObject
        {
            get { return (playerObject) ? playerObject : FindPlayerObject(); }
        }

        public StartGameBehaviour StartGameBehaviour
        {
            get { return (startGameBehaviour) ? startGameBehaviour : FindStartGameBehaviour(); }
        }

        public PlayerEntity PlayerEntity
        {
            get { return (playerEntity != null) ? playerEntity : FindPlayerEntity(); }
        }

        public PlayerDeath PlayerDeath
        {
            get { return (playerDeath) ? playerDeath : FindPlayerDeath(); }
        }

        public WeatherManager WeatherManager
        {
            get { return (weatherManager) ? weatherManager : FindWeatherManager(); }
        }

        public bool IsPlayerOnHUD
        {
            get { return IsHUDTopWindow(); }
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
            Debug.Log("Welcome to Daggerfall Unity " + VersionInfo.DaggerfallUnityVersion);
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

        bool hudDisabledByPause = false;
        public void PauseGame(bool pause, bool hideHUD = false)
        {
            if (pause && !isGamePaused)
            {
                savedTimeScale = Time.timeScale;
                Time.timeScale = 0;
                InputManager.Instance.IsPaused = true;
                isGamePaused = true;

                if (hideHUD)
                {
                    DaggerfallUI.Instance.DaggerfallHUD.Enabled = false;
                    hudDisabledByPause = true;
                }
            }
            else if (!pause && isGamePaused)
            {
                Time.timeScale = savedTimeScale;
                InputManager.Instance.IsPaused = false;
                isGamePaused = false;

                if (hudDisabledByPause)
                {
                    DaggerfallUI.Instance.DaggerfallHUD.Enabled = true;
                    hudDisabledByPause = false;
                }
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

        bool IsHUDTopWindow()
        {
            IUserInterfaceWindow topWindow = DaggerfallUI.UIManager.TopWindow;
            if (topWindow is DaggerfallHUD)
                return true;

            return false;
        }

        // Returns true when gameplay is active
        bool IsPlayingGame()
        {
            // Game not active when paused
            if (isGamePaused)
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

        GameObject FindPlayerObject()
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
                throw new Exception("GameManager could not find Player object.");

            return playerObject;
        }

        Camera FindMainCamera()
        {
            GameObject go = GameObject.FindGameObjectWithTag("MainCamera");
            if (go == null)
                throw new Exception("GameManager could not find MainCamera object.");

            mainCamera = go.GetComponent<Camera>();
            if (mainCamera == null)
                throw new Exception("GameManager could not find Camera.");

            return mainCamera;
        }

        StartGameBehaviour FindStartGameBehaviour()
        {
            startGameBehaviour = GameObject.FindObjectOfType<StartGameBehaviour>();
            if (startGameBehaviour == null)
                throw new Exception("GameManager could not find StartGameBehaviour.");

            return startGameBehaviour;
        }

        PlayerEntity FindPlayerEntity()
        {
            if (PlayerObject)
            {
                playerEntity = PlayerObject.GetComponent<PlayerEntity>();
                if (playerEntity == null)
                    throw new Exception("GameManager could not find PlayerEntity.");
                return playerEntity;
            }

            return null;
        }

        PlayerDeath FindPlayerDeath()
        {
            if (PlayerObject)
            {
                playerDeath = PlayerObject.GetComponent<PlayerDeath>();
                if (playerDeath == null)
                    throw new Exception("GameManager could not find PlayerDeath.");
                return playerDeath;
            }

            return null;
        }

        WeatherManager FindWeatherManager()
        {
            weatherManager = GameObject.FindObjectOfType<WeatherManager>();
            if (weatherManager == null)
                throw new Exception("GameManager could not find WeatherManager.");

            return weatherManager;
        }

        #endregion
    }
}
