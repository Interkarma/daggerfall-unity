// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using System;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// A simple clock component to raise current world time at runtime.
    /// Provides some events for time milestones. Be careful using events in combination
    /// with very high timescales. This can cause events to fire too often or be skipped entirely.
    /// For large-scale time changes (hundreds to thousands of objects) a better performance
    /// pattern is to simply check the DaggerfallUnity.Singleton.WorldTime.Now in Update() of each object.
    /// See DaggerfallLight.cs for an example of reading time directly for lights on/off.
    /// </summary>
    public class WorldTime : MonoBehaviour
    {
        [HideInInspector]
        public DaggerfallDateTime DaggerfallDateTime = new DaggerfallDateTime();
        public float RaiseTimeInSeconds = 0;
        public float TimeScale = 12f;
        public bool ShowDebugString = false;

        int lastHour;
        int lastDay;
        int lastMonth;
        int lastYear;

        /// <summary>
        /// Get the current world time object.
        /// Notes:
        ///  This is the live world time instance - any changes to this will be reflected in game world
        ///  If you just want a copy of value use Now.Clone()
        /// </summary>
        public DaggerfallDateTime Now
        {
            get { return DaggerfallDateTime; }
        }

        void Start()
        {
            // Init time change trackers to start time
            lastHour = DaggerfallDateTime.Hour;
            lastDay = DaggerfallDateTime.Day;
            lastMonth = DaggerfallDateTime.Month;
            lastYear = DaggerfallDateTime.Year;
        }

        void Update()
        {
            if (RaiseTimeInSeconds > 0)
            {
                DaggerfallDateTime.RaiseTime(RaiseTimeInSeconds);
                RaiseTimeInSeconds = 0;
            }
            DaggerfallDateTime.RaiseTime(Time.deltaTime * TimeScale);
            RaiseEvents();
        }

        void OnGUI()
        {
            if (Event.current.type.Equals(EventType.Repaint) && ShowDebugString)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.black;
                string text = DaggerfallDateTime.LongDateTimeString();
                GUI.Label(new Rect(10, 10, 500, 24), text, style);
                GUI.Label(new Rect(8, 8, 500, 24), text);
            }
        }

        void RaiseEvents()
        {
            // Dawn event
            if (lastHour != DaggerfallDateTime.DawnHour && DaggerfallDateTime.Hour == DaggerfallDateTime.DawnHour)
            {
                RaiseOnDawnEvent();
            }

            // Dusk event
            if (lastHour != DaggerfallDateTime.DuskHour && DaggerfallDateTime.Hour == DaggerfallDateTime.DuskHour)
            {
                RaiseOnDuskEvent();
            }

            // Midday event
            if (lastHour != DaggerfallDateTime.MiddayHour && DaggerfallDateTime.Hour == DaggerfallDateTime.MiddayHour)
            {
                RaiseOnMiddayEvent();
            }

            // Midnight event
            if (lastHour != DaggerfallDateTime.MidnightHour && DaggerfallDateTime.Hour == DaggerfallDateTime.MidnightHour)
            {
                RaiseOnMidnightEvent();
            }

            // City lights on event
            if (lastHour != DaggerfallDateTime.LightsOnHour && DaggerfallDateTime.Hour == DaggerfallDateTime.LightsOnHour)
            {
                RaiseOnCityLightsOnEvent();
            }

            // City lights off event
            if (lastHour != DaggerfallDateTime.LightsOffHour && DaggerfallDateTime.Hour == DaggerfallDateTime.LightsOffHour)
            {
                RaiseOnCityLightsOffEvent();
            }

            // New hour event
            if (lastHour != DaggerfallDateTime.Hour)
            {
                lastHour = DaggerfallDateTime.Hour;
                RaiseOnNewHourEvent();
            }

            // New day event
            if (lastDay != DaggerfallDateTime.Day)
            {
                lastDay = DaggerfallDateTime.Day;
                RaiseOnNewDayEvent();
            }

            // New month event
            if (lastMonth != DaggerfallDateTime.Month)
            {
                lastMonth = DaggerfallDateTime.Month;
                RaiseOnNewMonthEvent();
            }

            // New year event
            if (lastYear != DaggerfallDateTime.Year)
            {
                lastYear = DaggerfallDateTime.Year;
                RaiseOnNewYearEvent();
            }
        }

        #region Event Handlers

        // OnDawn
        public delegate void OnDawnEventHandler();
        public static event OnDawnEventHandler OnDawn;
        protected virtual void RaiseOnDawnEvent()
        {
            if (OnDawn != null)
                OnDawn();
        }

        // OnDusk
        public delegate void OnDuskEventHandler();
        public static event OnDuskEventHandler OnDusk;
        protected virtual void RaiseOnDuskEvent()
        {
            if (OnDusk != null)
                OnDusk();
        }

        // OnMidday
        public delegate void OnMiddayEventHandler();
        public static event OnMiddayEventHandler OnMidday;
        protected virtual void RaiseOnMiddayEvent()
        {
            if (OnMidday != null)
                OnMidday();
        }

        // OnMidnight
        public delegate void OnMidnightEventHandler();
        public static event OnMidnightEventHandler OnMidnight;
        protected virtual void RaiseOnMidnightEvent()
        {
            if (OnMidnight != null)
                OnMidnight();
        }

        // OnCityLightsOn
        public delegate void OnCityLightsOnEventHandler();
        public static event OnCityLightsOnEventHandler OnCityLightsOn;
        protected virtual void RaiseOnCityLightsOnEvent()
        {
            if (OnCityLightsOn != null)
                OnCityLightsOn();
        }

        // OnCityLightsOff
        public delegate void OnCityLightsOffEventHandler();
        public static event OnCityLightsOffEventHandler OnCityLightsOff;
        protected virtual void RaiseOnCityLightsOffEvent()
        {
            if (OnCityLightsOff != null)
                OnCityLightsOff();
        }

        // OnNewHour
        public delegate void OnNewHourEventHandler();
        public static event OnNewHourEventHandler OnNewHour;
        protected virtual void RaiseOnNewHourEvent()
        {
            if (OnNewHour != null)
                OnNewHour();
        }

        // OnNewDay
        public delegate void OnNewDayEventHandler();
        public static event OnNewDayEventHandler OnNewDay;
        protected virtual void RaiseOnNewDayEvent()
        {
            if (OnNewDay != null)
                OnNewDay();
        }

        // OnNewMonth
        public delegate void OnNewMonthEventHandler();
        public static event OnNewMonthEventHandler OnNewMonth;
        protected virtual void RaiseOnNewMonthEvent()
        {
            if (OnNewMonth != null)
                OnNewMonth();
        }

        // OnNewYear
        public delegate void OnNewYearEventHandler();
        public static event OnNewYearEventHandler OnNewYear;
        protected virtual void RaiseOnNewYearEvent()
        {
            if (OnNewYear != null)
                OnNewYear();
        }

        #endregion
    }
}