// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:    
// 
// Notes:
//

using UnityEngine;
 
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
        const float iconScale = 0.8f;
        const float minimalScale = 0.5f;

        const string classicStealFilename = "classic-steal";
        const string classicGrabFilename = "classic-grab";
        const string classicInfoFilename = "classic-info";
        const string classicTalkFilename = "classic-talk";
        const float classicScale = 3f;

        const string colourStealFilename = "colour-steal";
        const string colourGrabFilename = "colour-grab";
        const string colourInfoFilename = "colour-info";
        const string colourTalkFilename = "colour-talk";
        const float colourScale = 1f;

        const string monoStealFilename = "mono-steal";
        const string monoGrabFilename = "mono-grab";
        const string monoInfoFilename = "mono-info";
        const string monoTalkFilename = "mono-talk";
        const float monoScale = 0.8f;

        private const string IconsFolder = "Icons/";

        Vector2 stealSize;
        Vector2 grabSize;
        Vector2 infoSize;
        Vector2 talkSize;

        public Texture2D StealTexture;
        public Texture2D GrabTexture;
        public Texture2D InfoTexture;
        public Texture2D TalkTexture;

        float displayScale;

        Texture2D crosshairTexture;
        Vector2 crosshairSize;
        bool crosshair = false;
        HUDCrosshair hudCrosshair;

        public HUDInteractionModeIcon(HUDCrosshair hudCrosshair)
            : base()
        {
            this.hudCrosshair = hudCrosshair;
            LoadAssets();
        }

        public override void Update()
        {
            if (Enabled)
            {
                if (crosshair)
                {
                    if (crosshairTexture == null) {
                        crosshairTexture = hudCrosshair.CrosshairTexture;
                        crosshairSize = hudCrosshair.crosshairSize;
                    }

                    PlayerActivateModes mode = GameManager.Instance.PlayerActivate.CurrentMode;
                    switch (mode)
                    {
                        case PlayerActivateModes.Steal:
                            hudCrosshair.CrosshairTexture = StealTexture;
                            hudCrosshair.crosshairSize = stealSize * displayScale;
                            break;
                        case PlayerActivateModes.Grab:
                            hudCrosshair.CrosshairTexture = crosshairTexture;
                            hudCrosshair.crosshairSize = crosshairSize;
                            break;
                        case PlayerActivateModes.Info:
                            hudCrosshair.CrosshairTexture = InfoTexture;
                            hudCrosshair.crosshairSize = infoSize * displayScale;
                            break;
                        case PlayerActivateModes.Talk:
                            hudCrosshair.CrosshairTexture = TalkTexture;
                            hudCrosshair.crosshairSize = talkSize * displayScale;
                            break;
                    }
                }
                else
                {
                    float barWidth = HUDVitals.nativeBarWidth * Scale.x;
                    float resScale = Scale.x > 3 ? 1 : 1 / Scale.x * 3;     // Scale down at low resolutions.

                    PlayerActivateModes mode = GameManager.Instance.PlayerActivate.CurrentMode;
                    switch (mode)
                    {
                        case PlayerActivateModes.Steal:
                            BackgroundTexture = StealTexture;
                            Size = stealSize * displayScale / resScale;
                            break;
                        case PlayerActivateModes.Grab:
                            BackgroundTexture = GrabTexture;
                            Size = grabSize * displayScale / resScale;
                            break;
                        case PlayerActivateModes.Info:
                            BackgroundTexture = InfoTexture;
                            Size = infoSize * displayScale / resScale;
                            break;
                        case PlayerActivateModes.Talk:
                            BackgroundTexture = TalkTexture;
                            Size = talkSize * displayScale / resScale;
                            break;
                    }
                    Position = new Vector2((barWidth * 5) + (HUDVitals.borderSize * 2), Screen.height - HUDVitals.borderSize - Size.y);
                }

                base.Update();
            }
        }

        void LoadAssets()
        {
            string stealFilename;
            string grabFilename;
            string infoFilename;
            string talkFilename;

            string iconSetting = DaggerfallUnity.Settings.InteractionModeIcon.ToLower();
            switch (iconSetting)
            {
                case "classic":
                case "classicxhair":
                    stealFilename = classicStealFilename;
                    grabFilename = classicGrabFilename;
                    infoFilename = classicInfoFilename;
                    talkFilename = classicTalkFilename;
                    displayScale = classicScale;
                    break;

                case "monochrome":
                    stealFilename = monoStealFilename;
                    grabFilename = monoGrabFilename;
                    infoFilename = monoInfoFilename;
                    talkFilename = monoTalkFilename;
                    displayScale = monoScale;
                    break;

                case "colour":
                case "colourxhair":
                    stealFilename = colourStealFilename;
                    grabFilename = colourGrabFilename;
                    infoFilename = colourInfoFilename;
                    talkFilename = colourTalkFilename;
                    displayScale = colourScale;
                    break;

                default:
                    stealFilename = iconStealFilename;
                    grabFilename = iconGrabFilename;
                    infoFilename = iconInfoFilename;
                    talkFilename = iconTalkFilename;

                    if (DaggerfallUnity.Settings.InteractionModeIcon.ToLower() == "minimal")
                        displayScale = minimalScale;
                    else
                        displayScale = iconScale;
                    break;
            }

            StealTexture = DaggerfallUI.GetTextureFromResources(IconsFolder + stealFilename, out stealSize);
            GrabTexture = DaggerfallUI.GetTextureFromResources(IconsFolder + grabFilename, out grabSize);
            InfoTexture = DaggerfallUI.GetTextureFromResources(IconsFolder + infoFilename, out infoSize);
            TalkTexture = DaggerfallUI.GetTextureFromResources(IconsFolder + talkFilename, out talkSize);

            crosshair = iconSetting.EndsWith("xhair");
        }
    }
}