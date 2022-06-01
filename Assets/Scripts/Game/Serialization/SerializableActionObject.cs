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
    public class SerializableActionObject : MonoBehaviour, ISerializableGameObject
    {
        #region Fields

        DaggerfallAction action;

        #endregion

        #region Unity

        void Awake()
        {
            action = GetComponent<DaggerfallAction>();
            if (!action)
                throw new Exception("DaggerfallAction not found.");
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

        #endregion

        #region ISerializableGameObject

        public ulong LoadID { get { return GetLoadID(); } }
        public bool ShouldSave { get { return HasChanged(); } }

        public object GetSaveData()
        {
            if (!action)
                return null;

            ActionObjectData_v1 data = new ActionObjectData_v1();
            data.loadID = LoadID;
            data.currentPosition = action.transform.position;
            data.currentRotation = action.transform.rotation;
            data.currentState = action.CurrentState;

            if (action.IsMoving)
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
            ActionObjectData_v1 data = (ActionObjectData_v1)dataIn;
            if (data.loadID == LoadID)
            {
                action.transform.position = data.currentPosition;
                action.transform.rotation = data.currentRotation;
                action.CurrentState = data.currentState;
                action.RestartTween(1 - data.actionPercentage);
            }
        }

        #endregion

        #region Private Methods

        bool HasChanged()
        {
            if (!action)
                return false;

            // Save when action is not in starting position
            if (action.CurrentState != ActionState.Start)
                return true;

            return false;
        }

        ulong GetLoadID()
        {
            if (!action)
                return 0;

            return action.LoadID;
        }

        #endregion
    }
}