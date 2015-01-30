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
    [CustomEditor(typeof(DaggerfallDungeon))]
    public class DaggerfallDungeonEditor : Editor
    {
        private DaggerfallDungeon dfDungeon { get { return target as DaggerfallDungeon; } }

        private const string showAboutDungeonFoldout = "DaggerfallUnity_ShowAboutDungeonFoldout";
        private static bool ShowAboutDungeonFoldout
        {
            get { return EditorPrefs.GetBool(showAboutDungeonFoldout, true); }
            set { EditorPrefs.SetBool(showAboutDungeonFoldout, value); }
        }

        private const string showDungeonTexturesFoldout = "DaggerfallUnity_ShowDungeonTexturesFoldout";
        private static bool ShowDungeonTexturesFoldout
        {
            get { return EditorPrefs.GetBool(showDungeonTexturesFoldout, true); }
            set { EditorPrefs.SetBool(showDungeonTexturesFoldout, value); }
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
            DisplayDungeonTexturesGUI();

            // Save modified properties
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        private void DisplayAboutGUI()
        {
            EditorGUILayout.Space();
            ShowAboutDungeonFoldout = GUILayoutHelper.Foldout(ShowAboutDungeonFoldout, new GUIContent("About"), () =>
            {
                GUILayoutHelper.Indent(() =>
                {
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("ID", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfDungeon.Summary.ID.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Region Name", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfDungeon.Summary.RegionName, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Location Name", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfDungeon.Summary.LocationName, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Dungeon Type", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfDungeon.Summary.DungeonType.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                });
            });
        }

        private void DisplayDungeonTexturesGUI()
        {
            var propDungeonTextureUse = Prop("DungeonTextureUse");

            EditorGUILayout.Space();
            ShowDungeonTexturesFoldout = GUILayoutHelper.Foldout(ShowDungeonTexturesFoldout, new GUIContent("Dungeon Textures (Beta)"), () =>
            {
                GUILayoutHelper.Indent(() =>
                {
                    propDungeonTextureUse.enumValueIndex = (int)(DungeonTextureUse)EditorGUILayout.EnumPopup(new GUIContent("Usage"), (DungeonTextureUse)propDungeonTextureUse.enumValueIndex);
                    if (propDungeonTextureUse.enumValueIndex == (int)DungeonTextureUse.Disabled ||
                        propDungeonTextureUse.enumValueIndex == (int)DungeonTextureUse.UseLocation_NotImplemented)
                        return;

                    dfDungeon.DungeonTextureTable[0] = EditorGUILayout.IntField("119 is ->", dfDungeon.DungeonTextureTable[0]);
                    dfDungeon.DungeonTextureTable[1] = EditorGUILayout.IntField("120 is ->", dfDungeon.DungeonTextureTable[1]);
                    dfDungeon.DungeonTextureTable[2] = EditorGUILayout.IntField("122 is ->", dfDungeon.DungeonTextureTable[2]);
                    dfDungeon.DungeonTextureTable[3] = EditorGUILayout.IntField("123 is ->", dfDungeon.DungeonTextureTable[3]);
                    dfDungeon.DungeonTextureTable[4] = EditorGUILayout.IntField("124 is ->", dfDungeon.DungeonTextureTable[4]);
                    dfDungeon.DungeonTextureTable[5] = EditorGUILayout.IntField("168 is ->", dfDungeon.DungeonTextureTable[5]);

                    GUILayoutHelper.Horizontal(() =>
                    {
                        if (GUILayout.Button("Reset"))
                            dfDungeon.ResetDungeonTextureTable();
                        if (GUILayout.Button("Random"))
                            dfDungeon.RandomiseDungeonTextureTable();
                        if (GUILayout.Button("Apply"))
                            dfDungeon.ApplyDungeonTextureTable();
                    });
                });
            });
        }
    }
}