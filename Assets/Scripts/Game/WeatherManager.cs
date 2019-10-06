// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Michael Rauter (Nystul)
//
// Notes:
//

using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Weather;
using DaggerfallWorkshop.Utility;
using UnityEngine;
using UnityEngine.PostProcessing;
using DaggerfallConnect.Arena2;

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
        public float OvercastSunlightScale = 0.65f;
        [Range(0, 1)]
        public float RainSunlightScale = 0.45f;
        [Range(0, 1)]
        public float StormSunlightScale = 0.25f;
        [Range(0, 1)]
        public float SnowSunlightScale = 0.45f;
        [Range(0, 1)]
        public float WinterSunlightScale = 0.65f;

        [Range(0, 1)]
        public float OvercastShadowStrength = 0.6f;
        [Range(0, 1)]
        public float RainShadowStrength = 0.4f;
        [Range(0, 1)]
        public float StormShadowStrength = 0.4f;
        [Range(0, 1)]
        public float SnowShadowStrength = 0.25f;
        [Range(0, 1)]
        public float WinterShadowStrength = 0.8f;

        [System.Serializable]
        public struct FogSettings
        {
            public FogMode fogMode;
            [Range(0, 1)]
            public float density;
            public float startDistance;
            public float endDistance;
            public bool excludeSkybox;
        }

        public FogSettings SunnyFogSettings = new FogSettings { fogMode = FogMode.Linear, density = 0.0f, startDistance = 0, endDistance = 2400, excludeSkybox = true };
        public FogSettings OvercastFogSettings = new FogSettings { fogMode = FogMode.Linear, density = 0.0f, startDistance = 0, endDistance = 2400, excludeSkybox = true };
        public FogSettings RainyFogSettings = new FogSettings { fogMode = FogMode.Exponential, density = 0.003f, startDistance = 0, endDistance = 0, excludeSkybox = true };
        public FogSettings SnowyFogSettings = new FogSettings { fogMode = FogMode.Exponential, density = 0.005f, startDistance = 0, endDistance = 0, excludeSkybox = true };
        public FogSettings HeavyFogSettings = new FogSettings { fogMode = FogMode.Exponential, density = 0.05f, startDistance = 0, endDistance = 0, excludeSkybox = false };

        public FogSettings InteriorFogSettings = new FogSettings { fogMode = FogMode.Exponential, density = 0.001f, startDistance = 0, endDistance = 0, excludeSkybox = false };
        public FogSettings DungeonFogSettings = new FogSettings { fogMode = FogMode.Exponential, density = 0.005f, startDistance = 0, endDistance = 0, excludeSkybox = false };

        public FogSettings currentOutdoorFogSettings;
        Color previousOutdoorFogColor;

        // this is needed so weather from savegame load is not overwritten by code in StreamingWorld_OnInitWorld()
        // e.g. weather fog, going to interior, saving, restarting, loading save, going outdoors -> fog should still be present (without this workaround it is not)
        bool startedFromLoadedSaveGame; // needed to correctly restore fog after loading a savegame since StreamingWorld_OnInitWorld() happens after load event overwritting the values set there

        DaggerfallUnity _dfUnity;
        float _pollTimer;
        private WeatherTable _weatherTable;
        private float _pollWeatherInSeconds = 30f;

        // used to set post processing fog settings (excludeSkybox setting)
        private PostProcessingBehaviour postProcessingBehaviour;

        public bool IsRaining { get; private set; }

        public bool IsStorming { get; private set; }

        public bool IsSnowing { get; private set; }

        public bool IsOvercast { get; private set; }

        bool updateWeatherFromClimateArray = false;

        public bool UpdateWeatherFromClimateArray { get { return updateWeatherFromClimateArray; } set { updateWeatherFromClimateArray = value; } }

        void Awake()
        {
            StreamingWorld.OnInitWorld += StreamingWorld_OnInitWorld;
            SaveLoadManager.OnLoad += SaveLoadManager_OnLoad;            
            PlayerEnterExit.OnTransitionInterior += OnTransitionToInterior;
            PlayerEnterExit.OnTransitionExterior += OnTransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonInterior += OnTransitionToDungeon;
            PlayerEnterExit.OnTransitionDungeonExterior += OnTransitionToExterior;
        }

        void OnDestroy()
        {
            StreamingWorld.OnInitWorld -= StreamingWorld_OnInitWorld;
            SaveLoadManager.OnLoad -= SaveLoadManager_OnLoad;
            PlayerEnterExit.OnTransitionInterior -= OnTransitionToInterior;
            PlayerEnterExit.OnTransitionExterior -= OnTransitionToExterior;
            PlayerEnterExit.OnTransitionDungeonInterior -= OnTransitionToDungeon;
            PlayerEnterExit.OnTransitionDungeonExterior -= OnTransitionToExterior;
        }

        void Start()
        {
            _dfUnity = DaggerfallUnity.Instance;
            _weatherTable = WeatherTable.ParseJsonTable();

            postProcessingBehaviour = Camera.main.GetComponent<PostProcessingBehaviour>();
            if (postProcessingBehaviour != null)
            {
                var fogSettings = postProcessingBehaviour.profile.fog.settings;
                fogSettings.excludeSkybox = true;
                postProcessingBehaviour.profile.fog.settings = fogSettings;              
            }
            
            if (DaggerfallUnity.Settings.AssetInjection)
                AddWindZone();

            // initialize with clear overcast sky (so that initial fog settings like exponential fog mode are set)
            ClearOvercast();
        }

        void Update()
        {
            // Do nothing if player inside
            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
                return;

            SetAmbientEffects();
            //PollWeatherChanges();
            SetSunlightScale();
            UpdateFromClimateArrayCheck();
        }

        public void ClearOvercast()
        {
            if (DaggerfallSky)
                DaggerfallSky.WeatherStyle = WeatherStyle.Normal;
            IsOvercast = false;
            SetFog(SunnyFogSettings);
        }

        public void ClearAllWeather()
        {
            StopRaining();
            StopSnowing();
            ClearOvercast();
        }

        #region Fog

        public void SetFog(FogSettings fogSettings, bool isInteriorOrDungeonFog = false)
        {
            // if fog is a outdoor weather fog set currentOutdoorFogSettings so fog settings can be restored when player goes into an interior/dungeon and then back to exterior
            if (isInteriorOrDungeonFog == false)
            {
                currentOutdoorFogSettings = fogSettings;
            }
            else // if in interior or dungeon
            {
                // set fog color to black
                RenderSettings.fogColor = Color.black;
            }

            // set fog mode first
            RenderSettings.fogMode = fogSettings.fogMode;
            RenderSettings.fogDensity = fogSettings.density;
            RenderSettings.fogStartDistance = fogSettings.startDistance;
            RenderSettings.fogEndDistance = fogSettings.endDistance;

            // edit by Nystul: don't disable RenderSettings fog! Rendering Fog != Weather Fog
            //RenderSettings.fog = isFoggy;

            if (fogSettings.excludeSkybox == false)
            {
                //                RenderSettings.fogColor = Color.gray;

                if (postProcessingBehaviour != null)
                {
                    var fogPostProcess = postProcessingBehaviour.profile.fog.settings;
                    fogPostProcess.excludeSkybox = false;
                    postProcessingBehaviour.profile.fog.settings = fogPostProcess;
                }
            }
            else
            {                
                // TODO set this based on ... something.
                // ex. time of day/angle of sun so we can get nice sunset/rise atmospheric effects
                // also blend with climate so climates end up having faint 'tints' to them
                //                RenderSettings.fogColor = Color.clear;

                if (postProcessingBehaviour != null)
                {
                    var fogPostProcess = postProcessingBehaviour.profile.fog.settings;
                    fogPostProcess.excludeSkybox = true;
                    postProcessingBehaviour.profile.fog.settings = fogPostProcess;
                }
            }
        }

        #endregion

        #region Rain

        public void SetOvercast()
        {
            SetFog(OvercastFogSettings);
            IsOvercast = true;
        }

        public void SetRainOvercast()
        {
            if (DaggerfallSky)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                    DaggerfallSky.WeatherStyle = WeatherStyle.Rain1;
                else
                    DaggerfallSky.WeatherStyle = WeatherStyle.Rain2;
            }
            SetFog(RainyFogSettings);
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
                if (Random.Range(0f, 1f) > 0.5f)
                    DaggerfallSky.WeatherStyle = WeatherStyle.Snow1;
                else
                    DaggerfallSky.WeatherStyle = WeatherStyle.Snow2;
            }
            SetFog(SnowyFogSettings);
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
            float shadowStrength = SunlightManager.defaultShadowStrength;

            // Apply winter
            if (_dfUnity.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter)
            {
                scale = WinterSunlightScale;
                shadowStrength = WinterShadowStrength;
            }

            // Apply rain, storm, snow light scale
            if (IsRaining && !IsStorming)
            {
                scale = RainSunlightScale;
                shadowStrength = RainShadowStrength;
            }
            else if (IsRaining && IsStorming)
            {
                scale = StormSunlightScale;
                shadowStrength = StormShadowStrength;
            }
            else if (IsSnowing)
            {
                scale = SnowSunlightScale;
                shadowStrength = SnowShadowStrength;
            }
            else if (IsOvercast)
            {
                scale = OvercastSunlightScale;
                shadowStrength = OvercastShadowStrength;
            }

            shadowStrength *= Mathf.Exp(-50f * currentOutdoorFogSettings.density);

            // Apply scale to sunlight manager
            if (SunlightManager)
            {
                SunlightManager.ScaleFactor = scale;
                SunlightManager.ShadowStrength = shadowStrength;
            }
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

        void UpdateFromClimateArrayCheck()
        {
            if (updateWeatherFromClimateArray)
            {
                if (PlayerWeather.PlayerGps.ReadyCheck())
                {
                    SetWeatherFromWeatherClimateArray();
                    updateWeatherFromClimateArray = false;
                }
            }
        }

        // Sets weathers for each of the climate zones, like classic
        public void SetClimateWeathers()
        {
            PlayerWeather.ClimateWeathers[0] = (byte)_weatherTable.GetWeather((int)MapsFile.Climates.Desert, _dfUnity.WorldTime.Now.SeasonValue);
            PlayerWeather.ClimateWeathers[1] = (byte)_weatherTable.GetWeather((int)MapsFile.Climates.Mountain, _dfUnity.WorldTime.Now.SeasonValue);
            PlayerWeather.ClimateWeathers[2] = (byte)_weatherTable.GetWeather((int)MapsFile.Climates.Rainforest, _dfUnity.WorldTime.Now.SeasonValue);
            PlayerWeather.ClimateWeathers[3] = (byte)_weatherTable.GetWeather((int)MapsFile.Climates.Swamp, _dfUnity.WorldTime.Now.SeasonValue);
            PlayerWeather.ClimateWeathers[4] = (byte)_weatherTable.GetWeather((int)MapsFile.Climates.Subtropical, _dfUnity.WorldTime.Now.SeasonValue);
            PlayerWeather.ClimateWeathers[5] = (byte)_weatherTable.GetWeather((int)MapsFile.Climates.Woodlands, _dfUnity.WorldTime.Now.SeasonValue);
        }

        public void SetWeatherFromWeatherClimateArray()
        {
            int climate = PlayerWeather.PlayerGps.CurrentClimateIndex;
            int index = Utility.TravelTimeCalculator.climateIndices[climate - (int)MapsFile.Climates.Ocean];

            WeatherType weather = (WeatherType)PlayerWeather.ClimateWeathers[index];

            if (weather == PlayerWeather.WeatherType)
                return;

            SetWeather(weather);
        }

        public void SetWeather(WeatherType next)
        {
            Debug.Log("Next weather change: " + next);
            ClearAllWeather();
            switch (next)
            {
                case WeatherType.Cloudy:
                    // TODO make skybox cloudy
                    SetFog(SunnyFogSettings);
                    break;

                case WeatherType.Overcast:
                    SetOvercast();
                    break;

                case WeatherType.Fog:
                    SetRainOvercast();
                    SetFog(HeavyFogSettings);
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

        void AddWindZone()
        {
            // Add a wind zone for trees and particles.
            var windZone = gameObject.AddComponent<WindZone>();
            windZone.mode = WindZoneMode.Directional;
            windZone.windMain = 0.2f;
            windZone.windTurbulence = 0.1f;
            windZone.windPulseMagnitude = 1;
            windZone.windPulseFrequency = 0.25f;
        }

        #endregion

        #region Events Handlers

        void SaveLoadManager_OnLoad(SaveData_v1 saveData)
        {
            // first restore general outdoor weather (which sets fog)
            SetWeather(saveData.playerData.playerPosition.weather);

            // then check if player is inside and set fog accordingly
            if (GameManager.Instance.IsPlayerInsideBuilding)
            {
                SetFog(InteriorFogSettings, true);
            }
            else if (GameManager.Instance.IsPlayerInsideDungeon || GameManager.Instance.IsPlayerInsideCastle)
            {
                SetFog(DungeonFogSettings, true);
            }

            startedFromLoadedSaveGame = true; // needed so StreamingWorld_OnInitWorld() does not break stuff again (see details in comment at variable definition of startedFromLoadedSaveGame)
        }

        void StreamingWorld_OnInitWorld()
        {
            // check if we did not just load a savegame (see details in comment at variable definition of startedFromLoadedSaveGame)            
            if (startedFromLoadedSaveGame == false)
            {
                // Clear weather when starting up world
                ClearAllWeather();

                // Set weather in case we have loaded from a classic save.
                // Currently loading a Unity save will replace this with its own weather value after this.
                updateWeatherFromClimateArray = true;

                startedFromLoadedSaveGame = false;
            }
            else
            {
                // important so no weather update from climate array happens in case of loaded savegame 
                updateWeatherFromClimateArray = false;
            }
        }

        void OnTransitionToInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            previousOutdoorFogColor = RenderSettings.fogColor;
            SetFog(InteriorFogSettings, true);
        }

        void OnTransitionToDungeon(PlayerEnterExit.TransitionEventArgs args)
        {
            previousOutdoorFogColor = RenderSettings.fogColor;
            SetFog(DungeonFogSettings, true);
        }

        void OnTransitionToExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            SetFog(currentOutdoorFogSettings, false);
            RenderSettings.fogColor = previousOutdoorFogColor;
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