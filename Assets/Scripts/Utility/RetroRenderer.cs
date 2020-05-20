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
    /// Manages settings for retro mode rendering.
    /// </summary>
    public class RetroRenderer : MonoBehaviour
    {
        public RenderTexture RetroTexture320x200;
        public RenderTexture RetroTexture640x400;
        public RenderTexture RetroPresentationTarget;

        DaggerfallSky sky;
        RenderTexture retroTexture;
        int retroMode;

        Material depthProcessMaterial;

        public RenderTexture RetroTexture
        {
            get { return retroTexture; }
        }

        private void Start()
        {
            // Get retro mode and do nothing further if disabled
            retroMode = DaggerfallUnity.Settings.RetroRenderingMode;
            if (retroMode == 0)
                return;

            // Get sky reference
            sky = GameManager.Instance.SkyRig.GetComponent<DaggerfallSky>();

            // Get reference to retro rendertexture
            //  0 = retro rendering off
            //  1 = retro 320x200 rendering on
            //  2 = retro 640x400 rendering on
            if (retroMode == 1 && RetroTexture320x200)
                retroTexture = GameManager.Instance.MainCamera.targetTexture = RetroTexture320x200;
            else if (retroMode == 2 && RetroTexture640x400)
                retroTexture = GameManager.Instance.MainCamera.targetTexture = RetroTexture640x400;

            // Get depth process material
            Shader depthProcessShader = Shader.Find("Daggerfall/DepthProcessShader");
            depthProcessMaterial = new Material(depthProcessShader);
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
            if (retroMode == 0 || !retroTexture || !RetroPresentationTarget || !depthProcessMaterial)
                return;

            // Blit to presentation rendertexture with postprocess material
            Graphics.Blit(retroTexture, RetroPresentationTarget, depthProcessMaterial);
        }
    }
}