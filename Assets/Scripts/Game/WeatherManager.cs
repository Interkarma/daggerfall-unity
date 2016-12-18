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

using UnityEngine;
using DaggerfallWorkshop.Utility;
using Random = UnityEngine.Random;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Helper component to assign weather settings across multiple components.
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

        public float PollWeatherInSeconds = 4;

        [Range(0, 1)]
        public float ChanceToRainOvercast = 0.03f;
        [Range(0, 1)]
        public float ChanceToStartRaining = 0.04f;
        [Range(0, 1)]
        public float ChanceToStartStorming = 0.1f;

        [Range(0, 1)]
        public float ChanceToSnowOvercast = 0.03f;
        [Range(0, 1)]
        public float ChanceToStartSnowing = 0.1f;

        DaggerfallUnity dfUnity;
        bool isRaining;
        bool isStorming;
        bool isSnowing;
        bool isOvercast;
        float pollTimer;

        public bool IsRaining
        {
            get { return isRaining; }
        }

        public bool IsStorming
        {
            get { return isStorming; }
        }

        public bool IsSnowing
        {
            get { return isSnowing; }
        }

        public bool IsOvercast
        {
            get { return isOvercast; }
        }

        void Awake()
        {
            Serialization.SaveLoadManager.OnLoad += SaveLoadManager_OnLoad;
            StreamingWorld.OnInitWorld += StreamingWorld_OnInitWorld;
        }

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
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
            isOvercast = false;
            RaiseOnClearOvercastEvent();
        }

        public void ClearAllWeather()
        {
            StopRaining();
            StopSnowing();
            ClearOvercast();
        }

        #region Rain

        public void SetRainOvercast(bool rainOvercast)
        {
            if (DaggerfallSky && rainOvercast)
            {
                if (Random.Range(0, 1) > 0.5f)
                    DaggerfallSky.WeatherStyle = WeatherStyle.Rain1;
                else
                    DaggerfallSky.WeatherStyle = WeatherStyle.Rain2;
            }
            else if (DaggerfallSky && !rainOvercast)
            {
                DaggerfallSky.WeatherStyle = WeatherStyle.Normal;
            }

            isOvercast = true;
            RaiseOnSetRainOvercastEvent();
        }

        public void StartRaining()
        {
            StopSnowing();
            SetRainOvercast(true);
            isRaining = true;
            
            if (PlayerWeather)
                PlayerWeather.WeatherType = PlayerWeather.WeatherTypes.Rain_Normal;

            RaiseOnStartRainingEvent();
        }

        public void StartStorming()
        {
            StartRaining();
            isStorming = true;
            RaiseOnStartStormingEvent();
        }

        public void StopRaining()
        {
            isRaining = false;
            isStorming = false;

            if (PlayerWeather)
                PlayerWeather.WeatherType = PlayerWeather.WeatherTypes.None;

            RaiseOnStopRainingEvent();
        }

        #endregion

        #region Snow

        public void SetSnowOvercast(bool snowOvercast)
        {
            if (DaggerfallSky && snowOvercast)
            {
                if (Random.Range(0, 1) > 0.5f)
                    DaggerfallSky.WeatherStyle = WeatherStyle.Snow1;
                else
                    DaggerfallSky.WeatherStyle = WeatherStyle.Snow2;
            }
            else if (DaggerfallSky && !snowOvercast)
            {
                DaggerfallSky.WeatherStyle = WeatherStyle.Normal;
            }

            isOvercast = true;
            RaiseOnSetSnowOvercastEvent();
        }

        public void StartSnowing()
        {
            StopRaining();
            SetSnowOvercast(true);
            isSnowing = true;

            if (PlayerWeather)
                PlayerWeather.WeatherType = PlayerWeather.WeatherTypes.Snow_Normal;

            RaiseOnStartSnowingEvent();
        }

        public void StopSnowing()
        {
            isSnowing = false;

            if (PlayerWeather)
                PlayerWeather.WeatherType = PlayerWeather.WeatherTypes.None;

            RaiseOnStopSnowingEvent();
        }

        #endregion

        #region Private Methods

        void SetSunlightScale()
        {
            // Start with default scale
            float scale = SunlightManager.defaultScaleFactor;

            // Apply winter
            if (dfUnity.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
                scale = WinterSunlightScale;

            // Apply rain, storm, snow light scale
            if (isRaining && !isStorming)
                scale = RainSunlightScale;
            else if (isRaining && isStorming)
                scale = StormSunlightScale;
            else if (isSnowing)
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
            if (isRaining && !isStorming)
            {
                WeatherEffects.Presets = AmbientEffectsPlayer.AmbientSoundPresets.Rain;
                return;
            }
            else if (isRaining && isStorming)
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
            pollTimer += Time.deltaTime;
            if (pollTimer < PollWeatherInSeconds)
                return;

            // Reset timer
            pollTimer = 0;

            // Very simple weather ramp based on season - will improve later
            if (dfUnity.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Spring ||
                dfUnity.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Summer)
            {
                // Start overcast
                if (!IsOvercast && !IsRaining && !IsStorming)
                {
                    if (Random.value < ChanceToRainOvercast)
                    {
                        SetRainOvercast(true);
                        return;
                    }
                }

                // Progress to rain
                if (IsOvercast && !IsRaining && !IsStorming)
                {
                    if (Random.value < ChanceToStartRaining)
                    {
                        StartRaining();
                        return;
                    }
                }

                // Progress to storm
                if (IsRaining)
                {
                    if (Random.value < ChanceToStartStorming)
                    {
                        StartStorming();
                        return;
                    }
                }
            }
            else
            {
                // Start overcast
                if (!IsOvercast && !IsSnowing)
                {
                    if (Random.value < ChanceToSnowOvercast)
                        SetSnowOvercast(true);
                }

                // Progress to snow
                if (IsOvercast && !IsSnowing)
                {
                    if (Random.value < ChanceToStartSnowing)
                        StartSnowing();
                }
            }
        }

        #endregion

        #region Events Handlers

        void SaveLoadManager_OnLoad(Serialization.SaveData_v2 saveData)
        {
            switch (saveData.playerData.playerPosition.weather)
            {
                case PlayerWeather.WeatherTypes.Rain_Normal:
                    StartRaining();
                    break;
                case PlayerWeather.WeatherTypes.Snow_Normal:
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

        // OnSetRainOvercast
        public delegate void OnSetRainOvercastEventHandler();
        public static event OnSetRainOvercastEventHandler OnSetRainOvercast;
        protected virtual void RaiseOnSetRainOvercastEvent()
        {
            if (OnSetRainOvercast != null)
                OnSetRainOvercast();
        }

        // OnSetSnowOvercast
        public delegate void OnSetSnowOvercastEventHandler();
        public static event OnSetSnowOvercastEventHandler OnSetSnowOvercast;
        protected virtual void RaiseOnSetSnowOvercastEvent()
        {
            if (OnSetSnowOvercast != null)
                OnSetSnowOvercast();
        }

        // OnClearOvercast
        public delegate void OnClearOvercastEventHandler();
        public static event OnClearOvercastEventHandler OnClearOvercast;
        protected virtual void RaiseOnClearOvercastEvent()
        {
            if (OnClearOvercast != null)
                OnClearOvercast();
        }

        // OnStartRaining
        public delegate void OnStartRainingEventHandler();
        public static event OnStartRainingEventHandler OnStartRaining;
        protected virtual void RaiseOnStartRainingEvent()
        {
            if (OnStartRaining != null)
                OnStartRaining();
        }

        // OnStartStorming
        public delegate void OnStartStormingEventHandler();
        public static event OnStartStormingEventHandler OnStartStorming;
        protected virtual void RaiseOnStartStormingEvent()
        {
            if (OnStartStorming != null)
                OnStartStorming();
        }

        // OnStopRaining
        public delegate void OnStopRainingEventHandler();
        public static event OnStopRainingEventHandler OnStopRaining;
        protected virtual void RaiseOnStopRainingEvent()
        {
            if (OnStopRaining != null)
                OnStopRaining();
        }

        // OnStartSnowing
        public delegate void OnStartSnowingEventHandler();
        public static event OnStartSnowingEventHandler OnStartSnowing;
        protected virtual void RaiseOnStartSnowingEvent()
        {
            if (OnStartSnowing != null)
                OnStartSnowing();
        }

        // OnStopSnowing
        public delegate void OnStopSnowingEventHandler();
        public static event OnStopSnowingEventHandler OnStopSnowing;
        protected virtual void RaiseOnStopSnowingEvent()
        {
            if (OnStopSnowing != null)
                OnStopSnowing();
        }

        #endregion
    }
}