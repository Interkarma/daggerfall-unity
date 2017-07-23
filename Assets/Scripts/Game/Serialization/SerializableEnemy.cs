// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
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
using DaggerfallWorkshop.Game.Questing;

namespace DaggerfallWorkshop.Game.Serialization
{
    public class SerializableEnemy : MonoBehaviour, ISerializableGameObject
    {
        #region Fields

        DaggerfallEnemy enemy;

        #endregion

        #region Unity

        void Awake()
        {
            enemy = GetComponent<DaggerfallEnemy>();
            if (!enemy)
                throw new Exception("DaggerfallEnemy not found.");
        }

        void Start()
        {
            if (LoadID != 0)
                SaveLoadManager.RegisterSerializableGameObject(this);
        }

        void OnDestroy()
        {
            if (LoadID != 0)
                SaveLoadManager.DeregisterSerializableGameObject(this);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up, "LoadID=" + LoadID);
        }
#endif

#endregion

        #region ISerializableGameObject

        public ulong LoadID { get { return GetLoadID(); } }
        public bool ShouldSave { get { return HasChanged(); } }

        public object GetSaveData()
        {
            if (!enemy)
                return null;

            // Get entity behaviour
            DaggerfallEntityBehaviour entityBehaviour = enemy.GetComponent<DaggerfallEntityBehaviour>();
            if (!entityBehaviour)
                return null;

            // Create save data
            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            EnemyMotor motor = enemy.GetComponent<EnemyMotor>();
            DaggerfallMobileUnit mobileEnemy = enemy.GetComponentInChildren<DaggerfallMobileUnit>();
            EnemyData_v1 data = new EnemyData_v1();
            data.loadID = LoadID;
            data.gameObjectName = entityBehaviour.gameObject.name;
            data.currentPosition = enemy.transform.position;
            data.currentRotation = enemy.transform.rotation;
            data.entityType = entity.EntityType;
            data.careerName = entity.Career.Name;
            data.careerIndex = entity.CareerIndex;
            data.startingHealth = entity.MaxHealth;
            data.currentHealth = entity.CurrentHealth;
            data.currentFatigue = entity.CurrentFatigue;
            data.currentMagicka = entity.CurrentMagicka;
            data.isHostile = motor.IsHostile;
            data.isDead = (entity.CurrentHealth <= 0) ? true : false;
            data.questSpawn = enemy.QuestSpawn;
            data.mobileGender = mobileEnemy.Summary.Enemy.Gender;

            // Add quest resource data if present
            QuestResourceBehaviour questResourceBehaviour = GetComponent<QuestResourceBehaviour>();
            if (questResourceBehaviour)
            {
                data.questResource = questResourceBehaviour.GetSaveData();
            }

            return data;
        }

        public void RestoreSaveData(object dataIn)
        {
            if (!enemy)
                return;

            EnemyData_v1 data = (EnemyData_v1)dataIn;
            if (data.loadID != LoadID)
                return;

            DaggerfallEntityBehaviour entityBehaviour = enemy.GetComponent<DaggerfallEntityBehaviour>();
            EnemySenses senses = enemy.GetComponent<EnemySenses>();
            EnemyMotor motor = enemy.GetComponent<EnemyMotor>();
            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;

            // Restore enemy career or class if different
            if (entity == null || entity.EntityType != data.entityType || entity.CareerIndex != data.careerIndex)
            {
                SetupDemoEnemy setupEnemy = enemy.GetComponent<SetupDemoEnemy>();
                setupEnemy.ApplyEnemySettings(data.entityType, data.careerIndex, data.mobileGender, data.isHostile);
                setupEnemy.AlignToGround();

                if (entity == null)
                    entity = entityBehaviour.Entity as EnemyEntity;
            }

            // Quiesce entity during state restore
            entity.Quiesce = true;

            // Restore enemy data
            entityBehaviour.gameObject.name = data.gameObjectName;
            enemy.transform.position = data.currentPosition;
            enemy.transform.rotation = data.currentRotation;
            entity.MaxHealth = data.startingHealth;
            entity.CurrentHealth = data.currentHealth;
            entity.CurrentFatigue = data.currentFatigue;
            entity.CurrentMagicka = data.currentMagicka;
            motor.IsHostile = data.isHostile;
            senses.HasEncounteredPlayer = true;

            // Disable dead enemies
            if (data.isDead)
            {
                entityBehaviour.gameObject.SetActive(false);
            }

            // Restore quest resource link
            enemy.QuestSpawn = data.questSpawn;
            if (enemy.QuestSpawn)
            {
                // Add QuestResourceBehaviour to GameObject
                QuestResourceBehaviour questResourceBehaviour = entityBehaviour.gameObject.AddComponent<QuestResourceBehaviour>();
                questResourceBehaviour.RestoreSaveData(data.questResource);
            }

            // Resume entity
            entity.Quiesce = false;
        }

        #endregion

        #region Private Methods

        bool HasChanged()
        {
            if (!enemy)
                return false;

            // Always save enemy if a quest spawn
            if (enemy.QuestSpawn)
                return true;

            // Get references
            DaggerfallEntityBehaviour entityBehaviour = enemy.GetComponent<DaggerfallEntityBehaviour>();
            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            EnemySenses senses = enemy.GetComponent<EnemySenses>();

            // Save enemy if it has ever encountered player or if any vital signs have dropped
            // Enemy should otherwise still be in starting state
            bool save = false;
            if (senses.HasEncounteredPlayer ||
                entity.CurrentHealth < entity.MaxHealth ||
                entity.CurrentFatigue < entity.MaxFatigue ||
                entity.CurrentMagicka < entity.MaxMagicka)
            {
                save = true;
            }

            return save;
        }

        ulong GetLoadID()
        {
            if (!enemy)
                return 0;

            return enemy.LoadID;
        }

        #endregion
    }
}