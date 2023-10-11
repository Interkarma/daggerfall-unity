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

using System;
using System.Collections.Generic;
//using UnityEditor;
//using UnityEditor.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

/// <summary>
/// ITablePostprocessor string table patcher for in-game text.
/// Attempts to load alternate text from CSV file in StreamingAssets/Text and patch existing values with new text data.
/// Text keys are matched using Key (column 0) and if a match is found the Value (column 1) will become new string data in StringTable.
/// If no matching key is found then a new key/value entry is added to StringTable.
/// </summary>
[Serializable]
public class StringTablePatcher : ITablePostprocessor
{
    public void PostprocessTable(LocalizationTable table)
    {
        // Only supports StringTable input
        if (!(table is StringTable stringTable))
            return;

        // Load table patch data (if present)
        List<KeyValuePair<string, string>> rows = StringTableCSVParser.Load(table.TableCollectionName);
        if (rows == null || rows.Count == 0)
            return;

        // Patch string table from patch data
        foreach(var kvp in rows)
        {
            StringTableEntry entry = stringTable.GetEntry(kvp.Key);
            if (entry != null)
                entry.Value = kvp.Value;
            else
                stringTable.AddEntry(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// TablePostprocessor field does not appear to be exposed in-editor yet.
    /// Below used one-time to add patcher to localization settings.
    /// </summary>
    //public static class AssignStringTablePatcher
    //{
    //    [MenuItem("Daggerfall Tools/TEMP: Assign StringTablePostprocessor")]
    //    public static void AssignTablePostprocessor()
    //    {
    //        // Create an instance of the table provider.
    //        var provider = new StringTablePatcher();

    //        // A table postprocessor can be assigned to each database or the same can be shared between both.
    //        var settings = LocalizationEditorSettings.ActiveLocalizationSettings;
    //        settings.GetStringDatabase().TablePostprocessor = provider;
    //        settings.GetAssetDatabase().TablePostprocessor = provider;

    //        // Set dirty so the changes are saved.
    //        EditorUtility.SetDirty(settings);
    //    }
    //}
}
