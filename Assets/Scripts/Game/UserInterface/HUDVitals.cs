// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
        VerticalProgressSmoother healthProgress = new VerticalProgressSmoother();
        VerticalProgressSmoother fatigueProgress = new VerticalProgressSmoother();
        VerticalProgressSmoother magickaProgress = new VerticalProgressSmoother();
        VerticalProgress breathProgress = new VerticalProgress();
        VerticalProgressSmoother healthProgressLoss = new VerticalProgressSmoother();
        VerticalProgressSmoother fatigueProgressLoss = new VerticalProgressSmoother();
        VerticalProgressSmoother magickaProgressLoss = new VerticalProgressSmoother();
        VerticalProgress healthProgressGain = new VerticalProgress();
        VerticalProgress fatigueProgressGain = new VerticalProgress();
        VerticalProgress magickaProgressGain = new VerticalProgress();
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
            get { return healthProgressGain.Amount; }
            set { healthProgressGain.Amount = value; }
        }

        /// <summary>
        /// Gets or sets current fatigue as value between 0 and 1.
        /// </summary>
        public float Fatigue
        {
            get { return fatigueProgressGain.Amount; }
            set { fatigueProgressGain.Amount = value; }
        }

        /// <summary>
        /// Gets or sets current magicka as value between 0 and 1.
        /// </summary>
        public float Magicka
        {
            get { return magickaProgressGain.Amount; }
            set { magickaProgressGain.Amount = value; }
        }

        /// <summary>
        /// Gets or sets current breath as value between 0 and 1.
        /// </summary>
        public float Breath
        {
            get { return breathProgress.Amount; }
            set { SetRemainingBreath(value); }
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
            
            healthProgress.VerticalAlignment = VerticalAlignment.Bottom;
            fatigueProgress.VerticalAlignment = VerticalAlignment.Bottom;
            magickaProgress.VerticalAlignment = VerticalAlignment.Bottom;

            if (DaggerfallUnity.Settings.EnableVitalsIndicators)
            {
                healthProgressLoss.VerticalAlignment = VerticalAlignment.Bottom;
                fatigueProgressLoss.VerticalAlignment = VerticalAlignment.Bottom;
                magickaProgressLoss.VerticalAlignment = VerticalAlignment.Bottom;
                healthProgressGain.VerticalAlignment = VerticalAlignment.Bottom;
                fatigueProgressGain.VerticalAlignment = VerticalAlignment.Bottom;
                magickaProgressGain.VerticalAlignment = VerticalAlignment.Bottom;

                // to make bar appear behind other bars, add it first.
                Components.Add(healthProgressGain);
                Components.Add(fatigueProgressGain);
                Components.Add(magickaProgressGain);
                Components.Add(healthProgressLoss);
                Components.Add(fatigueProgressLoss);
                Components.Add(magickaProgressLoss);
            }

            Components.Add(healthProgress);
            Components.Add(fatigueProgress);
            Components.Add(magickaProgress);
            Components.Add(breathProgress);

            SaveLoadManager.OnStartLoad += SaveLoadManager_OnStartLoad;
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

                healthProgress.Position = new Vector2(0, 0);
                healthProgress.Size = new Vector2(barWidth, barHeight);

                fatigueProgress.Position = new Vector2(barWidth * 2, 0);
                fatigueProgress.Size = new Vector2(barWidth, barHeight);

                magickaProgress.Position = new Vector2(barWidth * 4, 0);
                magickaProgress.Size = new Vector2(barWidth, barHeight);

                breathProgress.Position = new Vector2(306 * Scale.x, (-60 * Scale.y) - breathBarHeight);
                breathProgress.Size = new Vector2(breathBarWidth, breathBarHeight);

                if (DaggerfallUnity.Settings.EnableVitalsIndicators)
                {
                    UpdateIndicators();
                }
                else
                {
                    // Adjust vitals based on current player state
                    healthProgress.Amount = playerEntity.CurrentHealth / (float)playerEntity.MaxHealth;
                    fatigueProgress.Amount = playerEntity.CurrentFatigue / (float)playerEntity.MaxFatigue;
                    magickaProgress.Amount = playerEntity.CurrentMagicka / (float)playerEntity.MaxMagicka;
                }
                breathProgress.Amount = playerEntity.CurrentBreath / (float)playerEntity.MaxBreath;
            }
        }

        void UpdateIndicators()
        {
            // these progress bars never smooth-change.
            healthProgressGain.Amount = playerEntity.CurrentHealth / (float)playerEntity.MaxHealth;
            fatigueProgressGain.Amount = playerEntity.CurrentFatigue / (float)playerEntity.MaxFatigue;
            magickaProgressGain.Amount = playerEntity.CurrentMagicka / (float)playerEntity.MaxMagicka;

            float target;
            // if there's any change in health... Smooth update the Loss bar, and
            // decide if should smooth update or instant update the progress bar
            if (vitalsDetector.HealthLost != 0)
            {
                target = healthProgressGain.Amount;
                healthProgressLoss.BeginSmoothChange(target);
                if (vitalsDetector.HealthLost > 0)
                    healthProgress.Amount = target;
                else if (vitalsDetector.HealthGain > 0)
                    healthProgress.BeginSmoothChange(target);
            }
            // if there's any change in fatigue...
            if (vitalsDetector.FatigueLost != 0)
            {
                target = fatigueProgressGain.Amount;
                fatigueProgressLoss.BeginSmoothChange(target);
                if (vitalsDetector.FatigueLost > 0)
                    fatigueProgress.Amount = target;
                else if (vitalsDetector.FatigueGain > 0)
                    fatigueProgress.BeginSmoothChange(target);
            }
            // if there's any change in magicka...
            if (vitalsDetector.MagickaLost != 0)
            {
                target = magickaProgressGain.Amount;
                magickaProgressLoss.BeginSmoothChange(target);
                if (vitalsDetector.MagickaLost > 0)
                    magickaProgress.Amount = target;
                else if (vitalsDetector.MagickaGain > 0)
                    magickaProgress.BeginSmoothChange(target);
            }

            healthProgressLoss.Cycle();
            fatigueProgressLoss.Cycle();
            magickaProgressLoss.Cycle();
            healthProgress.Cycle();
            fatigueProgress.Cycle();
            magickaProgress.Cycle();

            healthProgressLoss.Position = healthProgress.Position;
            healthProgressLoss.Size = healthProgress.Size;

            fatigueProgressLoss.Position = fatigueProgress.Position;
            fatigueProgressLoss.Size = fatigueProgress.Size;

            magickaProgressLoss.Position = magickaProgress.Position;
            magickaProgressLoss.Size = magickaProgress.Size;

            healthProgressGain.Position = healthProgress.Position;
            healthProgressGain.Size = healthProgress.Size;

            fatigueProgressGain.Position = fatigueProgress.Position;
            fatigueProgressGain.Size = fatigueProgress.Size;

            magickaProgressGain.Position = magickaProgress.Position;
            magickaProgressGain.Size = magickaProgress.Size;

        }
        void LoadAssets()
        {
            if (DaggerfallUnity.Settings.SwapHealthAndFatigueColors)
            {
                healthProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(fatigueBarFilename);
                fatigueProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(healthBarFilename);
                healthProgressLoss.Color = fatigueLossColor;
                fatigueProgressLoss.Color = healthLossColor;
                healthProgressGain.Color = fatigueGainColor;
                fatigueProgressGain.Color = healthGainColor;
            }
            else
            {
                healthProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(healthBarFilename);
                fatigueProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(fatigueBarFilename);
                healthProgressLoss.Color = healthLossColor;
                fatigueProgressLoss.Color = fatigueLossColor;
                healthProgressGain.Color = healthGainColor;
                fatigueProgressGain.Color = fatigueGainColor;
            }
            magickaProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(magickaBarFilename);
            magickaProgressLoss.Color = magickaLossColor;
            magickaProgressGain.Color = magickaGainColor;
        }

        void SetRemainingBreath(float amount)
        {
            breathProgress.Amount = amount;
            int threshold = ((GameManager.Instance.PlayerEntity.Stats.LiveEndurance) >> 3) + 4;
            if (threshold > GameManager.Instance.PlayerEntity.CurrentBreath)
                breathProgress.Color = new Color32(148, 12, 0, 255);
            else
                breathProgress.Color = new Color32(247, 239, 41, 255);
        }

        private void SaveLoadManager_OnStartLoad(SaveData_v1 saveData)
        {
            PlayerEntityData_v1 pData = saveData.playerData.playerEntity;

            // sync health bar
            healthProgressLoss.Amount = pData.currentHealth / (float)pData.maxHealth;
            healthProgress.Amount = healthProgressLoss.Amount;
            // sync fatigue bar
            int maxFatigue = (pData.stats.LiveStrength + pData.stats.LiveEndurance) * 64;
            fatigueProgressLoss.Amount = pData.currentFatigue / (float)maxFatigue;
            fatigueProgress.Amount = fatigueProgressLoss.Amount;

            // sync magicka bar
            DFCareer career = pData.careerTemplate;
            int maxMagicka = FormulaHelper.SpellPoints(pData.stats.LiveIntelligence, career.SpellPointMultiplierValue);
            magickaProgressLoss.Amount = pData.currentMagicka / (float)maxMagicka;
            magickaProgress.Amount = magickaProgressLoss.Amount;
        }
    }
}
