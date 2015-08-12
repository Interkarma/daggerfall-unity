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
using System.IO;
using System.Collections;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Demo.UserInterface;

namespace DaggerfallWorkshop.Demo.UserInterfaceWindows
{
    public abstract class DaggerfallBaseWindow : UserInterfaceWindow
    {
        public const int nativeScreenWidth = 320;
        public const int nativeScreenHeight = 200;

        bool isSetup;
        DaggerfallUnity dfUnity;
        Panel screenPanel = new Panel();      // Parent screen panel fits to entire viewport
        Panel nativePanel = new Panel();      // Native panel is 320x200 child panel scaled to fit parent

        public DaggerfallBaseWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
            screenPanel.Components.Add(nativePanel);   
            nativePanel.Scaling = Scaling.ScaleToFit;
            nativePanel.HorizontalAlignment = HorizontalAlignment.Center;
            nativePanel.BackgroundTextureLayout = TextureLayout.StretchToFill;
            nativePanel.Size = new Vector2(nativeScreenWidth, nativeScreenHeight);
        }

        protected DaggerfallUnity DaggerfallUnity
        {
            get { return dfUnity; }
        }

        protected bool IsSetup
        {
            get { return isSetup; }
            set { isSetup = value; }
        }

        protected Panel NativePanel
        {
            get { return nativePanel; }
        }

        public override void Update()
        {
            // DaggerfallUnity must be ready
            if (dfUnity == null)
                dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Must be setup
            if (!isSetup)
            {
                Setup();
                return;
            }

            // Process messages in queue
            if (uiManager.MessageCount > 0)
                ProcessMessageQueue();

            screenPanel.Update();
        }

        public override void Draw()
        {
            screenPanel.Draw();
        }

        protected abstract void Setup();

        #region Protected Methods

        protected virtual void ProcessMessageQueue()
        {
        }

        #endregion

        #region Setup Helpers

        protected Button AddButton(Vector2 position, Vector2 size)
        {
            Button button = new Button();
            button.Position = position;
            button.Size = size;
            NativePanel.Components.Add(button);

            return button;
        }

        protected Button AddButton(Rect rect)
        {
            return AddButton(
                new Vector2(rect.x, rect.y),
                new Vector2(rect.width, rect.height));
        }

        protected Button AddButton(Vector2 position, Vector2 size, string clickMessage)
        {
            Button button = AddButton(position, size);
            button.ClickMessage = clickMessage;
            
            return button;
        }

        protected Button AddButton(Vector2 position, Vector2 size, string clickMessage, string doubleClickMessage)
        {
            Button button = AddButton(position, size);
            button.ClickMessage = clickMessage;
            button.DoubleClickMessage = doubleClickMessage;

            return button;
        }

        protected TextLabel AddTextLabel(PixelFont font, Vector2 position, string text, int glyphSpacing = 1)
        {
            TextLabel textLabel = new TextLabel();
            textLabel.Scaling = Scaling.None;
            textLabel.Font = font;
            textLabel.Position = position;
            textLabel.FilterMode = dfUnity.MaterialReader.MainFilterMode;
            textLabel.GlyphSpacing = glyphSpacing;
            textLabel.Text = text;
            NativePanel.Components.Add(textLabel);

            return textLabel;
        }

        protected Outline AddOutline(Rect rect, Color color)
        {
            Outline outline = new Outline();
            outline.Scaling = Scaling.None;
            outline.Color = color;
            outline.Position = new Vector2(rect.x, rect.y);
            outline.Size = new Vector2(rect.width, rect.height);
            NativePanel.Components.Add(outline);

            return outline;
        }

        protected Texture2D GetTextureFromImg(string name, TextureFormat format = TextureFormat.ARGB32)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            ImgFile imgFile = new ImgFile(Path.Combine(dfUnity.Arena2Path, name), FileUsage.UseMemory, true);
            imgFile.LoadPalette(Path.Combine(dfUnity.Arena2Path, imgFile.PaletteName));
            Texture2D texture = GetTextureFromImg(imgFile, format);
            texture.filterMode = dfUnity.MaterialReader.MainFilterMode;

            return texture;
        }

        protected Texture2D GetTextureFromImg(ImgFile img, TextureFormat format = TextureFormat.ARGB32)
        {
            DFBitmap bitmap = img.GetDFBitmap();
            Texture2D texture = new Texture2D(bitmap.Width, bitmap.Height, format, false);
            texture.SetPixels32(img.GetColor32(bitmap, 0));
            texture.Apply(false, true);

            return texture;
        }

        protected Texture2D GetTextureFromSaveImage(SaveImage image, TextureFormat format = TextureFormat.ARGB32)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return null;

            DFBitmap bitmap = image.GetDFBitmap();
            Texture2D texture = new Texture2D(bitmap.Width, bitmap.Height, format, false);
            texture.SetPixels32(image.GetColor32(bitmap, 0));
            texture.Apply(false, true);
            texture.filterMode = dfUnity.MaterialReader.MainFilterMode;

            return texture;
        }

        #endregion
    }
}