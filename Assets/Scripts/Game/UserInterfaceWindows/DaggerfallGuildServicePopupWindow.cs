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

        // Temples:
        Make_Donation = 810,
        Cure_Diseases = 813,
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

        const string baseTextureName = "GILD00I0.IMG";      // Join Guild / Talk / Service
        Texture2D baseTexture;

        StaticNPC serviceNPC;
        FactionFile.GuildGroups guild;
        GuildServices service;

        static ItemCollection merchantItems;

        #endregion

        #region Constructors

        public DaggerfallGuildServicePopupWindow(IUserInterfaceManager uiManager, StaticNPC npc, FactionFile.GuildGroups guild, GuildServices service)
            : base(uiManager)
        {
            serviceNPC = npc;
            this.guild = guild;
            this.service = service;
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
                items.AddItem(ItemBuilder.CreateRandomArmor(playerEntity.Level, playerEntity.Gender, playerEntity.Race));
                items.AddItem(ItemBuilder.CreateRandomBook());
                items.AddItem(ItemBuilder.CreateRandomClothing(playerEntity.Gender));
                items.AddItem(ItemBuilder.CreateRandomClothing(playerEntity.Gender));
                items.AddItem(ItemBuilder.CreateRandomClothing(playerEntity.Gender));
                items.AddItem(ItemBuilder.CreateRandomClothing(playerEntity.Gender));
                items.AddItem(ItemBuilder.CreateRandomIngredient());
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
                    return HardStrings.serviceTraining;
                case GuildServices.FG_Repairs:
                    return HardStrings.serviceRepairs;
                case GuildServices.FG_Training:
                    return HardStrings.serviceTraining;
                case GuildServices.Cure_Diseases:
                    return HardStrings.serviceCure;
                case GuildServices.Make_Donation:
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
    }
}