// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;

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

        public void SetSunny()
        {
            StopRaining();
            StopSnowing();
            ClearOvercast();
        }

        public void ClearOvercast()
        {
            if (DaggerfallSky)
                DaggerfallSky.WeatherStyle = WeatherStyle.Normal;
            isOvercast = false;
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
        }

        public void StartStorming()
        {
            StartRaining();
            isStorming = true;
            SetAmbientEffects();
        }

        public void StopRaining()
        {
            isRaining = false;
            isStorming = false;

            if (PlayerWeather)
                PlayerWeather.WeatherType = PlayerWeather.WeatherTypes.None;

            SetSunlightScale();
            SetAmbientEffects();
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
        }

        public void StartSnowing()
        {
            StopRaining();
            SetSnowOvercast(true);
            isSnowing = true;
            SetSunlightScale();

            if (PlayerWeather)
                PlayerWeather.WeatherType = PlayerWeather.WeatherTypes.Snow_Normal;
        }

        public void StopSnowing()
        {
            isSnowing = false;

            if (PlayerWeather)
                PlayerWeather.WeatherType = PlayerWeather.WeatherTypes.None;

            SetSunlightScale();
            SetAmbientEffects();
        }

        #endregion

        #region Private Methods

        void SetSunlightScale()
        {
            // Start with default scale
            float scale = SunlightManager.defaultScaleFactor;

            // Apply winter
            if (dfUnity.WorldTime.SeasonValue == WorldTime.Seasons.Winter)
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
    }
}