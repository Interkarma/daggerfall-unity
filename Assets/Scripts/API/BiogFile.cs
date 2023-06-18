// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:
//
// Notes:
//

using System;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using System.IO;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Entity;
using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Utility;

namespace DaggerfallConnect.Arena2
{
    public partial class BiogFile
    {
        const int questionCount = 12;
        const int socialGroupCount = 5;
        const int defaultBackstoriesStart = 4116;

        // Folder names constants
        const string biogSourceFolderName = "BIOGs";

        string questionsStr = string.Empty;
        Question[] questions = new Question[questionCount];
        short[] changedReputations = new short[socialGroupCount];
        List<string> answerEffects = new List<string>();
        CharacterDocument characterDocument;
        int backstoryId;

        public static string BIOGSourceFolder
        {
            get { return Path.Combine(Application.streamingAssetsPath, biogSourceFolderName); }
        }

        public BiogFile(CharacterDocument characterDocument)
        {
            // Store reference to character document
            this.characterDocument = characterDocument;

            // Load text file
            string fileName = $"BIOG{characterDocument.classIndex:D2}T{characterDocument.biographyIndex}.TXT";
            FileProxy txtFile = new FileProxy(Path.Combine(BiogFile.BIOGSourceFolder, fileName), FileUsage.UseDisk, true);
            questionsStr = System.Text.Encoding.UTF8.GetString(txtFile.GetBytes());

            // Parse text into questions
            StringReader reader = new StringReader(questionsStr);
            string curLine = reader.ReadLine();
            for (int i = 0; i < questionCount; i++)
            {
                questions[i] = new Question();

                // Skip through any blank lines
                while (curLine.Length <= 1)
                    curLine = reader.ReadLine();

                // If we haven't parsed the first question yet, allow users to specify a custom backstory string id
                if(i == 0)
                {
                    if (curLine[0] == '#')
                    {
                        string value = curLine.Substring(1);
                        if (!int.TryParse(value, out backstoryId))
                        {
                            Debug.LogError($"{fileName}: Invalid string id '{value}'");
                            backstoryId = defaultBackstoriesStart + characterDocument.classIndex;
                        }

                        // Find the next non-empty line, which should be question 1
                        do
                        {
                            curLine = reader.ReadLine();
                        } while (curLine.Length <= 1);
                    }
                    else
                    {
                        backstoryId = defaultBackstoriesStart + characterDocument.classIndex;
                    }
                }

                // Parse question text
                for (int j = 0; j < Question.lines; j++)
                {
                    // Check if the next line is part of the question
                    if (j == 0) // first question line should lead with a number followed by a '.'
                    {
                        questions[i].Text[j] = curLine.Split(new[] { '.' }, 2)[1].Trim();
                    }
                    else if (j > 0 && curLine.IndexOf(".") != 1 && curLine.IndexOf(".") != 2)
                    {
                        questions[i].Text[j] = curLine.Trim();
                    }
                    else
                    {
                        break;
                    }
                    curLine = reader.ReadLine();
                }
                // Parse answers to the current question
                while (curLine.IndexOf(".") == 1 && char.IsLetter(curLine[0])) // Line without 2-char preamble including letter = end of answers
                {
                    Answer ans = new Answer();
                    // Get Answer text
                    ans.Text = curLine.Split('.')[1].Trim();
                    curLine = reader.ReadLine();

                    // Add answer effects
                    while (curLine.IndexOf(".") != 1 && curLine.Length > 1)
                    {
                        ans.Effects.Add(curLine.Trim());
                        curLine = reader.ReadLine();
                    }
                    questions[i].Answers.Add(ans);
                }
            }
            reader.Close();

            // Initialize reputation changes
            for (int i = 0; i < changedReputations.Length; i++)
            {
                changedReputations[i] = 0;
            }

            // Initialize question token lists
            Q1Tokens = new List<int>();
            Q2Tokens = new List<int>();
            Q3Tokens = new List<int>();
            Q4Tokens = new List<int>();
            Q5Tokens = new List<int>();
            Q6Tokens = new List<int>();
            Q7Tokens = new List<int>();
            Q8Tokens = new List<int>();
            Q9Tokens = new List<int>();
            Q10Tokens = new List<int>();
            Q11Tokens = new List<int>();
            Q12Tokens = new List<int>();
        }

