// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
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
using DaggerfallWorkshop;

namespace DaggerfallWorkshop.Game.Utility
{
    public class CompilerWindow : EditorWindow
    {
        const string windowTitle = "Compiler";
        const string menuPath = "Daggerfall Tools/Source Compiler";
        [SerializeField]
        private List<string> filesToCompile;
        [SerializeField]
        Vector2 scrollPos = Vector2.zero;
        [SerializeField]
        bool showFilesFoldout = true;

        private void OnEnable()
        {
            if (filesToCompile == null)
                filesToCompile = new List<string>();
        }

        private void RemoveFileFromList(int i)
        {
            filesToCompile.RemoveAt(i);
        }

        [MenuItem(menuPath)]
        static void Init()
        {
            CompilerWindow window = (CompilerWindow)EditorWindow.GetWindow(typeof(CompilerWindow));
            window.titleContent = new GUIContent(windowTitle);
        }

        void OnGUI()
        {
            GUILayout.Label("Select source files to compile");

            string path = "";

            if (GUILayout.Button("Add File", GUILayout.Width(500)))
            {
                path = EditorUtility.OpenFilePanelWithFilters("", Application.dataPath, new string[] { "CSharp", "cs" });
                if (filesToCompile.Contains(path))
                {
                    Debug.Log("This file is already selected");
                    return;
                }
                else if (File.Exists(path))
                    filesToCompile.Add(path);
                else
                    Debug.Log("Please select a valid file");

            }

            EditorGUILayout.Space();


            if (filesToCompile != null && filesToCompile.Count > 0)
            {
                if (GUILayout.Button("Compile"))
                {
                    try
                    {
                        System.Reflection.Assembly assembly = Compiler.CompileSource(filesToCompile.ToArray(), false, false);
                        Debug.Log(string.Format("Assembly {0} created", assembly.GetName().Name));
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
                }
            }

            EditorGUILayout.Space();

            showFilesFoldout = GUILayoutHelper.Foldout(showFilesFoldout, new GUIContent("Files"), () =>
            {
                scrollPos = GUILayoutHelper.ScrollView(scrollPos, () =>
                {

                    for (int i = 0; i < filesToCompile.Count; i++)
                    {

                        EditorGUILayout.Space();

                        GUILayoutHelper.Horizontal(() =>
                        {
                            EditorGUI.indentLevel++;

                            Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(filesToCompile[i]));
                            EditorGUILayout.LabelField(new GUIContent(filesToCompile[i]), GUILayout.Width(textDimensions[0])); //EditorGUILayout.SelectableLabel(filesToCompile[i]);

                            EditorGUI.indentLevel++;

                            if (GUILayout.Button("Remove", GUILayout.MinWidth(10), GUILayout.Width(60)))
                            {
                                RemoveFileFromList(i);
                                EditorGUILayout.Space();
                            }

                            EditorGUI.indentLevel--;
                            EditorGUI.indentLevel--;

                        });
                    }

                });
            });

        }


    }
}
