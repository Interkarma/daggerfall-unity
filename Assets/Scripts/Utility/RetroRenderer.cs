// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
        public RenderTexture RetroTexture;
        DaggerfallSky sky;

        private void Start()
        {
            if (DaggerfallUnity.Settings.Retro320x200World && RetroTexture)
            {
                GameManager.Instance.MainCamera.targetTexture = RetroTexture;
                sky = GameManager.Instance.SkyRig.GetComponent<DaggerfallSky>();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            // Conditionally handle classic sky camera
            // Sky may not be enabled at startup (e.g starting in dungeon) so need to check
            // Does nothing when retro world setting disabled as this behaviour is also disabled
            if (sky && sky.SkyCamera && sky.SkyCamera.targetTexture != RetroTexture)
                sky.SkyCamera.targetTexture = RetroTexture;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (RetroTexture)
                Graphics.Blit(RetroTexture, null as RenderTexture);
        }
    }
}