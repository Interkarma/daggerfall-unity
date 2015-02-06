// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Peer with PlayerGPS to display text when player enters a new location rect.
    /// </summary>
    [RequireComponent(typeof(PlayerGPS))]
    public class NewLocationAlert : MonoBehaviour
    {
        const float fadeDuration = 1;
        const float fadeTextWaitTime = 5;

        public Text LocationText;

        PlayerGPS playerGPS;
        bool isInLocationRect = false;
        bool fadingText = false;
        bool showText = false;
        float fadeTextTimer = 0;

        int lastRegionIndex = -1;
        int lastLocationIndex = -1;
        
        void Start()
        {
            playerGPS = GetComponent<PlayerGPS>();
            ResetText();
        }

        void Update()
        {
            // Reset text if we have changed region/location
            if (playerGPS.CurrentLocation.RegionIndex != lastRegionIndex ||
                playerGPS.CurrentLocation.LocationIndex != lastLocationIndex)
            {
                ResetText();
                lastRegionIndex = playerGPS.CurrentLocation.RegionIndex;
                lastLocationIndex = playerGPS.CurrentLocation.LocationIndex;
            }

            // Wait while fading text
            if (fadingText)
            {
                fadeTextTimer += Time.deltaTime;
                if (fadeTextTimer > fadeTextWaitTime)
                {
                    fadingText = false;
                    fadeTextTimer = 0;
                    if (showText)
                        HideText();
                }
                return;
            }

            // Do nothing unless player location status has changed
            if (playerGPS.IsPlayerInLocationRect != isInLocationRect)
            {
                isInLocationRect = playerGPS.IsPlayerInLocationRect;
                if (isInLocationRect)
                {
                    // Show text
                    if (LocationText)
                    {
                        string text = string.Format("{0}{1}({2})", playerGPS.CurrentLocation.Name, Environment.NewLine, playerGPS.CurrentLocation.RegionName);
                        ShowText(text);
                    }
                }
            }
        }

        void ShowText(string text)
        {
            if (LocationText)
            {
                LocationText.text = text;
                LocationText.CrossFadeAlpha(1, fadeDuration, true);
                fadingText = true;
                showText = true;
                fadeTextTimer = 0;
            }
        }

        void HideText()
        {
            if (LocationText)
            {
                LocationText.CrossFadeAlpha(0, fadeDuration, true);
                fadingText = true;
                showText = false;
                fadeTextTimer = 0;
            }
        }

        void ResetText()
        {
            if (LocationText)
            {
                LocationText.CrossFadeAlpha(0, 0, true);
                isInLocationRect = false;
                fadingText = false;
                showText = false;
                fadeTextTimer = 0;
            }
        }
    }
}