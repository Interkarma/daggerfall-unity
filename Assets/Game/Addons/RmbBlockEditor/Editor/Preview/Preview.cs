using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
    public class Preview
    {
        private Editor previewEditor;
        private GameObject go;

        private Preview()
        {
            // Make this a singleton class
        }

        private static readonly Preview _preview = new Preview();

        public static Preview GetPreview()
        {
            return _preview;
        }
        public void Render(VisualElement element, GameObject previewGameObject)
        {
            Clear();

            go = previewGameObject;
            go.name = "Object Preview";
            // We only need the GameObject for the preview element, so
            // make it really small, so it doesn't get in the way in the scene view.
            go.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

            // Change the parent of the preview GameObject to be the RmbBlockObject
            var rmbBlockObject = Object.FindObjectOfType<RmbBlockObject>();
            go.transform.parent = rmbBlockObject.gameObject.transform;

            element.Clear();

            Editor.DestroyImmediate(previewEditor);

            if (go != null)
            {
                previewEditor = Editor.CreateEditor(go);
                var previewImage = new IMGUIContainer(() =>
                {
                    previewEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(500, 450), GUIStyle.none);
                });
                element.Add(previewImage);
            }
        }

        public void Clear()
        {
            // If present, destroy the old preview GameObject
            if (go != null)
            {
                Object.DestroyImmediate(go);
            }
        }
    }
}