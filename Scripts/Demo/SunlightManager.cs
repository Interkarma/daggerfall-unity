// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop.Demo
{
    /// <summary>
    /// Changes angle of directional light to simulate sunrise through sunset.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class SunlightManager : MonoBehaviour
    {
        public const float defaultScaleFactor = 1;

        public float Angle = -90f;                          // Sunlight direction throughout day
        [Range(0, 1)]
        public float ScaleFactor = defaultScaleFactor;      // Scale all lights by this amount
        public Light[] OtherLights;                         // Other lights to scale and enable/disable

        float keyLightIntensity;
        float[] otherLightsIntensity;

        DaggerfallUnity dfUnity;

        void Start()
        {
            // Save initial intensity of all lights at start
            // This is the value our daily operates against
            SaveLightIntensity();
        }

        void Update()
        {
            // Do nothing if not ready
            if (!ReadyCheck())
                return;

            // Change to night
            if (dfUnity.WorldTime.IsNight && light.enabled)
            {
                light.enabled = false;
                if (OtherLights != null)
                {
                    for (int i = 0; i < OtherLights.Length; i++)
                        OtherLights[i].enabled = false;
                }
            }

            // Change to day
            if (!dfUnity.WorldTime.IsNight && !light.enabled)
            {
                light.enabled = true;
                if (OtherLights != null)
                {
                    for (int i = 0; i < OtherLights.Length; i++)
                        OtherLights[i].enabled = true;
                }
            }

            // Set sun direction and scale
            if (light.enabled)
            {
                // Get value 0-1 for dawn through dusk
                float dawn = WorldTime.DawnHour * WorldTime.MinutesPerHour;
                float dayRange = WorldTime.DuskHour * WorldTime.MinutesPerHour - dawn;
                float lerp = (dfUnity.WorldTime.MinuteOfDay - dawn) / dayRange;

                // Set angle of rotation based on time of day and user value
                float xrot = 180f * lerp;
                light.transform.rotation = Quaternion.Euler(xrot, Angle, 0);

                // Set light intensity
                float scale;
                if (lerp < 0.5f)
                    scale = lerp * 2f;
                else
                    scale = 1f - ((lerp - 0.5f) * 2f);

                // Adjust for custom scale factor
                scale *= ScaleFactor;

                //float scale = (lerp < 0.5f) ? lerp * 2f :  -lerp * 2f;
                SetLightIntensity(scale);
            }
        }

        #region Private Methods

        private bool ReadyCheck()
        {
            if (!dfUnity)
                dfUnity = DaggerfallUnity.Instance;

            // Must have a light component
            if (!light)
                return false;

            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady)
            {
                DaggerfallUnity.LogMessage("SunlightManager: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            return true;
        }

        private void SaveLightIntensity()
        {
            if (light)
                keyLightIntensity = light.intensity;

            if (OtherLights != null)
            {
                otherLightsIntensity = new float[OtherLights.Length];
                for (int i = 0; i < OtherLights.Length; i++)
                {
                    if (OtherLights[i] == null)
                        continue;
                    otherLightsIntensity[i] = OtherLights[i].intensity;
                }
            }
        }

        void SetLightIntensity(float scale)
        {
            if (light)
                light.intensity = keyLightIntensity * scale;

            if (OtherLights != null)
            {
                for (int i = 0; i < OtherLights.Length; i++)
                {
                    if (OtherLights[i] == null)
                        continue;
                    OtherLights[i].intensity = otherLightsIntensity[i] * scale;
                }
            }
        }

        #endregion
    }
}