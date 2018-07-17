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
        VerticalProgress healthProgress = new VerticalProgress();
        VerticalProgress fatigueProgress = new VerticalProgress();
        VerticalProgress magickaProgress = new VerticalProgress();
        VerticalProgress breathProgress = new VerticalProgress();
        VerticalProgressIndicator healthProgressLoss = new VerticalProgressIndicator();
        VerticalProgressIndicator fatigueProgressLoss = new VerticalProgressIndicator();
        VerticalProgressIndicator magickaProgressLoss = new VerticalProgressIndicator();

        Color healthLossColor = new Color(0, 0.22f, 0);
        Color fatigueLossColor = new Color(0.44f, 0, 0);
        Color magickaLossColor = new Color(0, 0, 0.44f);
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
            LoadAssets();

            BackgroundColor = Color.clear;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Bottom;
            SetMargins(Margins.All, borderSize);
            
            healthProgress.VerticalAlignment = VerticalAlignment.Bottom;
            fatigueProgress.VerticalAlignment = VerticalAlignment.Bottom;
            magickaProgress.VerticalAlignment = VerticalAlignment.Bottom;
            healthProgressLoss.VerticalAlignment = VerticalAlignment.Bottom;
            fatigueProgressLoss.VerticalAlignment = VerticalAlignment.Bottom;
            magickaProgressLoss.VerticalAlignment = VerticalAlignment.Bottom;

            // to make bar appear behind other bars, add it first.
            Components.Add(healthProgressLoss);
            Components.Add(fatigueProgressLoss);
            Components.Add(magickaProgressLoss);
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
                float breathBarHeight = GameManager.Instance.PlayerEntity.Stats.LiveEndurance * Scale.y;

                Size = new Vector2(barWidth * 5, barHeight);
                
                
                if (vitalsDetector.HealthLost != 0)
                    healthProgressLoss.BeginSmoothChange(GameManager.Instance.PlayerEntity.CurrentHealthPercent);
                if (vitalsDetector.FatigueLost != 0)
                    fatigueProgressLoss.BeginSmoothChange(GameManager.Instance.PlayerEntity.CurrentFatigue / GameManager.Instance.PlayerEntity.MaxFatigue);
                if (vitalsDetector.MagickaLost != 0)
                    magickaProgressLoss.BeginSmoothChange(GameManager.Instance.PlayerEntity.CurrentMagicka / GameManager.Instance.PlayerEntity.MaxMagicka);

                healthProgressLoss.Position = new Vector2(0, 0);
                healthProgressLoss.Size = new Vector2(barWidth, barHeight);
                healthProgressLoss.Cycle();

                fatigueProgressLoss.Position = new Vector2(barWidth * 2, 0);
                fatigueProgressLoss.Size = new Vector2(barWidth, barHeight);
                fatigueProgressLoss.Cycle();

                magickaProgressLoss.Position = new Vector2(barWidth * 4, 0);
                magickaProgressLoss.Size = new Vector2(barWidth, barHeight);
                magickaProgressLoss.Cycle();

                healthProgress.Position = new Vector2(0, 0);
                healthProgress.Size = new Vector2(barWidth, barHeight);

                fatigueProgress.Position = new Vector2(barWidth * 2, 0);
                fatigueProgress.Size = new Vector2(barWidth, barHeight);

                magickaProgress.Position = new Vector2(barWidth * 4, 0);
                magickaProgress.Size = new Vector2(barWidth, barHeight);

                breathProgress.Position = new Vector2(306 * Scale.x, (-60 * Scale.y) - breathBarHeight);
                breathProgress.Size = new Vector2(breathBarWidth, breathBarHeight);
            }
        }

        void LoadAssets()
        {
            if (DaggerfallUnity.Settings.SwapHealthAndFatigueColors)
            {
                healthProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(fatigueBarFilename);
                fatigueProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(healthBarFilename);
                healthProgressLoss.Color = fatigueLossColor;
                fatigueProgressLoss.Color = healthLossColor;
            }
            else
            {
                healthProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(healthBarFilename);
                fatigueProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(fatigueBarFilename);
                healthProgressLoss.Color = healthLossColor;
                fatigueProgressLoss.Color = fatigueLossColor;
            }
            magickaProgress.ProgressTexture = DaggerfallUI.GetTextureFromImg(magickaBarFilename);
            magickaProgressLoss.Color = magickaLossColor;
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

            // sync fatigue bar
            int maxFatigue = (pData.stats.LiveStrength + pData.stats.LiveEndurance) * 64;
            fatigueProgressLoss.Amount = pData.currentFatigue / (float)maxFatigue;

            // sync magicka bar
            DFCareer career = pData.careerTemplate;
            int maxMagicka = FormulaHelper.SpellPoints(pData.stats.LiveIntelligence, career.SpellPointMultiplierValue);
            magickaProgressLoss.Amount = pData.currentMagicka / (float)maxMagicka;
        }
    }
}
