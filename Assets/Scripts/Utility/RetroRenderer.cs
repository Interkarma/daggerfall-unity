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
using UnityEngine;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Manages settings for retro mode rendering.
    /// </summary>
    public class RetroRenderer : MonoBehaviour
    {
        public RenderTexture RetroTexture320x200;
        public RenderTexture RetroTexture320x200_HUD;
        public RenderTexture RetroTexture640x400;
        public RenderTexture RetroTexture640x400_HUD;
        public RenderTexture RetroPresentationTarget;

        private const string ExcludeSkyKeyword = "EXCLUDE_SKY";

        DaggerfallSky sky;
        RetroPresentation retroPresenter;
        RenderTexture retroTexture;
        int retroMode;
        private bool enablePostprocessing = true;

        Material postprocessMaterial;

        public RenderTexture RetroTexture
        {
            get { return retroTexture; }
        }

        public Material PostprocessMaterial
        {
            get { return postprocessMaterial; }
        }

        public void TogglePostprocessing()
        {
            enablePostprocessing = !enablePostprocessing;
        }

        static Color32[] art_pal =
        {
            new Color32( 255, 229, 129, 255),
            new Color32( 255, 206, 107, 255),
            new Color32( 255, 206,  99, 255),
            new Color32( 247, 206, 115, 255),
            new Color32( 255, 206,  90, 255),
            new Color32( 247, 206, 107, 255),
            new Color32( 239, 206, 115, 255),
            new Color32( 231, 206, 123, 255),
            new Color32( 255, 198,  99, 255),
            new Color32( 255, 197,  86, 255),
            new Color32( 231, 198, 122, 255),
            new Color32( 222, 198, 128, 255),
            new Color32( 247, 189,  79, 255),
            new Color32( 208, 185, 134, 255),
            new Color32( 228, 178,  80, 255),
            new Color32( 186, 174, 147, 255),
            new Color32( 176, 164, 148, 255),
            new Color32( 206, 159,  73, 255),
            new Color32( 179, 160, 121, 255),
            new Color32( 165, 156, 156, 255),
            new Color32( 185, 148,  76, 255),
            new Color32( 161, 147, 125, 255),
            new Color32( 164, 141,  94, 255),
            new Color32( 164, 130,  67, 255),
            new Color32( 140, 129, 119, 255),
            new Color32( 137, 121,  94, 255),
            new Color32( 132, 119, 107, 255),
            new Color32( 132, 114,  82, 255),
            new Color32( 137, 112,  66, 255),
            new Color32( 118, 105,  93, 255),
            new Color32( 112,  94,  72, 255),
            new Color32( 244, 202, 167, 255),
            new Color32( 227, 180, 144, 255),
            new Color32( 207, 152, 118, 255),
            new Color32( 193, 133, 100, 255),
            new Color32( 180, 113,  80, 255),
            new Color32( 165, 100,  70, 255),
            new Color32( 152,  93,  63, 255),
            new Color32( 140,  86,  55, 255),
            new Color32( 129,  79,  48, 255),
            new Color32( 122,  75,  43, 255),
            new Color32( 112,  70,  40, 255),
            new Color32( 103,  64,  39, 255),
            new Color32(  91,  67,  38, 255),
            new Color32(  79,  63,  43, 255),
            new Color32(  66,  54,  41, 255),
            new Color32(  54,  50,  40, 255),
            new Color32( 232, 188, 200, 255),
            new Color32( 220, 166, 188, 255),
            new Color32( 204, 146, 170, 255),
            new Color32( 188, 127, 158, 255),
            new Color32( 175, 111, 144, 255),
            new Color32( 155,  98, 130, 255),
            new Color32( 143,  84, 119, 255),
            new Color32( 127,  77, 106, 255),
            new Color32( 109,  69, 102, 255),
            new Color32( 101,  65,  96, 255),
            new Color32(  86,  58,  77, 255),
            new Color32(  75,  52,  71, 255),
            new Color32(  67,  51,  63, 255),
            new Color32(  63,  47,  56, 255),
            new Color32(  56,  45,  52, 255),
            new Color32(  46,  44,  46, 255),
            new Color32( 245, 212, 172, 255),
            new Color32( 229, 193, 150, 255),
            new Color32( 213, 174, 128, 255),
            new Color32( 196, 154, 105, 255),
            new Color32( 183, 140,  88, 255),
            new Color32( 173, 127,  78, 255),
            new Color32( 160, 118,  74, 255),
            new Color32( 151, 110,  69, 255),
            new Color32( 134, 103,  65, 255),
            new Color32( 123,  92,  60, 255),
            new Color32( 109,  85,  54, 255),
            new Color32(  96,  76,  51, 255),
            new Color32(  83,  71,  44, 255),
            new Color32(  69,  63,  42, 255),
            new Color32(  61,  54,  38, 255),
            new Color32(  50,  45,  34, 255),
            new Color32( 205, 205, 224, 255),
            new Color32( 188, 188, 199, 255),
            new Color32( 165, 165, 174, 255),
            new Color32( 145, 145, 159, 255),
            new Color32( 135, 135, 149, 255),
            new Color32( 122, 122, 137, 255),
            new Color32( 114, 114, 127, 255),
            new Color32( 103, 103, 116, 255),
            new Color32(  94,  94, 109, 255),
            new Color32(  85,  85,  96, 255),
            new Color32(  75,  75,  85, 255),
            new Color32(  68,  68,  80, 255),
            new Color32(  61,  61,  67, 255),
            new Color32(  53,  53,  59, 255),
            new Color32(  48,  48,  50, 255),
            new Color32(  44,  44,  45, 255),
            new Color32( 176, 205, 255, 255),
            new Color32( 147, 185, 244, 255),
            new Color32( 123, 164, 230, 255),
            new Color32( 104, 152, 217, 255),
            new Color32(  87, 137, 205, 255),
            new Color32(  68, 124, 192, 255),
            new Color32(  68, 112, 179, 255),
            new Color32(  62, 105, 167, 255),
            new Color32(  55,  97, 154, 255),
            new Color32(  49,  90, 142, 255),
            new Color32(  45,  82, 122, 255),
            new Color32(  51,  77, 102, 255),
            new Color32(  52,  69,  87, 255),
            new Color32(  50,  62,  73, 255),
            new Color32(  47,  59,  60, 255),
            new Color32(  44,  48,  49, 255),
            new Color32( 220, 220, 220, 255),
            new Color32( 197, 197, 197, 255),
            new Color32( 185, 185, 185, 255),
            new Color32( 174, 174, 174, 255),
            new Color32( 162, 162, 162, 255),
            new Color32( 147, 147, 147, 255),
            new Color32( 132, 132, 132, 255),
            new Color32( 119, 119, 119, 255),
            new Color32( 110, 110, 110, 255),
            new Color32(  99,  99,  99, 255),
            new Color32(  87,  87,  87, 255),
            new Color32(  78,  78,  78, 255),
            new Color32(  67,  67,  67, 255),
            new Color32(  58,  58,  58, 255),
            new Color32(  51,  51,  51, 255),
            new Color32(  44,  44,  44, 255),
            new Color32( 182, 218, 227, 255),
            new Color32( 158, 202, 202, 255),
            new Color32( 134, 187, 187, 255),
            new Color32( 109, 170, 170, 255),
            new Color32(  87, 154, 154, 255),
            new Color32(  77, 142, 142, 255),
            new Color32(  70, 135, 135, 255),
            new Color32(  62, 124, 124, 255),
            new Color32(  54, 112, 112, 255),
            new Color32(  46, 103, 103, 255),
            new Color32(  39,  91,  91, 255),
            new Color32(  40,  83,  83, 255),
            new Color32(  45,  72,  72, 255),
            new Color32(  47,  63,  63, 255),
            new Color32(  50,  55,  55, 255),
            new Color32(  45,  48,  48, 255),
            new Color32( 255, 246, 103, 255),
            new Color32( 241, 238,  45, 255),
            new Color32( 226, 220,   0, 255),
            new Color32( 212, 203,   0, 255),
            new Color32( 197, 185,   0, 255),
            new Color32( 183, 168,   0, 255),
            new Color32( 168, 150,   0, 255),
            new Color32( 154, 133,   0, 255),
            new Color32( 139, 115,   0, 255),
            new Color32( 127, 106,   4, 255),
            new Color32( 116,  97,   7, 255),
            new Color32( 104,  87,  11, 255),
            new Color32(  93,  78,  14, 255),
            new Color32(  81,  69,  18, 255),
            new Color32(  69,  60,  21, 255),
            new Color32(  58,  51,  25, 255),
            new Color32( 202, 221, 196, 255),
            new Color32( 175, 200, 168, 255),
            new Color32( 148, 176, 141, 255),
            new Color32( 123, 156, 118, 255),
            new Color32( 107, 144, 109, 255),
            new Color32(  93, 130,  94, 255),
            new Color32(  82, 116,  86, 255),
            new Color32(  77, 110,  78, 255),
            new Color32(  68,  99,  67, 255),
            new Color32(  61,  89,  53, 255),
            new Color32(  52,  77,  45, 255),
            new Color32(  46,  68,  37, 255),
            new Color32(  39,  60,  39, 255),
            new Color32(  30,  55,  30, 255),
            new Color32(  34,  51,  34, 255),
            new Color32(  40,  47,  40, 255),
            new Color32( 179, 107,  83, 255),
            new Color32( 175,  95,  75, 255),
            new Color32( 175,  87,  67, 255),
            new Color32( 163,  79,  59, 255),
            new Color32( 155,  75,  51, 255),
            new Color32( 147,  71,  47, 255),
            new Color32( 155,  91,  47, 255),
            new Color32( 139,  83,  43, 255),
            new Color32( 127,  75,  39, 255),
            new Color32( 115,  67,  35, 255),
            new Color32(  99,  63,  31, 255),
            new Color32(  87,  55,  27, 255),
            new Color32(  75,  47,  23, 255),
            new Color32(  59,  39,  19, 255),
            new Color32(  47,  31,  15, 255),
            new Color32(  35,  23,  11, 255),
            new Color32( 216, 227, 162, 255),
            new Color32( 185, 205, 127, 255),
            new Color32( 159, 183, 101, 255),
            new Color32( 130, 162,  77, 255),
            new Color32( 109, 146,  66, 255),
            new Color32( 101, 137,  60, 255),
            new Color32(  92, 127,  54, 255),
            new Color32(  84, 118,  48, 255),
            new Color32(  76, 108,  42, 255),
            new Color32(  65,  98,  37, 255),
            new Color32(  53,  87,  34, 255),
            new Color32(  51,  75,  35, 255),
            new Color32(  45,  64,  37, 255),
            new Color32(  43,  56,  39, 255),
            new Color32(  38,  51,  40, 255),
            new Color32(  43,  46,  45, 255),
            new Color32( 179, 115,  79, 255),
            new Color32( 175, 111,  75, 255),
            new Color32( 171, 107,  71, 255),
            new Color32( 167, 103,  67, 255),
            new Color32( 159,  99,  63, 255),
            new Color32( 155,  95,  59, 255),
            new Color32( 151,  91,  55, 255),
            new Color32( 143,  87,  51, 255),
            new Color32(  40,  40,  40, 255),
            new Color32(  38,  38,  38, 255),
            new Color32(  35,  35,  35, 255),
            new Color32(  31,  31,  31, 255),
            new Color32(  27,  27,  27, 255),
            new Color32(  23,  23,  23, 255),
            new Color32(  19,  19,  19, 255),
            new Color32(  15,  15,  15, 255),
            new Color32( 254, 255, 199, 255),
            new Color32( 254, 245, 185, 255),
            new Color32( 254, 235, 170, 255),
            new Color32( 254, 225, 156, 255),
            new Color32( 255, 215, 141, 255),
            new Color32( 255, 205, 127, 255),
            new Color32( 255, 195, 112, 255),
            new Color32( 255, 185,  98, 255),
            new Color32( 255, 175,  83, 255),
            new Color32( 241, 167,  54, 255),
            new Color32( 234, 155,  50, 255),
            new Color32( 226, 143,  46, 255),
            new Color32( 219, 131,  43, 255),
            new Color32( 212, 119,  39, 255),
            new Color32( 205, 107,  35, 255),
            new Color32( 198,  95,  31, 255),
            new Color32( 190,  84,  27, 255),
            new Color32( 183,  72,  23, 255),
            new Color32( 176,  60,  19, 255),
            new Color32( 169,  48,  15, 255),
            new Color32( 162,  36,  12, 255),
            new Color32( 154,  24,   8, 255),
            new Color32( 147,  12,   4, 255),
            new Color32( 130,  22,   0, 255),
            new Color32( 111,  34,   0, 255),
            new Color32( 102,  33,   1, 255),
            new Color32(  92,  33,   3, 255),
            new Color32(  83,  32,  10, 255),
            new Color32(  74,  39,  27, 255),
            new Color32(  65,  41,  33, 255),
            new Color32(  57,  43,  39, 255),
            new Color32(   0,   0,   0, 255),
            // Add a few missing grey levels
            new Color32(   4,   4,   4, 255),
            new Color32(   8,   8,   8, 255),
            new Color32(  12,  12,  12, 255),
        };

        // LUT downsampling
        // 0 - 64MB, 7s init (excellent, similar to k-d tree shader)
        // 1 - 8MB, 850ms init (very good, hard to tell from 0 visually, probably less cache misses too)
        // 2 - 1MB (good, slightly less crisp looking)
        // quality goes downhill from here, as some classic colors get conflated together in the LUT
        // 6 - EGA with bad color choice
        Texture3D lut = null;

        private void InitLut(int lutShift, int size)
        {
            if (lut)
                return;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            FastColorPalette.IPalette palette = FastColorPalette.BuildPalette(art_pal);
            watch.Stop();
            Debug.Log("Time spent building palette = " + watch.ElapsedMilliseconds + "ms");

            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            lut = new Texture3D(size, size, size, TextureFormat.RGBA32, false);
            lut.filterMode = FilterMode.Point;
            lut.wrapMode = TextureWrapMode.Clamp;

            Color32[] colors = new Color32[size * size * size];
            Color32 targetColor = new Color32();
            targetColor.a = 255;
            int colorsIndex = 0;
            Color32 color;
            for (int b = 0; b < size; b++)
            {
                targetColor.b = (byte)((b << lutShift));
                for (int g = 0; g < size; g++)
                {
                    targetColor.g = (byte)((g << lutShift));
                    for (int r = 0; r < size; r++)
                    {
                        targetColor.r = (byte)((r << lutShift));
                        palette.GetNearestColor(targetColor, out color);
                        colors[colorsIndex++] = color;
                    }
                }
            }
            watch2.Stop();
            Debug.Log("Time spent filling LUT = " + watch2.ElapsedMilliseconds + "ms");
            var watch3 = System.Diagnostics.Stopwatch.StartNew();
            lut.SetPixels32(colors);
            lut.Apply();
            watch3.Stop();
            Debug.Log("Time spent transferring LUT = " + watch3.ElapsedMilliseconds + "ms");
        }

        private void Start()
        {
            sky = GameManager.Instance.SkyRig.GetComponent<DaggerfallSky>();
            retroPresenter = GameManager.Instance.RetroPresenter;
        }

        public void UpdateSettings()
        {
            retroMode = DaggerfallUnity.Settings.RetroRenderingMode;
            UpdateRenderTarget();
            UpdateDepthProcessMaterial();
            if (retroPresenter)
                retroPresenter.gameObject.SetActive(retroMode != 0);
            if (sky && sky.SkyCamera && retroMode == 0)
                sky.SkyCamera.targetTexture = null;
        }

        public void UpdateDepthProcessMaterial()
        {
            // Get depth process material
            Shader shader;
            switch (DaggerfallUnity.Settings.PostProcessingInRetroMode)
            {
                case 0:
                    shader = Shader.Find(MaterialReader._DaggerfallRetroDepthShaderName);
                    postprocessMaterial = new Material(shader);
                    break;
                case 1:
                    postprocessMaterial = GetPosterizationMaterial(false);
                    break;
                case 2:
                    postprocessMaterial = GetPosterizationMaterial(true);
                    break;
                case 3:
                    postprocessMaterial = GetPalettizationMaterial(false);
                    break;
                case 4:
                    postprocessMaterial = GetPalettizationMaterial(true);
                    break;
            }
            if (!postprocessMaterial)
            {
                Debug.Log("Couldn't find retro shader " + DaggerfallUnity.Settings.PostProcessingInRetroMode);
                retroMode = 0;
            }
        }

        public void UpdateRenderTarget()
        {
            // Disable retro target texture when retro mode disabled
            if (DaggerfallUnity.Settings.RetroRenderingMode == 0)
            {
                GameManager.Instance.MainCamera.targetTexture = null;
                return;
            }

            // Unity viewport rect does not work with target render textures
            // Need to set new target with custom size when using a docked large HUD
            if (DaggerfallUnity.Settings.LargeHUD && DaggerfallUnity.Settings.LargeHUDDocked)
            {
                // Get reference to retro rendertexture
                //  0 = retro rendering off
                //  1 = retro 320x200 rendering on with docked large HUD
                //  2 = retro 640x400 rendering on with docked large HUD
                if (retroMode == 1 && RetroTexture320x200)
                    retroTexture = GameManager.Instance.MainCamera.targetTexture = RetroTexture320x200_HUD;
                else if (retroMode == 2 && RetroTexture640x400)
                    retroTexture = GameManager.Instance.MainCamera.targetTexture = RetroTexture640x400_HUD;
            }
            else
            {
                // Get reference to retro rendertexture
                //  0 = retro rendering off
                //  1 = retro 320x200 rendering on
                //  2 = retro 640x400 rendering on
                if (retroMode == 1 && RetroTexture320x200)
                    retroTexture = GameManager.Instance.MainCamera.targetTexture = RetroTexture320x200;
                else if (retroMode == 2 && RetroTexture640x400)
                    retroTexture = GameManager.Instance.MainCamera.targetTexture = RetroTexture640x400;
            }
        }

        private Material GetPosterizationMaterial(bool excludeSky)
        {
            Shader shader = Shader.Find(MaterialReader._DaggerfallRetroPosterizationShaderName);
            if (!shader)
                return null;

            Material material = new Material(shader);
            if (excludeSky)
                material.EnableKeyword(ExcludeSkyKeyword);
            else
                material.DisableKeyword(ExcludeSkyKeyword);
            return material;
        }

        private Material GetPalettizationMaterial(bool excludeSky)
        {
            Shader shader = Shader.Find(MaterialReader._DaggerfallRetroPalettizationShaderName);
            if (!shader)
                return null;

            int lutShift = DaggerfallUnity.Settings.PalettizationLUTShift;
            int size = 256 >> lutShift;
            InitLut(lutShift, size);

            Material material = new Material(shader);
            material.SetTexture("_Lut", lut);
            if (excludeSky)
                material.EnableKeyword(ExcludeSkyKeyword);
            else
                material.DisableKeyword(ExcludeSkyKeyword);
            return material;
        }

        private void Update()
        {
            // Do nothing if retro mode disabled
            if (retroMode == 0)
                return;

            // Conditionally handle classic sky camera
            // Sky may not be enabled at startup (e.g starting in dungeon) so need to check
            // Does nothing when retro world setting disabled as this behaviour is also disabled
            if (sky && sky.SkyCamera && retroTexture && sky.SkyCamera.targetTexture != retroTexture)
                sky.SkyCamera.targetTexture = retroTexture;
        }

        private void OnPostRender()
        {
            // Do nothing if retro mode disabled or texture not set
            if (retroMode == 0 || !retroTexture || !RetroPresentationTarget || !postprocessMaterial)
                return;

            // Blit to presentation rendertexture with postprocess material
            if (enablePostprocessing)
                Graphics.Blit(retroTexture, RetroPresentationTarget, postprocessMaterial);
            else
                Graphics.Blit(retroTexture, RetroPresentationTarget);
        }
    }
}