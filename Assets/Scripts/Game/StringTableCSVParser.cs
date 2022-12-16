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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using DaggerfallWorkshop.Game.Utility.ModSupport;

/// <summary>
/// Basic CSV parser for Windows-styled CSV files containing StringTable Key and Value columns only.
/// This parser is intended only for importing string tables for localization. It is not a general purpose CSV parser.
/// </summary>
public class StringTableCSVParser
{
    const string csvExt = ".csv";
    const string textString = "Text";
    const string keyString = "Key";
    const string valueString = "Value";

    /// <summary>
    /// Loads a CSV patch file for in-game text.
    /// Seeks mods first then StreamingAssets/Text folder.
    /// </summary>
    /// <param name="filename">Filename of StringTable CSV file.</param>
    /// <returns>KeyValuePair for each row if successful, otherwise null.</returns>
    public static KeyValuePair<string, string>[] Load(string filename)
    {
        string csvText = null;

        // Seek mods first then try loading from StreamingAssets/Text
        TextAsset csvTextAsset = null;
        if (ModManager.Instance != null && ModManager.Instance.TryGetAsset(filename, false, out csvTextAsset))
        {
            csvText = csvTextAsset.text;
        }
        else
        {
            if (!filename.EndsWith(csvExt))
                filename += csvExt;

            // Load patch file if present
            string path = Path.Combine(Application.streamingAssetsPath, textString, filename);
            if (File.Exists(path))
                csvText = ReadAllText(path);
            else
                return null;
        }

        // Exit if no patch file loaded or patch file is empty
        if (string.IsNullOrEmpty(csvText))
            return null;

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
    /// Parse source CSV data into key/value pairs separated by comma character.
    /// Source CSV file must have only two columns for Key and Value.
    /// </summary>
    /// <param name="csvText">Source CSV data.</param>
    /// <returns>KeyValuePair for each row.</returns>
    static KeyValuePair<string, string>[] ParseCSVRows(string csvText)
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

        // Remove first row if it contains "Key" as key and "Value" as value
        // This is the expected header row but doesn't need to be present
        // First row will be accepted if any other key/value pair is present instead
        if (rows.Count > 0 && rows[0].Key == keyString && rows[0].Value == valueString)
            rows.RemoveAt(0);

        return rows.ToArray();
    }

    /// <summary>
    /// Read text from a file in read-only access mode.
    /// Allows modder to keep CSV open in Excel without throwing exception in game.
    /// </summary>
    /// <param name="file">File to open.</param>
    /// <returns>All text read from file.</returns>
    static string ReadAllText(string file)
    {
        using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var textReader = new StreamReader(fileStream))
            return textReader.ReadToEnd();
    }
}
