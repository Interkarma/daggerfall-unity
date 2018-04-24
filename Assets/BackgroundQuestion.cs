using DaggerfallWorkshop.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game
{
    [Flags]
    public enum CareerCheckList
    {
        Interkarma = 0,
        Custom = 1,
        Mage = 2,
        Spellsword = 4,
        Battlemage = 8,
        Sorcerer = 16,
        Healer = 32,
        Nightblade = 64,
        Bard = 128,
        Burglar = 256,
        Rogue = 512,
        Acrobat = 1024,
        Thief = 2048,
        Assassin = 4096,
        Monk = 8192,
        Archer = 16384,
        Ranger = 32768,
        Barbarian = 65536,
        Warrior = 131702,
        Knight = 262144
    }
    public class BackgroundQuestion
    {
        /// <summary>
        /// The classes that the question is applicable to
        /// </summary>
        private CareerCheckList careers;
        private int questionID;
        private string question;

        private Dictionary<char, string> answerDictionary;
        private char[] answerKeys = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

        public int QuestionID  {  get { return questionID; } }
        public CareerCheckList Careers { get { return careers; } }
        public string Question   {  get { return question; } }
        /// <summary>
        /// Char refers to the character indexing the answer, string is the text answer for that character to display
        /// </summary>
        public Dictionary<char, string> AnswerDictionary  { get { return answerDictionary; }  }

        /// <summary>
        /// Creates a background question for use in the background questions section of character creation
        /// </summary>
        /// <param name="id">a unique ID for the question</param>
        /// <param name="questionText">the text for the question</param>
        /// <param name="answers">an array containing the answers to the question</param>
        /// <param name="classList">the classes that this question is applicable to</param>
        public BackgroundQuestion(int id, string questionText, string[] answers, CareerCheckList classList)
        {
            questionID = id;
            question = questionText;
            int i = 0;
            // Only add as many as there are keys and don't add keys without an answer
            while (i < answers.Length && i < answerKeys.Length)
            {
                answerDictionary.Add(answerKeys[i], answers[i]); 
                i++;
            }
            careers = classList;    
	    }
    }
}