        public void DigestRepChanges()
        {
            foreach (string effect in answerEffects)
            {
                int amount, id;
                string[] tokens = effect.Split(' ');

                if (effect[0] != 'r'
                    || effect[1] == 'f'
                    || tokens.Length < 2
                    || !int.TryParse(tokens[0].Split('r')[1], out id)
                    || !int.TryParse(tokens[1], out amount))
                {
                    continue;
                }
                changedReputations[id] += (short)amount;
            }
        }

        public List<string> GenerateBackstory()
        {
            #region Parse answer tokens
            List<int>[] tokenLists = new List<int>[questionCount * 2];
            tokenLists[0] = Q1Tokens;
            tokenLists[1] = Q2Tokens;
            tokenLists[2] = Q3Tokens;
            tokenLists[3] = Q4Tokens;
            tokenLists[4] = Q5Tokens;
            tokenLists[5] = Q6Tokens;
            tokenLists[6] = Q7Tokens;
            tokenLists[7] = Q8Tokens;
            tokenLists[8] = Q9Tokens;
            tokenLists[9] = Q10Tokens;
            tokenLists[10] = Q11Tokens;
            tokenLists[11] = Q12Tokens;

            // Setup tokens for macro handler
            foreach (string effect in answerEffects)
            {
                char prefix = effect[0];

                if (prefix == '#' || prefix == '!' || prefix == '?')
                {
                    int questionInd;
                    string[] effectSplit = effect.Split(' ');
                    string command = effectSplit[0];
                    string index = effectSplit[1];
                    if (!int.TryParse(index, out questionInd))
                    {
                        Debug.LogError("GenerateBackstory: Invalid question index.");
                        continue;
                    }

                    string[] splitStr = command.Split(prefix);
                    if (splitStr.Length > 1)
                    {
                        tokenLists[questionInd].Add(int.Parse(splitStr[1]));
                    }
                }
            }
            #endregion

            TextFile.Token lastToken = new TextFile.Token();
            GameManager.Instance.PlayerEntity.BirthRaceTemplate = characterDocument.raceTemplate; // Need correct race set when parsing %ra macro
            List<string> backStory = new List<string>();
            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(backstoryId);
            MacroHelper.ExpandMacros(ref tokens, (IMacroContextProvider)this);
            foreach (TextFile.Token token in tokens)
            {
                if (token.formatting == TextFile.Formatting.Text)
                {
                    backStory.Add(token.text);
                }
                else if (token.formatting == TextFile.Formatting.JustifyLeft)
                {
                    if (lastToken.formatting == TextFile.Formatting.JustifyLeft)
                        backStory.Add(string.Empty);
                }
                lastToken = token;
            }

            return backStory;
        }

        public void AddEffect(string effect, int index)
        {
            if (effect[0] == '#' || effect[0] == '!' || effect[0] == '?')
            {
                AnswerEffects.Add(effect + " " + index); // Tag text macros with question numbers
            }
            else
            {
                AnswerEffects.Add(effect);
            }
        }

        #region Static Methods

