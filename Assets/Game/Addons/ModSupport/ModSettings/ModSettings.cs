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
using System.Globalization;
using System.Reflection;
using UnityEngine;
using IniParser.Model;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    /// <summary>
    /// Read mod settings.
    /// </summary>
    public class ModSettings
    {
        // Fields
        Mod Mod;
        IniData userSettings;
        IniData defaultSettings;

        #region Public Methods

        /// <summary>
        /// Import settings for Mod.
        /// </summary>
        /// <param name="mod">Mod to load settings for.</param>
        public ModSettings(Mod mod)
        {
            Mod = mod;

            ModSettingsReader.GetSettings(mod, out userSettings, out defaultSettings);
        }

        /// <summary>
        /// Get string from user settings or, as fallback, from default settings.
        /// </summary>
        /// <param name="section">Name of section inside .ini</param>
        /// <param name="name">Name of key inside .ini</param>
        public string GetString (string section, string name)
        {
            return GetValue(section, name);
        }

        /// <summary>
        /// Get integer from user settings or, as fallback, from default settings.
        /// </summary>
        /// <param name="section">Name of section inside .ini</param>
        /// <param name="name">Name of key inside .ini</param>
        public int GetInt(string section, string name)
        {
            return int.Parse(GetValue(section, name));
        }

        /// <summary>
        /// Get integer from user settings or, as fallback, from default settings.
        /// Value is at least equal to min.
        /// </summary>
        /// <param name="section">Name of section inside .ini</param>
        /// <param name="name">Name of key inside .ini</param>
        /// <param name="min">Minimum accepted value.</param>
        public int GetInt(string section, string name, int min)
        {
            return Mathf.Max(min, GetInt(section, name));
        }

        /// <summary>
        /// Get integer from user settings or, as fallback, from default settings.
        /// Value is clamped in range (min-max).
        /// </summary>
        /// <param name="section">Name of section inside .ini</param>
        /// <param name="name">Name of key inside .ini</param>
        /// <param name="min">Minimum accepted value.</param>
        /// <param name="max">Maximum accepted value.</param>
        public int GetInt(string section, string name, int min, int max)
        {
            return Mathf.Clamp(GetInt(section, name), min, max);
        }

        /// <summary>
        /// Get float from user settings or, as fallback, from default settings.
        /// </summary>
        /// <param name="section">Name of section inside .ini</param>
        /// <param name="name">Name of key inside .ini</param>
        public float GetFloat(string section, string name)
        {
            return float.Parse(GetValue(section, name), NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get float from user settings or, as fallback, from default settings.
        /// Value is at least equal to min.
        /// </summary>
        /// <param name="section">Name of section inside .ini</param>
        /// <param name="name">Name of key inside .ini</param>
        /// <param name="min">Minimum accepted value.</param>
        public float GetFloat(string section, string name, float min)
        {
            return Mathf.Max(min, GetFloat(section, name));
        }

        /// <summary>
        /// Get float from user settings or, as fallback, from default settings.
        /// Value is clamped in range (min-max).
        /// </summary>
        /// <param name="section">Name of section inside .ini</param>
        /// <param name="name">Name of key inside .ini</param>
        /// <param name="min">Minimum accepted value.</param>
        /// <param name="max">Maximum accepted value.</param>
        public float GetFloat(string section, string name, float min, float max)
        {
            return Mathf.Clamp(GetFloat(section, name), min, max);
        }

        /// <summary>
        /// Get bool from user settings or, as fallback, from default settings.
        /// </summary>
        /// <param name="section">Name of section inside .ini</param>
        /// <param name="name">Name of key inside .ini</param>
        public bool GetBool(string section, string name)
        {
            return bool.Parse(GetValue(section, name));
        }

        /// <summary>
        /// Get color from user settings or, as fallback, from default settings.
        /// </summary>
        /// <param name="section">Name of section inside .ini</param>
        /// <param name="name">Name of key inside .ini</param>
        public Color GetColor(string section, string name)
        {
            string colorString = GetValue(section, name);
            int hexColor = 0;
            if (int.TryParse(colorString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out hexColor))
                return ModSettingsReader.ColorFromString(colorString);

            Debug.LogError(colorString + " from " + Mod.FileName + ".ini is not a valid color. Using default color.");
            return ModSettingsReader.ColorFromString(defaultSettings[section][name]);
        }

        /// <summary>
        /// Get Tuple from user settings or, as fallback, from default settings.
        /// Format: {First}{delimiter}{Second}
        /// </summary>
        /// <param name="section">Name of section inside .ini</param>
        /// <param name="name">Name of key inside .ini</param>
        /// <param name="delimiter">Char between First and Second.</param>
        public Tuple<string, string> GetTupleString (string section, string name, string delimiter = ModSettingsReader.tupleDelimiterChar)
        {
            try
            {
                string text = GetString(section, name);
                int index = text.IndexOf(delimiter);
                return new Tuple<string, string>(text.Substring(0, index), text.Substring(index + delimiter.Length));
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Failed to get {0},{1} from {2} settings!\n{2}", section, name, Mod.Title, e.Message));
                return new Tuple<string, string>("", "");
            }
        }

        /// <summary>
        /// Get Tuple from user settings or, as fallback, from default settings.
        /// Format: {First}{delimiter}{Second}
        /// </summary>
        /// <param name="section">Name of section inside .ini</param>
        /// <param name="name">Name of key inside .ini</param>
        /// <param name="delimiter">Char between First and Second.</param>
        public Tuple<int, int> GetTupleInt(string section, string name, string delimiter = ModSettingsReader.tupleDelimiterChar)
        {
            try
            {
                var tuple = GetTupleString(section, name, delimiter);
                return new Tuple<int, int>(int.Parse(tuple.First), int.Parse(tuple.Second));
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Failed to get {0},{1} from {2} settings!\n{2}", section, name, Mod.Title, e.Message));
                return new Tuple<int, int>(0, 0);
            }
        }

        /// <summary>
        /// Get Tuple from user settings or, as fallback, from default settings.
        /// Format: {First}{delimiter}{Second}
        /// </summary>
        /// <param name="section">Name of section inside .ini</param>
        /// <param name="name">Name of key inside .ini</param>
        /// <param name="delimiter">Char between First and Second.</param>
        public Tuple<float, float> GetTupleFloat(string section, string name, string delimiter = ModSettingsReader.tupleDelimiterChar)
        {
            try
            {
                var tuple = GetTupleString(section, name, delimiter);
                float first = float.Parse(tuple.First, NumberStyles.Float, CultureInfo.InvariantCulture);
                float second = float.Parse(tuple.Second, NumberStyles.Float, CultureInfo.InvariantCulture);
                return new Tuple<float, float>(first, second);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Failed to get {0},{1} from {2} settings!\n{2}", section, name, Mod.Title, e.Message));
                return new Tuple<float, float>(0, 0);
            }
        }

        /// <summary>
        /// Deserialize a section of settings in a class.
        /// </summary>
        /// <typeparam name="T">Type of class.</typeparam>
        /// <param name="section">Name of section.</param>
        /// <param name="instance">Instance of class with keys as public fields.</param>
        public void Deserialize<T>(string section, ref T instance) where T : class
        {
            if (!userSettings.Sections.ContainsSection(section))
            {
                Debug.LogErrorFormat("Failed to parse section {0} for mod {1}", section, Mod.Title);
                return;
            }

            KeyDataCollection sectionData = userSettings[section];
            foreach (var field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                try
                {
                    if (sectionData.ContainsKey(field.Name))
                    {
                        var value = GetValue(section, field.Name, field.FieldType);
                        if (value != null)
                            field.SetValue(instance, value);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Failed to parse section {0} for mod {1}\n{2}", section, Mod.Title, e.ToString());
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get value from user settings or, as fallback, 
        /// from default settings.
        /// </summary>
        private string GetValue(string section, string name)
        {
            try
            {
                return userSettings[section][name];
            }
            catch
            {
                Debug.LogErrorFormat("Failed to get ({0},{1}) for mod {2}. Using default value.", section, name, Mod.Title);

                try
                {
                    return defaultSettings[section][name];
                }
                catch
                {
                    Debug.LogErrorFormat("Failed to get ({0},{1}) from default settings of mod {2}.", section, name, Mod.Title);
                    return null;
                }
            }
        }

        /// <summary>
        /// Get value for specified type.
        /// </summary>
        private object GetValue(string section, string name, Type type)
        {
            if (type == typeof(string))
                return GetString(section, name);
            else if (type == typeof(int))
                return GetInt(section, name);
            else if (type == typeof(float))
                return GetFloat(section, name);
            else if (type == typeof(bool))
                return GetBool(section, name);
            else if (type == typeof(Tuple<int, int>))
                return GetTupleInt(section, name);
            else if (type == typeof(Tuple<float, float>))
                return GetTupleFloat(section, name);
            if (type == typeof(Color))
                return GetColor(section, name);

            return null;
        }

        #endregion
    }
}