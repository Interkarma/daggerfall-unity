// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com)
// Contributors:    
// 
// Notes:
//

using DaggerfallWorkshop.Utility;
using System.Text.RegularExpressions;

namespace DaggerfallWorkshop.Game.Questing
{
    public class ClockAction : ActionTemplate
    {

        private ulong startTimeInSeconds;
        private ulong endTimeInSeconds;
        private bool isRunning;                 //suspect this might serve the same purpose as one of the clock resource's flags

        public ulong StartTimeInSeconds
        {
            get { return startTimeInSeconds; }
            set { startTimeInSeconds = value; }
        }

        public ulong EndTimeInSeconds
        {
            get { return endTimeInSeconds; }
            set { endTimeInSeconds = value; }
        }

        public bool IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; }
        }

        public ClockAction(Quest parentQuest) : base(parentQuest)
        {
            base.IsTriggerCondition = true;
            this.IsRunning = false;
        }

        public void SetTimes(Clock.TimeValue start, Clock.TimeValue end)
        {
            DaggerfallDateTime startTime = new DaggerfallDateTime(DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime);
            DaggerfallDateTime endTime = new DaggerfallDateTime(DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime);
            StartTimeInSeconds = 0;
            EndTimeInSeconds = 0;

            if (start != null)
            {
                startTime.Minute += start.Minutes;
                startTime.Hour += start.Hours;
                startTime.Day += start.Days;
                this.startTimeInSeconds = startTime.ToSeconds();
            }

            if (end != null)
            {
                endTime = new DaggerfallDateTime(DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime);
                endTime.Minute += end.Minutes;
                endTime.Hour += end.Hours;
                endTime.Day += end.Days;
                this.endTimeInSeconds = endTime.ToSeconds();
            }

        }


        public override string Pattern
        {
            get
            {
                return "invalid_pattern";
            }
        }

        //the pattern property & create method are not used - the ClockAction is created by the
        //ClockResource directly rather than the QBN parser
        public override IQuestAction Create(string source, Quest parentQuest)
        {
            Match match = Test(source);
            if (!match.Success)
                return null;
            ClockAction clockAction = new ClockAction(parentQuest);
            return clockAction;
        }

        //evaluate clock condition
        public override bool CheckCondition(Task caller)
        {
            //a start clock action has to be called to set isRunning to true
            if (!this.IsRunning)
                return false;

            var currentTimeInSeconds = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds();

            if (endTimeInSeconds > 0)
            {
                if (currentTimeInSeconds > startTimeInSeconds && currentTimeInSeconds < endTimeInSeconds)
                    return true;
                else
                    return false;
            }
            else if (currentTimeInSeconds > startTimeInSeconds)
                return true;
            else
                return false;

        }

    }
}
