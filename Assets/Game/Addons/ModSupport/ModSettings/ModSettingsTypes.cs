// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using DaggerfallWorkshop.Utility;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    /// <summary>
    /// Type of setting key.
    /// </summary>
    public enum KeyType { Toggle, MultipleChoice, SliderInt, SliderFloat, TupleInt, TupleFloat, Text, Color }

    /// <summary>
    /// A collection of sections which can be accessed by index or name.
    /// </summary>
    [fsObject(Converter = typeof(KeyedCollectionConverter<SectionCollection, string, Section>))]
    public class SectionCollection : EnhancedKeyedCollection<string, Section>
    {
        protected override string GetKeyForItem(Section section)
        {
            return section.Name;
        }
    }

    /// <summary>
    /// A collection of keys which can be accessed by index or name.
    /// </summary>
    [fsObject(Converter = typeof(KeyedCollectionConverter<KeyCollection, string, Key>))]
    public class KeyCollection : EnhancedKeyedCollection<string, Key>
    {
        protected override string GetKeyForItem(Key key)
        {
            return key.Name;
        }
    }

    /// <summary>
    /// A collection of keys in a named group.
    /// </summary>
    [fsObject("v1")]
    public class Section
    {
        string name;

#if UNITY_EDITOR
        Action<string, string, string> syncKeyCallback;

        public Action<string, string> SyncSectionCallback {get; set;}

        public Action<string, string, string> SyncKeysCallback
        {
            set { SetSyncKeysCallback(value); }
        }
#endif
        [fsProperty()]
        public string Name
        {
            get { return name; }
            set { SetName(value); }
        }

        public KeyCollection Keys = new KeyCollection();

        public Key this[int index]
        {
            get { return Keys[index]; }
        }

        public Key this[string key]
        {
            get { return Keys[key]; }
        }

#if UNITY_EDITOR
        private void SyncKey(string oldName, string newName)
        {
            Keys.ChangeKey(Keys[oldName], newName);
            if (syncKeyCallback != null)
                syncKeyCallback(name, oldName, newName);
        }

        private void SetSyncKeysCallback(Action<string, string, string> value)
        {
            syncKeyCallback = value;
            foreach (var key in Keys)
                key.SyncKeyCallback = SyncKey;
        }
#endif

        private void SetName(string value)
        {
            if (value == name)
                return;
#if UNITY_EDITOR
            if (SyncSectionCallback != null)
                SyncSectionCallback(name, value);
#endif
            name = value;
        }
    }

    /// <summary>
    /// A single key with a name.
    /// </summary>
    [fsObject("v1")]
    public abstract class Key
    {
        const string TupleDelimiter = "<,>";

        string name;

#if UNITY_EDITOR
        public Action<string, string> SyncKeyCallback {get; set;}
#endif
        
        [fsProperty()]
        public string Name
        {
            get { return name; }
            set { SetName(value);}
        }
        
        public string Description {get; set;}
        public abstract KeyType KeyType { get; }
        public abstract string TextValue { get; set; }

        public abstract object ParseToObject(string serializedValue);

        public static Key NewDefaultKey()
        {
            return new ToggleKey();
        }

        public static string FormatTuple(object first, object second)
        {
            return string.Format("{0}{1}{2}", first, TupleDelimiter, second);
        }

        public static Tuple<string, string> SplitTuple(string text)
        {
            try
            {
                int index = text.IndexOf(TupleDelimiter);
                return new Tuple<string, string>(text.Substring(0, index), text.Substring(index + TupleDelimiter.Length));
            }
            catch { return new Tuple<string, string>(string.Empty, string.Empty); }
        }

        private void SetName(string value)
        {
            if (value == name)
                return;
#if UNITY_EDITOR
            if (SyncKeyCallback != null)
                SyncKeyCallback(name, value);
#endif
            name = value;
        }
    }

    /// <summary>
    /// A single type-safe key with a name.
    /// </summary>
    public abstract class Key<T> : Key
    {
        public T Value {get; set;}

        public sealed override string TextValue 
        {
            get { return Serialize(Value); }
            set { Value = Deserialize(value); }
        }

        public sealed override object ParseToObject(string serializedValue)
        {
            return serializedValue != null ? Deserialize(serializedValue) : Value;
        }

        public virtual string Serialize(T value)
        {
            return value.ToString();
        }

        public abstract T Deserialize (string text);

        protected static int Parse(string textValue, int fallback)
        {
            int value;
            return int.TryParse(textValue, out value) ? value : fallback;
        }

        protected static float Parse(string textValue, float fallback)
        {
            float value;
            return float.TryParse(textValue, out value) ? value : fallback;
        }
    }

    /// <summary>
    /// Holds a bool value.
    /// </summary>
    public class ToggleKey : Key<bool>
    {
        public override KeyType KeyType { get { return KeyType.Toggle; } }

        public override bool Deserialize(string textValue)
        {
            bool value;
            return bool.TryParse(textValue, out value) ? value : Value;
        }
    }

    /// <summary>
    /// Holds an option selected from a list.
    /// </summary>
    public class MultipleChoiceKey : Key<int>
    {
        public List<string> Options = new List<string>();

        public override KeyType KeyType { get { return KeyType.MultipleChoice; } }

        public override int Deserialize(string textValue)
        {
            return Mathf.Clamp(Parse(textValue, Value), 0, Options.Count);
        }
    }

    /// <summary>
    /// Holds an int value in a range.
    /// </summary>
    public class SliderIntKey : Key<int>
    {
        public int Min;
        public int Max;

        public override KeyType KeyType { get { return KeyType.SliderInt; } }

        public override int Deserialize(string textValue)
        {
            return Mathf.Clamp(Parse(textValue, Value), Min, Max);
        }
    }

    /// <summary>
    /// Holds a float value in a range.
    /// </summary>
    public class SliderFloatKey : Key<float>
    {
        public float Min;
        public float Max;

        public override KeyType KeyType { get { return KeyType.SliderFloat; } }

        public override float Deserialize(string textValue)
        {
            return Mathf.Clamp(Parse(textValue, Value), Min, Max);
        }
    }

    /// <summary>
    /// Holds two int values.
    /// </summary>
    public class TupleIntKey : Key<Tuple<int, int>>
    {
        public override KeyType KeyType { get { return KeyType.TupleInt; } }

        public override string Serialize(Tuple<int, int> value)
        {
            return FormatTuple(value.First, value.Second);
        }

        public override Tuple<int, int> Deserialize(string textValue)
        {
            var tuple = SplitTuple(textValue);
            return new Tuple<int, int>(Parse(tuple.First, Value.First), Parse(tuple.Second, Value.Second));
        }
    }

    /// <summary>
    /// Holds two float values.
    /// </summary>
    public class TupleFloatKey : Key<Tuple<float, float>>
    {
        public override KeyType KeyType { get { return KeyType.TupleFloat; } }

        public override string Serialize(Tuple<float, float> value)
        {
            return FormatTuple(value.First, value.Second);
        }

        public override Tuple<float, float> Deserialize(string textValue)
        {
            var tuple = SplitTuple(textValue);
            return new Tuple<float, float>(Parse(tuple.First, Value.First), Parse(tuple.Second, Value.Second));
        }
    }

    /// <summary>
    /// Holds a raw string.
    /// </summary>
    public class TextKey : Key<string>
    {
        public override KeyType KeyType { get { return KeyType.Text; } }

        public override string Deserialize(string value)
        {
            return value;
        }
    }

    /// <summary>
    /// Holds an RGBA color.
    /// </summary>
    public class ColorKey : Key<Color32>
    {
        public override KeyType KeyType { get { return KeyType.Color; } }

        public override string Serialize(Color32 value)
        {
            return ColorUtility.ToHtmlStringRGBA(Value);
        }
        
        public override Color32 Deserialize(string hexColor)
        {
            Color color;
            return ColorUtility.TryParseHtmlString("#" + hexColor, out color) ? color : (Color)Value;
        }
    }

    /// <summary>
    /// Holds a group of serialized values which can be merged in a corresponding <cref>ModSettingsData</cref> instance.
    /// </summary>
    [fsObject("v1")]
    public class Preset
    {
        /// <summary>
        /// Title (does not correspond to filename).
        /// </summary>
        public string Title;

        /// <summary>
        /// Optional field (for imported presets).
        /// </summary>
        public string Author;

        /// <summary>
        /// Short description shown on GUI.
        /// </summary>
        public string Description;

        /// <summary>
        /// Version of target settings (not version of preset!)
        /// </summary>
        public string SettingsVersion;

        public Dictionary<string, Dictionary<string, string>> Values = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Path on disk. This is not null only for local presets.
        /// </summary>
        [fsIgnore]
        public string DiskPath;
        public bool HasPath
        {
            get { return DiskPath != null; }
        }

        public string GetSafePath(Mod mod)
        {
            if (!HasPath)
            {
                // Create a new path from title.
                string name = string.Format("{0}preset{1}.ini", mod.FileName, Title);
                foreach (char c in Path.GetInvalidPathChars())
                    name = name.Replace(c, '_');
                DiskPath = Path.Combine(mod.DirPath, name);
            }
            return DiskPath;
        }

#if UNITY_EDITOR

        /// <summary>
        /// Does the preset contains this section?
        /// </summary>
        public bool this[string section]
        {
            get { return Values.ContainsKey(section); }
            set { SetState(Values, section, value, () => new Dictionary<string, string>()); }
        }

        /// <summary>
        /// Does the preset contains this key?
        /// </summary>
        public bool this[string section, string key]
        {
            get { return Values.Values.Any(x => x.ContainsKey(key)); }
            set
            {
                Dictionary<string, string> sectionDict;
                if (Values.TryGetValue(section, out sectionDict))
                    SetState(sectionDict, key, value, () => string.Empty);
            }
        }

        private static void SetState<T>(Dictionary<string, T> dict, string key, bool enabled, Func<T> init)
        {
            bool keyFound = dict.ContainsKey(key);
            if (enabled) { if (!keyFound) dict.Add(key, init()); }
            else { if (keyFound) dict.Remove(key); }
        }
#endif
    }
}
