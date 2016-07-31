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
using System.Collections;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Some basic methods for dfworkshop.net demos. Should be attached to Player game object.
    /// Input is hardcoded so project settings do not need to be distributed with library.
    /// Should use normal Unity Input setup in a real project.
    /// </summary>
    public class ExplorerMode : MonoBehaviour
    {
        const float hiRunSpeedValue = 240f;
        const float hiTorchRangeValue = 25f;

        public SongManager SongManager;
        public WeatherManager WeatherManager;
        public Light PlayerTorch;
        public WeaponManager WeaponManager;

        DaggerfallUnity dfUnity;
        StreamingWorld streamingWorld;
        PlayerEnterExit playerEnterExit;
        PlayerGPS playerGPS;
        //PlayerMotor playerMotor;
        //PlayerMouseLook playerMouseLook;
        //ShowTitleScreen titleScreen;

        //int timeScaleControl = 1;
        //int minTimeScaleControl = 1;
        //int maxTimeScaleControl = 150;
        //int timeScaleStep = 25;
        //float timeScaleMultiplier = 10f;
        //float startRunSpeed;
        //float startTorchRange;
        //bool showDebugStrings = false;
        //bool invertMouse = false;
        //bool hiRunSpeed = false;
        //bool hiTorchRange = false;

        void Start()
        {
            dfUnity = DaggerfallUnity.Instance;
            streamingWorld = GameObject.FindObjectOfType<StreamingWorld>();
            playerEnterExit = GetComponent<PlayerEnterExit>();
            playerGPS = GetComponent<PlayerGPS>();
            //playerMotor = GetComponent<PlayerMotor>();
            //playerMouseLook = GameObject.FindObjectOfType<PlayerMouseLook>();
            //titleScreen = GameObject.FindObjectOfType<ShowTitleScreen>();

            //// Get starting run speed
            //if (playerMotor)
            //    startRunSpeed = playerMotor.runSpeed;

            //// Get starting torch range
            //if (PlayerTorch != null)
            //    startTorchRange = PlayerTorch.range;
        }

        void Update()
        {

            if (dfUnity) dfUnity.WorldTime.ShowDebugString = true;
            if (streamingWorld) streamingWorld.ShowDebugString = true;

            if (streamingWorld.IsInit)
            {
                // Exit while loading world
                return;
            }
            else
            {
                //// Stop showing title screen
                //if (titleScreen)
                //    titleScreen.ShowTitle = false;
            }

            //// Random location
            //// Must have playerEnterExit reference
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    if (!playerEnterExit)
            //        return;

            //    // Sheate weapons
            //    if (WeaponManager)
            //    {
            //        WeaponManager.SheathWeapons();
            //    }

            //    if (playerEnterExit.IsPlayerInsideDungeon)
            //    {
            //        // Just move player to start of dungeon
            //        playerEnterExit.MovePlayerToDungeonStart();
            //    }
            //    else
            //    {
            //        // Randomise environment and location
            //        RandomiseEnvironment();
            //        StartCoroutine(TeleportRandomLocation());
            //    }
            //}

            //// Preset locations
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    StartCoroutine(TeleportLocation("Daggerfall", "Daggerfall"));
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    StartCoroutine(TeleportLocation("Wayrest", "Wayrest"));
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    StartCoroutine(TeleportLocation("Sentinel", "Sentinel"));
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha4))
            //{
            //    StartCoroutine(TeleportLocation("Orsinium Area", "Orsinium"));
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha5))
            //{
            //    StartCoroutine(TeleportLocation("Tulune", "The Old Copperham Place"));
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha6))
            //{
            //    StartCoroutine(TeleportLocation("Pothago", "The Stronghold of Cirden"));
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha7))
            //{
            //    StartCoroutine(TeleportLocation("Daggerfall", "Privateer's Hold"));
            //}

            //// Time scale
            //if (Input.GetKeyDown(KeyCode.Equals))
            //{
            //    timeScaleControl += timeScaleStep;
            //    if (timeScaleControl > maxTimeScaleControl)
            //        timeScaleControl = maxTimeScaleControl;
            //    dfUnity.WorldTime.TimeScale = timeScaleControl * timeScaleMultiplier;
            //}
            //if (Input.GetKeyDown(KeyCode.Minus))
            //{
            //    timeScaleControl -= timeScaleStep;
            //    if (timeScaleControl < minTimeScaleControl)
            //        timeScaleControl = minTimeScaleControl;
            //    dfUnity.WorldTime.TimeScale = timeScaleControl * timeScaleMultiplier;
            //}

            //// Music control
            //if (SongManager)
            //{
            //    if (Input.GetKeyDown(KeyCode.P))
            //        SongManager.TogglePlay();
            //    if (Input.GetKeyDown(KeyCode.LeftBracket))
            //        SongManager.PlayPreviousSong();
            //    if (Input.GetKeyDown(KeyCode.RightBracket))
            //        SongManager.PlayNextSong();
            //}

            //// Invert mouse
            //if (Input.GetKeyDown(KeyCode.I))
            //{
            //    invertMouse = !invertMouse;
            //    if (playerMouseLook) playerMouseLook.invertMouseY = invertMouse;
            //}

            //// High speed running
            //if (playerMotor)
            //{
            //    if (Input.GetKeyDown(KeyCode.H))
            //    {
            //        hiRunSpeed = !hiRunSpeed;
            //        if (hiRunSpeed)
            //            playerMotor.runSpeed = hiRunSpeedValue;
            //        else
            //            playerMotor.runSpeed = startRunSpeed;
            //    }
            //}

            //// Brighter torch
            //if (Input.GetKeyDown(KeyCode.T) && PlayerTorch != null)
            //{
            //    hiTorchRange = !hiTorchRange;
            //    if (hiTorchRange)
            //        PlayerTorch.range = hiTorchRangeValue;
            //    else
            //        PlayerTorch.range = startTorchRange;
            //}

            //// Debug strings
            //if (Input.GetKeyDown(KeyCode.L))
            //{
            //    showDebugStrings = !showDebugStrings;
            //    if (dfUnity) dfUnity.WorldTime.ShowDebugString = showDebugStrings;
            //    if (streamingWorld) streamingWorld.ShowDebugString = showDebugStrings;
            //    if (SongManager) SongManager.SongPlayer.ShowDebugString = showDebugStrings;
            //}
        }

        // Teleport player to any location by name
        IEnumerator TeleportLocation(string regionName, string locationName)
        {
            if (!CanTeleport())
                yield break;

            DFLocation location = dfUnity.ContentReader.MapFileReader.GetLocation(regionName, locationName);
            if (!location.Loaded)
                yield break;

            //if (titleScreen)
            //    titleScreen.ShowTitle = true;
            yield return new WaitForEndOfFrame();

            // Check inside range
            DFPosition mapPos = MapsFile.LongitudeLatitudeToMapPixel((int)location.MapTableData.Longitude, (int)location.MapTableData.Latitude);
            if (mapPos.X >= TerrainHelper.minMapPixelX || mapPos.X < TerrainHelper.maxMapPixelX ||
                mapPos.Y >= TerrainHelper.minMapPixelY || mapPos.Y < TerrainHelper.maxMapPixelY)
            {
                streamingWorld.TeleportToCoordinates(mapPos.X, mapPos.Y, StreamingWorld.RepositionMethods.RandomStartMarker);
            }
        }

        // Randomise player month, time of day, and weather
        void RandomiseEnvironment()
        {
            Random.InitState(Time.renderedFrameCount);

            // Want a 40% chance of winter
            if (Random.value < 0.4f)
            {
                // Set depth of winter
                dfUnity.WorldTime.Now.Month = (int)DaggerfallDateTime.Months.MorningStar;
            }
            else
            {
                // Just randomise any other month, remember that int range is exclusive of upper value
                dfUnity.WorldTime.Now.Month = Random.Range((int)DaggerfallDateTime.Months.FirstSeed, (int)DaggerfallDateTime.Months.EveningStar);
            }

            // Randomise time of day, weighted towards daylight hours
            float value = Random.value;
            if (value < 0.1f)
                dfUnity.WorldTime.Now.Hour = DaggerfallDateTime.MidnightHour;
            else if (value < 0.6f)
                dfUnity.WorldTime.Now.Hour = DaggerfallDateTime.MidMorningHour;
            else if (value < 0.8f)
                dfUnity.WorldTime.Now.Hour = DaggerfallDateTime.MidAfternoonHour;
            else if (value < 0.9f)
                dfUnity.WorldTime.Now.Hour = DaggerfallDateTime.LightsOffHour;
            else
                dfUnity.WorldTime.Now.Hour = DaggerfallDateTime.LightsOnHour;

            // Randomise weather
            // Just hardcoding chances here for demo purposes.
            // This should be based on a table using climate & season.
            if (WeatherManager)
            {
                WeatherManager.ClearAllWeather();
                bool isWinter = dfUnity.WorldTime.Now.SeasonValue == DaggerfallDateTime.Seasons.Winter;
                bool isDesert = playerGPS.ClimateSettings.ClimateType == DFLocation.ClimateBaseType.Desert;

                // Assign weather effects based on location
                if (isWinter && !isDesert)
                {
                    // In winter there is a 75% chance of snow
                    if (Random.value <= 0.75f)
                        WeatherManager.StartSnowing();
                }
                else if (isDesert)
                {
                    // Desert has only a 5% chance of rain
                    if (Random.value <= 0.05f)
                        WeatherManager.StartRaining();
                }
                else
                {
                    // Everywhere else there is a 40% chance of rain (10% storm, 30% rain)
                    if (Random.value <= 0.1f)
                        WeatherManager.StartStorming();
                    if (Random.value <= 0.4f)
                        WeatherManager.StartRaining();
                }
            }
        }

        // Teleports player to a random location in a random region
        IEnumerator TeleportRandomLocation()
        {
            if (!CanTeleport())
                yield break;

            // Find a random location
            Random.InitState(Time.renderedFrameCount);
            DFPosition mapPos = new DFPosition();
            bool found = false;
            while (!found)
            {
                // Get random region
                int regionIndex = UnityEngine.Random.Range(0, dfUnity.ContentReader.MapFileReader.RegionCount);
                DFRegion region = dfUnity.ContentReader.MapFileReader.GetRegion(regionIndex);
                if (region.LocationCount == 0)
                    continue;

                // Get random location
                int locationIndex = UnityEngine.Random.Range(0, region.MapTable.Length);
                DFLocation location = dfUnity.ContentReader.MapFileReader.GetLocation(regionIndex, locationIndex);
                if (!location.Loaded)
                    continue;

                // Check inside range
                mapPos = MapsFile.LongitudeLatitudeToMapPixel((int)location.MapTableData.Longitude, (int)location.MapTableData.Latitude);
                if ((mapPos.X >= TerrainHelper.minMapPixelX + 2 && mapPos.X < TerrainHelper.maxMapPixelX - 2) &&
                    (mapPos.Y >= TerrainHelper.minMapPixelY + 2 && mapPos.Y < TerrainHelper.maxMapPixelY - 2))
                {
                    found = true;
                }
            }

            // Teleport
            //if (titleScreen)
            //    titleScreen.ShowTitle = true;
            yield return new WaitForEndOfFrame();
            streamingWorld.TeleportToCoordinates(mapPos.X, mapPos.Y, StreamingWorld.RepositionMethods.RandomStartMarker);
        }

        private bool CanTeleport()
        {
            if (playerEnterExit.IsPlayerInside)
                return false;

            return true;
        }
    }
}