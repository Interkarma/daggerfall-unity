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
    /// Just drawing a basic crosshair using OnGUI().
    /// </summary>
    public class FPSCrosshair : MonoBehaviour
    {
        public Texture CrosshairTexture;

        void OnGUI()
        {
            if (CrosshairTexture != null)
            {
                Color color = new Color(1, 1, 1, 0.75f);
                DaggerfallUI.DrawTexture(
                    new Rect((Screen.width * 0.5f) - (CrosshairTexture.width * 0.5f),
                        (Screen.height * 0.5f) - (CrosshairTexture.height * 0.5f),
                        CrosshairTexture.width, CrosshairTexture.height),
                        (Texture2D)CrosshairTexture,
                        ScaleMode.StretchToFill,
                        true,
                        color);
            }
        }
    }
}