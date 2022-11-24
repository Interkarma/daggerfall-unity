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
        FactionFile factionFile;
        SaveGames saveGames;
        SaveTree[] saveTrees;
        SaveVars[] saveVars;
        SaveTree currentSaveTree;
        SaveVars currentSaveVars;
        SaveTreeBaseRecord[] currentItems;
        GUIContent[] saveNames;
        Texture2D[] saveTextures;
        bool showImageFoldout = false;

        Dictionary<int, FactionFile.FactionData> factionDict = new Dictionary<int, FactionFile.FactionData>();
        Dictionary<int, bool> factionFoldoutDict = new Dictionary<int, bool>();

        int lastSelectedSave = -1;
        int selectedSave = 0;

        Vector2 scrollPos;
        bool showFactionsFoldout = false;
        bool showItemsFoldout = false;
        bool showSaveTreeFoldout = false;

        CharacterRecord characterRecord = null;

        [MenuItem(menuPath)]
        static void Init()
        {
            SaveExplorerWindow window = (SaveExplorerWindow)EditorWindow.GetWindow(typeof(SaveExplorerWindow));
            window.titleContent = new GUIContent(windowTitle);
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
                currentSaveVars = saveVars[selectedSave];
                if (currentSaveTree == null || currentSaveVars == null)
                    return;

                currentItems = currentSaveTree.FindRecords(RecordTypes.Item).ToArray();

                // Merge savetree faction data
                factionDict = factionFile.Merge(currentSaveVars);

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
                    showFactionsFoldout = GUILayoutHelper.Foldout(showFactionsFoldout, new GUIContent("Factions"), () =>
                    {
                        GUILayoutHelper.Indent(() =>
                        {
                            DisplayFactionsFoldout();
                        });
                    });

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
                        EditorGUILayout.HelpBox("Temporarily Filtering out records of type Door and Goods to keep list manageable.", MessageType.Info);

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

            SaveTreeBaseRecord positionRecord = currentSaveTree.FindRecord(RecordTypes.CharacterPositionRecord);

            EditorGUILayout.Space();
            GUILayoutHelper.Horizontal(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Version"), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(currentSaveTree.Header.Version.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            });
            GUILayoutHelper.Horizontal(() =>
            {
                string positionText = string.Format("X={0}, Y={1}, Z={2}",
                    positionRecord.RecordRoot.Position.WorldX,
                    positionRecord.RecordRoot.Position.WorldY,
                    positionRecord.RecordRoot.Position.WorldZ);

                EditorGUILayout.LabelField(new GUIContent("Player Position", "Position of player in the world."), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(positionText, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            });
            GUILayoutHelper.Horizontal(() =>
            {
                DFPosition mapPixel = MapsFile.WorldCoordToMapPixel(positionRecord.RecordRoot.Position.WorldX, positionRecord.RecordRoot.Position.WorldZ);
                string mapPixelText = string.Format("X={0}, Y={1}", mapPixel.X, mapPixel.Y);

                EditorGUILayout.LabelField(new GUIContent("Player Map Pixel", "Position of player on small map."), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(mapPixelText, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            });
            GUILayoutHelper.Horizontal(() =>
            {
                DaggerfallDateTime time = new DaggerfallDateTime();
                time.FromClassicDaggerfallTime(currentSaveVars.GameTime);

                EditorGUILayout.LabelField(new GUIContent("Player Time", "World time of this save."), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(time.LongDateTimeString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            });
            GUILayoutHelper.Horizontal(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Player Environment"), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(((Environments)currentSaveTree.Header.Environment).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
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
            GUILayoutHelper.Horizontal(() =>
            {
                string health = string.Format("{0} / {1}", characterRecord.ParsedData.currentHealth, characterRecord.ParsedData.maxHealth);
                EditorGUILayout.LabelField(new GUIContent("Health"), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(health, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            });
        }

        void DisplayFactionsFoldout()
        {
            if (factionDict.Count == 0)
                return;

            foreach (var kvp in factionDict)
            {
                FactionFile.FactionData faction = kvp.Value;
                string textLabel = faction.name;

                if (!factionFoldoutDict.ContainsKey(faction.id))
                    factionFoldoutDict.Add(faction.id, false);

                factionFoldoutDict[faction.id] = GUILayoutHelper.Foldout(factionFoldoutDict[faction.id], new GUIContent(textLabel), () =>
                {
                    GUILayoutHelper.Indent(() =>
                    {
                        int parentid = faction.parent;
                        string parentText = "None";
                        if (parentid > 0)
                        {
                            parentText = factionDict[parentid].name;
                        }
                        EditorGUILayout.LabelField(string.Format("Parent: {0}", parentText));

                        EditorGUILayout.LabelField(string.Format("Type: {0}", (FactionFile.FactionTypes)faction.type));
                        EditorGUILayout.LabelField(string.Format("Reputation: {0}", faction.rep));

                        int region = faction.region;
                        string regionText = "None";
                        if (region > 0)
                        {
                            regionText = dfUnity.ContentReader.MapFileReader.GetRegionName(region); // Using non-localized name in save explorer editor
                        }
                        EditorGUILayout.LabelField(string.Format("Region: {0}", regionText));

                        int ruler = faction.ruler;
                        if (ruler > 0)
                            EditorGUILayout.LabelField(string.Format("Ruler: {0}", factionDict[ruler].name));

                        int ally1 = faction.ally1;
                        int ally2 = faction.ally2;
                        int ally3 = faction.ally3;
                        if (ally1 > 0)
                            EditorGUILayout.LabelField(string.Format("Ally1: {0}", factionDict[ally1].name));
                        if (ally2 > 0)
                            EditorGUILayout.LabelField(string.Format("Ally2: {0}", factionDict[ally2].name));
                        if (ally3 > 0)
                            EditorGUILayout.LabelField(string.Format("Ally2: {0}", factionDict[ally3].name));

                        int enemy1 = faction.enemy1;
                        int enemy2 = faction.enemy2;
                        int enemy3 = faction.enemy3;
                        if (enemy1 > 0)
                            EditorGUILayout.LabelField(string.Format("Enemy1: {0}", factionDict[enemy1].name));
                        if (enemy2 > 0)
                            EditorGUILayout.LabelField(string.Format("Enemy2: {0}", factionDict[enemy2].name));
                        if (enemy3 > 0)
                            EditorGUILayout.LabelField(string.Format("Enemy3: {0}", factionDict[enemy3].name));

                        int sgroup = faction.sgroup;
                        if (sgroup > 0)
                            EditorGUILayout.LabelField(string.Format("SocialGroup: {0}", (FactionFile.SocialGroups)sgroup));

                        int ggroup = faction.ggroup;
                        if (ggroup > 0)
                            EditorGUILayout.LabelField(string.Format("GuildGroup: {0}", (FactionFile.GuildGroups)ggroup));

                        int vam = faction.vam;
                        if (vam > 0)
                            EditorGUILayout.LabelField(string.Format("VampireClan: {0}", factionDict[vam].name));
                    });
                });
            }
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
                if (recordType == RecordTypes.Door || recordType == RecordTypes.Goods)
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

                // Tag container types
                if (recordType == RecordTypes.Container && parent.RecordType == RecordTypes.Character)
                {
                    string[] tags = {" [Weapons & Armor]", " [Magic Items]", " [Clothing & Misc]", " [Ingredients]", " [Wagon]",
                                     " [House]", " [Ship]", " [Tavern Rooms]", " [Item Repairers]"};

                    ContainerRecord containerRecord = (ContainerRecord)parent.Children[i];

                    textLabel += tags[containerRecord.RecordRoot.SpriteIndex];
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
                if (equippedItems[i] == record.RecordRoot.RecordID)
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

            if (factionFile == null)
                factionFile = new FactionFile(dfUnity.ContentReader.GetFactionFilePath(), FileUsage.UseMemory, true);

            if (saveGames == null || saveTrees == null || saveNames == null)
            {
                saveGames = new SaveGames();
                saveNames = new GUIContent[6];
                saveTrees = new SaveTree[6];
                saveVars = new SaveVars[6];
                saveTextures = new Texture2D[6];
                if (saveGames.OpenSavesPath(Path.GetDirectoryName(DaggerfallUnity.Instance.Arena2Path)))
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (saveGames.HasSave(i))
                        {
                            saveGames.OpenSave(i, false);
                            saveTrees[i] = saveGames.SaveTree;
                            saveVars[i] = saveGames.SaveVars;
                            saveNames[i] = new GUIContent(saveGames.SaveName);
                            saveTextures[i] = TextureReader.CreateFromAPIImage(saveGames.SaveImage);
                            saveTextures[i].filterMode = FilterMode.Point;
                        }
                        else
                        {
                            saveTrees[i] = null;
                            saveVars[i] = null;
                            saveTextures[i] = null;
                            saveNames[i] = new GUIContent("Empty");
                        }
                    }
                }

                // Prevent duplicate names so save games aren't automatically removed from the Save Select GUI
                for (int i = 0; i < saveNames.Length; i++)
                {
                    var nextName = saveNames[i];
                    if (nextName == null) continue;
                    int duplicateCount = 0;
                    for (int j = i + 1; j < saveNames.Length; j++)
                    {
                        var otherName = saveNames[j];
                        if (otherName == null) continue;
                        if (otherName.text == nextName.text)
                        {
                            bool unique = false;
                            while (!unique)
                            {
                                unique = true;
                                string replaceText = $"{otherName.text}({++duplicateCount})";
                                for (int k = 0; k < saveNames.Length; k++)
                                {
                                    if (saveNames[k].text == replaceText)
                                    {
                                        unique = false;
                                        break;
                                    }
                                }

                                if (unique)
                                    otherName.text = replaceText;
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}