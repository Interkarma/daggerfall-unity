// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:
//
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using System;
using DaggerfallWorkshop.Game.Questing;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Class for handling Daedra summoning.
    /// </summary>
    public class DaggerfallDaedraSummonedWindow : DaggerfallBaseWindow
    {
        private const int TextLinesPerChunk = 4;
        private const int TokensPerChunk = TextLinesPerChunk * 2;

        Rect messagePanelRect = new Rect(60, 150, 200, 40);

        Panel messagePanel = new Panel();
        MultiFormatTextLabel messageLabel;

        DaggerfallQuestPopupWindow.DaedraData daedraSummoned;
        Quest quest;
        TextFile.Token[] messageTokens;
        int idx = 0;
        bool lastChunk = false;
        bool done = false;

        public DaggerfallDaedraSummonedWindow(IUserInterfaceManager uiManager, DaggerfallQuestPopupWindow.DaedraData daedraSummoned, Quest quest)
            : base(uiManager)
        {
            ParentPanel.BackgroundColor = Color.clear;
            this.daedraSummoned = daedraSummoned;
            this.quest = quest;
        }

        protected override void Setup()
        {
            if (IsSetup)
                return;

            messageLabel = new MultiFormatTextLabel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                ExtraLeading = 3,
                BottomMargin = 100,
            };
            NativePanel.Components.Add(messageLabel);
            NativePanel.OnMouseClick += NativePanel_OnMouseClick;

            // Initialise with the quest offer message.
            Message message = quest.GetMessage((int) QuestMachine.QuestMessages.QuestorOffer);
            messageTokens = message.GetTextTokens();
            idx = 0;
            DisplayNextTextChunk();     
        }

        public override void Update()
        {
            base.Update();

            if (lastChunk)
            {
                idx = 0;
                if (Input.GetKey(KeyCode.Y))
                {
                    HandleAnswer(QuestMachine.QuestMessages.AcceptQuest);
                    QuestMachine.Instance.InstantiateQuest(quest);
                }
                else if (Input.GetKey(KeyCode.N))
                {
                    HandleAnswer(QuestMachine.QuestMessages.RefuseQuest);
                }
            }
        }

        private void HandleAnswer(QuestMachine.QuestMessages qMessage)
        {
            lastChunk = false;
            done = true;
            idx = 0;
            Message message = quest.GetMessage((int) qMessage);
            messageTokens = message.GetTextTokens();
            NativePanel.OnMouseClick += NativePanel_OnMouseClick;
            DisplayNextTextChunk();
        }

        private void DisplayNextTextChunk()
        {
            TextFile.Token[] chunk = new TextFile.Token[TokensPerChunk];
            int len = Math.Min(TokensPerChunk, messageTokens.Length - idx);
            Array.Copy(messageTokens, idx, chunk, 0, len);
            idx += TokensPerChunk;
            messageLabel.SetText(chunk);
            // Is this the last chunk?
            if (len < TokensPerChunk || idx + len == messageTokens.Length)
            {
                lastChunk = true;
                if (!done)
                {   // Disable click and listen for Y/N keypress.
                    NativePanel.OnMouseClick -= NativePanel_OnMouseClick;
                }
            }
        }

        private void NativePanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // If done, close window on click, else display next chunk of text.
            if (lastChunk && done)
                CloseWindow();
            else
                DisplayNextTextChunk();
        }

    }
}