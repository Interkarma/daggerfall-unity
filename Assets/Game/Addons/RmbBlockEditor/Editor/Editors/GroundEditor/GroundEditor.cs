// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class GroundEditor
    {
        public VisualElement visualElement;
        private GameObject gameObject;
        private bool paintMode;

        public GroundEditor(GameObject go)
        {
            gameObject = go;
            visualElement = new VisualElement();
            paintMode = false;
        }

        public VisualElement Render()
        {
            visualElement.Clear();
            RenderTemplate();
            RenderModeButton();
            ShowSelectedEditor();
            return visualElement;
        }

        private void RenderTemplate()
        {
            var tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/GroundEditor/Template.uxml");
            visualElement.Add(tree.CloneTree());
        }

        // Renders a button to change between selection mode and paint mode
        private void RenderModeButton()
        {
            var button = visualElement.Query<Button>("ground-edit-mode").First();
            button.text = paintMode ? "To Selection Mode" : "To Paint Mode";
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                paintMode = !paintMode;
                Render();
            }, TrickleDown.TrickleDown);
        }

        // Shows the appropriate editor depending on the the selected edit mode
        private void ShowSelectedEditor()
        {
            var modeContainer = visualElement.Query<VisualElement>("ground-edit-mode-container").First();
            modeContainer.Clear();
            if (paintMode)
            {
                var paintEditor = new GroundPaintEditor(gameObject);
                modeContainer.Add(paintEditor.Render());
            }
            else
            {
                var selectionEditor = new GroundSelectionEditor(gameObject);
                modeContainer.Add(selectionEditor.Render());
            }
        }
    }
}