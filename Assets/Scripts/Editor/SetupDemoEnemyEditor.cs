// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using System.Collections;
using DaggerfallWorkshop.Game;
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
                setupDemoEnemy.ApplyEnemySettings(setupDemoEnemy.EnemyGender);
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