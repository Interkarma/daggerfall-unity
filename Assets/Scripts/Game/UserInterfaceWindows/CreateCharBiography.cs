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

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements biography questionnaire
    /// </summary>
    public class CreateCharBiography : DaggerfallPopupWindow
    {
        const string nativeImgName = "BIOG00I0.IMG";
        const int questionLines = 2;
        const int questionLineSpace = 11;
        const int questionLeft = 30;
        const int questionTop = 23;
        const int questionWidth = 156;
        const int questionHeight = 45;
        const int buttonCount = 10;
        const int buttonsLeft = 10;
        const int buttonsTop = 71;
        const int buttonWidth = 149;
        const int buttonHeight = 24;
        public const int reputationToken = 35;

        int questionIndex = 0;
        Texture2D nativeTexture;
        TextLabel[] questionLabels = new TextLabel[questionLines];
        Button[] answerButtons = new Button[buttonCount];
        TextLabel[] answerLabels = new TextLabel[buttonCount];
        BiogFile biogFile;

        public CreateCharBiography(IUserInterfaceManager uiManager, CharacterDocument document)
            : base(uiManager)
        {
            Document = document;
        }

        protected override void Setup()
        {
            if (IsSetup)
                return;
            
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("CreateCharBiography: Could not load native texture.");

            // Load question data
            biogFile = new BiogFile(Document);

            // Set background
            NativePanel.BackgroundTexture = nativeTexture;

            // Set question text
            questionLabels[0] = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont,
                                                          new Vector2(questionLeft, questionTop),
                                                          string.Empty,
                                                          NativePanel);
            questionLabels[1] = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont,
                                                          new Vector2(questionLeft, questionTop + questionLineSpace),
                                                          string.Empty,
                                                          NativePanel);
            // Setup buttons
            for (int i = 0; i < buttonCount; i++)
            {
                int left = i % 2 == 0 ? buttonsLeft : buttonsLeft + buttonWidth;

                answerButtons[i] = DaggerfallUI.AddButton(new Rect((float)left,
                                                                   (float)(buttonsTop + (i / 2) * buttonHeight),
                                                                   (float)buttonWidth,
                                                                   (float)buttonHeight), NativePanel);
                answerButtons[i].Tag = i;
                answerButtons[i].OnMouseClick += AnswerButton_OnMouseClick;
                answerLabels[i] = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont,
                                                            new Vector2(21f, 5f),
                                                            string.Empty,
                                                            answerButtons[i]);
            }

            PopulateControls(biogFile.Questions[questionIndex]);

            IsSetup = true;
        }

        private void PopulateControls(BiogFile.Question question)
        {
            questionLabels[0].Text = question.Text[0];
            questionLabels[1].Text = question.Text[1];
            for (int i = 0; i < question.Answers.Count; i++)
            {
                answerLabels[i].Text = question.Answers[i].Text;
            }
            // blank out remaining labels
            for (int i = question.Answers.Count; i < buttonCount; i++)
            {
                answerLabels[i].Text = string.Empty;
            }
        }

        void AnswerButton_OnMouseClick(BaseScreenComponent sender, Vector2 pos)
        {
            int answerIndex = (int)sender.Tag;
            List<BiogFile.Answer> curAnswers = biogFile.Questions[questionIndex].Answers;

            if (answerIndex >= curAnswers.Count)
            {
                return; // not an answer for this question
            }
            else if (questionIndex < biogFile.Questions.Length - 1)
            {
                foreach (string effect in curAnswers[answerIndex].Effects)
                {
                    biogFile.AddEffect(effect, questionIndex);
                }
                questionIndex++;
                PopulateControls(biogFile.Questions[questionIndex]);
            }
            else
            {
                // Add final effects
                foreach (string effect in curAnswers[answerIndex].Effects)
                {
                    biogFile.AddEffect(effect, questionIndex);
                }

                // Create text biography
                BackStory = biogFile.GenerateBackstory();

                // Show reputation changes
                biogFile.DigestRepChanges();
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(reputationToken, biogFile);
                messageBox.ClickAnywhereToClose = true;
                messageBox.OnClose += MessageBox_OnClose;
                messageBox.Show();
            }
        }

        void MessageBox_OnClose()
        {
            CloseWindow();
        }

        public override void Update()
        {
            base.Update();
        }

        public CharacterDocument Document { get; set; }

        public int ClassIndex
        {
            set { Document.classIndex = value; }
            get { return Document.classIndex; }
        }

        public List<string> PlayerEffects
        {
            get { return biogFile.AnswerEffects; }
        }

        public List<string> BackStory { get; set; }
    }
}