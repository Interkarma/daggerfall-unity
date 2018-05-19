// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Lypyl (lypyldf@gmail.com)
// Contributors:    
// 
// Notes:
//


//#define LAYOUT


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Questing;


namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /* ##TODO:
     * dialogue text
     * implement sorting / filtering
     * option to show letters recieved
     */ 

    public class DaggerfallQuestJournalWindow : DaggerfallPopupWindow
    {
        const string nativeImgName  = "LGBK00I0.IMG";
        const int maxLines          = 19;
        int lastMessageIndex        = -1;
        int currentMessageIndex     = 0;

        List<Message> questMessages;

        MultiFormatTextLabel questLogLabel;

        Panel mainPanel;

        Button dialogButton;
        Button upArrowButton;
        Button downArrowButton;
        Button exitButton;


        public DaggerfallQuestJournalWindow(UserInterfaceManager uiManager) : base(uiManager) 
        {
        }


        protected override void Setup()
        {
            base.Setup();

            Texture2D texture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (texture == null)
            {
                Debug.LogError("failed to load texture: " + nativeImgName);
                CloseWindow();
            }


            mainPanel                       = DaggerfallUI.AddPanel(NativePanel, AutoSizeModes.None);
            mainPanel.BackgroundTexture     = texture;
            mainPanel.Size                  = new Vector2(320, 200);
            mainPanel.HorizontalAlignment   = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment     = VerticalAlignment.Middle;
            mainPanel.OnMouseScrollDown     += MainPanel_OnMouseScrollDown;
            mainPanel.OnMouseScrollUp       += MainPanel_OnMouseScrollUp;

            dialogButton                    = new Button();
            dialogButton.Position           = new Vector2(32, 187);
            dialogButton.Size               = new Vector2(68, 10);
            dialogButton.OnMouseClick       += dialogButton_OnMouseClick;
            dialogButton.Name               = "dialog_button";
            mainPanel.Components.Add(dialogButton);

            upArrowButton                   = new Button();
            upArrowButton.Position          = new Vector2(181, 188);
            upArrowButton.Size              = new Vector2(13, 7);
            upArrowButton.OnMouseClick      += upArrowButton_OnMouseClick;
            upArrowButton.Name              = "uparrow_button";
            mainPanel.Components.Add(upArrowButton);

            downArrowButton                 = new Button();
            downArrowButton.Position        = new Vector2(209, 188);
            downArrowButton.Size            = new Vector2(13, 7);
            downArrowButton.OnMouseClick    += downArrowButton_OnMouseClick;
            downArrowButton.Name            = "downarrow_button";
            mainPanel.Components.Add(downArrowButton);

            exitButton                      = new Button();
            exitButton.Position             = new Vector2(278, 187);
            exitButton.Size                 = new Vector2(30, 9);
            exitButton.OnMouseClick         += exitButton_OnMouseClick;
            exitButton.Name                 = "exit_button";
            mainPanel.Components.Add(exitButton);

            questLogLabel                   = new MultiFormatTextLabel();
            questLogLabel.Position          = new Vector2(30, 32);
            questLogLabel.Size              = new Vector2(238, 138);
            mainPanel.Components.Add(questLogLabel);

            questMessages = QuestMachine.Instance.GetAllQuestLogMessages();
            SetText();

#if LAYOUT
            SetBackgroundColors();
#endif
        }

        public override void OnPush()
        {
            base.OnPush();
            questMessages       = QuestMachine.Instance.GetAllQuestLogMessages();
            lastMessageIndex    = -1;
            currentMessageIndex = 0;
        }

        public override void OnPop()
        {
            base.OnPop();
            questMessages = null;
            questLogLabel.Clear();
        }

        public override void Update()
        {
            base.Update();

            if (lastMessageIndex != currentMessageIndex)
            {
                lastMessageIndex = currentMessageIndex;
                SetText();
            }
        }


#region events

        public void dialogButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            //Debug.LogError("button clicked: " + sender.Name);

        }

        public void upArrowButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            //Debug.Log("button clicked: " + sender.Name);
            if (currentMessageIndex - 1 >= 0)
                currentMessageIndex -= 1;

        }

        public void downArrowButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            //Debug.Log("button clicked: " + sender.Name);
            if (currentMessageIndex + 1 < questMessages.Count)
                currentMessageIndex += 1;

        }

        void MainPanel_OnMouseScrollUp(BaseScreenComponent sender)
        {
            upArrowButton_OnMouseClick(sender, Vector2.zero);
        }

        void MainPanel_OnMouseScrollDown(BaseScreenComponent sender)
        {
            downArrowButton_OnMouseClick(sender, Vector2.zero);
        }

        public void exitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            //Debug.Log("button clicked: " + sender.Name);
            CloseWindow();
        }

#endregion

        private void SetText()
        {
            questLogLabel.Clear();

            TextFile.Token newLineToken = new TextFile.Token();
            newLineToken.formatting = TextFile.Formatting.NewLine;

            int totalLineCount = 0;

            List<TextFile.Token> textTokens = new List<TextFile.Token>();

            for (int i = currentMessageIndex; i < questMessages.Count; i++)
            {
                if (totalLineCount >= 19)
                    break;

                var tokens = questMessages[i].GetTextTokens();

                for (int j = 0; j < tokens.Length; j++)
                {
                    if (totalLineCount >= 19)
                        break;

                    var token = tokens[j];

                    if (token.formatting == TextFile.Formatting.Text || token.formatting == TextFile.Formatting.NewLine)
                        totalLineCount++;
                    else
                        token.formatting = TextFile.Formatting.JustifyLeft;

                    textTokens.Add(token);
                }

                textTokens.Add(newLineToken);
                totalLineCount++;
            }

            questLogLabel.SetText(textTokens.ToArray());
        }


#if LAYOUT
        void SetBackgroundColors()
        {
            Color[] colors = new Color[]{
                new Color(0,0,0, .75f),
                new Color(1,0,0, .75f),
                new Color(0,1,0, .75f),
                new Color(0,0,0, .75f),
                new Color(1, 1, 1, 0.75f),
                new Color(1, 1, 0, 0.75f),
                new Color(1, 0, 1, 0.75f),
                new Color(0, 1, 1, 0.75f),
            };


            int i = 0;
            int color_index = 0;
            while (i < mainPanel.Components.Count)
            {
                if (color_index >= colors.Length)
                    color_index = 0;

                mainPanel.Components[i].BackgroundColor = colors[color_index];
                i++;
                color_index++;
            }
        }
#endif
    }
}
