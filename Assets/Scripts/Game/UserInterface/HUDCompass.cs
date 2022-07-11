// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Compass for HUD.
    /// </summary>
    public class HUDCompass : BaseScreenComponent
    {
        const string compassFilename = "COMPASS.IMG";
        const string compassBoxFilename = "COMPBOX.IMG";
        const string trackingIconFilename = "DetectMarker";

        Camera compassCamera;
        Texture2D compassTexture;
        Vector2 compassSize;
        Texture2D compassBoxTexture;
        Vector2 compassBoxSize;
        float eulerAngle;

        Texture2D defaultTrackingIcon;
        List<DetectEffect> registeredDetectors = new List<DetectEffect>();
        List<DetectEffect> expiredDetectors = new List<DetectEffect>();

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

        public HUDCompass()
            : base()
        {
            compassCamera = Camera.main;
            LoadAssets();

            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
        }

        public HUDCompass(Camera camera)
        {
            compassCamera = camera;
            HorizontalAlignment = HorizontalAlignment.Right;
            VerticalAlignment = VerticalAlignment.Bottom;
            LoadAssets();
        }

        public override void Update()
        {
            if (Enabled)
            {
                base.Update();
                Size = new Vector2(compassBoxSize.x * Scale.x, compassBoxSize.y * Scale.y);
            }
        }

        public override void Draw()
        {
            if (Enabled)
            {
                base.Draw();
                DrawCompass();
                DrawTrackedObjects();
                ClearExpiredDetectors();
            }
        }

        void LoadAssets()
        {
            compassTexture = DaggerfallUI.GetTextureFromImg(compassFilename);
            compassSize = TextureReplacement.GetSize(compassTexture, compassFilename, true);
            compassBoxTexture = DaggerfallUI.GetTextureFromImg(compassBoxFilename);
            compassBoxSize = TextureReplacement.GetSize(compassBoxTexture, compassBoxFilename, true);

            defaultTrackingIcon = Resources.Load<Texture2D>(trackingIconFilename);
            defaultTrackingIcon.filterMode = (FilterMode)DaggerfallUnity.Settings.GUIFilterMode;
        }

        void DrawCompass()
        {
            const int boxOutlineSize = 2;       // Pixel width of box outline
            const int boxInterior = 64;         // Pixel width of box interior
            const int nonWrappedPart = 258;     // Pixel width of non-wrapped part of compass strip

            if (!compassBoxTexture || !compassTexture)
                return;

            // Calculate displacement
            float percent;
            if (compassCamera != null)
                percent = compassCamera.transform.eulerAngles.y / 360f;
            else
                percent = eulerAngle;

            // Calculate scroll offset
            int scroll = (int)((float)nonWrappedPart * percent);

            // Compass box rect
            Rect compassBoxRect = new Rect();
            compassBoxRect.x = Position.x;
            compassBoxRect.y = Position.y;

            Vector2 boxRectSize = new Vector2(compassBoxSize.x * Scale.x, compassBoxSize.y * Scale.y);
            compassBoxRect.width = boxRectSize.x;
            compassBoxRect.height = boxRectSize.y;

            // Get compassTexture size
            float compassTextureWidth = compassSize.x;
            float compassTextureHeight = compassSize.y;

            // Compass strip source
            Rect compassSrcRect = new Rect();
            compassSrcRect.xMin = scroll / compassTextureWidth;
            compassSrcRect.yMin = 0;
            compassSrcRect.xMax = compassSrcRect.xMin + (float)boxInterior / compassTextureWidth;
            compassSrcRect.yMax = 1;

            // Compass strip destination
            Rect compassDstRect = new Rect();
            compassDstRect.x = compassBoxRect.x + boxOutlineSize * Scale.x;
            compassDstRect.y = compassBoxRect.y + boxOutlineSize * Scale.y;
            compassDstRect.width = compassBoxRect.width - (boxOutlineSize * 2) * Scale.x;
            compassDstRect.height = compassTextureHeight * Scale.y;

            DaggerfallUI.DrawTextureWithTexCoords(compassDstRect, compassTexture, compassSrcRect, false);
            DaggerfallUI.DrawTexture(compassBoxRect, compassBoxTexture, ScaleMode.StretchToFill, true);
        }

        /// <summary>
        /// Register a DetectEffect instance with compass.
        /// </summary>
        /// <param name="detectEffect">DetectEffect to register.</param>
        public void RegisterDetector(DetectEffect detectEffect)
        {
            if (detectEffect != null && !detectEffect.HasEnded)
            {
                registeredDetectors.Add(detectEffect);
                Debug.LogFormat("HUDCompass registered DetectEffect {0}", detectEffect.GetHashCode());
            }
        }

        /// <summary>
        /// Deregister a DetectEffect instance from compass.
        /// Compass will also expire effect once it has ended, but deregistering will remove markers immediately.
        /// </summary>
        /// <param name="detectorEffect"></param>
        public void DeregisterDetector(DetectEffect detectEffect)
        {
            if (detectEffect != null)
            {
                registeredDetectors.Remove(detectEffect);
                Debug.LogFormat("HUDCompass deregistered DetectEffect {0}", detectEffect.GetHashCode());
            }
        }

        void ClearExpiredDetectors()
        {
            if (expiredDetectors.Count > 0)
            {
                foreach (DetectEffect detector in expiredDetectors)
                {
                    registeredDetectors.Remove(detector);
                }
                expiredDetectors.Clear();
            }
        }

        void DrawTrackedObjects()
        {
            foreach (DetectEffect detector in registeredDetectors)
            {
                if (detector.HasEnded)
                {
                    expiredDetectors.Add(detector);
                    continue;
                }

                if (detector.DetectedObjects == null || detector.DetectedObjects.Count == 0)
                    continue;

                foreach(PlayerGPS.NearbyObject no in detector.DetectedObjects)
                {
                    if (no.gameObject != null)
                        DrawMarker(no.gameObject.transform.position);
                }
            }
        }

        void DrawMarker(Vector3 targetPosition)
        {
            // Get normal to target in horizontal plane
            Vector3 targetXZ = new Vector3(targetPosition.x, 0, targetPosition.z);
            Vector3 playerXZ = new Vector3(GameManager.Instance.PlayerObject.transform.position.x, 0, GameManager.Instance.PlayerObject.transform.position.z);
            Vector3 targetDirection = Vector3.Normalize(playerXZ - targetXZ);

            // Get angle to target
            Vector3 facingXZ = new Vector3(compassCamera.transform.forward.x, 0, compassCamera.transform.forward.z);
            float angle = (180f - Vector3.SignedAngle(targetDirection, facingXZ, Vector3.up)) / 360f;

            // Get left and right positions of compass box container - this can change with mods
            float boxLeft = Position.x;
            float boxRight = boxLeft + Size.x - defaultTrackingIcon.width * Scale.x;

            // Convert angle into lerp from left to right
            float lerp;
            if (angle >= 0 && angle < 0.5f)
                lerp = ChangeRange(angle, 0.25f, 0.0f, 1.0f, 0.5f);     // Object is to right of player
            else
                lerp = ChangeRange(angle, 1.0f, 0.75f, 0.5f, 0.0f);     // Object is to left of player

            // Set marker on compass box
            Rect markerRect = new Rect()
            {
                x = Mathf.Lerp(boxLeft, boxRight, lerp),
                y = Position.y - defaultTrackingIcon.height * Scale.y,
                width = defaultTrackingIcon.width * Scale.x,
                height = defaultTrackingIcon.height * Scale.y,
            };

            // Draw marker
            DaggerfallUI.DrawTexture(markerRect, defaultTrackingIcon, ScaleMode.StretchToFill, true);
        }

        float ChangeRange(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            return (value - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
        }

        void ResetTrackingState()
        {
            registeredDetectors.Clear();
            expiredDetectors.Clear();
        }

        #region Event Handlers

        private void StartGameBehaviour_OnNewGame()
        {
            ResetTrackingState();
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            ResetTrackingState();
        }

        #endregion
    }
}