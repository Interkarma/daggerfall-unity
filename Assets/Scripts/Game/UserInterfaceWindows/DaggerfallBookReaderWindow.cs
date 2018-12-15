// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: InconsolableCellist
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
    public class DaggerfallBookReaderWindow : DaggerfallBaseWindow
    {
        const string nativeImgName = "BOOK00I0.IMG";
        const int extraLeading = 0;

        DaggerfallUnity dfUnity;
        Texture2D nativeTexture;
        DaggerfallFont currentFont;
        List<TextLabel> pageLabels = new List<TextLabel>();
        DaggerfallUnityItem bookTarget;

        public DaggerfallBookReaderWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        public DaggerfallUnityItem BookTarget
        {
            get { return bookTarget; }
            set
            {
                bookTarget = value;
                DaggerfallUnity.Instance.TextProvider.OpenBook(bookTarget.message);
            }
        }

        protected override void Setup()
        {
            dfUnity = DaggerfallUnity.Instance;

            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallBookReaderWindow: Could not load native texture.");

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

            NativePanel.OnMouseScrollDown += Panel_OnMouseScrollDown;
            NativePanel.OnMouseScrollUp += Panel_OnMouseScrollUp;

            LayoutPage();
            DaggerfallUI.Instance.PlayOneShot(SoundClips.OpenBook);
        }

        private void NextPageButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (dfUnity.TextProvider.MoveNextPage())
            {
                LayoutPage();
                DaggerfallUI.Instance.PlayOneShot(SoundClips.PageTurn);
            }
        }

        private void PreviousPageButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (dfUnity.TextProvider.MovePreviousPage())
            {
                LayoutPage();
                DaggerfallUI.Instance.PlayOneShot(SoundClips.PageTurn);
            }
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void Panel_OnMouseScrollDown(BaseScreenComponent sender)
        {
            NextPageButton_OnMouseClick(sender, Vector2.zero);
        }

        private void Panel_OnMouseScrollUp(BaseScreenComponent sender)
        {
            PreviousPageButton_OnMouseClick(sender, Vector2.zero);
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
            TextFile.Token[] tokens = dfUnity.TextProvider.PageTokens;

            int x = 10, y = 20;
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.None;
            foreach (var token in tokens)
            {
                switch (token.formatting)
                {
                    case TextFile.Formatting.NewLine:
                        // Daggerfall books appear to reset horizontal alignment after newline
                        y += currentFont.GlyphHeight + extraLeading;
                        horizontalAlignment = HorizontalAlignment.None;
                        break;
                    case TextFile.Formatting.FontPrefix:
                        ChangeFont(token.x);
                        break;
                    case TextFile.Formatting.JustifyLeft:
                        horizontalAlignment = HorizontalAlignment.None;
                        break;
                    case TextFile.Formatting.JustifyCenter:
                        horizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case TextFile.Formatting.Text:
                        TextLabel label = DaggerfallUI.AddTextLabel(currentFont, new Vector2(x, y), token.text, NativePanel);
                        label.HorizontalAlignment = horizontalAlignment;
                        label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
                        label.ShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
                        label.ShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
                        pageLabels.Add(label);
                        break;
                    default:
                        Debug.Log("DaggerfallBookReaderWindow: Unknown formatting token: " + (int)token.formatting);
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
    }
}