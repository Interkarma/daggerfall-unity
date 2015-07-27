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
        bool isSetup;
        DaggerfallUnity dfUnity;
        PanelScreenComponent screenPanel = new PanelScreenComponent();      // Screen panel fits to entire viewport
        PanelScreenComponent nativePanel = new PanelScreenComponent();      // Native panel is 320x200 child panel scaled to fit viewport

        public DaggerfallBaseWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
            screenPanel.Components.Add(nativePanel);   
            nativePanel.Scaling = Scaling.ScaleToFit;
            nativePanel.HorizontalAlignment = HorizontalAlignment.Center;
            nativePanel.BackgroundTextureLayout = TextureLayout.StretchToFill;
            nativePanel.Size = new Vector2(NativeScreenWidth, NativeScreenHeight);
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

        protected PanelScreenComponent NativePanel
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

            screenPanel.Update();
        }

        public override void Draw()
        {
            screenPanel.Draw();
        }

        protected abstract void Setup();

        #region Protected Methods

        protected ButtonScreenComponent AddButton(Vector2 position, Vector2 size)
        {
            ButtonScreenComponent button = new ButtonScreenComponent();
            button.Position = position;
            button.Size = size;
            NativePanel.Components.Add(button);

            return button;
        }

        protected ButtonScreenComponent AddButton(Vector2 position, Vector2 size, string clickMessage)
        {
            ButtonScreenComponent button = AddButton(position, size);
            button.ClickMessage = clickMessage;
            
            return button;
        }

        protected ButtonScreenComponent AddButton(Vector2 position, Vector2 size, string clickMessage, string doubleClickMessage)
        {
            ButtonScreenComponent button = AddButton(position, size);
            button.ClickMessage = clickMessage;
            button.DoubleClickMessage = doubleClickMessage;

            return button;
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
            texture.SetPixels32(img.GetColors32(bitmap, 0));
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
            texture.SetPixels32(image.GetColors32(bitmap, 0));
            texture.Apply(false, true);
            texture.filterMode = dfUnity.MaterialReader.MainFilterMode;

            return texture;
        }

        #endregion
    }
}