// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Player;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements custom class creator window.
    /// </summary>
    public class CreateCharCustomClass : DaggerfallPopupWindow
    {
        const string nativeImgName = "CUST00I0.IMG";

        Texture2D nativeTexture;
		DaggerfallFont font;
		StatsRollout statsRollout;
		TextBox textBox = new TextBox();

        public CreateCharCustomClass(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            if (IsSetup)
                return;

            // Load native texture
            nativeTexture = DaggerfallUI.GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("CreateCharCustomClass: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Add stats rollout
            statsRollout = new StatsRollout(false, true);
            statsRollout.Position = new Vector2(0, 0);
            NativePanel.Components.Add(statsRollout);

			// Add name textbox
			textBox.Position = new Vector2(100, 5);
			textBox.Size = new Vector2(214, 7);
			NativePanel.Components.Add(textBox);

            IsSetup = true;
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

        #endregion

        #region Private Methods

        #endregion
    }    
}