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
using System.IO;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;
using UnityEditor;
using UnityEngine;
using Mono.CSharp;

namespace DaggerfallWorkshop.Localization
{
    /// <summary>
    /// Helper to import classic text data into StringTables.
    /// </summary>
    public static class DaggerfallStringTableImporter
    {
        const string enLocaleCode = "en";
        const string newline = "\n";

        // Markup for token conversion
        // Markup format is designed with the following requirements:
        //  1. Don't overcomplicate - must be simple to understand and edit using plain text
        //  2. Support all classic Daggerfall text data (excluding books which already have custom format)
        //  3. Convert easily between RSC tokens and markup as required
        //  4. Formatting must not conflict with regular text entry in any language
        const string markupJustifyLeft = "[/left]";
        const string markupJustifyCenter = "[/center]";
        const string markupNewLine = "[/newline]";
        const string markupTextPosition = "[/pos:x={0},y={1}]";
        const string markupInputCursor = "[/input]";
        const string markupSubrecordSeparator = "[/record]";

        /// <summary>
        /// Helper to import TEXT.RSC from classic game data into specified StringTable.
        /// WARNING: Named StringTable collection will be cleared and replaced with data from game files.
        /// </summary>
        /// <param name="name">StringTable collection name to receive TEXT.RSC data.</param>
        public static void ImportTextRSCToStringTables(string name)
        {
            // Clear all tables
            ClearStringTables(name);

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

                    // Add text to table
                    string key = MakeTextRSCKey(rsc.IndexToId(i));
                    string text = ConvertRSCTokensToString(tokens);
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
        /// UNDER ACTIVE DEVELOPMENT
        /// Converts RSC tokens into a string. Tokens will be converted into simple markup.
        /// This TextElement list can be converted back into the original RSC token stream.
        /// </summary>
        /// <param name="tokens">RSC token input.</param>
        /// <returns>List of TextElement markup converted from RSC tokens.</returns>
        public static string ConvertRSCTokensToString(TextFile.Token[] tokens)
        {
            string recordsText = string.Empty;

            // Convert RSC formatting tokens into markup text for easier human editing
            // Expect significant evolution of this before editor is completed
            string text = string.Empty;
            for (int i = 0; i < tokens.Length; i++)
            {
                switch (tokens[i].formatting)
                {
                    case TextFile.Formatting.Text:
                        text += tokens[i].text;
                        break;
                    case TextFile.Formatting.JustifyLeft:
                        text += markupJustifyLeft + newline;
                        break;
                    case TextFile.Formatting.JustifyCenter:
                        text += markupJustifyCenter + newline;
                        break;
                    case TextFile.Formatting.NewLine:
                        text += markupNewLine + newline;
                        break;
                    case TextFile.Formatting.PositionPrefix:
                        text += string.Format(markupTextPosition, tokens[i].x, tokens[i].y);
                        break;
                    case TextFile.Formatting.SubrecordSeparator:
                        recordsText = AppendSubrecord(recordsText, text) + newline;
                        text = string.Empty;
                        break;
                    case TextFile.Formatting.InputCursorPositioner:
                        text += markupInputCursor;
                        break;
                    default:
                        Debug.LogErrorFormat("Unexpected RSC formatting token encountered {0}. Ignoring.", tokens[i].formatting.ToString());
                        break;
                }
            }

            // Add pending text (if any)
            if (!string.IsNullOrEmpty(text))
                recordsText = AppendSubrecord(recordsText, text);

            return recordsText;
        }

        /// <summary>
        /// Append subrecord text to combined string.
        /// Subrecords are split using separator markup.
        /// </summary>
        /// <param name="current">Current combined text.</param>
        /// <param name="add">Incoming subrecord text to add.</param>
        /// <returns>Combined string.</returns>
        static string AppendSubrecord(string current, string add)
        {
            return string.Format("{0}{1}{2}", current, add, markupSubrecordSeparator);
        }

        /// <summary>
        /// UNDER ACTIVE DEVELOPMENT
        /// Converts TextElement list back into RSC tokens.
        /// </summary>
        /// <param name="textElements">TextElements input.</param>
        /// <returns>Array of RSC tokens converted from TextElements.</returns>
        public static TextFile.Token[] ConvertTextElementsToRSCTokens(List<TextElement> textElements)
        {
            throw new NotImplementedException();
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

        #endregion
    }
}