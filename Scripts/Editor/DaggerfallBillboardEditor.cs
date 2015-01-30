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
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("In Dungeon", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfBillboard.Summary.InDungeon.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Is Mobile", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfBillboard.Summary.IsMobile.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    if (dfBillboard.Summary.InDungeon && dfBillboard.Summary.IsMobile)
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
