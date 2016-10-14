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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public interface IUserInterfaceWindow
    {
        UserInterfaceWindow Value { get; }
        bool Enabled { get; set; }
        bool PauseWhileOpen { get; set; }
        Panel ParentPanel { get; }
        void Update();
        void Draw();
        void ProcessMessages();
    }

    /// <summary>
    /// UserInterfaceWindow abstract base class.
    /// Each window is a unique state managed by UserInterfaceManager.
    /// All subordinate controls should be added to ParentPanel.
    /// </summary>
    public abstract class UserInterfaceWindow : IUserInterfaceWindow
    {
        protected Panel parentPanel = new Panel();      // Parent panel fits to entire viewport
        protected IUserInterfaceManager uiManager;
        protected bool enabled = true;
        protected bool pauseWhileOpened = true;

        public UserInterfaceWindow Value
        {
            get { return (this); }
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public Panel ParentPanel
        {
            get { return parentPanel; }
        }

        public UserInterfaceWindow()
        {
            this.uiManager = DaggerfallUI.UIManager;
        }

        public UserInterfaceWindow(IUserInterfaceManager uiManager)
        {
            this.uiManager = uiManager;
        }

        public virtual void Update()
        {
            if (enabled)
            {
                parentPanel.Update();
            }
        }

        public virtual void Draw()
        {
            if (enabled)
            {
                parentPanel.Draw();
            }
        }

        public virtual void ProcessMessages()
        {
            if (uiManager != null)
            {
                string message = uiManager.PeekMessage();
                if (message == WindowMessages.wmCloseWindow)
                {
                    uiManager.GetMessage();     // Eat message
                    CloseWindow();
                }
            }
        }

        /// <summary>
        /// Called when this window is pushed to stack.
        /// </summary>
        public virtual void OnPush()
        {
        }

        /// <summary>
        /// Called when this window pops off from stack.
        /// </summary>
        public virtual void OnPop()
        {
        }

        /// <summary>
        /// Called when returning to this window after popping back from another window.
        /// </summary>
        public virtual void OnReturn()
        {
        }

        public void CloseWindow()
        {
            uiManager.PopWindow();
            RaiseOnCloseHandler();
            Resources.UnloadUnusedAssets();
        }

        public void PopWindow()
        {
            uiManager.PopWindow();
        }

        public virtual bool PauseWhileOpen
        {
            get { return pauseWhileOpened; }
            set { pauseWhileOpened = value; }
        }

        //internal protected virtual void WindowChanged(object sender, EventArgs e)
        //{
        //    if (uiManager.TopWindow == this.Value)
        //    {
        //    }
        //}

        #region Event Handlers

        // OnClose
        public delegate void OnCloseHandler();
        public event OnCloseHandler OnClose;
        protected virtual void RaiseOnCloseHandler()
        {
            if (OnClose != null)
                OnClose();
        }

        #endregion
    }
}
