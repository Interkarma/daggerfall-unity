// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Addons.RmbBlockEditor.Elements;
using UnityEditor;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class CatalogEditor : VisualElement
    {
        private ObjectPicker picker;
        private Catalog catalog;
        private string objectId;
        private CatalogItem selectedItem;
        private int selectedIndex;
        private Func<string, VisualElement> GetPreview;
        private Func<List<CatalogItem>> GetCustomAssets;
        private string title;

        public CatalogEditor(string title, Catalog catalog, Func<string, VisualElement> GetPreview,
            Func<List<CatalogItem>> GetCustomAssets)
        {
            this.title = title;
            this.catalog = catalog;
            this.GetPreview = GetPreview;
            this.GetCustomAssets = GetCustomAssets;
            RenderTemplate();
            InitializeCatalogHeader();
            RenderPicker();
            InitializeCatalogItemElement();
        }

        private void RenderTemplate()
        {
            Clear();
            // Register a callback to be invoked after the element has been removed
            RegisterCallback<DetachFromPanelEvent>(evt => OnRemovedFromHierarchy());

            var tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Editors/Catalogs/CatalogEditor/Template.uxml");
            Add(tree.CloneTree());
        }

        private void InitializeCatalogHeader()
        {
            var catalogHeader = this.Query<VisualElement>("catalog-header").First();
            var header = new CatalogHeader(catalog, GetCustomAssets, title);

            header.OnCatalogUpdated += HandleCatalogUpdate;
            catalogHeader.Add(header);
        }

        private void RenderPicker()
        {
            var pickerElement = this.Query<VisualElement>("object-picker").First();
            pickerElement.Clear();
            if (catalog == null)
            {
                return;
            }

            picker = new ObjectPicker(catalog.List(), OnItemSelected, GetPreview, objectId);
            pickerElement.Add(picker.visualElement);
        }

        private void InitializeCatalogItemElement()
        {
            var catalogItemElement = this.Query<CatalogItemElement>("catalog-item-element").First();
            catalogItemElement.OnSaveItem += OnSaveItem;
            catalogItemElement.OnRemoveItem += OnRemoveItem;
        }

        private void OnItemSelected(string objectId)
        {
            this.objectId = objectId;
            selectedIndex = catalog.List().FindIndex(item => item.ID == objectId);
            selectedItem = selectedIndex == -1
                ? new CatalogItem(objectId)
                : catalog.ItemsDictionary()[objectId];

            var catalogItemElement = this.Query<CatalogItemElement>("catalog-item-element").First();
            catalogItemElement.SetItem(selectedItem);

            ShowOptionsBox();
        }

        private void OnItemDeseleted()
        {
            objectId = "";
            HideOptionsBox();
        }

        private void HandleCatalogUpdate()
        {
            OnItemDeseleted();
            RenderPicker();
        }

        private void OnSaveItem(CatalogItem newItem)
        {
            var idToItem = catalog.ItemsDictionary();
            var isInCatalog = idToItem.ContainsKey(selectedItem.ID);

            if (!isInCatalog)
            {
                catalog.AddItem(newItem);
            }
            else
            {
                catalog.ReplaceItem(selectedIndex, newItem);
            }

            RenderPicker();
        }

        private void OnRemoveItem()
        {
            catalog.RemoveItemAt(selectedIndex);
            OnItemDeseleted();
            RenderPicker();
        }

        private void ShowOptionsBox()
        {
            var optionsBox = this.Query<Box>("options-box").First();
            optionsBox.RemoveFromClassList("hidden");
        }

        private void HideOptionsBox()
        {
            var optionsBox = this.Query<Box>("options-box").First();
            optionsBox.AddToClassList("hidden");
        }

        private void OnRemovedFromHierarchy()
        {
            var catalogItemElement = this.Query<CatalogItemElement>("catalog-item-element").First();
            catalogItemElement.OnSaveItem -= OnSaveItem;
            catalogItemElement.OnRemoveItem -= OnRemoveItem;
        }
    }
}