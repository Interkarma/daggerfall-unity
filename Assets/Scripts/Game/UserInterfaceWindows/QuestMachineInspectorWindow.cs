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
using System.Collections;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Inspect state and data of quests running inside Quest Machine.
    /// Provides some basic debugging features.
    /// Intended for testing only, not part of normal gameplay.
    /// </summary>
    public class QuestMachineInspectorWindow : DaggerfallPopupWindow
    {
        #region UI Rects
        #endregion

        #region Constructors

        public QuestMachineInspectorWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null, int screenWidth = 640, int screenHeight = 400)
            : base(uiManager, previous, screenWidth, screenHeight)
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
        }

        #endregion
    }
}