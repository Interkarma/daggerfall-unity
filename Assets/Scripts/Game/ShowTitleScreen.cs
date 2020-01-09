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
using System;
using System.Collections;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Displays a title screen overlay when ShowTitle is true.
    /// </summary>
    public class ShowTitleScreen : MonoBehaviour
    {
        public Texture2D TitleScreenTexture;

        Texture2D blackTexture;
        public bool showTitle = false;
        bool lastShowTitle = false;

        public bool ShowTitle
        {
            get
            {
                return showTitle;
            }
            set
            {
                showTitle = value;

                // Start event
                if (showTitle == true && lastShowTitle == false)
                    RaiseOnStartDisplayTitleScreenEvent();

                // End event
                if (showTitle == false && lastShowTitle == true)
                    RaiseOnEndDisplayTitleScreenEvent();

                lastShowTitle = showTitle;
            }
        }

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

        #region Event Handlers

        // OnStartDisplayTitleScreen
        public delegate void OnStartDisplayTitleScreenHandler();
        public static event OnStartDisplayTitleScreenHandler OnStartDisplayTitleScreen;
        protected virtual void RaiseOnStartDisplayTitleScreenEvent()
        {
            if (OnStartDisplayTitleScreen != null)
                OnStartDisplayTitleScreen();
        }

        // OnEndDisplayTitleScreen
        public delegate void OnEndDisplayTitleScreenHandler();
        public static event OnEndDisplayTitleScreenHandler OnEndDisplayTitleScreen;
        protected virtual void RaiseOnEndDisplayTitleScreenEvent()
        {
            if (OnEndDisplayTitleScreen != null)
                OnEndDisplayTitleScreen();
        }

        #endregion
    }
}