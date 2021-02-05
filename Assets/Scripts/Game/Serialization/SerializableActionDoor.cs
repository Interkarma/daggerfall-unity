// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
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
    /// Implements ActionDoor serialization. Should be attached to every DaggerfallActionDoor GameObject.
    /// </summary>
    public class SerializableActionDoor : MonoBehaviour, ISerializableGameObject
    {
        #region Fields

        DaggerfallActionDoor[] allActionDoors;

        #endregion

        #region Unity

        void Awake()
        {
            allActionDoors = GetComponentsInChildren<DaggerfallActionDoor>();
            if (!allActionDoors[0])
                throw new Exception("DaggerfallActionDoor not found.");
        }

        void Start()
        {
            if (LoadID != 0)
            {
                // Using same hack ID fix as SerializableEnemy
                if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon && allActionDoors[0])
                {
                    while (SaveLoadManager.StateManager.ContainsActionDoor(allActionDoors[0].LoadID))
                        allActionDoors[0].LoadID++;
                }

                SaveLoadManager.RegisterSerializableGameObject(this);
            }
        }

        void OnDestroy()
        {
            if (LoadID != 0)
                SaveLoadManager.DeregisterSerializableGameObject(this);
        }

        #endregion

        #region ISerializableGameObject

        public ulong LoadID { get { return GetLoadID(); } }
        public bool ShouldSave { get { return HasChanged(); } }

        public object GetSaveData()
        {
            if (!allActionDoors[0])
                return null;

            ActionDoorData_v1 data = new ActionDoorData_v1();
            data.loadID = LoadID;
            data.currentLockValue = allActionDoors[0].CurrentLockValue;
            data.currentRotation = transform.rotation;
            data.currentState = allActionDoors[0].CurrentState;
            data.lockpickFailedSkillLevel = allActionDoors[0].FailedSkillLevel;

            if (allActionDoors[0].IsMoving)
            {
                __ExternalAssets.iTween tween = GetComponent<__ExternalAssets.iTween>();
                if (tween)
                {
                    data.actionPercentage = tween.Percentage;
                }
            }

            return data;
        }

        public void RestoreSaveData(object dataIn)
        {
            ActionDoorData_v1 data = (ActionDoorData_v1)dataIn;
            if (data.loadID == LoadID)
            {
                allActionDoors[0].CurrentLockValue = data.currentLockValue;
                allActionDoors[0].transform.rotation = data.currentRotation;
                allActionDoors[0].CurrentState = data.currentState;
                allActionDoors[0].RestartTween(1 - data.actionPercentage);
                allActionDoors[0].FailedSkillLevel = data.lockpickFailedSkillLevel;
            }
        }

        #endregion

        #region Private Methods

        bool HasChanged()
        {
            if (!allActionDoors[0])
                return false;

            // Save when door not closed or lock value has changed
            // Door is otherwise in starting closed position with initial lock value
            if (allActionDoors[0].CurrentState != ActionState.Start || allActionDoors[0].CurrentLockValue != allActionDoors[0].StartingLockValue)
                return true;

            return false;
        }

        ulong GetLoadID()
        {
            if (!allActionDoors[0])
                return 0;

            return allActionDoors[0].LoadID;
        }

        #endregion
    }
}