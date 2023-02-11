// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class SettingsEditor
    {
        private VisualElement visualElement;
        private GameObject go;

        public SettingsEditor()
        {
            visualElement = new VisualElement();
        }

        public VisualElement Render(GameObject go)
        {
            this.go = go;
            RenderTemplate();
            BindFields();
            BindButton();
            return visualElement;
        }

        private void RenderTemplate()
        {
            visualElement.Clear();
            var rmbPropsTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/SettingsEditor/Template.uxml");
            visualElement.Add(rmbPropsTree.CloneTree());
        }

        private void BindFields()
        {
            if (go == null)
            {
                return;
            }

            // Get serialized properties:
            var rmbBlock = go.GetComponent<RmbBlockObject>();
            var so = new SerializedObject(rmbBlock);
            var climateProperty = so.FindProperty("Climate");
            var seasonProperty = so.FindProperty("Season");
            var windowStyleProperty = so.FindProperty("WindowStyle");

            // Select the bindable fields:
            var climateField = visualElement.Query<EnumField>("climate").First();
            var seasonField = visualElement.Query<EnumField>("season").First();
            var windowStyleField = visualElement.Query<EnumField>("window-style").First();

            // Bind the fields to the properties:
            climateField.BindProperty(climateProperty);
            seasonField.BindProperty(seasonProperty);
            windowStyleField.BindProperty(windowStyleProperty);
        }

        private void BindButton()
        {
            var rmbBlock = go.GetComponent<RmbBlockObject>();
            var button = visualElement.Query<Button>("apply").First();
            var allModels = go.GetComponentsInChildren<DaggerfallMesh>();
            var ground = go.GetComponentsInChildren<Ground>().First();
            var scenery = go.GetComponentsInChildren<Scenery>().First();
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                foreach (var daggerfallMesh in allModels)
                {
                    daggerfallMesh.SetClimate(rmbBlock.Climate, rmbBlock.Season, rmbBlock.WindowStyle);
                }
                ground.SetClimate(rmbBlock.Climate, rmbBlock.Season);
                scenery.SetClimate(rmbBlock.Climate, rmbBlock.Season);
            }, TrickleDown.TrickleDown);
        }
    }
}