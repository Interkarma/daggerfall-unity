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

        void Awake()
        {
            loot = GetComponent<DaggerfallLoot>();
            if (!loot)
                throw new Exception("DaggerfallLoot not found.");
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

        public long LoadID { get { return GetLoadID(); } }
        public bool ShouldSave { get { return HasChanged(); } }

        public object GetSaveData()
        {
            if (!loot)
                return null;

            // Create save data
            LootContainerData_v1 data = new LootContainerData_v1();
            data.loadID = LoadID;
            data.containerType = loot.ContainerType;
            data.currentPosition = loot.transform.position;
            data.textureArchive = loot.TextureArchive;
            data.textureRecord = loot.TextureRecord;
            data.lootTableKey = loot.LootTableKey;
            data.itemList = loot.Items.ToArray();

            return data;
        }

        public void RestoreSaveData(object dataIn)
        {
            if (!loot)
                return;

            LootContainerData_v1 data = (LootContainerData_v1)dataIn;
            if (data.loadID != LoadID)
                return;

            DaggerfallBillboard billboard = loot.GetComponent<DaggerfallBillboard>();

            // Restore position
            loot.transform.position = data.currentPosition;

            // Restore billboard appearance if present
            if (billboard)
            {
                billboard.SetMaterial(data.textureArchive, data.textureRecord, 0, true);
            }

            // Restore items
            loot.Items.Clear();
            loot.Items.AddRange(data.itemList);

            // Restore other data
            loot.ContainerType = data.containerType;
            loot.LootTableKey = data.lootTableKey;
            loot.TextureArchive = data.textureArchive;
            loot.TextureRecord = data.textureRecord;

            // TODO: Remove loot container if empty
        }

        #endregion

        #region Private Methods

        bool HasChanged()
        {
            // Always save loot containers
            return true;
        }

        long GetLoadID()
        {
            return loot.LoadID;
        }

        #endregion
    }
}