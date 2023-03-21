// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Addons.RmbBlockEditor.Elements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class AddBuilding
    {
        private VisualElement visualElement = new VisualElement();
        private AddBuildingFromFile addBuildingFromFile = new AddBuildingFromFile();
        private string objectId = "";
        private ObjectPainter painterObject;
        private ObjectPicker pickerObject;
        private List<CatalogItem> catalog = PersistedBuildingsCatalog.List();

        public AddBuilding()
        {
            RenderTemplate();
            BindTabButtons();
            RenderObjectPicker();
            RenderPainter();
        }

        public VisualElement Render()
        {
            catalog = PersistedBuildingsCatalog.List();
            RenderObjectPicker();
            return visualElement;
        }

        private void RenderTemplate()
        {
            var wrapper = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/AddEditors/BuildingSpecific.uxml");
            visualElement.Add(wrapper.CloneTree());
            var fromTemplate = visualElement.Query<VisualElement>("from-template").First();
            var fromFile = visualElement.Query<VisualElement>("from-file").First();

            var tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/AddEditors/Template.uxml");
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
            modelPicker.Clear();

            pickerObject =
                new ObjectPicker(catalog, OnItemSelected, GetPreview, objectId);
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
            return BuildingHelper.GetDummyExterior(objectId);
        }

        private void OnItemSelected(string objectId)
        {
            this.objectId = objectId;
            RenderPainter();
        }

        private VisualElement GetPreview(string buildingId)
        {
            var previewObject = BuildingHelper.GetDummyExterior(buildingId);
            return new GoPreview(previewObject);
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
            var gameObject = BuildingHelper.GetBuildingGameObject(buildingId, position, rotation);

            var buildingsRoot = GameObject.Find("Buildings");
            gameObject.transform.parent = buildingsRoot.transform;
        }

        public void Destroy()
        {
            if (painterObject != null)
            {
                painterObject.Destroy();
            }

            if (addBuildingFromFile != null)
            {
                addBuildingFromFile.Destroy();
            }
        }
    }
}