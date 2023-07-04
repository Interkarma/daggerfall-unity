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

using System;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallGuildServicePopupWindow : DaggerfallQuestPopupWindow, IMacroContextProvider
    {
        public const int NoPotionIngredients = 34;
        public const int SorcerorMagickaRecharge = 465;
        public const int TempleResetStats = 403;
        public const int InsufficientRankId = 3100;

        #region UI Rects

        protected Rect joinButtonRect = new Rect(5, 5, 120, 7);
        protected Rect talkButtonRect = new Rect(5, 14, 120, 7);
        protected Rect serviceButtonRect = new Rect(5, 23, 120, 7);
        protected Rect exitButtonRect = new Rect(44, 33, 43, 15);

        #endregion

        #region UI Controls

        protected Panel mainPanel = new Panel();
        protected Button joinButton = new Button();
        protected Button talkButton = new Button();
        protected Button serviceButton = new Button();
        protected Button exitButton = new Button();
        protected TextLabel serviceLabel = new TextLabel();

        #endregion

        #region Fields

        protected const string baseTextureName = "GILD00I0.IMG";      // Join Guild / Talk / Service
        protected const string memberTextureName = "GILD01I0.IMG";    // Join Guild / Talk / Service

        protected Texture2D baseTexture;
        protected PlayerEntity playerEntity;
        protected GuildManager guildManager;

        protected FactionFile.GuildGroups guildGroup;
        protected StaticNPC serviceNPC;
        protected GuildNpcServices npcService;
        protected GuildServices service;
        protected int buildingFactionId;  // Needed for temples & orders

        protected IGuild guild;
        protected PlayerGPS.DiscoveredBuilding buildingDiscoveryData;

        bool isCloseWindowDeferred = false;
        bool isTalkWindowDeferred = false;
        bool isServiceWindowDeferred = false;

        protected List<QuestData> questPool;

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

        protected override void Setup()
        {
            // Ascertain guild membership status, exempt Thieves Guild and Dark Brotherhood since should never find em until a member
            bool member = guildManager.GetGuild(guildGroup).IsMember();
            if (guildGroup == FactionFile.GuildGroups.DarkBrotherHood || guildGroup == FactionFile.GuildGroups.GeneralPopulace)
                member = true;

            // Load all textures
            LoadTextures(member);

            // Create interface panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.BackgroundTexture = baseTexture;
            mainPanel.Position = new Vector2(0, 50);
            mainPanel.Size = new Vector2(130, 51);

            // Join Guild button
            if (!member)
            {
                joinButton = DaggerfallUI.AddButton(joinButtonRect, mainPanel);
                joinButton.OnMouseClick += JoinButton_OnMouseClick;
                joinButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.GuildsJoin);
            }

            // Talk button
            talkButton = DaggerfallUI.AddButton(talkButtonRect, mainPanel);
            talkButton.OnMouseClick += TalkButton_OnMouseClick;
            talkButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.GuildsTalk);
            talkButton.OnKeyboardEvent += TalkButton_OnKeyboardEvent;

            // Service button
            serviceLabel.Position = new Vector2(0, 1);
            serviceLabel.ShadowPosition = Vector2.zero;
            serviceLabel.HorizontalAlignment = HorizontalAlignment.Center;
            serviceLabel.Text = Services.GetServiceLabelText(service);
            serviceButton = DaggerfallUI.AddButton(serviceButtonRect, mainPanel);
            serviceButton.Components.Add(serviceLabel);
            serviceButton.OnMouseClick += ServiceButton_OnMouseClick;
            serviceButton.Hotkey = DaggerfallShortcut.GetBinding(Services.GetServiceShortcutButton(service));
            serviceButton.OnKeyboardEvent += ServiceButton_OnKeyboardEvent;

            // Exit button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, mainPanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.GuildsExit);
            exitButton.OnKeyboardEvent += ExitButton_OnKeyboardEvent;

            NativePanel.Components.Add(mainPanel);
        }

        public override void OnPush()
        {
            base.OnPush();

            buildingDiscoveryData = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;
            
            // Check guild advancement
            TextFile.Token[] updatedRank = guild.UpdateRank(playerEntity);
            if (updatedRank != null)
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                messageBox.SetTextTokens(updatedRank, guild);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
            }
            // Check for free healing (Temple members)
            if (guild.FreeHealing())
            {
                // Free heal
                if (playerEntity.CurrentHealth < playerEntity.MaxHealth)
                {
                    playerEntity.SetHealth(playerEntity.MaxHealth);
                    DaggerfallUI.MessageBox(350);
                }
                // Check for free stat restoration (Temple members)
                EntityEffectManager playerEffectMgr = playerEntity.EntityBehaviour.GetComponent<EntityEffectManager>();
                if (playerEffectMgr != null)
                {
                    if (playerEffectMgr.HasDamagedAttributes())
                    {
                        DaggerfallMessageBox messageBox =
                            new DaggerfallMessageBox(uiManager, DaggerfallMessageBox.CommonMessageBoxButtons.YesNo, TempleResetStats, uiManager.TopWindow);
                        messageBox.OnButtonClick += ConfirmStatReset_OnButtonClick;
                        messageBox.Show();
                    }
                }
            }
            // Check for magicka restoration (sorcerers)
            if (guild.FreeMagickaRecharge() && playerEntity.CurrentMagicka < playerEntity.MaxMagicka)
            {
                DaggerfallMessageBox msgBox = new DaggerfallMessageBox(uiManager, this);
                msgBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRandomTokens(SorcerorMagickaRecharge));
                msgBox.ClickAnywhereToClose = true;
                msgBox.Show();

                // Refill magicka
                playerEntity.SetMagicka(playerEntity.MaxMagicka);
            }
        }

        protected virtual void ConfirmStatReset_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                EntityEffectManager playerEffectMgr = playerEntity.EntityBehaviour.GetComponent<EntityEffectManager>();
                playerEffectMgr.CureAllAttributes();
            }
        }

        #endregion

        #region Protected Methods

        protected virtual ItemCollection GetMerchantMagicItems(bool onlySoulGems = false)
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            ItemCollection items = new ItemCollection();
            int numOfItems = (buildingDiscoveryData.quality / 2) + 1;

            // Seed random from game time to rotate magic stock every 24 game hours
            // This more or less resolves issue of magic item stock not being deterministic every time player opens window
            // Doesn't match classic exactly as classic stocking method unknown, but should be "good enough" for now
            int seed = (int)(DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() / DaggerfallDateTime.MinutesPerDay);
            UnityEngine.Random.InitState(seed);

            if (!onlySoulGems)
            {
                for (int i = 0; i <= numOfItems; i++)
                {
                    // Create magic item which is already identified
                    DaggerfallUnityItem magicItem = ItemBuilder.CreateRandomMagicItem(playerEntity.Level, playerEntity.Gender, playerEntity.Race);
                    magicItem.IdentifyItem();
                    items.AddItem(magicItem);
                }
                items.AddItem(ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Spellbook));
            }

            if (guild.CanAccessService(GuildServices.BuySoulgems))
            {
                for (int i = 0; i <= numOfItems; i++)
                {
                    DaggerfallUnityItem magicItem;
                    if (Dice100.FailedRoll(25))
                    {
                        // Empty soul trap
                        magicItem = ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Soul_trap);
                        magicItem.value = 5000;
                        magicItem.TrappedSoulType = MobileTypes.None;
                    }
                    else
                    {
                        // Filled soul trap
                        magicItem = ItemBuilder.CreateRandomlyFilledSoulTrap();
                    }
                    items.AddItem(magicItem);
                }
            }
            return items;
        }

        // TODO: classic seems to generate each time player select buy potions.. should we make more persistent for DFU?
        protected virtual ItemCollection GetMerchantPotions()
        {
            ItemCollection potions = new ItemCollection();
            int n = buildingDiscoveryData.quality;
            while (n-- >= 0)
                potions.AddItem(ItemBuilder.CreateRandomPotion(UnityEngine.Random.Range(1, 5)));
            return potions;
        }

        void LoadTextures(bool member)
        {
            baseTexture = ImageReader.GetTexture(member ? memberTextureName : baseTextureName);
        }

        #endregion

        #region Event Handlers: General

        protected virtual void TalkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            GameManager.Instance.TalkManager.TalkToStaticNPC(serviceNPC);
        }

        protected virtual void TalkButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isTalkWindowDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isTalkWindowDeferred)
            {
                isTalkWindowDeferred = false;
                GameManager.Instance.TalkManager.TalkToStaticNPC(serviceNPC);
            }
        }

        protected virtual void DoGuildService()
        {
            // Check access to service
            if (!guild.CanAccessService(service))
            {
                if (guild.IsMember())
                {
                    DaggerfallMessageBox msgBox = new DaggerfallMessageBox(uiManager, this);
                    msgBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRandomTokens(InsufficientRankId));
                    msgBox.ClickAnywhereToClose = true;
                    msgBox.Show();
                }
                else
                {
                    DaggerfallUI.MessageBox(TextManager.Instance.GetLocalizedText("serviceMembersOnly"));
                }
                return;
            }

            // Handle custom service
            Services.CustomGuildService customService;
            if (Services.GetCustomGuildService((int)service, out customService))
            {
                CloseWindow();
                customService(this);
                return;
            }

            // Handle known service
            DaggerfallTradeWindow tradeWindow;
            switch (service)
            {
                case GuildServices.Quests:
                    GetQuest();
                    break;

                case GuildServices.Identify:
                    CloseWindow();
                    uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.Trade, new object[] { uiManager, this, DaggerfallTradeWindow.WindowModes.Identify, guild }));
                    break;

                case GuildServices.Repair:
                    CloseWindow();
                    uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.Trade, new object[] { uiManager, this, DaggerfallTradeWindow.WindowModes.Repair, guild }));
                    break;

                case GuildServices.Training:
                    CloseWindow();
                    UIWindowFactory.GetInstanceWithArgs(UIWindowType.GuildServiceTraining, new object[] { uiManager, npcService, guild });
                    break;

                case GuildServices.Donate:
                    CloseWindow();
                    uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.GuildServiceDonation, new object[] { uiManager, buildingFactionId }));
                    break;

                case GuildServices.CureDisease:
                    CloseWindow();
                    uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.GuildServiceCureDisease, new object[] { uiManager, buildingFactionId, guild }));
                    break;

                case GuildServices.BuyPotions:
                    CloseWindow();
                    tradeWindow = (DaggerfallTradeWindow)UIWindowFactory.GetInstanceWithArgs(UIWindowType.Trade, new object[] { uiManager, this, DaggerfallTradeWindow.WindowModes.Buy, guild });
                    tradeWindow.MerchantItems = GetMerchantPotions();
                    uiManager.PushWindow(tradeWindow);
                    break;

                case GuildServices.MakePotions:
                    MakePotionService();
                    break;

                case GuildServices.BuySpells:
                case GuildServices.BuySpellsMages:
                    CloseWindow();
                    uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.SpellBook, new object[] { uiManager, this, true }));
                    break;

                case GuildServices.MakeSpells:
                    CloseWindow();
                    if (GameManager.Instance.PlayerEntity.Items.Contains(ItemGroups.MiscItems, (int)MiscItems.Spellbook))
                        uiManager.PushWindow(DaggerfallUI.Instance.DfSpellMakerWindow);
                    else
                        DaggerfallUI.MessageBox(TextManager.Instance.GetLocalizedText("noSpellbook"));
                    break;

                case GuildServices.BuyMagicItems:
                    CloseWindow();
                    tradeWindow = (DaggerfallTradeWindow)UIWindowFactory.GetInstanceWithArgs(UIWindowType.Trade, new object[] { uiManager, this, DaggerfallTradeWindow.WindowModes.Buy, guild });
                    tradeWindow.MerchantItems = GetMerchantMagicItems();
                    uiManager.PushWindow(tradeWindow);
                    break;

                case GuildServices.MakeMagicItems:
                    CloseWindow();
                    uiManager.PushWindow(DaggerfallUI.Instance.DfItemMakerWindow);
                    break;

                case GuildServices.SellMagicItems:
                    CloseWindow();
                    uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.Trade, new object[] { uiManager, this, DaggerfallTradeWindow.WindowModes.SellMagic, guild }));
                    break;

                case GuildServices.Teleport:
                    CloseWindow();
                    DaggerfallUI.Instance.DfTravelMapWindow.ActivateTeleportationTravel();
                    uiManager.PushWindow(DaggerfallUI.Instance.DfTravelMapWindow);
                    break;

                case GuildServices.DaedraSummoning:
                    DaedraSummoningService((int)npcService);
                    break;

                case GuildServices.ReceiveArmor:
                    ReceiveArmorService();
                    break;

                case GuildServices.ReceiveHouse:
                    ReceiveHouseService();
                    break;

                case GuildServices.Spymaster:
                    const int spyMasterGreetingTextId = 402;
                    DaggerfallMessageBox msgBox = new DaggerfallMessageBox(uiManager, this);
                    msgBox.SetTextTokens(DaggerfallUnity.Instance.TextProvider.GetRandomTokens(spyMasterGreetingTextId));
                    msgBox.ClickAnywhereToClose = true;
                    msgBox.OnClose += SpyMasterGreetingPopUp_OnClose;
                    msgBox.Show();
                    break;

                case GuildServices.BuySoulgems:
                    CloseWindow();
                    tradeWindow = (DaggerfallTradeWindow)UIWindowFactory.GetInstanceWithArgs(UIWindowType.Trade, new object[] { uiManager, this, DaggerfallTradeWindow.WindowModes.Buy, guild });
                    tradeWindow.MerchantItems = GetMerchantMagicItems(true);
                    uiManager.PushWindow(tradeWindow);
                    break;

                default:
                    CloseWindow();
                    DaggerfallUI.MessageBox("Guild service not yet implemented.");
                    break;
            }
        }

        protected virtual void ServiceButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DoGuildService();
        }

        protected virtual void ServiceButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isServiceWindowDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isServiceWindowDeferred)
            {
                isServiceWindowDeferred = false;
                DoGuildService();
            }
        }

        protected virtual void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseWindow();
        }

        protected void ExitButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isCloseWindowDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isCloseWindowDeferred)
            {
                isCloseWindowDeferred = false;
                CloseWindow();
            }
        }

        #endregion

        #region Event Handlers: Joining Guild

        protected virtual void JoinButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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
                    messageBox.SetTextTokens(guild.TokensIneligible(playerEntity), guild);
                    messageBox.ClickAnywhereToClose = true;
                }
                messageBox.Show();
            }
        }

        protected virtual void ConfirmJoinGuild_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                guildManager.AddMembership(guildGroup, guild);

                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, uiManager.TopWindow);
                messageBox.SetTextTokens(guild.TokensWelcome(), guild);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
            }
        }

        #endregion

        #region Service Handling: Quests

        protected override void GetQuest()
        {
            // Just exit if this NPC already involved in an active quest
            // If quest conditions are complete the quest system should pickup ending
            if (QuestMachine.Instance.IsLastNPCClickedAnActiveQuestor(guild))
            {
                CloseWindow();
                return;
            }

            // Get member status, including temple specific statuses
            MembershipStatus status = guild.IsMember() ? MembershipStatus.Member : MembershipStatus.Nonmember;
            if (guild.IsMember() && guildGroup == FactionFile.GuildGroups.HolyOrder)
                status = (MembershipStatus)Enum.Parse(typeof(MembershipStatus), ((Temple)guild).Deity.ToString());

            // Get the faction id for affecting reputation on success/failure
            int factionId = GetFactionIdForGuild();

            // Set up a pool of available quests.
            QuestListsManager questListsManager = GameManager.Instance.QuestListsManager;
            questPool = questListsManager.GetGuildQuestPool(guildGroup, status, factionId, guild.GetReputation(playerEntity), guild.Rank);

            // Show the quest selection list if that feature has been enabled.
            if (DaggerfallUnity.Settings.GuildQuestListBox)
            {
                TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.CreateTokens(
                    TextFile.Formatting.JustifyCenter,
                    TextManager.Instance.GetLocalizedText("gettingQuests1"),
                    TextManager.Instance.GetLocalizedText("gettingQuests2"));

                DaggerfallMessageBox gettingQuestsBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, this);
                gettingQuestsBox.ClickAnywhereToClose = true;
                gettingQuestsBox.SetTextTokens(tokens);
                gettingQuestsBox.OnClose += GettingQuestsBox_OnClose;
                gettingQuestsBox.Show();
            }
            else
            {
                // Select a quest at random from appropriate pool
                offeredQuest = questListsManager.SelectQuest(questPool, factionId);
                OfferQuest();
            }
        }

        private int GetFactionIdForGuild()
        {
            return (guildGroup == FactionFile.GuildGroups.HolyOrder || guildGroup == FactionFile.GuildGroups.KnightlyOrder) ? buildingFactionId : guildManager.GetGuildFactionId(guildGroup);
        }

        protected virtual void OfferQuest()
        {
            if (offeredQuest != null)
            {
                // Log offered quest
                Debug.LogFormat("Offering quest {0} from Guild {1} affecting factionId {2}", offeredQuest.QuestName, guildGroup, offeredQuest.FactionId);

                // Offer the quest to player, setting external context provider to guild if a member
                if (guild.IsMember())
                    offeredQuest.ExternalMCP = guild;
                DaggerfallMessageBox messageBox = QuestMachine.Instance.CreateMessagePrompt(offeredQuest, (int)QuestMachine.QuestMessages.QuestorOffer);
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
            questPool.Clear();
        }

        protected virtual void GettingQuestsBox_OnClose()
        {
            DaggerfallListPickerWindow questPicker = new DaggerfallListPickerWindow(uiManager, uiManager.TopWindow);
            questPicker.OnItemPicked += QuestPicker_OnItemPicked;
            // Populate a list with quests that the player can choose from.
            List<int> failures = new List<int>();
            for (int i = 0; i < questPool.Count; i++)
            {
                try
                {
                    // Partially loading the quest to get the human readable quest name.
                    Quest quest = GameManager.Instance.QuestListsManager.LoadQuest(questPool[i], GetFactionIdForGuild(), true);
                    string displayName = quest.DisplayName;
                    string localizedDisplayName = QuestMachine.Instance.GetLocalizedQuestDisplayName(quest.QuestName);
                    if (!string.IsNullOrEmpty(localizedDisplayName))
                        displayName = localizedDisplayName;
                    questPicker.ListBox.AddItem(displayName ?? quest.QuestName);
                    quest.Dispose();
                }
                catch (Exception)
                {
                    failures.Add(i);  // Add to list any that fail (partial) parsing
                }
            }
            // Remove any quests that failed partial parsing
            foreach (int i in failures)
                questPool.RemoveAt(i);

            uiManager.PushWindow(questPicker);
        }

        protected virtual void QuestPicker_OnItemPicked(int index, string name)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaggerfallUI.UIManager.PopWindow();
            if (index < questPool.Count)
            {
                offeredQuest = GameManager.Instance.QuestListsManager.LoadQuest(questPool[index], GetFactionIdForGuild());
                OfferQuest();
            }
        }

        #endregion

        #region Service Handling: Make Potions

        private void MakePotionService()
        {
            // Open potion mixer window if player has some ingredients
            CloseWindow();
            foreach (ItemCollection playerItems in new ItemCollection[] { GameManager.Instance.PlayerEntity.Items, GameManager.Instance.PlayerEntity.WagonItems })
            {
                for (int i = 0; i < playerItems.Count; i++)
                {
                    if (playerItems.GetItem(i).IsIngredient)
                    {
                        uiManager.PushWindow(DaggerfallUI.Instance.DfPotionMakerWindow);
                        return;
                    }
                }
            }
            DaggerfallUI.MessageBox(NoPotionIngredients);
        }

        #endregion

        #region Service Handling: Receive Armor / House (Knightly Orders only)

        void ReceiveArmorService()
        {
            CloseWindow();
            KnightlyOrder order = (KnightlyOrder) guild;
            order.ReceiveArmor(playerEntity);
        }

        void ReceiveHouseService()
        {
            CloseWindow();
            KnightlyOrder order = (KnightlyOrder) guild;
            order.ReceiveHouse();
        }

        #endregion

        #region Service Handling: SpyMaster

        // Message box closed, talk to SpyMaster
        private void SpyMasterGreetingPopUp_OnClose()
        {
            GameManager.Instance.TalkManager.TalkToStaticNPC(QuestMachine.Instance.LastNPCClicked, true, true);
        }

        #endregion

        #region Macro handling

        public override MacroDataSource GetMacroDataSource()
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

            public override string Daedra()
            {
                FactionFile.FactionData factionData;
                if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(parent.daedraToSummon.factionId, out factionData))
                    return factionData.name;
                else
                    return "%dae[error]";
            }
        }

        #endregion

    }
}
