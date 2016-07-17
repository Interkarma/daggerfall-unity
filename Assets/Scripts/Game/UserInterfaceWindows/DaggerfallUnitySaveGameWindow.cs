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
using DaggerfallConnect.Save;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Daggerfall Unity save game interface.
    /// </summary>
    public class DaggerfallUnitySaveGameWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        Vector2 mainPanelSize = new Vector2(280, 170);

        #endregion

        #region UI Controls

        Panel mainPanel = new Panel();
        TextBox saveNameTextBox = new TextBox();
        TextLabel promptLabel = new TextLabel();

        Color mainPanelBackgroundColor = new Color(0.0f, 0f, 0.0f, 1.0f);
        Color namePanelBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

        #endregion

        #region UI Textures
        #endregion

        #region Fields

        const string promptText = "Save Game";

        #endregion

        #region Constructors

        public DaggerfallUnitySaveGameWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Main panel
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.Size = mainPanelSize;
            mainPanel.Outline.Enabled = true;
            mainPanel.BackgroundColor = mainPanelBackgroundColor;
            NativePanel.Components.Add(mainPanel);

            // Prompt
            promptLabel.ShadowPosition = Vector2.zero;
            promptLabel.Position = new Vector2(4, 4);
            mainPanel.Components.Add(promptLabel);

            // Name panel
            Panel namePanel = new Panel();
            namePanel.Position = new Vector2(4, 12);
            namePanel.Size = new Vector2(272, 9);
            namePanel.Outline.Enabled = true;
            namePanel.BackgroundColor = namePanelBackgroundColor;
            mainPanel.Components.Add(namePanel);

            // Name input
            saveNameTextBox.Position = new Vector2(2, 2);
            saveNameTextBox.MaxCharacters = 45;
            namePanel.Components.Add(saveNameTextBox);

            // Existing save list
            Panel existingSavesPanel = new Panel();
            existingSavesPanel.Position = new Vector2(4, 25);
            existingSavesPanel.Size = new Vector2(100, 141);
            existingSavesPanel.Outline.Enabled = true;
            mainPanel.Components.Add(existingSavesPanel);
        }

        public override void OnPush()
        {
            base.OnPush();

            // Set default text
            saveNameTextBox.DefaultText = DaggerfallUnity.Instance.WorldTime.Now.MidDateTimeString();

            // Update save game prompt
            promptLabel.Text = string.Format("{0} for '{1}'", promptText, GameManager.Instance.PlayerEntity.Name);
        }

        #endregion
    }
}