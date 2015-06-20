// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net), LypyL
// Contributors:    LypyL - major overhaul and support for floating vertical offset
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Moves player and world back near origin after a certain threshold distance reached.
    /// Must be attached to Player body parent GameObject that is peered to player controller.
    /// Call Initialize() whenever you move the player manually (e.g. teleport).
    /// </summary>
    public class FloatingOrigin : MonoBehaviour
    {
        const float threshold = 1000f;

        // Must specify streaming world component
        public StreamingWorld StreamingWorld;
        public GameObject player;
        public static FloatingOrigin instance;
        private Vector3 _totalOffset;
        private Vector3 _lastOffset;

        public Vector3 totalOffset
        {
            get { return _totalOffset; }
        }
        public Vector3 lastOffset
        {
            get { return _lastOffset; }
        }

        void Awake()
        {
            instance = this;

        }
        void Start()
        {
            if (!player)
                player = GameObject.FindGameObjectWithTag("Player");
            if (!StreamingWorld)
                StreamingWorld = GameObject.Find("StreamingWorld").GetComponent<StreamingWorld>();
        }

        void FixedUpdate()
        {
            // Must have streaming world reference
            if (!StreamingWorld)
                return;
            // Do nothing during streaming world init
            if (StreamingWorld.IsInit)
                return;


            if (CheckPosition())
            {
                Vector3 change = GetOffset(player.transform.position);
                _totalOffset += change;
                _lastOffset = change;

                // Offset streaming world
                OffsetChildren(StreamingWorld.gameObject, change);
                StreamingWorld.OffsetWorldCompensation(change);

                // Update last position
                RaiseOnPositionUpdateEvent(change);             //##trigger position update

            }
        }


        public bool CheckPosition()
        {
            return (Vector3.Distance(Vector3.zero, player.transform.position) > threshold);
        }

        /// <summary>
        /// Get negating vector to place player back at origin
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        Vector3 GetOffset(Vector3 pos)
        {
            return new Vector3(
                -(pos.x),
                -(pos.y),
                -(pos.z)
                );
        }

        void OffsetChildren(GameObject go, Vector3 offset)
        {
            foreach (Transform transform in go.transform)
            {
                transform.position += offset;
            }

            // TODO: Offset physic properties
        }

        public void OffsetPlayerController()
        {
            // Clear moving platform from player controller to prevent player moving improperly with world
            // You should implement ClearActivePlatform() or similar when using your own controller
            if (player)
                player.SendMessage("ClearActivePlatform", SendMessageOptions.DontRequireReceiver);
        }

        public void ChangeTargetPlayer(GameObject newPlayer)
        {
            if (newPlayer == null)
            {
                Debug.LogError("newPlayer invalid gameobject");
                return;
            }

            //Debug.Log("Changing player " + newPlayer.name);
            this.player = newPlayer;
        }

        /// <summary>
        /// Does nothing anymore - remove reference from StreamingWorld and delete this if you want.
        /// </summary>
        public void Initialize()
        {
            return;

        }

        #region Event Handlers

        // OnPositionUpdate
        public delegate void OnPositionUpdateEventHandler(Vector3 offset);
        public static event OnPositionUpdateEventHandler OnPositionUpdate;
        public void RaiseOnPositionUpdateEvent(Vector3 offset)
        {
            if (OnPositionUpdate != null)
            {
                OffsetPlayerController();
                OnPositionUpdate(offset);
            }
        }

        #endregion
    }
}