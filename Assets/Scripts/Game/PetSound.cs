// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class PetSound : MonoBehaviour
    {
        public float AttractRadius = 16f;           // Range at which enemy will play attract sounds like "move" and "bark"
        public int MinAttractDelay = 3;             // Minimum time between attract sounds
        public int MaxAttractDelay = 9;             // Maximum time between attract sounds
        public bool MuteHumanSounds = true;         // Human sounds are so bad even Daggerfall doesn't play them. Unmute here if you want them.
        public bool LinearRolloff = true;           // Linear rolloff is more Daggerfall-like
        public bool SoundsFromMobile;               // Assign sounds automatically based on a DaggerfallMobileUnit child object
        public SoundClips MoveSound;
        public SoundClips BarkSound;
        public Entity.Races RaceForSounds;

        AudioClip moveClip;
        AudioClip barkClip;
        AudioClip attackClip;

        GameObject player;
        MobileUnit mobile;
        DaggerfallAudioSource dfAudioSource;
        Vector3 directionToPlayer;
        float distanceToPlayer;
        bool playerInAttractRadius;
        float waitTime;
        float waitCounter;
        float volumeScale = 1f;

        void Awake()
        {
            // Save references
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            player = GameObject.FindGameObjectWithTag("Player");
            mobile = GetComponentInChildren<MobileUnit>();

            // Setup audio source
            dfAudioSource.AudioSource.maxDistance = AttractRadius;
            if (LinearRolloff)
                dfAudioSource.AudioSource.rolloffMode = AudioRolloffMode.Linear;
            dfAudioSource.AudioSource.spatialBlend = 1;

            // Assign sounds from mobile
            if (SoundsFromMobile && mobile)
            {
                MoveSound = (SoundClips)mobile.Enemy.MoveSound;
                BarkSound = (SoundClips)mobile.Enemy.BarkSound;
            }

            RaceForSounds = (Entity.Races)Random.Range(1, 5 + 1);

            // Start attract timer
            StartWaiting();
        }

        void FixedUpdate()
        {
            if (IsReady() && player)
            {
                directionToPlayer = player.transform.position - transform.position;
                distanceToPlayer = directionToPlayer.magnitude;
                playerInAttractRadius = (distanceToPlayer < AttractRadius);
            }
            else
            {
                playerInAttractRadius = false;
            }

            // Step timer.
            // Keep stepping even when player not in attract radius.
            // This means the player will get audio feedback the moment an enemy is near.
            waitCounter += Time.deltaTime;
            if (waitCounter > waitTime && playerInAttractRadius)
            {
                PlayAttractSound();
                StartWaiting();
            }
        }


        #region Private Methods

        private bool IsReady()
        {
            if (dfAudioSource == null)
                return false;

            if (!moveClip)
                moveClip = dfAudioSource.GetAudioClip((int)MoveSound);
            if (!barkClip)
                barkClip = dfAudioSource.GetAudioClip((int)BarkSound);

            return true;
        }

        private void StartWaiting()
        {
            // Reset countdown to next sound
            waitTime = Random.Range(MinAttractDelay, MaxAttractDelay + 1);
            waitCounter = 0;
        }

        private void PlayAttractSound()
        {

            // Dampen volume if something is between enemy and player
            SetVolumeScale();

            // Random chance favors bark sound
            if (Random.value > 0.8f)
                dfAudioSource.AudioSource.PlayOneShot(moveClip, volumeScale * DaggerfallUnity.Settings.SoundVolume);
            else
                dfAudioSource.AudioSource.PlayOneShot(barkClip, volumeScale * DaggerfallUnity.Settings.SoundVolume);
        }


        private void SetVolumeScale()
        {
            // Start at full volume scale
            volumeScale = 1f;

            // If something is between enemy and player then dampen sound.
            // This is a basic cheat for sound propagation.
            // Only checks when enemy plays attract sound, so not very expensive.
            RaycastHit hit;
            Ray ray = new Ray(transform.position, directionToPlayer);
            if (Physics.Raycast(ray, out hit))
            {
                // Ignore player hit
                if (hit.transform.gameObject == player)
                    return;

                // Doors aren't static but we want them to dampen also
                DaggerfallActionDoor door = hit.transform.GetComponent<DaggerfallActionDoor>();

                // Dampen based on hit
                if (GameObjectHelper.IsStaticGeometry(hit.transform.gameObject) || door)
                    volumeScale = 0.25f;
            }
        }

        #endregion
    }
}
