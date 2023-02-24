using System;
using DaggerfallWorkshop.Game.Addons.RmbBlockEditor.BuildingPresets;
using DaggerfallWorkshop.Utility.AssetInjection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class ModifyBuildingFromFile
    {
        private const string WorldDataFolder = "/StreamingAssets/WorldData/";
        private VisualElement visualElement;
        private BuildingPreset buildingHelper;
        private BuildingReplacementData building;
        private GameObject previewGameObject;
        private Editor previewEditor;
        private Action<BuildingReplacementData, Boolean, Boolean> onModify;

        public ModifyBuildingFromFile(BuildingPreset buildingHelper, Action<BuildingReplacementData, Boolean, Boolean> onModify)
        {
            visualElement = new VisualElement();
            this.buildingHelper = buildingHelper;
            this.onModify = onModify;
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
                "Assets/Game/Addons/RmbBlockEditor/Editor/ModifyBuildingEditor/BuildingFromFile.uxml");
            visualElement.Add(tree.CloneTree());
        }

        private void RenderFileLoader()
        {
            var loadFile = visualElement.Query<Button>("load-file").First();
            loadFile.RegisterCallback<MouseUpEvent>(evt =>
                {
                    LoadFile();
                    RenderPreview();
                    RenderApplyBox();
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

        private void RenderApplyBox()
        {
            var box = visualElement.Query<Box>("apply-modification-box").First();
            box.RemoveFromClassList("hidden");

            var interior = visualElement.Query<Toggle>("interior").First();
            var exterior = visualElement.Query<Toggle>("exterior").First();

            var button = visualElement.Query<Button>("apply-modification-from-file").First();
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                onModify(building, interior.value, exterior.value);
            });
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
    }
}