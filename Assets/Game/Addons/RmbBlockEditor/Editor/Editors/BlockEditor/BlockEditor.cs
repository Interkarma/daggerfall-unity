// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class BlockEditor
    {
        private VisualElement visualElement;
        private GameObject go;

        public BlockEditor(GameObject go)
        {
            this.go = go;

            visualElement = new VisualElement();
        }

        public VisualElement Render()
        {
            RenderTemplate();
            BindFields();
            return visualElement;
        }

        private void RenderTemplate()
        {
            var rmbPropsTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/BlockEditor/Template.uxml");
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
            var positionProperty = so.FindProperty("Position");
            var indexProperty = so.FindProperty("Index");
            var nameProperty = so.FindProperty("Name");
            var typeProperty = so.FindProperty("Type");

            // Select the bindable fields:
            var positionField = visualElement.Query<IntegerField>("rmb-block-position").First();
            var indexField = visualElement.Query<IntegerField>("rmb-block-index").First();
            var nameField = visualElement.Query<TextField>("rmb-block-name").First();
            var typeField = visualElement.Query<EnumField>("rmb-block-type").First();

            // Bind the fields to the properties:
            positionField.BindProperty(positionProperty);
            indexField.BindProperty(indexProperty);
            nameField.BindProperty(nameProperty);
            typeField.BindProperty(typeProperty);
        }
    }
}