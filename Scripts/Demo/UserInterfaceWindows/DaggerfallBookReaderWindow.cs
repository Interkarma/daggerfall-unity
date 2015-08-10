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
using DaggerfallWorkshop.Demo.UserInterface;

namespace DaggerfallWorkshop.Demo.UserInterfaceWindows
{
    public class DaggerfallBookReaderWindow : DaggerfallBaseWindow
    {
        const string nativeImgName = "BOOK00I0.IMG";
        const int extraLeading = 0;

        DaggerfallUnity dfUnity;
        Texture2D nativeTexture;
        DaggerfallFont currentFont;
        BookFile bookFile = new BookFile();

        List<TextLabel> pageLabels = new List<TextLabel>();
        int currentPage = 0;

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
            LoadBook("BOK00043.TXT");               // The Real Barenziah
            //LoadBook("BOK00101.TXT");               // Kind Edward, Part 2
            //LoadBook("BOK00008.TXT");               // The Pig Children
            LayoutPage(currentPage);

            IsSetup = true;
        }

        protected override void ProcessMessageQueue()
        {
            string message = uiManager.PeekMessage();
            switch (message)
            {
                case DaggerfallUIMessages.dfuiBookReaderPreviousPage:
                    if (--currentPage < 0) currentPage = 0;
                    LayoutPage(currentPage);
                    break;
                case DaggerfallUIMessages.dfuiBookReaderNextPage:
                    if (++currentPage >= bookFile.PageCount) currentPage = bookFile.PageCount - 1;
                    LayoutPage(currentPage);
                    break;
                default:
                    return;
            }

            // Message was handled, pop from stack
            uiManager.PopMessage();
        }

        void LoadBook(string name)
        {
            bookFile.OpenBook(dfUnity.Arena2Path, name);
        }

        void LayoutPage(int page)
        {
            ClearPage();
            TextResourceFile.Token[] tokens = bookFile.GetPageTokens(page);

            int x = 10, y = 20;
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.None;
            foreach (var token in tokens)
            {
                switch (token.formatting)
                {
                    case TextResourceFile.Formatting.NewLine:
                        // Daggerfall books appear to reset horizontal alignment after newline
                        y += currentFont.GlyphHeight + extraLeading;
                        horizontalAlignment = HorizontalAlignment.None;
                        break;
                    case TextResourceFile.Formatting.FontPrefix:
                        ChangeFont(token.x);
                        break;
                    case TextResourceFile.Formatting.JustifyLeft:
                        horizontalAlignment = HorizontalAlignment.None;
                        break;
                    case TextResourceFile.Formatting.JustifyCenter:
                        horizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case TextResourceFile.Formatting.Text:
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

        #region Private Methods

        void ChangeFont(int index)
        {
            currentFont = DaggerfallUI.Instance.GetFont(index);
        }

        #endregion
    }
}