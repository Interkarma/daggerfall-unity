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

        GameObject gameObjectCameraAutomap = null; // used to hold reference to GameObject to which camera class for automap camera is attached to
        Camera cameraExteriorAutomap = null; // camera for exterior automap camera

        GameObject gameObjectPlayerAdvanced = null; // used to hold reference to instance of GameObject "PlayerAdvanced"

        bool isOpenAutomap = false; // flag that indicates if automap window is open (set via Property IsOpenAutomap triggered by DaggerfallExteriorAutomapWindow script)

        // flag that indicates if external script should reset automap settings (set via Property ResetAutomapSettingsSignalForExternalScript checked and erased by DaggerfallExteriorAutomapWindow script)
        // this might look weirds - why not just notify the DaggerfallExteriorAutomapWindow class you may ask... - I wanted to make DaggerfallExteriorAutomap inaware and independent of the actual GUI implementation
        // so communication will always be only from DaggerfallExteriorAutomapWindow to DaggerfallExteriorAutomap class - so into other direction it works in that way that DaggerfallExteriorAutomap will pull
        // from DaggerfallExteriorAutomapWindow via flags - this is why this flag and its Property ResetAutomapSettingsSignalForExternalScript exist
        bool resetAutomapSettingsFromExternalScript = false;

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

        // location dimensions
        int locationWidth;
        int locationHeight;

        const int blockSizeWidth = 64;
        const int blockSizeHeight = 64;

        // layout image dimensions
        int layoutWidth;
        int layoutHeight;

        private BlockLayout[] exteriorLayout;

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

        /// <summary>
        /// DaggerfallExteriorAutomapWindow script will use this to check if it should reset automap settings (and if it does it will erase flag)
        /// </summary>
        public bool ResetAutomapSettingsSignalForExternalScript
        {
            get { return (resetAutomapSettingsFromExternalScript); }
            set { resetAutomapSettingsFromExternalScript = value; }
        }

        /// <summary>
        /// DaggerfallExteriorAutomapWindow script will use this to propagate if the automap window is open or not
        /// </summary>
        public bool IsOpenAutomap
        {
            set { isOpenAutomap = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// DaggerfallExteriorAutomapWindow script will use this to signal this script to update when automap window was pushed - TODO: check if this can done with an event (if events work with gui windows)
        /// </summary>
        public void updateAutomapStateOnWindowPush()
        {
            ContentReader.MapSummary mapSummary;
            DFPosition mapPixel = GameManager.Instance.PlayerGPS.CurrentMapPixel;
            if (!DaggerfallUnity.Instance.ContentReader.HasLocation(mapPixel.X, mapPixel.Y, out mapSummary))
            {
                // no location found, something went wrong
            }
            
            DFLocation location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(mapSummary.RegionIndex, mapSummary.MapIndex);
            if (!location.Loaded)
            {
                // Location not loaded, something went wrong
            }

            // Get location dimensions
            locationWidth = location.Exterior.ExteriorData.Width;
            locationHeight = location.Exterior.ExteriorData.Height;

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
                    DFBlock block = DaggerfallUnity.Instance.ContentReader.BlockFileReader.GetBlock(blockName);

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
            
            // Get layout image dimensions
            layoutWidth = locationWidth * blockSizeWidth;
            layoutHeight = locationHeight * blockSizeHeight;

            // Create layout image (texture)
            exteriorLayoutTexture = new Texture2D(layoutWidth, layoutHeight, TextureFormat.ARGB32, false);
            exteriorLayoutTexture.filterMode = FilterMode.Point;

            // Render map layout
            foreach (var layout in exteriorLayout)
            {
                DFBlock block = DaggerfallUnity.Instance.ContentReader.BlockFileReader.GetBlock(layout.name);
                DaggerfallUnity.Instance.ContentReader.BlockFileReader.LoadBlock(block.Index);
                // Get block automap image
                DFBitmap dfBitmap = DaggerfallUnity.Instance.ContentReader.BlockFileReader.GetBlockAutoMap(layout.name, true);

                int size = blockSizeWidth * blockSizeHeight;
                Color32[] colors = new Color32[size];
               // for (int i = 0; i < size; i++)
               // {
                for (int y=0; y < blockSizeHeight; y++)
                {
                    for (int x=0; x < blockSizeWidth; x++)
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
                                // do not display on automap
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
                exteriorLayoutTexture.Apply();

                DaggerfallUnity.Instance.ContentReader.BlockFileReader.DiscardBlock(block.Index);
            }

            //byte[] png = exteriorLayoutTexture.EncodeToPNG();
            //Debug.Log(String.Format("writing to folder {0}", Application.dataPath));
            //File.WriteAllBytes(Application.dataPath + "/test.png", png);

            createCustomCanvasForExteriorAutomap();

            // create camera (if not present) that will render automap level geometry
            createExteriorAutomapCamera();
        }

        /// <summary>
        /// DaggerfallExteriorAutomapWindow script will use this to signal this script to update when automap window was popped - TODO: check if this can done with an event (if events work with gui windows)
        /// </summary>
        public void updateAutomapStateOnWindowPop()
        {
            // destroy the camera so it does not use system resources
            if (gameObjectCameraAutomap != null)
            {
                UnityEngine.Object.Destroy(gameObjectCameraAutomap);
            }

            if (gameobjectCustomCanvas != null)
            {
                UnityEngine.Object.Destroy(gameobjectCustomCanvas);
                gameobjectCustomCanvas = null;
            }

            if (exteriorLayoutTexture != null)
            {
                UnityEngine.Object.Destroy(exteriorLayoutTexture);
                exteriorLayoutTexture = null;
            }
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

            layerAutomap = LayerMask.NameToLayer("ExteriorAutomap");
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
        }

        void OnDisable()
        {
        }

        void Start()
        {
            gameobjectExteriorAutomap = GameObject.Find("ExteriorAutomap");
            if (gameobjectExteriorAutomap == null)
            {
                DaggerfallUnity.LogMessage("GameObject \"ExteriorAutomap\" missing! Create a GameObject called \"ExteriorAutomap\" in root of hierarchy and add script Game/DaggerfallExteriorAutomap!\"", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }
        }

        void Update()
        {
            // if we are not in game (e.g. title menu) skip update function (update must not be skipped when in game or in gui window (to propagate all map control changes))
            if ((GameManager.Instance.StateManager.CurrentState != StateManager.StateTypes.Game) && (GameManager.Instance.StateManager.CurrentState != StateManager.StateTypes.UI))
            {
                return;
            }
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

        /// <summary>
        /// creates the exterior automap camera if not present and sets camera default settings, registers camera to compass
        /// </summary>
        private void createExteriorAutomapCamera()
        {
            if (!cameraExteriorAutomap)
            {
                gameObjectCameraAutomap = new GameObject("CameraExteriorAutomap");
                cameraExteriorAutomap = gameObjectCameraAutomap.AddComponent<Camera>();
                cameraExteriorAutomap.clearFlags = CameraClearFlags.SolidColor;
                cameraExteriorAutomap.cullingMask = 1 << layerAutomap;
                cameraExteriorAutomap.renderingPath = Camera.main.renderingPath;
                cameraExteriorAutomap.nearClipPlane = 0.7f;
                cameraExteriorAutomap.farClipPlane = 5000.0f;                
                gameObjectCameraAutomap.transform.SetParent(gameobjectExteriorAutomap.transform);
            }
        }

        private Mesh CreateMesh(float width, float height)
        {
            Mesh m = new Mesh();
            m.name = "ScriptedMesh";
            m.vertices = new Vector3[] {
                new Vector3(-width, 0.01f, height),
                new Vector3(width, 0.01f, height),
                new Vector3(width, 0.01f, -height),
                new Vector3(-width, 0.01f, -height)
            };
            m.uv = new Vector2[] {
                new Vector2 (0, 1),
                new Vector2 (1, 1),                
                new Vector2(1, 0),
                new Vector2 (0, 0)
            };
            m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
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
            meshFilter.mesh = CreateMesh(layoutWidth/10, layoutHeight/10); // create quad with normal facing into positive y-direction
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

        #endregion
    }
}