// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2023 Daggerfall Workshop
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
    /// Player vital signs for HUD.
    /// </summary>
    public class HUDVitals : Panel
    {
        const string healthBarFilename = "MAIN03I0.IMG";
        const string fatigueBarFilename = "MAIN04I0.IMG";
        const string magickaBarFilename = "MAIN05I0.IMG";
        public const int nativeBarWidth = 4;
        const int nativeBarHeight = 32;
        public const int borderSize = 10;

        VitalsChangeDetector vitalsDetector = GameManager.Instance.VitalsChangeDetector;
        VerticalProgressSmoother healthBar = new VerticalProgressSmoother();
        VerticalProgressSmoother fatigueBar = new VerticalProgressSmoother();
        VerticalProgressSmoother magickaBar = new VerticalProgressSmoother();
        VerticalProgressSmoother healthBarLoss = new VerticalProgressSmoother();
        VerticalProgressSmoother fatigueBarLoss = new VerticalProgressSmoother();
        VerticalProgressSmoother magickaBarLoss = new VerticalProgressSmoother();
        VerticalProgress healthBarGain = new VerticalProgress();
        VerticalProgress fatigueBarGain = new VerticalProgress();
        VerticalProgress magickaBarGain = new VerticalProgress();
        PlayerEntity playerEntity;

        Color healthLossColor = new Color(0, 0.22f, 0);
        Color fatigueLossColor = new Color(0.44f, 0, 0);
        Color magickaLossColor = new Color(0, 0, 0.44f);
        Color healthGainColor = new Color(0.60f, 1f, 0.60f);
        Color fatigueGainColor = new Color(1f, 0.50f, 0.50f);
        Color magickaGainColor = new Color(0.70f, 0.70f, 1f);

        /// <summary>
        /// Gets or sets current health as value between 0 and 1.
        /// </summary>
        public float Health
        {
            get { return healthBarGain.Amount; }
            set { healthBarGain.Amount = value; }
        }

        /// <summary>
        /// Gets or sets current fatigue as value between 0 and 1.
        /// </summary>
        public float Fatigue
        {
            get { return fatigueBarGain.Amount; }
            set { fatigueBarGain.Amount = value; }
        }

        /// <summary>
        /// Gets or sets current magicka as value between 0 and 1.
        /// </summary>
        public float Magicka
        {
            get { return magickaBarGain.Amount; }
            set { magickaBarGain.Amount = value; }
        }

        public Vector2? CustomHealthBarPosition { get; set; }
        public Vector2? CustomHealthBarSize { get; set; }

        public Vector2? CustomFatigueBarPosition { get; set; }
        public Vector2? CustomFatigueBarSize { get; set; }

        public Vector2? CustomMagickaBarPosition { get; set; }
        public Vector2? CustomMagickaBarSize { get; set; }

        public HUDVitals()
            :base()
        {
            playerEntity = GameManager.Instance.PlayerEntity;
            LoadAssets();

            BackgroundColor = Color.clear;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Bottom;
            SetMargins(Margins.All, borderSize);
            
            healthBar.VerticalAlignment = VerticalAlignment.Bottom;
            fatigueBar.VerticalAlignment = VerticalAlignment.Bottom;
            magickaBar.VerticalAlignment = VerticalAlignment.Bottom;

            if (DaggerfallUnity.Settings.EnableVitalsIndicators)
            {
                healthBarLoss.VerticalAlignment = VerticalAlignment.Bottom;
                fatigueBarLoss.VerticalAlignment = VerticalAlignment.Bottom;
                magickaBarLoss.VerticalAlignment = VerticalAlignment.Bottom;
                healthBarGain.VerticalAlignment = VerticalAlignment.Bottom;
                fatigueBarGain.VerticalAlignment = VerticalAlignment.Bottom;
                magickaBarGain.VerticalAlignment = VerticalAlignment.Bottom;

                // to make bar appear behind other bars, add it first.
                Components.Add(healthBarLoss);
                Components.Add(fatigueBarLoss);
                Components.Add(magickaBarLoss);
                Components.Add(healthBarGain);
                Components.Add(fatigueBarGain);
                Components.Add(magickaBarGain);
            }

            Components.Add(healthBar);
            Components.Add(fatigueBar);
            Components.Add(magickaBar);

            VitalsChangeDetector.OnReset += VitalChangeDetector_OnReset;
            vitalsDetector.HealthChanged += VitalsDetector_HealthChanged;
            vitalsDetector.FatigueChanged += VitalsDetector_FatigueChanged;
            vitalsDetector.MagickaChanged += VitalsDetector_MagickaChanged;
        }

        public void SetAllHorizontalAlignment(HorizontalAlignment alignment)
        {
            healthBar.HorizontalAlignment = alignment;
            fatigueBar.HorizontalAlignment = alignment;
            magickaBar.HorizontalAlignment = alignment;
            healthBarLoss.HorizontalAlignment = alignment;
            fatigueBarLoss.HorizontalAlignment = alignment;
            magickaBarLoss.HorizontalAlignment = alignment;
            healthBarGain.HorizontalAlignment = alignment;
            fatigueBarGain.HorizontalAlignment = alignment;
            magickaBarGain.HorizontalAlignment = alignment;
        }

        public void SetAllVerticalAlignment(VerticalAlignment alignment)
        {
            healthBar.VerticalAlignment = alignment;
            fatigueBar.VerticalAlignment = alignment;
            magickaBar.VerticalAlignment = alignment;
            healthBarLoss.VerticalAlignment = alignment;
            fatigueBarLoss.VerticalAlignment = alignment;
            magickaBarLoss.VerticalAlignment = alignment;
            healthBarGain.VerticalAlignment = alignment;
            fatigueBarGain.VerticalAlignment = alignment;
            magickaBarGain.VerticalAlignment = alignment;
        }

        public void SetAllAutoSize(AutoSizeModes mode)
        {
            healthBar.AutoSize = mode;
            fatigueBar.AutoSize = mode;
            magickaBar.AutoSize = mode;
            healthBarLoss.AutoSize = mode;
            fatigueBarLoss.AutoSize = mode;
            magickaBarLoss.AutoSize = mode;
            healthBarGain.AutoSize = mode;
            fatigueBarGain.AutoSize = mode;
            magickaBarGain.AutoSize = mode;
        }

        public void SetAllParent(Panel parent)
        {
            healthBar.Parent = parent;
            fatigueBar.Parent = parent;
            magickaBar.Parent = parent;
            healthBarLoss.Parent = parent;
            fatigueBarLoss.Parent = parent;
            magickaBarLoss.Parent = parent;
            healthBarGain.Parent = parent;
            fatigueBarGain.Parent = parent;
            magickaBarGain.Parent = parent;
        }

        void LoadAssets()
        {
            if (DaggerfallUnity.Settings.SwapHealthAndFatigueColors)
            {
                healthBar.ProgressTexture = DaggerfallUI.GetTextureFromImg(fatigueBarFilename);
                fatigueBar.ProgressTexture = DaggerfallUI.GetTextureFromImg(healthBarFilename);
                healthBarLoss.Color = fatigueLossColor;
                fatigueBarLoss.Color = healthLossColor;
                healthBarGain.Color = fatigueGainColor;
                fatigueBarGain.Color = healthGainColor;
            }
            else
            {
                healthBar.ProgressTexture = DaggerfallUI.GetTextureFromImg(healthBarFilename);
                fatigueBar.ProgressTexture = DaggerfallUI.GetTextureFromImg(fatigueBarFilename);
                healthBarLoss.Color = healthLossColor;
                fatigueBarLoss.Color = fatigueLossColor;
                healthBarGain.Color = healthGainColor;
                fatigueBarGain.Color = fatigueGainColor;
            }
            magickaBar.ProgressTexture = DaggerfallUI.GetTextureFromImg(magickaBarFilename);
            magickaBarLoss.Color = magickaLossColor;
            magickaBarGain.Color = magickaGainColor;
        }

        public override void Update()
        {
            if (Enabled)
            {
                base.Update();
                PositionIndicators();

                if (DaggerfallUnity.Settings.EnableVitalsIndicators)
                    UpdateAllVitals();
                else
                    SynchronizeImmediately();
            }
        }

        void PositionIndicators()
        {
            // Most of this depends on Scale and LiveEndurance only,
            // so does not change from frame to frame; Is it worth optimizing?
            float barWidth = nativeBarWidth * Scale.x;
            float barHeight = nativeBarHeight * Scale.y;

            Size = new Vector2(barWidth * 5, barHeight);

            healthBar.Position = (CustomHealthBarPosition != null) ? CustomHealthBarPosition.Value : Position + new Vector2(0, 0);
            healthBar.Size =  (CustomHealthBarSize != null) ? CustomHealthBarSize.Value : new Vector2(barWidth, barHeight);

            fatigueBar.Position = (CustomFatigueBarPosition != null) ? CustomFatigueBarPosition.Value : Position + new Vector2(barWidth * 2, 0);
            fatigueBar.Size = (CustomFatigueBarSize != null) ? CustomFatigueBarSize.Value : new Vector2(barWidth, barHeight);

            magickaBar.Position = (CustomMagickaBarPosition != null) ? CustomMagickaBarPosition.Value : Position + new Vector2(barWidth * 4, 0);
            magickaBar.Size = (CustomMagickaBarSize != null) ? CustomMagickaBarSize.Value : new Vector2(barWidth, barHeight);

            if (DaggerfallUnity.Settings.EnableVitalsIndicators)
            {
                healthBarLoss.Position = healthBar.Position;
                healthBarLoss.Size = healthBar.Size;

                fatigueBarLoss.Position = fatigueBar.Position;
                fatigueBarLoss.Size = fatigueBar.Size;

                magickaBarLoss.Position = magickaBar.Position;
                magickaBarLoss.Size = magickaBar.Size;

                healthBarGain.Position = healthBar.Position;
                healthBarGain.Size = healthBar.Size;

                fatigueBarGain.Position = fatigueBar.Position;
                fatigueBarGain.Size = fatigueBar.Size;

                magickaBarGain.Position = magickaBar.Position;
                magickaBarGain.Size = magickaBar.Size;
            }
        }

        private void SynchronizeImmediately()
        {
            // Adjust vitals based on current player state
            healthBar.Amount = playerEntity.CurrentHealth / (float)playerEntity.MaxHealth;
            fatigueBar.Amount = playerEntity.CurrentFatigue / (float)playerEntity.MaxFatigue;
            magickaBar.Amount = playerEntity.CurrentMagicka / (float)playerEntity.MaxMagicka;

            if (DaggerfallUnity.Settings.EnableVitalsIndicators)
            {
                healthBarGain.Amount = healthBar.Amount;
                healthBarLoss.Amount = healthBar.Amount;
                fatigueBarGain.Amount = fatigueBar.Amount;
                fatigueBarLoss.Amount = fatigueBar.Amount;
                magickaBarGain.Amount = magickaBar.Amount;
                magickaBarLoss.Amount = magickaBar.Amount;
            }
        }

        void UpdateAllVitals()
        {
            // these progress bars never smooth-change.
            healthBarGain.Amount = playerEntity.CurrentHealth / (float)playerEntity.MaxHealth;
            fatigueBarGain.Amount = playerEntity.CurrentFatigue / (float)playerEntity.MaxFatigue;
            magickaBarGain.Amount = playerEntity.CurrentMagicka / (float)playerEntity.MaxMagicka;
            healthBarLoss.Cycle();
            fatigueBarLoss.Cycle();
            magickaBarLoss.Cycle();
            healthBar.Cycle();
            fatigueBar.Cycle();
            magickaBar.Cycle();
        }

        private void VitalsDetector_HealthChanged(object sender, System.EventArgs e)
        {
            if (!DaggerfallUnity.Settings.EnableVitalsIndicators)
            {
                return;
            }

            healthBarGain.Amount = playerEntity.CurrentHealth / (float)playerEntity.MaxHealth;

            // if there's any change in health... Smooth update the Loss bar, and
            // decide if should smooth update or instant update the progress bar
            if (vitalsDetector.HealthLost != 0)
            {
                if (vitalsDetector.HealthLost > 0)
                    healthBar.Amount -= vitalsDetector.HealthLostPercent;
                else // assumed gaining health
                    healthBarLoss.Amount += vitalsDetector.HealthGainPercent;

                var target = healthBarGain.Amount;
                healthBar.BeginSmoothChange(target);
                healthBarLoss.BeginSmoothChange(target);
            }
        }

        private void VitalsDetector_FatigueChanged(object sender, System.EventArgs e)
        {
            if (!DaggerfallUnity.Settings.EnableVitalsIndicators)
            {
                return;
            }

            fatigueBar.Amount = playerEntity.CurrentFatigue / (float)playerEntity.MaxFatigue;

            // if there's any change in fatigue...
            if (vitalsDetector.FatigueLost != 0)
            {
                if (vitalsDetector.FatigueLost > 0)
                    fatigueBar.Amount -= vitalsDetector.FatigueLostPercent;
                else // assumed gaining health
                    fatigueBarLoss.Amount += vitalsDetector.FatigueGainPercent;

                var target = fatigueBarGain.Amount;
                fatigueBar.BeginSmoothChange(target);
                fatigueBarLoss.BeginSmoothChange(target);
            }
        }

        private void VitalsDetector_MagickaChanged(object sender, System.EventArgs e)
        {
            if (!DaggerfallUnity.Settings.EnableVitalsIndicators)
            {
                return;
            }

            magickaBarGain.Amount = playerEntity.CurrentMagicka / (float)playerEntity.MaxMagicka;

            // if there's any change in magicka...
            if (vitalsDetector.MagickaLost != 0)
            {
                if (vitalsDetector.MagickaLost > 0)
                    magickaBar.Amount -= vitalsDetector.MagickaLostPercent;
                else // assumed gaining health
                    magickaBarLoss.Amount += vitalsDetector.MagickaGainPercent;

                var target = magickaBarGain.Amount;
                magickaBar.BeginSmoothChange(target);
                magickaBarLoss.BeginSmoothChange(target);
            }
        }

        private void VitalChangeDetector_OnReset()
        {
            SynchronizeImmediately();
        }
    }
}
