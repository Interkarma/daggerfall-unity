// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class BuildingEditor
    {
        public static VisualElement Render(GameObject go, Action modifyBuilding, Action exportBuilding)
        {
            // Get serialized properties:
            var building = go.GetComponent<Building>();
            var so = new SerializedObject(building);
            var factionIdProperty = so.FindProperty("FactionId");
            var buildingTypeProperty = so.FindProperty("BuildingType");
            var qualityProperty = so.FindProperty("Quality");
            var xPosProperty = so.FindProperty("XPos");
            var zPosProperty = so.FindProperty("ZPos");
            var yPosProperty = so.FindProperty("ModelsYPos");
            var yRotationProperty = so.FindProperty("YRotation");

            // Render the template:
            var buildingPropsElement = new VisualElement();
            var rmbPropsTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/BuildingEditor/Template.uxml");
            buildingPropsElement.Add(rmbPropsTree.CloneTree());

            // Select the bindable fields:
            var export = buildingPropsElement.Query<Button>("export-building").First();
            var modify = buildingPropsElement.Query<Button>("modify-building").First();
            var factionIdField = buildingPropsElement.Query<IntegerField>("rmb-building-faction-id").First();
            var buildingTypeField = buildingPropsElement.Query<EnumField>("rmb-building-building-type").First();
            var qualityField = buildingPropsElement.Query<SliderInt>("rmb-building-quality").First();
            var qualityInputField = buildingPropsElement.Query<IntegerField>("rmb-building-quality-input").First();
            var xPosField = buildingPropsElement.Query<IntegerField>("rmb-building-x").First();
            var zPosField = buildingPropsElement.Query<IntegerField>("rmb-building-z").First();
            var yPosField = buildingPropsElement.Query<IntegerField>("rmb-building-y").First();
            var yRotationField = buildingPropsElement.Query<IntegerField>("rmb-building-y-rotation").First();

            // Bind the fields to the properties:
            factionIdField.BindProperty(factionIdProperty);
            buildingTypeField.BindProperty(buildingTypeProperty);
            qualityField.BindProperty(qualityProperty);
            qualityInputField.BindProperty(qualityProperty);
            xPosField.BindProperty(xPosProperty);
            zPosField.BindProperty(zPosProperty);
            yPosField.BindProperty(yPosProperty);
            yRotationField.BindProperty(yRotationProperty);

            modify.RegisterCallback<MouseUpEvent>(evt =>
            {
                modifyBuilding();
            });

            export.RegisterCallback<MouseUpEvent>(evt =>
            {
                exportBuilding();
            });

            return buildingPropsElement;
        }
    }
}