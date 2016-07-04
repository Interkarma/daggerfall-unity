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
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.Serialization
{
    public class SerializableLootContainer : MonoBehaviour, ISerializableGameObject
    {
        #region Fields

        DaggerfallLoot loot;

        #endregion

        #region Unity

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

        public long LoadID { get { return GetLoadID(); } }
        public bool ShouldSave { get { return HasChanged(); } }

        public object GetSaveData()
        {
            return loot;
        }

        public void RestoreSaveData(object dataIn)
        {
        }

        #endregion

        #region Private Methods

        bool HasChanged()
        {
            return false;
        }

        long GetLoadID()
        {
            return loot.LoadID;
        }

        #endregion
    }
}