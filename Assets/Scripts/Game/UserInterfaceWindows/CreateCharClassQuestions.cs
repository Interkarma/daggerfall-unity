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
using DaggerfallWorkshop.Game.UserInterface;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements class selection questionnaire
    /// </summary>
    public class CreateCharClassQuestions : DaggerfallPopupWindow
    {
        const string nativeImgName = "CHGN00I0.IMG";
        const string classesFileName = "CLASSES.DAT";
        const string scroll0FileName = "SCRL00I0.GFX";
        const string scroll1FileName = "SCRL01I0.GFX";
        const int questionLines = 2;
        const int questionLineSpace = 9;
        const float questionLeft = 20f;
        const float questionTop = 135f;
        const int questionWidth = 156;
        const int questionHeight = 45;
        const int classQuestionsToken = 9000;
        const int classDescriptionsTokenBase = 2100;
        const int questionCount = 10;
        public const byte noClassIndex = 255;
        const float leftTextOffset = 20f;
        const float topTextOffset = 16f;

        Texture2D nativeTexture;
        GfxFile scrollFile0;
        GfxFile scrollFile1;
        List<Texture2D> scrollTextures;
        MultiFormatTextLabel questionLabel = new MultiFormatTextLabel();
        Dictionary<int, string> questionLibrary;
        List<int> questionIndices;
        byte classIndex = 0;
        byte[] weights = new byte[] { 0, 0, 0 }; // Number of answers that steer class toward mage/rogue/warrior paths
        int questionsAnswered = 0;
        Panel questionScroll = new Panel();
        int scrollFrame = 0;
        bool isScrolling = false;
        bool scrollingDown = false;
        int aIndex = 0;
        int bIndex = 0;
        int cIndex = 0;

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

            // Load both scroll images as one contiguous list of textures
            ImgFile img = new ImgFile(Path.Combine(DaggerfallUnity.Arena2Path, nativeImgName), FileUsage.UseMemory, true);
            DFBitmap backgroundBitmap = img.GetDFBitmap(0, 0);
            DFPalette basePalette = backgroundBitmap.Palette;
            scrollFile0 = new GfxFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, scroll0FileName), FileUsage.UseMemory, true);
            scrollFile1 = new GfxFile(Path.Combine(DaggerfallUnity.Instance.Arena2Path, scroll1FileName), FileUsage.UseMemory, true);
            scrollFile0.Palette = basePalette;
            scrollFile1.Palette = basePalette;
            scrollTextures = new List<Texture2D>();
            for (int i = 0; i < scrollFile0.frames.Length; i++)
            {
                scrollTextures.Add(TextureReader.CreateFromAPIImage(scrollFile0, 0, i, 0));
                scrollTextures.Last().filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            }
            for (int i = scrollFile0.frames.Length; i < scrollFile0.frames.Length + scrollFile1.frames.Length; i++)
            {
                scrollTextures.Add(TextureReader.CreateFromAPIImage(scrollFile1, 0, i - scrollFile0.frames.Length, 0));
                scrollTextures.Last().filterMode = DaggerfallUI.Instance.GlobalFilterMode;
            }

            // Position scroll image on screen
            questionScroll.Position = new Vector2(0, 119f);
            questionScroll.Size = new Vector2(scrollTextures[0].width, scrollTextures[0].height);
            questionScroll.BackgroundTexture = scrollTextures[0];
            questionScroll.Parent = NativePanel;
            NativePanel.Components.Add(questionScroll);

            // Setup question label
            questionIndices = GetQuestions();
            DisplayQuestion(questionIndices[questionsAnswered]);

            // Handle scrolling
            NativePanel.OnMouseScrollDown += NativePanel_OnMouseScrollDown;
            NativePanel.OnMouseScrollUp += NativePanel_OnMouseScrollUp;
            questionScroll.OnMouseDown += QuestionScroll_OnMouseDown;
            questionScroll.OnMouseUp += QuestionScroll_OnMouseUp;

            IsSetup = true;
        }

        public override void Update()
        {
            base.Update();

            // User picked an answer with a key
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

            // User is scrolling with a mouseclick
            if (isScrolling)
            {
                if (scrollingDown && questionLabel.Position.y + questionLabel.Size.y > questionScroll.Size.y - topTextOffset)
                {
                    scrollFrame = (scrollFrame + 1) % scrollTextures.Count;
                    questionScroll.BackgroundTexture = scrollTextures[scrollFrame];
                    questionLabel.Position = new Vector2(questionLabel.Position.x, questionLabel.Position.y - 1f);
                }
                else if (!scrollingDown && questionLabel.Position.y < topTextOffset)
                {
                    scrollFrame = scrollFrame - 1 < 0 ? scrollTextures.Count - 1 : scrollFrame - 1;
                    questionScroll.BackgroundTexture = scrollTextures[scrollFrame];
                    questionLabel.Position = new Vector2(questionLabel.Position.x, questionLabel.Position.y + 1f);
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
        }
        #endregion Unity

        #region Helper Methods
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
                Position = new Vector2(leftTextOffset, topTextOffset),
                Size = new Vector2(320f, 0f), // make sure it has enough space - allow it to run off the screen
                TextColor = Color.black,
                ShadowPosition = new Vector2(0f, 0f)
            };
            string[] lines = questionLibrary[questionIndex].Split("\r\n".ToCharArray()).Where(x => x != string.Empty).ToArray();
            List<TextFile.Token> tokens = new List<TextFile.Token>();
            foreach (string line in lines)
            {
                tokens.Add(TextFile.CreateTextToken(line));
                tokens.Add(TextFile.CreateFormatToken(TextFile.Formatting.NewLine));
            }
            questionLabel.RectRestrictedRenderArea = new Rect(
                new Vector2(leftTextOffset, topTextOffset),
                new Vector2(questionScroll.Size.x, questionScroll.Size.y - topTextOffset * 2f)
                );
            questionLabel.SetText(tokens.ToArray());
            questionScroll.Components.Add(questionLabel);
            scrollFrame = 0;
            questionScroll.BackgroundTexture = scrollTextures[0];
            for (int i = 0; i < questionLabel.TextLabels.Count; i++)
            {
                TextLabel label = questionLabel.TextLabels[i];

                if (label.Text.Contains("a)"))
                    aIndex = i;
                else if (label.Text.Contains("b)"))
                    bIndex = i;
                else if (label.Text.Contains("c)"))
                    cIndex = i;
            }
        }

        /// <summary>
        /// Packs weight values into a single uint
        /// </summary>
        private uint WeightsToUint(byte w1, byte w2, byte w3)
        {
            return (uint)(w1 | (w2 << 8) | (w3 << 16));
        }

        /// <summary>
        /// Gets the index of the weights array that a given answer should increment
        /// </summary>
        /// <param name="chosenIndex">The index of the question table</param>
        /// <param name="column">The answer given to the chosen question index</param>
        private int GetWeightIndex(int chosenIndex, int column)
        {
            return answerTable[questionIndices[chosenIndex] * 3 + column];
        }

        private void AnswerAndContinue(int choice)
        {
            weights[GetWeightIndex(questionsAnswered, choice)]++;
            Debug.Log("CreateCharClassQuestions: Warrior: " + weights[0] + " Rogue: " + weights[1] + " Mage: " + weights[2]);
            if (questionsAnswered == questionCount - 1) // Final question was answered
            {
                // Blank out the background
                questionLabel.Clear();
                NativePanel.BackgroundTexture = null;
                questionScroll.BackgroundTexture = null;

                // Compute class index
                FileProxy classFile = new FileProxy(Path.Combine(DaggerfallUnity.Instance.Arena2Path, classesFileName), FileUsage.UseDisk, true);
                if (classFile == null)
                {
                    throw new Exception("CreateCharClassQuestions: Could not load CLASSES.DAT.");
                }
                byte[] classData = classFile.GetBytes();
                int headerIndex = GetHeaderIndex(classData);
                if (headerIndex == -1)
                {
                    throw new Exception("CreateCharClassQuestions: Error reading CLASSES.DAT - could not find a results match. Warrior: " + weights[0] + " Rogue: " + weights[1] + " Mage: " + weights[2]);
                }
                classIndex = GetClassIndex(headerIndex, classData);

                // Prompt user to confirm class
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

        private int GetHeaderIndex(byte[] classData)
        {
            byte resultsOffset = 18; // starting offset of results table in CLASSES.DAT
            for (byte headerOffset = 0; headerOffset < 48; headerOffset++)
            {
                // Check for results match
                if (WeightsToUint(classData[resultsOffset], classData[resultsOffset + 1], classData[resultsOffset + 2])
                    == WeightsToUint(weights[0], weights[1], weights[2]))
                {
                    return headerOffset / 4;
                }
                resultsOffset += 3;
            }

            for (byte headerOffset = 0; headerOffset < 18; headerOffset++)
            {
                // Check for results match
                if (WeightsToUint(classData[resultsOffset], classData[resultsOffset + 1], classData[resultsOffset + 2])
                    == WeightsToUint(weights[0], weights[1], weights[2]))
                {
                    return headerOffset / 4 + 12;
                }
                resultsOffset += 3;
            }

            return -1; // Should never reach this point
        }

        private byte GetClassIndex(int index, byte[] data)
        {
            // Array used by FALL.EXE is the same as in CLASSES.DAT except bytes 0x04 through 0x11 have their left nibbles zeroed out
            byte r = data[index];
            if (index > 3)
            {
                r &= 0x0F;
            }
            return r;
        }
        #endregion Helper Methods

        #region Properties
        public byte ClassIndex
        {
            get { return classIndex; }
        }
        #endregion Properties

        #region Event Handlers
        private void ConfirmDialog_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.No)
            {
                classIndex = noClassIndex;
            }
            sender.CloseWindow();
            CloseWindow();
        }

        private void NativePanel_OnMouseScrollDown(BaseScreenComponent sender)
        {
            if (questionLabel.Position.y + questionLabel.Size.y > questionScroll.Size.y - topTextOffset)
            {
                scrollFrame = (scrollFrame + 1) % scrollTextures.Count;
                questionScroll.BackgroundTexture = scrollTextures[scrollFrame];
                questionLabel.Position = new Vector2(questionLabel.Position.x, questionLabel.Position.y - 1f);
            }
        }

        private void NativePanel_OnMouseScrollUp(BaseScreenComponent sender)
        {
            if (questionLabel.Position.y < topTextOffset)
            {
                scrollFrame = scrollFrame - 1 < 0 ? scrollTextures.Count - 1 : scrollFrame - 1;
                questionScroll.BackgroundTexture = scrollTextures[scrollFrame];
                questionLabel.Position = new Vector2(questionLabel.Position.x, questionLabel.Position.y + 1f);
            }
        }

        private void QuestionScroll_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            // Handle scrolling by clicking
            if (position.y < topTextOffset)
            {
                isScrolling = true;
                scrollingDown = false;
                return;
            }
            else if (position.y > questionScroll.Size.y - topTextOffset)
            {
                isScrolling = true;
                scrollingDown = true;
                return;
            }

            // Handle clicking on answers
            int labelIndex = 0;
            for (int i = 0; i < questionLabel.TextLabels.Count; i++)
            {
                TextLabel label = questionLabel.TextLabels[i];

                // Determine if label was clicked
                if (position.y > questionLabel.Position.y + label.Position.y &&
                    position.y < questionLabel.Position.y + label.Position.y + label.Size.y)
                {
                    labelIndex = i;
                }
            }

            // Determine which answer was picked
            if (labelIndex >= aIndex && labelIndex < bIndex)
                AnswerAndContinue(0);
            else if (labelIndex >= bIndex && labelIndex < cIndex)
                AnswerAndContinue(1);
            else if (labelIndex >= cIndex)
                AnswerAndContinue(2);
        }

        private void QuestionScroll_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            isScrolling = false;
        }
        #endregion Event Handlers

        #region Data
        /*
         * This table determines the path that each answer corresponds to.
         * Each row has three columns - one for each answer (a, b, c).
         * Possible values are 00 (Warrior), 01 (Rogue), and 02 (Mage).
         * Ripped from FALL.EXE v1.07.213 at offset 0x0059820C.
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
