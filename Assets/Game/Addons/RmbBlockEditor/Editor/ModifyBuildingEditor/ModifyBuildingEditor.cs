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
    public class ModifyBuildingEditor
    {
        private VisualElement visualElement;
        private BuildingPreset buildingHelper;
        private string objectId;
        private ObjectPicker pickerObject;
        private Action<string> apply;

        public ModifyBuildingEditor(Action<string> apply)
        {
            visualElement = new VisualElement();
            buildingHelper = new BuildingPreset();
            this.apply = apply;
            RenderTemplate();
            RenderObjectPicker();
            BindApplyButton();
        }

        public VisualElement Render(ClimateBases climate, ClimateSeason season, WindowStyle windowStyle)
        {
            buildingHelper.SetClimate(climate, season, windowStyle);
            return visualElement;
        }

        private void RenderTemplate()
        {
            var tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/ModifyBuildingEditor/Template.uxml");
            visualElement.Add(tree.CloneTree());
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
            var button = visualElement.Query<Button>("apply-modification").First();
            button.RegisterCallback<MouseUpEvent>(evt =>
            {
                apply(objectId);
                if (pickerObject != null)
                {
                    pickerObject.Destroy();
                }
            });
        }

        private void OnItemSelected(string buildingId)
        {
            objectId = buildingId;
        }

        private GameObject AddPreview(string buildingId)
        {
            var previewObject = buildingHelper.AddBuildingPlaceholder(buildingId);
            return previewObject;
        }
    }
}