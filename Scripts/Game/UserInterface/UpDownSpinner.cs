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

        public int Value
        {
            get { return value; }
            set { SetValue(value); }
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
            Size = new Vector2(nativeTexture.width, nativeTexture.height);
            backgroundTexture = nativeTexture;

            // Add up/down buttons
            Components.Add(upButton);
            Components.Add(downButton);
            upButton.Position = new Vector2(0, 0);
            upButton.Size = new Vector2(15, 7);
            upButton.OnMouseClick += UpButton_OnMouseClick;
            downButton.Position = new Vector2(0, 13);
            downButton.Size = new Vector2(15, 7);
            downButton.OnMouseClick += DownButton_OnMouseClick;

            // Add value label
            Components.Add(valueLabel);
            valueLabel.Position = new Vector2(0, 7);
            valueLabel.Size = new Vector2(15, 6);
            valueLabel.HorizontalAlignment = HorizontalAlignment.Center;
            valueLabel.ShadowColor = DaggerfallUI.DaggerfallAlternateShadowColor1;
            SetValue(this.value);
        }

        void SetValue(int value)
        {
            this.value = value;
            valueLabel.Text = value.ToString();
        }

        void UpButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            RaiseOnUpButtonClicked();
        }

        void DownButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            RaiseOnDownButtonClicked();
        }

        #region Events

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