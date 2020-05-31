// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallMerchantRepairPopupWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        protected Rect repairButtonRect = new Rect(5, 5, 120, 7);
        protected Rect talkButtonRect = new Rect(5, 14, 120, 7);
        protected Rect sellButtonRect = new Rect(5, 23, 120, 7);
        protected Rect exitButtonRect = new Rect(44, 33, 43, 15);

        #endregion

        #region UI Controls

        protected Panel mainPanel = new Panel();
        protected Button repairButton = new Button();
        protected Button talkButton = new Button();
        protected Button sellButton = new Button();
        protected Button exitButton = new Button();

        #endregion

        #region Fields

        protected const string baseTextureName = "REPR01I0.IMG";      // Repair / Talk / Sell
        protected Texture2D baseTexture;

        protected StaticNPC merchantNPC;

        #endregion

        #region Constructors

        public DaggerfallMerchantRepairPopupWindow(IUserInterfaceManager uiManager, StaticNPC npc)
            : base(uiManager)
        {
            merchantNPC = npc;
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
            mainPanel.Size = new Vector2(130, 51);

            // Repair button
            repairButton = DaggerfallUI.AddButton(repairButtonRect, mainPanel);
            repairButton.OnMouseClick += RepairButton_OnMouseClick;
            repairButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.MerchantRepair);

            // Talk button
            talkButton = DaggerfallUI.AddButton(talkButtonRect, mainPanel);
            talkButton.OnMouseClick += TalkButton_OnMouseClick;
            talkButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.MerchantTalk);

            // Sell button
            sellButton = DaggerfallUI.AddButton(sellButtonRect, mainPanel);
            sellButton.OnMouseClick += SellButton_OnMouseClick;
            sellButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.MerchantSell);

            // Exit button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, mainPanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.MerchantExit);

            NativePanel.Components.Add(mainPanel);
        }

        #endregion

        #region Private Methods

        protected virtual void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName);
        }

        #endregion

        #region Event Handlers

        protected virtual void RepairButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseWindow();
            uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.Trade, new object[] { uiManager, this, DaggerfallTradeWindow.WindowModes.Repair, null }));
        }

        protected virtual void TalkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseWindow();
            GameManager.Instance.TalkManager.TalkToStaticNPC(merchantNPC);
        }

        protected virtual void SellButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseWindow();
            uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.Trade, new object[] { uiManager, this, DaggerfallTradeWindow.WindowModes.Sell, null }));
        }

        protected virtual void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseWindow();
        }

        #endregion
    }
}
