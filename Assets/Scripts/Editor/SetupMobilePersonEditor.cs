// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    [CustomEditor(typeof(SetupMobilePerson))]
    public class SetupMobilePersonEditor : Editor
    {
        private SetupMobilePerson setupMobilePerson { get { return target as SetupMobilePerson; } }

        SerializedProperty Prop(string name)
        {
            return serializedObject.FindProperty(name);
        }

        public override void OnInspectorGUI()
        {
            // Update
            serializedObject.Update();

            DrawDefaultInspector();
            if (GUILayout.Button("Apply Person Type"))
            {
                setupMobilePerson.ApplyPersonSettings();
            }
            if (GUILayout.Button("Align To Ground"))
            {
                DaggerfallMobilePerson mobilePerson = setupMobilePerson.GetComponentInChildren<DaggerfallMobilePerson>();
                if (mobilePerson)
                {
                    Vector3 billboardSize = mobilePerson.GetBillboardSize();
                    GameObjectHelper.AlignBillboardToGround(setupMobilePerson.gameObject, billboardSize);
                }
            }

            // Save modified properties
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}