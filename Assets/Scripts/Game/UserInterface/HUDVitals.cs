// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using DaggerfallConnect;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallWorkshop.Game.Serialization;
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
        const int nativeBreathBarWidth = 6;
        const int nativeBarHeight = 32;
        public const int borderSize = 10;

        VitalsChangeDetector vitalsDetector = GameManager.Instance.VitalsChangeDetector;
        VerticalProgressSmoother healthBar = new VerticalProgressSmoother();
        VerticalProgressSmoother fatigueBar = new VerticalProgressSmoother();
        VerticalProgressSmoother magickaBar = new VerticalProgressSmoother();
        VerticalProgress breathBar = new VerticalProgress();
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

        /// <summary>
        /// Gets or sets current breath as value between 0 and 1.
        /// </summary>
        public float Breath
        {
            get { return breathBar.Amount; }
            set { breathBar.Amount = value;
                  SetRemainingBreathColor(value); }
        }

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
            Components.Add(breathBar);

            VitalsChangeDetector.OnReset += VitalChangeDetector_OnReset;
        }

        public override void Update()
        {
            if (Enabled)
            {
                base.Update();

                float barWidth = nativeBarWidth * Scale.x;
                float breathBarWidth = nativeBreathBarWidth * Scale.x;
                float barHeight = nativeBarHeight * Scale.y;
                float breathBarHeight = playerEntity.Stats.LiveEndurance * Scale.y;

                Size = new Vector2(barWidth * 5, barHeight);

                healthBar.Position = new Vector2(0, 0);
                healthBar.Size = new Vector2(barWidth, barHeight);

                fatigueBar.Position = new Vector2(barWidth * 2, 0);
                fatigueBar.Size = new Vector2(barWidth, barHeight);

                magickaBar.Position = new Vector2(barWidth * 4, 0);
                magickaBar.Size = new Vector2(barWidth, barHeight);

                breathBar.Position = new Vector2(306 * Scale.x, (-60 * Scale.y) - breathBarHeight);
                breathBar.Size = new Vector2(breathBarWidth, breathBarHeight);

                if (DaggerfallUnity.Settings.EnableVitalsIndicators)
                {
                    UpdateAllVitals();
                    PositionIndicators();
                }
                else
                {
                    // Adjust vitals based on current player state
                    healthBar.Amount = playerEntity.CurrentHealth / (float)playerEntity.MaxHealth;
                    fatigueBar.Amount = playerEntity.CurrentFatigue / (float)playerEntity.MaxFatigue;
                    magickaBar.Amount = playerEntity.CurrentMagicka / (float)playerEntity.MaxMagicka;
                }
                breathBar.Amount = playerEntity.CurrentBreath / (float)playerEntity.MaxBreath;
                SetRemainingBreathColor(breathBar.Amount);
            }
        }

        void UpdateAllVitals()
        {
            // these progress bars never smooth-change.
            healthBarGain.Amount = playerEntity.CurrentHealth / (float)playerEntity.MaxHealth;
            fatigueBarGain.Amount = playerEntity.CurrentFatigue / (float)playerEntity.MaxFatigue;
            magickaBarGain.Amount = playerEntity.CurrentMagicka / (float)playerEntity.MaxMagicka;

            float target;
            // if there's any change in health... Smooth update the Loss bar, and
            // decide if should smooth update or instant update the progress bar
            if (vitalsDetector.HealthLost != 0)
            {
                if (vitalsDetector.HealthLost > 0)
                    healthBar.Amount -= vitalsDetector.HealthLostPercent;              
                else // assumed gaining health
                    healthBarLoss.Amount += vitalsDetector.HealthGainPercent;

                target = healthBarGain.Amount;
                healthBar.BeginSmoothChange(target);
                healthBarLoss.BeginSmoothChange(target);
            }
            // if there's any change in fatigue...
            if (vitalsDetector.FatigueLost != 0)
            {
                if (vitalsDetector.FatigueLost > 0)
                    fatigueBar.Amount -= vitalsDetector.FatigueLostPercent;
                else // assumed gaining health
                    fatigueBarLoss.Amount += vitalsDetector.FatigueGainPercent;

                target = fatigueBarGain.Amount;
                fatigueBar.BeginSmoothChange(target);
                fatigueBarLoss.BeginSmoothChange(target);
            }
            // if there's any change in magicka...
            if (vitalsDetector.MagickaLost != 0)
            {
                if (vitalsDetector.MagickaLost > 0)
                    magickaBar.Amount -= vitalsDetector.MagickaLostPercent;
                else // assumed gaining health
                    magickaBarLoss.Amount += vitalsDetector.MagickaGainPercent;

                target = magickaBarGain.Amount;
                magickaBar.BeginSmoothChange(target);
                magickaBarLoss.BeginSmoothChange(target);
            }

            healthBarLoss.Cycle();
            fatigueBarLoss.Cycle();
            magickaBarLoss.Cycle();
            healthBar.Cycle();
            fatigueBar.Cycle();
            magickaBar.Cycle();
        }

        void PositionIndicators()
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

        void SetRemainingBreathColor(float amount)
        {
            int threshold = ((GameManager.Instance.PlayerEntity.Stats.LiveEndurance) >> 3) + 4;
            if (threshold > GameManager.Instance.PlayerEntity.CurrentBreath)
                breathBar.Color = new Color32(148, 12, 0, 255);
            else
                breathBar.Color = new Color32(247, 239, 41, 255);
        }

        private void SynchronizeImmediately()
        {
            // sync health bar
            healthBarLoss.Amount = playerEntity.CurrentHealth / (float)playerEntity.MaxHealth;
            healthBar.Amount = healthBarLoss.Amount;

            // sync fatigue bar
            fatigueBarLoss.Amount = playerEntity.CurrentFatigue / (float)playerEntity.MaxFatigue;
            fatigueBar.Amount = fatigueBarLoss.Amount;

            // sync magicka bar
            magickaBarLoss.Amount = playerEntity.CurrentMagicka / (float)playerEntity.MaxMagicka;
            magickaBar.Amount = magickaBarLoss.Amount;
        }

        private void VitalChangeDetector_OnReset()
        {
            SynchronizeImmediately();
        }
    }
}
