// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Michael Rauter (a.k.a. Nystul)
// Contributors:    Interkarma
// 
// Notes:
//

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

//#define DEBUG_NameplateS

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// this class provides the automap core functionality like geometry creation and discovery mechanism 
    /// </summary>
    public class DaggerfallExteriorAutomap : MonoBehaviour
    {
        #region Singleton
        private static DaggerfallExteriorAutomap _instance;

        public static DaggerfallExteriorAutomap instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<DaggerfallExteriorAutomap>();
                return _instance;
            }
            private set { _instance = value; }
        }
        #endregion

        #region Fields

        GameObject gameobjectExteriorAutomap = null; // used to hold reference to instance of GameObject "ExteriorAutomap" (which has script Game/DaggerfallExteriorAutomap.cs attached)

        GameObject gameobjectCustomCanvas = null; // used to hold reference to instance of GameObject with the custom canvas used for drawing the exterior automap        

        int layerAutomap; // layer used for level geometry of automap

        GameObject gameObjectCameraExteriorAutomap = null; // used to hold reference to GameObject to which camera class for automap camera is attached to
        Camera cameraExteriorAutomap = null; // camera for exterior automap camera        
        Quaternion cameraTransformRotationSaved; // camera rotation is saved so that after closing and reopening exterior automap the camera transform settings can be restored
        float cameraOrthographicSizeSaved; // camera's orthographic size is saved so that after closing and reopening exterior automap the camera settings can be restored

        GameObject gameObjectPlayerAdvanced = null; // used to hold reference to instance of GameObject "PlayerAdvanced"

        // exterior automap view mode (controls settings for extra buildings and ground flats)
        public enum ExteriorAutomapViewMode
        {
            Original = 0,
            Extra = 1,
            All = 2
        };

        ExteriorAutomapViewMode currentExteriorAutomapViewMode = ExteriorAutomapViewMode.Original; // currently selected exterior automap view mode     

        // flag that indicates if external script should reset automap settings (set via Property ResetAutomapSettingsSignalForExternalScript checked and erased by DaggerfallExteriorAutomapWindow script)
        // this might look weirds - why not just notify the DaggerfallExteriorAutomapWindow class you may ask... - I wanted to make DaggerfallExteriorAutomap inaware and independent of the actual GUI implementation
        // so communication will always be only from DaggerfallExteriorAutomapWindow to DaggerfallExteriorAutomap class - so into other direction it works in that way that DaggerfallExteriorAutomap will pull
        // from DaggerfallExteriorAutomapWindow via flags - this is why this flag and its Property ResetAutomapSettingsSignalForExternalScript exist
        bool resetAutomapSettingsFromExternalScript = false;

        GameObject gameobjectPlayerMarkerArrow = null; // GameObject which will hold player marker arrow
        GameObject gameobjectPlayerMarkerArrowStamp = null; // GameObject which will hold player marker arrow stamp
        GameObject gameobjectPlayerMarkerCircle = null; // GameObject which will hold player marker circle (actually a cylinder)

        private struct BuildingNameplate
        {
            public float posX;
            public float posY;
            public string name;
            public int uniqueIndex;
            public TextLabel textLabel;
            public GameObject gameObject;
            public float textureWidth;
            public float textureHeight;
            public Vector2 anchorPoint;
            public float scale;
            public Vector2 offsetPlateCoordsInNewBase;
            public Vector2 offsetPlateCoordsInOriginalBase;
            public GameObject anchorLine;
            public float width;
            public float height;
            public float angle;
            public Vector2 upperLeftCorner;
            public Vector2 upperRightCorner;
            public Vector2 lowerLeftCorner;
            public Vector2 lowerRightCorner;
#if DEBUG_NameplateS
            public GameObject debugLine1;
            public GameObject debugLine2;
#endif
        }

        List<BuildingNameplate> buildingNameplates = null;

        GameObject gameObjectBuildingNameplates = null; // parent gameobject for all building name plates 

        private struct Rectangle
        {
            public int xpos;
            public int ypos;
            public int width;
            public int height;
        }

        /// <summary>
        /// Block layout of location.
        /// </summary>
        private struct BlockLayout
        {
            public int x;
            public int y;
            public Rectangle rect;
            public string name;
            public DFBlock.BlockTypes blocktype;
            public DFBlock.RdbTypes rdbType;
        }

        DFLocation location;

        // location dimensions
        int locationWidth;
        int locationHeight;

        const int blockSizeWidth = 64;
        const int blockSizeHeight = 64;

        // layout image dimensions
        int layoutWidth;
        int layoutHeight;

        private const float layoutMultiplier = 1.0f;

        private BlockLayout[] exteriorLayout = null;

        Texture2D exteriorLayoutTexture = null;

        #endregion

        #region Properties

        /// <summary>
        /// DaggerfallExteriorAutomapWindow script will use this to get automap layer
        /// </summary>
        public int LayerAutomap
        {
            get { return (layerAutomap); }
        }

        /// <summary>
        /// DaggerfallExteriorAutomapWindow script will use this to get automap camera
        /// </summary>
        public Camera CameraExteriorAutomap
        {
            get { return (cameraExteriorAutomap); }
        }

        public float LayoutMultiplier
        {
            get { return (layoutMultiplier); }
        }

        public int LocationWidth
        {
            get { return (locationWidth); }
        }

        public int LocationHeight
        {
            get { return (locationHeight); }
        }

        public GameObject GameobjectPlayerMarkerArrow
        {
            get { return (gameobjectPlayerMarkerArrow); }
        }

        /// <summary>
        /// DaggerfallExteriorAutomapWindow script will use this to check if it should reset automap settings (and if it does it will erase flag)
        /// </summary>
        public bool ResetAutomapSettingsSignalForExternalScript
        {
            get { return (resetAutomapSettingsFromExternalScript); }
            set { resetAutomapSettingsFromExternalScript = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// DaggerfallExteriorAutomapWindow script will use this to signal this script to update when automap window was pushed - TODO: check if this can done with an event (if events work with gui windows)
        /// </summary>
        public void updateAutomapStateOnWindowPush()
        {
            // create camera (if not present) that will render automap level geometry
            createExteriorAutomapCamera();

            // restore camera rotation and zoom
            gameObjectCameraExteriorAutomap.transform.rotation = cameraTransformRotationSaved;
            cameraExteriorAutomap.orthographicSize = cameraOrthographicSizeSaved;

            // focus player position
            cameraExteriorAutomap.transform.position = GameobjectPlayerMarkerArrow.transform.position + new Vector3(0.0f, 10.0f, 0.0f);

            // update player marker position and rotation
            updatePlayerMarker();
        }

        /// <summary>
        /// DaggerfallExteriorAutomapWindow script will use this to signal this script to update when automap window was popped - TODO: check if this can done with an event (if events work with gui windows)
        /// </summary>
        public void updateAutomapStateOnWindowPop()
        {
            cameraTransformRotationSaved = gameObjectCameraExteriorAutomap.transform.rotation;
            cameraOrthographicSizeSaved = cameraExteriorAutomap.orthographicSize;

            // destroy the camera so it does not use system resources
            if (gameObjectCameraExteriorAutomap != null)
            {
                UnityEngine.Object.Destroy(gameObjectCameraExteriorAutomap);
            }
        }

        public void switchToNextExteriorAutomapViewMode()
        {
            int numberOfExteriorAutomapViewModes = Enum.GetNames(typeof(ExteriorAutomapViewMode)).Length;
            currentExteriorAutomapViewMode++;
            if ((int)currentExteriorAutomapViewMode > numberOfExteriorAutomapViewModes - 1) // first mode is mode 0 -> so use numberOfExteriorAutomapViewModes-1 for comparison
                currentExteriorAutomapViewMode = 0;
            switch (currentExteriorAutomapViewMode)
            {
                default:
                case ExteriorAutomapViewMode.Original:
                    switchToExteriorAutomapViewModeOriginal();
                    break;
                case ExteriorAutomapViewMode.Extra:
                    switchToExteriorAutomapViewModeExtra();
                    break;
                case ExteriorAutomapViewMode.All:
                    switchToExteriorAutomapViewModeAll();
                    break;
            }
        }

        public void switchToExteriorAutomapViewModeOriginal()
        {
            currentExteriorAutomapViewMode = ExteriorAutomapViewMode.Original;
            createExteriorLayoutTexture(location, false, true, false);
            assignExteriorLayoutTextureToCustomCanvas();
        }

        public void switchToExteriorAutomapViewModeExtra()
        {
            currentExteriorAutomapViewMode = ExteriorAutomapViewMode.Extra;
            createExteriorLayoutTexture(location, true, true, false);
            assignExteriorLayoutTextureToCustomCanvas();
        }

        public void switchToExteriorAutomapViewModeAll()
        {
            currentExteriorAutomapViewMode = ExteriorAutomapViewMode.All;
            createExteriorLayoutTexture(location, true, false, false);
            assignExteriorLayoutTextureToCustomCanvas();
        }

        public enum LocationBorder
        {
            Top = 0,
            Bottom = 1,
            Left = 2,
            Right = 3
        };

        public Vector3 getLocationBorderPos(LocationBorder locationBorder)
        {
            int locationWidth = (int)(layoutWidth * layoutMultiplier);
            int locationHeight = (int)(layoutHeight * layoutMultiplier);
            Vector3 pos = Vector3.zero;
            switch (locationBorder)
            {
                case LocationBorder.Top:
                    pos.z = +locationHeight * 0.5f;
                    break;
                case LocationBorder.Bottom:
                    pos.z = -locationHeight * 0.5f;
                    break;
                case LocationBorder.Left:
                    pos.x = -locationWidth * 0.5f;
                    break;
                case LocationBorder.Right:
                    pos.x = +locationWidth * 0.5f;
                    break;
            }
            return (pos);
        }

        public void rotateBuildingNameplates(float angle)
        {
            undoNameplateOffsets();
            for (int i = 0; i < buildingNameplates.Count; i++)
            {
                BuildingNameplate buildingNameplate = buildingNameplates[i];

                buildingNameplate.gameObject.transform.Rotate(new Vector3(0.0f, angle, 0.0f));

                /*
                buildingNameplate.upperLeftCorner = Quaternion.Euler(0, 0, -angle) * buildingNameplate.upperLeftCorner;
                buildingNameplate.upperRightCorner = Quaternion.Euler(0, 0, -angle) * buildingNameplate.upperRightCorner;
                buildingNameplate.lowerLeftCorner = Quaternion.Euler(0, 0, -angle) * buildingNameplate.lowerLeftCorner;
                buildingNameplate.lowerRightCorner = Quaternion.Euler(0, 0, -angle) * buildingNameplate.lowerRightCorner;
                */

                buildingNameplate.upperLeftCorner = Quaternion.AngleAxis(-angle, Vector3.forward) * buildingNameplate.upperLeftCorner;
                buildingNameplate.upperRightCorner = Quaternion.AngleAxis(-angle, Vector3.forward) * buildingNameplate.upperRightCorner;
                buildingNameplate.lowerLeftCorner = Quaternion.AngleAxis(-angle, Vector3.forward) * buildingNameplate.lowerLeftCorner;
                buildingNameplate.lowerRightCorner = Quaternion.AngleAxis(-angle, Vector3.forward) * buildingNameplate.lowerRightCorner;

                buildingNameplate.angle += angle;

                buildingNameplates[i] = buildingNameplate;
            }
            computeNameplateOffsets();
            applyNameplateOffsets();
        }

        public void resetRotationBuildingNameplates()
        {
            undoNameplateOffsets();
            for (int i = 0; i < buildingNameplates.Count; i++)
            {
                BuildingNameplate buildingNameplate = buildingNameplates[i];
                buildingNameplate.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
                buildingNameplate.angle = 0.0f;
                buildingNameplate.upperLeftCorner = new Vector2(0.0f, +buildingNameplate.height * 0.5f);
                buildingNameplate.upperRightCorner = new Vector2(buildingNameplate.width, +buildingNameplate.height * 0.5f);
                buildingNameplate.lowerLeftCorner = new Vector2(0.0f, -buildingNameplate.height * 0.5f);
                buildingNameplate.lowerRightCorner = new Vector2(buildingNameplate.width, -buildingNameplate.height * 0.5f);
                buildingNameplates[i] = buildingNameplate;
            }
            computeNameplateOffsets();
            applyNameplateOffsets();
        }

        /// <summary>
        /// DaggerfallExteriorAutomapWindow script will use this to signal this script to update when anything changed that requires DaggerfallExteriorAutomap to update - TODO: check if this can done with an event (if events work with gui windows)
        /// </summary>
        public void forceUpdate()
        {
            Update();
        }

        #endregion

        #region Unity

        void Awake()
        {
            gameObjectPlayerAdvanced = GameObject.Find("PlayerAdvanced");
            if (!gameObjectPlayerAdvanced)
            {
                DaggerfallUnity.LogMessage("GameObject \"PlayerAdvanced\" not found! in script DaggerfallExteriorAutomap (in function Awake())", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }

            gameobjectExteriorAutomap = GameObject.Find("Automap/ExteriorAutomap");
            if (gameobjectExteriorAutomap == null)
            {
                DaggerfallUnity.LogMessage("GameObject \"Automap/ExteriorAutomap\" missing! Create a GameObject called \"Automap\" in root of hierarchy and add a GameObject \"ExteriorAutomap\" to it, to this add script Game/DaggerfallExteriorAutomap!\"", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }

            layerAutomap = LayerMask.NameToLayer("ExteriorAutomap");
            if (layerAutomap == -1)
            {
                DaggerfallUnity.LogMessage("Did not find Layer with name \"Automap\"! Defaulting to Layer 10\nIt is prefered that Layer \"Automap\" is set in Unity Editor under \"Edit/Project Settings/Tags and Layers!\"", true);
                layerAutomap = 10;
            }

            Camera.main.cullingMask = Camera.main.cullingMask & ~((1 << layerAutomap)); // don't render automap layer with main camera

            cameraTransformRotationSaved = Quaternion.identity;
            cameraOrthographicSizeSaved = 10.0f; // dummy value > 0.0f -> will be overwritten once camera zoom is applied

            // important that transition events/delegates are created in Awake() instead of OnEnable (since exteriorAutomap gameobject is disabled when going indoors and enabled when going outdoors)
            PlayerGPS.OnMapPixelChanged += OnMapPixelChanged;
            PlayerEnterExit.OnTransitionExterior += OnTransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonExterior += OnTransitionToDungeonExterior;
            PlayerEnterExit.OnTransitionInterior += OnTransitionToInterior;
            PlayerEnterExit.OnTransitionDungeonInterior += OnTransitionToDungeonInterior;
            SaveLoadManager.OnLoad += OnLoadEvent;
        }

        void OnDestroy()
        {
            // important that transition events/delegates are destroyed in OnDestroy() instead of OnDisable (since exteriorAutomap gameobject is disabled when going indoors and enabled when going outdoors)
            PlayerGPS.OnMapPixelChanged -= OnMapPixelChanged;
            PlayerEnterExit.OnTransitionExterior -= OnTransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonExterior -= OnTransitionToDungeonExterior;
            PlayerEnterExit.OnTransitionInterior -= OnTransitionToInterior;
            PlayerEnterExit.OnTransitionDungeonInterior -= OnTransitionToDungeonInterior;
            SaveLoadManager.OnLoad -= OnLoadEvent;
        }

        /*
        void OnEnable()
        {

        }

        void OnDisable()
        {

        }

        void Start()
        {

        }
        */

        void Update()
        {
            //// if we are not in game (e.g. title menu) skip update function (update must not be skipped when in game or in gui window (to propagate all map control changes))
            //if ((GameManager.Instance.StateManager.CurrentState != StateManager.StateTypes.Game) && (GameManager.Instance.StateManager.CurrentState != StateManager.StateTypes.UI))
            //{
            //    return;
            //}
        }

        #endregion

        #region Private Methods

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


        private GameObject DrawLine(Vector3 start, Vector3 end, Color color, float startWidth = 0.3f, float endWidth = 0.3f)
        {
            GameObject line = new GameObject();
            line.layer = layerAutomap;
            line.transform.position = start;
            line.AddComponent<LineRenderer>();
            LineRenderer lr = line.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Unlit/Color"));
            lr.material.color = color;
            lr.SetWidth(startWidth, endWidth);
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            return line;
        }

        /// <summary>
        /// creates the exterior automap camera if not present and sets camera default settings, registers camera to compass
        /// </summary>
        private void createExteriorAutomapCamera()
        {
            if (!cameraExteriorAutomap)
            {
                gameObjectCameraExteriorAutomap = new GameObject("CameraExteriorAutomap");
                cameraExteriorAutomap = gameObjectCameraExteriorAutomap.AddComponent<Camera>();
                cameraExteriorAutomap.clearFlags = CameraClearFlags.SolidColor;
                cameraExteriorAutomap.cullingMask = 1 << layerAutomap;
                cameraExteriorAutomap.renderingPath = Camera.main.renderingPath;
                cameraExteriorAutomap.orthographic = true;
                cameraExteriorAutomap.nearClipPlane = 0.7f;
                cameraExteriorAutomap.farClipPlane = 5000.0f;
                gameObjectCameraExteriorAutomap.transform.SetParent(gameobjectExteriorAutomap.transform);
            }
        }

        private Mesh CreateMesh(float width, float height)
        {
            Mesh m = new Mesh();
            m.name = "ScriptedMesh";
            m.vertices = new Vector3[] {
                new Vector3(-width/2.0f, 0.01f, height/2.0f),
                new Vector3(width/2.0f, 0.01f, height/2.0f),
                new Vector3(width/2.0f, 0.01f, -height/2.0f),
                new Vector3(-width/2.0f, 0.01f, -height/2.0f)
            };
            m.uv = new Vector2[] {
                new Vector2 (0, 1),
                new Vector2 (1, 1),                
                new Vector2(1, 0),
                new Vector2 (0, 0)
            };
            m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
            m.RecalculateNormals();

            return m;
        }

        private Mesh CreateLeftAlignedMesh(float width, float height)
        {
            Mesh m = new Mesh();
            m.name = "ScriptedMesh";
            m.vertices = new Vector3[] {
                new Vector3(0.0f, 0.01f, height/2.0f),
                new Vector3(width, 0.01f, height/2.0f),
                new Vector3(width, 0.01f, -height/2.0f),
                new Vector3(0.0f, 0.01f, -height/2.0f)
            };
            m.uv = new Vector2[] {
                new Vector2 (0, 1),
                new Vector2 (1, 1),                
                new Vector2(1, 0),
                new Vector2 (0, 0)
            };
            m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
            m.RecalculateNormals();

            return m;
        }

        /// <summary>
        /// creates the quad that is used for exterior automap drawing
        /// </summary>        
        private void createCustomCanvasForExteriorAutomap()
        {
            if (gameobjectCustomCanvas != null)
            {
                UnityEngine.Object.Destroy(gameobjectCustomCanvas);
                gameobjectCustomCanvas = null;
            }

            gameobjectCustomCanvas = new GameObject("CustomCanvasExteriorAutomap");
            //gameobjectCustomCanvas.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
            MeshFilter meshFilter = (MeshFilter)gameobjectCustomCanvas.AddComponent(typeof(MeshFilter));
            meshFilter.mesh = CreateMesh(layoutWidth * layoutMultiplier, layoutHeight * layoutMultiplier); // create quad with normal facing into positive y-direction
            MeshRenderer renderer = gameobjectCustomCanvas.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

            renderer.material.shader = Shader.Find("Unlit/Transparent");
            //Texture2D tex = new Texture2D(1, 1);
            //tex.SetPixel(0, 0, Color.magenta);
            //tex.Apply();
            renderer.material.mainTexture = exteriorLayoutTexture;
            //renderer.material.color = Color.green;
            renderer.enabled = true;

            SetLayerRecursively(gameobjectCustomCanvas, layerAutomap);
            gameobjectCustomCanvas.transform.SetParent(gameobjectExteriorAutomap.transform);
        }

        private void assignExteriorLayoutTextureToCustomCanvas()
        {
            if (gameobjectCustomCanvas != null)
            {
                MeshRenderer renderer = gameobjectCustomCanvas.GetComponent<MeshRenderer>();
                renderer.material.mainTexture = exteriorLayoutTexture;
            }
        }

        private void createBuildingNameplates(DFLocation location)
        {
            deleteBuildingNameplates();

            buildingNameplates = new List<BuildingNameplate>();

            gameObjectBuildingNameplates = new GameObject("building name plates");
            gameObjectBuildingNameplates.transform.SetParent(gameobjectExteriorAutomap.transform);

            //foreach (var layout in exteriorLayout)
            //{
            //    DFBlock block = DaggerfallUnity.Instance.ContentReader.BlockFileReader.GetBlock(layout.name);
            //}

            DFBlock[] blocks;
            RMBLayout.GetLocationBuildingData(location, out blocks);
            int width = location.Exterior.ExteriorData.Width;
            int height = location.Exterior.ExteriorData.Height;
            int uniqueIndex = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    RMBLayout.BuildingSummary[] buildingsInBlock = RMBLayout.GetBuildingData(blocks[index]);
                    BlockLayout layout = exteriorLayout[index];

                    foreach (RMBLayout.BuildingSummary buildingSummary in buildingsInBlock)
                    {
                        //Debug.Log(String.Format("x: {0}, y: {1}", buildingSummary.Position.x, buildingSummary.Position.z));
                        int xPosBuilding = layout.rect.xpos + (int)(buildingSummary.Position.x / (BlocksFile.RMBDimension * MeshReader.GlobalScale) * 64.0f);
                        int yPosBuilding = layout.rect.ypos + (int)(buildingSummary.Position.z / (BlocksFile.RMBDimension * MeshReader.GlobalScale) * 64.0f);

                        BuildingNameplate newBuildingNameplate;
                        try
                        {
                            newBuildingNameplate.name = BuildingNames.GetName(buildingSummary.NameSeed, buildingSummary.BuildingType, buildingSummary.FactionId, location.Name, location.RegionName);
                        }
                        catch (Exception e)
                        {
                            string exceptionMessage = String.Format("exception occured in function BuildingNames.GetName (exception message: " + e.Message + @") with params: 
                                                                        seed: {0}, type: {1}, factionID: {2}, locationName: {3}, regionName: {4}",
                                                                        buildingSummary.NameSeed, buildingSummary.BuildingType, buildingSummary.FactionId, location.Name, location.RegionName);
                            DaggerfallUnity.LogMessage(exceptionMessage, true);

                            //fallback
                            switch (buildingSummary.BuildingType)
                            {
                                case DFLocation.BuildingTypes.Alchemist:
                                    newBuildingNameplate.name = "Alchemist";
                                    break;
                                case DFLocation.BuildingTypes.Armorer:
                                    newBuildingNameplate.name = "Armorer";
                                    break;
                                case DFLocation.BuildingTypes.Bank:
                                    newBuildingNameplate.name = "Bank";
                                    break;
                                case DFLocation.BuildingTypes.Bookseller:
                                    newBuildingNameplate.name = "Bookseller";
                                    break;
                                case DFLocation.BuildingTypes.ClothingStore:
                                    newBuildingNameplate.name = "Clothing Store";
                                    break;
                                case DFLocation.BuildingTypes.FurnitureStore:
                                    newBuildingNameplate.name = "Furniture Store";
                                    break;
                                case DFLocation.BuildingTypes.GemStore:
                                    newBuildingNameplate.name = "Gem Store";
                                    break;
                                case DFLocation.BuildingTypes.GeneralStore:
                                    newBuildingNameplate.name = "General Store";
                                    break;
                                case DFLocation.BuildingTypes.GuildHall:
                                    newBuildingNameplate.name = "Guild Hall";
                                    break;
                                case DFLocation.BuildingTypes.HouseForSale:
                                    newBuildingNameplate.name = "House for Sale";
                                    break;
                                case DFLocation.BuildingTypes.Library:
                                    newBuildingNameplate.name = "Library";
                                    break;
                                case DFLocation.BuildingTypes.Palace:
                                    newBuildingNameplate.name = "Palace";
                                    break;
                                case DFLocation.BuildingTypes.PawnShop:
                                    newBuildingNameplate.name = "Pawn Shop";
                                    break;
                                case DFLocation.BuildingTypes.Tavern:
                                    newBuildingNameplate.name = "Tavern";
                                    break;
                                case DFLocation.BuildingTypes.Temple:
                                    newBuildingNameplate.name = "Temple";
                                    break;
                                case DFLocation.BuildingTypes.WeaponSmith:
                                    newBuildingNameplate.name = "Weapon Smith";
                                    break;
                                default:
                                    newBuildingNameplate.name = "unknown";
                                    break;
                            }
                        }

                        if (newBuildingNameplate.name != "")
                        {
                            newBuildingNameplate.uniqueIndex = uniqueIndex++;
                            newBuildingNameplate.posX = xPosBuilding;
                            newBuildingNameplate.posY = yPosBuilding;
                            newBuildingNameplate.textLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, Vector2.zero, newBuildingNameplate.name);
                            newBuildingNameplate.textLabel.TextColor = Color.yellow;
                            newBuildingNameplate.gameObject = new GameObject(String.Format("building name plate for [{0}]", newBuildingNameplate.name));
                            MeshFilter meshFilter = (MeshFilter)newBuildingNameplate.gameObject.AddComponent(typeof(MeshFilter));
                            newBuildingNameplate.textureWidth = newBuildingNameplate.textLabel.Texture.width;
                            newBuildingNameplate.textureHeight = newBuildingNameplate.textLabel.Texture.height;
                            meshFilter.mesh = CreateLeftAlignedMesh(newBuildingNameplate.textureWidth, newBuildingNameplate.textureHeight); // create left aligned (in relation to gameobject position) quad with normal facing into positive y-direction
                            MeshRenderer renderer = newBuildingNameplate.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

                            renderer.material.shader = Shader.Find("Unlit/Transparent");
                            renderer.material.mainTexture = newBuildingNameplate.textLabel.Texture;
                            renderer.enabled = true;

                            SetLayerRecursively(newBuildingNameplate.gameObject, layerAutomap);
                            newBuildingNameplate.gameObject.transform.SetParent(gameObjectBuildingNameplates.transform);

                            float posX = newBuildingNameplate.posX - locationWidth * blockSizeWidth * 0.5f;
                            float posY = newBuildingNameplate.posY - locationHeight * blockSizeHeight * 0.5f;
                            newBuildingNameplate.anchorPoint = new Vector2(posX, posY);
                            newBuildingNameplate.scale = 0.5f;
                            newBuildingNameplate.width = newBuildingNameplate.textureWidth * newBuildingNameplate.scale;
                            newBuildingNameplate.height = newBuildingNameplate.textureHeight * newBuildingNameplate.scale;
                            newBuildingNameplate.gameObject.transform.position = new Vector3(posX, 4.0f, posY);
                            newBuildingNameplate.gameObject.transform.localScale = new Vector3(newBuildingNameplate.scale, newBuildingNameplate.scale, newBuildingNameplate.scale);
                            newBuildingNameplate.offsetPlateCoordsInNewBase = Vector2.zero;
                            newBuildingNameplate.offsetPlateCoordsInOriginalBase = Vector2.zero;
                            newBuildingNameplate.angle = 0.0f;
                            newBuildingNameplate.upperLeftCorner = new Vector2(0.0f, +newBuildingNameplate.height * 0.5f);
                            newBuildingNameplate.upperRightCorner = new Vector2(newBuildingNameplate.width, +newBuildingNameplate.height * 0.5f);
                            newBuildingNameplate.lowerLeftCorner = new Vector2(0.0f, -newBuildingNameplate.height * 0.5f);
                            newBuildingNameplate.lowerRightCorner = new Vector2(newBuildingNameplate.width, -newBuildingNameplate.height * 0.5f);

                            newBuildingNameplate.anchorLine = null;

                            #if DEBUG_NameplateS
                            newBuildingNameplate.debugLine1 = null;
                            newBuildingNameplate.debugLine2 = null;
                            #endif

                            buildingNameplates.Add(newBuildingNameplate);
                        }
                    }
                }
            }
            computeNameplateOffsets();
            applyNameplateOffsets();
        }

        private void deleteBuildingNameplates()
        {
            if (buildingNameplates != null)
            {
                foreach (BuildingNameplate n in buildingNameplates)
                {
                    if (n.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(n.gameObject);
                    }
                }
                buildingNameplates.Clear();
                buildingNameplates = null;
            }

            if (gameObjectBuildingNameplates != null)
            {
                UnityEngine.Object.Destroy(gameObjectBuildingNameplates);
                gameObjectBuildingNameplates = null;
            }
        }
        /*
        private bool checkIntersectionOfLineSegments(Vector2 startLine1, Vector2 endLine1, Vector2 startLine2, Vector2 endLine2, out Vector2? outIntersectionPoint)
        {
            float m = (endLine1.y - startLine1.y) / (endLine1.x - startLine1.x);
            float b = endLine1.y - m * endLine1.x;
            Vector3 line1 = new Vector3(m, 0, b);
            m = (endLine2.y - startLine2.y) / (endLine2.x - startLine2.x);
            b = endLine2.y - m * endLine2.x;
            Vector3 line2 = new Vector3(m, 0, b);
            Vector3 intersectionPointHomogenous = Vector3.Cross(line1, line2); // intersection in homogenous coordinates
            if (intersectionPointHomogenous.z != 0.0f) // only proceed if lines are not parallel (its intersection point is not a point at infinity)
            {
                Vector2 intersectionPoint = new Vector2(intersectionPointHomogenous.x / intersectionPointHomogenous.z, intersectionPointHomogenous.y / intersectionPointHomogenous.z);
                outIntersectionPoint = intersectionPoint;
                float sx = 0;
                float sy = 0;
                if (endLine1.x - startLine1.x != 0)
                    sx = (intersectionPoint.x - startLine1.x) / (endLine1.x - startLine1.x);
                if (endLine1.y - startLine1.y != 0)
                    sy = (intersectionPoint.y - startLine1.y) / (endLine1.y - startLine1.y);
                float s = Math.Max(sx, sy);
                if ((s < 0.0f) || (s > 1.0f))
                    return false; // not on first line segment

                sx = 0;
                sy = 0;
                if (endLine2.x - startLine2.x != 0)
                    sx = (intersectionPoint.x - startLine2.x) / (endLine2.x - startLine2.x);
                if (endLine2.y - startLine2.y != 0)
                    sy = (intersectionPoint.y - startLine2.y) / (endLine2.y - startLine2.y);
                s = Math.Max(sx, sy);
                if ((s < 0.0f) || (s > 1.0f))
                    return false; // not on second line segment

                return true; // intersection is on both line segments
            }
            outIntersectionPoint = null;
            return false;
        }
        */

        private bool checkIntersectionOfNameplates(BuildingNameplate nameplate1, Vector2 offset1, BuildingNameplate nameplate2, Vector2 offset2, out Vector2 posNameplate1, out Vector2 posNameplate2, out Vector2 p, out float ySize, out float distanceVertical)
        {
            Vector2 vectorBetweenNamePlates = (new Vector2(nameplate2.gameObject.transform.position.x, nameplate2.gameObject.transform.position.z) + nameplate2.offsetPlateCoordsInOriginalBase + offset2) - (new Vector2(nameplate1.gameObject.transform.position.x, nameplate1.gameObject.transform.position.z) + nameplate1.offsetPlateCoordsInOriginalBase + offset1);
            posNameplate1 = Vector2.zero;
            posNameplate2 = vectorBetweenNamePlates;

            Vector2 b = posNameplate1 + (nameplate1.upperRightCorner - nameplate1.upperLeftCorner);
            b.Normalize();
            Vector2 a = posNameplate2 - posNameplate1;
            float a1 = Vector2.Dot(a, b); // length of projected vector a onto b
            p = posNameplate1 + b * a1;
            distanceVertical = Vector2.Distance(posNameplate2, p);
            
            float xSize = Vector2.Distance(nameplate1.upperRightCorner, nameplate1.upperLeftCorner); // assume that first nameplate is "left" of second (in terms of the rotated coordinate system
            // test if second nameplate is "left" and our initial assumption is false
            Vector2 pointRightNamePlate1 = (nameplate1.upperRightCorner + nameplate1.lowerRightCorner) * 0.5f;
            Vector2 u = pointRightNamePlate1 /* - Vector2.zero */; // direction vector of line
            float s = (p.x - 0.0f) / u.x;
            if (s < 0)
            {
                xSize = Vector2.Distance(nameplate2.upperRightCorner, nameplate2.upperLeftCorner);
            }

            ySize = Vector2.Distance(nameplate1.lowerLeftCorner, nameplate1.upperLeftCorner) * 0.5f + Vector2.Distance(nameplate2.lowerLeftCorner, nameplate2.upperLeftCorner) * 0.5f;

            // test if (rotated) nameplates intersect
            bool intersect = distanceVertical < ySize;
            float distanceHorizontal = Vector2.Distance(posNameplate1, p);
            //float sign = ((posNamePlate1 - p).y > 0.0f) ? -1.0f : 1.0f;
            intersect &= distanceHorizontal < xSize;

            return intersect;
        }

        private bool checkIntersectionOfNameplates(BuildingNameplate nameplate1, Vector2 offset1, BuildingNameplate nameplate2, Vector2 offset2)
        {
            Vector2 p, posNameplate1, posNameplate2;
            float ySize, distanceVertical;
            return (checkIntersectionOfNameplates(nameplate1, offset1, nameplate2, offset2, out posNameplate1, out posNameplate2, out p, out ySize, out distanceVertical));
        }
        private bool checkIntersectionOffsetNameplateAgainstOthers(BuildingNameplate nameplate, Vector2 offset, BuildingNameplate? skipNameplate = null)
        {
            bool check = false;
            for (int i=0; i < buildingNameplates.Count; i++)
            {
                BuildingNameplate otherNameplate = buildingNameplates[i];
                if ((skipNameplate.HasValue) && (skipNameplate.Value.uniqueIndex == otherNameplate.uniqueIndex))
                    continue;
                if (nameplate.uniqueIndex == otherNameplate.uniqueIndex)
                    continue;
                check |= checkIntersectionOfNameplates(nameplate, offset, otherNameplate, Vector2.zero);
                if (check)
                    break;                
            }
            return check;
        }

        private void computeNameplateOffsets()
        {
            for (int i = 0; i < buildingNameplates.Count; i++)
            {
                BuildingNameplate first = buildingNameplates[i];
                for (int j = i + 1; j < buildingNameplates.Count; j++)
                {
                    BuildingNameplate second = buildingNameplates[j];

                    Vector2 p, posNameplate1, posNameplate2;
                    float ySize;
                    float distanceVertical;
                    bool intersect = checkIntersectionOfNameplates(first, Vector2.zero, second, Vector2.zero, out posNameplate1, out posNameplate2, out p, out ySize, out distanceVertical);

                    if (!intersect)
                        continue;

                    string firstName = "The Odd Bijoutry"; // "The Red Porcupine"; // "The King's Fairy"; // "The Odd Blades"
                    string secondName = "The Emperor's Jewelers"; // "Daggerfall's Best Tailoring"; // "The White Muskrat"; // "The Lucky Wolf"
                    if (((first.name == firstName) && (second.name == secondName)) || ((first.name == secondName) && (second.name == firstName)))
                    {
                        bool test = false;
                    }
             
                    if (Math.Abs(distanceVertical) < ySize)
                    {
                        Vector2 halfPoint = (posNameplate2 + p) * 0.5f; // point lying halfway between centerNameplate2 and p
                        Vector2 vectorDirFirstNameplate = (p - halfPoint).normalized;
                        Vector2 vectorDirSecondNameplate = (posNameplate2 - halfPoint).normalized;
                        if (vectorDirFirstNameplate == Vector2.zero)
                        {
                            vectorDirFirstNameplate = Vector2.up;
                        }
                        if (vectorDirSecondNameplate == Vector2.zero)
                        {
                            vectorDirSecondNameplate = Vector2.down;
                        }
                        Vector2 vectorBiasFirstNameplate = vectorDirFirstNameplate * (ySize - Math.Abs(distanceVertical)) * 0.5f;
                        Vector2 vectorBiasSecondNameplate = vectorDirSecondNameplate * (ySize - Math.Abs(distanceVertical)) * 0.5f;

                        Vector2 vectorBiasFirstNameplateInOriginalBase = Quaternion.AngleAxis(first.angle, Vector3.forward) * vectorBiasFirstNameplate;
                        Vector2 vectorBiasSecondNameplateInOriginalBase = Quaternion.AngleAxis(second.angle, Vector3.forward) * vectorBiasSecondNameplate;

                        string stringNameplate1 = "";
                        string stringNameplate2 = "";
                        if ((first.offsetPlateCoordsInOriginalBase == Vector2.zero) && (second.offsetPlateCoordsInOriginalBase == Vector2.zero))
                        {
                            if (!checkIntersectionOffsetNameplateAgainstOthers(first, vectorBiasFirstNameplateInOriginalBase, second) && !checkIntersectionOffsetNameplateAgainstOthers(second, vectorBiasSecondNameplateInOriginalBase, first))
                            {
                                first.offsetPlateCoordsInOriginalBase += vectorBiasFirstNameplateInOriginalBase;
                                first.offsetPlateCoordsInNewBase += vectorBiasFirstNameplate;
                                stringNameplate1 = String.Format("{0} {1}", first.name, "^");

                                second.offsetPlateCoordsInOriginalBase += vectorBiasSecondNameplateInOriginalBase;
                                second.offsetPlateCoordsInNewBase += vectorBiasSecondNameplate;
                                stringNameplate2 = String.Format("{0} {1}", second.name, "v");
                            }
                            else if (!checkIntersectionOffsetNameplateAgainstOthers(first, vectorBiasFirstNameplateInOriginalBase * 2.0f))
                            {
                                first.offsetPlateCoordsInOriginalBase += vectorBiasFirstNameplateInOriginalBase * 2.0f;
                                first.offsetPlateCoordsInNewBase += vectorBiasFirstNameplate * 2.0f;
                                stringNameplate1 = String.Format("{0} {1}", first.name, "_^");
                            }
                            else if (!checkIntersectionOffsetNameplateAgainstOthers(second, vectorBiasSecondNameplateInOriginalBase * 2.0f))
                            {
                                second.offsetPlateCoordsInOriginalBase += vectorBiasSecondNameplateInOriginalBase * 2.0f;
                                second.offsetPlateCoordsInNewBase += vectorBiasSecondNameplate * 2.0f;
                                stringNameplate2 = String.Format("{0} {1}", second.name, "_v");
                            }
                            else
                            {
                                string abbreviation = second.name.Substring(0, 1) + ".";
                                for (int c=0; c < second.name.Length - 2; c++)
                                    abbreviation += " ";
                                stringNameplate2 = abbreviation;
                                //second.gameObject.SetActive(false);
                            }
                        }
                        else if ((first.offsetPlateCoordsInOriginalBase != Vector2.zero) && (second.offsetPlateCoordsInOriginalBase == Vector2.zero))
                        {
                            if (!checkIntersectionOffsetNameplateAgainstOthers(second, vectorBiasSecondNameplateInOriginalBase * 2.0f))
                            {
                                second.offsetPlateCoordsInOriginalBase += vectorBiasSecondNameplateInOriginalBase * -2.0f;
                                second.offsetPlateCoordsInNewBase += vectorBiasSecondNameplate * -2.0f;
                                stringNameplate2 = String.Format("{0} {1}", second.name, "vv");
                            }
                            else if (!checkIntersectionOffsetNameplateAgainstOthers(second, vectorBiasSecondNameplateInOriginalBase * -2.0f))
                            {
                                second.offsetPlateCoordsInOriginalBase += vectorBiasSecondNameplateInOriginalBase * -2.0f;
                                second.offsetPlateCoordsInNewBase += vectorBiasSecondNameplate * -2.0f;
                                stringNameplate2 = String.Format("{0} {1}", second.name, "^^");
                            }
                            else
                            {
                                second.gameObject.SetActive(false);
                            }
                        }
                        else if ((first.offsetPlateCoordsInOriginalBase == Vector2.zero) && (second.offsetPlateCoordsInOriginalBase != Vector2.zero))
                        {
                            if (!checkIntersectionOffsetNameplateAgainstOthers(first, vectorBiasFirstNameplateInOriginalBase * 2.0f))
                            {
                                first.offsetPlateCoordsInOriginalBase += vectorBiasFirstNameplateInOriginalBase * 2.0f;
                                first.offsetPlateCoordsInNewBase += vectorBiasFirstNameplate * 2.0f;
                                stringNameplate1 = String.Format("{0} {1}", first.name, "^^");
                            } else if (!checkIntersectionOffsetNameplateAgainstOthers(first, vectorBiasFirstNameplateInOriginalBase * -2.0f))
                            {
                                first.offsetPlateCoordsInOriginalBase += vectorBiasFirstNameplateInOriginalBase * -2.0f;
                                first.offsetPlateCoordsInNewBase += vectorBiasFirstNameplate * -2.0f;
                                stringNameplate1 = String.Format("{0} {1}", first.name, "vv");
                            }
                            else
                            {
                                first.gameObject.SetActive(false);
                            }
                        }

                        if (stringNameplate1 != "")
                        {
                            TextLabel newTextLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, Vector2.zero, stringNameplate1);
                            first.textLabel = newTextLabel;
                            MeshRenderer renderer = first.gameObject.GetComponent<MeshRenderer>();
                            renderer.material.mainTexture = newTextLabel.Texture;
                            //buildingNameplates[i] = first;
                        }
                        if (stringNameplate2 != "")
                        {
                            TextLabel newTextLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, Vector2.zero, stringNameplate2);
                            second.textLabel = newTextLabel;
                            MeshRenderer renderer = second.gameObject.GetComponent<MeshRenderer>();
                            renderer.material.mainTexture = newTextLabel.Texture;
                            //buildingNameplates[j] = second;
                        }
                        buildingNameplates[i] = first;
                        buildingNameplates[j] = second;
                    }
                }
            }
        }

        private void applyNameplateOffsets()
        {
            for (int i = 0; i < buildingNameplates.Count; i++)
            {
                BuildingNameplate buildingNameplate = buildingNameplates[i];

                //buildingNameplate.offsetPlateCoordsInOriginalBase = Quaternion.AngleAxis(buildingNameplate.angle, Vector3.forward) * buildingNameplate.offsetPlateCoordsInOriginalBase;
                //buildingNameplate.gameObject.transform.localPosition += new Vector3(buildingNameplate.offsetPlateCoordsInOriginalBase.x, 0.0f, buildingNameplate.offsetPlateCoordsInOriginalBase.y);
                buildingNameplate.gameObject.transform.Translate(buildingNameplate.offsetPlateCoordsInOriginalBase.x, 0.0f, buildingNameplate.offsetPlateCoordsInOriginalBase.y, Space.Self);

#if DEBUG_NameplateS
                Vector3 posAnchor = buildingNameplate.gameObject.transform.position;
                Vector3 posNameplate = buildingNameplate.gameObject.transform.position + new Vector3(buildingNameplate.offsetPlateCoordsInNewBase.x, 0.0f, buildingNameplate.offsetPlateCoordsInNewBase.y);
                if (buildingNameplate.anchorLine != null)
                {
                    buildingNameplate.anchorLine.SetActive(false); // hide old line gameobject immediately (since destroy seems to be delayed this is necessary)                    
                    GameObject.Destroy(buildingNameplate.anchorLine);
                    buildingNameplate.anchorLine = null;
                }
                buildingNameplate.anchorLine = DrawLine(posAnchor, posNameplate, Color.yellow, 0.5f, 0.5f);
                buildingNameplate.anchorLine.hideFlags = HideFlags.HideAndDontSave;

                buildingNameplates[i] = buildingNameplate;

                Vector3 start1 = buildingNameplate.gameObject.transform.position + new Vector3(buildingNameplate.upperLeftCorner.x, 0.5f, buildingNameplate.upperLeftCorner.y);
                Vector3 end1 = buildingNameplate.gameObject.transform.position + new Vector3(buildingNameplate.lowerRightCorner.x, 0.5f, buildingNameplate.lowerRightCorner.y);
                Vector3 start2 = buildingNameplate.gameObject.transform.position + new Vector3(buildingNameplate.lowerLeftCorner.x, 0.5f, buildingNameplate.lowerLeftCorner.y);
                Vector3 end2 = buildingNameplate.gameObject.transform.position + new Vector3(buildingNameplate.upperRightCorner.x, 0.5f, buildingNameplate.upperRightCorner.y);

                if (buildingNameplate.debugLine1 != null)
                {
                    buildingNameplate.debugLine1.SetActive(false); // hide old line gameobject immediately (since destroy seems to be delayed this is necessary)                    
                    GameObject.Destroy(buildingNameplate.debugLine1);
                    buildingNameplate.debugLine1 = null;
                }
                if (buildingNameplate.debugLine2 != null)
                {
                    buildingNameplate.debugLine2.SetActive(false); // hide old line gameobject immediately (since destroy seems to be delayed this is necessary)                    
                    GameObject.Destroy(buildingNameplate.debugLine2);
                    buildingNameplate.debugLine2 = null;
                }

                buildingNameplate.debugLine1 = DrawLine(start1, end1, Color.blue);
                buildingNameplate.debugLine2 = DrawLine(start2, end2, Color.red);
                buildingNameplate.debugLine1.hideFlags = HideFlags.HideAndDontSave;
                buildingNameplate.debugLine2.hideFlags = HideFlags.HideAndDontSave;

                buildingNameplates[i] = buildingNameplate;
#endif
            }
        }

        private void undoNameplateOffsets()
        {
            for (int i = 0; i < buildingNameplates.Count; i++)
            {
                BuildingNameplate buildingNameplate = buildingNameplates[i];
                //buildingNameplate.gameObject.transform.localPosition += new Vector3(-buildingNameplate.offsetPlateCoordsInOriginalBase.x, 0.0f, -buildingNameplate.offsetPlateCoordsInOriginalBase.y);
                buildingNameplate.gameObject.transform.Translate(-buildingNameplate.offsetPlateCoordsInOriginalBase.x, 0.0f, -buildingNameplate.offsetPlateCoordsInOriginalBase.y, Space.Self);
                buildingNameplate.offsetPlateCoordsInNewBase = Vector2.zero;
                buildingNameplate.offsetPlateCoordsInOriginalBase = Vector2.zero;
                buildingNameplate.gameObject.SetActive(true);

                TextLabel newTextLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, Vector2.zero, buildingNameplate.name);
                buildingNameplate.textLabel = newTextLabel;
                MeshRenderer renderer = buildingNameplate.gameObject.GetComponent<MeshRenderer>();
                renderer.material.mainTexture = newTextLabel.Texture;

                buildingNameplates[i] = buildingNameplate;
            }
        }

        private void updatePlayerMarker()
        {
            // place player marker            
            Vector3 playerPos;
            float scale = MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale;
            playerPos.x = ((GameManager.Instance.PlayerGPS.transform.position.x) % scale) / scale;
            playerPos.z = ((GameManager.Instance.PlayerGPS.transform.position.z) % scale) / scale;
            playerPos.y = 0.0f;

            int refWidth = (int)(blockSizeWidth * 8 * layoutMultiplier); // layoutWidth / layoutMultiplier
            int refHeight = (int)(blockSizeHeight * 8 * layoutMultiplier); // layoutHeight / layoutMultiplier
            playerPos.x *= refWidth;
            playerPos.y = 0.1f;
            playerPos.z *= refHeight;
            playerPos.x -= refWidth * 0.5f;
            playerPos.z -= refHeight * 0.5f;
            gameobjectPlayerMarkerArrow.transform.position = playerPos;
            gameobjectPlayerMarkerArrow.transform.rotation = gameObjectPlayerAdvanced.transform.rotation;

            // place player marker stamp
            Vector3 newPos = gameobjectPlayerMarkerArrow.transform.position;
            newPos.y = -10.0f;
            Vector3 biasVec = -Vector3.Normalize(gameObjectPlayerAdvanced.transform.forward);
            gameobjectPlayerMarkerArrowStamp.transform.position = newPos + biasVec * 0.8f;
            gameobjectPlayerMarkerArrowStamp.transform.rotation = gameobjectPlayerMarkerArrow.transform.rotation;

            newPos.y = -20.0f;
            gameobjectPlayerMarkerCircle.transform.position = newPos;
            gameobjectPlayerMarkerCircle.transform.rotation = gameobjectPlayerMarkerArrow.transform.rotation;
        }

        /// <summary>
        /// creates the map layout in the exterior layout texture
        /// </summary>  
        private void createExteriorLayoutTexture(DFLocation location, bool showAll = false, bool removeGroundFlats = true, bool createNameplates = true)
        {
            if (exteriorLayoutTexture != null)
            {
                UnityEngine.Object.Destroy(exteriorLayoutTexture);
                exteriorLayoutTexture = null;
            }

            int xpos = 0;
            int ypos = 0; //locationHeight * blockSizeHeight - blockSizeHeight;
            exteriorLayout = new BlockLayout[locationWidth * locationHeight];

            for (int y = 0; y < locationHeight; y++)
            {
                for (int x = 0; x < locationWidth; x++)
                {
                    // Get the block name
                    string blockName = DaggerfallUnity.Instance.ContentReader.BlockFileReader.CheckName(DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRmbBlockName(ref location, x, y));

                    // Get the block data
                    //                    DFBlock block = DaggerfallUnity.Instance.ContentReader.BlockFileReader.GetBlock(blockName);

                    // Now we can get the automap image data for this block and lay it out
                    //block.RmbBlock.SubRecords.

                    int index = y * locationWidth + x;
                    exteriorLayout[index].x = x;
                    exteriorLayout[index].y = y;
                    exteriorLayout[index].rect = new Rectangle();
                    exteriorLayout[index].rect.xpos = xpos;
                    exteriorLayout[index].rect.ypos = ypos;
                    exteriorLayout[index].rect.width = blockSizeWidth;
                    exteriorLayout[index].rect.height = blockSizeHeight;
                    exteriorLayout[index].name = blockName;
                    exteriorLayout[index].blocktype = DFBlock.BlockTypes.Rmb;
                    exteriorLayout[index].rdbType = DFBlock.RdbTypes.Unknown;
                    xpos += blockSizeWidth;
                }
                ypos += blockSizeHeight;
                xpos = 0;
            }

            // Create layout image (texture)
            exteriorLayoutTexture = new Texture2D(layoutWidth, layoutHeight, TextureFormat.ARGB32, false);
            exteriorLayoutTexture.filterMode = FilterMode.Point;

            // Render map layout
            foreach (var layout in exteriorLayout)
            {
                DFBlock block = DaggerfallUnity.Instance.ContentReader.BlockFileReader.GetBlock(layout.name);
                DaggerfallUnity.Instance.ContentReader.BlockFileReader.LoadBlock(block.Index);
                // Get block automap image
                DFBitmap dfBitmap = DaggerfallUnity.Instance.ContentReader.BlockFileReader.GetBlockAutoMap(layout.name, removeGroundFlats);

                int size = blockSizeWidth * blockSizeHeight;
                Color32[] colors = new Color32[size];
                // for (int i = 0; i < size; i++)
                // {
                for (int y = 0; y < blockSizeHeight; y++)
                {
                    for (int x = 0; x < blockSizeWidth; x++)
                    {
                        int i = y * blockSizeWidth + x;
                        int o = (blockSizeHeight - 1 - y) * blockSizeWidth + x;
                        switch (dfBitmap.Data[i])
                        {
                            // guilds
                            case 12: // guildhall
                            case 15: // temple
                                colors[o].r = 69;
                                colors[o].g = 125;
                                colors[o].b = 195;
                                colors[o].a = 255;
                                break;
                            // shops
                            case 1: // alchemist
                            case 3: // armorer
                            case 4: // bank
                            case 6: // bookseller
                            case 7: // clothing store
                            case 9: // gem store
                            case 10: // general store
                            case 11: // library
                            case 13: // pawn shop
                            case 14: // weapon smith
                                colors[o].r = 190;
                                colors[o].g = 85;
                                colors[o].b = 24;
                                colors[o].a = 255;
                                break;
                            case 16: // tavern
                                colors[o].r = 85;
                                colors[o].g = 117;
                                colors[o].b = 48;
                                colors[o].a = 255;
                                break;
                            // common
                            case 2: // house for sale
                            case 5: // town4
                            case 8: // furniture store
                            case 17: // palace
                            case 18: // house 1
                            case 19: // house 2
                            case 20: // house 3
                            case 21: // house 4
                            case 22: // house 5 (hedge)
                            case 23: // house 6
                            case 24: // town23
                                colors[o].r = 69;
                                colors[o].g = 60;
                                colors[o].b = 40;
                                colors[o].a = 255;
                                break;
                            case 25: // ship
                            case 117: // special 1
                            case 224: // special 2
                            case 250: // special 3
                            case 251: // special 4
                                if (showAll)
                                {
                                    colors[o].r = 69;
                                    colors[o].g = 60;
                                    colors[o].b = 40;
                                    colors[o].a = 255;
                                }
                                break;
                            case 0:
                                colors[o].r = 0;
                                colors[o].g = 0;
                                colors[o].b = 0;
                                colors[o].a = 0;
                                break;
                            default: // unknown
                                colors[o].r = 255;
                                colors[o].g = 0;
                                colors[o].b = dfBitmap.Data[i];
                                colors[o].a = 255;
                                break;
                        }
                    }
                }

                exteriorLayoutTexture.SetPixels32(layout.rect.xpos, layout.rect.ypos, layout.rect.width, layout.rect.height, colors);

                //RMBLayout.BuildingSummary[] buildingsInBlock = RMBLayout.GetBuildingData(block);
                //foreach (RMBLayout.BuildingSummary buildingSummary in buildingsInBlock)
                //{
                //    //Debug.Log(String.Format("x: {0}, y: {1}", buildingSummary.Position.x, buildingSummary.Position.z));
                //    int xPosBuilding = layout.rect.xpos + (int)(buildingSummary.Position.x / (BlocksFile.RMBDimension * MeshReader.GlobalScale) * 64.0f);
                //    int yPosBuilding  = layout.rect.ypos + (int)(buildingSummary.Position.z / (BlocksFile.RMBDimension * MeshReader.GlobalScale) * 64.0f);
                //    //exteriorLayoutTexture.SetPixel(xPosBuilding, yPosBuilding, Color.yellow);
                //}

                exteriorLayoutTexture.Apply();

                DaggerfallUnity.Instance.ContentReader.BlockFileReader.DiscardBlock(block.Index);
            }

            if (createNameplates)
            {
                createBuildingNameplates(location);
            }
        }

        private void loadAndCreateLocationExteriorAutomap()
        {
            ContentReader.MapSummary mapSummary;
            DFPosition mapPixel = GameManager.Instance.PlayerGPS.CurrentMapPixel;
            if (!DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out mapSummary))
            {
                // no location found
                return; // do nothing
            }

            DFLocation currentPlayerLocation = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(mapSummary.RegionIndex, mapSummary.MapIndex);
            if (!currentPlayerLocation.Loaded)
            {
                // Location not loaded, something went wrong
                DaggerfallUnity.LogMessage("error when loading location for exterior automap layouting", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }

            if ((location.Loaded) && (currentPlayerLocation.Name == location.Name)) // if already loaded
            {
                return; // do nothing
            }

            unloadLocationExteriorAutomap(); // first make sure to unload location exterior automap and destroy resources

            location = currentPlayerLocation; // set current location as new location

            // and now layout it

            // Get location dimensions
            locationWidth = location.Exterior.ExteriorData.Width;
            locationHeight = location.Exterior.ExteriorData.Height;

            // Get layout image dimensions
            layoutWidth = locationWidth * blockSizeWidth;
            layoutHeight = locationHeight * blockSizeHeight;

            // Create map layout
            switch (currentExteriorAutomapViewMode)
            {
                case ExteriorAutomapViewMode.Original:
                    createExteriorLayoutTexture(location, false, true);
                    break;
                case ExteriorAutomapViewMode.Extra:
                    createExteriorLayoutTexture(location, true, true);
                    break;
                case ExteriorAutomapViewMode.All:
                    createExteriorLayoutTexture(location, true, false);
                    break;
            }

            // create player marker
            if (!gameobjectPlayerMarkerArrow)
            {
                gameobjectPlayerMarkerArrow = GameObjectHelper.CreateDaggerfallMeshGameObject(99900, gameobjectExteriorAutomap.transform, false, null, true);
                gameobjectPlayerMarkerArrow.name = "PlayerMarkerArrow";
                gameobjectPlayerMarkerArrow.layer = layerAutomap;
                Material oldMat = gameobjectPlayerMarkerArrow.GetComponent<MeshRenderer>().material;
                Material newMat = new Material(oldMat);
                newMat.shader = Shader.Find("Unlit/Texture");
                //newMat.CopyPropertiesFromMaterial(oldMat);
                gameobjectPlayerMarkerArrow.GetComponent<MeshRenderer>().material = newMat;
                gameobjectPlayerMarkerArrow.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f) * layoutMultiplier;
            }

            // create player marker stamp (a darkened larger version in the background)
            if (!gameobjectPlayerMarkerArrowStamp)
            {
                gameobjectPlayerMarkerArrowStamp = GameObjectHelper.CreateDaggerfallMeshGameObject(99900, gameobjectExteriorAutomap.transform, false, null, true);
                gameobjectPlayerMarkerArrowStamp.name = "PlayerMarkerArrowStamp";
                gameobjectPlayerMarkerArrowStamp.layer = layerAutomap;
                Material oldMat = gameobjectPlayerMarkerArrowStamp.GetComponent<MeshRenderer>().material;
                Material newMat = new Material(oldMat);
                newMat.shader = Shader.Find("Unlit/Color");
                newMat.color = new Color(0.353f, 0.086f, 0.086f);
                //newMat.CopyPropertiesFromMaterial(oldMat);
                gameobjectPlayerMarkerArrowStamp.GetComponent<MeshRenderer>().material = newMat;
                gameobjectPlayerMarkerArrowStamp.transform.localScale = new Vector3(4.0f, 4.0f, 4.0f) * layoutMultiplier;
            }

            // create player circle
            if (!gameobjectPlayerMarkerCircle)
            {
                gameobjectPlayerMarkerCircle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                CapsuleCollider capsuleCollider = gameobjectPlayerMarkerCircle.GetComponent<CapsuleCollider>();
                if (capsuleCollider)
                {
                    // get rid of capsule collider
                    UnityEngine.Object.Destroy(capsuleCollider);
                }
                gameobjectPlayerMarkerCircle.name = "PlayerMarkerCircle";
                gameobjectPlayerMarkerCircle.layer = layerAutomap;
                gameobjectPlayerMarkerCircle.transform.SetParent(gameobjectExteriorAutomap.transform);
                Material oldMat = gameobjectPlayerMarkerCircle.GetComponent<MeshRenderer>().material;
                Material newMat = new Material(oldMat);
                newMat.shader = Shader.Find("Unlit/Color");
                newMat.color = new Color(0.75f, 0.71f, 0.71f);
                //newMat.CopyPropertiesFromMaterial(oldMat);
                gameobjectPlayerMarkerCircle.GetComponent<MeshRenderer>().material = newMat;
                gameobjectPlayerMarkerCircle.transform.localScale = new Vector3(12.0f, 1.0f, 12.0f) * layoutMultiplier;
            }

            updatePlayerMarker();

            //byte[] png = exteriorLayoutTexture.EncodeToPNG();
            //Debug.Log(String.Format("writing to folder {0}", Application.dataPath));
            //File.WriteAllBytes(Application.dataPath + "/test.png", png);

            createCustomCanvasForExteriorAutomap();

            ResetAutomapSettingsSignalForExternalScript = true; // force automap settings reset in next OnPush() function of DaggerfallExteriorAutomapWindow (for reset of camera settings)

            location.Loaded = true;
        }

        private void unloadLocationExteriorAutomap()
        {
            deleteBuildingNameplates();

            if (gameobjectCustomCanvas != null)
            {
                UnityEngine.Object.Destroy(gameobjectCustomCanvas);
                gameobjectCustomCanvas = null;
            }

            if (gameobjectPlayerMarkerArrow)
            {
                UnityEngine.Object.Destroy(gameobjectPlayerMarkerArrow);
                gameobjectPlayerMarkerArrow = null;
            }

            if (gameobjectPlayerMarkerArrowStamp)
            {
                UnityEngine.Object.Destroy(gameobjectPlayerMarkerArrowStamp);
                gameobjectPlayerMarkerArrowStamp = null;
            }

            if (gameobjectPlayerMarkerCircle)
            {
                UnityEngine.Object.Destroy(gameobjectPlayerMarkerCircle);
                gameobjectPlayerMarkerCircle = null;
            }

            if (exteriorLayoutTexture != null)
            {
                UnityEngine.Object.Destroy(exteriorLayoutTexture);
                exteriorLayoutTexture = null;
            }

            location.Loaded = false;
        }

        private void OnMapPixelChanged(DFPosition mapPixel)
        {
            ContentReader.MapSummary mapSummary;
            if (DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out mapSummary))
            {
                loadAndCreateLocationExteriorAutomap();
            }
        }

        private void OnTransitionToInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            gameobjectExteriorAutomap.SetActive(false);
        }

        private void OnTransitionToDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            gameobjectExteriorAutomap.SetActive(false);
        }

        private void OnTransitionToExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            gameobjectExteriorAutomap.SetActive(true);
            DFPosition mapPixel = GameManager.Instance.PlayerGPS.CurrentMapPixel;
            ContentReader.MapSummary mapSummary;
            if (DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out mapSummary))
            {
                loadAndCreateLocationExteriorAutomap();
            }
        }

        private void OnTransitionToDungeonExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            gameobjectExteriorAutomap.SetActive(true);
            DFPosition mapPixel = GameManager.Instance.PlayerGPS.CurrentMapPixel;
            ContentReader.MapSummary mapSummary;
            if (DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out mapSummary))
            {
                loadAndCreateLocationExteriorAutomap();
            }
        }

        void OnLoadEvent(SaveData_v1 saveData)
        {
            if (!GameManager.Instance.IsPlayerInside)
            {
                gameobjectExteriorAutomap.SetActive(true);
                DFPosition mapPixel = GameManager.Instance.PlayerGPS.CurrentMapPixel;
                ContentReader.MapSummary mapSummary;
                if (DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out mapSummary))
                {
                    loadAndCreateLocationExteriorAutomap();
                }
            }
        }
        #endregion
    }
}