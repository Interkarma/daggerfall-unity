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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility.AssetInjection;

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

        public bool AutoCameraSetup = true;                                 // Automatically setup camera on enable/disable
        public PlayerGPS LocalPlayerGPS;                                    // Set to local PlayerGPS
        [Range(0, 31)]
        public int SkyIndex = 16;                                           // Sky index for daytime skies
        [Range(0, 63)]
        public int SkyFrame = 31;                                           // Sky frame for daytime skies
        public bool IsNight = false;                                        // Swaps sky to night variant based on index
        public bool ShowStars = true;                                       // Draw stars onto night skies
        public Color SkyTintColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);      // Modulates output texture colour
        public float SkyColorScale = 1.0f;                                  // Scales sky color brighter or darker
        public AnimationCurve SkyCurve;                                     // Animation curve of sky
        public WeatherStyle WeatherStyle = WeatherStyle.Normal;             // Style of weather for texture changes

        const int myCameraDepth = -3;           // Relative camera depth to main camera
        const int skyNativeWidth = 512;         // Native image width of sky image
        const int skyNativeHalfWidth = 256;     // Half native image width
        const int skyNativeHeight = 220;        // Native image height
        const float skyScale = 1.3f;            // Scale of sky image relative to display area

        DaggerfallUnity dfUnity;
        WeatherManager weatherManager;
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
        System.Random random = new System.Random(0);
        bool showNightSky = true;

        public SkyColors skyColors = new SkyColors();
        float starChance = 0.004f;
        byte[] starColorIndices = new byte[] { 16, 32, 74, 105, 112, 120 };     // Some random sky colour indices

        public struct SkyColors
        {
            public Color32[] west;
            public Color32[] east;
            public Color clearColor;
            public Vector2Int imageSize;
        }

        #endregion

        public Camera SkyCamera
        {
            get { return myCamera; }
        }

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
            weatherManager = FindObjectOfType<WeatherManager>();

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
            float width = (int)(Screen.width * skyScale);
            float height = Screen.height * skyScale;
            float halfScreenWidth = Screen.width * 0.5f;

            // Scroll left-right
            float westOffset;
            float eastOffset;
            float scrollX;
            // -180f <= yAngle < 180f
            float yAngle = angles.y < 180f ? angles.y : angles.y - 360f;
            if (yAngle >= 0f)
            {
                float percent = 1.0f - yAngle / 180f;
                scrollX = width * (percent - 1.0f);

                // westRect center
                westOffset = (int)(halfScreenWidth + scrollX);
                if (yAngle < 90f)
                    // eastRect to the left of westRect
                    eastOffset = westOffset - width;
                else
                    // eastRect to the right of westRect
                    eastOffset = westOffset + width;
            }
            else
            {
                float percent = -yAngle / 180f;
                scrollX = width * (percent - 1.0f);

                // eastRect center
                eastOffset = (int)(halfScreenWidth + scrollX);
                if (yAngle < -90f)
                    // westRect to the left of eastRect
                    westOffset = eastOffset - width;
                else
                    // westRect to the right of eastRect
                    westOffset = eastOffset + width;
            }

            // Scroll up-down
            // "baseScrollY" puts the bottom of the fake sky at the center of the screen.
            float baseScrollY = -(height - Screen.height) - (Screen.height / 2);

            // Zoom of the camera (1.0 at fov=90).
            float zoom = 1f / Mathf.Tan((mainCamera.fieldOfView * 0.50f) * Mathf.Deg2Rad);
            float angleXRadians = angles.x * Mathf.Deg2Rad;

            // Y-shearing is the percent of the screen height to translate Y coordinates by.
            float yShear = (Mathf.Tan(angleXRadians) * zoom) * 0.50f;
            float scrollY = Mathf.Clamp(baseScrollY - (yShear * Screen.height), -height, 0f);

            westRect = new Rect(westOffset, scrollY, width, height);
            eastRect = new Rect(eastOffset, scrollY, width, height);
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
            // Destroy old textures
            Destroy(westTexture);
            Destroy(eastTexture);

            // Create new textures
            westTexture = new Texture2D(colors.imageSize.x, colors.imageSize.y, TextureFormat.ARGB32, false);
            eastTexture = new Texture2D(colors.imageSize.x, colors.imageSize.y, TextureFormat.ARGB32, false);

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

            SetSkyFogColor(colors);
        }

        public void SetSkyFogColor(SkyColors colors)
        {
            // Set camera clear colour
            cameraClearColor = colors.clearColor;
            myCamera.backgroundColor = ((cameraClearColor * SkyTintColor) * 2f) * SkyColorScale;

            // Set gray fog color for anything denser than heavy rain, otherwise use sky color for atmospheric fogging
            // Only operates when weatherManager can be found (i.e. in game scene) while DaggerfallSky is running
            if (weatherManager)
            {
                WeatherManager.FogSettings currentFogSettings = GameManager.Instance.WeatherManager.currentOutdoorFogSettings;
                WeatherManager.FogSettings rainyFogSettings = GameManager.Instance.WeatherManager.RainyFogSettings;
                if (currentFogSettings.fogMode == FogMode.Exponential && currentFogSettings.density > rainyFogSettings.density)
                    RenderSettings.fogColor = Color.gray;
                else
                    RenderSettings.fogColor = cameraClearColor;
            }
            else
            {
                RenderSettings.fogColor = cameraClearColor;
            }
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

            // Adjust sky frame by time of day curve
            // Classic Daggerfall does not evenly spread frames across the day
            // Morning ramp-up bewteen 6am-8am and afternoon ramp-down between 4pm-6pm happens relatively quickly
            // Sky animation then slows down between 10am-2pm during brightest hours of day
            if (!IsNight)
            {
                // Get value 0-1 for dawn through dusk
                float dawn = DaggerfallDateTime.DawnHour * DaggerfallDateTime.MinutesPerHour;
                float dayRange = DaggerfallDateTime.DuskHour * DaggerfallDateTime.MinutesPerHour - dawn;
                float time = (dfUnity.WorldTime.Now.MinuteOfDay - dawn) / dayRange;

                // Set sky frame based on curve
                SkyFrame = (int)(SkyCurve.Evaluate(time) * 64);
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
                LoadNightSky(targetFrame);
        }

        private void LoadDaySky(int frame)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                TryLoadDayTextures(SkyIndex, frame,
                    out Texture2D westTexture, out Texture2D eastTexture);

                if (westTexture && eastTexture)
                {
                    skyColors = new SkyColors();
                    skyColors.east = eastTexture.GetPixels32();
                    skyColors.west = westTexture.GetPixels32();
                    skyColors.clearColor = skyColors.west[0];
                    skyColors.imageSize = new Vector2Int(westTexture.width, westTexture.height);
                    return;
                }
            }

            skyColors = LoadVanillaDaySky(SkyIndex, frame);
        }

        private void LoadNightSky(int frame)
        {
            if (DaggerfallUnity.Settings.AssetInjection)
            {
                TryLoadNightSkyTextures(SkyIndex, frame,
                    out Texture2D westTexture, out Texture2D eastTexture);

                if (westTexture && eastTexture)
                {
                    skyColors = new SkyColors();
                    skyColors.east = eastTexture.GetPixels32();
                    skyColors.west = westTexture.GetPixels32();
                    skyColors.clearColor = skyColors.west[0];
                    skyColors.imageSize = new Vector2Int(westTexture.width, westTexture.height);

                    return;
                }
            }

            skyColors = LoadVanillaNightSky(SkyIndex);
        }

        /// <summary>
        /// Tries loading day sky files from mods and loose files
        /// </summary>
        /// <param name="skyIndex"></param>
        /// <param name="frame"></param>
        /// <param name="westTexture"></param>
        /// <param name="eastTexture"></param>
        /// <param name="applyStars"></param>
        /// <returns></returns>
        private bool TryLoadDayTextures(int skyIndex, int frame, out Texture2D westTexture, out Texture2D eastTexture)
        {
            string baseName = string.Format("SKY{0:00}.DAT", skyIndex);

            //In format SKY00_0-0.DAT
            string eastName = Path.GetFileNameWithoutExtension(baseName) + "_0-" + frame + Path.GetExtension(baseName);
            string westName = Path.GetFileNameWithoutExtension(baseName) + "_1-" + frame + Path.GetExtension(baseName);

            TextureReplacement.TryImportTexture(eastName, false, out eastTexture);
            TextureReplacement.TryImportTexture(westName, false, out westTexture);

            if (eastTexture != null && westTexture != null)
                return true;

            return false;
        }

        /// <summary>
        /// Tries loading day sky files from mods and loose files
        /// </summary>
        /// <param name="skyIndex"></param>
        /// <param name="frame"></param>
        /// <param name="westTexture"></param>
        /// <param name="eastTexture"></param>
        /// <param name="applyStars"></param>
        /// <returns></returns>
        private bool TryLoadNightSkyTextures(int skyIndex, int frame, out Texture2D westTexture, out Texture2D eastTexture)
        {
            // Get night sky matching sky index
            int vanillaNightSky;
            if (skyIndex >= 0 && skyIndex <= 7)
                vanillaNightSky = 3;
            else if (skyIndex >= 8 && skyIndex <= 15)
                vanillaNightSky = 1;
            else if (skyIndex >= 16 && skyIndex <= 23)
                vanillaNightSky = 2;
            else
                vanillaNightSky = 0;

            //TYPE 1: NIGHTIME VANILLA
            string baseName = string.Format("NITE{0:00}I0.IMG", vanillaNightSky);
            TextureReplacement.TryImportTexture(baseName, false, out westTexture);

            //Vanilla worked!
            if (westTexture != null)
            {
                //Note east is copied from west
                eastTexture = westTexture;
                return true;
            }

            //TYPE 2: FULL NIGHTIME
            baseName = string.Format("NITEFULL{0:00}I0.IMG", skyIndex);
            TextureReplacement.TryImportTexture(baseName, false, out westTexture);

            //Full worked!
            if (westTexture != null)
            {
                //Note east is copied from west
                eastTexture = westTexture;
                return true;
            }

            //TYPE 3: NIGHTIME VANILLA
            baseName = string.Format("NITE{0:00}I0.IMG", vanillaNightSky);
            string eastName = Path.GetFileNameWithoutExtension(baseName) + "-0" + Path.GetExtension(baseName);
            string westName = Path.GetFileNameWithoutExtension(baseName) + "-1" + Path.GetExtension(baseName);

            TextureReplacement.TryImportTexture(eastName, false, out eastTexture);
            TextureReplacement.TryImportTexture(westName, false, out westTexture);

            //Vanilla with east+west worked!
            if (westTexture != null && eastTexture != null)
                return true;

            //TYPE 2: FULL NIGHTIME
            baseName = string.Format("NITEFULL{0:00}I0.IMG", skyIndex);
            eastName = Path.GetFileNameWithoutExtension(baseName) + "-0" + Path.GetExtension(baseName);
            westName = Path.GetFileNameWithoutExtension(baseName) + "-1" + Path.GetExtension(baseName);

            TextureReplacement.TryImportTexture(eastName, false, out eastTexture);
            TextureReplacement.TryImportTexture(westName, false, out westTexture);

            //Full with east+west worked!
            if (westTexture != null && eastTexture != null)
                return true;

            westTexture = null;
            eastTexture = null;
            return false;
        }

        /// <summary>
        /// Loads day sky from arena files
        /// </summary>
        /// <param name="skyIndex"></param>
        /// <returns>SkyColors loaded</returns>
        private SkyColors LoadVanillaDaySky(int skyIndex, int frame)
        {
            SkyFile skyFile = new SkyFile(Path.Combine(dfUnity.Arena2Path, SkyFile.IndexToFileName(skyIndex)), FileUsage.UseMemory, true);
            skyFile.Palette = skyFile.GetDFPalette(frame);

            SkyColors colors = new SkyColors();
            colors.east = skyFile.GetColor32(0, frame);
            colors.west = skyFile.GetColor32(1, frame);
            colors.clearColor = colors.west[0];
            colors.imageSize = new Vector2Int(512, 220);

            return colors;
        }

        /// <summary>
        /// Loads night sky from arena files and adds stars if enabled in settings
        /// </summary>
        /// <param name="skyIndex"></param>
        /// <returns>SkyColors loaded</returns>
        private SkyColors LoadVanillaNightSky(int skyIndex)
        {
            const int width = 512;
            const int height = 219;

            // Get night sky matching sky index
            int nightSky;
            if (skyIndex >= 0 && skyIndex <= 7)
                nightSky = 3;
            else if (skyIndex >= 8 && skyIndex <= 15)
                nightSky = 1;
            else if (skyIndex >= 16 && skyIndex <= 23)
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
            Color32[] colors = imgFile.GetColor32(dfBitmap);

            // Fix seam on right side of night skies
            for (int y = 0; y < height; y++)
            {
                int pos = y * width + width - 2;
                colors[pos + 1] = colors[pos];
            }

            SkyColors skyColors = new SkyColors();

            skyColors.west = colors;
            skyColors.east = colors;
            skyColors.clearColor = skyColors.west[0];
            skyColors.imageSize = new Vector2Int(512, 219);
            return skyColors;
        }

        private void SetupCameras()
        {
            // Must have both cameras
            if (!mainCamera || !myCamera)
                return;

            myCamera.enabled = true;
            myCamera.renderingPath = mainCamera.renderingPath;
            myCamera.cullingMask = 0;
            myCamera.clearFlags = CameraClearFlags.SolidColor;

            if (AutoCameraSetup)
            {
                myCamera.depth = mainCamera.depth + myCameraDepth;
            }
        }

        private bool ReadyCheck()
        {
            // Must have both world and sky cameras to draw
            if (!mainCamera || !myCamera)
                return false;

            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady || !dfUnity.IsPathValidated)
            {
                //DaggerfallUnity.LogMessage("DaggerfallSky: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            return true;
        }

        #endregion
    }
}
