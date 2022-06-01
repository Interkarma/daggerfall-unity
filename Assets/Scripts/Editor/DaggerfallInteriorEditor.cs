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
    [CustomEditor(typeof(DaggerfallInterior))]
    public class DaggerfallInteriorEditor : Editor
    {
        private DaggerfallInterior dfInterior { get { return target as DaggerfallInterior; } }

        SerializedProperty Prop(string name)
        {
            return serializedObject.FindProperty(name);
        }

        public override void OnInspectorGUI()
        {
            // Update
            serializedObject.Update();

            DisplayAboutGUI();

            // Save modified properties
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        private void DisplayAboutGUI()
        {
            //var propBlockIndex = Prop("_BlockIndex");
            //var propRecordIndex = Prop("_RecordIndex");
            //var propLocationReference = Prop("_LocationReference");

            //EditorGUILayout.Space();
            //propBlockIndex.intValue = EditorGUILayout.IntField("Block Index", propBlockIndex.intValue);
            //propRecordIndex.intValue = EditorGUILayout.IntField("Record Index", propRecordIndex.intValue);

            //if (GUILayout.Button("Apply"))
            //{
            //    dfInterior.RemoveLayout();
            //    dfInterior.DoLayout(dfInterior._BlockIndex, dfInterior._RecordIndex);
            //}
        }
    }
}