// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
        Rect standardViewportRect = new Rect(0, 0, 1, 1);
        Rect lastViewportRect;
        Camera camera;

        private void Start()
        {
            camera = GetComponent<Camera>();
        }

        private void Update()
        {
            // HUD must be created
            if (DaggerfallUI.Instance.DaggerfallHUD == null)
                return;

            // Change viewport when large HUD is scale-to-fit
            // When not using scale-to-fit the large HUD is just an overlay of variable size and main viewport does not change
            if (DaggerfallUnity.Settings.LargeHUD && DaggerfallUnity.Settings.LargeHUDScaleToFit)
            {
                // Shrink viewport to area not occupied by scale-to-fit large HUD
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
                camera.rect = rect;
                lastViewportRect = rect;
            }
        }
    }
}