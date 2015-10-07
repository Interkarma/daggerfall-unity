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

            // Get career type
            EnemyEntity entity = entityBehaviour.Entity as EnemyEntity;
            MonsterCareers monsterCareer = MonsterCareers.None;
            ClassCareers classCareer = ClassCareers.None;
            if (entity.EntityType == EntityTypes.EnemyMonster)
                monsterCareer = (MonsterCareers)entity.CareerIndex;
            else if (entity.EntityType == EntityTypes.EnemyClass)
                classCareer = (ClassCareers)entity.CareerIndex;
            else
                return null;

            // Create save data
            EnemyData_v1 data = new EnemyData_v1();
            data.loadID = LoadID;
            data.currentPosition = enemy.transform.position;
            data.currentRotation = enemy.transform.rotation;
            data.entityType = entity.EntityType;
            data.monsterCareer = monsterCareer;
            data.classCareer = classCareer;
            data.startingHealth = entity.MaxHealth;
            data.currentHealth = entity.CurrentHealth;
            data.currentFatigue = entity.CurrentFatigue;
            data.currentMagicka = entity.CurrentMagicka;

            return data;
        }

        public void RestoreSaveData(object dataIn)
        {
            if (!enemy)
                return;

            EnemyData_v1 data = (EnemyData_v1)dataIn;
            if (data.loadID == LoadID)
            {
                enemy.transform.position = data.currentPosition;
                enemy.transform.rotation = data.currentRotation;
            }
        }

        #endregion

        #region Private Methods

        bool HasChanged()
        {
            if (!enemy)
                return false;

            return true;
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