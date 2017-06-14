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
        Rect customButtonRect = new Rect(5, 23, 120, 7);

        #endregion

        #region UI Controls

        Panel mainPanel = new Panel();
        Button joinGuildButton = new Button();
        Button talkButton = new Button();
        Button exitButton = new Button();
        Button customButton = new Button();
        TextLabel customText = new TextLabel();

        #endregion

        #region UI Textures

        Texture2D baseTexture;

        #endregion

        #region Fields

        const string baseTextureName = "GILD00I0.IMG";      // Join guild / Talk / Custom

        DaggerfallBillboard questorNPC;
        TempGuilds currentGuild = TempGuilds.Fighter;
        TempGuildRoles currentRole = TempGuildRoles.Questor;
        Quest offeredQuest = null;

        #endregion

        #region Properties

        public DaggerfallBillboard QuestorNPC
        {
            get { return questorNPC; }
            set { questorNPC = value; }
        }

        public TempGuilds CurrentGuild
        {
            get { return currentGuild; }
            set { currentGuild = value; }
        }

        public TempGuildRoles CurrentRole
        {
            get { return currentRole; }
            set { currentRole = TempGuildRoles.Questor; }
        }

        #endregion

        #region Enums

        /// <summary>
        /// Temporary supported guilds.
        /// </summary>
        public enum TempGuilds
        {
            Fighter,
        }

        /// <summary>
        /// Temporary supported guild roles.
        /// </summary>
        public enum TempGuildRoles
        {
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
            customButton = DaggerfallUI.AddButton(customButtonRect, mainPanel);
            customButton.OnMouseClick += CustomButton_OnMouseClick;

            // Custom text
            customText.Position = new Vector2(0, 1);
            customText.ShadowPosition = Vector2.zero;
            customText.HorizontalAlignment = HorizontalAlignment.Center;
            customButton.Components.Add(customText);

            NativePanel.Components.Add(mainPanel);
        }

        #endregion

        #region Overrides

        public override void OnPush()
        {
            base.OnPush();

            // Set custom text based on role
            SetCustomText();
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            baseTexture = ImageReader.GetTexture(baseTextureName);
        }

        void SetCustomText()
        {
            if (currentRole == TempGuildRoles.Questor)
                customText.Text = HardStrings.getQuest;
        }

        void AssignRandomGuildQuest()
        {
            // TODO: Full support for filename pattern
            // Here we just want to pull all available quests for pump
            string pattern = string.Format("{0}*.TXT", GetCurrentGuildPrefix());

            // Get all quests matching pattern
            string[] files = Directory.GetFiles(QuestMachine.Instance.QuestSourceFolder, pattern);

            // Get a random quest index
            // Not interested in membership, rank, etc. just handing out quests to testers
            int index = UnityEngine.Random.Range(0, files.Length);
            string questName = Path.GetFileName(files[index]);

            // Parse quest
            offeredQuest = QuestMachine.Instance.ParseQuest(questName);

            // Link questor NPC if set
            if (questorNPC != null)
            {
                // TODO: Link questor to quest data
            }

            // Offer the quest to player
            DaggerfallMessageBox messageBox = QuestMachine.Instance.CreateMessagePrompt(offeredQuest, (int)QuestMachine.QuestMessages.QuestorOffer);
            if (messageBox != null)
            {
                messageBox.OnButtonClick += OfferQuest_OnButtonClick;
                messageBox.Show();
            }
        }

        string GetCurrentGuildPrefix()
        {
            switch (currentGuild)
            {
                case TempGuilds.Fighter:
                    return "M";
                default:
                    throw new Exception("Not a supported guild");
            }
        }

        #endregion

        #region Event Handlers

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void CustomButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            AssignRandomGuildQuest();
        }

        private void OfferQuest_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                // TODO: Show accept message, add quest
            }
            else
            {
                // TODO: Show refuse message
            }

            sender.CloseWindow();
        }

        #endregion
    }
}