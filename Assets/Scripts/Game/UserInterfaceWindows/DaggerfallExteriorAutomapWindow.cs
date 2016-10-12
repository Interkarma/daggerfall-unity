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
    public class DaggerfallExteriorAutomapWindow : DaggerfallPopupWindow
    {
        const int toolTipDelay = 1; // delay in seconds before button tooltips are shown

        const float scrollLeftRightSpeed = 100.0f; // left mouse on button arrow left/right makes geometry move with this speed
        const float scrollUpDownSpeed = 100.0f; // left mouse on button arrow up/down makes geometry move with this speed
        const float moveUpstairsDownstairsSpeed = 500.0f; // left mouse on button upstairs/downstairs makes geometry move with this speed
        const float rotateSpeed = 150.0f; // left mouse on button rotate left/rotate right makes geometry rotate around the rotation pivot axis with this speed
        const float zoomSpeed = 50.0f; // zoom with this speed when keyboard hotkey is pressed        
        const float zoomSpeedMouseWheel = 2.0f; // mouse wheel inside main area of the automap window will zoom with this speed
        const float dragSpeed = 0.00345f; //= 0.002f; // hold left mouse button down and move mouse to move geometry with this speed
        const float dragRotateSpeed = 5.0f; // hold right mouse button down and move left/right to rotate geometry with this speed        
        //const float dragZoomSpeed = 0.007f; // hold right mouse button down and move up/down to zoom in/out

        const float cameraHeight = 90.0f; // initial camera height

        const float maxZoom = 25.0f; // the minimum external automap camera height
        const float minZoom = 250.0f; // the maximum external automap camera height

        const float locationSizeBasedStartZoomMultiplier = 10.0f; // the zoom multiplier based on location size used as starting zoom

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
        readonly HotkeySequence HotkeySequence_FocusPlayerPosition = new HotkeySequence(KeyCode.Tab, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_ResetView = new HotkeySequence(KeyCode.Backspace, HotkeySequence.KeyModifiers.None);      
        readonly HotkeySequence HotkeySequence_SwitchToNextExteriorAutomapViewMode = new HotkeySequence(KeyCode.Return, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_SwitchToExteriorAutomapViewModeOriginal = new HotkeySequence(KeyCode.F2, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_SwitchToExteriorAutomapViewModeExtra = new HotkeySequence(KeyCode.F3, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_SwitchToExteriorAutomapViewModeAll = new HotkeySequence(KeyCode.F4, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_SwitchToExteriorAutomapBackgroundOriginal = new HotkeySequence(KeyCode.F5, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_SwitchToExteriorAutomapBackgroundAlternative1 = new HotkeySequence(KeyCode.F6, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_SwitchToExteriorAutomapBackgroundAlternative2 = new HotkeySequence(KeyCode.F7, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_SwitchToExteriorAutomapBackgroundAlternative3 = new HotkeySequence(KeyCode.F8, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_MoveLeft = new HotkeySequence(KeyCode.LeftArrow, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_MoveRight = new HotkeySequence(KeyCode.RightArrow, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_MoveForward = new HotkeySequence(KeyCode.UpArrow, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_MoveBackward = new HotkeySequence(KeyCode.DownArrow, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_MoveToWestLocationBorder = new HotkeySequence(KeyCode.LeftArrow, HotkeySequence.KeyModifiers.LeftShift | HotkeySequence.KeyModifiers.RightShift);
        readonly HotkeySequence HotkeySequence_MoveToEastLocationBorder = new HotkeySequence(KeyCode.RightArrow, HotkeySequence.KeyModifiers.LeftShift | HotkeySequence.KeyModifiers.RightShift);
        readonly HotkeySequence HotkeySequence_MoveToNorthLocationBorder = new HotkeySequence(KeyCode.UpArrow, HotkeySequence.KeyModifiers.LeftShift | HotkeySequence.KeyModifiers.RightShift);
        readonly HotkeySequence HotkeySequence_MoveToSouthLocationBorder = new HotkeySequence(KeyCode.DownArrow, HotkeySequence.KeyModifiers.LeftShift | HotkeySequence.KeyModifiers.RightShift);
        readonly HotkeySequence HotkeySequence_RotateLeft = new HotkeySequence(KeyCode.LeftArrow, HotkeySequence.KeyModifiers.LeftControl | HotkeySequence.KeyModifiers.RightControl);
        readonly HotkeySequence HotkeySequence_RotateRight = new HotkeySequence(KeyCode.RightArrow, HotkeySequence.KeyModifiers.LeftControl | HotkeySequence.KeyModifiers.RightControl);
        readonly HotkeySequence HotkeySequence_RotateAroundPlayerPosLeft = new HotkeySequence(KeyCode.LeftArrow, HotkeySequence.KeyModifiers.LeftAlt | HotkeySequence.KeyModifiers.RightAlt);
        readonly HotkeySequence HotkeySequence_RotateAroundPlayerPosRight = new HotkeySequence(KeyCode.RightArrow, HotkeySequence.KeyModifiers.LeftAlt | HotkeySequence.KeyModifiers.RightAlt);
        readonly HotkeySequence HotkeySequence_Upstairs = new HotkeySequence(KeyCode.PageUp, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_Downstairs = new HotkeySequence(KeyCode.PageDown, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_ZoomIn = new HotkeySequence(KeyCode.KeypadPlus, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_ZoomOut = new HotkeySequence(KeyCode.KeypadMinus, HotkeySequence.KeyModifiers.None);
        readonly HotkeySequence HotkeySequence_MaxZoom1 = new HotkeySequence(KeyCode.PageUp, HotkeySequence.KeyModifiers.LeftControl | HotkeySequence.KeyModifiers.RightControl);
        readonly HotkeySequence HotkeySequence_MinZoom1 = new HotkeySequence(KeyCode.PageDown, HotkeySequence.KeyModifiers.LeftControl | HotkeySequence.KeyModifiers.RightControl);
        readonly HotkeySequence HotkeySequence_MinZoom2 = new HotkeySequence(KeyCode.KeypadPlus, HotkeySequence.KeyModifiers.LeftControl | HotkeySequence.KeyModifiers.RightControl);
        readonly HotkeySequence HotkeySequence_MaxZoom2 = new HotkeySequence(KeyCode.KeypadMinus, HotkeySequence.KeyModifiers.LeftControl | HotkeySequence.KeyModifiers.RightControl);

        const string nativeImgName = "AMAP00I0.IMG";
        const string nativeImgNameGrid3D = "AMAP01I0.IMG";

        DaggerfallExteriorAutomap daggerfallExteriorAutomap = null; // used to communicate with DaggerfallExteriorAutomap class

        GameObject gameobjectExteriorAutomap = null; // used to hold reference to instance of GameObject "ExteriorAutomap" (which has script Game/DaggerfallExteriorAutomap.cs attached)

        Camera cameraExteriorAutomap = null; // camera for automap camera

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

        Color[] backgroundOriginal; // texture with orignial background will be stored in here
        Color[] backgroundAlternative1; // texture with first alternative background will be stored in here
        Color[] backgroundAlternative2; // texture with second alternative background will be stored in here
        Color[] backgroundAlternative3; // texture with third alternative background will be stored in here

        HUDCompass compass = null;

        RenderTexture renderTextureExteriorAutomap = null; // render texture in which exterior automap camera will render into
        Texture2D textureExteriorAutomap = null; // render texture will converted to this texture so that it can be drawn in panelRenderExteriorAutomap

        int renderTextureExteriorAutomapDepth = 16;
        int oldRenderTextureExteriorAutomapWidth; // used to store previous width of exterior automap render texture to react to changes to NativePanel's size and react accordingly by setting texture up with new widht and height again
        int oldRenderTextureExteriorAutomapHeight; // used to store previous height of exterior automap render texture to react to changes to NativePanel's size and react accordingly by setting texture up with new widht and height again
		
        bool isSetup = false;

        public DaggerfallExteriorAutomapWindow(IUserInterfaceManager uiManager)
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

            initGlobalResources(); // initialize gameobjectAutomap, daggerfallExteriorAutomap and layerAutomap

            // set transform of gameobjectExteriorAutomap to zero so that all camera dependent actions in its future camera child work correctly
            gameobjectExteriorAutomap.transform.position = Vector3.zero;
            gameobjectExteriorAutomap.transform.rotation = Quaternion.Euler(Vector3.zero);

            // Load native texture
            imgFile = new ImgFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, nativeImgName), FileUsage.UseMemory, false);
            imgFile.LoadPalette(Path.Combine(DaggerfallUnity.Instance.Arena2Path, imgFile.PaletteName));
            bitmap = imgFile.GetDFBitmap();
            nativeTexture = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.ARGB32, false);
            nativeTexture.SetPixels32(imgFile.GetColor32(bitmap, 0));
            nativeTexture.Apply(false, false); // make readable
            nativeTexture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            if (!nativeTexture)
                throw new Exception("DaggerfallExteriorAutomapWindow: Could not load native texture (AMAP00I0.IMG).");
            
            // Load alternative Grid Icon (3D View Grid graphics)
            imgFile = new ImgFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, nativeImgNameGrid3D), FileUsage.UseMemory, false);
            imgFile.LoadPalette(Path.Combine(DaggerfallUnity.Instance.Arena2Path, imgFile.PaletteName));
            bitmap = imgFile.GetDFBitmap();
            Texture2D nativeTextureGrid3D = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.ARGB32, false);
            nativeTextureGrid3D.SetPixels32(imgFile.GetColor32(bitmap, 0));
            nativeTextureGrid3D.Apply(false, false); // make readable
            nativeTextureGrid3D.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            if (!nativeTextureGrid3D)
                throw new Exception("DaggerfallExteriorAutomapWindow: Could not load native texture (AMAP01I0.IMG).");

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
                backgroundAlternative3[i].r = 0.7f;
                backgroundAlternative3[i].g = 0.52f;
                backgroundAlternative3[i].b = 0.18f;
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
            gridButton.ToolTip = defaultToolTip;
            gridButton.ToolTipText = "left click: switch to next view mode (hotkey: enter key)\ravailable view modes are:\r- original (hotkey F2)\r- extra: includes extra buildings (hotkey F3)\r- all: includes extra buildings, ground flats (hotkey F4)\rswitch background texture with F5-F8";
            gridButton.ToolTip.ToolTipDelay = toolTipDelay;

            // forward button
            forwardButton = DaggerfallUI.AddButton(new Rect(105, 171, 21, 19), NativePanel);
            forwardButton.OnMouseDown += ForwardButton_OnMouseDown;
            forwardButton.OnMouseUp += ForwardButton_OnMouseUp;
            forwardButton.OnRightMouseDown += ForwardButton_OnRightMouseDown;
            forwardButton.OnRightMouseUp += ForwardButton_OnRightMouseUp;
            forwardButton.ToolTip = defaultToolTip;
            forwardButton.ToolTipText = "left click: move up (hotkey: up arrow)\rright click: move to north location border (hotkey: shift+up arrow)";
            forwardButton.ToolTip.ToolTipDelay = toolTipDelay;

            // backward button
            backwardButton = DaggerfallUI.AddButton(new Rect(126, 171, 21, 19), NativePanel);
            backwardButton.OnMouseDown += BackwardButton_OnMouseDown;
            backwardButton.OnMouseUp += BackwardButton_OnMouseUp;
            backwardButton.OnRightMouseDown += BackwardButton_OnRightMouseDown;
            backwardButton.OnRightMouseUp += BackwardButton_OnRightMouseUp;
            backwardButton.ToolTip = defaultToolTip;
            backwardButton.ToolTipText = "left click: move down (hotkey: down arrow)\rright click: move to south location border (hotkey: shift+down arrow)";
            backwardButton.ToolTip.ToolTipDelay = toolTipDelay;

            // left button
            leftButton = DaggerfallUI.AddButton(new Rect(149, 171, 21, 19), NativePanel);
            leftButton.OnMouseDown += LeftButton_OnMouseDown;
            leftButton.OnMouseUp += LeftButton_OnMouseUp;
            leftButton.OnRightMouseDown += LeftButton_OnRightMouseDown;
            leftButton.OnRightMouseUp += LeftButton_OnRightMouseUp;
            leftButton.ToolTip = defaultToolTip;
            leftButton.ToolTipText = "left click: move to the left (hotkey: left arrow)\rright click: move to west location border (hotkey: shift+left arrow)";
            leftButton.ToolTip.ToolTipDelay = toolTipDelay;

            // right button
            rightButton = DaggerfallUI.AddButton(new Rect(170, 171, 21, 19), NativePanel);
            rightButton.OnMouseDown += RightButton_OnMouseDown;
            rightButton.OnMouseUp += RightButton_OnMouseUp;
            rightButton.OnRightMouseDown += RightButton_OnRightMouseDown;
            rightButton.OnRightMouseUp += RightButton_OnRightMouseUp;
            rightButton.ToolTip = defaultToolTip;
            rightButton.ToolTipText = "left click: move to the right (hotkey: right arrow)\rright click: move to east location border (hotkey: shift+right arrow)";
            rightButton.ToolTip.ToolTipDelay = toolTipDelay;

            // rotate left button
            rotateLeftButton = DaggerfallUI.AddButton(new Rect(193, 171, 21, 19), NativePanel);
            rotateLeftButton.OnMouseDown += RotateLeftButton_OnMouseDown;
            rotateLeftButton.OnMouseUp += RotateLeftButton_OnMouseUp;
            rotateLeftButton.OnRightMouseDown += RotateLeftButton_OnRightMouseDown;
            rotateLeftButton.OnRightMouseUp += RotateLeftButton_OnRightMouseUp;
            rotateLeftButton.ToolTip = defaultToolTip;
            rotateLeftButton.ToolTipText = "left click: rotate map to the left (hotkey: control+right arrow)\rright click: rotate map around the player position\rto the left  (hotkey: alt+right arrow)";
            rotateLeftButton.ToolTip.ToolTipDelay = toolTipDelay;

            // rotate right button
            rotateRightButton = DaggerfallUI.AddButton(new Rect(214, 171, 21, 19), NativePanel);
            rotateRightButton.OnMouseDown += RotateRightButton_OnMouseDown;
            rotateRightButton.OnMouseUp += RotateRightButton_OnMouseUp;
            rotateRightButton.OnRightMouseDown += RotateRightButton_OnRightMouseDown;
            rotateRightButton.OnRightMouseUp += RotateRightButton_OnRightMouseUp;
            rotateRightButton.ToolTip = defaultToolTip;
            rotateRightButton.ToolTipText = "left click: rotate map to the right (hotkey: control+right arrow)\rright click: rotate map around the player position\rto the right (hotkey: alt+right arrow)";
            rotateRightButton.ToolTip.ToolTipDelay = toolTipDelay;

            // upstairs button
            upstairsButton = DaggerfallUI.AddButton(new Rect(237, 171, 21, 19), NativePanel);
            upstairsButton.OnMouseDown += UpstairsButton_OnMouseDown;
            upstairsButton.OnMouseUp += UpstairsButton_OnMouseUp;
            upstairsButton.OnRightMouseDown += UpstairsButton_OnRightMouseDown;
            upstairsButton.OnRightMouseUp += UpstairsButton_OnRightMouseUp;
            upstairsButton.ToolTip = defaultToolTip;
            upstairsButton.ToolTipText = "left click: zoom in (hotkey: page up)\rright click: apply maximum zoom";
            upstairsButton.ToolTip.ToolTipDelay = toolTipDelay;

            // downstairs button
            downstairsButton = DaggerfallUI.AddButton(new Rect(258, 171, 21, 19), NativePanel);
            downstairsButton.OnMouseDown += DownstairsButton_OnMouseDown;
            downstairsButton.OnMouseUp += DownstairsButton_OnMouseUp;
            downstairsButton.OnRightMouseDown += DownstairsButton_OnRightMouseDown;
            downstairsButton.OnRightMouseUp += DownstairsButton_OnRightMouseUp;
            downstairsButton.ToolTip = defaultToolTip;
            downstairsButton.ToolTipText = "left click: zoom out (hotkey: page down\rright click: apply minimum zoom)";
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
            dummyPanelCompass.ToolTipText = "left click: focus player position (hotkey: tab)\rright click: reset view (hotkey: backspace)";
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
        /// called when automap window is pushed - resets automap settings to default settings and signals DaggerfallExteriorAutomap class
        /// </summary>
        public override void OnPush()
        {
            initGlobalResources(); // initialize gameobjectAutomap, daggerfallExteriorAutomap and layerAutomap

            if (!isSetup) // if Setup() has not run, run it now
                Setup();

            daggerfallExteriorAutomap.IsOpenAutomap = true; // signal DaggerfallExteriorAutomap script that automap is open and it should do its stuff in its Update() function            

            daggerfallExteriorAutomap.updateAutomapStateOnWindowPush(); // signal DaggerfallExteriorAutomap script that automap window was closed and that it should update its state (updates player marker arrow)

            // get automap camera
            cameraExteriorAutomap = daggerfallExteriorAutomap.CameraExteriorAutomap;

            // create automap render texture and Texture2D used in conjuction with automap camera to render automap level geometry and display it in panel
            Rect positionPanelRenderAutomap = dummyPanelAutomap.Rectangle;
            createExteriorAutomapTextures((int)positionPanelRenderAutomap.width, (int)positionPanelRenderAutomap.height);

            if (compass != null)
            {
                compass.CompassCamera = cameraExteriorAutomap;
            }

            if (daggerfallExteriorAutomap.ResetAutomapSettingsSignalForExternalScript == true) // signaled to reset automap settings
            {
                // reset values to default whenever player enters building or dungeon
                resetCameraPosition();

                daggerfallExteriorAutomap.ResetAutomapSettingsSignalForExternalScript = false; // indicate the settings were reset
            }
            else
            {
                resetCameraPosition();
            }

            // and update the automap view
            updateAutomapView();
        }

        /// <summary>
        /// called when automap window is popped - destroys resources and signals DaggerfallExteriorAutomap class
        /// </summary>
        public override void OnPop()
        {
            daggerfallExteriorAutomap.IsOpenAutomap = false; // signal DaggerfallExteriorAutomap script that automap was closed

            // destroy the other gameobjects as well so they don't use system resources

            cameraExteriorAutomap.targetTexture = null;

            if (renderTextureExteriorAutomap != null)
            {
                UnityEngine.Object.Destroy(renderTextureExteriorAutomap);
            }

            if (textureExteriorAutomap != null)
            {
                UnityEngine.Object.Destroy(textureExteriorAutomap);
            }

            daggerfallExteriorAutomap.updateAutomapStateOnWindowPop(); // signal DaggerfallExteriorAutomap script that automap window was closed
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
            
            // check hotkeys and assign actions
            if (Input.GetKeyDown(HotkeySequence_CloseMap.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_CloseMap.modifiers))
            {                
                CloseWindow();
                Input.ResetInputAxes(); // prevents automap window to reopen immediately after closing
            }
            if (Input.GetKeyDown(HotkeySequence_FocusPlayerPosition.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_FocusPlayerPosition.modifiers))
            {
                ActionFocusPlayerPosition();
            }
            if (Input.GetKeyDown(HotkeySequence_ResetView.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_ResetView.modifiers))
            {
                ActionResetView();
            }

            if (Input.GetKeyDown(HotkeySequence_SwitchToNextExteriorAutomapViewMode.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToNextExteriorAutomapViewMode.modifiers))
            {
                ActionSwitchToNextExteriorAutomapViewMode();
            }
            if (Input.GetKeyDown(HotkeySequence_SwitchToExteriorAutomapViewModeOriginal.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToExteriorAutomapViewModeOriginal.modifiers))
            {
                ActionSwitchToExteriorAutomapViewModeOriginal();
            }
            if (Input.GetKeyDown(HotkeySequence_SwitchToExteriorAutomapViewModeExtra.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToExteriorAutomapViewModeExtra.modifiers))
            {
                ActionSwitchToExteriorAutomapViewModeExtra();
            }
            if (Input.GetKeyDown(HotkeySequence_SwitchToExteriorAutomapViewModeAll.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToExteriorAutomapViewModeAll.modifiers))
            {
                ActionSwitchToExteriorAutomapViewModeAll();
            }

            if (Input.GetKeyDown(HotkeySequence_SwitchToExteriorAutomapBackgroundOriginal.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToExteriorAutomapBackgroundOriginal.modifiers))
            {
                ActionSwitchToExteriorAutomapBackgroundOriginal();
            }
            if (Input.GetKeyDown(HotkeySequence_SwitchToExteriorAutomapBackgroundAlternative1.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToExteriorAutomapBackgroundAlternative1.modifiers))
            {
                ActionSwitchToExteriorAutomapBackgroundAlternative1();
            }
            if (Input.GetKeyDown(HotkeySequence_SwitchToExteriorAutomapBackgroundAlternative2.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToExteriorAutomapBackgroundAlternative2.modifiers))
            {
                ActionSwitchToExteriorAutomapBackgroundAlternative2();
            }
            if (Input.GetKeyDown(HotkeySequence_SwitchToExteriorAutomapBackgroundAlternative3.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_SwitchToExteriorAutomapBackgroundAlternative3.modifiers))
            {
                ActionSwitchToExteriorAutomapBackgroundAlternative3();
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

            if (Input.GetKey(HotkeySequence_MoveToNorthLocationBorder.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MoveToNorthLocationBorder.modifiers))
            {
                ActionMoveToNorthLocationBorder();
            }
            if (Input.GetKey(HotkeySequence_MoveToSouthLocationBorder.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MoveToSouthLocationBorder.modifiers))
            {
                ActionMoveToSouthLocationBorder();
            }
            if (Input.GetKey(HotkeySequence_MoveToWestLocationBorder.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MoveToWestLocationBorder.modifiers))
            {
                ActionMoveToWestLocationBorder();
            }
            if (Input.GetKey(HotkeySequence_MoveToEastLocationBorder.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MoveToEastLocationBorder.modifiers))
            {
                ActionMoveToEastLocationBorder();
            }        

            if (Input.GetKey(HotkeySequence_RotateLeft.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_RotateLeft.modifiers))
            {
                ActionRotateLeft();
            }
            if (Input.GetKey(HotkeySequence_RotateRight.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_RotateRight.modifiers))
            {
                ActionRotateRight();
            }
            if (Input.GetKey(HotkeySequence_RotateAroundPlayerPosLeft.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_RotateAroundPlayerPosLeft.modifiers))
            {
                ActionRotateAroundPlayerPosLeft();
            }
            if (Input.GetKey(HotkeySequence_RotateAroundPlayerPosRight.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_RotateAroundPlayerPosRight.modifiers))
            {
                ActionRotateAroundPlayerPosRight();
            }

            if (Input.GetKey(HotkeySequence_Upstairs.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_Upstairs.modifiers))
            {
                ActionMoveUpstairs();
            }
            if (Input.GetKey(HotkeySequence_Downstairs.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_Downstairs.modifiers))
            {
                ActionMoveDownstairs();
            }
            if (Input.GetKey(HotkeySequence_ZoomIn.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_ZoomIn.modifiers))
            {             
                ActionZoom(-zoomSpeed * Time.unscaledDeltaTime);
            }
            if (Input.GetKey(HotkeySequence_ZoomOut.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_ZoomOut.modifiers))
            {                
                ActionZoom(zoomSpeed * Time.unscaledDeltaTime);
            }

            if (Input.GetKey(HotkeySequence_MaxZoom1.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MaxZoom1.modifiers))
            {
                ActionApplyMaxZoom();
            }
            if (Input.GetKey(HotkeySequence_MinZoom1.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MinZoom1.modifiers))
            {
                ActionApplyMinZoom();
            }
            if (Input.GetKey(HotkeySequence_MinZoom2.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MinZoom2.modifiers))
            {
                ActionApplyMinZoom();
            }
            if (Input.GetKey(HotkeySequence_MaxZoom2.keyCode) && HotkeySequence.checkSetModifiers(keyModifiers, HotkeySequence_MaxZoom2.modifiers))
            {
                ActionApplyMaxZoom();
            }            

            // check mouse input and assign actions
            if (leftMouseDownOnPanelAutomap)
            {
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

                float dragSpeedCompensated;
                dragSpeedCompensated = dragSpeed * cameraExteriorAutomap.orthographicSize; // * cameraExteriorAutomap.transform.position.y;
                Vector2 bias = mousePosition - oldMousePosition;
                Vector3 translation = -cameraExteriorAutomap.transform.right * dragSpeedCompensated * bias.x + cameraExteriorAutomap.transform.up * dragSpeedCompensated * bias.y;
                cameraExteriorAutomap.transform.position += translation;
                updateAutomapView();
                oldMousePosition = mousePosition;
            }

            if (rightMouseDownOnPanelAutomap)
            {
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

                Vector2 bias = mousePosition - oldMousePosition;

                ActionRotate(dragRotateSpeed * bias.x);
                
                //float zoomSpeedCompensated = dragZoomSpeed * daggerfallExteriorAutomap.LayoutMultiplier;
                //ActionZoomOut(zoomSpeedCompensated * bias.y);

                updateAutomapView();
                oldMousePosition = mousePosition;
            }

            if (leftMouseDownOnForwardButton)
            {
                ActionMoveForward();
            }

            if (rightMouseDownOnForwardButton)
            {
                ActionMoveToNorthLocationBorder();
            }

            if (leftMouseDownOnBackwardButton)
            {
                ActionMoveBackward();
            }

            if (rightMouseDownOnBackwardButton)
            {
                ActionMoveToSouthLocationBorder();
            }

            if (leftMouseDownOnLeftButton)
            {
                ActionMoveLeft();
            }

            if (rightMouseDownOnLeftButton)
            {
                ActionMoveToWestLocationBorder();
            }

            if (leftMouseDownOnRightButton)
            {
                ActionMoveRight();
            }

            if (rightMouseDownOnRightButton)
            {
                ActionMoveToEastLocationBorder();
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
                ActionRotateAroundPlayerPosLeft();
            }

            if (rightMouseDownOnRotateRightButton)
            {
                ActionRotateAroundPlayerPosRight();
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
                ActionApplyMaxZoom();
            }

            if (rightMouseDownOnDownstairsButton)
            {
                ActionApplyMinZoom();
            }
        }        

        #region Private Methods

        /// <summary>
        /// tests for availability and initializes class resources like GameObject for automap, DaggerfallExteriorAutomap class and layerAutomap
        /// </summary>
        private void initGlobalResources()
        {
            if (!gameobjectExteriorAutomap)
            {
                gameobjectExteriorAutomap = GameObject.Find("Automap/ExteriorAutomap");
                if (gameobjectExteriorAutomap == null)
                {
                    DaggerfallUnity.LogMessage("GameObject \"Automap/ExteriorAutomap\" missing! Create a GameObject called \"Automap\" in root of hierarchy and add a GameObject \"ExteriorAutomap\" to it, to this add script Game/DaggerfallExteriorAutomap!\"", true);                
                }
            }

            if (!daggerfallExteriorAutomap)
            {
                daggerfallExteriorAutomap = gameobjectExteriorAutomap.GetComponent<DaggerfallExteriorAutomap>();
                if (daggerfallExteriorAutomap == null)
                {
                    DaggerfallUnity.LogMessage("Script DafferfallAutomap is missing in GameObject \"ExteriorAutomap\"! GameObject \"ExteriorAutomap\" must have script Game/DaggerfallExteriorAutomap attached!\"", true);
                }
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
                createExteriorAutomapTextures((int)positionPanelRenderAutomap.x, (int)positionPanelRenderAutomap.y);
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
        private void createExteriorAutomapTextures(int width, int height)
        {
            if ((!cameraExteriorAutomap) || (!renderTextureExteriorAutomap) || (oldRenderTextureExteriorAutomapWidth != width) || (oldRenderTextureExteriorAutomapHeight != height))
            {
                cameraExteriorAutomap.targetTexture = null;
                if (renderTextureExteriorAutomap)
                    UnityEngine.Object.Destroy(renderTextureExteriorAutomap);
                if (textureExteriorAutomap)
                    UnityEngine.Object.Destroy(textureExteriorAutomap);

                renderTextureExteriorAutomap = new RenderTexture(width, height, renderTextureExteriorAutomapDepth);
                cameraExteriorAutomap.targetTexture = renderTextureExteriorAutomap;

                textureExteriorAutomap = new Texture2D(renderTextureExteriorAutomap.width, renderTextureExteriorAutomap.height, TextureFormat.ARGB32, false);

                oldRenderTextureExteriorAutomapWidth = width;
                oldRenderTextureExteriorAutomapHeight = height;
            }
        }

        /// <summary>
        /// resets the automap camera position for active view mode
        /// </summary>
        private void resetCameraPosition()
        {
            // then set camera transform according to grid-button (view mode) setting
            resetCameraTransform();
        }

        /// <summary>
        /// resets the camera transform of the 2D view mode
        /// </summary>
        private void resetCameraTransform()
        {
            cameraExteriorAutomap.orthographicSize = locationSizeBasedStartZoomMultiplier * Math.Max(daggerfallExteriorAutomap.LocationWidth, daggerfallExteriorAutomap.LocationHeight) * daggerfallExteriorAutomap.LayoutMultiplier;
            cameraExteriorAutomap.orthographicSize = Math.Min(minZoom, Math.Max(maxZoom * daggerfallExteriorAutomap.LayoutMultiplier, cameraExteriorAutomap.orthographicSize));
            cameraExteriorAutomap.transform.position = daggerfallExteriorAutomap.GameobjectPlayerMarkerArrow.transform.position + new Vector3(0.0f, 10.0f, 0.0f); //Vector3.zero + Vector3.up * cameraHeight;
            cameraExteriorAutomap.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            //cameraExteriorAutomap.transform.LookAt(Vector3.zero);            
        }


        /// <summary>
        /// updates the automap view - signals DaggerfallExteriorAutomap class to update and renders the automap level geometry afterwards into the automap render panel
        /// </summary>
        private void updateAutomapView()
        {
            daggerfallExteriorAutomap.forceUpdate();

            if ((!cameraExteriorAutomap) || (!renderTextureExteriorAutomap))
                return;

            cameraExteriorAutomap.Render();

            RenderTexture.active = renderTextureExteriorAutomap;
            textureExteriorAutomap.ReadPixels(new Rect(0, 0, renderTextureExteriorAutomap.width, renderTextureExteriorAutomap.height), 0, 0);
            textureExteriorAutomap.Apply(false);
            RenderTexture.active = null;

            panelRenderAutomap.BackgroundTexture = textureExteriorAutomap;
        }


        #endregion

        #region Actions (Callbacks for Mouse Events and Hotkeys)

        /// <summary>
        /// action for move forward
        /// </summary>
        private void ActionMoveForward()
        {
            Vector3 translation;
            translation = cameraExteriorAutomap.transform.up * scrollUpDownSpeed * Time.unscaledDeltaTime * daggerfallExteriorAutomap.LayoutMultiplier;
            translation.y = 0.0f; // comment this out for movement along camera optical axis
            cameraExteriorAutomap.transform.position += translation;
            updateAutomapView();
        }

        /// <summary>
        /// action for move backward
        /// </summary>
        private void ActionMoveBackward()
        {
            Vector3 translation;
            translation = -cameraExteriorAutomap.transform.up * scrollUpDownSpeed * Time.unscaledDeltaTime * daggerfallExteriorAutomap.LayoutMultiplier;
            translation.y = 0.0f; // comment this out for movement along camera optical axis
            cameraExteriorAutomap.transform.position += translation;
            updateAutomapView();
        }

        /// <summary>
        /// action for move left
        /// </summary>
        private void ActionMoveLeft()
        {
            Vector3 translation = -cameraExteriorAutomap.transform.right * scrollLeftRightSpeed * Time.unscaledDeltaTime * daggerfallExteriorAutomap.LayoutMultiplier;
            translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector
            cameraExteriorAutomap.transform.position += translation;
            updateAutomapView();
        }

        /// <summary>
        /// action for move right
        /// </summary>
        private void ActionMoveRight()
        {
            Vector3 translation = cameraExteriorAutomap.transform.right * scrollLeftRightSpeed * Time.unscaledDeltaTime * daggerfallExteriorAutomap.LayoutMultiplier;
            translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector
            cameraExteriorAutomap.transform.position += translation;
            updateAutomapView();
        }

        /// <summary>
        /// action for rotating map to the left
        /// </summary>
        private void ActionRotateLeft()
        {
            ActionRotate(+rotateSpeed);
        }

        /// <summary>
        /// action for rotating map to the right
        /// </summary>
        private void ActionRotateRight()
        {
            ActionRotate(-rotateSpeed);
        }

        private void ActionRotate(float rotationAmount)
        {
            cameraExteriorAutomap.transform.RotateAround(cameraExteriorAutomap.transform.position, -Vector3.up, -rotationAmount * Time.unscaledDeltaTime);
            updateAutomapView();
        }

        /// <summary>
        /// action for rotating around player pos to the left
        /// </summary>
        private void ActionRotateAroundPlayerPosLeft()
        {
            ActionRotateAroundPlayerPos(+rotateSpeed);
        }

        /// <summary>
        /// action for rotating around player pos to the right
        /// </summary>
        private void ActionRotateAroundPlayerPosRight()
        {
            ActionRotateAroundPlayerPos(-rotateSpeed);
        }

        private void ActionRotateAroundPlayerPos(float rotationAmount)
        {
            cameraExteriorAutomap.transform.RotateAround(daggerfallExteriorAutomap.GameobjectPlayerMarkerArrow.transform.position, -Vector3.up, -rotationAmount * Time.unscaledDeltaTime);
            updateAutomapView();
        }

        /// <summary>
        /// action for moving upstairs
        /// </summary>
        private void ActionMoveUpstairs()
        {
            //cameraExteriorAutomap.transform.position += Vector3.up * moveUpstairsDownstairsSpeed * Time.unscaledDeltaTime;
            ActionZoom(-zoomSpeed * Time.unscaledDeltaTime);
            updateAutomapView();
        }

        /// <summary>
        /// action for moving downstairs
        /// </summary>
        private void ActionMoveDownstairs()
        {
            //cameraExteriorAutomap.transform.position += Vector3.down * moveUpstairsDownstairsSpeed * Time.unscaledDeltaTime;
            ActionZoom(zoomSpeed * Time.unscaledDeltaTime);
            updateAutomapView();
        }

        /// <summary>
        /// action for zooming in/out
        /// </summary>
        private void ActionZoom(float speed)
        {
            float zoomSpeedCompensated = speed * daggerfallExteriorAutomap.LayoutMultiplier; // * cameraExteriorAutomap.transform.position.y * daggerfallExteriorAutomap.LayoutMultiplier;
            //Vector3 translation = cameraExteriorAutomap.transform.forward * zoomSpeedCompensated;
            //cameraExteriorAutomap.transform.position += translation;
            //cameraExteriorAutomap.transform.position = new Vector3(cameraExteriorAutomap.transform.position.x, Math.Max(maxZoom, cameraExteriorAutomap.transform.position.y), cameraExteriorAutomap.transform.position.z);
            cameraExteriorAutomap.orthographicSize += zoomSpeedCompensated;
            cameraExteriorAutomap.orthographicSize = Math.Min(minZoom * daggerfallExteriorAutomap.LayoutMultiplier, (Math.Max(maxZoom * daggerfallExteriorAutomap.LayoutMultiplier, cameraExteriorAutomap.orthographicSize)));
            updateAutomapView();
        }

        /// <summary>
        /// action for applying minimum zoom
        /// </summary>
        private void ActionApplyMinZoom()
        {
            cameraExteriorAutomap.orthographicSize = minZoom;
            updateAutomapView();
        }

        /// <summary>
        /// action for applying maximum zoom
        /// </summary>
        public void ActionApplyMaxZoom()
        {
            cameraExteriorAutomap.orthographicSize = maxZoom;
            updateAutomapView();
        }

        /// <summary>
        /// action for moving camera to the left border of the current location
        /// </summary>
        private void ActionMoveToWestLocationBorder()
        {
            Vector3 pos = daggerfallExteriorAutomap.getLocationBorderPos(DaggerfallExteriorAutomap.LocationBorder.Left);
            cameraExteriorAutomap.transform.position = new Vector3(pos.x, cameraExteriorAutomap.transform.position.y, cameraExteriorAutomap.transform.position.z);
            updateAutomapView();
        }

        /// <summary>
        /// action for moving camera to the right border of the current location
        /// </summary>
        private void ActionMoveToEastLocationBorder()
        {
            Vector3 pos = daggerfallExteriorAutomap.getLocationBorderPos(DaggerfallExteriorAutomap.LocationBorder.Right);
            cameraExteriorAutomap.transform.position = new Vector3(pos.x, cameraExteriorAutomap.transform.position.y, cameraExteriorAutomap.transform.position.z);
            updateAutomapView();
        }

        /// <summary>
        /// action for moving camera to the top border of the current location
        /// </summary>
        private void ActionMoveToNorthLocationBorder()
        {
            Vector3 pos = daggerfallExteriorAutomap.getLocationBorderPos(DaggerfallExteriorAutomap.LocationBorder.Top);
            cameraExteriorAutomap.transform.position = new Vector3(cameraExteriorAutomap.transform.position.x, cameraExteriorAutomap.transform.position.y, pos.z);
            updateAutomapView();
        }

        /// <summary>
        /// action for moving camera to the bottom border of the current location
        /// </summary>
        private void ActionMoveToSouthLocationBorder()
        {
            Vector3 pos = daggerfallExteriorAutomap.getLocationBorderPos(DaggerfallExteriorAutomap.LocationBorder.Bottom);
            cameraExteriorAutomap.transform.position = new Vector3(cameraExteriorAutomap.transform.position.x, cameraExteriorAutomap.transform.position.y, pos.z);
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to next exterior automap view mode
        /// </summary>
        private void ActionSwitchToNextExteriorAutomapViewMode()
        {
            daggerfallExteriorAutomap.switchToNextExteriorAutomapViewMode();
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to exterior automap view mode "original"
        /// </summary>
        private void ActionSwitchToExteriorAutomapViewModeOriginal()
        {
            daggerfallExteriorAutomap.switchToExteriorAutomapViewModeOriginal();
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to exterior automap view mode "extra"
        /// </summary>
        private void ActionSwitchToExteriorAutomapViewModeExtra()
        {
            daggerfallExteriorAutomap.switchToExteriorAutomapViewModeExtra();
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to exterior automap view mode "all"
        /// </summary>
        private void ActionSwitchToExteriorAutomapViewModeAll()
        {
            daggerfallExteriorAutomap.switchToExteriorAutomapViewModeAll();
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to automap background to original background
        /// </summary>
        private void ActionSwitchToExteriorAutomapBackgroundOriginal()
        {
            nativeTexture.SetPixels(0, 29, nativeTexture.width, nativeTexture.height - 29, backgroundOriginal);
            nativeTexture.Apply(false);
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to automap background to background alternative 1
        /// </summary>
        private void ActionSwitchToExteriorAutomapBackgroundAlternative1()
        {
            nativeTexture.SetPixels(0, 29, nativeTexture.width, nativeTexture.height - 29, backgroundAlternative1);
            nativeTexture.Apply(false);
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to automap background to background alternative 2
        /// </summary>
        private void ActionSwitchToExteriorAutomapBackgroundAlternative2()
        {
            nativeTexture.SetPixels(0, 29, nativeTexture.width, nativeTexture.height - 29, backgroundAlternative2);
            nativeTexture.Apply(false);
            updateAutomapView();
        }

        /// <summary>
        /// action for switching to automap background to background alternative 3
        /// </summary>
        private void ActionSwitchToExteriorAutomapBackgroundAlternative3()
        {
            nativeTexture.SetPixels(0, 29, nativeTexture.width, nativeTexture.height - 29, backgroundAlternative3);
            nativeTexture.Apply(false);
            updateAutomapView();
        }

        /// <summary>
        /// action for focusing player position
        /// </summary>
        private void ActionFocusPlayerPosition()
        {
            // focus player position (set camera to player position)
            cameraExteriorAutomap.transform.position = daggerfallExteriorAutomap.GameobjectPlayerMarkerArrow.transform.position + new Vector3(0.0f, 10.0f, 0.0f);
            updateAutomapView();
        }

        /// <summary>
        /// action for reset view
        /// </summary>
        private void ActionResetView()
        {
            // reset values to default
            resetCameraPosition();
            updateAutomapView();
        }

        #endregion

        #region Event Handlers

        private void PanelAutomap_OnMouseScrollUp()
        {
            ActionZoom(-zoomSpeedMouseWheel);
        }

        private void PanelAutomap_OnMouseScrollDown()
        {
            ActionZoom(zoomSpeedMouseWheel);
        }

        private void PanelAutomap_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            if (alreadyInMouseDown)
                return;

            leftMouseClickedOnPanelAutomap = true; // used for debug teleport mode clicks

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

            ActionSwitchToNextExteriorAutomapViewMode();
        }

        private void GridButton_OnRightMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode())
                return;
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

            ActionFocusPlayerPosition();
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