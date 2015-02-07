// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Demo;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Script to play weather effects over player.
    /// Allows weather to be attached elsewhere than a child of player (such as exterior parent).
    /// In multiplayer do not sync weather particle effects to other players.
    /// </summary>
    [RequireComponent(typeof(PlayerGPS))]
    [RequireComponent(typeof(PlayerEnterExit))]
    public class PlayerWeather : MonoBehaviour
    {
        public GameObject RainParticles;
        public GameObject SnowParticles;
        public WeatherTypes WeatherType = WeatherTypes.None;

        PlayerGPS playerGPS;
        PlayerEnterExit playerEnterExit;
        WeatherTypes currentWeatherType = WeatherTypes.None;
        DFLocation.ClimateBaseType currentClimateType = DFLocation.ClimateBaseType.None;
        bool isInside = false;

        public enum WeatherTypes
        {
            None,
            Rain_Normal,
            Snow_Normal,
        }

        void Start()
        {
            playerGPS = GetComponent<PlayerGPS>();
            playerEnterExit = GetComponent<PlayerEnterExit>();
            if (RainParticles) RainParticles.SetActive(false);
            if (SnowParticles) SnowParticles.SetActive(false);
        }

        void Update()
        {
            // Update weather if context changes
            if (WeatherType != currentWeatherType ||
                playerEnterExit.IsPlayerInside != isInside ||
                playerGPS.ClimateSettings.ClimateType != currentClimateType)
            {
                isInside = playerEnterExit.IsPlayerInside;
                currentClimateType = playerGPS.ClimateSettings.ClimateType;
                currentWeatherType = WeatherType;
                SetWeather();
            }
        }

        void SetWeather()
        {
            // Always disable weather inside
            if (isInside)
            {
                if (RainParticles) RainParticles.SetActive(false);
                if (SnowParticles) SnowParticles.SetActive(false);
                return;
            }

            // Always snow in desert climate
            if (currentClimateType == DFLocation.ClimateBaseType.Desert &&
                currentWeatherType == WeatherTypes.Snow_Normal)
            {
                currentWeatherType = WeatherTypes.None;
                WeatherType = WeatherTypes.None;
            }

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
        }
    }
}