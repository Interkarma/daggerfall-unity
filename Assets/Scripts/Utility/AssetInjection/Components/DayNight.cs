// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using System;
using UnityEngine;
using DaggerfallWorkshop;

namespace DaggerfallWorkshop.Utility.AssetInjection
{
    /// <summary>
    /// Toggles lights and particle systems, switch emission maps color according to day time.
    /// </summary>
    [HelpURL("http://www.dfworkshop.net/projects/daggerfall-unity/modding/models-flats/#daynight")]
    public class DayNight : MonoBehaviour
    {
        #region Enums

        public enum LightingSelection { SelectedOnly, All };
        public enum EmissionColors { NoColors, DaggerfallColors, CustomColors };

        #endregion

        #region Fields

        [Header("Lighting")]

        [Tooltip("Disable lights and particles at day time.")]
        public LightingSelection toggleLighting;

        [Tooltip("This particle system will emit only at night.")]
        public ParticleSystem[] particles;

        [Tooltip("This light will be enabled only at night.")]
        public Light[] lights;

        [Header("Windows Emission Map")]

        [Tooltip("Apply colors to the emission map according to day time.")]
        public EmissionColors emissionColors;

        [Tooltip("Index of emissive material for the MeshRender.")]
        public uint materialIndex;

        [ColorUsage(false, true)]
        public Color dayColor;

        [ColorUsage(false, true)]
        public Color nightColor;

        DaggerfallUnity dfUnity;
        Material emissiveMaterial;
        bool lastFlag;

        #endregion

        #region Unity

        void OnDisable()
        {
            lastFlag = !lastFlag;
        }

        void Update()
        {
            if (ReadyCheck() && lastFlag != dfUnity.WorldTime.Now.IsCityLightsOn)
            {
                Set();
                lastFlag = dfUnity.WorldTime.Now.IsCityLightsOn;
            }
        }

        #endregion

        #region Methods
                
        /// <summary>
        /// Update components.
        /// </summary>
        private void Set()
        {
            // Are lights on?
            bool lightsOn = dfUnity.WorldTime.Now.IsCityLightsOn;

            if (dfUnity.Option_AutomateCityLights)
            {
                // Particles
                foreach (var particleSystem in particles)
                {
                    if (particleSystem)
                    {
                        if (lightsOn)
                            particleSystem.Play();
                        else
                            particleSystem.Stop();
                    }
                }

                // Lights
                foreach (var light in lights)
                {
                    if (light)
                        light.enabled = lightsOn;
                }
            }

            if (dfUnity.Option_AutomateCityWindows && emissiveMaterial)
            {
                // Emission
                switch (emissionColors)
                {
                    case EmissionColors.DaggerfallColors:
                        WindowStyle style = lightsOn ? WindowStyle.Night : WindowStyle.Day;
                        dfUnity.MaterialReader.ChangeWindowEmissionColor(emissiveMaterial, style);
                        break;

                    case EmissionColors.CustomColors:
                        emissiveMaterial.SetColor(Uniforms.EmissionColor, lightsOn ? nightColor : dayColor);
                        break;
                }
            }
        }

        /// <summary>
        /// Get material with emission map at specified index.
        /// </summary>
        private void InitEmissiveMaterial()
        {
            MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
            try
            {
                // Get material
                emissiveMaterial = renderer.materials[materialIndex];

                // Force emission
                if (!emissiveMaterial.IsKeywordEnabled(KeyWords.Emission))
                    emissiveMaterial.EnableKeyword(KeyWords.Emission);
            }
            catch (Exception e)
            {
                emissiveMaterial = null;
                Debug.LogErrorFormat("Failed to get emissive material from {0}: {1}", name, e.Message);
            }
        }

        private bool ReadyCheck()
        {
            // Ensure we have a DaggerfallUnity reference
            if (dfUnity == null)
            {
                dfUnity = DaggerfallUnity.Instance;

                // Stop component if not necessary
                if (!dfUnity.Option_AutomateCityLights && !dfUnity.Option_AutomateCityWindows)
                    Destroy(this);

                // Get lights
                if (toggleLighting == LightingSelection.All)
                {
                    particles = GetComponentsInChildren<ParticleSystem>();
                    lights = GetComponentsInChildren<Light>();
                }

                // Get emissive material
                if (emissionColors != EmissionColors.NoColors)
                    InitEmissiveMaterial();

                // Force first update to set
                lastFlag = !dfUnity.WorldTime.Now.IsCityLightsOn;
            }

            // Do nothing if DaggerfallUnity not ready
            if (!dfUnity.IsReady)
            {
                DaggerfallUnity.LogMessage("DayNight: DaggerfallUnity component is not ready. Have you set your Arena2 path?");
                return false;
            }

            return true;
        }

        #endregion
    }
}