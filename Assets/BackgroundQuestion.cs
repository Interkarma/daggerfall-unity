using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public class BackgroundQuestion
    {
        private int questionID;
        private string question;

        private Dictionary<char, string> answerTexts = new Dictionary<char, string>();
        private Dictionary<char, Action> answerResults = new Dictionary<char, Action>();
        private char[] answerKeys = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        private char selectedAnswerIndex;

        #region Properties
        public int QuestionID  {  get { return questionID; } }
        public string Question   {  get { return question; } }

        /// <summary>
        /// Char refers to the character indexing the answer, string is the text answer for that character to display
        /// </summary>
        public Dictionary<char, string> AnswerDictionary  { get { return answerTexts; }  }

        /// <summary>
        /// Dictionary of Actions to execute for each selectable answer
        /// </summary>
        public Dictionary<char, Action> AnswerResults { get; set; }
        public char SelectedAnswerIndex
        {
            get { return selectedAnswerIndex; }
            set
            {
                bool bIllegal = true;
                for (int i = 0; i < answerKeys.Length; i++)
                {
                    if (Char.ToLower(value) == answerKeys[i])
                    {
                        selectedAnswerIndex = value;
                        bIllegal = false;
                        break;
                    }                      
                }
                if (bIllegal)
                    Debug.LogError("An illegal value was entered for a background question's selected answer.");     
            }
        }
        #endregion

        /// <summary>
        /// Creates a background question for use in the background questions section of character creation
        /// </summary>
        /// <param name="id">a unique ID for the question</param>
        /// <param name="questionText">the text for the question</param>
        /// <param name="answers">an array containing the answers to the question</param>
        public BackgroundQuestion(int id, string questionText, string[] answers)
        {
            questionID = id;
            question = questionText;
            int i = 0;
            // Only add as many as there are keys and don't add keys without an answer
            while (i < answers.Length && i < answerKeys.Length)
            {
                answerTexts.Add(answerKeys[i], answers[i]); 
                i++;
            }
            //careers = classList;    
	    }

        /// <summary>
        /// uses index to select which Action to call, if no index is provided, Question's selectedAnswerIndex is used
        /// </summary>
        /// <param name="index">Character index to use to call Action</param>
        public void ExecuteAnswerResults(char index = ' ')
        {
            if (index == ' ')
                index = selectedAnswerIndex;
            else
                SelectedAnswerIndex = index;
            // call the Action indexed by selected answer
            answerResults[selectedAnswerIndex]();
        }

        /// <summary>
        /// Has the selectedAnswerIndex been assigned?
        /// </summary>
        /// <returns>returns true if selectedAnswerIndex has been assigned a legal value</returns>
        public bool hasBeenAnswered()
        {
            for (int i = 0; i < answerKeys.Length; i++)
            {
                if (selectedAnswerIndex == answerKeys[i])
                    return true;
            }
            
            return false;
        }

    }
}