// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Michael Rauter (a.k.a. Nystul)
// Contributors:    
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
        const int toolTipDelay = 1; // delay in seconds before button tooltips are shown

        const float scrollLeftRightSpeed = 50.0f; // left mouse on button arrow left/right makes geometry move with this speed
        const float scrollForwardBackwardSpeed = 50.0f; // left mouse on button arrow up/down makes geometry move with this speed
        const float moveRotationPivotAxisMarkerLeftRightSpeed = 10.0f; // right mouse on button arrow left/right makes rotation pivot axis move with this speed
        const float moveRotationPivotAxisMarkerForwardBackwardSpeed = 10.0f; // right mouse on button arrow up/down makes rotation pivot axis move with this speed
        const float moveUpDownSpeed = 25.0f; // left mouse on button upstairs/downstairs makes geometry move with this speed
        const float rotateSpeed = 150.0f; // left mouse on button rotate left/rotate right makes geometry rotate around the rotation pivot axis with this speed
        const float rotateCameraSpeed = 50.0f; // right mouse on button rotate left/rotate right makes camera rotate around itself with this speed
        const float rotateCameraOnCameraYZplaneAroundObjectSpeedInView3D = 50.0f; // rotate camera on camera YZ-plane around object with this speed when keyboard hotkey is pressed
        const float zoomSpeed = 3.0f; // zoom with this speed when keyboard hotkey is pressed
        const float zoomSpeedMouseWheel = 0.06f; // mouse wheel inside main area of the automap window will zoom with this speed
        const float dragSpeedInView3D = 0.002f; // hold left mouse button down and move mouse to move geometry with this speed) - in 3D view mode
        const float dragSpeedInTopView = 0.0002f; // hold left mouse button down and move mouse to move geometry with this speed) - in top view mode (2D view mode)
        const float dragRotateSpeedInTopView = 5.0f; // hold right mouse button down and move left/right to rotate geometry with this speed - in top view mode (2D view mode)
        const float dragRotateSpeedInView3D = 4.5f; // hold right mouse button down and move left/right to rotate geometry with this speed - in 3D view mode
        const float dragRotateCameraOnCameraYZplaneAroundObjectSpeedInView3D = 5.0f; // hold right mouse button down and move up/down to rotate camera on camera YZ-plane around object with this speed - in 3D view mode            

        const float changeSpeedCameraFieldOfView = 50.0f; // mouse wheel over grid button will change camera field of view in 3D mode with this speed

        const float fieldOfViewCameraMode2D = 15.0f; // camera field of view used for 2D mode
        const float defaultFieldOfViewCameraMode3D = 45.0f; // default camera field of view used for 3D mode
        float fieldOfViewCameraMode3D = defaultFieldOfViewCameraMode3D; // camera field of view used for 3D mode (can be changed with mouse wheel over grid button)
        const float minFieldOfViewCameraMode3D = 15.0f; // minimum value of camera field of view that can be adjusted in 3D mode
        const float maxFieldOfViewCameraMode3D = 65.0f; // maximum value of camera field of view that can be adjusted in 3D mode

        const float defaultSlicingBiasY = 0.2f;

        const float cameraHeightViewFromTop = 90.0f; // initial camera height in 2D mode
        const float cameraHeightView3D = 8.0f; // initial camera height in 3D mode
        const float cameraBackwardDistance = 20.0f; // initial camera distance "backwards" in 3D mode

        // this is a helper class to implement behaviour and easier use of hotkeys and key modifiers (left-shift, right-shift, ...) in conjunction
        // note: currently a combination of key modifiers like shift+alt is not supported. all specified modifiers are comined with an or-relation
        class HotkeySequence
        {
            public enum KeyModifiers
            {
                None = 0,
                LeftControl = 1,
                RightControl = 2,
                LeftShift = 4,
                RightShift = 8,
                LeftAlt = 16,
                RightAlt = 32
            };

            public KeyCode keyCode;
            public KeyModifiers modifiers;

            public HotkeySequence(KeyCode keyCode, KeyModifiers modifiers)      
            {
                this.keyCode = keyCode;
                this.modifiers = modifiers;
            }

            public static KeyModifiers getKeyModifiers(bool leftControl, bool rightControl, bool leftShift, bool rightShift, bool leftAlt, bool rightAlt)
            {
                KeyModifiers keyModifiers = KeyModifiers.None;
                if (leftControl)
                    keyModifiers = keyModifiers | KeyModifiers.LeftControl;
                if (rightControl)
                    keyModifiers = keyModifiers | KeyModifiers.RightControl;
                if (leftShift)
                    keyModifiers = keyModifiers | KeyModifiers.LeftShift;
                if (rightShift)
                    keyModifiers = keyModifiers | KeyModifiers.RightShift;
                if (leftAlt)
                    keyModifiers = keyModifiers | KeyModifiers.LeftAlt;
                if (rightAlt)
                    keyModifiers = keyModifiers | KeyModifiers.RightAlt;
                return keyModifiers;
            }

            public static bool checkSetModifiers(HotkeySequence.KeyModifiers pressedModifiers, HotkeySequence.KeyModifiers triggeringModifiers)
            {
                if (triggeringModifiers == KeyModifiers.None)
                {
                    if (pressedModifiers == KeyModifiers.None)
                        return true;
                    else
                        return false;
                }

                return ((pressedModifiers & triggeringModifiers) != 0); // if any of the modifiers in triggeringModifiers is pressed return true                
            }
        }
        // button definitions
        Button gridButton;
        Button forwardButton;
        Button backwardButton;
        Button leftButton;
        Button rightButton;
        Button rotateLeftButton;
        Button rotateRightButton;
        Button upstairsButton;
        Button downstairsButton;

        // definitions of hotkey sequences
        readonly HotkeySequence HotkeySequence_CloseMap = new HotkeySequence(KeyCode.M, HotkeySequence.KeyModifiers.None);        
        readonly HotkeySequence HotkeySequence_SwitchAutomapGridMode = new HotkeySequence(KeyCode.Space, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_ResetView = new HotkeySequence(KeyCode.Backspace, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_ResetRotationPivotAxisView = new HotkeySequence(KeyCode.Backspace, HotkeySequence.KeyModifiers.LeftControl | HotkeySequence.KeyModifiers.RightControl);
        readonly HotkeySequence HotkeySequence_SwitchFocusToNextBeaconObject = new HotkeySequence(KeyCode.Tab, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_SwitchToNextAutomapRenderMode = new HotkeySequence(KeyCode.Return, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_SwitchToAutomapRenderModeCutout = new HotkeySequence(KeyCode.F2, HotkeySequence.KeyModifiers.None);        
        readonly HotkeySequence HotkeySequence_SwitchToAutomapRenderModeWireframe = new HotkeySequence(KeyCode.F3, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_SwitchToAutomapRenderModeTransparent = new HotkeySequence(KeyCode.F4, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_SwitchToAutomapBackgroundOriginal = new HotkeySequence(KeyCode.F5, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_SwitchToAutomapBackgroundAlternative1 = new HotkeySequence(KeyCode.F6, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_SwitchToAutomapBackgroundAlternative2 = new HotkeySequence(KeyCode.F7, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_SwitchToAutomapBackgroundAlternative3 = new HotkeySequence(KeyCode.F8, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_MoveLeft = new HotkeySequence(KeyCode.LeftArrow, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_MoveRight = new HotkeySequence(KeyCode.RightArrow, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_MoveForward = new HotkeySequence(KeyCode.UpArrow, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_MoveBackward = new HotkeySequence(KeyCode.DownArrow, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_MoveRotationPivotAxisLeft = new HotkeySequence(KeyCode.LeftArrow, HotkeySequence.KeyModifiers.LeftControl | HotkeySequence.KeyModifiers.RightControl);
        readonly HotkeySequence HotkeySequence_MoveRotationPivotAxisRight = new HotkeySequence(KeyCode.RightArrow, HotkeySequence.KeyModifiers.LeftControl | HotkeySequence.KeyModifiers.RightControl);
        readonly HotkeySequence HotkeySequence_MoveRotationPivotAxisForward = new HotkeySequence(KeyCode.UpArrow, HotkeySequence.KeyModifiers.LeftControl | HotkeySequence.KeyModifiers.RightControl);
        readonly HotkeySequence HotkeySequence_MoveRotationPivotAxisBackward = new HotkeySequence(KeyCode.DownArrow, HotkeySequence.KeyModifiers.LeftControl | HotkeySequence.KeyModifiers.RightControl);
        readonly HotkeySequence HotkeySequence_RotateLeft = new HotkeySequence(KeyCode.LeftArrow, HotkeySequence.KeyModifiers.LeftAlt | HotkeySequence.KeyModifiers.RightAlt);
        readonly HotkeySequence HotkeySequence_RotateRight = new HotkeySequence(KeyCode.RightArrow, HotkeySequence.KeyModifiers.LeftAlt | HotkeySequence.KeyModifiers.RightAlt);
        readonly HotkeySequence HotkeySequence_RotateCameraLeft = new HotkeySequence(KeyCode.LeftArrow, HotkeySequence.KeyModifiers.LeftShift| HotkeySequence.KeyModifiers.RightShift);
        readonly HotkeySequence HotkeySequence_RotateCameraRight = new HotkeySequence(KeyCode.RightArrow, HotkeySequence.KeyModifiers.LeftShift | HotkeySequence.KeyModifiers.RightShift);
        readonly HotkeySequence HotkeySequence_RotateCameraOnCameraYZplaneAroundObjectUp = new HotkeySequence(KeyCode.UpArrow, HotkeySequence.KeyModifiers.LeftShift | HotkeySequence.KeyModifiers.RightShift);
        readonly HotkeySequence HotkeySequence_RotateCameraOnCameraYZplaneAroundObjectDown = new HotkeySequence(KeyCode.DownArrow, HotkeySequence.KeyModifiers.LeftShift | HotkeySequence.KeyModifiers.RightShift);
        readonly HotkeySequence HotkeySequence_Upstairs = new HotkeySequence(KeyCode.PageUp, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_Downstairs = new HotkeySequence(KeyCode.PageDown, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_IncreaseSliceLevel = new HotkeySequence(KeyCode.PageUp, HotkeySequence.KeyModifiers.LeftControl | HotkeySequence.KeyModifiers.RightControl);
        readonly HotkeySequence HotkeySequence_DecreaseSliceLevel = new HotkeySequence(KeyCode.PageDown, HotkeySequence.KeyModifiers.LeftControl | HotkeySequence.KeyModifiers.RightControl);
        readonly HotkeySequence HotkeySequence_ZoomIn = new HotkeySequence(KeyCode.KeypadPlus, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_ZoomOut = new HotkeySequence(KeyCode.KeypadMinus, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_IncreaseCameraFieldOfFiew = new HotkeySequence(KeyCode.KeypadMultiply, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_DecreaseCameraFieldOfFiew = new HotkeySequence(KeyCode.KeypadDivide, HotkeySequence.KeyModifiers.None);

        const string nativeImgName = "AMAP00I0.IMG";
        const string nativeImgNameGrid3D = "AMAP01I0.IMG";

        DaggerfallAutomap daggerfallAutomap = null; // used to communicate with DaggerfallAutomap class

        GameObject gameobjectAutomap = null; // used to hold reference to instance of GameObject "Automap" (which has script Game/DaggerfallAutomap.cs attached)

        Camera cameraAutomap = null; // camera for automap camera

        GameObject gameObjectPlayerAdvanced = null; // used to hold reference to instance of GameObject "PlayerAdvanced"

        public enum AutomapViewMode { View2D = 0, View3D = 1};
        AutomapViewMode automapViewMode = AutomapViewMode.View2D;

        Panel dummyPanelAutomap = null; // used to determine correct render panel position
        Panel panelRenderAutomap = null; // level geometry is rendered into this panel
        Rect oldPositionNativePanel;
        Vector2 oldMousePosition; // old mouse position used to determine offset of mouse movement since last time used for for drag and drop functionality

        Panel dummyPanelCompass = null; // used to determine correct compass position

        // these boolean flags are used to indicate which mouse button was pressed over which gui button/element - these are set in the event callbacks
        bool leftMouseClickedOnPanelAutomap = false; // used for debug teleport mode clicks
        bool leftMouseDownOnPanelAutomap = false;
        bool rightMouseDownOnPanelAutomap = false;
        bool leftMouseDownOnForwardButton = false;
        bool rightMouseDownOnForwardButton = false;
        bool leftMouseDownOnBackwardButton = false;
        bool rightMouseDownOnBackwardButton = false;
        bool leftMouseDownOnLeftButton = false;
        bool rightMouseDownOnLeftButton = false;
        bool leftMouseDownOnRightButton = false;
        bool rightMouseDownOnRightButton = false;
        bool leftMouseDownOnRotateLeftButton = false;
        bool rightMouseDownOnRotateLeftButton = false;
        bool leftMouseDownOnRotateRightButton = false;
        bool rightMouseDownOnRotateRightButton = false;
        bool leftMouseDownOnUpstairsButton = false;
        bool leftMouseDownOnDownstairsButton = false;
        bool rightMouseDownOnUpstairsButton = false;
        bool rightMouseDownOnDownstairsButton = false;
        bool alreadyInMouseDown = false;
        bool alreadyInRightMouseDown = false;
        bool inDragMode() { return leftMouseDownOnPanelAutomap || rightMouseDownOnPanelAutomap; }

        Texture2D nativeTexture; // background image will be stored in this Texture2D

        Color[] pixelsGrid2D; // grid button texture for 2D view image will be stored in here
        Color[] pixelsGrid3D; // grid button texture for 3D view image will be stored in here

        Color[] backgroundOriginal; // texture with orignial background will be stored in here
        Color[] backgroundAlternative1; // texture with first alternative background will be stored in here
        Color[] backgroundAlternative2; // texture with second alternative background will be stored in here
        Color[] backgroundAlternative3; // texture with third alternative background will be stored in here

        HUDCompass compass = null;

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

        // independent rotation pivot axis position for both 2D and 3D view
        Vector3 rotationPivotAxisPositionViewFromTop;
        Vector3 rotationPivotAxisPositionView3D;

        bool isSetup = false;

        public DaggerfallAutomapWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        /// <summary>
        /// initial window setup of the automap window
        /// </summary>
        protected override void Setup()
        {           
            ImgFile imgFile = null;
            DFBitmap bitmap = null;

            if (isSetup) // don't setup twice!
                return;

            initGlobalResources(); // initialize gameobjectAutomap, daggerfallAutomap and layerAutomap

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
            Texture2D nativeTextureGrid3D = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.ARGB32, false);
            nativeTextureGrid3D.SetPixels32(imgFile.GetColor32(bitmap, 0));
            nativeTextureGrid3D.Apply(false, false); // make readable
            nativeTextureGrid3D.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            if (!nativeTextureGrid3D)
                throw new Exception("DaggerfallAutomapWindow: Could not load native texture (AMAP01I0.IMG).");
            pixelsGrid3D = nativeTextureGrid3D.GetPixels(0, 0, 27, 19);

            // Cut out 2D View Grid graphics from background image
            pixelsGrid2D = nativeTexture.GetPixels(78, nativeTexture.height - 171 - 19, 27, 19);

            // store background graphics from from background image
            backgroundOriginal = nativeTexture.GetPixels(0, 29, nativeTexture.width, nativeTexture.height - 29);

            backgroundAlternative1 = new Color[backgroundOriginal.Length];
            for (int i = 0; i < backgroundOriginal.Length; ++i)
            {
                backgroundAlternative1[i].r = 0.0f;
                backgroundAlternative1[i].g = 0.0f;
                backgroundAlternative1[i].b = 0.0f;
                backgroundAlternative1[i].a = 1.0f;
            }

            backgroundAlternative2 = new Color[backgroundOriginal.Length];
            for (int i = 0; i < backgroundOriginal.Length; ++i)
            {
                backgroundAlternative2[i].r = 0.2f;
                backgroundAlternative2[i].g = 0.1f;
                backgroundAlternative2[i].b = 0.3f;
                backgroundAlternative2[i].a = 1.0f;
            }

            backgroundAlternative3 = new Color[backgroundOriginal.Length];
            for (int i = 0; i < backgroundOriginal.Length; ++i)
            {
                backgroundAlternative3[i].r = 0.3f;
                backgroundAlternative3[i].g = 0.1f;
                backgroundAlternative3[i].b = 0.2f;
                backgroundAlternative3[i].a = 1.0f;
            }

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            oldPositionNativePanel = NativePanel.Rectangle;

            // dummyPanelAutomap is used to get correct size for panelRenderAutomap
            Rect rectDummyPanelAutomap = new Rect();
            rectDummyPanelAutomap.position = new Vector2(1, 1);
            rectDummyPanelAutomap.size = new Vector2(318, 169);

            dummyPanelAutomap = DaggerfallUI.AddPanel(rectDummyPanelAutomap, NativePanel);

            // Setup automap render panel (into this the level geometry is rendered) - use dummyPanelAutomap to get size
            Rect positionPanelRenderAutomap = dummyPanelAutomap.Rectangle;            
            panelRenderAutomap = DaggerfallUI.AddPanel(positionPanelRenderAutomap, ParentPanel);
            panelRenderAutomap.AutoSize = AutoSizeModes.None;
            
            panelRenderAutomap.OnMouseScrollUp += PanelAutomap_OnMouseScrollUp;
            panelRenderAutomap.OnMouseScrollDown += PanelAutomap_OnMouseScrollDown;
            panelRenderAutomap.OnMouseDown += PanelAutomap_OnMouseDown;
            panelRenderAutomap.OnMouseUp += PanelAutomap_OnMouseUp;
            panelRenderAutomap.OnRightMouseDown += PanelAutomap_OnRightMouseDown;
            panelRenderAutomap.OnRightMouseUp += PanelAutomap_OnRightMouseUp;

            // Grid button (toggle 2D <-> 3D view)
            gridButton = DaggerfallUI.AddButton(new Rect(78, 171, 27, 19), NativePanel);
            gridButton.OnMouseClick += GridButton_OnMouseClick;
            gridButton.OnRightMouseClick += GridButton_OnRightMouseClick;
            gridButton.OnMouseScrollUp += GridButton_OnMouseScrollUp;
            gridButton.OnMouseScrollDown += GridButton_OnMouseScrollDown;
            gridButton.ToolTip = defaultToolTip;
            gridButton.ToolTipText = "left click: switch between 2D top view and 3D view (hotkey: space key)\rright click: reset rotation center to player position (hotkey: control+backspace)\rmouse wheel up while over this button: increase perspective (only 3D mode)\rmouse wheel down while over this button: decrease perspective (only 3D mode)";
            gridButton.ToolTip.ToolTipDelay = toolTipDelay;

            // forward button
            forwardButton = DaggerfallUI.AddButton(new Rect(105, 171, 21, 19), NativePanel);
            forwardButton.OnMouseDown += ForwardButton_OnMouseDown;
            forwardButton.OnMouseUp += ForwardButton_OnMouseUp;
            forwardButton.OnRightMouseDown += ForwardButton_OnRightMouseDown;
            forwardButton.OnRightMouseUp += ForwardButton_OnRightMouseUp;
            forwardButton.ToolTip = defaultToolTip;
            forwardButton.ToolTipText = "left click: move viewpoint forward (hotkey: up arrow)\rright click: move rotation center axis forward (hotkey: control+up arrow)";
            forwardButton.ToolTip.ToolTipDelay = toolTipDelay;

            // backward button
            backwardButton = DaggerfallUI.AddButton(new Rect(126, 171, 21, 19), NativePanel);
            backwardButton.OnMouseDown += BackwardButton_OnMouseDown;
            backwardButton.OnMouseUp += BackwardButton_OnMouseUp;
            backwardButton.OnRightMouseDown += BackwardButton_OnRightMouseDown;
            backwardButton.OnRightMouseUp += BackwardButton_OnRightMouseUp;
            backwardButton.ToolTip = defaultToolTip;
            backwardButton.ToolTipText = "left click: move viewpoint backwards (hotkey: down arrow)\rright click: move rotation center axis backwards (hotkey: control+down arrow)";
            backwardButton.ToolTip.ToolTipDelay = toolTipDelay;

            // left button
            leftButton = DaggerfallUI.AddButton(new Rect(149, 171, 21, 19), NativePanel);
            leftButton.OnMouseDown += LeftButton_OnMouseDown;
            leftButton.OnMouseUp += LeftButton_OnMouseUp;
            leftButton.OnRightMouseDown += LeftButton_OnRightMouseDown;
            leftButton.OnRightMouseUp += LeftButton_OnRightMouseUp;
            leftButton.ToolTip = defaultToolTip;
            leftButton.ToolTipText = "left click: move viewpoint to the left (hotkey: left arrow)\rright click: move rotation center axis to the left (hotkey: control+left arrow)";
            leftButton.ToolTip.ToolTipDelay = toolTipDelay;

            // right button
            rightButton = DaggerfallUI.AddButton(new Rect(170, 171, 21, 19), NativePanel);
            rightButton.OnMouseDown += RightButton_OnMouseDown;
            rightButton.OnMouseUp += RightButton_OnMouseUp;
            rightButton.OnRightMouseDown += RightButton_OnRightMouseDown;
            rightButton.OnRightMouseUp += RightButton_OnRightMouseUp;
            rightButton.ToolTip = defaultToolTip;
            rightButton.ToolTipText = "left click: move viewpoint to the right (hotkey: right arrow)\rright click: move rotation center axis to the right (hotkey: control+right arrow)";
            rightButton.ToolTip.ToolTipDelay = toolTipDelay;

            // rotate left button
            rotateLeftButton = DaggerfallUI.AddButton(new Rect(193, 171, 21, 19), NativePanel);
            rotateLeftButton.OnMouseDown += RotateLeftButton_OnMouseDown;
            rotateLeftButton.OnMouseUp += RotateLeftButton_OnMouseUp;
            rotateLeftButton.OnRightMouseDown += RotateLeftButton_OnRightMouseDown;
            rotateLeftButton.OnRightMouseUp += RotateLeftButton_OnRightMouseUp;
            rotateLeftButton.ToolTip = defaultToolTip;
            rotateLeftButton.ToolTipText = "left click: rotate dungeon model to the left (hotkey: alt+right arrow)\rright click: rotate camera view to the left (hotkey: shift+right arrow)";
            rotateLeftButton.ToolTip.ToolTipDelay = toolTipDelay;

            // rotate right button
            rotateRightButton = DaggerfallUI.AddButton(new Rect(214, 171, 21, 19), NativePanel);
            rotateRightButton.OnMouseDown += RotateRightButton_OnMouseDown;
            rotateRightButton.OnMouseUp += RotateRightButton_OnMouseUp;
            rotateRightButton.OnRightMouseDown += RotateRightButton_OnRightMouseDown;
            rotateRightButton.OnRightMouseUp += RotateRightButton_OnRightMouseUp;
            rotateRightButton.ToolTip = defaultToolTip;
            rotateRightButton.ToolTipText = "left click: rotate dungeon model to the right (hotkey: alt+right arrow)\rright click: rotate camera view to the right (hotkey: shift+right arrow)";
            rotateRightButton.ToolTip.ToolTipDelay = toolTipDelay;

            // upstairs button
            upstairsButton = DaggerfallUI.AddButton(new Rect(237, 171, 21, 19), NativePanel);
            upstairsButton.OnMouseDown += UpstairsButton_OnMouseDown;
            upstairsButton.OnMouseUp += UpstairsButton_OnMouseUp;
            upstairsButton.OnRightMouseDown += UpstairsButton_OnRightMouseDown;
            upstairsButton.OnRightMouseUp += UpstairsButton_OnRightMouseUp;
            upstairsButton.ToolTip = defaultToolTip;
            upstairsButton.ToolTipText = "left click: increase viewpoint (hotkey: page up)\rright click: increase slice level (hotkey: control+page up)\r\rhint: different render modes may show hidden geometry:\rhotkey F2: cutout mode\rhotkey F3: wireframe mode\rhotkey F4: transparent mode\rswitch between modes with return key";
            upstairsButton.ToolTip.ToolTipDelay = toolTipDelay;

            // downstairs button
            downstairsButton = DaggerfallUI.AddButton(new Rect(258, 171, 21, 19), NativePanel);
            downstairsButton.OnMouseDown += DownstairsButton_OnMouseDown;
            downstairsButton.OnMouseUp += DownstairsButton_OnMouseUp;
            downstairsButton.OnRightMouseDown += DownstairsButton_OnRightMouseDown;
            downstairsButton.OnRightMouseUp += DownstairsButton_OnRightMouseUp;
            downstairsButton.ToolTip = defaultToolTip;
            downstairsButton.ToolTipText = "left click: decrease viewpoint (hotkey: page down)\rright click: decrease slice level (hotkey: control+page down)\r\rhint: different render modes may show hidden geometry:\rhotkey F2: cutout mode\rhotkey F3: wireframe mode\rhotkey F4: transparent mode\rswitch between modes with return key";
            downstairsButton.ToolTip.ToolTipDelay = toolTipDelay;

            // Exit button
            Button exitButton = DaggerfallUI.AddButton(new Rect(281, 171, 28, 19), NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            // dummyPanelCompass is used to get correct size for compass
            Rect rectDummyPanelCompass = new Rect();
            rectDummyPanelCompass.position = new Vector2(3, 172);
            rectDummyPanelCompass.size = new Vector2(76, 17);
            dummyPanelCompass = DaggerfallUI.AddPanel(rectDummyPanelCompass, NativePanel);
            dummyPanelCompass.OnMouseClick += Compass_OnMouseClick;
            dummyPanelCompass.OnRightMouseClick += Compass_OnRightMouseClick;
            dummyPanelCompass.ToolTip = defaultToolTip;
            dummyPanelCompass.ToolTipText = "left click: toggle focus (hotkey: tab)\rred beacon: player, green beacon: entrance, blue beacon: rotation center\r\rright click: reset view (hotkey: backspace)";
            dummyPanelCompass.ToolTip.ToolTipDelay = toolTipDelay;

            // compass            
            compass = new HUDCompass();
            Vector2 scale = NativePanel.LocalScale;
            compass.Position = dummyPanelCompass.Rectangle.position;
            compass.Scale = scale;
            NativePanel.Components.Add(compass);

            isSetup = true;
        }

        /// <summary>
        /// called when automap window is pushed - resets automap settings to default settings and signals DaggerfallAutomap class
        /// </summary>
        public override void OnPush()
        {
            initGlobalResources(); // initialize gameobjectAutomap, daggerfallAutomap and layerAutomap

            if (!isSetup) // if Setup() has not run, run it now
                Setup();

            daggerfallAutomap.IsOpenAutomap = true; // signal DaggerfallAutomap script that automap is open and it should do its stuff in its Update() function            

            daggerfallAutomap.updateAutomapStateOnWindowPush(); // signal DaggerfallAutomap script that automap window was closed and that it should update its state (updates player marker arrow)

            // get automap camera
            cameraAutomap = daggerfallAutomap.CameraAutomap;

            // create automap render texture and Texture2D used in conjuction with automap camera to render automap level geometry and display it in panel
            Rect positionPanelRenderAutomap = dummyPanelAutomap.Rectangle;
            createAutomapTextures((int)positionPanelRenderAutomap.width, (int)positionPanelRenderAutomap.height);

            switch (automapViewMode)
            {
                case AutomapViewMode.View2D: default:
                    cameraAutomap.fieldOfView = fieldOfViewCameraMode2D;
                    break;
                case AutomapViewMode.View3D:
                    cameraAutomap.fieldOfView = fieldOfViewCameraMode3D;
                    break;
            }

            if (compass != null)
            {
                compass.CompassCamera = cameraAutomap;
            }

            // reset values to default whenever automap window is opened
            resetRotationPivotAxisPosition(); // reset rotation pivot axis
            daggerfallAutomap.SlicingBiasY = defaultSlicingBiasY; // reset slicing y-bias

            if (daggerfallAutomap.ResetAutomapSettingsSignalForExternalScript == true) // signaled to reset automap settings
            {
                // get initial values for camera transform for view from top
                resetCameraTransformViewFromTop();
                saveCameraTransformViewFromTop();

                // get initial values for camera transform for 3D view
                resetCameraTransformView3D();
                saveCameraTransformView3D();

                // reset values to default whenever player enters building or dungeon
                resetCameraPosition();
                fieldOfViewCameraMode3D = defaultFieldOfViewCameraMode3D;

                daggerfallAutomap.ResetAutomapSettingsSignalForExternalScript = false; // indicate the settings were reset
            }
            else
            {
                // backup view mode
                AutomapViewMode backupValueAutomapViewMode = automapViewMode;

                // focus player in 2D view mode - but keep old camera orientation of 2D view mode camera transform                
                automapViewMode = AutomapViewMode.View2D; // need to change view mode so that SwitchFocusToGameObject() does the correct thing
                restoreOldCameraTransformViewFromTop();
                SwitchFocusToGameObject(gameObjectPlayerAdvanced);
                saveCameraTransformViewFromTop();

                // focus player in 3D view mode - but keep old camera orientation of 3D view mode camera transform
                automapViewMode = AutomapViewMode.View3D; // need to change view mode so that SwitchFocusToGameObject() does the correct thing
                restoreOldCameraTransformView3D();
                SwitchFocusToGameObject(gameObjectPlayerAdvanced);
                saveCameraTransformView3D();

                // restore view mode
                automapViewMode = backupValueAutomapViewMode;

                switch (automapViewMode)
                {
                    case AutomapViewMode.View2D:
                    default:
                        restoreOldCameraTransformViewFromTop();
                        break;
                    case AutomapViewMode.View3D:
                        restoreOldCameraTransformView3D();
                        break;
                }
            }

            // and update the automap view
            updateAutomapView();
        }

        /// <summary>
        /// called when automap window is popped - destroys resources and signals DaggerfallAutomap class
        /// </summary>
        public override void OnPop()
        {
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                default:
                    saveCameraTransformViewFromTop();
                    break;
                case AutomapViewMode.View3D:
                    saveCameraTransformView3D();
                    break;
            }

            daggerfallAutomap.IsOpenAutomap = false; // signal DaggerfallAutomap script that automap was closed

            // destroy the other gameobjects as well so they don't use system resources

            cameraAutomap.targetTexture = null;

            if (renderTextureAutomap != null)
            {
                UnityEngine.Object.Destroy(renderTextureAutomap);
            }

            if (textureAutomap != null)
            {
                UnityEngine.Object.Destroy(textureAutomap);
            }

            daggerfallAutomap.updateAutomapStateOnWindowPop(); // signal DaggerfallAutomap script that automap window was closed
        }

        /// <summary>
        /// reacts on left/right mouse button down states over different automap buttons and other GUI elements
        /// handles resizing of NativePanel as well
        /// </summary>
        public override void Update()
        {
            base.Update();
            resizeGUIelementsOnDemand();

            HotkeySequence.KeyModifiers keyModifiers = HotkeySequence.getKeyModifiers(Input.GetKey(KeyCode.LeftControl), Input.GetKey(KeyCode.RightControl), Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.RightShift), Input.GetKey(KeyCode.LeftAlt), Input.GetKey(KeyCode.RightAlt));

            // debug teleport mode action
            if (
                (daggerfallAutomap.DebugTeleportMode == true) &&
                leftMouseClickedOnPanelAutomap && // make sure click happened in panel area
                Input.GetMouseButtonDown(0) && // make sure click was issued in this frame
                ((Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift)) || (Input.GetKey(KeyCode.RightControl) && Input.GetKey(KeyCode.RightShift)))
               )
            {
                //Vector2 mousePosition = new Vector2((Input.mousePosition.x / Screen.width) * panelRenderAutomap.Size.x, (Input.mousePosition.y / Screen.height) * panelRenderAutomap.Size.y);
                Vector2 mousePosition = panelRenderAutomap.ScaledMousePosition;
                mousePosition.y = panelRenderAutomap.Size.y - mousePosition.y;
                daggerfallAutomap.tryTeleportPlayerToDungeonSegmentAtScreenPosition(mousePosition);
                updateAutomapView();
            }
            
            // check hotkeys and assign actions
            if (Input.GetKeyDown(HotkeySequence_CloseMap.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_CloseMap.modifiers))
            {                
                CloseWindow();
                Input.ResetInputAxes(); // prevents automap window to reopen immediately after closing
            }
            if (Input.GetKeyDown(HotkeySequence_SwitchAutomapGridMode.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchAutomapGridMode.modifiers))
            {
                ActionChangeAutomapGridMode();
            }
            if (Input.GetKeyDown(HotkeySequence_ResetView.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_ResetView.modifiers))
            {
                ActionResetView();
            }
            if (Input.GetKeyDown(HotkeySequence_ResetRotationPivotAxisView.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_ResetRotationPivotAxisView.modifiers))
            {
                ActionResetRotationPivotAxis();
            }            
            if (Input.GetKeyDown(HotkeySequence_SwitchFocusToNextBeaconObject.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchFocusToNextBeaconObject.modifiers))
            {
                ActionSwitchFocusToNextBeaconObject();
            }
            if (Input.GetKeyDown(HotkeySequence_SwitchToNextAutomapRenderMode.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToNextAutomapRenderMode.modifiers))
            {
                ActionSwitchToNextAutomapRenderMode();
            }

            if (Input.GetKeyDown(HotkeySequence_SwitchToAutomapRenderModeTransparent.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToAutomapRenderModeTransparent.modifiers))
            {
                ActionSwitchToAutomapRenderModeTransparent();
            }
            if (Input.GetKeyDown(HotkeySequence_SwitchToAutomapRenderModeWireframe.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToAutomapRenderModeWireframe.modifiers))
            {
                ActionSwitchToAutomapRenderModeWireframe();
            }
            if (Input.GetKeyDown(HotkeySequence_SwitchToAutomapRenderModeCutout.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToAutomapRenderModeCutout.modifiers))
            {
                ActionSwitchToAutomapRenderModeCutout();
            }

            if (Input.GetKeyDown(HotkeySequence_SwitchToAutomapBackgroundOriginal.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToAutomapBackgroundOriginal.modifiers))
            {
                ActionSwitchToAutomapBackgroundOriginal();
            }
            if (Input.GetKeyDown(HotkeySequence_SwitchToAutomapBackgroundAlternative1.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToAutomapBackgroundAlternative1.modifiers))
            {
                ActionSwitchToAutomapBackgroundAlternative1();
            }
            if (Input.GetKeyDown(HotkeySequence_SwitchToAutomapBackgroundAlternative2.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToAutomapBackgroundAlternative2.modifiers))
            {
                ActionSwitchToAutomapBackgroundAlternative2();
            }
            if (Input.GetKeyDown(HotkeySequence_SwitchToAutomapBackgroundAlternative3.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToAutomapBackgroundAlternative3.modifiers))
            {
                ActionSwitchToAutomapBackgroundAlternative3();
            }


            if (Input.GetKey(HotkeySequence_MoveForward.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MoveForward.modifiers))
            {
                ActionMoveForward();
            }
            if (Input.GetKey(HotkeySequence_MoveBackward.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MoveBackward.modifiers))
            {
                ActionMoveBackward();
            }
            if (Input.GetKey(HotkeySequence_MoveLeft.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MoveLeft.modifiers))
            {
                ActionMoveLeft();
            }
            if (Input.GetKey(HotkeySequence_MoveRight.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MoveRight.modifiers))
            {
                ActionMoveRight();
            }
            if (Input.GetKey(HotkeySequence_MoveRotationPivotAxisForward.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MoveRotationPivotAxisForward.modifiers))
            {
                ActionMoveRotationPivotAxisForward();
            }
            if (Input.GetKey(HotkeySequence_MoveRotationPivotAxisBackward.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MoveRotationPivotAxisBackward.modifiers))
            {
                ActionMoveRotationPivotAxisBackward();
            }
            if (Input.GetKey(HotkeySequence_MoveRotationPivotAxisLeft.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MoveRotationPivotAxisLeft.modifiers))
            {
                ActionMoveRotationPivotAxisLeft();
            }
            if (Input.GetKey(HotkeySequence_MoveRotationPivotAxisRight.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MoveRotationPivotAxisRight.modifiers))
            {
                ActionMoveRotationPivotAxisRight();
            }
            if (Input.GetKey(HotkeySequence_RotateLeft.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_RotateLeft.modifiers))
            {
                ActionRotateLeft();
            }
            if (Input.GetKey(HotkeySequence_RotateRight.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_RotateRight.modifiers))
            {
                ActionRotateRight();
            }
            if (Input.GetKey(HotkeySequence_RotateCameraLeft.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_RotateCameraLeft.modifiers))
            {
                ActionRotateCamera(rotateCameraSpeed);
            }
            if (Input.GetKey(HotkeySequence_RotateCameraRight.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_RotateCameraRight.modifiers))
            {
                ActionRotateCamera(-rotateCameraSpeed);
            }
            if (Input.GetKey(HotkeySequence_RotateCameraOnCameraYZplaneAroundObjectUp.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_RotateCameraOnCameraYZplaneAroundObjectUp.modifiers))
            {
                ActionrotateCameraOnCameraYZplaneAroundObject(rotateCameraOnCameraYZplaneAroundObjectSpeedInView3D);
            }
            if (Input.GetKey(HotkeySequence_RotateCameraOnCameraYZplaneAroundObjectDown.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_RotateCameraOnCameraYZplaneAroundObjectDown.modifiers))
            {
                ActionrotateCameraOnCameraYZplaneAroundObject(-rotateCameraOnCameraYZplaneAroundObjectSpeedInView3D);
            }
            if (Input.GetKey(HotkeySequence_Upstairs.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_Upstairs.modifiers))
            {
                ActionMoveUpstairs();
            }
            if (Input.GetKey(HotkeySequence_Downstairs.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_Downstairs.modifiers))
            {
                ActionMoveDownstairs();
            }
            if (Input.GetKey(HotkeySequence_IncreaseSliceLevel.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_IncreaseSliceLevel.modifiers))
            {
                ActionIncreaseSliceLevel();
            }
            if (Input.GetKey(HotkeySequence_DecreaseSliceLevel.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_DecreaseSliceLevel.modifiers))
            {
                ActionDecreaseSliceLevel();
            }
            if (Input.GetKey(HotkeySequence_ZoomIn.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_ZoomIn.modifiers))
            {
                ActionZoomIn(zoomSpeed * Time.unscaledDeltaTime);
            }
            if (Input.GetKey(HotkeySequence_ZoomOut.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_ZoomOut.modifiers))
            {
                ActionZoomOut(zoomSpeed * Time.unscaledDeltaTime);
            }
            if (Input.GetKey(HotkeySequence_IncreaseCameraFieldOfFiew.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_IncreaseCameraFieldOfFiew.modifiers))
            {
                ActionIncreaseCameraFieldOfView();
            }
            if (Input.GetKey(HotkeySequence_DecreaseCameraFieldOfFiew.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_DecreaseCameraFieldOfFiew.modifiers))
            {
                ActionDecreaseCameraFieldOfView();
            }  

            // check mouse input and assign actions
            if (leftMouseDownOnPanelAutomap)
            {
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

                float dragSpeedCompensated;
                switch (automapViewMode)
                {
                    case AutomapViewMode.View2D:
                    default:
                        dragSpeedCompensated = dragSpeedInTopView * Vector3.Magnitude(Camera.main.transform.position - cameraAutomap.transform.position);
                        break;
                    case AutomapViewMode.View3D:
                        dragSpeedCompensated = dragSpeedInView3D * Vector3.Magnitude(Camera.main.transform.position - cameraAutomap.transform.position);
                        break;
                }
                
                Vector2 bias = mousePosition - oldMousePosition;
                Vector3 translation = -cameraAutomap.transform.right * dragSpeedCompensated * bias.x + cameraAutomap.transform.up * dragSpeedCompensated * bias.y;
                cameraAutomap.transform.position += translation;
                updateAutomapView();
                oldMousePosition = mousePosition;
            }

            if (rightMouseDownOnPanelAutomap)
            {
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

                Vector2 bias = mousePosition - oldMousePosition;

                switch (automapViewMode)
                {
                    case AutomapViewMode.View2D:
                    default:
                        ActionRotateCamera(+dragRotateSpeedInTopView * bias.x);
                        break;
                    case AutomapViewMode.View3D:                        
                        ActionRotate(dragRotateSpeedInView3D * bias.x);                        
                        ActionrotateCameraOnCameraYZplaneAroundObject(-dragRotateCameraOnCameraYZplaneAroundObjectSpeedInView3D * bias.y);
                        break;
                }

                updateAutomapView();
                oldMousePosition = mousePosition;
            }

            if (leftMouseDownOnForwardButton)
            {
                ActionMoveForward();
            }

            if (rightMouseDownOnForwardButton)
            {
                ActionMoveRotationPivotAxisForward();
            }

            if (leftMouseDownOnBackwardButton)
            {
                ActionMoveBackward();
            }

            if (rightMouseDownOnBackwardButton)
            {
                ActionMoveRotationPivotAxisBackward();
            }

            if (leftMouseDownOnLeftButton)
            {
                ActionMoveLeft();
            }

            if (rightMouseDownOnLeftButton)
            {
                ActionMoveRotationPivotAxisLeft();
            }

            if (leftMouseDownOnRightButton)
            {
                ActionMoveRight();
            }

            if (rightMouseDownOnRightButton)
            {
                ActionMoveRotationPivotAxisRight();
            }


            if (leftMouseDownOnRotateLeftButton)
            {
                ActionRotateLeft();
            }

            if (leftMouseDownOnRotateRightButton)
            {
                ActionRotateRight();
            }

            if (rightMouseDownOnRotateLeftButton)
            {
                ActionRotateCamera(rotateCameraSpeed);
            }

            if (rightMouseDownOnRotateRightButton)
            {
                ActionRotateCamera(-rotateCameraSpeed);
            }

            if (leftMouseDownOnUpstairsButton)
            {
                ActionMoveUpstairs();
            }

            if (leftMouseDownOnDownstairsButton)
            {
                ActionMoveDownstairs();
            }

            if (rightMouseDownOnUpstairsButton)
            {
                ActionIncreaseSliceLevel();
            }

            if (rightMouseDownOnDownstairsButton)
            {
                ActionDecreaseSliceLevel();
            }
        }        

        #region Private Methods

        /// <summary>
        /// tests for availability and initializes class resources like GameObject for automap, DaggerfallAutomap class and layerAutomap
        /// </summary>
        private void initGlobalResources()
        {
            if (!gameobjectAutomap)
            {
                gameobjectAutomap = GameObject.Find("Automap/InteriorAutomap");
                if (gameobjectAutomap == null)
                {
                    DaggerfallUnity.LogMessage("GameObject \"Automap/InteriorAutomap\" missing! Create a GameObject called \"Automap\" in root of hierarchy and add a GameObject \"InternalAutomap\" to it, to this add script Game/DaggerfallAutomap!\"", true);
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

            gameObjectPlayerAdvanced = GameObject.Find("PlayerAdvanced");
            if (!gameObjectPlayerAdvanced)
            {
                DaggerfallUnity.LogMessage("GameObject \"PlayerAdvanced\" not found! in script DaggerfallAutomap (in function Awake())", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }
        }

        /// <summary>
        /// resizes GUI elements automap render panel (and the needed RenderTexture and Texture2D) and compass
        /// </summary>
        private void resizeGUIelementsOnDemand()
        {
            if (oldPositionNativePanel != NativePanel.Rectangle)
            {
                // get panelRenderAutomap position and size from dummyPanelAutomap rectangle
                panelRenderAutomap.Position = dummyPanelAutomap.Rectangle.position;
                panelRenderAutomap.Size = new Vector2(dummyPanelAutomap.InteriorWidth, dummyPanelAutomap.InteriorHeight);

                //Debug.Log(String.Format("dummy panel size: {0}, {1}; {2}, {3}; {4}, {5}; {6}, {7}\n", NativePanel.InteriorWidth, NativePanel.InteriorHeight, ParentPanel.InteriorWidth, ParentPanel.InteriorHeight, dummyPanelAutomap.InteriorWidth, dummyPanelAutomap.InteriorHeight, parentPanel.InteriorWidth, parentPanel.InteriorHeight));
                //Debug.Log(String.Format("dummy panel pos: {0}, {1}; {2}, {3}; {4}, {5}; {6}, {7}\n", NativePanel.Rectangle.xMin, NativePanel.Rectangle.yMin, ParentPanel.Rectangle.xMin, ParentPanel.Rectangle.yMin, dummyPanelAutomap.Rectangle.xMin, dummyPanelAutomap.Rectangle.yMin, parentPanel.Rectangle.xMin, parentPanel.Rectangle.yMin));
                Vector2 positionPanelRenderAutomap = new Vector2(dummyPanelAutomap.InteriorWidth, dummyPanelAutomap.InteriorHeight);
                createAutomapTextures((int)positionPanelRenderAutomap.x, (int)positionPanelRenderAutomap.y);
                updateAutomapView();

                // get compass position from dummyPanelCompass rectangle
                Vector2 scale = NativePanel.LocalScale;
                compass.Position = dummyPanelCompass.Rectangle.position;
                compass.Scale = scale;

                oldPositionNativePanel = NativePanel.Rectangle;
            }
        }

        /// <summary>
        /// creates RenderTexture and Texture2D with the required size if it is not present or its old size differs from the expected size
        /// </summary>
        /// <param name="width"> the expected width of the RenderTexture and Texture2D </param>
        /// <param name="height"> the expected height of the RenderTexture and Texture2D </param>
        private void createAutomapTextures(int width, int height)
        {
            if ((!cameraAutomap) || (!renderTextureAutomap) || (oldRenderTextureAutomapWidth != width) || (oldRenderTextureAutomapHeight != height))
            {
                cameraAutomap.targetTexture = null;
                if (renderTextureAutomap)
                    UnityEngine.Object.Destroy(renderTextureAutomap);
                if (textureAutomap)
                    UnityEngine.Object.Destroy(textureAutomap);

                renderTextureAutomap = new RenderTexture(width, height, renderTextureAutomapDepth);
                cameraAutomap.targetTexture = renderTextureAutomap;

                textureAutomap = new Texture2D(renderTextureAutomap.width, renderTextureAutomap.height, TextureFormat.ARGB32, false);

                oldRenderTextureAutomapWidth = width;
                oldRenderTextureAutomapHeight = height;
            }
        }

        /// <summary>
        /// resets the automap camera position for active view mode
        /// </summary>
        private void resetCameraPosition()
        {
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

        /// <summary>
        /// resets the camera transform of the 2D view mode
        /// </summary>
        private void resetCameraTransformViewFromTop()
        {
            cameraAutomap.transform.position = Camera.main.transform.position + Vector3.up * cameraHeightViewFromTop;
            cameraAutomap.transform.LookAt(Camera.main.transform.position);
            saveCameraTransformViewFromTop();
        }

        /// <summary>
        /// resets the camera transform of the 3D view mode
        /// </summary>
        private void resetCameraTransformView3D()
        {
            Vector3 viewDirectionInXZ = Vector3.forward;
            cameraAutomap.transform.position = Camera.main.transform.position - viewDirectionInXZ * cameraBackwardDistance + Vector3.up * cameraHeightView3D;
            cameraAutomap.transform.LookAt(Camera.main.transform.position);
            saveCameraTransformView3D();
        }

        /// <summary>
        /// saves the camera transform of the 2D view mode
        /// </summary>
        private void saveCameraTransformViewFromTop()
        {
            backupCameraPositionViewFromTop = cameraAutomap.transform.position;
            backupCameraRotationViewFromTop = cameraAutomap.transform.rotation;
        }

        /// <summary>
        /// saves the camera transform of the 3D view mode
        /// </summary>
        private void saveCameraTransformView3D()
        {
            backupCameraPositionView3D = cameraAutomap.transform.position;
            backupCameraRotationView3D = cameraAutomap.transform.rotation;
        }

        /// <summary>
        /// restores the camera transform of the 2D view mode
        /// </summary>
        private void restoreOldCameraTransformViewFromTop()
        {
            cameraAutomap.transform.position = backupCameraPositionViewFromTop;
            cameraAutomap.transform.rotation = backupCameraRotationViewFromTop;
        }

        /// <summary>
        /// restores the camera transform of the 3D view mode
        /// </summary>
        private void restoreOldCameraTransformView3D()
        {
            cameraAutomap.transform.position = backupCameraPositionView3D;
            cameraAutomap.transform.rotation = backupCameraRotationView3D;
        }

        /// <summary>
        /// resets the rotation pivot axis position for the 2D view mode
        /// </summary>
        private void resetRotationPivotAxisPositionViewFromTop()
        {
            rotationPivotAxisPositionViewFromTop = gameObjectPlayerAdvanced.transform.position;
            daggerfallAutomap.RotationPivotAxisPosition = rotationPivotAxisPositionViewFromTop;
        }

        /// <summary>
        /// resets the rotation pivot axis position for the 3D view mode
        /// </summary>
        private void resetRotationPivotAxisPositionView3D()
        {
            rotationPivotAxisPositionView3D = gameObjectPlayerAdvanced.transform.position;
            daggerfallAutomap.RotationPivotAxisPosition = rotationPivotAxisPositionView3D;
        }

        /// <summary>
        /// resets the rotation pivot axis position for both view modes
        /// </summary>
        private void resetRotationPivotAxisPosition()
        {
            resetRotationPivotAxisPositionViewFromTop();
            resetRotationPivotAxisPositionView3D();
        }

        /// <summary>
        /// shifts the rotation pivot axis position for the currently selected view mode
        /// </summary>
        /// <param name="translation"> the translation for the shift </param>
        private void shiftRotationPivotAxisPosition(Vector3 translation)
        {
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    rotationPivotAxisPositionViewFromTop += translation;
                    daggerfallAutomap.RotationPivotAxisPosition = rotationPivotAxisPositionViewFromTop;
                    break;
                case AutomapViewMode.View3D:
                    rotationPivotAxisPositionView3D += translation;
                    daggerfallAutomap.RotationPivotAxisPosition = rotationPivotAxisPositionView3D;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// gets the rotation pivot axis position for the currently selected view mode
        /// </summary>
        /// <returns> the position of the rotation pivot axis </returns>
        private Vector3 getRotationPivotAxisPosition()
        {
            Vector3 rotationPivotAxisPosition;
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    rotationPivotAxisPosition = rotationPivotAxisPositionViewFromTop;
                    break;
                case AutomapViewMode.View3D:
                    rotationPivotAxisPosition = rotationPivotAxisPositionView3D;
                    break;
                default:
                    rotationPivotAxisPosition = gameObjectPlayerAdvanced.transform.position;
                    break;
            }
            return (rotationPivotAxisPosition);
        }

        /// <summary>
        /// updates the automap view - signals DaggerfallAutomap class to update and renders the automap level geometry afterwards into the automap render panel
        /// </summary>
        private void updateAutomapView()
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

        #region Actions (Callbacks for Mouse Events and Hotkeys)

        /// <summary>
        /// action for move forward
        /// </summary>
        private void ActionMoveForward()
        {
            Vector3 translation;
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    translation = cameraAutomap.transform.up * scrollForwardBackwardSpeed * Time.unscaledDeltaTime;
                    break;
                case AutomapViewMode.View3D:
                    translation = cameraAutomap.transform.forward * scrollForwardBackwardSpeed * Time.unscaledDeltaTime;
                    translation.y = 0.0f; // comment this out for movement along camera optical axis
                    break;
                default:
                    translation = Vector3.zero;
                    break;
            }
            cameraAutomap.transform.position += translation;
            updateAutomapView();
        }

        /// <summary>
        /// action for move backward
        /// </summary>
        private void ActionMoveBackward()
        {
            Vector3 translation;
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    translation = -cameraAutomap.transform.up * scrollForwardBackwardSpeed * Time.unscaledDeltaTime;
                    break;
                case AutomapViewMode.View3D:
                    translation = -cameraAutomap.transform.forward * scrollForwardBackwardSpeed * Time.unscaledDeltaTime;
                    translation.y = 0.0f; // comment this out for movement along camera optical axis
                    break;
                default:
                    translation = Vector3.zero;
                    break;
            }
            cameraAutomap.transform.position += translation;
            updateAutomapView();
        }

        /// <summary>
        /// action for move left
        /// </summary>
        private void ActionMoveLeft()
        {
            Vector3 translation = -cameraAutomap.transform.right * scrollLeftRightSpeed * Time.unscaledDeltaTime;
            translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector
            cameraAutomap.transform.position += translation;
            updateAutomapView();
        }

        /// <summary>
        /// action for move right
        /// </summary>
        private void ActionMoveRight()
        {
            Vector3 translation = cameraAutomap.transform.right * scrollLeftRightSpeed * Time.unscaledDeltaTime;
            translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector
            cameraAutomap.transform.position += translation;
            updateAutomapView();
        }

        /// <summary>
        /// action for moving rotation pivot axis forward
        /// </summary>
        private void ActionMoveRotationPivotAxisForward()
        {
            Vector3 translation;
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    translation = cameraAutomap.transform.up * moveRotationPivotAxisMarkerForwardBackwardSpeed * Time.unscaledDeltaTime;
                    break;
                case AutomapViewMode.View3D:
                    translation = cameraAutomap.transform.forward * moveRotationPivotAxisMarkerForwardBackwardSpeed * Time.unscaledDeltaTime;
                    //translation.y = 0.0f; // comment this out for movement along camera optical axis
                    break;
                default:
                    translation = Vector3.zero;
                    break;
            }
            shiftRotationPivotAxisPosition(translation);
            updateAutomapView();
        }

        /// <summary>
        /// action for moving rotation pivot axis backward
        /// </summary>
        private void ActionMoveRotationPivotAxisBackward()
        {
            Vector3 translation;
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    translation = -cameraAutomap.transform.up * moveRotationPivotAxisMarkerForwardBackwardSpeed * Time.unscaledDeltaTime;
                    break;
                case AutomapViewMode.View3D:
                    translation = -cameraAutomap.transform.forward * moveRotationPivotAxisMarkerForwardBackwardSpeed * Time.unscaledDeltaTime;
                    //translation.y = 0.0f; // comment this out for movement along camera optical axis
                    break;
                default:
                    translation = Vector3.zero;
                    break;
            }
            shiftRotationPivotAxisPosition(translation);
            updateAutomapView();
        }

        /// <summary>
        /// action for moving rotation pivot axis left
        /// </summary>
        private void ActionMoveRotationPivotAxisLeft()
        {
            Vector3 translation = -cameraAutomap.transform.right * moveRotationPivotAxisMarkerLeftRightSpeed * Time.unscaledDeltaTime;
            //translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector
            shiftRotationPivotAxisPosition(translation);
            updateAutomapView();
        }

        /// <summary>
        /// action for moving rotation pivot axis right
        /// </summary>
        private void ActionMoveRotationPivotAxisRight()
        {
            Vector3 translation = cameraAutomap.transform.right * moveRotationPivotAxisMarkerLeftRightSpeed * Time.unscaledDeltaTime;
            //translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector                
            shiftRotationPivotAxisPosition(translation);
            updateAutomapView();
        }

        /// <summary>
        /// action for rotating model left
        /// </summary>
        private void ActionRotateLeft()
        {
            //Vector3 rotationPivotAxisPosition;
            //switch (automapViewMode)
            //{
            //    case AutomapViewMode.View2D:
            //        rotationPivotAxisPosition = rotationPivotAxisPositionViewFromTop;
            //        break;
            //    case AutomapViewMode.View3D:
            //        rotationPivotAxisPosition = rotationPivotAxisPositionView3D;
            //        break;
            //    default:
            //        rotationPivotAxisPosition = Vector3.zero;
            //        break;
            //}
            //cameraAutomap.transform.RotateAround(rotationPivotAxisPosition, -Vector3.up, -rotateSpeed * Time.unscaledDeltaTime);
            //updateAutomapView();
            ActionRotate(+rotateSpeed);
        }

        /// <summary>
        /// action for rotating model right
        /// </summary>
        private void ActionRotateRight()
        {
            //Vector3 rotationPivotAxisPosition;
            //switch (automapViewMode)
            //{
            //    case AutomapViewMode.View2D:
            //        rotationPivotAxisPosition = rotationPivotAxisPositionViewFromTop;
            //        break;
            //    case AutomapViewMode.View3D:
            //        rotationPivotAxisPosition = rotationPivotAxisPositionView3D;
            //        break;
            //    default:
            //        rotationPivotAxisPosition = Vector3.zero;
            //        break;
            //}
            //cameraAutomap.transform.RotateAround(rotationPivotAxisPosition, -Vector3.up, +rotateSpeed * Time.unscaledDeltaTime);
            //updateAutomapView();
            ActionRotate(-rotateSpeed);
        }

        /// <summary>
        /// action for rotating camera around rotation axis about a certain rotationAmount
        /// </summary>
        private void ActionRotate(float rotationAmount)
        {
            Vector3 rotationPivotAxisPosition;
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    rotationPivotAxisPosition = rotationPivotAxisPositionViewFromTop;
                    break;
                case AutomapViewMode.View3D:
                    rotationPivotAxisPosition = rotationPivotAxisPositionView3D;
                    break;
                default:
                    rotationPivotAxisPosition = Vector3.zero;
                    break;
            }
            cameraAutomap.transform.RotateAround(rotationPivotAxisPosition, -Vector3.up, -rotationAmount * Time.unscaledDeltaTime);
            updateAutomapView();
        }

        /// <summary>
        /// action for rotating camera on camera YZ-plane around object about a certain rotationAmount
        /// </summary>
        private void ActionrotateCameraOnCameraYZplaneAroundObject(float rotationAmount)
        {
            if (automapViewMode == AutomapViewMode.View3D)
            {
                Vector3 rotationPoint = rotationPivotAxisPositionView3D;
                cameraAutomap.transform.RotateAround(rotationPoint, cameraAutomap.transform.right, -rotationAmount * Time.unscaledDeltaTime);
                updateAutomapView();
            }

        }

        /// <summary>
        /// action for changing camera rotation around y axis
        /// </summary>
        /// <param name="rotationSpeed"> amount used for rotation </param>
        private void ActionRotateCamera(float rotationAmount)
        {
            //cameraAutomap.transform.Rotate(0.0f, rotationAmount * Time.unscaledDeltaTime, 0.0f, Space.World);
            Vector3 vecRotationCenter = cameraAutomap.transform.position;
            cameraAutomap.transform.RotateAround(vecRotationCenter, Vector3.up, -rotationAmount * Time.unscaledDeltaTime);    
            updateAutomapView();
        }

        /// <summary>
        /// action for moving upstairs
        /// </summary>
        private void ActionMoveUpstairs()
        {
            cameraAutomap.transform.position += Vector3.up * moveUpDownSpeed * Time.unscaledDeltaTime;
            updateAutomapView();
        }

        /// <summary>
        /// action for moving downstairs
        /// </summary>
        private void ActionMoveDownstairs()
        {
            cameraAutomap.transform.position += Vector3.down * moveUpDownSpeed * Time.unscaledDeltaTime;
            updateAutomapView();
        }

        /// <summary>
        /// action for increasing slice level
        /// </summary>
        private void ActionIncreaseSliceLevel()
        {
            daggerfallAutomap.SlicingBiasY += Vector3.up.y * moveUpDownSpeed * Time.unscaledDeltaTime;
            updateAutomapView();
        }

        /// <summary>
        /// action for decreasing slice level
        /// </summary>
        private void ActionDecreaseSliceLevel()
        {
            daggerfallAutomap.SlicingBiasY += Vector3.down.y * moveUpDownSpeed * Time.unscaledDeltaTime;
            updateAutomapView();
        }

        /// <summary>
        /// action for zooming in
        /// </summary>
        private void ActionZoomIn(float zoomSpeed)
        {
            float zoomSpeedCompensated = zoomSpeed * Vector3.Magnitude(Camera.main.transform.position - cameraAutomap.transform.position);
            Vector3 translation = cameraAutomap.transform.forward * zoomSpeedCompensated;
            cameraAutomap.transform.position += translation;
            updateAutomapView();
        }

        /// <summary>
        /// action for zooming out
        /// </summary>
        private void ActionZoomOut(float zoomSpeed)
        {
            float zoomSpeedCompensated = zoomSpeed * Vector3.Magnitude(Camera.main.transform.position - cameraAutomap.transform.position);
            Vector3 translation = -cameraAutomap.transform.forward * zoomSpeedCompensated;
            cameraAutomap.transform.position += translation;
            updateAutomapView();
        }

        /// <summary>
        /// action for increasing camera field of view
        /// </summary>
        private void ActionIncreaseCameraFieldOfView()
        {
            if (automapViewMode == AutomapViewMode.View3D)
            {
                fieldOfViewCameraMode3D = Mathf.Max(minFieldOfViewCameraMode3D, Mathf.Min(maxFieldOfViewCameraMode3D, fieldOfViewCameraMode3D + changeSpeedCameraFieldOfView * Time.unscaledDeltaTime));
                cameraAutomap.fieldOfView = fieldOfViewCameraMode3D;
                updateAutomapView();
            }            
        }

        /// <summary>
        /// action for decreasing camera field of view
        /// </summary>
        private void ActionDecreaseCameraFieldOfView()
        {
            if (automapViewMode == AutomapViewMode.View3D)
            {
                fieldOfViewCameraMode3D = Mathf.Max(minFieldOfViewCameraMode3D, Mathf.Min(maxFieldOfViewCameraMode3D, fieldOfViewCameraMode3D - changeSpeedCameraFieldOfView * Time.unscaledDeltaTime));
                cameraAutomap.fieldOfView = fieldOfViewCameraMode3D;
                updateAutomapView();
            }
        }

        /// <summary>
        /// action for switching to next automap render mode
        /// </summary>
        private void ActionSwitchToNextAutomapRenderMode()
        {
            daggerfallAutomap.switchToNextAutomapRenderMode();
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to automap render mode "transparent"
        /// </summary>
        private void ActionSwitchToAutomapRenderModeTransparent()
        {
            daggerfallAutomap.switchToAutomapRenderModeTransparent();
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to automap render mode "wireframe"
        /// </summary>
        private void ActionSwitchToAutomapRenderModeWireframe()
        {
            daggerfallAutomap.switchToAutomapRenderModeWireframe();
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to automap render mode "cutout"
        /// </summary>
        private void ActionSwitchToAutomapRenderModeCutout()
        {
            daggerfallAutomap.switchToAutomapRenderModeCutout();
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to automap background to original background
        /// </summary>
        private void ActionSwitchToAutomapBackgroundOriginal()
        {
            nativeTexture.SetPixels(0, 29, nativeTexture.width, nativeTexture.height - 29, backgroundOriginal);
            nativeTexture.Apply(false);
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to automap background to background alternative 1
        /// </summary>
        private void ActionSwitchToAutomapBackgroundAlternative1()
        {
            nativeTexture.SetPixels(0, 29, nativeTexture.width, nativeTexture.height - 29, backgroundAlternative1);
            nativeTexture.Apply(false);
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to automap background to background alternative 2
        /// </summary>
        private void ActionSwitchToAutomapBackgroundAlternative2()
        {
            nativeTexture.SetPixels(0, 29, nativeTexture.width, nativeTexture.height - 29, backgroundAlternative2);
            nativeTexture.Apply(false);
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to automap background to background alternative 3
        /// </summary>
        private void ActionSwitchToAutomapBackgroundAlternative3()
        {
            nativeTexture.SetPixels(0, 29, nativeTexture.width, nativeTexture.height - 29, backgroundAlternative3);
            nativeTexture.Apply(false);
            updateAutomapView();
        }

        /// <summary>
        /// action for changing automap grid mode
        /// </summary>
        private void ActionChangeAutomapGridMode()
        {
            int numberOfViewModes = Enum.GetNames(typeof(AutomapViewMode)).Length;
            automapViewMode++;
            if ((int)automapViewMode > numberOfViewModes - 1) // first mode is mode 0 -> so use numberOfViewModes-1 for comparison
                automapViewMode = 0;
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    // update grid graphics
                    nativeTexture.SetPixels(78, nativeTexture.height - 171 - 19, 27, 19, pixelsGrid2D);
                    nativeTexture.Apply(false);
                    saveCameraTransformView3D();
                    restoreOldCameraTransformViewFromTop();
                    cameraAutomap.fieldOfView = fieldOfViewCameraMode2D;
                    daggerfallAutomap.RotationPivotAxisPosition = rotationPivotAxisPositionViewFromTop;
                    updateAutomapView();
                    break;
                case AutomapViewMode.View3D:
                    // update grid graphics
                    nativeTexture.SetPixels(78, nativeTexture.height - 171 - 19, 27, 19, pixelsGrid3D);
                    nativeTexture.Apply(false);
                    saveCameraTransformViewFromTop();
                    restoreOldCameraTransformView3D();
                    cameraAutomap.fieldOfView = fieldOfViewCameraMode3D;
                    daggerfallAutomap.RotationPivotAxisPosition = rotationPivotAxisPositionView3D;
                    updateAutomapView();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// action for reset view
        /// </summary>
        private void ActionResetView()
        {
            // reset values to default
            resetRotationPivotAxisPosition(); // reset rotation pivot axis
            daggerfallAutomap.SlicingBiasY = defaultSlicingBiasY; // reset slicing y-bias
            resetCameraPosition();
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    cameraAutomap.fieldOfView = fieldOfViewCameraMode2D;
                    break;
                case AutomapViewMode.View3D:
                    cameraAutomap.fieldOfView = fieldOfViewCameraMode3D;
                    break;
            }            
            updateAutomapView();
        }

        /// <summary>
        /// action for reset rotation pivot axis
        /// </summary>
        private void ActionResetRotationPivotAxis()
        {
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    resetRotationPivotAxisPositionViewFromTop();
                    updateAutomapView();
                    break;
                case AutomapViewMode.View3D:
                    resetRotationPivotAxisPositionView3D();
                    updateAutomapView();
                    break;
                default:
                    break;
            }
            updateAutomapView();
        }


        /// <summary>
        /// action for switching focus to next beacon object
        /// </summary>
        private void ActionSwitchFocusToNextBeaconObject()
        {
            GameObject gameobjectInFocus = daggerfallAutomap.switchFocusToNextObject();
            SwitchFocusToGameObject(gameobjectInFocus);
        }

        /// <summary>
        /// switch focus to GameObject gameobjectInFocus
        /// </summary>
        /// <param name="gameobjectInFocus"> the GameObject to focus at </param>        
        private void SwitchFocusToGameObject(GameObject gameobjectInFocus)
        {
            Vector3 newPosition;
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    newPosition = cameraAutomap.transform.position;
                    newPosition.x = gameobjectInFocus.transform.position.x;
                    newPosition.z = gameobjectInFocus.transform.position.z;
                    cameraAutomap.transform.position = newPosition;
                    updateAutomapView();
                    break;
                case AutomapViewMode.View3D:
                    float computedCameraBackwardDistance = Vector3.Magnitude(cameraAutomap.transform.position - gameobjectInFocus.transform.position);
                    newPosition = gameobjectInFocus.transform.position - cameraAutomap.transform.forward * computedCameraBackwardDistance;                    
                    cameraAutomap.transform.position = newPosition;
                    updateAutomapView();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Event Handlers

        private void PanelAutomap_OnMouseScrollUp()
        {
            ActionZoomIn(zoomSpeedMouseWheel);
        }

        private void PanelAutomap_OnMouseScrollDown()
        {
            ActionZoomOut(zoomSpeedMouseWheel);
        }

        private void PanelAutomap_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (alreadyInMouseDown)
                return;

            leftMouseClickedOnPanelAutomap = true; // used for debug teleport mode clicks

            if (daggerfallAutomap.DebugTeleportMode && ((Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift)) || (Input.GetKey(KeyCode.RightControl) && Input.GetKey(KeyCode.RightShift))))
                return;

            Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            oldMousePosition = mousePosition;
            leftMouseDownOnPanelAutomap = true;
            alreadyInMouseDown = true;
        }

        private void PanelAutomap_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            leftMouseClickedOnPanelAutomap = false; // used for debug teleport mode clicks
            leftMouseDownOnPanelAutomap = false;
            alreadyInMouseDown = false;
        }

        private void PanelAutomap_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (alreadyInRightMouseDown)
                return;

            Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            oldMousePosition = mousePosition;
            rightMouseDownOnPanelAutomap = true;
            alreadyInRightMouseDown = true;
        }

        private void PanelAutomap_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rightMouseDownOnPanelAutomap = false;
            alreadyInRightMouseDown = false;
        }

        private void GridButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode())
                return;

            ActionChangeAutomapGridMode();
        }

        private void GridButton_OnRightMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode())
                return;

            ActionResetRotationPivotAxis();
        }

        private void GridButton_OnMouseScrollUp()
        {
            if (inDragMode())
                return;

            ActionIncreaseCameraFieldOfView();
        }

        private void GridButton_OnMouseScrollDown()
        {
            if (inDragMode())
                return;

            ActionDecreaseCameraFieldOfView();
        }

        private void ForwardButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            forwardButton.SuppressToolTip = true;

            leftMouseDownOnForwardButton = true;
            alreadyInMouseDown = true;
        }

        private void ForwardButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            forwardButton.SuppressToolTip = false;

            leftMouseDownOnForwardButton = false;
            alreadyInMouseDown = false;
        }

        private void ForwardButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInRightMouseDown)
                return;

            forwardButton.SuppressToolTip = true;

            rightMouseDownOnForwardButton = true;
            alreadyInRightMouseDown = true;
        }

        private void ForwardButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            forwardButton.SuppressToolTip = false;

            rightMouseDownOnForwardButton = false;
            alreadyInRightMouseDown = false;
        }

        private void BackwardButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            backwardButton.SuppressToolTip = true;

            leftMouseDownOnBackwardButton = true;
            alreadyInMouseDown = true;
        }

        private void BackwardButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            backwardButton.SuppressToolTip = false;

            leftMouseDownOnBackwardButton = false;
            alreadyInMouseDown = false;
        }

        private void BackwardButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInRightMouseDown)
                return;

            backwardButton.SuppressToolTip = true;

            rightMouseDownOnBackwardButton = true;
            alreadyInRightMouseDown = true;
        }

        private void BackwardButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            backwardButton.SuppressToolTip = false;

            rightMouseDownOnBackwardButton = false;
            alreadyInRightMouseDown = false;
        }

        private void LeftButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            leftButton.SuppressToolTip = true;

            leftMouseDownOnLeftButton = true;
            alreadyInMouseDown = true;
        }

        private void LeftButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            leftButton.SuppressToolTip = false;

            leftMouseDownOnLeftButton = false;
            alreadyInMouseDown = false;
        }

        private void LeftButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInRightMouseDown)
                return;

            leftButton.SuppressToolTip = true;

            rightMouseDownOnLeftButton = true;
            alreadyInRightMouseDown = true;
        }

        private void LeftButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            leftButton.SuppressToolTip = false;

            rightMouseDownOnLeftButton = false;
            alreadyInRightMouseDown = false;
        }

        private void RightButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            rightButton.SuppressToolTip = true;

            leftMouseDownOnRightButton = true;
            alreadyInMouseDown = true;
        }

        private void RightButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rightButton.SuppressToolTip = false;

            leftMouseDownOnRightButton = false;
            alreadyInMouseDown = false;
        }

        private void RightButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInRightMouseDown)
                return;

            rightButton.SuppressToolTip = true;

            rightMouseDownOnRightButton = true;
            alreadyInRightMouseDown = true;
        }

        private void RightButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rightButton.SuppressToolTip = false;

            rightMouseDownOnRightButton = false;
            alreadyInRightMouseDown = false;
        }

        private void RotateLeftButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            rotateLeftButton.SuppressToolTip = true;

            leftMouseDownOnRotateLeftButton = true;
            alreadyInMouseDown = true;
        }

        private void RotateLeftButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rotateLeftButton.SuppressToolTip = false;

            leftMouseDownOnRotateLeftButton = false;
            alreadyInMouseDown = false;
        }

        private void RotateLeftButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            rotateLeftButton.SuppressToolTip = true;

            rightMouseDownOnRotateLeftButton = true;
            alreadyInMouseDown = true;
        }

        private void RotateLeftButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rotateLeftButton.SuppressToolTip = false;

            rightMouseDownOnRotateLeftButton = false;
            alreadyInMouseDown = false;
        }

        private void RotateRightButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            rotateRightButton.SuppressToolTip = true;

            leftMouseDownOnRotateRightButton = true;
            alreadyInMouseDown = true;
        }

        private void RotateRightButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rotateRightButton.SuppressToolTip = false;

            leftMouseDownOnRotateRightButton = false;
            alreadyInMouseDown = false;
        }

        private void RotateRightButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            rotateRightButton.SuppressToolTip = true;

            rightMouseDownOnRotateRightButton = true;
            alreadyInMouseDown = true;
        }

        private void RotateRightButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rotateRightButton.SuppressToolTip = false;

            rightMouseDownOnRotateRightButton = false;
            alreadyInMouseDown = false;
        }

        private void UpstairsButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            upstairsButton.SuppressToolTip = true;

            leftMouseDownOnUpstairsButton = true;
            alreadyInMouseDown = true;
        }

        private void UpstairsButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            upstairsButton.SuppressToolTip = false;

            leftMouseDownOnUpstairsButton = false;
            alreadyInMouseDown = false;
        }


        private void DownstairsButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            downstairsButton.SuppressToolTip = true;

            leftMouseDownOnDownstairsButton = true;
            alreadyInMouseDown = true;
        }

        private void DownstairsButton_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            downstairsButton.SuppressToolTip = false;

            leftMouseDownOnDownstairsButton = false;
            alreadyInMouseDown = false;
        }

        private void UpstairsButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInRightMouseDown)
                return;

            upstairsButton.SuppressToolTip = true;

            rightMouseDownOnUpstairsButton = true;
            alreadyInRightMouseDown = true;
        }

        private void UpstairsButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            upstairsButton.SuppressToolTip = false;

            rightMouseDownOnUpstairsButton = false;
            alreadyInRightMouseDown = false;
        }


        private void DownstairsButton_OnRightMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInRightMouseDown)
                return;

            downstairsButton.SuppressToolTip = true;

            rightMouseDownOnDownstairsButton = true;
            alreadyInRightMouseDown = true;
        }

        private void DownstairsButton_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            downstairsButton.SuppressToolTip = false;

            rightMouseDownOnDownstairsButton = false;
            alreadyInRightMouseDown = false;
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode())
                return;

            CloseWindow();
        }

        private void Compass_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode())
                return;

            ActionSwitchFocusToNextBeaconObject();
        }

        private void Compass_OnRightMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode())
                return;

            ActionResetView();
        }

        #endregion
    }
}