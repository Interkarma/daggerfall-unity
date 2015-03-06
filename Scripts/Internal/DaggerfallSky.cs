// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Implementation of Daggerfall's sky backgrounds. Works in both forward and deferred rendering paths.
    /// Uses two cameras and OnPostRender in local camera for sky drawing (uses normal camera solid colour clear).
    /// Sets own camera depth to MainCamera.depth-1 so sky is drawn first.
    /// 
    /// DO NOT ATTACH THIS SCRIPT TO MAINCAMERA GAMEOBJECT.
    /// Attach to an empty GameObject or use the prefab provided.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class DaggerfallSky : MonoBehaviour
    {
        #region Fields

        // Maximum timescale supported by SetByWorldTime()
        public static float MaxTimeScale = 2000;

        public PlayerGPS LocalPlayerGPS;                                    // Set to local PlayerGPS
        [Range(0, 31)]
        public int SkyIndex = 16;                                           // Sky index for daytime skies
        [Range(0, 63)]
        public int SkyFrame = 31;                                           // Sky frame for daytime skies
        public bool IsNight = false;                                        // Swaps sky to night variant based on index
        public bool ShowStars = true;                                       // Draw stars onto night skies
        public Color SkyTintColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);      // Modulates output texture colour
        public float SkyColorScale = 1.0f;                                  // Scales sky color brighter or darker
        public WeatherStyle WeatherStyle = WeatherStyle.Normal;             // Style of weather for texture changes

        const int skyNativeWidth = 512;         // Native image width of sky image
        const int skyNativeHalfWidth = 256;     // Half native image width
        const int skyNativeHeight = 220;        // Native image height
        const float skyScale = 1.3f;            // Scale of sky image relative to display area
        const float skyHorizon = 0.20f;         // Higher the value lower the horizon

        DaggerfallUnity dfUnity;
        public SkyFile skyFile;
        public ImgFile imgFile;
        Camera mainCamera;
        Camera myCamera;
        Texture2D westTexture;
        Texture2D eastTexture;
        Color cameraClearColor;
        Rect fullTextureRect = new Rect(0, 0, 1, 1);
        int lastSkyIndex = -1;
        int lastSkyFrame = -1;
        bool lastNightFlag = false;
        Rect westRect, eastRect;
        CameraClearFlags initialClearFlags;
        System.Random random = new System.Random(0);
        bool showNightSky = true;

        SkyColors skyColors = new SkyColors();
        float starChance = 0.004f;
        byte[] starColorIndices = new byte[] { 16, 32, 74, 105, 112, 120 };     // Some random sky colour indices

        public struct SkyColors
        {
            public Color32[] west;
            public Color32[] east;
            public Color clearColor;
        }

        #endregion

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;

            // Try to find local player GPS if not set
            if (LocalPlayerGPS == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player)
                {
                    LocalPlayerGPS = player.GetComponent<PlayerGPS>();
                }
            }

            // Find main camera gameobject
            GameObject go = GameObject.FindGameObjectWithTag("MainCamera");
            if (go)
            {
                mainCamera = go.GetComponent<Camera>();
            }

            // Check main camera component
            if (!mainCamera)
            {
                DaggerfallUnity.LogMessage("DaggerfallSky could not find MainCamera object. Disabling sky.", true);
                gameObject.SetActive(false);
                return;
            }

            // Save starting clear flags
            initialClearFlags = mainCamera.clearFlags;

            // Get my camera
            myCamera = GetComponent<Camera>();
            if (!myCamera)
            {
                DaggerfallUnity.LogMessage("DaggerfallSky could not find local camera. Disabling sky.", true);
                gameObject.SetActive(false);
                return;
            }

            // My camera must not be on the same GameObject as MainCamera
            if (myCamera == mainCamera)
            {
                DaggerfallUnity.LogMessage("DaggerfallSky must not be attached to same GameObject as MainCamera. Disabling sky.", true);
                gameObject.SetActive(false);
                return;
            }

            // Setup cameras
            SetupCameras();
        }

        void OnEnable()
        {
            SetupCameras();
        }

        void OnDisable()
        {
            // Restore main camera clear flags so we left it how we found it
            if (mainCamera)
                mainCamera.clearFlags = initialClearFlags;
        }

        void Update()
        {
            // Do nothing if not ready
            if (!ReadyCheck())
                return;

            // Automate time of day updates
            if (dfUnity.Option_AutomateSky && LocalPlayerGPS)
                ApplyTimeAndSpace();

            // Update sky textures if index or frame changed
            if ((lastSkyIndex != SkyIndex || lastSkyFrame != SkyFrame || lastNightFlag != IsNight))
            {
                // Get target frame index based on am/pm
                int targetFrame = SkyFrame;
                bool flip = false;
                if (!IsNight && SkyFrame >= 32)
                {
                    targetFrame = 63 - SkyFrame;
                    flip = true;
                }

                LoadCurrentSky(targetFrame);
                PromoteToTexture(skyColors, flip);
                lastSkyIndex = SkyIndex;
                lastSkyFrame = SkyFrame;
                lastNightFlag = IsNight;
            }
        }

        void OnPostRender()
        {
            UpdateSkyRects();
            DrawSky();
        }

        #region Private Methods

        private void UpdateSkyRects()
        {
            Vector3 angles = mainCamera.transform.eulerAngles;
            float width = Screen.width * skyScale;
            float height = Screen.height * skyScale;
            float halfScreenWidth = Screen.width * 0.5f;

            // Scroll left-right
            float percent = 0;
            float scrollX = 0;
            float westOffset = 0;
            float eastOffset = 0;
            if (angles.y >= 90f && angles.y < 180f)
            {
                percent = 1.0f - ((360f - angles.y) / 180f);
                scrollX = -width * percent;

                westOffset = -width + halfScreenWidth;
                eastOffset = halfScreenWidth;
            }
            else if (angles.y >= 0f && angles.y < 90f)
            {
                percent = 1.0f - ((360f - angles.y) / 180f);
                scrollX = -width * percent;

                westOffset = -width + halfScreenWidth;
                eastOffset = westOffset - width;
            }
            else if (angles.y >= 180f && angles.y < 270f)
            {
                percent = 1.0f - (angles.y / 180f);
                scrollX = width * percent;

                westOffset = -width + halfScreenWidth;
                eastOffset = halfScreenWidth;
            }
            else// if (angles.y >= 270f && angles.y < 360f)
            {
                percent = 1.0f - (angles.y / 180f);
                scrollX = width * percent;

                eastOffset = halfScreenWidth;
                westOffset = eastOffset + width;
            }

            // Scroll up-down
            float horizonY = -Screen.height + (Screen.height * skyHorizon);
            float scrollY = horizonY;
            if (angles.x >= 270f && angles.x < 360f)
            {
                // Scroll down until top of sky is aligned with top of screen
                percent = (360f - angles.x) / 75f;
                scrollY += height * percent;
                if (scrollY > 0) scrollY = 0;
            }
            else
            {
                // Keep scrolling up
                percent = angles.x / 75f;
                scrollY -= height * percent;
            }

            westRect = new Rect(westOffset + scrollX, scrollY, width, height);
            eastRect = new Rect(eastOffset + scrollX, scrollY, width, height);
        }

        private void DrawSky()
        {
            if (!westTexture || !eastTexture)
                return;

            GL.PushMatrix();
            GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);

            // Draw sky hemispheres
            Graphics.DrawTexture(westRect, westTexture, fullTextureRect, 0, 0, 0, 0, SkyTintColor * SkyColorScale, null);
            Graphics.DrawTexture(eastRect, eastTexture, fullTextureRect, 0, 0, 0, 0, SkyTintColor * SkyColorScale, null);

            GL.PopMatrix();
        }

        private void PromoteToTexture(SkyColors colors, bool flip = false)
        {
            const int dayWidth = 512;
            const int dayHeight = 220;
            const int nightWidth = 512;
            const int nightHeight = 219;

            // Destroy old textures
            Destroy(westTexture);
            Destroy(eastTexture);

            // Create new textures
            if (!IsNight || !showNightSky)
            {
                westTexture = new Texture2D(dayWidth, dayHeight, TextureFormat.RGB24, false);
                eastTexture = new Texture2D(dayWidth, dayHeight, TextureFormat.RGB24, false);
            }
            else
            {
                westTexture = new Texture2D(nightWidth, nightHeight, TextureFormat.RGB24, false);
                eastTexture = new Texture2D(nightWidth, nightHeight, TextureFormat.RGB24, false);
            }

            // Set pixels, flipping hemisphere if required
            if (!flip)
            {
                westTexture.SetPixels32(colors.west);
                eastTexture.SetPixels32(colors.east);
            }
            else
            {
                westTexture.SetPixels32(colors.east);
                eastTexture.SetPixels32(colors.west);
            }

            // Set wrap mode
            eastTexture.wrapMode = TextureWrapMode.Clamp;
            westTexture.wrapMode = TextureWrapMode.Clamp;

            // Set filter mode
            westTexture.filterMode = dfUnity.MaterialReader.SkyFilterMode;
            eastTexture.filterMode = dfUnity.MaterialReader.SkyFilterMode;

            // Compress sky textures
            if (dfUnity.MaterialReader.CompressSkyTextures)
            {
                westTexture.Compress(true);
                eastTexture.Compress(true);
            }

            // Apply changes
            westTexture.Apply(false, true);
            eastTexture.Apply(false, true);

            // Set camera clear colour
            cameraClearColor = colors.clearColor;
            myCamera.backgroundColor = ((cameraClearColor * SkyTintColor) * 2f) * SkyColorScale;

            // Assign colour to fog
            UnityEngine.RenderSettings.fogColor = cameraClearColor;
        }

        private void ApplyTimeAndSpace()
        {
            // Do nothing if timescale too fast or we'll be thrashing texture loads
            if (dfUnity.WorldTime.TimeScale > MaxTimeScale)
                return;

            // Set sky index by climate, season, and weather
            switch (WeatherStyle)
            {
                case DaggerfallWorkshop.WeatherStyle.Rain1:
                    SkyIndex = LocalPlayerGPS.ClimateSettings.SkyBase + (int)WeatherStyle.Rain1;
                    break;
                case DaggerfallWorkshop.WeatherStyle.Rain2:
                    SkyIndex = LocalPlayerGPS.ClimateSettings.SkyBase + (int)WeatherStyle.Rain2;
                    break;
                case DaggerfallWorkshop.WeatherStyle.Snow1:
                    SkyIndex = LocalPlayerGPS.ClimateSettings.SkyBase + (int)WeatherStyle.Snow1;
                    break;
                case DaggerfallWorkshop.WeatherStyle.Snow2:
                    SkyIndex = LocalPlayerGPS.ClimateSettings.SkyBase + (int)WeatherStyle.Snow2;
                    break;
                default:
                    // Season value enum ordered same as sky indices
                    SkyIndex = LocalPlayerGPS.ClimateSettings.SkyBase + (int)dfUnity.WorldTime.Now.SeasonValue;
                    break;
            }

            // Set night flag
            IsNight = dfUnity.WorldTime.Now.IsNight;

            // Disable clear night sky for bad weather
            if (WeatherStyle != DaggerfallWorkshop.WeatherStyle.Normal)
                showNightSky = false;
            else
                showNightSky = true;

            // Adjust sky frame by time of day
            if (!IsNight)
            {
                float minute = dfUnity.WorldTime.Now.MinuteOfDay - DaggerfallDateTime.DawnHour * DaggerfallDateTime.MinutesPerHour;
                float divisor = ((DaggerfallDateTime.DuskHour - DaggerfallDateTime.DawnHour) * DaggerfallDateTime.MinutesPerHour) / 64f;   // Total of 64 steps in daytime cycle
                float frame = minute / divisor;
                SkyFrame = (int)frame;
            }
            else
            {
                SkyFrame = 0;
            }
        }

        private void LoadCurrentSky(int targetFrame)
        {
            if (!IsNight || !showNightSky)
                LoadDaySky(targetFrame);
            else
                LoadNightSky();
        }

        private void LoadDaySky(int frame)
        {
            skyFile = new SkyFile(Path.Combine(dfUnity.Arena2Path, SkyFile.IndexToFileName(SkyIndex)), FileUsage.UseMemory, true);

            skyFile.Palette = skyFile.GetDFPalette(frame);
            skyColors.east = skyFile.GetColors32(0, frame);
            skyColors.west = skyFile.GetColors32(1, frame);
            skyColors.clearColor = skyColors.west[0];
        }

        private void LoadNightSky()
        {
            const int width = 512;
            const int height = 219;

            // Get night sky matching sky index
            int nightSky;
            if (SkyIndex >= 0 && SkyIndex <= 7)
                nightSky = 3;
            else if (SkyIndex >= 8 && SkyIndex <= 15)
                nightSky = 1;
            else if (SkyIndex >= 16 && SkyIndex <= 23)
                nightSky = 2;
            else
                nightSky = 0;

            string filename = string.Format("NITE{0:00}I0.IMG", nightSky);
            imgFile = new ImgFile(Path.Combine(dfUnity.Arena2Path, filename), FileUsage.UseMemory, true);
            imgFile.Palette.Load(Path.Combine(dfUnity.Arena2Path, imgFile.PaletteName));

            // Get sky bitmap
            DFBitmap dfBitmap = imgFile.GetDFBitmap(0, 0);

            // Draw stars
            if (ShowStars)
            {
                for (int i = 0; i < dfBitmap.Data.Length; i++)
                {
                    // Stars should only be drawn over clear sky indices
                    int index = dfBitmap.Data[i];
                    if (index > 16 && index < 32)
                    {
                        if (random.NextDouble() < starChance)
                            dfBitmap.Data[i] = starColorIndices[random.Next(0, starColorIndices.Length)];
                    }
                }
            }

            // Get sky colour array
            Color32[] colors = imgFile.GetColors32(dfBitmap);

            // Fix seam on right side of night skies
            for (int y = 0; y < height; y++)
            {
                int pos = y * width + width - 2;
                colors[pos + 1] = colors[pos];
            }

            skyColors.west = colors;
            skyColors.east = colors;
            skyColors.clearColor = skyColors.west[0];
        }

        private void SetupCameras()
        {
            // Must have both cameras
            if (!mainCamera || !myCamera)
                return;

            myCamera.enabled = true;
            myCamera.renderingPath = mainCamera.renderingPath;
            myCamera.depth = mainCamera.depth - 1;
            myCamera.cullingMask = 0;
            myCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.clearFlags = CameraClearFlags.Nothing;
        }

        private bool ReadyCheck()
        {
            // Must have both world and sky cameras to draw
            if (!mainCamera || !myCamera)
                return false;

            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady)
            {
                DaggerfallUnity.LogMessage("DaggerfallSky: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            return true;
        }

        #endregion
    }
}
