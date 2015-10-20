// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Player;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Save;
using DaggerfallConnect.Utility;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Explore a Daggerfall save game in Unity editor.
    /// </summary>
    public class SaveExplorerWindow : EditorWindow
    {
        const string windowTitle = "Save Explorer";
        const string menuPath = "Daggerfall Tools/Save Explorer [Beta]";

        DaggerfallUnity dfUnity;
        SaveGames saveGames;
        SaveTree[] saveTrees;
        SaveTree currentSaveTree;
        GUIContent[] saveNames;
        Texture2D[] saveTextures;
        int selectedSave = 0;
        bool showImageFoldout = false;
        Vector2 scrollPos;

        [MenuItem(menuPath)]
        static void Init()
        {
            SaveExplorerWindow window = (SaveExplorerWindow)EditorWindow.GetWindow(typeof(SaveExplorerWindow));
#if UNITY_5_0
            window.title = windowTitle;
#elif UNITY_5_1 || UNITY_5_2
            window.titleContent = new GUIContent(windowTitle);
#endif
        }

        void OnGUI()
        {
            if (!IsReady())
            {
                EditorGUILayout.HelpBox("DaggerfallUnity instance not ready. Have you set your Arena2 path?", MessageType.Info);
                return;
            }

            currentSaveTree = saveTrees[selectedSave];
            if (currentSaveTree == null)
                return;

            if (saveTrees != null && saveTrees.Length > 0)
            {
                DisplaySaveSelectGUI();
                DisplaySaveImageGUI();
                DisplaySaveStatsGUI();
                DisplaySaveCharacterGUI();

                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Temporarily Filtering out records of type UnknownTownLink and UnknownItemRecord to keep list manageable.", MessageType.Info);
                scrollPos = GUILayoutHelper.ScrollView(scrollPos, () =>
                {
                    DisplaySaveTree(currentSaveTree.RootRecord);
                });
            }
        }

        void DisplaySaveSelectGUI()
        {
            EditorGUILayout.Space();
            selectedSave = EditorGUILayout.Popup(new GUIContent("Save"), selectedSave, saveNames);
        }

        void DisplaySaveImageGUI()
        {
            if (saveTextures[selectedSave] != null)
            {
                showImageFoldout = GUILayoutHelper.Foldout(showImageFoldout, new GUIContent("Image"), () =>
                {
                    EditorGUILayout.Space();
                    int height = (int)(((float)Screen.width / (float)saveTextures[selectedSave].width) * (float)saveTextures[selectedSave].height);
                    Rect rect = EditorGUILayout.GetControlRect(false, height);
                    EditorGUI.DrawTextureTransparent(rect, saveTextures[selectedSave], ScaleMode.StretchToFill);
                });
            }
        }

        void DisplaySaveStatsGUI()
        {
            EditorGUILayout.Space();
            GUILayoutHelper.Horizontal(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Version"), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(currentSaveTree.Header.Version.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            });
            GUILayoutHelper.Horizontal(() =>
            {
                string positionText = string.Format("X={0}, Y={1}, Z={2}",
                    currentSaveTree.Header.CharacterPosition.Position.WorldX,
                    currentSaveTree.Header.CharacterPosition.Position.YBase - currentSaveTree.Header.CharacterPosition.Position.YOffset,
                    currentSaveTree.Header.CharacterPosition.Position.YBase,
                    currentSaveTree.Header.CharacterPosition.Position.WorldZ);

                EditorGUILayout.LabelField(new GUIContent("Player Position", "Position of player in the world."), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(positionText, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            });
            GUILayoutHelper.Horizontal(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("LocationDetail records"), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(currentSaveTree.LocationDetail.RecordCount.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            });
            GUILayoutHelper.Horizontal(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("RecordElement records"), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(currentSaveTree.RecordDictionary.Count.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            });
            //GUILayoutHelper.Horizontal(() =>
            //{
            //    EditorGUILayout.LabelField(new GUIContent("Header.Unknown"), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            //    EditorGUILayout.SelectableLabel(currentSaveTree.Header.Unknown.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            //});
            //GUILayoutHelper.Horizontal(() =>
            //{
            //    EditorGUILayout.LabelField(new GUIContent("CharacterPosition.Unknown"), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            //    EditorGUILayout.SelectableLabel(currentSaveTree.Header.CharacterPosition.Unknown.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            //});
        }

        void DisplaySaveCharacterGUI()
        {
            // Get character record
            List<SaveTreeBaseRecord> records = currentSaveTree.FindRecords(RecordTypes.Character);
            if (records.Count != 1)
                return;

            CharacterRecord characterRecord = (CharacterRecord)records[0];
            //CharacterSheet characterSheet = characterRecord.ToCharacterSheet();

            EditorGUILayout.Space();
            GUILayoutHelper.Horizontal(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Name"), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(characterRecord.ParsedData.characterName, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            });
            GUILayoutHelper.Horizontal(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Gender"), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(((int)characterRecord.ParsedData.gender).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            });
        }

        void DisplaySaveTree(SaveTreeBaseRecord parent)
        {
            for (int i = 0; i < parent.Children.Count; i++)
            {
                RecordTypes recordType = parent.Children[i].RecordType;
                if (recordType == RecordTypes.UnknownTownLink || recordType == RecordTypes.UnknownItemRecord)
                    continue;

                string textLabel = recordType.ToString();
                if (recordType == RecordTypes.Item || recordType == RecordTypes.Spell)
                {
                    textLabel += string.Format(" [{0}]", GetItemOrSpellName(parent.Children[i]));
                }

                EditorGUILayout.LabelField(textLabel);
                GUILayoutHelper.Indent(() =>
                {
                    DisplaySaveTree(parent.Children[i]);
                });
            }
        }

        string GetItemOrSpellName(SaveTreeBaseRecord record)
        {
            string result = string.Empty;

            byte[] recordData = record.RecordData;
            MemoryStream stream = new MemoryStream(recordData);
            BinaryReader reader = new BinaryReader(stream);

            if (record.RecordType == RecordTypes.Item)
            {
                result = FileProxy.ReadCString(reader);         // Name starts at offset 0
            }
            else if (record.RecordType == RecordTypes.Spell)
            {
                reader.BaseStream.Position += 47;               // Name starts at offset 47
                result = FileProxy.ReadCString(reader);
            }

            reader.Close();

            return result;
        }

        void ExportGUI(SaveTreeBaseRecord[] records, string name)
        {
            string path = EditorUtility.OpenFolderPanel("Export Path (Note: Place in subfolder as many files will be created)", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                int index = 0;
                foreach(SaveTreeBaseRecord record in records)
                {
                    string filename = Path.Combine(path, string.Format("{0}-{1}", name, index++));
                    File.WriteAllBytes(filename, record.StreamData);
                }
                EditorUtility.DisplayDialog("Finished", string.Format("Exported {0} records.", index), "Close");
            }
        }

        bool IsReady()
        {
            if (!dfUnity)
                dfUnity = DaggerfallUnity.Instance;

            if (!dfUnity.IsReady || string.IsNullOrEmpty(dfUnity.Arena2Path))
                return false;

            if (saveGames == null || saveTrees == null || saveNames == null)
            {
                saveGames = new SaveGames();
                saveNames = new GUIContent[6];
                saveTrees = new SaveTree[6];
                saveTextures = new Texture2D[6];
                if (saveGames.OpenSavesPath(Path.GetDirectoryName(DaggerfallUnity.Instance.Arena2Path)))
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (saveGames.HasSave(i))
                        {
                            saveGames.OpenSave(i);
                            saveTrees[i] = saveGames.SaveTree;
                            saveNames[i] = new GUIContent(saveGames.SaveName);
                            saveTextures[i] = TextureReader.CreateFromAPIImage(saveGames.SaveImage);
                            saveTextures[i].filterMode = FilterMode.Point;
                        }
                        else
                        {
                            saveTrees[i] = null;
                            saveTextures[i] = null;
                            saveNames[i] = new GUIContent("Empty");
                        }
                    }
                }
            }

            return true;
        }
    }
}