// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: Justin Steele
//
// Notes:
//

using UnityEngine;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements controls window.
    /// </summary>
    public class DaggerfallControlsWindow : DaggerfallPopupWindow
    {
        #region UI Rects



        #endregion

        #region UI Controls



        #endregion

        #region UI Textures

        Texture2D baseTexture;

        #endregion

        #region Fields

        const string baseTextureName = "CNFG00I0.IMG";

        #endregion

        #region Constructors

        public DaggerfallControlsWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
            StartGameBehaviour.OnNewGame += StartGameBehaviour_OnNewGame;
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load all the textures used by inventory system
            LoadTextures();

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundTexture = baseTexture;
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            //Load source textures
            baseTexture = ImageReader.GetTexture(baseTextureName);
        }

        #endregion

        #region Other Event Handlers

        private void StartGameBehaviour_OnNewGame()
        {
            // Reset certain elements on a new game
            if (IsSetup)
            {

            }
        }

        #endregion
    }
}