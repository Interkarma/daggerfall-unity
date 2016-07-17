// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Spinner for left/right number distribution.
    /// </summary>
    public class LeftRightSpinner : Panel
    {
        const string nativeImgName = "CHAR03I1.IMG";

        Texture2D nativeTexture;
        Button leftButton = new Button();
        Button rightButton = new Button();
        TextLabel valueLabel = new TextLabel();
        int value = 0;

        public int Value
        {
            get { return value; }
            set { SetValue(value); }
        }

        public LeftRightSpinner()
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
            Size = new Vector2(nativeTexture.width, nativeTexture.height);
            backgroundTexture = nativeTexture;

            // Add up/down buttons
            Components.Add(leftButton);
            Components.Add(rightButton);
            leftButton.Position = new Vector2(0, 0);
            leftButton.Size = new Vector2(11, 9);
            leftButton.OnMouseClick += LeftButton_OnMouseClick;
            rightButton.Position = new Vector2(26, 0);
            rightButton.Size = new Vector2(11, 9);
            rightButton.OnMouseClick += RightButton_OnMouseClick;

            // Add value label
            Components.Add(valueLabel);
            valueLabel.Position = new Vector2(0, 2);
            valueLabel.Size = new Vector2(15, 9);
            valueLabel.HorizontalAlignment = HorizontalAlignment.Center;
            valueLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            SetValue(this.value);
        }

        void SetValue(int value)
        {
            this.value = value;
            valueLabel.Text = value.ToString();
        }

        void LeftButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            RaiseOnLeftButtonClicked();
        }

        void RightButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            RaiseOnRightButtonClicked();
        }

        #region Events

        public delegate void OnLeftButtonClickedHandler();
        public event OnLeftButtonClickedHandler OnLeftButtonClicked;
        void RaiseOnLeftButtonClicked()
        {
            if (OnLeftButtonClicked != null)
                OnLeftButtonClicked();
        }

        public delegate void OnRightButtonClickedHandler();
        public event OnRightButtonClickedHandler OnRightButtonClicked;
        void RaiseOnRightButtonClicked()
        {
            if (OnRightButtonClicked != null)
                OnRightButtonClicked();
        }

        #endregion
    }
}