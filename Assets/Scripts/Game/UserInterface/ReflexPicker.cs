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
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Implements a reusable player reflexes picker.
    /// This control is used twice during character creation.
    /// </summary>
    public class ReflexPicker : Panel
    {
        const string highlightImgName = "CHAR05I1.IMG";

        DaggerfallUnity dfUnity;
        Texture2D highlightTexture;
        PlayerReflexes playerReflexes;

        Button[] selectButtons = new Button[5];
        Rect buttonRect;
        Rect highlightRect;

        public PlayerReflexes PlayerReflexes
        {
            get { return playerReflexes; }
            set { SetPlayerReflexes(value); }
        }

        public ReflexPicker()
        {
            SetupControl();
        }

        public override void Update()
        {
            base.Update();

            buttonRect = selectButtons[(int)PlayerReflexes].Rectangle;
        }

        public override void Draw()
        {
            base.Draw();

            DaggerfallUI.DrawTextureWithTexCoords(buttonRect, highlightTexture, highlightRect);
        }

        #region Private Methods

        void SetupControl()
        {
            dfUnity = DaggerfallUnity.Instance;
            if (!dfUnity.IsReady)
                return;

            // Load highlight texture
            ImgFile imgFile = new ImgFile(Path.Combine(dfUnity.Arena2Path, highlightImgName), FileUsage.UseMemory, true);
            imgFile.LoadPalette(Path.Combine(dfUnity.Arena2Path, imgFile.PaletteName));
            highlightTexture = TextureReader.CreateFromAPIImage(imgFile, 0, 0, 0);
            highlightTexture.filterMode = DaggerfallUI.Instance.GlobalFilterMode;

            // Add buttons
            Vector2 buttonPos = new Vector2(0, 0);
            Vector2 buttonSize = new Vector2(66, 9);
            for (int i = 0; i < 5; i++)
            {
                selectButtons[i] = new Button();
                this.Components.Add(selectButtons[i]);
                selectButtons[i].Position = buttonPos;
                selectButtons[i].Size = buttonSize;
                selectButtons[i].Tag = i;
                selectButtons[i].OnMouseClick += ReflexButton_OnMouseClick;
                buttonPos.y += buttonSize.y;
            }

            // Set size of this panel
            Size = new Vector2(66, 45);

            // Set starting value
            PlayerReflexes = PlayerReflexes.Average;
        }

        void SetPlayerReflexes(PlayerReflexes value)
        {
            playerReflexes = value;

            highlightRect = new Rect(0f, 0.2f * (4 - (int)value), 1f, 0.2f);
        }

        #endregion

        #region Event Handlers

        void ReflexButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetPlayerReflexes((PlayerReflexes)sender.Tag);
        }

        #endregion
    }
}