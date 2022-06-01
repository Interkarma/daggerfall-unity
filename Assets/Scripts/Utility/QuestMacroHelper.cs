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
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Resolves in-place text replacement macros like %var, =var_, __var, etc. for various objects.
    /// More information: http://www.dfworkshop.net/static_files/questing-source-docs.html#qrcsymbols
    /// Caller will need to supply related objects (e.g. quest) as needed.
    /// NOTE: This class will expand over time.
    /// </summary>
    public class QuestMacroHelper
    {
        #region Fields
        #endregion

        #region Structs and Enums

        /// <summary>
        /// A macro to resolve.
        /// </summary>
        struct Macro
        {
            public string token;            // String token of any macro found (e.g. "__symbol_")
            public MacroTypes type;         // Type of macro found (MacroTypes.None if not found)
            public string symbol;           // Inner symbol of macro
            public int index;               // Index of first macro character
            public int length;              // Length of macro text from index
        }

        #endregion

        #region Public Methods
        #endregion

        #region Quests

        /// <summary>
        /// Gets reference to all QuestResource objects referenced by message macros
        /// </summary>
        /// <param name="message">Message to extract resources from.</param>
        /// <returns>QuestResource array or null.</returns>
        public QuestResource[] GetMessageResources(Message message)
        {
            // Must have a message
            if (message == null)
                return null;

            // Get raw message and enumerate resource tokens
            List<QuestResource> resources = new List<QuestResource>();
            TextFile.Token[] tokens = message.GetTextTokens(-1, false);
            for (int token = 0; token < tokens.Length; token++)
            {
                string[] words = GetWords(tokens[token].text);
                for (int word = 0; word < words.Length; word++)
                {
                    Macro macro = GetMacro(words[word]);
                    QuestResource resource = message.ParentQuest.GetResource(macro.symbol);
                    if (resource != null)
                        resources.Add(resource);
                }
            }

            return resources.ToArray();
        }

        /// <summary>
        /// Expands any macros found inside quest message tokens.
        /// </summary>
        /// <param name="parentQuest">Parent quest of message.</param>
        /// <param name="tokens">Array of message tokens to expand macros inside of.</param>
        /// <param name="resolveDialogLinks">will reveal dialog linked resources in talk window (this must be false for all calls to this function except if caller is talk manager when expanding answers or for quest popups).</param>
        public void ExpandQuestMessage(Quest parentQuest, ref TextFile.Token[] tokens, bool revealDialogLinks = false)
        {
            // Iterate message tokens
            for (int token = 0; token < tokens.Length; token++)
            {
                // Split token text into individual words
                string[] words = GetWords(tokens[token].text);

                // Iterate words to find macros
                for (int word = 0; word < words.Length; word++)
                {
                    Macro macro = GetMacro(words[word]);
                    if (macro.type == MacroTypes.ContextMacro)
                    {
                        words[word] = words[word].Replace(macro.token, MacroHelper.GetValue(macro.token, parentQuest, parentQuest.ExternalMCP));
                    }
                    else
                    {
                        // fix for bug when parent quest is no longer available (http://forums.dfworkshop.net/viewtopic.php?f=24&t=1002) in case
                        // quest injected entries and rumors stay in list due to other bugs
                        // so parentQuest is no longer available
                        if (parentQuest == null)
                            return;

                        // Ask resource to expand macro if possible
                        QuestResource resource = parentQuest.GetResource(macro.symbol);
                        if (resource != null)
                        {
                            string result;
                            if (resource.ExpandMacro(macro.type, out result))
                            {
                                words[word] = words[word].Replace(macro.token, result);
                            }

                            // reveal dialog linked resources in talk window
                            if (revealDialogLinks && macro.type == MacroTypes.NameMacro1) // only resolve if their true name was expanded (given) which is MacroTypes.NameMacro1
                            {
                                System.Type t = resource.GetType();
                                if (t.Equals(typeof(DaggerfallWorkshop.Game.Questing.Place)))
                                {
                                        GameManager.Instance.TalkManager.AddDialogForQuestInfoResource(parentQuest.UID, macro.symbol, TalkManager.QuestInfoResourceType.Location);
                                }
                                else if (t.Equals(typeof(DaggerfallWorkshop.Game.Questing.Person)))
                                {
                                        GameManager.Instance.TalkManager.AddDialogForQuestInfoResource(parentQuest.UID, macro.symbol, TalkManager.QuestInfoResourceType.Person);
                                }
                                else if (t.Equals(typeof(DaggerfallWorkshop.Game.Questing.Item)))
                                {
                                        GameManager.Instance.TalkManager.AddDialogForQuestInfoResource(parentQuest.UID, macro.symbol, TalkManager.QuestInfoResourceType.Thing);
                                }
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

        public void ExpandQuestString(Quest parentQuest, ref string questString)
        {
            Macro macro = GetMacro(questString);
            if (macro.type == MacroTypes.ContextMacro)
            {
                questString = questString.Replace(macro.token, MacroHelper.GetValue(macro.token, parentQuest));
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
        /// Attempts to get macro data from a word string.
        /// Only a single macro will be matched per word.
        /// </summary>
        /// <param name="word">Source word to inspect for macro.</param>
        /// <returns>Macro data, if a macro is found. Type will be be MacroTypes.None if no macro found in text.</returns>
        Macro GetMacro(string word)
        {
            string pattern = @"(?<prefix>____)(?<NameMacro4_Symbol>[a-zA-Z0-9.]+)(?<suffix>_)|" +
                             @"(?<prefix>___)(?<NameMacro3_Symbol>[a-zA-Z0-9.]+)(?<suffix>_)|" +
                             @"(?<prefix>__)(?<NameMacro2_Symbol>[a-zA-Z0-9.]+)(?<suffix>_)|" +
                             @"(?<prefix>_)(?<NameMacro1_Symbol>[a-zA-Z0-9.]+)(?<suffix>_)|" +
                             @"(?<prefix>==)(?<FactionMacro_Symbol>[a-zA-Z0-9.]+)(?<suffix>_)|" +
                             @"(?<prefix>=#)(?<BindingMacro_Symbol>[a-zA-Z0-9.]+)(?<suffix>_)|" +
                             @"(?<prefix>=)(?<DetailsMacro_Symbol>[a-zA-Z0-9.]+)(?<suffix>_)|" +
                             @"(?<prefix>%)(?<ContextMacro_Symbol>\w+)";

            // Mactch macro type and inner symbol
            Macro macro = new Macro();
            Match match = Regex.Match(word, pattern);
            if (match.Success)
            {
                // Get possible groups from pattern to isolate match value
                Group foundGroup = null;
                Group NameMacro4_Group = match.Groups["NameMacro4_Symbol"];
                Group NameMacro3_Group = match.Groups["NameMacro3_Symbol"];
                Group NameMacro2_Group = match.Groups["NameMacro2_Symbol"];
                Group NameMacro1_Group = match.Groups["NameMacro1_Symbol"];
                Group FactionMacro_Group = match.Groups["FactionMacro_Symbol"];
                Group BindingMacro_Group = match.Groups["BindingMacro_Symbol"];
                Group DetailsMacro_Group = match.Groups["DetailsMacro_Symbol"];
                Group ContextMacro_Group = match.Groups["ContextMacro_Symbol"];

                // Check which match group (if any) was found
                if (!string.IsNullOrEmpty(NameMacro4_Group.Value))
                {
                    macro.type = MacroTypes.NameMacro4;
                    foundGroup = NameMacro4_Group;
                }
                else if (!string.IsNullOrEmpty(NameMacro3_Group.Value))
                {
                    macro.type = MacroTypes.NameMacro3;
                    foundGroup = NameMacro3_Group;
                }
                else if (!string.IsNullOrEmpty(NameMacro2_Group.Value))
                {
                    macro.type = MacroTypes.NameMacro2;
                    foundGroup = NameMacro2_Group;
                }
                else if (!string.IsNullOrEmpty(NameMacro1_Group.Value))
                {
                    macro.type = MacroTypes.NameMacro1;
                    foundGroup = NameMacro1_Group;
                }
                else if (!string.IsNullOrEmpty(FactionMacro_Group.Value))
                {
                    macro.type = MacroTypes.FactionMacro;
                    foundGroup = FactionMacro_Group;
                }
                else if (!string.IsNullOrEmpty(BindingMacro_Group.Value))
                {
                    macro.type = MacroTypes.BindingMacro;
                    foundGroup = BindingMacro_Group;
                }
                else if (!string.IsNullOrEmpty(DetailsMacro_Group.Value))
                {
                    macro.type = MacroTypes.DetailsMacro;
                    foundGroup = DetailsMacro_Group;
                }
                else if (!string.IsNullOrEmpty(ContextMacro_Group.Value))
                {
                    macro.type = MacroTypes.ContextMacro;
                    foundGroup = ContextMacro_Group;
                }

                // Set macro data if found
                if (foundGroup != null)
                {
                    Group prefix = match.Groups["prefix"];
                    Group suffix = match.Groups["suffix"];

                    macro.symbol = foundGroup.Value;
                    macro.index = prefix.Index;

                    // Length is from first character to end of suffix (if present)
                    // This will exclude any other characters (like fullstop) adjacent to macro token in word
                    if (macro.type != MacroTypes.ContextMacro)
                        macro.length = suffix.Index + suffix.Length - macro.index;
                    else
                        macro.length = prefix.Length + macro.symbol.Length;

                    // Get substring of macro token alone
                    // This is used for replace later
                    macro.token = word.Substring(macro.index, macro.length);
                }
            }

            return macro;
        }

        #endregion
    }
}