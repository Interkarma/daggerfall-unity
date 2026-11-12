// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class Catalog
    {
        private string SettingsDirectory;
        private string SettingsFileName;
        private string DefaultCatalogPath;
        private string directoryPath;
        private string filePath;

        [JsonProperty] private List<CatalogItem> _list;
        private Dictionary<string, CatalogItem> _items;
        private Dictionary<string, HashSet<string>> _subcategories;
        private Dictionary<string, HashSet<string>> _categories;

        public Catalog(string settingsDirectory, string settingsFileName, string defaultCatalogPath)
        {
            SettingsDirectory = settingsDirectory;
            SettingsFileName = settingsFileName;
            DefaultCatalogPath = defaultCatalogPath;
            directoryPath = Application.dataPath + SettingsDirectory;
            filePath = directoryPath + SettingsFileName;
        }

        public void Load()
        {
            try
            {
                StreamReader reader = new StreamReader(filePath);
                var data = reader.ReadToEnd();
                try
                {
                    var deserialized = JsonConvert.DeserializeObject<Catalog>(data);
                    _list = deserialized._list;
                    GenerateCatalogDictionaries(_list, ref _items, ref _subcategories, ref _categories);
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
                RestoreDefaults();
            }
        }

        public void Save()
        {
            Directory.CreateDirectory(directoryPath);
            var writer = File.CreateText(filePath);
            var fileContent = JsonConvert.SerializeObject(this);

            writer.Write(fileContent);
            writer.Close();

            GenerateCatalogDictionaries(_list, ref _items, ref _subcategories, ref _categories);
        }

        public void RestoreDefaults()
        {
            var path = Environment.CurrentDirectory + DefaultCatalogPath;
            try
            {
                var catalogJson = File.ReadAllText(path);
                _list = JsonConvert.DeserializeObject<List<CatalogItem>>(catalogJson);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            Save();
        }

        // setters
        public void Set(List<CatalogItem> list)
        {
            _list = list;
            Save();
        }

        public void AddItem(CatalogItem item)
        {
            _list.Add(item);
            Save();
        }

        public void ReplaceItem(int index, CatalogItem item)
        {
            _list[index] = item;
            Save();
        }

        public void RemoveItemAt(int index)
        {
            _list.RemoveAt(index);
            Save();
        }

        // getters
        public List<CatalogItem> List()
        {
            return _list;
        }

        public Dictionary<string, CatalogItem> ItemsDictionary()
        {
            return _items;
        }

        public Dictionary<string, HashSet<string>> SubcatalogDictionary()
        {
            return _subcategories;
        }

        public Dictionary<string, HashSet<string>> CatalogDictionary()
        {
            return _categories;
        }

        // helper
        public static void GenerateCatalogDictionaries(
            List<CatalogItem> catalog,
            ref Dictionary<string, CatalogItem> catalogItems,
            ref Dictionary<string, HashSet<string>> subcategories,
            ref Dictionary<string, HashSet<string>> categories
        )
        {
            catalogItems = new Dictionary<string, CatalogItem>();
            subcategories = new Dictionary<string, HashSet<string>>();
            categories = new Dictionary<string, HashSet<string>>();

            foreach (var t in catalog)
            {
                var catalogItem = t;

                // If the item has no category or no subcategory, fill these fields
                if (catalogItem.Category == "")
                {
                    catalogItem.Category = "Other";
                }

                if (catalogItem.Subcategory == "")
                {
                    catalogItem.Subcategory = $"{catalogItem.Category}_root";
                }

                // Save the item in a Dictionary for fast referencing
                catalogItems[catalogItem.ID] = catalogItem;

                // If the subcategory list of objects does not exist, create it...
                if (!subcategories.ContainsKey(catalogItem.Subcategory))
                {
                    subcategories.Add(catalogItem.Subcategory, new HashSet<string>());
                }

                // ...and add this item's id to it
                subcategories[catalogItem.Subcategory].Add(catalogItem.ID);

                // If the category's list of subcategories does not exist, create it...
                if (!categories.ContainsKey(catalogItem.Category))
                {
                    categories.Add(catalogItem.Category, new HashSet<string>());
                }

                // ...and add this subcategory to it
                categories[catalogItem.Category].Add(catalogItem.Subcategory);
            }
        }
    }
}