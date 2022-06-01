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
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility.AssetInjection;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Utility;

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
        ImageData[] faceTextures = new ImageData[faceCount];
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
                    String filename = raceGender == Genders.Male ? raceTemplate.PaperDollHeadsMale : raceTemplate.PaperDollHeadsFemale;
                    faceTextures[i] = ImageReader.GetImageData(filename, i, 0, true, true);
                }
            }

            SetCurrentFace();
        }

        void SetCurrentFace()
        {
            currentFaceTexture = faceTextures[faceIndex].texture;

            if (currentFaceTexture != null)
            {
                facePanel.BackgroundTexture = currentFaceTexture;
                facePanel.Size = new Vector2(faceTextures[faceIndex].width, faceTextures[faceIndex].height);
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
