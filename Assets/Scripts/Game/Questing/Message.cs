// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.Questing
{
    /// <summary>
    /// A message stores text for popups, journal, letters, rumours, etc.
    /// </summary>
    public class Message : QuestResource
    {
        #region Fields

        int id;
        List<MessageVariant> variants = new List<MessageVariant>();

        #endregion

        #region Properties

        public int ID
        {
            get { return id; }
        }

        public int VariantCount
        {
            get { return variants.Count; }
        }

        #endregion

        #region Structures

        struct MessageVariant
        {
            public List<TextFile.Token> tokens;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Message(Quest parentQuest)
            : base(parentQuest)
        {
        }

        /// <summary>
        /// Load message constructor.
        /// </summary>
        public Message(Quest parentQuest, int id, string[] source)
            : base(parentQuest)
        {
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
            const string splitToken = "<--->";
            const string centerToken = "<ce>";

            this.id = id;

            if (id == 1010)
            {
                int foo = 0;
            }

            // Read through message lines and create a new variant on split token <--->
            // Variants are used to provide some randomness to text, e.g. rumours
            MessageVariant variant = CreateVariant();
            for (int i = 0; i < source.Length; i++)
            {
                string line = source[i];

                // Handle known justification tokens
                TextFile.Formatting formatting = TextFile.Formatting.JustifyCenter;
                if (line.StartsWith(centerToken))
                {
                    formatting = TextFile.Formatting.JustifyCenter;
                    line = line.Replace(centerToken, "");
                }

                // Trim white space either side of line
                line = line.Trim();

                // Look for split token to start new variant
                if (line == splitToken)
                {
                    variants.Add(variant);
                    variant = CreateVariant();
                }

                // TODO: Handle %var text

                // TODO: Handle _symbol_ text

                // TODO: Handle =string_ text

                // Add formatting token
                TextFile.Token formattingToken = new TextFile.Token();
                formattingToken.formatting = formatting;
                variant.tokens.Add(formattingToken);

                // Add text token
                TextFile.Token textToken = new TextFile.Token();
                textToken.formatting = TextFile.Formatting.Text;
                textToken.text = line;
                variant.tokens.Add(textToken);
            }

            // Add final variant
            variants.Add(variant);
        }

        /// <summary>
        /// Gets text tokens for this message.
        /// Will return a random variant if variant = -1 and more than one variant exists.
        /// If only a single variant then same text tokens are returned each time.
        /// It is safe to leave variant as -1 if you don't care about the variant you receive.
        /// </summary>
        /// <param name="randomVariant">True for a random variant, otherwise use first and only variant.</param>
        /// <returns>Array of text tokens.</returns>
        public TextFile.Token[] GetTextTokens(int variant = -1)
        {
            // Randomise variant
            int index;
            if (variant == -1)
                index = Random.Range(0, VariantCount);
            else
                index = 0;

            return variants[index].tokens.ToArray();
        }

        #endregion

        #region Private Methods

        MessageVariant CreateVariant()
        {
            MessageVariant variant = new MessageVariant();
            variant.tokens = new List<TextFile.Token>();

            return variant;
        }

        #endregion
    }
}