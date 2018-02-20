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
        const float iconScale = 0.5f;

        const string classicStealFilename = "classic-steal";
        const string classicGrabFilename = "classic-grab";
        const string classicInfoFilename = "classic-info";
        const string classicTalkFilename = "classic-talk";
        const float classicScale = 3f;

        Vector2 stealSize;
        Vector2 grabSize;
        Vector2 infoSize;
        Vector2 talkSize;

        public Texture2D StealTexture;
        public Texture2D GrabTexture;
        public Texture2D InfoTexture;
        public Texture2D TalkTexture;

        float displayScale;

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
                        Size = stealSize * displayScale;
                        break;
                    case PlayerActivateModes.Grab:
                        BackgroundTexture = GrabTexture;
                        Size = grabSize * displayScale;
                        break;
                    case PlayerActivateModes.Info:
                        BackgroundTexture = InfoTexture;
                        Size = infoSize * displayScale;
                        break;
                    case PlayerActivateModes.Talk:
                        BackgroundTexture = TalkTexture;
                        Size = talkSize * displayScale;
                        break;
                }
                Position = new Vector2((barWidth * 5) + (HUDVitals.borderSize * 2), Screen.height - HUDVitals.borderSize - Size.y);

                base.Update();
            }
        }

        void LoadAssets()
        {
            string stealFilename;
            string grabFilename;
            string infoFilename;
            string talkFilename;

            if (DaggerfallUnity.Settings.InteractionModeIcon.ToLower() == "classic")
            {
                stealFilename = classicStealFilename;
                grabFilename = classicGrabFilename;
                infoFilename = classicInfoFilename;
                talkFilename = classicTalkFilename;
                displayScale = classicScale;
            }
            else
            {
                stealFilename = iconStealFilename;
                grabFilename = iconGrabFilename;
                infoFilename = iconInfoFilename;
                talkFilename = iconTalkFilename;
                displayScale = iconScale;
            }

            if (TextureReplacement.CustomTextureExist(stealFilename))
                StealTexture = TextureReplacement.LoadCustomTexture(stealFilename);
            else
                StealTexture = Resources.Load<Texture2D>(stealFilename);
            stealSize = TextureReplacement.GetSize(StealTexture, stealFilename, true);

            if (TextureReplacement.CustomTextureExist(grabFilename))
                GrabTexture = TextureReplacement.LoadCustomTexture(grabFilename);
            else
                GrabTexture = Resources.Load<Texture2D>(grabFilename);
            grabSize = TextureReplacement.GetSize(GrabTexture, grabFilename, true);

            if (TextureReplacement.CustomTextureExist(infoFilename))
                InfoTexture = TextureReplacement.LoadCustomTexture(infoFilename);
            else
                InfoTexture = Resources.Load<Texture2D>(infoFilename);
            infoSize = TextureReplacement.GetSize(InfoTexture, infoFilename, true);

            if (TextureReplacement.CustomTextureExist(talkFilename))
                TalkTexture = TextureReplacement.LoadCustomTexture(talkFilename);
            else
                TalkTexture = Resources.Load<Texture2D>(talkFilename);
            talkSize = TextureReplacement.GetSize(TalkTexture, talkFilename, true);
        }
    }
}