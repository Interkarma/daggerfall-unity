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

using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Resolves in-place text replacement macros like %var, =var_, __var, etc. for various objects.
    /// More information: http://www.dfworkshop.net/static_files/questing-source-docs.html#qrcsymbols
    /// Caller will need to supply related objects (e.g. quest) as needed.
    /// NOTE: This class will expand over time.
    /// </summary>
    public class MacroHelper
    {
        #region Fields
        #endregion

        #region Structs and Enums

        /// <summary>
        /// A macro to resolve.
        /// </summary>
        struct Macro
        {
            public MacroTypes type;
            public string symbol;
        }

        #endregion

        #region Public Methods
        #endregion

        #region Quests

        /// <summary>
        /// Expands any macros found inside quest message tokens.
        /// </summary>
        /// <param name="parentQuest">Parent quest of message.</param>
        /// <param name="tokens">Array of message tokens to expand macros inside of.</param>
        public void ExpandQuestMessage(Quest parentQuest, ref TextFile.Token[] tokens)
        {
            // Iterate message tokens
            for (int token = 0; token < tokens.Length; token++)
            {
                // Split token text into individual words
                string[] words = GetWords(tokens[token].text);

                // Iterate words to find macros
                for (int word = 0; word < words.Length; word++)
                {
                    Macro macro = MatchMacro(words[word]);
                    if (macro.type == MacroTypes.ContextMacro)
                    {
                        // TODO: Get a quest context macro like %qdt
                    }
                    else
                    {
                        // Ask resource to expand macro if possible
                        QuestResource resource = parentQuest.GetResource(macro.symbol);
                        if (resource != null)
                        {
                            string result;
                            if (resource.ExpandMacro(macro.type, out result))
                            {
                                words[word] = result;
                            }
                        }
                    }

                    // TODO: Need to store previous macro resource for pronomial context expansions
                }

                // Reassemble words and expanded macros back into final token text
                string final = string.Empty;
                for (int i = 0; i < words.Length; i++)
                {
                    final += words[i];
                    if (i != words.Length - 1)
                        final += " ";
                }

                // Store result back into token
                tokens[token].text = final;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Splits text into words.
        /// </summary>
        string[] GetWords(string text)
        {
            return text.Split(' ');
        }


        /// <summary>
        /// Gets a macro from source word.
        /// </summary>
        /// <param name="word">Word to check.</param>
        /// <returns>Macro. Type will be be None if word not matching any macros.</returns>
        Macro MatchMacro(string word)
        {
            // Trim word in case macro at end of sentence
            // Any other conflicting characters?
            // Maybe change this method to use regex later
            word = word.Trim('.');

            // Get macro type
            Macro macro = new Macro();
            if (word.StartsWith("%"))
            {
                macro.type = MacroTypes.ContextMacro;
            }
            if (word.EndsWith("_"))
            {
                if (word.StartsWith("____"))
                    macro.type = MacroTypes.NameMacro4;
                else if (word.StartsWith("___"))
                    macro.type = MacroTypes.NameMacro3;
                else if (word.StartsWith("__"))
                    macro.type = MacroTypes.NameMacro2;
                else if (word.StartsWith("_"))
                    macro.type = MacroTypes.NameMacro1;
                else if (word.StartsWith("=="))
                    macro.type = MacroTypes.FactionMacro;
                else if (word.StartsWith("=#"))
                    macro.type = MacroTypes.BindingMacro;
                else if (word.StartsWith("="))
                    macro.type = MacroTypes.DetailsMacro;
                else
                    macro.type = MacroTypes.None;
            }

            // Assign symbol
            if (macro.type != MacroTypes.None)
                macro.symbol = Parser.GetInnerSymbolName(word);

            return macro;
        }

        #endregion
    }
}