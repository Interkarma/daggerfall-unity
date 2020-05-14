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

using DaggerfallWorkshop.Game;
using UnityEngine;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Simple behaviour just to blit a render texture into an empty viewport camera.
    /// Does all the setup required or disables self when retro mode not enabled.
    /// Retro 320x200 rendering works as follows when enabled:
    /// - Main camera and SkyRig camera are set to use RetroTarget 320x200 render texture.
    /// - RetroCamera is a stationary viewport camera with culling mask of nothing.
    /// - RetroTarget output is set as the SourceRenderTexture on this behaviour.
    /// - OnRenderImage will blit 320x200 output to RetroCamera.
    /// </summary>
    public class RetroRenderer : MonoBehaviour
    {
        public RenderTexture RetroTexture320x200;
        public RenderTexture RetroTexture640x400;

        DaggerfallSky sky;
        RenderTexture retroTexture;

        public static bool enablePostprocessing = false;
        private Material postprocessMaterial = null;

        public RenderTexture RetroTexture
        {
            get { return retroTexture; }
        }

        public Material PostprocessMaterial
        {
            get { return postprocessMaterial; }
        }

        static Color[] art_pal =
        {
            new Color( 255f/255, 229f/255, 129f/255),
            new Color( 255f/255, 206f/255, 107f/255),
            new Color( 255f/255, 206f/255,  99f/255),
            new Color( 247f/255, 206f/255, 115f/255),
            new Color( 255f/255, 206f/255,  90f/255),
            new Color( 247f/255, 206f/255, 107f/255),
            new Color( 239f/255, 206f/255, 115f/255),
            new Color( 231f/255, 206f/255, 123f/255),
            new Color( 255f/255, 198f/255,  99f/255),
            new Color( 255f/255, 197f/255,  86f/255),
            new Color( 231f/255, 198f/255, 122f/255),
            new Color( 222f/255, 198f/255, 128f/255),
            new Color( 247f/255, 189f/255,  79f/255),
            new Color( 208f/255, 185f/255, 134f/255),
            new Color( 228f/255, 178f/255,  80f/255),
            new Color( 186f/255, 174f/255, 147f/255),
            new Color( 176f/255, 164f/255, 148f/255),
            new Color( 206f/255, 159f/255,  73f/255),
            new Color( 179f/255, 160f/255, 121f/255),
            new Color( 165f/255, 156f/255, 156f/255),
            new Color( 185f/255, 148f/255,  76f/255),
            new Color( 161f/255, 147f/255, 125f/255),
            new Color( 164f/255, 141f/255,  94f/255),
            new Color( 164f/255, 130f/255,  67f/255),
            new Color( 140f/255, 129f/255, 119f/255),
            new Color( 137f/255, 121f/255,  94f/255),
            new Color( 132f/255, 119f/255, 107f/255),
            new Color( 132f/255, 114f/255,  82f/255),
            new Color( 137f/255, 112f/255,  66f/255),
            new Color( 118f/255, 105f/255,  93f/255),
            new Color( 112f/255,  94f/255,  72f/255),
            new Color( 244f/255, 202f/255, 167f/255),
            new Color( 227f/255, 180f/255, 144f/255),
            new Color( 207f/255, 152f/255, 118f/255),
            new Color( 193f/255, 133f/255, 100f/255),
            new Color( 180f/255, 113f/255,  80f/255),
            new Color( 165f/255, 100f/255,  70f/255),
            new Color( 152f/255,  93f/255,  63f/255),
            new Color( 140f/255,  86f/255,  55f/255),
            new Color( 129f/255,  79f/255,  48f/255),
            new Color( 122f/255,  75f/255,  43f/255),
            new Color( 112f/255,  70f/255,  40f/255),
            new Color( 103f/255,  64f/255,  39f/255),
            new Color(  91f/255,  67f/255,  38f/255),
            new Color(  79f/255,  63f/255,  43f/255),
            new Color(  66f/255,  54f/255,  41f/255),
            new Color(  54f/255,  50f/255,  40f/255),
            new Color( 232f/255, 188f/255, 200f/255),
            new Color( 220f/255, 166f/255, 188f/255),
            new Color( 204f/255, 146f/255, 170f/255),
            new Color( 188f/255, 127f/255, 158f/255),
            new Color( 175f/255, 111f/255, 144f/255),
            new Color( 155f/255,  98f/255, 130f/255),
            new Color( 143f/255,  84f/255, 119f/255),
            new Color( 127f/255,  77f/255, 106f/255),
            new Color( 109f/255,  69f/255, 102f/255),
            new Color( 101f/255,  65f/255,  96f/255),
            new Color(  86f/255,  58f/255,  77f/255),
            new Color(  75f/255,  52f/255,  71f/255),
            new Color(  67f/255,  51f/255,  63f/255),
            new Color(  63f/255,  47f/255,  56f/255),
            new Color(  56f/255,  45f/255,  52f/255),
            new Color(  46f/255,  44f/255,  46f/255),
            new Color( 245f/255, 212f/255, 172f/255),
            new Color( 229f/255, 193f/255, 150f/255),
            new Color( 213f/255, 174f/255, 128f/255),
            new Color( 196f/255, 154f/255, 105f/255),
            new Color( 183f/255, 140f/255,  88f/255),
            new Color( 173f/255, 127f/255,  78f/255),
            new Color( 160f/255, 118f/255,  74f/255),
            new Color( 151f/255, 110f/255,  69f/255),
            new Color( 134f/255, 103f/255,  65f/255),
            new Color( 123f/255,  92f/255,  60f/255),
            new Color( 109f/255,  85f/255,  54f/255),
            new Color(  96f/255,  76f/255,  51f/255),
            new Color(  83f/255,  71f/255,  44f/255),
            new Color(  69f/255,  63f/255,  42f/255),
            new Color(  61f/255,  54f/255,  38f/255),
            new Color(  50f/255,  45f/255,  34f/255),
            new Color( 205f/255, 205f/255, 224f/255),
            new Color( 188f/255, 188f/255, 199f/255),
            new Color( 165f/255, 165f/255, 174f/255),
            new Color( 145f/255, 145f/255, 159f/255),
            new Color( 135f/255, 135f/255, 149f/255),
            new Color( 122f/255, 122f/255, 137f/255),
            new Color( 114f/255, 114f/255, 127f/255),
            new Color( 103f/255, 103f/255, 116f/255),
            new Color(  94f/255,  94f/255, 109f/255),
            new Color(  85f/255,  85f/255,  96f/255),
            new Color(  75f/255,  75f/255,  85f/255),
            new Color(  68f/255,  68f/255,  80f/255),
            new Color(  61f/255,  61f/255,  67f/255),
            new Color(  53f/255,  53f/255,  59f/255),
            new Color(  48f/255,  48f/255,  50f/255),
            new Color(  44f/255,  44f/255,  45f/255),
            new Color( 176f/255, 205f/255, 255f/255),
            new Color( 147f/255, 185f/255, 244f/255),
            new Color( 123f/255, 164f/255, 230f/255),
            new Color( 104f/255, 152f/255, 217f/255),
            new Color(  87f/255, 137f/255, 205f/255),
            new Color(  68f/255, 124f/255, 192f/255),
            new Color(  68f/255, 112f/255, 179f/255),
            new Color(  62f/255, 105f/255, 167f/255),
            new Color(  55f/255,  97f/255, 154f/255),
            new Color(  49f/255,  90f/255, 142f/255),
            new Color(  45f/255,  82f/255, 122f/255),
            new Color(  51f/255,  77f/255, 102f/255),
            new Color(  52f/255,  69f/255,  87f/255),
            new Color(  50f/255,  62f/255,  73f/255),
            new Color(  47f/255,  59f/255,  60f/255),
            new Color(  44f/255,  48f/255,  49f/255),
            new Color( 220f/255, 220f/255, 220f/255),
            new Color( 197f/255, 197f/255, 197f/255),
            new Color( 185f/255, 185f/255, 185f/255),
            new Color( 174f/255, 174f/255, 174f/255),
            new Color( 162f/255, 162f/255, 162f/255),
            new Color( 147f/255, 147f/255, 147f/255),
            new Color( 132f/255, 132f/255, 132f/255),
            new Color( 119f/255, 119f/255, 119f/255),
            new Color( 110f/255, 110f/255, 110f/255),
            new Color(  99f/255,  99f/255,  99f/255),
            new Color(  87f/255,  87f/255,  87f/255),
            new Color(  78f/255,  78f/255,  78f/255),
            new Color(  67f/255,  67f/255,  67f/255),
            new Color(  58f/255,  58f/255,  58f/255),
            new Color(  51f/255,  51f/255,  51f/255),
            new Color(  44f/255,  44f/255,  44f/255),
            new Color( 182f/255, 218f/255, 227f/255),
            new Color( 158f/255, 202f/255, 202f/255),
            new Color( 134f/255, 187f/255, 187f/255),
            new Color( 109f/255, 170f/255, 170f/255),
            new Color(  87f/255, 154f/255, 154f/255),
            new Color(  77f/255, 142f/255, 142f/255),
            new Color(  70f/255, 135f/255, 135f/255),
            new Color(  62f/255, 124f/255, 124f/255),
            new Color(  54f/255, 112f/255, 112f/255),
            new Color(  46f/255, 103f/255, 103f/255),
            new Color(  39f/255,  91f/255,  91f/255),
            new Color(  40f/255,  83f/255,  83f/255),
            new Color(  45f/255,  72f/255,  72f/255),
            new Color(  47f/255,  63f/255,  63f/255),
            new Color(  50f/255,  55f/255,  55f/255),
            new Color(  45f/255,  48f/255,  48f/255),
            new Color( 255f/255, 246f/255, 103f/255),
            new Color( 241f/255, 238f/255,  45f/255),
            new Color( 226f/255, 220f/255,   0f/255),
            new Color( 212f/255, 203f/255,   0f/255),
            new Color( 197f/255, 185f/255,   0f/255),
            new Color( 183f/255, 168f/255,   0f/255),
            new Color( 168f/255, 150f/255,   0f/255),
            new Color( 154f/255, 133f/255,   0f/255),
            new Color( 139f/255, 115f/255,   0f/255),
            new Color( 127f/255, 106f/255,   4f/255),
            new Color( 116f/255,  97f/255,   7f/255),
            new Color( 104f/255,  87f/255,  11f/255),
            new Color(  93f/255,  78f/255,  14f/255),
            new Color(  81f/255,  69f/255,  18f/255),
            new Color(  69f/255,  60f/255,  21f/255),
            new Color(  58f/255,  51f/255,  25f/255),
            new Color( 202f/255, 221f/255, 196f/255),
            new Color( 175f/255, 200f/255, 168f/255),
            new Color( 148f/255, 176f/255, 141f/255),
            new Color( 123f/255, 156f/255, 118f/255),
            new Color( 107f/255, 144f/255, 109f/255),
            new Color(  93f/255, 130f/255,  94f/255),
            new Color(  82f/255, 116f/255,  86f/255),
            new Color(  77f/255, 110f/255,  78f/255),
            new Color(  68f/255,  99f/255,  67f/255),
            new Color(  61f/255,  89f/255,  53f/255),
            new Color(  52f/255,  77f/255,  45f/255),
            new Color(  46f/255,  68f/255,  37f/255),
            new Color(  39f/255,  60f/255,  39f/255),
            new Color(  30f/255,  55f/255,  30f/255),
            new Color(  34f/255,  51f/255,  34f/255),
            new Color(  40f/255,  47f/255,  40f/255),
            new Color( 179f/255, 107f/255,  83f/255),
            new Color( 175f/255,  95f/255,  75f/255),
            new Color( 175f/255,  87f/255,  67f/255),
            new Color( 163f/255,  79f/255,  59f/255),
            new Color( 155f/255,  75f/255,  51f/255),
            new Color( 147f/255,  71f/255,  47f/255),
            new Color( 155f/255,  91f/255,  47f/255),
            new Color( 139f/255,  83f/255,  43f/255),
            new Color( 127f/255,  75f/255,  39f/255),
            new Color( 115f/255,  67f/255,  35f/255),
            new Color(  99f/255,  63f/255,  31f/255),
            new Color(  87f/255,  55f/255,  27f/255),
            new Color(  75f/255,  47f/255,  23f/255),
            new Color(  59f/255,  39f/255,  19f/255),
            new Color(  47f/255,  31f/255,  15f/255),
            new Color(  35f/255,  23f/255,  11f/255),
            new Color( 216f/255, 227f/255, 162f/255),
            new Color( 185f/255, 205f/255, 127f/255),
            new Color( 159f/255, 183f/255, 101f/255),
            new Color( 130f/255, 162f/255,  77f/255),
            new Color( 109f/255, 146f/255,  66f/255),
            new Color( 101f/255, 137f/255,  60f/255),
            new Color(  92f/255, 127f/255,  54f/255),
            new Color(  84f/255, 118f/255,  48f/255),
            new Color(  76f/255, 108f/255,  42f/255),
            new Color(  65f/255,  98f/255,  37f/255),
            new Color(  53f/255,  87f/255,  34f/255),
            new Color(  51f/255,  75f/255,  35f/255),
            new Color(  45f/255,  64f/255,  37f/255),
            new Color(  43f/255,  56f/255,  39f/255),
            new Color(  38f/255,  51f/255,  40f/255),
            new Color(  43f/255,  46f/255,  45f/255),
            new Color( 179f/255, 115f/255,  79f/255),
            new Color( 175f/255, 111f/255,  75f/255),
            new Color( 171f/255, 107f/255,  71f/255),
            new Color( 167f/255, 103f/255,  67f/255),
            new Color( 159f/255,  99f/255,  63f/255),
            new Color( 155f/255,  95f/255,  59f/255),
            new Color( 151f/255,  91f/255,  55f/255),
            new Color( 143f/255,  87f/255,  51f/255),
            new Color(  40f/255,  40f/255,  40f/255),
            new Color(  38f/255,  38f/255,  38f/255),
            new Color(  35f/255,  35f/255,  35f/255),
            new Color(  31f/255,  31f/255,  31f/255),
            new Color(  27f/255,  27f/255,  27f/255),
            new Color(  23f/255,  23f/255,  23f/255),
            new Color(  19f/255,  19f/255,  19f/255),
            new Color(  15f/255,  15f/255,  15f/255),
            new Color( 254f/255, 255f/255, 199f/255),
            new Color( 254f/255, 245f/255, 185f/255),
            new Color( 254f/255, 235f/255, 170f/255),
            new Color( 254f/255, 225f/255, 156f/255),
            new Color( 255f/255, 215f/255, 141f/255),
            new Color( 255f/255, 205f/255, 127f/255),
            new Color( 255f/255, 195f/255, 112f/255),
            new Color( 255f/255, 185f/255,  98f/255),
            new Color( 255f/255, 175f/255,  83f/255),
            new Color( 241f/255, 167f/255,  54f/255),
            new Color( 234f/255, 155f/255,  50f/255),
            new Color( 226f/255, 143f/255,  46f/255),
            new Color( 219f/255, 131f/255,  43f/255),
            new Color( 212f/255, 119f/255,  39f/255),
            new Color( 205f/255, 107f/255,  35f/255),
            new Color( 198f/255,  95f/255,  31f/255),
            new Color( 190f/255,  84f/255,  27f/255),
            new Color( 183f/255,  72f/255,  23f/255),
            new Color( 176f/255,  60f/255,  19f/255),
            new Color( 169f/255,  48f/255,  15f/255),
            new Color( 162f/255,  36f/255,  12f/255),
            new Color( 154f/255,  24f/255,   8f/255),
            new Color( 147f/255,  12f/255,   4f/255),
            new Color( 130f/255,  22f/255,   0f/255),
            new Color( 111f/255,  34f/255,   0f/255),
            new Color( 102f/255,  33f/255,   1f/255),
            new Color(  92f/255,  33f/255,   3f/255),
            new Color(  83f/255,  32f/255,  10f/255),
            new Color(  74f/255,  39f/255,  27f/255),
            new Color(  65f/255,  41f/255,  33f/255),
            new Color(  57f/255,  43f/255,  39f/255),
            new Color(   0f/255,   0f/255,   0f/255),
        };

        // LUT downsampling
        const int lutShift = 1;

        Texture3D lut = null;

        class Palette
        {
            private Color[] colors;

            public Palette(Color[] colors)
            {
                this.colors = colors;
            }

            public Color GetNearestColor(Color targetColor)
            {
                int bestColorIndex = 0;
                float bestFit = colorSqrDistance(targetColor, bestColorIndex);
                for (int i = 1; i < art_pal.Length; i++)
                {
                    float fit = colorSqrDistance(targetColor, i);
                    if (fit < bestFit)
                    {
                        bestColorIndex = i;
                        bestFit = fit;
                    }
                }
                return colors[bestColorIndex];
            }

            private float colorSqrDistance(Color targetColor, int i)
            {
                float diffr = (targetColor.r - colors[i].r);
                float diffg = (targetColor.g - colors[i].g);
                float diffb = (targetColor.b - colors[i].b);
                return diffr * diffr + diffg * diffg + diffb * diffb;
            }

        }

        private void initLut()
        {
            if (lut)
                return;

            Palette palette = new Palette(art_pal);

            int size = 256 >> lutShift;
            lut = new Texture3D(size, size, size, TextureFormat.RGBA32, false);
            lut.wrapMode = TextureWrapMode.Clamp;

            Color[] colors = new Color[size * size * size];
            for (int b = 0; b < size; b++)
            {
                float bFrac = (float)b / (size - 1);
                int bOffset = b * size * size;
                for (int g = 0; g < size; g++)
                {
                    float gFrac = (float)g / (size - 1);
                    int gOffset = g * size;
                    for (int r = 0; r < size; r++)
                    {
                        float rFrac = (float)r / (size - 1);
                        Color targetColor = new Color(rFrac, gFrac, bFrac);
                        colors[r + gOffset + bOffset] = palette.GetNearestColor(targetColor);
                    }
                }
            }
            lut.SetPixels(colors);
            lut.Apply();
        }

        private void Start()
        {
            // 0 = retro rendering off
            // 1 = retro 320x200 rendering on
            // 2 = retro 640x400 rendering on
            sky = GameManager.Instance.SkyRig.GetComponent<DaggerfallSky>();


            if (DaggerfallUnity.Settings.RetroRenderingMode == 1 && RetroTexture320x200)
                retroTexture = GameManager.Instance.MainCamera.targetTexture = RetroTexture320x200;
            else if (DaggerfallUnity.Settings.RetroRenderingMode == 2 && RetroTexture640x400)
                retroTexture = GameManager.Instance.MainCamera.targetTexture = RetroTexture640x400;
            else
                gameObject.SetActive(false);

            enablePostprocessing = DaggerfallUnity.Settings.PostProcessingInRetroMode > 0;
        }

        private void Update()
        {
            // Conditionally handle classic sky camera
            // Sky may not be enabled at startup (e.g starting in dungeon) so need to check
            // Does nothing when retro world setting disabled as this behaviour is also disabled
            if (sky && sky.SkyCamera && sky.SkyCamera.targetTexture != retroTexture)
                sky.SkyCamera.targetTexture = retroTexture;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!retroTexture)
                return;

            if (enablePostprocessing)
            {
                if (!postprocessMaterial)
                {
                    Shader shader;
                    switch (DaggerfallUnity.Settings.PostProcessingInRetroMode)
                    {
                        case 1:
                            shader = Shader.Find(MaterialReader._DaggerfallRetroPosterizationShaderName);
                            postprocessMaterial = new Material(shader);
                            break;
                        case 2:
                            initLut();
                            shader = Shader.Find(MaterialReader._DaggerfallRetroPalettizationShaderName);
                            postprocessMaterial = new Material(shader);
                            postprocessMaterial.SetTexture("_Lut", lut);
                            postprocessMaterial.SetInt("_LutQuantization", 256 >> lutShift);
                            break;
                    }
                    if (!postprocessMaterial)
                    {
                        Debug.Log("Couldn't find retro shader " + DaggerfallUnity.Settings.PostProcessingInRetroMode);
                        enablePostprocessing = false;
                    }
                }
                if (enablePostprocessing && postprocessMaterial)
                {
                    Graphics.Blit(retroTexture, null as RenderTexture, postprocessMaterial);
                    return;
                }
            }
            Graphics.Blit(retroTexture, null as RenderTexture);
        }
    }
}