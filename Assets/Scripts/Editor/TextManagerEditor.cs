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
            var runtimeInternalStrings = Prop("runtimeInternalStrings");
            var runtimeRSCStrings = Prop("runtimeRSCStrings");
            var runtimeBOKStrings = Prop("runtimeBOKStrings");

            var tableCopyOverwriteTargetStringTables = Prop("tableCopyOverwriteTargetStringTables");
            var tableCopyTargetInternalStrings = Prop("tableCopyTargetInternalStrings");
            var tableCopyTargetRSCStrings = Prop("tableCopyTargetRSCStrings");
            var tableCopyTargetBOKStrings = Prop("tableCopyTargetBOKStrings");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Live String Tables", EditorStyles.boldLabel);
            GUILayoutHelper.Indent(() =>
            {
                runtimeInternalStrings.stringValue = EditorGUILayout.TextField("Internal Strings", runtimeInternalStrings.stringValue);
                runtimeRSCStrings.stringValue = EditorGUILayout.TextField("RSC Strings", runtimeRSCStrings.stringValue);
                runtimeBOKStrings.stringValue = EditorGUILayout.TextField("BOK Strings", runtimeBOKStrings.stringValue);
            });

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Copy String Tables", EditorStyles.boldLabel);
            GUILayoutHelper.Indent(() =>
            {
                tableCopyTargetInternalStrings.stringValue = EditorGUILayout.TextField("Internal Strings > ", tableCopyTargetInternalStrings.stringValue);
                tableCopyTargetRSCStrings.stringValue = EditorGUILayout.TextField("RSC Strings > ", tableCopyTargetRSCStrings.stringValue);
                tableCopyTargetBOKStrings.stringValue = EditorGUILayout.TextField("BOK Strings > ", tableCopyTargetBOKStrings.stringValue);
                tableCopyOverwriteTargetStringTables.boolValue = EditorGUILayout.Toggle(new GUIContent("Overwrite String Tables?", "When enabled will copy over existing strings in target string tables."), tableCopyOverwriteTargetStringTables.boolValue);
            });

            if (tableCopyOverwriteTargetStringTables.boolValue)
                EditorGUILayout.HelpBox("Warning: Existing keys in the target string tables will be replaced by source.", MessageType.Warning);
            else
                EditorGUILayout.HelpBox("Copy will create all missing keys in target string tables from source. Existing keys will not be overwritten.", MessageType.Info);

            EditorGUILayout.Space();
            if (GUILayout.Button("Copy All"))
            {
                DaggerfallStringTableImporter.CopyInternalStringTable(tableCopyTargetInternalStrings.stringValue, tableCopyOverwriteTargetStringTables.boolValue);
                DaggerfallStringTableImporter.CopyTextRSCToStringTable(tableCopyTargetRSCStrings.stringValue, tableCopyOverwriteTargetStringTables.boolValue);
                //DaggerfallStringTableImporter.CopyTextBOKToStringTable(tableCopyTargetBOKStrings.stringValue, tableCopyOverwriteTargetStringTables.boolValue);
            }
        }
    }
}