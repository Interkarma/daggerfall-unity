// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using System;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Tracks world time for day/night cycles.
    /// Also provides seasons, birthsigns, etc.
    /// </summary>
    public class WorldTime : MonoBehaviour
    {
        public const int DawnHour = 6;
        public const int DuskHour = 18;
        public const int LightsOffHour = 8;
        public const int LightsOnHour = 17;
        public const int MiddayHour = 12;
        public const int MidnightHour = 0;
        public const int MidMorningHour = 10;
        public const int MidAfternoonHour = 15;

        public const int SecondsPerMinute = 60;
        public const int MinutesPerHour = 60;
        public const int HoursPerDay = 24;
        public const int DaysPerWeek = 7;
        public const int DaysPerMonth = 30;
        public const int MonthsPerYear = 12;

        public int Year = 405;
        public int Month = 5;
        public int Day = 0;
        public int Hour = 12;
        public int Minute = 0;
        public float Second = 0;
        public float TimeScale = 10f;
        public bool ShowDebugString = false;

        void Update()
        {
            RaiseTime(Time.deltaTime * TimeScale);
        }

        void OnGUI()
        {
            if (Event.current.type.Equals(EventType.Repaint) && ShowDebugString)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.black;
                string text = GetDebugDateString();
                GUI.Label(new Rect(10, 10, 500, 24), text, style);
                GUI.Label(new Rect(8, 8, 500, 24), text);
            }
        }

        #region Properties

        public string DayName
        {
            get { return GetDayName(); }
        }

        public Days DayValue
        {
            get { return (Days)Day; }
        }

        public string MonthName
        {
            get { return GetMonthName(); }
        }

        public Months MonthValue
        {
            get { return (Months)Month; }
        }

        public string BirthSignName
        {
            get { return GetBirthSignName(); }
        }

        public BirthSigns BirthSignValue
        {
            get { return (BirthSigns)Month; }
        }

        public string SeasonName
        {
            get { return GetSeasonName(); }
        }

        public Seasons SeasonValue
        {
            get { return GetSeasonValue(); }
        }

        public bool IsNight
        {
            get { return (Hour < DawnHour || Hour >= DuskHour) ? true : false; }
        }

        public bool CityLightsOn
        {
            get { return (Hour >= LightsOnHour || Hour < LightsOffHour) ? true : false; }
        }

        /// <summary>
        /// Gets minute of day 0-1339
        /// </summary>
        public int MinuteOfDay
        {
            get { return GetMinuteOfDay(); }
        }

        /// <summary>
        /// Gets day of month 1-30
        /// </summary>
        public int DayOfMonth
        {
            get { return GetDayOfMonth(); }
        }

        /// <summary>
        /// Gets day of year 1-360.
        /// </summary>
        public int DayOfYear
        {
            get { return GetDayOfYear(); }
        }

        /// <summary>
        /// Gets month of year 1-12.
        /// </summary>
        public int MonthOfYear
        {
            get { return GetMonthOfYear(); }
        }

        #endregion

        #region Descriptions

        string[] dayNames = new string[] {
            "Sundas", "Morndas", "Tirdas", "Middas", "Turdas", "Fredas", "Loredas",
        };

        string[] monthNames = new string[] {
            "Morning Star", "Sun's Dawn", "First Seed", "Rain's Hand", "Second Seed",
            "Midyear", "Sun's Height", "Last Seed", "Hearthfire", "Frostfall", "Sun's Dusk",
            "Evening Star",
        };

        string[] birthSignNames = new string[] {
            "The Ritual", "The Lover", "The Lord", "The Mage", "The Shadow", "The Steed",
            "The Apprentice", "The Warrior", "The Lady", "The Tower", "The Atronach",
            "The Thief",
        };

        string[] seasonNames = new string[] {
            "Fall", "Spring", "Summer", "Winter",
        };

        #endregion

        #region Enums

        public enum Days
        {
            Sundas,
            Morndas,
            Tirdas,
            Middas,
            Turdas,
            Fredas,
            Loredas,
        }

        public enum Months
        {
            MorningStar,
            SunsDawn,
            FirstSeed,
            RainsHand,
            SecondSeed,
            Midyear,
            SunsHeight,
            LastSeed,
            Hearthfire,
            Frostfall,
            SunsDusk,
            EveningStar,
        }

        public enum BirthSigns
        {
            TheRitual,
            TheLover,
            TheLord,
            TheMage,
            TheShadow,
            TheSteed,
            TheApprentice,
            TheWarrior,
            TheLady,
            TheTower,
            TheAtronach,
            TheThief,
        }

        public enum Seasons
        {
            Fall,
            Spring,
            Summer,
            Winter,
        }

        #endregion

        #region Private Methods

        private void RaiseTime(float seconds)
        {
            // Increment seconds by any amount
            Second += seconds;

            // Push remainder up the scale
            if (Second >= SecondsPerMinute)
            {
                int minutes = (int)(Second / SecondsPerMinute);
                Second -= minutes * SecondsPerMinute;
                Minute += minutes;
            }
            if (Minute >= MinutesPerHour)
            {
                int hours = (int)(Minute / MinutesPerHour);
                Minute -= hours * MinutesPerHour;
                Hour += hours;
            }
            if (Hour >= HoursPerDay)
            {
                int days = (int)(Hour / HoursPerDay);
                Hour -= days * HoursPerDay;
                Day += days;
            }
            if (Day >= DaysPerMonth)
            {
                int months = (int)(Day / DaysPerMonth);
                Day -= months * DaysPerMonth;
                Month += months;
            }
            if (Month >= MonthsPerYear)
            {
                int years = (int)(Month / MonthsPerYear);
                Month -= years * MonthsPerYear;
                Year += years;
            }
        }

        private string GetSuffix(int day)
        {
            string suffix = "th";
            if (day == 1 || day == 21)
                suffix = "st";
            else if (day == 2 || day == 22)
                suffix = "nd";
            else if (day == 3 || day == 33)
                suffix = "rd";

            return suffix;
        }


        private string GetDayName()
        {
            if (Day < 0 || Day >= DaysPerMonth)
                RaiseTime(0);

            int week = (int)(Day / DaysPerWeek);
            int day = (int)(Day - (week * DaysPerWeek));

            return dayNames[day];
        }

        private string GetMonthName()
        {
            if (Month < 0 || Month >= MonthsPerYear)
                RaiseTime(0);

            return monthNames[(int)Month];
        }

        private string GetBirthSignName()
        {
            if (Month < 0 || Month >= MonthsPerYear)
                RaiseTime(0);

            return birthSignNames[(int)Month];
        }

        private Seasons GetSeasonValue()
        {
            if (Month < 0 || Month >= MonthsPerYear)
                RaiseTime(0);

            // Daggerfall seems to roll over  seasons part way through final month.
            // Using clean month boundaries here for simplicity.
            // Could use DayOfYear ranges instead to be more accurate.

            switch (Month)
            {
                case 11:
                case 0:
                case 1:
                    return Seasons.Winter;
                case 2:
                case 3:
                case 4:
                    return Seasons.Spring;
                case 5:
                case 6:
                case 7:
                    return Seasons.Summer;
                case 8:
                case 9:
                case 10:
                    return Seasons.Fall;
                default:
                    return Seasons.Summer;
            }
        }

        private string GetSeasonName()
        {
            if (Month < 0 || Month >= MonthsPerYear)
                RaiseTime(0);

            return seasonNames[(int)GetSeasonValue()];
        }

        private int GetMinuteOfDay()
        {
            RaiseTime(0);
            return (Hour * MinutesPerHour) + Minute;
        }

        private int GetDayOfMonth()
        {
            RaiseTime(0);
            return Day + 1;
        }

        private int GetDayOfYear()
        {
            RaiseTime(0);
            return (Month * DaysPerMonth) + (Day + 1);
        }

        private int GetMonthOfYear()
        {
            RaiseTime(0);
            return Month + 1;
        }

        private string GetDebugDateString()
        {
            string final = string.Format("{0:00}:{1:00}:{2:00} on {3}, {4:00} of {5:00}, 3E{6}",
                Hour, Minute, Second, DayName, Day + 1, MonthName, Year);

            return final;
        }

        #endregion
    }
}