// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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

        DaggerfallActionDoor actionDoor;

        #endregion

        #region Unity

        void Awake()
        {
            actionDoor = GetComponent<DaggerfallActionDoor>();
            if (!actionDoor)
                throw new Exception("DaggerfallActionDoor not found.");
        }

        void Start()
        {
            if (LoadID != 0)
            {
                // Using same hack ID fix as SerializableEnemy
                if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon && actionDoor)
                {
                    while (SaveLoadManager.StateManager.ContainsActionDoor(actionDoor.LoadID))
                        actionDoor.LoadID++;
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
            if (!actionDoor)
                return null;

            ActionDoorData_v1 data = new ActionDoorData_v1();
            data.loadID = LoadID;
            data.currentLockValue = actionDoor.CurrentLockValue;
            data.currentRotation = transform.rotation;
            data.currentState = actionDoor.CurrentState;
            data.lockpickFailedSkillLevel = actionDoor.FailedSkillLevel;

            if (actionDoor.IsMoving)
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
                actionDoor.CurrentLockValue = data.currentLockValue;
                actionDoor.transform.rotation = data.currentRotation;
                actionDoor.CurrentState = data.currentState;
                actionDoor.RestartTween(1 - data.actionPercentage);
                actionDoor.FailedSkillLevel = data.lockpickFailedSkillLevel;
            }
        }

        #endregion

        #region Private Methods

        bool HasChanged()
        {
            if (!actionDoor)
                return false;

            // Save when door not closed or lock value has changed
            // Door is otherwise in starting closed position with initial lock value
            if (actionDoor.CurrentState != ActionState.Start || actionDoor.CurrentLockValue != actionDoor.StartingLockValue)
                return true;

            return false;
        }

        ulong GetLoadID()
        {
            if (!actionDoor)
                return 0;

            return actionDoor.LoadID;
        }

        #endregion
    }
}