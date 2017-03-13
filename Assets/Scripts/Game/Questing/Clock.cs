// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Collections;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// A clock tracks time remaining on quests or intervals between actions.
    /// Clocks can be defined ahead of time and will not start running until specified.
    /// Clocks will usually execute some task when they are finished running.
    /// </summary>
    public class Clock : QuestResource
    {
        #region Fields

        TimeValue start;    // Minimum amount of time once clock starts
        TimeValue end;      // Maximum amount of time once clock starts - ignored if less than start
        int flag;           // Currently unknown flag found after 'flag' in clock declaration
        int range1;         // Currently unknown first value found after 'range' in clock declaration
        int range2;         // Currently unknown second value found after 'range' in clock declaration

        #endregion

        #region Properties
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="parentQuest">Parent quest.</param>
        public Clock(Quest parentQuest)
            : base(parentQuest)
        {
        }

        /// <summary>
        /// Construct a clock from QBN input.
        /// </summary>
        /// <param name="parentQuest">Parent quest.</param>
        /// <param name="line">Clock definition line from QBN.</param>
        public Clock(Quest parentQuest, string line)
            : base(parentQuest)
        {
            SetClock(line);
        }

        #endregion

        #region Public Methods

        public void SetClock(string line)
        {
            // Get parts of line
            string[] parts = Parser.SplitLine(line);
            if (parts == null || parts.Length < 2)
                throw new Exception("Clock: line empty or invalid.");

            // Verify this looks like a clock resource
            if (string.Compare(parts[0], "clock", true) != 0 ||
                !parts[1].StartsWith("_") || !parts[1].EndsWith("_"))
            {
                throw new Exception("Clock: line does not begin with 'clock _symbol_'");
            }

            // Get symbol
            Symbol = parts[1];

            // Get start value
            if (parts.Length >= 3)
            {
                start = new TimeValue(parts[2]);
            }

            // Get end value
            if (parts.Length >=4)
            {
                end = new TimeValue(parts[3]);
            }

            // Get flag
            if (parts.Length >= 6 && string.Compare(parts[4], "flag", true) == 0)
            {
                flag = Parser.ParseInt(parts[5]);
            }

            // Get range
            if (parts.Length >= 9 && string.Compare(parts[6], "range", true) == 0)
            {
                range1 = Parser.ParseInt(parts[7]);
                range2 = Parser.ParseInt(parts[8]);
            }
        }

        #endregion

        #region TimeValue Class

        /// <summary>
        /// Stores a time value as found in clock declaration.
        /// </summary>
        public class TimeValue
        {
            public int Days;
            public int Hours;
            public int Minutes;

            public TimeValue()
            {
            }

            public TimeValue(int days, int hours, int minutes)
            {
                Days = days;
                Hours = hours;
                Minutes = minutes;
            }

            public TimeValue(string text)
            {
                Set(text);
            }

            public void Set(string text)
            {
                // Match dd.hh:mm OR hh:mm OR mm
                Match match = Regex.Match(text, @"(?<days>\d+).(?<hours>\d+):(?<minutes>\d+)|(?<hours>\d+):(?<minutes>\d+)|(?<minutes>\d+)");
                if (match.Success)
                {
                    Days = Parser.ParseInt(match.Groups["days"].Value);
                    Hours = Parser.ParseInt(match.Groups["hours"].Value);
                    Minutes = Parser.ParseInt(match.Groups["minutes"].Value);
                    return;
                }
            }

            public int TotalMinutes()
            {
                return Days * 1440 + Hours * 60 + Minutes;
            }

            public int TotalSeconds()
            {
                return TotalMinutes() * 60;
            }
        }

        #endregion
    }
}