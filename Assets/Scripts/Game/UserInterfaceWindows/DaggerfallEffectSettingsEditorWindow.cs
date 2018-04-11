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

        #endregion

        #region UI Controls

        Panel descriptionPanel;
        MultiFormatTextLabel descriptionLabel;

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
        }

        public override void OnPush()
        {
            if (IsSetup)
            {
                InitControlState();
            }
        }

        #endregion

        #region Public Methods
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
            //descriptionParentPanel.BackgroundColor = Color.red;

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
            InitControlState();
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
        }

        #endregion
    }
}