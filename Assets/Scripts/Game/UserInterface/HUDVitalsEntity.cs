using DaggerfallWorkshop.Game.Entity;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterface {
    public class HUDVitalsEntity : Panel {

        const string healthBarFilename = "MAIN04I0.IMG";
        const string healthBarAllyFilename = "MAIN03I0.IMG";

        const int screenWidth = 160;
        const int screenHeight = 100;

        const int nativeBarWidth = 32;
        const int nativeBarHeight = 4;
        public const int borderSize = 0;

        HorizontalProgressSmoother healthBar = new HorizontalProgressSmoother();
        HorizontalProgressSmoother healthBarLoss = new HorizontalProgressSmoother();
        HorizontalProgress healthBarGain = new HorizontalProgress();
        HorizontalProgress healthBarBackground = new HorizontalProgress();

        Color healthLossColor = new Color(0.44f, 0, 0);
        Color healthGainColor = new Color(1f, 0.50f, 0.50f);
        Color healthLossAllyColor = new Color(0, 0.22f, 0);
        Color healthGainAllyColor = new Color(0.60f, 1f, 0.60f);
        Color healthBackgoundColor = new Color(0.1f, 0.10f, 0.10f);
        Texture2D allyTexture, enemytexture;

        Vector3 healthBarPivotOffset = new Vector3(0, 0.8f, 0);

        DaggerfallEntity entity;
        MobileUnit mobile;
        Camera camera;

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

        public HUDVitalsEntity(Camera cameraRef) : base()
        {
            camera = cameraRef;
            BackgroundColor = Color.clear;

            LoadAssets();
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

            Enabled = false;
        }

        public override void Update()
        {
            if (Enabled)
            {
                if (mobile == null) {
                    Hide();
                    return;
                }

                base.Update();
                PositionIndicators();

                healthBarLoss.Cycle();
                healthBar.Cycle();
            }
        }

        #region Public Methods

        public void Show(bool allyToPlayer = false)
        {
            if (Enabled) return;
            SetTexture(allyToPlayer);
            SynchronizeImmediately();
            Enabled = true;
        }

        public void Hide(DaggerfallEntity dfEntity = null)
        {
            if (entity != null) {
                entity.OnHealthChange -= UpdateVitals;
                entity.OnDeath -= Hide;
            }

            Enabled = false;
        }

        public void SetOwner(MobileUnit mobile, DaggerfallEntity entity)
        {
            this.entity = entity;
            this.mobile = mobile;
            entity.OnHealthChange += UpdateVitals;
            entity.OnDeath += Hide;
        }

        #endregion

        #region Private Methods

        void LoadAssets()
        {
            allyTexture = DaggerfallUI.GetTextureFromImg(healthBarAllyFilename);
            enemytexture = DaggerfallUI.GetTextureFromImg(healthBarFilename);
        }

        void SetTexture(bool allyToPlayer) {

            healthBar.ProgressTexture = allyToPlayer ? allyTexture : enemytexture;
            healthBarBackground.Color = healthBackgoundColor;
            healthBarLoss.Color = allyToPlayer ? healthLossAllyColor : healthLossColor;
            healthBarGain.Color = allyToPlayer ? healthGainAllyColor : healthGainColor;
        }

        void PositionIndicators()
        {
            var worldPosition = mobile.transform.position + healthBarPivotOffset;
            var screenPoint = camera.WorldToScreenPoint(worldPosition);

            if (screenPoint.y < 0 || screenPoint.z < 0) return;

            var direction = worldPosition - camera.transform.position;
            float distance = direction.magnitude;

            float barWidth = nativeBarWidth * Scale.x;
            float barHeight = nativeBarHeight * Scale.y;

            Size = new Vector2(barWidth * 5, barHeight);

            float barSizeX = Mathf.RoundToInt(Mathf.Clamp(barWidth / (distance * 0.5f), 1, barWidth));
            float barSizeY = Mathf.RoundToInt(Mathf.Clamp(barHeight / (distance * 0.5f), 1, barHeight));

            float screenPosX = screenPoint.x / Screen.width;
            float screenPosY = screenPoint.y / Screen.height;

            float x = screenWidth * screenPosX - (barSizeX * 0.5f);
            float y = screenHeight * (-screenPosY + 0.5f);

            healthBar.Position = (CustomHealthBarPosition != null) ? CustomHealthBarPosition.Value : new Vector2(x, y);
            healthBar.Size =  (CustomHealthBarSize != null) ? CustomHealthBarSize.Value : new Vector2(barSizeX, barSizeY);

            healthBarLoss.Position = healthBar.Position;
            healthBarLoss.Size = healthBar.Size;

            healthBarGain.Position = healthBar.Position;
            healthBarGain.Size = healthBar.Size;

            healthBarBackground.Position = healthBar.Position;
            healthBarBackground.Size = healthBar.Size;
        }

        private void SynchronizeImmediately()
        {
            // Adjust vitals based on current player state
            healthBar.Amount = entity.CurrentHealth / (float)entity.MaxHealth;
            healthBarGain.Amount = healthBar.Amount;
            healthBarLoss.Amount = healthBar.Amount;

            //Stop smoothing
            healthBar.cycleTimer = false;
            healthBarLoss.cycleTimer = false;
        }

        void UpdateVitals(float healthLostValue)
        {
            Health = entity.CurrentHealth / (float)entity.MaxHealth;

            var healthLost = healthLostValue / entity.MaxHealth;

            // Smooth update the Loss bar
            if (healthLost != 0)
            {
                if (healthLost > 0)
                    healthBar.Amount -= healthLost;
                else // assumed gaining health
                    healthBarLoss.Amount += healthLost;

                float target = Health;
                healthBar.BeginSmoothChange(target);
                healthBarLoss.BeginSmoothChange(target);
            }

        }

        #endregion

    }
}