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
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements a VID player.
    /// </summary>
    public class DaggerfallVidPlayerWindow : DaggerfallBaseWindow
    {
        const string testFile = "DAG2.VID";

        DaggerfallVideo video;

        public DaggerfallVidPlayerWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Add video player control
            video = new DaggerfallVideo();
            video.HorizontalAlignment = HorizontalAlignment.Center;
            video.Size = new Vector2(nativeScreenWidth, nativeScreenHeight);
            NativePanel.Components.Add(video);

            // Open and play a video
            video.Open(testFile);
            video.Playing = true;
            
            IsSetup = true;
        }
    }
}