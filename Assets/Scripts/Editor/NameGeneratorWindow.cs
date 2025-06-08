// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Test name generator in Unity editor.
    /// </summary>
    public class NameGeneratorWindow : EditorWindow
    {
        const string windowTitle = "Name Generator";
        const string menuPath = "Daggerfall Tools/Name Generator [Beta]";

        DaggerfallUnity dfUnity;
        NameHelper nameHelper;

        NameHelper.BankTypes bankType = NameHelper.BankTypes.Breton;
        Genders gender = Genders.Male;
        int seed = -1;
        int count = 20;
        List<string> generatedNames = new List<string>();
        Vector2 scrollPos = Vector2.zero;

        [MenuItem(menuPath)]
        static void Init()
        {
            NameGeneratorWindow window = (NameGeneratorWindow)EditorWindow.GetWindow(typeof(NameGeneratorWindow));
            window.titleContent = new GUIContent(windowTitle);
        }

        void OnGUI()
        {
            if (!IsReady())
            {
                EditorGUILayout.HelpBox("DaggerfallUnity instance not ready. Have you set your Arena2 path?", MessageType.Info);
                return;
            }

            EditorGUILayout.Space();
            bankType = (NameHelper.BankTypes)EditorGUILayout.EnumPopup(new GUIContent("Type"), bankType);
            gender = (Genders)EditorGUILayout.EnumPopup(new GUIContent("Gender"), gender);
            seed = EditorGUILayout.IntField(new GUIContent("Seed"), seed);
            count = EditorGUILayout.IntField(new GUIContent("Count"), count);
            if (GUILayout.Button("Generate Names"))
            {
                if (seed != -1)
                    DFRandom.srand(seed);

                generatedNames.Clear();
                for (int i = 0; i < count; i++)
                {
                    generatedNames.Add(nameHelper.FullName(bankType, gender));
                }
            }

            scrollPos = GUILayoutHelper.ScrollView(scrollPos, () =>
            {
                for (int i = 0; i < generatedNames.Count; i++)
                {
                    EditorGUILayout.SelectableLabel(generatedNames[i], EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                }
            });
        }

        bool IsReady()
        {
            if (!dfUnity)
                dfUnity = DaggerfallUnity.Instance;

            if (nameHelper == null)
                nameHelper = new NameHelper();

            return true;
        }
    }
}