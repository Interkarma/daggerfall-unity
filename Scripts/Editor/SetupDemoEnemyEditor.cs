using UnityEngine;
using UnityEditor;
using System.Collections;
using DaggerfallWorkshop.Demo;
using DaggerfallWorkshop.Utility;

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
            if (GUILayout.Button("Align To Ground"))
            {
                CharacterController controller = setupDemoEnemy.GetComponent<CharacterController>();
                if (controller != null)
                {
                    GameObjectHelper.AlignControllerToGround(controller);
                }
            }

            // Save modified properties
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}