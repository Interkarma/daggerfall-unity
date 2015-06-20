// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Demo
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

        DaggerfallUnity dfUnity;
        bool isRaining = false;
        bool isStorming = false;
        bool isSnowing = false;
        bool isOvercast = false;

        public bool IsRaining
        {
            get { return isRaining; }
        }

        public bool IsSnowing
        {
            get { return isSnowing; }
        }

        public bool IsOvercast
        {
            get { return isOvercast; }
        }
        
        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
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
            SetSunlightScale();
            RaiseOnSetRainOvercastEvent();
        }

        public void StartRaining()
        {
            StopSnowing();
            SetRainOvercast(true);
            isRaining = true;
            
            if (PlayerWeather)
                PlayerWeather.WeatherType = PlayerWeather.WeatherTypes.Rain_Normal;

            SetSunlightScale();
            SetAmbientEffects();
            RaiseOnStartRainingEvent();
        }

        public void StartStorming()
        {
            StartRaining();
            isStorming = true;
            SetAmbientEffects();
            RaiseOnStartStormingEvent();
        }

        public void StopRaining()
        {
            isRaining = false;
            isStorming = false;

            if (PlayerWeather)
                PlayerWeather.WeatherType = PlayerWeather.WeatherTypes.None;

            SetSunlightScale();
            SetAmbientEffects();
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
            SetSunlightScale();
            RaiseOnSetSnowOvercastEvent();
        }

        public void StartSnowing()
        {
            StopRaining();
            SetSnowOvercast(true);
            isSnowing = true;
            SetSunlightScale();

            if (PlayerWeather)
                PlayerWeather.WeatherType = PlayerWeather.WeatherTypes.Snow_Normal;

            RaiseOnStartSnowingEvent();
        }

        public void StopSnowing()
        {
            isSnowing = false;

            if (PlayerWeather)
                PlayerWeather.WeatherType = PlayerWeather.WeatherTypes.None;

            SetSunlightScale();
            SetAmbientEffects();
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
                WeatherEffects.Presets = AmbientEffectsPlayer.AmbientSoundPresets.Rain;
            else if (isRaining && isStorming)
                WeatherEffects.Presets = AmbientEffectsPlayer.AmbientSoundPresets.Storm;
            else
                WeatherEffects.Presets = AmbientEffectsPlayer.AmbientSoundPresets.None;
        }

        #endregion

        #region Event Handlers

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