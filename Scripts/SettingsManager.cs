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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using IniParser;
using IniParser.Model;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Settings manager for reading game configuration from INI file.
    /// Can read from settings.ini in game data directory or from built-in fallback.ini in Resources.
    /// For any setting failing a read (ot settings.ini missing), fallback.ini will be used instead.
    /// </summary>
    public class SettingsManager
    {
        const string settingsIniName = "settings.ini";
        const string fallbackIniName = "fallback.ini";
        const string sectionDaggerfall = "Daggerfall";
        const string sectionStartup = "Startup";
        const string sectionGUI = "GUI";
        const string sectionControls = "Controls";
        const string sectionChildGuard = "ChildGuard";

        bool usingFallback = false;
        FileIniDataParser iniParser = new FileIniDataParser();
        IniData fallbackIniData = null;
        IniData userIniData = null;

        public SettingsManager()
        {
            ReadSettings();
        }

        #region Settings Properties

        public string MyDaggerfallPath
        {
            get { return GetString(sectionDaggerfall, "MyDaggerfallPath"); }
            set { SetString(sectionStartup, "MyDaggerfallPath", value); }
        }

        public string StartingLocation
        {
            get { return GetString(sectionStartup, "StartingLocation"); }
            set { SetString(sectionStartup, "StartingLocation", value); }
        }

        public bool StartInDungeon
        {
            get { return GetBool(sectionStartup, "StartInDungeon"); }
            set { SetBool(sectionStartup, "StartInDungeon", value); }
        }

        public bool SwapHealthAndFatigueColors
        {
            get { return GetBool(sectionGUI, "SwapHealthAndFatigueColors"); }
            set { SetBool(sectionGUI, "SwapHealthAndFatigueColors", value); }
        }

        public bool HeadBobbing
        {
            get { return GetBool(sectionControls, "HeadBobbing"); }
            set { SetBool(sectionControls, "HeadBobbing", value); }
        }

        public bool NoPlayerNudity
        {
            get { return GetBool(sectionChildGuard, "NoPlayerNudity"); }
            set { SetBool(sectionChildGuard, "NoPlayerNudity", value); }
        }

        #endregion

        #region Public Methods

        public void RereadSettings()
        {
            ReadSettings();
        }

        #endregion

        #region Private Methods

        void ReadSettings()
        {
            // Attempt to load settings.ini
            bool loadedUserSettings = false;
            string userIniPath = Path.Combine(Application.dataPath, settingsIniName);
            if (File.Exists(userIniPath))
            {
                userIniData = iniParser.ReadFile(userIniPath);
                loadedUserSettings = true;
            }

            // Load fallback.ini
            bool loadedFallbackSettings = false;
            TextAsset asset = Resources.Load<TextAsset>(fallbackIniName);
            if (asset != null)
            {
                MemoryStream stream = new MemoryStream(asset.bytes);
                StreamReader reader = new StreamReader(stream);
                fallbackIniData = iniParser.ReadData(reader);
                reader.Close();
                usingFallback = true;
                loadedFallbackSettings = true;
            }

            // Report on primary ini file
            if (loadedUserSettings)
                DaggerfallUnity.LogMessage("Loaded settings.ini.");
            else if (!loadedUserSettings && loadedFallbackSettings)
                DaggerfallUnity.LogMessage("Loaded fallback.ini");
            else
                DaggerfallUnity.LogMessage("Failed to load fallback.ini.");
        }

        void WriteSettings()
        {
            if (iniParser != null && !usingFallback)
            {
                string path = Path.Combine(Application.dataPath, settingsIniName);
                if (File.Exists(path))
                {
                    iniParser.WriteFile(path, userIniData);
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
                if (fallbackIniData != null)
                    return fallbackIniData[sectionName][valueName];
                else
                    throw new Exception("GetData() could not find settings.ini or fallback.ini.");
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

        #endregion
    }
}