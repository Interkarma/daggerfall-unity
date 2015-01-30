// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.InternalTypes;
using DaggerfallConnect.Utility;

namespace DaggerfallWorkshop
{
    [CustomEditor(typeof(MeshReader))]
    public class MeshReaderEditor : Editor
    {
        private MeshReader materialReader { get { return target as MeshReader; } }

        SerializedProperty Prop(string name)
        {
            return serializedObject.FindProperty(name);
        }

        public override void OnInspectorGUI()
        {
            // Update
            serializedObject.Update();

            DisplayGUI();

            // Save modified properties
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        private void DisplayGUI()
        {
            var propAddMeshTangents = Prop("AddMeshTangents");
            var propAddMeshLightmapUVs = Prop("AddMeshLightmapUVs");

            EditorGUILayout.Space();
            propAddMeshTangents.boolValue = EditorGUILayout.Toggle(new GUIContent("Add Mesh Tangents", "Add tangent to mesh data for normal mapping."), propAddMeshTangents.boolValue);
            propAddMeshLightmapUVs.boolValue = EditorGUILayout.Toggle(new GUIContent("Add Lightmap UVs", "Add secondary UV set for lightmapping. Will greatly increase import time."), propAddMeshLightmapUVs.boolValue);
        }
    }
}
