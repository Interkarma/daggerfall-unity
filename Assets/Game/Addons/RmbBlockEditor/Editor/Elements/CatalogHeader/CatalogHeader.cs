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
using UnityEditor;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor.Elements
{
    public class CatalogHeader : VisualElement
    {
        private const string WorldDataFolder = "/StreamingAssets/WorldData/";
        private const string exportFile = "CatalogExport";
        private Func<List<CatalogItem>> getCustomAssets;
        private Catalog catalog;
        private string title;
        private Action change;

        public event Action OnCatalogUpdated
        {
            add => change += value;
            remove => change -= value;
        }

        public CatalogHeader(Catalog catalog, Func<List<CatalogItem>> getCustomAssets, string title)
        {
            this.catalog = catalog;
            this.getCustomAssets = getCustomAssets;
            this.title = title;

            // Register a callback to be invoked after the element has been removed
            RegisterCallback<DetachFromPanelEvent>(evt => OnRemovedFromHierarchy());

            var template =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Elements/CatalogHeader/Template.uxml");
            Add(template.CloneTree());

            Initialize();
        }

        private void Initialize()
        {
            var titleElement = this.Query<Label>("catalog-title").First();
            var import = this.Query<Button>("import").First();
            var export = this.Query<Button>("export").First();
            var removeAll = this.Query<Button>("remove-all").First();
            var restoreDefaults = this.Query<Button>("restore-defaults").First();
            var scan = this.Query<Button>("scan").First();

            titleElement.text = title;

            import.RegisterCallback<MouseUpEvent>(OnImport, TrickleDown.TrickleDown);
            export.RegisterCallback<MouseUpEvent>(OnExport, TrickleDown.TrickleDown);
            removeAll.RegisterCallback<MouseUpEvent>(OnRemoveAllItems, TrickleDown.TrickleDown);
            restoreDefaults.RegisterCallback<MouseUpEvent>(OnRestoreDefault, TrickleDown.TrickleDown);
            scan.RegisterCallback<MouseUpEvent>(OnScan, TrickleDown.TrickleDown);
        }


        private void OnImport(MouseUpEvent evt)
        {
            var importType = EditorUtility.DisplayDialogComplex("Import Catalog",
                "You are about to import a catalog from a file. Would you like it to replace the existing catalog, or to be merged with it?",
                "Replace",
                "Merge",
                "Cancel");
            if (importType == 2) return; // Cancel

            var newCatalog = new List<CatalogItem>();
            var success = LoadFile(ref newCatalog);

            if (importType == 0 && success) // Replace
            {
                catalog.Set(newCatalog);
                change();
            }

            if (importType == 1 && success) // Merge
            {
                var newList = MergeCatalogs(catalog.List(), newCatalog);
                catalog.Set(newList);
                change();
            }
        }

        private void OnExport(MouseUpEvent evt)
        {
            var fileContent = JsonConvert.SerializeObject(catalog);
            var path = EditorUtility.SaveFilePanel("Save", WorldDataFolder, exportFile, "json");
            if (String.IsNullOrEmpty(path))
            {
                return;
            }
            File.WriteAllText(path, fileContent);
        }

        private void OnRemoveAllItems(MouseUpEvent evt)
        {
            var confirmed = EditorUtility.DisplayDialog("Remove All Items?",
                "You are about to remove all of the items from the catalog! Are you sure?", "Yes",
                "No");
            if (!confirmed) return;
            catalog.Set(new List<CatalogItem>());
            change();
        }

        private void OnRestoreDefault(MouseUpEvent evt)
        {
            var confirmed = EditorUtility.DisplayDialog("Restore Default Catalog?",
                "You are about to restore the default catalog! Are you sure?", "Yes",
                "No");
            if (!confirmed) return;
            catalog.RestoreDefaults();
            change();
        }

        private void OnScan(MouseUpEvent evt)
        {
            var importType = EditorUtility.DisplayDialogComplex(
                "Scan for custom assets",
                "You are about to scan for custom assets from mods. " +
                "Would you like the resulting catalog to replace the existing one, or to be merged with it?",
                "Replace",
                "Merge",
                "Cancel");

            if (importType == 2) return; // Cancel

            var newCatalog = getCustomAssets();

            if (importType == 0) // Replace
            {
                catalog.Set(newCatalog);
                change();
            }

            if (importType == 1) // Merge
            {
                var newList = MergeCatalogs(catalog.List(), newCatalog);
                catalog.Set(newList);
                change();
            }
        }

        private List<CatalogItem> MergeCatalogs(List<CatalogItem> catalog1, List<CatalogItem> catalog2)
        {
            foreach (var item in catalog2)
            {
                var oldIndex = catalog1.FindIndex((i) => i.ID == item.ID);
                if (oldIndex == -1)
                {
                    catalog1.Add(item);
                }
                else
                {
                    catalog1[oldIndex] = item;
                }
            }

            return catalog1;
        }

        private bool LoadFile(ref List<CatalogItem> newCatalogList)
        {
            var path = EditorUtility.OpenFilePanel("Import catalog", WorldDataFolder, "json");

            if (String.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return false;
            }

            try
            {
                var catalogJson = File.ReadAllText(path);
                var newCatalog = JsonConvert.DeserializeObject<Catalog>(catalogJson);
                newCatalogList = newCatalog?.List();
                return true;
            }
            catch (ArgumentException e)
            {
                return false;
            }
        }

        private void UnregisterCallbacks()
        {
            var import = this.Query<Button>("import").First();
            var export = this.Query<Button>("export").First();
            var removeAll = this.Query<Button>("remove-all").First();
            var restoreDefaults = this.Query<Button>("restore-defaults").First();
            var scan = this.Query<Button>("scan").First();

            import.UnregisterCallback<MouseUpEvent>(OnImport, TrickleDown.TrickleDown);
            export.UnregisterCallback<MouseUpEvent>(OnExport, TrickleDown.TrickleDown);
            removeAll.UnregisterCallback<MouseUpEvent>(OnRemoveAllItems, TrickleDown.TrickleDown);
            restoreDefaults.UnregisterCallback<MouseUpEvent>(OnRestoreDefault, TrickleDown.TrickleDown);
            scan.UnregisterCallback<MouseUpEvent>(OnScan, TrickleDown.TrickleDown);
        }

        private void OnRemovedFromHierarchy()
        {
            UnregisterCallbacks();
        }
    }
}