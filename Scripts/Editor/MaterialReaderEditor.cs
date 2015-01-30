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
    [CustomEditor(typeof(MaterialReader))]
    public class MaterialReaderEditor : Editor
    {
        private MaterialReader materialReader { get { return target as MaterialReader; } }

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

        private void DisplayGUI()
        {
            var propAtlasTextures = Prop("AtlasTextures");
            var propCompressSkyTextures = Prop("CompressSkyTextures");
            var propMainFilterMode = Prop("MainFilterMode");
            var propSkyFilterMode = Prop("SkyFilterMode");
            var propMipMaps = Prop("MipMaps");
            var propDefaultShaderName = Prop("DefaultShaderName");
            var propDefaultBillboardShaderName = Prop("DefaultBillboardShaderName");
            var propDefaultUnlitBillboardShaderName = Prop("DefaultUnlitBillboardShaderName");
            var propDefaultUnlitTextureShaderName = Prop("DefaultUnlitTextureShaderName");
            var propDefaultSelfIlluminShaderName = Prop("DefaultSelfIlluminShaderName");
            var propDayWindowColor = Prop("DayWindowColor");
            var propNightWindowColor = Prop("NightWindowColor");
            var propFogWindowColor = Prop("FogWindowColor");
            var propCustomWindowColor = Prop("CustomWindowColor");
            var propDayWindowIntensity = Prop("DayWindowIntensity");
            var propNightWindowIntensity = Prop("NightWindowIntensity");
            var propFogWindowIntensity = Prop("FogWindowIntensity");
            var propCustomWindowIntensity = Prop("CustomWindowIntensity");

            EditorGUILayout.Space();
            propAtlasTextures.boolValue = EditorGUILayout.Toggle(new GUIContent("Atlas Textures", "Combine billboards and ground textures into an atlas."), propAtlasTextures.boolValue);
            propMipMaps.boolValue = EditorGUILayout.Toggle(new GUIContent("MipMaps", "Enable mipmaps for textures. Sky and weapon textures never use mipmaps as they are always drawn 1:1."), propMipMaps.boolValue);
            propCompressSkyTextures.boolValue = EditorGUILayout.Toggle(new GUIContent("Compress Sky Textures", "Enable lossy texture compression for skies."), propCompressSkyTextures.boolValue);

            EditorGUILayout.Space();
            propMainFilterMode.enumValueIndex = (int)(FilterMode)EditorGUILayout.EnumPopup(new GUIContent("Main Filter Mode", "Filter mode for materials. Will be applied on next import or climate change."), (FilterMode)propMainFilterMode.enumValueIndex);
            propSkyFilterMode.enumValueIndex = (int)(FilterMode)EditorGUILayout.EnumPopup(new GUIContent("Sky Filter Mode", "Filter mode for the sky."), (FilterMode)propSkyFilterMode.enumValueIndex);
            propDefaultShaderName.stringValue = EditorGUILayout.TextField(new GUIContent("Default Shader", "Name of default mesh shader."), propDefaultShaderName.stringValue);
            propDefaultBillboardShaderName.stringValue = EditorGUILayout.TextField(new GUIContent("Billboard Shader", "Name of default billboard shader. Used for general mesh materials."), propDefaultBillboardShaderName.stringValue);
            propDefaultSelfIlluminShaderName.stringValue = EditorGUILayout.TextField(new GUIContent("Self-Illumin Shader", "Name of default self-illumin shader. Used for windows."), propDefaultSelfIlluminShaderName.stringValue);
            propDefaultUnlitBillboardShaderName.stringValue = EditorGUILayout.TextField(new GUIContent("Unlit Billboard Shader", "Name of default unlit billboard shader. Used for light billboards."), propDefaultUnlitBillboardShaderName.stringValue);
            propDefaultUnlitTextureShaderName.stringValue = EditorGUILayout.TextField(new GUIContent("Unlit Texture Shader", "Name of default unlit texture shader. Used for textures like the fireplace."), propDefaultUnlitTextureShaderName.stringValue);

            EditorGUILayout.Space();
            propDayWindowColor.colorValue = EditorGUILayout.ColorField(new GUIContent("Day Window Colour", "The colour of windows by day."), propDayWindowColor.colorValue);
            propDayWindowIntensity.floatValue = EditorGUILayout.Slider(new GUIContent("Day Window Brightness", "The brightness of windows by day."), propDayWindowIntensity.floatValue, 0, 1);

            EditorGUILayout.Space();
            propNightWindowColor.colorValue = EditorGUILayout.ColorField(new GUIContent("Night Window Colour", "The colour of windows at night."), propNightWindowColor.colorValue);
            propNightWindowIntensity.floatValue = EditorGUILayout.Slider(new GUIContent("Night Window Brightness", "The brightness of windows at night."), propNightWindowIntensity.floatValue, 0, 1);

            EditorGUILayout.Space();
            propFogWindowColor.colorValue = EditorGUILayout.ColorField(new GUIContent("Fog Window Colour", "The colour of windows when foggy."), propFogWindowColor.colorValue);
            propFogWindowIntensity.floatValue = EditorGUILayout.Slider(new GUIContent("Fog Window Brightness", "The brightness of windows when foggy."), propFogWindowIntensity.floatValue, 0, 1);

            EditorGUILayout.Space();
            propCustomWindowColor.colorValue = EditorGUILayout.ColorField(new GUIContent("Custom Window Colour", "Colour of custom windows."), propCustomWindowColor.colorValue);
            propCustomWindowIntensity.floatValue = EditorGUILayout.Slider(new GUIContent("Custom Window Brightness", "Brightness of custom windows."), propCustomWindowIntensity.floatValue, 0, 1);
        }
    }
}