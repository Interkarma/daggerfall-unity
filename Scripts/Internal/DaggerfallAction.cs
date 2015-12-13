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
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Defines and executes Daggerfall action records.
    /// </summary>
    public class DaggerfallAction : MonoBehaviour
    {
        public const int TYPE_11_TEXT_INDEX = 8600;
        public const int TYPE_12_TEXT_INDEX = 5400;
        public const int ANSWER_TEXT_INDEX = 5656;

        public bool ActionEnabled = false;                                          // Enable/disable action - not currently being used, but some objects are single activate
        public bool PlaySound = true;                                               // Play sound if present (ActionSound > 0)
        public string ModelDescription = string.Empty;                              // Description string for this model
        public DFBlock.RdbActionFlags ActionFlag = DFBlock.RdbActionFlags.None;     // Action flag value
        public DFBlock.RdbTriggerFlags TriggerFlag = DFBlock.RdbTriggerFlags.None;  // Trigger flag value
        public Vector3 ActionRotation = Vector3.zero;                               // Rotation to perform
        public Vector3 ActionTranslation = Vector3.zero;                            // Translation to perform
        public int Index = 0;                                                       // Index for things like spells, text, and also the raw sound index from daggerfall
        public float Magnitude = 0.0f;                                              // How far to move, how much damage etc.
        public float ActionDuration = 0;                                            // Time to reach final state
        public Space ActionSpace = Space.Self;                                      // Relative space to perform action in (self or world)

        public GameObject NextObject;                                               // Next object in action chain
        public GameObject PreviousObject;                                           // Previous object in action chain

        long loadID = 0;
        Vector3 startingPosition;
        Quaternion startingRotation;

        AudioSource audioSource;
        ActionState currentState;

        //lookup for action type12, temp. storing them here
        static Dictionary<int, string[]> actionTypeTwelveLookup = new Dictionary<int, string[]>()
        {
            {5404, new string[]{"bow","bow arrow","crossbow","bows","crossbows"}},   //sheogorath answer index = 5660
            {5406, new string[]{"one","1"}},                                         //blind god, answer index = 5662
            {5423, new string[]{"benefactor","the benefactor"}},                      //benefactor answer index = 5679
            {5424, new string[]{"shut up","shutup","shaddup"}},                         //shaddup! answer index = 5680
            {5464, new string[]{"yes","oK","i agree","y","agreed","done","fine","okay","sure","yep"}}, //daggerfall guard answer index = 5720
        };

        public string[] type12_answers;//answers for action type 12 dialogue questions



        public long LoadID
        {
            get { return loadID; }
            set { loadID = value; }
        }

        public Vector3 StartingPosition
        {
            get { return startingPosition; }
        }

        public Quaternion StartingRotation
        {
            get { return startingRotation; }
        }

        public ActionState CurrentState
        {
            get { return currentState; }
            set { SetState(value); }
        }

        public bool IsMoving
        {
            get { return (currentState == ActionState.PlayingForward || currentState == ActionState.PlayingReverse); }
        }

        /// <summary>
        /// Gets the actual duration for timed actions.
        /// </summary>
        public float Duration
        {
            get { return ActionDuration / 20f; }
        }

        //Action flag -> Action Delegate lookup
        private delegate void ActionDelegate(GameObject obj, DaggerfallAction thisAction);

        static Dictionary<DFBlock.RdbActionFlags, ActionDelegate> actionFunctions = new Dictionary<DFBlock.RdbActionFlags, ActionDelegate>()
        {
        {DFBlock.RdbActionFlags.Translation,new ActionDelegate(Move)},
        {DFBlock.RdbActionFlags.Rotation,   new ActionDelegate(Move)},
        {DFBlock.RdbActionFlags.PositiveX,  new ActionDelegate(Move)},
        {DFBlock.RdbActionFlags.NegativeX,  new ActionDelegate(Move)},
        {DFBlock.RdbActionFlags.PositiveZ,  new ActionDelegate(Move)},
        {DFBlock.RdbActionFlags.NegativeZ,  new ActionDelegate(Move)},
        {DFBlock.RdbActionFlags.PositiveY,  new ActionDelegate(Move)},
        {DFBlock.RdbActionFlags.NegativeY,  new ActionDelegate(Move)},
        {DFBlock.RdbActionFlags.ShowText,   new ActionDelegate(ShowText)},
        {DFBlock.RdbActionFlags.ShowTextWithInput,   new ActionDelegate(ShowTextWithInput)},
        {DFBlock.RdbActionFlags.Teleport,   new ActionDelegate(Teleport)},
        {DFBlock.RdbActionFlags.LockDoor,   new ActionDelegate(LockDoor)},
        {DFBlock.RdbActionFlags.UnlockDoor, new ActionDelegate(UnlockDoor)},
        {DFBlock.RdbActionFlags.OpenDoor,   new ActionDelegate(OpenDoor)},
        {DFBlock.RdbActionFlags.CloseDoor,  new ActionDelegate(CloseDoor)},
        {DFBlock.RdbActionFlags.Hurt21,     new ActionDelegate(Hurt)},      //lots of damage, varies
        {DFBlock.RdbActionFlags.Hurt22,     new ActionDelegate(Hurt)},      //22-25; dmg = level x magnitude
        {DFBlock.RdbActionFlags.Hurt23,     new ActionDelegate(Hurt)},
        {DFBlock.RdbActionFlags.Hurt24,     new ActionDelegate(Hurt)},
        {DFBlock.RdbActionFlags.Hurt25,     new ActionDelegate(Hurt)},
        {DFBlock.RdbActionFlags.Activate,   new ActionDelegate(Activate)},
        };

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            currentState = ActionState.Start;
            startingPosition = transform.position;
            startingRotation = transform.rotation;
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

            //Temp fix to prevent disabled objects from trying to play; 
            //the move types will get stuck in playing state which prevents other objects from working
            if (!gameObject.activeSelf || !this.enabled)
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


            if (ActionFlag != DFBlock.RdbActionFlags.ShowTextWithInput)
                ActivateNext();

            if (PlaySound && Index > 0 && audioSource)
            {
                audioSource.Play();
            }

            //stop if failed to get valid delegate from lookup - ideally this check should be done before playing
            //sound & activating next, but for testing purposes is done after
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
            if (currentState == ActionState.PlayingForward || currentState == ActionState.PlayingReverse)
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
                DaggerfallUnity.LogMessage(string.Format("Next object is null"));
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

        /// <summary>
        /// Restarts a tween in progress. For exmaple, if restoring from save.
        /// </summary>
        public void RestartTween(float durationScale = 1)
        {
            if (currentState == ActionState.PlayingForward)
                TweenToEnd(Duration * durationScale);
            else if (currentState == ActionState.PlayingReverse)
                TweenToStart(Duration * durationScale);
        }

        private void TweenToEnd(float duration)
        {
            Hashtable rotateParams = __ExternalAssets.iTween.Hash(
                "rotation", startingRotation.eulerAngles + ActionRotation,
                "space", ActionSpace,
                "time", duration,
                "easetype", __ExternalAssets.iTween.EaseType.linear);

            Hashtable moveParams = __ExternalAssets.iTween.Hash(
                "position", startingPosition + ActionTranslation,
                "time", duration,
                "easetype", __ExternalAssets.iTween.EaseType.linear,
                "oncomplete", "SetState",
                "oncompleteparams", ActionState.End);

            __ExternalAssets.iTween.RotateTo(gameObject, rotateParams);
            __ExternalAssets.iTween.MoveTo(gameObject, moveParams);
        }

        private void TweenToStart(float duration)
        {
            Hashtable rotateParams = __ExternalAssets.iTween.Hash(
                "rotation", startingRotation.eulerAngles,
                "space", ActionSpace,
                "time", duration,
                "easetype", __ExternalAssets.iTween.EaseType.linear);

            Hashtable moveParams = __ExternalAssets.iTween.Hash(
                "position", startingPosition,
                "time", duration,
                "easetype", __ExternalAssets.iTween.EaseType.linear,
                "oncomplete", "SetState",
                "oncompleteparams", ActionState.Start);

            __ExternalAssets.iTween.RotateTo(gameObject, rotateParams);
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

        /// <summary>
        /// Handles the input return event for action type 12
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="userInput"></param>
        public void UserInputHandler(DaggerfallInputMessageBox sender, string userInput)
        {
            if (type12_answers == null || type12_answers.Length == 0)
            {
                DaggerfallUnity.LogMessage(string.Format(("No answers to check for: {0} {1}"), this.gameObject.name, this.Index));
                return;
            }
            userInput = userInput.ToLower();
            for (int i = 0; i < type12_answers.Length; i++)
            {
                if (userInput == type12_answers[i].ToLower())
                {
                    ActivateNext();
                    return;
                }
            }
            //Debug.Log("no matching answer found for: " + userInput);
        }


        #endregion

        #region Actions
        /// <summary>
        /// 1-8
        /// Handles translation / rotation type actions.  
        /// </summary>
        public static void Move(GameObject obj, DaggerfallAction thisAction)
        {
            if (thisAction.CurrentState == ActionState.Start)
            {
                thisAction.CurrentState = ActionState.PlayingForward;
                thisAction.TweenToEnd(thisAction.Duration);
            }
            else if (thisAction.CurrentState == ActionState.End)
            {
                thisAction.CurrentState = ActionState.PlayingReverse;
                thisAction.TweenToStart(thisAction.Duration);
            }

        }

        /// <summary>
        /// 11
        /// Pop-up text
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="thisAction"></param>
        public static void ShowText(GameObject obj, DaggerfallAction thisAction)
        {
            DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallWorkshop.Game.DaggerfallUI.UIManager, null);
            messageBox.SetTextTokens(thisAction.Index + TYPE_11_TEXT_INDEX);
            messageBox.ClickAnywhereToClose = true;
            messageBox.ParentPanel.BackgroundColor = Color.clear;
            messageBox.Show();
        }

        /// <summary>
        /// 12
        /// Pop-up text that returns player input
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="thisAction"></param>
        public static void ShowTextWithInput(GameObject obj, DaggerfallAction thisAction)
        {
            int textID = thisAction.Index + TYPE_12_TEXT_INDEX;
            if (actionTypeTwelveLookup.ContainsKey(textID))
            {
                thisAction.type12_answers = actionTypeTwelveLookup[textID];
            }
            else
            {
                Debug.LogError(string.Format("Error: invalid key: {0} for action type 12, couldn't get answer(s)", textID));//todo - display error message
            }
            DaggerfallInputMessageBox inputBox = new DaggerfallInputMessageBox(DaggerfallWorkshop.Game.DaggerfallUI.UIManager, textID, 20, "\t> ", true, false, null);
            inputBox.ParentPanel.BackgroundColor = Color.clear;
            inputBox.OnGotUserInput += thisAction.UserInputHandler;
            inputBox.Show();
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

        /// <summary>
        /// 16
        /// Locks door when activated. Lock value used is unknown
        /// </summary>
        /// <param name="prevObject"></param>
        /// <param name="thisAction"></param>
        public static void LockDoor(GameObject prevObject, DaggerfallAction thisAction)
        {
            DaggerfallActionDoor door;
            if (!GetDoor(thisAction.gameObject, out door))
            {
                DaggerfallUnity.LogMessage(string.Format("No DaggerfallActionDoor component found to lock door"), true);

            }
            else
            {
                if (!door.IsLocked)
                    door.CurrentLockValue = 16; //don't know what what setting Daggerfall uses here
            }
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
                door.CurrentLockValue = 0;

            }
        }


        /// <summary>
        /// 18
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
                    door.CurrentLockValue = 0;
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
                    door.CurrentLockValue = door.StartingLockValue;
                }

            }

        }

        /// <summary>
        /// This just simulates activating one of the action objects that hurt the player.
        /// 21 does lots of damage, and varies.  Can only activate once.
        /// 22-25 do Player Level * Magnitude in Damage. Multiple activation
        /// </summary>
        public static void Hurt(GameObject playerObj, DaggerfallAction thisAction)
        {
            if (playerObj == null)
                return;

            playerObj.SendMessage("RemoveHealth", SendMessageOptions.DontRequireReceiver);
        }


        // <summary>
        /// Just activates next object in chain.
        /// </summary>
        public static void Activate(GameObject prevObj, DaggerfallAction thisAction)
        {
            return;
        }



        #endregion
    }
}