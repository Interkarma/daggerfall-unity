// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor.Elements
{
    public class GoPreview : VisualElement
    {
        public GameObject gameObject { get; set; }
        private Editor previewEditor;

        public GoPreview(GameObject go)
        {
            if (go == null) return;

            // Register a callback to be invoked after the element has been rendered
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            // Register a callback to be invoked after the element has been removed
            RegisterCallback<DetachFromPanelEvent>(evt => OnRemovedFromHierarchy());

            gameObject = go;
            gameObject.name = "Object Preview";
            // We only need the GameObject for the preview element, so
            // make it really small, so it doesn't get in the way in the scene view.
            gameObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            // Check if the element has been resized or repositioned
            if (evt.oldRect != evt.newRect)
            {
                // Get the size of the element after it has been rendered
                var currentSize = layout.size;
                // Get the larger dimension and use that for both width and height
                var max = Math.Max(currentSize.x, currentSize.y);

                Object.DestroyImmediate(previewEditor);
                previewEditor = Editor.CreateEditor(gameObject);
                var previewImage = new IMGUIContainer(() =>
                {
                    // Make sure the size of the rect is at least 1, as it can't be zero
                    var size = Math.Max(max, 1);
                    previewEditor.OnInteractivePreviewGUI(
                        GUILayoutUtility.GetRect(size, size), GUIStyle.none);
                });

                Clear();
                Add(previewImage);
            }
        }

        private void OnRemovedFromHierarchy()
        {
            Object.DestroyImmediate(previewEditor);
            Object.DestroyImmediate(gameObject);
        }
    }
}