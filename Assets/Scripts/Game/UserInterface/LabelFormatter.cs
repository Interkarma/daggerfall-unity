// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Helper class to uniformly reformat token streams into a line-wrapping TextLabel.
    /// -Problem description:
    ///   Text in classic Daggerfall uses hard-coded positions, line breaks, and other justification tokens which assume
    ///   text is written only with a certain size pixel font and only over a 320x200 display area.
    ///   This is very problematic for correct translation support as international text require fonts
    ///   of different proportions and each language requires variable distance to express the same concepts.
    ///   To make matter worse, embedded formatting is broken in several places such as books, where text can become
    ///   aligned oddly or even overflow the display area completely.
    ///   A general solution is needed that can reformat text from various sources (quests, books, TEXT.RSC, etc.)
    ///   into a new layout that will wrap words cleanly and use line breaks only when required.
    /// -Earlier implementation:
    ///   Daggerfall Unity first implemented true classic pixel font support and followed the same formatting tokens
    ///   precisely. This resulted in a perfect recreation of classic Daggerfall's user interface, but also
    ///   inherited the same general limitations. When SDF fonts were implemented, they still followed the same
    ///   formatting as classic. Glyphs were even stretched/squished to occupy the same spaces.
    ///   While this made text somewhat clearer to read, formatting remained uncomfortable and inflexible.
    /// -Iterated implementation:
    ///   The only way forward to correctly support translations will first require cleaner text formatting.
    ///   This is a multi-stage process that first implementes new font and formatting support.
    ///   It is necessary for this system to support both classic and SDF pathways with either
    ///   default fonts or custom fonts for non-Latin characters. Ideally this will support text
    ///   read from classic binary data in addition to new language databases.
    /// -Purpose of this helper class:
    ///   This class is an attempt at creating a tool to automatically reformat classic text data from a
    ///   variety of binary sources into a clean word-wrapped format. Its purpose is only to reformat
    ///   classic text from the DOS binaries and classic quests to output text in a clean and readable
    ///   manner for both classic and SDF font pathways.
    ///   This class is not intended to be used with text sourced from translated text databases (in development)
    ///   as this text will be written without classic formatting tokens and with normal word wrapping already in mind.
    /// -Supported classic sources:
    ///   Initial support will be for automatically reformatting books. This will be extended to other areas
    ///   where possible using lessons learned. Automatic text reformatting from classic binary data and quests
    ///   will always remain "best effort". Eventually all in-game text will be migrated to a default text database
    ///   at which point formatting can be cleaned up further.
    /// </summary>
    public class LabelFormatter
    {
        #region Fields

        List<TextGroup> groups = new List<TextGroup>();
        string space = " ";

        #endregion

        #region Properties

        /// <summary>
        /// Gets number of labels formatted so far.
        /// </summary>
        public int Count
        {
            get { return groups.Count; }
        }

        #endregion

        #region Structs & Enums

        /// <summary>
        /// Sources of text tokens to inform format choices.
        /// </summary>
        public enum TextSources
        {
            Book,
        }

        /// <summary>
        /// Represents each distinct label.
        /// </summary>
        struct TextGroup
        {
            public string text;
            public DaggerfallFont font;
            public HorizontalAlignment alignment;
            public Color color;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clear formatter of all data.
        /// </summary>
        public void Clear()
        {
            groups.Clear();
        }

        /// <summary>
        /// Create labels from formatted data.
        /// Note: If using TextLabel directly, consumer must set TextLabel.MaxWidth to suit layout area.
        /// </summary>
        public List<TextLabel> CreateLabels()
        {
            List<TextLabel> labels = new List<TextLabel>();

            foreach(TextGroup group in groups)
            {
                // Every group is cast into a word-wrapping label
                TextLabel label = new TextLabel();
                label.Font = group.font;
                label.HorizontalAlignment = group.alignment;
                label.TextColor = group.color;
                label.ShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
                label.ShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
                label.WrapText = true;
                label.WrapWords = true;
                label.Text = group.text;
                if (label.HorizontalAlignment == HorizontalAlignment.Center)
                    label.HorizontalTextAlignment = TextLabel.HorizontalTextAlignmentSetting.Center;
                labels.Add(label);
            }

            return labels;
        }

        #endregion

        #region Books

        // Notes:
        //  Book files have text pre-paginated based on classic 320x200.
        //  This has no meaning with custom fonts and languages as book might require more or fewer pages.
        //  Reformat process will structure book instead as a list of word-wrapped labels and breaks.
        //  Book reader UI can then break these back into pages based on fixed text height.
        //  For some reason the first line of first paragraph of many books is centre justified.
        //  After reformatting this results in entire first paragraph being centre justified.

        public bool ReformatBook(int id)
        {
            return ReformatBook(DaggerfallUnity.Instance.ItemHelper.GetBookFileName(id));
        }

        public bool ReformatBook(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;

            // Try to open book
            BookFile book = new BookFile();
            if (!BookReplacement.TryImportBook(filename, book) &&
                !book.OpenBook(DaggerfallUnity.Instance.Arena2Path, filename))
                return false;

            // Clear existing
            Clear();

            // Combine all tokens from all pages
            // The classic concept of pages becomes
            List<TextFile.Token> allPageTokens = new List<TextFile.Token>();
            for (int page = 0; page < book.PageCount; page++)
            {
                TextFile.Token[] tokens = book.GetPageTokens(page);
                foreach(TextFile.Token token in tokens)
                {
                    allPageTokens.Add(token);
                }
            }

            // Read all tokens and merge into text groups
            DaggerfallFont prevFont = DaggerfallUI.DefaultFont;
            TextFile.Token prevToken = new TextFile.Token(TextFile.Formatting.Nothing);
            TextGroup workingGroup = CreateEmptyTextGroup(prevFont);
            foreach (TextFile.Token token in allPageTokens)
            {
                switch (token.formatting)
                {
                    // Set font on current group
                    case TextFile.Formatting.FontPrefix:
                        workingGroup.font = prevFont = DaggerfallUI.Instance.GetFont(token.x);
                        break;

                    // Text is added to working group
                    case TextFile.Formatting.Text:
                        workingGroup.text += token.text;
                        break;

                    // Newline becomes a space unless previous token was also newline
                    // This will be treated as a paragraph break
                    case TextFile.Formatting.NewLine:
                        if (prevToken.formatting == TextFile.Formatting.NewLine)
                        {
                            StoreGroup(workingGroup);
                            StoreLineBreakGroup();
                            workingGroup = CreateEmptyTextGroup(prevFont);
                        }
                        else
                        {
                            workingGroup.text += space;
                        }
                        break;

                    // Set left justify on current group
                    case TextFile.Formatting.JustifyLeft:
                        workingGroup.alignment = HorizontalAlignment.None;
                        break;

                    // Set centre justify on current group
                    case TextFile.Formatting.JustifyCenter:
                        workingGroup.alignment = HorizontalAlignment.Center;
                        break;
                }

                prevToken = token;
            }

            return true;
        }

        void StoreGroup(TextGroup group)
        {
            // Reject group if no text
            if (string.IsNullOrEmpty(group.text))
                return;

            groups.Add(group);
        }

        void StoreLineBreakGroup(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                TextGroup group = CreateEmptyTextGroup();
                group.text = space;
                groups.Add(group);
            }
        }

        #endregion

        #region Private Methods

        TextGroup CreateEmptyTextGroup(DaggerfallFont font = null)
        {
            if (font == null)
                font = DaggerfallUI.DefaultFont;

            return new TextGroup()
            {
                text = string.Empty,
                font = font,
                alignment = HorizontalAlignment.None,
                color = DaggerfallUI.DaggerfallDefaultTextColor,
            };
        }

        #endregion
    }
}