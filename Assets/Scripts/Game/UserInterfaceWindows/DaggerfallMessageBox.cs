﻿// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements a Daggerfal popup message box dialog with variable buttons.
    /// Designed to take Daggerfall multiline text resource records.
    /// </summary>
    public class DaggerfallMessageBox : DaggerfallPopupWindow
    {
        const string buttonsFilename = "BUTTONS.RCI";

        Panel messagePanel = new Panel();
        Panel buttonPanel = new Panel();
        MultiFormatTextLabel label = new MultiFormatTextLabel();
        List<Button> buttons = new List<Button>();
        int buttonSpacing = 32;
        int buttonTextDistance = 4;
        MessageBoxButtons selectedButton = MessageBoxButtons.Cancel;
        bool clickAnywhereToClose = false;

        /// <summary>
        /// Default message box buttons are indices into BUTTONS.RCI.
        /// </summary>
        public enum MessageBoxButtons
        {
            Accept = 0,
            Reject = 1,
            Cancel = 2,
            Yes = 3,
            No = 4,
            OK = 5,
            Male = 6,
            Female = 7,
            Add = 8,
            Delete = 9,
            Edit = 10,
            Counter = 11,
            _12MON = 12,
            _36MON = 13,
            Copy = 14,
            Guilty = 15,
            NotGuilty = 16,
            Debate = 17,
            Lie = 18,
            Anchor = 19,
            Teleport = 20,
        }

        public enum CommonMessageBoxButtons
        {
            Nothing,
            YesNo,
        }

        public int ButtonSpacing
        {
            get { return buttonSpacing; }
            set { buttonSpacing = value; }
        }

        public int ButtonTextDistance
        {
            get { return buttonTextDistance; }
            set { buttonTextDistance = value; }
        }

        public MultiFormatTextLabel Label
        {
            get { return label; }
            set { label = value; }
        }

        public MessageBoxButtons SelectedButton
        {
            get { return selectedButton; }
        }

        public bool ClickAnywhereToClose
        {
            get { return clickAnywhereToClose; }
            set { clickAnywhereToClose = value; }
        }

        public DaggerfallMessageBox(IUserInterfaceManager uiManager, IUserInterfaceWindow previous = null)
            : base(uiManager, previous)
        {
        }

        public DaggerfallMessageBox(IUserInterfaceManager uiManager, CommonMessageBoxButtons buttons, TextFile.Token[] tokens, IUserInterfaceWindow previous = null)
            : base(uiManager, previous)
        {
            SetupBox(tokens, buttons);
        }

        public DaggerfallMessageBox(IUserInterfaceManager uiManager, CommonMessageBoxButtons buttons, int textId, IUserInterfaceWindow previous = null)
            : base(uiManager, previous)
        {
            SetupBox(textId, buttons);
        }

        public DaggerfallMessageBox(IUserInterfaceManager uiManager, CommonMessageBoxButtons buttons, string text, IUserInterfaceWindow previous = null)
            : base(uiManager, previous)
        {
            SetupBox(text, buttons);
        }

        protected override void Setup()
        {
            if (IsSetup)
                return;

            base.Setup();

            messagePanel.HorizontalAlignment = HorizontalAlignment.Center;
            messagePanel.VerticalAlignment = VerticalAlignment.Middle;
            DaggerfallUI.Instance.SetDaggerfallPopupStyle(DaggerfallUI.PopupStyle.Parchment, messagePanel);
            NativePanel.Components.Add(messagePanel);

            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Middle;
            messagePanel.Components.Add(label);

            buttonPanel.HorizontalAlignment = HorizontalAlignment.Center;
            buttonPanel.VerticalAlignment = VerticalAlignment.None;
            messagePanel.Components.Add(buttonPanel);

            IsSetup = true;
        }

        public override void OnPush()
        {
            base.OnPush();
            parentPanel.OnMouseClick += ParentPanel_OnMouseClick;
        }

        public override void OnPop()
        {
            base.OnPop();
            parentPanel.OnMouseClick -= ParentPanel_OnMouseClick;
        }

        #region Public Methods

        public void Show()
        {
            if (!IsSetup)
                Setup();

            uiManager.PushWindow(this);
        }

        public override void Update()
        {
            base.Update();
        
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
              CloseWindow();
        }

        public Button AddButton(MessageBoxButtons messageBoxButton)
        {
            if (!IsSetup)
                Setup();

            Texture2D background = DaggerfallUI.GetTextureFromCifRci(buttonsFilename, (int)messageBoxButton);
            Button button = DaggerfallUI.AddButton(Vector2.zero, 
                TextureReplacement.GetSizeFromTexture(background, buttonsFilename, (int)messageBoxButton), buttonPanel);
            button.BackgroundTexture = background;
            button.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
            button.Tag = messageBoxButton;
            button.OnMouseClick += ButtonClickHandler;
            buttons.Add(button);

            UpdatePanelSizes();

            return button;
        }

        public void SetText(params string[] rows)
        {
            // Tokenize rows
            List<TextFile.Token> tokenList = new List<TextFile.Token>();
            TextFile.Token textToken = new TextFile.Token();
            TextFile.Token newLineToken = new TextFile.Token();
            for (int i = 0; i < rows.Length; i++)
            {
                textToken.formatting = TextFile.Formatting.Text;
                textToken.text = rows[i];
                tokenList.Add(textToken);
                if (i < rows.Length - 1)
                {
                    newLineToken.formatting = TextFile.Formatting.NewLine;
                    tokenList.Add(newLineToken);
                }
            }

            // Set tokens
            SetTextTokens(tokenList.ToArray());
        }

        public void SetTextTokens(TextFile.Token[] tokens)
        {
            if (!IsSetup)
                Setup();

            label.SetText(tokens);
            UpdatePanelSizes();
        }

        public void SetTextTokens(int id)
        {
            if (!IsSetup)
                Setup();

            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(id);
            SetTextTokens(tokens);
        }

        #endregion

        #region Private Methods

        void ButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            selectedButton = (MessageBoxButtons)sender.Tag;
            RaiseOnButtonClickEvent(this, selectedButton);
        }

        void UpdatePanelSizes()
        {
            Vector2 finalSize = new Vector2();
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Position = new Vector2(finalSize.x, 0);

                Vector2 buttonSize = buttons[i].Size;
                finalSize.x += buttonSize.x;

                if (buttonSize.y > finalSize.y)
                    finalSize.y = buttonSize.y;

                if (i < buttons.Count - 1)
                    finalSize.x += buttonSpacing;
            }

            // If buttons have been added, resize label text by adding in the height of the finalized button panel.
            if (finalSize.y - buttonPanel.Size.y > 0)
                label.ResizeY(label.Size.y + (finalSize.y - buttonPanel.Size.y) + buttonTextDistance);

            buttonPanel.Size = finalSize;

            // Position buttons to be buttonTextDistance pixels below the repositioned text
            if (buttons.Count > 0)
            {
                float buttonY = messagePanel.Size.y - ((messagePanel.Size.y - label.Size.y) / 2) - buttonPanel.Size.y - messagePanel.BottomMargin;
                buttonPanel.Position = new Vector2(buttonPanel.Position.x, buttonY);
            }

            // Resize the message panel to get a clean border of 22x22 pixel textures
            int minimum = 44;
            float width = label.Size.x + messagePanel.LeftMargin + messagePanel.RightMargin;
            float height = label.Size.y + messagePanel.TopMargin + messagePanel.BottomMargin;

            if (width > minimum)
                width = (float)Math.Ceiling(width / 22) * 22;
            else
                width = minimum;

            if (height > minimum)
                height = (float)Math.Ceiling(height / 22) * 22;
            else
                height = minimum;

            messagePanel.Size = new Vector2(width, height);
        }

        void SetupBox(TextFile.Token[] tokens, CommonMessageBoxButtons buttons)
        {
            SetTextTokens(tokens);
            switch (buttons)
            {
                case CommonMessageBoxButtons.YesNo:
                    AddButton(MessageBoxButtons.Yes);
                    AddButton(MessageBoxButtons.No);
                    break;
            }
        }

        void SetupBox(int textId, CommonMessageBoxButtons buttons)
        {
            SetTextTokens(textId);
            switch (buttons)
            {
                case CommonMessageBoxButtons.YesNo:
                    AddButton(MessageBoxButtons.Yes);
                    AddButton(MessageBoxButtons.No);
                    break;
            }
        }

        void SetupBox(string text, CommonMessageBoxButtons buttons)
        {
            SetText(text);
            switch (buttons)
            {
                case CommonMessageBoxButtons.YesNo:
                    AddButton(MessageBoxButtons.Yes);
                    AddButton(MessageBoxButtons.No);
                    break;
            }
        }

        #endregion

        #region Event Handlers

        private void ParentPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (uiManager.TopWindow == this && clickAnywhereToClose)
                CloseWindow();
        }

        #endregion

        #region Events

        // OnButtonClick
        public delegate void OnButtonClickHandler(DaggerfallMessageBox sender, MessageBoxButtons messageBoxButton);
        public event OnButtonClickHandler OnButtonClick;
        void RaiseOnButtonClickEvent(DaggerfallMessageBox sender, MessageBoxButtons messageBoxButton)
        {
            if (OnButtonClick != null)
                OnButtonClick(sender, messageBoxButton);
        }

        #endregion
    }
}