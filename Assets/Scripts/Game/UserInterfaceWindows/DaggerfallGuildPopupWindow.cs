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
        #region Tables

        // Table constants
        const string tempFightersQuestsFilename = "Temp-FightersQuests";
        const string tempMagesQuestsFilename = "Temp-MagesGuild";

        #endregion

        #region UI Rects

        Rect joinGuildButtonRect = new Rect(5, 5, 120, 7);
        Rect talkButtonRect = new Rect(5, 14, 120, 7);
        Rect exitButtonRect = new Rect(44, 33, 43, 15);
        Rect serviceButtonRect = new Rect(5, 23, 120, 7);

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
            // Load quests table each time so player can edit their local file at runtime
            Table table = null;
            string questName = string.Empty;
            try
            {
                if (currentGuild == TempGuilds.Fighter)
                {
                    table = new Table(QuestMachine.Instance.GetTableSourceText(tempFightersQuestsFilename));
                }
                else if (currentGuild == TempGuilds.Mage)
                {
                    table = new Table(QuestMachine.Instance.GetTableSourceText(tempFightersQuestsFilename));
                }
                else
                {
                    throw new Exception("Could not load quests table for this guild.");
                }
            }
            catch(Exception ex)
            {
                DaggerfallUI.Instance.PopupMessage(ex.Message);
                return;
            }

            // Select a quest name at random from table
            try
            {
                if (table == null || table.RowCount == 0)
                    throw new Exception("Quests table is empty.");

                questName = table.GetRow(UnityEngine.Random.Range(0, table.RowCount))[0];
            }
            catch (Exception ex)
            {
                DaggerfallUI.Instance.PopupMessage(ex.Message);
                return;
            }

            // Log offered quest
            Debug.LogFormat("Offering quest {0} from TempGuild {1}", questName, currentGuild.ToString());

            // Parse quest
            offeredQuest = QuestMachine.Instance.ParseQuest(questName, questorNPC);
            if (offeredQuest == null)
            {
                // TODO: Show flavour text when quest cannot be compiled
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