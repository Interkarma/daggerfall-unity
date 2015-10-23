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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements character sheet window.
    /// </summary>
    public class DaggerfallCharacterSheetWindow : DaggerfallPopupWindow
    {
        const string nativeImgName = "INFO00I0.IMG";

        Texture2D nativeTexture;
        PlayerEntity playerEntity;
        TextLabel nameLabel = new TextLabel();
        TextLabel raceLabel = new TextLabel();
        TextLabel classLabel = new TextLabel();
        TextLabel levelLabel = new TextLabel();
        //TextLabel goldLabel = new TextLabel();
        TextLabel fatigueLabel = new TextLabel();
        TextLabel healthLabel = new TextLabel();
        //TextLabel encumbranceLabel = new TextLabel();
        TextLabel[] statLabels = new TextLabel[DaggerfallStats.Count];
        Panel playerBackgroundPanel = new Panel();
        Panel playerBodyPanel = new Panel();
        Panel playerHeadPanel = new Panel();

        PlayerEntity PlayerEntity
        {
            get { return (playerEntity != null) ? playerEntity : GetPlayerEntity(); }
        }

        public DaggerfallCharacterSheetWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallCharacterSheetWindow: Could not load native texture.");

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Setup labels
            nameLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(41, 4), NativePanel);
            raceLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(41, 14), NativePanel);
            classLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(46, 24), NativePanel);
            levelLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(45, 34), NativePanel);
            //goldLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(39, 44), NativePanel);
            fatigueLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(57, 54), NativePanel);
            healthLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(52, 64), NativePanel);
            //encumbranceLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(90, 74), NativePanel);

            // Setup stat labels
            Vector2 pos = new Vector2(150, 17);
            for (int i = 0; i < DaggerfallStats.Count; i++)
            {
                statLabels[i] = DaggerfallUI.AddDefaultShadowedTextLabel(pos, NativePanel);
                pos.y += 24f;
            }

            // Primary skills button
            Button primarySkillsButton = DaggerfallUI.AddButton(new Rect(11, 106, 115, 8), NativePanel);
            primarySkillsButton.OnMouseClick += PrimarySkillsButton_OnMouseClick;

            // Major skills button
            Button majorSkillsButton = DaggerfallUI.AddButton(new Rect(11, 116, 115, 8), NativePanel);
            majorSkillsButton.OnMouseClick += MajorSkillsButton_OnMouseClick;

            // Minor skills button
            Button minorSkillsButton = DaggerfallUI.AddButton(new Rect(11, 126, 115, 8), NativePanel);
            minorSkillsButton.OnMouseClick += MinorSkillsButton_OnMouseClick;

            // Miscellaneous skills button
            Button miscSkillsButton = DaggerfallUI.AddButton(new Rect(11, 136, 115, 8), NativePanel);
            miscSkillsButton.OnMouseClick += MiscSkillsButton_OnMouseClick;

            // Inventory button
            Button inventoryButton = DaggerfallUI.AddButton(new Rect(3, 151, 65, 12), NativePanel);
            inventoryButton.BackgroundColor = DaggerfallUI.DaggerfallUnityNotImplementedColor;

            // Spellbook button
            Button spellBookButton = DaggerfallUI.AddButton(new Rect(69, 151, 65, 12), NativePanel);
            spellBookButton.BackgroundColor = DaggerfallUI.DaggerfallUnityNotImplementedColor;

            // Logbook button
            Button logBookButton = DaggerfallUI.AddButton(new Rect(3, 165, 65, 12), NativePanel);
            logBookButton.BackgroundColor = DaggerfallUI.DaggerfallUnityNotImplementedColor;

            // History button
            Button historyButton = DaggerfallUI.AddButton(new Rect(69, 165, 65, 12), NativePanel);
            historyButton.BackgroundColor = DaggerfallUI.DaggerfallUnityNotImplementedColor;

            // Exit button
            Button exitButton = DaggerfallUI.AddButton(new Rect(50, 179, 39, 19), NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            // Attribute popup text
            pos = new Vector2(141, 6);
            for (int i = 0; i < DaggerfallStats.Count; i++)
            {
                Rect rect = new Rect(pos.x, pos.y, 28, 20);
                AddAttributePopupButton((DFCareer.Stats)i, rect);
                pos.y += 24f;
            }

            // Player paper doll components
            // This will later be moved later to a dedicated control with complete inventory support
            NativePanel.Components.Add(playerBackgroundPanel);
            NativePanel.Components.Add(playerBodyPanel);
            NativePanel.Components.Add(playerHeadPanel);

            // Update player paper doll for first time
            UpdatePlayerValues();
            UpdatePlayerAvatarPanel();
        }

        public override void OnPush()
        {
            if (IsSetup)
            {
                UpdatePlayerValues();
                UpdatePlayerAvatarPanel();
            }
        }

        #region Private Methods

        // Finds Player and returns PlayerEntity object
        PlayerEntity GetPlayerEntity()
        {
            // Get player object
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (!player)
                throw new Exception("Could not find Player.");

            DaggerfallEntityBehaviour entityBehaviour = player.GetComponent<DaggerfallEntityBehaviour>();
            if (!entityBehaviour)
                throw new Exception("Could not find DaggerfallEntityBehaviour on Player.");

            // Get player entity
            playerEntity = entityBehaviour.Entity as PlayerEntity;
            if (playerEntity == null)
                throw new Exception("Could not find PlayerEntity.");

            return playerEntity;
        }

        // Adds button for attribute popup text
        // NOTE: This has only partial functionality until %vars and all formatting tokens are supported
        void AddAttributePopupButton(DFCareer.Stats stat, Rect rect)
        {
            Button button = DaggerfallUI.AddButton(rect, NativePanel);
            button.Tag = DaggerfallUnity.TextProvider.GetStatDescriptionTextID(stat);
            button.OnMouseClick += StatButton_OnMouseClick;
        }

        // Creates formatting tokens for skill popups
        TextFile.Token[] CreateSkillTokens(DFCareer.Skills skill)
        {
            List<TextFile.Token> tokens = new List<TextFile.Token>();

            TextFile.Token skillNameToken = new TextFile.Token();
            skillNameToken.text = DaggerfallUnity.Instance.TextProvider.GetSkillName(skill);
            skillNameToken.formatting = TextFile.Formatting.Text;

            TextFile.Token skillValueToken = new TextFile.Token();
            skillValueToken.text = string.Format("{0}%", playerEntity.Skills.GetSkillValue(skill));
            skillValueToken.formatting = TextFile.Formatting.Text;

            DFCareer.Stats primaryStat = DaggerfallSkills.GetPrimaryStat(skill);
            TextFile.Token skillPrimaryStatToken = new TextFile.Token();
            skillPrimaryStatToken.text = DaggerfallUnity.Instance.TextProvider.GetAbbreviatedStatName(primaryStat);
            skillPrimaryStatToken.formatting = TextFile.Formatting.Text;

            TextFile.Token spacesToken = new TextFile.Token();
            spacesToken.formatting = TextFile.Formatting.Text;
            spacesToken.text = "  ";

            TextFile.Token tabToken = new TextFile.Token();
            tabToken.formatting = TextFile.Formatting.PositionPrefix;

            // Add tokens in order
            tokens.Add(skillNameToken);
            tokens.Add(tabToken);
            tokens.Add(tabToken);
            tokens.Add(skillValueToken);
            tokens.Add(spacesToken);
            tokens.Add(skillPrimaryStatToken);

            return tokens.ToArray();
        }

        void ShowSkillsDialog(List<DFCareer.Skills> skills, bool twoColumn = false)
        {
            bool secondColumn = false;
            List<TextFile.Token> tokens = new List<TextFile.Token>();
            for (int i = 0; i < skills.Count; i++)
            {
                if (!twoColumn)
                {
                    tokens.AddRange(CreateSkillTokens(skills[i]));
                    if (i < skills.Count - 1)
                        tokens.Add(TextFile.NewLineToken);
                }
                else
                {
                    tokens.AddRange(CreateSkillTokens(skills[i]));
                    if (!secondColumn)
                    {
                        secondColumn = !secondColumn;
                        tokens.Add(TextFile.TabToken);
                    }
                    else
                    {
                        secondColumn = !secondColumn;
                        if (i < skills.Count - 1)
                            tokens.Add(TextFile.NewLineToken);
                    }
                }
            }

            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            messageBox.SetTextTokens(tokens.ToArray());
            messageBox.ClickAnywhereToClose = true;
            messageBox.Show();
        }

        void UpdatePlayerValues()
        {
            // Update main labels
            nameLabel.Text = PlayerEntity.Name;
            raceLabel.Text = PlayerEntity.Race.Name;
            classLabel.Text = PlayerEntity.Career.Name;
            levelLabel.Text = PlayerEntity.Level.ToString();
            fatigueLabel.Text = string.Format("{0}/{1}", PlayerEntity.CurrentFatigue, PlayerEntity.MaxFatigue);
            healthLabel.Text = string.Format("{0}/{1}", PlayerEntity.CurrentHealth, PlayerEntity.MaxHealth);

            // Update stat labels
            for (int i = 0; i < DaggerfallStats.Count; i++)
            {
                statLabels[i].Text = playerEntity.Stats.GetStatValue(i).ToString();
            }
        }

        // Test paper doll setup only
        // Will be moved later to a unique control
        void UpdatePlayerAvatarPanel()
        {
            // Update player background
            Texture2D raceBackground = DaggerfallUI.GetTextureFromImg(PlayerEntity.Race.PaperDollBackground);
            playerBackgroundPanel.BackgroundTexture = raceBackground;
            playerBackgroundPanel.Position = new Vector2(192, 1);
            playerBackgroundPanel.Size = new Vector2(raceBackground.width, raceBackground.height);

            // Get player body image
            string paperDollBodyImageName = string.Empty;
            if (DaggerfallUnity.Settings.NoPlayerNudity)
            {
                if (PlayerEntity.Gender == Genders.Male)
                    paperDollBodyImageName = PlayerEntity.Race.PaperDollBodyMaleClothed;
                else
                    paperDollBodyImageName = PlayerEntity.Race.PaperDollBodyFemaleClothed;
            }
            else
            {
                if (PlayerEntity.Gender == Genders.Male)
                    paperDollBodyImageName = PlayerEntity.Race.PaperDollBodyMaleUnclothed;
                else
                    paperDollBodyImageName = PlayerEntity.Race.PaperDollBodyFemaleUnclothed;
            }

            // Update body
            DFPosition offset;
            Texture2D playerBodyTexture = DaggerfallUI.GetTextureFromImg(paperDollBodyImageName, out offset);
            playerBodyPanel.Size = new Vector2(playerBodyTexture.width, playerBodyTexture.height);
            playerBodyPanel.Position = new Vector2(offset.X, offset.Y);
            playerBodyPanel.BackgroundTexture = playerBodyTexture;

            // Get player head image
            Texture2D playerHeadTexture;
            if (PlayerEntity.Gender == Genders.Male)
                playerHeadTexture = DaggerfallUI.GetTextureFromCifRci(PlayerEntity.Race.PaperDollHeadsMale, PlayerEntity.FaceIndex, out offset);
            else
                playerHeadTexture = DaggerfallUI.GetTextureFromCifRci(PlayerEntity.Race.PaperDollHeadsFemale, PlayerEntity.FaceIndex, out offset);

            // Update head
            playerHeadPanel.Size = new Vector2(playerHeadTexture.width, playerHeadTexture.height);
            playerHeadPanel.Position = new Vector2(offset.X, offset.Y);
            playerHeadPanel.BackgroundTexture = playerHeadTexture;
        }

        #endregion

        #region Event Handlers

        private void PrimarySkillsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ShowSkillsDialog(PlayerEntity.GetPrimarySkills());
        }

        private void MajorSkillsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ShowSkillsDialog(PlayerEntity.GetMajorSkills());
        }

        private void MinorSkillsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ShowSkillsDialog(PlayerEntity.GetMinorSkills());
        }

        private void MiscSkillsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ShowSkillsDialog(PlayerEntity.GetMiscSkills(), true);
        }

        private void StatButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            messageBox.SetTextTokens((int)sender.Tag);
            messageBox.ClickAnywhereToClose = true;
            messageBox.Show();
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion
    }
}