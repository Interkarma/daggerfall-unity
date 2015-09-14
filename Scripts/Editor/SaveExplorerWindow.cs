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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Save;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Explore a Daggerfall save game in Unity editor.
    /// </summary>
    public class SaveExplorerWindow : EditorWindow
    {
        const string windowTitle = "Save Explorer";
        const string menuPath = "Daggerfall Tools/Save Explorer";

        DaggerfallUnity dfUnity;
        SaveGames saveGames;
        SaveTree[] saveTrees;
        GUIContent[] saveNames;
        Texture2D[] saveTextures;
        int selectedSave = 0;
        bool showImage = true;

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

            if (saveTrees != null && saveTrees.Length > 0)
            {
                DisplaySaveSelectGUI();
                DisplaySaveImageGUI();
                DisplaySaveStatsGUI();
            }
        }

        void DisplaySaveSelectGUI()
        {
            EditorGUILayout.Space();
            selectedSave = EditorGUILayout.Popup(new GUIContent("Save"), selectedSave, saveNames);
        }

        void DisplaySaveImageGUI()
        {
            showImage = EditorGUILayout.Toggle(new GUIContent("Show save image"), showImage);

            if (saveTextures[selectedSave] == null || !showImage)
                return;

            EditorGUILayout.Space();
            int height = (int)(((float)Screen.width / (float)saveTextures[selectedSave].width) * (float)saveTextures[selectedSave].height);
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawTextureTransparent(rect, saveTextures[selectedSave], ScaleMode.StretchToFill);
        }

        void DisplaySaveStatsGUI()
        {
            SaveTree saveTree = saveTrees[selectedSave];
            if (saveTree == null)
                return;

            EditorGUILayout.Space();
            //GUILayoutHelper.Horizontal(() =>
            //{
            //    EditorGUILayout.LabelField(new GUIContent("Location Detail Records"));
            //    EditorGUILayout.SelectableLabel(saveTree.LocationDetail.RecordCount.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            //});
            GUILayoutHelper.Horizontal(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Item Records"));
                EditorGUILayout.SelectableLabel(saveTree.ItemRecords.Count.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (GUILayout.Button("Export"))
                    ExportGUI(saveTree.ItemRecords.ToArray(), "item");
            });
            GUILayoutHelper.Horizontal(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Spell Records"));
                EditorGUILayout.SelectableLabel(saveTree.SpellRecords.Count.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (GUILayout.Button("Export"))
                    ExportGUI(saveTree.SpellRecords.ToArray(), "spell");
            });
            GUILayoutHelper.Horizontal(() =>
            {
                EditorGUILayout.LabelField(new GUIContent("Other Records"));
                EditorGUILayout.SelectableLabel(saveTree.OtherRecords.Count.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (GUILayout.Button("Export"))
                    ExportGUI(saveTree.OtherRecords.ToArray(), "other");
            });
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
                    File.WriteAllBytes(filename, record.RecordData);
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