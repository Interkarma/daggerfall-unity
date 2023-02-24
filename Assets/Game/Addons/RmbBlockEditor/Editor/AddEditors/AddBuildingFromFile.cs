using System;
using DaggerfallWorkshop.Game.Addons.RmbBlockEditor.BuildingPresets;
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
        private BuildingPreset buildingHelper;
        private BuildingReplacementData building;
        private GameObject previewGameObject;
        private Editor previewEditor;
        private ObjectPainter painterObject;

        public AddBuildingFromFile(BuildingPreset buildingHelper)
        {
            visualElement = new VisualElement();
            this.buildingHelper = buildingHelper;
            previewGameObject = null;
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
                "Assets/Game/Addons/RmbBlockEditor/Editor/AddEditors/BuildingFromFile.uxml");
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
            previewGameObject.name = "Object Preview";
            // We only need the previewGameObject for the preview element, so
            // make it really small, so it doesn't get in the way in the scene view.
            previewGameObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

            var preview = visualElement.Query<VisualElement>("file-preview").First();
            preview.Clear();

            Editor.DestroyImmediate(previewEditor);

            if (previewGameObject != null)
            {
                previewEditor = Editor.CreateEditor(previewGameObject);
                var previewImage = new IMGUIContainer(() =>
                {
                    previewEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(500, 450), GUIStyle.none);
                });
                preview.Add(previewImage);
            }
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
                previewGameObject = AddPreview();
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private GameObject AddPreview()
        {
            return buildingHelper.AddBuildingPlaceholder(building.RmbSubRecord);
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
            var gameObject = buildingHelper.AddBuildingObject(building, position, rotation);

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