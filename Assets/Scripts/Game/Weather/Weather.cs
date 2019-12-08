using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace DaggerfallWorkshop.Game.Weather
{
    /// <summary>
    /// The different kinds of weather possible in Daggerfall.
    /// Ordering of weather enums are very important. We order by descending pleasant-ness. That is, a snowy day is less
    /// pleasant than a sunny one. This makes weather transitions a simple math operation.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum WeatherType
    {
        Sunny,
        None = Sunny, // compatibility with older saves
        Cloudy,
        Overcast,
        Fog,
        Rain,
        Rain_Normal = Rain, // compatibility with older saves
        Thunder,
        Snow,
        Snow_Normal = Snow // compatibility with older saves
    }

    /// <summary>
    /// The weather odds for one climate and season.
    /// </summary>
    [Serializable]
    public class WeatherClimateSeason
    {
        public float Sunny;
        public float Cloudy;
        public float Overcast;
        public float Fog;
        public float Rain;
        public float Snow;
        public float Thunder;

        private readonly List<WeatherChance> _probabilityDist;
        private bool _compiled;

        /// <summary>
        /// Simple extension of Tuple to make weather chances code readable
        /// </summary>
        private class WeatherChance : DaggerfallWorkshop.Utility.Tuple<WeatherType, float>
        {
            public WeatherType Type { get { return First; }}
            public float Chance { get { return Second; }}

            public WeatherChance(WeatherType a, float b) : base(a, b) {}
        }

        public WeatherClimateSeason()
        {
            _probabilityDist = new List<WeatherChance>();
        }

        /// <summary>
        /// Ensures the configured season has valid probability.
        /// </summary>
        private void Validate()
        {
            var totalProbability = Sunny + Cloudy + Overcast + Fog + Rain + Snow + Thunder;
            // handle loss of precision with floating pts
            if (Math.Abs(totalProbability - 100f) < 0.1f) return;

            Debug.LogWarning("Total probability of the weather chances didn't add to 100%! Normalizing values.");
            var scale = 100f / totalProbability;
            Sunny *= scale;
            Cloudy *= scale;
            Overcast *= scale;
            Fog *= scale;
            Rain *= scale;
            Snow *= scale;
            Thunder *= scale;
        }

        /// <summary>
        /// Compiles the odds into a sorted list for optimized look ups during weather forecasting.
        /// </summary>
        public void CompileOdds()
        {
            if (_compiled) return;
            Validate();

            // Order doesn't matter as long as the index is uniformly randomly generated
            _probabilityDist.Add(new WeatherChance(WeatherType.Sunny, Sunny));
            _probabilityDist.Add(new WeatherChance(WeatherType.Cloudy, Cloudy));
            _probabilityDist.Add(new WeatherChance(WeatherType.Overcast, Overcast));
            _probabilityDist.Add(new WeatherChance(WeatherType.Fog, Fog));
            _probabilityDist.Add(new WeatherChance(WeatherType.Rain, Rain));
            _probabilityDist.Add(new WeatherChance(WeatherType.Snow, Snow));
            _probabilityDist.Add(new WeatherChance(WeatherType.Thunder, Thunder));
            _compiled = true;
        }

        /// <summary>
        /// Get the next randomly chosen weather.
        /// <p/>
        /// The brute force way to compute this is to create an N-element array with the weather types and pick a random
        /// index. N is 100/min_precision. For us, min_precision is 1% and N is 100.
        /// <p/>
        /// This uses a cumulative distribution list of weather chances. Generates a random value and iterate across
        /// until we find a >= cumulative probability. This cuts down the list to the number of weather types (7).
        /// <p/>
        /// This isn't critical since both methods are constant time and the weather logic is infrequent. But oh well!
        /// </summary>
        /// <returns>Next weather pattern to move towards.</returns>
        public WeatherType GetWeather()
        {
            CompileOdds();

            var rand = Random.Range(0f, 100f);
            foreach (var weather in _probabilityDist)
            {
                rand -= weather.Chance;
                if (rand <= 0) return weather.Type;
            }
            // shouldn't happen, but return something
            Debug.LogWarning("Couldn't find a cumulative probability?! " + rand);
            return WeatherType.Sunny;
        }

        /// <summary>
        /// Makes a pretty formatted string with all weather odds.
        /// </summary>
        public override string ToString()
        {
            return string.Format(
                "Sunny {0}; Cloudy {1}; Overcast {2}; Fog: {3}; Rain {4}; Thunder {5}; Snow {6}",
                Sunny, Cloudy, Overcast, Fog, Rain, Thunder, Snow
            );
        }
    }

    /// <summary>
    /// All the weather odds for a season
    /// </summary>
    [Serializable]
    public class WeatherClimate
    {
        public WeatherClimateSeason Winter;
        public WeatherClimateSeason Spring;
        public WeatherClimateSeason Summer;
        public WeatherClimateSeason Fall;

        public WeatherType GetWeather(DaggerfallDateTime.Seasons season)
        {
            switch (season)
            {
                case DaggerfallDateTime.Seasons.Fall:
                    return Fall.GetWeather();
                case DaggerfallDateTime.Seasons.Spring:
                    return Spring.GetWeather();
                case DaggerfallDateTime.Seasons.Summer:
                    return Summer.GetWeather();
                case DaggerfallDateTime.Seasons.Winter:
                    return Winter.GetWeather();
            }
            Debug.LogWarning("Unknown season! " + season);
            return WeatherType.Sunny;
        }
    }

    /// <summary>
    /// The complete weather table for all climates and seasons
    /// </summary>
    [Serializable]
    public class WeatherTable
    {
        /// <summary>
        /// Name of the weather table JSON file in the resource folder.
        /// <p/>
        /// The contents are taken from the weather table in the DaggerFall Chronicles pg. 47.
        /// Weather patterns without rows are assumed to have 0% chance of occurring.
        /// Values are in %. Totals for a season should add up to 100.
        /// </summary>
        private const string WeatherTableFile = "WeatherTable";

        public WeatherClimate Desert;
        public WeatherClimate Mountains;
        public WeatherClimate Jungle;
        public WeatherClimate Swamp;
        public WeatherClimate Subtropical;
        public WeatherClimate Woodlands;

        /// <summary>
        /// Computes the next weather based on the chances for the given climate and season.
        /// </summary>
        /// <param name="climate">Climate index from PlayerGPS</param>
        /// <param name="season">Current season</param>
        /// <returns>Next random weather</returns>
        public WeatherType GetWeather(int climate, DaggerfallDateTime.Seasons season)
        {
            var mapClimate = (MapsFile.Climates) climate;
            switch (mapClimate)
            {
                case MapsFile.Climates.Desert:
                case MapsFile.Climates.Desert2:
                    return Desert.GetWeather(season);
                case MapsFile.Climates.Mountain:
                case MapsFile.Climates.MountainWoods:
                    return Mountains.GetWeather(season);
                case MapsFile.Climates.Rainforest:
                    return Jungle.GetWeather(season);
                case MapsFile.Climates.Ocean:
                case MapsFile.Climates.Swamp:
                    return Swamp.GetWeather(season);
                case MapsFile.Climates.Subtropical:
                    return Subtropical.GetWeather(season);
                case MapsFile.Climates.Woodlands:
                case MapsFile.Climates.HauntedWoodlands:
                    return Woodlands.GetWeather(season);
            }
            Debug.LogWarning("Unknown climate! " + climate);
            return WeatherType.Sunny;
        }

        /// <summary>
        /// Parses the weather JSON table.
        /// </summary>
        /// <returns>A weather table object</returns>
        public static WeatherTable ParseJsonTable()
        {
            var json = Resources.Load(WeatherTableFile) as TextAsset;
            Assert.IsNotNull(json, "Could not find weather table JSON file!");
            return JsonUtility.FromJson(json.text, typeof(WeatherTable)) as WeatherTable;
        }
    }
}
