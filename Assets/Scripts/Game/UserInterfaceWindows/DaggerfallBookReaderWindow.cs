// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    InconsolableCellist
//
// Notes:
//

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Localization;
using System.Globalization;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallBookReaderWindow : DaggerfallBaseWindow
    {
        const string textFolderName = "Text";
        const string bookImagesPath = "Books/BookImages";

        const float scrollAmount = 24;
        const string nativeImgName = "BOOK00I0.IMG";
        char newline = '\n';

        Vector2 pagePanelPositionClassic = new Vector2(10, 21);
        Vector2 pagePanelSizeClassic = new Vector2(300, 159);
        Vector2 pagePanelPositionSDF = new Vector2(40, 21);
        Vector2 pagePanelSizeSDF = new Vector2(270, 159);
        Vector2 pagePanelPosition = Vector2.zero;
        Vector2 pagePanelSize = Vector2.zero;

        Texture2D nativeTexture;
        List<TextLabel> bookLabels = new List<TextLabel>();
        Panel pagePanel = new Panel();
        float maxHeight = 0;
        float scrollPosition = 0;
        int currentPage = 0; // Used for page turn sounds only

        const SoundClips openBook = SoundClips.OpenBook;

        public DaggerfallBookReaderWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        public bool IsBookOpen { get; private set; }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallBookReaderWindow: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Use smaller margins for SDF book text and wider margins for non-SDF (classic pixel font) text
            if (DaggerfallUnity.Settings.SDFFontRendering)
            {
                pagePanelPosition = pagePanelPositionSDF;
                pagePanelSize = pagePanelSizeSDF;
            }
            else
            {
                pagePanelPosition = pagePanelPositionClassic;
                pagePanelSize = pagePanelSizeClassic;
            }

            // Setup panel to contain text labels
            pagePanel.Position = pagePanelPosition;
            pagePanel.Size = pagePanelSize;
            pagePanel.RectRestrictedRenderArea = new Rect(pagePanel.Position, pagePanel.Size);
            pagePanel.HorizontalAlignment = HorizontalAlignment.Center;
            NativePanel.Components.Add(pagePanel);

            // Add buttons
            Button nextPageButton = DaggerfallUI.AddButton(new Rect(208, 188, 14, 8), NativePanel);
            nextPageButton.OnMouseClick += NextPageButton_OnMouseClick;

            Button previousPageButton = DaggerfallUI.AddButton(new Rect(181, 188, 14, 48), NativePanel);
            previousPageButton.OnMouseClick += PreviousPageButton_OnMouseClick;

            Button exitButton = DaggerfallUI.AddButton(new Rect(277, 187, 32, 10), NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;

            NativePanel.OnMouseScrollDown += Panel_OnMouseScrollDown;
            NativePanel.OnMouseScrollUp += Panel_OnMouseScrollUp;

            LayoutBookLabels();
            DaggerfallUI.Instance.PlayOneShot(openBook);
        }

        private void NextPageButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ScrollBook(-scrollAmount);
        }

        private void PreviousPageButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            ScrollBook(scrollAmount);
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
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
                LayoutBookLabels();
                DaggerfallUI.Instance.PlayOneShot(openBook);
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

        public void OpenBook(int id)
        {   
            OpenBook(DaggerfallUnity.Instance.ItemHelper.GetBookFileName(id));
        }

        public void OpenBook(DaggerfallUnityItem target)
        {
            if (target == null || target.ItemGroup != ItemGroups.Books || target.IsArtifact)
                throw new Exception("Item is not a valid book for book reader UI.");

            OpenBook(DaggerfallUnity.Instance.ItemHelper.GetBookFileName(target.message));
        }

        public void OpenBook(string filename)
        {
            // Open book file - localized book is first preference with fallback to classic book data from ARENA2 or mods
            LocalizedBook localizedBook = new LocalizedBook();
            if (localizedBook.OpenBookFile(filename))
            {
                IsBookOpen = true;
                Debug.LogFormat("Opened LocalizedBook '{0}'", filename);
            }
            else
            {
                Debug.LogErrorFormat("Failed to open LocalizedBook '{0}'", filename);
                return;
            }

            // Create book labels for UI
            CreateBookLabels(localizedBook);
        }

        void ScrollBook(float amount)
        {
            if (!IsBookOpen)
                return;

            // Stop scrolling at top or bottom of book layout
            if (amount < 0 && scrollPosition - pagePanel.Size.y - amount < -maxHeight)
                return;
            else if (amount > 0 && scrollPosition == 0)
                return;

            // Scroll label and only draw what can be seen
            scrollPosition += amount;
            foreach (TextLabel label in bookLabels)
            {
                label.Position = new Vector2(label.Position.x, label.Position.y + amount);
                label.Enabled = label.Position.y < pagePanel.Size.y && label.Position.y + label.Size.y > 0;
            }

            // page displayed at the center of the panel
            int centerPage = (int)(scrollPosition / pagePanelSize.y + 0.5f);
            if (currentPage != centerPage)
            {
                currentPage = centerPage;
                DaggerfallUI.Instance.PlayOneShot(SoundClips.PageTurn);
            }
        }

        #region Layout Methods

        void CreateBookLabels(LocalizedBook localizedBook)
        {
            DaggerfallFont currentFont = DaggerfallUI.DefaultFont;
            HorizontalAlignment currentAlignment = HorizontalAlignment.Left;
            Color currentColor = DaggerfallUI.DaggerfallDefaultTextColor;
            float currentScale = 1.0f;

            // Convert all book lines to labels
            bookLabels.Clear();
            string[] lines = localizedBook.Content.Split(newline);
            foreach (string line in lines)
            {
                TextFile.Token[] lineTokens = DaggerfallStringTableImporter.ConvertStringToRSCTokens(line);
                if (lineTokens == null || lineTokens.Length == 0)
                {
                    // Empty or newline label - also resets alignment, color, scale
                    bookLabels.Add(CreateLabel(DaggerfallUI.DefaultFont, HorizontalAlignment.Left, DaggerfallUI.DaggerfallDefaultTextColor, string.Empty));
                    currentAlignment = HorizontalAlignment.Left;
                    currentColor = DaggerfallUI.DaggerfallDefaultTextColor;
                    currentScale = 1.0f;
                }
                else
                {
                    foreach (TextFile.Token token in lineTokens)
                    {
                        switch (token.formatting)
                        {
                            case TextFile.Formatting.FontPrefix:
                                currentFont = DaggerfallUI.Instance.GetFont((DaggerfallFont.FontName)token.x - 1);
                                break;
                            case TextFile.Formatting.Color:
                                currentColor = TryParseColor(token.text);
                                break;
                            case TextFile.Formatting.JustifyLeft:
                                currentAlignment = HorizontalAlignment.Left;
                                break;
                            case TextFile.Formatting.JustifyCenter:
                                currentAlignment = HorizontalAlignment.Center;
                                break;
                            case TextFile.Formatting.Scale:
                                currentScale = TryParseScale(token.text);
                                break;
                            case TextFile.Formatting.Image:
                                bookLabels.Add(CreateImageLabel(token.text));
                                break;
                            default:
                                bookLabels.Add(CreateLabel(currentFont, currentAlignment, currentColor, token.text, currentScale));
                                break;
                        }
                        
                    }
                }
            }
        }

        TextLabel CreateLabel(DaggerfallFont font, HorizontalAlignment alignment, Color color, string text, float scale = 1.0f)
        {
            // Every group is cast into a word-wrapping label
            TextLabel label = new TextLabel();
            label.Font = font;
            label.HorizontalAlignment = alignment;
            label.TextColor = color;
            label.ShadowColor = DaggerfallUI.DaggerfallDefaultShadowColor;
            label.ShadowPosition = DaggerfallUI.DaggerfallDefaultShadowPos;
            label.WrapText = true;
            label.WrapWords = true;
            label.Text = text;
            label.TextScale = scale;
            if (label.HorizontalAlignment == HorizontalAlignment.Center)
                label.HorizontalTextAlignment = TextLabel.HorizontalTextAlignmentSetting.Center;

            return label;
        }

        ImageLabel CreateImageLabel(string filename)
        {
            ImageLabel label = new ImageLabel();

            // TODO: Seek book images from mods

            // Get path to localized book file and check it exists
            string path = Path.Combine(Application.streamingAssetsPath, textFolderName, bookImagesPath, filename);
            if (!File.Exists(path))
                return label;

            // Load image
            byte[] data = File.ReadAllBytes(path);
            if (data != null && data.Length > 0)
            {
                Texture2D image = new Texture2D(0, 0);
                image.LoadImage(data);
                label.Image = image;
            }

            return label;
        }

        void LayoutBookLabels()
        {
            if (!IsBookOpen)
                return;

            maxHeight = 0;
            scrollPosition = 0;
            pagePanel.Components.Clear();
            float x = 0, y = 0;
            foreach (TextLabel label in bookLabels)
            {
                label.Position = new Vector2(x, y);
                label.MaxWidth = (int)pagePanel.Size.x;
                label.RectRestrictedRenderArea = pagePanel.RectRestrictedRenderArea;
                label.RestrictedRenderAreaCoordinateType = TextLabel.RestrictedRenderArea_CoordinateType.ParentCoordinates;
                pagePanel.Components.Add(label);
                y += label.Size.y;
                maxHeight += label.Size.y;
            }
        }

        Color32 TryParseColor(string color)
        {
            try
            {
                // Must be RGB value, eg. 1F0A2B
                if (color.Length == 6)
                {
                    byte r = byte.Parse(color.Substring(0, 2), NumberStyles.HexNumber);
                    byte g = byte.Parse(color.Substring(2, 2), NumberStyles.HexNumber);
                    byte b = byte.Parse(color.Substring(4, 2), NumberStyles.HexNumber);
                    byte a = 0xff;
                    return new Color32(r, g, b, a);
                }
            }
            catch(Exception ex)
            {
                Debug.LogWarningFormat("DaggerfallBookReaderWindow.TryParseColor() exception: {0}", ex.Message);
            }

            // Fallback to default text color if parse failed
            return DaggerfallUI.DaggerfallDefaultTextColor;
        }

        float TryParseScale(string scale)
        {
            float output;
            if (float.TryParse(scale, out output))
                return output;

            // Fallback to 1.0 if parse failed
            return 1.0f;            
        }

        #endregion
    }
}
