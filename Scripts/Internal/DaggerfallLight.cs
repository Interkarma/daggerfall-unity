// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System;
using System.Collections;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Attached to point lights to create animated effect.
    /// </summary>
    public class DaggerfallLight : MonoBehaviour
    {
        public DaggerfallBillboard ParentBillboard;

        public bool Animate = false;

        DaggerfallUnity dfUnity;
        bool lastCityLightsFlag;

        float Variance = 1.0f;              // Maximum amount radius can vary per cycle
        float Speed = 0.4f;                 // Speed radius will shrink or grow towards varied radius
        float FramesPerSecond = 14f;        // Number of times per second animation will tick

        float startRange;
        float targetRange;
        bool stepping;
        bool restartAnims = true;
        Light myLight;

        void Start()
        {
            myLight = GetComponent<Light>();
        }

        void OnDisable()
        {
            restartAnims = true;
        }

        void Update()
        {
            // Restart animation coroutine if not running
            if (restartAnims)
            {
                if (myLight != null && Animate)
                    StartCoroutine(AnimateLight());
                restartAnims = false;
            }

            // Do nothing if not ready
            if (!ReadyCheck())
                return;

            // Handle automated light enable/disable
            if (dfUnity.Option_AutomateCityLights && myLight)
            {
                // Only change if day/night flag changes
                if (lastCityLightsFlag != dfUnity.WorldTime.Now.IsCityLightsOn)
                {
                    // Set light
                    myLight.enabled = dfUnity.WorldTime.Now.IsCityLightsOn;
                    lastCityLightsFlag = dfUnity.WorldTime.Now.IsCityLightsOn;
                }
            }
        }

        #region Private Methods

        IEnumerator AnimateLight()
        {
            startRange = myLight.range;

            while (Animate)
            {
                if (stepping)
                {
                    if (targetRange <= myLight.range)
                    {
                        myLight.range -= Speed;
                        if (myLight.range <= targetRange)
                            stepping = false;
                    }
                    else
                    {
                        myLight.range += Speed;
                        if (myLight.range >= targetRange)
                            stepping = false;
                    }
                }
                else
                {
                    // Start a new cycle
                    targetRange = UnityEngine.Random.Range(startRange - Variance, startRange);
                    stepping = true;
                }

                yield return new WaitForSeconds(1f / FramesPerSecond);
            }

            myLight.range = startRange;
        }

        private bool ReadyCheck()
        {
            // Ensure we have a DaggerfallUnity reference
            if (dfUnity == null)
            {
                dfUnity = DaggerfallUnity.Instance;

                // Force first update to set lights
                lastCityLightsFlag = !dfUnity.WorldTime.Now.IsCityLightsOn;
            }

            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady)
            {
                DaggerfallUnity.LogMessage("DaggerfallLight: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            // Get billboard component
            if (ParentBillboard == null)
                return false;

            // Must have a light component added
            if (!myLight)
                return false;

            return true;
        }

        #endregion
    }
}
