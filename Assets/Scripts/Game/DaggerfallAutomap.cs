// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Michael Rauter (a.k.a. Nystul)
// Contributors:    Lypyl, Interkarma
// 
// Notes:
//

//#define DEBUG_RAYCASTS
//#define DEBUG_SHOW_RAYCAST_TIMES

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;
using Wenzil.Console;

namespace DaggerfallWorkshop.Game
{
    using AutomapGeometryModelState = DaggerfallAutomap.AutomapGeometryBlockState.AutomapGeometryBlockElementState.AutomapGeometryModelState;
    using AutomapGeometryBlockElementState = DaggerfallAutomap.AutomapGeometryBlockState.AutomapGeometryBlockElementState;

    /// <summary>
    /// this class provides the automap core functionality like geometry creation and discovery mechanism 
    /// </summary>
    public class DaggerfallAutomap : MonoBehaviour
    {
        #region Singleton
        private static DaggerfallAutomap _instance;

        public static DaggerfallAutomap instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<DaggerfallAutomap>();
                return _instance;
            }
            private set { _instance = value; }
        }
        #endregion

        /// <summary>
        /// class to store state of discovery of a dungeon block or interior, this state can be used to restore state of GameObject gameobjectGeometry
        /// this class basically maps the two fields/states (MeshRenderer active state and Material shader keyword "RENDER_IN_GRAYSCALE") that are
        /// used for discovery state of the models inside GameObject gameobjectGeometry when an interior is loaded into this GameObject
        /// it is also used as type inside the blocks list in AutomapGeometryDungeonState
        /// </summary>
        public class AutomapGeometryBlockState
        {
            public class AutomapGeometryBlockElementState
            {
                public class AutomapGeometryModelState
                {
                    public bool discovered; /// discovered before (render model)
                    public bool visitedInThisRun; /// visited in this dungeon run (if true render in color, otherwise grayscale)
                }
                public List<AutomapGeometryModelState> models;
            }
            public List<AutomapGeometryBlockElementState> blockElements;
            public String blockName;
        }

        /// <summary>
        /// class to store state of discovery of a dungeon, this state can be used to restore state of GameObject gameobjectGeometry
        /// this class basically maps the two fields/states (MeshRenderer active state and Material shader keyword "RENDER_IN_GRAYSCALE") that are
        /// used for discovery state of the models inside GameObject gameobjectGeometry when a dungeon is loaded into this GameObject
        /// </summary>
        public class AutomapGeometryDungeonState
        {
            public String locationName; /// name of the dungeon location
            public ulong timeInSecondsLastVisited; /// time in seconds (from DaggerfallDateTime) when player last visited the dungeon (used to store only the n last dungeons in save games)
            public bool entranceDiscovered; /// indicates if the dungeon entrance has already been discovered
            public List<AutomapGeometryBlockState> blocks;
        }

        // dungeon state is of type AutomapGeometryDungeonState which has its models in 4th hierarchy level
        AutomapGeometryDungeonState automapGeometryDungeonState = null;
        // interior state is of type AutomapGeometryBlockState which has its models in 3rd hierarchy level
        AutomapGeometryBlockState automapGeometryInteriorState = null;

        /// <summary>
        /// this dictionary is used to store the discovery state of dungeons in the game world
        /// the AutomapGeometryDungeonState is stored for each dungeon in this dictionary identified by its identifier string
        /// </summary>
        public Dictionary<string, AutomapGeometryDungeonState> dictAutomapDungeonsDiscoveryState = new Dictionary<string, AutomapGeometryDungeonState>();


        int numberOfDungeonMemorized = 1; /// 0... vanilla daggerfall behavior, 1... remember last visited dungeon, n... remember n visited dungeons

        /// <summary>
        /// sets the number of dungeons that are memorized
        /// </summary>
        public void setNumberOfDungeonMemorized(int n)
        {
            numberOfDungeonMemorized = n;
        }

        /// <summary>
        /// GetState() method for save system integration
        /// </summary>
        public Dictionary<string, AutomapGeometryDungeonState> GetState()
        {
            saveStateAutomapDungeon(false);
            return dictAutomapDungeonsDiscoveryState;
        }
        
        /// <summary>
        /// SetState() method for save system integration
        /// </summary>
        public void SetState(Dictionary<string, AutomapGeometryDungeonState> savedDictAutomapDungeonsDiscoveryState)
        {
            dictAutomapDungeonsDiscoveryState = savedDictAutomapDungeonsDiscoveryState;

            if ((GameManager.Instance.IsPlayerInsidePalace)||(GameManager.Instance.IsPlayerInsideDungeon))
            {                
                InitWhenInInteriorOrDungeon(null, true);
                restoreStateAutomapDungeon();
            }           
        }

        #region Fields

        const float raycastDistanceDown = 3.0f; // 3 meters should be enough (note: flying too high will result in geometry not being revealed by this raycast
        const float raycastDistanceViewDirection = 30.0f; // don't want to make it too easy to discover big halls - although it shouldn't be to small as well
        const float raycastDistanceEntranceMarkerReveal = 100.0f;

        const float scanRateGeometryDiscoveryInHertz = 5.0f; // n times per second the discovery of new geometry/meshes is checked

        GameObject gameobjectAutomap = null; // used to hold reference to instance of GameObject "Automap" (which has script Game/DaggerfallAutomap.cs attached)

        GameObject gameobjectGeometry = null; // used to hold reference to instance of GameObject with level geometry used for automap        

        int layerAutomap; // layer used for level geometry of automap

        GameObject gameObjectCameraAutomap = null; // used to hold reference to GameObject to which camera class for automap camera is attached to
        Camera cameraAutomap = null; // camera for automap camera
        
        GameObject gameobjectAutomapKeyLight = null; // instead this script will use its own key light to lighten the level geometry used for automap
        GameObject gameobjectAutomapFillLight = null; // and fill light
        GameObject gameobjectAutomapBackLight = null; // and back light

        GameObject gameObjectPlayerAdvanced = null; // used to hold reference to instance of GameObject "PlayerAdvanced"

        float slicingBiasY; // y-bias from player y-position of geometry slice plane (set via Property SlicingBiasY used by DaggerfallAutomapWindow script to set value)

        Vector3 rotationPivotAxisPosition; // position of the rotation pivot axis (set via Property RotationPivotAxisPosition used by DaggerfallAutomapWindow script to set value)

        bool isOpenAutomap = false; // flag that indicates if automap window is open (set via Property IsOpenAutomap triggered by DaggerfallAutomapWindow script)

        // automap render mode is a setting that influences geometry rendered above slicing plane
        public enum AutomapRenderMode
        {
            Cutout = 0,
            Wireframe = 1,
            Transparent = 2
        };

        AutomapRenderMode currentAutomapRenderMode = AutomapRenderMode.Cutout; // currently selected automap render mode (default value: cutout)        

        // flag that indicates if external script should reset automap settings (set via Property ResetAutomapSettingsSignalForExternalScript checked and erased by DaggerfallAutomapWindow script)
        // this might look weirds - why not just notify the DaggerfallAutomapWindow class you may ask... - I wanted to make DaggerfallAutomap inaware and independent of the actual GUI implementation
        // so communication will always be only from DaggerfallAutomapWindow to DaggerfallAutomap class - so into other direction it works in that way that DaggerfallAutomap will pull
        // from DaggerfallAutomapWindow via flags - this is why this flag and its Property ResetAutomapSettingsSignalForExternalScript exist
        bool resetAutomapSettingsFromExternalScript = false;

        GameObject gameobjectBeacons = null; // collector GameObject to hold beacons
        GameObject gameobjectPlayerMarkerArrow = null; // GameObject which will hold player marker arrow
        GameObject gameobjectBeaconPlayerPosition = null; // GameObject which will hold player marker ray (red ray)
        GameObject gameobjectBeaconEntrancePosition = null; // GameObject which will hold (dungeon) entrance marker ray (green ray)
        GameObject gameobjectBeaconRotationPivotAxis = null; // GameObject which will hold rotation pivot axis ray (blue ray)

        GameObject gameObjectEntrancePositionCubeMarker = null; // used for entrance marker discovery

        Collider playerCollider = null;

        // specifies which object should have focus ()
        public enum AutomapFocusObject
        {
            Player = 0,
            Entrance = 1,
            RotationAxis = 2
        };

        AutomapFocusObject focusObject;

        readonly Vector3 rayPlayerPosOffset = new Vector3(-0.1f, 0.0f, +0.1f); // small offset to prevent ray for player position to be exactly in the same position as the rotation pivot axis
        readonly Vector3 rayEntrancePosOffset = new Vector3(0.1f, 0.0f, +0.1f); // small offset to prevent ray for dungeon entrance to be exactly in the same position as the rotation pivot axis

        bool debugTeleportMode = false;

        #endregion

        #region Properties

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to get automap layer
        /// </summary>
        public int LayerAutomap
        {
            get { return (layerAutomap); }
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to get automap camera
        /// </summary>
        public Camera CameraAutomap
        {
            get { return (cameraAutomap); }
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to check if it should reset automap settings (and if it does it will erase flag)
        /// </summary>
        public bool ResetAutomapSettingsSignalForExternalScript
        {
            get { return (resetAutomapSettingsFromExternalScript); }
            set { resetAutomapSettingsFromExternalScript = value; }
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to propagate its slicingBiasY (y-offset from the player y position)
        /// </summary>
        public float SlicingBiasY
        {
            get { return (slicingBiasY); }
            set { slicingBiasY = value; }
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to propagate its rotation pivot axis position (dependent on selected view)
        /// </summary>
        public Vector3 RotationPivotAxisPosition
        {
            get { return (rotationPivotAxisPosition); }
            set { rotationPivotAxisPosition = value; }
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to propagate if the automap window is open or not
        /// </summary>
        public bool IsOpenAutomap
        {
            set { isOpenAutomap = value; }
        }

        /// <summary>
        /// used to set or get the debug teleport mode
        /// </summary>
        public bool DebugTeleportMode
        {
            get { return (debugTeleportMode); }
            set { debugTeleportMode = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to update when automap window was pushed - TODO: check if this can done with an event (if events work with gui windows)
        /// </summary>
        public void updateAutomapStateOnWindowPush()
        {
            gameobjectGeometry.SetActive(true); // enable automap level geometry for revealing (so raycasts can hit colliders of automap level geometry)

            gameobjectBeacons.SetActive(true);

            gameobjectPlayerMarkerArrow.transform.position = gameObjectPlayerAdvanced.transform.position;
            gameobjectPlayerMarkerArrow.transform.rotation = gameObjectPlayerAdvanced.transform.rotation;

            // create camera (if not present) that will render automap level geometry
            createAutomapCamera();

            // create lights that will light automap level geometry
            createLightsForAutomapGeometry();

            gameobjectBeaconPlayerPosition.transform.position = gameObjectPlayerAdvanced.transform.position + rayPlayerPosOffset;

            updateSlicingPositionY();
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to update when automap window was popped - TODO: check if this can done with an event (if events work with gui windows)
        /// </summary>
        public void updateAutomapStateOnWindowPop()
        {
            // about gameobjectGeometry.SetActive(false):
            // this will not be enough if we will eventually allow gui windows to be opened while exploring the world
            // then it will be necessary to either only disable the colliders on the automap level geometry or
            // make player collision ignore colliders of objects in automap layer - I would clearly prefer this option
            gameobjectGeometry.SetActive(false); // disable gameobjectGeometry so player movement won't be affected by geometry colliders of automap level geometry
            
            gameobjectBeacons.SetActive(false);

            if ((GameManager.Instance.PlayerEnterExit.IsPlayerInside) && ((GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding) || (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon) || (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeonPalace)))
            {
                // and get rid of lights used to light the automap level geometry
                UnityEngine.Object.Destroy(gameobjectAutomapKeyLight);
                UnityEngine.Object.Destroy(gameobjectAutomapFillLight);
                UnityEngine.Object.Destroy(gameobjectAutomapBackLight);
            }

            // destroy the camera so it does not use system resources
            if (gameObjectCameraAutomap != null)
            {
                UnityEngine.Object.Destroy(gameObjectCameraAutomap);
            }
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to update when anything changed that requires DaggerfallAutomap to update - TODO: check if this can done with an event (if events work with gui windows)
        /// </summary>
        public void forceUpdate()
        {
            Update();
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to switch to the next available automap rendering mode
        /// </summary>
        public void switchToNextAutomapRenderMode()
        {
            int numberOfAutomapRenderModes = Enum.GetNames(typeof(AutomapRenderMode)).Length;
            currentAutomapRenderMode++;
            if ((int)currentAutomapRenderMode > numberOfAutomapRenderModes - 1) // first mode is mode 0 -> so use numberOfAutomapRenderModes-1 for comparison
                currentAutomapRenderMode = 0;
            switch (currentAutomapRenderMode)
            {
                default:
                case AutomapRenderMode.Transparent:
                    Shader.DisableKeyword("AUTOMAP_RENDER_MODE_WIREFRAME");
                    Shader.EnableKeyword("AUTOMAP_RENDER_MODE_TRANSPARENT");
                    break;
                case AutomapRenderMode.Wireframe:
                    Shader.EnableKeyword("AUTOMAP_RENDER_MODE_WIREFRAME");
                    Shader.DisableKeyword("AUTOMAP_RENDER_MODE_TRANSPARENT");
                    break;
                case AutomapRenderMode.Cutout:
                    Shader.DisableKeyword("AUTOMAP_RENDER_MODE_WIREFRAME");
                    Shader.DisableKeyword("AUTOMAP_RENDER_MODE_TRANSPARENT");
                    break;
            }
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to switch to automap rendering mode "transparent"
        /// </summary>
        public void switchToAutomapRenderModeTransparent()
        {
            currentAutomapRenderMode = AutomapRenderMode.Transparent;
            Shader.DisableKeyword("AUTOMAP_RENDER_MODE_WIREFRAME");
            Shader.EnableKeyword("AUTOMAP_RENDER_MODE_TRANSPARENT");
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to switch to automap rendering mode "wireframe"
        /// </summary>
        public void switchToAutomapRenderModeWireframe()
        {
            currentAutomapRenderMode = AutomapRenderMode.Wireframe;
            Shader.EnableKeyword("AUTOMAP_RENDER_MODE_WIREFRAME");
            Shader.DisableKeyword("AUTOMAP_RENDER_MODE_TRANSPARENT");
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to switch to automap rendering mode "cutout"
        /// </summary>
        public void switchToAutomapRenderModeCutout()
        {
            currentAutomapRenderMode = AutomapRenderMode.Cutout;
            Shader.DisableKeyword("AUTOMAP_RENDER_MODE_WIREFRAME");
            Shader.DisableKeyword("AUTOMAP_RENDER_MODE_TRANSPARENT");                        
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to switch focus to next object of interest and return the GameObject which has focus
        /// </summary>
        /// <returns> the GameObject which has the focus </returns>
        public GameObject switchFocusToNextObject()
        {
            int numberOfAutomapFocusObjects = Enum.GetNames(typeof(AutomapFocusObject)).Length;
            focusObject++;
            // if entrance is not discovered and focusObject is entrance
            if ((gameobjectBeaconEntrancePosition) && (!gameobjectBeaconEntrancePosition.activeSelf) && focusObject == AutomapFocusObject.Entrance)
            {
                focusObject++; // skip entrance and focus next object
            }
            if ((int)focusObject > numberOfAutomapFocusObjects - 1) // first mode is mode 0 -> so use numberOfAutomapFocusObjects-1 for comparison
                focusObject = 0;
            GameObject gameobjectInFocus;
            switch (focusObject)
            {
                default:
                case AutomapFocusObject.Player:
                    gameobjectInFocus = gameobjectBeaconPlayerPosition;
                    break;
                case AutomapFocusObject.Entrance:
                    gameobjectInFocus = gameobjectBeaconEntrancePosition;
                    break;
                case AutomapFocusObject.RotationAxis:
                    gameobjectInFocus = gameobjectBeaconRotationPivotAxis;
                    break;
            }
            return (gameobjectInFocus);
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to try to teleport player to dungeon segment shown at a provided screen position
        /// </summary>
        /// <param name="screenPosition"> the screen position of interest - if a dungeon segment is shown at this position player will be teleported there </param>
        public void tryTeleportPlayerToDungeonSegmentAtScreenPosition(Vector2 screenPosition)
        {
            Ray ray = cameraAutomap.ScreenPointToRay(screenPosition);

            RaycastHit[] hits = Physics.RaycastAll(ray, 10000, 1 << layerAutomap);            

            RaycastHit? nearestHit = null;
            float nearestDistance = float.MaxValue;
            foreach (RaycastHit hit in hits)
            {
                if ((hit.distance < nearestDistance) && (hit.collider.gameObject.GetComponent<MeshRenderer>().enabled))
                {
                    nearestHit = hit;
                    nearestDistance = hit.distance;
                }
            }

            if (nearestHit.HasValue)
            {
                gameObjectPlayerAdvanced.transform.position = nearestHit.Value.point + Vector3.up * 0.1f;
                gameobjectBeaconPlayerPosition.transform.position = nearestHit.Value.point + rayPlayerPosOffset;
                gameobjectPlayerMarkerArrow.transform.position = nearestHit.Value.point + rayPlayerPosOffset;
            }
        }

        #endregion

        #region Unity

        void Awake()
        {
            gameObjectPlayerAdvanced = GameObject.Find("PlayerAdvanced");
            if (!gameObjectPlayerAdvanced)
            {
                DaggerfallUnity.LogMessage("GameObject \"PlayerAdvanced\" not found! in script DaggerfallAutomap (in function Awake())", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }

            layerAutomap = LayerMask.NameToLayer("Automap");
            if (layerAutomap == -1)
            {
                DaggerfallUnity.LogMessage("Did not find Layer with name \"Automap\"! Defaulting to Layer 10\nIt is prefered that Layer \"Automap\" is set in Unity Editor under \"Edit/Project Settings/Tags and Layers!\"", true);
                layerAutomap = 10;
            }

            Camera.main.cullingMask = Camera.main.cullingMask & ~((1 << layerAutomap)); // don't render automap layer with main camera
        }

        void OnDestroy()
        {

        }

        void OnEnable()
        {
            PlayerEnterExit.OnTransitionInterior += OnTransitionToInterior;
            PlayerEnterExit.OnTransitionDungeonInterior += OnTransitionToDungeonInterior;
            PlayerEnterExit.OnTransitionExterior += OnTransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonExterior += OnTransitionToDungeonExterior;
            StartGameBehaviour.OnNewGame += onNewGame;
            SaveLoadManager.OnLoad += OnLoadEvent;            
        }

        void OnDisable()
        {
            PlayerEnterExit.OnTransitionInterior -= OnTransitionToInterior;
            PlayerEnterExit.OnTransitionDungeonInterior -= OnTransitionToDungeonInterior;
            PlayerEnterExit.OnTransitionExterior -= OnTransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonExterior -= OnTransitionToDungeonExterior;
            StartGameBehaviour.OnNewGame -= onNewGame;
            SaveLoadManager.OnLoad -= OnLoadEvent;
        }

        void Start()
        {
            // Set number of dungeons memorized
            setNumberOfDungeonMemorized(DaggerfallUnity.Settings.AutomapNumberOfDungeons);

            gameobjectAutomap = GameObject.Find("Automap/InteriorAutomap");
            if (gameobjectAutomap == null)
            {
                DaggerfallUnity.LogMessage("GameObject \"Automap/InteriorAutomap\" missing! Create a GameObject called \"Automap\" in root of hierarchy and add a GameObject \"InternalAutomap\" to it, to this add script Game/DaggerfallAutomap!\"", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }

            playerCollider = GameManager.Instance.PlayerGPS.gameObject.GetComponent<CharacterController>().GetComponent<Collider>();
            if (playerCollider == null)
            {
                DaggerfallUnity.LogMessage("Collider on PlayerGPS not found!\"", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }

            // set default automap render mode
            switchToAutomapRenderModeCutout();

            // register console commands
            try
            {
                AutoMapConsoleCommands.RegisterCommands();
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Error Registering Automap Console commands: {0}", ex.Message));

            }

            // coroutine for periodically update discovery state of automap level geometry
            StartCoroutine(CoroutineCheckForNewlyDiscoveredMeshes());
        }

        void Update()
        {
            // if we are not in game (e.g. title menu) skip update function (update must not be skipped when in game or in gui window (to propagate all map control changes))
            if ((GameManager.Instance.StateManager.CurrentState != StateManager.StateTypes.Game) && (GameManager.Instance.StateManager.CurrentState != StateManager.StateTypes.UI))
            {
                return;
            }

            if (GameManager.Instance.IsPlayerInside)
            {
                // I am not super happy with doing this in the update function, but found no other way to make starting in dungeon correctly initialize the automap geometry
                if (!gameobjectGeometry)
                {
                    // test if startup was inside dungeon or interior (and no transition event happened)                
                    InitWhenInInteriorOrDungeon();
                    // do initial geometry discovery
                    if (gameobjectGeometry) // this is necessary since when game starts up it can happen that InitWhenInInteriorOrDungeon() does not create geometry because GameManger.Instance.IsPlayerInsideDungeon and GameManager.Instance.IsPlayerInsidePalace are false
                    {
                        gameobjectGeometry.SetActive(true); // enable automap level geometry for revealing (so raycasts can hit colliders of automap level geometry)
                        CheckForNewlyDiscoveredMeshes();
                    }
                }

                if (isOpenAutomap) // only do this stuff if automap is indeed open
                {
                    updateSlicingPositionY();

                    // update position of rotation pivot axis
                    gameobjectBeaconRotationPivotAxis.transform.position = rotationPivotAxisPosition;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// does a raycast based hit test with additional protection raycast and marks automap level geometry meshes as discovered and visited in this entering/dungeon run
        /// </summary>
        private RaycastHit? scanWithRaycastInDirectionAndUpdateMeshesAndMaterials(
            Vector3 rayStartPos,    ///< the start position for the detection raycast
            Vector3 rayDirection,   ///< the ray direction of the detection raycast
            float rayDistance,      ///< the ray distance used for the detection raycast
            Vector3 offsetSecondProtectionRaycast ///< offset for second protection raycast (is used to prevent discovery of geometry through gaps in the level geometry)
            )
        {
            RaycastHit hit1, hit2, hit3;
            RaycastHit hitTrueLevelGeometry1, hitTrueLevelGeometry2, hitTrueLevelGeometry3;

            Vector3 offsetThirdProtectionRaycast = Vector3.Cross(Vector3.Normalize(rayDirection), Vector3.Normalize(offsetSecondProtectionRaycast)) * Vector3.Magnitude(offsetSecondProtectionRaycast);


#if DEBUG_RAYCASTS
            Debug.DrawRay(rayStartPos, rayDirection, Color.magenta, 1.0f);
            Debug.DrawRay(rayStartPos + offsetSecondProtectionRaycast, rayDirection, Color.yellow, 1.0f);
            Debug.DrawRay(rayStartPos + offsetThirdProtectionRaycast, rayDirection, Color.cyan, 1.0f);
#endif

#if DEBUG_SHOW_RAYCAST_TIMES
            // Start timing
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            long startTime = stopwatch.ElapsedMilliseconds;
#endif

            RaycastHit[] hitsTrueLevelGeometry1 = Physics.RaycastAll(rayStartPos, rayDirection, rayDistance, 1 << 0);
            RaycastHit[] hitsTrueLevelGeometry2 = Physics.RaycastAll(rayStartPos + offsetSecondProtectionRaycast, rayDirection, rayDistance, 1 << 0);
            RaycastHit[] hitsTrueLevelGeometry3 = Physics.RaycastAll(rayStartPos + offsetThirdProtectionRaycast, rayDirection, rayDistance, 1 << 0);
            // do raycast and protection raycast on main level geometry (use default layer as layer mask)

            hitTrueLevelGeometry1 = new RaycastHit();
            float nearestDistance = float.MaxValue;
            foreach (RaycastHit hit in hitsTrueLevelGeometry1)
            {
                if ((hit.collider.gameObject.name != "PlayerAdvanced") && (hit.distance < nearestDistance))
                {
                    hitTrueLevelGeometry1 = hit;
                    nearestDistance = hit.distance;
                }
            }

            hitTrueLevelGeometry2 = new RaycastHit();
            nearestDistance = float.MaxValue;
            foreach (RaycastHit hit in hitsTrueLevelGeometry2)
            {
                if ((hit.collider.gameObject.name != "PlayerAdvanced") && (hit.distance < nearestDistance))
                {
                    hitTrueLevelGeometry2 = hit;
                    nearestDistance = hit.distance;
                }
            }

            hitTrueLevelGeometry3 = new RaycastHit();
            nearestDistance = float.MaxValue;
            foreach (RaycastHit hit in hitsTrueLevelGeometry3)
            {
                if ((hit.collider.gameObject.name != "PlayerAdvanced") && (hit.distance < nearestDistance))
                {
                    hitTrueLevelGeometry3 = hit;
                    nearestDistance = hit.distance;
                }
            }

#if DEBUG_SHOW_RAYCAST_TIMES
            // Show timer
            long totalTime = stopwatch.ElapsedMilliseconds - startTime;
            DaggerfallUnity.LogMessage(string.Format("Time to do RaycastAll: {0}ms", totalTime), true);
#endif

            // main raycast (on Colliders in layer "Automap")
            bool didHit1 = Physics.Raycast(rayStartPos, rayDirection, out hit1, rayDistance, 1 << layerAutomap);
            // 2nd (protection) raycast (on Colliders in layer "Automap") with offset (protection against hole in daggerfall geometry prevention)
            bool didHit2 = Physics.Raycast(rayStartPos + offsetSecondProtectionRaycast, rayDirection, out hit2, rayDistance, 1 << layerAutomap);
            // 3rd (protection) raycast (on Colliders in layer "Automap") with offset (protection against hole in daggerfall geometry prevention)
            bool didHit3 = Physics.Raycast(rayStartPos + offsetThirdProtectionRaycast, rayDirection, out hit3, rayDistance, 1 << layerAutomap);

#if DEBUG_RAYCASTS
            //Debug.Log(String.Format("hitTG1: {0}, hitTG2: {1}, hitTG3: {2}, hit1: {3}, hit2: {4}, hit3: {5}", didHitTrueLevelGeometry1, didHitTrueLevelGeometry2, didHitTrueLevelGeometry3, didHit1, didHit2, didHit3));
            //if ((didHitTrueLevelGeometry1) && (didHitTrueLevelGeometry2) && (didHit1) && (didHit2))
            //{
            //    Debug.Log(String.Format("distanceTG1: {0}, distanceTG2: {1}, distanceTG3: {2}, distance1: {3}, distance2: {4}, distance3: {4}", hitTrueLevelGeometry1.distance, hitTrueLevelGeometry2.distance, hitTrueLevelGeometry2.distance, hit1.distance, hit2.distance, hit3.distance));
            //    Debug.Log(String.Format("collider of TGhit1: {0}, collider of TGhit2: {1}, collider of TGhit3: {2}, collider of hit1: {3}, collider of hit2: {4}, collider of hit3: {5}", hitTrueLevelGeometry1.collider.name, hitTrueLevelGeometry2.collider.name, hitTrueLevelGeometry3.collider.name, hit1.collider.name, hit2.collider.name, hit3.collider.name));
            //}

            //Debug.Log(String.Format("hit1: {0}, hit2: {1}, hit3: {2}", didHit1, didHit2, didHit3));
            if ((didHit1) && (didHit2) && (didHit3))
            {
                Debug.Log(String.Format("distanceTG1: {0}, distanceTG2: {1}, distanceTG3: {2}, distance1: {3}, distance2: {4}, distance3: {4}", hitTrueLevelGeometry1.distance, hitTrueLevelGeometry2.distance, hitTrueLevelGeometry3.distance, hit1.distance, hit2.distance, hit3.distance));
                Debug.Log(String.Format("collider of TGhit1: {0}, collider of TGhit2: {1}, collider of TGhit3: {2}, collider of hit1: {3}, collider of hit2: {4}, collider of hit3: {5}", hitTrueLevelGeometry1.collider.name, hitTrueLevelGeometry2.collider.name, hitTrueLevelGeometry3.collider.name, hit1.collider.name, hit2.collider.name, hit3.collider.name));
            }            
#endif

            if (
                didHit1 &&
                didHit2 &&
                didHit3 &&
                hit1.collider == hit2.collider &&  // hits must have same collider
                hit1.collider == hit3.collider &&  // hits must have same collider
                //didHitTrueLevelGeometry1 &&
                //didHitTrueLevelGeometry2 &&
                //didHitTrueLevelGeometry3 &&
                // hits on true geometry must have same distance as hits on automap geometry - otherwise there is a obstacle, e.g. a door
                Math.Abs(hitTrueLevelGeometry1.distance - hit1.distance) < 0.01f &&
                Math.Abs(hitTrueLevelGeometry2.distance - hit2.distance) < 0.01f &&
                Math.Abs(hitTrueLevelGeometry3.distance - hit3.distance) < 0.01f
                )
            {
                MeshCollider meshCollider = hit1.collider as MeshCollider;
                if (meshCollider != null)
                {
                    MeshRenderer hitMeshRenderer = meshCollider.gameObject.GetComponent<MeshRenderer>();
                    hitMeshRenderer.enabled = true; // mark mesh renderer as discovered (by enabling it)

                    Material[] mats = hitMeshRenderer.materials;
                    foreach (Material mat in mats)
                    {
                        mat.DisableKeyword("RENDER_IN_GRAYSCALE"); // mark material as visited in this entrance/dungeon run
                    }
                    hitMeshRenderer.materials = mats;
                }
                return (hitTrueLevelGeometry1); // return hit of true level geometry (which should be nearer in some cases than the automap geometry hit)
            }
            return (null);
        }

        /// <summary>
        /// basic automap level geometry revealing functionality
        /// </summary>
        void CheckForNewlyDiscoveredMeshes()
        {
            // now discovery/revealing outside...
            if (!GameManager.Instance.IsPlayerInside)
                return;

            if ((gameobjectGeometry != null) && ((GameManager.Instance.IsPlayerInsideBuilding) || (GameManager.Instance.IsPlayerInsideDungeon) || (GameManager.Instance.IsPlayerInsidePalace)))
            {
                // enable automap level geometry for revealing (so raycasts can hit colliders of automap level geometry)
                gameobjectGeometry.SetActive(true);

                // reveal geometry right below player - raycast down from player head position
                Vector3 rayStartPos = gameObjectPlayerAdvanced.transform.position + Camera.main.transform.localPosition;
                Vector3 rayDirection = Vector3.down;
                float rayDistance = raycastDistanceDown;
                Vector3 offsetSecondProtectionRaycast = Vector3.left * 0.1f; // will be used for protection raycast with slight offset of 10cm (protection against hole in daggerfall geometry prevention)            
                scanWithRaycastInDirectionAndUpdateMeshesAndMaterials(rayStartPos, rayDirection, rayDistance, offsetSecondProtectionRaycast);

                // reveal geometry which player is looking at (and which is near enough)
                rayDirection = Camera.main.transform.rotation * Vector3.forward;
                // shift 10cm to the side (computed by normalized cross product of forward vector of view direction and down vector of view direction)
                offsetSecondProtectionRaycast = Vector3.Normalize(Vector3.Cross(Camera.main.transform.rotation * Vector3.down, rayDirection)) * 0.1f;
                rayDistance = raycastDistanceViewDirection;
                RaycastHit? hitForward = scanWithRaycastInDirectionAndUpdateMeshesAndMaterials(rayStartPos, rayDirection, rayDistance, offsetSecondProtectionRaycast);

                if (hitForward.HasValue)
                {
                    //reveal geometry that is in front of player (by repeatly start further and further in front of the player and raycast in downward direction)                    
                    Vector3 stepVector = Vector3.zero;
                    while (true)
                    {
                        stepVector += Vector3.Normalize(Camera.main.transform.rotation * Vector3.forward) * 1.0f; // go 1 meters forward                        
                        if (Vector3.Magnitude(stepVector) >= hitForward.Value.distance)
                        {
                            break;
                        }
                        rayDirection = Vector3.down;
                        // shift 10cm to the side (computed by normalized cross product of forward vector of view direction and down vector of view direction)
                        offsetSecondProtectionRaycast = Vector3.Normalize(Vector3.Cross(Camera.main.transform.rotation * Vector3.down, rayDirection)) * 0.1f;
                        rayDistance = raycastDistanceDown;
                        scanWithRaycastInDirectionAndUpdateMeshesAndMaterials(rayStartPos + stepVector, rayDirection, rayDistance, offsetSecondProtectionRaycast);
                    }
                }

                // disable gameobjectGeometry so player movement won't be affected by geometry colliders of automap level geometry
                gameobjectGeometry.SetActive(false);
            }

            // entrance marker discovery check - only do as long as undiscovered
            if ((gameobjectBeaconEntrancePosition) && (!gameobjectBeaconEntrancePosition.activeSelf))
            {
                // store enabled state of player collider
                bool oldValueEnabledPlayerCollider = playerCollider.enabled;
                // enable capsule collider (for correct raycasting)
                playerCollider.enabled = true;

                // cast 3 raycasts (each with a different small offset) starting from entrance marker and see if it has direct line of sight to player head position
                RaycastHit hitTrueLevelGeometry1 = new RaycastHit();
                RaycastHit hitTrueLevelGeometry2 = new RaycastHit();
                RaycastHit hitTrueLevelGeometry3 = new RaycastHit();
                RaycastHit[] hitsTrueLevelGeometry;
                float nearestDistance;

                Vector3 entranceMarkerPos = gameObjectEntrancePositionCubeMarker.transform.position;
                Vector3 playerColliderPos = playerCollider.transform.position; //GameManager.Instance.PlayerGPS.transform.position; //Camera.main.transform.position;
                // raycast 1
                Vector3 rayStartPos = entranceMarkerPos;
                Vector3 rayToPlayer = playerColliderPos - rayStartPos;                
                hitsTrueLevelGeometry = Physics.RaycastAll(rayStartPos, rayToPlayer, raycastDistanceEntranceMarkerReveal, 1 << 0);                
                nearestDistance = float.MaxValue;
                foreach (RaycastHit hit in hitsTrueLevelGeometry)
                {
                    if (hit.distance < nearestDistance)
                    {
                        hitTrueLevelGeometry1 = hit;
                        nearestDistance = hit.distance;
                    }
                }

                // raycast 2
                rayStartPos = entranceMarkerPos + Vector3.left * 0.1f;
                rayToPlayer = playerColliderPos - rayStartPos;                
                hitsTrueLevelGeometry = Physics.RaycastAll(rayStartPos, rayToPlayer, raycastDistanceEntranceMarkerReveal, 1 << 0);                
                nearestDistance = float.MaxValue;
                foreach (RaycastHit hit in hitsTrueLevelGeometry)
                {
                    if (hit.distance < nearestDistance)
                    {
                        hitTrueLevelGeometry2 = hit;
                        nearestDistance = hit.distance;
                    }
                }

                // raycast 3
                rayStartPos = entranceMarkerPos + Vector3.forward * 0.1f + Vector3.up * 0.1f;
                rayToPlayer = playerColliderPos - rayStartPos;                
                hitsTrueLevelGeometry = Physics.RaycastAll(rayStartPos, rayToPlayer, raycastDistanceEntranceMarkerReveal, 1 << 0);
                nearestDistance = float.MaxValue;
                foreach (RaycastHit hit in hitsTrueLevelGeometry)
                {
                    if (hit.distance < nearestDistance)
                    {
                        hitTrueLevelGeometry3 = hit;
                        nearestDistance = hit.distance;
                    }
                }

                // if all 3 raycasts hit the player collider then the entrance was discovered
                if ((hitTrueLevelGeometry1.collider) && (hitTrueLevelGeometry2.collider) && (hitTrueLevelGeometry3.collider))
                {
                    if (
                        (hitTrueLevelGeometry1.collider == playerCollider) &&
                        (hitTrueLevelGeometry2.collider == playerCollider) &&
                        (hitTrueLevelGeometry3.collider == playerCollider)
                       )
                    {
                        // so set the entrance beacon to active (discovered)
                        gameobjectBeaconEntrancePosition.SetActive(true);
                    }
                }

                // restore enabled state of player collider
                playerCollider.enabled = oldValueEnabledPlayerCollider;
            }
        }

        /// <summary>
        /// coroutine for basic automap level geometry revealing functionality - this function is periodically invoked
        /// </summary>
        IEnumerator CoroutineCheckForNewlyDiscoveredMeshes()
        {
            while (true)
            {
                // only proceed if automap is not opened (otherwise command gameobjectGeometry.SetActive(false); will mess with automap rendering when scheduling is a bitch and overwrites changes from updateAutomapStateOnWindowPush()
                if (!isOpenAutomap)
                {
                    CheckForNewlyDiscoveredMeshes();
                }
                yield return new WaitForSeconds(1.0f / scanRateGeometryDiscoveryInHertz);
            }
        }

        /// <summary>
        /// update y-position of place of slicing automap level geometry
        /// </summary>
        private void updateSlicingPositionY()
        {
            float slicingPositionY = gameObjectPlayerAdvanced.transform.position.y + Camera.main.transform.localPosition.y + slicingBiasY;
            Shader.SetGlobalFloat("_SclicingPositionY", slicingPositionY);
        }

        private void updateMaterialsOfMeshRenderer(MeshRenderer meshRenderer, bool visitedInThisEntering = false)
        {
            Vector3 playerAdvancedPos = gameObjectPlayerAdvanced.transform.position;
            Material[] newMaterials = new Material[meshRenderer.materials.Length];
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                Material material = meshRenderer.materials[i];
                Material newMaterial = newMaterials[i];

                newMaterial = new Material(Shader.Find("Daggerfall/Automap"));
                //newMaterial.CopyPropertiesFromMaterial(material);
                newMaterial.name = "AutomapBelowSclicePlane injected for: " + material.name;
                Texture mainTex = material.GetTexture("_MainTex");
                newMaterial.SetTexture("_MainTex", mainTex);
                Texture bumpMapTex = material.GetTexture("_BumpMap");
                newMaterial.SetTexture("_BumpMap", bumpMapTex);
                Texture emissionMapTex = material.GetTexture("_EmissionMap");
                newMaterial.SetTexture("_EmissionMap", emissionMapTex);
                Color emissionColor = material.GetColor("_EmissionColor");
                newMaterial.SetColor("_EmissionColor", emissionColor);
                Vector4 playerPosition = new Vector4(playerAdvancedPos.x, playerAdvancedPos.y + Camera.main.transform.localPosition.y, playerAdvancedPos.z, 0.0f);
                newMaterial.SetVector("_PlayerPosition", playerPosition);
                if (visitedInThisEntering == true)
                    newMaterial.DisableKeyword("RENDER_IN_GRAYSCALE");
                else
                    newMaterial.EnableKeyword("RENDER_IN_GRAYSCALE");
                newMaterials[i] = newMaterial;
            }
            meshRenderer.materials = newMaterials;
        }     

        /// <summary>
        /// will inject materials and properties to MeshRenderer in the proper hierarchy level of automap level geometry GameObject
        /// note: the proper hierarchy level differs between an "Interior" and a "Dungeon" geometry GameObject
        /// </summary>
        /// <param name="resetDiscoveryState"> if true resets the discovery state for geometry that needs to be discovered (when inside dungeons or palaces) </param>
        private void injectMeshAndMaterialProperties(bool resetDiscoveryState = true)
        {
            if (GameManager.Instance.IsPlayerInsideBuilding)
            {
                // find all MeshRenderers in 3rd hierarchy level
                foreach (Transform elem in gameobjectGeometry.transform)
                {
                    foreach (Transform innerElem in elem.gameObject.transform)
                    {
                        foreach (Transform inner2Elem in innerElem.gameObject.transform)
                        {
                            // get rid of animated materials (will not break automap rendering but is not necessary)
                            AnimatedMaterial[] animatedMaterials = inner2Elem.gameObject.GetComponents<AnimatedMaterial>();                            
                            foreach (AnimatedMaterial animatedMaterial in animatedMaterials)
                            {
                                UnityEngine.Object.Destroy(animatedMaterial);
                            }

                            MeshRenderer meshRenderer = inner2Elem.gameObject.GetComponent<MeshRenderer>();
                            if (meshRenderer == null)
                                break;

                            // update materials and set meshes as visited in this run (so "Interior" geometry always is colored
                            // (since we don't disable the mesh, it is also discovered - which is a precondition for being rendered))
                            updateMaterialsOfMeshRenderer(meshRenderer, true);                            
                        }
                    }
                }
            }
            else if ((GameManager.Instance.IsPlayerInsideDungeon)||(GameManager.Instance.IsPlayerInsidePalace))
            {
                // find all MeshRenderers in 4th hierarchy level
                foreach (Transform elem in gameobjectGeometry.transform)
                {
                    foreach (Transform innerElem in elem.gameObject.transform)
                    {
                        foreach (Transform inner2Elem in innerElem.gameObject.transform)
                        {
                            foreach (Transform inner3Elem in inner2Elem.gameObject.transform)
                            {
                                // get rid of animated materials (will not break automap rendering but is not necessary)
                                AnimatedMaterial[] animatedMaterials = inner3Elem.gameObject.GetComponents<AnimatedMaterial>();
                                foreach (AnimatedMaterial animatedMaterial in animatedMaterials)
                                {
                                    UnityEngine.Object.Destroy(animatedMaterial);
                                }

                                MeshRenderer meshRenderer = inner3Elem.gameObject.GetComponent<MeshRenderer>();
                                if (meshRenderer == null)
                                    break;

                                // update materials (omit 2nd parameter so default behavior is initiated which is:
                                // meshes are marked as not visited in this run (so "Dungeon" geometry that has been discovered in a previous dungeon run is rendered in grayscale)
                                updateMaterialsOfMeshRenderer(meshRenderer);

                                if (resetDiscoveryState) // if forced reset of discovery state
                                {
                                    // mark meshRenderer as undiscovered
                                    meshRenderer.enabled = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// setup beacons: lazy creation of player marker arrow and beacons including
        /// player position beacon, dungeon entrance position beacon and rotation pivot axis position beacon
        /// will always get the current player position and update the player marker arrow position and the player position beacon
        /// will always get the rotation pivot axis position, will always set the dungeon entrance position (for interior: just takes the
        /// player position since this function is only called when geometry is created (when entering the dungeon or interior) -
        /// so the player position is at the entrance), for dungeon: will get the start marker from DaggerfallDungeon component
        /// </summary>
        private void setupBeacons()
        {
            if (!gameobjectBeacons)
            {
                gameobjectBeacons = new GameObject("Beacons");
                gameobjectBeacons.layer = layerAutomap;
                gameobjectBeacons.transform.SetParent(gameobjectAutomap.transform);
            }
            if (!gameobjectPlayerMarkerArrow)
            {
                gameobjectPlayerMarkerArrow = GameObjectHelper.CreateDaggerfallMeshGameObject(99900, gameobjectBeacons.transform, false, null, true);
                gameobjectPlayerMarkerArrow.name = "PlayerMarkerArrow";
                gameobjectPlayerMarkerArrow.layer = layerAutomap;
            }
            gameobjectPlayerMarkerArrow.transform.position = gameObjectPlayerAdvanced.transform.position;
            gameobjectPlayerMarkerArrow.transform.rotation = gameObjectPlayerAdvanced.transform.rotation;

            if (!gameobjectBeaconPlayerPosition)
            {
                gameobjectBeaconPlayerPosition = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                UnityEngine.Object.Destroy(gameobjectBeaconPlayerPosition.GetComponent<Collider>());
                gameobjectBeaconPlayerPosition.name = "BeaconPlayerPosition";
                gameobjectBeaconPlayerPosition.transform.SetParent(gameobjectBeacons.transform);
                gameobjectBeaconPlayerPosition.layer = layerAutomap;
                gameobjectBeaconPlayerPosition.transform.localScale = new Vector3(0.3f, 50.0f, 0.3f);
                Material material = new Material(Shader.Find("Standard"));
                material.color = new Color(1.0f, 0.0f, 0.0f);
                gameobjectBeaconPlayerPosition.GetComponent<MeshRenderer>().material = material;
            }
            gameobjectBeaconPlayerPosition.transform.position = gameObjectPlayerAdvanced.transform.position + rayPlayerPosOffset;

            if (!gameobjectBeaconRotationPivotAxis)
            {
                gameobjectBeaconRotationPivotAxis = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                UnityEngine.Object.Destroy(gameobjectBeaconRotationPivotAxis.GetComponent<Collider>());
                gameobjectBeaconRotationPivotAxis.name = "BeaconRotationPivotAxis";
                gameobjectBeaconRotationPivotAxis.transform.SetParent(gameobjectBeacons.transform);
                gameobjectBeaconRotationPivotAxis.layer = layerAutomap;
                gameobjectBeaconRotationPivotAxis.transform.localScale = new Vector3(0.3f, 50.0f, 0.3f);
                Material material = new Material(Shader.Find("Standard"));
                material.color = new Color(0.0f, 0.0f, 1.0f);
                gameobjectBeaconRotationPivotAxis.GetComponent<MeshRenderer>().material = material;
            }
            gameobjectBeaconRotationPivotAxis.transform.position = rotationPivotAxisPosition;

            if (!gameobjectBeaconEntrancePosition)
            {
                gameobjectBeaconEntrancePosition = new GameObject("BeaconEntracePosition");
                gameobjectBeaconEntrancePosition.transform.SetParent(gameobjectBeacons.transform);
                gameobjectBeaconEntrancePosition.layer = layerAutomap;

                GameObject gameobjectRay = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                UnityEngine.Object.Destroy(gameobjectRay.GetComponent<Collider>());
                gameobjectRay.name = "BeaconEntracePositionMarker";
                gameobjectRay.transform.SetParent(gameobjectBeaconEntrancePosition.transform);
                gameobjectRay.layer = layerAutomap;
                gameobjectRay.transform.localScale = new Vector3(0.3f, 50.0f, 0.3f);
                Material material = new Material(Shader.Find("Standard"));
                material.color = new Color(0.0f, 1.0f, 0.0f);
                gameobjectRay.GetComponent<MeshRenderer>().material = material;

                gameObjectEntrancePositionCubeMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                UnityEngine.Object.Destroy(gameObjectEntrancePositionCubeMarker.GetComponent<Collider>());
                gameObjectEntrancePositionCubeMarker.name = "CubeEntracePositionMarker";
                gameObjectEntrancePositionCubeMarker.transform.SetParent(gameobjectBeaconEntrancePosition.transform);
                gameObjectEntrancePositionCubeMarker.GetComponent<MeshRenderer>().material = material;
                gameObjectEntrancePositionCubeMarker.layer = layerAutomap;
                gameObjectEntrancePositionCubeMarker.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }

            if ((GameManager.Instance.IsPlayerInsideDungeon) || (GameManager.Instance.IsPlayerInsidePalace))
            {
                // entrance marker to dungeon start marker
                DaggerfallDungeon dungeon = GameManager.Instance.DungeonParent.GetComponentInChildren<DaggerfallDungeon>();                
                gameobjectBeaconEntrancePosition.transform.position = dungeon.StartMarker.transform.position + rayEntrancePosOffset;
                gameobjectBeaconEntrancePosition.SetActive(false); // set do undiscovered
            }
            else
            {
                // entrance marker to current position (position player entered)
                gameobjectBeaconEntrancePosition.transform.position = gameObjectPlayerAdvanced.transform.position + rayEntrancePosOffset;
                gameobjectBeaconEntrancePosition.SetActive(true); // set do discovered
            }
        }

        void DestroyBeacons()
        {
            if (gameobjectBeacons != null)
            {
                // after Destroy() set GameObject to null - this is necessary so that the handle is invalid immediately
                UnityEngine.Object.Destroy(gameobjectBeacons);
                gameobjectBeacons = null;
            }
            // also do this for all sub-GameObjects inside gameobjectBeacons
            if (gameobjectPlayerMarkerArrow != null)
            {
                //UnityEngine.Object.Destroy(gameobjectPlayerMarkerArrow);
                gameobjectPlayerMarkerArrow = null;
            }
            if (gameobjectBeaconPlayerPosition != null)
            {
                //UnityEngine.Object.Destroy(gameobjectBeaconPlayerPosition);
                gameobjectBeaconPlayerPosition = null;
            }
            if (gameobjectBeaconRotationPivotAxis != null)
            {
                //UnityEngine.Object.Destroy(gameobjectBeaconRotationPivotAxis);
                gameobjectBeaconRotationPivotAxis = null;
            }
            if (gameobjectBeaconEntrancePosition != null)
            {
                //UnityEngine.Object.Destroy(gameobjectBeaconEntrancePosition);
                gameobjectBeaconEntrancePosition = null;
            }
            if (gameObjectEntrancePositionCubeMarker != null)
            {
                //UnityEngine.Object.Destroy(gameObjectEntrancePositionCubeMarker);
                gameObjectEntrancePositionCubeMarker = null;
            }
        }

        /// <summary>
        /// sets layer of a GameObject and all of its childs recursively
        /// </summary>
        /// <param name="obj"> the target GameObject </param>
        /// <param name="layer"> the layer to be set </param>
        private static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        /// <summary>
        /// creates the indoor geometry used for automap rendering
        /// </summary>
        /// <param name="args"> the static door for loading the correct interior </param>
        private void createIndoorGeometryForAutomap(StaticDoor door)
        {
            String newGeometryName = string.Format("DaggerfallInterior [Block={0}, Record={1}]", door.blockIndex, door.recordIndex);

            if (gameobjectGeometry != null)
            {
                UnityEngine.Object.Destroy(gameobjectGeometry);
                gameobjectGeometry = null;
            }

            gameobjectGeometry = new GameObject("GeometryAutomap (Interior)");

            foreach (Transform elem in GameManager.Instance.InteriorParent.transform)
            {
                if (elem.name.Contains("DaggerfallInterior"))
                {
                    // Get climate
                    ClimateBases climateBase = ClimateBases.Temperate;
                    climateBase = ClimateSwaps.FromAPIClimateBase(GameManager.Instance.PlayerGPS.ClimateSettings.ClimateType);

                    // Layout interior
                    GameObject gameobjectInterior = new GameObject(newGeometryName);
                    DaggerfallInterior interior = gameobjectInterior.AddComponent<DaggerfallInterior>();

                    // automap layout is a simplified layout in contrast to normal layout (less objects)
                    interior.DoLayoutAutomap(null, door, climateBase);

                    gameobjectInterior.transform.SetParent(gameobjectGeometry.transform);

                    // copy position and rotation from real level geometry
                    gameobjectGeometry.transform.position = elem.transform.position;
                    gameobjectGeometry.transform.rotation = elem.transform.rotation;

                    // do this (here in createIndoorGeometryForAutomap()) analog in the same way and the same place like in createDungeonGeometryForAutomap()
                    setupBeacons();
                }
            }

            // put all objects inside gameobjectGeometry in layer "Automap"
            SetLayerRecursively(gameobjectGeometry, layerAutomap);
            gameobjectGeometry.transform.SetParent(gameobjectAutomap.transform);

            // inject all materials of automap geometry with automap shader and reset MeshRenderer enabled state (this is used for the discovery mechanism)
            injectMeshAndMaterialProperties();

            //oldGeometryName = newGeometryName;
        }

        /// <summary>
        /// creates the dungeon geometry used for automap rendering
        /// </summary>
        private void createDungeonGeometryForAutomap()
        {
            DFLocation location = GameManager.Instance.PlayerGPS.CurrentLocation;
            String newGeometryName = string.Format("DaggerfallDungeon [Region={0}, Name={1}]", location.RegionName, location.Name);


            if (gameobjectGeometry != null)
            {
                UnityEngine.Object.Destroy(gameobjectGeometry);
                gameobjectGeometry = null;
            }

            gameobjectGeometry = new GameObject("GeometryAutomap (Dungeon)");

            // disable this option to get all small dungeon parts as individual models
            DaggerfallUnity.Instance.Option_CombineRDB = false;

            foreach (Transform elem in GameManager.Instance.DungeonParent.transform)
            {
                if (elem.name.Contains("DaggerfallDungeon"))
                {
                    GameObject gameobjectDungeon = new GameObject(newGeometryName);

                    // Create dungeon layout
                    foreach (DFLocation.DungeonBlock block in location.Dungeon.Blocks)
                    {
                        if (location.Name == "Orsinium")
                        {
                            if (block.X == -1 && block.Z == -1 && block.BlockName == "N0000065.RDB")
                                continue;
                        }

                        DFBlock blockData;
                        int[] textureTable = null;
                        GameObject gameobjectBlock = RDBLayout.CreateBaseGameObject(block.BlockName, null, out blockData, textureTable, true, null, false);
                        gameobjectBlock.transform.position = new Vector3(block.X * RDBLayout.RDBSide, 0, block.Z * RDBLayout.RDBSide);

                        gameobjectBlock.transform.SetParent(gameobjectDungeon.transform);
                    }

                    gameobjectDungeon.transform.SetParent(gameobjectGeometry.transform);

                    // copy position and rotation from real level geometry
                    gameobjectGeometry.transform.position = elem.transform.position;
                    gameobjectGeometry.transform.rotation = elem.transform.rotation;

                    // do this here when DaggerfallDungeon GameObject is present, so that a call to setupBeacons() will not fail (it needs the DaggerfallDungeon component of this GameObject)                    
                    setupBeacons();

                    break;
                }
            }

            // enable this option (to reset to normal behavior)
            DaggerfallUnity.Instance.Option_CombineRDB = true;

            // put all objects inside gameobjectGeometry in layer "Automap"
            SetLayerRecursively(gameobjectGeometry, layerAutomap);
            gameobjectGeometry.transform.SetParent(gameobjectAutomap.transform);            

            // inject all materials of automap geometry with automap shader and reset MeshRenderer enabled state (this is used for the discovery mechanism)
            injectMeshAndMaterialProperties();

            //oldGeometryName = newGeometryName;
        }

        /// <summary>
        /// creates the automap camera if not present and sets camera default settings, registers camera to compass
        /// </summary>
        private void createAutomapCamera()
        {
            if (!cameraAutomap)
            {
                gameObjectCameraAutomap = new GameObject("CameraAutomap");
                cameraAutomap = gameObjectCameraAutomap.AddComponent<Camera>();
                cameraAutomap.clearFlags = CameraClearFlags.SolidColor;
                cameraAutomap.cullingMask = 1 << layerAutomap;
                cameraAutomap.renderingPath = Camera.main.renderingPath;
                cameraAutomap.nearClipPlane = 0.7f;
                cameraAutomap.farClipPlane = 5000.0f;
                gameObjectCameraAutomap.transform.SetParent(gameobjectAutomap.transform);
            }
        }

        /// <summary>
        /// creates (if not present) automap lights that light the automap level geometry
        /// </summary>
        private void createLightsForAutomapGeometry()
        {
            if (!gameobjectAutomapKeyLight)
            {
                gameobjectAutomapKeyLight = new GameObject("AutomapKeyLight");
                gameobjectAutomapKeyLight.transform.rotation = Quaternion.Euler(50.0f, 270.0f, 0.0f);
                Light keyLight = gameobjectAutomapKeyLight.AddComponent<Light>();
                keyLight.type = LightType.Directional;
                //keyLight.cullingMask = 1 << layerAutomap; // issues warning "Too many layers used to exclude objects from lighting. Up to 4 layers can be used to exclude lights"
                gameobjectAutomapKeyLight.transform.SetParent(gameobjectAutomap.transform);
            }

            if (!gameobjectAutomapFillLight)
            {
                gameobjectAutomapFillLight = new GameObject("AutomapFillLight");
                gameobjectAutomapFillLight.transform.rotation = Quaternion.Euler(50.0f, 126.0f, 0.0f);
                Light fillLight = gameobjectAutomapFillLight.AddComponent<Light>();
                fillLight.type = LightType.Directional;
                gameobjectAutomapFillLight.transform.SetParent(gameobjectAutomap.transform);
            }

            if (!gameobjectAutomapBackLight)
            {
                gameobjectAutomapBackLight = new GameObject("AutomapBackLight");
                gameobjectAutomapBackLight.transform.rotation = Quaternion.Euler(50.0f, 0.0f, 0.0f);
                Light backLight = gameobjectAutomapBackLight.AddComponent<Light>();
                backLight.type = LightType.Directional;
                gameobjectAutomapBackLight.transform.SetParent(gameobjectAutomap.transform);
            }

            if (GameManager.Instance.IsPlayerInsideBuilding)
            {
                Light keyLight = gameobjectAutomapKeyLight.GetComponent<Light>();
                Light fillLight = gameobjectAutomapFillLight.GetComponent<Light>();
                Light backLight = gameobjectAutomapBackLight.GetComponent<Light>();

                keyLight.intensity = 1.0f;
                fillLight.intensity = 0.6f;
                backLight.intensity = 0.2f;
            }
            else if ((GameManager.Instance.IsPlayerInsideDungeon) || (GameManager.Instance.IsPlayerInsidePalace))
            {
                Light keyLight = gameobjectAutomapKeyLight.GetComponent<Light>();
                Light fillLight = gameobjectAutomapFillLight.GetComponent<Light>();
                Light backLight = gameobjectAutomapBackLight.GetComponent<Light>();

                keyLight.intensity = 0.5f;
                fillLight.intensity = 0.5f;
                backLight.intensity = 0.5f;
            }
        }

        /// <summary>
        /// saves discovery state to object automapGeometryInteriorState
        /// this class is mapping the hierarchy inside the GameObject gameObjectGeometry in such way
        /// that the MeshRenderer enabled state for objects in the 3rd hierarchy level (which are the actual models)
        /// is matching value of field "discovered" in AutomapGeometryBlockState.AutomapGeometryBlockElementState.AutomapGeometryModelState
        /// </summary>
        private void saveStateAutomapInterior()
        {            
            Transform interiorBlock = gameobjectGeometry.transform.GetChild(0); // building interior should only have one block - so get it
            automapGeometryInteriorState = new AutomapGeometryBlockState();
            automapGeometryInteriorState.blockName = interiorBlock.name;

            List<AutomapGeometryBlockElementState> blockElements = new List<AutomapGeometryBlockElementState>();

            foreach (Transform currentTransformModel in interiorBlock.transform)
            {
                AutomapGeometryBlockElementState blockElement = new AutomapGeometryBlockElementState();

                List<AutomapGeometryModelState> models = new List<AutomapGeometryModelState>();
                foreach (Transform currentTransformMesh in currentTransformModel.transform)
                {
                    AutomapGeometryModelState model = new AutomapGeometryModelState();
                    MeshRenderer meshRenderer = currentTransformMesh.GetComponent<MeshRenderer>();
                    if ((meshRenderer) && (meshRenderer.enabled))
                    {
                        model.discovered = true;
                        model.visitedInThisRun = !meshRenderer.materials[0].IsKeywordEnabled("RENDER_IN_GRAYSCALE"); // all materials of model have the same setting - so just material[0] is tested
                    }
                    else
                    {
                        model.discovered = false;
                        model.visitedInThisRun = false;
                    }
                    models.Add(model);
                }

                blockElement.models = models;
                blockElements.Add(blockElement);
            }
            automapGeometryInteriorState.blockElements = blockElements;
        }

        /// <summary>
        /// saves discovery state to object automapGeometryDungeonState
        /// this class is mapping the hierarchy inside the GameObject gameObjectGeometry in such way
        /// that the MeshRenderer enabled state for objects in the 4th hierarchy level (which are the actual models)
        /// is matching value of field "discovered" in AutomapGeometryDungeonState.AutomapGeometryBlockState.AutomapGeometryBlockElementState.AutomapGeometryModelState
        /// dictionary of dungeon states of visited dungeons is then updated (which is used for save game mechanism)
        /// </summary>
        private void saveStateAutomapDungeon(bool forceSaveOfDungeonDiscoveryState)
        {
            if ((numberOfDungeonMemorized == 0)&&(!GameManager.Instance.IsPlayerInside)) // if discovery state of no dungeon has to be remembered, clear dictionary and skip the rest of this function
            {
                dictAutomapDungeonsDiscoveryState.Clear();
                return;
            }

            if ((!GameManager.Instance.IsPlayerInside)&&(!forceSaveOfDungeonDiscoveryState)) // if player is outside just skip this function
            {
                return;
            }

            int updatedNumberOfDungeonMemorized = numberOfDungeonMemorized;
            if ((numberOfDungeonMemorized == 0) && (GameManager.Instance.IsPlayerInside))
            {
                updatedNumberOfDungeonMemorized = 1;
            }

            Transform gameObjectGeometryDungeon = gameobjectGeometry.transform.GetChild(0);
            automapGeometryDungeonState = new AutomapGeometryDungeonState();
            automapGeometryDungeonState.locationName = gameObjectGeometryDungeon.name;
            automapGeometryDungeonState.entranceDiscovered = ((gameobjectBeaconEntrancePosition) && (gameobjectBeaconEntrancePosition.activeSelf)); // get entrance marker discovery state
            automapGeometryDungeonState.timeInSecondsLastVisited = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds();
            automapGeometryDungeonState.blocks = new List<AutomapGeometryBlockState>();

            foreach (Transform currentBlock in gameObjectGeometryDungeon.transform)
            {
                AutomapGeometryBlockState automapGeometryBlockState = new AutomapGeometryBlockState();
                automapGeometryBlockState.blockName = currentBlock.name;

                List<AutomapGeometryBlockElementState> blockElements = new List<AutomapGeometryBlockElementState>();

                foreach (Transform currentTransformModel in currentBlock.transform)
                {
                    AutomapGeometryBlockElementState blockElement = new AutomapGeometryBlockElementState();

                    List<AutomapGeometryModelState> models = new List<AutomapGeometryModelState>();
                    foreach (Transform currentTransformMesh in currentTransformModel.transform)
                    {
                        AutomapGeometryModelState model = new AutomapGeometryModelState();
                        MeshRenderer meshRenderer = currentTransformMesh.GetComponent<MeshRenderer>();
                        if ((meshRenderer) && (meshRenderer.enabled))
                        {
                            model.discovered = true;
                            model.visitedInThisRun = !meshRenderer.materials[0].IsKeywordEnabled("RENDER_IN_GRAYSCALE"); // all materials of model have the same setting - so just material[0] is tested
                        }
                        else
                        {
                            model.discovered = false;
                            model.visitedInThisRun = false;
                        }
                        models.Add(model);
                    }

                    blockElement.models = models;
                    blockElements.Add(blockElement);
                }
                automapGeometryBlockState.blockElements = blockElements;
                automapGeometryDungeonState.blocks.Add(automapGeometryBlockState);
            }

            // replace or add discovery state for current dungeon
            DFLocation dfLocation = GameManager.Instance.PlayerGPS.CurrentLocation;
            string locationStringIdentifier = string.Format("{0}/{1}", dfLocation.RegionName, dfLocation.Name);
            if (dictAutomapDungeonsDiscoveryState.ContainsKey(locationStringIdentifier))
            {
                dictAutomapDungeonsDiscoveryState[locationStringIdentifier] = automapGeometryDungeonState;
            }
            else
            {
                dictAutomapDungeonsDiscoveryState.Add(locationStringIdentifier, automapGeometryDungeonState);
            }

            // make list out of dictionary and sort by last time visited - then get rid of dungeons in dictionary that weren't visited lately
            List<KeyValuePair<string, AutomapGeometryDungeonState>> sortedList = new List<KeyValuePair<string, AutomapGeometryDungeonState>>(dictAutomapDungeonsDiscoveryState);
            sortedList.Sort(
                delegate (KeyValuePair<string, AutomapGeometryDungeonState> firstPair,
                KeyValuePair<string, AutomapGeometryDungeonState> nextPair)
                {
                    return nextPair.Value.timeInSecondsLastVisited.CompareTo(firstPair.Value.timeInSecondsLastVisited);
                }
            );

            //if there are more dungeons remembered than allowed - get rid of dungeons in dictionary that weren't visited lately
            if (sortedList.Count > updatedNumberOfDungeonMemorized)
            {
                ulong timeInSecondsLimit = sortedList[updatedNumberOfDungeonMemorized - 1].Value.timeInSecondsLastVisited;

                foreach (KeyValuePair<string, AutomapGeometryDungeonState> entry in sortedList) // use sorted list and iterate over it - remove elements of dictionary
                {
                    if (entry.Value.timeInSecondsLastVisited < timeInSecondsLimit)
                    {
                        dictAutomapDungeonsDiscoveryState.Remove(entry.Key); // remove this dungeon's entry from dictionary
                    }
                }
            }

        }

        void updateMeshRendererInteriorState(ref MeshRenderer meshRenderer, AutomapGeometryBlockState automapGeometryInteriorState, int indexElement, int indexModel, bool forceNotVisitedInThisRun)
        {
            if (automapGeometryInteriorState.blockElements[indexElement].models[indexModel].discovered == true)
            {
                meshRenderer.enabled = true;

                if ((!forceNotVisitedInThisRun)&&(automapGeometryInteriorState.blockElements[indexElement].models[indexModel].visitedInThisRun))
                {
                    Material[] materials = meshRenderer.materials;
                    foreach (Material mat in meshRenderer.materials)
                    {
                        mat.DisableKeyword("RENDER_IN_GRAYSCALE");
                    }
                    meshRenderer.materials = materials;
                }
                else
                {
                    Material[] materials = meshRenderer.materials;
                    foreach (Material mat in meshRenderer.materials)
                    {
                        mat.EnableKeyword("RENDER_IN_GRAYSCALE");
                    }
                    meshRenderer.materials = materials;
                }
            }
            else
            {
                meshRenderer.enabled = false;
            }
        }

        /// <summary>
        /// restores discovery state from automapGeometryInteriorState onto gameobjectGeometry
        /// this class is mapping the value of field "discovered" (AutomapGeometryBlockState.AutomapGeometryBlockElementState.AutomapGeometryModelState)
        /// inside object automapGeometryInteriorState to the objects inside the 3rd hierarchy level of GameObject gameObjectGeometry (which are the actual models)
        /// in such way that the MeshRenderer enabled state for these objects match the value of field "discovered"
        /// </summary>
        /// <param name="forceNotVisitedInThisRun"> if set to true geometry is restored and its state is forced to not being visited in this run </param>
        private void restoreStateAutomapInterior(bool forceNotVisitedInThisRun = false)
        {
            Transform interiorBlock = gameobjectGeometry.transform.GetChild(0);

            if (automapGeometryInteriorState == null)
                return;

            if (interiorBlock.name != automapGeometryInteriorState.blockName)
                return;

            for (int indexElement = 0; indexElement < interiorBlock.childCount; indexElement++)
            {
                Transform currentTransformElement = interiorBlock.GetChild(indexElement);

                for (int indexModel = 0; indexModel < currentTransformElement.childCount; indexModel++)
                {
                    Transform currentTransformModel = currentTransformElement.GetChild(indexModel);

                    MeshRenderer meshRenderer = currentTransformModel.GetComponent<MeshRenderer>();
                    if (meshRenderer)
                    {
                        updateMeshRendererInteriorState(ref meshRenderer, automapGeometryInteriorState, indexElement, indexModel, forceNotVisitedInThisRun);
                    }
                }
            }
        }

        void updateMeshRendererDungeonState(ref MeshRenderer meshRenderer, AutomapGeometryDungeonState automapGeometryDungeonState, int indexBlock, int indexElement, int indexModel, bool forceNotVisitedInThisRun)
        {
            if (automapGeometryDungeonState.blocks[indexBlock].blockElements[indexElement].models[indexModel].discovered == true)
            {
                meshRenderer.enabled = true;

                if ((!forceNotVisitedInThisRun)&&(automapGeometryDungeonState.blocks[indexBlock].blockElements[indexElement].models[indexModel].visitedInThisRun))
                {
                    Material[] materials = meshRenderer.materials;
                    foreach (Material mat in meshRenderer.materials)
                    {
                        mat.DisableKeyword("RENDER_IN_GRAYSCALE");
                    }
                    meshRenderer.materials = materials;
                }
                else
                {
                    Material[] materials = meshRenderer.materials;
                    foreach (Material mat in meshRenderer.materials)
                    {
                        mat.EnableKeyword("RENDER_IN_GRAYSCALE");
                    }
                    meshRenderer.materials = materials;
                }
            }
            else
            {
                meshRenderer.enabled = false;
            }
        }

        /// <summary>
        /// restores discovery state from automapGeometryDungeonState onto gameobjectGeometry
        /// automapGeometryDungeonState is loaded from dictionary of dungeon states of visited dungeons
        /// this class is mapping the value of field "discovered" (AutomapGeometryDungeonState.AutomapGeometryBlockState.AutomapGeometryBlockElementState.AutomapGeometryModelState)
        /// inside object automapGeometryDungeonState to the objects inside the 4th hierarchy level of GameObject gameObjectGeometry (which are the actual models)
        /// in such way that the MeshRenderer enabled state for these objects match the value of field "discovered"
        /// </summary>
        /// <param name="forceNotVisitedInThisRun"> if set to true geometry is restored and its state is forced to not being visited in this run </param>
        private void restoreStateAutomapDungeon(bool forceNotVisitedInThisRun = false)
        {
            Transform location = gameobjectGeometry.transform.GetChild(0);

            HideAll(); // clear discovery state of geometry as initial starting point

            DFLocation dfLocation = GameManager.Instance.PlayerGPS.CurrentLocation;
            string locationStringIdentifier = string.Format("{0}/{1}", dfLocation.RegionName, dfLocation.Name);
            if (dictAutomapDungeonsDiscoveryState.ContainsKey(locationStringIdentifier))
            {
                automapGeometryDungeonState = dictAutomapDungeonsDiscoveryState[locationStringIdentifier];
            }
            else
            {
                automapGeometryDungeonState = null;                
            }

            if (automapGeometryDungeonState == null)
                return;

            gameobjectBeaconEntrancePosition.SetActive(automapGeometryDungeonState.entranceDiscovered); // set entrance marker discovery state         

            if (location.name != automapGeometryDungeonState.locationName)
                return;

            for (int indexBlock = 0; indexBlock < location.childCount; indexBlock++)
            {
                Transform currentBlock = location.GetChild(indexBlock);

                if (currentBlock.name != automapGeometryDungeonState.blocks[indexBlock].blockName)
                    return;

                for (int indexElement = 0; indexElement < currentBlock.childCount; indexElement++)
                {
                    Transform currentTransformElement = currentBlock.GetChild(indexElement);

                    for (int indexModel = 0; indexModel < currentTransformElement.childCount; indexModel++)
                    {
                        Transform currentTransformModel = currentTransformElement.GetChild(indexModel);

                        MeshRenderer meshRenderer = currentTransformModel.GetComponent<MeshRenderer>();
                        if (meshRenderer)
                        {
                            updateMeshRendererDungeonState(ref meshRenderer, automapGeometryDungeonState, indexBlock, indexElement, indexModel, forceNotVisitedInThisRun);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// reveals complete dungeon (including disconnected dungeon segments) on automap
        /// </summary>
        private void RevealAll()
        {
            if (!gameobjectGeometry)
                return;
            MeshRenderer[] meshRenderers = gameobjectGeometry.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.enabled = true;
                Material[] materials = meshRenderer.materials;
                foreach (Material mat in meshRenderer.materials)
                {
                    mat.DisableKeyword("RENDER_IN_GRAYSCALE");
                }
                meshRenderer.materials = materials;
            }

            // so set the entrance beacon to active (discovered)
            gameobjectBeaconEntrancePosition.SetActive(true);
        }

        /// <summary>
        /// hides complete dungeon on automap
        /// </summary>
        public void HideAll()
        {
            if (!gameobjectGeometry)
                return;
            MeshRenderer[] meshRenderers = gameobjectGeometry.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.enabled = false;
            }

            // so set the entrance beacon to active=false (undiscovered)
            gameobjectBeaconEntrancePosition.SetActive(false);
        }

        void InitWhenInInteriorOrDungeon(StaticDoor? door = null, bool initFromLoadingSave = false)
        {
            if ((GameManager.Instance.IsPlayerInsideBuilding) && (door.HasValue))
            {
                createIndoorGeometryForAutomap(door.Value);
                restoreStateAutomapDungeon(true);
                resetAutomapSettingsFromExternalScript = true; // set flag so external script (DaggerfallAutomapWindow) can pull flag and reset automap values on next window push
                gameobjectGeometry.SetActive(false);
                gameobjectBeacons.SetActive(false);
            }
            else if ((GameManager.Instance.IsPlayerInsideDungeon) || (GameManager.Instance.IsPlayerInsidePalace))
            {
                createDungeonGeometryForAutomap();
                restoreStateAutomapDungeon(!initFromLoadingSave); // if a save game was loaded, do not reset the revisited state (don't set parameter forceNotVisitedInThisRun to true)
                resetAutomapSettingsFromExternalScript = true; // set flag so external script (DaggerfallAutomapWindow) can pull flag and reset automap values on next window push
                gameobjectGeometry.SetActive(false);
                gameobjectBeacons.SetActive(false);
            }
            else
            {
                Debug.LogError("Error in function InitWhenInInteriorOrDungeon: reached forbidden control flow point");
            }
        }

        private void OnTransitionToInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            InitWhenInInteriorOrDungeon(args.StaticDoor);
        }

        private void OnTransitionToDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            InitWhenInInteriorOrDungeon(null, false);
        }

        private void OnTransitionToExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            saveStateAutomapInterior();
            DestroyBeacons();
        }

        private void OnTransitionToDungeonExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            saveStateAutomapDungeon(true);
            DestroyBeacons();
        }

        void OnLoadEvent(SaveData_v1 saveData)
        {
            DestroyBeacons();

            if (GameManager.Instance.IsPlayerInside)
            {
                StaticDoor[] exteriorDoors = saveData.playerData.playerPosition.exteriorDoors;
                StaticDoor? door = null;
                if (exteriorDoors != null)
                {
                    door = exteriorDoors[0];
                }
                InitWhenInInteriorOrDungeon(door, true);
            }
        }

        void onNewGame()
        {
            // destroy automap geometry and beacons so that update function will create new automap geometry on its first iteration when a game has started
            if (gameobjectGeometry != null)
            {
                UnityEngine.Object.Destroy(gameobjectGeometry);
                gameobjectGeometry = null;
            }
            DestroyBeacons();
        }

        #endregion

        #region console_commands

        public static class AutoMapConsoleCommands
        {
            public static void RegisterCommands()
            {
                try
                {
                    ConsoleCommandsDatabase.RegisterCommand(RevealAll.name, RevealAll.description, RevealAll.usage, RevealAll.Execute);
                    ConsoleCommandsDatabase.RegisterCommand(HideAll.name, HideAll.description, HideAll.usage, HideAll.Execute);
                    ConsoleCommandsDatabase.RegisterCommand(DebugTeleportMode.name, DebugTeleportMode.description, DebugTeleportMode.usage, DebugTeleportMode.Execute);                    
                }
                catch (System.Exception ex)
                {
                    DaggerfallUnity.LogMessage(ex.Message, true);
                }
            }

            private static class RevealAll
            {
                public static readonly string name = "map_revealall";
                public static readonly string description = "Reveals entire map (including disconnected dungeon segments) on automap";
                public static readonly string usage = "map_revealall";


                public static string Execute(params string[] args)
                {
                    if (!GameManager.Instance.IsPlayerInside)
                    {
                        return "this command only has an effect when inside a dungeon";
                    }

                    DaggerfallAutomap daggerfallAutomap = DaggerfallAutomap.instance;
                    if (daggerfallAutomap == null)
                    {
                        return "DaggerfallAutomap instance not found";
                    }

                    daggerfallAutomap.RevealAll();
                    return "dungeon has been completely revealed on the automap";
                }
            }

            private static class HideAll
            {
                public static readonly string name = "map_hideall";
                public static readonly string description = "Hides entire map";
                public static readonly string usage = "map_hideall";


                public static string Execute(params string[] args)
                {
                    if (!GameManager.Instance.IsPlayerInside)
                    {
                        return "this command only has an effect when inside a dungeon";
                    }

                    DaggerfallAutomap daggerfallAutomap = DaggerfallAutomap.instance;
                    if (daggerfallAutomap == null)
                    {
                        return "DaggerfallAutomap instance not found";
                    }

                    daggerfallAutomap.HideAll();

                    return "hide complete on automap";
                }

            }

            private static class DebugTeleportMode
            {
                public static readonly string name = "map_teleportmode";
                public static readonly string description = "toggles (enables or disables) debug teleport mode (Control+Shift+Left Mouse Click on a dungeon segment will teleport player to it)";
                public static readonly string usage = "map_teleportmode";


                public static string Execute(params string[] args)
                {
                    DaggerfallAutomap daggerfallAutomap = DaggerfallAutomap.instance;
                    if (daggerfallAutomap == null)
                    {
                        return "DaggerfallAutomap instance not found";
                    }

                    bool oldDebugTeleportMode = daggerfallAutomap.DebugTeleportMode;
                    daggerfallAutomap.DebugTeleportMode = !oldDebugTeleportMode;
                    if (daggerfallAutomap.DebugTeleportMode == true)
                        return "debug teleport mode has been enabled";
                    else
                        return "debug teleport mode has been disabled";
                }
            }


        }

        #endregion
    }
}