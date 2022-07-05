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
using System.Collections;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Simple component to flash screen red on player damage.
    /// </summary>
    public class ShowPlayerDamage : MonoBehaviour
    {
        Texture2D damageTexture;
        bool fadingOut = false;
        float alphaFadeValue = 0;
        float fadeSpeed = 0.7f;

        void Start()
        {
            if (damageTexture == null)
                damageTexture = __ExternalAssets.iTween.CameraTexture(new Color(1, 0, 0, 1));
        }

        public void Flash()
        {
            alphaFadeValue = 0.4f;
            fadingOut = true;
        }

        void OnGUI()
        {
            if (fadingOut)
            {
                alphaFadeValue -= fadeSpeed * Time.deltaTime;
                if (alphaFadeValue > 0)
                {
                    Color color = new Color(1, 0, 0, alphaFadeValue);
                    DaggerfallUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), damageTexture, ScaleMode.StretchToFill, true, color);
                }
                else
                {
                    alphaFadeValue = 0;
                    fadingOut = false;
                }
            }
        }
    }
}