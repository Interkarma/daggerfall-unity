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
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallEffectSettingsEditorWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        DFSize baseSize = new DFSize(320, 200);

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

        #endregion

        #region UI Textures

        Texture2D baseTexture;

        #endregion

        #region Fields

        const string baseTextureFilename = "MASK05I0.IMG";

        const int alternateAlphaIndex = 12;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets IEntityEffect template for effect editor.
        /// </summary>
        public IEntityEffect EffectTemplate { get; set; }

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
            InitControlState();
        }

        public override void OnPush()
        {
            if (IsSetup)
            {
                InitControlState();
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
        }

        void InitControlState()
        {
            // Must have an effect template set
            if (EffectTemplate == null)
                throw new Exception("DaggerfallEffectSettingsEditorWindow does not have an EffectTemplate set.");

            // Get description text - effect must present either a classic TEXT.RSC ID or a custom token array
            TextFile.Token[] descriptionTokens;
            if (EffectTemplate.ClassicTextID != 0)
                descriptionTokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(EffectTemplate.ClassicTextID);
            else if (EffectTemplate.CustomText != null)
                descriptionTokens = EffectTemplate.CustomText;
            else
                throw new Exception(string.Format("DaggerfallEffectSettingsEditorWindow: EffectTemplate {0} does not present any description text.", EffectTemplate.Key));

            // Set description text
            if (descriptionTokens != null && descriptionTokens.Length > 0)
                descriptionLabel.SetText(descriptionTokens);

            // Duration support
            if (EffectTemplate.SupportDuration)
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
            if (EffectTemplate.SupportChance)
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
            if (EffectTemplate.SupportMagnitude)
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

        #endregion
    }
}