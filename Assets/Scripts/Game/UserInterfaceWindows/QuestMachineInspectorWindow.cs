// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Questing;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Inspect state and data of quests running inside Quest Machine.
    /// Provides some basic debugging features.
    /// Intended for testing only, not part of normal gameplay.
    /// </summary>
    public class QuestMachineInspectorWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        QuestMachine questMachine;

        Vector2 mainPanelSize = new Vector2(600, 360);
        TextBox addQuestTextBox;

        #endregion

        #region UI Controls

        Panel mainPanel = new Panel();

        //Color questListBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.4f);
        //Color questListTextColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        Color mainPanelBackgroundColor = new Color(0.0f, 0f, 0.0f, 1.0f);
        Color mainButtonBackgroundColor = new Color(0.0f, 0.5f, 0.0f, 0.4f);

        #endregion

        #region Constructors

        public QuestMachineInspectorWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null, int screenWidth = 640, int screenHeight = 400)
            : base(uiManager, previous, screenWidth, screenHeight)
        {
            // Get reference to questmachine
            questMachine = QuestMachine.Instance;
            if (!questMachine)
                throw new Exception("QuestMachine instance not found in scene.");
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Main panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.Size = mainPanelSize;
            mainPanel.Outline.Enabled = true;
            mainPanel.BackgroundColor = mainPanelBackgroundColor;
            NativePanel.Components.Add(mainPanel);

            // Add other panels
            SetupActiveQuestsPanel();
            SetupResourceSelectPanel();
            SetupResourceInspectPanel();
            SetupSourcePanel();

            //// Quest list
            //questList.Position = new Vector2(2, 2);
            //questList.Size = new Vector2(91, 129);
            //questList.TextColor = questListTextColor;
            //questList.BackgroundColor = questListBackgroundColor;
            //questList.ShadowPosition = Vector2.zero;
            //questList.RowsDisplayed = 16;
            //questList.OnScroll += QuestList_OnScroll;
            //questPanel.Components.Add(questList);
            //questList.AddItem("_BRISIEN");
            //questList.AddItem("_TUTOR__");
            //questList.AddItem("20C00Y00");
        }

        #endregion

        #region Panel Setup

        Panel AddPanel(Rect rect, string title)
        {
            // Panel
            Panel panel = new Panel();
            panel.Position = new Vector2(rect.x, rect.y);
            panel.Size = new Vector2(rect.width, rect.height);
            panel.Outline.Enabled = true;
            panel.Outline.Color = Color.gray;
            panel.BackgroundColor = new Color(0.25f, 0.25f, 0.25f, 0.5f);
            mainPanel.Components.Add(panel);

            // Label
            TextLabel label = new TextLabel();
            label.ShadowPosition = Vector2.zero;
            label.Position = new Vector2(0, 4);
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.Text = title;
            panel.Components.Add(label);

            return panel;
        }

        void SetupActiveQuestsPanel()
        {
            Panel questPanel = AddPanel(new Rect(4, 4, 100, 352), "Active Quests");
            addQuestTextBox = DaggerfallUI.AddTextBox(new Rect(2, 12, 76, 10), "Quest Filename", questPanel, 8);
            addQuestTextBox.BackgroundColor = Color.black;
            addQuestTextBox.UpperOnly = true;
            addQuestTextBox.FixedSize = true;
            addQuestTextBox.Outline.Enabled = true;
            addQuestTextBox.UseFocus = true;
            addQuestTextBox.Text = "_TUTOR__";
            //addQuestTextBox.Text = "_BRISIEN";
            addQuestTextBox.SetFocus();

            Button addQuestButton = DaggerfallUI.AddTextButton(new Rect(80, 12, 18, 10), "Add", questPanel);
            addQuestButton.BackgroundColor = mainButtonBackgroundColor;
            addQuestButton.OnMouseClick += AddQuestButton_OnMouseClick;
        }

        void SetupResourceSelectPanel()
        {
            /*Panel resourcePanel = */AddPanel(new Rect(108, 4, 100, 352), "Resources");
        }

        void SetupResourceInspectPanel()
        {
            /*Panel inspectPanel = */AddPanel(new Rect(212, 4, 384, 240), "Visual Inspector");
        }

        void SetupSourcePanel()
        {
            /*Panel sourcePanel = */AddPanel(new Rect(212, 248, 384, 108), "Related Source");
        }

        #endregion

        #region QuestsPanel Event Handlers

        private void AddQuestButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            questMachine.InstantiateQuest(addQuestTextBox.Text);
        }

        #endregion
    }
}