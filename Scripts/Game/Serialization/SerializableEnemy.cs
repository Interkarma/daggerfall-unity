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
            SaveLoadManager.RegisterSerializableGameObject(this);
        }

        void OnDestroy()
        {
            SaveLoadManager.DeregisterSerializableGameObject(this);
        }

        #endregion

        #region ISerializableGameObject

        public long LoadID { get { return GetLoadID(); } }
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
            EnemyData_v1 data = new EnemyData_v1();
            data.loadID = LoadID;
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
            if (entity.EntityType != data.entityType || entity.CareerIndex != data.careerIndex)
            {
                SetupDemoEnemy setupEnemy = enemy.GetComponent<SetupDemoEnemy>();
                setupEnemy.ApplyEnemySettings(data.entityType, data.careerIndex, data.isHostile);
                setupEnemy.AlignToGround();
            }

            // Restore enemy position
            enemy.transform.position = data.currentPosition;
            enemy.transform.rotation = data.currentRotation;
            entity.MaxHealth = data.startingHealth;
            entity.CurrentHealth = data.currentHealth;
            entity.CurrentFatigue = data.currentFatigue;
            entity.CurrentMagicka = data.currentMagicka;
            motor.IsHostile = data.isHostile;
            senses.HasEncounteredPlayer = true;

            // Set monster as dead
            if (data.isDead)
            {
                EnemyDeath enemyDeath = enemy.GetComponent<EnemyDeath>();
                if (enemyDeath)
                {
                    enemyDeath.Die(false);
                }
            }
        }

        #endregion

        #region Private Methods

        bool HasChanged()
        {
            if (!enemy)
                return false;

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

        long GetLoadID()
        {
            if (!enemy)
                return 0;

            return enemy.LoadID;
        }

        #endregion
    }
}