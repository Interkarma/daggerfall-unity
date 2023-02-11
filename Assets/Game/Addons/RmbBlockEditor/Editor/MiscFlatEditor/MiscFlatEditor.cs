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
    public static class MiscFlatEditor
    {
        public static VisualElement Render(GameObject go, Action<string> modifyFlat)
        {
            // Get serialized properties:
            var misc3d = go.GetComponent<MiscFlat>();
            var so = new SerializedObject(misc3d);
            var positionProperty = so.FindProperty("Position");
            var archiveProperty = so.FindProperty("TextureArchive");
            var recordProperty = so.FindProperty("TextureRecord");
            var factionIdProperty = so.FindProperty("FactionID");
            var flagsProperty = so.FindProperty("Flags");
            var worldPositionProperty = so.FindProperty("WorldPosition");

            // Render the template:
            var rmbMiscFlatPropsElement = new VisualElement();
            var rmbMiscFlatPropsTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/MiscFlatEditor/Template.uxml");
            rmbMiscFlatPropsElement.Add(rmbMiscFlatPropsTree.CloneTree());

            // Select the bindable fields:
            var positionField = rmbMiscFlatPropsElement.Query<Label>("position").First();
            var archiveField = rmbMiscFlatPropsElement.Query<Label>("archive").First();
            var recordField = rmbMiscFlatPropsElement.Query<Label>("record").First();
            var modify = rmbMiscFlatPropsElement.Query<Button>("modify-model").First();
            var factionIdField = rmbMiscFlatPropsElement.Query<IntegerField>("faction").First();
            var flagsField = rmbMiscFlatPropsElement.Query<IntegerField>("flags").First();
            var worldPosition = rmbMiscFlatPropsElement.Query<Vector3Field>("world-position").First();

            // Bind the fields to the properties:
            positionField.BindProperty(positionProperty);
            archiveField.BindProperty(archiveProperty);
            recordField.BindProperty(recordProperty);
            factionIdField.BindProperty(factionIdProperty);
            flagsField.BindProperty(flagsProperty);
            worldPosition.BindProperty(worldPositionProperty);

            modify.RegisterCallback<MouseUpEvent>(evt =>
            {
                var currentFlat = go.GetComponent<MiscFlat>();
                modifyFlat(currentFlat.TextureArchive + "." + currentFlat.TextureRecord);
            });

            return rmbMiscFlatPropsElement;
        }
    }
}