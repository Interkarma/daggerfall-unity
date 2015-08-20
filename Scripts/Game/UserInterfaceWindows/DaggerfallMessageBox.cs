// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
        MultilineTextLabel label = new MultilineTextLabel();
        List<Button> buttons = new List<Button>();
        int buttonSpacing = 32;
        int buttonTextDistance = 4;
        MessageBoxButtons selectedButton = MessageBoxButtons.Cancel;
        bool cancelled = false;

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

        public bool Cancelled
        {
            get { return cancelled; }
        }

        public DaggerfallMessageBox(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        protected override void Setup()
        {
            if (IsSetup)
                return;

            base.Setup();

            messagePanel.HorizontalAlignment = HorizontalAlignment.Center;
            messagePanel.VerticalAlignment = VerticalAlignment.Middle;
            DaggerfallUI.Instance.SetDaggerfallPopupStyle(messagePanel);
            NativePanel.Components.Add(messagePanel);

            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Top;
            messagePanel.Components.Add(label);

            buttonPanel.HorizontalAlignment = HorizontalAlignment.Center;
            buttonPanel.VerticalAlignment = VerticalAlignment.Bottom;
            messagePanel.Components.Add(buttonPanel);

            IsSetup = true;
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(exitKey))
            {
                cancelled = true;
                CloseWindow();
            }
        }

        public void AddButton(MessageBoxButtons messageBoxButton)
        {
            if (!IsSetup)
                Setup();

            Texture2D background = GetTextureFromCifRci(buttonsFilename, (int)messageBoxButton);
            Button button = AddButton(Vector2.zero, new Vector2(background.width, background.height), buttonPanel);
            button.BackgroundTexture = background;
            button.BackgroundTextureLayout = TextureLayout.StretchToFill;
            button.Tag = messageBoxButton;
            button.OnMouseClick += ButtonClickHandler;
            buttons.Add(button);

            UpdatePanelSizes();
        }

        public void SetTextTokens(TextFile.Token[] tokens)
        {
            if (!IsSetup)
                Setup();

            label.SetTextTokens(tokens);
            UpdatePanelSizes();
        }

        void ButtonClickHandler(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
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

            buttonPanel.Size = finalSize;
            messagePanel.Size = new Vector2(label.Size.x, label.Size.y + buttonPanel.Size.y + buttonTextDistance);
        }

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