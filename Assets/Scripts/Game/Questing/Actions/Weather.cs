// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: kaboissonneault (kaboissonneault@gmail.com)
// Contributors:    
// 
// Notes:
//

using System;
using System.Text.RegularExpressions;
using FullSerializer;

using DaggerfallWorkshop.Game.Weather;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// DFU extension action. Triggers when the current weather matches the condition
    /// </summary>
    public class Weather : ActionTemplate
    {
        WeatherType weather;

        public override string Pattern
        {
            get { return @"weather (?<weather>sunny|cloudy|overcast|fog|rain|thunder|snow)"; }
        }

        public Weather(Quest parentQuest)
                    : base(parentQuest)
        {
            IsTriggerCondition = true;
            IsAlwaysOnTriggerCondition = true;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new action
            Weather action = new Weather(parentQuest);
            if (!Enum.TryParse(match.Groups["weather"].Value, true /*ignore case*/, out action.weather))
            {
                throw new Exception("Weather: Syntax is 'weather sunny|cloudy|overcast|fog|rain|thunder|snow'");
            }

            return action;
        }

        public override bool CheckTrigger(Task caller)
        {
            WeatherManager WeatherManager = GameManager.Instance.WeatherManager;
            switch (weather)
            {
                case WeatherType.Sunny:
                    return !WeatherManager.IsRaining && !WeatherManager.IsOvercast && !WeatherManager.IsStorming && !WeatherManager.IsSnowing
                        && WeatherManager.currentOutdoorFogSettings.density == WeatherManager.SunnyFogSettings.density;
                case WeatherType.Cloudy:
                    return false; // TODO: weather not implemented in the weather manager
                case WeatherType.Overcast:
                    return WeatherManager.IsOvercast && WeatherManager.currentOutdoorFogSettings.density != WeatherManager.HeavyFogSettings.density
                        && !WeatherManager.IsRaining && !WeatherManager.IsStorming && !WeatherManager.IsSnowing;
                case WeatherType.Fog:
                    return WeatherManager.IsOvercast && WeatherManager.currentOutdoorFogSettings.density == WeatherManager.HeavyFogSettings.density
                        && !WeatherManager.IsStorming && !WeatherManager.IsSnowing;
                case WeatherType.Rain:
                    return WeatherManager.IsRaining && !WeatherManager.IsStorming && !WeatherManager.IsSnowing;
                case WeatherType.Thunder:
                    return WeatherManager.IsStorming && !WeatherManager.IsSnowing;
                case WeatherType.Snow:
                    return WeatherManager.IsSnowing;
                default:
                    throw new Exception("Weather: unexpected weather type");
            }
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public WeatherType weather;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.weather = weather;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            weather = data.weather;
        }
        #endregion

    }
}
