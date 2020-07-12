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
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;

namespace DaggerfallWorkshop
{
    [CustomEditor(typeof(TextManager))]
    public class TextManagerEditor : Editor
    {
        public StringTable textRSCTable;

        SerializedProperty Prop(string name)
        {
            return serializedObject.FindProperty(name);
        }

        private void OnEnable()
        {
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
            EditorGUILayout.LabelField(new GUIContent("StringTable Importers", "Helpers to import classic text data into StringTables."), EditorStyles.boldLabel);

            // TEXT.RSC importer
            GUILayoutHelper.Horizontal(() =>
            {
                textRSCTable = (StringTable)EditorGUILayout.ObjectField(new GUIContent("Text RSC Table", "StringTable target to receive TEXT.RSC strings."), textRSCTable, typeof(StringTable), false);
                if (GUILayout.Button("Import"))
                {
                    if (textRSCTable)
                    {
                        DaggerfallStringTableImporter.ImportTextRSC(textRSCTable);
                        EditorUtility.SetDirty(textRSCTable);
                    }
                }
                //if (GUILayout.Button("Clear"))
                //{
                //    if (textRSCTable)
                //    {
                //        DaggerfallStringTableImporter.ClearTable(textRSCTable);
                //        EditorUtility.SetDirty(textRSCTable);
                //    }
                //}
            });
        }
    }
}