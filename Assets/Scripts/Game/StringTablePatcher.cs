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

using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using static UnityEditor.Localization.LocalizationTableCollection;

[Serializable]
public class StringTablePatcher : ITablePostprocessor
{
    const string csvExt = ".csv";
    const string textString = "Text";
    const string keyString = "Key";
    const string valueString = "Value";

    public void PostprocessTable(LocalizationTable table)
    {
        // Only supports StringTable input
        if (!(table is StringTable stringTable))
            return;

        // Load table patch data (if present)
        KeyValuePair<string, string>[] rows = LoadCSVPatchFile(table.TableCollectionName + csvExt);
        if (rows == null || rows.Length == 0)
            return;

        // Patch string table from patch data
        foreach(var kvp in rows)
        {
            StringTableEntry entry = stringTable.GetEntry(kvp.Key);
            if (entry != null)
            {
                entry.Value = kvp.Value;
            }
        }
    }

    /// <summary>
    /// Loads a CSV patch file for in-game text.
    /// </summary>
    /// <param name="filename">Filename of patch file including .csv extension. Patch file must be in the StreamingAssets/Text folder to load.</param>
    /// <returns>KeyValuePair for each row.</returns>
    KeyValuePair<string, string>[] LoadCSVPatchFile(string filename)
    {
        string csvText = null;

        // Load patch file if present
        string path = Path.Combine(Application.streamingAssetsPath, textString, filename);
        if (File.Exists(path))
        {
            csvText = ReadAllText(path);
            if (string.IsNullOrEmpty(csvText))
                return null;
        }
        else
        {
            return null;
        }

        // Parse into CSV rows
        KeyValuePair<string, string>[] rows = null;
        try
        {
            rows = ParseCSVRows(csvText);
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("Could not parse CSV from file '{0}'. Exception message: {1}", filename, ex.Message);
            return null;
        }

        return rows;
    }

    /// <summary>
    /// Read text from a file in read-only access mode.
    /// Allows modder to keep CSV open in Excel without throwing exception in game.
    /// </summary>
    /// <param name="file">File to open.</param>
    /// <returns>All text read from file.</returns>
    string ReadAllText(string file)
    {
        using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var textReader = new StreamReader(fileStream))
            return textReader.ReadToEnd();
    }

    /// <summary>
    /// Parse source CSV data into key/value pairs separated by comma character.
    /// Source CSV file must have only two columns for Key and Value.
    /// </summary>
    /// <param name="csvText">Source CSV data.</param>
    /// <returns>KeyValuePair for each row.</returns>
    KeyValuePair<string, string>[] ParseCSVRows(string csvText)
    {
        // Regex pattern from https://gist.github.com/awwsmm/886ac0ce0cef517ad7092915f708175f
        const string linePattern = "(?:,|\\n|^)(\"(?:(?:\"\")*[^\"]*)*\"|[^\",\\n]*|(?:\\n|$))";

        // Split source CSV based on regex matches
        char[] trimChars = { '\r', '\n', '\"', ',' };
        List<KeyValuePair<string, string>> rows = new List<KeyValuePair<string, string>>();
        string[] matches = (from Match m in Regex.Matches(csvText, linePattern, RegexOptions.ExplicitCapture) select m.Groups[0].Value).ToArray();
        int pos = 0;
        while (pos < matches.Length)
        {
            if (pos + 1 == matches.Length)
            {
                // Exit if no valid pair at end of csv (likely an empty line at end of source data)
                break;
            }
            string key = matches[pos++].Trim(trimChars);
            string value = matches[pos++].Trim(trimChars);
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(key, value);
            rows.Add(kvp);
        }

        return rows.ToArray();
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
