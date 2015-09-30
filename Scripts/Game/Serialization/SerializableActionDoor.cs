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
            if (!actionDoor)
                return null;

            ActionDoorData_v1 data = new ActionDoorData_v1();
            data.loadID = LoadID;
            data.isOpen = actionDoor.IsOpen;
            data.startingLockValue = actionDoor.StartingLockValue;
            data.currentLockValue = actionDoor.CurrentLockValue;

            return data;
        }

        public void RestoreSaveData(object dataIn)
        {
            ActionDoorData_v1 data = (ActionDoorData_v1)dataIn;
            if (data.loadID == LoadID)
            {
                actionDoor.StartingLockValue = data.startingLockValue;
                actionDoor.CurrentLockValue = data.currentLockValue;
                actionDoor.SetOpen(data.isOpen, true, true);
            }
        }

        #endregion

        #region Private Methods

        bool HasChanged()
        {
            if (!actionDoor)
                return false;

            // Save when door is open or lock value has changed
            // Door is otherwise in starting closed position with initial lock value
            if (actionDoor.IsOpen || actionDoor.CurrentLockValue != actionDoor.StartingLockValue)
                return true;

            return false;
        }

        long GetLoadID()
        {
            if (!actionDoor)
                return 0;

            return actionDoor.LoadID;
        }

        #endregion
    }
}