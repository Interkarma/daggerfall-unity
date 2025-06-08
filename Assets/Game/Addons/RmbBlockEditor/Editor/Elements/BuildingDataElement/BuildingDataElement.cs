// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.IO;
using System.Threading.Tasks;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility.AssetInjection;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using DaggerfallWorkshop.Game.Utility.WorldDataEditor;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor.Elements
{
    public class BuildingDataElement : VisualElement
    {
        private const string WorldDataFolder = "/StreamingAssets/WorldData/";
        private BuildingReplacementData data;
        private Action<BuildingReplacementData> change;
        private Action<BuildingReplacementData> changeBuildingData;
        private Action<BuildingReplacementData> changeSubRecord;
        private Debouncer _debouncer = new Debouncer();
        private string selectedCatalogItem = "";

        public void SetData(BuildingReplacementData data)
        {
            this.data = data;
            Initialize();
        }

        public BuildingReplacementData GetData()
        {
            return data;
        }

        public new class UxmlFactory : UxmlFactory<BuildingDataElement, UxmlTraits>
        {
        }

        public BuildingDataElement()
        {
            // Register a callback to be invoked after the element has been removed
            RegisterCallback<DetachFromPanelEvent>(evt => OnRemovedFromHierarchy());

            var template =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Elements/BuildingDataElement/Template.uxml");
            Add(template.CloneTree());
        }

        public event Action<BuildingReplacementData> changed
        {
            add => change += value;
            remove => change -= value;
        }

        public event Action<BuildingReplacementData> changedBuildingData
        {
            add => changeBuildingData += value;
            remove => changeBuildingData -= value;
        }

        public event Action<BuildingReplacementData> changedSubRecord
        {
            add => changeSubRecord += value;
            remove => changeSubRecord -= value;
        }

        public void HideCatalogImport()
        {
            var replaceFromCatalogButton = this.Query<Button>("replace-from-catalog-button").First();
            replaceFromCatalogButton.AddToClassList("hidden");
        }

        private void Initialize()
        {
            UnregisterCallbacks();

            // Get field references
            var factionIdField = this.Query<IntegerField>("faction-id").First();
            var buildingTypeField = this.Query<EnumField>("building-type").First();
            var qualitySlider = this.Query<SliderInt>("quality").First();
            var qualityField = this.Query<IntegerField>("quality-input").First();
            buildingTypeField.Init(DFLocation.BuildingTypes.House1);

            factionIdField.value = data.FactionId;
            buildingTypeField.value = (DFLocation.BuildingTypes)data.BuildingType;
            qualitySlider.value = data.Quality;
            qualityField.value = data.Quality;

            // Show exterior thumbnail
            var exteriorThumb = this.Query<VisualElement>("exterior-thumbnail").First();
            exteriorThumb.Clear();
            exteriorThumb.Add(BuildingHelper.GetExteriorPreview(data));

            // Show interior thumbnail
            var interiorThumb = this.Query<VisualElement>("interior-thumbnail").First();
            interiorThumb.Clear();
            interiorThumb.Add(BuildingHelper.GetInteriorPreview(data));

            // Set the containers visibility
            HideReplaceFromFile();

            RegisterCallbacks();
        }

        private void RegisterCallbacks()
        {
            // Get element references
            var factionIdField = this.Query<IntegerField>("faction-id").First();
            var buildingTypeField = this.Query<EnumField>("building-type").First();
            var qualitySlider = this.Query<SliderInt>("quality").First();
            var qualityField = this.Query<IntegerField>("quality-input").First();
            var replaceFromFileButton = this.Query<Button>("replace-from-file-button").First();
            var importFromFile = this.Query<Button>("import-from-file").First();
            var importFromWDE = this.Query<Button>("import-from-WDE").First();
            var cancelReplaceFromFile = this.Query<Button>("cancel-replace-from-file").First();
            var replaceFromCatalogButton = this.Query<Button>("replace-from-catalog-button").First();
            var importFromCatalog = this.Query<Button>("import-from-catalog").First();
            var cancelReplaceFromCatalog = this.Query<Button>("cancel-replace-from-catalog").First();

            // Register field callbacks
            factionIdField.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                var factionId = ((IntegerField)evt.currentTarget).value;
                data.FactionId = (byte)factionId;
                changeBuildingData?.Invoke(data);
                change?.Invoke(data);
            }, TrickleDown.TrickleDown);
            buildingTypeField.RegisterCallback<ChangeEvent<Enum>>(evt =>
            {
                var buildingType = ((EnumField)evt.currentTarget).value;
                data.BuildingType = Convert.ToInt32(buildingType);
                changeBuildingData?.Invoke(data);
                change?.Invoke(data);
            }, TrickleDown.TrickleDown);
            qualitySlider.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                var quality = ((SliderInt)evt.currentTarget).value;
                qualityField.value = quality;
            }, TrickleDown.TrickleDown);
            qualityField.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                var quality = ((IntegerField)evt.currentTarget).value;
                data.Quality = (byte)quality;
                changeBuildingData?.Invoke(data);
                change?.Invoke(data);
            }, TrickleDown.TrickleDown);

            // Register button callbacks
            replaceFromFileButton.clicked += ShowReplaceFromFile;
            replaceFromCatalogButton.clicked += ShowReplaceFromCatalog;
            importFromFile.clicked += OnImportFromFile;
            cancelReplaceFromFile.clicked += HideReplaceFromFile;
            importFromCatalog.clicked += OnImportFromCatalog;
            importFromWDE.clicked += ImportFromWorldDataEditor;
            cancelReplaceFromCatalog.clicked += HideReplaceFromCatalog;
        }

        private async Task ScrollToImportFromFile()
        {
            // Get reference to importButton and scrollView
            var importButton = this.Query<Button>("import-from-file").First();
            var scrollView = importButton.GetFirstAncestorOfType<ScrollView>();

            scrollView.ScrollTo(importButton);
            await Task.CompletedTask; // Return a completed task.
        }

        private async Task ScrollToImportFromCatalog()
        {
            // Get reference to importButton and scrollView
            var importButton = this.Query<Button>("import-from-catalog").First();
            var scrollView = importButton.GetFirstAncestorOfType<ScrollView>();

            scrollView.ScrollTo(importButton);
            await Task.CompletedTask; // Return a completed task.
        }

        private void ShowReplaceFromFile()
        {
            // Hide the replace from catalog container
            HideReplaceFromCatalog();

            // Show replaceFromFileContainer
            var replaceFromFileContainer = this.Query<VisualElement>("replace-from-file-container").First();
            replaceFromFileContainer.RemoveFromClassList("hidden");

            // The following line uses a #pragma directive to suppress the CS4014 warning because we intentionally
            // do not await the Debounce method call here. The Debounce method handles asynchronous execution itself.
            #pragma warning disable CS4014
            // Give the hidden container time to repaint, so the scrollView can calculate the height correctly
            _debouncer.Debounce(ScrollToImportFromFile, 10);
            #pragma warning restore CS4014
        }

        private void ShowReplaceFromCatalog()
        {
            // Hide the replace from file container
            HideReplaceFromFile();

            // Show replaceFromFileContainer
            var replaceFromCatalogContainer = this.Query<VisualElement>("replace-from-catalog-container").First();
            replaceFromCatalogContainer.RemoveFromClassList("hidden");

            // Show the object picker
            var objectPickerContainer = this.Query<VisualElement>("object-picker-container").First();
            objectPickerContainer.Clear();
            var catalog = PersistedBuildingsCatalog.List();
            var pickerObject = new ObjectPicker(catalog, OnCatalogItemSelected, GetPreview);
            objectPickerContainer.Add(pickerObject.visualElement);

            // The following line uses a #pragma directive to suppress the CS4014 warning because we intentionally
            // do not await the Debounce method call here. The Debounce method handles asynchronous execution itself.
            #pragma warning disable CS4014
            // Give the hidden container time to repaint, so the scrollView can calculate the height correctly
            _debouncer.Debounce(ScrollToImportFromCatalog, 10);
            #pragma warning restore CS4014
        }

        private void HideReplaceFromFile()
        {
            var replaceFromFileContainer = this.Query<VisualElement>("replace-from-file-container").First();
            replaceFromFileContainer.AddToClassList("hidden");
        }

        private void HideReplaceFromCatalog()
        {
            var replaceFromCatalogContainer = this.Query<VisualElement>("replace-from-catalog-container").First();
            replaceFromCatalogContainer.AddToClassList("hidden");
        }

        private void OnImportFromFile()
        {
            var importProps = this.Query<Toggle>("import-props-from-file").First();
            var importExterior = this.Query<Toggle>("import-exterior-from-file").First();
            var importInterior = this.Query<Toggle>("import-interior-from-file").First();

            var loadedData = new BuildingReplacementData();
            var success = LoadBuildingFile(ref loadedData);
            if (!success) return;

            Import(loadedData, importProps.value, importExterior.value, importInterior.value);
        }

        private void OnImportFromCatalog()
        {
            var templates = PersistedBuildingsCatalog.Templates();

            var importProps = this.Query<Toggle>("import-props-from-catalog").First();
            var importExterior = this.Query<Toggle>("import-exterior-from-catalog").First();
            var importInterior = this.Query<Toggle>("import-interior-from-catalog").First();

            var buildingData = templates[selectedCatalogItem];
            Import(buildingData, importProps.value, importExterior.value, importInterior.value);
        }

        private void Import(BuildingReplacementData replacementData, bool importProps, bool importExterior,
            bool importInterior)
        {
            if (!importProps && !importExterior && !importInterior)
            {
                return;
            }

            if (importProps)
            {
                data.FactionId = replacementData.FactionId;
                data.BuildingType = replacementData.BuildingType;
                data.Quality = replacementData.Quality;
                changeBuildingData?.Invoke(data);
            }

            if (importExterior)
            {
                data.RmbSubRecord.Exterior = replacementData.RmbSubRecord.Exterior;
            }

            if (importInterior)
            {
                data.RmbSubRecord.Interior = replacementData.RmbSubRecord.Interior;
            }

            if (importExterior || importInterior)
            {
                changeSubRecord?.Invoke(data);
            }

            change?.Invoke(data);
            Initialize();
        }

        private void OnCatalogItemSelected(string objectId)
        {
            selectedCatalogItem = objectId;
        }

        private VisualElement GetPreview(string buildingId)
        {
            var templates = PersistedBuildingsCatalog.Templates();
            return BuildingHelper.GetPreview(templates[buildingId]);
        }

        private bool LoadBuildingFile(ref BuildingReplacementData buildingData, string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = EditorUtility.OpenFilePanel("Import buildings", WorldDataFolder, "json");
            }

            if (!File.Exists(path))
            {
                Debug.LogError($"File does not exist: {path}");
                return false;
            }
            
            try
            {
                var buildingJson = File.ReadAllText(path);
                buildingData = JsonConvert.DeserializeObject<BuildingReplacementData>(buildingJson);
                return true;
            }
            catch (ArgumentException e)
            {
                Debug.Log(e);
                return false;
            }
        }

        private void ImportFromWorldDataEditor()
        {
            Debug.Log("ImportFromWorldDataEditor is being called.");

            // Attempt to get an open instance of the WorldDataEditor window
            WorldDataEditor worldDataEditor = (WorldDataEditor)EditorWindow.GetWindow(typeof(WorldDataEditor), false, "WorldData Editor", false);
            if (worldDataEditor != null)
            {
                // Generate a temporary file path using temporaryCachePath for temporary files
                string tempDirectory = Path.Combine(Application.temporaryCachePath, "Temp");
                Directory.CreateDirectory(tempDirectory); // CreateDirectory checks for existence internally

                var fileName = $"temp_building.json";
                var path = Path.Combine(tempDirectory, fileName);

                try
                {
                    // Ensure to update building data before saving
                    worldDataEditor.UpdateBuildingWorldData();

                    // Access the buildingData from the WorldDataEditor instance to save it
                    BuildingReplacementData buildingData = worldDataEditor.buildingData;
                    WorldDataEditorBuildingHelper.SaveBuildingFile(buildingData, path);

                    // Attempt to load the building data from the temporary file
                    var loadedData = new BuildingReplacementData();
                    var success = LoadBuildingFile(ref loadedData, path);
                    if (!success) return;

                    // Import the loaded data
                    Import(loadedData, true, true, true);
                }
                finally
                {
                    // Ensure the temporary file is deleted after use
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }
            else
            {
                Debug.LogError("WorldDataEditor is not open or accessible.");
            }
        }

        private void UnregisterCallbacks()
        {
            changeBuildingData = null;
            changeSubRecord = null;
            change = null;
            var replaceFromFileButton = this.Query<Button>("replace-from-file-button").First();
            var replaceFromCatalogButton = this.Query<Button>("replace-from-catalog-button").First();
            var importFromFile = this.Query<Button>("import-from-file").First();
            var importFromWDE = this.Query<Button>("import-from-WDE").First();
            var cancelReplaceFromFile = this.Query<Button>("cancel-replace-from-file").First();
            var importFromCatalog = this.Query<Button>("import-from-catalog").First();
            var cancelReplaceFromCatalog = this.Query<Button>("cancel-replace-from-catalog").First();

            replaceFromFileButton.clicked -= ShowReplaceFromFile;
            replaceFromCatalogButton.clicked -= ShowReplaceFromCatalog;
            importFromFile.clicked -= OnImportFromFile;
            importFromWDE.clicked -= ImportFromWorldDataEditor;
            cancelReplaceFromFile.clicked -= HideReplaceFromFile;
            importFromCatalog.clicked -= OnImportFromCatalog;
            cancelReplaceFromCatalog.clicked -= HideReplaceFromCatalog;
        }

        private void OnRemovedFromHierarchy()
        {
            UnregisterCallbacks();
        }
    }
}
