// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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

        Rect repairButtonRect = new Rect(5, 5, 120, 7);
        Rect talkButtonRect = new Rect(5, 14, 120, 7);
        Rect sellButtonRect = new Rect(5, 23, 120, 7);
        Rect exitButtonRect = new Rect(44, 33, 43, 15);

        #endregion

        #region UI Controls

        protected Panel mainPanel = new Panel();
        protected Button repairButton = new Button();
        protected Button talkButton = new Button();
        protected Button sellButton = new Button();
        protected Button exitButton = new Button();

        #endregion

        #region Fields

        const string baseTextureName = "REPR01I0.IMG";      // Repair / Talk / Sell
        protected Texture2D baseTexture;

        readonly StaticNPC merchantNPC;

        bool isCloseWindowDeferred = false;
        bool isRepairWindowDeferred = false;
        bool isTalkWindowDeferred = false;
        bool isSellWindowDeferred = false;

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
            repairButton.OnKeyboardEvent += RepairButton_OnKeyboardEvent;

            // Talk button
            talkButton = DaggerfallUI.AddButton(talkButtonRect, mainPanel);
            talkButton.OnMouseClick += TalkButton_OnMouseClick;
            talkButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.MerchantTalk);
            talkButton.OnKeyboardEvent += TalkButton_OnKeyboardEvent;

            // Sell button
            sellButton = DaggerfallUI.AddButton(sellButtonRect, mainPanel);
            sellButton.OnMouseClick += SellButton_OnMouseClick;
            sellButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.MerchantSell);
            sellButton.OnKeyboardEvent += SellButton_OnKeyboardEvent;

            // Exit button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, mainPanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.MerchantExit);
            exitButton.OnKeyboardEvent += ExitButton_OnKeyboardEvent;

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

        void RepairButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isRepairWindowDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isRepairWindowDeferred)
            {
                isRepairWindowDeferred = false;
                CloseWindow();
                uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.Trade, new object[] { uiManager, this, DaggerfallTradeWindow.WindowModes.Repair, null }));
            }
        }

        protected virtual void TalkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseWindow();
            GameManager.Instance.TalkManager.TalkToStaticNPC(merchantNPC);
        }

        void TalkButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isTalkWindowDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isTalkWindowDeferred)
            {
                isTalkWindowDeferred = true;
                CloseWindow();
                GameManager.Instance.TalkManager.TalkToStaticNPC(merchantNPC);
            }
        }

        protected virtual void SellButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            CloseWindow();
            uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.Trade, new object[] { uiManager, this, DaggerfallTradeWindow.WindowModes.Sell, null }));
        }

        void SellButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isSellWindowDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isSellWindowDeferred)
            {
                isSellWindowDeferred = false;
                CloseWindow();
                uiManager.PushWindow(UIWindowFactory.GetInstanceWithArgs(UIWindowType.Trade, new object[] { uiManager, this, DaggerfallTradeWindow.WindowModes.Sell, null }));
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
    }
}
