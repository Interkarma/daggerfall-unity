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
        public string textRSCStringTableName = TextProvider.textRSCCollectionName;

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
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("StringTable Importer", "Import classic text data into named table assets.\nNOTE: You must create table assets manually.\nWARNING: Existing tables will be cleared."), EditorStyles.boldLabel);

            EditorGUILayout.Space();
            textRSCStringTableName = EditorGUILayout.TextField(new GUIContent("TEXT.RSC StringTable Name", "Name of table collection holding TEXT.RSC data."), textRSCStringTableName);

            EditorGUILayout.Space();
            GUILayoutHelper.Horizontal(() =>
            {
                if (GUILayout.Button("Import"))
                {
                    DaggerfallStringTableImporter.ImportTextRSCToStringTables(textRSCStringTableName);
                }
                if (GUILayout.Button("Clear"))
                {
                    DaggerfallStringTableImporter.ClearStringTables(textRSCStringTableName);
                }
            });
        }
    }
}