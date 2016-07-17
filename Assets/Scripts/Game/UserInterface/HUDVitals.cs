// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Player vital signs for HUD.
    /// </summary>
    public class HUDVitals : Panel
    {
        const string healthBarFilename = "MAIN03I0.IMG";
        const string fatigueBarFilename = "MAIN04I0.IMG";
        const string magickaBarFilename = "MAIN05I0.IMG";
        const int nativeBarWidth = 4;
        const int nativeBarHeight = 32;
        const int borderSize = 10;

        VerticalProgress healthProgress = new VerticalProgress();
        VerticalProgress fatigueProgress = new VerticalProgress();
        VerticalProgress magickaProgress = new VerticalProgress();

        /// <summary>
        /// Gets or sets current health as value between 0 and 1.
        /// </summary>
        public float Health
        {
            get { return healthProgress.Amount; }
            set { healthProgress.Amount = value; }
        }

        /// <summary>
        /// Gets or sets current fatigue as value between 0 and 1.
        /// </summary>
        public float Fatigue
        {
            get { return fatigueProgress.Amount; }
            set { fatigueProgress.Amount = value; }
        }

        /// <summary>
        /// Gets or sets current magicka as value between 0 and 1.
        /// </summary>
        public float Magicka
        {
            get { return magickaProgress.Amount; }
            set { magickaProgress.Amount = value; }
        }

        public HUDVitals()
            :base()
        {
            LoadAssets();

            BackgroundColor = Color.clear;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Bottom;
            SetMargins(Margins.All, borderSize);
            
            healthProgress.VerticalAlignment = VerticalAlignment.Bottom;
            fatigueProgress.VerticalAlignment = VerticalAlignment.Bottom;
            magickaProgress.VerticalAlignment = VerticalAlignment.Bottom;

            Components.Add(healthProgress);
            Components.Add(fatigueProgress);
            Components.Add(magickaProgress);
        }

        public override void Update()
        {
            if (Enabled)
            {
                base.Update();

                float barWidth = nativeBarWidth * Scale.x;
                float barHeight = nativeBarHeight * Scale.y;

                Size = new Vector2(barWidth * 5, barHeight);

                healthProgress.Position = new Vector2(0, 0);
                healthProgress.Size = new Vector2(barWidth, barHeight);

                fatigueProgress.Position = new Vector2(barWidth * 2, 0);
                fatigueProgress.Size = new Vector2(barWidth, barHeight);

                magickaProgress.Position = new Vector2(barWidth * 4, 0);
                magickaProgress.Size = new Vector2(barWidth, barHeight);
            }
        }

        void LoadAssets()
        {
            if (DaggerfallUnity.Settings.SwapHealthAndFatigueColors)
            {
                healthProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(fatigueBarFilename);
                fatigueProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(healthBarFilename);
            }
            else
            {
                healthProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(healthBarFilename);
                fatigueProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(fatigueBarFilename);
            }
            magickaProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(magickaBarFilename);
        }
    }
}