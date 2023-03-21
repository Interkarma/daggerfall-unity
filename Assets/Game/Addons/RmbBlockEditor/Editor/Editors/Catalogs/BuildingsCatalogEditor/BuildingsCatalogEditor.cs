// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using System.IO;
using DaggerfallWorkshop.Game.Addons.RmbBlockEditor.Elements;
using DaggerfallWorkshop.Utility.AssetInjection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class BuildingsCatalogEditor: VisualElement
    {
        private const string WorldDataFolder = "/StreamingAssets/WorldData/";
        private const string exportFile = "BuildingsCatalogExport";
        private ObjectPicker picker;
        private PersistedBuildingsCatalog catalog;
        private string objectId;
        private int selectedIndex;
        private CatalogItem selectedItem;
        private BuildingReplacementData selectedBuildingData;

        public BuildingsCatalogEditor()
        {
            catalog = PersistedBuildingsCatalog.Get();
            RenderTemplate();
            InitializeCatalogItemElement();
            BindCatalogOperations();
            RenderPicker();
        }

        private void RenderTemplate()
        {
            Clear();
            // Register a callback to be invoked after the element has been removed
            RegisterCallback<DetachFromPanelEvent>(evt => OnRemovedFromHierarchy());

            var tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/Catalogs/BuildingsCatalogEditor/Template.uxml");
            Add(tree.CloneTree());
        }

        private void InitializeCatalogItemElement()
        {
            var catalogItemElement = this.Query<CatalogItemElement>("catalog-item-element").First();
            catalogItemElement.readOnly = false;
            catalogItemElement.OnSaveItem += OnSaveItem;
            catalogItemElement.OnRemoveItem += OnRemoveItem;
        }

        private void InitializeBuildingDataElement()
        {
            var buildingDataElement = this.Query<BuildingDataElement>("building-data-element").First();
            buildingDataElement.changed -= OnChangeBuildingData;
            buildingDataElement.changed += OnChangeBuildingData;
            buildingDataElement.HideCatalogImport();
        }

        private void BindCatalogOperations()
        {
            var import = this.Query<Button>("import").First();
            var export = this.Query<Button>("export").First();
            var removeAll = this.Query<Button>("remove-all").First();
            var restoreDefaults = this.Query<Button>("restore-defaults").First();
            var addNew = this.Query<Button>("add-new").First();

            import.RegisterCallback<MouseUpEvent>(OnImportCatalog, TrickleDown.TrickleDown);
            export.RegisterCallback<MouseUpEvent>(evt => { SaveFile(); }, TrickleDown.TrickleDown);
            removeAll.RegisterCallback<MouseUpEvent>(OnRemoveAllItems, TrickleDown.TrickleDown);
            restoreDefaults.RegisterCallback<MouseUpEvent>(OnRestoreDefault, TrickleDown.TrickleDown);
            addNew.RegisterCallback<MouseUpEvent>(OnAddNewItem, TrickleDown.TrickleDown);
        }

        private void RenderPicker()
        {
            var pickerElement = this.Query<VisualElement>("object-picker").First();
            pickerElement.Clear();
            if (catalog == null)
            {
                return;
            }

            picker = new ObjectPicker(catalog.list, OnItemSelected, GetPreview, objectId);
            pickerElement.Add(picker.visualElement);
        }

        private void OnItemSelected(string objectId)
        {
            this.objectId = objectId;
            selectedIndex = catalog.list.FindIndex((item) => item.ID == objectId);

            if (selectedIndex != -1)
            {
                selectedItem = catalog.list[selectedIndex];
                selectedBuildingData = catalog.templates[selectedItem.ID];
            }
            else
            {
                // If the selected ID is not in the catalog, create a new Catalog Item to display
                selectedItem = new CatalogItem(objectId);
                selectedBuildingData = new BuildingReplacementData();
            }

            var catalogItemElement = this.Query<CatalogItemElement>("catalog-item-element").First();
            catalogItemElement.SetItem(selectedItem);

            var buildingDataElement = this.Query<BuildingDataElement>("building-data-element").First();
            buildingDataElement.SetData(selectedBuildingData);

            InitializeBuildingDataElement();
            ShowOptionsBox();
        }

        private void OnChangeBuildingData(BuildingReplacementData data)
        {
            selectedBuildingData = data;
        }

        private void OnItemDeseleted()
        {
            OnItemSelected("");
            HideOptionsBox();
        }

        private VisualElement GetPreview(string buildingId)
        {
            return BuildingHelper.GetPreview(catalog.templates[buildingId]);
        }

        private void OnImportCatalog(MouseUpEvent evt)
        {
            var importType = EditorUtility.DisplayDialogComplex("Import Catalog",
                "You are about to import a catalog from a file. Would you like it to replace the existing catalog, or to be merged with it?",
                "Replace",
                "Merge",
                "Cancel");
            if (importType == 2) return; // Cancel

            var newCatalog = new PersistedBuildingsCatalog();
            var success = LoadCatalogFile(ref newCatalog);

            if (importType == 0 && success) // Replace
            {
                catalog = newCatalog;
                PersistedBuildingsCatalog.Set(catalog);
            }

            if (importType == 1 && success) // Merge
            {
                catalog = MergeCatalogs(catalog, newCatalog);
                PersistedBuildingsCatalog.Set(catalog);
            }

            RenderPicker();
            OnItemDeseleted();
        }

        private void OnRemoveAllItems(MouseUpEvent evt)
        {
            var confirmed = EditorUtility.DisplayDialog("Remove All Items?",
                "You are about to remove all of the items from the catalog! Are you sure?", "Yes",
                "No");
            if (!confirmed) return;
            catalog.list = new List<CatalogItem>();
            catalog.templates = new Dictionary<string, BuildingReplacementData>();
            PersistedBuildingsCatalog.Set(catalog);
            RenderPicker();
            HideOptionsBox();
        }

        private void OnRestoreDefault(MouseUpEvent evt)
        {
            var confirmed = EditorUtility.DisplayDialog("Restore Default Catalog?",
                "You are about to restore the default catalog! Are you sure?", "Yes",
                "No");
            if (!confirmed) return;
            try
            {
                PersistedBuildingsCatalog.RestoreDefault();
                catalog = PersistedBuildingsCatalog.Get();
                RenderPicker();
                HideOptionsBox();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        private void OnAddNewItem(MouseUpEvent evt)
        {
            var newItem = new CatalogItem("new item");
            var newBuildingData = new BuildingReplacementData();
            catalog.list.Add(newItem);
            catalog.templates.Add(newItem.ID, newBuildingData);
            OnItemSelected(newItem.ID);
        }

        private void OnSaveItem(CatalogItem newItem)
        {
            var factionId = this.Query<IntegerField>("faction-id").First();
            var buildingType = this.Query<EnumField>("building-type").First();
            var quality = this.Query<SliderInt>("quality").First();
            selectedBuildingData.FactionId = (ushort)factionId.value;
            selectedBuildingData.BuildingType = Convert.ToInt32(buildingType.value);
            selectedBuildingData.Quality = (byte)quality.value;

            newItem.Label = newItem.Label == "" ? null : newItem.Label;
            var idChanged = objectId != newItem.ID;

            var index = catalog.list.FindIndex((item) => item.ID == newItem.ID);
            var inCatalog = catalog.list.FindIndex((item) => item.ID == newItem.ID) != -1;

            if (idChanged && inCatalog)
            {
                var confirmed = EditorUtility.DisplayDialog("Override?",
                    "An item with this ID already exists. Would you like to override it?", "Yes",
                    "No");
                if (!confirmed) return;
            }


            if (!inCatalog)
            {
                catalog.list.RemoveAt(selectedIndex);
                catalog.templates.Remove(selectedItem.ID);
                catalog.list.Add(newItem);
                catalog.templates.Add(newItem.ID, selectedBuildingData);
            }
            else if (idChanged)
            {
                catalog.list[index] = newItem;
                catalog.list.RemoveAt(selectedIndex);
                catalog.templates.Remove(selectedItem.ID);
                catalog.templates[newItem.ID] = selectedBuildingData;
            }
            else
            {
                catalog.list[index] = newItem;
                catalog.templates[newItem.ID] = selectedBuildingData;
            }

            PersistedBuildingsCatalog.Set(catalog);
            RenderPicker();
        }

        private void OnRemoveItem()
        {
            catalog.list.RemoveAt(selectedIndex);
            PersistedBuildingsCatalog.Set(catalog);
            OnItemDeseleted();
            RenderPicker();
        }

        private void ShowOptionsBox()
        {
            var optionsBox = this.Query<VisualElement>("options-box").First();
            optionsBox.RemoveFromClassList("hidden");
        }

        private void HideOptionsBox()
        {
            var optionsBox = this.Query<VisualElement>("options-box").First();
            optionsBox.AddToClassList("hidden");
        }

        private PersistedBuildingsCatalog MergeCatalogs(PersistedBuildingsCatalog catalog1,
            PersistedBuildingsCatalog catalog2)
        {
            foreach (var item in catalog2.list)
            {
                var oldIndex = catalog1.list.FindIndex((i) => i.ID == item.ID);
                if (oldIndex == -1)
                {
                    catalog1.list.Add(item);
                }
                else
                {
                    catalog1.list[oldIndex] = item;
                }

                catalog1.templates[item.ID] = catalog2.templates[item.ID];
            }

            return catalog1;
        }

        private Boolean LoadCatalogFile(ref PersistedBuildingsCatalog newCatalog)
        {
            var path = EditorUtility.OpenFilePanel("Import buildings catalog", WorldDataFolder, "json");

            if (String.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return false;
            }

            try
            {
                var catalogJson = File.ReadAllText(path);
                newCatalog = JsonConvert.DeserializeObject<PersistedBuildingsCatalog>(catalogJson);
                return true;
            }
            catch (ArgumentException e)
            {
                return false;
            }
        }

        private void SaveFile()
        {
            var fileContent = JsonConvert.SerializeObject(catalog);
            var path = EditorUtility.SaveFilePanel("Save", WorldDataFolder, exportFile, "json");
            File.WriteAllText(path, fileContent);
            OnItemDeseleted();
        }

        private void OnRemovedFromHierarchy()
        {
            var catalogItemElement = this.Query<CatalogItemElement>("catalog-item-element").First();
            catalogItemElement.OnSaveItem -= OnSaveItem;
            catalogItemElement.OnRemoveItem -= OnRemoveItem;

            var buildingDataElement = this.Query<BuildingDataElement>("building-data-element").First();
            buildingDataElement.changed -= OnChangeBuildingData;
        }
    }
}