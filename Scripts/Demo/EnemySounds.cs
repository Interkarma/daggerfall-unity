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
    /// Example enemy sounds component.
    /// Handles attract and attack sounds.
    /// </summary>
    [RequireComponent(typeof(DaggerfallAudioSource))]
    public class EnemySounds : MonoBehaviour
    {
        public float AttractRadius = 16f;           // Range at which enemy will play attract sounds like "move" and "bark"
        public int MinAttractDelay = 3;             // Minimum time between attract sounds
        public int MaxAttractDelay = 9;             // Maximum time between attract sounds
        public bool MuteHumanSounds = true;         // Human sounds are so bad even Daggerfall doesn't play them. Unmute here if you want them.
        public bool LinearRolloff = true;           // Linear rolloff is more Daggerfall-like
        public bool SoundsFromMobile;               // Assign sounds automatically based on a DaggerfallMobileUnit child object
        public SoundClips MoveSound;
        public SoundClips BarkSound;
        public SoundClips AttackSound;

        AudioClip moveClip;
        AudioClip barkClip;
        AudioClip attackClip;

        GameObject player;
        DaggerfallMobileUnit mobile;
        DaggerfallAudioSource dfAudioSource;
        Vector3 directionToPlayer;
        float distanceToPlayer;
        bool playerInAttractRadius;
        float waitTime;
        float waitCounter;
        float volumeScale = 1f;

        void Start()
        {
            // Save references
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
            player = GameObject.FindGameObjectWithTag("Player");
            mobile = GetComponentInChildren<DaggerfallMobileUnit>();

            // Setup audio source
            dfAudioSource.AudioSource.maxDistance = AttractRadius;
            if (LinearRolloff)
                dfAudioSource.AudioSource.rolloffMode = AudioRolloffMode.Linear;
            dfAudioSource.AudioSource.spatialBlend = 1;

            // Assign sounds from mobile
            if (SoundsFromMobile && mobile)
            {
                MoveSound = (SoundClips)mobile.Summary.Enemy.MoveSound;
                BarkSound = (SoundClips)mobile.Summary.Enemy.BarkSound;
                AttackSound = (SoundClips)mobile.Summary.Enemy.AttackSound;
            }

            // Start attrack timer
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

        public void PlayAttackSound()
        {
            if (IsReady())
            {
                // Humans must be flagged to play sounds
                if (IgnoreHumanSounds())
                    return;

                // Play attack sound only about half the time
                if (Random.value > 0.5f)
                    dfAudioSource.AudioSource.PlayOneShot(attackClip);
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
            if (!attackClip)
                attackClip = dfAudioSource.GetAudioClip((int)AttackSound);

            return true;
        }

        private void StartWaiting()
        {
            // Reset countdown to next sound
            waitTime = Random.Range(MinAttractDelay, MaxAttractDelay);
            waitCounter = 0;
        }

        private void PlayAttractSound()
        {
            // Humans must be flagged to play sounds
            if (IgnoreHumanSounds())
                return;

            // Dampen volume if something is between enemy and player
            SetVolumeScale();

            // Random chance favors bark sound
            if (Random.value > 0.8f)
                dfAudioSource.AudioSource.PlayOneShot(moveClip, volumeScale);
            else
                dfAudioSource.AudioSource.PlayOneShot(barkClip, volumeScale);
        }

        private bool IgnoreHumanSounds()
        {
            if (mobile.Summary.Enemy.ID > 127 && MuteHumanSounds)
                return true;
            else
                return false;
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
                if (hit.transform.gameObject.isStatic || door)
                    volumeScale = 0.25f;
            }
        }

        #endregion
    }
}