// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:
//
// Notes:
// Format:
//
//     Multiline Text Label (optional)
//      :::                 :::
//      :::TextPanelDistance:::
//      :::                 :::
//     Text Label (prompt):::InputDistance:::Text Input Box

using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;
using UnityEngine;
using System;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// General purpose pop-up window for displaying a message with an input text box.
    /// </summary>
    public class DaggerfallInputMessageBox : DaggerfallPopupWindow
    {
        private Panel messagePanel = new Panel();
        private Panel textPanel = new Panel();
        private MultiFormatTextLabel multiLineLabel = new MultiFormatTextLabel();
        private TextBox textBox = new TextBox();
        private TextLabel textBoxLabel = new TextLabel();
        private Color parentPanelColor = Color.clear;

        private int textPanelDistanceX = 0;             //horizontal distance between the text prompt / input & the multiline label
        private int textPanelDistanceY = 12;            //vertical distance between the text prompt / input & the multiline label  
        private int inputDistanceX = 0;                 //horizontal distance between the input label & input box
        private int inputDistanceY = 0;                 //vertical distance between the input label & input box
        private bool useParchmentStyle = true;          //if true, box will use PopupStyle Parchment background
        private bool clickAnywhereToClose = false;
        private bool showAtTopOfScreen = false;

        public bool ClickAnywhereToClose
        {
            get { return clickAnywhereToClose; }
            set { clickAnywhereToClose = value; }
        }

        public int TextPanelDistanceY
        {
            get { return textPanelDistanceY; }
            set { textPanelDistanceY = value; }
        }

        public int TextPanelDistanceX
        {
            get { return textPanelDistanceX; }
            set { textPanelDistanceX = value; }
        }

        public int InputDistanceX
        {
            get { return inputDistanceX; }
            set { inputDistanceX = value; }
        }

        public int InputDistanceY
        {
            get { return inputDistanceY; }
            set { inputDistanceY = value; }
        }

        public TextBox TextBox
        {
            get { return textBox; }
        }

        //public int MinWidth
        //{
        //    get { return minWidth; }
        //    set { minWidth = value; }
        //}

        public Color ParentPanelColor
        {
            get { return parentPanelColor; }
            set { parentPanelColor = value; this.ParentPanel.BackgroundColor = value; }
        }

        public DaggerfallInputMessageBox(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        public DaggerfallInputMessageBox(
            IUserInterfaceManager uiManager,
            int textId,
            int maxCharacters = 31,
            string textBoxLabel = null,
            bool useParchmentBackGround = true,
            bool showAtTopOfScreen = false,
            UserInterfaceWindow previous = null)
            : base(uiManager, previous)
        {
            this.textBox.MaxCharacters = maxCharacters;
            this.useParchmentStyle = useParchmentBackGround;
            this.SetTextBoxLabel(textBoxLabel);
            this.showAtTopOfScreen = showAtTopOfScreen;
            SetupBox(textId);
        }

        public DaggerfallInputMessageBox(
            IUserInterfaceManager uiManager,
            TextFile.Token[] textTokens,
            int maxCharacters = 31,
            string textBoxLabel = null,
            bool useParchmentBackGround = true,
            UserInterfaceWindow previous = null)
            : base(uiManager, previous)
        {
            this.textBox.MaxCharacters = maxCharacters;
            this.useParchmentStyle = useParchmentBackGround;
            this.SetTextBoxLabel(textBoxLabel);
            SetupBox(textTokens);
        }

        protected override void Setup()
        {
            base.Setup();

            allowFreeScaling = false;

            if (useParchmentStyle)
                DaggerfallUI.Instance.SetDaggerfallPopupStyle(DaggerfallUI.PopupStyle.Parchment, messagePanel);

            messagePanel.HorizontalAlignment = HorizontalAlignment.Center;
            if (showAtTopOfScreen)
                messagePanel.VerticalAlignment = VerticalAlignment.Top;
            else
                messagePanel.VerticalAlignment = VerticalAlignment.Middle;
            messagePanel.OnMouseClick += MessagePanel_OnMouseClick;
            NativePanel.Components.Add(messagePanel);

            multiLineLabel.HorizontalAlignment = HorizontalAlignment.Center;
            if (showAtTopOfScreen)
                multiLineLabel.VerticalAlignment = VerticalAlignment.Top;
            else
                multiLineLabel.VerticalAlignment = VerticalAlignment.Middle;
            messagePanel.Components.Add(multiLineLabel);

            messagePanel.Components.Add(textPanel);
            textPanel.Components.Add(textBoxLabel);
            textPanel.Components.Add(textBox);
            ParentPanel.BackgroundColor = ParentPanelColor;
            UpdatePanelSizes();
        }

        public void Show()
        {
            uiManager.PushWindow(this);
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ReturnPlayerInputEvent(this, textBox.Text);
            }
            //else if (Input.GetKeyDown(exitKey))
            // {
            //  ReturnPlayerInputEvent(this, string.Empty);
            //}
        }

        private void SetupBox(int textId)
        {
            SetTextTokens(textId);
        }

        private void SetupBox(TextFile.Token[] tokens)
        {
            if (tokens != null)
                SetTextTokens(tokens);
        }

        public void SetTextTokens(TextFile.Token[] tokens, IMacroContextProvider mcp = null)
        {
            MacroHelper.ExpandMacros(ref tokens, mcp);
            multiLineLabel.SetText(tokens);
        }

        public void SetTextTokens(int id, IMacroContextProvider mcp = null)
        {

            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(id);
            SetTextTokens(tokens, mcp);
        }

        public void SetTextBoxLabel(string label)
        {
            if (label == null)
                return;
            textBoxLabel.Text = label;
        }

        private void UpdatePanelSizes()
        {
            int minimum = 44;

            float width = textBox.WidthOverride;
            if (width <= 0)
                width = (Mathf.Max(multiLineLabel.Size.x + messagePanel.LeftMargin + messagePanel.RightMargin, (textBoxLabel.Size.x + inputDistanceX + textBox.MaxSize.x)));

            if (width > minimum)
                width = (float)Math.Ceiling(width / 22) * 22;
            else
                width = minimum;

            float height = (messagePanel.TopMargin + multiLineLabel.Size.y + textPanelDistanceY + messagePanel.BottomMargin);

            if (height > minimum)
                height = (float)Math.Ceiling(height / 22) * 22;
            else
                height = minimum;

            messagePanel.Size = new Vector2(width, height);
            textBoxLabel.Position = new Vector2(textPanelDistanceX, multiLineLabel.Position.y + multiLineLabel.Size.y + textPanelDistanceY);
            textBox.Position = new Vector2(textBoxLabel.Position.x + textBoxLabel.Size.x + inputDistanceX, textBoxLabel.Position.y + inputDistanceY);
        }

        private void MessagePanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (clickAnywhereToClose)
            {
                ReturnPlayerInputEvent(this, textBox.Text);
            }
        }

        /// <summary>
        /// Handles event from textbox on user finishing entering input.
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="text"></param>
        public void textBox_OnAcceptUserInputHandler(TextBox textBox, string text)
        {
            ReturnPlayerInputEvent(this, text);
        }

        /// <summary>
        /// Signature for listening to the input received by a <see cref="DaggerfallInputMessageBox"/>.
        /// </summary>
        /// <param name="sender">The instance that received the input.</param>
        /// <param name="input">Content of the input text box.</param>
        public delegate void OnReturnPlayerInputHandler(DaggerfallInputMessageBox sender, string input);

        /// <summary>
        /// Raised when the <see cref="DaggerfallInputMessageBox"/> window is closed.
        /// </summary>
        public event OnReturnPlayerInputHandler OnGotUserInput;

        private void ReturnPlayerInputEvent(DaggerfallInputMessageBox sender, string userInput)
        {
            CloseWindow();
            DaggerfallUI.Instance.timeClosedInputMessageBox = Time.realtimeSinceStartup;
            if (OnGotUserInput != null)
                OnGotUserInput(sender, userInput);
        }
    }
}