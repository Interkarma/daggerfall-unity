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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallEffectSettingsEditorWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        const int spinnerWidth = 24;
        const int spinnerHeight = 16;

        Rect spinnerUpButtonRect = new Rect(0, 0, spinnerWidth, 5);
        Rect spinnerDownButtonRect = new Rect(0, 11, spinnerWidth, 5);
        Rect spinnerValueLabelRect = new Rect(0, 5, spinnerWidth, 6);
        Rect durationBaseSpinnerRect = new Rect(64, 94, spinnerWidth, spinnerHeight);
        Rect durationPlusSpinnerRect = new Rect(104, 94, spinnerWidth, spinnerHeight);
        Rect durationPerLevelSpinnerRect = new Rect(160, 94, spinnerWidth, spinnerHeight);
        Rect chanceBaseSpinnerRect = new Rect(64, 114, spinnerWidth, spinnerHeight);
        Rect chancePlusSpinnerRect = new Rect(104, 114, spinnerWidth, spinnerHeight);
        Rect chancePerLevelSpinnerRect = new Rect(160, 114, spinnerWidth, spinnerHeight);
        Rect magnitudeBaseMinSpinnerRect = new Rect(64, 134, spinnerWidth, spinnerHeight);
        Rect magnitudeBaseMaxSpinnerRect = new Rect(104, 134, spinnerWidth, spinnerHeight);
        Rect magnitudePlusMinSpinnerRect = new Rect(144, 134, spinnerWidth, spinnerHeight);
        Rect magnitudePlusMaxSpinnerRect = new Rect(184, 134, spinnerWidth, spinnerHeight);
        Rect magnitudePerLevelSpinnerRect = new Rect(235, 134, spinnerWidth, spinnerHeight);

        Rect exitButtonRect = new Rect(281, 94, 24, 16);

        #endregion

        #region UI Controls

        Panel descriptionPanel;
        MultiFormatTextLabel descriptionLabel;

        UpDownSpinner durationBaseSpinner;
        UpDownSpinner durationPlusSpinner;
        UpDownSpinner durationPerLevelSpinner;
        UpDownSpinner chanceBaseSpinner;
        UpDownSpinner chancePlusSpinner;
        UpDownSpinner chancePerLevelSpinner;
        UpDownSpinner magnitudeBaseMinSpinner;
        UpDownSpinner magnitudeBaseMaxSpinner;
        UpDownSpinner magnitudePlusMinSpinner;
        UpDownSpinner magnitudePlusMaxSpinner;
        UpDownSpinner magnitudePerLevelSpinner;

        Button exitButton;

        #endregion

        #region UI Textures

        Texture2D baseTexture;

        #endregion

        #region Fields

        const string baseTextureFilename = "MASK05I0.IMG";
        const string noEffectTemplateError = "DaggerfallEffectSettingsEditorWindow does not have an EffectTemplate set.";

        const int alternateAlphaIndex = 12;

        IEntityEffect effectTemplate = null;
        bool userExit;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets IEntityEffect template for effect editor.
        /// </summary>
        public IEntityEffect EffectTemplate
        {
            get { return effectTemplate; }
            set { SetEffectTemplate(value); }
        }

        /// <summary>
        /// Gets or sets effect entry details after using settings window.
        /// Setting entry will also assign EffectTemplate based on entry key.
        /// </summary>
        public EffectEntry EffectEntry
        {
            get { return GetEffectEntry(); }
            set { SetEffectEntry(value); }
        }

        /// <summary>
        /// Set to true when user clicks "exit" button to accept changes.
        /// If user cancels settings UI with escape key this will be false.
        /// </summary>
        public bool UserExit
        {
            get { return userExit; }
        }

        #endregion

        #region Constructors

        public DaggerfallEffectSettingsEditorWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load all the textures used by effect editor window
            LoadTextures();

            // Setup native panel background
            NativePanel.BackgroundTexture = baseTexture;

            // Setup controls
            SetupEffectDescriptionPanels();
            SetupSpinners();
            SetupButtons();
            InitControlState();

            UpdateCosts();
        }

        public override void OnPush()
        {
            userExit = false;
            
            if (IsSetup)
            {
                InitControlState();
                UpdateCosts();
            }
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            // Load source textures
            baseTexture = ImageReader.GetTexture(baseTextureFilename, 0, 0, true, alternateAlphaIndex);
        }

        void SetupEffectDescriptionPanels()
        {
            // Create parent panel to house effect description
            Panel descriptionParentPanel = DaggerfallUI.AddPanel(new Rect(5, 19, 312, 69), NativePanel);
            descriptionParentPanel.HorizontalAlignment = HorizontalAlignment.Center;

            // Create description panel centred inside of parent
            descriptionPanel = DaggerfallUI.AddPanel(descriptionParentPanel, AutoSizeModes.None);
            descriptionPanel.Size = new Vector2(306, 69);
            descriptionPanel.HorizontalAlignment = HorizontalAlignment.Center;
            descriptionPanel.VerticalAlignment = VerticalAlignment.Middle;
            DaggerfallUI.Instance.SetDaggerfallPopupStyle(DaggerfallUI.PopupStyle.Parchment, descriptionPanel);

            // Create multiline label for description text
            descriptionLabel = new MultiFormatTextLabel();
            descriptionLabel.HorizontalAlignment = HorizontalAlignment.Center;
            descriptionLabel.VerticalAlignment = VerticalAlignment.Middle;
            descriptionPanel.Components.Add(descriptionLabel);
        }

        void SetupSpinners()
        {
            // Add spinner controls
            durationBaseSpinner = new UpDownSpinner(durationBaseSpinnerRect, spinnerUpButtonRect, spinnerDownButtonRect, spinnerValueLabelRect, 0, null, NativePanel);
            durationPlusSpinner = new UpDownSpinner(durationPlusSpinnerRect, spinnerUpButtonRect, spinnerDownButtonRect, spinnerValueLabelRect, 0, null, NativePanel);
            durationPerLevelSpinner = new UpDownSpinner(durationPerLevelSpinnerRect, spinnerUpButtonRect, spinnerDownButtonRect, spinnerValueLabelRect, 0, null, NativePanel);
            chanceBaseSpinner = new UpDownSpinner(chanceBaseSpinnerRect, spinnerUpButtonRect, spinnerDownButtonRect, spinnerValueLabelRect, 0, null, NativePanel);
            chancePlusSpinner = new UpDownSpinner(chancePlusSpinnerRect, spinnerUpButtonRect, spinnerDownButtonRect, spinnerValueLabelRect, 0, null, NativePanel);
            chancePerLevelSpinner = new UpDownSpinner(chancePerLevelSpinnerRect, spinnerUpButtonRect, spinnerDownButtonRect, spinnerValueLabelRect, 0, null, NativePanel);
            magnitudeBaseMinSpinner = new UpDownSpinner(magnitudeBaseMinSpinnerRect, spinnerUpButtonRect, spinnerDownButtonRect, spinnerValueLabelRect, 0, null, NativePanel);
            magnitudeBaseMaxSpinner = new UpDownSpinner(magnitudeBaseMaxSpinnerRect, spinnerUpButtonRect, spinnerDownButtonRect, spinnerValueLabelRect, 0, null, NativePanel);
            magnitudePlusMinSpinner = new UpDownSpinner(magnitudePlusMinSpinnerRect, spinnerUpButtonRect, spinnerDownButtonRect, spinnerValueLabelRect, 0, null, NativePanel);
            magnitudePlusMaxSpinner = new UpDownSpinner(magnitudePlusMaxSpinnerRect, spinnerUpButtonRect, spinnerDownButtonRect, spinnerValueLabelRect, 0, null, NativePanel);
            magnitudePerLevelSpinner = new UpDownSpinner(magnitudePerLevelSpinnerRect, spinnerUpButtonRect, spinnerDownButtonRect, spinnerValueLabelRect, 0, null, NativePanel);

            // Set spinner ranges
            durationBaseSpinner.SetRange(1, 60);
            durationPlusSpinner.SetRange(1, 60);
            durationPerLevelSpinner.SetRange(1, 20);
            chanceBaseSpinner.SetRange(1, 100);
            chancePlusSpinner.SetRange(1, 100);
            chancePerLevelSpinner.SetRange(1, 20);
            magnitudeBaseMinSpinner.SetRange(1, 100);
            magnitudeBaseMaxSpinner.SetRange(1, 100);
            magnitudePlusMinSpinner.SetRange(1, 100);
            magnitudePlusMaxSpinner.SetRange(1, 100);
            magnitudePerLevelSpinner.SetRange(1, 20);

            // Set spinner events
            durationBaseSpinner.OnValueChanged += DurationBaseSpinner_OnValueChanged;
            durationPlusSpinner.OnValueChanged += DurationPlusSpinner_OnValueChanged;
            durationPerLevelSpinner.OnValueChanged += DurationPerLevelSpinner_OnValueChanged;
            chanceBaseSpinner.OnValueChanged += ChanceBaseSpinner_OnValueChanged;
            chancePlusSpinner.OnValueChanged += ChancePlusSpinner_OnValueChanged;
            chancePerLevelSpinner.OnValueChanged += ChancePerLevelSpinner_OnValueChanged;
            magnitudeBaseMinSpinner.OnValueChanged += MagnitudeBaseMinSpinner_OnValueChanged;
            magnitudeBaseMaxSpinner.OnValueChanged += MagnitudeBaseMaxSpinner_OnValueChanged;
            magnitudePlusMinSpinner.OnValueChanged += MagnitudePlusMinSpinner_OnValueChanged;
            magnitudePlusMaxSpinner.OnValueChanged += MagnitudePlusMaxSpinner_OnValueChanged;
        }

        void SetupButtons()
        {
            // Exit button
            exitButton = DaggerfallUI.AddButton(exitButtonRect, NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
        }

        void InitControlState()
        {
            // Must have an effect template set
            if (EffectTemplate == null)
                throw new Exception(noEffectTemplateError);

            // Get description text
            TextFile.Token[] descriptionTokens = EffectTemplate.Properties.SpellMakerDescription;
            if (descriptionTokens == null || descriptionTokens.Length == 0)
                throw new Exception(string.Format("DaggerfallEffectSettingsEditorWindow: EffectTemplate {0} does not present any spellmaker description text.", EffectTemplate.Key));
            else
                descriptionLabel.SetText(descriptionTokens);

            // Duration support
            if (EffectTemplate.Properties.SupportDuration)
            {
                durationBaseSpinner.Enabled = true;
                durationPlusSpinner.Enabled = true;
                durationPerLevelSpinner.Enabled = true;
            }
            else
            {
                durationBaseSpinner.Enabled = false;
                durationPlusSpinner.Enabled = false;
                durationPerLevelSpinner.Enabled = false;
            }

            // Chance support
            if (EffectTemplate.Properties.SupportChance)
            {
                chanceBaseSpinner.Enabled = true;
                chancePlusSpinner.Enabled = true;
                chancePerLevelSpinner.Enabled = true;
            }
            else
            {
                chanceBaseSpinner.Enabled = false;
                chancePlusSpinner.Enabled = false;
                chancePerLevelSpinner.Enabled = false;
            }

            // Magnitude support
            if (EffectTemplate.Properties.SupportMagnitude)
            {
                magnitudeBaseMinSpinner.Enabled = true;
                magnitudeBaseMaxSpinner.Enabled = true;
                magnitudePlusMinSpinner.Enabled = true;
                magnitudePlusMaxSpinner.Enabled = true;
                magnitudePerLevelSpinner.Enabled = true;
            }
            else
            {
                magnitudeBaseMinSpinner.Enabled = false;
                magnitudeBaseMaxSpinner.Enabled = false;
                magnitudePlusMinSpinner.Enabled = false;
                magnitudePlusMaxSpinner.Enabled = false;
                magnitudePerLevelSpinner.Enabled = false;
            }
        }

        EffectEntry GetEffectEntry()
        {
            if (!IsSetup)
                return new EffectEntry();

            // Must have an effect template set
            if (EffectTemplate == null)
                throw new Exception(noEffectTemplateError);

            // Create settings for effect from UI controls
            EffectSettings effectSettings = new EffectSettings();
            effectSettings.DurationBase = durationBaseSpinner.Value;
            effectSettings.DurationPlus = durationPlusSpinner.Value;
            effectSettings.DurationPerLevel = durationPerLevelSpinner.Value;
            effectSettings.ChanceBase = chanceBaseSpinner.Value;
            effectSettings.ChancePlus = chancePlusSpinner.Value;
            effectSettings.ChancePerLevel = chancePerLevelSpinner.Value;
            effectSettings.MagnitudeBaseMin = magnitudeBaseMinSpinner.Value;
            effectSettings.MagnitudeBaseMax = magnitudeBaseMaxSpinner.Value;
            effectSettings.MagnitudePlusMin = magnitudePlusMinSpinner.Value;
            effectSettings.MagnitudePlusMax = magnitudePlusMaxSpinner.Value;
            effectSettings.MagnitudePerLevel = magnitudePerLevelSpinner.Value;

            // Create entry
            EffectEntry effectEntry = new EffectEntry();
            effectEntry.Key = EffectTemplate.Key;
            effectEntry.Settings = effectSettings;

            return effectEntry;
        }

        void SetEffectEntry(EffectEntry entry)
        {
            if (!IsSetup)
                return;

            // Assign effect template based on entry key
            EffectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(entry.Key);
            if (EffectTemplate == null)
                throw new Exception(string.Format("SetEffectEntry() could not find effect key {0}", entry.Key));

            // Assign settings to spinners
            durationBaseSpinner.Value = entry.Settings.DurationBase;
            durationPlusSpinner.Value = entry.Settings.DurationPlus;
            durationPerLevelSpinner.Value = entry.Settings.DurationPerLevel;
            chanceBaseSpinner.Value = entry.Settings.ChanceBase;
            chancePlusSpinner.Value = entry.Settings.ChancePlus;
            chancePerLevelSpinner.Value = entry.Settings.ChancePerLevel;
            magnitudeBaseMinSpinner.Value = entry.Settings.MagnitudeBaseMin;
            magnitudeBaseMaxSpinner.Value = entry.Settings.MagnitudeBaseMax;
            magnitudePlusMinSpinner.Value = entry.Settings.MagnitudePlusMin;
            magnitudePlusMaxSpinner.Value = entry.Settings.MagnitudePlusMax;
            magnitudePerLevelSpinner.Value = entry.Settings.MagnitudePerLevel;
        }

        void SetSpinners(EffectSettings settings)
        {
            if (!IsSetup)
                return;

            durationBaseSpinner.Value = settings.DurationBase;
            durationPlusSpinner.Value = settings.DurationPlus;
            durationPerLevelSpinner.Value = settings.DurationPerLevel;
            chanceBaseSpinner.Value = settings.ChanceBase;
            chancePlusSpinner.Value = settings.ChancePlus;
            chancePerLevelSpinner.Value = settings.ChancePerLevel;
            magnitudeBaseMinSpinner.Value = settings.MagnitudeBaseMin;
            magnitudeBaseMaxSpinner.Value = settings.MagnitudeBaseMax;
            magnitudePlusMinSpinner.Value = settings.MagnitudePlusMin;
            magnitudePlusMaxSpinner.Value = settings.MagnitudePlusMax;
            magnitudePerLevelSpinner.Value = settings.MagnitudePerLevel;
        }

        void SetEffectTemplate(IEntityEffect effectTemplate)
        {
            this.effectTemplate = effectTemplate;
            SetSpinners(new EffectSettings());
        }

        void UpdateCosts()
        {
            // TODO: Multipliers for target type

            float offsetGold, offsetSpellPoints, factor, costA, costB;
            int skillValue = GameManager.Instance.PlayerEntity.Skills.GetLiveSkillValue((DFCareer.Skills)effectTemplate.Properties.MagicSkill);

            int durationGoldCost = 0, durationSpellCost = 0;
            if (effectTemplate.Properties.SupportDuration)
            {
                offsetGold = effectTemplate.Properties.DurationCosts.OffsetGold;
                offsetSpellPoints = effectTemplate.Properties.DurationCosts.OffsetSpellPoints;
                factor = effectTemplate.Properties.DurationCosts.Factor;
                costA = effectTemplate.Properties.DurationCosts.CostA;
                costB = effectTemplate.Properties.DurationCosts.CostB;

                durationGoldCost = FormulaHelper.GetEffectGoldCost(offsetGold, costA, costB, durationBaseSpinner.Value, durationPlusSpinner.Value, durationPerLevelSpinner.Value);
                durationSpellCost = FormulaHelper.GetEffectSpellPointCost(skillValue, offsetSpellPoints, factor, costA, costB, durationBaseSpinner.Value, durationPlusSpinner.Value, durationPerLevelSpinner.Value);

                Debug.LogFormat("Duration: gold {0} spellpoints {1}", durationGoldCost, durationSpellCost);
            }

            int chanceGoldCost = 0, chanceSpellPointCost = 0;
            if (effectTemplate.Properties.SupportChance)
            {
                offsetGold = effectTemplate.Properties.ChanceCosts.OffsetGold;
                offsetSpellPoints = effectTemplate.Properties.ChanceCosts.OffsetSpellPoints;
                factor = effectTemplate.Properties.ChanceCosts.Factor;
                costA = effectTemplate.Properties.ChanceCosts.CostA;
                costB = effectTemplate.Properties.ChanceCosts.CostB;

                chanceGoldCost = FormulaHelper.GetEffectGoldCost(offsetGold, costA, costB, chanceBaseSpinner.Value, chancePlusSpinner.Value, chancePerLevelSpinner.Value);
                chanceSpellPointCost = FormulaHelper.GetEffectSpellPointCost(skillValue, offsetSpellPoints, factor, costA, costB, chanceBaseSpinner.Value, chancePlusSpinner.Value, chancePerLevelSpinner.Value);

                Debug.LogFormat("Chance: gold {0} spellpoints {1}", chanceGoldCost, chanceSpellPointCost);
            }

            int magnitudeGoldCost = 0, magnitudeSpellPointCost = 0;
            if (effectTemplate.Properties.SupportMagnitude)
            {
                offsetGold = effectTemplate.Properties.MagnitudeCosts.OffsetGold;
                offsetSpellPoints = effectTemplate.Properties.MagnitudeCosts.OffsetSpellPoints;
                factor = effectTemplate.Properties.MagnitudeCosts.Factor;
                costA = effectTemplate.Properties.MagnitudeCosts.CostA;
                costB = effectTemplate.Properties.MagnitudeCosts.CostB;

                int magnitudeBase = magnitudeBaseMinSpinner.Value;
                int magnitudePlus = magnitudePlusMinSpinner.Value;

                magnitudeGoldCost = FormulaHelper.GetEffectGoldCost(offsetGold, costA, costB, magnitudeBase, magnitudePlus, magnitudePerLevelSpinner.Value);
                magnitudeSpellPointCost = FormulaHelper.GetEffectSpellPointCost(skillValue, offsetSpellPoints, factor, costA, costB, magnitudeBase, magnitudePlus, magnitudePerLevelSpinner.Value);

                Debug.LogFormat("Magnitude: gold {0} spellpoints {1}", magnitudeGoldCost, magnitudeSpellPointCost);
            }

            //int goldCost = FormulaHelper.GetEffectGoldCost(0, 40, 120, durationBaseSpinner.Value, durationPlusSpinner.Value, durationPerLevelSpinner.Value);
            //int spellPointCost = FormulaHelper.GetEffectSpellPointCost(50, 0, 40, 120, 2, durationBaseSpinner.Value, durationPlusSpinner.Value, durationPerLevelSpinner.Value);
            //Debug.LogFormat("Gold: {0} SpellPoints: {1}", goldCost, spellPointCost);
        }

        #endregion

        #region Event Handlers

        private void DurationBaseSpinner_OnValueChanged()
        {
            UpdateCosts();
        }

        private void DurationPlusSpinner_OnValueChanged()
        {
            UpdateCosts();
        }

        private void DurationPerLevelSpinner_OnValueChanged()
        {
            UpdateCosts();
        }

        private void ChanceBaseSpinner_OnValueChanged()
        {
            UpdateCosts();
        }

        private void ChancePlusSpinner_OnValueChanged()
        {
            UpdateCosts();
        }

        private void ChancePerLevelSpinner_OnValueChanged()
        {
            UpdateCosts();
        }

        private void MagnitudeBaseMinSpinner_OnValueChanged()
        {
            if (magnitudeBaseMinSpinner.Value > magnitudeBaseMaxSpinner.Value)
                magnitudeBaseMaxSpinner.Value = magnitudeBaseMinSpinner.Value;

            UpdateCosts();
        }

        private void MagnitudeBaseMaxSpinner_OnValueChanged()
        {
            if (magnitudeBaseMaxSpinner.Value < magnitudeBaseMinSpinner.Value)
                magnitudeBaseMinSpinner.Value = magnitudeBaseMaxSpinner.Value;

            UpdateCosts();
        }

        private void MagnitudePlusMinSpinner_OnValueChanged()
        {
            if (magnitudePlusMinSpinner.Value > magnitudePlusMaxSpinner.Value)
                magnitudePlusMaxSpinner.Value = magnitudePlusMinSpinner.Value;

            UpdateCosts();
        }

        private void MagnitudePlusMaxSpinner_OnValueChanged()
        {
            if (magnitudePlusMaxSpinner.Value < magnitudePlusMinSpinner.Value)
                magnitudePlusMinSpinner.Value = magnitudePlusMaxSpinner.Value;

            UpdateCosts();
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            userExit = true;
            CloseWindow();
        }

        #endregion
    }
}