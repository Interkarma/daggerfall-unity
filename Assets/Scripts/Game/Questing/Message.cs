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

using UnityEngine;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Arena2;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// A message stores text for popups, journal, letters, rumours, etc.
    /// </summary>
    public class Message
    {
        #region Fields

        const string centerToken = "<ce>";
        const string splitToken = "<--->";

        int id;
        Quest parentQuest = null;
        List<MessageVariant> variants = new List<MessageVariant>();

        #endregion

        #region Properties

        public int ID
        {
            get { return id; }
        }

        public Quest ParentQuest
        {
            get { return parentQuest; }
        }

        public int VariantCount
        {
            get { return variants.Count; }
        }

        public List<MessageVariant> Variants
        {
            get { return variants; }
        }

        #endregion

        #region Structures

        [SerializeField]
        public struct MessageVariant
        {
            public List<TextFile.Token> tokens;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Message(Quest parentQuest)
        {
            this.parentQuest = parentQuest;
        }

        /// <summary>
        /// Load message constructor.
        /// </summary>
        public Message(Quest parentQuest, int id, string[] source)
        {
            this.parentQuest = parentQuest;
            LoadMessage(id, source);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load message contents to tokens and split variants.
        /// </summary>
        /// <param name="id">ID of message.</param>
        /// <param name="source">Array of source lines in message.</param>
        public void LoadMessage(int id, string[] source)
        {
            this.id = id;

            // Read through message lines and create a new variant on split token <--->
            // Variants are used to provide some randomness to text, e.g. rumours
            MessageVariant variant = CreateVariant();
            for (int i = 0; i < source.Length; i++)
            {
                string line = source[i];

                // Handle known justification tokens
                TextFile.Formatting formatting = TextFile.Formatting.Nothing;
                if (line.StartsWith(centerToken))
                {
                    formatting = TextFile.Formatting.JustifyCenter;
                    line = line.Replace(centerToken, "");
                }

                // Trim end of line only and preserve left format if no formatting defined
                // Otherwise trim whole line and use formatting specified
                if (formatting == TextFile.Formatting.Nothing)
                    line = line.TrimEnd();
                else
                    line = line.Trim();

                // Look for split token to start new variant
                if (line.Contains(splitToken))
                {
                    variants.Add(variant);
                    variant = CreateVariant();
                    continue;
                }

                // Add tokens
                AddToken(TextFile.Formatting.Text, line, variant.tokens);
                AddToken(formatting, variant.tokens);
            }

            // Add final variant
            variants.Add(variant);
        }

        /// <summary>
        /// Replace a message with new source lines (e.g. from translation data).
        /// </summary>
        /// <param name="id">ID of message.</param>
        /// <param name="source">Array of source lines in message.</param>
        public void ReplaceMessage(int id, string[] source)
        {
            variants.Clear();
            LoadMessage(id, source);
        }

        /// <summary>
        /// Gets text tokens for this message.
        /// Will return a random variant if variant = -1 and more than one variant exists.
        /// If only a single variant then same text tokens are returned each time.
        /// It is safe to leave variant as -1 if you don't care about the variant you receive.
        /// </summary>
        /// <param name="randomVariant">True for a random variant, otherwise use first and only variant.</param>
        /// <param name="expandMacros">True to expand text macros like %foo and __foo_.</param>
        /// <returns>Array of text tokens.</returns>
        public TextFile.Token[] GetTextTokens(int variant = -1, bool expandMacros = true)
        {
            // Randomise variant
            int index;
            if (variant == -1)
                index = Random.Range(0, VariantCount);
            else
                index = 0;

            // Get token array
            TextFile.Token[] tokens = variants[index].tokens.ToArray();

            // Expand macros
            if (expandMacros)
            {
                QuestMacroHelper macroHelper = new QuestMacroHelper();

                ParentQuest.CurrentLogMessageId = this.id;

                // note Nystul: reveal dialog linked resources here on purpose (quest popups should reveal them: see this issue: https://forums.dfworkshop.net/viewtopic.php?f=24&t=1678&p=22069#p22069)
                macroHelper.ExpandQuestMessage(ParentQuest, ref tokens, true);

                ParentQuest.CurrentLogMessageId = -1;
            }

            return tokens;
        }

        /// <summary>
        /// Gets text tokens for this message for specified variant.
        /// Will return the specified variant
        /// </summary>
        /// <param name="variant">the variant of interest.</param>
        /// <param name="expandMacros">True to expand text macros like %foo and __foo_.</param>
        /// <returns>Array of text tokens.</returns>
        public TextFile.Token[] GetTextTokensByVariant(int variant = 0, bool expandMacros = true)
        {
            // Get token array
            TextFile.Token[] tokens = variants[variant].tokens.ToArray();

            // Expand macros
            if (expandMacros)
            {
                QuestMacroHelper macroHelper = new QuestMacroHelper();
                macroHelper.ExpandQuestMessage(ParentQuest, ref tokens);
            }

            return tokens;
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct MessageSaveData_v1
        {
            public int id;
            public string[] lines;
        }

        public MessageSaveData_v1 GetSaveData()
        {
            // Convert lines back to source format
            List<string> lines = new List<string>();
            for (int variant = 0; variant < variants.Count; variant++)
            {
                // Add split token for subsequent variants
                if (variant > 0)
                    lines.Add(splitToken);

                bool foundText = false;
                string currentLine = string.Empty;
                foreach (TextFile.Token token in variants[variant].tokens)
                {
                    switch (token.formatting)
                    {
                        case TextFile.Formatting.Text:
                            // Found another text token without a formatting line break - need to break to a new line
                            if (foundText)
                            {
                                lines.Add(currentLine);
                                currentLine = string.Empty;
                                foundText = false;
                                continue;
                            }
                            // Just add the text
                            currentLine += token.text;
                            foundText = true;
                            break;

                        case TextFile.Formatting.JustifyCenter:
                            // Prepend formatting token and start new line
                            currentLine = centerToken + currentLine;
                            lines.Add(currentLine);
                            currentLine = string.Empty;
                            foundText = false;
                            break;

                        case TextFile.Formatting.Nothing:
                            // Probably last token in stream - add line and continue
                            lines.Add(currentLine);
                            currentLine = string.Empty;
                            foundText = false;
                            continue;

                        default:
                            throw new System.Exception(string.Format("Message.GetSaveData() encountered unexpected formatting token {0}", token.formatting));
                    }
                }
            }

            // Create save data
            MessageSaveData_v1 data = new MessageSaveData_v1();
            data.id = id;
            data.lines = lines.ToArray();

            return data;
        }

        public void RestoreSaveData(MessageSaveData_v1 data)
        {
            LoadMessage(data.id, data.lines);
        }

        #endregion

        #region Private Methods

        MessageVariant CreateVariant()
        {
            MessageVariant variant = new MessageVariant();
            variant.tokens = new List<TextFile.Token>();

            return variant;
        }

        void AddToken(TextFile.Formatting formatting, List<TextFile.Token> tokenList)
        {
            AddToken(formatting, string.Empty, tokenList);
        }

        void AddToken(TextFile.Formatting formatting, string text, List<TextFile.Token> tokenList)
        {
            TextFile.Token token = new TextFile.Token();
            token.formatting = formatting;
            token.text = text;
            tokenList.Add(token);
        }

        #endregion
    }
}