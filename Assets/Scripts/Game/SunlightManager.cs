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

using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game
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
        public Light IndirectLight;                         // Point light that follows player to simulate indirect lighting
        public GameObject LocalPlayer;                      // Player for indirect light positioning
        public Light[] OtherLights;                         // Other lights to scale and enable/disable

        Light myLight;
        float keyLightIntensity;
        float indirectLightIntensity;
        float[] otherLightsIntensity;
        float daylightScale;

        DaggerfallUnity dfUnity;

        /// <summary>
        /// Gets daylight light scale which peaks at midday and wanes at dusk at dawn.
        /// </summary>
        public float DaylightScale
        {
            get { return daylightScale; }
        }

        void Start()
        {
            // Save reference to light
            myLight = GetComponent<Light>();

            // Save initial intensity of all lights at start
            // This is the value our daily operates against
            SaveLightIntensity();
        }

        void Update()
        {
            // Do nothing if not ready
            if (!ReadyCheck())
                return;

            // Position indirect light
            if (IndirectLight != null && LocalPlayer != null)
            {
                IndirectLight.transform.position = LocalPlayer.transform.position;
            }

            // Change to night
            if (dfUnity.WorldTime.Now.IsNight && myLight.enabled)
            {
                myLight.enabled = false;
                if (OtherLights != null)
                {
                    for (int i = 0; i < OtherLights.Length; i++)
                        OtherLights[i].enabled = false;
                }
                if (IndirectLight != null)
                {
                    IndirectLight.enabled = false;
                }
            }

            // Change to day
            if (!dfUnity.WorldTime.Now.IsNight && !myLight.enabled)
            {
                myLight.enabled = true;
                if (OtherLights != null)
                {
                    for (int i = 0; i < OtherLights.Length; i++)
                        OtherLights[i].enabled = true;
                }
                if (IndirectLight != null)
                {
                    IndirectLight.enabled = true;
                }
            }

            // Get value 0-1 for dawn through dusk
            float dawn = DaggerfallDateTime.DawnHour * DaggerfallDateTime.MinutesPerHour;
            float dayRange = DaggerfallDateTime.DuskHour * DaggerfallDateTime.MinutesPerHour - dawn;
            float lerp = (dfUnity.WorldTime.Now.MinuteOfDay - dawn) / dayRange;

            // Set angle of rotation based on time of day and user value
            float xrot = 180f * lerp;
            myLight.transform.rotation = Quaternion.Euler(xrot, Angle, 0);

            // Set light intensity
            if (lerp < 0.5f)
                daylightScale = lerp * 2f;
            else
                daylightScale = 1f - ((lerp - 0.5f) * 2f);

            // Set sun direction and scale
            if (myLight.enabled)
            {
                // Adjust for custom scale factor
                daylightScale *= ScaleFactor;

                SetLightIntensity(daylightScale);
            }
        }

        #region Private Methods

        private bool ReadyCheck()
        {
            if (!dfUnity)
                dfUnity = DaggerfallUnity.Instance;

            // Must have a light component
            if (!myLight)
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
            if (myLight)
                keyLightIntensity = myLight.intensity;

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
            if (IndirectLight != null)
            {
                indirectLightIntensity = IndirectLight.intensity;
            }
        }

        void SetLightIntensity(float scale)
        {
            if (myLight)
                myLight.intensity = keyLightIntensity * scale;

            if (OtherLights != null)
            {
                for (int i = 0; i < OtherLights.Length; i++)
                {
                    if (OtherLights[i] == null)
                        continue;
                    OtherLights[i].intensity = otherLightsIntensity[i] * scale;
                }
            }
            if (IndirectLight != null)
            {
                IndirectLight.intensity = indirectLightIntensity * scale;
            }
        }

        #endregion
    }
}