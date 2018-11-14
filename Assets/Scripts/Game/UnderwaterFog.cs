using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    public class UnderwaterFog 
    {
        DaggerfallSky sky;
        Camera mainCamera;
        public Color waterFogColor { get; set; }
        private float fogDensityMin;
        private float fogDensityMax;
        public readonly FogMode originalFog = RenderSettings.fogMode;

        public UnderwaterFog()
        {
            mainCamera = GameManager.Instance.MainCamera;
            sky = GameManager.Instance.SkyRig;
            waterFogColor = new Color(0.25f, 0.55f, 0.79f, 1);
            fogDensityMin = 0f;
            fogDensityMax = 0.23f;
        }

        public void UpdateFog(float waterLevel)
        {
            float fogT;
            float yPos = mainCamera.transform.position.y;

            float adjustedCamYPos = yPos + (50 * MeshReader.GlobalScale) - 0.95f;
            float waterEntryThreshold = (waterLevel * -1 * MeshReader.GlobalScale) + 0.38f;
            float waterExitThreshold = (waterEntryThreshold) - 0.02f;

            float clampedCamYPos = Mathf.Clamp(adjustedCamYPos, waterExitThreshold, waterEntryThreshold);

            fogT = 1 - ((clampedCamYPos - waterExitThreshold) / (waterEntryThreshold - waterExitThreshold));

            RenderSettings.fogColor = waterFogColor;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = Mathf.Lerp(fogDensityMin, fogDensityMax, fogT);
        }

        public void ResetFog()
        { 
            sky.SetSkyFogColor(sky.skyColors);
            RenderSettings.fogMode = originalFog;
        }
    }
}