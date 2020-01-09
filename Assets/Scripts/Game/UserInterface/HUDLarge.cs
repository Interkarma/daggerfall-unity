// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Implements the large HUD in Daggerfall Unity as just another overlay inside of primary HUD.
    /// This is so it can work uniformly in both widescreen and retro modes.
    /// Other HUD elements can still work alongside large HUD, but some will be disabled as they occupy same screen area.
    /// </summary>
    public class HUDLarge : BaseScreenComponent
    {
        const string mainFilename = "MAIN00I0.IMG";

        Texture2D mainTexture;

        PlayerEntity playerEntity;

        public HUDLarge()
            : base()
        {
            playerEntity = GameManager.Instance.PlayerEntity;
            LoadAssets();

            BackgroundTexture = mainTexture;
        }

        void LoadAssets()
        {
            mainTexture = ImageReader.GetTexture(mainFilename);
        }

        public override void Update()
        {
            if (Enabled)
            {
            }
        }
    }
}