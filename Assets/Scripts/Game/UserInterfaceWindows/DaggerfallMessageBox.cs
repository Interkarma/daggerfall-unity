// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements a Daggerfal popup message box dialog with variable buttons.
    /// Designed to take Daggerfall multiline text resource records.
    /// </summary>
    public class DaggerfallMessageBox : DaggerfallPopupWindow
    {
        const string buttonsFilename = "BUTTONS.RCI";
        const float minTimePresented = 0.0833f;
        const int minBoxWidth = 132;

        Panel imagePanel = new Panel();
        Panel messagePanel = new Panel();
        Panel buttonPanel = new Panel();
        MultiFormatTextLabel label = new MultiFormatTextLabel();
        List<Button> buttons = new List<Button>();
        int buttonSpacing = 32;
        int buttonTextDistance = 4;
        MessageBoxButtons selectedButton = MessageBoxButtons.Cancel;
        bool clickAnywhereToClose = false;
        DaggerfallMessageBox nextMessageBox;
        int customYPos = -1;
        float presentationTime = 0;

        KeyCode extraProceedBinding = KeyCode.None;

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
            AnchorTeleport,
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

        public MessageBoxButtons SelectedButton
        {
            get { return selectedButton; }
        }

        public bool ClickAnywhereToClose
        {
            get { return clickAnywhereToClose; }
            set { clickAnywhereToClose = value; }
        }

        public KeyCode ExtraProceedBinding
        {
            get { return extraProceedBinding; }
            set { extraProceedBinding = value; }
        }

        public Panel ImagePanel
        {
            get { return imagePanel; }
        }

        public DaggerfallMessageBox(IUserInterfaceManager uiManager, IUserInterfaceWindow previous = null, bool wrapText = false, int posY = -1)
            : base(uiManager, previous)
        {
            if (wrapText)
            {
                label.WrapText = label.WrapWords = true;
                // If wrapping text, set maxWidth to 288. This is just an aesthetically chosen value, as
                // it is the widest text can be without making the parchment textures expand off the edges of the screen.
                label.MaxTextWidth = 288;
            }

            if (posY > -1)
                customYPos = posY;
        }

        public DaggerfallMessageBox(IUserInterfaceManager uiManager, CommonMessageBoxButtons buttons, TextFile.Token[] tokens, IUserInterfaceWindow previous = null, IMacroContextProvider mcp = null)
            : base(uiManager, previous)
        {
            SetupBox(tokens, buttons, mcp);
        }

        public DaggerfallMessageBox(IUserInterfaceManager uiManager, CommonMessageBoxButtons buttons, int textId, IUserInterfaceWindow previous = null, IMacroContextProvider mcp = null)
            : base(uiManager, previous)
        {
            SetupBox(textId, buttons, mcp);
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

            if (customYPos > -1)
            {
                messagePanel.VerticalAlignment = VerticalAlignment.None;
                messagePanel.Position = new Vector2(messagePanel.Position.x, customYPos);
            }
            else
                messagePanel.VerticalAlignment = VerticalAlignment.Middle;

            DaggerfallUI.Instance.SetDaggerfallPopupStyle(DaggerfallUI.PopupStyle.Parchment, messagePanel);
            NativePanel.Components.Add(messagePanel);

            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Middle;
            messagePanel.Components.Add(label);

            buttonPanel.HorizontalAlignment = HorizontalAlignment.Center;
            buttonPanel.VerticalAlignment = VerticalAlignment.None;
            messagePanel.Components.Add(buttonPanel);

            imagePanel.HorizontalAlignment = HorizontalAlignment.Center;
            imagePanel.VerticalAlignment = VerticalAlignment.Top;
            messagePanel.Components.Add(imagePanel);

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

            // Check if any previous message boxes need to be closed as well.
            DaggerfallMessageBox prevWindow = PreviousWindow as DaggerfallMessageBox;
            if (prevWindow != null && prevWindow.nextMessageBox != null)
            {
                prevWindow.nextMessageBox.CloseWindow();
            }
        }

        #region Public Methods

        /// <summary>
        /// Adds another nested message box to be displayed next when click detected.
        /// </summary>
        /// <param name="nextMessageBox">Next message box.</param>
        public void AddNextMessageBox(DaggerfallMessageBox nextMessageBox)
        {
            this.nextMessageBox = nextMessageBox;
        }

        public void Show()
        {
            if (!IsSetup)
                Setup();

            uiManager.PushWindow(this);
            presentationTime = Time.realtimeSinceStartup;
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyUp(extraProceedBinding))
            {
                // Special handling for message boxes with buttons
                if (buttons.Count > 0)
                {
                    // Trigger default button if one is present
                    Button defaultButton = GetDefaultButton();
                    if (defaultButton != null)
                        defaultButton.TriggerMouseClick();

                    // Exit here if no other message boxes queued
                    // Most of the time this won't be the case and we don't want message boxes waiting for input to close prematurely
                    if (nextMessageBox == null)
                        return;
                }

                // if there is a nested next message box show it
                if (this.nextMessageBox != null)
                {
                    nextMessageBox.Show();
                }
                else // or close window if there is no next message box to show
                {
                    CloseWindow();                    
                }
            }
        }

        public Button AddButton(MessageBoxButtons messageBoxButton, bool defaultButton = false)
        {
            if (!IsSetup)
                Setup();

            // If this is to become default button, first unset any other default buttons
            // Only one button in collection can be default
            if (defaultButton)
            {
                foreach (Button b in buttons)
                    b.DefaultButton = false;
            }

            Texture2D background = DaggerfallUI.GetTextureFromCifRci(buttonsFilename, (int)messageBoxButton);
            Button button = DaggerfallUI.AddButton(Vector2.zero, 
                TextureReplacement.GetSize(background, buttonsFilename, (int)messageBoxButton), buttonPanel);
            button.BackgroundTexture = background;
            button.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
            button.Tag = messageBoxButton;
            button.OnMouseClick += ButtonClickHandler;
            button.DefaultButton = defaultButton;
            buttons.Add(button);

            // Once a button has been added the owner is expecting some kind of input from player
            // Don't allow a messagebox with buttons to be cancelled with escape
            AllowCancel = false;

            UpdatePanelSizes();

            return button;
        }

        /// <summary>
        /// Gets default button (if any).
        /// </summary>
        /// <returns>Default Button reference, or null if no default button defined.</returns>
        public Button GetDefaultButton()
        {
            foreach (Button b in buttons)
            {
                if (b.DefaultButton)
                    return b;
            }

            return null;
        }

        public void SetText(string text, IMacroContextProvider mcp = null)
        {
            SetText(new string[] { text }, mcp);
        }

        public void SetText(string[] rows, IMacroContextProvider mcp = null)
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
            SetTextTokens(tokenList.ToArray(), mcp);
        }

        public void SetTextTokens(TextFile.Token[] tokens, IMacroContextProvider mcp = null, bool expandMacros = true)
        {
            if (!IsSetup)
                Setup();

            if (expandMacros)
                MacroHelper.ExpandMacros(ref tokens, mcp);
            label.SetText(tokens);
            UpdatePanelSizes();
        }

        public void SetTextTokens(int id, IMacroContextProvider mcp = null)
        {
            if (!IsSetup)
                Setup();

            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(id);
            SetTextTokens(tokens, mcp);
        }

        /// <summary>
        /// Must be set before text otherwise layout has already occurred.
        /// </summary>
        public void SetHighlightColor(Color highlightColor)
        {
            label.HighlightColor = highlightColor;
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

            // Enforce a minimum size
            if (width < minBoxWidth)
                width = minBoxWidth;

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

        void SetupBox(TextFile.Token[] tokens, CommonMessageBoxButtons buttons, IMacroContextProvider mcp = null)
        {
            SetTextTokens(tokens, mcp);
            AddCommonButtons(buttons);
        }

        void SetupBox(int textId, CommonMessageBoxButtons buttons, IMacroContextProvider mcp = null)
        {
            SetTextTokens(textId, mcp);
            AddCommonButtons(buttons);
        }

        void SetupBox(string text, CommonMessageBoxButtons buttons)
        {
            SetText(text);
            AddCommonButtons(buttons);
        }

        void AddCommonButtons(CommonMessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case CommonMessageBoxButtons.YesNo:
                    AddButton(MessageBoxButtons.Yes);
                    AddButton(MessageBoxButtons.No, true);
                    break;
                case CommonMessageBoxButtons.AnchorTeleport:
                    AddButton(MessageBoxButtons.Anchor);
                    AddButton(MessageBoxButtons.Teleport);
                    break;
            }
        }

        #endregion

        #region Event Handlers

        private void ParentPanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Must be presented for minimum time before allowing to click through
            // This prevents capturing parent-level click events and closing immediately
            if (Time.realtimeSinceStartup - presentationTime < minTimePresented)
                return;

            if (uiManager.TopWindow == this)
            {
                if (nextMessageBox != null)
                    nextMessageBox.Show();
                else if (clickAnywhereToClose)
                    CloseWindow();
            }
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