// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
    /// Demo popup window to play .FLC files in DagUI.
    /// </summary>
    public class DemoFLCWindow : DaggerfallPopupWindow
    {
        FLCPlayer playerPanel = null;

        public string Filename { get; set; }

        public DemoFLCWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            :base(uiManager, previous)
        {
        }

        protected override void Setup()
        {
            // Set panel width/height to whatever - by default the image will maintain aspect inside panel area
            playerPanel = new FLCPlayer();
            playerPanel.HorizontalAlignment = HorizontalAlignment.Center;
            playerPanel.VerticalAlignment = VerticalAlignment.Middle;
            playerPanel.Size = new Vector2(320, 200);
            NativePanel.Components.Add(playerPanel);
            StartPlaying();
        }

        public override void OnPush()
        {
            StartPlaying();
        }

        void StartPlaying()
        {
            if (playerPanel == null || string.IsNullOrEmpty(Filename))
                return;

            playerPanel.Load(Filename);
            if (!playerPanel.FLCFile.ReadyToPlay)
                return;

            playerPanel.Start();
        }
    }
}