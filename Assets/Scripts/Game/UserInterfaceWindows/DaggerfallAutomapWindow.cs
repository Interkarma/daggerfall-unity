// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.UserInterface;

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
        const float nearClipPlaneCameraMode2D = 100f; // simulate classic Daggerfall near clip plane
        const float defaultFieldOfViewCameraMode3D = 45.0f; // default camera field of view used for 3D mode
        float fieldOfViewCameraMode3D = defaultFieldOfViewCameraMode3D; // camera field of view used for 3D mode (can be changed with mouse wheel over grid button)
        const float nearClipPlaneCameraMode3D = 0.3f; // default Unity3D value
        const float minFieldOfViewCameraMode3D = 15.0f; // minimum value of camera field of view that can be adjusted in 3D mode
        const float maxFieldOfViewCameraMode3D = 65.0f; // maximum value of camera field of view that can be adjusted in 3D mode

        const float defaultSlicingBiasY = 0.2f;

        const float cameraHeightViewFromTop = 150.0f; // initial camera height in 2D mode
        const float cameraHeightView3D = 8.0f; // initial camera height in 3D mode
        const float cameraBackwardDistance = 20.0f; // initial camera distance "backwards" in 3D mode

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

        // hover text label in status bar
        TextLabel labelHoverText;

        // Handle toggle closing
        KeyCode automapBinding = KeyCode.None;
        bool isCloseWindowDeferred = false;
        readonly KeyCode fallbackKey = KeyCode.Home;

        // definitions of hotkey sequences
        HotkeySequence HotkeySequence_SwitchAutomapGridMode;
        HotkeySequence HotkeySequence_ResetView;
        HotkeySequence HotkeySequence_ResetRotationPivotAxisView;
        HotkeySequence HotkeySequence_SwitchFocusToNextBeaconObject;
        HotkeySequence HotkeySequence_SwitchToNextAutomapRenderMode;
        HotkeySequence HotkeySequence_SwitchToAutomapRenderModeCutout;
        HotkeySequence HotkeySequence_SwitchToAutomapRenderModeWireframe;
        HotkeySequence HotkeySequence_SwitchToAutomapRenderModeTransparent;
        HotkeySequence HotkeySequence_SwitchToAutomapBackgroundOriginal;
        HotkeySequence HotkeySequence_SwitchToAutomapBackgroundAlternative1;
        HotkeySequence HotkeySequence_SwitchToAutomapBackgroundAlternative2;
        HotkeySequence HotkeySequence_SwitchToAutomapBackgroundAlternative3;
        HotkeySequence HotkeySequence_MoveLeft;
        HotkeySequence HotkeySequence_MoveRight;
        HotkeySequence HotkeySequence_MoveForward;
        HotkeySequence HotkeySequence_MoveBackward;
        HotkeySequence HotkeySequence_MoveRotationPivotAxisLeft;
        HotkeySequence HotkeySequence_MoveRotationPivotAxisRight;
        HotkeySequence HotkeySequence_MoveRotationPivotAxisForward;
        HotkeySequence HotkeySequence_MoveRotationPivotAxisBackward;
        HotkeySequence HotkeySequence_RotateLeft;
        HotkeySequence HotkeySequence_RotateRight;
        HotkeySequence HotkeySequence_RotateCameraLeft;
        HotkeySequence HotkeySequence_RotateCameraRight;
        HotkeySequence HotkeySequence_RotateCameraOnCameraYZplaneAroundObjectUp;
        HotkeySequence HotkeySequence_RotateCameraOnCameraYZplaneAroundObjectDown;
        HotkeySequence HotkeySequence_Upstairs;
        HotkeySequence HotkeySequence_Downstairs;
        HotkeySequence HotkeySequence_IncreaseSliceLevel;
        HotkeySequence HotkeySequence_DecreaseSliceLevel;
        HotkeySequence HotkeySequence_ZoomIn;
        HotkeySequence HotkeySequence_ZoomOut;
        HotkeySequence HotkeySequence_IncreaseCameraFieldOfFiew;
        HotkeySequence HotkeySequence_DecreaseCameraFieldOfFiew;

        const string nativeImgName = "AMAP00I0.IMG";
        const string nativeImgNameGrid3D = "AMAP01I0.IMG";

        Automap automap = null; // used to communicate with Automap class

        GameObject gameobjectAutomap = null; // used to hold reference to instance of GameObject "Automap" (which has script Game/Automap.cs attached)

        Camera cameraAutomap = null; // camera for automap camera

        GameObject gameObjectPlayerAdvanced = null; // used to hold reference to instance of GameObject "PlayerAdvanced"

        public enum AutomapViewMode { View2D = 0, View3D = 1};
        AutomapViewMode automapViewMode = AutomapViewMode.View3D; // default to 3D - this deviation from classic is on purpose (after people asked for it)

        Panel dummyPanelAutomap = null; // used to determine correct render panel position
        Panel panelRenderAutomap = null; // level geometry is rendered into this panel
        Panel dummyPanelOverlay = null; // used to determine correct panel position for additional overlays
        Panel panelRenderOverlay = null; // used for overlays rendering (micro-map)
        Rect oldPositionNativePanel;
        Vector2 oldMousePosition; // old mouse position used to determine offset of mouse movement since last time used for for drag and drop functionality
        Rect? oldCustomScreenRect = null;

        Panel dummyPanelCompass = null; // used to determine correct compass position

        // these boolean flags are used to indicate which mouse button was pressed over which gui button/element - these are set in the event callbacks
        bool leftMouseClickedOnPanelAutomap = false; // used for debug teleport mode clicks
        bool leftMouseDownOnPanelAutomap = false;
        bool rightMouseDownOnPanelAutomap = false;
        bool middleMouseDownOnPanelAutomap = false;
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
        bool alreadyInMiddleMouseDown = false;
        bool inDragMode() { return leftMouseDownOnPanelAutomap || rightMouseDownOnPanelAutomap || middleMouseDownOnPanelAutomap; }

        Texture2D nativeTexture; // background image will be stored in this Texture2D

        Texture2D nativeTextureGrid2D;
        Texture2D nativeTextureGrid3D;

        Color[] pixelsGrid2D; // grid button texture for 2D view image will be stored in here
        //Color[] pixelsGrid3D; // grid button texture for 3D view image will be stored in here

        Color[] backgroundOriginal; // texture with orignial background will be stored in here
        Color[] backgroundAlternative1; // texture with first alternative background will be stored in here
        Color[] backgroundAlternative2; // texture with second alternative background will be stored in here
        Color[] backgroundAlternative3; // texture with third alternative background will be stored in here

        Texture2D textureBackgroundAlternative1;
        Texture2D textureBackgroundAlternative2;
        Texture2D textureBackgroundAlternative3;

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
            // Prevent duplicate close calls with base class's exitKey (Escape)
            AllowCancel = false;
        }

        /// <summary>
        /// sets up hotkey sequences (tests for automap open key and uses fallback key for actions that are assigned to the same key)
        /// </summary>
        private void SetupHotkeySequences()
        {
            HotkeySequence_SwitchAutomapGridMode = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapSwitchAutomapGridMode);
            HotkeySequence_ResetView = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapResetView);
            HotkeySequence_ResetRotationPivotAxisView = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapResetRotationPivotAxisView);
            HotkeySequence_SwitchFocusToNextBeaconObject = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapSwitchFocusToNextBeaconObject);
            HotkeySequence_SwitchToNextAutomapRenderMode = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapSwitchToNextAutomapRenderMode);
            HotkeySequence_SwitchToAutomapRenderModeCutout = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapSwitchToAutomapRenderModeCutout);
            HotkeySequence_SwitchToAutomapRenderModeWireframe = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapSwitchToAutomapRenderModeWireframe);
            HotkeySequence_SwitchToAutomapRenderModeTransparent = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapSwitchToAutomapRenderModeTransparent);
            HotkeySequence_SwitchToAutomapBackgroundOriginal = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapSwitchToAutomapBackgroundOriginal);
            HotkeySequence_SwitchToAutomapBackgroundAlternative1 = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapSwitchToAutomapBackgroundAlternative1);
            HotkeySequence_SwitchToAutomapBackgroundAlternative2 = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapSwitchToAutomapBackgroundAlternative2);
            HotkeySequence_SwitchToAutomapBackgroundAlternative3 = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapSwitchToAutomapBackgroundAlternative3);
            HotkeySequence_MoveLeft = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapMoveLeft);
            HotkeySequence_MoveRight = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapMoveRight);
            HotkeySequence_MoveForward = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapMoveForward);
            HotkeySequence_MoveBackward = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapMoveBackward);
            HotkeySequence_MoveRotationPivotAxisLeft = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapMoveRotationPivotAxisLeft);
            HotkeySequence_MoveRotationPivotAxisRight = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapMoveRotationPivotAxisRight);
            HotkeySequence_MoveRotationPivotAxisForward = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapMoveRotationPivotAxisForward);
            HotkeySequence_MoveRotationPivotAxisBackward = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapMoveRotationPivotAxisBackward);
            HotkeySequence_RotateLeft = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapRotateLeft);
            HotkeySequence_RotateRight = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapRotateRight);
            HotkeySequence_RotateCameraLeft = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapRotateCameraLeft);
            HotkeySequence_RotateCameraRight = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapRotateCameraRight);
            HotkeySequence_RotateCameraOnCameraYZplaneAroundObjectUp = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapRotateCameraOnCameraYZplaneAroundObjectUp);
            HotkeySequence_RotateCameraOnCameraYZplaneAroundObjectDown = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapRotateCameraOnCameraYZplaneAroundObjectDown);
            HotkeySequence_Upstairs = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapUpstairs);
            HotkeySequence_Downstairs = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapDownstairs);
            HotkeySequence_IncreaseSliceLevel = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapIncreaseSliceLevel);
            HotkeySequence_DecreaseSliceLevel = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapDecreaseSliceLevel);
            HotkeySequence_ZoomIn = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapZoomIn);
            HotkeySequence_ZoomOut = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapZoomOut);
            HotkeySequence_IncreaseCameraFieldOfFiew = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapIncreaseCameraFieldOfFiew);
            HotkeySequence_DecreaseCameraFieldOfFiew = ShortcutOrFallback(DaggerfallShortcut.Buttons.AutomapDecreaseCameraFieldOfFiew);
        }

        private HotkeySequence ShortcutOrFallback(DaggerfallShortcut.Buttons button)
        {
            HotkeySequence hotkeySequence = DaggerfallShortcut.GetBinding(button);
            if (hotkeySequence.IsSameKeyCode(automapBinding))
                return hotkeySequence.WithKeyCode(fallbackKey);
            else
                return hotkeySequence;
        }

        /// <summary>
        /// updates button tool tip texts (with dynamic hotkey mappings)
        /// </summary>
        private void UpdateButtonToolTipsText()
        {
            gridButton.ToolTipText = String.Format(TextManager.Instance.GetLocalizedText("automapToolTipTextGridButton"), HotkeySequence_SwitchAutomapGridMode, HotkeySequence_ResetRotationPivotAxisView);
            forwardButton.ToolTipText = String.Format(TextManager.Instance.GetLocalizedText("automapToolTipForwardButton"), HotkeySequence_MoveForward, HotkeySequence_MoveRotationPivotAxisForward);
            backwardButton.ToolTipText = String.Format(TextManager.Instance.GetLocalizedText("automapToolTipBackwardButton"), HotkeySequence_MoveBackward, HotkeySequence_MoveRotationPivotAxisBackward);
            leftButton.ToolTipText = String.Format(TextManager.Instance.GetLocalizedText("automapToolTipLeftButton"), HotkeySequence_MoveLeft, HotkeySequence_MoveRotationPivotAxisLeft);
            rightButton.ToolTipText = String.Format(TextManager.Instance.GetLocalizedText("automapToolTipRightButton"), HotkeySequence_MoveRight, HotkeySequence_MoveRotationPivotAxisRight);
            rotateLeftButton.ToolTipText = String.Format(TextManager.Instance.GetLocalizedText("automapToolTipRotateLeftButton"), HotkeySequence_RotateLeft, HotkeySequence_RotateCameraLeft);
            rotateRightButton.ToolTipText = String.Format(TextManager.Instance.GetLocalizedText("automapToolTipRotateRightButton"), HotkeySequence_RotateRight, HotkeySequence_RotateCameraRight);
            upstairsButton.ToolTipText = String.Format(TextManager.Instance.GetLocalizedText("automapToolTipUpstairsButton"), HotkeySequence_Upstairs, HotkeySequence_IncreaseSliceLevel, HotkeySequence_SwitchToAutomapRenderModeCutout, HotkeySequence_SwitchToAutomapRenderModeWireframe, HotkeySequence_SwitchToAutomapRenderModeTransparent);
            downstairsButton.ToolTipText = String.Format(TextManager.Instance.GetLocalizedText("automapToolTipDownstairsButton"), HotkeySequence_Downstairs, HotkeySequence_DecreaseSliceLevel, HotkeySequence_SwitchToAutomapRenderModeCutout, HotkeySequence_SwitchToAutomapRenderModeWireframe, HotkeySequence_SwitchToAutomapRenderModeTransparent);
            dummyPanelCompass.ToolTipText = String.Format(TextManager.Instance.GetLocalizedText("automapToolTipPanelCompass"), HotkeySequence_SwitchFocusToNextBeaconObject, HotkeySequence_ResetView);
        }

        /// <summary>
        /// initial window setup of the automap window
        /// </summary>
        protected override void Setup()
        {           
            if (isSetup) // don't setup twice!
                return;

            InitGlobalResources(); // initialize gameobjectAutomap, daggerfallAutomap and layerAutomap

            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName, TextureFormat.ARGB32, false);
            nativeTexture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            if (!nativeTexture)
                throw new Exception("DaggerfallAutomapWindow: Could not load native texture (AMAP00I0.IMG).");

            // Load alternative Grid Icon (3D View Grid graphics)
            nativeTextureGrid3D = DaggerfallUI.GetTextureFromImg(nativeImgNameGrid3D, TextureFormat.ARGB32, false);
            nativeTextureGrid3D.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            if (!nativeTextureGrid3D)
                throw new Exception("DaggerfallAutomapWindow: Could not load native texture (AMAP01I0.IMG).");            
            //pixelsGrid3D = nativeTextureGrid3D.GetPixels((int)(0), (int)((200 - 0 - 19) * (nativeTextureGrid3D.height / 200f)), (int)(27 * (nativeTextureGrid3D.width / 320f)), (int)(19 * (nativeTextureGrid3D.height / 200f)));

            // Cut out 2D View Grid graphics from background image            
            int width = (int)(27 * (nativeTexture.width / 320f));
            int height = (int)(19 * (nativeTexture.height / 200f));
            pixelsGrid2D = nativeTexture.GetPixels((int)(78 * (nativeTexture.width / 320f)), (int)((200 - 171 - 19) * (nativeTexture.height / 200f)), width, height);
            nativeTextureGrid2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
            nativeTextureGrid2D.SetPixels(pixelsGrid2D);
            nativeTextureGrid2D.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            nativeTextureGrid2D.Apply(false, false);

            // store background graphics from from background image            
            width = (int)(320 * (nativeTexture.width / 320f));
            height = (int)((200 - 29) * (nativeTexture.height / 200f));
            backgroundOriginal = nativeTexture.GetPixels((int)(0 * (nativeTexture.width / 320f)), (int)(29 * (nativeTexture.height / 200f)), width, height);

            backgroundAlternative1 = new Color[backgroundOriginal.Length];
            for (int i = 0; i < backgroundOriginal.Length; ++i)
            {
                backgroundAlternative1[i].r = 0.0f;
                backgroundAlternative1[i].g = 0.0f;
                backgroundAlternative1[i].b = 0.0f;
                backgroundAlternative1[i].a = 1.0f;
            }
            textureBackgroundAlternative1 = new Texture2D(width, height, TextureFormat.ARGB32, false);
            textureBackgroundAlternative1.SetPixels(backgroundAlternative1);
            textureBackgroundAlternative1.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            textureBackgroundAlternative1.Apply(false, true);

            backgroundAlternative2 = new Color[backgroundOriginal.Length];
            for (int i = 0; i < backgroundOriginal.Length; ++i)
            {
                backgroundAlternative2[i].r = 0.2f;
                backgroundAlternative2[i].g = 0.1f;
                backgroundAlternative2[i].b = 0.3f;
                backgroundAlternative2[i].a = 1.0f;
            }
            textureBackgroundAlternative2 = new Texture2D(width, height, TextureFormat.ARGB32, false);
            textureBackgroundAlternative2.SetPixels(backgroundAlternative2);
            textureBackgroundAlternative2.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            textureBackgroundAlternative2.Apply(false, true);

            backgroundAlternative3 = new Color[backgroundOriginal.Length];
            for (int i = 0; i < backgroundOriginal.Length; ++i)
            {
                backgroundAlternative3[i].r = 0.3f;
                backgroundAlternative3[i].g = 0.1f;
                backgroundAlternative3[i].b = 0.2f;
                backgroundAlternative3[i].a = 1.0f;
            }
            textureBackgroundAlternative3 = new Texture2D(width, height, TextureFormat.ARGB32, false);
            textureBackgroundAlternative3.SetPixels(backgroundAlternative3);
            textureBackgroundAlternative3.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            textureBackgroundAlternative3.Apply(false, true);

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

            // Setup automap render panel (the level geometry is rendered into this panel) - use dummyPanelAutomap to get size
            Rect positionPanelRenderAutomap = dummyPanelAutomap.Rectangle;            
            panelRenderAutomap = DaggerfallUI.AddPanel(positionPanelRenderAutomap, ParentPanel);
            panelRenderAutomap.AutoSize = AutoSizeModes.None;
            
            panelRenderAutomap.OnMouseScrollUp += PanelAutomap_OnMouseScrollUp;
            panelRenderAutomap.OnMouseScrollDown += PanelAutomap_OnMouseScrollDown;
            panelRenderAutomap.OnMouseDown += PanelAutomap_OnMouseDown;
            panelRenderAutomap.OnMouseUp += PanelAutomap_OnMouseUp;
            panelRenderAutomap.OnRightMouseDown += PanelAutomap_OnRightMouseDown;
            panelRenderAutomap.OnRightMouseUp += PanelAutomap_OnRightMouseUp;
            panelRenderAutomap.OnMiddleMouseDown += PanelAutomap_OnMiddleMouseDown;
            panelRenderAutomap.OnMiddleMouseUp += PanelAutomap_OnMiddleMouseUp;
            panelRenderAutomap.OnMouseDoubleClick += PanelAutomap_OnMouseDoubleClick;
            panelRenderAutomap.OnRightMouseDoubleClick += PanelAutomap_OnRightMouseDoubleClick;
            panelRenderAutomap.OnMiddleMouseDoubleClick += PanelAutomap_OnMiddleMouseDoubleClick;


            // dummyPanelOverlay is used to get correct size for compass
            Rect rectPanelOverlay = new Rect();
            rectPanelOverlay.position = new Vector2(0, 52);
            rectPanelOverlay.size = new Vector2(28, 28);
            dummyPanelOverlay = DaggerfallUI.AddPanel(rectPanelOverlay, NativePanel);

            // Setup automap overlay panel (overlay is rendered into this panel) - use dummyPanelOverlay to get size
            Rect positionPanelOverlay = dummyPanelOverlay.Rectangle;
            panelRenderOverlay = DaggerfallUI.AddPanel(positionPanelOverlay, ParentPanel);
            panelRenderOverlay.AutoSize = AutoSizeModes.None;


            // Grid button (toggle 2D <-> 3D view)
            gridButton = DaggerfallUI.AddButton(new Rect(78, 171, 27, 19), NativePanel);
            gridButton.OnMouseClick += GridButton_OnMouseClick;
            gridButton.OnRightMouseClick += GridButton_OnRightMouseClick;
            gridButton.OnMouseScrollUp += GridButton_OnMouseScrollUp;
            gridButton.OnMouseScrollDown += GridButton_OnMouseScrollDown;
            gridButton.ToolTip = defaultToolTip;
            

            // forward button
            forwardButton = DaggerfallUI.AddButton(new Rect(105, 171, 21, 19), NativePanel);
            forwardButton.OnMouseDown += ForwardButton_OnMouseDown;
            forwardButton.OnMouseUp += ForwardButton_OnMouseUp;
            forwardButton.OnRightMouseDown += ForwardButton_OnRightMouseDown;
            forwardButton.OnRightMouseUp += ForwardButton_OnRightMouseUp;
            forwardButton.ToolTip = defaultToolTip;

            // backward button
            backwardButton = DaggerfallUI.AddButton(new Rect(126, 171, 21, 19), NativePanel);
            backwardButton.OnMouseDown += BackwardButton_OnMouseDown;
            backwardButton.OnMouseUp += BackwardButton_OnMouseUp;
            backwardButton.OnRightMouseDown += BackwardButton_OnRightMouseDown;
            backwardButton.OnRightMouseUp += BackwardButton_OnRightMouseUp;
            backwardButton.ToolTip = defaultToolTip;

            // left button
            leftButton = DaggerfallUI.AddButton(new Rect(149, 171, 21, 19), NativePanel);
            leftButton.OnMouseDown += LeftButton_OnMouseDown;
            leftButton.OnMouseUp += LeftButton_OnMouseUp;
            leftButton.OnRightMouseDown += LeftButton_OnRightMouseDown;
            leftButton.OnRightMouseUp += LeftButton_OnRightMouseUp;
            leftButton.ToolTip = defaultToolTip;

            // right button
            rightButton = DaggerfallUI.AddButton(new Rect(170, 171, 21, 19), NativePanel);
            rightButton.OnMouseDown += RightButton_OnMouseDown;
            rightButton.OnMouseUp += RightButton_OnMouseUp;
            rightButton.OnRightMouseDown += RightButton_OnRightMouseDown;
            rightButton.OnRightMouseUp += RightButton_OnRightMouseUp;
            rightButton.ToolTip = defaultToolTip;

            // rotate left button
            rotateLeftButton = DaggerfallUI.AddButton(new Rect(193, 171, 21, 19), NativePanel);
            rotateLeftButton.OnMouseDown += RotateLeftButton_OnMouseDown;
            rotateLeftButton.OnMouseUp += RotateLeftButton_OnMouseUp;
            rotateLeftButton.OnRightMouseDown += RotateLeftButton_OnRightMouseDown;
            rotateLeftButton.OnRightMouseUp += RotateLeftButton_OnRightMouseUp;
            rotateLeftButton.ToolTip = defaultToolTip;

            // rotate right button
            rotateRightButton = DaggerfallUI.AddButton(new Rect(214, 171, 21, 19), NativePanel);
            rotateRightButton.OnMouseDown += RotateRightButton_OnMouseDown;
            rotateRightButton.OnMouseUp += RotateRightButton_OnMouseUp;
            rotateRightButton.OnRightMouseDown += RotateRightButton_OnRightMouseDown;
            rotateRightButton.OnRightMouseUp += RotateRightButton_OnRightMouseUp;
            rotateRightButton.ToolTip = defaultToolTip;

            // upstairs button
            upstairsButton = DaggerfallUI.AddButton(new Rect(237, 171, 21, 19), NativePanel);
            upstairsButton.OnMouseDown += UpstairsButton_OnMouseDown;
            upstairsButton.OnMouseUp += UpstairsButton_OnMouseUp;
            upstairsButton.OnRightMouseDown += UpstairsButton_OnRightMouseDown;
            upstairsButton.OnRightMouseUp += UpstairsButton_OnRightMouseUp;
            upstairsButton.ToolTip = defaultToolTip;

            // downstairs button
            downstairsButton = DaggerfallUI.AddButton(new Rect(258, 171, 21, 19), NativePanel);
            downstairsButton.OnMouseDown += DownstairsButton_OnMouseDown;
            downstairsButton.OnMouseUp += DownstairsButton_OnMouseUp;
            downstairsButton.OnRightMouseDown += DownstairsButton_OnRightMouseDown;
            downstairsButton.OnRightMouseUp += DownstairsButton_OnRightMouseUp;
            downstairsButton.ToolTip = defaultToolTip;

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

            // status bar
            labelHoverText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 192), "", NativePanel);
            labelHoverText.MaxWidth = 320;
            labelHoverText.TextScale = 1.0f;
            labelHoverText.MaxCharacters = 64;
            labelHoverText.HorizontalAlignment = HorizontalAlignment.Center;
            labelHoverText.HorizontalTextAlignment = TextLabel.HorizontalTextAlignmentSetting.Center;

            if (defaultToolTip != null)
            {
                gridButton.ToolTip.ToolTipDelay = toolTipDelay;
                forwardButton.ToolTip.ToolTipDelay = toolTipDelay;
                backwardButton.ToolTip.ToolTipDelay = toolTipDelay;
                leftButton.ToolTip.ToolTipDelay = toolTipDelay;
                rightButton.ToolTip.ToolTipDelay = toolTipDelay;
                rotateLeftButton.ToolTip.ToolTipDelay = toolTipDelay;
                rotateRightButton.ToolTip.ToolTipDelay = toolTipDelay;
                upstairsButton.ToolTip.ToolTipDelay = toolTipDelay;
                downstairsButton.ToolTip.ToolTipDelay = toolTipDelay;
                dummyPanelCompass.ToolTip.ToolTipDelay = toolTipDelay;
            }

            // compass            
            compass = new HUDCompass();
            Vector2 scale = NativePanel.LocalScale;
            compass.Position = dummyPanelCompass.Rectangle.position;
            compass.Scale = scale;
            NativePanel.Components.Add(compass);

            isSetup = true;
        }

        /// <summary>
        /// called when automap window is pushed - resets automap settings to default settings and signals Automap class
        /// </summary>
        public override void OnPush()
        {
            InitGlobalResources(); // initialize gameobjectAutomap, daggerfallAutomap and layerAutomap

            if (!isSetup) // if Setup() has not run, run it now
            {
                Setup();

                // reset values to default on first automap window open
                automap.SlicingBiasY = defaultSlicingBiasY; // reset slicing y-bias
                //ResetRotationPivotAxisPosition(); // reset rotation pivot axis                
            }

            // check if global automap open/close hotkey has changed
            if (InputManager.Instance.GetBinding(InputManager.Actions.AutoMap) != automapBinding)
            {
                automapBinding = InputManager.Instance.GetBinding(InputManager.Actions.AutoMap);

                // update hotkey sequences taking current toggleClosedBinding into account
                SetupHotkeySequences();
                // update button tool tip texts - since hotkeys changed
                UpdateButtonToolTipsText();
            }

            automap.IsOpenAutomap = true; // signal Automap script that automap is open and it should do its stuff in its Update() function            

            automap.UpdateAutomapStateOnWindowPush(); // signal Automap script that automap window was opened and that it should update its state (updates player marker arrow)

            // get automap camera
            cameraAutomap = automap.CameraAutomap;

            // create automap render texture and Texture2D used in conjuction with automap camera to render automap level geometry and display it in panel
            Rect positionPanelRenderAutomap = dummyPanelAutomap.Rectangle;
            CreateAutomapTextures((int)positionPanelRenderAutomap.width, (int)positionPanelRenderAutomap.height);

            switch (automapViewMode)
            {
                case AutomapViewMode.View2D: default:
                    cameraAutomap.fieldOfView = fieldOfViewCameraMode2D;
                    cameraAutomap.nearClipPlane = nearClipPlaneCameraMode2D;
                    break;
                case AutomapViewMode.View3D:
                    cameraAutomap.fieldOfView = fieldOfViewCameraMode3D;
                    cameraAutomap.nearClipPlane = nearClipPlaneCameraMode3D;
                    break;
            }

            if (compass != null)
            {
                compass.CompassCamera = cameraAutomap;
            }

            if (!DaggerfallUnity.Settings.AutomapRememberSliceLevel)
            {
                // reset values to default whenever automap window is opened
                automap.SlicingBiasY = defaultSlicingBiasY; // reset slicing y-bias
            }

            // reset rotation pivot axis position to player position
            ResetRotationPivotAxisPosition(); // reset rotation pivot axis

            if (automap.ResetAutomapSettingsSignalForExternalScript == true) // signaled to reset automap settings
            {
                // get initial values for camera transform for view from top
                ResetCameraTransformViewFromTop();
                SaveCameraTransformViewFromTop();

                // get initial values for camera transform for 3D view
                ResetCameraTransformView3D();
                SaveCameraTransformView3D();

                // reset values to default whenever player enters building or dungeon
                ResetCameraPosition();
                fieldOfViewCameraMode3D = defaultFieldOfViewCameraMode3D;

                // set standard automap render mode dependent where player is
                if (GameManager.Instance.IsPlayerInsideBuilding) // if inside building
                {
                    ActionSwitchToAutomapRenderModeCutout(); // set automap render mode to cutout (since floors above the current are often distracting when viewing a building)
                }
                else // if inside dungeon or palace
                {
                    ActionSwitchToAutomapRenderModeTransparent(); // set automap render mode to transparent (since people that don't know the map functionality often think cutout mode is a bug)
                }

                // reset rotation pivot axis position
                ResetRotationPivotAxisPosition();

                // reset slicing y-bias
                automap.SlicingBiasY = defaultSlicingBiasY;

                automap.ResetAutomapSettingsSignalForExternalScript = false; // indicate the settings were reset
            }
            else
            {
                // backup view mode
                AutomapViewMode backupValueAutomapViewMode = automapViewMode;

                // focus player in 2D view mode - but keep old camera orientation of 2D view mode camera transform                
                automapViewMode = AutomapViewMode.View2D; // need to change view mode so that SwitchFocusToGameObject() does the correct thing
                RestoreOldCameraTransformViewFromTop();
                SwitchFocusToGameObject(gameObjectPlayerAdvanced);
                SaveCameraTransformViewFromTop();

                // focus player in 3D view mode - but keep old camera orientation of 3D view mode camera transform
                automapViewMode = AutomapViewMode.View3D; // need to change view mode so that SwitchFocusToGameObject() does the correct thing
                RestoreOldCameraTransformView3D();
                SwitchFocusToGameObject(gameObjectPlayerAdvanced);
                SaveCameraTransformView3D();

                // restore view mode
                automapViewMode = backupValueAutomapViewMode;

                switch (automapViewMode)
                {
                    case AutomapViewMode.View2D:
                    default:
                        RestoreOldCameraTransformViewFromTop();
                        break;
                    case AutomapViewMode.View3D:
                        RestoreOldCameraTransformView3D();
                        break;
                }
            }

            // and update the automap view
            UpdateAutomapView();
        }

        /// <summary>
        /// called when automap window is popped - destroys resources and signals Automap class
        /// </summary>
        public override void OnPop()
        {
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                default:
                    SaveCameraTransformViewFromTop();
                    break;
                case AutomapViewMode.View3D:
                    SaveCameraTransformView3D();
                    break;
            }

            automap.IsOpenAutomap = false; // signal Automap script that automap was closed

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

            automap.UpdateAutomapStateOnWindowPop(); // signal Automap script that automap window was closed
        }

        /// <summary>
        /// reacts on left/right mouse button down states over different automap buttons and other GUI elements
        /// handles resizing of NativePanel as well
        /// </summary>
        public override void Update()
        {
            // check if iTween camera animation is running
            if (automap.ITweenCameraAnimationIsRunning)
            {
                // update oldMousePosition to prevent problems with drag and drog action that starts before animation is over 
                oldMousePosition = new Vector2(InputManager.Instance.MousePosition.x, Screen.height - InputManager.Instance.MousePosition.y);

                // if so update automap view so animation plays correctly
                UpdateAutomapView();
                // and then return and do nothing else (until animation is finished no control commands can be issued)
                return;
            }

            base.Update();
            ResizeGUIelementsOnDemand();

            HotkeySequence.KeyModifiers keyModifiers = HotkeySequence.GetKeyboardKeyModifiers();

            if (InputManager.Instance.GetBackButtonDown() ||
                // Toggle window closed with same hotkey used to open it
                InputManager.Instance.GetKeyDown(automapBinding))
                isCloseWindowDeferred = true;
            else if ((InputManager.Instance.GetBackButtonUp() ||
                // Toggle window closed with same hotkey used to open it
                InputManager.Instance.GetKeyUp(automapBinding)) && isCloseWindowDeferred)
            {
                isCloseWindowDeferred = false;
                CloseWindow();
                return;
            }

            // debug teleport mode action
            if (
                (automap.DebugTeleportMode == true) &&
                leftMouseClickedOnPanelAutomap && // make sure click happened in panel area
                InputManager.Instance.GetMouseButtonDown(0) && // make sure click was issued in this frame
                ((Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift)) || (Input.GetKey(KeyCode.RightControl) && Input.GetKey(KeyCode.RightShift)))
               )
            {
                //Vector2 mousePosition = new Vector2((InputManager.Instance.MousePosition.x / Screen.width) * panelRenderAutomap.Size.x, (InputManager.Instance.MousePosition.y / Screen.height) * panelRenderAutomap.Size.y);
                Vector2 mousePosition = panelRenderAutomap.ScaledMousePosition;
                mousePosition.y = panelRenderAutomap.Size.y - mousePosition.y;
                automap.TryTeleportPlayerToDungeonSegmentAtScreenPosition(mousePosition);
                UpdateAutomapView();
            }

            // check hotkeys and assign actions
            if (HotkeySequence_SwitchAutomapGridMode.IsDownWith(keyModifiers))
            {
                ActionChangeAutomapGridMode();
            }
            if (HotkeySequence_ResetView.IsDownWith(keyModifiers))
            {
                ActionResetView();
            }
            if (HotkeySequence_ResetRotationPivotAxisView.IsDownWith(keyModifiers))
            {
                ActionResetRotationPivotAxis();
            }
            if (HotkeySequence_SwitchFocusToNextBeaconObject.IsDownWith(keyModifiers))
            {
                ActionSwitchFocusToNextBeaconObject();
            }
            if (HotkeySequence_SwitchToNextAutomapRenderMode.IsDownWith(keyModifiers))
            {
                ActionSwitchToNextAutomapRenderMode();
            }

            if (HotkeySequence_SwitchToAutomapRenderModeTransparent.IsDownWith(keyModifiers))
            {
                ActionSwitchToAutomapRenderModeTransparent();
            }
            if (HotkeySequence_SwitchToAutomapRenderModeWireframe.IsDownWith(keyModifiers))
            {
                ActionSwitchToAutomapRenderModeWireframe();
            }
            if (HotkeySequence_SwitchToAutomapRenderModeCutout.IsDownWith(keyModifiers))
            {
                ActionSwitchToAutomapRenderModeCutout();
            }

            if (HotkeySequence_SwitchToAutomapBackgroundOriginal.IsDownWith(keyModifiers))
            {
                ActionSwitchToAutomapBackgroundOriginal();
            }
            if (HotkeySequence_SwitchToAutomapBackgroundAlternative1.IsDownWith(keyModifiers))
            {
                ActionSwitchToAutomapBackgroundAlternative1();
            }
            if (HotkeySequence_SwitchToAutomapBackgroundAlternative2.IsDownWith(keyModifiers))
            {
                ActionSwitchToAutomapBackgroundAlternative2();
            }
            if (HotkeySequence_SwitchToAutomapBackgroundAlternative3.IsDownWith(keyModifiers))
            {
                ActionSwitchToAutomapBackgroundAlternative3();
            }


            if (HotkeySequence_MoveForward.IsPressedWith(keyModifiers))
            {
                ActionMoveForward();
            }
            if (HotkeySequence_MoveBackward.IsPressedWith(keyModifiers))
            {
                ActionMoveBackward();
            }
            if (HotkeySequence_MoveLeft.IsPressedWith(keyModifiers))
            {
                ActionMoveLeft();
            }
            if (HotkeySequence_MoveRight.IsPressedWith(keyModifiers))
            {
                ActionMoveRight();
            }
            if (HotkeySequence_MoveRotationPivotAxisForward.IsPressedWith(keyModifiers))
            {
                ActionMoveRotationPivotAxisForward();
            }
            if (HotkeySequence_MoveRotationPivotAxisBackward.IsPressedWith(keyModifiers))
            {
                ActionMoveRotationPivotAxisBackward();
            }
            if (HotkeySequence_MoveRotationPivotAxisLeft.IsPressedWith(keyModifiers))
            {
                ActionMoveRotationPivotAxisLeft();
            }
            if (HotkeySequence_MoveRotationPivotAxisRight.IsPressedWith(keyModifiers))
            {
                ActionMoveRotationPivotAxisRight();
            }
            if (HotkeySequence_RotateLeft.IsPressedWith(keyModifiers))
            {
                ActionRotateLeft();
            }
            if (HotkeySequence_RotateRight.IsPressedWith(keyModifiers))
            {
                ActionRotateRight();
            }
            if (HotkeySequence_RotateCameraLeft.IsPressedWith(keyModifiers))
            {
                ActionRotateCamera(rotateCameraSpeed);
            }
            if (HotkeySequence_RotateCameraRight.IsPressedWith(keyModifiers))
            {
                ActionRotateCamera(-rotateCameraSpeed);
            }
            if (HotkeySequence_RotateCameraOnCameraYZplaneAroundObjectUp.IsPressedWith(keyModifiers))
            {
                ActionrotateCameraOnCameraYZplaneAroundObject(rotateCameraOnCameraYZplaneAroundObjectSpeedInView3D);
            }
            if (HotkeySequence_RotateCameraOnCameraYZplaneAroundObjectDown.IsPressedWith(keyModifiers))
            {
                ActionrotateCameraOnCameraYZplaneAroundObject(-rotateCameraOnCameraYZplaneAroundObjectSpeedInView3D);
            }
            if (HotkeySequence_Upstairs.IsPressedWith(keyModifiers))
            {
                ActionMoveUpstairs();
            }
            if (HotkeySequence_Downstairs.IsPressedWith(keyModifiers))
            {
                ActionMoveDownstairs();
            }
            if (HotkeySequence_IncreaseSliceLevel.IsPressedWith(keyModifiers))
            {
                ActionIncreaseSliceLevel();
            }
            if (HotkeySequence_DecreaseSliceLevel.IsPressedWith(keyModifiers))
            {
                ActionDecreaseSliceLevel();
            }
            if (HotkeySequence_ZoomIn.IsPressedWith(keyModifiers))
            {
                ActionZoomIn(zoomSpeed * Time.unscaledDeltaTime);
            }
            if (HotkeySequence_ZoomOut.IsPressedWith(keyModifiers))
            {
                ActionZoomOut(zoomSpeed * Time.unscaledDeltaTime);
            }
            if (HotkeySequence_IncreaseCameraFieldOfFiew.IsPressedWith(keyModifiers))
            {
                ActionIncreaseCameraFieldOfView();
            }
            if (HotkeySequence_DecreaseCameraFieldOfFiew.IsPressedWith(keyModifiers))
            {
                ActionDecreaseCameraFieldOfView();
            }

            // check mouse input and assign actions
            if (leftMouseDownOnPanelAutomap)
            {
                Vector2 mousePosition = new Vector2(InputManager.Instance.MousePosition.x, Screen.height - InputManager.Instance.MousePosition.y);

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
                UpdateAutomapView();
                oldMousePosition = mousePosition;
            }

            if (rightMouseDownOnPanelAutomap)
            {
                Vector2 mousePosition = new Vector2(InputManager.Instance.MousePosition.x, Screen.height - InputManager.Instance.MousePosition.y);

                Vector2 bias = mousePosition - oldMousePosition;

                if (bias != Vector2.zero)
                {
                    switch (automapViewMode)
                    {
                        case AutomapViewMode.View2D:
                        default:
                            ActionRotateCamera(+dragRotateSpeedInTopView * bias.x, false);
                            break;
                        case AutomapViewMode.View3D:
                            ActionRotate(dragRotateSpeedInView3D * bias.x, false);
                            ActionrotateCameraOnCameraYZplaneAroundObject(-dragRotateCameraOnCameraYZplaneAroundObjectSpeedInView3D * bias.y, false);
                            break;
                    }
                    UpdateAutomapView();
                }
                oldMousePosition = mousePosition;
            }

            if (middleMouseDownOnPanelAutomap)
            {
                Vector2 mousePosition = new Vector2(InputManager.Instance.MousePosition.x, Screen.height - InputManager.Instance.MousePosition.y);

                Vector2 bias = mousePosition - oldMousePosition;

                ActionMoveSliceLevel(bias.y);

                UpdateAutomapView();
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

            UpdateMouseHoverOverText();
            UpdateMouseHoverOverGameObjects();
        }

        #region Private Methods

        /// <summary>
        /// updates the mouse hover over text in the status bar in the bottom of the automap window
        /// </summary>
        private void UpdateMouseHoverOverText()
        {
            Vector2 mousePosition = panelRenderAutomap.ScaledMousePosition;
            mousePosition.y = panelRenderAutomap.Size.y - mousePosition.y;
            string hoverOverText = automap.GetMouseHoverOverText(mousePosition);
            labelHoverText.Text = hoverOverText;
        }

        /// <summary>
        /// updates the mouse hover over gameobjects in the automap window (e.g. connections of portals)
        /// </summary>
        private void UpdateMouseHoverOverGameObjects()
        {
            Vector2 mousePosition = panelRenderAutomap.ScaledMousePosition;
            mousePosition.y = panelRenderAutomap.Size.y - mousePosition.y;
            if (automap.UpdateMouseHoverOverGameObjects(mousePosition))
                UpdateAutomapView();
        }

        /// <summary>
        /// tests for availability and initializes class resources like GameObject for automap, Automap class and layerAutomap
        /// </summary>
        private void InitGlobalResources()
        {
            if (!gameobjectAutomap)
            {
                gameobjectAutomap = GameObject.Find("Automap/InteriorAutomap");
                if (gameobjectAutomap == null)
                {
                    DaggerfallUnity.LogMessage("GameObject \"Automap/InteriorAutomap\" missing! Create a GameObject called \"Automap\" in root of hierarchy and add a GameObject \"InternalAutomap\" to it, to this add script Game/Automap!\"", true);
                }
            }

            if (!automap)
            {
                automap = gameobjectAutomap.GetComponent<Automap>();
                if (automap == null)
                {
                    DaggerfallUnity.LogMessage("Script Automap is missing in GameObject \"Automap\"! GameObject \"Automap\" must have script Game/Automap attached!\"", true);
                }
            }

            gameObjectPlayerAdvanced = GameObject.Find("PlayerAdvanced");
            if (!gameObjectPlayerAdvanced)
            {
                DaggerfallUnity.LogMessage("GameObject \"PlayerAdvanced\" not found! in script AutomapWindow (in function initGlobalResources())", true);
                if (Application.isEditor)
                    Debug.Break();
                else
                    Application.Quit();
            }
        }

        /// <summary>
        /// resizes GUI elements automap render panel (and the needed RenderTexture and Texture2D) and compass
        /// </summary>
        private void ResizeGUIelementsOnDemand()
        {
            if (oldPositionNativePanel != NativePanel.Rectangle || oldCustomScreenRect != DaggerfallUI.Instance.CustomScreenRect)
            {
                // get panelRenderAutomap position and size from dummyPanelAutomap rectangle
                if (DaggerfallUI.Instance.CustomScreenRect == null)
                    panelRenderAutomap.Position = dummyPanelAutomap.Rectangle.position;
                else
                    panelRenderAutomap.Position = dummyPanelAutomap.ScreenToLocal(dummyPanelAutomap.Rectangle.position);
                //panelRenderAutomap.Size = new Vector2(dummyPanelAutomap.InteriorWidth, dummyPanelAutomap.InteriorHeight);
                panelRenderAutomap.Size = new Vector2(dummyPanelAutomap.Rectangle.width, dummyPanelAutomap.Rectangle.height);

                // get panelRenderOverlay position and size from dummyPanelOverlay rectangle
                if (DaggerfallUI.Instance.CustomScreenRect == null)
                    panelRenderOverlay.Position = dummyPanelOverlay.Rectangle.position;
                else
                    panelRenderAutomap.Position = dummyPanelAutomap.ScreenToLocal(dummyPanelAutomap.Rectangle.position);
                panelRenderOverlay.Size = new Vector2(dummyPanelOverlay.Rectangle.width, dummyPanelOverlay.Rectangle.height);

                //Debug.Log(String.Format("dummy panel size: {0}, {1}; {2}, {3}; {4}, {5}; {6}, {7}\n", NativePanel.InteriorWidth, NativePanel.InteriorHeight, ParentPanel.InteriorWidth, ParentPanel.InteriorHeight, dummyPanelAutomap.InteriorWidth, dummyPanelAutomap.InteriorHeight, parentPanel.InteriorWidth, parentPanel.InteriorHeight));
                //Debug.Log(String.Format("dummy panel pos: {0}, {1}; {2}, {3}; {4}, {5}; {6}, {7}\n", NativePanel.Rectangle.xMin, NativePanel.Rectangle.yMin, ParentPanel.Rectangle.xMin, ParentPanel.Rectangle.yMin, dummyPanelAutomap.Rectangle.xMin, dummyPanelAutomap.Rectangle.yMin, parentPanel.Rectangle.xMin, parentPanel.Rectangle.yMin));
                //Vector2 positionPanelRenderAutomap = new Vector2(dummyPanelAutomap.InteriorWidth, dummyPanelAutomap.InteriorHeight);
                Vector2 positionPanelRenderAutomap = new Vector2(dummyPanelAutomap.Rectangle.width, dummyPanelAutomap.Rectangle.height);
                CreateAutomapTextures((int)positionPanelRenderAutomap.x, (int)positionPanelRenderAutomap.y);
                UpdateAutomapView();

                // get compass position from dummyPanelCompass rectangle
                Vector2 scale = NativePanel.LocalScale;
                compass.Position = dummyPanelCompass.Rectangle.position;
                compass.Scale = scale;

                oldPositionNativePanel = NativePanel.Rectangle;
                oldCustomScreenRect = DaggerfallUI.Instance.CustomScreenRect;
            }
        }

        /// <summary>
        /// creates RenderTexture and Texture2D with the required size if it is not present or its old size differs from the expected size
        /// </summary>
        /// <param name="width"> the expected width of the RenderTexture and Texture2D </param>
        /// <param name="height"> the expected height of the RenderTexture and Texture2D </param>
        private void CreateAutomapTextures(int width, int height)
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
        private void ResetCameraPosition()
        {
            // then set camera transform according to grid-button (view mode) setting
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D: default:
                    ResetCameraTransformViewFromTop();
                    break;
                case AutomapViewMode.View3D:
                    ResetCameraTransformView3D();
                    break;
            }
        }

        /// <summary>
        /// resets the camera transform of the 2D view mode
        /// </summary>
        private void ResetCameraTransformViewFromTop()
        {
            cameraAutomap.transform.position = Camera.main.transform.position + Vector3.up * cameraHeightViewFromTop;
            cameraAutomap.transform.LookAt(Camera.main.transform.position);
            SaveCameraTransformViewFromTop();
        }

        /// <summary>
        /// resets the camera transform of the 3D view mode
        /// </summary>
        private void ResetCameraTransformView3D()
        {
            Vector3 viewDirectionInXZ = Vector3.forward;
            cameraAutomap.transform.position = Camera.main.transform.position - viewDirectionInXZ * cameraBackwardDistance + Vector3.up * cameraHeightView3D;
            cameraAutomap.transform.LookAt(Camera.main.transform.position);
            SaveCameraTransformView3D();
        }

        /// <summary>
        /// saves the camera transform of the 2D view mode
        /// </summary>
        private void SaveCameraTransformViewFromTop()
        {
            backupCameraPositionViewFromTop = cameraAutomap.transform.position;
            backupCameraRotationViewFromTop = cameraAutomap.transform.rotation;
        }

        /// <summary>
        /// saves the camera transform of the 3D view mode
        /// </summary>
        private void SaveCameraTransformView3D()
        {
            backupCameraPositionView3D = cameraAutomap.transform.position;
            backupCameraRotationView3D = cameraAutomap.transform.rotation;
        }

        /// <summary>
        /// restores the camera transform of the 2D view mode
        /// </summary>
        private void RestoreOldCameraTransformViewFromTop()
        {
            cameraAutomap.transform.position = backupCameraPositionViewFromTop;
            cameraAutomap.transform.rotation = backupCameraRotationViewFromTop;
        }

        /// <summary>
        /// restores the camera transform of the 3D view mode
        /// </summary>
        private void RestoreOldCameraTransformView3D()
        {
            cameraAutomap.transform.position = backupCameraPositionView3D;
            cameraAutomap.transform.rotation = backupCameraRotationView3D;
        }

        /// <summary>
        /// resets the rotation pivot axis position for the 2D view mode
        /// </summary>
        private void ResetRotationPivotAxisPositionViewFromTop()
        {
            rotationPivotAxisPositionViewFromTop = gameObjectPlayerAdvanced.transform.position;
            automap.RotationPivotAxisPosition = rotationPivotAxisPositionViewFromTop;
        }

        /// <summary>
        /// resets the rotation pivot axis position for the 3D view mode
        /// </summary>
        private void ResetRotationPivotAxisPositionView3D()
        {
            rotationPivotAxisPositionView3D = gameObjectPlayerAdvanced.transform.position;
            automap.RotationPivotAxisPosition = rotationPivotAxisPositionView3D;
        }

        /// <summary>
        /// resets the rotation pivot axis position for both view modes
        /// </summary>
        private void ResetRotationPivotAxisPosition()
        {
            ResetRotationPivotAxisPositionViewFromTop();
            ResetRotationPivotAxisPositionView3D();
        }

        /// <summary>
        /// shifts the rotation pivot axis position for the currently selected view mode
        /// </summary>
        /// <param name="translation"> the translation for the shift </param>
        private void ShiftRotationPivotAxisPosition(Vector3 translation)
        {
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    rotationPivotAxisPositionViewFromTop += translation;
                    automap.RotationPivotAxisPosition = rotationPivotAxisPositionViewFromTop;
                    break;
                case AutomapViewMode.View3D:
                    rotationPivotAxisPositionView3D += translation;
                    automap.RotationPivotAxisPosition = rotationPivotAxisPositionView3D;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// gets the rotation pivot axis position for the currently selected view mode
        /// </summary>
        /// <returns> the position of the rotation pivot axis </returns>
        private Vector3 GetRotationPivotAxisPosition()
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
        /// updates the automap view - signals Automap class to update and renders the automap level geometry afterwards into the automap render panel
        /// </summary>
        private void UpdateAutomapView()
        {
            automap.ForceUpdate();

            // update rotation of pivot axis (which will affect its child objects i.e. the rotation indicator arrows)
            automap.RotationPivotAxisRotation = Quaternion.Euler(0.0f, cameraAutomap.transform.rotation.eulerAngles.y, 0.0f);

            if ((!cameraAutomap) || (!renderTextureAutomap))
                return;

            cameraAutomap.Render();

            RenderTexture.active = renderTextureAutomap;
            textureAutomap.ReadPixels(new Rect(0, 0, renderTextureAutomap.width, renderTextureAutomap.height), 0, 0);
            textureAutomap.Apply(false);
            RenderTexture.active = null;

            panelRenderAutomap.BackgroundTexture = textureAutomap;

            panelRenderOverlay.BackgroundTexture = automap.TextureMicroMap;
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
            UpdateAutomapView();
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
            UpdateAutomapView();
        }

        /// <summary>
        /// action for move left
        /// </summary>
        private void ActionMoveLeft()
        {
            Vector3 translation = -cameraAutomap.transform.right * scrollLeftRightSpeed * Time.unscaledDeltaTime;
            translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector
            cameraAutomap.transform.position += translation;
            UpdateAutomapView();
        }

        /// <summary>
        /// action for move right
        /// </summary>
        private void ActionMoveRight()
        {
            Vector3 translation = cameraAutomap.transform.right * scrollLeftRightSpeed * Time.unscaledDeltaTime;
            translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector
            cameraAutomap.transform.position += translation;
            UpdateAutomapView();
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
                    translation.y = 0.0f; // do not move the axis in y direction (so rotation arrows stay in vertical place)
                    break;
                default:
                    translation = Vector3.zero;
                    break;
            }
            ShiftRotationPivotAxisPosition(translation);
            UpdateAutomapView();
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
                    translation.y = 0.0f; // do not move the axis in y direction (so rotation arrows stay in vertical place)
                    break;
                default:
                    translation = Vector3.zero;
                    break;
            }
            ShiftRotationPivotAxisPosition(translation);
            UpdateAutomapView();
        }

        /// <summary>
        /// action for moving rotation pivot axis left
        /// </summary>
        private void ActionMoveRotationPivotAxisLeft()
        {
            Vector3 translation = -cameraAutomap.transform.right * moveRotationPivotAxisMarkerLeftRightSpeed * Time.unscaledDeltaTime;
            //translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector
            ShiftRotationPivotAxisPosition(translation);
            UpdateAutomapView();
        }

        /// <summary>
        /// action for moving rotation pivot axis right
        /// </summary>
        private void ActionMoveRotationPivotAxisRight()
        {
            Vector3 translation = cameraAutomap.transform.right * moveRotationPivotAxisMarkerLeftRightSpeed * Time.unscaledDeltaTime;
            //translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector                
            ShiftRotationPivotAxisPosition(translation);
            UpdateAutomapView();
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
            //UpdateAutomapView();
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
            //UpdateAutomapView();
            ActionRotate(-rotateSpeed);
        }

        /// <summary>
        /// action for rotating camera around rotation axis about a certain rotationAmount
        /// </summary>
        private void ActionRotate(float rotationAmount, bool updateView = true)
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
            if (updateView)
                UpdateAutomapView();
        }

        /// <summary>
        /// action for rotating camera on camera YZ-plane around object about a certain rotationAmount
        /// </summary>
        private void ActionrotateCameraOnCameraYZplaneAroundObject(float rotationAmount, bool updateView = true)
        {
            if (automapViewMode == AutomapViewMode.View3D)
            {
                Vector3 rotationPoint = rotationPivotAxisPositionView3D;
                cameraAutomap.transform.RotateAround(rotationPoint, cameraAutomap.transform.right, -rotationAmount * Time.unscaledDeltaTime);
                // Prevent the map from being seen upside-down
                Vector3 transformedUp = cameraAutomap.transform.TransformDirection(Vector3.up);
                if (transformedUp.y < 0)
                {
                    float rotateBack = Vector3.SignedAngle(transformedUp, Vector3.ProjectOnPlane(transformedUp, Vector3.up), cameraAutomap.transform.right);
                    cameraAutomap.transform.RotateAround(rotationPoint, cameraAutomap.transform.right, rotateBack);
                }
                if (updateView)
                    UpdateAutomapView(); 
            }
        }

        /// <summary>
        /// action for changing camera rotation around y axis
        /// </summary>
        /// <param name="rotationSpeed"> amount used for rotation </param>
        private void ActionRotateCamera(float rotationAmount, bool updateView = true)
        {
            //cameraAutomap.transform.Rotate(0.0f, rotationAmount * Time.unscaledDeltaTime, 0.0f, Space.World);
            Vector3 vecRotationCenter = cameraAutomap.transform.position;
            cameraAutomap.transform.RotateAround(vecRotationCenter, Vector3.up, -rotationAmount * Time.unscaledDeltaTime);

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

            if (updateView)
                UpdateAutomapView();
        }

        /// <summary>
        /// action for moving upstairs
        /// </summary>
        private void ActionMoveUpstairs()
        {
            cameraAutomap.transform.position += Vector3.up * moveUpDownSpeed * Time.unscaledDeltaTime;
            UpdateAutomapView();
        }

        /// <summary>
        /// action for moving downstairs
        /// </summary>
        private void ActionMoveDownstairs()
        {
            cameraAutomap.transform.position += Vector3.down * moveUpDownSpeed * Time.unscaledDeltaTime;
            UpdateAutomapView();
        }

        /// <summary>
        /// action for increasing slice level
        /// </summary>
        private void ActionIncreaseSliceLevel()
        {
            automap.SlicingBiasY += Vector3.up.y * moveUpDownSpeed * Time.unscaledDeltaTime;
            UpdateAutomapView();
        }

        /// <summary>
        /// action for decreasing slice level
        /// </summary>
        private void ActionDecreaseSliceLevel()
        {
            automap.SlicingBiasY += Vector3.down.y * moveUpDownSpeed * Time.unscaledDeltaTime;
            UpdateAutomapView();
        }

        /// <summary>
        /// action for moving (adjusting) slice level relative to current level
        /// </summary>
        private void ActionMoveSliceLevel(float bias)
        {
            automap.SlicingBiasY += Vector3.down.y * bias * Time.unscaledDeltaTime;
            UpdateAutomapView();
        }

        /// <summary>
        /// action for zooming in
        /// </summary>
        private void ActionZoomIn(float zoomSpeed)
        {
            float zoomSpeedCompensated = zoomSpeed * Vector3.Magnitude(Camera.main.transform.position - cameraAutomap.transform.position);
            Vector3 translation = cameraAutomap.transform.forward * zoomSpeedCompensated;
            cameraAutomap.transform.position += translation;
            UpdateAutomapView();
        }

        /// <summary>
        /// action for zooming out
        /// </summary>
        private void ActionZoomOut(float zoomSpeed)
        {
            float zoomSpeedCompensated = zoomSpeed * Vector3.Magnitude(Camera.main.transform.position - cameraAutomap.transform.position);
            Vector3 translation = -cameraAutomap.transform.forward * zoomSpeedCompensated;
            cameraAutomap.transform.position += translation;
            UpdateAutomapView();
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
                cameraAutomap.nearClipPlane = nearClipPlaneCameraMode3D;
                UpdateAutomapView();
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
                cameraAutomap.nearClipPlane = nearClipPlaneCameraMode3D;
                UpdateAutomapView();
            }
        }

        /// <summary>
        /// action for switching to next automap render mode
        /// </summary>
        private void ActionSwitchToNextAutomapRenderMode()
        {
            automap.SwitchToNextAutomapRenderMode();
            UpdateAutomapView();
        }

        /// <summary>
        /// action for switching to automap render mode "transparent"
        /// </summary>
        private void ActionSwitchToAutomapRenderModeTransparent()
        {
            automap.SwitchToAutomapRenderModeTransparent();
            UpdateAutomapView();
        }

        /// <summary>
        /// action for switching to automap render mode "wireframe"
        /// </summary>
        private void ActionSwitchToAutomapRenderModeWireframe()
        {
            automap.SwitchToAutomapRenderModeWireframe();
            UpdateAutomapView();
        }

        /// <summary>
        /// action for switching to automap render mode "cutout"
        /// </summary>
        private void ActionSwitchToAutomapRenderModeCutout()
        {
            automap.SwitchToAutomapRenderModeCutout();
            UpdateAutomapView();
        }

        /// <summary>
        /// action for switching to automap background to original background
        /// </summary>
        private void ActionSwitchToAutomapBackgroundOriginal()
        {
            dummyPanelAutomap.BackgroundTexture = null;
            UpdateAutomapView();
        }

        /// <summary>
        /// action for switching to automap background to background alternative 1
        /// </summary>
        private void ActionSwitchToAutomapBackgroundAlternative1()
        {
            dummyPanelAutomap.BackgroundTexture = textureBackgroundAlternative1;
            UpdateAutomapView();
        }

        /// <summary>
        /// action for switching to automap background to background alternative 2
        /// </summary>
        private void ActionSwitchToAutomapBackgroundAlternative2()
        {
            dummyPanelAutomap.BackgroundTexture = textureBackgroundAlternative2;
            UpdateAutomapView();
        }

        /// <summary>
        /// action for switching to automap background to background alternative 3
        /// </summary>
        private void ActionSwitchToAutomapBackgroundAlternative3()
        {
            dummyPanelAutomap.BackgroundTexture = textureBackgroundAlternative3;
            UpdateAutomapView();
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
                    gridButton.BackgroundTexture = nativeTextureGrid2D;
                    SaveCameraTransformView3D();
                    RestoreOldCameraTransformViewFromTop();
                    cameraAutomap.fieldOfView = fieldOfViewCameraMode2D;
                    cameraAutomap.nearClipPlane = nearClipPlaneCameraMode2D;
                    automap.RotationPivotAxisPosition = rotationPivotAxisPositionViewFromTop;
                    UpdateAutomapView();
                    break;
                case AutomapViewMode.View3D:
                    // update grid graphics
                    gridButton.BackgroundTexture = nativeTextureGrid3D;
                    SaveCameraTransformViewFromTop();
                    RestoreOldCameraTransformView3D();
                    cameraAutomap.fieldOfView = fieldOfViewCameraMode3D;
                    cameraAutomap.nearClipPlane = nearClipPlaneCameraMode3D;
                    automap.RotationPivotAxisPosition = rotationPivotAxisPositionView3D;
                    UpdateAutomapView();
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
            automap.SlicingBiasY = defaultSlicingBiasY; // reset slicing y-bias
            ResetCameraPosition();
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    cameraAutomap.fieldOfView = fieldOfViewCameraMode2D;
                    cameraAutomap.nearClipPlane = nearClipPlaneCameraMode2D;
                    ResetRotationPivotAxisPositionViewFromTop(); // reset rotation pivot axis                 
                    break;
                case AutomapViewMode.View3D:
                    cameraAutomap.fieldOfView = fieldOfViewCameraMode3D;
                    cameraAutomap.nearClipPlane = nearClipPlaneCameraMode3D;
                    ResetRotationPivotAxisPositionView3D(); // reset rotation pivot axis
                    break;
            }            
            UpdateAutomapView();
        }

        /// <summary>
        /// action for reset rotation pivot axis
        /// </summary>
        private void ActionResetRotationPivotAxis()
        {
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    ResetRotationPivotAxisPositionViewFromTop();
                    UpdateAutomapView();
                    break;
                case AutomapViewMode.View3D:
                    ResetRotationPivotAxisPositionView3D();
                    UpdateAutomapView();
                    break;
                default:
                    break;
            }
            UpdateAutomapView();
        }


        /// <summary>
        /// action for switching focus to next beacon object
        /// </summary>
        private void ActionSwitchFocusToNextBeaconObject()
        {
            GameObject gameobjectInFocus = automap.SwitchFocusToNextObject();
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
                    UpdateAutomapView();
                    break;
                case AutomapViewMode.View3D:
                    float computedCameraBackwardDistance = Vector3.Magnitude(cameraAutomap.transform.position - gameobjectInFocus.transform.position);
                    newPosition = gameobjectInFocus.transform.position - cameraAutomap.transform.forward * computedCameraBackwardDistance;                    
                    cameraAutomap.transform.position = newPosition;
                    UpdateAutomapView();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Event Handlers

        private void PanelAutomap_OnMouseScrollUp(BaseScreenComponent sender)
        {
            ActionZoomIn(zoomSpeedMouseWheel);
        }

        private void PanelAutomap_OnMouseScrollDown(BaseScreenComponent sender)
        {
            ActionZoomOut(zoomSpeedMouseWheel);
        }

        private void PanelAutomap_OnMouseDoubleClick(BaseScreenComponent sender, Vector2 position)
        {
            Vector2 mousePosition = panelRenderAutomap.ScaledMousePosition;
            mousePosition.y = panelRenderAutomap.Size.y - mousePosition.y;
            if (!GameManager.Instance.IsPlayerInsideBuilding)
            {
                // first check for teleporter portal marker hits
                if (automap.TryForTeleporterPortalsAtScreenPosition(mousePosition))
                    return;

                // if no teleporter portal marker was hit, try to add or edit a user marker note
                automap.TryToAddOrEditUserNoteMarkerOnDungeonSegmentAtScreenPosition(mousePosition, !Input.GetKey(KeyCode.LeftControl));
            }
        }

        private void PanelAutomap_OnRightMouseDoubleClick(BaseScreenComponent sender, Vector2 position)
        {            
            Vector2 mousePosition = panelRenderAutomap.ScaledMousePosition;
            mousePosition.y = panelRenderAutomap.Size.y - mousePosition.y;

            // try to remove user note markers
            if (automap.TryToRemoveUserNoteMarkerOnDungeonSegmentAtScreenPosition(mousePosition))
                return; // if successful do nothing more

            //if no user note marker could be removed, positioning of rotation pivot axis was initiated
            automap.TrySetRotationPivotAxisToDungeonSegmentAtScreenPosition(mousePosition);

            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    rotationPivotAxisPositionViewFromTop = automap.RotationPivotAxisPosition;
                    break;
                case AutomapViewMode.View3D:
                    rotationPivotAxisPositionView3D = automap.RotationPivotAxisPosition;
                    break;
                default:
                    break;
            }

            UpdateAutomapView();
        }

        private void PanelAutomap_OnMiddleMouseDoubleClick(BaseScreenComponent sender, Vector2 position)
        {
            Vector2 mousePosition = panelRenderAutomap.ScaledMousePosition;
            mousePosition.y = panelRenderAutomap.Size.y - mousePosition.y;
            automap.TryCenterAutomapCameraOnDungeonSegmentAtScreenPosition(mousePosition);
        }

        private void PanelAutomap_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (alreadyInMouseDown)
                return;

            leftMouseClickedOnPanelAutomap = true; // used for debug teleport mode clicks

            if (automap.DebugTeleportMode && ((Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift)) || (Input.GetKey(KeyCode.RightControl) && Input.GetKey(KeyCode.RightShift))))
                return;

            Vector2 mousePosition = new Vector2(InputManager.Instance.MousePosition.x, Screen.height - InputManager.Instance.MousePosition.y);
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

            Vector2 mousePosition = new Vector2(InputManager.Instance.MousePosition.x, Screen.height - InputManager.Instance.MousePosition.y);
            oldMousePosition = mousePosition;
            rightMouseDownOnPanelAutomap = true;
            alreadyInRightMouseDown = true;
        }

        private void PanelAutomap_OnRightMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            rightMouseDownOnPanelAutomap = false;
            alreadyInRightMouseDown = false;
        }

        private void PanelAutomap_OnMiddleMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (alreadyInMiddleMouseDown)
                return;

            Vector2 mousePosition = new Vector2(InputManager.Instance.MousePosition.x, Screen.height - InputManager.Instance.MousePosition.y);
            oldMousePosition = mousePosition;
            middleMouseDownOnPanelAutomap = true;
            alreadyInMiddleMouseDown = true;
        }

        private void PanelAutomap_OnMiddleMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            middleMouseDownOnPanelAutomap = false;
            alreadyInMiddleMouseDown = false;
        }

        private void GridButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode())
                return;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ActionChangeAutomapGridMode();
        }

        private void GridButton_OnRightMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode())
                return;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ActionResetRotationPivotAxis();
        }

        private void GridButton_OnMouseScrollUp(BaseScreenComponent sender)
        {
            if (inDragMode())
                return;

            ActionIncreaseCameraFieldOfView();
        }

        private void GridButton_OnMouseScrollDown(BaseScreenComponent sender)
        {
            if (inDragMode())
                return;

            ActionDecreaseCameraFieldOfView();
        }

        private void ForwardButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode() || alreadyInMouseDown)
                return;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseWindow();
        }

        private void Compass_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode())
                return;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ActionSwitchFocusToNextBeaconObject();
        }

        private void Compass_OnRightMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode())
                return;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ActionResetView();
        }

        #endregion
    }
}
