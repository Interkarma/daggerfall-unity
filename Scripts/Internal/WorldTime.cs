// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using System;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// A simple clock component to raise current world time at runtime.
    /// </summary>
    public class WorldTime : MonoBehaviour
    {
        public DaggerfallDateTime WorldDateTime = new DaggerfallDateTime();
        public float TimeScale = 10f;
        public bool ShowDebugString = false;

        public DaggerfallDateTime Now
        {
            get { return WorldDateTime; }
        }

        void Update()
        {
            WorldDateTime.RaiseTime(Time.deltaTime * TimeScale);
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
    }
}