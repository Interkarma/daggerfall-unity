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
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Handles fading HUD during scene transitions.
    /// </summary>
    public class FadeBehaviour : MonoBehaviour
    {
        bool allowFade = true;
        bool fadeInProgress;
        Panel fadeTargetPanel;
        float fadeTimer;
        float fadeTotalTime;
        float fadeDuration;
        Color fadeStartColor;
        Color fadeEndColor;
        //Color fadeCurrentColor;

        public bool AllowFade
        {
            get { return allowFade; }
            set { allowFade = value; }
        }

        public Panel FadeTargetPanel
        {
            get { return fadeTargetPanel; }
            set { fadeTargetPanel = value; }
        }

        public bool FadeInProgress
        {
            get { return fadeInProgress; }
        }

        private void OnGUI()
        {
            TickFade();
        }

        public void SmashHUDToBlack()
        {
            if (fadeTargetPanel == null || !allowFade)
                return;

            fadeTargetPanel.BackgroundColor = Color.black;
        }

        public void FadeHUDToBlack(float fadeDuration = 0.5f)
        {
            if (fadeTargetPanel == null || !allowFade)
                return;

            fadeStartColor = Color.clear;
            fadeEndColor = Color.black;
            this.fadeDuration = fadeDuration;
            fadeTargetPanel.BackgroundColor = Color.clear;
            fadeInProgress = true;
        }

        public void FadeHUDFromBlack(float fadeDuration = 0.5f)
        {
            if (fadeTargetPanel == null || !allowFade)
                return;

            fadeStartColor = Color.black;
            fadeEndColor = Color.clear;
            this.fadeDuration = fadeDuration;
            fadeTargetPanel.BackgroundColor = Color.black;
            fadeInProgress = true;
        }

        public void ClearFade()
        {
            if (fadeTargetPanel == null || !allowFade)
                return;

            fadeTargetPanel.BackgroundColor = Color.clear;
            fadeTimer = 0;
            fadeTotalTime = 0;
            fadeInProgress = false;
        }

        void TickFade()
        {
            const float fadeStep = 0.02f;

            if (fadeTargetPanel == null || !fadeInProgress || !allowFade)
                return;

            // Change fade setting
            fadeTimer += Time.deltaTime;
            if (fadeTimer > fadeStep)
            {
                fadeTotalTime += fadeStep;
                float progress = fadeTotalTime / fadeDuration;
                fadeTargetPanel.BackgroundColor = Color.Lerp(fadeStartColor, fadeEndColor, progress);
                fadeTimer = 0;
            }

            // Handle fade completion
            if (fadeTotalTime > fadeDuration)
            {
                fadeTargetPanel.BackgroundColor = fadeEndColor;
                fadeTimer = 0;
                fadeTotalTime = 0;
                fadeInProgress = false;
            }
        }
    }
}