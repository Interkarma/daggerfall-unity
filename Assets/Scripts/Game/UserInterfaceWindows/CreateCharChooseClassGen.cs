// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Numidium
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Utility.AssetInjection;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements class choice (selected or generated) window
    /// </summary>
    public class CreateCharChooseClassGen : DaggerfallPopupWindow
    {
        const string nativeImgName = "BUTN01I0.IMG";

        Texture2D nativeTexture;
        Panel classGenChoosePanel = new Panel();
        Button chooseClass;
        Button chooseGenerate;
        Rect chooseClassRect = new Rect(8, 41, 167, 43);
        Rect chooseQuestionsRect = new Rect(8, 100, 167, 34);
        bool choseGenerate = false;

        public CreateCharChooseClassGen(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        protected override void Setup()
        {
            base.Setup();

            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("CreateCharChooseClassGen: Could not load native texture.");

            // Create panel for window
            classGenChoosePanel.Size = TextureReplacement.GetSize(nativeTexture, nativeImgName);
            classGenChoosePanel.HorizontalAlignment = HorizontalAlignment.Center;
            classGenChoosePanel.VerticalAlignment = VerticalAlignment.Middle;
            classGenChoosePanel.BackgroundTexture = nativeTexture;
            classGenChoosePanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
            NativePanel.Components.Add(classGenChoosePanel);

            // Create buttons
            chooseClass = DaggerfallUI.AddButton(chooseClassRect, classGenChoosePanel);
            chooseClass.OnMouseClick += ChooseClass_OnMouseClick;
            chooseGenerate = DaggerfallUI.AddButton(chooseQuestionsRect, classGenChoosePanel);
            chooseGenerate.OnMouseClick += ChooseGenerate_OnMouseClick;
        }

        void ChooseGenerate_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            choseGenerate = true;
            CloseWindow();
        }

        void ChooseClass_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        public override void Update()
        {
            base.Update();
        }

        public bool ChoseGenerate
        {
            get { return choseGenerate; }
        }
    }
}
