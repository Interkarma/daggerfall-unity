// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using System;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    [Serializable]
    public class ModSettingsKey
    {
        public enum KeyType { Toggle, MultipleChoice, Slider, FloatSlider, Tuple, FloatTuple, Text, Color }

        public string name = "Key";
        public string description = "A new key";
        public KeyType type = KeyType.Toggle;

        public Toggle toggle;
        public MultipleChoice multipleChoice;
        public Slider slider;
        public FloatSlider floatSlider;
        public Tuple tuple;
        public FloatTuple floatTuple;
        public Text text;
        public Tint color;

        [Serializable]
        public class Toggle
        {
            public bool value;
        }

        [Serializable]
        public class MultipleChoice
        {
            public string[] choices;
        }

        [Serializable]
        public class Slider
        {
            public int value;
            public int min;
            public int max;
        }

        [Serializable]
        public class FloatSlider
        {
            public float value;
            public float min;
            public float max;
        }

        [Serializable]
        public class Tuple
        {
            public int first;
            public int second;
        }

        [Serializable]
        public class FloatTuple
        {
            public float first;
            public float second;
        }

        [Serializable]
        public class Text
        {
            [TextAreaAttribute(1, 4)]
            public string text;
        }

        [Serializable]
        public class Tint
        {
            public Color32 color;
        }
    }
}
