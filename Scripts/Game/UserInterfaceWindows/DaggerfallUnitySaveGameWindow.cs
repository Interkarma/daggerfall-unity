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
        Color outlineColor = DaggerfallUI.DaggerfallDefaultShadowColor;
        Color panelBackgroundColor = new Color(1, 1, 1, 0.1f);
        Panel savePanel = new Panel();
        Panel saveListPanel = new Panel();
        Panel detailsPanel = new Panel();

        public DaggerfallUnitySaveGameWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        protected override void Setup()
        {
            // Save panel
            savePanel.HorizontalAlignment = HorizontalAlignment.Center;
            savePanel.VerticalAlignment = VerticalAlignment.Middle;
            savePanel.Size = new Vector2(300, 160);
            DaggerfallUI.Instance.SetDaggerfallPopupStyle(DaggerfallUI.PopupStyle.Parchment, savePanel);
            NativePanel.Components.Add(savePanel);

            // Title
            TextLabel titleLabel = new TextLabel();
            titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
            titleLabel.VerticalAlignment = VerticalAlignment.Top;
            titleLabel.Text = HardStrings.saveGame;
            titleLabel.Font = DaggerfallUI.TitleFont;
            savePanel.Components.Add(titleLabel);

            // Child panels
            int top = 16;
            saveListPanel = DaggerfallUI.AddPanel(new Rect(0, top, 110, savePanel.Size.y - top), savePanel);
            saveListPanel.HorizontalAlignment = HorizontalAlignment.Left;
            saveListPanel.BackgroundColor = panelBackgroundColor;
            detailsPanel = DaggerfallUI.AddPanel(new Rect(110, top, 180, savePanel.Size.y - top), savePanel);
            detailsPanel.HorizontalAlignment = HorizontalAlignment.Right;
            detailsPanel.BackgroundColor = panelBackgroundColor;

            // Outlines
            DaggerfallUI.AddOutline(new Rect(0, 0, saveListPanel.Size.x, saveListPanel.Size.y), outlineColor, saveListPanel);
            DaggerfallUI.AddOutline(new Rect(0, 0, detailsPanel.Size.x, detailsPanel.Size.y), outlineColor, detailsPanel);
        }
    }
}