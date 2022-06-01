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
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.InternalTypes;
using DaggerfallConnect.Utility;

namespace DaggerfallWorkshop
{
    [CustomEditor(typeof(DaggerfallBillboardBatch))]
    public class DaggerfallBillboardBatchEditor : Editor
    {
        private DaggerfallBillboardBatch dfBillboardBatch { get { return target as DaggerfallBillboardBatch; } }

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

        public void DisplayGUI()
        {
            if (dfBillboardBatch.IsCustom)
            {
                EditorGUILayout.HelpBox("Cannot set properties of a custom batch material from editor at this time.", MessageType.Info);
                return;
            }

            DrawDefaultInspector();

            GUILayoutHelper.Horizontal(() =>
            {
                if (GUILayout.Button("Clear"))
                {
                    dfBillboardBatch.__EditorClearBillboards();
                }
                if (GUILayout.Button("Random"))
                {
                    dfBillboardBatch.__EditorRandomLayout();
                }
            });
        }
    }
}