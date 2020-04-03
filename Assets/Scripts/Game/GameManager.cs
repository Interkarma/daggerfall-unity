// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// GameManager singleton class.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Fields
        public const float classicUpdateInterval = 0.0625f;        // Update every 1/16 of a second. An approximation of classic's update loop, which varies with framerate.

        public bool Verbose = false;
        bool isGamePaused = false;
        float savedTimeScale;
        float classicUpdateTimer = 0;                       // Timer for matching classic's update loop
        bool classicUpdate = false;                         // True when reached a classic update
        float initialQualitySettingsShadowDistance;
        //Texture2D pauseScreenshot;

        GameObject playerObject = null;
        Camera mainCamera = null;
        RetroRenderer retroRenderer = null;
        PlayerMouseLook playerMouseLook = null;
        PlayerHealth playerHealth = null;
        VitalsChangeDetector vitalsChangeDetector = null;
        StartGameBehaviour startGameBehaviour = null;
        PlayerEntity playerEntity = null;
        DaggerfallEntityBehaviour playerEntityBehaviour = null;
        EntityEffectBroker entityEffectBroker = null;
        EntityEffectManager playerEffectManager = null;
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
        AcrobatMotor acrobatMotor = null;
        ClimbingMotor climbingMotor = null;
        PlayerSpeedChanger speedChanger = null;
        FrictionMotor frictionMotor = null;
        FloatingOrigin floatingOrigin = null;
        FPSWeapon[] playerWeapons = new FPSWeapon[2];
        FPSSpellCasting playerSpellCasting = null;
        PlayerActivate playerActivate = null;
        CharacterController playerController = null;
        SunlightManager sunlightManager = null;
        ItemHelper itemHelper = null;
        StateManager stateManager = null;
        Automap interiorAutomap = null;
        ExteriorAutomap exteriorAutomap = null;
        QuestMachine questMachine = null;
		TransportManager transportManager = null;
        TalkManager talkManager = null;
        GuildManager guildManager = null;
        QuestListsManager questListsManager = null;

        #endregion

        #region Properties

        public bool IsReady { get; private set; }

        public static bool IsGamePaused
        {
            get { return Instance.isGamePaused; }
        }

        public static bool ClassicUpdate
        {
            get { return Instance.classicUpdate; }
        }

        public bool DisableAI { get; set; }

        public StateManager StateManager
        {
            get { return (stateManager != null) ? stateManager : stateManager = new StateManager(StateManager.StateTypes.None); }
            set { stateManager = value; }
        }

        public Camera MainCamera
        {
            get { return (mainCamera) ? mainCamera : mainCamera = GetComponentFromObject<Camera>(MainCameraObject, "MainCamera"); }
            set { mainCamera = value;}
        }

        public RetroRenderer RetroRenderer
        {
            get { return (retroRenderer) ? retroRenderer : retroRenderer = GetMonoBehaviour<RetroRenderer>(false); }
            set { retroRenderer = value; }
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

        public VitalsChangeDetector VitalsChangeDetector
        {
            get { return (vitalsChangeDetector) ? vitalsChangeDetector : vitalsChangeDetector = GetComponentFromObject<VitalsChangeDetector>(PlayerObject, "Player"); }
            set { vitalsChangeDetector = value; }
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

        public EntityEffectManager PlayerEffectManager
        {
            get { return (playerEffectManager != null) ? playerEffectManager : playerEffectManager = GetComponentFromObject<EntityEffectManager>(PlayerObject); }
            set { playerEffectManager = value; }
        }

        public EntityEffectBroker EntityEffectBroker
        {
            get { return (entityEffectBroker != null) ? entityEffectBroker : entityEffectBroker = GetMonoBehaviour<EntityEffectBroker>(); }
            set { entityEffectBroker = value; }
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
        
        public PlayerSpeedChanger SpeedChanger
        {
            get { return (speedChanger) ? speedChanger : speedChanger = GetComponentFromObject<PlayerSpeedChanger>(PlayerObject); }
                set { speedChanger = value; }
        }

        public PlayerMotor PlayerMotor
        {
            get { return (playerMotor) ? playerMotor : playerMotor = GetComponentFromObject<PlayerMotor>(PlayerObject); }
            set { playerMotor = value; }
        }

        public AcrobatMotor AcrobatMotor
        {
            get { return (acrobatMotor) ? acrobatMotor : acrobatMotor = GetComponentFromObject<AcrobatMotor>(PlayerObject); }
            set { acrobatMotor = value; }
        }

        public ClimbingMotor ClimbingMotor
        {
            get { return (climbingMotor) ? climbingMotor : climbingMotor = GetComponentFromObject<ClimbingMotor>(PlayerObject); }
            set { climbingMotor = value; }
        }

        public FrictionMotor FrictionMotor
        {
            get { return (frictionMotor) ? frictionMotor : frictionMotor = GetComponentFromObject<FrictionMotor>(PlayerObject); }
            set { frictionMotor = value; }
        }

        public FloatingOrigin FloatingOrigin
        {
            get { return (floatingOrigin) ? floatingOrigin :  floatingOrigin = GetMonoBehaviour<FloatingOrigin>(); }
            set { floatingOrigin = value; }
        }

        //public FPSWeapon LeftHandWeapon
        //{
        //    get { return (playerWeapons[0]) ? playerWeapons[0] : playerWeapons[0] = GetComponentFromObject<FPSWeapon>(GetGameObjectWithName("Left Hand Weapon") ); }
        //    set { playerWeapons[0] = value; }
        //}

        public FPSWeapon RightHandWeapon
        {
            get { return (playerWeapons[1]) ? playerWeapons[1] : playerWeapons[1] = GetComponentFromObject<FPSWeapon>(GetGameObjectWithName("Right Hand Weapon")); }
            set { playerWeapons[1] = value; }
        }

        public FPSSpellCasting PlayerSpellCasting
        {
            get { return (playerSpellCasting) ? playerSpellCasting : playerSpellCasting = GetComponentFromObject<FPSSpellCasting>(PlayerObject); }
            set { playerSpellCasting = value; }
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

        public Automap InteriorAutomap
        {
            get { return (interiorAutomap != null) ? interiorAutomap : interiorAutomap = GetComponentFromObject<Automap>(GetGameObjectWithName("InteriorAutomap")); }
            set { interiorAutomap = value; }
        }

        public ExteriorAutomap ExteriorAutomap
        {
            get { return (exteriorAutomap != null) ? exteriorAutomap : exteriorAutomap = GetComponentFromObject<ExteriorAutomap>(GetGameObjectWithName("ExteriorAutomap")); }
            set { exteriorAutomap = value; }
        }

        public QuestMachine QuestMachine
        {
            get { return (questMachine) ? questMachine : questMachine = GetMonoBehaviour<QuestMachine>(); }
            set { questMachine = value; }
        }

        public TransportManager TransportManager
        {
            get { return (transportManager) ? transportManager : transportManager = GetComponentFromObject<TransportManager>(PlayerObject); }
            set { transportManager = value; }
        }

        public TalkManager TalkManager
        {
            get { return (talkManager) ? talkManager : talkManager = GetComponentFromObject<TalkManager>(GetGameObjectWithName("TalkManager")); }
            set { talkManager = value; }
        }

        public GuildManager GuildManager
        {
            get { return (guildManager != null) ? guildManager : guildManager = new GuildManager(); }
            set { guildManager = value; }
        }

        public QuestListsManager QuestListsManager
        {
            get { return (questListsManager != null) ? questListsManager : questListsManager = new QuestListsManager(); }
            set { questListsManager = value; }
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

        public bool IsPlayerInsideCastle
        {
            get { return PlayerEnterExit.IsPlayerInsideDungeonCastle; }
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

        void Awake()
        {
            if (!SetupSingleton())
            {
                Destroy(this);
                return;
            }
        }

        void Start()
        {
            // Try to set all properties at startup
            //GetProperties();

            // Cache initial QualitySettings shadow distance
            initialQualitySettingsShadowDistance = QualitySettings.shadowDistance;

            // Always start game paused
            PauseGame(true);

            // Log welcome message
            Debug.Log("Welcome to Daggerfall Unity " + VersionInfo.DaggerfallUnityVersion);
        }

        void FixedUpdate()
        {
            if (!IsPlayingGame())
            {
                classicUpdate = false;
                return;
            }

            // Update timer that approximates the timing of original Daggerfall's game update loop
            classicUpdateTimer += Time.deltaTime;
            if (classicUpdateTimer >= classicUpdateInterval)
            {
                classicUpdateTimer = 0;
                classicUpdate = true;
            }
            else
                classicUpdate = false;
        }

        void Update()
        {
            // Don't process game manager input messages when game not running
            if (!IsPlayingGame())
                return;

            // Post message to open options dialog on escape during gameplay
            if (InputManager.Instance.ActionComplete(InputManager.Actions.Escape))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenPauseOptionsDialog);
            }

            // Handle in-game windows
            if (InputManager.Instance.ActionComplete(InputManager.Actions.CharacterSheet))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenCharacterSheetWindow);
            }
            else if (InputManager.Instance.ActionComplete(InputManager.Actions.Inventory))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenInventoryWindow);
            }
            else if (InputManager.Instance.ActionComplete(InputManager.Actions.TravelMap))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenTravelMapWindow);
            }
            else if (InputManager.Instance.ActionComplete(InputManager.Actions.Rest))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenRestWindow);
            }
            else if (InputManager.Instance.ActionComplete(InputManager.Actions.Transport))
            {
	            DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenTransportWindow);
            }
            else if (InputManager.Instance.ActionComplete(InputManager.Actions.LogBook))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenQuestJournalWindow);
            }
            else if (InputManager.Instance.ActionComplete(InputManager.Actions.NoteBook))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenNotebookWindow);
            }
            else if (InputManager.Instance.ActionComplete(InputManager.Actions.CastSpell))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenSpellBookWindow);
            }
            else if (InputManager.Instance.ActionComplete(InputManager.Actions.UseMagicItem))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenUseMagicItemWindow);
            }

            if (InputManager.Instance.ActionComplete(InputManager.Actions.Status))
            {
                DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiStatusInfo);
            }

            if (InputManager.Instance.ActionComplete(InputManager.Actions.AutoMap))
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
                if (SaveLoadManager.Instance.HasQuickSave(GameManager.Instance.PlayerEntity.Name))
                {
                    SaveLoadManager.Instance.QuickLoad();
                }
            }
        }

        #endregion

        #region Public Methods

        bool hudDisabledByPause = false;
        public void PauseGame(bool pause, bool hideHUD = false)
        {
            DaggerfallUI.Instance.ShowVersionText = false;

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

        /// <summary>
        /// Determines if enemies are nearby. Uses include whether player is able to rest or not.
        /// Based on distance to nearest monster, and if monster can actually sense player.
        /// </summary>
        /// <param name="resting">Is player initiating or continuing rest?</param>
        /// <param name="includingPacified">Include pacified enemies in this test?</param>
        /// <returns>True if enemies are nearby.</returns>
        public bool AreEnemiesNearby(bool resting = false, bool includingPacified = false)
        {
            const float spawnDistance = 1024 * MeshReader.GlobalScale;
            const float restingDistance = 12f;

            bool areEnemiesNearby = false;
            DaggerfallEntityBehaviour[] entityBehaviours = FindObjectsOfType<DaggerfallEntityBehaviour>();
            for (int i = 0; i < entityBehaviours.Length; i++)
            {
                DaggerfallEntityBehaviour entityBehaviour = entityBehaviours[i];
                if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
                {
                    EnemySenses enemySenses = entityBehaviour.GetComponent<EnemySenses>();
                    if (enemySenses)
                    {
                        // Check if enemy can actively target player
                        bool enemyCanSeePlayer = enemySenses.Target == Instance.PlayerEntityBehaviour && enemySenses.TargetInSight;

                        // Allow for a shorter test distance if enemy is unaware of player while resting
                        if (resting && !enemyCanSeePlayer && Vector3.Distance(entityBehaviour.transform.position, PlayerController.transform.position) > restingDistance)
                            continue;

                        // Can enemy see player or is close enough they would be spawned in classic?
                        if (enemyCanSeePlayer || enemySenses.WouldBeSpawnedInClassic)
                        {
                            // Is it hostile or pacified?
                            EnemyMotor enemyMotor = entityBehaviour.GetComponent<EnemyMotor>();
                            EnemyEntity enemyEntity = entityBehaviour.Entity as EnemyEntity;
                            if (includingPacified || (enemyMotor.IsHostile && enemyEntity.MobileEnemy.Team != MobileTeams.PlayerAlly))
                            {
                                areEnemiesNearby = true;
                                break;
                            }
                        }
                    }
                }
            }

            // Also check for enemy spawners that might emit an enemy
            FoeSpawner[] spawners = FindObjectsOfType<FoeSpawner>();
            for (int i = 0; i < spawners.Length; i++)
            {
                // Is a spawner inside min distance?
                if (Vector3.Distance(spawners[i].transform.position, PlayerController.transform.position) < spawnDistance)
                {
                    areEnemiesNearby = true;
                    break;
                }
            }

            return areEnemiesNearby;
        }

        /// <summary>
        /// Gets how many enemies of a given type exist.
        /// </summary>
        /// <param name="type">Enemy type to search for.</param>
        /// <param name="stopLookingIfFound">Return as soon as an enemy of given type is found.</param>
        /// <returns>Number of this enemy type.</returns>
        public int HowManyEnemiesOfType(MobileTypes type, bool stopLookingIfFound = false, bool includingPacified = false)
        {
            int numberOfEnemies = 0;
            DaggerfallEntityBehaviour[] entityBehaviours = FindObjectsOfType<DaggerfallEntityBehaviour>();
            for (int i = 0; i < entityBehaviours.Length; i++)
            {
                DaggerfallEntityBehaviour entityBehaviour = entityBehaviours[i];
                if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
                {
                    EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
                    if (entity.MobileEnemy.ID == (int)type)
                    {
                        // Is it hostile or pacified?
                        EnemyMotor enemyMotor = entityBehaviour.GetComponent<EnemyMotor>();
                        if (includingPacified || (enemyMotor.IsHostile && entity.Team != MobileTeams.PlayerAlly))
                        {
                            numberOfEnemies++;
                            if (stopLookingIfFound)
                                return numberOfEnemies;
                        }
                    }
                }
            }

            // Also check for enemy spawners that might emit an enemy
            FoeSpawner[] spawners = FindObjectsOfType<FoeSpawner>();
            for (int i = 0; i < spawners.Length; i++)
            {
                // Is a spawner inside min distance?
                if (spawners[i].FoeType == type)
                {
                    numberOfEnemies++;
                    if (stopLookingIfFound)
                        return numberOfEnemies;
                }
            }

            return numberOfEnemies;
        }

        /// <summary>
        /// Clears the area of enemies.
        /// </summary>
        public void ClearEnemies()
        {
            DaggerfallEntityBehaviour[] entityBehaviours = FindObjectsOfType<DaggerfallEntityBehaviour>();
            for (int i = 0; i < entityBehaviours.Length; i++)
            {
                DaggerfallEntityBehaviour entityBehaviour = entityBehaviours[i];
                if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
                    Destroy(entityBehaviour.gameObject);
            }

            // Also check for enemy spawners that might emit an enemy
            FoeSpawner[] spawners = FindObjectsOfType<FoeSpawner>();
            for (int i = 0; i < spawners.Length; i++)
                Destroy(spawners[i].gameObject);
        }

        /// <summary>
        /// Make all enemies in an area go hostile.
        /// </summary>
        public void MakeEnemiesHostile()
        {
            DaggerfallEntityBehaviour[] entityBehaviours = FindObjectsOfType<DaggerfallEntityBehaviour>();
            for (int i = 0; i < entityBehaviours.Length; i++)
            {
                DaggerfallEntityBehaviour entityBehaviour = entityBehaviours[i];
                if (entityBehaviour.EntityType == EntityTypes.EnemyMonster || entityBehaviour.EntityType == EntityTypes.EnemyClass)
                {
                    EnemyMotor enemyMotor = entityBehaviour.GetComponent<EnemyMotor>();
                    if (enemyMotor)
                    {
                        enemyMotor.IsHostile = true;
                    }
                }
            }
        }

        #endregion

        #region Public Static Methods

        public static bool FindSingleton(out GameManager singletonOut)
        {
            singletonOut = GameObject.FindObjectOfType<GameManager>();
            if (singletonOut == null)
            {
                DaggerfallUnity.LogMessage("Could not locate GameManager GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        public static void UpdateShadowDistance()
        {
            if (Instance.playerEnterExit.IsPlayerInsideDungeon)
                QualitySettings.shadowDistance = DaggerfallUnity.Settings.DungeonShadowDistance;
            else if (Instance.PlayerEnterExit.IsPlayerInsideBuilding)
                QualitySettings.shadowDistance = DaggerfallUnity.Settings.InteriorShadowDistance;
            else if (!Instance.PlayerEnterExit.IsPlayerInside)
                QualitySettings.shadowDistance = DaggerfallUnity.Settings.ExteriorShadowDistance;
            else
                QualitySettings.shadowDistance = Instance.initialQualitySettingsShadowDistance;
        }

        public static void UpdateShadowResolution()
        {
            switch (DaggerfallUnity.Settings.ShadowResolutionMode)
            {
                case 0:
                    QualitySettings.shadowResolution = ShadowResolution.Low;
                    break;
                case 1:
                default:
                    QualitySettings.shadowResolution = ShadowResolution.Medium;
                    break;
                case 2:
                    QualitySettings.shadowResolution = ShadowResolution.High;
                    break;
                case 3:
                    QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                    break;
            }
        }

        #endregion

        #region Private Methods

        private bool SetupSingleton()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (instance != this)
            {
                if (Application.isPlaying)
                {
                    DaggerfallUnity.LogMessage("Multiple GameManager instances detected in scene!", true);
                    Destroy(gameObject);
                }
            }

            return instance == this;
        }



        bool IsHUDTopWindow()
        {
            IUserInterfaceWindow topWindow = DaggerfallUI.UIManager.TopWindow;
            if (topWindow is DaggerfallHUD)
                return true;

            return false;
        }

        /// <summary>
        /// Returns true when gameplay is active.
        /// </summary>
        public bool IsPlayingGame()
        {
            // Game not active when paused
            if (isGamePaused)
                return false;

            // Game not active when SaveLoadManager not present or when loading
            if (SaveLoadManager.Instance == null || SaveLoadManager.Instance.LoadInProgress)
                return false;

            // Game not active when top window is neither null or HUD
            IUserInterfaceWindow topWindow = DaggerfallUI.UIManager.TopWindow;
            if (topWindow != null && !(topWindow is DaggerfallHUD))
                return false;

            return true;
        }

        //// Takes a screenshot at end of current frame
        //IEnumerator TakeScreenshot()
        //{
        //    yield return new WaitForEndOfFrame();

        //    pauseScreenshot = new Texture2D(Screen.width, Screen.height);
        //    pauseScreenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        //    pauseScreenshot.Apply();
        //}

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
                    object obj = prop.GetValue(GameManager.instance, null);
                    if(Verbose)
                        Debug.Log(string.Format("GameManager Startup...property: {0} value: {1}", prop.Name, obj.ToString()));
                }
                catch(Exception ex)
                {
                    if (Verbose)
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
        public static T GetMonoBehaviour<T>(bool errorIfNotFound = true) where T : MonoBehaviour
        {
            T result = (T)GameObject.FindObjectOfType<T>();
            if (result == null && errorIfNotFound)
            {
                string errorText = string.Format("GameManager could not find {0}.", typeof(T));
                Debug.LogError(errorText);
                throw new Exception(errorText);
            }
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
                string errorText = string.Format("GameManager could not find component type {0} - both object & string were null.", typeof(T));
                Debug.LogError(errorText);
                throw new Exception(errorText);
            }
            
            if(obj != null)
            {
                result = obj.GetComponent<T>();
            }
            if (result == null)
            {
                string errorText = string.Format("GameManager could not find component type {0} on object {1}.", typeof(T), obj.name);
                Debug.LogError(errorText);
                throw new Exception(errorText);
            }
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
            if (string.IsNullOrEmpty(tag))
            {
                string errorText = string.Format("GameManager could not find GameObject with tag as string was null or empty");
                Debug.LogError(errorText);
                throw new Exception(errorText);
            }
            GameObject result = GameObject.FindGameObjectWithTag(tag);
            if (result == null)
            {
                string errorText = string.Format("GameManager could not find GameObject with tag {0}", tag);
                Debug.LogError(errorText);
                throw new Exception(errorText);
            }
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
            {
                string errorText = string.Format("GameManager could not find GameObject with name as string was null or empty");
                Debug.LogError(errorText);
                throw new Exception(errorText);
            }
            GameObject result = GameObject.Find(name);
            if (result == null)
            {
                string errorText = string.Format("GameManager could not find GameObject with name {0}", name);
                Debug.LogError(errorText);
                throw new Exception(errorText);
            }
            else
                return result;
        }

        #endregion

        #region Event Handlers

        //void OnLevelWasLoaded(int index)
        //{
        //    //if(index == SceneControl.GameSceneIndex)
        //    //{
        //    //    StateManager.ChangeState(StateManager.StateTypes.Start);
        //    //}
        //    //else if(index == SceneControl.StartupSceneIndex)
        //    //{
        //    //    StateManager.ChangeState(StateManager.StateTypes.Setup);
        //    //}
        //    GetProperties();
        //}

        void PathErrorMessageBox_OnClose()
        {
            Application.Quit();
        }


        // OnEncounter
        public delegate void OnEncounterEventHandler();
        public static event OnEncounterEventHandler OnEncounter;
        public virtual void RaiseOnEncounterEvent()
        {
            if (OnEncounter != null)
                OnEncounter();
        }

        #endregion
    }
}
