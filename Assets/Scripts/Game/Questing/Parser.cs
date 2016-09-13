using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// Reads a quest source file and generates new quest object.
    /// Final step of parsing may be handed off to components as text at construction time.
    /// This allows parser to handle high-level structure while components take care of their own specific needs.
    /// </summary>
    public static class Parser
    {
        #region Public Methods

        /// <summary>
        /// Attempts to parse a text source file.
        /// </summary>
        /// <param name="source">Array of text lines from quest source.</param>
        public static void Parse(string[] source)
        {
            const StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;

            string questName = string.Empty;
            bool inQRC = false;
            bool inQBN = false;
            List<string> qrcLines = new List<string>();
            List<string> qbnLines = new List<string>();

            // First pass gathers basic information and structure
            // Basic structure is:
            //   <Header>
            //   <QRC>
            //   <QBN>
            foreach(string line in source)
            {
                // Trim trailing white space from either end of source line data
                string text = line.Trim();

                // Skip comment lines
                if (text.StartsWith("--", comparison))
                    continue;

                // Handle basic structure
                if (text.StartsWith("quest:", comparison))
                {
                    questName = GetFieldStringValue(text);
                }
                else if (text.StartsWith("qrc:", comparison))
                {
                    inQRC = true;
                    inQBN = false;
                    continue;
                }
                else if (text.StartsWith("qbn:", comparison))
                {
                    inQRC = false;
                    inQBN = false;
                    continue;
                }

                // Add full lines to QRC section
                if (inQRC)
                {
                    qrcLines.Add(line);
                }

                // Add full lines to QBN section
                if (inQBN)
                {
                    qbnLines.Add(line);
                }
            }

            // Validate name
            if (string.IsNullOrEmpty(questName))
            {
                throw new Exception("Parse() error: Quest has no name.");
            }

            // Validate QRC
            if (qrcLines.Count == 0)
            {
                throw new Exception("Parse() error: Quest has no QRC section.");
            }

            // Validate QBN
            if (qrcLines.Count == 0)
            {
                throw new Exception("Parse() error: Quest has no QBN section.");
            }

            // Parse QRC
            ParseQRC(qrcLines);

            // Parse QBN
            ParseQBN(qbnLines);
        }

        #endregion

        #region Private Methods

        static void ParseQRC(List<string> lines)
        {

        }

        static void ParseQBN(List<string> lines)
        {

        }

        #endregion

        #region Helpers

        // Splits a field with format 'FieldName: Value" using : as separator
        // Be default expects two string values as result
        static string[] SplitField(string text, int expectedCount = 2)
        {
            const string error = "SplitField() encountered invalid number of results.";

            string[] parts = text.Split(':');
            if (parts.Length != expectedCount)
                throw new Exception(error);

            return parts;
        }

        // Gets string value from text field with format 'FieldName: String'
        static string GetFieldStringValue(string text)
        {
            string[] parts = SplitField(text);
            return parts[1].Trim();
        }

        // Gets int value from text field with format 'FieldName: Int'
        static int GetFieldIntValue(string text)
        {
            string[] parts = SplitField(text);
            return ParseInt(parts[1].Trim());
        }

        // Just a wrapper for int.Parse
        static int ParseInt(string text)
        {
            return int.Parse(text);
        }

        #endregion
    }
}