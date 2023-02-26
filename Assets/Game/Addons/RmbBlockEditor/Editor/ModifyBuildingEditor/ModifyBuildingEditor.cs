// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Addons.RmbBlockEditor.BuildingPresets;
using DaggerfallWorkshop.Utility.AssetInjection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class ModifyBuildingEditor
    {
        private readonly VisualElement visualElement;
        private readonly BuildingPreset buildingHelper;
        private readonly ModifyBuildingFromFile modifyBuildingFromFile;
        private string objectId;
        private ObjectPicker pickerObject;
        private readonly GameObject oldGo;

        public ModifyBuildingEditor(GameObject oldGo)
        {
            visualElement = new VisualElement();
            buildingHelper = new BuildingPreset();
            modifyBuildingFromFile = new ModifyBuildingFromFile(buildingHelper, Modify);
            this.oldGo = oldGo;

            RenderTemplate();
            BindTabButtons();
            RenderObjectPicker();
            BindApplyButton();
        }

        public VisualElement Render()
        {
            return visualElement;
        }

        private void RenderTemplate()
        {
            var wrapper = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Game/Addons/RmbBlockEditor/Editor/AddEditors/BuildingSpecific.uxml");
            visualElement.Add(wrapper.CloneTree());
            var fromTemplate = visualElement.Query<VisualElement>("from-template").First();
            var fromFile = visualElement.Query<VisualElement>("from-file").First();

            var tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/ModifyBuildingEditor/Template.uxml");
            fromTemplate.Add(tree.CloneTree());
            fromFile.Add(modifyBuildingFromFile.Render());
        }

        private void BindTabButtons()
        {
            var templateTab = visualElement.Query<Button>("template-tab").First();
            var fileTab = visualElement.Query<Button>("file-tab").First();
            var fromTemplate = visualElement.Query<VisualElement>("from-template").First();
            var fromFile = visualElement.Query<VisualElement>("from-file").First();

            fileTab.RegisterCallback<MouseUpEvent>(evt =>
                {
                    templateTab.RemoveFromClassList("selected");
                    fromTemplate.AddToClassList("hidden");

                    fileTab.AddToClassList("selected");
                    fromFile.RemoveFromClassList("hidden");
                },
                TrickleDown.TrickleDown);

            templateTab.RegisterCallback<MouseUpEvent>(evt =>
                {
                    templateTab.AddToClassList("selected");
                    fromTemplate.RemoveFromClassList("hidden");

                    fileTab.RemoveFromClassList("selected");
                    fromFile.AddToClassList("hidden");
                },
                TrickleDown.TrickleDown);
        }

        private void RenderObjectPicker()
        {
            var modelPicker = visualElement.Query<VisualElement>("object-picker").First();
            var categories = new Dictionary<string, Dictionary<string, string>>
            {
                { "Houses", BuildingPresetData.houses },
                { "Shops", BuildingPresetData.shops },
                { "Services", BuildingPresetData.services },
                { "Guilds", BuildingPresetData.guilds },
                { "Others", BuildingPresetData.others },
            };

            if (pickerObject != null)
            {
                pickerObject.Destroy();
            }

            pickerObject =
                new ObjectPicker(categories, BuildingPresetData.buildingGroups,
                    true, OnItemSelected, AddPreview);
            modelPicker.Add(pickerObject.visualElement);
        }

        private void BindApplyButton()
        {
            var interior = visualElement.Query<Toggle>("interior").First();
            var exterior = visualElement.Query<Toggle>("exterior").First();

            var button = visualElement.Query<Button>("apply-modification").First();
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                Modify(objectId, interior.value, exterior.value);
                if (pickerObject != null)
                {
                    pickerObject.Destroy();
                }
            });
        }

        private void Modify(string buildingId, Boolean interior, Boolean exterior)
        {
            var currentBuilding = oldGo.GetComponent<Building>();
            var newGo = buildingHelper.ReplaceBuildingObject(buildingId, currentBuilding, interior, exterior);
            Modify(newGo);
        }

        private void Modify(BuildingReplacementData building, Boolean interior, Boolean exterior)
        {
            var currentBuilding = oldGo.GetComponent<Building>();
            var newGo = buildingHelper.ReplaceBuildingObject(building, currentBuilding, interior, exterior);
            Modify(newGo);
        }

        private void Modify(GameObject newBuildingGo)
        {
            try
            {
                newBuildingGo.transform.parent = oldGo.transform.parent;
                Object.DestroyImmediate(oldGo);
                Selection.SetActiveObjectWithContext(newBuildingGo, null);
            }
            catch (Exception error)
            {
                Debug.LogError(error);
            }
        }

        private void OnItemSelected(string buildingId)
        {
            var box = visualElement.Query<Box>("apply-modification-box").First();
            box.RemoveFromClassList("hidden");
            objectId = buildingId;
        }

        private GameObject AddPreview(string buildingId)
        {
            var previewObject = buildingHelper.AddBuildingPlaceholder(buildingId);
            return previewObject;
        }
    }
}