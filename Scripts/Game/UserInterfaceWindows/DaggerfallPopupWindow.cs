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
using System.Collections;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implementation of a popup window designed to appear on top of other screens.
    /// Will render previous window hierarchy before its own.
    /// </summary>
    public class DaggerfallPopupWindow : DaggerfallBaseWindow
    {
        DaggerfallBaseWindow previous;

        Color screenDimColor = new Color32(0, 0, 0, 128);

        public Color ScreenDimColor
        {
            get { return screenDimColor; }
            set { screenDimColor = value; }
        }

        public DaggerfallPopupWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager)
        {
            this.previous = previous;
        }

        protected override void Setup()
        {
        }

        public override void Draw()
        {
            if (previous != null)
            {
                previous.Draw();
                ScreenPanel.BackgroundColor = screenDimColor;
            }

            base.Draw();
        }
    }
}