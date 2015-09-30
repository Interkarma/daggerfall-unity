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
using FullSerializer;

namespace DaggerfallWorkshop.Game.Serialization
{
    /// <summary>
    /// Implements Player serialization. Should be attached to Player GameObject.
    /// </summary>
    public class SerializablePlayer : MonoBehaviour, ISerializableGameObject
    {
        #region Fields

        PlayerEnterExit playerEnterExit;
        StreamingWorld streamingWorld;
        Camera playerCamera;
        PlayerMouseLook playerMouseLook;

        #endregion

        #region Unity

        void Awake()
        {
            playerEnterExit = GetComponent<PlayerEnterExit>();
            if (!playerEnterExit)
                throw new Exception("PlayerEnterExit not found.");

            streamingWorld = GameObject.FindObjectOfType<StreamingWorld>();
            if (!streamingWorld)
                throw new Exception("StreamingWorld not found.");

            playerCamera = GetComponentInChildren<Camera>();
            if (!playerCamera)
                throw new Exception("Player Camera not found.");

            playerMouseLook = playerCamera.GetComponent<PlayerMouseLook>();
            if (!playerMouseLook)
                throw new Exception("PlayerMouseLook not found.");
        }

        void Start()
        {
            SaveLoadManager.RegisterSerializableGameObject(this);
        }

        void OnDestroy()
        {
            SaveLoadManager.DeregisterSerializableGameObject(this);
        }

        #endregion

        #region ISerializableGameObject

        public long LoadID { get { return 1; } }            // Only saves local player
        public bool ShouldSave { get { return true; } }     // Always save player

        public object GetSaveData()
        {
            if (!playerEnterExit || !streamingWorld)
                return null;

            PlayerData_v1 data = new PlayerData_v1();
            data.position = transform.position;
            data.bodyRotation = transform.rotation;
            data.cameraRotation = playerCamera.transform.rotation; ;
            data.worldPosX = streamingWorld.LocalPlayerGPS.WorldX;
            data.worldPosZ = streamingWorld.LocalPlayerGPS.WorldZ;
            data.worldCompensation = streamingWorld.WorldCompensation;
            data.insideDungeon = playerEnterExit.IsPlayerInsideDungeon;

            return data;
        }

        public void RestoreSaveData(object dataIn)
        {
            if (!playerEnterExit || !streamingWorld || !playerCamera || !playerMouseLook)
                return;

            PlayerData_v1 data = (PlayerData_v1)dataIn;
            transform.position = data.position;
            playerMouseLook.SetPlayerFacing(data.bodyRotation, data.cameraRotation);
        }

        #endregion
    }
}