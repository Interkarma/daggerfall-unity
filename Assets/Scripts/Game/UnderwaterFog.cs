using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public class UnderwaterFog 
    {
        //DaggerfallSky sky;
        Camera mainCamera;
        public Color waterFogColor { get; set; }
        private float fogDensityMin;
        private float fogDensityMax;

        // store fog values to reset fog after leaving water
        private FogMode originalFogMode;               
        private float originalFogDensity;
        private float originalFogStartDistance;
        private float originalFogEndDistance;
        private Color originalFogColor;

        // used to identify player transition from out of water to entering water
        private float oldFogT = 0.0f;

        public UnderwaterFog()
        {
            mainCamera = GameManager.Instance.MainCamera;
            waterFogColor = new Color(0.25f, 0.55f, 0.79f, 1);
            fogDensityMin = 0f;
            fogDensityMax = 0.23f;
            // get initial (backup) values - will be overwritten (this is just a safety net mechanism so we start out with some values)
            originalFogMode = RenderSettings.fogMode;
            originalFogDensity = RenderSettings.fogDensity;
            originalFogStartDistance = RenderSettings.fogStartDistance;
            originalFogEndDistance = RenderSettings.fogEndDistance;
            originalFogColor = RenderSettings.fogColor;
        }

        public void UpdateFog(float waterLevel)
        {
            float fogT;
            float yPos = mainCamera.transform.position.y;

            float adjustedCamYPos = yPos + (50 * MeshReader.GlobalScale) - 0.95f;
            float waterEntryThreshold = (waterLevel * -1 * MeshReader.GlobalScale) + 0.38f;
            float waterExitThreshold = (waterEntryThreshold) - 0.02f;

            float clampedCamYPos = Mathf.Clamp(adjustedCamYPos, waterExitThreshold, waterEntryThreshold);

            if (waterEntryThreshold - waterExitThreshold != 0.0f)
                fogT = 1 - ((clampedCamYPos - waterExitThreshold) / (waterEntryThreshold - waterExitThreshold));
            else
                fogT = 0.0f;

            // backup fog settings when player is out of water or just entering water (oldFogT is in both cases zero)
            if (oldFogT == 0.0f)
            {
                originalFogMode = RenderSettings.fogMode;
                originalFogDensity = RenderSettings.fogDensity;
                originalFogStartDistance = RenderSettings.fogStartDistance;
                originalFogEndDistance = RenderSettings.fogEndDistance;
                originalFogColor = RenderSettings.fogColor;
            }
            oldFogT = fogT;

            // if player is submerged or entering water apply underwater fog
            if (fogT > 0.0f)
            {
                RenderSettings.fogMode = FogMode.Exponential;
                RenderSettings.fogDensity = Mathf.Lerp(fogDensityMin, fogDensityMax, fogT);
                RenderSettings.fogColor = waterFogColor;
            }
            else // otherwise restore old fog settings
            {
                RenderSettings.fogMode = originalFogMode;
                RenderSettings.fogDensity = originalFogDensity;
                RenderSettings.fogStartDistance = originalFogStartDistance;
                RenderSettings.fogEndDistance = originalFogEndDistance;
                RenderSettings.fogColor = originalFogColor;
            }
        }
    }
}