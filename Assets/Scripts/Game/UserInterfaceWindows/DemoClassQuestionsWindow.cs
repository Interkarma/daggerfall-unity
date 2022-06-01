// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Demo popup window used to research and test parts of class questions UI.
    /// Not for use in live game. Can be removed once class questions UI fully functional.
    /// </summary>
    public class DemoClassQuestionsWindow : DaggerfallPopupWindow
    {
        const int scrollFrameCount = 16;
        const int magePaletteIndex = 128;
        const int roguePaletteIndex = 160;
        const int warriorPaletteIndex = 192;

        const string backgroundFile = "CHGN00I0.IMG";
        const string scroll0File = "SCRL00I0.GFX";
        const string scroll1File = "SCRL01I0.GFX";
        const string warriorCelFile = "WARRIOR.CEL";
        const string mageCelFile = "MAGE.CEL";
        const string rogueCelFile = "ROGUE.CEL";

        Vector2 mainPanelSize = new Vector2(320, 200);
        Vector2 scrollPanelSize = new Vector2(320, 80);
        Rect scrollUpButtonRect = new Rect(0, 119, 320, 16);
        Rect scrollDownButtonRect = new Rect(0, 183, 320, 16);

        DFBitmap backgroundBitmap;
        DFPalette basePalette;
        ScrollFrame[] scrollFrames;

        Panel mainPanel;
        Panel scrollPanel;
        Button scrollUpButton;
        Button scrollDownButton;

        int curScrollIndex = 0;

        struct ScrollFrame
        {
            public Color32[] colors;
            public Texture2D texture;
        }

        public DemoClassQuestionsWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
           : base(uiManager, previous)
        {
        }

        protected override void Setup()
        {
            LoadResources();

            // Background panel
            mainPanel = DaggerfallUI.AddPanel(NativePanel, AutoSizeModes.None);
            mainPanel.Size = mainPanelSize;
            mainPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = VerticalAlignment.Middle;
            mainPanel.BackgroundColor = Color.black;

            // Scroll panel
            scrollPanel = DaggerfallUI.AddPanel(NativePanel, AutoSizeModes.None);
            scrollPanel.Size = scrollPanelSize;
            scrollPanel.HorizontalAlignment = HorizontalAlignment.Center;
            scrollPanel.VerticalAlignment = VerticalAlignment.Bottom;

            // Scroll buttons
            scrollUpButton = DaggerfallUI.AddButton(scrollUpButtonRect, NativePanel);
            scrollDownButton = DaggerfallUI.AddButton(scrollDownButtonRect, NativePanel);

            // Set initial textures
            mainPanel.BackgroundTexture = GetBackgroundTexture();
            scrollPanel.BackgroundTexture = GetScrollFrameTexture();
        }

        public override void Update()
        {
            base.Update();

            // Scroll questions up or down
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (scrollUpButton.MouseOverComponent)
                    ScrollQuestions(-1);
                else if (scrollDownButton.MouseOverComponent)
                    ScrollQuestions(1);
            }
        }

        void ScrollQuestions(int step)
        {
            // Step scroll index
            curScrollIndex += step;
            scrollPanel.BackgroundTexture = GetScrollFrameTexture();

            // TODO: Scroll text and stop scrolling at top or bottom
        }

        #region Private Methods

        void LoadResources()
        {
            // Get background image and store base palette
            ImgFile img = new ImgFile(Path.Combine(DaggerfallUnity.Arena2Path, backgroundFile), FileUsage.UseMemory, true);
            backgroundBitmap = img.GetDFBitmap(0, 0);
            basePalette = backgroundBitmap.Palette;

            // Get scroll images and set palette
            GfxFile scroll0 = new GfxFile(Path.Combine(DaggerfallUnity.Arena2Path, scroll0File), FileUsage.UseMemory, true);
            GfxFile scroll1 = new GfxFile(Path.Combine(DaggerfallUnity.Arena2Path, scroll1File), FileUsage.UseMemory, true);
            scroll0.Palette = basePalette;
            scroll1.Palette = basePalette;

            // Build color buffers for all scroll frames ahead of time
            scrollFrames = new ScrollFrame[scrollFrameCount];
            for (int frame = 0; frame < scrollFrameCount; frame++)
            {
                if (frame < 8)
                    scrollFrames[frame].colors = scroll0.GetColor32(0, frame);
                else
                    scrollFrames[frame].colors = scroll1.GetColor32(0, frame - 8);
            }
        }

        // Always a positive modulus
        int mod(int a, int n)
        {
            return ((a % n) + n) % n;
        }

        Texture2D GetScrollFrameTexture()
        {
            // Generate scroll frame texture on first use and cache for subsequent uses
            //int modFrame = frame % scrollFrameCount;
            int modFrame = mod(curScrollIndex, scrollFrameCount);
            if (!scrollFrames[modFrame].texture)
            {
                ScrollFrame scrollFrame = scrollFrames[modFrame];
                scrollFrame.texture = new Texture2D((int)scrollPanelSize.x, (int)scrollPanelSize.y, TextureFormat.ARGB32, false);
                scrollFrame.texture.SetPixels32(scrollFrames[modFrame].colors);
                scrollFrame.texture.Apply();
                scrollFrame.texture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
                scrollFrames[modFrame] = scrollFrame;
            }

            return scrollFrames[modFrame].texture;
        }

        Texture2D GetBackgroundTexture()
        {
            DFPalette palette = new DFPalette(basePalette);
            // TODO: Set mage, rogue, warrior values
            backgroundBitmap.Palette = palette;

            Texture2D backgroundTexture = new Texture2D((int)mainPanelSize.x, (int)mainPanelSize.y);
            backgroundTexture.SetPixels32(backgroundBitmap.GetColor32());
            backgroundTexture.Apply();
            backgroundTexture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            return backgroundTexture;
        }

        #endregion
    }
}