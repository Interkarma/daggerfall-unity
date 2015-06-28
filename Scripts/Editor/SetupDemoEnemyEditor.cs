using UnityEngine;
using UnityEditor;
using System.Collections;
using DaggerfallWorkshop.Demo;

namespace DaggerfallWorkshop
{
    [CustomEditor(typeof(SetupDemoEnemy))]
    public class SetupDemoEnemyEditor : Editor
    {
        private SetupDemoEnemy setupDemoEnemy { get { return target as SetupDemoEnemy; } }

        SerializedProperty Prop(string name)
        {
            return serializedObject.FindProperty(name);
        }

        public override void OnInspectorGUI()
        {
            // Update
            serializedObject.Update();

            DrawDefaultInspector();
            if (GUILayout.Button("Apply Enemy Type"))
            {
                setupDemoEnemy.ApplyEnemySettings();
                EditorUtility.SetDirty(setupDemoEnemy);
                EditorUtility.SetDirty(setupDemoEnemy.GetMobileBillboardChild());
            }

            // Save modified properties
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}