// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor.Elements
{
    public class CatalogItemElement : VisualElement
    {
        public CatalogItem Item;
        public Action<CatalogItem> saveItem;
        public Action removeItem;
        public bool readOnly = true;

        public event Action<CatalogItem> OnSaveItem
        {
            add => saveItem += value;
            remove => saveItem -= value;
        }

        public event Action OnRemoveItem
        {
            add => removeItem += value;
            remove => removeItem -= value;
        }
        public new class UxmlFactory : UxmlFactory<CatalogItemElement, UxmlTraits>
        {
        }

        public CatalogItem GetItem() => Item;

        public void SetItem(CatalogItem value)
        {
            Item = value;
            InitializeFields();
        }

        public CatalogItemElement()
        {
            var template =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Elements/CatalogItem/Template.uxml");
            Add(template.CloneTree());

            InitializeFields();
            BindButtons();
        }

        private void InitializeFields()
        {
            var idElement = this.Query<TextField>("id").First();
            var label = this.Query<TextField>("label").First();
            var category = this.Query<TextField>("category").First();
            var subcategory = this.Query<TextField>("subcategory").First();
            var tags = this.Query<TextField>("tags").First();

            idElement.value = GetItem().ID;
            idElement.isReadOnly = readOnly;
            label.value = GetItem().Label;
            category.value = GetItem().Category;
            subcategory.value = GetItem().Subcategory;
            tags.value = GetItem().Tags;
        }

        private void BindButtons()
        {
            var saveButton = this.Query<Button>("save-item").First();
            saveButton.RegisterCallback<MouseUpEvent>(HandleSubmit, TrickleDown.TrickleDown);
            var removeButton = this.Query<Button>("remove-item").First();
            removeButton.RegisterCallback<MouseUpEvent>(HandleRemove, TrickleDown.TrickleDown);
        }

        private void HandleSubmit(MouseUpEvent e)
        {
            var idElement = this.Query<TextField>("id").First();
            var label = this.Query<TextField>("label").First();
            var category = this.Query<TextField>("category").First();
            var subcategory = this.Query<TextField>("subcategory").First();
            var tags = this.Query<TextField>("tags").First();

            var newItem = new CatalogItem(idElement.value, label.value, category.value, subcategory.value, tags.value);
            saveItem(newItem);
        }

        private void HandleRemove(MouseUpEvent e)
        {
            var confirmed = EditorUtility.DisplayDialog("Remove Item?",
                "You are about to remove this item from the catalog! Are you sure?", "Yes",
                "No");
            if (!confirmed) return;
            removeItem();
        }
    }
}