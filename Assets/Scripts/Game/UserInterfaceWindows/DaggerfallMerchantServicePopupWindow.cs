// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors: Numidium
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallMerchantServicePopupWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        Rect talkButtonRect = new Rect(5, 5, 120, 7);
        Rect serviceButtonRect = new Rect(5, 14, 120, 7);
        Rect exitButtonRect = new Rect(44, 24, 43, 15);

        #endregion

        #region UI Controls

        Panel mainPanel = new Panel();
        Button talkButton = new Button();
        Button serviceButton = new Button();
        Button exitButton = new Button();
        TextLabel serviceLabel = new TextLabel();

        #endregion

        #region Fields

        const string baseTextureName = "GNRC01I0.IMG";      // Talk / Sell
        Texture2D baseTexture;

        Services currentService;
        StaticNPC merchantNPC;

        #endregion

        #region Enums

        /// <summary>
        /// Supported services.
        /// </summary>
        public enum Services
        {
            Sell,
            Banking,
        }

        #endregion

        #region Constructors

        public DaggerfallMerchantServicePopupWindow(IUserInterfaceManager uiManager, StaticNPC npc, Services service)
            : base(uiManager)
        {
            merchantNPC = npc;
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
            mainPanel.Size = new Vector2(130, 42);

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
            if (Guilds.Services.HasCustomMerchantService(merchantNPC.Data.factionID))
                return Guilds.Services.GetCustomMerchantServiceLabel(merchantNPC.Data.factionID);

            switch (currentService)
            {
                default:
                case Services.Sell:
                    return HardStrings.serviceSell;
                case Services.Banking:
                    return HardStrings.serviceBanking;
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
            CloseWindow();
            GameManager.Instance.TalkManager.TalkToStaticNPC(merchantNPC);
        }

        private void ServiceButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();

            // Use a registered custom service method
            Guilds.Services.CustomMerchantService customMerchantService;
            if (Guilds.Services.GetCustomMerchantService(merchantNPC.Data.factionID, out customMerchantService))
                customMerchantService(this);
            else
            {
                switch (currentService)
                {
                    default:
                    case Services.Sell:
                        uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.Trade, new object[] { uiManager, this, DaggerfallTradeWindow.WindowModes.Sell, null }));
                        break;
                    case Services.Banking:
                        uiManager.PushWindow(UIWindowFactory.GetInstance(UIWindowType.Banking, uiManager, this));
                        break;
                }
            }
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion
    }
}