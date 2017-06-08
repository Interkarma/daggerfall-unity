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

using System;
using UnityEngine;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// A clock tracks time remaining on quests or intervals between actions.
    /// Clock must be started and stopped by quest actions.
    /// Clock will execute a task with same symbol name when countdown finished.
    /// </summary>
    public class Clock : QuestResource
    {
        ClockTypes clockType;
        DaggerfallDateTime clockStartTime;
        DaggerfallDateTime clockEndTime;
        bool clockRunning = false;

        #region Properties

        public ClockTypes ClockType
        {
            get { return clockType; }
        }

        public DaggerfallDateTime StartTime
        {
            get { return clockStartTime; }
        }

        public DaggerfallDateTime EndTime
        {
            get { return clockEndTime; }
        }

        public bool IsRunning
        {
            get { return clockRunning; }
        }

        #endregion

        #region Enums

        public enum ClockTypes
        {
            None,               // Timer not set
            Range,              // Timer starts from range between dd.hh:mm and dd.hh:mm
            Fixed,              // Timer starts from a fixed dd.hh:mm
            Basic,              // Timer is just random - currently unknown what extents classic uses here
        }

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
            SetResource(line);
        }

        #endregion

        #region Overrides

        public override void SetResource(string line)
        {
            int days, hours, minutes;
            int maxDays, maxHours, maxMinutes;
            int minDays, minHours, minMinutes;
            int flag, minRange, maxRange;

            base.SetResource(line);

            string declMatchStr = @"(?<range>Clock|clock) (?<symbol>[a-zA-Z0-9_.-]+) (?<maxDays>\d+).(?<maxHours>\d+):(?<maxMinutes>\d+) (?<minDays>\d+).(?<minHours>\d+):(?<minMinutes>\d+)|" +
                                  @"(?<fixed>Clock|clock) (?<symbol>[a-zA-Z0-9_.-]+) (?<days>\d+).(?<hours>\d+):(?<minutes>\d+)|" +
                                  @"(?<basic>Clock|clock) (?<symbol>[a-zA-Z0-9_.-]+)";

            string optionsMatchStr = @"flag (?<flag>\d+)|" +
                                     @"range (?<minRange>\d+) (?<maxRange>\d+)";

            // Try to match source line with pattern
            Match match = Regex.Match(line, declMatchStr);
            if (match.Success)
            {
                // Seed random
                UnityEngine.Random.InitState(Time.renderedFrameCount);

                // Store symbol for quest system
                Symbol = new Symbol(match.Groups["symbol"].Value);

                // Match clock type
                if (!string.IsNullOrEmpty(match.Groups["range"].Value))
                {
                    clockType = ClockTypes.Range;
                    maxDays = Parser.ParseInt(match.Groups["maxDays"].Value);
                    maxHours = Parser.ParseInt(match.Groups["maxHours"].Value);
                    maxMinutes = Parser.ParseInt(match.Groups["maxMinutes"].Value);
                    minDays = Parser.ParseInt(match.Groups["minDays"].Value);
                    minHours = Parser.ParseInt(match.Groups["minHours"].Value);
                    minMinutes = Parser.ParseInt(match.Groups["minMinutes"].Value);
                }
                else if (!string.IsNullOrEmpty(match.Groups["fixed"].Value))
                {
                    clockType = ClockTypes.Fixed;
                    days = Parser.ParseInt(match.Groups["days"].Value);
                    hours = Parser.ParseInt(match.Groups["hours"].Value);
                    minutes = Parser.ParseInt(match.Groups["minutes"].Value);
                }
                else if (!string.IsNullOrEmpty(match.Groups["basic"].Value))
                {
                    clockType = ClockTypes.Basic;
                    // TODO: Find extents for basic random clock
                }
                else
                {
                    throw new Exception("Invalid clock syntax");
                }

                // Match all options
                MatchCollection options = Regex.Matches(line, optionsMatchStr);
                foreach (Match option in options)
                {
                    // Unknown flag value
                    Group flagGroup = option.Groups["flag"];
                    if (flagGroup.Success)
                        flag = Parser.ParseInt(flagGroup.Value);

                    // Unknown minRange value
                    Group minRangeGroup = option.Groups["minRange"];
                    if (minRangeGroup.Success)
                        minRange = Parser.ParseInt(minRangeGroup.Value);

                    // Unknown maxRange value
                    Group maxRangeGroup = option.Groups["maxRange"];
                    if (maxRangeGroup.Success)
                        maxRange = Parser.ParseInt(maxRangeGroup.Value);
                }
            }
        }

        public override void Tick(Quest caller)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start or stop clock from running.
        /// Clocks must started by an action.
        /// </summary>
        /// <param name="running">Boolean to start and stop clock.</param>
        public void SetClockRunning(bool running)
        {
            clockRunning = running;
        }

        #endregion
    }
}