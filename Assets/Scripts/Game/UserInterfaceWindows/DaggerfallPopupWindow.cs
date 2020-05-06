// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:
//
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implementation of a popup window designed to appear on top of other screens.
    /// Can optionally render previous window hierarchy before its own.
    /// Popups can be optionally cancelled at any time using the ESC key.
    /// </summary>
    public abstract class DaggerfallPopupWindow : DaggerfallBaseWindow
    {
        IUserInterfaceWindow previousWindow;

        //Color screenDimColor = new Color32(0, 0, 0, 128);
        Color screenDimColor = Color.clear;
        bool allowCancel = true;
        bool cancelled = false;

        bool isCancelWindowRearmed = false;
        bool isCancelWindowDeferred = false;

        public Color ScreenDimColor
        {
            get { return GetScreenDimColor(); }
            set { screenDimColor = Color.clear;/*value*/; }
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

        public DaggerfallPopupWindow(IUserInterfaceManager uiManager, IUserInterfaceWindow previousWindow = null, int screenWidth = 320, int screenHeight = 200)
            : base(uiManager, screenWidth, screenHeight)
        {
            this.previousWindow = previousWindow;
            this.screenDimColor.a = 0; //DaggerfallUnity.Settings.DimAlphaStrength;
        }

        protected override void Setup()
        {
        }

        public override void OnPush()
        {
            isCancelWindowRearmed = false;
        }

        public override void Update()
        {
            base.Update();

            cancelled = false;
            if (!DaggerfallUI.Instance.HotkeySequenceProcessed)
            {
                if (allowCancel)
                {
                    if (!Input.GetKey(exitKey))
                        isCancelWindowRearmed = true;
                    if (Input.GetKeyDown(exitKey) && isCancelWindowRearmed)
                        isCancelWindowDeferred = true;
                    else if (Input.GetKeyUp(exitKey) && isCancelWindowDeferred)
                    {
                        isCancelWindowDeferred = false;
                        CancelWindow();
                    }
                }
            }
        }

        public override void Draw()
        {
            if (previousWindow != null)
            {
                previousWindow.Draw();
                parentPanel.BackgroundColor = ScreenDimColor;
            }

            base.Draw();
        }

        public virtual void CancelWindow()
        {
            cancelled = true;
            RaiseOnCancelEvent(this);
            uiManager.PostMessage(WindowMessages.wmCloseWindow);
        }

        Color GetScreenDimColor()
        {
            return screenDimColor;
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