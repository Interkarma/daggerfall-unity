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
        const int questionCount = 10;
        #region Data
        /*
         * This table determines the path that each answer corresponds to.
         * Each row has three columns - one for each answer (a, b, c).
         * Possible values are 00 (Warrior), 01 (Rogue), and 02 (Mage).
         * Ripped from FALL.EXE v1.07.213 at offset 0x59820C.
         */
        int[] answerTable = {   00, 02, 01,
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
        #endregion

        Texture2D nativeTexture;
        List<TextLabel> questionLabels;
        int labelOffset = 0;
        Dictionary<int, string> questionLibrary;
        List<int> questionIndices;
        int questionIndex = 0;
        int[] weights = new int[] { 0, 0, 0 }; // Number of answers that steer class toward warrior/rogue/mage paths
        Dictionary<int, int> resultToClassMappings;
        int classIndex = 0;

        public CreateCharClassQuestions(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

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
            /*
            FileProxy classFile = new FileProxy(Path.Combine(DaggerfallUnity.Instance.Arena2Path, classesFileName), FileUsage.UseDisk, true);
            byte[] classData = classFile.GetBytes();
            */

            GetQuestions();
            DisplayQuestion(questionIndices[0]);

            IsSetup = true;
        }

        private void GetQuestions()
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
            questionIndices = new List<int>();
            for (int i = 0; i < questionCount; i++)
            {
                int index = UnityEngine.Random.Range(0, questionLibrary.Count);
                while (pickedQuestions.ContainsKey(index)) // Ensure all picked questions are unique.
                {
                    index = (index + 1) % questionLibrary.Count;
                }

                pickedQuestions[index] = true;
                questionIndices.Add(index);
            }
        }

        private void DisplayQuestion(int questionIndex)
        {
            questionLabels = new List<TextLabel>();
            string[] lines = questionLibrary[questionIndex].Split("\r\n".ToCharArray()).Where(x => x != string.Empty).ToArray();
            for (int i = 0; i < lines.Length; i++)
            {
                questionLabels.Add(DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont,
                                                  new Vector2(questionLeft, labelOffset + questionTop + i * questionLineSpace),
                                                  lines[i],
                                                  NativePanel));
            }
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.A))
            {

            }
            else if (Input.GetKeyDown(KeyCode.B))
            {

            }
            else if (Input.GetKeyDown(KeyCode.C))
            {

            }
        }

        public int ClassIndex
        {
            get { return classIndex; }
        }
    }
}
