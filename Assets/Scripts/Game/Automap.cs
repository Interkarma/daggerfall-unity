// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Michael Rauter (a.k.a. Nystul)
// Contributors:    Lypyl, Interkarma, Numidium
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
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;
using Wenzil.Console;

namespace DaggerfallWorkshop.Game
{
    using AutomapGeometryModelState = Automap.AutomapGeometryBlockState.AutomapGeometryBlockElementState.AutomapGeometryModelState;
    using AutomapGeometryBlockElementState = Automap.AutomapGeometryBlockState.AutomapGeometryBlockElementState;

    /// <summary>
    /// this class provides the automap core functionality like geometry creation and discovery mechanism 
    /// </summary>
    public class Automap : MonoBehaviour
    {
        #region Singleton
        private static Automap _instance;

        public static Automap instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<Automap>();
                return _instance;
            }
            private set { _instance = value; }
        }
        #endregion

        #region classes

        /// <summary>
        /// class to store state of discovery of a dungeon block or interior, this state can be used to restore state of GameObject gameobjectGeometry
        /// this class basically maps the two fields/states (MeshRenderer active state and Material shader keyword "RENDER_IN_GRAYSCALE") that are
        /// used for discovery state of the models inside GameObject gameobjectGeometry when an interior is loaded into this GameObject
        /// it is also used as type inside the blocks list in AutomapDungeonState
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
        /// it also stores list of user note markers
        /// </summary>
        public class AutomapDungeonState
        {
            public String locationName; /// name of the dungeon location
            public ulong timeInSecondsLastVisited; /// time in seconds (from DaggerfallDateTime) when player last visited the dungeon (used to store only the n last dungeons in save games)
            public bool entranceDiscovered; /// indicates if the dungeon entrance has already been discovered
            public List<AutomapGeometryBlockState> blocks; // automap geometry dungeon state
            public SortedList<int, NoteMarker> listUserNoteMarkers; // list of user note markers
            public Dictionary<string, TeleporterConnection> dictTeleporterConnections; // dictionary of discovered teleporter connections
        }

        public class NoteMarker
        {
            public string note;
            public Vector3 position;

            public NoteMarker(Vector3 position, string note)
            {
                this.note = note;
                this.position = position;
            }
        }

        /// <summary>
        /// we need to use this instead of Transform since Transform can not be serialized
        /// </summary>
        public class TeleporterTransform
        {
            public Vector3 position;
            public Quaternion rotation;

            public TeleporterTransform(Transform transform)
            {
                position = transform.position;
                rotation = transform.rotation;
            }

            public TeleporterTransform(Transform transform, Vector3 positionOffset)
            {
                position = transform.position + positionOffset;
                rotation = transform.rotation;
            }
        }

        public class TeleporterConnection
        {
            public TeleporterTransform teleporterEntrance;
            public TeleporterTransform teleporterExit;

            public override string ToString()
            {
              return "position: " + teleporterEntrance.position.ToString() + ", rotation: " + teleporterExit.rotation.ToString();
            }
        }

        #endregion

        #region Fields

        const string ResourceNameRotateArrow = "RotateArrow";

        const string NameGamobjectBeacons = "Beacons";

        const string NameGameobjectPlayerMarkerArrow = "PlayerMarkerArrow";
        const string NameGameobjectBeaconPlayerPosition = "BeaconPlayerPosition";
        const string NameGameobjectBeaconRotationPivotAxis = "BeaconRotationPivotAxis";
        const string NameGameobjectRotateArrow = "CurvedArrow";
        const string NameGameobjectBeaconEntrance = "BeaconEntrancePosition"; // gameobject will hold both entrance position marker beacon and cube entrance position marker
        const string NameGameobjectBeaconEntrancePositionMarker = "BeaconEntrancePositionMarker";
        const string NameGameobjectCubeEntrancePositionMarker = "CubeEntrancePositionMarker";        

        const string NameGameobjectTeleporterPortalMarker = "PortalMarker";
        const string NameGameobjectTeleporterSubStringStart = "Teleporter [";
        const string NameGameobjectTeleporterEntranceSubStringEnd = "] - Portal Entrance";
        const string NameGameobjectTeleporterExitSubStringEnd = "] - Portal Exit";
        const string NameGameobjectTeleporterConnection = "Teleporter Connection";

        const string NameGameobjectUserMarkerNotes = "UserMarkerNotes";
        const string NameGameobjectUserNoteMarkerSubStringStart = "UserNoteMarker_";

        const string NameGameobjectDiamond = "Diamond";

        const float raycastDistanceDown = 3.0f; // 3 meters should be enough (note: flying too high will result in geometry not being revealed by this raycast
        const float raycastDistanceViewDirection = 30.0f; // don't want to make it too easy to discover big halls - although it shouldn't be to small as well
        const float raycastDistanceEntranceMarkerReveal = 100.0f;

        const float scanRateGeometryDiscoveryInHertz = 5.0f; // n times per second the discovery of new geometry/meshes is checked

        GameObject gameobjectAutomap = null; // used to hold reference to instance of GameObject "Automap" (which has script Game/Automap.cs attached)

        GameObject gameobjectGeometry = null; // used to hold reference to instance of GameObject with level geometry used for automap        

        int layerAutomap; // layer used for level geometry of automap
        int layerPlayer; // player layer

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
        // this might look weirds - why not just notify the DaggerfallAutomapWindow class you may ask... - I wanted to make Automap inaware and independent of the actual GUI implementation
        // so communication will always be only from DaggerfallAutomapWindow to Automap class - so into other direction it works in that way that Automap will pull
        // from DaggerfallAutomapWindow via flags - this is why this flag and its Property ResetAutomapSettingsSignalForExternalScript exist
        bool resetAutomapSettingsFromExternalScript = false;

        GameObject gameobjectBeacons = null; // collector GameObject to hold beacons
        GameObject gameobjectPlayerMarkerArrow = null; // GameObject which will hold player marker arrow
        GameObject gameobjectBeaconPlayerPosition = null; // GameObject which will hold player marker ray (red ray)
        GameObject gameobjectBeaconEntrancePosition = null; // GameObject which will hold (dungeon) entrance marker ray (green ray)
        GameObject gameobjectBeaconRotationPivotAxis = null; // GameObject which will hold rotation pivot axis ray (blue ray)
        GameObject gameobjectRotationArrow1 = null; // GameObject which will hold rotation arrow1 (blue arrow)
        GameObject gameobjectRotationArrow2 = null; // GameObject which will hold rotation arrow2 (blue arrow)
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

        //readonly Vector3 rayPlayerPosOffset = new Vector3(-0.1f, 0.0f, +0.1f); // small offset to prevent ray for player position to be exactly in the same position as the rotation pivot axis
        //readonly Vector3 rayEntrancePosOffset = new Vector3(0.1f, 0.0f, +0.1f); // small offset to prevent ray for dungeon entrance to be exactly in the same position as the rotation pivot axis
        readonly Vector3 rayPlayerPosOffset = new Vector3(0.0f, 0.0f, 0.0f); // small offset to prevent ray for player position to be exactly in the same position as the rotation pivot axis
        readonly Vector3 rayEntrancePosOffset = new Vector3(0.0f, 0.0f, 0.0f); // small offset to prevent ray for dungeon entrance to be exactly in the same position as the rotation pivot axis

        bool debugTeleportMode = false;

        Texture2D textureMicroMap = null;

        SortedList<int, NoteMarker> listUserNoteMarkers = new SortedList<int, NoteMarker>(); // the list containing the user note markers, key is the id used when creating the user note marker
        int idOfUserMarkerNoteToBeChanged; // used to identify id of last double-clicked user note marker when changing note text
        DaggerfallInputMessageBox messageboxUserNote;
        GameObject gameObjectUserNoteMarkers = null; // container object for custom user notes markers


        /// <summary>
        /// this dictionary is used to store teleporter connections after being discovered by pc
        /// </summary>
        Dictionary<string, TeleporterConnection> dictTeleporterConnections = new Dictionary<string, TeleporterConnection>();

        GameObject gameobjectTeleporterMarkers = null; // container object for teleporter markers

        GameObject gameobjectTeleporterConnection = null; // on mouse hover over connection between portal entrance and exit is shown

        int numberOfDungeonMemorized = 1; /// 0... vanilla daggerfall behavior, 1... remember last visited dungeon, n... remember n visited dungeons

        // dungeon state is of type AutomapDungeonState which has its models in block field in a 4 level hierarchy
        AutomapDungeonState automapDungeonState = null;
        // interior state is of type AutomapGeometryBlockState which has its models in a 3 level hierarchy
        AutomapGeometryBlockState automapGeometryInteriorState = null;

        /// <summary>
        /// this dictionary is used to store the discovery state of dungeons in the game world
        /// the AutomapDungeonState is stored for each dungeon in this dictionary identified by its identifier string
        /// </summary>
        Dictionary<string, AutomapDungeonState> dictAutomapDungeonsDiscoveryState = new Dictionary<string, AutomapDungeonState>();

        bool iTweenCameraAnimationIsRunning = false; // indicates if iTween camera animation is running (i.e. teleporter portal jump animation plays)

        static bool isCreatingDungeonAutomapBaseGameObjects = false; // Indicates that the Automap is creating the models for a Castle's or Dungeon's Automap

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
        /// DaggerfallAutomapWindow script will use this to propagate its rotation pivot axis rotation
        /// (rotating the pixot axis will rotate the indicator arrows as they are child objects of the pivot axis)
        /// </summary>
        public Quaternion RotationPivotAxisRotation
        {
            get { return (gameobjectBeaconRotationPivotAxis.transform.rotation); }
            set { gameobjectBeaconRotationPivotAxis.transform.rotation = value; }
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

        /// <summary>
        /// returns the Texture2D containing the texture with the micro map (small texture with 2x2 pixels representing a dungeon block - note: rendering will render this texture at double size)
        /// </summary>
        public Texture2D TextureMicroMap
        {
            get { return textureMicroMap; }
        }

        /// <summary>
        /// returns true if a iTween camera animation is running
        /// (i.e. teleporter portal has been clicked and animation runs to jump to /
        /// focus portal at other end of teleporter connection)
        /// </summary>
        public bool ITweenCameraAnimationIsRunning
        {
            get { return iTweenCameraAnimationIsRunning; }
        }

        public static bool IsCreatingDungeonAutomapBaseGameObjects
        {
            get { return isCreatingDungeonAutomapBaseGameObjects; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// GetState() method for save system integration
        /// </summary>
        public Dictionary<string, AutomapDungeonState> GetState()
        {
            SaveStateAutomapDungeon(false);
            return dictAutomapDungeonsDiscoveryState;
        }

        /// <summary>
        /// SetState() method for save system integration
        /// </summary>
        public void SetState(Dictionary<string, AutomapDungeonState> savedDictAutomapDungeonsDiscoveryState)
        {
            dictAutomapDungeonsDiscoveryState = savedDictAutomapDungeonsDiscoveryState;
        }

        /// <summary>
        /// sets the number of dungeons that are memorized
        /// </summary>
        public void SetNumberOfDungeonMemorized(int n)
        {
            numberOfDungeonMemorized = n;
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to update when automap window was pushed - TODO: check if this can done with an event (if events work with gui windows)
        /// </summary>
        public void UpdateAutomapStateOnWindowPush()
        {
            // create teleport markers (that are not already present on map)
            // since new teleporters could have been discovered by pc since last time map was open this must be checked here       
            CreateTeleporterMarkers();

            SetActivationStateOfMapObjects(true);

            gameobjectPlayerMarkerArrow.transform.position = gameObjectPlayerAdvanced.transform.position;
            gameobjectPlayerMarkerArrow.transform.rotation = gameObjectPlayerAdvanced.transform.rotation;

            gameobjectBeaconPlayerPosition.transform.position = gameObjectPlayerAdvanced.transform.position + rayPlayerPosOffset;

            // create camera (if not present) that will render automap level geometry
            CreateAutomapCamera();

            // create lights that will light automap level geometry
            CreateLightsForAutomapGeometry();

            UpdateMicroMapTexture();

            UpdateSlicingPositionY();
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to update when automap window was popped - TODO: check if this can done with an event (if events work with gui windows)
        /// </summary>
        public void UpdateAutomapStateOnWindowPop()
        {
            // about SetActivationStateOfMapObjects(false):
            // this will not be enough if we will eventually allow gui windows to be opened while exploring the world
            // then it will be necessary to either only disable the colliders on the automap level geometry or
            // make player collision ignore colliders of objects in automap layer - I would clearly prefer this option
            SetActivationStateOfMapObjects(false);

            if ((GameManager.Instance.PlayerEnterExit.IsPlayerInside) && ((GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding) || (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon) || (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeonCastle)))
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
        /// DaggerfallAutomapWindow script will use this to signal this script to update when anything changed that requires Automap to update - TODO: check if this can done with an event (if events work with gui windows)
        /// </summary>
        public void ForceUpdate()
        {
            Update();
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to switch to the next available automap rendering mode
        /// </summary>
        public void SwitchToNextAutomapRenderMode()
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
        public void SwitchToAutomapRenderModeTransparent()
        {
            currentAutomapRenderMode = AutomapRenderMode.Transparent;
            Shader.DisableKeyword("AUTOMAP_RENDER_MODE_WIREFRAME");
            Shader.EnableKeyword("AUTOMAP_RENDER_MODE_TRANSPARENT");
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to switch to automap rendering mode "wireframe"
        /// </summary>
        public void SwitchToAutomapRenderModeWireframe()
        {
            currentAutomapRenderMode = AutomapRenderMode.Wireframe;
            Shader.EnableKeyword("AUTOMAP_RENDER_MODE_WIREFRAME");
            Shader.DisableKeyword("AUTOMAP_RENDER_MODE_TRANSPARENT");
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to switch to automap rendering mode "cutout"
        /// </summary>
        public void SwitchToAutomapRenderModeCutout()
        {
            currentAutomapRenderMode = AutomapRenderMode.Cutout;
            Shader.DisableKeyword("AUTOMAP_RENDER_MODE_WIREFRAME");
            Shader.DisableKeyword("AUTOMAP_RENDER_MODE_TRANSPARENT");                        
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to switch focus to next object of interest and return the GameObject which has focus
        /// </summary>
        /// <returns> the GameObject which has the focus </returns>
        public GameObject SwitchFocusToNextObject()
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
        /// gets the mouse hover over text that will be displayed in the status bar/info bar at the bottom of the automap window
        /// </summary>
        /// <param name="screenPosition">the mouse position to be used for raycast</param>
        /// <returns>the string containing the hover over text</returns>
        public string GetMouseHoverOverText(Vector2 screenPosition)
        {
            RaycastHit? nearestHit = null;
            GetRayCastNearestHitOnAutomapLayer(screenPosition, out nearestHit);

            if (nearestHit.HasValue)
            {
                // if hit geometry is user note marker
                if (nearestHit.Value.transform.name.StartsWith(NameGameobjectUserNoteMarkerSubStringStart))
                {
                    int id = System.Convert.ToInt32(nearestHit.Value.transform.name.Replace(NameGameobjectUserNoteMarkerSubStringStart, ""));
                    if (listUserNoteMarkers.ContainsKey(id))
                        return listUserNoteMarkers[id].note; // get user note by id
                }
                // if hit geometry is player position beacon
                else if (nearestHit.Value.transform.name == NameGameobjectBeaconPlayerPosition)
                {
                    return TextManager.Instance.GetLocalizedText("automapPlayerPositionBeacon");
                }
                // if hit geometry is player rotation pivot axis or rotation indicator arrows
                else if (nearestHit.Value.transform.name == NameGameobjectBeaconRotationPivotAxis || nearestHit.Value.transform.name == NameGameobjectRotateArrow)
                {
                    return TextManager.Instance.GetLocalizedText("automapRotationPivotAxis");
                }
                // if hit geometry is dungeon entrance/exit position beacon
                else if (nearestHit.Value.transform.name == NameGameobjectBeaconEntrancePositionMarker)
                {
                    return TextManager.Instance.GetLocalizedText("automapEntranceExitPositionBeacon");
                }
                // if hit geometry is dungeon entrance/exit position marker
                else if (nearestHit.Value.transform.name == NameGameobjectCubeEntrancePositionMarker)
                {
                    return TextManager.Instance.GetLocalizedText("automapEntranceExit");
                }
                // if hit geometry is player position marker arrow
                else if (nearestHit.Value.transform.name == NameGameobjectPlayerMarkerArrow)
                {
                    return TextManager.Instance.GetLocalizedText("automapPlayerMarker");
                }
                // if hit geometry is teleporter portal marker and its parent gameobject is an teleporter entrance
                else if (
                        nearestHit.Value.transform.name == NameGameobjectTeleporterPortalMarker &&
                        nearestHit.Value.transform.parent.transform.name.StartsWith(NameGameobjectTeleporterSubStringStart) &&
                        nearestHit.Value.transform.parent.transform.name.EndsWith(NameGameobjectTeleporterEntranceSubStringEnd)
                        )
                {
                    return TextManager.Instance.GetLocalizedText("automapTeleporterEntrance");
                }
                // if hit geometry is teleporter portal marker and its parent gameobject is an teleporter exit
                else if (
                        nearestHit.Value.transform.name == NameGameobjectTeleporterPortalMarker &&
                        nearestHit.Value.transform.parent.transform.name.StartsWith(NameGameobjectTeleporterSubStringStart) &&
                        nearestHit.Value.transform.parent.transform.name.EndsWith(NameGameobjectTeleporterExitSubStringEnd)
                        )
                {
                    return TextManager.Instance.GetLocalizedText("automapTeleporterExit");
                }
            }
            return ""; // otherwise return empty string (= no mouse hover over text will be displayed)
        }

        /// <summary>
        /// will make the mouse hover over gameobjects appear in the automap window
        /// </summary>
        /// <param name="screenPosition">the mouse position to be used for raycast</param>
        /// <returns>a flag that will indicate that the automap render panel will need to be updated</returns>
        public bool UpdateMouseHoverOverGameObjects(Vector2 screenPosition)
        {
            RaycastHit? nearestHit = null;
            GetRayCastNearestHitOnAutomapLayer(screenPosition, out nearestHit);

            if (nearestHit.HasValue)
            {
                // if hit geometry is teleporter portal marker and its parent gameobject is an teleporter entrance
                if (nearestHit.Value.transform.name == NameGameobjectTeleporterPortalMarker && gameobjectTeleporterConnection == null)
                {
                    gameobjectTeleporterConnection = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    UnityEngine.Object.Destroy(gameobjectTeleporterConnection.GetComponent<Collider>());
                    gameobjectTeleporterConnection.name = NameGameobjectTeleporterConnection;
                    gameobjectTeleporterConnection.transform.SetParent(gameobjectAutomap.transform);
                    gameobjectTeleporterConnection.layer = layerAutomap;
                    Material material = new Material(Shader.Find("Standard"));
                    material.color = new Color(0.43f, 0.34f, 0.85f, 1.0f);
                    material.SetFloat("_Metallic", 0.0f);
                    material.SetFloat("_Glossiness", 0.0f);
                    gameobjectTeleporterConnection.GetComponent<MeshRenderer>().material = material;

                    TeleporterConnection connection = null;
                    // hit portal marker is an entrance portal
                    if (nearestHit.Value.transform.parent.transform.name.EndsWith(NameGameobjectTeleporterEntranceSubStringEnd))
                    {
                        string key = nearestHit.Value.transform.parent.transform.name.Replace(NameGameobjectTeleporterSubStringStart, "");
                        key = key.Replace(NameGameobjectTeleporterEntranceSubStringEnd, "");
                        if (dictTeleporterConnections.ContainsKey(key))
                        {
                            connection = dictTeleporterConnections[key];
                        }
                    }
                    // hit portal marker is an exit portal
                    else if (nearestHit.Value.transform.parent.transform.name.EndsWith(NameGameobjectTeleporterExitSubStringEnd))
                    {
                        string key = nearestHit.Value.transform.parent.transform.name.Replace(NameGameobjectTeleporterSubStringStart, "");
                        key = key.Replace(NameGameobjectTeleporterExitSubStringEnd, "");
                        if (dictTeleporterConnections.ContainsKey(key))
                        {
                            connection = dictTeleporterConnections[key];
                        }
                    }

                    gameobjectTeleporterConnection.transform.position = (connection.teleporterEntrance.position + connection.teleporterExit.position) * 0.5f;
                    gameobjectTeleporterConnection.transform.localScale = new Vector3(0.2f, (connection.teleporterEntrance.position - connection.teleporterExit.position).magnitude * 0.5f, 0.2f);
                    gameobjectTeleporterConnection.transform.rotation = Quaternion.FromToRotation(Vector3.up, (connection.teleporterEntrance.position - connection.teleporterExit.position));

                    return true; // signalize to update automap render panel
                }

                if (nearestHit.Value.transform.name != NameGameobjectTeleporterPortalMarker)// if no teleporter portal was hit
                {
                    // destroy teleporter connection gameobject
                    if (gameobjectTeleporterConnection != null)
                    {
                        UnityEngine.GameObject.Destroy(gameobjectTeleporterConnection);                        
                    }
                    return true; // signalize to update automap render panel
                }
            }
            else
            {
                if (gameobjectTeleporterConnection != null)
                {
                    UnityEngine.GameObject.Destroy(gameobjectTeleporterConnection);                    
                }
                return true; // signalize to update automap render panel
            }

            return false; // no update required in the automap render panel
        }

        public bool TryForTeleporterPortalsAtScreenPosition(Vector2 screenPosition)
        {
            RaycastHit? nearestHit = null;
            GetRayCastNearestHitOnAutomapLayer(screenPosition, out nearestHit);

            if (nearestHit.HasValue)
            {
                // if hit gamobject is a teleporter portal marker
                if (nearestHit.Value.transform.name == NameGameobjectTeleporterPortalMarker)
                {
                    // hit portal marker is an entrance portal
                    if (nearestHit.Value.transform.parent.transform.name.EndsWith(NameGameobjectTeleporterEntranceSubStringEnd))
                    {
                        string key = nearestHit.Value.transform.parent.transform.name.Replace(NameGameobjectTeleporterSubStringStart, "");
                        key = key.Replace(NameGameobjectTeleporterEntranceSubStringEnd, "");
                        if (dictTeleporterConnections.ContainsKey(key))
                        {
                            TeleporterConnection connection = dictTeleporterConnections[key];

                            iTweenCameraAnimationIsRunning = true;
                            Hashtable moveParams = __ExternalAssets.iTween.Hash(
                            "position", cameraAutomap.transform.position - (connection.teleporterEntrance.position - connection.teleporterExit.position),
                            //"position", connection.teleporterExit.position - cameraAutomap.transform.forward * computedCameraBackwardDistance,
                            "time", 1.0f,
                            "ignoretimescale", true, // important since timescale == 0 in menus
                            "easetype", __ExternalAssets.iTween.EaseType.easeInOutSine,
                            "oncomplete", "ITweenAnimationComplete",
                            "oncompletetarget", this.gameObject // important to specify target so that oncomplete function is found (since it is not part of camera gameobject)
                            );
                            __ExternalAssets.iTween.MoveTo(cameraAutomap.gameObject, moveParams); // MoveTo call on camera gameObject
                        }
                    }
                    // hit portal marker is an exit portal
                    else if (nearestHit.Value.transform.parent.transform.name.EndsWith(NameGameobjectTeleporterExitSubStringEnd))
                    {
                        string key = nearestHit.Value.transform.parent.transform.name.Replace(NameGameobjectTeleporterSubStringStart, "");
                        key = key.Replace(NameGameobjectTeleporterExitSubStringEnd, "");
                        if (dictTeleporterConnections.ContainsKey(key))
                        {
                            TeleporterConnection connection = dictTeleporterConnections[key];

                            iTweenCameraAnimationIsRunning = true;
                            Hashtable moveParams = __ExternalAssets.iTween.Hash(
                            "position", cameraAutomap.transform.position - (connection.teleporterExit.position - connection.teleporterEntrance.position),
                            //"position", connection.teleporterEntrance.position - cameraAutomap.transform.forward * computedCameraBackwardDistance,
                            "time", 1.0f,
                            "ignoretimescale", true, // important since timescale == 0 in menus
                            "easetype", __ExternalAssets.iTween.EaseType.easeInOutSine,
                            "oncomplete", "ITweenAnimationComplete",
                            "oncompletetarget", this.gameObject // important to specify target so that oncomplete function is found (since it is not part of camera gameobject)
                            );
                            __ExternalAssets.iTween.MoveTo(cameraAutomap.gameObject, moveParams); // MoveTo call on camera gameObject
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private void ITweenAnimationComplete()
        {
            iTweenCameraAnimationIsRunning = false;
        }

        /// <summary>
        /// method which tries to add or edit an existing user marker on a given click position
        /// raycast test: if automap geometry is hit -> add new marker, if marker is hit -> edit marker, otherwise: do nothing
        /// </summary>
        /// <param name="screenPosition">the mouse position to be used for raycast</param>
        public void TryToAddOrEditUserNoteMarkerOnDungeonSegmentAtScreenPosition(Vector2 screenPosition, bool editUserNoteOnCreation)
        {
            RaycastHit? nearestHit = null;
            GetRayCastNearestHitOnAutomapLayer(screenPosition, out nearestHit);
            
            if (nearestHit.HasValue)
            {
                // if hit gamobject is not a user note marker (so for now it is possible to create markers around beacons - this is intended)
                if (!nearestHit.Value.transform.name.StartsWith(NameGameobjectUserNoteMarkerSubStringStart))
                {
                    // add a new user note marker
                    Vector3 spawningPosition = (nearestHit.Value.point) + nearestHit.Value.normal * 0.7f;

                    // test if there is already a user note marker near to the requested spawning position
                    var enumerator = listUserNoteMarkers.GetEnumerator();
                    while (enumerator.MoveNext())
                    {                        
                        if (Vector3.Distance(enumerator.Current.Value.position, spawningPosition) < 1.0f)
                            return; // if yes, do not add a new marker
                    }
                    
                    int id = listUserNoteMarkers.AddNext(new NoteMarker(spawningPosition, ""));
                    /*GameObject gameObjectNewUserNoteMarker =*/ CreateUserMarker(id, spawningPosition);

                    if (editUserNoteOnCreation)
                    {
                        EditUserNote(id);
                    }
                }
                else
                {
                    // edit user note marker
                    int id = System.Convert.ToInt32(nearestHit.Value.transform.name.Replace(NameGameobjectUserNoteMarkerSubStringStart, ""));
                    EditUserNote(id);
                }
            }
        }

        /// <summary>
        /// method which tries to delete an existing user marker on a given click position
        /// </summary>
        /// <param name="screenPosition">the mouse position to be used for raycast</param>
        /// <returns>true if a marker was hit and thus deleted</returns>
        public bool TryToRemoveUserNoteMarkerOnDungeonSegmentAtScreenPosition(Vector2 screenPosition)
        {
            RaycastHit? nearestHit = null;
            GetRayCastNearestHitOnAutomapLayer(screenPosition, out nearestHit);

            if (nearestHit.HasValue)
            {
                if (nearestHit.Value.transform.name.StartsWith(NameGameobjectUserNoteMarkerSubStringStart)) // if user note marker was hit
                {                    
                    int id = System.Convert.ToInt32(nearestHit.Value.transform.name.Replace(NameGameobjectUserNoteMarkerSubStringStart, ""));
                    if (listUserNoteMarkers.ContainsKey(id))
                        listUserNoteMarkers.Remove(id); // remove it from list
                    GameObject.Destroy(nearestHit.Value.transform.gameObject); // and destroy gameobject
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// method which tries if a raycast will hit automap geometry and if so will center camera on the click position
        /// </summary>
        /// <param name="screenPosition">the mouse position to be used for raycast</param>
        public void TryCenterAutomapCameraOnDungeonSegmentAtScreenPosition(Vector2 screenPosition)
        {
            RaycastHit? nearestHit = null;
            GetRayCastNearestHitOnAutomapLayer(screenPosition, out nearestHit);

            if (nearestHit.HasValue)
            {
                float distance = (cameraAutomap.transform.position - gameObjectPlayerAdvanced.transform.position).magnitude;
                cameraAutomap.transform.position = (nearestHit.Value.point);
                cameraAutomap.transform.position -= cameraAutomap.transform.forward * distance;
            }
        }

        /// <summary>
        /// method which tries if a raycast will hit automap geometry and if so will center rotation pivot axis on the click position
        /// </summary>
        /// <param name="screenPosition">the mouse position to be used for raycast</param>
        public void TrySetRotationPivotAxisToDungeonSegmentAtScreenPosition(Vector2 screenPosition)
        {
            RaycastHit? nearestHit = null;
            GetRayCastNearestHitOnAutomapLayer(screenPosition, out nearestHit);

            if (nearestHit.HasValue)
            {
                float yOffset = +1.0f;
                rotationPivotAxisPosition = new Vector3(nearestHit.Value.point.x, nearestHit.Value.point.y + yOffset, nearestHit.Value.point.z);
            }
        }

        /// <summary>
        /// DaggerfallAutomapWindow script will use this to signal this script to try to teleport player to dungeon segment shown at a provided screen position
        /// </summary>
        /// <param name="screenPosition"> the screen position of interest - if a dungeon segment is shown at this position player will be teleported there </param>
        public void TryTeleportPlayerToDungeonSegmentAtScreenPosition(Vector2 screenPosition)
        {
            RaycastHit? nearestHit = null;
            GetRayCastNearestHitOnAutomapLayer(screenPosition, out nearestHit);

            if (nearestHit.HasValue)
            {
                gameObjectPlayerAdvanced.transform.position = nearestHit.Value.point + Vector3.up * 0.1f;
                gameobjectBeaconPlayerPosition.transform.position = nearestHit.Value.point + rayPlayerPosOffset;
                gameobjectPlayerMarkerArrow.transform.position = nearestHit.Value.point + rayPlayerPosOffset;
            }

            // don't forget to update micro map texture, so new player position is visualized correctly on micro map
            UpdateMicroMapTexture();
        }

        #endregion

        #region Unity

        void Awake()
        {
            gameObjectPlayerAdvanced = GameObject.Find("PlayerAdvanced");
            if (!gameObjectPlayerAdvanced)
            {
                DaggerfallUnity.LogMessage("GameObject \"PlayerAdvanced\" not found! in script Automap (in function Awake())", true);
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

            layerPlayer = LayerMask.NameToLayer("Player");
            if (layerPlayer == -1)
            {
                DaggerfallUnity.LogMessage("Did not find Layer with name \"Player\"! entrance/exit marker discovery will not work", true);
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
            StartGameBehaviour.OnNewGame += OnNewGame;
            SaveLoadManager.OnLoad += OnLoadEvent;
            DaggerfallAction.OnTeleportAction += OnTeleportAction;
        }

        void OnDisable()
        {
            PlayerEnterExit.OnTransitionInterior -= OnTransitionToInterior;
            PlayerEnterExit.OnTransitionDungeonInterior -= OnTransitionToDungeonInterior;
            PlayerEnterExit.OnTransitionExterior -= OnTransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonExterior -= OnTransitionToDungeonExterior;
            StartGameBehaviour.OnNewGame -= OnNewGame;
            SaveLoadManager.OnLoad -= OnLoadEvent;
            DaggerfallAction.OnTeleportAction -= OnTeleportAction;
        }

        void Start()
        {
            // Set number of dungeons memorized
            SetNumberOfDungeonMemorized(DaggerfallUnity.Settings.AutomapNumberOfDungeons);

            gameobjectAutomap = GameObject.Find("Automap/InteriorAutomap");
            if (gameobjectAutomap == null)
            {
                DaggerfallUnity.LogMessage("GameObject \"Automap/InteriorAutomap\" missing! Create a GameObject called \"Automap\" in root of hierarchy and add a GameObject \"InternalAutomap\" to it, to this add script Game/Automap!\"", true);
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
            SwitchToAutomapRenderModeCutout();

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

            if (!GameManager.Instance.IsPlayerInside)
                return;

            // I am not super happy with doing this in the update function, but found no other way to make starting in dungeon correctly initialize the automap geometry
            if (!gameobjectGeometry)
            {
                // test if startup was inside dungeon or interior (and no transition event happened)                
                InitWhenInInteriorOrDungeon();
                // do initial geometry discovery
                if (gameobjectGeometry) // this is necessary since when game starts up it can happen that InitWhenInInteriorOrDungeon() does not create geometry because GameManger.Instance.IsPlayerInsideDungeon and GameManager.Instance.IsPlayerInsideCastle are false
                {
                    gameobjectGeometry.SetActive(true); // enable automap level geometry for revealing (so raycasts can hit colliders of automap level geometry)
                    CheckForNewlyDiscoveredMeshes();
                }
            }

            if (isOpenAutomap) // only do this stuff if automap is indeed open
            {
                UpdateSlicingPositionY();

                // update position of rotation pivot axis
                gameobjectBeaconRotationPivotAxis.transform.position = rotationPivotAxisPosition;
            }            
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// does a raycast based hit test with additional protection raycast and marks automap level geometry meshes as discovered and visited in this entering/dungeon run
        /// </summary>
        private RaycastHit? ScanWithRaycastInDirectionAndUpdateMeshesAndMaterials(
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

            if ((gameobjectGeometry != null) && ((GameManager.Instance.IsPlayerInsideBuilding) || (GameManager.Instance.IsPlayerInsideDungeon) || (GameManager.Instance.IsPlayerInsideCastle)))
            {
                // enable automap level geometry for revealing (so raycasts can hit colliders of automap level geometry)
                gameobjectGeometry.SetActive(true);

                // reveal geometry right below player - raycast down from player head position
                Vector3 rayStartPos = gameObjectPlayerAdvanced.transform.position + Camera.main.transform.localPosition;
                Vector3 rayDirection = Vector3.down;
                float rayDistance = raycastDistanceDown;
                Vector3 offsetSecondProtectionRaycast = Vector3.left * 0.1f; // will be used for protection raycast with slight offset of 10cm (protection against hole in daggerfall geometry prevention)            
                ScanWithRaycastInDirectionAndUpdateMeshesAndMaterials(rayStartPos, rayDirection, rayDistance, offsetSecondProtectionRaycast);

                // reveal geometry which player is looking at (and which is near enough)
                rayDirection = Camera.main.transform.rotation * Vector3.forward;
                // shift 10cm to the side (computed by normalized cross product of forward vector of view direction and down vector of view direction)
                offsetSecondProtectionRaycast = Vector3.Normalize(Vector3.Cross(Camera.main.transform.rotation * Vector3.down, rayDirection)) * 0.1f;
                rayDistance = raycastDistanceViewDirection;
                RaycastHit? hitForward = ScanWithRaycastInDirectionAndUpdateMeshesAndMaterials(rayStartPos, rayDirection, rayDistance, offsetSecondProtectionRaycast);

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
                        ScanWithRaycastInDirectionAndUpdateMeshesAndMaterials(rayStartPos + stepVector, rayDirection, rayDistance, offsetSecondProtectionRaycast);
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

                int layerMask = (1 << layerPlayer) + 1; // test against player and level geometry (+1... == 1 << 1 == "Default" layer == level geometry)

                Vector3 entranceMarkerPos = gameObjectEntrancePositionCubeMarker.transform.position;
                Vector3 playerColliderPos = playerCollider.transform.position; //GameManager.Instance.PlayerGPS.transform.position; //Camera.main.transform.position;
                // raycast 1
                Vector3 rayStartPos = entranceMarkerPos;
                Vector3 rayToPlayer = playerColliderPos - rayStartPos;                
                hitsTrueLevelGeometry = Physics.RaycastAll(rayStartPos, rayToPlayer, raycastDistanceEntranceMarkerReveal, layerMask);                
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
                hitsTrueLevelGeometry = Physics.RaycastAll(rayStartPos, rayToPlayer, raycastDistanceEntranceMarkerReveal, layerMask);                
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
                hitsTrueLevelGeometry = Physics.RaycastAll(rayStartPos, rayToPlayer, raycastDistanceEntranceMarkerReveal, layerMask);
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
                // only proceed if automap is not opened (otherwise command gameobjectGeometry.SetActive(false); will mess with automap rendering when scheduling is a bitch and overwrites changes from UpdateAutomapStateOnWindowPush()
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
        private void UpdateSlicingPositionY()
        {
            float slicingPositionY;
            if (!DaggerfallUnity.Settings.AutomapAlwaysMaxOutSliceLevel)
                slicingPositionY = gameObjectPlayerAdvanced.transform.position.y + Camera.main.transform.localPosition.y + slicingBiasY;
            else
                slicingPositionY = float.MaxValue;
            Shader.SetGlobalFloat("_SclicingPositionY", slicingPositionY);
        }

        //private void SetMaterialTransparency(Material material)
        //{
        //    material.SetOverrideTag("RenderType", "Transparent");
        //    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        //    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //    material.SetInt("_ZWrite", 0);
        //    material.DisableKeyword("_ALPHATEST_ON");
        //    material.DisableKeyword("_ALPHABLEND_ON");
        //    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        //    material.renderQueue = 3000;
        //}

        /// <summary>
        /// sets active state of map GameObjects like geometry, beacons, user note markers and teleporter markers
        /// used on automap open to enable (show) objects and hide them on automap close
        /// it is important to set them inactive when closing the map - so that ingame raycasts won't hit colliders of map objects
        /// </summary>
        /// <param name="active">the desired activation state for the map objects to be set</param>
        private void SetActivationStateOfMapObjects(bool active)
        {          
            gameobjectGeometry.SetActive(active);

            gameobjectBeacons.SetActive(active);

            if (gameObjectUserNoteMarkers != null)
                gameObjectUserNoteMarkers.SetActive(active);

            if (gameobjectTeleporterMarkers != null)
                gameobjectTeleporterMarkers.SetActive(active);
        }

        /// <summary>
        /// setup beacons: lazy creation of player marker arrow and beacons including
        /// player position beacon, dungeon entrance position beacon and rotation pivot axis position beacon
        /// will always get the current player position and update the player marker arrow position and the player position beacon
        /// will always get the rotation pivot axis position, will always set the dungeon entrance position (for interior: just takes the
        /// player position since this function is only called when geometry is created (when entering the dungeon or interior) -
        /// so the player position is at the entrance), for dungeon: will get the start marker from DaggerfallDungeon component
        /// </summary>
        private void SetupBeacons(StaticDoor ?entranceDoor = null)
        {
            if (!gameobjectBeacons)
            {
                gameobjectBeacons = new GameObject(NameGamobjectBeacons);
                gameobjectBeacons.layer = layerAutomap;
                gameobjectBeacons.transform.SetParent(gameobjectAutomap.transform);
            }
            if (!gameobjectPlayerMarkerArrow)
            {
                gameobjectPlayerMarkerArrow = GameObjectHelper.CreateDaggerfallMeshGameObject(99900, gameobjectBeacons.transform, false, null, true);
                gameobjectPlayerMarkerArrow.name = NameGameobjectPlayerMarkerArrow;
                gameobjectPlayerMarkerArrow.layer = layerAutomap;
                gameobjectPlayerMarkerArrow.AddComponent<MeshCollider>();
            }
            gameobjectPlayerMarkerArrow.transform.position = gameObjectPlayerAdvanced.transform.position;
            gameobjectPlayerMarkerArrow.transform.rotation = gameObjectPlayerAdvanced.transform.rotation;

            if (!gameobjectBeaconPlayerPosition)
            {
                gameobjectBeaconPlayerPosition = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                //UnityEngine.Object.Destroy(gameobjectBeaconPlayerPosition.GetComponent<Collider>());
                gameobjectBeaconPlayerPosition.name = NameGameobjectBeaconPlayerPosition;
                gameobjectBeaconPlayerPosition.transform.SetParent(gameobjectBeacons.transform);
                gameobjectBeaconPlayerPosition.layer = layerAutomap;
                gameobjectBeaconPlayerPosition.transform.localScale = new Vector3(0.3f, 50.0f, 0.3f);
                Material material = new Material(Shader.Find("Standard"));
                material.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
                //SetMaterialTransparency(material);
                gameobjectBeaconPlayerPosition.GetComponent<MeshRenderer>().material = material;
            }
            gameobjectBeaconPlayerPosition.transform.position = gameObjectPlayerAdvanced.transform.position + rayPlayerPosOffset;

            if (!gameobjectBeaconRotationPivotAxis)
            {
                //MeshCollider mc;
                gameobjectBeaconRotationPivotAxis = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                //UnityEngine.Object.Destroy(gameobjectBeaconRotationPivotAxis.GetComponent<Collider>());
                gameobjectBeaconRotationPivotAxis.name = NameGameobjectBeaconRotationPivotAxis;
                gameobjectBeaconRotationPivotAxis.transform.SetParent(gameobjectBeacons.transform);
                gameobjectBeaconRotationPivotAxis.layer = layerAutomap;
                gameobjectBeaconRotationPivotAxis.transform.localScale = new Vector3(0.15f, 50.2f, 0.15f);
                Material material = new Material(Shader.Find("Standard"));
                material.color = new Color(0.0f, 0.0f, 1.0f, 0.5f);
                //SetMaterialTransparency(material);
                gameobjectBeaconRotationPivotAxis.GetComponent<MeshRenderer>().material = material;

                gameobjectRotationArrow1 = (GameObject)Instantiate(Resources.Load(ResourceNameRotateArrow));
                gameobjectRotationArrow1.name = NameGameobjectRotateArrow;
                gameobjectRotationArrow1.transform.SetParent(gameobjectBeaconRotationPivotAxis.transform);
                gameobjectRotationArrow1.layer = layerAutomap;
                gameobjectRotationArrow1.transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
                gameobjectRotationArrow1.transform.GetChild(0).gameObject.layer = layerAutomap;
                gameobjectRotationArrow1.transform.localPosition = new Vector3(2.0f, -0.02f, -2.0f);
                gameobjectRotationArrow1.transform.localScale = new Vector3(0.15f, 0.0005f, 0.15f);
                gameobjectRotationArrow1.GetComponentInChildren<MeshRenderer>().material = material;

                gameobjectRotationArrow2 = (GameObject)Instantiate(Resources.Load(ResourceNameRotateArrow));
                gameobjectRotationArrow2.name = NameGameobjectRotateArrow;
                gameobjectRotationArrow2.transform.SetParent(gameobjectBeaconRotationPivotAxis.transform);
                gameobjectRotationArrow2.layer = layerAutomap;
                gameobjectRotationArrow2.transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
                gameobjectRotationArrow2.transform.GetChild(0).gameObject.layer = layerAutomap;
                gameobjectRotationArrow2.transform.localPosition = new Vector3(-2.0f, -0.02f, 2.0f);
                gameobjectRotationArrow2.transform.localScale = new Vector3(0.15f, 0.0005f, 0.15f);
                gameobjectRotationArrow2.transform.Rotate(0.0f, 180.0f, 0.0f);
                gameobjectRotationArrow2.GetComponentInChildren<MeshRenderer>().material = material;
            }
            gameobjectBeaconRotationPivotAxis.transform.position = rotationPivotAxisPosition;

            if (!gameobjectBeaconEntrancePosition)
            {
                gameobjectBeaconEntrancePosition = new GameObject(NameGameobjectBeaconEntrance);
                gameobjectBeaconEntrancePosition.transform.SetParent(gameobjectBeacons.transform);
                gameobjectBeaconEntrancePosition.layer = layerAutomap;

                GameObject gameobjectRay = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                //UnityEngine.Object.Destroy(gameobjectRay.GetComponent<Collider>());
                gameobjectRay.name = NameGameobjectBeaconEntrancePositionMarker;
                gameobjectRay.transform.SetParent(gameobjectBeaconEntrancePosition.transform);
                gameobjectRay.layer = layerAutomap;
                gameobjectRay.transform.localScale = new Vector3(0.3f, 50.0f, 0.3f);
                Material material = new Material(Shader.Find("Standard"));
                material.color = new Color(0.0f, 1.0f, 0.0f, 0.75f);
                //SetMaterialTransparency(material);
                gameobjectRay.GetComponent<MeshRenderer>().material = material;

                gameObjectEntrancePositionCubeMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //UnityEngine.Object.Destroy(gameObjectEntrancePositionCubeMarker.GetComponent<Collider>());
                gameObjectEntrancePositionCubeMarker.name = NameGameobjectCubeEntrancePositionMarker;
                gameObjectEntrancePositionCubeMarker.transform.SetParent(gameobjectBeaconEntrancePosition.transform);
                Material materialCubeEntracePositionMarker = new Material(Shader.Find("Standard"));
                materialCubeEntracePositionMarker.color = new Color(0.0f, 1.0f, 0.0f);
                gameObjectEntrancePositionCubeMarker.GetComponent<MeshRenderer>().material = materialCubeEntracePositionMarker;
                gameObjectEntrancePositionCubeMarker.layer = layerAutomap;
                gameObjectEntrancePositionCubeMarker.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }

            if ((GameManager.Instance.IsPlayerInsideDungeon) || (GameManager.Instance.IsPlayerInsideCastle))
            {
                // entrance marker to dungeon start marker
                DaggerfallDungeon dungeon = GameManager.Instance.DungeonParent.GetComponentInChildren<DaggerfallDungeon>();
                gameobjectBeaconEntrancePosition.transform.position = dungeon.StartMarker.transform.position + rayEntrancePosOffset;
                gameobjectBeaconEntrancePosition.SetActive(false); // set do undiscovered
            }
            else
            {
                // entrance marker to current position (position player entered)                
                StaticDoor door = entranceDoor.Value;
                gameobjectBeaconEntrancePosition.transform.position = door.ownerRotation * door.buildingMatrix.MultiplyPoint3x4(door.centre);
                gameobjectBeaconEntrancePosition.transform.position += door.ownerPosition;
                gameobjectBeaconEntrancePosition.SetActive(true); // set do discovered
            }
        }

        private void DestroyBeacons()
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
        /// destroys all user marker notes (will destroy all marker gameobjects and will also clear list of user note markers)
        /// </summary>
        private void DestroyUserMarkerNotes()
        {
            if (gameObjectUserNoteMarkers != null)
            {
                foreach (Transform child in gameObjectUserNoteMarkers.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            listUserNoteMarkers.Clear();
        }

        /// <summary>
        /// creates a diamond shaped primitive
        /// </summary>
        /// <returns>the diamond shaped primitive GameObject</returns>
        private GameObject CreateDiamondShapePrimitive()
        {
            GameObject gameobjectDiamondShape = new GameObject(NameGameobjectDiamond);
            MeshFilter meshFilter = gameobjectDiamondShape.AddComponent<MeshFilter>();

            Vector3 p0 = new Vector3(+0.5f, 0, -0.5f);
            Vector3 p1 = new Vector3(-0.5f, 0, -0.5f);
            Vector3 p2 = new Vector3(-0.5f, 0, +0.5f);
            Vector3 p3 = new Vector3(+0.5f, 0, +0.5f);            
            Vector3 s1 = new Vector3(0, 1.0f, 0);
            Vector3 s2 = new Vector3(0, -1.0f, 0);

            Mesh mesh = new Mesh();
            mesh.Clear();

            mesh.vertices = new Vector3[]{
            p0,p1,s1,
            p1,p2,s1,
            p2,p3,s1,
            p3,p0,s1,
            p1,p0,s2,
            p2,p1,s2,
            p3,p2,s2,
            p0,p3,s2
            };
            mesh.triangles = new int[]{
                0,1,2,
                3,4,5,
                6,7,8,
                9,10,11,
                12,13,14,
                15,16,17,
                18,19,20,
                21,22,23
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter.sharedMesh = mesh;

            gameobjectDiamondShape.AddComponent<MeshRenderer>();

            gameobjectDiamondShape.AddComponent<MeshCollider>();

            return gameobjectDiamondShape;
        }

        /// <summary>
        /// creates gameobjects for user marker (it is just the marker, not the note - the marker and note info is stored seperately and not touched by this function)
        /// </summary>
        /// <param name="id">the target id of the marker</param>
        /// <param name="spawningPosition">the requested spawning position of the marker</param>
        /// <returns>the GameObject with the marker</returns>
        private GameObject CreateUserMarker(int id, Vector3 spawningPosition)
        {
            if (gameObjectUserNoteMarkers == null)
            {
                gameObjectUserNoteMarkers = new GameObject(NameGameobjectUserMarkerNotes);
                gameObjectUserNoteMarkers.transform.SetParent(gameobjectAutomap.transform);
                gameObjectUserNoteMarkers.layer = layerAutomap;
            }
            GameObject gameObjectUserNoteMarker = CreateDiamondShapePrimitive();
            gameObjectUserNoteMarker.transform.SetParent(gameObjectUserNoteMarkers.transform);
            gameObjectUserNoteMarker.transform.position = spawningPosition;
            gameObjectUserNoteMarker.name = NameGameobjectUserNoteMarkerSubStringStart + id;
            Material materialUserNoteMarker = new Material(Shader.Find("Standard"));
            materialUserNoteMarker.color = new Color(1.0f, 0.55f, 0.0f);
            gameObjectUserNoteMarker.GetComponent<MeshRenderer>().material = materialUserNoteMarker;
            gameObjectUserNoteMarker.layer = layerAutomap;
            gameObjectUserNoteMarker.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            return gameObjectUserNoteMarker;
        }

        /// <summary>
        /// edits the user note with a given id
        /// </summary>
        /// <param name="id">the id of the user note to be edited</param>
        private void EditUserNote(int id)
        {
            // edit user note marker
            messageboxUserNote = new DaggerfallInputMessageBox(DaggerfallUI.UIManager, DaggerfallUI.Instance.AutomapWindow);
            messageboxUserNote.SetTextBoxLabel(TextManager.Instance.GetLocalizedText("youNote"));
            messageboxUserNote.TextPanelDistanceX = 5;
            messageboxUserNote.TextPanelDistanceY = 8;            
            if (listUserNoteMarkers.ContainsKey(id))
                messageboxUserNote.TextBox.Text = listUserNoteMarkers[id].note;
            messageboxUserNote.TextBox.Numeric = false;
            messageboxUserNote.TextBox.MaxCharacters = 50;
            messageboxUserNote.TextBox.WidthOverride = 306;
            idOfUserMarkerNoteToBeChanged = id;
            messageboxUserNote.OnGotUserInput += UserNote_OnGotUserInput;
            messageboxUserNote.Show();
        }

        /// <summary>
        /// add GameObjects for a teleport marker pair (entrance, exit) so it does show up on the map - if already present nothing is added
        /// </summary>
        /// <param name="startPoint">the transform of the entrance portal</param>
        /// <param name="endPoint">the transform of the exit portal</param>
        /// <param name="dictkey">the key used in the dictionary for this portal - will be used for unique naming of GameObjects</param>
        private void AddTeleporterMarkerOnMap(TeleporterTransform startPoint, TeleporterTransform endPoint, string dictkey)
        {
            if (gameobjectTeleporterMarkers == null)
            {
                gameobjectTeleporterMarkers = new GameObject("TeleporterMarkers");
                gameobjectTeleporterMarkers.transform.SetParent(gameobjectAutomap.transform);
                gameobjectTeleporterMarkers.layer = layerAutomap;
            }

            gameobjectTeleporterMarkers.SetActive(false);

            string teleporterEntranceName = NameGameobjectTeleporterSubStringStart + dictkey + NameGameobjectTeleporterEntranceSubStringEnd;
            if (gameobjectTeleporterMarkers.transform.Find(teleporterEntranceName) == null)
            {
                GameObject gameObjectTeleporterEntrance = new GameObject(teleporterEntranceName);
                gameObjectTeleporterEntrance.transform.SetParent(gameobjectTeleporterMarkers.transform);
                gameObjectTeleporterEntrance.transform.position = startPoint.position; // + Vector3.up * 1.0f;
                gameObjectTeleporterEntrance.transform.rotation = startPoint.rotation;
                gameObjectTeleporterEntrance.transform.Rotate(0.0f, 90.0f, 0.0f);
                gameObjectTeleporterEntrance.layer = layerAutomap;

                GameObject gameObjectTeleporterEntranceMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                gameObjectTeleporterEntranceMarker.transform.SetParent(gameObjectTeleporterEntrance.transform);
                gameObjectTeleporterEntranceMarker.name = NameGameobjectTeleporterPortalMarker;
                Material materialTeleporterEntranceMarker = new Material(Shader.Find("Standard"));
                materialTeleporterEntranceMarker.color = new Color(0.513f, 0.4f, 1.0f);
                materialTeleporterEntranceMarker.SetFloat("_Metallic", 0.0f);
                materialTeleporterEntranceMarker.SetFloat("_Glossiness", 0.0f);
                gameObjectTeleporterEntranceMarker.GetComponent<MeshRenderer>().material = materialTeleporterEntranceMarker;
                gameObjectTeleporterEntranceMarker.layer = layerAutomap;
                gameObjectTeleporterEntranceMarker.transform.localPosition = Vector3.zero;
                gameObjectTeleporterEntranceMarker.transform.localScale = new Vector3(2.0f, 0.1f, 1.0f);
                gameObjectTeleporterEntranceMarker.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 90.0f);
            }

            string teleporterExitName = NameGameobjectTeleporterSubStringStart + dictkey + NameGameobjectTeleporterExitSubStringEnd;
            if (gameobjectTeleporterMarkers.transform.Find(teleporterExitName) == null)
            {
                GameObject gameObjectTeleporterExit = new GameObject(teleporterExitName);
                gameObjectTeleporterExit.transform.SetParent(gameobjectTeleporterMarkers.transform);
                gameObjectTeleporterExit.transform.position = endPoint.position; // + Vector3.up * 0.2f;
                //gameObjectTeleporterExit.transform.rotation = endPoint.rotation;
                //gameObjectTeleporterExit.transform.Rotate(0.0f, 180.0f, 0.0f);
                gameObjectTeleporterExit.transform.rotation = startPoint.rotation; // take rotation from portal entrance
                gameObjectTeleporterExit.transform.Rotate(0.0f, 90.0f, 0.0f);
                gameObjectTeleporterExit.layer = layerAutomap;

                GameObject gameObjectTeleporterExitMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                gameObjectTeleporterExitMarker.transform.SetParent(gameObjectTeleporterExit.transform);
                gameObjectTeleporterExitMarker.name = NameGameobjectTeleporterPortalMarker;
                Material materialTeleporterExitMarker = new Material(Shader.Find("Standard"));
                materialTeleporterExitMarker.color = new Color(0.355f, 0.279f, 0.7f);
                materialTeleporterExitMarker.SetFloat("_Metallic", 0.0f);
                materialTeleporterExitMarker.SetFloat("_Glossiness", 0.0f);
                gameObjectTeleporterExitMarker.GetComponent<MeshRenderer>().material = materialTeleporterExitMarker;
                gameObjectTeleporterExitMarker.layer = layerAutomap;
                gameObjectTeleporterExitMarker.transform.localPosition = Vector3.zero;
                gameObjectTeleporterExitMarker.transform.localScale = new Vector3(2.0f, 0.1f, 1.0f);
                gameObjectTeleporterExitMarker.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 90.0f);
            }
        }

        /// <summary>
        /// creates GameObjects for the (already discovered) teleport markers if not already present
        /// </summary>
        void CreateTeleporterMarkers()
        {
            foreach (KeyValuePair<string, TeleporterConnection> connection in dictTeleporterConnections)
            {
                AddTeleporterMarkerOnMap(connection.Value.teleporterEntrance, connection.Value.teleporterExit, connection.Key);
            }
        }

        /// <summary>
        /// destroys all teleport markers (will destroy all teleport marker gameobjects and will also clear the dictionray of teleporter connections)
        /// </summary>
        void DestroyTeleporterMarkers()
        {
            if (gameobjectTeleporterMarkers != null)
            {
                foreach (Transform child in gameobjectTeleporterMarkers.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            dictTeleporterConnections.Clear();
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
        /// updates the micro map texture
        /// </summary>
        private void UpdateMicroMapTexture()
        {
            if (GameManager.Instance.IsPlayerInsideBuilding)
            {
                UpdateMicroMapTexture(null);
            }
            else
            {
                DFLocation location = GameManager.Instance.PlayerGPS.CurrentLocation;
                UpdateMicroMapTexture(location);
            }
        }

        /// <summary>
        /// updates the micro map texture depending on if player is inside or in dungeon
        /// </summary>
        /// <param name="currentLocation">provide null if player is interior, provide DFLocation if player is in dungeon</param>
        private void UpdateMicroMapTexture(DFLocation? currentLocation)
        {
            if (DaggerfallUnity.Settings.AutomapDisableMicroMap)
                return;

            if (!currentLocation.HasValue)
            {
                textureMicroMap = null;
                return;
            }

            DFLocation location = currentLocation.Value;
            int originX, originY, sizeX, sizeY;
            {
                const int sizeMin = 7;

                int blockXMin = 1000;
                int blockXMax = -1000;
                int blockZMin = 1000;
                int blockZMax = -1000;
                foreach (DFLocation.DungeonBlock block in location.Dungeon.Blocks)
                {
                    if (block.X < blockXMin) blockXMin = block.X;
                    if (block.X > blockXMax) blockXMax = block.X;
                    if (block.Z < blockZMin) blockZMin = block.Z;
                    if (block.Z > blockZMax) blockZMax = block.Z;
                }
                originX = -blockXMin + 1;
                originY = -blockZMin + 1;
                sizeX = sizeY = Math.Max(sizeMin, Math.Max(blockXMax + 1 + originX, blockZMax + 1 + originY));
            }

            int microMapBlockSizeInPixels = 2;
            int width = sizeX * microMapBlockSizeInPixels;
            int height = sizeY * microMapBlockSizeInPixels;
            textureMicroMap = new Texture2D(width, height, TextureFormat.ARGB32, false);
            textureMicroMap.filterMode = FilterMode.Point;

            Color[] colors = new Color[width * height];
            for (int i = 0; i < width * height; i++)
                colors[i] = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            textureMicroMap.SetPixels(0, 0, width, height, colors);

            foreach (DFLocation.DungeonBlock block in location.Dungeon.Blocks)
            {
                int xBlockPos = originX + block.X;
                int yBlockPos = originY + block.Z;
                for (int y = 0; y < microMapBlockSizeInPixels; y++)
                {
                    for (int x = 0; x < microMapBlockSizeInPixels; x++)
                    {
                        if (DaggerfallUnity.Settings.DungeonMicMapQoL)
                        {
                            if (block.BlockName.Substring(0, 1) == "B")
                                textureMicroMap.SetPixel(xBlockPos * microMapBlockSizeInPixels + x, yBlockPos * microMapBlockSizeInPixels + y, DaggerfallUnity.Settings.DunMicMapBorderColor);
                            else
                                textureMicroMap.SetPixel(xBlockPos * microMapBlockSizeInPixels + x, yBlockPos * microMapBlockSizeInPixels + y, DaggerfallUnity.Settings.DunMicMapInnerColor);
                        }
                        else
                        {
                            textureMicroMap.SetPixel(xBlockPos * microMapBlockSizeInPixels + x, yBlockPos * microMapBlockSizeInPixels + y, Color.yellow);
                        }
                    }
                }
            }

            // mark entrance position on micro map
            DaggerfallDungeon dungeon = GameManager.Instance.DungeonParent.GetComponentInChildren<DaggerfallDungeon>();
            float entrancePosX = dungeon.StartMarker.transform.position.x / RDBLayout.RDBSide;
            float entrancePosY = dungeon.StartMarker.transform.position.z / RDBLayout.RDBSide;
            int xPosOfBlock = originX * microMapBlockSizeInPixels + (int)(Mathf.Floor(entrancePosX*2)) * (microMapBlockSizeInPixels / 2);
            int yPosOfBlock = originY * microMapBlockSizeInPixels + (int)(Mathf.Floor(entrancePosY*2)) * (microMapBlockSizeInPixels / 2);
            for (int y = 0; y < microMapBlockSizeInPixels / 2; y++)
            {
                for (int x = 0; x < microMapBlockSizeInPixels / 2; x++)
                {
                    textureMicroMap.SetPixel(xPosOfBlock + x, yPosOfBlock + y, Color.green);
                }
            }

            // mark player position on micro map            
            float playerPosX = gameObjectPlayerAdvanced.transform.position.x / RDBLayout.RDBSide;
            float playerPosY = gameObjectPlayerAdvanced.transform.position.z / RDBLayout.RDBSide;
            xPosOfBlock = originX * microMapBlockSizeInPixels + (int)(Mathf.Floor(playerPosX * 2)) * (microMapBlockSizeInPixels / 2);
            yPosOfBlock = originY * microMapBlockSizeInPixels + (int)(Mathf.Floor(playerPosY * 2)) * (microMapBlockSizeInPixels / 2);
            for (int y = 0; y < microMapBlockSizeInPixels / 2; y++)
            {
                for (int x = 0; x < microMapBlockSizeInPixels / 2; x++)
                {
                    textureMicroMap.SetPixel(xPosOfBlock + x, yPosOfBlock + y, Color.red);
                }
            }

            textureMicroMap.Apply(false);
        }

        /// <summary>
        /// get the nearest RaycastHit with automap layer geometry
        /// </summary>
        /// <param name="screenPosition">the mouse position used for raycast</param>
        /// <param name="nearestHit">[out] the nearest RaycastHit with automap layer geometry, might be null if no geometry was hit</param>
        private void GetRayCastNearestHitOnAutomapLayer(Vector2 screenPosition, out RaycastHit? nearestHit)
        {
            Ray ray = cameraAutomap.ScreenPointToRay(screenPosition);

            RaycastHit[] hits = Physics.RaycastAll(ray, 10000, 1 << layerAutomap);

            nearestHit = null;
            float nearestDistance = float.MaxValue;
            foreach (RaycastHit hit in hits)
            {
                if ((hit.distance < nearestDistance) && (hit.collider.gameObject.GetComponent<MeshRenderer>().enabled))
                {
                    nearestHit = hit;
                    nearestDistance = hit.distance;
                }
            }
        }

        /// <summary>
        /// creates the indoor geometry used for automap rendering
        /// </summary>
        /// <param name="args"> the static door for loading the correct interior </param>
        private void CreateIndoorGeometryForAutomap(StaticDoor door)
        {
            String newGeometryName = DaggerfallInterior.GetSceneName(GameManager.Instance.PlayerGPS.CurrentLocation, door);

            //SetupMicroMapTexture(null); // setup micro map texture for interior geometry

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
                    SetupBeacons(door);
                }
            }

            // put all objects inside gameobjectGeometry in layer "Automap"
            SetLayerRecursively(gameobjectGeometry, layerAutomap);
            gameobjectGeometry.transform.SetParent(gameobjectAutomap.transform);

            // inject all materials of automap geometry with automap shader and reset MeshRenderer enabled state (this is used for the discovery mechanism)
            RaiseOnInjectMeshAndMaterialPropertiesEvent();

            //oldGeometryName = newGeometryName;
        }

        /// <summary>
        /// creates the dungeon geometry used for automap rendering
        /// </summary>
        private void CreateDungeonGeometryForAutomap()
        {
            DFLocation location = GameManager.Instance.PlayerGPS.CurrentLocation;
            String newGeometryName = DaggerfallDungeon.GetSceneName(location);

            //SetupMicroMapTexture(location); // setup micro map texture for dungeon

            if (gameobjectGeometry != null)
            {
                UnityEngine.Object.Destroy(gameobjectGeometry);
                gameobjectGeometry = null;
            }

            gameobjectGeometry = new GameObject("GeometryAutomap (Dungeon)");

            // disable this option to get all small dungeon parts as individual models
            bool combineRdbOptionState = DaggerfallUnity.Instance.Option_CombineRDB;
            DaggerfallUnity.Instance.Option_CombineRDB = false;

            foreach (Transform elem in GameManager.Instance.DungeonParent.transform)
            {
                if (elem.name.Contains("DaggerfallDungeon"))
                {
                    DFLocation.DungeonBlock[] blocks = GameManager.Instance.PlayerEnterExit.Dungeon.Summary.LocationData.Dungeon.Blocks;
                    GameObject gameobjectDungeon = new GameObject(newGeometryName);

                    // Create dungeon layout
                    foreach (DFLocation.DungeonBlock block in blocks)
                    {
                        DFBlock blockData;
                        int[] textureTable = GameManager.Instance.PlayerEnterExit?.Dungeon?.DungeonTextureTable;
                        Automap.isCreatingDungeonAutomapBaseGameObjects = true;
                        GameObject gameobjectBlock = RDBLayout.CreateBaseGameObject(block.BlockName, null, out blockData, textureTable, true, null, false, true);
                        Automap.isCreatingDungeonAutomapBaseGameObjects = false;
                        gameobjectBlock.transform.position = new Vector3(block.X * RDBLayout.RDBSide, 0, block.Z * RDBLayout.RDBSide);

                        gameobjectBlock.transform.SetParent(gameobjectDungeon.transform);
                        AddWater(gameobjectBlock, block.WaterLevel);
                    }

                    gameobjectDungeon.transform.SetParent(gameobjectGeometry.transform);

                    // copy position and rotation from real level geometry
                    gameobjectGeometry.transform.position = elem.transform.position;
                    gameobjectGeometry.transform.rotation = elem.transform.rotation;

                    // do this here when DaggerfallDungeon GameObject is present, so that a call to SetupBeacons() will not fail (it needs the DaggerfallDungeon component of this GameObject)                    
                    SetupBeacons();

                    break;
                }
            }

            // set this option back to previous state (to reset to previous behavior)
            if (combineRdbOptionState)
                DaggerfallUnity.Instance.Option_CombineRDB = true;

            // put all objects inside gameobjectGeometry in layer "Automap"
            SetLayerRecursively(gameobjectGeometry, layerAutomap);
            gameobjectGeometry.transform.SetParent(gameobjectAutomap.transform);

            // inject all materials of automap geometry with automap shader and reset MeshRenderer enabled state (this is used for the discovery mechanism)
            RaiseOnInjectMeshAndMaterialPropertiesEvent();

            //oldGeometryName = newGeometryName;
        }

        private void AddWater(GameObject parent, short nativeBlockWaterLevel)
        {
            // Exit if no water present
            if (nativeBlockWaterLevel == 10000)
                return;

            float waterLevel = nativeBlockWaterLevel * -1 * MeshReader.GlobalScale;
            MeshRenderer[] renderers = parent.GetComponentsInChildren<MeshRenderer>();

            if (renderers != null)
            {
                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                foreach (MeshRenderer renderer in renderers)
                {
                    renderer.GetPropertyBlock(propertyBlock);
                    propertyBlock.SetFloat("_WaterLevel", waterLevel);
                    renderer.SetPropertyBlock(propertyBlock);
                }
            }
        }

        /// <summary>
        /// creates the automap camera if not present and sets camera default settings, registers camera to compass
        /// </summary>
        private void CreateAutomapCamera()
        {
            if (!cameraAutomap)
            {
                gameObjectCameraAutomap = new GameObject("CameraAutomap");
                cameraAutomap = gameObjectCameraAutomap.AddComponent<Camera>();
                cameraAutomap.clearFlags = CameraClearFlags.SolidColor;
                cameraAutomap.cullingMask = 1 << layerAutomap;
                cameraAutomap.renderingPath = RenderingPath.Forward;
                cameraAutomap.nearClipPlane = 0.7f;
                cameraAutomap.farClipPlane = 5000.0f;
                gameObjectCameraAutomap.AddComponent<NoFogCamera>();
                gameObjectCameraAutomap.transform.SetParent(gameobjectAutomap.transform);
            }
        }

        /// <summary>
        /// creates (if not present) automap lights that light the automap level geometry
        /// </summary>
        private void CreateLightsForAutomapGeometry()
        {
            if (!gameobjectAutomapKeyLight)
            {
                gameobjectAutomapKeyLight = new GameObject("AutomapKeyLight");
                gameobjectAutomapKeyLight.transform.rotation = Quaternion.Euler(50.0f, 270.0f, 0.0f);
                Light keyLight = gameobjectAutomapKeyLight.AddComponent<Light>();
                keyLight.type = LightType.Directional;
                keyLight.cullingMask = 1 << layerAutomap;
                gameobjectAutomapKeyLight.transform.SetParent(gameobjectAutomap.transform);
            }

            if (!gameobjectAutomapFillLight)
            {
                gameobjectAutomapFillLight = new GameObject("AutomapFillLight");
                gameobjectAutomapFillLight.transform.rotation = Quaternion.Euler(50.0f, 126.0f, 0.0f);
                Light fillLight = gameobjectAutomapFillLight.AddComponent<Light>();
                fillLight.type = LightType.Directional;
                fillLight.cullingMask = 1 << layerAutomap;
                gameobjectAutomapFillLight.transform.SetParent(gameobjectAutomap.transform);
            }

            if (!gameobjectAutomapBackLight)
            {
                gameobjectAutomapBackLight = new GameObject("AutomapBackLight");
                gameobjectAutomapBackLight.transform.rotation = Quaternion.Euler(50.0f, 0.0f, 0.0f);
                Light backLight = gameobjectAutomapBackLight.AddComponent<Light>();
                backLight.type = LightType.Directional;
                backLight.cullingMask = 1 << layerAutomap;
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
            else if ((GameManager.Instance.IsPlayerInsideDungeon) || (GameManager.Instance.IsPlayerInsideCastle))
            {
                Light keyLight = gameobjectAutomapKeyLight.GetComponent<Light>();
                Light fillLight = gameobjectAutomapFillLight.GetComponent<Light>();
                Light backLight = gameobjectAutomapBackLight.GetComponent<Light>();

                keyLight.intensity = 0.9f;
                fillLight.intensity = 0.7f;
                backLight.intensity = 0.5f;
            }
        }

        /// <summary>
        /// saves discovery state to object automapGeometryInteriorState
        /// this class is mapping the hierarchy inside the GameObject gameObjectGeometry in such way
        /// that the MeshRenderer enabled state for objects in the 3rd hierarchy level (which are the actual models)
        /// is matching value of field "discovered" in AutomapGeometryBlockState.AutomapGeometryBlockElementState.AutomapGeometryModelState
        /// </summary>
        private void SaveStateAutomapInterior()
        {
            if (!gameobjectGeometry)
              return;
        
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
        /// saves discovery state to object automapDungeonState
        /// this class is mapping the hierarchy inside the GameObject gameObjectGeometry in such way
        /// that the MeshRenderer enabled state for objects in the 4th hierarchy level (which are the actual models)
        /// is matching value of field "discovered" in AutomapDungeonState.AutomapGeometryBlockState.AutomapGeometryBlockElementState.AutomapGeometryModelState
        /// dictionary of dungeon states of visited dungeons is then updated (which is used for save game mechanism)
        /// </summary>
        private void SaveStateAutomapDungeon(bool forceSaveOfDungeonDiscoveryState)
        {
            if ((numberOfDungeonMemorized == 0)&&(!GameManager.Instance.IsPlayerInside)) // if discovery state of no dungeon has to be remembered, clear dictionary and skip the rest of this function
            {
                dictAutomapDungeonsDiscoveryState.Clear();
                return;
            }

            if ((!GameManager.Instance.IsPlayerInside)&&(!forceSaveOfDungeonDiscoveryState) || // if player is outside just skip this function
                gameobjectGeometry == null)                                                    // also skip if object geomtry not initialised
            {
                return;
            }

            int updatedNumberOfDungeonMemorized = numberOfDungeonMemorized;
            if ((numberOfDungeonMemorized == 0) && (GameManager.Instance.IsPlayerInside))
            {
                updatedNumberOfDungeonMemorized = 1;
            }

            Transform gameObjectGeometryDungeon = gameobjectGeometry.transform.GetChild(0);
            automapDungeonState = new AutomapDungeonState();
            automapDungeonState.locationName = gameObjectGeometryDungeon.name;
            automapDungeonState.entranceDiscovered = ((gameobjectBeaconEntrancePosition) && (gameobjectBeaconEntrancePosition.activeSelf)); // get entrance marker discovery state
            automapDungeonState.timeInSecondsLastVisited = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds();
            automapDungeonState.blocks = new List<AutomapGeometryBlockState>();

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
                        MeshRenderer[] meshRenderers = currentTransformMesh.GetComponentsInChildren<MeshRenderer>();
                        for (int i = 0; i < meshRenderers.Length; i++)
                        {
                            if ((meshRenderers[i]) && (meshRenderers[i].enabled))
                            {
                                model.discovered = true;
                                model.visitedInThisRun = !meshRenderers[i].materials[0].IsKeywordEnabled("RENDER_IN_GRAYSCALE"); // all materials of model have the same setting - so just material[0] is tested
                                break;
                            }
                            else
                            {
                                model.discovered = false;
                                model.visitedInThisRun = false;
                            }
                        }
                        models.Add(model);
                    }

                    blockElement.models = models;
                    blockElements.Add(blockElement);
                }
                automapGeometryBlockState.blockElements = blockElements;
                automapDungeonState.blocks.Add(automapGeometryBlockState);
            }

            // store list of user note markers
            automapDungeonState.listUserNoteMarkers = new SortedList<int, NoteMarker>(listUserNoteMarkers);

            // store list of teleporter connections
            automapDungeonState.dictTeleporterConnections = new Dictionary<string, TeleporterConnection>(dictTeleporterConnections);

            // replace or add discovery state for current dungeon
            DFLocation dfLocation = GameManager.Instance.PlayerGPS.CurrentLocation;
            string locationStringIdentifier = string.Format("{0}/{1}", dfLocation.RegionName, dfLocation.Name);
            if (dictAutomapDungeonsDiscoveryState.ContainsKey(locationStringIdentifier))
            {
                dictAutomapDungeonsDiscoveryState[locationStringIdentifier] = automapDungeonState;
            }
            else
            {
                dictAutomapDungeonsDiscoveryState.Add(locationStringIdentifier, automapDungeonState);
            }

            // make list out of dictionary and sort by last time visited - then get rid of dungeons in dictionary that weren't visited lately
            List<KeyValuePair<string, AutomapDungeonState>> sortedList = new List<KeyValuePair<string, AutomapDungeonState>>(dictAutomapDungeonsDiscoveryState);
            sortedList.Sort(
                delegate (KeyValuePair<string, AutomapDungeonState> firstPair,
                KeyValuePair<string, AutomapDungeonState> nextPair)
                {
                    return nextPair.Value.timeInSecondsLastVisited.CompareTo(firstPair.Value.timeInSecondsLastVisited);
                }
            );

            //if there are more dungeons remembered than allowed - get rid of dungeons in dictionary that weren't visited lately
            if (sortedList.Count > updatedNumberOfDungeonMemorized)
            {
                ulong timeInSecondsLimit = sortedList[updatedNumberOfDungeonMemorized - 1].Value.timeInSecondsLastVisited;

                foreach (KeyValuePair<string, AutomapDungeonState> entry in sortedList) // use sorted list and iterate over it - remove elements of dictionary
                {
                    if (entry.Value.timeInSecondsLastVisited < timeInSecondsLimit)
                    {
                        dictAutomapDungeonsDiscoveryState.Remove(entry.Key); // remove this dungeon's entry from dictionary
                    }
                }
            }

        }

        void UpdateMeshRendererInteriorState(ref MeshRenderer meshRenderer, AutomapGeometryBlockState automapGeometryInteriorState, int indexElement, int indexModel, bool forceNotVisitedInThisRun)
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
        private void RestoreStateAutomapInterior(bool forceNotVisitedInThisRun = false)
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
                        UpdateMeshRendererInteriorState(ref meshRenderer, automapGeometryInteriorState, indexElement, indexModel, forceNotVisitedInThisRun);
                    }
                }
            }

            DestroyUserMarkerNotes();
            DestroyTeleporterMarkers();
        }

        void UpdateMeshRendererDungeonState(MeshRenderer meshRenderer, AutomapDungeonState automapDungeonState, int indexBlock, int indexElement, int indexModel, bool forceNotVisitedInThisRun)
        {
            if ((indexBlock < automapDungeonState.blocks.Count) && (indexElement < automapDungeonState.blocks[indexBlock].blockElements.Count) &&
                (indexModel < automapDungeonState.blocks[indexBlock].blockElements[indexElement].models.Count) &&
                (automapDungeonState.blocks[indexBlock].blockElements[indexElement].models[indexModel].discovered == true))
            {
                meshRenderer.enabled = true;

                if ((!forceNotVisitedInThisRun)&&(automapDungeonState.blocks[indexBlock].blockElements[indexElement].models[indexModel].visitedInThisRun))
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
        /// restores discovery state from automapDungeonState onto gameobjectGeometry
        /// automapDungeonState is loaded from dictionary of dungeon states of visited dungeons
        /// this class is mapping the value of field "discovered" (AutomapDungeonState.AutomapGeometryBlockState.AutomapGeometryBlockElementState.AutomapGeometryModelState)
        /// inside object automapDungeonState to the objects inside the 4th hierarchy level of GameObject gameObjectGeometry (which are the actual models)
        /// in such way that the MeshRenderer enabled state for these objects match the value of field "discovered"
        /// </summary>
        /// <param name="forceNotVisitedInThisRun"> if set to true geometry is restored and its state is forced to not being visited in this run </param>
        private void RestoreStateAutomapDungeon(bool forceNotVisitedInThisRun = false)
        {
            Transform location = gameobjectGeometry.transform.GetChild(0);

            HideAll(); // clear discovery state of geometry as initial starting point

            // make sure no user note markers are left over from previous dungeon run
            DestroyUserMarkerNotes();
            // make sure no teleporter markers are left over from previous dungeon run
            DestroyTeleporterMarkers();

            DFLocation dfLocation = GameManager.Instance.PlayerGPS.CurrentLocation;
            string locationStringIdentifier = string.Format("{0}/{1}", dfLocation.RegionName, dfLocation.Name);
            if (dictAutomapDungeonsDiscoveryState.ContainsKey(locationStringIdentifier))
            {
                automapDungeonState = dictAutomapDungeonsDiscoveryState[locationStringIdentifier];
            }
            else
            {
                automapDungeonState = null;                
            }

            if (automapDungeonState == null)
                return;

            gameobjectBeaconEntrancePosition.SetActive(automapDungeonState.entranceDiscovered); // set entrance marker discovery state         

            if (location.name != automapDungeonState.locationName)
                return;

            for (int indexBlock = 0; indexBlock < location.childCount; indexBlock++)
            {
                Transform currentBlock = location.GetChild(indexBlock);

                if (currentBlock.name != automapDungeonState.blocks[indexBlock].blockName)
                    return;

                for (int indexElement = 0; indexElement < currentBlock.childCount; indexElement++)
                {
                    Transform currentTransformElement = currentBlock.GetChild(indexElement);

                    for (int indexModel = 0; indexModel < currentTransformElement.childCount; indexModel++)
                    {
                        Transform currentTransformModel = currentTransformElement.GetChild(indexModel);

                        MeshRenderer[] meshRenderers = currentTransformModel.GetComponentsInChildren<MeshRenderer>();
                        for (int i = 0; i < meshRenderers.Length; i++)
                        {
                            if (meshRenderers[i])
                            {
                                UpdateMeshRendererDungeonState(meshRenderers[i], automapDungeonState, indexBlock, indexElement, indexModel, forceNotVisitedInThisRun);
                            }
                        }
                    }
                }
            }

            // (try to) load user note markers
            var loadedListUserNoteMarkers = automapDungeonState.listUserNoteMarkers;
            if (loadedListUserNoteMarkers != null)
                listUserNoteMarkers = loadedListUserNoteMarkers; //new SortedList<int, NoteMarker>(loadedListUserNoteMarkers);

            foreach (var userNoteMarker in listUserNoteMarkers)
            {
                CreateUserMarker(userNoteMarker.Key, userNoteMarker.Value.position);
            }

            // (try to) load teleporter connections (creation of teleporter gameobjects for map is done on map open (UpdateAutomapStateOnWindowPush))
            var loadedDictTeleporterConnections = automapDungeonState.dictTeleporterConnections; // new Dictionary<string, TeleporterConnection>(automapDungeonState.dictTeleporterConnections);
            if (loadedDictTeleporterConnections != null)
                dictTeleporterConnections = loadedDictTeleporterConnections;
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

        /// <summary>
        /// The percentage of dungeon explored.
        /// </summary>
        public int ExploredPercentage()
        {
            int explored = 0;
            if (!gameobjectGeometry)
                return explored;
            MeshRenderer[] meshRenderers = gameobjectGeometry.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer meshRenderer in meshRenderers)
                if (meshRenderer.enabled)
                    explored++;

            return (int)(((double) explored / meshRenderers.Length) * 100);
        }

        void InitWhenInInteriorOrDungeon(StaticDoor? door = null, bool initFromLoadingSave = false)
        {
            if ((GameManager.Instance.IsPlayerInsideBuilding) && (door.HasValue))
            {
                CreateIndoorGeometryForAutomap(door.Value);
                RestoreStateAutomapDungeon(true);
                resetAutomapSettingsFromExternalScript = true; // set flag so external script (DaggerfallAutomapWindow) can pull flag and reset automap values on next window push

                SetActivationStateOfMapObjects(false);
            }
            else if ((GameManager.Instance.IsPlayerInsideDungeon) || (GameManager.Instance.IsPlayerInsideCastle))
            {
                CreateDungeonGeometryForAutomap();
                RestoreStateAutomapDungeon(!initFromLoadingSave); // if a save game was loaded, do not reset the revisited state (don't set parameter forceNotVisitedInThisRun to true)
                resetAutomapSettingsFromExternalScript = true; // set flag so external script (DaggerfallAutomapWindow) can pull flag and reset automap values on next window push

                SetActivationStateOfMapObjects(false);
            }
            else
            {
                Debug.LogError("Error in function InitWhenInInteriorOrDungeon: reached forbidden control flow point");
            }
        }

        void UserNote_OnGotUserInput(DaggerfallInputMessageBox sender, string input)
        {
            listUserNoteMarkers[idOfUserMarkerNoteToBeChanged].note = input;
            messageboxUserNote = null;
        }

        #endregion

        #region events

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
            SaveStateAutomapInterior();
            DestroyBeacons();
        }

        private void OnTransitionToDungeonExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            SaveStateAutomapDungeon(true);
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

        void OnNewGame()
        {
            // destroy automap geometry and beacons so that update function will create new automap geometry on its first iteration when a game has started
            if (gameobjectGeometry != null)
            {
                UnityEngine.Object.Destroy(gameobjectGeometry);
                gameobjectGeometry = null;
            }
            DestroyBeacons();
            DestroyUserMarkerNotes();
            DestroyTeleporterMarkers();
        }

        void OnTeleportAction(GameObject triggerObj, GameObject nextObj)
        {
            TeleporterConnection connection = new TeleporterConnection();
            connection.teleporterEntrance = new TeleporterTransform(triggerObj.transform, (Vector3.up * 1.0f));
            connection.teleporterExit = new TeleporterTransform(nextObj.transform, (Vector3.up * 0.2f));
            if (!dictTeleporterConnections.ContainsKey(connection.ToString()))
                dictTeleporterConnections.Add(connection.ToString(), connection);
        }

        /// <summary>
        /// Send an event instructing all Automap models to apply Automap materials and properties to their MeshRenderers.
        /// </summary>
        /// <param name="resetDiscoveryState"> if true resets the discovery state for geometry that needs to be discovered (when inside dungeons or castles) </param>
        public delegate void OnInjectMeshAndMaterialPropertiesEventHandler(bool playerIsInsideBuilding, Vector3 playerAdvancedPos, Material automapMaterial, bool resetDiscoveryState);
        public static event OnInjectMeshAndMaterialPropertiesEventHandler OnInjectMeshAndMaterialProperties;
        private void RaiseOnInjectMeshAndMaterialPropertiesEvent(bool resetDiscoveryState = true)
        {
            if (OnInjectMeshAndMaterialProperties != null)
            {
                bool playerIsInsideBuilding = false;
                if (GameManager.Instance.IsPlayerInsideBuilding)
                    playerIsInsideBuilding = true;

                Vector3 playerAdvancedPos = gameObjectPlayerAdvanced.transform.position;
                Material automapMaterial = new Material(Shader.Find("Daggerfall/Automap"));
                automapMaterial.SetColor("_WaterColor", GameManager.Instance.PlayerEnterExit.UnderwaterFog.waterMapColor);

                OnInjectMeshAndMaterialProperties(playerIsInsideBuilding, playerAdvancedPos, automapMaterial, resetDiscoveryState);
            }
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

                    Automap daggerfallAutomap = Automap.instance;
                    if (daggerfallAutomap == null)
                    {
                        return "Automap instance not found";
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

                    Automap automap = Automap.instance;
                    if (automap == null)
                    {
                        return "Automap instance not found";
                    }

                    automap.HideAll();

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
                    Automap daggerfallAutomap = Automap.instance;
                    if (daggerfallAutomap == null)
                    {
                        return "Automap instance not found";
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

    #region class extensions
    /// <summary>
    /// extension for SortedList class - we want to reuse id's if list items have been deleted from the list and thus the id is free - so we need to have a function that supports this
    /// </summary>
    public static class SortedListExtensions
    {
        ///Add item to next (lowest free) numeric key in SortedList, returns id (key) of the newly added item
        public static int AddNext<T>(this SortedList<int, T> sortedList, T item)
        {
            int key = 0;
            int count = sortedList.Count;

            int counter = 0;
            do
            {
                if (count == 0) break;
                int nextKeyInList = sortedList.Keys[counter++];

                if (key != nextKeyInList) break;

                key = nextKeyInList + 1;

                if (count == 1 || counter == count) break;


                if (key != sortedList.Keys[counter])
                    break;

            } while (true);

            sortedList.Add(key, item);
            return key;
        }

    }

    #endregion
}