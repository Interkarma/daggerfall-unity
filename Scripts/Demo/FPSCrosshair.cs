// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Demo
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
                GUI.color = new Color(1, 1, 1, 0.75f);
                GUI.DrawTexture(
                    new Rect((Screen.width * 0.5f) - (CrosshairTexture.width * 0.5f),
                        (Screen.height * 0.5f) - (CrosshairTexture.height * 0.5f),
                        CrosshairTexture.width,
                        CrosshairTexture.height), CrosshairTexture);
                GUI.color = Color.white;
            }
        }
    }
}