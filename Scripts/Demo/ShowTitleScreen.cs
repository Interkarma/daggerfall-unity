// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System;
using System.Collections;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Displays a title screen overlay when ShowTitle is true.
    /// </summary>
    public class ShowTitleScreen : MonoBehaviour
    {
        public Texture2D TitleScreenTexture;
        public bool ShowTitle = false;

        Texture2D blackTexture;

        void Awake()
        {
            blackTexture = new Texture2D(16, 16);
            Color32[] colors = new Color32[16 * 16];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.black;
            }
            blackTexture.SetPixels32(colors);
            blackTexture.Apply(false, true);

            if (!TitleScreenTexture)
                TitleScreenTexture = Resources.Load<Texture2D>("TitleScreen");
            if (TitleScreenTexture)
                TitleScreenTexture.filterMode = FilterMode.Point;
        }

        void OnGUI()
        {
            if (ShowTitle && TitleScreenTexture)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture, ScaleMode.StretchToFill);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), TitleScreenTexture, ScaleMode.ScaleToFit, false, 1.6f);
            }
        }
    }
}