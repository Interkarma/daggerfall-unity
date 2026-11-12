// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    [ExecuteInEditMode]
    public class PersistedSettings
    {
        private const string SettingsDirectory = "/Editor/Settings/RmbBlockEditor/";
        private const string SettingsFileName = "settings.json";
        private string directoryPath;
        private string filePath;

        [JsonProperty] private ClimateBases _climateBases = DaggerfallWorkshop.ClimateBases.Temperate;
        [JsonProperty] private ClimateSeason _climateSeason = DaggerfallWorkshop.ClimateSeason.Summer;
        [JsonProperty] private WindowStyle _windowStyle = DaggerfallWorkshop.WindowStyle.Day;

        // Make this a singleton class
        private PersistedSettings()
        {
            directoryPath = Application.dataPath + SettingsDirectory;
            filePath = directoryPath + SettingsFileName;
        }

        private static readonly PersistedSettings _settings = new PersistedSettings();

        public static void Load()
        {
            try
            {
                StreamReader reader = new StreamReader(_settings.filePath);
                var data = reader.ReadToEnd();
                try
                {
                    var deserialized = JsonConvert.DeserializeObject<PersistedSettings>(data);
                    _settings._climateBases = deserialized._climateBases;
                    _settings._climateSeason = deserialized._climateSeason;
                    _settings._windowStyle = deserialized._windowStyle;
                }
                catch (Exception error)
                {
                    // The file is corrupt, so save a new one
                    Save();
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception error)
            {
                // The settings file does not exist, so save a new one
                Save();
            }
        }

        private static void Save()
        {
            Directory.CreateDirectory(_settings.directoryPath);
            var writer = File.CreateText(_settings.filePath);
            var fileContent = JsonConvert.SerializeObject(_settings);

            writer.Write(fileContent);
            writer.Close();
        }

        // setters
        public static void SetClimate(ClimateBases climateBases, ClimateSeason climateSeason, WindowStyle windowStyle)
        {
            _settings._climateBases = climateBases;
            _settings._climateSeason = climateSeason;
            _settings._windowStyle = windowStyle;
            Save();
        }


        // getters
        public static ClimateBases ClimateBases()
        {
            return _settings._climateBases;
        }

        public static ClimateSeason ClimateSeason()
        {
            return _settings._climateSeason;
        }

        public static WindowStyle WindowStyle()
        {
            return _settings._windowStyle;
        }
    }
}