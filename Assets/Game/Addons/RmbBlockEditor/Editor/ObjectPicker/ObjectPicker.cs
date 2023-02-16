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
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class ObjectPicker
    {
        public VisualElement visualElement;
        private readonly Dictionary<string, Dictionary<string, string>> dictionary;
        private readonly Dictionary<string, string[]> objectGroups;
        private List<KeyValuePair<string, List<KeyValuePair<string, string>>>> list;
        private string search;
        private bool hasSearch;
        private string objectId;
        private Action<string> onItemSelected;
        private GameObject previewGameObject;
        private Editor previewEditor;
        private Func<string, GameObject> getPreviewGO;

        public ObjectPicker(Dictionary<string, Dictionary<string, string>> dictionary,
            Dictionary<string, string[]> objectGroups,
            bool hasSearch, Action<string> onItemSelected, Func<string, GameObject> getPreviewGO, string objectId = "")
        {
            visualElement = new VisualElement();
            this.dictionary = dictionary;
            this.objectGroups = objectGroups;
            this.hasSearch = hasSearch;
            this.onItemSelected = onItemSelected;
            this.getPreviewGO = getPreviewGO;
            previewGameObject = null;
            search = "";
            this.objectId = objectId;

            FilterDictionary();
            RenderTemplate();
            BindIdField();
            RenderSearch();
            RenderList();
            ApplyObjectId();
        }

        private void OnSearch(ChangeEvent<string> e)
        {
            search = e.newValue;
            FilterDictionary();
            RenderList();
        }

        private void FilterDictionary()
        {
            list = new List<KeyValuePair<string, List<KeyValuePair<string, string>>>>();
            dictionary.ToList().ForEach((element) =>
            {
                var sublist = element.Value.ToList();
                sublist = sublist.FindAll(subElement => subElement.Value.ToLower().Contains(search.ToLower()));
                if (sublist.Count > 0)
                {
                    list.Add(new KeyValuePair<string, List<KeyValuePair<string, string>>>(element.Key, sublist));
                }
            });
        }

        private void RenderTemplate()
        {
            var tree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/ObjectPicker/Template.uxml");
            visualElement.Add(tree.CloneTree());
        }

        private void BindIdField()
        {
            var objectIdField = visualElement.Query<TextField>("object-id").First();

            objectIdField.RegisterCallback<ChangeEvent<string>>(evt => { OnChangeObjectId(); },
                TrickleDown.TrickleDown);
        }

        private void OnChangeObjectId()
        {
            var objectIdField = visualElement.Query<TextField>("object-id").First();
            DestroyPreview();
            try
            {
                objectId = objectIdField.value;
                previewGameObject = getPreviewGO(objectId);
                if (previewGameObject != null)
                {
                    // Return the ID to the parent, only if it is an actual object
                    onItemSelected(objectId);

                    previewGameObject.name = "Object Preview";
                    // We only need the previewGameObject for the preview element, so
                    // make it really small, so it doesn't get in the way in the scene view.
                    previewGameObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
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
            if (!hasSearch) return;

            var searchContainer = visualElement.Query<VisualElement>("search-container").First();
            var searchField = visualElement.Query<TextField>("search").First();
            searchContainer.RemoveFromClassList("hidden");

            searchField.RegisterValueChangedCallback(OnSearch);
        }

        private void RenderList()
        {
            var listContainer = visualElement.Query<VisualElement>("list-container").First();
            listContainer.Clear();

            list.ForEach(element =>
            {
                var foldout = new Foldout();
                foldout.text = element.Key;
                foldout.value = false;
                foldout.AddToClassList("object-list-foldout");

                var subList = element.Value;
                var subListContainer = new VisualElement();

                subList.ForEach(subElement =>
                {
                    string[] objectGroup = new string[0];

                    var firstId = subElement.Key;
                    foreach (var objectGroupName in objectGroups.Keys)
                    {
                        // Some flats have leading zeros in their IDs
                        if (objectGroups[objectGroupName][0] != firstId &&
                            objectGroups[objectGroupName][0] != firstId.TrimStart('0'))
                            continue;
                        objectGroup = objectGroups[objectGroupName];
                        break;
                    }

                    if (objectGroup.Length == 0)
                    {
                        var button = new Button();
                        button.text = subElement.Value;
                        button.name = subElement.Key;
                        button.AddToClassList("object-list-button");
                        button.RegisterCallback<MouseUpEvent>(evt => { OnItemClick(subElement.Key); },
                            TrickleDown.TrickleDown);
                        subListContainer.Add(button);
                    }
                    else
                    {
                        var childFoldout = new Foldout();
                        childFoldout.text = subElement.Value;
                        childFoldout.style.paddingLeft = 15;
                        childFoldout.value = false;
                        childFoldout.AddToClassList("object-list-foldout");
                        foreach (var itemId in objectGroup)
                        {
                            var button = new Button();
                            button.text = itemId;
                            button.name = itemId;
                            button.AddToClassList("object-list-button");

                            button.RegisterCallback<MouseUpEvent>(evt => { OnItemClick(itemId); },
                                TrickleDown.TrickleDown);
                            childFoldout.Add(button);
                        }

                        subListContainer.Add(childFoldout);
                    }
                });

                foldout.Add(subListContainer);
                listContainer.Add(foldout);
            });
        }

        private void ApplyObjectId()
        {
            if (objectId != "")
            {
                var objectIdField = visualElement.Query<TextField>("object-id").First();
                objectIdField.value = objectId;
                OnChangeObjectId();
            }
        }

        private void OnItemClick(string objectId)
        {
            if (objectId == this.objectId)
            {
                return;
            }

            var newSelection = visualElement.Query<Button>(objectId).First();
            newSelection.AddToClassList("selected");
            var oldSelection = visualElement.Query<Button>(this.objectId).First();
            if (oldSelection != null)
            {
                oldSelection.RemoveFromClassList("selected");
            }

            this.objectId = objectId;
            var objectIdField = visualElement.Query<TextField>("object-id").First();
            objectIdField.value = this.objectId;
        }

        private void RenderPreview()
        {
            var preview = visualElement.Query<VisualElement>("preview").First();
            preview.Clear();

            Editor.DestroyImmediate(previewEditor);

            if (objectId != "" && previewGameObject != null)
            {
                previewEditor = Editor.CreateEditor(previewGameObject);
                var previewImage = new IMGUIContainer(() =>
                {
                    previewEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(500, 450), GUIStyle.none);
                });
                preview.Add(previewImage);
            }
        }

        private void DestroyPreview()
        {
            // Remove the preview GameObject
            if (previewGameObject != null)
            {
                GameObject.DestroyImmediate(previewGameObject);
            }
        }

        public void Destroy()
        {
            DestroyPreview();
        }
    }
}