        private static void ApplyPlayerEffect(PlayerEntity playerEntity, string effect)
        {
            string[] tokens = effect.Split(null);
            int parseResult;

            // Skill modifier effect
            if (int.TryParse(tokens[0], out parseResult))
            {
                short modValue;
                DFCareer.Skills skill = (DFCareer.Skills)parseResult;
                if (short.TryParse(tokens[1], out modValue))
                {
                    short startValue = playerEntity.Skills.GetPermanentSkillValue(skill);
                    playerEntity.Skills.SetPermanentSkillValue(skill, (short)(startValue + modValue));
                }
                else
                {
                    Debug.LogError("CreateCharBiography: Invalid skill adjustment value.");
                }
            }
            // Modify gold amount
            else if (effect.StartsWith("GP"))
            {
                // Correct GP commands with spaces between the sign and the amount
                if (tokens.Length > 2)
                {
                    tokens[1] = tokens[1] + tokens[2];
                }
                if (!int.TryParse(tokens[1], out parseResult))
                {
                    Debug.LogError("CreateCharBiography: GP - invalid argument.");
                    return;
                }
                if (tokens[1][0] == '+')
                {
                    playerEntity.GoldPieces += parseResult;
                }
                else if (tokens[1][0] == '-')
                {
                    playerEntity.GoldPieces -= parseResult;
                    // The player can't carry negative gold pieces
                    playerEntity.GoldPieces = playerEntity.GoldPieces < 0 ? 0 : playerEntity.GoldPieces;
                }
            }
            // Add item
            else if (effect.StartsWith("IT"))
            {
                int itemGroup;
                int groupIndex;
                int material;
                if (!int.TryParse(tokens[1], out itemGroup)
                    || !int.TryParse(tokens[2], out groupIndex)
                    || !int.TryParse(tokens[3], out material))
                {
                    Debug.LogError("CreateCharBiography: IT - invalid argument(s).");
                    return;
                }

                DaggerfallUnityItem newItem = null;
                if ((ItemGroups)itemGroup == ItemGroups.Weapons)
                {
                    newItem = ItemBuilder.CreateWeapon((Weapons)Enum.GetValues(typeof(Weapons)).GetValue(groupIndex), (WeaponMaterialTypes)material);
                }
                else if ((ItemGroups)itemGroup == ItemGroups.Armor)
                {
                    // Biography commands treat weapon and armor material types the same
                    newItem = ItemBuilder.CreateArmor(playerEntity.Gender, playerEntity.Race, (Armor)Enum.GetValues(typeof(Armor)).GetValue(groupIndex), WeaponToArmorMaterialType((WeaponMaterialTypes)material));
                }
                else if ((ItemGroups)itemGroup == ItemGroups.Books)
                {
                    newItem = ItemBuilder.CreateRandomBook();
                }
                else
                {
                    newItem = new DaggerfallUnityItem((ItemGroups)itemGroup, groupIndex);
                }
                playerEntity.Items.AddItem(newItem);
            }
            // Adjust reputation
            else if (effect.StartsWith("r"))
            {
                int id;
                int amount;
                // Faction
                if (effect[1] == 'f')
                {
                    if (!int.TryParse(tokens[0].Split('f')[1], out id) || !int.TryParse(tokens[1], out amount))
                    {
                        Debug.LogError("CreateCharBiography: rf - invalid argument.");
                        return;
                    }
                    playerEntity.FactionData.ChangeReputation(id, amount, true);
                }
                // Social group (Merchants, Commoners, etc.)
                else
                {
                    if (!int.TryParse(tokens[0].Split('r')[1], out id) || !int.TryParse(tokens[1], out amount))
                    {
                        Debug.LogError("CreateCharBiography: r - invalid argument.");
                        return;
                    }
                    playerEntity.SGroupReputations[id] += (short)amount;
                }
            }
            // Adjust poison resistance
            else if (effect.StartsWith("RP"))
            {
                if (int.TryParse(tokens[1], out parseResult))
                {
                    playerEntity.BiographyResistPoisonMod = parseResult;
                }
                else
                {
                    Debug.LogError("CreateCharBiography: RP - invalid argument.");
                }
            }
            // Adjust fatigue
            else if (effect.StartsWith("FT"))
            {
                if (int.TryParse(tokens[1], out parseResult))
                {
                    playerEntity.BiographyFatigueMod = parseResult;
                }
                else
                {
                    Debug.LogError("CreateCharBiography: FT - invalid argument.");
                }
            }
            // Adjust reaction roll
            else if (effect.StartsWith("RR"))
            {
                if (int.TryParse(tokens[1], out parseResult))
                {
                    playerEntity.BiographyReactionMod = parseResult;
                }
                else
                {
                    Debug.LogError("CreateCharBiography: RR - invalid argument.");
                }
            }
            // Adjust disease resistance
            else if (effect.StartsWith("RD"))
            {
                if (int.TryParse(tokens[1], out parseResult))
                {
                    playerEntity.BiographyResistDiseaseMod = parseResult;
                }
                else
                {
                    Debug.LogError("CreateCharBiography: RD - invalid argument.");
                }
            }
            // Adjust magic resistance
            else if (effect.StartsWith("MR"))
            {
                if (int.TryParse(tokens[1], out parseResult))
                {
                    playerEntity.BiographyResistMagicMod = parseResult;
                }
                else
                {
                    Debug.LogError("CreateCharBiography: MR - invalid argument.");
                }
            }
            // Adjust to-hit
            else if (effect.StartsWith("TH"))
            {
                if (int.TryParse(tokens[1], out parseResult))
                {
                    playerEntity.BiographyAvoidHitMod = parseResult;
                }
                else
                {
                    Debug.LogError("CreateCharBiography: TH - invalid argument.");
                }
            }
            else if (effect[0] == '#' || effect[0] == '!' || effect[0] == '?')
            {
                Debug.Log("CreateCharBiography: Detected biography text command.");
            }
            // Unimplemented commands
            else if (effect.StartsWith("AE"))
            {
                Debug.Log("CreateCharBiography: AE - command unimplemented.");
            }
            else if (effect.StartsWith("AF"))
            {
                Debug.Log("CreateCharBiography: AF - command unimplemented.");
            }
            else if (effect.StartsWith("AO"))
            {
                Debug.Log("CreateCharBiography: AO - command unimplemented.");
            }
            else
            {
                Debug.LogError("CreateCharBiography: Invalid command - " + effect);
            }
        }

