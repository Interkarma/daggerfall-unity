// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
using System.Collections.Generic;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class DaggerfallBookReaderWindow : DaggerfallBaseWindow
    {
        const float scrollAmount = 24;
        const string nativeImgName = "BOOK00I0.IMG";

        Vector2 pagePanelPosition = new Vector2(55, 21);
        Vector2 pagePanelSize = new Vector2(210, 159);

        Texture2D nativeTexture;
        LabelFormatter labelFormatter = new LabelFormatter();
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

            // Setup panel to contain text labels
            pagePanel.Position = pagePanelPosition;
            pagePanel.Size = pagePanelSize;
            pagePanel.RectRestrictedRenderArea = new Rect(pagePanel.Position, pagePanel.Size);
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

            LayoutBook();
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
                LayoutBook();
                DaggerfallUI.Instance.PlayOneShot(openBook);
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

        public void OpenBook(int id)
        {
            bookLabels.Clear();
            IsBookOpen = labelFormatter.ReformatBook(id);
            if (IsBookOpen)
                bookLabels = labelFormatter.CreateLabels();
        }

        public void OpenBook(DaggerfallUnityItem target)
        {
            bookLabels.Clear();
            if (target == null || target.ItemGroup != ItemGroups.Books || target.IsArtifact)
                throw new Exception("Item is not a valid book for book reader UI.");

            IsBookOpen = labelFormatter.ReformatBook(target.message);
            if (IsBookOpen)
                bookLabels = labelFormatter.CreateLabels();
        }

        void LayoutBook()
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
    }
}
