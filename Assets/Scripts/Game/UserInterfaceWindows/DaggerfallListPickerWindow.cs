// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements a list picker popup window.
    /// </summary>
    public class DaggerfallListPickerWindow : DaggerfallPopupWindow
    {
        const string nativeImgName = "PICK00I0.IMG";

        Texture2D nativeTexture;
        protected Panel pickerPanel = new Panel();
        protected ListBox listBox = new ListBox();
        protected VerticalScrollBar scrollBar;
        protected DaggerfallFont font = null;
        protected int rowsDisplayed = 0;

        public DaggerfallFont Font
        {
            get { return listBox.Font; }
            set { listBox.Font = (value != null) ? value : DaggerfallUI.DefaultFont; }
        }

        public int RowsDisplayed
        {
            get { return listBox.RowsDisplayed; }
            set { listBox.RowsDisplayed = (value > 0) ? value : listBox.RowsDisplayed; }
        }

        public DaggerfallListPickerWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previous = null, DaggerfallFont font = null, int rowsDisplayed = 0)
            : base(uiManager, previous)
        {
            Font = font;
            RowsDisplayed = rowsDisplayed;
        }

        public ListBox ListBox
        {
            get { return listBox; }
        }

        protected override void Setup()
        {
            base.Setup();

            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallClassSelectWindow: Could not load native texture.");

            // Create panel for picker
            pickerPanel.Size = TextureReplacement.GetSize(nativeTexture, nativeImgName);
            pickerPanel.HorizontalAlignment = HorizontalAlignment.Center;
            pickerPanel.VerticalAlignment = VerticalAlignment.Middle;
            pickerPanel.BackgroundTexture = nativeTexture;
            pickerPanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
            NativePanel.Components.Add(pickerPanel);

            // Create list box
            listBox.Position = new Vector2(26, 27);
            listBox.Size = new Vector2(138, 72);
            listBox.OnUseSelectedItem += ListBox_OnUseSelectedItem;
            pickerPanel.Components.Add(listBox);

            // Add previous button
            Button previousButton = DaggerfallUI.AddButton(new Rect(179, 10, 9, 9), pickerPanel);
            previousButton.OnMouseClick += PreviousButton_OnMouseClick;

            // Add next button
            Button nextButton = DaggerfallUI.AddButton(new Rect(179, 108, 9, 9), pickerPanel);
            nextButton.OnMouseClick += NextButton_OnMouseClick;

            // Add scrollbar
            scrollBar = new VerticalScrollBar();
            scrollBar.Position = new Vector2(181, 23);
            scrollBar.Size = new Vector2(5, 82);
            scrollBar.OnScroll += ScrollBar_OnScroll;
            pickerPanel.Components.Add(scrollBar);
        }

        public override void Update()
        {
            base.Update();

            scrollBar.TotalUnits = listBox.Count;
            scrollBar.DisplayUnits = listBox.RowsDisplayed;

            if (scrollBar.DraggingThumb)
            {
                listBox.ScrollIndex = scrollBar.ScrollIndex;
                //listBox.ClampSelectionToVisibleRange();
            }
            else
            {
                scrollBar.ScrollIndex = listBox.ScrollIndex;
            }
        }

        void PreviousButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            listBox.SelectPrevious();
        }

        void NextButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            listBox.SelectNext();
        }

        private void ScrollBar_OnScroll()
        {
            listBox.ScrollIndex = scrollBar.ScrollIndex;
        }

        void ListBox_OnUseSelectedItem()
        {
            RaiseOnItemPickedEvent();
        }

        #region Event Handlers

        public delegate void OnItemPickedEventHandler(int index, string itemString);
        public event OnItemPickedEventHandler OnItemPicked;
        void RaiseOnItemPickedEvent()
        {
            if (OnItemPicked != null)
                OnItemPicked(listBox.SelectedIndex, listBox.SelectedItem);
        }

        #endregion
    }
}