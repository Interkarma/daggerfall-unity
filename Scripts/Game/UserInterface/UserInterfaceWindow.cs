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
using System.Collections.Generic;
using System.Text;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public interface IUserInterfaceWindow
    {
        UserInterfaceWindow Value { get; }
        void Update();
        void Draw();
    }

    /// <summary>
    /// UserInterfaceWindow abstract base class.
    /// Each window is a unique state managed by UserInterfaceManager.
    /// </summary>
    public abstract class UserInterfaceWindow : IUserInterfaceWindow
    {
        protected IUserInterfaceManager uiManager;

        public UserInterfaceWindow Value
        {
            get { return (this); }
        }

        public UserInterfaceWindow(IUserInterfaceManager uiManager)
        {
            this.uiManager = uiManager;
        }

        public virtual void Update()
        {
        }

        public virtual void Draw()
        {
        }

        internal protected virtual void WindowChanged(object sender, EventArgs e)
        {
            if (uiManager.TopWindow == this.Value)
            {
            }
        }
    }
}
