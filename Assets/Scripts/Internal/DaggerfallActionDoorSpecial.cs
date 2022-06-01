// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    LypyL (lypyl@dfworkshop.net)
// 
// Notes:
//

using UnityEngine;
using System.Collections;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// This door component is assigned when linked action calls for a door, but no normal door record found.
    /// Usually found on super-secret doors the player is never intended to open manually and must find a lever.
    /// Player cannot open, bash, pick, or cast their way through this type of door.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(BoxCollider))]
    public class DaggerfallActionDoorSpecial : MonoBehaviour
    {
        public bool StartOpen = false;                  // Door should start in open state
        public float OpenAngle = -90f;                  // Angle to swing door on axis when opening
        public float OpenDuration = 1.5f;               // How long in seconds for door to open
        public bool IsTriggerWhenOpen = true;           // Collider is disabled when door opens
        public bool PlaySounds = true;                  // Play open and close sounds if present (OpenSound > 0, CloseSound > 0)

        public SoundClips OpenSound = SoundClips.DungeonDoorOpen;            // Sound clip to use when door opens
        public SoundClips CloseSound = SoundClips.DungeonDoorClose;          // Sound clip to use when door closes

        ActionState currentState;
        ulong loadID = 0;

        Quaternion startingRotation;
        AudioSource audioSource;
        BoxCollider boxCollider;

        public ulong LoadID
        {
            get { return loadID; }
            set { loadID = value; }
        }

        public bool IsOpen
        {
            get { return (currentState == ActionState.End); }
        }

        public bool IsClosed
        {
            get { return (currentState == ActionState.Start); }
        }

        public bool IsMoving
        {
            get { return (currentState == ActionState.PlayingForward || currentState == ActionState.PlayingReverse); }
        }

        public Quaternion ClosedRotation
        {
            get { return startingRotation; }
        }

        public ActionState CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            boxCollider = GetComponent<BoxCollider>();
        }

        void Start()
        {
            currentState = ActionState.Start;
            startingRotation = transform.rotation;
            if (StartOpen)
                Open(0, true);
        }

        public void ToggleDoor(bool activatedByPlayer = false)
        {
            if (IsMoving)
                return;

            if (IsOpen)
                Close(OpenDuration, activatedByPlayer);
            else
                Open(OpenDuration, false, activatedByPlayer);
        }

        public void SetOpen(bool open, bool instant = false, bool ignoreLocks = false)
        {
            float duration = (instant) ? 0 : OpenDuration;
            if (open)
                Open(duration, ignoreLocks);
            else
                Close(duration);
        }

        /// <summary>
        /// Restarts a tween in progress. For exmaple, if restoring from save.
        /// </summary>
        public void RestartTween(float durationScale = 1)
        {
            if (currentState == ActionState.PlayingForward)
                Open(OpenDuration * durationScale);
            else if (currentState == ActionState.PlayingReverse)
                Close(OpenDuration * durationScale);
            else if (currentState == ActionState.End)
                MakeTrigger(true);
        }

        #region Private Methods

        private void Open(float duration, bool ignoreLocks = false, bool activatedByPlayer = false)
        {
            //// Tween rotation
            //Hashtable rotateParams = __ExternalAssets.iTween.Hash(
            //    "rotation", startingRotation.eulerAngles + new Vector3(0, OpenAngle, 0),
            //    "time", duration,
            //    "easetype", __ExternalAssets.iTween.EaseType.linear,
            //    "oncomplete", "OnCompleteOpen");
            //__ExternalAssets.iTween.RotateTo(gameObject, rotateParams);
            //currentState = ActionState.PlayingForward;

            // Tween rotation
            Hashtable rotateParams = __ExternalAssets.iTween.Hash(
                "amount", new Vector3(0f, OpenAngle / 360f, 0f),
                "space", Space.Self,
                "time", duration,
                "easetype", __ExternalAssets.iTween.EaseType.linear,
                "oncomplete", "OnCompleteOpen");
            __ExternalAssets.iTween.RotateBy(gameObject, rotateParams);
            currentState = ActionState.PlayingForward;

            // Set collider to trigger only
            MakeTrigger(true);

            // Play open sound if flagged and ready
            if (PlaySounds && OpenSound > 0 && duration > 0 && audioSource)
            {
                DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();
                if (dfAudioSource != null)
                    dfAudioSource.PlayOneShot(OpenSound);
            }

            // For doors that are also action objects, execute action when door opened / closed
            // Only doing so if player was the activator, to keep DoorText actions from running
            // when enemies open doors.
            if (activatedByPlayer)
                ExecuteActionOnToggle();
        }

        private void Close(float duration, bool activatedByPlayer = false)
        {
            // Do nothing if door cannot be closed right now
            if (IsClosed)
                return;

            // Tween rotation
            Hashtable rotateParams = __ExternalAssets.iTween.Hash(
                "rotation", startingRotation.eulerAngles,
                "time", duration,
                "easetype", __ExternalAssets.iTween.EaseType.linear,
                "oncomplete", "OnCompleteClose",
                "oncompleteparams", duration);
            __ExternalAssets.iTween.RotateTo(gameObject, rotateParams);
            currentState = ActionState.PlayingReverse;

            // For doors that are also action objects, execute action when door opened / closed
            // Only doing so if player was the activator, to keep DoorText actions from running
            // when enemies open doors.
            if (activatedByPlayer)
                ExecuteActionOnToggle();
        }

        private void OnCompleteOpen()
        {
            currentState = ActionState.End;
        }

        private void OnCompleteClose(float duration)
        {
            // Play close sound if flagged and ready
            if (PlaySounds && CloseSound > 0 && duration > 0 && audioSource)
            {
                DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();
                dfAudioSource.PlayOneShot(CloseSound);
            }

            // Set collider back to a solid object
            MakeTrigger(false);

            currentState = ActionState.Start;
        }

        private void MakeTrigger(bool isTrigger)
        {
            if (IsTriggerWhenOpen && boxCollider != null)
                boxCollider.isTrigger = isTrigger;
        }

        //For Doors that are also action objects, executes action when door opened / closed
        private void ExecuteActionOnToggle()
        {
            DaggerfallAction action = GetComponent<DaggerfallAction>();
            if(action != null)
                action.Receive(gameObject, DaggerfallAction.TriggerTypes.Door);

        }

        #endregion
    }
}