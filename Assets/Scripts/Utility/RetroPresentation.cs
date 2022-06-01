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

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Presents final retro rendering to viewport.
    /// </summary>
    public class RetroPresentation : MonoBehaviour
    {
        public RenderTexture RetroPresentationSource;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (RetroPresentationSource && DaggerfallUnity.Settings.RetroRenderingMode != 0)
            {
                // Present retro render
                Graphics.Blit(RetroPresentationSource, null as RenderTexture);
            }
        }
    }
}