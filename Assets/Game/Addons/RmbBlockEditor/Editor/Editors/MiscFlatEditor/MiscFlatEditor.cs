// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Linq;
using DaggerfallWorkshop.Game.Addons.RmbBlockEditor.Elements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public static class MiscFlatEditor
    {
        public static VisualElement Render(GameObject go)
        {
            // Get serialized properties:
            var miscFlat = go.GetComponent<MiscFlat>();
            var id = $"{miscFlat.TextureArchive}.{miscFlat.TextureRecord}";
            var so = new SerializedObject(miscFlat);
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
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/MiscFlatEditor/Template.uxml");
            rmbMiscFlatPropsElement.Add(rmbMiscFlatPropsTree.CloneTree());

            // Select the bindable fields:
            var positionField = rmbMiscFlatPropsElement.Query<Label>("position").First();
            var archiveField = rmbMiscFlatPropsElement.Query<Label>("archive").First();
            var recordField = rmbMiscFlatPropsElement.Query<Label>("record").First();
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

            var catalog = PersistedFlatsCatalog.Get();

            var modifyCatalogObject = new ModifyCatalogObject(catalog, GetPreview, id);
            modifyCatalogObject.changed += (newId) => { miscFlat.ChangeId(newId); };
            rmbMiscFlatPropsElement.Add(modifyCatalogObject);

            return rmbMiscFlatPropsElement;
        }

        private static VisualElement GetPreview(string modelId)
        {
            var previewObject = RmbBlockHelper.AddFlatObject(modelId);
            return new GoPreview(previewObject);
        }
    }
}