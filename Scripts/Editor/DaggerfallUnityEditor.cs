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
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop
{
    [CustomEditor(typeof(DaggerfallUnity))]
    public class DaggerfallUnityEditor : Editor
    {
        private DaggerfallUnity dfUnity { get { return target as DaggerfallUnity; } }

        private const string showOptionsFoldout = "DaggerfallUnity_ShowOptionsFoldout";
        private static bool ShowOptionsFoldout
        {
            get { return EditorPrefs.GetBool(showOptionsFoldout, false); }
            set { EditorPrefs.SetBool(showOptionsFoldout, value); }
        }

        private const string showImportFoldout = "DaggerfallUnity_ShowImportFoldout";
        private static bool ShowImportFoldout
        {
            get { return EditorPrefs.GetBool(showImportFoldout, true); }
            set { EditorPrefs.SetBool(showImportFoldout, value); }
        }

        private const string showEnemyAdvancedFoldout = "DaggerfallUnity_ShowEnemyAdvancedFoldout";
        private static bool ShowEnemyAdvancedFoldout
        {
            get { return EditorPrefs.GetBool(showEnemyAdvancedFoldout, false); }
            set { EditorPrefs.SetBool(showEnemyAdvancedFoldout, value); }
        }

        private const string showTimeAndSpaceFoldout = "DaggerfallUnity_ShowTimeAndSpaceFoldout";
        private static bool ShowTimeAndSpaceFoldout
        {
            get { return EditorPrefs.GetBool(showTimeAndSpaceFoldout, true); }
            set { EditorPrefs.SetBool(showTimeAndSpaceFoldout, value); }
        }

        private const string showAssetExportFoldout = "DaggerfallUnity_ShowAssetExportFoldout";
        private static bool ShowAssetExportFoldout
        {
            get { return EditorPrefs.GetBool(showAssetExportFoldout, false); }
            set { EditorPrefs.SetBool(showAssetExportFoldout, value); }
        }

        SerializedProperty Prop(string name)
        {
            return serializedObject.FindProperty(name);
        }

        public override void OnInspectorGUI()
        {
            // Update
            serializedObject.Update();

            // Get properties
            var propArena2Path = Prop("Arena2Path");

            // Browse for Arena2 path
            EditorGUILayout.Space();
            GUILayoutHelper.Horizontal(() =>
            {
                EditorGUILayout.LabelField("Arena2 Path", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(dfUnity.Arena2Path, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (GUILayout.Button("Browse..."))
                {
                    string path = EditorUtility.OpenFolderPanel("Locate Arena2 Path", "", "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        if (!dfUnity.ValidateArena2Path(path))
                        {
                            EditorUtility.DisplayDialog("Invalid Path", "The selected Arena2 path is invalid", "Close");
                        }
                        else
                        {
                            dfUnity.Arena2Path = path;
                            propArena2Path.stringValue = path;
                        }
                    }
                }
            });

            // Prompt user to set Arena2 path
            if (string.IsNullOrEmpty(dfUnity.Arena2Path))
            {
                EditorGUILayout.HelpBox("Please set the Arena2 path of your Daggerfall installation.", MessageType.Info);
                return;
            }

            // Display other GUI items
            //DisplayAssetExporterGUI();
            DisplayOptionsGUI();
            DisplayImporterGUI();

            // Save modified properties
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        private void DisplayOptionsGUI()
        {
            EditorGUILayout.Space();
            ShowOptionsFoldout = GUILayoutHelper.Foldout(ShowOptionsFoldout, new GUIContent("Options"), () =>
            {
                // Combining options
                var propCombineRMB = Prop("Option_CombineRMB");
                var propCombineRDB = Prop("Option_CombineRDB");
                //var propCombineLocations = Prop("Option_CombineLocations");
                //var propBatchBillboards = Prop("Option_BatchBillboards");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Performance");
                GUILayoutHelper.Indent(() =>
                {
                    propCombineRMB.boolValue = EditorGUILayout.Toggle(new GUIContent("Combine RMB", "Combine city-block meshes together."), propCombineRMB.boolValue);
                    propCombineRDB.boolValue = EditorGUILayout.Toggle(new GUIContent("Combine RDB", "Combine dungeon-block meshes together."), propCombineRDB.boolValue);
                    //propBatchBillboards.boolValue = EditorGUILayout.Toggle(new GUIContent("Batch Billboards", "Use a fast vertex shader and batched data to draw billboards."), propBatchBillboards.boolValue);
                    //propCombineLocations.boolValue = EditorGUILayout.Toggle(new GUIContent("Combine Locations", "Super-combine location RMB blocks together. First combines RMB then chunks those together."), propCombineLocations.boolValue);
                });

                // Import options
                var propSetStaticFlags = Prop("Option_SetStaticFlags");
                var propAddMeshColliders = Prop("Option_AddMeshColliders");
                var propDefaultSounds = Prop("Option_DefaultSounds");
                var propSimpleGroundPlane = Prop("Option_SimpleGroundPlane");
                var propCloseCityGates = Prop("Option_CloseCityGates");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Import Options");
                GUILayoutHelper.Indent(() =>
                {
                    propSetStaticFlags.boolValue = EditorGUILayout.Toggle(new GUIContent("Set Static Flags", "Apply static flag where appropriate when building scenes. Billboards and dynamic objects are not marked static."), propSetStaticFlags.boolValue);
                    propAddMeshColliders.boolValue = EditorGUILayout.Toggle(new GUIContent("Add Colliders", "Add colliders where appropriate when building scenes. Decorative billboards will not receive colliders."), propAddMeshColliders.boolValue);
                    propDefaultSounds.boolValue = EditorGUILayout.Toggle(new GUIContent("Default Sounds", "Adds DaggerfallAudioSource and setup default sounds for noise-making scene objects."), propDefaultSounds.boolValue);
                    propSimpleGroundPlane.boolValue = EditorGUILayout.Toggle(new GUIContent("Simple Ground Plane", "Adds simple quad ground plane to imported exterior locations (ignored by terrain system)."), propSimpleGroundPlane.boolValue);
                    propCloseCityGates.boolValue = EditorGUILayout.Toggle(new GUIContent("Close City Gates", "In walled cities use this flag to start city gates in closed position."), propCloseCityGates.boolValue);
                });

                // Light options
                var propImportPointLights = Prop("Option_ImportPointLights");
                var propAnimatedPointLights = Prop("Option_AnimatedPointLights");
                var propPointLightTag = Prop("Option_PointLightTag");
                var propCustomPointLightScript = Prop("Option_CustomPointLightScript");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Lights");
                GUILayoutHelper.Indent(() =>
                {
                    propImportPointLights.boolValue = EditorGUILayout.Toggle(new GUIContent("Import Point Lights", "Import point lights with city and dungeon blocks."), propImportPointLights.boolValue);
                    GUILayoutHelper.EnableGroup(propImportPointLights.boolValue, () =>
                    {
                        GUILayoutHelper.Indent(() =>
                        {
                            propAnimatedPointLights.boolValue = EditorGUILayout.Toggle(new GUIContent("Animated", "Adds a script to make point lights flicker like Daggerfall. Works best with deferred rendering path"), propAnimatedPointLights.boolValue);
                            propPointLightTag.stringValue = EditorGUILayout.TagField(new GUIContent("Tag", "Custom tag to assign point lights."), propPointLightTag.stringValue);
                            propCustomPointLightScript.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Custom Script", "Custom script to assign point lights."), propCustomPointLightScript.objectReferenceValue, typeof(MonoScript), false);
                        });
                    });
                });

                // Enemy options
                var propImportEnemies = Prop("Option_ImportEnemies");
                var propEnemyCharacterController = Prop("Option_EnemyCharacterController");
                var propEnemyRigidbody = Prop("Option_EnemyRigidbody");
                var propEnemyCapsuleCollider = Prop("Option_EnemyCapsuleCollider");
                var propEnemyNavMeshAgent = Prop("Option_EnemyNavMeshAgent");
                var propEnemyExampleAI = Prop("Option_EnemyExampleAI");
                var propEnemyTag = Prop("Option_EnemyTag");
                var propCustomEnemyScript = Prop("Option_CustomEnemyScript");
                var propEnemyRadius = Prop("Option_EnemyRadius");
                var propEnemySlopeLimit = Prop("Option_EnemySlopeLimit");
                var propEnemyStepOffset = Prop("Option_EnemyStepOffset");
                var propEnemyUseGravity = Prop("Option_EnemyUseGravity");
                var propEnemyIsKinematic = Prop("Option_EnemyIsKinematic");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Enemies");
                GUILayoutHelper.Indent(() =>
                {
                    propImportEnemies.boolValue = EditorGUILayout.Toggle(new GUIContent("Import Enemies", "Import fixed and random enemies from dungeon blocks."), propImportEnemies.boolValue);
                    GUILayoutHelper.EnableGroup(propImportEnemies.boolValue, () =>
                    {
                        GUILayoutHelper.Indent(() =>
                        {
                            propEnemyCharacterController.boolValue = EditorGUILayout.Toggle(new GUIContent("CharacterController", "Add a CharacterController to every enemy."), propEnemyCharacterController.boolValue);
                            propEnemyRigidbody.boolValue = EditorGUILayout.Toggle(new GUIContent("Rigidbody", "Add a Rigidbody to every enemy."), propEnemyRigidbody.boolValue);
                            propEnemyCapsuleCollider.boolValue = EditorGUILayout.Toggle(new GUIContent("CapsuleCollider", "Add a CapsuleCollider to every enemy."), propEnemyCapsuleCollider.boolValue);
                            propEnemyNavMeshAgent.boolValue = EditorGUILayout.Toggle(new GUIContent("NavMeshAgent", "Add a NavMeshAgent to every enemy."), propEnemyNavMeshAgent.boolValue);
                            propEnemyExampleAI.boolValue = EditorGUILayout.Toggle(new GUIContent("Example AI", "Add example AI scripts to every enemy. Adds CharacterController."), propEnemyExampleAI.boolValue);
                            propEnemyTag.stringValue = EditorGUILayout.TagField(new GUIContent("Tag", "Custom tag to assign enemies."), propEnemyTag.stringValue);
                            propCustomEnemyScript.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Custom Script", "Custom script to assign enemies."), propCustomEnemyScript.objectReferenceValue, typeof(MonoScript), false);
                            ShowEnemyAdvancedFoldout = GUILayoutHelper.Foldout(ShowEnemyAdvancedFoldout, new GUIContent("Advanced"), () =>
                            {
                                GUILayoutHelper.Indent(() =>
                                {
                                    propEnemyRadius.floatValue = EditorGUILayout.FloatField(new GUIContent("Radius", "Enemy radius for CharacterController, CapsuleCollider, NavMeshAgent."), propEnemyRadius.floatValue);
                                    propEnemySlopeLimit.floatValue = EditorGUILayout.FloatField(new GUIContent("Slope Limit", "Slope limit for CharacterController."), propEnemySlopeLimit.floatValue);
                                    propEnemyStepOffset.floatValue = EditorGUILayout.FloatField(new GUIContent("Step Offset", "Step offset for CharacterController."), propEnemyStepOffset.floatValue);
                                    propEnemyUseGravity.boolValue = EditorGUILayout.Toggle(new GUIContent("Use Gravity", "Rigidbody should use gravity."), propEnemyUseGravity.boolValue);
                                    propEnemyIsKinematic.boolValue = EditorGUILayout.Toggle(new GUIContent("Is Kinematic", "Rigidbody should be kinematic."), propEnemyIsKinematic.boolValue);
                                });
                            });
                        });
                    });
                });

                // Time & Space options
                var propAutomateTextureSwaps = Prop("Option_AutomateTextureSwaps");
                var propAutomateSky = Prop("Option_AutomateSky");
                var propAutomateCityLights = Prop("Option_AutomateCityLights");
                //var propAutomateCityGates = Prop("Option_AutomateCityGates");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Time & Space");
                GUILayoutHelper.Indent(() =>
                {
                    propAutomateTextureSwaps.boolValue = EditorGUILayout.Toggle(new GUIContent("Automate Textures", "Assign textures for climate, season, and windows at runtime based on player world position and world time."), propAutomateTextureSwaps.boolValue);
                    propAutomateSky.boolValue = EditorGUILayout.Toggle(new GUIContent("Automate Sky", "Assign seasonal skies and full day/night cycle based on player world position and world time."), propAutomateSky.boolValue);
                    propAutomateCityLights.boolValue = EditorGUILayout.Toggle(new GUIContent("Automate City Lights", "Turn city lights on/off based on world time."), propAutomateCityLights.boolValue);
                    //propAutomateCityGates.boolValue = EditorGUILayout.Toggle(new GUIContent("Automate City Gates", "Open/close city gates based on world time. Not implemented."), propAutomateCityGates.boolValue);
                });
            });
        }

        private void DisplayImporterGUI()
        {
            // Hide importer GUI when not active in hierarchy
            if (!dfUnity.gameObject.activeInHierarchy)
                return;

            EditorGUILayout.Space();
            ShowImportFoldout = GUILayoutHelper.Foldout(ShowImportFoldout, new GUIContent("Importer"), () =>
            {
                GUILayoutHelper.Indent(() =>
                {
                    EditorGUILayout.Space();
                    var propModelID = Prop("ModelImporter_ModelID");
                    EditorGUILayout.LabelField(new GUIContent("ModelID", "Enter numeric ID of model."));
                    GUILayoutHelper.Horizontal(() =>
                    {
                        propModelID.intValue = EditorGUILayout.IntField(propModelID.intValue);
                        if (GUILayout.Button("Import"))
                        {
                            GameObjectHelper.CreateDaggerfallMeshGameObject((uint)propModelID.intValue, null);
                        }
                    });

                    EditorGUILayout.Space();
                    var propBlockName = Prop("BlockImporter_BlockName");
                    EditorGUILayout.LabelField(new GUIContent("Block Name", "Enter name of block. Accepts .RMB and .RDB blocks."));
                    GUILayoutHelper.Horizontal(() =>
                    {
                        propBlockName.stringValue = EditorGUILayout.TextField(propBlockName.stringValue.Trim().ToUpper());
                        if (GUILayout.Button("Import"))
                        {
                            GameObjectHelper.CreateDaggerfallBlockGameObject(propBlockName.stringValue, null);
                        }
                    });

                    EditorGUILayout.Space();
                    var propCityName = Prop("CityImporter_CityName");
                    EditorGUILayout.LabelField(new GUIContent("City Name", "Enter exact city name in format RegionName/CityName. Case-sensitive."));
                    GUILayoutHelper.Horizontal(() =>
                    {
                        propCityName.stringValue = EditorGUILayout.TextField(propCityName.stringValue.Trim());
                        if (GUILayout.Button("Import"))
                        {
                            GameObjectHelper.CreateDaggerfallLocationGameObject(propCityName.stringValue, null);
                        }
                    });

                    EditorGUILayout.Space();
                    var propDungeonName = Prop("DungeonImporter_DungeonName");
                    EditorGUILayout.LabelField(new GUIContent("Dungeon Name", "Enter exact dungeon name in format RegionName/DungeonName. Case-sensitive."));
                    GUILayoutHelper.Horizontal(() =>
                    {
                        propDungeonName.stringValue = EditorGUILayout.TextField(propDungeonName.stringValue.Trim());
                        if (GUILayout.Button("Import"))
                        {
                            GameObjectHelper.CreateDaggerfallDungeonGameObject(propDungeonName.stringValue, null);
                        }
                    });
                });
            });
        }

        private void DisplayAssetExporterGUI()
        {
            EditorGUILayout.Space();
            ShowAssetExportFoldout = GUILayoutHelper.Foldout(ShowAssetExportFoldout, new GUIContent("Asset Exporter (Beta)"), () =>
            {
                EditorGUILayout.HelpBox("Export pre-built assets to specified Resources folder and subfolder.", MessageType.Info);

                // Parent Resources path
                var propMyResourcesFolder = Prop("Option_MyResourcesFolder");
                propMyResourcesFolder.stringValue = EditorGUILayout.TextField(new GUIContent("My Resources Folder", "Path to Resources folder for asset export."), propMyResourcesFolder.stringValue);

                // Terrain atlases
                GUILayoutHelper.Horizontal(() =>
                {
                    var propTerrainAtlasesSubFolder = Prop("Option_TerrainAtlasesSubFolder");
                    propTerrainAtlasesSubFolder.stringValue = EditorGUILayout.TextField(new GUIContent("Terrain Atlas SubFolder", "Sub-folder for terrain atlas textures."), propTerrainAtlasesSubFolder.stringValue);
                    if (GUILayout.Button("Update"))
                    {
//#if UNITY_EDITOR && !UNITY_WEBPLAYER
//                        dfUnity.ExportTerrainTextureAtlases();
//#endif
                    }
                });
            });
        }
    }
}
