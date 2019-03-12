// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
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
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Player;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Implements a reusable player face picker.
    /// This control is used twice during character creation.
    /// </summary>
    public class FacePicker : Panel
    {
        const int faceCount = 10;
        const int minFaceIndex = 0;
        const int maxFaceIndex = faceCount - 1;

        RaceTemplate raceTemplate;
        Genders raceGender;
        Panel faceDisplayPanel = new Panel();
        Panel facePanel = new Panel();
        Button prevFaceButton;
        Button nextFaceButton;
        int faceIndex = 0;
        Texture2D[] faceTextures = new Texture2D[faceCount];
        Texture2D currentFaceTexture;

        public int FaceIndex
        {
            get { return faceIndex; }
            set { faceIndex = value; SetCurrentFace(); }
        }

        public FacePicker()
        {
            // Face display panel
            faceDisplayPanel.Position = new Vector2(247, 25);
            faceDisplayPanel.Size = new Vector2(64, 40);
            this.Components.Add(faceDisplayPanel);

            // Face panel
            facePanel.HorizontalAlignment = HorizontalAlignment.Center;
            facePanel.VerticalAlignment = VerticalAlignment.Middle;
            faceDisplayPanel.Components.Add(facePanel);

            // Previous/Next buttons
            prevFaceButton = DaggerfallUI.AddButton(new Rect(245, 69, 42, 9), this);
            prevFaceButton.OnMouseClick += PrevFaceButton_OnMouseClick;
            nextFaceButton = DaggerfallUI.AddButton(new Rect(287, 69, 26, 9), this);
            nextFaceButton.OnMouseClick += NextFaceButton_OnMouseClick;
        }

        #region Public Methods

        public void SetFaceTextures(RaceTemplate raceTemplate, Genders raceGender)
        {
            this.raceTemplate = raceTemplate;
            this.raceGender = raceGender;
            UpdateFaceTextures();
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
                        faceTextures[i] = DaggerfallUI.GetTextureFromCifRci(raceTemplate.PaperDollHeadsMale, i);
                    else if (raceGender == Genders.Female)
                        faceTextures[i] = DaggerfallUI.GetTextureFromCifRci(raceTemplate.PaperDollHeadsFemale, i);
                }
            }

            SetCurrentFace();
        }

        void SetCurrentFace()
        {
            currentFaceTexture = faceTextures[faceIndex];

            if (currentFaceTexture != null)
            {
                if (raceGender == Genders.Male)
                    facePanel.Size = TextureReplacement.GetSize(currentFaceTexture, raceTemplate.PaperDollHeadsMale, faceIndex);
                else if (raceGender == Genders.Female)
                    facePanel.Size = TextureReplacement.GetSize(currentFaceTexture, raceTemplate.PaperDollHeadsFemale, faceIndex);

                facePanel.BackgroundTexture = currentFaceTexture;
            }
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
            if (faceIndex > maxFaceIndex)
                faceIndex = minFaceIndex;

            SetCurrentFace();
        }

        #endregion
    }
}
