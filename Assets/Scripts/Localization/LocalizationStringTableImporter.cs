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
using DaggerfallConnect.Arena2;
using UnityEngine.Localization.Tables;
using UnityEditor;

namespace DaggerfallWorkshop.Localization
{
    /// <summary>
    /// Helper to import classic text data into StringTables.
    /// </summary>
    public static class DaggerfallStringTableImporter
    {
        // Markup for token conversion
        // Markup format is designed with the following requirements:
        //  1. Don't overcomplicate - must be simple to understand and edit using plain text
        //  2. Support all classic Daggerfall text data (excluding books which already have custom format)
        //  3. Convert easily between RSC tokens and markup as required
        //  4. Formatting must not conflict with regular text entry in any language
        const string markupJustifyLeft = "[/left]";
        const string markupJustifyCenter = "[/center]";
        const string markupNewLine = "\\n";
        const string markupTextPosition = "[/pos:x={0},y={1}]";
        const string markupInputCursor = "[/input]";
        const string markupSubrecordSeparator = "[/record]";

        /// <summary>
        /// Helper to import TEXT.RSC from classic game data into specified StringTable.
        /// Existing entries will be replaced.
        /// </summary>
        /// <param name="db">StringTable to receive TEXT.RSC data.</param>
        public static void ImportTextRSC(StringTable target)
        {
            // Validate
            if (!target)
                return;

            // Load TEXT.RSC file
            TextFile rsc = new TextFile(DaggerfallUnity.Instance.Arena2Path, TextFile.Filename);
            if (rsc == null || rsc.IsLoaded == false)
                throw new Exception("Could not load TEXT.RSC");

            // Iterate records
            int overwriteCount = 0;
            for (int i = 0; i < rsc.RecordCount; i++)
            {
                // Extract this record to tokens
                byte[] buffer = rsc.GetBytesByIndex(i);
                TextFile.Token[] tokens = TextFile.ReadTokens(ref buffer, 0, TextFile.Formatting.EndOfRecord);

                // Get key and remove duplicate if one exists
                string key = MakeTextRSCKey(rsc.IndexToId(i));
                StringTableEntry currentEntry = target.GetEntry(key);
                if (currentEntry != null)
                {
                    //target.RemoveEntry(key);
                    overwriteCount++;
                }

                string text = ConvertRSCTokensToString(tokens);
                target.AddEntry(key, text);
            }

            EditorUtility.SetDirty(target);

            UnityEngine.Debug.LogFormat("Added {0} TEXT.RSC entries to table with {1} overwrites.", rsc.RecordCount, overwriteCount);
        }

        /// <summary>
        /// Clear a StringTable.
        /// </summary>
        /// <param name="target">StringTable to clear.</param>
        public static void ClearTable(StringTable target)
        {
            if (target)
            {
                target.Clear();
                EditorUtility.SetDirty(target);
            }
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
                        text += markupJustifyLeft;
                        break;
                    case TextFile.Formatting.JustifyCenter:
                        text += markupJustifyCenter;
                        break;
                    case TextFile.Formatting.NewLine:
                        text += markupNewLine;
                        break;
                    case TextFile.Formatting.PositionPrefix:
                        text += string.Format(markupTextPosition, tokens[i].x, tokens[i].y);
                        break;
                    case TextFile.Formatting.SubrecordSeparator:
                        recordsText = AppendSubrecord(recordsText, text);
                        text = string.Empty;
                        break;
                    case TextFile.Formatting.InputCursorPositioner:
                        text += markupInputCursor;
                        break;
                    default:
                        throw new Exception(string.Format("Unexpected RSC formatting token encountered {0}", tokens[i].formatting.ToString()));
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

        #endregion
    }
}