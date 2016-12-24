﻿// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using DaggerfallWorkshop.Game;
using IniParser;
using IniParser.Model;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Settings manager for reading game configuration from INI file.
    /// Deploys new settings from Resources/defaults.ini to persistentDataPath/settings.ini.
    /// Settings no longer present in defaults.ini are removed from settings.ini.
    /// Reads on start to public property cache. This prevents expensive string parsing if property checked every frame.
    /// Transfers settings from public property cache back to ini data on save.
    /// </summary>
    public class SettingsManager
    {
        const string defaultsIniName = "defaults.ini";
        const string settingsIniName = "settings.ini";

        const string sectionDaggerfall = "Daggerfall";
        const string sectionVideo = "Video";
        const string sectionChildGuard = "ChildGuard";
        const string sectionGUI = "GUI";
        const string sectionControls = "Controls";
        const string sectionStartup = "Startup";
        const string sectionExperimental = "Experimental";
        const string sectionEnhancements = "Enhancements";

        FileIniDataParser iniParser = new FileIniDataParser();
        IniData defaultIniData = null;
        IniData userIniData = null;

        public string PersistentDataPath
        {
            get { return Application.persistentDataPath; }
        }

        public SettingsManager()
        {
            LoadSettings();
        }

        #region Public Settings Properties

        // [Daggerfall]
        public string MyDaggerfallPath { get; set; }
        public string MyDaggerfallUnitySavePath { get; set; }

        // [Video]
        public int ResolutionWidth { get; set; }
        public int ResolutionHeight { get; set; }
        public bool VSync { get; set; }
        public bool Fullscreen { get; set; }
        public int FieldOfView { get; set; }
        public int MainFilterMode { get; set; }
        public int QualityLevel { get; set; }
        public bool UseLegacyDeferred { get; set; }
        public bool DungeonLightShadows { get; set; }

        // [ChildGuard]
        public bool PlayerNudity { get; set; }

        // [GUI]
        public bool ShowOptionsAtStart { get; set; }
        public int GUIFilterMode { get; set; }
        public int VideoFilterMode { get; set; }
        public bool Crosshair { get; set; }
        public bool SwapHealthAndFatigueColors { get; set; }
        public float DimAlphaStrength { get; set; }
        public bool FreeScaling { get; set; }
        public bool EnableToolTips { get; set; }
        public float ToolTipDelayInSeconds { get; set; }
        public Color32 ToolTipBackgroundColor { get; set; }
        public Color32 ToolTipTextColor { get; set; }
        public int AutomapNumberOfDungeons { get; set; }

        // [Controls]
        public bool InvertMouseVertical { get; set; }
        public bool MouseLookSmoothing { get; set; }
        public float MouseLookSensitivity { get; set; }
        public bool HeadBobbing { get; set; }
        public int Handedness { get; set; }
        public float WeaponSwingThreshold { get; set; }
        public float WeaponSensitivity { get; set; }

        // [Startup]
        public int StartCellX { get; set; }
        public int StartCellY { get; set; }
        public bool StartInDungeon { get; set; }

        // [Experimental]
        public bool HQTooltips { get; set; }
        public int TerrainDistance { get; set; }

        // [Enhancements]
        public bool LypyL_GameConsole { get; set; }
        public bool LypyL_ModSystem { get; set; }
        public bool LypyL_EnhancedSky { get; set; }
        public bool Nystul_IncreasedTerrainDistance { get; set; }
        public bool Nystul_RealtimeReflections { get; set; }
        public bool UncannyValley_RealGrass { get; set; }
        public bool UncannyValley_BirdsInDaggerfall { get; set; }
        public bool MeshAndTextureReplacement { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load settings from settings.ini to live properties.
        /// </summary>
        public void LoadSettings()
        {
            // Read settings from persistent file
            ReadSettingsFile();

            // Read ini data to property cache
            MyDaggerfallPath = GetString(sectionDaggerfall, "MyDaggerfallPath");
            MyDaggerfallUnitySavePath = GetString(sectionDaggerfall, "MyDaggerfallUnitySavePath");
            ResolutionWidth = GetInt(sectionVideo, "ResolutionWidth");
            ResolutionHeight = GetInt(sectionVideo, "ResolutionHeight");
            VSync = GetBool(sectionVideo, "VSync");
            Fullscreen = GetBool(sectionVideo, "Fullscreen");
            FieldOfView = GetInt(sectionVideo, "FieldOfView", 60, 80);
            MainFilterMode = GetInt(sectionVideo, "MainFilterMode", 0, 2);
            QualityLevel = GetInt(sectionVideo, "QualityLevel", 0, 5);
            UseLegacyDeferred = GetBool(sectionVideo, "UseLegacyDeferred");
            DungeonLightShadows = GetBool(sectionVideo, "DungeonLightShadows");
            PlayerNudity = GetBool(sectionChildGuard, "PlayerNudity");
            ShowOptionsAtStart = GetBool(sectionGUI, "ShowOptionsAtStart");
            GUIFilterMode = GetInt(sectionGUI, "GUIFilterMode", 0, 2);
            VideoFilterMode = GetInt(sectionGUI, "VideoFilterMode");
            Crosshair = GetBool(sectionGUI, "Crosshair");
            SwapHealthAndFatigueColors = GetBool(sectionGUI, "SwapHealthAndFatigueColors");
            DimAlphaStrength = GetFloat(sectionGUI, "DimAlphaStrength", 0, 1);
            FreeScaling = GetBool(sectionGUI, "FreeScaling");
            EnableToolTips = GetBool(sectionGUI, "EnableToolTips");
            ToolTipDelayInSeconds = GetFloat(sectionGUI, "ToolTipDelayInSeconds", 0, 10);
            ToolTipBackgroundColor = GetColor(sectionGUI, "ToolTipBackgroundColor", DaggerfallUI.DaggerfallUnityDefaultToolTipBackgroundColor);
            ToolTipTextColor = GetColor(sectionGUI, "ToolTipTextColor", DaggerfallUI.DaggerfallUnityDefaultToolTipTextColor);
            AutomapNumberOfDungeons = GetInt(sectionGUI, "AutomapNumberOfDungeons", 0, 100);
            InvertMouseVertical = GetBool(sectionControls, "InvertMouseVertical");
            MouseLookSmoothing = GetBool(sectionControls, "MouseLookSmoothing");
            MouseLookSensitivity = GetFloat(sectionControls, "MouseLookSensitivity", 0.1f, 4.0f);
            HeadBobbing = GetBool(sectionControls, "HeadBobbing");
            Handedness = GetInt(sectionControls, "Handedness", 0, 3);
            WeaponSwingThreshold = GetFloat(sectionControls, "WeaponSwingThreshold", 0.1f, 1.0f);
            WeaponSensitivity = GetFloat(sectionControls, "WeaponSensitivity", 0.1f, 10.0f);
            StartCellX = GetInt(sectionStartup, "StartCellX", 2, 997);
            StartCellY = GetInt(sectionStartup, "StartCellY", 2, 497);
            StartInDungeon = GetBool(sectionStartup, "StartInDungeon");
            HQTooltips = GetBool(sectionExperimental, "HQTooltips");
            TerrainDistance = GetInt(sectionExperimental, "TerrainDistance", 1, 4);
            LypyL_GameConsole = GetBool(sectionEnhancements, "LypyL_GameConsole");
            LypyL_ModSystem = GetBool(sectionEnhancements, "LypyL_ModSystem");
            LypyL_EnhancedSky = GetBool(sectionEnhancements, "LypyL_EnhancedSky");
            Nystul_IncreasedTerrainDistance = GetBool(sectionEnhancements, "Nystul_IncreasedTerrainDistance");
            Nystul_RealtimeReflections = GetBool(sectionEnhancements, "Nystul_RealtimeReflections");
            UncannyValley_RealGrass = GetBool(sectionEnhancements, "UncannyValley_RealGrass");
            UncannyValley_BirdsInDaggerfall = GetBool(sectionEnhancements, "UncannyValley_BirdsInDaggerfall");
            MeshAndTextureReplacement = GetBool(sectionEnhancements, "MeshAndTextureReplacement");
        }

        /// <summary>
        /// Save live properties back to settings.ini.
        /// </summary>
        public void SaveSettings()
        {
            // Write property cache to ini data
            SetString(sectionDaggerfall, "MyDaggerfallPath", MyDaggerfallPath);
            SetString(sectionDaggerfall, "MyDaggerfallUnitySavePath", MyDaggerfallUnitySavePath);
            SetInt(sectionVideo, "ResolutionWidth", ResolutionWidth);
            SetInt(sectionVideo, "ResolutionHeight", ResolutionHeight);
            SetBool(sectionVideo, "VSync", VSync);
            SetBool(sectionVideo, "Fullscreen", Fullscreen);
            SetInt(sectionVideo, "FieldOfView", FieldOfView);
            SetInt(sectionVideo, "MainFilterMode", MainFilterMode);
            SetInt(sectionVideo, "QualityLevel", QualityLevel);
            SetBool(sectionVideo, "UseLegacyDeferred", UseLegacyDeferred);
            SetBool(sectionVideo, "DungeonLightShadows", DungeonLightShadows);
            SetBool(sectionChildGuard, "PlayerNudity", PlayerNudity);
            SetBool(sectionGUI, "ShowOptionsAtStart", ShowOptionsAtStart);
            SetInt(sectionGUI, "GUIFilterMode", GUIFilterMode);
            SetInt(sectionGUI, "VideoFilterMode", VideoFilterMode);
            SetBool(sectionGUI, "Crosshair", Crosshair);
            SetBool(sectionGUI, "SwapHealthAndFatigueColors", SwapHealthAndFatigueColors);
            SetFloat(sectionGUI, "DimAlphaStrength", DimAlphaStrength);
            SetBool(sectionGUI, "FreeScaling", FreeScaling);
            SetBool(sectionGUI, "EnableToolTips", EnableToolTips);
            SetFloat(sectionGUI, "ToolTipDelayInSeconds", ToolTipDelayInSeconds);
            SetColor(sectionGUI, "ToolTipBackgroundColor", ToolTipBackgroundColor);
            SetColor(sectionGUI, "ToolTipTextColor", ToolTipTextColor);
            SetInt(sectionGUI, "AutomapNumberOfDungeons", AutomapNumberOfDungeons);
            SetBool(sectionControls, "InvertMouseVertical", InvertMouseVertical);
            SetBool(sectionControls, "MouseLookSmoothing", MouseLookSmoothing);
            SetFloat(sectionControls, "MouseLookSensitivity", MouseLookSensitivity);
            SetBool(sectionControls, "HeadBobbing", HeadBobbing);
            SetInt(sectionControls, "Handedness", Handedness);
            SetFloat(sectionControls, "WeaponSwingThreshold", WeaponSwingThreshold);
            SetFloat(sectionControls, "WeaponSensitivity", WeaponSensitivity);
            SetInt(sectionStartup, "StartCellX", StartCellX);
            SetInt(sectionStartup, "StartCellY", StartCellY);
            SetBool(sectionStartup, "StartInDungeon", StartInDungeon);
            SetBool(sectionExperimental, "HQTooltips", HQTooltips);
            SetInt(sectionExperimental, "TerrainDistance", TerrainDistance);
            SetBool(sectionEnhancements, "LypyL_GameConsole", LypyL_GameConsole);
            SetBool(sectionEnhancements, "LypyL_ModSystem", LypyL_ModSystem);
            SetBool(sectionEnhancements, "LypyL_EnhancedSky", LypyL_EnhancedSky);
            SetBool(sectionEnhancements, "Nystul_IncreasedTerrainDistance", Nystul_IncreasedTerrainDistance);
            SetBool(sectionEnhancements, "Nystul_RealtimeReflections", Nystul_RealtimeReflections);
            SetBool(sectionEnhancements, "UncannyValley_RealGrass", UncannyValley_RealGrass);
            SetBool(sectionEnhancements, "UncannyValley_BirdsInDaggerfall", UncannyValley_BirdsInDaggerfall);
            SetBool(sectionEnhancements, "MeshAndTextureReplacement", MeshAndTextureReplacement);

            // Write settings to persistent file
            WriteSettingsFile();
        }

        #endregion

        #region Private Methods

        void ReadSettingsFile()
        {
            // Load defaults.ini
            TextAsset asset = Resources.Load<TextAsset>(defaultsIniName);
            MemoryStream stream = new MemoryStream(asset.bytes);
            StreamReader reader = new StreamReader(stream);
            defaultIniData = iniParser.ReadData(reader);
            reader.Close();

            // Must have settings.ini in persistent data path
            string message;
            string userIniPath = Path.Combine(Application.persistentDataPath, settingsIniName);
            if (!File.Exists(userIniPath))
            {
                // Create file
                message = string.Format("Creating new '{0}' at path '{1}'", settingsIniName, userIniPath);
                File.WriteAllBytes(userIniPath, asset.bytes);
                DaggerfallUnity.LogMessage(message);
            }

            // Log ini path in use
            message = string.Format("Using '{0}' at path '{1}'", settingsIniName, userIniPath);
            DaggerfallUnity.LogMessage(message);

            // Load settings.ini or set as read-only
            userIniData = iniParser.ReadFile(userIniPath);

            // Ensure user ini data in sync with default ini data
            SyncIniData();
        }

        void WriteSettingsFile()
        {
            if (iniParser != null)
            {
                try
                {
                    string path = Path.Combine(Application.persistentDataPath, settingsIniName);
                    if (File.Exists(path))
                    {
                        iniParser.WriteFile(path, userIniData);
                    }
                }
                catch
                {
                    DaggerfallUnity.LogMessage("Failed to write settings.ini.");
                }
            }
        }

        string GetData(string sectionName, string valueName)
        {
            try
            {
                if (userIniData != null)
                    return userIniData[sectionName][valueName];
                else
                    throw new Exception();
            }
            catch
            {
                if (defaultIniData != null)
                    return defaultIniData[sectionName][valueName];
                else
                    throw new Exception("GetData() could not find settings.ini or defaults.ini.");
            }
        }

        void SetData(string sectionName, string valueName, string valueData)
        {
            try
            {
                if (userIniData != null)
                    userIniData[sectionName][valueName] = valueData;
                else
                    throw new Exception();
            }
            catch
            {
                throw new Exception(string.Format("Failed to SetData() for value {0}.", valueName));
            }
        }

        string GetString(string sectionName, string valueName)
        {
            return GetData(sectionName, valueName);
        }

        void SetString(string sectionName, string valueName, string value)
        {
            SetData(sectionName, valueName, value);
        }

        bool GetBool(string sectionName, string valueName)
        {
            return bool.Parse(GetData(sectionName, valueName));
        }

        void SetBool(string sectionName, string valueName, bool value)
        {
            SetData(sectionName, valueName, value.ToString());
        }

        int GetInt(string sectionName, string valueName)
        {
            return int.Parse(GetData(sectionName, valueName));
        }

        int GetInt(string sectionName, string valueName, int min, int max)
        {
            int value = int.Parse(GetData(sectionName, valueName));
            return Mathf.Clamp(value, min, max);
        }

        void SetInt(string sectionName, string valueName, int value)
        {
            SetData(sectionName, valueName, value.ToString());
        }

        float GetFloat(string sectionName, string valueName)
        {
            return float.Parse(GetData(sectionName, valueName), NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        float GetFloat(string sectionName, string valueName, float min, float max)
        {
            float value = float.Parse(GetData(sectionName, valueName), NumberStyles.Float, CultureInfo.InvariantCulture);
            return Mathf.Clamp(value, min, max);
        }

        void SetFloat(string sectionName, string valueName, float value)
        {
            SetData(sectionName, valueName, value.ToString(CultureInfo.InvariantCulture));
        }

        Color GetColor(string sectionName, string valueName, Color defaultColor)
        {
            try
            {
                string colorStr = GetData(sectionName, valueName);
                Color result = StringToColor(colorStr);
                return result;
            }
            catch
            {
                return defaultColor;
            }
        }

        void SetColor(string sectionName, string valueName, Color value)
        {
            SetData(sectionName, valueName, ColorToString(value));
        }

        #endregion

        #region Custom Parsers

        string ColorToString(Color32 color)
        {
            return string.Format("{0:X02}{1:X02}{2:X02}{3:X02}", color.r, color.g, color.b, color.a);
        }

        Color32 StringToColor(string colorStr)
        {
            const string errorMsg = "Color must be in 32-bit RGBA format, e.g. black is 000000FF.";

            if (colorStr.Length != 8)
                throw new Exception(errorMsg);

            byte r = byte.Parse(colorStr.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(colorStr.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(colorStr.Substring(4, 2), NumberStyles.HexNumber);
            byte a = byte.Parse(colorStr.Substring(6, 2), NumberStyles.HexNumber);

            return new Color32(r, g, b, a);
        }

        #endregion

        #region Syncing

        void SyncIniData()
        {
            // Default and user settings must both be loaded
            if (defaultIniData == null || userIniData == null)
                return;

            // Sync sections
            AddSections(defaultIniData, userIniData);
            RemoveSections(defaultIniData, userIniData);

            // Sync settings
            foreach (var srcSection in defaultIniData.Sections)
            {
                SectionData dstSection = userIniData.Sections.GetSectionData(srcSection.SectionName);
                if (dstSection != null)
                {
                    AddSettings(srcSection, dstSection);
                    RemoveSettings(srcSection, dstSection);
                }
            }

            // Write updated settings
            WriteSettingsFile();
        }

        void AddSections(IniData srcData, IniData dstData)
        {
            // Must have data
            if (srcData == null || dstData == null)
                return;

            // Add any sections missing from source
            foreach (var section in srcData.Sections)
            {
                if (!dstData.Sections.ContainsSection(section.SectionName))
                {
                    Debug.Log("SettingsManager: Adding section " + section.SectionName);
                    dstData.Sections.AddSection(section.SectionName);
                }
            }
        }

        void RemoveSections(IniData srcData, IniData dstData)
        {
            // Must have data
            if (srcData == null || dstData == null)
                return;

            // Gather all sections not present in source
            List<string> sectionsToRemove = new List<string>();
            foreach (var section in dstData.Sections)
            {
                if (!srcData.Sections.ContainsSection(section.SectionName))
                    sectionsToRemove.Add(section.SectionName);
            }

            // Remove sections
            foreach (var name in sectionsToRemove)
            {
                Debug.Log("SettingsManager: Removing section " + name);
                dstData.Sections.RemoveSection(name);
            }
        }

        void AddSettings(SectionData srcSection, SectionData dstSection)
        {
            // Must have data
            if (srcSection == null || dstSection == null)
                return;

            // Add source keys missing in destination
            foreach (var key in srcSection.Keys)
            {
                if (!dstSection.Keys.ContainsKey(key.KeyName))
                {
                    Debug.Log("SettingsManager: Adding setting " + key.KeyName);
                    dstSection.Keys.AddKey(key);
                }
            }
        }

        void RemoveSettings(SectionData srcSection, SectionData dstSection)
        {
            // Must have data
            if (srcSection == null || dstSection == null)
                return;

            // Gather all keys missing in source
            List<string> keysToRemove = new List<string>();
            foreach (var key in dstSection.Keys)
            {
                if (!srcSection.Keys.ContainsKey(key.KeyName))
                    keysToRemove.Add(key.KeyName);
            }

            // Remove keys
            foreach(var name in keysToRemove)
            {
                Debug.Log("SettingsManager: Removing setting " + name);
                dstSection.Keys.RemoveKey(name);
            }
        }

        #endregion
    }
}