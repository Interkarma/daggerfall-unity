// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Implements the large HUD in Daggerfall Unity as just another overlay inside of primary HUD.
    /// This is so it can work uniformly in both widescreen and retro modes.
    /// Other HUD elements can still work alongside large HUD, but some will be disabled as they occupy same screen area.
    /// </summary>
    public class HUDLarge : Panel
    {
        const string mainFilename = "MAIN00I0.IMG";
        const string compass0Filename = "CMPA00I0.BSS";     // Standard compass
        const string compass1Filename = "CMPA01I0.BSS";     // Blue compass (unused)
        const string compass2Filename = "CMPA02I0.BSS";     // Red compass (unused)
        const int compassFrameCount = 32;

        protected Rect compassPanelRect = new Rect(275, 2, 43, 42);

        Texture2D mainTexture;
        Texture2D[] compassTextures = new Texture2D[compassFrameCount];

        Panel compassPanel;
        Camera compassCamera;
        float eulerAngle;

        PlayerEntity playerEntity;

        /// <summary>
        /// Gets or sets a compass camera to automatically determine compass heading.
        /// </summary>
        public Camera CompassCamera
        {
            get { return compassCamera; }
            set { compassCamera = value; }
        }

        /// <summary>
        /// Gets or a sets a Euler angle to use for compass heading.
        /// This value is only observed when CompassCamera is null.
        /// </summary>
        public float EulerAngle
        {
            get { return eulerAngle; }
            set { eulerAngle = Mathf.Clamp(value, 0f, 360f); }
        }

        public HUDLarge()
            : base()
        {
            CompassCamera = Camera.main;
            playerEntity = GameManager.Instance.PlayerEntity;
            LoadAssets();
            Setup();
        }

        void LoadAssets()
        {
            mainTexture = ImageReader.GetTexture(mainFilename);

            // Read compass animations
            for (int i = 0; i < compassFrameCount; i++)
            {
                compassTextures[i] = ImageReader.GetTexture(compass0Filename, 0, i, true);
            }
        }

        void Setup()
        {
            BackgroundTexture = mainTexture;

            compassPanel = DaggerfallUI.AddPanel(compassPanelRect);
            Components.Add(compassPanel);
        }

        public override void Update()
        {
            if (!Enabled)
                return;

            // When using custom scale adjust position and size based on screen scale
            // HUD elements exist outside of native window space so elements have full control over their own placement
            // This also requires some manual adjustments as child panels don't inherit scale
            if (!DaggerfallUnity.Settings.LargeHUDScaleToFit)
            {
                compassPanel.Position = compassPanelRect.position * Scale;
                compassPanel.Size = compassPanelRect.size * Scale;
            }

            // Calculate compass rotation percent
            float percent;
            if (compassCamera != null)
                percent = compassCamera.transform.eulerAngles.y / 360f;
            else
                percent = eulerAngle;

            // Update compass pointer
            compassPanel.BackgroundTexture = compassTextures[(int)(compassFrameCount * percent)];

            base.Update();
        }
    }
}