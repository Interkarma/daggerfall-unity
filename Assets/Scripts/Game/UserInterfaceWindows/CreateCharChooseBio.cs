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
    /// Implements bio choice (automatically or manually generate) window
    /// </summary>
    public class CreateCharChooseBio : DaggerfallPopupWindow
    {
        const string nativeImgName = "BUTN02I0.IMG";

        Texture2D nativeTexture;
        Panel bioChoosePanel = new Panel();
        Button chooseGenerate;
        Button chooseQuestions;
        Rect chooseGenerateRect = new Rect(8, 41, 167, 54);
        Rect chooseQuestionsRect = new Rect(8, 113, 167, 46);
        bool choseQuestions = false;

        public CreateCharChooseBio(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        protected override void Setup()
        {
            base.Setup();

            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("CreateCharChooseBio: Could not load native texture.");

            // Create panel for window
            bioChoosePanel.Size = TextureReplacement.GetSize(nativeTexture, nativeImgName);
            bioChoosePanel.HorizontalAlignment = HorizontalAlignment.Center;
            bioChoosePanel.VerticalAlignment = VerticalAlignment.Middle;
            bioChoosePanel.BackgroundTexture = nativeTexture;
            bioChoosePanel.BackgroundTextureLayout = BackgroundLayout.StretchToFill;
            NativePanel.Components.Add(bioChoosePanel);

            // Create buttons
            chooseGenerate = DaggerfallUI.AddButton(chooseGenerateRect, bioChoosePanel);
            chooseGenerate.OnMouseClick += ChooseGenerate_OnMouseClick;
            chooseQuestions = DaggerfallUI.AddButton(chooseQuestionsRect, bioChoosePanel);
            chooseQuestions.OnMouseClick += ChooseQuestions_OnMouseClick;
        }

        void ChooseGenerate_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        void ChooseQuestions_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            choseQuestions = true;
            CloseWindow();
        }

        public override void Update()
        {
            base.Update();
        }

        public bool ChoseQuestions
        {
            get { return choseQuestions; }
        }
    }
}