        public static void ApplyEffects(IEnumerable<string> effects, PlayerEntity playerEntity)
        {
            if (effects == null)
                return;

            foreach (string effect in effects)
            {
                ApplyPlayerEffect(playerEntity, effect);
            }
        }

        public static int[] GetSkillEffects(IEnumerable<string> effects)
        {
            if (effects == null)
                return null;

            int skillCount = (int)DFCareer.Skills.Count;
            int[] skills = new int[skillCount];

            // Apply only skill effects
            foreach(string effect in effects)
            {
                string[] tokens = effect.Split(null);
                int parseResult;

                // Skill modifier effect
                if (int.TryParse(tokens[0], out parseResult) && parseResult >= 0 && parseResult < skillCount)
                {
                    short modValue;
                    if (short.TryParse(tokens[1], out modValue))
                    {
                        skills[parseResult] += modValue;
                    }
                    else
                    {
                        Debug.LogError("CreateCharBiography: Invalid skill adjustment value.");
                    }
                }
            }

            return skills;
        }

        private static ArmorMaterialTypes WeaponToArmorMaterialType(WeaponMaterialTypes materialType)
        {
            switch (materialType)
            {
                case WeaponMaterialTypes.Iron:
                    return ArmorMaterialTypes.Iron;
                case WeaponMaterialTypes.Steel:
                    return ArmorMaterialTypes.Steel;
                case WeaponMaterialTypes.Silver:
                    return ArmorMaterialTypes.Silver;
                case WeaponMaterialTypes.Elven:
                    return ArmorMaterialTypes.Elven;
                case WeaponMaterialTypes.Dwarven:
                    return ArmorMaterialTypes.Dwarven;
                case WeaponMaterialTypes.Mithril:
                    return ArmorMaterialTypes.Mithril;
                case WeaponMaterialTypes.Adamantium:
                    return ArmorMaterialTypes.Adamantium;
                case WeaponMaterialTypes.Ebony:
                    return ArmorMaterialTypes.Ebony;
                case WeaponMaterialTypes.Orcish:
                    return ArmorMaterialTypes.Orcish;
                case WeaponMaterialTypes.Daedric:
                    return ArmorMaterialTypes.Daedric;
                default:
                    return ArmorMaterialTypes.None;
            }
        }

