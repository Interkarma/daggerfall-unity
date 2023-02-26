// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Addons.RmbBlockEditor.BuildingPresets;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class AddBuilding
    {
        private VisualElement visualElement;
        private BuildingPreset buildingHelper;
        private AddBuildingFromFile addBuildingFromFile;
        private string objectId;
        private ObjectPainter painterObject;
        private ObjectPicker pickerObject;

        public AddBuilding()
        {
            visualElement = new VisualElement();
            buildingHelper = new BuildingPreset();
            addBuildingFromFile = new AddBuildingFromFile(buildingHelper);
            objectId = "";
            RenderTemplate();
            BindTabButtons();
            RenderObjectPicker();
            RenderPainter();
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
                    "Assets/Game/Addons/RmbBlockEditor/Editor/AddEditors/Template.uxml");
            fromTemplate.Add(tree.CloneTree());
            fromFile.Add(addBuildingFromFile.Render());
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

        private void RenderPainter()
        {
            var painterContainer = visualElement.Query<VisualElement>("painter-container").First();
            painterContainer.Clear();
            if (painterObject != null)
            {
                painterObject.Destroy();
            }

            if (objectId != "")
            {
                painterObject =
                    new BuildingPainter(AddPlaceholder, AddBuildingObject);
                painterContainer.Add(painterObject.visualElement);
            }
        }

        private GameObject AddPlaceholder()
        {
            return AddPreview(objectId);
        }

        private void OnItemSelected(string objectId)
        {
            this.objectId = objectId;
            RenderPainter();
        }

        private GameObject AddPreview(string buildingId)
        {
            var previewObject = buildingHelper.AddBuildingPlaceholder(buildingId);
            return previewObject;
        }

        private void AddBuildingObject(Vector3 position, Vector3 rotation)
        {
            try
            {
                AddBuildingObject(objectId, position, rotation);
            }
            catch (Exception e)
            {
                Console.WriteLine("No such Object ID is available.");
            }
        }

        private void AddBuildingObject(string buildingId, Vector3 position, Vector3 rotation)
        {
            var gameObject = buildingHelper.AddBuildingObject(buildingId, position, rotation);

            var buildingsRoot = GameObject.Find("Buildings");
            gameObject.transform.parent = buildingsRoot.transform;
        }

        public void Destroy()
        {
            if (painterObject != null)
            {
                painterObject.Destroy();
            }
            if (pickerObject != null)
            {
                pickerObject.Destroy();
            }
            if (addBuildingFromFile != null)
            {
                addBuildingFromFile.Destroy();
            }
        }
    }
}