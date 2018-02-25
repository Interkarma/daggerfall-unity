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
    /// <summary>
    /// Supported guild services.
    /// </summary>
    public enum GuildServices
    {
        // Mages Guild:
        MG_Buy_Spells = 60,
        MG_Training = 61,
        MG_Teleportation = 62,
        MG_Quests = 63,
        MG_MakeSpells = 64,
        MG_BuyMagicItems = 65,
        MG_DaedraSummoning = 66,
        MG_Identify = 801,
        MG_MakeMagicItems = 802,

        // Fighters Guild:
        FG_Training = 849,
        FG_Repairs = 850,
        FG_Quests = 851,

        // Thieves Guild:
        TG_Training = 803,
        TG_Quests = 804,
        TG_SellMagicItems = 805,
        TG_Spymaster = 806,

        // Dark Brotherhood:
        DB_Quests = 807,
        DB_Training = 839,
        DB_MakePotions = 840,
        DB_BuyPotions = 841,
        DB_Spymaster = 842,
        DB_BuySoulgems = 843,

        // Temples, generic:
        T_Quests = 240,
        T_MakeDonation = 810,
        T_CureDiseases = 813,

        // Temples, specific:
        TAr_Training = 241,
        TZe_Training = 243,
        TMa_Training = 245,
        TAk_Training = 247,
        TJu_Training = 249,
        TDi_Training = 250,
        TSt_Training = 252,
        TKy_Training = 254,

        TKy_BuySpells = 497,

        TDi_BuyPotions = 485,
        TDi_MakePotions = 487,

        // Templar orders
        OAk_BuyPotions = 473,
        OAk_MakePotions = 474,
        OAk_DaedraSummoning = 475,

        // Knightly orders:
        KO_Quests = 846,

    }

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

        Texture2D baseTexture;
        PlayerEntity playerEntity;
        GuildManager guildManager;

        StaticNPC serviceNPC;
        FactionFile.GuildGroups guildGroup;
        GuildServices service;
        int buildingFactionId;  // Needed for temples & orders

        Guild guild;
        Quest offeredQuest = null;

        static ItemCollection merchantItems;    // Temporary

        #endregion

        #region Constructors

        public DaggerfallGuildServicePopupWindow(IUserInterfaceManager uiManager, StaticNPC npc, FactionFile.GuildGroups guildGroup, GuildServices service, int buildingFactionId)
            : base(uiManager)
        {
            playerEntity = GameManager.Instance.PlayerEntity;
            guildManager = GameManager.Instance.GuildManager;

            serviceNPC = npc;
            this.guildGroup = guildGroup;
            this.service = service;
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
            LoadTextures();

            // Create interface panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.BackgroundTexture = baseTexture;
            mainPanel.Position = new Vector2(0, 50);
            mainPanel.Size = new Vector2(baseTexture.width, baseTexture.height);

            // Join Guild button
            joinButton = DaggerfallUI.AddButton(joinButtonRect, mainPanel);
            joinButton.OnMouseClick += JoinButton_OnMouseClick;
            if (guildManager.GetGuild(guildGroup).IsMember())  // TODO: use different image
                joinButton.BackgroundColor = DaggerfallUI.DaggerfallUnityNotImplementedColor;

            // Talk button
            talkButton = DaggerfallUI.AddButton(talkButtonRect, mainPanel);
            talkButton.OnMouseClick += TalkButton_OnMouseClick;

            // Service button
            serviceLabel.Position = new Vector2(0, 1);
            serviceLabel.ShadowPosition = Vector2.zero;
            serviceLabel.HorizontalAlignment = HorizontalAlignment.Center;
            serviceLabel.Text = GetServiceLabelText();
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
        }

        #endregion

        #region Private Methods

        string GetServiceLabelText()
        {
            switch (service)
            {
                case GuildServices.FG_Quests:
                case GuildServices.MG_Quests:
                case GuildServices.TG_Quests:
                case GuildServices.DB_Quests:
                case GuildServices.T_Quests:
                    return HardStrings.serviceQuests;

                case GuildServices.MG_Identify:
                    return HardStrings.serviceIdentify;

                case GuildServices.MG_Buy_Spells:
                case GuildServices.TKy_BuySpells:
                    return HardStrings.serviceBuySpells;

                case GuildServices.MG_BuyMagicItems:
                    return HardStrings.serviceBuyMagicItems;

                case GuildServices.MG_MakeSpells:
                    return HardStrings.serviceMakeSpells;

                case GuildServices.MG_MakeMagicItems:
                    return HardStrings.serviceMakeMagicItems;

                case GuildServices.MG_DaedraSummoning:
                    return HardStrings.serviceDaedraSummon;

                case GuildServices.MG_Teleportation:
                    return HardStrings.serviceTeleport;

                case GuildServices.MG_Training:
                case GuildServices.FG_Training:
                case GuildServices.TG_Training:
                case GuildServices.DB_Training:
                case GuildServices.TAk_Training:
                case GuildServices.TAr_Training:
                case GuildServices.TDi_Training:
                case GuildServices.TJu_Training:
                case GuildServices.TKy_Training:
                case GuildServices.TMa_Training:
                case GuildServices.TSt_Training:
                case GuildServices.TZe_Training:
                    return HardStrings.serviceTraining;

                case GuildServices.FG_Repairs:
                    return HardStrings.serviceRepairs;

                case GuildServices.TG_SellMagicItems:
                    return HardStrings.serviceBuyMagicItems;

                case GuildServices.TG_Spymaster:
                case GuildServices.DB_Spymaster:
                    return HardStrings.serviceSpymaster;

                case GuildServices.DB_MakePotions:
                    return HardStrings.serviceMakePotions;

                case GuildServices.DB_BuyPotions:
                    return HardStrings.serviceBuyPotions;

                case GuildServices.DB_BuySoulgems:
                    return HardStrings.serviceBuySoulgems;

                case GuildServices.T_MakeDonation:
                    return HardStrings.serviceDonate;

                case GuildServices.T_CureDiseases:
                    return HardStrings.serviceCure;

                default:
                    return "?";
            }
        }

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName);
        }

        #endregion

        #region Event Handlers: General

        private void TalkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            GameManager.Instance.TalkManager.TalkToStaticNPC(serviceNPC);
        }

        private void ServiceButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            switch (service)
            {
                case GuildServices.FG_Quests:
                case GuildServices.MG_Quests:
                case GuildServices.TG_Quests:
                case GuildServices.DB_Quests:
                case GuildServices.T_Quests:
                    GetQuest();
                    break;

                case GuildServices.MG_Identify:
                    CloseWindow();
                    uiManager.PushWindow(new DaggerfallTradeWindow(uiManager, DaggerfallTradeWindow.WindowModes.Identify, this));
                    break;

                case GuildServices.FG_Repairs:
                    CloseWindow();
                    if (guild.IsMember())
                        uiManager.PushWindow(new DaggerfallTradeWindow(uiManager, DaggerfallTradeWindow.WindowModes.Repair, this, guild));
                    else
                        DaggerfallUI.MessageBox(HardStrings.serviceMembersOnly);
                    break;

                case GuildServices.MG_Training:
                case GuildServices.FG_Training:
                case GuildServices.TG_Training:
                case GuildServices.DB_Training:
                case GuildServices.TAk_Training:
                case GuildServices.TAr_Training:
                case GuildServices.TDi_Training:
                case GuildServices.TJu_Training:
                case GuildServices.TKy_Training:
                case GuildServices.TMa_Training:
                case GuildServices.TSt_Training:
                case GuildServices.TZe_Training:
                    TrainingService();
                    break;

                case GuildServices.MG_BuyMagicItems:
                    CloseWindow();
                    DaggerfallTradeWindow tradeWindow = new DaggerfallTradeWindow(uiManager, DaggerfallTradeWindow.WindowModes.Buy, this);
                    tradeWindow.MerchantItems = GetMerchantItems();
                    uiManager.PushWindow(tradeWindow);
                    break;

                case GuildServices.MG_Teleportation:
                    CloseWindow();
                    DaggerfallUI.Instance.DfTravelMapWindow.ActivateTeleportationTravel();
                    uiManager.PushWindow(DaggerfallUI.Instance.DfTravelMapWindow);
                    break;

                case GuildServices.MG_Buy_Spells:
                    //uiManager.PushWindow(new DaggerfallBankingWindow(uiManager, this));
                    //break;

                default:
                    CloseWindow();
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

        static Dictionary<GuildServices, string> guildQuestTables = new Dictionary<GuildServices, string>()
        {
            { GuildServices.FG_Quests, tempFightersQuestsFilename },
            { GuildServices.MG_Quests, tempMagesQuestsFilename },
        };

        void GetQuest()
        {
            // Just exit if this NPC already involved in an active quest
            // If quest conditions are complete the quest system should pickup ending
            if (QuestMachine.Instance.IsLastNPCClickedAnActiveQuestor())
            {
                CloseWindow();
                return;
            }

            if (guildQuestTables.ContainsKey(service))
            {
                OfferGuildQuest();
            }
            else
            {
                CloseWindow();
                DaggerfallUI.MessageBox("Guild quests not yet implemented.");
            }
        }

        void OfferGuildQuest()
        {
            // Load quests table each time so player can edit their local file at runtime
            Table table = null;
            string questName = string.Empty;
            try
            {
                table = new Table(QuestMachine.Instance.GetTableSourceText(guildQuestTables[service]));

                // Select a quest name at random from table
                if (table == null || table.RowCount == 0)
                    throw new Exception("Quests table is empty.");

                questName = table.GetRow(UnityEngine.Random.Range(0, table.RowCount))[0];
            }
            catch (Exception ex)
            {
                DaggerfallUI.Instance.PopupMessage(ex.Message);
                return;
            }

            // Log offered quest
            Debug.LogFormat("Offering quest {0} from Guild {1}", questName, guildGroup);

            // Parse quest
            try
            {
                offeredQuest = QuestMachine.Instance.ParseQuest(questName);
            }
            catch (Exception ex)
            {
                // Log exception, show random flavour text, and exit
                Debug.LogErrorFormat("Exception during quest compile: {0}", ex.Message);
                ShowFailCompileMessage();
                return;
            }

            // Offer the quest to player
            DaggerfallMessageBox messageBox = QuestMachine.Instance.CreateMessagePrompt(offeredQuest, (int)QuestMachine.QuestMessages.QuestorOffer);
            if (messageBox != null)
            {
                messageBox.OnButtonClick += OfferQuest_OnButtonClick;
                messageBox.Show();
            }
        }

        void ShowFailCompileMessage()
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

        static Dictionary<GuildServices, List<DFCareer.Skills>> guildTrainingSkills = new Dictionary<GuildServices, List<DFCareer.Skills>>()
        {
            { GuildServices.MG_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Alteration, DFCareer.Skills.Daedric, DFCareer.Skills.Destruction, DFCareer.Skills.Dragonish, 
                DFCareer.Skills.Harpy, DFCareer.Skills.Illusion, DFCareer.Skills.Impish, DFCareer.Skills.Mysticism, 
                DFCareer.Skills.Orcish, DFCareer.Skills.Restoration, DFCareer.Skills.Spriggan, DFCareer.Skills.Thaumaturgy } },
            { GuildServices.TG_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Backstabbing, DFCareer.Skills.BluntWeapon, DFCareer.Skills.Climbing, DFCareer.Skills.Dodging,
                DFCareer.Skills.Jumping, DFCareer.Skills.Lockpicking, DFCareer.Skills.Pickpocket,
                DFCareer.Skills.ShortBlade, DFCareer.Skills.Stealth, DFCareer.Skills.Streetwise, DFCareer.Skills.Swimming } },
            { GuildServices.DB_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery, DFCareer.Skills.Backstabbing, DFCareer.Skills.Climbing, DFCareer.Skills.CriticalStrike,
                DFCareer.Skills.Daedric, DFCareer.Skills.Destruction, DFCareer.Skills.Dodging, DFCareer.Skills.Running,
                DFCareer.Skills.ShortBlade, DFCareer.Skills.Stealth, DFCareer.Skills.Streetwise, DFCareer.Skills.Swimming } },
        };

        private List<DFCareer.Skills> GetTrainingSkills()
        {
            switch (service)
            {
                // Handle Temples even when not a member
                case GuildServices.TAk_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Akatosh);
                case GuildServices.TAr_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Arkay);
                case GuildServices.TDi_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Dibella);
                case GuildServices.TJu_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Julianos);
                case GuildServices.TKy_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Kynareth);
                case GuildServices.TMa_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Mara);
                case GuildServices.TSt_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Stendarr);
                case GuildServices.TZe_Training:
                    return Temple.GetTrainingSkills(Temple.Divines.Zenithar);
                default:
                    return guild.TrainingSkills;
            }
        }

        void TrainingService()
        {
            CloseWindow();
            if (guildGroup == FactionFile.GuildGroups.HolyOrder || guild.IsMember())
            {
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
            else
                DaggerfallUI.MessageBox(HardStrings.serviceMembersOnly);
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

            int maxTraining = 50;
            if (DaggerfallSkills.IsLanguageSkill(skillToTrain))     // BCHG: Language skill training is capped by char intelligence instead of 50%
                maxTraining = playerEntity.Stats.PermanentIntelligence;

            if (playerEntity.Skills.GetPermanentSkillValue(skillToTrain) > maxTraining)
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
/*
        #region Macro Handling

        public MacroDataSource GetMacroDataSource()
        {
            return new GuildServiceMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for guild services window.
        /// </summary>
        private class GuildServiceMacroDataSource : MacroDataSource
        {
            private DaggerfallGuildServicePopupWindow parent;
            public GuildServiceMacroDataSource(DaggerfallGuildServicePopupWindow guildServiceWindow)
            {
                this.parent = guildServiceWindow;
            }

            public override string Amount()
            {
                return parent.GetServicePrice().ToString();
            }
        }

        #endregion
    */
    }
}