// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Nystul
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using Wenzil.Console;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public class DaggerfallTalkWindow : DaggerfallPopupWindow
    {
        const string IMGNAME    = "TALK01I0.IMG";

        Panel mainPanel;

        TextLabel pcSay;

        Button goodbyeButton;

        public DaggerfallTalkWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
            // register console commands
            try
            {
                TalkConsoleCommands.RegisterCommands();
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Error Registering Talk Window Console commands: {0}", ex.Message));

            }
        }

        protected override void Setup()
        {
            base.Setup();

            var background = DaggerfallUI.GetTextureFromImg(IMGNAME);
            if (background == null)
            {
                Debug.LogError(string.Format("Failed to load background image {0} for talk window", IMGNAME));
                CloseWindow();
                return;
            }

            ParentPanel.BackgroundColor = ScreenDimColor;

            mainPanel                       = DaggerfallUI.AddPanel(NativePanel, AutoSizeModes.None);
            mainPanel.BackgroundTexture     = background;
            mainPanel.Size                  = new Vector2(background.width, background.height);
            mainPanel.HorizontalAlignment   = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment     = VerticalAlignment.Middle;

            /*
            pcSay = new TextLabel();
            pcSay.Position = new Vector2(150, 14);
            pcSay.Size = new Vector2(60, 13);
            pcSay.Name = "accnt_total_label";
            pcSay.MaxCharacters = 13;
            mainPanel.Components.Add(pcSay);
            */

            goodbyeButton = new Button();
            goodbyeButton.Position = new Vector2(118, 183);
            goodbyeButton.Size = new Vector2(67, 10);
            goodbyeButton.Name = "goodbye_button";
            goodbyeButton.OnMouseClick += goodbyeButton_OnMouseClick;
            mainPanel.Components.Add(goodbyeButton);

            UpdateLabels();
        }

        public override void OnPush()
        {
            base.OnPush();
        }

        public override void OnPop()
        {
            base.OnPop();
        }

        public override void Update()
        {
            base.Update();

            UpdateLabels();
        }


        void UpdateLabels()
        {

        }

        void UpdateButtons()
        {

        }

        #region event handlers

        void goodbyeButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion

        #region console_commands

        public static class TalkConsoleCommands
        {
            public static void RegisterCommands()
            {
                try
                {
                    ConsoleCommandsDatabase.RegisterCommand(OpenTalkWindow.name, OpenTalkWindow.description, OpenTalkWindow.usage, OpenTalkWindow.Execute);
                }
                catch (System.Exception ex)
                {
                    DaggerfallUnity.LogMessage(ex.Message, true);
                }
            }

            private static class OpenTalkWindow
            {
                public static readonly string name = "openTalkWindow";
                public static readonly string description = "opens talk window";
                public static readonly string usage = "openTalkWindow";

                public static string Execute(params string[] args)
                {
                    DaggerfallUI.UIManager.PushWindow(DaggerfallUI.Instance.TalkWindow);
                    return "talk window opened";
                }
            }
        }

        #endregion
    }
}
