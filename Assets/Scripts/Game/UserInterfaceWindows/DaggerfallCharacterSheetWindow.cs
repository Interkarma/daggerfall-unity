// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
        #region Fields
        const string nativeImgName = "INFO00I0.IMG";

        private const int noAffiliationsMsgId = 19;

        StatsRollout statsRollout;

        bool isCloseWindowDeferred = false;
        bool isInventoryWindowDeferred = false;
        bool isSpellbookWindowDeferred = false;
        bool isLogbookWindowDeferred = false;
        bool isHistoryWindowDeferred = false;
        bool leveling = false;

        const int oghmaBonusPool = 30;

        SoundClips levelUpSound = SoundClips.LevelUp;

        KeyCode toggleClosedBinding;

        #endregion

        #region UI Controls

        PlayerEntity playerEntity;
        TextLabel nameLabel = new TextLabel();
        TextLabel raceLabel = new TextLabel();
        TextLabel classLabel = new TextLabel();
        TextLabel levelLabel = new TextLabel();
        TextLabel goldLabel = new TextLabel();
        TextLabel fatigueLabel = new TextLabel();
        TextLabel healthLabel = new TextLabel();
        TextLabel encumbranceLabel = new TextLabel();
        Panel[] statPanels = new Panel[DaggerfallStats.Count];
        TextLabel[] statLabels = new TextLabel[DaggerfallStats.Count];
        PaperDoll characterPortrait = new PaperDoll(showArmorValues: false);

        #endregion

        #region UI Textures

        Texture2D nativeTexture;

        #endregion

        #region Properties

        PlayerEntity PlayerEntity
        {
            get { return (playerEntity != null) ? playerEntity : playerEntity = GameManager.Instance.PlayerEntity; }
        }

        #endregion

        #region Constructors

        public DaggerfallCharacterSheetWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
            // Prevent duplicate close calls with base class's exitKey (Escape)
            AllowCancel = false;
        }

        #endregion

        #region Setup Methods

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
            Vector2 panelPos = new Vector2(141, 17);
            for (int i = 0; i < DaggerfallStats.Count; i++)
            {
                statPanels[i] = DaggerfallUI.AddPanel(new Rect(panelPos.x, panelPos.y, 28, 6), NativePanel);
                statLabels[i] = DaggerfallUI.AddDefaultShadowedTextLabel(Vector2.zero, statPanels[i]);
                statLabels[i].HorizontalAlignment = HorizontalAlignment.Center;
                panelPos.y += 24f;
            }

            // Name button
            Button nameButton = DaggerfallUI.AddButton(new Rect(4, 3, 132, 8), NativePanel);
            nameButton.OnMouseClick += NameButton_OnMouseClick;
            nameButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetName);

            // Level button
            Button levelButton = DaggerfallUI.AddButton(new Rect(4, 33, 132, 8), NativePanel);
            levelButton.OnMouseClick += LevelButton_OnMouseClick;
            levelButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetLevel);

            // Gold button
            Button goldButton = DaggerfallUI.AddButton(new Rect(4, 43, 132, 8), NativePanel);
            goldButton.OnMouseClick += GoldButton_OnMouseClick;
            goldButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetGold);

            // Health button
            Button healthButton = DaggerfallUI.AddButton(new Rect(4, 63, 128, 8), NativePanel);
            healthButton.OnMouseClick += HealthButton_OnMouseClick;
            healthButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetHealth);

            // Affiliations button
            Button affiliationsButton = DaggerfallUI.AddButton(new Rect(3, 84, 130, 8), NativePanel);
            affiliationsButton.OnMouseClick += AffiliationsButton_OnMouseClick;
            affiliationsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetAffiliations);

            // Primary skills button
            Button primarySkillsButton = DaggerfallUI.AddButton(new Rect(11, 106, 115, 8), NativePanel);
            primarySkillsButton.OnMouseClick += PrimarySkillsButton_OnMouseClick;
            primarySkillsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetPrimarySkills);

            // Major skills button
            Button majorSkillsButton = DaggerfallUI.AddButton(new Rect(11, 116, 115, 8), NativePanel);
            majorSkillsButton.OnMouseClick += MajorSkillsButton_OnMouseClick;
            majorSkillsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetMajorSkills);

            // Minor skills button
            Button minorSkillsButton = DaggerfallUI.AddButton(new Rect(11, 126, 115, 8), NativePanel);
            minorSkillsButton.OnMouseClick += MinorSkillsButton_OnMouseClick;
            minorSkillsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetMinorSkills);

            // Miscellaneous skills button
            Button miscSkillsButton = DaggerfallUI.AddButton(new Rect(11, 136, 115, 8), NativePanel);
            miscSkillsButton.OnMouseClick += MiscSkillsButton_OnMouseClick;
            miscSkillsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetMiscSkills);

            // Inventory button
            Button inventoryButton = DaggerfallUI.AddButton(new Rect(3, 151, 65, 12), NativePanel);
            inventoryButton.OnMouseClick += InventoryButton_OnMouseClick;
            inventoryButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetInventory);
            inventoryButton.OnKeyboardEvent += InventoryButton_OnKeyboardEvent;

            // Spellbook button
            Button spellBookButton = DaggerfallUI.AddButton(new Rect(69, 151, 65, 12), NativePanel);
            spellBookButton.OnMouseClick += SpellBookButton_OnMouseClick;
            spellBookButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetSpellbook);
            spellBookButton.OnKeyboardEvent += SpellBookButton_OnKeyboardEvent;

            // Logbook button
            Button logBookButton = DaggerfallUI.AddButton(new Rect(3, 165, 65, 12), NativePanel);
            logBookButton.OnMouseClick += LogBookButton_OnMouseClick;
            logBookButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetLogbook);
            logBookButton.OnKeyboardEvent += LogBookButton_OnKeyboardEvent;

            // History button
            Button historyButton = DaggerfallUI.AddButton(new Rect(69, 165, 65, 12), NativePanel);
            historyButton.OnMouseClick += HistoryButton_OnMouseClick;
            historyButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetHistory);
            historyButton.OnKeyboardEvent += HistoryButton_OnKeyboardEvent;

            // Exit button
            Button exitButton = DaggerfallUI.AddButton(new Rect(50, 179, 39, 19), NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetExit);
            exitButton.OnKeyboardEvent += ExitButton_OnKeyboardEvent;

            // Attribute popup text
            Vector2 pos = new Vector2(141, 6);
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

            // Store toggle closed binding for this window
            toggleClosedBinding = InputManager.Instance.GetBinding(InputManager.Actions.CharacterSheet);
        }

        #endregion

        #region Overrides

        public override void Update()
        {
            base.Update();

            if (DaggerfallUI.Instance.HotkeySequenceProcessed  == HotkeySequence.HotkeySequenceProcessStatus.NotFound)
            {
                // Toggle window closed with same hotkey used to open it
                if (InputManager.Instance.GetKeyUp(toggleClosedBinding) || InputManager.Instance.GetBackButtonUp())
                    if (CheckIfDoneLeveling())
                        CloseWindow();
            }
        }

        public override void OnPush()
        {
            toggleClosedBinding = InputManager.Instance.GetBinding(InputManager.Actions.CharacterSheet);
            Refresh();
        }

        public override void OnReturn()
        {
            Refresh();
        }

        public override void CancelWindow()
        {
            if (CheckIfDoneLeveling())
                base.CancelWindow();
        }

        #endregion

        #region Private Methods

        // Adds button for attribute popup text
        // NOTE: This has only partial functionality until %vars and all formatting tokens are supported
        void AddAttributePopupButton(DFCareer.Stats stat, Rect rect)
        {
            Button button = DaggerfallUI.AddButton(rect, NativePanel);
            button.Tag = DaggerfallUnity.TextProvider.GetStatDescriptionTextID(stat);
            button.OnMouseClick += StatButton_OnMouseClick;
        }

        protected virtual void ShowSkillsDialog(List<DFCareer.Skills> skills, bool twoColumn = false)
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
                    tokens.AddRange(DaggerfallUnity.TextProvider.GetSkillSummary(skills[i], 0));
                    if (i < skills.Count - 1)
                        tokens.Add(TextFile.NewLineToken);
                }
                else
                {
                    if (!secondColumn)
                    {
                        tokens.AddRange(DaggerfallUnity.TextProvider.GetSkillSummary(skills[i], 0));
                        secondColumn = !secondColumn;
                    }
                    else
                    {
                        tokens.AddRange(DaggerfallUnity.TextProvider.GetSkillSummary(skills[i], 136));
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
            messageBox.SetHighlightColor(DaggerfallUI.DaggerfallUnityStatIncreasedTextColor);
            messageBox.SetTextTokens(tokens.ToArray(), null, false);
            messageBox.ClickAnywhereToClose = true;
            messageBox.Show();
        }

        protected virtual void ShowAffiliationsDialog()
        {
            List<TextFile.Token> tokens = new List<TextFile.Token>();
            List<IGuild> guildMemberships = GameManager.Instance.GuildManager.GetMemberships();

            if (guildMemberships.Count == 0)
                DaggerfallUI.MessageBox(noAffiliationsMsgId);
            else
            {
                TextFile.Token tab = TextFile.TabToken;
                tab.x = 125;
                tokens.Add(new TextFile.Token() {
                    text = TextManager.Instance.GetLocalizedText("affiliation"),
                    formatting = TextFile.Formatting.TextHighlight
                });
                tokens.Add(tab);
                tokens.Add(new TextFile.Token()
                {
                    text = TextManager.Instance.GetLocalizedText("rank"),
                    formatting = TextFile.Formatting.TextHighlight
                });
                tokens.Add(TextFile.NewLineToken);

                foreach (IGuild guild in guildMemberships)
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
                DaggerfallUI.Instance.PlayOneShot(levelUpSound);

                int bonusPool;
                if (!PlayerEntity.OghmaLevelUp)
                {
                    bonusPool = FormulaHelper.BonusPool();
                    PlayerEntity.Level++;
                    PlayerEntity.MaxHealth = PlayerEntity.RawMaxHealth + FormulaHelper.CalculateHitPointsPerLevelUp(PlayerEntity);
                }
                else
                    bonusPool = oghmaBonusPool;

                // Add stats rollout for leveling up
                NativePanel.Components.Add(statsRollout);

                this.statsRollout.StartingStats = PlayerEntity.Stats.Clone();
                this.statsRollout.WorkingStats = PlayerEntity.Stats.Clone();
                this.statsRollout.BonusPool = bonusPool;

                PlayerEntity.ReadyToLevelUp = false;
                PlayerEntity.OghmaLevelUp = false;
            }

            // Update main labels
            nameLabel.Text = PlayerEntity.Name;
            raceLabel.Text = PlayerEntity.RaceTemplate.Name;
            classLabel.Text = PlayerEntity.Career.Name;
            levelLabel.Text = PlayerEntity.Level.ToString();
            goldLabel.Text = PlayerEntity.GetGoldAmount().ToString();
            fatigueLabel.Text = string.Format("{0}/{1}", PlayerEntity.CurrentFatigue / DaggerfallEntity.FatigueMultiplier, PlayerEntity.MaxFatigue / DaggerfallEntity.FatigueMultiplier);
            healthLabel.Text = string.Format("{0}/{1}", PlayerEntity.CurrentHealth, PlayerEntity.MaxHealth);
            encumbranceLabel.Text = string.Format("{0}/{1}", (int)PlayerEntity.CarriedWeight, PlayerEntity.MaxEncumbrance);

            // Update stat labels
            for (int i = 0; i < DaggerfallStats.Count; i++)
            {
                if (!leveling)
                    statLabels[i].Text = PlayerEntity.Stats.GetLiveStatValue(i).ToString();
                else
                    statLabels[i].Text = ""; // If leveling, statsRollout will fill in the stat labels.

                // Handle stat colour changes
                if (PlayerEntity.Stats.GetLiveStatValue(i) < PlayerEntity.Stats.GetPermanentStatValue(i))
                    statLabels[i].TextColor = DaggerfallUI.DaggerfallUnityStatDrainedTextColor;
                else if (PlayerEntity.Stats.GetLiveStatValue(i) > PlayerEntity.Stats.GetPermanentStatValue(i))
                    statLabels[i].TextColor = DaggerfallUI.DaggerfallUnityStatIncreasedTextColor;
                else
                    statLabels[i].TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
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
            if (leveling)
            {
                if (statsRollout.BonusPool > 0 && !statsRollout.WorkingStats.IsAllMax())
                {
                    DaggerfallMessageBox messageBox = new DaggerfallMessageBox(uiManager, this);
                    messageBox.SetText(TextManager.Instance.GetLocalizedText("mustDistributeBonusPoints"));
                    messageBox.ClickAnywhereToClose = true;
                    messageBox.Show();
                    return false;
                }
                else
                {
                    leveling = false;
                    PlayerEntity.Stats = statsRollout.WorkingStats;
                    NativePanel.Components.Remove(statsRollout);
                }
            }
            else
            {
                playerEntity.ResetSkillsRecentlyRaised();
            }
            return true;
        }

        string[] GetClassSpecials()
        {
            List<string> specials = new List<string>();
            DFCareer career = GameManager.Instance.PlayerEntity.Career;
            RaceTemplate race = GameManager.Instance.PlayerEntity.RaceTemplate;

            // Tolerances
            Dictionary<DFCareer.Tolerance, string> tolerances = new Dictionary<DFCareer.Tolerance, string>
            {
                { DFCareer.Tolerance.CriticalWeakness, TextManager.Instance.GetLocalizedText(HardStrings.criticalWeakness) },
                { DFCareer.Tolerance.Immune, TextManager.Instance.GetLocalizedText(HardStrings.immunity) },
                { DFCareer.Tolerance.LowTolerance, TextManager.Instance.GetLocalizedText(HardStrings.lowTolerance) },
                { DFCareer.Tolerance.Resistant, TextManager.Instance.GetLocalizedText(HardStrings.resistance) }
            };

            if (career.Paralysis != DFCareer.Tolerance.Normal)
                specials.Add(tolerances[career.Paralysis] + " " + TextManager.Instance.GetLocalizedText(HardStrings.toParalysis));

            if (career.Magic != DFCareer.Tolerance.Normal)
                specials.Add(tolerances[career.Magic] + " " + TextManager.Instance.GetLocalizedText(HardStrings.toMagic));

            if (career.Poison != DFCareer.Tolerance.Normal)
                specials.Add(tolerances[career.Poison] + " " + TextManager.Instance.GetLocalizedText(HardStrings.toPoison));

            if (career.Fire != DFCareer.Tolerance.Normal)
                specials.Add(tolerances[career.Fire] + " " + TextManager.Instance.GetLocalizedText(HardStrings.toFire));

            if (career.Frost != DFCareer.Tolerance.Normal)
                specials.Add(tolerances[career.Frost] + " " + TextManager.Instance.GetLocalizedText(HardStrings.toFrost));

            if (career.Shock != DFCareer.Tolerance.Normal)
                specials.Add(tolerances[career.Shock] + " " + TextManager.Instance.GetLocalizedText(HardStrings.toShock));

            if (career.Disease != DFCareer.Tolerance.Normal)
                specials.Add(tolerances[career.Disease] + " " + TextManager.Instance.GetLocalizedText(HardStrings.toDisease));

            // Weapon Proficiencies
            Dictionary<DFCareer.Proficiency, string> profs = new Dictionary<DFCareer.Proficiency, string>
            {
                { DFCareer.Proficiency.Expert, TextManager.Instance.GetLocalizedText(HardStrings.expertiseIn) },
                { DFCareer.Proficiency.Forbidden, TextManager.Instance.GetLocalizedText(HardStrings.forbiddenWeaponry) }
            };

            if (career.ShortBlades != DFCareer.Proficiency.Normal)
                specials.Add(profs[career.ShortBlades] + " " + TextManager.Instance.GetLocalizedText(HardStrings.shortBlade));

            if (career.LongBlades != DFCareer.Proficiency.Normal)
                specials.Add(profs[career.LongBlades] + " " + TextManager.Instance.GetLocalizedText(HardStrings.longBlade));

            if (career.HandToHand != DFCareer.Proficiency.Normal)
                specials.Add(profs[career.HandToHand] + " " + TextManager.Instance.GetLocalizedText(HardStrings.handToHand));

            if (career.Axes != DFCareer.Proficiency.Normal)
                specials.Add(profs[career.Axes] + " " + TextManager.Instance.GetLocalizedText(HardStrings.axe));

            if (career.BluntWeapons != DFCareer.Proficiency.Normal)
                specials.Add(profs[career.BluntWeapons] + " " + TextManager.Instance.GetLocalizedText(HardStrings.bluntWeapon));

            if (career.MissileWeapons != DFCareer.Proficiency.Normal)
                specials.Add(profs[career.MissileWeapons] + " " + TextManager.Instance.GetLocalizedText(HardStrings.missileWeapon));

            // Attack modifiers
            Dictionary<DFCareer.AttackModifier, string> atkMods = new Dictionary<DFCareer.AttackModifier, string>
            {
                { DFCareer.AttackModifier.Bonus, TextManager.Instance.GetLocalizedText(HardStrings.bonusToHit) },
                { DFCareer.AttackModifier.Phobia, TextManager.Instance.GetLocalizedText(HardStrings.phobia) }
            };

            if (career.UndeadAttackModifier != DFCareer.AttackModifier.Normal)
                specials.Add(atkMods[career.UndeadAttackModifier] + " " + TextManager.Instance.GetLocalizedText(HardStrings.undead));

            if (career.DaedraAttackModifier != DFCareer.AttackModifier.Normal)
                specials.Add(atkMods[career.DaedraAttackModifier] + " " + TextManager.Instance.GetLocalizedText(HardStrings.daedra));

            if (career.HumanoidAttackModifier != DFCareer.AttackModifier.Normal)
                specials.Add(atkMods[career.HumanoidAttackModifier] + " " + TextManager.Instance.GetLocalizedText(HardStrings.humanoid));

            if (career.AnimalsAttackModifier != DFCareer.AttackModifier.Normal)
                specials.Add(atkMods[career.AnimalsAttackModifier] + " " + TextManager.Instance.GetLocalizedText(HardStrings.animals));

            // Darkness/light powered magery
            if (career.DarknessPoweredMagery != DFCareer.DarknessMageryFlags.Normal)
            {
                if ((career.DarknessPoweredMagery & DFCareer.DarknessMageryFlags.ReducedPowerInLight) == DFCareer.DarknessMageryFlags.ReducedPowerInLight)
                    specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.darknessPoweredMagery) + " " + TextManager.Instance.GetLocalizedText(HardStrings.lowerMagicAbilityDaylight));
                if ((career.DarknessPoweredMagery & DFCareer.DarknessMageryFlags.UnableToCastInLight) == DFCareer.DarknessMageryFlags.UnableToCastInLight)
                    specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.darknessPoweredMagery) + " " + TextManager.Instance.GetLocalizedText(HardStrings.unableToUseMagicInDaylight));
            }

            if (career.LightPoweredMagery != DFCareer.LightMageryFlags.Normal)
            {
                if ((career.LightPoweredMagery & DFCareer.LightMageryFlags.ReducedPowerInDarkness) == DFCareer.LightMageryFlags.ReducedPowerInDarkness)
                    specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.lightPoweredMagery) + " " + TextManager.Instance.GetLocalizedText(HardStrings.lowerMagicAbilityDarkness));
                if ((career.LightPoweredMagery & DFCareer.LightMageryFlags.UnableToCastInDarkness) == DFCareer.LightMageryFlags.UnableToCastInDarkness)
                    specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.lightPoweredMagery) + " " + TextManager.Instance.GetLocalizedText(HardStrings.unableToUseMagicInDarkness));
            }

            // Forbidden materials (multiple)
            if (career.ForbiddenMaterials != 0)
            {
                Dictionary<DFCareer.MaterialFlags, string> forbMaterials = new Dictionary<DFCareer.MaterialFlags, string>
                {
                    { DFCareer.MaterialFlags.Adamantium, TextManager.Instance.GetLocalizedText(HardStrings.adamantium) },
                    { DFCareer.MaterialFlags.Daedric, TextManager.Instance.GetLocalizedText(HardStrings.daedric) },
                    { DFCareer.MaterialFlags.Dwarven, TextManager.Instance.GetLocalizedText(HardStrings.dwarven) },
                    { DFCareer.MaterialFlags.Ebony, TextManager.Instance.GetLocalizedText(HardStrings.ebony) },
                    { DFCareer.MaterialFlags.Elven, TextManager.Instance.GetLocalizedText(HardStrings.elven) },
                    { DFCareer.MaterialFlags.Iron, TextManager.Instance.GetLocalizedText(HardStrings.iron) },
                    { DFCareer.MaterialFlags.Mithril, TextManager.Instance.GetLocalizedText(HardStrings.mithril) },
                    { DFCareer.MaterialFlags.Orcish, TextManager.Instance.GetLocalizedText(HardStrings.orcish) },
                    { DFCareer.MaterialFlags.Silver, TextManager.Instance.GetLocalizedText(HardStrings.silver) },
                    { DFCareer.MaterialFlags.Steel, TextManager.Instance.GetLocalizedText(HardStrings.steel) }
                };
                foreach (DFCareer.MaterialFlags flag in Enum.GetValues(typeof(DFCareer.MaterialFlags)))
                {
                    if ((career.ForbiddenMaterials & flag) == flag)
                        specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.forbiddenMaterial) + " " + forbMaterials[flag]);
                }
            }

            // Forbidden shields (multiple)
            if (career.ForbiddenShields != 0)
            {
                Dictionary<DFCareer.ShieldFlags, string> forbShields = new Dictionary<DFCareer.ShieldFlags, string>
                {
                    { DFCareer.ShieldFlags.Buckler, TextManager.Instance.GetLocalizedText(HardStrings.buckler) },
                    { DFCareer.ShieldFlags.KiteShield, TextManager.Instance.GetLocalizedText(HardStrings.kiteShield) },
                    { DFCareer.ShieldFlags.RoundShield, TextManager.Instance.GetLocalizedText(HardStrings.roundShield) },
                    { DFCareer.ShieldFlags.TowerShield, TextManager.Instance.GetLocalizedText(HardStrings.towerShield) }
                };
                foreach (DFCareer.ShieldFlags flag in Enum.GetValues(typeof(DFCareer.ShieldFlags)))
                {
                    if ((career.ForbiddenShields & flag) == flag)
                        specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.forbiddenShieldTypes) + " " + forbShields[flag]);
                }
            }

            // Forbidden armor (multiple)
            if (career.ForbiddenArmors != 0)
            {
                Dictionary<DFCareer.ArmorFlags, string> forbArmors = new Dictionary<DFCareer.ArmorFlags, string>
                {
                    { DFCareer.ArmorFlags.Chain, TextManager.Instance.GetLocalizedText(HardStrings.chain) },
                    { DFCareer.ArmorFlags.Leather, TextManager.Instance.GetLocalizedText(HardStrings.leather) },
                    { DFCareer.ArmorFlags.Plate, TextManager.Instance.GetLocalizedText(HardStrings.plate) }
                };
                foreach (DFCareer.ArmorFlags flag in Enum.GetValues(typeof(DFCareer.ArmorFlags)))
                {
                    if ((career.ForbiddenArmors & flag) == flag)
                        specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.forbiddenArmorType) + " " + forbArmors[flag]);
                }
            }

            // Forbidden proficiencies (flags)
            // Expert proficiencies (flags)
            // Omitted - redundant

            // Spell point multiplier
            if (career.SpellPointMultiplier != DFCareer.SpellPointMultipliers.Times_0_50)
            {
                Dictionary<DFCareer.SpellPointMultipliers, string> spellPtMults = new Dictionary<DFCareer.SpellPointMultipliers, string>
                {
                    { DFCareer.SpellPointMultipliers.Times_1_00, TextManager.Instance.GetLocalizedText(HardStrings.intInSpellPoints) },
                    { DFCareer.SpellPointMultipliers.Times_1_50, TextManager.Instance.GetLocalizedText(HardStrings.intInSpellPoints15) },
                    { DFCareer.SpellPointMultipliers.Times_1_75, TextManager.Instance.GetLocalizedText(HardStrings.intInSpellPoints175) },
                    { DFCareer.SpellPointMultipliers.Times_2_00, TextManager.Instance.GetLocalizedText(HardStrings.intInSpellPoints2) },
                    { DFCareer.SpellPointMultipliers.Times_3_00, TextManager.Instance.GetLocalizedText(HardStrings.intInSpellPoints3) },
                };
                specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.increasedMagery) + " " + spellPtMults[career.SpellPointMultiplier]);
            }

            // Spell absorption
            if (career.SpellAbsorption != DFCareer.SpellAbsorptionFlags.None)
            {
                Dictionary<DFCareer.SpellAbsorptionFlags, string> absorbConds = new Dictionary<DFCareer.SpellAbsorptionFlags, string>
                {
                    { DFCareer.SpellAbsorptionFlags.Always, TextManager.Instance.GetLocalizedText(HardStrings.general) },
                    { DFCareer.SpellAbsorptionFlags.InDarkness, TextManager.Instance.GetLocalizedText(HardStrings.inDarkness) },
                    { DFCareer.SpellAbsorptionFlags.InLight, TextManager.Instance.GetLocalizedText(HardStrings.inLight) }
                };
                specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.spellAbsorption) + " " + absorbConds[career.SpellAbsorption]);
            }

            // Spell point regeneration
            if (career.NoRegenSpellPoints)
                specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.inabilityToRegen));

            // Talents
            if (career.AcuteHearing)
                specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.acuteHearing));

            if (career.Athleticism)
                specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.athleticism));

            if (career.AdrenalineRush)
                specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.adrenalineRush));

            // Regeneration and rapid healing
            if (career.Regeneration != DFCareer.RegenerationFlags.None)
            {
                Dictionary<DFCareer.RegenerationFlags, string> regenConds = new Dictionary<DFCareer.RegenerationFlags, string>
                {
                    { DFCareer.RegenerationFlags.Always, TextManager.Instance.GetLocalizedText(HardStrings.general) },
                    { DFCareer.RegenerationFlags.InDarkness, TextManager.Instance.GetLocalizedText(HardStrings.inDarkness) },
                    { DFCareer.RegenerationFlags.InLight, TextManager.Instance.GetLocalizedText(HardStrings.inLight) },
                    { DFCareer.RegenerationFlags.InWater, TextManager.Instance.GetLocalizedText(HardStrings.whileImmersed) }
                };
                specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.regenerateHealth) + " " + regenConds[career.Regeneration]);
            }

            if (career.RapidHealing != DFCareer.RapidHealingFlags.None)
            {
                Dictionary<DFCareer.RapidHealingFlags, string> rapidHealingConds = new Dictionary<DFCareer.RapidHealingFlags, string>
                {
                    { DFCareer.RapidHealingFlags.Always, TextManager.Instance.GetLocalizedText(HardStrings.general) },
                    { DFCareer.RapidHealingFlags.InDarkness, TextManager.Instance.GetLocalizedText(HardStrings.inDarkness) },
                    { DFCareer.RapidHealingFlags.InLight, TextManager.Instance.GetLocalizedText(HardStrings.inLight) }
                };
                specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.rapidHealing) + " " + rapidHealingConds[career.RapidHealing]);
            }

            // Damage
            if (career.DamageFromSunlight)
                specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.damage) + " " + TextManager.Instance.GetLocalizedText(HardStrings.fromSunlight));

            if (career.DamageFromHolyPlaces)
                specials.Add(TextManager.Instance.GetLocalizedText(HardStrings.damage) + " " + TextManager.Instance.GetLocalizedText(HardStrings.fromHolyPlaces));

            // Add racial tolerances and abilities
            Dictionary<DFCareer.EffectFlags, string> raceEffectMods = new Dictionary<DFCareer.EffectFlags, string>
            {
                { DFCareer.EffectFlags.Paralysis, TextManager.Instance.GetLocalizedText(HardStrings.toParalysis) },
                { DFCareer.EffectFlags.Magic, TextManager.Instance.GetLocalizedText(HardStrings.toMagic) },
                { DFCareer.EffectFlags.Poison, TextManager.Instance.GetLocalizedText(HardStrings.toPoison) },
                { DFCareer.EffectFlags.Fire, TextManager.Instance.GetLocalizedText(HardStrings.toFire) },
                { DFCareer.EffectFlags.Frost, TextManager.Instance.GetLocalizedText(HardStrings.toFrost) },
                { DFCareer.EffectFlags.Shock, TextManager.Instance.GetLocalizedText(HardStrings.toShock) },
                { DFCareer.EffectFlags.Disease, TextManager.Instance.GetLocalizedText(HardStrings.toDisease) },
            };
            foreach (DFCareer.EffectFlags effectFlag in Enum.GetValues(typeof(DFCareer.EffectFlags)))
            {
                if (effectFlag != DFCareer.EffectFlags.None)
                {
                    // Resistances
                    if ((race.ResistanceFlags & effectFlag) == effectFlag)
                    { 
                        string toAdd = TextManager.Instance.GetLocalizedText(HardStrings.resistance) + " " + raceEffectMods[effectFlag];
                        if (!specials.Contains(toAdd)) // prevent duplicates from career
                        {
                            specials.Add(toAdd);
                        }
                    }
                    // Immunities
                    if ((race.ImmunityFlags & effectFlag) == effectFlag)
                    {
                        string toAdd = TextManager.Instance.GetLocalizedText(HardStrings.immunity) + " " + raceEffectMods[effectFlag];
                        if (!specials.Contains(toAdd))
                        {
                            specials.Add(toAdd);
                        }
                    }
                    // Low tolerances
                    if ((race.LowToleranceFlags & effectFlag) == effectFlag)
                    {
                        string toAdd = TextManager.Instance.GetLocalizedText(HardStrings.lowTolerance) + " " + raceEffectMods[effectFlag];
                        if (!specials.Contains(toAdd))
                        {
                            specials.Add(toAdd);
                        }
                    }
                    // Critical weaknesses
                    if ((race.CriticalWeaknessFlags & effectFlag) == effectFlag)
                    {
                        string toAdd = TextManager.Instance.GetLocalizedText(HardStrings.criticalWeakness) + " " + raceEffectMods[effectFlag];
                        if (!specials.Contains(toAdd))
                        {
                            specials.Add(toAdd);
                        }
                    }
                }
            }

            Dictionary<DFCareer.SpecialAbilityFlags, string> raceAbilities = new Dictionary<DFCareer.SpecialAbilityFlags, string>
            {
                { DFCareer.SpecialAbilityFlags.AcuteHearing, TextManager.Instance.GetLocalizedText(HardStrings.acuteHearing) },
                { DFCareer.SpecialAbilityFlags.Athleticism, TextManager.Instance.GetLocalizedText(HardStrings.acuteHearing) },
                { DFCareer.SpecialAbilityFlags.AdrenalineRush, TextManager.Instance.GetLocalizedText(HardStrings.adrenalineRush) },
                { DFCareer.SpecialAbilityFlags.NoRegenSpellPoints, TextManager.Instance.GetLocalizedText(HardStrings.inabilityToRegen) },
                { DFCareer.SpecialAbilityFlags.SunDamage, TextManager.Instance.GetLocalizedText(HardStrings.damage) + " " + TextManager.Instance.GetLocalizedText(HardStrings.fromSunlight) },
                { DFCareer.SpecialAbilityFlags.HolyDamage, TextManager.Instance.GetLocalizedText(HardStrings.damage) + " " + TextManager.Instance.GetLocalizedText(HardStrings.fromHolyPlaces) }
            };
            foreach (DFCareer.SpecialAbilityFlags abilityFlag in Enum.GetValues(typeof(DFCareer.SpecialAbilityFlags)))
            {
                if (abilityFlag != DFCareer.SpecialAbilityFlags.None && (race.SpecialAbilities & abilityFlag) == abilityFlag)
                {
                    string toAdd = raceAbilities[abilityFlag];
                    if (!specials.Contains(toAdd))
                    {
                        specials.Add(toAdd);
                    }
                }
            }

            return specials.ToArray();
        }

        #endregion

        #region Event Handlers

        private void NameButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallInputMessageBox mb = new DaggerfallInputMessageBox(uiManager, this);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            mb.TextBox.Text = nameLabel.Text;
            mb.SetTextBoxLabel(TextManager.Instance.GetLocalizedText("enterNewName"));
            mb.OnGotUserInput += EnterName_OnGotUserInput;
            mb.Show();
        }

        private void LevelButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            float currentLevel = (playerEntity.CurrentLevelUpSkillSum - playerEntity.StartingLevelUpSkillSum + 28f) / 15f;
            int progress = (int)((currentLevel % 1) * 100);
            DaggerfallUI.MessageBox(string.Format(TextManager.Instance.GetLocalizedText("levelProgress"), progress));
        }

        private void GoldButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaggerfallMessageBox bankingStatusBox = DaggerfallBankingWindow.CreateBankingStatusBox(this);
            bankingStatusBox.Show();
        }

        private void EnterName_OnGotUserInput(DaggerfallInputMessageBox sender, string input)
        {
            if (input.Length > 0)
                PlayerEntity.Name = input;
            UpdatePlayerValues();
        }

        private void HealthButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaggerfallMessageBox healthBox = DaggerfallUI.Instance.CreateHealthStatusBox(this);
            healthBox.Show();
        }

        private void AffiliationsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ShowAffiliationsDialog();
        }

        private void PrimarySkillsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ShowSkillsDialog(PlayerEntity.GetPrimarySkills());
        }

        private void MajorSkillsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ShowSkillsDialog(PlayerEntity.GetMajorSkills());
        }

        private void MinorSkillsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ShowSkillsDialog(PlayerEntity.GetMinorSkills());
        }

        private void MiscSkillsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ShowSkillsDialog(PlayerEntity.GetMiscSkills(), true);
        }

        private void InventoryButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenInventoryWindow);
        }

        void InventoryButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isInventoryWindowDeferred = true;
            } 
            else if (keyboardEvent.type == EventType.KeyUp && isInventoryWindowDeferred)
            {
                isInventoryWindowDeferred = false;
                uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenInventoryWindow);
            }
        }

        private void SpellBookButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenSpellBookWindow);
        }

        void SpellBookButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isSpellbookWindowDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isSpellbookWindowDeferred)
            {
                isSpellbookWindowDeferred = false;
                uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenSpellBookWindow);
            }
        }

        private void LogBookButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenQuestJournalWindow);
        }

        void LogBookButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isLogbookWindowDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isLogbookWindowDeferred)
            {
                isLogbookWindowDeferred = false;
                uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenQuestJournalWindow);
            }
        }

        void HistoryButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaggerfallMessageBox advantageTextBox = DaggerfallUI.MessageBox(GetClassSpecials());
            advantageTextBox.OnClose += AdvantageTextBox_OnClose;
        }

        void HistoryButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isHistoryWindowDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isHistoryWindowDeferred)
            {
                isHistoryWindowDeferred = false;
                DaggerfallMessageBox advantageTextBox = DaggerfallUI.MessageBox(GetClassSpecials());
                advantageTextBox.OnClose += AdvantageTextBox_OnClose;
            }
        }

        void AdvantageTextBox_OnClose()
        {
            uiManager.PostMessage(DaggerfallUIMessages.dfuiOpenPlayerHistoryWindow);
        }

        private void StatButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (!leveling)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            if (CheckIfDoneLeveling())
                CloseWindow();
        }

        protected void ExitButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                if (CheckIfDoneLeveling())
                    isCloseWindowDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isCloseWindowDeferred)
            {
                isCloseWindowDeferred = false;
                CloseWindow();
            }
        }


        private void StatsRollout_OnStatChanged()
        {
            UpdateSecondaryStatLabels();
        }

        private void UpdateSecondaryStatLabels()
        {
            DaggerfallStats workingStats = statsRollout.WorkingStats;
            fatigueLabel.Text = string.Format("{0}/{1}", PlayerEntity.CurrentFatigue / DaggerfallEntity.FatigueMultiplier, workingStats.LiveStrength + workingStats.LiveEndurance);
            encumbranceLabel.Text = string.Format("{0}/{1}", (int)PlayerEntity.CarriedWeight, FormulaHelper.MaxEncumbrance(workingStats.LiveStrength));
        }

        #endregion
    }
}
