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
    [CustomEditor(typeof(DaggerfallAudioSource))]
    public class DaggerfallAudioSourceEditor : Editor
    {
        private DaggerfallAudioSource dfAudioSource { get { return target as DaggerfallAudioSource; } }

        private const string showPreviewSoundFoldout = "DaggerfallUnity_PreviewSoundFoldout";
        private static bool ShowPreviewSoundFoldout
        {
            get { return EditorPrefs.GetBool(showPreviewSoundFoldout, true); }
            set { EditorPrefs.SetBool(showPreviewSoundFoldout, value); }
        }

        SerializedProperty Prop(string name)
        {
            return serializedObject.FindProperty(name);
        }

        public override void OnInspectorGUI()
        {
            // Update
            serializedObject.Update();

            DisplayEditorGUI();

            // Save modified properties
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        private void DisplayEditorGUI()
        {
            var propPreset = Prop("Preset");

            EditorGUILayout.Space();
            propPreset.enumValueIndex = (int)(AudioPresets)EditorGUILayout.EnumPopup(new GUIContent("Preset"), (AudioPresets)propPreset.enumValueIndex);
            dfAudioSource.SoundIndex = EditorGUILayout.IntField(new GUIContent("Index", "Index of sound to apply. Valid range is 0-458."), dfAudioSource.SoundIndex);

            //EditorGUILayout.Space();
            //if (GUILayout.Button("Apply"))
            //    dfAudioSource.Apply();

            EditorGUILayout.Space();
            ShowPreviewSoundFoldout = GUILayoutHelper.Foldout(ShowPreviewSoundFoldout, new GUIContent("Sound Finder"), () =>
            {
                GUILayoutHelper.Indent(() =>
                {
                    GUILayoutHelper.Horizontal(() =>
                    {
                        dfAudioSource.PreviewIndex = EditorGUILayout.IntField(new GUIContent("Preview Index", "Preview sound by index. Valid range is 0-458."), dfAudioSource.PreviewIndex);
                        if (GUILayout.Button("Preview"))
                            dfAudioSource.EditorPreviewByIndex();
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        dfAudioSource.PreviewID = EditorGUILayout.IntField(new GUIContent("Preview ID", "Preview sound by ID."), dfAudioSource.PreviewID);
                        if (GUILayout.Button("Preview"))
                            dfAudioSource.EditorPreviewByID();
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        dfAudioSource.PreviewClip = (SoundClips)EditorGUILayout.EnumPopup(new GUIContent("Preview Clip", "Preview sound by clip enum."), dfAudioSource.PreviewClip);
                        if (GUILayout.Button("Preview"))
                            dfAudioSource.EditorPreviewBySoundClip();
                    });
                });
            });
        }
    }
}