// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
using System.Text.RegularExpressions;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Questing;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;
using UnityEditor;
using UnityEngine;
using UnityScript.Steps;
using DaggerfallWorkshop.Utility;
using UnityEngine.UI;

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
        const string markupInputCursor = "[/input]";
        const string markupSubrecordSeparator = "[/record]";
        const string markupEndRecord = "[/end]";

        /// <summary>
        /// Helper to import TEXT.RSC from classic game data into specified StringTable.
        /// WARNING: Named StringTable collection will be cleared and replaced with data from game files.
        /// </summary>
        /// <param name="name">StringTable collection name to receive TEXT.RSC data.</param>
        public static void ImportTextRSCToStringTables(string name)
        {
            // Clear all tables
            ClearStringTables(name);

            // Load character mapping table
            Table charMappingTable = null;
            TextAsset mappingTableText = Resources.Load<TextAsset>(textMappingTableFilename);
            if (mappingTableText)
                charMappingTable = new Table(mappingTableText.text);

            // Load default TEXT.RSC file
            TextFile defaultRSC = new TextFile(DaggerfallUnity.Instance.Arena2Path, TextFile.Filename);
            if (defaultRSC == null || defaultRSC.IsLoaded == false)
                throw new Exception("Could not load default TEXT.RSC");

            // Get string tables collection
            var collection = LocalizationEditorSettings.GetStringTableCollection(name);
            if (collection == null)
                return;

            // Add all text records to each table
            foreach (StringTable table in collection.StringTables)
            {
                bool en = table.LocaleIdentifier.Code == enLocaleCode;

                TextFile rsc = defaultRSC;
                TextFile localeRSC = LoadCustomLocaleTextRSC(table.LocaleIdentifier.Code);
                if (localeRSC != null)
                    rsc = localeRSC;

                for (int i = 0; i < defaultRSC.RecordCount; i++)
                {
                    // Extract this record to tokens
                    byte[] buffer = rsc.GetBytesByIndex(i);
                    TextFile.Token[] tokens = TextFile.ReadTokens(ref buffer, 0, TextFile.Formatting.EndOfRecord);

                    // Gey token key and text
                    string key = MakeTextRSCKey(rsc.IndexToId(i));
                    string text = ConvertRSCTokensToString(tokens);

                    // Remap characters when mapping table present
                    if (charMappingTable != null)
                        text = RemapCharacters(table.LocaleIdentifier.Code, text, charMappingTable);

                    // Add text to table
                    table.AddEntry(key, text);

                    // Add shared keys only when reading en table
                    // These keys match across entire collection
                    if (en)
                        collection.SharedData.AddKey(key);
                }

                // Set table dirty
                EditorUtility.SetDirty(table);
                Debug.LogFormat("Added {0} TEXT.RSC entries to table {1}", rsc.RecordCount, table.LocaleIdentifier.Code);
            }

            // Set shared data dirty
            EditorUtility.SetDirty(collection.SharedData);
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
        public static string ConvertRSCTokensToString(TextFile.Token[] tokens)
        {
            string recordsText = string.Empty;

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
                        if (trimStartSpaces)
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
                        Debug.LogErrorFormat("Ignoring unexpected RSC formatting token {0}.", tokens[i].formatting.ToString());
                        break;
                }
            }

            // Add any pending text
            if (!string.IsNullOrEmpty(text))
                recordsText = AppendSubrecord(recordsText, text, true);

            return recordsText;
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
                string localeFilename = string.Format("TEXT-{0}.RSC", code);
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
    }
}