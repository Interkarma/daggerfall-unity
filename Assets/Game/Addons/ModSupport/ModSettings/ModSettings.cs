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
        internal ModSettings(Mod mod)
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
            return GetValue<int>(section, name);
        }

        /// <summary>
        /// Get float from user settings or, as fallback, from default settings.
        /// </summary>
        /// <param name="section">Name of section.</param>
        /// <param name="name">Name of key.</param>
        public float GetFloat(string section, string name)
        {
            return GetValue<float>(section, name);
        }

        /// <summary>
        /// Get bool from user settings or, as fallback, from default settings.
        /// </summary>
        /// <param name="section">Name of section.</param>
        /// <param name="name">Name of key.</param>
        public bool GetBool(string section, string name)
        {
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
    }
}