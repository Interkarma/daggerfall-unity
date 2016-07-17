// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Editor window for basic item exploration.
    /// </summary>
    public class ItemExplorerWindow : EditorWindow
    {
        const string windowTitle = "Item Explorer";
        const string menuPath = "Daggerfall Tools/Item Explorer [Test]";
        const string itemsFilename = "ItemTemplates.txt";

        DaggerfallUnity dfUnity;

        [MenuItem(menuPath)]
        static void Init()
        {
            ItemExplorerWindow window = (ItemExplorerWindow)EditorWindow.GetWindow(typeof(ItemExplorerWindow));
            window.titleContent = new GUIContent(windowTitle);
        }

        void OnGUI()
        {
            if (!IsReady())
            {
                EditorGUILayout.HelpBox("DaggerfallUnity instance not ready. Have you set your Arena2 path?", MessageType.Info);
                return;
            }

            if (GUILayout.Button("Generate Item JSON"))
            {
                string fallExePath = Path.Combine(Path.GetDirectoryName(dfUnity.Arena2Path), "FALL.EXE");
                string outputPath = Path.Combine(Application.dataPath, itemsFilename);
                ItemDataToJSON.CreateJSON(fallExePath, outputPath);
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