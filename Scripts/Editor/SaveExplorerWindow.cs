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

        const string saveTreePath = @"D:\Program Files (x86)\Bethesda Softworks\Daggerfall\SAVE1\SAVETREE.DAT";

        DaggerfallUnity dfUnity;

        [MenuItem(menuPath)]
        static void Init()
        {
            SaveExplorerWindow window = (SaveExplorerWindow)EditorWindow.GetWindow(typeof(SaveExplorerWindow));
#if UNITY_5_0
            window.title = windowTitle;
#elif UNITY_5_1
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

            EditorGUILayout.Space();
            if (GUILayout.Button("Big Test Button"))
            {
                //SaveTree saveTree = new SaveTree(saveTreePath);
                ProfileTests.ProfileEnemyImportTime();
            }
        }

        bool IsReady()
        {
            if (!dfUnity)
                dfUnity = DaggerfallUnity.Instance;

            if (!dfUnity.IsReady || string.IsNullOrEmpty(dfUnity.Arena2Path))
                return false;

            return true;
        }
    }
}