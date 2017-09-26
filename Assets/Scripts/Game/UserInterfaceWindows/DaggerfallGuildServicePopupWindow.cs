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
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallConnect;

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

        #region UI Textures

        Texture2D baseTexture;

        #endregion

        #region Fields

        const string baseTextureName = "GILD00I0.IMG";      // Join Guild / Talk / Service
        GuildServices currentService;

        StaticNPC merchantNPC;

        #endregion

        #region Properties

        public StaticNPC MerchantNPC
        {
            get { return merchantNPC; }
            set { merchantNPC = value; }
        }

        #endregion

        #region Constructors

        public DaggerfallGuildServicePopupWindow(IUserInterfaceManager uiManager, GuildServices service)
            : base(uiManager)
        {
            currentService = service;
            // Clear background
            ParentPanel.BackgroundColor = Color.clear;
        }

        #endregion

        #region Setup Methods

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
            switch (currentService)
            {
                default:
                case GuildServices.MG_Identify:
                    return HardStrings.serviceIdentify;
                case GuildServices.MG_Buy_Spells:
                    return HardStrings.serviceBuySpells;
            }
        }

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName);
        }

        #endregion

        #region Event Handlers

        private void TalkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            GameManager.Instance.TalkManager.TalkToStaticNPC(merchantNPC);
        }

        private void ServiceButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
            switch (currentService)
            {
                default:
                case GuildServices.MG_Identify:
                    uiManager.PushWindow(new DaggerfallTradeWindow(uiManager, DaggerfallTradeWindow.WindowModes.Identify, this));
                    break;
                case GuildServices.MG_Buy_Spells:
                    //uiManager.PushWindow(new DaggerfallBankingWindow(uiManager, this));
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