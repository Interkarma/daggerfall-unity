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
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Editor class to remix MobilePersonNPC from editor.
    /// May not be required past initial stages of mobile NPC development.
    /// </summary>
    [CustomEditor(typeof(MobilePersonNPC))]
    public class SetupMobilePersonEditor : Editor
    {
        private MobilePersonNPC setupMobilePerson { get { return target as MobilePersonNPC; } }

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
                setupMobilePerson.ApplyPersonSettingsViaInspector();
            }
            if (GUILayout.Button("Align To Ground"))
            {
                var mobilePerson = setupMobilePerson.GetComponentInChildren<MobilePersonAsset>();
                if (mobilePerson)
                {
                    Vector3 billboardSize = mobilePerson.GetSize();
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