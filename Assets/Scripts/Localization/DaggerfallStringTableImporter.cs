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
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Utility;
using UnityEngine;
using UnityEngine.Localization.Tables;
using DaggerfallWorkshop.Game;
using DaggerfallConnect;
using DaggerfallConnect.Save;
using DaggerfallConnect.FallExe;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Localization;
#endif

namespace DaggerfallWorkshop.Localization
{
    /// <summary>
    /// Helper to import classic text data into StringTables.
    /// </summary>
    public static class DaggerfallStringTableImporter
    {
        const string textMappingTableFilename = "TextMappingTable";
        const string enLocaleCode = "en";
        const char newline = '\n';

        // Markup for RSC token conversion
        // Markup format is designed with the following requirements:
        //  1. Don't overcomplicate - must be simple to understand and edit using plain text
        //  2. Support all classic Daggerfall text data (excluding books which already have custom format)
        //  3. Convert easily between RSC tokens and markup as required
        //  4. Formatting must not conflict with regular text entry in any language
        const string markupJustifyLeft = "[/left]";
        const string markupJustifyCenter = "[/center]";
        const string markupNewLine = "[/newline]";
        const string markupTextPosition = "[/pos:x={0},y={1}]";
        const string markupTextPositionPrefix = "[/pos";
        const string markupFontPrefix = "[/font";
        const string markupTextColorPrefix = "[/color";
        const string markupTextScalePrefix = "[/scale";
        const string markupImagePrefix = "[/image";
        const string markupInputCursor = "[/input]";
        const string markupSubrecordSeparator = "[/record]";
        const string markupEndRecord = "[/end]";

        #region Importer Helpers

        /// <summary>
        /// Create TEXT.RSC key in format "text.nnnn" where "nnnn" is numeric ID from TEXT.RSC entry.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Key for TEXT.RSC ID.</returns>
        public static string MakeTextRSCKey(int id)
        {
            return string.Format(id.ToString());
        }

        /// <summary>
        /// Converts RSC tokens into a string. Tokens will be converted into simple markup.
        /// This string can be converted back into the original RSC token stream.
        /// </summary>
        /// <param name="tokens">RSC token input.</param>
        /// <returns>String with markup converted from RSC tokens.</returns>
        public static string ConvertRSCTokensToString(int id, TextFile.Token[] tokens)
        {
            string recordsText = string.Empty;

            if (tokens == null || tokens.Length == 0)
                return recordsText;

            // Convert RSC formatting tokens into markup text for easier human editing
            // Intentionally adding newlines so that text wraps nicely at justify tokens similar to in-game
            // Unity's StringTable editor does not at this time wrap text (this is being added as an option)
            // But even if it did, layout would not be as nice to read as it is with this method
            // - These newlines are trimmed back out again when reading markup back in later
            // - It's necessary to trim spaces from start of line after newline or Unity's JSON serialization will trim incorrectly
            // - This leads to unusual line-breaking layout in JSON files, but is character accurate and reads correctly in editor and runtime
            string text = string.Empty;
            bool trimStartSpaces = false;
            for (int i = 0; i < tokens.Length; i++)
            {
                switch (tokens[i].formatting)
                {
                    case TextFile.Formatting.Text:
                        if (trimStartSpaces && !ExcludeFromStartTrim(id))
                        {
                            text += tokens[i].text.TrimStart(' ');
                            trimStartSpaces = false;
                        }
                        else
                        {
                            text += tokens[i].text;
                        }
                        break;
                    case TextFile.Formatting.JustifyLeft:
                        text += markupJustifyLeft + newline;
                        trimStartSpaces = true;
                        break;
                    case TextFile.Formatting.JustifyCenter:
                        text += markupJustifyCenter + newline;
                        trimStartSpaces = true;
                        break;
                    case TextFile.Formatting.NewLine:
                        text += markupNewLine + newline;
                        trimStartSpaces = true;
                        break;
                    case TextFile.Formatting.PositionPrefix:
                        text += string.Format(markupTextPosition, tokens[i].x, tokens[i].y);
                        break;
                    case TextFile.Formatting.SubrecordSeparator:
                        recordsText = AppendSubrecord(recordsText, text) + newline;
                        text = string.Empty;
                        trimStartSpaces = true;
                        break;
                    case TextFile.Formatting.InputCursorPositioner:
                        text += markupInputCursor;
                        break;
                    default:
                        // Only the above tokens are found in classic TEXT.RSC
                        // If other tokens appear in translations, they are not supported and will be ignored
                        // Enable the below comment to discover any unhandled tokens during import
                        //Debug.LogErrorFormat("Ignoring unexpected RSC formatting token {0}.", tokens[i].formatting.ToString());
                        break;
                }
            }

            // Add any pending text
            if (!string.IsNullOrEmpty(text))
                recordsText = AppendSubrecord(recordsText, text, true);

            return recordsText;
        }

