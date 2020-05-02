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
using DaggerfallWorkshop.Game;

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
                    Shader shader = null;
                    switch(DaggerfallUnity.Settings.PostProcessingInRetroMode)
                    {
                        case 1:
                            shader = Shader.Find(MaterialReader._DaggerfallRetroPosterizationShaderName);
                            break;
                        case 2:
                            shader = Shader.Find(MaterialReader._DaggerfallRetroPalettizationShaderName);
                            break;
                    }
                    if (shader)
                        postprocessMaterial = new Material(shader);
                    else
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