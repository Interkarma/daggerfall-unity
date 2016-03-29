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
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Save;
using DaggerfallConnect.Utility;
using DaggerfallConnect.FallExe;

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
        ItemHelper itemHelper;
        SaveGames saveGames;
        SaveTree[] saveTrees;
        SaveTree currentSaveTree;
        SaveTreeBaseRecord[] currentItems;
        GUIContent[] saveNames;
        Texture2D[] saveTextures;
        bool showImageFoldout = false;

        int lastSelectedSave = -1;
        int selectedSave = 0;

        Vector2 scrollPos;
        bool showItemsFoldout = false;
        bool showSaveTreeFoldout = true;

        CharacterRecord characterRecord = null;

        [MenuItem(menuPath)]
        static void Init()
        {
            SaveExplorerWindow window = (SaveExplorerWindow)EditorWindow.GetWindow(typeof(SaveExplorerWindow));
#if UNITY_5_0
            window.title = windowTitle;
#elif UNITY_5_1 || UNITY_5_2 || UNITY_5_3
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

            if (selectedSave != lastSelectedSave || currentSaveTree == null)
            {
                currentSaveTree = saveTrees[selectedSave];
                if (currentSaveTree == null)
                    return;

                currentItems = currentSaveTree.FindRecords(RecordTypes.Item).ToArray();

                lastSelectedSave = selectedSave;
            }

            if (saveTrees != null && saveTrees.Length > 0)
            {
                DisplaySaveSelectGUI();
                DisplaySaveImageGUI();
                DisplaySaveStatsGUI();
                DisplaySaveCharacterGUI();

                scrollPos = GUILayoutHelper.ScrollView(scrollPos, () =>
                {
                    EditorGUILayout.Space();
                    showItemsFoldout = GUILayoutHelper.Foldout(showItemsFoldout, new GUIContent("Items"), () =>
                    {
                        GUILayoutHelper.Indent(() =>
                        {
                            DisplayItemsFoldout();
                        });
                    });

                    EditorGUILayout.Space();
                    showSaveTreeFoldout = GUILayoutHelper.Foldout(showSaveTreeFoldout, new GUIContent("SaveTree"), () =>
                    {
                        EditorGUILayout.HelpBox("Temporarily Filtering out records of type UnknownTownLink and UnknownItemRecord to keep list manageable.", MessageType.Info);

                        DisplaySaveTree(currentSaveTree.RootRecord);
                    });
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
            if (currentSaveTree == null)
                return;

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
            if (currentSaveTree == null)
                return;

            // Get character record
            List<SaveTreeBaseRecord> records = currentSaveTree.FindRecords(RecordTypes.Character);
            if (records.Count != 1)
                return;

            characterRecord = (CharacterRecord)records[0];
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

        void DisplayItemsFoldout()
        {
            if (currentItems == null || currentItems.Length == 0)
            {
                EditorGUILayout.HelpBox("No items found.", MessageType.Info);
                return;
            }

            for (int i = 0; i < currentItems.Length; i++)
            {
                ItemRecord itemRecord = currentItems[i] as ItemRecord;
                DaggerfallUnityItem item = new DaggerfallUnityItem(itemRecord);

                int equippedIndex = GetEquippedIndex(itemRecord);
                //if (equippedIndex == -1)
                //    continue;

                //string savedName = itemRecord.ParsedData.name;
                //ItemGroups itemGroup = (ItemGroups)itemRecord.ParsedData.category1;
                //int itemIndex = itemRecord.ParsedData.category2;
                //ItemDescription itemDesc = itemHelper.GetItemDescription(itemGroup, itemIndex);
                ////string cat1 = ((ItemGroups)itemRecord.ParsedData.category1).ToString();
                ////string cat2 = itemRecord.ParsedData.category2.ToString();
                ////string subName = ItemHelper.ToSubCategoryName((ItemGroups)itemRecord.ParsedData.category1, itemRecord.ParsedData.category2);
                ////string other = itemRecord.ParsedData.category2.ToString();

                //string textLabel = string.Format(
                //    "{0} [mat=0x{1:X4}]",
                //    itemHelper.ResolveItemName(itemRecord.ParsedData),
                //    itemRecord.ParsedData.material);

                string textLabel = string.Format("Item [{0}]", item.LongName);

                if (equippedIndex != -1)
                    textLabel = "*" + textLabel;

                //string textLabel = itemHelper.GetClassicItemName(itemRecord.ParsedData);
                EditorGUILayout.LabelField(textLabel);
            }
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

                // Check if item equipped
                if (recordType == RecordTypes.Item)
                {
                    //ItemRecord itemRecord = (ItemRecord)parent.Children[i];
                    //textLabel += string.Format(" [id={0}]", itemRecord.RecordRoot.RecordID);

                    int equippedIndex = GetEquippedIndex(parent.Children[i] as ItemRecord);
                    if (equippedIndex != -1)
                    {
                        //textLabel = string.Format("(*={0:00}) {1}", equippedIndex, textLabel);
                    }
                }

                // Tag wagon container
                if (recordType == RecordTypes.Container && parent.RecordType == RecordTypes.Character)
                {
                    ContainerRecord containerRecord = (ContainerRecord)parent.Children[i];
                    if (containerRecord.IsWagon)
                        textLabel += " [Wagon]";
                }

                EditorGUILayout.LabelField(textLabel);
                GUILayoutHelper.Indent(() =>
                {
                    DisplaySaveTree(parent.Children[i]);
                });
            }
        }

        int GetEquippedIndex(ItemRecord record)
        {
            if (characterRecord == null || record.RecordType != RecordTypes.Item || record.Parent == null)
                return -1;

            // Try to match item RecordID with equipped item IDs
            // Item RecordID must be shifted right 8 bits
            uint[] equippedItems = characterRecord.ParsedData.equippedItems;
            for (int i = 0; i < equippedItems.Length; i++)
            {
                if (equippedItems[i] == (record.RecordRoot.RecordID >> 8))
                    return i;
            }

            return -1;
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

            if (itemHelper == null)
                itemHelper = new ItemHelper();

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