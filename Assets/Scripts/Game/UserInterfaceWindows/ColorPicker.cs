// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:
//
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    public class ColorPicker : DaggerfallPopupWindow
    {
        #region Fields & Properties

        const int numberOfColors = 360;
        const int panelWidth = 280;
        const int panelheight = 120;
        const int colorPreviewWidth = (int)((float)panelWidth / 3 * 2);
        const int colorPreviewHeight = (int)((float)panelheight / 4 * 3);
        const float textScale = 0.6f;

        Panel pickerPanel;

        TextBox hexColor;
        TextLabel rgbaColor;
        TextLabel hsvColor;

        HorizontalSlider slider;
        Panel colorPreview;
        Panel crosshair;

        Color[] colors = GetColors();

        bool draggingThumb = false;

        public Color Color
        {
            get { return pickerPanel.BackgroundColor; }
            set { SetColor(value); }
        }

        Button sender;
        public Button Sender
        {
            get { return sender; }
        }

        #endregion

        #region Constructors

        public ColorPicker(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        public ColorPicker(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous, Button sender)
            : base(uiManager, previous)
        {
            this.sender = sender;
        }

        #endregion

        #region Overrides

        protected override void Setup()
        {
            base.Setup();

            ParentPanel.BackgroundColor = Color.clear;

            pickerPanel = new Panel();
            pickerPanel.Size = new Vector2(panelWidth, panelheight);
            pickerPanel.HorizontalAlignment = HorizontalAlignment.Center;
            pickerPanel.VerticalAlignment = VerticalAlignment.Middle;
            pickerPanel.BackgroundColor = new Color(0, 0, 0, 0.7f);
            pickerPanel.Outline.Enabled = true;
            NativePanel.Components.Add(pickerPanel);

            var hexSymbol = new TextLabel();
            hexSymbol.Text = "#";
            hexSymbol.Position = new Vector2(5, 30);
            pickerPanel.Components.Add(hexSymbol);

            hexColor = new TextBox();
            hexColor.Position = new Vector2(5 + hexSymbol.Size.x, 30);
            hexColor.MaxCharacters = 8;
            hexColor.Scale = new Vector2(10, 10);
            hexColor.OnType += HexColor_OnType;
            pickerPanel.Components.Add(hexColor);

            rgbaColor = new TextLabel();
            rgbaColor.TextScale = textScale;
            rgbaColor.Position = new Vector2(5, 55);
            pickerPanel.Components.Add(rgbaColor);

            hsvColor = new TextLabel();
            hsvColor.TextScale = textScale;
            hsvColor.Position = new Vector2(5, 60);
            pickerPanel.Components.Add(hsvColor);

            Button okButton = new Button();
            okButton.Label.Text = "OK";
            okButton.Size = new Vector2(39, 22);
            okButton.Position = new Vector2((panelWidth - colorPreviewWidth) / (float)2 - okButton.Size.x / 2, 75);
            okButton.OnMouseClick += OkButton_OnMouseClick;
            pickerPanel.Components.Add(okButton);

            colorPreview = new Panel();
            colorPreview.Size = new Vector2(colorPreviewWidth, colorPreviewHeight);
            colorPreview.HorizontalAlignment = HorizontalAlignment.Right;
            colorPreview.Position = new Vector2(0, 0);
            pickerPanel.Components.Add(colorPreview);

            var crosshairTex = Resources.Load<Texture2D>("Crosshair");
            crosshair = new Panel();
            crosshair.BackgroundTexture = crosshairTex;
            crosshair.Size = new Vector2(crosshairTex.width, crosshairTex.height) * 0.5f;
            colorPreview.Components.Add(crosshair);

            slider = new HorizontalSlider();
            slider.HorizontalAlignment = HorizontalAlignment.Right;
            slider.Position = new Vector2(0, colorPreviewHeight + 5);
            slider.Size = new Vector2(colorPreview.Size.x, 10);
            slider.DisplayUnits = 50;
            slider.TotalUnits = slider.DisplayUnits + colors.Length - 1;
            slider.BackgroundTexture = GetSliderBackground(colors);
            slider.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
            slider.TintColor = new Color(1, 1, 1, 0.7f);
            slider.OnScroll += Slider_OnScroll;
            pickerPanel.Components.Add(slider);

            if (sender != null)
                SetColor(sender.BackgroundColor);
            else
                SetColor(Color.white);
        }

        public override void Update()
        {
            base.Update();

            if (InputManager.Instance.GetMouseButton(0) && colorPreview.MouseOverComponent)
            {
                if (!draggingThumb)
                {
                    // Ensures button pressed while window is being opened is not detected as first click
                    if (!InputManager.Instance.GetMouseButtonDown(0))
                        return;

                    draggingThumb = true;
                }

                Vector2 position = colorPreview.ScaledMousePosition;
                crosshair.Position = position - crosshair.Size / 2;

                Color color = GetPixel(colorPreview.BackgroundTexture, position);
                hexColor.Text = ColorUtility.ToHtmlStringRGBA(color);
                rgbaColor.Text = color.ToString();
                hsvColor.Text = GetHSV(color);
                ConfirmColorPicked(color);

            }
            else if (draggingThumb)
            {
                draggingThumb = false;
            }
        }

        #endregion

        #region Private Methods

        private void SetColor(Color color)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            slider.ScrollIndex = Mathf.RoundToInt(numberOfColors * h);
            SetCrosshairPosition(s, v);

            colorPreview.BackgroundTexture = GetColorPreview(color);
            hexColor.Text = ColorUtility.ToHtmlStringRGBA(color);
            rgbaColor.Text = color.ToString();
            hsvColor.Text = GetHSV(color);

            ConfirmColorPicked(color);
        }

        private void SetCrosshairPosition(float saturation, float value)
        {
            float posX = colorPreview.Size.x * saturation;
            float posY = colorPreview.Size.y * (1 - value);
            crosshair.Position = new Vector2(posX, posY);
        }

        private void ConfirmColorPicked(Color color)
        {
            pickerPanel.BackgroundColor = color;
        }

        #endregion

        #region Static Methods

        private static Color[] GetColors()
        {
            Color[] colors = new Color[numberOfColors];

            for (int i = 0; i < numberOfColors; i++)
            {
                float h = i == 0 ? 0 : (float)i / numberOfColors;
                colors[i] = Color.HSVToRGB(h, 1, 1);
            }

            return colors;
        }

        private static Texture2D GetSliderBackground(Color[] colors)
        {
            var texture = new Texture2D(colors.Length, 1);
            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }

        private static Texture2D GetColorPreview(Color color)
        {
            var texture = new Texture2D(colorPreviewWidth, colorPreviewHeight);
            Color[] colors = texture.GetPixels();

            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);

            int index = 0;
            for (int i = 0; i < texture.height; i++)
            {
                v = i == 0 ? 0 : (float)i / texture.height;
                for (int j = 0; j < texture.width; j++)
                {
                    s = j == 0 ? 0 : (float)j / texture.width;
                    colors[index] = Color.HSVToRGB(h, s, v);
                    index++;
                }
            }

            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }

        private static string GetHSV(Color color)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            return string.Format("HSV({0}, {1}, {2})", h, s, v);
        }

        private static Color GetPixel(Texture2D tex, Vector2 position)
        {
            int posX = Mathf.RoundToInt(position.x);
            int posY = Mathf.RoundToInt(position.y);

            return tex.GetPixel(posX, tex.height - posY);
        }

        #endregion

        #region Event Handlers

        private void HexColor_OnType()
        {
            if (hexColor.Text.Length == 8)
            {
                Color color;
                if (ColorUtility.TryParseHtmlString("#" + hexColor.Text, out color))
                    SetColor(color);
            }
        }

        private void Slider_OnScroll()
        {
            colorPreview.BackgroundTexture = GetColorPreview(colors[slider.ScrollIndex]);
            Color color = GetPixel(colorPreview.BackgroundTexture, crosshair.Position);

            hexColor.Text = ColorUtility.ToHtmlStringRGBA(color);
            rgbaColor.Text = color.ToString();
            hsvColor.Text = GetHSV(color);
            ConfirmColorPicked(color);
        }

        private void OkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            if (this.sender != null)
                this.sender.BackgroundColor = pickerPanel.BackgroundColor;

            RaiseOnColorPickedEvent();
            CloseWindow();
        }

        public delegate void OnColorPickedEventHandler(Color color);
        public event OnColorPickedEventHandler OnColorPicked;
        void RaiseOnColorPickedEvent()
        {
            if (OnColorPicked != null)
                OnColorPicked(Color);
        }

        #endregion
    }
}
