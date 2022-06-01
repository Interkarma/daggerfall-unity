// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 
// Notes:
//

using System.Text.RegularExpressions;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using FullSerializer;

namespace DaggerfallWorkshop.Game.Questing.Actions
{
    /// <summary>
    /// Prompt which displays a dialog with 2-4 buttons that each execute a different task based on user selection.
    ///
    /// Example usage:
    /// promptmulti 1072 4:noChoice _dirRand_ 24:south _headS_ 25:west _headW_ 28:swest _headSW_
    /// </summary>
    public class PromptMulti : ActionTemplate
    {
        int id;
        int opt1button, opt2button, opt3button, opt4button;
        Symbol opt1TaskSymbol;
        Symbol opt2TaskSymbol;
        Symbol opt3TaskSymbol;
        Symbol opt4TaskSymbol;

        public override string Pattern
        {
            get {
                return @"promptmulti (?<id>\d+) (?<opt1>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt1TaskName>[a-zA-Z0-9_.]+) (?<opt2>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt2TaskName>[a-zA-Z0-9_.]+) (?<opt3>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt3TaskName>[a-zA-Z0-9_.]+) (?<opt4>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt4TaskName>[a-zA-Z0-9_.]+)|" +
                       @"promptmulti (?<id>\d+) (?<opt1>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt1TaskName>[a-zA-Z0-9_.]+) (?<opt2>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt2TaskName>[a-zA-Z0-9_.]+) (?<opt3>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt3TaskName>[a-zA-Z0-9_.]+)|" +
                       @"promptmulti (?<id>\d+) (?<opt1>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt1TaskName>[a-zA-Z0-9_.]+) (?<opt2>[0-9]+)(:[a-zA-Z0-9]+)? (?<opt2TaskName>[a-zA-Z0-9_.]+)";
            }
        }

        public PromptMulti(Quest parentQuest)
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
            PromptMulti prompt = new PromptMulti(parentQuest);
            prompt.id = Parser.ParseInt(match.Groups["id"].Value);

            prompt.opt1button = Parser.ParseInt(match.Groups["opt1"].Value);
            prompt.opt1TaskSymbol = new Symbol(match.Groups["opt1TaskName"].Value);

            prompt.opt2button = Parser.ParseInt(match.Groups["opt2"].Value);
            prompt.opt2TaskSymbol = new Symbol(match.Groups["opt2TaskName"].Value);

            Group opt3TaskGroup = match.Groups["opt3TaskName"];
            if (opt3TaskGroup.Success) {
                prompt.opt3button = Parser.ParseInt(match.Groups["opt3"].Value);
                prompt.opt3TaskSymbol = new Symbol(opt3TaskGroup.Value);
            }
            Group opt4TaskGroup = match.Groups["opt4TaskName"];
            if (opt4TaskGroup.Success) {
                prompt.opt4button = Parser.ParseInt(match.Groups["opt4"].Value);
                prompt.opt4TaskSymbol = new Symbol(opt4TaskGroup.Value);
            }

            return prompt;
        }

        public override void Update(Task caller)
        {
            Message message = ParentQuest.GetMessage(id);
            TextFile.Token[] tokens = message.GetTextTokens();

            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
            messageBox.SetTextTokens(tokens);
            messageBox.ClickAnywhereToClose = false;
            messageBox.AllowCancel = false;
            messageBox.ParentPanel.BackgroundColor = UnityEngine.Color.clear;

            messageBox.AddButton((DaggerfallMessageBox.MessageBoxButtons)opt1button, true);
            messageBox.AddButton((DaggerfallMessageBox.MessageBoxButtons)opt2button);
            if (opt3TaskSymbol != null) {
                messageBox.ButtonSpacing = 28;
                messageBox.AddButton((DaggerfallMessageBox.MessageBoxButtons)opt3button);
            }
            if (opt4TaskSymbol != null) {
                messageBox.ButtonSpacing = 24;
                messageBox.AddButton((DaggerfallMessageBox.MessageBoxButtons)opt4button);
            }

            messageBox.OnButtonClick += MessageBox_OnButtonClick;
            messageBox.Show();

            SetComplete();
        }

        private void MessageBox_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            // Start selected task
            if (messageBoxButton == (DaggerfallMessageBox.MessageBoxButtons)opt1button)
                ParentQuest.StartTask(opt1TaskSymbol);
            else if(messageBoxButton == (DaggerfallMessageBox.MessageBoxButtons)opt2button)
                ParentQuest.StartTask(opt2TaskSymbol);
            else if (messageBoxButton == (DaggerfallMessageBox.MessageBoxButtons)opt3button)
                ParentQuest.StartTask(opt3TaskSymbol);
            else if (messageBoxButton == (DaggerfallMessageBox.MessageBoxButtons)opt4button)
                ParentQuest.StartTask(opt4TaskSymbol);

            // Close prompt
            sender.CloseWindow();
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int id;
            public int opt1button;
            public int opt2button;
            public int opt3button;
            public int opt4button;
            public Symbol opt1TaskSymbol;
            public Symbol opt2TaskSymbol;
            public Symbol opt3TaskSymbol;
            public Symbol opt4TaskSymbol;

        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.id = id;
            data.opt1button = opt1button;
            data.opt2button = opt2button;
            data.opt3button = opt3button;
            data.opt4button = opt4button;
            data.opt1TaskSymbol = opt1TaskSymbol;
            data.opt2TaskSymbol = opt2TaskSymbol;
            data.opt3TaskSymbol = opt3TaskSymbol;
            data.opt4TaskSymbol = opt4TaskSymbol;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            id = data.id;
            opt1button = data.opt1button;
            opt2button = data.opt2button;
            opt3button = data.opt3button;
            opt4button = data.opt4button;
            opt1TaskSymbol = data.opt1TaskSymbol;
            opt2TaskSymbol = data.opt2TaskSymbol;
            opt3TaskSymbol = data.opt3TaskSymbol;
            opt4TaskSymbol = data.opt4TaskSymbol;
        }

        #endregion
    }
}