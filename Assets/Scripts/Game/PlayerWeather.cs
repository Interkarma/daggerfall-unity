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
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Weather;

namespace DaggerfallWorkshop.Game
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
        public WeatherType WeatherType = WeatherType.Sunny;
        public PlayerGPS PlayerGps { get; private set; }
        public byte[] ClimateWeathers;

        PlayerEnterExit playerEnterExit;
        WeatherType _currentWeatherType = WeatherType.Sunny;
        DFLocation.ClimateBaseType currentClimateType = DFLocation.ClimateBaseType.None;
        bool isInside = false;

        void Start()
        {
            PlayerGps = GetComponent<PlayerGPS>();
            playerEnterExit = GetComponent<PlayerEnterExit>();
            if (RainParticles) RainParticles.SetActive(false);
            if (SnowParticles) SnowParticles.SetActive(false);
            ClimateWeathers = new byte[6];
        }

        void Update()
        {
            // Update weather if context changes
            if (WeatherType != _currentWeatherType ||
                playerEnterExit.IsPlayerInside != isInside ||
                PlayerGps.ClimateSettings.ClimateType != currentClimateType)
            {
                isInside = playerEnterExit.IsPlayerInside;
                currentClimateType = PlayerGps.ClimateSettings.ClimateType;
                _currentWeatherType = WeatherType;
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

            switch (WeatherType)
            {
                case WeatherType.Rain:
                case WeatherType.Thunder:
                    if (RainParticles) RainParticles.SetActive(true);
                    if (SnowParticles) SnowParticles.SetActive(false);
                    break;
                case WeatherType.Snow:
                    if (RainParticles) RainParticles.SetActive(false);
                    if (SnowParticles) SnowParticles.SetActive(true);
                    break;
                default:
                    if (RainParticles) RainParticles.SetActive(false);
                    if (SnowParticles) SnowParticles.SetActive(false);
                    break;
            }
        }
    }
}