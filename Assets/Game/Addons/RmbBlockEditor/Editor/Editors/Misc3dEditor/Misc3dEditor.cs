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
    public static class Misc3dEditor
    {
        public static VisualElement Render(GameObject go)
        {
            // Get serialized properties:
            var misc3d = go.GetComponent<Misc3d>();
            var id = misc3d.ModelId.ToString();
            var so = new SerializedObject(misc3d);
            var modelIdProperty = so.FindProperty("ModelId");
            var objectTypeProperty = so.FindProperty("ObjectType");
            var posProperty = so.FindProperty("pos");
            var rotProperty = so.FindProperty("rotation");
            var scaleProperty = so.FindProperty("scale");

            // Render the template:
            var misc3dPropsElement = new VisualElement();
            var rmbMisc3dPropsTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/Misc3dEditor/Template.uxml");
            misc3dPropsElement.Add(rmbMisc3dPropsTree.CloneTree());

            // Select the bindable fields:
            var modelIdField = misc3dPropsElement.Query<Label>("model-id").First();
            var objectTypeField = misc3dPropsElement.Query<Label>("object-type").First();
            var posField = misc3dPropsElement.Query<Vector3Field>("pos").First();
            var rotField = misc3dPropsElement.Query<Vector3Field>("rot").First();
            var scaleField = misc3dPropsElement.Query<FloatField>("scale").First();

            // Bind the fields to the properties:
            modelIdField.BindProperty(modelIdProperty);
            objectTypeField.BindProperty(objectTypeProperty);
            posField.BindProperty(posProperty);
            rotField.BindProperty(rotProperty);
            scaleField.BindProperty(scaleProperty);

            var catalog = PersistedModelsCatalog.Get();

            var modifyCatalogObject = new ModifyCatalogObject(catalog, GetPreview, id);
            modifyCatalogObject.changed += (newId) => { misc3d.ChangeId(newId); };
            misc3dPropsElement.Add(modifyCatalogObject);

            return misc3dPropsElement;
        }

        private static VisualElement GetPreview(string modelId)
        {
            var previewObject = RmbBlockHelper.Add3dObject(modelId);
            return new GoPreview(previewObject);
        }
    }
}