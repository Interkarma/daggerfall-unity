// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyl@dfworkshop.net)
// Contributors:
//
// Notes:
//

using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// General purpose pop-up window for displaying a message w/ an input text box
    /// </summary>
    public class DaggerfallInputMessageBox : DaggerfallPopupWindow
    {
        private Panel messagePanel = new Panel();
        private Panel textPanel = new Panel();
        private MultiFormatTextLabel label = new MultiFormatTextLabel();
        private TextBox textBox = new TextBox();
        private TextLabel textBoxLabel = new TextLabel();
        private Color parentPanelColor = Color.clear;

        private int textPanelDistance = 12;             //distance between the text panel & the top label
        private int inputDistance = 4;                  //distance between the input label & input box
        private int minWidth = 160;
        private bool useParchmentStyle = true;          //if true, box will use PopupStyle Parchment background
        private bool maxFit = true;                     //size panel to fit at least max # charcters; assumes all widest glyphs;
        private bool clickAnywhereToClose = false;

        public bool ClickAnywhereToClose
        {
            get { return clickAnywhereToClose; }
            set { clickAnywhereToClose = value; }
        }

        public int TextPanelDistance
        {
            get { return textPanelDistance; }
            set { textPanelDistance = value; }
        }

        public int InputDistance
        {
            get { return inputDistance; }
            set { inputDistance = value; }
        }

        public int MinWidth
        {
            get { return minWidth; }
            set { minWidth = value; }
        }

        public bool MaxFit
        {
            get { return maxFit; }
            set { maxFit = value; }
        }

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
            bool maxFit = true,
            bool useParchmentBackGround = true,
            UserInterfaceWindow previous = null)
            : base(uiManager, previous)
        {
            this.textBox.MaxCharacters = maxCharacters;
            this.MaxFit = maxFit;
            this.useParchmentStyle = useParchmentBackGround;
            this.SetTextBoxLabel(textBoxLabel);
            SetupBox(textId);
        }

        public DaggerfallInputMessageBox(
            IUserInterfaceManager uiManager,
            TextFile.Token[] textTokens,
            int maxCharacters = 31,
            string textBoxLabel = null,
            bool maxFit = true,
            bool useParchmentBackGround = true,
            UserInterfaceWindow previous = null)
            : base(uiManager, previous)
        {
            this.textBox.MaxCharacters = maxCharacters;
            this.MaxFit = maxFit;
            this.useParchmentStyle = useParchmentBackGround;
            this.SetTextBoxLabel(textBoxLabel);
            SetupBox(textTokens);
        }

        protected override void Setup()
        {
            if (IsSetup)
                return;

            base.Setup();

            if (useParchmentStyle)
                DaggerfallUI.Instance.SetDaggerfallPopupStyle(DaggerfallUI.PopupStyle.Parchment, messagePanel);
            messagePanel.HorizontalAlignment = HorizontalAlignment.Center;
            messagePanel.VerticalAlignment = VerticalAlignment.Middle;
            messagePanel.OnMouseClick += MessagePanel_OnMouseClick;
            NativePanel.Components.Add(messagePanel);

            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Top;
            messagePanel.Components.Add(label);

            messagePanel.Components.Add(textPanel);
            textPanel.Components.Add(textBoxLabel);
            textPanel.Components.Add(textBox);
            ParentPanel.BackgroundColor = ParentPanelColor;

            IsSetup = true;
        }

        public void Show()
        {
            if (!IsSetup)
                Setup();

            uiManager.PushWindow(this);
        }

        public override void Update()
        {
            base.Update();
            UpdatePanelSizes();

            if (Input.GetKeyDown(KeyCode.Return))
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

        public void SetTextTokens(TextFile.Token[] tokens)
        {
            if (!IsSetup)
                Setup();

            label.SetTextTokens(tokens);
            //UpdatePanelSizes();
        }

        public void SetTextTokens(int id)
        {
            if (!IsSetup)
                Setup();

            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(id);
            SetTextTokens(tokens);
        }

        public void SetTextBoxLabel(string label)
        {
            if (!IsSetup)
                Setup();
            if (label == null)
                return;
            textBoxLabel.Text = label;
        }

        private void UpdatePanelSizes()
        {
            Vector2 textBoxSize;
            if (MaxFit)
            {
                textBoxSize = textBox.MaxSize;
            }
            else
            {
                textBoxSize = textBox.Size;
            }

            textPanel.Position = new Vector2(textPanel.Position.x, label.Position.y + label.Size.y + textPanelDistance);
            float width = textBoxLabel.Position.x + textBoxLabel.Size.x + inputDistance + textBoxSize.x;
            float height = Mathf.Max(textBox.Size.y, textBoxSize.y);
            textPanel.Size = new Vector2(Mathf.Max(width, minWidth), height);
            textBox.Position = new Vector2(textBoxLabel.Position.x + inputDistance + textBoxLabel.Size.x, textBoxLabel.Position.y);
            messagePanel.Size = new Vector2(Mathf.Max(textPanel.Size.x, label.Size.x), label.Size.y + textPanelDistance + textPanel.Size.y);
        }

        private void MessagePanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (clickAnywhereToClose)
            {
                ReturnPlayerInputEvent(this, textBox.Text);
            }
        }

        /// <summary>
        /// Handles event from textbox on user finishing entering input
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="text"></param>
        public void textBox_OnAcceptUserInputHandler(TextBox textBox, string text)
        {
            ReturnPlayerInputEvent(this, text);
        }

        /// <summary>
        /// Returns player input & closes window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="input"></param>
        public delegate void OnReturnPlayerInputHandler(DaggerfallInputMessageBox sender, string input);

        public event OnReturnPlayerInputHandler OnGotUserInput;

        private void ReturnPlayerInputEvent(DaggerfallInputMessageBox sender, string userInput)
        {
            CloseWindow();
            if (OnGotUserInput != null)
                OnGotUserInput(sender, userInput);
        }
    }
}