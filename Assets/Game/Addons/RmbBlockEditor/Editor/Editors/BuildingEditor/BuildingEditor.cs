// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.IO;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Addons.RmbBlockEditor.Elements;
using DaggerfallWorkshop.Utility.AssetInjection;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using DaggerfallWorkshop.Game.Utility.WorldDataEditor;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class BuildingEditor : VisualElement
    {
        private const string WorldDataFolder = "/StreamingAssets/WorldData/";
        private readonly Building building;

        public BuildingEditor(Building building)
        {
            // Register a callback to be invoked after the element has been removed
            RegisterCallback<DetachFromPanelEvent>(evt => OnRemovedFromHierarchy());

            this.building = building;
            SetupTemplate();
            Initialize();
            RegisterCallbacks();
        }

        private void SetupTemplate()
        {
            var template =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/BuildingEditor/Template.uxml");
            Add(template.CloneTree());
        }

        private void Initialize()
        {
            var xPosField = this.Query<IntegerField>("building-x").First();
            var zPosField = this.Query<IntegerField>("building-z").First();
            var yPosField = this.Query<IntegerField>("building-y").First();
            var yRotationField = this.Query<IntegerField>("building-y-rotation").First();
            var buildingDataField = this.Query<BuildingDataElement>("building-data-element").First();
            var buildingData = building.GetBuildingData();
            var subRecord = building.GetSubRecord();

            xPosField.value = building.XPos;
            zPosField.value = building.ZPos;
            yPosField.value = building.ModelsYPos;
            yRotationField.value = building.YRotation;

            var newReplacementData = new BuildingReplacementData
            {
                FactionId = buildingData.FactionId,
                BuildingType = (int)buildingData.BuildingType,
                Quality = buildingData.Quality,
                NameSeed = buildingData.NameSeed,
                RmbSubRecord = subRecord
            };

            buildingDataField.SetData(newReplacementData);
        }

        private void RegisterCallbacks()
        {
            var exportButton = this.Query<Button>("export-building").First();
            var openInWorldDataEditorButton = this.Query<Button>("open-in-world-data-editor").First();
            var xPosField = this.Query<IntegerField>("building-x").First();
            var zPosField = this.Query<IntegerField>("building-z").First();
            var yPosField = this.Query<IntegerField>("building-y").First();
            var yRotationField = this.Query<IntegerField>("building-y-rotation").First();
            var buildingDataField = this.Query<BuildingDataElement>("building-data-element").First();

            xPosField.RegisterCallback<ChangeEvent<IntegerField>>(evt =>
            {
                var fieldVal = ((IntegerField)evt.currentTarget).value;
                building.XPos = fieldVal;
            }, TrickleDown.TrickleDown);

            zPosField.RegisterCallback<ChangeEvent<IntegerField>>(evt =>
            {
                var fieldVal = ((IntegerField)evt.currentTarget).value;
                building.ZPos = fieldVal;
            }, TrickleDown.TrickleDown);

            yPosField.RegisterCallback<ChangeEvent<IntegerField>>(evt =>
            {
                var fieldVal = ((IntegerField)evt.currentTarget).value;
                building.ModelsYPos = fieldVal;
            }, TrickleDown.TrickleDown);

            yRotationField.RegisterCallback<ChangeEvent<IntegerField>>(evt =>
            {
                var fieldVal = ((IntegerField)evt.currentTarget).value;
                building.YRotation = fieldVal;
            }, TrickleDown.TrickleDown);

            UnregisterCallbacks();

            buildingDataField.changedBuildingData += HandleBuildingDataChange;
            buildingDataField.changedSubRecord += HandleSubRecordChange;
            exportButton.clicked += ExportToFile;

            if (openInWorldDataEditorButton != null)
            {
                openInWorldDataEditorButton.clicked += OpenInWorldDataEditor; // Make sure your method has the correct signature
            }
        }

        private void HandleBuildingDataChange(BuildingReplacementData newData)
        {
            building.FactionId = newData.FactionId;
            building.BuildingType = (DFLocation.BuildingTypes)newData.BuildingType;
            building.Quality = newData.Quality;
            building.NameSeed = newData.NameSeed;
        }

        private void HandleSubRecordChange(BuildingReplacementData newData)
        {
            building.SetSubRecord(newData.RmbSubRecord);
        }

        private void ExportToFile()
        {
            var index = building.transform.GetSiblingIndex();
            var rmbBlock = building.transform.GetComponentInParent<RmbBlockObject>();

            var buildingDataField = this.Query<BuildingDataElement>("building-data-element").First();
            var fileName = string.Format("{0}-{1}-building{2}", rmbBlock.Name, rmbBlock.Index, index);
            var path = EditorUtility.SaveFilePanel("Save as", WorldDataFolder, fileName, "json");
            RmbBlockHelper.SaveBuildingFile(buildingDataField.GetData(), path);
        }

        private void OpenInWorldDataEditor()
        {
            // Generate a temporary file path
            string tempDirectory = Path.Combine(Application.dataPath, "Temp");
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }
            var index = building.transform.GetSiblingIndex();
            var rmbBlock = building.transform.GetComponentInParent<RmbBlockObject>();
            var fileName = string.Format("temp_{0}-{1}-building{2}.json", rmbBlock.Name, rmbBlock.Index, index);
            var path = Path.Combine(tempDirectory, fileName);

            // Use the same method to get building data as in ExportToFile
            var buildingDataField = this.Query<BuildingDataElement>("building-data-element").First();
            BuildingReplacementData buildingData = buildingDataField.GetData();

            // Use RmbBlockHelper to save the file, ensuring all necessary data is serialized
            RmbBlockHelper.SaveBuildingFile(buildingData, path);

            try
            {
                // Attempt to open the saved file with the World Data Editor
                WorldDataEditor worldDataEditorWindow = (WorldDataEditor)EditorWindow.GetWindow(typeof(WorldDataEditor), true, "WorldData Editor");
                if (worldDataEditorWindow != null)
                {
                    worldDataEditorWindow.OpenBuildingFile(path);
                }
                else
                {
                    Debug.LogError("Failed to open the World Data Editor.");
                }
            }
            finally
            {
                // Ensure the temporary file is deleted after opening it
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private void UnregisterCallbacks()
        {
            var exportButton = this.Query<Button>("export-building").First();
            var openInWorldDataEditorButton = this.Query<Button>("open-in-world-data-editor").First();
            var buildingDataField = this.Query<BuildingDataElement>("building-data-element").First();

            // Detach the event handlers
            exportButton.clicked -= ExportToFile;
            openInWorldDataEditorButton.clicked -= OpenInWorldDataEditor; // Detach the new button's click event

            buildingDataField.changedBuildingData -= HandleBuildingDataChange;
            buildingDataField.changedSubRecord -= HandleSubRecordChange;
        }

        private void OnRemovedFromHierarchy()
        {
            UnregisterCallbacks();
        }
    }
}
