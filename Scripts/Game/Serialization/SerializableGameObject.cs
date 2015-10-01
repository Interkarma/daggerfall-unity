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
    /// Implement this interface with any MonoBehaviour-derived class that can save/load state.
    /// Classes implementing this interface must also register/deregister themselves to SaveLoadManager.
    /// Only registered objects will be serialized/deserialized. If a deserialized object of the specified
    /// LoadID cannot be found then that object will not have any state restored.
    /// </summary>
    public interface ISerializableGameObject
    {
        /// <summary>
        /// ID used to match serialized objects to runtime objects.
        /// Must be unique for its data type and always reference the same object when procedural scene is recreated.
        /// Object will not be serialized if left at default value of 0 or if ID collision detected.
        /// Serialization class may not have enough information by itself to generate ID.
        /// e.g. It may be necessary for scene builder to create a unique ID during procedural layout.
        /// </summary>
        long LoadID { get; }

        /// <summary>
        /// Return true if object should be saved.
        /// Can return false if object is equivalent to a new instance.
        /// e.g. Only save doors which are unlocked or open, no need to save every single door.
        /// </summary>
        bool ShouldSave { get; }

        /// <summary>
        /// Get object state data to serialize.
        /// </summary>
        object GetSaveData();

        /// <summary>
        /// Restore object state from serialized data onto a fresh procedural layout.
        /// It will usually be necessary to adjust runtime state to match saved state.
        /// e.g. Open doors must be set open, dead enemies must be despawned.
        /// </summary>
        void RestoreSaveData(object dataIn);
    }

    #region Root Data

    [fsObject("v1")]
    public class SaveData_v1
    {
        public PlayerData_v1 playerData;
        public DungeonData_v1 dungeonData;
    }

    #endregion

    #region Player Data

    [fsObject("v1")]
    public class PlayerData_v1
    {
        public Vector3 position;
        public float yaw;
        public float pitch;
        public int worldPosX;
        public int worldPosZ;
        public Vector3 worldCompensation;
        public bool insideDungeon;
    }

    #endregion

    #region Dungeon Data

    [fsObject("v1")]
    public class DungeonData_v1
    {
        public ActionDoorData_v1[] actionDoors;
        public ActionObjectData_v1[] actionObjects;
    }

    #endregion

    #region ActionDoor Data

    [fsObject("v1")]
    public class ActionDoorData_v1
    {
        public long loadID;
        public int currentLockValue;
        public Quaternion currentRotation;
        public ActionState currentState;
        public float actionPercentage;
    }

    #endregion

    #region Action Data

    [fsObject("v1")]
    public class ActionObjectData_v1
    {
        public long loadID;
        public Vector3 currentPosition;
        public Quaternion currentRotation;
        public ActionState currentState;
        public float actionPercentage;
    }

    #endregion
}