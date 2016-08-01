// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
    /// Specialised action component for hinged doors in buildings interiors and dungeons.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(BoxCollider))]
    public class DaggerfallActionDoor : MonoBehaviour
    {
        public bool StartOpen = false;                  // Door should start in open state
        public int CurrentLockValue = 0;                // if > 0, door is locked. Can check w. IsLocked prop
        public float OpenAngle = -90f;                  // Angle to swing door on axis when opening
        public float OpenDuration = 1.5f;               // How long in seconds for door to open
        public bool IsTriggerWhenOpen = true;           // Collider is disabled when door opens
        public float ChanceToBash = 0.25f;              // Chance of successfully bashing open door (0=no chance, 1=first time)
        public bool PlaySounds = true;                  // Play open and close sounds if present (OpenSound > 0, CloseSound > 0)

        public SoundClips OpenSound = SoundClips.NormalDoorOpen;            // Sound clip to use when door opens
        public SoundClips CloseSound = SoundClips.NormalDoorClose;          // Sound clip to use when door closes
        public SoundClips BashSound = SoundClips.PlayerDoorBash;            // Sound clip to use when bashing door
        public SoundClips LockedSound = SoundClips.ActivateLockUnlock;      // Sound clip to use when trying to open locked door

        ActionState currentState;
        int startingLockValue = 0;                      // if > 0, is locked.
        ulong loadID = 0;

        Quaternion startingRotation;
        AudioSource audioSource;
        BoxCollider boxCollider;

        public int StartingLockValue                    // Use to set starting lock value, will set current lock value as well
        {
            get { return startingLockValue; }
            set { startingLockValue = CurrentLockValue = value; }
        }

        public ulong LoadID
        {
            get { return loadID; }
            set { loadID = value; }
        }

        public bool IsLocked
        {
            get { return CurrentLockValue > 0; }
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

        public bool IsMagicallyHeld
        {
            get { return CurrentLockValue >= 160; }
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

        public void ToggleDoor()
        {
            if (IsMoving)
                return;

            if (IsOpen)
                Close(OpenDuration);
            else
                Open(OpenDuration);
        }

        public void SetOpen(bool open, bool instant = false, bool ignoreLocks = false)
        {
            float duration = (instant) ? 0 : OpenDuration;
            if (open)
                Open(duration, ignoreLocks);
            else
                Close(duration);
        }

        public void AttemptBash()
        {
            if (!IsOpen)
            {
                // Play bash sound if flagged and ready
                if (PlaySounds && BashSound > 0 && audioSource)
                {
                    DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();
                    if (dfAudioSource != null)
                        dfAudioSource.PlayOneShot(BashSound);
                }

                // Cannot bash magically held doors
                if (!IsMagicallyHeld)
                {
                    // Roll for chance to open
                    UnityEngine.Random.InitState(Time.frameCount);
                    float roll = UnityEngine.Random.Range(0f, 1f);
                    if (roll >= (1f - ChanceToBash))
                    {
                        CurrentLockValue = 0;
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

        private void Open(float duration, bool ignoreLocks = false)
        {
            // Do nothing if door cannot be opened right now
            if ((IsLocked && !ignoreLocks) || IsOpen)
            {
                // Play open sound if flagged and ready
                if (PlaySounds && LockedSound > 0 && duration > 0 && audioSource)
                {
                    DaggerfallAudioSource dfAudioSource = GetComponent<DaggerfallAudioSource>();
                    if (dfAudioSource != null)
                        dfAudioSource.PlayOneShot(LockedSound);
                }

                //Just a temp. setup to provide feedback on locks until lockpicking is added
                if(!IsOpen)
                {
                    string lockedDoorMessage = "This door is locked";
                    if (IsMagicallyHeld)
                        lockedDoorMessage = "This is a magically held door";
                    DaggerfallWorkshop.Game.DaggerfallUI.Instance.PopupMessage(lockedDoorMessage);
                }
                return;
            }

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

            //For Doors that are also action objects, executes action when door opened / closed
            ExecuteActionOnToggle();

            // Set flag
            //IsMagicallyHeld = false;
            CurrentLockValue = 0;
        }

        private void Close(float duration)
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

            //For Doors that are also action objects, executes action when door opened / closed
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