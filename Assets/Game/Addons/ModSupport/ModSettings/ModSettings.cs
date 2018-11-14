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
using System.Linq;
using System.Reflection;
using UnityEngine;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings
{
    /// <summary>
    /// Provides a read-only access to mod settings.
    /// </summary>
    public class ModSettings
    {
        #region Fields

        readonly Mod mod;
        readonly ModSettingsData data;

        #endregion

        #region Public Methods

        /// <summary>
        /// Import settings for a mod.
        /// </summary>
        /// <param name="mod">Mod to load settings for.</param>
        [Obsolete("Use Mod.GetSettings() to get an instance for the mod; the constructor won't be publicy accessible anymore in 0.6.")]
        public ModSettings(Mod mod)
        {
            if (!mod.HasSettings)
                throw new ArgumentException(string.Format("{0} has no settings.", mod.Title), "mod");

            this.mod = mod;
            data = ModSettingsData.Make(mod);
            data.LoadLocalValues();
        }

        /// <summary>
        /// Get string from user settings or, as fallback, from default settings.
        /// </summary>
        /// <param name="section">Name of section.</param>
        /// <param name="name">Name of key.</param>
        public string GetString(string section, string name)
        {
            return GetValue<string>(section, name);
        }

        /// <summary>
        /// Get integer from user settings or, as fallback, from default settings.
        /// </summary>
        /// <param name="section">Name of section.</param>
        /// <param name="name">Name of key.</param>
        public int GetInt(string section, string name)
        {
            // Legacy support
            int value;
            if (int.TryParse(GetTextValue(section, name), out value))
                return value;

            return GetValue<int>(section, name);
        }

        /// <summary>
        /// Get integer from user settings or, as fallback, from default settings.
        /// Value is at least equal to min.
        /// </summary>
        /// <param name="section">Name of section.</param>
        /// <param name="name">Name of key.</param>
        /// <param name="min">Minimum accepted value.</param>
        [Obsolete("Set range from editor gui")]
        public int GetInt(string section, string name, int min)
        {
            return Mathf.Max(min, GetInt(section, name));
        }

        /// <summary>
        /// Get integer from user settings or, as fallback, from default settings.
        /// Value is clamped in range (min-max).
        /// </summary>
        /// <param name="section">Name of section.</param>
        /// <param name="name">Name of key.</param>
        /// <param name="min">Minimum accepted value.</param>
        /// <param name="max">Maximum accepted value.</param>
        [Obsolete("Set range from editor gui")]
        public int GetInt(string section, string name, int min, int max)
        {
            return Mathf.Clamp(GetInt(section, name), min, max);
        }

        /// <summary>
        /// Get float from user settings or, as fallback, from default settings.
        /// </summary>
        /// <param name="section">Name of section.</param>
        /// <param name="name">Name of key.</param>
        public float GetFloat(string section, string name)
        {
            // Legacy support
            float value;
            if (float.TryParse(GetTextValue(section, name), out value))
                return value;

            return GetValue<float>(section, name);
        }

        /// <summary>
        /// Get float from user settings or, as fallback, from default settings.
        /// Value is at least equal to min.
        /// </summary>
        /// <param name="section">Name of section.</param>
        /// <param name="name">Name of key.</param>
        /// <param name="min">Minimum accepted value.</param>
        [Obsolete("Set range from editor gui")]
        public float GetFloat(string section, string name, float min)
        {
            return Mathf.Max(min, GetFloat(section, name));
        }

        /// <summary>
        /// Get float from user settings or, as fallback, from default settings.
        /// Value is clamped in range (min-max).
        /// </summary>
        /// <param name="section">Name of section.</param>
        /// <param name="name">Name of key.</param>
        /// <param name="min">Minimum accepted value.</param>
        /// <param name="max">Maximum accepted value.</param>
        [Obsolete("Set range from editor gui")]
        public float GetFloat(string section, string name, float min, float max)
        {
            return Mathf.Clamp(GetFloat(section, name), min, max);
        }

        /// <summary>
        /// Get bool from user settings or, as fallback, from default settings.
        /// </summary>
        /// <param name="section">Name of section.</param>
        /// <param name="name">Name of key.</param>
        public bool GetBool(string section, string name)
        {
            // Legacy support
            bool value;
            if (bool.TryParse(GetTextValue(section, name), out value))
                return value;

            return GetValue<bool>(section, name);
        }

        /// <summary>
        /// Get color from user settings or, as fallback, from default settings.
        /// </summary>
        /// <param name="section">Name of section.</param>
        /// <param name="name">Name of key.</param>
        public Color32 GetColor(string section, string name)
        {
            return GetValue<Color32>(section, name);
        }

        /// <summary>
        /// Get Tuple from user settings or, as fallback, from default settings.
        /// </summary>
        /// <param name="section">Name of section.</param>
        /// <param name="name">Name of key.</param>
        public Tuple<int, int> GetTupleInt(string section, string name)
        {
            return GetValue<Tuple<int, int>>(section, name);
        }

        /// <summary>
        /// Get Tuple from user settings or, as fallback, from default settings.
        /// </summary>
        /// <param name="section">Name of section.</param>
        /// <param name="name">Name of key.</param>
        public Tuple<float, float> GetTupleFloat(string section, string name)
        {
            return GetValue<Tuple<float, float>>(section, name);
        }

        /// <summary>
        /// Get a value from user settings or, as fallback, from default settings.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="sectionName">Name of section.</param>
        /// <param name="keyName">Name of key.</param>
        public T GetValue<T>(string sectionName, string keyName)
        {
            return data.GetValue<T>(sectionName, keyName);
        }

        /// <summary>
        /// Deserialize a section of settings in a class.
        /// </summary>
        /// <typeparam name="T">Type of class.</typeparam>
        /// <param name="sectionName">Name of section.</param>
        /// <param name="instance">Instance of class with keys as public fields or properties.</param>
        public void Deserialize<T>(string sectionName, ref T instance, bool includeProperties = false) where T : class
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            Section section;
            if (!data.Sections.TryGetValue(sectionName, out section))
                throw new ArgumentException(string.Format("{0} does not contains section '{1}'", mod.Title, sectionName));

            foreach (FieldInfo field in typeof(T).GetFields(bindingFlags))
            {
                Key key;
                if (section.Keys.TryGetValue(field.Name, out key))
                    field.SetValue(instance, key.ToObject());
            }

            if (includeProperties)
                foreach (PropertyInfo property in typeof(T).GetProperties(bindingFlags).Where(x => x.CanWrite))
                {
                    Key key;
                    if (section.Keys.TryGetValue(property.Name, out key))
                        property.SetValue(instance, key.ToObject(), null);
                }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get non type-safe value from user settings (legacy support).
        /// </summary>
        private string GetTextValue(string section, string name)
        {
            Key key;
            if (data.TryGetKey(section, name, out key))
                return key.TextValue;

            Debug.LogErrorFormat("Failed to get ({0},{1}) for mod {2}.", section, name, mod.Title);
            return null;
        }

        #endregion
    }
}