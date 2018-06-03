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
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Abstract class for popup windows that can offer quests.
    /// </summary>
    public abstract class DaggerfallQuestPopupWindow : DaggerfallPopupWindow
    {
        protected Quest offeredQuest = null;

        public DaggerfallQuestPopupWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            : base(uiManager, previousWindow)
        {
        }

        protected abstract void GetQuest();
        
        protected virtual void ShowFailGetQuestMessage()
        {
            const int flavourMessageID = 600;

            // Display random flavour message such as "You're too late I gave the job to some spellsword"
            // This is a generic fallback hanlder, does not require quest data or MCP for this popup
            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(flavourMessageID);
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
            messageBox.SetTextTokens(tokens);
            messageBox.ClickAnywhereToClose = true;
            messageBox.AllowCancel = true;
            messageBox.ParentPanel.BackgroundColor = Color.clear;
            messageBox.Show();
        }

        // Show a popup such as accept/reject message close guild window
        protected virtual void ShowQuestPopupMessage(Quest quest, int id, bool exitOnClose = true)
        {
            // Get message resource
            Message message = quest.GetMessage(id);
            if (message == null)
                return;

            // Setup popup message
            TextFile.Token[] tokens = message.GetTextTokens();
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
            messageBox.SetTextTokens(tokens, offeredQuest.ExternalMCP);
            messageBox.ClickAnywhereToClose = true;
            messageBox.AllowCancel = true;
            messageBox.ParentPanel.BackgroundColor = Color.clear;

            // Exit on close if requested
            if (exitOnClose)
                messageBox.OnClose += QuestPopupMessage_OnClose;

            // Present popup message
            messageBox.Show();
        }

        protected virtual void OfferQuest_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                // Show accept message, add quest
                sender.CloseWindow();
                ShowQuestPopupMessage(offeredQuest, (int)QuestMachine.QuestMessages.AcceptQuest);
                QuestMachine.Instance.InstantiateQuest(offeredQuest);
            }
            else
            {
                // inform TalkManager so that it can remove the quest topics that have been added
                // (note by Nystul: I know it is a bit ugly that it is added in the first place at all, but didn't find a good way to do it differently -
                // may revisit this later)
                GameManager.Instance.TalkManager.RemoveQuestInfoTopicsForSpecificQuest(offeredQuest.UID);

                // remove quest rumors (rumor mill command) for this quest from talk manager
                GameManager.Instance.TalkManager.RemoveQuestRumorsFromRumorMill(offeredQuest.UID);

                // remove quest progress rumors for this quest from talk manager
                GameManager.Instance.TalkManager.RemoveQuestProgressRumorsFromRumorMill(offeredQuest.UID);

                // Show refuse message
                sender.CloseWindow();
                ShowQuestPopupMessage(offeredQuest, (int)QuestMachine.QuestMessages.RefuseQuest, false);
            }
        }

        protected virtual void QuestPopupMessage_OnClose()
        {
            CloseWindow();
        }
    }
}