        public static bool ExcludeFromStartTrim(int id)
        {
            // 9000 requires whitespace at start for character questionnaire to work
            int[] excludedIDs = new int[] { 9000 };

            return excludedIDs.Contains<int>(id);
        }

        /// <summary>
        /// Append subrecord text to combined string.
        /// Subrecords are split using separator markup.
        /// </summary>
        /// <param name="current">Current combined text.</param>
        /// <param name="add">Incoming subrecord text to add.</param>
        /// <param name="end">True if last record.</param>
        /// <returns>Combined string.</returns>
        static string AppendSubrecord(string current, string add, bool end = false)
        {
            return string.Format("{0}{1}{2}", current, add, (end) ? markupEndRecord : markupSubrecordSeparator);
        }

        /// <summary>
        /// Converts string back into RSC tokens.
        /// </summary>
        /// <param name="input">Input string formatted with RSC markup.</param>
        /// <returns>Array of RSC tokens converted from TextElements.</returns>
        public static TextFile.Token[] ConvertStringToRSCTokens(string input)
        {
            List<TextFile.Token> tokens = new List<TextFile.Token>();

            char[] chars = input.ToCharArray();
            if (chars == null || chars.Length == 0)
                return null;

            string text = string.Empty;
            for (int i = 0; i < chars.Length; i++)
            {
                // Strip out newline chars from text input
                // TEXT.RSC does not use newline, this is only added to string output so text more readable in editor
                if (chars[i] == newline)
                    continue;

                string markup;
                if (PeekMarkup(ref chars, i, out markup))
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        tokens.Add(new TextFile.Token(TextFile.Formatting.Text, text));
                        text = string.Empty;
                    }
                    AddToken(tokens, markup);
                    i += markup.Length - 1;
                }
                else
                {
                    text += chars[i];
                }
            }

            if (!string.IsNullOrEmpty(text))
                tokens.Add(new TextFile.Token(TextFile.Formatting.Text, text));

