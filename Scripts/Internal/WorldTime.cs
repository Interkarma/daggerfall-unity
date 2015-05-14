// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
        public DaggerfallDateTime WorldDateTime = new DaggerfallDateTime();
        public float TimeScale = 10f;
        public bool ShowDebugString = false;

        int lastHour;
        int lastDay;
        int lastMonth;
        int lastYear;

        public DaggerfallDateTime Now
        {
            get { return WorldDateTime; }
        }

        void Start()
        {
            // Init time change trackers to start time
            lastHour = WorldDateTime.Hour;
            lastDay = WorldDateTime.Day;
            lastMonth = WorldDateTime.Month;
            lastYear = WorldDateTime.Year;
        }

        void Update()
        {
            WorldDateTime.RaiseTime(Time.deltaTime * TimeScale);
            RaiseEvents();
        }

        void OnGUI()
        {
            if (Event.current.type.Equals(EventType.Repaint) && ShowDebugString)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.black;
                string text = WorldDateTime.LongDateTimeString();
                GUI.Label(new Rect(10, 10, 500, 24), text, style);
                GUI.Label(new Rect(8, 8, 500, 24), text);
            }
        }

        void RaiseEvents()
        {
            // Dawn event
            if (lastHour != DaggerfallDateTime.DawnHour && WorldDateTime.Hour == DaggerfallDateTime.DawnHour)
            {
                RaiseOnDawnEvent();
            }

            // Dusk event
            if (lastHour != DaggerfallDateTime.DuskHour && WorldDateTime.Hour == DaggerfallDateTime.DuskHour)
            {
                RaiseOnDuskEvent();
            }

            // Midday event
            if (lastHour != DaggerfallDateTime.MiddayHour && WorldDateTime.Hour == DaggerfallDateTime.MiddayHour)
            {
                RaiseOnMiddayEvent();
            }

            // Midnight event
            if (lastHour != DaggerfallDateTime.MidnightHour && WorldDateTime.Hour == DaggerfallDateTime.MidnightHour)
            {
                RaiseOnMidnightEvent();
            }

            // City lights on event
            if (lastHour != DaggerfallDateTime.LightsOnHour && WorldDateTime.Hour == DaggerfallDateTime.LightsOnHour)
            {
                RaiseOnCityLightsOnEvent();
            }

            // City lights off event
            if (lastHour != DaggerfallDateTime.LightsOffHour && WorldDateTime.Hour == DaggerfallDateTime.LightsOffHour)
            {
                RaiseOnCityLightsOffEvent();
            }

            // New hour event
            if (lastHour != WorldDateTime.Hour)
            {
                lastHour = WorldDateTime.Hour;
                RaiseOnNewHourEvent();
            }

            // New day event
            if (lastDay != WorldDateTime.Day)
            {
                lastDay = WorldDateTime.Day;
                RaiseOnNewDayEvent();
            }

            // New month event
            if (lastMonth != WorldDateTime.Month)
            {
                lastMonth = WorldDateTime.Month;
                RaiseOnNewMonthEvent();
            }

            // New year event
            if (lastYear != WorldDateTime.Year)
            {
                lastYear = WorldDateTime.Year;
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