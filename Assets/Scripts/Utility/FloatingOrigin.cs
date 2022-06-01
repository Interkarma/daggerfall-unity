// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Game;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Moves world back to origin so central player terrain is at 0,0,0.
    /// </summary>
    public class FloatingOrigin : MonoBehaviour
    {
        #region Fields

        public const int floatingOriginVersion = 3;

        const float verticalThreshold = 500f;

        public StreamingWorld StreamingWorld;
        public GameObject Player;
        public static FloatingOrigin instance;

        PlayerMotor playerMotor = null;
        AcrobatMotor acrobatMotor = null;
        PlayerGroundMotor groundMotor = null;
        DFPosition lastMapPixel;
        DFPosition currentMapPixel;

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

            acrobatMotor = Player.GetComponent<AcrobatMotor>();
            if (!acrobatMotor)
                throw new Exception("Player object does not have an AcrobatMotor.");

            groundMotor = Player.GetComponent<PlayerGroundMotor>();
            if (!groundMotor)
                throw new Exception("Player object does not have a PlayerGroundMotor.");
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

            // Get Y change
            float yChange = 0;
            if (playerMotor.transform.position.y < -verticalThreshold ||
                playerMotor.transform.position.y > verticalThreshold)
            {
                yChange = -playerMotor.transform.position.y;
            }

            // Get X-Z (map pixel) change
            float xChange = 0, zChange = 0;
            if (CheckMapPosition())
            {
                xChange = (currentMapPixel.X - lastMapPixel.X) * (MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale);
                zChange = (currentMapPixel.Y - lastMapPixel.Y) * (MapsFile.WorldMapTerrainDim * MeshReader.GlobalScale);
            }

            // Create offset
            Vector3 offset = new Vector3(-xChange, yChange, zChange);
            if (offset != Vector3.zero)
            {
                // Offset player
                OffsetPlayerController(offset);

                // Offset streaming world
                OffsetChildren(StreamingWorld.StreamingTarget.gameObject, offset);

                // Offset loaded enemies
                // Not that many in DFU, but it happens in mods
                foreach (EnemyMotor enemy in FindObjectsOfType<EnemyMotor>())
                    enemy.AdjustLastGrounded(offset.y);

                // Raise event
                RaiseOnPositionUpdateEvent(offset);
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
            groundMotor.ClearActivePlatform();
            DFPosition mapPixel = StreamingWorld.LocalPlayerGPS.CurrentMapPixel;
            currentMapPixel = mapPixel;
            lastMapPixel = mapPixel;
        }

        bool CheckMapPosition()
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
            groundMotor.ClearActivePlatform();
            acrobatMotor.AdjustFallStart(offset.y);
            playerMotor.transform.position += offset;
            StreamingWorld.OffsetWorldCompensation(offset, true);
        }

        void OffsetChildren(GameObject parent, Vector3 offset)
        {
            foreach (Transform child in parent.transform)
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
        }

        #endregion
    }
}