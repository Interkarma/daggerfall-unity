// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Utility;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Prompt which displays a yes/no dialog that executes a different task based on user input.
    /// </summary>
    public class Prompt : ActionTemplate
    {
        int id;
        Symbol yesTaskSymbol;
        Symbol noTaskSymbol;

        public override string Pattern
        {
            get { return @"prompt (?<id>\d+) yes (?<yesTaskName>[a-zA-Z0-9_.]+) no (?<noTaskName>[a-zA-Z0-9_.]+)|" +
                         @"prompt (?<idName>\w+) yes (?<yesTaskName>[a-zA-Z0-9_.]+) no (?<noTaskName>[a-zA-Z0-9_.]+)"; }
        }

        public Prompt(Quest parentQuest)
            : base(parentQuest)
        {
            allowRearm = false;
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new prompt
            Prompt prompt = new Prompt(parentQuest);
            prompt.id = Parser.ParseInt(match.Groups["id"].Value);
            prompt.yesTaskSymbol = new Symbol(match.Groups["yesTaskName"].Value);
            prompt.noTaskSymbol = new Symbol(match.Groups["noTaskName"].Value);

            // Resolve static message back to ID
            string idName = match.Groups["idName"].Value;
            if (prompt.id == 0 && !string.IsNullOrEmpty(idName))
            {
                Table table = QuestMachine.Instance.StaticMessagesTable;
                prompt.id = Parser.ParseInt(table.GetValue("id", idName));
            }

            return prompt;
        }

        public override void Update(Task caller)
        {
            DaggerfallMessageBox messageBox = QuestMachine.Instance.CreateMessagePrompt(ParentQuest, id);
            if (messageBox != null)
            {
                messageBox.OnButtonClick += MessageBox_OnButtonClick;
                messageBox.Show();
            }

            SetComplete();
        }

        private void MessageBox_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            // Start yes or no task
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
                ParentQuest.StartTask(yesTaskSymbol);
            else
                ParentQuest.StartTask(noTaskSymbol);

            // Close prompt
            sender.CloseWindow();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int id;
            public Symbol yesTaskSymbol;
            public Symbol noTaskSymbol;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.id = id;
            data.yesTaskSymbol = yesTaskSymbol;
            data.noTaskSymbol = noTaskSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            id = data.id;
            yesTaskSymbol = data.yesTaskSymbol;
            noTaskSymbol = data.noTaskSymbol;
        }

        #endregion
    }
}