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
    /// Displays a prompt which user can click to dismiss.
    /// </summary>
    public class Say : ActionTemplate
    {
        int id;

        public override string Pattern
        {
            get { return @"say (?<id>\d+)"; }
        }

        public Say(Quest parentQuest)
            : base(parentQuest)
        {
        }

        public override IQuestAction Create(string source, Quest parentQuest)
        {
            // Source must match pattern
            Match match = Test(source);
            if (!match.Success)
                return null;

            // Factory new say
            Say say = new Say(parentQuest);
            say.id = Parser.ParseInt(match.Groups["id"].Value);

            return say;
        }

        public override void Update(Task caller)
        {
            ShowPopup();
            SetComplete();
        }

        void ShowPopup()
        {
            // Get message resource
            Message message = ParentQuest.GetMessage(id);
            if (message == null)
                return;

            // Get message tokens
            TextFile.Token[] tokens = message.GetTextTokens();

            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
            messageBox.SetTextTokens(tokens);
            messageBox.ClickAnywhereToClose = true;
            messageBox.AllowCancel = true;
            messageBox.ParentPanel.BackgroundColor = Color.clear;
            messageBox.Label.TextColor = DaggerfallUI.DaggerfallQuestTextColor;
            messageBox.Show();
        }
    }
}