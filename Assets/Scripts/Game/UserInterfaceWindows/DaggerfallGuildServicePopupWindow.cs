// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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
        MG_Make_Spells = 64,
        MG_Buy_Magic_Items = 65,
        MG_Daedra_Summoning = 66,
        MG_Identify = 801,
        MG_Make_Magic_Items = 802,

        // Fighters Guild:
        FG_Training = 849,
        FG_Repairs = 850,
        FG_Quests = 851,

        // Thieves Guild:
        TG_Training = 803,
        TG_Quests = 804,
        TG_Sell_Magic_Items = 805,
        TG_Spymaster = 806,

        // Dark Brotherhood:
        DB_Quests = 807,
        DB_Training = 839,
        DB_Make_Potions = 840,
        DB_Buy_Potions = 841,
        DB_Spymaster = 841,
        DB_Buy_Soulgems = 843,

        // Temples:
        T_Quests = 240,
        T_Make_Donation = 810,
        T_Cure_Diseases = 813,

        // Temples, specific:
        TAr_Training = 241,
        TZe_Training = 243,
        TMa_Training = 245,
        TAk_Training = 247,
        TJu_Training = 249,
        TDi_Training = 250,
        TSt_Training = 252,
        TKy_Training = 254,
        TKy_Buy_Spells = 256,
    }

    public class DaggerfallGuildServicePopupWindow : DaggerfallPopupWindow, IMacroContextProvider
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

        const string baseTextureName = "GILD00I0.IMG";      // Join Guild / Talk / Service

        const int TrainingOfferId = 8;
        const int TrainingTooSkilledId = 4022;
        const int TrainingToSoonId = 4023;
        const int TrainSkillId = 5221;
        const int notEnoughGoldId = 454;

        Texture2D baseTexture;
        PlayerEntity playerEntity;
        StaticNPC serviceNPC;
        FactionFile.GuildGroups guild;
        GuildServices service;

        static ItemCollection merchantItems;    // Temporary

        #endregion

        #region Constructors

        public DaggerfallGuildServicePopupWindow(IUserInterfaceManager uiManager, StaticNPC npc, FactionFile.GuildGroups guild, GuildServices service)
            : base(uiManager)
        {
            serviceNPC = npc;
            this.guild = guild;
            this.service = service;
            playerEntity = GameManager.Instance.PlayerEntity;
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
                items.AddItem(ItemBuilder.CreateRandomArmor(playerEntity.Level, playerEntity.Gender, playerEntity.Race));
                items.AddItem(ItemBuilder.CreateRandomArmor(playerEntity.Level, playerEntity.Gender, playerEntity.Race));
                items.AddItem(ItemBuilder.CreateRandomArmor(playerEntity.Level, playerEntity.Gender, playerEntity.Race));
                DaggerfallUnityItem magic = ItemBuilder.CreateRandomArmor(playerEntity.Level, playerEntity.Gender, playerEntity.Race);
                magic.legacyMagic = new int[] { 1, 87, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535 };
                items.AddItem(magic);
                items.AddItem(ItemBuilder.CreateRandomBook());
                items.AddItem(ItemBuilder.CreateRandomClothing(playerEntity.Gender, playerEntity.Race));
                items.AddItem(ItemBuilder.CreateRandomClothing(playerEntity.Gender, playerEntity.Race));
                items.AddItem(ItemBuilder.CreateRandomClothing(playerEntity.Gender, playerEntity.Race));
                items.AddItem(ItemBuilder.CreateRandomClothing(playerEntity.Gender, playerEntity.Race));
                items.AddItem(ItemBuilder.CreateRandomIngredient());
                items.AddItem(ItemBuilder.CreateItem(ItemGroups.MiscItems, 274));
                items.AddItem(ItemBuilder.CreateRandomReligiousItem());
                items.AddItem(ItemBuilder.CreateRandomWeapon(playerEntity.Level));
                items.AddItem(ItemBuilder.CreateRandomWeapon(playerEntity.Level));
                items.AddItem(ItemBuilder.CreateRandomWeapon(playerEntity.Level));
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

        #endregion

        #region Private Methods

        string GetServiceLabelText()
        {
            switch (service)
            {
                case GuildServices.MG_Quests:
                    return HardStrings.serviceQuests;
                case GuildServices.MG_Identify:
                    return HardStrings.serviceIdentify;
                case GuildServices.MG_Buy_Spells:
                    return HardStrings.serviceBuySpells;
                case GuildServices.MG_Buy_Magic_Items:
                    return HardStrings.serviceBuyMagicItems;
                case GuildServices.MG_Make_Spells:
                    return HardStrings.serviceMakeSpells;
                case GuildServices.MG_Make_Magic_Items:
                    return HardStrings.serviceMakeMagicItems;
                case GuildServices.MG_Daedra_Summoning:
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
                case GuildServices.T_Cure_Diseases:
                    return HardStrings.serviceCure;
                case GuildServices.T_Make_Donation:
                    return HardStrings.serviceDonate;
                default:
                    return "?";
            }
        }

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName);
        }

        #endregion

        #region Event Handlers

        private void JoinButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.MessageBox("Joining guild " + guild + " not yet implemented.");
        }

        private void TalkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            GameManager.Instance.TalkManager.TalkToStaticNPC(serviceNPC);
        }

        private void ServiceButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
            switch (service)
            {
                case GuildServices.MG_Identify:
                    uiManager.PushWindow(new DaggerfallTradeWindow(uiManager, DaggerfallTradeWindow.WindowModes.Identify, this));
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
                case GuildServices.MG_Buy_Magic_Items:
                    DaggerfallTradeWindow tradeWindow = new DaggerfallTradeWindow(uiManager, DaggerfallTradeWindow.WindowModes.Buy, this);
                    tradeWindow.MerchantItems = GetMerchantItems();
                    uiManager.PushWindow(tradeWindow);
                    break;
                case GuildServices.MG_Buy_Spells:
                    //uiManager.PushWindow(new DaggerfallBankingWindow(uiManager, this));
                    //break;
                default:
                    DaggerfallUI.MessageBox("Guild service not yet implemented.");
                    break;
            }
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion

        #region Service handling

        static Dictionary<GuildServices, List<DFCareer.Skills>> guildTrainingSkills = new Dictionary<GuildServices, List<DFCareer.Skills>>()
        {
            { GuildServices.FG_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery, DFCareer.Skills.Axe, DFCareer.Skills.BluntWeapon, DFCareer.Skills.CriticalStrike, 
                DFCareer.Skills.Giantish, DFCareer.Skills.Jumping, DFCareer.Skills.LongBlade, DFCareer.Skills.Orcish, 
                DFCareer.Skills.Running, DFCareer.Skills.ShortBlade, DFCareer.Skills.Swimming } },
            { GuildServices.MG_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Alteration, DFCareer.Skills.Daedric, DFCareer.Skills.Destruction, DFCareer.Skills.Dragonish, 
                DFCareer.Skills.Harpy, DFCareer.Skills.Illusion, DFCareer.Skills.Impish, DFCareer.Skills.Mysticism, 
                DFCareer.Skills.Orcish, DFCareer.Skills.Restoration, DFCareer.Skills.Spriggan, DFCareer.Skills.Thaumaturgy } },
            { GuildServices.TG_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Backstabbing, DFCareer.Skills.BluntWeapon, DFCareer.Skills.Climbing, DFCareer.Skills.Dodging,
                DFCareer.Skills.Jumping, DFCareer.Skills.Lockpicking, DFCareer.Skills.Pickpocket, DFCareer.Skills.Pickpocket,
                DFCareer.Skills.ShortBlade, DFCareer.Skills.Stealth, DFCareer.Skills.Streetwise, DFCareer.Skills.Swimming } },
            { GuildServices.DB_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery, DFCareer.Skills.Backstabbing, DFCareer.Skills.Climbing, DFCareer.Skills.CriticalStrike,
                DFCareer.Skills.Daedric, DFCareer.Skills.Destruction, DFCareer.Skills.Dodging, DFCareer.Skills.Running,
                DFCareer.Skills.ShortBlade, DFCareer.Skills.Stealth, DFCareer.Skills.Streetwise, DFCareer.Skills.Swimming } },
            { GuildServices.TAk_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Alteration, DFCareer.Skills.Archery, DFCareer.Skills.Daedric, DFCareer.Skills.Destruction,
                DFCareer.Skills.Dragonish, DFCareer.Skills.LongBlade, DFCareer.Skills.Running, DFCareer.Skills.Stealth, DFCareer.Skills.Swimming } },
            { GuildServices.TAr_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Axe, DFCareer.Skills.Backstabbing, DFCareer.Skills.Climbing, DFCareer.Skills.CriticalStrike,
                DFCareer.Skills.Daedric, DFCareer.Skills.Destruction, DFCareer.Skills.Medical, DFCareer.Skills.Restoration, DFCareer.Skills.ShortBlade } },
            { GuildServices.TDi_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Daedric, DFCareer.Skills.Etiquette, DFCareer.Skills.Harpy, DFCareer.Skills.Illusion, DFCareer.Skills.Lockpicking,
                DFCareer.Skills.LongBlade, DFCareer.Skills.Nymph, DFCareer.Skills.Orcish, DFCareer.Skills.Restoration, DFCareer.Skills.Streetwise } },
            { GuildServices.TJu_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Alteration, DFCareer.Skills.CriticalStrike, DFCareer.Skills.Daedric, DFCareer.Skills.Impish, DFCareer.Skills.Lockpicking,
                DFCareer.Skills.Mercantile, DFCareer.Skills.Mysticism, DFCareer.Skills.ShortBlade, DFCareer.Skills.Thaumaturgy } },
            { GuildServices.TKy_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery, DFCareer.Skills.Climbing, DFCareer.Skills.Daedric, DFCareer.Skills.Destruction,
                DFCareer.Skills.Dodging, DFCareer.Skills.Dragonish, DFCareer.Skills.Harpy, DFCareer.Skills.Illusion,
                DFCareer.Skills.Jumping, DFCareer.Skills.Running, DFCareer.Skills.Stealth } },
            { GuildServices.TMa_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Archery, DFCareer.Skills.CriticalStrike, DFCareer.Skills.Daedric, DFCareer.Skills.Etiquette, DFCareer.Skills.Harpy,
                DFCareer.Skills.Illusion, DFCareer.Skills.Medical, DFCareer.Skills.Nymph, DFCareer.Skills.Restoration, DFCareer.Skills.Streetwise } },
            { GuildServices.TSt_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.Axe, DFCareer.Skills.BluntWeapon, DFCareer.Skills.CriticalStrike, DFCareer.Skills.Daedric, DFCareer.Skills.Dodging,
                DFCareer.Skills.Medical, DFCareer.Skills.Orcish, DFCareer.Skills.Restoration, DFCareer.Skills.Spriggan } },
            { GuildServices.TZe_Training, new List<DFCareer.Skills>() {
                DFCareer.Skills.BluntWeapon, DFCareer.Skills.Centaurian, DFCareer.Skills.Daedric, DFCareer.Skills.Etiquette,
                DFCareer.Skills.Giantish, DFCareer.Skills.Harpy, DFCareer.Skills.Mercantile, DFCareer.Skills.Orcish,
                DFCareer.Skills.Pickpocket, DFCareer.Skills.Spriggan, DFCareer.Skills.Streetwise, DFCareer.Skills.Thaumaturgy } },
        };

        void TrainingService()
        {
            // Check enough time has passed since last trained
            DaggerfallDateTime now = DaggerfallUnity.Instance.WorldTime.Now;
            if ((now.ToClassicDaggerfallTime() - playerEntity.TimeOfLastSkillTraining) < 720)
            {
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(TrainingToSoonId);
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                messageBox.SetTextTokens(tokens, this);
                messageBox.ClickAnywhereToClose = true;
                messageBox.ParentPanel.BackgroundColor = Color.clear;
                uiManager.PushWindow(messageBox);
                return;
            }
            else
            {   // Offer training price
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(TrainingOfferId);
                messageBox.SetTextTokens(tokens, this);
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
                if (playerEntity.GetGoldAmount() >= GetServicePrice())
                {
                    // Show skill picker loaded with guild training skills
                    DaggerfallListPickerWindow skillPicker = new DaggerfallListPickerWindow(uiManager, this);
                    skillPicker.OnItemPicked += TrainingSkill_OnItemPicked;

                    List<DFCareer.Skills> trainingSkills;
                    if (guildTrainingSkills.TryGetValue(service, out trainingSkills))
                    {
                        foreach (DFCareer.Skills skill in trainingSkills)
                            skillPicker.ListBox.AddItem(DaggerfallUnity.Instance.TextProvider.GetSkillName(skill));

                        uiManager.PushWindow(skillPicker);
                    }
                }
                else
                    DaggerfallUI.MessageBox(notEnoughGoldId);
            }
        }

        public void TrainingSkill_OnItemPicked(int index, string skillName)
        {
            CloseWindow();
            List<DFCareer.Skills> trainingSkills;
            if (guildTrainingSkills.TryGetValue(service, out trainingSkills))
            {
                DFCareer.Skills skillToTrain = trainingSkills[index];

                if (playerEntity.Skills.GetSkillValue(skillToTrain) > 50)
                {
                    // Inform player they're too skilled to train
                    TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(TrainingTooSkilledId);
                    DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                    messageBox.SetTextTokens(tokens, this);
                    messageBox.ClickAnywhereToClose = true;
                    uiManager.PushWindow(messageBox);
                }
                else
                {   // Train the skill
                    DaggerfallDateTime now = DaggerfallUnity.Instance.WorldTime.Now;
                    playerEntity.TimeOfLastSkillTraining = now.ToClassicDaggerfallTime();
                    now.RaiseTime(DaggerfallDateTime.SecondsPerHour * 3);
                    playerEntity.DeductGoldAmount(GetServicePrice());
                    playerEntity.DecreaseFatigue(PlayerEntity.DefaultFatigueLoss * 180);
                    int skillAdvancementMultiplier = DaggerfallSkills.GetAdvancementMultiplier(skillToTrain);
                    short tallyAmount = (short)(Random.Range(10, 21) * skillAdvancementMultiplier);
                    playerEntity.TallySkill(skillToTrain, tallyAmount);
                    DaggerfallUI.MessageBox(TrainSkillId);
                }
            }
            else
                Debug.LogError("Invalid skill selected for training.");
        }

        int GetServicePrice()
        {
            // TODO: 400 * level, if non-member of temple
            return 100 * playerEntity.Level;
        }

        #endregion

        #region Macro handling

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

    }
}