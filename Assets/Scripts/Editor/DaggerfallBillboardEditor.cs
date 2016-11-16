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
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.InternalTypes;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    [CustomEditor(typeof(DaggerfallBillboard))]
    public class DaggerfallBillboardEditor : Editor
    {
        private DaggerfallBillboard dfBillboard { get { return target as DaggerfallBillboard; } }

        private const string showAboutBillboardFoldout = "DaggerfallUnity_ShowAboutBillboardFoldout";
        private static bool ShowAboutBillboardFoldout
        {
            get { return EditorPrefs.GetBool(showAboutBillboardFoldout, true); }
            set { EditorPrefs.SetBool(showAboutBillboardFoldout, value); }
        }

        private const string showCustomBillboardFoldout = "DaggerfallUnity_ShowCustomBillboardFoldout";
        private static bool ShowCustomBillboardFoldout
        {
            get { return EditorPrefs.GetBool(showCustomBillboardFoldout, true); }
            set { EditorPrefs.SetBool(showCustomBillboardFoldout, value); }
        }

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
            EditorGUILayout.Space();
            ShowCustomBillboardFoldout = GUILayoutHelper.Foldout(ShowCustomBillboardFoldout, new GUIContent("Custom"), () =>
            {
                var propCustomArchive = Prop("customArchive");
                var propCustomRecord = Prop("customRecord");
                GUILayoutHelper.Indent(() =>
                {
                    propCustomArchive.intValue = EditorGUILayout.IntField(new GUIContent("Archive", "Set texture archive index (e.g. TEXTURE.210 is 210)"), propCustomArchive.intValue);
                    propCustomRecord.intValue = EditorGUILayout.IntField(new GUIContent("Record", "Set texture record index (between 0-n)"), propCustomRecord.intValue);
                    if (GUILayout.Button("Set Billboard Texture"))
                    {
                        try
                        {
                            dfBillboard.SetMaterial(propCustomArchive.intValue, propCustomRecord.intValue);
                        }
                        catch(Exception ex)
                        {
                            Debug.Log("Failed to set custom billboard texture. Exception: " + ex.Message);
                        }
                    }
                    if (GUILayout.Button("Align To Surface"))
                    {
                        GameObjectHelper.AlignBillboardToGround(dfBillboard.gameObject, dfBillboard.Summary.Size, 4);
                    }
                });
            });

            EditorGUILayout.Space();
            ShowAboutBillboardFoldout = GUILayoutHelper.Foldout(ShowAboutBillboardFoldout, new GUIContent("About"), () =>
            {
                GUILayoutHelper.Indent(() =>
                {
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("File", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(TextureFile.IndexToFileName(dfBillboard.Summary.Archive), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Index", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfBillboard.Summary.Record.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    //GUILayoutHelper.Horizontal(() =>
                    //{
                    //    EditorGUILayout.LabelField("In Dungeon", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                    //    EditorGUILayout.SelectableLabel(dfBillboard.Summary.InDungeon.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    //});
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Is Mobile", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfBillboard.Summary.IsMobile.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    if (dfBillboard.Summary.IsMobile)
                    {
                        GUILayoutHelper.Horizontal(() =>
                        {
                            EditorGUILayout.LabelField("Gender", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                            EditorGUILayout.SelectableLabel(((DFBlock.RdbFlatGenders)(dfBillboard.Summary.Gender)).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        });
                        GUILayoutHelper.Horizontal(() =>
                        {
                            EditorGUILayout.LabelField("Type", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                            EditorGUILayout.SelectableLabel(dfBillboard.Summary.FixedEnemyType.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        });
                    }
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Is Atlased", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfBillboard.Summary.AtlasedMaterial.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Is Animated", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfBillboard.Summary.AnimatedMaterial.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Current Frame", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfBillboard.Summary.CurrentFrame.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                });
            });
        }
    }
}
