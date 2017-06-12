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
    /// A clock is an alarm that that executes a task with the same symbol name.
    /// Clock must be started and stopped by quest actions.
    /// Clock runs down in game-time (default is 12x real-time).
    /// This also means timer is paused when game is paused.
    /// </summary>
    public class Clock : QuestResource
    {
        DaggerfallDateTime lastWorldTimeSample;
        int startingTimeInSeconds;
        int remainingTimeInSeconds;
        int flag = 0;
        int minRange = 0;
        int maxRange = 0;
        bool clockEnabled = false;
        bool clockFinished = false;

        #region Properties

        public bool Enabled
        {
            get { return clockEnabled; }
            set { clockEnabled = value; }
        }

        public bool Finished
        {
            get { return clockFinished; }
        }

        public int Flag
        {
            get { return flag; }
            set { flag = value; }
        }

        public int MinRange
        {
            get { return minRange; }
            set { minRange = value; }
        }

        public int MaxRange
        {
            get { return maxRange; }
            set { maxRange = value; }
        }

        public int StartingTimeInSeconds
        {
            get { return startingTimeInSeconds; }
        }

        public int RemainingTimeInSeconds
        {
            get { return remainingTimeInSeconds; }
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
            base.SetResource(line);

            string declMatchStr = @"(Clock|clock) (?<symbol>[a-zA-Z0-9_.-]+)";

            string optionsMatchStr = @"(?<ddhhmm>)\d+.\d+:\d+|" +
                                     @"(?<hhmm>)\d+:\d+|" +
                                     @"(?<mm>)\d+|" +
                                     @"flag (?<flag>\d+)|" +
                                     @"range (?<minRange>\d+) (?<maxRange>\d+)";

            // Try to match source line with pattern
            Match match = Regex.Match(line, declMatchStr);
            if (match.Success)
            {
                // Seed random
                UnityEngine.Random.InitState(Time.renderedFrameCount);

                // Store symbol for quest system
                Symbol = new Symbol(match.Groups["symbol"].Value);

                // Split options from declaration
                string optionsLine = line.Substring(match.Length);

                // Match all options
                // TODO: Work out meaning of "flag" and "range" values
                int timeValue0 = -1;
                int timeValue1 = -1;
                int currentTimeValue = 0;
                MatchCollection options = Regex.Matches(optionsLine, optionsMatchStr);
                foreach (Match option in options)
                {
                    // Match any possible time value syntax
                    Group ddhhmmGroup = option.Groups["ddhhmm"];
                    Group hhmmGroup = option.Groups["hhmm"];
                    Group mmGroup = option.Groups["mm"];
                    if (ddhhmmGroup.Success || hhmmGroup.Success | mmGroup.Success)
                    {
                        // Get time value
                        int timeValue = MatchTimeValue(option.Value);

                        // Assign time value
                        if (currentTimeValue == 0)
                        {
                            timeValue0 = timeValue;
                            currentTimeValue++;
                        }
                        else if (currentTimeValue == 1)
                        {
                            timeValue1 = timeValue;
                            currentTimeValue++;
                        }
                        else
                        {
                            throw new Exception("Clock cannot specify more than 2 time values.");
                        }
                    }

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

                // Set total clock time based on values
                int clockTimeInSeconds = 0;
                if (currentTimeValue == 0)
                {
                    // No time value specifed: "clock _symbol_"
                    // Clock timer starts at a random value between 1 week and 1 minute
                    // TODO: Work out the actual range Daggerfall uses here
                    int minSeconds = GetTimeInSeconds(0, 0, 1);
                    int maxSeconds = GetTimeInSeconds(7, 0, 0);
                    clockTimeInSeconds = FromRange(minSeconds, maxSeconds);
                }
                else if (currentTimeValue == 1)
                {
                    // One time value specified: "clock _symbol_ dd.hh:mm"
                    // Clock timer starts at this value
                    clockTimeInSeconds = timeValue0;
                }
                else if (currentTimeValue == 2)
                {
                    // Two time values specified: "clock _symbol_ dd.hh:mm dd.hh:mm"
                    // Clock timer starts at a random point between timeValue0 (min) and timeValue1 (max)
                    // But second value must be greater than first to be a valid range
                    if (timeValue1 > timeValue0)
                        clockTimeInSeconds = FromRange(timeValue0, timeValue1);
                    else
                        clockTimeInSeconds = timeValue0;
                }

                // Set timer value in seconds
                InitialiseTimer(clockTimeInSeconds);
            }
        }

        public override void Tick(Quest caller)
        {
            // Exit if not enabled or finished
            if (!clockEnabled || clockFinished)
                return;

            // Get how much time has passed in whole seconds
            DaggerfallDateTime now = DaggerfallUnity.Instance.WorldTime.Now.Clone();
            ulong difference = now.ToSeconds() - lastWorldTimeSample.ToSeconds();

            // Remove time passed from time remaining
            remainingTimeInSeconds -= (int)difference;

            // Check if time is up
            if (remainingTimeInSeconds <= 0)
            {
                TriggerTask();
                clockEnabled = false;
                clockFinished = true;
                remainingTimeInSeconds = 0;
            }

            // Update last time sample to now
            lastWorldTimeSample = now;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts timer if not already running or complete.
        /// </summary>
        public void StartTimer()
        {
            if (!clockFinished)
            {
                clockEnabled = true;
                lastWorldTimeSample = DaggerfallUnity.Instance.WorldTime.Now.Clone();
            }
        }

        /// <summary>
        /// Stops clock from running if not already complete.
        /// </summary>
        public void StopTimer()
        {
            if (!clockFinished)
            {
                clockEnabled = false;
            }
        }

        public string GetRemainingTimeString()
        {
            TimeSpan time = TimeSpan.FromSeconds(remainingTimeInSeconds);

            return string.Format("{0}.{1}:{2}", time.Days, time.Hours, time.Minutes);
        }

        #endregion

        #region Private Methods

        int MatchTimeValue(string line)
        {
            string matchStr = @"(?<days>\d+).(?<hours>\d+):(?<minutes>\d+)|(?<hours>\d+):(?<minutes>\d+)|(?<minutes>\d+)";

            Match match = Regex.Match(line, matchStr);
            if (match.Success)
            {
                int days = Parser.ParseInt(match.Groups["days"].Value);
                int hours = Parser.ParseInt(match.Groups["hours"].Value);
                int minutes = Parser.ParseInt(match.Groups["minutes"].Value);

                return GetTimeInSeconds(days, hours, minutes);
            }

            return 0;
        }

        int GetTimeInSeconds(int days, int hours, int minutes)
        {
            return (days * 86400) + (hours * 3600) + (minutes * 60);
        }

        int FromRange(int minSeconds, int maxSeconds)
        {
            return UnityEngine.Random.Range(minSeconds, maxSeconds);
        }

        void InitialiseTimer(int clockTimeInSeconds)
        {
            startingTimeInSeconds = clockTimeInSeconds;
            remainingTimeInSeconds = clockTimeInSeconds;
        }

        void TriggerTask()
        {
            // Attempt to get task with same symbol name as this timer
            Task task = ParentQuest.GetTask(Symbol);
            if (task != null)
            {
                task.Set();
            }
            else
            {
                Debug.LogFormat("Clock timer {0} completed but could not find a task with same name.", Symbol.Name);
            }
        }

        #endregion
    }
}