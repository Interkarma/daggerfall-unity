// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor.Elements
{
    public class ModifyCatalogObject : VisualElement
    {
        private string objectId;
        private Action<string> change;
        private Action OnPreviousDelegate;
        private Action OnNextDelegate;
        private Action OnShowCatalogDelegate;
        private bool pickerInstantiated;

        public event Action<string> changed
        {
            add => change += value;
            remove => change -= value;
        }

        public ModifyCatalogObject(Catalog catalog, Func<string, VisualElement> GetPreview, string objectId)
        {
            this.objectId = objectId;

            // Register a callback to be invoked after the element has been removed
            RegisterCallback<DetachFromPanelEvent>(evt => OnRemovedFromHierarchy());

            // Render the template
            var template =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Elements/ModifyCatalogObject/Template.uxml");
            Add(template.CloneTree());


            InitializeSlider(catalog, GetPreview);
            InitializeObjectPicker();
        }

        private void InitializeSlider(Catalog catalog, Func<string, VisualElement> GetPreview)
        {
            var itemById = catalog.ItemsDictionary();
            var subcategories = catalog.SubcatalogDictionary();

            // Get references to the buttons
            var previous = this.Query<Button>("previous").First();
            var next = this.Query<Button>("next").First();
            var modify = this.Query<Button>("modify").First();

            // Show the subcategory slider if the item exists in the catalog
            if (itemById.ContainsKey(objectId))
            {
                var item = itemById[objectId];
                var subcategory = subcategories[item.Subcategory].ToList();
                var indexInCategory = subcategory.FindIndex(s => s == objectId);

                // Show subcategory label
                var subcategoryLabel = this.Query<Label>("subcategory").First();
                var isRoot = item.Subcategory.Contains("_root");
                var labelToShow = isRoot ? item.Category : $"{item.Category}/{item.Subcategory}";
                subcategoryLabel.text = labelToShow;

                // Show item label
                var itemLabel = this.Query<Label>("item-id-in-subcategory").First();
                itemLabel.text = item.Label;

                OnPreviousDelegate = delegate
                {
                    var previousIndex = indexInCategory != 0 ? indexInCategory - 1 : 0;
                    objectId = subcategory[previousIndex];
                    OnApply();
                };

                OnNextDelegate = delegate
                {
                    var nextIndex = indexInCategory != subcategory.Count - 1
                        ? indexInCategory + 1
                        : subcategory.Count - 1;
                    objectId = subcategory[nextIndex];
                    OnApply();
                };
            }

            OnShowCatalogDelegate = delegate { ShowCatalog(catalog, GetPreview); };

            // Register callbacks
            previous.clicked += OnPreviousDelegate;
            next.clicked += OnNextDelegate;
            modify.clicked += OnShowCatalogDelegate;
        }

        private void InitializeObjectPicker()
        {
            var apply = this.Query<Button>("apply-button").First();
            var cancel = this.Query<Button>("cancel-button").First();

            // Register the callbacks
            apply.clicked += OnApply;
            cancel.clicked += HideCatalog;
        }

        private void OnItemSelected(string objectId)
        {
            this.objectId = objectId;
        }

        private void ShowCatalog(Catalog catalog, Func<string, VisualElement> GetPreview)
        {
            // Instantiate the object picker
            if (!pickerInstantiated)
            {
                pickerInstantiated = true;
                var pickerContainer = this.Query<VisualElement>("object-picker").First();
                var picker = new ObjectPicker(catalog.List(), OnItemSelected, GetPreview, objectId);
                pickerContainer.Add(picker.visualElement);
            }


            // Show the catalog section
            var catalogContainer = this.Query<VisualElement>("catalog-container").First();
            catalogContainer.RemoveFromClassList("hidden");
        }

        private void HideCatalog()
        {
            var catalogContainer = this.Query<VisualElement>("catalog-container").First();
            catalogContainer.AddToClassList("hidden");
        }

        private void OnApply()
        {
            change(objectId);
        }

        private void OnRemovedFromHierarchy()
        {
            var previous = this.Query<Button>("previous").First();
            var next = this.Query<Button>("next").First();
            var modify = this.Query<Button>("modify").First();
            var apply = this.Query<Button>("apply-button").First();
            var cancel = this.Query<Button>("cancel-button").First();

            previous.clicked -= OnPreviousDelegate;
            next.clicked -= OnNextDelegate;
            modify.clicked -= OnShowCatalogDelegate;
            apply.clicked -= OnApply;
            cancel.clicked -= HideCatalog;
        }
    }
}