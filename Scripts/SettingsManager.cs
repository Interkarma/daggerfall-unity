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
    public class SettingsManager
    {
        const string settingsIniName = "settings.ini";
        const string fallbackIniName = "fallback.ini";
        const string sectionDaggerfall = "Daggerfall";
        const string sectionStartup = "Startup";
        const string sectionGUI = "GUI";
        const string sectionControls = "Controls";

        bool usingFallback = false;
        FileIniDataParser iniParser = new FileIniDataParser();
        IniData iniData;

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

        #endregion

        #region Private Methods

        void ReadSettings()
        {
            string path = Path.Combine(Application.dataPath, settingsIniName);
            if (File.Exists(path))
            {
                // Load settings.ini
                iniData = iniParser.ReadFile(path);
                DaggerfallUnity.LogMessage("Loaded settings.ini.");
            }
            else
            {
                // Load fallback.ini
                TextAsset asset = Resources.Load<TextAsset>(fallbackIniName);
                if (asset != null)
                {
                    MemoryStream stream = new MemoryStream(asset.bytes);
                    StreamReader reader = new StreamReader(stream);
                    iniData = iniParser.ReadData(reader);
                    reader.Close();
                    usingFallback = true;
                    DaggerfallUnity.LogMessage("Loaded fallback.ini");
                }
                else
                {
                    DaggerfallUnity.LogMessage("Failed to load fallback.ini.");
                }
            }
        }

        void WriteSettings()
        {
            if (iniParser != null && !usingFallback)
            {
                string path = Path.Combine(Application.dataPath, settingsIniName);
                if (File.Exists(path))
                {
                    iniParser.WriteFile(path, iniData);
                }
            }
        }

        string GetData(string sectionName, string valueName)
        {
            if (iniData != null)
            {
                return iniData[sectionName][valueName];
            }
            else
            {
                return string.Empty;
            }
        }

        void SetData(string sectionName, string valueName, string valueData)
        {
            if (iniData != null)
            {
                iniData[sectionName][valueName] = valueData;
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