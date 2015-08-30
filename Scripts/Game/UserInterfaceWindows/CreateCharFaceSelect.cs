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
    /// Implements the select face window.
    /// </summary>
    public class CreateCharFaceSelect : DaggerfallPopupWindow
    {
        const string nativeImgName = "CHAR01I0.IMG";
        const int faceCount = 10;
        const int minFaceIndex = 0;
        const int maxFaceIndex = faceCount - 1;

        Texture2D nativeTexture;
        RaceTemplate raceTemplate;
        Genders raceGender;
        Panel faceDisplayPanel = new Panel();
        Panel facePanel = new Panel();
        Button prevFaceButton;
        Button nextFaceButton;
        Button okButton;
        int faceIndex = 0;
        Texture2D[] faceTextures = new Texture2D[faceCount];
        Texture2D currentFaceTexture;

        public CreateCharFaceSelect(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        public int FaceIndex
        {
            get { return faceIndex; }
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("CreateCharFaceSelect: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Face display panel
            faceDisplayPanel.Position = new Vector2(247, 25);
            faceDisplayPanel.Size = new Vector2(64, 40);
            NativePanel.Components.Add(faceDisplayPanel);

            // Face panel
            facePanel.HorizontalAlignment = HorizontalAlignment.Center;
            facePanel.VerticalAlignment = VerticalAlignment.Middle;
            faceDisplayPanel.Components.Add(facePanel);

            // Previous/Next buttons
            prevFaceButton = AddButton(new Rect(245, 69, 42, 9));
            prevFaceButton.OnMouseClick += PrevFaceButton_OnMouseClick;
            nextFaceButton = AddButton(new Rect(287, 69, 26, 9));
            nextFaceButton.OnMouseClick += NextFaceButton_OnMouseClick;

            // OK button
            okButton = AddButton(new Rect(263, 172, 39, 22));
            okButton.OnMouseClick += OkButton_OnMouseClick;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }

        #region Public Methods

        public void SetFaceTextures(RaceTemplate raceTemplate, Genders raceGender)
        {
            this.raceTemplate = raceTemplate;
            this.raceGender = raceGender;
            UpdateFaceTextures();
        }

        #endregion

        #region Event Handlers

        void PrevFaceButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            faceIndex--;
            if (faceIndex < minFaceIndex)
                faceIndex = maxFaceIndex;

            SetCurrentFace();
        }

        void NextFaceButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            faceIndex++;
            if (faceIndex >= maxFaceIndex)
                faceIndex = minFaceIndex;

            SetCurrentFace();
        }

        void OkButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        #endregion

        #region Private Methods

        void UpdateFaceTextures()
        {
            if (raceTemplate != null)
            {
                for (int i = 0; i < faceCount; i++)
                {
                    if (raceGender == Genders.Male)
                        faceTextures[i] = GetTextureFromCifRci(raceTemplate.PaperDollHeadsMale, i);
                    else if (raceGender == Genders.Female)
                        faceTextures[i] = GetTextureFromCifRci(raceTemplate.PaperDollHeadsFemale, i);
                }
            }

            SetCurrentFace();
        }

        void SetCurrentFace()
        {
            currentFaceTexture = faceTextures[faceIndex];
            if (currentFaceTexture != null)
            {
                facePanel.Size = new Vector2(currentFaceTexture.width, currentFaceTexture.height);
                facePanel.BackgroundTexture = currentFaceTexture;
            }
        }

        #endregion
    }
}