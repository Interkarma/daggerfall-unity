// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:

using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallQuestOfferWindow : DaggerfallPopupWindow
    {

        const string tempMerchantsQuestsFilename = "Temp-Merchants";
        const string tempNobilityQuestsFilename = "Temp-Nobility";
        string questsFilename;
        Texture2D baseTexture;

        StaticNPC questorNPC;
        Quest offeredQuest;

        #region Constructors

        public DaggerfallQuestOfferWindow(IUserInterfaceManager uiManager, StaticNPC npc, FactionFile.SocialGroups socialGroup)
            : base(uiManager)
        {
            questorNPC = npc;
            TalkManager.Instance.RemoveMerchantQuestor(npc.Data.nameSeed); // Remove potential questor from pool after quest has been offered
            // TODO - assemble quest lists for more social groups
            switch (socialGroup)
            {
                default:
                case FactionFile.SocialGroups.Merchants:
                    questsFilename = tempMerchantsQuestsFilename;
                    break;
                case FactionFile.SocialGroups.Nobility:
                    questsFilename = tempNobilityQuestsFilename;
                    break;
            }
            // Clear background
            ParentPanel.BackgroundColor = Color.clear;
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            Table table = new Table(QuestMachine.Instance.GetTableSourceText(tempMerchantsQuestsFilename));
            string questName = table.GetRow(UnityEngine.Random.Range(0, table.RowCount))[0];
            offeredQuest = QuestMachine.Instance.ParseQuest(questName);
            // Offer the quest to player
            DaggerfallMessageBox messageBox = QuestMachine.Instance.CreateMessagePrompt(offeredQuest, (int)QuestMachine.QuestMessages.QuestorOffer);
            if (messageBox != null)
            {
                messageBox.OnButtonClick += OfferQuest_OnButtonClick;
                messageBox.Show();
            }
        }

        #endregion

        #region Event Handlers

        private void OfferQuest_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
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
                // Show refuse message
                sender.CloseWindow();
                ShowQuestPopupMessage(offeredQuest, (int)QuestMachine.QuestMessages.RefuseQuest, true);
            }
        }

        // Show a popup such as accept/reject message and close service window
        void ShowQuestPopupMessage(Quest quest, int id, bool exitOnClose = true)
        {
            // Get message resource
            Message message = quest.GetMessage(id);
            if (message == null)
                return;

            // Setup popup message
            TextFile.Token[] tokens = message.GetTextTokens();
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
            messageBox.SetTextTokens(tokens);
            messageBox.ClickAnywhereToClose = true;
            messageBox.AllowCancel = true;
            messageBox.ParentPanel.BackgroundColor = Color.clear;

            // Exit on close if requested
            if (exitOnClose)
                messageBox.OnClose += QuestPopupMessage_OnClose;

            // Present popup message
            messageBox.Show();
        }

        private void QuestPopupMessage_OnClose()
        {
            CloseWindow();
        }

        #endregion
    }
}