            return tokens.ToArray();
        }

        /// <summary>
        /// Adds supported RSC markup tokens to list.
        /// Unhandled markup is added as a text token.
        /// </summary>
        static void AddToken(List<TextFile.Token> tokens, string markup)
        {
            if (markup == markupJustifyLeft)
                tokens.Add(new TextFile.Token(TextFile.Formatting.JustifyLeft));
            else if (markup == markupJustifyCenter)
                tokens.Add(new TextFile.Token(TextFile.Formatting.JustifyCenter));
            else if (markup == markupNewLine)
                tokens.Add(new TextFile.Token(TextFile.Formatting.NewLine));
            else if (markup.StartsWith(markupTextPositionPrefix))
                tokens.Add(ParsePositionMarkup(markup));
            else if (markup.StartsWith(markupFontPrefix))
                tokens.Add(ParseFontMarkup(markup));
            else if (markup.StartsWith(markupTextColorPrefix))
                tokens.Add(ParseColorMarkup(markup));
            else if (markup.StartsWith(markupTextScalePrefix))
                tokens.Add(ParseScaleMarkup(markup));
            else if (markup.StartsWith(markupImagePrefix))
                tokens.Add(ParseImageMarkup(markup));
            else if (markup == markupSubrecordSeparator)
                tokens.Add(new TextFile.Token(TextFile.Formatting.SubrecordSeparator));
            else if (markup == markupInputCursor)
                tokens.Add(new TextFile.Token(TextFile.Formatting.InputCursorPositioner));
            else if (markup == markupEndRecord)
                tokens.Add(new TextFile.Token(TextFile.Formatting.EndOfRecord));
            else
                tokens.Add(new TextFile.Token(TextFile.Formatting.Text, markup)); // Unhandled markup
        }

        /// <summary>
        /// Reads position markup using regex.
        /// If markup pattern not matched then returned as a string token.
        /// </summary>
        static TextFile.Token ParsePositionMarkup(string markup)
        {
            const string pattern = @"pos:x=(?<x>\d+),y=(?<y>\d+)";

            Match match = Regex.Match(markup, pattern);
            if (!match.Success)
                return new TextFile.Token(TextFile.Formatting.Text, markup);

            int x = Parser.ParseInt(match.Groups["x"].Value);
            int y = Parser.ParseInt(match.Groups["y"].Value);

            return new TextFile.Token(TextFile.Formatting.PositionPrefix, string.Empty, x, y);
        }

        /// <summary>
        /// Reads font markup using regex.
        /// If markup pattern not matched then returned as a string token.
        /// </summary>
        static TextFile.Token ParseFontMarkup(string markup)
        {
            const string pattern = @"font=(?<x>\d+)";

            Match match = Regex.Match(markup, pattern);
            if (!match.Success)
                return new TextFile.Token(TextFile.Formatting.Text, markup);

            int x = Parser.ParseInt(match.Groups["x"].Value);

            return new TextFile.Token(TextFile.Formatting.FontPrefix, string.Empty, x, 0);
        }

        static TextFile.Token ParseColorMarkup(string markup)
        {
            const string pattern = @"color=(?<x>[0-9a-f]{6})";

            Match match = Regex.Match(markup, pattern);
            if (!match.Success)
                return new TextFile.Token(TextFile.Formatting.Text, markup);

            return new TextFile.Token(TextFile.Formatting.Color, match.Groups["x"].Value, 0, 0);
        }

        static TextFile.Token ParseScaleMarkup(string markup)
        {
            const string pattern = @"scale=(?<x>[+-]?([0-9]*[.])?[0-9]+)";

            Match match = Regex.Match(markup, pattern);
            if (!match.Success)
                return new TextFile.Token(TextFile.Formatting.Text, markup);

            return new TextFile.Token(TextFile.Formatting.Scale, match.Groups["x"].Value, 0, 0);
        }

        static TextFile.Token ParseImageMarkup(string markup)
        {
            const string pattern = @"\[\/image=(?<x>.*)\]";

            Match match = Regex.Match(markup, pattern);
            if (!match.Success)
                return new TextFile.Token(TextFile.Formatting.Text, markup);

            return new TextFile.Token(TextFile.Formatting.Image, match.Groups["x"].Value, 0, 0);
        }

        /// <summary>
        /// Peeks a markup token from current position in stream.
        /// </summary>
        static bool PeekMarkup(ref char[] chars, int pos, out string markup)
        {
            markup = string.Empty;

            if (chars == null || pos > chars.Length - 2)
                return false;

            if (chars[pos] == '[' && chars[pos + 1] == '/')
            {
                int endpos = pos;
                for (int i = pos + 2; i < chars.Length; i++)
                {
                    if (chars[i] == ']')
                    {
                        endpos = i + 1;
                        break;
                    }
                }

                if (endpos > pos)
                {
                    markup = new string(chars, pos, endpos - pos);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempt to load a custom TEXT.RSC binary file for a locale.
        /// To make available - append matching locale code and .bytes to end of file then drop into a Resources folder
        /// e.g. French TEXT.RSC filename becomes "TEXT-fr.RSC.bytes" before adding to a Resources folder
        /// </summary>
        /// <param name="code">Locale code.</param>
        /// <returns>TextFile or null.</returns>
        static TextFile LoadCustomLocaleTextRSC(string code)
        {
            TextFile localeRSC = null;
            if (code != enLocaleCode)
            {
                string localeFilename = string.Format("TEXT_{0}.RSC", code);
                TextAsset binaryFile = Resources.Load<TextAsset>(localeFilename);
                if (binaryFile)
                {
                    localeRSC = new TextFile();
                    localeRSC.Load(binaryFile.bytes, localeFilename);
                    if (!localeRSC.IsLoaded)
                        localeRSC = null;
                }
            }

            return localeRSC;
        }

        /// <summary>
        /// Remaps characters from character mapping table.
        /// </summary>
        static string RemapCharacters(string locale, string input, Table charMappingTable)
        {
            string result = input;

            for (int i = 0; i < charMappingTable.RowCount; i++)
            {
                // 0 = key
                // 1 = locale
                // 2 = source
                // 3 = replacement
                string[] row = charMappingTable.GetRow(i);
                if (row[1] == locale)
                    result = result.Replace(row[2], row[3]);
            }

            return result;
        }

        #endregion

        #region Editor Only Methods

#if UNITY_EDITOR
        // Gets string table from collection name and locale code
        private static StringTable GetSourceStringTable(string source, string locale)
        {
            // Get source string table collection
            var sourceCollection = LocalizationEditorSettings.GetStringTableCollection(source);
            if (sourceCollection == null)
            {
                Debug.LogErrorFormat("GetSourceStringTable() could not find source string table collection '{0}'", source);
                return null;
            }

            // Find table in source collection with locale code
            StringTable sourceTable = null;
            foreach (StringTable table in sourceCollection.StringTables)
            {
                if (table.LocaleIdentifier == locale)
                {
                    sourceTable = table;
                    break;
                }
            }

            // Handle table not found
            if (sourceTable == null)
            {
                Debug.LogErrorFormat("GetSourceStringTable() could not find source string table with locale code '{0}' in collection '{1}'", locale, source);
                return null;
            }

            return sourceTable;
        }

        // Gets list of all text keys from table
        private static List<string> GetKeys(LocalizationTable table)
        {
            List<string> keys = new List<string>();
            foreach (SharedTableData.SharedTableEntry entry in table.SharedData.Entries)
            {
                keys.Add(entry.Key);
            }
            return keys;
        }

        /// <summary>
        /// Copies default Internal_Strings EN to all string tables in target collection.
        /// </summary>
        /// <param name="target">Target string table collection name.</param>
        /// <param name="overwriteExistingKeys">When true will overwrite existing keys with source string. When false existing keys are left unchanged.</param>
        public static void CopyInternalStringTable(string target, bool overwriteExistingKeys)
        {
            // Use default Internal_Strings collection with EN locale code as source
            string sourceCollectionName = TextManager.defaultInternalStringsCollectionName;
            string sourceLocaleCode = enLocaleCode;

            // Do nothing if target not set
            if (string.IsNullOrEmpty(target))
                return;

            // Target cannot be same as default
            if (string.Compare(target, sourceCollectionName, true) == 0)
            {
                Debug.LogError("CopyInternalStringTable() target cannot be same as default");
                return;
            }

            // Get target string table collection
            var targetCollection = LocalizationEditorSettings.GetStringTableCollection(target);
            if (targetCollection == null)
            {
                Debug.LogErrorFormat("CopyInternalStringTable() could not find target string table collection '{0}'", target);
                return;
            }

            // Get source string table
            StringTable sourceTable = GetSourceStringTable(sourceCollectionName, sourceLocaleCode);
            if (!sourceTable)
            {
                Debug.LogErrorFormat("CopyInternalStringTable() could not find source table '{0}' with locale '{1}'", sourceCollectionName, sourceLocaleCode);
                return;
            }

            // Copy source strings to all tables in target collection
            int totalSourceEntries = sourceTable.SharedData.Entries.Count;
            int copiedNew = 0;
            int copiedOverwrite = 0;
            foreach (StringTable targetTable in targetCollection.StringTables)
            {
                // Iterate through all string table values found in source table
                foreach (var item in sourceTable.SharedData.Entries)
                {
                    string key = item.Key;
                    var sourceEntry = sourceTable.GetEntry(key);
                    if (sourceEntry == null)
                    {
                        Debug.LogWarningFormat("CopyInternalStringTable() could not find source table entry for key '{0}'", key);
                        continue;
                    }

                    var targetEntry = targetTable.GetEntry(key);
                    if (targetEntry == null)
                    {
                        targetTable.AddEntry(key, sourceEntry.Value);
                        copiedNew++;
                    }
                    else if (targetEntry != null && overwriteExistingKeys)
                    {
                        if (targetTable.RemoveEntry(key))
                        {
                            targetTable.AddEntry(key, sourceEntry.Value);
                            copiedOverwrite++;
                        }
                        else
                        {
                            Debug.LogErrorFormat("CopyInternalStringTable() could not remove key '{0}'. Overwrite failed.", key);
                        }
                    }
                }

                // Set table dirty
                EditorUtility.SetDirty(targetTable);
            }

            // Set target collection shared data dirty
            EditorUtility.SetDirty(targetCollection.SharedData);

            Debug.LogFormat("Source collection '{0}' has a total of {1} entries.\nTarget collection '{2}' received {3} new entries, {4} entries were overwritten.", sourceCollectionName, totalSourceEntries, target, copiedNew, copiedOverwrite);
        }

        /// <summary>
        /// Imports TEXT.RSC strings from classic game data into specified StringTable.
        /// </summary>
        /// <param name="target">Target string table collection name.</param>
        /// <param name="overwriteExistingKeys">When true will overwrite existing keys with source string. When false existing keys are left unchanged.</param>
        public static void CopyTextRSCToStringTable(string target, bool overwriteExistingKeys)
        {
            // Use default Internal_RSC collection with EN locale code as source
            // Note: Internal_RSC is reserved for future use
            string sourceCollectionName = TextManager.defaultInternalRSCCollectionName;

            // Do nothing if target not set
            if (string.IsNullOrEmpty(target))
                return;

            // Target cannot be same as default
            if (string.Compare(target, sourceCollectionName, true) == 0)
            {
                Debug.LogError("CopyTextRSCToStringTable() target cannot be same as default");
                return;
            }

            // Load character mapping table
            Table charMappingTable = null;
            TextAsset mappingTableText = Resources.Load<TextAsset>(textMappingTableFilename);
            if (mappingTableText)
                charMappingTable = new Table(mappingTableText.text);

            // Load default TEXT.RSC file
            TextFile defaultRSC = new TextFile(DaggerfallUnity.Instance.Arena2Path, TextFile.Filename);
            if (defaultRSC == null || defaultRSC.IsLoaded == false)
            {
                Debug.LogError("CopyTextRSCToStringTable() could not find default TEXT.RSC file");
                return;
            }

            // Get target string table collection
            var targetCollection = LocalizationEditorSettings.GetStringTableCollection(target);
            if (targetCollection == null)
            {
                Debug.LogErrorFormat("CopyTextRSCToStringTable() could not find target string table collection '{0}'", target);
                return;
            }

            // Copy source strings to all tables in target collection
            int totalSourceEntries = defaultRSC.RecordCount;
            int copiedNew = 0;
            int copiedOverwrite = 0;
            foreach (StringTable targetTable in targetCollection.StringTables)
            {
                TextFile rsc = defaultRSC;
                TextFile localeRSC = LoadCustomLocaleTextRSC(targetTable.LocaleIdentifier.Code);
                if (localeRSC != null)
                    rsc = localeRSC;

                for (int i = 0; i < defaultRSC.RecordCount; i++)
                {
                    // Extract this record to tokens
                    byte[] buffer = rsc.GetBytesByIndex(i);
                    TextFile.Token[] tokens = TextFile.ReadTokens(ref buffer, 0, TextFile.Formatting.EndOfRecord);

                    // Get token key and text
                    int id = rsc.IndexToId(i);
                    if (id == -1)
                        continue;
                    string key = MakeTextRSCKey(id);
                    string text = ConvertRSCTokensToString(id, tokens);

                    // Remap characters when mapping table present
                    if (charMappingTable != null)
                        text = RemapCharacters(targetTable.LocaleIdentifier.Code, text, charMappingTable);

                    // ID 9000 is diverted to multiple records after initial conversion
                    // Otherwise this record is too long for string table editor
                    // Multiple records also make questionnaire easier to maintain
                    if (id == 9000)
                    {
                        SplitQuestionnaireRecord(text, key, targetTable, overwriteExistingKeys, ref copiedNew, ref copiedOverwrite);
                        continue;
                    }

                    var targetEntry = targetTable.GetEntry(key);
                    if (targetEntry == null)
                    {
                        targetTable.AddEntry(key, text);
                        copiedNew++;
                    }
                    else if (targetEntry != null && overwriteExistingKeys)
                    {
                        if (targetTable.RemoveEntry(key))
                        {
                            targetTable.AddEntry(key, text);
                            copiedOverwrite++;
                        }
                        else
                        {
                            Debug.LogErrorFormat("CopyTextRSCToStringTable() could not remove key '{0}'. Overwrite failed.", key);
                        }
                    }
                }

                // Set table dirty
                EditorUtility.SetDirty(targetTable);
            }

            // Set target collection shared data dirty
            EditorUtility.SetDirty(targetCollection.SharedData);

            Debug.LogFormat("Source collection TEXT.RSC has a total of {0} entries.\nTarget collection '{1}' received {2} new entries, {3} entries were overwritten.", totalSourceEntries, target, copiedNew, copiedOverwrite);
        }

        /// <summary>
        /// Imports FLATS.CFG EN strings from embedded classic game data into specified StringTable.
        /// </summary>
        /// <param name="target">Target string table collection name.</param>
        /// <param name="overwriteExistingKeys">When true will overwrite existing keys with source string. When false existing keys are left unchanged.</param>
        public static void CopyTextFlatsToStringTable(string target, bool overwriteExistingKeys)
        {
            // Do nothing if target not set
            if (string.IsNullOrEmpty(target))
                return;

            // Load default FLATS.CFG file
            FlatsFile flatsFile = new FlatsFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, FlatsFile.Filename), FileUsage.UseMemory, true);
            if (flatsFile == null)
            {
                Debug.LogError("CopyTextFlatsToStringTable() could not find default FLATS.CFG file");
                return;
            }

            // Get target string table collection
            var targetCollection = LocalizationEditorSettings.GetStringTableCollection(target);
            if (targetCollection == null)
            {
                Debug.LogErrorFormat("CopyTextFlatsToStringTable() could not find target string table collection '{0}'", target);
                return;
            }

            // Copy source strings to all tables in target collection
            int totalSourceEntries = flatsFile.FlatsDict.Count;
            int copiedNew = 0;
            int copiedOverwrite = 0;
            foreach (StringTable targetTable in targetCollection.StringTables)
            {
                foreach (var item in flatsFile.FlatsDict)
                {
                    string key = item.Key.ToString();
                    string text = item.Value.caption;

                    var targetEntry = targetTable.GetEntry(key);
                    if (targetEntry == null)
                    {
                        targetTable.AddEntry(key, text);
                        copiedNew++;
                    }
                    else if (targetEntry != null && overwriteExistingKeys)
                    {
                        if (targetTable.RemoveEntry(key))
                        {
                            targetTable.AddEntry(key, text);
                            copiedOverwrite++;
                        }
                        else
                        {
                            Debug.LogErrorFormat("CopyTextFlatsToStringTable() could not remove key '{0}'. Overwrite failed.", key);
                        }
                    }
                }

                // Set table dirty
                EditorUtility.SetDirty(targetTable);
            }

            // Set target collection shared data dirty
            EditorUtility.SetDirty(targetCollection.SharedData);

            Debug.LogFormat("Source collection FLATS.CFG has a total of {0} entries.\nTarget collection '{1}' received {2} new entries, {3} entries were overwritten.", totalSourceEntries, target, copiedNew, copiedOverwrite);
        }

        /// <summary>
        /// Import MAPS.BSA EN location names from embedded game data into specified StringTable.
        /// </summary>
        /// <param name="target">Target string table collection name.</param>
        /// <param name="overwriteExistingKeys">When true will overwrite existing keys with source string. When false existing keys are left unchanged.</param>
        public static void CopyLocationsToStringTable(string target, bool overwriteExistingKeys)
        {
            // Do nothing if target not set
            if (string.IsNullOrEmpty(target))
                return;

            // Load default MAPS.BSA file
            MapsFile mapsFile = new MapsFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, MapsFile.Filename), FileUsage.UseMemory, true);
            if (mapsFile == null)
            {
                Debug.LogError("CopyLocationsToStringTable() could not find default MAPS.BSA file");
                return;
            }

            // Get target string table collection
            var targetCollection = LocalizationEditorSettings.GetStringTableCollection(target);
            if (targetCollection == null)
            {
                Debug.LogErrorFormat("CopyLocationsToStringTable() could not find target string table collection '{0}'", target);
                return;
            }

            int copiedNew = 0;
            int copiedOverwrite = 0;
            foreach (StringTable targetTable in targetCollection.StringTables)
            {
                // Copy all location names across all regions into target string table
                for (int region = 0; region < mapsFile.RegionCount; region++)
                {
                    if (mapsFile.LoadRegion(region))
                    {
                        DFRegion regionData = mapsFile.GetRegion(region);
                        for (int location = 0; location < regionData.LocationCount; location++)
                        {
                            DFLocation locationData = mapsFile.GetLocation(region, location);
                            string key = locationData.MapTableData.MapId.ToString();
                            string text = locationData.Name;

                            var targetEntry = targetTable.GetEntry(key);
                            if (targetEntry == null)
                            {
                                targetTable.AddEntry(key, text);
                                copiedNew++;
                            }
                            else if (targetEntry != null && overwriteExistingKeys)
                            {
                                if (targetTable.RemoveEntry(key))
                                {
                                    targetTable.AddEntry(key, text);
                                    copiedOverwrite++;
                                }
                                else
                                {
                                    Debug.LogErrorFormat("CopyLocationsToStringTable() could not remove key '{0}'. Overwrite failed.", key);
                                }
                            }
                        }
                    }
                }

                // Set table dirty
                EditorUtility.SetDirty(targetTable);
            }

            // Set target collection shared data dirty
            EditorUtility.SetDirty(targetCollection.SharedData);

            Debug.LogFormat("Target collection '{0}' received {1} new entries, {2} entries were overwritten.", target, copiedNew, copiedOverwrite);
        }

        /// <summary>
        /// Import SPELLS.STD spell names into specified StringTable.
        /// </summary>
        /// <param name="target">Target string table collection name.</param>
        /// <param name="overwriteExistingKeys">When true will overwrite existing keys with source string. When false existing keys are left unchanged.</param>
        public static void CopySpellsToStringTable(string target, bool overwriteExistingKeys)
        {
            const string spellsStd = "SPELLS.STD";

            // Do nothing if target not set
            if (string.IsNullOrEmpty(target))
                return;

            // Load default SPELLS.STD file
            List<SpellRecord.SpellRecordData> standardSpells = DaggerfallSpellReader.ReadSpellsFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, spellsStd));
            if (standardSpells == null || standardSpells.Count == 0)
            {
                Debug.LogError("CopySpellsToStringTable() could not find default SPELLS.STD file");
                return;
            }

            // Get target string table collection
            var targetCollection = LocalizationEditorSettings.GetStringTableCollection(target);
            if (targetCollection == null)
            {
                Debug.LogErrorFormat("CopySpellsToStringTable() could not find target string table collection '{0}'", target);
                return;
            }

            int copiedNew = 0;
            int copiedOverwrite = 0;
            foreach (StringTable targetTable in targetCollection.StringTables)
            {
                foreach (var spell in standardSpells)
                {
                    string key = spell.index.ToString();
                    string text = spell.spellName;

                    var targetEntry = targetTable.GetEntry(key);
                    if (targetEntry == null)
                    {
                        targetTable.AddEntry(key, text);
                        copiedNew++;
                    }
                    else if (targetEntry != null && overwriteExistingKeys)
                    {
                        if (targetTable.RemoveEntry(key))
                        {
                            targetTable.AddEntry(key, text);
                            copiedOverwrite++;
                        }
                        else
                        {
                            Debug.LogErrorFormat("CopySpellsToStringTable() could not remove key '{0}'. Overwrite failed.", key);
                        }
                    }
                }

                // Set table dirty
                EditorUtility.SetDirty(targetTable);
            }

            // Set target collection shared data dirty
            EditorUtility.SetDirty(targetCollection.SharedData);

            Debug.LogFormat("Target collection '{0}' received {1} new entries, {2} entries were overwritten.", target, copiedNew, copiedOverwrite);
        }

        /// <summary>
        /// Imports item template names into specified StringTable.
        /// </summary>
        /// <param name="target">Target string table collection name.</param>
        /// <param name="overwriteExistingKeys">When true will overwrite existing keys with source string. When false existing keys are left unchanged.</param>
        public static void CopyItemsToStringTable(string target, bool overwriteExistingKeys)
        {
            // Do nothing if target not set
            if (string.IsNullOrEmpty(target))
                return;

            // Get item template names
            ItemTemplate[] itemTemplates = DaggerfallUnity.Instance.ItemHelper.ItemTemplates;
            if (itemTemplates == null || itemTemplates.Length == 0)
            {
                Debug.LogError("CopyItemsToStringTable() could not get item template data.");
                return;
            }

            // Get target string table collection
            var targetCollection = LocalizationEditorSettings.GetStringTableCollection(target);
            if (targetCollection == null)
            {
                Debug.LogErrorFormat("CopyItemsToStringTable() could not find target string table collection '{0}'", target);
                return;
            }

            int copiedNew = 0;
            int copiedOverwrite = 0;
            foreach (StringTable targetTable in targetCollection.StringTables)
            {
                foreach (ItemTemplate item in itemTemplates)
                {
                    string key = item.index.ToString();
                    string text = item.name;

                    var targetEntry = targetTable.GetEntry(key);
                    if (targetEntry == null)
                    {
                        targetTable.AddEntry(key, text);
                        copiedNew++;
                    }
                    else if (targetEntry != null && overwriteExistingKeys)
                    {
                        if (targetTable.RemoveEntry(key))
                        {
                            targetTable.AddEntry(key, text);
                            copiedOverwrite++;
                        }
                        else
                        {
                            Debug.LogErrorFormat("CopyItemsToStringTable() could not remove key '{0}'. Overwrite failed.", key);
                        }
                    }
                }

                // Set table dirty
                EditorUtility.SetDirty(targetTable);
            }

            // Set target collection shared data dirty
            EditorUtility.SetDirty(targetCollection.SharedData);

            Debug.LogFormat("Target collection '{0}' received {1} new entries, {2} entries were overwritten.", target, copiedNew, copiedOverwrite);
        }

        /// <summary>
        /// Imports magic item template names into specified StringTable.
        /// </summary>
        /// <param name="target">Target string table collection name.</param>
        /// <param name="overwriteExistingKeys">When true will overwrite existing keys with source string. When false existing keys are left unchanged.</param>
        public static void CopyMagicItemsToStringTable(string target, bool overwriteExistingKeys)
        {
            // Do nothing if target not set
            if (string.IsNullOrEmpty(target))
                return;

            // Get all magic item template names including artifacts
            MagicItemTemplate[] magicItemTemplate = DaggerfallUnity.Instance.ItemHelper.MagicItemTemplates;
            if (magicItemTemplate == null || magicItemTemplate.Length == 0)
            {
                Debug.LogError("CopyMagicItemsToStringTable() could not get magic item template data.");
                return;
            }

            // Get target string table collection
            var targetCollection = LocalizationEditorSettings.GetStringTableCollection(target);
            if (targetCollection == null)
            {
                Debug.LogErrorFormat("CopyMagicItemsToStringTable() could not find target string table collection '{0}'", target);
                return;
            }

            int copiedNew = 0;
            int copiedOverwrite = 0;
            foreach (StringTable targetTable in targetCollection.StringTables)
            {
                foreach (MagicItemTemplate item in magicItemTemplate)
                {
                    string key = item.index.ToString();
                    string text = item.name;

                    var targetEntry = targetTable.GetEntry(key);
                    if (targetEntry == null)
                    {
                        targetTable.AddEntry(key, text);
                        copiedNew++;
                    }
                    else if (targetEntry != null && overwriteExistingKeys)
                    {
                        if (targetTable.RemoveEntry(key))
                        {
                            targetTable.AddEntry(key, text);
                            copiedOverwrite++;
                        }
                        else
                        {
                            Debug.LogErrorFormat("CopyMagicItemsToStringTable() could not remove key '{0}'. Overwrite failed.", key);
                        }
                    }
                }

                // Set table dirty
                EditorUtility.SetDirty(targetTable);
            }

            // Set target collection shared data dirty
            EditorUtility.SetDirty(targetCollection.SharedData);

            Debug.LogFormat("Target collection '{0}' received {1} new entries, {2} entries were overwritten.", target, copiedNew, copiedOverwrite);
        }

        /// <summary>
        /// Imports faction names into specified StringTable.
        /// </summary>
        /// <param name="target">Target string table collection name.</param>
        /// <param name="overwriteExistingKeys">When true will overwrite existing keys with source string. When false existing keys are left unchanged.</param>
        public static void CopyFactionsToStringTable(string target, bool overwriteExistingKeys)
        {
            // Do nothing if target not set
            if (string.IsNullOrEmpty(target))
                return;

            // Get faction data
            FactionFile factionFile = new FactionFile(DaggerfallUnity.Instance.ContentReader.GetFactionFilePath(), FileUsage.UseMemory, true);
            if (factionFile == null || factionFile.FactionDict.Count == 0)
            {
                Debug.LogError("CopyFactionsToStringTable() could not get faction data.");
                return;
            }

            // Get target string table collection
            var targetCollection = LocalizationEditorSettings.GetStringTableCollection(target);
            if (targetCollection == null)
            {
                Debug.LogErrorFormat("CopyFactionsToStringTable() could not find target string table collection '{0}'", target);
                return;
            }

            int copiedNew = 0;
            int copiedOverwrite = 0;
            foreach (StringTable targetTable in targetCollection.StringTables)
            {
                foreach (var item in factionFile.FactionDict)
                {
                    string key = item.Key.ToString();
                    string text = item.Value.name;

                    var targetEntry = targetTable.GetEntry(key);
                    if (targetEntry == null)
                    {
                        targetTable.AddEntry(key, text);
                        copiedNew++;
                    }
                    else if (targetEntry != null && overwriteExistingKeys)
                    {
                        if (targetTable.RemoveEntry(key))
                        {
                            targetTable.AddEntry(key, text);
                            copiedOverwrite++;
                        }
                        else
                        {
                            Debug.LogErrorFormat("CopyMagicItemsToStringTable() could not remove key '{0}'. Overwrite failed.", key);
                        }
                    }
                }

                // Set table dirty
                EditorUtility.SetDirty(targetTable);
            }

            // Set target collection shared data dirty
            EditorUtility.SetDirty(targetCollection.SharedData);

            Debug.LogFormat("Target collection '{0}' received {1} new entries, {2} entries were overwritten.", target, copiedNew, copiedOverwrite);
        }

        static void SplitQuestionnaireRecord(string text, string key, StringTable targetTable, bool overwriteExistingKeys, ref int copiedNew, ref int copiedOverwrite)
        {
            string[] splitText = text.Split('{');

            if (splitText == null || splitText.Length == 0)
            {
                Debug.LogErrorFormat("SplitQuestionnaireRecord() found 0 entries.", key);
                return;
            }

            for (int i = 0; i < splitText.Length; i++)
            {
                string itemKey = string.Format("{0}.{1}", key, i);
                string itemText = splitText[i];

                // Discard any empty records and trailing markup end record
                if (string.IsNullOrEmpty(itemText) || string.Compare(markupEndRecord, itemText) == 0)
                    continue;

                var targetEntry = targetTable.GetEntry(itemKey);
                if (targetEntry == null)
                {
                    targetTable.AddEntry(itemKey, itemText);
                    copiedNew++;
                }
                else if (targetEntry != null && overwriteExistingKeys)
                {
                    if (targetTable.RemoveEntry(itemKey))
                    {
                        targetTable.AddEntry(itemKey, itemText);
                        copiedOverwrite++;
                    }
                    else
                    {
                        Debug.LogErrorFormat("SplitQuestionnaireRecord() could not remove key '{0}'. Overwrite failed.", key);
                    }
                }
            }
        }

        /// <summary>
        /// Clear a named StringTable collection.
        /// </summary>
        /// <param name="name">StringTable collection to clear.</param>
        public static void ClearStringTables(string name)
        {
            var collection = LocalizationEditorSettings.GetStringTableCollection(name);
            if (collection == null)
                return;

            // Clear tables in collection
            foreach (StringTable table in collection.StringTables)
            {
                table.Clear();
                EditorUtility.SetDirty(table);
            }

            // Clear shared data entries
            collection.SharedData.Entries.Clear();
            EditorUtility.SetDirty(collection.SharedData);
        }

#endif

        #endregion
    }
}