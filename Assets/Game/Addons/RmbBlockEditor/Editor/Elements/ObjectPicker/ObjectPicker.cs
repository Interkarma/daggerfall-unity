// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class ObjectPicker
    {
        public VisualElement visualElement = new VisualElement();
        private string search = "";
        private string objectId;
        private Action<string> onItemSelected;
        private GameObject[] previewGameObjects;
        private VisualElement preview;
        private Func<string, VisualElement> getPreview;
        private Debouncer _debouncer = new Debouncer();

        private List<CatalogItem> catalog;
        private List<CatalogItem> filteredCatalog;

        private Dictionary<string, CatalogItem> itemById;
        private Dictionary<string, HashSet<string>> subcategories;
        private Dictionary<string, HashSet<string>> categories;

        private List<KeyValuePair<string, List<KeyValuePair<string, string>>>> list;

        public ObjectPicker(List<CatalogItem> catalog,
            Action<string> onItemSelected, Func<string, VisualElement> getPreview,
            string objectId = "")
        {
            this.catalog = catalog;
            this.filteredCatalog = catalog;
            this.onItemSelected = onItemSelected;
            this.getPreview = getPreview;
            this.objectId = objectId;
            Initialize();
        }

        private void Initialize()
        {
            RenderTemplate();
            GenerateDictionaries();
            RenderList();
            BindIdField();
            RenderSearch();

            if (objectId != "")
            {
                var objectIdField = visualElement.Query<TextField>("object-id").First();
                objectIdField.value = objectId;
                SetNewItemID();
            }
        }

        private void GenerateDictionaries()
        {
            Catalog.GenerateCatalogDictionaries(filteredCatalog, ref itemById, ref subcategories, ref categories);
        }

        private void OnSearch(ChangeEvent<string> e)
        {
            search = e.newValue.ToLower();
            _debouncer.Debounce(FilterCatalog);
        }

        private void OnChangeObjectIdField(ChangeEvent<string> e)
        {
            var objectIdField = visualElement.Query<TextField>("object-id").First();
            if (objectId == objectIdField.value) return;

            objectId = objectIdField.value;
            _debouncer.Debounce(SetNewItemID);
        }

        private async Task FilterCatalog()
        {
            filteredCatalog = catalog.Where((item) =>
            {
                var idMatch = item.ID.Contains(search);
                var labelMatch = item.Label.ToLower().Contains(search);
                var categoryMatch = item.Category.ToLower().Contains(search);
                var subCategoryMatch = item.Subcategory.ToLower().Contains(search);
                var tagsMatch = item.Tags.ToLower().Contains(search);
                return idMatch || labelMatch || categoryMatch || subCategoryMatch || tagsMatch;
            }).ToList();
            GenerateDictionaries();
            RenderList();
        }

        private void RenderTemplate()
        {
            var tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Elements/ObjectPicker/Template.uxml");
            visualElement.Add(tree.CloneTree());
        }

        private void BindIdField()
        {
            var objectIdField = visualElement.Query<TextField>("object-id").First();
            objectIdField.RegisterCallback<ChangeEvent<string>>(OnChangeObjectIdField,
                TrickleDown.TrickleDown);
        }

        private async Task SetNewItemID()
        {
            var listContainer = visualElement.Query<ScrollView>("list-container").First();

            var oldSelection = visualElement.Query<Button>(className: "selected").First();
            oldSelection?.RemoveFromClassList("selected");

            var newSelection = visualElement.Query<Button>(objectId).First();
            newSelection?.AddToClassList("selected");

            Foldout categoryFoldout = null;
            if (itemById.ContainsKey(objectId))
            {
                categoryFoldout = visualElement.Query<Foldout>(itemById[objectId].Category).First();
            }

            Foldout subcategoryFoldout = null;
            if (itemById.ContainsKey(objectId))
            {
                subcategoryFoldout = categoryFoldout?.Query<Foldout>(itemById[objectId].Subcategory).First();
            }

            if (categoryFoldout != null)
            {
                categoryFoldout.value = true;
            }

            if (subcategoryFoldout != null)
            {
                subcategoryFoldout.value = true;
            }

            if (newSelection != null)
            {
                listContainer.ScrollTo(newSelection);
            }

            try
            {
                preview = getPreview(objectId);

                if (preview != null)
                {
                    // Return the ID to the parent, only if it is an actual object
                    onItemSelected(objectId);
                }
            }
            catch (Exception err)
            {
                objectId = "";
            }
            finally
            {
                RenderPreview();
            }
        }

        private void RenderSearch()
        {
            var searchField = visualElement.Query<TextField>("search").First();
            searchField.RegisterValueChangedCallback(OnSearch);
        }

        private void RenderList()
        {
            var listContainer = visualElement.Query<VisualElement>("list-container").First();
            listContainer.Clear();
            categories.ToList().ForEach(category =>
            {
                var categoryName = category.Key;
                var foldout = new Foldout();
                foldout.text = categoryName;
                foldout.name = categoryName;
                foldout.value = false;
                foldout.AddToClassList("object-list-foldout");

                var subCategories = category.Value;

                subCategories.ToList().ForEach(subCategoryName =>
                {
                    var objectGroup = subcategories[subCategoryName];
                    Foldout subCategoryFoldout;

                    if (subCategoryName == $"{categoryName}_root")
                    {
                        // If the subcategory is the _root subcategory for this category, place the items directly into the category
                        subCategoryFoldout = foldout;
                    }
                    else
                    {
                        // Otherwise, create a new foldout for the subcategory
                        subCategoryFoldout = new Foldout();
                        subCategoryFoldout.text = subCategoryName;
                        subCategoryFoldout.name = subCategoryName;
                        subCategoryFoldout.style.marginLeft = 3;
                        subCategoryFoldout.value = false;
                        subCategoryFoldout.AddToClassList("object-list-foldout");
                        foldout.Add(subCategoryFoldout);
                    }

                    // Place the items of the subcategory in the foldout
                    foreach (var itemId in objectGroup)
                    {
                        var catalogItem = itemById[itemId];

                        var button = new Button();
                        button.text = catalogItem.Label;
                        button.name = catalogItem.ID;
                        button.AddToClassList("object-list-button");

                        button.RegisterCallback<MouseUpEvent>(evt => { OnItemClick(itemId); },
                            TrickleDown.TrickleDown);
                        subCategoryFoldout.Add(button);
                    }
                });
                listContainer.Add(foldout);
            });
        }

        private void OnItemClick(string objectId)
        {
            if (objectId == this.objectId)
            {
                return;
            }

            this.objectId = objectId;

            var objectIdField = visualElement.Query<TextField>("object-id").First();
            objectIdField.value = objectId;

            SetNewItemID();
        }

        private void RenderPreview()
        {
            var element = visualElement.Query<VisualElement>("preview").First();
            element.Clear();
            element.Add(preview);
        }
    }
}