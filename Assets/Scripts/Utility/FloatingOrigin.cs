// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net), LypyL
// Contributors:    LypyL
// 
// Notes:           Interkarma 23/10/15 - Changed to trigger at map pixel boundary
//                  rather than a relative amount of movement. This fits better with
//                  serialization/deserialization which stores player position relative
//                  to origin of central map pixel.
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Moves world back to origin so central player terrain is at 0,0,0.
    /// Currently only works on X-Z plane.
    /// Need to review floating Y with serialization/deserialization process.
    /// </summary>
    public class FloatingOrigin : MonoBehaviour
    {
        #region Fields

        const float verticalThreshold = 800f;

        public StreamingWorld StreamingWorld;
        public GameObject Player;
        public static FloatingOrigin instance;

        PlayerMotor playerMotor = null;
        DFPosition lastMapPixel;
        DFPosition currentMapPixel;
        //bool forceFloatingOriginUpdate = false;

        #endregion

        #region Properties

        public DFPosition LastMapPixel
        {
            get { return lastMapPixel; }
        }

        public DFPosition CurrentMapPixel
        {
            get { return currentMapPixel; }
        }

        #endregion

        #region Unity

        void Awake()
        {
            instance = this;

            Player = GameObject.FindGameObjectWithTag("Player");
            if (!Player)
                throw new Exception("Player object not set in FloatingOrigin script.");

            if (!StreamingWorld)
                throw new Exception("StreamingWorld not set in FloatingOrigin script.");

            playerMotor = Player.GetComponent<PlayerMotor>();
            if (!playerMotor)
                throw new Exception("Player object does not have a PlayerMotor.");
        }

        void Start()
        {
            Initialize();
            StreamingWorld.OnInitWorld += StreamingWorld_OnInitWorld;
        }

        void FixedUpdate()
        {
            // Must have streaming world reference
            if (!StreamingWorld)
                return;

            // Do nothing during streaming world init
            if (StreamingWorld.IsInit)
                return;

            // Update world position when appropriate
            //if (CheckPosition() || forceFloatingOriginUpdate)
            if (CheckPosition())
            {
                // Get X-Z offset
                float xChange = (currentMapPixel.X - lastMapPixel.X) * (MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale);
                float zChange = (currentMapPixel.Y - lastMapPixel.Y) * (MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale);

                // Get Y offset
                float yChange = 0;
                //if (playerMotor.transform.position.y < -verticalThreshold ||
                //    playerMotor.transform.position.y > verticalThreshold ||
                //    forceFloatingOriginUpdate)
                //{
                //    yChange = -playerMotor.transform.position.y;
                //}

                // Create offset
                Vector3 offset = new Vector3(-xChange, yChange, zChange);

                // Offset player
                OffsetPlayerController(offset);

                // Offset streaming world
                OffsetChildren(StreamingWorld.StreamingTarget.gameObject, offset);

                // Raise event
                RaiseOnPositionUpdateEvent(offset);

                // Lower update flags
                //forceFloatingOriginUpdate = false;
            }
        }

        #endregion

        #region Public Methods

        public void ChangeTargetPlayer(GameObject newPlayer)
        {
            if (!newPlayer)
                throw new Exception("newPlayer is invalid.");

            Player = newPlayer;
        }

        #endregion

        #region Private Methods

        void Initialize()
        {
            playerMotor.ClearActivePlatform();
            DFPosition mapPixel = StreamingWorld.LocalPlayerGPS.CurrentMapPixel;
            currentMapPixel = mapPixel;
            lastMapPixel = mapPixel;
        }

        bool CheckPosition()
        {
            DFPosition mapPixel = StreamingWorld.LocalPlayerGPS.CurrentMapPixel;
            if (mapPixel.X != currentMapPixel.X ||
                mapPixel.Y != currentMapPixel.Y)
            {
                lastMapPixel = currentMapPixel;
                currentMapPixel = mapPixel;
                return true;
            }

            return false;
        }

        void OffsetPlayerController(Vector3 offset)
        {
            playerMotor.ClearActivePlatform();
            playerMotor.AdjustFallStart(offset.y);
            playerMotor.transform.position += offset;
            StreamingWorld.OffsetWorldCompensation(offset, true);
        }

        void OffsetChildren(GameObject parent, Vector3 offset)
        {
            foreach(Transform child in parent.transform)
            {
                child.position += offset;
            }
        }

        #endregion

        #region Events

        // OnPositionUpdate
        public delegate void OnPositionUpdateEventHandler(Vector3 offset);
        public static event OnPositionUpdateEventHandler OnPositionUpdate;
        public void RaiseOnPositionUpdateEvent(Vector3 offset)
        {
            if (OnPositionUpdate != null)
                OnPositionUpdate(offset);
        }

        #endregion

        #region Event Handlers

        private void StreamingWorld_OnInitWorld()
        {
            Initialize();
            //forceFloatingOriginUpdate = true;
        }

        #endregion
    }
}