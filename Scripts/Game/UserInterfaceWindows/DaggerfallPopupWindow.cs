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
    /// Can optionally render previous window hierarchy before its own.
    /// Popups can be optionally cancelled at any time using the ESC key.
    /// </summary>
    public class DaggerfallPopupWindow : DaggerfallBaseWindow
    {
        IUserInterfaceWindow previousWindow;

        Color screenDimColor = new Color32(0, 0, 0, 128);
        bool allowCancel = true;
        bool cancelled = false;

        public Color ScreenDimColor
        {
            get { return screenDimColor; }
            set { screenDimColor = value; }
        }

        public bool AllowCancel
        {
            get { return allowCancel; }
            set { allowCancel = value; }
        }

        public bool Cancelled
        {
            get { return cancelled; }
        }

        public IUserInterfaceWindow PreviousWindow
        {
            get { return previousWindow; }
            set { previousWindow = value; }
        }

        public DaggerfallPopupWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null)
            : base(uiManager)
        {
            this.previousWindow = previousWindow;
        }

        protected override void Setup()
        {
        }

        public override void Update()
        {
            base.Update();

            cancelled = false;
            if (allowCancel && Input.GetKeyDown(exitKey))
                CancelWindow();
        }

        public override void Draw()
        {
            if (previousWindow != null)
            {
                previousWindow.Draw();
                parentPanel.BackgroundColor = screenDimColor;
            }

            base.Draw();
        }

        public void CancelWindow()
        {
            cancelled = true;
            RaiseOnCancelEvent(this);
            uiManager.PostMessage(WindowMessages.wmCloseWindow);
        }

        #region Events

        // OnCancel
        public delegate void OnCancelHandler(DaggerfallPopupWindow sender);
        public event OnCancelHandler OnCancel;
        void RaiseOnCancelEvent(DaggerfallPopupWindow sender)
        {
            if (OnCancel != null)
                OnCancel(sender);
        }

        #endregion
    }
}