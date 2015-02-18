// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System;
using System.Collections;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Time and date implementation for Daggerfall's specific calendar system.
    /// Daggerfall has fixed 30-day months. See below link for more information.
    /// http://www.uesp.net/wiki/Lore:Calendar#Daggerfall_Calendar
    /// </summary>
    [Serializable]
    public class DaggerfallDateTime
    {
        #region Fields

        // Time multipliers
        public const int SecondsPerMinute = 60;
        public const int MinutesPerHour = 60;
        public const int HoursPerDay = 24;
        public const int DaysPerWeek = 7;
        public const int DaysPerMonth = 30;
        public const int MonthsPerYear = 12;
        public const int SecondsPerHour = SecondsPerMinute * MinutesPerHour;
        public const int SecondsPerDay = SecondsPerHour * HoursPerDay;
        public const int SecondsPerWeek = SecondsPerDay * DaysPerWeek;
        public const int SecondsPerMonth = SecondsPerDay * DaysPerMonth;
        public const int SecondsPerYear = SecondsPerMonth * MonthsPerYear;

        // Time common events take place
        public const int DawnHour = 6;
        public const int DuskHour = 18;
        public const int LightsOffHour = 8;
        public const int LightsOnHour = 17;
        public const int MiddayHour = 12;
        public const int MidnightHour = 0;
        public const int MidMorningHour = 10;
        public const int MidAfternoonHour = 15;

        // Time values by unit for easy use
        public int Year = 405;
        public int Month = 5;
        public int Day = 0;
        public int Hour = 12;
        public int Minute = 0;
        public float Second = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Gets current day as string.
        /// </summary>
        public string DayName
        {
            get { return GetDayName(); }
        }

        /// <summary>
        /// Gets current day enum value.
        /// </summary>
        public Days DayValue
        {
            get { return (Days)Day; }
        }

        /// <summary>
        /// Gets current month as string.
        /// </summary>
        public string MonthName
        {
            get { return GetMonthName(); }
        }

        /// <summary>
        /// Gets current month enum value.
        /// </summary>
        public Months MonthValue
        {
            get { return (Months)Month; }
        }

        /// <summary>
        /// Gets birth sign name for current month.
        /// </summary>
        public string BirthSignName
        {
            get { return GetBirthSignName(); }
        }
        
        /// <summary>
        /// Gets current birth sign enum value.
        /// </summary>
        public BirthSigns BirthSignValue
        {
            get { return (BirthSigns)Month; }
        }

        /// <summary>
        /// Gets current season as string.
        /// </summary>
        public string SeasonName
        {
            get { return GetSeasonName(); }
        }

        /// <summary>
        /// Gets current season enum value.
        /// </summary>
        public Seasons SeasonValue
        {
            get { return GetSeasonValue(); }
        }

        /// <summary>
        /// True from slightly before dusk until slightly after dawn.
        /// Daggerfall NPCs never sleep.
        /// </summary>
        public bool IsCityLightsOn
        {
            get { return (Hour >= LightsOnHour || Hour < LightsOffHour) ? true : false; }
        }

        /// <summary>
        /// True when full night has fallen.
        /// </summary>
        public bool IsNight
        {
            get { return (Hour < DawnHour || Hour >= DuskHour) ? true : false; }
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

        #region Enums

        /// <summary>
        /// Days of week.
        /// </summary>
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

        /// <summary>
        /// Months of year.
        /// </summary>
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

        /// <summary>
        /// Birthsigns by month.
        /// </summary>
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

        /// <summary>
        /// Season, ordered as in game.
        /// </summary>
        public enum Seasons
        {
            Fall,
            Spring,
            Summer,
            Winter,
        }

        #endregion

        #region Strings

        static string[] dayNames = new string[] {
            "Sundas", "Morndas", "Tirdas", "Middas", "Turdas", "Fredas", "Loredas",
        };

        static string[] monthNames = new string[] {
            "Morning Star", "Sun's Dawn", "First Seed", "Rain's Hand", "Second Seed",
            "Midyear", "Sun's Height", "Last Seed", "Hearthfire", "Frostfall", "Sun's Dusk",
            "Evening Star",
        };

        static string[] birthSignNames = new string[] {
            "The Ritual", "The Lover", "The Lord", "The Mage", "The Shadow", "The Steed",
            "The Apprentice", "The Warrior", "The Lady", "The Tower", "The Atronach",
            "The Thief",
        };

        static string[] seasonNames = new string[] {
            "Fall", "Spring", "Summer", "Winter",
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Raise time by seconds. Partial seconds are supported.
        /// </summary>
        /// <param name="seconds">Amount in seconds to raise time values.</param>
        public void RaiseTime(float seconds)
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

        /// <summary>
        /// Gets a short time string.
        /// </summary>
        public string ShortTimeString()
        {
            string final = string.Format("{0:00}:{1:00}:{2:00}", Hour, Minute, Second);

            return final;
        }

        /// <summary>
        /// Gets a long date time string.
        /// </summary>
        public string LongDateTimeString()
        {
            string final = string.Format("{0:00}:{1:00}:{2:00} on {3}, {4:00} of {5:00}, 3E{6}",
                Hour, Minute, Second, DayName, Day + 1, MonthName, Year);

            return final;
        }

        /// <summary>
        /// Gets current time in seconds.
        /// </summary>
        public long ToSeconds()
        {
            long final =
                SecondsPerYear * Year +
                SecondsPerMonth * Month +
                SecondsPerDay * Day +
                SecondsPerHour * Hour +
                SecondsPerMinute * Minute +
                (int)Second;

            return final;
        }

        /// <summary>
        /// True when date times are equal.
        /// </summary>
        public bool Equals(DaggerfallDateTime other)
        {
            if (other.ToSeconds() == this.ToSeconds())
                return true;
            else
                return false;
        }

        /// <summary>
        /// True when this date time is less than another date time.
        /// </summary>
        public bool LessThan(DaggerfallDateTime other)
        {
            if (this.ToSeconds() < other.ToSeconds())
                return true;
            else
                return false;
        }

        /// <summary>
        /// True when this date time is greater than another date time.
        /// </summary>
        public bool GreaterThan(DaggerfallDateTime other)
        {
            if (this.ToSeconds() > other.ToSeconds())
                return true;
            else
                return false;
        }

        #endregion

        #region Private Methods

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

        #endregion
    }
}