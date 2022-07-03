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
using System;
using System.Collections.Generic;
using System.Text;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop.Game.UserInterface
{
    public class Checkbox : Panel
    {
        #region Fields

        const string uncheckedTextureFilename = "checkbox_unchecked";
        const string checkedTextureFilename = "checkbox_checked";

        int checkTextHorzOffset = 2;
        int checkTextVertOffset = 1;

        TextLabel label = new TextLabel();
        bool isChecked = false;

        Color checkboxColor = Color.white;
        Texture2D uncheckedTexture = null;
        Texture2D checkedTexture = null;
        Vector2 checkTextureSize = Vector2.zero;
        bool previousSDFState;

        public delegate void OnToggleStateHandler();
        public event OnToggleStateHandler OnToggleState;

        #endregion

        #region Properties

        public Color CheckBoxColor
        {
            get { return checkboxColor; }
            set { checkboxColor = value; }
        }

        public TextLabel Label
        {
            get { return label; }
        }

        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; }
        }

        public int TextHorzOffset
        {
            get { return checkTextHorzOffset; }
            set { checkTextHorzOffset = value; }
        }

        public int TextVertOffset
        {
            get { return checkTextVertOffset; }
            set { checkTextVertOffset = value; }
        }

        #endregion

        #region Constructors

        public Checkbox()
            : base()
        {
            label.Parent = this;
            label.ShadowPosition = Vector2.zero;
            OnMouseClick += Checkbox_OnMouseClick;
            checkboxColor = DaggerfallUI.DaggerfallDefaultTextColor;
            LoadResources();
        }

        #endregion

        #region Overrides

        public override void Update()
        {
            base.Update();

            if (label.Font != null && label.Font.IsSDFCapable != previousSDFState)
            {
                label.RefreshLayout();
                previousSDFState = label.Font.IsSDFCapable;
            }

            float width = checkTextureSize.x + checkTextHorzOffset + label.Size.x;
            float height = Mathf.Max(checkTextureSize.y, label.Size.y);
            Size = new Vector2(width, height);
        }

        public override void Draw()
        {
            base.Draw();

            // Exit if textures not set
            if (checkTextureSize == Vector2.zero ||
                uncheckedTexture == null ||
                checkedTexture == null)
            {
                return;
            }

            // Calculate rect and align checkbox or text to tallest element
            Rect rect = Rectangle;
            float checkHeight = checkTextureSize.y;
            float textHeight = label.Size.y;
            float textOffset = 0;
            if (checkHeight > textHeight)
                textOffset = (checkHeight - textHeight) / 2;
            else
                rect.y += ((Size.y - checkTextureSize.y) / 2f) * LocalScale.y;

            rect.width = checkTextureSize.x * LocalScale.x;
            rect.height = checkTextureSize.y * LocalScale.y;

            // Draw checkbox in current state
            if (!isChecked)
                DaggerfallUI.DrawTexture(rect, uncheckedTexture, ScaleMode.StretchToFill, true, checkboxColor);
            else
                DaggerfallUI.DrawTexture(rect, checkedTexture, ScaleMode.StretchToFill, true, checkboxColor);

            // Draw label
            label.Position = new Vector2(checkTextureSize.x + checkTextHorzOffset, textOffset + checkTextVertOffset);
            label.Draw();
        }

        #endregion

        #region Private Methods

        void LoadResources()
        {
            uncheckedTexture = Resources.Load<Texture2D>(uncheckedTextureFilename);
            checkedTexture = Resources.Load<Texture2D>(checkedTextureFilename);
            checkTextureSize = new Vector2(uncheckedTexture.width, uncheckedTexture.height);
        }

        #endregion

        #region Event Handlers

        private void Checkbox_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            isChecked = !isChecked;
            if (OnToggleState != null)
                OnToggleState();
        }

        #endregion
    }
}