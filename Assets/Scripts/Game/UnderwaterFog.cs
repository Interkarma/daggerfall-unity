using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

namespace DaggerfallWorkshop.Game
{
    public class UnderwaterFog 
    {
        GlobalFog globalFog;
        DaggerfallSky sky;
        Camera mainCamera;
        public Color waterFogColor { get; set; }
        private float fogDensityMin;
        private float fogDensityMax;

        // store fog values to reset fog after leaving water
        //private bool fogSettingsChanged = false;
        private FogMode originalFogMode;               
        private float originalFogDensity;
        private float originalFogStartDistance;
        private float originalFogEndDistance;

        // used to identify player transition from out of water to entering water
        private float oldFogT = 0.0f;

        //public bool HaveFogSettingsChanged()
        //{
        //    //if (originalFogMode != RenderSettings.fogMode || originalFogDensity != RenderSettings.fogDensity || originalFogDensity != RenderSettings.fogStartDistance || originalFogDensity != RenderSettings.fogEndDistance)
        //    if (fogSettingsChanged == true)
        //        return true;
        //    else
        //        return false;
        //}

        public UnderwaterFog()
        {
            mainCamera = GameManager.Instance.MainCamera;
            globalFog = mainCamera.GetComponent<GlobalFog>();
            globalFog.enabled = true;
            sky = GameManager.Instance.SkyRig;
            waterFogColor = new Color(0.25f, 0.55f, 0.79f, 1);
            fogDensityMin = 0f;
            fogDensityMax = 0.23f;
            originalFogMode = RenderSettings.fogMode;
            originalFogDensity = RenderSettings.fogDensity;
            originalFogStartDistance = RenderSettings.fogStartDistance;
            originalFogEndDistance = RenderSettings.fogEndDistance;
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

            //// on player transition from out of water to entering water
            //// (this is important: only backup fog values from before entering water
            //// (otherwise water fog values will be backed up unintentionally))
            //if (oldFogT == 0.0f && fogT > 0.0f)
            //{
            //    fogSettingsChanged = true;
            //    originalFogMode = RenderSettings.fogMode;
            //    originalFogDensity = RenderSettings.fogDensity;
            //    originalFogStartDistance = RenderSettings.fogStartDistance;
            //    originalFogEndDistance = RenderSettings.fogEndDistance;
            //}
            //oldFogT = fogT;

            if (oldFogT == 0.0f)
            {
                //fogSettingsChanged = true;
                originalFogMode = RenderSettings.fogMode;
                originalFogDensity = RenderSettings.fogDensity;
                originalFogStartDistance = RenderSettings.fogStartDistance;
                originalFogEndDistance = RenderSettings.fogEndDistance;
            }
            oldFogT = fogT;

            //RenderSettings.fogMode = FogMode.Exponential;
            //RenderSettings.fogDensity = Mathf.Lerp(fogDensityMin, fogDensityMax, fogT);
            //RenderSettings.fogColor = waterFogColor;

            // if player is entering water or submerged
            if (fogT > 0.0f)
            {
                RenderSettings.fogMode = FogMode.Exponential;
                RenderSettings.fogDensity = Mathf.Lerp(fogDensityMin, fogDensityMax, fogT);
                RenderSettings.fogColor = waterFogColor;
            }
            else
            {
                RenderSettings.fogMode = originalFogMode;
                RenderSettings.fogDensity = originalFogDensity;
                RenderSettings.fogStartDistance = originalFogStartDistance;
                RenderSettings.fogEndDistance = originalFogEndDistance;
            }
        }

        //public void ResetFog()
        //{ 
        //    sky.SetSkyFogColor(sky.skyColors);
        //    if (fogSettingsChanged == true)
        //    {
        //        RenderSettings.fogMode = originalFogMode;
        //        RenderSettings.fogDensity = originalFogDensity;
        //        RenderSettings.fogStartDistance = originalFogStartDistance;
        //        RenderSettings.fogEndDistance = originalFogEndDistance;
        //        fogSettingsChanged = false;
        //    }
        //}
    }
}