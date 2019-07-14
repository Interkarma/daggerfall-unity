// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Player;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using DaggerfallConnect.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements class selection questionnaire
    /// </summary>
    public class CreateCharClassQuestions : DaggerfallPopupWindow
    {
        const string nativeImgName = "CHGN00I0.IMG";
        const string classesFileName = "CLASSES.DAT";
        const int questionLines = 2;
        const int questionLineSpace = 9;
        const int questionLeft = 25;
        const int questionTop = 135;
        const int questionWidth = 156;
        const int questionHeight = 45;
        const int classQuestionsToken = 9000;
        const int classDescriptionsTokenBase = 2100;
        const int questionCount = 10;

        Texture2D nativeTexture;
        List<TextLabel> questionLabels;
        MultiFormatTextLabel questionLabel = new MultiFormatTextLabel();
        int labelOffset = 0;
        Dictionary<int, string> questionLibrary;
        List<int> questionIndices;
        Dictionary<uint, int> resultToClassMappings;
        int classIndex = -1;
        byte[] weights = new byte[] { 0, 0, 0 }; // Number of answers that steer class toward mage/rogue/warrior paths
        int questionsAnswered = 0;

        public CreateCharClassQuestions(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        #region Unity
        protected override void Setup()
        {
            if (IsSetup)
                return;
            
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("CreateCharClassQuestions: Could not load native texture.");

            // Set background
            NativePanel.BackgroundTexture = nativeTexture;

            // Map results to class indices
            FileProxy classFile = new FileProxy(Path.Combine(DaggerfallUnity.Instance.Arena2Path, classesFileName), FileUsage.UseDisk, true);
            if (classFile == null)
                throw new Exception("CreateCharClassQuestions: Could not load CLASSES.DAT.");
            byte[] classData = classFile.GetBytes();
            resultToClassMappings = GetClassMappings(classData);

            questionIndices = GetQuestions();
            DisplayQuestion(questionIndices[questionsAnswered]);

            IsSetup = true;
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.A))
            {
                AnswerAndContinue(0);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                AnswerAndContinue(1);
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                AnswerAndContinue(2);
            }
        }
        #endregion Unity

        #region Helper Methods
        private Dictionary<uint, int> GetClassMappings(byte[] data)
        {
            Dictionary<uint, int> result = new Dictionary<uint, int>();
            // TODO: Determine how classic matches results to a class
            return result;
        }

        private List<int> GetQuestions()
        {
            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(classQuestionsToken);
            StringBuilder question = new StringBuilder();
            int questionInd = 0;
            questionLibrary = new Dictionary<int, string>();
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].text != null)
                {
                    string[] tokenText = Regex.Split(tokens[i].text, @"{\d+.");
                    if (tokenText.Length > 1) // If true the line contains the expression, which means it is the start of a question.
                    {
                        if (question.Length != 0)
                        {
                            // Finished parsing question - add to library and continue
                            questionLibrary.Add(questionInd++, question.ToString());
                            question = new StringBuilder();
                        }
                        question.AppendLine(tokenText[1]);
                    }
                    else
                    {
                        question.AppendLine(tokenText[0]);
                    }
                }
            }
            questionLibrary.Add(questionInd, question.ToString()); // add the final question

            Dictionary<int, bool> pickedQuestions = new Dictionary<int, bool>();
            List<int> indices = new List<int>();
            for (int i = 0; i < questionCount; i++)
            {
                int index = UnityEngine.Random.Range(0, questionLibrary.Count);
                while (pickedQuestions.ContainsKey(index)) // Ensure all picked questions are unique.
                {
                    index = (index + 1) % questionLibrary.Count;
                }

                pickedQuestions[index] = true;
                indices.Add(index);
            }

            return indices;
        }

        private void DisplayQuestion(int questionIndex)
        {
            questionLabel.Clear();
            questionLabel = new MultiFormatTextLabel
            {
                Position = new Vector2(questionLeft, questionTop),
                Size = new Vector2(320, 240) // make sure it has enough space - allow it to run off the screen
            };
            string[] lines = questionLibrary[questionIndex].Split("\r\n".ToCharArray()).Where(x => x != string.Empty).ToArray();
            List<TextFile.Token> tokens = new List<TextFile.Token>();
            foreach (string line in lines)
            {
                tokens.Add(TextFile.CreateTextToken(line));
                tokens.Add(TextFile.CreateFormatToken(TextFile.Formatting.NewLine));
            }
            questionLabel.SetText(tokens.ToArray());
            NativePanel.Components.Add(questionLabel);
        }

        private uint WeightsToUint(byte w1, byte w2, byte w3)
        {
            return (uint)(w1 | (w2 << 8) | (w3 << 16));
        }

        private int GetWeightIndex(int chosenIndex, int column)
        {
            return answerTable[questionIndices[chosenIndex] * 3 + column];
        }

        private void AnswerAndContinue(int choice)
        {
            weights[GetWeightIndex(questionsAnswered, choice)]++;
            if (questionsAnswered == questionCount - 1)
            {
                questionLabel.Clear();
                NativePanel.BackgroundTexture = null;
                //classIndex = resultToClassMappings[WeightsToUint(weights[0], weights[1], weights[2])];
                classIndex = 0;
                DaggerfallMessageBox confirmDialog = new DaggerfallMessageBox(uiManager,
                                                                              DaggerfallMessageBox.CommonMessageBoxButtons.YesNo,
                                                                              classDescriptionsTokenBase + classIndex,
                                                                              uiManager.TopWindow);
                confirmDialog.OnButtonClick += ConfirmDialog_OnButtonClick;
                uiManager.PushWindow(confirmDialog);
            }
            else
            {
                DisplayQuestion(questionIndices[++questionsAnswered]);
            }
        }
        #endregion Helper Methods

        public int ClassIndex
        {
            get { return classIndex; }
        }

        #region Event Handlers
        private void ConfirmDialog_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.No)
            {
                classIndex = -1;
            }
            sender.CloseWindow();
            CloseWindow();
        }
        #endregion Event Handlers

        #region Data
        /*
         * This table determines the path that each answer corresponds to.
         * Each row has three columns - one for each answer (a, b, c).
         * Possible values are 00 (Warrior), 01 (Rogue), and 02 (Mage).
         * Ripped from FALL.EXE v1.07.213 at offset 0x59820C.
         */
        readonly byte[] answerTable = {     00, 02, 01,
                                            00, 02, 01,
                                            00, 01, 02,
                                            02, 00, 01,
                                            00, 01, 02,
                                            01, 00, 02,
                                            00, 01, 02,
                                            02, 00, 01,
                                            00, 02, 01,
                                            00, 01, 02,
                                            00, 01, 02,
                                            00, 01, 02,
                                            00, 02, 01,
                                            00, 01, 02,
                                            01, 00, 02,
                                            01, 02, 00,
                                            02, 00, 01,
                                            01, 00, 02,
                                            00, 01, 02,
                                            02, 00, 01,
                                            01, 00, 02,
                                            00, 01, 02,
                                            00, 02, 01,
                                            02, 01, 00,
                                            01, 00, 02,
                                            00, 02, 01,
                                            02, 01, 00,
                                            02, 00, 01,
                                            02, 01, 00,
                                            02, 01, 00,
                                            02, 00, 01,
                                            01, 00, 02,
                                            00, 02, 01,
                                            02, 01, 00,
                                            01, 02, 00,
                                            02, 00, 01,
                                            02, 00, 01,
                                            01, 02, 00,
                                            00, 01, 02,
                                            01, 02, 00 };
        #endregion Data
    }
}
