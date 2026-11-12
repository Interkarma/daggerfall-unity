// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using System.IO;
using DaggerfallWorkshop.Utility.AssetInjection;
using Newtonsoft.Json;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class BuildingTemplates
    {
        private string SettingsDirectory;
        private string DefaultTemplatesPath;
        private string directoryPath;
        private string filePath;

        [JsonProperty] private Dictionary<string, BuildingReplacementData> _list;

        public BuildingTemplates(string settingsDirectory, string templatesFileName, string defaultTemplatesPath)
        {
            SettingsDirectory = settingsDirectory;
            DefaultTemplatesPath = defaultTemplatesPath;
            directoryPath = Application.dataPath + SettingsDirectory;
            filePath = directoryPath + templatesFileName;
        }

        public void Load()
        {
            try
            {
                StreamReader reader = new StreamReader(this.filePath);
                var data = reader.ReadToEnd();
                try
                {
                    var deserialized = JsonConvert.DeserializeObject<BuildingTemplates>(data);
                    this._list = deserialized._list;
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
                // The file does not exist, so save the default catalog
                var path = Environment.CurrentDirectory + this.DefaultTemplatesPath;
                try
                {
                    var templatesJson = File.ReadAllText(path);
                    this._list = JsonConvert.DeserializeObject<Dictionary<string, BuildingReplacementData>>(templatesJson);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                Save();
            }
        }
        public void Save()
        {
            Directory.CreateDirectory(this.directoryPath);
            var writer = File.CreateText(this.filePath);
            var fileContent = JsonConvert.SerializeObject(this);

            writer.Write(fileContent);
            writer.Close();
        }

        // setters
        public void Set(Dictionary<string, BuildingReplacementData> list)
        {
            this._list = list;
            Save();
        }

        // getters
        public Dictionary<string, BuildingReplacementData> List()
        {
            return this._list;
        }
    }
}