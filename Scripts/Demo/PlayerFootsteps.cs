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
    /// Player footstep sounds.
    /// Designed to be attached to Player object.
    /// </summary>
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class PlayerFootsteps : MonoBehaviour
    {
        public float WalkStepInterval = 1.6f;
        public float RunStepInterval = 1.8f;
        public float PitchVariance = 0.3f;
        public float GroundDistance = 1.8f;
        public float FootstepVolumeScale = 0.5f;
        public SoundClips FootstepSound = SoundClips.PlayerFootstepNormal;
        public SoundClips FallHardSound = SoundClips.FallHard;
        public SoundClips FallDamageSound = SoundClips.FallDamage;

        DaggerfallAudioSource dfAudioSource;
        PlayerMotor playerMotor;
        AudioSource customAudioSource;
        AudioClip clip;
        Vector3 lastPosition;
        bool lostGrounding;
        float distance;

        void Start()
        {
            // Store references
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            playerMotor = GetComponent<PlayerMotor>();

            // Add our own custom audio source at runtime as we need to change the pitch of footsteps.
            // We don't want that affecting to other sounds on this game object.
            customAudioSource = gameObject.AddComponent<AudioSource>();
            customAudioSource.playOnAwake = false;
            customAudioSource.loop = false;
            customAudioSource.dopplerLevel = 0f;

            // Set start position
            lastPosition = GetHorizontalPosition();
        }

        void FixedUpdate()
        {
            // Load clip
            // Check this here as dynamic clip may need to be reloaded
            if (clip == null)
            {
                clip = dfAudioSource.GetAudioClip((int)FootstepSound, false);
            }

            // Check if player is grounded
            if (!IsGrounded())
            {
                // Player has lost grounding
                distance = 0f;
                lostGrounding = true;
                return;
            }
            else
            {
                // Player is grounded but we might need to reset after losing grounding
                if (lostGrounding)
                {
                    distance = 0f;
                    lastPosition = GetHorizontalPosition();
                    lostGrounding = false;
                    return;
                }
            }

            // Get distance player travelled horizontally
            Vector3 position = GetHorizontalPosition();
            distance += Vector3.Distance(position, lastPosition);
            lastPosition = position;

            // Get threshold
            float threshold = WalkStepInterval;
            if (playerMotor)
                threshold = (playerMotor.IsRunning) ? RunStepInterval : WalkStepInterval;

            // Play sound if over distance threshold
            if (distance > threshold && customAudioSource && clip)
            {
                // Set a random pitch so footsteps don't sound too mechanical
                customAudioSource.pitch = Random.Range(1f - PitchVariance, 1f + PitchVariance);
                customAudioSource.PlayOneShot(clip, FootstepVolumeScale);
                distance = 0f;
            }
        }

        #region Private Methods

        private Vector3 GetHorizontalPosition()
        {
            return new Vector3(transform.position.x, 0, transform.position.z);
        }

        private bool IsGrounded()
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out hit, GroundDistance))
                return true;
            else
                return false;
        }

        // Capture this message so we can play fall damage sound
        private void ApplyPlayerFallDamage(float fallDistance)
        {
            // Play falling damage one-shot through normal audio source
            if (dfAudioSource)
                dfAudioSource.PlayOneShot((int)FallDamageSound, false, FootstepVolumeScale);
        }

        // Capture this message so we can play hard fall sound
        private void HardFallAlert(float fallDistance)
        {
            // Play hard fall one-shot through normal audio source
            if (dfAudioSource)
                dfAudioSource.PlayOneShot((int)FallHardSound, false, FootstepVolumeScale);
        }

        #endregion
    }
}