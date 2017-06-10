// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallConnect.Arena2;

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
            get { return @"prompt (?<id>\d+) yes (?<yesTaskName>[a-zA-Z0-9_.]+) no (?<noTaskName>[a-zA-Z0-9_.]+)"; }
        }

        public Prompt(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction CreateNew(string source, Quest parentQuest)
        {
            base.CreateNew(source, parentQuest);

            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new prompt
            Prompt prompt = new Prompt(parentQuest);
            prompt.id = Parser.ParseInt(match.Groups["id"].Value);
            prompt.yesTaskSymbol = new Symbol(match.Groups["yesTaskName"].Value);
            prompt.noTaskSymbol = new Symbol(match.Groups["noTaskName"].Value);

            return prompt;
        }

        public override void Update(Task caller)
        {
            ShowPrompt(caller);
            SetComplete();
        }

        void ShowPrompt(Task caller)
        {
            // Get message resource
            Message message = ParentQuest.GetMessage(id);
            if (message == null)
                return;

            // Get message tokens
            TextFile.Token[] tokens = message.GetTextTokens();

            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallMessageBox.CommonMessageBoxButtons.YesNo, tokens);
            messageBox.ClickAnywhereToClose = false;
            messageBox.AllowCancel = false;
            messageBox.ParentPanel.BackgroundColor = Color.clear;
            messageBox.OnButtonClick += MessageBox_OnButtonClick;
            messageBox.Show();
        }

        private void MessageBox_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            // Start yes or no task
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
                ParentQuest.SetTask(yesTaskSymbol);
            else
                ParentQuest.SetTask(noTaskSymbol);

            // Close prompt
            sender.CloseWindow();
        }
    }
}