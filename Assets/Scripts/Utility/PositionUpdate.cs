// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: LypL
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections;

///Attach to parent gameobject that isn't already updated by floating origin.
///Will update the transform position + any particles, unless bools are set to false.
///
namespace DaggerfallWorkshop.Utility
{
    public class PositionUpdate : MonoBehaviour
    {
        
        public bool updateTransform = true;
        public bool updateParticles = false;
        

        /// <summary>
        /// Handles Position Update events from Floating Origin.
        /// </summary>
        /// <param name="offset"></param>
        void UpdatePosition(Vector3 offset)
        {

            if (updateTransform)
                transform.position += offset;

            if (updateParticles && gameObject.activeSelf)
                UpdateParticles(offset);
        }

        /// <summary>
        /// Update world position of particle effects with offset.
        /// </summary>
        /// <param name="offset"></param>
        /// 
        void UpdateParticles(Vector3 offset)
        {
            ParticleSystem[] particleSystems;
            particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particleSystems.Length; i++)
            {
                ParticleSystem ps = particleSystems[i];
                
                if (ps == null)
                    continue;
                else if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Local)
                    continue;

                ParticleSystem.Particle[] particles;
                particles = new ParticleSystem.Particle[ps.particleCount];
                int n = ps.GetParticles(particles);

                for (int j = 0; j < n; j++)
                {
                    particles[j].position += offset;
                }
                ps.SetParticles(particles, n);
            }
        }


        /// <summary>
        /// Re-subscribe to events on enable.
        /// </summary>
        void OnEnable()
        {
            FloatingOrigin.OnPositionUpdate += UpdatePosition;
        }

        /// <summary>
        /// Unsubscribe from events on disable
        /// </summary>
        void OnDisable()
        {
            //StopAllCoroutines();
            FloatingOrigin.OnPositionUpdate -= UpdatePosition;
        }

    }

}