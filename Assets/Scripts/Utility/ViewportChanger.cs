// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net), Pango
// Contributors:    
// 
// Notes:
//

using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using UnityEngine;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Changes camera viewport based on large HUD configuration.
    /// </summary>
    public class ViewportChanger : MonoBehaviour
    {
        public bool isRetroPresenter = false;
        public Camera retroClearerCamera;

        Rect standardViewportRect = new Rect(0, 0, 1, 1);
        Rect lastViewportRect;
        new Camera camera;

        private void Start()
        {
            camera = GetComponent<Camera>();
        }

        private void Update()
        {
            // HUD must be created
            if (DaggerfallUI.Instance.DaggerfallHUD == null)
                return;

            // Offload when using retro aspect correction
            if (isRetroPresenter && DaggerfallUnity.Settings.RetroModeAspectCorrection != 0)
            {
                SetRetroAspectViewport();
                return;
            }
            else
            {
                DaggerfallUI.Instance.CustomScreenRect = null;
            }

            // Change viewport when large HUD is docked
            // When not using docked the large HUD is just an overlay of variable size and main viewport does not change
            if (DaggerfallUnity.Settings.LargeHUD && DaggerfallUnity.Settings.LargeHUDDocked)
            {
                // Shrink viewport to area not occupied by docked large HUD
                // Check size every frame as HUD height can change (e.g. resizing window, changing resolution)
                HUDLarge largeHUD = DaggerfallUI.Instance.DaggerfallHUD.LargeHUD;
                float hudHeight = largeHUD.ScreenHeight / Screen.height;
                Rect rect = new Rect(0, hudHeight, 1, 1 - hudHeight);
                SetViewport(rect);
            }
            else
            {
                // Set standard viewport area
                SetViewport(standardViewportRect);
            }
        }

        void SetViewport(Rect rect)
        {
            // Do nothing if viewport rect hasn't changed
            if (rect == lastViewportRect)
                return;

            // Set viewport rect to camera
            if (camera)
            {
                // Handle retro rendering mode
                // Camera viewport does not work with render textures so need to adjust output to appropriately size render target instead
                // Then retro presentation needs to use correct screen viewport area, not main camera
                if (DaggerfallUnity.Settings.RetroRenderingMode != 0 && !isRetroPresenter)
                {
                    camera.rect = standardViewportRect;
                    GameManager.Instance.RetroRenderer.UpdateRenderTarget();
                }
                else
                {
                    camera.rect = rect;
                }

                lastViewportRect = rect;
            }
        }

        void SetRetroAspectViewport()
        {
            float heightRatio = 0;
            int viewWidth = 0;
            RetroModeAspects aspect = (RetroModeAspects)DaggerfallUnity.Settings.RetroModeAspectCorrection;
            if (aspect == RetroModeAspects.FourThree)
            {
                // Classic rendered at 320x200 (Mode13h/16:10) but was typically displayed on 4:3 monitors (e.g. 320x240)
                // In this environment display output signal was stretched 20% higher in vertical dimension
                // This setting scales output viewport to simulate resulting aspect ratio in this environment
                // Works from ideal 16:10 > 4:3 upscale (1600x1200 or 5x width, 6x height, 20% higher) and ratios into actual screen area

                // Start with screen height at 6x classic to get a ratio
                heightRatio = Screen.height / 6f / 200f;

                // Then determine 5x classic width at this ratio
                viewWidth = (int)(320f * 5f * heightRatio);
            }
            else if (aspect == RetroModeAspects.SixteenTen)
            {
                // Upscale 320x200 6x in both dimensions to 1920x1200, a very common 16:10 resolution, then ratio into actual screen area

                // Start with screen height at 6x classic to get a ratio
                heightRatio = Screen.height / 6f / 200f;

                // Then determine 6x classic width at this ratio
                viewWidth = (int)(320f * 6f * heightRatio);
            }

            // Get pillarbox width offset to centre viewport horizontally
            int pillarWidth = (Screen.width - viewWidth) / 2;

            // Handle docked large HUD
            float hudHeight = DaggerfallUnity.Settings.LargeHUD && DaggerfallUnity.Settings.LargeHUDDocked
                                ? DaggerfallUI.Instance.DaggerfallHUD.LargeHUD.ScreenHeight / Screen.height
                                : 0;

            // Set final viewport area
            float x = (float)pillarWidth / Screen.width;
            float h = 1 - x * 2;
            Rect rect = new Rect(x, hudHeight, h, 1.0f - hudHeight);

            // Get screen rect and pass over to UI so it can treat this viewport as entire screen space
            Rect adjustedScreenRect = new Rect(pillarWidth, 0, Screen.width - pillarWidth * 2, Screen.height);
            DaggerfallUI.Instance.CustomScreenRect = adjustedScreenRect;

            // After adjusting output viewport pillarbox bars won't be cleared automatically
            // Use camera with -1 depth covering entire viewport to clear black first
            // This camera renders nothing, just clears screen black before custom viewport drawn centred in screen
            if (retroClearerCamera && !retroClearerCamera.gameObject.activeSelf)
                retroClearerCamera.gameObject.SetActive(true);

            SetViewport(rect);
        }
    }
}