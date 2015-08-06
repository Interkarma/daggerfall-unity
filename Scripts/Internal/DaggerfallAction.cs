// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Lypyl (lypyl@dfworkshop.net)
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Defines and executes Daggerfall action records.
    /// </summary>
    public class DaggerfallAction : MonoBehaviour
    {
        public bool ActionEnabled = false;                                          // Enable/disable action - not currently being used, but some objects are single activate
        public bool PlaySound = true;                                               // Play sound if present (ActionSound > 0)
        public string ModelDescription = string.Empty;                              // Description string for this model
        public DFBlock.RdbActionFlags ActionFlag = DFBlock.RdbActionFlags.None;     // Action flag value
        public DFBlock.RdbTriggerFlags TriggerFlag = DFBlock.RdbTriggerFlags.None;  // Trigger flag value
        public int ActionSoundID = 0;                                               // Action sound ID
        public Vector3 ActionRotation = Vector3.zero;                               // Rotation to perform
        public Vector3 ActionTranslation = Vector3.zero;                            // Translation to perform
        public Space ActionSpace = Space.Self;                                      // Relative space to perform action in (self or world)
        public float ActionDuration = 0;                                            // Time to reach final state
        public GameObject NextObject;                                               // Next object in action chain
        public GameObject PreviousObject;                                           // Previous object in action chain

        AudioSource audioSource;
        ActionState currentState;
        Vector3 startPosition;

        public enum ActionState
        {
            Start,
            Playing,
            End,
        }

        ActionState CurrentState
        {
            get { return currentState; }
            set { SetState(value); }
        }

        //Action flag -> Action Delegate lookup
        private delegate void ActionDelegate(GameObject obj, DaggerfallAction thisAction);

        static Dictionary<DFBlock.RdbActionFlags, ActionDelegate> actionFunctions = new Dictionary<DFBlock.RdbActionFlags, ActionDelegate>()
        {
        {DFBlock.RdbActionFlags.Translation,new ActionDelegate(Move)},
        {DFBlock.RdbActionFlags.Rotation,   new ActionDelegate(Move)},
        {DFBlock.RdbActionFlags.Unlock,     new ActionDelegate(UnlockDoor)},
        {DFBlock.RdbActionFlags.OpenDoor,   new ActionDelegate(OpenDoor)},
        {DFBlock.RdbActionFlags.CloseDoor,  new ActionDelegate(CloseDoor)},
        {DFBlock.RdbActionFlags.Teleport,   new ActionDelegate(Teleport)},
        {DFBlock.RdbActionFlags.Hurt22,     new ActionDelegate(Hurt)},
        {DFBlock.RdbActionFlags.Hurt23,     new ActionDelegate(Hurt)},
        {DFBlock.RdbActionFlags.Activate,   new ActionDelegate(Activate)},
        };


        void Start()
        {
            currentState = ActionState.Start;
            startPosition = transform.position;
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Handles incoming activations.  
        /// </summary>
        public void Receive(GameObject prev = null, bool playerTriggered = false)
        {
            if (playerTriggered && TriggerFlag != DFBlock.RdbTriggerFlags.Direct && playerTriggered && TriggerFlag != DFBlock.RdbTriggerFlags.DualTrigger)
            {
                return;
            }

            if (IsPlaying())
            {
                DaggerfallUnity.LogMessage(string.Format("Already playing"));
                return;
            }

            //start action sequence
            Play(prev);
            return;

        }

        public void Play(GameObject prev)
        {
            ActionDelegate d = null;
            if (actionFunctions.ContainsKey(ActionFlag))
            {
                d = actionFunctions[ActionFlag];
            }

            //always activate next && play sound, even if unknown action type. Many will just activate next actions
            //this will have to be updated in future, as a couple actions (like Text w/ input) only trigger next conditionally, but it works for now
            ActivateNext();
            if (PlaySound && ActionSoundID > 0 && audioSource)
            {
                Debug.Log("Playing sound");
                audioSource.Play();
            }

            //if failed to get valid delegate from lookup, stop
            if (d == null)
            {
                DaggerfallUnity.LogMessage(string.Format("No delegate found for this action flag: {0}", ActionFlag));
                return;
            }


            d(prev, this);
        }

        public bool IsPlaying()
        {
            // Check if this action or any chained action is playing
            if (currentState == ActionState.Playing)
            {
                return true;
            }
            else
            {
                if (NextObject == null)
                    return false;

                DaggerfallAction nextAction = NextObject.GetComponent<DaggerfallAction>();
                if (nextAction == null)
                    return false;

                return nextAction.IsPlaying();
            }
        }


        #region Action Helper Methods
        /// <summary>
        /// Triggers the next action in chain if any
        /// </summary>
        private void ActivateNext()
        {
            if (NextObject == null)
            {
                DaggerfallUnity.LogMessage(string.Format("Next object is null, can't activate"));
                return;
            }
            else
            {
                DaggerfallAction action = NextObject.GetComponent<DaggerfallAction>();
                if (action != null)
                {
                    action.Receive(this.gameObject, false);
                }
            }

        }

        public void SetState(ActionState state)
        {
            currentState = state;
        }

     

        private void TweenToEnd()
        {
            Hashtable rotateParams = __ExternalAssets.iTween.Hash(
                "amount", new Vector3(ActionRotation.x / 360f, ActionRotation.y / 360f, ActionRotation.z / 360f),
                "space", ActionSpace,
                "time", ActionDuration,
                "easetype", __ExternalAssets.iTween.EaseType.linear);

            Hashtable moveParams = __ExternalAssets.iTween.Hash(
                "position", startPosition + ActionTranslation,
                "time", ActionDuration,
                "easetype", __ExternalAssets.iTween.EaseType.linear,
                "oncomplete", "SetState",
                "oncompleteparams", ActionState.End);

            __ExternalAssets.iTween.RotateBy(gameObject, rotateParams);
            __ExternalAssets.iTween.MoveTo(gameObject, moveParams);
        }

        private void TweenToStart()
        {
            Hashtable rotateParams = __ExternalAssets.iTween.Hash(
                "amount", new Vector3(-ActionRotation.x / 360f, -ActionRotation.y / 360f, -ActionRotation.z / 360f),
                "space", ActionSpace,
                "time", ActionDuration,
                "easetype", __ExternalAssets.iTween.EaseType.linear);

            Hashtable moveParams = __ExternalAssets.iTween.Hash(
                "position", startPosition,
                "time", ActionDuration,
                "easetype", __ExternalAssets.iTween.EaseType.linear,
                "oncomplete", "SetState",
                "oncompleteparams", ActionState.Start);

            __ExternalAssets.iTween.RotateBy(gameObject, rotateParams);
            __ExternalAssets.iTween.MoveTo(gameObject, moveParams);
        }

        /// <summary>
        ///  Used by door actions (open / close / unlock), tries to get DaggerfallActionDoor from object
        ///  returns true if object is a valid action door, false if not
        /// </summary>
        public static bool GetDoor(GameObject go, out DaggerfallActionDoor door)
        {
            door = null;
            if (go == null)
                return false;

            door = go.GetComponent<DaggerfallActionDoor>();
            if (door == null)
            {
                DaggerfallUnity.LogMessage(string.Format("No DaggerfallActionDoor component found to unlock door: {0}", go.name));
                return false;
            }
            else
                return true;

        }
        #endregion

        #region Actions
        /// <summary>
        /// Handles translation / rotation type actions.  
        /// If at Start, will TweenToEnd, if at End, will TweenToStart.
        /// </summary>
        public static void Move(GameObject obj, DaggerfallAction thisAction)
        {
            if (thisAction.CurrentState == ActionState.Start)
            {
                thisAction.CurrentState = ActionState.Playing;
                thisAction.TweenToEnd();
            }
            else if (thisAction.CurrentState == ActionState.End)
            {
                thisAction.CurrentState = ActionState.Playing;
                thisAction.TweenToStart();
            }
        }

        // <summary>
        /// Just activates next object in chain.
        /// </summary>
        public static void Activate(GameObject prevObj, DaggerfallAction thisAction)
        {
            return;
        }


        /// <summary>
        /// 17
        /// Unlocks a door. Doesn't relock on reverse.
        /// </summary>
        public static void UnlockDoor(GameObject prevObj, DaggerfallAction thisAction)
        {
            DaggerfallActionDoor door;
            if (!GetDoor(thisAction.gameObject, out door))
            {
                DaggerfallUnity.LogMessage(string.Format("No DaggerfallActionDoor component found to unlock door"));

            }
            else
            {
                door.currentLockValue = 0;

            }
        }


        /// <summary>
        /// Opens (and unlocks if is locked) door
        /// </summary>
        public static void OpenDoor(GameObject prevObj, DaggerfallAction thisAction)
        {
            DaggerfallActionDoor door;
            if (!GetDoor(thisAction.gameObject, out door))
            {
                DaggerfallUnity.LogMessage(string.Format("No DaggerfallActionDoor component found"));
            }
            else
            {
                if (door.IsOpen)
                    return;
                else
                {
                    door.currentLockValue = 0;
                    door.ToggleDoor();
                }
            }
        }


        /// <summary>
        /// 20
        /// Closes door on activate.  If door has a starting lock value, will re-lock door.
        /// </summary>
        public static void CloseDoor(GameObject prevObj, DaggerfallAction thisAction)
        {

            DaggerfallActionDoor door;
            if (!GetDoor(thisAction.gameObject, out door))
            {
                DaggerfallUnity.LogMessage(string.Format("No DaggerfallActionDoor component found"));
            }
            else
            {
                if (!door.IsOpen)
                    return;
                else
                {
                    door.ToggleDoor();
                    door.currentLockValue = door.StartingLockValue;
                }

            }

        }

        /// <summary>
        /// This just simulates activating one of the action objects that hurt the player.
        /// Not enough is known right now to do more
        /// </summary>
        public static void Hurt(GameObject playerObj, DaggerfallAction thisAction)
        {
            if(playerObj == null)
                return;

            playerObj.SendMessage("RemoveHealth", SendMessageOptions.DontRequireReceiver);
        }


        /// <summary>
        /// 14
        /// This is assumes will always be activated by player directly and input object will always be player.
        /// </summary>
        public static void Teleport(GameObject playerObj, DaggerfallAction thisAction)
        {

            if (thisAction.NextObject == null)
            {
                DaggerfallUnity.LogMessage(string.Format("Teleport next object null - can't teleport"));
                return;
            }
            if (playerObj == null)
            {
                DaggerfallUnity.LogMessage(string.Format("Player object null - can't teleport"));
                return;
            }

            //Might need to adjust for player collider height
            playerObj.transform.position = thisAction.NextObject.transform.position;
            playerObj.transform.rotation = thisAction.NextObject.transform.rotation;
        }

        #endregion
    }
}