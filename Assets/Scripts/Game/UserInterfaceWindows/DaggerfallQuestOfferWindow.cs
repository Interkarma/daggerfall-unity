// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors: Hazelnut

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implementation of a (dummy) popup window that can offer quests.
    /// </summary>
    public class DaggerfallQuestOfferWindow : DaggerfallQuestPopupWindow
    {
        StaticNPC.NPCData questorNPC;
        FactionFile.SocialGroups socialGroup;
        bool menu;

        #region Constructors

        public DaggerfallQuestOfferWindow(IUserInterfaceManager uiManager, StaticNPC.NPCData npc, FactionFile.SocialGroups socialGroup, bool menu)
            : base(uiManager)
        {
            questorNPC = npc;
            this.socialGroup = socialGroup;
            this.menu = menu;

            // Remove potential questor from pool after quest has been offered
            if (!GameManager.Instance.IsPlayerInsideCastle)
                TalkManager.Instance.RemoveNpcQuestor(npc.nameSeed);

            // Clear background
            ParentPanel.BackgroundColor = Color.clear;
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            CloseWindow();
            GetQuest();
        }

        #endregion

        #region Quest handling

        protected override void GetQuest()
        {
            // Just exit if this NPC already involved in an active quest
            // If quest conditions are complete the quest system should pickup ending
            if (QuestMachine.Instance.IsLastNPCClickedAnActiveQuestor())
            {
                CloseWindow();
                return;
            }

            // Get the faction id for affecting reputation on success/failure, and current rep
            int factionId = questorNPC.factionID;
            int reputation = GameManager.Instance.PlayerEntity.FactionData.GetReputation(factionId);
            int level = GameManager.Instance.PlayerEntity.Level;

            // Select a quest at random from appropriate pool
            offeredQuest = GameManager.Instance.QuestListsManager.GetSocialQuest(socialGroup, factionId, reputation, level);
            if (offeredQuest != null)
            {
                // Log offered quest
                Debug.LogFormat("Offering quest {0} from Social group {1} affecting factionId {2}", offeredQuest.QuestName, socialGroup, offeredQuest.FactionId);

                // Offer the quest to player
                DaggerfallMessageBox messageBox = QuestMachine.Instance.CreateMessagePrompt(offeredQuest, (int)QuestMachine.QuestMessages.QuestorOffer);// TODO - need to provide an mcp for macros?
                if (messageBox != null)
                {
                    messageBox.OnButtonClick += OfferQuest_OnButtonClick;
                    messageBox.Show();
                }
            }
            else if (!GameManager.Instance.IsPlayerInsideCastle) // Failed get quest messages do not appear inside castles in classic.
            {
                ShowFailGetQuestMessage();
            }
        }

        protected override void QuestPopupMessage_OnClose()
        {
            // Close popup menu if talk was initiated from one
            if (menu) {
                CloseWindow();
            }
        }

        // Not required for this class.
        public override MacroDataSource GetMacroDataSource()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}