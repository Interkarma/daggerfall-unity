// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements a user interface window for native Daggerfall 320x200 screens.
    /// Also provides some control helpers common to UI windows.
    /// </summary>
    public abstract class DaggerfallBaseWindow : UserInterfaceWindow
    {
        public const float nativeScreenWidth = 320;
        public const float nativeScreenHeight = 200;
        public const KeyCode exitKey = KeyCode.Escape;

        bool isSetup;
        DaggerfallUnity dfUnity;
        Panel nativePanel = new Panel();      // Native panel is 320x200 child panel scaled to fit parent

        protected ToolTip defaultToolTip = null;

        public DaggerfallBaseWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
            // Parent panel
            parentPanel.Components.Add(nativePanel);
            parentPanel.BackgroundColor = Color.black;

            // Native panel
            nativePanel.HorizontalAlignment = HorizontalAlignment.Center;
            nativePanel.VerticalAlignment = VerticalAlignment.Middle;
            nativePanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
            nativePanel.Size = new Vector2(nativeScreenWidth, nativeScreenHeight);

            // Set native panel scaling mode
            if (DaggerfallUnity.Settings.FreeScaling)
                nativePanel.AutoSize = AutoSizeModes.ScaleFreely;
            else
                nativePanel.AutoSize = AutoSizeModes.ScaleToFit;

            // Setup default tooltip
            if (DaggerfallUnity.Settings.EnableToolTips)
            {
                defaultToolTip = new ToolTip();
                defaultToolTip.ToolTipDelay = DaggerfallUnity.Settings.ToolTipDelayInSeconds;
                defaultToolTip.BackgroundColor = DaggerfallUnity.Settings.ToolTipBackgroundColor;
                defaultToolTip.TextColor = DaggerfallUnity.Settings.ToolTipTextColor;
                defaultToolTip.Parent = nativePanel;

                // Experimental HQ tooltip
                if (DaggerfallUnity.Settings.HQTooltips)
                {
                    defaultToolTip.Font = DaggerfallUI.Instance.GetHQPixelFont(DaggerfallUI.HQPixelFonts.Petrock_32);
                    defaultToolTip.Parent = parentPanel;
                }
            }
        }

        protected DaggerfallUnity DaggerfallUnity
        {
            get { return (dfUnity != null) ? dfUnity : dfUnity = DaggerfallUnity.Instance; }
        }

        protected bool IsSetup
        {
            get { return isSetup; }
            set { isSetup = value; }
        }

        protected Panel NativePanel
        {
            get { return nativePanel; }
        }

        public override void Update()
        {
            // DaggerfallUnity must be ready
            if (dfUnity == null)
                dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Must be setup
            if (!isSetup)
            {
                Setup();
                isSetup = true;
                return;
            }

            base.Update();

            // Update tooltip last
            if (defaultToolTip != null)
                defaultToolTip.Update();
        }

        public override void Draw()
        {
            base.Draw();

            // Draw tooltip last
            if (defaultToolTip != null)
                defaultToolTip.Draw();
        }

        protected abstract void Setup();
    }
}