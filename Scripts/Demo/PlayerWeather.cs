// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Demo;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Script to play weather effects over player.
    /// Allows weather to be attached elsewhere than a child of player (such as exterior parent).
    /// In multiplayer do not sync weather particle effects to other players.
    /// </summary>
    public class PlayerWeather : MonoBehaviour
    {
        public GameObject RainParticles;
        public GameObject SnowParticles;
        public WeatherTypes WeatherType = WeatherTypes.None;

        WeatherTypes lastWeatherType = WeatherTypes.None;

        public enum WeatherTypes
        {
            None,
            Rain_Normal,
            Snow_Normal,
        }

        void Start()
        {
            if (RainParticles) RainParticles.SetActive(false);
            if (SnowParticles) SnowParticles.SetActive(false);
        }

        void Update()
        {
            if (WeatherType != lastWeatherType)
                SetWeather();
        }

        void SetWeather()
        {
            switch (WeatherType)
            {
                case WeatherTypes.None:
                    if (RainParticles) RainParticles.SetActive(false);
                    if (SnowParticles) SnowParticles.SetActive(false);
                    break;
                case WeatherTypes.Rain_Normal:
                    if (RainParticles) RainParticles.SetActive(true);
                    if (SnowParticles) SnowParticles.SetActive(false);
                    break;
                case WeatherTypes.Snow_Normal:
                    if (RainParticles) RainParticles.SetActive(false);
                    if (SnowParticles) SnowParticles.SetActive(true);
                    break;
            }
            lastWeatherType = WeatherType;
        }
    }
}