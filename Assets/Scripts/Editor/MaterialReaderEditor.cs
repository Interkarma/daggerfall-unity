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
            var propGenerateNormals = Prop("GenerateNormals");
            var propNormalTextureStrength = Prop("NormalTextureStrength");
            var propMipMaps = Prop("MipMaps");
            var propReadableTextures = Prop("ReadableTextures");
            var propSharpen = Prop("Sharpen");
            var propDayWindowColor = Prop("DayWindowColor");
            var propNightWindowColor = Prop("NightWindowColor");
            var propFogWindowColor = Prop("FogWindowColor");
            var propCustomWindowColor = Prop("CustomWindowColor");
            var propDayWindowIntensity = Prop("DayWindowIntensity");
            var propNightWindowIntensity = Prop("NightWindowIntensity");
            var propFogWindowIntensity = Prop("FogWindowIntensity");
            var propCustomWindowIntensity = Prop("CustomWindowIntensity");
            var propAlphaTextureFormat = Prop("AlphaTextureFormat");

            EditorGUILayout.Space();
            if (!propAtlasTextures.boolValue ||
                !propMipMaps.boolValue ||
                propReadableTextures.boolValue ||
                propSharpen.boolValue ||
                propGenerateNormals.boolValue)
            {
                EditorGUILayout.HelpBox("Below settings will impact runtime performance. Only change when necessary.", MessageType.Warning);
            }
            propAtlasTextures.boolValue = EditorGUILayout.Toggle(new GUIContent("Atlas Textures", "Combine billboards and ground textures into an atlas."), propAtlasTextures.boolValue);
            propMipMaps.boolValue = EditorGUILayout.Toggle(new GUIContent("MipMaps", "Enable mipmaps for textures. Sky and weapon textures never use mipmaps as they are always drawn 1:1."), propMipMaps.boolValue);
            propReadableTextures.boolValue = EditorGUILayout.Toggle(new GUIContent("Readable Textures", "Enable readable textures for exporting models from scene."), propReadableTextures.boolValue);
            propSharpen.boolValue = EditorGUILayout.Toggle(new GUIContent("Sharpen", "Sharpen image on import. Increases time to import textures."), propSharpen.boolValue);
            propGenerateNormals.boolValue = EditorGUILayout.Toggle(new GUIContent("Normal Textures", "Generate normal textures. Increases time to import textures."), propGenerateNormals.boolValue);
            if (propGenerateNormals.boolValue)
            {
                propNormalTextureStrength.floatValue = EditorGUILayout.Slider(new GUIContent("Normal Strength", "Power of generated normals."), propNormalTextureStrength.floatValue, 0, 1);
            }
            propCompressSkyTextures.boolValue = EditorGUILayout.Toggle(new GUIContent("Compress Sky Textures", "Enable lossy texture compression for skies."), propCompressSkyTextures.boolValue);

            EditorGUILayout.Space();
            propMainFilterMode.enumValueIndex = (int)(FilterMode)EditorGUILayout.EnumPopup(new GUIContent("Main Filter Mode", "Filter mode for materials. Will be applied on next import or climate change."), (FilterMode)propMainFilterMode.enumValueIndex);
            propSkyFilterMode.enumValueIndex = (int)(FilterMode)EditorGUILayout.EnumPopup(new GUIContent("Sky Filter Mode", "Filter mode for the sky."), (FilterMode)propSkyFilterMode.enumValueIndex);

            EditorGUILayout.Space();
            propAlphaTextureFormat.enumValueIndex = (int)(SupportedAlphaTextureFormats)EditorGUILayout.EnumPopup(new GUIContent("Alpha Texture Format", "TextureFormat of alpha-enabled textures such as billboard cutouts."), (SupportedAlphaTextureFormats)propAlphaTextureFormat.enumValueIndex);

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

            EditorGUILayout.Space();
            if (GUILayout.Button("Clear Material Cache"))
            {
                materialReader.ClearCache();
            }
        }
    }
}