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
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.UserInterface;

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

        public DaggerfallBookReaderWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            dfUnity = DaggerfallUnity.Instance;

            // Load native texture
            nativeTexture = GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallBookReaderWindow: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Load default pixel font
            ChangeFont(4);

            // Add buttons
            AddButton(new Vector2(181, 188), new Vector2(14, 8), DaggerfallUIMessages.dfuiBookReaderPreviousPage);
            AddButton(new Vector2(208, 188), new Vector2(14, 8), DaggerfallUIMessages.dfuiBookReaderNextPage);
            //AddButton(new Vector2(277, 187), new Vector2(32, 10), WindowMessages.wmCloseWindow);

            // Test book
            dfUnity.TextProvider.OpenBook("BOK00043.TXT");      // The Real Barenziah
            //dfUnity.TextProvider.OpenBook("BOK00101.TXT");      // Kind Edward, Part 2
            //dfUnity.TextProvider.OpenBook("BOK00008.TXT");      // The Pig Children
            LayoutPage();
        }

        protected override void ProcessMessageQueue()
        {
            string message = uiManager.PeekMessage();
            switch (message)
            {
                case DaggerfallUIMessages.dfuiBookReaderPreviousPage:
                    if (dfUnity.TextProvider.MovePreviousPage())
                        LayoutPage();
                    break;
                case DaggerfallUIMessages.dfuiBookReaderNextPage:
                    if (dfUnity.TextProvider.MoveNextPage())
                        LayoutPage();
                    break;
                default:
                    return;
            }

            // Message was handled, pop from stack
            uiManager.PopMessage();
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
                        TextLabel label = AddTextLabel(currentFont, new Vector2(x, y), token.text);
                        label.HorizontalAlignment = horizontalAlignment;
                        label.TextColor = DaggerfallUI.DaggerfallDefaultTextColor;
                        label.ShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
                        label.ShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
                        pageLabels.Add(label);
                        break;
                    default:
                        Debug.Log("Unknown formatting token: " + (int)token.formatting);
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