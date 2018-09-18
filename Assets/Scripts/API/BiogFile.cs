using System;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using System.IO;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Entity;
using UnityEngine;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallConnect.Arena2
{
    public partial class BiogFile
    {
        const int questionCount = 12;
        const int socialGroupCount = 5;

        string questionsStr = string.Empty;
        Question[] questions = new Question[questionCount];
        short[] changedReputations = new short[socialGroupCount];
        List<string> answerEffects = new List<string>();

        public BiogFile(int classIndex)
        {
            // Load text file
            string fileName = "BIOG" + classIndex.ToString("D" + 2) + "T0.TXT";
            FileProxy txtFile = new FileProxy(Path.Combine(DaggerfallUnity.Instance.Arena2Path, fileName), FileUsage.UseDisk, true);
            questionsStr = System.Text.Encoding.UTF8.GetString(txtFile.GetBytes());

            // Parse text into questions
            StringReader reader = new StringReader(questionsStr);
            for (int i = 0; i < questionCount; i++)
            {
                questions[i] = new Question();

                string curLine = reader.ReadLine();
                // Parse question text
                for (int j = 0; j < Question.lines; j++)
                {
                    // Check if the next line is part of the question
                    if (j == 0) // first question line should lead with a number followed by a '.'
                    {
                        questions[i].Text[j] = curLine.Split(new[] { '.' }, 2)[1].Trim();
                    } 
                    else if (j > 0 && curLine.IndexOf (".") != 1 && curLine.IndexOf (".") != 2)
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
                while (curLine.Length > 1) // Line without 2-char preamble = Empty line = end of answers
                {
                    Answer ans = new Answer();
                    // Get Answer text
                    if (curLine.IndexOf(".") == 1)
                    {
                        ans.Text = curLine.Split('.')[1].Trim();
                        curLine = reader.ReadLine();
                    }
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

        #region Static Methods

        private static void ApplyPlayerEffect(PlayerEntity playerEntity, string effect)
        {
            string[] tokens = effect.Split(' ');
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
                int templateIndex;
                int material;
                if (!int.TryParse(tokens[1], out itemGroup)
                    || !int.TryParse(tokens[2], out templateIndex)
                    || !int.TryParse(tokens[3], out material))
                {
                    Debug.LogError("CreateCharBiography: IT - invalid argument(s).");
                    return;
                }
                    
                DaggerfallUnityItem newItem;
                if ((ItemGroups)itemGroup == ItemGroups.Weapons)
                {
                    newItem = ItemBuilder.CreateWeapon((Weapons)Enum.GetValues(typeof(Weapons)).GetValue(templateIndex), (WeaponMaterialTypes)material);
                }
                else if ((ItemGroups)itemGroup == ItemGroups.Armor)
                {
                    // Biography commands treat weapon and armor material types the same
                    newItem = ItemBuilder.CreateArmor(playerEntity.Gender, playerEntity.Race, (Armor)Enum.GetValues(typeof(Armor)).GetValue(templateIndex), WeaponToArmorMaterialType((WeaponMaterialTypes)material));
                }
                else if ((ItemGroups)itemGroup == ItemGroups.Books)
                {
                    newItem = ItemBuilder.CreateRandomBook();
                }
                else
                {
                    newItem = ItemBuilder.CreateItem((ItemGroups)itemGroup, templateIndex);
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
                    // TODO: Not 100% sure if this should propagate or not
                    playerEntity.FactionData.ChangeReputation(id, amount);
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
                if (!int.TryParse(tokens[1], out parseResult))
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
                if (!int.TryParse(tokens[1], out parseResult))
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
                if (!int.TryParse(tokens[1], out parseResult))
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
                if (!int.TryParse(tokens[1], out parseResult))
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
                if (!int.TryParse(tokens[1], out parseResult))
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
                if (!int.TryParse(tokens[1], out parseResult))
                {
                    playerEntity.BiographyAvoidHitMod = parseResult;
                }
                else
                {
                    Debug.LogError("CreateCharBiography: TH - invalid argument.");
                }
            }
            else if (effect[0] == '#' || effect[0] == '!')
            {
                // TODO: Implement biography history text commands
                Debug.Log("CreateCharBiography: Biography history text commands not yet implemented.");
            }
            // Unknown commands
            else if (effect.StartsWith("AE"))
            {
                Debug.Log("CreateCharBiography: AE - command function unknown.");
            }
            else if (effect.StartsWith("AF"))
            {
                Debug.Log("CreateCharBiography: AF - command function unknown.");
            }
            else
            {
                Debug.LogError("CreateCharBiography: Invalid command - " + effect);
            }
        }

        public static void ApplyEffects(List<string> effects, PlayerEntity playerEntity)
        {
            foreach (string effect in effects)
            {
                ApplyPlayerEffect(playerEntity, effect);
            }
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
            const int maxAnswers = 10;

            string[] text = new string[lines];
            List<Answer> answers = new List<Answer>();

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
            List<string> effects = new List<string>();

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

        #endregion
    }
}

