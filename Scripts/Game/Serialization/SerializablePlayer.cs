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
using DaggerfallWorkshop.Game.Entity;

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
        PlayerMotor playerMotor;
        DaggerfallEntityBehaviour playerEntityBehaviour;

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

            playerMotor = GetComponent<PlayerMotor>();
            if (!playerMotor)
                throw new Exception("PlayerMotor not found.");

            playerEntityBehaviour = GetComponent<DaggerfallEntityBehaviour>();
            if (!playerEntityBehaviour)
                throw new Exception("PlayerEntityBehaviour not found.");
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

            // Store player entity data
            PlayerEntity entity = playerEntityBehaviour.Entity as PlayerEntity;
            data.playerEntity = new PlayerEntityData_v1();
            data.playerEntity.gender = entity.Gender;
            data.playerEntity.raceTemplate = entity.Race;
            data.playerEntity.faceIndex = entity.FaceIndex;
            data.playerEntity.reflexes = entity.Reflexes;
            data.playerEntity.careerTemplate = entity.Career;
            data.playerEntity.name = entity.Name;
            data.playerEntity.level = entity.Level;
            data.playerEntity.stats = entity.Stats;
            data.playerEntity.skills = entity.Skills;
            data.playerEntity.maxHealth = entity.MaxHealth;
            data.playerEntity.currentHealth = entity.CurrentHealth;
            data.playerEntity.currentFatigue = entity.CurrentFatigue;
            data.playerEntity.currentMagicka = entity.CurrentMagicka;

            // Store player position data
            data.playerPosition = new PlayerPositionData_v1();
            data.playerPosition.position = transform.position;
            data.playerPosition.yaw = playerMouseLook.Yaw;
            data.playerPosition.pitch = playerMouseLook.Pitch;
            data.playerPosition.isCrouching = playerMotor.IsCrouching;
            data.playerPosition.worldPosX = streamingWorld.LocalPlayerGPS.WorldX;
            data.playerPosition.worldPosZ = streamingWorld.LocalPlayerGPS.WorldZ;
            data.playerPosition.worldCompensation = streamingWorld.WorldCompensation;
            data.playerPosition.insideDungeon = playerEnterExit.IsPlayerInsideDungeon;

            return data;
        }

        public void RestoreSaveData(object dataIn)
        {
            if (!playerEnterExit || !streamingWorld || !playerCamera || !playerMouseLook)
                return;

            // Restore player entity data
            PlayerData_v1 data = (PlayerData_v1)dataIn;
            PlayerEntity entity = playerEntityBehaviour.Entity as PlayerEntity;
            entity.Gender = data.playerEntity.gender;
            entity.Race = data.playerEntity.raceTemplate;
            entity.FaceIndex = data.playerEntity.faceIndex;
            entity.Reflexes = data.playerEntity.reflexes;
            entity.Career = data.playerEntity.careerTemplate;
            entity.Name = data.playerEntity.name;
            entity.Level = data.playerEntity.level;
            entity.Stats = data.playerEntity.stats;
            entity.Skills = data.playerEntity.skills;
            entity.MaxHealth = data.playerEntity.maxHealth;
            entity.CurrentHealth = data.playerEntity.currentHealth;
            entity.CurrentFatigue = data.playerEntity.currentFatigue;
            entity.CurrentMagicka = data.playerEntity.currentMagicka;

            // TODO: Restore player position data
            transform.position = data.playerPosition.position;
            playerMouseLook.Yaw = data.playerPosition.yaw;
            playerMouseLook.Pitch = data.playerPosition.pitch;
            playerMotor.IsCrouching = data.playerPosition.isCrouching;
        }

        #endregion
    }
}