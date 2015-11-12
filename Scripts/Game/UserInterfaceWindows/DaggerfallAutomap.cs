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
    /// Implements indoor and dungeon automap window.
    /// </summary>
    public class DaggerfallAutomapWindow : DaggerfallPopupWindow
    {
        const float scrollLeftRightSpeed = 1.0f;
        const float scrollForwardBackwardSpeed = 1.0f;
        const float scrollUpDownSpeed = 1.0f;
        const float rotateSpeed = 10.0f;
        const float zoomSpeed = 0.1f; // suggested value range: 0.1f (fast) to 0.01f (slow)
        const float dragSpeed = 0.002f; // suggested value range: 0.01f (fast) to 0.001f (slow)

        const float cameraHeightViewFromTop = 30.0f;
        const float cameraHeightView3D = 8.0f;
        const float cameraBackwardDistance = 20.0f;        


        const string nativeImgName = "AMAP00I0.IMG";
        const string nativeImgNameGrid3D = "AMAP01I0.IMG";

        enum AutomapViewMode { View2D = 0, View3D = 1};
        AutomapViewMode automapViewMode = AutomapViewMode.View2D;

        Panel panelAutomap = null;
        Rect oldPositionPanelAutomap;
        Vector2 oldDragPosition;

        bool leftMouseDownOnPanelAutomap = false;
        bool leftMouseDownOnForwardButton = false;
        bool leftMouseDownOnBackwardButton = false;
        bool leftMouseDownOnLeftButton = false;
        bool leftMouseDownOnRightButton = false;
        bool leftMouseDownOnRotateLeftButton = false;
        bool leftMouseDownOnRotateRightButton = false;
        bool leftMouseDownOnUpstairsButton = false;
        bool leftMouseDownOnDownstairsButton = false;
        bool alreadyInMouseDown = false;
        bool inDragMode() { return leftMouseDownOnPanelAutomap; }

        Texture2D nativeTexture;
        Texture2D nativeTextureGrid2D;
        Texture2D nativeTextureGrid3D;

        Color[] pixelsGrid2D;
        Color[] pixelsGrid3D;

        HUDCompass compass;

        Camera cameraAutomap = null;
        RenderTexture renderTextureAutomap = null;
        Texture2D textureAutomap = null;


        int renderTextureAutomapDepth = 16;
        int oldRenderTextureAutomapWidth;
        int oldRenderTextureAutomapHeight;

        Vector3 backupCameraPositionViewFromTop;
        Quaternion backupCameraRotationViewFromTop;
        Vector3 backupCameraPositionView3D;
        Quaternion backupCameraRotationView3D;

        Vector3 biasFromInitialPositionViewFromTop;
        Vector3 biasFromInitialPositionView3D;

        public DaggerfallAutomapWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            ImgFile imgFile = null;
            DFBitmap bitmap = null;

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

            ParentPanel.Update();

            // Setup automap panel (into this the level geometry is rendered)
            //Rect positionPanelAutomap = new Rect(9f, 2f, (float)(ParentPanel.InteriorWidth - 109), (float)(ParentPanel.InteriorHeight - 198));
            //Rect positionPanelAutomap = new Rect(ParentPanel.Position.x, ParentPanel.Position.y, (float)(ParentPanel.Size.x - 19), (float)(ParentPanel.Size.y - 98));
            Rect positionPanelAutomap = new Rect(ParentPanel.Position.x, ParentPanel.Position.y, (float)(ParentPanel.InteriorWidth * 0.98f), (float)(ParentPanel.InteriorHeight * 0.8f));
            oldPositionPanelAutomap = positionPanelAutomap;
            if (panelAutomap == null)
            {
                panelAutomap = DaggerfallUI.AddPanel(positionPanelAutomap, ParentPanel);
                panelAutomap.ScalingMode = Scaling.None;
            }
            panelAutomap.OnMouseScrollUp += PanelAutomap_OnMouseScrollUp;
            panelAutomap.OnMouseScrollDown += PanelAutomap_OnMouseScrollDown;
            panelAutomap.OnMouseDown += PanelAutomap_OnMouseDown;
            panelAutomap.OnMouseUp += PanelAutomap_OnMouseUp;

            // Grid button (toggle 2D <-> 3D view)
            Button gridButton = DaggerfallUI.AddButton(new Rect(78, 171, 27, 19), NativePanel);
            gridButton.OnMouseClick += GridButton_OnMouseClick;
            gridButton.OnRightMouseClick += GridButton_OnRightMouseClick;
            gridButton.OnMouseScrollUp += GridButton_OnMouseScrollUp;
            gridButton.OnMouseScrollDown += GridButton_OnMouseScrollDown;

            // forward button
            Button forwardButton = DaggerfallUI.AddButton(new Rect(105, 171, 21, 19), NativePanel);
            forwardButton.OnMouseDown += ForwardButton_OnMouseDown;
            forwardButton.OnMouseUp += ForwardButton_OnMouseUp;

            // backward button
            Button backwardButton = DaggerfallUI.AddButton(new Rect(126, 171, 21, 19), NativePanel);
            backwardButton.OnMouseDown += BackwardButton_OnMouseDown;
            backwardButton.OnMouseUp += BackwardButton_OnMouseUp;

            // left button
            Button leftButton = DaggerfallUI.AddButton(new Rect(149, 171, 21, 19), NativePanel);
            leftButton.OnMouseDown += LeftButton_OnMouseDown;
            leftButton.OnMouseUp += LeftButton_OnMouseUp;
            
            // right button
            Button rightButton = DaggerfallUI.AddButton(new Rect(170, 171, 21, 19), NativePanel);
            rightButton.OnMouseDown += RightButton_OnMouseDown;
            rightButton.OnMouseUp += RightButton_OnMouseUp;

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

            // downstairs button
            Button downstairsButton = DaggerfallUI.AddButton(new Rect(258, 171, 21, 19), NativePanel);
            downstairsButton.OnMouseDown += DownstairsButton_OnMouseDown;
            downstairsButton.OnMouseUp += DownstairsButton_OnMouseUp;

            // Exit button
            Button exitButton = DaggerfallUI.AddButton(new Rect(281, 171, 28, 19), NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            if (!cameraAutomap)
            {
                GameObject gameObjectCameraAutomap = new GameObject("cameraAutomap");
                cameraAutomap = gameObjectCameraAutomap.AddComponent<Camera>();
                cameraAutomap.clearFlags = CameraClearFlags.SolidColor;
                cameraAutomap.cullingMask = 1;
                cameraAutomap.renderingPath = RenderingPath.DeferredLighting;
                cameraAutomap.nearClipPlane = 0.7f;
                cameraAutomap.farClipPlane = 1000.0f;
                cameraAutomap.fieldOfView = 45.0f;
            }

            createAutomapTextures((int)positionPanelAutomap.width, (int)positionPanelAutomap.height);

            resetCameraPosition();
            resetBiasFromInitialPosition();
            updateAutoMapView();

            NativePanel.Update(); // needed so that NativePanel.LocalScale holds correct value
            
            compass = new HUDCompass(cameraAutomap);
            Vector2 scale = NativePanel.LocalScale;
            compass.CompassBoxRect = new Rect(3f * scale.x, 172f * scale.y, 76f * scale.x, 17f * scale.y);
            compass.Scale = NativePanel.LocalScale;
            NativePanel.Components.Add(compass);
        }

        public override void Update()
        {
            base.Update();

            Rect positionPanelAutomap = new Rect(ParentPanel.Position.x, ParentPanel.Position.y, (float)(ParentPanel.InteriorWidth * 0.98f), (float)(ParentPanel.InteriorHeight * 0.8f));
            if (oldPositionPanelAutomap != positionPanelAutomap)
            {
                panelAutomap.Position = new Vector2(positionPanelAutomap.x, positionPanelAutomap.y);
                panelAutomap.Size = new Vector2(positionPanelAutomap.width, positionPanelAutomap.height);
                createAutomapTextures((int)positionPanelAutomap.width, (int)positionPanelAutomap.height);
                updateAutoMapView();
            }

            if (leftMouseDownOnPanelAutomap)
            {
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

                float dragSpeedCompensated = dragSpeed * Vector3.Magnitude(Camera.main.transform.position + getBiasFromInitialPosition() - cameraAutomap.transform.position);
                Vector2 bias = mousePosition - oldDragPosition;
                Vector3 translation = -cameraAutomap.transform.right * dragSpeedCompensated * bias.x + cameraAutomap.transform.up * dragSpeedCompensated * bias.y;
                cameraAutomap.transform.position += translation;
                updateAutoMapView();
                oldDragPosition = mousePosition;
            }

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

            if (leftMouseDownOnLeftButton)
            {
                Vector3 translation = cameraAutomap.transform.right * scrollLeftRightSpeed;
                translation.y = 0.0f; // comment this out for movement perpendicular to camera optical axis and up vector
                cameraAutomap.transform.position += translation;
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


            if (leftMouseDownOnRotateLeftButton)
            {
                Vector3 biasFromInitialPosition;
                switch (automapViewMode)
                {
                    case AutomapViewMode.View2D:
                        biasFromInitialPosition = biasFromInitialPositionViewFromTop;
                        break;
                    case AutomapViewMode.View3D:
                        biasFromInitialPosition = biasFromInitialPositionView3D;
                        break;
                    default:
                        biasFromInitialPosition = Vector3.zero;
                        break;
                }
                cameraAutomap.transform.RotateAround(Camera.main.transform.position + biasFromInitialPosition, -Vector3.up, -rotateSpeed);
                //cameraAutomap.transform.RotateAround(Camera.main.transform.position, Vector3.up, -rotateSpeed);
                updateAutoMapView();
            }

            if (leftMouseDownOnRotateRightButton)
            {
                Vector3 biasFromInitialPosition;
                switch (automapViewMode)
                {
                    case AutomapViewMode.View2D:
                        biasFromInitialPosition = biasFromInitialPositionViewFromTop;
                        break;
                    case AutomapViewMode.View3D:
                        biasFromInitialPosition = biasFromInitialPositionView3D;
                        break;
                    default:
                        biasFromInitialPosition = Vector3.zero;
                        break;
                }
                cameraAutomap.transform.RotateAround(Camera.main.transform.position + biasFromInitialPosition, -Vector3.up, +rotateSpeed);
                //cameraAutomap.transform.RotateAround(Camera.main.transform.position, Vector3.up, +rotateSpeed);
                updateAutoMapView();
            }

            if (leftMouseDownOnUpstairsButton)
            {
                cameraAutomap.transform.position += Vector3.up * scrollUpDownSpeed;
                updateAutoMapView();
            }

            if (leftMouseDownOnDownstairsButton)
            {
                cameraAutomap.transform.position += Vector3.down * scrollUpDownSpeed;
                updateAutoMapView();
            }
        }
        
        public override void OnPush()
        {
            if (IsSetup)
            {
                if (cameraAutomap)
                {
                    resetCameraPosition();
                    resetBiasFromInitialPosition();
                    updateAutoMapView();
                }              
            }
        }

        #region Private Methods

        private void createAutomapTextures(int width, int height)
        {
            if ((!renderTextureAutomap) || (oldRenderTextureAutomapWidth != width) || (oldRenderTextureAutomapHeight != height))
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
                case AutomapViewMode.View2D:
                    resetCameraTransformViewFromTop();
                    break;
                case AutomapViewMode.View3D:
                    resetCameraTransformView3D();
                    break;
                default:
                    break;
            }
        }

        private void resetBiasFromInitialPosition()
        {
            biasFromInitialPositionViewFromTop = Vector3.zero;
            biasFromInitialPositionView3D = Vector3.forward;
        }

        private void shiftBiasFromInitialPosition(Vector3 translation)
        {
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    biasFromInitialPositionViewFromTop += translation;
                    break;
                case AutomapViewMode.View3D:
                    biasFromInitialPositionView3D += translation;
                    break;
                default:
                    break;
            }
        }

        private Vector3 getBiasFromInitialPosition()
        {
            Vector3 biasFromInitialPosition;
            switch (automapViewMode)
            {
                case AutomapViewMode.View2D:
                    biasFromInitialPosition = biasFromInitialPositionViewFromTop;
                    break;
                case AutomapViewMode.View3D:
                    biasFromInitialPosition = biasFromInitialPositionView3D;
                    break;
                default:
                    biasFromInitialPosition = Vector3.zero;
                    break;
            }
            return (biasFromInitialPosition);
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
            if ((!cameraAutomap) || (!renderTextureAutomap))
                return;

            cameraAutomap.Render();

            RenderTexture.active = renderTextureAutomap;
            textureAutomap.ReadPixels(new Rect(0, 0, renderTextureAutomap.width, renderTextureAutomap.height), 0, 0);
            textureAutomap.Apply(false);
            RenderTexture.active = null;

            panelAutomap.BackgroundTexture = textureAutomap;
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
            oldDragPosition = mousePosition;            
            leftMouseDownOnPanelAutomap = true;
            alreadyInMouseDown = true;
        }

        private void PanelAutomap_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            leftMouseDownOnPanelAutomap = false;
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
                    updateAutoMapView();
                    break;
                case AutomapViewMode.View3D:
                    // update grid graphics
                    nativeTexture.SetPixels(78, nativeTexture.height - 171 - 19, 27, 19, pixelsGrid3D);
                    nativeTexture.Apply(false);
                    saveCameraTransformViewFromTop();
                    restoreOldCameraTransformView3D();
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
                    biasFromInitialPositionViewFromTop = Vector3.zero;
                    updateAutoMapView();
                    break;
                case AutomapViewMode.View3D:
                    biasFromInitialPositionView3D = Vector3.zero;
                    updateAutoMapView();
                    break;
                default:
                    break;
            }
        }        

        private void GridButton_OnMouseScrollUp()
        {
            if (inDragMode())
                return;

            if (automapViewMode == AutomapViewMode.View3D)
            {
                cameraAutomap.transform.Rotate(1.0f, 0.0f, 0.0f, Space.Self);
                updateAutoMapView();
            }
        }

        private void GridButton_OnMouseScrollDown()
        {
            if (inDragMode())
                return;

            if (automapViewMode == AutomapViewMode.View3D)
            {
                cameraAutomap.transform.Rotate(-1.0f, 0.0f, 0.0f, Space.Self);
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

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (inDragMode())
                return;

            CloseWindow();
        }

        #endregion
    }
}