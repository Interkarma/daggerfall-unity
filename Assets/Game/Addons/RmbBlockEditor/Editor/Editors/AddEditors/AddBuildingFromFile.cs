// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using DaggerfallWorkshop.Utility.AssetInjection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class AddBuildingFromFile
    {
        private const string WorldDataFolder = "/StreamingAssets/WorldData/";
        private VisualElement visualElement;
        private BuildingReplacementData building;
        private Editor previewEditor;
        private ObjectPainter painterObject;

        public AddBuildingFromFile()
        {
            visualElement = new VisualElement();
            RenderTemplate();
            RenderFileLoader();
        }

        public VisualElement Render()
        {
            return visualElement;
        }

        private void RenderTemplate()
        {
            var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/AddEditors/BuildingFromFile.uxml");
            visualElement.Add(tree.CloneTree());
        }

        private void RenderFileLoader()
        {
            var loadFile = visualElement.Query<Button>("load-file").First();
            loadFile.RegisterCallback<MouseUpEvent>(evt =>
                {
                    LoadFile();
                    RenderPreview();
                    RenderPainter();
                },
                TrickleDown.TrickleDown);
        }

        private void RenderPreview()
        {
            var preview = visualElement.Query<VisualElement>("file-preview").First();
            preview.Clear();
            preview.Add(BuildingHelper.GetPreview(building));
        }

        private void RenderPainter()
        {
            var painterContainer = visualElement.Query<VisualElement>("file-painter-container").First();
            painterContainer.Clear();
            if (painterObject != null)
            {
                painterObject.Destroy();
            }

            if (!building.Equals(default(BuildingReplacementData)))
            {
                painterObject =
                    new BuildingPainter(AddPreview, AddBuildingObject);
                painterContainer.Add(painterObject.visualElement);
            }
        }

        private void LoadFile()
        {
            var path = EditorUtility.OpenFilePanel("Open Building File", WorldDataFolder, "json");
            if (String.IsNullOrEmpty(path))
            {
                return;
            }

            try
            {
                building = RmbBlockHelper.LoadBuildingFile(path);
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private GameObject AddPreview()
        {
            return BuildingHelper.GetDummyExterior(building);
        }

        private void AddBuildingObject(Vector3 position, Vector3 rotation)
        {
            try
            {
                AddBuildingObject(building, position, rotation);
            }
            catch (Exception e)
            {
                Console.WriteLine("No such Object ID is available.");
            }
        }

        private void AddBuildingObject(BuildingReplacementData building, Vector3 position, Vector3 rotation)
        {
            var gameObject = BuildingHelper.GetBuildingGameObject(building, position, rotation);

            var buildingsRoot = GameObject.Find("Buildings");
            gameObject.transform.parent = buildingsRoot.transform;
        }

        public void Destroy()
        {
            if (painterObject != null)
            {
                painterObject.Destroy();
            }
        }
    }
}