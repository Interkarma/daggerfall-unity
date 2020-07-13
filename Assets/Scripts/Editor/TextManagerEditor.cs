// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Localization;
using DaggerfallWorkshop.Utility;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;

namespace DaggerfallWorkshop
{
    [CustomEditor(typeof(TextManager))]
    public class TextManagerEditor : Editor
    {
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

        void DisplayGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("String Importer", "Import classic text data into named String Table Collections.\nNOTE: You must create collections manually.\nWARNING: Existing collections will be cleared."), EditorStyles.boldLabel);
            GUILayoutHelper.Horizontal(() =>
            {
                var rscCollectionName = Prop("textRSCCollection");
                if (GUILayout.Button("Import All"))
                {
                    DaggerfallStringTableImporter.ImportTextRSCToStringTables(rscCollectionName.stringValue);
                }
                if (GUILayout.Button("Clear All"))
                {
                    DaggerfallStringTableImporter.ClearStringTables(rscCollectionName.stringValue);
                }
            });
        }
    }
}