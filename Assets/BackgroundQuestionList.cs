using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Game
{
    public class BackgroundQuestionList
    {
        public List<BackgroundQuestion> List = new List<BackgroundQuestion>();

        public BackgroundQuestionList()
        {
            // add all background questions to List Here:
            
            // unit test
            string[] strings = { "Answer a", "answer b", "Answer c", "answer d", "Answer e", "answer f", "Answer g", "answer h" };
            List.Add(new BackgroundQuestion(1, "This is a question", strings, CareerCheckList.Acrobat | CareerCheckList.Assassin));

            
        }

    }
}
