// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using UnityEditor;
using System.Collections;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    [CustomEditor(typeof(StreamingWorld))]
    public class CustomWorldEditor : Editor
    {
        private StreamingWorld streamingWorld { get { return target as StreamingWorld; } }

        Texture2D travelMap;
        Texture2D pushPin;
        Rect travelRect;
        Rect pushPinRect;

        SerializedProperty Prop(string name)
        {
            return serializedObject.FindProperty(name);
        }

        public override void OnInspectorGUI()
        {
            // Update
            serializedObject.Update();

            DisplayGUI();
            HandleEvents();

            // Save modified properties
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        private void DisplayGUI()
        {
            if (!streamingWorld.IsReady)
            {
                EditorGUILayout.HelpBox("Requires DaggerfallUnity singleton with Arena2 path set.", MessageType.Info);
                return;
            }

            if (!streamingWorld.LocalPlayerGPS)
            {
                EditorGUILayout.HelpBox("Requires player object with PlayerGPS component.", MessageType.Info);
                var propLocalPlayerGPS = Prop("LocalPlayerGPS");
                propLocalPlayerGPS.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("", ""), propLocalPlayerGPS.objectReferenceValue, typeof(PlayerGPS), true);
                return;
            }

            DrawTravelMap();
            DrawDefaultInspector();

            EditorGUILayout.Space();
            var propFindLocationString = Prop("EditorFindLocationString");
            EditorGUILayout.LabelField(new GUIContent("Find Location", "Enter exact city name in format RegionName/CityName. Case-sensitive."));
            GUILayoutHelper.Horizontal(() =>
            {
                propFindLocationString.stringValue = EditorGUILayout.TextField(propFindLocationString.stringValue.Trim());
                if (GUILayout.Button("Find"))
                {
                    streamingWorld.__EditorFindLocation();
                }
            });

            EditorGUILayout.Space();
            GUILayoutHelper.Horizontal(() =>
            {
                if (GUILayout.Button("Get From PlayerGPS"))
                {
                    streamingWorld.__EditorGetFromPlayerGPS();
                    EditorUtility.SetDirty(streamingWorld.LocalPlayerGPS);
                }
                if (GUILayout.Button("Apply To PlayerGPS"))
                {
                    streamingWorld.__EditorApplyToPlayerGPS();
                    EditorUtility.SetDirty(streamingWorld.LocalPlayerGPS);
                }
            });

            //if (GUILayout.Button("Refresh Preview"))
            //{
            //    streamingWorld.__EditorRefreshPreview();
            //}
        }

        private void DrawTravelMap()
        {
            // Load textures
            if (travelMap == null) travelMap = Resources.Load<Texture2D>("TravelMap");
            if (pushPin == null) pushPin = Resources.Load<Texture2D>("PushPin");

            // Exit if textures not loaded
            if (travelMap == null || pushPin == null)
                return;

            // Draw scaled map texture
            int height = (int)(((float)Screen.width / (float)travelMap.width) * (float)travelMap.height);
            travelRect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawTextureTransparent(travelRect, travelMap, ScaleMode.StretchToFill);

            // Draw push pin
            float percentX = (float)streamingWorld.MapPixelX / (float)MapsFile.MaxMapPixelX;
            float percentY = (float)streamingWorld.MapPixelY / (float)MapsFile.MaxMapPixelY;
            pushPinRect = new Rect(
                travelRect.xMin + travelRect.width * percentX - pushPin.width / 2,
                travelRect.yMin + travelRect.height * percentY - pushPin.height / 2,
                pushPin.width,
                pushPin.height);
            //EditorGUI.DrawTextureTransparent(pushPinRect, pushPin);   // Does not draw transparent properly for some reason
            GUI.DrawTexture(pushPinRect, pushPin);                      // But this does
        }

        private void HandleEvents()
        {
            Event evt = Event.current;
            if ((evt.type == EventType.MouseDown || evt.type == EventType.MouseDrag) &&
                travelRect.Contains(evt.mousePosition))
            {
                // Get scaled mouse position inside travel map rect
                float clickX = evt.mousePosition.x - travelRect.xMin;
                float clickY = evt.mousePosition.y - travelRect.yMin;
                float percentX = clickX / travelRect.width;
                float percentY = clickY / travelRect.height;

                // Convert to map pixel coordinates
                int mapPixelX = (int)((float)MapsFile.MaxMapPixelX * percentX);
                int mapPixelY = (int)((float)MapsFile.MaxMapPixelY * percentY);

                // Assign new coordinates to streaming world preview
                streamingWorld.MapPixelX = mapPixelX;
                streamingWorld.MapPixelY = mapPixelY;

                // Consume event
                Event.current.Use();

                //// Refresh preview
                //streamingWorld.__EditorRefreshPreview();
            }
        }
    }
}