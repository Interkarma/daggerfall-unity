// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Specialised action component for hinged doors in builings and interiors.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(MeshCollider))]
    public class DaggerfallActionDoor : MonoBehaviour
    {
        public bool StartOpen = false;                  // Door should start in open state
        public bool IsLocked = false;                   // Normally locked doors must be unlocked by keys, spells, or force
        public bool IsMagicallyHeld = false;            // Magically held locks can be opened by spells only
        public float OpenAngle = -90f;                  // Angle to swing door on axis when opening
        public float OpenDuration = 1.5f;               // How long in seconds for door to open
        public bool DisableColliderWhenOpen = true;     // Collider is disabled when door opens
        public float ChanceToBash = 0.25f;              // Chance of successfully bashing open door (0=no chance, 1=first time)
        public bool PlaySounds = true;                  // Play open and close sounds if present (OpenSound > 0, CloseSound > 0)

        public SoundClips OpenSound = SoundClips.NormalDoorOpen;            // Sound clip to use when door opens
        public SoundClips CloseSound = SoundClips.NormalDoorClose;          // Sound clip to use when door closes
        public SoundClips BashSound = SoundClips.PlayerDoorBash;            // Sound clip to use when bashing door
        public SoundClips LockedSound = SoundClips.ActivateLockUnlock;      // Sound clip to use when trying to open locked door

        bool isOpen = false;
        bool isMoving = false;
        Vector3 closedTransform;
        AudioSource audioSource;
        MeshCollider meshCollider;

        public bool IsOpen
        {
            get { return isOpen; }
        }

        public void Start()
        {
            closedTransform = transform.rotation.eulerAngles;
            audioSource = GetComponent<AudioSource>();
            meshCollider = GetComponent<MeshCollider>();

            if (StartOpen)
                Open(0, true);
        }

        public void ToggleDoor()
        {
            if (isOpen)
                Close(OpenDuration);
            else
                Open(OpenDuration);
        }

        public void AttemptBash()
        {
            if (!isOpen)
            {
                // Play bash sound if flagged and ready
                if (PlaySounds && BashSound > 0 && audioSource)
                {
                    DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();
                    dfAudioSource.PlayOneShot(BashSound);
                }

                // Cannot bash magically held doors
                if (!IsMagicallyHeld)
                {
                    // Roll for chance to open
                    Random.seed = Time.frameCount;
                    float roll = Random.Range(0f, 1f);
                    if (roll >= (1f - ChanceToBash))
                    {
                        IsLocked = false;
                        ToggleDoor();
                    }
                }
            }
        }

        public void SetInteriorDoorSounds()
        {
            OpenSound = SoundClips.NormalDoorOpen;
            CloseSound = SoundClips.NormalDoorClose;
            BashSound = SoundClips.PlayerDoorBash;
            LockedSound = SoundClips.ActivateLockUnlock;
        }

        public void SetDungeonDoorSounds()
        {
            OpenSound = SoundClips.DungeonDoorOpen;
            CloseSound = SoundClips.DungeonDoorClose;
            BashSound = SoundClips.PlayerDoorBash;
            LockedSound = SoundClips.ActivateLockUnlock;
        }

        #region Private Methods

        private void Open(float duration, bool ignoreLocks = false)
        {
            // Do nothing if door cannot be opened right now
            if (((IsLocked || IsMagicallyHeld) && !ignoreLocks) || isOpen || isMoving)
            {
                // Play open sound if flagged and ready
                if (PlaySounds && LockedSound > 0 && duration > 0 && audioSource)
                {
                    DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();
                    dfAudioSource.PlayOneShot(LockedSound);
                }

                return;
            }

            // Tween rotation
            Hashtable rotateParams = __ExternalAssets.iTween.Hash(
                "amount", new Vector3(0f, OpenAngle / 360f, 0f),
                "space", Space.Self,
                "time", duration,
                "easetype", __ExternalAssets.iTween.EaseType.linear,
                "oncomplete", "OnCompleteOpen");
            __ExternalAssets.iTween.RotateBy(gameObject, rotateParams);
            isMoving = true;

            // Set collider to trigger only
            if (DisableColliderWhenOpen && meshCollider)
            {
                meshCollider.convex = true;
                meshCollider.isTrigger = true;
            }

            // Play open sound if flagged and ready
            if (PlaySounds && OpenSound > 0 && duration > 0 && audioSource)
            {
                DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();
                dfAudioSource.PlayOneShot(OpenSound);
            }

            // Set flag
            isOpen = true;
        }

        private void Close(float duration)
        {
            // Do nothing if door cannot be closed right now
            if (!isOpen || isMoving)
                return;

            // Tween rotation
            Hashtable rotateParams = __ExternalAssets.iTween.Hash(
                "rotation", closedTransform,
                "time", duration,
                "easetype", __ExternalAssets.iTween.EaseType.linear,
                "oncomplete", "OnCompleteClose",
                "oncompleteparams", duration);
            __ExternalAssets.iTween.RotateTo(gameObject, rotateParams);
            isMoving = true;

            // Set flag
            isOpen = false;
        }

        private void OnCompleteOpen()
        {
            isMoving = false;
        }

        private void OnCompleteClose(float duration)
        {
            isMoving = false;

            // Play close sound if flagged and ready
            if (PlaySounds && CloseSound > 0 && duration > 0 && audioSource)
            {
                DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();
                dfAudioSource.PlayOneShot(CloseSound);
            }

            // Set collider back to a solid object
            if (DisableColliderWhenOpen && meshCollider)
                meshCollider.isTrigger = false;
        }

        #endregion
    }
}
