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
    /// InteractionModeIcon for HUD.
    /// </summary>
    public class HUDInteractionModeIcon : BaseScreenComponent
    {
        const string iconStealFilename = "icon-steal";
        const string iconGrabFilename = "icon-grab";
        const string iconInfoFilename = "icon-info";
        const string iconTalkFilename = "icon-talk";

        Vector2 stealSize;
        Vector2 grabSize;
        Vector2 infoSize;
        Vector2 talkSize;

        public Texture2D StealTexture;
        public Texture2D GrabTexture;
        public Texture2D InfoTexture;
        public Texture2D TalkTexture;
        public float IconScale = 0.5f;

        public HUDInteractionModeIcon()
            : base()
        {
            LoadAssets();
        }

        public override void Update()
        {
            if (Enabled)
            {
                float barWidth = HUDVitals.nativeBarWidth * Scale.x;

                PlayerActivateModes mode = GameManager.Instance.PlayerActivate.CurrentMode;
                switch (mode)
                {
                    case PlayerActivateModes.Steal:
                        BackgroundTexture = StealTexture;
                        Size = stealSize * IconScale;
                        break;
                    case PlayerActivateModes.Grab:
                        BackgroundTexture = GrabTexture;
                        Size = grabSize * IconScale;
                        break;
                    case PlayerActivateModes.Info:
                        BackgroundTexture = InfoTexture;
                        Size = infoSize * IconScale;
                        break;
                    case PlayerActivateModes.Talk:
                        BackgroundTexture = TalkTexture;
                        Size = talkSize * IconScale;
                        break;
                }
                Position = new Vector2((barWidth * 5) + (HUDVitals.borderSize * 2), Screen.height - HUDVitals.borderSize - Size.y);

                base.Update();
            }
        }

        void LoadAssets()
        {
            if (TextureReplacement.CustomTextureExist(iconStealFilename))
                StealTexture = TextureReplacement.LoadCustomTexture(iconStealFilename);
            else
                StealTexture = Resources.Load<Texture2D>(iconStealFilename);
            stealSize = TextureReplacement.GetSize(StealTexture, iconStealFilename, true);

            if (TextureReplacement.CustomTextureExist(iconGrabFilename))
                GrabTexture = TextureReplacement.LoadCustomTexture(iconGrabFilename);
            else
                GrabTexture = Resources.Load<Texture2D>(iconGrabFilename);
            grabSize = TextureReplacement.GetSize(GrabTexture, iconGrabFilename, true);

            if (TextureReplacement.CustomTextureExist(iconInfoFilename))
                InfoTexture = TextureReplacement.LoadCustomTexture(iconInfoFilename);
            else
                InfoTexture = Resources.Load<Texture2D>(iconInfoFilename);
            infoSize = TextureReplacement.GetSize(InfoTexture, iconInfoFilename, true);

            if (TextureReplacement.CustomTextureExist(iconTalkFilename))
                TalkTexture = TextureReplacement.LoadCustomTexture(iconTalkFilename);
            else
                TalkTexture = Resources.Load<Texture2D>(iconTalkFilename);
            talkSize = TextureReplacement.GetSize(TalkTexture, iconTalkFilename, true);
        }
    }
}