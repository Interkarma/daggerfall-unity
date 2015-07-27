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
using System.Collections.Generic;
using System.Text;
using DaggerfallWorkshop.Demo.UserInterfaceWindows;

namespace DaggerfallWorkshop.Demo.UserInterface
{
    /// <summary>
    /// A simple button component.
    /// </summary>
    public class ButtonScreenComponent : BaseScreenComponent
    {
        public string ClickMessage { get; set; }
        public string DoubleClickMessage { get; set; }

        public ButtonScreenComponent()
            : base()
        {
            OnMouseClick += ClickHandler;
            OnMouseDoubleClick += DoubleClickHandler;
            //BackgroundColor = new Color(1, 1, 0, 0.25f);
        }

        void ClickHandler(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(ClickMessage))
            {
                DaggerfallUI.PostMessage(ClickMessage);
                //Debug.Log("Sending click message " + ClickMessage);
            }
        }

        void DoubleClickHandler(object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(DoubleClickMessage))
            {
                DaggerfallUI.PostMessage(DoubleClickMessage);
                //Debug.Log("Sending double-click message " + DoubleClickMessage);
            }
        }
    }
}
