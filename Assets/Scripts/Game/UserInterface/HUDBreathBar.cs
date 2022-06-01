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
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Player breath bar for HUD.
    /// </summary>
    public class HUDBreathBar : Panel
    {
        public const int nativeBarWidth = 4;
        const int nativeBreathBarWidth = 6;
        public const int borderSize = 10;

        VerticalProgress breathBar = new VerticalProgress();
        PlayerEntity playerEntity;

        Color32 normalBreathColor = new Color32(247, 239, 41, 255);
        Color32 shortOnBreathColor = new Color32(148, 12, 0, 255);

        /// <summary>
        /// Gets or sets current breath as value between 0 and 1.
        /// </summary>
        public float Breath
        {
            get { return breathBar.Amount; }
            set { breathBar.Amount = value; UpdateBreathBar(); }
        }

        public Vector2? CustomBreathBarPosition { get; set; }
        public Vector2? CustomBreathBarSize { get; set; }

        public HUDBreathBar()
            :base()
        {
            playerEntity = GameManager.Instance.PlayerEntity;

            BackgroundColor = Color.clear;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Bottom;
            SetMargins(Margins.All, borderSize);
           
            Components.Add(breathBar);
        }

        public override void Update()
        {
            if (Enabled)
            {
                base.Update();
                UpdateBreathBar();
            }
        }

        void UpdateBreathBar()
        {
            float breathBarWidth = nativeBreathBarWidth * Scale.x;
            float breathBarHeight = playerEntity.Stats.LiveEndurance * Scale.y;

            breathBar.Position = (CustomBreathBarPosition != null) ? CustomBreathBarPosition.Value : Position + new Vector2(306 * Scale.x, (-92 * Scale.y) - breathBarHeight);
            breathBar.Size = (CustomBreathBarSize != null) ? CustomBreathBarSize.Value : new Vector2(breathBarWidth, breathBarHeight);

            breathBar.Amount = playerEntity.CurrentBreath / (float)playerEntity.MaxBreath;

            int threshold = ((GameManager.Instance.PlayerEntity.Stats.LiveEndurance) >> 3) + 4;
            if (threshold > GameManager.Instance.PlayerEntity.CurrentBreath)
                breathBar.Color = shortOnBreathColor;
            else
                breathBar.Color = normalBreathColor;
        }
    }
}
