// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using SmartLocalization;
using SmartLocalization.Editor;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Localization;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;

public class LocalizationEditorWindow : EditorWindow
{
    public const string RSCNamespacePrefix = "RSC.";

    const string windowTitle = "Localization";
    const string menuPath = "Daggerfall Tools/Localization";

    //Dictionary<string, Dictionary<string, string>> allLanguageFiles = new Dictionary<string, Dictionary<string, string>>();
    //bool allLanguagesLoaded = false;

    //string[] languageCodes = new string[0];
    //int selectedLanguageCode = 0;

    [MenuItem(menuPath)]
    static void Init()
    {
        LocalizationEditorWindow window = (LocalizationEditorWindow)EditorWindow.GetWindow(typeof(LocalizationEditorWindow));
        window.title = windowTitle;
    }

    void OnGUI()
    {
        if (!LocalizationWorkspace.Exists())
        {
            EditorGUILayout.HelpBox("There is no localization workspace in this project. Use Window > Smart Localization to create a new workspace first.", MessageType.Info);
            return;
        }

        if (GUILayout.Button("Add TEXT.RSC To Root Language"))
        {
            TestCreateNewRootKeys();
        }

        //if (GUILayout.Button("Add German Translations"))
        //{
        //    TestAddGermanTranslations();
        //}

        //if (allLanguageFiles.Count < 1 && !allLanguagesLoaded)
        //{
        //    allLanguageFiles = LanguageHandlerEditor.LoadAllLanguageFiles();

        //    int count = 0;
        //    languageCodes = new string[allLanguageFiles.Count];
        //    foreach(var kvp in allLanguageFiles)
        //    {
        //        languageCodes[count++] = kvp.Key;
        //    }

        //    allLanguagesLoaded = true;
        //}

        //if (languageCodes.Length > 0)
        //{
        //    selectedLanguageCode = EditorGUILayout.Popup(selectedLanguageCode, languageCodes);
        //}
    }

    void TestCreateNewRootKeys()
    {
        // Show root language editor window and get root values
        EditRootLanguageFileWindow window = EditRootLanguageFileWindow.ShowWindow();
        Dictionary<string, LocalizedObject> dict = LanguageHandlerEditor.LoadParsedLanguageFile(string.Empty, true);

        // Add any missing TEXT.RSC records
        DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
        if (dfUnity.IsReady)
        {
            TextResourceFile rscFile = new TextResourceFile(dfUnity.Arena2Path, "TEXT.RSC");
            for (int i = 0; i < rscFile.RecordCount; i++)
            {
                TextResourceFile.TextRecord record = rscFile.GetTextRecordByIndex(i);
                string key = RSCNamespacePrefix + record.id.ToString();
                if (!dict.ContainsKey(key))
                {
                    LocalizedObject obj = new LocalizedObject();
                    obj.ObjectType = LocalizedObjectType.STRING;
                    obj.TextValue = record.text;
                    LanguageDictionaryHelper.AddNewKeyPersistent(dict, key, obj);
                }
            }
        }

        //// Add new root keys and values
        //LocalizedObject obj = new LocalizedObject();
        //obj.ObjectType = LocalizedObjectType.STRING;
        //obj.TextValue = "This is a new text key.";
        //LanguageDictionaryHelper.AddNewKeyPersistent(dict, "NewKey.Test", obj);

        // Set new root values
        window.SetRootValues(dict);
    }

    //void TestAddGermanTranslations()
    //{
    //    const string path = @"D:\Users\Gavin Clayton\Desktop\GameDev\DFUnity\Translations\German\DF-D_v0.70\Version 0.70\arena2\TEXT.RSC";
    //    const string code = "de";

    //    SmartLocalizationWindow slWindow = GetWindow<SmartLocalizationWindow>();

    //    SmartCultureInfo sci = new SmartCultureInfo();
    //    sci.languageCode = code;
    //    TranslateLanguageWindow window = TranslateLanguageWindow.ShowWindow(sci, slWindow);
    //}
}
