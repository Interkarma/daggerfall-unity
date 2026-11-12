// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Utility.AssetInjection;
using Newtonsoft.Json;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class PersistedBuildingsCatalog
    {
        private string SettingsDirectory = $"{Application.dataPath}/Editor/Settings/RmbBlockEditor/";
        private string CatalogFile =
            $"{Application.dataPath}/Editor/Settings/RmbBlockEditor/buildings-catalog.json";
        private string DefaultCatalog =
            $"{Environment.CurrentDirectory}/Assets/Game/Addons/RmbBlockEditor/Persisted/DefaultBuildingsCatalog.json";

        [JsonProperty] public List<CatalogItem> list;
        [JsonProperty] public Dictionary<string, BuildingReplacementData> templates;
        private Dictionary<string, CatalogItem> _items;
        private Dictionary<string, HashSet<string>> _subcategories;
        private Dictionary<string, HashSet<string>> _categories;

        private static readonly PersistedBuildingsCatalog _catalog = new PersistedBuildingsCatalog();

        public PersistedBuildingsCatalog()
        {
        }

        public static void Load()
        {
            try
            {
                StreamReader reader = new StreamReader(_catalog.CatalogFile);
                var data = reader.ReadToEnd();
                try
                {
                    _catalog.LoadFromFile(data);
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
                RestoreDefault();
            }
        }

        public static void Save()
        {
            Directory.CreateDirectory(_catalog.SettingsDirectory);
            var writer = File.CreateText(_catalog.CatalogFile);
            var fileContent = JsonConvert.SerializeObject(_catalog);

            writer.Write(fileContent);
            writer.Close();
        }

        public static void RestoreDefault()
        {
            try
            {
                var defaultData = File.ReadAllText(_catalog.DefaultCatalog);
                _catalog.LoadFromFile(defaultData);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            finally
            {
                Save();
            }
        }

        private void LoadFromFile(string streamData)
        {
            var deserialized = JsonConvert.DeserializeObject<PersistedBuildingsCatalog>(streamData);
            list = deserialized.list;
            templates = deserialized.templates;
            Catalog.GenerateCatalogDictionaries(list, ref _items, ref _subcategories, ref _categories);
        }

        public static void Set(PersistedBuildingsCatalog catalog)
        {
            _catalog.list = catalog.list;
            _catalog.templates = catalog.templates;
        }

        public static void SetList(List<CatalogItem> list)
        {
            _catalog.list = list;
            Save();
        }

        public static void SetTemplates(Dictionary<string, BuildingReplacementData> templates)
        {
            _catalog.templates = templates;
        }

        public static PersistedBuildingsCatalog Get()
        {
            return _catalog;
        }

        public static List<CatalogItem> List()
        {
            return _catalog.list;
        }

        public static Dictionary<string, BuildingReplacementData> Templates()
        {
            return _catalog.templates;
        }

        public static Dictionary<string, CatalogItem> ItemsDictionary()
        {
            return _catalog._items;
        }

        public static Dictionary<string, HashSet<string>> SubcatalogDictionary()
        {
            return _catalog._subcategories;
        }

        public static Dictionary<string, HashSet<string>> CatalogDictionary()
        {
            return _catalog._categories;
        }
    }
}