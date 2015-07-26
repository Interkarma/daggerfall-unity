// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
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
        public Vector3 ActionRotation = Vector3.zero;               // Rotation to perform
        public Vector3 ActionTranslation = Vector3.zero;            // Translation to perform
        public Vector3 startPosition = Vector3.zero;
        public float ActionDuration = 0;                            // Time to reach final state
        public string ModelDescription = string.Empty;              // Description string for this model
        public int actionSoundID = 0;                               // Action sound ID

        public GameObject NextObject;                               // Next object in action chain
        public GameObject PreviousObject;                           // Previous object in action chain, not always set
       
        public ActionState currentState;
        public DFBlock.RdbActionFlags actionFlag;                   //action type
        public TriggerType triggerType;                             //trigger type
        
        private bool soundSet = false;                               //if true, sound is ready to be played
        AudioSource audioSource;
        private bool debugMessages = false;

        //will trigger action in update if set to true, uncomment the update block to use in inspector
        //public bool triggerAction = false;

        //Action flag -> Action Delegate lookup
        private delegate void Delegate(GameObject obj, DaggerfallAction thisAction);
        static Dictionary<DFBlock.RdbActionFlags, Delegate> actionFunctions = new Dictionary<DFBlock.RdbActionFlags, Delegate>()
    {
        {DFBlock.RdbActionFlags.Translation, new Delegate(Move)},
        {DFBlock.RdbActionFlags.Rotation, new Delegate(Move)},
        {DFBlock.RdbActionFlags.Unlock, new Delegate(Unlock)},
        {DFBlock.RdbActionFlags.Teleport, new Delegate(Teleport)},
        {DFBlock.RdbActionFlags.Activate, new Delegate(Activate)},
        {DFBlock.RdbActionFlags.PlaySound, new Delegate(Activate)},
    };


        #region enums
        //how the action is triggered. unknown1 field
        //The different collision flags don't all use
        //collisions in DFall but similiar methods. 
        //Until they can be worked out, just using collisions in unity
        public enum TriggerType
        {
            none = -1,
            indirect = 0, //can't be activated by player directly
            collide = 1,
            direct = 2,   //levers, switches etc.
            collide3 = 3, //found on bookcase that rotates when collided w.
            collide9 = 9,  //red brick door teleporters, and some palace rooms
            unknown10 = 10, //on the door leading to Cysandra in Daggerfall castle.
        }

       
        public enum ActionState
        {
            Start,
            Playing,
            End,
        }

        #endregion

       

        void Start()
        {
            startPosition = transform.position;
            audioSource = GetComponent<AudioSource>();

            if (actionSoundID > 0 && audioSource)
                soundSet = true;

        }

        /*
        void Update()
        {
            //this is for testing in editor mode, can be removed
            if(triggerAction)
            {
                triggerAction = false;
                GameObject obj = (PreviousObject != null) ? PreviousObject : (GameObject.FindGameObjectWithTag("Player"));
                Play(obj);
            }

        }
        */

        /// <summary>
        /// Handles incoming activations.  
        /// </summary>
        /// <param name="prev">The gameObject that triggered this action, might be player</param>
        /// <param name="playerTriggered">true if player triggered this action directly</param>
        public bool Receive(GameObject prev = null, bool playerTriggered = false)
        {
            DaggerfallUnity.LogMessage(string.Format("{0} Recieved activation, playerTriggered: {1}", gameObject.name, playerTriggered), debugMessages);

            if (playerTriggered && triggerType != TriggerType.direct)
            {
                DaggerfallUnity.LogMessage(string.Format("This action can't be triggered by player directly {0}", triggerType), debugMessages);
                return false;
            }

            if (IsPlaying())
            {
                DaggerfallUnity.LogMessage(string.Format("Already playing; returning"), debugMessages);
                return false;
            }

            //start action sequence
            Play(prev);
            return true;

        }

        /// <summary>
        /// Check if action already playing - right now only used for "Movement" type actions.
        /// Checks all the Next Objects in the chain, returns true if any are playing.
        /// </summary>
        /// <returns></returns>
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



        public void SetState(ActionState state)
        {
            currentState = state;
        }



        /// <summary>
        /// Triggers the next action in chain if any
        /// </summary>
        private void ActivateNext()
        {
            if (NextObject == null)
            {
                DaggerfallUnity.LogMessage(string.Format("Next object is null, can't activate"), debugMessages);
                return;
            }
            else
            {
                DaggerfallAction action = NextObject.GetComponent<DaggerfallAction>();
                if (action != null)
                {
                    action.Receive(this.gameObject, false);
                    DaggerfallUnity.LogMessage(string.Format("Activated Next object {0}", NextObject.name), debugMessages);
                }
            }

        }

        /// <summary>
        /// Get the delegate from the lookup using the actionFlag as key,
        /// if one is found starts the action sequence.
        /// </summary>
        /// <param name="prev"></param>
        private void Play(GameObject prev)
        {
            DaggerfallUnity.LogMessage(string.Format("Playing"), debugMessages);
            Delegate d = null;
            if (actionFunctions.ContainsKey(actionFlag))
            {
                d = actionFunctions[actionFlag];
            }

            //if failed to get valid delegate from lookup, stop
            if (d == null)
            {
                DaggerfallUnity.LogMessage(string.Format("No delegate found for this action flag: {0}", actionFlag), debugMessages);
                return;
            }

            ActivateNext();
            PlaySound();
            d(prev, this);
        }

        //Play sound if set
        private void PlaySound()
        {
            DaggerfallUnity.LogMessage(string.Format("Playing Sound: {0}", actionSoundID), debugMessages);

            if (soundSet)
                audioSource.Play();
        }

        #region Actions
        /// <summary>
        /// Handles translation / rotation type actions.  
        /// If at Start, will TweenToEnd, if at End, will TweenToStart.
        /// </summary>
        /// <param name="prevObj"></param>
        /// <param name="thisAction"></param>
        public static void Move(GameObject obj, DaggerfallAction thisAction)
        {
            DaggerfallUnity.LogMessage(string.Format("Move action"), thisAction.debugMessages);


            if (thisAction.currentState == ActionState.Start)
            {
                thisAction.currentState = ActionState.Playing;
                thisAction.TweenToEnd();
            }
            else if (thisAction.currentState == ActionState.End)
            {
                thisAction.currentState = ActionState.Playing;
                thisAction.TweenToStart();
            }
            else
                DaggerfallUnity.LogMessage(string.Format("Already playing"), thisAction.debugMessages);
        }

        public void TweenToEnd()
        {
            currentState = ActionState.Playing;
            DaggerfallUnity.LogMessage(string.Format("Tweening to end"), debugMessages);

            Hashtable rotateParams = __ExternalAssets.iTween.Hash(
                "amount", new Vector3(ActionRotation.x / 360f, ActionRotation.y / 360f, ActionRotation.z / 360f),
                "space", Space.Self,
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
            currentState = ActionState.Playing;
            DaggerfallUnity.LogMessage(string.Format("Tweening to start"), debugMessages);


            Hashtable rotateParams = __ExternalAssets.iTween.Hash(
                    "amount", new Vector3(-ActionRotation.x / 360f, -ActionRotation.y / 360f, -ActionRotation.z / 360f),
                    "space", Space.Self,
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



        // <summary>
        // 30
        /// </summary>
        /// <param name="prevObj"></param>
        /// <param name="thisAction"></param>
        public static void Activate(GameObject prevObj, DaggerfallAction thisAction)
        {
            DaggerfallUnity.LogMessage(string.Format("Activate action"), thisAction.debugMessages);
            return;
        }


        /// <summary>
        /// 17
        /// Unlocks a door. Have only seen it on the door itself.
        /// Doesn't relock on reverse.  Not sure if can unlock multiple times 
        /// (ie, if you cast lock on door will it unlock it again.
        /// </summary>
        /// <param name="prevObj"></param>
        /// <param name="thisAction"></param>
        public static void Unlock(GameObject prevObj, DaggerfallAction thisAction)
        {
            DaggerfallUnity.LogMessage(string.Format("Unlock door action"), thisAction.debugMessages);
            DaggerfallActionDoor door = thisAction.GetComponent<DaggerfallActionDoor>();
            if (door == null)
            {
                DaggerfallUnity.LogMessage(string.Format("No DaggerfallActionDoor component found to unlock door"), thisAction.debugMessages);
            }
            else
            {
                door.IsLocked = false;
            }

        }

        /// <summary>
        /// 14
        /// This is assumes will always be activated by player directly and input object will always be player.
        /// </summary>
        /// <param name="playerObj"></param>
        /// <param name="thisAction"></param>
        public static void Teleport(GameObject playerObj, DaggerfallAction thisAction)
        {
            DaggerfallUnity.LogMessage(string.Format("Teleport action"), thisAction.debugMessages);

            if (thisAction.NextObject == null)
            {
                DaggerfallUnity.LogMessage(string.Format("Teleport next object null - can't teleport"), thisAction.debugMessages);
                return;
            }
            if (playerObj == null)
            {
                DaggerfallUnity.LogMessage(string.Format("Player object null - can't teleport"), thisAction.debugMessages);
                return;
            }

            //Might need to adjust for player collider height
            playerObj.transform.position = thisAction.NextObject.transform.position;
            playerObj.transform.rotation = thisAction.NextObject.transform.rotation;
        }

        #endregion
    }
}