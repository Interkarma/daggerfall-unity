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
    [CustomEditor(typeof(DaggerfallLocation))]
    public class DaggerfallLocationEditor : Editor
    {
        private DaggerfallLocation dfLocation { get { return target as DaggerfallLocation; } }

        private const string showAboutLocationFoldout = "DaggerfallUnity_ShowAboutLocationFoldout";
        private static bool ShowAboutLocationFoldout
        {
            get { return EditorPrefs.GetBool(showAboutLocationFoldout, true); }
            set { EditorPrefs.SetBool(showAboutLocationFoldout, value); }
        }

        private const string showClimateFoldout = "DaggerfallUnity_ShowClimateFoldout";
        private static bool ShowClimateFoldout
        {
            get { return EditorPrefs.GetBool(showClimateFoldout, true); }
            set { EditorPrefs.SetBool(showClimateFoldout, value); }
        }

        private const string showWindowTexturesFoldout = "DaggerfallUnity_ShowWindowTexturesFoldout";
        private static bool ShowWindowTexturesFoldout
        {
            get { return EditorPrefs.GetBool(showWindowTexturesFoldout, true); }
            set { EditorPrefs.SetBool(showWindowTexturesFoldout, value); }
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
            DisplayClimateGUI();

            // Save modified properties
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        private void DisplayAboutGUI()
        {
            EditorGUILayout.Space();
            ShowAboutLocationFoldout = GUILayoutHelper.Foldout(ShowAboutLocationFoldout, new GUIContent("About"), () =>
            {
                GUILayoutHelper.Indent(() =>
                {
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("ID", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfLocation.Summary.ID.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Longitude", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfLocation.Summary.Longitude.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Latitude", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfLocation.Summary.Latitude.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Map Pixel X", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfLocation.Summary.MapPixelX.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Map Pixel Y", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfLocation.Summary.MapPixelY.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("World Coord X", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfLocation.Summary.WorldCoordX.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("World Coord Z", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfLocation.Summary.WorldCoordZ.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("World Climate", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfLocation.Summary.WorldClimate.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Sky Base", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfLocation.Summary.SkyBase.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Region Name", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfLocation.Summary.RegionName, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Location Name", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfLocation.Summary.LocationName, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Has Dungeon", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfLocation.Summary.HasDungeon.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Climate Base", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfLocation.Summary.Climate.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Nature Flats", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                        EditorGUILayout.SelectableLabel(dfLocation.Summary.Nature.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                });
            });
        }

        private void DisplayClimateGUI()
        {
            var propClimateUse = Prop("ClimateUse");
            var propCurrentClimate = Prop("CurrentClimate");
            var propCurrentSeason = Prop("CurrentSeason");
            var propCurrentNatureSet = Prop("CurrentNatureSet");
            var propWindowTextureStyle = Prop("WindowTextureStyle");

            EditorGUILayout.Space();
            ShowClimateFoldout = GUILayoutHelper.Foldout(ShowClimateFoldout, new GUIContent("Climate"), () =>
            {
                GUILayoutHelper.Indent(() =>
                {
                    propClimateUse.enumValueIndex = (int)(LocationClimateUse)EditorGUILayout.EnumPopup(new GUIContent("Usage"), (LocationClimateUse)propClimateUse.enumValueIndex);
                    if (propClimateUse.enumValueIndex == (int)LocationClimateUse.Disabled)
                        return;

                    propCurrentSeason.enumValueIndex = (int)(ClimateSeason)EditorGUILayout.EnumPopup(new GUIContent("Season"), (ClimateSeason)propCurrentSeason.enumValueIndex);
                    if (propClimateUse.enumValueIndex == (int)LocationClimateUse.Custom)
                    {
                        propCurrentClimate.enumValueIndex = (int)(ClimateBases)EditorGUILayout.EnumPopup(new GUIContent("Climate"), (ClimateBases)propCurrentClimate.enumValueIndex);
                        propCurrentNatureSet.enumValueIndex = (int)(ClimateNatureSets)EditorGUILayout.EnumPopup(new GUIContent("Nature Flats"), (ClimateNatureSets)propCurrentNatureSet.enumValueIndex);
                    }

                    propWindowTextureStyle.enumValueIndex = (int)(WindowStyle)EditorGUILayout.EnumPopup(new GUIContent("Windows", "Change window material for day, night, etc."), (WindowStyle)propWindowTextureStyle.enumValueIndex);

                    if (GUILayout.Button("Apply"))
                    {
                        dfLocation.ApplyClimateSettings();
                    }
                });
            });
        }
    }
}
