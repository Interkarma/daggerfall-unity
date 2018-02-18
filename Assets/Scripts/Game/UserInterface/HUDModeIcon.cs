// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Utility.AssetInjection;
 
namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Crosshair for HUD.
    /// </summary>
    public class HUDModeIcon : BaseScreenComponent
    {
        const string iconInfoFilename = "icon-info";
        const string iconTalkFilename = "icon-talk";

        Vector2 infoSize;
        Vector2 talkSize;

        public Texture2D IconTexture;
        public Texture2D InfoTexture;
        public Texture2D TalkTexture;
        public float IconScale = 1.0f;

        public HUDModeIcon()
            : base()
        {
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Bottom;
            LoadAssets();
        }

        public override void Update()
        {
            if (Enabled)
            {
                PlayerActivateModes mode = GameManager.Instance.PlayerActivate.CurrentMode;
                switch (mode)
                {
                    case PlayerActivateModes.Info:
                        BackgroundTexture = InfoTexture;
                        Size = infoSize * IconScale;
                        break;
                    case PlayerActivateModes.Talk:
                        BackgroundTexture = InfoTexture;
                        Size = infoSize * IconScale;
                        break;

                }

                base.Update();
            }
        }

        void LoadAssets()
        {
            if (TextureReplacement.CustomTextureExist(iconInfoFilename))
                InfoTexture = TextureReplacement.LoadCustomTexture(iconInfoFilename);
            else
                InfoTexture = Resources.Load<Texture2D>(iconInfoFilename);
            infoSize = TextureReplacement.GetSize(InfoTexture, iconInfoFilename, true);
        }
    }
}