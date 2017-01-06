// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:
//
// Notes:
//

using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Weather;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Helper component to assign weather settings across multiple components.
    ///
    /// The logic for weather changes are based on the climate and weather table described in the Daggerfall Chronicles,
    /// pg. 47. Plus some additional logic to handle more natural weather transitions.
    ///
    /// TODO add smooth weather transitions
    /// </summary>
    public class WeatherManager : MonoBehaviour
    {
        public PlayerWeather PlayerWeather;
        public DaggerfallSky DaggerfallSky;
        public SunlightManager SunlightManager;
        public AmbientEffectsPlayer WeatherEffects;

        [Range(0, 1)]
        public float RainSunlightScale = 0.45f;
        [Range(0, 1)]
        public float StormSunlightScale = 0.25f;
        [Range(0, 1)]
        public float SnowSunlightScale = 0.45f;
        [Range(0, 1)]
        public float WinterSunlightScale = 0.65f;

        [Range(0, 1)]
        public float SunnyFogDensity = 0.00005f;
        [Range(0, 1)]
        public float OvercastFogDensity = 0.00005f;
        [Range(0, 1)]
        public float RainyFogDensity = 0.0001f;
        [Range(0, 1)]
        public float SnowyFogDensity = 0.001f;
        [Range(0, 1)]
        public float HeavyFogDensity = 0.05f;

        DaggerfallUnity _dfUnity;
        float _pollTimer;
        private WeatherTable _weatherTable;
        private float _pollWeatherInSeconds = 30f;

        public bool IsRaining { get; private set; }

        public bool IsStorming { get; private set; }

        public bool IsSnowing { get; private set; }

        public bool IsOvercast { get; private set; }

        void Awake()
        {
            SaveLoadManager.OnLoad += SaveLoadManager_OnLoad;
            StreamingWorld.OnInitWorld += StreamingWorld_OnInitWorld;
        }

        void Start()
        {
            _dfUnity = DaggerfallUnity.Instance;
            _weatherTable = WeatherTable.ParseJsonTable();
        }

        void Update()
        {
            SetAmbientEffects();
            PollWeatherChanges();
            SetSunlightScale();
        }

        public void ClearOvercast()
        {
            if (DaggerfallSky)
                DaggerfallSky.WeatherStyle = WeatherStyle.Normal;
            IsOvercast = false;
            SetFog(false, SunnyFogDensity);
        }

        public void ClearAllWeather()
        {
            StopRaining();
            StopSnowing();
            ClearOvercast();
        }

        #region Fog

        public void SetFog(bool isFoggy, float density)
        {
            RenderSettings.fog = isFoggy;

            if (isFoggy)
            {
                RenderSettings.fogDensity = density;
//                RenderSettings.fogMode = FogMode.Exponential;
//                RenderSettings.fogColor = Color.gray;
            }
            else
            {
                RenderSettings.fogDensity = SunnyFogDensity;
                // TODO set this based on ... something.
                // ex. time of day/angle of sun so we can get nice sunset/rise atmospheric effects
                // also blend with climate so climates end up having faint 'tints' to them
//                RenderSettings.fogColor = Color.clear;
            }
        }

        #endregion

        #region Rain

        public void SetRainOvercast()
        {
            if (DaggerfallSky)
            {
                if (Random.Range(0, 1) > 0.5f)
                    DaggerfallSky.WeatherStyle = WeatherStyle.Rain1;
                else
                    DaggerfallSky.WeatherStyle = WeatherStyle.Rain2;
            }
            SetFog(true, RainyFogDensity);
            IsOvercast = true;
        }

        public void StartRaining()
        {
            SetRainOvercast();
            IsRaining = true;
        }

        public void StartStorming()
        {
            StartRaining();
            IsStorming = true;
        }

        public void StopRaining()
        {
            IsRaining = false;
            IsStorming = false;
        }

        #endregion

        #region Snow

        public void SetSnowOvercast()
        {
            if (DaggerfallSky)
            {
                if (Random.Range(0, 1) > 0.5f)
                    DaggerfallSky.WeatherStyle = WeatherStyle.Snow1;
                else
                    DaggerfallSky.WeatherStyle = WeatherStyle.Snow2;
            }
            SetFog(true, SnowyFogDensity);
            IsOvercast = true;
        }

        public void StartSnowing()
        {
            SetSnowOvercast();
            IsSnowing = true;
        }

        public void StopSnowing()
        {
            IsSnowing = false;
        }

        #endregion

        #region Private Methods

        void SetSunlightScale()
        {
            // Start with default scale
            float scale = SunlightManager.defaultScaleFactor;

            // Apply winter
            if (_dfUnity.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
                scale = WinterSunlightScale;

            // Apply rain, storm, snow light scale
            if (IsRaining && !IsStorming)
                scale = RainSunlightScale;
            else if (IsRaining && IsStorming)
                scale = StormSunlightScale;
            else if (IsSnowing)
                scale = SnowSunlightScale;

            // Apply scale to sunlight manager
            if (SunlightManager)
                SunlightManager.ScaleFactor = scale;
        }

        void SetAmbientEffects()
        {
            if (!WeatherEffects)
                return;

            // Set presets based on weather type
            if (IsRaining && !IsStorming)
            {
                WeatherEffects.Presets = AmbientEffectsPlayer.AmbientSoundPresets.Rain;
                return;
            }
            else if (IsRaining && IsStorming)
            {
                WeatherEffects.Presets = AmbientEffectsPlayer.AmbientSoundPresets.Storm;
                return;
            }

            // Set presets based on time of day
            if (DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.IsDay)
            {
                WeatherEffects.Presets = AmbientEffectsPlayer.AmbientSoundPresets.SunnyDay;
                return;
            }
            else if (DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.IsNight)
            {
                WeatherEffects.Presets = AmbientEffectsPlayer.AmbientSoundPresets.ClearNight;
                return;
            }

            WeatherEffects.Presets = AmbientEffectsPlayer.AmbientSoundPresets.None;
        }

        void PollWeatherChanges()
        {
            // Increment poll timer
            _pollTimer += Time.deltaTime;
            if (_pollTimer < _pollWeatherInSeconds)
                return;

            // Reset timer
            _pollTimer = 0;

            var climate = PlayerWeather.PlayerGps.CurrentClimateIndex;
            var next = _weatherTable.GetWeather(climate, _dfUnity.WorldTime.Now.SeasonValue);
            if (next == PlayerWeather.WeatherType)
                return;

            // TODO smooth weather transitions. eg: storming to sunny is a jarring transition

            SetWeather(next);
        }

        public void SetWeather(WeatherType next)
        {
            Debug.Log("Next weather change: " + next);
            ClearAllWeather();
            switch (next)
            {
                case WeatherType.Cloudy:
                    // TODO make skybox cloudy
                    SetFog(true, SunnyFogDensity);
                    break;

                case WeatherType.Overcast:
                    SetRainOvercast();
                    break;

                case WeatherType.Fog:
                    SetRainOvercast();
                    SetFog(true, HeavyFogDensity);
                    break;

                case WeatherType.Rain:
                    StartRaining();
                    break;

                case WeatherType.Thunder:
                    StartStorming();
                    break;

                case WeatherType.Snow:
                    StartSnowing();
                    break;
            }
            RaiseOnWeatherChangeEvent(next);
            PlayerWeather.WeatherType = next;
        }

        #endregion

        #region Events Handlers

        void SaveLoadManager_OnLoad(SaveData_v1 saveData)
        {
            switch (saveData.playerData.playerPosition.weather)
            {
                case WeatherType.Rain:
                    StartRaining();
                    break;
                case WeatherType.Snow:
                    StartSnowing();
                    break;
            }
        }

        void StreamingWorld_OnInitWorld()
        {
            // Clear weather when starting up world
            ClearAllWeather();
        }

        #endregion

        #region Events

        public delegate void OnWeatherChangeHandler(WeatherType weather);
        public static event OnWeatherChangeHandler OnWeatherChange;

        static void RaiseOnWeatherChangeEvent(WeatherType weather)
        {
            if (OnWeatherChange != null)
                OnWeatherChange(weather);
        }

        #endregion
    }
}