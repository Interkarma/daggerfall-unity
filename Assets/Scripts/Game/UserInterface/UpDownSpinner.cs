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
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Spinner for up/down number distribution.
    /// </summary>
    public class UpDownSpinner : Panel
    {
        const string nativeImgName = "CHAR02I1.IMG";

        Texture2D nativeTexture;
        Button upButton = new Button();
        Button downButton = new Button();
        TextLabel valueLabel = new TextLabel();
        int value = 0;
        int minValue = 0;
        int maxValue = 0;
        float lastTickTime;
        float tickTimeInterval = 0.3f;
        Action action = Action.None;

        private enum Action
        {
            None,
            Up,
            Down
        }

        /// <summary>
        /// Gets or sets current value directly.
        /// </summary>
        public int Value
        {
            get { return value; }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets minimum value.
        /// If both MinValue and MaxValue are 0 then owner of control must handle up/down events as required.
        /// </summary>
        public int MinValue
        {
            get { return minValue; }
            set { SetRange(value, MaxValue); }
        }

        /// <summary>
        /// Gets or sets maximum value.
        /// If both MinValue and MaxValue are 0 then owner of control must handle up/down events as required.
        /// </summary>
        public int MaxValue
        {
            get { return minValue; }
            set { SetRange(MinValue, value); }
        }

        /// <summary>
        /// TickTimeInterval is the length of time in seconds between inc/dec when holding mouse button on up or down buttons. (must be > 0.1)
        /// </summary>
        public float TickTimeInterval
        {
            get { return tickTimeInterval; }
            set { if (value > 0.1f) tickTimeInterval = value; }
        }

        public UpDownSpinner()
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Load IMG texture
            ImgFile imgFile = new ImgFile(Path.Combine(dfUnity.Arena2Path, nativeImgName), FileUsage.UseMemory, true);
            imgFile.LoadPalette(Path.Combine(dfUnity.Arena2Path, imgFile.PaletteName));
            nativeTexture = TextureReader.CreateFromAPIImage(imgFile, 0, 0, 0);
            nativeTexture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            // Setup spinner panel
            Size = new Vector2(15, 20);
            backgroundTexture = nativeTexture;

            // Add up/down buttons
            Components.Add(upButton);
            Components.Add(downButton);
            upButton.Position = new Vector2(0, 0);
            upButton.Size = new Vector2(15, 7);
            upButton.OnMouseClick += UpButton_OnMouseClick;
            upButton.OnMouseDown += UpButton_OnMouseDown;
            upButton.OnMouseUp += UpDownButtons_OnMouseUp;
            downButton.Position = new Vector2(0, 13);
            downButton.Size = new Vector2(15, 7);
            downButton.OnMouseClick += DownButton_OnMouseClick;
            downButton.OnMouseDown += DownButton_OnMouseDown;
            downButton.OnMouseUp += UpDownButtons_OnMouseUp;

            // Add value label
            Components.Add(valueLabel);
            valueLabel.Position = new Vector2(0, 7);
            valueLabel.Size = new Vector2(15, 6);
            valueLabel.HorizontalAlignment = HorizontalAlignment.Center;
            valueLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            SetValue(this.value);
        }

        public UpDownSpinner(Rect spinnerScreenRect, Rect upButtonRect, Rect downButtonRect, Rect valueLabelRect, int initialValue = 0, Texture2D customBackground = null, Panel parentPanel = null)
        {
            DaggerfallUnity dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Setup spinner panel
            Position = new Vector2(spinnerScreenRect.xMin, spinnerScreenRect.yMin);
            Size = new Vector2(spinnerScreenRect.width, spinnerScreenRect.height);
            if (customBackground)
            {
                customBackground.filterMode = DaggerfallUI.Instance.GlobalFilterMode;
                backgroundTexture = customBackground;
            }

            // Add up/down buttons
            Components.Add(upButton);
            Components.Add(downButton);
            upButton.Position = new Vector2(upButtonRect.xMin, upButtonRect.yMin);
            upButton.Size = new Vector2(upButtonRect.width, upButtonRect.height);
            upButton.OnMouseClick += UpButton_OnMouseClick;
            upButton.OnMouseDown += UpButton_OnMouseDown;
            upButton.OnMouseUp += UpDownButtons_OnMouseUp;
            downButton.Position = new Vector2(downButtonRect.xMin, downButtonRect.yMin);
            downButton.Size = new Vector2(downButtonRect.width, downButtonRect.height);
            downButton.OnMouseClick += DownButton_OnMouseClick;
            downButton.OnMouseDown += DownButton_OnMouseDown;
            downButton.OnMouseUp += UpDownButtons_OnMouseUp;

            // Add value label
            Components.Add(valueLabel);
            valueLabel.Position = new Vector2(valueLabelRect.xMin, valueLabelRect.yMin);
            valueLabel.Size = new Vector2(valueLabelRect.width, valueLabelRect.height);
            valueLabel.HorizontalAlignment = HorizontalAlignment.Center;
            valueLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            SetValue(initialValue);

            // Add to parent panel
            if (parentPanel != null)
            {
                parentPanel.Components.Add(this);
            }
        }

        public void SetRange(int minValue, int maxValue)
        {
            // Set new range and reset current value to ensure clamped
            this.minValue = minValue;
            this.maxValue = maxValue;
            SetValue(value);
        }

        public void SetValue(int value)
        {
            // Clamp values if ranges specified, otherwise accept whatever owner wants
            if (minValue != 0 || maxValue != 0)
                this.value = Mathf.Clamp(value, minValue, maxValue);
            else
                this.value = value;

            valueLabel.Text = this.value.ToString();

            RaiseOnValueChanged();
        }

        public void SetMouseOverBackgroundColor(Color color)
        {
            upButton.MouseOverBackgroundColor = color;
            downButton.MouseOverBackgroundColor = color;
        }

        void UpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Auto +1 if ranges set
            if (minValue != 0 || maxValue != 0)
                SetValue(value + 1);

            lastTickTime = Time.realtimeSinceStartup;
            RaiseOnUpButtonClicked();
        }

        void DownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Auto -1 if ranges set
            if (minValue != 0 || maxValue != 0)
                SetValue(value - 1);

            lastTickTime = Time.realtimeSinceStartup;
            RaiseOnDownButtonClicked();
        }

        void UpButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            action = Action.Up;
        }
        void DownButton_OnMouseDown(BaseScreenComponent sender, Vector2 position)
        {
            action = Action.Down;
        }
        void UpDownButtons_OnMouseUp(BaseScreenComponent sender, Vector2 position)
        {
            action = Action.None;
        }

        public override void Update()
        {
            if (!Enabled)
                return;

            base.Update();

            if (Time.realtimeSinceStartup > lastTickTime + tickTimeInterval)
            {
                lastTickTime = Time.realtimeSinceStartup;

                // Handle any currently active mouse action.
                if (action == Action.Up)
                {   // Auto +1 if ranges set
                    if (minValue != 0 || maxValue != 0)
                        SetValue(value + 1);
                    RaiseOnUpButtonClicked();
                }
                else if (action == Action.Down)
                {   // Auto -1 if ranges set
                    if (minValue != 0 || maxValue != 0)
                        SetValue(value - 1);
                    RaiseOnDownButtonClicked();
                }
            }
        }

        #region Events

        public delegate void OnValueChangedHandler();
        public event OnValueChangedHandler OnValueChanged;
        void RaiseOnValueChanged()
        {
            if (OnValueChanged != null)
                OnValueChanged();
        }

        public delegate void OnUpButtonClickedHandler();
        public event OnUpButtonClickedHandler OnUpButtonClicked;
        void RaiseOnUpButtonClicked()
        {
            if (OnUpButtonClicked != null)
                OnUpButtonClicked();
        }

        public delegate void OnDownButtonClickedHandler();
        public event OnDownButtonClickedHandler OnDownButtonClicked;
        void RaiseOnDownButtonClicked()
        {
            if (OnDownButtonClicked != null)
                OnDownButtonClicked();
        }

        #endregion
    }
}