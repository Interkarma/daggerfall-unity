// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Questing;

namespace DaggerfallWorkshop
{
    [CustomEditor(typeof(QuestMachine))]
    public class QuestMachineEditor : Editor
    {
        string testQuest = "_BRISIEN";

        QuestMachine questMachine { get { return target as QuestMachine; } }

        SerializedProperty Prop(string name)
        {
            return serializedObject.FindProperty(name);
        }

        public override void OnInspectorGUI()
        {
            // Update
            serializedObject.Update();

            DrawDefaultInspector();
            DisplayGUI();

            // Save modified properties
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        private void DisplayGUI()
        {
            // Parse a test quest from editor
            EditorGUILayout.Space();
            GUILayoutHelper.Horizontal(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Test Quest", "Parse a quest directly from editor for testing."), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(testQuest, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (GUILayout.Button("Parse"))
                {
                    questMachine.EditorParseQuest(testQuest);
                }
            });
        }
    }
}