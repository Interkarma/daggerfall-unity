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
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;

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
        PlayerMouseLook playerMouseLook = null;
        PlayerHealth playerHealth = null;
        StartGameBehaviour startGameBehaviour = null;
        PlayerEntity playerEntity = null;
        DaggerfallEntityBehaviour playerEntityBehaviour = null;
        PlayerDeath playerDeath = null;
        PlayerGPS playerGPS  = null;
        PlayerEnterExit playerEnterExit = null;
        WeatherManager weatherManager = null;
        DaggerfallSky skyRig = null;
        WeaponManager weaponManager = null;
        GameObject mainCameraObject = null;
        GameObject interiorParent = null;
        GameObject exteriorParent = null;
        GameObject dungeonParent = null;
        StreamingWorld streamingWorld = null;
        GameObject streamingTarget = null;
        SaveLoadManager saveLoadManager = null;
        PlayerMotor playerMotor = null;
        FloatingOrigin floatingOrigin = null;
        FPSWeapon[] playerWeapons = new FPSWeapon[2];
        PlayerActivate playerActivate = null;
        CharacterController playerController = null;
        SunlightManager sunlightManager = null;
        ItemHelper itemHelper = null;
        StateManager stateManager = null;
        #endregion

        #region Properties

        public bool IsReady { get; private set; }

        public static bool IsGamePaused
        {
            get { return Instance.isGamePaused; }
        }

        public StateManager StateManager
        {
            get { return (stateManager != null) ? stateManager : stateManager = new StateManager(StateManager.StateTypes.Start); }
            set { stateManager = value; }
        }

        public Camera MainCamera
        {
            get { return (mainCamera) ? mainCamera : mainCamera = GetComponentFromObject<Camera>(MainCameraObject, "MainCamera"); }
            set { mainCamera = value;}
        }

        public GameObject PlayerObject
        {
            get { return (playerObject) ? playerObject : playerObject = GetGameObjectWithTag("Player"); }
            set { playerObject = value; }
        }

        public PlayerMouseLook PlayerMouseLook
        {
            get { return (playerMouseLook) ? playerMouseLook : playerMouseLook = GetComponentFromObject<PlayerMouseLook>(MainCameraObject, "MainCamera"); }
            set { playerMouseLook = value; }
        }

        public PlayerHealth PlayerHealth
        {
            get { return (playerHealth) ? playerHealth : playerHealth = GetComponentFromObject<PlayerHealth>(PlayerObject, "Player"); }
            set { playerHealth = value; }
        }

        public StartGameBehaviour StartGameBehaviour
        {
            get { return (startGameBehaviour) ? startGameBehaviour : startGameBehaviour = GetMonoBehaviour<StartGameBehaviour>(); }
            set { startGameBehaviour = value; }
        }

        public PlayerEntity PlayerEntity
        {
            get { return (playerEntity != null) ? playerEntity : playerEntity = PlayerEntityBehaviour.Entity as PlayerEntity; }
            set { playerEntity = value; }
        }

        public DaggerfallEntityBehaviour PlayerEntityBehaviour
        {
            get { return (playerEntityBehaviour != null) ? playerEntityBehaviour : playerEntityBehaviour = GetComponentFromObject<DaggerfallEntityBehaviour>(PlayerObject); }
            set { playerEntityBehaviour = value; }
        }

        public PlayerDeath PlayerDeath
        {
            get { return (playerDeath) ? playerDeath : playerDeath = GetComponentFromObject<PlayerDeath>(PlayerObject); }
            set { playerDeath = value; }
        }

        public PlayerGPS PlayerGPS
        {
            get { return (playerGPS) ? playerGPS : playerGPS = GetComponentFromObject<PlayerGPS>(PlayerObject);}
            set { playerGPS = value; }
        }

        public PlayerEnterExit PlayerEnterExit
        {
            get { return (playerEnterExit) ? playerEnterExit: playerEnterExit = GetComponentFromObject<PlayerEnterExit>(PlayerObject); }
            set { playerEnterExit = value; }
        }

        public WeatherManager WeatherManager
        {
            get { return (weatherManager) ? weatherManager : weatherManager = GetMonoBehaviour<WeatherManager>(); }
            set { weatherManager = value; }
        }

        public DaggerfallSky SkyRig
        {
            get { return (skyRig) ? skyRig : skyRig = GetMonoBehaviour<DaggerfallSky>(); }
            set { skyRig = value; }
        }

        public WeaponManager WeaponManager
        {
            get { return (weaponManager) ? weaponManager : weaponManager = GetComponentFromObject<WeaponManager>(PlayerObject); }
            set { weaponManager = value; }
        }

        public GameObject MainCameraObject 
        {
            get { return (mainCameraObject) ? mainCameraObject : mainCameraObject = GetGameObjectWithTag("MainCamera") ; }
            set { mainCameraObject = value; }
        }

        public GameObject InteriorParent
        {
            get { return (interiorParent) ? interiorParent : interiorParent = GetGameObjectWithName("Interior"); }
            set { interiorParent = value; }
        }
        public GameObject ExteriorParent
        {
            get { return (exteriorParent) ? exteriorParent : exteriorParent = GetGameObjectWithName("Exterior"); }
            set { exteriorParent = value; }
        }

        public GameObject DungeonParent
        {
            get { return (dungeonParent) ? dungeonParent : dungeonParent = GetGameObjectWithName("Dungeon"); }
            set { dungeonParent = value; }
        }

        public StreamingWorld StreamingWorld
        {
            get { return (streamingWorld) ? streamingWorld : streamingWorld = GetMonoBehaviour<StreamingWorld>(); }
            set { streamingWorld = value; }
        }

        public GameObject StreamingTarget
        {
            get { return (streamingTarget) ? streamingTarget : streamingTarget = GetGameObjectWithName("StreamingTarget"); }
            set { streamingTarget = value; }
        }

        public SaveLoadManager SaveLoadManager
        {
            get { return (saveLoadManager) ? saveLoadManager : saveLoadManager = GetMonoBehaviour<SaveLoadManager>(); }
            set { saveLoadManager = value; }
        }

        public PlayerMotor PlayerMotor
        {
            get { return (playerMotor) ? playerMotor : playerMotor = GetComponentFromObject<PlayerMotor>(PlayerObject); }
            set { playerMotor = value; }
        }

        public FloatingOrigin FloatingOrigin
        {
            get { return (floatingOrigin) ? floatingOrigin :  floatingOrigin = GetMonoBehaviour<FloatingOrigin>(); }
            set { floatingOrigin = value; }
        }

        public FPSWeapon LeftHandWeapon
        {
            get { return (playerWeapons[0]) ? playerWeapons[0] : playerWeapons[0] = GetComponentFromObject<FPSWeapon>(GetGameObjectWithName("Left Hand Weapon") ); }
            set { playerWeapons[0] = value; }

        }

        public FPSWeapon RightHandWeapon
        {
            get { return (playerWeapons[1]) ? playerWeapons[1] : playerWeapons[1] = GetComponentFromObject<FPSWeapon>(GetGameObjectWithName("Right Hand Weapon")); }
            set { playerWeapons[1] = value; }
        }

        public PlayerActivate PlayerActivate
        {
            get { return (playerActivate) ? playerActivate : playerActivate = GetComponentFromObject<PlayerActivate>(PlayerObject); }
            set { playerActivate = value; }
        }

        public CharacterController PlayerController
        {
            get { return (playerController) ? playerController : playerController = GetComponentFromObject<CharacterController>(PlayerObject); }
            set { playerController = value; }
        }

        public SunlightManager SunlightManager
        {
            get { return (sunlightManager) ? sunlightManager : sunlightManager = GetComponentFromObject<SunlightManager>(GetGameObjectWithName("SunLight")); }
            set { sunlightManager = value; }
        }

        public ItemHelper ItemHelper
        {
            get { return (itemHelper != null) ? itemHelper : itemHelper = new ItemHelper(); }
            set { itemHelper = value; }
        }

        public bool IsPlayerOnHUD
        {
            get { return IsHUDTopWindow(); }
        }

        public bool IsPlayerInside 
        {
            get { return PlayerEnterExit.IsPlayerInside;}
        }

        public bool IsPlayerInsideDungeon
        {
            get { return PlayerEnterExit.IsPlayerInsideDungeon; }
        }

        public bool IsPlayerInsideBuilding
        {
            get { return PlayerEnterExit.IsPlayerInsideBuilding; }
        }

        public bool IsPlayerInsidePalace
        {
            get { return PlayerEnterExit.IsPlayerInsideDungeonPalace; }
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

            // Try to set all properties at startup
            GetProperties();

            // Log welcome message
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
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenCharacterSheetWindow);
            }
            else if (InputManager.Instance.ActionStarted(InputManager.Actions.Inventory))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenInventoryWindow);
            }
            else if (InputManager.Instance.ActionStarted(InputManager.Actions.TravelMap))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenTravelMapWindow);
            }

            if (InputManager.Instance.ActionStarted(InputManager.Actions.AutoMap))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenAutomap);
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

                if (hideHUD && DaggerfallUI.Instance.DaggerfallHUD != null)
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

                if (hudDisabledByPause && DaggerfallUI.Instance.DaggerfallHUD != null)
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

        /// <summary>
        /// Checks all of the GameManager's properties at start up.
        /// </summary>
        public void GetProperties()
        {
            var props = typeof(GameManager).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach(PropertyInfo prop in props)
            {
                try
                {
                    prop.GetValue(GameManager.instance, null);
                    //DaggerfallUnity.LogMessage(string.Format("GameManager Startup...property: {0} value: {1}", prop.Name, value), true);
                }
                catch(Exception ex)
                {
                    Debug.Log(string.Format("{0} | GameManager Failed to get value for prop: {1}", ex.Message, prop.Name));
                }
            }

            if (GameManager.HasInstance)
            {
                IsReady = true;
                DaggerfallUnity.LogMessage("GameManager ready.");
            }
                
        }

        /// <summary>
        /// Get monobehaviour object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetMonoBehaviour<T>() where T : MonoBehaviour
        {
            T result = (T)GameObject.FindObjectOfType<T>();
            if (result == null)
                throw new Exception(string.Format("GameManager could not find {0}.", typeof(T)));
            else
                return result;
        }


        /// <summary>
        /// Get a component from an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">Object to check for component</param>
        /// <param name="tag">optional; if object is null, will attempt to find object with tag</param>
        /// <returns></returns>
        public static T GetComponentFromObject<T>(GameObject obj, string tag = null) where T : Component
        {
            T result = default(T);    
            if(obj == null && !string.IsNullOrEmpty(tag))
            {
                obj = GetGameObjectWithTag(tag);
            }
            else if(obj == null && string.IsNullOrEmpty(tag))
            {
                throw new Exception(string.Format("GameManager could not find component type {0} - both object & string were null.", typeof(T), obj.name));  
            }
            
            if(obj != null)
            {
                result = obj.GetComponent<T>();
            }
            if (result == null)
                throw new Exception(string.Format("GameManager could not find component type {0} on object {1}.", typeof(T), obj.name));
            else
                return result;
        }

        /// <summary>
        /// Find a gameobject by tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static GameObject GetGameObjectWithTag(string tag)
        {
            if(string.IsNullOrEmpty(tag))
                throw new Exception(string.Format("GameManager could not find GameObject with tag as string was null or empty"));
            GameObject result = GameObject.FindGameObjectWithTag(tag);
            if (result == null)
                throw new Exception(string.Format("GameManager could not find GameObject with tag {0}", tag));
            else
                return result;
        }

        /// <summary>
        /// Find a gameobject by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject GetGameObjectWithName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception(string.Format("GameManager could not find GameObject with name as string was null or empty"));
            GameObject result = GameObject.Find(name);
            if (result == null)
                throw new Exception(string.Format("GameManager could not find GameObject with name {0}", name));
            else
                return result;
        }

        #endregion

        #region Event Handlers

        void PathErrorMessageBox_OnClose()
        {
            Application.Quit();
        }

        #endregion
    }
}
