// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

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