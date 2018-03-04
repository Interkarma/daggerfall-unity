// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Hazelnut
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Guilds;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements character sheet window.
    /// </summary>
    public class DaggerfallCharacterSheetWindow : DaggerfallPopupWindow
    {
        const string nativeImgName = "INFO00I0.IMG";
        private const int noAffiliationsMsgId = 19;

        Texture2D nativeTexture;
        PlayerEntity playerEntity;
        TextLabel nameLabel = new TextLabel();
        TextLabel raceLabel = new TextLabel();
        TextLabel classLabel = new TextLabel();
        TextLabel levelLabel = new TextLabel();
        TextLabel goldLabel = new TextLabel();
        TextLabel fatigueLabel = new TextLabel();
        TextLabel healthLabel = new TextLabel();
        TextLabel encumbranceLabel = new TextLabel();
        TextLabel[] statLabels = new TextLabel[DaggerfallStats.Count];
        PaperDoll characterPortrait = new PaperDoll();

        StatsRollout statsRollout;

        bool leveling = false;

        const int minBonusPool = 4;        // The minimum number of free points to allocate on level up
        const int maxBonusPool = 6;        // The maximum number of free points to allocate on level up
        SoundClips levelUpSound = SoundClips.LevelUp;

        PlayerEntity PlayerEntity
        {
            get { return (playerEntity != null) ? playerEntity : playerEntity = GameManager.Instance.PlayerEntity; }
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

            // Character portrait
            NativePanel.Components.Add(characterPortrait);
            characterPortrait.Position = new Vector2(200, 8);

            // Setup labels
            nameLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(41, 4), NativePanel);
            raceLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(41, 14), NativePanel);
            classLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(46, 24), NativePanel);
            levelLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(45, 34), NativePanel);
            goldLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(39, 44), NativePanel);
            fatigueLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(57, 54), NativePanel);
            healthLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(52, 64), NativePanel);
            encumbranceLabel = DaggerfallUI.AddDefaultShadowedTextLabel(new Vector2(90, 74), NativePanel);

            // Setup stat labels
            Vector2 pos = new Vector2(150, 17);
            for (int i = 0; i < DaggerfallStats.Count; i++)
            {
                statLabels[i] = DaggerfallUI.AddDefaultShadowedTextLabel(pos, NativePanel);
                pos.y += 24f;
            }

            // Health button
            Button healthButton = DaggerfallUI.AddButton(new Rect(4, 63, 128, 8), NativePanel);
            healthButton.OnMouseClick += HealthButton_OnMouseClick;

            // Affiliations button
            Button affiliationsButton = DaggerfallUI.AddButton(new Rect(3, 84, 130, 8), NativePanel);
            affiliationsButton.OnMouseClick += AffiliationsButton_OnMouseClick;

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
            inventoryButton.OnMouseClick += InventoryButton_OnMouseClick;
            //inventoryButton.BackgroundColor = DaggerfallUI.DaggerfallUnityNotImplementedColor;

            // Spellbook button
            Button spellBookButton = DaggerfallUI.AddButton(new Rect(69, 151, 65, 12), NativePanel);
            spellBookButton.BackgroundColor = DaggerfallUI.DaggerfallUnityNotImplementedColor;

            // Logbook button
            Button logBookButton = DaggerfallUI.AddButton(new Rect(3, 165, 65, 12), NativePanel);
            logBookButton.OnMouseClick += LogBookButton_OnMouseClick;
            //logBookButton.BackgroundColor = DaggerfallUI.DaggerfallUnityNotImplementedColor;

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

            statsRollout = new StatsRollout(true);
            statsRollout.OnStatChanged += StatsRollout_OnStatChanged;

            // Update player paper doll for first time
            UpdatePlayerValues();
            characterPortrait.Refresh();
        }

        public override void OnPush()
        {
            Refresh();
        }

        public override void OnReturn()
        {
            Refresh();
        }

        public override void CancelWindow()
        {
            if (leveling)
            {
                if (!CheckIfDoneLeveling())
                    return;
            }
            base.CancelWindow();
        }

        #region Private Methods

        // Adds button for attribute popup text
        // NOTE: This has only partial functionality until %vars and all formatting tokens are supported
        void AddAttributePopupButton(DFCareer.Stats stat, Rect rect)
        {
            Button button = DaggerfallUI.AddButton(rect, NativePanel);
            button.Tag = DaggerfallUnity.TextProvider.GetStatDescriptionTextID(stat);
            button.OnMouseClick += StatButton_OnMouseClick;
        }

        // Creates formatting tokens for skill popups
        TextFile.Token[] CreateSkillTokens(DFCareer.Skills skill, bool twoColumn = false, int startPosition = 0)
        {
            List<TextFile.Token> tokens = new List<TextFile.Token>();

            TextFile.Token skillNameToken = new TextFile.Token();
            skillNameToken.text = DaggerfallUnity.Instance.TextProvider.GetSkillName(skill);
            skillNameToken.formatting = TextFile.Formatting.Text;

            TextFile.Token skillValueToken = new TextFile.Token();
            skillValueToken.text = string.Format("{0}%", playerEntity.Skills.GetLiveSkillValue(skill));
            skillValueToken.formatting = TextFile.Formatting.Text;

            DFCareer.Stats primaryStat = DaggerfallSkills.GetPrimaryStat(skill);
            TextFile.Token skillPrimaryStatToken = new TextFile.Token();
            skillPrimaryStatToken.text = DaggerfallUnity.Instance.TextProvider.GetAbbreviatedStatName(primaryStat);
            skillPrimaryStatToken.formatting = TextFile.Formatting.Text;

            TextFile.Token positioningToken = new TextFile.Token();
            positioningToken.formatting = TextFile.Formatting.PositionPrefix;

            TextFile.Token tabToken = new TextFile.Token();
            tabToken.formatting = TextFile.Formatting.PositionPrefix;

            // Add tokens in order
            if (!twoColumn)
            {
                tokens.Add(skillNameToken);
                tokens.Add(tabToken);
                tokens.Add(tabToken);
                tokens.Add(tabToken);
                tokens.Add(skillValueToken);
                tokens.Add(tabToken);
                tokens.Add(skillPrimaryStatToken);
            }
            else // miscellaneous skills
            {
                if (startPosition != 0) // if this is the second column
                {
                    positioningToken.x = startPosition;
                    tokens.Add(positioningToken);
                }
                tokens.Add(skillNameToken);
                positioningToken.x = startPosition + 85;
                tokens.Add(positioningToken);
                tokens.Add(skillValueToken);
                positioningToken.x = startPosition + 112;
                tokens.Add(positioningToken);
                tokens.Add(skillPrimaryStatToken);
            }

            return tokens.ToArray();
        }

        void ShowSkillsDialog(List<DFCareer.Skills> skills, bool twoColumn = false)
        {
            bool secondColumn = false;
            bool showHandToHandDamage = false;
            List<TextFile.Token> tokens = new List<TextFile.Token>();
            for (int i = 0; i < skills.Count; i++)
            {
                if (!showHandToHandDamage && (skills[i] == DFCareer.Skills.HandToHand))
                    showHandToHandDamage = true;

                if (!twoColumn)
                {
                    tokens.AddRange(CreateSkillTokens(skills[i]));
                    if (i < skills.Count - 1)
                        tokens.Add(TextFile.NewLineToken);
                }
                else
                {
                    if (!secondColumn)
                    {
                        tokens.AddRange(CreateSkillTokens(skills[i], true));
                        secondColumn = !secondColumn;
                    }
                    else
                    {
                        tokens.AddRange(CreateSkillTokens(skills[i], true, 136));
                        secondColumn = !secondColumn;
                        if (i < skills.Count - 1)
                            tokens.Add(TextFile.NewLineToken);
                    }
                }
            }

            if (showHandToHandDamage)
            {
                tokens.Add(TextFile.NewLineToken);
                TextFile.Token HandToHandDamageToken = new TextFile.Token();
                int minDamage = FormulaHelper.CalculateHandToHandMinDamage(playerEntity.Skills.GetLiveSkillValue(DFCareer.Skills.HandToHand));
                int maxDamage = FormulaHelper.CalculateHandToHandMaxDamage(playerEntity.Skills.GetLiveSkillValue(DFCareer.Skills.HandToHand));
                HandToHandDamageToken.text = DaggerfallUnity.Instance.TextProvider.GetSkillName(DFCareer.Skills.HandToHand) + " dmg: " + minDamage + "-" + maxDamage;
                HandToHandDamageToken.formatting = TextFile.Formatting.Text;
                tokens.Add(HandToHandDamageToken);
            }

            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
            messageBox.SetTextTokens(tokens.ToArray(), null, false);
            messageBox.ClickAnywhereToClose = true;
            messageBox.Show();
        }

        void ShowAffiliationsDialog()
        {
            List<TextFile.Token> tokens = new List<TextFile.Token>();
            List<Guild> guildMemberships = GameManager.Instance.GuildManager.GetMemberships();

            if (guildMemberships.Count == 0)
                DaggerfallUI.MessageBox(noAffiliationsMsgId);
            else
            {
                TextFile.Token tab = TextFile.TabToken;
                tab.x = 120;
                tokens.Add(new TextFile.Token() {
                    text = HardStrings.affiliation,
                    formatting = TextFile.Formatting.TextHighlight
                });
                tokens.Add(tab);
                tokens.Add(new TextFile.Token()
                {
                    text = HardStrings.rank,
                    formatting = TextFile.Formatting.TextHighlight
                });
                tokens.Add(TextFile.NewLineToken);

                foreach (Guild guild in guildMemberships)
                {
                    tokens.Add(TextFile.CreateTextToken(guild.GetAffiliation()));
                    tokens.Add(tab);
                    tokens.Add(TextFile.CreateTextToken(guild.GetTitle() //)); DEBUG rep:
                        + " (rep:" + guild.GetReputation(playerEntity).ToString() + ")"));
                    tokens.Add(TextFile.NewLineToken);
                }

                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens(tokens.ToArray(), null, false);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
            }
        }


        void UpdatePlayerValues()
        {
            // Handle leveling up
            if (PlayerEntity.ReadyToLevelUp)
            {
                leveling = true;
                PlayerEntity.Level++;
                PlayerEntity.MaxHealth += FormulaHelper.CalculateHitPointsPerLevelUp(PlayerEntity);
                DaggerfallUI.Instance.PlayOneShot(levelUpSound);

                // Roll bonus pool for player to distribute
                // Using maxBonusPool + 1 for inclusive range
                int bonusPool = UnityEngine.Random.Range(minBonusPool, maxBonusPool + 1);

                // Add stats rollout for leveling up
                NativePanel.Components.Add(statsRollout);

                this.statsRollout.StartingStats = PlayerEntity.Stats.Clone();
                this.statsRollout.WorkingStats = PlayerEntity.Stats.Clone();
                this.statsRollout.BonusPool = bonusPool;

                PlayerEntity.ReadyToLevelUp = false;
            }

            // Update main labels
            nameLabel.Text = PlayerEntity.Name;
            raceLabel.Text = PlayerEntity.RaceTemplate.Name;
            classLabel.Text = PlayerEntity.Career.Name;
            levelLabel.Text = PlayerEntity.Level.ToString();
            goldLabel.Text = PlayerEntity.GetGoldAmount().ToString();
            fatigueLabel.Text = string.Format("{0}/{1}", PlayerEntity.CurrentFatigue / 64, PlayerEntity.MaxFatigue / 64);
            healthLabel.Text = string.Format("{0}/{1}", PlayerEntity.CurrentHealth, PlayerEntity.MaxHealth);
            encumbranceLabel.Text = string.Format("{0}/{1}", (int)PlayerEntity.CarriedWeight, PlayerEntity.MaxEncumbrance);

            // Update stat labels
            // TODO: Colour blue/red based on whether stat mod has change above/below permanent value
            for (int i = 0; i < DaggerfallStats.Count; i++)
            {
                if (!leveling)
                    statLabels[i].Text = PlayerEntity.Stats.GetLiveStatValue(i).ToString();
                else
                    statLabels[i].Text = ""; // If leveling, statsRollout will fill in the stat labels.
            }
        }

        void Refresh()
        {
            if (IsSetup)
            {
                UpdatePlayerValues();
                characterPortrait.Refresh();
            }
        }

        bool CheckIfDoneLeveling()
        {
            if (statsRollout.BonusPool > 0)
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetText(HardStrings.mustDistributeBonusPoints);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
                return false;
            }
            else
            {
                leveling = false;
                PlayerEntity.Stats = statsRollout.WorkingStats;
                NativePanel.Components.Remove(statsRollout);
                return true;
            }
        }

        #endregion

        #region Event Handlers

        private void HealthButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallMessageBox healthBox = DaggerfallUI.Instance.CreateHealthStatusBox(this);
            healthBox.Show();
        }

        private void AffiliationsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ShowAffiliationsDialog();
        }

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

        private void InventoryButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenInventoryWindow);
        }

        private void LogBookButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenQuestJournalWindow);
        }

        private void StatButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!leveling)
            {
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                messageBox.SetTextTokens((int)sender.Tag, playerEntity.Stats);
                messageBox.ClickAnywhereToClose = true;
                messageBox.Show();
            }
            else
            {
                // If leveling, let the statsRollOut use the stat buttons
            }
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (leveling)
            {
                if (!CheckIfDoneLeveling())
                    return;
            }
            CloseWindow();
        }

        private void StatsRollout_OnStatChanged()
        {
            UpdateSecondaryStatLabels();
        }

        private void UpdateSecondaryStatLabels()
        {
            DaggerfallStats workingStats = statsRollout.WorkingStats;
            fatigueLabel.Text = string.Format("{0}/{1}", PlayerEntity.CurrentFatigue / 64, workingStats.LiveStrength + workingStats.LiveEndurance);
            encumbranceLabel.Text = string.Format("{0}/{1}", (int)PlayerEntity.CarriedWeight, FormulaHelper.MaxEncumbrance(workingStats.LiveStrength));
        }

        #endregion
    }
}