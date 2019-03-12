// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using System.IO;
using System.Text.RegularExpressions;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallPlayerHistoryWindow : DaggerfallBaseWindow
    {
        const string nativeImgName = "LGBK00I0.IMG";
        const int extraLeading = 0;
        const int maxPageLines = 21;

        Texture2D nativeTexture;
        DaggerfallFont currentFont;
        List<TextLabel> pageLabels = new List<TextLabel>();
        int pageLines;
        int pageStartLine = 0;

        public int ClassId { get; protected set; }

        public DaggerfallPlayerHistoryWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
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
            pageStartLine = 0; // Go back to the first page
            CloseWindow();
        }

        public override void OnPush()
        {
            if (IsSetup)
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
            for (int i = pageStartLine; i < GameManager.Instance.PlayerEntity.BackStory.Count; i++)
            {
                TextLabel label = DaggerfallUI.AddTextLabel(currentFont, new Vector2(x, y), GameManager.Instance.PlayerEntity.BackStory[i], NativePanel);
                label.HorizontalAlignment = HorizontalAlignment.None;
                label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
                label.ShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
                label.ShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
                pageLabels.Add(label);
                pageLines++;

                if (pageLines == maxPageLines)
                {
                    break;
                }

                y += currentFont.GlyphHeight + extraLeading;
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
            if (pageStartLine + maxPageLines < GameManager.Instance.PlayerEntity.BackStory.Count)
            {
                pageStartLine += maxPageLines;

                return true;
            }

            return false;
        }

        bool MovePreviousPage()
        {
            if (pageStartLine != 0)
            {
                pageStartLine -= maxPageLines;

                return true;
            }

            return false;
        }
    }
}