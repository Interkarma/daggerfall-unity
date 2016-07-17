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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements the enter name window.
    /// </summary>
    public class CreateCharNameSelect : DaggerfallPopupWindow
    {
        const string nativeImgName = "CHAR00I0.IMG";

        Texture2D nativeTexture;
        TextBox textBox = new TextBox();
        Button okButton;

        public string CharacterName
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        public CreateCharNameSelect(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("CreateCharNameSelect: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Text edit box
            textBox.Position = new Vector2(100, 5);
            textBox.Size = new Vector2(214, 7);
            NativePanel.Components.Add(textBox);

            // OK button
            okButton = DaggerfallUI.AddButton(new Rect(263, 172, 39, 22), NativePanel);
            okButton.OnMouseClick += OkButton_OnMouseClick;
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Return))
                AcceptName();
        }

        void AcceptName()
        {
            if (textBox.Text.Length > 0)
                CloseWindow();
        }

        #region Event Handlers

        void OkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            AcceptName();
        }

        #endregion
    }
}