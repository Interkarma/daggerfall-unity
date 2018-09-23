// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
//
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallPlayerHistoryWindow : DaggerfallBaseWindow
    {
        const string nativeImgName = "LGBK00I0.IMG";
        const int extraLeading = 0;
        const int maxPageLines = 21;

        public int tokenID = 4116;
        DaggerfallUnity dfUnity;
        Texture2D nativeTexture;
        DaggerfallFont currentFont;
        List<TextLabel> pageLabels = new List<TextLabel>();
        int pageLines;
        int tokenIndex = 0;
        TextFile.Token[] tokens;

        public DaggerfallPlayerHistoryWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            dfUnity = DaggerfallUnity.Instance;

            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallPlayerHistoryWindow: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Load default pixel font
            ChangeFont(4);

            // Add buttons
            Button nextPageButton = DaggerfallUI.AddButton(new Rect(208, 188, 14, 8), NativePanel);
            nextPageButton.OnMouseClick += NextPageButton_OnMouseClick;

            Button previousPageButton = DaggerfallUI.AddButton(new Rect(181, 188, 14, 48), NativePanel);
            previousPageButton.OnMouseClick += PreviousPageButton_OnMouseClick;

            Button exitButton = DaggerfallUI.AddButton(new Rect(277, 187, 32, 10), NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            tokens = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(tokenID);
            LayoutPage();
            DaggerfallUI.Instance.PlayOneShot(SoundClips.OpenBook);
        }

        private void NextPageButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (MoveNextPage())
            {
                LayoutPage();
            }
        }

        private void PreviousPageButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (MovePreviousPage())
            {
                LayoutPage();
            }
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        public override void OnPush()
        {
            if (IsSetup && tokenID != -1)
            {
                LayoutPage();
                DaggerfallUI.Instance.PlayOneShot(SoundClips.OpenBook);
            }
        }

        void LayoutPage()
        {
            ClearPage();
            pageLines = 0;

            int x = 10, y = 25;
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.None;
            for (int i = tokenIndex; i < tokens.Length; i++)
            {
                
                switch (tokens[i].formatting)
                {
                case TextFile.Formatting.JustifyLeft: // Acts as a newline in this context
                    if (i > tokenIndex)
                    {
                        y += currentFont.GlyphHeight + extraLeading;
                        horizontalAlignment = HorizontalAlignment.None;
                    }
                    break;
                case TextFile.Formatting.FirstCharacter:
                    // We can ignore these tokens
                    break;
                case TextFile.Formatting.Text:
                    TextLabel label = DaggerfallUI.AddTextLabel(currentFont, new Vector2(x, y), tokens[i].text, NativePanel);
                    label.HorizontalAlignment = horizontalAlignment;
                    label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
                    label.ShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
                    label.ShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
                    pageLabels.Add(label);
                    pageLines++;
                    break;
                default:
                    Debug.Log("DaggerfallPlayerHistoryWindow: Unknown formatting token: " + (int)tokens[i].formatting);
                    break;
                }

                if (pageLines == maxPageLines)
                {
                    break;
                }
            }
        }

        void ClearPage()
        {
            foreach (TextLabel label in pageLabels)
            {
                NativePanel.Components.Remove(label);
            }
            pageLabels.Clear();
        }

        void ChangeFont(int index)
        {
            currentFont = DaggerfallUI.Instance.GetFont(index);
        }

        bool MoveNextPage()
        {
            int deltaLines = 0;
            int deltaTokens = 0;
            // Jump past a page's worth of text tokens
            while (deltaLines < maxPageLines)
            {
                int index = tokenIndex + deltaTokens;
                if (++deltaTokens + tokenIndex >= tokens.Length)
                {
                    return false;
                }
                if (tokens[index].formatting == TextFile.Formatting.Text) 
                {
                    deltaLines++;
                }
            }
            tokenIndex += deltaTokens;

            return true;
        }

        bool MovePreviousPage()
        {
            if (tokenIndex == 0)
            {
                return false;
            }

            int deltaLines = 0;
            // Jump backwards until we pass by a page's worth of text tokens
            while (tokenIndex > 0 && deltaLines < maxPageLines)
            {
                tokenIndex--;
                if (tokens[tokenIndex].formatting == TextFile.Formatting.Text)
                {
                    deltaLines++;
                }
            }

            return true;
        }
    }
}