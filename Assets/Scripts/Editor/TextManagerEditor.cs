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
using UnityEditor.Localization.UI;

namespace DaggerfallWorkshop
{
    [CustomEditor(typeof(TextManager))]
    public class TextManagerEditor : Editor
    {
        bool overwriteTargetStringTables = false;
        string targetInternalStrings = string.Empty;
        string targetRSCStrings = string.Empty;

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
            EditorGUILayout.LabelField("Copy String Tables", EditorStyles.boldLabel);
            GUILayoutHelper.Indent(() =>
            {
                targetInternalStrings = EditorGUILayout.TextField("Internal Strings > ", targetInternalStrings);
                targetRSCStrings = EditorGUILayout.TextField("RSC Strings > ", targetRSCStrings);
            });

            overwriteTargetStringTables = EditorGUILayout.Toggle(new GUIContent("Overwrite Target String Tables?", "When enabled will copy over existing strings in target string tables."), overwriteTargetStringTables);
            if (overwriteTargetStringTables)
                EditorGUILayout.HelpBox("Warning: Existing keys in the target string tables will be replaced by source.", MessageType.Warning);
            else
                EditorGUILayout.HelpBox("Copy will create all missing keys in target string tables from source. Existing keys will not be overwritten.", MessageType.Info);

            EditorGUILayout.Space();
            if (GUILayout.Button("Copy All"))
            {
                DaggerfallStringTableImporter.CopyInternalStringTable(targetInternalStrings, overwriteTargetStringTables);
                //DaggerfallStringTableImporter.ImportTextRSCToStringTables(rscCollectionName.stringValue);
            }
        }
    }
}