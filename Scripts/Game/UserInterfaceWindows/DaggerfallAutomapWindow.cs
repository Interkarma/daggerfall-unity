// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Nystul
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

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements indoor and dungeon automap window window.
    /// </summary>
    public class DaggerfallAutomapWindow : DaggerfallPopupWindow
    {
        const float scrollLeftRightSpeed = 1.0f; // left mouse on button arrow left/right makes geometry move with this speed
        const float scrollForwardBackwardSpeed = 1.0f; // left mouse on button arrow up/down makes geometry move with this speed
        const float moveRotationMarkerLeftRightSpeed = 0.2f; // right mouse on button arrow left/right makes rotation axis move with this speed
        const float moveRotationMarkerForwardBackwardSpeed = 0.2f; // right mouse on button arrow up/down makes rotation axis move with this speed
        const float moveUpDownSpeed = 0.5f; // left mouse on button upstairs/downstairs makes geometry move with this speed
        const float rotateSpeed = 3.0f; // left mouse on button rotate left/rotate right makes geometry rotate around the rotation pivot axis with this speed
        const float zoomSpeed = 0.1f; // suggested value range: 0.1f (fast) to 0.01f (slow) // mouse wheel inside main area of the automap window will zoom with this speed
        const float cameraTiltChangeSpeed = 0.5f; // currently no mouse functionality linked
        const float dragSpeed = 0.002f; // suggested value range: 0.01f (fast) to 0.001f (slow) // hold left mouse button down and move mouse to move geometry with this speed)
        const float dragRotateSpeed = 0.5f; // hold right mouse button down and move left/right to rotate geometry with this speed
        const float dragRotateCameraTiltSpeed = 0.15f; // hold right mouse button down and move up/down to change tilt of camera with this speed
        const float changeSpeedCameraFieldOfView = 1.0f; // mouse wheel over grid button will change camera field of view in 3D mode with this speed

        const float fieldOfViewCameraMode2D = 15.0f; // camera field of view used for 2D mode
        float fieldOfViewCameraMode3D = 45.0f; // camera field of view used for 3D mode (can be changed with mouse wheel over grid button)
        const float minFieldOfViewCameraMode3D = 15.0f; // minimum value of camera field of view that can be adjusted in 3D mode
        const float maxFieldOfViewCameraMode3D = 65.0f; // maximum value of camera field of view that can be adjusted in 3D mode

        const float cameraHeightViewFromTop = 90.0f; // initial camera height in 2D mode
        const float cameraHeightView3D = 8.0f; // initial camera height in 3D mode
        const float cameraBackwardDistance = 20.0f; // initial camera distance "backwards" in 3D mode

        const string nativeImgName = "AMAP00I0.IMG";
        const string nativeImgNameGrid3D = "AMAP01I0.IMG";

        DaggerfallAutomap daggerfallAutomap = null; // used to communicate with DaggerfallAutomap class

        GameObject gameObjectCameraAutomap = null; // used to hold reference to GameObject to which camera class for automap camera is attached to

        GameObject gameobjectAutomap = null; // used to hold reference to instance of GameObject "Automap" (which has script Game/DaggerfallAutomap.cs attached)

        int layerAutomap; // layer used for level geometry of automap
        GameObject gameObjectInteriorLightRig = null; // reference to instance of GameObject called "InteriorLightRig" - this will be used to deactivate lights of interior geometry inside GameObject "Interior"
        GameObject gameobjectAutomapKeyLight = null; // instead this script will use its own key light to lighten the level geometry used for automap
        GameObject gameobjectAutomapFillLight = null; // and fill light
        GameObject gameobjectAutomapBackLight = null; // and back light

        public enum AutomapViewMode { View2D = 0, View3D = 1};
        AutomapViewMode automapViewMode = AutomapViewMode.View2D;

        Panel dummyPanelAutomap = null; // used to determine correct render panel position
        Panel panelRenderAutomap = null; // level geometry is rendered into this panel
        Rect oldPositionNativePanel;
        Vector2 oldMousePosition; // old mouse position used to determine offset of mouse movement since last time used for for drag and drop functionality

        Panel dummyPanelCompass = null; // used to determine correct compass position

        // these boolean flags are used to indicate which mouse button was pressed over which gui button/element - these are set in the event callbacks
        bool leftMouseDownOnPanelAutomap = false;
        bool rightMouseDownOnPanelAutomap = false;
        bool rightMouseDownOnGridButton = false;
        bool leftMouseDownOnForwardButton = false;
        bool rightMouseDownOnForwardButton = false;
        bool leftMouseDownOnBackwardButton = false;
        bool rightMouseDownOnBackwardButton = false;
        bool leftMouseDownOnLeftButton = false;
        bool rightMouseDownOnLeftButton = false;
        bool leftMouseDownOnRightButton = false;
        bool rightMouseDownOnRightButton = false;
        bool leftMouseDownOnRotateLeftButton = false;
        bool leftMouseDownOnRotateRightButton = false;
        bool leftMouseDownOnUpstairsButton = false;
        bool leftMouseDownOnDownstairsButton = false;
        bool rightMouseDownOnUpstairsButton = false;
        bool rightMouseDownOnDownstairsButton = false;
        bool alreadyInMouseDown = false;
        bool inDragMode() { return leftMouseDownOnPanelAutomap || rightMouseDownOnPanelAutomap || rightMouseDownOnGridButton; }

        Texture2D nativeTexture; // background image will be stored in this Texture2D
        Texture2D nativeTextureGrid2D; // grid button texture for 2D view image will be stored in this Texture2D
        Texture2D nativeTextureGrid3D; // grid button texture for 3D view image will be stored in this Texture2D

        Color[] pixelsGrid2D;
        Color[] pixelsGrid3D;

        HUDCompass compass = null;

        Camera cameraAutomap = null; // camera for automap camera
        RenderTexture renderTextureAutomap = null; // render texture in which automap camera will render into
        Texture2D textureAutomap = null; // render texture will converted to this texture so that it can be drawn in panelRenderAutomap

        int renderTextureAutomapDepth = 16;
        int oldRenderTextureAutomapWidth; // used to store previous width of automap render texture to react to changes to NativePanel's size and react accordingly by setting texture up with new widht and height again
        int oldRenderTextureAutomapHeight; // used to store previous height of automap render texture to react to changes to NativePanel's size and react accordingly by setting texture up with new widht and height again

        // backup positions and rotations for 2D and 3D view to allow independent camera settings for both
        Vector3 backupCameraPositionViewFromTop;
        Quaternion backupCameraRotationViewFromTop;
        Vector3 backupCameraPositionView3D;
        Quaternion backupCameraRotationView3D;

        // independent bias of rotation pivot axis for both 2D and 3D view
        Vector3 biasRotationPivotAxisFromInitialPositionViewFromTop;
        Vector3 biasRotationPivotAxisFromInitialPositionView3D;

        float slicingBiasPositionY = 0.0f; // level of geometry slicing (y-position of slice plane)

        public AutomapViewMode CurrentAutomapViewMode
        {
            get { return (automapViewMode); }
        }

        public Vector3 BiasRotationPivotAxisFromInitialPositionViewFromTop
        {
            get { return (biasRotationPivotAxisFromInitialPositionViewFromTop); }
        }

        public Vector3 BiasRotationPivotAxisFromInitialPositionView3D
        {
            get { return (biasRotationPivotAxisFromInitialPositionView3D); }
        }

        public DaggerfallAutomapWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            ImgFile imgFile = null;
            DFBitmap bitmap = null;

            initClassResources();

            // Load native texture
            imgFile = new ImgFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, nativeImgName), FileUsage.UseMemory, false);
            imgFile.LoadPalette(Path.Combine(DaggerfallUnity.Instance.Arena2Path, imgFile.PaletteName));
            bitmap = imgFile.GetDFBitmap();
            nativeTexture = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.ARGB32, false);
            nativeTexture.SetPixels32(imgFile.GetColor32(bitmap, 0));
            nativeTexture.Apply(false, false); // make readable
            nativeTexture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            if (!nativeTexture)
                throw new Exception("DaggerfallAutomapWindow: Could not load native texture (AMAP00I0.IMG).");
            
            // Load alternative Grid Icon (3D View Grid graphics)
            imgFile = new ImgFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, nativeImgNameGrid3D), FileUsage.UseMemory, false);
            imgFile.LoadPalette(Path.Combine(DaggerfallUnity.Instance.Arena2Path, imgFile.PaletteName));
            bitmap = imgFile.GetDFBitmap();
            nativeTextureGrid3D = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.ARGB32, false);
            nativeTextureGrid3D.SetPixels32(imgFile.GetColor32(bitmap, 0));
            nativeTextureGrid3D.Apply(false, false); // make readable
            nativeTextureGrid3D.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            if (!nativeTextureGrid3D)
                throw new Exception("DaggerfallAutomapWindow: Could not load native texture (AMAP01I0.IMG).");
            pixelsGrid3D = nativeTextureGrid3D.GetPixels(0, 0, 27, 19);

            // Cut out 2D View Grid graphics from background image
            nativeTextureGrid2D = new Texture2D(27, 19, TextureFormat.ARGB32, false);
            nativeTextureGrid2D.filterMode = FilterMode.Point;
            pixelsGrid2D = nativeTexture.GetPixels(78, nativeTexture.height - 171 - 19, 27, 19);
            nativeTextureGrid2D.SetPixels(pixelsGrid2D);
            
            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            oldPositionNativePanel = NativePanel.Rectangle;

            // Setup automap render panel (into this the level geometry is rendered)
            Rect rectDummyPanelAutomap = new Rect();
            rectDummyPanelAutomap.position = new Vector2(1, 1);
            rectDummyPanelAutomap.size = new Vector2(318, 169);

            dummyPanelAutomap = DaggerfallUI.AddPanel(rectDummyPanelAutomap, NativePanel);
            //oldPositionDummyPanelAutomap = dummyPanelAutomap.Rectangle;

            Rect positionPanelRenderAutomap = dummyPanelAutomap.Rectangle;            
            panelRenderAutomap = DaggerfallUI.AddPanel(positionPanelRenderAutomap, ParentPanel);
            panelRenderAutomap.ScalingMode = Scaling.None;
            //oldPositionPanelRenderAutomap = panelRenderAutomap.Rectangle;
            
            panelRenderAutomap.OnMouseScrollUp += PanelAutomap_OnMouseScrollUp;
            panelRenderAutomap.OnMouseScrollDown += PanelAutomap_OnMouseScrollDown;
            panelRenderAutomap.OnMouseDown += PanelAutomap_OnMouseDown;
            panelRenderAutomap.OnMouseUp += PanelAutomap_OnMouseUp;
            panelRenderAutomap.OnRightMouseDown += PanelAutomap_OnRightMouseDown;
            panelRenderAutomap.OnRightMouseUp += PanelAutomap_OnRightMouseUp;

            // Grid button (toggle 2D <-> 3D view)
            Button gridButton = DaggerfallUI.AddButton(new Rect(78, 171, 27, 19), NativePanel);
            gridButton.OnMouseClick += GridButton_OnMouseClick;
            gridButton.OnRightMouseClick += GridButton_OnRightMouseClick;
            gridButton.OnMouseScrollUp += GridButton_OnMouseScrollUp;
            gridButton.OnMouseScrollDown += GridButton_OnMouseScrollDown;
            //gridButton.OnRightMouseUp += GridButton_OnRightMouseUp;
            //gridButton.OnRightMouseDown += GridButton_OnRightMouseDown;

            // forward button
            Button forwardButton = DaggerfallUI.AddButton(new Rect(105, 171, 21, 19), NativePanel);
            forwardButton.OnMouseDown += ForwardButton_OnMouseDown;
            forwardButton.OnMouseUp += ForwardButton_OnMouseUp;
            forwardButton.OnRightMouseDown += ForwardButton_OnRightMouseDown;
            forwardButton.OnRightMouseUp += ForwardButton_OnRightMouseUp;

            // backward button
            Button backwardButton = DaggerfallUI.AddButton(new Rect(126, 171, 21, 19), NativePanel);
            backwardButton.OnMouseDown += BackwardButton_OnMouseDown;
            backwardButton.OnMouseUp += BackwardButton_OnMouseUp;
            backwardButton.OnRightMouseDown += BackwardButton_OnRightMouseDown;
            backwardButton.OnRightMouseUp += BackwardButton_OnRightMouseUp;

            // left button
            Button leftButton = DaggerfallUI.AddButton(new Rect(149, 171, 21, 19), NativePanel);
            leftButton.OnMouseDown += LeftButton_OnMouseDown;
            leftButton.OnMouseUp += LeftButton_OnMouseUp;
            leftButton.OnRightMouseDown += LeftButton_OnRightMouseDown;
            leftButton.OnRightMouseUp += LeftButton_OnRightMouseUp;
            
            // right button
            Button rightButton = DaggerfallUI.AddButton(new Rect(170, 171, 21, 19), NativePanel);
            rightButton.OnMouseDown += RightButton_OnMouseDown;
            rightButton.OnMouseUp += RightButton_OnMouseUp;
            rightButton.OnRightMouseDown += RightButton_OnRightMouseDown;
            rightButton.OnRightMouseUp += RightButton_OnRightMouseUp;

            // rotate left button
            Button rotateLeftButton = DaggerfallUI.AddButton(new Rect(193, 171, 21, 19), NativePanel);
            rotateLeftButton.OnMouseDown += RotateLeftButton_OnMouseDown;
            rotateLeftButton.OnMouseUp += RotateLeftButton_OnMouseUp;

            // rotate right button
            Button rotateRightButton = DaggerfallUI.AddButton(new Rect(214, 171, 21, 19), NativePanel);
            rotateRightButton.OnMouseDown += RotateRightButton_OnMouseDown;
            rotateRightButton.OnMouseUp += RotateRightButton_OnMouseUp;

            // upstairs button
            Button upstairsButton = DaggerfallUI.AddButton(new Rect(237, 171, 21, 19), NativePanel);
            upstairsButton.OnMouseDown += UpstairsButton_OnMouseDown;
            upstairsButton.OnMouseUp += UpstairsButton_OnMouseUp;
            upstairsButton.OnRightMouseDown += UpstairsButton_OnRightMouseDown;
            upstairsButton.OnRightMouseUp += UpstairsButton_OnRightMouseUp;

            // downstairs button
            Button downstairsButton = DaggerfallUI.AddButton(new Rect(258, 171, 21, 19), NativePanel);
            downstairsButton.OnMouseDown += DownstairsButton_OnMouseDown;
            downstairsButton.OnMouseUp += DownstairsButton_OnMouseUp;
            downstairsButton.OnRightMouseDown += DownstairsButton_OnRightMouseDown;
            downstairsButton.OnRightMouseUp += DownstairsButton_OnRightMouseUp;

            // Exit button
            Button exitButton = DaggerfallUI.AddButton(new Rect(281, 171, 28, 19), NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            createLightsForAutomapGeometry();

            createAutomapCamera();

            createAutomapTextures((int)positionPanelRenderAutomap.width, (int)positionPanelRenderAutomap.height);

            resetCameraPosition();
            resetBiasFromInitialPosition();
            updateAutoMapView();

            Rect rectDummyPanelCompass = new Rect();
            rectDummyPanelCompass.position = new Vector2(3, 172);
            rectDummyPanelCompass.size = new Vector2(76, 17);
            dummyPanelCompass = DaggerfallUI.AddPanel(rectDummyPanelCompass, NativePanel);

            compass = new HUDCompass(cameraAutomap);
            Vector2 scale = NativePanel.LocalScale;

            compass.Position = dummyPanelCompass.Rectangle.position;
            compass.Scale = scale;
            NativePanel.Components.Add(compass);
        }

        private void resizeGUIelementsOnDemand()
        {
            if (oldPositionNativePanel != NativePanel.Rectangle)
            {
                panelRenderAutomap.Position = dummyPanelAutomap.Rectangle.position;
                panelRenderAutomap.Size = new Vector2(dummyPanelAutomap.InteriorWidth, dummyPanelAutomap.InteriorHeight);
           
                //Debug.Log(String.Format("dummy panel size: {0}, {1}; {2}, {3}; {4}, {5}; {6}, {7}\n", NativePanel.InteriorWidth, NativePanel.InteriorHeight, ParentPanel.InteriorWidth, ParentPanel.InteriorHeight, dummyPanelAutomap.InteriorWidth, dummyPanelAutomap.InteriorHeight, parentPanel.InteriorWidth, parentPanel.InteriorHeight));
                //Debug.Log(String.Format("dummy panel pos: {0}, {1}; {2}, {3}; {4}, {5}; {6}, {7}\n", NativePanel.Rectangle.xMin, NativePanel.Rectangle.yMin, ParentPanel.Rectangle.xMin, ParentPanel.Rectangle.yMin, dummyPanelAutomap.Rectangle.xMin, dummyPanelAutomap.Rectangle.yMin, parentPanel.Rectangle.xMin, parentPanel.Rectangle.yMin));
                Vector2 positionPanelRenderAutomap = new Vector2(dummyPanelAutomap.InteriorWidth, dummyPanelAutomap.InteriorHeight);
                createAutomapTextures((int)positionPanelRenderAutomap.x, (int)positionPanelRenderAutomap.y);
                updateAutoMapView();

                oldPositionNativePanel = NativePanel.Rectangle;

                Vector2 scale = NativePanel.LocalScale;
                compass.Position = dummyPanelCompass.Rectangle.position;
                compass.Scale = scale;
            }
        }

        public override void OnPush()
        {
            initClassResources();

            daggerfallAutomap.registerDaggerfallAutomapWindow(this);
            daggerfallAutomap.IsOpenAutomap = true; // signal DaggerfallAutomap script that automap is open and it should do its stuff in its Update() function            
            daggerfallAutomap.updateAutomapStateOnWindowPush(); // signal DaggerfallAutomap script that automap window was closed and that it should update its state (updates player marker arrow)

            if ((GameManager.Instance.PlayerEnterExit.IsPlayerInside) && (GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding))
            {
                // disable interior lights - disabling instead of setting lights' culling mask - since only a small number of lights can be ignored by layer (got a warning when I tried)
                gameObjectInteriorLightRig = GameObject.Find("InteriorLightRig");
                gameObjectInteriorLightRig.SetActive(false);
            }

            if (IsSetup)
            {
                createLightsForAutomapGeometry();

                createAutomapCamera();

                Rect positionPanelRenderAutomap = dummyPanelAutomap.Rectangle;
                createAutomapTextures((int)positionPanelRenderAutomap.width, (int)positionPanelRenderAutomap.height);

                if (cameraAutomap)
                {
                    resetCameraPosition();
                    resetBiasFromInitialPosition();
                    updateAutoMapView();
                }

                slicingBiasPositionY = 0.0f;
            }
        }

        public override void OnPop()
        {
            daggerfallAutomap.IsOpenAutomap = false;

            if ((GameManager.Instance.PlayerEnterExit.IsPlayerInside) && ((GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding) || (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon) || (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeonPalace)))
            {
                // enable interior lights
                if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
                {
                    gameObjectInteriorLightRig.SetActive(true);
                }
                UnityEngine.Object.DestroyImmediate(gameobjectAutomapKeyLight);
                UnityEngine.Object.DestroyImmediate(gameobjectAutomapFillLight);
                UnityEngine.Object.DestroyImmediate(gameobjectAutomapBackLight);
            }

            if (gameObjectCameraAutomap != null)
            {
                UnityEngine.Object.DestroyImmediate(gameObjectCameraAutomap);
            }

            if (renderTextureAutomap != null)
            {
                UnityEngine.Object.DestroyImmediate(renderTextureAutomap);
            }

            if (textureAutomap != null)
            {
                UnityEngine.Object.DestroyImmediate(textureAutomap);
            }

            daggerfallAutomap.updateAutomapStateOnWindowPush(); // signal DaggerfallAutomap script that automap window was closed
        }


        public override void Update()
        {
            base.Update();

            resizeGUIelementsOnDemand();

            if (leftMouseDownOnPanelAutomap)
            {
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

                float dragSpeedCompensated = dragSpeed * Vector3.Magnitude(Camera.main.transform.position + getBiasFromInitialPosition() - cameraAutomap.transform.position);
                Vector2 bias = mousePosition - oldMousePosition;
                Vector3 translation = -cameraAutomap.transform.right * dragSpeedCompensated * bias.x + cameraAutomap.transform.up * dragSpeedCompensated * bias.y;
                cameraAutomap.transform.position += translation;
                updateAutoMapView();
                oldMousePosition = mousePosition;
            }

            if (rightMouseDownOnPanelAutomap)
            {
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
                
                Vector2 bias = mousePosition - oldMousePosition;

                cameraAutomap.transform.Rotate(0.0f, +dragRotateSpeed * bias.x, 0.0f, Space.World);

                if (automapViewMode == AutomapViewMode.View3D)
                {
                    cameraAutomap.transform.Rotate(+dragRotateCameraTiltSpeed * bias.y, 0.0f, 0.0f, Space.Self);
                }

                updateAutoMapView();
                oldMousePosition = mousePosition;
            }

            //if (rightMouseDownOnGridButton)
            //{
            //    Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

            //    Vector2 bias = mousePosition - oldMousePosition;

            //    if (automapViewMode == AutomapViewMode.View3D)
            //    {
            //        cameraAutomap.transform.Rotate(cameraTiltChangeSpeed * bias.y, 0.0f, 0.0f, Space.Self);
            //        updateAutoMapView();
            //    }

            //    updateAutoMapView();
            //    oldMousePosition = mousePosition;
            //}

            if (leftMouseDownOnForwardButton)
            {
                Vector3 translation;
                switch (automapViewMode)
                {
                    case AutomapViewMode.View2D:
                        translation = -cameraAutomap.transform.up * scrollForwardBackwardSpeed;
                        break;
                    case AutomapViewMode.View3D:
                        translation = -cameraAutomap.transform.forward * scrollForwardBackwardSpeed;
                        translation.y = 0.0f; // comment this out for movement along camera optical axis
                        break;
                    default:
                        translation = Vector3.zero;
                        break;
                }
                cameraAutomap.transform.position += translation;
                shiftBiasFromInitialPosition(translation);
                updateAutoMapView();
            }

            if (rightMouseDownOnForwardButton)
            {
                Vector3 translation;
                switch (automapViewMode)
                {
                    case AutomapViewMode.View2D:
                        translation = cameraAutomap.transform.up * moveRotationMarkerForwardBackwardSpeed;
                        break;
                    case AutomapViewMode.View3D:
                        translation = cameraAutomap.transform.forward * moveRotationMarkerForwardBackwardSpeed;
                        translation.y = 0.0f; // comment this out for movement along camera optical axis
                        break;
                    default:
                        translation = Vector3.zero;
                        break;
                }
                shiftBiasFromInitialPosition(translation);
                updateAutoMapView();
            }

            if (leftMouseDownOnBackwardButton)
            {
                Vector3 translation;
                switch (automapViewMode)
                {
                    case AutomapViewMode.View2D:
                        translation = cameraAutomap.transform.up * scrollForwardBackwardSpeed;
                        break;
                    case AutomapViewMode.View3D:
                        translation = cameraAutomap.transform.forward * scrollForwardBackwardSpeed;
                        translation.y = 0.0f; // comment this out for movement along camera optical axis
                        break;
                    default:
                        translation = Vector3.zero;
                        break;
                }
                cameraAutomap.transform.position += translation;
                shiftBiasFromInitialPosition(translation);
                updateAutoMapView();
            }

            if (rightMouseDownOnBackwardButton)
            {
                Vector3 translation;
                switch (automapViewMode)
                {
                    case AutomapViewMode.View2D:
                        translation = -cameraAutomap.transform.up * moveRotationMarkerForwardBackwardSpeed;
                        break;
                    case AutomapViewMode.View3D:
                        translation = -cameraAutomap.transform.forward * moveRotationMarkerForwardBackwardSpeed;
                        translation.y = 0.0f; // comment this out for movement along camera optical axis
                        break;
                    default:
                        translation = Vector3.zero;
                        break;
                }
                shiftBiasFromInitialPosition(translation);             
                updateAutoMapView();
            }


            if (leftMouseDownOnLeftButton)
            {
                Vector3 translation = cameraAutomap.transform.right * scrollLeftRightSpeed;
                translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector
                cameraAutomap.transform.position += translation;
                shiftBiasFromInitialPosition(translation);
                updateAutoMapView();
            }

            if (rightMouseDownOnLeftButton)
            {
                Vector3 translation = -cameraAutomap.transform.right * moveRotationMarkerLeftRightSpeed;
                translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector
                shiftBiasFromInitialPosition(translation);
                updateAutoMapView();
            }

            if (leftMouseDownOnRightButton)
            {
                Vector3 translation = -cameraAutomap.transform.right * scrollLeftRightSpeed;
                translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector
                cameraAutomap.transform.position += translation;
                shiftBiasFromInitialPosition(translation);
                updateAutoMapView();
            }

            if (rightMouseDownOnRightButton)
            {
                Vector3 translation = cameraAutomap.transform.right * moveRotationMarkerLeftRightSpeed;
                translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector                
                shiftBiasFromInitialPosition(translation);
                updateAutoMapView();
            }


            if (leftMouseDownOnRotateLeftButton)
            {
                Vector3 biasRotationPivotAxisFromInitialPosition;
                switch (automapViewMode)
                {
                    case AutomapViewMode.View2D:
                        biasRotationPivotAxisFromInitialPosition = biasRotationPivotAxisFromInitialPositionViewFromTop;
                        break;
                    case AutomapViewMode.View3D:
                        biasRotationPivotAxisFromInitialPosition = biasRotationPivotAxisFromInitialPositionView3D;
                        break;
                    default:
                        biasRotationPivotAxisFromInitialPosition = Vector3.zero;
                        break;
                }
                cameraAutomap.transform.RotateAround(Camera.main.transform.position + biasRotationPivotAxisFromInitialPosition, -Vector3.up, -rotateSpeed);
                //cameraAutomap.transform.RotateAround(Camera.main.transform.position, Vector3.up, -rotateSpeed);
                updateAutoMapView();
            }

            if (leftMouseDownOnRotateRightButton)
            {
                Vector3 biasRotationPivotAxisFromInitialPosition;
                switch (automapViewMode)
                {
                    case AutomapViewMode.View2D:
                        biasRotationPivotAxisFromInitialPosition = biasRotationPivotAxisFromInitialPositionViewFromTop;
                        break;
                    case AutomapViewMode.View3D:
                        biasRotationPivotAxisFromInitialPosition = biasRotationPivotAxisFromInitialPositionView3D;
                        break;
                    default:
                        biasRotationPivotAxisFromInitialPosition = Vector3.zero;
                        break;
                }
                cameraAutomap.transform.RotateAround(Camera.main.transform.position + biasRotationPivotAxisFromInitialPosition, -Vector3.up, +rotateSpeed);
                //cameraAutomap.transform.RotateAround(Camera.main.transform.position, Vector3.up, +rotateSpeed);
                updateAutoMapView();
            }

            if (leftMouseDownOnUpstairsButton)
            {
                cameraAutomap.transform.position += Vector3.up * moveUpDownSpeed;
                updateAutoMapView();
            }

            if (leftMouseDownOnDownstairsButton)
            {
                cameraAutomap.transform.position += Vector3.down * moveUpDownSpeed;
                updateAutoMapView();
            }

            if (rightMouseDownOnUpstairsButton)
            {
                slicingBiasPositionY += Vector3.up.y * moveUpDownSpeed;
                daggerfallAutomap.SlicingBiasPositionY = slicingBiasPositionY;
                updateAutoMapView();
            }

            if (rightMouseDownOnDownstairsButton)
            {
                slicingBiasPositionY += Vector3.down.y * moveUpDownSpeed;
                daggerfallAutomap.SlicingBiasPositionY = slicingBiasPositionY;
                updateAutoMapView();
            }
        }        

        #region Private Methods

        private void initClassResources()
        {
            if (!gameobjectAutomap)
            {
                gameobjectAutomap = GameObject.Find("Automap");
                if (gameobjectAutomap == null)
                {
                    DaggerfallUnity.LogMessage("GameObject \"Automap\" missing! Create a GameObject called \"Automap\" in root of hierarchy and add script Game/DaggerfallAutomap!\"", true);
                }
            }

            if (!daggerfallAutomap)
            {
                daggerfallAutomap = gameobjectAutomap.GetComponent<DaggerfallAutomap>();
                if (daggerfallAutomap == null)
                {
                    DaggerfallUnity.LogMessage("Script DafferfallAutomap is missing in GameObject \"Automap\"! GameObject \"Automap\" must have script Game/DaggerfallAutomap attached!\"", true);
                }
            }

            layerAutomap = LayerMask.NameToLayer("Automap");
            if (layerAutomap == -1)
            {
                DaggerfallUnity.LogMessage("Layer with name \"Automap\" missing! Set it in Unity Editor under \"Edit/Project Settings/Tags and Layers!\"", true);
            }

        }

        static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }

        private void createAutomapCamera()
        {
            if (!cameraAutomap)
            {
                gameObjectCameraAutomap = new GameObject("CameraAutomap");
                cameraAutomap = gameObjectCameraAutomap.AddComponent<Camera>();
                cameraAutomap.clearFlags = CameraClearFlags.SolidColor;
                cameraAutomap.cullingMask = 1 << layerAutomap;
                cameraAutomap.renderingPath = RenderingPath.DeferredLighting;
                cameraAutomap.nearClipPlane = 0.7f;
                cameraAutomap.farClipPlane = 5000.0f;

                switch (automapViewMode)
                {
                    case AutomapViewMode.View2D: default:
                        cameraAutomap.fieldOfView = fieldOfViewCameraMode2D;
                        break;
                    case AutomapViewMode.View3D:
                        cameraAutomap.fieldOfView = fieldOfViewCameraMode3D;
                        break;
                }

                gameObjectCameraAutomap.transform.SetParent(gameobjectAutomap.transform);

                if (compass != null)
                {
                    compass.CompassCamera = cameraAutomap;
                }                        
            }
        }


        private void createLightsForAutomapGeometry()
        {
            gameobjectAutomapKeyLight = new GameObject("AutomapKeyLight");
            gameobjectAutomapKeyLight.transform.rotation = Quaternion.Euler(50.0f, 270.0f, 0.0f);
            Light scriptKeyLight = gameobjectAutomapKeyLight.AddComponent<Light>();
            scriptKeyLight.type = LightType.Directional;            
            //scriptKeyLight.cullingMask = 1 << layerAutomap; // issues warning "Too many layers used to exclude objects from lighting. Up to 4 layers can be used to exclude lights"
            gameobjectAutomapKeyLight.transform.SetParent(gameobjectAutomap.transform);

            gameobjectAutomapFillLight = new GameObject("AutomapFillLight");
            gameobjectAutomapFillLight.transform.rotation = Quaternion.Euler(50.0f, 126.0f, 0.0f);
            Light scriptFillLight = gameobjectAutomapFillLight.AddComponent<Light>();
            scriptFillLight.type = LightType.Directional;          
            gameobjectAutomapFillLight.transform.SetParent(gameobjectAutomap.transform);

            gameobjectAutomapBackLight = new GameObject("AutomapBackLight");
            gameobjectAutomapBackLight.transform.rotation = Quaternion.Euler(50.0f, 0.0f, 0.0f);
            Light scriptBackLight = gameobjectAutomapBackLight.AddComponent<Light>();
            scriptBackLight.type = LightType.Directional;            
            gameobjectAutomapBackLight.transform.SetParent(gameobjectAutomap.transform);

            if (GameManager.Instance.IsPlayerInsideBuilding)
            {
                scriptKeyLight.intensity = 1.0f;
                scriptFillLight.intensity = 0.6f;
                scriptBackLight.intensity = 0.2f;
            }
            else if  ((GameManager.Instance.IsPlayerInsideDungeon)||(GameManager.Instance.IsPlayerInsidePalace))
            {
                scriptKeyLight.intensity = 0.5f;
                scriptFillLight.intensity = 0.5f;
                scriptBackLight.intensity = 0.5f;
            }
        }

        private void createAutomapTextures(int width, int height)
        {
            if ((!cameraAutomap) || (!renderTextureAutomap) || (oldRenderTextureAutomapWidth != width) || (oldRenderTextureAutomapHeight != height))
            {
                cameraAutomap.targetTexture = null;
                if (renderTextureAutomap)
                    UnityEngine.Object.DestroyImmediate(renderTextureAutomap);
                if (textureAutomap)
                    UnityEngine.Object.DestroyImmediate(textureAutomap);

                renderTextureAutomap = new RenderTexture(width, height, renderTextureAutomapDepth);
                cameraAutomap.targetTexture = renderTextureAutomap;

                textureAutomap = new Texture2D(renderTextureAutomap.width, renderTextureAutomap.height, TextureFormat.ARGB32, false);

                oldRenderTextureAutomapWidth = width;
                oldRenderTextureAutomapHeight = height;
            }
        }

        private void resetCameraPosition()
        {
            // get initial values for camera transform for view from top
            resetCameraTransformViewFromTop();
            saveCameraTransformViewFromTop();
            
            // get initial values for camera transform for 3D view
            resetCameraTransformView3D();
            saveCameraTransformView3D();

            // then set camera transform according to grid-button (view mode) setting
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D: default:
                    resetCameraTransformViewFromTop();
                    break;
                case AutomapViewMode.View3D:
                    resetCameraTransformView3D();
                    break;
            }
        }

        private void resetBiasFromInitialPosition()
        {
            biasRotationPivotAxisFromInitialPositionViewFromTop = Vector3.zero;
            biasRotationPivotAxisFromInitialPositionView3D = Vector3.forward;
        }

        private void shiftBiasFromInitialPosition(Vector3 translation)
        {
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    biasRotationPivotAxisFromInitialPositionViewFromTop += translation;
                    break;
                case AutomapViewMode.View3D:
                    biasRotationPivotAxisFromInitialPositionView3D += translation;
                    break;
                default:
                    break;
            }
        }

        private Vector3 getBiasFromInitialPosition()
        {
            Vector3 biasRotationPivotAxisFromInitialPosition;
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    biasRotationPivotAxisFromInitialPosition = biasRotationPivotAxisFromInitialPositionViewFromTop;
                    break;
                case AutomapViewMode.View3D:
                    biasRotationPivotAxisFromInitialPosition = biasRotationPivotAxisFromInitialPositionView3D;
                    break;
                default:
                    biasRotationPivotAxisFromInitialPosition = Vector3.zero;
                    break;
            }
            return (biasRotationPivotAxisFromInitialPosition);
        }

        private void resetCameraTransformViewFromTop()
        {
            cameraAutomap.transform.position = Camera.main.transform.position + Vector3.up * cameraHeightViewFromTop;
            cameraAutomap.transform.LookAt(Camera.main.transform.position);            
        }

        private void resetCameraTransformView3D()
        {
            Vector3 viewDirectionInXZ = Vector3.forward; // new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z);
            cameraAutomap.transform.position = Camera.main.transform.position - viewDirectionInXZ * cameraBackwardDistance + Vector3.up * cameraHeightView3D;
            //cameraAutomap.transform.rotation = Camera.main.transform.rotation;
            cameraAutomap.transform.LookAt(Camera.main.transform.position);
        }

        private void saveCameraTransformViewFromTop()
        {
            backupCameraPositionViewFromTop = cameraAutomap.transform.position;
            backupCameraRotationViewFromTop = cameraAutomap.transform.rotation;
        }

        private void saveCameraTransformView3D()
        {
            backupCameraPositionView3D = cameraAutomap.transform.position;
            backupCameraRotationView3D = cameraAutomap.transform.rotation;
        }

        private void restoreOldCameraTransformViewFromTop()
        {
            cameraAutomap.transform.position = backupCameraPositionViewFromTop;
            cameraAutomap.transform.rotation = backupCameraRotationViewFromTop;
        }

        private void restoreOldCameraTransformView3D()
        {
            cameraAutomap.transform.position = backupCameraPositionView3D;
            cameraAutomap.transform.rotation = backupCameraRotationView3D;
        }

        private void updateAutoMapView()
        {
            daggerfallAutomap.forceUpdate();

            if ((!cameraAutomap) || (!renderTextureAutomap))
                return;

            cameraAutomap.Render();

            RenderTexture.active = renderTextureAutomap;
            textureAutomap.ReadPixels(new Rect(0, 0, renderTextureAutomap.width, renderTextureAutomap.height), 0, 0);
            textureAutomap.Apply(false);
            RenderTexture.active = null;

            panelRenderAutomap.BackgroundTexture = textureAutomap;
        }


        #endregion

        #region Event Handlers

        private void PanelAutomap_OnMouseScrollUp()
        {
            float zoomSpeedCompensated = zoomSpeed * Vector3.Magnitude(Camera.main.transform.position + getBiasFromInitialPosition() - cameraAutomap.transform.position);
            Vector3 translation = cameraAutomap.transform.forward * zoomSpeedCompensated;
            cameraAutomap.transform.position += translation;
            updateAutoMapView();
        }

        private void PanelAutomap_OnMouseScrollDown()
        {
            float zoomSpeedCompensated = zoomSpeed * Vector3.Magnitude(Camera.main.transform.position + getBiasFromInitialPosition() - cameraAutomap.transform.position);
            Vector3 translation = -cameraAutomap.transform.forward * zoomSpeedCompensated;
            cameraAutomap.transform.position += translation;
            updateAutoMapView();
        }

        private void PanelAutomap_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (alreadyInMouseDown)
                return;

            Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            oldMousePosition = mousePosition;            
            leftMouseDownOnPanelAutomap = true;
            alreadyInMouseDown = true;
        }

        private void PanelAutomap_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            leftMouseDownOnPanelAutomap = false;
            alreadyInMouseDown = false;
        }

        private void PanelAutomap_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (alreadyInMouseDown)
                return;

            Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            oldMousePosition = mousePosition;
            rightMouseDownOnPanelAutomap = true;
            alreadyInMouseDown = true;
        }

        private void PanelAutomap_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rightMouseDownOnPanelAutomap = false;
            alreadyInMouseDown = false;
        }

        private void GridButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode())
                return;

            int numberOfViewModes = Enum.GetNames(typeof(AutomapViewMode)).Length;
            automapViewMode++;
            if ((int)automapViewMode > numberOfViewModes - 1) // first mode is mode 0 -> so use numberOfViewModes-1 for comparison
                automapViewMode = 0;
            switch(automapViewMode)
            {
                case AutomapViewMode.View2D:
                    // update grid graphics
                    nativeTexture.SetPixels(78, nativeTexture.height - 171 - 19, 27, 19, pixelsGrid2D);
                    nativeTexture.Apply(false);
                    saveCameraTransformView3D();
                    restoreOldCameraTransformViewFromTop();
                    cameraAutomap.fieldOfView = fieldOfViewCameraMode2D;
                    updateAutoMapView();
                    break;
                case AutomapViewMode.View3D:
                    // update grid graphics
                    nativeTexture.SetPixels(78, nativeTexture.height - 171 - 19, 27, 19, pixelsGrid3D);
                    nativeTexture.Apply(false);
                    saveCameraTransformViewFromTop();
                    restoreOldCameraTransformView3D();
                    cameraAutomap.fieldOfView = fieldOfViewCameraMode3D;
                    updateAutoMapView();
                    break;
                default:
                    break;
            }
        }

        private void GridButton_OnRightMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode())
                return;

            switch(automapViewMode)
            {
                case AutomapViewMode.View2D:
                    biasRotationPivotAxisFromInitialPositionViewFromTop = Vector3.zero;
                    updateAutoMapView();
                    break;
                case AutomapViewMode.View3D:
                    biasRotationPivotAxisFromInitialPositionView3D = Vector3.zero;
                    updateAutoMapView();
                    break;
                default:
                    break;
            }
            updateAutoMapView();
        }

        //private void GridButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        //{
        //    if (alreadyInMouseDown)
        //        return;

        //    Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        //    oldMousePosition = mousePosition;

        //    rightMouseDownOnGridButton = true;
        //    alreadyInMouseDown = true;
        //}

        //private void GridButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        //{
        //    rightMouseDownOnGridButton = false;
        //    alreadyInMouseDown = false;
        //}

        private void GridButton_OnMouseScrollUp()
        {
            if (inDragMode())
                return;

            if (automapViewMode == AutomapViewMode.View3D)
            {
                fieldOfViewCameraMode3D = Mathf.Max(minFieldOfViewCameraMode3D, Mathf.Min(maxFieldOfViewCameraMode3D, fieldOfViewCameraMode3D + changeSpeedCameraFieldOfView));
                cameraAutomap.fieldOfView = fieldOfViewCameraMode3D;
                updateAutoMapView();
            }
        }

        private void GridButton_OnMouseScrollDown()
        {
            if (inDragMode())
                return;

            if (automapViewMode == AutomapViewMode.View3D)
            {
                fieldOfViewCameraMode3D = Mathf.Max(minFieldOfViewCameraMode3D, Mathf.Min(maxFieldOfViewCameraMode3D, fieldOfViewCameraMode3D - changeSpeedCameraFieldOfView));
                cameraAutomap.fieldOfView = fieldOfViewCameraMode3D;
                updateAutoMapView();
            }
        }

        private void ForwardButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            leftMouseDownOnForwardButton = true;
            alreadyInMouseDown = true;
        }

        private void ForwardButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            leftMouseDownOnForwardButton = false;
            alreadyInMouseDown = false;
        }

        private void ForwardButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            rightMouseDownOnForwardButton = true;
            alreadyInMouseDown = true;
        }

        private void ForwardButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rightMouseDownOnForwardButton = false;
            alreadyInMouseDown = false;
        }

        private void BackwardButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            leftMouseDownOnBackwardButton = true;
            alreadyInMouseDown = true;
        }

        private void BackwardButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            leftMouseDownOnBackwardButton = false;
            alreadyInMouseDown = false;
        }

        private void BackwardButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            rightMouseDownOnBackwardButton = true;
            alreadyInMouseDown = true;
        }

        private void BackwardButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rightMouseDownOnBackwardButton = false;
            alreadyInMouseDown = false;
        }

        private void LeftButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            leftMouseDownOnLeftButton = true;
            alreadyInMouseDown = true;
        }

        private void LeftButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            leftMouseDownOnLeftButton = false;
            alreadyInMouseDown = false;
        }

        private void LeftButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            rightMouseDownOnLeftButton = true;
            alreadyInMouseDown = true;
        }

        private void LeftButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rightMouseDownOnLeftButton = false;
            alreadyInMouseDown = false;
        }

        private void RightButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            leftMouseDownOnRightButton = true;
            alreadyInMouseDown = true;
        }

        private void RightButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            leftMouseDownOnRightButton = false;
            alreadyInMouseDown = false;
        }

        private void RightButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            rightMouseDownOnRightButton = true;
            alreadyInMouseDown = true;
        }

        private void RightButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rightMouseDownOnRightButton = false;
            alreadyInMouseDown = false;
        }

        private void RotateLeftButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            leftMouseDownOnRotateLeftButton = true;
            alreadyInMouseDown = true;
        }

        private void RotateLeftButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            leftMouseDownOnRotateLeftButton = false;
            alreadyInMouseDown = false;
        }

        private void RotateRightButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            leftMouseDownOnRotateRightButton = true;
            alreadyInMouseDown = true;
        }

        private void RotateRightButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            leftMouseDownOnRotateRightButton = false;
            alreadyInMouseDown = false;
        }


        private void UpstairsButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            leftMouseDownOnUpstairsButton = true;
            alreadyInMouseDown = true;
        }

        private void UpstairsButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            leftMouseDownOnUpstairsButton = false;
            alreadyInMouseDown = false;
        }


        private void DownstairsButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            leftMouseDownOnDownstairsButton = true;
            alreadyInMouseDown = true;
        }

        private void DownstairsButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            leftMouseDownOnDownstairsButton = false;
            alreadyInMouseDown = false;
        }

        private void UpstairsButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            rightMouseDownOnUpstairsButton = true;
            alreadyInMouseDown = true;
        }

        private void UpstairsButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rightMouseDownOnUpstairsButton = false;
            alreadyInMouseDown = false;
        }


        private void DownstairsButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            rightMouseDownOnDownstairsButton = true;
            alreadyInMouseDown = true;
        }

        private void DownstairsButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rightMouseDownOnDownstairsButton = false;
            alreadyInMouseDown = false;
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode())
                return;

            CloseWindow();
        }

        #endregion
    }
}