// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class ClimateSettingsEditor
    {
        private VisualElement visualElement;
        private GameObject go;

        public ClimateSettingsEditor()
        {
            visualElement = new VisualElement();
        }

        public VisualElement Render(GameObject go)
        {
            this.go = go;
            RenderTemplate();
            Initialize();
            BindButton();
            return visualElement;
        }

        private void RenderTemplate()
        {
            visualElement.Clear();
            var rmbPropsTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/ClimateSettingsEditor/Template.uxml");
            visualElement.Add(rmbPropsTree.CloneTree());
        }

        private void Initialize()
        {
            var climateField = visualElement.Query<EnumField>("climate").First();
            var seasonField = visualElement.Query<EnumField>("season").First();
            var windowStyleField = visualElement.Query<EnumField>("window-style").First();

            climateField.Init(PersistedSettings.ClimateBases());
            seasonField.Init(PersistedSettings.ClimateSeason());
            windowStyleField.Init(PersistedSettings.WindowStyle());

        }

        private void BindButton()
        {
            var climateField = visualElement.Query<EnumField>("climate").First();
            var seasonField = visualElement.Query<EnumField>("season").First();
            var windowStyleField = visualElement.Query<EnumField>("window-style").First();

            var button = visualElement.Query<Button>("apply").First();
            var allModels = go.GetComponentsInChildren<DaggerfallMesh>();
            var ground = go.GetComponentsInChildren<Ground>().First();

            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                // Read the new values
                ClimateBases climate = (ClimateBases)climateField.value;
                ClimateSeason season = (ClimateSeason)seasonField.value;
                WindowStyle windowStyle = (WindowStyle)windowStyleField.value;

                // Update the settings singleton
                PersistedSettings.SetClimate(climate, season, windowStyle);

                // Update all the meshes in the scene
                foreach (var daggerfallMesh in allModels)
                {
                    daggerfallMesh.SetClimate(climate, season, windowStyle);
                }

                ground.Update();

            }, TrickleDown.TrickleDown);
        }
    }
}