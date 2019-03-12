// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
        const int colorPreviewWidth = 400;
        const int colorPreviewHeight = 200;

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
            pickerPanel.Size = new Vector2(600, 250);
            pickerPanel.HorizontalAlignment = HorizontalAlignment.Center;
            pickerPanel.VerticalAlignment = VerticalAlignment.Middle;
            pickerPanel.BackgroundColor = new Color(0, 0, 0, 0.7f);
            pickerPanel.Outline.Enabled = true;
            ParentPanel.Components.Add(pickerPanel);

            var hexSymbol = new TextLabel();
            hexSymbol.Text = "#";
            hexSymbol.Position = new Vector2(20, 140);
            pickerPanel.Components.Add(hexSymbol);

            hexColor = new TextBox();
            hexColor.Position = new Vector2(20 + hexSymbol.Size.x, 140);
            hexColor.MaxCharacters = 8;
            hexColor.OnType += HexColor_OnType;
            pickerPanel.Components.Add(hexColor);

            rgbaColor = new TextLabel();
            rgbaColor.Position = new Vector2(20, 150);
            pickerPanel.Components.Add(rgbaColor);

            hsvColor = new TextLabel();
            hsvColor.Position = new Vector2(20, 160);
            pickerPanel.Components.Add(hsvColor);

            colorPreview = new Panel();
            colorPreview.Size = new Vector2(colorPreviewWidth, colorPreviewHeight);
            colorPreview.HorizontalAlignment = HorizontalAlignment.Right;
            colorPreview.Position = new Vector2(0, 1);
            pickerPanel.Components.Add(colorPreview);

            var crosshairTex = Resources.Load<Texture2D>("Crosshair");
            crosshair = new Panel();
            crosshair.BackgroundTexture = crosshairTex;
            crosshair.Size = new Vector2(crosshairTex.width, crosshairTex.height);
            colorPreview.Components.Add(crosshair);

            slider = new HorizontalSlider();
            slider.HorizontalAlignment = HorizontalAlignment.Right;
            slider.Position = new Vector2(0, 220);
            slider.Size = new Vector2(400.0f, 10.0f);
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

            if (Input.GetMouseButton(0) && colorPreview.MouseOverComponent)
            {
                if (!draggingThumb)
                    draggingThumb = true;

                Vector2 position = colorPreview.ScaledMousePosition;
                crosshair.Position = position;

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

            if (sender != null)
                sender.BackgroundColor = color;

            RaiseOnColorPickedEvent();
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
            return string.Format("HSV({0}, {1}, {2})", h, s , v);
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
