// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
    public class DaggerfallPlayerHistoryWindow : DaggerfallPopupWindow
    {
        const string nativeImgName = "LGBK00I0.IMG";
        const int extraLeading = 0;
        const int maxPageLines = 21;

        Texture2D nativeTexture;
        DaggerfallFont currentFont;
        List<TextLabel> pageLabels = new List<TextLabel>();
        int pageLines;
        int pageStartLine = 0;

        bool isCloseWindowDeferred = false;

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
            ChangeFont(DaggerfallFont.FontName.FONT0003);

            // Add buttons
            Button nextPageButton = DaggerfallUI.AddButton(new Rect(208, 188, 14, 8), NativePanel);
            nextPageButton.OnMouseClick += NextPageButton_OnMouseClick;
            nextPageButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.HistoryNextPage);

            Button previousPageButton = DaggerfallUI.AddButton(new Rect(181, 188, 14, 48), NativePanel);
            previousPageButton.OnMouseClick += PreviousPageButton_OnMouseClick;
            previousPageButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.HistoryPreviousPage);

            Button exitButton = DaggerfallUI.AddButton(new Rect(277, 187, 32, 10), NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.HistoryExit);
            exitButton.OnKeyboardEvent += ExitButton_OnKeyboardEvent;

            LayoutPage();
            DaggerfallUI.Instance.PlayOneShot(SoundClips.OpenBook);

            NativePanel.OnMouseScrollDown += NativePanel_OnMouseScrollDown;
            NativePanel.OnMouseScrollUp += NativePanel_OnMouseScrollUp;
        }

        private void NextPageButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (MoveNextPage())
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.OpenBook);
                LayoutPage();
            }
        }

        private void PreviousPageButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (MovePreviousPage())
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.OpenBook);
                LayoutPage();
            }
        }

        private void NativePanel_OnMouseScrollDown(BaseScreenComponent sender)
        {
            if (MoveNextPage())
            {
                LayoutPage();
            }
        }

        private void NativePanel_OnMouseScrollUp(BaseScreenComponent sender)
        {
            if (MovePreviousPage())
            {
                LayoutPage();
            }
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            pageStartLine = 0; // Go back to the first page
            CloseWindow();
        }

        protected void ExitButton_OnKeyboardEvent(BaseScreenComponent sender, Event keyboardEvent)
        {
            if (keyboardEvent.type == EventType.KeyDown)
            {
                DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
                isCloseWindowDeferred = true;
            }
            else if (keyboardEvent.type == EventType.KeyUp && isCloseWindowDeferred)
            {
                isCloseWindowDeferred = false;
                pageStartLine = 0; // Go back to the first page
                CloseWindow();
            }
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

            int x = 20, y = 25;
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

        void ChangeFont(DaggerfallFont.FontName fontName)
        {
            currentFont = DaggerfallUI.Instance.GetFont(fontName);
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
