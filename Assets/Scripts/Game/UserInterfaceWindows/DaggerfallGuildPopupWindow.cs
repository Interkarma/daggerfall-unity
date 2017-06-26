// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2017 Daggerfall Workshop
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
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Questing;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Hacks in guild quest pump.
    /// There is no actual guild membership, benefits, etc. in game yet.
    /// This is simply to create a wider pool of random quests for testers.
    /// </summary>
    public class DaggerfallGuildPopupWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        Rect joinGuildButtonRect = new Rect(5, 5, 120, 7);
        Rect talkButtonRect = new Rect(5, 14, 120, 7);
        Rect exitButtonRect = new Rect(44, 33, 43, 15);
        Rect serviceButtonRect = new Rect(5, 23, 120, 7);

        #endregion

        #region Curated Quests

        // These curated quests are considered "mostly working" under quest system in current state
        // This pool of quests will be made available to player for testing quest system within intended scope
        // Quests might be added or removed as development progresses

        string[] fightersQuests = new string[]
        {
            "M0B00Y06",
            //"M0B00Y00", "M0B00Y06", "M0B00Y07", "M0B00Y15", "M0B00Y16",
        };

        string[] magesQuests = new string[]
        {
            "N0B00Y06",
        };

        #endregion

        #region UI Controls

        Panel mainPanel = new Panel();
        Button joinGuildButton = new Button();
        Button talkButton = new Button();
        Button exitButton = new Button();
        Button serviceButton = new Button();
        TextLabel serviceText = new TextLabel();

        #endregion

        #region UI Textures

        Texture2D baseTexture;

        #endregion

        #region Fields

        const int noJobsNowMessageID = 600;
        const string baseTextureName = "GILD00I0.IMG";      // Join guild / Talk / Custom

        StaticNPC questorNPC;
        TempGuilds currentGuild = TempGuilds.None;
        TempGuildServices currentService = TempGuildServices.None;
        Quest offeredQuest = null;

        #endregion

        #region Properties

        public StaticNPC QuestorNPC
        {
            get { return questorNPC; }
            set { questorNPC = value; }
        }

        public TempGuilds CurrentGuild
        {
            get { return currentGuild; }
            set { currentGuild = value; }
        }

        public TempGuildServices CurrentService
        {
            get { return currentService; }
            set { currentService = value; }
        }

        #endregion

        #region Enums

        /// <summary>
        /// Temporary supported guilds.
        /// </summary>
        public enum TempGuilds
        {
            None,
            Fighter,
            Mage,
        }

        /// <summary>
        /// Temporary supported guild services.
        /// </summary>
        public enum TempGuildServices
        {
            None,
            Questor,
        }

        #endregion

        #region Constructors

        public DaggerfallGuildPopupWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load all textures
            LoadTextures();

            // Clear background
            ParentPanel.BackgroundColor = Color.clear;

            // Create interface panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.BackgroundTexture = baseTexture;
            mainPanel.Position = new Vector2(0, 50);
            mainPanel.Size = new Vector2(baseTexture.width, baseTexture.height);
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;

            // Join Guild button
            joinGuildButton = DaggerfallUI.AddButton(joinGuildButtonRect, mainPanel);
            joinGuildButton.BackgroundColor = DaggerfallUI.DaggerfallUnityNotImplementedColor;

            // Talk button
            talkButton = DaggerfallUI.AddButton(talkButtonRect, mainPanel);
            talkButton.BackgroundColor = DaggerfallUI.DaggerfallUnityNotImplementedColor;

            // Exit button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, mainPanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            // Custom button
            serviceButton = DaggerfallUI.AddButton(serviceButtonRect, mainPanel);
            serviceButton.OnMouseClick += ServiceButton_OnMouseClick;

            // Custom text
            serviceText.Position = new Vector2(0, 1);
            serviceText.ShadowPosition = Vector2.zero;
            serviceText.HorizontalAlignment = HorizontalAlignment.Center;
            serviceButton.Components.Add(serviceText);

            NativePanel.Components.Add(mainPanel);
        }

        #endregion

        #region Overrides

        public override void OnPush()
        {
            base.OnPush();

            // Set custom text based on role
            SetServiceText();
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName);
        }

        void SetServiceText()
        {
            if (currentService == TempGuildServices.Questor)
                serviceText.Text = HardStrings.getQuest;
        }

        void OfferCuratedGuildQuest()
        {
            // Select a quest from curated pool
            string questName;
            if (currentGuild == TempGuilds.Fighter)
                questName = fightersQuests[UnityEngine.Random.Range(0, fightersQuests.Length)];
            else if (currentGuild == TempGuilds.Mage)
                questName = magesQuests[UnityEngine.Random.Range(0, magesQuests.Length)];
            else
                return;

            // Parse quest
            offeredQuest = QuestMachine.Instance.ParseQuest(questName, questorNPC);
            if (offeredQuest == null)
            {
                // TODO: Show flavour text when quest cannor be compiled
                return;
            }

            // Offer the quest to player
            DaggerfallMessageBox messageBox = QuestMachine.Instance.CreateMessagePrompt(offeredQuest, (int)QuestMachine.QuestMessages.QuestorOffer);
            if (messageBox != null)
            {
                messageBox.OnButtonClick += OfferQuest_OnButtonClick;
                messageBox.Show();
            }
        }

        // Show a popup such as accept/reject message close guild window
        void ShowQuestPopupMessage(Quest quest, int id, bool exitOnClose = true)
        {
            // Get message resource
            Message message = quest.GetMessage(id);
            if (message == null)
                return;

            // Setup popup message
            TextFile.Token[] tokens = message.GetTextTokens();
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
            messageBox.SetTextTokens(tokens);
            messageBox.ClickAnywhereToClose = true;
            messageBox.AllowCancel = true;
            messageBox.ParentPanel.BackgroundColor = Color.clear;

            // Exit on close if requested
            if (exitOnClose)
                messageBox.OnClose += QuestPopupMessage_OnClose;

            // Present popup message
            messageBox.Show();
        }

        // Show a basic popup message, no Quest data involved
        void ShowQuestPopupMessage(int id, bool exitOnClose = true)
        {
            // Setup popup message
            TextFile.Token[] tokens = DaggerfallUnity.Instance.TextProvider.GetRandomTokens(id);
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager);
            messageBox.SetTextTokens(tokens);
            messageBox.ClickAnywhereToClose = true;
            messageBox.AllowCancel = true;
            messageBox.ParentPanel.BackgroundColor = Color.clear;

            // Exit on close if requested
            if (exitOnClose)
                messageBox.OnClose += QuestPopupMessage_OnClose;

            // Present popup message
            messageBox.Show();
        }

        bool IsAssignedToQuestPerson()
        {

            return false;
        }

        #endregion

        #region Event Handlers

        private void QuestPopupMessage_OnClose()
        {
            CloseWindow();
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void ServiceButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Just exit if this NPC already involved in a quest
            // If quest conditions are complete the quest system should pickup ending
            if (QuestMachine.Instance.IsLastNPCClickedQuestor())
            {
                CloseWindow();
                return;
            }

            // Offer quest from curated pool
            if (currentService == TempGuildServices.Questor)
                OfferCuratedGuildQuest();
        }

        private void OfferQuest_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                // Show accept message, add quest
                sender.CloseWindow();
                ShowQuestPopupMessage(offeredQuest, (int)QuestMachine.QuestMessages.AcceptQuest);
                QuestMachine.Instance.InstantiateQuest(offeredQuest);
            }
            else
            {
                // Show refuse message
                sender.CloseWindow();
                ShowQuestPopupMessage(offeredQuest, (int)QuestMachine.QuestMessages.RefuseQuest, false);
            }
        }

        #endregion
    }
}