using System;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using System.IO;
using System.Collections.Generic;

namespace DaggerfallConnect.Arena2
{
    public class BiogFile
    {
        const int questionCount = 12;

        string questionsStr = string.Empty;
        Question[] questions = new Question[questionCount];

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
        }

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
    }
}

