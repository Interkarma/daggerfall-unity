using DaggerfallWorkshop.Game.Entity;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterface {
    public class HUDVitalsEntity : Panel {

        const string healthBarFilename = "MAIN04I0.IMG";

        const int screenWidth = 160;
        const int screenHeight = 100;

        const int nativeBarWidth = 32;
        const int nativeBarHeight = 4;
        public const int borderSize = 0;

        HorizontalProgressSmoother healthBar = new HorizontalProgressSmoother();
        HorizontalProgressSmoother healthBarLoss = new HorizontalProgressSmoother();
        HorizontalProgress healthBarGain = new HorizontalProgress();
        HorizontalProgress healthBarBackground = new HorizontalProgress();
        DaggerfallEntity entity;

        Color healthLossColor = new Color(0.44f, 0, 0);
        Color healthGainColor = new Color(1f, 0.50f, 0.50f);
        Color healthBackgoundColor = new Color(0.1f, 0.10f, 0.10f);

        Camera camera;
        float lastKnownHealthValue = 1;

        public Vector2? CustomHealthBarPosition { get; set; }
        public Vector2? CustomHealthBarSize { get; set; }

        /// <summary>
        /// Gets or sets current health as value between 0 and 1.
        /// </summary>
        public float Health
        {
            get { return healthBarGain.Amount; }
            set { healthBarGain.Amount = value; }
        }

        public HUDVitalsEntity() : base()
        {
            LoadAssets();

            camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            entity = GameManager.Instance.PlayerEntity;
            BackgroundColor = Color.clear;

            SetMargins(Margins.All, borderSize);

            AutoSize = AutoSizeModes.ScaleToFit;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Middle;

            healthBarBackground.VerticalAlignment = VerticalAlignment.None;
            healthBarLoss.VerticalAlignment = VerticalAlignment.None;
            healthBarGain.VerticalAlignment = VerticalAlignment.None;
            healthBar.VerticalAlignment = VerticalAlignment.None;

            healthBarBackground.HorizontalAlignment = HorizontalAlignment.None;
            healthBarLoss.HorizontalAlignment = HorizontalAlignment.None;
            healthBarGain.HorizontalAlignment = HorizontalAlignment.None;
            healthBar.HorizontalAlignment = HorizontalAlignment.None;

            // to make bar appear behind other bars, add it first.
            Components.Add(healthBarBackground);
            Components.Add(healthBarLoss);
            Components.Add(healthBarGain);
            Components.Add(healthBar);

        }

        void LoadAssets()
        {
            healthBar.ProgressTexture = DaggerfallUI.GetTextureFromImg(healthBarFilename);
            healthBarBackground.Color = healthBackgoundColor;
            healthBarLoss.Color = healthLossColor;
            healthBarGain.Color = healthGainColor;
        }


        public override void Update()
        {
            if (Enabled)
            {
                base.Update();
                PositionIndicators();

                UpdateAllVitals();
            }
        }

        void PositionIndicators()
        {
            float barWidth = nativeBarWidth * Scale.x;
            float barHeight = nativeBarHeight * Scale.y;

            Size = new Vector2(barWidth * 5, barHeight);

            float x = screenWidth * 0.5f - (barWidth / 2);//Screen.width * 0.25f;//Random.Range(-100, 100);//
            float y = screenHeight * 0f;//Screen.height * 0.25f;//Random.Range(-100, 100);//

            healthBar.Position = (CustomHealthBarPosition != null) ? CustomHealthBarPosition.Value : new Vector2(x, y);
            healthBar.Size =  (CustomHealthBarSize != null) ? CustomHealthBarSize.Value : new Vector2(barWidth, barHeight);

            healthBarLoss.Position = healthBar.Position;
            healthBarLoss.Size = healthBar.Size;

            healthBarGain.Position = healthBar.Position;
            healthBarGain.Size = healthBar.Size;

            healthBarBackground.Position = healthBar.Position;
            healthBarBackground.Size = healthBar.Size;
        }

        void UpdateAllVitals()
        {
            healthBarGain.Amount = entity.CurrentHealth / (float)entity.MaxHealth;

            var healthLost = lastKnownHealthValue - healthBarGain.Amount;

            float target;
            // Smooth update the Loss bar, and
            if (healthLost != 0)
            {
                if (healthLost > 0)
                    healthBar.Amount -= healthLost;
                else // assumed gaining health
                    healthBarLoss.Amount += healthLost;

                target = healthBarGain.Amount;
                healthBar.BeginSmoothChange(target);
                healthBarLoss.BeginSmoothChange(target);
            }
            lastKnownHealthValue = healthBarGain.Amount;

            healthBarLoss.Cycle();
            healthBar.Cycle();
        }
    }
}