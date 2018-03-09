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
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Questing;
using System;
using DaggerfallWorkshop.Game.Guilds;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallGuildServicePopupWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        Rect joinButtonRect = new Rect(5, 5, 120, 7);
        Rect talkButtonRect = new Rect(5, 14, 120, 7);
        Rect serviceButtonRect = new Rect(5, 23, 120, 7);
        Rect exitButtonRect = new Rect(44, 33, 43, 15);

        #endregion

        #region UI Controls

        Panel mainPanel = new Panel();
        Button joinButton = new Button();
        Button talkButton = new Button();
        Button serviceButton = new Button();
        Button exitButton = new Button();
        TextLabel serviceLabel = new TextLabel();

        #endregion

        #region Fields

        const string tempFightersQuestsFilename = "Temp-FightersQuests";
        const string tempMagesQuestsFilename = "Temp-MagesGuild";

        const string baseTextureName = "GILD00I0.IMG";      // Join Guild / Talk / Service
        const string memberTextureName = "GILD01I0.IMG";      // Join Guild / Talk / Service

        const int TrainingOfferId = 8;
        const int TrainingTooSkilledId = 4022;
        const int TrainingToSoonId = 4023;
        const int TrainSkillId = 5221;
        const int notEnoughGoldId = 454;
        const int insufficientRankId = 3100;

        Texture2D baseTexture;
        PlayerEntity playerEntity;
        GuildManager guildManager;

        FactionFile.GuildGroups guildGroup;
        StaticNPC serviceNPC;
        GuildNpcServices npcService;
        GuildServices service;
        int buildingFactionId;  // Needed for temples & orders

        Guild guild;
        Quest offeredQuest = null;

        static ItemCollection merchantItems;    // Temporary

        #endregion

        #region Constructors

        public DaggerfallGuildServicePopupWindow(IUserInterfaceManager uiManager, StaticNPC npc, FactionFile.GuildGroups guildGroup, int buildingFactionId)
            : base(uiManager)
        {
            playerEntity = GameManager.Instance.PlayerEntity;
            guildManager = GameManager.Instance.GuildManager;

            serviceNPC = npc;
            npcService = (GuildNpcServices) npc.Data.factionID;
            service = Services.GetService(npcService);
            Debug.Log("NPC offers guild service: " + service.ToString());

            this.guildGroup = guildGroup;
            this.buildingFactionId = buildingFactionId;

            guild = guildManager.GetGuild(guildGroup, buildingFactionId);

            // Clear background
            ParentPanel.BackgroundColor = Color.clear;
        }

        #endregion

        #region Setup Methods

        // TODO: replace with proper merchant item generation...
        ItemCollection GetMerchantItems()
        {
            if (merchantItems == null)
            {
                PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
                ItemCollection items = new ItemCollection();
                DaggerfallUnityItem magicArm = ItemBuilder.CreateRandomArmor(playerEntity.Level, playerEntity.Gender, playerEntity.Race);
                magicArm.legacyMagic = new int[] { 1, 87, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535 };
                items.AddItem(magicArm);
                DaggerfallUnityItem magicWeap = ItemBuilder.CreateRandomWeapon(playerEntity.Level);
                magicWeap.legacyMagic = new int[] { 1, 87, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535 };
                items.AddItem(magicWeap);
                merchantItems = items;
            }
            return merchantItems;
        }

        protected override void Setup()
        {
            // Load all textures
            bool member = guildManager.GetGuild(guildGroup).IsMember();
            LoadTextures(member);

            // Create interface panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.BackgroundTexture = baseTexture;
            mainPanel.Position = new Vector2(0, 50);
            mainPanel.Size = new Vector2(baseTexture.width, baseTexture.height);

            // Join Guild button
            if (!member)
            {
                joinButton = DaggerfallUI.AddButton(joinButtonRect, mainPanel);
                joinButton.OnMouseClick += JoinButton_OnMouseClick;
            }

            // Talk button
            talkButton = DaggerfallUI.AddButton(talkButtonRect, mainPanel);
            talkButton.OnMouseClick += TalkButton_OnMouseClick;

            // Service button
            serviceLabel.Position = new Vector2(0, 1);
            serviceLabel.ShadowPosition = Vector2.zero;
            serviceLabel.HorizontalAlignment = HorizontalAlignment.Center;
            serviceLabel.Text = Services.GetServiceLabelText(service);
            serviceButton = DaggerfallUI.AddButton(serviceButtonRect, mainPanel);
            serviceButton.Components.Add(serviceLabel);
            serviceButton.OnMouseClick += ServiceButton_OnMouseClick;

            // Exit button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, mainPanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            NativePanel.Components.Add(mainPanel);
        }

        public override void OnPush()
        {
            base.OnPush();

            // Check guild advancement
            TextFile.Token[] updatedRank = guild.UpdateRank(playerEntity);
            if (updatedRank != null)
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                messageBox.SetTextTokens(updatedRank, guild);
                messageBox.ClickAnywhereToClose = true;
                uiManager.PushWindow(messageBox);
            }
            // Check for free healing (Temple members)
            if (guild.FreeHealing() && playerEntity.CurrentHealth < playerEntity.MaxHealth)
            {
                playerEntity.SetHealth(playerEntity.MaxHealth);
                DaggerfallUI.MessageBox(350);
            }
            // TODO: Check for magicka restoration (sorcerers)
        }

        #endregion

        #region Private Methods

        void LoadTextures(bool member)
        {
            baseTexture = ImageReader.GetTexture(member ? memberTextureName : baseTextureName);
        }

        #endregion

        #region Event Handlers: General

        private void TalkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            GameManager.Instance.TalkManager.TalkToStaticNPC(serviceNPC);
        }

        private void ServiceButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Check access to service
            if (!guild.CanAccessService(service))
            {
                if (guild.IsMember())
                {
                    DaggerfallMessageBox msgBox = new DaggerfallMessageBox(uiManager, this);
                    msgBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRandomTokens(insufficientRankId));
                    msgBox.ClickAnywhereToClose = true;
                    msgBox.Show();
                }
                else
                {
                    DaggerfallUI.MessageBox(HardStrings.serviceMembersOnly);
                }
                return;
            }
            // Handle known service
            switch (service)
            {
                case GuildServices.Quests:
                    GetQuest();
                    break;

                case GuildServices.Identify:
                    CloseWindow();
                    uiManager.PushWindow(new DaggerfallTradeWindow(uiManager, DaggerfallTradeWindow.WindowModes.Identify, this, guild));
                    break;

                case GuildServices.Repair:
                    CloseWindow();
                    uiManager.PushWindow(new DaggerfallTradeWindow(uiManager, DaggerfallTradeWindow.WindowModes.Repair, this, guild));
                    break;

                case GuildServices.Training:
                    TrainingService();
                    break;

                case GuildServices.BuyMagicItems:   // TODO: switch items depending on npcService?
                    CloseWindow();
                    DaggerfallTradeWindow tradeWindow = new DaggerfallTradeWindow(uiManager, DaggerfallTradeWindow.WindowModes.Buy, this);
                    tradeWindow.MerchantItems = GetMerchantItems();
                    uiManager.PushWindow(tradeWindow);
                    break;

                case GuildServices.Teleport:
                    CloseWindow();
                    DaggerfallUI.Instance.DfTravelMapWindow.ActivateTeleportationTravel();
                    uiManager.PushWindow(DaggerfallUI.Instance.DfTravelMapWindow);
                    break;

                //case GuildServices.BuySpells:
                    //uiManager.PushWindow(new DaggerfallBankingWindow(uiManager, this));
                    //break;

                default:
                    CloseWindow();
                    Services.CustomGuildService customService;
                    if (Services.GetCustomGuildService((int)service, out customService))
                        customService();
                    else
                        DaggerfallUI.MessageBox("Guild service not yet implemented.");
                    break;
            }
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion

        #region Event Handlers: Joining Guild

        private void JoinButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
            guild = guildManager.JoinGuild(guildGroup, buildingFactionId);
            if (guild == null)
            {
                DaggerfallUI.MessageBox("Joining guild " + guildGroup + " not implemented.");
            }
            else if (!guild.IsMember())
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                if (guild.IsEligibleToJoin(playerEntity))
                {
                    messageBox.SetTextTokens(guild.TokensEligible(playerEntity), guild);
                    messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                    messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                    messageBox.OnButtonClick += ConfirmJoinGuild_OnButtonClick;
                }
                else
                {
                    messageBox.SetTextTokens(guild.TokensIneligible(playerEntity));
                    messageBox.ClickAnywhereToClose = true;
                }
                uiManager.PushWindow(messageBox);
            }
        }

        public void ConfirmJoinGuild_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                guildManager.AddMembership(guildGroup, guild);

                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                messageBox.SetTextTokens(guild.TokensWelcome(), guild);
                messageBox.ClickAnywhereToClose = true;
                uiManager.PushWindow(messageBox);
            }
        }

        #endregion

        #region Service Handling: Quests

        void GetQuest()
        {
            // Just exit if this NPC already involved in an active quest
            // If quest conditions are complete the quest system should pickup ending
            if (QuestMachine.Instance.IsLastNPCClickedAnActiveQuestor())
            {
                CloseWindow();
                return;
            }

            // Get member status, including temple specific statuses
            MembershipStatus status = guild.IsMember() ? MembershipStatus.Member : MembershipStatus.Nonmember;
            if (guild.IsMember() && guildGroup == FactionFile.GuildGroups.HolyOrder)
                status = (MembershipStatus) Enum.Parse(typeof(MembershipStatus), ((Temple)guild).Deity.ToString());

            // Get the faction id for affecting reputation on success/failure
            int factionId = (guildGroup == FactionFile.GuildGroups.HolyOrder) ? buildingFactionId : guildManager.GetGuildFactionId(guildGroup);

            // Select a quest at random from appropriate pool
            offeredQuest = GameManager.Instance.QuestTablesManager.GetGuildQuest(guildGroup, status, factionId, guild.GetReputation(playerEntity));
            if (offeredQuest != null)
            {
                // Log offered quest
                Debug.LogFormat("Offering quest {0} from Guild {1} affecting factionId {2}", offeredQuest.QuestName, guildGroup, offeredQuest.FactionId);

                // Offer the quest to player
                DaggerfallMessageBox messageBox = QuestMachine.Instance.CreateMessagePrompt(offeredQuest, (int)QuestMachine.QuestMessages.QuestorOffer);// TODO - need to provide guild mcp for macros
                if (messageBox != null)
                {
                    messageBox.OnButtonClick += OfferQuest_OnButtonClick;
                    messageBox.Show();
                }
            }
            else
            {
                ShowFailGetQuestMessage();
            }
        }

        void ShowFailGetQuestMessage()
        {
            const int flavourMessageID = 600;

            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(flavourMessageID);
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
            messageBox.SetTextTokens(tokens);
            messageBox.ClickAnywhereToClose = true;
            messageBox.AllowCancel = true;
            messageBox.ParentPanel.BackgroundColor = Color.clear;
            messageBox.Show();
        }

        // Show a popup such as accept/reject message close guild window
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
                ShowQuestPopupMessage(offeredQuest, (int)QuestMachine.QuestMessages.RefuseQuest, false);
            }
        }

        private void QuestPopupMessage_OnClose()
        {
            CloseWindow();
        }

        #endregion

        #region Service Handling: Training

        static Dictionary<GuildNpcServices, List<DFCareer.Skills>> guildTrainingSkills = new Dictionary<GuildNpcServices, List<DFCareer.Skills>>()
        {
            { GuildNpcServices.TG_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Backstabbing, DFCareer.Skills.BluntWeapon, DFCareer.Skills.Climbing, DFCareer.Skills.Dodging,
                DFCareer.Skills.Jumping, DFCareer.Skills.Lockpicking, DFCareer.Skills.Pickpocket,
                DFCareer.Skills.ShortBlade, DFCareer.Skills.Stealth, DFCareer.Skills.Streetwise, DFCareer.Skills.Swimming } },
            { GuildNpcServices.DB_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery, DFCareer.Skills.Backstabbing, DFCareer.Skills.Climbing, DFCareer.Skills.CriticalStrike,
                DFCareer.Skills.Daedric, DFCareer.Skills.Destruction, DFCareer.Skills.Dodging, DFCareer.Skills.Running,
                DFCareer.Skills.ShortBlade, DFCareer.Skills.Stealth, DFCareer.Skills.Streetwise, DFCareer.Skills.Swimming } },
        };

        private List<DFCareer.Skills> GetTrainingSkills()
        {
            switch (npcService)
            {
                // Handle Temples even when not a member
                case GuildNpcServices.TAk_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Akatosh);
                case GuildNpcServices.TAr_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Arkay);
                case GuildNpcServices.TDi_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Dibella);
                case GuildNpcServices.TJu_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Julianos);
                case GuildNpcServices.TKy_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Kynareth);
                case GuildNpcServices.TMa_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Mara);
                case GuildNpcServices.TSt_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Stendarr);
                case GuildNpcServices.TZe_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Zenithar);
                default:
                    return guild.TrainingSkills;
            }
        }

        void TrainingService()
        {
            CloseWindow();
            // Check enough time has passed since last trained
            DaggerfallDateTime now = DaggerfallUnity.Instance.WorldTime.Now;
            if ((now.ToClassicDaggerfallTime() - playerEntity.TimeOfLastSkillTraining) < 720)
            {
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(TrainingToSoonId);
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                messageBox.SetTextTokens(tokens);
                messageBox.ClickAnywhereToClose = true;
                messageBox.ParentPanel.BackgroundColor = Color.clear;
                uiManager.PushWindow(messageBox);
            }
            else
            {   // Offer training price
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(TrainingOfferId);
                messageBox.SetTextTokens(tokens, guild);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.Yes);
                messageBox.AddButton(DaggerfallMessageBox.MessageBoxButtons.No);
                messageBox.OnButtonClick += ConfirmTraining_OnButtonClick;
                uiManager.PushWindow(messageBox);
            }
        }

        public void ConfirmTraining_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                if (playerEntity.GetGoldAmount() >= guild.GetTrainingPrice())
                {
                    // Show skill picker loaded with guild training skills
                    DaggerfallListPickerWindow skillPicker = new DaggerfallListPickerWindow(uiManager, this);
                    skillPicker.OnItemPicked += TrainingSkill_OnItemPicked;

                    foreach (DFCareer.Skills skill in GetTrainingSkills())
                        skillPicker.ListBox.AddItem(DaggerfallUnity.Instance.TextProvider.GetSkillName(skill));

                    uiManager.PushWindow(skillPicker);
                }
                else
                    DaggerfallUI.MessageBox(notEnoughGoldId);
            }
        }

        public void TrainingSkill_OnItemPicked(int index, string skillName)
        {
            CloseWindow();
            List<DFCareer.Skills> trainingSkills = GetTrainingSkills();
            DFCareer.Skills skillToTrain = trainingSkills[index];

            if (playerEntity.Skills.GetPermanentSkillValue(skillToTrain) > guild.GetTrainingMax(skillToTrain))
            {
                // Inform player they're too skilled to train
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(TrainingTooSkilledId);
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                messageBox.SetTextTokens(tokens, guild);
                messageBox.ClickAnywhereToClose = true;
                uiManager.PushWindow(messageBox);
            }
            else
            {   // Train the skill
                DaggerfallDateTime now = DaggerfallUnity.Instance.WorldTime.Now;
                playerEntity.TimeOfLastSkillTraining = now.ToClassicDaggerfallTime();
                now.RaiseTime(DaggerfallDateTime.SecondsPerHour * 3);
                playerEntity.DeductGoldAmount(guild.GetTrainingPrice());
                playerEntity.DecreaseFatigue(PlayerEntity.DefaultFatigueLoss * 180);
                int skillAdvancementMultiplier = DaggerfallSkills.GetAdvancementMultiplier(skillToTrain);
                short tallyAmount = (short)(UnityEngine.Random.Range(10, 21) * skillAdvancementMultiplier);
                playerEntity.TallySkill(skillToTrain, tallyAmount);
                DaggerfallUI.MessageBox(TrainSkillId);
            }
        }

        #endregion
    }
}