        public static int GetClassAffinityIndex(DFCareer custom, List<DFCareer> classes)
        {
            int highestAffinity = 0;
            int selectedIndex = 0;
            for (int i = 0; i < classes.Count; i++)
            {
                int affinity = 0;
                List<DFCareer.Skills> classSkills = new List<DFCareer.Skills>();
                classSkills.Add(classes[i].PrimarySkill1);
                classSkills.Add(classes[i].PrimarySkill2);
                classSkills.Add(classes[i].PrimarySkill3);
                classSkills.Add(classes[i].MajorSkill1);
                classSkills.Add(classes[i].MajorSkill2);
                classSkills.Add(classes[i].MajorSkill3);
                classSkills.Add(classes[i].MinorSkill1);
                classSkills.Add(classes[i].MinorSkill2);
                classSkills.Add(classes[i].MinorSkill3);
                classSkills.Add(classes[i].MinorSkill4);
                classSkills.Add(classes[i].MinorSkill5);
                classSkills.Add(classes[i].MinorSkill6);
                if (classSkills.Contains(custom.PrimarySkill1)) affinity++;
                if (classSkills.Contains(custom.PrimarySkill2)) affinity++;
                if (classSkills.Contains(custom.PrimarySkill3)) affinity++;
                if (classSkills.Contains(custom.MajorSkill1)) affinity++;
                if (classSkills.Contains(custom.MajorSkill2)) affinity++;
                if (classSkills.Contains(custom.MajorSkill3)) affinity++;
                if (classSkills.Contains(custom.MinorSkill1)) affinity++;
                if (classSkills.Contains(custom.MinorSkill2)) affinity++;
                if (classSkills.Contains(custom.MinorSkill3)) affinity++;
                if (classSkills.Contains(custom.MinorSkill4)) affinity++;
                if (classSkills.Contains(custom.MinorSkill5)) affinity++;
                if (classSkills.Contains(custom.MinorSkill6)) affinity++;
                if (affinity > highestAffinity)
                {
                    highestAffinity = affinity;
                    selectedIndex = i;
                }
            }

            return selectedIndex;
        }

        #endregion

        #region Properties

        public Question[] Questions
        {
            get { return questions; }
        }

        public class Question
        {
            public const int lines = 2;
            //const int maxAnswers = 10;

            readonly string[] text = new string[lines];
            readonly List<Answer> answers = new List<Answer>();

            public Question()
            {
                for (int i = 0; i < lines; i++)
                {
                    text[i] = string.Empty;
                }
            }

            public string[] Text
            {
                get { return text; }
            }

            public List<Answer> Answers
            {
                get { return answers; }
            }
        }

        public class Answer
        {
            string text = string.Empty;
            readonly List<string> effects = new List<string>();

            public string Text
            {
                get { return text; }
                set { text = value; }
            }

            public List<String> Effects
            {
                get { return effects; }
            }
        }

        public List<string> AnswerEffects
        {
            get { return answerEffects; }
        }

        public List<int> Q1Tokens { get; set; }
        public List<int> Q2Tokens { get; set; }
        public List<int> Q3Tokens { get; set; }
        public List<int> Q4Tokens { get; set; }
        public List<int> Q5Tokens { get; set; }
        public List<int> Q6Tokens { get; set; }
        public List<int> Q7Tokens { get; set; }
        public List<int> Q8Tokens { get; set; }
        public List<int> Q9Tokens { get; set; }
        public List<int> Q10Tokens { get; set; }
        public List<int> Q11Tokens { get; set; }
        public List<int> Q12Tokens { get; set; }

        #endregion
    }
}
